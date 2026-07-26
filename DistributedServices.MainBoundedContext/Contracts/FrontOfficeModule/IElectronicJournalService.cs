using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IElectronicJournalService
    {
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        ElectronicJournalDTO ParseElectronicJournalImport(string fileName);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool CloseElectronicJournal(ElectronicJournalDTO electronicJournalDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ClearTruncatedCheque(TruncatedChequeDTO truncatedChequeDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool MatchTruncatedChequePaymentVoucher(TruncatedChequeDTO truncatedChequeDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<ElectronicJournalDTO> FindElectronicJournals();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<ElectronicJournalDTO> FindElectronicJournalsByFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        ElectronicJournalDTO FindElectronicJournal(Guid electronicJournalId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<TruncatedChequeDTO> FindTruncatedChequesByElectronicJournalIdAndFilterInPage(Guid electronicJournalId, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<TruncatedChequeDTO> FindTruncatedChequesByElectronicJournalIdAndStatusAndFilterInPage(Guid electronicJournalId, int status, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        TruncatedChequeDTO FindTruncatedCheque(Guid truncatedChequeId);
    }
}
