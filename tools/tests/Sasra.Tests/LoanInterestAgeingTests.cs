using System;
using System.Linq;
using System.Collections.Generic;
using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.DTO.BackOfficeModule;
static class LoanInterestAgeingTests
{
    static int checks;
    static Guid account=Guid.NewGuid(),caseId=Guid.NewGuid(),principal=Guid.NewGuid(),receivable=Guid.NewGuid(),income=Guid.NewGuid(),cash=Guid.NewGuid();
    static DateTime start=new DateTime(2026,1,1),due=new DateTime(2026,1,31);
    static void Check(bool condition,string message){if(!condition)throw new Exception("Interest ageing: "+message);checks++;}
    static LoanPlanDTO Plan(){return new LoanPlanDTO{Id=Guid.NewGuid(),LoanCaseId=caseId,CaseNumber=1,CustomerAccountId=account,PrincipalChartOfAccountId=principal,InterestReceivableChartOfAccountId=receivable,InterestChargedChartOfAccountId=income,SourceJournalId=Guid.NewGuid(),DisbursementDate=start,Principal=100,IsConfirmed=true,InterestTermsConfirmed=true,AllocationPolicy=LoanAgeingEngine.Policy,Evidence="Approved contract",Instalments=new List<LoanPlanInstalmentDTO>{new LoanPlanInstalmentDTO{Number=1,DueDate=due,Principal=50,Interest=10,InterestDueDate=due},new LoanPlanInstalmentDTO{Number=2,DueDate=due.AddMonths(1),Principal=50,Interest=10,InterestDueDate=due.AddMonths(1)}}};}
    static LoanAgeingPosting Event(decimal amount,DateTime date,bool charge=false){return new LoanAgeingPosting{Id=Guid.NewGuid(),JournalId=Guid.NewGuid(),CustomerAccountId=account,ChartOfAccountId=receivable,ContraChartOfAccountId=charge?income:cash,Amount=amount,EffectiveDate=date,TransactionCode=charge?21:30};}
    static LoanInterestAgeingResult Run(LoanPlanDTO p,List<LoanAgeingPosting> events,DateTime? at=null){return LoanInterestAgeingEngine.Calculate(account,at??due.AddDays(1),new List<LoanAgeingCaseDTO>{new LoanAgeingCaseDTO{Id=caseId,CaseNumber=1,Status=48829}},new List<LoanPlanDTO>{p},events);}
    static void Reject(Action<LoanPlanDTO> mutate){var p=Plan();mutate(p);try{LoanAgeingEngine.ValidatePlan(p);throw new Exception("Invalid interest terms accepted");}catch(LoanAgeingException e){Check(!string.IsNullOrWhiteSpace(e.Field),"field-aware validation");}}
    public static void Run()
    {
        var p=Plan();var es=new List<LoanAgeingPosting>{Event(20,start,true)};
        var r=Run(p,es,due);Check(r.OverdueInterest==0&&r.DaysPastDue==0,"due day is not overdue");
        r=Run(p,es);Check(r.OverdueInterest==10&&r.DaysPastDue==1&&r.Receivable==20,"unpaid interest ages independently");
        es.Add(Event(-4,due));r=Run(p,es);Check(r.OverdueInterest==6&&r.DaysPastDue==1,"partial interest keeps its original due date");
        es.Add(Event(-12,due));r=Run(p,es);Check(r.OverdueInterest==0&&r.Instalments[1].AllocatedInterest==6,"prepayments settle future interest");
        var refund=Event(8,due.AddDays(1));refund.TransactionCode=37;es.Add(refund);r=Run(p,es);Check(r.OverdueInterest==2&&r.Receivable==12,"refund reopens latest settlements");
        r=Run(p,es,due);Check(r.Receivable==4&&r.NetSettlements==16,"historical cutoff excludes later refund");
        es=new List<LoanAgeingPosting>{Event(20,start,true),Event(-20,start)};r=Run(p,es);Check(r.Receivable==0&&r.OverdueInterest==0,"fully paid upfront does not age again");
        p.Instalments[0].Interest=20;p.Instalments[0].InterestDueDate=start;p.Instalments[1].Interest=0;
        r=Run(p,new List<LoanAgeingPosting>{Event(20,start,true)},start.AddDays(1));Check(r.DaysPastDue==1&&r.OverdueInterest==20,"upfront interest due before first principal instalment");
        p=Plan();r=Run(p,new List<LoanAgeingPosting>());Check(r.Receivable==0&&r.OverdueInterest==null&&r.UnaccruedDueInterest==10,"uncharged interest is unknown, not settled");
        p.InterestTermsConfirmed=false;r=Run(p,es);Check(r.DaysPastDue==null,"principal-only revision does not imply zero interest");
        p=Plan();p.Instalments.ForEach(x=>x.Interest=0);r=Run(p,new List<LoanAgeingPosting>());Check(r.OverdueInterest==0&&r.DaysPastDue==0,"explicit interest-free schedule");
        p=Plan();es=new List<LoanAgeingPosting>{Event(20,start,true),Event(-10,due)};var reversal=Event(10,due.AddDays(1));reversal.ParentJournalId=es[1].JournalId;es.Add(reversal);r=Run(p,es);Check(r.OverdueInterest==10,"linked payment reversal");
        reversal.ParentJournalId=null;r=Run(p,es);Check(r.DaysPastDue==null&&r.Issues.Any(x=>x.Contains("not an identified")),"unlinked reversal needs review");
        es=new List<LoanAgeingPosting>{Event(20,start,true),Event(-5,due,true)};r=Run(p,es);Check(r.DaysPastDue==null&&r.NetSettlements==0,"charge reduction is not payment");
        es=new List<LoanAgeingPosting>{Event(30,start,true)};r=Run(p,es);Check(r.DaysPastDue==null,"charges beyond contractual total");
        es=new List<LoanAgeingPosting>{Event(20,start,true),Event(-25,due)};r=Run(p,es);Check(r.DaysPastDue==null,"excess settlement requires review");
        es=new List<LoanAgeingPosting>{Event(20,start,true)};es[0].ContraChartOfAccountId=null;r=Run(p,es);Check(r.DaysPastDue==null,"missing contra is not guessed");
        es=new List<LoanAgeingPosting>{Event(20,start,true)};es[0].ChartOfAccountId=Guid.NewGuid();r=Run(p,es);Check(r.DaysPastDue==null,"unmapped interest GL");
        var other=Plan();other.Id=Guid.NewGuid();other.LoanCaseId=Guid.NewGuid();other.CaseNumber=2;other.Instalments.ForEach(x=>x.InterestDueDate=x.InterestDueDate.Value.AddDays(1));
        var cases=new List<LoanAgeingCaseDTO>{new LoanAgeingCaseDTO{Id=caseId,CaseNumber=1,Status=48829},new LoanAgeingCaseDTO{Id=other.LoanCaseId,CaseNumber=2,Status=48829}};
        r=LoanInterestAgeingEngine.Calculate(account,due.AddDays(3),cases,new List<LoanPlanDTO>{Plan(),other},new List<LoanAgeingPosting>{Event(40,start,true),Event(-15,due)});
        Check(r.OverdueInterest==5&&r.DaysPastDue==2&&r.Instalments[1].CaseNumber==2,"shared account interest FIFO and oldest unpaid date");
        cases[1].Status=48833;r=LoanInterestAgeingEngine.Calculate(account,due.AddDays(3),cases,new List<LoanPlanDTO>{Plan(),other},new List<LoanAgeingPosting>());Check(r.DaysPastDue==null,"restructuring guard");
        Reject(x=>x.Instalments[0].Interest=null);Reject(x=>x.Instalments[0].Interest=-1);Reject(x=>x.Instalments[0].Interest=.001m);Reject(x=>x.Instalments[0].InterestDueDate=null);Reject(x=>x.Instalments[0].InterestDueDate=start.AddDays(-1));Reject(x=>x.InterestReceivableChartOfAccountId=principal);Reject(x=>x.InterestChargedChartOfAccountId=principal);Reject(x=>{x.InterestTermsConfirmed=false;x.Instalments[0].Interest=-1;});
        Console.WriteLine("PASS: "+checks+" interest-ageing assertions.");
    }
}
