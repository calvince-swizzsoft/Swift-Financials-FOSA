using System;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Application.MainBoundedContext.DTO.AccountsModule;
using Domain.MainBoundedContext.AccountsModule.Aggregates.SasraAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
namespace Application.MainBoundedContext.AccountsModule.Services
{
    public partial class SasraSetupAppService
    {
        public SasraVersionDTO GetForm7Definition(ServiceHeader h)
        {
            using(scopes.CreateReadOnly())
            {
    // Workbook definitions and mappings can be read without institution setup.
                var latest=versions.AllMatching(new DirectSpecification<SasraTemplateVersion>(x=>x.Profile=="DT"&&x.ReportCode=="FORM 7"&&x.Version==SasraForm7.Version),h).OrderByDescending(x=>x.Revision).FirstOrDefault();
                return latest==null?SasraForm7.Definition():Dto(latest,h,true);
            }
        }
        public SasraForm7Result PreviewForm7(SasraForm7Request input,ServiceHeader h)
        {
            Check(input!=null&&input.VersionId!=Guid.Empty,"VersionId","Save Form 7 account mappings before previewing.");
            Check(input.YearStart.Year>=1753&&input.YearStart.Year<9999&&input.AsAt.Year<9999&&input.AsAt.Date>=input.YearStart.Date&&input.AsAt.Date<input.YearStart.Date.AddYears(1),"AsAt","Choose the financial year start and an as-at date within that financial year.");
            using(scopes.CreateReadOnlyWithTransaction(IsolationLevel.Serializable))
            {
                var profile=Current(h);Check(profile!=null&&profile.Profile=="DT","Profile","Save the DT institution profile before generating Form 7.");
                var v=versions.Get(input.VersionId,h);Check(v!=null,"VersionId","The selected Form 7 revision was not found.",404);
                var definition=Dto(v,h,true);Check(definition.Version==SasraForm7.Version,"VersionId","Open the supplied Form 7 workbook definition and save its mappings first.");
                ValidateVersion(definition);
                var balances=accounts.DatabaseSqlQuery<SasraForm7Balance>(Form7BalancesSql,h,
                    new SqlParameter("@Start",input.YearStart.Date),new SqlParameter("@End",input.AsAt.Date.AddDays(1)),new SqlParameter("@Closing",(int)SystemTransactionCode.FiscalPeriodClosing)).ToList();
                var ids=definition.Lines.SelectMany(x=>x.AccountIds).ToArray();
                var selected=accounts.AllMatching(new DirectSpecification<ChartOfAccount>(x=>ids.Contains(x.Id)),h);
                foreach(var line in definition.Lines.Where(x=>x.AccountIds.Count>0))
                    Check(selected.Count(x=>line.AccountIds.Contains(x.Id)&&SasraForm7.AllowsAccountType(line.Cell,x.AccountType))==line.AccountIds.Count,"AccountIds","A mapped account was removed or its category changed. Review and save the mappings again.");
                Check(accounts.AllMatchingCount(new DirectSpecification<ChartOfAccount>(x=>x.ParentId.HasValue&&ids.Contains(x.ParentId.Value)),h)==0,"AccountIds","A mapped account now has child accounts. Map its posting accounts instead.");
                var ledgerDifference=accounts.DatabaseSqlQuery<decimal>(@"SELECT COALESCE(SUM(e.Amount),0) FROM dbo.swiftFin_JournalEntries e
JOIN dbo.swiftFin_Journals j ON j.Id=e.JournalId
WHERE COALESCE(j.ValueDate,j.CreatedDate)>=@Start AND COALESCE(j.ValueDate,j.CreatedDate)<@End",h,new SqlParameter("@Start",input.YearStart.Date),new SqlParameter("@End",input.AsAt.Date.AddDays(1))).Single();
                return SasraForm7.Build(definition,ProfileDto(profile),input,balances,ledgerDifference);
            }
        }
        public const string Form7BalancesSql=@"
SELECT a.Id, CAST(a.AccountType AS int) AccountType,
CONVERT(nvarchar(20),a.AccountCode)+N' — '+a.AccountName Name,
SUM(CASE WHEN j.TransactionCode<>@Closing THEN e.Amount ELSE 0 END) Balance,
SUM(CASE WHEN j.TransactionCode=@Closing THEN e.Amount ELSE 0 END) ClosingBalance
FROM dbo.swiftFin_JournalEntries e
JOIN dbo.swiftFin_Journals j ON j.Id=e.JournalId
JOIN dbo.swiftFin_ChartOfAccounts a ON a.Id=e.ChartOfAccountId
WHERE a.AccountType IN (4000,5000) AND COALESCE(j.ValueDate,j.CreatedDate)>=@Start AND COALESCE(j.ValueDate,j.CreatedDate)<@End
GROUP BY a.Id,a.AccountType,a.AccountCode,a.AccountName
HAVING SUM(CASE WHEN j.TransactionCode<>@Closing THEN e.Amount ELSE 0 END)<>0 OR SUM(CASE WHEN j.TransactionCode=@Closing THEN e.Amount ELSE 0 END)<>0";
    }
}
