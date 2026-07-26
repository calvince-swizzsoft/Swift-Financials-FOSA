using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Crosscutting.Framework.Utils;

namespace Application.MainBoundedContext.FrontOfficeModule.Services
{
    public interface IElectronicJournalAppService
    {
        ElectronicJournalDTO ParseElectronicJournalImport(string fileDirectory, string fileName, string blobDatabaseConnectionString, ServiceHeader serviceHeader);

        bool CloseElectronicJournal(ElectronicJournalDTO electronicJournalDTO, string encryptionPublicKeyPath, string encryptionPrivateKeyPath, string encryptionPassPhrase, string fileExportDirectory, ServiceHeader serviceHeader);

        bool ClearTruncatedCheque(TruncatedChequeDTO truncatedChequeDTO, ServiceHeader serviceHeader);

        bool MatchTruncatedChequePaymentVoucher(TruncatedChequeDTO truncatedChequeDTO, ServiceHeader serviceHeader);

        List<ElectronicJournalDTO> FindElectronicJournals(ServiceHeader serviceHeader);

        PageCollectionInfo<ElectronicJournalDTO> FindElectronicJournals(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        ElectronicJournalDTO FindElectronicJournal(Guid electronicJournalId, ServiceHeader serviceHeader);

        PageCollectionInfo<TruncatedChequeDTO> FindTruncatedCheques(Guid electronicJournalId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<TruncatedChequeDTO> FindTruncatedCheques(Guid electronicJournalId, int status, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        TruncatedChequeDTO FindTruncatedCheque(Guid truncatedChequeId, ServiceHeader serviceHeader);
    }
}
