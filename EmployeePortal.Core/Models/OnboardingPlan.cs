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
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Status Status { get; set; } = Status.Open;
        public List<TaskGroup>? TaskGroups { get; set; }
        //public Template? AdminTemplate { get; set; }
        //public Template? DepartmentTemplate { get; set; }
        public string? ReferencePerson { get; set; }
        [ForeignKey(nameof(ReferencePerson))]
        public Employee? ReferenceEmployee { get; set; }

    } 
}
