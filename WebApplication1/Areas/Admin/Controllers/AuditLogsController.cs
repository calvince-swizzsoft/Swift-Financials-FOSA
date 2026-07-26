using Application.MainBoundedContext.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace WebApplication1.Areas.Admin.Controllers
{
    [RoutePrefix("api/administration/auditlogs")]
    public class AuditLogsController : ApiController
    {

        private readonly IAuditLogAppService _auditLogAppService;

        public AuditLogsController(IAuditLogAppService auditLogAppService)
        {
            _auditLogAppService = auditLogAppService;
        }

        [HttpGet, Route("")]
        public IHttpActionResult Index()
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            var auditTrails = _auditLogAppService.FindAuditLogs(serviceHeader);


            return Ok(auditTrails);
        }



    }
}