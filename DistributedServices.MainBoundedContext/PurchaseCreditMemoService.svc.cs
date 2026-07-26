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
    public class PurchaseCreditMemoService : IPurchaseCreditMemoService
    {
        public readonly IPurchaseCreditMemoAppService _iPurchaseCreditMemoAppService;

        public PurchaseCreditMemoService(IPurchaseCreditMemoAppService iPurchaseCreditMemoAppService)
        {
            Guard.ArgumentNotNull(iPurchaseCreditMemoAppService, nameof(iPurchaseCreditMemoAppService));
            _iPurchaseCreditMemoAppService = iPurchaseCreditMemoAppService;
        }

        public PurchaseCreditMemoDTO AddPurchaseCreditMemo(PurchaseCreditMemoDTO purchaseCreditMemoDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);
            return _iPurchaseCreditMemoAppService.AddNewPurchaseCreditMemo(purchaseCreditMemoDTO, serviceHeader);
        }

        public bool UpdatePurchaseCreditMemo(PurchaseCreditMemoDTO purchaseCreditMemoDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);
            return _iPurchaseCreditMemoAppService.UpdatePurchaseCreditMemo(purchaseCreditMemoDTO, serviceHeader);
        }

        public List<PurchaseCreditMemoDTO> FindPurchaseCreditMemos()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);
            return _iPurchaseCreditMemoAppService.FindPurchaseCreditMemos(serviceHeader);
        }

        public JournalDTO PostPurchaseCreditMemo(PurchaseCreditMemoDTO purchaseCreditMemoDTO, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);
            return _iPurchaseCreditMemoAppService.PostPurchaseCreditMemo(purchaseCreditMemoDTO, moduleNavigationItemCode, serviceHeader);
        }
    }
}