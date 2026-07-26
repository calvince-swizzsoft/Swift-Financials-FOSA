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
    public class ReceiptService : IReceiptService
    {

        public readonly IReceiptAppService _receiptAppService;

        public ReceiptService(IReceiptAppService receiptAppService)
        {

            Guard.ArgumentNotNull(receiptAppService, nameof(receiptAppService));

            _receiptAppService = receiptAppService;

        }

        public ReceiptDTO AddReceipt(ReceiptDTO receiptDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _receiptAppService.AddNewReceipt(receiptDTO, serviceHeader);
        }

        public bool UpdateReceipt(ReceiptDTO receiptDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _receiptAppService.UpdateReceipt(receiptDTO, serviceHeader);
        }


        public List<ReceiptDTO> FindReceipts()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _receiptAppService.FindReceipts(serviceHeader);
        }


        public List<ReceiptLineDTO> FindReceiptLines()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _receiptAppService.FindReceiptLines(serviceHeader);
        }



        public JournalDTO PostReceipt(ReceiptDTO receiptDTO, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _receiptAppService.PostReceipt(receiptDTO, moduleNavigationItemCode, serviceHeader);
        }


        //public JournalDTO PayVendorInvoice(PaymentVoucherDTO paymentVoucherDTO, int moduleNavigationItemCode)
        //{
        //    var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);
        //    return _purchaseInvoiceAppService.PayVendorInvoice(paymentVoucherDTO, moduleNavigationItemCode, serviceHeader);
        //}
    }
}