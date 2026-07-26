using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.FrontOfficeModule.Services
{
    public interface IExternalChequeAppService
    {
        ExternalChequeDTO AddNewExternalCheque(ExternalChequeDTO externalChequeDTO, ServiceHeader serviceHeader);

        bool MarkExternalChequeCleared(Guid externalChequeId, ServiceHeader serviceHeader);

        List<ExternalChequeDTO> FindExternalCheques(ServiceHeader serviceHeader);

        PageCollectionInfo<ExternalChequeDTO> FindExternalCheques(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<ExternalChequeDTO> FindExternalCheques(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<ExternalChequeDTO> FindExternalCheques(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        ExternalChequeDTO FindExternalCheque(Guid externalChequeId, ServiceHeader serviceHeader);

        List<ExternalChequeDTO> FindUnClearedExternalChequesByCustomerAccountId(Guid customerAccountId, ServiceHeader serviceHeader);

        List<ExternalChequeDTO> FindUnTransferredExternalChequesByTellerId(Guid tellerId, string text, ServiceHeader serviceHeader);

        PageCollectionInfo<ExternalChequeDTO> FindUnTransferredExternalChequesByTellerId(Guid tellerId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        bool TransferExternalCheques(List<ExternalChequeDTO> externalChequeDTOs, TellerDTO tellerDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        PageCollectionInfo<ExternalChequeDTO> FindUnClearedExternalCheques(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<ExternalChequeDTO> FindUnClearedExternalCheques(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        bool ClearExternalCheque(ExternalChequeDTO externalChequeDTO, int clearingOption, int moduleNavigationItemCode, UnPayReasonDTO unPayReasonDTO, ServiceHeader serviceHeader);

        PageCollectionInfo<ExternalChequeDTO> FindUnBankedExternalCheques(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        bool BankExternalCheques(List<ExternalChequeDTO> externalChequeDTOs, BankLinkageDTO bankLinkageDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        List<ExternalChequePayableDTO> FindExternalChequePayablesByExternalChequeId(Guid externalChequeId, ServiceHeader serviceHeader);

        bool UpdateExternalChequePayables(Guid externalChequeId, List<ExternalChequePayableDTO> externalChequePayables, ServiceHeader serviceHeader);
    }
}
