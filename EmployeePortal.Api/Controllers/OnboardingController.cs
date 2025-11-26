using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Helpers;
using EmployeePortal.Core.Models;
using EmployeePortal.Core.Services;
using EmployeePortal.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EmployeePortal.Api.Controllers
{

    [Route("[controller]")]
    [ApiController]

    public class OnboardingController : ControllerBase
    {
        private readonly IOnboardingService _onboardingService;

        public OnboardingController(IOnboardingService onboadingService)
        {
            _onboardingService = onboadingService;
        }

        [HttpPost("CreateOnboardingPlan")]
        public async Task<IActionResult> CreateOnboardingPlan([FromBody] OnboardingPlanDto onboardingPlanDataDto)
        {
                try
                {
                    var onbaordingPlan = await _onboardingService.CreateOnboardingPlan(onboardingPlanDataDto);
                    return Ok(new BaseResult { Data = onbaordingPlan, IsSuccessfull = true });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }
        }

        [HttpDelete("DeleteOnboardingPlan")]
        public async Task<IActionResult> DeleteOnboardingPlan([FromBody] string id)
        {

            var result = await _onboardingService.DeleteOnboardingPlan(Guid.Parse(id));
            if (result)
            {
                return Ok(new BaseResult { IsSuccessfull = true, Message = "Onboarding plan deleted successfully." });
            }
            return BadRequest(new BaseResult { IsSuccessfull = false, Message = "Onboarding plan not found." });
        }

        [HttpPost("GetOnboardingPlanByEmployeeId")]
        public async Task<IActionResult> GetOnboardingPlanByEmployeeId([FromBody] string employeeId)
        {
            Guid id = Guid.Parse(employeeId);
            var onboardingPlan = await _onboardingService.GetOnboardingPlanByEmployee(id);

            if (onboardingPlan != null)
            {
                return Ok(new BaseResult { Data = onboardingPlan, IsSuccessfull = true });
            }
            return NotFound(new BaseResult { IsSuccessfull = false, Message = "Onboarding plan not found." });
        }

        [HttpPost("UpdateOnboardingPlan")]
        public async Task<ActionResult<OnboardingPlanDto>> UpdateOnboardingPlan([FromBody] OnboardingPlanDto onboardingPlanDataDto)
        {
            try
            {
                var updatedPlan = await _onboardingService.UpdateOnboardingPlan(onboardingPlanDataDto);
                return Ok(new BaseResult { Data = updatedPlan, IsSuccessfull = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResult { IsSuccessfull = false, Message = "ex" });
            }
        }

        [HttpPost("GetAllOnboardingPlans")]
        public async Task<IActionResult> GetAllOnboardingPlans()
        {
            try
            {
                var updatedPlan = await _onboardingService.GetAllOnboardingPlans();
                return Ok(new BaseResult { Data = updatedPlan, IsSuccessfull = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResult { IsSuccessfull = false, Message = "ex" });
            }
        }
    }
}
