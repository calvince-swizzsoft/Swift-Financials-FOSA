using System;
using System.Linq;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.Services;
using Infrastructure.Crosscutting.Framework.Utils;
static class LoanDisbursementScheduleTests
{
 static int checks;
 static void Check(bool value,string label){if(!value)throw new Exception(label);checks++;}
 static LoanDisbursementBatchEntryDTO Terms(){return new LoanDisbursementBatchEntryDTO{
  LoanCaseId=Guid.NewGuid(),LoanCaseStatus=(int)LoanCaseStatus.Audited,LoanCaseApprovedAmount=40000,
  LoanCaseLoanProductChartOfAccountId=Guid.NewGuid(),LoanCaseLoanProductInterestReceivableChartOfAccountId=Guid.NewGuid(),LoanCaseLoanProductInterestChargedChartOfAccountId=Guid.NewGuid(),
  LoanCaseLoanRegistrationTermInMonths=11,LoanCaseLoanRegistrationPaymentFrequencyPerYear=12,
  LoanCaseLoanRegistrationPaymentDueDate=0,LoanCaseLoanInterestCalculationMode=(int)InterestCalculationMode.ReducingBalance,
  LoanCaseLoanInterestAnnualPercentageRate=12,LoanCaseLoanInterestChargeMode=(int)InterestChargeMode.Periodic,
  LoanCaseLoanInterestRecoveryMode=(int)InterestRecoveryMode.Periodic,LoanCaseLoanRegistrationRoundingType=(int)RoundingType.ToEven};}
 static LoanPlanDTO Build(LoanDisbursementBatchEntryDTO t){
  var pv=t.LoanCaseApprovedAmount+t.LoanCaseAuditTopUpAmount;
  var rows=new FinancialsService().RepaymentSchedule(t.LoanCaseLoanRegistrationTermInMonths,t.LoanCaseLoanRegistrationPaymentFrequencyPerYear,t.LoanCaseLoanRegistrationGracePeriod,t.LoanCaseLoanInterestCalculationMode,t.LoanCaseLoanInterestAnnualPercentageRate,-(double)pv,0,t.LoanCaseLoanRegistrationPaymentDueDate);
  return LoanDisbursementSchedule.Build(t,Guid.NewGuid(),Guid.NewGuid(),new DateTime(2026,8,25),pv,rows);
 }
 static void Reject(Action a){try{a();throw new Exception("Invalid schedule accepted");}catch(LoanAgeingException){checks++;}}
 public static void Run(){
  var t=Terms();var p=Build(t);
  Check(p.IsConfirmed&&p.InterestTermsConfirmed,"normal new loan must auto-confirm principal and interest");
  Check(p.Instalments.Sum(x=>x.Principal)==40000&&p.Instalments.Count==11,"principal reconciles");
  Check(p.Instalments.First().DueDate==new DateTime(2026,9,25),"effective date anchors schedule");
  Check(p.Instalments.First().Interest==400&&p.Instalments.All(x=>x.Interest.HasValue&&x.InterestDueDate.HasValue),"interest and due dates explicit");
  var stored=LoanAgeingEngine.Entity(p,new ServiceHeader{ApplicationUserName="operator"});
  Check(stored.IsConfirmed&&stored.InterestTermsConfirmed&&stored.CreatedBy=="operator"&&stored.CreatedDate.Date==DateTime.Today,"persisted confirmation retains actual audit identity/time");
  t.LoanCaseLoanRegistrationMinimumInterestAmount=450;t.LoanCaseLoanRegistrationRoundingType=(int)RoundingType.Ceiling;p=Build(t);
  Check(p.IsConfirmed&&p.Instalments.All(x=>x.Interest>=450),"minimum and rounding included before confirmation");
  t=Terms();t.LoanCaseAuditTopUpAmount=500;p=Build(t);Check(p.Principal==40500&&p.Instalments.Sum(x=>x.Principal)==40500,"capitalized charges included");
  t=Terms();t.LoanCaseLoanInterestAnnualPercentageRate=0;p=Build(t);Check(p.IsConfirmed&&p.Instalments.All(x=>x.Interest==0),"interest-free terms explicitly confirmed as zero");
  t=Terms();t.LoanCaseLoanInterestChargeMode=(int)InterestChargeMode.Upfront;t.LoanCaseLoanInterestRecoveryMode=(int)InterestRecoveryMode.Upfront;p=Build(t);
  Check(p.IsConfirmed&&p.Instalments[0].Interest>0&&p.Instalments[0].InterestDueDate==p.DisbursementDate&&p.Instalments.Skip(1).All(x=>x.Interest==0),"upfront interest due exactly once on effective date");
  t.LoanCaseLoanInterestRecoveryMode=(int)InterestRecoveryMode.Periodic;p=Build(t);Check(!p.IsConfirmed&&!p.InterestTermsConfirmed&&p.Evidence.Contains("manual"),"mixed interest modes remain reviewable drafts");
  t=Terms();t.LoanCaseApprovedPrincipalPayment=1;p=Build(t);Check(!p.IsConfirmed&&p.Evidence.Contains("overrides"),"nonstandard approved payment needs manual review");
  t=Terms();t.LoanCaseLoanProductInterestChargedChartOfAccountId=t.LoanCaseLoanProductChartOfAccountId;p=Build(t);Check(!p.IsConfirmed,"ambiguous mappings never confirmed");
  t=Terms();t.LoanCaseStatus=(int)LoanCaseStatus.Disbursed;Reject(()=>Build(t));
  t=Terms();t.LoanCaseLoanRegistrationPaymentFrequencyPerYear=0;Reject(()=>Build(t));
  var draft=LoanAgeingEngine.CaptureDraft(Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid(),new DateTime(2026,8,25),100,new[]{new Application.MainBoundedContext.DTO.AmortizationTableEntry{DueDate=new DateTime(2026,9,25),PrincipalPayment=100,InterestPayment=0}},new ServiceHeader{ApplicationUserName="import"});
  Check(!draft.IsConfirmed&&!draft.InterestTermsConfirmed,"historical capture still requires manual confirmation");
  Console.WriteLine("PASS: "+checks+" automatic disbursement schedule and historical exception checks.");
 }
}
