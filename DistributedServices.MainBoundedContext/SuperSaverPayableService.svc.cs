using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.FrontOfficeModule.Services;
using Application.MainBoundedContext.Services;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class SuperSaverPayableService : ISuperSaverPayableService
    {
        private readonly ISuperSaverPayableAppService _superSaverPayableAppService;
        private readonly ISqlCommandAppService _sqlCommandAppService;

        public SuperSaverPayableService(ISuperSaverPayableAppService superSaverPayableAppService, 
            ISqlCommandAppService sqlCommandAppService)
        {
            Guard.ArgumentNotNull(superSaverPayableAppService, nameof(superSaverPayableAppService));

            Guard.ArgumentNotNull(sqlCommandAppService, nameof(sqlCommandAppService));

            _superSaverPayableAppService = superSaverPayableAppService;

            _sqlCommandAppService = sqlCommandAppService;
        }
        public SuperSaverPayableDTO AddNewSuperSaverPayable(SuperSaverPayableDTO superSaverPayableDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _superSaverPayableAppService.AddNewSuperSaverPayable(superSaverPayableDTO, serviceHeader);
        }

        public SuperSaverPayableDTO FindSuperSaverPayable(Guid superSaverPayableId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _superSaverPayableAppService.FindSuperSaverPayable(superSaverPayableId, serviceHeader);
        }

        public PageCollectionInfo<SuperSaverPayableDTO> FindSuperSaverPayablesByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _superSaverPayableAppService.FindSuperSaverPayablesByStatus(status, startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }
        
        public bool AuditSuperSaverPayable(SuperSaverPayableDTO superSaverPayableDTO, int superSaverPayableAuditOption)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _superSaverPayableAppService.AuditSuperSaverPayable(superSaverPayableDTO, superSaverPayableAuditOption, serviceHeader);
        }

        public bool AuthorizeSuperSaverPayable(SuperSaverPayableDTO superSaverPayableDTO, int superSaverPayableAuthOption, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _superSaverPayableAppService.AuthorizeSuperSaverPayable(superSaverPayableDTO, superSaverPayableAuthOption, moduleNavigationItemCode, serviceHeader);
        }

        public SuperSaverInterestDTO FindCustomerSuperSaverPayable(Guid customerId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _sqlCommandAppService.FindCustomerSuperSaverPayable(customerId, serviceHeader);
        }
    }
}
