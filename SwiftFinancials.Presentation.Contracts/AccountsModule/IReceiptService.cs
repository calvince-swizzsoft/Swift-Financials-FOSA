using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IReceiptService")]
    public interface IReceiptService
    {

        #region Receipt

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddReceipt(ReceiptDTO receiptDTO, AsyncCallback callback, Object state);
        ReceiptDTO EndAddReceipt(IAsyncResult result);


        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateReceipt(ReceiptDTO receiptDTO, AsyncCallback callback, Object state);
        bool EndUpdateReceipt(IAsyncResult result);


        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]

        IAsyncResult BeginFindReceipts(AsyncCallback callback, Object state);

        List<ReceiptDTO> EndFindReceipts(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]

        IAsyncResult BeginFindReceiptLines(AsyncCallback callback, Object state);

        List<ReceiptLineDTO> EndFindReceiptLines(IAsyncResult result);


        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]

        IAsyncResult BeginPostReceipt(ReceiptDTO receiptDTO, int moduleNavigationItemCode, AsyncCallback callback, Object state);

        JournalDTO EndPostReceipt(IAsyncResult result);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContract(typeof(ApplicationServiceError))]

        //IAsyncResult BeginPayVendorInvoice(PaymentVoucherDTO paymentVoucherDTO, int moduleNavigationItemCode, AsyncCallback callback, Object state);

        //JournalDTO EndPayVendorInvoice(IAsyncResult result);

        #endregion
    }
}