using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IGeneralLedgerService")]
    public interface IGeneralLedgerService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddGeneralLedger(GeneralLedgerDTO generalLedgerDTO, AsyncCallback callback, Object state);
        GeneralLedgerDTO EndAddGeneralLedger(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateGeneralLedger(GeneralLedgerDTO generalLedgerDTO, AsyncCallback callback, Object state);
        bool EndUpdateGeneralLedger(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddGeneralLedgerEntry(GeneralLedgerEntryDTO generalLedgerEntryDTO, AsyncCallback callback, Object state);
        GeneralLedgerEntryDTO EndAddGeneralLedgerEntry(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRemoveGeneralLedgerEntries(List<GeneralLedgerEntryDTO> generalLedgerEntryDTOs, AsyncCallback callback, Object state);
        bool EndRemoveGeneralLedgerEntries(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAuditGeneralLedger(GeneralLedgerDTO generalLedgerDTO, int generalLedgerAuthOption, AsyncCallback callback, Object state);
        bool EndAuditGeneralLedger(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAuthorizeGeneralLedger(GeneralLedgerDTO generalLedgerDTO, int generalLedgerAuthOption, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndAuthorizeGeneralLedger(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateGeneralLedgerEntryCollection(Guid generalLedgerId, List<GeneralLedgerEntryDTO> generalLedgerEntryCollection, AsyncCallback callback, Object state);
        bool EndUpdateGeneralLedgerEntryCollection(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindGeneralLedgers(AsyncCallback callback, Object state);
        List<GeneralLedgerDTO> EndFindGeneralLedgers(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindGeneralLedgersInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<GeneralLedgerDTO> EndFindGeneralLedgersInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindGeneralLedgersByDateRangeAndFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<GeneralLedgerDTO> EndFindGeneralLedgersByDateRangeAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindGeneralLedgersByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<GeneralLedgerDTO> EndFindGeneralLedgersByStatusAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindGeneralLedger(Guid generalLedgerId, AsyncCallback callback, Object state);
        GeneralLedgerDTO EndFindGeneralLedger(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindGeneralLedgerEntriesByGeneralLedgerId(Guid generalLedgerId, AsyncCallback callback, Object state);
        List<GeneralLedgerEntryDTO> EndFindGeneralLedgerEntriesByGeneralLedgerId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindGeneralLedgerEntriesByGeneralLedgerIdInPage(Guid generalLedgerId, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<GeneralLedgerEntryDTO> EndFindGeneralLedgerEntriesByGeneralLedgerIdInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginParseGeneralLedgerImportEntries(GeneralLedgerEntryDTO generalLedgerEntryDTO, string fileName, AsyncCallback callback, Object state);
        BatchImportParseInfo EndParseGeneralLedgerImportEntries(IAsyncResult result);
    }
}
