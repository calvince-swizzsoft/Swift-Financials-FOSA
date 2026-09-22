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
  r.Interest=new LoanInterestAgeingResult{DaysPastDue=20,OverdueInterest=7,Instalments=new List<LoanInterestInstalmentResult>{new LoanInterestInstalmentResult{CaseNumber=1,DueDate=new DateTime(2026,1,31),RemainingInterest=7}}};
  var loanRows=LoanAgeingEngine.ByLoan(r,cs,new DateTime(2026,2,20));
  Check(loanRows.Count==2&&loanRows.Select(x=>x.LoanCaseId).Distinct().Count()==2,"one row per loan");
  Check(loanRows[0].OutstandingPrincipal==50&&loanRows[1].OutstandingPrincipal==80&&loanRows.Sum(x=>x.OutstandingPrincipal)==130,"allocated loan principal reconciles without duplicated repayments");
  Check(loanRows[0].OverduePrincipal==0&&loanRows[1].OverduePrincipal==30,"separate overdue principal");
  Check(loanRows[0].OverdueInterest==7&&loanRows[1].OverdueInterest==0,"separate overdue interest");
  Check(loanRows[0].DaysPastDue==20&&loanRows[1].DaysPastDue==5,"each loan uses its own maximum principal and interest days");
  Check(loanRows.Sum(x=>x.OutstandingInterest)==7&&loanRows.Sum(x=>x.TotalOutstanding)==137,"remaining interest and combined balance do not double count shared accounts");
  r.Interest.Instalments.Add(new LoanInterestInstalmentResult{CaseNumber=1,Number=2,DueDate=new DateTime(2026,4,30),RemainingInterest=11});
  loanRows=LoanAgeingEngine.ByLoan(r,cs,new DateTime(2026,2,20));
  Check(loanRows[0].OutstandingInterest==18&&loanRows[0].OverdueInterest==7,"remaining interest includes future interest but overdue interest does not");
  foreach(var boundary in new[]{Tuple.Create(0,"Performing"),Tuple.Create(1,"Watch"),Tuple.Create(30,"Watch"),Tuple.Create(31,"Substandard"),Tuple.Create(180,"Substandard"),Tuple.Create(181,"Doubtful"),Tuple.Create(360,"Doubtful"),Tuple.Create(361,"Loss")}){
   var snapshot=new LoanAgeingAccountResult{DaysPastDue=boundary.Item1,Interest=new LoanInterestAgeingResult{DaysPastDue=0}};
   snapshot.Instalments.Add(new LoanAgeingInstalmentResult{CaseNumber=1,Number=1,DueDate=new DateTime(2026,1,1),RemainingPrincipal=100});
   Check(LoanAgeingEngine.ByLoan(snapshot,Cases(),new DateTime(2026,1,1).AddDays(boundary.Item1))[0].RiskClassification==boundary.Item2,"register risk boundary "+boundary.Item1);
   snapshot.IsRestructured=true;snapshot.PriorRiskCategory=4;
   Check(LoanAgeingEngine.ByLoan(snapshot,Cases(),new DateTime(2026,1,1).AddDays(boundary.Item1))[0].RiskClassification=="Loss","restructuring cannot improve retained classification");
  }
  var missedSnapshot=new LoanAgeingAccountResult{DaysPastDue=5,Interest=new LoanInterestAgeingResult{DaysPastDue=0}};
  for(int n=1;n<=2;n++)missedSnapshot.Instalments.Add(new LoanAgeingInstalmentResult{CaseNumber=1,Number=n,DueDate=new DateTime(2026,2,15),RemainingPrincipal=50});
  Check(LoanAgeingEngine.ByLoan(missedSnapshot,Cases(),new DateTime(2026,2,20))[0].RiskClassification=="Substandard","missed instalment count can raise the risk above the day band");
  missedSnapshot.Interest.DaysPastDue=null;
  Check(LoanAgeingEngine.ByLoan(missedSnapshot,Cases(),new DateTime(2026,2,20))[0].RiskClassification=="Needs review","unresolved interest cannot be called performing");
  r.DaysPastDue=null;r.Issues.Add("Missing schedule");r.Instalments.Clear();
  loanRows=LoanAgeingEngine.ByLoan(r,cs,new DateTime(2026,2,20));
  Check(loanRows.All(x=>x.OutstandingPrincipal==null&&x.OverduePrincipal==null&&x.DaysPastDue==null&&x.Status=="Needs review"),"unknown shared allocation cannot duplicate the account balance or imply current");
  Check(LoanAgeingEngine.ByLoan(r,Cases(),new DateTime(2026,2,20))[0].OutstandingPrincipal==130,"single loan retains known ledger principal despite missing schedule");
  r.Issues.Clear();r.DaysPastDue=0;r.OutstandingPrincipal=0;r.Interest=new LoanInterestAgeingResult{DaysPastDue=0};
  Check(LoanAgeingEngine.ByLoan(r,cs,new DateTime(2026,2,20)).All(x=>x.OutstandingPrincipal==0&&x.DaysPastDue==0),"settled shared account does not require invented instalments");
  foreach(var pair in new[]{Tuple.Create(0,"Current"),Tuple.Create(1,"1–30 days"),Tuple.Create(30,"1–30 days"),Tuple.Create(31,"31–90 days"),Tuple.Create(90,"31–90 days"),Tuple.Create(91,"91–180 days"),Tuple.Create(180,"91–180 days"),Tuple.Create(181,"181–360 days"),Tuple.Create(360,"181–360 days"),Tuple.Create(361,"Over 360 days")})Check(LoanAgeingEngine.Bucket(pair.Item1)==pair.Item2,"bucket boundary "+pair.Item1);
  Reject(p0=>p0.Instalments[0].Principal=49);Reject(p0=>p0.Principal=-1);Reject(p0=>p0.Instalments[1].DueDate=p0.Instalments[0].DueDate);Reject(p0=>p0.Instalments[0].DueDate=new DateTime(2025,1,1));Reject(p0=>p0.Instalments[0].Number=7);Reject(p0=>p0.Evidence="");Reject(p0=>p0.SourceJournalId=Guid.Empty);Reject(p0=>p0.AllocationPolicy="guess");Reject(p0=>p0.Instalments=null);
  var draft=LoanAgeingEngine.CaptureDraft(caseId,account,gl,source,new DateTime(2026,1,1),100,Enumerable.Range(1,3).Select(i=>new AmortizationTableEntry{DueDate=new DateTime(2026,i,28),PrincipalPayment=100m/3}),new ServiceHeader{ApplicationUserName="test"});Check(!draft.IsConfirmed&&draft.Revision==1&&draft.Instalments.Sum(x=>x.Principal)==100&&draft.Instalments.Last().Principal==33.34m,"new disbursement captures immutable draft with cents residual");
  var productService=(Application.MainBoundedContext.AccountsModule.Services.LoanProductAppService)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(Application.MainBoundedContext.AccountsModule.Services.LoanProductAppService));
  foreach(var charge in new[]{InterestChargeMode.Periodic,InterestChargeMode.Upfront})foreach(var recovery in new[]{InterestRecoveryMode.Periodic,InterestRecoveryMode.Upfront}){
   // Empty account identifiers avoid repository access; isolate interest-mode field errors.
   var product=new Application.MainBoundedContext.DTO.AccountsModule.LoanProductDTO{LoanInterestChargeMode=(int)charge,LoanInterestRecoveryMode=(int)recovery};
   var validation=productService.ValidateLoanProduct(product,new ServiceHeader());
   Check(validation.ContainsKey("LoanInterestRecoveryMode")==(charge==InterestChargeMode.Periodic&&recovery==InterestRecoveryMode.Upfront),"product rejects only periodic charging with upfront recovery: "+charge+"/"+recovery);
  }
  Check(LoanScheduleGeneration.PeriodicInterest(20m,50m)==50m,"minimum interest is a per-period floor");
  Check(LoanScheduleGeneration.PeriodicInterest(80m,50m)==80m,"minimum is not added to calculated interest");
  Check(LoanScheduleGeneration.PeriodicInterest(0m,50m)==50m,"saved minimum applies even when rate interest is zero");
  Check(LoanScheduleGeneration.PeriodicInterest(0m,0m)==0m,"genuine interest-free terms stay zero");
  Check(LoanScheduleGeneration.PeriodicInterest(10.501m,0m,(int)RoundingType.ToEven)==11m,"round raw interest only once");
  Check(LoanScheduleGeneration.PeriodicInterest(1m,10.5m,(int)RoundingType.ToEven)==10m,"minimum before banker's rounding");
  Check(LoanScheduleGeneration.PeriodicInterest(1m,10.5m,(int)RoundingType.AwayFromZero)==11m,"minimum before away-from-zero rounding");
  Check(LoanScheduleGeneration.PeriodicInterest(10.1m,0m,(int)RoundingType.Ceiling)==11m,"ceiling rounding");
  Check(LoanScheduleGeneration.PeriodicInterest(10.9m,0m,(int)RoundingType.Floor)==10m,"floor rounding");
  var minimumPlan=Plan();var minimumProposal=LoanScheduleGeneration.Generate(minimumPlan,12,12,0,0,(int)InterestCalculationMode.ReducingBalance,12,5m);
  Check(minimumPlan.Instalments.Count==12&&minimumPlan.Instalments.All(x=>x.Interest==5m&&x.InterestDueDate==x.DueDate)&&minimumPlan.Instalments.Sum(x=>x.Principal)==100m,"minimum schedule retains dates and principal while flooring each periodic interest amount");
  try{LoanScheduleGeneration.PeriodicInterest(10m,-1m);throw new Exception("Negative minimum accepted");}catch(LoanAgeingException){Check(true,"invalid saved minimum rejected");}
  Console.WriteLine("PASS Loan ageing: "+checks+" assertions using synthetic loan schedules and transactions.");
 }
}
