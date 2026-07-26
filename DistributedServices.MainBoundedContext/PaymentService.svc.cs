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
    public class PaymentService : IPaymentService
    {

        public readonly IPaymentAppService _paymentAppService;

        public PaymentService(IPaymentAppService paymentAppService)
        {

            Guard.ArgumentNotNull(paymentAppService, nameof(paymentAppService));

            _paymentAppService = paymentAppService;

        }

        public PaymentDTO AddPayment(PaymentDTO paymentDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _paymentAppService.AddNewPayment(paymentDTO, serviceHeader);
        }

        public bool UpdatePayment(PaymentDTO paymentDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _paymentAppService.UpdatePayment(paymentDTO, serviceHeader);
        }


        public List<PaymentDTO> FindPayments()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _paymentAppService.FindPayments(serviceHeader);
        }


        public List<PaymentLineDTO> FindPaymentLines()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _paymentAppService.FindPaymentLines(serviceHeader);
        }



        public JournalDTO PostPayment(PaymentDTO paymentDTO, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _paymentAppService.PostPayment(paymentDTO, moduleNavigationItemCode, serviceHeader);
        }


        //public JournalDTO PayVendorInvoice(PaymentVoucherDTO paymentVoucherDTO, int moduleNavigationItemCode)
        //{
        //    var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);
        //    return _purchaseInvoiceAppService.PayVendorInvoice(paymentVoucherDTO, moduleNavigationItemCode, serviceHeader);
        //}
    }
}