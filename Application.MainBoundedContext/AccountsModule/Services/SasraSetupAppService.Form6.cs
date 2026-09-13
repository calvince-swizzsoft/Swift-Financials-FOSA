using System;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Application.MainBoundedContext.DTO.AccountsModule;
using Domain.MainBoundedContext.AccountsModule.Aggregates.SasraAgg;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
namespace Application.MainBoundedContext.AccountsModule.Services
{
    public partial class SasraSetupAppService
    {
        public SasraVersionDTO GetForm6Definition(ServiceHeader h)
        {
            using(scopes.CreateReadOnly())
            {
    // Workbook definitions and mappings can be read without institution setup.
                var latest=versions.AllMatching(new DirectSpecification<SasraTemplateVersion>(x=>x.Profile=="DT"&&x.ReportCode=="FORM 6"&&x.Version==SasraForm6.Version),h).OrderByDescending(x=>x.Revision).FirstOrDefault();
                return latest==null?SasraForm6.Definition():Dto(latest,h,true);
            }
        }
        public SasraForm6Result PreviewForm6(SasraForm6Request input,ServiceHeader h)
        {
            Check(input!=null&&input.VersionId!=Guid.Empty,"VersionId","Save Form 6 account mappings before previewing.");
            Check(input.YearStart.Year>=1753&&input.YearStart.Year<9999&&input.AsAt.Year<9999&&input.AsAt.Date>=input.YearStart.Date&&input.AsAt.Date<input.YearStart.Date.AddYears(1),"AsAt","Choose the financial year start and an as-at date within that financial year.");
            using(scopes.CreateReadOnlyWithTransaction(IsolationLevel.Serializable))
            {
                var profile=Current(h);Check(profile!=null&&profile.Profile=="DT","Profile","Save the DT institution profile before generating Form 6.");
                var v=versions.Get(input.VersionId,h);Check(v!=null,"VersionId","The selected Form 6 revision was not found.",404);
                var definition=Dto(v,h,true);Check(definition.Version==SasraForm6.Version,"VersionId","Open the supplied Form 6 workbook definition and save its mappings first.");
                ValidateVersion(definition);
                var balances=accounts.DatabaseSqlQuery<SasraForm6Balance>(@"
SELECT a.Id, CAST(a.AccountType AS int) AccountType,
CONVERT(nvarchar(20),a.AccountCode)+N' — '+a.AccountName Name,
SUM(e.Amount) Balance,
SUM(CASE WHEN COALESCE(j.ValueDate,j.CreatedDate)<@Start THEN e.Amount ELSE 0 END) BeforeYear
FROM dbo.swiftFin_JournalEntries e
JOIN dbo.swiftFin_Journals j ON j.Id=e.JournalId
JOIN dbo.swiftFin_ChartOfAccounts a ON a.Id=e.ChartOfAccountId
WHERE COALESCE(j.ValueDate,j.CreatedDate)<@End
GROUP BY a.Id,a.AccountType,a.AccountCode,a.AccountName
HAVING SUM(e.Amount)<>0 OR SUM(CASE WHEN COALESCE(j.ValueDate,j.CreatedDate)<@Start THEN e.Amount ELSE 0 END)<>0",h,
                    new SqlParameter("@Start",input.YearStart.Date),new SqlParameter("@End",input.AsAt.Date.AddDays(1))).ToList();
                var ids=definition.Lines.SelectMany(x=>x.AccountIds).ToArray();
                var selected=accounts.AllMatching(new DirectSpecification<Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg.ChartOfAccount>(x=>ids.Contains(x.Id)),h);
                foreach(var line in definition.Lines.Where(x=>x.AccountIds.Count>0))
                {
                    var row=int.Parse(line.Cell.Substring(1));var expected=row<38?1000:row<54?2000:3000;
                    Check(selected.Count(x=>line.AccountIds.Contains(x.Id)&&x.AccountType==expected)==line.AccountIds.Count,"AccountIds","A mapped account was removed or its category changed. Review and save the mappings again.");
                }
                return SasraForm6.Build(definition,ProfileDto(profile),input,balances);
            }
        }
    }
}
