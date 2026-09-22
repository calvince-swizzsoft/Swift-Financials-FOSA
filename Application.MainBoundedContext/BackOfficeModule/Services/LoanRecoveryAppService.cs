using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanRecoveryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
namespace Application.MainBoundedContext.BackOfficeModule.Services
{
 public class LoanRecoveryAppService : ILoanRecoveryAppService
 {
  readonly IDbContextScopeFactory scopes;
  readonly IRepository<LoanRecovery> recoveries;
  readonly IRepository<Journal> journals;
  readonly ILoanNoticeAppService notices;
  readonly ILoanAgeingAppService ageing;
  public LoanRecoveryAppService(IDbContextScopeFactory scopes,IRepository<LoanRecovery> recoveries,IRepository<Journal> journals,ILoanNoticeAppService notices,ILoanAgeingAppService ageing)
  {this.scopes=scopes;this.recoveries=recoveries;this.journals=journals;this.notices=notices;this.ageing=ageing;}
  static void Check(bool valid,string message,int status=409){if(!valid)throw new LoanAgeingException("Recovery",message,status);}
  List<T> Read<T>(string sql,ServiceHeader h,params object[] args){return recoveries.DatabaseSqlQuery<T>(sql,h,args).ToList();}
  static SqlParameter P(string name,object value){return new SqlParameter(name,value);}
  public LoanRecoveryPreview Preview(Guid loanCaseId,ServiceHeader h)
  {using(scopes.CreateReadOnly())return Build(loanCaseId,h);}
  public LoanRecoveryPreview Preview(LoanRecoveryRequest input,ServiceHeader h)
  {
   Check(input!=null,"Choose a loan.",400);
   using(scopes.CreateReadOnly())return Build(input.LoanCaseId,h,input.SelectedAccountIds);
  }
  public class LoanLink
  {public Guid CustomerId{get;set;}public Guid AccountId{get;set;}public Guid BranchId{get;set;}public Guid PrincipalGlId{get;set;}public Guid InterestGlId{get;set;}public string Name{get;set;}}
  public class Guarantee
  {public Guid Id{get;set;}public Guid CustomerId{get;set;}public string Name{get;set;}public decimal AmountGuaranteed{get;set;}public decimal AlreadyAttached{get;set;}}
  const string Name="CASE WHEN c.Type=0 THEN LTRIM(RTRIM(COALESCE(c.Individual_FirstName,'')+' '+COALESCE(c.Individual_LastName,''))) ELSE c.NonIndividual_Description END";
  decimal Ledger(Guid account,Guid gl,ServiceHeader h)
  {
   return Read<decimal>(@"SELECT CAST(COALESCE(SUM(e.Amount),0) AS decimal(18,2)) FROM dbo.swiftFin_JournalEntries e JOIN dbo.swiftFin_Journals j ON j.Id=e.JournalId WHERE e.CustomerAccountId=@Account AND e.ChartOfAccountId=@Gl AND COALESCE(j.ValueDate,j.CreatedDate)<@End",h,P("@Account",account),P("@Gl",gl),P("@End",DateTime.Today.AddDays(1))).Single();
  }
  LoanRecoveryMember Member(Guid customer,Guid loan,string name,ServiceHeader h)
  {
   var m=new LoanRecoveryMember{CustomerId=customer,Name=name};
   // Reserve other active guarantees conservatively; never subtract this loan's
   // own commitment from the funds that are now being called under that guarantee.
   m.OtherCommitments=Read<decimal>(@"SELECT CAST(COALESCE(SUM(AmountGuaranteed),0) AS decimal(18,2)) FROM dbo.swiftFin_LoanGuarantors WHERE CustomerId=@Customer AND Status=0 AND (LoanCaseId IS NULL OR LoanCaseId<>@Loan)",h,P("@Customer",customer),P("@Loan",loan)).Single();
   // Only liability-backed deposits. Refundability controls refunds, not recovery.
   // Equity/share capital is excluded by GL classification. No ABS balances.
   m.Accounts=Read<LoanRecoveryAccount>(@"
SELECT a.Id,p.ChartOfAccountId,p.Description Product,
 CAST(CASE WHEN -COALESCE(b.Amount,0)-(CASE WHEN p.MinimumBalance>0 THEN p.MinimumBalance ELSE 0 END)>0 THEN -COALESCE(b.Amount,0)-(CASE WHEN p.MinimumBalance>0 THEN p.MinimumBalance ELSE 0 END) ELSE 0 END AS decimal(18,2)) Available
FROM dbo.swiftFin_CustomerAccounts a
JOIN dbo.swiftFin_Customers c ON c.Id=a.CustomerId AND c.IsLocked=0 AND c.RecordStatus=2
CROSS APPLY (
 SELECT s.ChartOfAccountId,s.Description,COALESCE((SELECT TOP 1 x.MinimumBalance FROM dbo.swiftFin_SavingsProductExemptions x WHERE x.SavingsProductId=s.Id AND x.BranchId=a.BranchId ORDER BY x.Id),s.MinimumBalance) MinimumBalance,0 MaturityPeriod
 FROM dbo.swiftFin_SavingsProducts s WHERE a.CustomerAccountType_ProductCode=1 AND s.Id=a.CustomerAccountType_TargetProductId AND s.IsLocked=0
 UNION ALL
 SELECT i.ChartOfAccountId,i.Description,i.MinimumBalance,CAST(i.MaturityPeriod AS int)
 FROM dbo.swiftFin_InvestmentProducts i WHERE a.CustomerAccountType_ProductCode=3 AND i.Id=a.CustomerAccountType_TargetProductId AND i.IsLocked=0
) p
JOIN dbo.swiftFin_ChartOfAccounts gl ON gl.Id=p.ChartOfAccountId AND gl.AccountType=2000 AND gl.IsLocked=0
OUTER APPLY (
 SELECT SUM(e.Amount) Amount FROM dbo.swiftFin_JournalEntries e JOIN dbo.swiftFin_Journals j ON j.Id=e.JournalId
 WHERE e.CustomerAccountId=a.Id AND e.ChartOfAccountId=p.ChartOfAccountId
 AND (e.Amount>=0 OR (COALESCE(j.ValueDate,j.CreatedDate)<=@Now AND e.CreatedDate<=DATEADD(day,-p.MaturityPeriod,@Now)))
) b
WHERE a.CustomerId=@Customer AND a.Status=0 AND a.RecordStatus=2
 AND NOT EXISTS(SELECT 1 FROM dbo.swiftFin_FixedDeposits f WHERE f.CustomerAccountId=a.Id AND f.Status IN (1,8))
ORDER BY a.Id",h,P("@Customer",customer),P("@Now",DateTime.Now));
   return m;
  }
  LoanRecoveryPreview Build(Guid loanId,ServiceHeader h,List<Guid> selectedAccountIds=null)
  {
   Check(loanId!=Guid.Empty,"Choose a loan.",400);
   var eligible=notices.RecoveryLoan(loanId,h);
   var report=ageing.GetNoticeLoanReport(DateTime.UtcNow.Date,h);
   var loan=report.Loans.SingleOrDefault(l=>l.LoanCaseId==loanId);
   Check(loan!=null&&loan.Issues.Count==0&&loan.OverduePrincipal.HasValue&&loan.OverdueInterest.HasValue,"The loan ageing requires review.");
   Check(report.Loans.Count(l=>l.CustomerAccountId==loan.CustomerAccountId)==1,"Separate the loans sharing this account before deposit recovery.");
   var links=Read<LoanLink>(@"SELECT l.CustomerId,a.Id AccountId,a.BranchId,p.ChartOfAccountId PrincipalGlId,p.InterestReceivableChartOfAccountId InterestGlId,"+Name+@" Name
FROM dbo.swiftFin_LoanCases l JOIN dbo.swiftFin_Customers c ON c.Id=l.CustomerId
JOIN dbo.swiftFin_CustomerAccounts a ON a.CustomerId=l.CustomerId AND a.CustomerAccountType_TargetProductId=l.LoanProductId AND a.CustomerAccountType_ProductCode=2
JOIN dbo.swiftFin_LoanProducts p ON p.Id=l.LoanProductId
JOIN dbo.swiftFin_ChartOfAccounts pg ON pg.Id=p.ChartOfAccountId AND pg.AccountType=1000 AND pg.IsLocked=0
JOIN dbo.swiftFin_ChartOfAccounts ig ON ig.Id=p.InterestReceivableChartOfAccountId AND ig.AccountType=1000 AND ig.IsLocked=0
WHERE l.Id=@Loan AND l.Status IN (48829,48833) AND a.Status=0 AND a.RecordStatus=2 AND p.IsLocked=0",h,P("@Loan",loanId));
   Check(links.Count==1&&links[0].AccountId==loan.CustomerAccountId,"Review the loan account and principal/interest ledger mappings.");
   var link=links[0];Check(link.PrincipalGlId!=link.InterestGlId,"Principal and interest must use different control accounts.");
   var periods=Read<Guid>(@"SELECT Id FROM dbo.swiftFin_PostingPeriods WHERE IsActive=1 AND IsClosed=0 AND IsLocked=0 AND Duration_StartDate<=@Today AND Duration_EndDate>=@Today",h,P("@Today",DateTime.Today));
   Check(periods.Count==1,"Open one current posting period before recovery.");
   var p=new LoanRecoveryPreview{LoanCaseId=loanId,CaseNumber=loan.CaseNumber,AsAt=DateTime.Today,LoanAccountId=link.AccountId,BranchId=link.BranchId,PrincipalGlId=link.PrincipalGlId,InterestGlId=link.InterestGlId,PostingPeriodId=periods[0],EligibilityHash=eligible.BasisHash};
   p.PrincipalDue=LoanRecoveryAllocation.Floor(Math.Min(loan.OverduePrincipal.Value,Math.Max(0,Ledger(link.AccountId,link.PrincipalGlId,h))));
   p.InterestDue=LoanRecoveryAllocation.Floor(Math.Min(loan.OverdueInterest.Value,Math.Max(0,Ledger(link.AccountId,link.InterestGlId,h))));
   p.UnpostedInterest=LoanRecoveryAllocation.Floor(loan.OverdueInterest.Value-p.InterestDue);
   p.Borrower=Member(link.CustomerId,loanId,link.Name,h);
   var history=Read<LoanRecovery>("SELECT * FROM dbo.swiftFin_LoanRecoveries WHERE LoanCaseId=@Loan ORDER BY CreatedDate,Id",h,P("@Loan",loanId)).Select(r=>JsonConvert.DeserializeObject<LoanRecoveryPreview>(r.SnapshotJson)).ToList();
   var gs=Read<Guarantee>(@"SELECT g.Id,g.CustomerId,g.AmountGuaranteed,"+Name+@" Name,
 CAST(COALESCE((SELECT SUM(e.PrincipalAttached+e.InterestAttached) FROM dbo.swiftFin_LoanGuarantorAttachmentHistoryEntries e WHERE e.LoanGuarantorId=g.Id),0) AS decimal(18,2)) AlreadyAttached
FROM dbo.swiftFin_LoanGuarantors g JOIN dbo.swiftFin_Customers c ON c.Id=g.CustomerId WHERE g.LoanCaseId=@Loan AND g.Status=0 AND g.CustomerId<>@Borrower ORDER BY g.CustomerId,g.Id",h,P("@Loan",loanId),P("@Borrower",link.CustomerId));
   Check(gs.GroupBy(g=>g.CustomerId).All(g=>g.Count()==1),"Resolve duplicate active guarantees for this loan before recovery.");
   foreach(var g in gs)
   {
    var m=Member(g.CustomerId,loanId,g.Name,h);m.GuarantorId=g.Id;m.Guaranteed=g.AmountGuaranteed;
    m.RemainingGuarantee=LoanRecoveryAllocation.Floor(g.AmountGuaranteed-g.AlreadyAttached-history.SelectMany(x=>x.Guarantors).Where(x=>x.GuarantorId==g.Id).Sum(x=>x.Amount));
    p.Guarantors.Add(m);
   }
   var selected=new HashSet<Guid>(selectedAccountIds??new List<Guid>());
   var accounts=new[]{p.Borrower}.Concat(p.Guarantors).SelectMany(m=>m.Accounts).ToList();
   Check(selected.All(id=>accounts.Any(a=>a.Id==id)),"A selected deposit account is no longer eligible. Reopen recovery and select the accounts again.");
   foreach(var account in accounts)account.Selected=selected.Contains(account.Id);
   LoanRecoveryAllocation.Allocate(p);
   using(var sha=SHA256.Create())p.BasisHash=BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(p)))).Replace("-","");
   return p;
  }
  static LoanRecoveryReceipt Receipt(LoanRecovery r){return new LoanRecoveryReceipt{Id=r.Id,LoanCaseId=r.LoanCaseId,Total=r.Total,PostedAt=r.CreatedDate};}
  public LoanRecoveryReceipt Post(LoanRecoveryRequest input,ServiceHeader h)
  {
   Check(input!=null&&input.LoanCaseId!=Guid.Empty&&input.SelectedAccountIds!=null&&input.SelectedAccountIds.Count>0&&input.RequestId!=Guid.Empty&&!string.IsNullOrWhiteSpace(input.BasisHash),"Refresh the recovery preview before posting.",400);
   using(var scope=scopes.CreateWithTransaction(IsolationLevel.Serializable))
   {
    // Serialize requests for this case. Serializable reads also protect the
    // source ledger ranges/guarantees while the reviewed balances are rechecked.
    Check(Read<Guid>("SELECT Id FROM dbo.swiftFin_LoanCases WITH (UPDLOCK,HOLDLOCK) WHERE Id=@Loan",h,P("@Loan",input.LoanCaseId)).Count==1,"Loan not found.",404);
    var existing=Read<LoanRecovery>("SELECT * FROM dbo.swiftFin_LoanRecoveries WHERE RequestId=@Request",h,P("@Request",input.RequestId)).SingleOrDefault();
    if(existing!=null){Check(existing.LoanCaseId==input.LoanCaseId&&existing.BasisHash==input.BasisHash,"This recovery request was already used.");var saved=JsonConvert.DeserializeObject<LoanRecoveryPreview>(existing.SnapshotJson);Check(new HashSet<Guid>(input.SelectedAccountIds).SetEquals(new[]{saved.Borrower}.Concat(saved.Guarantors).SelectMany(m=>m.Accounts).Where(a=>a.Selected).Select(a=>a.Id)),"This recovery request was already used with different deposit accounts.");return Receipt(existing);}
    var plan=Build(input.LoanCaseId,h,input.SelectedAccountIds);
    Check(plan.BasisHash==input.BasisHash,"Balances or guarantees changed. Refresh the recovery preview.");
    Check(plan.TotalRecovery>0,"No eligible deposits are available for recovery.");
    var ids=new List<Guid>();var interest=plan.InterestRecovery;
    foreach(var member in new[]{plan.Borrower}.Concat(plan.Guarantors))foreach(var source in member.Accounts.Where(a=>a.Amount>0))
    {
     var interestPart=Math.Min(interest,source.Amount);interest-=interestPart;
     foreach(var part in new[]{new{Amount=interestPart,Gl=plan.InterestGlId,Label="Interest"},new{Amount=source.Amount-interestPart,Gl=plan.PrincipalGlId,Label="Principal"}}.Where(x=>x.Amount>0))
     {
      var journal=JournalFactory.CreateJournal(null,plan.PostingPeriodId,plan.BranchId,null,part.Amount,"Loan deposit recovery",part.Label+" - Loan "+plan.CaseNumber,"Recovery "+input.RequestId.ToString("N"),0,(int)SystemTransactionCode.LoanOffsetting,null,h,true);
      journal.PostDoubleEntries(source.ChartOfAccountId,part.Gl,plan.LoanAccountId,source.Id,h);
      Check(journal.JournalEntries.Count==2&&journal.JournalEntries.Sum(e=>e.Amount)==0,"Recovery journal is not balanced.");
      journals.Add(journal,h);ids.Add(journal.Id);
     }
    }
    var record=new LoanRecovery{LoanCaseId=plan.LoanCaseId,RequestId=input.RequestId,BasisHash=plan.BasisHash,Total=plan.TotalRecovery,SnapshotJson=JsonConvert.SerializeObject(plan),JournalIdsJson=JsonConvert.SerializeObject(ids),CreatedBy=h.ApplicationUserName,CreatedDate=DateTime.Now};record.GenerateNewIdentity();recoveries.Add(record,h);
    scope.SaveChanges(h);return Receipt(record);
   }
  }
 }
}
