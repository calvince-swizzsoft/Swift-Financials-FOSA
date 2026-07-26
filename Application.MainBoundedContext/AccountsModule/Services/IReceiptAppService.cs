using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IReceiptAppService
    {
        ReceiptDTO AddNewReceipt(ReceiptDTO receiptDTO, ServiceHeader serviceHeader);

        bool UpdateReceipt(ReceiptDTO receiptDTO, ServiceHeader serviceHeader);

        List<ReceiptDTO> FindReceipts(ServiceHeader serviceHeader);

        List<ReceiptLineDTO> FindReceiptLines(ServiceHeader serviceHeader);

        JournalDTO PostReceipt(ReceiptDTO receiptDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        // JournalDTO PayVendorInvoice(PaymentVoucherDTO paymentVoucherDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader);
    }
}
