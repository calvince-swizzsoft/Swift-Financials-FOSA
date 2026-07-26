using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ChequeTypeAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.TellerAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.ExternalChequeAgg
{
    public class ExternalCheque : Entity
    {
        public Guid TellerId { get; set; }

        public virtual Teller Teller { get; private set; }

        public Guid? ChequeTypeId { get; set; }

        public virtual ChequeType ChequeType { get; private set; }

        public Guid? CustomerAccountId { get; set; }

        public virtual CustomerAccount CustomerAccount { get; private set; }

        public Guid? BankLinkageChartOfAccountId { get; set; }

        public virtual ChartOfAccount BankLinkageChartOfAccount { get; set; }

        public string Number { get; set; }

        public decimal Amount { get; set; }

        public string Drawer { get; set; }

        public string DrawerBank { get; set; }

        public string DrawerBankBranch { get; set; }

        public DateTime WriteDate { get; set; }

        public DateTime MaturityDate { get; set; }

        public string Remarks { get; set; }

        public bool IsCleared { get; set; }

        public string ClearedBy { get; set; }

        public DateTime? ClearedDate { get; set; }

        public bool IsBanked { get; set; }

        public string BankedBy { get; set; }

        public DateTime? BankedDate { get; set; }

        public bool IsTransferred { get; set; }

        public string TransferredBy { get; set; }

        public DateTime? TransferredDate { get; set; }
    }
}
