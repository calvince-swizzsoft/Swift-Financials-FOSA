using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.FrontOfficeModule
{
    [ServiceContract(Name = "IExpensePayableService")]
    public interface IExpensePayableService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddExpensePayable(ExpensePayableDTO expensePayableDTO, AsyncCallback callback, Object state);
        ExpensePayableDTO EndAddExpensePayable(IAsyncResult result);


        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateExpensePayable(ExpensePayableDTO expensePayableDTO, AsyncCallback callback, Object state);
        bool EndUpdateExpensePayable(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddExpensePayableEntry(ExpensePayableEntryDTO expensePayableEntryDTO, AsyncCallback callback, Object state);
        ExpensePayableEntryDTO EndAddExpensePayableEntry(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRemoveExpensePayableEntries(List<ExpensePayableEntryDTO> expensePayableEntryDTOs, AsyncCallback callback, Object state);
        bool EndRemoveExpensePayableEntries(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAuditExpensePayable(ExpensePayableDTO expensePayableDTO, int expensePayableAuthOption, AsyncCallback callback, Object state);
        bool EndAuditExpensePayable(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAuthorizeExpensePayable(ExpensePayableDTO expensePayableDTO, int expensePayableAuthOption, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndAuthorizeExpensePayable(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateExpensePayableEntryCollection(Guid expensePayableId, List<ExpensePayableEntryDTO> expensePayableEntryCollection, AsyncCallback callback, Object state);
        bool EndUpdateExpensePayableEntryCollection(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindExpensePayables(AsyncCallback callback, Object state);
        List<ExpensePayableDTO> EndFindExpensePayables(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindExpensePayablesInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<ExpensePayableDTO> EndFindExpensePayablesInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindExpensePayablesByDateRangeAndFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<ExpensePayableDTO> EndFindExpensePayablesByDateRangeAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindExpensePayablesByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<ExpensePayableDTO> EndFindExpensePayablesByStatusAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindExpensePayable(Guid expensePayableId, AsyncCallback callback, Object state);
        ExpensePayableDTO EndFindExpensePayable(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindExpensePayableEntriesByExpensePayableId(Guid expensePayableId, AsyncCallback callback, Object state);
        List<ExpensePayableEntryDTO> EndFindExpensePayableEntriesByExpensePayableId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindExpensePayableEntriesByExpensePayableIdInPage(Guid expensePayableId, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<ExpensePayableEntryDTO> EndFindExpensePayableEntriesByExpensePayableIdInPage(IAsyncResult result);


        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindExpensePayablesByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<ExpensePayableDTO> EndFindExpensePayablesByFilterInPage(IAsyncResult result);



        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateExpensePayableEntriesByExpensePayableId(Guid expensePayableId, List<ExpensePayableEntryDTO> expensePayableEntries, AsyncCallback callback, Object state);
        bool EndUpdateExpensePayableEntriesByExpensePayableId(IAsyncResult result);


        //IAsyncResult BeginUpdateExpensePayableEntriesByExpensePayableId(Guid expensePayableId, List<ExpensePayableEntryDTO> expensePayableEntries, AsyncCallback callback, Object state);
        //bool EndUpdateExpensePayableEntriesByExpensePayableId(IAsyncResult result);
    }
}
