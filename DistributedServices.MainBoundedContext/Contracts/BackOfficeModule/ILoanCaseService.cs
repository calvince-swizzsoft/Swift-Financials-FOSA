using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface ILoanCaseService
    {
        #region Loan Case
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool MarkLoanCaseDisbursed(LoanDisbursementBatchEntryDTO loanDisbursementBatchEntryDTO);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        LoanCaseDTO AddLoanCase(LoanCaseDTO loanCaseDTO);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> UpdateLoanCaseAsync(LoanCaseDTO loanCaseDTO);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateLoanGuarantorsByLoanCaseId(Guid loanCaseId, List<LoanGuarantorDTO> loanGuarantors);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateLoanCollateralsByLoanCaseId(Guid loanCaseId, List<CustomerDocumentDTO> customerDocuments);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateAttachedLoansByLoanCaseId(Guid loanCaseId, List<AttachedLoanDTO> attachedLoans);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateLoanAppraisalFactorsByLoanCaseId(Guid loanCaseId, List<LoanAppraisalFactorDTO> loanAppraisalFactors);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> SubstituteLoanGuarantorsAsync(Guid substituteGuarantorCustomerId, List<LoanGuarantorDTO> loansGuaranteed, int moduleNavigationItemCode);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<LoanCaseDTO> FindLoanCases(bool includeBatchStatus);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<LoanCaseDTO> FindLoanCasesInPage(int pageIndex, int pageSize, bool includeBatchStatus);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<LoanCaseDTO> FindLoanCasesByFilterInPage(string text, int loanCaseFilter, int pageIndex, int pageSize, bool includeBatchStatus);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<LoanCaseDTO> FindLoanCasesByStatusAndFilterInPage(int status, string text, int loanCaseFilter, int pageIndex, int pageSize, bool includeBatchStatus);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<LoanCaseDTO> FindLoanCasesByLoanProductSectionAndStatusAndFilterInPage(int loanProductSection, int status, DateTime startDate, DateTime endDate, string text, int loanCaseFilter, int pageIndex, int pageSize, bool includeBatchStatus);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<LoanCaseDTO> FindLoanCasesByLoanProductCategoryAndStatusAndFilterInPage(int loanProductCategory, int status, DateTime startDate, DateTime endDate, string text, int loanCaseFilter, decimal approvedAmountThreshold, int pageIndex, int pageSize, bool includeBatchStatus);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<LoanCaseDTO> FindLoanCasesByLoanProductSectionAndFilterInPage(int loanProductSection, DateTime startDate, DateTime endDate, string text, int loanCaseFilter, int pageIndex, int pageSize, bool includeBatchStatus);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        LoanCaseDTO FindLoanCase(Guid loanCaseId);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<LoanCaseDTO> FindLoanCaseByLoanCaseNumber(int caseNumber);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        LoanGuarantorDTO FindLoanGuarantor(Guid loanGuarantorId);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<LoanGuarantorDTO> FindLoanGuarantorsByCustomerId(Guid customerId);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<LoanGuarantorDTO> FindLoanGuarantorsByFilterInPage(string text, int pageIndex, int pageSize);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<LoanGuarantorDTO> FindLoanGuarantorsByCustomerIdAndFilterInPage(Guid customerId, string text, int pageIndex, int pageSize);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<LoanGuarantorDTO> FindLoanGuarantorsByLoanCaseId(Guid loanCaseId);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<AttachedLoanDTO> FindAttachedLoansByLoanCaseId(Guid loanCaseId);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> AppraiseLoanCaseAsync(LoanCaseDTO loanCaseDTO, int loanAppraisalOption, int moduleNavigationItemCode);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> ApproveLoanCaseAsync(LoanCaseDTO loanCaseDTO, int loanApprovalOption);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> AuditLoanCaseAsync(LoanCaseDTO loanCaseDTO, int loanAuditOption);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> CancelLoanCaseAsync(LoanCaseDTO loanCaseDTO, int loanCancellationOption);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateLoanAppraisalFactors(Guid loanCaseId, List<LoanAppraisalFactorDTO> loanAppraisalFactors);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<LoanAppraisalFactorDTO> FindLoanAppraisalFactorsByLoanCaseId(Guid loanCaseId);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<LoanGuarantorDTO> FindLoanGuarantorsByLoaneeCustomerIdAndLoanProductId(Guid loaneeCustomerId, Guid loanProductId);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        LoanCaseDTO FindLastLoanCaseByCustomerId(Guid customerId, Guid loanProductId);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<LoanCaseDTO> FindLoanCasesByCustomerIdAndLoanProductIdAndAuxiliaryLoanCondition(Guid customerId, Guid loanProductId, int auxiliaryLoanCondition);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AttachLoanGuarantors(Guid sourceCustomerAccountId, Guid destinationLoanProductId, List<LoanGuarantorDTO> loanGuarantors, int moduleNavigationItemCode);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<LoanCaseDTO> FindLoanCasesByCustomerIdInProcess(Guid customerId);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RestructureLoan(Guid branchId, Guid customerAccountId, double NPer, double Pmt, string reference, int moduleNavigationItemCode);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<LoanCollateralDTO> FindLoanCollateralsByLoanCaseId(Guid loanCaseId);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<LoanGuarantorAttachmentHistoryDTO> FindLoanGuarantorAttachmentHistoryByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<LoanGuarantorAttachmentHistoryEntryDTO> FindLoanGuarantorAttachmentHistoryEntriesByLoanGuarantorAttachmentHistoryId(Guid loanGuarantorAttachmentHistoryId);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        LoanGuarantorAttachmentHistoryEntryDTO FindLoanGuarantorAttachmentHistoryEntry(Guid loanGuarantorAttachmentHistoryEntryId);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RelieveLoanGuarantors(Guid loanGuarantorAttachmentHistoryId, int moduleNavigationItemCode);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RemoveLoanGuarantors(List<LoanGuarantorDTO> loanGuarantorDTOs);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        LoanGuarantorDTO AddLoanGuarantor(LoanGuarantorDTO loanGuarantorDTO);
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> ReleaseLoanGuarantorsByLoaneeCustomerAccountAsync(CustomerAccountDTO customerAccountDTO, int moduleNavigationItemCode);
        #endregion
    }
}