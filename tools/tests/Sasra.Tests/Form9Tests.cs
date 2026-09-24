using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using Newtonsoft.Json;
using NPOI.XSSF.UserModel;
using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Domain.MainBoundedContext.AccountsModule.Aggregates.SasraAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
static class Form9Tests
{
    static int checks;
    static void Check(bool ok,string label){if(!ok)throw new Exception("Form 9: "+label);checks++;}
    static void Reject(Action action,int status=400){try{action();throw new Exception("Expected rejection");}catch(SasraSetupException e){Check(e.Status==status,"validation status");}}
    class Proxy:RealProxy{readonly Func<IMethodCallMessage,object> call;public Proxy(Type t,Func<IMethodCallMessage,object> f):base(t){call=f;}public override IMessage Invoke(IMessage msg){var c=(IMethodCallMessage)msg;try{return new ReturnMessage(call(c),null,0,c.LogicalCallContext,c);}catch(Exception e){return new ReturnMessage(e,c);}}}
    static T Stub<T>(Func<IMethodCallMessage,object> f){return (T)new Proxy(typeof(T),f).GetTransparentProxy();}
    static Form9PolicyDTO Policy(Guid product){return new Form9PolicyDTO{GrantBasis="BoardDecision",PopulationBasis="AtGrantOrPeriodEnd",OutstandingBasis="Principal",DepositBasis="AtPeriodEnd",SectionBBasis="AllOutstanding",SharedAllocation="Block",NilReturn="ReviewRequired",BosaProductIds=new List<Guid>{product},RulesConfirmed=true,Evidence="Officer confirmation"};}
    public static void Run()
    {
        Reject(()=>SasraForm9.ValidateMonth(new Form9Request{Month=DateTime.Today}));
        Reject(()=>SasraForm9.ValidateMonth(new Form9Request{Month=new DateTime(2020,2,2)}));
        SasraForm9.ValidateMonth(new Form9Request{Month=new DateTime(2020,2,1)});checks++;
        var appointment=new InsiderAppointmentDTO{StartsAt=new DateTime(2020,2,1),EndsAt=new DateTime(2020,2,29)};
        Check(!SasraForm9.Active(appointment,new DateTime(2020,1,31))&&SasraForm9.Active(appointment,new DateTime(2020,2,1))&&SasraForm9.Active(appointment,new DateTime(2020,2,29))&&!SasraForm9.Active(appointment,new DateTime(2020,3,1)),"inclusive tenure boundaries");
        appointment.IsVoided=true;Check(!SasraForm9.Active(appointment,new DateTime(2020,2,10)),"voided appointment excluded");
        var product=Guid.NewGuid();var policy=Policy(product);SasraForm9.ValidatePolicy(policy);checks++;
        foreach(var field in new[]{"GrantBasis","PopulationBasis","OutstandingBasis","DepositBasis","SectionBBasis","SharedAllocation","NilReturn"}){
            var invalid=Policy(product);typeof(Form9PolicyDTO).GetProperty(field).SetValue(invalid,"");Reject(()=>SasraForm9.ValidatePolicy(invalid));
        }
        var dup=Policy(product);dup.BosaProductIds.Add(product);Reject(()=>SasraForm9.ValidatePolicy(dup));
        var unconfirmed=Policy(product);unconfirmed.RulesConfirmed=false;Reject(()=>SasraForm9.ValidatePolicy(unconfirmed));
        foreach(int countA in new[]{0,1,5,8})foreach(int countB in new[]{0,1,5,9}){
            var r=new Form9Result{Id=Guid.NewGuid(),Month=new DateTime(2020,2,1),AsAt=new DateTime(2020,2,29),Policy=policy,InstitutionName="Test SACCO",RegistrationNumber="CS123",TemplateHash=SasraForm9.TemplateHash,SnapshotHash="test",CreatedBy="test"};
            for(int i=0;i<countA;i++)r.NewLoans.Add(Row(i));
            for(int i=0;i<countB;i++)r.OutstandingLoans.Add(Row(i));
            var bytes=SasraForm9.Export(r);var wb=new XSSFWorkbook(new MemoryStream(bytes));{
                int a=Math.Max(0,countA-5),b=Math.Max(0,countB-5);var s=wb.GetSheet("Insider return");
                Check(wb.GetSheet("Notes")!=null&&wb.GetSheet("Validation")!=null,"official notes and validation retained");
                Check(s.GetRow(0).GetCell(1).StringCellValue.Contains("WORKING DRAFT"),"draft explicit");
                Check(s.GetRow(2).GetCell(3).StringCellValue=="Test SACCO"&&s.GetRow(4).GetCell(3).DateCellValue==r.Month,"header identity and typed dates");
                Check(s.GetRow(16+a).GetCell(7).NumericCellValue==countA*100,"new loans cached total after expansion");
                Check(s.GetRow(26+a+b).GetCell(13).NumericCellValue==countB*50,"outstanding cached total after expansion");
                Check(s.GetRow(19+a).GetCell(2).StringCellValue.Contains("PERFORMANCE"),"lower section preserved");
                Check(s.GetRow(32+a+b).GetCell(2).StringCellValue=="AUTHORIZATION:","signature area shifted");
                if(countA>0)Check(s.GetRow(10+countA-1).GetCell(2).StringCellValue=="Person "+(countA-1),"all new rows exported");
                if(countB>0)Check(s.GetRow(20+a+countB-1).GetCell(13).NumericCellValue==50,"all outstanding rows exported");
            }
        }
        var rows=new List<SasraInsiderRecord>();int commits=0;Guid customer=Guid.NewGuid(),caseId=Guid.NewGuid();
        var profile=new SasraProfileDTO{Profile="DT",InstitutionName="Test",RegistrationNumber="CS1"};
        var loan=new Form9Source{LoanCaseId=caseId,CustomerId=customer,CaseNumber=1,Borrower="Person",MemberNumber="M1",Product="Normal",AmountApplied=100,ApprovedAmount=100,CreatedDate=new DateTime(2020,1,1),DisbursedDate=new DateTime(2020,2,5),TermMonths=12};
        var finance=new LoanAgeingResult{Loans=new List<LoanAgeingLoanResult>{new LoanAgeingLoanResult{LoanCaseId=caseId,OutstandingPrincipal=50,OutstandingInterest=10,RiskClassification="Performing"}}};
        var scope=Stub<IDbContextScope>(c=>{if(c.MethodName=="SaveChanges"){commits++;return 1;}return null;});
        var read=Stub<IDbContextReadOnlyScope>(c=>null);
        var scopes=Stub<IDbContextScopeFactory>(c=>c.MethodName.StartsWith("CreateReadOnly")?(object)read:scope);
        var repository=Stub<IRepository<SasraInsiderRecord>>(c=>{
            if(c.MethodName=="AllMatching")return rows.Where(((ISpecification<SasraInsiderRecord>)c.Args[0]).SatisfiedBy().Compile()).ToList();
            if(c.MethodName=="Add"){rows.Add((SasraInsiderRecord)c.Args[0]);return null;}
            if(c.MethodName=="DatabaseSqlQuery"){var sql=(string)c.Args[0];
                if(sql.StartsWith("SELECT Id CustomerId"))return new[]{new Form9Candidate{CustomerId=customer,Name="Person",MemberNumber="M1"}};
                if(sql.StartsWith("SELECT COUNT(*)"))return new[]{1};
                if(sql.StartsWith("SELECT Id,Description"))return new[]{new Form9Product{Id=product,Description="Deposits"}};
                if(sql.StartsWith("WITH candidates"))return new[]{new Form9Candidate{CustomerId=customer,Name="Person",Kind="Employee"}};
                if(sql.StartsWith("WITH products"))return new[]{500m};
                if(sql.Contains("SELECT l.Id LoanCaseId"))return new[]{loan};
            }throw new Exception("Unexpected call "+c.MethodName);
        });
        var auth=Stub<IAuthorizationAppService>(c=>{
            if(c.MethodName=="GetRolesForSystemPermissionType")return new[]{"Approver"};
            throw new Exception("Form 9 must not query legacy module grants: "+c.MethodName);
        });
        string[] navigationGrants=new[]{"Reporting"};
        var navigation=Stub<INavigationItemInRoleAppService>(c=>{
            if(c.MethodName!="GetRolesForNavigationItemCode"||(int)c.Args[0]!=26016)throw new Exception("Unexpected navigation grant lookup");
            return navigationGrants;
        });
        var setup=Stub<ISasraSetupAppService>(c=>profile);
        var age=Stub<ILoanAgeingAppService>(c=>{if(c.MethodName=="GetNoticeLoanReport")return finance;return new List<LoanPlanDTO>{new LoanPlanDTO{LoanCaseId=caseId,IsConfirmed=true,DisbursementDate=new DateTime(2020,2,5),Revision=1,Instalments=new List<LoanPlanInstalmentDTO>{new LoanPlanInstalmentDTO{DueDate=new DateTime(2020,3,5)}}}};});
        var svc=new SasraInsiderAppService(scopes,repository,setup,age,auth,navigation);var h=new ServiceHeader{ApplicationUserName="maker",ApplicationUserRoles=new List<string>{"Reporting"}};
        Reject(()=>svc.GetPolicy(new ServiceHeader{ApplicationUserName="denied",ApplicationUserRoles=new List<string>()}),403);
        Check(svc.GetPolicy(h)!=null,"current module grant permits reporting without a legacy grant");
        Check(svc.GetPolicy(new ServiceHeader{ApplicationUserName="maker",ApplicationUserRoles=new List<string>{"reporting"}})!=null,"role comparison is case insensitive");
        Reject(()=>svc.GetPolicy(null),403);
        Reject(()=>svc.GetPolicy(new ServiceHeader{ApplicationUserName="maker",ApplicationUserRoles=null}),403);
        var approver=new ServiceHeader{ApplicationUserName="approver",ApplicationUserRoles=new List<string>{"Approver"}};
        Check(svc.History("board",caseId,approver).Count==0,"loan approver retains board access");
        Reject(()=>svc.GetPolicy(approver),403);
        navigationGrants=null;
        Reject(()=>svc.GetPolicy(h),403);
        navigationGrants=new string[0];
        Reject(()=>svc.GetPolicy(h),403);
        navigationGrants=new[]{"Unrelated"};
        Reject(()=>svc.GetPolicy(h),403);
        navigationGrants=new[]{"Reporting"};
        var a1=svc.SaveAppointment(new InsiderAppointmentDTO{CustomerId=customer,Kind="Employee",Position="Officer",StartsAt=new DateTime(2019,1,1),Evidence="Appointment letter"},h);
        Check(a1.Revision==1&&rows.Count==1,"appointment persisted");
        Reject(()=>svc.SaveAppointment(new InsiderAppointmentDTO{CustomerId=customer,Kind="Employee",Position="Duplicate",StartsAt=new DateTime(2020,1,1),Evidence="Letter"},h));
        var stale=JsonConvert.DeserializeObject<InsiderAppointmentDTO>(JsonConvert.SerializeObject(a1));a1.Position="Senior officer";svc.SaveAppointment(a1,h);
        Check(svc.History("appointment",a1.Id,h).Count==2,"immutable appointment history");Reject(()=>svc.SaveAppointment(stale,h),409);
        var board=svc.SaveDecision(new InsiderDecisionDTO{LoanCaseId=caseId,Decision="Approved",DecisionDate=new DateTime(2020,2,3),MinuteReference="BOD/1",Evidence="Minutes",ApplicantAbsent=true,SecurityConfirmed=true,SecurityDescription="Unsecured"},h);
        board.ApplicantAbsent=false;Reject(()=>svc.SaveDecision(board,h));
        svc.SavePolicy(policy,h);
        var result=svc.Preview(new Form9Request{Month=new DateTime(2020,2,1)},h);
        Check(result.IsComplete&&result.NewLoans.Count==1&&result.OutstandingLoans.Count==1,"both sections fully populated");
        Check(result.TotalGranted==100&&result.KnownOutstanding==50&&result.NewLoans[0].BosaDeposits==500,"calculated figures");
        var saved=svc.SaveDraft(new Form9Request{Month=new DateTime(2020,2,1)},h);loan.Borrower="Changed";
        Check(svc.GetRun(saved.Id,h).NewLoans[0].Borrower=="Person","snapshot immutable after source changes");
        Check(saved.SnapshotHash.Length==64&&saved.Status=="Working draft","draft has content hash and no false approval");
        Check(svc.Export(saved.Id,h).WorkbookBase64.Length>0,"saved snapshot export");
        finance.Loans[0].OutstandingPrincipal=null;
        Check(!svc.Preview(new Form9Request{Month=new DateTime(2020,2,1)},h).IsComplete,"unknown balance is an exception not zero");
        Console.WriteLine("PASS: "+checks+" Form 9 checks");
    }
    static Form9Row Row(int i){return new Form9Row{LoanCaseId=Guid.NewGuid(),Borrower="Person "+i,MemberNumber="M"+i,Position="Employee",Product="Normal",Applied=120,Granted=100,Outstanding=50,DecisionDate=new DateTime(2020,2,3),BosaDeposits=500,Security="Unsecured",FirstDueDate=new DateTime(2020,3,3),TermMonths=12,Performance="Performing"};}
}
