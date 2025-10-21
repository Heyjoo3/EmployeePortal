using AutoMapper;
using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Helpers;
using EmployeePortal.Core.Models;
using EmployeePortal.Core.Repositories;
using EmployeePortal.Core.Services;
using EmployeePortal.Data;
using Microsoft.AspNetCore.Identity;

namespace EmployeePortal.Services
{
    public class EmployeeService: IEmployeeService
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IVacationRepository _vacationRepository;
        private readonly IPasswordHasher<Employee> _passwordHasher;


        public EmployeeService(IMapper mapper, IEmployeeRepository employeeRepository, IVacationRepository vacationRepository, IPasswordHasher<Employee> passwordHasher)
        {
            _mapper = mapper;
            _employeeRepository = employeeRepository;
            _vacationRepository = vacationRepository;
            _passwordHasher= passwordHasher;
        }
       
        public async Task<IEnumerable<EmployeeDto>> GetAllEmployees()
        {
            var allEmployees = await _employeeRepository.GetAll();
            var employeeList = _mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeDto>>(allEmployees.Where(x=> x.Id != null));
            return employeeList;
        }

        public async Task<BaseResult> Authenticateemployee(EmployeeDto employeeDto)
        {
            var employee = await _employeeRepository.FindByNameAsync(employeeDto.UserName);
            PasswordVerificationResult resultPassword = PasswordVerificationResult.Failed; 
            BaseResult loginError = new BaseResult { IsSuccessfull = false, Message = "Incorrect Login", Data = null };

            if (employee != null) 
            {
                resultPassword = _passwordHasher.VerifyHashedPassword(employee, employee.PasswordHash, employeeDto.Password);
            }
            else
            {
                return loginError;
            }

            if (employee != null && resultPassword == PasswordVerificationResult.Success)
            {
                var data = new { Id = employee.Id, Role = employee.Role };
                return new BaseResult { IsSuccessfull = true, Data = data};
            }
            else
            {
                return loginError;
            }
        }

        public async Task<BaseResult> UpdateCredentialsAsync(CredentialsDto credentialsDto)
        {
            var employee = await _employeeRepository.FindByIdAsync(credentialsDto.EmployeeId);
            PasswordVerificationResult resultPassword = PasswordVerificationResult.Failed;
            IdentityResult resultUpdateUser = IdentityResult.Success;
            IdentityResult resultUpdatePassword = IdentityResult.Success;

            if (employee != null)
            {
                resultPassword = _passwordHasher.VerifyHashedPassword(employee, employee.PasswordHash, credentialsDto.PasswordOld);
            }
            else
            {
                return new BaseResult { IsSuccessfull = false, Message = "User konnte nicht gefunden werden", Data = null };
            }

            if (resultPassword == PasswordVerificationResult.Success && employee.UserName != credentialsDto.UserName)
            {
                employee.UserName = credentialsDto.UserName;    
                resultUpdateUser = await _employeeRepository.UpdateAsync(employee, null);
            }
            if (credentialsDto.PasswordNew != null)
            {
                PasswordVerificationResult passwordResult = _passwordHasher.VerifyHashedPassword(employee, employee.PasswordHash, credentialsDto.PasswordNew);
                if (passwordResult == PasswordVerificationResult.Failed)
                {
                resultUpdatePassword = await _employeeRepository.UpdatePasswordAsync(employee, credentialsDto.PasswordOld, credentialsDto.PasswordNew);
                } else
                {
                    return new BaseResult { IsSuccessfull = false, Message = "Das neue Passwort kann nicht das selbe sein wie vorher sein.", Data = new { errorType = 1 } };
                }
            }

            if (resultUpdateUser.Succeeded && resultUpdatePassword.Succeeded)
            {
                var updatedEmployee = _employeeRepository.FindByIdAsync(employee.Id);
                return new BaseResult { IsSuccessfull = true, Message = "Logindaten konnten aktualisiert werden.", Data = updatedEmployee };
            }
            else
            {
                return new BaseResult { IsSuccessfull = false, Message = "Logindaten konnten nicht aktualisiert werden.", Data = null };
            }
        }

        public async Task<IdentityResult> CreateEmployeeAsync(EmployeeDto employeeDataDto)
        {
            var employeeData = _mapper.Map<EmployeeDto, Employee>(employeeDataDto);
            return await _employeeRepository.CreateAsync(employeeData, employeeDataDto.Password);
        }

        public async Task<Employee> GetEmployeeByIdAsync(string userId)
        {
           return await _employeeRepository.FindByIdAsync(userId);
        }

        public async Task<Employee> GetEmployeeByEmailAsync(string email)
        {
            return await _employeeRepository.FindByEmailAsync(email);
        }

        public async Task<IdentityResult> UpdateEmployeeAsync(EmployeeUpdateDto employeeDto, bool forcePasswordUpdate)
        {
            var employeeData = _mapper.Map<EmployeeUpdateDto, Employee>(employeeDto);
            if (forcePasswordUpdate)
            {
                var employee = await _employeeRepository.FindByNameAsync(employeeDto.UserName);
                PasswordVerificationResult passwordResult = _passwordHasher.VerifyHashedPassword(employee, employee.PasswordHash, employeeDto.Password);
                if (passwordResult == PasswordVerificationResult.Failed)
                {
                employeeData.ForceNewPassword = true;
                }
            }
            return await _employeeRepository.UpdateAsync(employeeData, employeeDto.Password);
        }

        public async Task<IdentityResult> DeleteEmployeeAsync(Employee employee)
        {
            return await _employeeRepository.DeleteAsync(employee);
        }

        public async Task<IdentityResult> AddEmployeeToRoleAsync(Employee employee, string role)
        {
            return await _employeeRepository.AddToRoleAsync(employee, role);
        }

        public async Task<bool> IsEmployeeInRoleAsync(Employee employee, string role)
        {
            return await _employeeRepository.IsInRoleAsync(employee, role);
        }

        public async Task<IEnumerable<GetEmployeeDto>> GetAllEmployeesReduced()
        {
            var employees = await _employeeRepository.GetAll();
            var reducedEmployees = new List<GetEmployeeDto>();

            foreach (var employee in employees)
            {
                var dto = new GetEmployeeDto()
                {
                    Id = Guid.Parse(employee.Id),
                    EmployeeInternalId = employee.EmployeeInternalId,
                    Name = employee.FirstName + " " + employee.LastName,
                    Department = employee.Department,
                    Role = employee.Role
                };
                reducedEmployees.Add(dto);
            }
            return reducedEmployees;
        } 
      public async Task<IEnumerable<EmployeeDto>> GetSupervisedEmployees(Guid employeeId)
        {
            var allEmployees = await _employeeRepository.GetAll();
            var allVacations = await _vacationRepository.GetAllVacations();
            var supervisedEmployees = allEmployees.Where(e => e.Supervisor == employeeId.ToString() 
            || allVacations.Any(v => v.EmployeeId.ToString() == e.Id && v.Supervisor == employeeId.ToString())).ToList();
            return _mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeDto>>(supervisedEmployees);
        }

        public async Task<IEnumerable<EmployeeDto>> GetSubstitutedEmployees(Guid employeeId)
        {
            var allEmployees = await _employeeRepository.GetAll();
            var allVacations = await _vacationRepository.GetAllVacations();
            var substitutedEmployees = allEmployees.Where(e => e.Substitute == employeeId.ToString()
            || allVacations.Any(v => v.EmployeeId.ToString() == e.Id && v.Substitute == employeeId.ToString())).ToList();
            return _mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeDto>>(substitutedEmployees);
        }
    }
}
