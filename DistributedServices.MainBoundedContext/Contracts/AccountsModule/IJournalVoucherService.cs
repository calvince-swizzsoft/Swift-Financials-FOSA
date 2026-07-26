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
    public interface IJournalVoucherService
    {
        #region Journal Voucher

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        JournalVoucherDTO AddJournalVoucher(JournalVoucherDTO journalVoucherDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateJournalVoucher(JournalVoucherDTO journalVoucherDTO);


        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<JournalVoucherDTO> FindJournalVouchersByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        JournalVoucherEntryDTO AddJournalVoucherEntry(JournalVoucherEntryDTO journalVoucherEntryDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RemoveJournalVoucherEntries(List<JournalVoucherEntryDTO> journalVoucherEntryDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AuditJournalVoucher(JournalVoucherDTO journalVoucherDTO, int journalVoucherAuthOption);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AuthorizeJournalVoucher(JournalVoucherDTO journalVoucherDTO, int journalVoucherAuthOption, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateJournalVoucherEntryCollection(Guid journalVoucherId, List<JournalVoucherEntryDTO> journalVoucherEntryCollection);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<JournalVoucherDTO> FindJournalVouchers();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<JournalVoucherDTO> FindJournalVouchersInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<JournalVoucherDTO> FindJournalVouchersByDateRangeAndFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<JournalVoucherDTO> FindJournalVouchersByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        JournalVoucherDTO FindJournalVoucher(Guid journalVoucherId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<JournalVoucherEntryDTO> FindJournalVoucherEntriesByJournalVoucherId(Guid journalVoucherId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<JournalVoucherEntryDTO> FindJournalVoucherEntriesByJournalVoucherIdInPage(Guid journalVoucherId, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateJournalVoucherEntriesByJournalVoucherId(Guid journalVoucherId, List<JournalVoucherEntryDTO> journalVoucherEntries);

        #endregion
    }
}
