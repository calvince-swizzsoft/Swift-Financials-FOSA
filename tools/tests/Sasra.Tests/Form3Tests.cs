using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
static class Form3Tests
{
 static int checks;
 static void Check(bool ok,string message){if(!ok)throw new Exception("Form 3: "+message);checks++;}
 static Guid nw=Guid.NewGuid(),savings=Guid.NewGuid(),term=Guid.NewGuid(),interest=Guid.NewGuid();
 static SasraVersionDTO Definition(){var d=SasraForm3.Definition();d.Id=Guid.NewGuid();d.Lines[0].AccountIds.Add(nw);d.Lines[1].AccountIds.AddRange(new[]{savings,interest});d.Lines[2].AccountIds.Add(term);return d;}
 static SasraVersionDTO Sofp(){var d=SasraForm6.Definition();d.Id=Guid.NewGuid();d.Lines.Single(x=>x.Cell=="C44").AccountIds.Add(nw);d.Lines.Single(x=>x.Cell=="C42").AccountIds.AddRange(new[]{savings,interest});d.Lines.Single(x=>x.Cell=="C43").AccountIds.Add(term);return d;}
 static SasraForm3Request Input(){return new SasraForm3Request{VersionId=Guid.NewGuid(),Form6VersionId=Guid.NewGuid(),YearStart=new DateTime(2026,1,1),AsAt=new DateTime(2026,9,13)};}
 static SasraForm3AccountBalance Account(Guid gl,decimal amount,Guid? customer=null){return new SasraForm3AccountBalance{ChartOfAccountId=gl,CustomerAccountId=customer??Guid.NewGuid(),HasCustomerAccount=true,Balance=-amount};}
 static List<SasraForm3AccountBalance> Accounts(){return new List<SasraForm3AccountBalance>{Account(nw,49999.99m),Account(savings,100000),Account(term,300000),Account(term,1000000),Account(term,1000000.01m)};}
 static List<SasraForm6Balance> Ledger(List<SasraForm3AccountBalance> a){var b=a.GroupBy(x=>x.ChartOfAccountId).Select(g=>new SasraForm6Balance{Id=g.Key,AccountType=2000,Balance=g.Sum(x=>x.Balance)}).ToList();b.Add(new SasraForm6Balance{Id=Guid.NewGuid(),AccountType=1000,Balance=-b.Sum(x=>x.Balance)});return b;}
 static SasraForm3Result Build(List<SasraForm3AccountBalance> a=null,SasraVersionDTO d=null,List<SasraForm6Balance> b=null){a=a??Accounts();return SasraForm3.Build(d??Definition(),new SasraProfileDTO{RegistrationNumber="TEST",InstitutionName="Fixture SACCO"},Input(),a,b??Ledger(a),Sofp());}
 public static void Run()
 {
  SasraSetupAppService.ValidateVersion(Definition());
  foreach(var pair in new[]{Tuple.Create(.01m,0),Tuple.Create(49999.99m,0),Tuple.Create(50000m,1),Tuple.Create(100000m,1),Tuple.Create(100000.01m,2),Tuple.Create(300000m,2),Tuple.Create(300000.01m,3),Tuple.Create(1000000m,3),Tuple.Create(1000000.01m,4)})Check(SasraForm3.Band(pair.Item1)==pair.Item2,"exact band boundary "+pair.Item1);
  foreach(var amount in new[]{0m,-1m})try{SasraForm3.Band(amount);throw new Exception("Nonpositive accepted");}catch(ArgumentOutOfRangeException){Check(true,"nonpositive not banded");}
  var r=Build();Check(r.Issues.Count==0,string.Join(";",r.Issues));Check(r.DepositRows.Count==15&&r.TotalAccounts==5,"all bands and account counts");Check(r.TotalDeposits==2450000&&r.DepositRows.Sum(x=>x.Amount)==2450,"KSh thousands conversion preserves cents");Check(r.Difference==0&&r.Form6Difference==0&&r.LedgerDifference==0,"three reconciliations");
  var id=Guid.NewGuid();var a=new List<SasraForm3AccountBalance>{Account(savings,49000,id),Account(interest,2000,id),Account(nw,200,id),Account(term,0)};r=Build(a);Check(r.TotalAccounts==2&&r.DepositRows.Single(x=>x.Cell=="E13").Amount==51&&r.DepositRows.Single(x=>x.Cell=="E13").AccountCount==1,"combine principal and interest before banding; same account in different categories counted separately");Check(r.ZeroBalanceAccounts==1,"zero account excluded");
  a=Accounts();a.Add(Account(savings,-10));r=Build(a);Check(r.NegativeBalanceAccounts==1&&r.WorkbookBase64==null,"debit depositor blocks export");
  a=Accounts();var missing=Account(savings,10);missing.CustomerAccountId=null;a.Add(missing);r=Build(a);Check(r.UnallocatedGroups==1&&r.UnallocatedBalance==10&&r.WorkbookBase64==null,"null link blocks counts");
  a=Accounts();missing=Account(savings,10);missing.HasCustomerAccount=false;a.Add(missing);Check(Build(a).WorkbookBase64==null,"orphan link blocked");
  a=Accounts();missing=Account(savings,10);missing.CustomerAccountId=Guid.Empty;a.Add(missing);Check(Build(a).WorkbookBase64==null,"empty link blocked");
  a=Accounts();missing=Account(savings,10);missing.HasCustomerAccount=false;a.Add(missing);a.Add(Account(savings,-10));r=Build(a);Check(r.Difference==0&&r.WorkbookBase64==null,"offsetting bad allocations still blocked");
  var d=Definition();d.Lines[0].AccountIds.Clear();r=Build(null,d);Check(r.UnmappedAccounts.Count==1&&r.WorkbookBase64==null,"missing Form 6 deposit mapping blocked");
  a=Accounts();var ledger=Ledger(a);ledger[0].Balance+=1;Check(Build(a,null,ledger).WorkbookBase64==null,"unbalanced ledger blocked");
  ledger=Ledger(a);ledger[0].Balance-=1;ledger.Last().Balance+=1;r=Build(a,null,ledger);Check(r.Difference==-1&&r.WorkbookBase64==null,"customer versus G/L mismatch blocked");
  foreach(var mutate in new Action<SasraForm3Request>[]{i=>i.VersionId=Guid.Empty,i=>i.Form6VersionId=Guid.Empty,i=>i.AsAt=new DateTime(2027,1,1),i=>i.AsAt=new DateTime(2025,12,31),i=>i.YearStart=DateTime.MinValue}){var i=Input();mutate(i);try{SasraForm3.ValidateRequest(i);throw new Exception("Invalid dates accepted");}catch(SasraSetupException e){Check(!string.IsNullOrWhiteSpace(e.Field),"useful date/version field");}}
  d=Definition();d.Lines[0].Sign=1;try{SasraSetupAppService.ValidateVersion(d);throw new Exception("Wrong sign accepted");}catch(SasraSetupException){Check(true,"canonical sign protected");}
  d=Definition();d.Lines[2].AccountIds.Add(savings);try{SasraSetupAppService.ValidateVersion(d);throw new Exception("Duplicate accepted");}catch(SasraSetupException){Check(true,"cross-category duplicate rejected");}
  r=Build();var wb=new HSSFWorkbook(new MemoryStream(Convert.FromBase64String(r.WorkbookBase64)));var sheet=wb.GetSheet(SasraForm3.Sheet);var original=SasraForm3.Open().GetSheet(SasraForm3.Sheet);
  Check(!sheet.Protect&&sheet.GetRow(7).GetCell(3).CellStyle.WrapText,"editable workbook and wrapped count header");
  foreach(var row in r.DepositRows){var n=int.Parse(row.Cell.Substring(1))-1;Check(sheet.GetRow(n).GetCell(3).NumericCellValue==row.AccountCount,"count cell "+row.CountCell);Check(Math.Abs(sheet.GetRow(n).GetCell(4).NumericCellValue-(double)row.Amount)<1e-9,"amount cell "+row.Cell);}
  for(int n=7;n<=sheet.LastRowNum;n++){var old=original.GetRow(n);if(old==null)continue;foreach(var cell in old.Cells){if(cell.CellType==CellType.String)Check(sheet.GetRow(n).GetCell(cell.ColumnIndex).StringCellValue==cell.StringCellValue,"official label preserved");if(cell.CellType==CellType.Formula)Check(sheet.GetRow(n).GetCell(cell.ColumnIndex).CellFormula==cell.CellFormula,"original formula preserved");}}
  Check(sheet.GetRow(25).GetCell(3).NumericCellValue==5&&sheet.GetRow(25).GetCell(4).NumericCellValue==2450,"cached totals correct");
  File.WriteAllBytes(Path.Combine(Path.GetTempPath(),"sasra-form3-fixture.xls"),Convert.FromBase64String(r.WorkbookBase64));
  sheet.GetRow(8).GetCell(3).SetCellValue(2);sheet.GetRow(8).GetCell(4).SetCellValue(50);new HSSFFormulaEvaluator(wb).EvaluateAll();Check(sheet.GetRow(25).GetCell(3).NumericCellValue==6&&Math.Abs(sheet.GetRow(25).GetCell(4).NumericCellValue-2450.00001)<1e-9,"Excel count and amount edits recalculate totals");
  Console.WriteLine("PASS Form 3: "+checks+" assertions.");
 }
}
