using System;
using System.Collections.Generic;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using Domain.Seedwork;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAppraisalAgg;
using System.Linq;
using Domain.Seedwork.Specification;
using Application.Seedwork;
using Application.MainBoundedContext.DTO;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public class EmployeeAppraisalAppService : IEmployeeAppraisalAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<EmployeeAppraisal> _employeeAppraisalRepository;

        public EmployeeAppraisalAppService(
            IDbContextScopeFactory dbContextScopeFactory,
            IRepository<EmployeeAppraisal> employeeAppraisalRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (employeeAppraisalRepository == null)
                throw new ArgumentNullException(nameof(employeeAppraisalRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _employeeAppraisalRepository = employeeAppraisalRepository;
        }
        public bool AddNewEmployeeAppraisal(List<EmployeeAppraisalDTO> employeeAppraisalDTOs, ServiceHeader serviceHeader)
        {
            if (employeeAppraisalDTOs != null && employeeAppraisalDTOs.Any())
            {
                foreach (var employeeAppraisalDTO in employeeAppraisalDTOs)
                {
                    if (employeeAppraisalDTO.EmployeeAppraisalPeriodId == Guid.Empty || employeeAppraisalDTO.EmployeeId == Guid.Empty || employeeAppraisalDTO.EmployeeAppraisalTargetId == Guid.Empty)
                        return false;
                }

                var appraisals = FindEmployeeAppraisals(employeeAppraisalDTOs.FirstOrDefault().EmployeeId, employeeAppraisalDTOs.FirstOrDefault().EmployeeAppraisalPeriodId, serviceHeader);

                if (appraisals == null)
                {
                    using (var dbContextScope = _dbContextScopeFactory.Create())
                    {
                        employeeAppraisalDTOs.ForEach(employeeAppraisalDTO =>
                        {
                            var employeeAppraisal = EmployeeAppraisalFactory.CreateEmployeeAppraisal(employeeAppraisalDTO.EmployeeAppraisalPeriodId, employeeAppraisalDTO.EmployeeId, employeeAppraisalDTO.BranchId, employeeAppraisalDTO.EmployeeAppraisalTargetId, employeeAppraisalDTO.AppraiseeAnswer);

                            employeeAppraisal.CreatedBy = serviceHeader.ApplicationUserName;

                            _employeeAppraisalRepository.Add(employeeAppraisal, serviceHeader);
                        });

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                }
                else return false;
            }
            else return false;
        }

        public bool UpdateEmployeeAppraisal(EmployeeAppraisalDTO employeeAppraisalDTO, ServiceHeader serviceHeader)
        {
            if (employeeAppraisalDTO == null || employeeAppraisalDTO.Id == Guid.Empty || employeeAppraisalDTO.IsLocked || DateTime.Today > employeeAppraisalDTO.EmployeeAppraisalPeriodDurationEndDate)
                return false;

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _employeeAppraisalRepository.Get(employeeAppraisalDTO.Id, serviceHeader);

                    if (persisted != null)
                    {
                        var current = EmployeeAppraisalFactory.CreateEmployeeAppraisal(employeeAppraisalDTO.EmployeeAppraisalPeriodId, employeeAppraisalDTO.EmployeeId, employeeAppraisalDTO.BranchId, employeeAppraisalDTO.EmployeeAppraisalTargetId, employeeAppraisalDTO.AppraiseeAnswer);

                        current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                        _employeeAppraisalRepository.Merge(persisted, current, serviceHeader);

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
        }

        public bool AppraiseEmployeeAppraisal(EmployeeAppraisalDTO employeeAppraisalDTO, ServiceHeader serviceHeader)
        {
            if (employeeAppraisalDTO == null || employeeAppraisalDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _employeeAppraisalRepository.Get(employeeAppraisalDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = EmployeeAppraisalFactory.CreateEmployeeAppraisal(employeeAppraisalDTO.EmployeeAppraisalPeriodId, employeeAppraisalDTO.EmployeeId, employeeAppraisalDTO.BranchId, employeeAppraisalDTO.EmployeeAppraisalTargetId, employeeAppraisalDTO.AppraiseeAnswer);

                    current.AppraiserAnswer = employeeAppraisalDTO.AppraiserAnswer;

                    current.AppraisedBy = serviceHeader.ApplicationUserName;

                    current.AppraisedDate = DateTime.Now;

                    current.Lock();

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                    _employeeAppraisalRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<EmployeeAppraisalDTO> FindEmployeeAppraisals(Guid employeeId, Guid employeeAppraisalPeriodId, ServiceHeader serviceHeader)
        {
            if (employeeId == Guid.Empty || employeeAppraisalPeriodId == Guid.Empty)
                return null;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<EmployeeAppraisal> spec = EmployeeAppraisalSpecifications.EmployeeAppraisalWithEmployeeIdAndEmployeeAppraisalPeriodId(employeeId, employeeAppraisalPeriodId);

                var employeeAppraisals = _employeeAppraisalRepository.AllMatching(spec, serviceHeader);

                if (employeeAppraisals != null && employeeAppraisals.Any())
                {
                    return employeeAppraisals.ProjectedAsCollection<EmployeeAppraisalDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<EmployeeAppraisalDTO> FindEmployeeAppraisals(Guid employeeId, Guid employeeAppraisalPeriodId, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (employeeId == Guid.Empty || employeeAppraisalPeriodId == Guid.Empty)
                return null;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = EmployeeAppraisalSpecifications.EmployeeAppraisalWithEmployeeIdAndEmployeeAppraisalPeriodId(employeeId, employeeAppraisalPeriodId);

                ISpecification<EmployeeAppraisal> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var employeeAppraisalPagedCollection = _employeeAppraisalRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (employeeAppraisalPagedCollection != null)
                {
                    var pageCollection = employeeAppraisalPagedCollection.PageCollection.ProjectedAsCollection<EmployeeAppraisalDTO>();

                    var itemsCount = employeeAppraisalPagedCollection.ItemsCount;

                    return new PageCollectionInfo<EmployeeAppraisalDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }
    }
}
