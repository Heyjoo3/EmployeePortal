using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Core.Dto
{
    public class PublicHolidayDto
    {
      
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Day { get; set; }
        public string Type { get; set; }
    }
}
