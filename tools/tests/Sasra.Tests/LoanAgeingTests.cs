using System;
using System.Linq;
using System.Collections.Generic;
using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Infrastructure.Crosscutting.Framework.Utils;
static class LoanAgeingTests
{
 static int checks;static void Check(bool ok,string message){if(!ok)throw new Exception("Loan ageing: "+message);checks++;}
 static Guid account=Guid.NewGuid(),caseId=Guid.NewGuid(),source=Guid.NewGuid(),gl=Guid.NewGuid();
 static LoanPlanDTO Plan(){return new LoanPlanDTO{Id=Guid.NewGuid(),LoanCaseId=caseId,CaseNumber=1,CustomerAccountId=account,SourceJournalId=source,PrincipalChartOfAccountId=gl,Principal=100,DisbursementDate=new DateTime(2026,1,1),IsConfirmed=true,AllocationPolicy=LoanAgeingEngine.Policy,Evidence="Signed terms",Instalments=new List<LoanPlanInstalmentDTO>{new LoanPlanInstalmentDTO{Number=1,DueDate=new DateTime(2026,1,31),Principal=50},new LoanPlanInstalmentDTO{Number=2,DueDate=new DateTime(2026,2,28),Principal=50}}};}
 static LoanAgeingPosting Event(decimal amount,DateTime date,int code=30){return new LoanAgeingPosting{Id=Guid.NewGuid(),JournalId=Guid.NewGuid(),CustomerAccountId=account,ChartOfAccountId=gl,Amount=amount,EffectiveDate=date,TransactionCode=code};}
 static List<LoanAgeingPosting> Events(){var e=Event(100,new DateTime(2026,1,1),21);e.JournalId=source;return new List<LoanAgeingPosting>{e};}
 static List<LoanAgeingCaseDTO> Cases(){return new List<LoanAgeingCaseDTO>{new LoanAgeingCaseDTO{Id=caseId,CaseNumber=1,Status=48829}};}
 static LoanAgeingAccountResult Build(DateTime date,List<LoanAgeingPosting> events=null,List<LoanPlanDTO> plans=null,List<LoanAgeingCaseDTO> cases=null){return LoanAgeingEngine.Calculate(account,date,cases??Cases(),plans??new List<LoanPlanDTO>{Plan()},events??Events());}
 static void Reject(Action<LoanPlanDTO> mutate){var p=Plan();mutate(p);try{LoanAgeingEngine.ValidatePlan(p);throw new Exception("Invalid schedule accepted");}catch(LoanAgeingException e){Check(!string.IsNullOrEmpty(e.Field),"useful field error");}}
 public static void Run()
 {
  LoanAgeingEngine.ValidatePlan(Plan());Check(true,"valid plan");
  var r=Build(new DateTime(2026,1,31));Check(r.Issues.Count==0&&r.DaysPastDue==0&&r.OverduePrincipal==0,"not overdue on due day");
  r=Build(new DateTime(2026,2,1));Check(r.DaysPastDue==1&&r.OverduePrincipal==50,"overdue from next day");
  var es=Events();es.Add(Event(-20,new DateTime(2026,1,20)));r=Build(new DateTime(2026,2,10),es);Check(r.OutstandingPrincipal==80&&r.OverduePrincipal==30&&r.DaysPastDue==10,"partial settlement retains oldest unpaid date");
  es=Events();es.Add(Event(-80,new DateTime(2026,1,20)));r=Build(new DateTime(2026,2,10),es);Check(r.OverduePrincipal==0&&r.Instalments[1].AllocatedPrincipal==30&&r.OutstandingPrincipal==20,"prepayment settles future principal");
  es.Add(Event(40,new DateTime(2026,2,5),37));r=Build(new DateTime(2026,2,10),es);Check(r.OverduePrincipal==10&&r.OutstandingPrincipal==60,"refund reopens latest settled principal");
  r=Build(new DateTime(2026,2,4),es);Check(r.OutstandingPrincipal==20&&r.OverduePrincipal==0,"historical date excludes later refund");
  es=Events();var payment=Event(-50,new DateTime(2026,1,20));es.Add(payment);var reversal=Event(50,new DateTime(2026,2,1));reversal.ParentJournalId=payment.JournalId;es.Add(reversal);r=Build(new DateTime(2026,2,2),es);Check(r.Issues.Count==0&&r.OverduePrincipal==50,"linked repayment reversal restores debt");
  es=Events();es.Add(Event(10,new DateTime(2026,1,2),24));r=Build(new DateTime(2026,2,2),es);Check(r.Issues.Count>0&&r.DaysPastDue==null,"unscheduled capitalized interest not falsely aged");
  es=Events();es.Add(Event(-110,new DateTime(2026,1,2)));r=Build(new DateTime(2026,2,2),es);Check(r.Issues.Count>0&&r.DaysPastDue==null,"credit balance needs review");
  es=Events();es.Add(Event(-100,new DateTime(2026,1,2)));r=Build(new DateTime(2026,2,2),es,new List<LoanPlanDTO>());Check(r.Bucket=="Settled"&&r.OutstandingPrincipal==0,"settled balance recognized without fabricating a schedule");
  r=Build(new DateTime(2026,2,2),null,new List<LoanPlanDTO>());Check(r.Issues.Count>0&&r.DaysPastDue==null,"missing history is unknown not current");
  var p=Plan();p.IsConfirmed=false;r=Build(new DateTime(2026,2,2),null,new List<LoanPlanDTO>{p});Check(r.DaysPastDue==null,"captured draft not treated as contractual confirmation");
  p=Plan();p.Principal=90;p.Instalments[1].Principal=40;r=Build(new DateTime(2026,2,2),null,new List<LoanPlanDTO>{p});Check(r.Issues.Any(x=>x.Contains("disbursement")),"funding mismatch detected");
  var cs=Cases();cs[0].Status=48833;r=Build(new DateTime(2026,2,2),null,null,cs);Check(r.Issues.Count>0&&r.DaysPastDue==null,"restructured loan cannot reuse old schedule");
  var p2=Plan();p2.Id=Guid.NewGuid();p2.LoanCaseId=Guid.NewGuid();p2.CaseNumber=2;p2.SourceJournalId=Guid.NewGuid();p2.Instalments[0].DueDate=new DateTime(2026,2,15);p2.Instalments[1].DueDate=new DateTime(2026,3,15);
  cs=Cases();cs.Add(new LoanAgeingCaseDTO{Id=p2.LoanCaseId,CaseNumber=2,Status=48829});es=Events();var funding=Event(100,new DateTime(2026,1,5),21);funding.JournalId=p2.SourceJournalId;es.Add(funding);es.Add(Event(-70,new DateTime(2026,2,1)));r=Build(new DateTime(2026,2,20),es,new List<LoanPlanDTO>{Plan(),p2},cs);Check(r.Issues.Count==0&&r.OutstandingPrincipal==130&&r.OverduePrincipal==30&&r.DaysPastDue==5,"shared account FIFO combines separate schedules without double counting");Check(r.Instalments.Select(x=>x.PlanId).Distinct().Count()==2,"allocation retains schedule provenance");
  foreach(var pair in new[]{Tuple.Create(0,"Current"),Tuple.Create(1,"1–30 days"),Tuple.Create(30,"1–30 days"),Tuple.Create(31,"31–90 days"),Tuple.Create(90,"31–90 days"),Tuple.Create(91,"91–180 days"),Tuple.Create(180,"91–180 days"),Tuple.Create(181,"181–360 days"),Tuple.Create(360,"181–360 days"),Tuple.Create(361,"Over 360 days")})Check(LoanAgeingEngine.Bucket(pair.Item1)==pair.Item2,"bucket boundary "+pair.Item1);
  Reject(p0=>p0.Instalments[0].Principal=49);Reject(p0=>p0.Principal=-1);Reject(p0=>p0.Instalments[1].DueDate=p0.Instalments[0].DueDate);Reject(p0=>p0.Instalments[0].DueDate=new DateTime(2025,1,1));Reject(p0=>p0.Instalments[0].Number=7);Reject(p0=>p0.Evidence="");Reject(p0=>p0.SourceJournalId=Guid.Empty);Reject(p0=>p0.AllocationPolicy="guess");Reject(p0=>p0.Instalments=null);
  var draft=LoanAgeingEngine.CaptureDraft(caseId,account,gl,source,new DateTime(2026,1,1),100,Enumerable.Range(1,3).Select(i=>new AmortizationTableEntry{DueDate=new DateTime(2026,i,28),PrincipalPayment=100m/3}),new ServiceHeader{ApplicationUserName="test"});Check(!draft.IsConfirmed&&draft.Revision==1&&draft.Instalments.Sum(x=>x.Principal)==100&&draft.Instalments.Last().Principal==33.34m,"new disbursement captures immutable draft with cents residual");
  Console.WriteLine("PASS Loan ageing: "+checks+" assertions using synthetic loan schedules and transactions.");
 }
}
