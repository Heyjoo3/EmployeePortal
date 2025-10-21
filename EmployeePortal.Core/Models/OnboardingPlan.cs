using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Core.Models
{
    [Table("OnboardingPlans")]
    public class OnboardingPlan
    {
        [Key]
        public Guid OnbardingId { get; set; }
        public Guid EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public Status Status { get; set; } = Status.Pending;
        public List<TaskGroup>? taskGroups { get; set; }
        //public Template? AdminTemplate { get; set; }
        //public Template? DepartmentTemplate { get; set; }

    } 
}
