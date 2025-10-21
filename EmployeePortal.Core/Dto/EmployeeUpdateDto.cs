using EmployeePortal.Core.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EmployeePortal.Core.Dto
{
    public class EmployeeUpdateDto : IdentityUser<string>
    {
        public SalutationType Salutation { get; set; }
        public long? EmployeeInternalId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime DateofBirth { get; set; }
        public string? Department { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }

        public string? Supervisor { get; set; }
        public string? Substitute { get; set; }
        public int? AnnualVacation { get; set; }
        public int? SickNoteDeadLine { get; set; }

    }
}