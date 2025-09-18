using ErrorOr;

namespace ClinicApp.Application.Common
{
    public static partial class Errors
    {
        public static class General
        {
            public static Error NotFound => Error.NotFound(
                code: "General.NotFound",
                description: "The requested resource was not found.");
        }

        public static class Doctor
        {
            public static Error CreateFailed => Error.Unexpected(
                code: "Doctor.CreateFailed",
                description: "Something went wrong and the doctor could not be created.");
        }

        public static class Room
        {
            public static Error CreateFailed => Error.Unexpected(
                code: "Room.CreateFailed",
                description: "Something went wrong and the room could not be created.");
        }
    }
}