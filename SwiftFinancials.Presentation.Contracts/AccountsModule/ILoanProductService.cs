using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "ILoanProductService")]
    public interface ILoanProductService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddLoanProduct(LoanProductDTO loanProductDTO, AsyncCallback callback, Object state);
        LoanProductDTO EndAddLoanProduct(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateLoanProduct(LoanProductDTO loanProductDTO, AsyncCallback callback, Object state);
        bool EndUpdateLoanProduct(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanProducts(AsyncCallback callback, Object state);
        List<LoanProductDTO> EndFindLoanProducts(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanProductsByCode(int code, AsyncCallback callback, Object state);
        List<LoanProductDTO> EndFindLoanProductsByCode(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanProductsByInterestChargeMode(int interestChargeMode, AsyncCallback callback, Object state);
        List<LoanProductDTO> EndFindLoanProductsByInterestChargeMode(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanProductsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<LoanProductDTO> EndFindLoanProductsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanProductsByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<LoanProductDTO> EndFindLoanProductsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanProductsByLoanProductSectionAndFilterInPage(int loanProductSection, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<LoanProductDTO> EndFindLoanProductsByLoanProductSectionAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanProduct(Guid loanProductId, AsyncCallback callback, Object state);
        LoanProductDTO EndFindLoanProduct(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDynamicChargesByLoanProductId(Guid loanProductId, AsyncCallback callback, Object state);
        List<DynamicChargeDTO> EndFindDynamicChargesByLoanProductId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDynamicChargesByLoanProductIdAndRecoveryMode(Guid loanProductId, int recoveryMode, AsyncCallback callback, Object state);
        List<DynamicChargeDTO> EndFindDynamicChargesByLoanProductIdAndRecoveryMode(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateDynamicChargesByLoanProductId(Guid loanProductId, List<DynamicChargeDTO> dynamicCharges, AsyncCallback callback, Object state);
        bool EndUpdateDynamicChargesByLoanProductId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAppraisalProductsByLoanProductId(Guid loanProductId, AsyncCallback callback, Object state);
        ProductCollectionInfo EndFindAppraisalProductsByLoanProductId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateAppraisalProductsByLoanProductId(Guid loanProductId, ProductCollectionInfo appraisalProductsTuple, AsyncCallback callback, Object state);
        bool EndUpdateAppraisalProductsByLoanProductId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanCyclesByLoanProductId(Guid loanProductId, AsyncCallback callback, Object state);
        List<LoanCycleDTO> EndFindLoanCyclesByLoanProductId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateLoanCyclesByLoanProductId(Guid loanProductId, List<LoanCycleDTO> loanCycles, AsyncCallback callback, Object state);
        bool EndUpdateLoanCyclesByLoanProductId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanProductAuxiliaryConditions(Guid baseLoanProductId, AsyncCallback callback, Object state);
        List<LoanProductAuxiliaryConditionDTO> EndFindLoanProductAuxiliaryConditions(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateLoanProductAuxiliaryConditions(Guid baseLoanProductId, List<LoanProductAuxiliaryConditionDTO> loanProductAuxiliaryConditions, AsyncCallback callback, Object state);
        bool EndUpdateLoanProductAuxiliaryConditions(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanProductDeductiblesByLoanProductId(Guid loanProductId, AsyncCallback callback, Object state);
        List<LoanProductDeductibleDTO> EndFindLoanProductDeductiblesByLoanProductId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateLoanProductDeductiblesByLoanProductId(Guid loanProductId, List<LoanProductDeductibleDTO> loanProductDeductibles, AsyncCallback callback, Object state);
        bool EndUpdateLoanProductDeductiblesByLoanProductId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanProductAuxilliaryAppraisalFactorsByLoanProductId(Guid loanProductId, AsyncCallback callback, Object state);
        List<LoanProductAuxilliaryAppraisalFactorDTO> EndFindLoanProductAuxilliaryAppraisalFactorsByLoanProductId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateLoanProductAuxilliaryAppraisalFactorsByLoanProductId(Guid loanProductId, List<LoanProductAuxilliaryAppraisalFactorDTO> loanProductAuxilliaryAppraisalFactors, AsyncCallback callback, Object state);
        bool EndUpdateLoanProductAuxilliaryAppraisalFactorsByLoanProductId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginGetLoaneeAppraisalFactor(Guid loanProductId, decimal totalValue, AsyncCallback callback, Object state);
        double EndGetLoaneeAppraisalFactor(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginGetGuarantorAppraisalFactor(Guid loanProductId, decimal totalValue, AsyncCallback callback, Object state);
        double EndGetGuarantorAppraisalFactor(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCommissionsByLoanProductId(Guid loanProductId, int loanProductKnownChargeType, AsyncCallback callback, Object state);
        List<CommissionDTO> EndFindCommissionsByLoanProductId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateCommissionsByLoanProductId(Guid loanProductId, List<CommissionDTO> commissions, int loanProductKnownChargeType, int loanProductChargeBasisValue, AsyncCallback callback, Object state);
        bool EndUpdateCommissionsByLoanProductId(IAsyncResult result);
    }
}
