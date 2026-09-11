using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Accounts.Controllers
{
    [Authorize]
    [RoutePrefix("api/accounts/financial-statements/cash-flow")]
    public class CashFlowController : ApiController
    {
        private readonly ICashFlowAppService _service;
        public CashFlowController(ICashFlowAppService service) { _service=service ?? throw new ArgumentNullException(nameof(service)); }
        private async Task<IHttpActionResult> Respond<T>(Func<Task<T>> action)
        {
            if (!ModelState.IsValid) return Content(HttpStatusCode.BadRequest,new { success=false,message="Invalid cash-flow request." });
            try { return Ok(new { success=true,message="",data=await action() }); }
            catch (CashFlowValidationException ex) { return Content(HttpStatusCode.BadRequest,new { success=false,message=ex.Message }); }
        }
        [HttpGet,Route("")]
        public Task<IHttpActionResult> Generate(DateTime startDate,DateTime endDate,Guid? branchId=null)
        { return Respond(() => _service.Generate(startDate,endDate,branchId,Utils.CreateServiceHeader())); }
        [HttpGet,Route("entries")]
        public Task<IHttpActionResult> Entries(DateTime startDate,DateTime endDate,string section,string line,Guid? branchId=null,int pageIndex=0,int pageSize=20)
        { return Respond(() => _service.GetDetails(startDate,endDate,branchId,section,line,pageIndex,pageSize,Utils.CreateServiceHeader())); }
        [HttpGet,Route("mappings")]
        public Task<IHttpActionResult> Mappings() { return Respond(() => _service.GetMappings(Utils.CreateServiceHeader())); }
        [HttpPut,Route("mappings/{accountId:guid}")]
        public Task<IHttpActionResult> Save(Guid accountId,[FromBody] CashFlowMappingDTO mapping)
        { return Respond(async () => { if (mapping==null || mapping.ChartOfAccountId!=accountId) throw new CashFlowValidationException("The selected account does not match the mapping."); await _service.SaveMapping(mapping,Utils.CreateServiceHeader()); return true; }); }
        [HttpDelete,Route("mappings/{accountId:guid}")]
        public Task<IHttpActionResult> Remove(Guid accountId)
        { return Respond(async () => { await _service.RemoveMapping(accountId,Utils.CreateServiceHeader()); return true; }); }
    }
}
