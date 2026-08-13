using System;
using System.Collections.Generic;

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
        // real runtime-created row, its id differs per install) - ops must set this once, at
        // app-start, to the Branch under which self-onboarded digital-channel customers (e.g.
        // WhatsApp Banking) are registered. Defaults to Guid.Empty (unconfigured); callers must
        // treat that as "not set up yet", not "no branch restriction".
        public Guid DigitalChannelBranchId { get; set; }

        // Same reasoning as DigitalChannelBranchId - a real provider-issued value (Paybill/
        // Till/business shortcode), differs per deployment/provider contract, cannot be
        // hardcoded. Defaults to null/empty (unconfigured).
        public string MobileMoneyPaybillBusinessShortCode { get; set; }

        // Shared secret the inbound C2B webhook (WebApplication1/Areas/WhatsAppBanking) checks
        // against an X-Webhook-Secret header, since a payment provider's server-to-server
        // callback can't participate in this system's staff/service JWT bearer scheme. Ops must
        // set this to a real secret shared with the provider before the webhook is usable;
        // defaults to null/empty (unconfigured), which the webhook must treat as "refuse every
        // request", not "no secret required".
        public string MobileToBankWebhookSecret { get; set; }
    }
}
