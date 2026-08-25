namespace WebApplication1.ApiErrors
{
    public static class ErrorCodes
    {
        public const string InvalidRequest = "INVALID_REQUEST";
        public const string ValidationFailed = "VALIDATION_FAILED";
        public const string AuthenticationRequired = "AUTHENTICATION_REQUIRED";
        public const string InvalidCredentials = "INVALID_CREDENTIALS";
        public const string AccessDenied = "ACCESS_DENIED";
        public const string ResourceNotFound = "RESOURCE_NOT_FOUND";
        public const string ResourceConflict = "RESOURCE_CONFLICT";
        public const string DependencyUnavailable = "DEPENDENCY_UNAVAILABLE";
        public const string InternalError = "INTERNAL_ERROR";
        public const string InitialPasswordChangeNotAllowed = "INITIAL_PASSWORD_CHANGE_NOT_ALLOWED";
        public const string PasswordChangeFailed = "PASSWORD_CHANGE_FAILED";
        public const string PasswordChangeOutcomeUnknown = "PASSWORD_CHANGE_OUTCOME_UNKNOWN";
    }
}
