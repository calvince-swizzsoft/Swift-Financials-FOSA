using System;
using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
static class LoanDisbursementDateTests
{
    static void Reject(Action action) { try { action(); } catch (LoanDisbursementDateException) { return; } throw new Exception("Invalid disbursement date was accepted"); }
    class Proxy : System.Runtime.Remoting.Proxies.RealProxy
    {
        readonly Func<System.Runtime.Remoting.Messaging.IMethodCallMessage,object> call;
        public Proxy(Type t, Func<System.Runtime.Remoting.Messaging.IMethodCallMessage,object> f):base(t){call=f;}
        public override System.Runtime.Remoting.Messaging.IMessage Invoke(System.Runtime.Remoting.Messaging.IMessage message)
        {var c=(System.Runtime.Remoting.Messaging.IMethodCallMessage)message;try{return new System.Runtime.Remoting.Messaging.ReturnMessage(call(c),null,0,c.LogicalCallContext,c);}catch(Exception e){return new System.Runtime.Remoting.Messaging.ReturnMessage(e,c);}}
    }
    static void CheckService(DateTime date, PostingPeriodDTO period)
    {
        var loan=new Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanCaseAgg.LoanCase{ReceivedDate=date,ApprovedDate=date.AddDays(1),Status=(byte)0};
        loan.Status=(int)Infrastructure.Crosscutting.Framework.Utils.LoanCaseStatus.Audited;
        var batch=new Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanDisbursementBatchAgg.LoanDisbursementBatch{EffectiveDisbursementDate=date,Status=(byte)Infrastructure.Crosscutting.Framework.Utils.BatchStatus.Posted};
        var entry=new Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanDisbursementBatchEntryAgg.LoanDisbursementBatchEntry();
        int writes=0, commits=0;
        batch.GenerateNewIdentity();
        batch.CreatedBy="editor";
        var editScope=new Proxy(typeof(Numero3.EntityFramework.Interfaces.IDbContextScope),c=>{if(c.MethodName=="SaveChanges"){commits++;return 1;}return null;}).GetTransparentProxy();
        var scope=new Proxy(typeof(Numero3.EntityFramework.Interfaces.IDbContextReadOnlyScope),c=>null).GetTransparentProxy();
        var constructor=typeof(LoanDisbursementBatchAppService).GetConstructors()[0];
        var args=System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Select(constructor.GetParameters(),p=>new Proxy(p.ParameterType,c=>{
            if(p.Name=="dbContextScopeFactory" && c.MethodName=="CreateReadOnly")return scope;
            if(p.Name=="dbContextScopeFactory" && c.MethodName=="CreateWithTransaction"){
                if((System.Data.IsolationLevel)c.Args[0]!=System.Data.IsolationLevel.Serializable)throw new Exception("Date edits require serialized status checks");
                return editScope;
            }
            if(p.Name=="loanDisbursementBatchRepository" && c.MethodName=="DatabaseSqlQuery")return new[]{batch.Id};
            if(p.Name=="loanDisbursementBatchEntryRepository" && c.MethodName=="AllMatching")return new System.Collections.Generic.List<Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanDisbursementBatchEntryAgg.LoanDisbursementBatchEntry>{entry};
            if(p.Name=="postingPeriodAppService" && c.MethodName=="FindPostingPeriods")return new System.Collections.Generic.List<PostingPeriodDTO>{period};
            if(c.MethodName=="Get"){
                if(p.Name=="loanCaseRepository")return loan;
                if(p.Name=="loanDisbursementBatchRepository")return batch;
                if(p.Name=="loanDisbursementBatchEntryRepository")return entry;
            }
            if(p.Name=="loanCaseRepository" && c.MethodName=="DatabaseSqlQuery")return new string[0];
            writes++;throw new Exception("Unexpected mutation or dependency: "+p.Name+"."+c.MethodName);
        }).GetTransparentProxy()));
        var service=(LoanDisbursementBatchAppService)constructor.Invoke(args);
        Reject(()=>service.PostLoanDisbursementBatchEntry(Guid.NewGuid(),0,new Infrastructure.Crosscutting.Framework.Utils.ServiceHeader()));
        if(writes!=0 || entry.Status!=0 || loan.DisbursedDate.HasValue)throw new Exception("Date validation must precede posting mutations");
        var header=new Infrastructure.Crosscutting.Framework.Utils.ServiceHeader{ApplicationUserName="editor"};
        var dto=new Application.MainBoundedContext.DTO.BackOfficeModule.LoanDisbursementBatchDTO{Id=batch.Id,EffectiveDisbursementDate=date.AddDays(2),Reference="unchanged",Priority=3};
        foreach(var status in new[]{Infrastructure.Crosscutting.Framework.Utils.BatchStatus.Audited,Infrastructure.Crosscutting.Framework.Utils.BatchStatus.Posted,Infrastructure.Crosscutting.Framework.Utils.BatchStatus.Rejected}){
            batch.Status=(byte)status;
            Reject(()=>service.UpdateLoanDisbursementBatch(dto,header));
            if(commits!=0 || batch.EffectiveDisbursementDate!=date)throw new Exception("Locked batch date changed");
        }
        batch.Status=(byte)Infrastructure.Crosscutting.Framework.Utils.BatchStatus.Pending;
        Reject(()=>service.UpdateLoanDisbursementBatch(dto,new Infrastructure.Crosscutting.Framework.Utils.ServiceHeader{ApplicationUserName="other"}));
        dto.EffectiveDisbursementDate=date.AddDays(-1);
        Reject(()=>service.UpdateLoanDisbursementBatch(dto,header));
        if(commits!=0 || batch.EffectiveDisbursementDate!=date)throw new Exception("Rejected date edit mutated the batch");
        dto.EffectiveDisbursementDate=date.AddDays(2);
        if(!service.UpdateLoanDisbursementBatch(dto,header) || commits!=1 || batch.EffectiveDisbursementDate!=dto.EffectiveDisbursementDate)throw new Exception("Valid pending date edit failed");
        Console.WriteLine("PASS: Pending date edit; attached-loan revalidation; creator check; verified, posted and rejected batches locked.");
    }
    public static void Run()
    {
        var date=new DateTime(2026,8,25);var today=new DateTime(2026,9,25);
        var period=new PostingPeriodDTO{Id=Guid.NewGuid(),DurationStartDate=new DateTime(2026,1,1),DurationEndDate=new DateTime(2026,12,31)};
        if(LoanDisbursementDates.Period(date,today,new[]{period}).Id!=period.Id)throw new Exception("Wrong period");
        Reject(()=>LoanDisbursementDates.Period(today.AddDays(1),today,new[]{period}));
        Reject(()=>LoanDisbursementDates.Period(date,today,new PostingPeriodDTO[0]));
        Reject(()=>LoanDisbursementDates.Period(date,today,new[]{period,period}));
        period.IsClosed=true;Reject(()=>LoanDisbursementDates.Period(date,today,new[]{period}));period.IsClosed=false;
        period.IsLocked=true;Reject(()=>LoanDisbursementDates.Period(date,today,new[]{period}));period.IsLocked=false;
        LoanDisbursementDates.Loan(date,date,date);
        Reject(()=>LoanDisbursementDates.Loan(date,date.AddDays(1),date));
        Reject(()=>LoanDisbursementDates.Loan(date,date,date.AddDays(1)));
        Reject(()=>LoanDisbursementDates.Loan(date,date,null));
        if(LoanScheduleGeneration.DueDate(date,0,12,0,0)!=new DateTime(2026,9,25))throw new Exception("End-period first instalment must follow effective date");
        if(LoanScheduleGeneration.DueDate(date,0,12,1,0)!=date)throw new Exception("Beginning-period schedule must start on effective date");
        if(LoanScheduleGeneration.DueDate(new DateTime(2026,1,31),0,12,0,0)!=new DateTime(2026,2,28))throw new Exception("Month-end anchor invalid");
        CheckService(date,period);
        Console.WriteLine("PASS: effective disbursement date, open period, approval/application boundaries, and schedule anchors.");
    }
}
