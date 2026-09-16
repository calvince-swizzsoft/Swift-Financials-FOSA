using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Infrastructure.Crosscutting.Framework.Utils;
namespace Application.MainBoundedContext.BackOfficeModule.Services
{
 public partial class LoanAgeingAppService
 {
  public LoanScheduleProposalDTO GenerateSchedule(Guid caseId,ServiceHeader h)
  {
   using(scopes.CreateReadOnlyWithTransaction(IsolationLevel.Serializable))
   {
    var p=GetPlan(caseId,h);var c=cases.Get(caseId,h);
    Check(!p.IsRestructuring,"LoanCaseId","Replacement schedules use the approved restructuring terms. Open the existing replacement schedule to confirm it.");
    Check(c.LoanRegistration!=null&&c.LoanInterest!=null,"LoanCaseId","The loan case is missing its saved repayment terms.");
    var t=c.LoanRegistration;var interest=c.LoanInterest;
    bool upfrontCharge=interest.ChargeMode==(int)InterestChargeMode.Upfront,upfrontRecovery=interest.RecoveryMode==(int)InterestRecoveryMode.Upfront;
    Check(Enum.IsDefined(typeof(InterestChargeMode),(int)interest.ChargeMode)&&Enum.IsDefined(typeof(InterestRecoveryMode),(int)interest.RecoveryMode),"Interest","The saved interest charging or recovery mode is unsupported.");
    var r=LoanScheduleGeneration.Generate(p,t.TermInMonths,t.PaymentFrequencyPerYear,t.GracePeriod,t.PaymentDueDate,interest.CalculationMode,interest.AnnualPercentageRate,upfrontCharge?0:t.MinimumInterestAmount,upfrontCharge?(int?)null:t.RoundingType);
    if(!upfrontCharge)r.Terms+="; minimum interest KSh "+t.MinimumInterestAmount.ToString("0.00",CultureInfo.InvariantCulture)+" per repayment period, applied before "+(RoundingType)t.RoundingType+" rounding";
    r.Terms+="; charge "+(InterestChargeMode)interest.ChargeMode+"; recovery "+(InterestRecoveryMode)interest.RecoveryMode;
    // Historical loans retain their saved terms. Periodic charging never entered the
    // upfront deduction block at disbursement, even if recovery was configured upfront.
    if(!upfrontCharge&&upfrontRecovery)
     r.Terms+="; reproduced with periodic recovery because charging is periodic (legacy conflicting recovery setting retained in the loan record)";
    if(upfrontCharge&&upfrontRecovery)
    {
     var posted=ReadInterestPostings(new DateTime(9998,12,31),h).Where(x=>x.CustomerAccountId==p.CustomerAccountId&&x.ChartOfAccountId==p.InterestReceivableChartOfAccountId&&(x.JournalId==p.SourceJournalId||x.ParentJournalId==p.SourceJournalId)&&x.TransactionCode==(int)SystemTransactionCode.LoanDisbursement).ToList();
     decimal charged=posted.Where(x=>x.ContraChartOfAccountId==p.InterestChargedChartOfAccountId).Sum(x=>x.Amount);
     decimal paid=-posted.Where(x=>x.ContraChartOfAccountId!=p.InterestChargedChartOfAccountId).Sum(x=>x.Amount);
     var expected=Math.Max(p.Instalments.Sum(x=>x.Interest??0),t.MinimumInterestAmount*t.TermInMonths);
     if(upfrontCharge&&charged>0)
     {
      LoanScheduleGeneration.Upfront(p,charged);
      r.Terms+="; upfront interest KSh "+charged.ToString("0.00",CultureInfo.InvariantCulture)+" from the original disbursement charge";
      if(charged!=paid)r.Warnings.Add("The original upfront interest charge and recovery differ. Review the posting history before confirming interest terms.");
     }
     else
     {
      LoanScheduleGeneration.Upfront(p,decimal.Round(expected,2,MidpointRounding.AwayFromZero));
      if(expected>0)r.Warnings.Add("Upfront interest is configured but no matching original charge was found. The proposed interest is calculated, not evidence of payment.");
     }
    }
    else if(upfrontCharge)
     r.Warnings.Add("Upfront charging with periodic recovery requires confirmation of the contractual interest allocation.");
    if(!p.InterestReceivableChartOfAccountId.HasValue||!p.InterestChargedChartOfAccountId.HasValue||p.InterestReceivableChartOfAccountId==p.InterestChargedChartOfAccountId||p.InterestReceivableChartOfAccountId==p.PrincipalChartOfAccountId||p.InterestChargedChartOfAccountId==p.PrincipalChartOfAccountId)
     r.Warnings.Add("Configure distinct principal, interest-receivable and interest-charged accounts before confirming interest.");
    p.Evidence="Generated from the saved loan-case terms and original posted disbursement. "+r.Terms+". First due date follows saved beginning/end-of-period and grace settings. Confirmed by the saving user as the reporting schedule; no independent credit-quality assessment is implied.";
    p.IsConfirmed=false;p.InterestTermsConfirmed=false;
    LoanAgeingEngine.ValidatePlan(p);r.CanConfirm=r.Warnings.Count==0;
    using(var sha=SHA256.Create())r.ProposalHash=BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(r)))).Replace("-","");
    return r;
   }
  }
  public List<LoanPlanDTO> ConfirmGeneratedSchedules(List<LoanScheduleConfirmationDTO> input,ServiceHeader h)
  {
   Check(input!=null&&input.Count>0&&input.Count<=100,"Schedules","Select between 1 and 100 generated schedules.");
   Check(input.All(x=>x!=null)&&input.Select(x=>x.LoanCaseId).Distinct().Count()==input.Count,"Schedules","Select each loan case once.");
   using(var scope=scopes.CreateWithTransaction(IsolationLevel.Serializable))
   {
    var proposals=input.Select(x=>new{Request=x,Proposal=GenerateSchedule(x.LoanCaseId,h)}).ToList();
    foreach(var x in proposals){Check(x.Proposal.CanConfirm,"Schedules","Case "+x.Proposal.Plan.CaseNumber+": resolve its generation exceptions before bulk confirmation.");Check(x.Request.Revision==x.Proposal.Plan.Revision&&x.Request.ProposalHash==x.Proposal.ProposalHash,"Revision","A loan's terms or schedule changed. Generate a fresh preview before confirming.",409);}
    var saved=new List<LoanPlanDTO>();
    foreach(var x in proposals){x.Proposal.Plan.IsConfirmed=true;x.Proposal.Plan.InterestTermsConfirmed=true;saved.Add(SavePlan(x.Proposal.Plan,h));}
    scope.SaveChanges(h);return saved;
   }
  }
 }
}
