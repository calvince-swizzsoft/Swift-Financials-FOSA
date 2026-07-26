using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.Services;
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
    public class CreditBatchService : ICreditBatchService
    {
        private readonly ICreditBatchAppService _creditBatchAppService;
        private readonly ICustomerAccountAppService _customerAccountAppService;
        private readonly ISqlCommandAppService _sqlCommandAppService;

        public CreditBatchService(
            ICreditBatchAppService creditBatchAppService,
            ICustomerAccountAppService customerAccountAppService,
            ISqlCommandAppService sqlCommandAppService)
        {
            Guard.ArgumentNotNull(creditBatchAppService, nameof(creditBatchAppService));
            Guard.ArgumentNotNull(customerAccountAppService, nameof(customerAccountAppService));
            Guard.ArgumentNotNull(sqlCommandAppService, nameof(sqlCommandAppService));

            _creditBatchAppService = creditBatchAppService;
            _customerAccountAppService = customerAccountAppService;
            _sqlCommandAppService = sqlCommandAppService;
        }

        #region Credit Batch

        public CreditBatchDTO AddCreditBatch(CreditBatchDTO creditBatchDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _creditBatchAppService.AddNewCreditBatch(creditBatchDTO, serviceHeader);
        }

        public bool UpdateCreditBatch(CreditBatchDTO creditBatchDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _creditBatchAppService.UpdateCreditBatch(creditBatchDTO, serviceHeader);
        }

        public CreditBatchEntryDTO AddCreditBatchEntry(CreditBatchEntryDTO creditBatchEntryDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _creditBatchAppService.AddNewCreditBatchEntry(creditBatchEntryDTO, serviceHeader);
        }

        public bool RemoveCreditBatchEntries(List<CreditBatchEntryDTO> creditBatchEntryDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _creditBatchAppService.RemoveCreditBatchEntries(creditBatchEntryDTOs, serviceHeader);
        }

        public bool UpdateCreditBatchEntry(CreditBatchEntryDTO creditBatchEntryDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _creditBatchAppService.UpdateCreditBatchEntry(creditBatchEntryDTO, serviceHeader);
        }

        public bool AuditCreditBatch(CreditBatchDTO creditBatchDTO, int batchAuthOption)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _creditBatchAppService.AuditCreditBatch(creditBatchDTO, batchAuthOption, serviceHeader);

        }

        public bool AuthorizeCreditBatch(CreditBatchDTO creditBatchDTO, int batchAuthOption, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _creditBatchAppService.AuthorizeCreditBatch(creditBatchDTO, batchAuthOption, moduleNavigationItemCode, serviceHeader);
        }

        public bool MatchCreditBatchDiscrepancyByGeneralLedgerAccount(CreditBatchDiscrepancyDTO creditBatchDiscrepancyDTO, Guid chartOfAccountId, int moduleNavigationItemCode, int discrepancyAuthOption)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _creditBatchAppService.MatchCreditBatchDiscrepancy(creditBatchDiscrepancyDTO, chartOfAccountId, moduleNavigationItemCode, discrepancyAuthOption, serviceHeader);
        }

        public bool MatchCreditBatchDiscrepancyByCustomerAccount(CreditBatchDiscrepancyDTO creditBatchDiscrepancyDTO, CustomerAccountDTO customerAccountDTO, int discrepancyAuthOption)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _creditBatchAppService.MatchCreditBatchDiscrepancy(creditBatchDiscrepancyDTO, customerAccountDTO, discrepancyAuthOption, serviceHeader);
        }

        public bool MatchCreditBatchDiscrepanciesByCustomerAccount(List<CreditBatchDiscrepancyDTO> creditBatchDiscrepancyDTOs, CustomerAccountDTO customerAccountDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _creditBatchAppService.MatchCreditBatchDiscrepancies(creditBatchDiscrepancyDTOs, customerAccountDTO, serviceHeader);
        }

        public bool PostCreditBatchEntry(Guid creditBatchEntryId, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _creditBatchAppService.PostCreditBatchEntry(creditBatchEntryId, moduleNavigationItemCode, serviceHeader);
        }

        public List<CreditBatchDTO> FindCreditBatches()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _creditBatchAppService.FindCreditBatches(serviceHeader);
        }

        public PageCollectionInfo<CreditBatchDTO> FindCreditBatchesByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _creditBatchAppService.FindCreditBatches(text, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<CreditBatchDTO> FindCreditBatchesByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _creditBatchAppService.FindCreditBatches(status, startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public CreditBatchDTO FindCreditBatch(Guid creditBatchId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _creditBatchAppService.FindCreditBatch(creditBatchId, serviceHeader);
        }

        public List<CreditBatchEntryDTO> FindCreditBatchEntriesByCreditBatchId(Guid creditBatchId, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var creditBatchEntries = _creditBatchAppService.FindCreditBatchEntriesByCreditBatchId(creditBatchId, serviceHeader);

            if (includeProductDescription && creditBatchEntries != null)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(creditBatchEntries, serviceHeader);

            return creditBatchEntries;
        }

        public PageCollectionInfo<CreditBatchEntryDTO> FindCreditBatchEntriesByCreditBatchIdInPage(Guid creditBatchId, string text, int creditBatchEntryFilter, int pageIndex, int pageSize, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var creditBatchEntries = _creditBatchAppService.FindCreditBatchEntriesByCreditBatchId(creditBatchId, text, creditBatchEntryFilter, pageIndex, pageSize, serviceHeader);

            if (includeProductDescription && creditBatchEntries != null)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(creditBatchEntries.PageCollection, serviceHeader);

            return creditBatchEntries;
        }

        public PageCollectionInfo<CreditBatchDiscrepancyDTO> FindCreditBatchDiscrepanciesByCreditBatchIdInPage(Guid creditBatchId, string text, int creditBatchDiscrepancyFilter, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _creditBatchAppService.FindCreditBatchDiscrepanciesByCreditBatchId(creditBatchId, text, creditBatchDiscrepancyFilter, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<CreditBatchDiscrepancyDTO> FindCreditBatchDiscrepanciesByCreditBatchTypeInPage(int creditBatchType, int status, int productCode, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _creditBatchAppService.FindCreditBatchDiscrepancies(creditBatchType, status, productCode, text, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<CreditBatchDiscrepancyDTO> FindCreditBatchDiscrepanciesInPage(int status, DateTime startDate, DateTime endDate, string text, int creditBatchDiscrepancyFilter, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _creditBatchAppService.FindCreditBatchDiscrepancies(status, startDate, endDate, text, creditBatchDiscrepancyFilter, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<CreditBatchEntryDTO> FindCreditBatchEntriesByCreditBatchTypeInPage(int creditBatchType, DateTime startDate, DateTime endDate, string text, int creditBatchEntryFilter, int pageIndex, int pageSize, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var creditBatchEntries = _creditBatchAppService.FindCreditBatchEntriesByCreditBatchType(creditBatchType, startDate, endDate, text, creditBatchEntryFilter, pageIndex, pageSize, serviceHeader);

            if (includeProductDescription && creditBatchEntries != null)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(creditBatchEntries.PageCollection, serviceHeader);

            return creditBatchEntries;
        }

        public List<CreditBatchEntryDTO> FindCreditBatchEntriesByCustomerId(int creditBatchType, Guid customerId, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var creditBatchEntries = _creditBatchAppService.FindCreditBatchEntriesByCustomerId(creditBatchType, customerId, serviceHeader);

            if (includeProductDescription && creditBatchEntries != null)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(creditBatchEntries, serviceHeader);

            return creditBatchEntries;
        }

        public List<CreditBatchEntryDTO> FindLoanAppraisalCreditBatchEntriesByCustomerId(Guid customerId, Guid loanProductId, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var creditBatchEntries = _creditBatchAppService.FindLoanAppraisalCreditBatchEntriesByCustomerId(customerId, loanProductId, serviceHeader);

            if (includeProductDescription && creditBatchEntries != null)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(creditBatchEntries, serviceHeader);

            return creditBatchEntries;
        }

        public List<BatchImportEntryWrapper> ParseCreditBatchImport(Guid creditBatchId, string fileName)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var serviceBrokerSettingsElement = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);

            //return _creditBatchAppService.ParseCreditBatchImport(creditBatchId, serviceBrokerSettingsElement.FileUploadDirectory, fileName, serviceHeader);

            return _creditBatchAppService.ParseCreditBatchImport(creditBatchId,
               @"C:\swiftfin_file_uploads\",
               fileName,
               serviceHeader
           );

        }

        public CreditBatchEntryDTO FindLastCreditBatchEntryByCustomerAccountId(Guid customerAccountId, int creditBatchType)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _sqlCommandAppService.FindLastCreditBatchEntryByCustomerAccountId(customerAccountId, creditBatchType, serviceHeader);
        }

        public PageCollectionInfo<CreditBatchEntryDTO> FindQueableCreditBatchEntriesInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _creditBatchAppService.FindQueableCreditBatchEntries(pageIndex, pageSize, serviceHeader);
        }

        #endregion
    }
}
