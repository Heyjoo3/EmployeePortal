using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeePortal.Core.Services
{
        public interface IVacationService
        {
                Task<IEnumerable<VacationDto>> GetAllVacations();
                Task<Guid> CreateVacation(VacationDto vacationDataDto);
                Task<Guid> AdminCreateVacation(VacationDto vacationDataDto);
                Task<BaseResult> DeleteVacation(Guid vacationId);
                Task<IEnumerable<VacationDto>> GetVacationsWithEmployee();
                Task<BaseResult> UpdateVacation(VacationDto vacationDataDto);
                Task<BaseResult> CheckForOverlaps(VacationDto companyVacationDataDto);
                Task<BaseResult> CheckForAbsenceOverlaps(AbsenceDto absence);
                Task<float> CalculateVacationDays(Guid vacationId);
                Task<BaseResult> CloseOpenVacationRequests(Guid employeeId);
                Task<IEnumerable<VacationDto>> GetVacationsByEmployeeId(Guid id);
                Task<IEnumerable<VacationDto>> GetAnnualVacationsByEmployeeId(Guid id);
                Task<BaseResult> UpdateVacationsAfterPublicHolidayDeletion(DateTime publicHolidayDate);
                Task<IEnumerable<VacationDto>> GetCompanyVacation();
                Task<IEnumerable<RequestDto>> GetOpenRequests(RequestFormDto requestForm);
        }
}
