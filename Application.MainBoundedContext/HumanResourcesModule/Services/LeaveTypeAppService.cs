using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.Seedwork;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.LeaveTypeAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Adapter;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public class LeaveTypeAppService : ILeaveTypeAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<LeaveType> _leaveTypeRepository;

        public LeaveTypeAppService(
            IDbContextScopeFactory dbContextScopeFactory,
            IRepository<LeaveType> leaveTypeRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (leaveTypeRepository == null)
                throw new ArgumentNullException(nameof(leaveTypeRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _leaveTypeRepository = leaveTypeRepository;
        }

        public LeaveTypeDTO AddNewLeaveType(LeaveTypeDTO leaveTypeDTO, ServiceHeader serviceHeader)
        {
            var leaveTypeBindingModel = leaveTypeDTO.ProjectedAs<LeaveTypeBindingModel>();

            leaveTypeBindingModel.ValidateAll();

            if (leaveTypeBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, leaveTypeBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
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
            var leaveTypeBindingModel = leaveTypeDTO.ProjectedAs<LeaveTypeBindingModel>();

            leaveTypeBindingModel.ValidateAll();

            if (leaveTypeBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, leaveTypeBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _leaveTypeRepository.Get(leaveTypeDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = LeaveTypeFactory.CreateLeaveType(leaveTypeDTO.Description, leaveTypeDTO.Entitlement, leaveTypeDTO.TargetGender, leaveTypeDTO.IsAccrued, leaveTypeDTO.UnitType, leaveTypeDTO.ExcludeHolidays, leaveTypeDTO.ExcludeWeekends);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                    _leaveTypeRepository.Merge(persisted, current, serviceHeader);

                    // Lock?
                    if (leaveTypeDTO.IsLocked && !persisted.IsLocked)
                        LockLeaveType(persisted.Id, serviceHeader);
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        private bool LockLeaveType(Guid leaveTypeId, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _leaveTypeRepository.Get(leaveTypeId, serviceHeader);

                if (persisted != null)
                {
                    persisted.Lock();
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }
    }
}
