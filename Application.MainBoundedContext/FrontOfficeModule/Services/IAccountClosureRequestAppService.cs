using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.FrontOfficeModule.Services
{
    public interface IAccountClosureRequestAppService
    {
        AccountClosureRequestDTO AddNewAccountClosureRequest(AccountClosureRequestDTO accountClosureRequestDTO, ServiceHeader serviceHeader);

        bool ApproveAccountClosureRequest(AccountClosureRequestDTO accountClosureRequestDTO, int accountClosureApprovalOption, ServiceHeader serviceHeader);

        bool AuditAccountClosureRequest(AccountClosureRequestDTO accountClosureRequestDTO, int accountClosureAuditOption, ServiceHeader serviceHeader);

        bool SettleAccountClosureRequest(AccountClosureRequestDTO accountClosureRequestDTO, int accountClosureSettlementOption, ServiceHeader serviceHeader);

        AccountClosureRequestDTO FindAccountClosureRequest(Guid accountClosureRequestId, ServiceHeader serviceHeader);

        List<AccountClosureRequestDTO> FindAccountClosureRequests(ServiceHeader serviceHeader);

        List<AccountClosureRequestDTO> FindAccountClosureRequestsByCustomerAccountId(Guid customerAccountId, ServiceHeader serviceHeader);

        PageCollectionInfo<AccountClosureRequestDTO> FindAccountClosureRequests(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<AccountClosureRequestDTO> FindAccountClosureRequests(string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<AccountClosureRequestDTO> FindAccountClosureRequests(DateTime startDate, DateTime endDate, int status, string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader);
    }
}
