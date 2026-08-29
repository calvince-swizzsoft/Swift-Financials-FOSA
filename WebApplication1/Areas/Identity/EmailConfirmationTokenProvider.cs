using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;

namespace WebApplication1.Areas.Identity
{
    // Stores only a SHA-256 token hash and issue time in the existing Identity claims table.
    public static class EmailConfirmationTokens
    {
        private const string ClaimType = "SwiftFinancials.EmailConfirmation.v3";

        public static string Issue(ApplicationDbContext context, ApplicationUser user)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (user == null || string.IsNullOrWhiteSpace(user.Id))
                throw new InvalidOperationException("An existing user is required before an email-confirmation token can be issued.");

            var claims = context.Set<IdentityUserClaim>();
            var previousTokens = claims.Where(item => item.UserId == user.Id && item.ClaimType == ClaimType).ToList();
            if (previousTokens.Count > 0) claims.RemoveRange(previousTokens);

            var tokenBytes = new byte[32];
            using (var random = RandomNumberGenerator.Create()) random.GetBytes(tokenBytes);
            var token = Base64UrlEncode(tokenBytes);
            claims.Add(new IdentityUserClaim
            {
                UserId = user.Id,
                ClaimType = ClaimType,
                ClaimValue = Hash(token) + "." + DateTime.UtcNow.Ticks
            });

            try { context.SaveChanges(); }
            catch (Exception exception)
            {
                throw new InvalidOperationException("The email-confirmation token could not be stored in AuthStore.", exception);
            }
            return token;
        }

        public static bool ValidateAndConsume(ApplicationDbContext context, ApplicationUser user, string token)
        {
            if (context == null || user == null || string.IsNullOrWhiteSpace(token)) return false;
            var claims = context.Set<IdentityUserClaim>();
            var storedTokens = claims.Where(item => item.UserId == user.Id && item.ClaimType == ClaimType).ToList();
            var expectedHash = Hash(token);
            var lifetimeHours = 24;
            int configuredHours;
            if (int.TryParse(ConfigurationManager.AppSettings["Identity:EmailConfirmationTokenHours"], out configuredHours) && configuredHours > 0)
                lifetimeHours = configuredHours;

            if (!storedTokens.Any(item => IsValid(item.ClaimValue, expectedHash, lifetimeHours))) return false;
            user.EmailConfirmed = true;
            claims.RemoveRange(storedTokens);
            context.SaveChanges();
            return true;
        }

        private static bool IsValid(string value, string expectedHash, int lifetimeHours)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            var separator = value.LastIndexOf('.');
            long issuedTicks;
            if (separator <= 0 || !long.TryParse(value.Substring(separator + 1), out issuedTicks)) return false;
            DateTime issuedUtc;
            try { issuedUtc = new DateTime(issuedTicks, DateTimeKind.Utc); }
            catch (ArgumentOutOfRangeException) { return false; }
            return FixedTimeEquals(value.Substring(0, separator), expectedHash)
                && DateTime.UtcNow <= issuedUtc.AddHours(lifetimeHours);
        }

        private static string Hash(string token)
        {
            using (var sha256 = SHA256.Create())
                return Base64UrlEncode(sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(token ?? string.Empty)));
        }

        private static string Base64UrlEncode(byte[] value)
        {
            return Convert.ToBase64String(value).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        private static bool FixedTimeEquals(string left, string right)
        {
            if (left == null || right == null || left.Length != right.Length) return false;
            var difference = 0;
            for (var index = 0; index < left.Length; index++) difference |= left[index] ^ right[index];
            return difference == 0;
        }
    }
}
