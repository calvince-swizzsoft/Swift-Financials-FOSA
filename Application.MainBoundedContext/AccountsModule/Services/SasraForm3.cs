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
 public static class SasraForm3
 {
  public const string Hash="25b1834496bce1d3f88956fb6e32f86bdb7c2a5383f05b47ccbc18c599071eea";
  public const string Version="WORKBOOK-25B1834496BC";
  public const string Sheet="Deposits";
  static readonly string[] Codes={"NON_WITHDRAWABLE","SAVINGS","TERM"};
  static readonly string[] Names={"Non-withdrawable deposits","Savings deposits","Term deposits"};
  static readonly int[] FirstRows={9,12,16,19,23};
  static readonly string[] Ranges={"Below 50,000","50,000–100,000","Above 100,000–300,000","Above 300,000–1,000,000","Above 1,000,000"};
  public static HSSFWorkbook Open()
  {
   using(var stream=typeof(SasraForm3).Assembly.GetManifestResourceStream("Sasra.Form3.xls"))
   {
    if(stream==null)throw new InvalidOperationException("The Form 3 workbook resource is missing.");
    using(var hash=SHA256.Create())if(BitConverter.ToString(hash.ComputeHash(stream)).Replace("-","").ToLowerInvariant()!=Hash)throw new InvalidOperationException("Form 3 workbook checksum mismatch.");
    stream.Position=0;return new HSSFWorkbook(stream);
   }
  }
  public static SasraVersionDTO Definition()
  {
   var d=new SasraVersionDTO{Profile="DT",ReportCode="FORM 3",Title="Statement of Deposit Return",Version=Version,WorkbookSha256=Hash,SourceUrl="https://www.sasra.go.ke/download/form3-statement-of-deposit-return/"};
   // A category mapping applies to all five bands, not just the first output cell.
   for(int i=0;i<3;i++)d.Lines.Add(new SasraLineDTO{Code=Codes[i],Description=Names[i],Source="GlBalance",Sheet=Sheet,Cell="E"+(9+i),Sign=-1});
   return d;
  }
  public static void Validate(SasraVersionDTO d)
  {
   var expected=Definition();
   if(d==null||d.Profile!="DT"||d.ReportCode!="FORM 3"||d.WorkbookSha256!=Hash||d.Version!=Version||d.Lines==null||d.Lines.Count!=3)throw new SasraSetupException("Template","Use the supplied Form 3 definition and change only its deposit G/L mappings.");
   for(int i=0;i<3;i++)
   {
    var a=expected.Lines[i];var z=d.Lines[i];
    if(z==null||a.Code!=z.Code||a.Description!=z.Description||a.Source!=z.Source||a.Sheet!=z.Sheet||a.Cell!=z.Cell||a.Sign!=z.Sign||z.AccountIds==null)throw new SasraSetupException("Lines","Form 3 deposit categories, cells and signs are fixed. Reload the definition.");
   }
  }
  public static void ValidateRequest(SasraForm3Request input)
  {
   if(input==null||input.VersionId==Guid.Empty||input.Form6VersionId==Guid.Empty)throw new SasraSetupException("VersionId","Save Form 3 and Form 6 mappings before previewing the deposit return.");
   if(input.YearStart.Year<1753||input.YearStart.Year>=9999||input.AsAt.Year>=9999||input.AsAt.Date<input.YearStart.Date||input.AsAt.Date>=input.YearStart.Date.AddYears(1))throw new SasraSetupException("AsAt","Choose an as-at date within the selected financial year.");
  }
  public static int Band(decimal amount)
  {
   if(amount<=0)throw new ArgumentOutOfRangeException("amount","Deposit bands require a positive balance.");
   return amount<50000?0:amount<=100000?1:amount<=300000?2:amount<=1000000?3:4;
  }
  public static SasraForm3Result Build(SasraVersionDTO definition,SasraProfileDTO profile,SasraForm3Request input,List<SasraForm3AccountBalance> accounts,List<SasraForm6Balance> ledger,SasraVersionDTO sofp)
  {
   Validate(definition);ValidateRequest(input);
   var result=new SasraForm3Result{VersionId=definition.Id,Form6VersionId=sofp.Id,Form6Revision=sofp.Revision,YearStart=input.YearStart.Date,AsAt=input.AsAt.Date,GeneratedAtUtc=DateTime.UtcNow,InstitutionName=profile.InstitutionName};
   result.LedgerDifference=ledger.Sum(x=>x.Balance);
   if(Math.Abs(result.LedgerDifference)>0.01m)result.Issues.Add("The closing ledger does not balance. Review the underlying postings.");
   if(string.IsNullOrWhiteSpace(profile.RegistrationNumber))result.Issues.Add("Save the institution registration number before downloading.");
   var mapped=new HashSet<Guid>(definition.Lines.SelectMany(x=>x.AccountIds));
   var sofpDeposits=new HashSet<Guid>(sofp.Lines.Where(x=>new[]{"C42","C43","C44"}.Contains(x.Cell)).SelectMany(x=>x.AccountIds));
   result.UnmappedAccounts=ledger.Where(x=>x.Balance!=0&&sofpDeposits.Contains(x.Id)&&!mapped.Contains(x.Id)).ToList();
   if(result.UnmappedAccounts.Count>0)result.Issues.Add("Map all non-zero deposit accounts identified in Form 6.");
   result.MappedLedgerDeposits=-ledger.Where(x=>mapped.Contains(x.Id)).Sum(x=>x.Balance);
   result.Form6Difference=result.MappedLedgerDeposits+ledger.Where(x=>sofpDeposits.Contains(x.Id)).Sum(x=>x.Balance);
   if(Math.Abs(result.Form6Difference)>0.01m)result.Issues.Add("Form 3 deposit G/L mappings do not reconcile to the selected Form 6 deposit mappings.");
   for(int band=0;band<5;band++)for(int category=0;category<3;category++)
    result.DepositRows.Add(new SasraForm3Row{Range=Ranges[band],DepositType=Names[category],Category=Codes[category],Band=band,CountCell="D"+(FirstRows[band]+category),Cell="E"+(FirstRows[band]+category)});
   for(int category=0;category<3;category++)
   {
    var line=definition.Lines[category];var selected=accounts.Where(x=>line.AccountIds.Contains(x.ChartOfAccountId)).ToList();
    var unallocated=selected.Where(x=>!x.CustomerAccountId.HasValue||x.CustomerAccountId==Guid.Empty||!x.HasCustomerAccount).Where(x=>x.Balance!=0).ToList();
    result.UnallocatedGroups+=unallocated.Count;result.UnallocatedBalance-=unallocated.Sum(x=>x.Balance);
    // Combine principal and posted accrued-interest G/L balances before banding each customer account.
    foreach(var account in selected.Where(x=>x.CustomerAccountId.HasValue&&x.CustomerAccountId!=Guid.Empty&&x.HasCustomerAccount).GroupBy(x=>x.CustomerAccountId.Value))
    {
     var amount=-account.Sum(x=>x.Balance);
     if(amount==0){result.ZeroBalanceAccounts++;continue;}
     if(amount<0){result.NegativeBalanceAccounts++;result.NegativeBalance+=amount;continue;}
     var row=result.DepositRows.Single(x=>x.Category==line.Code&&x.Band==Band(amount));row.AccountCount++;row.Amount+=amount/1000m;
     result.TotalAccounts++;result.TotalDeposits+=amount;
    }
   }
   if(result.UnallocatedGroups>0)result.Issues.Add(result.UnallocatedGroups+" non-zero deposit balance group(s) have no valid customer-account link. Correct the allocation before reporting account counts.");
   if(result.NegativeBalanceAccounts>0)result.Issues.Add(result.NegativeBalanceAccounts+" customer deposit account(s) have debit balances. Review these separately; they are not netted against other depositors.");
   result.Difference=result.TotalDeposits-result.MappedLedgerDeposits;
   if(Math.Abs(result.Difference)>0.01m)result.Issues.Add("Banded customer deposits do not reconcile to the mapped deposit G/L totals.");
   result.Warnings.Add("Counts are positive-balance customer accounts within each deposit category, not unique members or individual fixed-deposit contracts. Zero-balance accounts are excluded; current account status does not erase historical balances.");
   result.Warnings.Add("Posted accrued interest is included when its G/L account is mapped and linked to the customer account. Review unposted accruals and supporting schedules in the editable Excel file. Excel edits are not saved back into the system.");
   result.Warnings.Add("Band boundaries: 50,000 enters the second band; 100,000 stays in the second, 300,000 in the third, and 1,000,000 in the fourth. Each balance is counted once before conversion to KSh thousands.");
   var wb=Open();var sheet=wb.GetSheet(Sheet);sheet.ProtectSheet(null);
   Func<int,int,ICell> cell=(row,col)=>{var r=sheet.GetRow(row-1)??sheet.CreateRow(row-1);return r.GetCell(col)??r.CreateCell(col);};
   cell(3,3).SetCellValue(profile.RegistrationNumber??"");cell(4,3).SetCellValue(input.YearStart.Year==input.AsAt.Year?input.AsAt.Year.ToString():input.YearStart.Year+"/"+input.AsAt.Year);
   foreach(var date in new[]{Tuple.Create(5,input.YearStart.Date),Tuple.Create(6,input.AsAt.Date)}){var c=cell(date.Item1,3);var style=wb.CreateCellStyle();style.CloneStyleFrom(c.CellStyle);style.DataFormat=wb.CreateDataFormat().GetFormat("dd/mm/yyyy");c.CellStyle=style;c.SetCellValue(date.Item2);}
   var amountStyle=wb.CreateCellStyle();amountStyle.CloneStyleFrom(cell(9,4).CellStyle);amountStyle.DataFormat=wb.CreateDataFormat().GetFormat("#,##0.00000;(#,##0.00000);\"-\"");
   foreach(var row in result.DepositRows){int n=int.Parse(row.Cell.Substring(1));cell(n,3).SetCellValue(row.AccountCount);cell(n,4).SetCellValue((double)row.Amount);cell(n,4).CellStyle=amountStyle;}
   // The source's account-count heading clips into the amount heading; wrap without changing labels.
   var header=cell(8,3);var headerStyle=wb.CreateCellStyle();headerStyle.CloneStyleFrom(header.CellStyle);headerStyle.WrapText=true;header.CellStyle=headerStyle;sheet.GetRow(7).HeightInPoints=Math.Max(32,sheet.GetRow(7).HeightInPoints);
   var totalStyle=wb.CreateCellStyle();totalStyle.CloneStyleFrom(cell(26,4).CellStyle);totalStyle.DataFormat=amountStyle.DataFormat;cell(26,4).CellStyle=totalStyle;
   var evaluator=new HSSFFormulaEvaluator(wb);evaluator.EvaluateAll();
   foreach(var col in new[]{3,4})if(cell(26,col).CachedFormulaResultType!=CellType.Numeric)result.Issues.Add("The workbook total could not be calculated.");
   if(result.Issues.Count==0)using(var output=new MemoryStream()){wb.Write(output);result.WorkbookBase64=Convert.ToBase64String(output.ToArray());}
   return result;
  }
 }
}
