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
        public Status Status { get; set; }
        public string TaskType { get; set; }

        public Guid TaskGroupId { get; set; }
        public TaskGroup TaskGroup { get; set; }

    }

    public enum Status
    {
        Pending = 1,
        InProgress = 2,
        Completed = 3,
    }

    public enum TaskType
    {
        Todo = 1,
        Networking = 2,
    }
}

