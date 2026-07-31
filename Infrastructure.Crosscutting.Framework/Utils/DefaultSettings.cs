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
    }
}
