using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.Services;
using Application.MainBoundedContext.AdministrationModule.Services;
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
        private readonly IEmployeeAppService _employeeAppService;
        private readonly IHolidayAppService _holidayAppService;
        private readonly IBrokerService _brokerService;
        private readonly INavigationItemInRoleAppService _navigationItemInRoleAppService;
        private const int LeaveApplicationModuleCode = 22016;
        private const int LeaveApprovalModuleCode = 22017;
        private const int LeaveRecallModuleCode = 22018;

        public LeaveApplicationAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<LeaveApplication> leaveApplicationRepository,
           ILeaveTypeAppService leaveTypeAppService,
           IEmployeeAppService employeeAppService,
           IHolidayAppService holidayAppService,
           IBrokerService brokerService,
           INavigationItemInRoleAppService navigationItemInRoleAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (leaveApplicationRepository == null)
                throw new ArgumentNullException(nameof(leaveApplicationRepository));

            if (leaveTypeAppService == null)
                throw new ArgumentNullException(nameof(leaveTypeAppService));

            if (employeeAppService == null)
                throw new ArgumentNullException(nameof(employeeAppService));

            if (holidayAppService == null)
                throw new ArgumentNullException(nameof(holidayAppService));

            if (brokerService == null)
                throw new ArgumentNullException(nameof(brokerService));

            if (navigationItemInRoleAppService == null)
                throw new ArgumentNullException(nameof(navigationItemInRoleAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _leaveApplicationRepository = leaveApplicationRepository;
            _leaveTypeAppService = leaveTypeAppService;
            _employeeAppService = employeeAppService;
            _holidayAppService = holidayAppService;
            _brokerService = brokerService;
            _navigationItemInRoleAppService = navigationItemInRoleAppService;
        }

        public LeaveApplicationDTO AddNewLeaveApplication(LeaveApplicationDTO leaveApplicationDTO, ServiceHeader serviceHeader)
        {
            EnsurePermission(LeaveApplicationModuleCode, serviceHeader);
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var leaveType = ValidateAndApplyRequest(leaveApplicationDTO, null, serviceHeader);
                var requestedDays = CalculateWorkingDays(leaveApplicationDTO.DurationStartDate, leaveApplicationDTO.DurationEndDate, leaveType, serviceHeader);
                var currentBalance = CalculateEmployeeLeaveBalance(leaveApplicationDTO.EmployeeId, leaveApplicationDTO.LeaveTypeId, leaveApplicationDTO.DurationStartDate, null, serviceHeader);
                leaveApplicationDTO.Balance = currentBalance - requestedDays;
                if (leaveApplicationDTO.Balance < 0)
                    throw new InvalidOperationException(string.Format("Insufficient leave balance. Available: {0} day(s); requested: {1} day(s).", currentBalance, requestedDays));

                var duration = new Duration(leaveApplicationDTO.DurationStartDate, leaveApplicationDTO.DurationEndDate);

                var leaveApplication = LeaveApplicationFactory.CreateLeaveApplication(leaveApplicationDTO.EmployeeId, leaveApplicationDTO.LeaveTypeId, duration, leaveApplicationDTO.Reason, leaveApplicationDTO.Balance, leaveApplicationDTO.DocumentNumber, leaveApplicationDTO.FileName, leaveApplicationDTO.FileTitle, leaveApplicationDTO.FileDescription, leaveApplicationDTO.FileMIMEType);

                leaveApplication.Status = (int)LeaveApplicationStatus.Pending;

                leaveApplication.CreatedBy = serviceHeader.ApplicationUserName;

                _leaveApplicationRepository.Add(leaveApplication, serviceHeader);

                return dbContextScope.SaveChanges(serviceHeader) >= 0 ? leaveApplication.ProjectedAs<LeaveApplicationDTO>() : null;
            }
        }

        public bool UpdateLeaveApplication(LeaveApplicationDTO leaveApplicationDTO, ServiceHeader serviceHeader)
        {
            EnsurePermission(LeaveApplicationModuleCode, serviceHeader);
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _leaveApplicationRepository.Get(leaveApplicationDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    if (persisted.Status != (byte)LeaveApplicationStatus.Pending)
                        throw new InvalidOperationException("Only a pending leave application can be edited.");
                    leaveApplicationDTO.EmployeeId = persisted.EmployeeId;
                    var leaveType = ValidateAndApplyRequest(leaveApplicationDTO, persisted.Id, serviceHeader);
                    var requestedDays = CalculateWorkingDays(leaveApplicationDTO.DurationStartDate, leaveApplicationDTO.DurationEndDate, leaveType, serviceHeader);
                    var currentBalance = CalculateEmployeeLeaveBalance(persisted.EmployeeId, leaveApplicationDTO.LeaveTypeId, leaveApplicationDTO.DurationStartDate, persisted.Id, serviceHeader);
                    leaveApplicationDTO.Balance = currentBalance - requestedDays;
                    if (leaveApplicationDTO.Balance < 0)
                        throw new InvalidOperationException(string.Format("Insufficient leave balance. Available: {0} day(s); requested: {1} day(s).", currentBalance, requestedDays));

                    var duration = new Duration(leaveApplicationDTO.DurationStartDate, leaveApplicationDTO.DurationEndDate);

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
            EnsurePermission(LeaveApprovalModuleCode, serviceHeader);
            var result = false;
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _leaveApplicationRepository.Get(leaveApplicationDTO.Id, serviceHeader);

                if (persisted == null) return false;
                if (persisted.Status != (byte)LeaveApplicationStatus.Pending)
                    throw new InvalidOperationException("Only a pending leave application can be approved or rejected.");
                if (leaveApplicationDTO.Status != (byte)LeaveApplicationStatus.Approved && leaveApplicationDTO.Status != (byte)LeaveApplicationStatus.Rejected)
                    throw new InvalidOperationException("The authorization decision must be Approved or Rejected.");

                persisted.Status = (byte)leaveApplicationDTO.Status;
                persisted.AuthorizationRemarks = leaveApplicationDTO.AuthorizationRemarks;
                persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                persisted.AuthorizedDate = DateTime.Now;

                if (persisted.Status == (byte)LeaveApplicationStatus.Rejected && persisted.LeaveTypeId.HasValue)
                    persisted.Balance = CalculateEmployeeLeaveBalance(persisted.EmployeeId, persisted.LeaveTypeId.Value, DateTime.Today, persisted.Id, serviceHeader);

                result = dbContextScope.SaveChanges(serviceHeader) >= 0;
            }

            if (result)
            {
                var committed = FindLeaveApplication(leaveApplicationDTO.Id, serviceHeader);
                if (committed != null)
                    _brokerService.ProcessLeaveApprovalAccountAlerts(DMLCommand.None, serviceHeader, committed);
            }

            return result;
        }

        public bool RecallLeaveApplication(LeaveApplicationDTO leaveApplicationDTO, ServiceHeader serviceHeader)
        {
            EnsurePermission(LeaveRecallModuleCode, serviceHeader);
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _leaveApplicationRepository.Get(leaveApplicationDTO.Id, serviceHeader);

                if (persisted == null) return false;
                if (persisted.Status != (byte)LeaveApplicationStatus.Approved)
                    throw new InvalidOperationException("Only an approved leave application can be recalled.");

                persisted.Status = (byte)LeaveApplicationStatus.Recalled;
                persisted.RecallRemarks = leaveApplicationDTO.RecallRemarks;
                persisted.RecalledBy = serviceHeader.ApplicationUserName;
                persisted.RecalledDate = DateTime.Now;
                if (persisted.LeaveTypeId.HasValue)
                    persisted.Balance = CalculateEmployeeLeaveBalance(persisted.EmployeeId, persisted.LeaveTypeId.Value, DateTime.Today, persisted.Id, serviceHeader);

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
            if (employeeId == Guid.Empty || leaveTypeId == Guid.Empty) return 0m;
            using (_dbContextScopeFactory.CreateReadOnly())
                return CalculateEmployeeLeaveBalance(employeeId, leaveTypeId, DateTime.Today, null, serviceHeader);
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

        private LeaveTypeDTO ValidateAndApplyRequest(LeaveApplicationDTO request, Guid? existingId, ServiceHeader serviceHeader)
        {
            if (request == null) throw new InvalidOperationException("Leave application details are required.");
            if (request.EmployeeId == Guid.Empty) throw new InvalidOperationException("An employee is required.");
            if (request.LeaveTypeId == Guid.Empty) throw new InvalidOperationException("A leave type is required.");
            if (string.IsNullOrWhiteSpace(request.Reason)) throw new InvalidOperationException("A reason for leave is required.");
            if (request.DurationStartDate.Date < DateTime.Today) throw new InvalidOperationException("The leave start date cannot be in the past.");
            if (request.DurationEndDate.Date < request.DurationStartDate.Date) throw new InvalidOperationException("The leave end date cannot be earlier than the start date.");

            var employee = _employeeAppService.FindEmployee(request.EmployeeId, serviceHeader);
            if (employee == null) throw new InvalidOperationException("The selected employee could not be found.");
            if (employee.IsLocked) throw new InvalidOperationException("The selected employee is locked and cannot apply for leave.");
            var leaveType = _leaveTypeAppService.FindLeaveType(request.LeaveTypeId, serviceHeader);
            if (leaveType == null) throw new InvalidOperationException("The selected leave type could not be found.");
            if (leaveType.IsLocked) throw new InvalidOperationException("The selected leave type is locked and cannot be used.");
            if (leaveType.UnitType != (byte)LeaveUnitTypes.Weekly && leaveType.UnitType != (byte)LeaveUnitTypes.Monthly && leaveType.UnitType != (byte)LeaveUnitTypes.Yearly)
                throw new InvalidOperationException("The selected leave type has an invalid entitlement cycle.");
            if (leaveType.Entitlement <= 0) throw new InvalidOperationException("The selected leave type must have a positive entitlement.");
            if (leaveType.TargetGender != (byte)LeaveTypeTargetGender.Unknown && leaveType.TargetGender != employee.CustomerIndividualGender)
                throw new InvalidOperationException("The selected leave type is not available for this employee's gender.");

            var overlaps = _leaveApplicationRepository.AllMatching(
                LeaveApplicationSpecifications.OverlappingActiveLeaveApplications(request.EmployeeId, request.DurationStartDate.Date, request.DurationEndDate.Date), serviceHeader);
            if (overlaps != null && overlaps.Any(x => !existingId.HasValue || x.Id != existingId.Value))
                throw new InvalidOperationException("The employee already has a pending or approved leave application overlapping these dates.");

            ApplyLeaveType(request, leaveType);
            return leaveType;
        }

        private static void ApplyLeaveType(LeaveApplicationDTO request, LeaveTypeDTO leaveType)
        {
            request.LeaveTypeDescription = leaveType.Description;
            request.LeaveTypeUnitType = leaveType.UnitType;
            request.LeaveTypeIsAccrued = leaveType.IsAccrued;
            request.LeaveTypeEntitlement = leaveType.Entitlement;
            request.LeaveTypeExcludeHolidays = leaveType.ExcludeHolidays;
            request.LeaveTypeExcludeWeekends = leaveType.ExcludeWeekends;
        }

        private decimal CalculateEmployeeLeaveBalance(Guid employeeId, Guid leaveTypeId, DateTime targetDate, Guid? excludedApplicationId, ServiceHeader serviceHeader)
        {
            var employee = _employeeAppService.FindEmployee(employeeId, serviceHeader);
            if (employee == null) throw new InvalidOperationException("The selected employee could not be found.");
            var leaveType = _leaveTypeAppService.FindLeaveType(leaveTypeId, serviceHeader);
            if (leaveType == null) throw new InvalidOperationException("The selected leave type could not be found.");

            targetDate = targetDate.Date;
            var employmentStart = employee.CreatedDate == default(DateTime) ? targetDate : employee.CreatedDate.Date;
            if (targetDate < employmentStart) employmentStart = targetDate;
            DateTime usageStart;
            DateTime usageEnd;
            decimal entitlement;

            if (leaveType.IsAccrued)
            {
                usageStart = employmentStart;
                usageEnd = DateTime.MaxValue.Date;
                entitlement = leaveType.Entitlement * GetAccruedPeriodCount(employmentStart, targetDate, (LeaveUnitTypes)leaveType.UnitType);
            }
            else
            {
                GetCycleBounds(targetDate, (LeaveUnitTypes)leaveType.UnitType, out usageStart, out usageEnd);
                entitlement = leaveType.Entitlement;
            }

            var applications = _leaveApplicationRepository.AllMatching(
                LeaveApplicationSpecifications.LeaveApplicationsWithEmployeeIdAndLeaveTypeId(employeeId, leaveTypeId), serviceHeader);
            decimal used = 0m;
            if (applications != null)
            {
                foreach (var application in applications.Where(x =>
                    (!excludedApplicationId.HasValue || x.Id != excludedApplicationId.Value) &&
                    (x.Status == (byte)LeaveApplicationStatus.Pending || x.Status == (byte)LeaveApplicationStatus.Approved) &&
                    x.Duration.StartDate.Date >= usageStart && x.Duration.StartDate.Date <= usageEnd))
                {
                    used += CalculateWorkingDays(application.Duration.StartDate, application.Duration.EndDate, leaveType, serviceHeader);
                }
            }
            return Math.Max(0m, entitlement - used);
        }

        private decimal CalculateWorkingDays(DateTime startDate, DateTime endDate, LeaveTypeDTO leaveType, ServiceHeader serviceHeader)
        {
            startDate = startDate.Date;
            endDate = endDate.Date;
            if (endDate < startDate) throw new InvalidOperationException("The leave end date cannot be earlier than the start date.");
            var holidayDates = new HashSet<DateTime>();
            if (leaveType.ExcludeHolidays)
            {
                var holidays = _holidayAppService.FindHolidays(startDate, endDate, serviceHeader) ?? new List<HolidayDTO>();
                foreach (var holiday in holidays.Where(x => !x.IsLocked))
                {
                    var date = holiday.DurationStartDate.Date < startDate ? startDate : holiday.DurationStartDate.Date;
                    var last = holiday.DurationEndDate.Date > endDate ? endDate : holiday.DurationEndDate.Date;
                    while (date <= last) { holidayDates.Add(date); date = date.AddDays(1); }
                }
            }
            decimal days = 0m;
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (leaveType.ExcludeWeekends && (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)) continue;
                if (holidayDates.Contains(date)) continue;
                days += 1m;
            }
            if (days <= 0m) throw new InvalidOperationException("The selected dates contain no chargeable leave days.");
            return days;
        }

        private static int GetAccruedPeriodCount(DateTime startDate, DateTime targetDate, LeaveUnitTypes unitType)
        {
            switch (unitType)
            {
                case LeaveUnitTypes.Weekly: return Math.Max(1, ((targetDate - startDate).Days / 7) + 1);
                case LeaveUnitTypes.Monthly: return Math.Max(1, ((targetDate.Year - startDate.Year) * 12) + targetDate.Month - startDate.Month + 1);
                case LeaveUnitTypes.Yearly: return Math.Max(1, targetDate.Year - startDate.Year + 1);
                default: throw new InvalidOperationException("The leave entitlement cycle is invalid.");
            }
        }

        private static void GetCycleBounds(DateTime targetDate, LeaveUnitTypes unitType, out DateTime startDate, out DateTime endDate)
        {
            switch (unitType)
            {
                case LeaveUnitTypes.Weekly:
                    var offset = ((int)targetDate.DayOfWeek + 6) % 7;
                    startDate = targetDate.AddDays(-offset).Date;
                    endDate = startDate.AddDays(6);
                    break;
                case LeaveUnitTypes.Monthly:
                    startDate = new DateTime(targetDate.Year, targetDate.Month, 1);
                    endDate = startDate.AddMonths(1).AddDays(-1);
                    break;
                case LeaveUnitTypes.Yearly:
                    startDate = new DateTime(targetDate.Year, 1, 1);
                    endDate = new DateTime(targetDate.Year, 12, 31);
                    break;
                default: throw new InvalidOperationException("The leave entitlement cycle is invalid.");
            }
        }

        private void EnsurePermission(int moduleCode, ServiceHeader serviceHeader)
        {
            if (serviceHeader == null) throw new InvalidOperationException("Authenticated caller context is required.");
            var callerRoles = serviceHeader.ApplicationUserRoles ?? new List<string>();
            var grantedRoles = _navigationItemInRoleAppService.GetRolesForNavigationItemCode(moduleCode, serviceHeader) ?? new string[0];
            if (!callerRoles.Any(callerRole => grantedRoles.Any(grantedRole =>
                string.Equals(callerRole, grantedRole, StringComparison.OrdinalIgnoreCase))))
                throw new InvalidOperationException("Access denied: your role is not authorized for this leave operation.");
        }
    }
}
