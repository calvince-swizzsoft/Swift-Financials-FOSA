using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.MessagingModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAgg;
using Domain.MainBoundedContext.MessagingModule.Aggregates.MessageGroupAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.DivisionAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.StationAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.ZoneAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using KBCsv;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Profile;
using System.Web.Security;

namespace Application.MainBoundedContext.MessagingModule.Services
{
    public class MessageGroupAppService : IMessageGroupAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<MessageGroup> _messageGroupRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Station> _stationRepository;
        private readonly IRepository<Zone> _zoneRepository;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<Division> _divisionRepository;
        private readonly ISqlCommandAppService _sqlCommandAppService;

        public MessageGroupAppService(
            IDbContextScopeFactory dbContextScopeFactory,
            IRepository<MessageGroup> messageGroupRepository,
            IRepository<Customer> customerRepository,
            IRepository<Station> stationRepository,
            IRepository<Zone> zoneRepository,
            IRepository<Employee> employeeRepository,
            IRepository<Division> divisionRepository,
            ISqlCommandAppService sqlCommandAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (messageGroupRepository == null)
                throw new ArgumentNullException(nameof(messageGroupRepository));

            if (customerRepository == null)
                throw new ArgumentNullException(nameof(customerRepository));

            if (stationRepository == null)
                throw new ArgumentNullException(nameof(stationRepository));

            if (zoneRepository == null)
                throw new ArgumentNullException(nameof(zoneRepository));

            if (employeeRepository == null)
                throw new ArgumentNullException(nameof(employeeRepository));

            if (divisionRepository == null)
                throw new ArgumentNullException(nameof(divisionRepository));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _messageGroupRepository = messageGroupRepository;
            _customerRepository = customerRepository;
            _stationRepository = stationRepository;
            _zoneRepository = zoneRepository;
            _employeeRepository = employeeRepository;
            _divisionRepository = divisionRepository;
            _sqlCommandAppService = sqlCommandAppService;
        }

        public MessageGroupDTO AddNewMessageGroup(MessageGroupDTO messageGroupDTO, ServiceHeader serviceHeader)
        {
            if (messageGroupDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var messageGroup = MessageGroupFactory.CreateMessageGroup(messageGroupDTO.Description, messageGroupDTO.Target, messageGroupDTO.TargetValues);

                    messageGroup.CreatedBy = serviceHeader.ApplicationUserName;

                    _messageGroupRepository.Add(messageGroup, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return messageGroup.ProjectedAs<MessageGroupDTO>();
                }
            }
            else return null;
        }

