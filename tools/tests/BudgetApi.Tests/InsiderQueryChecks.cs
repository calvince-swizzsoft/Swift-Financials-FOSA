using System;
using System.Net.Http;
using System.Web.Http;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using Application.MainBoundedContext.AccountsModule.Services;
using WebApplication1.Areas.Accounts.Controllers;

public class InsiderQueryProbeController : ApiController
{
    public IHttpActionResult Get(string kind="appointments",string text="",int page=0,int size=20)
    {
        var controller=new SasraInsiderController((ISasraInsiderAppService)new InsiderQueryStub().GetTransparentProxy());
        controller.ActionContext=ActionContext;
        if(kind=="candidates")return controller.Candidates(text,page,size);
        if(kind=="loans")return controller.Loans(text,page,size);
        return controller.Appointments(text,page,size);
    }
}
public class InsiderQueryStub : RealProxy
{
    public static int Calls; public static string LastText; public static string LastMethod;
    public InsiderQueryStub():base(typeof(ISasraInsiderAppService)){}
    public override IMessage Invoke(IMessage message)
    {
        var call=(IMethodCallMessage)message;
        if(call.MethodName!="GetAppointments"&&call.MethodName!="GetCandidates"&&call.MethodName!="GetLoans")
            return new ReturnMessage(new Exception("Unexpected service call"),call);
        Calls++; LastText=(string)call.Args[0]; LastMethod=call.MethodName;
        return new ReturnMessage(null,null,0,call.LogicalCallContext,call);
    }
}
static class InsiderQueryChecks
{
    public static void Run()
    {
        using(var config=new HttpConfiguration())
        {
            config.Routes.MapHttpRoute("insider-probe","api/{controller}");
            using(var server=new HttpServer(config))using(var client=new HttpClient(server))
            foreach(var kind in new[]{"appointments","candidates","loans"})
            foreach(var query in new[]{"&text=&page=0&size=20","&page=0&size=20","&text=%20&page=0&size=20","&text=Jane%20%26%20John&page=0&size=20","&text=&page=bad&size=20","&text=&page=0&size=bad","&text=&page=999999999999&size=20"})
            {
                int before=InsiderQueryStub.Calls;
                var response=client.GetAsync("http://localhost/api/insiderqueryprobe?kind="+kind+query).Result;
                int expected=query.Contains("bad")||query.Contains("999999999999")?400:200;
                if((int)response.StatusCode!=expected)throw new Exception(kind+query+": "+response.Content.ReadAsStringAsync().Result);
                if(InsiderQueryStub.Calls-before!=(expected==200?1:0))throw new Exception("Incorrect service call count");
                if(expected==200&&InsiderQueryStub.LastMethod!="Get"+char.ToUpperInvariant(kind[0])+kind.Substring(1))throw new Exception("Wrong list queried");
                if(query.Contains("Jane")&&InsiderQueryStub.LastText!="Jane & John")throw new Exception("Search changed");
                Console.WriteLine("PASS insider "+kind+" "+expected+" "+query);
            }
        }
    }
}