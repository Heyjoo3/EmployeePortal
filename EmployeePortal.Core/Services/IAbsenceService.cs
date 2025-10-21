using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Helpers;

namespace EmployeePortal.Core.Services
{
    public interface IAbsenceService
    {
        public Task<IEnumerable<AbsenceDto>> GetAllAbsences();
        public Task<IEnumerable<AbsenceDto>> GetAbsencesByEmployeeId(Guid employeeId);
        public Task<Guid> CreateAbsence(AbsenceDto absenceDto);
        public Task<BaseResult> DeleteAbsence(Guid absenceId);
        public Task <BaseResult> UpdateAbsence(AbsenceDto absenceDto);
        public Task<BaseResult> CheckSickLeaveDeadline(Guid EmployeeId);
    }
}
