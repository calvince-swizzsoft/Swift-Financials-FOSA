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
  readonly ILoanRecoveryAppService recovery;
  public LoanNoticeController(ILoanNoticeAppService service,ILoanRecoveryAppService recovery){this.service=service??throw new ArgumentNullException(nameof(service));this.recovery=recovery??throw new ArgumentNullException(nameof(recovery));}
  static bool SqlFailure(Exception ex,params int[] codes){for(var e=ex;e!=null;e=e.InnerException)if(e is SqlException&&codes.Contains(((SqlException)e).Number))return true;return false;}
  IHttpActionResult Execute(Func<IHttpActionResult> action)
  {
   if(!ModelState.IsValid)return ResponseMessage(ApiErrorResponses.Create(Request,HttpStatusCode.BadRequest,"loan_notice.invalid_request","Select a valid date and notice, then try again."));
   try{return action();}
   catch(LoanAgeingException e){return ResponseMessage(ApiErrorResponses.Create(Request,(HttpStatusCode)e.Status,"loan_notice.validation",e.Message));}
   catch(Exception e)when(SqlFailure(e,207,208)){return ResponseMessage(ApiErrorResponses.Create(Request,HttpStatusCode.ServiceUnavailable,"loan_notice.migration_required","Run the updated Utility automatic migration and restart the API to enable notice storage."));}
   catch(Exception e)when(SqlFailure(e,1205,2601,2627)||e is System.Data.Entity.Infrastructure.DbUpdateConcurrencyException){return ResponseMessage(ApiErrorResponses.Create(Request,HttpStatusCode.Conflict,"loan_notice.conflict","Another user changed or generated this notice. Refresh the list to view its latest state."));}
  }
  [HttpGet,Route("loans/{id:guid}/recovery")]public IHttpActionResult Recovery(Guid id){return Execute(()=>Ok(new{success=true,data=recovery.Preview(id,Utils.CreateServiceHeader())}));}
  [HttpPost,Route("loans/recovery/preview")]public IHttpActionResult RecoveryPreview(LoanRecoveryRequest input){return Execute(()=>Ok(new{success=true,data=recovery.Preview(input,Utils.CreateServiceHeader())}));}
  [HttpPost,Route("loans/recovery")]public IHttpActionResult Recover(LoanRecoveryRequest input){return Execute(()=>Ok(new{success=true,data=recovery.Post(input,Utils.CreateServiceHeader())}));}
  [HttpGet,Route("loans")]public IHttpActionResult Loans(DateTime asAt,string stage,int pageIndex=0,int pageSize=20){return Execute(()=>Ok(new{success=true,data=service.LoanWorkflow(asAt,stage,pageIndex,pageSize,Utils.CreateServiceHeader())}));}
  [HttpPost,Route("loans/action")]public IHttpActionResult LoanAction(LoanNoticeLoanAction input){return Execute(()=>Ok(new{success=true,data=service.LoanStageAction(input,Utils.CreateServiceHeader())}));}
  [HttpGet,Route("loans/{id:guid}/print")]public IHttpActionResult PrintLoan(Guid id,string stage){return Execute(()=>{
   var response=new HttpResponseMessage(HttpStatusCode.OK){Content=new StringContent(service.PrintableLoanStage(id,stage,Utils.CreateServiceHeader()),Encoding.UTF8,"text/html")};
   response.Content.Headers.ContentDisposition=new ContentDispositionHeaderValue("attachment"){FileName="loan-notices-"+id+".html"};
   response.Headers.CacheControl=new CacheControlHeaderValue{NoStore=true};return ResponseMessage(response);
  });}
  [HttpGet,Route("workflow")]public IHttpActionResult Workflow(DateTime asAt,string stage,int pageIndex=0,int pageSize=20){return Execute(()=>Ok(new{success=true,data=service.Workflow(asAt,stage,pageIndex,pageSize,Utils.CreateServiceHeader())}));}
  [HttpPost,Route("{id:guid}/send")]public IHttpActionResult Send(Guid id,LoanNoticeSendRequest input){return Execute(()=>Ok(new{success=true,data=service.Send(id,input,Utils.CreateServiceHeader())}));}
  [HttpPost,Route("{id:guid}/record-sent")]public IHttpActionResult RecordSent(Guid id,LoanNoticeDispatchRequest input){return Execute(()=>Ok(new{success=true,data=service.RecordSent(id,input,Utils.CreateServiceHeader())}));}
  [HttpGet,Route("eligible")]public IHttpActionResult Eligible(DateTime asAt,int pageIndex=0,int pageSize=20,bool otherOnly=false){return Execute(()=>Ok(new{success=true,data=service.Eligible(asAt,pageIndex,pageSize,Utils.CreateServiceHeader(),otherOnly)}));}
  [HttpGet,Route("")]public IHttpActionResult History(int pageIndex=0,int pageSize=20){return Execute(()=>Ok(new{success=true,data=service.History(pageIndex,pageSize,Utils.CreateServiceHeader())}));}
  [HttpGet,Route("{id:guid}")]public IHttpActionResult Get(Guid id){return Execute(()=>Ok(new{success=true,data=service.Get(id,Utils.CreateServiceHeader())}));}
  [HttpPost,Route("generate")]public IHttpActionResult Generate(LoanNoticeRequest input){return Execute(()=>Ok(new{success=true,data=service.Generate(input,Utils.CreateServiceHeader())}));}
  [HttpPost,Route("{id:guid}/refresh-approval")]public IHttpActionResult RefreshApproval(Guid id){return Execute(()=>Ok(new{success=true,data=service.RefreshApproval(id,Utils.CreateServiceHeader())}));}
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
