using System;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Security.Cryptography;
using System.Collections.Generic;
using Application.MainBoundedContext.DTO.BackOfficeModule;
namespace Application.MainBoundedContext.BackOfficeModule.Services
{
 public static class LoanRestructureAgeing
 {
  public static string Hash(IEnumerable<LoanAgeingPosting> principal,IEnumerable<LoanAgeingPosting> interest,DateTime at)
  {
   var text=string.Join("\n",principal.Concat(interest).Where(x=>x.EffectiveDate<=at&&x.TransactionCode!=23).OrderBy(x=>x.Id).Select(x=>x.Id+"|"+x.JournalId+"|"+x.ChartOfAccountId+"|"+x.ContraChartOfAccountId+"|"+x.Amount.ToString(CultureInfo.InvariantCulture)+"|"+x.EffectiveDate.ToString("O")+"|"+x.TransactionCode));
   using(var sha=SHA256.Create())return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(text))).Replace("-","");
  }
  public static int Category(int days,int missed=0){return Math.Max(days>360?4:days>180?3:days>30?2:days>0?1:0,missed>12?4:missed>6?3:missed>1?2:missed>0?1:0);}
  public static int Missed(LoanAgeingAccountResult p,LoanInterestAgeingResult interest,DateTime date)
  {
   return p.Instalments.Where(x=>x.DueDate<date.Date&&x.RemainingPrincipal>0).Select(x=>Tuple.Create(x.CaseNumber,x.Number)).Concat(interest.Instalments.Where(x=>x.DueDate<date.Date&&x.RemainingInterest>0).Select(x=>Tuple.Create(x.CaseNumber,x.Number))).Distinct().GroupBy(x=>x.Item1).Select(x=>x.Count()).DefaultIfEmpty(0).Max();
  }
  public static LoanAgeingAccountResult Calculate(Guid account,DateTime date,List<LoanAgeingCaseDTO> cases,List<LoanPlanDTO> plans,List<LoanAgeingPosting> principal,List<LoanAgeingPosting> interest)
  {
   var replacement=plans.Where(x=>x.IsRestructuring&&x.EffectiveAt.HasValue&&x.EffectiveAt.Value<date.Date.AddDays(1)).OrderByDescending(x=>x.EffectiveAt).First();
   var result=new LoanAgeingAccountResult{CustomerAccountId=account,CaseNumbers=cases.Select(x=>x.CaseNumber).OrderBy(x=>x).ToList(),OutstandingPrincipal=principal.Sum(x=>x.Amount),IsRestructured=true,PriorRiskCategory=replacement.PriorRiskCategory,RestructuringPlanId=replacement.Id,RestructuredAt=replacement.EffectiveAt,Bucket="Needs review",Interest=new LoanInterestAgeingResult{Receivable=interest.Sum(x=>x.Amount)}};
   var at=replacement.EffectiveAt.Value;
   if(!replacement.IsConfirmed||!replacement.InterestTermsConfirmed||!replacement.PriorRiskCategory.HasValue)result.Issues.Add("Confirm the replacement schedule, opening allocation and retained pre-restructure classification.");
   if(plans.Count(x=>x.IsRestructuring)>1||cases.Count(x=>x.Status==48833)>1)result.Issues.Add("Multiple restructurings require review; this workflow supports one restructuring per loan lifecycle.");
   if(Hash(principal,interest,at)!=replacement.OpeningLedgerHash)result.Issues.Add("Postings before the restructuring changed. Reload and confirm a corrected opening snapshot before ageing.");
   if(principal.Where(x=>x.EffectiveDate<=at).Sum(x=>x.Amount)!=replacement.Principal||interest.Where(x=>x.EffectiveDate<=at).Sum(x=>x.Amount)!=(replacement.OpeningInterest??-1))result.Issues.Add("The saved restructuring opening does not reconcile to its ledger boundary.");
   if(principal.Count(x=>x.JournalId==replacement.SourceJournalId&&x.TransactionCode==23&&x.Amount==replacement.Principal&&x.EffectiveDate==at)!=1||principal.Where(x=>x.TransactionCode==23&&x.EffectiveDate<=at).Sum(x=>x.Amount)!=0)result.Issues.Add("The source restructuring journals changed or no longer balance.");
   if(replacement.OpeningInterest!=0)result.Issues.Add("Interest carry-forward or capitalization requires a separate approved allocation; it cannot be assumed paid.");
   Guid[] prior;
   try{prior=(replacement.PriorPlanIds??"").Split(new[]{','},StringSplitOptions.RemoveEmptyEntries).Select(Guid.Parse).ToArray();}catch(FormatException){prior=new Guid[0];}
   if(prior.Length==0||prior.Any(id=>!plans.Any(p=>p.Id==id)))result.Issues.Add("The previous schedule revisions changed or are missing. Reconfirm the opening snapshot against the current history.");
   if(result.Issues.Count>0){result.Interest.Issues.Add("Resolve the restructuring opening and schedule before interest ageing.");return result;}
   var selected=plans.Where(x=>x.Id==replacement.Id||!prior.Contains(x.Id)&&!x.IsRestructuring).ToList();
   var cs=cases.Where(c=>selected.Any(x=>x.LoanCaseId==c.Id)).Select(c=>new LoanAgeingCaseDTO{Id=c.Id,CaseNumber=c.CaseNumber,Status=48829}).ToList();
   var ps=principal.Where(x=>x.EffectiveDate>at).ToList();
   // Synthetic opening exists only in this calculation, never in the posting repository.
   ps.Add(new LoanAgeingPosting{JournalId=replacement.SourceJournalId,CustomerAccountId=account,ChartOfAccountId=replacement.PrincipalChartOfAccountId,EffectiveDate=at,Amount=replacement.Principal,TransactionCode=21});
   var aged=LoanAgeingEngine.Calculate(account,date,cs,selected,ps);
   aged.Interest=LoanInterestAgeingEngine.Calculate(account,date,cs,selected,interest.Where(x=>x.EffectiveDate>at).ToList());
   aged.IsRestructured=true;aged.PriorRiskCategory=replacement.PriorRiskCategory;aged.RestructuringPlanId=replacement.Id;aged.RestructuredAt=at;aged.CaseNumbers=result.CaseNumbers;
   return aged;
  }
 }
}
