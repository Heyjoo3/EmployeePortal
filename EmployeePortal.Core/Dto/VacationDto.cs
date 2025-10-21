using EmployeePortal.Core.Models;

namespace EmployeePortal.Core.Dto
{
    public class VacationDto
    {        
        public Guid VacationId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsHalfStartDay { get; set; }        
        public bool IsHalfEndDay { get; set; }
        public string? Supervisor { get; set; }
        public string? Substitute { get; set; }
        public string? Type { get; set; }
        public VacationStatus Status { get; set; }
        public VacationStatus? EmployeeStatus { get; set; }
        public VacationStatus? SupervisorStatus { get; set; }
        public VacationStatus? SubstituteStatus { get; set; }
        public VacationStatus? AdminStatus { get; set; }
        public float Vacationdays { get; set; }
        public string? Description { get; set; }
        public Guid? EmployeeId { get; set; }
    }
}
