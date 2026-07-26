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
    public class EmployeeTypeService : IEmployeeTypeService
    {
        private readonly IEmployeeTypeAppService _employeeTypeAppService;

        public EmployeeTypeService(
            IEmployeeTypeAppService employeeTypeAppService)
        {
            Guard.ArgumentNotNull(employeeTypeAppService, nameof(employeeTypeAppService));

            _employeeTypeAppService = employeeTypeAppService;
        }

        #region EmployeeType

        public EmployeeTypeDTO AddEmployeeType(EmployeeTypeDTO employeeTypeDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeTypeAppService.AddNewEmployeeType(employeeTypeDTO, serviceHeader);
        }

        public bool UpdateEmployeeType(EmployeeTypeDTO employeeTypeDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeTypeAppService.UpdateEmployeeType(employeeTypeDTO, serviceHeader);
        }

        public List<EmployeeTypeDTO> FindEmployeeTypes()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeTypeAppService.FindEmployeeTypes(serviceHeader);
        }

        public EmployeeTypeDTO FindEmployeeType(Guid employeeTypeId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeTypeAppService.FindEmployeeType(employeeTypeId, serviceHeader);
        }

        public PageCollectionInfo<EmployeeTypeDTO> FindEmployeeTypesInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeTypeAppService.FindEmployeeTypes(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<EmployeeTypeDTO> FindEmployeeTypesByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeTypeAppService.FindEmployeeTypes(text, pageIndex, pageSize, serviceHeader);
        }

        #endregion
    }
}
