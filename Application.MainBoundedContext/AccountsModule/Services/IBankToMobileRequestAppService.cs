using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IBankToMobileRequestAppService
    {
        BankToMobileRequestDTO AddNewBankToMobileRequest(BankToMobileRequestDTO bankToMobileRequestDTO, ServiceHeader serviceHeader);

        bool UpdateBankToMobileRequestResponse(Guid bankToMobileRequestId, string outgoingPlainTextPayload, string outgoingCipherTextPayload, ServiceHeader serviceHeader);

        bool UpdateBankToMobileRequestIPNStatus(Guid bankToMobileRequestId, int ipnStatus, string ipnResponse, ServiceHeader serviceHeader);

        bool ResetBankToMobileRequestsIPNStatus(Guid[] bankToMobileRequestIds, ServiceHeader serviceHeader);

        List<BankToMobileRequestDTO> FindBankToMobileRequests(ServiceHeader serviceHeader);

        List<BankToMobileRequestDTO> FindBankToMobileRequests(Guid[] bankToMobileRequestIds, ServiceHeader serviceHeader);

        PageCollectionInfo<BankToMobileRequestDTO> FindBankToMobileRequests(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<BankToMobileRequestDTO> FindThirdPartyNotifiableBankToMobileRequests(string text, int pageIndex, int pageSize, int daysCap, ServiceHeader serviceHeader);

        BankToMobileRequestDTO FindBankToMobileRequest(Guid bankToMobileRequestId, ServiceHeader serviceHeader);
    }
}
