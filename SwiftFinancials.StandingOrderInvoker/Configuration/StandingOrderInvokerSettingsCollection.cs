using System.Configuration;

namespace SwiftFinancials.StandingOrderInvoker.Configuration
{
    public class StandingOrderInvokerSettingsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new StandingOrderInvokerSettingsElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((StandingOrderInvokerSettingsElement)(element)).UniqueId;
        }

        public StandingOrderInvokerSettingsElement this[int index]
        {
            get { return (StandingOrderInvokerSettingsElement)BaseGet(index); }
        }


        [ConfigurationProperty("name", IsRequired = false)]
        public string Name
        {
            get
            {
                return (string)base["name"];
            }
        }

        [ConfigurationProperty("standingOrderJobCronExpression", IsRequired = true)]
        public string StandingOrderJobCronExpression
        {
            get
            {
                return (string)base["standingOrderJobCronExpression"];
            }
        }

        [ConfigurationProperty("skippedStandingOrderJobCronExpression", IsRequired = true)]
        public string SkippedStandingOrderJobCronExpression
        {
            get
            {
                return (string)base["skippedStandingOrderJobCronExpression"];
            }
        }

        [ConfigurationProperty("sweepingStandingOrderJobCronExpression", IsRequired = true)]
        public string SweepingStandingOrderJobCronExpression
        {
            get
            {
                return (string)base["sweepingStandingOrderJobCronExpression"];
            }
        }
    }
}
