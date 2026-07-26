using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.Seedwork;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.DepartmentAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public class DepartmentAppService : IDepartmentAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<Department> _departmentRepository;

        public DepartmentAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<Department> departmentRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (departmentRepository == null)
                throw new ArgumentNullException(nameof(departmentRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _departmentRepository = departmentRepository;
        }

        public DepartmentDTO AddNewDepartment(DepartmentDTO departmentDTO, ServiceHeader serviceHeader)
        {
            if (departmentDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var department = DepartmentFactory.CreateDepartment(departmentDTO.Description);

                    if (departmentDTO.IsLocked)
                        department.Lock();
                    else department.UnLock();

                    _departmentRepository.Add(department, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return department.ProjectedAs<DepartmentDTO>();
                }
            }
            else return null;
        }

        public bool UpdateDepartment(DepartmentDTO departmentDTO, ServiceHeader serviceHeader)
        {
            if (departmentDTO == null || departmentDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _departmentRepository.Get(departmentDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = DepartmentFactory.CreateDepartment(departmentDTO.Description);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    
                    if (departmentDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _departmentRepository.Merge(persisted, current, serviceHeader);

                    // Set as registry?
                    if (departmentDTO.IsRegistry && !persisted.IsRegistry)
                        SetDepartmentAsRegistry(persisted.Id, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<DepartmentDTO> FindDepartments(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var departments = _departmentRepository.GetAll(serviceHeader);

                if (departments != null && departments.Any())
                {
                    return departments.ProjectedAsCollection<DepartmentDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<DepartmentDTO> FindDepartments(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = DepartmentSpecifications.DefaultSpec();

                ISpecification<Department> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var departmentPagedCollection = _departmentRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (departmentPagedCollection != null)
                {
                    var pageCollection = departmentPagedCollection.PageCollection.ProjectedAsCollection<DepartmentDTO>();

                    var itemsCount = departmentPagedCollection.ItemsCount;

                    return new PageCollectionInfo<DepartmentDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<DepartmentDTO> FindDepartments(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = DepartmentSpecifications.DepartmentFullText(text);

                ISpecification<Department> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var departmentCollection = _departmentRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (departmentCollection != null)
                {
                    var pageCollection = departmentCollection.PageCollection.ProjectedAsCollection<DepartmentDTO>();

                    var itemsCount = departmentCollection.ItemsCount;

                    return new PageCollectionInfo<DepartmentDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public DepartmentDTO FindDepartment(Guid departmentId, ServiceHeader serviceHeader)
        {
            if (departmentId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var department = _departmentRepository.Get(departmentId, serviceHeader);

                    if (department != null)
                    {
                        return department.ProjectedAs<DepartmentDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public DepartmentDTO FindRegistryDepartment(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = DepartmentSpecifications.RegistryDepartment();

                ISpecification<Department> spec = filter;

                var departments = _departmentRepository.AllMatching(spec, serviceHeader);

                if (departments != null && departments.Any() && departments.Count() == 1)
                {
                    var department = departments.SingleOrDefault();

                    if (department != null)
                    {
                        return department.ProjectedAs<DepartmentDTO>();
                    }
                    else return null;
                }
                else return null;
            }
        }

        private bool SetDepartmentAsRegistry(Guid departmentId, ServiceHeader serviceHeader)
        {
            if (departmentId == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _departmentRepository.Get(departmentId, serviceHeader);

                if (persisted != null)
                {
                    persisted.SetAsRegistry();

                    var otherDepartments = _departmentRepository.GetAll(serviceHeader);

                    foreach (var item in otherDepartments)
                    {
                        if (item.Id != persisted.Id)
                        {
                            var department = _departmentRepository.Get(item.Id, serviceHeader);

                            department.ResetAsRegistry();
                        }
                    }

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }
    }
}
