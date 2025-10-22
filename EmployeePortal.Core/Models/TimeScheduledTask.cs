using EmployeePortal.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Core.Models
{
    public class TimeScheduledTask : BaseTask, ISchedulable
    {
        public DateTime?StartDate { get; set; }
        public DateTime?   EndDate { get; set; }
        public string? Description { get; set; }

        public Task<TimeSpan?> CalculateDuration()
        {
            if (!StartDate.HasValue || !EndDate.HasValue)
            {
                return Task.FromResult<TimeSpan?>(null);
            }

            TimeSpan duration = EndDate.Value - StartDate.Value;
            return Task.FromResult<TimeSpan?>(duration);
        }
    }
}

