using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface ICommissionService
    {
        #region Commission

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        CommissionDTO AddCommission(CommissionDTO commissionDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateCommission(CommissionDTO commissionDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CommissionDTO> FindCommissions();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        CommissionDTO FindCommission(Guid commissionId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<CommissionDTO> FindCommissionsInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<CommissionDTO> FindCommissionsByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<GraduatedScaleDTO> FindGraduatedScalesByCommissionId(Guid commissionId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateGraduatedScalesByCommissionId(Guid commissionId, List<GraduatedScaleDTO> graduatedScales);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<LevyDTO> FindLeviesByCommissionId(Guid commissionId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateLeviesByCommissionId(Guid commissionId, List<LevyDTO> levies);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CommissionSplitDTO> FindCommissionSplitsByCommissionId(Guid commissionId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateCommissionSplitsByCommissionId(Guid commissionId, List<CommissionSplitDTO> commissionSplits);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CommissionDTO> GetCommissionsForSystemTransactionType(int systemTransactionType);
        
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AddSystemTransactionTypeToCommissions(int systemTransactionType, CommissionDTO[] commissions, ChargeDTO chargeDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RemoveSystemTransactionTypeFromCommissions(int systemTransactionType, CommissionDTO[] commissions);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool IsSystemTransactionTypeInCommission(int systemTransactionType, Guid commissionId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool MapSystemTransactionTypeToCommissions(int systemTransactionType, CommissionDTO[] commissions, ChargeDTO chargeDTO);
        
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateCommissionSplit(CommissionSplitDTO commissionSplitDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<TariffWrapper> ComputeTariffs(int systemTransactionType, decimal totalValue, CustomerAccountDTO customerAccountDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<TariffWrapper> ComputeTariffsByChequeType(Guid chequeTypeId, decimal totalValue, CustomerAccountDTO customerAccountDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<TariffWrapper> ComputeTariffsByCreditBatchEntry(CreditBatchEntryDTO creditBatchEntry);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<TariffWrapper> ComputeTariffsBySavingsProduct(Guid savingsProductId, int savingsProductKnownChargeType, decimal totalValue, CustomerAccountDTO customerAccountDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<TariffWrapper> ComputeTariffsByLoanProduct(Guid loanProductId, int loanProductKnownChargeType, decimal bookBalance, decimal principalBalance, CustomerAccountDTO customerAccountDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<TariffWrapper> ComputeTariffsByAlternateChannelType(int alternateChannelType, int alternateChannelTypeKnownChargeType, decimal totalValue, CustomerAccountDTO customerAccountDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<TariffWrapper> ComputeTariffsByTextAlert(int systemTransactionCode, decimal totalValue, CustomerAccountDTO customerAccountDTO);

        #endregion
    }
}
