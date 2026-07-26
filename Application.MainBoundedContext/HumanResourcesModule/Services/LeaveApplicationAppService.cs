using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.LeaveApplicationAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Adapter;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public class LeaveApplicationAppService : ILeaveApplicationAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<LeaveApplication> _leaveApplicationRepository;
        private readonly ILeaveTypeAppService _leaveTypeAppService;
        private readonly IHolidayAppService _holidayAppService;
        private readonly IBrokerService _brokerService;

        public LeaveApplicationAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<LeaveApplication> leaveApplicationRepository,
           ILeaveTypeAppService leaveTypeAppService,
           IHolidayAppService holidayAppService,
           IBrokerService brokerService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (leaveApplicationRepository == null)
                throw new ArgumentNullException(nameof(leaveApplicationRepository));

            if (leaveTypeAppService == null)
                throw new ArgumentNullException(nameof(leaveTypeAppService));

            if (holidayAppService == null)
                throw new ArgumentNullException(nameof(holidayAppService));

            if (brokerService == null)
                throw new ArgumentNullException(nameof(brokerService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _leaveApplicationRepository = leaveApplicationRepository;
            _leaveTypeAppService = leaveTypeAppService;
            _holidayAppService = holidayAppService;
            _brokerService = brokerService;
        }

        public LeaveApplicationDTO AddNewLeaveApplication(LeaveApplicationDTO leaveApplicationDTO, ServiceHeader serviceHeader)
        {
            var leaveApplicationBindingModel = leaveApplicationDTO.ProjectedAs<LeaveApplicationBindingModel>();

            leaveApplicationBindingModel.ValidateAll();

            if (leaveApplicationBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, leaveApplicationBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                if (leaveApplicationDTO.DurationStartDate < DateTime.Today)
                    throw new InvalidOperationException("The start date must not be less than today!");
                else if (leaveApplicationDTO.DurationStartDate > leaveApplicationDTO.DurationEndDate)
                    throw new InvalidOperationException("The start date should not be greater than end date!");

                if (leaveApplicationDTO.Balance < 0)
                {
                    throw new InvalidOperationException("The duration applied  should not be greater than the available balance");
                }

                var duration = new Duration(leaveApplicationDTO.DurationStartDate, leaveApplicationDTO.DurationEndDate);

                #region Exclude weekends?

                if (leaveApplicationDTO.LeaveTypeExcludeWeekends)
                {
                    var date = leaveApplicationDTO.DurationStartDate;

                    while (date < leaveApplicationDTO.DurationEndDate)
                    {
                        date = date.AddDays(1);

                        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                        {
                            leaveApplicationDTO.Balance += 1;
                        }
                    }
                }

                #endregion

                #region Exclude holidays?

                if (leaveApplicationDTO.LeaveTypeExcludeHolidays)
                {
                    var holidays = _holidayAppService.FindHolidays(leaveApplicationDTO.DurationStartDate, leaveApplicationDTO.DurationEndDate, serviceHeader);

                    if (holidays != null)
                    {
                        foreach (var holiday in holidays)
                        {
                            if (!holiday.IsLocked)
                            {
                                var holidayDuration = holiday.DurationEndDate.AddDays(1) - holiday.DurationStartDate;

                                leaveApplicationDTO.Balance += decimal.Parse(holidayDuration.ToString());

                                //case holiday duration is within a weekend, then deduct balance 

                                var date = holiday.DurationStartDate;

                                while (date < holiday.DurationEndDate)
                                {
                                    date = date.AddDays(1);

                                    if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                                    {
                                        leaveApplicationDTO.Balance -= 1;
                                    }
                                }
                            }
                        }
                    }
                }

                #endregion

                var leaveApplication = LeaveApplicationFactory.CreateLeaveApplication(leaveApplicationDTO.EmployeeId, leaveApplicationDTO.LeaveTypeId, duration, leaveApplicationDTO.Reason, leaveApplicationDTO.Balance, leaveApplicationDTO.DocumentNumber, leaveApplicationDTO.FileName, leaveApplicationDTO.FileTitle, leaveApplicationDTO.FileDescription, leaveApplicationDTO.FileMIMEType);

                leaveApplication.Status = (int)LeaveApplicationStatus.Pending;

                leaveApplication.CreatedBy = serviceHeader.ApplicationUserName;

                _leaveApplicationRepository.Add(leaveApplication, serviceHeader);

                return dbContextScope.SaveChanges(serviceHeader) >= 0 ? leaveApplication.ProjectedAs<LeaveApplicationDTO>() : null;
            }
        }

        public bool UpdateLeaveApplication(LeaveApplicationDTO leaveApplicationDTO, ServiceHeader serviceHeader)
        {
            var leaveApplicationBindingModel = leaveApplicationDTO.ProjectedAs<LeaveApplicationBindingModel>();

            leaveApplicationBindingModel.ValidateAll();

            if (leaveApplicationBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, leaveApplicationBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _leaveApplicationRepository.Get(leaveApplicationDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    if (persisted.Duration.StartDate != leaveApplicationDTO.DurationStartDate && leaveApplicationDTO.DurationStartDate < DateTime.Today)
                        throw new InvalidOperationException("The start date must not be less than today!");
                    else if (leaveApplicationDTO.DurationStartDate > leaveApplicationDTO.DurationEndDate)
                        throw new InvalidOperationException("The start date should not be greater than end date!");

                    if (leaveApplicationDTO.Balance < 0)
                    {
                        throw new InvalidOperationException("The duration applied  should not be greater than the available balance");
                    }

                    var duration = new Duration(leaveApplicationDTO.DurationStartDate, leaveApplicationDTO.DurationEndDate);

                    #region Exclude weekends?

                    if (leaveApplicationDTO.LeaveTypeExcludeWeekends)
                    {
                        var date = leaveApplicationDTO.DurationStartDate;

                        while (date < leaveApplicationDTO.DurationEndDate)
                        {
                            date = date.AddDays(1);

                            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                            {
                                leaveApplicationDTO.Balance += 1;
                            }
                        }
                    }

                    #endregion

                    #region Exclude holidays?

                    if (leaveApplicationDTO.LeaveTypeExcludeHolidays)
                    {
                        var holidays = _holidayAppService.FindHolidays(leaveApplicationDTO.DurationStartDate, leaveApplicationDTO.DurationEndDate, serviceHeader);

                        if (holidays != null)
                        {
                            foreach (var holiday in holidays)
                            {
                                if (!holiday.IsLocked)
                                {
                                    var holidayDuration = holiday.DurationEndDate.AddDays(1) - holiday.DurationStartDate;

                                    leaveApplicationDTO.Balance += decimal.Parse(holidayDuration.ToString());

                                    //case holiday duration is within a weekend, then deduct balance 

                                    var date = holiday.DurationStartDate;

                                    while (date < holiday.DurationEndDate)
                                    {
                                        date = date.AddDays(1);

                                        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                                        {
                                            leaveApplicationDTO.Balance -= 1;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                    var current = LeaveApplicationFactory.CreateLeaveApplication(persisted.EmployeeId, leaveApplicationDTO.LeaveTypeId, duration, leaveApplicationDTO.Reason, leaveApplicationDTO.Balance, leaveApplicationDTO.DocumentNumber, leaveApplicationDTO.FileName, leaveApplicationDTO.FileTitle, leaveApplicationDTO.FileDescription, leaveApplicationDTO.FileMIMEType);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    current.Status = persisted.Status;
                    current.CreatedBy = persisted.CreatedBy;

                    _leaveApplicationRepository.Merge(persisted, current, serviceHeader);
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public bool AuthorizeLeaveApplication(LeaveApplicationDTO leaveApplicationDTO, ServiceHeader serviceHeader)
        {
            var leaveApplicationBindingModel = leaveApplicationDTO.ProjectedAs<LeaveApplicationBindingModel>();

            leaveApplicationBindingModel.ValidateAll();

            if (leaveApplicationBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, leaveApplicationBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _leaveApplicationRepository.Get(leaveApplicationDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    persisted.Status = (byte)leaveApplicationDTO.Status;
                    persisted.AuthorizationRemarks = leaveApplicationDTO.AuthorizationRemarks;
                    persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                    persisted.AuthorizedDate = DateTime.Now;
                }

                _brokerService.ProcessLeaveApprovalAccountAlerts(DMLCommand.None, serviceHeader, leaveApplicationDTO);

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public bool RecallLeaveApplication(LeaveApplicationDTO leaveApplicationDTO, ServiceHeader serviceHeader)
        {
            var leaveApplicationBindingModel = leaveApplicationDTO.ProjectedAs<LeaveApplicationBindingModel>();

            leaveApplicationBindingModel.ValidateAll();

            if (leaveApplicationBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, leaveApplicationBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _leaveApplicationRepository.Get(leaveApplicationDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    persisted.Status = (byte)LeaveApplicationStatus.Recalled;
                    persisted.RecallRemarks = leaveApplicationDTO.RecallRemarks;
                    persisted.RecalledBy = serviceHeader.ApplicationUserName;
                    persisted.RecalledDate = DateTime.Now;
                    persisted.Balance = persisted.Balance + (decimal)(leaveApplicationDTO.DurationEndDate - leaveApplicationDTO.DurationStartDate).TotalDays; // return balance
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public List<LeaveApplicationDTO> FindLeaveApplications(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _leaveApplicationRepository.GetAll<LeaveApplicationDTO>(serviceHeader);
            }
        }

        public List<LeaveApplicationDTO> FindActiveLeaveApplications(Guid employeeId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = LeaveApplicationSpecifications.ActiveLeaveApplicationWithEmployeeId(employeeId);

                ISpecification<LeaveApplication> spec = filter;

                return _leaveApplicationRepository.AllMatching<LeaveApplicationDTO>(spec, serviceHeader);
            }
        }

        public List<LeaveApplicationDTO> FindLeaveApplicationsByEmployeeId(Guid employeeId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = LeaveApplicationSpecifications.ActiveLeaveApplicationWithEmployeeId(employeeId);

                ISpecification<LeaveApplication> spec = filter;

                return _leaveApplicationRepository.AllMatching<LeaveApplicationDTO>(spec, serviceHeader);
            }
        }

        public List<LeaveApplicationDTO> FindLeaveApplicationsByEmployeeIdAndLeaveTypeId(Guid employeeId, Guid leaveTypeId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = LeaveApplicationSpecifications.LeaveApplicationsWithEmployeeIdAndLeaveTypeId(employeeId, leaveTypeId);

                ISpecification<LeaveApplication> spec = filter;

                return _leaveApplicationRepository.AllMatching<LeaveApplicationDTO>(spec, serviceHeader);
            }
        }

        public PageCollectionInfo<LeaveApplicationDTO> FindLeaveApplications(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = LeaveApplicationSpecifications.DefaultSpec();

                ISpecification<LeaveApplication> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return _leaveApplicationRepository.AllMatchingPaged<LeaveApplicationDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public decimal FindEmployeeLeaveBalances(Guid employeeId, Guid leaveTypeId, ServiceHeader serviceHeader)
        {
            decimal result = 0m;

            if (employeeId != Guid.Empty && leaveTypeId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var leaveApplication = FindLeaveApplicationsByEmployeeIdAndLeaveTypeId(employeeId, leaveTypeId, serviceHeader).OrderByDescending(m => m.CreatedDate).FirstOrDefault(); ;

                    if (leaveApplication != null)
                    {
                        switch ((LeaveUnitTypes)leaveApplication.LeaveTypeUnitType)
                        {
                            case LeaveUnitTypes.Weekly:
                                if (leaveApplication.LeaveTypeIsAccrued)
                                {
                                    #region Accrue entitlement balance per week

                                    var period = Math.Round(((DateTime.Today - leaveApplication.CreatedDate).TotalDays / 7), 0);

                                    if (period > 1)
                                    {
                                        for (int i = 1; i <= period; i++)
                                        {
                                            result = leaveApplication.Balance + leaveApplication.LeaveTypeEntitlement;
                                        }
                                    }
                                    else
                                    {
                                        result = leaveApplication.Balance;
                                    }

                                    #endregion
                                }
                                else
                                {
                                    var period = Math.Round(((DateTime.Today - leaveApplication.CreatedDate).TotalDays / 7), 0);

                                    if (period > 1)
                                    {
                                        result = leaveApplication.LeaveTypeEntitlement;
                                    }
                                    else
                                    {
                                        result = leaveApplication.Balance;
                                    }
                                }

                                break;

                            case LeaveUnitTypes.Monthly:
                                if (leaveApplication.LeaveTypeIsAccrued)
                                {
                                    #region Accrue entitlement balance per month

                                    var period = ((DateTime.Today.Year - leaveApplication.CreatedDate.Year) * 12) + DateTime.Today.Month - leaveApplication.CreatedDate.Month;

                                    if (period > 1)
                                    {
                                        for (int i = 1; i <= period; i++)
                                        {
                                            result = leaveApplication.Balance + leaveApplication.LeaveTypeEntitlement;
                                        }
                                    }
                                    else
                                    {
                                        result = leaveApplication.Balance;
                                    }

                                    #endregion
                                }
                                else
                                {
                                    var period = ((DateTime.Today.Year - leaveApplication.CreatedDate.Year) * 12) + DateTime.Today.Month - leaveApplication.CreatedDate.Month;

                                    if (period > 1)
                                    {
                                        result = leaveApplication.LeaveTypeEntitlement;
                                    }
                                    else
                                    {
                                        result = leaveApplication.Balance;
                                    }
                                }

                                break;

                            case LeaveUnitTypes.Yearly:

                                if (leaveApplication.LeaveTypeIsAccrued)
                                {
                                    #region Accrue entitlement balance per year

                                    var period = DateTime.Today.Year - leaveApplication.CreatedDate.Year;

                                    if (period > 1)
                                    {
                                        for (int i = 1; i <= period; i++)
                                        {
                                            result = leaveApplication.Balance + leaveApplication.LeaveTypeEntitlement;
                                        }
                                    }
                                    else
                                    {
                                        result = leaveApplication.Balance;
                                    }

                                    #endregion
                                }
                                else
                                {
                                    var period = DateTime.Today.Year - leaveApplication.CreatedDate.Year;

                                    if (period > 1)
                                    {
                                        result = leaveApplication.LeaveTypeEntitlement;
                                    }
                                    else
                                    {
                                        result = leaveApplication.Balance;
                                    }
                                }
                                break;

                            default:
                                break;
                        }
                    }

                    else
                    {
                        var leaveType = _leaveTypeAppService.FindLeaveType(leaveTypeId, serviceHeader);

                        result = leaveType.Entitlement;
                    }
                }
            }

            return result;
        }

        public PageCollectionInfo<LeaveApplicationDTO> FindLeaveApplications(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = string.IsNullOrWhiteSpace(text) ? LeaveApplicationSpecifications.DefaultSpec() : LeaveApplicationSpecifications.LeaveApplicationFullText(text);

                ISpecification<LeaveApplication> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return _leaveApplicationRepository.AllMatchingPaged<LeaveApplicationDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public PageCollectionInfo<LeaveApplicationDTO> FindLeaveApplications(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = LeaveApplicationSpecifications.LeaveApplicationsWithDateRangeAndStatus(startDate, endDate, status, text);

                ISpecification<LeaveApplication> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return _leaveApplicationRepository.AllMatchingPaged<LeaveApplicationDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public LeaveApplicationDTO FindLeaveApplication(Guid leaveApplicationId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _leaveApplicationRepository.Get<LeaveApplicationDTO>(leaveApplicationId, serviceHeader);
            }
        }
    }
}
