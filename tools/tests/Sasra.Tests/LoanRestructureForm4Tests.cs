using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using NPOI.HSSF.UserModel;
static class LoanRestructureForm4Tests
{
 static int checks;static void Check(bool ok,string label){if(!ok)throw new Exception(label);checks++;}
 static Guid account=Guid.NewGuid(),gl=Guid.NewGuid(),interestGl=Guid.NewGuid(),income=Guid.NewGuid();
 static DateTime start=new DateTime(2026,1,1),at=new DateTime(2026,3,1,12,0,0);
 static LoanPlanDTO Plan(int number,DateTime date,decimal principal){return new LoanPlanDTO{Id=Guid.NewGuid(),LoanCaseId=Guid.NewGuid(),CaseNumber=number,CustomerAccountId=account,SourceJournalId=Guid.NewGuid(),PrincipalChartOfAccountId=gl,InterestReceivableChartOfAccountId=interestGl,InterestChargedChartOfAccountId=income,Principal=principal,DisbursementDate=date,IsConfirmed=true,InterestTermsConfirmed=true,Evidence="Approved terms",AllocationPolicy=LoanAgeingEngine.Policy,Instalments=new List<LoanPlanInstalmentDTO>{new LoanPlanInstalmentDTO{Number=1,DueDate=date.AddMonths(1),Principal=principal,Interest=0}}};}
 static LoanAgeingPosting Event(decimal amount,DateTime date,int code,Guid? source=null){return new LoanAgeingPosting{Id=Guid.NewGuid(),JournalId=source??Guid.NewGuid(),CustomerAccountId=account,ChartOfAccountId=gl,Amount=amount,EffectiveDate=date,TransactionCode=code};}
 public static void Run()
 {
  var old=Plan(1,start,100);var replacement=Plan(2,at.Date,80);replacement.IsRestructuring=true;replacement.EffectiveAt=at;replacement.OpeningInterest=0;replacement.PriorRiskCategory=2;replacement.PriorPlanIds=old.Id.ToString();
  var es=new List<LoanAgeingPosting>{Event(100,start,21,old.SourceJournalId),Event(-20,start.AddDays(2),30),Event(-80,at.AddSeconds(-1),23),Event(80,at,23,replacement.SourceJournalId)};
  replacement.OpeningLedgerHash=LoanRestructureAgeing.Hash(es,new List<LoanAgeingPosting>(),at);
  var cs=new List<LoanAgeingCaseDTO>{new LoanAgeingCaseDTO{Id=old.LoanCaseId,CaseNumber=1,Status=48829},new LoanAgeingCaseDTO{Id=replacement.LoanCaseId,CaseNumber=2,Status=48833}};
  var plans=new List<LoanPlanDTO>{old,replacement};
  Func<DateTime,LoanAgeingAccountResult> calculate=date=>LoanRestructureAgeing.Calculate(account,date,cs,plans,es.Where(x=>x.EffectiveDate.Date<=date.Date).ToList(),new List<LoanAgeingPosting>());
  var r=calculate(at.Date);Check(r.Issues.Count==0&&r.OutstandingPrincipal==80&&r.OverduePrincipal==0,"replacement opening is not a second advance");Check(r.PriorRiskCategory==2&&r.IsRestructured,"retained risk history");
  r.CombinedDaysPastDue=0;Check(SasraForm4.Minimum(r,at)==2,"new due dates cannot improve classification");
  var historic=LoanAgeingEngine.Calculate(account,at.Date.AddDays(-1),cs.Take(1).ToList(),plans.Take(1).ToList(),es.Where(x=>x.EffectiveDate<at.Date).ToList());Check(historic.OverduePrincipal==80,"earlier report retains old contractual arrears");
  es.Add(Event(-30,at.AddDays(2),30));r=calculate(at.AddMonths(1).AddDays(1));Check(r.OutstandingPrincipal==50&&r.OverduePrincipal==50,"post-restructure partial payment allocates to replacement only");
  es.Add(Event(10,at.AddDays(3),37));r=calculate(at.AddMonths(1).AddDays(1));Check(r.OutstandingPrincipal==60&&r.OverduePrincipal==60,"refund restores replacement principal");
  es.Add(Event(-1,start.AddDays(3),30));r=calculate(at.AddDays(3));Check(r.Issues.Any(x=>x.Contains("changed"))&&r.DaysPastDue==null,"backdated changes invalidate frozen opening");es.RemoveAt(es.Count-1);
  replacement.IsConfirmed=false;r=calculate(at.AddDays(3));Check(r.DaysPastDue==null,"draft restructure cannot age");replacement.IsConfirmed=true;
  replacement.OpeningInterest=1;r=calculate(at.AddDays(3));Check(r.DaysPastDue==null,"unallocated carried interest blocked");replacement.OpeningInterest=0;
  var savedId=old.Id;old.Id=Guid.NewGuid();r=calculate(at.AddDays(3));Check(r.Issues.Any(x=>x.Contains("previous schedule")),"old schedule correction requires opening re-review");old.Id=savedId;
  cs.Add(new LoanAgeingCaseDTO{Id=Guid.NewGuid(),Status=48833});r=calculate(at.AddDays(3));Check(r.Issues.Any(x=>x.Contains("Multiple")),"repeat restructuring blocked");cs.RemoveAt(cs.Count-1);
  foreach(var boundary in new[]{Tuple.Create(0,0),Tuple.Create(1,1),Tuple.Create(30,1),Tuple.Create(31,2),Tuple.Create(180,2),Tuple.Create(181,3),Tuple.Create(360,3),Tuple.Create(361,4)})Check(LoanRestructureAgeing.Category(boundary.Item1)==boundary.Item2,"regulatory day boundary "+boundary.Item1);
  Check(LoanRestructureAgeing.Category(1,2)==2&&LoanRestructureAgeing.Category(1,7)==3&&LoanRestructureAgeing.Category(1,13)==4,"missed-instalment alternative cannot underclassify");
  var request=new SasraForm4Request{YearStart=start,AsAt=at.Date,InstitutionName="Synthetic SACCO",RegistrationNumber="TEST"};
  var portfolio=new LoanAgeingResult();
  for(int i=0;i<10;i++){var a=new LoanAgeingAccountResult{CustomerAccountId=Guid.NewGuid(),CaseNumbers=new List<int>{i+1},OutstandingPrincipal=1000,CombinedDaysPastDue=new[]{0,1,31,181,361}[i%5],IsRestructured=i>=5,PriorRiskCategory=i>=5?(int?)(i%5):null,Interest=new LoanInterestAgeingResult{DaysPastDue=0,OverdueInterest=0}};portfolio.Accounts.Add(a);}
  portfolio.LedgerPrincipal=10000;portfolio.AccountPrincipal=10000;
  var reviews=portfolio.Accounts.Select((a,i)=>new LoanRiskReviewDTO{CustomerAccountId=a.CustomerAccountId,AsAt=at.Date,Revision=1,RiskCategory=i%5,ProvisioningAdjustment=0,Evidence="Synthetic credit and suspense review",BasisHash=SasraForm4.Basis(a)}).ToList();
  var form=SasraForm4.Build(request,portfolio,reviews);Check(form.Issues.Count==0&&form.Rows.All(x=>x.Accounts==1),"ten sections count each account once");Check(form.TotalExposure==10000&&form.TotalProvision==3620,"all five provision rates, ordinary and restructured");
  using(var stream=new MemoryStream(Convert.FromBase64String(form.WorkbookBase64))){var wb=new HSSFWorkbook(stream);var sheet=wb.GetSheet("Portfolio Analysis");Check(sheet.GetRow(22).GetCell(2).NumericCellValue==10,"official workbook count formula");Check(sheet.GetRow(22).GetCell(5).NumericCellValue==3620,"official workbook provision formula");Check(wb.GetSheet("Loan detail")!=null,"account basis accompanies working export");for(int i=0;i<10;i++){int row=(i<5?9:16)+i%5;Check(sheet.GetRow(row).GetCell(3).NumericCellValue==1000,"official amount cell "+row);}}
  if(Environment.GetEnvironmentVariable("FORM4_TEST_OUTPUT") is string output&&!string.IsNullOrWhiteSpace(output))File.WriteAllBytes(output,Convert.FromBase64String(form.WorkbookBase64));
  reviews[0].ProvisioningAdjustment=100;form=SasraForm4.Build(request,portfolio,reviews);Check(form.TotalExposure==10000&&form.TotalProvision==3620,"UI adjustments do not change system-derived export");
  reviews.RemoveAt(0);form=SasraForm4.Build(request,portfolio,reviews);Check(form.WorkbookBase64!=null&&form.UnresolvedAccounts==0,"dated UI review is not an export prerequisite");
  reviews[0].BasisHash="stale";form=SasraForm4.Build(request,portfolio,reviews);Check(form.UnresolvedAccounts==0,"stale manual review is not applied");
  portfolio.Issues.Add("G/L difference");form=SasraForm4.Build(request,portfolio,reviews);Check(form.WorkbookBase64!=null&&form.Issues.Count>0,"ledger discrepancy is disclosed in working export");
  Check(SasraForm4.Open().GetSheet("Portfolio Analysis")!=null,"embedded workbook checksum verified");
  Console.WriteLine("PASS: "+checks+" restructuring, risk-classification and Form 4 workbook assertions.");
 }
}
