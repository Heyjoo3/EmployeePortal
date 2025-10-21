using EmployeePortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Core.Dto
{
    public class RequestDto
    {
        public Guid VacationId { get; set; }
        public string VacationType { get; set; }
        public DateTime StartDate { get; set; }
        public VacationStatus VacationStatus { get; set; }
        public DateTime EndDate { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }
        public long EmployeeInternalId { get; set; }
        public string Relation { get; set; }
    }
}
