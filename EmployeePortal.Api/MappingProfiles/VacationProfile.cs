using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Models;

namespace EmployeePortal.Api.MappingProfiles
{
    public class VacationProfile : AutoMapper.Profile
    {
        public VacationProfile() {

            CreateMap<Vacation, VacationDto>().ReverseMap();
            CreateMap<PublicHoliday, PublicHolidayDto>().ReverseMap();
        }
    }
}
