using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface ICommissionAppService
    {
        CommissionDTO AddNewCommission(CommissionDTO commissionDTO, ServiceHeader serviceHeader);

        bool UpdateCommission(CommissionDTO commissionDTO, ServiceHeader serviceHeader);

        List<CommissionDTO> FindCommissions(ServiceHeader serviceHeader);

        PageCollectionInfo<CommissionDTO> FindCommissions(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<CommissionDTO> FindCommissions(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        CommissionDTO FindCommission(Guid commissionId, ServiceHeader serviceHeader);

        List<GraduatedScaleDTO> FindGraduatedScales(Guid commissionId, ServiceHeader serviceHeader);

        List<GraduatedScaleDTO> FindCachedGraduatedScales(Guid commissionId, ServiceHeader serviceHeader);

        bool UpdateGraduatedScales(Guid commissionId, List<GraduatedScaleDTO> graduatedScales, ServiceHeader serviceHeader);

        List<LevyDTO> FindLevies(Guid commissionId, ServiceHeader serviceHeader);

        List<LevyDTO> FindCachedLevies(Guid commissionId, ServiceHeader serviceHeader);

        bool UpdateLevies(Guid commissionId, List<LevyDTO> levies, ServiceHeader serviceHeader);

        List<CommissionSplitDTO> FindCommissionSplits(Guid commissionId, ServiceHeader serviceHeader);

        List<CommissionSplitDTO> FindCachedCommissionSplits(Guid commissionId, ServiceHeader serviceHeader);

        bool UpdateCommissionSplits(Guid commissionId, List<CommissionSplitDTO> commissionSplits, ServiceHeader serviceHeader);

        List<CommissionDTO> GetCommissionsForSystemTransactionType(int systemTransactionType, ServiceHeader serviceHeader);

        bool AddSystemTransactionTypeToCommissions(int systemTransactionType, CommissionDTO[] commissions, ChargeDTO chargeDTO, ServiceHeader serviceHeader);

        bool RemoveSystemTransactionTypeFromCommissions(int systemTransactionType, CommissionDTO[] commissions, ServiceHeader serviceHeader);

        bool IsSystemTransactionTypeInCommission(int systemTransactionType, Guid commissionId, ServiceHeader serviceHeader);

        bool MapSystemTransactionTypeToCommissions(int systemTransactionType, CommissionDTO[] commissions, ChargeDTO chargeDTO, ServiceHeader serviceHeader);

        bool UpdateCommissionSplit(CommissionSplitDTO commissionSplitDTO, ServiceHeader serviceHeader);

        List<TariffWrapper> ComputeTariffsBySystemTransactionType(int systemTransactionType, decimal totalValue, Guid debitChartOfAccountId, int debitChartOfAccountCode, string debitChartOfAccountName, ServiceHeader serviceHeader, bool useCache = true);

        List<TariffWrapper> ComputeTariffsBySystemTransactionType(int systemTransactionType, decimal totalValue, CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader, bool useCache = true);

        List<TariffWrapper> ComputeTariffsByLoanProduct(Guid loanProductId, int dynamicChargeRecoverySource, int dynamicChargeRecoveryMode, decimal totalValue, CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader, int loanCaseLoanRegistrationTermInMonths = -1, decimal topUpValue = 0m, decimal attachedLoansAmount = 0m, bool useCache = true);

        List<TariffWrapper> ComputeTariffsByPayoutCreditType(Guid creditTypeId, decimal totalValue, CustomerAccountDTO customerAccount, ServiceHeader serviceHeader, bool useCache = true);

        List<TariffWrapper> ComputeTariffsByCheckOffCreditType(Guid creditTypeId, decimal totalValue, Guid debitChartOfAccountId, ServiceHeader serviceHeader, bool useCache = true);

        List<TariffWrapper> ComputeTariffsByCreditBatchEntry(CreditBatchEntryDTO creditBatchEntry, ServiceHeader serviceHeader, bool useCache = true);

        List<TariffWrapper> ComputeTariffsByChequeType(Guid chequeTypeId, decimal totalValue, CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader, bool useCache = true);

        List<TariffWrapper> ComputeTariffsByChequeType(Guid chequeTypeId, decimal totalValue, Guid debitChartOfAccountId, int debitChartOfAccountCode, string debitChartOfAccountName, ServiceHeader serviceHeader, bool useCache = true);

        List<TariffWrapper> ComputeTariffsByDebitType(Guid debitTypeId, decimal totalValue, double multiplier, CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader, bool useCache = true);

        List<TariffWrapper> ComputeTariffsByUnPayReason(Guid unPayReasonId, decimal totalValue, CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader, bool useCache = true);

        List<TariffWrapper> ComputeTariffsBySavingsProduct(Guid savingsProductId, int savingsProductKnownChargeType, decimal totalValue, CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader, double multiplier = 1, bool useCache = true);

        List<TariffWrapper> ComputeTariffsByLoanProduct(Guid loanProductId, int loanProductKnownChargeType, decimal bookBalance, decimal principalBalance, CustomerAccountDTO customerAccount, ServiceHeader serviceHeader, bool useCache = true);

        List<TariffWrapper> ComputeTariffsByDynamicCharges(List<DynamicChargeDTO> dynamicCharges, decimal totalValue, CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader, bool useCache = true);

        List<TariffWrapper> ComputeTariffsByAlternateChannelType(int alternateChannelType, int alternateChannelTypeKnownChargeType, decimal totalValue, CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader, bool useCache = true);

        List<TariffWrapper> ComputeTariffsByTextAlert(int systemTransactionCode, decimal totalValue, CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader, bool useCache = true);

        List<TariffWrapper> ComputeTariffsByWireTransferType(Guid wireTransferTypeId, decimal totalValue, CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader, bool useCache = true);
    }
}
