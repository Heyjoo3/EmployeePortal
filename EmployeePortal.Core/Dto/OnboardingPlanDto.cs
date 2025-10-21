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
        public string? Title { get; set; }
        public List<TaskGroupDto>? TaskGroups { get; set; }
        public Guid? EmployeeId { get; set; }
        }
}
