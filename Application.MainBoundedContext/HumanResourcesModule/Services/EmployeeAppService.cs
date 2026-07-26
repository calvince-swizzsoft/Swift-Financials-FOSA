using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.Seedwork;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryCardAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public class EmployeeAppService : IEmployeeAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<SalaryCard> _salaryCardRepository;

        public EmployeeAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<Employee> employeeRepository,
           IRepository<SalaryCard> salaryCardRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (employeeRepository == null)
                throw new ArgumentNullException(nameof(employeeRepository));

            if (salaryCardRepository == null)
                throw new ArgumentNullException(nameof(salaryCardRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _employeeRepository = employeeRepository;
            _salaryCardRepository = salaryCardRepository;
        }

        public EmployeeDTO AddNewEmployee(EmployeeDTO employeeDTO, ServiceHeader serviceHeader)
        {
            if (employeeDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    // get the specification
                    var filter = EmployeeSpecifications.EmployeeWithCustomerId(employeeDTO.CustomerId);

                    ISpecification<Employee> spec = filter;

                    //Query this criteria
                    var employees = _employeeRepository.AllMatching(spec, serviceHeader);

                    if (employees != null && employees.Any())
                    {
                        employeeDTO.errormassage = string.Format(("Sorry, but the selected customer already exists as an employee!"));
                        return employeeDTO;
                    }
                  
                    else
                    {
                        var employee = EmployeeFactory.CreateEmployee(employeeDTO.CustomerId, employeeDTO.BranchId, employeeDTO.DesignationId, employeeDTO.DepartmentId, employeeDTO.EmployeeTypeId, employeeDTO.NationalSocialSecurityFundNumber, employeeDTO.NationalHospitalInsuranceFundNumber, employeeDTO.BloodGroup, employeeDTO.Remarks, employeeDTO.OnlineNotificationsEnabled, employeeDTO.EnforceBiometricsForLogin);

                        if (employeeDTO.IsLocked)
                            employee.Lock();
                        else employee.UnLock();

                        _employeeRepository.Add(employee, serviceHeader);

                        dbContextScope.SaveChanges(serviceHeader);

                        return employee.ProjectedAs<EmployeeDTO>();
                    }
                }
            }
            else return null;
        }

        public bool UpdateEmployee(EmployeeDTO employeeDTO, ServiceHeader serviceHeader)
        {
            if (employeeDTO == null || employeeDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _employeeRepository.Get(employeeDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = EmployeeFactory.CreateEmployee(persisted.CustomerId, employeeDTO.BranchId, employeeDTO.DesignationId, employeeDTO.DepartmentId, employeeDTO.EmployeeTypeId, employeeDTO.NationalSocialSecurityFundNumber, employeeDTO.NationalHospitalInsuranceFundNumber, employeeDTO.BloodGroup, employeeDTO.Remarks, employeeDTO.OnlineNotificationsEnabled, employeeDTO.EnforceBiometricsForLogin);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                    if (employeeDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _employeeRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public bool CustomerIsEmployee(Guid customerId, ServiceHeader serviceHeader)
        {
            if (customerId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = EmployeeSpecifications.EmployeeWithCustomerId(customerId);

                    ISpecification<Employee> spec = filter;

                    return _employeeRepository.AllMatchingCount(spec, serviceHeader) != 0;
                }
            }
            else return false;
        }

        public List<EmployeeDTO> FindEmployees(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var employees = _employeeRepository.GetAll(serviceHeader);

                if (employees != null && employees.Any())
                {
                    return employees.ProjectedAsCollection<EmployeeDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<EmployeeDTO> FindEmployees(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = EmployeeSpecifications.DefaultSpec();

                ISpecification<Employee> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var employeePagedCollection = _employeeRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (employeePagedCollection != null)
                {
                    var pageCollection = employeePagedCollection.PageCollection.ProjectedAsCollection<EmployeeDTO>();

                    var itemsCount = employeePagedCollection.ItemsCount;

                    return new PageCollectionInfo<EmployeeDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<EmployeeDTO> FindEmployees(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = EmployeeSpecifications.EmployeeFullText(text);

                ISpecification<Employee> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var employeePagedCollection = _employeeRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (employeePagedCollection != null)
                {
                    var pageCollection = employeePagedCollection.PageCollection.ProjectedAsCollection<EmployeeDTO>();

                    var itemsCount = employeePagedCollection.ItemsCount;

                    return new PageCollectionInfo<EmployeeDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<EmployeeDTO> FindEmployees(Guid departmentId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = EmployeeSpecifications.EmployeesByDepartmentIdWithText(departmentId, text);

                ISpecification<Employee> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var EmployeePagedCollection = _employeeRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (EmployeePagedCollection != null)
                {
                    var pageCollection = EmployeePagedCollection.PageCollection.ProjectedAsCollection<EmployeeDTO>();

                    var itemsCount = EmployeePagedCollection.ItemsCount;

                    return new PageCollectionInfo<EmployeeDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public EmployeeDTO FindEmployee(Guid employeeId, ServiceHeader serviceHeader)
        {
            if (employeeId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var employee = _employeeRepository.Get(employeeId, serviceHeader);

                    if (employee != null)
                    {
                        return employee.ProjectedAs<EmployeeDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<EmployeeDTO> FindEmployees(SalaryProcessingDTO salaryPeriodDTO, List<SalaryGroupDTO> salaryGroups, List<BranchDTO> branches, List<DepartmentDTO> departments, ServiceHeader serviceHeader)
        {
            var result = new List<EmployeeDTO> { };

            if (salaryPeriodDTO != null)
            {
                var salaryGroupEmployees = new List<Guid>();

                if (salaryGroups != null && salaryGroups.Any())
                {
                    using (_dbContextScopeFactory.CreateReadOnly())
                    {
                        salaryGroups.ForEach(salaryGroup =>
                        {
                            // get the specification
                            var filter = SalaryCardSpecifications.SalaryCardWithSalaryGroupId(salaryGroup.Id);

                            ISpecification<SalaryCard> spec = filter;

                            //Query this criteria
                            var salaryCards = _salaryCardRepository.AllMatching(spec, serviceHeader);

                            if (salaryCards != null && salaryCards.Any())
                            {
                                salaryGroupEmployees.AddRange(salaryCards.Select(x => x.EmployeeId));
                            }
                        });

                        if (salaryGroupEmployees != null && salaryGroupEmployees.Any())
                        {
                            var branchEmployees = new List<Guid>();

                            var departmentEmployees = new List<Guid>();

                            if (branches != null && branches.Any())
                            {
                                branches.ForEach(branch =>
                                {
                                    // get the specification
                                    var filter = EmployeeSpecifications.EmployeeWithBranchId(branch.Id);

                                    ISpecification<Employee> spec = filter;

                                    //Query this criteria
                                    var employees = _employeeRepository.AllMatching(spec, serviceHeader);

                                    if (employees != null && employees.Any())
                                    {
                                        branchEmployees.AddRange(employees.Select(x => x.Id));
                                    }
                                });
                            }

                            if (departments != null && departments.Any())
                            {
                                departments.ForEach(department =>
                                {
                                    // get the specification
                                    var filter = EmployeeSpecifications.EmployeeWithDepartmentId(department.Id);

                                    ISpecification<Employee> spec = filter;

                                    //Query this criteria
                                    var employees = _employeeRepository.AllMatching(spec, serviceHeader);

                                    if (employees != null && employees.Any())
                                    {
                                        departmentEmployees.AddRange(employees.Select(x => x.Id));
                                    }
                                });
                            }

                            var listOfLists = new List<IEnumerable<Guid>> { salaryGroupEmployees };

                            if (branchEmployees.Any())
                                listOfLists.Add(branchEmployees);

                            if (departmentEmployees.Any())
                                listOfLists.Add(departmentEmployees);

                            var intersection = listOfLists
                                .Skip(1)
                                .Aggregate(new HashSet<Guid>(listOfLists.First()), (h, e) =>
                                {
                                    h.IntersectWith(e);

                                    return h;
                                });

                            if (intersection != null && intersection.Any())
                            {
                                foreach (var item in intersection)
                                {
                                    var targetEmployee = FindEmployee(item, serviceHeader);

                                    if (targetEmployee != null && targetEmployee.EmployeeTypeCategory == salaryPeriodDTO.EmployeeCategory)
                                        result.Add(targetEmployee);
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        public EmployeeDTO FindEmployee(int serialNumber, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = EmployeeSpecifications.EmployeeWithCustomerSerialNumber(serialNumber);

                ISpecification<Employee> spec = filter;

                var employeeCollection = _employeeRepository.AllMatching(spec, serviceHeader);

                if (employeeCollection != null)
                {
                    var projection = employeeCollection.ProjectedAsCollection<EmployeeDTO>();

                    return projection.Count == 1 ? projection[0] : null;
                }
                else return null;
            }
        }
    }
}
