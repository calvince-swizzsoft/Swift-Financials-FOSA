using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.MicroCreditModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.MicroCreditModule.Aggregates.MicroCreditGroupAgg;
using Domain.MainBoundedContext.MicroCreditModule.Aggregates.MicroCreditGroupMemberAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using KBCsv;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Application.MainBoundedContext.MicroCreditModule.Services
{
    public class MicroCreditGroupAppService : IMicroCreditGroupAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<MicroCreditGroup> _microCreditGroupRepository;
        private readonly IRepository<MicroCreditGroupMember> _microCreditGroupMemberRepository;
        private readonly ILoanProductAppService _loanProductAppService;
        private readonly IInvestmentProductAppService _investmentProductAppService;
        private readonly ICustomerAccountAppService _customerAccountAppService;
        private readonly ISqlCommandAppService _sqlCommandAppService;

        public MicroCreditGroupAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<MicroCreditGroup> microCreditGroupRepository,
           IRepository<MicroCreditGroupMember> microCreditGroupMemberRepository,
           ILoanProductAppService loanProductAppService,
           IInvestmentProductAppService investmentProductAppService,
           ICustomerAccountAppService customerAccountAppService,
           ISqlCommandAppService sqlCommandAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (microCreditGroupRepository == null)
                throw new ArgumentNullException(nameof(microCreditGroupRepository));

            if (microCreditGroupMemberRepository == null)
                throw new ArgumentNullException(nameof(microCreditGroupMemberRepository));

            if (loanProductAppService == null)
                throw new ArgumentNullException(nameof(loanProductAppService));

            if (investmentProductAppService == null)
                throw new ArgumentNullException(nameof(investmentProductAppService));

            if (customerAccountAppService == null)
                throw new ArgumentNullException(nameof(customerAccountAppService));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _microCreditGroupRepository = microCreditGroupRepository;
            _microCreditGroupMemberRepository = microCreditGroupMemberRepository;
            _loanProductAppService = loanProductAppService;
            _investmentProductAppService = investmentProductAppService;
            _customerAccountAppService = customerAccountAppService;
            _sqlCommandAppService = sqlCommandAppService;
        }

        public MicroCreditGroupDTO AddNewMicroCreditGroup(MicroCreditGroupDTO microCreditGroupDTO, ServiceHeader serviceHeader)
        {
            if (microCreditGroupDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    // get the specification
                    var filter = MicroCreditGroupSpecifications.MicroCreditGroupWithCustomerId(microCreditGroupDTO.CustomerId);

                    ISpecification<MicroCreditGroup> spec = filter;

                    //Query this criteria
                    var microCreditGroups = _microCreditGroupRepository.AllMatching(spec, serviceHeader);

                    if (microCreditGroups != null && microCreditGroups.Any())
                    {
                        //microCreditGroupDTO.errorMessages = string.Format(("Sorry, but the selected customer already exists as a microcredit group!"));
                        return microCreditGroupDTO;

                    }

                    else
                    {
                        var microCreditGroup = MicroCreditGroupFactory.CreateMicroCreditGroup(microCreditGroupDTO.ParentId, microCreditGroupDTO.CustomerId, microCreditGroupDTO.MicroCreditOfficerId, microCreditGroupDTO.Type, microCreditGroupDTO.Purpose, microCreditGroupDTO.Activities, microCreditGroupDTO.MeetingFrequency, microCreditGroupDTO.MeetingDayOfWeek, microCreditGroupDTO.MeetingPlace, microCreditGroupDTO.MinimumMembers, microCreditGroupDTO.MaximumMembers, microCreditGroupDTO.Remarks);

                        microCreditGroup.CreatedBy = serviceHeader.ApplicationUserName;

                        _microCreditGroupRepository.Add(microCreditGroup, serviceHeader);

                        dbContextScope.SaveChanges(serviceHeader);

                        return microCreditGroup.ProjectedAs<MicroCreditGroupDTO>();
                    }
                }
            }
            else return null;
        }

        public bool UpdateMicroCreditGroup(MicroCreditGroupDTO microCreditGroupDTO, ServiceHeader serviceHeader)
        {
            if (microCreditGroupDTO == null || microCreditGroupDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _microCreditGroupRepository.Get(microCreditGroupDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = MicroCreditGroupFactory.CreateMicroCreditGroup(microCreditGroupDTO.ParentId, microCreditGroupDTO.CustomerId, microCreditGroupDTO.MicroCreditOfficerId, microCreditGroupDTO.Type, microCreditGroupDTO.Purpose, microCreditGroupDTO.Activities, microCreditGroupDTO.MeetingFrequency, microCreditGroupDTO.MeetingDayOfWeek, microCreditGroupDTO.MeetingPlace, microCreditGroupDTO.MinimumMembers, microCreditGroupDTO.MaximumMembers, microCreditGroupDTO.Remarks);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    current.CustomerId = persisted.CustomerId;
                    current.CreatedBy = persisted.CreatedBy;
                    

                    _microCreditGroupRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public MicroCreditGroupMemberDTO AddNewMicroCreditGroupMember(MicroCreditGroupMemberDTO microCreditGroupMemberDTO, ServiceHeader serviceHeader)
        {
            if (microCreditGroupMemberDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var officials = default(int);

                    switch ((MicroCreditGroupMemberDesignation)microCreditGroupMemberDTO.Designation)
                    {
                        case MicroCreditGroupMemberDesignation.Chairperson:
                        case MicroCreditGroupMemberDesignation.DeputyChairperson:
                        case MicroCreditGroupMemberDesignation.Secretary:
                        case MicroCreditGroupMemberDesignation.DeputySecretary:
                        case MicroCreditGroupMemberDesignation.Treasurer:

                            officials = _microCreditGroupMemberRepository.AllMatchingCount(MicroCreditGroupMemberSpecifications.MicroCreditGroupMemberWithMicroCreditGroupIdAndDesignation(microCreditGroupMemberDTO.MicroCreditGroupId, microCreditGroupMemberDTO.Designation), serviceHeader);

                            break;
                        case MicroCreditGroupMemberDesignation.OrdinaryMember:
                            break;
                        default:
                            break;
                    }

                    if (officials > 0)
                    {
                        microCreditGroupMemberDTO.errorMsg = string.Format("Sorry, but the selected microcredit group already has a member designated as {0}!", EnumHelper.GetDescription((MicroCreditGroupMemberDesignation)microCreditGroupMemberDTO.Designation));
                        return microCreditGroupMemberDTO;
                    }

                    var similarMembers = _microCreditGroupMemberRepository.AllMatchingCount(MicroCreditGroupMemberSpecifications.MicroCreditGroupMemberWithCustomerId(microCreditGroupMemberDTO.CustomerId), serviceHeader);

                    if (similarMembers > 0)
                    {
                        microCreditGroupMemberDTO.errorMsg = string.Format("Sorry, but the selected customer is already linked to a microcredit group!");
                        return microCreditGroupMemberDTO;
                    }

                    var microCreditGroupMember = MicroCreditGroupMemberFactory.CreateMicroCreditGroupMember(microCreditGroupMemberDTO.MicroCreditGroupId, microCreditGroupMemberDTO.CustomerId, microCreditGroupMemberDTO.Designation, microCreditGroupMemberDTO.LoanCycle, microCreditGroupMemberDTO.Remarks);

                    microCreditGroupMember.CreatedBy = serviceHeader.ApplicationUserName;

                    _microCreditGroupMemberRepository.Add(microCreditGroupMember, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return microCreditGroupMember.ProjectedAs<MicroCreditGroupMemberDTO>();
                }
            }
            else return null;
        }

        public bool UpdateMicroCreditGroupMember(MicroCreditGroupMemberDTO microCreditGroupMemberDTO, ServiceHeader serviceHeader)
        {
            if (microCreditGroupMemberDTO == null || microCreditGroupMemberDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _microCreditGroupMemberRepository.Get(microCreditGroupMemberDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    persisted.LoanCycle = (short)microCreditGroupMemberDTO.LoanCycle;
                    persisted.Remarks = microCreditGroupMemberDTO.Remarks;

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public bool RemoveMicroCreditGroupMembers(List<MicroCreditGroupMemberDTO> microCreditGroupMemberDTOs, ServiceHeader serviceHeader)
        {
            if (microCreditGroupMemberDTOs == null)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                foreach (var item in microCreditGroupMemberDTOs)
                {
                    var persisted = _microCreditGroupMemberRepository.Get(item.Id, serviceHeader);

                    if (persisted != null)
                    {
                        _microCreditGroupMemberRepository.Remove(persisted, serviceHeader);
                    }
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public bool MicroCreditGroupMemberExists(Guid microCreditGroupMemberCustomerId, Guid microCreditGroupCustomerId, ServiceHeader serviceHeader)
        {
            if (microCreditGroupMemberCustomerId != null && microCreditGroupMemberCustomerId != Guid.Empty && microCreditGroupCustomerId != null && microCreditGroupCustomerId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var matches = _microCreditGroupMemberRepository.AllMatchingCount(MicroCreditGroupMemberSpecifications.MicroCreditGroupMemberWithCustomerIdAndMicroCreditGroupWithCustomerId(microCreditGroupMemberCustomerId, microCreditGroupCustomerId), serviceHeader);

                    return matches != 0;
                }
            }
            else return false;
        }

        public bool UpdateMicroCreditGroupMemberCollection(Guid microCreditGroupId, List<MicroCreditGroupMemberDTO> microCreditGroupMemberCollection, ServiceHeader serviceHeader)
        {
            if (microCreditGroupId != null && microCreditGroupMemberCollection != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _microCreditGroupRepository.Get(microCreditGroupId, serviceHeader);

                    if (persisted != null)
                    {
                        var existing = FindMicroCreditGroupMembers(persisted.Id, serviceHeader);

                        if (existing != null && existing.Any())
                        {
                            foreach (var item in existing)
                            {
                                var microCreditGroupMember = _microCreditGroupMemberRepository.Get(item.Id, serviceHeader);

                                if (microCreditGroupMember != null)
                                {
                                    _microCreditGroupMemberRepository.Remove(microCreditGroupMember, serviceHeader);
                                }
                            }
                        }

                        if (microCreditGroupMemberCollection.Any())
                        {
                            foreach (var item in microCreditGroupMemberCollection)
                            {
                                var microCreditGroupMember = MicroCreditGroupMemberFactory.CreateMicroCreditGroupMember(persisted.Id, item.CustomerId, item.Designation, item.LoanCycle, item.Remarks);

                                microCreditGroupMember.CreatedBy = serviceHeader.ApplicationUserName;

                                _microCreditGroupMemberRepository.Add(microCreditGroupMember, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public List<MicroCreditGroupDTO> FindMicroCreditGroups(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var microCreditGroups = _microCreditGroupRepository.GetAll(serviceHeader);

                if (microCreditGroups != null && microCreditGroups.Any())
                {
                    return microCreditGroups.ProjectedAsCollection<MicroCreditGroupDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<MicroCreditGroupDTO> FindMicroCreditGroups(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = MicroCreditGroupSpecifications.MicroCreditGroupFullText(text);

                ISpecification<MicroCreditGroup> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var microCreditGroupPagedCollection = _microCreditGroupRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (microCreditGroupPagedCollection != null)
                {
                    var pageCollection = microCreditGroupPagedCollection.PageCollection.ProjectedAsCollection<MicroCreditGroupDTO>();

                    var itemsCount = microCreditGroupPagedCollection.ItemsCount;

                    return new PageCollectionInfo<MicroCreditGroupDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public MicroCreditGroupDTO FindMicroCreditGroup(Guid microCreditGroupId, ServiceHeader serviceHeader)
        {
            if (microCreditGroupId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var microCreditGroup = _microCreditGroupRepository.Get(microCreditGroupId, serviceHeader);

                    if (microCreditGroup != null)
                    {
                        return microCreditGroup.ProjectedAs<MicroCreditGroupDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<MicroCreditGroupMemberDTO> FindMicroCreditGroupMembers(Guid microCreditGroupId, ServiceHeader serviceHeader)
        {
            if (microCreditGroupId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = MicroCreditGroupMemberSpecifications.MicroCreditGroupMemberWithMicroCreditGroupId(microCreditGroupId);

                    ISpecification<MicroCreditGroupMember> spec = filter;

                    var microCreditGroupMembers = _microCreditGroupMemberRepository.AllMatching(spec, serviceHeader);

                    if (microCreditGroupMembers != null)
                    {
                        return microCreditGroupMembers.ProjectedAsCollection<MicroCreditGroupMemberDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<MicroCreditGroupMemberDTO> FindMicroCreditGroupMembersByMicroCreditGroupCustomerId(Guid microCreditGroupCustomerId, ServiceHeader serviceHeader)
        {
            if (microCreditGroupCustomerId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = MicroCreditGroupMemberSpecifications.MicroCreditGroupMemberWithMicroCreditGroupCustomerId(microCreditGroupCustomerId);

                    ISpecification<MicroCreditGroupMember> spec = filter;

                    var microCreditGroupMembers = _microCreditGroupMemberRepository.AllMatching(spec, serviceHeader);

                    if (microCreditGroupMembers != null)
                    {
                        return microCreditGroupMembers.ProjectedAsCollection<MicroCreditGroupMemberDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public MicroCreditGroupMemberDTO FindMicroCreditGroupMemberByCustomerId(Guid customerId, ServiceHeader serviceHeader)
        {
            if (customerId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = MicroCreditGroupMemberSpecifications.MicroCreditGroupMemberWithCustomerId(customerId);

                    ISpecification<MicroCreditGroupMember> spec = filter;

                    var microCreditGroupMembers = _microCreditGroupMemberRepository.AllMatching(spec, serviceHeader);

                    if (microCreditGroupMembers != null && microCreditGroupMembers.Count() == 1)
                    {
                        return microCreditGroupMembers.ProjectedAsCollection<MicroCreditGroupMemberDTO>()[0];
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<MicroCreditGroupMemberDTO> FindMicroCreditGroupMembers(Guid microCreditGroupId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (microCreditGroupId != null && microCreditGroupId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = MicroCreditGroupMemberSpecifications.MicroCreditGroupMemberFullText(microCreditGroupId, text);

                    ISpecification<MicroCreditGroupMember> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var microCreditGroupMemberPagedCollection = _microCreditGroupMemberRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (microCreditGroupMemberPagedCollection != null)
                    {
                        var pageCollection = microCreditGroupMemberPagedCollection.PageCollection.ProjectedAsCollection<MicroCreditGroupMemberDTO>();

                        var itemsCount = microCreditGroupMemberPagedCollection.ItemsCount;

                        return new PageCollectionInfo<MicroCreditGroupMemberDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                    }
                    else return null;
                }
            }
            else return null;
        }

        public BatchImportParseInfo ParseMicroCreditGroupImport(Guid microCreditGroupCustomerId, string fileUploadDirectory, string fileName, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var tally = _microCreditGroupRepository.AllMatchingCount(MicroCreditGroupSpecifications.MicroCreditGroupWithCustomerId(microCreditGroupCustomerId), serviceHeader);

                if (tally != 0 && !string.IsNullOrWhiteSpace(fileUploadDirectory) && !string.IsNullOrWhiteSpace(fileName))
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

                                if (dataRecord.Count == 6)
                                {
                                    var apportionmentEntry = new BatchImportEntryWrapper
                                    {
                                        Column1 = dataRecord[0], //Personal File Number
                                        Column2 = dataRecord[1], //Primary Description
                                        Column3 = dataRecord[2], //Contribution
                                        Column4 = dataRecord[3], //Loan/Investment Product Credit Code
                                        Column5 = dataRecord[4], //Type (sInterest,sLoan,sShare,wCont)
                                        Column6 = dataRecord[5], //OtherReference
                                    };

                                    importEntries.Add(apportionmentEntry);
                                }
                            }
                        }

                        if (importEntries.Any())
                        {
                            return DoParse(microCreditGroupCustomerId, importEntries, serviceHeader);
                        }
                        else return null;
                    }
                    else return null;
                }
                else return null;
            }
        }

        private BatchImportParseInfo DoParse(Guid microCreditGroupMemberCustomerId, List<BatchImportEntryWrapper> importEntries, ServiceHeader serviceHeader)
        {
            var result = new BatchImportParseInfo
            {
                MatchedCollection3 = new List<ApportionmentWrapper> { },
                MismatchedCollection = new List<BatchImportEntryWrapper> { }
            };

            var loanProducts = _loanProductAppService.FindLoanProducts(serviceHeader);

            var investmentProducts = _investmentProductAppService.FindInvestmentProducts(serviceHeader);

            var count = 0;

            importEntries.ForEach(item =>
            {
                var contributionAmount = default(decimal);

                if (decimal.TryParse(item.Column3, NumberStyles.Any, CultureInfo.InvariantCulture, out contributionAmount))
                {
                    var productCode = default(int);

                    if (int.TryParse(item.Column4, out productCode))
                    {
                        switch ((CheckOffEntryType)Enum.Parse(typeof(CheckOffEntryType), item.Column5))
                        {
                            case CheckOffEntryType.sLoan:

                                #region sLoan

                                var sLoan_MatchedLoanProducts = loanProducts.Where(x => x.Code == productCode);

                                if (sLoan_MatchedLoanProducts != null && sLoan_MatchedLoanProducts.Any() && sLoan_MatchedLoanProducts.Count() == 1)
                                {
                                    var targetLoanPrincipalProduct = sLoan_MatchedLoanProducts.First();

                                    var customerLoanPrincipalAccounts = _sqlCommandAppService.FindCustomerAccountsByTargetProductIdAndReference3(targetLoanPrincipalProduct.Id, item.Column1, serviceHeader);

                                    if (customerLoanPrincipalAccounts.Any())
                                    {
                                        if (customerLoanPrincipalAccounts.Count == 1)
                                        {
                                            if (MicroCreditGroupMemberExists(customerLoanPrincipalAccounts[0].CustomerId, microCreditGroupMemberCustomerId, serviceHeader))
                                            {
                                                var targetCustomerAccount = customerLoanPrincipalAccounts[0];

                                                var existingEntries = from b in result.MatchedCollection3
                                                                      where b.CreditCustomerAccountId == targetCustomerAccount.Id
                                                                      select b;

                                                if (existingEntries != null && existingEntries.Any())
                                                {
                                                    if (existingEntries.Count() == 1)
                                                        existingEntries.Single().Principal += contributionAmount;
                                                    else
                                                    {
                                                        item.Remarks = string.Format("Record #{0} ~ (sLoan) >> existing entry couldn't be matched!", count);

                                                        result.MismatchedCollection.Add(item);
                                                    }
                                                }
                                                else
                                                {
                                                    targetCustomerAccount = _customerAccountAppService.FindCustomerAccountDTO(targetCustomerAccount.Id, serviceHeader);

                                                    ApportionmentWrapper apportionment = new ApportionmentWrapper();

                                                    apportionment.ApportionTo = (int)ApportionTo.CustomerAccount;
                                                    apportionment.CreditCustomerAccountId = targetCustomerAccount.Id;
                                                    apportionment.CreditCustomerAccount = targetCustomerAccount;
                                                    apportionment.FullAccountNumber = targetCustomerAccount.FullAccountNumber;
                                                    apportionment.PrimaryDescription = item.Column1;
                                                    apportionment.SecondaryDescription = item.Column2;
                                                    apportionment.Reference = item.Column6;
                                                    apportionment.ProductDescription = targetLoanPrincipalProduct.Description;
                                                    apportionment.Principal = contributionAmount;

                                                    result.MatchedCollection3.Add(apportionment);
                                                }
                                            }
                                            else
                                            {
                                                item.Remarks = string.Format("Record #{0} ~ (sLoan) >> account is not associated with the group!", count);

                                                result.MismatchedCollection.Add(item);
                                            }
                                        }
                                        else
                                        {
                                            item.Remarks = string.Format("Record #{0} ~ found {1} customer account matches.", count, customerLoanPrincipalAccounts.Count());

                                            result.MismatchedCollection.Add(item);
                                        }
                                    }
                                    else
                                    {
                                        item.Remarks = string.Format("Record #{0} ~ no match for loan product customer account.", count);

                                        result.MismatchedCollection.Add(item);
                                    }
                                }
                                else
                                {
                                    item.Remarks = string.Format("Record #{0} ~ no match for loan product code {1}", count, item.Column4);

                                    result.MismatchedCollection.Add(item);
                                }

                                #endregion

                                break;

                            case CheckOffEntryType.sInterest:

                                #region sInterest

                                var sInterest_MatchedLoanProducts = loanProducts.Where(x => x.Code == productCode);

                                if (sInterest_MatchedLoanProducts != null && sInterest_MatchedLoanProducts.Any() && sInterest_MatchedLoanProducts.Count() == 1)
                                {
                                    var targetLoanInterestProduct = sInterest_MatchedLoanProducts.First();

                                    var customerLoanInterestAccounts = _sqlCommandAppService.FindCustomerAccountsByTargetProductIdAndReference3(targetLoanInterestProduct.Id, item.Column1, serviceHeader);

                                    if (customerLoanInterestAccounts.Any())
                                    {
                                        if (customerLoanInterestAccounts.Count == 1)
                                        {
                                            if (MicroCreditGroupMemberExists(customerLoanInterestAccounts[0].CustomerId, microCreditGroupMemberCustomerId, serviceHeader))
                                            {
                                                var targetCustomerAccount = customerLoanInterestAccounts[0];

                                                var existingEntries = from b in result.MatchedCollection3
                                                                      where b.CreditCustomerAccountId == targetCustomerAccount.Id
                                                                      select b;

                                                if (existingEntries != null && existingEntries.Any())
                                                {
                                                    if (existingEntries.Count() == 1)
                                                        existingEntries.Single().Interest += contributionAmount;
                                                    else
                                                    {
                                                        item.Remarks = string.Format("Record #{0} ~ (sInterest) >> Existing entry couldn't be matched!", count);

                                                        result.MismatchedCollection.Add(item);
                                                    }
                                                }
                                                else
                                                {
                                                    targetCustomerAccount = _customerAccountAppService.FindCustomerAccountDTO(targetCustomerAccount.Id, serviceHeader);

                                                    ApportionmentWrapper apportionment = new ApportionmentWrapper();

                                                    apportionment.ApportionTo = (int)ApportionTo.CustomerAccount;
                                                    apportionment.CreditCustomerAccountId = targetCustomerAccount.Id;
                                                    apportionment.CreditCustomerAccount = targetCustomerAccount;
                                                    apportionment.FullAccountNumber = targetCustomerAccount.FullAccountNumber;
                                                    apportionment.PrimaryDescription = item.Column1;
                                                    apportionment.SecondaryDescription = item.Column2;
                                                    apportionment.Reference = item.Column6;
                                                    apportionment.ProductDescription = targetLoanInterestProduct.Description;
                                                    apportionment.Interest = contributionAmount;

                                                    result.MatchedCollection3.Add(apportionment);
                                                }
                                            }
                                            else
                                            {
                                                item.Remarks = string.Format("Record #{0} ~ (sInterest) >> Account is not associated with the group!", count);

                                                result.MismatchedCollection.Add(item);
                                            }
                                        }
                                        else
                                        {
                                            item.Remarks = string.Format("Record #{0} ~ found {1} customer account matches.", count, customerLoanInterestAccounts.Count());

                                            result.MismatchedCollection.Add(item);
                                        }
                                    }
                                    else
                                    {
                                        item.Remarks = string.Format("Record #{0} ~ no match for loan product customer account.", count);

                                        result.MismatchedCollection.Add(item);
                                    }
                                }
                                else
                                {
                                    item.Remarks = string.Format("Record #{0} ~ no match for loan product code {1}", count, item.Column4);

                                    result.MismatchedCollection.Add(item);
                                }

                                #endregion

                                break;

                            case CheckOffEntryType.sShare:
                            case CheckOffEntryType.wCont:
                            case CheckOffEntryType.sInvest:
                            case CheckOffEntryType.sRisk:

                                #region sShare/wCont/sInvest/sRisk

                                var matchedInvestmentProducts = investmentProducts.Where(x => x.Code == productCode);

                                if (matchedInvestmentProducts != null && matchedInvestmentProducts.Any() && matchedInvestmentProducts.Count() == 1)
                                {
                                    var targetInvestmentProduct = matchedInvestmentProducts.First();

                                    var customerInvestmentAccounts = _sqlCommandAppService.FindCustomerAccountsByTargetProductIdAndReference3(targetInvestmentProduct.Id, item.Column1, serviceHeader);

                                    if (customerInvestmentAccounts.Any())
                                    {
                                        if (customerInvestmentAccounts.Count == 1)
                                        {
                                            if (MicroCreditGroupMemberExists(customerInvestmentAccounts[0].CustomerId, microCreditGroupMemberCustomerId, serviceHeader))
                                            {
                                                var targetCustomerAccount = customerInvestmentAccounts[0];

                                                targetCustomerAccount = _customerAccountAppService.FindCustomerAccountDTO(targetCustomerAccount.Id, serviceHeader);

                                                ApportionmentWrapper apportionment = new ApportionmentWrapper();

                                                apportionment.ApportionTo = (int)ApportionTo.CustomerAccount;
                                                apportionment.CreditCustomerAccountId = targetCustomerAccount.Id;
                                                apportionment.CreditCustomerAccount = targetCustomerAccount;
                                                apportionment.FullAccountNumber = targetCustomerAccount.FullAccountNumber;
                                                apportionment.PrimaryDescription = item.Column1;
                                                apportionment.SecondaryDescription = item.Column2;
                                                apportionment.Reference = item.Column6;
                                                apportionment.ProductDescription = targetInvestmentProduct.Description;
                                                apportionment.Principal = contributionAmount;

                                                result.MatchedCollection3.Add(apportionment);
                                            }
                                            else
                                            {
                                                item.Remarks = string.Format("Record #{0} ~ ({1}) >> Account is not associated with the group!", count, item.Column5);

                                                result.MismatchedCollection.Add(item);
                                            }
                                        }
                                        else
                                        {
                                            item.Remarks = string.Format("Record #{0} ~ found {1} customer account matches.", count, customerInvestmentAccounts.Count());

                                            result.MismatchedCollection.Add(item);
                                        }
                                    }
                                    else
                                    {
                                        item.Remarks = string.Format("Record #{0} ~ no match for investment product customer account.", count);

                                        result.MismatchedCollection.Add(item);
                                    }
                                }
                                else
                                {
                                    item.Remarks = string.Format("Record #{0} ~ no match for investment product code {1}", count, item.Column4);

                                    result.MismatchedCollection.Add(item);
                                }

                                #endregion

                                break;

                            default:

                                item.Remarks = string.Format("Record #{0} ~ unable to parse apportioment type {1}.", count, item.Column5);

                                result.MismatchedCollection.Add(item);

                                break;
                        }
                    }
                    else
                    {
                        item.Remarks = string.Format("Record #{0} ~ unable to parse product code {1}.", count, item.Column4);

                        result.MismatchedCollection.Add(item);
                    }
                }
                else
                {
                    item.Remarks = string.Format("Record #{0} ~ unable to parse amount {1}.", count, item.Column3);

                    result.MismatchedCollection.Add(item);
                }

                // tally
                count += 1;
            });

            return result;
        }
    }
}
