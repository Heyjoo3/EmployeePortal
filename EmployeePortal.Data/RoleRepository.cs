using EmployeePortal.Core.Data;
using EmployeePortal.Core.Repositories;
using EmployeePortal.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Data
{
    
    public class RoleRepository : Repository<IdentityRole>,  IRoleRepository
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleRepository(RoleManager<IdentityRole> roleManager, EmployeePortalContext context) : base(context)
        {
            _roleManager = roleManager;
        }
        public async Task<IdentityResult> CreateAsync(IdentityRole role)
        {
            return await _roleManager.CreateAsync(role);
        }

        public async Task<IdentityResult> DeleteAsync(IdentityRole role)
        {
            return await _roleManager.DeleteAsync(role);
        }

        public async Task<string> GetRoleNameAsync(IdentityRole role)
        {
            return await _roleManager.GetRoleNameAsync(role);
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await _roleManager.RoleExistsAsync(roleName);
        }
    }
}
