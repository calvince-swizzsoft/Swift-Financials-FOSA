using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface ILoanProductAppService
    {
        LoanProductDTO AddNewLoanProduct(LoanProductDTO loanProductDTO, ServiceHeader serviceHeader);

        bool UpdateLoanProduct(LoanProductDTO loanProductDTO, ServiceHeader serviceHeader);

        List<LoanProductDTO> FindLoanProducts(ServiceHeader serviceHeader);

        List<LoanProductDTO> FindCachedLoanProducts(ServiceHeader serviceHeader);

        List<LoanProductDTO> FindLoanProducts(int code, ServiceHeader serviceHeader);

        List<LoanProductDTO> FindLoanProductsByInterestChargeMode(int interestChargeMode, ServiceHeader serviceHeader);

        PageCollectionInfo<LoanProductDTO> FindLoanProducts(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<LoanProductDTO> FindLoanProducts(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<LoanProductDTO> FindLoanProducts(int loanProductSection, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        LoanProductDTO FindLoanProduct(Guid loanProductId, ServiceHeader serviceHeader);

        LoanProductDTO FindLoanProductByName(string name, ServiceHeader serviceHeader);

        LoanProductDTO FindCachedLoanProduct(Guid loanProductId, ServiceHeader serviceHeader);

        List<DynamicChargeDTO> FindDynamicCharges(Guid loanProductId, ServiceHeader serviceHeader);

        List<DynamicChargeDTO> FindDynamicCharges(Guid loanProductId, int recoveryMode, ServiceHeader serviceHeader);

        List<DynamicChargeDTO> FindCachedDynamicCharges(Guid loanProductId, ServiceHeader serviceHeader);

        bool UpdateDynamicCharges(Guid loanProductId, List<DynamicChargeDTO> dynamicCharges, ServiceHeader serviceHeader);

        ProductCollectionInfo FindAppraisalProducts(Guid loanProductId, ServiceHeader serviceHeader);

        bool UpdateAppraisalProducts(Guid loanProductId, ProductCollectionInfo appraisalProductsTuple, ServiceHeader serviceHeader);

        List<LoanCycleDTO> FindLoanCycles(Guid loanProductId, ServiceHeader serviceHeader);

        bool UpdateLoanCycles(Guid loanProductId, List<LoanCycleDTO> loanCycles, ServiceHeader serviceHeader);

        List<LoanProductAuxiliaryConditionDTO> FindLoanProductAuxiliaryConditions(Guid baseLoanProductId, ServiceHeader serviceHeader);

        bool UpdateLoanProductAuxiliaryConditions(Guid baseLoanProductId, List<LoanProductAuxiliaryConditionDTO> loanProductAuxiliaryConditions, ServiceHeader serviceHeader);

        List<LoanProductDeductibleDTO> FindLoanProductDeductibles(Guid loanProductId, ServiceHeader serviceHeader);

        bool UpdateLoanProductDeductibles(Guid loanProductId, List<LoanProductDeductibleDTO> loanProductDeductibles, ServiceHeader serviceHeader);

        void FetchLoanProductDeductiblesProductDescription(List<LoanProductDeductibleDTO> loanProductDeductibles, ServiceHeader serviceHeader, bool useCache = true);

        List<LoanProductAuxilliaryAppraisalFactorDTO> FindLoanProductAuxilliaryAppraisalFactors(Guid loanProductId, ServiceHeader serviceHeader);

        bool UpdateLoanProductAuxilliaryAppraisalFactors(Guid loanProductId, List<LoanProductAuxilliaryAppraisalFactorDTO> loanProductAuxilliaryAppraisalFactors, ServiceHeader serviceHeader);

        double GetLoaneeAppraisalFactor(Guid loanProductId, decimal totalValue, ServiceHeader serviceHeader);

        double GetGuarantorAppraisalFactor(Guid loanProductId, decimal totalValue, ServiceHeader serviceHeader);

        List<CommissionDTO> FindCommissions(Guid loanProductId, int loanProductKnownChargeType, ServiceHeader serviceHeader);

        List<CommissionDTO> FindCachedCommissions(Guid loanProductId, int loanProductKnownChargeType, ServiceHeader serviceHeader);

        bool UpdateCommissions(Guid loanProductId, List<CommissionDTO> commissionDTOs, int loanProductKnownChargeType, int loanProductChargeBasisValue, ServiceHeader serviceHeader);
    }
}
