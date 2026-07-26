using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.FrontOfficeModule.Services;
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
    public class ExpensePayableService : IExpensePayableService
    {
        private readonly IExpensePayableAppService _expensePayableAppService;

        public ExpensePayableService(
           IExpensePayableAppService expensePayableAppService)
        {
            Guard.ArgumentNotNull(expensePayableAppService, nameof(expensePayableAppService));

            _expensePayableAppService = expensePayableAppService;
        }

        #region Expense Payable

        public ExpensePayableDTO AddExpensePayable(ExpensePayableDTO expensePayableDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _expensePayableAppService.AddNewExpensePayable(expensePayableDTO, serviceHeader);
        }

        public bool UpdateExpensePayable(ExpensePayableDTO expensePayableDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _expensePayableAppService.UpdateExpensePayable(expensePayableDTO, serviceHeader);
        }

        public ExpensePayableEntryDTO AddExpensePayableEntry(ExpensePayableEntryDTO expensePayableEntryDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _expensePayableAppService.AddNewExpensePayableEntry(expensePayableEntryDTO, serviceHeader);
        }

        public bool RemoveExpensePayableEntries(List<ExpensePayableEntryDTO> expensePayableEntryDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _expensePayableAppService.RemoveExpensePayableEntries(expensePayableEntryDTOs, serviceHeader);
        }

        public bool AuditExpensePayable(ExpensePayableDTO expensePayableDTO, int expensePayableAuthOption)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _expensePayableAppService.AuditExpensePayable(expensePayableDTO, expensePayableAuthOption, serviceHeader);
        }

        public bool AuthorizeExpensePayable(ExpensePayableDTO expensePayableDTO, int expensePayableAuthOption, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _expensePayableAppService.AuthorizeExpensePayable(expensePayableDTO, expensePayableAuthOption, moduleNavigationItemCode, serviceHeader);
        }

        public bool UpdateExpensePayableEntryCollection(Guid expensePayableId, List<ExpensePayableEntryDTO> expensePayableEntryCollection)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _expensePayableAppService.UpdateExpensePayableEntryCollection(expensePayableId, expensePayableEntryCollection, serviceHeader);
        }

        public List<ExpensePayableDTO> FindExpensePayables()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _expensePayableAppService.FindExpensePayables(serviceHeader);
        }

        public PageCollectionInfo<ExpensePayableDTO> FindExpensePayablesInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _expensePayableAppService.FindExpensePayables(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<ExpensePayableDTO> FindExpensePayablesByDateRangeAndFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _expensePayableAppService.FindExpensePayables(startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<ExpensePayableDTO> FindExpensePayablesByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _expensePayableAppService.FindExpensePayables(status, startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<ExpensePayableDTO> FindExpensePayablesByFilterInPage( string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _expensePayableAppService.FindExpensePayables( text, pageIndex, pageSize, serviceHeader);
        }


        public ExpensePayableDTO FindExpensePayable(Guid expensePayableId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _expensePayableAppService.FindExpensePayable(expensePayableId, serviceHeader);
        }

        public List<ExpensePayableEntryDTO> FindExpensePayableEntriesByExpensePayableId(Guid expensePayableId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _expensePayableAppService.FindExpensePayableEntriesByExpensePayableId(expensePayableId, serviceHeader);
        }

        public PageCollectionInfo<ExpensePayableEntryDTO> FindExpensePayableEntriesByExpensePayableIdInPage(Guid expensePayableId, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _expensePayableAppService.FindExpensePayableEntriesByExpensePayableId(expensePayableId, pageIndex, pageSize, serviceHeader);
        }




        public bool UpdateExpensePayableEntriesByExpensePayableId(Guid expensePayableId, List<ExpensePayableEntryDTO> expensePayableEntries)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _expensePayableAppService.UpdateExpensePayableEntries(expensePayableId, expensePayableEntries, serviceHeader);
        }
        #endregion

    }
}
