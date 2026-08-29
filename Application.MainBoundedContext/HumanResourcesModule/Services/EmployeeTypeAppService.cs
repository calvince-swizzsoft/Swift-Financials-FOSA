using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.AccountsModule.Services;
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
        private readonly IChartOfAccountAppService _chartOfAccountAppService;

        public EmployeeTypeAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<EmployeeType> employeeTypeRepository,
           IChartOfAccountAppService chartOfAccountAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (employeeTypeRepository == null)
                throw new ArgumentNullException(nameof(employeeTypeRepository));

            if (chartOfAccountAppService == null)
                throw new ArgumentNullException(nameof(chartOfAccountAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _employeeTypeRepository = employeeTypeRepository;
            _chartOfAccountAppService = chartOfAccountAppService;
        }

        public string ValidateEmployeeType(EmployeeTypeDTO employeeTypeDTO, Guid? existingEmployeeTypeId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return ValidateEmployeeTypeCore(employeeTypeDTO, existingEmployeeTypeId, serviceHeader);
            }
        }

        private string ValidateEmployeeTypeCore(EmployeeTypeDTO employeeTypeDTO, Guid? existingEmployeeTypeId, ServiceHeader serviceHeader)
        {
            if (employeeTypeDTO == null)
                return "Employee type details are required.";
            if (employeeTypeDTO.ChartOfAccountId == Guid.Empty)
                return "A payroll control account is required.";
            if (string.IsNullOrWhiteSpace(employeeTypeDTO.Description))
                return "An employee type description is required.";
            if (!Enum.IsDefined(typeof(EmployeeCategory), employeeTypeDTO.Category))
                return "The selected employee category is invalid.";

            var chartOfAccount = _chartOfAccountAppService.FindChartOfAccount(employeeTypeDTO.ChartOfAccountId, serviceHeader);
            if (chartOfAccount == null)
                return "The selected payroll control account could not be found.";
            if (chartOfAccount.AccountCategory != (int)ChartOfAccountCategory.DetailAccount)
                return "The payroll control account must be a detail account that accepts postings.";
            if (chartOfAccount.IsLocked)
                return "The selected payroll control account is locked.";

            var sameCategory = _employeeTypeRepository.AllMatching(
                EmployeeTypeSpecifications.EmployeeTypeWithCategory(employeeTypeDTO.Category), serviceHeader);
            if (sameCategory != null && sameCategory.Any(x => !existingEmployeeTypeId.HasValue || x.Id != existingEmployeeTypeId.Value))
                return string.Format(
                    "An employee type already exists for the {0} category. Each category can only be configured once.",
                    EnumHelper.GetDescription((EmployeeCategory)employeeTypeDTO.Category));

            return null;
        }

        public EmployeeTypeDTO AddNewEmployeeType(EmployeeTypeDTO employeeTypeDTO, ServiceHeader serviceHeader)
        {
            if (employeeTypeDTO == null) return null;

            if (employeeTypeDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var validationError = ValidateEmployeeTypeCore(employeeTypeDTO, null, serviceHeader);
                    if (!string.IsNullOrWhiteSpace(validationError))
                    {
                        employeeTypeDTO.ErrorMessageResult = validationError;
                        return employeeTypeDTO;
                    }

                    var proceed = true;

                    var filter = EmployeeTypeSpecifications.EmployeeTypeWithCategory(employeeTypeDTO.Category);

                    ISpecification<EmployeeType> spec = filter;

                    var salaryHeads = _employeeTypeRepository.AllMatching(spec, serviceHeader);

                    if (salaryHeads != null && salaryHeads.Any())
                    {
                        employeeTypeDTO.ErrorMessageResult = string.Format(
                            "An employee type already exists for the {0} category. Each category can only be configured once.",
                            EnumHelper.GetDescription((EmployeeCategory)employeeTypeDTO.Category));
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
                    else return employeeTypeDTO;
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
                if (!string.IsNullOrWhiteSpace(ValidateEmployeeTypeCore(employeeTypeDTO, employeeTypeDTO.Id, serviceHeader)))
                    return false;

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
