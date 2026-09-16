using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using Application.MainBoundedContext.DTO.AccountsModule;
using Domain.MainBoundedContext.AccountsModule.Aggregates.SasraAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ReportTemplateAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ReportTemplateEntryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
namespace Application.MainBoundedContext.AccountsModule.Services
{
    public partial class SasraSetupAppService : ISasraSetupAppService
    {
        readonly IDbContextScopeFactory scopes;
        readonly IRepository<SasraInstitutionProfile> profiles;
        readonly IRepository<SasraTemplateVersion> versions;
        readonly IRepository<SasraLineDefinition> lines;
        readonly IRepository<ReportTemplate> templates;
        readonly IRepository<ReportTemplateEntry> entries;
        readonly IRepository<ChartOfAccount> accounts;
        public SasraSetupAppService(IDbContextScopeFactory scopes,IRepository<SasraInstitutionProfile> profiles,IRepository<SasraTemplateVersion> versions,IRepository<SasraLineDefinition> lines,IRepository<ReportTemplate> templates,IRepository<ReportTemplateEntry> entries,IRepository<ChartOfAccount> accounts)
        { this.scopes=scopes;this.profiles=profiles;this.versions=versions;this.lines=lines;this.templates=templates;this.entries=entries;this.accounts=accounts; }
        static void Check(bool ok,string field,string message,int status=400){if(!ok)throw new SasraSetupException(field,message,status);}
        static string Required(string value,string field,int max){Check(!string.IsNullOrWhiteSpace(value)&&value.Trim().Length<=max,field,"Enter "+field+" (up to "+max+" characters).");return value.Trim();}
        static void Profile(string value,bool optional=false){Check(value=="DT"||value=="NWDT"||(optional&&value=="NotApplicable"),"Profile","Select DT, NW-DT or Not applicable.");}
        SasraInstitutionProfile Current(ServiceHeader h){return profiles.AllMatching(new DirectSpecification<SasraInstitutionProfile>(x=>x.SingletonKey==1),h).SingleOrDefault();}
        static SasraProfileDTO ProfileDto(SasraInstitutionProfile p){return p==null?new SasraProfileDTO():new SasraProfileDTO{Profile=p.Profile,InstitutionName=p.InstitutionName,RegistrationNumber=p.RegistrationNumber,Revision=p.Revision};}
        public List<SasraVersionDTO> GetStandardDefinitions(string profile)
        { Profile(profile);return SasraStandardDefinitions.ForProfile(profile); }
        public int AddStandardDefinitions(ServiceHeader h)
        {
            // Outer transaction keeps catalogue initialization all-or-nothing; existing revisions are never replaced.
            using(var scope=scopes.CreateWithTransaction(IsolationLevel.Serializable))
            {
                var p=Current(h);Check(p!=null&&(p.Profile=="DT"||p.Profile=="NWDT"),"Profile","Save your institution's DT or NW-DT reporting profile first.");
                var existing=versions.AllMatching(new DirectSpecification<SasraTemplateVersion>(v=>v.Profile==p.Profile),h).Select(v=>v.ReportCode).ToList();
                int added=0;
                foreach(var definition in SasraStandardDefinitions.ForProfile(p.Profile))
                {
                    if(existing.Any(code=>string.Equals(code,definition.ReportCode,StringComparison.OrdinalIgnoreCase)))continue;
                    ValidateVersion(definition);SaveVersionWithinScope(definition,h);added++;
                }
                scope.SaveChanges(h);return added;
            }
        }
        public SasraProfileDTO GetProfile(ServiceHeader h){using(scopes.CreateReadOnly())return ProfileDto(Current(h));}
        public SasraProfileDTO SaveProfile(SasraProfileDTO input,ServiceHeader h)
        {
            Check(input!=null,"Profile","Provide the institution settings."); Profile(input.Profile,true);
            var name=Required(input.InstitutionName,"InstitutionName",256);
            Check((input.RegistrationNumber??"").Trim().Length<=80,"RegistrationNumber","Registration number cannot exceed 80 characters.");
            using(var scope=scopes.CreateWithTransaction(IsolationLevel.Serializable))
            {
                var p=Current(h);Check(input.Revision==(p==null?0:p.Revision),"Revision","Settings changed. Reload before saving.",409);
                if(p==null){p=new SasraInstitutionProfile{SingletonKey=1,CreatedDate=DateTime.Now};p.GenerateNewIdentity();profiles.Add(p,h);}
                p.Profile=input.Profile;p.InstitutionName=name;p.RegistrationNumber=input.RegistrationNumber?.Trim();p.Revision++;p.ModifiedBy=h.ApplicationUserName;p.ModifiedDate=DateTime.Now;
                scope.SaveChanges(h);return ProfileDto(p);
            }
        }
        SasraVersionDTO Dto(SasraTemplateVersion v,ServiceHeader h,bool detail)
        {
            var dto=new SasraVersionDTO{Id=v.Id,Profile=v.Profile,ReportCode=v.ReportCode,Version=v.Version,Revision=v.Revision,Title=(v.RootTemplate??templates.Get(v.RootTemplateId,h)).Description,SourceUrl=v.SourceUrl,WorkbookSha256=v.WorkbookSha256,EffectiveFrom=v.EffectiveFrom};
            if(detail)
            {
                var definitions=lines.AllMatching(new DirectSpecification<SasraLineDefinition>(x=>x.VersionId==v.Id),h,x=>x.ReportTemplate).OrderBy(x=>x.Position).ToList();
                var ids=definitions.Select(x=>x.ReportTemplateId).ToArray();
                var mappings=entries.AllMatching(new DirectSpecification<ReportTemplateEntry>(x=>ids.Contains(x.ReportTemplateId)),h,x=>x.ChartOfAccount).ToLookup(x=>x.ReportTemplateId);
                dto.Lines=definitions.Select(l=>new SasraLineDTO{Code=l.Code,Description=l.ReportTemplate.Description,Source=l.Source,Sheet=l.Sheet,Cell=l.ReportTemplate.SpreadsheetCellReference,Sign=l.Sign,AccountIds=mappings[l.ReportTemplateId].Select(x=>x.ChartOfAccountId).ToList(),AccountNames=mappings[l.ReportTemplateId].ToDictionary(x=>x.ChartOfAccountId,x=>x.ChartOfAccount.AccountCode+" — "+x.ChartOfAccount.AccountName)}).ToList();
            }
            return dto;
        }
        public SasraVersionPageDTO GetVersions(int pageIndex,int pageSize,ServiceHeader h)
        {
            Check(pageIndex>=0&&pageIndex<=100000&&pageSize>=1&&pageSize<=100,"Page","Choose a valid page and page size (1-100).");
            using(scopes.CreateReadOnly())
            {
                var page=versions.AllMatchingPaged(new DirectSpecification<SasraTemplateVersion>(x=>true),pageIndex,pageSize,new List<string>{"CreatedDate","Id"},false,h,x=>x.RootTemplate);
                return new SasraVersionPageDTO{Items=page.PageCollection.Select(v=>Dto(v,h,false)).ToList(),Total=page.ItemsCount};
            }
        }
        public SasraVersionDTO GetVersion(Guid id,ServiceHeader h)
        {using(scopes.CreateReadOnly()){var v=versions.Get(id,h);Check(v!=null,"Id","Template revision was not found.",404);return Dto(v,h,true);}}
        public static void ValidateVersion(SasraVersionDTO input)
        {
            Check(input!=null,"Template","Provide a template definition.");Profile(input.Profile);
            input.ReportCode=Required(input.ReportCode,"ReportCode",40).ToUpperInvariant();input.Version=Required(input.Version,"Version",40).ToUpperInvariant();input.Title=Required(input.Title,"Title",256);
            Check(!input.EffectiveFrom.HasValue||input.EffectiveFrom.Value.Year>=1753,"EffectiveFrom","Choose an effective date on or after 1753.");
            Uri source;Check(string.IsNullOrWhiteSpace(input.SourceUrl)||(input.SourceUrl.Length<=1000&&Uri.TryCreate(input.SourceUrl,UriKind.Absolute,out source)&&source.Scheme=="https"&&(source.Host=="sasra.go.ke"||source.Host.EndsWith(".sasra.go.ke",StringComparison.OrdinalIgnoreCase))),"SourceUrl","Use an HTTPS source URL on sasra.go.ke.");
            Check(string.IsNullOrWhiteSpace(input.WorkbookSha256)||Regex.IsMatch(input.WorkbookSha256,"\\A[0-9a-fA-F]{64}\\z"),"WorkbookSha256","A SHA-256 checksum must contain exactly 64 hexadecimal characters.");
            Check(input.Lines!=null&&input.Lines.Count>0&&input.Lines.Count<=500,"Lines","Add between 1 and 500 report lines.");
            if(input.Version==SasraForm6.Version) SasraForm6.Validate(input);
            if(input.Version==SasraForm7.Version) SasraForm7.Validate(input);
            if(input.Version==SasraForm1.Version) SasraForm1.Validate(input);
            if(input.Version==SasraForm2.Version) SasraForm2.Validate(input);
            if(input.Version==SasraForm3.Version) SasraForm3.Validate(input);
            if(input.Version==SasraForm5.Version) SasraForm5.Validate(input);
            var codes=new HashSet<string>(StringComparer.OrdinalIgnoreCase);var cells=new HashSet<string>(StringComparer.OrdinalIgnoreCase);var used=new HashSet<Guid>();
            for(int i=0;i<input.Lines.Count;i++)
            {
                var l=input.Lines[i];var f="Lines["+i+"]";Check(l!=null,f,"Remove the empty line.");l.Code=Required(l.Code,f+".Code",40);l.Description=Required(l.Description,f+".Description",256);
                Check(codes.Add(l.Code),f+".Code","Line codes must be unique.");Check((new[]{"Header","GlBalance","GlMovement"}.Contains(l.Source)||(input.Version==SasraForm6.Version && new[]{"Formula","CurrentSurplus"}.Contains(l.Source))||(input.Version==SasraForm7.Version&&l.Source=="Formula")||((input.Version==SasraForm1.Version||input.Version==SasraForm2.Version||input.Version==SasraForm5.Version)&&new[]{"Formula","Manual","Derived","Constant"}.Contains(l.Source))),f+".Source","Select Header, G/L closing balance or G/L movement.");
                Check(l.Sign==1||l.Sign==-1,f+".Sign","Select preserve sign or reverse sign.");Check(l.AccountIds!=null&&l.AccountIds.Count<=500,f+".AccountIds","A line may contain up to 500 account mappings.");
                if(l.Source=="Header"){Check(l.AccountIds.Count==0&&string.IsNullOrWhiteSpace(l.Cell)&&string.IsNullOrWhiteSpace(l.Sheet),f,"Headers cannot have account mappings or output cells.");continue;}
                l.Sheet=Required(l.Sheet,f+".Sheet",31);Check(!Regex.IsMatch(l.Sheet,@"[\[\]:*?/\\]"),f+".Sheet","Enter a valid worksheet name.");
                l.Cell=Required(l.Cell,f+".Cell",12).ToUpperInvariant();Check(Regex.IsMatch(l.Cell,@"\A[A-Z]{1,3}[1-9][0-9]{0,6}\z"),f+".Cell","Use an Excel cell address such as D10.");
                var letters=new string(l.Cell.TakeWhile(char.IsLetter).ToArray());var col=letters.Aggregate(0,(n,c)=>n*26+c-'A'+1);var row=int.Parse(l.Cell.Substring(letters.Length));Check(col<=16384&&row<=1048576,f+".Cell","Cell is outside the Excel worksheet limits.");
                Check(cells.Add(l.Sheet+"!"+l.Cell),f+".Cell","Two lines cannot write to the same worksheet cell.");
                foreach(var id in l.AccountIds)Check(id!=Guid.Empty&&used.Add(id),f+".AccountIds","Select each posting account only once in this draft. Derived totals will be configured separately.");
            }
        }
        public SasraVersionDTO SaveVersion(SasraVersionDTO input,ServiceHeader h)
        {
            ValidateVersion(input);
            using(var scope=scopes.CreateWithTransaction(IsolationLevel.Serializable))
            {
                var result=SaveVersionWithinScope(input,h);
                scope.SaveChanges(h);return result;
            }
        }
        private SasraVersionDTO SaveVersionWithinScope(SasraVersionDTO input,ServiceHeader h)
        {
                var profile=Current(h);Check(profile!=null&&profile.Profile==input.Profile,"Profile","Save matching institution reporting settings first.");
                var accountIds=input.Lines.SelectMany(x=>x.AccountIds).ToArray();
                Check(accountIds.Length<=2000,"AccountIds","A definition may contain up to 2000 account mappings.");
                if(accountIds.Length>0)
                {
                    var selected=accounts.AllMatching(new DirectSpecification<ChartOfAccount>(x=>accountIds.Contains(x.Id)),h);
                    if(input.Version==SasraForm6.Version)
                    {
                        foreach(var line in input.Lines.Where(x=>x.AccountIds.Count>0))
                        {
                            var row=int.Parse(line.Cell.Substring(1));
                            var expected=row<38?1000:row<54?2000:3000;
                            Check(selected.Where(x=>line.AccountIds.Contains(x.Id)).All(x=>x.AccountType==expected),"AccountIds",line.Description+": select accounts in the matching asset, liability or equity category. Income and expense accounts are included automatically in surplus.");
                        }
                    }
                    if(input.Version==SasraForm7.Version)
                        foreach(var line in input.Lines.Where(x=>x.AccountIds.Count>0))
                            Check(selected.Where(x=>line.AccountIds.Contains(x.Id)).All(x=>SasraForm7.AllowsAccountType(line.Cell,x.AccountType)),"AccountIds",line.Description+": select a matching income or expense posting account. Balance-sheet accounts cannot be mapped to Form 7.");
                    if(input.Version==SasraForm1.Version)
                        foreach(var line in input.Lines.Where(x=>x.AccountIds.Count>0))
                            Check(selected.Where(x=>line.AccountIds.Contains(x.Id)).All(x=>SasraForm1.AllowsAccountType(line.Cell,x.AccountType)),"AccountIds",line.Description+": select equity accounts for capital components or asset accounts for on-balance-sheet assets.");
                    if(input.Version==SasraForm5.Version)
                        Check(selected.All(x=>x.AccountType==1000),"AccountIds","Form 5 requires asset posting accounts.");
                    if(input.Version==SasraForm2.Version)
                        foreach(var line in input.Lines.Where(x=>x.AccountIds.Count>0))
                            Check(selected.Where(x=>line.AccountIds.Contains(x.Id)).All(x=>SasraForm2.AllowsAccountType(line.Cell,x.AccountType)),"AccountIds",line.Description+": select matching asset or liability posting accounts.");
                    if(input.Version==SasraForm3.Version)
                        foreach(var account in selected.Where(x=>x.AccountType!=(int)ChartOfAccountType.Liability))
                        {
                            var classification=Enum.GetName(typeof(ChartOfAccountType),account.AccountType)??("Unknown ("+account.AccountType+")");
                            Check(false,"AccountIds",account.AccountCode+" - "+account.AccountName+" is classified as "+classification+" in the G/L. Form 3 requires a Liability posting account for deposits or related accrued interest. Linking an account to a savings product does not change its G/L classification.");
                        }
                    var children=accounts.AllMatchingCount(new DirectSpecification<ChartOfAccount>(x=>x.ParentId.HasValue&&accountIds.Contains(x.ParentId.Value)),h);
                    Check(selected.Count==accountIds.Length&&children==0,"AccountIds","Select existing posting accounts without child accounts.");
                }
                var existing=versions.AllMatching(new DirectSpecification<SasraTemplateVersion>(x=>x.Profile==input.Profile&&x.ReportCode==input.ReportCode&&x.Version==input.Version),h).ToList();
                var revision=existing.Count==0?1:existing.Max(x=>x.Revision)+1;
                Check(input.Revision==revision-1,"Revision","This definition has a newer revision. Open it before saving.",409);
                var root=ReportTemplateFactory.CreateReportTemplate(null,input.Title,4096,null);root.MarkVersioned();root.Lock();templates.Add(root,h);
                var v=new SasraTemplateVersion{Profile=input.Profile,ReportCode=input.ReportCode,Version=input.Version,Revision=revision,SourceUrl=input.SourceUrl?.Trim(),WorkbookSha256=input.WorkbookSha256?.ToLowerInvariant(),EffectiveFrom=input.EffectiveFrom?.Date,RootTemplateId=root.Id,CreatedDate=DateTime.Now};v.GenerateNewIdentity();versions.Add(v,h);
                for(int i=0;i<input.Lines.Count;i++)
                {
                    var line=input.Lines[i];var t=ReportTemplateFactory.CreateReportTemplate(root.Id,line.Description,line.Source=="Header"?4096:4097,line.Cell);t.MarkVersioned();t.Lock();templates.Add(t,h);
                    var d=new SasraLineDefinition{VersionId=v.Id,ReportTemplateId=t.Id,Position=i,Code=line.Code,Source=line.Source,Sheet=line.Sheet,Sign=line.Sign,CreatedDate=DateTime.Now};d.GenerateNewIdentity();lines.Add(d,h);
                    foreach(var id in line.AccountIds)entries.Add(ReportTemplateEntryFactory.CreateReportTemplateEntry(t.Id,id),h);
                }
                input.Id=v.Id;input.Revision=revision;return input;
        }
    }
}
