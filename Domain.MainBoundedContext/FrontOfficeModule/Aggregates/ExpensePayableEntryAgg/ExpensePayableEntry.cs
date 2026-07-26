using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.ExpensePayableAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.ExpensePayableEntryAgg
{
    public class ExpensePayableEntry : Entity
    {
        public Guid ExpensePayableId { get; set; }

        public virtual ExpensePayable ExpensePayable { get; private set; }

        public Guid BranchId { get; set; }

        public virtual Branch Branch { get; private set; }

        public Guid ChartOfAccountId { get; set; }

        public virtual ChartOfAccount ChartOfAccount { get; private set; }

        public decimal Value { get; set; }

        public string PrimaryDescription { get; set; }

        public string SecondaryDescription { get; set; }

        public string Reference { get; set; }

        public int Status { get; set; }

        

        
    }
}
