using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.Seedwork;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeTypeAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public class EmployeeTypeAppService : IEmployeeTypeAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<EmployeeType> _employeeTypeRepository;

        public EmployeeTypeAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<EmployeeType> employeeTypeRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (employeeTypeRepository == null)
                throw new ArgumentNullException(nameof(employeeTypeRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _employeeTypeRepository = employeeTypeRepository;
        }

        public EmployeeTypeDTO AddNewEmployeeType(EmployeeTypeDTO employeeTypeDTO, ServiceHeader serviceHeader)
        {
            if (employeeTypeDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var proceed = true;

                    var filter = EmployeeTypeSpecifications.EmployeeTypeWithCategory(employeeTypeDTO.Category);

                    ISpecification<EmployeeType> spec = filter;

                    var salaryHeads = _employeeTypeRepository.AllMatching(spec, serviceHeader);

                    if (salaryHeads != null && salaryHeads.Any())
                    {
                        proceed = false;
                    }
                    
                    if (proceed)
                    {
                        var employeeType = EmployeeTypeFactory.CreateEmployeeType(employeeTypeDTO.ChartOfAccountId, employeeTypeDTO.Description, employeeTypeDTO.Category);

                        if (employeeTypeDTO.IsLocked)
                            employeeType.Lock();
                        else employeeType.UnLock();

                        _employeeTypeRepository.Add(employeeType, serviceHeader);

                        dbContextScope.SaveChanges(serviceHeader);

                        return employeeType.ProjectedAs<EmployeeTypeDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public bool UpdateEmployeeType(EmployeeTypeDTO employeeTypeDTO, ServiceHeader serviceHeader)
        {
            if (employeeTypeDTO == null || employeeTypeDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _employeeTypeRepository.Get(employeeTypeDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = EmployeeTypeFactory.CreateEmployeeType(employeeTypeDTO.ChartOfAccountId, employeeTypeDTO.Description, employeeTypeDTO.Category);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    
                    if (employeeTypeDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _employeeTypeRepository.Merge(persisted, current, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return true;
                }
                else return false;
            }
        }

        public List<EmployeeTypeDTO> FindEmployeeTypes(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var employeeTypes = _employeeTypeRepository.GetAll(serviceHeader);

                if (employeeTypes != null && employeeTypes.Any())
                {
                    return employeeTypes.ProjectedAsCollection<EmployeeTypeDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<EmployeeTypeDTO> FindEmployeeTypes(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = EmployeeTypeSpecifications.DefaultSpec();

                ISpecification<EmployeeType> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var employeeTypePagedCollection = _employeeTypeRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (employeeTypePagedCollection != null)
                {
                    var pageCollection = employeeTypePagedCollection.PageCollection.ProjectedAsCollection<EmployeeTypeDTO>();

                    var itemsCount = employeeTypePagedCollection.ItemsCount;

                    return new PageCollectionInfo<EmployeeTypeDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<EmployeeTypeDTO> FindEmployeeTypes(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = string.IsNullOrWhiteSpace(text) ? EmployeeTypeSpecifications.DefaultSpec() : EmployeeTypeSpecifications.EmployeeTypeFullText(text);

                ISpecification<EmployeeType> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var employeeTypePagedCollection = _employeeTypeRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (employeeTypePagedCollection != null)
                {
                    var pageCollection = employeeTypePagedCollection.PageCollection.ProjectedAsCollection<EmployeeTypeDTO>();

                    var itemsCount = employeeTypePagedCollection.ItemsCount;

                    return new PageCollectionInfo<EmployeeTypeDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public EmployeeTypeDTO FindEmployeeType(Guid employeeTypeId, ServiceHeader serviceHeader)
        {
            if (employeeTypeId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var employeeType = _employeeTypeRepository.Get(employeeTypeId, serviceHeader);

                    if (employeeType != null)
                    {
                        return employeeType.ProjectedAs<EmployeeTypeDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }
    }
}
