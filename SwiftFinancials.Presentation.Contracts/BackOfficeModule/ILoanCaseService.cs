using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.BackOfficeModule
{
    [ServiceContract(Name = "ILoanCaseService")]
    public interface ILoanCaseService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddLoanCase(LoanCaseDTO loanCaseDTO, AsyncCallback callback, Object state);
        LoanCaseDTO EndAddLoanCase(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateLoanCase(LoanCaseDTO loanCaseDTO, AsyncCallback callback, Object state);
        bool EndUpdateLoanCase(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateLoanGuarantorsByLoanCaseId(Guid loanCaseId, List<LoanGuarantorDTO> loanGuarantors, AsyncCallback callback, Object state);
        bool EndUpdateLoanGuarantorsByLoanCaseId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanGuarantorsByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<LoanGuarantorDTO> EndFindLoanGuarantorsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateLoanCollateralsByLoanCaseId(Guid loanCaseId, List<CustomerDocumentDTO> customerDocuments, AsyncCallback callback, Object state);
        bool EndUpdateLoanCollateralsByLoanCaseId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateAttachedLoansByLoanCaseId(Guid loanCaseId, List<AttachedLoanDTO> attachedLoans, AsyncCallback callback, Object state);
        bool EndUpdateAttachedLoansByLoanCaseId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateLoanAppraisalFactorsByLoanCaseId(Guid loanCaseId, List<LoanAppraisalFactorDTO> loanAppraisalFactors, AsyncCallback callback, Object state);
        bool EndUpdateLoanAppraisalFactorsByLoanCaseId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginSubstituteLoanGuarantors(Guid substituteGuarantorCustomerId, List<LoanGuarantorDTO> loansGuaranteed, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndSubstituteLoanGuarantors(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanCases( AsyncCallback callback, Object state);
        List<LoanCaseDTO> EndFindLoanCases(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanCasesInPage(int pageIndex, int pageSize, bool includeBatchStatus, AsyncCallback callback, Object state);
        PageCollectionInfo<LoanCaseDTO> EndFindLoanCasesInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanCasesByFilterInPage(string text, int loanCaseFilter, int pageIndex, int pageSize, bool includeBatchStatus, AsyncCallback callback, Object state);
        PageCollectionInfo<LoanCaseDTO> EndFindLoanCasesByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanCasesByStatusAndFilterInPage(int status, string text, int loanCaseFilter, int pageIndex, int pageSize, bool includeBatchStatus, AsyncCallback callback, Object state);
        PageCollectionInfo<LoanCaseDTO> EndFindLoanCasesByStatusAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanCasesByLoanProductSectionAndStatusAndFilterInPage(int loanProductSection, int status, DateTime startDate, DateTime endDate, string text, int loanCaseFilter, int pageIndex, int pageSize, bool includeBatchStatus, AsyncCallback callback, Object state);
        PageCollectionInfo<LoanCaseDTO> EndFindLoanCasesByLoanProductSectionAndStatusAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanCasesByLoanProductCategoryAndStatusAndFilterInPage(int loanProductCategory, int status, DateTime startDate, DateTime endDate, string text, int loanCaseFilter, decimal approvedAmountThreshold, int pageIndex, int pageSize, bool includeBatchStatus, AsyncCallback callback, Object state);
        PageCollectionInfo<LoanCaseDTO> EndFindLoanCasesByLoanProductCategoryAndStatusAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanCasesByLoanProductSectionAndFilterInPage(int loanProductSection, DateTime startDate, DateTime endDate, string text, int loanCaseFilter, int pageIndex, int pageSize, bool includeBatchStatus, AsyncCallback callback, Object state);
        PageCollectionInfo<LoanCaseDTO> EndFindLoanCasesByLoanProductSectionAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanCase(Guid loanCaseId, AsyncCallback callback, Object state);
        LoanCaseDTO EndFindLoanCase(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanCaseByLoanCaseNumber(int caseNumber, AsyncCallback callback, Object state);
        List<LoanCaseDTO> EndFindLoanCaseByLoanCaseNumber(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanGuarantor(Guid loanGuarantorId, AsyncCallback callback, Object state);
        LoanGuarantorDTO EndFindLoanGuarantor(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanGuarantorsByCustomerId(Guid customerId, AsyncCallback callback, Object state);
        List<LoanGuarantorDTO> EndFindLoanGuarantorsByCustomerId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanGuarantorsByCustomerIdAndFilterInPage(Guid customerId, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<LoanGuarantorDTO> EndFindLoanGuarantorsByCustomerIdAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanGuarantorsByLoanCaseId(Guid loanCaseId, AsyncCallback callback, Object state);
        List<LoanGuarantorDTO> EndFindLoanGuarantorsByLoanCaseId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAttachedLoansByLoanCaseId(Guid loanCaseId, AsyncCallback callback, Object state);
        List<AttachedLoanDTO> EndFindAttachedLoansByLoanCaseId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAppraiseLoanCase(LoanCaseDTO loanCaseDTO, int loanAppraisalOption, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndAppraiseLoanCase(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginApproveLoanCase(LoanCaseDTO loanCaseDTO, int loanApprovalOption, AsyncCallback callback, Object state);
        bool EndApproveLoanCase(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAuditLoanCase(LoanCaseDTO loanCaseDTO, int loanAuditOption, AsyncCallback callback, Object state);
        bool EndAuditLoanCase(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginCancelLoanCase(LoanCaseDTO loanCaseDTO, int loanCancellationOption, AsyncCallback callback, Object state);
        bool EndCancelLoanCase(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateLoanAppraisalFactors(Guid loanCaseId, List<LoanAppraisalFactorDTO> loanAppraisalFactors, AsyncCallback callback, Object state);
        bool EndUpdateLoanAppraisalFactors(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanAppraisalFactorsByLoanCaseId(Guid loanCaseId, AsyncCallback callback, Object state);
        List<LoanAppraisalFactorDTO> EndFindLoanAppraisalFactorsByLoanCaseId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanGuarantorsByLoaneeCustomerIdAndLoanProductId(Guid loaneeCustomerId, Guid loanProductId, AsyncCallback callback, Object state);
        List<LoanGuarantorDTO> EndFindLoanGuarantorsByLoaneeCustomerIdAndLoanProductId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLastLoanCaseByCustomerId(Guid customerId, Guid loanProductId, AsyncCallback callback, Object state);
        LoanCaseDTO EndFindLastLoanCaseByCustomerId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanCasesByCustomerIdAndLoanProductIdAndAuxiliaryLoanCondition(Guid customerId, Guid loanProductId, int auxiliaryLoanCondition, AsyncCallback callback, Object state);
        List<LoanCaseDTO> EndFindLoanCasesByCustomerIdAndLoanProductIdAndAuxiliaryLoanCondition(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAttachLoanGuarantors(Guid sourceCustomerAccountId, Guid destinationLoanProductId, List<LoanGuarantorDTO> loanGuarantors, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndAttachLoanGuarantors(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanCasesByCustomerIdInProcess(Guid customerId, AsyncCallback callback, Object state);
        List<LoanCaseDTO> EndFindLoanCasesByCustomerIdInProcess(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRestructureLoan(Guid branchId, Guid customerAccountId, double NPer, double Pmt, string reference, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndRestructureLoan(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanCollateralsByLoanCaseId(Guid loanCaseId, AsyncCallback callback, Object state);
        List<LoanCollateralDTO> EndFindLoanCollateralsByLoanCaseId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanGuarantorAttachmentHistoryByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<LoanGuarantorAttachmentHistoryDTO> EndFindLoanGuarantorAttachmentHistoryByStatusAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanGuarantorAttachmentHistoryEntriesByLoanGuarantorAttachmentHistoryId(Guid loanGuarantorAttachmentHistoryId, AsyncCallback callback, Object state);
        List<LoanGuarantorAttachmentHistoryEntryDTO> EndFindLoanGuarantorAttachmentHistoryEntriesByLoanGuarantorAttachmentHistoryId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanGuarantorAttachmentHistoryEntry(Guid loanGuarantorAttachmentHistoryEntryId, AsyncCallback callback, Object state);
        LoanGuarantorAttachmentHistoryEntryDTO EndFindLoanGuarantorAttachmentHistoryEntry(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRelieveLoanGuarantors(Guid loanGuarantorAttachmentHistoryId, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndRelieveLoanGuarantors(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRemoveLoanGuarantors(List<LoanGuarantorDTO> loanGuarantorDTOs, AsyncCallback callback, Object state);
        bool EndRemoveLoanGuarantors(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddLoanGuarantor(LoanGuarantorDTO loanGuarantorDTO, AsyncCallback callback, Object state);
        LoanGuarantorDTO EndAddLoanGuarantor(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginReleaseLoanGuarantorsByLoaneeCustomerAccount(CustomerAccountDTO customerAccountDTO, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndReleaseLoanGuarantorsByLoaneeCustomerAccount(IAsyncResult result);


        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginMarkLoanCaseDisbursed(LoanDisbursementBatchEntryDTO loanDisbursementBatchEntryDTO, AsyncCallback callback, Object state);
        bool EndMarkLoanCaseDisbursed(IAsyncResult result);
    }
}
