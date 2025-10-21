using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Core.Dto
{
    public class TaskGroupDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public List<TaskDto> Tasks { get; set; }
    }
}
