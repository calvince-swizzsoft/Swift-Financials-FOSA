using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.BackOfficeModule;
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
    public class LoanDisbursementBatchService : ILoanDisbursementBatchService
    {
        private readonly ILoanDisbursementBatchAppService _loanDisbursementBatchAppService;

        public LoanDisbursementBatchService(
            ILoanDisbursementBatchAppService loanDisbursementBatchAppService)
        {
            Guard.ArgumentNotNull(loanDisbursementBatchAppService, nameof(loanDisbursementBatchAppService));

            _loanDisbursementBatchAppService = loanDisbursementBatchAppService;
        }

        #region Loan Disbursement Batch

        public LoanDisbursementBatchDTO AddLoanDisbursementBatch(LoanDisbursementBatchDTO loanDisbursementBatchDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanDisbursementBatchAppService.AddNewLoanDisbursementBatch(loanDisbursementBatchDTO, serviceHeader);
        }

        public bool UpdateLoanDisbursementBatch(LoanDisbursementBatchDTO loanDisbursementBatchDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanDisbursementBatchAppService.UpdateLoanDisbursementBatch(loanDisbursementBatchDTO, serviceHeader);
        }

        public LoanDisbursementBatchEntryDTO AddLoanDisbursementBatchEntry(LoanDisbursementBatchEntryDTO loanDisbursementBatchEntryDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanDisbursementBatchAppService.AddNewLoanDisbursementBatchEntry(loanDisbursementBatchEntryDTO, serviceHeader);
        }

        public bool RemoveLoanDisbursementBatchEntries(List<LoanDisbursementBatchEntryDTO> loanDisbursementBatchEntryDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanDisbursementBatchAppService.RemoveLoanDisbursementBatchEntries(loanDisbursementBatchEntryDTOs, serviceHeader);
        }

        public bool UpdateLoanDisbursementBatchEntry(LoanDisbursementBatchEntryDTO loanDisbursementBatchEntryDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanDisbursementBatchAppService.UpdateLoanDisbursementBatchEntry(loanDisbursementBatchEntryDTO, serviceHeader);
        }

        public bool UpdateLoanDisbursementBatchEntries(Guid loanDisbursementBatchId, List<LoanDisbursementBatchEntryDTO> loanDisbursementBatchEntries)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanDisbursementBatchAppService.UpdateLoanDisbursementBatchEntries(loanDisbursementBatchId, loanDisbursementBatchEntries, serviceHeader);
        }

        public bool AuditLoanDisbursementBatch(LoanDisbursementBatchDTO loanDisbursementBatchDTO, int batchAuthOption)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanDisbursementBatchAppService.AuditLoanDisbursementBatch(loanDisbursementBatchDTO, batchAuthOption, serviceHeader);
        }

        public bool AuthorizeLoanDisbursementBatch(LoanDisbursementBatchDTO loanDisbursementBatchDTO, int batchAuthOption, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanDisbursementBatchAppService.AuthorizeLoanDisbursementBatch(loanDisbursementBatchDTO, batchAuthOption, moduleNavigationItemCode, serviceHeader);
        }

        public bool PostLoanDisbursementBatchEntry(Guid loanDisbursementBatchEntryId, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanDisbursementBatchAppService.PostLoanDisbursementBatchEntry(loanDisbursementBatchEntryId, moduleNavigationItemCode, serviceHeader);
        }

        public decimal DisburseMicroLoan(Guid alternateChannelLogId, Guid settlementChartOfAccountId, CustomerAccountDTO customerLoanAccountDTO, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanDisbursementBatchAppService.DisburseMicroLoan(alternateChannelLogId, settlementChartOfAccountId, customerLoanAccountDTO, totalValue, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, serviceHeader);
        }

        public List<LoanDisbursementBatchDTO> FindLoanDisbursementBatches()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanDisbursementBatchAppService.FindLoanDisbursementBatches(serviceHeader);
        }

        public PageCollectionInfo<LoanDisbursementBatchDTO> FindLoanDisbursementBatchesByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanDisbursementBatchAppService.FindLoanDisbursementBatches(status, startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public LoanDisbursementBatchDTO FindLoanDisbursementBatch(Guid loanDisbursementBatchId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanDisbursementBatchAppService.FindLoanDisbursementBatch(loanDisbursementBatchId, serviceHeader);
        }

        public List<LoanDisbursementBatchEntryDTO> FindLoanDisbursementBatchEntriesByLoanDisbursementBatchId(Guid loanDisbursementBatchId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanDisbursementBatchAppService.FindLoanDisbursementBatchEntriesByLoanDisbursementBatchId(loanDisbursementBatchId, serviceHeader);
        }

        public PageCollectionInfo<LoanDisbursementBatchEntryDTO> FindLoanDisbursementBatchEntriesByLoanDisbursementBatchIdInPage(Guid loanDisbursementBatchId, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanDisbursementBatchAppService.FindLoanDisbursementBatchEntriesByLoanDisbursementBatchId(loanDisbursementBatchId, text, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<LoanDisbursementBatchEntryDTO> FindLoanDisbursementBatchEntriesByLoanDisbursementBatchTypeInPage(int loanDisbursementBatchType, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanDisbursementBatchAppService.FindLoanDisbursementBatchEntriesByLoanDisbursementBatchType(loanDisbursementBatchType, startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public List<LoanDisbursementBatchEntryDTO> FindLoanDisbursementBatchEntriesByCustomerId(int loanDisbursementBatchType, Guid customerId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanDisbursementBatchAppService.FindLoanDisbursementBatchEntriesByCustomerId(loanDisbursementBatchType, customerId, serviceHeader);
        }

        public PageCollectionInfo<LoanDisbursementBatchEntryDTO> FindQueableLoanDisbursementBatchEntriesInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanDisbursementBatchAppService.FindQueableLoanDisbursementBatchEntries(pageIndex, pageSize, serviceHeader);
        }

        public bool ValidateLoanDisbursementBatchEntriesExceedTransactionThreshold(Guid loanDisbursementBatchId, Guid designationId, int transactionThresholdType)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _loanDisbursementBatchAppService.ValidateLoanDisbursementBatchEntriesExceedTransactionThreshold(loanDisbursementBatchId, designationId, transactionThresholdType, serviceHeader);
        }

        #endregion
    }
}
