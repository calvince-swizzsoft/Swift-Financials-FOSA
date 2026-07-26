using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IExpensePayableService
    {
        #region Expense Payable

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        ExpensePayableDTO AddExpensePayable(ExpensePayableDTO expensePayableDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateExpensePayable(ExpensePayableDTO expensePayableDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        ExpensePayableEntryDTO AddExpensePayableEntry(ExpensePayableEntryDTO expensePayableEntryDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RemoveExpensePayableEntries(List<ExpensePayableEntryDTO> expensePayableEntryDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AuditExpensePayable(ExpensePayableDTO expensePayableDTO, int expensePayableAuthOption);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AuthorizeExpensePayable(ExpensePayableDTO expensePayableDTO, int expensePayableAuthOption, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateExpensePayableEntryCollection(Guid expensePayableId, List<ExpensePayableEntryDTO> expensePayableEntryCollection);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<ExpensePayableDTO> FindExpensePayables();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<ExpensePayableDTO> FindExpensePayablesInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<ExpensePayableDTO> FindExpensePayablesByDateRangeAndFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<ExpensePayableDTO> FindExpensePayablesByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<ExpensePayableDTO> FindExpensePayablesByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        ExpensePayableDTO FindExpensePayable(Guid expensePayableId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<ExpensePayableEntryDTO> FindExpensePayableEntriesByExpensePayableId(Guid expensePayableId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<ExpensePayableEntryDTO> FindExpensePayableEntriesByExpensePayableIdInPage(Guid expensePayableId, int pageIndex, int pageSize);






        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateExpensePayableEntriesByExpensePayableId(Guid expensePayableId, List<ExpensePayableEntryDTO> expensePayableEntries);
        #endregion
    }
}
