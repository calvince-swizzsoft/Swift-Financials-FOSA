using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Serialization;
using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;

class Stub : RealProxy
{
    readonly Func<IMethodCallMessage, object> call;
    public Stub(Type type, Func<IMethodCallMessage, object> call) : base(type) { this.call = call; }
    public override IMessage Invoke(IMessage message)
    {
        var method = (IMethodCallMessage)message;
        try { return new ReturnMessage(call(method), null, 0, method.LogicalCallContext, method); }
        catch (Exception ex) { return new ReturnMessage(ex, method); }
    }
}
class Regression
{
    static string backend;
    static int Main(string[] args)
    {
        backend = args[0];
        AppDomain.CurrentDomain.AssemblyResolve += (s,e) => {
            var path = Path.Combine(backend, new AssemblyName(e.Name).Name + ".dll");
            return File.Exists(path) ? Assembly.LoadFrom(path) : null;
        };
        try { Run(); Console.WriteLine("Check-Off regression checks passed."); return 0; }
        catch(Exception e) { Console.Error.WriteLine(e); return 1; }
    }
    static void Set(object service, string field, object value) { service.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(service,value); }
    static object Fake(Type type, Func<IMethodCallMessage,object> call) { return new Stub(type,call).GetTransparentProxy(); }
    static void Require(bool ok, string text) { if (!ok) throw new Exception(text); }
    [MethodImpl(MethodImplOptions.NoInlining)]
    static void Run()
    {
        var service = (CreditBatchAppService)FormatterServices.GetUninitializedObject(typeof(CreditBatchAppService));
        var productId = Guid.NewGuid();
        var account = new CustomerAccountDTO { Id=Guid.NewGuid(), CustomerId=Guid.NewGuid(), BranchId=Guid.NewGuid(), CustomerAccountTypeTargetProductId=productId, CustomerAccountTypeProductCode=3 };
        int lookupCount=0;
        bool ambiguous=false;
        Set(service,"_investmentProductAppService",Fake(typeof(IInvestmentProductAppService), m => new List<InvestmentProductDTO> { new InvestmentProductDTO { Id=productId, Code=17, Description="Shares" } }));
        Set(service,"_loanProductAppService",Fake(typeof(ILoanProductAppService), m => new List<LoanProductDTO>()));
        Set(service,"_sqlCommandAppService",Fake(typeof(ISqlCommandAppService), m => {
            if (m.MethodName.Contains("Reference3")) throw new Exception("Personal file lookup must not be used.");
            Require(m.MethodName=="FindCustomerAccountsByTargetProductIdAndPayrollNumber","Unexpected SQL lookup: "+m.MethodName);
            Require((Guid)m.Args[0]==productId,"Wrong product");
            Require((string)m.Args[1]=="0007998","Payroll leading zeros changed");
            lookupCount++;
            return ambiguous ? new List<CustomerAccountDTO>{account,account} : new List<CustomerAccountDTO>{account};
        }));
        var parse=typeof(CreditBatchAppService).GetMethod("ParseCheckOff",BindingFlags.Instance|BindingFlags.NonPublic);
        var entryType=parse.GetParameters()[1].ParameterType.GetGenericArguments()[0];
        Func<string, object> run= payroll => {
            var row=Activator.CreateInstance(entryType);
            string[] values={"Development",payroll,"100.00","0.00","Existing member","17","sShare","TEST"};
            for(int i=0;i<8;i++) entryType.GetProperty("Column"+(i+1)).SetValue(row,values[i],null);
            var rows=(IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(entryType)); rows.Add(row);
            return parse.Invoke(service,new object[]{FormatterServices.GetUninitializedObject(parse.GetParameters()[0].ParameterType),rows,new ServiceHeader()});
        };
        Func<object,string,int> count=(result,property)=>((ICollection)result.GetType().GetProperty(property).GetValue(result,null)).Count;
        var matched=run("0007998");
        Require(count(matched,"MatchedCollection1")==1 && count(matched,"MismatchedCollection")==0,"Payroll match failed");
        var blank=run(" ");
        Require(count(blank,"MatchedCollection1")==0 && count(blank,"MismatchedCollection")==1 && lookupCount==1,"Blank payroll was not rejected before querying");
        ambiguous=true;
        var duplicate=run("0007998");
        Require(count(duplicate,"MatchedCollection1")==0 && count(duplicate,"MismatchedCollection")==1,"Ambiguous match was accepted");
    }
}
