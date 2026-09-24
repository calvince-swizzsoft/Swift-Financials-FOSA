using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using Application.MainBoundedContext.DTO.AccountsModule;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
namespace Application.MainBoundedContext.AccountsModule.Services
{
    public static class SasraForm9
    {
        public const string TemplateHash="07aa18b8f484c4a1939b3a453e80fb1ad297b6a27d660b78fff0b16f59019186";
        public static string Hash(byte[] bytes){using(var sha=SHA256.Create())return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();}
        public static string Hash(string value){return Hash(Encoding.UTF8.GetBytes(value));}
        static void Check(bool ok,string field,string message){if(!ok)throw new SasraSetupException(field,message);}
        public static bool Active(InsiderAppointmentDTO a,DateTime day){return !a.IsVoided&&a.StartsAt.Date<=day.Date&&(!a.EndsAt.HasValue||a.EndsAt.Value.Date>=day.Date);}
        public static void ValidateMonth(Form9Request input)
        {
            Check(input!=null,"Month","Select the reporting month.");
            Check(input.Month.Year>=1900&&input.Month.Day==1&&input.Month.TimeOfDay==TimeSpan.Zero&&input.Month<DateTime.Today.AddDays(1-DateTime.Today.Day),"Month","Select the first day of a completed reporting month.");
        }
        public static void ValidatePolicy(Form9PolicyDTO p)
        {
            Check(p!=null,"Policy","Save the Form 9 reporting policy.");
            Check(new[]{"BoardDecision","Disbursement"}.Contains(p.GrantBasis),"GrantBasis","Confirm how new grants are selected: board decision date or disbursement date.");
            Check(new[]{"AtGrant","AtPeriodEnd","AtGrantOrPeriodEnd"}.Contains(p.PopulationBasis),"PopulationBasis","Confirm the date used to identify insiders.");
            Check(new[]{"Principal","PrincipalAndInterest"}.Contains(p.OutstandingBasis),"OutstandingBasis","Confirm whether outstanding includes interest.");
            Check(new[]{"AtPeriodEnd","AtBoardDecision"}.Contains(p.DepositBasis),"DepositBasis","Confirm the BOSA deposit valuation date.");
            Check(new[]{"AllOutstanding","PriorMonthsOnly"}.Contains(p.SectionBBasis),"SectionBBasis","Confirm whether Section B includes this month's grants.");
            Check(new[]{"Block","OldestDueFirst"}.Contains(p.SharedAllocation),"SharedAllocation","Confirm the treatment of repayments shared between loans.");
            Check(new[]{"ReviewRequired","ExportNil"}.Contains(p.NilReturn),"NilReturn","Confirm the treatment of a nil return.");
            Check(p.BosaProductIds!=null&&p.BosaProductIds.Count>0&&p.BosaProductIds.Count<=100&&p.BosaProductIds.All(x=>x!=Guid.Empty)&&p.BosaProductIds.Distinct().Count()==p.BosaProductIds.Count,"BosaProductIds","Map one to 100 distinct BOSA deposit products.");
            Check(p.RulesConfirmed&&!string.IsNullOrWhiteSpace(p.Evidence),"RulesConfirmed","Record the reporting officer's confirmation and policy evidence.");
        }
        public static byte[] Export(Form9Result result)
        {
            XSSFWorkbook workbook;
            using(var stream=typeof(SasraForm9).Assembly.GetManifestResourceStream("Sasra.Form9.xlsx")){
                if(stream==null)throw new InvalidOperationException("The official Form 9 template is missing.");
                using(var copy=new MemoryStream()){stream.CopyTo(copy);var bytes=copy.ToArray();if(Hash(bytes)!=TemplateHash)throw new InvalidOperationException("The Form 9 template checksum does not match.");workbook=new XSSFWorkbook(new MemoryStream(bytes));}
            }
            var sheet=workbook.GetSheet("Insider return");
            int extraA=Math.Max(0,result.NewLoans.Count-5),extraB=Math.Max(0,result.OutstandingLoans.Count-5);
            // Expand the lower section first, then shift it with the upper expansion.
            if(extraB>0)sheet.ShiftRows(25,sheet.LastRowNum,extraB,true,false);
            if(extraA>0)sheet.ShiftRows(15,sheet.LastRowNum,extraA,true,false);
            int startA=10,startB=20+extraA,totalA=16+extraA,totalB=26+extraA+extraB;
            var date=workbook.CreateCellStyle();date.DataFormat=workbook.CreateDataFormat().GetFormat("dd/mm/yyyy");
            var money=workbook.CreateCellStyle();money.DataFormat=workbook.CreateDataFormat().GetFormat("#,##0.00");
            var text=workbook.CreateCellStyle();text.WrapText=true;
            Action<int,int,object> put=(r,c,value)=>{
                var row=sheet.GetRow(r)??sheet.CreateRow(r);var cell=row.GetCell(c)??row.CreateCell(c);
                if(value==null){cell.SetCellType(CellType.Blank);return;}
                if(value is DateTime){cell.SetCellValue((DateTime)value);cell.CellStyle=date;}
                else if(value is decimal){cell.SetCellValue((double)(decimal)value);cell.CellStyle=money;}
                else if(value is int)cell.SetCellValue((int)value);
                else{cell.SetCellValue(value.ToString());cell.CellStyle=text;}
            };
            put(0,1,"WORKING DRAFT - NOT APPROVED OR SUBMITTED");
            put(2,3,result.InstitutionName);put(3,3,result.RegistrationNumber);put(4,3,result.Month);put(5,3,result.AsAt);
            Action<int,Form9Row,bool,int> write=(r,x,b,n)=>{
                var row=sheet.GetRow(r)??sheet.CreateRow(r);row.HeightInPoints=48;
                put(r,1,n);put(r,2,x.Borrower);put(r,3,x.MemberNumber);put(r,4,x.Position);put(r,5,x.Product);
                put(r,6,x.Applied);put(r,7,x.Granted);put(r,8,x.DecisionDate);put(r,9,x.BosaDeposits);put(r,10,x.Security);
                put(r,11,x.FirstDueDate);put(r,12,x.TermMonths);
                if(b){put(r,13,x.Outstanding);put(r,14,x.Performance);}else put(r,13,x.Remarks);
            };
            for(int i=0;i<result.NewLoans.Count;i++)write(startA+i,result.NewLoans[i],false,i+1);
            for(int i=0;i<result.OutstandingLoans.Count;i++)write(startB+i,result.OutstandingLoans[i],true,i+1);
            (sheet.GetRow(totalA).GetCell(7)??sheet.GetRow(totalA).CreateCell(7)).SetCellFormula("SUM(H11:H"+(15+extraA)+")");
            (sheet.GetRow(totalB).GetCell(13)??sheet.GetRow(totalB).CreateCell(13)).SetCellFormula("SUM(N"+(startB+1)+":N"+(startB+Math.Max(5,result.OutstandingLoans.Count))+")");
            var notes=workbook.CreateSheet("Validation");
            string[] lines=new[]{"Working draft. Review and approval are not configured.","Snapshot: "+result.Id,"Snapshot SHA-256: "+result.SnapshotHash,"Template SHA-256: "+result.TemplateHash,"Prepared by: "+result.CreatedBy+" at "+result.CreatedAt.ToString("u"),"Repayment period is expressed in months.","Outstanding totals include known values only where exceptions exist.","Rules and BOSA mappings: "+Newtonsoft.Json.JsonConvert.SerializeObject(result.Policy)};
            int nrow=0;foreach(var line in lines)notes.CreateRow(nrow++).CreateCell(0).SetCellValue(line);
            foreach(var issue in result.Issues)notes.CreateRow(nrow++).CreateCell(0).SetCellValue(issue);
            foreach(var row in result.NewLoans.Concat(result.OutstandingLoans).GroupBy(x=>x.LoanCaseId).Select(g=>g.First()))
                foreach(var issue in row.Issues)notes.CreateRow(nrow++).CreateCell(0).SetCellValue("Loan "+row.CaseNumber+": "+issue);
            foreach(var warning in result.Warnings)notes.CreateRow(nrow++).CreateCell(0).SetCellValue(warning);
            notes.SetColumnWidth(0,110*256);
            sheet.Footer.Left="Working draft - review Validation sheet";
            sheet.PrintSetup.Landscape=true;sheet.PrintSetup.FitWidth=1;sheet.PrintSetup.FitHeight=0;sheet.FitToPage=true;
            workbook.SetPrintArea(0,1,14,0,sheet.LastRowNum);
            XSSFFormulaEvaluator.EvaluateAllFormulaCells(workbook);
            using(var output=new MemoryStream()){workbook.Write(output);return output.ToArray();}
        }
    }
}
