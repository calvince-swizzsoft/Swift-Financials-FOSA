using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.MessagingModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.MessagingModule.Aggregates.CustomerMessageHistoryAgg;
using Domain.MainBoundedContext.MessagingModule.Aggregates.TextAlertAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Application.MainBoundedContext.DTO.AccountsModule;
using Domain.MainBoundedContext.MessagingModule.Aggregates.TextAlertCommissionAgg;
using LazyCache;

namespace Application.MainBoundedContext.MessagingModule.Services
{
    public class TextAlertAppService : ITextAlertAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<TextAlert> _textAlertRepository;
        private readonly IRepository<TextAlertCommission> _textAlertCommissionRepository;
        private readonly IBranchAppService _branchAppService;
        private readonly IMessageGroupAppService _messageGroupAppService;
        private readonly IBrokerService _brokerService;
        private readonly IAppCache _appCache;

        public TextAlertAppService(
            IDbContextScopeFactory dbContextScopeFactory,
            IRepository<TextAlert> textAlertRepository,
            IRepository<TextAlertCommission> textAlertCommissionRepository,
            IBranchAppService branchAppService,
            IMessageGroupAppService messageGroupAppService,
            IBrokerService brokerService,
            IAppCache appCache)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (textAlertRepository == null)
                throw new ArgumentNullException(nameof(textAlertRepository));

            if (textAlertCommissionRepository == null)
                throw new ArgumentNullException(nameof(textAlertCommissionRepository));

            if (branchAppService == null)
                throw new ArgumentNullException(nameof(branchAppService));

            if (messageGroupAppService == null)
                throw new ArgumentNullException(nameof(messageGroupAppService));

            if (brokerService == null)
                throw new ArgumentNullException(nameof(brokerService));

            if (appCache == null)
                throw new ArgumentNullException(nameof(appCache));

