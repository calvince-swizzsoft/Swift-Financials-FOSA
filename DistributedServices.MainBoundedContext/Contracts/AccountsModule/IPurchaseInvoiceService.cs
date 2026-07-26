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
    public interface IPurchaseInvoiceService
    {

        #region Purchase Invoice

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PurchaseInvoiceDTO AddPurchaseInvoice(PurchaseInvoiceDTO purchaseInvoiceDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdatePurchaseInvoice(PurchaseInvoiceDTO purchaseInvoiceDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<PurchaseInvoiceDTO> FindPurchaseInvoices();


        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<PurchaseInvoiceLineDTO> FindPurchaseInvoiceLines();


        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        JournalDTO PostPurchaseInvoice(PurchaseInvoiceDTO purchaseInvoiceDTO, int moduleNavigationItemCode);


        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]

        JournalDTO PayVendorInvoice(PaymentVoucherDTO paymentVoucherDTO, int moduleNavigationItemCode);

      


        #endregion
    }
}