using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using Infrastructure.Crosscutting.Framework.Adapter;
using Domain.MainBoundedContext.AccountsModule.Aggregates.BudgetAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.BudgetEntryAgg;
using Domain.Seedwork;
using Numero3.EntityFramework.Interfaces;

static class BudgetSaveChecks
{
    static int checks;
    static void Assert(bool ok, string label) { if (!ok) throw new Exception(label); checks++; }
    class Proxy : RealProxy
    {
        readonly Func<IMethodCallMessage, object> run;
        public Proxy(Type t, Func<IMethodCallMessage, object> run) : base(t) { this.run = run; }
        public override IMessage Invoke(IMessage msg) { var c = (IMethodCallMessage)msg; try { return new ReturnMessage(run(c), null, 0, c.LogicalCallContext, c); } catch(Exception e) { return new ReturnMessage(e,c); } }
    }
    static object Stub(Type t, Func<IMethodCallMessage,object> f) { return new Proxy(t,f).GetTransparentProxy(); }
    class Mapping : ITypeAdapterFactory, ITypeAdapter
    {
        public ITypeAdapter Create() { return this; }
        public T Adapt<S,T>(S s) where S:class where T:class,new() { return Adapt<T>(s); }
        public T Adapt<T>(object s) where T:class,new() { var b=(Budget)s; return (T)(object)new BudgetDTO { Id=b.Id,TotalValue=b.TotalValue }; }
    }
    static BudgetDTO Model() { return new BudgetDTO {Description="Test",BranchId=Guid.NewGuid(),PostingPeriodId=Guid.NewGuid(),TotalValue=100}; }
    static List<BudgetEntryDTO> Lines() { return new List<BudgetEntryDTO>{new BudgetEntryDTO {Type=0,ChartOfAccountId=Guid.NewGuid(),Amount=100}}; }
    static void Invalid(Action<BudgetDTO,List<BudgetEntryDTO>> change, string expectedField)
    {
        var m=Model(); var e=Lines(); change(m,e);
        try { typeof(BudgetAppService).GetMethod("ValidateAppropriation",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{m,e}); throw new Exception("Invalid request accepted"); }
        catch(TargetInvocationException x) { var error=x.InnerException as BudgetValidationException; Assert(error!=null && error.Field==expectedField,"Useful field error: "+expectedField); }
    }
    public static void Run()
    {
        Invalid((m,e)=>m.BranchId=null,"Budget.BranchId");
        Invalid((m,e)=>m.PostingPeriodId=Guid.Empty,"Budget.PostingPeriodId");
        Invalid((m,e)=>m.Description=" ","Budget.Description");
        Invalid((m,e)=>m.TotalValue=-100,"Budget.TotalValue");
        Invalid((m,e)=>m.TotalValue=100.001m,"Budget.TotalValue");
        Invalid((m,e)=>e.Clear(),"Entries");
        Invalid((m,e)=>e[0]=null,"Entries[0]");
        Invalid((m,e)=>e[0].Type=2,"Entries[0].Type");
        Invalid((m,e)=>e[0].Amount=0,"Entries[0].Amount");
        Invalid((m,e)=>e[0].Amount=100.001m,"Entries[0].Amount");
        Invalid((m,e)=>e[0].ChartOfAccountId=null,"Entries[0].ChartOfAccountId");
        Invalid((m,e)=>e[0].LoanProductId=Guid.NewGuid(),"Entries[0].ChartOfAccountId");
        Invalid((m,e)=>{e[0].Type=1;e[0].ChartOfAccountId=null;},"Entries[0].LoanProductId");
        Invalid((m,e)=>e[0].Reference=new string('x',257),"Entries[0].Reference");
        Invalid((m,e)=>e[0].Amount=90,"Entries");
        Invalid((m,e)=>e[0].Amount=110,"Entries");
        TypeAdapterFactory.SetCurrent(new Mapping());
        foreach(var mode in new[]{"create","update","failure","duplicate","missingTarget","missingBudget"})
        {
            var m=Model(); var lines=Lines(); int commits=0, added=0, removed=0, mutations=0;
            var existing=BudgetFactory.CreateBudget(m.PostingPeriodId,m.BranchId.Value,"Old",50);
            if(mode=="update" || mode=="missingBudget") m.Id=existing.Id;
            var factory=Stub(typeof(IDbContextScopeFactory),c=>{
                Assert(c.MethodName=="CreateWithTransaction" && (System.Data.IsolationLevel)c.Args[0]==System.Data.IsolationLevel.Serializable,"Atomic serializable scope");
                return Stub(typeof(IDbContextScope),sc=>{if(sc.MethodName=="SaveChanges"){ Assert(added==1,"Header and line prepared before sole commit");commits++;return 1;} return null;});
            });
            var budgets=Stub(typeof(IRepository<Budget>),c=>{
                if(c.MethodName=="Get")return mode=="missingBudget"?null:existing;
                if(c.MethodName=="AllMatching")return mode=="duplicate"?new List<Budget>{existing}:new List<Budget>();
                if(c.MethodName=="DatabaseSqlQuery")return new[]{mode=="missingTarget"?0:1};
                if(c.MethodName=="Add" || c.MethodName=="Merge"){mutations++;return null;}
                throw new Exception(c.MethodName);
            });
            var entries=Stub(typeof(IRepository<BudgetEntry>),c=>{
                if(c.MethodName=="AllMatching")return new List<BudgetEntry>{new BudgetEntry()};
                if(c.MethodName=="Remove"){removed++;return null;}
                if(c.MethodName=="Add"){if(mode=="failure")throw new InvalidOperationException("Simulated persistence failure");added++;return null;}
                throw new Exception(c.MethodName);
            });
            var ctor=typeof(BudgetAppService).GetConstructors().Single();
            var parameters=ctor.GetParameters();
            var service=(BudgetAppService)ctor.Invoke(new[]{factory,budgets,entries,Stub(parameters[3].ParameterType,c=>null),Stub(parameters[4].ParameterType,c=>null)});
            try { service.SaveBudget(m,lines,new ServiceHeader{ApplicationUserName="test"}); Assert(mode=="create" || mode=="update","Expected failure"); Assert(commits==1 && mutations==1,"One commit for header and lines"); Assert(removed==(mode=="update"?1:0),"Replacement only on update"); }
            catch(BudgetValidationException e) { Assert(commits==0 && mutations==0,"Validation prevents mutations"); Assert(e.StatusCode==(mode=="duplicate"?409:mode=="missingBudget"?404:400),"Correct HTTP classification"); }
            catch(InvalidOperationException) { Assert(mode=="failure" && commits==0,"Persistence failure cannot commit header alone"); }
        }
        Console.WriteLine("PASS: "+checks+" appropriation validation and save assertions (test doubles, no database writes).");
    }
}
