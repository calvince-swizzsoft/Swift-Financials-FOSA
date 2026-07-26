using Domain.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.Reimbursement
{
    public class Reimbursement : Entity 
    {
        public string ReimbursementNo { get; set; }

        public Guid ImprestSurrenderId { get; set; }

        public string EmployeeNo { get; set; }

        public DateTime RequestDate { get; set; }

        public decimal Amount { get; set; }

        public String Status { get; set; }
    }
}
