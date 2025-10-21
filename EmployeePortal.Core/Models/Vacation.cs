using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using EmployeePortal.Core.Helpers;

namespace EmployeePortal.Core.Models
{
    [Table("Vacations")]
    public class Vacation
    {
        
        [Key]
        public Guid VacationId { get; set; }

        [ESLog("l_vacationData.startDate")]
        public DateTime StartDate { get; set; }
        [ESLog("l_vacationData.endDate")]
        public DateTime EndDate { get; set; }
        [ESLog("l_vacationData.isHalfStartDay")]
        public bool IsHalfStartDay { get; set; }
        [ESLog("l_vacationData.isHalfEndDay")]
        public bool IsHalfEndDay { get; set;}

        [ESLog("l_vacationData.supervisor")]
        public string? Supervisor { get; set; }

        [ESLog("l_vacationData.substitute")]
        public string? Substitute { get; set; }

        [ESLog("l_vacationData.description")]
        public string? Description { get; set; }

        [ESLog("l_vacationData.vacationDays")]
        public float Vacationdays { get; set; }
        [ESLog("l_vacationData.Type")]
        public string? Type { get; set; }
        [ESLog("l_vacationData.Status")]
        public VacationStatus Status { get; set; }
        [ESLog("l_vacationData.employeeStatus")]
        public VacationStatus EmployeeStatus { get; set; } = VacationStatus.NotRelevant;
        [ESLog("l_vacationData.supervisorStatus")]
        public VacationStatus SupervisorStatus { get; set; } = VacationStatus.VacationPending;
        [ESLog("l_vacationData.substituteStatus")]
        public VacationStatus SubstituteStatus { get; set; } = VacationStatus.VacationPending;
        [ESLog("l_vacationData.adminStatus")]
        public VacationStatus AdminStatus { get; set; } = VacationStatus.VacationPending;

        [ForeignKey("EmployeeId")]
        public Guid? EmployeeId { get; set; }

    }

    public enum VacationStatus
    {

        [ESLog("l_system.status.notRelevant")]
        NotRelevant = 0,

        [ESLog("l_system.status.vacationApproved")]
        VacationApproved = 1,
        [ESLog("l_system.status.vacationRejected")]
        VacationRejected = 2,
        [ESLog("l_system.status.vacationPending")]
        VacationPending = 3,

        [ESLog("l_system.status.cancellationApproved")]
        CancellationApproved = 4,
        [ESLog("l_system.status.cancellationRejected")]
        CancellationRejected = 5,
        [ESLog("l_system.status.cancellationPending")]
        CancellationPending = 6
    }
}
