using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Models;
using EmployeePortal.Core.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Core.Data
{
    public static class EmployeePortalSeeder
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

        public static async Task SeedDatabaseAsync(UserManager<Employee> userManager, RoleManager<IdentityRole> roleManager)
        {
            var roles = new List<string> { "Admin", "Employee", "Supervisor" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var random = new Random();

            var employees = new List<Employee>
                {
                 new Employee
                {
                    UserName = "admin",
                    Email = "admin@contechnet.de",

                    FirstName = "Ada",
                    LastName = "Admin",
                    DateofBirth = new DateTime(random.Next(1950, 2000), random.Next(1, 12), random.Next(1, 28)),
                    EmployeeInternalId = 0001,
                    Salutation = SalutationType.Female,
                    Role = "Admin",
                    AnnualVacation = 30,
                    Supervisor = null,
                    Substitute = null,
                    SickNoteDeadLine = 2,
                    Department = "Administration"

                },
                new Employee
                {
                    UserName = "lmolitor",
                    Email = "lina.molitor@contechnet.de",

                    FirstName = "Lina",
                    LastName = "Molitor",
                    DateofBirth = new DateTime(random.Next(1950, 2000), random.Next(1, 12), random.Next(1, 28)),
                    EmployeeInternalId = 0002,
                    Salutation = SalutationType.Female,
                    Role = "Admin",
                    AnnualVacation = 30,
                    Supervisor = null,
                    Substitute = null,
                    SickNoteDeadLine = 2,
                    Department = "Administration"

                },
                new Employee
                {
                    UserName = "ibubolz",
                    Email = "iris.bubolz@contechnet.de",

                    FirstName = "Iris",
                    LastName = "Bubolz",
                    DateofBirth = new DateTime(random.Next(1950, 2000), random.Next(1, 12), random.Next(1, 28)),
                    EmployeeInternalId = 0003,
                    Salutation = SalutationType.Female,
                    Role = "Admin",
                    AnnualVacation = 30,
                    Supervisor = null,
                    Substitute = null,
                    SickNoteDeadLine = 2,
                    Department = "Administration"
                },
                new Employee
                {
                    UserName = "greimann",
                    Email = "georg.reimann@contechnet.de",

                    FirstName = "Georg",
                    LastName = "Reimann",
                    DateofBirth = new DateTime(random.Next(1950, 2000), random.Next(1, 12), random.Next(1, 28)),
                    EmployeeInternalId = 0004,
                    Salutation = SalutationType.Male,
                    Role = "Supervisor",
                    AnnualVacation = 30,
                    Supervisor = null,
                    Substitute = null,
                    SickNoteDeadLine = 2,
                    Department = "Geschäftsführung"
                },
                new Employee
                {
                    UserName = "rfinke",
                    Email = "rainer.finke@contechnet.de",

                    FirstName = "Rainer",
                    LastName = "Finke",
                    DateofBirth = new DateTime(random.Next(1950, 2000), random.Next(1, 12), random.Next(1, 28)),
                    EmployeeInternalId = 0005,
                    Salutation = SalutationType.Male,
                    Role = "Supervisor",
                    AnnualVacation = 30,
                    Supervisor = null,
                    Substitute = null,
                    SickNoteDeadLine = 2,
                    Department = "Geschäftsführung"
                },
                new Employee
                {
                    UserName = "lrost",
                    Email = "lisa.rost@contechnet.de",

                    FirstName = "Lisa",
                    LastName = "Rost",
                    DateofBirth = new DateTime(random.Next(1950, 2000), random.Next(1, 12), random.Next(1, 28)),
                    EmployeeInternalId = 0006,
                    Salutation = SalutationType.Female,
                    Role = "Supervisor",
                    AnnualVacation = 30,
                    Supervisor = null,
                    Substitute = null,
                    SickNoteDeadLine = 2,
                    Department = "Marketing"
                },
                new Employee
                {
                    UserName = "mfreye",
                    Email = "markus.freye@contechnet.de",

                    FirstName = "Markus",
                    LastName = "Freye",
                    DateofBirth = new DateTime(random.Next(1950, 2000), random.Next(1, 12), random.Next(1, 28)),
                    EmployeeInternalId = 0007,
                    Salutation = SalutationType.Male,
                    Role = "Employee",
                    AnnualVacation = 30,
                    Supervisor = null,
                    Substitute = null,
                    SickNoteDeadLine = 2,
                    Department = "Marketing"
                },
                new Employee
                {
                    UserName = "sschmuck",
                    Email = "sabrina.schmuck@contechnet.de",

                    FirstName = "Sabrina",
                    LastName = "Schmuck",
                    DateofBirth = new DateTime(random.Next(1950, 2000), random.Next(1, 12), random.Next(1, 28)),
                    EmployeeInternalId = 0008,
                    Salutation = SalutationType.Female,
                    Role = "Employee",
                    AnnualVacation = 30,
                    Supervisor = null,
                    Substitute = null,
                    SickNoteDeadLine = 2,
                    Department = "Marketing"
                },
                new Employee
                {
                    UserName = "croth",
                    Email = "christian.roth@contechnet.de",

                    FirstName = "Christian",
                    LastName = "Roth",
                    DateofBirth = new DateTime(random.Next(1950, 2000), random.Next(1, 12), random.Next(1, 28)),
                    EmployeeInternalId = 0009,
                    Salutation = SalutationType.Male,
                    Role = "Supervisor",
                    AnnualVacation = 30,
                    Supervisor = null,
                    Substitute = null,
                    SickNoteDeadLine = 2,
                    Department = "Sales"
                },
                new Employee
                {
                    UserName = "mhaenel",
                    Email = "mark.haenel@contechnet.de",

                    FirstName = "Mark",
                    LastName = "Haenel",
                    DateofBirth = new DateTime(random.Next(1950, 2000), random.Next(1, 12), random.Next(1, 28)),
                    EmployeeInternalId = 0010,
                    Salutation = SalutationType.Male,
                    Role = "Employee",
                    AnnualVacation = 30,
                    Supervisor = null,
                    Substitute = null,
                    SickNoteDeadLine = 2,
                    Department = "Sales"
                },
                new Employee
                {
                    UserName = "broesler",
                    Email = "bastian.roesler@contechnet.de",

                    FirstName = "Bastian",
                    LastName = "Roesler",
                    DateofBirth = new DateTime(random.Next(1950, 2000), random.Next(1, 12), random.Next(1, 28)),
                    EmployeeInternalId = 0011,
                    Salutation = SalutationType.Male,
                    Role = "Employee",
                    AnnualVacation = 30,
                    Supervisor = null,
                    Substitute = null,
                    SickNoteDeadLine = 2,
                    Department = "PreSales-Consulting"
                },
                new Employee
                {
                    UserName = "aseyedalikhani",
                    Email = "ali.seyedalikhani@contechnet.de",

                    FirstName = "Ali",
                    LastName = "Seyedalikhani",
                    DateofBirth = new DateTime(random.Next(1950, 2000), random.Next(1, 12), random.Next(1, 28)),
                    EmployeeInternalId = 0012,
                    Salutation = SalutationType.Male,
                    Role = "Supervisor",
                    AnnualVacation = 30,
                    Supervisor = null,
                    Substitute = null,
                    SickNoteDeadLine = 2,
                    Department = "Suite"
                },
                new Employee
                {
                    UserName = "aneumann",
                    Email = "axel.neumann@contechnet.de",

                    FirstName = "Axel",
                    LastName = "Neumann",
                    DateofBirth = new DateTime(random.Next(1950, 2000), random.Next(1, 12), random.Next(1, 28)),
                    EmployeeInternalId = 0013,
                    Salutation = SalutationType.Male,
                    Role = "Employee",
                    AnnualVacation = 30,
                    Supervisor = null,
                    Substitute = null,
                    SickNoteDeadLine = 2,
                    Department = "Suite"
                },
                new Employee
                {
                    UserName = "malegha",
                    Email = "mohammadreza.aleagha@contechnet.de",

                    FirstName = "Mohammadreza",
                    LastName = "Ale Agha",
                    DateofBirth = new DateTime(random.Next(1950, 2000), random.Next(1, 12), random.Next(1, 28)),
                    EmployeeInternalId = 0014,
                    Salutation = SalutationType.Male,
                    Role = "Employee",
                    AnnualVacation = 30,
                    Supervisor = null,
                    Substitute = null,
                    SickNoteDeadLine = 2,
                    Department = "Suite"
                },
                new Employee
                {
                    UserName = "malishah",
                    Email = "mostafa.alishah@contechnet.de",

                    FirstName = "Mostafa",
                    LastName = "Alishah",
                    DateofBirth = new DateTime(random.Next(1950, 2000), random.Next(1, 12), random.Next(1, 28)),
                    EmployeeInternalId = 0015,
                    Salutation = SalutationType.Male,
                    Role = "Employee",
                    AnnualVacation = 30,
                    Supervisor = null,
                    Substitute = null,
                    SickNoteDeadLine = 2,
                    Department = "Suite"
                },
                new Employee
                {
                    UserName = "aschneider",
                    Email = "anna.schneider@contechnet.de",

                    FirstName = "Anna",
                    LastName = "Schneider",
                    DateofBirth = new DateTime(random.Next(1950, 2000), random.Next(1, 12), random.Next(1, 28)),
                    EmployeeInternalId = 0016,
                    Salutation = SalutationType.Female,
                    Role = "Employee",
                    AnnualVacation = 30,
                    Supervisor = null,
                    Substitute = null,
                    SickNoteDeadLine = 2,
                    Department = "Suite"
                },
                new Employee
                {
                    UserName = "mkierstein",
                    Email = "martin.kierstein@contechnet.de",

                    FirstName = "Martin",
                    LastName = "Kierstein",
                    DateofBirth = new DateTime(random.Next(1950, 2000), random.Next(1, 12), random.Next(1, 28)),
                    EmployeeInternalId = 0017,
                    Salutation = SalutationType.Male,
                    Role = "Supervisor",
                    AnnualVacation = 30,
                    Supervisor = null,
                    Substitute = null,
                    SickNoteDeadLine = 2,
                    Department = "Product Management"
                },
                new Employee
                {
                    UserName = "rvonkries",
                    Email = "raphaela.vonkries@contechnet.de",

                    FirstName = "Raphaela",
                    LastName = "von Kries",
                    DateofBirth = new DateTime(random.Next(1950, 2000), random.Next(1, 12), random.Next(1, 28)),
                    EmployeeInternalId = 0018,
                    Salutation = SalutationType.Female,
                    Role = "Employee",
                    AnnualVacation = 30,
                    Supervisor = null,
                    Substitute = null,
                    SickNoteDeadLine = 2,
                    Department = "Product Management"
                },
                new Employee
                {
                    UserName = "schandgude",
                    Email = "sharanya.chandgude@contechnet.de",

                    FirstName = "Sharanya",
                    LastName = "Chandgude",
                    DateofBirth = new DateTime(random.Next(1950, 2000), random.Next(1, 12), random.Next(1, 28)),
                    EmployeeInternalId = 0019,
                    Salutation = SalutationType.Female,
                    Role = "Employee",
                    AnnualVacation = 30,
                    Supervisor = null,
                    Substitute = null,
                    SickNoteDeadLine = 2,
                    Department = "Product Management"
                },
                new Employee
                {
                    UserName = "rrana",
                    Email = "radhika.rana@contechnet.de",

                    FirstName = "Radhika",
                    LastName = "Rana",
                    DateofBirth = new DateTime(random.Next(1950, 2000), random.Next(1, 12), random.Next(1, 28)),
                    EmployeeInternalId = 0020,
                    Salutation = SalutationType.Female,
                    Role = "Employee",
                    AnnualVacation = 30,
                    Supervisor = null,
                    Substitute = null,
                    SickNoteDeadLine = 2,
                    Department = "Product Management"
                },
                new Employee
                {
                    UserName = "mhaladyn",
                    Email = "michal.haladyn@contechnet.de",

                    FirstName = "Michal",
                    LastName = "Haladyn",
                    DateofBirth = new DateTime(random.Next(1950, 2000), random.Next(1, 12), random.Next(1, 28)),
                    EmployeeInternalId = 0021,
                    Salutation = SalutationType.Male,
                    Role = "Supervisor",
                    AnnualVacation = 30,
                    Supervisor = null,
                    Substitute = null,
                    SickNoteDeadLine = 2,
                    Department = "Suite +"
                },
                new Employee
                {
                    UserName = "cguer",
                    Email = "cagatay.guer@contechnet.de",

                    FirstName = "Cagatay",
                    LastName = "Gür",
                    DateofBirth = new DateTime(random.Next(1950, 2000), random.Next(1, 12), random.Next(1, 28)),
                    EmployeeInternalId = 0022,
                    Salutation = SalutationType.Male,
                    Role = "Employee",
                    AnnualVacation = 30,
                    Supervisor = null,
                    Substitute = null,
                    SickNoteDeadLine = 2,
                    Department = "Suite +"
                },
                new Employee
                {
                    UserName = "srosa",
                    Email = "sofia.rosa@contechnet.de",

                    FirstName = "Sofia",
                    LastName = "Rosa",
                    DateofBirth = new DateTime(random.Next(1950, 2000), random.Next(1, 12), random.Next(1, 28)),
                    EmployeeInternalId = 0023,
                    Salutation = SalutationType.Female,
                    Role = "Employee",
                    AnnualVacation = 30,
                    Supervisor = null,
                    Substitute = null,
                    SickNoteDeadLine = 2,
                    Department = "Suite +"
                },
                new Employee
                {
                    UserName = "smullapudi",
                    Email = "sudheer.mullapudi@contechnet.de",

                    FirstName = "Sudheer",
                    LastName = "Mullapudi",
                    DateofBirth = new DateTime(random.Next(1950, 2000), random.Next(1, 12), random.Next(1, 28)),
                    EmployeeInternalId = 0024,
                    Salutation = SalutationType.Male,
                    Role = "Employee",
                    AnnualVacation = 30,
                    Supervisor = null,
                    Substitute = null,
                    SickNoteDeadLine = 2,
                    Department = "Suite +"
                },
                new Employee
                {
                    UserName = "atheodorou",
                    Email = "angelos.theodorou@contechnet.de",

                    FirstName = "Angelos",
                    LastName = "Theodorou",
                    DateofBirth = new DateTime(random.Next(1950, 2000), random.Next(1, 12), random.Next(1, 28)),
                    EmployeeInternalId = 0025,
                    Salutation = SalutationType.Male,
                    Role = "Employee",
                    AnnualVacation = 18,
                    Supervisor = null,
                    Substitute = null,
                    SickNoteDeadLine = 1,
                    Department = "Ausbildung"
                },
                new Employee
                {
                    UserName = "lheidland",
                    Email = "luca.heidland@contechnet.de",

                    FirstName = "Luca",
                    LastName = "Heidland",
                    DateofBirth = new DateTime(random.Next(1950, 2000), random.Next(1, 12), random.Next(1, 28)),
                    EmployeeInternalId = 0026,
                    Salutation = SalutationType.Male,
                    Role = "Employee",
                    AnnualVacation = 18,
                    Supervisor = null,
                    Substitute = null,
                    SickNoteDeadLine = 1,
                    Department = "Ausbildung"
                },
                new Employee
                {
                    UserName = "fkaplan",
                    Email = "fatih.kaplan@contechnet.de",

                    FirstName = "Fatih",
                    LastName = "Kaplan",
                    DateofBirth = new DateTime(random.Next(1950, 2000), random.Next(1, 12), random.Next(1, 28)),
                    EmployeeInternalId = 0027,
                    Salutation = SalutationType.Male,
                    Role = "Employee",
                    AnnualVacation = 18,
                    Supervisor = null,
                    Substitute = null,
                    SickNoteDeadLine = 1,
                    Department = "Ausbildung"
                },
                new Employee
                {
                    UserName = "fewers",
                    Email = "friederike.ewers@contechnet.de",

                    FirstName = "Friederike",
                    LastName = "Ewers",
                    DateofBirth = new DateTime(random.Next(1950, 2000), random.Next(1, 12), random.Next(1, 28)),
                    EmployeeInternalId = 0028,
                    Salutation = SalutationType.Female,
                    Role = "Employee",
                    AnnualVacation = 28,
                    Supervisor = null,
                    Substitute = null,
                    SickNoteDeadLine = 1,
                    Department = "Ausbildung"
                },
                new Employee
                {
                    UserName = "nschulz",
                    Email = "nico.schulz@contechnet.de",

                    FirstName = "Nico",
                    LastName = "Schulz",
                    DateofBirth = new DateTime(random.Next(1950, 2000), random.Next(1, 12), random.Next(1, 28)),
                    EmployeeInternalId = 0029,
                    Salutation = SalutationType.Male,
                    Role = "Employee",
                    AnnualVacation = 28,
                    Supervisor = null,
                    Substitute = null,
                    SickNoteDeadLine = 1,
                    Department = "Ausbildung"
                },
                new Employee
                {
                    UserName = "rzegham",
                    Email = "reshtin.zegham@contechnet.de",

                    FirstName = "Reshtin",
                    LastName = "Zegham",
                    DateofBirth = new DateTime(random.Next(1950, 2000), random.Next(1, 12), random.Next(1, 28)),
                    EmployeeInternalId = 0030,
                    Salutation = SalutationType.Male,
                    Role = "Employee",
                    AnnualVacation = 28,
                    Supervisor = null,
                    Substitute = null,
                    SickNoteDeadLine = 1,
                    Department = "Ausbildung"
                },
                new Employee
                {
                    UserName = "mschmidt",
                    Email = "marcel.schmidt@contechnet.de",

                    FirstName = "Marcel",
                    LastName = "Schmidt",
                    DateofBirth = new DateTime(random.Next(1950, 2000), random.Next(1, 12), random.Next(1, 28)),
                    EmployeeInternalId = 0031,
                    Salutation = SalutationType.Male,
                    Role = "Supervisor",
                    AnnualVacation = 30,
                    Supervisor = null,
                    Substitute = null,
                    SickNoteDeadLine = 2,
                    Department = "Inside Sales"
                },
                new Employee
                {
                    UserName = "hstoot",
                    Email = "heike.stoot@contechnet.de",

                    FirstName = "Heike",
                    LastName = "Stoot",
                    DateofBirth = new DateTime(random.Next(1950, 2000), random.Next(1, 12), random.Next(1, 28)),
                    EmployeeInternalId = 0032,
                    Salutation = SalutationType.Male,
                    Role = "Employee",
                    AnnualVacation = 30,
                    Supervisor = null,
                    Substitute = null,
                    SickNoteDeadLine = 2,
                    Department = "Inside Sales"
                },
                new Employee
                {
                    UserName = "mczekalla",
                    Email = "melissa.czekalla@contechnet.de",

                    FirstName = "Melissa",
                    LastName = "Czekalla",
                    DateofBirth = new DateTime(random.Next(1950, 2000), random.Next(1, 12), random.Next(1, 28)),
                    EmployeeInternalId = 0032,
                    Salutation = SalutationType.Male,
                    Role = "Employee",
                    AnnualVacation = 30,
                    Supervisor = null,
                    Substitute = null,
                    SickNoteDeadLine = 2,
                    Department = "Inside Sales"
                },
            };

            for (int i = 0; i < employees.Count; i++)
            {
                employees[i].PasswordHash = new PasswordHasher<Employee>().HashPassword(employees[i], "Password@123");
                var employee = await userManager.FindByEmailAsync(employees[i].Email);
                if (employee == null)
                {
                    var createUser = await userManager.CreateAsync(employees[i], "Password@123"); // Use a secure password
                    if (createUser.Succeeded)
                    {
                        await userManager.AddToRoleAsync(employees[i], employees[i].Role);
                    }
                }
            }
        }

        public static async Task SeedCompanyVacation(IVacationService vacationService)
        {
            var vacations = await vacationService.GetAllVacations();
            var companyVac = vacations.FirstOrDefault(v => v.EmployeeId == null);

            if (companyVac == null)
            {

                var random = new Random();
                var companyVacations = new List<VacationDto>
                {
                    new VacationDto
                    {
                        StartDate = new DateTime(2025, 12, 24) ,
                        EndDate = new DateTime (2025, 12, 31),
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
                    },
                    //new VacationDto
                    //{
                    //    StartDate = new DateTime(2026, 12, 24) ,
                    //    EndDate = new DateTime (2026, 12, 31),
                    //    IsHalfStartDay = true ,
                    //    IsHalfEndDay = true ,
                    //    Vacationdays = 0,
                    //    Supervisor = null,
                    //    Substitute = null,
                    //    EmployeeId = null,
                    //    Type = "Betriebsurlaub",
                    //    SupervisorStatus = VacationStatus.VacationApproved,
                    //    AdminStatus = VacationStatus.VacationApproved,
                    //    SubstituteStatus = VacationStatus.VacationApproved,
                    //    Status = VacationStatus.VacationApproved,
                    //    EmployeeStatus = VacationStatus.NotRelevant,
                    //},
                    //new VacationDto
                    //{
                    //    StartDate = new DateTime(2027, 12, 24) ,
                    //    EndDate = new DateTime (2027, 12, 31),
                    //    IsHalfStartDay = true ,
                    //    IsHalfEndDay = true ,
                    //    Vacationdays = 0,
                    //    Supervisor = null,
                    //    Substitute = null,
                    //    EmployeeId = null,
                    //    Type = "Betriebsurlaub",
                    //    SupervisorStatus = VacationStatus.VacationApproved,
                    //    AdminStatus = VacationStatus.VacationApproved,
                    //    SubstituteStatus = VacationStatus.VacationApproved,
                    //    Status = VacationStatus.VacationApproved,
                    //    EmployeeStatus = VacationStatus.NotRelevant,
                    //},
                };

                foreach (var vaccation in companyVacations)
                {
                    await vacationService.CreateVacation(vaccation);
                }

            }
        }

        public static async Task SeedPublicHolidays(IPublicHolidayService publicHolidayService)
        {
            var existingHolidays = await publicHolidayService.GetAllHolidaysByYear(2025);

            if (existingHolidays == null || existingHolidays.Count() == 0)
            {
                var holidays = new List<PublicHolidayDto>
                {

                    new PublicHolidayDto { Date = DateTime.Parse("2025-01-01T00:00:00.000+01:00"), Day = "Mittwoch", Type = "Neujahr" },
                    new PublicHolidayDto { Date = DateTime.Parse("2025-04-18T00:00:00.000+02:00"), Day = "Freitag", Type = "Karfreitag" },
                    new PublicHolidayDto { Date = DateTime.Parse("2025-04-20T00:00:00.000+02:00"), Day = "Sonntag", Type = "Ostersonntag" },
                    new PublicHolidayDto { Date = DateTime.Parse("2025-04-21T00:00:00.000+02:00"), Day = "Montag", Type = "Ostermontag" },
                    new PublicHolidayDto { Date = DateTime.Parse("2025-05-01T00:00:00.000+02:00"), Day = "Donnerstag", Type = "Tag der Arbeit" },
                    new PublicHolidayDto { Date = DateTime.Parse("2025-05-29T00:00:00.000+02:00"), Day = "Donnerstag", Type = "Christi Himmelfahrt" },
                    new PublicHolidayDto { Date = DateTime.Parse("2025-06-08T00:00:00.000+02:00"), Day = "Sonntag", Type = "Pfingstsonntag" },
                    new PublicHolidayDto { Date = DateTime.Parse("2025-06-09T00:00:00.000+02:00"), Day = "Montag", Type = "Pfingstmontag" },
                    new PublicHolidayDto { Date = DateTime.Parse("2025-10-03T00:00:00.000+02:00"), Day = "Freitag", Type = "Tag der Deutschen Einheit" },
                    new PublicHolidayDto { Date = DateTime.Parse("2025-10-31T00:00:00.000+01:00"), Day = "Freitag", Type = "Reformationstag" },
                    new PublicHolidayDto { Date = DateTime.Parse("2025-12-25T00:00:00.000+01:00"), Day = "Donnerstag", Type = "1. Weihnachtstag" },
                    new PublicHolidayDto { Date = DateTime.Parse("2025-12-26T00:00:00.000+01:00"), Day = "Freitag", Type = "2. Weihnachtstag" },

                    new PublicHolidayDto { Date = DateTime.Parse("2026-01-01T00:00:00.000+01:00"), Day = "Donnerstag", Type = "Neujahr" },
                    new PublicHolidayDto { Date = DateTime.Parse("2026-04-03T00:00:00.000+02:00"), Day = "Freitag", Type = "Karfreitag" },
                    new PublicHolidayDto { Date = DateTime.Parse("2026-04-05T00:00:00.000+02:00"), Day = "Sonntag", Type = "Ostersonntag" },
                    new PublicHolidayDto { Date = DateTime.Parse("2026-04-06T00:00:00.000+02:00"), Day = "Montag", Type = "Ostermontag" },
                    new PublicHolidayDto { Date = DateTime.Parse("2026-05-01T00:00:00.000+02:00"), Day = "Freitag", Type = "Tag der Arbeit" },
                    new PublicHolidayDto { Date = DateTime.Parse("2026-05-14T00:00:00.000+02:00"), Day = "Donnerstag", Type = "Christi Himmelfahrt" },
                    new PublicHolidayDto { Date = DateTime.Parse("2026-05-24T00:00:00.000+02:00"), Day = "Sonntag", Type = "Pfingstsonntag" },
                    new PublicHolidayDto { Date = DateTime.Parse("2026-05-25T00:00:00.000+02:00"), Day = "Montag", Type = "Pfingstmontag" },
                    new PublicHolidayDto { Date = DateTime.Parse("2026-10-03T00:00:00.000+02:00"), Day = "Samstag", Type = "Tag der Deutschen Einheit" },
                    new PublicHolidayDto { Date = DateTime.Parse("2026-10-31T00:00:00.000+01:00"), Day = "Samstag", Type = "Reformationstag" },
                    new PublicHolidayDto { Date = DateTime.Parse("2026-12-25T00:00:00.000+01:00"), Day = "Freitag", Type = "1. Weihnachtstag" },
                    new PublicHolidayDto { Date = DateTime.Parse("2026-12-26T00:00:00.000+01:00"), Day = "Samstag", Type = "2. Weihnachtstag" },

                    new PublicHolidayDto { Date = DateTime.Parse("2027-01-01T00:00:00.000+01:00"), Day = "Freitag", Type = "Neujahr" },
                    new PublicHolidayDto { Date = DateTime.Parse("2027-03-26T00:00:00.000+02:00"), Day = "Freitag", Type = "Karfreitag" },
                    new PublicHolidayDto { Date = DateTime.Parse("2027-03-28T00:00:00.000+02:00"), Day = "Sonntag", Type = "Ostersonntag" },
                    new PublicHolidayDto { Date = DateTime.Parse("2027-03-29T00:00:00.000+02:00"), Day = "Montag", Type = "Ostermontag" },
                    new PublicHolidayDto { Date = DateTime.Parse("2027-05-01T00:00:00.000+02:00"), Day = "Samstag", Type = "Tag der Arbeit" },
                    new PublicHolidayDto { Date = DateTime.Parse("2027-05-06T00:00:00.000+02:00"), Day = "Donnerstag", Type = "Christi Himmelfahrt" },
                    new PublicHolidayDto { Date = DateTime.Parse("2027-05-16T00:00:00.000+02:00"), Day = "Sonntag", Type = "Pfingstsonntag" },
                    new PublicHolidayDto { Date = DateTime.Parse("2027-05-17T00:00:00.000+02:00"), Day = "Montag", Type = "Pfingstmontag" },
                    new PublicHolidayDto { Date = DateTime.Parse("2027-10-03T00:00:00.000+02:00"), Day = "Sonntag", Type = "Tag der Deutschen Einheit" },
                    new PublicHolidayDto { Date = DateTime.Parse("2027-10-31T00:00:00.000+01:00"), Day = "Sonntag", Type = "Reformationstag" },
                    new PublicHolidayDto { Date = DateTime.Parse("2027-12-25T00:00:00.000+01:00"), Day = "Samstag", Type = "1. Weihnachtstag" },
                    new PublicHolidayDto { Date = DateTime.Parse("2027-12-26T00:00:00.000+01:00"), Day = "Sonntag", Type = "2. Weihnachtstag" }
                };          

                for (int i = 0; i < holidays.Count; i++)
                {
                    var holidayCreateDto = holidays[i];
                    await publicHolidayService.CreateHoliday(holidayCreateDto);
                }
            }
        }
    }
}
