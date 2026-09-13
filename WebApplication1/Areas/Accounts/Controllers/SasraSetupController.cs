using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Data.SqlClient;
using System.Web.Http;
using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using WebApplication1.Helpers;
using WebApplication1.ApiErrors;
namespace WebApplication1.Areas.Accounts.Controllers
{
    [Authorize,RoutePrefix("api/accounts/sasra/setup")]
    public class SasraSetupController : ApiController
    {
        readonly ISasraSetupAppService service;
        public SasraSetupController(ISasraSetupAppService service){this.service=service??throw new ArgumentNullException(nameof(service));}
        static bool SqlFailure(Exception ex,params int[] numbers)
        {
            for(var current=ex;current!=null;current=current.InnerException)
                if(current is SqlException && numbers.Contains(((SqlException)current).Number))return true;
            return false;
        }
        IHttpActionResult Execute<T>(Func<T> action)
        {
            if(!ModelState.IsValid)return ResponseMessage(ApiErrorResponses.Create(Request,HttpStatusCode.BadRequest,"sasra.invalid_request","The SASRA request contains an invalid date, number or account identifier.",ModelState.Where(x=>x.Value.Errors.Count>0).ToDictionary(x=>x.Key,x=>new[]{"Check the date, number or account identifier in this field."})));
            try{return Ok(new{success=true,message="",data=action()});}
            catch(Exception ex) when(SqlFailure(ex,207,208)){return ResponseMessage(ApiErrorResponses.Create(Request,HttpStatusCode.ServiceUnavailable,"sasra.schema_missing","SASRA setup is not available until the backend database migration is applied."));}
            catch(Exception ex) when(SqlFailure(ex,1205,2601,2627)){return ResponseMessage(ApiErrorResponses.Create(Request,HttpStatusCode.Conflict,"sasra.conflict","Another change was saved at the same time. Reload the latest revision and try again."));}
            catch(SasraSetupException ex){return ResponseMessage(ApiErrorResponses.Create(Request,(HttpStatusCode)ex.Status,"sasra.validation",ex.Message,new Dictionary<string,string[]>{{ex.Field,new[]{ex.Message}}}));}
        }
        [HttpGet,Route("form3/definition")] public IHttpActionResult Form3Definition(){return Execute(()=>service.GetForm3Definition(Utils.CreateServiceHeader()));}
        [HttpPost,Route("form3/preview")] public IHttpActionResult PreviewForm3(SasraForm3Request input){return Execute(()=>service.PreviewForm3(input,Utils.CreateServiceHeader()));}
        [HttpGet,Route("form2/definition")] public IHttpActionResult Form2Definition(){return Execute(()=>service.GetForm2Definition(Utils.CreateServiceHeader()));}
        [HttpPost,Route("form2/preview")] public IHttpActionResult PreviewForm2(SasraForm2Request input){return Execute(()=>service.PreviewForm2(input,Utils.CreateServiceHeader()));}
        [HttpGet,Route("form1/definition")] public IHttpActionResult Form1Definition(){return Execute(()=>service.GetForm1Definition(Utils.CreateServiceHeader()));}
        [HttpPost,Route("form1/preview")] public IHttpActionResult PreviewForm1(SasraForm1Request input){return Execute(()=>service.PreviewForm1(input,Utils.CreateServiceHeader()));}
        [HttpGet,Route("form7/definition")] public IHttpActionResult Form7Definition(){return Execute(()=>service.GetForm7Definition(Utils.CreateServiceHeader()));}
        [HttpPost,Route("form7/preview")] public IHttpActionResult PreviewForm7(SasraForm7Request input){return Execute(()=>service.PreviewForm7(input,Utils.CreateServiceHeader()));}
        [HttpGet,Route("form6/definition")] public IHttpActionResult Form6Definition(){return Execute(()=>service.GetForm6Definition(Utils.CreateServiceHeader()));}
        [HttpPost,Route("form6/preview")] public IHttpActionResult PreviewForm6(SasraForm6Request input){return Execute(()=>service.PreviewForm6(input,Utils.CreateServiceHeader()));}
        [HttpGet,Route("standard-definitions")] public IHttpActionResult StandardDefinitions(string profile){return Execute(()=>service.GetStandardDefinitions(profile));}
        [HttpPost,Route("standard-definitions")] public IHttpActionResult AddStandardDefinitions(){return Execute(()=>service.AddStandardDefinitions(Utils.CreateServiceHeader()));}
        [HttpGet,Route("profile")] public IHttpActionResult Profile(){return Execute(()=>service.GetProfile(Utils.CreateServiceHeader()));}
        [HttpPut,Route("profile")] public IHttpActionResult SaveProfile(SasraProfileDTO input){return Execute(()=>service.SaveProfile(input,Utils.CreateServiceHeader()));}
        [HttpGet,Route("versions")] public IHttpActionResult Versions(int pageIndex=0,int pageSize=20){return Execute(()=>service.GetVersions(pageIndex,pageSize,Utils.CreateServiceHeader()));}
        [HttpGet,Route("versions/{id:guid}")] public IHttpActionResult Version(Guid id){return Execute(()=>service.GetVersion(id,Utils.CreateServiceHeader()));}
        [HttpPost,Route("versions")] public IHttpActionResult SaveVersion(SasraVersionDTO input){return Execute(()=>service.SaveVersion(input,Utils.CreateServiceHeader()));}
    }
}
