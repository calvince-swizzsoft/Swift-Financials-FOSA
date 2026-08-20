using Application.MainBoundedContext.AdministrationModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Messaging.Controllers
{
    [Authorize]
    [RoutePrefix("api/messaging/instant-messages")]
    public class InstantMessagingController : ApiController
    {
        private readonly IAuthorizationAppService _authorizationAppService;
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["SwiftFin_Dev"].ConnectionString;
        private readonly string _authConnectionString = ConfigurationManager.ConnectionStrings["AuthStore"].ConnectionString;

        public InstantMessagingController(IAuthorizationAppService authorizationAppService)
        {
            _authorizationAppService = authorizationAppService ?? throw new ArgumentNullException(nameof(authorizationAppService));
        }

        [HttpGet, Route("contacts")]
        public async Task<IHttpActionResult> Contacts([FromUri] string text = "", [FromUri] int pageIndex = 0, [FromUri] int pageSize = 50)
        {
            var denied = DenyUnlessAllowed(); if (denied != null) return denied;
            if (pageIndex < 0 || pageSize < 1 || pageSize > 100) return BadRequest("Invalid paging values.");
            var rows = new List<object>(); var current = CurrentUser();
            using (var connection = new SqlConnection(_authConnectionString))
            using (var command = new SqlCommand(@"SELECT UserName,FirstName,OtherNames,Email FROM swiftFin_AspNetUsers WHERE UserName<>@Current AND (LockoutEndDateUtc IS NULL OR LockoutEndDateUtc<=GETUTCDATE()) AND (@Text='' OR UserName LIKE '%'+@Text+'%' OR FirstName LIKE '%'+@Text+'%' OR OtherNames LIKE '%'+@Text+'%' OR Email LIKE '%'+@Text+'%') ORDER BY FirstName,OtherNames,UserName OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY", connection))
            {
                command.Parameters.AddWithValue("@Current", current); command.Parameters.AddWithValue("@Text", (text ?? "").Trim()); command.Parameters.AddWithValue("@Offset", pageIndex * pageSize); command.Parameters.AddWithValue("@PageSize", pageSize);
                await connection.OpenAsync(); using (var reader = await command.ExecuteReaderAsync()) while (await reader.ReadAsync()) rows.Add(new { userName = reader.GetString(0), firstName = reader.IsDBNull(1) ? null : reader.GetString(1), otherNames = reader.IsDBNull(2) ? null : reader.GetString(2), email = reader.IsDBNull(3) ? null : reader.GetString(3) });
            }
            return Ok(new { success = true, message = "Contacts retrieved successfully.", data = rows });
        }

        [HttpGet, Route("conversations")]
        public async Task<IHttpActionResult> Conversations()
        {
            var denied = DenyUnlessAllowed(); if (denied != null) return denied;
            var rows = new List<object>(); var current = CurrentUser();
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(@"SELECT c.Id,c.Title,c.IsGroup,c.CreatedDate,c.ModifiedDate,(SELECT TOP 1 p2.UserName FROM swiftFin_InstantMessageConversationParticipants p2 WHERE p2.ConversationId=c.Id AND p2.UserName<>@User ORDER BY p2.UserName) OtherUser,(SELECT TOP 1 m.Body FROM swiftFin_InstantMessages m WHERE m.ConversationId=c.Id ORDER BY m.Id DESC) LastMessage,(SELECT TOP 1 m.SenderUserName FROM swiftFin_InstantMessages m WHERE m.ConversationId=c.Id ORDER BY m.Id DESC) LastSender,(SELECT TOP 1 m.CreatedDate FROM swiftFin_InstantMessages m WHERE m.ConversationId=c.Id ORDER BY m.Id DESC) LastMessageDate,(SELECT COUNT(*) FROM swiftFin_InstantMessages m WHERE m.ConversationId=c.Id AND m.SenderUserName<>@User AND (p.LastReadDate IS NULL OR m.CreatedDate>p.LastReadDate)) UnreadCount,(SELECT COUNT(*) FROM swiftFin_InstantMessageConversationParticipants pc WHERE pc.ConversationId=c.Id) ParticipantCount FROM swiftFin_InstantMessageConversations c INNER JOIN swiftFin_InstantMessageConversationParticipants p ON p.ConversationId=c.Id AND p.UserName=@User ORDER BY COALESCE((SELECT MAX(m.CreatedDate) FROM swiftFin_InstantMessages m WHERE m.ConversationId=c.Id),c.ModifiedDate) DESC", connection))
            {
                command.Parameters.AddWithValue("@User", current); await connection.OpenAsync(); using (var reader = await command.ExecuteReaderAsync()) while (await reader.ReadAsync()) rows.Add(new { id = reader.GetGuid(0), title = reader.IsDBNull(1) ? null : reader.GetString(1), isGroup = reader.GetBoolean(2), createdDate = reader.GetDateTime(3), modifiedDate = reader.GetDateTime(4), otherUser = reader.IsDBNull(5) ? null : reader.GetString(5), lastMessage = reader.IsDBNull(6) ? null : reader.GetString(6), lastSender = reader.IsDBNull(7) ? null : reader.GetString(7), lastMessageDate = reader.IsDBNull(8) ? (DateTime?)null : reader.GetDateTime(8), unreadCount = reader.GetInt32(9), participantCount = reader.GetInt32(10) });
            }
            return Ok(new { success = true, message = "Conversations retrieved successfully.", data = rows });
        }

        [HttpPost, Route("conversations")]
        public async Task<IHttpActionResult> CreateConversation(CreateConversationRequest request)
        {
            var denied = DenyUnlessAllowed(); if (denied != null) return denied;
            var current = CurrentUser();
            var participants = (request?.ParticipantUserNames ?? new List<string>()).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).Where(x => !string.Equals(x, current, StringComparison.OrdinalIgnoreCase)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (participants.Count < 1 || participants.Count > 49) return BadRequest("Select between 1 and 49 other participants.");
            if (!await UsersExist(participants)) return BadRequest("One or more selected users do not exist or are currently locked.");

            var isGroup = participants.Count > 1;
            if (isGroup && string.IsNullOrWhiteSpace(request.Title)) return BadRequest("A group conversation title is required.");
            if (!isGroup)
            {
                var existing = await FindDirectConversation(current, participants[0]);
                if (existing.HasValue) return Ok(new { success = true, message = "Existing direct conversation returned.", data = new { id = existing.Value } });
            }

            var id = Guid.NewGuid();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(); using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var command = new SqlCommand("INSERT INTO swiftFin_InstantMessageConversations(Id,Title,IsGroup,CreatedBy) VALUES(@Id,@Title,@IsGroup,@User)", connection, transaction)) { command.Parameters.AddWithValue("@Id", id); command.Parameters.AddWithValue("@Title", isGroup ? (object)request.Title.Trim() : DBNull.Value); command.Parameters.AddWithValue("@IsGroup", isGroup); command.Parameters.AddWithValue("@User", current); await command.ExecuteNonQueryAsync(); }
                        participants.Insert(0, current);
                        foreach (var participant in participants) using (var command = new SqlCommand("INSERT INTO swiftFin_InstantMessageConversationParticipants(ConversationId,UserName,LastReadDate) VALUES(@Id,@User,SYSUTCDATETIME())", connection, transaction)) { command.Parameters.AddWithValue("@Id", id); command.Parameters.AddWithValue("@User", participant); await command.ExecuteNonQueryAsync(); }
                        transaction.Commit();
                    }
                    catch { transaction.Rollback(); throw; }
                }
            }
            return Content(HttpStatusCode.Created, new { success = true, message = "Conversation created successfully.", data = new { id } });
        }

        [HttpGet, Route("conversations/{conversationId:guid}/messages")]
        public async Task<IHttpActionResult> Messages(Guid conversationId, [FromUri] long afterId = 0, [FromUri] int pageSize = 50)
        {
            var denied = DenyUnlessAllowed(); if (denied != null) return denied;
            if (pageSize < 1 || pageSize > 100) return BadRequest("pageSize must be between 1 and 100.");
            if (!await IsParticipant(conversationId, CurrentUser())) return StatusCode(HttpStatusCode.Forbidden);
            var rows = new List<object>();
            var sql = afterId > 0 ? @"SELECT TOP (@PageSize) Id,ConversationId,SenderUserName,Body,CreatedDate FROM swiftFin_InstantMessages WHERE ConversationId=@Id AND Id>@AfterId ORDER BY Id" : @"SELECT * FROM (SELECT TOP (@PageSize) Id,ConversationId,SenderUserName,Body,CreatedDate FROM swiftFin_InstantMessages WHERE ConversationId=@Id ORDER BY Id DESC) recent ORDER BY Id";
            using (var connection = new SqlConnection(_connectionString)) using (var command = new SqlCommand(sql, connection)) { command.Parameters.AddWithValue("@Id", conversationId); command.Parameters.AddWithValue("@AfterId", afterId); command.Parameters.AddWithValue("@PageSize", pageSize); await connection.OpenAsync(); using (var reader = await command.ExecuteReaderAsync()) while (await reader.ReadAsync()) rows.Add(new { id = reader.GetInt64(0), conversationId = reader.GetGuid(1), senderUserName = reader.GetString(2), body = reader.GetString(3), createdDate = reader.GetDateTime(4) }); }
            return Ok(new { success = true, message = "Messages retrieved successfully.", data = rows });
        }

        [HttpPost, Route("conversations/{conversationId:guid}/messages")]
        public async Task<IHttpActionResult> Send(Guid conversationId, SendMessageRequest request)
        {
            var denied = DenyUnlessAllowed(); if (denied != null) return denied;
            var current = CurrentUser(); if (!await IsParticipant(conversationId, current)) return StatusCode(HttpStatusCode.Forbidden);
            var body = (request?.Body ?? "").Trim(); if (body.Length < 1 || body.Length > 4000) return BadRequest("Message must contain between 1 and 4000 characters.");
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(); using (var transaction = connection.BeginTransaction())
                {
                    long id;
                    using (var command = new SqlCommand("INSERT INTO swiftFin_InstantMessages(ConversationId,SenderUserName,Body) OUTPUT INSERTED.Id VALUES(@ConversationId,@Sender,@Body)", connection, transaction)) { command.Parameters.AddWithValue("@ConversationId", conversationId); command.Parameters.AddWithValue("@Sender", current); command.Parameters.AddWithValue("@Body", body); id = Convert.ToInt64(await command.ExecuteScalarAsync()); }
                    using (var command = new SqlCommand("UPDATE swiftFin_InstantMessageConversations SET ModifiedDate=SYSUTCDATETIME() WHERE Id=@Id; UPDATE swiftFin_InstantMessageConversationParticipants SET LastReadDate=SYSUTCDATETIME() WHERE ConversationId=@Id AND UserName=@User", connection, transaction)) { command.Parameters.AddWithValue("@Id", conversationId); command.Parameters.AddWithValue("@User", current); await command.ExecuteNonQueryAsync(); }
                    transaction.Commit();
                    return Content(HttpStatusCode.Created, new { success = true, message = "Message sent successfully.", data = new { id, conversationId, senderUserName = current, body, createdDate = DateTime.UtcNow } });
                }
            }
        }

        [HttpPost, Route("conversations/{conversationId:guid}/read")]
        public async Task<IHttpActionResult> MarkRead(Guid conversationId)
        {
            var denied = DenyUnlessAllowed(); if (denied != null) return denied;
            using (var connection = new SqlConnection(_connectionString)) using (var command = new SqlCommand("UPDATE swiftFin_InstantMessageConversationParticipants SET LastReadDate=SYSUTCDATETIME() WHERE ConversationId=@Id AND UserName=@User", connection)) { command.Parameters.AddWithValue("@Id", conversationId); command.Parameters.AddWithValue("@User", CurrentUser()); await connection.OpenAsync(); return await command.ExecuteNonQueryAsync() == 0 ? (IHttpActionResult)StatusCode(HttpStatusCode.Forbidden) : Ok(new { success = true, message = "Conversation marked as read." }); }
        }

        private IHttpActionResult DenyUnlessAllowed() { var header = Utils.CreateServiceHeader(); var roles = _authorizationAppService.GetRolesForSystemPermissionType((int)SystemPermissionType.InstantMessagingAccess, header) ?? new string[0]; return header.ApplicationUserRoles.Any(role => roles.Any(allowed => string.Equals(role, allowed, StringComparison.OrdinalIgnoreCase))) ? null : StatusCode(HttpStatusCode.Forbidden); }
        private static string CurrentUser() { return (HttpContext.Current?.User as ClaimsPrincipal)?.Identity?.Name; }
        private async Task<bool> IsParticipant(Guid id, string user) { using (var connection = new SqlConnection(_connectionString)) using (var command = new SqlCommand("SELECT COUNT(*) FROM swiftFin_InstantMessageConversationParticipants WHERE ConversationId=@Id AND UserName=@User", connection)) { command.Parameters.AddWithValue("@Id", id); command.Parameters.AddWithValue("@User", user); await connection.OpenAsync(); return Convert.ToInt32(await command.ExecuteScalarAsync()) > 0; } }
        private async Task<Guid?> FindDirectConversation(string first, string second) { using (var connection = new SqlConnection(_connectionString)) using (var command = new SqlCommand(@"SELECT TOP 1 c.Id FROM swiftFin_InstantMessageConversations c WHERE c.IsGroup=0 AND (SELECT COUNT(*) FROM swiftFin_InstantMessageConversationParticipants p WHERE p.ConversationId=c.Id)=2 AND EXISTS(SELECT 1 FROM swiftFin_InstantMessageConversationParticipants p WHERE p.ConversationId=c.Id AND p.UserName=@First) AND EXISTS(SELECT 1 FROM swiftFin_InstantMessageConversationParticipants p WHERE p.ConversationId=c.Id AND p.UserName=@Second)", connection)) { command.Parameters.AddWithValue("@First", first); command.Parameters.AddWithValue("@Second", second); await connection.OpenAsync(); var value = await command.ExecuteScalarAsync(); return value == null ? (Guid?)null : (Guid)value; } }
        private async Task<bool> UsersExist(List<string> users) { using (var connection = new SqlConnection(_authConnectionString)) using (var command = connection.CreateCommand()) { var names = new List<string>(); for (var i = 0; i < users.Count; i++) { var name = "@U" + i; names.Add(name); command.Parameters.AddWithValue(name, users[i]); } command.CommandText = "SELECT COUNT(*) FROM swiftFin_AspNetUsers WHERE UserName IN (" + string.Join(",", names) + ") AND (LockoutEndDateUtc IS NULL OR LockoutEndDateUtc<=GETUTCDATE())"; await connection.OpenAsync(); return Convert.ToInt32(await command.ExecuteScalarAsync()) == users.Count; } }
        public sealed class CreateConversationRequest { public string Title { get; set; } public List<string> ParticipantUserNames { get; set; } }
        public sealed class SendMessageRequest { public string Body { get; set; } }
    }
}
