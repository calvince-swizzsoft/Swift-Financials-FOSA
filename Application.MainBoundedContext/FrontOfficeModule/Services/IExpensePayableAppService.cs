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
    public interface IExpensePayableAppService
    {
        ExpensePayableDTO AddNewExpensePayable(ExpensePayableDTO expensePayableDTO, ServiceHeader serviceHeader);

        bool UpdateExpensePayable(ExpensePayableDTO expensePayableDTO, ServiceHeader serviceHeader);

        ExpensePayableEntryDTO AddNewExpensePayableEntry(ExpensePayableEntryDTO expensePayableEntryDTO, ServiceHeader serviceHeader);

        bool RemoveExpensePayableEntries(List<ExpensePayableEntryDTO> expensePayableEntryDTOs, ServiceHeader serviceHeader);

        bool AuditExpensePayable(ExpensePayableDTO expensePayableDTO, int expensePayableAuthOption, ServiceHeader serviceHeader);

        bool AuthorizeExpensePayable(ExpensePayableDTO expensePayableDTO, int expensePayableAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        bool UpdateExpensePayableEntryCollection(Guid expensePayableId, List<ExpensePayableEntryDTO> expensePayableEntryCollection, ServiceHeader serviceHeader);

        List<ExpensePayableDTO> FindExpensePayables(ServiceHeader serviceHeader);

        PageCollectionInfo<ExpensePayableDTO> FindExpensePayables(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<ExpensePayableDTO> FindExpensePayables(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<ExpensePayableDTO> FindExpensePayables(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<ExpensePayableDTO> FindExpensePayables(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        ExpensePayableDTO FindExpensePayable(Guid expensePayableId, ServiceHeader serviceHeader);

        List<ExpensePayableEntryDTO> FindExpensePayableEntriesByExpensePayableId(Guid expensePayableId, ServiceHeader serviceHeader);

        PageCollectionInfo<ExpensePayableEntryDTO> FindExpensePayableEntriesByExpensePayableId(Guid expensePayableId, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        bool UpdateExpensePayableEntries(Guid expensePayableId, List<ExpensePayableEntryDTO> expensePayableEntries, ServiceHeader serviceHeader);
    }
}
