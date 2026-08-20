using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.BackOffice.Controllers
{
    [Authorize]
    [RoutePrefix("api/backoffice/checkoff-data-capture")]
    public class CheckOffDataCaptureController : ApiController
    {
        private readonly IDataAttachmentPeriodAppService _periods;
        private readonly IPostingPeriodAppService _postingPeriods;
        private readonly IAuthorizationAppService _authorization;
        private readonly ICustomerAccountAppService _customerAccounts;

        public CheckOffDataCaptureController(IDataAttachmentPeriodAppService periods, IPostingPeriodAppService postingPeriods, IAuthorizationAppService authorization, ICustomerAccountAppService customerAccounts)
        {
            _periods = periods ?? throw new ArgumentNullException(nameof(periods));
            _postingPeriods = postingPeriods ?? throw new ArgumentNullException(nameof(postingPeriods));
            _authorization = authorization ?? throw new ArgumentNullException(nameof(authorization));
            _customerAccounts = customerAccounts ?? throw new ArgumentNullException(nameof(customerAccounts));
        }

        [HttpGet, Route("periods")]
        public IHttpActionResult Periods([FromUri] string text = "", [FromUri] int pageIndex = 0, [FromUri] int pageSize = 20)
        {
            var denied = RequireAny(SystemPermissionType.BackOfficeCheckOffPeriodManagement, SystemPermissionType.BackOfficeCheckOffDataCapture, SystemPermissionType.BackOfficeCheckOffPeriodClosing, SystemPermissionType.BackOfficeCheckOffCatalogueViewing); if (denied != null) return denied;
            if (!ValidPage(pageIndex, pageSize)) return BadRequest("Invalid paging values.");
            var header = Utils.CreateServiceHeader();
            var page = string.IsNullOrWhiteSpace(text) ? _periods.FindDataAttachmentPeriods(pageIndex, pageSize, header) : _periods.FindDataAttachmentPeriods(text.Trim(), pageIndex, pageSize, header);
            return Success("Data periods retrieved.", page);
        }

        [HttpGet, Route("periods/current")]
        public IHttpActionResult Current()
        {
            var denied = RequireAny(SystemPermissionType.BackOfficeCheckOffDataCapture); if (denied != null) return denied;
            return Success("Current data period retrieved.", _periods.FindCurrentDataAttachmentPeriod(Utils.CreateServiceHeader()));
        }

        [HttpGet, Route("periods/{id:guid}")]
        public IHttpActionResult Period(Guid id)
        {
            var denied = RequireAny(SystemPermissionType.BackOfficeCheckOffPeriodManagement, SystemPermissionType.BackOfficeCheckOffDataCapture, SystemPermissionType.BackOfficeCheckOffPeriodClosing, SystemPermissionType.BackOfficeCheckOffCatalogueViewing); if (denied != null) return denied;
            var item = _periods.FindDataAttachmentPeriod(id, Utils.CreateServiceHeader());
            return item == null ? Error(HttpStatusCode.NotFound, "Data period not found.") : Success("Data period retrieved.", item);
        }

        [HttpGet, Route("posting-periods")]
        public IHttpActionResult PostingPeriods()
        {
            var denied = RequireAny(SystemPermissionType.BackOfficeCheckOffPeriodManagement); if (denied != null) return denied;
            var items = (_postingPeriods.FindPostingPeriods(Utils.CreateServiceHeader()) ?? new List<Application.MainBoundedContext.DTO.AccountsModule.PostingPeriodDTO>()).Where(x => x.IsActive).ToList();
            return Success("Active posting periods retrieved.", items);
        }

        [HttpPost, Route("periods")]
        public IHttpActionResult CreatePeriod(DataAttachmentPeriodDTO request)
        {
            var denied = RequireAny(SystemPermissionType.BackOfficeCheckOffPeriodManagement); if (denied != null) return denied;
            if (request == null || request.PostingPeriodId == Guid.Empty || request.Month < 1 || request.Month > 12 || string.IsNullOrWhiteSpace(request.Remarks)) return BadRequest("Posting period, month, and remarks are required.");
            var posting = _postingPeriods.FindPostingPeriod(request.PostingPeriodId, Utils.CreateServiceHeader());
            if (posting == null || !posting.IsActive) return BadRequest("Select an active posting period.");
            var created = _periods.AddNewDataAttachmentPeriod(request, Utils.CreateServiceHeader());
            if (!string.IsNullOrWhiteSpace(created?.ErrorMessageResult)) return Error(HttpStatusCode.Conflict, created.ErrorMessageResult);
            if (created != null && request.IsActive) { created.IsActive = true; _periods.UpdateDataAttachmentPeriod(created, Utils.CreateServiceHeader()); created = _periods.FindDataAttachmentPeriod(created.Id, Utils.CreateServiceHeader()); }
            return Content(HttpStatusCode.Created, new { success = true, message = "Data period opened.", data = created });
        }

        [HttpPut, Route("periods/{id:guid}")]
        public IHttpActionResult UpdatePeriod(Guid id, DataAttachmentPeriodDTO request)
        {
            var denied = RequireAny(SystemPermissionType.BackOfficeCheckOffPeriodManagement); if (denied != null) return denied;
            var persisted = _periods.FindDataAttachmentPeriod(id, Utils.CreateServiceHeader());
            if (persisted == null) return Error(HttpStatusCode.NotFound, "Data period not found.");
            if (persisted.Status != (int)DataAttachmentPeriodStatus.Open) return Error(HttpStatusCode.Conflict, "Only an open data period can be edited.");
            if (request == null || request.Month < 1 || request.Month > 12 || string.IsNullOrWhiteSpace(request.Remarks)) return BadRequest("Month and remarks are required.");
            request.Id = id; request.PostingPeriodId = persisted.PostingPeriodId; request.Status = persisted.Status;
            return _periods.UpdateDataAttachmentPeriod(request, Utils.CreateServiceHeader()) ? Success("Data period updated.", request) : Error(HttpStatusCode.Conflict, request.ErrorMessageResult ?? "Data period could not be updated.");
        }

        [HttpPost, Route("periods/{id:guid}/close")]
        public IHttpActionResult Close(Guid id, ClosePeriodRequest request)
        {
            var denied = RequireAny(SystemPermissionType.BackOfficeCheckOffPeriodClosing); if (denied != null) return denied;
            var persisted = _periods.FindDataAttachmentPeriod(id, Utils.CreateServiceHeader());
            if (persisted == null) return Error(HttpStatusCode.NotFound, "Data period not found.");
            if (persisted.Status != (int)DataAttachmentPeriodStatus.Open) return Error(HttpStatusCode.Conflict, "Only an open data period can be closed.");
            if (string.IsNullOrWhiteSpace(request?.Remarks)) return BadRequest("Closing remarks are required.");
            persisted.AuthorizationRemarks = request.Remarks.Trim();
            return _periods.CloseDataAttachmentPeriod(persisted, Utils.CreateServiceHeader()) ? Success("Data period closed.", persisted) : Error(HttpStatusCode.Conflict, "Data period could not be closed.");
        }

        [HttpGet, Route("periods/{id:guid}/entries")]
        public IHttpActionResult Entries(Guid id, [FromUri] string text = "", [FromUri] int pageIndex = 0, [FromUri] int pageSize = 20)
        {
            var denied = RequireAny(SystemPermissionType.BackOfficeCheckOffDataCapture, SystemPermissionType.BackOfficeCheckOffPeriodClosing, SystemPermissionType.BackOfficeCheckOffCatalogueViewing); if (denied != null) return denied;
            if (!ValidPage(pageIndex, pageSize)) return BadRequest("Invalid paging values.");
            if (_periods.FindDataAttachmentPeriod(id, Utils.CreateServiceHeader()) == null) return Error(HttpStatusCode.NotFound, "Data period not found.");
            return Success("Data entries retrieved.", _periods.FindDataAttachmentEntriesByDataAttachmentPeriodId(id, text ?? "", pageIndex, pageSize, Utils.CreateServiceHeader()));
        }

        [HttpPost, Route("periods/{id:guid}/entries")]
        public IHttpActionResult AddEntry(Guid id, DataAttachmentEntryDTO request)
        {
            var denied = RequireAny(SystemPermissionType.BackOfficeCheckOffDataCapture); if (denied != null) return denied;
            var period = _periods.FindDataAttachmentPeriod(id, Utils.CreateServiceHeader());
            if (period == null) return Error(HttpStatusCode.NotFound, "Data period not found.");
            if (period.Status != (int)DataAttachmentPeriodStatus.Open) return Error(HttpStatusCode.Conflict, "Entries can only be added to an open period.");
            if (request == null || request.CustomerAccountId == Guid.Empty || !Enum.IsDefined(typeof(DataAttachmentTransactionType), request.TransactionType)) return BadRequest("Customer account and a valid transaction type are required.");
            var existing = _periods.FindDataAttachmentEntriesByDataAttachmentPeriodIdAndCustomerAccountId(id, request.CustomerAccountId, Utils.CreateServiceHeader()) ?? new List<DataAttachmentEntryDTO>();
            request.DataAttachmentPeriodId = id;
            request.SequenceNumber = existing.Where(x => x.TransactionType == request.TransactionType).Select(x => x.SequenceNumber).DefaultIfEmpty(0).Max() + 1;
            var created = _periods.AddNewDataAttachmentEntry(request, Utils.CreateServiceHeader());
            return Content(HttpStatusCode.Created, new { success = true, message = "Checkoff entry captured.", data = created });
        }

        [HttpPost, Route("periods/{id:guid}/entries/import")]
        public async Task<IHttpActionResult> ImportEntries(Guid id)
        {
            var denied = RequireAny(SystemPermissionType.BackOfficeCheckOffDataCapture); if (denied != null) return denied;
            var period = _periods.FindDataAttachmentPeriod(id, Utils.CreateServiceHeader());
            if (period == null) return Error(HttpStatusCode.NotFound, "Data period not found.");
            if (period.Status != (int)DataAttachmentPeriodStatus.Open) return Error(HttpStatusCode.Conflict, "Entries can only be imported into an open period.");
            if (!Request.Content.IsMimeMultipartContent()) return Error(HttpStatusCode.UnsupportedMediaType, "Expected multipart/form-data content with a 'file' part.");

            var header = Utils.CreateServiceHeader();
            var settings = ConfigurationHelper.GetServiceBrokerConfigurationSettings(header);
            if (settings == null || string.IsNullOrWhiteSpace(settings.FileUploadDirectory)) return Error(HttpStatusCode.InternalServerError, "File upload directory is not configured (serviceBrokerConfiguration).");

            Directory.CreateDirectory(settings.FileUploadDirectory);
            var provider = new MultipartFormDataStreamProvider(settings.FileUploadDirectory);
            await Request.Content.ReadAsMultipartAsync(provider);

            var uploaded = provider.FileData.FirstOrDefault();
            if (uploaded == null) return Error(HttpStatusCode.BadRequest, "No file was uploaded.");

            var suppliedName = uploaded.Headers?.ContentDisposition?.FileName?.Trim('"');
            var originalFileName = Path.GetFileName(suppliedName ?? "checkoff-import.csv");
            if (!string.Equals(Path.GetExtension(originalFileName), ".csv", StringComparison.OrdinalIgnoreCase))
            {
                File.Delete(uploaded.LocalFileName);
                return Error(HttpStatusCode.BadRequest, "Checkoff imports must be CSV files.");
            }

            string[] lines;
            try { lines = File.ReadAllLines(uploaded.LocalFileName); }
            finally { File.Delete(uploaded.LocalFileName); }

            var dataLines = lines.Where(l => !string.IsNullOrWhiteSpace(l)).ToList();
            if (dataLines.Count < 2) return Error(HttpStatusCode.BadRequest, "The file contained no data rows.");

            var columns = SplitCsvLine(dataLines[0]).Select(c => c.Trim().ToLowerInvariant()).ToList();
            var accountNumberIdx = columns.IndexOf("accountnumber");
            var transactionTypeIdx = columns.IndexOf("transactiontype");
            var newAmountIdx = columns.IndexOf("newamount");
            var currentAmountIdx = columns.IndexOf("currentamount");
            var newBalanceIdx = columns.IndexOf("newbalance");
            var currentBalanceIdx = columns.IndexOf("currentbalance");
            var newAbilityIdx = columns.IndexOf("newability");
            var currentAbilityIdx = columns.IndexOf("currentability");
            var remarksIdx = columns.IndexOf("remarks");
            if (accountNumberIdx < 0 || transactionTypeIdx < 0) return Error(HttpStatusCode.BadRequest, "The file must include AccountNumber and TransactionType columns.");

            var descriptionLookup = Enum.GetValues(typeof(DataAttachmentTransactionType)).Cast<DataAttachmentTransactionType>()
                .ToDictionary(t => EnumHelper.GetDescription(t), t => (int)t, StringComparer.OrdinalIgnoreCase);
            var existing = _periods.FindDataAttachmentEntriesByDataAttachmentPeriodId(id, "", 0, int.MaxValue, header)?.PageCollection ?? new List<DataAttachmentEntryDTO>();
            var sequenceTracker = existing.GroupBy(e => new { e.CustomerAccountId, e.TransactionType }).ToDictionary(g => g.Key, g => g.Max(e => e.SequenceNumber));

            var imported = 0;
            var errors = new List<object>();

            for (var rowIndex = 1; rowIndex < dataLines.Count; rowIndex++)
            {
                var fields = SplitCsvLine(dataLines[rowIndex]);
                var rowNumber = rowIndex + 1;

                var accountNumber = CsvField(fields, accountNumberIdx);
                if (string.IsNullOrWhiteSpace(accountNumber)) { errors.Add(new { Row = rowNumber, Error = "Missing account number." }); continue; }

                var account = _customerAccounts.FindCustomerAccountDTO(accountNumber, header);
                if (account == null) { errors.Add(new { Row = rowNumber, AccountNumber = accountNumber, Error = "Account number not found." }); continue; }

                var typeText = CsvField(fields, transactionTypeIdx);
                if (!int.TryParse(typeText, out var transactionType) || !Enum.IsDefined(typeof(DataAttachmentTransactionType), transactionType))
                {
                    if (!descriptionLookup.TryGetValue(typeText, out transactionType))
                    {
                        errors.Add(new { Row = rowNumber, AccountNumber = accountNumber, Error = string.Format("Unrecognised transaction type '{0}'.", typeText) });
                        continue;
                    }
                }

                if (!TryParseAmount(fields, newAmountIdx, out var newAmount) || !TryParseAmount(fields, currentAmountIdx, out var currentAmount) ||
                    !TryParseAmount(fields, newBalanceIdx, out var newBalance) || !TryParseAmount(fields, currentBalanceIdx, out var currentBalance) ||
                    !TryParseAmount(fields, newAbilityIdx, out var newAbility) || !TryParseAmount(fields, currentAbilityIdx, out var currentAbility))
                {
                    errors.Add(new { Row = rowNumber, AccountNumber = accountNumber, Error = "One or more amount, balance, or ability values are not valid numbers." });
                    continue;
                }

                var key = new { CustomerAccountId = account.Id, TransactionType = transactionType };
                var nextSequence = (sequenceTracker.TryGetValue(key, out var currentMax) ? currentMax : 0) + 1;
                sequenceTracker[key] = nextSequence;

                _periods.AddNewDataAttachmentEntry(new DataAttachmentEntryDTO
                {
                    DataAttachmentPeriodId = id,
                    CustomerAccountId = account.Id,
                    TransactionType = transactionType,
                    SequenceNumber = nextSequence,
                    NewAmount = newAmount,
                    CurrentAmount = currentAmount,
                    NewBalance = newBalance,
                    CurrentBalance = currentBalance,
                    NewAbility = newAbility,
                    CurrentAbility = currentAbility,
                    Remarks = CsvField(fields, remarksIdx),
                }, header);
                imported++;
            }

            var data = new { Imported = imported, Failed = errors.Count, Errors = errors };
            return Ok(new { success = true, message = string.Format("Imported {0} of {1} row(s).", imported, dataLines.Count - 1), data });
        }

        [HttpDelete, Route("periods/{periodId:guid}/entries/{entryId:guid}")]
        public IHttpActionResult RemoveEntry(Guid periodId, Guid entryId)
        {
            var denied = RequireAny(SystemPermissionType.BackOfficeCheckOffDataCapture); if (denied != null) return denied;
            var period = _periods.FindDataAttachmentPeriod(periodId, Utils.CreateServiceHeader());
            if (period == null) return Error(HttpStatusCode.NotFound, "Data period not found.");
            if (period.Status != (int)DataAttachmentPeriodStatus.Open) return Error(HttpStatusCode.Conflict, "Entries can only be removed from an open period.");
            var page = _periods.FindDataAttachmentEntriesByDataAttachmentPeriodId(periodId, "", 0, int.MaxValue, Utils.CreateServiceHeader());
            if (page?.PageCollection == null || !page.PageCollection.Any(x => x.Id == entryId)) return Error(HttpStatusCode.NotFound, "Entry not found in this period.");
            return _periods.RemoveDataAttachmentEntries(new List<DataAttachmentEntryDTO> { new DataAttachmentEntryDTO { Id = entryId } }, Utils.CreateServiceHeader()) ? Success("Entry removed.") : Error(HttpStatusCode.Conflict, "Entry could not be removed.");
        }

        private static List<string> SplitCsvLine(string line)
        {
            var result = new List<string>();
            var current = new StringBuilder();
            var inQuotes = false;
            for (var i = 0; i < line.Length; i++)
            {
                var c = line[i];
                if (inQuotes)
                {
                    if (c == '"')
                    {
                        if (i + 1 < line.Length && line[i + 1] == '"') { current.Append('"'); i++; }
                        else inQuotes = false;
                    }
                    else current.Append(c);
                }
                else
                {
                    if (c == '"') inQuotes = true;
                    else if (c == ',') { result.Add(current.ToString()); current.Clear(); }
                    else current.Append(c);
                }
            }
            result.Add(current.ToString());
            return result;
        }

        private static string CsvField(List<string> fields, int index) { return index >= 0 && index < fields.Count ? (fields[index] ?? "").Trim() : ""; }

        private static bool TryParseAmount(List<string> fields, int index, out decimal value)
        {
            var text = CsvField(fields, index);
            if (string.IsNullOrWhiteSpace(text)) { value = 0m; return true; }
            return decimal.TryParse(text, out value);
        }

        private bool ValidPage(int index, int size) { return index >= 0 && size > 0 && size <= 1000; }
        private IHttpActionResult Success(string message, object data = null) { return Ok(new { success = true, message, data }); }
        private IHttpActionResult Error(HttpStatusCode status, string message) { return Content(status, new { success = false, message }); }
        private IHttpActionResult RequireAny(params SystemPermissionType[] permissions)
        {
            var header = Utils.CreateServiceHeader();
            foreach (var permission in permissions)
            {
                var roles = _authorization.GetRolesForSystemPermissionType((int)permission, header) ?? new string[0];
                if (header.ApplicationUserRoles.Any(role => roles.Any(allowed => string.Equals(role, allowed, StringComparison.OrdinalIgnoreCase)))) return null;
            }
            var required = string.Join(", ", permissions);
            return Error(HttpStatusCode.Forbidden, string.Format(
                "You do not have permission to do this. Your role needs one of the following permissions: {0}. Ask an administrator to grant it under Administration > Roles > Permission Types.",
                required));
        }
        public sealed class ClosePeriodRequest { public string Remarks { get; set; } }
    }
}
