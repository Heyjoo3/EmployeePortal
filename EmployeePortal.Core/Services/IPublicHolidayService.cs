using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Helpers;
using EmployeePortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Core.Services
{
    public interface IPublicHolidayService
    {
        Task CreateHoliday(PublicHolidayDto holidayDto);
        Task<IEnumerable<PublicHolidayDto>> GetAllHolidaysByYear(int year);
        Task<IEnumerable<int>> GetAllYears();
        Task<BaseResult> DeleteHoliday(int holidayId);
    }
}
