namespace EmployeePortal.Api.MappingProfiles
{
    public class OnboardingProfile : AutoMapper.Profile
    {
        public OnboardingProfile()
        {

            CreateMap<Core.Models.OnboardingPlan, Core.Dto.OnboardingPlanDto>()
                .ForMember(dest => dest.OnboardingId, opt => opt.MapFrom(src => src.OnbardingId))
                .ForMember(dest => dest.TaskGroups, opt => opt.MapFrom(src => src.TaskGroups));

            CreateMap<Core.Models.TaskGroup, Core.Dto.TaskGroupDto>()
                .ForMember(dest => dest.Tasks, opt => opt.MapFrom(src => src.Tasks));

            CreateMap<Core.Models.BaseTask, Core.Dto.TaskDto>();

        }
    }
}