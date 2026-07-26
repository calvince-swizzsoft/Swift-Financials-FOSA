using System.Configuration;
using System.Web.Configuration;

namespace DistributedServices.MainBoundedContext
{
    public class SecurityConfig
    {
        public static void ProtectConfiguration()
        {
            // Get the application configuration file.
            Configuration config = WebConfigurationManager.OpenWebConfiguration("~");

            // Define the Dpapi provider name.
            string provider = "DataProtectionConfigurationProvider";

            // Get the section to protect - connStrings.
            ConfigurationSection connStrings = config.ConnectionStrings;

            if (connStrings != null)
            {
                if (!connStrings.SectionInformation.IsProtected)
                {
                    if (!connStrings.ElementInformation.IsLocked)
                    {
                        // Protect the section.
                        connStrings.SectionInformation.ProtectSection(provider);

                        connStrings.SectionInformation.ForceSave = true;
                        config.Save(ConfigurationSaveMode.Full);
                    }
                }
            }

            // Get the section to protect - appSettings.
            //ConfigurationSection appSettings = config.GetSection("appSettings");

            //if (appSettings != null)
            //{
            //    if (!appSettings.SectionInformation.IsProtected)
            //    {
            //        if (!appSettings.ElementInformation.IsLocked)
            //        {
            //            // Protect the section.
            //            appSettings.SectionInformation.ProtectSection(provider);

            //            appSettings.SectionInformation.ForceSave = true;
            //            config.Save(ConfigurationSaveMode.Full);
            //        }
            //    }
            //}
        }

        public static void UnProtectConfiguration()
        {
            // Get the application configuration file.
            Configuration config = WebConfigurationManager.OpenWebConfiguration("~");

            // Get the section to unprotect - connStrings.
            ConfigurationSection connStrings = config.ConnectionStrings;

            if (connStrings != null)
            {
                if (connStrings.SectionInformation.IsProtected)
                {
                    if (!connStrings.ElementInformation.IsLocked)
                    {
                        // Unprotect the section.
                        connStrings.SectionInformation.UnprotectSection();

                        connStrings.SectionInformation.ForceSave = true;
                        config.Save(ConfigurationSaveMode.Full);
                    }
                }
            }

            // Get the section to unprotect - appSettings.
            ConfigurationSection appSettings = config.GetSection("appSettings");

            if (appSettings != null)
            {
                if (appSettings.SectionInformation.IsProtected)
                {
                    if (!appSettings.ElementInformation.IsLocked)
                    {
                        // Unprotect the section.
                        appSettings.SectionInformation.UnprotectSection();

                        appSettings.SectionInformation.ForceSave = true;
                        config.Save(ConfigurationSaveMode.Full);
                    }
                }
            }
        }
    }
}