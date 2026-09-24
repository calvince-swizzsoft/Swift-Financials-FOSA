using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Newtonsoft.Json;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.BackOfficeModule.Services;
using Domain.MainBoundedContext.AccountsModule.Aggregates.SasraAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class SasraInsiderAppService : ISasraInsiderAppService
    {
        readonly IDbContextScopeFactory scopes;
        readonly IRepository<SasraInsiderRecord> records;
        readonly ISasraSetupAppService setup;
        readonly ILoanAgeingAppService ageing;
        readonly IAuthorizationAppService authorization;
        readonly INavigationItemInRoleAppService navigation;
        static readonly Guid PolicyId=new Guid("09000000-0000-0000-0000-000000000009");
        public SasraInsiderAppService(IDbContextScopeFactory scopes,IRepository<SasraInsiderRecord> records,ISasraSetupAppService setup,ILoanAgeingAppService ageing,IAuthorizationAppService authorization,INavigationItemInRoleAppService navigation)
        {this.scopes=scopes;this.records=records;this.setup=setup;this.ageing=ageing;this.authorization=authorization;this.navigation=navigation??throw new ArgumentNullException(nameof(navigation));}
        static void Check(bool value,string field,string message,int status=400){if(!value)throw new SasraSetupException(field,message,status);}
        bool HasGrant(string[] grants,ServiceHeader h){return (h.ApplicationUserRoles??new List<string>()).Any(r=>(grants??new string[0]).Any(g=>string.Equals(g,r,StringComparison.OrdinalIgnoreCase)));}
        void Access(ServiceHeader h,bool board=false)
        {
            Check(h!=null&&!string.IsNullOrWhiteSpace(h.ApplicationUserName),"Permission","Sign in before using Form 9.",403);
            // Use the same navigation grants as Administration / Modules, not the legacy module catalogue.
            bool allowed=HasGrant(navigation.GetRolesForNavigationItemCode(26016,h),h);
            if(board)allowed=allowed||HasGrant(authorization.GetRolesForSystemPermissionType((int)SystemPermissionType.BackOfficeLoanApproval,h),h)||HasGrant(authorization.GetRolesForSystemPermissionType((int)SystemPermissionType.FrontOfficeLoanApproval,h),h);
            Check(allowed,"Permission",board?"SASRA reporting or loan approval permission is required.":"SASRA reporting permission is required.",403);
        }
        static string Text(string text,int max,string field,bool required=true)
        {text=(text??"").Trim();Check((!required||text.Length>0)&&text.Length<=max,field,"Enter "+field+" (up to "+max+" characters).");return text;}
        static void Page(int page,int size){Check(page>=0&&page<=100000&&size>=1&&size<=100,"Page","Use a valid page and page size between 1 and 100.");}
        List<T> Sql<T>(string sql,ServiceHeader h,params object[] args){return records.DatabaseSqlQuery<T>(sql,h,args).ToList();}
        List<SasraInsiderRecord> Latest(string kind,ServiceHeader h)
        {return records.AllMatching(new DirectSpecification<SasraInsiderRecord>(x=>x.Kind==kind),h).GroupBy(x=>x.SubjectId).Select(g=>g.OrderByDescending(x=>x.Revision).First()).ToList();}
        SasraInsiderRecord Current(string kind,Guid id,ServiceHeader h)
        {return records.AllMatching(new DirectSpecification<SasraInsiderRecord>(x=>x.Kind==kind&&x.SubjectId==id),h).OrderByDescending(x=>x.Revision).FirstOrDefault();}
        static T Read<T>(SasraInsiderRecord record) where T:new(){return record==null?new T():JsonConvert.DeserializeObject<T>(record.Payload);}
        void Append(string kind,Guid id,int expected,object payload,ServiceHeader h)
        {
            var old=Current(kind,id,h);
            Check((old==null?0:old.Revision)==expected,"Revision","This record changed. Reload before saving.",409);
            var entity=new SasraInsiderRecord{Kind=kind,SubjectId=id,Revision=expected+1,Payload=JsonConvert.SerializeObject(payload),CreatedBy=h.ApplicationUserName,CreatedDate=DateTime.UtcNow};
            entity.GenerateNewIdentity();records.Add(entity,h);
        }
        public List<Form9Revision> History(string kind,Guid id,ServiceHeader h)
        {
            Access(h,kind=="board");Check(new[]{"appointment","board","policy","run"}.Contains(kind),"Kind","Select a supported record type.");
            if(kind=="policy")id=PolicyId;
            using(scopes.CreateReadOnly())return records.AllMatching(new DirectSpecification<SasraInsiderRecord>(x=>x.Kind==kind&&x.SubjectId==id),h).OrderByDescending(x=>x.Revision).Select(x=>new Form9Revision{Revision=x.Revision,CreatedDate=x.CreatedDate,CreatedBy=x.CreatedBy,Payload=x.Payload}).ToList();
        }
        const string CustomerSql="SELECT Id CustomerId,LTRIM(RTRIM(COALESCE(Individual_FirstName,'')+' '+COALESCE(Individual_LastName,''))) Name,Reference2 MemberNumber FROM dbo.swiftFin_Customers WHERE Type=0 AND Id=@Id";
        public InsiderAppointmentDTO SaveAppointment(InsiderAppointmentDTO input,ServiceHeader h)
        {
            Access(h);Check(input!=null,"Appointment","Provide appointment details.");
            Check(input.Kind=="Director"||input.Kind=="Employee","Kind","Choose Director or Employee.");
            input.Position=Text(input.Position,150,"Position");input.Evidence=Text(input.Evidence,2000,"Evidence");
            Check(input.StartsAt.Year>=1900&&input.StartsAt.Date<=DateTime.Today,"StartsAt","Enter the actual appointment start date, not a future date.");
            Check(!input.EndsAt.HasValue||(input.EndsAt.Value.Date>=input.StartsAt.Date&&input.EndsAt.Value.Date<=DateTime.Today),"EndsAt","End date must be between start date and today.");
            using(var scope=scopes.CreateWithTransaction(IsolationLevel.Serializable))
            {
                var customer=Sql<Form9Candidate>(CustomerSql,h,new SqlParameter("@Id",input.CustomerId)).SingleOrDefault();
                Check(customer!=null,"CustomerId","Choose an existing individual customer.");
                if(input.Id==Guid.Empty){Check(input.Revision==0,"Revision","A new appointment must have revision zero.");input.Id=Guid.NewGuid();}
                var previous=Current("appointment",input.Id,h);
                if(previous!=null)Check(Read<InsiderAppointmentDTO>(previous).CustomerId==input.CustomerId,"CustomerId","An appointment cannot be moved to another customer.");
                var overlap=Latest("appointment",h).Select(Read<InsiderAppointmentDTO>).Any(a=>!a.IsVoided&&a.Id!=input.Id&&a.CustomerId==input.CustomerId&&a.Kind==input.Kind&&a.StartsAt.Date<=(input.EndsAt??DateTime.MaxValue).Date&&input.StartsAt.Date<=(a.EndsAt??DateTime.MaxValue).Date);
                Check(input.IsVoided||!overlap,"StartsAt","This customer has an overlapping appointment of the same kind. Correct its dates first.");
                input.StartsAt=input.StartsAt.Date;input.EndsAt=input.EndsAt?.Date;input.CustomerName=customer.Name;input.MemberNumber=customer.MemberNumber;
                int expected=input.Revision;input.Revision++;Append("appointment",input.Id,expected,input,h);scope.SaveChanges(h);return input;
            }
        }
        public Form9Page<InsiderAppointmentDTO> GetAppointments(string text,int page,int size,ServiceHeader h)
        {
            Access(h);Page(page,size);text=(text??"").Trim();
            using(scopes.CreateReadOnly()){
                var all=Latest("appointment",h).Select(Read<InsiderAppointmentDTO>).Where(a=>string.IsNullOrEmpty(text)||((a.CustomerName??"")+" "+a.MemberNumber+" "+a.Position).IndexOf(text,StringComparison.OrdinalIgnoreCase)>=0).OrderBy(a=>a.CustomerName).ThenBy(a=>a.Id).ToList();
                return new Form9Page<InsiderAppointmentDTO>{Total=all.Count,Items=all.Skip(page*size).Take(size).ToList()};
            }
        }
        const string CandidatesSql=@"WITH candidates AS (
SELECT d.CustomerId,'Director' Kind,'Director' Position,CAST(NULL AS datetime) SuggestedStart,d.IsLocked
FROM dbo.swiftFin_Directors d
UNION ALL
SELECT e.CustomerId,'Employee',p.Description,e.EmploymentStartDate,e.IsLocked FROM dbo.swiftFin_Employees e LEFT JOIN dbo.swiftFin_Designations p ON p.Id=e.DesignationId)
SELECT c.CustomerId,LTRIM(RTRIM(COALESCE(u.Individual_FirstName,'')+' '+COALESCE(u.Individual_LastName,''))) Name,u.Reference2 MemberNumber,c.Kind,c.Position,c.SuggestedStart,c.IsLocked
FROM candidates c JOIN dbo.swiftFin_Customers u ON u.Id=c.CustomerId WHERE (@Text='' OR u.Individual_FirstName LIKE @Like OR u.Individual_LastName LIKE @Like OR u.Reference2 LIKE @Like)";
        public Form9Page<Form9Candidate> GetCandidates(string text,int page,int size,ServiceHeader h)
        {
            Access(h);Page(page,size);
            using(scopes.CreateReadOnly()){
                var rows=Sql<Form9Candidate>(CandidatesSql,h,new SqlParameter("@Text",text??""),new SqlParameter("@Like","%"+(text??"")+"%")).GroupBy(x=>new{x.CustomerId,x.Kind}).Select(g=>g.First()).OrderBy(x=>x.Name).ThenBy(x=>x.CustomerId).ToList();
                var appointments=Latest("appointment",h).Select(Read<InsiderAppointmentDTO>).Where(a=>!a.IsVoided).ToList();
                foreach(var row in rows)row.HasHistory=appointments.Any(a=>a.CustomerId==row.CustomerId&&a.Kind==row.Kind);
                return new Form9Page<Form9Candidate>{Total=rows.Count,Items=rows.Skip(page*size).Take(size).ToList()};
            }
        }
        const string LoansSql=@"SELECT l.Id LoanCaseId,l.CustomerId,l.CaseNumber,l.AmountApplied,l.ApprovedAmount,l.ApprovedDate,l.DisbursedDate,l.CreatedDate,CAST(l.Status AS int) Status,CAST(l.LoanRegistration_TermInMonths AS int) TermMonths,
