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
    [ServiceContract(Name = "ISalesInvoiceService")]
    public interface ISalesInvoiceService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddSalesInvoice(SalesInvoiceDTO salesInvoiceDTO, AsyncCallback callback, Object state);
        SalesInvoiceDTO EndAddSalesInvoice(IAsyncResult result);
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateSalesInvoice(SalesInvoiceDTO salesInvoiceDTO, AsyncCallback callback, Object state);
        bool EndUpdateSalesInvoice(IAsyncResult result);
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSalesInvoices(AsyncCallback callback, Object state);
        List<SalesInvoiceDTO> EndFindSalesInvoices(IAsyncResult result);
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginPostSalesInvoice(SalesInvoiceDTO salesInvoiceDTO, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        JournalDTO EndPostSalesInvoice(IAsyncResult result);
    }
}