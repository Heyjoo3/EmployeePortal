using EmployeePortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Core.Dto
{
    public class TaskDto
    {

        public string? Id { get; set; } 
        public string Title { get; set; }
        public string? TaskType { get; set; } 
        public Guid? TaskGroupId { get; set; }
        public Status Status { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ExampleQuestions { get; set; }
        public Priority? Priority { get; set; }
        public string? Tools { get; set; }
        public string? ReferencePerson { get; set; }
        public TimeSpan? Duration { get; set; }




    }
}
