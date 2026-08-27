using System.Net;
using System.Net.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace WebApplication1.ApiErrors
{
    public sealed class ApiExceptionHandler : ExceptionHandler
    {
        public override void Handle(ExceptionHandlerContext context)
        {
            context.Result = new ResponseMessageResult(CreateResponse(context.Request, context.Exception));
        }

        public static HttpResponseMessage CreateResponse(HttpRequestMessage request, System.Exception exception)
        {
            var apiException = exception as ApiException;

            if (apiException != null)
            {
                return ApiErrorResponses.Create(request, apiException.StatusCode,
                    apiException.ErrorCode, apiException.SafeMessage, apiException.ValidationErrors);
            }

            return ApiErrorResponses.Create(request, HttpStatusCode.InternalServerError,
                ErrorCodes.InternalError, "An unexpected error occurred.");
        }
    }
}
