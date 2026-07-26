using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.RegistryModule.Services;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace DistributedServices.MainBoundedContext
{


    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class ARCustomerService : IARCustomerService
    {


        public readonly IARCustomerAppService _arCustomerAppService;

        public ARCustomerService(IARCustomerAppService arCustomerAppService)
        {

            Guard.ArgumentNotNull(arCustomerAppService, nameof(arCustomerAppService));

            _arCustomerAppService = arCustomerAppService;

        }

        public ARCustomerDTO AddARCustomer(ARCustomerDTO arCustomerDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);
            return _arCustomerAppService.AddNewARCustomer(arCustomerDTO, serviceHeader);
        }

        public bool UpdateARCustomer(ARCustomerDTO arCustomerDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);
            return _arCustomerAppService.UpdateARCustomer(arCustomerDTO, serviceHeader);
        }

        public List<ARCustomerDTO> FindARCustomers()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);
            return _arCustomerAppService.FindARCustomers(serviceHeader);
        }

        public ARCustomerDTO FindARCustomerById(Guid id)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);
            return _arCustomerAppService.FindARCustomerById(id, serviceHeader);
        }
    }

}