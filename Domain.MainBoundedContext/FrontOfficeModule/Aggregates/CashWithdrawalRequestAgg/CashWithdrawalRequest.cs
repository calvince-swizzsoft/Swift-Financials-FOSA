using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.CashWithdrawalRequestAgg
{
    public class CashWithdrawalRequest : Entity
    {
        public Guid BranchId { get; set; }

        public virtual Branch Branch { get; private set; }

        public Guid? CustomerAccountId { get; set; }

        public virtual CustomerAccount CustomerAccount { get; private set; }

        public Guid? ChartOfAccountId { get; set; }

        public virtual ChartOfAccount ChartOfAccount { get; private set; }

        public byte Type { get; set; }

        public byte Category { get; set; }

        public byte Status { get; set; }

        public decimal Amount { get; set; }

        public string Remarks { get; set; }

        public DateTime MaturityDate { get; set; }

        public string AuthorizedBy { get; set; }

        public string AuthorizationRemarks { get; set; }

        public DateTime? AuthorizedDate { get; set; }

        public string PaidBy { get; set; }

        public DateTime? PaidDate { get; set; }

        public Guid? PaymentVoucherId { get; set; }

        public string PaymentVoucherPayee { get; set; }

        public int TransactionType { get; set; }
    }
}
