using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Models;

namespace EmployeePortal.Api.MappingProfiles
{
    public class EmployeeProfile : AutoMapper.Profile
    {

        public EmployeeProfile()
        {
            CreateMap<Employee, EmployeeDto>().ReverseMap();
            CreateMap<Employee, EmployeeUpdateDto>().ReverseMap();
            CreateMap<Employee, SideNavEmployeeDto>().ReverseMap(); 
               
        }
    }
}
