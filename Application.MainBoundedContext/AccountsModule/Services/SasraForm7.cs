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
    // This identifier pins the uploaded workbook, not a claimed regulatory effective date.
    public static class SasraForm7
    {
        public const string Hash="a4d3f3d375ee0e03b5db865e05103f9086bfceb202e31c2c7bce91c7cb7116ca";
        public const string Version="WORKBOOK-A4D3F3D375EE";
        public const string Sheet="Income Statement";
        static readonly int[] InputRows={12,13,16,17,18,19,22,23,24,25,27,28,33,34,37,38,39,40,41,46,47,51,54};
        public static HSSFWorkbook Open()
        {
            using(var stream=typeof(SasraForm7).Assembly.GetManifestResourceStream("Sasra.Form7.xls"))
            {
                if(stream==null)throw new InvalidOperationException("The Form 7 template resource is missing.");
                using(var hash=SHA256.Create())
                    if(BitConverter.ToString(hash.ComputeHash(stream)).Replace("-","").ToLowerInvariant()!=Hash)
                        throw new InvalidOperationException("The Form 7 template checksum does not match.");
                stream.Position=0;return new HSSFWorkbook(stream);
            }
        }
        public static SasraVersionDTO Definition()
        {
            var result=new SasraVersionDTO{Profile="DT",ReportCode="FORM 7",Title="Statement of Comprehensive Income",Version=Version,WorkbookSha256=Hash,SourceUrl="https://www.sasra.go.ke/download/form-7-statement-of-comprehensive-income/"};
            var wb=Open();
            {
                var s=wb.GetSheet(Sheet);
                for(int row=10;row<=56;row++)
                {
                    var r=s.GetRow(row-1);var label=r?.GetCell(1)?.ToString();
                    if(string.IsNullOrWhiteSpace(label))continue;
                    var formula=r.GetCell(2)?.CellType==CellType.Formula;
                    var input=InputRows.Contains(row);
                    result.Lines.Add(new SasraLineDTO{Code="F7_"+row.ToString("000"),Description=label.Trim(),
                        Source=formula?"Formula":input?"GlMovement":"Header",
                        Sheet=formula||input?Sheet:null,Cell=formula||input?"C"+row:null,
                        Sign=input&&(row<20||row==34||row==46||row==54)?-1:1});
                }
            }
            return result;
        }
        public static bool AllowsAccountType(string cell,int type)
        {
            if(type!=4000&&type!=5000)return false;
            int row=int.Parse(cell.Substring(1));
            if(row==34||row==54)return true; // Loan-loss recoveries and net donations may use credit income or contra-expense accounts.
            return row<20||row==46 ? type==4000 : type==5000;
        }
        public static void Validate(SasraVersionDTO value)
        {
            var expected=Definition();
            if(value.Profile!="DT"||value.ReportCode!="FORM 7"||value.WorkbookSha256!=Hash||value.Lines==null||value.Lines.Count!=expected.Lines.Count)
                throw new SasraSetupException("Template","Use the supplied Form 7 definition. Only account mappings can be changed.");
            for(int i=0;i<expected.Lines.Count;i++)
            {
                var a=expected.Lines[i];var b=value.Lines[i];
                if(b==null||a.Code!=b.Code||a.Description!=b.Description||a.Source!=b.Source||a.Sheet!=b.Sheet||a.Cell!=b.Cell||a.Sign!=b.Sign||b.AccountIds==null||((a.Source=="Header"||a.Source=="Formula")&&b.AccountIds.Count>0))
                    throw new SasraSetupException("Lines","Form 7 rows, signs and formulas are fixed by the workbook. Reload Form 7 and edit its account mappings.");
            }
        }
        public static SasraForm7Result Build(SasraVersionDTO version,SasraProfileDTO profile,SasraForm7Request request,List<SasraForm7Balance> balances,decimal ledgerDifference=0m)
        {
            Validate(version);
            var result=new SasraForm7Result{VersionId=version.Id,YearStart=request.YearStart.Date,AsAt=request.AsAt.Date,InstitutionName=profile.InstitutionName,GeneratedAtUtc=DateTime.UtcNow};
            var used=new HashSet<Guid>(version.Lines.SelectMany(x=>x.AccountIds));
            result.UnmappedAccounts=balances.Where(x=>(x.Balance!=0||x.ClosingBalance!=0)&&!used.Contains(x.Id)).ToList();
            if(result.UnmappedAccounts.Count>0)result.Issues.Add("Map every income and expense account with activity before downloading.");
            result.LedgerNetIncome=-balances.Sum(x=>x.Balance)/1000m;
            result.UnclosedSurplus=-balances.Sum(x=>x.Balance+x.ClosingBalance)/1000m;
            result.ClosingAdjustment=result.LedgerNetIncome-result.UnclosedSurplus;
            result.LedgerDifference=ledgerDifference/1000m;
            if(Math.Abs(result.LedgerDifference)>0.00001m)result.Issues.Add("The underlying ledger does not balance for this period. Resolve the ledger difference before downloading.");
            if(string.IsNullOrWhiteSpace(profile.RegistrationNumber))result.Issues.Add("Enter the SACCO registration number in institution settings before downloading.");
            var wb=Open();
            {
                var sheet=wb.GetSheet(Sheet);
                // The supplied template wraps these long descriptions but fixes their height to one line.
                // Allow two lines when adjacent amount cells are populated.
                foreach(var rowNumber in new[]{18,19})
                    sheet.GetRow(rowNumber-1).HeightInPoints=Math.Max(29f,sheet.GetRow(rowNumber-1).HeightInPoints);
                Action<int,string> text=(row,value)=> (sheet.GetRow(row-1).GetCell(2)??sheet.GetRow(row-1).CreateCell(2)).SetCellValue(value);
                text(3,profile.RegistrationNumber??"");text(4,request.YearStart.Year==request.AsAt.Year?request.AsAt.Year.ToString():request.YearStart.Year+"/"+request.AsAt.Year);
                foreach(var pair in new[]{Tuple.Create(5,request.YearStart.Date),Tuple.Create(6,request.AsAt.Date)})
                {
                    var c=sheet.GetRow(pair.Item1-1).GetCell(2)??sheet.GetRow(pair.Item1-1).CreateCell(2);
                    var style=wb.CreateCellStyle();style.CloneStyleFrom(c.CellStyle);style.DataFormat=wb.CreateDataFormat().GetFormat("dd/mm/yyyy");c.CellStyle=style;c.SetCellValue(pair.Item2);
                }
                foreach(var line in version.Lines.Where(x=>x.Source=="GlMovement"))
                {
                    decimal amount=balances.Where(x=>line.AccountIds.Contains(x.Id)).Sum(x=>x.Balance)*line.Sign;
                    var row=int.Parse(line.Cell.Substring(1))-1;
                    (sheet.GetRow(row).GetCell(2)??sheet.GetRow(row).CreateCell(2)).SetCellValue((double)(amount/1000m));
                }
                var evaluator=new HSSFFormulaEvaluator(wb);evaluator.EvaluateAll();
                foreach(var line in version.Lines)
                {
                    var row=new SasraForm7Row{Description=line.Description,Cell=line.Cell,Source=line.Source};
                    if(line.Cell!=null)
                    {
                        var cell=sheet.GetRow(int.Parse(line.Cell.Substring(1))-1).GetCell(2);
                        if(cell.CellType==CellType.Formula){row.Formula=cell.CellFormula;if(cell.CachedFormulaResultType!=CellType.Numeric)throw new SasraSetupException("Formula","A Form 7 formula could not be calculated.");}
                        row.Amount=Convert.ToDecimal(cell.NumericCellValue);
                    }
                    result.Rows.Add(row);
                }
                result.Difference=result.Rows.Single(x=>x.Cell=="C56").Amount.Value-result.LedgerNetIncome;
                if(Math.Abs(result.Difference)>0.00001m)result.Issues.Add("Net income does not agree with the income and expense ledger. Review mappings and account categories.");
                if(result.Issues.Count==0){using(var output=new MemoryStream()){wb.Write(output);result.WorkbookBase64=Convert.ToBase64String(output.ToArray());}}
            }
            return result;
        }
    }
}
