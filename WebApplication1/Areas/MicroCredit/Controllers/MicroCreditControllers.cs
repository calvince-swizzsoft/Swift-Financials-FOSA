using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO.MicroCreditModule;
using Application.MainBoundedContext.MicroCreditModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.MicroCredit.Controllers
{
    [Authorize, RoutePrefix("api/microcredit/officers")]
    public class MicroCreditOfficersController : ApiController
    {
        readonly IMicroCreditOfficerAppService service; readonly INavigationItemInRoleAppService permissions;
        public MicroCreditOfficersController(IMicroCreditOfficerAppService service, INavigationItemInRoleAppService permissions) { this.service = service; this.permissions = permissions; }
        bool Allowed(ServiceHeader h) { var grants = permissions.GetRolesForNavigationItemCode(31003, h) ?? new string[0]; return (h.ApplicationUserRoles ?? new List<string>()).Any(r => grants.Any(g => string.Equals(r, g, StringComparison.OrdinalIgnoreCase))); }
        [HttpGet, Route("")] public IHttpActionResult List(string text = "", int pageIndex = 0, int pageSize = 20) { var h = Utils.CreateServiceHeader(); if (!Allowed(h)) return StatusCode(HttpStatusCode.Forbidden); return Ok(service.FindMicroCreditOfficers(text ?? "", pageIndex, pageSize, h)); }
        [HttpPost, Route("")] public IHttpActionResult Create(MicroCreditOfficerDTO dto) { var h = Utils.CreateServiceHeader(); if (!Allowed(h)) return StatusCode(HttpStatusCode.Forbidden); if (dto == null || dto.EmployeeId == Guid.Empty) return BadRequest("Select an employee."); var result = service.AddNewMicroCreditOfficer(dto, h); if (!string.IsNullOrWhiteSpace(result?.errormassage)) return BadRequest(result.errormassage); return Ok(result); }
        [HttpPut, Route("{id:guid}")] public IHttpActionResult Update(Guid id, MicroCreditOfficerDTO dto) { var h = Utils.CreateServiceHeader(); if (!Allowed(h)) return StatusCode(HttpStatusCode.Forbidden); if (dto == null) return BadRequest("Officer details are required."); dto.Id = id; return service.UpdateMicroCreditOfficer(dto, h) ? Ok(dto) : (IHttpActionResult)NotFound(); }
    }

    [Authorize, RoutePrefix("api/microcredit/groups")]
    public class MicroCreditGroupsController : ApiController
    {
        readonly IMicroCreditGroupAppService service; readonly INavigationItemInRoleAppService permissions;
        public MicroCreditGroupsController(IMicroCreditGroupAppService service, INavigationItemInRoleAppService permissions) { this.service = service; this.permissions = permissions; }
        bool Allowed(ServiceHeader h) { var grants = permissions.GetRolesForNavigationItemCode(31004, h) ?? new string[0]; return (h.ApplicationUserRoles ?? new List<string>()).Any(r => grants.Any(g => string.Equals(r, g, StringComparison.OrdinalIgnoreCase))); }
        [HttpGet, Route("")] public IHttpActionResult List(string text = "", int pageIndex = 0, int pageSize = 20) { var h = Utils.CreateServiceHeader(); if (!Allowed(h)) return StatusCode(HttpStatusCode.Forbidden); return Ok(service.FindMicroCreditGroups(text ?? "", pageIndex, pageSize, h)); }
        [HttpGet, Route("{id:guid}/members")] public IHttpActionResult Members(Guid id) { var h = Utils.CreateServiceHeader(); if (!Allowed(h)) return StatusCode(HttpStatusCode.Forbidden); return Ok(service.FindMicroCreditGroupMembers(id, h) ?? new List<MicroCreditGroupMemberDTO>()); }
        [HttpPost, Route("")] public IHttpActionResult Create(MicroCreditGroupDTO dto) { var h = Utils.CreateServiceHeader(); if (!Allowed(h)) return StatusCode(HttpStatusCode.Forbidden); if (dto == null || dto.CustomerId == Guid.Empty || dto.MicroCreditOfficerId == Guid.Empty) return BadRequest("Group customer and credit officer are required."); return Ok(service.AddNewMicroCreditGroup(dto, h)); }
        [HttpPut, Route("{id:guid}")] public IHttpActionResult Update(Guid id, MicroCreditGroupDTO dto) { var h = Utils.CreateServiceHeader(); if (!Allowed(h)) return StatusCode(HttpStatusCode.Forbidden); if (dto == null) return BadRequest("Group details are required."); dto.Id = id; return service.UpdateMicroCreditGroup(dto, h) ? Ok(dto) : (IHttpActionResult)NotFound(); }
        [HttpPost, Route("{id:guid}/members")] public IHttpActionResult AddMember(Guid id, MicroCreditGroupMemberDTO dto) { var h = Utils.CreateServiceHeader(); if (!Allowed(h)) return StatusCode(HttpStatusCode.Forbidden); if (dto == null || dto.CustomerId == Guid.Empty) return BadRequest("Select a customer."); dto.MicroCreditGroupId = id; var result = service.AddNewMicroCreditGroupMember(dto, h); if (!string.IsNullOrWhiteSpace(result?.errorMsg)) return BadRequest(result.errorMsg); return Ok(result); }
        [HttpPut, Route("{id:guid}/members")] public IHttpActionResult ReplaceMembers(Guid id, List<MicroCreditGroupMemberDTO> members) { var h = Utils.CreateServiceHeader(); if (!Allowed(h)) return StatusCode(HttpStatusCode.Forbidden); return Ok(service.UpdateMicroCreditGroupMemberCollection(id, members ?? new List<MicroCreditGroupMemberDTO>(), h)); }
    }
}
