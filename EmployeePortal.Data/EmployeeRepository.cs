using Microsoft.EntityFrameworkCore;
using EmployeePortal.Core.Data;
using EmployeePortal.Core.Models;
using EmployeePortal.Core.Repositories;
using EmployeePortal.Data.Repositories;
using Microsoft.AspNetCore.Identity;

namespace EmployeePortal.Data
{

    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        private readonly UserManager<Employee> _userManager;
        private new readonly EmployeePortalContext _context;

        public EmployeeRepository(UserManager<Employee> userManager, EmployeePortalContext context): base(context) {

            _userManager = userManager;
            _context = context;
        }

        private EmployeePortalContext EmployeePortalContext
        {
            get { return _context as EmployeePortalContext; }
        }

        public async Task<IdentityResult> AddToRoleAsync(Employee employee, string role)
        {
            return await _userManager.AddToRoleAsync(employee, role);
        }

        public async Task<IdentityResult> CreateAsync(Employee employee, string password)
        {
            return await _userManager.CreateAsync(employee, password);
        }

        public async Task<IdentityResult> UpdatePasswordAsync(Employee employee,string passwordOld, string passwordNew)
        {
            employee.ForceNewPassword = false;
            return await _userManager.ChangePasswordAsync(employee, passwordOld, passwordNew);
        }

        public async Task<IdentityResult> DeleteAsync(Employee employee)
        {
            return await _userManager.DeleteAsync(employee);
        }

        public async Task<Employee> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<Employee> FindByIdAsync(string employeeId)
        {
            return await _userManager.FindByIdAsync(employeeId);
            //Employee employee = await EmployeePortalContext.Employees.Include(e => e.Vacations).Include(e => e.Absences).FirstOrDefaultAsync(e => e.Id == employeeId);
            //return employee;
        }

        public async Task<string> GetFullNameById(string employeeId)
        {
            Employee employee = await EmployeePortalContext.Employees.FirstOrDefaultAsync(e => e.Id == employeeId);
            return employee.FirstName + " " + employee.LastName;
        }

        public async Task<string> GetEmailById(string employeeId)
        {
            Employee employee = await EmployeePortalContext.Employees.FirstOrDefaultAsync(e => e.Id == employeeId);
            return employee.Email;
        }

        public async Task<Employee> FindByNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public new async Task<IEnumerable<Employee>> GetAll()
        {
            List<Employee> employees = await EmployeePortalContext.Employees.OrderBy(e => e.LastName).Select(e => new Employee
            {
                Id = e.Id,
                Salutation = e.Salutation,
                EmployeeInternalId = e.EmployeeInternalId,
                FirstName = e.FirstName,
                LastName = e.LastName,
                DateofBirth = e.DateofBirth,
                Department = e.Department,
                Role = e.Role,
                Supervisor = e.Supervisor,
                Substitute = e.Substitute,
                AnnualVacation = e.AnnualVacation,
                Email = e.Email,
                UserName = e.UserName,
                PhoneNumber = e.PhoneNumber,
                //Vacations = e.Vacations,
                //Absences = e.Absences, 
                SickNoteDeadLine = e.SickNoteDeadLine
            }).ToListAsync();
            return employees;
        }

        public async Task<IEnumerable<Employee>> GetEmployeesSortedByDepartment()
        {
            List<Employee> employees = await EmployeePortalContext.Employees.OrderBy(e => e.Department).ToListAsync();
            return employees;
        }

        public async Task<bool> IsInRoleAsync(Employee employee, string role)
        {
           return await _userManager.IsInRoleAsync(employee, role);
        }

        public async Task<IdentityResult> UpdateAsync(Employee employee, string passwordNew)

        {
            var existingEmployee = await _context.Employees.FindAsync(employee.Id);
            if (existingEmployee == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Employee with ID {employee.Id} not found." });
            }

            IdentityResult resultUpdatePassword = IdentityResult.Success;

            _context.Entry(existingEmployee).CurrentValues.SetValues(employee);
            var resultUpdateUser = await _userManager.UpdateAsync(existingEmployee);

            if (!string.IsNullOrEmpty(passwordNew))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(existingEmployee);
                resultUpdatePassword = await _userManager.ResetPasswordAsync(existingEmployee, token, passwordNew);
            }

            if (resultUpdatePassword.Succeeded && resultUpdateUser.Succeeded)
            {
                return IdentityResult.Success;
            }

            return IdentityResult.Failed(new IdentityError { Description = "IdentityResult failed." });
        }
    }
}
