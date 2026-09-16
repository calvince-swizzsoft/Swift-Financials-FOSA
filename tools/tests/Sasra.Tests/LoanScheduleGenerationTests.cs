using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.AccountsModule.Services;
using NPOI.HSSF.UserModel;
static class LoanScheduleGenerationTests
{
 static int checks;
 static void Check(bool value,string message){if(!value)throw new Exception(message);checks++;}
 static LoanPlanDTO Plan(){return new LoanPlanDTO{Principal=100000,DisbursementDate=new DateTime(2024,1,31)};}
 static void Reject(Action action,string label){try{action();throw new Exception("Expected rejection: "+label);}catch(LoanAgeingException){checks++;}}
 public static void Run()
 {
  var p=Plan();var proposal=LoanScheduleGeneration.Generate(p,3,12,0,0,512,12);
  Check(p.Instalments.Count==3,"3 monthly instalments");Check(p.Instalments[0].DueDate==new DateTime(2024,2,29)&&p.Instalments[1].DueDate==new DateTime(2024,3,31)&&p.Instalments[2].DueDate==new DateTime(2024,4,30),"historical end-of-period dates retain month anchor");
  Check(p.Instalments.Sum(x=>x.Principal)==100000,"principal cents reconcile");Check(p.Instalments[0].Interest==1000&&p.Instalments[1].Interest==666.67m,"reducing balance interest");
  LoanScheduleGeneration.Generate(p,3,12,5,1,512,12);Check(p.Instalments[0].DueDate==new DateTime(2024,2,5),"beginning of period plus grace");
  LoanScheduleGeneration.Generate(p,12,4,0,0,512,0);Check(p.Instalments.Count==4&&p.Instalments[0].DueDate==new DateTime(2024,4,30),"four payments per year means three calendar months");
  LoanScheduleGeneration.Generate(p,12,52,0,0,512,0);Check(p.Instalments[0].DueDate==new DateTime(2024,2,7)&&p.Instalments.Last().DueDate==new DateTime(2025,1,29),"weekly dates anchored historically");
  LoanScheduleGeneration.Generate(p,1,12,0,0,516,.4);Check(p.Instalments[0].Interest==33.33m,"one-month fixed annual rate");
  LoanScheduleGeneration.Generate(p,12,12,0,0,515,12);Check(p.Instalments.Sum(x=>x.Principal)==100000&&p.Instalments.Last().Principal>p.Instalments[0].Principal,"diminishing amortization reconciles");
  LoanScheduleGeneration.Upfront(p,650);Check(p.Instalments.Sum(x=>x.Interest)==650&&p.Instalments[0].InterestDueDate==p.DisbursementDate&&p.Instalments.Skip(1).All(x=>x.Interest==0&&x.InterestDueDate==null),"upfront interest counted once");
  Reject(()=>LoanScheduleGeneration.Generate(Plan(),1,52,0,0,512,12),"fractional instalment term");Reject(()=>LoanScheduleGeneration.Generate(Plan(),12,0,0,0,512,12),"missing frequency");Reject(()=>LoanScheduleGeneration.Generate(Plan(),12,12,0,0,999,12),"unsupported method");Reject(()=>LoanScheduleGeneration.Generate(Plan(),12,12,0,0,512,double.NaN),"invalid rate");
  var bad=Plan();bad.DisbursementDate=new DateTime(9998,12,31);Reject(()=>LoanScheduleGeneration.Generate(bad,12,12,0,0,512,12),"date limit");
  var request=new SasraForm4Request{YearStart=new DateTime(2026,1,1),AsAt=new DateTime(2026,9,13),InstitutionName="Synthetic test SACCO",RegistrationNumber="TEST"};
  var portfolio=new LoanAgeingResult{LedgerPrincipal=10000,AccountPrincipal=10000};
  for(int i=0;i<10;i++)portfolio.Accounts.Add(new LoanAgeingAccountResult{CustomerAccountId=Guid.NewGuid(),CaseNumbers=new List<int>{i+1},OutstandingPrincipal=1000,CombinedDaysPastDue=new[]{0,1,31,181,361}[i%5],IsRestructured=i>=5,PriorRiskCategory=i>=5?(int?)(i%5):null,Interest=new LoanInterestAgeingResult{DaysPastDue=0,OverdueInterest=0}});
  var oldReviews=portfolio.Accounts.Select(a=>new LoanRiskReviewDTO{CustomerAccountId=a.CustomerAccountId,AsAt=request.AsAt,RiskCategory=4,ProvisioningAdjustment=999,Evidence="Old adjustment",BasisHash="stale"}).ToList();
  var form=SasraForm4.Build(request,portfolio,oldReviews);Check(form.TotalExposure==10000&&form.TotalProvision==3620&&form.UnresolvedAccounts==0,"derived classification ignores UI adjustments and stale review prerequisites");
  using(var stream=new MemoryStream(Convert.FromBase64String(form.WorkbookBase64)))
  {
   var wb=new HSSFWorkbook(stream);var sheet=wb.GetSheet("Portfolio Analysis");var detail=wb.GetSheet("Loan detail");
   Check(sheet.GetRow(22).GetCell(2).NumericCellValue==10&&sheet.GetRow(22).GetCell(5).NumericCellValue==3620,"ten categories and official total formulas");
   Check(detail.LastRowNum==10&&wb.GetSheet("Report notes")!=null,"all accounts and report basis retained");
   detail.GetRow(1).GetCell(8).SetCellValue(100);new HSSFFormulaEvaluator(wb).EvaluateAll();Check(sheet.GetRow(22).GetCell(3).NumericCellValue==10100&&sheet.GetRow(22).GetCell(5).NumericCellValue==3621,"Excel exposure adjustment recalculates return");
   detail.GetRow(1).GetCell(7).SetCellValue("Watch");new HSSFFormulaEvaluator(wb).EvaluateAll();Check(sheet.GetRow(9).GetCell(2).NumericCellValue==0&&sheet.GetRow(10).GetCell(2).NumericCellValue==2&&sheet.GetRow(22).GetCell(5).NumericCellValue==3665,"Excel category adjustment moves counts and exposure");
   detail.GetRow(1).GetCell(10).SetCellValue(0);new HSSFFormulaEvaluator(wb).EvaluateAll();Check(sheet.GetRow(22).GetCell(2).NumericCellValue==9&&sheet.GetRow(22).GetCell(3).NumericCellValue==9000,"exclude account updates both count and exposure");
  }
  portfolio.Accounts[0].CombinedDaysPastDue=null;portfolio.Accounts[0].Interest.Issues.Add("Missing interest terms");portfolio.Issues.Add("G/L difference requires review");
  form=SasraForm4.Build(request,portfolio,new List<LoanRiskReviewDTO>());Check(form.IsWorkingCopy&&form.WorkbookBase64!=null&&form.UnresolvedAccounts==1&&form.UnclassifiedPrincipal==1000,"incomplete data exports as working copy without inventing a category");
  using(var stream=new MemoryStream(Convert.FromBase64String(form.WorkbookBase64))){var wb=new HSSFWorkbook(stream);Check(wb.GetSheet("Loan detail").LastRowNum==10&&wb.GetSheet("Loan detail").GetRow(1).GetCell(7).CellType==NPOI.SS.UserModel.CellType.Blank,"unresolved account kept with blank editable category");Check(wb.GetSheet("Portfolio Analysis").GetRow(1).GetCell(1).StringCellValue.Contains("WORKING COPY"),"working status visible on return");}
  request.InstitutionName="";Check(SasraForm4.Build(request,portfolio,new List<LoanRiskReviewDTO>()).WorkbookBase64==null,"institution required before export");
  Console.WriteLine("PASS: "+checks+" historical generation and staged Form 4 Excel assertions.");
 }
}
