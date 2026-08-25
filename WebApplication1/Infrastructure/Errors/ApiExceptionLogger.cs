using Infrastructure.Crosscutting.Framework.Logging;
using System.Net;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace WebApplication1.ApiErrors
{
    public sealed class ApiExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            if (context == null || context.Exception == null) return;

            var apiException = context.Exception as ApiException;
            if (apiException != null && apiException.StatusCode != HttpStatusCode.ServiceUnavailable &&
                apiException.StatusCode != HttpStatusCode.InternalServerError) return;

            var request = context.Request;
            var correlationId = CorrelationIdHandler.GetCorrelationId(request);
            var method = request != null && request.Method != null ? request.Method.Method : "UNKNOWN";
            var controllerContext = context.ExceptionContext != null
                ? context.ExceptionContext.ControllerContext
                : null;
            var routeData = controllerContext != null ? controllerContext.RouteData : null;
            var route = routeData != null && routeData.Route != null ? routeData.Route.RouteTemplate : "UNKNOWN";
            var errorCode = apiException != null ? apiException.ErrorCode : ErrorCodes.InternalError;
            var logger = LoggerFactory.CreateLog();

            if (logger != null)
            {
                logger.LogError(
                    "Unhandled API exception CorrelationId={CorrelationId} Method={Method} Route={Route} ErrorCode={ErrorCode}",
                    context.Exception, correlationId, method, route, errorCode);
                return;
            }

            System.Diagnostics.Trace.TraceError(
                "Unhandled API exception CorrelationId={0} Method={1} Route={2} ErrorCode={3} Exception={4}",
                correlationId, method, route, errorCode, context.Exception);
        }
    }
}
