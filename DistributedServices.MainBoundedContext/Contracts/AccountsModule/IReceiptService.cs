using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IReceiptService
    {

        #region Receipt

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        ReceiptDTO AddReceipt(ReceiptDTO receiptDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateReceipt(ReceiptDTO receiptDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<ReceiptDTO> FindReceipts();


        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<ReceiptLineDTO> FindReceiptLines();


        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        JournalDTO PostReceipt(ReceiptDTO receiptDTO, int moduleNavigationItemCode);


        //[OperationContract()]
        //[FaultContract(typeof(ApplicationServiceError))]

        //JournalDTO PayVendorInvoice(PaymentVoucherDTO paymentVoucherDTO, int moduleNavigationItemCode);




        #endregion
    }
}