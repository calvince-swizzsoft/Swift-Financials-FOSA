using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using DistributedServices.MainBoundedContext.Identity;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web.Profile;
using System.Web.Security;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeAppService _employeeAppService;
        private readonly IEmployeeDisciplinaryCaseAppService _employeeDisciplinaryCaseAppService;
        private readonly IMembershipService _membershipService;

        public EmployeeService(
            IEmployeeAppService employeeAppService, IEmployeeDisciplinaryCaseAppService employeeDisciplinaryCaseAppService, IMembershipService membershipService)
        {
            Guard.ArgumentNotNull(employeeAppService, nameof(employeeAppService));

            Guard.ArgumentNotNull(employeeDisciplinaryCaseAppService, nameof(employeeDisciplinaryCaseAppService));

            Guard.ArgumentNotNull(membershipService, nameof(membershipService));

            _employeeAppService = employeeAppService;
            _employeeDisciplinaryCaseAppService = employeeDisciplinaryCaseAppService;
            _membershipService = membershipService;
        }

        #region Employee

        public EmployeeDTO AddEmployee(EmployeeDTO employeeDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeAppService.AddNewEmployee(employeeDTO, serviceHeader);
        }

        public bool UpdateEmployee(EmployeeDTO employeeDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeAppService.UpdateEmployee(employeeDTO, serviceHeader);
        }

        public bool CustomerIsEmployee(Guid customerId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeAppService.CustomerIsEmployee(customerId, serviceHeader);
        }

        public List<EmployeeDTO> FindEmployees()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var employeeDTOs = _employeeAppService.FindEmployees(serviceHeader);

            //PopulateEmployeeApplicationUserName(employeeDTOs, serviceHeader);

            return employeeDTOs;
        }

        public PageCollectionInfo<EmployeeDTO> FindEmployeesInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var employeePageCollectionInfo = _employeeAppService.FindEmployees(pageIndex, pageSize, serviceHeader);

           // PopulateEmployeeApplicationUserName(employeePageCollectionInfo.PageCollection, serviceHeader);

            return employeePageCollectionInfo;
        }

        public PageCollectionInfo<EmployeeDTO> FindEmployeesByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var employeePageCollectionInfo = _employeeAppService.FindEmployees(text, pageIndex, pageSize, serviceHeader);

            //PopulateEmployeeApplicationUserName(employeePageCollectionInfo.PageCollection, serviceHeader);

            return employeePageCollectionInfo;
        }

        public PageCollectionInfo<EmployeeDTO> FindEmployeesByDepartmentIdAndFilterInPage(Guid departmentId, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var employeePageCollectionInfo = _employeeAppService.FindEmployees(departmentId, text, pageIndex, pageSize, serviceHeader);

            //PopulateEmployeeApplicationUserName(employeePageCollectionInfo.PageCollection, serviceHeader);

            return employeePageCollectionInfo;
        }

        public EmployeeDTO FindEmployee(Guid employeeId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var employeeDTO = _employeeAppService.FindEmployee(employeeId, serviceHeader);

            if (employeeDTO != null)
            {
               // PopulateEmployeeApplicationUserName(new List<EmployeeDTO> { employeeDTO }, serviceHeader);
            }

            return employeeDTO;
        }

        public EmployeeDTO FindEmployeeBySerialNumber(int serialNumber)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var employeeDTO = _employeeAppService.FindEmployee(serialNumber, serviceHeader);

            if (employeeDTO != null)
            {
               // PopulateEmployeeApplicationUserName(new List<EmployeeDTO> { employeeDTO }, serviceHeader);
            }

            return employeeDTO;
        }

        public List<EmployeeDTO> FindEmployeesBySalaryGroupsBranchesAndDepartments(SalaryProcessingDTO salaryPeriodDTO, List<SalaryGroupDTO> salaryGroups, List<BranchDTO> branches, List<DepartmentDTO> departments)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var employeeDTOs = _employeeAppService.FindEmployees(salaryPeriodDTO, salaryGroups, branches, departments, serviceHeader);

            //PopulateEmployeeApplicationUserName(employeeDTOs, serviceHeader);

            return employeeDTOs;
        }

        private void PopulateEmployeeApplicationUserName(List<EmployeeDTO> employeeDTOs, ServiceHeader serviceHeader)
        {
            if (employeeDTOs != null && employeeDTOs.Any())
            {
                Membership.ApplicationName = string.Format("/{0}", serviceHeader.ApplicationDomainName);

                ProfileManager.ApplicationName = string.Format("/{0}", serviceHeader.ApplicationDomainName);

                var userDTOs = _membershipService.FindMemberships();

               // var membershipUserCollection = Membership.GetAllUsers();

                var profileCollection = new List<ProfileBase>();

                foreach (var item in userDTOs)
                {
                    var membershipUser = item as UserDTO;

                    var profile = ProfileBase.Create(membershipUser.UserName);

                    profileCollection.Add(profile);
                }

                if (profileCollection != null && profileCollection.Any())
                {
                    employeeDTOs.ForEach(item =>
                    {
                        var targetUserNames = from profile in profileCollection
                                              where profile["EmployeeId"] != null && Guid.Parse(profile["EmployeeId"].ToString()) == item.Id
                                              select profile.UserName;

                        if (targetUserNames != null)
                            item.ApplicationUserName = string.Join(",", targetUserNames);
                    });
                }
            }
        }

        #endregion

        #region EmployeeDisciplinaryCases

        public EmployeeDisciplinaryCaseDTO AddNewEmployeeDisciplinaryCase(EmployeeDisciplinaryCaseDTO employeeDisciplinaryCaseDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeDisciplinaryCaseAppService.AddNewEmployeeDisciplinaryCase(employeeDisciplinaryCaseDTO, serviceHeader);
        }

        public EmployeeDisciplinaryCaseDTO FindEmployeeDisciplinaryCase(Guid employeeDisciplinaryCaseId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeDisciplinaryCaseAppService.FindEmployeeDisciplinaryCase(employeeDisciplinaryCaseId, serviceHeader);
        }

        public List<EmployeeDisciplinaryCaseDTO> FindEmployeeDisciplinaryCases()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeDisciplinaryCaseAppService.FindEmployeeDisciplinaryCases(serviceHeader);
        }

        public PageCollectionInfo<EmployeeDisciplinaryCaseDTO> FindEmployeeDisciplinaryCasesInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeDisciplinaryCaseAppService.FindEmployeeDisciplinaryCases(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<EmployeeDisciplinaryCaseDTO> FindEmployeeDisciplinaryCasesByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeDisciplinaryCaseAppService.FindEmployeeDisciplinaryCases(text, pageIndex, pageSize, serviceHeader);
        }

        public bool UpdateEmployeeDisciplinaryCase(EmployeeDisciplinaryCaseDTO employeeDisciplinaryCaseDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeDisciplinaryCaseAppService.UpdateEmployeeDisciplinaryCase(employeeDisciplinaryCaseDTO, serviceHeader);
        }

        public PageCollectionInfo<EmployeeDisciplinaryCaseDTO> FindEmployeeDisciplinaryCasesByEmployeeId(Guid employeeId, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeDisciplinaryCaseAppService.FindEmployeeDisciplinaryCasesByEmployeeId(employeeId, pageIndex, pageSize, serviceHeader);
        }

        #endregion
    }
}