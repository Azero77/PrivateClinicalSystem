using ErrorOr;

namespace ClinicApp.Domain.Session
{
    public static class SessionErrors
    {
        public static class SessionTimeValidationError
        {
            public const string code = "Session.SessionTimeValidationError";
            public const string message = "You can not put Start Time After End Time";
            public static Error error = Error.Validation(code, message);
        }

        public const string OverlappingSessionValidationError = "Session.OverlappingValidationError";
    }
}
