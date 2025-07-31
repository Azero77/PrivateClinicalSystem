using ErrorOr;
using System.Collections.Generic;

namespace ClinicApp.Domain.Services.Sessions
{
    public static class ScheduleErrors
    {
        public static Error SessionAlreadyExists => Error.Validation(
            code: "Schedule.SessionAlreadyExists",
            description: "Can't add a session that is already added.");

        public static Error ConflictingSession(object newSession, object existingSession) => Error.Conflict(
            code: "Schedule.Conflict",
            description: "Can't book this time due to a conflict.",
            metadata: new Dictionary<string, object>
            {
                { "Conflicted", newSession },
                { "Existing", existingSession }
            });
    }
}
