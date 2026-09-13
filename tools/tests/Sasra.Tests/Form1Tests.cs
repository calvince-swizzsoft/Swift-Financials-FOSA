using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
static class Form1Tests
{
 static int checks;
 static void Check(bool ok,string message){if(!ok)throw new Exception("Form 1: "+message);checks++;}
 static void Reject(Action action){try{action();throw new Exception("Invalid capital request accepted");}catch(SasraSetupException){checks++;}}
 public static void Run()
 {
  var d=SasraForm1.Definition();d.Id=Guid.NewGuid();SasraSetupAppService.ValidateVersion(d);SasraSetupAppService.ValidateVersion(d);
  Check(d.Lines.Count(x=>x.Source=="GlBalance")==13,"13 G/L mapping lines");Check(d.Lines.Count(x=>x.Source=="Formula")==14,"14 original formulas in amount column");
  var source=SasraForm6.Definition();source.Id=Guid.NewGuid();source.Revision=2;
  var sofp=new SasraForm6Result{Rows=new List<SasraForm6Row>{new SasraForm6Row{Cell="C38",Amount=20000m},new SasraForm6Row{Cell="C45",Amount=15000m}}};
  var data=new List<SasraForm1Balance>();
  Action<string,int,decimal> add=(cell,type,value)=>{var b=new SasraForm1Balance{Id=Guid.NewGuid(),AccountType=type,Balance=value};data.Add(b);if(cell!=null)d.Lines.Single(x=>x.Cell==cell).AccountIds.Add(b.Id);};
  add("D28",1000,20000000m);add(null,2000,-15000000m);add("D10",3000,-3000000m);add("D12",3000,-1000000m);add(null,4000,-1000000m);
  var profile=new SasraProfileDTO{Profile="DT",InstitutionName="Test SACCO",RegistrationNumber="CS123"};
  var q=new SasraForm1Request{VersionId=d.Id,Form6VersionId=source.Id,YearStart=new DateTime(2026,1,1),AsAt=new DateTime(2026,12,31),SurplusAdjustment=0,InvestmentDeduction=0,OtherDeductions=0,OffBalanceSheetAssets=0,CapitalEligibilityReviewed=true};
  var result=SasraForm1.Build(d,profile,q,data,source,sofp);
  Func<string,decimal?> amount=cell=>result.Rows.Single(x=>x.Cell==cell).Amount;
  Check(result.RawCurrentSurplus==1000000&&result.EligibleCurrentSurplus==500000,"positive surplus receives 50 percent eligibility");
  Check(amount("D22")==4500000&&amount("D23")==1500000,"core and institutional capital");
  Check(amount("D34")==20000000&&amount("D44")==15000000&&result.Difference==0,"Form 6 thousands converted to Form 1 KSh once");
  Check(amount("D45")==0.225m&&amount("D48")==0.075m&&amount("D51")==0.3m,"all three capital ratios");
  Check(result.Warnings.Count==1&&result.Issues.Count==0&&result.WorkbookBase64!=null,"deficiency warning does not prevent an accurate regulatory return");
  Check(result.Form6VersionId==source.Id&&result.Form6Revision==2,"source revision identified");
  var output=new HSSFWorkbook(new MemoryStream(Convert.FromBase64String(result.WorkbookBase64)));var original=SasraForm1.Open();
  var sheet=output.GetSheet(SasraForm1.Sheet);var old=original.GetSheet(SasraForm1.Sheet);
  Check(output.NumberOfSheets==original.NumberOfSheets&&sheet.NumMergedRegions==old.NumMergedRegions,"workbook structure retained");
  for(int row=0;row<=old.LastRowNum;row++)
  {
   var oldRow=old.GetRow(row);if(oldRow==null)continue;
   Check(sheet.GetRow(row).Height==oldRow.Height,"row height retained");
   foreach(var c in oldRow.Cells)
   {
    var actual=sheet.GetRow(row).GetCell(c.ColumnIndex);
    if(c.CellType==CellType.Formula)Check(actual.CellFormula==c.CellFormula&&actual.CachedFormulaResultType==CellType.Numeric,"all D/E/F formulas retained and evaluated");
    else if(c.ColumnIndex!=3)Check(actual.ToString()==c.ToString()&&actual.CellStyle.Index==c.CellStyle.Index,"labels and additional-year values/styles unchanged");
   }
  }
  Check(sheet.GetRow(44).GetCell(3).NumericCellValue==0.225,"ratio cache survives reopen");
  Check(sheet.GetRow(4).GetCell(3).DateCellValue==q.YearStart&&sheet.GetColumnWidth(3)==old.GetColumnWidth(3),"typed date and column width");
  var mappedRequest=new SasraForm1Request{VersionId=d.Id,Form6VersionId=source.Id,YearStart=q.YearStart,AsAt=q.AsAt,MappingBased=true};
  var mapped=SasraForm1.Build(d,profile,mappedRequest,data,source,sofp);
  Check(mapped.WorkbookBase64!=null&&mapped.Issues.Count==0,"mapping-based export needs no manual inputs or false review attestation");
  Check(mapped.Warnings.Any(x=>x.Contains("without additional")),"mapping-based scope disclosed");
  var editable=new HSSFWorkbook(new MemoryStream(Convert.FromBase64String(mapped.WorkbookBase64)));var editSheet=editable.GetSheet(SasraForm1.Sheet);
  Check(!editSheet.Protect,"export is editable");
  editSheet.GetRow(18).GetCell(3).SetCellValue(100000);editSheet.GetRow(19).GetCell(3).SetCellValue(50000);editSheet.GetRow(36).GetCell(3).SetCellValue(1000000);editSheet.GetRow(12).GetCell(3).SetCellValue(400000);
  new HSSFFormulaEvaluator(editable).EvaluateAll();
  Check(editSheet.GetRow(21).GetCell(3).NumericCellValue==4250000&&editSheet.GetRow(42).GetCell(3).NumericCellValue==21000000,"Excel adjustments recalculate capital and exposure totals");
  editSheet.GetRow(27).GetCell(3).SetCellValue(19000000);new HSSFFormulaEvaluator(editable).EvaluateAll();
  Check(editSheet.GetRow(40).GetCell(3).NumericCellValue==19000000,"Excel asset edits recalculate the denominator");
  mappedRequest.OtherDeductions=1;Reject(()=>SasraForm1.ValidateRequest(mappedRequest));mappedRequest.OtherDeductions=null;
  sofp.Issues.Add("Invalid ledger");Check(SasraForm1.Build(d,profile,mappedRequest,data,source,sofp).WorkbookBase64==null,"mapping-based mode preserves source validation");sofp.Issues.Clear();
  File.WriteAllBytes(Path.Combine(Path.GetTempPath(),"sasra-capital-mapping.xls"),Convert.FromBase64String(mapped.WorkbookBase64));
  q.SurplusAdjustment=-200000;q.ReviewNotes="Proposed dividends";result=SasraForm1.Build(d,profile,q,data,source,sofp);Check(result.EligibleCurrentSurplus==400000,"adjustment precedes profit eligibility fraction");
  q.SurplusAdjustment=0;q.ReviewNotes=null;data[4].Balance=2000000;data[2].Balance=-6000000;
  result=SasraForm1.Build(d,profile,q,data,source,sofp);Check(result.EligibleCurrentSurplus==-2000000,"full loss included rather than halved");
  data[4].Balance=0;data[4].CurrentClosingBalance=1000000;data[2].Balance=-3000000;data[3].Balance=-2000000;data[3].CurrentClosingBalance=-1000000;
  result=SasraForm1.Build(d,profile,q,data,source,sofp);Check(amount("D12")==1000000&&amount("D22")==4500000,"current-year closing transfers do not double-count profits in retained earnings");
  q.InvestmentDeduction=100000;q.OtherDeductions=50000;q.OffBalanceSheetAssets=1000000;q.ReviewNotes="Investment and guarantees schedules";
  result=SasraForm1.Build(d,profile,q,data,source,sofp);Check(amount("D22")==4350000&&amount("D43")==21000000,"deductions reduce capital and exposures increase denominator");
  q.CapitalEligibilityReviewed=false;result=SasraForm1.Build(d,profile,q,data,source,sofp);Check(result.WorkbookBase64==null,"unreviewed eligibility blocks download");q.CapitalEligibilityReviewed=true;
  sofp.Issues.Add("Unmapped share capital");result=SasraForm1.Build(d,profile,q,data,source,sofp);Check(result.WorkbookBase64==null&&result.Issues.Any(x=>x.Contains("Form 6:")),"source Form 6 issues propagate");sofp.Issues.Clear();
  sofp.Rows[0].Amount=19000;result=SasraForm1.Build(d,profile,q,data,source,sofp);Check(result.Difference==1000000&&result.WorkbookBase64==null,"independent SOFP asset comparison blocks mismatch");sofp.Rows[0].Amount=20000;
  sofp.Rows[1].Amount=0;result=SasraForm1.Build(d,profile,q,data,source,sofp);Check(amount("D51")==null&&result.WorkbookBase64==null,"zero denominator unavailable, not a healthy zero ratio");sofp.Rows[1].Amount=15000;
  q.OffBalanceSheetAssets=-1;Reject(()=>SasraForm1.ValidateRequest(q));q.OffBalanceSheetAssets=null;Reject(()=>SasraForm1.ValidateRequest(q));q.OffBalanceSheetAssets=0;
  q.SurplusAdjustment=decimal.MinValue;Reject(()=>SasraForm1.ValidateRequest(q));q.SurplusAdjustment=decimal.MaxValue;Reject(()=>SasraForm1.ValidateRequest(q));q.SurplusAdjustment=0;
  q.ReviewNotes="";Reject(()=>SasraForm1.ValidateRequest(q));q.ReviewNotes="Supporting schedules";
  q.AsAt=new DateTime(2027,1,1);Reject(()=>SasraForm1.ValidateRequest(q));q.AsAt=new DateTime(2026,12,31);
  Check(!SasraForm1.AllowsAccountType("D10",2000)&&SasraForm1.AllowsAccountType("D10",3000)&&SasraForm1.AllowsAccountType("D29",1000),"capital and asset mapping categories");
  var bad=SasraForm1.Definition();bad.Lines.Single(x=>x.Cell=="D13").AccountIds.Add(Guid.NewGuid());Reject(()=>SasraSetupAppService.ValidateVersion(bad));
  bad=SasraForm1.Definition();bad.Lines.Single(x=>x.Cell=="D10").Sign=1;Reject(()=>SasraSetupAppService.ValidateVersion(bad));
  bad=SasraForm1.Definition();var id=Guid.NewGuid();bad.Lines.Single(x=>x.Cell=="D28").AccountIds.Add(id);bad.Lines.Single(x=>x.Cell=="D29").AccountIds.Add(id);Reject(()=>SasraSetupAppService.ValidateVersion(bad));
  Console.WriteLine("Form 1: "+checks+" assertions passed.");
 }
}
