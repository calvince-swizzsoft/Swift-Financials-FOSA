using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
static class Form5Tests
{
 static int checks;
 static void Check(bool ok,string label){if(!ok)throw new Exception("Form 5: "+label);checks++;}
 static Guid investment=Guid.NewGuid(),land=Guid.NewGuid(),other=Guid.NewGuid();
 static SasraVersionDTO Definition(){var d=SasraForm5.Definition();d.Id=Guid.NewGuid();d.Revision=1;d.Lines.Single(x=>x.Cell=="C11").AccountIds.Add(other);d.Lines.Single(x=>x.Cell=="C12").AccountIds.Add(investment);d.Lines.Single(x=>x.Cell=="C13").AccountIds.Add(land);return d;}
 static SasraVersionDTO Sofp(){var d=SasraForm6.Definition();d.Id=Guid.NewGuid();d.Revision=2;d.Lines.Single(x=>x.Cell=="C20").AccountIds.Add(investment);d.Lines.Single(x=>x.Cell=="C33").AccountIds.AddRange(new[]{land,other});return d;}
 static SasraForm1Result Capital(decimal core=5000000){return new SasraForm1Result{Rows=new List<SasraForm6Row>{new SasraForm6Row{Cell="D22",Amount=core},new SasraForm6Row{Cell="D34",Amount=20000000},new SasraForm6Row{Cell="D44",Amount=15000000}}};}
 static SasraForm5Request Input(){return new SasraForm5Request{VersionId=Guid.NewGuid(),Form1VersionId=Guid.NewGuid(),Form6VersionId=Guid.NewGuid(),YearStart=new DateTime(2026,1,1),AsAt=new DateTime(2026,9,30)};}
 static List<SasraForm1Balance> Balances(){return new List<SasraForm1Balance>{new SasraForm1Balance{Id=investment,AccountType=1000,Balance=1000000},new SasraForm1Balance{Id=land,AccountType=1000,Balance=600000},new SasraForm1Balance{Id=other,AccountType=1000,Balance=400000},new SasraForm1Balance{Id=Guid.NewGuid(),AccountType=1000,Balance=18000000},new SasraForm1Balance{Id=Guid.NewGuid(),AccountType=2000,Balance=-15000000},new SasraForm1Balance{Id=Guid.NewGuid(),AccountType=3000,Balance=-5000000}};}
 static SasraForm5Result Build(SasraVersionDTO d=null,List<SasraForm1Balance> balances=null,SasraForm1Result capital=null,SasraForm5Request input=null){return SasraForm5.Build(d??Definition(),new SasraProfileDTO{Profile="DT",InstitutionName="Test SACCO",RegistrationNumber="CS123"},input??Input(),balances??Balances(),Sofp(),capital??Capital(),3);}
 static decimal? Amount(SasraForm5Result r,string cell){return r.Rows.Single(x=>x.Cell==cell).Amount;}
 public static void Run(){
  var d=Definition();SasraSetupAppService.ValidateVersion(d);Check(d.Lines.Count==18,"18 canonical report lines");
  var r=Build();Check(r.Issues.Count==0&&r.WorkbookBase64!=null,"Valid export");
  Check(Amount(r,"C8")==5000000&&Amount(r,"C9")==20000000&&Amount(r,"C10")==15000000,"Source figures retain KSh units");
  Check(Amount(r,"C15")==0.03m&&Amount(r,"C16")==0.05m&&Amount(r,"C17")==-0.02m,"Missing source formulas repaired");
  Check(Amount(r,"C19")==0.2m&&Amount(r,"C21")==-0.2m&&Amount(r,"C26")==0.02m,"Ratio numerators and denominators");
  Check(Math.Abs(Amount(r,"C23").Value-1m/15m)<0.000000001m&&r.Warnings.Any(x=>x.Contains("exceeds")),"Excess remains downloadable");
  r=Build(capital:Capital(0));Check(Amount(r,"C19")==null&&Amount(r,"C21")==null&&r.WorkbookBase64!=null,"Zero core capital is unavailable, not zero ratio");
  r=Build(capital:Capital(-1));Check(Amount(r,"C19")==null,"Negative core capital cannot give misleading ratio");
  var emptyMappedAccount=Guid.NewGuid();d=Definition();d.Lines.Single(x=>x.Cell=="C11").AccountIds.Add(emptyMappedAccount);
  var emptySofp=Sofp();emptySofp.Lines.Single(x=>x.Cell=="C33").AccountIds.Add(emptyMappedAccount);
  var emptyResult=SasraForm5.Build(d,new SasraProfileDTO{Profile="DT",InstitutionName="Test",RegistrationNumber="CS123"},Input(),Balances(),emptySofp,Capital(),3);
  Check(emptyResult.Issues.Count==0&&emptyResult.WorkbookBase64!=null&&Amount(emptyResult,"C11")==400000,"Valid mapped account without postings contributes zero");
  var b=Balances();b[0].Balance+=1;Check(Build(balances:b).WorkbookBase64==null,"Unbalanced ledger blocks export");
  d=Definition();d.Lines.Single(x=>x.Cell=="C13").AccountIds.Clear();r=Build(d);Check(r.UnmappedAccounts.Any(x=>x.Id==land)&&r.WorkbookBase64==null,"Unclassified property blocks export");
  d=Definition();d.Lines.Single(x=>x.Cell=="C11").AccountIds.Add(land);try{SasraSetupAppService.ValidateVersion(d);throw new Exception("Duplicate accepted");}catch(SasraSetupException){Check(true,"Duplicate mappings rejected");}
  var source=Capital();source.Issues.Add("Missing SOFP account");Check(Build(capital:source).WorkbookBase64==null,"Source validation propagated");
  foreach(var line in SasraForm5.Definition().Lines){foreach(var field in new[]{"sign","cell","source","label"}){d=Definition();var x=d.Lines.Single(l=>l.Code==line.Code);if(field=="sign")x.Sign=-1;if(field=="cell")x.Cell="C99";if(field=="source")x.Source="Manual";if(field=="label")x.Description="changed";try{SasraSetupAppService.ValidateVersion(d);throw new Exception("Template changed");}catch(SasraSetupException){Check(true,"Canonical "+field);}}}
  foreach(var field in new[]{"date","source"}){var q=Input();if(field=="date")q.AsAt=q.YearStart.AddYears(1);else q.Form1VersionId=Guid.Empty;try{SasraForm5.ValidateRequest(q);throw new Exception("Invalid input accepted");}catch(SasraSetupException){Check(true,"Required source and dates");}}
  foreach(var date in new[]{new DateTime(2026,3,31),new DateTime(2026,6,30),new DateTime(2026,9,30),new DateTime(2026,12,31)}){var q=Input();q.AsAt=date;SasraForm5.ValidateRequest(q);Check(true,"Quarter end accepted");}
  foreach(var purpose in new[]{"Quarterly","invalid",null}){var q=Input();q.ReportPurpose=purpose;q.AsAt=new DateTime(2026,9,16);try{SasraForm5.ValidateRequest(q);throw new Exception("Invalid purpose/date accepted");}catch(SasraSetupException){Check(true,"Invalid quarterly date or purpose rejected");}}
  var interim=Input();interim.ReportPurpose="Interim";interim.AsAt=new DateTime(2026,9,16);var interimResult=Build(input:interim);
  Check(interimResult.ReportPurpose=="Interim"&&interimResult.WorkbookBase64!=null,"Interim export allowed");
  var interimBook=new HSSFWorkbook(new MemoryStream(Convert.FromBase64String(interimResult.WorkbookBase64)));
  Check(interimBook.GetSheet(SasraForm5.Sheet).GetRow(0).GetCell(1).StringCellValue.Contains("INTERIM WORKING COPY"),"Visible interim workbook label");
  File.WriteAllBytes(Path.Combine(Path.GetTempPath(),"sasra-form5-interim.xls"),Convert.FromBase64String(interimResult.WorkbookBase64));
  r=Build();Check(r.ReportPurpose=="Quarterly","Default quarterly purpose");{var wb=new HSSFWorkbook(new MemoryStream(Convert.FromBase64String(r.WorkbookBase64)));
   var sheet=wb.GetSheet(SasraForm5.Sheet);Check(sheet.GetRow(2).GetCell(2).StringCellValue=="Test SACCO","Institution heading");
   foreach(IRow row in sheet){foreach(var cell in row.Cells)if(cell.CellType==CellType.Formula)Check(cell.CachedFormulaResultType!=CellType.Error,"No cached formula errors");}
   sheet.GetRow(12).GetCell(2).SetCellValue(2000000d);new HSSFFormulaEvaluator(wb).EvaluateAll();Check(Math.Abs(sheet.GetRow(14).GetCell(2).NumericCellValue-0.1)<0.000001,"Excel adjustments recalculate land ratio");
  }
  File.WriteAllBytes(Path.Combine(Path.GetTempPath(),"sasra-form5-fixture.xls"),Convert.FromBase64String(r.WorkbookBase64));
  Console.WriteLine("PASS Form 5: "+checks+" assertions.");
 }
}
