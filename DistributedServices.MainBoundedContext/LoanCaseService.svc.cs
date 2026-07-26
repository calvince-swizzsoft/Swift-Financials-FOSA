using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.Services;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class LoanCaseService : ILoanCaseService
    {
        private readonly ILoanCaseAppService _loanCaseAppService;
        private readonly ISqlCommandAppService _sqlCommandAppService;

        public LoanCaseService(
            ILoanCaseAppService loanCaseAppService,
            ISqlCommandAppService sqlCommandAppService)
        {
            Guard.ArgumentNotNull(loanCaseAppService, nameof(loanCaseAppService));
            Guard.ArgumentNotNull(sqlCommandAppService, nameof(sqlCommandAppService));

            _loanCaseAppService = loanCaseAppService;
            _sqlCommandAppService = sqlCommandAppService;
        }

        #region Loan Case
        public bool MarkLoanCaseDisbursed(LoanDisbursementBatchEntryDTO loanDisbursementBatchEntryDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);
            return _loanCaseAppService.MarkLoanCaseDisbursed(loanDisbursementBatchEntryDTO, serviceHeader);
        }

        public LoanCaseDTO AddLoanCase(LoanCaseDTO loanCaseDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanCaseAppService.AddNewLoanCase(loanCaseDTO, serviceHeader);
        }

        public async Task<bool> UpdateLoanCaseAsync(LoanCaseDTO loanCaseDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _loanCaseAppService.UpdateLoanCaseAsync(loanCaseDTO, serviceHeader);
        }

        public bool UpdateLoanGuarantorsByLoanCaseId(Guid loanCaseId, List<LoanGuarantorDTO> loanGuarantors)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanCaseAppService.UpdateLoanGuarantors(loanCaseId, loanGuarantors, serviceHeader);
        }

        public bool UpdateLoanCollateralsByLoanCaseId(Guid loanCaseId, List<CustomerDocumentDTO> customerDocuments)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanCaseAppService.UpdateLoanCollaterals(loanCaseId, customerDocuments, serviceHeader);
        }

        public bool UpdateAttachedLoansByLoanCaseId(Guid loanCaseId, List<AttachedLoanDTO> attachedLoans)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanCaseAppService.UpdateAttachedLoans(loanCaseId, attachedLoans, serviceHeader);
        }

        public bool UpdateLoanAppraisalFactorsByLoanCaseId(Guid loanCaseId, List<LoanAppraisalFactorDTO> loanAppraisalFactors)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanCaseAppService.UpdateLoanAppraisalFactors(loanCaseId, loanAppraisalFactors, serviceHeader);
        }

        public List<LoanCaseDTO> FindLoanCases(bool includeBatchStatus)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanCaseAppService.FindLoanCases(serviceHeader);
        }

        public PageCollectionInfo<LoanCaseDTO> FindLoanCasesInPage(int pageIndex, int pageSize, bool includeBatchStatus)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanCaseAppService.FindLoanCases(pageIndex, pageSize, includeBatchStatus, serviceHeader);
        }

        public PageCollectionInfo<LoanCaseDTO> FindLoanCasesByFilterInPage(string text, int loanCaseFilter, int pageIndex, int pageSize, bool includeBatchStatus)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanCaseAppService.FindLoanCases(text, loanCaseFilter, pageIndex, pageSize, includeBatchStatus, serviceHeader);
        }

        public PageCollectionInfo<LoanCaseDTO> FindLoanCasesByStatusAndFilterInPage(int status, string text, int loanCaseFilter, int pageIndex, int pageSize, bool includeBatchStatus)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanCaseAppService.FindLoanCasesByStatus(status, text, loanCaseFilter, pageIndex, pageSize, includeBatchStatus, serviceHeader);
        }

        public PageCollectionInfo<LoanCaseDTO> FindLoanCasesByLoanProductSectionAndStatusAndFilterInPage(int loanProductSection, int status, DateTime startDate, DateTime endDate, string text, int loanCaseFilter, int pageIndex, int pageSize, bool includeBatchStatus)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanCaseAppService.FindLoanCasesBySectionAndStatus(loanProductSection, status, startDate, endDate, text, loanCaseFilter, pageIndex, pageSize, includeBatchStatus, serviceHeader);
        }

        public PageCollectionInfo<LoanCaseDTO> FindLoanCasesByLoanProductCategoryAndStatusAndFilterInPage(int loanProductCategory, int status, DateTime startDate, DateTime endDate, string text, int loanCaseFilter, decimal approvedAmountThreshold, int pageIndex, int pageSize, bool includeBatchStatus)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanCaseAppService.FindLoanCasesByCategoryAndStatus(loanProductCategory, status, startDate, endDate, text, loanCaseFilter, approvedAmountThreshold, pageIndex, pageSize, includeBatchStatus, serviceHeader);
        }

        public PageCollectionInfo<LoanCaseDTO> FindLoanCasesByLoanProductSectionAndFilterInPage(int loanProductSection, DateTime startDate, DateTime endDate, string text, int loanCaseFilter, int pageIndex, int pageSize, bool includeBatchStatus)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanCaseAppService.FindLoanCasesBySection(loanProductSection, startDate, endDate, text, loanCaseFilter, pageIndex, pageSize, includeBatchStatus, serviceHeader);
        }

        public LoanCaseDTO FindLoanCase(Guid loanCaseId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanCaseAppService.FindLoanCase(loanCaseId, serviceHeader);
        }

        public LoanGuarantorDTO FindLoanGuarantor(Guid loanGuarantorId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanCaseAppService.FindLoanGuarantor(loanGuarantorId, serviceHeader);
        }

        public List<LoanGuarantorDTO> FindLoanGuarantorsByCustomerId(Guid customerId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanCaseAppService.FindLoanGuarantorsByCustomerId(customerId, serviceHeader);
        }

        public PageCollectionInfo<LoanGuarantorDTO> FindLoanGuarantorsByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanCaseAppService.FindLoanGuarantors(text, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<LoanGuarantorDTO> FindLoanGuarantorsByCustomerIdAndFilterInPage(Guid customerId, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanCaseAppService.FindLoanGuarantorsByCustomerId(customerId, text, pageIndex, pageSize, serviceHeader);
        }

        public List<LoanGuarantorDTO> FindLoanGuarantorsByLoanCaseId(Guid loanCaseId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanCaseAppService.FindLoanGuarantorsByLoanCaseId(loanCaseId, serviceHeader);
        }

        public List<LoanCaseDTO> FindLoanCaseByLoanCaseNumber(int caseNumber)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanCaseAppService.FindLoanCaseByLoanCaseNumber(caseNumber, serviceHeader);
        }

        public List<AttachedLoanDTO> FindAttachedLoansByLoanCaseId(Guid loanCaseId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanCaseAppService.FindAttachedLoansByLoanCaseId(loanCaseId, serviceHeader);
        }

        public async Task<bool> AppraiseLoanCaseAsync(LoanCaseDTO loanCaseDTO, int loanAppraisalOption, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _loanCaseAppService.AppraiseLoanCaseAsync(loanCaseDTO, loanAppraisalOption, moduleNavigationItemCode, serviceHeader);
        }

        public async Task<bool> ApproveLoanCaseAsync(LoanCaseDTO loanCaseDTO, int loanApprovalOption)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _loanCaseAppService.ApproveLoanCaseAsync(loanCaseDTO, loanApprovalOption, serviceHeader);
        }

        public async Task<bool> AuditLoanCaseAsync(LoanCaseDTO loanCaseDTO, int loanAuditOption)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _loanCaseAppService.AuditLoanCaseAsync(loanCaseDTO, loanAuditOption, serviceHeader);
        }

        public async Task<bool> CancelLoanCaseAsync(LoanCaseDTO loanCaseDTO, int loanCancellationOption)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _loanCaseAppService.CancelLoanCaseAsync(loanCaseDTO, loanCancellationOption, serviceHeader);
        }

        public bool UpdateLoanAppraisalFactors(Guid loanCaseId, List<LoanAppraisalFactorDTO> loanAppraisalFactors)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanCaseAppService.UpdateLoanAppraisalFactors(loanCaseId, loanAppraisalFactors, serviceHeader);
        }

        public async Task<bool> SubstituteLoanGuarantorsAsync(Guid substituteGuarantorCustomerId, List<LoanGuarantorDTO> loansGuaranteed, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _loanCaseAppService.SubstituteLoanGuarantorsAsync(substituteGuarantorCustomerId, loansGuaranteed, moduleNavigationItemCode, serviceHeader);
        }

        public List<LoanAppraisalFactorDTO> FindLoanAppraisalFactorsByLoanCaseId(Guid loanCaseId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanCaseAppService.FindLoanAppraisalFactorsByLoanCaseId(loanCaseId, serviceHeader);
        }

        public List<LoanCaseDTO> FindLoanCasesByCustomerIdAndLoanProductIdAndAuxiliaryLoanCondition(Guid customerId, Guid loanProductId, int auxiliaryLoanCondition)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanCaseAppService.FindLoanCasesByCustomerIdAndLoanProductIdAndAuxiliaryLoanCondition(customerId, loanProductId, auxiliaryLoanCondition, serviceHeader);
        }

        public List<LoanGuarantorDTO> FindLoanGuarantorsByLoaneeCustomerIdAndLoanProductId(Guid loaneeCustomerId, Guid loanProductId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanCaseAppService.FindLoanGuarantorsByLoaneeCustomerIdAndLoanProductId(loaneeCustomerId, loanProductId, serviceHeader);
        }

        public LoanCaseDTO FindLastLoanCaseByCustomerId(Guid customerId, Guid loanProductId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _sqlCommandAppService.FindLastLoanCaseByCustomerId(customerId, loanProductId, serviceHeader);
        }

        public bool AttachLoanGuarantors(Guid sourceCustomerAccountId, Guid destinationLoanProductId, List<LoanGuarantorDTO> loanGuarantors, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanCaseAppService.AttachLoanGuarantors(sourceCustomerAccountId, destinationLoanProductId, loanGuarantors, moduleNavigationItemCode, serviceHeader);
        }

        public bool RestructureLoan(Guid branchId, Guid customerAccountId, double NPer, double Pmt, string reference, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanCaseAppService.RestructureLoan(branchId, customerAccountId, NPer, Pmt, reference, moduleNavigationItemCode, serviceHeader);
        }

        public List<LoanCaseDTO> FindLoanCasesByCustomerIdInProcess(Guid customerId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanCaseAppService.FindLoanCasesByCustomerIdInProcess(customerId, serviceHeader);
        }

        public List<LoanCollateralDTO> FindLoanCollateralsByLoanCaseId(Guid loanCaseId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanCaseAppService.FindLoanCollateralsByLoanCaseId(loanCaseId, serviceHeader);
        }

        public PageCollectionInfo<LoanGuarantorAttachmentHistoryDTO> FindLoanGuarantorAttachmentHistoryByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanCaseAppService.FindLoanGuarantorAttachmentHistoryByStatus(status, startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public List<LoanGuarantorAttachmentHistoryEntryDTO> FindLoanGuarantorAttachmentHistoryEntriesByLoanGuarantorAttachmentHistoryId(Guid loanGuarantorAttachmentHistoryId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanCaseAppService.FindLoanGuarantorAttachmentHistoryEntriesByLoanGuarantorAttachmentHistoryId(loanGuarantorAttachmentHistoryId, serviceHeader);
        }

        public LoanGuarantorAttachmentHistoryEntryDTO FindLoanGuarantorAttachmentHistoryEntry(Guid loanGuarantorAttachmentHistoryEntryId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanCaseAppService.FindLoanGuarantorAttachmentHistoryEntry(loanGuarantorAttachmentHistoryEntryId, serviceHeader);
        }

        public bool RelieveLoanGuarantors(Guid loanGuarantorAttachmentHistoryId, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanCaseAppService.RelieveLoanGuarantors(loanGuarantorAttachmentHistoryId, moduleNavigationItemCode, serviceHeader);
        }

        public bool RemoveLoanGuarantors(List<LoanGuarantorDTO> loanGuarantorDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanCaseAppService.RemoveLoanGuarantors(loanGuarantorDTOs, serviceHeader);
        }

        public LoanGuarantorDTO AddLoanGuarantor(LoanGuarantorDTO loanGuarantorDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanCaseAppService.AddNewLoanGuarantor(loanGuarantorDTO, serviceHeader);
        }

        public async Task<bool> ReleaseLoanGuarantorsByLoaneeCustomerAccountAsync(CustomerAccountDTO customerAccountDTO, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _loanCaseAppService.ReleaseLoanGuarantorsAsync(customerAccountDTO, moduleNavigationItemCode, serviceHeader);
        }

        #endregion
    }
}
