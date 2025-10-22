using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Core.Models
{

        [Table("TaskGroups")]
        public class TaskGroup
        {
            [Key]
            public Guid? Id { get; set; }
            public string Title { get; set; }
            public Guid? OnboardingPlanId { get; set; }
            public OnboardingPlan? OnboardingPlan { get; set; }
            public List<BaseTask>? Tasks { get; set; }
            public string? ReferencePerson { get; set; }
            [ForeignKey(nameof(ReferencePerson))]
            public Employee? ReferenceEmployee { get; set; } // Navigation property
    }
}
