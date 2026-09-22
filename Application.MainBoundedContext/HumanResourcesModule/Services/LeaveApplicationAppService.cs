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
            if (leaveApplicationDTO == null) throw new InvalidOperationException("Leave application details are required.");
            using (var dbContextScope = _dbContextScopeFactory.CreateWithTransaction(System.Data.IsolationLevel.Serializable))
            {
                LockEmployee(leaveApplicationDTO.EmployeeId, serviceHeader);
                var leaveType = ValidateAndApplyRequest(leaveApplicationDTO, null, serviceHeader);
                var preview = PreviewLeave(leaveApplicationDTO.EmployeeId, leaveApplicationDTO.LeaveTypeId, leaveApplicationDTO.DurationStartDate, leaveApplicationDTO.DurationEndDate, null, serviceHeader);
                if (!preview.CanSubmit) throw new InvalidOperationException(preview.Error);
                leaveApplicationDTO.Balance = preview.Cycles.Min(x => x.Remaining);
                var duration = new Duration(leaveApplicationDTO.DurationStartDate, leaveApplicationDTO.DurationEndDate);

                var leaveApplication = LeaveApplicationFactory.CreateLeaveApplication(leaveApplicationDTO.EmployeeId, leaveApplicationDTO.LeaveTypeId, duration, leaveApplicationDTO.Reason, leaveApplicationDTO.Balance, leaveApplicationDTO.DocumentNumber, leaveApplicationDTO.FileName, leaveApplicationDTO.FileTitle, leaveApplicationDTO.FileDescription, leaveApplicationDTO.FileMIMEType);

                leaveApplication.ChargedDates = LeaveCalendar.Encode(GetChargeDates(leaveApplicationDTO.DurationStartDate, leaveApplicationDTO.DurationEndDate, leaveType, serviceHeader));
                leaveApplication.NotificationPending = true;
                leaveApplication.Status = (int)LeaveApplicationStatus.Pending;

                leaveApplication.CreatedBy = serviceHeader.ApplicationUserName;

                _leaveApplicationRepository.Add(leaveApplication, serviceHeader);

                return dbContextScope.SaveChanges(serviceHeader) >= 0 ? leaveApplication.ProjectedAs<LeaveApplicationDTO>() : null;
            }
        }

        public bool UpdateLeaveApplication(LeaveApplicationDTO leaveApplicationDTO, ServiceHeader serviceHeader)
        {
            EnsurePermission(LeaveApplicationModuleCode, serviceHeader);
            using (var dbContextScope = _dbContextScopeFactory.CreateWithTransaction(System.Data.IsolationLevel.Serializable))
            {
                var persisted = _leaveApplicationRepository.Get(leaveApplicationDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    LockEmployee(persisted.EmployeeId, serviceHeader);
                    if (persisted.Status != (byte)LeaveApplicationStatus.Pending)
                        throw new InvalidOperationException("Only a pending leave application can be edited.");
                    leaveApplicationDTO.EmployeeId = persisted.EmployeeId;
                    var leaveType = ValidateAndApplyRequest(leaveApplicationDTO, persisted.Id, serviceHeader);
                    var preview = PreviewLeave(persisted.EmployeeId, leaveApplicationDTO.LeaveTypeId, leaveApplicationDTO.DurationStartDate, leaveApplicationDTO.DurationEndDate, persisted.Id, serviceHeader);
                    if (!preview.CanSubmit) throw new InvalidOperationException(preview.Error);
                    leaveApplicationDTO.Balance = preview.Cycles.Min(x => x.Remaining);
                    var duration = new Duration(leaveApplicationDTO.DurationStartDate, leaveApplicationDTO.DurationEndDate);

                    var current = LeaveApplicationFactory.CreateLeaveApplication(persisted.EmployeeId, leaveApplicationDTO.LeaveTypeId, duration, leaveApplicationDTO.Reason, leaveApplicationDTO.Balance, leaveApplicationDTO.DocumentNumber, leaveApplicationDTO.FileName, leaveApplicationDTO.FileTitle, leaveApplicationDTO.FileDescription, leaveApplicationDTO.FileMIMEType);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    current.ChargedDates = persisted.LeaveTypeId == leaveApplicationDTO.LeaveTypeId && persisted.Duration.StartDate.Date == leaveApplicationDTO.DurationStartDate.Date && persisted.Duration.EndDate.Date == leaveApplicationDTO.DurationEndDate.Date ? persisted.ChargedDates : LeaveCalendar.Encode(GetChargeDates(leaveApplicationDTO.DurationStartDate, leaveApplicationDTO.DurationEndDate, leaveType, serviceHeader));
                    current.NotificationPending = true;
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
            using (var dbContextScope = _dbContextScopeFactory.CreateWithTransaction(System.Data.IsolationLevel.Serializable))
            {
                var persisted = _leaveApplicationRepository.Get(leaveApplicationDTO.Id, serviceHeader);

                if (persisted == null) return false;
                LockEmployee(persisted.EmployeeId, serviceHeader);
                if (persisted.Status != (byte)LeaveApplicationStatus.Pending)
                    throw new InvalidOperationException("Only a pending leave application can be approved or rejected.");
                if (leaveApplicationDTO.Status != (byte)LeaveApplicationStatus.Approved && leaveApplicationDTO.Status != (byte)LeaveApplicationStatus.Rejected)
                    throw new InvalidOperationException("The authorization decision must be Approved or Rejected.");

                if (string.Equals(persisted.CreatedBy, serviceHeader.ApplicationUserName, StringComparison.OrdinalIgnoreCase) || serviceHeader.ApplicationUserEmployeeId == persisted.EmployeeId)
                    throw new InvalidOperationException("Another authorized user must decide this leave application.");
                if (leaveApplicationDTO.Status == (byte)LeaveApplicationStatus.Approved)
                {
                    var request = new LeaveApplicationDTO { EmployeeId = persisted.EmployeeId, LeaveTypeId = persisted.LeaveTypeId ?? Guid.Empty, DurationStartDate = persisted.Duration.StartDate, DurationEndDate = persisted.Duration.EndDate, Reason = persisted.Reason };
                    ValidateAndApplyRequest(request, persisted.Id, serviceHeader, true);
                    var preview = PreviewLeave(request.EmployeeId, request.LeaveTypeId, request.DurationStartDate, request.DurationEndDate, persisted.Id, serviceHeader);
                    if (!preview.CanSubmit) throw new InvalidOperationException(preview.Error);
                    persisted.Balance = preview.Cycles.Min(x => x.Remaining);
                }
                persisted.NotificationPending = true;
                persisted.Status = (byte)leaveApplicationDTO.Status;
                persisted.AuthorizationRemarks = leaveApplicationDTO.AuthorizationRemarks;
                persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                persisted.AuthorizedDate = DateTime.Now;

                if (persisted.Status == (byte)LeaveApplicationStatus.Rejected && persisted.LeaveTypeId.HasValue)
                    persisted.Balance = CalculateEmployeeLeaveBalance(persisted.EmployeeId, persisted.LeaveTypeId.Value, DateTime.Today, persisted.Id, serviceHeader);

                result = dbContextScope.SaveChanges(serviceHeader) >= 0;
            }

            if (result) RetryLeaveNotification(leaveApplicationDTO.Id, serviceHeader);

            return result;
        }

        public bool RecallLeaveApplication(LeaveApplicationDTO leaveApplicationDTO, ServiceHeader serviceHeader)
        {
            EnsurePermission(LeaveRecallModuleCode, serviceHeader);
            using (var dbContextScope = _dbContextScopeFactory.CreateWithTransaction(System.Data.IsolationLevel.Serializable))
            {
                var persisted = _leaveApplicationRepository.Get(leaveApplicationDTO.Id, serviceHeader);

                if (persisted == null) return false;
                LockEmployee(persisted.EmployeeId, serviceHeader);
                if (persisted.Status != (byte)LeaveApplicationStatus.Approved)
                    throw new InvalidOperationException("Only an approved leave application can be recalled.");

                if (!leaveApplicationDTO.EffectiveReturnDate.HasValue) throw new InvalidOperationException("Select the effective return-to-work date.");
                var returnDate = leaveApplicationDTO.EffectiveReturnDate.Value.Date;
                if (returnDate < DateTime.Today || returnDate < persisted.Duration.StartDate.Date || returnDate > persisted.Duration.EndDate.Date)
                    throw new InvalidOperationException("The return date must be today or later and within the approved leave dates. Completed leave requires an HR correction, not recall.");
                persisted.EffectiveReturnDate = returnDate;
                persisted.Status = (byte)LeaveApplicationStatus.Recalled;
                persisted.RecallRemarks = leaveApplicationDTO.RecallRemarks;
                persisted.RecalledBy = serviceHeader.ApplicationUserName;
                persisted.RecalledDate = DateTime.Now;
                if (persisted.LeaveTypeId.HasValue)
                    persisted.Balance = CalculateEmployeeLeaveBalance(persisted.EmployeeId, persisted.LeaveTypeId.Value, returnDate, null, serviceHeader);

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
                var filter = LeaveApplicationSpecifications.LeaveApplicationsByEmployeeId(employeeId);

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

        private LeaveTypeDTO ValidateAndApplyRequest(LeaveApplicationDTO request, Guid? existingId, ServiceHeader serviceHeader, bool approving = false)
        {
            if (request == null) throw new InvalidOperationException("Leave application details are required.");
            if (request.EmployeeId == Guid.Empty) throw new InvalidOperationException("An employee is required.");
            if (request.LeaveTypeId == Guid.Empty) throw new InvalidOperationException("A leave type is required.");
            if (string.IsNullOrWhiteSpace(request.Reason)) throw new InvalidOperationException("A reason for leave is required.");
            if (!approving && request.DurationStartDate.Date < DateTime.Today) throw new InvalidOperationException("The leave start date cannot be in the past.");
            if (request.DurationEndDate.Date < request.DurationStartDate.Date) throw new InvalidOperationException("The leave end date cannot be earlier than the start date.");

            var employee = _employeeAppService.FindEmployee(request.EmployeeId, serviceHeader);
            if (employee == null) throw new InvalidOperationException("The selected employee could not be found.");
            if (employee.EmploymentStartDate.HasValue && request.DurationStartDate.Date < employee.EmploymentStartDate.Value.Date) throw new InvalidOperationException("Leave cannot begin before employment starts.");
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

        private void LockEmployee(Guid employeeId, ServiceHeader header)
        {
            var result = _leaveApplicationRepository.DatabaseSqlQuery<int>(
                "DECLARE @result int; EXEC @result = sys.sp_getapplock @Resource=@resource, @LockMode='Exclusive', @LockOwner='Transaction', @LockTimeout=10000; SELECT @result;",
                header, new System.Data.SqlClient.SqlParameter("@resource", "employee-leave:" + employeeId.ToString("D"))).Single();
            if (result < 0) throw new InvalidOperationException("Another leave operation is in progress for this employee. Please retry.");
        }

        private List<DateTime> GetChargeDates(DateTime start, DateTime end, LeaveTypeDTO policy, ServiceHeader header)
        {
            var holidays = policy.ExcludeHolidays ? _holidayAppService.FindHolidays(start.Date, end.Date, header) : new List<HolidayDTO>();
            return LeaveCalendar.ChargeDates(start, end, policy, holidays);
        }

        private List<DateTime> ConsumedDates(LeaveApplication application)
        {
            if (application.Status != (byte)LeaveApplicationStatus.Pending && application.Status != (byte)LeaveApplicationStatus.Approved && application.Status != (byte)LeaveApplicationStatus.Recalled)
                return new List<DateTime>();
            if (application.Status == (byte)LeaveApplicationStatus.Recalled && !application.EffectiveReturnDate.HasValue) return new List<DateTime>(); // historical cancellations
            if (application.ChargedDates == null) throw new InvalidOperationException("This employee has legacy leave awaiting charged-day migration. Ask an administrator to apply the leave schema update.");
            return LeaveCalendar.Decode(application.ChargedDates).Where(x => !application.EffectiveReturnDate.HasValue || x < application.EffectiveReturnDate.Value.Date).ToList();
        }

        private LeaveCycleBalanceDTO GetCycleBalance(EmployeeDTO employee, LeaveTypeDTO policy, DateTime date, Guid? excludedId, ServiceHeader header, List<LeaveApplication> loadedApplications = null)
        {
            var start = policy.IsAccrued ? (employee.EmploymentStartDate ?? date).Date : LeaveCalendar.CycleStart(date, policy.UnitType);
            var end = policy.IsAccrued ? DateTime.MaxValue.Date : LeaveCalendar.CycleEnd(start, policy.UnitType);
            var result = new LeaveCycleBalanceDTO { Start = start, End = end, Entitlement = LeaveCalendar.Entitlement(policy, employee.EmploymentStartDate, date) };
            var applications = loadedApplications ?? _leaveApplicationRepository.AllMatching(LeaveApplicationSpecifications.LeaveApplicationsWithEmployeeIdAndLeaveTypeId(employee.Id, policy.Id), header) ?? new List<LeaveApplication>();
            foreach (var application in applications.Where(x => !excludedId.HasValue || x.Id != excludedId.Value))
            {
                var count = ConsumedDates(application).Count(x => x >= start && x <= end);
                if (application.Status == (byte)LeaveApplicationStatus.Pending) result.Reserved += count;
                else result.Used += count;
            }
            result.Available = result.Entitlement - result.Used - result.Reserved;
            return result;
        }

        private decimal CalculateEmployeeLeaveBalance(Guid employeeId, Guid leaveTypeId, DateTime targetDate, Guid? excludedApplicationId, ServiceHeader header)
        {
            var employee = _employeeAppService.FindEmployee(employeeId, header);
            var policy = _leaveTypeAppService.FindLeaveType(leaveTypeId, header);
            if (employee == null || policy == null) throw new InvalidOperationException("Select an existing employee and leave type.");
            return GetCycleBalance(employee, policy, targetDate.Date, excludedApplicationId, header).Available;
        }

        public EmployeeLeaveStatisticsDTO GetEmployeeLeaveStatistics(Guid employeeId, Guid leaveTypeId, DateTime asAt, int pageIndex, ServiceHeader header)
        {
            EnsureReadPermission(header);
            if (asAt.Year < 1900 || asAt.Year > 9998 || pageIndex < 0 || pageIndex > 100000)
                throw new InvalidOperationException("Select a valid balance date and history page.");
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var employee = _employeeAppService.FindEmployee(employeeId, header);
                var policy = _leaveTypeAppService.FindLeaveType(leaveTypeId, header);
                if (employee == null || policy == null) throw new InvalidOperationException("Select an existing employee and leave type.");
                var applications = _leaveApplicationRepository.AllMatching(LeaveApplicationSpecifications.LeaveApplicationsWithEmployeeIdAndLeaveTypeId(employeeId, leaveTypeId), header) ?? new List<LeaveApplication>();
                var result = new EmployeeLeaveStatisticsDTO { AsAt = asAt.Date, Today = DateTime.Today, LeaveTypeDescription = policy.Description, IsAccrued = policy.IsAccrued };
                try
                {
                    result.Balance = GetCycleBalance(employee, policy, asAt.Date, null, header, applications);
                    var approvedDates = applications.Where(x => x.Status != (int)LeaveApplicationStatus.Pending).SelectMany(ConsumedDates)
                        .Where(x => x >= result.Balance.Start && x <= result.Balance.End).ToList();
                    result.TakenDays = approvedDates.Count(x => x <= DateTime.Today);
                    result.UpcomingDays = approvedDates.Count(x => x > DateTime.Today);
                }
                catch (InvalidOperationException ex) { result.BalanceError = ex.Message; }
                var start = new DateTime(asAt.Year, 1, 1);
                var end = start.AddYears(1);
                var history = applications.Where(x => x.Duration.StartDate < end && x.Duration.EndDate >= start)
                    .OrderByDescending(x => x.Duration.StartDate).ThenByDescending(x => x.CreatedDate).ThenBy(x => x.Id).ToList();
                result.HistoryCount = history.Count;
                result.History = history.Skip(pageIndex * 10).Take(10).Select(x => new EmployeeLeaveHistoryDTO {
                    Id = x.Id, Start = x.Duration.StartDate, End = x.Duration.EndDate, Status = x.Status,
                    ChargedDaysInYear = x.ChargedDates == null ? 0 : ConsumedDates(x).Count(d => d >= start && d < end),
                    EffectiveReturnDate = x.EffectiveReturnDate, Reason = x.Reason, AuthorizedBy = x.AuthorizedBy
                }).ToList();
                return result;
            }
        }

        public LeavePreviewDTO PreviewLeave(Guid employeeId, Guid leaveTypeId, DateTime start, DateTime end, Guid? excludedId, ServiceHeader header)
        {
            EnsureReadPermission(header);
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var result = new LeavePreviewDTO();
                try
                {
                    var policy = ValidateAndApplyRequest(new LeaveApplicationDTO { EmployeeId = employeeId, LeaveTypeId = leaveTypeId, DurationStartDate = start, DurationEndDate = end, Reason = "Preview" }, excludedId, header, true);
                    var employee = _employeeAppService.FindEmployee(employeeId, header);
                    var dates = GetChargeDates(start, end, policy, header);
                    if (excludedId.HasValue)
                    {
                        var existing = _leaveApplicationRepository.Get(excludedId.Value, header);
                        if (existing == null || existing.EmployeeId != employeeId) throw new InvalidOperationException("The application does not belong to the selected employee.");
                        if (existing.LeaveTypeId == leaveTypeId && existing.Duration.StartDate.Date == start.Date && existing.Duration.EndDate.Date == end.Date && existing.ChargedDates != null)
                            dates = LeaveCalendar.Decode(existing.ChargedDates);
                    }
                    if (!dates.Any()) throw new InvalidOperationException("The selected dates contain no chargeable leave days.");
                    result.RequestedDays = dates.Count;
                    foreach (var group in dates.GroupBy(x => policy.IsAccrued ? start.Date : LeaveCalendar.CycleStart(x, policy.UnitType)))
                    {
                        var cycle = GetCycleBalance(employee, policy, group.Key < start.Date ? start.Date : group.Key, excludedId, header);
                        cycle.Requested = group.Count();
                        cycle.Remaining = cycle.Available - cycle.Requested;
                        result.Cycles.Add(cycle);
                    }
                    result.CanSubmit = result.Cycles.All(x => x.Remaining >= 0);
                    if (!result.CanSubmit) result.Error = "Insufficient leave balance in one or more entitlement periods.";
                }
                catch (InvalidOperationException ex) { result.Error = ex.Message; result.CanSubmit = false; }
                return result;
            }
        }

        public bool WithdrawLeaveApplication(Guid id, ServiceHeader header)
        {
            EnsurePermission(LeaveApplicationModuleCode, header);
            using (var scope = _dbContextScopeFactory.CreateWithTransaction(System.Data.IsolationLevel.Serializable))
            {
                var application = _leaveApplicationRepository.Get(id, header);
                if (application == null) return false;
                LockEmployee(application.EmployeeId, header);
                if (application.Status != (byte)LeaveApplicationStatus.Pending) throw new InvalidOperationException("Only pending leave can be withdrawn.");
                if (!string.Equals(application.CreatedBy, header.ApplicationUserName, StringComparison.OrdinalIgnoreCase) && header.ApplicationUserEmployeeId != application.EmployeeId)
                    throw new InvalidOperationException("Only the employee or submitting user can withdraw this application.");
                application.Status = (byte)LeaveApplicationStatus.Withdrawn;
                application.NotificationPending = false;
                application.RecalledBy = header.ApplicationUserName;
                application.RecalledDate = DateTime.Now;
                application.RecallRemarks = "Withdrawn before approval";
                return scope.SaveChanges(header) >= 0;
            }
        }

        public void MarkLeaveNotificationQueued(Guid id, ServiceHeader header)
        {
            EnsureReadPermission(header);
            using (var scope = _dbContextScopeFactory.CreateWithTransaction(System.Data.IsolationLevel.Serializable))
            {
                var application = _leaveApplicationRepository.Get(id, header);
                if (application == null) return;
                application.NotificationPending = false;
                scope.SaveChanges(header);
            }
        }

        public bool RetryLeaveNotification(Guid id, ServiceHeader header)
        {
            EnsurePermission(LeaveApprovalModuleCode, header);
            try
            {
                var application = FindLeaveApplication(id, header);
                if (application == null || !application.NotificationPending) return true;
                if (application.Status != (byte)LeaveApplicationStatus.Approved && application.Status != (byte)LeaveApplicationStatus.Rejected) return false;
                if (!_brokerService.ProcessLeaveApprovalAccountAlerts(DMLCommand.None, header, application)) return false;
                MarkLeaveNotificationQueued(id, header);
                return true;
            }
            catch (Exception ex) { System.Diagnostics.Trace.TraceError("Leave notification retry failed for {0}: {1}", id, ex); return false; }
        }

        private void EnsureReadPermission(ServiceHeader header)
        {
            if (header == null) throw new InvalidOperationException("Authenticated caller context is required.");
            var roles = header.ApplicationUserRoles ?? new List<string>();
            if (!new[] { LeaveApplicationModuleCode, LeaveApprovalModuleCode, LeaveRecallModuleCode }.Any(code =>
                (_navigationItemInRoleAppService.GetRolesForNavigationItemCode(code, header) ?? new string[0]).Any(granted => roles.Any(role => string.Equals(role, granted, StringComparison.OrdinalIgnoreCase)))))
                throw new InvalidOperationException("Access denied for employee leave.");
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
