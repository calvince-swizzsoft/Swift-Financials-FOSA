using System;
using System.Linq;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.CompanyAgg;
using Infrastructure.Crosscutting.Framework.Utils;
namespace Application.MainBoundedContext.AdministrationModule.Services
{
 public partial class CompanyAppService
 {
  public static DefaulterNoticePolicyDTO ParseNoticePolicy(string json)
  {
   if(json==null)return new DefaulterNoticePolicyDTO();
   if(json.Length>60000)throw new InvalidOperationException("Defaulter notices: the policy is too large.");
   DefaulterNoticePolicyDTO p;
   try{p=JsonConvert.DeserializeObject<DefaulterNoticePolicyDTO>(json,new JsonSerializerSettings{MissingMemberHandling=MissingMemberHandling.Error,MaxDepth=12});}
   catch(JsonException){throw new InvalidOperationException("Defaulter notices: the settings could not be read. Reopen the company and try again.");}
   if(p==null||p.Stages==null||p.Stages.Count>6||p.Enabled&&p.Stages.Count==0)throw new InvalidOperationException("Defaulter notices: provide one to six stages when enabled.");
   var types=new[]{"Reminder","FirstNotice","SecondNotice","FinalDemand","GuarantorNotification","GuarantorDemand"};
   var tokens=new[]{"companyName","borrowerName","recipientName","loanNumber","arrears","principalOverdue","interestOverdue","daysOverdue","asAt","responseDeadline"};
   foreach(var s in p.Stages){
    if(s==null||!types.Contains(s.NoticeType))throw new InvalidOperationException("Defaulter notices: select a supported notice type. Statutory and CRB notices require a separate workflow.");
    if(s.DaysOverdue<1||s.DaysOverdue>3650||s.ResponseDays<1||s.ResponseDays>365)throw new InvalidOperationException("Defaulter notices: overdue days must be 1–3650 and response days 1–365.");
    if(s.MinimumArrears<0||s.MinimumArrears>1000000000000000m||decimal.Round(s.MinimumArrears,2)!=s.MinimumArrears)throw new InvalidOperationException("Defaulter notices: minimum arrears must be non-negative with at most two decimal places.");
    if(!new[]{"Borrower","Guarantors","Both"}.Contains(s.Recipient)||!new[]{"Print","Email","SMS"}.Contains(s.Channel))throw new InvalidOperationException("Defaulter notices: select a recipient and delivery channel.");
    if(s.NoticeType.StartsWith("Guarantor")&&s.Recipient=="Borrower")throw new InvalidOperationException("Defaulter notices: guarantor notices must include guarantors as recipients.");
    if(string.IsNullOrWhiteSpace(s.Template)||s.Template.Length>8000)throw new InvalidOperationException("Defaulter notices: each stage needs a template of 1–8000 characters.");
    foreach(Match m in Regex.Matches(s.Template,@"\{\{(.*?)\}\}"))if(!tokens.Contains(m.Groups[1].Value))throw new InvalidOperationException("Defaulter notices: use only the supported template placeholders.");
    var remainder=Regex.Replace(s.Template,@"\{\{(.*?)\}\}","");
    if(remainder.Contains("{{")||remainder.Contains("}}"))throw new InvalidOperationException("Defaulter notices: close every template placeholder with double braces.");
   }
   if(p.Stages.GroupBy(x=>x.NoticeType).Any(g=>g.Count()>1))throw new InvalidOperationException("Defaulter notices: each notice type may appear only once.");
   return p;
  }
  public static void ApplyNoticePolicy(Company target,CompanyDTO input,Company previous)
  {
   target.DefaulterNoticePolicyJson=previous?.DefaulterNoticePolicyJson;
   target.DefaulterNoticePolicyRevision=previous?.DefaulterNoticePolicyRevision??0;
   if(input.DefaulterNoticePolicyJson==null)return; // Older callers preserve the saved policy.
   var policy=ParseNoticePolicy(input.DefaulterNoticePolicyJson);
   var json=JsonConvert.SerializeObject(policy,new JsonSerializerSettings{ContractResolver=new CamelCasePropertyNamesContractResolver()});
   if(previous!=null&&input.DefaulterNoticePolicyRevision!=previous.DefaulterNoticePolicyRevision)throw new InvalidOperationException("Defaulter notices: another user changed the policy. Reopen the company before saving.");
   if(json!=target.DefaulterNoticePolicyJson){target.DefaulterNoticePolicyJson=json;target.DefaulterNoticePolicyRevision++;}
  }
  public ResolvedDefaulterNoticePolicyDTO ResolveDefaulterNoticePolicy(Guid loanCaseId,ServiceHeader h)
  {
   using(_dbContextScopeFactory.CreateReadOnly()){
    var row=_companyRepository.DatabaseSqlQuery<ResolvedDefaulterNoticePolicyDTO>(@"SELECT l.Id LoanCaseId,l.BranchId,b.CompanyId,c.Description CompanyName,c.DefaulterNoticePolicyJson PolicyJson,ISNULL(c.DefaulterNoticePolicyRevision,0) Revision FROM dbo.swiftFin_LoanCases l LEFT JOIN dbo.swiftFin_Branches b ON b.Id=l.BranchId LEFT JOIN dbo.swiftFin_Companies c ON c.Id=b.CompanyId WHERE l.Id=@Id",h,new SqlParameter("@Id",loanCaseId)).SingleOrDefault();
    if(row==null)throw new InvalidOperationException("The selected loan case was not found.");
    if(!row.CompanyId.HasValue||row.CompanyName==null)throw new InvalidOperationException("The loan's branch has no valid company. Link the branch to a company before configuring notices.");
    row.Policy=ParseNoticePolicy(row.PolicyJson);row.PolicyJson=null;return row;
   }
  }
 }
}
