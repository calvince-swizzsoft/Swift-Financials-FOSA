using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
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
    public class ImprestService : IImprestService
    {

        public readonly IImprestAppService _imprestAppService;

        public ImprestService(IImprestAppService imprestAppService)
        {

            Guard.ArgumentNotNull(imprestAppService, nameof(imprestAppService));

            _imprestAppService = imprestAppService;

        }

        public ImprestDTO AddImprest(ImprestDTO imprestDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _imprestAppService.AddNewImprest(imprestDTO, serviceHeader);
        }

        public bool UpdateImprest(ImprestDTO imprestDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _imprestAppService.UpdateImprest(imprestDTO, serviceHeader);
        }


        public List<ImprestDTO> FindImprests()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _imprestAppService.FindImprests(serviceHeader);
        }


        public List<ImprestLineDTO> FindImprestLines()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _imprestAppService.FindImprestLines(serviceHeader);
        }



        public JournalDTO PostImprest(ImprestDTO imprestDTO, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _imprestAppService.PostImprest(imprestDTO, moduleNavigationItemCode, serviceHeader);
        }


        public JournalDTO PostSurrender(ImprestDTO imprestDTO, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _imprestAppService.PostSurrender(imprestDTO, moduleNavigationItemCode, serviceHeader);
        }


        //public JournalDTO PayImprest(PaymentVoucherDTO paymentVoucherDTO, int moduleNavigationItemCode)
        //{
        //    var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);
        //    return _imprestAppService.PayImprest(paymentVoucherDTO, moduleNavigationItemCode, serviceHeader);
        //}
    }
}