using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IGeneralLedgerService
    {
        #region General Ledger

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        GeneralLedgerDTO AddGeneralLedger(GeneralLedgerDTO generalLedgerDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateGeneralLedger(GeneralLedgerDTO generalLedgerDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        GeneralLedgerEntryDTO AddGeneralLedgerEntry(GeneralLedgerEntryDTO generalLedgerEntryDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RemoveGeneralLedgerEntries(List<GeneralLedgerEntryDTO> generalLedgerEntryDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AuditGeneralLedger(GeneralLedgerDTO generalLedgerDTO, int generalLedgerAuthOption);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AuthorizeGeneralLedger(GeneralLedgerDTO generalLedgerDTO, int generalLedgerAuthOption, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateGeneralLedgerEntryCollection(Guid generalLedgerId, List<GeneralLedgerEntryDTO> generalLedgerEntryCollection);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<GeneralLedgerDTO> FindGeneralLedgers();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<GeneralLedgerDTO> FindGeneralLedgersInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<GeneralLedgerDTO> FindGeneralLedgersByDateRangeAndFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<GeneralLedgerDTO> FindGeneralLedgersByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        GeneralLedgerDTO FindGeneralLedger(Guid generalLedgerId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<GeneralLedgerEntryDTO> FindGeneralLedgerEntriesByGeneralLedgerId(Guid generalLedgerId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<GeneralLedgerEntryDTO> FindGeneralLedgerEntriesByGeneralLedgerIdInPage(Guid generalLedgerId, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        BatchImportParseInfo ParseGeneralLedgerImportEntries(GeneralLedgerEntryDTO generalLedgerEntryDTO, string fileName);

        #endregion
    }
}
