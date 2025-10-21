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
        Task<OnboardingPlan?> GetByEmployeeIdAsync(Guid employeeId);
        //AddAsync, UpdateAsync, DeleteAsync inherited from IRepository
        Task CreateOnboardingPlan(OnboardingPlan onboardingPlan);

    }
}
