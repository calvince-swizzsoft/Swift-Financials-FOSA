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

        // AddNewBankToMobileRequest above does NOT debit any account or post any journal - it
        // never references a CustomerAccountId at all, it is purely an outbound-payout-intent
        // audit row (confirmed by reading its implementation). RequestPayout is the real,
        // money-moving primitive: balance-checks and posts a real double-entry debit journal
        // (mirroring MobileToBankRequestAppService's C2B posting pattern, in reverse - Debit the
        // customer's product account, Credit SystemGeneralLedgerAccountCode.MobileWalletB2CSettlement),
        // then records the same kind of BankToMobileRequest intent row for a future outbound
        // payout host to consume. Returns null on any failure (account not found, non-Savings/
        // Investment product, non-positive amount, amount exceeds AvailableBalance, no open
        // posting period, no chart-of-account mapping for MobileWalletB2CSettlement) - callers
        // cannot distinguish these cases from the return value alone, same as every other
        // Add/Update method in this codebase; check preconditions before calling if a specific
        // error message matters to the caller.
        BankToMobileRequestDTO RequestPayout(Guid customerAccountId, decimal amount, string accountNumber, int transactionType, ServiceHeader serviceHeader);

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
