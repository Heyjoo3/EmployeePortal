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
    public class TaskGroupRepository : Repository<TaskGroup>, ITaskGroupRepository
    {
        public TaskGroupRepository(EmployeePortalContext context) : base(context) { }
        private EmployeePortalContext EmployeePortalContext
        {
            get { return _context as EmployeePortalContext; }
        }
        public async Task DeleteTaskGroup(Guid taskGroupId)
        {
            var taskGroup = await EmployeePortalContext.TaskGroups
              .Include(tg => tg.Tasks)
              .FirstOrDefaultAsync(tg => tg.Id == taskGroupId);

            if (taskGroup == null)
            {
                throw new KeyNotFoundException($"TaskGroup was not found.");
            }

            EmployeePortalContext.Tasks.RemoveRange(taskGroup.Tasks);
            EmployeePortalContext.TaskGroups.Remove(taskGroup);
            await EmployeePortalContext.SaveChangesAsync();
        }
    }
}
