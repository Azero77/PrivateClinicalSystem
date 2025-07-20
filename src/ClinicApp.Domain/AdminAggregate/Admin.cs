using ClinicApp.Domain.Common;
using ClinicApp.Domain.Doctor;
using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApp.Domain.AdminAggregate
{
    public class Admin : AggregateRoot
    {
        public ErrorOr<Updated> ChangeDoctorWorktime(Doctor.Doctor doctor,
            WorkingHours hours,
            WorkingDays days)
        {
        }


    }
}
