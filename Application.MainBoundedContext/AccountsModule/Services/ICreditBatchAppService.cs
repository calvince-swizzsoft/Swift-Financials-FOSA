using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface ICreditBatchAppService
    {
        CreditBatchDTO AddNewCreditBatch(CreditBatchDTO creditBatchDTO, ServiceHeader serviceHeader);

        bool UpdateCreditBatch(CreditBatchDTO creditBatchDTO, ServiceHeader serviceHeader);

        bool AuditCreditBatch(CreditBatchDTO creditBatchDTO, int batchAuthOption, ServiceHeader serviceHeader);

        bool AuthorizeCreditBatch(CreditBatchDTO creditBatchDTO, int batchAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        bool MatchCreditBatchDiscrepancy(CreditBatchDiscrepancyDTO creditBatchDiscrepancyDTO, Guid chartOfAccountId, int moduleNavigationItemCode, int discrepancyAuthOption, ServiceHeader serviceHeader);

        bool MatchCreditBatchDiscrepancy(CreditBatchDiscrepancyDTO creditBatchDiscrepancyDTO, CustomerAccountDTO customerAccountDTO, int discrepancyAuthOption, ServiceHeader serviceHeader);

        bool MatchCreditBatchDiscrepancies(List<CreditBatchDiscrepancyDTO> creditBatchDiscrepancyDTOs, CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader);

        CreditBatchEntryDTO AddNewCreditBatchEntry(CreditBatchEntryDTO creditBatchEntryDTO, ServiceHeader serviceHeader);

        bool RemoveCreditBatchEntries(List<CreditBatchEntryDTO> creditBatchEntryDTOs, ServiceHeader serviceHeader);

        bool UpdateCreditBatchEntry(CreditBatchEntryDTO creditBatchEntryDTO, ServiceHeader serviceHeader);

        bool PostCreditBatchEntry(Guid creditBatchEntryId, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        List<CreditBatchDTO> FindCreditBatches(ServiceHeader serviceHeader);

        PageCollectionInfo<CreditBatchDTO> FindCreditBatches(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<CreditBatchDTO> FindCreditBatches(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<CreditBatchDTO> FindCreditBatches(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);
        CreditBatchDTO FindCreditBatch(Guid creditBatchId, ServiceHeader serviceHeader);

        CreditBatchDTO FindCachedCreditBatch(Guid creditBatchId, ServiceHeader serviceHeader);

        CreditBatchEntryDTO FindCreditBatchEntry(Guid creditBatchEntryId, ServiceHeader serviceHeader);

        List<CreditBatchEntryDTO> FindCreditBatchEntriesByCreditBatchId(Guid creditBatchId, ServiceHeader serviceHeader);

        PageCollectionInfo<CreditBatchEntryDTO> FindCreditBatchEntriesByCreditBatchId(Guid creditBatchId, string text, int creditBatchEntryFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<CreditBatchEntryDTO> FindCreditBatchEntriesByCreditBatchType(int creditBatchType, DateTime startDate, DateTime endDate, string text, int creditBatchEntryFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<CreditBatchDiscrepancyDTO> FindCreditBatchDiscrepanciesByCreditBatchId(Guid creditBatchId, string text, int creditBatchDiscrepancyFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<CreditBatchDiscrepancyDTO> FindCreditBatchDiscrepancies(int status, DateTime startDate, DateTime endDate, string text, int creditBatchDiscrepancyFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<CreditBatchDiscrepancyDTO> FindCreditBatchDiscrepancies(int creditBatchType, int status, int productCode, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<CreditBatchEntryDTO> FindQueableCreditBatchEntries(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        List<CreditBatchEntryDTO> FindCreditBatchEntriesByCustomerId(int creditBatchType, Guid customerId, ServiceHeader serviceHeader);

        List<CreditBatchEntryDTO> FindLoanAppraisalCreditBatchEntriesByCustomerId(Guid customerId, Guid loanProductId, ServiceHeader serviceHeader);

        List<BatchImportEntryWrapper> ParseCreditBatchImport(Guid creditBatchId, string fileUploadDirectory, string fileName, ServiceHeader serviceHeader);
    }
}
