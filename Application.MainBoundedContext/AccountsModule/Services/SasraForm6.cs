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
    public static class SasraForm6
    {
        public const string Hash="cd81fc5daad85edd657b8a2f3a9ab7f634774ea7f35d54eb39bcdf98f64a6dea";
        public const string Version="WORKBOOK-CD81FC5DAAD8";
        public const string Sheet="Balance Sheet";
        static readonly int[] InputRows={11,12,14,17,18,19,20,23,24,27,28,29,32,33,34,35,36,42,43,44,48,49,50,51,52,53,57,58,61,62,65,66,67,68,69};
        public static HSSFWorkbook Open()
        {
            using(var stream=typeof(SasraForm6).Assembly.GetManifestResourceStream("Sasra.Form6.xls"))
            {
                if(stream==null)throw new InvalidOperationException("The Form 6 template resource is missing.");
                using(var hash=SHA256.Create())
                    if(BitConverter.ToString(hash.ComputeHash(stream)).Replace("-","").ToLowerInvariant()!=Hash)
                        throw new InvalidOperationException("The Form 6 template checksum does not match.");
                stream.Position=0;return new HSSFWorkbook(stream);
            }
        }
        public static SasraVersionDTO Definition()
        {
            var result=new SasraVersionDTO{Profile="DT",ReportCode="FORM 6",Title="Statement of Financial Position",Version=Version,WorkbookSha256=Hash,SourceUrl="https://www.sasra.go.ke/download/form-6-statement-of-financial-position/"};
            var wb=Open();
            {
                var s=wb.GetSheet(Sheet);
                for(int row=9;row<=72;row++)
                {
                    var r=s.GetRow(row-1);var label=r?.GetCell(1)?.ToString();
                    if(string.IsNullOrWhiteSpace(label))continue;
                    var formula=r.GetCell(2)?.CellType==CellType.Formula;
                    var input=InputRows.Contains(row);
                    result.Lines.Add(new SasraLineDTO{Code="F6_"+row.ToString("000"),Description=label.Trim(),
                        Source=formula?"Formula":row==62?"CurrentSurplus":input?"GlBalance":"Header",
                        Sheet=formula||input?Sheet:null,Cell=formula||input?"C"+row:null,
                        Sign=input&&(row==24||row>=42)?-1:1});
                }
            }
            return result;
        }
        public static void Validate(SasraVersionDTO value)
        {
            var expected=Definition();
            if(value.Profile!="DT"||value.ReportCode!="FORM 6"||value.WorkbookSha256!=Hash||value.Lines==null||value.Lines.Count!=expected.Lines.Count)
                throw new SasraSetupException("Template","Use the supplied Form 6 definition. Only account mappings can be changed.");
            for(int i=0;i<expected.Lines.Count;i++)
            {
                var a=expected.Lines[i];var b=value.Lines[i];
                if(b==null||a.Code!=b.Code||a.Description!=b.Description||a.Source!=b.Source||a.Sheet!=b.Sheet||a.Cell!=b.Cell||a.Sign!=b.Sign||b.AccountIds==null||((a.Source=="Header"||a.Source=="Formula"||a.Source=="CurrentSurplus")&&b.AccountIds.Count>0))
                    throw new SasraSetupException("Lines","Form 6 rows, signs and formulas are fixed by the workbook. Reload Form 6 and edit its account mappings.");
            }
        }
        public static SasraForm6Result Build(SasraVersionDTO version,SasraProfileDTO profile,SasraForm6Request request,List<SasraForm6Balance> balances)
        {
            Validate(version);
            var result=new SasraForm6Result{VersionId=version.Id,YearStart=request.YearStart.Date,AsAt=request.AsAt.Date,InstitutionName=profile.InstitutionName,GeneratedAtUtc=DateTime.UtcNow};
            var used=new HashSet<Guid>(version.Lines.SelectMany(x=>x.AccountIds));
            var pnl=balances.Where(x=>x.AccountType==4000||x.AccountType==5000).ToList();
            result.UnmappedAccounts=balances.Where(x=>x.Balance!=0&&!used.Contains(x.Id)&&x.AccountType!=4000&&x.AccountType!=5000).ToList();
            if(result.UnmappedAccounts.Count>0)result.Issues.Add("Map every account with a non-zero closing balance before downloading.");
            result.LedgerDifference=balances.Sum(x=>x.Balance)/1000m;
            if(Math.Abs(result.LedgerDifference)>0.00001m)result.Issues.Add("The underlying ledger does not balance. Resolve the ledger difference before downloading.");
            if(string.IsNullOrWhiteSpace(profile.RegistrationNumber))result.Issues.Add("Enter the SACCO registration number in institution settings before downloading.");
            var wb=Open();
            {
                var sheet=wb.GetSheet(Sheet);
                Action<int,string> text=(row,value)=> (sheet.GetRow(row-1).GetCell(2)??sheet.GetRow(row-1).CreateCell(2)).SetCellValue(value);
                text(3,profile.RegistrationNumber??"");text(4,request.YearStart.Year==request.AsAt.Year?request.AsAt.Year.ToString():request.YearStart.Year+"/"+request.AsAt.Year);
                foreach(var pair in new[]{Tuple.Create(5,request.YearStart.Date),Tuple.Create(6,request.AsAt.Date)})
                {
                    var c=sheet.GetRow(pair.Item1-1).GetCell(2)??sheet.GetRow(pair.Item1-1).CreateCell(2);
                    var style=wb.CreateCellStyle();style.CloneStyleFrom(c.CellStyle);style.DataFormat=wb.CreateDataFormat().GetFormat("dd/mm/yyyy");c.CellStyle=style;c.SetCellValue(pair.Item2);
                }
                foreach(var line in version.Lines.Where(x=>x.Source=="GlBalance"||x.Source=="CurrentSurplus"))
                {
                    decimal amount=line.Source=="CurrentSurplus"?-pnl.Sum(x=>x.Balance-x.BeforeYear):balances.Where(x=>line.AccountIds.Contains(x.Id)).Sum(x=>x.Balance)*line.Sign;
                    // Unclosed prior-year income/expenses belong in accumulated surplus, not the current year.
                    if(line.Cell=="C61")amount-=pnl.Sum(x=>x.BeforeYear);
                    var row=int.Parse(line.Cell.Substring(1))-1;
                    (sheet.GetRow(row).GetCell(2)??sheet.GetRow(row).CreateCell(2)).SetCellValue((double)(amount/1000m));
                }
                var evaluator=new HSSFFormulaEvaluator(wb);evaluator.EvaluateAll();
                foreach(var line in version.Lines)
                {
                    var row=new SasraForm6Row{Description=line.Description,Cell=line.Cell,Source=line.Source};
                    if(line.Cell!=null)
                    {
                        var cell=sheet.GetRow(int.Parse(line.Cell.Substring(1))-1).GetCell(2);
                        if(cell.CellType==CellType.Formula){row.Formula=cell.CellFormula;if(cell.CachedFormulaResultType!=CellType.Numeric)throw new SasraSetupException("Formula","A Form 6 formula could not be calculated.");}
                        row.Amount=Convert.ToDecimal(cell.NumericCellValue);
                    }
                    result.Rows.Add(row);
                }
                result.Difference=result.Rows.Single(x=>x.Cell=="C38").Amount.Value-result.Rows.Single(x=>x.Cell=="C72").Amount.Value;
                if(Math.Abs(result.Difference)>0.00001m)result.Issues.Add("Total assets and total liabilities plus equity do not agree. Review mappings and account categories.");
                if(result.Issues.Count==0){using(var output=new MemoryStream()){wb.Write(output);result.WorkbookBase64=Convert.ToBase64String(output.ToArray());}}
            }
            return result;
        }
    }
}
