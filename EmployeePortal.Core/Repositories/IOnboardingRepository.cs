using EmployeePortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Core.Repositories
{
    public interface IOnboardingRepository :IRepository<OnboardingPlan>
    {
        Task<OnboardingPlan?> GetByEmployeeId(Guid employeeId);
        Task<IEnumerable<OnboardingPlan>> GetAllWithDetails();
        Task <OnboardingPlan> Update(OnboardingPlan onboardingPlan);
        Task Delete(Guid id);
        Task <OnboardingPlan> GetPlanById(Guid onboardingId);
    }
}
