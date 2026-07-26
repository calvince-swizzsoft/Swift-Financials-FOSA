using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ElectronicStatementOrderAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ElectronicStatementOrderHistoryAgg;
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
    public class ElectronicStatementOrderAppService : IElectronicStatementOrderAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<ElectronicStatementOrder> _electronicStatementOrderRepository;
        private readonly IRepository<ElectronicStatementOrderHistory> _electronicStatementOrderHistoryRepository;
        private readonly ISqlCommandAppService _sqlCommandAppService;
        private readonly IHolidayAppService _holidayAppService;

        public ElectronicStatementOrderAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<ElectronicStatementOrder> electronicStatementOrderRepository,
           IRepository<ElectronicStatementOrderHistory> electronicStatementOrderHistoryRepository,
           ISqlCommandAppService sqlCommandAppService,
           IHolidayAppService holidayAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (electronicStatementOrderRepository == null)
                throw new ArgumentNullException(nameof(electronicStatementOrderRepository));

            if (electronicStatementOrderHistoryRepository == null)
                throw new ArgumentNullException(nameof(electronicStatementOrderHistoryRepository));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            if (holidayAppService == null)
                throw new ArgumentNullException(nameof(holidayAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _electronicStatementOrderRepository = electronicStatementOrderRepository;
            _electronicStatementOrderHistoryRepository = electronicStatementOrderHistoryRepository;
            _sqlCommandAppService = sqlCommandAppService;
            _holidayAppService = holidayAppService;
        }

        public ElectronicStatementOrderDTO AddNewElectronicStatementOrder(ElectronicStatementOrderDTO electronicStatementOrderDTO, ServiceHeader serviceHeader)
        {
            if (electronicStatementOrderDTO != null)
            {
                var existingElectronicStatementOrders = FindElectronicStatementOrdersByCustomerAccountId(electronicStatementOrderDTO.CustomerAccountId, serviceHeader);

                if (existingElectronicStatementOrders != null && existingElectronicStatementOrders.Any())
                    throw new InvalidOperationException(string.Format("Sorry, but an e-statement order for the selected account already exists!"));

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var duration = new Duration(electronicStatementOrderDTO.DurationStartDate, electronicStatementOrderDTO.DurationEndDate);

                    var expectedRunDate = duration.StartDate >= DateTime.Today ? duration.StartDate : DateTime.Today;

                    var nextRunDate = duration.StartDate >= DateTime.Today ? duration.StartDate : DateTime.Today;

                    var schedule = new Schedule(electronicStatementOrderDTO.ScheduleFrequency, expectedRunDate, nextRunDate, 0, electronicStatementOrderDTO.ScheduleForceExecute);

                    var electronicStatementOrder = ElectronicStatementOrderFactory.CreateElectronicStatementOrder(electronicStatementOrderDTO.CustomerAccountId, duration, schedule, electronicStatementOrderDTO.Remarks);

                    if (electronicStatementOrderDTO.IsLocked)
                        electronicStatementOrder.Lock();
                    else electronicStatementOrder.UnLock();

                    electronicStatementOrder.CreatedBy = serviceHeader.ApplicationUserName;

                    _electronicStatementOrderRepository.Add(electronicStatementOrder, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return electronicStatementOrder.ProjectedAs<ElectronicStatementOrderDTO>();
                }
            }
            else return null;
        }

        public bool UpdateElectronicStatementOrder(ElectronicStatementOrderDTO electronicStatementOrderDTO, ServiceHeader serviceHeader)
        {
            if (electronicStatementOrderDTO == null || electronicStatementOrderDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _electronicStatementOrderRepository.Get(electronicStatementOrderDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    if (persisted.Duration.StartDate != electronicStatementOrderDTO.DurationStartDate && electronicStatementOrderDTO.DurationStartDate < DateTime.Today)
                        throw new InvalidOperationException("The start date must not be less than today!");

                    if (persisted.Duration.StartDate.Date != electronicStatementOrderDTO.DurationStartDate.Date || persisted.Schedule.ExpectedRunDate <= DateTime.Today/*has skipped schedules since last edited*/)
                    {
                        var holidayDTOs = _holidayAppService.FindHolidaysInCurrentPostingPeriod(serviceHeader);

                        var holidays = new List<DateTime>();

                        var dateRanges = (from h in holidayDTOs ?? new List<HolidayDTO>() select new { h.DurationStartDate, h.DurationEndDate });

                        foreach (var item in dateRanges)
                            for (DateTime date = item.DurationStartDate; date <= item.DurationEndDate; date = date.AddDays(1))
                                holidays.Add(date);

                        var siFrequency = (ScheduleFrequency)electronicStatementOrderDTO.ScheduleFrequency;

                        var startDate = electronicStatementOrderDTO.ScheduleExpectedRunDate;

                        if (persisted.Duration.StartDate.Date != electronicStatementOrderDTO.DurationStartDate.Date)
                            startDate = electronicStatementOrderDTO.DurationStartDate.Date;
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

                                electronicStatementOrderDTO.ScheduleExpectedRunDate = startDate.AddDays(1).AddYears(1);

                                if (electronicStatementOrderDTO.ScheduleExpectedRunDate < originalStartDate)
                                    electronicStatementOrderDTO.ScheduleExpectedRunDate = originalStartDate;

                                electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddYears(1), 1, holidays);

                                while (electronicStatementOrderDTO.ScheduleActualRunDate < electronicStatementOrderDTO.ScheduleExpectedRunDate)
                                {
                                    electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(electronicStatementOrderDTO.ScheduleActualRunDate, 1, holidays);
                                }

                                break;
                            case ScheduleFrequency.SemiAnnual:

                                startDate = startDate.AddMonths(-6).AddDays(-1);

                                electronicStatementOrderDTO.ScheduleExpectedRunDate = startDate.AddDays(1).AddMonths(6);

                                if (electronicStatementOrderDTO.ScheduleExpectedRunDate < originalStartDate)
                                    electronicStatementOrderDTO.ScheduleExpectedRunDate = originalStartDate;

                                electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddMonths(6), 1, holidays);

                                while (electronicStatementOrderDTO.ScheduleActualRunDate < electronicStatementOrderDTO.ScheduleExpectedRunDate)
                                {
                                    electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(electronicStatementOrderDTO.ScheduleActualRunDate, 1, holidays);
                                }

                                break;
                            case ScheduleFrequency.TriAnnual:

                                startDate = startDate.AddMonths(-4).AddDays(-1);

                                electronicStatementOrderDTO.ScheduleExpectedRunDate = startDate.AddDays(1).AddMonths(4);

                                if (electronicStatementOrderDTO.ScheduleExpectedRunDate < originalStartDate)
                                    electronicStatementOrderDTO.ScheduleExpectedRunDate = originalStartDate;

                                electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddMonths(4), 1, holidays);

                                while (electronicStatementOrderDTO.ScheduleActualRunDate < electronicStatementOrderDTO.ScheduleExpectedRunDate)
                                {
                                    electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(electronicStatementOrderDTO.ScheduleActualRunDate, 1, holidays);
                                }

                                break;
                            case ScheduleFrequency.Quarterly:

                                startDate = startDate.AddMonths(-3).AddDays(-1);

                                electronicStatementOrderDTO.ScheduleExpectedRunDate = startDate.AddDays(1).AddMonths(3);

                                if (electronicStatementOrderDTO.ScheduleExpectedRunDate < originalStartDate)
                                    electronicStatementOrderDTO.ScheduleExpectedRunDate = originalStartDate;

                                electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddMonths(3), 1, holidays);

                                while (electronicStatementOrderDTO.ScheduleActualRunDate < electronicStatementOrderDTO.ScheduleExpectedRunDate)
                                {
                                    electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(electronicStatementOrderDTO.ScheduleActualRunDate, 1, holidays);
                                }

                                break;
                            case ScheduleFrequency.BiMonthly:

                                startDate = startDate.AddMonths(-2).AddDays(-1);

                                electronicStatementOrderDTO.ScheduleExpectedRunDate = startDate.AddDays(1).AddMonths(2);

                                if (electronicStatementOrderDTO.ScheduleExpectedRunDate < originalStartDate)
                                    electronicStatementOrderDTO.ScheduleExpectedRunDate = originalStartDate;

                                electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddMonths(2), 1, holidays);

                                while (electronicStatementOrderDTO.ScheduleActualRunDate < electronicStatementOrderDTO.ScheduleExpectedRunDate)
                                {
                                    electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(electronicStatementOrderDTO.ScheduleActualRunDate, 1, holidays);
                                }

                                break;
                            case ScheduleFrequency.Monthly:

                                startDate = startDate.AddMonths(-1).AddDays(-1);

                                electronicStatementOrderDTO.ScheduleExpectedRunDate = startDate.AddDays(1).AddMonths(1);

                                if (electronicStatementOrderDTO.ScheduleExpectedRunDate < originalStartDate)
                                    electronicStatementOrderDTO.ScheduleExpectedRunDate = originalStartDate;

                                electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddMonths(1), 1, holidays);

                                while (electronicStatementOrderDTO.ScheduleActualRunDate < electronicStatementOrderDTO.ScheduleExpectedRunDate)
                                {
                                    electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(electronicStatementOrderDTO.ScheduleActualRunDate, 1, holidays);
                                }

                                break;
                            case ScheduleFrequency.SemiMonthly:

                                startDate = startDate.AddDays(-16);

                                electronicStatementOrderDTO.ScheduleExpectedRunDate = startDate.AddDays(16);

                                if (electronicStatementOrderDTO.ScheduleExpectedRunDate < originalStartDate)
                                    electronicStatementOrderDTO.ScheduleExpectedRunDate = originalStartDate;

                                electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddDays(15), 1, holidays);

                                while (electronicStatementOrderDTO.ScheduleActualRunDate < electronicStatementOrderDTO.ScheduleExpectedRunDate)
                                {
                                    electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(electronicStatementOrderDTO.ScheduleActualRunDate, 1, holidays);
                                }

                                break;
                            case ScheduleFrequency.BiWeekly:

                                startDate = startDate.AddDays(-15);

                                electronicStatementOrderDTO.ScheduleExpectedRunDate = startDate.AddDays(15);

                                if (electronicStatementOrderDTO.ScheduleExpectedRunDate < originalStartDate)
                                    electronicStatementOrderDTO.ScheduleExpectedRunDate = originalStartDate;

                                electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddDays(14), 1, holidays);

                                while (electronicStatementOrderDTO.ScheduleActualRunDate < electronicStatementOrderDTO.ScheduleExpectedRunDate)
                                {
                                    electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(electronicStatementOrderDTO.ScheduleActualRunDate, 1, holidays);
                                }

                                break;
                            case ScheduleFrequency.Weekly:

                                startDate = startDate.AddDays(-8);

                                electronicStatementOrderDTO.ScheduleExpectedRunDate = startDate.AddDays(8);

                                if (electronicStatementOrderDTO.ScheduleExpectedRunDate < originalStartDate)
                                    electronicStatementOrderDTO.ScheduleExpectedRunDate = originalStartDate;

                                electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddDays(7), 1, holidays);

                                while (electronicStatementOrderDTO.ScheduleActualRunDate < electronicStatementOrderDTO.ScheduleExpectedRunDate)
                                {
                                    electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(electronicStatementOrderDTO.ScheduleActualRunDate, 1, holidays);
                                }

                                break;
                            case ScheduleFrequency.Daily:

                                startDate = startDate.AddDays(-1);

                                electronicStatementOrderDTO.ScheduleExpectedRunDate = startDate.AddDays(1);

                                if (electronicStatementOrderDTO.ScheduleExpectedRunDate < originalStartDate)
                                    electronicStatementOrderDTO.ScheduleExpectedRunDate = originalStartDate;

                                electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate, 1, holidays);

                                while (electronicStatementOrderDTO.ScheduleActualRunDate < electronicStatementOrderDTO.ScheduleExpectedRunDate)
                                {
                                    electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(electronicStatementOrderDTO.ScheduleActualRunDate, 1, holidays);
                                }

                                break;
                            default:
                                break;
                        }
                    }

                    var duration = new Duration(electronicStatementOrderDTO.DurationStartDate, electronicStatementOrderDTO.DurationEndDate);

                    var schedule = new Schedule(electronicStatementOrderDTO.ScheduleFrequency, electronicStatementOrderDTO.ScheduleExpectedRunDate, electronicStatementOrderDTO.ScheduleActualRunDate, 0, electronicStatementOrderDTO.ScheduleForceExecute);

                    var current = ElectronicStatementOrderFactory.CreateElectronicStatementOrder(electronicStatementOrderDTO.CustomerAccountId, duration, schedule, electronicStatementOrderDTO.Remarks);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    current.CreatedBy = persisted.CreatedBy;

                    if (electronicStatementOrderDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _electronicStatementOrderRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public bool FixSkippedElectronicStatementOrders(DateTime targetDate, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var electronicStatementOrderDTOs = FindSkippedElectronicStatementOrders(targetDate, null, (int)CustomerFilter.Reference1, serviceHeader);

            if (electronicStatementOrderDTOs != null && electronicStatementOrderDTOs.Any())
            {
                electronicStatementOrderDTOs.ForEach(item =>
                {
                    item.ScheduleExecuteAttemptCount = 0;

                    result = UpdateElectronicStatementOrder(item, serviceHeader);
                });
            }

            return result;
        }

        public List<ElectronicStatementOrderDTO> FindElectronicStatementOrders(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var electronicStatementOrders = _electronicStatementOrderRepository.GetAll(serviceHeader);

                if (electronicStatementOrders != null && electronicStatementOrders.Any())
                {
                    return electronicStatementOrders.ProjectedAsCollection<ElectronicStatementOrderDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<ElectronicStatementOrderDTO> FindElectronicStatementOrders(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ElectronicStatementOrderSpecifications.DefaultSpec();

                ISpecification<ElectronicStatementOrder> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var electronicStatementOrderPagedCollection = _electronicStatementOrderRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (electronicStatementOrderPagedCollection != null)
                {
                    var pageCollection = electronicStatementOrderPagedCollection.PageCollection.ProjectedAsCollection<ElectronicStatementOrderDTO>();

                    var itemsCount = electronicStatementOrderPagedCollection.ItemsCount;

                    return new PageCollectionInfo<ElectronicStatementOrderDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<ElectronicStatementOrderDTO> FindElectronicStatementOrders(string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ElectronicStatementOrderSpecifications.ElectronicStatementOrderFullText(text, customerFilter);

                ISpecification<ElectronicStatementOrder> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var electronicStatementOrderPagedCollection = _electronicStatementOrderRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (electronicStatementOrderPagedCollection != null)
                {
                    var pageCollection = electronicStatementOrderPagedCollection.PageCollection.ProjectedAsCollection<ElectronicStatementOrderDTO>();

                    var itemsCount = electronicStatementOrderPagedCollection.ItemsCount;

                    return new PageCollectionInfo<ElectronicStatementOrderDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<ElectronicStatementOrderHistoryDTO> FindElectronicStatementOrderHistory(Guid electronicStatementOrderId, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (electronicStatementOrderId != null && electronicStatementOrderId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = ElectronicStatementOrderHistorySpecifications.ElectronicStatementOrderHistoryWithElectronicStatementOrderId(electronicStatementOrderId);

                    ISpecification<ElectronicStatementOrderHistory> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var electronicStatementOrderHistoryPagedCollection = _electronicStatementOrderHistoryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (electronicStatementOrderHistoryPagedCollection != null)
                    {
                        var pageCollection = electronicStatementOrderHistoryPagedCollection.PageCollection.ProjectedAsCollection<ElectronicStatementOrderHistoryDTO>();

                        var itemsCount = electronicStatementOrderHistoryPagedCollection.ItemsCount;

                        return new PageCollectionInfo<ElectronicStatementOrderHistoryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                    }
                    else return null;
                }
            }
            else return null;
        }

        public ElectronicStatementOrderDTO FindElectronicStatementOrder(Guid electronicStatementOrderId, ServiceHeader serviceHeader)
        {
            if (electronicStatementOrderId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var electronicStatementOrder = _electronicStatementOrderRepository.Get(electronicStatementOrderId, serviceHeader);

                    if (electronicStatementOrder != null)
                    {
                        return electronicStatementOrder.ProjectedAs<ElectronicStatementOrderDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public ElectronicStatementOrderHistoryDTO FindElectronicStatementOrderHistory(Guid electronicStatementOrderHistoryId, ServiceHeader serviceHeader)
        {
            if (electronicStatementOrderHistoryId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var electronicStatementOrderHistory = _electronicStatementOrderHistoryRepository.Get(electronicStatementOrderHistoryId, serviceHeader);

                    if (electronicStatementOrderHistory != null)
                    {
                        return electronicStatementOrderHistory.ProjectedAs<ElectronicStatementOrderHistoryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<ElectronicStatementOrderDTO> FindElectronicStatementOrdersByCustomerId(Guid customerId, int customerAccountTypeProductCode, ServiceHeader serviceHeader)
        {
            if (customerId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = ElectronicStatementOrderSpecifications.ElectronicStatementOrderWithCustomerIdAndCustomerAccountCustomerAccountTypeProductCode(customerId, customerAccountTypeProductCode);

                    ISpecification<ElectronicStatementOrder> spec = filter;

                    var electronicStatementOrders = _electronicStatementOrderRepository.AllMatching(spec, serviceHeader);

                    if (electronicStatementOrders != null && electronicStatementOrders.Any())
                    {
                        return electronicStatementOrders.ProjectedAsCollection<ElectronicStatementOrderDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<ElectronicStatementOrderDTO> FindElectronicStatementOrdersByCustomerAccountId(Guid customerAccountId, ServiceHeader serviceHeader)
        {
            if (customerAccountId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = ElectronicStatementOrderSpecifications.ElectronicStatementOrderWithCustomerAccountId(customerAccountId);

                    ISpecification<ElectronicStatementOrder> spec = filter;

                    var electronicStatementOrders = _electronicStatementOrderRepository.AllMatching(spec, serviceHeader);

                    if (electronicStatementOrders != null && electronicStatementOrders.Any())
                    {
                        return electronicStatementOrders.ProjectedAsCollection<ElectronicStatementOrderDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<ElectronicStatementOrderDTO> FindDueElectronicStatementOrders(DateTime targetDate, int targetDateOption, string searchString, int customerFilter, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ElectronicStatementOrderSpecifications.DueElectronicStatementOrders(targetDate, targetDateOption, searchString, customerFilter);

                ISpecification<ElectronicStatementOrder> spec = filter;

                var electronicStatementOrders = _electronicStatementOrderRepository.AllMatching(spec, serviceHeader);

                if (electronicStatementOrders != null && electronicStatementOrders.Any())
                {
                    return electronicStatementOrders.ProjectedAsCollection<ElectronicStatementOrderDTO>();
                }
                else return null;
            }
        }

        public List<ElectronicStatementOrderDTO> FindSkippedElectronicStatementOrders(DateTime targetDate, string searchString, int customerFilter, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ElectronicStatementOrderSpecifications.SkippedElectronicStatementOrders(targetDate, searchString, customerFilter);

                ISpecification<ElectronicStatementOrder> spec = filter;

                var electronicStatementOrders = _electronicStatementOrderRepository.AllMatching(spec, serviceHeader);

                if (electronicStatementOrders != null && electronicStatementOrders.Any())
                {
                    return electronicStatementOrders.ProjectedAsCollection<ElectronicStatementOrderDTO>();
                }
                else return null;
            }
        }
    }
}
