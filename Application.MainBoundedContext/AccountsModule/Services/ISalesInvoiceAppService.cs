using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface ISalesInvoiceAppService
    {

        SalesInvoiceDTO AddNewSalesInvoice(SalesInvoiceDTO salesInvoiceDTO, ServiceHeader serviceHeader);

        bool UpdateSalesInvoice(SalesInvoiceDTO salesInvoiceDTO, ServiceHeader serviceHeader);

        List<SalesInvoiceDTO> FindSalesInvoices(ServiceHeader serviceHeader);

        JournalDTO PostSalesInvoice(SalesInvoiceDTO salesInvoiceDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        SalesInvoiceDTO FindSalesInvoice(Guid salesInvoiceId, ServiceHeader serviceHeader);

    }
}
