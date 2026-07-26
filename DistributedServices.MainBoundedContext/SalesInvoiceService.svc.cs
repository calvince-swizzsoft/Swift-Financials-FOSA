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
    public class SalesInvoiceService : ISalesInvoiceService
    {

        public readonly ISalesInvoiceAppService _salesInvoiceAppService;

        public SalesInvoiceService(ISalesInvoiceAppService salesInvoiceAppService)
        {

            Guard.ArgumentNotNull(salesInvoiceAppService, nameof(salesInvoiceAppService));

            _salesInvoiceAppService = salesInvoiceAppService;

        }

        public SalesInvoiceDTO AddSalesInvoice(SalesInvoiceDTO salesInvoiceDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salesInvoiceAppService.AddNewSalesInvoice(salesInvoiceDTO, serviceHeader);
        }

        public bool UpdateSalesInvoice(SalesInvoiceDTO salesInvoiceDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salesInvoiceAppService.UpdateSalesInvoice(salesInvoiceDTO, serviceHeader);
        }


        public SalesInvoiceDTO FindSalesInvoice(Guid salesInvoiceId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salesInvoiceAppService.FindSalesInvoice(salesInvoiceId, serviceHeader);
        }


        public List<SalesInvoiceDTO> FindSalesInvoices()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salesInvoiceAppService.FindSalesInvoices(serviceHeader);
        }



        public JournalDTO PostSalesInvoice(SalesInvoiceDTO salesInvoiceDTO, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salesInvoiceAppService.PostSalesInvoice(salesInvoiceDTO, moduleNavigationItemCode, serviceHeader);
        }
    }
}