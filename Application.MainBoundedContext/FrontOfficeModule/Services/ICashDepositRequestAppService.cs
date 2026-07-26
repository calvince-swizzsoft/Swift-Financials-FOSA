using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.FrontOfficeModule.Services
{
    public interface ICashDepositRequestAppService
    {
        CashDepositRequestDTO AddNewCashDepositRequest(CashDepositRequestDTO cashDepositRequestDTO, ServiceHeader serviceHeader);

        bool AuthorizeCashDepositRequest(CashDepositRequestDTO cashDepositRequestDTO, int customerTransactionAuthOption, ServiceHeader serviceHeader);

        bool PostCashDepositRequest(CashDepositRequestDTO cashDepositRequestDTO, ServiceHeader serviceHeader);

        List<CashDepositRequestDTO> FindCashDepositRequests(ServiceHeader serviceHeader);

        List<CashDepositRequestDTO> FindActionableCashDepositRequestsByCustomerAccount(CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader);

        PageCollectionInfo<CashDepositRequestDTO> FindCashDepositRequests(DateTime startDate, DateTime endDate, int status, string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        CashDepositRequestDTO FindCashDepositRequest(Guid cashDepositRequestId, ServiceHeader serviceHeader);
    }
}
