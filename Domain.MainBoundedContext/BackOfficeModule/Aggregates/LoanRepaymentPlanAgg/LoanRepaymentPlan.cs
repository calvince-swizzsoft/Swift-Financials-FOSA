using System;
using System.Collections.Generic;
using Domain.Seedwork;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanCaseAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanRepaymentPlanAgg
{
    // Immutable schedule revisions. Confirming or correcting a plan appends a revision.
    public class LoanRepaymentPlan : Entity
    {
        public bool IsRestructuring { get; set; }
        public DateTime? EffectiveAt { get; set; }
        public decimal? OpeningInterest { get; set; }
        public int? PriorRiskCategory { get; set; }
        public string PriorPlanIds { get; set; }
        public string OpeningLedgerHash { get; set; }
        public Guid LoanCaseId { get; set; }
        public virtual LoanCase LoanCase { get; set; }
        public Guid CustomerAccountId { get; set; }
        public virtual CustomerAccount CustomerAccount { get; set; }
        public Guid PrincipalChartOfAccountId { get; set; }
        public virtual ChartOfAccount PrincipalChartOfAccount { get; set; }
        public Guid? InterestReceivableChartOfAccountId { get; set; }
        public virtual ChartOfAccount InterestReceivableChartOfAccount { get; set; }
        public Guid? InterestChargedChartOfAccountId { get; set; }
        public virtual ChartOfAccount InterestChargedChartOfAccount { get; set; }
        public bool InterestTermsConfirmed { get; set; }
        public Guid SourceJournalId { get; set; }
        public virtual Journal SourceJournal { get; set; }
        public int Revision { get; set; }
        public bool IsConfirmed { get; set; }
        public DateTime DisbursementDate { get; set; }
        public decimal Principal { get; set; }
        public string Evidence { get; set; }
        public string AllocationPolicy { get; set; }
        public virtual ICollection<LoanRepaymentInstalment> Instalments { get; set; } = new List<LoanRepaymentInstalment>();
    }
    public class LoanRepaymentInstalment : Entity
    {
        public Guid PlanId { get; set; }
        public virtual LoanRepaymentPlan Plan { get; set; }
        public int Number { get; set; }
        public decimal? Interest { get; set; }
        public DateTime? InterestDueDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Principal { get; set; }
    }
}
