using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IJournalVoucherService")]
    public interface IJournalVoucherService
    {

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindJournalVouchersByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<JournalVoucherDTO> EndFindJournalVouchersByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddJournalVoucher(JournalVoucherDTO journalVoucherDTO, AsyncCallback callback, Object state);
        JournalVoucherDTO EndAddJournalVoucher(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateJournalVoucher(JournalVoucherDTO journalVoucherDTO, AsyncCallback callback, Object state);
        bool EndUpdateJournalVoucher(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddJournalVoucherEntry(JournalVoucherEntryDTO journalVoucherEntryDTO, AsyncCallback callback, Object state);
        JournalVoucherEntryDTO EndAddJournalVoucherEntry(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRemoveJournalVoucherEntries(List<JournalVoucherEntryDTO> journalVoucherEntryDTOs, AsyncCallback callback, Object state);
        bool EndRemoveJournalVoucherEntries(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAuditJournalVoucher(JournalVoucherDTO journalVoucherDTO, int journalVoucherAuthOption, AsyncCallback callback, Object state);
        bool EndAuditJournalVoucher(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAuthorizeJournalVoucher(JournalVoucherDTO journalVoucherDTO, int journalVoucherAuthOption, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndAuthorizeJournalVoucher(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateJournalVoucherEntryCollection(Guid journalVoucherId, List<JournalVoucherEntryDTO> journalVoucherEntryCollection, AsyncCallback callback, Object state);
        bool EndUpdateJournalVoucherEntryCollection(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindJournalVouchers(AsyncCallback callback, Object state);
        List<JournalVoucherDTO> EndFindJournalVouchers(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindJournalVouchersInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<JournalVoucherDTO> EndFindJournalVouchersInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindJournalVouchersByDateRangeAndFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<JournalVoucherDTO> EndFindJournalVouchersByDateRangeAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindJournalVouchersByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<JournalVoucherDTO> EndFindJournalVouchersByStatusAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindJournalVoucher(Guid journalVoucherId, AsyncCallback callback, Object state);
        JournalVoucherDTO EndFindJournalVoucher(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindJournalVoucherEntriesByJournalVoucherId(Guid journalVoucherId, AsyncCallback callback, Object state);
        List<JournalVoucherEntryDTO> EndFindJournalVoucherEntriesByJournalVoucherId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindJournalVoucherEntriesByJournalVoucherIdInPage(Guid journalVoucherId, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<JournalVoucherEntryDTO> EndFindJournalVoucherEntriesByJournalVoucherIdInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateJournalVoucherEntriesByJournalVoucherId(Guid journalVoucherId, List<JournalVoucherEntryDTO> journalVoucherEntries, AsyncCallback callback, Object state);
        bool EndUpdateJournalVoucherEntriesByJournalVoucherId(IAsyncResult result);
    }
}
