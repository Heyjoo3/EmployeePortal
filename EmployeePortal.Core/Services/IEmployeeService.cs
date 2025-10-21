using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Helpers;
using EmployeePortal.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace EmployeePortal.Core.Services
{
    public interface IEmployeeService
    {
   
        Task<IEnumerable<EmployeeDto>> GetAllEmployees();
        Task<BaseResult> Authenticateemployee(EmployeeDto employeeDto);
        Task<BaseResult> UpdateCredentialsAsync(CredentialsDto credentialsDto);
        Task<IdentityResult> CreateEmployeeAsync(EmployeeDto model);
        Task<Employee> GetEmployeeByIdAsync(string userId);
        Task<Employee> GetEmployeeByEmailAsync(string email);
        Task<IdentityResult> UpdateEmployeeAsync(EmployeeUpdateDto employee, bool forcePasswordUpdate);
        Task<IdentityResult> DeleteEmployeeAsync(Employee employee);
        Task<IdentityResult> AddEmployeeToRoleAsync(Employee employee, string role);
        Task<bool> IsEmployeeInRoleAsync(Employee employee, string role);
        Task<IEnumerable<GetEmployeeDto>> GetAllEmployeesReduced();
        Task<IEnumerable<EmployeeDto>> GetSupervisedEmployees(Guid employeeId);
        Task<IEnumerable<EmployeeDto>> GetSubstitutedEmployees(Guid employeeId);
    }
}
