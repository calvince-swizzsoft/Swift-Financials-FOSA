using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanNoticeAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
static class LoanNoticeTests
{
 class Proxy:RealProxy{readonly Func<IMethodCallMessage,object> call;public Proxy(Type t,Func<IMethodCallMessage,object> f):base(t){call=f;}public override IMessage Invoke(IMessage msg){var c=(IMethodCallMessage)msg;try{return new ReturnMessage(call(c),null,0,c.LogicalCallContext,c);}catch(Exception e){return new ReturnMessage(e,c);}}}
 static T Stub<T>(Func<IMethodCallMessage,object> f){return (T)new Proxy(typeof(T),f).GetTransparentProxy();}
 static int checks;
 static void Check(bool ok,string label){if(!ok)throw new Exception(label);checks++;}
 static void Reject(Action action,int status){try{action();throw new Exception("Expected rejection");}catch(LoanAgeingException e){Check(e.Status==status,"Useful HTTP error");}}
 public static void Run()
 {
  var loan=new LoanAgeingLoanResult{LoanCaseId=Guid.NewGuid(),CaseNumber=123,OverduePrincipal=10000,OverdueInterest=2500,DaysPastDue=30};
  var report=new LoanAgeingResult{Loans=new List<LoanAgeingLoanResult>{loan},TotalLoans=1};
  var borrower=new LoanNoticeContact{LoanCaseId=loan.LoanCaseId,CustomerId=Guid.NewGuid(),Name="Borrower",CompanyId=Guid.NewGuid(),CompanyName="Company",PolicyRevision=1,
   PolicyJson="{\"enabled\":true,\"stages\":[{\"noticeType\":\"Reminder\",\"daysOverdue\":30,\"minimumArrears\":12500,\"recipient\":\"Both\",\"responseDays\":14,\"channel\":\"Print\",\"requireApproval\":true,\"template\":\"Dear {{recipientName}}, KSh {{arrears}} on {{loanNumber}}. Respond by {{responseDeadline}}.\"}]}"};
  var guarantor=new LoanNoticeContact{LoanCaseId=loan.LoanCaseId,CustomerId=Guid.NewGuid(),Name="Guarantor"};
  var rows=new List<LoanNotice>();int commits=0;
  var scope=Stub<IDbContextScope>(c=>{if(c.MethodName=="SaveChanges"){commits++;return 1;}return null;});
  var readScope=Stub<IDbContextReadOnlyScope>(c=>null);
  var scopes=Stub<IDbContextScopeFactory>(c=>c.MethodName.StartsWith("CreateReadOnly")?(object)readScope:scope);
  var age=Stub<ILoanAgeingAppService>(c=>{Check(c.MethodName=="GetNoticeLoanReport","Uses full per-loan ageing");return report;});
  var repo=Stub<IRepository<LoanNotice>>(c=>{
   switch(c.MethodName){
    case "DatabaseSqlQuery":var sql=(string)c.Args[0];if(sql.Contains("COUNT(*)"))return new[]{rows.Count};if(sql.Contains("swiftFin_LoanGuarantors"))return new[]{guarantor,guarantor};if(sql.Contains("swiftFin_LoanCases"))return new[]{borrower};return rows;
    case "AllMatching":return rows.Where(((ISpecification<LoanNotice>)c.Args[0]).SatisfiedBy().Compile()).ToList();
    case "Get":return rows.SingleOrDefault(x=>x.Id==(Guid)c.Args[0]);
    case "Add":rows.Add((LoanNotice)c.Args[0]);return null;
    default:throw new Exception("Unexpected repository method "+c.MethodName);
   }
  });
  var svc=new LoanNoticeAppService(scopes,repo,age);var h=new ServiceHeader{ApplicationUserName="maker"};var asAt=DateTime.UtcNow.Date.AddDays(-1);
  var eligible=svc.Eligible(asAt,0,20,h);Check(eligible.Total==2,"One borrower and distinct attached guarantor");
  var c1=eligible.Items.Single(x=>x.RecipientCustomerId==borrower.CustomerId);
  var req=new LoanNoticeRequest{LoanCaseId=c1.LoanCaseId,RecipientCustomerId=c1.RecipientCustomerId,AsAt=asAt,NoticeType=c1.NoticeType,BasisHash=c1.BasisHash};
  loan.OverdueInterest=2600;Reject(()=>svc.Generate(req,h),409);Check(rows.Count==0,"Changed ageing cannot save stale draft");loan.OverdueInterest=2500;
  var saved=svc.Generate(req,h);Check(rows.Count==1&&commits==1,"One persisted draft");Check(saved.Status=="Draft"&&saved.Body.Contains("12,500.00")&&!saved.Body.Contains("{{"),"Rendered and approval required");
  Check(saved.ResponseDeadline==DateTime.UtcNow.Date.AddDays(14),"Deadline starts at preparation not historical snapshot");
  Check(svc.Generate(req,h).Id==saved.Id&&rows.Count==1&&commits==1,"Double click is idempotent");
  req.AsAt=asAt.AddDays(1);Check(svc.Generate(req,h).Id==saved.Id,"Changing date does not duplicate active notice");
  Reject(()=>svc.Approve(saved.Id,h),403);Check(rows[0].Status=="Draft","Maker cannot self approve");
  h.ApplicationUserName="checker";Check(svc.Approve(saved.Id,h).Status=="Approved","Independent approval recorded");
  borrower.Name="Changed borrower";Check(svc.Get(saved.Id,h).BorrowerName=="Borrower","Saved snapshot is immutable");
  var html=LoanNoticeAppService.PrintableDocument(new LoanNoticeDTO{CompanyName="<script>x</script>",Body="<img src=x onerror=x>",Status="Draft",CreatedDate=asAt,AsAt=asAt});
  Check(!html.Contains("<script>")&&!html.Contains("<img")&&html.Contains("&lt;img")&&html.Contains("NOT APPROVED"),"Printable output escapes content and marks drafts");
  Check(svc.Cancel(saved.Id,h).Status=="Cancelled"&&svc.Get(saved.Id,h).CancelledBy=="checker","Cancellation preserves history and actor");
  loan.OverdueInterest=null;Check(svc.Eligible(asAt,0,20,h).Total==0,"Unknown interest excludes loan");loan.OverdueInterest=2500;
  loan.DaysPastDue=29;Check(svc.Eligible(asAt,0,20,h).Total==0,"Days threshold respected");loan.DaysPastDue=30;
  loan.OverduePrincipal=9999;Check(svc.Eligible(asAt,0,20,h).Total==0,"Arrears threshold respected");loan.OverduePrincipal=10000;
  borrower.PolicyJson=borrower.PolicyJson.Replace("true","false");Check(svc.Eligible(asAt,0,20,h).DisabledCount==1,"Disabled policy reported");
  Reject(()=>svc.Eligible(DateTime.UtcNow.Date.AddDays(1),0,20,h),400);
  Console.WriteLine("PASS: "+checks+" loan notice assertions.");
 }
}
