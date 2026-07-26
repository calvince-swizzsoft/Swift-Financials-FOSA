using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class ChartOfAccountBulkCopyDTO
    {
        public Guid Id { get; set; }

        public Guid? ParentId { get; set; }

        public Guid? CostCenterId { get; set; }

        public int AccountType { get; set; }

        public int AccountCategory { get; set; }

        public int AccountCode { get; set; }

        public string AccountName { get; set; }

        public int Depth { get; set; }

        public bool IsControlAccount { get; set; }

        public bool IsReconciliationAccount { get; set; }

        public bool PostAutomaticallyOnly { get; set; }

        public bool IsLocked { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
