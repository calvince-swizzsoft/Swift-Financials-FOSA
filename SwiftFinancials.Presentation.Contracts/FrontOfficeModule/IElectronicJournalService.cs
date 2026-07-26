using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.FrontOfficeModule
{
    [ServiceContract(Name = "IElectronicJournalService")]
    public interface IElectronicJournalService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginParseElectronicJournalImport(string fileName, AsyncCallback callback, Object state);
        ElectronicJournalDTO EndParseElectronicJournalImport(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginCloseElectronicJournal(ElectronicJournalDTO electronicJournalDTO, AsyncCallback callback, Object state);
        bool EndCloseElectronicJournal(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginClearTruncatedCheque(TruncatedChequeDTO truncatedChequeDTO, AsyncCallback callback, Object state);
        bool EndClearTruncatedCheque(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginMatchTruncatedChequePaymentVoucher(TruncatedChequeDTO truncatedChequeDTO, AsyncCallback callback, Object state);
        bool EndMatchTruncatedChequePaymentVoucher(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindElectronicJournals(AsyncCallback callback, Object state);
        List<ElectronicJournalDTO> EndFindElectronicJournals(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindElectronicJournalsByFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<ElectronicJournalDTO> EndFindElectronicJournalsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindElectronicJournal(Guid electronicJournalId, AsyncCallback callback, Object state);
        ElectronicJournalDTO EndFindElectronicJournal(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindTruncatedChequesByElectronicJournalIdAndFilterInPage(Guid electronicJournalId, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<TruncatedChequeDTO> EndFindTruncatedChequesByElectronicJournalIdAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindTruncatedChequesByElectronicJournalIdAndStatusAndFilterInPage(Guid electronicJournalId, int status, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<TruncatedChequeDTO> EndFindTruncatedChequesByElectronicJournalIdAndStatusAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindTruncatedCheque(Guid truncatedChequeId, AsyncCallback callback, Object state);
        TruncatedChequeDTO EndFindTruncatedCheque(IAsyncResult result);
    }
}
