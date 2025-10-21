using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Core.Repositories
{
    public interface IRoleRepository: IRepository<IdentityRole>
    {
        Task<bool> RoleExistsAsync(string roleName);
        Task<IdentityResult> CreateAsync(IdentityRole role);
        Task<IdentityResult> DeleteAsync(IdentityRole role);
        Task<string> GetRoleNameAsync(IdentityRole role);
    }
}
