using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Models;

namespace EmployeePortal.Core.Repositories
{
    public interface IPublicHolidayRepository: IRepository<PublicHoliday>
    {
        Task CreateHoliday(PublicHolidayDto holidayDto);
        Task<PublicHoliday> GetByYear(int year);
        Task<PublicHoliday> GetById(int holidayId);
        Task<IEnumerable<PublicHoliday>> GetPublicHolidaysByDateRange(DateTime startDate, DateTime endDate);
    }
}

