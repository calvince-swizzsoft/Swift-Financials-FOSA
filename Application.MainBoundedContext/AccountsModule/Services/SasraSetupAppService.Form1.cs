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
  public SasraVersionDTO GetForm1Definition(ServiceHeader h)
  {
   using(scopes.CreateReadOnly())
   {
    // Workbook definitions and mappings can be read without institution setup.
    var latest=versions.AllMatching(new DirectSpecification<SasraTemplateVersion>(x=>x.Profile=="DT"&&x.ReportCode=="FORM 1"&&x.Version==SasraForm1.Version),h).OrderByDescending(x=>x.Revision).FirstOrDefault();
    if(latest!=null)return Dto(latest,h,true);
    var definition=SasraForm1.Definition();var sofp=GetForm6Definition(h);
    // Reuse established SOFP classifications only where their meaning carries over.
    var crosswalk=new[]{Tuple.Create("D10",new[]{"C57"}),Tuple.Create("D11",new[]{"C65"}),Tuple.Create("D12",new[]{"C61"}),Tuple.Create("D14",new[]{"C58"}),
     Tuple.Create("D26",new[]{"C11"}),Tuple.Create("D27",new[]{"C17"}),Tuple.Create("D28",new[]{"C12","C19"}),Tuple.Create("D29",new[]{"C23","C24"}),Tuple.Create("D30",new[]{"C18","C20"}),Tuple.Create("D31",new[]{"C32","C33"}),Tuple.Create("D32",new[]{"C14","C27","C28","C29","C34","C35","C36"})};
    foreach(var link in crosswalk){var line=definition.Lines.Single(x=>x.Cell==link.Item1);foreach(var source in sofp.Lines.Where(x=>link.Item2.Contains(x.Cell)))foreach(var id in source.AccountIds){line.AccountIds.Add(id);line.AccountNames[id]=source.AccountNames[id];}}
    return definition;
   }
  }
  public SasraForm1Result PreviewForm1(SasraForm1Request input,ServiceHeader h)
  {
   SasraForm1.ValidateRequest(input);
   using(scopes.CreateReadOnlyWithTransaction(IsolationLevel.Serializable))
   {
    var profile=Current(h);Check(profile!=null&&profile.Profile=="DT","Profile","Save the DT institution profile before generating Form 1.");
    var v=versions.Get(input.VersionId,h);Check(v!=null&&v.Version==SasraForm1.Version,"VersionId","Select a saved Form 1 workbook revision.",404);var definition=Dto(v,h,true);ValidateVersion(definition);
    var source=versions.Get(input.Form6VersionId,h);Check(source!=null&&source.Version==SasraForm6.Version,"Form6VersionId","Select a saved Form 6 workbook revision.",404);var sofpDefinition=Dto(source,h,true);
    // Read and lock the ledger first. The nested Form 6 read sees the same balances while this serializable scope remains open.
    var balances=accounts.DatabaseSqlQuery<SasraForm1Balance>(Form1BalancesSql,h,new SqlParameter("@Start",input.YearStart.Date),new SqlParameter("@End",input.AsAt.Date.AddDays(1)),new SqlParameter("@Closing",(int)SystemTransactionCode.FiscalPeriodClosing)).ToList();
    var ids=definition.Lines.SelectMany(x=>x.AccountIds).ToArray();var selected=accounts.AllMatching(new DirectSpecification<ChartOfAccount>(x=>ids.Contains(x.Id)),h);
    foreach(var line in definition.Lines.Where(x=>x.AccountIds.Count>0))Check(selected.Count(x=>line.AccountIds.Contains(x.Id)&&SasraForm1.AllowsAccountType(line.Cell,x.AccountType))==line.AccountIds.Count,"AccountIds","A mapped account was removed or its category changed. Review the mappings.");
    Check(accounts.AllMatchingCount(new DirectSpecification<ChartOfAccount>(x=>x.ParentId.HasValue&&ids.Contains(x.ParentId.Value)),h)==0,"AccountIds","Map posting accounts without children.");
    var sofp=PreviewForm6(new SasraForm6Request{VersionId=input.Form6VersionId,YearStart=input.YearStart,AsAt=input.AsAt},h);
    return SasraForm1.Build(definition,ProfileDto(profile),input,balances,sofpDefinition,sofp);
   }
  }
  public const string Form1BalancesSql=@"
SELECT a.Id,CAST(a.AccountType AS int) AccountType,CONVERT(nvarchar(20),a.AccountCode)+N' — '+a.AccountName Name,
SUM(e.Amount) Balance,
SUM(CASE WHEN COALESCE(j.ValueDate,j.CreatedDate)<@Start THEN e.Amount ELSE 0 END) BeforeYear,
SUM(CASE WHEN COALESCE(j.ValueDate,j.CreatedDate)>=@Start AND j.TransactionCode=@Closing THEN e.Amount ELSE 0 END) CurrentClosingBalance
FROM dbo.swiftFin_JournalEntries e JOIN dbo.swiftFin_Journals j ON j.Id=e.JournalId JOIN dbo.swiftFin_ChartOfAccounts a ON a.Id=e.ChartOfAccountId
WHERE COALESCE(j.ValueDate,j.CreatedDate)<@End GROUP BY a.Id,a.AccountType,a.AccountCode,a.AccountName
HAVING SUM(e.Amount)<>0 OR SUM(CASE WHEN COALESCE(j.ValueDate,j.CreatedDate)<@Start THEN e.Amount ELSE 0 END)<>0 OR SUM(CASE WHEN COALESCE(j.ValueDate,j.CreatedDate)>=@Start AND j.TransactionCode=@Closing THEN e.Amount ELSE 0 END)<>0";
 }
}
