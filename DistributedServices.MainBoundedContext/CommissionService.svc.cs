using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class CommissionService : ICommissionService
    {
        private readonly ICommissionAppService _commissionAppService;

        public CommissionService(
           ICommissionAppService commissionAppService)
        {
            Guard.ArgumentNotNull(commissionAppService, nameof(commissionAppService));

            _commissionAppService = commissionAppService;
        }

        #region Commission

        public CommissionDTO AddCommission(CommissionDTO commissionDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _commissionAppService.AddNewCommission(commissionDTO, serviceHeader);
        }

        public bool UpdateCommission(CommissionDTO commissionDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _commissionAppService.UpdateCommission(commissionDTO, serviceHeader);
        }

        public List<CommissionDTO> FindCommissions()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _commissionAppService.FindCommissions(serviceHeader);
        }

        public CommissionDTO FindCommission(Guid commissionId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _commissionAppService.FindCommission(commissionId, serviceHeader);
        }

        public PageCollectionInfo<CommissionDTO> FindCommissionsInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _commissionAppService.FindCommissions(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<CommissionDTO> FindCommissionsByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _commissionAppService.FindCommissions(text, pageIndex, pageSize, serviceHeader);
        }

        public List<GraduatedScaleDTO> FindGraduatedScalesByCommissionId(Guid commissionId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _commissionAppService.FindGraduatedScales(commissionId, serviceHeader);
        }

        public bool UpdateGraduatedScalesByCommissionId(Guid commissionId, List<GraduatedScaleDTO> graduatedScales)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _commissionAppService.UpdateGraduatedScales(commissionId, graduatedScales, serviceHeader);
        }

        public List<LevyDTO> FindLeviesByCommissionId(Guid commissionId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _commissionAppService.FindLevies(commissionId, serviceHeader);
        }

        public bool UpdateLeviesByCommissionId(Guid commissionId, List<LevyDTO> levies)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _commissionAppService.UpdateLevies(commissionId, levies, serviceHeader);
        }

        public List<CommissionSplitDTO> FindCommissionSplitsByCommissionId(Guid commissionId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _commissionAppService.FindCommissionSplits(commissionId, serviceHeader);
        }

        public bool UpdateCommissionSplitsByCommissionId(Guid commissionId, List<CommissionSplitDTO> commissionSplits)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _commissionAppService.UpdateCommissionSplits(commissionId, commissionSplits, serviceHeader);
        }

        public List<CommissionDTO> GetCommissionsForSystemTransactionType(int systemTransactionType)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _commissionAppService.GetCommissionsForSystemTransactionType(systemTransactionType, serviceHeader);
        }

        public bool AddSystemTransactionTypeToCommissions(int systemTransactionType, CommissionDTO[] commissions, ChargeDTO chargeDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _commissionAppService.AddSystemTransactionTypeToCommissions(systemTransactionType, commissions, chargeDTO, serviceHeader);
        }

        public bool RemoveSystemTransactionTypeFromCommissions(int systemTransactionType, CommissionDTO[] commissions)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _commissionAppService.RemoveSystemTransactionTypeFromCommissions(systemTransactionType, commissions, serviceHeader);
        }

        public bool IsSystemTransactionTypeInCommission(int systemTransactionType, Guid commissionId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _commissionAppService.IsSystemTransactionTypeInCommission(systemTransactionType, commissionId, serviceHeader);
        }

        public bool MapSystemTransactionTypeToCommissions(int systemTransactionType, CommissionDTO[] commissions, ChargeDTO chargeDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _commissionAppService.MapSystemTransactionTypeToCommissions(systemTransactionType, commissions, chargeDTO, serviceHeader);
        }

        public bool UpdateCommissionSplit(CommissionSplitDTO commissionSplitDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _commissionAppService.UpdateCommissionSplit(commissionSplitDTO, serviceHeader);
        }

        public List<TariffWrapper> ComputeTariffs(int systemTransactionType, decimal totalValue, CustomerAccountDTO customerAccountDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _commissionAppService.ComputeTariffsBySystemTransactionType(systemTransactionType, totalValue, customerAccountDTO, serviceHeader);
        }

        public List<TariffWrapper> ComputeTariffsByCreditBatchEntry(CreditBatchEntryDTO creditBatchEntry)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _commissionAppService.ComputeTariffsByCreditBatchEntry(creditBatchEntry, serviceHeader);
        }

        public List<TariffWrapper> ComputeTariffsByChequeType(Guid chequeTypeId, decimal totalValue, CustomerAccountDTO customerAccountDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _commissionAppService.ComputeTariffsByChequeType(chequeTypeId, totalValue, customerAccountDTO, serviceHeader);
        }

        public List<TariffWrapper> ComputeTariffsBySavingsProduct(Guid savingsProductId, int savingsProductKnownChargeType, decimal totalValue, CustomerAccountDTO customerAccountDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _commissionAppService.ComputeTariffsBySavingsProduct(savingsProductId, savingsProductKnownChargeType, totalValue, customerAccountDTO, serviceHeader);
        }

        public List<TariffWrapper> ComputeTariffsByLoanProduct(Guid loanProductId, int loanProductKnownChargeType, decimal bookBalance, decimal principalBalance, CustomerAccountDTO customerAccountDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _commissionAppService.ComputeTariffsByLoanProduct(loanProductId, loanProductKnownChargeType, bookBalance, principalBalance, customerAccountDTO, serviceHeader);
        }

        public List<TariffWrapper> ComputeTariffsByAlternateChannelType(int alternateChannelType, int alternateChannelTypeKnownChargeType, decimal totalValue, CustomerAccountDTO customerAccountDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _commissionAppService.ComputeTariffsByAlternateChannelType(alternateChannelType, alternateChannelTypeKnownChargeType, totalValue, customerAccountDTO, serviceHeader);
        }

        public List<TariffWrapper> ComputeTariffsByTextAlert(int systemTransactionCode, decimal totalValue, CustomerAccountDTO customerAccountDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _commissionAppService.ComputeTariffsByTextAlert(systemTransactionCode, totalValue, customerAccountDTO, serviceHeader);
        }

        #endregion
    }
}
