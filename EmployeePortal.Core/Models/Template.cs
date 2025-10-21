using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Core.Models
{

    [Table("Templates")]
    public class Template
    {
        [Key]
        public Guid Id { get; set; }

        public string Title { get; set; }
        public Guid CreatedBy { get; set; }
        public Department Department { get; set; }
        public List<TaskGroup> TaskGroups { get; set; }
    }

    public enum Department
    {
        HR = 1,
        IT = 2,
        Sales = 3,
        Marketing = 4,
        Finance = 5,
        Admin = 6
    }

}

