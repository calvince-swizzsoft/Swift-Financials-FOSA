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
  public LoanNoticeAppService(IDbContextScopeFactory scopes,IRepository<LoanNotice> notices,ILoanAgeingAppService ageing)
  {this.scopes=scopes;this.notices=notices;this.ageing=ageing;}
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
  LoanNoticeEligibility Candidates(DateTime asAt,ServiceHeader h)
  {
   Date(asAt);
   var report=ageing.GetNoticeLoanReport(asAt,h);
   var result=new LoanNoticeEligibility();result.Issues.AddRange(report.Issues);
   var contacts=notices.DatabaseSqlQuery<LoanNoticeContact>("SELECT l.Id LoanCaseId,c.Id CustomerId,"+NameSql+@" Name,b.CompanyId,co.Description CompanyName,co.DefaulterNoticePolicyJson PolicyJson,ISNULL(co.DefaulterNoticePolicyRevision,0) PolicyRevision FROM dbo.swiftFin_LoanCases l JOIN dbo.swiftFin_Customers c ON c.Id=l.CustomerId LEFT JOIN dbo.swiftFin_Branches b ON b.Id=l.BranchId LEFT JOIN dbo.swiftFin_Companies co ON co.Id=b.CompanyId WHERE l.Status IN (48829,48833)",h).ToDictionary(x=>x.LoanCaseId);
   var guarantors=notices.DatabaseSqlQuery<LoanNoticeContact>("SELECT g.LoanCaseId,c.Id CustomerId,"+NameSql+" Name FROM dbo.swiftFin_LoanGuarantors g JOIN dbo.swiftFin_Customers c ON c.Id=g.CustomerId WHERE g.Status=0 AND g.LoanCaseId IS NOT NULL",h).ToLookup(x=>x.LoanCaseId);
   var existing=notices.DatabaseSqlQuery<LoanNotice>("SELECT * FROM dbo.swiftFin_LoanNotices WHERE AsAt=@AsAt OR Status IN ('Draft','Ready','Approved')",h,new SqlParameter("@AsAt",asAt.Date)).ToList();
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
    if(!policy.Enabled){result.DisabledCount++;continue;}
    foreach(var stage in policy.Stages.Where(s=>Matches(loan,s)))
    {
     var recipients=new List<LoanNoticeContact>();
     if(stage.Recipient!="Guarantors")recipients.Add(borrower);
     if(stage.Recipient!="Borrower"){
      var attached=guarantors[loan.LoanCaseId].Where(x=>x.CustomerId!=borrower.CustomerId).GroupBy(x=>x.CustomerId).Select(g=>g.First()).ToList();
      if(attached.Count==0)result.Issues.Add("Loan "+loan.CaseNumber+", "+stage.NoticeType+": no currently attached guarantors found.");
      recipients.AddRange(attached);
     }
     foreach(var recipient in recipients)
     {
      if(string.IsNullOrWhiteSpace(recipient.Name)){result.Issues.Add("Loan "+loan.CaseNumber+": recipient name is missing.");continue;}
      var candidate=new LoanNoticeCandidate{LoanCaseId=loan.LoanCaseId,RecipientCustomerId=recipient.CustomerId,AsAt=asAt.Date,CaseNumber=loan.CaseNumber,BorrowerName=borrower.Name,RecipientName=recipient.Name,CompanyId=borrower.CompanyId.Value,CompanyName=borrower.CompanyName,PolicyRevision=borrower.PolicyRevision,PrincipalOverdue=loan.OverduePrincipal.Value,InterestOverdue=loan.OverdueInterest.Value,DaysOverdue=loan.DaysPastDue.Value,NoticeType=stage.NoticeType,Channel=stage.Channel,RequireApproval=stage.RequireApproval,Template=stage.Template,ResponseDays=stage.ResponseDays,PolicyJson=borrower.PolicyJson};
      candidate.BasisHash=Hash(candidate);
      // An existing active notice blocks another draft even when the operator changes the as-at date.
      var saved=existing.Where(n=>n.LoanCaseId==loan.LoanCaseId&&n.RecipientCustomerId==recipient.CustomerId&&n.NoticeType==stage.NoticeType).OrderByDescending(n=>n.CreatedDate).FirstOrDefault();
      candidate.ExistingNoticeId=saved?.Id;
      result.Items.Add(candidate);
     }
    }
   }
   result.Total=result.Items.Count;return result;
  }
  public LoanNoticeEligibility Eligible(DateTime asAt,int pageIndex,int pageSize,ServiceHeader h)
  {
   Page(pageIndex,pageSize);using(scopes.CreateReadOnlyWithTransaction(IsolationLevel.Serializable)){
    var r=Candidates(asAt,h);r.Items=r.Items.OrderBy(x=>x.CaseNumber).ThenBy(x=>x.NoticeType).ThenBy(x=>x.RecipientCustomerId).Skip(pageIndex*pageSize).Take(pageSize).ToList();return r;
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
    var now=DateTime.UtcNow;var deadline=now.Date.AddDays(c.ResponseDays);
    var n=new LoanNotice{LoanCaseId=c.LoanCaseId,CompanyId=c.CompanyId,RecipientCustomerId=c.RecipientCustomerId,DuplicateKey=duplicate,CaseNumber=c.CaseNumber,PolicyRevision=c.PolicyRevision,AsAt=c.AsAt,ResponseDeadline=deadline,NoticeType=c.NoticeType,Channel=c.Channel,RecipientName=c.RecipientName,BorrowerName=c.BorrowerName,CompanyName=c.CompanyName,PrincipalOverdue=c.PrincipalOverdue,InterestOverdue=c.InterestOverdue,DaysOverdue=c.DaysOverdue,Body=Render(c,deadline),PolicyJson=c.PolicyJson,SnapshotJson=JsonConvert.SerializeObject(c),RequireApproval=c.RequireApproval,Status=c.RequireApproval?"Draft":"Ready",CreatedBy=h.ApplicationUserName,CreatedDate=now};
    n.GenerateNewIdentity();notices.Add(n,h);scope.SaveChanges(h);return Dto(n);
   }
  }
  static LoanNoticeDTO Dto(LoanNotice n){return new LoanNoticeDTO{Id=n.Id,LoanCaseId=n.LoanCaseId,CaseNumber=n.CaseNumber,BorrowerName=n.BorrowerName,RecipientName=n.RecipientName,CompanyName=n.CompanyName,AsAt=n.AsAt,ResponseDeadline=n.ResponseDeadline,CreatedDate=n.CreatedDate,NoticeType=n.NoticeType,Channel=n.Channel,Status=n.Status,Body=n.Body,PolicyRevision=n.PolicyRevision,RequireApproval=n.RequireApproval,PrincipalOverdue=n.PrincipalOverdue,InterestOverdue=n.InterestOverdue,DaysOverdue=n.DaysOverdue,CreatedBy=n.CreatedBy,ApprovedBy=n.ApprovedBy,ApprovedAtUtc=n.ApprovedAtUtc,CancelledBy=n.CancelledBy,CancelledAtUtc=n.CancelledAtUtc};}
  public LoanNoticePage History(int pageIndex,int pageSize,ServiceHeader h)
  {
   Page(pageIndex,pageSize);using(scopes.CreateReadOnly())return new LoanNoticePage{Total=notices.DatabaseSqlQuery<int>("SELECT COUNT(*) FROM dbo.swiftFin_LoanNotices",h).Single(),Items=notices.DatabaseSqlQuery<LoanNotice>("SELECT * FROM dbo.swiftFin_LoanNotices ORDER BY CreatedDate DESC,Id OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY",h,new SqlParameter("@Skip",pageIndex*pageSize),new SqlParameter("@Take",pageSize)).Select(Dto).ToList()};
  }
  LoanNotice Find(Guid id,ServiceHeader h){var n=notices.Get(id,h);Check(n!=null,"The notice was not found.",404);return n;}
  public LoanNoticeDTO Get(Guid id,ServiceHeader h){using(scopes.CreateReadOnly())return Dto(Find(id,h));}
  public LoanNoticeDTO Approve(Guid id,ServiceHeader h)
  {
   using(var scope=scopes.CreateWithTransaction(IsolationLevel.Serializable)){
    var n=Find(id,h);Check(n.Status=="Draft","Only a draft requiring approval can be approved.",409);
    Check(!string.Equals(n.CreatedBy,h.ApplicationUserName,StringComparison.OrdinalIgnoreCase),"Another user must approve this notice.",403);
    Check(n.ResponseDeadline>=DateTime.UtcNow.Date,"The response deadline has passed. Cancel this notice and generate a fresh draft.");
    n.Status="Approved";n.ApprovedBy=h.ApplicationUserName;n.ApprovedAtUtc=DateTime.UtcNow;scope.SaveChanges(h);return Dto(n);
   }
  }
  public LoanNoticeDTO Cancel(Guid id,ServiceHeader h)
  {
   using(var scope=scopes.CreateWithTransaction(IsolationLevel.Serializable)){var n=Find(id,h);Check(n.Status!="Cancelled","This notice is already cancelled.",409);n.Status="Cancelled";n.CancelledBy=h.ApplicationUserName;n.CancelledAtUtc=DateTime.UtcNow;scope.SaveChanges(h);return Dto(n);}
  }
  public static string PrintableDocument(LoanNoticeDTO n)
  {
   Func<string,string> e=WebUtility.HtmlEncode;
   var label=n.Status=="Draft"?"DRAFT - NOT APPROVED":n.Status=="Cancelled"?"CANCELLED":n.Channel=="Print"?"PRINT COPY - DELIVERY NOT RECORDED":"PREVIEW COPY - NOT SENT";
   return "<!doctype html><html><head><meta charset='utf-8'><meta name='viewport' content='width=device-width'><title>Loan notice "+n.CaseNumber+"</title><style>body{font:12pt Arial,sans-serif;line-height:1.5;margin:30px auto;max-width:760px;padding:24px;color:#111}pre{font:inherit;white-space:pre-wrap;overflow-wrap:anywhere}small{color:#555}@page{size:A4;margin:20mm}@media print{body{margin:0;padding:0}}</style></head><body><h1>"+e(n.CompanyName)+"</h1><p><strong>"+e(label)+"</strong></p><p>Notice: "+e(n.NoticeType)+" · Loan "+n.CaseNumber+"<br>To: "+e(n.RecipientName)+"<br>Prepared: "+n.CreatedDate.ToString("dd MMM yyyy",CultureInfo.InvariantCulture)+"<br>Arrears as at: "+n.AsAt.ToString("dd MMM yyyy",CultureInfo.InvariantCulture)+"</p><hr><pre>"+e(n.Body)+"</pre><hr><small>Reference: "+n.Id+" · Policy revision "+n.PolicyRevision+"</small></body></html>";
  }
  public string Printable(Guid id,ServiceHeader h){return PrintableDocument(Get(id,h));}
 }
}
