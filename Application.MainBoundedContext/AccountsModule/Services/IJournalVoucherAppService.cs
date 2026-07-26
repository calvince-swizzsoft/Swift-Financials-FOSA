using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Crosscutting.Framework.Utils;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IJournalVoucherAppService
    {
        JournalVoucherDTO AddNewJournalVoucher(JournalVoucherDTO journalVoucherDTO, ServiceHeader serviceHeader);

        bool UpdateJournalVoucher(JournalVoucherDTO journalVoucherDTO, ServiceHeader serviceHeader);

        JournalVoucherEntryDTO AddNewJournalVoucherEntry(JournalVoucherEntryDTO journalVoucherEntryDTO, ServiceHeader serviceHeader);

        bool RemoveJournalVoucherEntries(List<JournalVoucherEntryDTO> journalVoucherEntryDTOs, ServiceHeader serviceHeader);

        bool AuditJournalVoucher(JournalVoucherDTO journalVoucherDTO, int journalVoucherAuthOption, ServiceHeader serviceHeader);

        bool AuthorizeJournalVoucher(JournalVoucherDTO journalVoucherDTO, int journalVoucherAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        bool UpdateJournalVoucherEntryCollection(Guid journalVoucherId, List<JournalVoucherEntryDTO> journalVoucherEntryCollection, ServiceHeader serviceHeader);

        bool UpdateJournalVoucherEntries(Guid journalVoucherId, List<JournalVoucherEntryDTO> journalVoucherEntries, ServiceHeader serviceHeader);
        

        List<JournalVoucherDTO> FindJournalVouchers(ServiceHeader serviceHeader);

        PageCollectionInfo<JournalVoucherDTO> FindJournalVouchers(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<JournalVoucherDTO> FindJournalVouchers(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<JournalVoucherDTO> FindJournalVouchers(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<JournalVoucherDTO> FindJournalVouchers(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        JournalVoucherDTO FindJournalVoucher(Guid journalVoucherId, ServiceHeader serviceHeader);

        List<JournalVoucherEntryDTO> FindJournalVoucherEntriesByJournalVoucherId(Guid journalVoucherId, ServiceHeader serviceHeader);

        PageCollectionInfo<JournalVoucherEntryDTO> FindJournalVoucherEntriesByJournalVoucherId(Guid journalVoucherId, int pageIndex, int pageSize, ServiceHeader serviceHeader);
    }
}
