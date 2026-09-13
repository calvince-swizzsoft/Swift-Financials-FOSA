using System;
using System.Collections.Generic;
using System.Linq;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Infrastructure.Crosscutting.Framework.Utils;

namespace Application.MainBoundedContext.BackOfficeModule.Services
{
    // Contractual interest is separate from principal and from the accrual ledger.
    public static class LoanInterestAgeingEngine
    {
        public static void ValidateTerms(LoanPlanDTO plan)
        {
            foreach (var row in plan.Instalments ?? new List<LoanPlanInstalmentDTO>())
            {
                if (row == null) continue; // Principal validator supplies the row error.
                if (row.Interest.HasValue && (row.Interest < 0 || row.Interest > 1000000000000000m || decimal.Round(row.Interest.Value, 2) != row.Interest))
                    throw new LoanAgeingException("Instalments", "Interest must be non-negative with at most two decimals, or left blank pending confirmation.");
                if (row.InterestDueDate.HasValue && (row.InterestDueDate.Value.Year < 1753 || row.InterestDueDate.Value.Year >= 9999 || row.InterestDueDate.Value.Date < plan.DisbursementDate.Date))
                    throw new LoanAgeingException("Instalments", "Interest due dates must be valid dates on or after disbursement.");
            }
            if (!plan.InterestTermsConfirmed) return; // Existing principal-only revisions remain valid.
            if (!plan.InterestReceivableChartOfAccountId.HasValue || plan.InterestReceivableChartOfAccountId == Guid.Empty
                || !plan.InterestChargedChartOfAccountId.HasValue || plan.InterestChargedChartOfAccountId == Guid.Empty
                || plan.InterestReceivableChartOfAccountId == plan.PrincipalChartOfAccountId
                || plan.InterestReceivableChartOfAccountId == plan.InterestChargedChartOfAccountId
                || plan.InterestChargedChartOfAccountId == plan.PrincipalChartOfAccountId)
                throw new LoanAgeingException("InterestReceivableChartOfAccountId", "Configure distinct loan principal, interest-receivable and interest-charged accounts before confirming interest terms.");
            foreach (var row in plan.Instalments ?? new List<LoanPlanInstalmentDTO>())
            {
                if (row == null || !row.Interest.HasValue || row.Interest < 0 || row.Interest > 1000000000000000m || decimal.Round(row.Interest.Value, 2) != row.Interest)
                    throw new LoanAgeingException("Instalments", "Enter the agreed interest for every instalment, with at most two decimals. Enter zero explicitly when none is due.");
                if (row.Interest > 0 && (!row.InterestDueDate.HasValue || row.InterestDueDate.Value.Date < plan.DisbursementDate.Date || row.InterestDueDate.Value.Year >= 9999))
                    throw new LoanAgeingException("Instalments", "Every positive interest amount needs its contractual due date, on or after disbursement. Upfront interest is due on disbursement.");
            }
        }

