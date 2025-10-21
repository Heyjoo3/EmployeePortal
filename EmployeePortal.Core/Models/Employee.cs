using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EmployeePortal.Core.Helpers;
using Microsoft.AspNetCore.Identity;

namespace EmployeePortal.Core.Models
{
    [Table("Employees")]
    public class Employee: IdentityUser<string>
    {

        [ESLog("l_employeeData.salutation")]
        public SalutationType Salutation { get; set; }

        [ESLog("l_employeeData.employeeInternalId")]
        public long? EmployeeInternalId { get; set; }

        [ESLog("l_employeeData.firstName")]
        public string? FirstName { get; set; }

        [ESLog("l_employeeData.lastName")]
        public string? LastName { get; set; }

        [ESLog("l_employeeData.dateofBirth")]
        public DateTime DateofBirth { get; set; }

        [ESLog("l_employeeData.department")]
        public string? Department { get; set; }

        [ESLog("l_employeeData.role")]
        public string? Role { get; set; }

        [ESLog("l_employeeData.supervisor")]
        public string? Supervisor { get; set; }

        [ESLog("l_employeeData.substitute")]
        public string? Substitute { get; set; }

        //[ESLog("l_employeeData.onbaording")]
        //public OnboardingPlan? OnboardingPlan { get; set; }

        //[ESLog("l_employeeData.onbaording")]
        //public Guid? OnboardingPlanId { get; set; }

        [ESLog("l_employeeData.annualVacation")]
        public int AnnualVacation { get; set; }

        [ESLog("l_employeeData.sickNoteDeadline")]
        public int? SickNoteDeadLine { get; set; }

        public bool ForceNewPassword { get; set; } 

        public Employee()
        {
            Id = Guid.NewGuid().ToString();
        }
    }

    public enum SalutationType
    {
        [ESLog("l_employeeData.salutationTypes.male")]
        Male = 1,
        [ESLog("l_employeeData.salutationTypes.female")]
        Female = 2,
        [ESLog("l_employeeData.salutationTypes.diverse")]
        Diverse = 3,
    }

  
}
