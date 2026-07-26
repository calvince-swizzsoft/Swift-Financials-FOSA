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
    public class SalesCreditMemoService : ISalesCreditMemoService
    {
        public readonly ISalesCreditMemoAppService _salesCreditMemoAppService;

        public SalesCreditMemoService(ISalesCreditMemoAppService salesCreditMemoAppService)
        {
            Guard.ArgumentNotNull(salesCreditMemoAppService, nameof(salesCreditMemoAppService));
            _salesCreditMemoAppService = salesCreditMemoAppService;
        }

        public SalesCreditMemoDTO AddSalesCreditMemo(SalesCreditMemoDTO salesCreditMemoDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);
            return _salesCreditMemoAppService.AddNewSalesCreditMemo(salesCreditMemoDTO, serviceHeader);
        }

        public bool UpdateSalesCreditMemo(SalesCreditMemoDTO salesCreditMemoDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);
            return _salesCreditMemoAppService.UpdateSalesCreditMemo(salesCreditMemoDTO, serviceHeader);
        }

        public List<SalesCreditMemoDTO> FindSalesCreditMemos()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);
            return _salesCreditMemoAppService.FindSalesCreditMemos(serviceHeader);
        }

        public JournalDTO PostSalesCreditMemo(SalesCreditMemoDTO salesCreditMemoDTO, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);
            return _salesCreditMemoAppService.PostSalesCreditMemo(salesCreditMemoDTO, moduleNavigationItemCode, serviceHeader);
        }
    }
}