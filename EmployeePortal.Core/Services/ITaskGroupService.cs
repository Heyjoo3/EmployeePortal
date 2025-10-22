using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Core.Services
{
    public interface ITaskGroupService
    {
        Task<TaskGroupDto> CreateTaskGroup(TaskGroupDto taskGroupDto);
        Task<BaseResult> DeleteTaskGroup(Guid id);
        Task<TaskGroupDto>UpdateTaskGroup(TaskGroupDto taskGroupDto);


    }
}
