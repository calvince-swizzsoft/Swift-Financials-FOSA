using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using Application.MainBoundedContext.DTO.AccountsModule;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
namespace Application.MainBoundedContext.AccountsModule.Services
{
 public static class SasraForm2
 {
  public const string Hash="b71442b70b5eed0867078026911763ec6742fc24c2e46581b1167bee98c0a7b1";
  public const string Version="WORKBOOK-B71442B70B5E";
  public const string Sheet="Liquidity";
  static readonly int[] MappedRows={9,10,13,19,20,22,23,27,28,33,34};
  public static HSSFWorkbook Open()
  {
   using(var stream=typeof(SasraForm2).Assembly.GetManifestResourceStream("Sasra.Form2.xls"))
   {
    if(stream==null)throw new InvalidOperationException("The Form 2 workbook resource is missing.");
    using(var hash=SHA256.Create())if(BitConverter.ToString(hash.ComputeHash(stream)).Replace("-","").ToLowerInvariant()!=Hash)throw new InvalidOperationException("Form 2 workbook checksum mismatch.");
    stream.Position=0;return new HSSFWorkbook(stream);
   }
  }
  public static SasraVersionDTO Definition()
  {
   var d=new SasraVersionDTO{Profile="DT",ReportCode="FORM 2",Title="Liquidity Statement",Version=Version,WorkbookSha256=Hash,SourceUrl="https://www.sasra.go.ke/download/form-2-liquidity-statement/"};
   var sheet=Open().GetSheet(Sheet);
   for(int row=8;row<=53;row++)
   {
    var r=sheet.GetRow(row-1);var label=r?.GetCell(2)?.ToString();if(string.IsNullOrWhiteSpace(label))continue;
    var formula=r.GetCell(3)?.CellType==CellType.Formula;
    var source=formula?"Formula":MappedRows.Contains(row)?"GlBalance":new[]{15,24,37,38,39,44,45}.Contains(row)?"Manual":new[]{16}.Contains(row)?"Derived":new[]{52}.Contains(row)?"Constant":"Header";
    d.Lines.Add(new SasraLineDTO{Code="F2_"+row.ToString("000"),Description=label.Trim(),Source=source,Sheet=source=="Header"?null:Sheet,Cell=source=="Header"?null:"D"+row,Sign=new[]{22,23,33,34}.Contains(row)?-1:1});
   }
   return d;
  }
  public static bool AllowsAccountType(string cell,int type){return new[]{"D22","D23","D33","D34"}.Contains(cell)?type==2000:type==1000;}
  public static void Validate(SasraVersionDTO d)
  {
   var expected=Definition();
   if(d==null||d.Profile!="DT"||d.ReportCode!="FORM 2"||d.WorkbookSha256!=Hash||d.Lines==null||d.Lines.Count!=expected.Lines.Count)throw new SasraSetupException("Template","Use the supplied Form 2 definition and change only its G/L mappings.");
   for(int i=0;i<expected.Lines.Count;i++)
   {
    var a=expected.Lines[i];var b=d.Lines[i];
    if(b==null||a.Code!=b.Code||a.Description!=b.Description||a.Source!=b.Source||a.Sheet!=b.Sheet||a.Cell!=b.Cell||a.Sign!=b.Sign||b.AccountIds==null||(a.Source!="GlBalance"&&b.AccountIds.Count>0))throw new SasraSetupException("Lines","Form 2 cells, signs and calculation sources are fixed. Reload the workbook definition.");
   }
  }

