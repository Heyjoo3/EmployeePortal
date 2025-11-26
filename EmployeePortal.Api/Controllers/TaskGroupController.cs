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
        public async Task<IActionResult> CreateTaskGroup([FromBody] TaskGroupDto taskGroupDto)
        {

            try
            {
                var taskGroup = await _taskGroupService.CreateTaskGroup(taskGroupDto);
                return Ok(new BaseResult { Data = taskGroup, IsSuccessfull = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete("DeleteTaskGroup")]
        public async Task<IActionResult> DeleteTaskGroup([FromBody] string id)
        {
            try
            {
                return Ok(await _taskGroupService.DeleteTaskGroup(Guid.Parse(id)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        [HttpPost("UpdateTaskGroup")]
        public async Task<IActionResult> UpdateTaskGroup([FromBody] TaskGroupDto taskGroupDto)
        {
            try
            {
                var taskGroup = await _taskGroupService.UpdateTaskGroup(taskGroupDto);
                return Ok(new BaseResult { Data = taskGroup, IsSuccessfull = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
