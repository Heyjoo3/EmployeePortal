using EmployeePortal.Core.Models;

namespace EmployeePortal.Core.Dto
{
    public class AbsenceDto
    {
        public Guid AbsenceId { get; set; }
    
        public DateTime StartDate { get; set; }
    
        public DateTime EndDate { get; set; }
   
        public AbsenceType? AbsenceType { get; set; }

        public float Duration { get; set; }

        public string? Remarks { get; set; }

        public bool? HasSickLeave { get; set; }

        public bool? HasStartedShift { get; set; }

        public bool IsCredited { get; set; }

        public Guid? EmployeeId { get; set; }

    }
}
