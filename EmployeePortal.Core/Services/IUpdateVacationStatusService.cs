using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Helpers;

namespace EmployeePortal.Core.Services
{
    public interface IUpdateVacationStatusService
    {
        Task<BaseResult> AcceptVacation(VacationDto vacationDataDto);
        Task<BaseResult> CancelVacation(VacationDto vacationDataDto);
    }
}
