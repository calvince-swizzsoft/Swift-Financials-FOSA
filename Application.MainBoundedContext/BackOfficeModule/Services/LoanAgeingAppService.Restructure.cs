using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanCaseAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanRepaymentPlanAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
namespace Application.MainBoundedContext.BackOfficeModule.Services
{
 public partial class LoanAgeingAppService
 {
  LoanPlanDTO RestructurePlan(LoanCase c,ServiceHeader h,LoanPlanDTO pending=null)
  {
   var links=customers.AllMatching(new DirectSpecification<CustomerAccount>(x=>x.CustomerId==c.CustomerId&&x.CustomerAccountType.TargetProductId==c.LoanProductId),h).ToList();
   Check(links.Count==1,"CustomerAccountId","Resolve the restructuring to one loan account before capturing its schedule.");
   var info=pending==null?cases.DatabaseSqlQuery<LoanAgeingCaseDTO>(CasesSql+" WHERE l.Id=@Id",h,new SqlParameter("@Id",c.Id)).Single():new LoanAgeingCaseDTO{PrincipalChartOfAccountId=pending.PrincipalChartOfAccountId,InterestReceivableChartOfAccountId=pending.InterestReceivableChartOfAccountId,InterestChargedChartOfAccountId=pending.InterestChargedChartOfAccountId};
   var es=ReadPostings(null,null,h).Where(x=>x.CustomerAccountId==links[0].Id).ToList();
   var funding=es.Where(x=>x.TransactionCode==23&&x.Amount>0&&x.ChartOfAccountId==info.PrincipalChartOfAccountId&&x.EffectiveDate.Date==c.CreatedDate.Date&&(x.Reference??"")== (c.Reference??"")).ToList();
   if(pending!=null)funding=new List<LoanAgeingPosting>{new LoanAgeingPosting{JournalId=pending.SourceJournalId,EffectiveDate=pending.EffectiveAt.Value,Amount=pending.Principal}};
   Check(funding.Count==1,"SourceJournalId","The restructuring must have one identifiable positive principal journal with the same reference and date as its case.");
   var source=funding[0];var at=source.EffectiveDate;
   Check(es.Where(x=>x.TransactionCode==23&&x.EffectiveDate<=at).Sum(x=>x.Amount)==0,"Principal","The restructuring clearing journals do not net to zero. Correct the posting before capturing terms.");
   var previousCases=cases.DatabaseSqlQuery<LoanAgeingCaseDTO>(CasesSql+" WHERE l.CustomerId=@Customer AND l.LoanProductId=@Product AND l.Status IN (48829,48833)",h,new SqlParameter("@Customer",c.CustomerId),new SqlParameter("@Product",c.LoanProductId)).Where(x=>x.Id!=c.Id&&x.CreatedDate<=at).ToList();
   Check(!previousCases.Any(x=>x.Status==48833),"LoanCaseId","A second restructuring cannot be confirmed through this workflow.");
   var ids=previousCases.Select(x=>x.Id).ToArray();
   var old=plans.AllMatching(new DirectSpecification<LoanRepaymentPlan>(x=>ids.Contains(x.LoanCaseId)),h).GroupBy(x=>x.LoanCaseId).Select(g=>g.OrderByDescending(x=>x.Revision).First()).ToList();
   var prior=old.Select(x=>Dto(x,previousCases.Single(z=>z.Id==x.LoanCaseId).CaseNumber,h)).ToList();
   // Cases with no funding before the boundary are not part of the opening allocation.
   previousCases=previousCases.Where(x=>es.Any(e=>e.TransactionCode==21&&e.EffectiveDate<=at&&(e.Reference??"").EndsWith("~L#"+x.CaseNumber.ToString("D7"),StringComparison.OrdinalIgnoreCase))).ToList();
   prior=prior.Where(x=>previousCases.Any(z=>z.Id==x.LoanCaseId)).ToList();
   var interest=ReadInterestPostings(at.Date.AddDays(1),h).Where(x=>x.CustomerAccountId==links[0].Id&&x.EffectiveDate<=at).ToList();
   var opening=es.Where(x=>x.EffectiveDate<=at).Sum(x=>x.Amount);
   Check(opening>0&&opening==source.Amount,"Principal","The restructuring amount does not match principal at its posting boundary. Review backdated changes and the clearing journals.");
   Check(interest.Sum(x=>x.Amount)==0,"OpeningInterest","Settle or separately account for existing interest before confirming this restructuring. Nonzero interest cannot be silently carried forward.");
   var principalAge=LoanAgeingEngine.Calculate(links[0].Id,at.Date,previousCases,prior,es.Where(x=>x.EffectiveDate<=at&&x.TransactionCode!=23).ToList());
   var interestAge=LoanInterestAgeingEngine.Calculate(links[0].Id,at.Date,previousCases,prior,interest);
   int? risk=principalAge.DaysPastDue.HasValue&&interestAge.DaysPastDue.HasValue?(int?)LoanRestructureAgeing.Category(Math.Max(principalAge.DaysPastDue.Value,interestAge.DaysPastDue.Value),LoanRestructureAgeing.Missed(principalAge,interestAge,at)):null;
   var previousReview=riskReviews.AllMatching(new DirectSpecification<LoanRiskReview>(x=>x.CustomerAccountId==links[0].Id&&x.AsAt<=at),h).OrderByDescending(x=>x.AsAt).ThenByDescending(x=>x.Revision).FirstOrDefault();
   if(risk.HasValue&&previousReview!=null)risk=Math.Max(risk.Value,previousReview.RiskCategory);
   Check(prior.Count<=100,"PriorPlanIds","This opening contains too many schedule revisions for one supported restructuring.");
   return new LoanPlanDTO{LoanCaseId=c.Id,CaseNumber=c.CaseNumber,CustomerAccountId=links[0].Id,SourceJournalId=source.JournalId,PrincipalChartOfAccountId=info.PrincipalChartOfAccountId,InterestReceivableChartOfAccountId=info.InterestReceivableChartOfAccountId,InterestChargedChartOfAccountId=info.InterestChargedChartOfAccountId,IsRestructuring=true,EffectiveAt=at,DisbursementDate=at.Date,Principal=opening,OpeningInterest=0,PriorRiskCategory=risk,PriorPlanIds=string.Join(",",prior.Select(x=>x.Id).OrderBy(x=>x)),OpeningLedgerHash=LoanRestructureAgeing.Hash(es,interest,at),AllocationPolicy=LoanAgeingEngine.Policy,Evidence=""};
  }
  public void CaptureRestructureSchedule(LoanPlanDTO pending,IEnumerable<AmortizationTableEntry> schedule,ServiceHeader h)
  {
   using(var scope=scopes.Create())
   {
    var caseId=pending.LoanCaseId;var c=cases.Get(caseId,h);Check(c!=null&&c.Status==48833,"LoanCaseId","A posted restructuring case is required.");
    Check(!plans.AllMatching(new DirectSpecification<LoanRepaymentPlan>(x=>x.LoanCaseId==caseId),h).Any(),"Revision","A restructuring schedule already exists.",409);
    var p=RestructurePlan(c,h,pending);var rows=schedule.ToList();
    for(int i=0;i<rows.Count;i++)p.Instalments.Add(new LoanPlanInstalmentDTO{Number=i+1,DueDate=rows[i].DueDate.Date,Principal=decimal.Round(rows[i].PrincipalPayment,2,MidpointRounding.AwayFromZero),Interest=decimal.Round(rows[i].InterestPayment,2,MidpointRounding.AwayFromZero),InterestDueDate=rows[i].DueDate.Date});
    if(p.Instalments.Count>0)p.Instalments.Last().Principal+=p.Principal-p.Instalments.Sum(x=>x.Principal);
    p.Evidence="Captured from the posted restructuring. Review contractual terms, prior schedules and retained risk classification.";p.Revision=1;
    LoanAgeingEngine.ValidatePlan(p);plans.Add(LoanAgeingEngine.Entity(p,h),h);scope.SaveChanges(h);
   }
  }
 }
}
