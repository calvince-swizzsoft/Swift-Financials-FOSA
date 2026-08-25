using System;
using System.Collections.Generic;
using System.Net;

namespace WebApplication1.ApiErrors
{
    public class ApiException : Exception
    {
        public ApiException(HttpStatusCode statusCode, string errorCode, string safeMessage,
            IDictionary<string, string[]> validationErrors = null, Exception innerException = null)
            : base(safeMessage, innerException)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
            SafeMessage = safeMessage;
            ValidationErrors = validationErrors;
        }

        public HttpStatusCode StatusCode { get; private set; }
        public string ErrorCode { get; private set; }
        public string SafeMessage { get; private set; }
        public IDictionary<string, string[]> ValidationErrors { get; private set; }
    }

    public sealed class ResourceNotFoundException : ApiException
    {
        public ResourceNotFoundException(string message, string errorCode = ErrorCodes.ResourceNotFound)
            : base(HttpStatusCode.NotFound, errorCode, message) { }
    }

    public sealed class BusinessConflictException : ApiException
    {
        public BusinessConflictException(string errorCode, string message)
            : base(HttpStatusCode.Conflict, errorCode, message) { }
    }

    public sealed class DependencyUnavailableException : ApiException
    {
        public DependencyUnavailableException(string message, Exception innerException = null)
            : base(HttpStatusCode.ServiceUnavailable, ErrorCodes.DependencyUnavailable, message, null, innerException) { }
    }
}
