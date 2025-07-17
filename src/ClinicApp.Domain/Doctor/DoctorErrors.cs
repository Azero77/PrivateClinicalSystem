using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApp.Domain.Doctor
{
    public static class DoctorErrors
    {
        public const string SessionValidationError = "Doctor.SessionOverlapping";
        public const string SessionOutOfWorkingDay = "Doctor.SessionOutOfWorkingDay";
    }
}
