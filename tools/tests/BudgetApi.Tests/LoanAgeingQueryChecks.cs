using System;
using System.Net.Http;
using System.Web.Http;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using Application.MainBoundedContext.BackOfficeModule.Services;

using WebApplication1.Areas.BackOffice.Controllers;
public class LoanAgeingQueryProbeController : ApiController
{
 public IHttpActionResult Get(string text="",int pageIndex=0,int pageSize=20)
 {
  var controller=new LoanAgeingController((ILoanAgeingAppService)new LoanAgeingQueryStub().GetTransparentProxy());
  controller.ActionContext=ActionContext;
  return controller.Cases(text,pageIndex,pageSize);
 }
}
public class LoanAgeingQueryStub : RealProxy
{
 public static int Calls;public static string LastText;
 public LoanAgeingQueryStub():base(typeof(ILoanAgeingAppService)){}
 public override IMessage Invoke(IMessage message)
 {
  var call=(IMethodCallMessage)message;
  if(call.MethodName!="GetCases")return new ReturnMessage(new Exception("Unexpected service call"),call);
  Calls++;LastText=(string)call.Args[0];
  return new ReturnMessage(null,null,0,call.LogicalCallContext,call);
 }
}
static class LoanAgeingQueryChecks
{
 public static void Run()
 {
  using(var config=new HttpConfiguration())
  {
   config.Routes.MapHttpRoute("loan-query-probe","api/{controller}");
   using(var server=new HttpServer(config))using(var client=new HttpClient(server))
   {
    foreach(var query in new[]{"?text=&pageIndex=0&pageSize=20","?pageIndex=0&pageSize=20","?text=%20&pageIndex=0&pageSize=20","?text=school&pageIndex=0&pageSize=20","?text=&pageIndex=bad&pageSize=20","?text=&pageIndex=0&pageSize=bad","?text=&pageIndex=999999999999&pageSize=20"})
    {
     int before=LoanAgeingQueryStub.Calls;
     var response=client.GetAsync("http://localhost/api/loanageingqueryprobe"+query).Result;
     int expected=query.Contains("bad")||query.Contains("999999999999")?400:200;
     if((int)response.StatusCode!=expected)throw new Exception(query+": "+response.Content.ReadAsStringAsync().Result);
     if(LoanAgeingQueryStub.Calls-before!=(expected==200?1:0))throw new Exception("Incorrect service calls for "+query);
     if(query.Contains("school")&&LoanAgeingQueryStub.LastText!="school")throw new Exception("Search text was changed");
     Console.WriteLine("PASS loan ageing "+expected+" "+query);
    }
   }
  }
 }
}
