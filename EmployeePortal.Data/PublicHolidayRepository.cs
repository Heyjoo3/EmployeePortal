using EmployeePortal.Core.Data;
using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Models;
using EmployeePortal.Core.Repositories;
using EmployeePortal.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Data
{
    public class PublicHolidayRepository: Repository<PublicHoliday>, IPublicHolidayRepository
    {
        public PublicHolidayRepository(EmployeePortalContext context) : base(context)
        {


        }

        private EmployeePortalContext EmployeePortalContext
        {
            get { return _context as EmployeePortalContext; }
        }

        public async new Task Add(PublicHoliday holidayData)
        {
            await EmployeePortalContext.PublicHolidays.AddAsync(holidayData);
        }

        public Task CreateHoliday(PublicHolidayDto holidayDto)
        {
            throw new NotImplementedException();
        }

        public Task<PublicHoliday> GetByYear(int year)
        {
            throw new NotImplementedException();
        }

        public async Task<PublicHoliday> GetById(int holidayId)
        {
            var holiday = await EmployeePortalContext.PublicHolidays.Where(x => x.Id == holidayId).FirstOrDefaultAsync();
            return holiday;
                                                            
        }

        public async  Task<IEnumerable<PublicHoliday>> GetPublicHolidaysByDateRange(DateTime startDate, DateTime endDate)
        {
           var holidays = await EmployeePortalContext.PublicHolidays.Where(x => x.Date >= startDate && x.Date <= endDate).ToListAsync();
           return holidays;
        }
    }
}
