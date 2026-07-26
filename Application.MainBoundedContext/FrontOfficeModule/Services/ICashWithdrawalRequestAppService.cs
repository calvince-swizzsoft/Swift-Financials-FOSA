using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.FrontOfficeModule.Services
{
    public interface ICashWithdrawalRequestAppService
    {
        CashWithdrawalRequestDTO AddNewCashWithdrawalRequest(CashWithdrawalRequestDTO cashWithdrawalRequestDTO, ServiceHeader serviceHeader);

        bool AuthorizeCashWithdrawalRequest(CashWithdrawalRequestDTO cashWithdrawalRequestDTO, int customerTransactionAuthOption, ServiceHeader serviceHeader);

        bool PayCashWithdrawalRequest(CashWithdrawalRequestDTO cashWithdrawalRequestDTO, PaymentVoucherDTO paymentVoucherDTO, ServiceHeader serviceHeader);

        List<CashWithdrawalRequestDTO> FindCashWithdrawalRequests(ServiceHeader serviceHeader);

        List<CashWithdrawalRequestDTO> FindMatureCashWithdrawalRequestsByCustomerAccountId(CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader);

        List<CashWithdrawalRequestDTO> FindMatureCashWithdrawalRequestsByChartOfAccountId(Guid chartOfAccountId, ServiceHeader serviceHeader);

        PageCollectionInfo<CashWithdrawalRequestDTO> FindCashWithdrawalRequests(DateTime startDate, DateTime endDate, int status, string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        CashWithdrawalRequestDTO FindCashWithdrawalRequest(Guid cashWithdrawalRequestId, ServiceHeader serviceHeader);
    }
}
