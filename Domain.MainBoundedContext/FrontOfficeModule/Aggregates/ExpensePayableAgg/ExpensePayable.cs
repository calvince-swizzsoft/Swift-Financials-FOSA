using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.ExpensePayableEntryAgg;
using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.ExpensePayableAgg
{
    public class ExpensePayable : Entity
    {
        public Guid BranchId { get; set; }

        public virtual Branch Branch { get; private set; }

        public Guid ChartOfAccountId { get; set; }

        public virtual ChartOfAccount ChartOfAccount { get; private set; }

        [Index("IX_ExpensePayable_VoucherNumber")]
        public int VoucherNumber { get; set; }

        public int Type { get; set; }

        public decimal TotalValue { get; set; }

        public DateTime ValueDate { get; set; }

        public string Remarks { get; set; }

        public int Status { get; set; }

        public string AuditedBy { get; set; }

        public string AuditRemarks { get; set; }

        public DateTime? AuditedDate { get; set; }

        public string AuthorizedBy { get; set; }

        public string AuthorizationRemarks { get; set; }

        public DateTime? AuthorizedDate { get; set; }

        

        

        HashSet<ExpensePayableEntry> _expensePayableEntries;
        public virtual ICollection<ExpensePayableEntry> ExpensePayableEntries
        {
            get
            {
                if (_expensePayableEntries == null)
                {
                    _expensePayableEntries = new HashSet<ExpensePayableEntry>();
                }
                return _expensePayableEntries;
            }
            private set
            {
                _expensePayableEntries = new HashSet<ExpensePayableEntry>(value);
            }
        }
    }
}
