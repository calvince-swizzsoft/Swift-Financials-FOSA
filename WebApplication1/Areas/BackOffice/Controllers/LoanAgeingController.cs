using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Http;
using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using WebApplication1.Helpers;
using WebApplication1.ApiErrors;
namespace WebApplication1.Areas.BackOffice.Controllers
{
 [Authorize,RoutePrefix("api/backoffice/loan-ageing")]
 public class LoanAgeingController : ApiController
 {
  readonly ILoanAgeingAppService service;public LoanAgeingController(ILoanAgeingAppService service){this.service=service??throw new ArgumentNullException(nameof(service));}
  static bool SqlFailure(Exception ex,params int[] codes){for(var e=ex;e!=null;e=e.InnerException)if(e is SqlException&&codes.Contains(((SqlException)e).Number))return true;return false;}
  IHttpActionResult Execute<T>(Func<T> action)
  {
   if(!ModelState.IsValid)return ResponseMessage(ApiErrorResponses.Create(Request,HttpStatusCode.BadRequest,"loan_ageing.invalid_request","Check the date, amount or identifier in the loan-ageing request.",ModelState.Where(x=>x.Value.Errors.Count>0).ToDictionary(x=>x.Key,x=>new[]{"Enter a valid date, amount or identifier."})));
   try{return Ok(new{success=true,message="",data=action()});}
   catch(LoanAgeingException e){return ResponseMessage(ApiErrorResponses.Create(Request,(HttpStatusCode)e.Status,"loan_ageing.validation",e.Message,new Dictionary<string,string[]>{{e.Field,new[]{e.Message}}}));}
   catch(Exception e)when(SqlFailure(e,207,208)){return ResponseMessage(ApiErrorResponses.Create(Request,HttpStatusCode.ServiceUnavailable,"loan_ageing.migration_required","Loan ageing is unavailable until the backend automatic migration has added the repayment-plan tables."));}
   catch(Exception e)when(SqlFailure(e,1205,2601,2627)){return ResponseMessage(ApiErrorResponses.Create(Request,HttpStatusCode.Conflict,"loan_ageing.conflict","Another schedule was saved concurrently. Reload the latest revision and try again."));}
  }
  [HttpPost,Route("form4")]public IHttpActionResult Form4(SasraForm4Request input){return Execute(()=>service.PreviewForm4(input,Utils.CreateServiceHeader()));}
  [HttpPost,Route("risk-reviews")]public IHttpActionResult RiskReview(LoanRiskReviewDTO input){return Execute(()=>service.SaveRiskReview(input,Utils.CreateServiceHeader()));}
  [HttpGet,Route("cases")]
  public IHttpActionResult Cases(string text="",int pageIndex=0,int pageSize=20)
  {
   // Web API binds ?text= to null and adds text.String as a required-value error.
   // Search is optional. Keep every other binding error, including invalid paging.
   if(string.IsNullOrWhiteSpace(text))ModelState.Remove("text.String");
   return Execute(()=>service.GetCases(text,pageIndex,pageSize,Utils.CreateServiceHeader()));
  }
  [HttpGet,Route("cases/{id:guid}/schedule-proposal")]public IHttpActionResult Proposal(Guid id){return Execute(()=>service.GenerateSchedule(id,Utils.CreateServiceHeader()));}
  [HttpPost,Route("generated-schedules/confirm")]public IHttpActionResult ConfirmGenerated(List<LoanScheduleConfirmationDTO> input){return Execute(()=>service.ConfirmGeneratedSchedules(input,Utils.CreateServiceHeader()));}
  [HttpGet,Route("cases/{id:guid}/plan")]public IHttpActionResult Plan(Guid id){return Execute(()=>service.GetPlan(id,Utils.CreateServiceHeader()));}
  [HttpGet,Route("cases/{id:guid}/history")]public IHttpActionResult History(Guid id){return Execute(()=>service.GetHistory(id,Utils.CreateServiceHeader()));}
  [HttpPost,Route("plans")]public IHttpActionResult Save(LoanPlanDTO input){return Execute(()=>service.SavePlan(input,Utils.CreateServiceHeader()));}
  [HttpGet,Route("loans")]public IHttpActionResult Loans(DateTime asAt,Guid? branchId=null,int pageIndex=0,int pageSize=20){return Execute(()=>service.GetLoanReport(asAt,branchId,pageIndex,pageSize,Utils.CreateServiceHeader()));}
  [HttpGet,Route("report")]public IHttpActionResult Report(DateTime asAt,Guid? branchId=null,int pageIndex=0,int pageSize=20,Guid? accountId=null){return Execute(()=>service.GetReport(asAt,branchId,pageIndex,pageSize,accountId,Utils.CreateServiceHeader()));}
 }
}
