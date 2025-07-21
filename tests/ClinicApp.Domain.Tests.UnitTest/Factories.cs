using ClinicApp.Domain.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApp.Domain.Tests.UnitTest
{
    public static class Factories
    {
        public static Doctor.Doctor DoctorFactory() => new Doctor.Doctor(Guid.NewGuid(),
                WorkingDays.Monday | WorkingDays.Wednesday,
                WorkingHours.Create(new TimeOnly(9, 0), new TimeOnly(17, 0)));
    }
}
