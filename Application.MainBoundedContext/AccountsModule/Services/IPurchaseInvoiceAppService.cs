using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IPurchaseInvoiceAppService
    {

        PurchaseInvoiceDTO AddNewPurchaseInvoice(PurchaseInvoiceDTO purchaseInvoiceDTO, ServiceHeader serviceHeader);

        bool UpdatePurchaseInvoice(PurchaseInvoiceDTO purchaseInvoiceDTO, ServiceHeader serviceHeader);

        List<PurchaseInvoiceDTO> FindPurchaseInvoices(ServiceHeader serviceHeader);

        PurchaseInvoiceDTO FindPurchaseInvoice(Guid purchaseInvoiceId, ServiceHeader serviceHeader);

        List<PurchaseInvoiceLineDTO> FindPurchaseInvoiceLines(ServiceHeader serviceHeader);

        JournalDTO PostPurchaseInvoice(PurchaseInvoiceDTO purchaseInvoiceDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        JournalDTO PayVendorInvoice(PaymentVoucherDTO paymentVoucherDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader);

    }
}
