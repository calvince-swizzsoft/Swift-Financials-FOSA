using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Data.SqlClient;
using System.Web.Http;
using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using WebApplication1.ApiErrors;
using WebApplication1.Helpers;
namespace WebApplication1.Areas.BackOffice.Controllers
{
 [Authorize,RoutePrefix("api/backoffice/loan-notices")]
 public class LoanNoticeController:ApiController
 {
  readonly ILoanNoticeAppService service;
  public LoanNoticeController(ILoanNoticeAppService service){this.service=service??throw new ArgumentNullException(nameof(service));}
  static bool SqlFailure(Exception ex,params int[] codes){for(var e=ex;e!=null;e=e.InnerException)if(e is SqlException&&codes.Contains(((SqlException)e).Number))return true;return false;}
  IHttpActionResult Execute(Func<IHttpActionResult> action)
  {
   if(!ModelState.IsValid)return ResponseMessage(ApiErrorResponses.Create(Request,HttpStatusCode.BadRequest,"loan_notice.invalid_request","Select a valid date and notice, then try again."));
   try{return action();}
   catch(LoanAgeingException e){return ResponseMessage(ApiErrorResponses.Create(Request,(HttpStatusCode)e.Status,"loan_notice.validation",e.Message));}
   catch(Exception e)when(SqlFailure(e,207,208)){return ResponseMessage(ApiErrorResponses.Create(Request,HttpStatusCode.ServiceUnavailable,"loan_notice.migration_required","Run the updated Utility automatic migration and restart the API to enable notice storage."));}
   catch(Exception e)when(SqlFailure(e,1205,2601,2627)||e is System.Data.Entity.Infrastructure.DbUpdateConcurrencyException){return ResponseMessage(ApiErrorResponses.Create(Request,HttpStatusCode.Conflict,"loan_notice.conflict","Another user changed or generated this notice. Refresh the list to view its latest state."));}
  }
  [HttpGet,Route("eligible")]public IHttpActionResult Eligible(DateTime asAt,int pageIndex=0,int pageSize=20){return Execute(()=>Ok(new{success=true,data=service.Eligible(asAt,pageIndex,pageSize,Utils.CreateServiceHeader())}));}
  [HttpGet,Route("")]public IHttpActionResult History(int pageIndex=0,int pageSize=20){return Execute(()=>Ok(new{success=true,data=service.History(pageIndex,pageSize,Utils.CreateServiceHeader())}));}
  [HttpGet,Route("{id:guid}")]public IHttpActionResult Get(Guid id){return Execute(()=>Ok(new{success=true,data=service.Get(id,Utils.CreateServiceHeader())}));}
  [HttpPost,Route("generate")]public IHttpActionResult Generate(LoanNoticeRequest input){return Execute(()=>Ok(new{success=true,data=service.Generate(input,Utils.CreateServiceHeader())}));}
  [HttpPost,Route("{id:guid}/approve")]public IHttpActionResult Approve(Guid id){return Execute(()=>Ok(new{success=true,data=service.Approve(id,Utils.CreateServiceHeader())}));}
  [HttpPost,Route("{id:guid}/cancel")]public IHttpActionResult Cancel(Guid id){return Execute(()=>Ok(new{success=true,data=service.Cancel(id,Utils.CreateServiceHeader())}));}
  [HttpGet,Route("{id:guid}/print")]public IHttpActionResult Print(Guid id){return Execute(()=>{
   var response=new HttpResponseMessage(HttpStatusCode.OK){Content=new StringContent(service.Printable(id,Utils.CreateServiceHeader()),Encoding.UTF8,"text/html")};
   response.Content.Headers.ContentDisposition=new ContentDispositionHeaderValue("attachment"){FileName="loan-notice-"+id+".html"};
   response.Headers.TryAddWithoutValidation("Content-Security-Policy","default-src 'none'; style-src 'unsafe-inline'");
   response.Headers.CacheControl=new CacheControlHeaderValue{NoStore=true};return ResponseMessage(response);
  });}
 }
}
