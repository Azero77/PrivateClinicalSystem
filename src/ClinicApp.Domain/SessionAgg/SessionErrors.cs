using ErrorOr;

namespace ClinicApp.Domain.SessionAgg
{
    public static class SessionErrors
    {
        public static class SessionTimeValidationError
        {
            public const string code = "Session.SessionTimeValidationError";
            public const string message = "You can not put Start Time After End Time";
            public static Error error = Error.Validation(code, message);
        }


        public static class SessionTimeInThePast
        {

            public const string code = "Session.SessionTimeValidationError";
            public const string message = "Session Time In the past";
            public static Error error = Error.Validation(code, message);
        }

        public static Error CantDeleteADeletedSession => Error.Validation(
            code: "Session.Validation",
            description: "Can't Delete a deleted session");

        public static Error CantStartADeletedSession => Error.Validation(
            code: "Session.Validation",
            description: "Can't Start a deleted session");

        public static Error CantFinishADeletedSession => Error.Validation(
            code: "Session.Validation",
            description: "Can't Finish a deleted session");

        public static Error CantRejectADeletedOrFinishedSession => Error.Validation(
            code: "Session.Validation",
            description: "Can't Reject a deleted or finished session");

        public static Error CantRejectASessionInTheFuture => Error.Validation(
            code: "Session.Validation",
            description: "Can't Reject a session in the future");

        public static Error CantUpdateDeletedSessions => Error.Validation(
            "Session.SessionDeletionError",
            "Can't Update Deleted Sessions");

        public static Error CantUpdateFinishedSessions => Error.Validation(
            "Session.SessionDeletionError",
            "Can't Update Finished Sessions");

        public static Error CantCreateSessionWithUserRole => Error.Validation("Session.Create", "Can't Create Session if the user role is not doctor,admin or secretary");

    }
}
