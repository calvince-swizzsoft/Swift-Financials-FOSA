using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanRecoveryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
static class LoanRecoveryTests
{
 static int checks;
 static void Check(bool value,string label){if(!value)throw new Exception(label);checks++;}
 class Proxy:RealProxy{readonly Func<IMethodCallMessage,object> f;public Proxy(Type t,Func<IMethodCallMessage,object> f):base(t){this.f=f;}public override IMessage Invoke(IMessage msg){var c=(IMethodCallMessage)msg;try{return new ReturnMessage(f(c),null,0,c.LogicalCallContext,c);}catch(Exception e){return new ReturnMessage(e,c);}}}
 static T Stub<T>(Func<IMethodCallMessage,object> f){return (T)new Proxy(typeof(T),f).GetTransparentProxy();}
 static void Reject(Action a){try{a();throw new Exception("Expected recovery rejection");}catch(LoanAgeingException){checks++;}}
 static LoanRecoveryMember Member(decimal guarantee,decimal deposit,decimal other=0){return new LoanRecoveryMember{CustomerId=Guid.NewGuid(),GuarantorId=Guid.NewGuid(),RemainingGuarantee=guarantee,OtherCommitments=other,Accounts=new List<LoanRecoveryAccount>{new LoanRecoveryAccount{Id=Guid.NewGuid(),Available=deposit,Selected=true}}};}
 public static void Run()
 {
  var p=new LoanRecoveryPreview{PrincipalDue=75000,InterestDue=5000,Borrower=Member(0,20000),Guarantors=new List<LoanRecoveryMember>{Member(60000,60000),Member(40000,40000)}};
  LoanRecoveryAllocation.Allocate(p);Check(p.Borrower.Amount==20000&&p.Guarantors[0].Amount==36000&&p.Guarantors[1].Amount==24000,"Borrower first then 60/40 guarantees");
  Check(p.InterestRecovery==5000&&p.PrincipalRecovery==75000&&p.Shortfall==0,"Interest first, overdue principal only");
  p.Guarantors[0].Accounts[0].Available=10000;LoanRecoveryAllocation.Allocate(p);Check(p.Guarantors[0].Amount==10000&&p.Guarantors[1].Amount==40000&&p.Shortfall==10000,"Cap funds and guarantees; disclose shortfall");
  p.Borrower.OtherCommitments=5000;LoanRecoveryAllocation.Allocate(p);Check(p.Borrower.Amount==15000&&p.Shortfall==15000,"Other guarantees protected");
  p.Borrower.Accounts[0].Available=-20000;LoanRecoveryAllocation.Allocate(p);Check(p.Borrower.Amount==0,"Debit deposit balance is not recovery capacity");
  p.Borrower.Accounts[0].Available=100000;p.Borrower.OtherCommitments=0;LoanRecoveryAllocation.Allocate(p);Check(p.Borrower.Amount==80000&&p.Guarantors.All(g=>g.Amount==0),"Borrower covers all; no guarantor debit");
  p.Borrower.Accounts[0].Selected=false;p.Guarantors.ForEach(g=>g.Accounts.ForEach(a=>a.Selected=false));
  LoanRecoveryAllocation.Allocate(p);Check(p.TotalRecovery==0&&new[]{p.Borrower}.Concat(p.Guarantors).SelectMany(g=>g.Accounts).All(a=>a.Amount==0),"Deselecting clears all prior allocations");
  var random=new Random(514);
  for(var i=0;i<300;i++)
  {
   p=new LoanRecoveryPreview{PrincipalDue=random.Next(1,100000)/100m,InterestDue=random.Next(100)/100m,Borrower=Member(0,random.Next(5000)/100m),Guarantors=Enumerable.Range(0,random.Next(1,8)).Select(x=>Member(random.Next(1,40000)/100m,random.Next(40000)/100m,random.Next(5000)/100m)).ToList()};
   LoanRecoveryAllocation.Allocate(p);
   Check(p.TotalRecovery<=p.TotalDue&&p.TotalRecovery>=0&&p.Shortfall==p.TotalDue-p.TotalRecovery,"Conserves recovery target");
   Check(p.Guarantors.All(g=>g.Amount>=0&&g.Amount<=g.RemainingGuarantee&&g.Amount<=g.Available&&g.Amount==decimal.Round(g.Amount,2)),"No guarantee or deposit cap exceeded; exact cents");
   Check(new[]{p.Borrower}.Concat(p.Guarantors).All(g=>g.Accounts.Sum(a=>a.Amount)==g.Amount&&g.Accounts.All(a=>a.Amount<=a.Available)),"Account allocation reconciles");
  }
  ServiceTests();Console.WriteLine("PASS: "+checks+" recovery allocation and posting assertions.");
 }
 static void ServiceTests()
 {
  var id=Guid.NewGuid();var loan=new LoanAgeingLoanResult{LoanCaseId=id,CaseNumber=9,CustomerAccountId=Guid.NewGuid(),OverduePrincipal=80,OverdueInterest=20,DaysPastDue=90};
  var link=new LoanRecoveryAppService.LoanLink{AccountId=loan.CustomerAccountId,CustomerId=Guid.NewGuid(),BranchId=Guid.NewGuid(),PrincipalGlId=Guid.NewGuid(),InterestGlId=Guid.NewGuid(),Name="Borrower"};
  var guarantee=new LoanRecoveryAppService.Guarantee{Id=Guid.NewGuid(),CustomerId=Guid.NewGuid(),Name="Guarantor",AmountGuaranteed=100};
  var period=Guid.NewGuid();var sourceId=Guid.NewGuid();var guarantorSourceId=Guid.NewGuid();var unusedSourceId=Guid.NewGuid();var depositGl=Guid.NewGuid();decimal deposits=30,principal=80,interest=10;int commits=0;bool eligible=true,fail=false;
  var records=new List<LoanRecovery>();var pendingRecords=new List<LoanRecovery>();var posted=new List<Journal>();var pendingJournals=new List<Journal>();
  var read=Stub<IDbContextReadOnlyScope>(c=>null);
  var scope=Stub<IDbContextScope>(c=>{if(c.MethodName=="SaveChanges"){Check(pendingRecords.Count==1&&pendingJournals.Count>0,"History and journals staged before one commit");if(fail)throw new LoanAgeingException("Test","Simulated commit failure");records.AddRange(pendingRecords);posted.AddRange(pendingJournals);commits++;return 1;}if(c.MethodName=="Dispose"){pendingRecords.Clear();pendingJournals.Clear();}return null;});
  var scopes=Stub<IDbContextScopeFactory>(c=>{if(c.MethodName=="CreateReadOnly")return read;Check(c.MethodName=="CreateWithTransaction"&&(IsolationLevel)c.Args[0]==IsolationLevel.Serializable,"Posting uses serializable transaction");return scope;});
  var notice=Stub<ILoanNoticeAppService>(c=>{Check(c.MethodName=="RecoveryLoan","Uses current recovery eligibility");if(!eligible)throw new LoanAgeingException("Test","Not eligible");return new LoanNoticeLoanRow{BasisHash="policy"};});
  var age=Stub<ILoanAgeingAppService>(c=>new LoanAgeingResult{Loans=new List<LoanAgeingLoanResult>{loan}});
  var repo=Stub<IRepository<LoanRecovery>>(c=>{
   if(c.MethodName=="Add"){pendingRecords.Add((LoanRecovery)c.Args[0]);return null;}
   var sql=(string)c.Args[0];var parameters=(object[])c.Args[2];
   if(sql.Contains("WITH (UPDLOCK"))return new[]{id};
   if(sql.Contains("WHERE RequestId")){var req=(Guid)((System.Data.SqlClient.SqlParameter)parameters[0]).Value;return records.Where(r=>r.RequestId==req).ToList();}
   if(sql.Contains("FROM dbo.swiftFin_LoanRecoveries"))return records;
   if(sql.Contains("SELECT l.CustomerId"))return new[]{link};
   if(sql.Contains("PostingPeriods"))return new[]{period};
   if(sql.Contains("SELECT g.Id"))return new[]{guarantee};
   if(sql.Contains("SUM(AmountGuaranteed)"))return new[]{0m};
   if(sql.Contains("CROSS APPLY")){
    var customer=(Guid)((System.Data.SqlClient.SqlParameter)parameters[0]).Value;
    if(customer==link.CustomerId)return new[]{new LoanRecoveryAccount{Id=sourceId,ChartOfAccountId=depositGl,Product="Deposits",Available=deposits},new LoanRecoveryAccount{Id=unusedSourceId,ChartOfAccountId=depositGl,Product="Other deposits",Available=500}};
    return new[]{new LoanRecoveryAccount{Id=guarantorSourceId,ChartOfAccountId=depositGl,Product="Deposits",Available=deposits}};
   }
   if(sql.Contains("SUM(e.Amount)")){var gl=(Guid)((System.Data.SqlClient.SqlParameter)parameters[1]).Value;return new[]{gl==link.PrincipalGlId?principal:interest};}
   throw new Exception(sql);
  });
  var jr=Stub<IRepository<Journal>>(c=>{Check(c.MethodName=="Add","Only stages financial journals");pendingJournals.Add((Journal)c.Args[0]);return null;});
  var service=new LoanRecoveryAppService(scopes,repo,jr,notice,age);var h=new ServiceHeader{ApplicationUserName="test"};
  var initial=service.Preview(id,h);Check(initial.TotalRecovery==0&&initial.Borrower.Accounts.All(a=>!a.Selected),"Opening drawer does not automatically select funds");
  var selection=new LoanRecoveryRequest{LoanCaseId=id,SelectedAccountIds=new List<Guid>{sourceId,guarantorSourceId}};
  var borrowerOnly=service.Preview(new LoanRecoveryRequest{LoanCaseId=id,SelectedAccountIds=new List<Guid>{sourceId}},h);
  Check(borrowerOnly.TotalRecovery==30&&borrowerOnly.Guarantors[0].Amount==0,"Only selected borrower account contributes");
  var guarantorOnly=service.Preview(new LoanRecoveryRequest{LoanCaseId=id,SelectedAccountIds=new List<Guid>{guarantorSourceId}},h);
  Check(guarantorOnly.TotalRecovery==30&&guarantorOnly.Borrower.Amount==0,"Only selected guarantor account contributes");
  Reject(()=>service.Preview(new LoanRecoveryRequest{LoanCaseId=id,SelectedAccountIds=new List<Guid>{Guid.NewGuid()}},h));
  var preview=service.Preview(selection,h);Check(preview.PrincipalDue==80&&preview.InterestDue==10&&preview.UnpostedInterest==10,"Unposted interest cannot be recovered");Check(commits==0&&posted.Count==0,"Preview is read-only");
  var input=new LoanRecoveryRequest{LoanCaseId=id,RequestId=Guid.NewGuid(),BasisHash=preview.BasisHash,SelectedAccountIds=selection.SelectedAccountIds};
  input.SelectedAccountIds=new List<Guid>{sourceId};Reject(()=>service.Post(input,h));Check(commits==0,"Changing selected accounts invalidates reviewed hash");
  input.SelectedAccountIds=new List<Guid>();Reject(()=>service.Post(input,h));
  input.SelectedAccountIds=selection.SelectedAccountIds;
  deposits=29;Reject(()=>service.Post(input,h));Check(commits==0&&posted.Count==0,"Reject stale balances without writes");deposits=30;
  eligible=false;Reject(()=>service.Post(input,h));eligible=true;
  fail=true;Reject(()=>service.Post(input,h));Check(commits==0&&records.Count==0&&posted.Count==0,"Failed commit leaves no recovery");fail=false;
  var receipt=service.Post(input,h);Check(commits==1&&receipt.Total==60&&records.Count==1,"One atomic recovery");
  Check(posted.SelectMany(j=>j.JournalEntries).All(e=>e.CustomerAccountId!=unusedSourceId),"Unselected account never debited despite larger balance");
  Check(posted.All(j=>j.JournalEntries.Count==2&&j.JournalEntries.Sum(e=>e.Amount)==0),"Every journal balanced");
  Check(posted.SelectMany(j=>j.JournalEntries).Where(e=>e.CustomerAccountId==loan.CustomerAccountId).Sum(e=>e.Amount)==-60,"Credit loan reduces debt");
  Check(posted.SelectMany(j=>j.JournalEntries).Where(e=>e.ChartOfAccountId==depositGl).Sum(e=>e.Amount)==60,"Debit deposits reduces liability");
  Check(service.Post(input,h).Id==receipt.Id&&commits==1,"Retry returns receipt without second debit");
  input.SelectedAccountIds=new List<Guid>{sourceId};Reject(()=>service.Post(input,h));input.SelectedAccountIds=selection.SelectedAccountIds;
  input.BasisHash="different";Reject(()=>service.Post(input,h));
  var next=service.Preview(id,h);Check(next.Guarantors[0].RemainingGuarantee==70,"Prior recovery reduces guarantee capacity");
  guarantee.AlreadyAttached=50;next=service.Preview(id,h);Check(next.Guarantors[0].RemainingGuarantee==20,"Legacy debt attachment also consumes guarantee");
 }
}
