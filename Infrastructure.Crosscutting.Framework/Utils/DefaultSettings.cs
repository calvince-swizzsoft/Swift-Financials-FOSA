using System;
using System.Collections.Generic;
using System.Configuration;

namespace Infrastructure.Crosscutting.Framework.Utils
{
    public sealed class DefaultSettings
    {
        private static readonly object SyncRoot = new object();

        private DefaultSettings() { }

        private static DefaultSettings instance;
        public static DefaultSettings Instance
        {
            get
            {
                lock (SyncRoot)
                {
                    if (instance == null)
                        instance = new DefaultSettings();

                    /*
                     * Only vendor should know this :-(
                     */
                    instance.RootUser = "admin";
                    instance.RootPassword = "Abc.2020!";
                    instance.AuditUser = "auditor";
                    instance.AuditPassword = "Myadmin.2020!";
                    instance.Password = "Kenya@2030!";
                    instance.PasswordQuestion = "Where were you when you first heard about 14/13?";
                    instance.PasswordAnswer = "bmt";
                    instance.RootEmail = "info@stamlinetechnologies.com";
                    instance.TablePrefix = "swiftFin_";

                    instance.PageSizes = new List<int> { 15, 25, 50, 100, 200, 300, 400 };
                    
                    instance.MinRequiredMembershipAge = 18;
                    instance.ServerDate = DateTime.Now;

                    instance.AlternateChannelsDefaultDailyLimit = 40000m;

                    // Deployment-specific digital-channel settings (WhatsApp Banking and future
                    // self-service channels) - read from <appSettings> here, same as every other
                    // value in this block, rather than hardcoded, since they're real per-install
                    // values (a Branch id, a provider Paybill, a shared secret) nobody can bake into
                    // source. Missing/unparseable keys fall back to the "unconfigured" defaults the
                    // properties below already document (Guid.Empty / null) - callers already treat
                    // that as a per-request failure, not a reason to crash startup.
                    instance.DigitalChannelBranchId = Guid.TryParse(ConfigurationManager.AppSettings["DigitalChannelBranchId"], out var digitalChannelBranchId)
                        ? digitalChannelBranchId
                        : Guid.Empty;
                    instance.MobileMoneyPaybillBusinessShortCode = ConfigurationManager.AppSettings["MobileMoneyPaybillBusinessShortCode"];
                    instance.MobileToBankWebhookSecret = ConfigurationManager.AppSettings["MobileToBankWebhookSecret"];
                }

                return instance;
            }
        }

        public string RootUser { get; private set; }

        public string RootPassword { get; private set; }

        public string AuditUser { get; private set; }

        public string AuditPassword { get; private set; }

        public string Password { get; private set; }

        public string PasswordQuestion { get; private set; }

        public string PasswordAnswer { get; private set; }

        public string RootEmail { get; private set; }

        public string TablePrefix { get; private set; }

        public List<int> PageSizes { get; private set; }

        public string CurrentAppDomainName { get; set; }

        public string CurrentAppUserName { get; set; }

        public string CurrentAppUserPassword { get; set; }

        public int MinRequiredPasswordLength { get; set; }

        public int MinRequiredNonAlphanumericCharacters { get; set; }

        public string SSRSHost { get; set; }

        public int? SSRSPort { get; set; }

        public string SignalRHubUrl { get; set; }

        public int MinRequiredMembershipAge { get; private set; }

        public decimal AlternateChannelsDefaultDailyLimit { get; private set; }

        public DateTime ServerDate { get; set; }

        // Deployment-specific, not a hardcodable constant like the settings above (a Branch is a
        // real runtime-created row, its id differs per install) - the Branch under which
        // self-onboarded digital-channel customers (e.g. WhatsApp Banking) are registered.
        // Populated above from <appSettings key="DigitalChannelBranchId"> in Web.config; ops set
        // it there, no code change needed. Defaults to Guid.Empty (unconfigured) if missing/
        // unparseable; callers must treat that as "not set up yet", not "no branch restriction".
        public Guid DigitalChannelBranchId { get; set; }

        // Same reasoning as DigitalChannelBranchId - a real provider-issued value (Paybill/
        // Till/business shortcode), differs per deployment/provider contract, cannot be
        // hardcoded. Populated above from <appSettings key="MobileMoneyPaybillBusinessShortCode">.
        // Defaults to null/empty (unconfigured) if the key is missing.
        public string MobileMoneyPaybillBusinessShortCode { get; set; }

        // Shared secret the inbound C2B webhook (WebApplication1/Areas/WhatsAppBanking) checks
        // against an X-Webhook-Secret header, since a payment provider's server-to-server
        // callback can't participate in this system's staff/service JWT bearer scheme. Populated
        // above from <appSettings key="MobileToBankWebhookSecret">. Defaults to null/empty
        // (unconfigured) if the key is missing, which the webhook must treat as "refuse every
        // request", not "no secret required".
        public string MobileToBankWebhookSecret { get; set; }
    }
}
