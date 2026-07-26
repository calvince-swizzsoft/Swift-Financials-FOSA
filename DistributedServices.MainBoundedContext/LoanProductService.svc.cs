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
    public class LoanProductService : ILoanProductService
    {
        private readonly ILoanProductAppService _loanProductAppService;

        public LoanProductService(
            ILoanProductAppService loanProductAppService)
        {
            Guard.ArgumentNotNull(loanProductAppService, nameof(loanProductAppService));

            _loanProductAppService = loanProductAppService;
        }

        #region Loan Product

        public LoanProductDTO AddLoanProduct(LoanProductDTO loanProductDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanProductAppService.AddNewLoanProduct(loanProductDTO, serviceHeader);
        }

        public bool UpdateLoanProduct(LoanProductDTO loanProductDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanProductAppService.UpdateLoanProduct(loanProductDTO, serviceHeader);
        }

        public List<LoanProductDTO> FindLoanProducts()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanProductAppService.FindLoanProducts(serviceHeader);
        }

        public List<LoanProductDTO> FindLoanProductsByCode(int code)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanProductAppService.FindLoanProducts(code, serviceHeader);
        }

        public List<LoanProductDTO> FindLoanProductsByInterestChargeMode(int interestChargeMode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanProductAppService.FindLoanProductsByInterestChargeMode(interestChargeMode, serviceHeader);
        }

        public PageCollectionInfo<LoanProductDTO> FindLoanProductsInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanProductAppService.FindLoanProducts(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<LoanProductDTO> FindLoanProductsByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanProductAppService.FindLoanProducts(text, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<LoanProductDTO> FindLoanProductsByLoanProductSectionAndFilterInPage(int loanProductSection, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanProductAppService.FindLoanProducts(loanProductSection, text, pageIndex, pageSize, serviceHeader);
        }

        public LoanProductDTO FindLoanProduct(Guid loanProductId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanProductAppService.FindLoanProduct(loanProductId, serviceHeader);
        }

        public List<DynamicChargeDTO> FindDynamicChargesByLoanProductId(Guid loanProductId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanProductAppService.FindDynamicCharges(loanProductId, serviceHeader);
        }

        public List<DynamicChargeDTO> FindDynamicChargesByLoanProductIdAndRecoveryMode(Guid loanProductId, int recoveryMode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanProductAppService.FindDynamicCharges(loanProductId, recoveryMode, serviceHeader);
        }

        public bool UpdateDynamicChargesByLoanProductId(Guid loanProductId, List<DynamicChargeDTO> dynamicCharges)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanProductAppService.UpdateDynamicCharges(loanProductId, dynamicCharges, serviceHeader);
        }

        public ProductCollectionInfo FindAppraisalProductsByLoanProductId(Guid loanProductId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanProductAppService.FindAppraisalProducts(loanProductId, serviceHeader);
        }

        public bool UpdateAppraisalProductsByLoanProductId(Guid loanProductId, ProductCollectionInfo appraisalProductsTuple)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanProductAppService.UpdateAppraisalProducts(loanProductId, appraisalProductsTuple, serviceHeader);
        }

        public List<LoanCycleDTO> FindLoanCyclesByLoanProductId(Guid loanProductId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanProductAppService.FindLoanCycles(loanProductId, serviceHeader);
        }

        public bool UpdateLoanCyclesByLoanProductId(Guid loanProductId, List<LoanCycleDTO> loanCycles)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanProductAppService.UpdateLoanCycles(loanProductId, loanCycles, serviceHeader);
        }

        public List<LoanProductAuxiliaryConditionDTO> FindLoanProductAuxiliaryConditions(Guid baseLoanProductId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanProductAppService.FindLoanProductAuxiliaryConditions(baseLoanProductId, serviceHeader);
        }

        public bool UpdateLoanProductAuxiliaryConditions(Guid baseLoanProductId, List<LoanProductAuxiliaryConditionDTO> loanProductAuxiliaryConditions)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanProductAppService.UpdateLoanProductAuxiliaryConditions(baseLoanProductId, loanProductAuxiliaryConditions, serviceHeader);
        }

        public List<LoanProductDeductibleDTO> FindLoanProductDeductiblesByLoanProductId(Guid loanProductId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var loanProductDeductibles = _loanProductAppService.FindLoanProductDeductibles(loanProductId, serviceHeader);

            _loanProductAppService.FetchLoanProductDeductiblesProductDescription(loanProductDeductibles, serviceHeader);

            return loanProductDeductibles;
        }

        public bool UpdateLoanProductDeductiblesByLoanProductId(Guid loanProductId, List<LoanProductDeductibleDTO> loanProductDeductibles)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanProductAppService.UpdateLoanProductDeductibles(loanProductId, loanProductDeductibles, serviceHeader);
        }

        public List<LoanProductAuxilliaryAppraisalFactorDTO> FindLoanProductAuxilliaryAppraisalFactorsByLoanProductId(Guid loanProductId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanProductAppService.FindLoanProductAuxilliaryAppraisalFactors(loanProductId, serviceHeader);
        }

        public bool UpdateLoanProductAuxilliaryAppraisalFactorsByLoanProductId(Guid loanProductId, List<LoanProductAuxilliaryAppraisalFactorDTO> loanProductAuxilliaryAppraisalFactors)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanProductAppService.UpdateLoanProductAuxilliaryAppraisalFactors(loanProductId, loanProductAuxilliaryAppraisalFactors, serviceHeader);
        }

        public double GetLoaneeAppraisalFactor(Guid loanProductId, decimal totalValue)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanProductAppService.GetLoaneeAppraisalFactor(loanProductId, totalValue, serviceHeader);
        }

        public double GetGuarantorAppraisalFactor(Guid loanProductId, decimal totalValue)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanProductAppService.GetGuarantorAppraisalFactor(loanProductId, totalValue, serviceHeader);
        }

        public List<CommissionDTO> FindCommissionsByLoanProductId(Guid loanProductId, int loanProductKnownChargeType)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanProductAppService.FindCommissions(loanProductId, loanProductKnownChargeType, serviceHeader);
        }

        public bool UpdateCommissionsByLoanProductId(Guid loanProductId, List<CommissionDTO> commissions, int loanProductKnownChargeType, int loanProductChargeBasisValue)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanProductAppService.UpdateCommissions(loanProductId, commissions, loanProductKnownChargeType, loanProductChargeBasisValue, serviceHeader);
        }

        #endregion
    }
}
