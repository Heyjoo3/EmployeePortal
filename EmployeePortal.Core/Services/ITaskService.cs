using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Core.Services
{
    public interface ITaskService
    {
        Task<TaskDto> UpdateTask(TaskDto task);
        Task<BaseResult> DeleteTask(Guid taskId);
    }
}
