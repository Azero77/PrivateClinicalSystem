using ErrorOr;

namespace ClinicApp.Domain.AdminAggregate
{
    public static class AdminErrors
    {
        public static Error RoomAlreadyExists => Error.Validation(
            code: "Admin.RoomAlreadyExists",
            description: "Can't add a room that is already in the list.");

        public static Error RoomNotFound => Error.Validation(
            code: "Admin.RoomNotFound",
            description: "The specified room was not found.");
    }
}
