using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Application.MainBoundedContext.AdministrationModule.Services;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanNoticeAgg;
using Domain.MainBoundedContext.MessagingModule.Aggregates.EmailAlertAgg;
using Domain.MainBoundedContext.MessagingModule.Aggregates.TextAlertAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
namespace Application.MainBoundedContext.BackOfficeModule.Services
{
 public class LoanNoticeAppService : ILoanNoticeAppService
 {
  readonly IDbContextScopeFactory scopes;
  readonly IRepository<LoanNotice> notices;
  readonly ILoanAgeingAppService ageing;
  readonly IRepository<EmailAlert> emails;
  readonly IRepository<TextAlert> texts;
  public LoanNoticeAppService(IDbContextScopeFactory scopes,IRepository<LoanNotice> notices,ILoanAgeingAppService ageing,IRepository<EmailAlert> emails,IRepository<TextAlert> texts)
  {this.scopes=scopes;this.notices=notices;this.ageing=ageing;this.emails=emails;this.texts=texts;}
  static void Check(bool ok,string message,int status=400){if(!ok)throw new LoanAgeingException("Notice",message,status);}
  static void Page(int page,int size){Check(page>=0&&page<100000&&size>=1&&size<=100,"Choose a valid page and a page size between 1 and 100.");}
  static void Date(DateTime asAt){Check(asAt.Year>=1753&&asAt.Date<=DateTime.UtcNow.Date,"Select an as-at date up to today.");}
  public static bool Matches(LoanAgeingLoanResult loan,DefaulterNoticeStageDTO stage)
  {
   return loan.OverduePrincipal.HasValue&&loan.OverdueInterest.HasValue&&loan.DaysPastDue.HasValue&&loan.Issues.Count==0
    &&loan.OverduePrincipal>=0&&loan.OverdueInterest>=0&&loan.DaysPastDue>=stage.DaysOverdue
    &&loan.OverduePrincipal+loan.OverdueInterest>0&&loan.OverduePrincipal+loan.OverdueInterest>=stage.MinimumArrears;
  }
  public static string DuplicateKey(Guid loan,Guid recipient,string type,DateTime asAt)
  {return loan.ToString("N")+":"+recipient.ToString("N")+":"+type+":"+asAt.ToString("yyyyMMdd",CultureInfo.InvariantCulture);}
  static string Hash(LoanNoticeCandidate c){using(var sha=SHA256.Create())return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(c)))).Replace("-","");}
  const string NameSql="CASE WHEN c.Type=0 THEN LTRIM(RTRIM(COALESCE(c.Individual_FirstName,'')+' '+COALESCE(c.Individual_LastName,''))) ELSE c.NonIndividual_Description END";
  static readonly string[] WorkflowStages={"FirstNotice","SecondNotice","FinalDemand"};
  public static string NextStage(IEnumerable<LoanNotice> history)
  {
   var sent=history.Where(n=>n.Status=="Sent"&&n.SentAtUtc.HasValue).ToList();
   foreach(var type in WorkflowStages){
    var group=sent.Where(n=>n.NoticeType==type).ToList();
    if(group.Count==0)return type;
    var first=group.OrderBy(n=>n.SentAtUtc).First();
    var required=StageRecipients(first);
    if(required==null||required.Count==0||required.Any(id=>!group.Any(n=>n.RecipientCustomerId==id)))return type;
   }
   return "Recovery";
  }
  static List<Guid> StageRecipients(LoanNotice notice)
  {
   return !string.IsNullOrWhiteSpace(notice.StageRecipientIdsJson)?JsonConvert.DeserializeObject<List<Guid>>(notice.StageRecipientIdsJson):JsonConvert.DeserializeObject<LoanNoticeCandidate>(notice.SnapshotJson)?.ExpectedRecipientIds;
  }
  LoanNoticeEligibility Candidates(DateTime asAt,ServiceHeader h,bool workflow=false)
  {
   Date(asAt);
   var report=ageing.GetNoticeLoanReport(asAt,h);
   var result=new LoanNoticeEligibility();result.Issues.AddRange(report.Issues);
   var contacts=notices.DatabaseSqlQuery<LoanNoticeContact>("SELECT l.Id LoanCaseId,c.Id CustomerId,"+NameSql+@" Name,b.CompanyId,co.Description CompanyName,co.DefaulterNoticePolicyJson PolicyJson,ISNULL(co.DefaulterNoticePolicyRevision,0) PolicyRevision FROM dbo.swiftFin_LoanCases l JOIN dbo.swiftFin_Customers c ON c.Id=l.CustomerId LEFT JOIN dbo.swiftFin_Branches b ON b.Id=l.BranchId LEFT JOIN dbo.swiftFin_Companies co ON co.Id=b.CompanyId WHERE l.Status IN (48829,48833)",h).ToDictionary(x=>x.LoanCaseId);
   var guarantors=notices.DatabaseSqlQuery<LoanNoticeContact>("SELECT g.LoanCaseId,c.Id CustomerId,"+NameSql+" Name FROM dbo.swiftFin_LoanGuarantors g JOIN dbo.swiftFin_Customers c ON c.Id=g.CustomerId WHERE g.Status=0 AND g.LoanCaseId IS NOT NULL",h).ToLookup(x=>x.LoanCaseId);
   var existing=notices.DatabaseSqlQuery<LoanNotice>("SELECT * FROM dbo.swiftFin_LoanNotices WHERE AsAt=@AsAt OR Status IN ('Draft','Ready','Approved','Sent','Queued','DeliveryFailed','DeliveryReview')",h,new SqlParameter("@AsAt",asAt.Date)).ToList();
   var policies=new Dictionary<Guid,DefaulterNoticePolicyDTO>();
   foreach(var loan in report.Loans)
   {
    if(!loan.OverduePrincipal.HasValue||!loan.OverdueInterest.HasValue||!loan.DaysPastDue.HasValue||loan.Issues.Count>0){result.ReviewCount++;result.Issues.Add("Loan "+loan.CaseNumber+": ageing requires review. "+string.Join(" ",loan.Issues));continue;}
    if(loan.OverduePrincipal+loan.OverdueInterest<=0||loan.DaysPastDue<=0)continue;
    LoanNoticeContact borrower;
    if(!contacts.TryGetValue(loan.LoanCaseId,out borrower)||!borrower.CompanyId.HasValue||string.IsNullOrWhiteSpace(borrower.CompanyName)||string.IsNullOrWhiteSpace(borrower.Name)){result.ReviewCount++;result.Issues.Add("Loan "+loan.CaseNumber+": borrower or branch-company link is missing.");continue;}
    DefaulterNoticePolicyDTO policy;
    try{if(!policies.TryGetValue(borrower.CompanyId.Value,out policy)){policy=CompanyAppService.ParseNoticePolicy(borrower.PolicyJson);policies.Add(borrower.CompanyId.Value,policy);}}
    catch(InvalidOperationException){result.ReviewCount++;result.Issues.Add("Loan "+loan.CaseNumber+": correct the company's notice settings before generating notices.");continue;}
    if(!policy.Enabled)result.DisabledCount++;
    var history=existing.Where(n=>n.LoanCaseId==loan.LoanCaseId).ToList();
    var next=NextStage(history);
    var nextIndex=Array.IndexOf(WorkflowStages,next);
    var previous=next=="Recovery"?"FinalDemand":nextIndex>0?WorkflowStages[nextIndex-1]:null;
    var deadlines=history.Where(n=>n.Status=="Sent"&&n.NoticeType==previous).Select(n=>(DateTime?)n.ResponseDeadline).ToList();
    var eligibleAfter=deadlines.Count>0?deadlines.Max():null;
    var stages=policy.Stages.Where(s=>s.NoticeType==next||(!workflow&&!WorkflowStages.Contains(s.NoticeType))).ToList();
    if(workflow&&stages.Count==0)stages.Add(new DefaulterNoticeStageDTO{NoticeType=next,Recipient="Borrower"});
    foreach(var stage in stages)
    {
     var recipients=new List<LoanNoticeContact>();
     if(stage.Recipient!="Guarantors")recipients.Add(borrower);
     if(stage.Recipient!="Borrower"){
      var attached=guarantors[loan.LoanCaseId].Where(x=>x.CustomerId!=borrower.CustomerId).GroupBy(x=>x.CustomerId).Select(g=>g.First()).ToList();
      if(attached.Count==0)result.Issues.Add("Loan "+loan.CaseNumber+", "+stage.NoticeType+": no currently attached guarantors found.");
      recipients.AddRange(attached);
     }
     var isWorkflow=WorkflowStages.Contains(stage.NoticeType)||stage.NoticeType=="Recovery";
     var frozen=history.Where(n=>n.NoticeType==stage.NoticeType&&n.Status!="Cancelled"&&(n.Status=="Sent"||n.MessageAlertId.HasValue)).OrderBy(n=>n.Status=="Sent"?0:1).ThenBy(n=>n.CreatedDate).FirstOrDefault();
     var expected=frozen==null?recipients.Select(r=>r.CustomerId).Distinct().ToList():StageRecipients(frozen);
     if(expected==null||expected.Count==0)expected=recipients.Select(r=>r.CustomerId).Distinct().ToList();
     var missingRecipients=expected.Where(id=>!recipients.Any(r=>r.CustomerId==id)).ToList();
     if(isWorkflow&&frozen!=null){
      recipients=recipients.Where(r=>expected.Contains(r.CustomerId)).ToList();
      foreach(var id in expected.Where(id=>!recipients.Any(r=>r.CustomerId==id)))
       recipients.Add(new LoanNoticeContact{CustomerId=id,Name=history.FirstOrDefault(n=>n.RecipientCustomerId==id)?.RecipientName??"Recipient requires review"});
     }
     var reason=!policy.Enabled?"Company notice policy is disabled.":
      stage.NoticeType!="Recovery"&&!policy.Stages.Any(s=>s.NoticeType==stage.NoticeType)?"Configure this notice stage in the company settings.":
      stage.NoticeType!="Recovery"&&!Matches(loan,stage)?"Days overdue or minimum arrears threshold has not been met.":null;
     if(isWorkflow&&stage.Recipient!="Borrower"&&!guarantors[loan.LoanCaseId].Any(g=>g.CustomerId!=borrower.CustomerId)&&frozen==null){
      var guarantorReason="No active guarantors attached.";
      reason=string.IsNullOrEmpty(reason)?guarantorReason:reason+" "+guarantorReason;
     }
     if(workflow&&recipients.Count==0)recipients.Add(borrower);
     foreach(var recipient in recipients)
     {
      if(string.IsNullOrWhiteSpace(recipient.Name)){result.Issues.Add("Loan "+loan.CaseNumber+": recipient name is missing.");continue;}
      var candidate=new LoanNoticeCandidate{LoanCaseId=loan.LoanCaseId,RecipientCustomerId=recipient.CustomerId,AsAt=asAt.Date,CaseNumber=loan.CaseNumber,BorrowerName=borrower.Name,RecipientName=recipient.Name,CompanyId=borrower.CompanyId.Value,CompanyName=borrower.CompanyName,PolicyRevision=borrower.PolicyRevision,PrincipalOverdue=loan.OverduePrincipal.Value,InterestOverdue=loan.OverdueInterest.Value,DaysOverdue=loan.DaysPastDue.Value,NoticeType=stage.NoticeType,Channel=stage.Channel,RequireApproval=stage.RequireApproval,Template=stage.Template,ResponseDays=stage.ResponseDays,PolicyJson=borrower.PolicyJson};
      candidate.WorkflowStage=isWorkflow?next:null;candidate.EligibleAfter=isWorkflow?eligibleAfter:null;
      candidate.ExpectedRecipientIds=expected;candidate.BlockingReason=reason;candidate.CanGenerate=reason==null;
      if(isWorkflow&&frozen!=null&&missingRecipients.Contains(recipient.CustomerId)){
       candidate.CanGenerate=false;candidate.BlockingReason="A recipient saved for this stage is no longer attached. Review the guarantors.";
      }
      candidate.BasisHash=Hash(candidate);
      // An existing active notice blocks another draft even when the operator changes the as-at date.
      var saved=existing.Where(n=>n.LoanCaseId==loan.LoanCaseId&&n.RecipientCustomerId==recipient.CustomerId&&n.NoticeType==stage.NoticeType&&n.Status!="Cancelled").OrderByDescending(n=>n.CreatedDate).FirstOrDefault();
      candidate.ExistingNoticeId=saved?.Id;
      if(saved?.Status=="Sent")continue;
      if(!workflow&&!candidate.CanGenerate)continue;
      result.Items.Add(candidate);
     }
    }
   }
   result.Total=result.Items.Count;return result;
  }
  public LoanNoticeEligibility Eligible(DateTime asAt,int pageIndex,int pageSize,ServiceHeader h,bool otherOnly=false)
  {
   Page(pageIndex,pageSize);using(scopes.CreateReadOnlyWithTransaction(IsolationLevel.Serializable)){
    var r=Candidates(asAt,h);if(otherOnly){r.Items=r.Items.Where(x=>x.WorkflowStage==null).ToList();r.Total=r.Items.Count;}r.Items=r.Items.OrderBy(x=>x.CaseNumber).ThenBy(x=>x.NoticeType).ThenBy(x=>x.RecipientCustomerId).Skip(pageIndex*pageSize).Take(pageSize).ToList();return r;
   }
  }
  public LoanNoticeEligibility Workflow(DateTime asAt,string stage,int pageIndex,int pageSize,ServiceHeader h)
  {
   Page(pageIndex,pageSize);Check(WorkflowStages.Contains(stage)||stage=="Recovery","Choose a valid notice stage.");
   using(scopes.CreateReadOnlyWithTransaction(IsolationLevel.Serializable)){
    var result=Candidates(asAt,h,true);var items=result.Items.Where(x=>x.WorkflowStage==stage).OrderBy(x=>x.CaseNumber).ThenBy(x=>x.RecipientCustomerId).ToList();
    result.Total=items.Count;result.Items=items.Skip(pageIndex*pageSize).Take(pageSize).ToList();return result;
   }
  }
  public LoanNoticeLoanPage LoanWorkflow(DateTime asAt,string stage,int pageIndex,int pageSize,ServiceHeader h)
  {
   Page(pageIndex,pageSize);Check(WorkflowStages.Contains(stage)||stage=="Recovery","Choose a valid notice stage.");
   using(scopes.CreateReadOnlyWithTransaction(IsolationLevel.Serializable)){
    var candidates=Candidates(asAt,h,true);
    var rows=candidates.Items.Where(x=>x.WorkflowStage==stage).GroupBy(x=>x.LoanCaseId).Select(g=>LoanRow(g.ToList(),h)).OrderBy(x=>x.CaseNumber).ToList();
    return new LoanNoticeLoanPage{Total=rows.Count,Items=rows.Skip(pageIndex*pageSize).Take(pageSize).ToList(),Issues=candidates.Issues.Distinct().ToList()};
   }
  }
  public LoanNoticeLoanRow RecoveryLoan(Guid loanCaseId,ServiceHeader h)
  {
   using(scopes.CreateReadOnly()){
    var candidates=Candidates(DateTime.UtcNow.Date,h,true).Items.Where(x=>x.LoanCaseId==loanCaseId&&x.WorkflowStage=="Recovery").ToList();
    Check(candidates.Count>0,"This loan is not in the recovery stage.",409);
    var row=LoanRow(candidates,h);Check(row.CanGenerate,"This loan is not eligible for recovery.",409);return row;
   }
  }
  LoanNoticeLoanRow LoanRow(List<LoanNoticeCandidate> candidates,ServiceHeader h)
  {
   var first=candidates.First();
   var saved=candidates.Where(c=>c.ExistingNoticeId.HasValue).Select(c=>Find(c.ExistingNoticeId.Value,h)).ToList();
   var expected=first.ExpectedRecipientIds.Distinct().Count();
   var statuses=saved.Select(n=>(n.Status=="Draft"||n.Status=="Ready")?(first.RequireApproval?"Draft":"Ready"):n.Status).ToList();
   var row=new LoanNoticeLoanRow{LoanCaseId=first.LoanCaseId,CaseNumber=first.CaseNumber,BorrowerName=first.BorrowerName,CompanyName=first.CompanyName,Stage=first.WorkflowStage,AsAt=first.AsAt,Channel=first.Channel,PrincipalOverdue=first.PrincipalOverdue,InterestOverdue=first.InterestOverdue,DaysPastDue=first.DaysOverdue,RecipientCount=expected,SentCount=Math.Max(0,expected-candidates.Count),QueuedCount=statuses.Count(x=>x=="Queued"),FailedCount=statuses.Count(x=>x=="DeliveryFailed"),RequireApproval=first.RequireApproval,CanGenerate=candidates.All(c=>c.CanGenerate),BlockingReason=string.Join(" ",candidates.Select(c=>c.BlockingReason).Where(x=>!string.IsNullOrWhiteSpace(x)).Distinct()),HasPrepared=saved.Count>0,CanApprove=saved.Any(n=>n.Status=="Draft")&&first.RequireApproval&&saved.Where(n=>n.Status=="Draft").All(n=>!string.Equals(n.CreatedBy,h.ApplicationUserName,StringComparison.OrdinalIgnoreCase))};
   row.NeedsPreparation=saved.Count<candidates.Count;
   row.Status=!row.CanGenerate?"Waiting":row.Stage=="Recovery"?"Eligible for recovery":statuses.Contains("DeliveryReview")?"Delivery review":row.FailedCount>0?"Delivery failed":row.QueuedCount>0?"Sending":statuses.Contains("Draft")?"Awaiting approval":row.NeedsPreparation?(first.RequireApproval?"Not prepared":"Ready"):"Ready";
   row.CanSend=row.CanGenerate&&row.Stage!="Recovery"&&!statuses.Any(x=>x=="Draft"||x=="DeliveryReview")&&(!first.RequireApproval||!row.NeedsPreparation);
   row.CanReset=row.HasPrepared&&saved.All(n=>n.Status=="Draft"||n.Status=="Ready"||n.Status=="Approved");
   using(var sha=SHA256.Create())row.BasisHash=BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(string.Join("|",candidates.OrderBy(c=>c.RecipientCustomerId).Select(c=>c.BasisHash))+"|"+string.Join("|",saved.OrderBy(n=>n.Id).Select(n=>n.Id+":"+n.Status))))).Replace("-","");
   return row;
  }
  public LoanNoticeLoanRow LoanStageAction(LoanNoticeLoanAction input,ServiceHeader h)
  {
   Check(input!=null&&input.LoanCaseId!=Guid.Empty&&WorkflowStages.Contains(input.Stage),"Choose a loan and notice stage.");
   Check(new[]{"prepare","approve","send","record-sent","reset"}.Contains(input.Action),"Choose a valid loan notice action.");
   Date(input.AsAt);
   using(var scope=scopes.CreateWithTransaction(IsolationLevel.Serializable)){
    var candidates=Candidates(input.AsAt,h,true).Items.Where(c=>c.LoanCaseId==input.LoanCaseId&&c.WorkflowStage==input.Stage).OrderBy(c=>c.RecipientCustomerId).ToList();
    Check(candidates.Count>0,"This loan has moved to another stage. Refresh the list.",409);
    var row=LoanRow(candidates,h);Check(row.BasisHash==input.BasisHash,"The loan or notice setup changed. Refresh the list.",409);
    if(input.Action=="reset"){
     Check(row.CanReset,"Notices already being sent cannot be reset.",409);
     foreach(var c in candidates.Where(c=>c.ExistingNoticeId.HasValue)){var n=Find(c.ExistingNoticeId.Value,h);n.Status="Cancelled";n.DuplicateKey="cancelled:"+n.Id.ToString("N");n.CancelledBy=h.ApplicationUserName;n.CancelledAtUtc=DateTime.UtcNow;}
     scope.SaveChanges(h);return row;
    }
    Check(row.CanGenerate,row.BlockingReason??"This loan is not eligible.",409);
    var saved=candidates.Where(c=>c.ExistingNoticeId.HasValue).ToDictionary(c=>c.RecipientCustomerId,c=>Find(c.ExistingNoticeId.Value,h));
    // Validate every pending recipient before creating or queuing any message.
    if(input.Action=="send"||input.Action=="record-sent"){
     var currentCandidates=Candidates(DateTime.UtcNow.Date,h,true).Items.Where(c=>c.LoanCaseId==input.LoanCaseId&&c.WorkflowStage==input.Stage).ToList();
     Check(currentCandidates.Count==candidates.Count&&currentCandidates.All(c=>c.CanGenerate)&&!candidates.Any(c=>!currentCandidates.Any(now=>now.RecipientCustomerId==c.RecipientCustomerId)),"This loan is no longer eligible. Refresh the list.",409);
     Check(row.CanSend,"Prepare and approve this loan's notices before sending.",409);
     Check(input.Action=="send"?row.Channel!="Print":row.Channel=="Print","Use the configured delivery channel.",409);
     foreach(var c in candidates){
      LoanNotice n;saved.TryGetValue(c.RecipientCustomerId,out n);
      if(n?.Status=="Queued")continue;
      Check(n==null||n.Channel==row.Channel,"The delivery channel changed. Prepare the notices again.",409);
      Check(n==null||n.ResponseDeadline>DateTime.UtcNow.Date,"The notices have expired. Prepare them again.",409);
      Check(n?.Status!="DeliveryFailed"||input.Retry,"Confirm retrying failed messages.",409);
      if(input.Action=="send"){
       var contact=DeliveryContact(n??new LoanNotice{LoanCaseId=c.LoanCaseId,RecipientCustomerId=c.RecipientCustomerId},h);
       try{Check(contact!=null&&contact.BranchId.HasValue,"Loan branch or contact is missing.");ValidateDestination(row.Channel,row.Channel=="Email"?contact.Email:contact.Mobile);}
       catch(LoanAgeingException e){throw new LoanAgeingException("Notice",c.RecipientName+": "+e.Message,409);}
      }
     }
    }
    if(input.Action=="approve"){
     Check(!row.NeedsPreparation&&row.CanApprove,"Another user must approve the prepared notices.",403);
    }
    if(input.Action=="record-sent")Check(!string.IsNullOrWhiteSpace(input.DispatchReference)&&input.DispatchReference.Trim().Length<=500,"Enter a dispatch reference.");
    foreach(var c in candidates){
     LoanNotice n;saved.TryGetValue(c.RecipientCustomerId,out n);
     if(n==null)n=CreateNotice(c,h);
     RefreshApprovalRequirement(n,h);
     if(input.Action=="approve"&&n.Status=="Draft"){Check(n.ResponseDeadline>=DateTime.UtcNow.Date,"The notices have expired. Prepare them again.",409);n.Status="Approved";n.ApprovedBy=h.ApplicationUserName;n.ApprovedAtUtc=DateTime.UtcNow;}
     else if(input.Action=="send"&&n.Status!="Queued"){
      var contact=DeliveryContact(n,h);
      Check(n.Status=="Ready"||n.Status=="Approved"||n.Status=="DeliveryFailed","The notice is not ready to send.",409);
      Check(n.ResponseDeadline>DateTime.UtcNow.Date,"The notices have expired. Prepare them again.",409);
      QueueElectronicNotice(n,contact,ValidateDestination(n.Channel,n.Channel=="Email"?contact?.Email:contact?.Mobile),c,h);
     }else if(input.Action=="record-sent"){Check(n.Status=="Ready"||n.Status=="Approved","The notice is not ready to dispatch.",409);Check(n.ResponseDeadline>DateTime.UtcNow.Date,"The notices have expired. Prepare them again.",409);n.StageRecipientIdsJson=JsonConvert.SerializeObject(c.ExpectedRecipientIds);n.Status="Sent";n.SentBy=h.ApplicationUserName;n.SentAtUtc=DateTime.UtcNow;n.DispatchReference=input.DispatchReference.Trim();}
    }
    scope.SaveChanges(h);return row;
   }
  }
  public string PrintableLoanStage(Guid loanCaseId,string stage,ServiceHeader h)
  {
   Check(WorkflowStages.Contains(stage),"Choose a notice stage.");
   using(scopes.CreateReadOnly()){
    var rows=notices.AllMatching(new DirectSpecification<LoanNotice>(n=>n.LoanCaseId==loanCaseId&&n.NoticeType==stage&&n.Status!="Cancelled"),h).ToList();
    Check(rows.Count>0,"Prepare the notices first.",409);
    return "<!doctype html><html><head><meta charset='utf-8'><title>Loan notices</title><style>section{break-after:page;page-break-after:always}section:last-child{break-after:auto;page-break-after:auto}body{font:12pt Arial;margin:30px}pre{font:inherit;white-space:pre-wrap}</style></head><body>"+string.Join("",rows.Select(n=>"<section><h1>"+WebUtility.HtmlEncode(n.CompanyName)+"</h1><p>Loan "+n.CaseNumber+" · "+WebUtility.HtmlEncode(n.RecipientName)+"</p>"+(n.Status=="Draft"?"<strong>DRAFT - NOT APPROVED</strong>":"")+"<pre>"+WebUtility.HtmlEncode(n.Body)+"</pre></section>"))+"</body></html>";
   }
  }

  public static string Render(LoanNoticeCandidate c,DateTime deadline)
  {
   var culture=CultureInfo.InvariantCulture;
   var tokens=new Dictionary<string,string>{{"companyName",c.CompanyName},{"borrowerName",c.BorrowerName},{"recipientName",c.RecipientName},{"loanNumber",c.CaseNumber.ToString(culture)},{"arrears",(c.PrincipalOverdue+c.InterestOverdue).ToString("N2",culture)},{"principalOverdue",c.PrincipalOverdue.ToString("N2",culture)},{"interestOverdue",c.InterestOverdue.ToString("N2",culture)},{"daysOverdue",c.DaysOverdue.ToString(culture)},{"asAt",c.AsAt.ToString("dd MMM yyyy",culture)},{"responseDeadline",deadline.ToString("dd MMM yyyy",culture)}};
   return Regex.Replace(c.Template,@"\{\{(.*?)\}\}",m=>{string value;Check(tokens.TryGetValue(m.Groups[1].Value,out value),"The template contains an unsupported detail. Correct the company settings.");return value;});
  }
  public LoanNoticeDTO Generate(LoanNoticeRequest input,ServiceHeader h)
  {
   Check(input!=null&&input.LoanCaseId!=Guid.Empty&&input.RecipientCustomerId!=Guid.Empty&&!string.IsNullOrWhiteSpace(input.NoticeType)&&!string.IsNullOrWhiteSpace(input.BasisHash),"Choose an eligible notice before generating a draft.");
   Date(input.AsAt);
   using(var scope=scopes.CreateWithTransaction(IsolationLevel.Serializable))
   {
    var duplicate=DuplicateKey(input.LoanCaseId,input.RecipientCustomerId,input.NoticeType,input.AsAt);
    var saved=notices.AllMatching(new DirectSpecification<LoanNotice>(x=>x.DuplicateKey==duplicate),h).FirstOrDefault();
    if(saved!=null)return Dto(saved);
    var c=Candidates(input.AsAt,h).Items.SingleOrDefault(x=>x.LoanCaseId==input.LoanCaseId&&x.RecipientCustomerId==input.RecipientCustomerId&&x.NoticeType==input.NoticeType);
    Check(c!=null,"This notice is no longer eligible. Refresh the eligible notices.",409);
    if(c.ExistingNoticeId.HasValue)return Dto(notices.Get(c.ExistingNoticeId.Value,h));
    Check(c.BasisHash==input.BasisHash,"The ageing, recipient or company policy changed. Refresh and review the notice before generating.",409);
    var n=CreateNotice(c,h);scope.SaveChanges(h);return Dto(n);
   }
  }
  LoanNotice CreateNotice(LoanNoticeCandidate c,ServiceHeader h)
  {
    var now=DateTime.UtcNow;var deadline=now.Date.AddDays(c.ResponseDays);
    var n=new LoanNotice{LoanCaseId=c.LoanCaseId,CompanyId=c.CompanyId,RecipientCustomerId=c.RecipientCustomerId,DuplicateKey=DuplicateKey(c.LoanCaseId,c.RecipientCustomerId,c.NoticeType,c.AsAt),CaseNumber=c.CaseNumber,PolicyRevision=c.PolicyRevision,AsAt=c.AsAt,ResponseDeadline=deadline,NoticeType=c.NoticeType,Channel=c.Channel,RecipientName=c.RecipientName,BorrowerName=c.BorrowerName,CompanyName=c.CompanyName,PrincipalOverdue=c.PrincipalOverdue,InterestOverdue=c.InterestOverdue,DaysOverdue=c.DaysOverdue,Body=Render(c,deadline),PolicyJson=c.PolicyJson,SnapshotJson=JsonConvert.SerializeObject(c),RequireApproval=c.RequireApproval,Status=c.RequireApproval?"Draft":"Ready",CreatedBy=h.ApplicationUserName,CreatedDate=now};
    n.GenerateNewIdentity();notices.Add(n,h);return n;
  }
  static LoanNoticeDTO Dto(LoanNotice n){return new LoanNoticeDTO{Id=n.Id,LoanCaseId=n.LoanCaseId,CaseNumber=n.CaseNumber,BorrowerName=n.BorrowerName,RecipientName=n.RecipientName,CompanyName=n.CompanyName,AsAt=n.AsAt,ResponseDeadline=n.ResponseDeadline,CreatedDate=n.CreatedDate,NoticeType=n.NoticeType,Channel=n.Channel,Status=n.Status,Body=n.Body,PolicyRevision=n.PolicyRevision,RequireApproval=n.RequireApproval,PrincipalOverdue=n.PrincipalOverdue,InterestOverdue=n.InterestOverdue,DaysOverdue=n.DaysOverdue,CreatedBy=n.CreatedBy,ApprovedBy=n.ApprovedBy,ApprovedAtUtc=n.ApprovedAtUtc,CancelledBy=n.CancelledBy,CancelledAtUtc=n.CancelledAtUtc,SentBy=n.SentBy,SentAtUtc=n.SentAtUtc,DispatchReference=n.DispatchReference,DeliveryAttemptsJson=n.DeliveryAttemptsJson,MessageAlertId=n.MessageAlertId,DeliveryDestination=n.DeliveryDestination,DeliveryStatus=n.DeliveryStatus,QueuedBy=n.QueuedBy,QueuedAtUtc=n.QueuedAtUtc};}
  public LoanNoticePage History(int pageIndex,int pageSize,ServiceHeader h)
  {
   Page(pageIndex,pageSize);using(scopes.CreateReadOnly())return new LoanNoticePage{Total=notices.DatabaseSqlQuery<int>("SELECT COUNT(*) FROM dbo.swiftFin_LoanNotices",h).Single(),Items=notices.DatabaseSqlQuery<LoanNotice>("SELECT * FROM dbo.swiftFin_LoanNotices ORDER BY CreatedDate DESC,Id OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY",h,new SqlParameter("@Skip",pageIndex*pageSize),new SqlParameter("@Take",pageSize)).Select(Dto).ToList()};
  }
  LoanNotice Find(Guid id,ServiceHeader h){var n=notices.Get(id,h);Check(n!=null,"The notice was not found.",404);return n;}
  public LoanNoticeDTO Get(Guid id,ServiceHeader h){using(scopes.CreateReadOnly()){
   var n=Find(id,h);var dto=Dto(n);
   if((n.Status=="Draft"||n.Status=="Ready"||n.Status=="Approved"||n.Status=="DeliveryFailed")&&(n.Channel=="Email"||n.Channel=="SMS")){
    var contact=DeliveryContact(n,h);dto.SendDestination=n.Channel=="Email"?contact?.Email?.Trim():contact?.Mobile?.Trim();
   }
   return dto;
  }}
  public static bool ApplyCurrentApproval(LoanNotice notice,DefaulterNoticePolicyDTO policy)
  {
   if(notice.Status!="Draft"&&notice.Status!="Ready")return false;
   var stage=policy.Stages.SingleOrDefault(x=>x.NoticeType==notice.NoticeType);
   if(stage==null||!policy.Enabled)return false;
   var status=stage.RequireApproval?"Draft":"Ready";
   if(notice.RequireApproval==stage.RequireApproval&&notice.Status==status)return false;
   notice.RequireApproval=stage.RequireApproval;notice.Status=status;return true;
  }
  bool RefreshApprovalRequirement(LoanNotice notice,ServiceHeader h)
  {
   if(notice.Status!="Draft"&&notice.Status!="Ready")return false;
   var company=notices.DatabaseSqlQuery<LoanNoticeContact>("SELECT DefaulterNoticePolicyJson PolicyJson FROM dbo.swiftFin_Companies WHERE Id=@Company",h,new SqlParameter("@Company",notice.CompanyId)).SingleOrDefault();
   Check(company!=null,"The notice company was not found.",409);
   DefaulterNoticePolicyDTO policy;
   try{policy=CompanyAppService.ParseNoticePolicy(company.PolicyJson);}catch(InvalidOperationException){throw new LoanAgeingException("Notice","Correct the company notice policy.",409);}
   return ApplyCurrentApproval(notice,policy);
  }
  public LoanNoticeDTO RefreshApproval(Guid id,ServiceHeader h)
  {
   using(var scope=scopes.CreateWithTransaction(IsolationLevel.Serializable)){
    var notice=Find(id,h);if(RefreshApprovalRequirement(notice,h))scope.SaveChanges(h);return Dto(notice);
   }
  }
  public LoanNoticeDTO Approve(Guid id,ServiceHeader h)
  {
   using(var scope=scopes.CreateWithTransaction(IsolationLevel.Serializable)){
    var n=Find(id,h);Check(n.Status=="Draft","Only a draft requiring approval can be approved.",409);
    Check(!string.Equals(n.CreatedBy,h.ApplicationUserName,StringComparison.OrdinalIgnoreCase),"Another user must approve this notice.",403);
    Check(n.ResponseDeadline>=DateTime.UtcNow.Date,"The response deadline has passed. Cancel this notice and generate a fresh draft.");
    n.Status="Approved";n.ApprovedBy=h.ApplicationUserName;n.ApprovedAtUtc=DateTime.UtcNow;scope.SaveChanges(h);return Dto(n);
   }
  }
  LoanNoticeDeliveryContact DeliveryContact(LoanNotice n,ServiceHeader h)
  {
   return notices.DatabaseSqlQuery<LoanNoticeDeliveryContact>("SELECT l.BranchId,c.Address_Email Email,c.Address_MobileLine Mobile FROM dbo.swiftFin_LoanCases l JOIN dbo.swiftFin_Customers c ON c.Id=@Recipient WHERE l.Id=@Loan",h,new SqlParameter("@Recipient",n.RecipientCustomerId),new SqlParameter("@Loan",n.LoanCaseId)).SingleOrDefault();
  }
  public static string ValidateDestination(string channel,string value)
  {
   value=value?.Trim();Check(!string.IsNullOrWhiteSpace(value)&&value.Length<=256,"The recipient needs a valid email address or mobile number in their customer record.");
   if(channel=="Email"){
    System.Net.Mail.MailAddress address=null;
    try{address=new System.Net.Mail.MailAddress(value);}catch(FormatException){}
    Check(address!=null&&address.Address==value&&!value.Contains("\r")&&!value.Contains("\n"),"Correct the recipient's email address in their customer record.");
   }else Check(channel=="SMS"&&Regex.IsMatch(value,@"^\+[0-9]{7,15}$"),"Correct the recipient's mobile number. Use international format beginning with +.");
   return value;
  }
  public LoanNoticeDTO Send(Guid id,LoanNoticeSendRequest input,ServiceHeader h)
  {
   // The alert and its notice link commit together. The existing dispatcher queueing jobs
   // pick up Pending alerts only after commit; no external send occurs inside this transaction.
   using(var scope=scopes.CreateWithTransaction(IsolationLevel.Serializable)){
    var n=Find(id,h);RefreshApprovalRequirement(n,h);Check(n.Channel=="Email"||n.Channel=="SMS","Use Record as sent for printed notices.",409);
    if(n.MessageAlertId.HasValue&&n.Status!="DeliveryFailed")return Dto(n);
    Check(n.Status!="DeliveryFailed"||input?.Retry==true,"Confirm a retry before resending a failed notice.",409);
    Check(n.Status=="Ready"||n.Status=="Approved"||n.Status=="DeliveryFailed","Only ready, approved or failed notices can be sent.",409);
    Check(n.ResponseDeadline>DateTime.UtcNow.Date,"The response deadline has passed or is today. Prepare a fresh notice before sending.",409);
    var candidate=Candidates(DateTime.UtcNow.Date,h).Items.SingleOrDefault(c=>c.LoanCaseId==n.LoanCaseId&&c.RecipientCustomerId==n.RecipientCustomerId&&c.NoticeType==n.NoticeType);
    Check(candidate!=null&&candidate.CanGenerate&&candidate.ExistingNoticeId==n.Id,"This notice is no longer eligible. Refresh and review the loan.",409);
    var contact=DeliveryContact(n,h);Check(contact!=null&&contact.BranchId.HasValue,"The loan branch or recipient contact could not be resolved.");
    var destination=ValidateDestination(n.Channel,n.Channel=="Email"?contact.Email:contact.Mobile);
    Check(input!=null&&input.Destination==destination,"The recipient contact changed. Reopen the notice and confirm the destination.",409);
    QueueElectronicNotice(n,contact,destination,candidate,h);
    scope.SaveChanges(h);return Dto(n);
   }
  }
  void QueueElectronicNotice(LoanNotice n,LoanNoticeDeliveryContact contact,string destination,LoanNoticeCandidate candidate,ServiceHeader h)
  {
    if(n.MessageAlertId.HasValue){
     var attempts=string.IsNullOrWhiteSpace(n.DeliveryAttemptsJson)?new List<object>():JsonConvert.DeserializeObject<List<object>>(n.DeliveryAttemptsJson);
     attempts.Add(new{n.MessageAlertId,n.DeliveryDestination,n.DeliveryStatus,n.QueuedBy,n.QueuedAtUtc});n.DeliveryAttemptsJson=JsonConvert.SerializeObject(attempts);
    }
    if(n.Channel=="Email"){
     var message=new MailMessage(null,destination,null,"Loan "+n.CaseNumber+" - "+n.NoticeType,n.Body,false,(int)DLRStatus.Pending,(int)MessageOrigin.Within,(int)QueuePriority.Normal,0,false,null);
     var alert=EmailAlertFactory.CreateEmailAlert(contact.BranchId,message);alert.CreatedBy=h.ApplicationUserName;emails.Add(alert,h);n.MessageAlertId=alert.Id;
    }else{
     var message=new TextMessage(destination,n.Body,(int)DLRStatus.Pending,null,(int)MessageOrigin.Within,(int)QueuePriority.Normal,0,false);
     var alert=TextAlertFactory.CreateTextAlert(contact.BranchId,message,h);texts.Add(alert,h);n.MessageAlertId=alert.Id;
    }
    n.StageRecipientIdsJson=JsonConvert.SerializeObject(candidate.ExpectedRecipientIds);
    n.Status="Queued";n.DeliveryStatus="Queued";n.DeliveryDestination=destination;n.QueuedBy=h.ApplicationUserName;n.QueuedAtUtc=DateTime.UtcNow;
  }
  // Called inside the messaging AppService transaction that saves the provider result.
  public static void ApplyDeliveryResult(IRepository<LoanNotice> repository,Guid alertId,string channel,int status,ServiceHeader h)
  {
   var notice=repository.AllMatching(new DirectSpecification<LoanNotice>(n=>n.MessageAlertId==alertId),h).SingleOrDefault();
   if(notice==null||notice.Channel!=channel)return;
   var accepted=status==(int)DLRStatus.Delivered||(channel=="SMS"&&status==(int)DLRStatus.Submitted);
   if(accepted){
    notice.Status="Sent";notice.DeliveryStatus=channel=="Email"?"Accepted by mail server":status==(int)DLRStatus.Delivered?"Delivered":"Accepted by SMS provider";
    notice.SentBy=notice.QueuedBy;if(!notice.SentAtUtc.HasValue)notice.SentAtUtc=DateTime.UtcNow;
    notice.DispatchReference=channel+" alert "+alertId;
   }else if(status==(int)DLRStatus.Failed){
    // A later SMS delivery failure must be visible, without erasing recorded submission.
    notice.DeliveryStatus="Failed";if(notice.Status!="Sent")notice.Status="DeliveryFailed";
   }
  }
  public LoanNoticeDTO RecordSent(Guid id,LoanNoticeDispatchRequest input,ServiceHeader h)
  {
   Check(input!=null&&!string.IsNullOrWhiteSpace(input.DispatchReference)&&input.DispatchReference.Trim().Length<=500,"Enter a dispatch reference or delivery note (up to 500 characters).");
   using(var scope=scopes.CreateWithTransaction(IsolationLevel.Serializable)){
    var n=Find(id,h);RefreshApprovalRequirement(n,h);Check(n.Channel=="Print","Email and SMS notices must be sent through their delivery channel.",409);if(n.Status=="Sent")return Dto(n);
    Check(n.Status=="Ready"||n.Status=="Approved","Only ready or approved notices can be recorded as sent.",409);
    Check(n.ResponseDeadline>DateTime.UtcNow.Date,"The response deadline has passed or is today. Prepare a fresh notice before dispatch.",409);
    var candidate=Candidates(DateTime.UtcNow.Date,h).Items.SingleOrDefault(c=>c.LoanCaseId==n.LoanCaseId&&c.RecipientCustomerId==n.RecipientCustomerId&&c.NoticeType==n.NoticeType);
    Check(candidate!=null&&candidate.CanGenerate&&candidate.ExistingNoticeId==n.Id,"This notice is no longer eligible for dispatch. Refresh the list and review the loan.",409);
    n.StageRecipientIdsJson=JsonConvert.SerializeObject(candidate.ExpectedRecipientIds);
    n.Status="Sent";n.SentBy=h.ApplicationUserName;n.SentAtUtc=DateTime.UtcNow;n.DispatchReference=input.DispatchReference.Trim();
    scope.SaveChanges(h);return Dto(n);
   }
  }
  public LoanNoticeDTO Cancel(Guid id,ServiceHeader h)
  {
   using(var scope=scopes.CreateWithTransaction(IsolationLevel.Serializable)){var n=Find(id,h);Check(n.Status=="Draft"||n.Status=="Ready"||n.Status=="Approved","Only unsent drafts, ready or approved notices can be cancelled.",409);n.Status="Cancelled";n.DuplicateKey="cancelled:"+n.Id.ToString("N");n.CancelledBy=h.ApplicationUserName;n.CancelledAtUtc=DateTime.UtcNow;scope.SaveChanges(h);return Dto(n);}
  }
  public static string PrintableDocument(LoanNoticeDTO n)
  {
   Func<string,string> e=WebUtility.HtmlEncode;
   var label=n.Status=="Sent"?"DISPATCH RECORDED":n.Status=="Draft"?"DRAFT - NOT APPROVED":n.Status=="Cancelled"?"CANCELLED":n.Channel=="Print"?"PRINT COPY - DELIVERY NOT RECORDED":"PREVIEW COPY - NOT SENT";
   return "<!doctype html><html><head><meta charset='utf-8'><meta name='viewport' content='width=device-width'><title>Loan notice "+n.CaseNumber+"</title><style>body{font:12pt Arial,sans-serif;line-height:1.5;margin:30px auto;max-width:760px;padding:24px;color:#111}pre{font:inherit;white-space:pre-wrap;overflow-wrap:anywhere}small{color:#555}@page{size:A4;margin:20mm}@media print{body{margin:0;padding:0}}</style></head><body><h1>"+e(n.CompanyName)+"</h1><p><strong>"+e(label)+"</strong></p><p>Notice: "+e(n.NoticeType)+" · Loan "+n.CaseNumber+"<br>To: "+e(n.RecipientName)+"<br>Prepared: "+n.CreatedDate.ToString("dd MMM yyyy",CultureInfo.InvariantCulture)+"<br>Arrears as at: "+n.AsAt.ToString("dd MMM yyyy",CultureInfo.InvariantCulture)+"</p><hr><pre>"+e(n.Body)+"</pre><hr><small>Reference: "+n.Id+" · Policy revision "+n.PolicyRevision+"</small></body></html>";
  }
  public string Printable(Guid id,ServiceHeader h){return PrintableDocument(Get(id,h));}
 }
}
