using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentAppService _departmentAppService;

        public DepartmentService(
            IDepartmentAppService departmentAppService)
        {
            Guard.ArgumentNotNull(departmentAppService, nameof(departmentAppService));

            _departmentAppService = departmentAppService;
        }

        #region Department

        public DepartmentDTO AddDepartment(DepartmentDTO departmentDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _departmentAppService.AddNewDepartment(departmentDTO, serviceHeader);
        }

        public bool UpdateDepartment(DepartmentDTO departmentDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _departmentAppService.UpdateDepartment(departmentDTO, serviceHeader);
        }

        public List<DepartmentDTO> FindDepartments()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _departmentAppService.FindDepartments(serviceHeader);
        }

        public DepartmentDTO FindDepartment(Guid departmentId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _departmentAppService.FindDepartment(departmentId, serviceHeader);
        }

        public PageCollectionInfo<DepartmentDTO> FindDepartmentsInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _departmentAppService.FindDepartments(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<DepartmentDTO> FindDepartmentsByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _departmentAppService.FindDepartments(text, pageIndex, pageSize, serviceHeader);
        }

        #endregion
    }
}
