using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IGeneralLedgerAppService
    {
        GeneralLedgerDTO AddNewGeneralLedger(GeneralLedgerDTO generalLedgerDTO, ServiceHeader serviceHeader);

        bool UpdateGeneralLedger(GeneralLedgerDTO generalLedgerDTO, ServiceHeader serviceHeader);

        GeneralLedgerEntryDTO AddNewGeneralLedgerEntry(GeneralLedgerEntryDTO generalLedgerEntryDTO, ServiceHeader serviceHeader);

        bool RemoveGeneralLedgerEntries(List<GeneralLedgerEntryDTO> generalLedgerEntryDTOs, ServiceHeader serviceHeader);

        bool AuditGeneralLedger(GeneralLedgerDTO generalLedgerDTO, int generalLedgerAuthOption, ServiceHeader serviceHeader);

        bool AuthorizeGeneralLedger(GeneralLedgerDTO generalLedgerDTO, int generalLedgerAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        bool UpdateGeneralLedgerEntryCollection(Guid generalLedgerId, List<GeneralLedgerEntryDTO> generalLedgerEntryCollection, ServiceHeader serviceHeader);

        List<GeneralLedgerDTO> FindGeneralLedgers(ServiceHeader serviceHeader);

        PageCollectionInfo<GeneralLedgerDTO> FindGeneralLedgers(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<GeneralLedgerDTO> FindGeneralLedgers(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<GeneralLedgerDTO> FindGeneralLedgers(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        GeneralLedgerDTO FindGeneralLedger(Guid generalLedgerId, ServiceHeader serviceHeader);

        List<GeneralLedgerEntryDTO> FindGeneralLedgerEntriesByGeneralLedgerId(Guid generalLedgerId, ServiceHeader serviceHeader);

        PageCollectionInfo<GeneralLedgerEntryDTO> FindGeneralLedgerEntriesByGeneralLedgerId(Guid generalLedgerId, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        BatchImportParseInfo ParseGeneralLedgerImportEntries(GeneralLedgerEntryDTO generalLedgerEntryDTO, string fileUploadDirectory, string fileName, ServiceHeader serviceHeader);
    }
}
