using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Core.Models
{
    [Table("Tasks")]
    public abstract class BaseTask
    {
        [Key]
        public Guid Id { get; set; }

        public string Title { get; set; }
        public Status Status { get; set; } = Status.Open;
        public string TaskType { get; set; }

        public Guid? TaskGroupId { get; set; }
        public TaskGroup? TaskGroup { get; set; }

        public string? ReferencePerson { get; set; }
        [ForeignKey(nameof(ReferencePerson))]
        public Employee? ReferenceEmployee { get; set; }


    }

    public enum Status
    {
        Open = 1,
        InProgress = 2,
        Done = 3,
    }
}

