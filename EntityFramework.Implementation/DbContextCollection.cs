using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Models;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
/* 
 * Copyright (C) 2014 Mehdi El Gueddari
 * http://mehdi.me
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Messaging;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Numero3.EntityFramework.Implementation
{
    /// <summary>
    /// As its name suggests, DbContextCollection maintains a collection of DbContext instances.
    /// 
    /// What it does in a nutshell:
    /// - Lazily instantiates DbContext instances when its Get Of TDbContext () method is called
    /// (and optionally starts an explicit database transaction).
    /// - Keeps track of the DbContext instances it created so that it can return the existing
    /// instance when asked for a DbContext of a specific type.
    /// - Takes care of committing / rolling back changes and transactions on all the DbContext
    /// instances it created when its Commit() or Rollback() method is called.
    /// 
    /// </summary>
    public class DbContextCollection : IDbContextCollection
    {
        private Dictionary<Type, DbContext> _initializedDbContexts;
        private Dictionary<DbContext, DbContextTransaction> _transactions;
        private IsolationLevel? _isolationLevel;
        private readonly IDbContextFactory _dbContextFactory;
        private bool _disposed;
        private bool _completed;
        private bool _readOnly;

        internal Dictionary<Type, DbContext> InitializedDbContexts { get { return _initializedDbContexts; } }

        public DbContextCollection(bool readOnly = false, IsolationLevel? isolationLevel = null, IDbContextFactory dbContextFactory = null)
        {
            _disposed = false;
            _completed = false;

            _initializedDbContexts = new Dictionary<Type, DbContext>();
            _transactions = new Dictionary<DbContext, DbContextTransaction>();

            _readOnly = readOnly;
            _isolationLevel = isolationLevel;
            _dbContextFactory = dbContextFactory;
        }

        public TDbContext Get<TDbContext>(ServiceHeader serviceHeader) where TDbContext : DbContext
        {
            if (_disposed)
                throw new ObjectDisposedException("DbContextCollection");

            var requestedType = typeof(TDbContext);

            if (!_initializedDbContexts.ContainsKey(requestedType))
            {
                // First time we've been asked for this particular DbContext type.
                // Create one, cache it and start its database transaction if needed.
                var dbContext = _dbContextFactory != null
                    ? _dbContextFactory.CreateDbContext<TDbContext>(serviceHeader)
                    : Activator.CreateInstance<TDbContext>();

                dbContext.Database.Log = Log;

                _initializedDbContexts.Add(requestedType, dbContext);

                if (_readOnly)
                {
                    dbContext.Configuration.AutoDetectChangesEnabled = false;
                }

                if (_isolationLevel.HasValue)
                {
                    var tran = dbContext.Database.BeginTransaction(_isolationLevel.Value);
                    _transactions.Add(dbContext, tran);
                }
            }

            return _initializedDbContexts[requestedType] as TDbContext;
        }

        public int Commit(ServiceHeader serviceHeader)
        {
            if (_disposed)
                throw new ObjectDisposedException("DbContextCollection");
            if (_completed)
                throw new InvalidOperationException("You can't call Commit() or Rollback() more than once on a DbContextCollection. All the changes in the DbContext instances managed by this collection have already been saved or rollback and all database transactions have been completed and closed. If you wish to make more data changes, create a new DbContextCollection and make your changes there.");

            // Best effort. You'll note that we're not actually implementing an atomic commit 
            // here. Its entirely possible that one DbContext instance will be committed successfully
            // and another will fail. Implementing an atomic commit would require us to wrap
            // all of this in a TransactionScope. The problem with TransactionScope is that 
            // the database transaction it creates may be automatically promoted to a 
            // distributed transaction if our DbContext instances happen to be using different 
            // databases. And that would require the DTC service (Distributed Transaction Coordinator)
            // to be enabled on all of our live and dev servers as well as on all of our dev workstations.
            // Otherwise the whole thing would blow up at runtime. 

            // In practice, if our services are implemented following a reasonably DDD approach,
            // a business transaction (i.e. a service method) should only modify entities in a single
            // DbContext. So we should never find ourselves in a situation where two DbContext instances
            // contain uncommitted changes here. We should therefore never be in a situation where the below
            // would result in a partial commit. 

            ExceptionDispatchInfo lastError = null;

            var c = 0;

            List<AuditLogBCP> auditLogBCPList = new List<AuditLogBCP>();

            List<QueueDTO> accountAlertList = new List<QueueDTO>();

            foreach (var dbContext in _initializedDbContexts.Values)
            {
                try
                {
                    if (!_readOnly)
                    {
                        // Get all Added/Deleted/Modified entities (not Unmodified or Detached)
                        foreach (var entry in dbContext.ChangeTracker.Entries().Where(p => p.State == EntityState.Added || p.State == EntityState.Deleted || p.State == EntityState.Modified))
                        {
                            // For each changed record, get the audit record entries and add them
                            var auditInfoWrapper = GetAuditRecordsForChange(dbContext, entry);

                            if (auditInfoWrapper != null)
                            {
                                auditLogBCPList.Add(auditInfoWrapper.GetBulkCopyEntry(serviceHeader));

                                if (auditInfoWrapper.TableName.Equals(string.Format("{0}Journals", DefaultSettings.Instance.TablePrefix), StringComparison.OrdinalIgnoreCase))
                                {
                                    foreach (var item in auditInfoWrapper.AuditInfoCollection)
                                    {
                                        switch (item.ColumnName)
                                        {
                                            case "ParentId":
                                                if (string.IsNullOrWhiteSpace(item.NewValue))
                                                {
                                                    var queueDTO = new QueueDTO
                                                    {
                                                        RecordId = Guid.Parse(auditInfoWrapper.RecordID),
                                                        AppDomainName = serviceHeader.ApplicationDomainName,
                                                        AccountAlertTrigger = (int)AccountAlertTrigger.Transaction,
                                                    };

                                                    accountAlertList.Add(queueDTO);
                                                }
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }
                            }
                        }

                        if (auditLogBCPList.Any() && serviceHeader != null && !string.IsNullOrWhiteSpace(serviceHeader.ApplicationUserName) && serviceHeader.ApplicationUserName.Equals(string.Format("{0}_{1}", DefaultSettings.Instance.AuditUser, serviceHeader.ApplicationDomainName), StringComparison.OrdinalIgnoreCase))
                            throw new InvalidOperationException("Sorry, but CRUD operations are not permitted!");

                        c += dbContext.SaveChanges();
                    }

                    // If we've started an explicit database transaction, time to commit it now.
                    var tran = GetValueOrDefault(_transactions, dbContext);
                    if (tran != null)
                    {
                        tran.Commit();
                        tran.Dispose();
                    }
                }
                catch (Exception e)
                {
                    lastError = ExceptionDispatchInfo.Capture(e);
                }
            }

            _transactions.Clear();
            _completed = true;

            if (lastError != null)
                lastError.Throw(); // Re-throw while maintaining the exception's original stack trace

            if (auditLogBCPList.Any())
                SendMessage(auditLogBCPList);

            if (accountAlertList.Any())
                SendMessage(accountAlertList);

            return c;
        }

        public Task<int> CommitAsync(ServiceHeader serviceHeader)
        {
            return CommitAsync(serviceHeader, CancellationToken.None);
        }

        public async Task<int> CommitAsync(ServiceHeader serviceHeader, CancellationToken cancelToken)
        {
            if (cancelToken == null)
                throw new ArgumentNullException("cancelToken");
            if (_disposed)
                throw new ObjectDisposedException("DbContextCollection");
            if (_completed)
                throw new InvalidOperationException("You can't call Commit() or Rollback() more than once on a DbContextCollection. All the changes in the DbContext instances managed by this collection have already been saved or rollback and all database transactions have been completed and closed. If you wish to make more data changes, create a new DbContextCollection and make your changes there.");

            // See comments in the sync version of this method for more details.

            ExceptionDispatchInfo lastError = null;

            var c = 0;

            List<AuditLogBCP> auditLogBCPList = new List<AuditLogBCP>();

            List<QueueDTO> accountAlertList = new List<QueueDTO>();

            foreach (var dbContext in _initializedDbContexts.Values)
            {
                try
                {
                    if (!_readOnly)
                    {
                        // Get all Added/Deleted/Modified entities (not Unmodified or Detached)
                        foreach (var entry in dbContext.ChangeTracker.Entries().Where(p => p.State == EntityState.Added || p.State == EntityState.Deleted || p.State == EntityState.Modified))
                        {
                            // For each changed record, get the audit record entries and add them
                            var auditInfoWrapper = GetAuditRecordsForChange(dbContext, entry);

                            if (auditInfoWrapper != null)
                            {
                                auditLogBCPList.Add(auditInfoWrapper.GetBulkCopyEntry(serviceHeader));

                                if (auditInfoWrapper.TableName.Equals(string.Format("{0}Journals", DefaultSettings.Instance.TablePrefix), StringComparison.OrdinalIgnoreCase))
                                {
                                    foreach (var item in auditInfoWrapper.AuditInfoCollection)
                                    {
                                        switch (item.ColumnName)
                                        {
                                            case "ParentId":
                                                if (string.IsNullOrWhiteSpace(item.NewValue))
                                                {
                                                    var queueDTO = new QueueDTO
                                                    {
                                                        RecordId = Guid.Parse(auditInfoWrapper.RecordID),
                                                        AppDomainName = serviceHeader.ApplicationDomainName,
                                                        AccountAlertTrigger = (int)AccountAlertTrigger.Transaction,
                                                    };

                                                    accountAlertList.Add(queueDTO);
                                                }
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }
                            }
                        }

                        if (auditLogBCPList.Any() && serviceHeader != null && !string.IsNullOrWhiteSpace(serviceHeader.ApplicationUserName) && serviceHeader.ApplicationUserName.Equals(string.Format("{0}_{1}", DefaultSettings.Instance.AuditUser, serviceHeader.ApplicationDomainName), StringComparison.OrdinalIgnoreCase))
                            throw new InvalidOperationException("Sorry, but CRUD operations are not permitted!");

                        c += await dbContext.SaveChangesAsync(cancelToken).ConfigureAwait(false);
                    }

                    // If we've started an explicit database transaction, time to commit it now.
                    var tran = GetValueOrDefault(_transactions, dbContext);
                    if (tran != null)
                    {
                        tran.Commit();
                        tran.Dispose();
                    }
                }
                catch (Exception e)
                {
                    lastError = ExceptionDispatchInfo.Capture(e);
                }
            }

            _transactions.Clear();

            _completed = true;

            if (lastError != null)
                lastError.Throw(); // Re-throw while maintaining the exception's original stack trace

            if (auditLogBCPList.Any())
                SendMessage(auditLogBCPList);

            if (accountAlertList.Any())
                SendMessage(accountAlertList);

            return c;
        }

        public void Rollback()
        {
            if (_disposed)
                throw new ObjectDisposedException("DbContextCollection");
            if (_completed)
                throw new InvalidOperationException("You can't call Commit() or Rollback() more than once on a DbContextCollection. All the changes in the DbContext instances managed by this collection have already been saved or rollback and all database transactions have been completed and closed. If you wish to make more data changes, create a new DbContextCollection and make your changes there.");

            ExceptionDispatchInfo lastError = null;

            foreach (var dbContext in _initializedDbContexts.Values)
            {
                // There's no need to explicitly rollback changes in a DbContext as
                // DbContext doesn't save any changes until its SaveChanges() method is called.
                // So "rolling back" for a DbContext simply means not calling its SaveChanges()
                // method. 

                // But if we've started an explicit database transaction, then we must roll it back.
                var tran = GetValueOrDefault(_transactions, dbContext);
                if (tran != null)
                {
                    try
                    {
                        tran.Rollback();
                        tran.Dispose();
                    }
                    catch (Exception e)
                    {
                        lastError = ExceptionDispatchInfo.Capture(e);
                    }
                }
            }

            _transactions.Clear();
            _completed = true;

            if (lastError != null)
                lastError.Throw(); // Re-throw while maintaining the exception's original stack trace
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            // Do our best here to dispose as much as we can even if we get errors along the way.
            // Now is not the time to throw. Correctly implemented applications will have called
            // either Commit() or Rollback() first and would have got the error there.

            if (!_completed)
            {
                try
                {
                    if (_readOnly) Commit(null);
                    else Rollback();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }
            }

            foreach (var dbContext in _initializedDbContexts.Values)
            {
                try
                {
                    dbContext.Dispose();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }
            }

            _initializedDbContexts.Clear();
            _disposed = true;
        }

        /// <summary>
        /// Returns the value associated with the specified key or the default 
        /// value for the TValue  type.
        /// </summary>
        private static TValue GetValueOrDefault<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : default(TValue);
        }

        private static AuditInfoWrapper GetAuditRecordsForChange(DbContext dbContext, DbEntityEntry dbEntry)
        {
            // Get table name 
            string tableName = string.Format("{0}{1}", DefaultSettings.Instance.TablePrefix, dbContext.GetTableName(dbEntry));

            if (tableName.Equals(string.Format("{0}AuditLogs", DefaultSettings.Instance.TablePrefix), StringComparison.OrdinalIgnoreCase))
                return null;

            var entityType = dbEntry.Entity.GetType();

            if (entityType.BaseType != null && entityType.Namespace == "System.Data.Entity.DynamicProxies")
                entityType = entityType.BaseType;

            // Get primary key value (If you have more than one key column, this will need to be adjusted)
            var memberInfo = entityType.GetProperties().SingleOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Count() > 0);

            if (memberInfo == null) return null;

            string keyName = memberInfo.Name;

            List<AuditInfo> infoList = new List<AuditInfo>();

            string eventType = string.Empty;

            string recordID = string.Empty;

            if (dbEntry.State == EntityState.Added)
            {
                foreach (string propertyName in dbEntry.CurrentValues.PropertyNames)
                {
                    var currentValues = string.Empty;
                    if (dbEntry.CurrentValues.GetValue<object>(propertyName) != null)
                    {
                        if (dbEntry.CurrentValues.GetValue<object>(propertyName) is DbPropertyValues)
                            currentValues = ((DbPropertyValues)dbEntry.CurrentValues.GetValue<object>(propertyName)).GetValues();
                        else currentValues = dbEntry.CurrentValues.GetValue<object>(propertyName).ToString();
                    }

                    eventType = "Entity_Added";

                    recordID = dbEntry.CurrentValues.GetValue<object>(keyName).ToString();

                    var addedAuditLog = new AuditInfo
                    {
                        ColumnName = propertyName,
                        OriginalValue = string.Empty,
                        NewValue = currentValues,
                    };

                    infoList.Add(addedAuditLog);
                }
            }
            else if (dbEntry.State == EntityState.Deleted)
            {
                foreach (string propertyName in dbEntry.OriginalValues.PropertyNames)
                {
                    var originalValues = string.Empty;
                    if (dbEntry.OriginalValues.GetValue<object>(propertyName) != null)
                    {
                        if (dbEntry.OriginalValues.GetValue<object>(propertyName) is DbPropertyValues)
                            originalValues = ((DbPropertyValues)dbEntry.OriginalValues.GetValue<object>(propertyName)).GetValues();
                        else originalValues = dbEntry.OriginalValues.GetValue<object>(propertyName).ToString();
                    }

                    eventType = "Entity_Deleted";

                    recordID = dbEntry.OriginalValues.GetValue<object>(keyName).ToString();

                    var deletedAuditLog = new AuditInfo
                    {
                        ColumnName = propertyName,
                        OriginalValue = originalValues,
                        NewValue = string.Empty,
                    };

                    infoList.Add(deletedAuditLog);
                }
            }
            else if (dbEntry.State == EntityState.Modified)
            {
                foreach (string propertyName in dbEntry.OriginalValues.PropertyNames)
                {
                    // For updates, we only want to capture the columns that actually changed
                    if (!object.Equals(dbEntry.OriginalValues.GetValue<object>(propertyName), dbEntry.CurrentValues.GetValue<object>(propertyName)))
                    {
                        var originalValues = string.Empty;
                        if (dbEntry.OriginalValues.GetValue<object>(propertyName) != null)
                        {
                            if (dbEntry.OriginalValues.GetValue<object>(propertyName) is DbPropertyValues)
                                originalValues = ((DbPropertyValues)dbEntry.OriginalValues.GetValue<object>(propertyName)).GetValues();
                            else originalValues = dbEntry.OriginalValues.GetValue<object>(propertyName).ToString();
                        }

                        var currentValues = string.Empty;
                        if (dbEntry.CurrentValues.GetValue<object>(propertyName) != null)
                        {
                            if (dbEntry.CurrentValues.GetValue<object>(propertyName) is DbPropertyValues)
                                currentValues = ((DbPropertyValues)dbEntry.CurrentValues.GetValue<object>(propertyName)).GetValues();
                            else currentValues = dbEntry.CurrentValues.GetValue<object>(propertyName).ToString();
                        }

                        eventType = "Entity_Modified";

                        recordID = dbEntry.OriginalValues.GetValue<object>(keyName).ToString();

                        var modifiedAuditLog = new AuditInfo
                        {
                            ColumnName = propertyName,
                            OriginalValue = originalValues,
                            NewValue = currentValues,
                        };

                        infoList.Add(modifiedAuditLog);
                    }
                }
            }
            // Otherwise, don't do anything, we don't care about Unchanged or Detached entities

            return new AuditInfoWrapper { TableName = tableName, EventType = eventType, RecordID = recordID, AuditInfoCollection = infoList };
        }

        private static void SendMessage(List<AuditLogBCP> data)
        {
            var serviceBrokerConfigSection = ConfigurationHelper.GetServiceBrokerConfiguration();

            #region AuditLog

            if (!MessageQueue.Exists(serviceBrokerConfigSection.ServiceBrokerSettingsItems.AuditLogDispatcherQueuePath))
                MessageQueue.Create(serviceBrokerConfigSection.ServiceBrokerSettingsItems.AuditLogDispatcherQueuePath, true);

            using (MessageQueue messageQueue = new MessageQueue(serviceBrokerConfigSection.ServiceBrokerSettingsItems.AuditLogDispatcherQueuePath, QueueAccessMode.Send))
            {
                messageQueue.Formatter = new BinaryMessageFormatter();

                messageQueue.MessageReadPropertyFilter.SetAll();

                using (MessageQueueTransaction mqt = new MessageQueueTransaction())
                {
                    mqt.Begin();

                    using (Message message = new Message(data, new BinaryMessageFormatter()))
                    {
                        message.Label = string.Format("{0}|{1}", EnumHelper.GetDescription(MessageCategory.AuditLog), EnumHelper.GetDescription(System.Messaging.MessagePriority.High));
                        message.AppSpecific = (int)MessageCategory.AuditLog;
                        message.Priority = System.Messaging.MessagePriority.High;
                        message.TimeToBeReceived = TimeSpan.FromMinutes(serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);

                        messageQueue.Send(message, mqt);
                    }

                    mqt.Commit();
                }
            }

            #endregion

            #region AuditTrail
            
            if (!MessageQueue.Exists(serviceBrokerConfigSection.ServiceBrokerSettingsItems.AuditTrailDispatcherQueuePath))
                MessageQueue.Create(serviceBrokerConfigSection.ServiceBrokerSettingsItems.AuditTrailDispatcherQueuePath, true);

            using (MessageQueue messageQueue = new MessageQueue(serviceBrokerConfigSection.ServiceBrokerSettingsItems.AuditTrailDispatcherQueuePath, QueueAccessMode.Send))
            {
                messageQueue.Formatter = new BinaryMessageFormatter();

                messageQueue.MessageReadPropertyFilter.SetAll();

                using (MessageQueueTransaction mqt = new MessageQueueTransaction())
                {
                    mqt.Begin();

                    using (Message message = new Message(data, new BinaryMessageFormatter()))
                    {
                        message.Label = string.Format("{0}|{1}", EnumHelper.GetDescription(MessageCategory.AuditTrail), EnumHelper.GetDescription(System.Messaging.MessagePriority.High));
                        message.AppSpecific = (int)MessageCategory.AuditTrail;
                        message.Priority = System.Messaging.MessagePriority.High;
                        message.TimeToBeReceived = TimeSpan.FromMinutes(serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);

                        messageQueue.Send(message, mqt);
                    }

                    mqt.Commit();
                }
            }

            #endregion
        }

        private static void SendMessage(List<QueueDTO> data)
        {
            var serviceBrokerConfigSection = ConfigurationHelper.GetServiceBrokerConfiguration();
            
            if (!MessageQueue.Exists(serviceBrokerConfigSection.ServiceBrokerSettingsItems.AccountAlertDispatcherQueuePath))
                MessageQueue.Create(serviceBrokerConfigSection.ServiceBrokerSettingsItems.AccountAlertDispatcherQueuePath, true);

            using (MessageQueue messageQueue = new MessageQueue(serviceBrokerConfigSection.ServiceBrokerSettingsItems.AccountAlertDispatcherQueuePath, QueueAccessMode.Send))
            {
                messageQueue.Formatter = new BinaryMessageFormatter();

                messageQueue.MessageReadPropertyFilter.SetAll();

                using (MessageQueueTransaction mqt = new MessageQueueTransaction())
                {
                    mqt.Begin();

                    using (Message message = new Message(data, new BinaryMessageFormatter()))
                    {
                        message.Label = string.Format("{0}|{1}", EnumHelper.GetDescription(MessageCategory.AccountAlert), EnumHelper.GetDescription(System.Messaging.MessagePriority.High));
                        message.AppSpecific = (int)MessageCategory.AccountAlert;
                        message.Priority = System.Messaging.MessagePriority.High;
                        message.TimeToBeReceived = TimeSpan.FromMinutes(serviceBrokerConfigSection.ServiceBrokerSettingsItems.TimeToBeReceived);

                        messageQueue.Send(message, mqt);
                    }

                    mqt.Commit();
                }
            }
        }

        private static void Log(string message)
        {
            var serviceBrokerConfigSection = ConfigurationHelper.GetServiceBrokerConfiguration();

            if (serviceBrokerConfigSection.ServiceBrokerSettingsItems.LogEnabled == 1)
                LoggerFactory.CreateLog().LogInfo("{0}****{0}EF Message:{0}{1}{0}***{0}", Environment.NewLine, message);
        }
    }

    internal static class Extensions
    {
        public static string GetTableName<T>(this DbContext context) where T : class
        {
            var workspace = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;

            var mappingItemCollection = (StorageMappingItemCollection)workspace.GetItemCollection(DataSpace.CSSpace);

            var storeContainer = ((EntityContainerMapping)mappingItemCollection[0]).StoreEntityContainer;

            var baseEntitySet = storeContainer.BaseEntitySets.Single(es => es.Name == typeof(T).Name);

            return string.Format("{0}.{1}", baseEntitySet.Schema, baseEntitySet.Table);
        }

        public static string GetTableName(this DbContext context, DbEntityEntry ent)
        {
            var objectContext = ((IObjectContextAdapter)context).ObjectContext;

            var entityType = ent.Entity.GetType();

            if (entityType.BaseType != null && entityType.Namespace == "System.Data.Entity.DynamicProxies")
                entityType = entityType.BaseType;

            string entityTypeName = entityType.Name;

            var container = objectContext.MetadataWorkspace.GetEntityContainer(objectContext.DefaultContainerName, DataSpace.CSpace);

            string entitySetName = (from meta in container.BaseEntitySets
                                    where meta.ElementType.Name == entityTypeName
                                    select meta.Name).First();
            return entitySetName;
        }

        public static string GetValues(this DbPropertyValues values)
        {
            var sb = new StringBuilder();

            foreach (var propertyName in values.PropertyNames)
            {
                sb.AppendLine(string.Format("Property '{0}' has value '{1}'", propertyName, values[propertyName]));
            }

            return string.Format("{0}", sb);
        }

        public static AuditLogBCP GetBulkCopyEntry(this AuditInfoWrapper value, ServiceHeader serviceHeader)
        {
            return new AuditLogBCP
            {
                AppDomainName = serviceHeader.ApplicationDomainName,
                TableName = value.TableName,
                EventType = value.EventType,
                RecordID = value.RecordID,
                AuditInfoWrapper = value,
                ApplicationUserName = serviceHeader != null ? serviceHeader.ApplicationUserName : string.Empty,
                EnvironmentUserName = serviceHeader != null ? serviceHeader.EnvironmentUserName : string.Empty,
                EnvironmentMachineName = serviceHeader != null ? serviceHeader.EnvironmentMachineName : string.Empty,
                EnvironmentDomainName = serviceHeader != null ? serviceHeader.EnvironmentDomainName : string.Empty,
                EnvironmentOSVersion = serviceHeader != null ? serviceHeader.EnvironmentOSVersion : string.Empty,
                EnvironmentMACAddress = serviceHeader != null ? serviceHeader.EnvironmentMACAddress : string.Empty,
                EnvironmentMotherboardSerialNumber = serviceHeader != null ? serviceHeader.EnvironmentMotherboardSerialNumber : string.Empty,
                EnvironmentProcessorId = serviceHeader != null ? serviceHeader.EnvironmentProcessorId : string.Empty,
                EnvironmentIPAddress = serviceHeader != null ? serviceHeader.EnvironmentIPAddress : string.Empty,
                CreatedBy = serviceHeader != null ? serviceHeader.ApplicationUserName : string.Empty,
                CreatedDate = DateTime.Now,
            };
        }
    }
}