namespace WebApplication1.ApiErrors
{
    public static class ErrorCodes
    {
        public const string InvalidRequest = "INVALID_REQUEST";
        public const string ValidationFailed = "VALIDATION_FAILED";
        public const string AuthenticationRequired = "AUTHENTICATION_REQUIRED";
        public const string InvalidCredentials = "INVALID_CREDENTIALS";
        public const string EmailNotConfirmed = "EMAIL_NOT_CONFIRMED";
        public const string InvalidEmailConfirmation = "INVALID_EMAIL_CONFIRMATION";
        public const string AccessDenied = "ACCESS_DENIED";
        public const string ResourceNotFound = "RESOURCE_NOT_FOUND";
        public const string ResourceConflict = "RESOURCE_CONFLICT";
        public const string DependencyUnavailable = "DEPENDENCY_UNAVAILABLE";
        public const string InternalError = "INTERNAL_ERROR";
        public const string MethodNotAllowed = "METHOD_NOT_ALLOWED";
        public const string NotAcceptable = "NOT_ACCEPTABLE";
        public const string PayloadTooLarge = "PAYLOAD_TOO_LARGE";
        public const string UnsupportedMediaType = "UNSUPPORTED_MEDIA_TYPE";
        public const string RateLimitExceeded = "RATE_LIMIT_EXCEEDED";
        public const string InitialPasswordChangeNotAllowed = "INITIAL_PASSWORD_CHANGE_NOT_ALLOWED";
        public const string PasswordChangeFailed = "PASSWORD_CHANGE_FAILED";
        public const string PasswordChangeOutcomeUnknown = "PASSWORD_CHANGE_OUTCOME_UNKNOWN";
        public const string UserCreateFailed = "USER_CREATE_FAILED";
        public const string UserUpdateFailed = "USER_UPDATE_FAILED";
        public const string PasswordResetFailed = "PASSWORD_RESET_FAILED";
        public const string RoleCreateFailed = "ROLE_CREATE_FAILED";
        public const string RoleAssignmentFailed = "ROLE_ASSIGNMENT_FAILED";
        public const string InvalidPermissionType = "INVALID_PERMISSION_TYPE";
        public const string WorkflowInvalidState = "WORKFLOW_INVALID_STATE";
        public const string WorkflowNotFinal = "WORKFLOW_NOT_FINAL";
        public const string WorkflowItemRequiresDetailedScreen = "WORKFLOW_ITEM_REQUIRES_DETAILED_SCREEN";
        public const string MakerCheckerViolation = "MAKER_CHECKER_VIOLATION";
    }
}
