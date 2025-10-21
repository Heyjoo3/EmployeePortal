using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Models;

namespace EmployeePortal.Core.Repositories
{
    public interface IAbsenceRepository : IRepository<Absence>
    {
        Task<IEnumerable<Absence>> GetAllAbsences();
        new Task<Absence> GetById(Guid absenceId);
        Task<IEnumerable<Absence>> GetAbsencesByEmployeeId(Guid EmployeeId);
        Task<IEnumerable<Absence>> UpdateAbsence(Absence absence);
        Task<IEnumerable<Absence>> GetUnexcusedSickLeaves(Guid EmployeeId);
        Task<IEnumerable<Absence>> GetSickDaysByEmployeeAndDateRange(String EmployeeId, DateTime startDate, DateTime endDate, int? deadline);
    }
}