        public bool UpdateMessageGroup(MessageGroupDTO messageGroupDTO, ServiceHeader serviceHeader)
        {
            if (messageGroupDTO == null || messageGroupDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _messageGroupRepository.Get(messageGroupDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = MessageGroupFactory.CreateMessageGroup(messageGroupDTO.Description, messageGroupDTO.Target, messageGroupDTO.TargetValues);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    current.CreatedBy = persisted.CreatedBy;


                    _messageGroupRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<MessageGroupDTO> FindMessageGroups(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var messageGroups = _messageGroupRepository.GetAll(serviceHeader);

                if (messageGroups != null && messageGroups.Any())
                {
                    return messageGroups.ProjectedAsCollection<MessageGroupDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<MessageGroupDTO> FindMessageGroups(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = MessageGroupSpecifications.DefaultSpec();

                ISpecification<MessageGroup> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var zonePagedCollection = _messageGroupRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (zonePagedCollection != null)
                {
                    var pageCollection = zonePagedCollection.PageCollection.ProjectedAsCollection<MessageGroupDTO>();

                    var itemsCount = zonePagedCollection.ItemsCount;

                    return new PageCollectionInfo<MessageGroupDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<MessageGroupDTO> FindMessageGroups(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = MessageGroupSpecifications.MessageGroupFullText(text);

                ISpecification<MessageGroup> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var messageGroupPagedCollection = _messageGroupRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (messageGroupPagedCollection != null)
                {
                    var pageCollection = messageGroupPagedCollection.PageCollection.ProjectedAsCollection<MessageGroupDTO>();

                    var itemsCount = messageGroupPagedCollection.ItemsCount;

                    return new PageCollectionInfo<MessageGroupDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public MessageGroupDTO FindMessageGroup(Guid messageGroupId, ServiceHeader serviceHeader)
        {
            if (messageGroupId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var messageGroup = _messageGroupRepository.Get(messageGroupId, serviceHeader);

                    if (messageGroup != null)
                    {
                        return messageGroup.ProjectedAs<MessageGroupDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<CustomerDTO> FindCustomersByMessageGroupIds(Guid[] messageGroupIds, int messageCategory, ServiceHeader serviceHeader)
        {
            var result = new List<CustomerDTO> { };

            var listOfLists = new List<IEnumerable<Guid>> { };

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                Array.ForEach(messageGroupIds, messageGroupId =>
                {
                    var messageGroup = _messageGroupRepository.Get(messageGroupId, serviceHeader);

                    if (messageGroup != null && !string.IsNullOrWhiteSpace(messageGroup.TargetValues))
                    {
                        switch ((MessageGroupTarget)messageGroup.Target)
                        {
                            case MessageGroupTarget.Zone:
                                {
                                    var zoneIds = messageGroup.TargetValues.Split(new[] { ',' }).Select(Guid.Parse);

                                    var zoneMembers = new List<Guid>();

                                    foreach (var zoneId in zoneIds)
                                    {
                                        var stations = FindStationsByZoneId(zoneId, serviceHeader);

                                        if (stations != null && stations.Any())
                                        {
                                            foreach (var station in stations)
                                            {
                                                var members = FindCustomersByStationId(station.Id, serviceHeader);

                                                if (members != null && members.Any())
                                                {
                                                    switch ((MessageCategory)messageCategory)
                                                    {
                                                        case MessageCategory.SMSAlert:
                                                            zoneMembers.AddRange(members.Where(x => Regex.IsMatch(x.AddressMobileLine ?? string.Empty, @"^\+(?:[0-9]??){6,14}[0-9]$")).Select(x => x.Id));
                                                            break;
                                                        case MessageCategory.EmailAlert:
                                                            zoneMembers.AddRange(members.Where(x => Regex.IsMatch(x.AddressEmail ?? string.Empty, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")).Select(x => x.Id));
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    listOfLists.Add(zoneMembers);
                                }
                                break;
                            case MessageGroupTarget.Station:
                                {
                                    var stationMembers = new List<Guid>();

                                    var stationIds = messageGroup.TargetValues.Split(new[] { ',' }).Select(Guid.Parse);

                                    foreach (var stationId in stationIds)
                                    {
                                        var members = FindCustomersByStationId(stationId, serviceHeader);

                                        if (members != null && members.Any())
                                        {
                                            switch ((MessageCategory)messageCategory)
                                            {
                                                case MessageCategory.SMSAlert:
                                                    stationMembers.AddRange(members.Where(x => Regex.IsMatch(x.AddressMobileLine ?? string.Empty, @"^\+(?:[0-9]??){6,14}[0-9]$")).Select(x => x.Id));
                                                    break;
                                                case MessageCategory.EmailAlert:
                                                    stationMembers.AddRange(members.Where(x => Regex.IsMatch(x.AddressEmail ?? string.Empty, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")).Select(x => x.Id));
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }

                                    listOfLists.Add(stationMembers);
                                }
                                break;
                            case MessageGroupTarget.Role:
                                {
                                    ProfileManager.ApplicationName = string.Format("/{0}", serviceHeader.ApplicationDomainName);

                                    var roles = messageGroup.TargetValues.Split(new[] { ',' }).Select(x => x);

                                    var roleMembers = new List<Guid>();

                                    foreach (var role in roles)
                                    {
                                        var users = Roles.GetUsersInRole(role);

                                        foreach (var user in users)
                                        {
                                            var profile = ProfileBase.Create(user);

                                            var memberId = profile["EmployeeId"];

                                            var employee = _employeeRepository.Get(Guid.Parse(memberId.ToString()), serviceHeader);

                                            switch ((MessageCategory)messageCategory)
                                            {
                                                case MessageCategory.SMSAlert:
                                                    if (Regex.IsMatch(employee.Customer.Address.MobileLine ?? string.Empty, @"^\+(?:[0-9]??){6,14}[0-9]$"))
                                                        roleMembers.Add(employee.CustomerId);
                                                    break;
                                                case MessageCategory.EmailAlert:
                                                    if (Regex.IsMatch(employee.Customer.Address.Email ?? string.Empty, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                                                        roleMembers.Add(employee.CustomerId);
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }

                                    listOfLists.Add(roleMembers);
                                }
                                break;
                            case MessageGroupTarget.Members:
                                {
                                    var members = FindCustomers(serviceHeader);

                                    if (members != null && members.Any())
                                    {
                                        switch ((MessageCategory)messageCategory)
                                        {
                                            case MessageCategory.SMSAlert:
                                                listOfLists.Add(members.Where(x => Regex.IsMatch(x.AddressMobileLine ?? string.Empty, @"^\+(?:[0-9]??){6,14}[0-9]$")).Select(x => x.Id));
                                                break;
                                            case MessageCategory.EmailAlert:
                                                listOfLists.Add(members.Where(x => Regex.IsMatch(x.AddressEmail ?? string.Empty, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")).Select(x => x.Id));
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }
                                break;
                            case MessageGroupTarget.Employer:
                                {
                                    var employerIds = messageGroup.TargetValues.Split(new[] { ',' }).Select(Guid.Parse);

                                    var employerMembers = new List<Guid>();

                                    foreach (var employerId in employerIds)
                                    {
                                        var divisions = FindDivisionsByEmployerId(employerId, serviceHeader);

                                        if (divisions != null && divisions.Any())
                                        {
                                            foreach (var division in divisions)
                                            {
                                                var zones = FindZonesByDivisionId(division.Id, serviceHeader);

                                                if (zones != null && zones.Any())
                                                {
                                                    foreach (var zone in zones)
                                                    {
                                                        var stations = FindStationsByZoneId(zone.Id, serviceHeader);

                                                        if (stations != null && stations.Any())
                                                        {
                                                            foreach (var station in stations)
                                                            {
                                                                var members = FindCustomersByStationId(station.Id, serviceHeader);

                                                                if (members != null && members.Any())
                                                                {
                                                                    switch ((MessageCategory)messageCategory)
                                                                    {
                                                                        case MessageCategory.SMSAlert:
                                                                            employerMembers.AddRange(members.Where(x => Regex.IsMatch(x.AddressMobileLine ?? string.Empty, @"^\+(?:[0-9]??){6,14}[0-9]$")).Select(x => x.Id));
                                                                            break;
                                                                        case MessageCategory.EmailAlert:
                                                                            employerMembers.AddRange(members.Where(x => Regex.IsMatch(x.AddressEmail ?? string.Empty, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")).Select(x => x.Id));
                                                                            break;
                                                                        default:
                                                                            break;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    listOfLists.Add(employerMembers);
                                }
                                break;
                            case MessageGroupTarget.Custom:
                                {
                                    var customerIds = messageGroup.TargetValues.Split(new[] { ',' }).Select(Guid.Parse);

                                    var customMembers = new List<Guid>();

                                    foreach (var customerId in customerIds)
                                    {
                                        var filter = CustomerSpecifications.CustomerId(customerId);

                                        ISpecification<Customer> spec = filter;

                                        var members = _customerRepository.AllMatching(spec, serviceHeader);

                                        if (members != null && members.Any())
                                        {
                                            switch ((MessageCategory)messageCategory)
                                            {
                                                case MessageCategory.SMSAlert:
                                                    customMembers.AddRange(members.Where(x => Regex.IsMatch(x.Address.MobileLine ?? string.Empty, @"^\+(?:[0-9]??){6,14}[0-9]$")).Select(x => x.Id));
                                                    break;
                                                case MessageCategory.EmailAlert:
                                                    customMembers.AddRange(members.Where(x => Regex.IsMatch(x.Address.Email ?? string.Empty, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")).Select(x => x.Id));
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }

                                    listOfLists.Add(customMembers);
                                }
                                break;
                            case MessageGroupTarget.Division:
                                {
                                    var divisionIds = messageGroup.TargetValues.Split(new[] { ',' }).Select(Guid.Parse);

                                    var divisionMembers = new List<Guid>();

                                    foreach (var divisionId in divisionIds)
                                    {
                                        var zones = FindZonesByDivisionId(divisionId, serviceHeader);

                                        if (zones != null && zones.Any())
                                        {
                                            foreach (var zone in zones)
                                            {
                                                var stations = FindStationsByZoneId(zone.Id, serviceHeader);

                                                if (stations != null && stations.Any())
                                                {
                                                    foreach (var station in stations)
                                                    {
                                                        var members = FindCustomersByStationId(station.Id, serviceHeader);

                                                        if (members != null && members.Any())
                                                        {
                                                            switch ((MessageCategory)messageCategory)
                                                            {
                                                                case MessageCategory.SMSAlert:
                                                                    divisionMembers.AddRange(members.Where(x => Regex.IsMatch(x.AddressMobileLine ?? string.Empty, @"^\+(?:[0-9]??){6,14}[0-9]$")).Select(x => x.Id));
                                                                    break;
                                                                case MessageCategory.EmailAlert:
                                                                    divisionMembers.AddRange(members.Where(x => Regex.IsMatch(x.AddressEmail ?? string.Empty, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")).Select(x => x.Id));
                                                                    break;
                                                                default:
                                                                    break;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    listOfLists.Add(divisionMembers);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                });

                if (listOfLists.Any())
                {
                    var intersection = listOfLists
                                .Skip(1)
                                .Aggregate(new HashSet<Guid>(listOfLists.First()), (h, e) =>
                                {
                                    h.IntersectWith(e);

                                    return h;
                                });

                    if (intersection != null && intersection.Any())
                    {
                        foreach (var item in intersection)
                        {
                            var targetCustomer = FindCustomer(item, serviceHeader);

                            if (targetCustomer != null)
                                result.Add(targetCustomer);
                        }
                    }
                }
            }

            return result;
        }

        public BatchImportParseInfo ParseCustomersCustomMessageGroupImport(string fileUploadDirectory, string fileName, ServiceHeader serviceHeader)
        {
            BatchImportParseInfo parseInfo = null;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                if (!string.IsNullOrWhiteSpace(fileUploadDirectory) && !string.IsNullOrWhiteSpace(fileName))
                {
                    var path = Path.Combine(fileUploadDirectory, fileName);

                    if (File.Exists(path))
                    {
                        var importEntries = new List<BatchImportEntryWrapper> { };

                        using (var streamReader = new StreamReader(path))
                        using (var reader = new CsvReader(streamReader))
                        {
                            // the CSV file has a header record, so we read that first
                            reader.ReadHeaderRecord();

                            while (reader.HasMoreRecords)
                            {
                                var dataRecord = reader.ReadDataRecord();

                                if (dataRecord.Count == 3)
                                {
                                    var payoutEntry = new BatchImportEntryWrapper
                                    {
                                        Column1 = dataRecord[0], //Payroll Number
                                        Column2 = dataRecord[1], //Customer Name
                                        Column3 = dataRecord[2], //OtherReference
                                    };

                                    importEntries.Add(payoutEntry);
                                }
                            }
                        }
                        if (importEntries.Any())
                        {
                            var result = new BatchImportParseInfo
                            {
                                MatchedCollection4 = new List<CustomerDTO> { },
                                MismatchedCollection = new List<BatchImportEntryWrapper> { }
                            };

                            var count = 0;

                            importEntries.ForEach(item =>
                            {
                                var customers = _sqlCommandAppService.FindCustomersByPayrollNumber(item.Column1, serviceHeader);

                                if (customers.Any())
                                {
                                    if (customers.Count == 1)
                                    {
                                        var targetCustomer = customers[0];

                                        if (!string.IsNullOrWhiteSpace(targetCustomer.AddressMobileLine) && Regex.IsMatch(targetCustomer.AddressMobileLine, @"^\+(?:[0-9]??){6,14}[0-9]$") && targetCustomer.AddressMobileLine.Length >= 13)
                                        {
                                            result.MatchedCollection4.Add(targetCustomer);
                                        }
                                        else
                                        {
                                            item.Remarks = string.Format("Record #{0} ~ invalid mobile line '{1}'.", count, targetCustomer.AddressMobileLine);

                                            result.MismatchedCollection.Add(item);
                                        }
                                    }
                                    else
                                    {
                                        item.Remarks = string.Format("Record #{0} ~ found {1} customer matches.", count, customers.Count());

                                        result.MismatchedCollection.Add(item);
                                    }
                                }
                                else
                                {
                                    item.Remarks = string.Format("Record #{0} ~ no match for customer.", count);

                                    result.MismatchedCollection.Add(item);
                                }

                                // tally
                                count += 1;
                            });

                            parseInfo = result;
                        }
                    }
                    return parseInfo;
                }
                return parseInfo;
            }
        }

        public BatchImportParseInfo ParseQuickAlertImport(string fileUploadDirectory, string fileName, int messageCategory, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                if (!string.IsNullOrWhiteSpace(fileUploadDirectory) && !string.IsNullOrWhiteSpace(fileName))
                {
                    var path = Path.Combine(fileUploadDirectory, fileName);

                    if (System.IO.File.Exists(path))
                    {
                        var importEntries = new List<BatchImportEntryWrapper> { };

                        using (var streamReader = new StreamReader(path))
                        using (var reader = new CsvReader(streamReader))
                        {
                            // the CSV file has a header record, so we read that first
                            reader.ReadHeaderRecord();

                            while (reader.HasMoreRecords)
                            {
                                var dataRecord = reader.ReadDataRecord();

                                if (dataRecord.Count == 3)
                                {
                                    var payoutEntry = new BatchImportEntryWrapper
                                    {
                                        Column1 = dataRecord[0], //Payroll Number
                                        Column2 = dataRecord[1], //Customer Name
                                        Column3 = dataRecord[2], //OtherReference
                                    };

                                    importEntries.Add(payoutEntry);
                                }
                            }
                        }

                        if (importEntries.Any())
                        {
                            BatchImportParseInfo parseInfo = null;

                            switch ((MessageCategory)messageCategory)
                            {
                                case MessageCategory.SMSAlert:
                                    parseInfo = ParseQuickTextAlert(importEntries, serviceHeader);
                                    break;
                                case MessageCategory.EmailAlert:
                                    parseInfo = ParseQuickEmailAlert(importEntries, serviceHeader);
                                    break;
                                default:
                                    break;
                            }

                            return parseInfo;
                        }
                        else return null;
                    }
                    else return null;
                }
                else return null;
            }
        }

        private BatchImportParseInfo ParseQuickTextAlert(List<BatchImportEntryWrapper> importEntries, ServiceHeader serviceHeader)
        {
            var result = new BatchImportParseInfo
            {
                MatchedCollection4 = new List<CustomerDTO> { },
                MismatchedCollection = new List<BatchImportEntryWrapper> { }
            };

            var count = 0;

            importEntries.ForEach(item =>
            {
                var customers = _sqlCommandAppService.FindCustomersByPayrollNumber(item.Column1, serviceHeader);

                if (customers.Any())
                {
                    if (customers.Count == 1)
                    {
                        var targetCustomer = customers[0];

                        if (!string.IsNullOrWhiteSpace(targetCustomer.AddressMobileLine) && Regex.IsMatch(targetCustomer.AddressMobileLine, @"^\+(?:[0-9]??){6,14}[0-9]$") && targetCustomer.AddressMobileLine.Length >= 13)
                        {
                            result.MatchedCollection4.Add(targetCustomer);
                        }
                        else
                        {
                            item.Remarks = string.Format("Record #{0} ~ invalid mobile line '{1}'.", count, targetCustomer.AddressMobileLine);

                            result.MismatchedCollection.Add(item);
                        }
                    }
                    else
                    {
                        item.Remarks = string.Format("Record #{0} ~ found {1} customer matches.", count, customers.Count());

                        result.MismatchedCollection.Add(item);
                    }
                }
                else
                {
                    item.Remarks = string.Format("Record #{0} ~ no match for customer.", count);

                    result.MismatchedCollection.Add(item);
                }

                // tally
                count += 1;
            });

            return result;
        }

        private BatchImportParseInfo ParseQuickEmailAlert(List<BatchImportEntryWrapper> importEntries, ServiceHeader serviceHeader)
        {
            var result = new BatchImportParseInfo
            {
                MatchedCollection4 = new List<CustomerDTO> { },
                MismatchedCollection = new List<BatchImportEntryWrapper> { }
            };

            var count = 0;

            importEntries.ForEach(item =>
            {
                var customers = _sqlCommandAppService.FindCustomersByPayrollNumber(item.Column1, serviceHeader);

                if (customers.Any())
                {
                    if (customers.Count == 1)
                    {
                        var targetCustomer = customers[0];

                        if (!string.IsNullOrWhiteSpace(targetCustomer.AddressEmail) && Regex.IsMatch(targetCustomer.AddressEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                        {
                            result.MatchedCollection4.Add(targetCustomer);
                        }
                        else
                        {
                            item.Remarks = string.Format("Record #{0} ~ invalid e-mail '{1}'.", count, targetCustomer.AddressEmail);

                            result.MismatchedCollection.Add(item);
                        }
                    }
                    else
                    {
                        item.Remarks = string.Format("Record #{0} ~ found {1} customer matches.", count, customers.Count());

                        result.MismatchedCollection.Add(item);
                    }
                }
                else
                {
                    item.Remarks = string.Format("Record #{0} ~ no match for customer.", count);

                    result.MismatchedCollection.Add(item);
                }

                // tally
                count += 1;
            });

            return result;
        }

        private List<StationDTO> FindStationsByZoneId(Guid zoneId, ServiceHeader serviceHeader)
        {
            if (zoneId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = StationSpecifications.StationWithZoneId(zoneId);

                    ISpecification<Station> spec = filter;

                    var stations = _stationRepository.AllMatching(spec, serviceHeader);

                    if (stations != null)
                    {
                        return stations.ProjectedAsCollection<StationDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        private CustomerDTO FindCustomer(Guid customerId, ServiceHeader serviceHeader)
        {
            if (customerId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var customer = _customerRepository.Get(customerId, serviceHeader);

                    if (customer != null)
                    {
                        return customer.ProjectedAs<CustomerDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        private List<CustomerDTO> FindCustomersByStationId(Guid stationId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CustomerSpecifications.CustomerWithStationId(stationId, string.Empty, (int)CustomerFilter.Reference1);

                ISpecification<Customer> spec = filter;

                return _customerRepository.AllMatching<CustomerDTO>(spec, serviceHeader);
            }
        }

        private List<CustomerDTO> FindCustomers(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CustomerSpecifications.DefaultSpec();

                ISpecification<Customer> spec = filter;

                return _customerRepository.AllMatching<CustomerDTO>(spec, serviceHeader);
            }
        }

        private List<DivisionDTO> FindDivisionsByEmployerId(Guid employerId, ServiceHeader serviceHeader)
        {
            if (employerId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = DivisionSpecifications.DivisionWithEmployerId(employerId);

                    ISpecification<Division> spec = filter;

                    var divisions = _divisionRepository.AllMatching(spec, serviceHeader);

                    if (divisions != null)
                    {
                        return divisions.ProjectedAsCollection<DivisionDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        private List<ZoneDTO> FindZonesByDivisionId(Guid divisionId, ServiceHeader serviceHeader)
        {
            if (divisionId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = ZoneSpecifications.ZoneWithDivisionId(divisionId);

                    ISpecification<Zone> spec = filter;

                    var zones = _zoneRepository.AllMatching(spec, serviceHeader);

                    if (zones != null)
                    {
                        return zones.ProjectedAsCollection<ZoneDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }
    }
}
