using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
static class Form6Tests
{
    static int count;
    static void Check(bool value,string label){if(!value)throw new Exception("Form6: "+label);count++;}
    static void Reject(Action action,string label){try{action();throw new Exception("Accepted "+label);}catch(SasraSetupException){count++;}}
    public static void Run()
    {
        var d=SasraForm6.Definition();SasraSetupAppService.ValidateVersion(d);
        var cash=Guid.NewGuid();var loans=Guid.NewGuid();var allowance=Guid.NewGuid();var deposits=Guid.NewGuid();var capital=Guid.NewGuid();
        foreach(var pair in new[]{Tuple.Create("C11",cash),Tuple.Create("C23",loans),Tuple.Create("C24",allowance),Tuple.Create("C42",deposits),Tuple.Create("C57",capital)})d.Lines.Single(l=>l.Cell==pair.Item1).AccountIds.Add(pair.Item2);
        var balances=new List<SasraForm6Balance>{
            new SasraForm6Balance{Id=cash,AccountType=1000,Balance=100000},
            new SasraForm6Balance{Id=loans,AccountType=1000,Balance=200000},
            new SasraForm6Balance{Id=allowance,AccountType=1000,Balance=-10000},
            new SasraForm6Balance{Id=deposits,AccountType=2000,Balance=-240000},
            new SasraForm6Balance{Id=capital,AccountType=3000,Balance=-20000},
            new SasraForm6Balance{Id=Guid.NewGuid(),AccountType=4000,Balance=-40000,BeforeYear=-15000},
            new SasraForm6Balance{Id=Guid.NewGuid(),AccountType=5000,Balance=10000,BeforeYear=5000}};
        var profile=new SasraProfileDTO{Profile="DT",InstitutionName="Test SACCO",RegistrationNumber="CS123"};
        var request=new SasraForm6Request{YearStart=new DateTime(2026,1,1),AsAt=new DateTime(2026,9,30)};
        var result=SasraForm6.Build(d,profile,request,balances);
        Func<string,decimal> amount=cell=>result.Rows.Single(l=>l.Cell==cell).Amount.Value;
        Check(amount("C24")==10&&amount("C22")==190,"positive allowance is deducted from gross loans");
        Check(amount("C61")==10&&amount("C62")==20,"prior unclosed surplus separated from current year");
        Check(amount("C38")==290&&amount("C72")==290,"signed balances reconcile in thousands");
        Check(result.Difference==0&&result.LedgerDifference==0&&result.Issues.Count==0,"independent controls pass");
        Check(!string.IsNullOrEmpty(result.WorkbookBase64),"balanced report downloadable");
        var output=new HSSFWorkbook(new MemoryStream(Convert.FromBase64String(result.WorkbookBase64)));
        var original=SasraForm6.Open();var s=output.GetSheet("Balance Sheet");var source=original.GetSheet("Balance Sheet");
        Check(output.NumberOfSheets==original.NumberOfSheets&&s.NumMergedRegions==source.NumMergedRegions,"sheets and merges preserved");
        Check(s.GetRow(21).GetCell(2).CellFormula=="C23-C24"&&s.GetRow(21).GetCell(2).NumericCellValue==190,"formula and recalculated cache preserved on reopen");
        Check(s.GetRow(2).GetCell(2).StringCellValue=="CS123"&&s.GetRow(4).GetCell(2).DateCellValue==request.YearStart,"registration and typed start date");
        for(int row=0;row<=source.LastRowNum;row++)
        {
            var originalRow=source.GetRow(row);if(originalRow==null)continue;
            foreach(var c in originalRow.Cells)
            {
                var actual=s.GetRow(row).GetCell(c.ColumnIndex);
                if(c.CellType==CellType.Formula)Check(actual.CellFormula==c.CellFormula&&actual.CachedFormulaResultType==CellType.Numeric,"all template formulas preserved and evaluated");
                if(c.ColumnIndex<2)Check(actual.ToString()==c.ToString()&&actual.CellStyle.Index==c.CellStyle.Index,"labels and styles unchanged");
            }
            Check(s.GetRow(row).Height==originalRow.Height,"row height preserved");
        }
        Check(s.PrintSetup.PaperSize==source.PrintSetup.PaperSize&&s.GetColumnWidth(1)==source.GetColumnWidth(1),"print setup and column width preserved");
        d.Lines.Single(l=>l.Cell=="C12").AccountIds.Add(cash);
        Reject(()=>SasraSetupAppService.ValidateVersion(d),"duplicate account");d.Lines.Single(l=>l.Cell=="C12").AccountIds.Clear();
        var bad=SasraForm6.Definition();bad.Lines.Single(l=>l.Cell=="C24").Sign=1;Reject(()=>SasraSetupAppService.ValidateVersion(bad),"allowance sign tampering");
        bad=SasraForm6.Definition();bad.Lines.Single(l=>l.Cell=="C38").AccountIds.Add(Guid.NewGuid());Reject(()=>SasraSetupAppService.ValidateVersion(bad),"mapping total row");
        balances[0].Balance=-1000;balances[3].Balance=-139000;
        result=SasraForm6.Build(d,profile,request,balances);Check(result.Rows.Single(l=>l.Cell=="C11").Amount==-1&&result.Difference==0,"abnormal debit/credit sign never forced positive");
        balances.Add(new SasraForm6Balance{Id=Guid.NewGuid(),AccountType=1000,Balance=100,Name="Unmapped"});
        result=SasraForm6.Build(d,profile,request,balances);Check(result.UnmappedAccounts.Count==1&&result.WorkbookBase64==null,"unmapped nonzero account blocks download");Check(result.LedgerDifference==0.1m,"unbalanced ledger detected independently");
        result=SasraForm6.Build(SasraForm6.Definition(),profile,request,new List<SasraForm6Balance>());Check(result.Difference==0&&result.WorkbookBase64!=null,"real zero ledger supported");
        profile.RegistrationNumber="";result=SasraForm6.Build(d,profile,request,new List<SasraForm6Balance>());Check(result.WorkbookBase64==null&&result.Issues.Count==1,"missing registration blocks download");
        Console.WriteLine("Form 6: "+count+" assertions passed.");
    }
}
