using Application.MainBoundedContext.DTO.AdministrationModule;
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
  var rows=new List<LoanNotice>();int commits=0;bool hasActiveGuarantors=true;
  var deliveryContact=new LoanNoticeDeliveryContact{BranchId=Guid.NewGuid(),Email="notice@example.test",Mobile="+254700000001"};
  var emailRows=new List<Domain.MainBoundedContext.MessagingModule.Aggregates.EmailAlertAgg.EmailAlert>();
  var textRows=new List<Domain.MainBoundedContext.MessagingModule.Aggregates.TextAlertAgg.TextAlert>();
  var emailRepo=Stub<IRepository<Domain.MainBoundedContext.MessagingModule.Aggregates.EmailAlertAgg.EmailAlert>>(c=>{if(c.MethodName=="Add"){emailRows.Add((Domain.MainBoundedContext.MessagingModule.Aggregates.EmailAlertAgg.EmailAlert)c.Args[0]);return null;}throw new Exception(c.MethodName);});
  var textRepo=Stub<IRepository<Domain.MainBoundedContext.MessagingModule.Aggregates.TextAlertAgg.TextAlert>>(c=>{if(c.MethodName=="Add"){textRows.Add((Domain.MainBoundedContext.MessagingModule.Aggregates.TextAlertAgg.TextAlert)c.Args[0]);return null;}throw new Exception(c.MethodName);});
  var scope=Stub<IDbContextScope>(c=>{if(c.MethodName=="SaveChanges"){commits++;return 1;}return null;});
  var readScope=Stub<IDbContextReadOnlyScope>(c=>null);
  var scopes=Stub<IDbContextScopeFactory>(c=>c.MethodName.StartsWith("CreateReadOnly")?(object)readScope:scope);
  var age=Stub<ILoanAgeingAppService>(c=>{Check(c.MethodName=="GetNoticeLoanReport","Uses full per-loan ageing");return report;});
  var repo=Stub<IRepository<LoanNotice>>(c=>{
   switch(c.MethodName){
    case "DatabaseSqlQuery":var sql=(string)c.Args[0];if(sql.Contains("FROM dbo.swiftFin_Companies"))return new[]{borrower};if(sql.Contains("Address_Email"))return new[]{deliveryContact};if(sql.Contains("COUNT(*)"))return new[]{rows.Count};if(sql.Contains("swiftFin_LoanGuarantors"))return hasActiveGuarantors?new[]{guarantor,guarantor}:new LoanNoticeContact[0];if(sql.Contains("swiftFin_LoanCases"))return new[]{borrower};return rows;
    case "AllMatching":return rows.Where(((ISpecification<LoanNotice>)c.Args[0]).SatisfiedBy().Compile()).ToList();
    case "Get":return rows.SingleOrDefault(x=>x.Id==(Guid)c.Args[0]);
    case "Add":rows.Add((LoanNotice)c.Args[0]);return null;
    default:throw new Exception("Unexpected repository method "+c.MethodName);
   }
  });
  var svc=new LoanNoticeAppService(scopes,repo,age,emailRepo,textRepo);var h=new ServiceHeader{ApplicationUserName="maker"};var asAt=DateTime.UtcNow.Date.AddDays(-1);
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
  var approvalPolicy=new DefaulterNoticePolicyDTO{Enabled=true,Stages=new List<DefaulterNoticeStageDTO>{new DefaulterNoticeStageDTO{NoticeType="SecondNotice",RequireApproval=false}}};
  var approvalDraft=new LoanNotice{NoticeType="SecondNotice",Status="Draft",RequireApproval=true,Body="Original message",PolicyRevision=9,SnapshotJson="original"};
  Check(LoanNoticeAppService.ApplyCurrentApproval(approvalDraft,approvalPolicy)&&approvalDraft.Status=="Ready"&&!approvalDraft.RequireApproval,"Existing draft adopts approval-off setting");
  Check(approvalDraft.Body=="Original message"&&approvalDraft.PolicyRevision==9&&approvalDraft.SnapshotJson=="original","Refreshing approval preserves original message and preparation snapshot");
  Check(!LoanNoticeAppService.ApplyCurrentApproval(approvalDraft,approvalPolicy),"Approval refresh is idempotent");
  approvalPolicy.Stages[0].RequireApproval=true;
  Check(LoanNoticeAppService.ApplyCurrentApproval(approvalDraft,approvalPolicy)&&approvalDraft.Status=="Draft","Approval-on setting restores approval requirement for ready notices");
  foreach(var terminalStatus in new[]{"Approved","Sent","Queued","Cancelled","DeliveryFailed"}){approvalDraft.Status=terminalStatus;approvalPolicy.Stages[0].RequireApproval=false;Check(!LoanNoticeAppService.ApplyCurrentApproval(approvalDraft,approvalPolicy)&&approvalDraft.Status==terminalStatus,"Approval refresh preserves "+terminalStatus);}
  var html=LoanNoticeAppService.PrintableDocument(new LoanNoticeDTO{CompanyName="<script>x</script>",Body="<img src=x onerror=x>",Status="Draft",CreatedDate=asAt,AsAt=asAt});
  Check(!html.Contains("<script>")&&!html.Contains("<img")&&html.Contains("&lt;img")&&html.Contains("NOT APPROVED"),"Printable output escapes content and marks drafts");
  Check(svc.Cancel(saved.Id,h).Status=="Cancelled"&&svc.Get(saved.Id,h).CancelledBy=="checker","Cancellation preserves history and actor");
  loan.OverdueInterest=null;Check(svc.Eligible(asAt,0,20,h).Total==0,"Unknown interest excludes loan");loan.OverdueInterest=2500;
  loan.DaysPastDue=29;Check(svc.Eligible(asAt,0,20,h).Total==0,"Days threshold respected");loan.DaysPastDue=30;
  loan.OverduePrincipal=9999;Check(svc.Eligible(asAt,0,20,h).Total==0,"Arrears threshold respected");loan.OverduePrincipal=10000;
  borrower.PolicyJson=borrower.PolicyJson.Replace("true","false");Check(svc.Eligible(asAt,0,20,h).DisabledCount==1,"Disabled policy reported");
  // The workflow is driven by recorded dispatch, never by draft preparation or approval.
  rows.Clear();borrower.Name="Borrower";
  borrower.PolicyJson="{\"enabled\":true,\"stages\":["+
   "{\"noticeType\":\"FirstNotice\",\"daysOverdue\":30,\"minimumArrears\":12500,\"recipient\":\"Both\",\"responseDays\":14,\"channel\":\"Print\",\"requireApproval\":true,\"template\":\"First notice {{recipientName}}\"},"+
   "{\"noticeType\":\"SecondNotice\",\"daysOverdue\":40,\"minimumArrears\":12500,\"recipient\":\"Borrower\",\"responseDays\":14,\"channel\":\"Print\",\"requireApproval\":false,\"template\":\"Second notice\"},"+
   "{\"noticeType\":\"FinalDemand\",\"daysOverdue\":60,\"minimumArrears\":12500,\"recipient\":\"Borrower\",\"responseDays\":14,\"channel\":\"Print\",\"requireApproval\":false,\"template\":\"Third notice\"}]}";
  var today=DateTime.UtcNow.Date;
  Func<string,LoanNoticeEligibility> queue=stage=>svc.Workflow(today,stage,0,20,h);
  Func<LoanNoticeCandidate,LoanNoticeDTO> generate=c=>svc.Generate(new LoanNoticeRequest{LoanCaseId=c.LoanCaseId,RecipientCustomerId=c.RecipientCustomerId,AsAt=c.AsAt,NoticeType=c.NoticeType,BasisHash=c.BasisHash},h);
  var dispatch=new LoanNoticeDispatchRequest{DispatchReference="Test dispatch record"};
  Check(queue("FirstNotice").Total==2&&queue("SecondNotice").Total==0&&queue("FinalDemand").Total==0&&queue("Recovery").Total==0,"Only current stage is populated");
  Check(svc.Eligible(today,0,20,h,true).Total==0,"Other notices excludes workflow stages before pagination");
  loan.DaysPastDue=29;Check(queue("FirstNotice").Items.All(x=>!x.CanGenerate),"Below-threshold defaulters stay visible but blocked");loan.DaysPastDue=90;
  var firstRows=queue("FirstNotice").Items;
  h.ApplicationUserName="maker";var first=generate(firstRows[0]);
  Reject(()=>svc.RecordSent(first.Id,dispatch,h),409);
  Check(queue("FirstNotice").Total==2,"Preparing a draft does not advance the loan");
  var unsentRecipientPolicy=borrower.PolicyJson;
  borrower.PolicyJson=borrower.PolicyJson.Replace("\"recipient\":\"Both\"","\"recipient\":\"Guarantors\"");
  Check(queue("FirstNotice").Total==1&&queue("FirstNotice").Items.Single().RecipientCustomerId==guarantor.CustomerId,"Unsent drafts follow updated guarantors-only policy");
  borrower.PolicyJson=unsentRecipientPolicy;
  Check(queue("FirstNotice").Total==2&&queue("FirstNotice").Items.All(c=>c.ExpectedRecipientIds.Count==2),"Changing back to Both adds borrower before dispatch without cancelling the draft");
  h.ApplicationUserName="checker";svc.Approve(first.Id,h);
  Check(queue("FirstNotice").Total==2,"Approval does not advance the loan");
  Reject(()=>svc.RecordSent(first.Id,new LoanNoticeDispatchRequest(),h),400);
  var sent=svc.RecordSent(first.Id,dispatch,h);
  Check(sent.Status=="Sent"&&sent.SentBy=="checker"&&sent.SentAtUtc.HasValue&&sent.DispatchReference==dispatch.DispatchReference,"Dispatch actor, time and reference persist");
  var sentCommits=commits;svc.RecordSent(first.Id,dispatch,h);Check(commits==sentCommits,"Repeated dispatch recording is idempotent");
  Reject(()=>svc.Cancel(first.Id,h),409);
  Check(queue("FirstNotice").Total==1&&queue("SecondNotice").Total==0,"Sending one recipient removes only that notice; remaining recipients prevent escalation");
  h.ApplicationUserName="maker";var firstGuarantor=generate(queue("FirstNotice").Items.Single());h.ApplicationUserName="checker";svc.Approve(firstGuarantor.Id,h);svc.RecordSent(firstGuarantor.Id,dispatch,h);
  Check(queue("FirstNotice").Total==0&&queue("SecondNotice").Total==1,"All first notices sent moves the loan exclusively to second");
  Check(queue("SecondNotice").Items.Single().CanGenerate&&queue("SecondNotice").Items.Single().EligibleAfter>today,"Second notice is eligible before the previous response deadline");

  var originalPolicy=borrower.PolicyJson;
  borrower.PolicyJson=borrower.PolicyJson.Replace("\"recipient\":\"Borrower\"","\"recipient\":\"Both\"");hasActiveGuarantors=false;
  var releasedCandidate=queue("SecondNotice").Items.Single();
  Check(!releasedCandidate.CanGenerate&&releasedCandidate.BlockingReason.Contains("No active guarantors attached.")&&!releasedCandidate.BlockingReason.Contains("response period"),"Inactive guarantors still block early notice generation");
  borrower.PolicyJson=originalPolicy;hasActiveGuarantors=true;

  loan.DaysPastDue=35;Check(!queue("SecondNotice").Items.Single().CanGenerate,"Next stage still enforces its overdue threshold");loan.DaysPastDue=90;
  var second=generate(queue("SecondNotice").Items.Single());Check(second.Status=="Ready","No-approval policy creates ready notice");
  rows.Single(n=>n.Id==second.Id).ResponseDeadline=today;Reject(()=>svc.RecordSent(second.Id,dispatch,h),409);rows.Single(n=>n.Id==second.Id).ResponseDeadline=today.AddDays(14);
  svc.RecordSent(second.Id,dispatch,h);
  Check(queue("SecondNotice").Total==0&&queue("FinalDemand").Total==1&&queue("Recovery").Total==0,"Second dispatch advances only to third");
  Check(queue("FinalDemand").Items.Single().CanGenerate&&queue("FinalDemand").Items.Single().EligibleAfter>today,"Third notice is eligible before the second response deadline");
  var third=generate(queue("FinalDemand").Items.Single());svc.RecordSent(third.Id,dispatch,h);
  Check(queue("FinalDemand").Total==0&&queue("Recovery").Total==1&&queue("Recovery").Items.Single().CanGenerate&&queue("Recovery").Items.Single().EligibleAfter>today,"Third dispatch permits recovery before the final response deadline");
  rows.Single(n=>n.Id==third.Id).ResponseDeadline=today;Check(queue("Recovery").Items.Single().CanGenerate,"Recovery is eligible on the deadline date");
  rows.Single(n=>n.Id==third.Id).ResponseDeadline=today.AddDays(-1);Check(queue("Recovery").Items.Single().CanGenerate,"Recovery remains eligible after final response deadline");
  Check(svc.Workflow(today,"Recovery",1,1,h).Total==1&&svc.Workflow(today,"Recovery",1,1,h).Items.Count==0,"Stage filtering occurs before pagination");
  loan.OverduePrincipal=0;loan.OverdueInterest=0;Check(queue("Recovery").Total==0,"Repaid loans disappear from recovery");
  loan.OverduePrincipal=10000;loan.OverdueInterest=null;Check(queue("Recovery").Total==0&&queue("Recovery").ReviewCount==1,"Unresolved ageing cannot qualify for recovery");loan.OverdueInterest=2500;
  Check(LoanNoticeAppService.PrintableDocument(sent).Contains("DISPATCH RECORDED"),"Sent print copies reflect recorded dispatch");
  // Pre-workflow drafts have no recipient list in their immutable snapshot.
  rows.Clear();
  h.ApplicationUserName="maker";var legacy=generate(queue("FirstNotice").Items.First());
  rows[0].SnapshotJson="{}";h.ApplicationUserName="checker";svc.Approve(legacy.Id,h);svc.RecordSent(legacy.Id,dispatch,h);
  Check(rows[0].SnapshotJson=="{}"&&rows[0].StageRecipientIdsJson.Contains(guarantor.CustomerId.ToString()),"Legacy dispatch captures recipient requirements without rewriting the saved snapshot");
  Check(queue("FirstNotice").Total==1&&queue("SecondNotice").Total==0,"Legacy first dispatch cannot skip remaining recipients");
  h.ApplicationUserName="maker";var legacyOther=generate(queue("FirstNotice").Items.Single());h.ApplicationUserName="checker";svc.Approve(legacyOther.Id,h);svc.RecordSent(legacyOther.Id,dispatch,h);
  Check(queue("FirstNotice").Total==0&&queue("SecondNotice").Total==1,"Legacy drafts can complete the first stage");
  rows.Clear();borrower.PolicyJson="{\"enabled\":false,\"stages\":[]}";Check(queue("FirstNotice").Total==1&&!queue("FirstNotice").Items.Single().CanGenerate,"Missing or disabled policy remains visible in first notice queue");
  Reject(()=>svc.Workflow(today,"Invalid",0,20,h),400);
  Reject(()=>svc.Eligible(DateTime.UtcNow.Date.AddDays(1),0,20,h),400);
  foreach(var channel in new[]{"Email","SMS"}){
   rows.Clear();emailRows.Clear();textRows.Clear();
   borrower.PolicyJson="{\"enabled\":true,\"stages\":[{\"noticeType\":\"FirstNotice\",\"daysOverdue\":1,\"minimumArrears\":1,\"recipient\":\"Borrower\",\"responseDays\":14,\"channel\":\""+channel+"\",\"requireApproval\":false,\"template\":\"Saved notice {{loanNumber}}\"}]}";
   var notice=generate(queue("FirstNotice").Items.Single());
   var preview=svc.Get(notice.Id,h);var destination=channel=="Email"?deliveryContact.Email:deliveryContact.Mobile;
   Check(preview.SendDestination==destination,"Preview resolves actual recipient contact for "+channel);
   Reject(()=>svc.RecordSent(notice.Id,dispatch,h),409);
   Reject(()=>svc.Send(notice.Id,new LoanNoticeSendRequest{Destination="changed"},h),409);
   Check(emailRows.Count+textRows.Count==0,"Stale destination cannot queue a message");
   var beforeSend=commits;
   var queued=svc.Send(notice.Id,new LoanNoticeSendRequest{Destination=destination},h);
   Check(queued.Status=="Queued"&&queued.MessageAlertId.HasValue&&queued.DeliveryDestination==destination&&commits==beforeSend+1,"Notice and message queue in one commit");
   Check(emailRows.Count+textRows.Count==1&&queue("FirstNotice").Total==1&&queue("SecondNotice").Total==0,"Queued notice does not advance");
   if(channel=="Email")Check(emailRows[0].MailMessage.Body==notice.Body&&emailRows[0].MailMessage.To==destination&&emailRows[0].MailMessage.DLRStatus==(int)DLRStatus.Pending,"Email uses saved text and actual recipient");
   else Check(textRows[0].TextMessage.Body==notice.Body&&textRows[0].TextMessage.Recipient==destination&&textRows[0].TextMessage.DLRStatus==(int)DLRStatus.Pending,"SMS uses saved text and actual recipient");
   svc.Send(notice.Id,new LoanNoticeSendRequest{Destination=destination},h);Check(emailRows.Count+textRows.Count==1&&commits==beforeSend+1,"Repeated send cannot create duplicate alerts");
   Reject(()=>svc.Cancel(notice.Id,h),409);
   LoanNoticeAppService.ApplyDeliveryResult(repo,queued.MessageAlertId.Value,channel,(int)DLRStatus.Failed,h);
   Check(svc.Get(notice.Id,h).Status=="DeliveryFailed"&&queue("FirstNotice").Total==1,"Failure remains at current stage");
   Reject(()=>svc.Send(notice.Id,new LoanNoticeSendRequest{Destination=destination},h),409);
   var retry=svc.Send(notice.Id,new LoanNoticeSendRequest{Destination=destination,Retry=true},h);
   Check(retry.MessageAlertId!=queued.MessageAlertId&&retry.DeliveryAttemptsJson.Contains(queued.MessageAlertId.ToString()),"Explicit retry preserves failed attempt and creates a distinct message");
   LoanNoticeAppService.ApplyDeliveryResult(repo,queued.MessageAlertId.Value,channel,(int)DLRStatus.Failed,h);
   Check(svc.Get(notice.Id,h).Status=="Queued","Old attempt cannot overwrite current retry status");
   LoanNoticeAppService.ApplyDeliveryResult(repo,retry.MessageAlertId.Value,channel,(int)DLRStatus.Pending,h);
   Check(queue("SecondNotice").Total==0,"Pending provider result cannot advance workflow");
   LoanNoticeAppService.ApplyDeliveryResult(repo,retry.MessageAlertId.Value,channel,channel=="Email"?(int)DLRStatus.Delivered:(int)DLRStatus.Submitted,h);
   var accepted=svc.Get(notice.Id,h);
   Check(accepted.Status=="Sent"&&accepted.SentAtUtc.HasValue&&accepted.SentBy==h.ApplicationUserName,"Provider acceptance records dispatch");
   Check(queue("FirstNotice").Total==0&&queue("SecondNotice").Total==1,"Accepted message advances exactly one stage");
   svc.Send(notice.Id,new LoanNoticeSendRequest{Destination=destination},h);Check(emailRows.Count+textRows.Count==2,"Sent notice cannot be sent again");
  }
  Reject(()=>LoanNoticeAppService.ValidateDestination("Email","two@example.test,other@example.test"),400);
  Reject(()=>LoanNoticeAppService.ValidateDestination("SMS","0700000001"),400);
  Reject(()=>LoanNoticeAppService.ValidateDestination("SMS",null),400);
  rows.Clear();emailRows.Clear();textRows.Clear();h.ApplicationUserName="maker";hasActiveGuarantors=true;
  loan.OverduePrincipal=10000;loan.OverdueInterest=2500;loan.DaysPastDue=90;
  borrower.PolicyJson="{\"enabled\":true,\"stages\":[{\"noticeType\":\"FirstNotice\",\"daysOverdue\":1,\"minimumArrears\":1,\"recipient\":\"Both\",\"responseDays\":14,\"channel\":\"Email\",\"requireApproval\":false,\"template\":\"Notice {{recipientName}}\"}]}";
  Func<LoanNoticeLoanRow> loanRow=()=>svc.LoanWorkflow(today,"FirstNotice",0,20,h).Items.Single();
  Func<string,bool,LoanNoticeLoanAction> batchRequest=(action,retryFlag)=>new LoanNoticeLoanAction{LoanCaseId=loan.LoanCaseId,Stage="FirstNotice",AsAt=today,Action=action,BasisHash=loanRow().BasisHash,Retry=retryFlag,DispatchReference="All printed notices dispatched"};
  var grouped=svc.LoanWorkflow(today,"FirstNotice",0,1,h);
  Check(grouped.Total==1&&grouped.Items.Count==1&&grouped.Items[0].RecipientCount==2,"Borrower and guarantor are one loan row");
  Check(svc.LoanWorkflow(today,"FirstNotice",1,1,h).Items.Count==0,"Loan grouping happens before pagination");
  deliveryContact.Email=null;Reject(()=>svc.LoanStageAction(batchRequest("send",false),h),409);
  Check(rows.Count==0&&emailRows.Count==0,"Missing destination prevents all notice and queue writes");deliveryContact.Email="notice@example.test";
  var beforeBatch=commits;svc.LoanStageAction(batchRequest("send",false),h);
  Check(rows.Count==2&&emailRows.Count==2&&rows.All(n=>n.Status=="Queued")&&commits==beforeBatch+1,"One send queues every configured recipient in exactly one commit");
  Check(loanRow().QueuedCount==2&&loanRow().RecipientCount==2,"Queued messages are aggregated under the loan");
  svc.LoanStageAction(batchRequest("send",false),h);Check(emailRows.Count==2,"Repeated loan send never duplicates queued messages");
  var firstMessage=rows[0].MessageAlertId.Value;var secondMessage=rows[1].MessageAlertId.Value;
  LoanNoticeAppService.ApplyDeliveryResult(repo,firstMessage,"Email",(int)DLRStatus.Delivered,h);
  LoanNoticeAppService.ApplyDeliveryResult(repo,secondMessage,"Email",(int)DLRStatus.Failed,h);
  Check(loanRow().SentCount==1&&loanRow().FailedCount==1,"Partial delivery stays one loan with correct counts");
  Reject(()=>svc.LoanStageAction(batchRequest("send",false),h),409);
  svc.LoanStageAction(batchRequest("send",true),h);Check(emailRows.Count==3&&rows.Count==2,"Loan retry queues only failed recipients");
  LoanNoticeAppService.ApplyDeliveryResult(repo,rows[1].MessageAlertId.Value,"Email",(int)DLRStatus.Delivered,h);
  Check(svc.LoanWorkflow(today,"FirstNotice",0,20,h).Total==0&&svc.LoanWorkflow(today,"SecondNotice",0,20,h).Total==1,"Loan advances once all configured recipients are accepted");
  rows.Clear();emailRows.Clear();borrower.PolicyJson=borrower.PolicyJson.Replace("\"requireApproval\":false","\"requireApproval\":true");
  Reject(()=>svc.LoanStageAction(batchRequest("send",false),h),409);svc.LoanStageAction(batchRequest("prepare",false),h);
  Check(rows.Count==2&&rows.All(n=>n.Status=="Draft")&&emailRows.Count==0,"Loan preparation creates all required approval drafts without sending");
  Reject(()=>svc.LoanStageAction(batchRequest("approve",false),h),403);
  h.ApplicationUserName="checker";svc.LoanStageAction(batchRequest("approve",false),h);Check(rows.All(n=>n.Status=="Approved"),"Independent checker approves the whole loan stage");
  svc.LoanStageAction(batchRequest("send",false),h);Check(emailRows.Count==2,"Approved loan stage sends every required notice");
  rows.Clear();emailRows.Clear();h.ApplicationUserName="maker";borrower.PolicyJson=borrower.PolicyJson.Replace("\"requireApproval\":true","\"requireApproval\":false").Replace("\"channel\":\"Email\"","\"channel\":\"Print\"");
  svc.LoanStageAction(batchRequest("prepare",false),h);Check(svc.PrintableLoanStage(loan.LoanCaseId,"FirstNotice",h).Contains("Borrower")&&svc.PrintableLoanStage(loan.LoanCaseId,"FirstNotice",h).Contains("Guarantor"),"One print download contains every recipient");
  svc.LoanStageAction(batchRequest("reset",false),h);Check(rows.All(n=>n.Status=="Cancelled")&&loanRow().NeedsPreparation,"Reset retains cancelled history and permits fresh preparation");
  svc.LoanStageAction(batchRequest("prepare",false),h);Check(rows.Count==4&&rows.Count(n=>n.Status=="Ready")==2,"Reset permits same-date replacement without duplicate-key conflict");
  svc.LoanStageAction(batchRequest("record-sent",false),h);Check(rows.Count(n=>n.Status=="Sent")==2,"Printed dispatch is recorded for all recipients together");
  Console.WriteLine("PASS: "+checks+" loan notice assertions.");
 }
}
