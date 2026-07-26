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
    public class PurchaseInvoiceService : IPurchaseInvoiceService
    {

        public readonly IPurchaseInvoiceAppService _purchaseInvoiceAppService;

        public PurchaseInvoiceService (IPurchaseInvoiceAppService purchaseInvoiceAppService)
        {

            Guard.ArgumentNotNull(purchaseInvoiceAppService, nameof(purchaseInvoiceAppService));

            _purchaseInvoiceAppService = purchaseInvoiceAppService; 
  
        }

        public PurchaseInvoiceDTO AddPurchaseInvoice(PurchaseInvoiceDTO purchaseInvoiceDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _purchaseInvoiceAppService.AddNewPurchaseInvoice(purchaseInvoiceDTO, serviceHeader);
        }

        public bool UpdatePurchaseInvoice(PurchaseInvoiceDTO purchaseInvoiceDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _purchaseInvoiceAppService.UpdatePurchaseInvoice(purchaseInvoiceDTO, serviceHeader);
        }


        public List<PurchaseInvoiceDTO> FindPurchaseInvoices()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _purchaseInvoiceAppService.FindPurchaseInvoices(serviceHeader);
        }


        public List<PurchaseInvoiceLineDTO> FindPurchaseInvoiceLines()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _purchaseInvoiceAppService.FindPurchaseInvoiceLines(serviceHeader);
        }



        public JournalDTO PostPurchaseInvoice(PurchaseInvoiceDTO purchaseInvoiceDTO, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _purchaseInvoiceAppService.PostPurchaseInvoice(purchaseInvoiceDTO, moduleNavigationItemCode, serviceHeader);
        }


        public JournalDTO PayVendorInvoice(PaymentVoucherDTO paymentVoucherDTO, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);
            return _purchaseInvoiceAppService.PayVendorInvoice(paymentVoucherDTO, moduleNavigationItemCode, serviceHeader);   
        }
    }
}