using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Models;

namespace EmployeePortal.Api.MappingProfiles
{
    public class AbsenceProfile : AutoMapper.Profile
    {
        public AbsenceProfile()
        {
            CreateMap<Absence, AbsenceDto>().ReverseMap();
        }
    }
}
