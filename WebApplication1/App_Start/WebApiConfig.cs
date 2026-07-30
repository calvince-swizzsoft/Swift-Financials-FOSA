using System;
using System.Collections.Generic;
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

            var cors = new EnableCorsAttribute(
            "http://localhost:5173",
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
