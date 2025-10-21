using EmployeePortal.Core.Repositories;
using EmployeePortal.Core.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Services
{
    public class RoleService: IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await _roleRepository.RoleExistsAsync(roleName);
        } 

        public async Task<IdentityResult> CreateRoleAsync(string roleName)
        {
            return await _roleRepository.CreateAsync(new IdentityRole(roleName));
        } 
        public async Task<IdentityResult> DeleteRoleAsync(IdentityRole roleName)
        {
            var role = await _roleRepository.GetRoleNameAsync(roleName);
            if (role == null)
            {
                // Role not found
                return IdentityResult.Failed(new IdentityError { Description = $"Role '{roleName}' not found." });
            }
            return await _roleRepository.DeleteAsync(roleName);
        }

        public async Task<IEnumerable<string>> GetAllRoles()
        {
            var allRoles = await _roleRepository.GetAll();
            return allRoles.Select(x=>x.Name);
        }
    }
}
