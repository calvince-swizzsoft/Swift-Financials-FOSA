using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using Domain.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.CashDepositRequestAgg
{
    public class CashDepositRequest : Entity
    {
        public Guid BranchId { get; set; }

        public virtual Branch Branch { get; private set; }

        public Guid CustomerAccountId { get; set; }

        public virtual CustomerAccount CustomerAccount { get; private set; }
        
        public byte Status { get; set; }

        public decimal Amount { get; set; }

        public string Remarks { get; set; }

        public string AuthorizedBy { get; set; }

        public string AuthorizationRemarks { get; set; }

        public DateTime? AuthorizedDate { get; set; }

        public string PostedBy { get; set; }

        public DateTime? PostedDate { get; set; }

        public int TransactionType { get; set; }
    }
}
