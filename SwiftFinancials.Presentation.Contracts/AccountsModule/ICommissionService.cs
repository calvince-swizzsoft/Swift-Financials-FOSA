using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "ICommissionService")]
    public interface ICommissionService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddCommission(CommissionDTO commissionDTO, AsyncCallback callback, Object state);
        CommissionDTO EndAddCommission(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateCommission(CommissionDTO commissionDTO, AsyncCallback callback, Object state);
        bool EndUpdateCommission(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCommissions(AsyncCallback callback, Object state);
        List<CommissionDTO> EndFindCommissions(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCommission(Guid commissionId, AsyncCallback callback, Object state);
        CommissionDTO EndFindCommission(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCommissionsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<CommissionDTO> EndFindCommissionsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCommissionsByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<CommissionDTO> EndFindCommissionsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindGraduatedScalesByCommissionId(Guid commissionId, AsyncCallback callback, Object state);
        List<GraduatedScaleDTO> EndFindGraduatedScalesByCommissionId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateGraduatedScalesByCommissionId(Guid commissionId, List<GraduatedScaleDTO> graduatedScales, AsyncCallback callback, Object state);
        bool EndUpdateGraduatedScalesByCommissionId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLeviesByCommissionId(Guid commissionId, AsyncCallback callback, Object state);
        List<LevyDTO> EndFindLeviesByCommissionId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateLeviesByCommissionId(Guid commissionId, List<LevyDTO> levies, AsyncCallback callback, Object state);
        bool EndUpdateLeviesByCommissionId(IAsyncResult result);


        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateCommissionLeviesByCommissionId(Guid commissionId, List<CommissionLevyDTO> commissionLevies, AsyncCallback callback, Object state);
        bool EndUpdateCommissionLeviesByCommissionId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCommissionSplitsByCommissionId(Guid commissionId, AsyncCallback callback, Object state);
        List<CommissionSplitDTO> EndFindCommissionSplitsByCommissionId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateCommissionSplitsByCommissionId(Guid commissionId, List<CommissionSplitDTO> commissionSplits, AsyncCallback callback, Object state);
        bool EndUpdateCommissionSplitsByCommissionId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginGetCommissionsForSystemTransactionType(int systemTransactionType, AsyncCallback callback, Object state);
        List<CommissionDTO> EndGetCommissionsForSystemTransactionType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddSystemTransactionTypeToCommissions(int systemTransactionType, CommissionDTO[] commissions, ChargeDTO chargeDTO, AsyncCallback callback, Object state);
        bool EndAddSystemTransactionTypeToCommissions(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRemoveSystemTransactionTypeFromCommissions(int systemTransactionType, CommissionDTO[] commissions, AsyncCallback callback, Object state);
        bool EndRemoveSystemTransactionTypeFromCommissions(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginIsSystemTransactionTypeInCommission(int systemTransactionType, Guid commissionId, AsyncCallback callback, Object state);
        bool EndIsSystemTransactionTypeInCommission(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginMapSystemTransactionTypeToCommissions(int systemTransactionType, CommissionDTO[] commissions, ChargeDTO chargeDTO, AsyncCallback callback, Object state);
        bool EndMapSystemTransactionTypeToCommissions(IAsyncResult result);
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateCommissionSplit(CommissionSplitDTO commissionSplitDTO, AsyncCallback callback, Object state);
        bool EndUpdateCommissionSplit(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginComputeTariffs(int systemTransactionType, decimal totalValue, CustomerAccountDTO customerAccountDTO, AsyncCallback callback, Object state);
        List<TariffWrapper> EndComputeTariffs(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginComputeTariffsByChequeType(Guid chequeTypeId, decimal totalValue, CustomerAccountDTO customerAccountDTO, AsyncCallback callback, Object state);
        List<TariffWrapper> EndComputeTariffsByChequeType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginComputeTariffsByCreditBatchEntry(CreditBatchEntryDTO creditBatchEntry, AsyncCallback callback, Object state);
        List<TariffWrapper> EndComputeTariffsByCreditBatchEntry(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginComputeTariffsBySavingsProduct(Guid savingsProductId, int savingsProductKnownChargeType, decimal totalValue, CustomerAccountDTO customerAccountDTO, AsyncCallback callback, Object state);
        List<TariffWrapper> EndComputeTariffsBySavingsProduct(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginComputeTariffsByLoanProduct(Guid loanProductId, int loanProductKnownChargeType, decimal bookBalance, decimal principalBalance, CustomerAccountDTO customerAccountDTO, AsyncCallback callback, Object state);
        List<TariffWrapper> EndComputeTariffsByLoanProduct(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginComputeTariffsByAlternateChannelType(int alternateChannelType, int alternateChannelTypeKnownChargeType, decimal totalValue, CustomerAccountDTO customerAccountDTO, AsyncCallback callback, Object state);
        List<TariffWrapper> EndComputeTariffsByAlternateChannelType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginComputeTariffsByTextAlert(int systemTransactionCode, decimal totalValue, CustomerAccountDTO customerAccountDTO, AsyncCallback callback, Object state);
        List<TariffWrapper> EndComputeTariffsByTextAlert(IAsyncResult result);
    }
}
