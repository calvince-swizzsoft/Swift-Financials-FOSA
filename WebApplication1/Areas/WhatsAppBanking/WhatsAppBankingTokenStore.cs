using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.RegistryModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using LazyCache;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Areas.WhatsAppBanking
{
    // Ephemeral OTP/session state for the WhatsApp Banking bot-facing API, backed by the same
    // IAppCache (LazyCache/System.Runtime.Caching) already used for read-model caching
    // throughout Application.MainBoundedContext (e.g. LoanProductAppService, AlternateChannelAppService).
    // In-process only - does not survive an app-pool recycle and is not shared across
    // load-balanced instances. Reasonable for a single-instance pilot deployment; a
    // multi-instance production deployment needs a shared store (Redis, a DB table) instead -
    // flagged in docs/api/whatsapp-banking-api-spec.md, not solved here.
    public class WhatsAppBankingTokenStore
    {
        private static readonly TimeSpan OtpTtl = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan PhoneVerifiedTokenTtl = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan SessionTtl = TimeSpan.FromMinutes(15);

        private readonly IAppCache _appCache;

        public WhatsAppBankingTokenStore(IAppCache appCache)
        {
            _appCache = appCache ?? throw new ArgumentNullException(nameof(appCache));
        }

        public int OtpTtlSeconds => (int)OtpTtl.TotalSeconds;
        public int PhoneVerifiedTokenTtlSeconds => (int)PhoneVerifiedTokenTtl.TotalSeconds;
        public int SessionTtlSeconds => (int)SessionTtl.TotalSeconds;

        public string IssueOtp(string phoneNumber)
        {
            var otp = new Random().Next(100000, 999999).ToString();
            _appCache.Add(OtpKey(phoneNumber), otp, DateTimeOffset.Now.Add(OtpTtl));
            return otp;
        }

        public bool VerifyAndConsumeOtp(string phoneNumber, string otp)
        {
            var key = OtpKey(phoneNumber);
            var stored = _appCache.Get<string>(key);

            if (string.IsNullOrEmpty(stored) || !string.Equals(stored, otp, StringComparison.Ordinal))
                return false;

            _appCache.Remove(key);
            return true;
        }

        public string IssuePhoneVerifiedToken(string phoneNumber)
        {
            var token = Guid.NewGuid().ToString("N");
            _appCache.Add(PhoneVerifiedTokenKey(token), phoneNumber, DateTimeOffset.Now.Add(PhoneVerifiedTokenTtl));
            return token;
        }

        // consume = true (the default) removes the token after reading it - phoneVerifiedToken
        // is meant to authorize exactly one follow-up action (register OR link), not be reused.
        // Register calls with consume: false so the same token can still complete Link right
        // after (Register itself doesn't consume it - Link does).
        public string GetPhoneForVerifiedToken(string token, bool consume = true)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var key = PhoneVerifiedTokenKey(token);
            var phoneNumber = _appCache.Get<string>(key);

            if (!string.IsNullOrEmpty(phoneNumber) && consume)
                _appCache.Remove(key);

            return phoneNumber;
        }

        public string IssueSession(WhatsAppBankingSession session)
        {
            var token = Guid.NewGuid().ToString("N");
            _appCache.Add(SessionKey(token), session, DateTimeOffset.Now.Add(SessionTtl));
            return token;
        }

        // Refreshes the TTL on every successful lookup ("expiresInSeconds ... refreshed on each
        // call" per docs/api/whatsapp-banking-api-spec.md §2).
        public WhatsAppBankingSession GetSession(string sessionToken, bool refresh = true)
        {
            if (string.IsNullOrWhiteSpace(sessionToken))
                return null;

            var key = SessionKey(sessionToken);
            var session = _appCache.Get<WhatsAppBankingSession>(key);

            if (session != null && refresh)
                _appCache.Add(key, session, DateTimeOffset.Now.Add(SessionTtl));

            return session;
        }

        private static string OtpKey(string phoneNumber) => string.Format("WhatsAppBankingOtp_{0}", phoneNumber);
        private static string PhoneVerifiedTokenKey(string token) => string.Format("WhatsAppBankingPhoneVerifiedToken_{0}", token);
        private static string SessionKey(string token) => string.Format("WhatsAppBankingSession_{0}", token);
    }

    public class WhatsAppBankingSession
    {
        public Guid AlternateChannelId { get; set; }
        public Guid CustomerAccountId { get; set; }
        public Guid CustomerId { get; set; }
        public string PhoneNumber { get; set; }
    }

    // Shared "does this phone number already belong to a customer" lookup, used by both
    // IdentityController.VerifyOtp (to decide onboarding vs. linking) and
    // RegistrationController (to refuse creating a duplicate Customer, and to authorize Link
    // against the right customer). Checks two places, in order: an already-linked
    // AlternateChannel of ANY type (a member who already uses Sacco Link/Sparrow/etc. is still
    // an existing customer, not just a WhatsAppBanking-specific link), then
    // Customer.Address.MobileLine for a member who's never linked any channel yet. Getting this
    // wrong in either direction (treating an existing member as new, or vice versa) is a real
    // correctness bug, not a formality - checked deliberately, not assumed.
    internal static class WhatsAppBankingCustomerLookup
    {
        public static async Task<Guid?> FindExistingCustomerIdAsync(string phoneNumber, IAlternateChannelAppService alternateChannelAppService, ICustomerAppService customerAppService, ServiceHeader serviceHeader)
        {
            var existingLinks = alternateChannelAppService.FindAlternateChannelsByCardNumber(phoneNumber, serviceHeader);

            if (existingLinks != null && existingLinks.Any())
                return existingLinks[0].CustomerAccountCustomerId;

            var matches = await customerAppService.FindCustomersAsync(phoneNumber, (int)CustomerFilter.MobileLine, 0, 1, serviceHeader);

            if (matches?.PageCollection != null && matches.PageCollection.Any())
                return matches.PageCollection[0].Id;

            return null;
        }
    }
}
