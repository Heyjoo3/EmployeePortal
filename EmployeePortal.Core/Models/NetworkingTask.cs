using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Core.Models
{
    public class NetworkingTask : BaseTask
    {
        public string? Location { get; set; }
        public DateTime? StartDate { get; set; }
        public string? ExampleQuestions { get; set; }
    }
}
