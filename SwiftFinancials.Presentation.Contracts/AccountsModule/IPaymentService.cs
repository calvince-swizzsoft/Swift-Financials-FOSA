using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IPaymentService")]
    public interface IPaymentService
    {

        #region Payment

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddPayment(PaymentDTO paymentDTO, AsyncCallback callback, Object state);
        PaymentDTO EndAddPayment(IAsyncResult result);


        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdatePayment(PaymentDTO paymentDTO, AsyncCallback callback, Object state);
        bool EndUpdatePayment(IAsyncResult result);


        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]

        IAsyncResult BeginFindPayments(AsyncCallback callback, Object state);

        List<PaymentDTO> EndFindPayments(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]

        IAsyncResult BeginFindPaymentLines(AsyncCallback callback, Object state);

        List<PaymentLineDTO> EndFindPaymentLines(IAsyncResult result);


        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]

        IAsyncResult BeginPostPayment(PaymentDTO paymentDTO, int moduleNavigationItemCode, AsyncCallback callback, Object state);

        JournalDTO EndPostPayment(IAsyncResult result);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContract(typeof(ApplicationServiceError))]

        //IAsyncResult BeginPayVendorInvoice(PaymentVoucherDTO paymentVoucherDTO, int moduleNavigationItemCode, AsyncCallback callback, Object state);

        //JournalDTO EndPayVendorInvoice(IAsyncResult result);



        #endregion
    }
}