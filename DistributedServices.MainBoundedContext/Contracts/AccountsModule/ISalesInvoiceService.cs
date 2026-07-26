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
    public interface ISalesInvoiceService
    {

        #region Purchase Invoice

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        SalesInvoiceDTO AddSalesInvoice(SalesInvoiceDTO salesInvoiceDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateSalesInvoice(SalesInvoiceDTO salesInvoiceDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        SalesInvoiceDTO FindSalesInvoice(Guid salesInvoiceId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<SalesInvoiceDTO> FindSalesInvoices();


        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        JournalDTO PostSalesInvoice(SalesInvoiceDTO salesInvoiceDTO, int moduleNavigationItemCode);


        #endregion
    }
}