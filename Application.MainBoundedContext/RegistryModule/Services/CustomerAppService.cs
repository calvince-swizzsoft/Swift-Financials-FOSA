using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.MessagingModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.MessagingModule.Services;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.RegistryModule.Aggregates.AccountAlertAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.CorporationMemberAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerCreditTypeAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.NextOfKinAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.PartnershipMemberAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.PopulationRegisterQueryAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.RefereeAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Adapter;
using Infrastructure.Crosscutting.Framework.Models;
using Infrastructure.Crosscutting.Framework.Utils;
using LazyCache;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.RegistryModule.Services
{
    public class CustomerAppService : ICustomerAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<NextOfKin> _nextOfKinRepository;
        private readonly IRepository<AccountAlert> _accountAlertRepository;
        private readonly IRepository<PartnershipMember> _partnershipMemberRepository;
        private readonly IRepository<CorporationMember> _corporationMemberRepository;
        private readonly IRepository<Referee> _refereeRepository;
        private readonly IRepository<CustomerCreditType> _customerCreditTypeRepository;
        private readonly IRepository<PopulationRegisterQuery> _populationRegisterQueryRepository;
        private readonly IZoneAppService _zoneAppService;
        private readonly ICustomerAccountAppService _customerAccountAppService;
        private readonly IJournalAppService _journalAppService;
        private readonly ITextAlertAppService _textAlertAppService;
        private readonly IEmailAlertAppService _emailAlertAppService;
        private readonly IBranchAppService _branchAppService;
        private readonly ISqlCommandAppService _sqlCommandAppService;
        private readonly ICommissionAppService _commissionAppService;
        private readonly IBrokerService _brokerService;
        private readonly IAppCache _appCache;

        public CustomerAppService(
            IDbContextScopeFactory dbContextScopeFactory,
            IRepository<Customer> customerRepository,
            IRepository<NextOfKin> nextOfKinRepository,
            IRepository<AccountAlert> accountAlertRepository,
            IRepository<PartnershipMember> partnershipMemberRepository,
            IRepository<CorporationMember> corporationMemberRepository,
            IRepository<Referee> refereeRepository,
            IRepository<CustomerCreditType> customerCreditTypeRepository,
            IRepository<PopulationRegisterQuery> populationRegisterQueryRepository,
            IZoneAppService zoneAppService,
            ICustomerAccountAppService customerAccountAppService,
            IJournalAppService journalAppService,
            ITextAlertAppService textAlertAppService,
            IEmailAlertAppService emailAlertAppService,
            IBranchAppService branchAppService,
            ISqlCommandAppService sqlCommandAppService,
            ICommissionAppService commissionAppService,
            IBrokerService brokerService,
            IAppCache appCache)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (customerRepository == null)
                throw new ArgumentNullException(nameof(customerRepository));

            if (nextOfKinRepository == null)
                throw new ArgumentNullException(nameof(nextOfKinRepository));

            if (accountAlertRepository == null)
                throw new ArgumentNullException(nameof(accountAlertRepository));

            if (partnershipMemberRepository == null)
                throw new ArgumentNullException(nameof(partnershipMemberRepository));

            if (corporationMemberRepository == null)
                throw new ArgumentNullException(nameof(corporationMemberRepository));

            if (refereeRepository == null)
                throw new ArgumentNullException(nameof(refereeRepository));

            if (customerCreditTypeRepository == null)
                throw new ArgumentNullException(nameof(customerCreditTypeRepository));

            if (populationRegisterQueryRepository == null)
                throw new ArgumentNullException(nameof(populationRegisterQueryRepository));

            if (zoneAppService == null)
                throw new ArgumentNullException(nameof(zoneAppService));

            if (customerAccountAppService == null)
                throw new ArgumentNullException(nameof(customerAccountAppService));

            if (journalAppService == null)
                throw new ArgumentNullException(nameof(journalAppService));

            if (textAlertAppService == null)
                throw new ArgumentNullException(nameof(textAlertAppService));

            if (emailAlertAppService == null)
                throw new ArgumentNullException(nameof(emailAlertAppService));

            if (branchAppService == null)
                throw new ArgumentNullException(nameof(branchAppService));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            if (commissionAppService == null)
                throw new ArgumentNullException(nameof(commissionAppService));

            if (brokerService == null)
                throw new ArgumentNullException(nameof(brokerService));

            if (appCache == null)
                throw new ArgumentNullException(nameof(appCache));

            _dbContextScopeFactory = dbContextScopeFactory;
            _customerRepository = customerRepository;
            _nextOfKinRepository = nextOfKinRepository;
            _accountAlertRepository = accountAlertRepository;
            _partnershipMemberRepository = partnershipMemberRepository;
            _corporationMemberRepository = corporationMemberRepository;
            _refereeRepository = refereeRepository;
            _customerCreditTypeRepository = customerCreditTypeRepository;
            _populationRegisterQueryRepository = populationRegisterQueryRepository;
            _zoneAppService = zoneAppService;
            _customerAccountAppService = customerAccountAppService;
            _journalAppService = journalAppService;
            _textAlertAppService = textAlertAppService;
            _emailAlertAppService = emailAlertAppService;
            _branchAppService = branchAppService;
            _sqlCommandAppService = sqlCommandAppService;
            _commissionAppService = commissionAppService;
            _brokerService = brokerService;
            _appCache = appCache;
        }

        public async Task<CustomerDTO> AddNewCustomerAsync(CustomerDTO customerDTO, List<DebitTypeDTO> mandatoryDebitTypes, List<InvestmentProductDTO> mandatoryInvestmentProducts, List<SavingsProductDTO> mandatorySavingsProducts, ProductCollectionInfo mandatoryProducts, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var customerBindingModel = customerDTO.ProjectedAs<CustomerBindingModel>();

            if (customerDTO.StationId == null || customerDTO.StationId == Guid.Empty)
                customerBindingModel.StationId = Guid.NewGuid(); // a hack to pass validation

            customerBindingModel.ValidateAll();
            if (customerBindingModel.HasErrors)
                throw new InvalidOperationException(string.Join(Environment.NewLine, customerBindingModel.ErrorMessages));

            var proceed = true; // Initial state, proceed with the operation
            var branchId = customerDTO.BranchId;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var address = new Address(customerDTO.AddressAddressLine1, customerDTO.AddressAddressLine2, customerDTO.AddressStreet, customerDTO.AddressPostalCode, customerDTO.AddressCity, customerDTO.AddressEmail, customerDTO.AddressLandLine, customerDTO.AddressMobileLine);

                var individual = new Individual(customerDTO.Type, customerDTO.IndividualType, customerDTO.IndividualFirstName, customerDTO.IndividualLastName, customerDTO.IndividualIdentityCardType, customerDTO.IndividualIdentityCardNumber, customerDTO.IndividualIdentityCardSerialNumber, customerDTO.IndividualPayrollNumbers, customerDTO.IndividualSalutation, customerDTO.IndividualGender, customerDTO.IndividualMaritalStatus, customerDTO.IndividualNationality, customerDTO.IndividualBirthDate, customerDTO.IndividualEmploymentDesignation, customerDTO.IndividualEmploymentTermsOfService, customerDTO.IndividualEmploymentDate, customerDTO.IndividualClassification);

                var nonIndividual = new NonIndividual(customerDTO.Type, customerDTO.NonIndividualDescription, customerDTO.NonIndividualRegistrationNumber, customerDTO.NonIndividualRegistrationSerialNumber, customerDTO.NonIndividualDateEstablished);

                var customer = CustomerFactory.CreateCustomer(customerDTO.Type, customerDTO.PersonalIdentificationNumber, individual, nonIndividual, address, customerDTO.StationId, customerDTO.Reference1, customerDTO.Reference2, customerDTO.Reference3, customerDTO.Remarks, customerDTO.RegistrationDate, customerDTO.RecruitedBy, customerDTO.AdministrativeDivisionId);

                customer.SerialNumber = _customerRepository.DatabaseSqlQuery<int>(string.Format("SELECT ISNULL(MAX(SerialNumber),0) + 1 AS Expr1 FROM {0}Customers", DefaultSettings.Instance.TablePrefix), serviceHeader).FirstOrDefault();
                customer.Reference2 =( _customerRepository.DatabaseSqlQuery<int>(string.Format("SELECT ISNULL(MAX(Reference2),0) + 1 AS Expr1 FROM {0}Customers", DefaultSettings.Instance.TablePrefix), serviceHeader).FirstOrDefault()).ToString() ;
                customer.SignatureImageId = IdentityGenerator.NewSequentialGuid();
                customer.IdentityCardBackSideImageId = IdentityGenerator.NewSequentialGuid();
                customer.IdentityCardFrontSideImageId = IdentityGenerator.NewSequentialGuid();
                customer.BiometricFingerprintImageId = IdentityGenerator.NewSequentialGuid();
                customer.BiometricFingerprintTemplateId = IdentityGenerator.NewSequentialGuid();
                customer.BiometricFingerprintTemplateFormat = (byte)customerDTO.BiometricFingerprintTemplateFormat;
                customer.BiometricFingerVeinTemplateId = IdentityGenerator.NewSequentialGuid();
                customer.BiometricFingerVeinTemplateFormat = (byte)customerDTO.BiometricFingerVeinTemplateFormat;
  
                customer.CreatedBy = serviceHeader.ApplicationUserName;

                if (customerDTO.IsLocked) customer.Lock();
                else customer.UnLock();

                if (customerDTO.InhibitGuaranteeing) customer.LockGuaranteeing();
                else customer.UnlockGuaranteeing();

                switch ((CustomerType)customerDTO.Type)
                {
                    case CustomerType.Individual:
                        if (!string.IsNullOrWhiteSpace(customer.Individual.IdentityCardNumber))
                        {
                            var filter1 = CustomerSpecifications.CustomerIndividualIdentityCardNumber(customer.Individual.IdentityCardNumber, true);
                            var matchedCustomers = await _customerRepository.AllMatchingAsync(filter1, serviceHeader);
                            if (matchedCustomers != null && matchedCustomers.Any())
                            {
                                proceed = false;
                                customerDTO.ErrorMessageResult = "A customer with a similar identity card number already exists.";
                                return customerDTO; // Exit early if validation fails
                            }
                        }
                        break;

                    case CustomerType.Partnership:
                    case CustomerType.Corporation:
                        if (!string.IsNullOrWhiteSpace(customer.NonIndividual.RegistrationNumber))
                        {
                            var filter2 = CustomerSpecifications.CustomerNonIndividualRegistrationNumber(customer.NonIndividual.RegistrationNumber);
                            var matchedCustomers1 = await _customerRepository.AllMatchingAsync(filter2, serviceHeader);
                            if (matchedCustomers1 != null && matchedCustomers1.Any())
                            {
                                proceed = false;
                                customerDTO.ErrorMessageResult = "A customer with a similar registration number already exists.";
                                return customerDTO; // Exit early if validation fails
                            }
                        }
                        break;
                }

                if (proceed && !string.IsNullOrWhiteSpace(customer.Individual.PayrollNumbers))
                {
                    var matchedCustomers = _sqlCommandAppService.FindCustomersByPayrollNumber(customer.Individual.PayrollNumbers, serviceHeader);
                    if (matchedCustomers != null && matchedCustomers.Any())
                    {
                        proceed = false;
                        customerDTO.ErrorMessageResult = "A customer with a similar payroll number already exists.";
                        return customerDTO; // Exit early if validation fails
                    }
                }

                _customerRepository.Add(customer, serviceHeader);
                proceed = await dbContextScope.SaveChangesAsync(serviceHeader) > 0;

                if (!proceed)
                {
                    customerDTO.ErrorMessageResult = "Error saving customer to the database.";
                    return customerDTO; // Exit early if save failed
                }

                customerDTO = customer.ProjectedAs<CustomerDTO>();
            }

            if (proceed && customerDTO != null)
            {
                var currrentBranch = _branchAppService.FindBranch(branchId, serviceHeader);
                if (currrentBranch != null)
                {
                    #region Send Text Notification
                    if (currrentBranch.CompanyApplicationMembershipTextAlertsEnabled && !string.IsNullOrWhiteSpace(customerDTO.AddressMobileLine) && Regex.IsMatch(customerDTO.AddressMobileLine, @"^\+(?:[0-9]??){6,14}[0-9]$") && customerDTO.AddressMobileLine.Length >= 13)
                    {
                        var smsBody = new StringBuilder();
                        smsBody.AppendFormat("Dear {0},\nWelcome to {1}.", customerDTO.FullName, currrentBranch.CompanyDescription);
                        smsBody.Append(!string.IsNullOrWhiteSpace(customerDTO.Reference2) ? $"\nYour membership number is {customerDTO.Reference2}." : $"\nYour serial number is {customerDTO.PaddedSerialNumber}.");
                        var textAlertDTO = new TextAlertDTO
                        {
                            BranchId = currrentBranch.Id,
                            TextMessageOrigin = (int)MessageOrigin.Within,
                            TextMessageRecipient = customerDTO.AddressMobileLine,
                            TextMessageBody = smsBody.ToString(),
                            MessageCategory = (int)MessageCategory.SMSAlert,
                            AppendSignature = false,
                            TextMessagePriority = (int)QueuePriority.Highest,
                        };
                        _textAlertAppService.AddNewTextAlert(textAlertDTO, serviceHeader);
                    }
                    #endregion

                    #region Auto-Create Mandatory Accounts
                    if (mandatoryProducts != null)
                    {
                        customerDTO.BranchId = currrentBranch.Id;
                        customerDTO.BranchDescription = currrentBranch.Description;
                        _customerAccountAppService.AddNewCustomerAccounts(customerDTO, mandatoryProducts.SavingsProductCollection, mandatoryProducts.InvestmentProductCollection, mandatoryProducts.LoanProductCollection, serviceHeader);
                    
                    }
                    #endregion

                    #region Effect Mandatory Debit Types
                    if (mandatoryDebitTypes != null && mandatoryDebitTypes.Any())
                    {
                        foreach (var item in mandatoryDebitTypes)
                        {
                            var customerAccounts = _customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(customerDTO.Id, item.CustomerAccountTypeTargetProductId, serviceHeader);
                            var customerAccountDTO = customerAccounts?.FirstOrDefault() ?? new CustomerAccountDTO
                            {
                                CustomerId = customerDTO.Id,
                                BranchId = currrentBranch.Id,
                                CustomerAccountTypeProductCode = item.CustomerAccountTypeProductCode,
                                CustomerAccountTypeTargetProductId = item.CustomerAccountTypeTargetProductId,
                                CustomerAccountTypeTargetProductCode = item.CustomerAccountTypeTargetProductCode,
                                Status = (int)CustomerAccountStatus.Normal,
                            };

                            if (customerAccounts == null || !customerAccounts.Any())
                                customerAccountDTO = _customerAccountAppService.AddNewCustomerAccount(customerAccountDTO, serviceHeader);

                            var debitTypeTariffs = _commissionAppService.ComputeTariffsByDebitType(item.Id, 0m, 1d, customerAccountDTO, serviceHeader);
                            foreach (var tariff in debitTypeTariffs)
                            {
                                _journalAppService.AddNewJournal(currrentBranch.Id, null, tariff.Amount, tariff.Description, item.Description, string.Format("{0}", customerDTO.SerialNumber).PadLeft(6, '0'), moduleNavigationItemCode, 0, null, tariff.CreditGLAccountId, tariff.DebitGLAccountId, customerAccountDTO, customerAccountDTO, serviceHeader);
                            }
                        }
                    }
                    #endregion
                }
            }

            return customerDTO;
        }
        public async Task<bool> UpdateCustomerAsync(CustomerDTO customerDTO, ServiceHeader serviceHeader)
        {
            var customerBindingModel = customerDTO.ProjectedAs<CustomerBindingModel>();

            if (customerDTO.StationId == null || customerDTO.StationId == Guid.Empty)
                customerBindingModel.StationId = Guid.NewGuid();/*a hack to pass validation*/

            customerBindingModel.ValidateAll();

            if (customerBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, customerBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _customerRepository.GetAsync(customerDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var address = new Address(customerDTO.AddressAddressLine1, customerDTO.AddressAddressLine2, customerDTO.AddressStreet, customerDTO.AddressPostalCode, customerDTO.AddressCity, customerDTO.AddressEmail, customerDTO.AddressLandLine, customerDTO.AddressMobileLine);

                    var individual = new Individual(customerDTO.Type, customerDTO.IndividualType, customerDTO.IndividualFirstName, customerDTO.IndividualLastName, customerDTO.IndividualIdentityCardType, customerDTO.IndividualIdentityCardNumber, customerDTO.IndividualIdentityCardSerialNumber, customerDTO.IndividualPayrollNumbers, customerDTO.IndividualSalutation, customerDTO.IndividualGender, customerDTO.IndividualMaritalStatus, customerDTO.IndividualNationality, customerDTO.IndividualBirthDate, customerDTO.IndividualEmploymentDesignation, customerDTO.IndividualEmploymentTermsOfService, customerDTO.IndividualEmploymentDate, customerDTO.IndividualClassification);

                    var nonIndividual = new NonIndividual(customerDTO.Type, customerDTO.NonIndividualDescription, customerDTO.NonIndividualRegistrationNumber, customerDTO.NonIndividualRegistrationSerialNumber, customerDTO.NonIndividualDateEstablished);

                    var current = CustomerFactory.CreateCustomer(customerDTO.Type, customerDTO.PersonalIdentificationNumber, individual, nonIndividual, address, customerDTO.StationId, customerDTO.Reference1, customerDTO.Reference2, customerDTO.Reference3, customerDTO.Remarks, customerDTO.RegistrationDate, customerDTO.RecruitedBy, customerDTO.AdministrativeDivisionId);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    current.SerialNumber = persisted.SerialNumber;
                    current.PassportImageId = persisted.PassportImageId ?? IdentityGenerator.NewSequentialGuid();
                    current.SignatureImageId = persisted.SignatureImageId ?? IdentityGenerator.NewSequentialGuid();
                    current.IdentityCardBackSideImageId = persisted.IdentityCardBackSideImageId ?? IdentityGenerator.NewSequentialGuid();
                    current.IdentityCardFrontSideImageId = persisted.IdentityCardFrontSideImageId ?? IdentityGenerator.NewSequentialGuid();
                    current.BiometricFingerprintImageId = persisted.BiometricFingerprintImageId ?? IdentityGenerator.NewSequentialGuid();
                    current.BiometricFingerprintTemplateId = persisted.BiometricFingerprintTemplateId ?? IdentityGenerator.NewSequentialGuid();
                    current.BiometricFingerprintTemplateFormat = (byte)customerDTO.BiometricFingerprintTemplateFormat;
                    current.BiometricFingerVeinTemplateId = persisted.BiometricFingerVeinTemplateId ?? IdentityGenerator.NewSequentialGuid();
                    current.BiometricFingerVeinTemplateFormat = (byte)customerDTO.BiometricFingerVeinTemplateFormat;
                    current.CreatedBy = persisted.CreatedBy;

                    current.IsDefaulter = persisted.IsDefaulter;

                    current.RecordStatus = (byte)customerDTO.RecordStatus;
                    current.ModifiedBy = serviceHeader.ApplicationUserName;
                    current.ModifiedDate = DateTime.Now;

                    if (customerDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    if (customerDTO.InhibitGuaranteeing)
                        current.LockGuaranteeing();
                    else current.UnlockGuaranteeing();

                    var proceed = true;

                    if (!customerDTO.BiometricEnrollment/*ignore below checks for biometric enrollment*/)
                    {
                        switch ((CustomerType)current.Type)
                        {
                            case CustomerType.Individual:

                                if (!string.IsNullOrWhiteSpace(current.Individual.IdentityCardNumber))
                                {
                                    var filter1 = CustomerSpecifications.CustomerIndividualIdentityCardNumber(current.Individual.IdentityCardNumber, true);

                                    ISpecification<Customer> spec = filter1;

                                    var matchedCustomers = await _customerRepository.AllMatchingAsync(spec, serviceHeader);

                                    if (matchedCustomers != null && matchedCustomers.Except(new List<Customer> { current }).Any())
                                        proceed = false;
                                }

                                break;
                            case CustomerType.Partnership:
                            case CustomerType.Corporation:

                                if (!string.IsNullOrWhiteSpace(current.NonIndividual.RegistrationNumber))
                                {
                                    var filter2 = CustomerSpecifications.CustomerNonIndividualRegistrationNumber(current.NonIndividual.RegistrationNumber);

                                    ISpecification<Customer> spec1 = filter2;

                                    var matchedCustomers1 = await _customerRepository.AllMatchingAsync(spec1, serviceHeader);

                                    if (matchedCustomers1 != null && matchedCustomers1.Except(new List<Customer> { current }).Any())
                                        proceed = false;
                                }

                                break;
                            default:
                                break;
                        }

                        if (proceed && !string.IsNullOrWhiteSpace(current.Individual.PayrollNumbers))
                        {
                            var matchedCustomers = _sqlCommandAppService.FindCustomersByPayrollNumber(current.Individual.PayrollNumbers, serviceHeader);

                            if (matchedCustomers != null && matchedCustomers.Where(x => x.Id != current.Id).Any())
                                proceed = false;
                        }
                    }

                    if (persisted.Address.MobileLine != current.Address.MobileLine)
                    {
                        #region Send alert?

                        var queueDTO = new QueueDTO
                        {
                            RecordId = persisted.Id,
                            AppDomainName = serviceHeader.ApplicationDomainName,
                            AccountAlertReference = "Mobile Line",
                            AccountAlertPrimaryDescription = current.Address.MobileLine,
                            AccountAlertSecondaryDescription = persisted.Address.MobileLine ?? current.Address.MobileLine,
                            AccountAlertTrigger = (int)AccountAlertTrigger.CustomerDetailsEditing,
                        };

                        _brokerService.ProcessCustomerDetailsEditingAccountAlerts(DMLCommand.None, serviceHeader, queueDTO);

                        #endregion
                    }

                    if (!proceed)
                        customerDTO.ErrorMessageResult = ("Sorry, but a customer with a similar identity and/or payroll numbers already exists!");
                    else
                        _customerRepository.Merge(persisted, current, serviceHeader);
                }

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }
        }

        public async Task<bool> UpdateNextOfKinCollectionAsync(CustomerDTO customerDTO, List<NextOfKinDTO> nextOfKinCollection, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _customerRepository.GetAsync(customerDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var existing = await FindNextOfKinCollectionAsync(persisted.Id, serviceHeader);

                    if (existing != null && existing.Any())
                    {
                        foreach (var item in existing)
                        {
                            var nextOfKin = await _nextOfKinRepository.GetAsync(item.Id, serviceHeader);

                            if (nextOfKin != null)
                            {
                                _nextOfKinRepository.Remove(nextOfKin, serviceHeader);
                            }
                        }
                    }

                    if (nextOfKinCollection != null && nextOfKinCollection.Any())
                    {
                        foreach (var item in nextOfKinCollection)
                        {
                            var address = new Address(item.AddressAddressLine1, item.AddressAddressLine2, item.AddressStreet, item.AddressPostalCode, item.AddressCity, item.AddressEmail, item.AddressLandLine, item.AddressMobileLine);

                            var nextOfKin = NextOfKinFactory.CreateNextOfKin(persisted.Id, item.Salutation, item.FirstName, item.LastName, item.IdentityCardType, item.IdentityCardNumber, item.Gender, item.Relationship, address, item.NominatedPercentage, item.Remarks);

                            nextOfKin.CreatedBy = serviceHeader.ApplicationUserName;

                            _nextOfKinRepository.Add(nextOfKin, serviceHeader);
                        }

                        var incomingNextOfKinSet = from c in nextOfKinCollection ?? new List<NextOfKinDTO> { } select c;

                        var newNextOfKinSet = incomingNextOfKinSet;

                        if (existing != null && existing.Any())
                        {
                            var existingNextOfKinSet = from c in existing ?? new List<NextOfKinDTO> { } select c;

                            var commonNextOfKinSet = existingNextOfKinSet.Intersect(incomingNextOfKinSet, new NextOfKinDTOEqualityComparer());

                            newNextOfKinSet = incomingNextOfKinSet.Except(commonNextOfKinSet, new NextOfKinDTOEqualityComparer());
                        }

                        if (newNextOfKinSet != null)
                        {
                            #region Send alert?

                            var nextOfKinNamesList = new List<string>();

                            foreach (var nextOfKin in newNextOfKinSet)
                            {
                                nextOfKinNamesList.Add(string.Format("{0} {1}", nextOfKin.FirstName, nextOfKin.LastName));
                            }

                            var queueDTO = new QueueDTO
                            {
                                RecordId = persisted.Id,
                                AppDomainName = serviceHeader.ApplicationDomainName,
                                AccountAlertReference = "Next Of Kin",
                                AccountAlertPrimaryDescription = string.Join(",", nextOfKinNamesList.ToArray()),
                                AccountAlertSecondaryDescription = persisted.Address.MobileLine,
                                AccountAlertTrigger = (int)AccountAlertTrigger.CustomerDetailsEditing,
                            };

                            _brokerService.ProcessCustomerDetailsEditingAccountAlerts(DMLCommand.None, serviceHeader, queueDTO);

                            #endregion
                        }
                    }
                }

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }
        }

        public async Task<bool> UpdateAccountAlertCollectionAsync(Guid customerId, List<AccountAlertDTO> accountAlertCollection, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _customerRepository.GetAsync(customerId, serviceHeader);

                if (persisted != null)
                {
                    var existing = await FindAccountAlertCollectionAsync(persisted.Id, serviceHeader);

                    if (existing != null && existing.Any())
                    {
                        foreach (var item in existing)
                        {
                            var accountAlert = await _accountAlertRepository.GetAsync(item.Id, serviceHeader);

                            if (accountAlert != null)
                            {
                                _accountAlertRepository.Remove(accountAlert, serviceHeader);
                            }
                        }
                    }

                    if (accountAlertCollection != null && accountAlertCollection.Any())
                    {
                        foreach (var item in accountAlertCollection)
                        {
                            var accountAlert = AccountAlertFactory.CreateAccountAlert(persisted.Id, item.Type, item.Threshold, item.Priority, item.MaskTransactionValue, item.ReceiveTextAlert, item.ReceiveEmailAlert);

                            accountAlert.CreatedBy = serviceHeader.ApplicationUserName;

                            _accountAlertRepository.Add(accountAlert, serviceHeader);
                        }
                    }
                }

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }
        }

        public async Task<bool> UpdatePartnershipMemberCollectionAsync(Guid partnershipId, List<PartnershipMemberDTO> partnershipMemberCollection, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _customerRepository.GetAsync(partnershipId, serviceHeader);

                if (persisted != null)
                {
                    var existing = await FindRefereeCollectionAsync(persisted.Id, serviceHeader);

                    if (existing != null && existing.Any())
                    {
                        foreach (var item in existing)
                        {
                            var partnershipMember = await _partnershipMemberRepository.GetAsync(item.Id, serviceHeader);

                            if (partnershipMember != null)
                            {
                                _partnershipMemberRepository.Remove(partnershipMember, serviceHeader);
                            }
                        }
                    }

                    if (partnershipMemberCollection != null && partnershipMemberCollection.Any())
                    {
                        foreach (var item in partnershipMemberCollection)
                        {
                            var address = new Address(item.AddressAddressLine1, item.AddressAddressLine2, item.AddressStreet, item.AddressPostalCode, item.AddressCity, item.AddressEmail, item.AddressLandLine, item.AddressMobileLine);

                            var partnershipMember = PartnershipMemberFactory.CreatePartnershipMember(persisted.Id, item.Salutation, item.FirstName, item.LastName, item.IdentityCardType, item.IdentityCardNumber, item.Gender, item.Relationship, address, item.Remarks, item.Signatory);

                            partnershipMember.CreatedBy = serviceHeader.ApplicationUserName;

                            _partnershipMemberRepository.Add(partnershipMember, serviceHeader);
                        }
                    }
                }

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }
        }

        public async Task<bool> UpdateCorporationMemberCollectionAsync(Guid corporationId, List<CorporationMemberDTO> corporationMemberCollection, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _customerRepository.GetAsync(corporationId, serviceHeader);

                if (persisted != null)
                {
                    var existing = await FindCorporationMemberCollectionAsync(persisted.Id, serviceHeader);

                    if (existing != null && existing.Any())
                    {
                        foreach (var item in existing)
                        {
                            var corporationMember = await _corporationMemberRepository.GetAsync(item.Id, serviceHeader);

                            if (corporationMember != null)
                            {
                                _corporationMemberRepository.Remove(corporationMember, serviceHeader);
                            }
                        }
                    }

                    if (corporationMemberCollection != null && corporationMemberCollection.Any())
                    {
                        foreach (var item in corporationMemberCollection)
                        {
                            var corporationMember = CorporationMemberFactory.CreateCorporationMember(persisted.Id, item.CustomerId, item.Remarks, item.Signatory);

                            corporationMember.CreatedBy = serviceHeader.ApplicationUserName;

                            _corporationMemberRepository.Add(corporationMember, serviceHeader);
                        }
                    }
                }

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }
        }

        public async Task<bool> UpdateRefereeCollectionAsync(Guid customerId, List<RefereeDTO> refereeCollection, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _customerRepository.GetAsync(customerId, serviceHeader);

                if (persisted != null)
                {
                    var existing = await FindRefereeCollectionAsync(persisted.Id, serviceHeader);

                    if (existing != null && existing.Any())
                    {
                        foreach (var item in existing)
                        {
                            var referee = await _refereeRepository.GetAsync(item.Id, serviceHeader);

                            if (referee != null)
                            {
                                _refereeRepository.Remove(referee, serviceHeader);
                            }
                        }
                    }

                    if (refereeCollection != null && refereeCollection.Any())
                    {
                        foreach (var item in refereeCollection)
                        {
                            var referee = RefereeFactory.CreateReferee(persisted.Id, item.WitnessId, item.Remarks);

                            referee.CreatedBy = serviceHeader.ApplicationUserName;

                            _refereeRepository.Add(referee, serviceHeader);
                        }
                    }
                }

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }
        }

        public async Task<bool> UpdateCustomerStationAsync(CustomerDTO customerDTO, ServiceHeader serviceHeader)
        {
            if (customerDTO == null)
                return false;

            var station = await _zoneAppService.FindStationAsync(customerDTO.StationId ?? Guid.Empty, serviceHeader);

            if (station == null)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _customerRepository.GetAsync(customerDTO.Id, serviceHeader);

                if (persisted == null)
                    return false;

                persisted.StationId = station.Id;

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }
        }

        public async Task<bool> ResetCustomerStationAsync(List<CustomerDTO> customerDTOs, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                foreach (var item in customerDTOs)
                {
                    if (item.Id != null && item.Id != Guid.Empty)
                    {
                        var persisted = await _customerRepository.GetAsync(item.Id, serviceHeader);

                        if (persisted != null)
                        {
                            persisted.StationId = null;
                        }
                    }
                }

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }
        }

        public bool UpdateCustomerBranch(CustomerDTO customerDTO, ServiceHeader serviceHeader)
        {
            if (customerDTO == null)
                return false;

            var branch = _branchAppService.FindBranch(customerDTO.BranchId, serviceHeader);

            if (branch == null)
                return false;

            var customerAccountDTOs = _customerAccountAppService.FindCustomerAccountsByCustomerId(customerDTO.Id, serviceHeader);

            if (customerAccountDTOs != null && customerAccountDTOs.Any())
            {
                customerAccountDTOs.ForEach(customerAccountDTO =>
                {
                    customerAccountDTO.BranchId = branch.Id;
                });

                return _customerAccountAppService.UpdateCustomerAccountsBranch(customerAccountDTOs, serviceHeader);
            }
            else return false;
        }

        public async Task<bool> UpdateCreditTypesAsync(Guid customerId, List<CreditTypeDTO> creditTypeDTOs, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _customerRepository.GetAsync(customerId, serviceHeader);

                if (persisted != null)
                {
                    var filter = CustomerCreditTypeSpecifications.CustomerCreditTypeWithCustomerId(customerId);

                    ISpecification<CustomerCreditType> spec = filter;

                    var customerCreditTypes = await _customerCreditTypeRepository.AllMatchingAsync(spec, serviceHeader);

                    if (customerCreditTypes != null)
                    {
                        customerCreditTypes.ToList().ForEach(x => _customerCreditTypeRepository.Remove(x, serviceHeader));
                    }

                    if (creditTypeDTOs != null && creditTypeDTOs.Any())
                    {
                        foreach (var item in creditTypeDTOs)
                        {
                            var customerCreditType = CustomerCreditTypeFactory.CreateCustomerCreditType(persisted.Id, item.Id);

                            customerCreditType.CreatedBy = serviceHeader.ApplicationDomainName;

                            _customerCreditTypeRepository.Add(customerCreditType, serviceHeader);
                        }
                    }
                }

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }
        }

        public async Task<List<CustomerDTO>> FindCustomersAsync(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CustomerSpecifications.DefaultSpec();

                ISpecification<Customer> spec = filter;

                return await _customerRepository.AllMatchingAsync<CustomerDTO>(spec, serviceHeader);
            }
        }

        public async Task<PageCollectionInfo<CustomerDTO>> FindCustomersAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CustomerSpecifications.DefaultSpec();

                ISpecification<Customer> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return await _customerRepository.AllMatchingPagedAsync<CustomerDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public async Task<PageCollectionInfo<CustomerDTO>> FindCustomersAsync(string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CustomerSpecifications.CustomerFullText(text, customerFilter);

                ISpecification<Customer> spec = filter;


                var sortFields = new List<string> { "SequentialId" };

                return await _customerRepository.AllMatchingPagedAsync<CustomerDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public async Task<PageCollectionInfo<CustomerDTO>> FindCustomersByTypeAsync(int type, string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CustomerSpecifications.CustomerType(type, text, customerFilter);

                ISpecification<Customer> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return await _customerRepository.AllMatchingPagedAsync<CustomerDTO>(spec, pageIndex, pageSize, sortFields, false, serviceHeader);
            }
        }

        public async Task<PageCollectionInfo<CustomerDTO>> FindCustomersByRecordStatusAsync(int recordStatus, string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CustomerSpecifications.CustomerRecordStatus(recordStatus, text, customerFilter);

                ISpecification<Customer> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return await _customerRepository.AllMatchingPagedAsync<CustomerDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public async Task<PageCollectionInfo<CustomerDTO>> FindCustomersAsync(Guid stationId, string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CustomerSpecifications.CustomerWithStationId(stationId, text, customerFilter);

                ISpecification<Customer> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return await _customerRepository.AllMatchingPagedAsync<CustomerDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public CustomerDTO FindCustomer(Guid customerId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _customerRepository.Get<CustomerDTO>(customerId, serviceHeader);
            }
        }

        public async Task<CustomerDTO> FindCustomerAsync(Guid customerId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _customerRepository.GetAsync<CustomerDTO>(customerId, serviceHeader);
            }
        }

        public async Task<List<CustomerDTO>> FindCustomersByPayrollNumbersAsync(string payrollNumbers, bool matchExtact, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CustomerSpecifications.CustomerPayrollNumbers(payrollNumbers, matchExtact);

                ISpecification<Customer> spec = filter;

                return await _customerRepository.AllMatchingAsync<CustomerDTO>(spec, serviceHeader);
            }
        }

        public List<CustomerDTO> FindCustomerBySerialNumber(int serialNumber, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CustomerSpecifications.CustomerSerialNumber(serialNumber);

                ISpecification<Customer> spec = filter;

                var customers = _customerRepository.AllMatching(spec, serviceHeader);

                if (customers != null && customers.Any())
                {
                    return customers.ProjectedAsCollection<CustomerDTO>();
                }
                else return null;
            }
        }

        public async Task<List<CustomerDTO>> FindCustomerBySerialNumberAsync(int serialNumber, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CustomerSpecifications.CustomerSerialNumber(serialNumber);

                ISpecification<Customer> spec = filter;

                return (await _customerRepository.AllMatchingAsync<CustomerDTO>(spec, serviceHeader)).ToList();
            }
        }

        public async Task<List<CustomerDTO>> FindCustomersByIdentityCardNumberAsync(string identityCardNumber, bool matchExtact, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CustomerSpecifications.CustomerIndividualIdentityCardNumber(identityCardNumber, matchExtact);

                ISpecification<Customer> spec = filter;

                return await _customerRepository.AllMatchingAsync<CustomerDTO>(spec, serviceHeader);
            }
        }

        public async Task<List<CustomerDTO>> FindCustomersByIDNumberAsync(string identityCardNumber, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CustomerSpecifications.CustomerIndividualIDNumber(identityCardNumber);

                ISpecification<Customer> spec = filter;

                return await _customerRepository.AllMatchingAsync<CustomerDTO>(spec, serviceHeader);
            }
        }

        public async Task<List<NextOfKinDTO>> FindNextOfKinCollectionAsync(Guid customerId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = NextOfKinSpecifications.NextOfKinWithCustomerId(customerId);

                ISpecification<NextOfKin> spec = filter;

                return await _nextOfKinRepository.AllMatchingAsync<NextOfKinDTO>(spec, serviceHeader);
            }
        }

        public async Task<List<AccountAlertDTO>> FindAccountAlertCollectionAsync(Guid customerId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = AccountAlertSpecifications.AccountAlertWithCustomerId(customerId);

                ISpecification<AccountAlert> spec = filter;

                return await _accountAlertRepository.AllMatchingAsync<AccountAlertDTO>(spec, serviceHeader);
            }
        }

        public List<AccountAlertDTO> FindAccountAlertCollection(Guid customerId, int accountAlertType, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = AccountAlertSpecifications.AccountAlertWithCustomerIdAndType(customerId, accountAlertType);

                ISpecification<AccountAlert> spec = filter;

                var accountAlertCollection = _accountAlertRepository.AllMatching(spec, serviceHeader);

                if (accountAlertCollection != null)
                {
                    return accountAlertCollection.ProjectedAsCollection<AccountAlertDTO>();
                }
                else return null;
            }
        }

        public List<AccountAlertDTO> FindCachedAccountAlertCollection(Guid customerId, int accountAlertType, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<List<AccountAlertDTO>>(string.Format("AccountAlertCollectionByCustomerIdAndAccountAlertType_{0}_{1}_{2}", serviceHeader.ApplicationDomainName, customerId, accountAlertType), () =>
            {
                return FindAccountAlertCollection(customerId, accountAlertType, serviceHeader);
            });
        }

        public async Task<List<PartnershipMemberDTO>> FindPartnershipMemberCollectionAsync(Guid partnershipId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = PartnershipMemberSpecifications.PartnershipMemberWithPartnershipId(partnershipId);

                ISpecification<PartnershipMember> spec = filter;

                return await _partnershipMemberRepository.AllMatchingAsync<PartnershipMemberDTO>(spec, serviceHeader);
            }
        }

        public async Task<List<CorporationMemberDTO>> FindCorporationMemberCollectionAsync(Guid corporationId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CorporationMemberSpecifications.CorporationMemberWithCorporationId(corporationId);

                ISpecification<CorporationMember> spec = filter;

                return await _corporationMemberRepository.AllMatchingAsync<CorporationMemberDTO>(spec, serviceHeader);
            }
        }

        public async Task<List<RefereeDTO>> FindRefereeCollectionAsync(Guid customerId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = RefereeSpecifications.RefereeWithCustomerId(customerId);

                ISpecification<Referee> spec = filter;

                return await _refereeRepository.AllMatchingAsync<RefereeDTO>(spec, serviceHeader);
            }
        }

        public List<CreditTypeDTO> FindCreditTypes(Guid customerId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CustomerCreditTypeSpecifications.CustomerCreditTypeWithCustomerId(customerId);

                ISpecification<CustomerCreditType> spec = filter;

                var customerCreditTypes = _customerCreditTypeRepository.AllMatching(spec, serviceHeader);

                if (customerCreditTypes != null)
                {
                    var projection = customerCreditTypes.ProjectedAsCollection<CustomerCreditTypeDTO>();

                    return (from p in projection select p.CreditType).ToList();
                }
                else return null;
            }
        }

        public PopulationRegisterQueryDTO AddNewPopulationRegisterQuery(PopulationRegisterQueryDTO populationRegisterQueryDTO, ServiceHeader serviceHeader)
        {
            var populationRegisterQueryBindingModel = populationRegisterQueryDTO.ProjectedAs<PopulationRegisterQueryBindingModel>();

            populationRegisterQueryBindingModel.ValidateAll();

            if (populationRegisterQueryBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, populationRegisterQueryBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var populationRegisterQuery = PopulationRegisterQueryFactory.CreatePopulationRegisterQuery(populationRegisterQueryDTO.BranchId, populationRegisterQueryDTO.CustomerId, populationRegisterQueryDTO.IdentityType, populationRegisterQueryDTO.IdentityNumber, populationRegisterQueryDTO.IdentitySerialNumber, populationRegisterQueryDTO.Remarks);
                populationRegisterQuery.Status = (int)PopulationRegisterQueryStatus.New;
                populationRegisterQuery.PhotoImageId = IdentityGenerator.NewSequentialGuid();
                populationRegisterQuery.PhotoFromPassportImageId = IdentityGenerator.NewSequentialGuid();
                populationRegisterQuery.SignatureImageId = IdentityGenerator.NewSequentialGuid();
                populationRegisterQuery.FingerprintImageId = IdentityGenerator.NewSequentialGuid();
                populationRegisterQuery.CreatedBy = serviceHeader.ApplicationUserName;

                _populationRegisterQueryRepository.Add(populationRegisterQuery, serviceHeader);

                dbContextScope.SaveChanges(serviceHeader);

                populationRegisterQueryDTO = populationRegisterQuery.ProjectedAs<PopulationRegisterQueryDTO>();
            }

            return populationRegisterQueryDTO;
        }

        public bool AuthorizePopulationRegisterQuery(PopulationRegisterQueryDTO populationRegisterQueryDTO, int populationRegisterQueryAuthOption, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (populationRegisterQueryDTO != null && Enum.IsDefined(typeof(PopulationRegisterQueryAuthOption), populationRegisterQueryAuthOption))
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _populationRegisterQueryRepository.Get(populationRegisterQueryDTO.Id, serviceHeader);

                    if (persisted != null && persisted.Status == (int)PopulationRegisterQueryStatus.New)
                    {
                        switch ((PopulationRegisterQueryAuthOption)populationRegisterQueryAuthOption)
                        {
                            case PopulationRegisterQueryAuthOption.Authorize:

                                persisted.Status = (int)PopulationRegisterQueryStatus.Authorized;
                                persisted.AuthorizationRemarks = populationRegisterQueryDTO.AuthorizationRemarks;
                                persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                                persisted.AuthorizedDate = DateTime.Now;

                                break;
                            case PopulationRegisterQueryAuthOption.Reject:

                                persisted.Status = (int)PopulationRegisterQueryStatus.Rejected;
                                persisted.AuthorizationRemarks = populationRegisterQueryDTO.AuthorizationRemarks;
                                persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                                persisted.AuthorizedDate = DateTime.Now;

                                break;
                            default:
                                break;
                        }

                        result = dbContextScope.SaveChanges(serviceHeader) > 0;
                    }
                }

                if (result && populationRegisterQueryAuthOption == (int)PopulationRegisterQueryAuthOption.Authorize)
                {
                    _brokerService.ProcessPopulationRegisterQueries(DMLCommand.None, serviceHeader, populationRegisterQueryDTO);
                }
            }

            return result;
        }

        public bool SyncPopulationRegisterQueryResponse(PopulationRegisterQueryDTO populationRegisterQueryDTO, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (populationRegisterQueryDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _populationRegisterQueryRepository.Get(populationRegisterQueryDTO.Id, serviceHeader);

                    if (persisted != null && persisted.Status == (int)PopulationRegisterQueryStatus.Authorized)
                    {
                        persisted.Status = (int)PopulationRegisterQueryStatus.Queried;

                        persisted.IDNumber = populationRegisterQueryDTO.IDNumber;
                        persisted.SerialNumber = populationRegisterQueryDTO.SerialNumber;
                        persisted.Gender = populationRegisterQueryDTO.Gender;
                        persisted.FirstName = populationRegisterQueryDTO.FirstName;
                        persisted.OtherName = populationRegisterQueryDTO.OtherName;
                        persisted.Surname = populationRegisterQueryDTO.Surname;
                        persisted.Pin = populationRegisterQueryDTO.Pin;
                        persisted.Citizenship = populationRegisterQueryDTO.Citizenship;
                        persisted.Family = populationRegisterQueryDTO.Family;
                        persisted.Clan = populationRegisterQueryDTO.Clan;
                        persisted.EthnicGroup = populationRegisterQueryDTO.EthnicGroup;
                        persisted.Occupation = populationRegisterQueryDTO.Occupation;
                        persisted.PlaceOfBirth = populationRegisterQueryDTO.PlaceOfBirth;
                        persisted.PlaceOfDeath = populationRegisterQueryDTO.PlaceOfDeath;
                        persisted.PlaceOfLive = populationRegisterQueryDTO.PlaceOfLive;
                        persisted.RegOffice = populationRegisterQueryDTO.RegOffice;
                        persisted.DateOfBirth = populationRegisterQueryDTO.DateOfBirth;
                        persisted.DateOfBirthFromPassport = populationRegisterQueryDTO.DateOfBirthFromPassport;
                        persisted.DateOfDeath = populationRegisterQueryDTO.DateOfDeath;
                        persisted.DateOfIssue = populationRegisterQueryDTO.DateOfIssue;
                        persisted.DateOfExpiry = populationRegisterQueryDTO.DateOfExpiry;
                        persisted.ErrorResponse = populationRegisterQueryDTO.ErrorResponse;

                        result = dbContextScope.SaveChanges(serviceHeader) > 0;
                    }
                }
            }

            return result;
        }

        public PopulationRegisterQueryDTO FindPopulationRegisterQuery(Guid populationRegisterQueryId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _populationRegisterQueryRepository.Get<PopulationRegisterQueryDTO>(populationRegisterQueryId, serviceHeader);
            }
        }

        public PageCollectionInfo<PopulationRegisterQueryDTO> FindPopulationRegisterQueries(int status, DateTime startDate, DateTime endDate, string text, int populationRegisterFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = PopulationRegisterQuerySpecifications.PopulationRegisterQueryFullText(status, startDate, endDate, text, populationRegisterFilter);

                ISpecification<PopulationRegisterQuery> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return _populationRegisterQueryRepository.AllMatchingPaged<PopulationRegisterQueryDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public PageCollectionInfo<PopulationRegisterQueryDTO> FindThirdPartyNotifiablePopulationRegisterQueries(string text, int populationRegisterFilter, int pageIndex, int pageSize, int daysCap, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = PopulationRegisterQuerySpecifications.ThirdPartyNotifiablePopulationRegisterQueries(text, populationRegisterFilter, daysCap);

                ISpecification<PopulationRegisterQuery> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return _populationRegisterQueryRepository.AllMatchingPaged<PopulationRegisterQueryDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public async Task<int> GetCustomersCountAsync(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _customerRepository.CountAllAsync(serviceHeader);
            }
        }
    }
}
