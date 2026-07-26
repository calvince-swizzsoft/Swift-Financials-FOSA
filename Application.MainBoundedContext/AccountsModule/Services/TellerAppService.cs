using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.FrontOfficeModule.Services;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.TellerAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class TellerAppService : ITellerAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<Teller> _tellerRepository;
        private readonly ICommissionAppService _commissionAppService;
        private readonly ICashWithdrawalRequestAppService _cashWithdrawalRequestAppService;
        private readonly ISqlCommandAppService _sqlCommandAppService;

        public TellerAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<Teller> tellerRepository,
           ICommissionAppService commissionAppService,
           ISqlCommandAppService sqlCommandAppService,
           ICashWithdrawalRequestAppService cashWithdrawalRequestAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (tellerRepository == null)
                throw new ArgumentNullException(nameof(tellerRepository));

            if (commissionAppService == null)
                throw new ArgumentNullException(nameof(commissionAppService));

            if (cashWithdrawalRequestAppService == null)
                throw new ArgumentNullException(nameof(cashWithdrawalRequestAppService));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _tellerRepository = tellerRepository;
            _commissionAppService = commissionAppService;
            _cashWithdrawalRequestAppService = cashWithdrawalRequestAppService;
            _sqlCommandAppService = sqlCommandAppService;
        }

        public TellerDTO AddNewTeller(TellerDTO tellerDTO, ServiceHeader serviceHeader)
        {
            if (tellerDTO != null && tellerDTO.ChartOfAccountId != Guid.Empty && tellerDTO.ShortageChartOfAccountId != Guid.Empty)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    bool proceed = true;

                    switch ((TellerType)tellerDTO.Type)
                    {
                        case TellerType.Employee:

                            if (tellerDTO.EmployeeId != null && tellerDTO.EmployeeId != Guid.Empty)
                            {
                                var filter = TellerSpecifications.TellerWithEmployeeId(tellerDTO.EmployeeId.Value);

                                ISpecification<Teller> spec = filter;

                                var tellers = _tellerRepository.AllMatching(spec, serviceHeader);

                                if (tellers != null && tellers.Any())
                                {
                                    proceed = false;
                                }
                            }
                            else
                            {
                                proceed = false;
                            }
                            break;
                        case TellerType.AutomatedTellerMachine:
                        case TellerType.InhousePointOfSale:
                        case TellerType.AgentPointOfSale:
                            break;
                        default:
                            break;
                    }

                    if (!proceed)
                    {
                        return null;
                    }
                    else
                    {
                        var range = new Range(tellerDTO.RangeLowerLimit, tellerDTO.RangeUpperLimit);

                        var teller = TellerFactory.CreateTeller(tellerDTO.Type, tellerDTO.EmployeeId, tellerDTO.ChartOfAccountId, tellerDTO.ShortageChartOfAccountId, tellerDTO.ExcessChartOfAccountId, tellerDTO.FloatCustomerAccountId, tellerDTO.CommissionCustomerAccountId, tellerDTO.Description, range, tellerDTO.MiniStatementItemsCap, tellerDTO.Reference);

                        teller.Code = (short)_tellerRepository.DatabaseSqlQuery<int>(string.Format("SELECT ISNULL(MAX(Code),0) + 1 AS Expr1 FROM {0}Tellers", DefaultSettings.Instance.TablePrefix), serviceHeader).FirstOrDefault();

                        if (tellerDTO.IsLocked)
                            teller.Lock();
                        else teller.UnLock();

                        _tellerRepository.Add(teller, serviceHeader);

                        dbContextScope.SaveChanges(serviceHeader);

                        return teller.ProjectedAs<TellerDTO>();
                    }
                }
            }
            else return null;
        }

        public bool UpdateTeller(TellerDTO tellerDTO, ServiceHeader serviceHeader)
        {
            if (tellerDTO == null || tellerDTO.Id == Guid.Empty || tellerDTO.EmployeeId == Guid.Empty || tellerDTO.ChartOfAccountId == Guid.Empty || tellerDTO.ShortageChartOfAccountId == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _tellerRepository.Get(tellerDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var range = new Range(tellerDTO.RangeLowerLimit, tellerDTO.RangeUpperLimit);

                    var current = TellerFactory.CreateTeller(persisted.Type, persisted.EmployeeId, tellerDTO.ChartOfAccountId, tellerDTO.ShortageChartOfAccountId, tellerDTO.ExcessChartOfAccountId, tellerDTO.FloatCustomerAccountId, tellerDTO.CommissionCustomerAccountId, tellerDTO.Description, range, tellerDTO.MiniStatementItemsCap, tellerDTO.Reference);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    current.Code = persisted.Code;


                    if (tellerDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _tellerRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<TellerDTO> FindTellers(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var tellers = _tellerRepository.GetAll(serviceHeader);

                if (tellers != null && tellers.Any())
                {
                    return tellers.ProjectedAsCollection<TellerDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<TellerDTO> FindTellers(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = TellerSpecifications.DefaultSpec();

                ISpecification<Teller> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var tellerPagedCollection = _tellerRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (tellerPagedCollection != null)
                {
                    var pageCollection = tellerPagedCollection.PageCollection.ProjectedAsCollection<TellerDTO>();

                    var itemsCount = tellerPagedCollection.ItemsCount;

                    return new PageCollectionInfo<TellerDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<TellerDTO> FindTellers(int tellerType, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = TellerSpecifications.TellerFullText(tellerType, text);

                ISpecification<Teller> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var tellerCollection = _tellerRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (tellerCollection != null)
                {
                    var pageCollection = tellerCollection.PageCollection.ProjectedAsCollection<TellerDTO>();

                    var itemsCount = tellerCollection.ItemsCount;

                    return new PageCollectionInfo<TellerDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public List<TellerDTO> FindTellers(int tellerType, string reference, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = TellerSpecifications.TellerWithTellerType(tellerType, reference);

                ISpecification<Teller> spec = filter;

                var tellers = _tellerRepository.AllMatching(spec, serviceHeader);

                if (tellers != null && tellers.Any())
                {
                    return tellers.ProjectedAsCollection<TellerDTO>();
                }
                else return null;
            }
        }

        public List<TellerDTO> FindTellers(string reference, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = TellerSpecifications.TellerWithReference(reference);

                ISpecification<Teller> spec = filter;

                var tellers = _tellerRepository.AllMatching(spec, serviceHeader);

                if (tellers != null && tellers.Any())
                {
                    return tellers.ProjectedAsCollection<TellerDTO>();
                }
                else return null;
            }
        }

        public TellerDTO FindTeller(Guid tellerId, ServiceHeader serviceHeader)
        {
            if (tellerId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var teller = _tellerRepository.Get(tellerId, serviceHeader);

                    if (teller != null)
                    {
                        return teller.ProjectedAs<TellerDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public TellerDTO FindTellerByEmployeeId(Guid employeeId, ServiceHeader serviceHeader)
        {
            if (employeeId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = TellerSpecifications.TellerWithEmployeeId(employeeId);

                    ISpecification<Teller> spec = filter;

                    var tellers = _tellerRepository.AllMatching(spec, serviceHeader);

                    if (tellers != null && tellers.Any() && tellers.Count() == 1)
                    {
                        var teller = tellers.SingleOrDefault();

                        if (teller != null)
                        {
                            return teller.ProjectedAs<TellerDTO>();
                        }
                        else return null;
                    }
                    else return null;
                }
            }
            else return null;
        }

        public void FetchTellerBalances(List<TellerDTO> tellers, ServiceHeader serviceHeader)
        {
            if (tellers != null && tellers.Any())
            {
                tellers.ForEach(teller =>
                {
                    switch ((TellerType)teller.Type)
                    {
                        case TellerType.Employee:
                        case TellerType.AutomatedTellerMachine:
                        case TellerType.InhousePointOfSale:
                            if (teller.ChartOfAccountId.HasValue)
                            {
                                teller.BookBalance = _sqlCommandAppService.FindGlAccountBalance(teller.ChartOfAccountId.Value, DateTime.Now, (int)TransactionDateFilter.CreatedDate, serviceHeader);

                                var statisticsTuple = _sqlCommandAppService.FindGlAccountStatistics(teller.ChartOfAccountId.Value, DateTime.Today, DateTime.Now, (int)TransactionDateFilter.CreatedDate, serviceHeader);

                                teller.TotalCredits = statisticsTuple.Item1;
                                teller.TotalDebits = statisticsTuple.Item2;

                                teller.OpeningBalance = statisticsTuple.Item3;
                                teller.ClosingBalance = statisticsTuple.Item4;
                                teller.ItemsCount = statisticsTuple.Item5;
                            }
                            break;
                        case TellerType.AgentPointOfSale:
                        default:
                            break;
                    }
                });
            }
        }

        public List<TariffWrapper> ComputeCashTariffs(CustomerAccountDTO customerAccountDTO, decimal totalValue, int frontOfficeTransactionType, ServiceHeader serviceHeader)
        {
            var tariffs = new List<TariffWrapper>();

            var savingsProductKnownChargeType = SavingsProductKnownChargeType.CashDeposit;

            switch ((FrontOfficeTransactionType)frontOfficeTransactionType)
            {
                case FrontOfficeTransactionType.CashWithdrawal:
                    savingsProductKnownChargeType = SavingsProductKnownChargeType.CashWithdrawal;
                    break;
                case FrontOfficeTransactionType.CashDeposit:
                    savingsProductKnownChargeType = SavingsProductKnownChargeType.CashDeposit;
                    break;
                case FrontOfficeTransactionType.CashWithdrawalPaymentVoucher:
                    savingsProductKnownChargeType = SavingsProductKnownChargeType.CashWithdrawalPaymentVoucher;
                    break;
                default:
                    break;
            }

            var knownTariffs = _commissionAppService.ComputeTariffsBySavingsProduct(customerAccountDTO.CustomerAccountTypeTargetProductId, (int)savingsProductKnownChargeType, totalValue, customerAccountDTO, serviceHeader);

            if (knownTariffs != null && knownTariffs.Any())
                tariffs.AddRange(knownTariffs);

            if (savingsProductKnownChargeType.In(SavingsProductKnownChargeType.CashWithdrawal))
            {
                if (totalValue > customerAccountDTO.CustomerAccountTypeTargetProductWithdrawalNoticeAmount)
                {
                    var matureCashWithdrawalRequests = _cashWithdrawalRequestAppService.FindMatureCashWithdrawalRequestsByCustomerAccountId(customerAccountDTO, serviceHeader);

                    var compute = default(bool);

                    if (matureCashWithdrawalRequests == null)
                        compute = true;
                    else if (!(matureCashWithdrawalRequests.Any(x => x.Amount == totalValue && x.Status == (int)CashWithdrawalRequestAuthStatus.Authorized)))
                        compute = true;
                    else if ((matureCashWithdrawalRequests.Any(x => x.Amount == totalValue && x.Status == (int)CashWithdrawalRequestAuthStatus.Authorized && x.Type == (int)CashWithdrawalRequestType.ImmediateNotice)))
                        compute = true;

                    if (compute)
                    {
                        var withoutNoticeTariffs = _commissionAppService.ComputeTariffsBySavingsProduct(customerAccountDTO.CustomerAccountTypeTargetProductId, (int)SavingsProductKnownChargeType.CashWithdrawalWithoutNotice, totalValue, customerAccountDTO, serviceHeader);

                        if (withoutNoticeTariffs != null && withoutNoticeTariffs.Any())
                            tariffs.AddRange(withoutNoticeTariffs);
                    }
                }

                if (IsCashWithdrawalIntervalPremature(customerAccountDTO, serviceHeader))
                {
                    var prematureIntervalTariffs = _commissionAppService.ComputeTariffsBySavingsProduct(customerAccountDTO.CustomerAccountTypeTargetProductId, (int)SavingsProductKnownChargeType.CashWithdrawalPrematureInterval, 0m, customerAccountDTO, serviceHeader);

                    if (prematureIntervalTariffs != null && prematureIntervalTariffs.Any())
                        tariffs.AddRange(prematureIntervalTariffs);
                }

                var previousAvailableBalance = _sqlCommandAppService.FindCustomerAccountAvailableBalance(customerAccountDTO, DateTime.Now, serviceHeader);

                if (((totalValue + tariffs.Where(x => x.ChargeBenefactor == (int)ChargeBenefactor.Customer).Sum(x => x.Amount)) > previousAvailableBalance) && ((totalValue + tariffs.Sum(x => x.Amount)) <= (previousAvailableBalance + customerAccountDTO.CustomerAccountTypeTargetProductMinimumBalance)))
                {
                    var belowMinimumBalanceTariffs = _commissionAppService.ComputeTariffsBySavingsProduct(customerAccountDTO.CustomerAccountTypeTargetProductId, (int)SavingsProductKnownChargeType.CashWithdrawalBelowMinimumBalance, 0m, customerAccountDTO, serviceHeader);

                    foreach (var item in belowMinimumBalanceTariffs)
                    {
                        tariffs.Add(item);
                    }
                }
            }

            return tariffs;
        }

        private bool IsCashWithdrawalIntervalPremature(CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader)
        {
            if (customerAccountDTO != null)
            {
                var lastCashWithdrawalDate = _sqlCommandAppService.CheckCustomerAccountLastWithdrawalDate(customerAccountDTO.Id, serviceHeader);

                if (lastCashWithdrawalDate != null)
                {
                    var targetDate = lastCashWithdrawalDate.Value.AddDays(customerAccountDTO.CustomerAccountTypeTargetProductWithdrawalInterval);

                    return targetDate >= DateTime.Now;
                }
                else return false;
            }
            else return false;
        }
    }
}
