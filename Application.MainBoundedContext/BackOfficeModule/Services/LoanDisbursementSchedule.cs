using System;
using System.Collections.Generic;
using System.Linq;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Infrastructure.Crosscutting.Framework.Utils;

namespace Application.MainBoundedContext.BackOfficeModule.Services
{
    public static class LoanDisbursementSchedule
    {
        public static decimal UpfrontInterest(LoanDisbursementBatchEntryDTO terms, IEnumerable<AmortizationTableEntry> rows)
        {
            return LoanScheduleGeneration.PeriodicInterest(rows.Sum(x => x.InterestPayment),
                terms.LoanCaseLoanRegistrationMinimumInterestAmount * terms.LoanCaseLoanRegistrationTermInMonths,
                terms.LoanCaseLoanRegistrationRoundingType);
        }

        // Called only by the live disbursement workflow. Historical CaptureDraft and
        // saved revisions deliberately keep their existing confirmation state.
        public static LoanPlanDTO Build(LoanDisbursementBatchEntryDTO t, Guid accountId, Guid journalId,
            DateTime effectiveDate, decimal principal, IList<AmortizationTableEntry> calculated)
        {
            var p = new LoanPlanDTO {
                LoanCaseId=t.LoanCaseId, CustomerAccountId=accountId, SourceJournalId=journalId,
                PrincipalChartOfAccountId=t.LoanCaseLoanProductChartOfAccountId,
                InterestReceivableChartOfAccountId=t.LoanCaseLoanProductInterestReceivableChartOfAccountId,
                InterestChargedChartOfAccountId=t.LoanCaseLoanProductInterestChargedChartOfAccountId,
                DisbursementDate=effectiveDate.Date, Principal=principal,
                AllocationPolicy=LoanAgeingEngine.Policy, Revision=1
            };
            if (t.LoanCaseStatus != (int)LoanCaseStatus.Audited)
                throw new LoanAgeingException("Status", "Automatic schedule confirmation requires a verified loan entering disbursement.");
            bool upfront=t.LoanCaseLoanInterestChargeMode==(int)InterestChargeMode.Upfront;
            bool recoveredUpfront=t.LoanCaseLoanInterestRecoveryMode==(int)InterestRecoveryMode.Upfront;
            if (!Enum.IsDefined(typeof(InterestChargeMode),t.LoanCaseLoanInterestChargeMode) ||
                !Enum.IsDefined(typeof(InterestRecoveryMode),t.LoanCaseLoanInterestRecoveryMode))
                throw new LoanAgeingException("Interest", "The saved interest charging or recovery mode is invalid.");
            var generated=LoanScheduleGeneration.Generate(p,t.LoanCaseLoanRegistrationTermInMonths,
                t.LoanCaseLoanRegistrationPaymentFrequencyPerYear,t.LoanCaseLoanRegistrationGracePeriod,
                t.LoanCaseLoanRegistrationPaymentDueDate,t.LoanCaseLoanInterestCalculationMode,
                t.LoanCaseLoanInterestAnnualPercentageRate,upfront?0:t.LoanCaseLoanRegistrationMinimumInterestAmount,
                upfront?(int?)null:t.LoanCaseLoanRegistrationRoundingType);
            if(calculated==null || calculated.Count!=p.Instalments.Count || calculated.Where((x,i)=>Math.Abs(decimal.Round(x.PrincipalPayment,2,MidpointRounding.AwayFromZero)-p.Instalments[i].Principal)>.01m*(p.Instalments.Count+1)).Any())
                throw new LoanAgeingException("Instalments", "The disbursement calculation differs from the approved loan terms. Review the schedule before disbursing.");
            var review=new List<string>();
            if(upfront!=recoveredUpfront)review.Add("Mixed upfront and periodic interest terms require contractual review.");
            if(upfront && recoveredUpfront)LoanScheduleGeneration.Upfront(p,UpfrontInterest(t,calculated));
            bool averaged=t.LoanCaseLoanInterestCalculationMode==(int)InterestCalculationMode.StraightLineAmortization || t.LoanCaseLoanInterestCalculationMode==(int)InterestCalculationMode.DiminishingBalanceAmortization;
            decimal expectedPrincipal=averaged?p.Instalments.Sum(x=>x.Principal)/t.LoanCaseLoanRegistrationTermInMonths:p.Instalments[0].Principal;
            decimal expectedInterest=recoveredUpfront?0:averaged?p.Instalments.Sum(x=>x.Interest.Value)/t.LoanCaseLoanRegistrationTermInMonths:p.Instalments[0].Interest.Value;
            if(t.LoanCaseApprovedPrincipalPayment!=0 && Math.Abs(t.LoanCaseApprovedPrincipalPayment-expectedPrincipal)>.01m)
                review.Add("The approved principal payment overrides the generated schedule.");
            if(t.LoanCaseApprovedInterestPayment!=0 && Math.Abs(t.LoanCaseApprovedInterestPayment-expectedInterest)>.01m)
                review.Add("The approved interest payment overrides the generated schedule.");
            // Validate complete interest terms even if another exception requires review.
            p.InterestTermsConfirmed=true;
            try { LoanInterestAgeingEngine.ValidateTerms(p); }
            catch(LoanAgeingException e) { review.Add(e.Message); }
            p.IsConfirmed=p.InterestTermsConfirmed=review.Count==0;
            p.Evidence=(p.IsConfirmed
                ? "Automatically validated and confirmed at disbursement from the verified loan's saved approved terms and effective date. "
                : "Captured at disbursement; manual contractual review required. "+string.Join(" ",review)+" ")+generated.Terms;
            LoanAgeingEngine.ValidatePlan(p);
            return p;
        }
    }
}
