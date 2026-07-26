using System;
using System.Collections.Generic;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeDisciplinaryCaseAgg;
using Domain.Seedwork;
using Application.Seedwork;
using System.Linq;
using Domain.Seedwork.Specification;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public class EmployeeDisciplinaryCaseAppService : IEmployeeDisciplinaryCaseAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<EmployeeDisciplinaryCase> _employeeDisciplinaryCaseRepository;

        public EmployeeDisciplinaryCaseAppService(IDbContextScopeFactory dbContextScopeFactory,
           IRepository<EmployeeDisciplinaryCase> employeeDisciplinaryCaseRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (employeeDisciplinaryCaseRepository == null)
                throw new ArgumentNullException(nameof(employeeDisciplinaryCaseRepository));

            _dbContextScopeFactory = dbContextScopeFactory;

            _employeeDisciplinaryCaseRepository = employeeDisciplinaryCaseRepository;
        }

        public EmployeeDisciplinaryCaseDTO AddNewEmployeeDisciplinaryCase(EmployeeDisciplinaryCaseDTO employeeDisciplinaryCaseDTO, ServiceHeader serviceHeader)
        {
            if (employeeDisciplinaryCaseDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var employeeDisciplinaryCase = EmployeeDisciplinaryCaseFactory.CreateEmployeeDisciplinaryCase(employeeDisciplinaryCaseDTO.EmployeeId, employeeDisciplinaryCaseDTO.IncidentDate, employeeDisciplinaryCaseDTO.Type, employeeDisciplinaryCaseDTO.FileName, employeeDisciplinaryCaseDTO.FileTitle, employeeDisciplinaryCaseDTO.FileDescription, employeeDisciplinaryCaseDTO.FileMIMEType, employeeDisciplinaryCaseDTO.Remarks);

                    employeeDisciplinaryCase.CreatedBy = serviceHeader.ApplicationUserName;

                    _employeeDisciplinaryCaseRepository.Add(employeeDisciplinaryCase, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return employeeDisciplinaryCase.ProjectedAs<EmployeeDisciplinaryCaseDTO>();
                }
            }
            else return null;
        }

        public EmployeeDisciplinaryCaseDTO FindEmployeeDisciplinaryCase(Guid employeeDisciplinaryCaseId, ServiceHeader serviceHeader)
        {
            if (employeeDisciplinaryCaseId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var employeeDisciplinaryCase = _employeeDisciplinaryCaseRepository.Get(employeeDisciplinaryCaseId, serviceHeader);

                    if (employeeDisciplinaryCase != null)
                    {
                        return employeeDisciplinaryCase.ProjectedAs<EmployeeDisciplinaryCaseDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<EmployeeDisciplinaryCaseDTO> FindEmployeeDisciplinaryCases(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var employeeDisciplinaryCases = _employeeDisciplinaryCaseRepository.GetAll(serviceHeader);

                if (employeeDisciplinaryCases != null && employeeDisciplinaryCases.Any())
                {
                    return employeeDisciplinaryCases.ProjectedAsCollection<EmployeeDisciplinaryCaseDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<EmployeeDisciplinaryCaseDTO> FindEmployeeDisciplinaryCases(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = EmployeeDisciplinaryCaseSpecifications.DefaultSpec();

                ISpecification<EmployeeDisciplinaryCase> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var employeeDisciplinaryCasePagedCollection = _employeeDisciplinaryCaseRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (employeeDisciplinaryCasePagedCollection != null)
                {
                    var pageCollection = employeeDisciplinaryCasePagedCollection.PageCollection.ProjectedAsCollection<EmployeeDisciplinaryCaseDTO>();

                    var itemsCount = employeeDisciplinaryCasePagedCollection.ItemsCount;

                    return new PageCollectionInfo<EmployeeDisciplinaryCaseDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<EmployeeDisciplinaryCaseDTO> FindEmployeeDisciplinaryCases(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = string.IsNullOrWhiteSpace(text) ? EmployeeDisciplinaryCaseSpecifications.DefaultSpec() : EmployeeDisciplinaryCaseSpecifications.EmployeeDisciplinaryCaseWithText(text);

                ISpecification<EmployeeDisciplinaryCase> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var employeeDisciplinaryCasePagedCollection = _employeeDisciplinaryCaseRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (employeeDisciplinaryCasePagedCollection != null)
                {
                    var pageCollection = employeeDisciplinaryCasePagedCollection.PageCollection.ProjectedAsCollection<EmployeeDisciplinaryCaseDTO>();

                    var itemsCount = employeeDisciplinaryCasePagedCollection.ItemsCount;

                    return new PageCollectionInfo<EmployeeDisciplinaryCaseDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<EmployeeDisciplinaryCaseDTO> FindEmployeeDisciplinaryCasesByEmployeeId(Guid employeeId, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (employeeId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = EmployeeDisciplinaryCaseSpecifications.EmployeeDisciplinaryCaseWithEmployeeId(employeeId);

                    ISpecification<EmployeeDisciplinaryCase> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var employeeDisciplinaryCases = _employeeDisciplinaryCaseRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (employeeDisciplinaryCases != null)
                    {
                        var pageCollection = employeeDisciplinaryCases.PageCollection.ProjectedAsCollection<EmployeeDisciplinaryCaseDTO>();

                        var itemsCount = employeeDisciplinaryCases.ItemsCount;

                        return new PageCollectionInfo<EmployeeDisciplinaryCaseDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                    }
                    else return null;
                }
            }
            else return null;
        }

        public bool UpdateEmployeeDisciplinaryCase(EmployeeDisciplinaryCaseDTO employeeDisciplinaryCaseDTO, ServiceHeader serviceHeader)
        {
            if (employeeDisciplinaryCaseDTO == null || employeeDisciplinaryCaseDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _employeeDisciplinaryCaseRepository.Get(employeeDisciplinaryCaseDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = EmployeeDisciplinaryCaseFactory.CreateEmployeeDisciplinaryCase(employeeDisciplinaryCaseDTO.EmployeeId, employeeDisciplinaryCaseDTO.IncidentDate, employeeDisciplinaryCaseDTO.Type, employeeDisciplinaryCaseDTO.FileName, employeeDisciplinaryCaseDTO.FileTitle, employeeDisciplinaryCaseDTO.FileDescription, employeeDisciplinaryCaseDTO.FileMIMEType, employeeDisciplinaryCaseDTO.Remarks);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                    _employeeDisciplinaryCaseRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }
    }
}
