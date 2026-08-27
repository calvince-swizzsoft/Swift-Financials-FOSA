using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Accounts.Controllers
{
    // Adapted from the reference MVC UnPayReasonController (Areas/Accounts). Its
    // attached-commissions Create/Edit flow took a comma-separated SelectedIds string,
    // resolved each id to a full CommissionDTO via a round trip
    // (_channelService.FindCommissionAsync) purely to hand it to
    // UpdateCommissionsByUnPayReasonIdAsync — which only ever reads CommissionDTO.Id
    // (UnPayReasonAppService.UpdateCommissions -> UnPayReasonCommissionFactory.
    // CreateUnPayReasonCommission(persisted.Id, item.Id)). Not ported: this controller
    // takes commission ids directly and builds bare CommissionDTO{ Id = ... } locally,
    // no lookup needed.
    //
    // Also not ported: the reference Edit POST checks unpayReasonDTO.HasErrors without
    // ever calling ValidateAll() first (Create does), so UnPayReasonDTO's [Required] on
    // Description was never actually enforced on edit. PUT /{id} here calls ValidateAll()
    // before checking HasErrors, same as every other controller's Update action.
    //
    // Commissions are a sub-resource (GET/PUT .../commissions) rather than folded into
    // Create/Update, same pattern as LevyController's splits and CommissionController's
    // own graduated-scales/splits/levies sub-resources.
    [Authorize]
    [RoutePrefix("api/accounts/unpayreasons")]
    public class UnPayReasonController : ApiController
    {
        private readonly IUnPayReasonAppService _unPayReasonAppService;

        public UnPayReasonController(IUnPayReasonAppService unPayReasonAppService)
        {
            _unPayReasonAppService = unPayReasonAppService ?? throw new ArgumentNullException(nameof(unPayReasonAppService));
        }

        // Unpaged — for pickers (e.g. ChequesController's clear/unpay flow, which needs
        // a valid UnPayReasonDTO up front). See GET /paged for the admin-screen listing.
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var unPayReasons = _unPayReasonAppService.FindUnPayReasons(serviceHeader);

                return Ok(new { success = true, message = "", data = unPayReasons ?? new List<UnPayReasonDTO>() });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("paged")]
        public async Task<IHttpActionResult> Index(string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = string.IsNullOrWhiteSpace(text)
                    ? _unPayReasonAppService.FindUnPayReasons(pageIndex, pageSize, serviceHeader)
                    : _unPayReasonAppService.FindUnPayReasons(text, pageIndex, pageSize, serviceHeader);

                return Ok(new { success = true, message = "", data = page });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IHttpActionResult> Get(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var unPayReason = _unPayReasonAppService.FindUnPayReason(id, serviceHeader);

                if (unPayReason == null)
                {
                    return NotFound();
                }

                return Ok(new { success = true, message = "", data = unPayReason });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create(CreateUnPayReasonRequest request)
        {
            try
            {
                if (request?.UnPayReason == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid unpay reason data", data = (object)null });
                }

                request.UnPayReason.ValidateAll();

                var serviceHeader = Utils.CreateServiceHeader();

                if (request.UnPayReason.HasErrors)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", request.UnPayReason.ErrorMessages), data = (object)null });
                }

                var createdUnPayReason = _unPayReasonAppService.AddNewUnPayReason(request.UnPayReason, serviceHeader);

                if (createdUnPayReason == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Sorry, but the unpay reason could not be created.", data = (object)null });
                }

                // Duplicate Description is reported by echoing the DTO back with
                // ErrorMessageResult set (Id stays Guid.Empty) — same pattern as
                // CommissionController/CostCenter.
                if (!string.IsNullOrWhiteSpace(createdUnPayReason.ErrorMessageResult))
                {
                    return Content(HttpStatusCode.Conflict, new { success = false, message = createdUnPayReason.ErrorMessageResult, data = (object)null });
                }

                if (request.CommissionIds != null)
                {
                    var commissions = request.CommissionIds.Select(commissionId => new CommissionDTO { Id = commissionId }).ToList();

                    _unPayReasonAppService.UpdateCommissions(createdUnPayReason.Id, commissions, serviceHeader);
                }

                var refreshed = _unPayReasonAppService.FindUnPayReason(createdUnPayReason.Id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Updates only the UnPayReason's own fields. Deliberately does not touch
        // attached commissions — use the sub-resource endpoint below.
        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IHttpActionResult> Update(Guid id, UnPayReasonDTO unPayReasonDTO)
        {
            try
            {
                if (unPayReasonDTO == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid unpay reason data", data = (object)null });
                }

                unPayReasonDTO.Id = id;

                unPayReasonDTO.ValidateAll();

                var serviceHeader = Utils.CreateServiceHeader();

                if (unPayReasonDTO.HasErrors)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", unPayReasonDTO.ErrorMessages), data = (object)null });
                }

                var updated = _unPayReasonAppService.UpdateUnPayReason(unPayReasonDTO, serviceHeader);

                if (!updated)
                {
                    return NotFound();
                }

                var refreshed = _unPayReasonAppService.FindUnPayReason(id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("{id:guid}/commissions")]
        public async Task<IHttpActionResult> GetCommissions(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var commissions = _unPayReasonAppService.FindCommissions(id, serviceHeader);

                return Ok(new { success = true, message = "", data = commissions ?? new List<CommissionDTO>() });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut]
        [Route("{id:guid}/commissions")]
        public async Task<IHttpActionResult> UpdateCommissions(Guid id, List<Guid> commissionIds)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var commissions = (commissionIds ?? new List<Guid>())
                    .Select(commissionId => new CommissionDTO { Id = commissionId })
                    .ToList();

                var updated = _unPayReasonAppService.UpdateCommissions(id, commissions, serviceHeader);

                if (!updated)
                {
                    return NotFound();
                }

                var refreshed = _unPayReasonAppService.FindCommissions(id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed ?? new List<CommissionDTO>() });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class CreateUnPayReasonRequest
    {
        public UnPayReasonDTO UnPayReason { get; set; }
        public List<Guid> CommissionIds { get; set; }
    }
}
