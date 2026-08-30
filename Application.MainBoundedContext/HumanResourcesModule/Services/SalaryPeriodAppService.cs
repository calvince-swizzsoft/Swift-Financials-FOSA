using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryPeriodAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using LazyCache;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public class SalaryPeriodAppService : ISalaryPeriodAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<SalaryPeriod> _salaryPeriodRepository;
        private readonly IPostingPeriodAppService _postingPeriodAppService;
        private readonly IChartOfAccountAppService _chartOfAccountAppService;
        private readonly ICustomerAccountAppService _customerAccountAppService;
        private readonly IJournalEntryPostingService _journalEntryPostingService;
        private readonly ILoanProductAppService _loanProductAppService;
        private readonly ISavingsProductAppService _savingsProductAppService;
        private readonly IInvestmentProductAppService _investmentProductAppService;
        private readonly ICommissionAppService _commissionAppService;
        private readonly IStandingOrderAppService _standingOrderAppService;
        private readonly ISalaryCardAppService _salaryCardAppService;
        private readonly IPaySlipAppService _paySlipAppService;
        private readonly ISqlCommandAppService _sqlCommandAppService;
        private readonly IRecurringBatchAppService _recurringBatchAppService;
        private readonly IBrokerService _brokerService;
        private readonly IAppCache _appCache;
        private readonly IEmployeeTypeAppService _employeeTypeAppService;

        public SalaryPeriodAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<SalaryPeriod> salaryPeriodRepository,
           IPostingPeriodAppService postingPeriodAppService,
           IChartOfAccountAppService chartOfAccountAppService,
           ICustomerAccountAppService customerAccountAppService,
           IJournalEntryPostingService journalEntryPostingService,
           ILoanProductAppService loanProductAppService,
           ISavingsProductAppService savingsProductAppService,
           IInvestmentProductAppService investmentProductAppService,
           ICommissionAppService commissionAppService,
           IStandingOrderAppService standingOrderAppService,
           ISalaryCardAppService salaryCardAppService,
           IPaySlipAppService paySlipAppService,
           ISqlCommandAppService sqlCommandAppService,
           IRecurringBatchAppService recurringBatchAppService,
           IBrokerService brokerService,
           IAppCache appCache,
           IEmployeeTypeAppService employeeTypeAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (salaryPeriodRepository == null)
                throw new ArgumentNullException(nameof(salaryPeriodRepository));

            if (postingPeriodAppService == null)
                throw new ArgumentNullException(nameof(postingPeriodAppService));

            if (chartOfAccountAppService == null)
                throw new ArgumentNullException(nameof(chartOfAccountAppService));

            if (customerAccountAppService == null)
                throw new ArgumentNullException(nameof(customerAccountAppService));

            if (journalEntryPostingService == null)
                throw new ArgumentNullException(nameof(journalEntryPostingService));

            if (loanProductAppService == null)
                throw new ArgumentNullException(nameof(loanProductAppService));

            if (savingsProductAppService == null)
                throw new ArgumentNullException(nameof(savingsProductAppService));

            if (investmentProductAppService == null)
                throw new ArgumentNullException(nameof(investmentProductAppService));

            if (commissionAppService == null)
                throw new ArgumentNullException(nameof(commissionAppService));

            if (standingOrderAppService == null)
                throw new ArgumentNullException(nameof(standingOrderAppService));

            if (salaryCardAppService == null)
                throw new ArgumentNullException(nameof(salaryCardAppService));

            if (paySlipAppService == null)
                throw new ArgumentNullException(nameof(paySlipAppService));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            if (recurringBatchAppService == null)
                throw new ArgumentNullException(nameof(recurringBatchAppService));

            if (brokerService == null)
                throw new ArgumentNullException(nameof(brokerService));

            if (appCache == null)
                throw new ArgumentNullException(nameof(appCache));

            if (employeeTypeAppService == null)
                throw new ArgumentNullException(nameof(employeeTypeAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _salaryPeriodRepository = salaryPeriodRepository;
            _postingPeriodAppService = postingPeriodAppService;
            _chartOfAccountAppService = chartOfAccountAppService;
            _customerAccountAppService = customerAccountAppService;
            _journalEntryPostingService = journalEntryPostingService;
            _loanProductAppService = loanProductAppService;
            _savingsProductAppService = savingsProductAppService;
            _investmentProductAppService = investmentProductAppService;
            _commissionAppService = commissionAppService;
            _standingOrderAppService = standingOrderAppService;
            _salaryCardAppService = salaryCardAppService;
            _paySlipAppService = paySlipAppService;
            _sqlCommandAppService = sqlCommandAppService;
            _recurringBatchAppService = recurringBatchAppService;
            _brokerService = brokerService;
            _appCache = appCache;
            _employeeTypeAppService = employeeTypeAppService;
        }

        public SalaryProcessingDTO AddNewSalaryPeriod(SalaryProcessingDTO salaryPeriodDTO, ServiceHeader serviceHeader)
        {
            if (salaryPeriodDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    // get the specification
                    var filter = SalaryPeriodSpecifications.SalaryPeriodWithPostingPeriodIdMonthAndEmployeeType(salaryPeriodDTO.PostingPeriodId, salaryPeriodDTO.Month, salaryPeriodDTO.EmployeeCategory);

                    ISpecification<SalaryPeriod> spec = filter;

                    //Query this criteria
                    var salaryPeriods = _salaryPeriodRepository.AllMatching(spec, serviceHeader);

                    if (salaryPeriods != null && salaryPeriods.Any(x => x.Status == (int)SalaryPeriodStatus.Closed || x.Status == (int)SalaryPeriodStatus.Open))
                    {
                        salaryPeriodDTO.ErrorMessageResult = string.Format("Sorry, but there is already an open/closed {0} payroll period for the month of {1}!", salaryPeriodDTO.EmployeeCategoryDescription, salaryPeriodDTO.MonthDescription);
                        return salaryPeriodDTO;
                    }
                    else
                    {
                        var salaryPeriod = SalaryPeriodFactory.CreateSalaryPeriod(salaryPeriodDTO.PostingPeriodId, salaryPeriodDTO.Month, salaryPeriodDTO.EmployeeCategory, salaryPeriodDTO.TaxReliefAmount, salaryPeriodDTO.MaximumProvidentFundReliefAmount, salaryPeriodDTO.MaximumInsuranceReliefAmount, salaryPeriodDTO.Remarks);

                        salaryPeriod.EnforceMonthValueDate = salaryPeriodDTO.EnforceMonthValueDate;
                        salaryPeriod.ExecutePayoutStandingOrders = salaryPeriodDTO.ExecutePayoutStandingOrders;
                        salaryPeriod.Status = (int)SalaryPeriodStatus.Open;
                        salaryPeriod.CreatedBy = serviceHeader.ApplicationUserName;

                        _salaryPeriodRepository.Add(salaryPeriod, serviceHeader);

                        dbContextScope.SaveChanges(serviceHeader);

                        return salaryPeriod.ProjectedAs<SalaryProcessingDTO>();
                    }
                }
            }
            else return null;
        }

        public bool UpdateSalaryPeriod(SalaryProcessingDTO salaryPeriodDTO, ServiceHeader serviceHeader)
        {
            if (salaryPeriodDTO == null || salaryPeriodDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _salaryPeriodRepository.Get(salaryPeriodDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    // get the specification
                    var filter = SalaryPeriodSpecifications.SalaryPeriodWithPostingPeriodIdMonthAndEmployeeType(salaryPeriodDTO.PostingPeriodId, salaryPeriodDTO.Month, salaryPeriodDTO.EmployeeCategory);

                    ISpecification<SalaryPeriod> spec = filter;

                    //Query this criteria
                    var salaryPeriods = _salaryPeriodRepository.AllMatching(spec, serviceHeader);

                    if (salaryPeriods != null && salaryPeriods.Any(x => x.Id != persisted.Id && x.Status == (int)SalaryPeriodStatus.Closed || x.Status == (int)SalaryPeriodStatus.Suspended))
                        throw new InvalidOperationException(string.Format("Sorry, but there is already an open/closed {0} payroll period for the month of {1}!", salaryPeriodDTO.EmployeeCategoryDescription, salaryPeriodDTO.MonthDescription));
                    else
                    {
                        var current = SalaryPeriodFactory.CreateSalaryPeriod(persisted.PostingPeriodId, salaryPeriodDTO.Month, salaryPeriodDTO.EmployeeCategory, salaryPeriodDTO.TaxReliefAmount, salaryPeriodDTO.MaximumProvidentFundReliefAmount, salaryPeriodDTO.MaximumInsuranceReliefAmount, salaryPeriodDTO.Remarks);

                        current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                        current.EnforceMonthValueDate = salaryPeriodDTO.EnforceMonthValueDate;
                        current.ExecutePayoutStandingOrders = salaryPeriodDTO.ExecutePayoutStandingOrders;
                        current.Status = persisted.Status;
                        current.CreatedBy = persisted.CreatedBy;

                        _salaryPeriodRepository.Merge(persisted, current, serviceHeader);

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                }
                else return false;
            }
        }

        public bool ProcessSalaryPeriod(SalaryProcessingDTO salaryPeriodDTO, List<EmployeeDTO> employees, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (salaryPeriodDTO != null)
            {
                var persistedSalaryPeriod = FindSalaryPeriod(salaryPeriodDTO.Id, serviceHeader);

                if (persistedSalaryPeriod != null && persistedSalaryPeriod.Status == (int)SalaryPeriodStatus.Open)
                {
                    result = _paySlipAppService.PurgePaySlips(salaryPeriodDTO, serviceHeader);

                    if (result)
                    {
                        var paySlipDTOs = new List<PaySlipDTO>();

                        if (employees != null)
                        {
                            employees.ForEach(employeeDTO =>
                            {
                                if (!employeeDTO.IsLocked)
                                {
                                    var salaryCardDTO = _salaryCardAppService.FindSalaryCardByEmployeeId(employeeDTO.Id, serviceHeader);

                                    if (salaryCardDTO != null)
                                    {
                                        var salaryCardEntries = _salaryCardAppService.FindSalaryCardEntriesBySalaryCardId(salaryCardDTO.Id, serviceHeader);

                                        var targetBasicPayEarningSalaryHeadType = SalaryHeadType.FullTimeBasicPayEarning;

                                        switch ((EmployeeCategory)employeeDTO.EmployeeTypeCategory)
                                        {
                                            case EmployeeCategory.FullTime:
                                                targetBasicPayEarningSalaryHeadType = SalaryHeadType.FullTimeBasicPayEarning;
                                                break;
                                            case EmployeeCategory.PartTime:
                                                targetBasicPayEarningSalaryHeadType = SalaryHeadType.PartTimeBasicPayEarning;
                                                break;
                                            case EmployeeCategory.Contract:
                                                targetBasicPayEarningSalaryHeadType = SalaryHeadType.ContractBasicPayEarning;
                                                break;
                                            default:
                                                break;
                                        }

                                        if (salaryCardEntries != null && salaryCardEntries.Any(x => x.SalaryGroupEntrySalaryHeadType == (int)targetBasicPayEarningSalaryHeadType)) /*BasicPayEarning entry must be present*/
                                        {
                                            #region Ensure employee has an account for each entry, if not create it

                                            foreach (var salaryCardEntryDTO in salaryCardEntries)
                                            {
                                                var customerAccountId = Guid.Empty;

                                                var customerAccounts = _sqlCommandAppService.FindCustomerAccountsByTargetProductIdAndCustomerId(salaryCardEntryDTO.SalaryGroupEntrySalaryHeadCustomerAccountTypeTargetProductId, employeeDTO.CustomerId, serviceHeader);

                                                if (customerAccounts != null && customerAccounts.Any())
                                                    customerAccountId = customerAccounts.First().Id;
                                                else
                                                {
                                                    var customerAccountDTO = new CustomerAccountDTO
                                                    {
                                                        BranchId = employeeDTO.BranchId,
                                                        CustomerId = employeeDTO.CustomerId,
                                                        CustomerAccountTypeProductCode = salaryCardEntryDTO.SalaryGroupEntrySalaryHeadCustomerAccountTypeProductCode,
                                                        CustomerAccountTypeTargetProductId = salaryCardEntryDTO.SalaryGroupEntrySalaryHeadCustomerAccountTypeTargetProductId,
                                                        CustomerAccountTypeTargetProductCode = salaryCardEntryDTO.SalaryGroupEntrySalaryHeadCustomerAccountTypeTargetProductCode
                                                    };

                                                    customerAccountDTO = _customerAccountAppService.AddNewCustomerAccount(customerAccountDTO, serviceHeader);

                                                    customerAccountId = (customerAccountDTO != null) ? customerAccountDTO.Id : Guid.Empty;
                                                }

                                                salaryCardEntryDTO.CustomerAccountId = customerAccountId;
                                            }

                                            #endregion

                                            var basicPayEarningEntry = salaryCardEntries.FirstOrDefault(x => x.SalaryGroupEntrySalaryHeadType == (int)targetBasicPayEarningSalaryHeadType);

                                            if (basicPayEarningEntry.ChargeFixedAmount > 0m)
                                            {
                                                var payslipDTO = new PaySlipDTO { SalaryPeriodId = persistedSalaryPeriod.Id, SalaryPeriodMonth = persistedSalaryPeriod.Month, SalaryCardId = salaryCardDTO.Id };

                                                #region collate pay slip entries

                                                foreach (var salaryCardEntryDTO in salaryCardEntries)
                                                {
                                                    switch ((SalaryHeadType)salaryCardEntryDTO.SalaryGroupEntrySalaryHeadType)
                                                    {
                                                        case SalaryHeadType.FullTimeBasicPayEarning:
                                                        case SalaryHeadType.PartTimeBasicPayEarning:
                                                        case SalaryHeadType.ContractBasicPayEarning:
                                                        case SalaryHeadType.NSSFDeduction:
                                                        case SalaryHeadType.NHIFDeduction:
                                                        case SalaryHeadType.PAYEDeduction:
                                                        case SalaryHeadType.StatutoryProvidentFundDeduction:

                                                            payslipDTO.PaySlipEntries.Add(new PaySlipEntryDTO
                                                            {
                                                                CustomerAccountId = salaryCardEntryDTO.CustomerAccountId,
                                                                ChartOfAccountId = salaryCardEntryDTO.SalaryGroupEntrySalaryHeadChartOfAccountId,
                                                                Description = salaryCardEntryDTO.SalaryGroupEntrySalaryHeadDescription,
                                                                SalaryHeadType = salaryCardEntryDTO.SalaryGroupEntrySalaryHeadType,
                                                                SalaryHeadCategory = salaryCardEntryDTO.SalaryGroupEntrySalaryHeadCategory,
                                                                SalaryCardEntryChargeType = salaryCardEntryDTO.ChargeType,
                                                                SalaryCardEntryChargeFixedAmount = salaryCardEntryDTO.ChargeFixedAmount,
                                                                SalaryCardEntryChargePercentage = salaryCardEntryDTO.ChargePercentage,
                                                                Principal = salaryCardEntryDTO.ChargeFixedAmount,
                                                                RoundingType = salaryCardEntryDTO.SalaryGroupEntryRoundingType,
                                                            });

                                                            break;

                                                        case SalaryHeadType.OtherDeduction:

                                                            var deductionValue = 0m;

                                                            switch ((ChargeType)salaryCardEntryDTO.ChargeType)
                                                            {
                                                                case ChargeType.Percentage:
                                                                    deductionValue = Convert.ToDecimal((salaryCardEntryDTO.ChargePercentage * Convert.ToDouble(basicPayEarningEntry.ChargeFixedAmount)) / 100);
                                                                    deductionValue = Math.Max(deductionValue, salaryCardEntryDTO.SalaryGroupEntryMinimumValue);
                                                                    break;
                                                                case ChargeType.FixedAmount:
                                                                    deductionValue = salaryCardEntryDTO.ChargeFixedAmount;
                                                                    deductionValue = Math.Max(deductionValue, salaryCardEntryDTO.SalaryGroupEntryMinimumValue);
                                                                    break;
                                                                default:
                                                                    break;
                                                            }

                                                            if (deductionValue > 0m)
                                                            {
                                                                switch ((RoundingType)salaryCardEntryDTO.SalaryGroupEntryRoundingType)
                                                                {
                                                                    case RoundingType.ToEven:
                                                                        deductionValue = Math.Round(deductionValue, MidpointRounding.ToEven);
                                                                        break;
                                                                    case RoundingType.AwayFromZero:
                                                                        deductionValue = Math.Round(deductionValue, MidpointRounding.AwayFromZero);
                                                                        break;
                                                                    case RoundingType.Ceiling:
                                                                        deductionValue = Math.Ceiling(deductionValue);
                                                                        break;
                                                                    case RoundingType.Floor:
                                                                        deductionValue = Math.Floor(deductionValue);
                                                                        break;
                                                                    default:
                                                                        break;
                                                                }
                                                            }

                                                            payslipDTO.PaySlipEntries.Add(new PaySlipEntryDTO
                                                            {
                                                                CustomerAccountId = salaryCardEntryDTO.CustomerAccountId,
                                                                ChartOfAccountId = salaryCardEntryDTO.SalaryGroupEntrySalaryHeadChartOfAccountId,
                                                                Description = salaryCardEntryDTO.SalaryGroupEntrySalaryHeadDescription,
                                                                SalaryHeadType = salaryCardEntryDTO.SalaryGroupEntrySalaryHeadType,
                                                                SalaryHeadCategory = salaryCardEntryDTO.SalaryGroupEntrySalaryHeadCategory,
                                                                SalaryCardEntryChargeType = salaryCardEntryDTO.ChargeType,
                                                                SalaryCardEntryChargeFixedAmount = salaryCardEntryDTO.ChargeFixedAmount,
                                                                SalaryCardEntryChargePercentage = salaryCardEntryDTO.ChargePercentage,
                                                                Principal = deductionValue,
                                                                RoundingType = salaryCardEntryDTO.SalaryGroupEntryRoundingType,
                                                            });

                                                            break;

                                                        case SalaryHeadType.OtherEarning:

                                                            var earningValue = 0m;

                                                            switch ((ChargeType)salaryCardEntryDTO.ChargeType)
                                                            {
                                                                case ChargeType.Percentage:
                                                                    earningValue = Convert.ToDecimal((salaryCardEntryDTO.ChargePercentage * Convert.ToDouble(basicPayEarningEntry.ChargeFixedAmount)) / 100);
                                                                    earningValue = Math.Max(earningValue, salaryCardEntryDTO.SalaryGroupEntryMinimumValue);
                                                                    break;
                                                                case ChargeType.FixedAmount:
                                                                    earningValue = salaryCardEntryDTO.ChargeFixedAmount;
                                                                    earningValue = Math.Max(earningValue, salaryCardEntryDTO.SalaryGroupEntryMinimumValue);
                                                                    break;
                                                                default:
                                                                    break;
                                                            }

                                                            if (earningValue > 0m)
                                                            {
                                                                switch ((RoundingType)salaryCardEntryDTO.SalaryGroupEntryRoundingType)
                                                                {
                                                                    case RoundingType.ToEven:
                                                                        earningValue = Math.Round(earningValue, MidpointRounding.ToEven);
                                                                        break;
                                                                    case RoundingType.AwayFromZero:
                                                                        earningValue = Math.Round(earningValue, MidpointRounding.AwayFromZero);
                                                                        break;
                                                                    case RoundingType.Ceiling:
                                                                        earningValue = Math.Ceiling(earningValue);
                                                                        break;
                                                                    case RoundingType.Floor:
                                                                        earningValue = Math.Floor(earningValue);
                                                                        break;
                                                                    default:
                                                                        break;
                                                                }
                                                            }

                                                            payslipDTO.PaySlipEntries.Add(new PaySlipEntryDTO
                                                            {
                                                                CustomerAccountId = salaryCardEntryDTO.CustomerAccountId,
                                                                ChartOfAccountId = salaryCardEntryDTO.SalaryGroupEntrySalaryHeadChartOfAccountId,
                                                                Description = salaryCardEntryDTO.SalaryGroupEntrySalaryHeadDescription,
                                                                SalaryHeadType = salaryCardEntryDTO.SalaryGroupEntrySalaryHeadType,
                                                                SalaryHeadCategory = salaryCardEntryDTO.SalaryGroupEntrySalaryHeadCategory,
                                                                SalaryCardEntryChargeType = salaryCardEntryDTO.ChargeType,
                                                                SalaryCardEntryChargeFixedAmount = salaryCardEntryDTO.ChargeFixedAmount,
                                                                SalaryCardEntryChargePercentage = salaryCardEntryDTO.ChargePercentage,
                                                                Principal = earningValue,
                                                                RoundingType = salaryCardEntryDTO.SalaryGroupEntryRoundingType,
                                                            });

                                                            break;

                                                        case SalaryHeadType.LoanDeduction:

                                                            var existingLoanStandingOrders = _standingOrderAppService.FindStandingOrders(basicPayEarningEntry.CustomerAccountId, salaryCardEntryDTO.CustomerAccountId, (int)StandingOrderTrigger.CheckOff, serviceHeader);

                                                            if (existingLoanStandingOrders != null && existingLoanStandingOrders.Any(x => !x.IsLocked))
                                                            {
                                                                var customerLoanAccount = _sqlCommandAppService.FindCustomerAccountById(salaryCardEntryDTO.CustomerAccountId, serviceHeader);

                                                                _customerAccountAppService.FetchCustomerAccountBalances(new List<CustomerAccountDTO> { customerLoanAccount }, serviceHeader, true);

                                                                if (customerLoanAccount.PrincipalBalance * -1 > 0) // Check that we have a principal balance..
                                                                {
                                                                    var expectedPrincipal = Math.Min(Math.Abs(customerLoanAccount.PrincipalBalance), existingLoanStandingOrders.Where(x => !x.IsLocked).Sum(x => x.Principal));

                                                                    var expectedInterest = Math.Min(Math.Abs(customerLoanAccount.InterestBalance), existingLoanStandingOrders.Where(x => !x.IsLocked).Sum(x => x.Interest));

                                                                    payslipDTO.PaySlipEntries.Add(new PaySlipEntryDTO
                                                                    {
                                                                        CustomerAccountId = salaryCardEntryDTO.CustomerAccountId,
                                                                        ChartOfAccountId = salaryCardEntryDTO.SalaryGroupEntrySalaryHeadChartOfAccountId,
                                                                        Description = salaryCardEntryDTO.SalaryGroupEntrySalaryHeadDescription,
                                                                        SalaryHeadType = salaryCardEntryDTO.SalaryGroupEntrySalaryHeadType,
                                                                        SalaryHeadCategory = salaryCardEntryDTO.SalaryGroupEntrySalaryHeadCategory,
                                                                        SalaryCardEntryChargeType = salaryCardEntryDTO.ChargeType,
                                                                        SalaryCardEntryChargeFixedAmount = salaryCardEntryDTO.ChargeFixedAmount,
                                                                        SalaryCardEntryChargePercentage = salaryCardEntryDTO.ChargePercentage,
                                                                        Principal = expectedPrincipal,
                                                                        Interest = expectedInterest,
                                                                        RoundingType = salaryCardEntryDTO.SalaryGroupEntryRoundingType,
                                                                    });
                                                                }
                                                            }

                                                            break;
                                                        case SalaryHeadType.InvestmentDeduction:

                                                            var existingInvestmentStandingOrders = _standingOrderAppService.FindStandingOrders(basicPayEarningEntry.CustomerAccountId, salaryCardEntryDTO.CustomerAccountId, (int)StandingOrderTrigger.CheckOff, serviceHeader);

                                                            if (existingInvestmentStandingOrders != null && existingInvestmentStandingOrders.Any(x => !x.IsLocked))
                                                            {
                                                                foreach (var investmentStandingOrder in existingInvestmentStandingOrders)
                                                                {
                                                                    if (investmentStandingOrder.IsLocked)
                                                                        continue;

                                                                    switch ((ChargeType)investmentStandingOrder.ChargeType)
                                                                    {
                                                                        case ChargeType.FixedAmount:

                                                                            payslipDTO.PaySlipEntries.Add(new PaySlipEntryDTO
                                                                            {
                                                                                CustomerAccountId = salaryCardEntryDTO.CustomerAccountId,
                                                                                ChartOfAccountId = salaryCardEntryDTO.SalaryGroupEntrySalaryHeadChartOfAccountId,
                                                                                Description = salaryCardEntryDTO.SalaryGroupEntrySalaryHeadDescription,
                                                                                SalaryHeadType = salaryCardEntryDTO.SalaryGroupEntrySalaryHeadType,
                                                                                SalaryHeadCategory = salaryCardEntryDTO.SalaryGroupEntrySalaryHeadCategory,
                                                                                SalaryCardEntryChargeType = salaryCardEntryDTO.ChargeType,
                                                                                SalaryCardEntryChargeFixedAmount = salaryCardEntryDTO.ChargeFixedAmount,
                                                                                SalaryCardEntryChargePercentage = salaryCardEntryDTO.ChargePercentage,
                                                                                Principal = investmentStandingOrder.ChargeFixedAmount,
                                                                                RoundingType = salaryCardEntryDTO.SalaryGroupEntryRoundingType,
                                                                            });

                                                                            break;

                                                                        case ChargeType.Percentage:

                                                                            payslipDTO.PaySlipEntries.Add(new PaySlipEntryDTO
                                                                            {
                                                                                CustomerAccountId = salaryCardEntryDTO.CustomerAccountId,
                                                                                ChartOfAccountId = salaryCardEntryDTO.SalaryGroupEntrySalaryHeadChartOfAccountId,
                                                                                Description = salaryCardEntryDTO.SalaryGroupEntrySalaryHeadDescription,
                                                                                SalaryHeadType = salaryCardEntryDTO.SalaryGroupEntrySalaryHeadType,
                                                                                SalaryHeadCategory = salaryCardEntryDTO.SalaryGroupEntrySalaryHeadCategory,
                                                                                SalaryCardEntryChargeType = salaryCardEntryDTO.ChargeType,
                                                                                SalaryCardEntryChargeFixedAmount = salaryCardEntryDTO.ChargeFixedAmount,
                                                                                SalaryCardEntryChargePercentage = salaryCardEntryDTO.ChargePercentage,
                                                                                Principal = Convert.ToDecimal((investmentStandingOrder.ChargePercentage * Convert.ToDouble(basicPayEarningEntry.ChargeFixedAmount)) / 100),
                                                                                RoundingType = salaryCardEntryDTO.SalaryGroupEntryRoundingType,
                                                                            });

                                                                            break;

                                                                        default:
                                                                            break;
                                                                    }
                                                                }
                                                            }

                                                            break;

                                                        case SalaryHeadType.VoluntaryProvidentFundDeduction:

                                                            var voluntaryProvidentFundDeductionValue = 0m;

                                                            switch ((ChargeType)salaryCardEntryDTO.ChargeType)
                                                            {
                                                                case ChargeType.Percentage:
                                                                    voluntaryProvidentFundDeductionValue = Convert.ToDecimal((salaryCardEntryDTO.ChargePercentage * Convert.ToDouble(basicPayEarningEntry.ChargeFixedAmount)) / 100);
                                                                    voluntaryProvidentFundDeductionValue = Math.Max(voluntaryProvidentFundDeductionValue, salaryCardEntryDTO.SalaryGroupEntryMinimumValue);
                                                                    break;
                                                                case ChargeType.FixedAmount:
                                                                    voluntaryProvidentFundDeductionValue = salaryCardEntryDTO.ChargeFixedAmount;
                                                                    voluntaryProvidentFundDeductionValue = Math.Max(voluntaryProvidentFundDeductionValue, salaryCardEntryDTO.SalaryGroupEntryMinimumValue);
                                                                    break;
                                                                default:
                                                                    break;
                                                            }

                                                            if (voluntaryProvidentFundDeductionValue > 0m)
                                                            {
                                                                switch ((RoundingType)salaryCardEntryDTO.SalaryGroupEntryRoundingType)
                                                                {
                                                                    case RoundingType.ToEven:
                                                                        voluntaryProvidentFundDeductionValue = Math.Round(voluntaryProvidentFundDeductionValue, MidpointRounding.ToEven);
                                                                        break;
                                                                    case RoundingType.AwayFromZero:
                                                                        voluntaryProvidentFundDeductionValue = Math.Round(voluntaryProvidentFundDeductionValue, MidpointRounding.AwayFromZero);
                                                                        break;
                                                                    case RoundingType.Ceiling:
                                                                        voluntaryProvidentFundDeductionValue = Math.Ceiling(voluntaryProvidentFundDeductionValue);
                                                                        break;
                                                                    case RoundingType.Floor:
                                                                        voluntaryProvidentFundDeductionValue = Math.Floor(voluntaryProvidentFundDeductionValue);
                                                                        break;
                                                                    default:
                                                                        break;
                                                                }
                                                            }

                                                            payslipDTO.PaySlipEntries.Add(new PaySlipEntryDTO
                                                            {
                                                                CustomerAccountId = salaryCardEntryDTO.CustomerAccountId,
                                                                ChartOfAccountId = salaryCardEntryDTO.SalaryGroupEntrySalaryHeadChartOfAccountId,
                                                                Description = salaryCardEntryDTO.SalaryGroupEntrySalaryHeadDescription,
                                                                SalaryHeadType = salaryCardEntryDTO.SalaryGroupEntrySalaryHeadType,
                                                                SalaryHeadCategory = salaryCardEntryDTO.SalaryGroupEntrySalaryHeadCategory,
                                                                SalaryCardEntryChargeType = salaryCardEntryDTO.ChargeType,
                                                                SalaryCardEntryChargeFixedAmount = salaryCardEntryDTO.ChargeFixedAmount,
                                                                SalaryCardEntryChargePercentage = salaryCardEntryDTO.ChargePercentage,
                                                                Principal = voluntaryProvidentFundDeductionValue,
                                                                RoundingType = salaryCardEntryDTO.SalaryGroupEntryRoundingType,
                                                            });

                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                }

                                                #endregion

                                                #region compute statutory deductions

                                                if (employeeDTO.EmployeeTypeCategory.In((int)EmployeeCategory.FullTime, (int)EmployeeCategory.Contract))
                                                {
                                                    if (payslipDTO.PaySlipEntries.Any())
                                                    {
                                                        #region NSSF,NHIF,Provident Fund

                                                        var basicPay = payslipDTO.PaySlipEntries.Where(x => x.SalaryHeadType == (int)targetBasicPayEarningSalaryHeadType).Sum(x => x.Principal);

                                                        var voluntaryProvidentFundAmount = payslipDTO.PaySlipEntries.Where(x => x.SalaryHeadType == (int)SalaryHeadType.VoluntaryProvidentFundDeduction).Sum(x => x.Principal);

                                                        var nssfAmount = 0m;

                                                        var statutoryProvidentFundAmount = 0m;

                                                        foreach (var payslipEntry in payslipDTO.PaySlipEntries)
                                                        {
                                                            switch ((SalaryHeadType)payslipEntry.SalaryHeadType)
                                                            {
                                                                case SalaryHeadType.NSSFDeduction:

                                                                    nssfAmount = ComputeStatutoryCharges(payslipEntry, basicPay, serviceHeader);

                                                                    payslipEntry.Principal = nssfAmount;

                                                                    break;
                                                                case SalaryHeadType.NHIFDeduction:

                                                                    var totalEarnings = payslipDTO.PaySlipEntries.Where(x => x.SalaryHeadType == (int)SalaryHeadType.OtherEarning).Sum(x => x.Principal);

                                                                    payslipEntry.Principal = ComputeStatutoryCharges(payslipEntry, (basicPay + totalEarnings), serviceHeader);

                                                                    break;
                                                                case SalaryHeadType.StatutoryProvidentFundDeduction:

                                                                    statutoryProvidentFundAmount = ComputeStatutoryCharges(payslipEntry, basicPay, serviceHeader);

                                                                    payslipEntry.Principal = statutoryProvidentFundAmount;

                                                                    break;
                                                                default:
                                                                    break;
                                                            }
                                                        }

                                                        #endregion

                                                        #region PAYE

                                                        foreach (var payslipEntry in payslipDTO.PaySlipEntries)
                                                        {
                                                            switch ((SalaryHeadType)payslipEntry.SalaryHeadType)
                                                            {
                                                                case SalaryHeadType.PAYEDeduction:

                                                                    var grossPay = payslipDTO.PaySlipEntries.Where(x => x.SalaryHeadType == (int)targetBasicPayEarningSalaryHeadType || x.SalaryHeadType == (int)SalaryHeadType.OtherEarning).Sum(x => x.Principal);

                                                                    var taxablePay = 0m;

                                                                    if ((statutoryProvidentFundAmount + voluntaryProvidentFundAmount + nssfAmount) >= salaryPeriodDTO.MaximumProvidentFundReliefAmount)
                                                                        taxablePay = grossPay - salaryPeriodDTO.MaximumProvidentFundReliefAmount;
                                                                    else taxablePay = grossPay - (nssfAmount + Math.Min((statutoryProvidentFundAmount + voluntaryProvidentFundAmount), salaryPeriodDTO.MaximumProvidentFundReliefAmount));

                                                                    if (salaryCardDTO.IsTaxExempt)
                                                                    {
                                                                        taxablePay = taxablePay - Math.Abs(salaryCardDTO.TaxExemption);
                                                                    }

                                                                    if (taxablePay > 0m)
                                                                    {
                                                                        var taxValue = ComputeStatutoryCharges(payslipEntry, taxablePay, serviceHeader);

                                                                        var netTax = taxValue - (salaryPeriodDTO.TaxReliefAmount + Math.Min(salaryCardDTO.InsuranceReliefAmount, salaryPeriodDTO.MaximumInsuranceReliefAmount));

                                                                        if ((netTax * -1) < 0m) // net tax must be non-negative
                                                                        {
                                                                            payslipEntry.Principal = netTax;
                                                                        }
                                                                    }

                                                                    break;
                                                                default:
                                                                    break;
                                                            }
                                                        }

                                                        #endregion
                                                    }
                                                }

                                                #endregion

                                                paySlipDTOs.Add(payslipDTO);
                                            }
                                        }
                                    }
                                }
                            });
                        }

                        if (paySlipDTOs.Any())
                        {
                            result = _paySlipAppService.AddNewPaySlips(paySlipDTOs, serviceHeader);
                        }
                    }
                }
            }

            return result;
        }

        public bool CloseSalaryPeriod(SalaryProcessingDTO salaryPeriodDTO, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (salaryPeriodDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _salaryPeriodRepository.Get(salaryPeriodDTO.Id, serviceHeader);

                    if (persisted == null || persisted.Status != (int)SalaryPeriodStatus.Open)
                        return result;

                    persisted.Status = (int)SalaryPeriodStatus.Closed;
                    persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                    persisted.AuthorizedDate = DateTime.Now;

                    result = dbContextScope.SaveChanges(serviceHeader) >= 0;

                    if (result)
                    {
                        var query = _salaryPeriodRepository.DatabaseSqlQuery<Guid>(string.Format(
                             @"SELECT Id
                            FROM  {0}PaySlips
                            WHERE(SalaryPeriodId = @SalaryPeriodId)", DefaultSettings.Instance.TablePrefix), serviceHeader,
                               new SqlParameter("SalaryPeriodId", salaryPeriodDTO.Id));

                        if (query != null)
                        {
                            var data = from l in query
                                       select new PaySlipDTO
                                       {
                                           Id = l,
                                       };

                            _brokerService.ProcessPaySlips(DMLCommand.None, serviceHeader, data.ToArray());
                        }
                    }
                }
            }

            return result;
        }

        public bool PostPaySlip(Guid paySlipId, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var pendingPaySlip = _paySlipAppService.FindPaySlip(paySlipId, serviceHeader);
            if (pendingPaySlip == null)
                return result;

            var employeeType = _employeeTypeAppService.FindEmployeeType(pendingPaySlip.SalaryCardEmployeeEmployeeTypeId, serviceHeader);
            if (employeeType == null)
                throw new InvalidOperationException("The payslip employee type could not be found.");
            if (employeeType.IsLocked)
                throw new InvalidOperationException("The payslip employee type is locked and cannot be used for payroll posting.");

            var payrollControlAccount = _chartOfAccountAppService.FindChartOfAccount(employeeType.ChartOfAccountId, serviceHeader);
            if (payrollControlAccount == null || payrollControlAccount.IsLocked || payrollControlAccount.AccountCategory != (int)ChartOfAccountCategory.DetailAccount)
                throw new InvalidOperationException("The employee type payroll control account is missing, locked, or non-postable.");

            if (_paySlipAppService.MarkPaySlipPosted(paySlipId, serviceHeader))
            {
                var paySlipDTO = _paySlipAppService.FindPaySlip(paySlipId, serviceHeader);
                if (paySlipDTO == null || paySlipDTO.Status != (int)BatchEntryStatus.Posted)
                    return result;

                var salaryPeriodDTO = FindCachedSalaryPeriod(paySlipDTO.SalaryPeriodId, serviceHeader);
                if (salaryPeriodDTO == null)
                    return result;

                var postingPeriodDTO = _postingPeriodAppService.FindPostingPeriod(paySlipDTO.SalaryPeriodPostingPeriodId, serviceHeader) ?? _postingPeriodAppService.FindCachedCurrentPostingPeriod(serviceHeader);
                if (postingPeriodDTO == null)
                    return result;

                serviceHeader.ApplicationUserName = salaryPeriodDTO.AuthorizedBy ?? serviceHeader.ApplicationUserName;

                var paySlipEntries = _paySlipAppService.FindPaySlipEntriesByPaySlipId(paySlipDTO.Id, serviceHeader);

                var targetBasicPayEarningSalaryHeadType = SalaryHeadType.FullTimeBasicPayEarning;

                switch ((EmployeeCategory)paySlipDTO.SalaryCardEmployeeEmployeeTypeCategory)
                {
                    case EmployeeCategory.FullTime:
                        targetBasicPayEarningSalaryHeadType = SalaryHeadType.FullTimeBasicPayEarning;
                        break;
                    case EmployeeCategory.PartTime:
                        targetBasicPayEarningSalaryHeadType = SalaryHeadType.PartTimeBasicPayEarning;
                        break;
                    case EmployeeCategory.Contract:
                        targetBasicPayEarningSalaryHeadType = SalaryHeadType.ContractBasicPayEarning;
                        break;
                    default:
                        break;
                }

                if (paySlipEntries != null && paySlipEntries.Any((x => x.SalaryHeadType == (int)targetBasicPayEarningSalaryHeadType))) /*BasicPayEarning entry must be present*/
                {
                    var basicPayEarningEntry = paySlipEntries.FirstOrDefault(x => x.SalaryHeadType == (int)targetBasicPayEarningSalaryHeadType);

                    var payrollSavingsAccount = _sqlCommandAppService.FindCustomerAccountById(basicPayEarningEntry.CustomerAccountId, serviceHeader);

                    _customerAccountAppService.FetchCustomerAccountBalances(new List<CustomerAccountDTO> { payrollSavingsAccount }, serviceHeader);

                    var payrollSavingsProduct = _savingsProductAppService.FindCachedSavingsProduct(payrollSavingsAccount.CustomerAccountTypeTargetProductId, payrollSavingsAccount.BranchId, serviceHeader);

                    Guid? parentJournalId = null;

                    var journals = new List<Journal>();

                    #region Post Salary Heads

                    paySlipEntries.ForEach(paySlipEntryDTO =>
                    {
                        switch ((SalaryHeadType)paySlipEntryDTO.SalaryHeadType)
                        {
                            case SalaryHeadType.FullTimeBasicPayEarning:
                            case SalaryHeadType.PartTimeBasicPayEarning:
                            case SalaryHeadType.ContractBasicPayEarning:
                                // Earning Jounrnal: Credit PaySlipDTO.SalaryCardEmployeeEmployeeTypeChartOfAccountId, Debit PaySlipEntryDTO.ChartOfAccountId 
                                var basicPayEarningJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, paySlipDTO.SalaryCardEmployeeBranchId, null, paySlipEntryDTO.Principal, paySlipEntryDTO.Description, paySlipEntryDTO.SalaryHeadCategoryDescription, salaryPeriodDTO.Remarks, moduleNavigationItemCode, (int)SystemTransactionCode.SalaryProcessing, UberUtil.GetLastDayOfMonth(salaryPeriodDTO.Month, postingPeriodDTO.DurationEndDate.Year, salaryPeriodDTO.EnforceMonthValueDate), serviceHeader, true);
                                _journalEntryPostingService.PerformDoubleEntry(basicPayEarningJournal, paySlipDTO.SalaryCardEmployeeEmployeeTypeChartOfAccountId, paySlipEntryDTO.ChartOfAccountId, payrollSavingsAccount, payrollSavingsAccount, serviceHeader);
                                journals.Add(basicPayEarningJournal);
                                parentJournalId = basicPayEarningJournal.Id;
                                break;
                            case SalaryHeadType.OtherEarning:

                                // Earning Jounrnal: Credit PaySlipDTO.SalaryCardEmployeeEmployeeTypeChartOfAccountId, Debit PaySlipEntryDTO.ChartOfAccountId 
                                var otherEarningJournal = JournalFactory.CreateJournal(parentJournalId, postingPeriodDTO.Id, paySlipDTO.SalaryCardEmployeeBranchId, null, paySlipEntryDTO.Principal, paySlipEntryDTO.Description, paySlipEntryDTO.SalaryHeadCategoryDescription, salaryPeriodDTO.Remarks, moduleNavigationItemCode, (int)SystemTransactionCode.SalaryProcessing, UberUtil.GetLastDayOfMonth(salaryPeriodDTO.Month, postingPeriodDTO.DurationEndDate.Year, salaryPeriodDTO.EnforceMonthValueDate), serviceHeader);
                                _journalEntryPostingService.PerformDoubleEntry(otherEarningJournal, paySlipDTO.SalaryCardEmployeeEmployeeTypeChartOfAccountId, paySlipEntryDTO.ChartOfAccountId, payrollSavingsAccount, payrollSavingsAccount, serviceHeader);
                                journals.Add(otherEarningJournal);

                                break;
                            case SalaryHeadType.NSSFDeduction:
                            case SalaryHeadType.NHIFDeduction:
                            case SalaryHeadType.PAYEDeduction:
                            case SalaryHeadType.StatutoryProvidentFundDeduction:
                            case SalaryHeadType.OtherDeduction:
                            case SalaryHeadType.VoluntaryProvidentFundDeduction:

                                // Deduction Journal: Credit PaySlipEntryDTO.ChartOfAccountId, Debit PaySlipDTO.SalaryCardEmployeeEmployeeTypeChartOfAccountId
                                var deductionJournal = JournalFactory.CreateJournal(parentJournalId, postingPeriodDTO.Id, paySlipDTO.SalaryCardEmployeeBranchId, null, paySlipEntryDTO.Principal, paySlipEntryDTO.Description, paySlipEntryDTO.SalaryHeadCategoryDescription, salaryPeriodDTO.Remarks, moduleNavigationItemCode, (int)SystemTransactionCode.SalaryProcessing, UberUtil.GetLastDayOfMonth(salaryPeriodDTO.Month, postingPeriodDTO.DurationEndDate.Year, salaryPeriodDTO.EnforceMonthValueDate), serviceHeader);
                                _journalEntryPostingService.PerformDoubleEntry(deductionJournal, paySlipEntryDTO.ChartOfAccountId, paySlipDTO.SalaryCardEmployeeEmployeeTypeChartOfAccountId, payrollSavingsAccount, payrollSavingsAccount, serviceHeader);
                                journals.Add(deductionJournal);

                                if (paySlipDTO.SalaryCardEmployeeEmployeeTypeCategory.In((int)EmployeeCategory.FullTime, (int)EmployeeCategory.Contract))
                                {
                                    var basicPay = paySlipEntries.Where(x => x.SalaryHeadType == (int)targetBasicPayEarningSalaryHeadType).Sum(x => x.Principal);

                                    if (paySlipEntryDTO.SalaryHeadType.In((int)SalaryHeadType.NSSFDeduction))
                                    {
                                        // compute employer's contribution

                                        var employerNSSFContributionChartOfAccountId = _chartOfAccountAppService.GetCachedChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.EmployerNSSFContribution, serviceHeader);

                                        if (employerNSSFContributionChartOfAccountId != Guid.Empty)
                                        {
                                            var employerNSSFAmount = ComputeComplementaryStatutoryCharges(paySlipEntryDTO, basicPay, serviceHeader);

                                            // Deduction Journal: Credit PaySlipEntryDTO.ChartOfAccountId, Debit SystemGeneralLedgerAccountCode.NSSFEmployerContribution
                                            var employerNSSFContributionJournal = JournalFactory.CreateJournal(parentJournalId, postingPeriodDTO.Id, paySlipDTO.SalaryCardEmployeeBranchId, null, employerNSSFAmount, paySlipEntryDTO.Description, paySlipEntryDTO.SalaryHeadCategoryDescription, salaryPeriodDTO.Remarks, moduleNavigationItemCode, (int)SystemTransactionCode.SalaryProcessing, UberUtil.GetLastDayOfMonth(salaryPeriodDTO.Month, postingPeriodDTO.DurationEndDate.Year, salaryPeriodDTO.EnforceMonthValueDate), serviceHeader);
                                            _journalEntryPostingService.PerformDoubleEntry(employerNSSFContributionJournal, paySlipEntryDTO.ChartOfAccountId, employerNSSFContributionChartOfAccountId, payrollSavingsAccount, payrollSavingsAccount, serviceHeader);
                                            journals.Add(employerNSSFContributionJournal);
                                        }
                                    }
                                    else if (paySlipEntryDTO.SalaryHeadType.In((int)SalaryHeadType.StatutoryProvidentFundDeduction))
                                    {
                                        // compute employer's contribution
                                        var employerProvidentFundContributionChartOfAccountId = _chartOfAccountAppService.GetCachedChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.EmployerProvidentFundContribution, serviceHeader);

                                        if (employerProvidentFundContributionChartOfAccountId != Guid.Empty)
                                        {
                                            var employerProvidentFundAmount = ComputeComplementaryStatutoryCharges(paySlipEntryDTO, basicPay, serviceHeader);

                                            // Deduction Journal: Credit PaySlipEntryDTO.ChartOfAccountId, Debit SystemGeneralLedgerAccountCode.ProvidentFundEmployerContribution
                                            var employerProvidentFundContributionJournal = JournalFactory.CreateJournal(parentJournalId, postingPeriodDTO.Id, paySlipDTO.SalaryCardEmployeeBranchId, null, employerProvidentFundAmount, paySlipEntryDTO.Description, paySlipEntryDTO.SalaryHeadCategoryDescription, salaryPeriodDTO.Remarks, moduleNavigationItemCode, (int)SystemTransactionCode.SalaryProcessing, UberUtil.GetLastDayOfMonth(salaryPeriodDTO.Month, postingPeriodDTO.DurationEndDate.Year, salaryPeriodDTO.EnforceMonthValueDate), serviceHeader);
                                            _journalEntryPostingService.PerformDoubleEntry(employerProvidentFundContributionJournal, paySlipEntryDTO.ChartOfAccountId, employerProvidentFundContributionChartOfAccountId, payrollSavingsAccount, payrollSavingsAccount, serviceHeader);
                                            journals.Add(employerProvidentFundContributionJournal);
                                        }
                                    }
                                }

                                break;
                            case SalaryHeadType.InvestmentDeduction:

                                var investmentAccount = _sqlCommandAppService.FindCustomerAccountById(paySlipEntryDTO.CustomerAccountId, serviceHeader);

                                var investmentProduct = _investmentProductAppService.FindCachedInvestmentProduct(investmentAccount.CustomerAccountTypeTargetProductId, serviceHeader);

                                // Credit InvestmentProduct.ChartOfAccountId, Debit PaySlipDTO.SalaryCardEmployeeEmployeeTypeChartOfAccountId
                                var investmentJournal = JournalFactory.CreateJournal(parentJournalId, postingPeriodDTO.Id, paySlipDTO.SalaryCardEmployeeBranchId, null, paySlipEntryDTO.Principal, string.Format("Investment~{0}", investmentProduct.Description), paySlipEntryDTO.Description, salaryPeriodDTO.Remarks, moduleNavigationItemCode, (int)SystemTransactionCode.SalaryProcessing, UberUtil.GetLastDayOfMonth(salaryPeriodDTO.Month, postingPeriodDTO.DurationEndDate.Year, salaryPeriodDTO.EnforceMonthValueDate), serviceHeader);
                                _journalEntryPostingService.PerformDoubleEntry(investmentJournal, investmentProduct.ChartOfAccountId, paySlipDTO.SalaryCardEmployeeEmployeeTypeChartOfAccountId, investmentAccount, investmentAccount, serviceHeader);
                                journals.Add(investmentJournal);

                                break;
                            case SalaryHeadType.LoanDeduction:

                                var loanAccount = _sqlCommandAppService.FindCustomerAccountById(paySlipEntryDTO.CustomerAccountId, serviceHeader);

                                var loanProduct = _loanProductAppService.FindCachedLoanProduct(loanAccount.CustomerAccountTypeTargetProductId, serviceHeader);

                                // Credit LoanProduct.InterestReceivableChartOfAccountId, Debit PaySlipDTO.SalaryCardEmployeeEmployeeTypeChartOfAccountId
                                var loanInterestReceivableJournal = JournalFactory.CreateJournal(parentJournalId, postingPeriodDTO.Id, paySlipDTO.SalaryCardEmployeeBranchId, null, paySlipEntryDTO.Interest, string.Format("Interest Paid~{0}", loanProduct.Description), paySlipEntryDTO.Description, salaryPeriodDTO.Remarks, moduleNavigationItemCode, (int)SystemTransactionCode.SalaryProcessing, UberUtil.GetLastDayOfMonth(salaryPeriodDTO.Month, postingPeriodDTO.DurationEndDate.Year, salaryPeriodDTO.EnforceMonthValueDate), serviceHeader);
                                _journalEntryPostingService.PerformDoubleEntry(loanInterestReceivableJournal, loanProduct.InterestReceivableChartOfAccountId, paySlipDTO.SalaryCardEmployeeEmployeeTypeChartOfAccountId, loanAccount, loanAccount, serviceHeader);
                                journals.Add(loanInterestReceivableJournal);

                                // Credit LoanProduct.InterestReceivedChartOfAccountId, Debit LoanProduct.InterestChargedChartOfAccountId
                                var loanInterestReceivedJournal = JournalFactory.CreateJournal(parentJournalId, postingPeriodDTO.Id, paySlipDTO.SalaryCardEmployeeBranchId, null, paySlipEntryDTO.Interest, string.Format("Interest Paid~{0}", loanProduct.Description), paySlipEntryDTO.Description, salaryPeriodDTO.Remarks, moduleNavigationItemCode, (int)SystemTransactionCode.SalaryProcessing, UberUtil.GetLastDayOfMonth(salaryPeriodDTO.Month, postingPeriodDTO.DurationEndDate.Year, salaryPeriodDTO.EnforceMonthValueDate), serviceHeader);
                                _journalEntryPostingService.PerformDoubleEntry(loanInterestReceivedJournal, loanProduct.InterestReceivedChartOfAccountId, loanProduct.InterestChargedChartOfAccountId, loanAccount, loanAccount, serviceHeader);
                                journals.Add(loanInterestReceivedJournal);

                                // Credit LoanProduct.ChartOfAccountId, Debit PaySlipDTO.SalaryCardEmployeeEmployeeTypeChartOfAccountId
                                var loanPrincipalJournal = JournalFactory.CreateJournal(parentJournalId, postingPeriodDTO.Id, paySlipDTO.SalaryCardEmployeeBranchId, null, paySlipEntryDTO.Principal, string.Format("Principal Paid~{0}", loanProduct.Description), paySlipEntryDTO.Description, salaryPeriodDTO.Remarks, moduleNavigationItemCode, (int)SystemTransactionCode.SalaryProcessing, UberUtil.GetLastDayOfMonth(salaryPeriodDTO.Month, postingPeriodDTO.DurationEndDate.Year, salaryPeriodDTO.EnforceMonthValueDate), serviceHeader);
                                _journalEntryPostingService.PerformDoubleEntry(loanPrincipalJournal, loanProduct.ChartOfAccountId, paySlipDTO.SalaryCardEmployeeEmployeeTypeChartOfAccountId, loanAccount, loanAccount, serviceHeader);
                                journals.Add(loanPrincipalJournal);

                                break;
                            default:
                                break;
                        }
                    });

                    #endregion

                    #region Post Net Pay

                    var totalEarnings = paySlipEntries.Where(x => x.SalaryHeadCategory == (int)SalaryHeadCategory.Earning).Sum(x => x.Principal + x.Interest);

                    var totalDeductions = paySlipEntries.Where(x => x.SalaryHeadCategory == (int)SalaryHeadCategory.Deduction).Sum(x => x.Principal + x.Interest);

                    var netPay = totalEarnings - totalDeductions;

                    var payrollProcessingTariffs = _commissionAppService.ComputeTariffsBySavingsProduct(payrollSavingsProduct.Id, (int)SavingsProductKnownChargeType.PayrollProcessingCharges, netPay, payrollSavingsAccount, serviceHeader);

                    // Net Pay Journal: Credit SavingsProduct.ChartOfAccountId, Debit PaySlipDTO.SalaryCardEmployeeEmployeeTypeChartOfAccountId
                    var netPayJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, paySlipDTO.SalaryCardEmployeeBranchId, null, netPay, salaryPeriodDTO.Remarks, salaryPeriodDTO.MonthDescription, salaryPeriodDTO.PostingPeriodDescription, moduleNavigationItemCode, (int)SystemTransactionCode.SalaryProcessing, UberUtil.GetLastDayOfMonth(salaryPeriodDTO.Month, postingPeriodDTO.DurationEndDate.Year, salaryPeriodDTO.EnforceMonthValueDate), serviceHeader);
                    _journalEntryPostingService.PerformDoubleEntry(netPayJournal, payrollSavingsProduct.ChartOfAccountId, paySlipDTO.SalaryCardEmployeeEmployeeTypeChartOfAccountId, payrollSavingsAccount, payrollSavingsAccount, serviceHeader);
                    journals.Add(netPayJournal);

                    payrollProcessingTariffs.ForEach(tariff =>
                    {
                        // Tariff Journal: Credit Tariff.CreditGLAccountId, Debit Tariff.DebitGLAccountId
                        var tariffJournal = JournalFactory.CreateJournal(parentJournalId, postingPeriodDTO.Id, paySlipDTO.SalaryCardEmployeeBranchId, null, tariff.Amount, tariff.Description, salaryPeriodDTO.MonthDescription, salaryPeriodDTO.Remarks, moduleNavigationItemCode, (int)SystemTransactionCode.SalaryProcessing, UberUtil.GetLastDayOfMonth(salaryPeriodDTO.Month, postingPeriodDTO.DurationEndDate.Year, salaryPeriodDTO.EnforceMonthValueDate), serviceHeader);
                        _journalEntryPostingService.PerformDoubleEntry(tariffJournal, tariff.CreditGLAccountId, tariff.DebitGLAccountId, payrollSavingsAccount, payrollSavingsAccount, serviceHeader);
                        journals.Add(tariffJournal);
                    });

                    #endregion

                    #region Bulk-Insert journals && journal entries

                    if (journals.Any())
                    {
                        result = _journalEntryPostingService.BulkSave(serviceHeader, journals);

                        if (result)
                        {
                            #region Zeroize one-off earnings?

                            _salaryCardAppService.ZeroizeOneOffEarnings(paySlipDTO.SalaryCardId, serviceHeader);

                            #endregion

                            #region Execute payout standing orders?

                            if (salaryPeriodDTO.ExecutePayoutStandingOrders)
                                _recurringBatchAppService.ExecutePayoutStandingOrders(payrollSavingsAccount.Id, salaryPeriodDTO.Month, (int)QueuePriority.High, serviceHeader);

                            #endregion
                        }
                    }

                    #endregion
                }
            }

            return result;
        }

        public List<SalaryProcessingDTO> FindSalaryPeriods(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var salaryPeriods = _salaryPeriodRepository.GetAll(serviceHeader);

                if (salaryPeriods != null && salaryPeriods.Any())
                {
                    return salaryPeriods.ProjectedAsCollection<SalaryProcessingDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<SalaryProcessingDTO> FindSalaryPeriods(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = SalaryPeriodSpecifications.DefaultSpec();

                ISpecification<SalaryPeriod> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var salaryPeriodPagedCollection = _salaryPeriodRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (salaryPeriodPagedCollection != null)
                {
                    var pageCollection = salaryPeriodPagedCollection.PageCollection.ProjectedAsCollection<SalaryProcessingDTO>();

                    var itemsCount = salaryPeriodPagedCollection.ItemsCount;

                    return new PageCollectionInfo<SalaryProcessingDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<SalaryProcessingDTO> FindSalaryPeriods(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = SalaryPeriodSpecifications.SalaryPeriodFullText(text);

                ISpecification<SalaryPeriod> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var salaryPeriodCollection = _salaryPeriodRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (salaryPeriodCollection != null)
                {
                    var pageCollection = salaryPeriodCollection.PageCollection.ProjectedAsCollection<SalaryProcessingDTO>();

                    var itemsCount = salaryPeriodCollection.ItemsCount;

                    return new PageCollectionInfo<SalaryProcessingDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<SalaryProcessingDTO> FindSalaryPeriods(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = SalaryPeriodSpecifications.SalaryPeriodFullText(status, startDate, endDate, text);

                ISpecification<SalaryPeriod> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var salaryPeriodPagedCollection = _salaryPeriodRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (salaryPeriodPagedCollection != null)
                {
                    var pageCollection = salaryPeriodPagedCollection.PageCollection.ProjectedAsCollection<SalaryProcessingDTO>();

                    if (pageCollection != null && pageCollection.Any())
                    {
                        foreach (var item in pageCollection)
                        {
                            var totalItems = _paySlipAppService.CountPaySlipsBySalaryPeriodId(item.Id, serviceHeader);

                            var postedItems = _paySlipAppService.CountPostedPaySlipsBySalaryPeriodId(item.Id, serviceHeader);

                            item.PostedEntries = string.Format("{0}/{1}", postedItems, totalItems);
                        }
                    }

                    var itemsCount = salaryPeriodPagedCollection.ItemsCount;

                    return new PageCollectionInfo<SalaryProcessingDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public SalaryProcessingDTO FindSalaryPeriod(Guid salaryPeriodId, ServiceHeader serviceHeader)
        {
            if (salaryPeriodId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var salaryPeriod = _salaryPeriodRepository.Get(salaryPeriodId, serviceHeader);

                    if (salaryPeriod != null)
                    {
                        return salaryPeriod.ProjectedAs<SalaryProcessingDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public SalaryProcessingDTO FindCachedSalaryPeriod(Guid salaryPeriodId, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<SalaryProcessingDTO>(string.Format("{0}_{1}", serviceHeader.ApplicationDomainName, salaryPeriodId.ToString("D")), () =>
            {
                return FindSalaryPeriod(salaryPeriodId, serviceHeader);
            });
        }

        private decimal ComputeComplementaryStatutoryCharges(PaySlipEntryDTO payslipEntryDTO, decimal chargeableAmount, ServiceHeader serviceHeader)
        {
            var totalCharges = default(decimal);

            int systemTransactionType = 0;

            switch ((SalaryHeadType)payslipEntryDTO.SalaryHeadType)
            {
                case SalaryHeadType.NSSFDeduction:
                    systemTransactionType = (int)SystemTransactionType.NSSFCharges;
                    break;
                case SalaryHeadType.StatutoryProvidentFundDeduction:
                    systemTransactionType = (int)SystemTransactionType.ProvidentFundCharges;
                    break;
                default:
                    break;
            }

            var commissions = _commissionAppService.GetCommissionsForSystemTransactionType(systemTransactionType, serviceHeader);

            if (commissions != null && commissions.Any() && commissions.Count() == 1)
            {
                var targetCommission = commissions.FirstOrDefault();

                switch ((ChargeType)targetCommission.ComplementType)
                {
                    case ChargeType.Percentage:
                        totalCharges = Convert.ToDecimal((targetCommission.ComplementPercentage * Convert.ToDouble(chargeableAmount)) / 100);
                        totalCharges = targetCommission.MaximumCharge > 0m ? Math.Min(totalCharges, targetCommission.MaximumCharge) : totalCharges;
                        break;
                    case ChargeType.FixedAmount:
                        totalCharges = targetCommission.ComplementFixedAmount;
                        totalCharges = targetCommission.MaximumCharge > 0m ? Math.Min(totalCharges, targetCommission.MaximumCharge) : totalCharges;
                        break;
                    default:
                        break;
                }
            }

            if (totalCharges > 0m)
            {
                switch ((RoundingType)payslipEntryDTO.RoundingType)
                {
                    case RoundingType.ToEven:
                        totalCharges = Math.Round(totalCharges, MidpointRounding.ToEven);
                        break;
                    case RoundingType.AwayFromZero:
                        totalCharges = Math.Round(totalCharges, MidpointRounding.AwayFromZero);
                        break;
                    case RoundingType.Ceiling:
                        totalCharges = Math.Ceiling(totalCharges);
                        break;
                    case RoundingType.Floor:
                        totalCharges = Math.Floor(totalCharges);
                        break;
                    default:
                        break;
                }
            }

            return totalCharges;
        }

        private decimal ComputeStatutoryCharges(PaySlipEntryDTO payslipEntryDTO, decimal chargeableAmount, ServiceHeader serviceHeader)
        {
            var totalCharges = default(decimal);

            int systemTransactionType = 0;

            switch ((SalaryHeadType)payslipEntryDTO.SalaryHeadType)
            {
                case SalaryHeadType.NSSFDeduction:
                    systemTransactionType = (int)SystemTransactionType.NSSFCharges;
                    break;
                case SalaryHeadType.NHIFDeduction:
                    systemTransactionType = (int)SystemTransactionType.NHIFCharges;
                    break;
                case SalaryHeadType.PAYEDeduction:
                    systemTransactionType = (int)SystemTransactionType.PAYECharges;
                    break;
                case SalaryHeadType.StatutoryProvidentFundDeduction:
                    systemTransactionType = (int)SystemTransactionType.ProvidentFundCharges;
                    break;
                default:
                    break;
            }

            var commissions = _commissionAppService.GetCommissionsForSystemTransactionType(systemTransactionType, serviceHeader);

            if (commissions != null && commissions.Any() && commissions.Count() == 1)
            {
                var targetCommission = commissions.FirstOrDefault();

                var graduatedScales = _commissionAppService.FindGraduatedScales(targetCommission.Id, serviceHeader);

                if (graduatedScales != null && graduatedScales.Any())
                {
                    switch ((SystemTransactionType)systemTransactionType)
                    {
                        case SystemTransactionType.PAYECharges:

                            var orderedGraduatedScales = graduatedScales.OrderBy(x => x.RangeLowerLimit);

                            foreach (var graduatedScale in orderedGraduatedScales)
                            {
                                var rangeDifference = graduatedScale.RangeUpperLimit - graduatedScale.RangeLowerLimit;

                                var rangeCharge = Math.Min(chargeableAmount, rangeDifference) * Convert.ToDecimal((graduatedScale.ChargePercentage) / 100);

                                if ((rangeCharge * -1) < 0m)
                                {
                                    chargeableAmount -= rangeDifference;

                                    totalCharges += rangeCharge;
                                }
                            }

                            break;
                        case SystemTransactionType.NSSFCharges:
                        case SystemTransactionType.NHIFCharges:
                        case SystemTransactionType.ProvidentFundCharges:

                            var targetGraduatedScale = graduatedScales.Where(x => (chargeableAmount >= x.RangeLowerLimit && chargeableAmount <= x.RangeUpperLimit)).SingleOrDefault();

                            if (targetGraduatedScale != null)
                            {
                                switch ((ChargeType)targetGraduatedScale.ChargeType)
                                {
                                    case ChargeType.Percentage:
                                        totalCharges = Convert.ToDecimal((targetGraduatedScale.ChargePercentage * Convert.ToDouble(chargeableAmount)) / 100);
                                        totalCharges = targetCommission.MaximumCharge > 0m ? Math.Min(totalCharges, targetCommission.MaximumCharge) : totalCharges;
                                        break;
                                    case ChargeType.FixedAmount:
                                        totalCharges = targetGraduatedScale.ChargeFixedAmount;
                                        totalCharges = targetCommission.MaximumCharge > 0m ? Math.Min(totalCharges, targetCommission.MaximumCharge) : totalCharges;
                                        break;
                                    default:
                                        break;
                                }
                            }

                            break;
                        default:
                            break;
                    }
                }
            }

            if (totalCharges > 0m)
            {
                switch ((RoundingType)payslipEntryDTO.RoundingType)
                {
                    case RoundingType.ToEven:
                        totalCharges = Math.Round(totalCharges, MidpointRounding.ToEven);
                        break;
                    case RoundingType.AwayFromZero:
                        totalCharges = Math.Round(totalCharges, MidpointRounding.AwayFromZero);
                        break;
                    case RoundingType.Ceiling:
                        totalCharges = Math.Ceiling(totalCharges);
                        break;
                    case RoundingType.Floor:
                        totalCharges = Math.Floor(totalCharges);
                        break;
                    default:
                        break;
                }
            }

            return totalCharges;
        }
    }
}