  public static readonly string[] ManualCells={"D15","D16","D24","D37","D38","D39","D44","D45"};
  public static readonly string[] ExclusionCells={"D9","D10","D13","D19","D20","D22","D23","D27","D28","D33","D34"};
  public static void ValidateRequest(SasraForm2Request input)
  {
   if(input==null||input.VersionId==Guid.Empty||input.Form6VersionId==Guid.Empty)throw new SasraSetupException("VersionId","Save Form 2 and Form 6 mappings before previewing liquidity.");
   if(input.YearStart.Year<1753||input.YearStart.Year>=9999||input.AsAt.Year>=9999||input.AsAt.Date<input.YearStart.Date||input.AsAt.Date>=input.YearStart.Date.AddYears(1))throw new SasraSetupException("AsAt","Choose an as-at date within the selected financial year.");
   if(input.MappingBased)
   {
    if((input.ManualAmounts!=null&&input.ManualAmounts.Any(x=>!ManualCells.Contains(x.Key)||x.Value.GetValueOrDefault()!=0)) ||
       (input.Exclusions!=null&&input.Exclusions.Any(x=>!ExclusionCells.Contains(x.Key)||x.Value.GetValueOrDefault()!=0)))throw new SasraSetupException("MappingBased","Complete additional maturity and eligibility adjustments in Excel when generating from mappings.");
    return;
   }
   foreach(var pair in new[]{Tuple.Create("ManualAmounts",input.ManualAmounts,ManualCells),Tuple.Create("Exclusions",input.Exclusions,ExclusionCells)})
   {
    if(pair.Item2==null||pair.Item2.Count!=pair.Item3.Length||pair.Item3.Any(k=>!pair.Item2.ContainsKey(k)||!pair.Item2[k].HasValue||pair.Item2[k]<0||pair.Item2[k]>1000000000000000m))throw new SasraSetupException(pair.Item1,"Enter a non-negative KSh amount for each schedule line, including an explicit zero where none applies.");
   }
   if(string.IsNullOrWhiteSpace(input.ReviewNotes)||(input.ReviewNotes??"").Length>1000)throw new SasraSetupException("ReviewNotes","Identify the supporting bank, maturity, deposit and restriction schedules (up to 1,000 characters).");
  }
  public static SasraForm2Result Build(SasraVersionDTO definition,SasraProfileDTO profile,SasraForm2Request input,List<SasraForm6Balance> balances,SasraVersionDTO sofp)
  {
   Validate(definition);ValidateRequest(input);
   var result=new SasraForm2Result{VersionId=definition.Id,Form6VersionId=sofp.Id,Form6Revision=sofp.Revision,YearStart=input.YearStart.Date,AsAt=input.AsAt.Date,GeneratedAtUtc=DateTime.UtcNow,InstitutionName=profile.InstitutionName,Inputs=input};
   result.LedgerDifference=balances.Sum(x=>x.Balance);
   if(Math.Abs(result.LedgerDifference)>0.01m)result.Issues.Add("The closing ledger does not balance. Review the underlying postings.");
   if(!input.MappingBased&&!input.LiquidityReviewed)result.Issues.Add("Review cash availability, bank reconciliation, maturities, accrued interest, restricted investments and all other liabilities before downloading.");
   if(string.IsNullOrWhiteSpace(profile.RegistrationNumber))result.Issues.Add("Save the institution registration number before downloading.");
   if(input.MappingBased)result.Warnings.Add("Generated from mappings without additional maturity or eligibility adjustments. Review exclusions, obligations and liability maturities in Excel. Initial schedule zeros do not confirm that these items are absent. D16 already includes mapped bank overdrafts. Excel edits are not saved back to the system.");
   var used=new HashSet<Guid>(definition.Lines.SelectMany(x=>x.AccountIds));
   var candidates=new HashSet<Guid>(sofp.Lines.Where(x=>new[]{"C11","C12","C17","C42","C43","C44"}.Contains(x.Cell)).SelectMany(x=>x.AccountIds));
   result.UnmappedAccounts=balances.Where(x=>x.Balance!=0&&candidates.Contains(x.Id)&&!used.Contains(x.Id)).ToList();
   if(result.UnmappedAccounts.Count>0)result.Issues.Add("Map the remaining cash, bank, government-security and deposit accounts identified in Form 6.");
   result.Warnings.Add("Review other asset and liability accounts against supporting schedules: G/L categories alone do not establish liquidity eligibility or maturity.");
   var wb=Open();var sheet=wb.GetSheet(Sheet);
   if(input.MappingBased)sheet.ProtectSheet(null); // Users complete adjustments in the downloaded workbook.
   Func<int,ICell> cell=row=>sheet.GetRow(row-1).GetCell(3)??sheet.GetRow(row-1).CreateCell(3);
   cell(3).SetCellValue(profile.RegistrationNumber??"");cell(4).SetCellValue(input.YearStart.Year==input.AsAt.Year?input.AsAt.Year.ToString():input.YearStart.Year+"/"+input.AsAt.Year);
   foreach(var pair in new[]{Tuple.Create(5,input.YearStart.Date),Tuple.Create(6,input.AsAt.Date)}){var c=cell(pair.Item1);var style=wb.CreateCellStyle();style.CloneStyleFrom(c.CellStyle);style.DataFormat=wb.CreateDataFormat().GetFormat("dd/mm/yyyy");c.CellStyle=style;c.SetCellValue(pair.Item2);}
   foreach(var line in definition.Lines.Where(x=>x.Source=="GlBalance"))
   {
    var selected=balances.Where(x=>line.AccountIds.Contains(x.Id)).ToList();
    // Split bank balances per posting account. Never net one bank's overdraft against another bank's cash.
    var gross=line.Cell=="D13"?selected.Sum(x=>Math.Max(0,x.Balance)):selected.Sum(x=>x.Balance)*line.Sign;
    if(line.Cell=="D13")result.BankOverdrafts=selected.Sum(x=>Math.Max(0,-x.Balance));
    var excluded=input.MappingBased?0m:input.Exclusions[line.Cell].Value;
    if(gross<0||excluded>gross)result.Issues.Add(line.Description+": the reported balance cannot be negative and exclusions cannot exceed its G/L balance.");
    cell(int.Parse(line.Cell.Substring(1))).SetCellValue((double)(gross-excluded));
   }
   foreach(var key in ManualCells)cell(int.Parse(key.Substring(1))).SetCellValue((double)(input.MappingBased?0m:input.ManualAmounts[key].Value));
   cell(16).SetCellValue((double)(result.BankOverdrafts+(input.MappingBased?0m:input.ManualAmounts["D16"].Value)));
   if(cell(15).NumericCellValue>cell(13).NumericCellValue)result.Issues.Add("Time deposits over 90 days cannot exceed the commercial-bank balances remaining after exclusions.");
   // Source D43 sums both components AND their total, doubling the displayed section total.
   // Correct only this redundant heading; the regulatory ratio uses D46 and is unchanged.
   cell(43).SetCellFormula("SUM(D44:D45)");
   var evaluator=new HSSFFormulaEvaluator(wb);evaluator.EvaluateAll();
   bool validDenominator=cell(50).NumericCellValue>0;
   if(!validDenominator)result.Issues.Add("Total deposits plus other short-term liabilities must be positive to calculate the liquidity ratio.");
   if(cell(41).NumericCellValue<0)result.Issues.Add("Deposit deductions cannot exceed total deposits. Include only balances already within deposit totals, not bank overdrafts deducted from liquid assets.");
   var depositIds=new HashSet<Guid>(sofp.Lines.Where(x=>new[]{"C42","C43","C44"}.Contains(x.Cell)).SelectMany(x=>x.AccountIds));
   var form2Deposits=new HashSet<Guid>(definition.Lines.Where(x=>x.Cell=="D33"||x.Cell=="D34").SelectMany(x=>x.AccountIds));
   result.Difference=-balances.Where(x=>form2Deposits.Contains(x.Id)).Sum(x=>x.Balance)+balances.Where(x=>depositIds.Contains(x.Id)).Sum(x=>x.Balance);
   if(Math.Abs(result.Difference)>0.01m)result.Issues.Add("Gross deposits before exclusions do not reconcile to the selected Form 6 deposit mappings.");
   foreach(var line in definition.Lines)
   {
    var row=new SasraForm6Row{Cell=line.Cell,Description=line.Description,Source=line.Source};
    if(line.Cell!=null)
    {
     var n=int.Parse(line.Cell.Substring(1));var c=cell(n);row.IsRatio=n>=51;
     if(c.CellType==CellType.Formula)row.Formula=c.CellFormula;
     if(c.CellType==CellType.Numeric||(c.CellType==CellType.Formula&&c.CachedFormulaResultType==CellType.Numeric))row.Amount=Convert.ToDecimal(c.NumericCellValue);
     else result.Issues.Add("Formula "+line.Cell+" is unavailable. Check its inputs.");
     if(!validDenominator&&(n==51||n==53))row.Amount=null;
    }
    result.Rows.Add(row);
   }
   if(validDenominator&&cell(51).NumericCellValue<cell(52).NumericCellValue)result.Warnings.Add("Liquidity is below the workbook minimum of 15%. A correctly calculated deficit remains reportable.");
   result.Warnings.Add("Workbook correction: D43 now totals D44 and D45 once. The ratio retains the official D35 + D46 denominator (gross deposits, not net deposits).");
   if(result.Issues.Count==0)using(var output=new MemoryStream()){wb.Write(output);result.WorkbookBase64=Convert.ToBase64String(output.ToArray());}
   return result;
  }
 }
}
