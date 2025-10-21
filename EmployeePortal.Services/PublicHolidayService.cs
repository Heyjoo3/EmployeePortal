using AutoMapper;
using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Helpers;
using EmployeePortal.Core.Models;
using EmployeePortal.Core.Repositories;
using EmployeePortal.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace EmployeePortal.Services
{
    public class PublicHolidayService : IPublicHolidayService
    {
        private readonly IMapper _mapper;
        private readonly IPublicHolidayRepository _publicHolidayRepository;


        public PublicHolidayService(IMapper mapper, IPublicHolidayRepository holidayRepository)
        {
            _mapper = mapper;
            _publicHolidayRepository = holidayRepository;
        }
        public async Task CreateHoliday(PublicHolidayDto holidayDataDto)
        {
            var holidayData = _mapper.Map<PublicHolidayDto, PublicHoliday>(holidayDataDto);
            await _publicHolidayRepository.Add(holidayData);
            await _publicHolidayRepository.Commit();
        }

        public async Task<BaseResult> DeleteVacation(int vacationId)
        {
            var vacation = await _publicHolidayRepository.GetById(vacationId);
            _publicHolidayRepository.Remove(vacation);
            if (await _publicHolidayRepository.Commit() > 0)
            {
                return new BaseResult { IsSuccessfull = true };
            }

            return new BaseResult { IsSuccessfull = false };
        }

        public async Task<IEnumerable<PublicHolidayDto>> GetAllHolidaysByYear(int year)
        {
            var holidays = await _publicHolidayRepository.GetAll();

            var holidaysList = _mapper.Map<IEnumerable<PublicHoliday>, IEnumerable<PublicHolidayDto>>(holidays.Where(x => x.Date.Year == year));
            return holidaysList;
        }

        public async Task<IEnumerable<int>> GetAllYears()
        {
            var allHolidays = await _publicHolidayRepository.GetAll();
            return allHolidays.Select(x => x.Date.Year);
        }

        public async Task<BaseResult> DeleteHoliday(int holidayId)
        {
            var holiday = await _publicHolidayRepository.GetById(holidayId);
             _publicHolidayRepository.Remove(holiday);
           if( await _publicHolidayRepository.Commit()> 0)
            {
                return new BaseResult { IsSuccessfull = true };
            }

            return new BaseResult { IsSuccessfull = false };
        }

    }
}
