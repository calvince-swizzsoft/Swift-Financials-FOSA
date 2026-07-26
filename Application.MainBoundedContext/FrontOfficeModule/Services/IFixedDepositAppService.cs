using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.FrontOfficeModule.Services
{
    public interface IFixedDepositAppService
    {
        FixedDepositDTO InvokeFixedDeposit(FixedDepositDTO fixedDepositDTO, ServiceHeader serviceHeader);

        bool AuditFixedDeposit(FixedDepositDTO fixedDepositDTO, int fixedDepositAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader, bool suppressBalanceCheck = false);

        bool RevokeFixedDeposits(List<FixedDepositDTO> fixedDepositDTOs, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        bool PayFixedDeposit(FixedDepositDTO fixedDepositDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        List<FixedDepositPayableDTO> FindFixedDepositPayablesByFixedDepositId(Guid fixedDepositId, ServiceHeader serviceHeader);

        bool UpdateFixedDepositPayables(Guid fixedDepositId, List<FixedDepositPayableDTO> fixedDepositPayables, ServiceHeader serviceHeader);

        List<FixedDepositDTO> FindFixedDeposits(ServiceHeader serviceHeader);

        List<FixedDepositDTO> FindFixedDepositsByCustomerAccountId(Guid customerAccountId, ServiceHeader serviceHeader);

        PageCollectionInfo<FixedDepositDTO> FindFixedDeposits(Guid branchId, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<FixedDepositDTO> FindPayableFixedDeposits(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<FixedDepositDTO> FindRevocableFixedDeposits(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<FixedDepositDTO> FindFixedDeposits(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<FixedDepositDTO> FindFixedDepositsByStatus(int status, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        FixedDepositDTO FindFixedDeposit(Guid fixedDepositId, ServiceHeader serviceHeader);

        PageCollectionInfo<FixedDepositDTO> FindDueFixedDeposits(DateTime targetDate, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        bool ExecutePayableFixedDeposits(DateTime targetDate, int pageSize, ServiceHeader serviceHeader);
    }
}
