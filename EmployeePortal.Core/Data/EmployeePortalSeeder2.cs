using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Models;
using EmployeePortal.Core.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeePortal.Core.Data
{
    public static class EmployeePortalSeeder2
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            var roles = new List<string> { "Admin", "Employee", "Supervisor" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public static async Task SeedPublicHolidays(IPublicHolidayService publicHolidayService)
        {
            var existingHolidays = await publicHolidayService.GetAllHolidaysByYear(new DateOnly().Year);

            if (existingHolidays == null) { 

                var holidays = new List<PublicHolidayDto>
                {
                    new PublicHolidayDto { Date = DateTime.Parse("2024-01-01T00:00:00.000+01:00"), Day = "Montag", Type = "Neujahr" },
                    new PublicHolidayDto { Date = DateTime.Parse("2024-04-01T00:00:00.000+02:00"), Day = "Montag", Type = "Ostermontag" },
                    new PublicHolidayDto { Date = DateTime.Parse("2024-05-01T00:00:00.000+02:00"), Day = "Dienstag", Type = "Tag der Arbeit" },
                    new PublicHolidayDto { Date = DateTime.Parse("2024-05-09T00:00:00.000+02:00"), Day = "Donnerstag", Type = "Christi Himmelfahrt" },
                    new PublicHolidayDto { Date = DateTime.Parse("2024-05-20T00:00:00.000+02:00"), Day = "Montag", Type = "Pfingstmontag" },
                    new PublicHolidayDto { Date = DateTime.Parse("2024-10-03T00:00:00.000+02:00"), Day = "Donnerstag", Type = "Tag der Deutschen Einheit" },
                    new PublicHolidayDto { Date = DateTime.Parse("2024-10-31T00:00:00.000+01:00"), Day = "Donnerstag", Type = "Reformationstag" },
                    new PublicHolidayDto { Date = DateTime.Parse("2024-11-01T00:00:00.000+01:00"), Day = "Freitag", Type = "Allerheiligen" },
                    new PublicHolidayDto { Date = DateTime.Parse("2024-12-25T00:00:00.000+01:00"), Day = "Dienstag", Type = "1. Weihnachtstag" },
                    new PublicHolidayDto { Date = DateTime.Parse("2024-12-26T00:00:00.000+01:00"), Day = "Mittwoch", Type = "2. Weihnachtstag" }
                };

                for (int i = 0; i < holidays.Count; i++)
                {
                    var holidayCreateDto = holidays[i];
                    await publicHolidayService.CreateHoliday(holidayCreateDto);
                }
            }
        }

        public static async Task SeedCompanyVacation (IVacationService vacationService)
        {
            var vacations = await vacationService.GetAllVacations();
            var companyVac = vacations.FirstOrDefault(v => v.EmployeeId == null);

            if (companyVac == null) { 

                var random = new Random(); 
                var companyVacations = new List<VacationDto>
                {
                    new VacationDto
                    {
                        StartDate = new DateTime(2024, 12, 24) ,
                        EndDate = new DateTime (2024, 12, 31),
                        IsHalfStartDay = true ,
                        IsHalfEndDay = true ,
                        Vacationdays = 0,
                        Supervisor = null, 
                        Substitute = null, 
                        EmployeeId = null, 
                        Type = "Betriebsurlaub",
                        SupervisorStatus = VacationStatus.VacationApproved,
                        AdminStatus = VacationStatus.VacationApproved,
                        SubstituteStatus = VacationStatus.VacationApproved,
                        Status = VacationStatus.VacationApproved,
                        EmployeeStatus = VacationStatus.NotRelevant,

                    }
                };

                foreach (var vaccation in companyVacations)
                {
                    await vacationService.CreateVacation(vaccation);
                }

            }
        }

        public static async Task SeedDatabaseAsync(UserManager<Employee> userManager, RoleManager<IdentityRole> roleManager, IVacationService vacationService, IEmployeeService employeeService, IAbsenceService absenceService)
        {
            var roles = new List<string> { "Admin", "Employee", "Supervisor" };
            var departments = new[] {
                "Suite", "Suite+", "Services", "Product Management", "IT", "Consulting",
                "Geschäftsführung", "Administration", "Sales", "Finance", "HR", "Management"
            };
            var random = new Random();


            // Seed roles
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Create sample employees
            for (int i = 1; i <= 40; i++)
            {
                var employee = new Employee
                {
                    UserName = $"employee{i}",
                    Email = $"employee{i}@example.com",
                    FirstName = $"FirstName{i}",
                    LastName = $"LastName{i}",
                    DateofBirth = new DateTime(random.Next(1950, 2000), random.Next(1, 12), random.Next(1, 28)),
                    EmployeeInternalId = i + 1000,
                    Salutation = (i % 2 == 0) ? SalutationType.Female : SalutationType.Male,
                    Role = (i <= 3) ? "Admin" : (i <= 8) ? "Supervisor" : "Employee",
                    AnnualVacation = random.Next(20, 30),
                    Supervisor = null, 
                    Substitute = null, 
                    SickNoteDeadLine = random.Next(1, 4),
                    Department = departments.ElementAt(random.Next(0, departments.Length))

                };
                employee.PasswordHash = new PasswordHasher<Employee>().HashPassword(employee, "Password@123");

                var user = await userManager.FindByEmailAsync(employee.Email);
                if (user == null)
                {
                    var createResult = await userManager.CreateAsync(employee, "Password@123");
                    if (createResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(employee, employee.Role);

                        // Seed Vacations for the employee
                        //await SeedVacationsForEmployee(vacationService, employee.Id);
                        //// Seed Absences for the employee
                        await SeedAbsencesForEmployee(vacationService, absenceService, employee.Id);
                    }
                }
            }

            await SeedVacationsForEmployee(vacationService, employeeService);
        }

        private static async Task SeedVacationsForEmployee(IVacationService vacationService, IEmployeeService employeeService)
        {
            var vacations = await vacationService.GetAllVacations();

            if (vacations == null) { 

                var users = await employeeService.GetAllEmployees();
                var supervisors = users.Where(u => u.Role == "Supervisor");
                var random = new Random(); 

                for (int j = 0; j < users.Count(); j++)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        var startDate = RandomDateInYear(random, 2024);
                        var endDate = startDate.AddDays(random.Next(1, 14));
                        var possibleSub = users.ElementAt(random.Next(0, users.Count())).Id;
                        var substitute = possibleSub == users.ElementAt(j).Id ? null : possibleSub;

                        var vacationDto = new VacationDto
                        {
                            StartDate = startDate,
                            EndDate = endDate,
                            EmployeeId = Guid.Parse(users.ElementAt(j).Id),
                            Description = $"Vacation {i + 1} for Employee {users.ElementAt(j).EmployeeInternalId}",
                            Type = "Jahresurlaub", 
                            IsHalfStartDay = (j * i * random.Next() ) % 2 == 0 ? true : false,
                            IsHalfEndDay = (j * i * random.Next()) % 2 == 0 ? true : false,
                            Supervisor = supervisors.ElementAt(random.Next(0, supervisors.Count())).Id,
                            Substitute = substitute,
                            Vacationdays = 0,
                            Status = VacationStatus.VacationApproved, 
                            AdminStatus = VacationStatus.VacationApproved, 
                            SupervisorStatus = VacationStatus.VacationApproved, 
                            SubstituteStatus = VacationStatus.VacationApproved, 
                            EmployeeStatus = VacationStatus.NotRelevant,
                        };

                        await vacationService.CreateVacation(vacationDto);
                    }
                }
            }
        }

        private static DateTime RandomDateInYear(Random random, int year)
        {
            var month = random.Next(1, 13);
            var day = random.Next(1, DateTime.DaysInMonth(year, month) + 1);
            return new DateTime(year, month, day);
        }

        private static async Task SeedAbsencesForEmployee(IVacationService vacationService, IAbsenceService absenceService, string employeeId)
        {
            var absences = await absenceService.GetAllAbsences();

            if(absences == null) { 


                for (int i = 0; i < 5; i++)
                {
                    var random = new Random();
                    var startDate = RandomDateInYear(random, 2024);
                    var endDate = startDate.AddDays(2);

                    var absenceDto = new AbsenceDto
                    {
                        StartDate = startDate,
                        EndDate = endDate, 
                        EmployeeId = Guid.Parse( employeeId),
                        Duration = 2,
                        Remarks = $"Absence {i + 1} for Employee {employeeId}",
                        HasSickLeave = (i * random.Next())%2 == 0 ? true : false,
                        HasStartedShift = (i * random.Next()) % 2 == 0 ? true : false,
                        AbsenceType = AbsenceType.SickLeave,
                    };

                    await absenceService.CreateAbsence(absenceDto);
                }
            }
        }
    }
}
