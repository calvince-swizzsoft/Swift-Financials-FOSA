using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication1.Areas.Auth;

namespace WebApplication1
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Was hardcoded to the local Vite dev server origin only, which
            // silently blocks every deployed frontend (different host/port)
            // with no way to fix it short of a rebuild. Reads from Web.config
            // instead, defaulting to the same dev origin so local behavior is
            // unchanged; a deployment sets AllowedCorsOrigins directly in its
            // own Web.config (comma-separated for more than one origin).
            var allowedOrigins = ConfigurationManager.AppSettings["AllowedCorsOrigins"] ?? "http://localhost:5173";

            var cors = new EnableCorsAttribute(
            allowedOrigins,
            "*",
            "*");

            config.EnableCors(cors);

            // Web API configuration and services
            config.MessageHandlers.Add(new JwtAuthenticationHandler());

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
