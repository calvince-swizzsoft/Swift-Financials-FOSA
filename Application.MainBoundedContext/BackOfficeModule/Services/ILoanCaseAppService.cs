using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.BackOfficeModule.Services
{
    public interface ILoanCaseAppService
    {
        LoanCaseDTO AddNewLoanCase(LoanCaseDTO loanCaseDTO, ServiceHeader serviceHeader);

        Task<bool> UpdateLoanCaseAsync(LoanCaseDTO loanCaseDTO, ServiceHeader serviceHeader);

        bool AppraiseLoanCase(LoanCaseDTO loanCaseDTO, int loanAppraisalOption, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        Task<bool> AppraiseLoanCaseAsync(LoanCaseDTO loanCaseDTO, int loanAppraisalOption, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        bool ApproveLoanCase(LoanCaseDTO loanCaseDTO, int loanApprovalOption, ServiceHeader serviceHeader);

        Task<bool> ApproveLoanCaseAsync(LoanCaseDTO loanCaseDTO, int loanApprovalOption, ServiceHeader serviceHeader);

        bool AuditLoanCase(LoanCaseDTO loanCaseDTO, int loanAuditOption, ServiceHeader serviceHeader);

        Task<bool> AuditLoanCaseAsync(LoanCaseDTO loanCaseDTO, int loanAuditOption, ServiceHeader serviceHeader);

        bool CancelLoanCase(LoanCaseDTO loanCaseDTO, int loanCancellationOption, ServiceHeader serviceHeader);

        Task<bool> CancelLoanCaseAsync(LoanCaseDTO loanCaseDTO, int loanCancellationOption, ServiceHeader serviceHeader);

        bool UpdateLoanAppraisalFactors(Guid loanCaseId, List<LoanAppraisalFactorDTO> loanAppraisalFactors, ServiceHeader serviceHeader);

        bool UpdateLoanGuarantors(Guid loanCaseId, List<LoanGuarantorDTO> loanGuarantors, ServiceHeader serviceHeader);

        bool UpdateLoanCollaterals(Guid loanCaseId, List<CustomerDocumentDTO> customerDocuments, ServiceHeader serviceHeader);

        bool UpdateAttachedLoans(Guid loanCaseId, List<AttachedLoanDTO> attachedLoans, ServiceHeader serviceHeader);

        bool SubstituteLoanGuarantors(Guid substituteGuarantorCustomerId, List<LoanGuarantorDTO> loansGuaranteed, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        Task<bool> SubstituteLoanGuarantorsAsync(Guid substituteGuarantorCustomerId, List<LoanGuarantorDTO> loansGuaranteed, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        bool ReleaseLoanGuarantors(Guid customerAccountId, ServiceHeader serviceHeader);

        Task<bool> ReleaseLoanGuarantorsAsync(Guid customerAccountId, ServiceHeader serviceHeader);

        bool ReleaseRefinancedLoanGuarantors(Guid customerAccountId, DateTime refinanceDate, ServiceHeader serviceHeader);

        Task<bool> ReleaseRefinancedLoanGuarantorsAsync(Guid customerAccountId, DateTime refinanceDate, ServiceHeader serviceHeader);

        bool AttachLoanGuarantors(Guid sourceCustomerAccountId, Guid destinationLoanProductId, List<LoanGuarantorDTO> loanGuarantors, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        bool RestructureLoan(Guid branchId, Guid customerAccountId, double NPer, double Pmt, string reference, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        bool MarkLoanCaseDisbursed(LoanDisbursementBatchEntryDTO loanDisbursementBatchEntryDTO, ServiceHeader serviceHeader);

        List<LoanCaseDTO> FindLoanCases(ServiceHeader serviceHeader);

        PageCollectionInfo<LoanCaseDTO> FindLoanCases(int pageIndex, int pageSize, bool includeBatchStatus, ServiceHeader serviceHeader);

        PageCollectionInfo<LoanCaseDTO> FindLoanCases(string text, int loanCaseFilter, int pageIndex, int pageSize, bool includeBatchStatus, ServiceHeader serviceHeader);

        PageCollectionInfo<LoanCaseDTO> FindLoanCasesByStatus(int status, string text, int loanCaseFilter, int pageIndex, int pageSize, bool includeBatchStatus, ServiceHeader serviceHeader);

        PageCollectionInfo<LoanCaseDTO> FindLoanCasesBySectionAndStatus(int loanProductSection, int status, DateTime startDate, DateTime endDate, string text, int loanCaseFilter, int pageIndex, int pageSize, bool includeBatchStatus, ServiceHeader serviceHeader);

        PageCollectionInfo<LoanCaseDTO> FindLoanCasesByCategoryAndStatus(int loanProductCategory, int status, DateTime startDate, DateTime endDate, string text, int loanCaseFilter, decimal approvedAmountThreshold, int pageIndex, int pageSize, bool includeBatchStatus, ServiceHeader serviceHeader);

        PageCollectionInfo<LoanCaseDTO> FindLoanCasesBySection(int loanProductSection, DateTime startDate, DateTime endDate, string text, int loanCaseFilter, int pageIndex, int pageSize, bool includeBatchStatus, ServiceHeader serviceHeader);

        LoanCaseDTO FindLoanCase(Guid loanCaseId, ServiceHeader serviceHeader);

        LoanGuarantorDTO FindLoanGuarantor(Guid loanGuarantorId, ServiceHeader serviceHeader);

        List<LoanGuarantorDTO> FindLoanGuarantorsByCustomerId(Guid customerId, ServiceHeader serviceHeader);

        PageCollectionInfo<LoanGuarantorDTO> FindLoanGuarantors(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<LoanGuarantorDTO> FindLoanGuarantorsByCustomerId(Guid customerId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        List<LoanGuarantorDTO> FindLoanGuarantorsByLoanCaseId(Guid loanCaseId, ServiceHeader serviceHeader);

        List<AttachedLoanDTO> FindAttachedLoansByLoanCaseId(Guid loanCaseId, ServiceHeader serviceHeader);

        List<LoanCaseDTO> FindLoanCaseByLoanCaseNumber(int caseNumber, ServiceHeader serviceHeader);

        List<LoanAppraisalFactorDTO> FindLoanAppraisalFactorsByLoanCaseId(Guid loanCaseId, ServiceHeader serviceHeader);

        List<LoanCaseDTO> FindLoanCasesByCustomerIdAndLoanProductId(Guid customerId, Guid loanProductId, ServiceHeader serviceHeader);

        List<LoanCaseDTO> FindLoanCasesByCustomerIdAndLoanProductIdAndAuxiliaryLoanCondition(Guid customerId, Guid loanProductId, int auxiliaryLoanCondition, ServiceHeader serviceHeader);

        List<LoanGuarantorDTO> FindLoanGuarantorsByLoaneeCustomerIdAndLoanProductId(Guid loaneeCustomerId, Guid loanProductId, ServiceHeader serviceHeader);

        List<LoanCaseDTO> FindLoanCasesByCustomerIdInProcess(Guid customerId, ServiceHeader serviceHeader);

        List<LoanCollateralDTO> FindLoanCollateralsByLoanCaseId(Guid loanCaseId, ServiceHeader serviceHeader);

        List<LoanCollateralDTO> FindLoanCollateralsByLoaneeCustomerIdAndLoanProductId(Guid loaneeCustomerId, Guid loanProductId, ServiceHeader serviceHeader);

        PageCollectionInfo<LoanGuarantorAttachmentHistoryDTO> FindLoanGuarantorAttachmentHistoryByStatus(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        List<LoanGuarantorAttachmentHistoryEntryDTO> FindLoanGuarantorAttachmentHistoryEntriesByLoanGuarantorAttachmentHistoryId(Guid loanGuarantorAttachmentHistoryId, ServiceHeader serviceHeader);

        LoanGuarantorAttachmentHistoryEntryDTO FindLoanGuarantorAttachmentHistoryEntry(Guid loanGuarantorAttachmentHistoryEntryId, ServiceHeader serviceHeader);

        bool RelieveLoanGuarantors(Guid loanGuarantorAttachmentHistoryId, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        bool RemoveLoanGuarantors(List<LoanGuarantorDTO> loanGuarantorDTOs, ServiceHeader serviceHeader);

        LoanGuarantorDTO AddNewLoanGuarantor(LoanGuarantorDTO loanGuarantorDTO, ServiceHeader serviceHeader);

        bool ReleaseLoanGuarantors(CustomerAccountDTO customerAccountDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        Task<bool> ReleaseLoanGuarantorsAsync(CustomerAccountDTO customerAccountDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader);

    }
}