            _dbContextScopeFactory = dbContextScopeFactory;
            _textAlertRepository = textAlertRepository;
            _textAlertCommissionRepository = textAlertCommissionRepository;
            _branchAppService = branchAppService;
            _messageGroupAppService = messageGroupAppService;
            _brokerService = brokerService;
            _appCache = appCache;
        }

        public bool AddNewBulkMessage(BulkMessageDTO bulkMessageDTO, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (bulkMessageDTO != null && !string.IsNullOrEmpty(bulkMessageDTO.TextMessage) && !string.IsNullOrEmpty(bulkMessageDTO.Recipients))
            {
                var textAlertDTOs = new List<TextAlertDTO>();

                var buffer = bulkMessageDTO.Recipients.Trim().Split(new char[] { ',' });

                Array.ForEach(buffer, (Action<string>)(recipientNumber =>
                {
                    if (!textAlertDTOs.Any(x => x.TextMessageRecipient.Equals(recipientNumber, StringComparison.OrdinalIgnoreCase)))
                    {
                        if (Regex.IsMatch(recipientNumber, @"^\+(?:[0-9]??){6,14}[0-9]$") && recipientNumber.Length >= 13)
                        {
                            var textAlertDTO = new TextAlertDTO { BranchId = bulkMessageDTO.BranchId, TextMessageBody = bulkMessageDTO.TextMessage, TextMessageRecipient = recipientNumber, TextMessageOrigin = (int)MessageOrigin.Within, MessageCategory = (int)MessageCategory.SMSAlert, TextMessagePriority = bulkMessageDTO.Priority, AppendSignature = bulkMessageDTO.AppendSignature, TextMessageSecurityCritical = bulkMessageDTO.SecurityCritical };

                            textAlertDTOs.Add(textAlertDTO);
                        }
                    }
                }));

                result = AddNewTextAlerts(textAlertDTOs, serviceHeader);
            }

            return result;
        }

        public bool AddNewUSSDMessage(USSDMessageDTO ussdMessageDTO, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (ussdMessageDTO != null && !string.IsNullOrEmpty(ussdMessageDTO.Message))
            {
                var textAlertDTO = new TextAlertDTO { TextMessageBody = ussdMessageDTO.Message, TextMessageOrigin = (int)MessageOrigin.Within, MessageCategory = (int)MessageCategory.USSDQuery };

                result = AddNewTextAlert(textAlertDTO, serviceHeader) != null;
            }

            return result;
        }

        public TextAlertDTO AddNewTextAlert(TextAlertDTO textAlertDTO, ServiceHeader serviceHeader)
        {
            if (textAlertDTO != null)
            {
                if ((MessageCategory)textAlertDTO.MessageCategory == MessageCategory.SMSAlert && (string.IsNullOrWhiteSpace(textAlertDTO.TextMessageRecipient) || !Regex.IsMatch(textAlertDTO.TextMessageRecipient, @"^\+(?:[0-9]??){6,14}[0-9]$")))
                    return null;

                var textMessageBody = textAlertDTO.TextMessageBody;

                if (textAlertDTO.AppendSignature && textAlertDTO.BranchId != Guid.Empty)
                {
                    var currentBranch = _branchAppService.FindBranch(textAlertDTO.BranchId, serviceHeader);

                    if (currentBranch != null)
                        textMessageBody = string.Format("{0}{1}{2}{3}{4}", textMessageBody, Environment.NewLine, currentBranch.Description, Environment.NewLine, currentBranch.CompanyDescription);
                }

                var dlrStatus = DLRStatus.UnKnown;

                switch ((MessageOrigin)textAlertDTO.TextMessageOrigin)
                {
                    case MessageOrigin.Within:
                    case MessageOrigin.Without:
                        dlrStatus = DLRStatus.Pending;
                        break;
                    case MessageOrigin.Other:
                        dlrStatus = DLRStatus.NotApplicable;
                        break;
                    default:
                        break;
                }

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var textMessage = new TextMessage(textAlertDTO.TextMessageRecipient, textMessageBody, (int)dlrStatus, null, textAlertDTO.TextMessageOrigin, textAlertDTO.TextMessagePriority, 0, textAlertDTO.TextMessageSecurityCritical);

                    var textAlert = TextAlertFactory.CreateTextAlert(textAlertDTO.BranchId, textMessage, serviceHeader);

                    _textAlertRepository.Add(textAlert, serviceHeader);

                    if (dbContextScope.SaveChanges(serviceHeader) >= 0)
                    {
                        var projection = textAlert.ProjectedAs<TextAlertDTO>();

                        if (_brokerService.ProcessTextAlerts(DMLCommand.Insert, serviceHeader, projection))
                        {
                            return projection;
                        }
                        else return null;
                    }
                    else return null;
                }
            }
            else return null;
        }

        public bool AddNewTextAlerts(List<TextAlertDTO> textAlertDTOs, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (textAlertDTOs != null && textAlertDTOs.Any())
            {
                textAlertDTOs.ForEach(item =>
                {
                    AddNewTextAlert(item, serviceHeader);
                });

                result = true;
            }

            return result;
        }

        public bool UpdateTextAlert(TextAlertDTO textAlertDTO, ServiceHeader serviceHeader)
        {
            if (textAlertDTO == null || textAlertDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _textAlertRepository.Get(textAlertDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var text = new TextMessage(persisted.TextMessage.Recipient, persisted.TextMessage.Body, textAlertDTO.TextMessageDLRStatus, textAlertDTO.TextMessageReference, persisted.TextMessage.Origin, textAlertDTO.TextMessagePriority, textAlertDTO.TextMessageSendRetry, persisted.TextMessage.SecurityCritical);

                    var current = TextAlertFactory.CreateTextAlert(textAlertDTO.BranchId, text, serviceHeader);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                    _textAlertRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<TextAlertDTO> FindTextAlerts(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var textAlerts = _textAlertRepository.GetAll(serviceHeader);

                if (textAlerts != null && textAlerts.Any())
                {
                    return textAlerts.ProjectedAsCollection<TextAlertDTO>();
                }
                else return null;
            }
        }

        public List<TextAlertDTO> FindTextAlertsByMessageReference(string messageReference, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = TextAlertSpecifications.TextAlertWithMessageReference(messageReference);

                ISpecification<TextAlert> spec = filter;

                var textAlerts = _textAlertRepository.AllMatching(spec, serviceHeader);

                if (textAlerts != null && textAlerts.Any())
                {
                    return textAlerts.ProjectedAsCollection<TextAlertDTO>();
                }
                else return null;
            }
        }

        public List<TextAlertDTO> FindTextAlertsByDLRStatus(int dlrStatus, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = TextAlertSpecifications.TextAlertWithDLRStatus(dlrStatus);

                ISpecification<TextAlert> spec = filter;

                var textAlerts = _textAlertRepository.AllMatching(spec, serviceHeader);

                if (textAlerts != null && textAlerts.Any())
                {
                    return textAlerts.ProjectedAsCollection<TextAlertDTO>();
                }
                else return null;
            }
        }

        public TextAlertDTO FindTextAlert(Guid textAlertId, ServiceHeader serviceHeader)
        {
            if (textAlertId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var textAlert = _textAlertRepository.Get(textAlertId, serviceHeader);

                    if (textAlert != null)
                    {
                        return textAlert.ProjectedAs<TextAlertDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<TextAlertDTO> FindTextAlerts(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = TextAlertSpecifications.DefaultSpec();

                ISpecification<TextAlert> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var textAlertPagedCollection = _textAlertRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (textAlertPagedCollection != null)
                {
                    var pageCollection = textAlertPagedCollection.PageCollection.ProjectedAsCollection<TextAlertDTO>();

                    var itemsCount = textAlertPagedCollection.ItemsCount;

                    return new PageCollectionInfo<TextAlertDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<TextAlertDTO> FindTextAlerts(int dlrStatus, string text, int pageIndex, int pageSize, int daysCap, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = TextAlertSpecifications.TextAlertFullText(dlrStatus, text, daysCap);

                ISpecification<TextAlert> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var textAlertPagedCollection = _textAlertRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (textAlertPagedCollection != null)
                {
                    var pageCollection = textAlertPagedCollection.PageCollection.ProjectedAsCollection<TextAlertDTO>();

                    var itemsCount = textAlertPagedCollection.ItemsCount;

                    return new PageCollectionInfo<TextAlertDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<TextAlertDTO> FindTextAlerts(int dlrStatus, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = TextAlertSpecifications.TextAlertFullText(dlrStatus, startDate, endDate, text);

                ISpecification<TextAlert> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var textAlertPagedCollection = _textAlertRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (textAlertPagedCollection != null)
                {
                    var pageCollection = textAlertPagedCollection.PageCollection.ProjectedAsCollection<TextAlertDTO>();

                    var itemsCount = textAlertPagedCollection.ItemsCount;

                    return new PageCollectionInfo<TextAlertDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public List<TextAlertDTO> FindTextAlertsByDLRStatusAndOrigin(int dlrStatus, int origin, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = TextAlertSpecifications.TextAlertWithDLRStatusAndOrigin(dlrStatus, origin);

                ISpecification<TextAlert> spec = filter;

                var textAlerts = _textAlertRepository.AllMatching(spec, serviceHeader);

                if (textAlerts != null && textAlerts.Any())
                {
                    return textAlerts.ProjectedAsCollection<TextAlertDTO>();
                }
                else return null;
            }
        }

        public bool ProcessInboundMessage(InboundMessageDTO inboundMessageDTO, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        public bool AddTextAlertsWithHistory(GroupTextAlertDTO groupTextAlertDTO, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var textAlerts = new List<TextAlert>();

            var messageHistories = new List<CustomerMessageHistory>();

            var buffer = groupTextAlertDTO.Recipients.Trim().Split(new char[] { ',' }).Select(Guid.Parse);

            var customers = _messageGroupAppService.FindCustomersByMessageGroupIds(buffer.ToArray(), (int)MessageCategory.SMSAlert, serviceHeader);

            if (customers != null && customers.Any())
            {
                var currentBranch = _branchAppService.FindBranch(groupTextAlertDTO.BranchId, serviceHeader);

                foreach (var customer in customers)
                {
                    if (!string.IsNullOrEmpty(customer.AddressMobileLine) && Regex.IsMatch(customer.AddressMobileLine, @"^\+(?:[0-9]??){6,14}[0-9]$") && customer.AddressMobileLine.Length >= 13)
                    {
                        var textMessageBody = groupTextAlertDTO.TextMessageBody;

                        if (groupTextAlertDTO.AppendSignature && currentBranch != null && !string.IsNullOrWhiteSpace(currentBranch.Description) && !string.IsNullOrWhiteSpace(currentBranch.CompanyDescription))
                            textMessageBody = string.Format("{0}~{1}, {2}", textMessageBody, currentBranch.Description, currentBranch.CompanyDescription);

                        var textMessage = new TextMessage(customer.AddressMobileLine, textMessageBody, (int)DLRStatus.Pending, null, (int)MessageOrigin.Within, (int)QueuePriority.Normal, 0, false);

                        var textAlert = TextAlertFactory.CreateTextAlert(groupTextAlertDTO.CompanyId, textMessage, serviceHeader);

                        textAlerts.Add(textAlert);

                        var customerMessageHistory = CustomerMessageHistoryFactory.CreateCustomerMessageHistory(customer.Id, textMessage.Body, string.Empty, (int)MessageCategory.SMSAlert, textMessage.Recipient);

                        messageHistories.Add(customerMessageHistory);
                    }
                }
            }

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                if (textAlerts.Any())
                {
                    textAlerts.ForEach(item =>
                    {
                        _textAlertRepository.Add(item, serviceHeader);
                    });

                    result = dbContextScope.SaveChanges(serviceHeader) >= 0;

                    if (result)
                    {
                        if (textAlerts.Any())
                        {
                            var data = from t in textAlerts
                                       select new TextAlertDTO
                                       {
                                           Id = t.Id,
                                           TextMessageDLRStatus = (int)DLRStatus.Pending,
                                       };

                            _brokerService.ProcessTextAlerts(DMLCommand.Insert, serviceHeader, data.ToArray());
                        }
                    }
                }
            }

            return result;
        }

        public bool AddQuickTextAlert(QuickTextAlertDTO quickTextAlertDTO, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var textAlerts = new List<TextAlert>();

            var buffer = quickTextAlertDTO.Recipients.Trim().Split(new char[] { ',' });

            if (buffer != null && buffer.Any())
            {
                var currentBranch = _branchAppService.FindBranch(quickTextAlertDTO.BranchId, serviceHeader);

                Array.ForEach(buffer, recipient =>
                {
                    if (!string.IsNullOrEmpty(recipient) && Regex.IsMatch(recipient, @"^\+(?:[0-9]??){6,14}[0-9]$") && recipient.Length >= 13)
                    {
                        var textMessageBody = quickTextAlertDTO.TextMessageBody;

                        if (quickTextAlertDTO.AppendSignature && currentBranch != null && !string.IsNullOrWhiteSpace(currentBranch.Description) && !string.IsNullOrWhiteSpace(currentBranch.CompanyDescription))
                            textMessageBody = string.Format("{0}~{1}, {2}", textMessageBody, currentBranch.Description, currentBranch.CompanyDescription);

                        var textMessage = new TextMessage(recipient, textMessageBody, (int)DLRStatus.Pending, null, (int)MessageOrigin.Within, (int)QueuePriority.Highest, 0, false);

                        var textAlert = TextAlertFactory.CreateTextAlert(quickTextAlertDTO.CompanyId, textMessage, serviceHeader);

                        textAlerts.Add(textAlert);
                    }
                });
            }

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                if (textAlerts.Any())
                {
                    textAlerts.ForEach(item =>
                    {
                        _textAlertRepository.Add(item, serviceHeader);
                    });

                    result = dbContextScope.SaveChanges(serviceHeader) >= 0;

                    if (result)
                    {
                        if (textAlerts.Any())
                        {
                            var data = from t in textAlerts
                                       select new TextAlertDTO
                                       {
                                           Id = t.Id,
                                           TextMessageDLRStatus = (int)DLRStatus.Pending,
                                       };

                            _brokerService.ProcessTextAlerts(DMLCommand.Insert, serviceHeader, data.ToArray());
                        }
                    }
                }
            }

            return result;
        }

        public List<CommissionDTO> FindCommissions(int systemTransactionCode, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = TextAlertCommissionSpecifications.TextAlertCommission(systemTransactionCode);

                ISpecification<TextAlertCommission> spec = filter;

                var textAlertCommissions = _textAlertCommissionRepository.AllMatching(spec, serviceHeader);

                if (textAlertCommissions != null)
                {
                    var textAlertCommissionDTOs = textAlertCommissions.ProjectedAsCollection<TextAlertCommissionDTO>();

                    var projection = (from p in textAlertCommissionDTOs
                                      select new
                                      {
                                          p.ChargeBenefactor,
                                          p.Commission
                                      });

                    foreach (var item in projection)
                        item.Commission.ChargeBenefactor = item.ChargeBenefactor; // map benefactor

                    return (from p in projection select p.Commission).ToList();
                }
                else return null;
            }
        }

        public List<CommissionDTO> FindCachedCommissions(int systemTransactionCode, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<List<CommissionDTO>>(string.Format("TextAlertCommissionsBySystemTransactionCode_{0}_{1}", serviceHeader.ApplicationDomainName, systemTransactionCode), () =>
            {
                return FindCommissions(systemTransactionCode, serviceHeader);
            });
        }

        public bool UpdateCommissions(int systemTransactionCode, CommissionDTO[] commissionDTOs, int chargeBenefactor, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var existingCommissions = FindCommissions(systemTransactionCode, serviceHeader);

                if (existingCommissions != null && existingCommissions.Any())
                {
                    var oldSet = from c in existingCommissions ?? new List<CommissionDTO> { } select c.Id;

                    var newSet = from c in commissionDTOs ?? new CommissionDTO[] { } select c.Id;

                    var commonSet = oldSet.Intersect(newSet);

                    var insertSet = newSet.Except(commonSet);

                    var deleteSet = oldSet.Except(commonSet);

                    foreach (var commissionId in insertSet)
                    {
                        var textAlertCommission = TextAlertCommissionFactory.CreateTextAlertCommission(systemTransactionCode, commissionId, chargeBenefactor);

                        _textAlertCommissionRepository.Add(textAlertCommission, serviceHeader);
                    }

                    foreach (var commissionId in deleteSet)
                    {
                        var filter = TextAlertCommissionSpecifications.SystemTransactionCodeAndCommissionId(systemTransactionCode, commissionId);

                        ISpecification<TextAlertCommission> spec = filter;

                        var matches = _textAlertCommissionRepository.AllMatching(spec, serviceHeader);

                        if (matches != null && matches.Any())
                        {
                            foreach (var mapping in matches)
                            {
                                var persisted = _textAlertCommissionRepository.Get(mapping.Id, serviceHeader);

                                _textAlertCommissionRepository.Remove(persisted, serviceHeader);
                            }
                        }
                    }

                    foreach (var commissionId in commonSet)
                    {
                        var filter = TextAlertCommissionSpecifications.SystemTransactionCodeAndCommissionId(systemTransactionCode, commissionId);

                        ISpecification<TextAlertCommission> spec = filter;

                        var matches = _textAlertCommissionRepository.AllMatching(spec, serviceHeader);

                        if (matches != null && matches.Any())
                        {
                            foreach (var mapping in matches)
                            {
                                var persisted = _textAlertCommissionRepository.Get(mapping.Id, serviceHeader);

                                persisted.ChargeBenefactor = (byte)chargeBenefactor;
                            }
                        }
                    }
                }
                else
                {
                    foreach (var item in commissionDTOs)
                    {
                        var textAlertCommission = TextAlertCommissionFactory.CreateTextAlertCommission(systemTransactionCode, item.Id, chargeBenefactor);

                        textAlertCommission.CreatedBy = serviceHeader.ApplicationDomainName;

                        _textAlertCommissionRepository.Add(textAlertCommission, serviceHeader);
                    }
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }
    }
}
