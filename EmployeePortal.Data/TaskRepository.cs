using EmployeePortal.Core.Data;
using EmployeePortal.Core.Models;
using EmployeePortal.Core.Repositories;
using EmployeePortal.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Data
{
    public class TaskRepository : Repository<BaseTask>, ITaskRepository
    {
        public TaskRepository(EmployeePortalContext context) : base(context) { }

        private EmployeePortalContext EmployeePortalContext
        {
            get { return _context as EmployeePortalContext; }
        }

        public async Task DeleteTask(Guid taskId)
        {
            var task = await EmployeePortalContext.Tasks.FindAsync(taskId);
            if (task == null)
            {
                throw new KeyNotFoundException($"Task with ID {taskId} was not found.");
            }

            EmployeePortalContext.Tasks.Remove(task);
            await EmployeePortalContext.SaveChangesAsync();
        }
    }
}
