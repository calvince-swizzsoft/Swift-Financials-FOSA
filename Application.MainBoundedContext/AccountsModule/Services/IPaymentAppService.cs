using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IPaymentAppService
    {
        PaymentDTO AddNewPayment(PaymentDTO paymenyDTO, ServiceHeader serviceHeader);

        bool UpdatePayment(PaymentDTO paymentDTO, ServiceHeader serviceHeader);

        List<PaymentDTO> FindPayments(ServiceHeader serviceHeader);

        List<PaymentLineDTO> FindPaymentLines(ServiceHeader serviceHeader);

        JournalDTO PostPayment(PaymentDTO paymentDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader);

       // JournalDTO PayVendorInvoice(PaymentVoucherDTO paymentVoucherDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader);
    }
}
