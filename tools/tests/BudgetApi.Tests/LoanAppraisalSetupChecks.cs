using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Application.MainBoundedContext.AccountsModule.Services;
using WebApplication1.ApiErrors;

public class LoanAppraisalSetupProbeController : ApiController
{
    public IHttpActionResult Get(bool income=false, bool maker=false)
    {
        if(maker) throw new Application.Seedwork.MakerCheckerViolationException();
        if(income) throw new Application.MainBoundedContext.BackOfficeModule.Services.LoanIncomeAssessmentException("Verified income assessment is required.");
        throw new LoanAppraisalConfigurationException("Select eligible investment products before calculating deposit-based entitlement.");
    }
}
static class LoanAppraisalSetupChecks
{
    public static void Run()
    {
        LoanStageMakerCheckerChecks.Run();
        using(var config=new HttpConfiguration())
        {
            config.Services.Replace(typeof(IExceptionHandler),new ApiExceptionHandler());
            config.MessageHandlers.Add(new CorrelationIdHandler());
            config.MessageHandlers.Add(new ApiErrorNormalizationHandler());
            config.Routes.MapHttpRoute("appraisal-setup-probe","api/{controller}");
            using(var server=new HttpServer(config))using(var client=new HttpClient(server))
            {
                var response=client.GetAsync("http://localhost/api/loanappraisalsetupprobe").Result;
                var body=response.Content.ReadAsStringAsync().Result;
                if(response.StatusCode!=HttpStatusCode.Conflict || !body.Contains("LOAN_APPRAISAL_SETUP_REQUIRED") || !body.Contains("Select eligible investment products"))
                    throw new Exception("Missing appraisal mapping must return a clear 409 configuration error: "+body);
                Console.WriteLine("PASS loan appraisal missing mapping returns 409 LOAN_APPRAISAL_SETUP_REQUIRED");
                var income=client.GetAsync("http://localhost/api/loanappraisalsetupprobe?income=true").Result;
                var incomeBody=income.Content.ReadAsStringAsync().Result;
                if(income.StatusCode!=HttpStatusCode.Conflict || !incomeBody.Contains("LOAN_INCOME_ASSESSMENT_REQUIRED") || !incomeBody.Contains("Verified income assessment"))
                    throw new Exception("Missing income assessment must return a clear 409 response: "+incomeBody);
                Console.WriteLine("PASS loan income assessment returns 409 LOAN_INCOME_ASSESSMENT_REQUIRED");
                var maker=client.GetAsync("http://localhost/api/loanappraisalsetupprobe?maker=true").Result;
                var makerBody=maker.Content.ReadAsStringAsync().Result;
                if(maker.StatusCode!=HttpStatusCode.Conflict || !makerBody.Contains("MAKER_CHECKER_VIOLATION") || !makerBody.Contains("A different authorized user"))
                    throw new Exception("Maker/checker explanation was lost: "+makerBody);
                Console.WriteLine("PASS maker/checker 409 preserves actionable explanation through response normalization");
            }
        }
    }
}
