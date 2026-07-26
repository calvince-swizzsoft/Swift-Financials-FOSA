using System;
using System.Configuration;

namespace DistributedServices.MainBoundedContext
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            var configCode = default(int);

            if (int.TryParse(ConfigurationManager.AppSettings["ConfigCode"], out configCode))
            {
                if (configCode == 3040)
                {
                    SecurityConfig.ProtectConfiguration();
                }
                else if (configCode == 4030)
                {
                    SecurityConfig.UnProtectConfiguration();
                }
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}