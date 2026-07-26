using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class JournalVoucherService : IJournalVoucherService
    {
        private readonly IJournalVoucherAppService _journalVoucherAppService;

        public JournalVoucherService(
           IJournalVoucherAppService journalVoucherAppService)
        {
            Guard.ArgumentNotNull(journalVoucherAppService, nameof(journalVoucherAppService));

            _journalVoucherAppService = journalVoucherAppService;
        }

        #region Journal Voucher

        public JournalVoucherDTO AddJournalVoucher(JournalVoucherDTO journalVoucherDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalVoucherAppService.AddNewJournalVoucher(journalVoucherDTO, serviceHeader);
        }

        public bool UpdateJournalVoucher(JournalVoucherDTO journalVoucherDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalVoucherAppService.UpdateJournalVoucher(journalVoucherDTO, serviceHeader);
        }
       
        public PageCollectionInfo<JournalVoucherDTO> FindJournalVouchersByFilterInPage(string text, int pageIndex, int pageSize)
        { 
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalVoucherAppService.FindJournalVouchers(text, pageIndex, pageSize, serviceHeader);
        }

        public JournalVoucherEntryDTO AddJournalVoucherEntry(JournalVoucherEntryDTO journalVoucherEntryDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalVoucherAppService.AddNewJournalVoucherEntry(journalVoucherEntryDTO, serviceHeader);
        }

        public bool RemoveJournalVoucherEntries(List<JournalVoucherEntryDTO> journalVoucherEntryDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalVoucherAppService.RemoveJournalVoucherEntries(journalVoucherEntryDTOs, serviceHeader);
        }

        public bool AuditJournalVoucher(JournalVoucherDTO journalVoucherDTO, int journalVoucherAuthOption)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalVoucherAppService.AuditJournalVoucher(journalVoucherDTO, journalVoucherAuthOption, serviceHeader);
        }

        public bool AuthorizeJournalVoucher(JournalVoucherDTO journalVoucherDTO, int journalVoucherAuthOption, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalVoucherAppService.AuthorizeJournalVoucher(journalVoucherDTO, journalVoucherAuthOption, moduleNavigationItemCode, serviceHeader);
        }

        public bool UpdateJournalVoucherEntryCollection(Guid journalVoucherId, List<JournalVoucherEntryDTO> journalVoucherEntryCollection)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalVoucherAppService.UpdateJournalVoucherEntryCollection(journalVoucherId, journalVoucherEntryCollection, serviceHeader);
        }

        public bool UpdateJournalVoucherEntriesByJournalVoucherId(Guid journalVoucherId, List<JournalVoucherEntryDTO> journalVoucherEntries)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalVoucherAppService.UpdateJournalVoucherEntries(journalVoucherId, journalVoucherEntries, serviceHeader);
        }

        public List<JournalVoucherDTO> FindJournalVouchers()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalVoucherAppService.FindJournalVouchers(serviceHeader);
        }

        public PageCollectionInfo<JournalVoucherDTO> FindJournalVouchersInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalVoucherAppService.FindJournalVouchers(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<JournalVoucherDTO> FindJournalVouchersByDateRangeAndFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalVoucherAppService.FindJournalVouchers(startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<JournalVoucherDTO> FindJournalVouchersByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalVoucherAppService.FindJournalVouchers(status, startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public JournalVoucherDTO FindJournalVoucher(Guid journalVoucherId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalVoucherAppService.FindJournalVoucher(journalVoucherId, serviceHeader);
        }

        public List<JournalVoucherEntryDTO> FindJournalVoucherEntriesByJournalVoucherId(Guid journalVoucherId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalVoucherAppService.FindJournalVoucherEntriesByJournalVoucherId(journalVoucherId, serviceHeader);
        }

        public PageCollectionInfo<JournalVoucherEntryDTO> FindJournalVoucherEntriesByJournalVoucherIdInPage(Guid journalVoucherId, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalVoucherAppService.FindJournalVoucherEntriesByJournalVoucherId(journalVoucherId, pageIndex, pageSize, serviceHeader);
        }

        #endregion
    }
}
