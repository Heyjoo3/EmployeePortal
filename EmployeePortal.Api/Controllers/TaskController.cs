using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Helpers;
using EmployeePortal.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EmployeePortal.Api.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost("UpdateTask")]
        public async Task<IActionResult> UpdateTask(IFormCollection taskData)
        {
            if (taskData.TryGetValue("taskData",out var someString ))
             { 
                try
                {
                    var taskDto = JsonConvert.DeserializeObject<TaskDto>(someString);
                    var updatedTask = await _taskService.UpdateTask(taskDto);
                    return Ok(new BaseResult { Data = updatedTask, IsSuccessfull = true });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }
            }
            return BadRequest(ModelState.IsValid);
        }

        [HttpDelete("DeleteTask/{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var result = await _taskService.DeleteTask(id);
            return Ok(result);
        }
    }
}
