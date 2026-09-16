using System;
using System.Linq;
using System.Collections.Generic;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanRepaymentPlanAgg;
using Infrastructure.Crosscutting.Framework.Utils;
namespace Application.MainBoundedContext.BackOfficeModule.Services
{
 public static class LoanAgeingEngine
 {
  public const string Policy="PRINCIPAL-FIFO-V1";
  public static void ValidatePlan(LoanPlanDTO input)
  {
   if(input==null)throw new LoanAgeingException("Plan","Provide the repayment schedule.");
   if(input.LoanCaseId==Guid.Empty||input.CustomerAccountId==Guid.Empty||input.PrincipalChartOfAccountId==Guid.Empty||input.SourceJournalId==Guid.Empty)throw new LoanAgeingException("LoanCaseId","Select a loan case, its customer account and its posted disbursement.");
   if(input.DisbursementDate.Year<1753||input.DisbursementDate.Year>=9999)throw new LoanAgeingException("DisbursementDate","Enter a valid original disbursement date.");
   if(input.Principal<=0||input.Principal>1000000000000000m||decimal.Round(input.Principal,2)!=input.Principal)throw new LoanAgeingException("Principal","Principal must be positive, with at most two decimal places.");
   if(input.AllocationPolicy!=Policy)throw new LoanAgeingException("AllocationPolicy","Use the supported oldest-principal-instalment-first allocation policy.");
   if(string.IsNullOrWhiteSpace(input.Evidence)||input.Evidence.Length>2000)throw new LoanAgeingException("Evidence","Describe the source of the confirmed due dates and repayment terms (up to 2,000 characters).");
   if(input.Instalments==null||input.Instalments.Count==0||input.Instalments.Count>1200)throw new LoanAgeingException("Instalments","Provide between 1 and 1,200 principal instalments.");
   LoanInterestAgeingEngine.ValidateTerms(input);
   DateTime previous=input.DisbursementDate.Date.AddDays(-1);
   for(int i=0;i<input.Instalments.Count;i++)
   {
    var row=input.Instalments[i];
    if(row==null||row.Number!=i+1||row.DueDate.Date<=previous||row.DueDate.Year>=9999||row.Principal<=0||decimal.Round(row.Principal,2)!=row.Principal)throw new LoanAgeingException("Instalments["+i+"]","Use sequential instalments, increasing due dates on or after disbursement, and positive principal amounts with at most two decimal places.");
    previous=row.DueDate.Date;
   }
   if(input.Instalments.Sum(x=>x.Principal)!=input.Principal)throw new LoanAgeingException("Instalments","The instalment principal total must exactly equal the loan's scheduled principal. Adjust the final instalment for rounding.");
  }
  public static LoanRepaymentPlan CaptureDraft(Guid loanCaseId,Guid accountId,Guid glId,Guid journalId,DateTime effectiveDate,decimal principal,IEnumerable<AmortizationTableEntry> schedule,ServiceHeader h)
  {
   var rows=schedule.ToList();
   var p=new LoanPlanDTO{LoanCaseId=loanCaseId,CustomerAccountId=accountId,PrincipalChartOfAccountId=glId,SourceJournalId=journalId,DisbursementDate=effectiveDate.Date,Principal=decimal.Round(principal,2),AllocationPolicy=Policy,Evidence="Captured at disbursement from the generated principal schedule. Confirm contractual due dates before ageing.",Revision=1};
   for(int i=0;i<rows.Count;i++)p.Instalments.Add(new LoanPlanInstalmentDTO{Number=i+1,DueDate=rows[i].DueDate.Date,Interest=decimal.Round(rows[i].InterestPayment,2,MidpointRounding.AwayFromZero),InterestDueDate=rows[i].DueDate.Date,Principal=decimal.Round(rows[i].PrincipalPayment,2,MidpointRounding.AwayFromZero)});
   if(p.Instalments.Count>0)p.Instalments.Last().Principal+=p.Principal-p.Instalments.Sum(x=>x.Principal);
   ValidatePlan(p);return Entity(p,h);
  }
  public static LoanRepaymentPlan Entity(LoanPlanDTO p,ServiceHeader h)
  {
   var e=new LoanRepaymentPlan{IsRestructuring=p.IsRestructuring,EffectiveAt=p.EffectiveAt,OpeningInterest=p.OpeningInterest,PriorRiskCategory=p.PriorRiskCategory,PriorPlanIds=p.PriorPlanIds,OpeningLedgerHash=p.OpeningLedgerHash,InterestTermsConfirmed=p.InterestTermsConfirmed,InterestReceivableChartOfAccountId=p.InterestReceivableChartOfAccountId,InterestChargedChartOfAccountId=p.InterestChargedChartOfAccountId,LoanCaseId=p.LoanCaseId,CustomerAccountId=p.CustomerAccountId,PrincipalChartOfAccountId=p.PrincipalChartOfAccountId,SourceJournalId=p.SourceJournalId,Revision=p.Revision,IsConfirmed=p.IsConfirmed,DisbursementDate=p.DisbursementDate.Date,Principal=p.Principal,Evidence=p.Evidence.Trim(),AllocationPolicy=p.AllocationPolicy,CreatedBy=h.ApplicationUserName,CreatedDate=DateTime.Now};e.GenerateNewIdentity();
   foreach(var r in p.Instalments){var row=new LoanRepaymentInstalment{PlanId=e.Id,Number=r.Number,DueDate=r.DueDate.Date,Principal=r.Principal,Interest=r.Interest,InterestDueDate=r.InterestDueDate?.Date,CreatedDate=e.CreatedDate,CreatedBy=e.CreatedBy};row.GenerateNewIdentity();e.Instalments.Add(row);}return e;
  }
  // Project already allocated instalments, never allocate the same account repayments again per loan.
  public static List<LoanAgeingLoanResult> ByLoan(LoanAgeingAccountResult account,List<LoanAgeingCaseDTO> cases,DateTime asAt)
  {
   return cases.Select(c=>{
    var row=new LoanAgeingLoanResult{LoanCaseId=c.Id,CustomerAccountId=account.CustomerAccountId,CaseNumber=c.CaseNumber,LoaneeName=c.LoaneeName,Product=c.Product};
    row.Issues.AddRange(account.Issues);if(account.Interest!=null)row.Issues.AddRange(account.Interest.Issues);
    var principal=account.Instalments.Where(x=>x.CaseNumber==c.CaseNumber).ToList();
    int? principalDays=null,interestDays=null;
    if(account.DaysPastDue.HasValue&&account.Issues.Count==0){
     row.OutstandingPrincipal=principal.Sum(x=>x.RemainingPrincipal);
     row.OverduePrincipal=principal.Where(x=>x.DueDate.Date<asAt.Date).Sum(x=>x.RemainingPrincipal);
     principalDays=principal.Where(x=>x.RemainingPrincipal>0).Select(x=>Math.Max(0,(asAt.Date-x.DueDate.Date).Days)).DefaultIfEmpty(0).Max();
    }else if(cases.Count==1)row.OutstandingPrincipal=account.OutstandingPrincipal;
    if(account.Interest!=null&&account.Interest.DaysPastDue.HasValue&&account.Interest.Issues.Count==0){
     var interest=account.Interest.Instalments.Where(x=>x.CaseNumber==c.CaseNumber).ToList();
     row.OverdueInterest=interest.Where(x=>x.DueDate.Date<asAt.Date).Sum(x=>x.RemainingInterest);
     interestDays=interest.Where(x=>x.RemainingInterest>0).Select(x=>Math.Max(0,(asAt.Date-x.DueDate.Date).Days)).DefaultIfEmpty(0).Max();
    }
    if(principalDays.HasValue&&interestDays.HasValue){row.DaysPastDue=Math.Max(principalDays.Value,interestDays.Value);row.Status=Bucket(row.DaysPastDue.Value);}
    if(cases.Count>1)row.Issues.Add("Repayments shared by these loans are allocated to the oldest due instalments first. These are reporting allocations.");
    if(account.IsRestructured&&principal.Count==0&&account.OutstandingPrincipal>0&&principalDays.HasValue)row.Issues.Add("This loan's original balance is covered by the replacement restructuring schedule.");
    return row;
   }).ToList();
  }
  public static string Bucket(int days){return days==0?"Current":days<=30?"1–30 days":days<=90?"31–90 days":days<=180?"91–180 days":days<=360?"181–360 days":"Over 360 days";}
  public static LoanAgeingAccountResult Calculate(Guid accountId,DateTime asAt,List<LoanAgeingCaseDTO> cases,List<LoanPlanDTO> plans,List<LoanAgeingPosting> postings)
  {
   var result=new LoanAgeingAccountResult{CustomerAccountId=accountId,Bucket="Needs review",CaseNumbers=cases.Select(x=>x.CaseNumber).OrderBy(x=>x).ToList()};
   var events=postings.Where(x=>x.CustomerAccountId==accountId&&x.EffectiveDate.Date<=asAt.Date).ToList();
   result.OutstandingPrincipal=events.Sum(x=>x.Amount);
   if(events.Any(x=>x.TransactionCode==(int)SystemTransactionCode.LoanRestructuring)||cases.Any(x=>x.Status==(int)LoanCaseStatus.Restructured)){result.Issues.Add("Restructured account: confirm a versioned restructuring schedule and opening allocation before ageing. Original schedules cannot be reused.");return result;}
   if(events.Count==0&&cases.Count>0){result.Issues.Add("Loan marked disbursed without corresponding principal postings. Review the disbursement.");return result;}
   if(result.OutstandingPrincipal<0){result.Issues.Add("The loan account has a credit balance. Review overpayments or corrections.");return result;}
   if(result.OutstandingPrincipal==0){result.DaysPastDue=0;result.OverduePrincipal=0;result.Bucket="Settled";return result;}
   foreach(var c in cases)
   {
    var p=plans.SingleOrDefault(x=>x.LoanCaseId==c.Id);
    if(p==null||!p.IsConfirmed)result.Issues.Add("Case "+c.CaseNumber+": "+(p==null?"schedule required.":"captured schedule requires confirmation."));
   }
   if(cases.Count==0)result.Issues.Add("No posted loan case is linked to this account. Review the opening balance or lending source.");
   foreach(var p in plans)
   {
    try{ValidatePlan(p);}catch(LoanAgeingException e){result.Issues.Add("Case "+p.CaseNumber+": "+e.Message);continue;}
    if(p.CustomerAccountId!=accountId||p.DisbursementDate.Date>asAt.Date){result.Issues.Add("The selected schedule does not match this account and reporting date.");continue;}
    var funding=events.Where(x=>x.TransactionCode==(int)SystemTransactionCode.LoanDisbursement&&(x.JournalId==p.SourceJournalId||x.ParentJournalId==p.SourceJournalId)).Sum(x=>x.Amount);
    if(funding!=p.Principal)result.Issues.Add("Case "+p.CaseNumber+": scheduled principal does not match its posted disbursement and capitalized charges.");
   }
   var sources=new HashSet<Guid>(plans.Select(x=>x.SourceJournalId));
   foreach(var e in events.Where(x=>x.Amount>0))
   {
    bool funding=e.TransactionCode==(int)SystemTransactionCode.LoanDisbursement&&(sources.Contains(e.JournalId)||(e.ParentJournalId.HasValue&&sources.Contains(e.ParentJournalId.Value)));
    bool refund=e.TransactionCode==(int)SystemTransactionCode.OverDeductionBatch;
    // Only explicitly linked repayment reversals can be identified safely; older unlinked reversals require review.
    bool paymentReversal=e.ParentJournalId.HasValue&&events.Any(x=>x.JournalId==e.ParentJournalId&&x.Amount<0);
    if(!funding&&!refund&&!paymentReversal)result.Issues.Add("Principal increase on "+e.EffectiveDate.ToString("yyyy-MM-dd")+" has no matching scheduled disbursement or repayment reversal. Review its source.");
   }
   if(result.Issues.Count>0)return result;
   decimal paid=plans.Sum(x=>x.Principal)-result.OutstandingPrincipal;
   if(paid<0){result.Issues.Add("Outstanding principal exceeds the confirmed schedules. Review additional advances, fees or refunds.");return result;}
   // Recompute against the entire effective-date ledger at every as-at date. Net repayments
   // settle oldest due principal first, including future instalments for prepayments.
   // Refunds/reversals reduce the pool and reopen the most recently settled instalments.
   foreach(var row in plans.SelectMany(p=>p.Instalments.Select(i=>new LoanAgeingInstalmentResult{PlanId=p.Id,CaseNumber=p.CaseNumber,Number=i.Number,DueDate=i.DueDate.Date,Principal=i.Principal})).OrderBy(x=>x.DueDate).ThenBy(x=>x.CaseNumber).ThenBy(x=>x.Number))
   {
    row.AllocatedPrincipal=Math.Min(paid,row.Principal);paid-=row.AllocatedPrincipal;row.RemainingPrincipal=row.Principal-row.AllocatedPrincipal;
    row.DaysOverdue=row.RemainingPrincipal>0?Math.Max(0,(asAt.Date-row.DueDate).Days):0;result.Instalments.Add(row);
   }
   result.OverduePrincipal=result.Instalments.Where(x=>x.DueDate<asAt.Date).Sum(x=>x.RemainingPrincipal);
   var oldest=result.Instalments.FirstOrDefault(x=>x.RemainingPrincipal>0&&x.DueDate<asAt.Date);result.OldestUnpaidDueDate=oldest?.DueDate;result.DaysPastDue=oldest?.DaysOverdue??0;result.Bucket=Bucket(result.DaysPastDue.Value);
   if(result.Instalments.Sum(x=>x.RemainingPrincipal)!=result.OutstandingPrincipal)result.Issues.Add("Instalment balances do not reconcile to the account ledger.");
   return result;
  }
 }
}