        public static LoanInterestAgeingResult Calculate(Guid accountId, DateTime asAt, List<LoanAgeingCaseDTO> cases, List<LoanPlanDTO> plans, List<LoanAgeingPosting> postings)
        {
            var events = postings.Where(x => x.CustomerAccountId == accountId && x.EffectiveDate.Date <= asAt.Date).ToList();
            var result = new LoanInterestAgeingResult { Receivable = events.Sum(x => x.Amount) };
            if (cases.Count == 0) result.Issues.Add("No posted loan case is linked to the interest account.");
            if (cases.Any(x => x.Status == (int)LoanCaseStatus.Restructured) || events.Any(x => x.TransactionCode == (int)SystemTransactionCode.LoanRestructuring))
                result.Issues.Add("Restructured interest requires its effective replacement schedule and opening allocation.");
            foreach (var c in cases)
            {
                var matching = plans.Where(x => x.LoanCaseId == c.Id).ToList();
                if (matching.Count != 1 || !matching[0].IsConfirmed || !matching[0].InterestTermsConfirmed)
                    result.Issues.Add("Case " + c.CaseNumber + ": confirm contractual interest amounts and due dates, including an explicit zero for interest-free terms.");
            }
            foreach (var p in plans)
            {
                try { LoanAgeingEngine.ValidatePlan(p); }
                catch (LoanAgeingException e) { result.Issues.Add("Case " + p.CaseNumber + ": " + e.Message); }
                if (p.CustomerAccountId != accountId || p.DisbursementDate.Date > asAt.Date)
                    result.Issues.Add("The interest schedule does not match this account and reporting date.");
            }
            if (result.Issues.Count > 0) return result;
            var scheduleTotal = plans.Sum(x => x.Instalments.Sum(i => i.Interest.Value));
            result.ScheduledInterest = scheduleTotal;
            decimal charges = 0, settlements = 0;
            foreach (var e in events)
            {
                var applicable = plans.Where(p => p.InterestReceivableChartOfAccountId == e.ChartOfAccountId).ToList();
                if (applicable.Count == 0) { result.Issues.Add("An interest posting uses a G/L account outside the confirmed schedules."); continue; }
                if (!e.ContraChartOfAccountId.HasValue || e.ContraChartOfAccountId == Guid.Empty)
                { result.Issues.Add("An interest posting has no contra account; its charge or settlement cannot be identified."); continue; }
                bool charge = applicable.Any(p => p.InterestChargedChartOfAccountId == e.ContraChartOfAccountId);
                if (charge)
                {
                    // A credit against accrued income may be a waiver or charge reversal; it is not a payment.
                    if (e.Amount < 0) result.Issues.Add("An interest charge was reduced or reversed. Confirm revised contractual interest before this account can be aged.");
                    charges += e.Amount;
                }
                else if (e.Amount <= 0) settlements -= e.Amount;
                else
                {
                    bool linkedReversal = e.ParentJournalId.HasValue && events.Any(x => x.JournalId == e.ParentJournalId && x.Amount < 0 && x.ChartOfAccountId == e.ChartOfAccountId && x.ContraChartOfAccountId == e.ContraChartOfAccountId);
                    if (e.TransactionCode != (int)SystemTransactionCode.OverDeductionBatch && !linkedReversal)
                        result.Issues.Add("Interest increase on " + e.EffectiveDate.ToString("yyyy-MM-dd") + " is not an identified charge, refund or linked settlement reversal.");
                    settlements -= e.Amount;
                }
            }
            result.NetSettlements = settlements;
            var due = plans.Sum(p => p.Instalments.Where(i => i.Interest > 0 && i.InterestDueDate.Value.Date <= asAt.Date).Sum(i => i.Interest.Value));
            result.UnaccruedDueInterest = Math.Max(0, due - charges);
            if (charges > scheduleTotal) result.Issues.Add("Posted interest charges exceed the confirmed contractual total. Review the schedule, fees or duplicate charges.");
            if (settlements < 0 || settlements > scheduleTotal) result.Issues.Add("Net interest settlements fall outside the confirmed contractual total. Review refunds, excess payments or missing loan schedules.");
            if (result.Receivable < 0) result.Issues.Add("Interest receivable has a credit balance. Review prepayments and accruals.");
            if (result.UnaccruedDueInterest > 0) result.Issues.Add("Contractual interest is due but has not been fully charged in the ledger. Review accruals; a zero receivable balance does not establish settlement.");
            if (result.Issues.Count > 0) return result;
            var available = settlements;
            foreach (var row in plans.SelectMany(p => p.Instalments.Where(i => i.Interest > 0).Select(i => new LoanInterestInstalmentResult { PlanId = p.Id, CaseNumber = p.CaseNumber, Number = i.Number, DueDate = i.InterestDueDate.Value.Date, Interest = i.Interest.Value })).OrderBy(x => x.DueDate).ThenBy(x => x.CaseNumber).ThenBy(x => x.Number))
            {
                row.AllocatedInterest = Math.Min(available, row.Interest);
                available -= row.AllocatedInterest;
                row.RemainingInterest = row.Interest - row.AllocatedInterest;
                result.Instalments.Add(row);
            }
            result.OverdueInterest = result.Instalments.Where(x => x.DueDate < asAt.Date).Sum(x => x.RemainingInterest);
            var oldest = result.Instalments.FirstOrDefault(x => x.DueDate < asAt.Date && x.RemainingInterest > 0);
            result.DaysPastDue = oldest == null ? 0 : (asAt.Date - oldest.DueDate).Days;
            result.Bucket = LoanAgeingEngine.Bucket(result.DaysPastDue.Value);
            return result;
        }
    }
}
