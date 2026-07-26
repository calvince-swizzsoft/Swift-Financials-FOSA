using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface ILoanProductService
    {
        #region Loan Product

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        LoanProductDTO AddLoanProduct(LoanProductDTO loanProductDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateLoanProduct(LoanProductDTO loanProductDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<LoanProductDTO> FindLoanProducts();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<LoanProductDTO> FindLoanProductsByCode(int code);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<LoanProductDTO> FindLoanProductsByInterestChargeMode(int interestChargeMode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<LoanProductDTO> FindLoanProductsInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<LoanProductDTO> FindLoanProductsByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<LoanProductDTO> FindLoanProductsByLoanProductSectionAndFilterInPage(int loanProductSection, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        LoanProductDTO FindLoanProduct(Guid loanProductId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<DynamicChargeDTO> FindDynamicChargesByLoanProductId(Guid loanProductId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<DynamicChargeDTO> FindDynamicChargesByLoanProductIdAndRecoveryMode(Guid loanProductId, int recoveryMode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateDynamicChargesByLoanProductId(Guid loanProductId, List<DynamicChargeDTO> dynamicCharges);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        ProductCollectionInfo FindAppraisalProductsByLoanProductId(Guid loanProductId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateAppraisalProductsByLoanProductId(Guid loanProductId, ProductCollectionInfo appraisalProductsTuple);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<LoanCycleDTO> FindLoanCyclesByLoanProductId(Guid loanProductId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateLoanCyclesByLoanProductId(Guid loanProductId, List<LoanCycleDTO> loanCycles);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<LoanProductAuxiliaryConditionDTO> FindLoanProductAuxiliaryConditions(Guid baseLoanProductId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateLoanProductAuxiliaryConditions(Guid baseLoanProductId, List<LoanProductAuxiliaryConditionDTO> loanProductAuxiliaryConditions);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<LoanProductDeductibleDTO> FindLoanProductDeductiblesByLoanProductId(Guid loanProductId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateLoanProductDeductiblesByLoanProductId(Guid loanProductId, List<LoanProductDeductibleDTO> loanProductDeductibles);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<LoanProductAuxilliaryAppraisalFactorDTO> FindLoanProductAuxilliaryAppraisalFactorsByLoanProductId(Guid loanProductId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateLoanProductAuxilliaryAppraisalFactorsByLoanProductId(Guid loanProductId, List<LoanProductAuxilliaryAppraisalFactorDTO> loanProductAuxilliaryAppraisalFactors);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        double GetLoaneeAppraisalFactor(Guid loanProductId, decimal totalValue);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        double GetGuarantorAppraisalFactor(Guid loanProductId, decimal totalValue);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CommissionDTO> FindCommissionsByLoanProductId(Guid loanProductId, int loanProductKnownChargeType);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateCommissionsByLoanProductId(Guid loanProductId, List<CommissionDTO> commissions, int loanProductKnownChargeType, int loanProductChargeBasisValue);

        #endregion
    }
}
