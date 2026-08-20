using Application.MainBoundedContext.AdministrationModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml;
using System.Xml.Linq;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Reports.Controllers
{
    [Authorize]
    [RoutePrefix("api/reports/user-defined")]
    public class UserDefinedReportsController : ApiController
    {
        private readonly IAuthorizationAppService _authorizationAppService;
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["SwiftFin_Dev"].ConnectionString;
        private readonly string _viewerBaseUrl = ConfigurationManager.AppSettings["Ssrs:ViewerBaseUrl"];
        private readonly int _maxRdlBytes;

        public UserDefinedReportsController(IAuthorizationAppService authorizationAppService)
        {
            _authorizationAppService = authorizationAppService ?? throw new ArgumentNullException(nameof(authorizationAppService));
            int configuredMax;
            _maxRdlBytes = int.TryParse(ConfigurationManager.AppSettings["Ssrs:MaxRdlBytes"], out configuredMax) ? configuredMax : 5 * 1024 * 1024;
        }

        [HttpGet, Route("")]
        public async Task<IHttpActionResult> Index([FromUri] string text = "", [FromUri] int? categoryId = null, [FromUri] int pageIndex = 0, [FromUri] int pageSize = 20, [FromUri] bool includeInactive = false)
        {
            var header = Utils.CreateServiceHeader();
            var canManage = HasPermission(SystemPermissionType.UserDefinedReportAdministration, header);
            if (!canManage && !HasPermission(SystemPermissionType.UserDefinedReportViewing, header)) return StatusCode(HttpStatusCode.Forbidden);
            if (pageIndex < 0 || pageSize < 1 || pageSize > 100) return BadRequest("pageIndex must be non-negative and pageSize must be between 1 and 100.");

            try
            {
                var rows = new List<object>();
                int count;
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    const string where = @" WHERE (@CanSeeInactive=1 OR r.IsActive=1) AND (@CategoryId IS NULL OR r.CategoryId=@CategoryId) AND (@Text='' OR r.Name LIKE '%'+@Text+'%' OR r.Description LIKE '%'+@Text+'%' OR r.ReportPath LIKE '%'+@Text+'%' OR c.Name LIKE '%'+@Text+'%')";
                    using (var countCommand = new SqlCommand("SELECT COUNT(*) FROM swiftFin_UserDefinedReports r INNER JOIN swiftFin_UserDefinedReportCategories c ON c.Id=r.CategoryId" + where, connection))
                    {
                        AddFilters(countCommand, text, categoryId, includeInactive && canManage);
                        count = Convert.ToInt32(await countCommand.ExecuteScalarAsync());
                    }
                    using (var command = new SqlCommand(@"SELECT r.Id,r.CategoryId,c.Name CategoryName,r.Name,r.Description,r.ReportPath,r.FileName,r.IsActive,r.CreatedBy,r.CreatedDate,r.ModifiedBy,r.ModifiedDate FROM swiftFin_UserDefinedReports r INNER JOIN swiftFin_UserDefinedReportCategories c ON c.Id=r.CategoryId" + where + " ORDER BY c.Name,r.Name OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY", connection))
                    {
                        AddFilters(command, text, categoryId, includeInactive && canManage);
                        command.Parameters.AddWithValue("@Offset", pageIndex * pageSize);
                        command.Parameters.AddWithValue("@PageSize", pageSize);
                        using (var reader = await command.ExecuteReaderAsync())
                            while (await reader.ReadAsync()) rows.Add(MapReport(reader));
                    }
                }
                return Ok(new { success = true, message = "Reports retrieved successfully.", data = new { pageIndex, pageSize, itemsCount = count, pageCollection = rows, canManage, viewerConfigured = IsViewerConfigured() } });
            }
            catch (SqlException ex) when (ex.Number == 208) { return Content(HttpStatusCode.ServiceUnavailable, new { success = false, message = "User-Defined Reports database schema is not installed." }); }
            catch (Exception ex) { return InternalServerError(ex); }
        }

        [HttpGet, Route("categories")]
        public async Task<IHttpActionResult> Categories()
        {
            var header = Utils.CreateServiceHeader();
            if (!HasPermission(SystemPermissionType.UserDefinedReportViewing, header) && !HasPermission(SystemPermissionType.UserDefinedReportAdministration, header)) return StatusCode(HttpStatusCode.Forbidden);
            var rows = new List<object>();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand("SELECT Id,Name,CreatedBy,CreatedDate FROM swiftFin_UserDefinedReportCategories ORDER BY Name", connection))
                {
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync()) while (await reader.ReadAsync()) rows.Add(new { id = reader.GetInt32(0), name = reader.GetString(1), createdBy = reader.IsDBNull(2) ? null : reader.GetString(2), createdDate = reader.GetDateTime(3) });
                }
                return Ok(new { success = true, message = "Categories retrieved successfully.", data = rows });
            }
            catch (Exception ex) { return InternalServerError(ex); }
        }

        [HttpGet, Route("{id:int}/view")]
        public async Task<IHttpActionResult> View(int id)
        {
            var header = Utils.CreateServiceHeader();
            if (!HasPermission(SystemPermissionType.UserDefinedReportViewing, header) && !HasPermission(SystemPermissionType.UserDefinedReportAdministration, header)) return StatusCode(HttpStatusCode.Forbidden);
            if (!IsViewerConfigured()) return Content(HttpStatusCode.ServiceUnavailable, new { success = false, message = "SSRS viewer is not configured. Set Ssrs:ViewerBaseUrl for this environment." });
            string path;
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SELECT ReportPath FROM swiftFin_UserDefinedReports WHERE Id=@Id AND IsActive=1", connection))
            {
                command.Parameters.AddWithValue("@Id", id); await connection.OpenAsync(); path = Convert.ToString(await command.ExecuteScalarAsync());
            }
            if (string.IsNullOrWhiteSpace(path)) return NotFound();
            var segments = path.Trim().Trim('/').Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Select(Uri.EscapeDataString);
            return Ok(new { success = true, message = "Viewer URL generated successfully.", data = new { url = _viewerBaseUrl.TrimEnd('/') + "/" + string.Join("/", segments) } });
        }

        [HttpPost, Route("categories")]
        public async Task<IHttpActionResult> CreateCategory(CategoryRequest request)
        {
            var header = Utils.CreateServiceHeader(); if (!HasPermission(SystemPermissionType.UserDefinedReportAdministration, header)) return StatusCode(HttpStatusCode.Forbidden);
            var name = (request?.Name ?? "").Trim(); if (name.Length < 2 || name.Length > 150) return BadRequest("Category name must be between 2 and 150 characters.");
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand("INSERT INTO swiftFin_UserDefinedReportCategories(Name,CreatedBy) OUTPUT INSERTED.Id VALUES(@Name,@User)", connection))
                { command.Parameters.AddWithValue("@Name", name); command.Parameters.AddWithValue("@User", header.ApplicationUserName); await connection.OpenAsync(); var id = Convert.ToInt32(await command.ExecuteScalarAsync()); return Content(HttpStatusCode.Created, new { success = true, message = "Category created successfully.", data = new { id, name } }); }
            }
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627) { return Content(HttpStatusCode.Conflict, new { success = false, message = "A category with that name already exists." }); }
            catch (Exception ex) { return InternalServerError(ex); }
        }

        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Upload()
        {
            var header = Utils.CreateServiceHeader(); if (!HasPermission(SystemPermissionType.UserDefinedReportAdministration, header)) return StatusCode(HttpStatusCode.Forbidden);
            if (!Request.Content.IsMimeMultipartContent()) return BadRequest("multipart/form-data is required.");
            if (Request.Content.Headers.ContentLength > _maxRdlBytes + 65536) return Content(HttpStatusCode.RequestEntityTooLarge, new { success = false, message = "RDL file exceeds the configured size limit." });
            try
            {
                var provider = new MultipartMemoryStreamProvider(); await Request.Content.ReadAsMultipartAsync(provider);
                var filePart = provider.Contents.FirstOrDefault(c => c.Headers.ContentDisposition.FileName != null);
                if (filePart == null) return BadRequest("An RDL file is required.");
                var fileName = Path.GetFileName(filePart.Headers.ContentDisposition.FileName.Trim('"'));
                var bytes = await filePart.ReadAsByteArrayAsync();
                if (!fileName.EndsWith(".rdl", StringComparison.OrdinalIgnoreCase) || bytes.Length == 0 || bytes.Length > _maxRdlBytes || !IsValidRdl(bytes)) return BadRequest("The upload must be a valid, non-empty .rdl XML document within the configured size limit.");
                var fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var part in provider.Contents.Where(c => c.Headers.ContentDisposition.FileName == null)) fields[part.Headers.ContentDisposition.Name.Trim('"')] = (await part.ReadAsStringAsync()).Trim();
                int categoryId; if (!int.TryParse(Get(fields, "categoryId"), out categoryId)) return BadRequest("categoryId is required.");
                var name = Get(fields, "name"); var description = Get(fields, "description"); var reportPath = NormalizeReportPath(Get(fields, "reportPath"));
                if (string.IsNullOrWhiteSpace(name) || name.Length > 200 || string.IsNullOrWhiteSpace(reportPath)) return BadRequest("name and reportPath are required.");
                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(@"INSERT INTO swiftFin_UserDefinedReports(CategoryId,Name,Description,ReportPath,FileName,RdlContent,CreatedBy) OUTPUT INSERTED.Id VALUES(@CategoryId,@Name,@Description,@ReportPath,@FileName,@Content,@User)", connection))
                { command.Parameters.AddWithValue("@CategoryId", categoryId); command.Parameters.AddWithValue("@Name", name); command.Parameters.AddWithValue("@Description", (object)description ?? DBNull.Value); command.Parameters.AddWithValue("@ReportPath", reportPath); command.Parameters.AddWithValue("@FileName", fileName); command.Parameters.Add("@Content", SqlDbType.VarBinary, -1).Value = bytes; command.Parameters.AddWithValue("@User", header.ApplicationUserName); await connection.OpenAsync(); var id = Convert.ToInt32(await command.ExecuteScalarAsync()); return Content(HttpStatusCode.Created, new { success = true, message = "Report catalogue entry created. Publish the RDL to SSRS at the configured report path before viewing it.", data = new { id } }); }
            }
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627) { return Content(HttpStatusCode.Conflict, new { success = false, message = "A report with that name or SSRS path already exists." }); }
            catch (Exception ex) { return InternalServerError(ex); }
        }

        [HttpPut, Route("{id:int}")]
        public async Task<IHttpActionResult> Update(int id, UpdateReportRequest request)
        {
            var header = Utils.CreateServiceHeader(); if (!HasPermission(SystemPermissionType.UserDefinedReportAdministration, header)) return StatusCode(HttpStatusCode.Forbidden);
            if (request == null || string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.ReportPath)) return BadRequest("name and reportPath are required.");
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(@"UPDATE swiftFin_UserDefinedReports SET CategoryId=@CategoryId,Name=@Name,Description=@Description,ReportPath=@ReportPath,IsActive=@IsActive,ModifiedBy=@User,ModifiedDate=SYSUTCDATETIME() WHERE Id=@Id", connection))
            { command.Parameters.AddWithValue("@Id", id); command.Parameters.AddWithValue("@CategoryId", request.CategoryId); command.Parameters.AddWithValue("@Name", request.Name.Trim()); command.Parameters.AddWithValue("@Description", (object)request.Description ?? DBNull.Value); command.Parameters.AddWithValue("@ReportPath", NormalizeReportPath(request.ReportPath)); command.Parameters.AddWithValue("@IsActive", request.IsActive); command.Parameters.AddWithValue("@User", header.ApplicationUserName); await connection.OpenAsync(); return await command.ExecuteNonQueryAsync() == 0 ? (IHttpActionResult)NotFound() : Ok(new { success = true, message = "Report updated successfully." }); }
        }

        [HttpDelete, Route("{id:int}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var header = Utils.CreateServiceHeader(); if (!HasPermission(SystemPermissionType.UserDefinedReportAdministration, header)) return StatusCode(HttpStatusCode.Forbidden);
            using (var connection = new SqlConnection(_connectionString)) using (var command = new SqlCommand("DELETE FROM swiftFin_UserDefinedReports WHERE Id=@Id", connection)) { command.Parameters.AddWithValue("@Id", id); await connection.OpenAsync(); return await command.ExecuteNonQueryAsync() == 0 ? (IHttpActionResult)NotFound() : Ok(new { success = true, message = "Report deleted successfully." }); }
        }

        [HttpGet, Route("{id:int}/rdl")]
        public async Task<HttpResponseMessage> DownloadRdl(int id)
        {
            var header = Utils.CreateServiceHeader(); if (!HasPermission(SystemPermissionType.UserDefinedReportAdministration, header)) return Request.CreateResponse(HttpStatusCode.Forbidden);
            byte[] bytes = null; string fileName = null;
            using (var connection = new SqlConnection(_connectionString)) using (var command = new SqlCommand("SELECT FileName,RdlContent FROM swiftFin_UserDefinedReports WHERE Id=@Id", connection)) { command.Parameters.AddWithValue("@Id", id); await connection.OpenAsync(); using (var reader = await command.ExecuteReaderAsync()) if (await reader.ReadAsync()) { fileName = reader.GetString(0); bytes = (byte[])reader[1]; } }
            if (bytes == null) return Request.CreateResponse(HttpStatusCode.NotFound);
            var response = Request.CreateResponse(HttpStatusCode.OK); response.Content = new ByteArrayContent(bytes); response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml"); response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = fileName }; return response;
        }

        private bool HasPermission(SystemPermissionType permission, ServiceHeader header) { var roles = _authorizationAppService.GetRolesForSystemPermissionType((int)permission, header) ?? new string[0]; return header.ApplicationUserRoles.Any(role => roles.Any(allowed => string.Equals(role, allowed, StringComparison.OrdinalIgnoreCase))); }
        private bool IsViewerConfigured() { Uri uri; return Uri.TryCreate(_viewerBaseUrl, UriKind.Absolute, out uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps); }
        private static string NormalizeReportPath(string value) { return string.IsNullOrWhiteSpace(value) ? null : string.Join("/", value.Trim().Trim('/').Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)); }
        private static string Get(IDictionary<string, string> fields, string key) { string value; return fields.TryGetValue(key, out value) ? value : null; }
        private static bool IsValidRdl(byte[] bytes) { try { var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null, MaxCharactersInDocument = 10 * 1024 * 1024 }; using (var stream = new MemoryStream(bytes)) using (var reader = XmlReader.Create(stream, settings)) return XDocument.Load(reader).Root?.Name.LocalName == "Report"; } catch { return false; } }
        private static void AddFilters(SqlCommand command, string text, int? categoryId, bool includeInactive) { command.Parameters.AddWithValue("@Text", (text ?? "").Trim()); command.Parameters.AddWithValue("@CategoryId", (object)categoryId ?? DBNull.Value); command.Parameters.AddWithValue("@CanSeeInactive", includeInactive); }
        private object MapReport(SqlDataReader r) { var path = r.GetString(5); return new { id = r.GetInt32(0), categoryId = r.GetInt32(1), categoryName = r.GetString(2), name = r.GetString(3), description = r.IsDBNull(4) ? null : r.GetString(4), reportPath = path, fileName = r.GetString(6), isActive = r.GetBoolean(7), createdBy = r.IsDBNull(8) ? null : r.GetString(8), createdDate = r.GetDateTime(9), modifiedBy = r.IsDBNull(10) ? null : r.GetString(10), modifiedDate = r.IsDBNull(11) ? (DateTime?)null : r.GetDateTime(11) }; }
        public sealed class CategoryRequest { public string Name { get; set; } }
        public sealed class UpdateReportRequest { public int CategoryId { get; set; } public string Name { get; set; } public string Description { get; set; } public string ReportPath { get; set; } public bool IsActive { get; set; } }
    }
}
