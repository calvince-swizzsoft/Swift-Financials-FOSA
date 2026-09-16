using System;
using System.Net;
using System.Web.Http;
using Application.MainBoundedContext.AdministrationModule.Services;
using WebApplication1.ApiErrors;
namespace WebApplication1.Areas.BackOffice.Controllers
{
 [Authorize,RoutePrefix("api/backoffice/loan-notices")]
 public class LoanNoticePolicyController:ApiController
 {
  readonly ICompanyAppService service;
  public LoanNoticePolicyController(ICompanyAppService service){this.service=service;}
  [HttpGet,Route("cases/{id:guid}/policy")]
  public IHttpActionResult Policy(Guid id){
   try{return Ok(new{success=true,data=service.ResolveDefaulterNoticePolicy(id,WebApplication1.Helpers.Utils.CreateServiceHeader())});}
   catch(InvalidOperationException e){return ResponseMessage(ApiErrorResponses.Create(Request,HttpStatusCode.BadRequest,ErrorCodes.ValidationFailed,e.Message));}
  }
 }
}
