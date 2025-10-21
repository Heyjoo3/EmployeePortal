using EmployeePortal.Core.Helpers;
using EmployeePortal.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EmployeePortal.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class SideNavController : ControllerBase 
    {
        private readonly ISideNavService _sideNavService;

        public SideNavController(ISideNavService sideNavService)
        {
            _sideNavService = sideNavService;
        }

        [HttpPost("EmployeesSortedByDepartment")]
        public async Task<ActionResult> GetAllEmployeesSortedByDepartment()
        {
            try
            {
                var employees = await _sideNavService.GetAllEmployeesSortedByDepartment();
                return Ok(new BaseResult
                {
                    IsSuccessfull = true,
                    Data = employees
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResult
                {
                    IsSuccessfull = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("SupervisedAndSubsitutedEmployees")]
        public async Task<ActionResult> GetSupervisedAndSubstitutedEmployees(IFormCollection employeeIdData)
        {
            try 
            {
                employeeIdData.TryGetValue("employeeId", out var someString);

                var employeeId = JsonConvert.DeserializeObject<Guid>(someString);
                var employees = await _sideNavService.GetSupervisedAndSubstitutedEmployees(employeeId);
                return Ok(new BaseResult
                {
                    IsSuccessfull = true,
                    Data = employees
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResult
                {
                    IsSuccessfull = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("getTeams")]
        public async Task<ActionResult> GetTeams(IFormCollection employeeIdData)
        {
            try
            {
                employeeIdData.TryGetValue("employeeId", out var someString);

                var employeeId = JsonConvert.DeserializeObject<Guid>(someString);
                var teams = await _sideNavService.GetTeams(employeeId);
                return Ok(new BaseResult
                {
                    IsSuccessfull = true,
                    Data = teams
                });
            } catch (Exception ex)
            {
                return BadRequest(new BaseResult
                {
                    IsSuccessfull = false,
                    Message = ex.Message
                });
            }
        }
    }
}
