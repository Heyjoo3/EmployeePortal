using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Repositories;
using EmployeePortal.Core.Services;
using System.Runtime.InteropServices;

namespace EmployeePortal.Services
{
    public class SideNavService : ISideNavService
    {
        private readonly IEmployeeService _employeeService;
        private readonly IVacationService _vacationService;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IVacationRepository _vacationRepository;

        public SideNavService(IEmployeeService employeeService, IVacationService vacationService, IEmployeeRepository employeeRepository, IVacationRepository vacationRepository )
        {
            _employeeService = employeeService;
            _vacationService = vacationService;
            _employeeRepository = employeeRepository;
            _vacationRepository = vacationRepository;
        }

        public async Task<IEnumerable<SideNavDto>> GetAllEmployeesSortedByDepartment()
        {
            var employees = await _employeeRepository.GetEmployeesSortedByDepartment();
            var departments = employees.Select(e => e.Department).Distinct();

            var sideNavDtos = new List<SideNavDto>();
            foreach (var department in departments)
            {
                var employeesInDepartment = employees.Where(e => e.Department == department).OrderBy(e => e.FirstName)
            .ThenBy(e => e.LastName);
                var sideNavDto = new SideNavDto
                {
                    Department = department,
                    Employees = employeesInDepartment.Select(e => new SideNavEmployeeDto
                    {
                        Id = Guid.Parse(e.Id),
                        EmployeeInternalId = e.EmployeeInternalId,
                        Name = e.FirstName + " " + e.LastName,
                        Department = e.Department

                    })
                };
                sideNavDtos.Add(sideNavDto);
            }
            return sideNavDtos;
        }

        public async Task<IEnumerable<SideNavDto>> GetSupervisedAndSubstitutedEmployees(Guid employeeId)
        {
            var supervisedEmployees = await _employeeService.GetSupervisedEmployees(employeeId);
            var substitutedEmployees = await _employeeService.GetSubstitutedEmployees(employeeId);


            List<SideNavEmployeeDto> sideNavSupervisedDtos = new List<SideNavEmployeeDto>();

            foreach (var employee in supervisedEmployees)
            {
                sideNavSupervisedDtos.Add(new SideNavEmployeeDto
                {
                    Id = Guid.Parse(employee.Id),
                    EmployeeInternalId = employee.EmployeeInternalId,
                    Name = employee.FirstName + " " + employee.LastName,
                    Department = employee.Department
                });
            }

            List<SideNavEmployeeDto> sideNavSubstitutedDtos = new List<SideNavEmployeeDto>();
            foreach (var employee in substitutedEmployees)
            {
                sideNavSubstitutedDtos.Add(new SideNavEmployeeDto
                {
                    Id = Guid.Parse(employee.Id),
                    EmployeeInternalId = employee.EmployeeInternalId,
                    Name = employee.FirstName + " " + employee.LastName,
                    Department = employee.Department
                });
            }

            List<SideNavDto> sideNavDtos = new List<SideNavDto>();
            sideNavDtos.Add(new SideNavDto
            {
                Department = "Supervised",
                Employees = sideNavSupervisedDtos
            });
            sideNavDtos.Add(new SideNavDto
            {
                Department = "Substituted",
                Employees = sideNavSubstitutedDtos
            });

            return sideNavDtos;
        }
        public async Task<IEnumerable<SideNavDto>> GetTeams(Guid employeeId)
        {
            var allEmployees = await _employeeRepository.GetAll();
            var employee = await _employeeRepository.FindByIdAsync(employeeId.ToString());
            if (employee == null)
            {
                throw new Exception("Employee not found");
            }

            var ownDepartmentEmployees = allEmployees.Where(e => e.Department == employee.Department);

            var supervisedTeams = await _employeeService.GetSupervisedEmployees(employeeId);
            supervisedTeams = supervisedTeams.Where(e => e.Supervisor == employeeId.ToString());

            var sideNavDtos = new List<SideNavDto>();

            var ownDepartmentDto = new SideNavDto
            {
                Department = employee.Department,
                Employees = ownDepartmentEmployees.Select(e => new SideNavEmployeeDto
                {
                    Id = Guid.Parse(e.Id),
                    EmployeeInternalId = e.EmployeeInternalId,
                    Name = e.FirstName + " " + e.LastName,
                    Department = e.Department
                })
            };
            sideNavDtos.Add(ownDepartmentDto);

            var supervisedTeamsByDepartment = supervisedTeams.GroupBy(e => e.Department);
            foreach (var departmentGroup in supervisedTeamsByDepartment)
            {
                if (departmentGroup.Key != employee.Department)
                {
                    var supervisedTeamsDto = new SideNavDto
                    {
                        Department = departmentGroup.Key,
                        Employees = departmentGroup.SelectMany(e => allEmployees.Where(ae => ae.Department == e.Department).Select(ae => new SideNavEmployeeDto
                        {
                            Id = Guid.Parse(ae.Id),
                            EmployeeInternalId = ae.EmployeeInternalId,
                            Name = ae.FirstName + " " + ae.LastName,
                            Department = ae.Department
                        }))
                    };
                    sideNavDtos.Add(supervisedTeamsDto);
                }
            }

            return sideNavDtos;
        }

    }
}
