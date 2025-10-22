using EmployeePortal.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Core.Models
{
    public class ProjectTask: BaseTask, ISchedulable
    {
        public string? Description { get; set; }
        public Priority Priority { get; set; }
        public string? Tools { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

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

    public enum Priority
    {
        Low,
        Medium,
        High
    }
}
