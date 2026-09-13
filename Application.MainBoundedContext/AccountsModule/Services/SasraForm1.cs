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
 public static class SasraForm1
 {
  public const string Hash="65571f0718af4de497d6c003214af6e6aefea7d52521466997368538dca79c13";
  public const string Version="WORKBOOK-65571F0718AF";
  public const string Sheet="Capital Adequacy";
  static readonly int[] MappedRows={10,11,12,14,15,16,26,27,28,29,30,31,32};
  public static HSSFWorkbook Open()
  {
   using(var stream=typeof(SasraForm1).Assembly.GetManifestResourceStream("Sasra.Form1.xls"))
   {
    if(stream==null)throw new InvalidOperationException("The Form 1 workbook resource is missing.");
    using(var hash=SHA256.Create())if(BitConverter.ToString(hash.ComputeHash(stream)).Replace("-","").ToLowerInvariant()!=Hash)throw new InvalidOperationException("Form 1 workbook checksum mismatch.");
    stream.Position=0;return new HSSFWorkbook(stream);
   }
  }
  public static SasraVersionDTO Definition()
  {
   var d=new SasraVersionDTO{Profile="DT",ReportCode="FORM 1",Title="Capital Adequacy",Version=Version,WorkbookSha256=Hash,SourceUrl="https://www.sasra.go.ke/download/form-1-capital-adequacy/"};
   var sheet=Open().GetSheet(Sheet);
   for(int row=8;row<=53;row++)
   {
    var r=sheet.GetRow(row-1);var label=r?.GetCell(2)?.ToString();if(string.IsNullOrWhiteSpace(label))continue;
    var formula=r.GetCell(3)?.CellType==CellType.Formula;
    var source=formula?"Formula":MappedRows.Contains(row)?"GlBalance":new[]{19,20,37}.Contains(row)?"Manual":new[]{13,34,41,44}.Contains(row)?"Derived":new[]{46,49,52}.Contains(row)?"Constant":"Header";
    d.Lines.Add(new SasraLineDTO{Code="F1_"+row.ToString("000"),Description=label.Trim(),Source=source,Sheet=source=="Header"?null:Sheet,Cell=source=="Header"?null:"D"+row,Sign=MappedRows.Contains(row)&&row<17?-1:1});
   }
   return d;
  }
  public static bool AllowsAccountType(string cell,int type){return int.Parse(cell.Substring(1))<17?type==3000:type==1000;}
  public static void Validate(SasraVersionDTO d)
  {
   var expected=Definition();
   if(d==null||d.Profile!="DT"||d.ReportCode!="FORM 1"||d.WorkbookSha256!=Hash||d.Lines==null||d.Lines.Count!=expected.Lines.Count)throw new SasraSetupException("Template","Use the supplied Form 1 definition and change only its G/L mappings.");
   for(int i=0;i<expected.Lines.Count;i++)
   {
    var a=expected.Lines[i];var b=d.Lines[i];
    if(b==null||a.Code!=b.Code||a.Description!=b.Description||a.Source!=b.Source||a.Sheet!=b.Sheet||a.Cell!=b.Cell||a.Sign!=b.Sign||b.AccountIds==null||(a.Source!="GlBalance"&&b.AccountIds.Count>0))throw new SasraSetupException("Lines","Form 1 cells, signs and calculation sources are fixed. Reload the workbook definition.");
   }
  }
  public static void ValidateRequest(SasraForm1Request input)
  {
   if(input==null||input.VersionId==Guid.Empty||input.Form6VersionId==Guid.Empty)throw new SasraSetupException("VersionId","Save Form 1 mappings and a Form 6 revision before previewing capital adequacy.");
   if(input.YearStart.Year<1753||input.YearStart.Year>=9999||input.AsAt.Year>=9999||input.AsAt.Date<input.YearStart.Date||input.AsAt.Date>=input.YearStart.Date.AddYears(1))throw new SasraSetupException("AsAt","Choose an as-at date within the selected financial year.");
   if(input.MappingBased)
   {
    if(new[]{input.SurplusAdjustment,input.InvestmentDeduction,input.OtherDeductions,input.OffBalanceSheetAssets}.Any(x=>x.GetValueOrDefault()!=0))throw new SasraSetupException("MappingBased","Complete additional adjustments in Excel when generating from mappings.");
    return;
   }
   foreach(var pair in new[]{Tuple.Create("SurplusAdjustment",input.SurplusAdjustment),Tuple.Create("InvestmentDeduction",input.InvestmentDeduction),Tuple.Create("OtherDeductions",input.OtherDeductions),Tuple.Create("OffBalanceSheetAssets",input.OffBalanceSheetAssets)})
    if(!pair.Item2.HasValue||(pair.Item2.Value>1000000000000000m||pair.Item2.Value< -1000000000000000m)||(pair.Item1!="SurplusAdjustment"&&pair.Item2.Value<0))throw new SasraSetupException(pair.Item1,"Enter a valid KSh amount, including an explicit zero if none. Deductions and off-balance-sheet exposures cannot be negative.");
   if((input.ReviewNotes??"").Length>1000)throw new SasraSetupException("ReviewNotes","Limit the supporting explanation to 1,000 characters.");
   if(new[]{input.SurplusAdjustment.GetValueOrDefault(),input.InvestmentDeduction.GetValueOrDefault(),input.OtherDeductions.GetValueOrDefault(),input.OffBalanceSheetAssets.GetValueOrDefault()}.Any(x=>x!=0)&&string.IsNullOrWhiteSpace(input.ReviewNotes))throw new SasraSetupException("ReviewNotes","Explain the non-zero adjustments and exposures and identify their supporting schedules.");
  }
  public static SasraForm1Result Build(SasraVersionDTO definition,SasraProfileDTO profile,SasraForm1Request input,List<SasraForm1Balance> balances,SasraVersionDTO form6Definition,SasraForm6Result form6)
  {
   Validate(definition);ValidateRequest(input);
   var result=new SasraForm1Result{VersionId=definition.Id,Form6VersionId=form6Definition.Id,Form6Revision=form6Definition.Revision,YearStart=input.YearStart.Date,AsAt=input.AsAt.Date,GeneratedAtUtc=DateTime.UtcNow,InstitutionName=profile.InstitutionName,Inputs=input};
   var pnl=balances.Where(x=>x.AccountType==4000||x.AccountType==5000).ToList();
   result.RawCurrentSurplus=-pnl.Sum(x=>x.Balance-x.BeforeYear-x.CurrentClosingBalance);
   result.AdjustedSurplus=result.RawCurrentSurplus+input.SurplusAdjustment.GetValueOrDefault();
   result.EligibleCurrentSurplus=result.AdjustedSurplus>0?result.AdjustedSurplus*0.5m:result.AdjustedSurplus;
   result.LedgerDifference=balances.Sum(x=>x.Balance);
   if(Math.Abs(result.LedgerDifference)>0.01m)result.Issues.Add("The underlying closing ledger does not balance.");
   result.Issues.AddRange(form6.Issues.Select(x=>"Form 6: "+x));
   if(!input.MappingBased&&!input.CapitalEligibilityReviewed)result.Issues.Add("Review capital eligibility, provisions, proposed dividends, deductions and off-balance-sheet exposures before downloading.");
   if(input.MappingBased)result.Warnings.Add("Generated from mappings without additional regulatory adjustments. Review eligible surplus (D13), investment deductions (D19), other deductions (D20) and off-balance-sheet exposures (D37) in Excel. Initial zeros do not confirm that these items are absent. Excel edits are not saved back to the system.");
   var used=new HashSet<Guid>(definition.Lines.SelectMany(x=>x.AccountIds));
   var excludedEquity=new HashSet<Guid>(form6Definition.Lines.Where(x=>x.Cell=="C67"||x.Cell=="C68").SelectMany(x=>x.AccountIds));
   result.UnmappedAccounts=balances.Where(x=>x.Balance!=0&&(x.AccountType==1000||x.AccountType==3000)&&!used.Contains(x.Id)&&!excludedEquity.Contains(x.Id)).Cast<SasraForm6Balance>().ToList();
   if(result.UnmappedAccounts.Count>0)result.Issues.Add("Map the remaining non-zero asset and capital accounts; revaluation reserves and proposed dividends mapped in Form 6 are excluded from core capital.");
   if(string.IsNullOrWhiteSpace(profile.RegistrationNumber))result.Issues.Add("Save the institution registration number before downloading.");
   var wb=Open();var sheet=wb.GetSheet(Sheet);
   Func<int,ICell> cell=row=>sheet.GetRow(row-1).GetCell(3)??sheet.GetRow(row-1).CreateCell(3);
   cell(3).SetCellValue(profile.RegistrationNumber??"");cell(4).SetCellValue(input.YearStart.Year==input.AsAt.Year?input.AsAt.Year.ToString():input.YearStart.Year+"/"+input.AsAt.Year);
   foreach(var pair in new[]{Tuple.Create(5,input.YearStart.Date),Tuple.Create(6,input.AsAt.Date)}){var c=cell(pair.Item1);var style=wb.CreateCellStyle();style.CloneStyleFrom(c.CellStyle);style.DataFormat=wb.CreateDataFormat().GetFormat("dd/mm/yyyy");c.CellStyle=style;c.SetCellValue(pair.Item2);}
   foreach(var line in definition.Lines.Where(x=>x.Source=="GlBalance"))
   {
    var row=int.Parse(line.Cell.Substring(1));
    // Remove this year's fiscal-close transfers from capital components; current earnings enter once through D13.
    var amount=balances.Where(x=>line.AccountIds.Contains(x.Id)).Sum(x=>x.Balance-(row<17?x.CurrentClosingBalance:0))*line.Sign;
    if(row==12)amount-=pnl.Sum(x=>x.BeforeYear);
    cell(row).SetCellValue((double)amount);
   }
   cell(13).SetCellValue((double)result.EligibleCurrentSurplus);
   cell(19).SetCellValue((double)input.InvestmentDeduction.GetValueOrDefault());cell(20).SetCellValue((double)input.OtherDeductions.GetValueOrDefault());cell(37).SetCellValue((double)input.OffBalanceSheetAssets.GetValueOrDefault());
   cell(34).SetCellValue((double)(form6.Rows.Single(x=>x.Cell=="C38").Amount.Value*1000m));
   cell(44).SetCellValue((double)(form6.Rows.Single(x=>x.Cell=="C45").Amount.Value*1000m));
   cell(41).SetCellFormula("SUM(D26:D32)");
   var evaluator=new HSSFFormulaEvaluator(wb);evaluator.EvaluateAll();
   bool validAssets=cell(43).NumericCellValue>0,validDeposits=cell(44).NumericCellValue>0;
   if(!validAssets)result.Issues.Add("Total on- and off-balance-sheet assets must be positive to calculate meaningful capital-to-assets ratios.");
   if(!validDeposits)result.Issues.Add("Deposit liabilities must be positive to calculate the capital-to-deposits ratio.");
   foreach(var line in definition.Lines)
   {
    var row=new SasraForm6Row{Cell=line.Cell,Description=line.Description,Source=line.Source};
    if(line.Cell!=null)
    {
     var n=int.Parse(line.Cell.Substring(1));var c=cell(n);row.IsRatio=n>=45;
     if(c.CellType==CellType.Formula)row.Formula=c.CellFormula;
     if(c.CellType==CellType.Numeric||(c.CellType==CellType.Formula&&c.CachedFormulaResultType==CellType.Numeric))row.Amount=Convert.ToDecimal(c.NumericCellValue);
     else result.Issues.Add("Formula "+line.Cell+" is unavailable. Check its input amounts.");
     if((!validAssets&&new[]{45,47,48,50}.Contains(n))||(!validDeposits&&new[]{51,53}.Contains(n)))row.Amount=null;
    }
    result.Rows.Add(row);
   }
   result.Difference=result.Rows.Single(x=>x.Cell=="D35").Amount.Value;
   if(Math.Abs(result.Difference)>0.01m)result.Issues.Add("Form 1 asset categories do not reconcile to the selected Form 6 revision. Review mappings before downloading.");
   foreach(var pair in new[]{Tuple.Create(45,46,"Core capital / assets"),Tuple.Create(48,49,"Institutional capital / assets"),Tuple.Create(51,52,"Core capital / deposits")})
   {
    var actual=result.Rows.Single(x=>x.Cell=="D"+pair.Item1).Amount;var required=Convert.ToDecimal(cell(pair.Item2).NumericCellValue);
    if(actual.HasValue&&actual.Value<required)result.Warnings.Add(pair.Item3+" is below the workbook minimum of "+(required*100).ToString("0.##")+"%.");
   }
   // Deficient but correctly calculated ratios remain reportable; data errors and missing reviews block export.
   if(result.Issues.Count==0)using(var output=new MemoryStream()){wb.Write(output);result.WorkbookBase64=Convert.ToBase64String(output.ToArray());}
   return result;
  }
 }
}
