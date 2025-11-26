using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Repositories;
using System;
using System.Threading.Tasks;

namespace EmployeePortal.Core.Helpers
{
    public static class EmployeeHelper
    {
        private static IEmployeeRepository? _employeeRepository;

        public static void Initialize(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public static async Task<string?> ValidateReferencePersonAsync(string? referencePersonId)
        {
            if (_employeeRepository == null)
            {
                throw new InvalidOperationException("EmployeeHelper is not initialized. Call Initialize() with a valid repository.");
            }

            if (string.IsNullOrEmpty(referencePersonId))
            {
                return null;
            }

            var referenceEmployee = await _employeeRepository.FindByIdAsync(referencePersonId);
            return referenceEmployee == null ? null : referencePersonId;
        }
    }
}
