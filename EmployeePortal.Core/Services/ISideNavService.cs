using EmployeePortal.Core.Dto;

namespace EmployeePortal.Core.Services
{
    public interface ISideNavService
    {
        Task<IEnumerable<SideNavDto>> GetAllEmployeesSortedByDepartment();
        Task<IEnumerable<SideNavDto>> GetSupervisedAndSubstitutedEmployees(Guid employeeId);
        Task<IEnumerable<SideNavDto>> GetTeams(Guid employeeId);
    }
}
