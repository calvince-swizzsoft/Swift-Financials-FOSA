using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Application.MainBoundedContext.BackOfficeModule.Services;
namespace Application.MainBoundedContext.AccountsModule.Services
{
 public static class SasraForm4
 {
  public const string Hash="5e8db04fc69d36d8d39a05be674f8c64d5634007575666dc9d817f0c8d9bad75";
  public const string Version="WORKBOOK-5E8DB04FC69D";
  public static readonly string[] Names={"Performing","Watch","Substandard","Doubtful","Loss"};
  public static readonly decimal[] Rates={.01m,.05m,.25m,.50m,1m};
  public static string Basis(LoanAgeingAccountResult account){using(var sha=SHA256.Create())return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(account)))).Replace("-","");}
  public static int? Minimum(LoanAgeingAccountResult a,DateTime date)
  {
   if(!a.CombinedDaysPastDue.HasValue)return null;
   return Math.Max(LoanRestructureAgeing.Category(a.CombinedDaysPastDue.Value,LoanRestructureAgeing.Missed(a,a.Interest,date)),a.IsRestructured?(a.PriorRiskCategory??4):0);
  }
  public static void Validate(SasraForm4Request input)
  {
   if(input==null||input.YearStart.Year<1753||input.YearStart.Year>=9999||input.AsAt.Date<input.YearStart.Date||input.AsAt.Date>=input.YearStart.Date.AddYears(1))throw new LoanAgeingException("AsAt","Choose a reporting date within the selected financial year.");
   if((input.InstitutionName??"").Length>256||(input.RegistrationNumber??"").Length>100)throw new LoanAgeingException("InstitutionName","Institution name or registration number is too long.");
  }
  public static SasraForm4Result Build(SasraForm4Request input,LoanAgeingResult ageing,List<LoanRiskReviewDTO> reviews)
  {
   Validate(input);var r=new SasraForm4Result{Version=Version,AsAt=input.AsAt.Date,LedgerPrincipal=ageing.LedgerPrincipal,PrincipalDifference=ageing.Difference};
   r.Issues.AddRange(ageing.Issues);
   if(string.IsNullOrWhiteSpace(input.InstitutionName)||string.IsNullOrWhiteSpace(input.RegistrationNumber))r.Issues.Add("Enter the institution name and registration number before downloading.");
   for(int section=0;section<2;section++)for(int category=0;category<5;category++)r.Rows.Add(new SasraForm4Row{IsRestructured=section==1,Category=category,Name=Names[category],Rate=Rates[category]});
   foreach(var a in ageing.Accounts)
   {
    var minimum=Minimum(a,input.AsAt);var hash=Basis(a);var review=reviews.Where(x=>x.CustomerAccountId==a.CustomerAccountId&&x.AsAt.Date==input.AsAt.Date).OrderByDescending(x=>x.Revision).FirstOrDefault();
    var line=new SasraForm4Account{CustomerAccountId=a.CustomerAccountId,Cases=string.Join(", ",a.CaseNumbers),Principal=a.OutstandingPrincipal,DaysPastDue=a.CombinedDaysPastDue,MinimumCategory=minimum,IsRestructured=a.IsRestructured,Review=review,BasisHash=hash};r.Accounts.Add(line);
    line.Issues.AddRange(a.Issues);line.Issues.AddRange(a.Interest.Issues);
    if(!minimum.HasValue)line.Issues.Add("Complete principal and interest ageing before classification.");
    // Confirmed zero principal/interest accounts are excluded from account counts.
    if(minimum.HasValue&&a.OutstandingPrincipal==0&&a.Interest.Receivable==0&&a.Interest.OverdueInterest==0)continue;
    if(review==null)line.Issues.Add("Save a dated credit-quality and provisioning-basis review.");
    else if(review.BasisHash!=hash||review.RiskCategory<(minimum??0))line.Issues.Add("The ageing basis changed. Recheck and save a new review revision.");
    if(line.Issues.Count>0){r.UnresolvedAccounts++;continue;}
    line.Category=Math.Max(minimum.Value,review.RiskCategory);
    var row=r.Rows.Single(x=>x.IsRestructured==a.IsRestructured&&x.Category==line.Category);
    row.Accounts++;row.Principal+=a.OutstandingPrincipal;row.Adjustment+=review.ProvisioningAdjustment.Value;
   }
   foreach(var row in r.Rows){row.Exposure=row.Principal+row.Adjustment;row.Provision=decimal.Round(row.Exposure*row.Rate,2,MidpointRounding.AwayFromZero);}
   r.TotalExposure=r.Rows.Sum(x=>x.Exposure);r.TotalProvision=r.Rows.Sum(x=>x.Provision);
   if(r.UnresolvedAccounts>0)r.Issues.Add(r.UnresolvedAccounts+" loan account(s) require review. The displayed classified totals are incomplete and Excel download is blocked.");
   if(r.Issues.Count==0)r.WorkbookBase64=Export(input,r);
   return r;
  }
  public static HSSFWorkbook Open()
  {
   using(var stream=typeof(SasraForm4).Assembly.GetManifestResourceStream("Sasra.Form4.xls"))
   {if(stream==null)throw new InvalidOperationException("Form 4 workbook missing.");using(var hash=SHA256.Create())if(BitConverter.ToString(hash.ComputeHash(stream)).Replace("-","").ToLowerInvariant()!=Hash)throw new InvalidOperationException("Form 4 workbook checksum mismatch.");stream.Position=0;return new HSSFWorkbook(stream);}
  }
  static string Export(SasraForm4Request input,SasraForm4Result result)
  {
   var wb=Open();var sheet=wb.GetSheet("Portfolio Analysis");sheet.ProtectSheet(null);
   Func<int,int,ICell> cell=(row,col)=>{var r=sheet.GetRow(row-1)??sheet.CreateRow(row-1);return r.GetCell(col)??r.CreateCell(col);};
   cell(3,3).SetCellValue(input.RegistrationNumber);cell(4,3).SetCellValue(input.YearStart.Year==input.AsAt.Year?input.AsAt.Year.ToString():input.YearStart.Year+"/"+input.AsAt.Year);
   foreach(var dt in new[]{Tuple.Create(5,input.YearStart),Tuple.Create(6,input.AsAt)}){var c=cell(dt.Item1,3);var style=wb.CreateCellStyle();style.CloneStyleFrom(c.CellStyle);style.DataFormat=wb.CreateDataFormat().GetFormat("dd/mm/yyyy");c.CellStyle=style;c.SetCellValue(dt.Item2.Date);}
   foreach(var row in result.Rows){int n=(row.IsRestructured?17:10)+row.Category;cell(n,2).SetCellValue(row.Accounts);cell(n,3).SetCellValue((double)row.Exposure);cell(n,4).SetCellValue((double)row.Rate);cell(n,5).SetCellFormula("ROUND(D"+n+"*E"+n+",2)");}
   var detail=wb.CreateSheet("Review evidence");var headings=new[]{"Institution","As at","Cases","Principal KSh","Basis adjustment KSh","Risk category","Evidence","Review revision"};
   var header=detail.CreateRow(0);for(int i=0;i<headings.Length;i++){header.CreateCell(i).SetCellValue(headings[i]);detail.SetColumnWidth(i,(i==6?70:24)*256);}
   int index=1;foreach(var a in result.Accounts.Where(x=>x.Category.HasValue)){var row=detail.CreateRow(index++);row.CreateCell(0).SetCellValue(input.InstitutionName);row.CreateCell(1).SetCellValue(input.AsAt.ToString("yyyy-MM-dd"));row.CreateCell(2).SetCellValue(a.Cases);row.CreateCell(3).SetCellValue((double)a.Principal);row.CreateCell(4).SetCellValue((double)a.Review.ProvisioningAdjustment.Value);row.CreateCell(5).SetCellValue(Names[a.Category.Value]);row.CreateCell(6).SetCellValue(a.Review.Evidence);row.CreateCell(7).SetCellValue(a.Review.Revision);}
   detail.CreateFreezePane(0,1);new HSSFFormulaEvaluator(wb).EvaluateAll();
   if(Math.Abs((decimal)cell(23,5).NumericCellValue-result.TotalProvision)>.01m)throw new InvalidOperationException("Form 4 workbook provision total failed reconciliation.");
   using(var output=new MemoryStream()){wb.Write(output);return Convert.ToBase64String(output.ToArray());}
  }
 }
}
