using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "ILoanProductAppraisalProductService")]
    public interface ILoanProductAppraisalProductService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddLoanProductAppraisalProduct(LoanProductAppraisalProductDTO loanProductAppraisalProductDTO, AsyncCallback callback, Object state);
        LoanProductAppraisalProductDTO EndAddLoanProductAppraisalProduct(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateLoanProductAppraisalProduct(LoanProductAppraisalProductDTO loanProductAppraisalProductDTO, AsyncCallback callback, Object state);
        bool EndUpdateLoanProductAppraisalProduct(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanProductAppraisalProducts(AsyncCallback callback, Object state);
        List<LoanProductAppraisalProductDTO> EndFindLoanProductAppraisalProducts(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanProductAppraisalProductsByCode(int code, AsyncCallback callback, Object state);
        List<LoanProductAppraisalProductDTO> EndFindLoanProductAppraisalProductsByCode(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanProductAppraisalProductsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<LoanProductAppraisalProductDTO> EndFindLoanProductAppraisalProductsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanProductAppraisalProductsByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<LoanProductAppraisalProductDTO> EndFindLoanProductAppraisalProductsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanProductAppraisalProductsByLoanProductAppraisalProductSectionAndFilterInPage(int LoanProductAppraisalProductSection, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<LoanProductAppraisalProductDTO> EndFindLoanProductAppraisalProductsByLoanProductAppraisalProductSectionAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanProductAppraisalProduct(Guid loanProductAppraisalProductId, AsyncCallback callback, Object state);
        LoanProductAppraisalProductDTO EndFindLoanProductAppraisalProduct(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDynamicChargesByLoanProductAppraisalProductId(Guid loanProductAppraisalProductId, AsyncCallback callback, Object state);
        List<DynamicChargeDTO> EndFindDynamicChargesByLoanProductAppraisalProductId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateDynamicChargesByLoanProductAppraisalProductId(Guid loanProductAppraisalProductId, List<DynamicChargeDTO> dynamicCharges, AsyncCallback callback, Object state);
        bool EndUpdateDynamicChargesByLoanProductAppraisalProductId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAppraisalProductsByLoanProductAppraisalProductId(Guid loanProductAppraisalProductId, AsyncCallback callback, Object state);
        ProductCollectionInfo EndFindAppraisalProductsByLoanProductAppraisalProductId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateAppraisalProductsByLoanProductAppraisalProductId(Guid loanProductAppraisalProductId, ProductCollectionInfo appraisalProductsTuple, AsyncCallback callback, Object state);
        bool EndUpdateAppraisalProductsByLoanProductAppraisalProductId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanCyclesByLoanProductAppraisalProductId(Guid loanProductAppraisalProductId, AsyncCallback callback, Object state);
        List<LoanCycleDTO> EndFindLoanCyclesByLoanProductAppraisalProductId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateLoanCyclesByLoanProductAppraisalProductId(Guid loanProductAppraisalProductId, List<LoanCycleDTO> loanCycles, AsyncCallback callback, Object state);
        bool EndUpdateLoanCyclesByLoanProductAppraisalProductId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanProductAppraisalProductDeductiblesByLoanProductAppraisalProductId(Guid loanProductAppraisalProductId, AsyncCallback callback, Object state);
        List<LoanProductAppraisalProductDTO> EndFindLoanProductAppraisalProductDeductiblesByLoanProductAppraisalProductId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateLoanProductAppraisalProductDeductiblesByLoanProductAppraisalProductId(Guid loanProductAppraisalProductId, List<LoanProductAppraisalProductDTO> LoanProductAppraisalProductDeductibles, AsyncCallback callback, Object state);
        bool EndUpdateLoanProductAppraisalProductDeductiblesByLoanProductAppraisalProductId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanProductAppraisalProductAuxilliaryAppraisalFactorsByLoanProductAppraisalProductId(Guid loanProductAppraisalProductId, AsyncCallback callback, Object state);
        List<LoanProductAppraisalProductDTO> EndFindLoanProductAppraisalProductAuxilliaryAppraisalFactorsByLoanProductAppraisalProductId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateLoanProductAppraisalProductAuxilliaryAppraisalFactorsByLoanProductAppraisalProductId(Guid loanProductAppraisalProductId, List<LoanProductAppraisalProductDTO> LoanProductAppraisalProductAuxilliaryAppraisalFactors, AsyncCallback callback, Object state);
        bool EndUpdateLoanProductAppraisalProductAuxilliaryAppraisalFactorsByLoanProductAppraisalProductId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginGetLoaneeAppraisalFactor(Guid loanProductAppraisalProductId, decimal totalValue, AsyncCallback callback, Object state);
        double EndGetLoaneeAppraisalFactor(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginGetGuarantorAppraisalFactor(Guid loanProductAppraisalProductId, decimal totalValue, AsyncCallback callback, Object state);
        double EndGetGuarantorAppraisalFactor(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCommissionsByLoanProductAppraisalProductId(Guid loanProductAppraisalProductId, int LoanProductAppraisalProductKnownChargeType, AsyncCallback callback, Object state);
        List<CommissionDTO> EndFindCommissionsByLoanProductAppraisalProductId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateCommissionsByLoanProductAppraisalProductId(Guid loanProductAppraisalProductId, List<CommissionDTO> commissions, int LoanProductAppraisalProductKnownChargeType, int LoanProductAppraisalProductChargeBasisValue, AsyncCallback callback, Object state);
        bool EndUpdateCommissionsByLoanProductAppraisalProductId(IAsyncResult result);
    }
}
