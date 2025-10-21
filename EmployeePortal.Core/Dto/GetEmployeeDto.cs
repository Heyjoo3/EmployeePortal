using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Core.Dto
{
    public class GetEmployeeDto
    {
        public Guid Id { get; set; }
        public long? EmployeeInternalId { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string Role { get; set; }
    }
}
