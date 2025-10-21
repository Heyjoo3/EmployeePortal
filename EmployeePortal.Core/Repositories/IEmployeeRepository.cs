using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace EmployeePortal.Core.Repositories
{
    public interface IEmployeeRepository: IRepository<Employee>
    {
        Task<IdentityResult> CreateAsync(Employee employee, string password);
        Task<IdentityResult> UpdatePasswordAsync(Employee employee, string passwordOld, string passwordNew);
        Task<Employee> FindByIdAsync(string employeeId);
        Task<Employee> FindByEmailAsync(string email);
        Task<Employee> FindByNameAsync(string username);
        Task<string> GetFullNameById(string employeeId);
        Task<string> GetEmailById(string employeeId);
        Task<IdentityResult> UpdateAsync(Employee employee, string passwordNew);
        Task<IdentityResult> DeleteAsync(Employee employee);
        Task <IdentityResult> AddToRoleAsync(Employee employee, string role);
        Task<bool> IsInRoleAsync(Employee employee, string role);
        Task<IEnumerable<Employee>> GetEmployeesSortedByDepartment();

    }
}
