using System;
using System.Linq;
using System.Net;
using System.Data.SqlClient;
using System.Web.Http;
using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using WebApplication1.Helpers;
using WebApplication1.ApiErrors;
namespace WebApplication1.Areas.Accounts.Controllers
{
    [Authorize,RoutePrefix("api/accounts/sasra/insiders")]
    public class SasraInsiderController:ApiController
    {
        readonly ISasraInsiderAppService service;
        public SasraInsiderController(ISasraInsiderAppService service){this.service=service??throw new ArgumentNullException(nameof(service));}
        static bool SqlFailure(Exception ex,params int[] codes){for(var e=ex;e!=null;e=e.InnerException)if(e is SqlException&&codes.Contains(((SqlException)e).Number))return true;return false;}
        IHttpActionResult Execute<T>(Func<T> action){
            if(!ModelState.IsValid)return ResponseMessage(ApiErrorResponses.Create(Request,HttpStatusCode.BadRequest,"sasra.invalid_request","Check the supplied dates, numbers and identifiers."));
            try{return Ok(new{success=true,message="",data=action()});}
            catch(SasraSetupException e){return ResponseMessage(ApiErrorResponses.Create(Request,(HttpStatusCode)e.Status,"sasra.validation",e.Message));}
            catch(LoanAgeingException e){return ResponseMessage(ApiErrorResponses.Create(Request,(HttpStatusCode)e.Status,"sasra.loan_data",e.Message));}
            catch(Exception e) when(SqlFailure(e,207,208)){return ResponseMessage(ApiErrorResponses.Create(Request,HttpStatusCode.ServiceUnavailable,"sasra.schema_missing","Apply the Form 9 database migration before using insider reporting."));}
            catch(Exception e) when(SqlFailure(e,1205,2601,2627)){return ResponseMessage(ApiErrorResponses.Create(Request,HttpStatusCode.Conflict,"sasra.conflict","Another change was saved. Reload and try again."));}
        }
        [HttpGet,Route("appointments")] public IHttpActionResult Appointments(string text="",int page=0,int size=20){
            // Empty search is valid. Web API binds ?text= as a missing string value.
            // Preserve other binding errors, including invalid page and size values.
            if(string.IsNullOrWhiteSpace(text))ModelState.Remove("text.String");
            return Execute(()=>service.GetAppointments(text,page,size,Utils.CreateServiceHeader()));
        }
        [HttpPost,Route("appointments")] public IHttpActionResult SaveAppointment(InsiderAppointmentDTO input){return Execute(()=>service.SaveAppointment(input,Utils.CreateServiceHeader()));}
        [HttpGet,Route("candidates")] public IHttpActionResult Candidates(string text="",int page=0,int size=20){
            // Empty search is valid. Web API binds ?text= as a missing string value.
            // Preserve other binding errors, including invalid page and size values.
            if(string.IsNullOrWhiteSpace(text))ModelState.Remove("text.String");
            return Execute(()=>service.GetCandidates(text,page,size,Utils.CreateServiceHeader()));
        }
        [HttpGet,Route("loans")] public IHttpActionResult Loans(string text="",int page=0,int size=20){
            // Empty search is valid. Web API binds ?text= as a missing string value.
            // Preserve other binding errors, including invalid page and size values.
            if(string.IsNullOrWhiteSpace(text))ModelState.Remove("text.String");
            return Execute(()=>service.GetLoans(text,page,size,Utils.CreateServiceHeader()));
        }
        [HttpGet,Route("board/{id:guid}")] public IHttpActionResult Decision(Guid id){return Execute(()=>service.GetDecision(id,Utils.CreateServiceHeader()));}
        [HttpPut,Route("board/{id:guid}")] public IHttpActionResult SaveDecision(Guid id,InsiderDecisionDTO input){if(input!=null)input.LoanCaseId=id;return Execute(()=>service.SaveDecision(input,Utils.CreateServiceHeader()));}
        [HttpGet,Route("policy")] public IHttpActionResult Policy(){return Execute(()=>service.GetPolicy(Utils.CreateServiceHeader()));}
        [HttpPut,Route("policy")] public IHttpActionResult SavePolicy(Form9PolicyDTO input){return Execute(()=>service.SavePolicy(input,Utils.CreateServiceHeader()));}
        [HttpGet,Route("products")] public IHttpActionResult Products(){return Execute(()=>service.GetProducts(Utils.CreateServiceHeader()));}
        [HttpPost,Route("preview")] public IHttpActionResult Preview(Form9Request input){return Execute(()=>service.Preview(input,Utils.CreateServiceHeader()));}
        [HttpPost,Route("runs")] public IHttpActionResult SaveRun(Form9Request input){return Execute(()=>service.SaveDraft(input,Utils.CreateServiceHeader()));}
        [HttpGet,Route("runs")] public IHttpActionResult Runs(int page=0,int size=20){return Execute(()=>service.GetRuns(page,size,Utils.CreateServiceHeader()));}
        [HttpGet,Route("runs/{id:guid}")] public IHttpActionResult Run(Guid id){return Execute(()=>service.GetRun(id,Utils.CreateServiceHeader()));}
        [HttpGet,Route("runs/{id:guid}/export")] public IHttpActionResult Export(Guid id){return Execute(()=>service.Export(id,Utils.CreateServiceHeader()));}
        [HttpGet,Route("history/{kind}/{id:guid}")] public IHttpActionResult History(string kind,Guid id){return Execute(()=>service.History(kind,id,Utils.CreateServiceHeader()));}
    }
}
