using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.AdministrationModule.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.LeaveTypeAgg;
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
    public class LeaveTypeAppService : ILeaveTypeAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<LeaveType> _leaveTypeRepository;
        private readonly INavigationItemInRoleAppService _navigationItemInRoleAppService;
        private const int LeaveApplicationModuleCode = 22016;

        public LeaveTypeAppService(
            IDbContextScopeFactory dbContextScopeFactory,
            IRepository<LeaveType> leaveTypeRepository,
            INavigationItemInRoleAppService navigationItemInRoleAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (leaveTypeRepository == null)
                throw new ArgumentNullException(nameof(leaveTypeRepository));

            if (navigationItemInRoleAppService == null)
                throw new ArgumentNullException(nameof(navigationItemInRoleAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _leaveTypeRepository = leaveTypeRepository;
            _navigationItemInRoleAppService = navigationItemInRoleAppService;
        }

        public LeaveTypeDTO AddNewLeaveType(LeaveTypeDTO leaveTypeDTO, ServiceHeader serviceHeader)
        {
            EnsurePermission(serviceHeader);
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                ValidateLeaveType(leaveTypeDTO, null, serviceHeader);
                var leaveType = LeaveTypeFactory.CreateLeaveType(leaveTypeDTO.Description, leaveTypeDTO.Entitlement, leaveTypeDTO.TargetGender, leaveTypeDTO.IsAccrued, leaveTypeDTO.UnitType, leaveTypeDTO.ExcludeHolidays, leaveTypeDTO.ExcludeWeekends);

                leaveType.CreatedBy = serviceHeader.ApplicationUserName;

                if (leaveTypeDTO.IsLocked)
                    leaveType.Lock();
                else
                    leaveType.UnLock();

                _leaveTypeRepository.Add(leaveType, serviceHeader);

                return dbContextScope.SaveChanges(serviceHeader) >= 0 ? leaveType.ProjectedAs<LeaveTypeDTO>() : null;
            }
        }

        public LeaveTypeDTO FindLeaveType(Guid leaveTypeId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _leaveTypeRepository.Get<LeaveTypeDTO>(leaveTypeId, serviceHeader);
            }
        }

        public List<LeaveTypeDTO> FindLeaveTypes(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _leaveTypeRepository.GetAll<LeaveTypeDTO>(serviceHeader);
            }
        }

        public PageCollectionInfo<LeaveTypeDTO> FindLeaveTypes(string filterText, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = LeaveTypeSpecifications.LeaveTypeFullText(filterText);

                ISpecification<LeaveType> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return _leaveTypeRepository.AllMatchingPaged<LeaveTypeDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public bool UpdateLeaveType(LeaveTypeDTO leaveTypeDTO, ServiceHeader serviceHeader)
        {
            EnsurePermission(serviceHeader);
            if (leaveTypeDTO == null || leaveTypeDTO.Id == Guid.Empty) return false;
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                ValidateLeaveType(leaveTypeDTO, leaveTypeDTO.Id, serviceHeader);
                var persisted = _leaveTypeRepository.Get(leaveTypeDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = LeaveTypeFactory.CreateLeaveType(leaveTypeDTO.Description, leaveTypeDTO.Entitlement, leaveTypeDTO.TargetGender, leaveTypeDTO.IsAccrued, leaveTypeDTO.UnitType, leaveTypeDTO.ExcludeHolidays, leaveTypeDTO.ExcludeWeekends);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                    _leaveTypeRepository.Merge(persisted, current, serviceHeader);

                    if (leaveTypeDTO.IsLocked) persisted.Lock();
                    else persisted.UnLock();

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                return false;
            }
        }

        private void ValidateLeaveType(LeaveTypeDTO leaveTypeDTO, Guid? existingId, ServiceHeader serviceHeader)
        {
            if (leaveTypeDTO == null) throw new InvalidOperationException("Leave type details are required.");
            leaveTypeDTO.Description = (leaveTypeDTO.Description ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(leaveTypeDTO.Description)) throw new InvalidOperationException("A leave type description is required.");
            if (leaveTypeDTO.Entitlement <= 0) throw new InvalidOperationException("Entitlement must be greater than zero days.");
            if (!Enum.IsDefined(typeof(LeaveUnitTypes), (int)leaveTypeDTO.UnitType) || leaveTypeDTO.UnitType == (byte)LeaveUnitTypes.Unknown)
                throw new InvalidOperationException("Select a valid weekly, monthly, or yearly entitlement cycle.");
            if (!Enum.IsDefined(typeof(LeaveTypeTargetGender), (int)leaveTypeDTO.TargetGender))
                throw new InvalidOperationException("Select a valid target gender.");

            var matches = _leaveTypeRepository.AllMatching(LeaveTypeSpecifications.LeaveTypeWithDescription(leaveTypeDTO.Description), serviceHeader);
            if (matches != null && matches.Any(x => !existingId.HasValue || x.Id != existingId.Value))
                throw new InvalidOperationException("A leave type with this description already exists.");
        }

        private void EnsurePermission(ServiceHeader serviceHeader)
        {
            if (serviceHeader == null) throw new InvalidOperationException("Authenticated caller context is required.");
            var callerRoles = serviceHeader.ApplicationUserRoles ?? new List<string>();
            var grantedRoles = _navigationItemInRoleAppService.GetRolesForNavigationItemCode(LeaveApplicationModuleCode, serviceHeader) ?? new string[0];
            if (!callerRoles.Any(callerRole => grantedRoles.Any(grantedRole => string.Equals(callerRole, grantedRole, StringComparison.OrdinalIgnoreCase))))
                throw new InvalidOperationException("Access denied: your role is not authorized to manage leave configuration.");
        }
    }
}
