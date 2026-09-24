using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using Application.MainBoundedContext.AccountsModule.Services;
using WebApplication1.Areas.Accounts.Controllers;
public class BudgetListProbeController : ApiController
{
    public IHttpActionResult Get(string text = "", int pageIndex = 0, int pageSize = 20)
    {
        var controller = new BudgetController((IBudgetAppService)new BudgetStub().GetTransparentProxy());
        controller.ActionContext = ActionContext;
        return controller.Get(text, pageIndex, pageSize);
    }
}
public class BudgetStub : RealProxy
{
    public static int Calls;
    public BudgetStub() : base(typeof(IBudgetAppService)) { }
    public override IMessage Invoke(IMessage msg)
    {
        var call = (IMethodCallMessage)msg;
        if (call.MethodName != "FindBudgets") return new ReturnMessage(new Exception("Unexpected service call"), call);
        Calls++;
        return new ReturnMessage(null, null, 0, call.LogicalCallContext, call);
    }
}
class Program
{
    static void Main()
    {
        LoanAppraisalSetupChecks.Run();
        InsiderQueryChecks.Run();
        LoanAgeingQueryChecks.Run();
        using (var config = new HttpConfiguration())
        {
            config.Routes.MapHttpRoute("probe", "api/{controller}");
            using (var server = new HttpServer(config)) using (var client = new HttpClient(server))
            {
                foreach (var query in new[] { "?text=&pageIndex=0&pageSize=20", "?pageIndex=0&pageSize=20", "?text=%20&pageIndex=0&pageSize=20", "?text=rent&pageIndex=0&pageSize=20", "?text=&pageIndex=bad&pageSize=20", "?text=&pageIndex=-1&pageSize=20", "?text=&pageIndex=0&pageSize=101" })
                {
                    var before = BudgetStub.Calls;
                    var response = client.GetAsync("http://localhost/api/budgetlistprobe" + query).Result;
                    var expected = query.Contains("bad") || query.Contains("-1") || query.Contains("101") ? 400 : 200;
                    if ((int)response.StatusCode != expected) throw new Exception(query + ": " + response.Content.ReadAsStringAsync().Result);
                    if (BudgetStub.Calls - before != (expected == 200 ? 1 : 0)) throw new Exception("Incorrect service call count");
                    Console.WriteLine("PASS " + expected + " " + query);
                }
            }
        }
    }
}
