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

        public async Task Delete(Guid id)
        {
            var onboardingPlan = await EmployeePortalContext.OnboardingPlans
                .Include(op => op.TaskGroups)
                .ThenInclude(tg => tg.Tasks)
                .FirstOrDefaultAsync(op => op.OnboardingId == id);

            if (onboardingPlan == null)
            {
                throw new KeyNotFoundException($"Onboarding plan with ID {id} was not found.");
            }

            // Remove all tasks in the task groups
            foreach (var taskGroup in onboardingPlan.TaskGroups ?? new List<TaskGroup>())
            {
                EmployeePortalContext.Tasks.RemoveRange(taskGroup.Tasks);
            }

            // Remove all task groups
            EmployeePortalContext.TaskGroups.RemoveRange(onboardingPlan.TaskGroups);

            // Remove the onboarding plan
            EmployeePortalContext.OnboardingPlans.Remove(onboardingPlan);

            // Save changes
            await EmployeePortalContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<OnboardingPlan>> GetAllWithDetails()
        {
            return await EmployeePortalContext.OnboardingPlans
                .Include(op => op.TaskGroups)
                .ThenInclude(tg => tg.Tasks)
                .ToListAsync();
        }

        public async Task<OnboardingPlan?> GetByEmployeeId(Guid employeeId)
        {
            return await EmployeePortalContext.OnboardingPlans
                .Include(op => op.TaskGroups)
                .ThenInclude(tg => tg.Tasks)
                .FirstOrDefaultAsync(op => op.EmployeeId == employeeId.ToString());
        }


        public async Task<OnboardingPlan> GetPlanById(Guid onboardingId)
        {
            var onboardingPlan = await EmployeePortalContext.OnboardingPlans
                .Include(op => op.TaskGroups)
                .ThenInclude(tg => tg.Tasks)
                .FirstOrDefaultAsync(op => op.OnboardingId == onboardingId);

            if (onboardingPlan == null)
            {
                throw new KeyNotFoundException($"Onboarding plan with ID {onboardingId} was not found.");
            }

            return onboardingPlan;
        }

        public async Task<OnboardingPlan> Update(OnboardingPlan onboardingPlan)
        {
            _context.OnboardingPlans.Update(onboardingPlan);
            await _context.SaveChangesAsync();
            return onboardingPlan;
        }

    }
}
