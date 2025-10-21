using EmployeePortal.Core.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EmployeePortal.Core.Models
{
    [Table("Absences")]
    public class Absence
    {
        [Key]
        public Guid AbsenceId { get; set; }


        [ESLog("l_absenceData.absenceType")]
        public AbsenceType AbsenceType { get; set; }
        [ESLog("l_absenceData.startDate")]
        public DateTime StartDate { get; set; }
        [ESLog("l_absenceData.endDate")]
        public DateTime EndDate { get; set; }
        [ESLog("l_absenceData.duration")]
        public float Duration { get; set; }
        [ESLog("l_absenceData.remarks")]
        public string? Remarks { get; set; }
        [ESLog("l_absenceData.hasSickLeave")]
        public bool? HasSickLeave { get; set; }
        [ESLog("l_absenceData.hasStartedShift")]
        public bool? HasStartedShift { get; set; }
        public bool IsCredited { get; set; }


        [ForeignKey("EmployeeId")]
        //public Employee Employee { get; set; }

        public Guid? EmployeeId { get; set; }
    }

    public enum AbsenceType
    {
        Other = 1,
        SickLeave = 2,
        Accident = 3,
        Training = 4,
        ParentalLeave = 5,
        ChildCare = 6,
    }
}