LTRIM(RTRIM(COALESCE(c.Individual_FirstName,'')+' '+COALESCE(c.Individual_LastName,''))) Borrower,c.Reference2 MemberNumber,p.Description Product
FROM dbo.swiftFin_LoanCases l JOIN dbo.swiftFin_Customers c ON c.Id=l.CustomerId JOIN dbo.swiftFin_LoanProducts p ON p.Id=l.LoanProductId ";
        public Form9Page<Form9Source> GetLoans(string text,int page,int size,ServiceHeader h)
        {
            Access(h,true);Page(page,size);
            using(scopes.CreateReadOnly()){
                string where=" WHERE (@Text='' OR c.Individual_FirstName LIKE @Like OR c.Individual_LastName LIKE @Like OR c.Reference2 LIKE @Like OR CONVERT(varchar(20),l.CaseNumber)=@Text)";
                var count=Sql<int>("SELECT COUNT(*) FROM dbo.swiftFin_LoanCases l JOIN dbo.swiftFin_Customers c ON c.Id=l.CustomerId "+where,h,new SqlParameter("@Text",text??""),new SqlParameter("@Like","%"+(text??"")+"%")).Single();
                var rows=Sql<Form9Source>(LoansSql+where+" ORDER BY l.CaseNumber DESC,l.Id OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY",h,new SqlParameter("@Text",text??""),new SqlParameter("@Like","%"+(text??"")+"%"),new SqlParameter("@Skip",page*size),new SqlParameter("@Take",size));
                return new Form9Page<Form9Source>{Total=count,Items=rows};
            }
        }
        public InsiderDecisionDTO GetDecision(Guid id,ServiceHeader h)
        {Access(h,true);using(scopes.CreateReadOnly()){var d=Read<InsiderDecisionDTO>(Current("board",id,h));d.LoanCaseId=id;return d;}}
        public InsiderDecisionDTO SaveDecision(InsiderDecisionDTO input,ServiceHeader h)
        {
            Access(h,true);Check(input!=null,"Decision","Provide board decision details.");
            Check(input.Decision=="Approved"||input.Decision=="Ratified","Decision","Select Approved or Ratified.");
            Check(input.DecisionDate.Year>=1900&&input.DecisionDate.Date<=DateTime.Today,"DecisionDate","Enter the actual board decision date.");
            input.MinuteReference=Text(input.MinuteReference,200,"MinuteReference");input.Evidence=Text(input.Evidence,2000,"Evidence");
            Check(input.ApplicantAbsent,"ApplicantAbsent","Confirm the applicant was absent from the board decision.");
            Check(input.SecurityConfirmed,"SecurityConfirmed","Review the nature of security, including an explicit unsecured description where applicable.");
            input.SecurityDescription=Text(input.SecurityDescription,1000,"SecurityDescription");input.Remarks=Text(input.Remarks,1000,"Remarks",false);
            using(var scope=scopes.CreateWithTransaction(IsolationLevel.Serializable)){
                Check(Sql<int>("SELECT COUNT(*) FROM dbo.swiftFin_LoanCases WHERE Id=@Id",h,new SqlParameter("@Id",input.LoanCaseId)).Single()==1,"LoanCaseId","Loan case not found.",404);
                int expected=input.Revision;input.Revision++;input.DecisionDate=input.DecisionDate.Date;Append("board",input.LoanCaseId,expected,input,h);scope.SaveChanges(h);return input;
            }
        }
        public Form9PolicyDTO GetPolicy(ServiceHeader h){Access(h);using(scopes.CreateReadOnly())return Read<Form9PolicyDTO>(Current("policy",PolicyId,h));}
        public List<Form9Product> GetProducts(ServiceHeader h)
        {
            Access(h);using(scopes.CreateReadOnly())return Sql<Form9Product>(@"SELECT Id,Description,'Savings' Kind,IsLocked FROM dbo.swiftFin_SavingsProducts
UNION ALL SELECT Id,Description,'Investment' Kind,IsLocked FROM dbo.swiftFin_InvestmentProducts ORDER BY Description",h);
        }
        public Form9PolicyDTO SavePolicy(Form9PolicyDTO input,ServiceHeader h)
        {
            Access(h);SasraForm9.ValidatePolicy(input);input.Evidence=Text(input.Evidence,2000,"Evidence");
            using(var scope=scopes.CreateWithTransaction(IsolationLevel.Serializable)){
                var products=GetProducts(h);
                Check(input.BosaProductIds.All(id=>products.Any(p=>p.Id==id&&!p.IsLocked)),"BosaProductIds","Select available unlocked BOSA products.");
                int expected=input.Revision;input.Revision++;Append("policy",PolicyId,expected,input,h);scope.SaveChanges(h);return input;
            }
        }
        public Form9Result Preview(Form9Request input,ServiceHeader h)
        {Access(h);using(scopes.CreateReadOnlyWithTransaction(IsolationLevel.Serializable))return Build(input,h);}
        public Form9Result SaveDraft(Form9Request input,ServiceHeader h)
        {
            Access(h);using(var scope=scopes.CreateWithTransaction(IsolationLevel.Serializable)){
                var result=Build(input,h);result.Id=Guid.NewGuid();result.SnapshotHash=null;result.SnapshotHash=SasraForm9.Hash(JsonConvert.SerializeObject(result));
                Append("run",result.Id,0,result,h);scope.SaveChanges(h);return result;
            }
        }
        Form9Result Build(Form9Request input,ServiceHeader h)
        {
            SasraForm9.ValidateMonth(input);
            var profile=setup.GetProfile(h);Check(profile.Profile=="DT","Profile","Save the DT institution profile before generating Form 9.");
            var policy=Read<Form9PolicyDTO>(Current("policy",PolicyId,h));
            var result=new Form9Result{Month=input.Month.Date,AsAt=input.Month.AddMonths(1).AddDays(-1).Date,CreatedAt=DateTime.UtcNow,CreatedBy=h.ApplicationUserName,InstitutionName=profile.InstitutionName,RegistrationNumber=profile.RegistrationNumber,Policy=policy,TemplateHash=SasraForm9.TemplateHash};
            if(string.IsNullOrWhiteSpace(profile.InstitutionName)||string.IsNullOrWhiteSpace(profile.RegistrationNumber))result.Issues.Add("Institution name and CS number must be configured.");
            try{SasraForm9.ValidatePolicy(policy);var available=GetProducts(h);Check(policy.BosaProductIds.All(id=>available.Any(p=>p.Id==id&&!p.IsLocked)),"BosaProductIds","A mapped BOSA product is missing or locked. Review reporting setup.");}catch(SasraSetupException e){result.Issues.Add(e.Message);return result;}
            var appointments=Latest("appointment",h).Select(Read<InsiderAppointmentDTO>).Where(a=>!a.IsVoided).ToList();
            var decisionRecords=Latest("board",h);var decisions=decisionRecords.Select(Read<InsiderDecisionDTO>).ToDictionary(d=>d.LoanCaseId);
            var candidates=Sql<Form9Candidate>(CandidatesSql,h,new SqlParameter("@Text",""),new SqlParameter("@Like","%"));
            foreach(var c in candidates.Where(c=>!appointments.Any(a=>a.CustomerId==c.CustomerId&&a.Kind==c.Kind)).GroupBy(c=>new{c.CustomerId,c.Kind}).Select(g=>g.First()))
                result.Issues.Add(c.Name+" ("+c.Kind+"): appointment history has not been captured, including any cessation date.");
            var loans=Sql<Form9Source>(LoansSql+" WHERE l.CreatedDate<@End",h,new SqlParameter("@End",result.AsAt.AddDays(1)));
            var age=ageing.GetNoticeLoanReport(result.AsAt,h);var ageLookup=age.Loans.ToDictionary(x=>x.LoanCaseId);
            if(Math.Abs(age.Difference)>.01m||Math.Abs(age.InterestDifference)>.01m)result.Issues.Add("Loan portfolio and ledger balances do not reconcile. Resolve loan-ageing differences.");
            var planEvidence=new List<LoanPlanDTO>();
            foreach(var loan in loans)
            {
                InsiderDecisionDTO decision;decisions.TryGetValue(loan.LoanCaseId,out decision);
                DateTime? grant=policy.GrantBasis=="BoardDecision"?decision?.DecisionDate:loan.DisbursedDate;
                var membershipDate=policy.PopulationBasis=="AtGrant"?grant:(DateTime?)result.AsAt;
                var known=appointments.Where(a=>a.CustomerId==loan.CustomerId).ToList();
                bool atEnd=known.Any(a=>SasraForm9.Active(a,result.AsAt));
                bool atGrant=grant.HasValue&&known.Any(a=>SasraForm9.Active(a,grant.Value));
                bool include=policy.PopulationBasis=="AtGrant"?atGrant:policy.PopulationBasis=="AtPeriodEnd"?atEnd:atGrant||atEnd;
                if(!grant.HasValue&&known.Count>0&&(loan.ApprovedAmount>0||loan.DisbursedDate.HasValue)){result.Issues.Add("Loan "+loan.CaseNumber+": grant event is missing; insider inclusion cannot be resolved.");continue;}
                if(!include||!grant.HasValue)continue;
                bool sectionA=grant.Value.Date>=result.Month&&grant.Value.Date<=result.AsAt;
                LoanAgeingLoanResult financial;ageLookup.TryGetValue(loan.LoanCaseId,out financial);
                bool posted=loan.DisbursedDate.HasValue&&loan.DisbursedDate.Value.Date<=result.AsAt;
                decimal? outstanding=financial==null?null:policy.OutstandingBasis=="Principal"?financial.OutstandingPrincipal:financial.TotalOutstanding;
                bool sectionB=posted&&(!outstanding.HasValue||outstanding.Value>0)&&(policy.SectionBBasis=="AllOutstanding"||grant.Value.Date<result.Month);
                if(!sectionA&&!sectionB)continue;
                var relevant=known.Where(a=>SasraForm9.Active(a,membershipDate??result.AsAt)||(policy.PopulationBasis=="AtGrantOrPeriodEnd"&&grant.HasValue&&SasraForm9.Active(a,grant.Value))).ToList();
                var row=new Form9Row{LoanCaseId=loan.LoanCaseId,CustomerId=loan.CustomerId,CaseNumber=loan.CaseNumber,Borrower=loan.Borrower,MemberNumber=loan.MemberNumber,Position=string.Join("; ",relevant.Select(a=>a.Position).Distinct()),Product=loan.Product,Applied=loan.AmountApplied,Granted=loan.ApprovedAmount,DecisionDate=decision?.DecisionDate,Security=decision?.SecurityDescription,TermMonths=loan.TermMonths,Remarks=decision?.Remarks,Outstanding=outstanding,Performance=financial?.RiskClassification};
                if(string.IsNullOrWhiteSpace(row.MemberNumber))row.Issues.Add("Membership number is missing.");
                if(string.IsNullOrWhiteSpace(row.Position))row.Issues.Add("Position history does not resolve for the selected policy.");
                if(decision==null)row.Issues.Add("Board approval or ratification evidence is missing.");
                else if(decision.DecisionDate.Date>result.AsAt)row.Issues.Add("Board decision is after the reporting period.");
                var history=ageing.GetHistory(loan.LoanCaseId,h);
                var plan=history.Where(p=>p.IsConfirmed&&(p.EffectiveAt??p.DisbursementDate).Date<=result.AsAt).OrderByDescending(p=>p.Revision).FirstOrDefault();
                if(plan==null||plan.Instalments.Count==0)row.Issues.Add("A confirmed repayment schedule is missing.");
                else{row.FirstDueDate=plan.Instalments.Min(x=>x.DueDate);planEvidence.Add(plan);}
                if(plan!=null&&plan.IsRestructuring)row.Issues.Add("Restructured loan: confirm case-level term and commencement before submission.");
                if(sectionB){
                    if(!outstanding.HasValue)row.Issues.Add("Loan outstanding amount could not be calculated.");
                    if(outstanding<0)row.Issues.Add("Credit loan balance needs review.");
                    if(financial!=null)foreach(var issue in financial.Issues){
                        if(issue.StartsWith("Repayments shared")&&policy.SharedAllocation=="OldestDueFirst"){result.Warnings.Add("Loan "+loan.CaseNumber+": shared repayments allocated oldest due first.");continue;}
                        row.Issues.Add(issue);
                    }
                    if(!new[]{"Performing","Watch","Substandard","Doubtful","Loss"}.Contains(row.Performance))row.Issues.Add("Loan performance classification is unresolved.");
                }
                var depositDate=policy.DepositBasis=="AtPeriodEnd"?(DateTime?)result.AsAt:decision?.DecisionDate;
                if(!depositDate.HasValue)row.Issues.Add("BOSA valuation date cannot be resolved.");
                else row.BosaDeposits=Deposits(loan.CustomerId,depositDate.Value,policy.BosaProductIds,h);
                if(row.BosaDeposits<0)row.Issues.Add("BOSA deposit balance is negative; review the mapped accounts.");
                if(sectionA)result.NewLoans.Add(row);if(sectionB)result.OutstandingLoans.Add(row);
            }
            var duplicateMembers=loans.Where(l=>!string.IsNullOrWhiteSpace(l.MemberNumber)).GroupBy(l=>l.MemberNumber.Trim(),StringComparer.OrdinalIgnoreCase).Where(g=>g.Select(l=>l.CustomerId).Distinct().Count()>1).Select(g=>g.Key).ToHashSetCompat();
            foreach(var row in result.NewLoans.Concat(result.OutstandingLoans).Distinct())if(duplicateMembers.Contains(row.MemberNumber??""))row.Issues.Add("Membership number is shared by different customers.");
            if(result.NewLoans.Count==0&&result.OutstandingLoans.Count==0&&policy.NilReturn=="ReviewRequired")result.Issues.Add("No reportable loans: confirm the nil return treatment before submission.");
            result.TotalGranted=result.NewLoans.Sum(r=>r.Granted);result.KnownOutstanding=result.OutstandingLoans.Sum(r=>r.Outstanding??0);
            result.IsComplete=result.Issues.Count==0&&result.NewLoans.Concat(result.OutstandingLoans).All(r=>r.Issues.Count==0);
            result.SourceEvidenceJson=JsonConvert.SerializeObject(new{Appointments=appointments,Decisions=decisionRecords.Select(r=>new{r.Id,r.SubjectId,r.Revision,r.Payload,r.CreatedBy,r.CreatedDate}),Plans=planEvidence,LoanLedgerDifference=age.Difference,InterestLedgerDifference=age.InterestDifference});
            result.Warnings=result.Warnings.Distinct().ToList();return result;
        }
        decimal Deposits(Guid customer,DateTime date,List<Guid> products,ServiceHeader h)
        {
            // Product identifiers are server-validated GUIDs; parameters carry all values.
            var parameters=new List<object>{new SqlParameter("@Customer",customer),new SqlParameter("@End",date.Date.AddDays(1))};
            var names=new List<string>();for(int i=0;i<products.Count;i++){names.Add("@P"+i);parameters.Add(new SqlParameter("@P"+i,products[i]));}
            string sql=@"WITH products AS (SELECT Id,ChartOfAccountId FROM dbo.swiftFin_SavingsProducts UNION ALL SELECT Id,ChartOfAccountId FROM dbo.swiftFin_InvestmentProducts)
SELECT COALESCE(-SUM(e.Amount),0) FROM dbo.swiftFin_JournalEntries e JOIN dbo.swiftFin_Journals j ON j.Id=e.JournalId
JOIN dbo.swiftFin_CustomerAccounts a ON a.Id=e.CustomerAccountId JOIN products p ON p.Id=a.CustomerAccountType_TargetProductId AND p.ChartOfAccountId=e.ChartOfAccountId
WHERE a.CustomerId=@Customer AND COALESCE(j.ValueDate,j.CreatedDate)<@End AND p.Id IN ("+string.Join(",",names)+")";
            return Sql<decimal>(sql,h,parameters.ToArray()).Single();
        }
        public Form9Page<Form9Result> GetRuns(int page,int size,ServiceHeader h)
        {
            Access(h);Page(page,size);using(scopes.CreateReadOnly()){
                var all=Latest("run",h).OrderByDescending(x=>x.CreatedDate).ToList();
                var rows=all.Skip(page*size).Take(size).Select(Read<Form9Result>).ToList();
                foreach(var row in rows){row.NewLoans.Clear();row.OutstandingLoans.Clear();row.SourceEvidenceJson=null;}
                return new Form9Page<Form9Result>{Total=all.Count,Items=rows};
            }
        }
        public Form9Result GetRun(Guid id,ServiceHeader h){Access(h);using(scopes.CreateReadOnly()){var record=Current("run",id,h);Check(record!=null,"Id","Saved draft not found.",404);return Read<Form9Result>(record);}}
        public Form9Export Export(Guid id,ServiceHeader h)
        {
            var result=GetRun(id,h);var hash=result.SnapshotHash;result.SnapshotHash=null;Check(hash==SasraForm9.Hash(JsonConvert.SerializeObject(result)),"Snapshot","The saved report integrity check failed.",409);result.SnapshotHash=hash;var bytes=SasraForm9.Export(result);
            return new Form9Export{FileName="SASRA-Form9-working-draft-"+result.AsAt.ToString("yyyy-MM-dd")+".xlsx",WorkbookBase64=Convert.ToBase64String(bytes),Sha256=SasraForm9.Hash(bytes)};
        }
    }
    internal static class Form9SetExtensions {public static HashSet<string> ToHashSetCompat(this IEnumerable<string> values){return new HashSet<string>(values,StringComparer.OrdinalIgnoreCase);}}
}
