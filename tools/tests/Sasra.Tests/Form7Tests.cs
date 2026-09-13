using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
static class Form7Tests
{
 static int checks;
 static void Check(bool ok,string message){if(!ok)throw new Exception("Form 7: "+message);checks++;}
 static void Reject(Action action){try{action();throw new Exception("Invalid Form 7 accepted");}catch(SasraSetupException){checks++;}}
 public static void Run()
 {
  var d=SasraForm7.Definition();SasraSetupAppService.ValidateVersion(d);SasraSetupAppService.ValidateVersion(d);
  Check(d.Lines.Count(x=>x.Source=="GlMovement")==23,"23 input lines");
  Check(d.Lines.Count(x=>x.Source=="Formula")==12,"12 calculated lines");
  var data=new List<SasraForm7Balance>();
  Action<string,int,decimal> add=(cell,type,value)=>{var b=new SasraForm7Balance{Id=Guid.NewGuid(),AccountType=type,Balance=value};data.Add(b);d.Lines.Single(x=>x.Cell==cell).AccountIds.Add(b.Id);};
  add("C12",4000,-100000);add("C19",4000,-20000);add("C22",5000,10000);add("C33",5000,5000);add("C34",4000,-2000);add("C37",5000,15000);add("C40",5000,3000);add("C46",4000,-1000);add("C47",5000,2000);add("C51",5000,10000);add("C54",4000,-5000);add("C54",5000,1000);
  SasraSetupAppService.ValidateVersion(d);
  var profile=new SasraProfileDTO{Profile="DT",InstitutionName="Test SACCO",RegistrationNumber="CS123"};
  var request=new SasraForm7Request{YearStart=new DateTime(2026,1,1),AsAt=new DateTime(2026,12,31)};
  var result=SasraForm7.Build(d,profile,request,data);
  Func<string,decimal> amount=cell=>result.Rows.Single(x=>x.Cell==cell).Amount.Value;
  Check(amount("C10")==120&&amount("C21")==10,"income and expenses use natural signs and thousands");
  Check(amount("C32")==3&&amount("C36")==18,"recoveries reduce provision and operating expenses total");
  Check(amount("C54")==4&&amount("C56")==82,"net donations add correctly to final income");
  Check(result.LedgerNetIncome==82&&result.Difference==0&&result.WorkbookBase64!=null,"net income reconciles and download available");
  var wb=new HSSFWorkbook(new MemoryStream(Convert.FromBase64String(result.WorkbookBase64)));var source=SasraForm7.Open();
  var sheet=wb.GetSheet(SasraForm7.Sheet);var original=source.GetSheet(SasraForm7.Sheet);
  Check(wb.NumberOfSheets==source.NumberOfSheets&&sheet.NumMergedRegions==original.NumMergedRegions,"sheet structure preserved");
  for(int row=0;row<=original.LastRowNum;row++)
  {
   var old=original.GetRow(row);if(old==null)continue;
   Check(sheet.GetRow(row).HeightInPoints==((row==17||row==18)?Math.Max(29f,old.HeightInPoints):old.HeightInPoints),"row height retained except two wrapped income labels");
   foreach(var cell in old.Cells)
   {
    var actual=sheet.GetRow(row).GetCell(cell.ColumnIndex);
    if(cell.ColumnIndex<2)Check(actual.ToString()==cell.ToString()&&actual.CellStyle.Index==cell.CellStyle.Index,"original labels and styles preserved");
    if(cell.CellType==CellType.Formula)Check(actual.CellFormula==cell.CellFormula&&actual.CachedFormulaResultType==CellType.Numeric,"every formula preserved and recalculated");
   }
  }
  Check(sheet.GetRow(55).GetCell(2).NumericCellValue==82,"final formula cache survives reopen");
  Check(sheet.GetRow(4).GetCell(2).DateCellValue==request.YearStart&&sheet.GetRow(5).GetCell(2).DateCellValue==request.AsAt,"typed dates exported");
  Check(sheet.PrintSetup.PaperSize==original.PrintSetup.PaperSize&&sheet.GetColumnWidth(1)==original.GetColumnWidth(1),"print setup and width preserved");
  foreach(var b in data)b.ClosingBalance=-b.Balance;
  result=SasraForm7.Build(d,profile,request,data);
  Check(result.LedgerNetIncome==82&&result.UnclosedSurplus==0&&result.ClosingAdjustment==82&&result.Difference==0,"closing transfers do not erase financial-year income");
  data[0].Balance=5000;
  result=SasraForm7.Build(d,profile,request,data);Check(result.Rows.Single(x=>x.Cell=="C12").Amount==-5,"abnormal debit income retains negative sign");
  var unmapped=new SasraForm7Balance{Id=Guid.NewGuid(),AccountType=5000,Balance=1000};data.Add(unmapped);
  result=SasraForm7.Build(d,profile,request,data);Check(result.UnmappedAccounts.Count==1&&result.WorkbookBase64==null&&result.Difference==1,"unmapped expense blocks export even with calculations present");
  data.Remove(unmapped);result=SasraForm7.Build(d,profile,request,data,1m);Check(result.WorkbookBase64==null&&result.LedgerDifference==0.001m,"independent ledger imbalance blocks export");
  Check(!SasraForm7.AllowsAccountType("C40",1000)&&SasraForm7.AllowsAccountType("C40",5000),"depreciation expense cannot use accumulated depreciation asset");
  Check(!SasraForm7.AllowsAccountType("C12",5000)&&SasraForm7.AllowsAccountType("C34",5000)&&SasraForm7.AllowsAccountType("C34",4000),"account categories reflect row meaning");
  var bad=SasraForm7.Definition();bad.Lines.Single(x=>x.Cell=="C12").Sign=1;Reject(()=>SasraSetupAppService.ValidateVersion(bad));
  bad=SasraForm7.Definition();bad.Lines.Single(x=>x.Cell=="C56").AccountIds.Add(Guid.NewGuid());Reject(()=>SasraSetupAppService.ValidateVersion(bad));
  bad=SasraForm7.Definition();bad.Lines[0].Cell="D10";Reject(()=>SasraSetupAppService.ValidateVersion(bad));
  bad=SasraForm7.Definition();bad.Lines.Single(x=>x.Cell=="C12").AccountIds.Add(data[0].Id);bad.Lines.Single(x=>x.Cell=="C19").AccountIds.Add(data[0].Id);Reject(()=>SasraSetupAppService.ValidateVersion(bad));
  result=SasraForm7.Build(SasraForm7.Definition(),profile,request,new List<SasraForm7Balance>());Check(result.WorkbookBase64!=null&&result.LedgerNetIncome==0,"zero activity supported");
  profile.RegistrationNumber=null;result=SasraForm7.Build(d,profile,request,data);Check(result.WorkbookBase64==null,"registration required for export");
  Console.WriteLine("Form 7: "+checks+" assertions passed.");
 }
}
