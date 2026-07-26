using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.StandingOrderAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.StandingOrderHistoryAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class StandingOrderAppService : IStandingOrderAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<StandingOrder> _standingOrderRepository;
        private readonly IRepository<StandingOrderHistory> _standingOrderHistoryRepository;
        private readonly IHolidayAppService _holidayAppService;
        private readonly ICustomerAccountAppService _customerAccountAppService;
        private readonly ISqlCommandAppService _sqlCommandAppService;

        public StandingOrderAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<StandingOrder> standingOrderRepository,
           IRepository<StandingOrderHistory> standingOrderHistoryRepository,
           IHolidayAppService holidayAppService,
           ICustomerAccountAppService customerAccountAppService,
           ISqlCommandAppService sqlCommandAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (standingOrderRepository == null)
                throw new ArgumentNullException(nameof(standingOrderRepository));

            if (standingOrderHistoryRepository == null)
                throw new ArgumentNullException(nameof(standingOrderHistoryRepository));

            if (holidayAppService == null)
                throw new ArgumentNullException(nameof(holidayAppService));

            if (customerAccountAppService == null)
                throw new ArgumentNullException(nameof(customerAccountAppService));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _standingOrderRepository = standingOrderRepository;
            _standingOrderHistoryRepository = standingOrderHistoryRepository;
            _holidayAppService = holidayAppService;
            _customerAccountAppService = customerAccountAppService;
            _sqlCommandAppService = sqlCommandAppService;
        }

        public StandingOrderDTO AddNewStandingOrder(StandingOrderDTO standingOrderDTO, ServiceHeader serviceHeader)
        {
            if (standingOrderDTO != null)
            {
                var existingStandingOrders = FindStandingOrders(standingOrderDTO.BenefactorCustomerAccountId, standingOrderDTO.BeneficiaryCustomerAccountId, standingOrderDTO.Trigger, serviceHeader);

                if (existingStandingOrders != null && existingStandingOrders.Any())
                    //throw new InvalidOperationException(string.Format("Sorry, but a standing order with trigger '{0}' already exists!", EnumHelper.GetDescription((StandingOrderTrigger)standingOrderDTO.Trigger)));

                    standingOrderDTO.ErrorMessageResult = string.Format("Sorry, but a standing order with trigger '{0}' already exists!", EnumHelper.GetDescription((StandingOrderTrigger)standingOrderDTO.Trigger));
               
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var duration = new Duration(standingOrderDTO.DurationStartDate, standingOrderDTO.DurationEndDate);

                    var expectedRunDate = duration.StartDate >= DateTime.Today ? duration.StartDate : DateTime.Today;

                    var nextRunDate = duration.StartDate >= DateTime.Today ? duration.StartDate : DateTime.Today;

                    var schedule = new Schedule(standingOrderDTO.ScheduleFrequency, expectedRunDate, nextRunDate, 0, standingOrderDTO.ScheduleForceExecute);

                    var charge = new Charge(standingOrderDTO.ChargeType, standingOrderDTO.ChargePercentage, standingOrderDTO.ChargeFixedAmount);

                    if (standingOrderDTO.BeneficiaryProductProductCode == (int)ProductCode.Loan)
                    {
                        switch ((RoundingType)standingOrderDTO.BeneficiaryProductRoundingType)
                        {
                            case RoundingType.ToEven:
                                standingOrderDTO.Principal = Math.Round(standingOrderDTO.Principal, MidpointRounding.ToEven);
                                standingOrderDTO.Interest = Math.Round(standingOrderDTO.Interest, MidpointRounding.ToEven);
                                standingOrderDTO.PaymentPerPeriod = Math.Round((standingOrderDTO.Principal + standingOrderDTO.Interest), MidpointRounding.ToEven);
                                break;
                            case RoundingType.AwayFromZero:
                                standingOrderDTO.Principal = Math.Round(standingOrderDTO.Principal, MidpointRounding.AwayFromZero);
                                standingOrderDTO.Interest = Math.Round(standingOrderDTO.Interest, MidpointRounding.AwayFromZero);
                                standingOrderDTO.PaymentPerPeriod = Math.Round((standingOrderDTO.Principal + standingOrderDTO.Interest), MidpointRounding.AwayFromZero);
                                break;
                            case RoundingType.Ceiling:
                                standingOrderDTO.Principal = Math.Ceiling(standingOrderDTO.Principal);
                                standingOrderDTO.Interest = Math.Ceiling(standingOrderDTO.Interest);
                                standingOrderDTO.PaymentPerPeriod = Math.Ceiling(standingOrderDTO.Principal + standingOrderDTO.Interest);
                                break;
                            case RoundingType.Floor:
                                standingOrderDTO.Principal = Math.Floor(standingOrderDTO.Principal);
                                standingOrderDTO.Interest = Math.Floor(standingOrderDTO.Interest);
                                standingOrderDTO.PaymentPerPeriod = Math.Floor(standingOrderDTO.Principal + standingOrderDTO.Interest);
                                break;
                            default:
                                break;
                        }
                    }

                    var standingOrder = StandingOrderFactory.CreateStandingOrder(standingOrderDTO.BenefactorCustomerAccountId, standingOrderDTO.BeneficiaryCustomerAccountId, duration, schedule, charge, standingOrderDTO.Trigger, standingOrderDTO.LoanAmount, standingOrderDTO.PaymentPerPeriod, standingOrderDTO.Principal, standingOrderDTO.Interest, standingOrderDTO.CapitalizedInterest, standingOrderDTO.Remarks, standingOrderDTO.Chargeable);

                    if (standingOrderDTO.IsLocked)
                        standingOrder.Lock();
                    else standingOrder.UnLock();

                    standingOrder.CreatedBy = serviceHeader.ApplicationUserName;

                    _standingOrderRepository.Add(standingOrder, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) > 0 ? standingOrder.ProjectedAs<StandingOrderDTO>() : null;
                }
            }
            else return null;
        }

        public bool UpdateStandingOrder(StandingOrderDTO standingOrderDTO, ServiceHeader serviceHeader)
        {
            if (standingOrderDTO == null || standingOrderDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _standingOrderRepository.Get(standingOrderDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    if (standingOrderDTO.Trigger == (int)StandingOrderTrigger.Schedule && persisted.Duration.StartDate != standingOrderDTO.DurationStartDate && standingOrderDTO.DurationStartDate < DateTime.Today)
                        throw new InvalidOperationException("The start date must not be less than today!");

                    if (standingOrderDTO.Trigger == (int)StandingOrderTrigger.Schedule && persisted.Duration.StartDate.Date != standingOrderDTO.DurationStartDate.Date || persisted.Schedule.ExpectedRunDate <= DateTime.Today/*has skipped schedules since last edited*/)
                    {
                        var holidayDTOs = _holidayAppService.FindHolidaysInCurrentPostingPeriod(serviceHeader);

                        var holidays = new List<DateTime>();

                        var dateRanges = (from h in holidayDTOs ?? new List<HolidayDTO>() select new { h.DurationStartDate, h.DurationEndDate });

                        foreach (var item in dateRanges)
                            for (DateTime date = item.DurationStartDate; date <= item.DurationEndDate; date = date.AddDays(1))
                                holidays.Add(date);

                        var siFrequency = (ScheduleFrequency)standingOrderDTO.ScheduleFrequency;

                        var startDate = standingOrderDTO.ScheduleExpectedRunDate;

                        if (persisted.Duration.StartDate.Date != standingOrderDTO.DurationStartDate.Date)
                            startDate = standingOrderDTO.DurationStartDate.Date;
                        else if (startDate < DateTime.Today)
                        {
                            while (startDate < DateTime.Today)
                            {
                                switch (siFrequency)
                                {
                                    case ScheduleFrequency.Annual:
                                        startDate = startDate.AddYears(1);
                                        break;
                                    case ScheduleFrequency.SemiAnnual:
                                        startDate = startDate.AddMonths(6);
                                        break;
                                    case ScheduleFrequency.TriAnnual:
                                        startDate = startDate.AddMonths(4);
                                        break;
                                    case ScheduleFrequency.Quarterly:
                                        startDate = startDate.AddMonths(3);
                                        break;
                                    case ScheduleFrequency.BiMonthly:
                                        startDate = startDate.AddMonths(2);
                                        break;
                                    case ScheduleFrequency.Monthly:
                                        startDate = startDate.AddMonths(1);
                                        break;
                                    case ScheduleFrequency.SemiMonthly:
                                        startDate = startDate.AddDays(15);
                                        break;
                                    case ScheduleFrequency.BiWeekly:
                                        startDate = startDate.AddDays(14);
                                        break;
                                    case ScheduleFrequency.Weekly:
                                        startDate = startDate.AddDays(7);
                                        break;
                                    case ScheduleFrequency.Daily:
                                        startDate = startDate.AddDays(1);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }

                        var originalStartDate = startDate;

                        switch (siFrequency)
                        {
                            case ScheduleFrequency.Annual:

                                startDate = startDate.AddYears(-1).AddDays(-1);

                                standingOrderDTO.ScheduleExpectedRunDate = startDate.AddDays(1).AddYears(1);

                                if (standingOrderDTO.ScheduleExpectedRunDate < originalStartDate)
                                    standingOrderDTO.ScheduleExpectedRunDate = originalStartDate;

                                standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddYears(1), 1, holidays);

                                while (standingOrderDTO.ScheduleActualRunDate < standingOrderDTO.ScheduleExpectedRunDate)
                                {
                                    standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(standingOrderDTO.ScheduleActualRunDate, 1, holidays);
                                }

                                break;
                            case ScheduleFrequency.SemiAnnual:

                                startDate = startDate.AddMonths(-6).AddDays(-1);

                                standingOrderDTO.ScheduleExpectedRunDate = startDate.AddDays(1).AddMonths(6);

                                if (standingOrderDTO.ScheduleExpectedRunDate < originalStartDate)
                                    standingOrderDTO.ScheduleExpectedRunDate = originalStartDate;

                                standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddMonths(6), 1, holidays);

                                while (standingOrderDTO.ScheduleActualRunDate < standingOrderDTO.ScheduleExpectedRunDate)
                                {
                                    standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(standingOrderDTO.ScheduleActualRunDate, 1, holidays);
                                }

                                break;
                            case ScheduleFrequency.TriAnnual:

                                startDate = startDate.AddMonths(-4).AddDays(-1);

                                standingOrderDTO.ScheduleExpectedRunDate = startDate.AddDays(1).AddMonths(4);

                                if (standingOrderDTO.ScheduleExpectedRunDate < originalStartDate)
                                    standingOrderDTO.ScheduleExpectedRunDate = originalStartDate;

                                standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddMonths(4), 1, holidays);

                                while (standingOrderDTO.ScheduleActualRunDate < standingOrderDTO.ScheduleExpectedRunDate)
                                {
                                    standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(standingOrderDTO.ScheduleActualRunDate, 1, holidays);
                                }

                                break;
                            case ScheduleFrequency.Quarterly:

                                startDate = startDate.AddMonths(-3).AddDays(-1);

                                standingOrderDTO.ScheduleExpectedRunDate = startDate.AddDays(1).AddMonths(3);

                                if (standingOrderDTO.ScheduleExpectedRunDate < originalStartDate)
                                    standingOrderDTO.ScheduleExpectedRunDate = originalStartDate;

                                standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddMonths(3), 1, holidays);

                                while (standingOrderDTO.ScheduleActualRunDate < standingOrderDTO.ScheduleExpectedRunDate)
                                {
                                    standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(standingOrderDTO.ScheduleActualRunDate, 1, holidays);
                                }

                                break;
                            case ScheduleFrequency.BiMonthly:

                                startDate = startDate.AddMonths(-2).AddDays(-1);

                                standingOrderDTO.ScheduleExpectedRunDate = startDate.AddDays(1).AddMonths(2);

                                if (standingOrderDTO.ScheduleExpectedRunDate < originalStartDate)
                                    standingOrderDTO.ScheduleExpectedRunDate = originalStartDate;

                                standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddMonths(2), 1, holidays);

                                while (standingOrderDTO.ScheduleActualRunDate < standingOrderDTO.ScheduleExpectedRunDate)
                                {
                                    standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(standingOrderDTO.ScheduleActualRunDate, 1, holidays);
                                }

                                break;
                            case ScheduleFrequency.Monthly:

                                startDate = startDate.AddMonths(-1).AddDays(-1);

                                standingOrderDTO.ScheduleExpectedRunDate = startDate.AddDays(1).AddMonths(1);

                                if (standingOrderDTO.ScheduleExpectedRunDate < originalStartDate)
                                    standingOrderDTO.ScheduleExpectedRunDate = originalStartDate;

                                standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddMonths(1), 1, holidays);

                                while (standingOrderDTO.ScheduleActualRunDate < standingOrderDTO.ScheduleExpectedRunDate)
                                {
                                    standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(standingOrderDTO.ScheduleActualRunDate, 1, holidays);
                                }

                                break;
                            case ScheduleFrequency.SemiMonthly:

                                startDate = startDate.AddDays(-16);

                                standingOrderDTO.ScheduleExpectedRunDate = startDate.AddDays(16);

                                if (standingOrderDTO.ScheduleExpectedRunDate < originalStartDate)
                                    standingOrderDTO.ScheduleExpectedRunDate = originalStartDate;

                                standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddDays(15), 1, holidays);

                                while (standingOrderDTO.ScheduleActualRunDate < standingOrderDTO.ScheduleExpectedRunDate)
                                {
                                    standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(standingOrderDTO.ScheduleActualRunDate, 1, holidays);
                                }

                                break;
                            case ScheduleFrequency.BiWeekly:

                                startDate = startDate.AddDays(-15);

                                standingOrderDTO.ScheduleExpectedRunDate = startDate.AddDays(15);

                                if (standingOrderDTO.ScheduleExpectedRunDate < originalStartDate)
                                    standingOrderDTO.ScheduleExpectedRunDate = originalStartDate;

                                standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddDays(14), 1, holidays);

                                while (standingOrderDTO.ScheduleActualRunDate < standingOrderDTO.ScheduleExpectedRunDate)
                                {
                                    standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(standingOrderDTO.ScheduleActualRunDate, 1, holidays);
                                }

                                break;
                            case ScheduleFrequency.Weekly:

                                startDate = startDate.AddDays(-8);

                                standingOrderDTO.ScheduleExpectedRunDate = startDate.AddDays(8);

                                if (standingOrderDTO.ScheduleExpectedRunDate < originalStartDate)
                                    standingOrderDTO.ScheduleExpectedRunDate = originalStartDate;

                                standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddDays(7), 1, holidays);

                                while (standingOrderDTO.ScheduleActualRunDate < standingOrderDTO.ScheduleExpectedRunDate)
                                {
                                    standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(standingOrderDTO.ScheduleActualRunDate, 1, holidays);
                                }

                                break;
                            case ScheduleFrequency.Daily:

                                startDate = startDate.AddDays(-1);

                                standingOrderDTO.ScheduleExpectedRunDate = startDate.AddDays(1);

                                if (standingOrderDTO.ScheduleExpectedRunDate < originalStartDate)
                                    standingOrderDTO.ScheduleExpectedRunDate = originalStartDate;

                                standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate, 1, holidays);

                                while (standingOrderDTO.ScheduleActualRunDate < standingOrderDTO.ScheduleExpectedRunDate)
                                {
                                    standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(standingOrderDTO.ScheduleActualRunDate, 1, holidays);
                                }

                                break;
                            default:
                                break;
                        }
                    }

                    var duration = new Duration(standingOrderDTO.DurationStartDate, standingOrderDTO.DurationEndDate);

                    var schedule = new Schedule(standingOrderDTO.ScheduleFrequency, standingOrderDTO.ScheduleExpectedRunDate, standingOrderDTO.ScheduleActualRunDate, 0, standingOrderDTO.ScheduleForceExecute);

                    var charge = new Charge(standingOrderDTO.ChargeType, standingOrderDTO.ChargePercentage, standingOrderDTO.ChargeFixedAmount);

                    if (standingOrderDTO.BeneficiaryProductProductCode == (int)ProductCode.Loan)
                    {
                        switch ((RoundingType)standingOrderDTO.BeneficiaryProductRoundingType)
                        {
                            case RoundingType.ToEven:
                                standingOrderDTO.Principal = Math.Round(standingOrderDTO.Principal, MidpointRounding.ToEven);
                                standingOrderDTO.Interest = Math.Round(standingOrderDTO.Interest, MidpointRounding.ToEven);
                                standingOrderDTO.PaymentPerPeriod = Math.Round((standingOrderDTO.Principal + standingOrderDTO.Interest), MidpointRounding.ToEven);
                                break;
                            case RoundingType.AwayFromZero:
                                standingOrderDTO.Principal = Math.Round(standingOrderDTO.Principal, MidpointRounding.AwayFromZero);
                                standingOrderDTO.Interest = Math.Round(standingOrderDTO.Interest, MidpointRounding.AwayFromZero);
                                standingOrderDTO.PaymentPerPeriod = Math.Round((standingOrderDTO.Principal + standingOrderDTO.Interest), MidpointRounding.AwayFromZero);
                                break;
                            case RoundingType.Ceiling:
                                standingOrderDTO.Principal = Math.Ceiling(standingOrderDTO.Principal);
                                standingOrderDTO.Interest = Math.Ceiling(standingOrderDTO.Interest);
                                standingOrderDTO.PaymentPerPeriod = Math.Ceiling(standingOrderDTO.Principal + standingOrderDTO.Interest);
                                break;
                            case RoundingType.Floor:
                                standingOrderDTO.Principal = Math.Floor(standingOrderDTO.Principal);
                                standingOrderDTO.Interest = Math.Floor(standingOrderDTO.Interest);
                                standingOrderDTO.PaymentPerPeriod = Math.Floor(standingOrderDTO.Principal + standingOrderDTO.Interest);
                                break;
                            default:
                                break;
                        }
                    }

                    var current = StandingOrderFactory.CreateStandingOrder(standingOrderDTO.BenefactorCustomerAccountId, standingOrderDTO.BeneficiaryCustomerAccountId, duration, schedule, charge, standingOrderDTO.Trigger, standingOrderDTO.LoanAmount, standingOrderDTO.PaymentPerPeriod, standingOrderDTO.Principal, standingOrderDTO.Interest, standingOrderDTO.CapitalizedInterest, standingOrderDTO.Remarks, standingOrderDTO.Chargeable);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    current.CreatedBy = persisted.CreatedBy;

                    if (standingOrderDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _standingOrderRepository.Merge(persisted, current, serviceHeader);
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public List<StandingOrderDTO> FindStandingOrders(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var standingOrders = _standingOrderRepository.GetAll(serviceHeader);

                if (standingOrders != null && standingOrders.Any())
                {
                    return standingOrders.ProjectedAsCollection<StandingOrderDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<StandingOrderDTO> FindStandingOrders(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = StandingOrderSpecifications.DefaultSpec();

                ISpecification<StandingOrder> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var standingOrderPagedCollection = _standingOrderRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (standingOrderPagedCollection != null)
                {
                    var pageCollection = standingOrderPagedCollection.PageCollection.ProjectedAsCollection<StandingOrderDTO>();

                    var itemsCount = standingOrderPagedCollection.ItemsCount;

                    return new PageCollectionInfo<StandingOrderDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<StandingOrderDTO> FindStandingOrders(string text, int customerAccountFilter, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = StandingOrderSpecifications.StandingOrderFullText(text, customerAccountFilter, customerFilter);

                ISpecification<StandingOrder> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var standingOrderPagedCollection = _standingOrderRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (standingOrderPagedCollection != null)
                {
                    var pageCollection = standingOrderPagedCollection.PageCollection.ProjectedAsCollection<StandingOrderDTO>();

                    var itemsCount = standingOrderPagedCollection.ItemsCount;

                    return new PageCollectionInfo<StandingOrderDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<StandingOrderDTO> FindStandingOrders(int trigger, string text, int customerAccountFilter, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = StandingOrderSpecifications.StandingOrderFullText(trigger, text, customerAccountFilter, customerFilter);

                ISpecification<StandingOrder> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var standingOrderPagedCollection = _standingOrderRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (standingOrderPagedCollection != null)
                {
                    var pageCollection = standingOrderPagedCollection.PageCollection.ProjectedAsCollection<StandingOrderDTO>();

                    var itemsCount = standingOrderPagedCollection.ItemsCount;

                    return new PageCollectionInfo<StandingOrderDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<StandingOrderHistoryDTO> FindStandingOrderHistory(Guid standingOrderId, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (standingOrderId != null && standingOrderId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = StandingOrderHistorySpecifications.StandingOrderHistoryWithStandingOrderId(standingOrderId);

                    ISpecification<StandingOrderHistory> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var standingOrderHistoryPagedCollection = _standingOrderHistoryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (standingOrderHistoryPagedCollection != null)
                    {
                        var pageCollection = standingOrderHistoryPagedCollection.PageCollection.ProjectedAsCollection<StandingOrderHistoryDTO>();

                        var itemsCount = standingOrderHistoryPagedCollection.ItemsCount;

                        return new PageCollectionInfo<StandingOrderHistoryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                    }
                    else return null;
                }
            }
            else return null;
        }

        public StandingOrderDTO FindStandingOrder(Guid standingOrderId, ServiceHeader serviceHeader)
        {
            if (standingOrderId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var standingOrder = _standingOrderRepository.Get(standingOrderId, serviceHeader);

                    if (standingOrder != null)
                    {
                        return standingOrder.ProjectedAs<StandingOrderDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<StandingOrderDTO> FindStandingOrders(Guid benefactorCustomerAccountId, Guid beneficiaryCustomerAccountId, ServiceHeader serviceHeader)
        {
            if (benefactorCustomerAccountId != null && benefactorCustomerAccountId != Guid.Empty && beneficiaryCustomerAccountId != null && beneficiaryCustomerAccountId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = StandingOrderSpecifications.StandingOrderWithBenefactorCustomerAccountIdAndBeneficiaryCustomerAccountId(benefactorCustomerAccountId, beneficiaryCustomerAccountId);

                    ISpecification<StandingOrder> spec = filter;

                    var standingOrders = _standingOrderRepository.AllMatching(spec, serviceHeader);

                    if (standingOrders != null && standingOrders.Any())
                    {
                        return standingOrders.ProjectedAsCollection<StandingOrderDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<StandingOrderDTO> FindStandingOrders(Guid benefactorCustomerAccountId, Guid beneficiaryCustomerAccountId, int trigger, ServiceHeader serviceHeader)
        {
            if (benefactorCustomerAccountId != null && benefactorCustomerAccountId != Guid.Empty && beneficiaryCustomerAccountId != null && beneficiaryCustomerAccountId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = StandingOrderSpecifications.StandingOrderWithBenefactorCustomerAccountIdAndBeneficiaryCustomerAccountIdAndTrigger(benefactorCustomerAccountId, beneficiaryCustomerAccountId, trigger);

                    ISpecification<StandingOrder> spec = filter;

                    var standingOrders = _standingOrderRepository.AllMatching(spec, serviceHeader);

                    if (standingOrders != null && standingOrders.Any())
                    {
                        return standingOrders.ProjectedAsCollection<StandingOrderDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<StandingOrderDTO> FindStandingOrdersByBenefactorCustomerId(Guid benefactorCustomerId, int benefactorCustomerAccountTypeProductCode, ServiceHeader serviceHeader)
        {
            if (benefactorCustomerId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = StandingOrderSpecifications.StandingOrderWithBenefactorCustomerIdAndBenefactorCustomerAccountCustomerAccountTypeProductCode(benefactorCustomerId, benefactorCustomerAccountTypeProductCode);

                    ISpecification<StandingOrder> spec = filter;

                    var standingOrders = _standingOrderRepository.AllMatching(spec, serviceHeader);

                    if (standingOrders != null && standingOrders.Any())
                    {
                        return standingOrders.ProjectedAsCollection<StandingOrderDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<StandingOrderDTO> FindStandingOrdersByBenefactorCustomerAccountId(Guid benefactorCustomerAccountId, ServiceHeader serviceHeader)
        {
            if (benefactorCustomerAccountId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = StandingOrderSpecifications.StandingOrderWithBenefactorCustomerAccountId(benefactorCustomerAccountId);

                    ISpecification<StandingOrder> spec = filter;

                    var standingOrders = _standingOrderRepository.AllMatching(spec, serviceHeader);

                    if (standingOrders != null && standingOrders.Any())
                    {
                        return standingOrders.ProjectedAsCollection<StandingOrderDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<StandingOrderDTO> FindStandingOrdersByBeneficiaryCustomerAccountId(Guid beneficiaryCustomerAccountId, ServiceHeader serviceHeader)
        {
            if (beneficiaryCustomerAccountId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = StandingOrderSpecifications.StandingOrderWithBeneficiaryCustomerAccountId(beneficiaryCustomerAccountId);

                    ISpecification<StandingOrder> spec = filter;

                    var standingOrders = _standingOrderRepository.AllMatching(spec, serviceHeader);

                    if (standingOrders != null && standingOrders.Any())
                    {
                        return standingOrders.ProjectedAsCollection<StandingOrderDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<StandingOrderDTO> FindStandingOrdersByBeneficiaryCustomerAccountId(Guid beneficiaryCustomerAccountId, int trigger, ServiceHeader serviceHeader)
        {
            if (beneficiaryCustomerAccountId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = StandingOrderSpecifications.StandingOrderWithBeneficiaryCustomerAccountIdAndTrigger(beneficiaryCustomerAccountId, trigger);

                    ISpecification<StandingOrder> spec = filter;

                    var standingOrders = _standingOrderRepository.AllMatching(spec, serviceHeader);

                    if (standingOrders != null && standingOrders.Any())
                    {
                        return standingOrders.ProjectedAsCollection<StandingOrderDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<StandingOrderDTO> FindStandingOrdersByBenefactorCustomerAccountId(Guid benefactorCustomerAccountId, int trigger, ServiceHeader serviceHeader)
        {
            if (benefactorCustomerAccountId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = StandingOrderSpecifications.StandingOrderWithBenefactorCustomerAccountIdAndTrigger(benefactorCustomerAccountId, trigger);

                    ISpecification<StandingOrder> spec = filter;

                    var standingOrders = _standingOrderRepository.AllMatching(spec, serviceHeader);

                    if (standingOrders != null && standingOrders.Any())
                    {
                        return standingOrders.ProjectedAsCollection<StandingOrderDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<StandingOrderDTO> FindDueStandingOrders(DateTime targetDate, int targetDateOption, string searchString, int customerAccountFilter, int customerFilter, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = StandingOrderSpecifications.DueStandingOrders(targetDate, targetDateOption, searchString, customerAccountFilter, customerFilter);

                ISpecification<StandingOrder> spec = filter;

                var standingOrders = _standingOrderRepository.AllMatching(spec, serviceHeader);

                if (standingOrders != null && standingOrders.Any())
                {
                    return standingOrders.ProjectedAsCollection<StandingOrderDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<StandingOrderDTO> FindSkippedStandingOrders(DateTime targetDate, string searchString, int customerAccountFilter, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = StandingOrderSpecifications.SkippedStandingOrders(targetDate, searchString, customerAccountFilter, customerFilter);

                ISpecification<StandingOrder> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var standingOrderPagedCollection = _standingOrderRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (standingOrderPagedCollection != null)
                {
                    var pageCollection = standingOrderPagedCollection.PageCollection.ProjectedAsCollection<StandingOrderDTO>();

                    var itemsCount = standingOrderPagedCollection.ItemsCount;

                    return new PageCollectionInfo<StandingOrderDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public bool AutoCreateStandindOrders(Guid benefactorProductId, int benefactorProductCode, Guid beneficiaryProductId, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (benefactorProductId != null && benefactorProductId != Guid.Empty && beneficiaryProductId != null && beneficiaryProductId != Guid.Empty)
            {
                var benefactorCustomerAccounts = _sqlCommandAppService.FindCustomerAccountsByTargetProductId(benefactorProductId, serviceHeader);

                var beneficiaryCustomerAccounts = _sqlCommandAppService.FindCustomerAccountsByTargetProductId(beneficiaryProductId, serviceHeader);

                if (benefactorCustomerAccounts != null && benefactorCustomerAccounts.Any() && beneficiaryCustomerAccounts != null && beneficiaryCustomerAccounts.Any())
                {
                    var counter = default(int);

                    foreach (var targetBenefactorCustomerAccount in benefactorCustomerAccounts)
                    {
                        counter += 1;

                        var benefactorStandingOrders = FindStandingOrdersByBenefactorCustomerId(targetBenefactorCustomerAccount.CustomerId, benefactorProductCode, serviceHeader);

                        if (benefactorStandingOrders != null && benefactorStandingOrders.Any(x => x.BeneficiaryCustomerAccountCustomerId == targetBenefactorCustomerAccount.CustomerId && x.BeneficiaryCustomerAccountCustomerAccountTypeTargetProductId == beneficiaryProductId))
                            continue;

                        var targetBeneficiaryCustomerAccount = beneficiaryCustomerAccounts.FirstOrDefault(x => x.CustomerId == targetBenefactorCustomerAccount.CustomerId);

                        if (targetBeneficiaryCustomerAccount != null)
                        {
                            var standingOrderDTO = new StandingOrderDTO
                            {
                                BenefactorCustomerAccountId = targetBenefactorCustomerAccount.Id,
                                BeneficiaryCustomerAccountId = targetBeneficiaryCustomerAccount.Id,
                                DurationStartDate = DateTime.Today,
                                DurationEndDate = DateTime.Today.AddYears(5),
                                ScheduleFrequency = (int)ScheduleFrequency.Monthly,
                                Trigger = (int)StandingOrderTrigger.Payout,
                                LoanAmount = 0m,
                                Principal = 0m,
                                Interest = 0m,
                                ChargeType = (int)ChargeType.FixedAmount,
                                ChargeFixedAmount = 50m,
                                ChargePercentage = 0d,
                            };

                            AddNewStandingOrder(standingOrderDTO, serviceHeader);
                        }
                    }

                    result = true;
                }
            }

            return result;
        }

        public bool FixSkippedStandingOrders(DateTime targetDate, int pageSize, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var standingOrderDTOs = new List<StandingOrderDTO>();

            var itemsCount = 0;

            var pageIndex = 0;

            var pageCollectionInfo = FindSkippedStandingOrders(targetDate, null, (int)StandingOrderCustomerAccountFilter.Beneficiary, (int)CustomerFilter.Reference1, pageIndex, pageSize, serviceHeader);

            if (pageCollectionInfo != null && pageCollectionInfo.PageCollection != null)
            {
                itemsCount = pageCollectionInfo.ItemsCount;

                standingOrderDTOs.AddRange(pageCollectionInfo.PageCollection);

                if (itemsCount > pageSize)
                {
                    ++pageIndex;

                    while ((pageSize * pageIndex) <= itemsCount)
                    {
                        pageCollectionInfo = FindSkippedStandingOrders(targetDate, null, (int)StandingOrderCustomerAccountFilter.Beneficiary, (int)CustomerFilter.Reference1, pageIndex, pageSize, serviceHeader);

                        if (pageCollectionInfo != null && pageCollectionInfo.PageCollection != null)
                        {
                            standingOrderDTOs.AddRange(pageCollectionInfo.PageCollection);

                            ++pageIndex;
                        }
                        else break;
                    }
                }
            }

            if (standingOrderDTOs != null && standingOrderDTOs.Any())
            {
                standingOrderDTOs.ForEach(item =>
                {
                    item.ScheduleExecuteAttemptCount = 0;

                    result = UpdateStandingOrder(item, serviceHeader);
                });
            }

            return result;
        }
    }
}
