using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.Seedwork;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.HolidayAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public class HolidayAppService : IHolidayAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<Holiday> _holidayRepository;
        private readonly IPostingPeriodAppService _postingPeriodAppService;

        public HolidayAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<Holiday> holidayRepository,
           IPostingPeriodAppService postingPeriodAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (holidayRepository == null)
                throw new ArgumentNullException(nameof(holidayRepository));

            if (postingPeriodAppService == null)
                throw new ArgumentNullException(nameof(postingPeriodAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _holidayRepository = holidayRepository;
            _postingPeriodAppService = postingPeriodAppService;
        }

        public HolidayDTO AddNewHoliday(HolidayDTO holidayDTO, ServiceHeader serviceHeader)
        {
            if (holidayDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var duration = new Duration(holidayDTO.DurationStartDate, holidayDTO.DurationEndDate);

                    var holiday = HolidayFactory.CreateHoliday(holidayDTO.PostingPeriodId, holidayDTO.Description, duration);

                    if (holidayDTO.IsLocked)
                        holiday.Lock();
                    else holiday.UnLock();

                    _holidayRepository.Add(holiday, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return holiday.ProjectedAs<HolidayDTO>();
                }
            }
            else return null;
        }

        public bool UpdateHoliday(HolidayDTO holidayDTO, ServiceHeader serviceHeader)
        {
            if (holidayDTO == null || holidayDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _holidayRepository.Get(holidayDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var duration = new Duration(holidayDTO.DurationStartDate, holidayDTO.DurationEndDate);

                    var current = HolidayFactory.CreateHoliday(persisted.PostingPeriodId, holidayDTO.Description, duration);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                    if (holidayDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _holidayRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public bool RemoveHoliday(Guid holidayId, ServiceHeader serviceHeader)
        {
            if (holidayId == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _holidayRepository.Get(holidayId, serviceHeader);

                if (persisted != null)
                {
                    _holidayRepository.Remove(persisted, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<HolidayDTO> FindHolidays(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var holidays = _holidayRepository.GetAll(serviceHeader);

                if (holidays != null && holidays.Any())
                {
                    return holidays.ProjectedAsCollection<HolidayDTO>();
                }
                else return null;
            }
        }

        public List<HolidayDTO> FindHolidays(Guid postingPeriodId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = HolidaySpecifications.HolidayWithPostingPeriodId(postingPeriodId);

                ISpecification<Holiday> spec = filter;

                var holidays = _holidayRepository.AllMatching(spec, serviceHeader);

                if (holidays != null && holidays.Any())
                {
                    return holidays.ProjectedAsCollection<HolidayDTO>();
                }
                else return null;
            }
        }

        public List<HolidayDTO> FindHolidaysInCurrentPostingPeriod(ServiceHeader serviceHeader)
        {
            var postingPeriod = _postingPeriodAppService.FindCurrentPostingPeriod(serviceHeader);

            if (postingPeriod != null)
            {
                return FindHolidays(postingPeriod.Id, serviceHeader);
            }
            else return null;
        }

        public PageCollectionInfo<HolidayDTO> FindHolidays(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = HolidaySpecifications.DefaultSpec();

                ISpecification<Holiday> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var holidayPagedCollection = _holidayRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (holidayPagedCollection != null)
                {
                    var pageCollection = holidayPagedCollection.PageCollection.ProjectedAsCollection<HolidayDTO>();

                    var itemsCount = holidayPagedCollection.ItemsCount;

                    return new PageCollectionInfo<HolidayDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<HolidayDTO> FindHolidays(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = string.IsNullOrWhiteSpace(text) ? HolidaySpecifications.DefaultSpec() : HolidaySpecifications.HolidayFullText(text);

                ISpecification<Holiday> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var holidayPagedCollection = _holidayRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (holidayPagedCollection != null)
                {
                    var pageCollection = holidayPagedCollection.PageCollection.ProjectedAsCollection<HolidayDTO>();

                    var itemsCount = holidayPagedCollection.ItemsCount;

                    return new PageCollectionInfo<HolidayDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else
                    return null;
            }
        }

        public HolidayDTO FindHoliday(Guid holidayId, ServiceHeader serviceHeader)
        {
            if (holidayId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var holiday = _holidayRepository.Get(holidayId, serviceHeader);

                    if (holiday != null)
                    {
                        return holiday.ProjectedAs<HolidayDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public DateTime? FindBusinessDay(int addValue, bool nextDay, ServiceHeader serviceHeader)
        {
            var holidays = new List<DateTime>();

            var holidayDTOs = FindHolidaysInCurrentPostingPeriod(serviceHeader);

            var dateRanges = (from h in holidayDTOs ?? new List<HolidayDTO>() select new { h.DurationStartDate, h.DurationEndDate });

            foreach (var item in dateRanges)
                for (DateTime date = item.DurationStartDate; date <= item.DurationEndDate; date = date.AddDays(1))
                    holidays.Add(date);

            var baseDay = DateTime.Today;

            if (addValue == 0)
            {
                if (!nextDay && baseDay.DayOfWeek.In(DayOfWeek.Saturday, DayOfWeek.Sunday))
                    nextDay = true;
                else
                {
                    baseDay = baseDay.AddDays(-1);

                    nextDay = true;
                }
            }

            var workingDay = baseDay.AddDays(Math.Abs(addValue));

            var businessDay = UberUtil.GetBusinessDay(workingDay, nextDay ? 1 : -1, holidays);

            return businessDay;
        }

        public List<HolidayDTO> FindHolidays(DateTime durationStartDate, DateTime durationEndDate, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = HolidaySpecifications.HolidayWithinDurationDates(durationStartDate, durationEndDate);

                ISpecification<Holiday> spec = filter;

                var holidays = _holidayRepository.AllMatching(spec, serviceHeader);

                if (holidays != null && holidays.Any())
                {
                    return holidays.ProjectedAsCollection<HolidayDTO>();
                }
                else return null;
            }
        }
    }
}
