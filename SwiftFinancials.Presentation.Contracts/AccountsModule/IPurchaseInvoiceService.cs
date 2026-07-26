using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{

    [ServiceContract(Name = "IPurchaseInvoiceService")]
    public  interface IPurchaseInvoiceService
    {

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddPurchaseInvoice(PurchaseInvoiceDTO purchaseInvoiceDTO, AsyncCallback callback, Object state);
        PurchaseInvoiceDTO  EndAddPurchaseInvoice(IAsyncResult result);


        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdatePurchaseInvoice(PurchaseInvoiceDTO purchaseInvoiceDTO, AsyncCallback callback, Object state);
        bool EndUpdatePurchaseInvoice(IAsyncResult result);


        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]

        IAsyncResult BeginFindPurchaseInvoices(AsyncCallback callback, Object state);
        
        List<PurchaseInvoiceDTO> EndFindPurchaseInvoices(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]

        IAsyncResult BeginFindPurchaseInvoiceLines(AsyncCallback callback, Object state);

        List<PurchaseInvoiceLineDTO> EndFindPurchaseInvoiceLines(IAsyncResult result);


        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]

        IAsyncResult BeginPostPurchaseInvoice(PurchaseInvoiceDTO purchaseInvoiceDTO, int moduleNavigationItemCode, AsyncCallback callback, Object state);

        JournalDTO EndPostPurchaseInvoice(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]

        IAsyncResult BeginPayVendorInvoice(PaymentVoucherDTO paymentVoucherDTO, int moduleNavigationItemCode, AsyncCallback callback, Object state);

        JournalDTO EndPayVendorInvoice(IAsyncResult result);
    }
}
