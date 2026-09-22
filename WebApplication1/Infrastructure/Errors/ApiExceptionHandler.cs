using Application.Seedwork;
using Application.MainBoundedContext.HumanResourcesModule.Services;
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
            var payrollSetup = exception as PayrollSetupException;
            if (payrollSetup != null)
                return ApiErrorResponses.Create(request, HttpStatusCode.Conflict,
                    "PAYROLL_SETUP_REQUIRED", payrollSetup.Message);

            var authorityFailure = exception as TransactionAuthorityException;
            if (authorityFailure != null)
            {
                return ApiErrorResponses.Create(request, HttpStatusCode.Forbidden,
                    "TRANSACTION_AUTHORITY_DENIED", authorityFailure.Message);
            }

            var makerCheckerViolation = exception as MakerCheckerViolationException;
            if (makerCheckerViolation != null)
            {
                return ApiErrorResponses.Create(request, HttpStatusCode.Conflict,
                    ErrorCodes.MakerCheckerViolation, makerCheckerViolation.Message);
            }

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
