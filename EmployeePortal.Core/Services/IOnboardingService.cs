using EmployeePortal.Core.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Core.Services
{
    public interface IOnboardingService
    {
        Task<OnboardingPlanDto> CreateOnboardingPlan(OnboardingPlanDto onboardingPlanDataDto);
        Task<bool> DeleteOnboardingPlan(Guid id);
        Task<OnboardingPlanDto?> GetOnboardingPlan(Guid id);
        Task<bool> UpdateOnboardingPlan(OnboardingPlanDto onboardingPlanDataDto);
    }
}
