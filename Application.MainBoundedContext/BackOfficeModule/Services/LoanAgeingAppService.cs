using System;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanRepaymentPlanAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanCaseAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
namespace Application.MainBoundedContext.BackOfficeModule.Services
{
 public partial class LoanAgeingAppService : ILoanAgeingAppService
 {
  readonly IRepository<LoanRiskReview> riskReviews;readonly IDbContextScopeFactory scopes;readonly IRepository<LoanRepaymentPlan> plans;readonly IRepository<LoanRepaymentInstalment> instalments;readonly IRepository<LoanCase> cases;readonly IRepository<CustomerAccount> customers;readonly IRepository<ChartOfAccount> accounts;
  public LoanAgeingAppService(IDbContextScopeFactory scopes,IRepository<LoanRepaymentPlan> plans,IRepository<LoanRepaymentInstalment> instalments,IRepository<LoanCase> cases,IRepository<CustomerAccount> customers,IRepository<ChartOfAccount> accounts,IRepository<LoanRiskReview> riskReviews){this.riskReviews=riskReviews;this.scopes=scopes;this.plans=plans;this.instalments=instalments;this.cases=cases;this.customers=customers;this.accounts=accounts;}
  static void Check(bool ok,string field,string message,int status=400){if(!ok)throw new LoanAgeingException(field,message,status);}
  static void Page(int page,int size){Check(page>=0&&page<=100000&&size>0&&size<=100,"PageIndex","Choose a valid page and a page size between 1 and 100.");}
  const string CasesSql=@"SELECT l.Id,l.CreatedDate,l.ReceivedDate,l.AmountApplied,l.ApprovedAmount,CAST(l.CaseNumber AS int) CaseNumber,p.Description Product,CASE WHEN customer.Type=0 THEN LTRIM(RTRIM(COALESCE(customer.Individual_FirstName,'')+' '+COALESCE(customer.Individual_LastName,''))) ELSE customer.NonIndividual_Description END LoaneeName,l.CustomerId,l.LoanProductId,l.BranchId,l.DisbursedDate,l.DisbursedAmount,CAST(l.Status AS int) Status,CAST(l.LoanRegistration_TermInMonths AS int) TermMonths,CAST(l.LoanRegistration_PaymentFrequencyPerYear AS int) Frequency,CAST(l.LoanInterest_CalculationMode AS int) CalculationMode,p.InterestReceivableChartOfAccountId,p.InterestChargedChartOfAccountId,p.ChartOfAccountId PrincipalChartOfAccountId,ISNULL(v.Revision,0) PlanRevision,ISNULL(v.IsConfirmed,0) IsConfirmed FROM dbo.swiftFin_LoanCases l JOIN dbo.swiftFin_LoanProducts p ON p.Id=l.LoanProductId LEFT JOIN dbo.swiftFin_Customers customer ON customer.Id=l.CustomerId OUTER APPLY (SELECT TOP 1 Revision,IsConfirmed FROM dbo.swiftFin_LoanRepaymentPlans s WHERE s.LoanCaseId=l.Id ORDER BY Revision DESC) v ";
  const string CaseFilter=" WHERE l.Status IN (48829,48833) AND (@Text='' OR CONVERT(varchar(20),l.CaseNumber)=@Text OR p.Description LIKE @Like) ";
  public LoanAgeingCasePage GetCases(string text,int pageIndex,int pageSize,ServiceHeader h)
  {
   Page(pageIndex,pageSize);text=(text??"").Trim();Check(text.Length<=100,"Text","Search text must be at most 100 characters.");
   var like="%"+text.Replace("[","[[]").Replace("%","[%]").Replace("_","[_]")+"%";
   using(scopes.CreateReadOnly())return new LoanAgeingCasePage{Total=cases.DatabaseSqlQuery<int>("SELECT COUNT(*) FROM dbo.swiftFin_LoanCases l JOIN dbo.swiftFin_LoanProducts p ON p.Id=l.LoanProductId"+CaseFilter,h,new SqlParameter("@Text",text),new SqlParameter("@Like",like)).Single(),Items=cases.DatabaseSqlQuery<LoanAgeingCaseDTO>(CasesSql+CaseFilter+" ORDER BY l.CaseNumber OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY",h,new SqlParameter("@Text",text),new SqlParameter("@Like",like),new SqlParameter("@Skip",pageIndex*pageSize),new SqlParameter("@Take",pageSize)).ToList()};
  }
  LoanPlanDTO Dto(LoanRepaymentPlan p,int caseNumber,ServiceHeader h)
  {
   return new LoanPlanDTO{IsRestructuring=p.IsRestructuring,EffectiveAt=p.EffectiveAt,OpeningInterest=p.OpeningInterest,PriorRiskCategory=p.PriorRiskCategory,PriorPlanIds=p.PriorPlanIds,OpeningLedgerHash=p.OpeningLedgerHash,InterestTermsConfirmed=p.InterestTermsConfirmed,InterestReceivableChartOfAccountId=p.InterestReceivableChartOfAccountId,InterestChargedChartOfAccountId=p.InterestChargedChartOfAccountId,Id=p.Id,LoanCaseId=p.LoanCaseId,CaseNumber=caseNumber,CustomerAccountId=p.CustomerAccountId,PrincipalChartOfAccountId=p.PrincipalChartOfAccountId,SourceJournalId=p.SourceJournalId,Revision=p.Revision,IsConfirmed=p.IsConfirmed,DisbursementDate=p.DisbursementDate,Principal=p.Principal,Evidence=p.Evidence,AllocationPolicy=p.AllocationPolicy,CreatedBy=p.CreatedBy,CreatedDate=p.CreatedDate,Instalments=instalments.AllMatching(new DirectSpecification<LoanRepaymentInstalment>(x=>x.PlanId==p.Id),h).OrderBy(x=>x.Number).Select(x=>new LoanPlanInstalmentDTO{Number=x.Number,DueDate=x.DueDate,Principal=x.Principal,Interest=x.Interest,InterestDueDate=x.InterestDueDate}).ToList()};
  }
  public LoanPlanDTO GetPlan(Guid caseId,ServiceHeader h)
  {
   using(scopes.CreateReadOnly())
   {
    var c=cases.Get(caseId,h);Check(c!=null,"LoanCaseId","Loan case not found.",404);
    var latest=plans.AllMatching(new DirectSpecification<LoanRepaymentPlan>(x=>x.LoanCaseId==caseId),h).OrderByDescending(x=>x.Revision).FirstOrDefault();if(latest!=null){var dto=Dto(latest,c.CaseNumber,h);if(dto.IsRestructuring){var opening=RestructurePlan(c,h);if(dto.OpeningLedgerHash!=opening.OpeningLedgerHash||dto.PriorPlanIds!=opening.PriorPlanIds)dto.IsConfirmed=false;dto.PriorPlanIds=opening.PriorPlanIds;dto.OpeningLedgerHash=opening.OpeningLedgerHash;dto.OpeningInterest=opening.OpeningInterest;dto.PriorRiskCategory=opening.PriorRiskCategory.HasValue?(int?)Math.Max(dto.PriorRiskCategory??0,opening.PriorRiskCategory.Value):dto.PriorRiskCategory;}if(!dto.InterestReceivableChartOfAccountId.HasValue){var current=cases.DatabaseSqlQuery<LoanAgeingCaseDTO>(CasesSql+" WHERE l.Id=@Id",h,new SqlParameter("@Id",caseId)).Single();dto.InterestReceivableChartOfAccountId=current.InterestReceivableChartOfAccountId;dto.InterestChargedChartOfAccountId=current.InterestChargedChartOfAccountId;}return dto;}
    if(c.Status==(int)LoanCaseStatus.Restructured)return RestructurePlan(c,h);
    Check(c.Status==(int)LoanCaseStatus.Disbursed,"LoanCaseId","Only a posted loan can be onboarded here.");
    var links=customers.AllMatching(new DirectSpecification<CustomerAccount>(x=>x.CustomerId==c.CustomerId&&x.CustomerAccountType.TargetProductId==c.LoanProductId),h).ToList();
    Check(links.Count==1,"CustomerAccountId","This case does not resolve to one customer loan account. Review the account links before capturing a schedule.");
    var info=cases.DatabaseSqlQuery<LoanAgeingCaseDTO>(CasesSql+" WHERE l.Id=@Id",h,new SqlParameter("@Id",caseId)).Single();
    var postings=ReadPostings(null,null,h).Where(x=>x.CustomerAccountId==links[0].Id&&x.ChartOfAccountId==info.PrincipalChartOfAccountId).ToList();
    string suffix="~L#"+c.CaseNumber.ToString("D7");
    var funding=postings.Where(x=>x.TransactionCode==(int)SystemTransactionCode.LoanDisbursement&&!x.ParentJournalId.HasValue&&x.Amount>0&&(x.Reference??"").EndsWith(suffix,StringComparison.OrdinalIgnoreCase)).ToList();
    Check(funding.Count==1,"SourceJournalId","The original disbursement cannot be identified unambiguously. Review its case reference and posting before capturing a schedule.");
    var source=funding[0];
    return new LoanPlanDTO{InterestReceivableChartOfAccountId=info.InterestReceivableChartOfAccountId,InterestChargedChartOfAccountId=info.InterestChargedChartOfAccountId,LoanCaseId=caseId,CaseNumber=c.CaseNumber,CustomerAccountId=links[0].Id,PrincipalChartOfAccountId=info.PrincipalChartOfAccountId,SourceJournalId=source.JournalId,DisbursementDate=source.EffectiveDate.Date,Principal=postings.Where(x=>x.JournalId==source.JournalId||x.ParentJournalId==source.JournalId).Sum(x=>x.Amount),AllocationPolicy=LoanAgeingEngine.Policy,Evidence=""};
   }
  }
  public List<LoanPlanDTO> GetHistory(Guid caseId,ServiceHeader h){using(scopes.CreateReadOnly()){var c=cases.Get(caseId,h);Check(c!=null,"LoanCaseId","Loan case not found.",404);return plans.AllMatching(new DirectSpecification<LoanRepaymentPlan>(x=>x.LoanCaseId==caseId),h).OrderByDescending(x=>x.Revision).Take(100).Select(x=>Dto(x,c.CaseNumber,h)).ToList();}}
  public LoanPlanDTO SavePlan(LoanPlanDTO input,ServiceHeader h)
  {
   LoanAgeingEngine.ValidatePlan(input);Check(input.IsConfirmed,"IsConfirmed","Confirm the principal amounts, contractual due dates and supporting evidence before saving.");
   using(var scope=scopes.CreateWithTransaction(IsolationLevel.Serializable))
   {
    var original=GetPlan(input.LoanCaseId,h);
    Check(original.Revision==input.Revision,"Revision","A newer schedule revision exists. Reload before saving.",409);
    Check(input.CustomerAccountId==original.CustomerAccountId&&input.SourceJournalId==original.SourceJournalId&&input.PrincipalChartOfAccountId==original.PrincipalChartOfAccountId&&input.DisbursementDate.Date==original.DisbursementDate.Date&&input.Principal==original.Principal,"Principal","The original loan account, disbursement and principal cannot be changed in a schedule correction.");
    var c=cases.Get(input.LoanCaseId,h);Check(c.Status==(int)LoanCaseStatus.Disbursed||c.Status==(int)LoanCaseStatus.Restructured,"LoanCaseId","Only posted loan schedules can be confirmed here.");
    Check(input.IsRestructuring==original.IsRestructuring,"IsRestructuring","The schedule type must match its posted loan case.");
    if(input.IsRestructuring){var opening=RestructurePlan(c,h);Check(input.EffectiveAt==opening.EffectiveAt&&input.Principal==opening.Principal,"Principal","The restructuring opening changed; reload and review it.");Check(opening.PriorRiskCategory.HasValue,"PriorRiskCategory","Confirm all previous principal and interest schedules before confirming the replacement schedule.");Check(input.PriorRiskCategory.HasValue&&input.PriorRiskCategory>=Math.Max(opening.PriorRiskCategory.Value,original.PriorRiskCategory??0)&&input.PriorRiskCategory<=4,"PriorRiskCategory","Retain at least the pre-restructure risk classification; it cannot improve when dates are replaced.");input.OpeningInterest=opening.OpeningInterest;input.PriorPlanIds=opening.PriorPlanIds;input.OpeningLedgerHash=opening.OpeningLedgerHash;Check(input.InterestTermsConfirmed,"InterestTermsConfirmed","Confirm replacement interest terms as well as principal before saving a restructuring schedule.");}
    else Check(input.EffectiveAt==null&&input.OpeningInterest==null&&input.PriorRiskCategory==null&&string.IsNullOrEmpty(input.PriorPlanIds)&&string.IsNullOrEmpty(input.OpeningLedgerHash),"IsRestructuring","Opening-allocation metadata is only valid for a restructuring schedule.");
    Check(input.InterestReceivableChartOfAccountId==original.InterestReceivableChartOfAccountId&&input.InterestChargedChartOfAccountId==original.InterestChargedChartOfAccountId,"InterestReceivableChartOfAccountId","Interest accounts must match the loan product or the saved schedule. Reload the schedule before saving.");
    var row=LoanAgeingEngine.Entity(input,h);row.Revision=original.Revision+1;plans.Add(row,h);scope.SaveChanges(h);return Dto(row,c.CaseNumber,h);
   }
  }
  const string PostingsSql=@"SELECT e.Id,e.JournalId,j.ParentId ParentJournalId,e.CustomerAccountId,e.ChartOfAccountId,e.ContraChartOfAccountId,e.Amount,COALESCE(j.ValueDate,j.CreatedDate) EffectiveDate,CAST(j.TransactionCode AS int) TransactionCode,j.Reference FROM dbo.swiftFin_JournalEntries e JOIN dbo.swiftFin_Journals j ON j.Id=e.JournalId WHERE (@End IS NULL OR COALESCE(j.ValueDate,j.CreatedDate)<@End) AND (@Branch IS NULL OR j.BranchId=@Branch) AND (EXISTS(SELECT 1 FROM dbo.swiftFin_LoanProducts p WHERE p.ChartOfAccountId=e.ChartOfAccountId) OR EXISTS(SELECT 1 FROM dbo.swiftFin_LoanRepaymentPlans s WHERE s.PrincipalChartOfAccountId=e.ChartOfAccountId))";
  List<LoanAgeingPosting> ReadPostings(DateTime? end,Guid? branch,ServiceHeader h){return accounts.DatabaseSqlQuery<LoanAgeingPosting>(PostingsSql,h,new SqlParameter("@End",SqlDbType.DateTime){Value=(object)end??DBNull.Value},new SqlParameter("@Branch",SqlDbType.UniqueIdentifier){Value=(object)branch??DBNull.Value}).ToList();}
  List<LoanAgeingPosting> ReadInterestPostings(DateTime end,ServiceHeader h)
  {
   const string sql=@"SELECT e.Id,e.JournalId,j.ParentId ParentJournalId,e.CustomerAccountId,e.ChartOfAccountId,e.ContraChartOfAccountId,e.Amount,COALESCE(j.ValueDate,j.CreatedDate) EffectiveDate,CAST(j.TransactionCode AS int) TransactionCode,j.Reference FROM dbo.swiftFin_JournalEntries e JOIN dbo.swiftFin_Journals j ON j.Id=e.JournalId WHERE COALESCE(j.ValueDate,j.CreatedDate)<@End AND (EXISTS(SELECT 1 FROM dbo.swiftFin_LoanProducts p WHERE p.InterestReceivableChartOfAccountId=e.ChartOfAccountId) OR EXISTS(SELECT 1 FROM dbo.swiftFin_LoanRepaymentPlans s WHERE s.InterestReceivableChartOfAccountId=e.ChartOfAccountId))";
   return accounts.DatabaseSqlQuery<LoanAgeingPosting>(sql,h,new SqlParameter("@End",end)).ToList();
  }
  public LoanAgeingResult GetReport(DateTime asAt,Guid? branchId,int pageIndex,int pageSize,Guid? accountId,ServiceHeader h){return ReportCore(asAt,branchId,pageIndex,pageSize,accountId,h,false);}
  public LoanAgeingResult GetNoticeLoanReport(DateTime asAt,ServiceHeader h){return ReportCore(asAt,null,0,100,null,h,true,true);}
  public LoanAgeingResult GetLoanReport(DateTime asAt,Guid? branchId,int pageIndex,int pageSize,ServiceHeader h)
  {
   Page(pageIndex,pageSize);
   using(scopes.CreateReadOnlyWithTransaction(IsolationLevel.Serializable))
   {
    var report=ReportCore(asAt,null,0,100,null,h,true,true);
    var register=cases.DatabaseSqlQuery<LoanAgeingCaseDTO>(CasesSql+" WHERE l.CreatedDate<@End AND (@Branch IS NULL OR l.BranchId=@Branch)",h,new SqlParameter("@End",asAt.Date.AddDays(1)),new SqlParameter("@Branch",SqlDbType.UniqueIdentifier){Value=(object)branchId??DBNull.Value}).ToList();
    var calculated=report.Loans.ToDictionary(x=>x.LoanCaseId);
    report.Loans=register.Select(c=>{
     LoanAgeingLoanResult row;
     bool posted=(c.Status==(int)LoanCaseStatus.Disbursed||c.Status==(int)LoanCaseStatus.Restructured)&&(c.DisbursedDate<asAt.Date.AddDays(1)||(c.Status==(int)LoanCaseStatus.Restructured&&c.CreatedDate<asAt.Date.AddDays(1)));
     if(!calculated.TryGetValue(c.Id,out row)){
      row=new LoanAgeingLoanResult{LoanCaseId=c.Id,CaseNumber=c.CaseNumber,LoaneeName=c.LoaneeName,Product=c.Product};
      if(posted)row.Issues.Add("No unambiguous loan account and ageing result was found for this posted case.");
      else row.RiskClassification="Not disbursed";
     }
     row.AppliedDate=c.ReceivedDate;row.DisbursedDate=c.DisbursedDate;row.AmountApplied=c.AmountApplied;row.ApprovedAmount=c.ApprovedAmount;row.DisbursedAmount=c.DisbursedAmount;row.TermMonths=c.TermMonths;row.IsDisbursed=posted;
     row.LoanStatus=c.Status==(int)LoanCaseStatus.Audited?"Verified":Enum.IsDefined(typeof(LoanCaseStatus),c.Status)?((LoanCaseStatus)c.Status).ToString():"Unknown";
     return row;
    }).OrderBy(x=>x.CaseNumber).ThenBy(x=>x.LoanCaseId).ToList();
    report.TotalLoans=report.Loans.Count;report.Loans=report.Loans.Skip(pageIndex*pageSize).Take(pageSize).ToList();return report;
   }
  }
  LoanAgeingResult ReportCore(DateTime asAt,Guid? branchId,int pageIndex,int pageSize,Guid? accountId,ServiceHeader h,bool allDetails,bool perLoan=false)
  {
   Page(pageIndex,pageSize);Check(asAt.Year>=1753&&asAt.Year<9999,"AsAt","Select a valid reporting date.");
   using(scopes.CreateReadOnlyWithTransaction(IsolationLevel.Serializable))
   {
    // Branch scopes follow account ownership; all cross-branch postings for those accounts are included.
    var allCases=cases.DatabaseSqlQuery<LoanAgeingCaseDTO>(CasesSql+" WHERE l.Status IN (48829,48833) AND (l.DisbursedDate<@End OR (l.Status=48833 AND l.CreatedDate<@End))",h,new SqlParameter("@End",asAt.Date.AddDays(1))).ToList();
    var links=customers.DatabaseSqlQuery<LoanAgeingAccountLink>("SELECT Id,CustomerId,CustomerAccountType_TargetProductId ProductId,BranchId FROM dbo.swiftFin_CustomerAccounts WHERE CustomerAccountType_ProductCode=2",h).ToList();
    var postingRows=ReadPostings(asAt.Date.AddDays(1),null,h);
    var interestRows=ReadInterestPostings(asAt.Date.AddDays(1),h);
    var planRows=plans.AllMatching(new DirectSpecification<LoanRepaymentPlan>(x=>x.DisbursementDate<=asAt),h).GroupBy(x=>x.LoanCaseId).Select(g=>g.OrderByDescending(x=>x.Revision).First()).ToList();
    var planIds=planRows.Select(x=>x.Id).ToArray();
    var allInstalments=instalments.AllMatching(new DirectSpecification<LoanRepaymentInstalment>(x=>planIds.Contains(x.PlanId)),h).ToList();
    // Materialize schedule rows once, avoiding a repository query per loan in portfolio reports.
    var instalmentLookup=allInstalments.ToLookup(x=>x.PlanId);
    var caseLookup=allCases.ToDictionary(x=>x.Id);
    var snapshots=planRows.Select(p=>new LoanPlanDTO{IsRestructuring=p.IsRestructuring,EffectiveAt=p.EffectiveAt,OpeningInterest=p.OpeningInterest,PriorRiskCategory=p.PriorRiskCategory,PriorPlanIds=p.PriorPlanIds,OpeningLedgerHash=p.OpeningLedgerHash,InterestTermsConfirmed=p.InterestTermsConfirmed,InterestReceivableChartOfAccountId=p.InterestReceivableChartOfAccountId,InterestChargedChartOfAccountId=p.InterestChargedChartOfAccountId,Id=p.Id,LoanCaseId=p.LoanCaseId,CaseNumber=caseLookup.ContainsKey(p.LoanCaseId)?caseLookup[p.LoanCaseId].CaseNumber:0,CustomerAccountId=p.CustomerAccountId,PrincipalChartOfAccountId=p.PrincipalChartOfAccountId,SourceJournalId=p.SourceJournalId,Revision=p.Revision,IsConfirmed=p.IsConfirmed,DisbursementDate=p.DisbursementDate,Principal=p.Principal,Evidence=p.Evidence,AllocationPolicy=p.AllocationPolicy,Instalments=instalmentLookup[p.Id].OrderBy(x=>x.Number).Select(x=>new LoanPlanInstalmentDTO{Number=x.Number,DueDate=x.DueDate,Principal=x.Principal,Interest=x.Interest,InterestDueDate=x.InterestDueDate}).ToList()}).ToList();
    var selectedLinks=links.Where(x=>!branchId.HasValue||x.BranchId==branchId).ToList();var selectedIds=new HashSet<Guid>(selectedLinks.Select(x=>x.Id));
    var r=new LoanAgeingResult{AsAt=asAt.Date,GeneratedAtUtc=DateTime.UtcNow,Policy=LoanAgeingEngine.Policy};
    var interestByAccount=interestRows.Where(x=>x.CustomerAccountId.HasValue).ToLookup(x=>x.CustomerAccountId.Value);
    var grouped=postingRows.Where(x=>x.CustomerAccountId.HasValue&&selectedIds.Contains(x.CustomerAccountId.Value)).GroupBy(x=>x.CustomerAccountId.Value).ToDictionary(g=>g.Key,g=>g.ToList());
    var casesByCustomerProduct=allCases.ToLookup(x=>Tuple.Create(x.CustomerId,x.LoanProductId));
    var linksByCustomerProduct=links.ToLookup(x=>Tuple.Create(x.CustomerId,x.ProductId));
    var plansByAccount=snapshots.ToLookup(x=>x.CustomerAccountId);
    foreach(var link in selectedLinks.OrderBy(x=>x.Id))
    {
     var cs=casesByCustomerProduct[Tuple.Create(link.CustomerId,link.ProductId)].ToList();List<LoanAgeingPosting> es;grouped.TryGetValue(link.Id,out es);es=es??new List<LoanAgeingPosting>();
     if(cs.Count==0&&es.Count==0&&!interestByAccount[link.Id].Any())continue;
     var ps=plansByAccount[link.Id].Where(x=>cs.Any(c=>c.Id==x.LoanCaseId)).ToList();
     var replacement=ps.Where(x=>x.IsRestructuring&&x.EffectiveAt.HasValue&&x.EffectiveAt.Value<asAt.Date.AddDays(1)).OrderByDescending(x=>x.EffectiveAt).FirstOrDefault();
     var item=replacement==null?LoanAgeingEngine.Calculate(link.Id,asAt,cs,ps,es):LoanRestructureAgeing.Calculate(link.Id,asAt,cs,ps,es,interestByAccount[link.Id].ToList());
     if(cs.Any(c=>linksByCustomerProduct[Tuple.Create(c.CustomerId,c.LoanProductId)].Count()>1))item.Issues.Add("Multiple customer accounts match the same loan case. Resolve the ambiguity before ageing.");
     if(item.Issues.Count>0){item.Bucket="Needs review";item.DaysPastDue=null;item.OverduePrincipal=null;item.Instalments.Clear();}
     if(item.Interest==null)item.Interest=LoanInterestAgeingEngine.Calculate(link.Id,asAt,cs,ps,interestByAccount[link.Id].ToList());
     if(cs.Any(c=>linksByCustomerProduct[Tuple.Create(c.CustomerId,c.LoanProductId)].Count()>1)){item.Interest.Issues.Add("Multiple customer accounts match the same loan case.");item.Interest.DaysPastDue=null;item.Interest.OverdueInterest=null;item.Interest.Bucket="Needs review";item.Interest.Instalments.Clear();}
     if(item.DaysPastDue.HasValue&&item.Interest.DaysPastDue.HasValue){item.CombinedDaysPastDue=Math.Max(item.DaysPastDue.Value,item.Interest.DaysPastDue.Value);item.CombinedBucket=LoanAgeingEngine.Bucket(item.CombinedDaysPastDue.Value);}
     r.Accounts.Add(item);
     if(perLoan)r.Loans.AddRange(LoanAgeingEngine.ByLoan(item,cs,asAt));
     if(perLoan&&cs.Count==0)r.Issues.Add("A loan account has postings without a linked loan case; its balance cannot be listed per loan.");
    }
    r.AccountInterestReceivable=r.Accounts.Sum(x=>x.Interest.Receivable);
    r.LedgerInterestReceivable=branchId.HasValue?interestRows.Where(x=>x.CustomerAccountId.HasValue&&selectedIds.Contains(x.CustomerAccountId.Value)).Sum(x=>x.Amount):interestRows.Sum(x=>x.Amount);
    r.InterestDifference=r.AccountInterestReceivable-r.LedgerInterestReceivable;
    if(Math.Abs(r.InterestDifference)>.01m)r.Issues.Add("Loan-account interest receivable does not reconcile to the interest-receivable G/L. Review unlinked postings.");
    r.KnownOverdueInterest=r.Accounts.Sum(x=>x.Interest.OverdueInterest??0);r.InterestAccountsRequiringReview=r.Accounts.Count(x=>x.Interest.Issues.Count>0);
    r.AccountPrincipal=r.Accounts.Sum(x=>x.OutstandingPrincipal);
    r.LedgerPrincipal=branchId.HasValue?postingRows.Where(x=>x.CustomerAccountId.HasValue&&selectedIds.Contains(x.CustomerAccountId.Value)).Sum(x=>x.Amount):postingRows.Sum(x=>x.Amount);
    r.Difference=r.AccountPrincipal-r.LedgerPrincipal;if(Math.Abs(r.Difference)>.01m)r.Issues.Add("Loan-account principal does not reconcile to the selected loan G/L balances. Review unlinked or non-loan-account postings.");
    r.TotalAccounts=r.Accounts.Count;r.AccountsRequiringReview=r.Accounts.Count(x=>x.Issues.Count>0);r.KnownOverduePrincipal=r.Accounts.Sum(x=>x.OverduePrincipal??0m);
    r.Warnings.Add("Principal and confirmed contractual interest are aged separately. Combined days overdue requires both components to be resolved. SASRA classifications, provisions and restructuring are not assessed. Unknown accounts are excluded from known overdue totals.");
    r.Warnings.Add("Confirmed schedule corrections are applied to historical reports. The response identifies the schedule revision through its plan ID; earlier revisions remain available in schedule history.");
    r.Warnings.Add("Net principal reductions settle oldest due instalments first across cases sharing an account. Prepayments settle future instalments; refunds and repayment reversals reopen the most recently settled principal. This is a reporting allocation, not a new financial posting.");
    if(perLoan){r.TotalLoans=r.Loans.Select(x=>x.LoanCaseId).Distinct().Count();r.Loans=r.Loans.GroupBy(x=>x.LoanCaseId).Select(g=>{var row=g.First();if(g.Count()>1){row.OutstandingPrincipal=null;row.OutstandingInterest=null;row.RiskClassification="Needs review";row.OverduePrincipal=null;row.OverdueInterest=null;row.DaysPastDue=null;row.Status="Needs review";}return row;}).OrderBy(x=>x.CaseNumber).ThenBy(x=>x.LoanCaseId).ToList();if(!allDetails)r.Loans=r.Loans.Skip(pageIndex*pageSize).Take(pageSize).ToList();r.Accounts.Clear();return r;}
    if(accountId.HasValue){r.Accounts=r.Accounts.Where(x=>x.CustomerAccountId==accountId).ToList();Check(r.Accounts.Count>0,"CustomerAccountId","No loan account was found in this reporting scope.",404);}else if(!allDetails){r.Accounts=r.Accounts.Skip(pageIndex*pageSize).Take(pageSize).ToList();foreach(var a in r.Accounts){a.Instalments.Clear();a.Interest.Instalments.Clear();}}
    return r;
   }
  }
 }
}
