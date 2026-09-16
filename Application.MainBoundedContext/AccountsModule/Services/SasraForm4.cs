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
    var minimum=Minimum(a,input.AsAt);
    var line=new SasraForm4Account{CustomerAccountId=a.CustomerAccountId,Cases=string.Join(", ",a.CaseNumbers),Principal=a.OutstandingPrincipal,InterestReceivable=a.Interest.Receivable,DaysPastDue=a.CombinedDaysPastDue,MinimumCategory=minimum,IsRestructured=a.IsRestructured,BasisHash=Basis(a)};r.Accounts.Add(line);
    line.Issues.AddRange(a.Issues);line.Issues.AddRange(a.Interest.Issues);
    if(!minimum.HasValue)line.Issues.Add("Complete principal and interest ageing before classification.");
    if(minimum.HasValue&&line.Issues.Count==0&&a.OutstandingPrincipal==0&&a.Interest.Receivable==0&&a.Interest.OverdueInterest==0){line.IsSettled=true;continue;}
    if(line.Issues.Count>0){r.UnresolvedAccounts++;r.UnclassifiedPrincipal+=a.OutstandingPrincipal;continue;}
    // This export is a system-derived working copy. Historical manual review
    // adjustments are deliberately not applied; all report adjustments belong in Excel.
    line.Category=minimum.Value;
    var row=r.Rows.Single(x=>x.IsRestructured==a.IsRestructured&&x.Category==line.Category);
    row.Accounts++;row.Principal+=a.OutstandingPrincipal;
   }
   foreach(var row in r.Rows){row.Exposure=row.Principal+row.Adjustment;row.Provision=decimal.Round(row.Exposure*row.Rate,2,MidpointRounding.AwayFromZero);}
   r.TotalExposure=r.Rows.Sum(x=>x.Exposure);r.TotalProvision=r.Rows.Sum(x=>x.Provision);
   if(r.UnresolvedAccounts>0)r.Issues.Add(r.UnresolvedAccounts+" account(s) remain unclassified. The working Excel includes every account and its exceptions; classified totals are incomplete.");
   if(!string.IsNullOrWhiteSpace(input.InstitutionName)&&!string.IsNullOrWhiteSpace(input.RegistrationNumber))r.WorkbookBase64=Export(input,r);
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
   if(result.Accounts.Count>65000)throw new LoanAgeingException("Accounts","This .xls template supports up to 65,000 account detail rows. Use a smaller reporting scope or an .xlsx template for this portfolio.");
   cell(2,1).SetCellValue("RISK CLASSIFICATION OF ASSETS AND PROVISIONING - SYSTEM WORKING COPY");sheet.GetRow(1).HeightInPoints=32;
   sheet.Footer.Left="System working copy - review Loan detail and Report notes before submission";
   var detail=wb.CreateSheet("Loan detail");
   var headings=new[]{"Cases","Loan account ID","Section","System principal KSh","Interest receivable KSh","System days overdue","System category","Excel category","Excel adjustment KSh","Adjusted exposure KSh","Include account (1/0)","System exceptions / Excel explanation","Classification key (calculated)"};
   var blue=wb.CreateFont();blue.Color=NPOI.HSSF.Util.HSSFColor.Blue.Index;
   var inputStyle=wb.CreateCellStyle();inputStyle.SetFont(blue);inputStyle.IsLocked=false;
   var moneyStyle=wb.CreateCellStyle();moneyStyle.DataFormat=wb.CreateDataFormat().GetFormat("#,##0.00;(#,##0.00);\"-\"");
   var moneyInput=wb.CreateCellStyle();moneyInput.CloneStyleFrom(moneyStyle);moneyInput.SetFont(blue);moneyInput.IsLocked=false;
   var headerStyle=wb.CreateCellStyle();var bold=wb.CreateFont();bold.IsBold=true;headerStyle.SetFont(bold);headerStyle.WrapText=true;
   var header=detail.CreateRow(0);header.HeightInPoints=32;
   for(int i=0;i<headings.Length;i++){var c=header.CreateCell(i);c.SetCellValue(headings[i]);c.CellStyle=headerStyle;detail.SetColumnWidth(i,(i==11?80:i==1?38:24)*256);}
   int index=1;
   foreach(var a in result.Accounts)
   {
    var row=detail.CreateRow(index);int n=index+1;index++;
    row.CreateCell(0).SetCellValue(a.Cases);row.CreateCell(1).SetCellValue(a.CustomerAccountId.ToString());row.CreateCell(2).SetCellValue(a.IsRestructured?"Restructured":"Ordinary");
    row.CreateCell(3).SetCellValue((double)a.Principal);row.GetCell(3).CellStyle=moneyStyle;
    row.CreateCell(4).SetCellValue((double)a.InterestReceivable);row.GetCell(4).CellStyle=moneyStyle;
    var days=row.CreateCell(5);if(a.DaysPastDue.HasValue)days.SetCellValue(a.DaysPastDue.Value);
    row.CreateCell(6).SetCellValue(a.IsSettled?"Settled":a.Category.HasValue?Names[a.Category.Value]:"Unclassified");
    var category=row.CreateCell(7);if(a.Category.HasValue)category.SetCellValue(Names[a.Category.Value]);category.CellStyle=inputStyle;
    row.CreateCell(8).SetCellValue(0);row.GetCell(8).CellStyle=moneyInput;
    row.CreateCell(9).SetCellFormula("D"+n+"+I"+n);row.GetCell(9).CellStyle=moneyStyle;
    row.CreateCell(10).SetCellValue(a.IsSettled?0:1);row.GetCell(10).CellStyle=inputStyle;
    row.CreateCell(11).SetCellValue(string.Join(" ",a.Issues));row.GetCell(11).CellStyle=inputStyle;
    row.CreateCell(12).SetCellFormula("C"+n+"&\"|\"&H"+n+"&\"|\"&K"+n);
   }
   int last=Math.Max(2,index);
   var validation=detail.GetDataValidationHelper();var categoryValidation=validation.CreateValidation(validation.CreateExplicitListConstraint(Names),new NPOI.SS.Util.CellRangeAddressList(1,last-1,7,7));categoryValidation.ShowErrorBox=true;detail.AddValidationData(categoryValidation);
   foreach(var row in result.Rows)
   {
    int n=(row.IsRestructured?17:10)+row.Category;
    // BIFF8-compatible conditional sums. A calculated key avoids SUMIFS
    // add-in serialization and array expressions unsupported by HSSF evaluation.
    string key=(row.IsRestructured?"Restructured":"Ordinary")+"|"+Names[row.Category]+"|1";
    cell(n,2).SetCellFormula("SUMIF('Loan detail'!$M$2:$M$"+last+",\""+key+"\",'Loan detail'!$K$2:$K$"+last+")");
    cell(n,3).SetCellFormula("SUMIF('Loan detail'!$M$2:$M$"+last+",\""+key+"\",'Loan detail'!$J$2:$J$"+last+")");
    cell(n,4).SetCellValue((double)row.Rate);cell(n,5).SetCellFormula("ROUND(D"+n+"*E"+n+",2)");
   }
   var notes=wb.CreateSheet("Report notes");notes.SetColumnWidth(0,38*256);notes.SetColumnWidth(1,110*256);
   Action<int,string,string> note=(n,label,value)=>{var row=notes.CreateRow(n);row.CreateCell(0).SetCellValue(label);row.CreateCell(1).SetCellValue(value);};
   note(0,"Status","SYSTEM WORKING COPY - adjustments and credit-quality review are completed in Excel.");
   note(1,"Institution",input.InstitutionName);note(2,"Reporting date",input.AsAt.ToString("yyyy-MM-dd"));
   note(3,"How to adjust","Edit the blue Excel category, Excel adjustment, Include account and explanation cells in Loan detail. Portfolio Analysis recalculates from these cells.");
   note(4,"Calculation basis","System categories use confirmed schedules, posted repayments and retained restructuring risk. Exposure starts at booked principal. No manual UI adjustments or credit reviews are applied.");
   note(5,"Missing classification","Unclassified accounts are included in Loan detail with a blank Excel category. They contribute to portfolio totals only after a supported category is entered. Do not treat them as performing or zero exposure.");
   note(6,"Adjustment scope","Excel adjustments affect this downloaded file only. They do not update the database, loan schedules or G/L. Keep the explanation with each adjustment and review before filing.");
   note(7,"Principal G/L KSh",result.LedgerPrincipal.ToString("0.00",System.Globalization.CultureInfo.InvariantCulture));
   note(8,"Principal G/L difference KSh",result.PrincipalDifference.ToString("0.00",System.Globalization.CultureInfo.InvariantCulture));
   note(9,"System unclassified accounts",result.UnresolvedAccounts.ToString());note(10,"System unclassified principal KSh",result.UnclassifiedPrincipal.ToString("0.00",System.Globalization.CultureInfo.InvariantCulture));
   for(int i=0;i<result.Issues.Count;i++)note(12+i,"System exception",result.Issues[i]);
   var wrap=wb.CreateCellStyle();wrap.WrapText=true;for(int i=0;i<=notes.LastRowNum;i++){var row=notes.GetRow(i);if(row==null)continue;row.GetCell(1).CellStyle=wrap;row.HeightInPoints=i>=3&&i<=6?45:30;}
   detail.CreateFreezePane(3,1);detail.SetAutoFilter(new NPOI.SS.Util.CellRangeAddress(0,last-1,0,11));
   wb.SetActiveSheet(0);wb.ForceFormulaRecalculation=true;new HSSFFormulaEvaluator(wb).EvaluateAll();
   if(Math.Abs((decimal)cell(23,5).NumericCellValue-result.TotalProvision)>.01m||Math.Abs((decimal)cell(23,3).NumericCellValue-result.TotalExposure)>.01m)throw new InvalidOperationException("Form 4 workbook totals failed reconciliation.");
   using(var output=new MemoryStream()){wb.Write(output);return Convert.ToBase64String(output.ToArray());}
  }
 }
}
