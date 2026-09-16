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
 public static class SasraForm5
 {
  public const string Hash="c1a7ced517a55824467f615d4be666db4e20129f17d09e453e3a404c2f9d4bc5";
  public const string Version="WORKBOOK-C1A7CED517A5";
  public const string Sheet="Investments";
  static readonly int[] Rows={8,9,10,11,12,13,15,16,17,19,20,21,23,24,25,26,27,28};
  public static HSSFWorkbook Open(){using(var stream=typeof(SasraForm5).Assembly.GetManifestResourceStream("Sasra.Form5.xls")){
   if(stream==null)throw new InvalidOperationException("The Form 5 template resource is missing.");
   using(var sha=SHA256.Create())if(BitConverter.ToString(sha.ComputeHash(stream)).Replace("-","").ToLowerInvariant()!=Hash)throw new InvalidOperationException("Form 5 template checksum mismatch.");
   stream.Position=0;return new HSSFWorkbook(stream);
  }}
  public static SasraVersionDTO Definition(){var d=new SasraVersionDTO{Profile="DT",ReportCode="FORM 5",Title="Investment Return",Version=Version,WorkbookSha256=Hash,SourceUrl="https://www.sasra.go.ke/download/form-5-investment-return/"};var s=Open().GetSheet(Sheet);
   foreach(var r in Rows)d.Lines.Add(new SasraLineDTO{Code="F5_"+r.ToString("000"),Description=s.GetRow(r-1).GetCell(1).ToString().Trim(),Sheet=Sheet,Cell="C"+r,Sign=1,Source=r<=10?"Derived":r<=13?"GlBalance":new[]{16,20,24,27}.Contains(r)?"Constant":"Formula"});return d;
  }
  public static void Validate(SasraVersionDTO d){var expected=Definition();
   if(d==null||d.Profile!="DT"||d.ReportCode!="FORM 5"||d.Version!=Version||d.WorkbookSha256!=Hash||d.Lines==null||d.Lines.Count!=expected.Lines.Count)throw new SasraSetupException("Template","Use the supplied Form 5 definition and change only its G/L mappings.");
   for(int i=0;i<expected.Lines.Count;i++){var a=expected.Lines[i];var b=d.Lines[i];if(b==null||a.Code!=b.Code||a.Description!=b.Description||a.Source!=b.Source||a.Sheet!=b.Sheet||a.Cell!=b.Cell||a.Sign!=b.Sign||b.AccountIds==null||(a.Source!="GlBalance"&&b.AccountIds.Count>0))throw new SasraSetupException("Lines","Form 5 cells, signs and calculation sources are fixed. Reload the workbook definition.");}
  }
  public static void ValidateRequest(SasraForm5Request q){if(q==null||q.VersionId==Guid.Empty||q.Form1VersionId==Guid.Empty||q.Form6VersionId==Guid.Empty)throw new SasraSetupException("VersionId","Save Form 5, Capital Adequacy and SOFP mappings before previewing.");
   if(q.YearStart.Year<1753||q.YearStart.Year>=9999||q.AsAt.Year>=9999||q.AsAt.Date<q.YearStart.Date||q.AsAt.Date>=q.YearStart.Date.AddYears(1))throw new SasraSetupException("AsAt","Choose an as-at date within the selected financial year.");
   if(q.ReportPurpose!="Quarterly"&&q.ReportPurpose!="Interim")throw new SasraSetupException("ReportPurpose","Choose Quarterly return or Interim working copy.");
   if(q.ReportPurpose=="Quarterly"&&!(new[]{3,6,9,12}.Contains(q.AsAt.Month)&&q.AsAt.Day==DateTime.DaysInMonth(q.AsAt.Year,q.AsAt.Month)))throw new SasraSetupException("AsAt","Quarterly returns require 31 March, 30 June, 30 September or 31 December. Choose Interim working copy for another date.");
  }
  public static SasraForm5Result Build(SasraVersionDTO d,SasraProfileDTO profile,SasraForm5Request q,List<SasraForm1Balance> balances,SasraVersionDTO sofp,SasraForm1Result capital,int capitalRevision){
   Validate(d);ValidateRequest(q);
   var r=new SasraForm5Result{ReportPurpose=q.ReportPurpose,VersionId=d.Id,Form1VersionId=q.Form1VersionId,Form1Revision=capitalRevision,Form6VersionId=sofp.Id,Form6Revision=sofp.Revision,YearStart=q.YearStart.Date,AsAt=q.AsAt.Date,InstitutionName=profile.InstitutionName,GeneratedAtUtc=DateTime.UtcNow,LedgerDifference=balances.Sum(x=>x.Balance)};
   r.Issues.AddRange(capital.Issues.Select(x=>"Capital Adequacy / SOFP: "+x));
   if(string.IsNullOrWhiteSpace(profile.InstitutionName)||string.IsNullOrWhiteSpace(profile.RegistrationNumber))r.Issues.Add("Save the institution name and registration number before downloading.");
   if(Math.Abs(r.LedgerDifference)>0.01m)r.Issues.Add("The closing G/L does not balance.");
   var wb=Open();var s=wb.GetSheet(Sheet);s.ProtectSheet(null);
   if(q.ReportPurpose=="Interim"){
    s.GetRow(0).GetCell(1).SetCellValue("FORM 5 - INTERIM WORKING COPY");
    s.Header.Center="Interim working copy";
    r.Warnings.Add("Interim working copy — not a quarterly return.");
   }
   Func<int,ICell> cell=n=>s.GetRow(n-1).GetCell(2)??s.GetRow(n-1).CreateCell(2);
   cell(3).SetCellValue(profile.InstitutionName??"");cell(4).SetCellValue(q.YearStart.Year==q.AsAt.Year?q.AsAt.Year.ToString():q.YearStart.Year+"/"+q.AsAt.Year);
   foreach(var pair in new[]{Tuple.Create(5,q.YearStart),Tuple.Create(6,q.AsAt)}){var c=cell(pair.Item1);var style=wb.CreateCellStyle();style.CloneStyleFrom(c.CellStyle);style.DataFormat=wb.CreateDataFormat().GetFormat("dd/mm/yyyy");c.CellStyle=style;c.SetCellValue(pair.Item2.Date);}
   foreach(var pair in new[]{Tuple.Create(8,"D22"),Tuple.Create(9,"D34"),Tuple.Create(10,"D44")}){var amount=capital.Rows.SingleOrDefault(x=>x.Cell==pair.Item2)?.Amount;if(!amount.HasValue)r.Issues.Add("The source value for "+pair.Item2+" is unavailable in Capital Adequacy.");cell(pair.Item1).SetCellValue((double)amount.GetValueOrDefault());}
   var used=new HashSet<Guid>();var sofpAssets=new HashSet<Guid>(sofp.Lines.Where(x=>x.Cell!=null&&int.Parse(x.Cell.Substring(1))<38).SelectMany(x=>x.AccountIds));
   foreach(var line in d.Lines.Where(x=>x.Source=="GlBalance")){
    var selected=balances.Where(x=>line.AccountIds.Contains(x.Id)).ToList();
    // The AppService validates all mapped IDs against the COA. The balance query omits accounts with no postings.
    if(selected.Any(x=>x.AccountType!=1000))r.Issues.Add(line.Description+": select existing asset posting accounts.");
    if(line.AccountIds.Any(id=>!used.Add(id)))r.Issues.Add("An account is mapped to more than one investment-return line.");
    if(line.AccountIds.Any(id=>!sofpAssets.Contains(id)))r.Issues.Add(line.Description+": every mapped account must also be included in SOFP assets.");
    var amount=selected.Sum(x=>x.Balance);if(amount<0)r.Issues.Add(line.Description+": the net mapped asset value cannot be negative.");cell(int.Parse(line.Cell.Substring(1))).SetCellValue((double)amount);
   }
   var candidates=new HashSet<Guid>(sofp.Lines.Where(x=>new[]{"C18","C20","C32","C33"}.Contains(x.Cell)).SelectMany(x=>x.AccountIds));
   r.UnmappedAccounts=balances.Where(x=>x.Balance!=0&&candidates.Contains(x.Id)&&!used.Contains(x.Id)).Cast<SasraForm6Balance>().ToList();
   if(r.UnmappedAccounts.Count>0)r.Issues.Add("Classify the remaining SOFP investment/property/equipment accounts in the Form 5 mappings.");
   if(cell(11).NumericCellValue+cell(12).NumericCellValue+cell(13).NumericCellValue>cell(9).NumericCellValue+0.01)r.Issues.Add("The mapped asset categories exceed total assets. Review the mappings.");
   // Repair the source's missing land/buildings calculation and fixed -5% excess.
   foreach(var pair in new[]{Tuple.Create(15,"IF(C9>0,C13/C9,\"n.a.\")"),Tuple.Create(17,"IF(ISNUMBER(C15),C15-C16,\"n.a.\")"),Tuple.Create(19,"IF(C8>0,C12/C8,\"n.a.\")"),Tuple.Create(21,"IF(ISNUMBER(C19),C19-C20,\"n.a.\")"),Tuple.Create(23,"IF(C10>0,C12/C10,\"n.a.\")"),Tuple.Create(25,"IF(ISNUMBER(C23),C23-C24,\"n.a.\")"),Tuple.Create(26,"IF(C9>0,C11/C9,\"n.a.\")"),Tuple.Create(28,"IF(ISNUMBER(C26),C26-C27,\"n.a.\")")})cell(pair.Item1).SetCellFormula(pair.Item2);
   cell(16).SetCellValue(0.05);cell(20).SetCellValue(0.4);cell(24).SetCellValue(0.05);cell(27).SetCellValue(0.1);
   foreach(var n in Rows){var c=cell(n);var style=wb.CreateCellStyle();style.CloneStyleFrom(c.CellStyle);style.DataFormat=wb.CreateDataFormat().GetFormat(n>=15?"0.00%":"#,##0.00");c.CellStyle=style;}
   var evaluator=new HSSFFormulaEvaluator(wb);evaluator.EvaluateAll();
   foreach(var line in d.Lines){var c=cell(int.Parse(line.Cell.Substring(1)));var type=c.CellType==CellType.Formula?c.CachedFormulaResultType:c.CellType;var row=new SasraForm6Row{Cell=line.Cell,Source=line.Source,Description=line.Description,IsRatio=int.Parse(line.Cell.Substring(1))>=15,Formula=c.CellType==CellType.Formula?c.CellFormula:null};if(type==CellType.Numeric)row.Amount=Convert.ToDecimal(c.NumericCellValue);else if(type!=CellType.String||c.StringCellValue!="n.a.")r.Issues.Add("Formula "+line.Cell+" could not be calculated.");r.Rows.Add(row);}
   foreach(var n in new[]{15,19,23,26}){var row=r.Rows.Single(x=>x.Cell=="C"+n);var limit=cell(n+1).NumericCellValue;if(!row.Amount.HasValue)r.Warnings.Add(row.Description+": ratio unavailable because its denominator is not positive. Review the input in Excel.");else if(row.Amount.Value>(decimal)limit)r.Warnings.Add(row.Description+": exceeds the workbook maximum of "+(limit*100).ToString("0.##")+"%. A correctly calculated excess remains reportable.");}
   r.Warnings.Add("Mapping-based working copy. Core capital uses Form 1 without Excel-only adjustments. Review C8:C13 and any applicable exclusions or waivers in Excel; edits are not saved back into the system.");
   r.Warnings.Add("C11 follows the workbook label excluding land/buildings. Review other non-earning assets and non-government investment eligibility against supporting schedules. G/L categories alone do not establish eligibility.");
   r.Warnings.Add("Workbook correction: C15 calculates C13/C9, C16 contains the 5% maximum, and C17 calculates the excess/deficiency. Non-positive denominators display n.a. instead of spreadsheet errors.");
   if(r.Issues.Count==0)using(var output=new MemoryStream()){wb.Write(output);r.WorkbookBase64=Convert.ToBase64String(output.ToArray());}return r;
  }
 }
}
