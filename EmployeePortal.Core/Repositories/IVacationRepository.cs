using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Models;

namespace EmployeePortal.Core.Repositories
{
    public interface IVacationRepository: IRepository<Vacation>
    {
        Task<IEnumerable<Vacation>> GetAllVacations();
        new Task<Vacation> GetById(Guid vacationId);
        Task<IEnumerable<Vacation>> GetVacationsWithEmployee();
        Task<IEnumerable<Vacation>> UpdateVacation(Vacation vacation);
        Task<IEnumerable<Vacation>> GetOverlappedVacations(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Vacation>> GetOverlappedByAbsence(Guid employeeId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<Vacation>> GetByEmployeeId(Guid Id);
        Task<IEnumerable<Vacation>> GetAnnualByEmployeeId(Guid Id);
        Task<IEnumerable<Employee>> GetSupervisedEmployees(Guid employeeId);
        Task<IEnumerable<Employee>> GetSubstitutedEmployees(Guid employeeId);
        Task<IEnumerable<Vacation>> GetCompanyVacations();
        Task<IEnumerable<Vacation>> GetRequestsAdmin(Guid employeeId);
        Task<IEnumerable<Vacation>> GetRequestsSupervised(Guid employeeId);
        Task<IEnumerable<Vacation>> GetRequestsSubstituted(Guid employeeId);
        Task<IEnumerable<Vacation>> GetRequestsEmployee(Guid employeeId);
    }
}
