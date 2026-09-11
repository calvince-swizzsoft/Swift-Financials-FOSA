using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Threading.Tasks;
using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CashFlowMappingAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;

static class RepositoryChecks
{
    sealed class Stub<T> : RealProxy
    {
        readonly Func<IMethodCallMessage,object> handler;
        public Stub(Func<IMethodCallMessage,object> handler) : base(typeof(T)) { this.handler=handler; }
        public override IMessage Invoke(IMessage message)
        {
            var call=(IMethodCallMessage)message;
            try { return new ReturnMessage(handler(call),null,0,call.LogicalCallContext,call); }
            catch(Exception ex) { return new ReturnMessage(ex,call); }
        }
        public T Proxy { get { return (T)GetTransparentProxy(); } }
    }

    public static void Run(Action<bool,string> check)
    {
        var saved=0;
        var added=0;
        var removed=0;
        var records=new List<CashFlowMapping>();
        var accountId=Guid.NewGuid();
        var account=new ChartOfAccount { AccountType=1000, AccountCode=101, AccountName="Bank" };
        var readScope=new Stub<IDbContextReadOnlyScope>(call => null).Proxy;
        var writeScope=new Stub<IDbContextScope>(call => {
            if(call.MethodName=="SaveChangesAsync") { saved++; return Task.FromResult(1); }
            if(call.MethodName=="Dispose") return null;
            throw new Exception("Unexpected write-scope call: "+call.MethodName);
        }).Proxy;
        var scopes=new Stub<IDbContextScopeFactory>(call => {
            if(call.MethodName=="Create" || call.MethodName=="CreateWithTransaction") return writeScope;
            if(call.MethodName=="CreateReadOnly") return readScope;
            throw new Exception("Unexpected scope: "+call.MethodName);
        }).Proxy;
        var accounts=new Stub<IRepository<ChartOfAccount>>(call => {
            if(call.MethodName=="GetAsync") return Task.FromResult(account);
            if(call.MethodName=="AllMatchingCountAsync") return Task.FromResult(0);
            throw new Exception("Mapping CRUD bypassed repository operations: "+call.MethodName);
        }).Proxy;
        var mappings=new Stub<IRepository<CashFlowMapping>>(call => {
            if(call.MethodName=="AllMatchingAsync") return Task.FromResult(records.Where(((ISpecification<CashFlowMapping>)call.Args[0]).SatisfiedBy().Compile()).ToList());
            if(call.MethodName=="Add") {
                var mapping=(CashFlowMapping)call.Args[0];
                typeof(CashFlowMapping).GetProperty("ChartOfAccount").SetValue(mapping,account);
                records.Add(mapping); added++; return null;
            }
            if(call.MethodName=="Remove") { records.Remove((CashFlowMapping)call.Args[0]); removed++; return null; }
            throw new Exception("Unexpected mapping operation: "+call.MethodName);
        }).Proxy;
        var service=new CashFlowAppService(scopes,accounts,mappings);
        var header=new ServiceHeader { ApplicationUserName="tester" };
        var input=new CashFlowMappingDTO { ChartOfAccountId=accountId,Section="Cash",Line="Bank" };
        service.SaveMapping(input,header).GetAwaiter().GetResult();
        check(added==1 && saved==1 && records.Count==1,"Create adds domain entity and saves scope");
        var id=records[0].Id;
        input.Line="Cash at bank";
        service.SaveMapping(input,header).GetAwaiter().GetResult();
        check(added==1 && saved==2 && records[0].Id==id && records[0].Line==input.Line,"Update tracks existing entity without replacing identity");
        var rows=service.GetMappings(header).GetAwaiter().GetResult();
        check(rows.Count==1 && rows[0].AccountCode=="101" && rows[0].AccountName=="Bank","Mapping list projects repository account details");
        service.RemoveMapping(accountId,header).GetAwaiter().GetResult();
        check(removed==1 && saved==3 && records.Count==0,"Delete uses repository and commits");
        service.RemoveMapping(accountId,header).GetAwaiter().GetResult();
        check(removed==1 && saved==3,"Missing mapping removal is idempotent");
        input.Section="Internal";
        var rejected=false;
        try { service.SaveMapping(input,header).GetAwaiter().GetResult(); } catch(CashFlowValidationException) { rejected=true; }
        check(rejected && records.Count==0 && saved==3,"Domain validation prevents invalid repository writes");
    }
}
