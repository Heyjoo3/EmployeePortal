using EmployeePortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Core.Dto
{
    public class OnboardingPlanDto
    {
        public Guid? OnboardingId { get; set; }
        public List<TaskGroupDto>? TaskGroups { get; set; }
        public string? EmployeeId { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Status Status { get; set; } = Status.Open;
        public string? ReferencePerson { get; set; }
        public int? Progress { get; set; }

    }
}
