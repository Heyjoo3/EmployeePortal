using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Core.Services
{
    public interface IRoleService 
    {
        Task<bool> RoleExistsAsync(string roleName);
        Task<IdentityResult> CreateRoleAsync(string roleName);
        Task<IdentityResult> DeleteRoleAsync(IdentityRole role);
        Task<IEnumerable<string>> GetAllRoles();
    }
}
