using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ChequeTypeAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.InHouseChequeAgg
{
    public class InHouseCheque : Entity
    {
        public Guid BranchId { get; set; }

        public virtual Branch Branch { get; private set; }

        public Guid? ChequeTypeId { get; set; }

        public virtual ChequeType ChequeType { get; private set; }

        public Guid DebitChartOfAccountId { get; set; }

        public virtual ChartOfAccount DebitChartOfAccount { get; private set; }

        public Guid? DebitCustomerAccountId { get; set; }

        public virtual CustomerAccount DebitCustomerAccount { get; private set; }

        public byte Funding { get; set; }

        public decimal Amount { get; set; }

        public string Payee { get; set; }

        public string Reference { get; set; }

        public bool Chargeable { get; set; }

        public bool IsPrinted { get; set; }

        public string PrintedNumber { get; set; }

        public string PrintedBy { get; set; }

        public DateTime? PrintedDate { get; set; }
    }
}
