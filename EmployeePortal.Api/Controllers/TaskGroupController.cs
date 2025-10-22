using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Helpers;
using EmployeePortal.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EmployeePortal.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]

    public class TaskGroupController : ControllerBase
    {
        private readonly ITaskGroupService _taskGroupService;

        public TaskGroupController(ITaskGroupService taskGroupService)
        {
            _taskGroupService = taskGroupService;
        }

        [HttpPost("CreateTaskGroup")]
        public async Task<IActionResult> CreateTaskGroup(IFormCollection TaskGroupData)
        {
           if (TaskGroupData.TryGetValue("taskGroupFormData", out var someStrin)) 
            {
                try
                {
                    var taskGroupDto = JsonConvert.DeserializeObject<TaskGroupDto>(someStrin);
                    var taskGroup = await _taskGroupService.CreateTaskGroup(taskGroupDto);
                    return Ok(new BaseResult { Data = taskGroup, IsSuccessfull = true });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }
            }
            else
                return BadRequest(ModelState.IsValid);
        }

        [HttpDelete("DeleteTaskGroup/{id}")]
        public async Task<IActionResult> DeleteTaskGroup(Guid id)
        {
            return Ok(await _taskGroupService.DeleteTaskGroup(id));
        }

        [HttpPost("UpdateTaskGroup")]
        public async Task<IActionResult> UpdateTaskGroup(IFormCollection TaskGroupData)
        {          
           if (TaskGroupData.TryGetValue("taskGroupFormData", out var someStrin))
            {
                try
                {
                    var taskGroupDto = JsonConvert.DeserializeObject<TaskGroupDto>(someStrin);
                    var taskGroup = await _taskGroupService.UpdateTaskGroup(taskGroupDto);
                    return Ok(new BaseResult { Data = taskGroup, IsSuccessfull = true });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }
            }
            else
                return BadRequest(ModelState.IsValid);
        }
    }
}
