using EmployeePortal.Core.Data;
using EmployeePortal.Core.Models;
using EmployeePortal.Core.Repositories;
using EmployeePortal.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Data
{
    public class OnboardingRepository : Repository<OnboardingPlan>, IOnboardingRepository
    {
        public OnboardingRepository(EmployeePortalContext context) : base(context) { }

        private EmployeePortalContext EmployeePortalContext
        {
            get { return _context as EmployeePortalContext; }
        }

        public Task Create(OnboardingPlan onboardingPlan)
        {

            throw new NotImplementedException();
        }

        public Task<bool> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<OnboardingPlan>> GetAllWithDetails()
        {
            throw new NotImplementedException();
        }

        public Task<OnboardingPlan?> GetByEmployeeId(Guid employeeId)
        {
            throw new NotImplementedException();
        }

        public Task<OnboardingPlan?> GetByEmployeeIdAsync(Guid employeeId)
        {
            throw new NotImplementedException();
        }

        public async Task<OnboardingPlan> Update(OnboardingPlan onboardingPlan)
        {
            _context.OnboardingPlans.Update(onboardingPlan);
            await _context.SaveChangesAsync();
            return onboardingPlan;
        }

        Task<OnboardingPlan> IOnboardingRepository.Create(OnboardingPlan onboardingPlan)
        {
            throw new NotImplementedException();
        }
    }
}
