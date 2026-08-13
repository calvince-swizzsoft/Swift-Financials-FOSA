using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Accounts.Controllers
{
    // Generic, channel-type-agnostic adaptation of the reference MVC RegisterController/
    // AlternateChannelsController/AlternatechannelManagementController (all Areas/Accounts).
    // The reference app never builds a controller per AlternateChannelType — Sacco Link,
    // Sparrow, MCo-op Cash, SpotCash, Citius, Agency Banking, PesaPepe, ABC Bank, Broker all
    // flow through these same generic controllers, keyed off AlternateChannelDTO.Type. This
    // is "Piece A" from WebApplication1/Areas/WhatsAppBanking/WORKFLOW.md §4 — built first
    // and generically so WhatsApp Banking (once AlternateChannelType.WhatsAppBanking exists)
    // is just one more value in Type, not a reason to duplicate this controller.
    //
    // Real bugs found in the reference RegisterController while reading it, not ported:
    // - Verify/Authorize are bound to DebitBatchDTO and call AuditDebitBatchAsync/
    //   AuthorizeDebitBatchAsync — copy-pasted from a DebitBatch controller, nothing to do
    //   with AlternateChannel at all. GetDebitBatchesAsync is the same kind of leftover.
    // - History's POST action is byte-for-byte identical to Linking's POST (calls
    //   AddAlternateChannelAsync again) — it does not fetch history, despite the name.
    // - Create(Guid id, ...)/Search/Linking(Guid id)/History(Guid id) (the GET overloads)
    //   only ever pre-fill a create form from an existing CustomerAccount lookup — pure MVC
    //   view-staging, not a distinct operation. A JSON API client can call the existing
    //   CustomerAccount endpoints itself before submitting Create; not reproduced here, same
    //   reasoning as LoanProductController's GetInvestmentProductDetails/GetSavingDetails.
    //
    // IMPORTANT correction to WhatsAppBanking WORKFLOW.md's assumption: there is no real
    // maker-checker gate for AlternateChannel anywhere in this codebase. AddNewAlternateChannel
    // never sets RecordStatus (defaults to byte 0 = New); UpdateAlternateChannel copies
    // whatever RecordStatus the caller supplies straight onto the persisted record, with no
    // check that it's currently New/Edited, no distinct maker-vs-checker identity check —
    // unlike e.g. the Batch Procedures module's real Audited/Authorized guard clauses. The
    // Approve/Reject actions below are a thin convenience over that same ungated Update call,
    // not a new enforcement layer this controller invents. If real maker-checker enforcement
    // is wanted for channel linking, it does not exist yet at any layer — flagged, not solved.
    //
    // Also flagged, not fixed: AlternateChannelDTO.CheckAlternateChannelNumber (the CardNumber
    // validator) has real dead ends for AlternateChannelType.AgencyBanking and .Citius — both
    // fall through to a branch that unconditionally blanks CardNumber, so Create/Update can
    // never succeed for those two channel types today. No spec anywhere states the intended
    // CardNumber format for either, so guessing one here would be worse than leaving it
    // flagged. Whoever adds AlternateChannelType.WhatsAppBanking will hit the same `default:`
    // branch and needs a real case added there, not just the enum value.
    [Authorize]
    [RoutePrefix("api/accounts/alternatechannels")]
    public class AlternateChannelController : ApiController
    {
        private readonly IAlternateChannelAppService _alternateChannelAppService;

        public AlternateChannelController(IAlternateChannelAppService alternateChannelAppService)
        {
            _alternateChannelAppService = alternateChannelAppService ?? throw new ArgumentNullException(nameof(alternateChannelAppService));
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var alternateChannels = _alternateChannelAppService.FindAlternateChannels(serviceHeader);

                return Ok(new { success = true, message = "", data = alternateChannels ?? new List<AlternateChannelDTO>() });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("paged")]
        public async Task<IHttpActionResult> GetPaged(string text = "", int filter = 0, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = string.IsNullOrWhiteSpace(text)
                    ? _alternateChannelAppService.FindAlternateChannels(pageIndex, pageSize, serviceHeader)
                    : _alternateChannelAppService.FindAlternateChannels(text, filter, pageIndex, pageSize, serviceHeader);

                return Ok(new { success = true, message = "", data = page });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // The exact-match filter behind the checker-inbox use case (e.g. "every New
        // WhatsApp Banking link waiting for approval" = type=512, recordStatus=0) — both
        // type and recordStatus are matched exactly, there is no "any type"/"any status"
        // sentinel at the specification layer, so both are required route segments here
        // rather than optional/defaulted query params.
        [HttpGet]
        [Route("paged/type/{type:int}/status/{recordStatus:int}")]
        public async Task<IHttpActionResult> GetPagedByTypeAndStatus(int type, int recordStatus, string text = "", int filter = 0, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = _alternateChannelAppService.FindAlternateChannels(type, recordStatus, text ?? string.Empty, filter, pageIndex, pageSize, serviceHeader);

                return Ok(new { success = true, message = "", data = page });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Approved, unlocked, not-yet-third-party-notified links of a given type — the
        // queue an outbound notifier (e.g. informing a telco a number is now linked) would
        // page through. No consumer of this exists yet in this codebase (see WORKFLOW.md
        // §3/§9 on the outbound B2C host being unimplemented) — exposed because the
        // filtering logic is real and already built, not because something calls it today.
        [HttpGet]
        [Route("third-party-notifiable/type/{type:int}")]
        public async Task<IHttpActionResult> GetThirdPartyNotifiable(int type, string text = "", int filter = 0, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = _alternateChannelAppService.FindThirdPartyNotifiableAlternateChannels(type, text ?? string.Empty, filter, pageIndex, pageSize, serviceHeader);

                return Ok(new { success = true, message = "", data = page });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IHttpActionResult> Get(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var alternateChannel = _alternateChannelAppService.FindAlternateChannel(id, serviceHeader);

                if (alternateChannel == null)
                {
                    return NotFound();
                }

                return Ok(new { success = true, message = "", data = alternateChannel });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("by-customer-account/{customerAccountId:guid}")]
        public async Task<IHttpActionResult> GetByCustomerAccount(Guid customerAccountId)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var alternateChannels = _alternateChannelAppService.FindAlternateChannelsByCustomerAccountId(customerAccountId, serviceHeader);

                return Ok(new { success = true, message = "", data = alternateChannels ?? new List<AlternateChannelDTO>() });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("by-customer/{customerId:guid}")]
        public async Task<IHttpActionResult> GetByCustomer(Guid customerId)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var alternateChannels = _alternateChannelAppService.FindAlternateChannelsByCustomerId(customerId, serviceHeader);

                return Ok(new { success = true, message = "", data = alternateChannels ?? new List<AlternateChannelDTO>() });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // cardNumber is a query param, not a route segment — MSISDN-shaped values (e.g.
        // "+254712345678") don't round-trip cleanly through URL path segments. This is
        // exactly the lookup MobileToBankRequestAppService's C2B matching uses internally
        // (FindAlternateChannelsByCardNumber) — exposed here for the same reason as
        // third-party-notifiable above: real, already-built, no controller reached it before.
        [HttpGet]
        [Route("by-card-number")]
        public async Task<IHttpActionResult> GetByCardNumber(string cardNumber, int? type = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cardNumber))
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "cardNumber is required", data = (object)null });
                }

                var serviceHeader = Utils.CreateServiceHeader();

                var alternateChannels = type.HasValue
                    ? _alternateChannelAppService.FindAlternateChannelsByCardNumberAndCardType(cardNumber, type.Value, serviceHeader)
                    : _alternateChannelAppService.FindAlternateChannelsByCardNumber(cardNumber, serviceHeader);

                return Ok(new { success = true, message = "", data = alternateChannels ?? new List<AlternateChannelDTO>() });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Duplicate CardNumber+Type is reported by echoing the DTO back with
        // ErrorMessageResult set (Id stays Guid.Empty) — same pattern as Commission/CostCenter.
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create(AlternateChannelDTO alternateChannelDTO)
        {
            try
            {
                if (alternateChannelDTO == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid alternate channel data", data = (object)null });
                }

                alternateChannelDTO.ValidateAll();

                if (alternateChannelDTO.HasErrors)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", alternateChannelDTO.ErrorMessages), data = (object)null });
                }

                var serviceHeader = Utils.CreateServiceHeader();

                var created = _alternateChannelAppService.AddNewAlternateChannel(alternateChannelDTO, serviceHeader);

                if (created == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Sorry, but the alternate channel could not be linked.", data = (object)null });
                }

                if (!string.IsNullOrWhiteSpace(created.ErrorMessageResult))
                {
                    return Content(HttpStatusCode.Conflict, new { success = false, message = created.ErrorMessageResult, data = (object)null });
                }

                return Ok(new { success = true, message = "Operation Success", data = created });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Generic field update — CardNumber/Remarks/DailyLimit/ThirdPartyNotified(+Response)/
        // IsLocked/RecordStatus. This is also, today, the only way anything moves RecordStatus
        // forward (see the controller header comment) — there is no separate gated approval
        // call underneath. Use POST {id}/approve or {id}/reject for the common case instead of
        // hand-building a full DTO just to flip RecordStatus.
        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IHttpActionResult> Update(Guid id, AlternateChannelDTO alternateChannelDTO)
        {
            try
            {
                if (alternateChannelDTO == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid alternate channel data", data = (object)null });
                }

                alternateChannelDTO.Id = id;
                alternateChannelDTO.ValidateAll();

                if (alternateChannelDTO.HasErrors)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", alternateChannelDTO.ErrorMessages), data = (object)null });
                }

                var serviceHeader = Utils.CreateServiceHeader();

                var updated = _alternateChannelAppService.UpdateAlternateChannel(alternateChannelDTO, serviceHeader);

                if (!updated)
                {
                    if (!string.IsNullOrWhiteSpace(alternateChannelDTO.ErrorMessageResult))
                    {
                        return Content(HttpStatusCode.Conflict, new { success = false, message = alternateChannelDTO.ErrorMessageResult, data = (object)null });
                    }

                    return NotFound();
                }

                var refreshed = _alternateChannelAppService.FindAlternateChannel(id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Card/number replacement (lost/stolen SIM, new card, ...) — sets RecordStatus back
        // to Edited and logs a Channel Replacement CustomerAccountHistory entry, unlike the
        // plain Update above. CustomerAccountId in the body must match the persisted record's
        // — a mismatch is indistinguishable from "not found" at the app-service layer, so
        // both map to 404 here.
        [HttpPost]
        [Route("{id:guid}/replace")]
        public async Task<IHttpActionResult> Replace(Guid id, AlternateChannelDTO alternateChannelDTO)
        {
            try
            {
                if (alternateChannelDTO == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid alternate channel data", data = (object)null });
                }

                alternateChannelDTO.Id = id;
                alternateChannelDTO.ValidateAll();

                if (alternateChannelDTO.HasErrors)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", alternateChannelDTO.ErrorMessages), data = (object)null });
                }

                var serviceHeader = Utils.CreateServiceHeader();

                var replaced = _alternateChannelAppService.ReplaceAlternateChannel(alternateChannelDTO, serviceHeader);

                if (!replaced)
                {
                    if (!string.IsNullOrWhiteSpace(alternateChannelDTO.ErrorMessageResult))
                    {
                        return Content(HttpStatusCode.Conflict, new { success = false, message = alternateChannelDTO.ErrorMessageResult, data = (object)null });
                    }

                    return NotFound();
                }

                var refreshed = _alternateChannelAppService.FindAlternateChannel(id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Same shape/preconditions as Replace — logs a Channel Renewal history entry instead.
        [HttpPost]
        [Route("{id:guid}/renew")]
        public async Task<IHttpActionResult> Renew(Guid id, AlternateChannelDTO alternateChannelDTO)
        {
            try
            {
                if (alternateChannelDTO == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid alternate channel data", data = (object)null });
                }

                alternateChannelDTO.Id = id;
                alternateChannelDTO.ValidateAll();

                if (alternateChannelDTO.HasErrors)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", alternateChannelDTO.ErrorMessages), data = (object)null });
                }

                var serviceHeader = Utils.CreateServiceHeader();

                var renewed = _alternateChannelAppService.RenewAlternateChannel(alternateChannelDTO, serviceHeader);

                if (!renewed)
                {
                    if (!string.IsNullOrWhiteSpace(alternateChannelDTO.ErrorMessageResult))
                    {
                        return Content(HttpStatusCode.Conflict, new { success = false, message = alternateChannelDTO.ErrorMessageResult, data = (object)null });
                    }

                    return NotFound();
                }

                var refreshed = _alternateChannelAppService.FindAlternateChannel(id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Locks the channel (suspends transacting) and logs a Channel Stoppage history entry
        // — distinct from Delink below, which removes the link entirely. Only CustomerAccountId
        // and Remarks on the body matter; everything else on the persisted record is untouched.
        [HttpPost]
        [Route("{id:guid}/stop")]
        public async Task<IHttpActionResult> Stop(Guid id, AlternateChannelDTO alternateChannelDTO)
        {
            try
            {
                if (alternateChannelDTO == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid alternate channel data", data = (object)null });
                }

                alternateChannelDTO.Id = id;

                var serviceHeader = Utils.CreateServiceHeader();

                var stopped = _alternateChannelAppService.StopAlternateChannel(alternateChannelDTO, serviceHeader);

                if (!stopped)
                {
                    return NotFound();
                }

                var refreshed = _alternateChannelAppService.FindAlternateChannel(id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Hard-deletes the AlternateChannel row (after logging a Channel Delinking history
        // entry) — POST, not DELETE, because CustomerAccountId and Remarks on the body drive
        // the history log entry and aren't recoverable from the route id alone.
        [HttpPost]
        [Route("{id:guid}/delink")]
        public async Task<IHttpActionResult> Delink(Guid id, AlternateChannelDTO alternateChannelDTO)
        {
            try
            {
                if (alternateChannelDTO == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid alternate channel data", data = (object)null });
                }

                alternateChannelDTO.Id = id;

                var serviceHeader = Utils.CreateServiceHeader();

                var delinked = _alternateChannelAppService.DelinkAlternateChannel(alternateChannelDTO, serviceHeader);

                if (!delinked)
                {
                    return NotFound();
                }

                return Ok(new { success = true, message = "Operation Success", data = (object)null });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Convenience wrapper over Update — fetches the current record, flips RecordStatus to
        // Approved (optionally overwriting Remarks), and saves. NOT a maker-checker gate: see
        // the controller header comment. Nothing here checks the record is currently New/
        // Edited, or that the approver differs from whoever created/last edited it.
        [HttpPost]
        [Route("{id:guid}/approve")]
        public async Task<IHttpActionResult> Approve(Guid id, RecordStatusChangeRequest request)
        {
            return await SetRecordStatus(id, (int)RecordStatus.Approved, request?.Remarks);
        }

        // Same as Approve, RecordStatus.Rejected instead.
        [HttpPost]
        [Route("{id:guid}/reject")]
        public async Task<IHttpActionResult> Reject(Guid id, RecordStatusChangeRequest request)
        {
            return await SetRecordStatus(id, (int)RecordStatus.Rejected, request?.Remarks);
        }

        private async Task<IHttpActionResult> SetRecordStatus(Guid id, int recordStatus, string remarks)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var persisted = _alternateChannelAppService.FindAlternateChannel(id, serviceHeader);

                if (persisted == null)
                {
                    return NotFound();
                }

                persisted.RecordStatus = recordStatus;

                if (remarks != null)
                {
                    persisted.Remarks = remarks;
                }

                var updated = _alternateChannelAppService.UpdateAlternateChannel(persisted, serviceHeader);

                if (!updated)
                {
                    return Content(HttpStatusCode.Conflict, new { success = false, message = persisted.ErrorMessageResult ?? "Unable to update the alternate channel's record status.", data = (object)null });
                }

                var refreshed = _alternateChannelAppService.FindAlternateChannel(id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Fees are scoped by channel TYPE, not by individual link — every WhatsApp Banking
        // link (say) shares the same DepositCharges commission. AlternateChannelKnownChargeType
        // (Infrastructure.Crosscutting.Framework.Utils) enumerates the fee hooks: Linking,
        // Replacement, Renewal, WithdrawalCharges, DepositCharges, MiniStatementCharges,
        // BalanceInquiryCharges, AirtimeCharges, PINResetCharges, ... — required, not defaulted,
        // same reasoning as LoanProductController's commissions sub-resource.
        [HttpGet]
        [Route("types/{type:int}/commissions")]
        public async Task<IHttpActionResult> GetCommissions(int type, int knownChargeType)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var commissions = _alternateChannelAppService.FindCommissions(type, knownChargeType, serviceHeader);

                return Ok(new { success = true, message = "", data = commissions ?? new List<CommissionDTO>() });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Full replace of which Commission records apply for this type+knownChargeType — only
        // each CommissionDTO.Id is read (same join-table pattern as LoanProductController's
        // commissions sub-resource). chargeBenefactor (Infrastructure.Crosscutting.Framework.
        // Utils.ChargeBenefactor — Customer vs. Institution) applies to the whole batch.
        [HttpPut]
        [Route("types/{type:int}/commissions")]
        public async Task<IHttpActionResult> UpdateCommissions(int type, UpdateAlternateChannelCommissionsRequest request)
        {
            try
            {
                if (request == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid commissions data", data = (object)null });
                }

                var serviceHeader = Utils.CreateServiceHeader();

                var updated = _alternateChannelAppService.UpdateCommissions(type, request.Commissions ?? new List<CommissionDTO>(), request.KnownChargeType, request.ChargeBenefactor, serviceHeader);

                if (!updated)
                {
                    return NotFound();
                }

                var refreshed = _alternateChannelAppService.FindCommissions(type, request.KnownChargeType, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed ?? new List<CommissionDTO>() });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }

    public class RecordStatusChangeRequest
    {
        public string Remarks { get; set; }
    }

    public class UpdateAlternateChannelCommissionsRequest
    {
        public int KnownChargeType { get; set; }
        public int ChargeBenefactor { get; set; }
        public List<CommissionDTO> Commissions { get; set; }
    }
}
