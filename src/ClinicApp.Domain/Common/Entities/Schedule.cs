using ClinicApp.Domain.Common.ValueObjects;
using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApp.Domain.Common.Entities
{
    public class Schedule : Entity
    {
        public List<TimeRange> _calendar { get; private set; }
        public Schedule(Guid id) : base(id)
        {
        }

        internal ErrorOr<Success> CanBookTimeSlot(TimeRange newTime)
        {
            foreach (var existingtime in _calendar)
            {
                if (isOverlapping(existingtime, newTime))
                    return Error.Validation("Schedule.Conflict", $"Can't Book this time",new Dictionary<string, object>()
                    {
                        {"Conflicted",existingtime},
                        {"Exisiting",newTime }
                    });
            }
            return Result.Success;
        }

        private static bool isOverlapping(TimeRange t1, TimeRange t2) => t2.StartTime < t1.EndTime && t1.StartTime < t2.EndTime;
    }
}
