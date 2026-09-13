using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
static class Form2Tests
{
 static int checks;
 static void Check(bool ok,string message){if(!ok)throw new Exception("Form 2: "+message);checks++;}
 static Guid cash=Guid.NewGuid(),bank=Guid.NewGuid(),overdraft=Guid.NewGuid(),deposit=Guid.NewGuid(),capital=Guid.NewGuid();
 static SasraVersionDTO Definition(){var d=SasraForm2.Definition();d.Id=Guid.NewGuid();d.Lines.Single(x=>x.Cell=="D9").AccountIds.Add(cash);d.Lines.Single(x=>x.Cell=="D13").AccountIds.AddRange(new[]{bank,overdraft});d.Lines.Single(x=>x.Cell=="D33").AccountIds.Add(deposit);return d;}
 static SasraVersionDTO Sofp(){var d=SasraForm6.Definition();d.Id=Guid.NewGuid();d.Lines.Single(x=>x.Cell=="C11").AccountIds.Add(cash);d.Lines.Single(x=>x.Cell=="C12").AccountIds.AddRange(new[]{bank,overdraft});d.Lines.Single(x=>x.Cell=="C42").AccountIds.Add(deposit);return d;}
 static SasraForm2Request Input(){return new SasraForm2Request{VersionId=Guid.NewGuid(),Form6VersionId=Guid.NewGuid(),YearStart=new DateTime(2026,1,1),AsAt=new DateTime(2026,9,12),ManualAmounts=SasraForm2.ManualCells.ToDictionary(x=>x,x=>(decimal?)0),Exclusions=SasraForm2.ExclusionCells.ToDictionary(x=>x,x=>(decimal?)0),LiquidityReviewed=true,ReviewNotes="Bank reconciliation, deposits, restriction and maturity schedules reviewed."};}
 static List<SasraForm6Balance> Balances(){return new List<SasraForm6Balance>{new SasraForm6Balance{Id=cash,AccountType=1000,Balance=100},new SasraForm6Balance{Id=bank,AccountType=1000,Balance=500},new SasraForm6Balance{Id=overdraft,AccountType=1000,Balance=-200},new SasraForm6Balance{Id=deposit,AccountType=2000,Balance=-1000},new SasraForm6Balance{Id=capital,AccountType=3000,Balance=600}};}
 static SasraForm2Result Build(SasraForm2Request i=null,List<SasraForm6Balance> b=null,SasraVersionDTO d=null,SasraVersionDTO s=null){return SasraForm2.Build(d??Definition(),new SasraProfileDTO{Profile="DT",RegistrationNumber="TEST",InstitutionName="Fixture SACCO"},i??Input(),b??Balances(),s??Sofp());}
 static decimal Amount(SasraForm2Result r,string c){return r.Rows.Single(x=>x.Cell==c).Amount.Value;}
 static void Reject(Action<SasraForm2Request> action,string field){var i=Input();action(i);try{SasraForm2.ValidateRequest(i);throw new Exception("Invalid input accepted");}catch(SasraSetupException e){Check(e.Field==field,field+" useful validation");}}
 public static void Run()
 {
  var d=Definition();SasraSetupAppService.ValidateVersion(d);Check(d.Lines.Count(x=>x.Source=="GlBalance")==11,"eleven mapping lines");
  foreach(var l in d.Lines.Where(x=>x.Source=="GlBalance")){Check(SasraForm2.AllowsAccountType(l.Cell,l.Sign<0?2000:1000),"account classification "+l.Cell);Check(!SasraForm2.AllowsAccountType(l.Cell,3000),"equity excluded "+l.Cell);}
  var r=Build();Check(r.Issues.Count==0,string.Join(";",r.Issues));Check(Amount(r,"D13")==500&&Amount(r,"D16")==200,"banks split individually");Check(Amount(r,"D30")==400&&Amount(r,"D51")==0.4m,"whole KSh ratio");Check(r.Difference==0&&r.LedgerDifference==0,"deposit and ledger reconciliation");Check(r.WorkbookBase64!=null,"valid export");
  var mappedInput=new SasraForm2Request{VersionId=Guid.NewGuid(),Form6VersionId=Guid.NewGuid(),YearStart=new DateTime(2026,1,1),AsAt=new DateTime(2026,9,12),MappingBased=true,ManualAmounts=null,Exclusions=null};
  var mapped=Build(mappedInput);Check(mapped.Issues.Count==0&&mapped.WorkbookBase64!=null,"mapping export needs no schedule inputs or review attestation");
  Check(Amount(mapped,"D16")==200&&Amount(mapped,"D30")==400,"automatic bank overdraft retained in mapping mode");
  Check(mapped.Warnings.Any(x=>x.Contains("without additional")),"mapping-only scope disclosed");
  var edited=new HSSFWorkbook(new MemoryStream(Convert.FromBase64String(mapped.WorkbookBase64)));var editable=edited.GetSheet(SasraForm2.Sheet);Check(!editable.Protect,"mapping workbook editable");
  editable.GetRow(14).GetCell(3).SetCellValue(50);editable.GetRow(15).GetCell(3).SetCellValue(220);editable.GetRow(43).GetCell(3).SetCellValue(70);editable.GetRow(44).GetCell(3).SetCellValue(30);
  new HSSFFormulaEvaluator(edited).EvaluateAll();
  Check(editable.GetRow(29).GetCell(3).NumericCellValue==330&&editable.GetRow(49).GetCell(3).NumericCellValue==1100,"Excel maturity and obligation edits update totals");
  Check(Math.Abs(editable.GetRow(50).GetCell(3).NumericCellValue-0.3)<1e-12,"Excel liquidity ratio recalculates");
  var unbalanced=Balances();unbalanced[0].Balance+=1;Check(Build(mappedInput,unbalanced).WorkbookBase64==null,"mapping mode retains ledger checks");
  mappedInput.ManualAmounts=new Dictionary<string,decimal?>{{"D15",1}};try{SasraForm2.ValidateRequest(mappedInput);throw new Exception("Nonzero manual amount silently accepted");}catch(SasraSetupException){Check(true,"nonzero schedules rejected in mapping mode");}
  File.WriteAllBytes(Path.Combine(Path.GetTempPath(),"sasra-liquidity-mapping.xls"),Convert.FromBase64String(mapped.WorkbookBase64));
  var original=SasraForm2.Open();var wb=new HSSFWorkbook(new MemoryStream(Convert.FromBase64String(r.WorkbookBase64)));var a=original.GetSheet(SasraForm2.Sheet);var b=wb.GetSheet(SasraForm2.Sheet);
  for(int n=7;n<53;n++)for(int col=0;col<4;col++){var x=a.GetRow(n)?.GetCell(col);var y=b.GetRow(n)?.GetCell(col);if(x==null)continue;Check(y!=null&&x.CellStyle.DataFormat==y.CellStyle.DataFormat,"preserved format");if(x.CellType==CellType.Formula)Check(y.CellFormula==(n==42&&col==3?"SUM(D44:D45)":x.CellFormula),"formula preservation");if(x.CellType==CellType.String)Check(x.StringCellValue==y.StringCellValue,"label preservation");}
  var evaluator=new HSSFFormulaEvaluator(wb);evaluator.EvaluateAll();Check(Math.Abs(b.GetRow(50).GetCell(3).NumericCellValue-0.4)<1e-12,"export recalculates");
  var input=Input();input.ManualAmounts["D44"]=70;input.ManualAmounts["D45"]=30;r=Build(input);Check(Amount(r,"D43")==100&&Amount(r,"D46")==100&&Amount(r,"D50")==1100,"no double counting other liabilities");
  input=Input();input.Exclusions["D13"]=50;input.ManualAmounts["D15"]=100;input.ManualAmounts["D16"]=20;r=Build(input);Check(Amount(r,"D30")==230,"exclusions, maturity and additional bank obligations deducted once");
  input=Input();input.ManualAmounts["D37"]=100;r=Build(input);Check(Amount(r,"D41")==900&&Amount(r,"D50")==1000,"ratio uses official gross-deposit denominator");
  input=Input();input.Exclusions["D33"]=10;r=Build(input);Check(r.Difference==0&&Amount(r,"D35")==990,"gross reconciliation precedes uncleared deposit exclusion");
  var balances=Balances();balances.Single(x=>x.Id==bank).Balance=-500;balances.Single(x=>x.Id==capital).Balance=1600;r=Build(null,balances);Check(Amount(r,"D30")==-600&&Amount(r,"D51")==-0.6m&&r.WorkbookBase64!=null,"negative liquidity remains reportable");
  input=Input();input.LiquidityReviewed=false;Check(Build(input).WorkbookBase64==null,"review required");
  input=Input();input.Exclusions["D13"]=501;Check(Build(input).Issues.Any(x=>x.Contains("exclusions")),"exclusion bounds");
  input=Input();input.ManualAmounts["D15"]=501;Check(Build(input).Issues.Any(x=>x.Contains("Time deposits")),"time deposits bounded");
  input=Input();input.ManualAmounts["D38"]=1001;Check(Build(input).Issues.Any(x=>x.Contains("Deposit deductions")),"deposit deductions bounded");
  balances=Balances();balances.Single(x=>x.Id==deposit).Balance=0;balances.Single(x=>x.Id==capital).Balance=-400;r=Build(null,balances);Check(r.Rows.Single(x=>x.Cell=="D51").Amount==null&&r.WorkbookBase64==null,"zero denominator unavailable");
  balances=Balances();balances[0].Balance+=1;Check(Build(null,balances).WorkbookBase64==null,"unbalanced ledger blocked");
  d=Definition();d.Lines.Single(x=>x.Cell=="D33").AccountIds.Clear();r=Build(null,null,d);Check(r.UnmappedAccounts.Any(x=>x.Id==deposit)&&r.Issues.Any(x=>x.Contains("reconcile")),"missing deposit mapping detected");
  Reject(i=>i.ManualAmounts=null,"ManualAmounts");Reject(i=>i.ManualAmounts["D15"]=null,"ManualAmounts");Reject(i=>i.Exclusions["D13"]=-1,"Exclusions");Reject(i=>i.ManualAmounts["D24"]=decimal.MinValue,"ManualAmounts");Reject(i=>i.ManualAmounts["D24"]=decimal.MaxValue,"ManualAmounts");Reject(i=>i.Exclusions["D99"]=0,"Exclusions");Reject(i=>i.ReviewNotes=" ","ReviewNotes");Reject(i=>i.Form6VersionId=Guid.Empty,"VersionId");Reject(i=>i.AsAt=new DateTime(2027,1,1),"AsAt");
  foreach(var l in SasraForm2.Definition().Lines){var bad=SasraForm2.Definition();bad.Lines.Single(x=>x.Code==l.Code).Sign*=-1;try{SasraSetupAppService.ValidateVersion(bad);throw new Exception("modified sign accepted");}catch(SasraSetupException){Check(true,"canonical sign "+l.Code);}}
  File.WriteAllBytes(Path.Combine(Path.GetTempPath(),"sasra-form2-fixture.xls"),Convert.FromBase64String(Build().WorkbookBase64));
  Console.WriteLine("PASS Form 2: "+checks+" assertions.");
 }
}
