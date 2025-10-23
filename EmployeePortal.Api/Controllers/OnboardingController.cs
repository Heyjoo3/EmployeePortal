using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Helpers;
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
        public async Task<IActionResult> CreateOnboardingPlan(From)
        {
            if (OnboadingPlanData.TryGetValue("onboardingPlanData", out var someString))
            {
                try
                {
                    var onboardingPlanDto = JsonConvert.DeserializeObject<OnboardingPlanDto>(someString);
                    var onbaordingPlan = await _onboardingService.CreateOnboardingPlan(onboardingPlanDto);
                    return Ok(new BaseResult { Data = onbaordingPlan, IsSuccessfull = true });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }        
            }
            else
                return BadRequest(ModelState.IsValid);
        }

        [HttpDelete("DeleteOnboardingPlan/{id}")]
        public async Task<IActionResult> DeleteOnboardingPlan(Guid id)
        {
            var result = await _onboardingService.DeleteOnboardingPlan(id);
            if (result)
            {
                return Ok(new BaseResult { IsSuccessfull = true, Message = "Onboarding plan deleted successfully." });
            }
            return NotFound(new BaseResult { IsSuccessfull = false, Message = "Onboarding plan not found." });
        }

        [HttpGet("GetOnboardingPlan/{id}")]
        public async Task<IActionResult> GetOnboardingPlan(Guid id)
        {
            var onboardingPlan = await _onboardingService.GetOnboardingPlan(id);
            if (onboardingPlan != null)
            {
                return Ok(new BaseResult { Data = onboardingPlan, IsSuccessfull = true });
            }
            return NotFound(new BaseResult { IsSuccessfull = false, Message = "Onboarding plan not found." });
        }

        [HttpPut("UpdateOnboardingPlanOld")]
        public async Task<IActionResult> UpdateOnboardingPlanOld(IFormCollection OnboardingPlanData)
        {
            if (OnboardingPlanData.TryGetValue("onboardingPlanData", out var someString))
            {
                var onboardingPlanDataDto = JsonConvert.DeserializeObject<OnboardingPlanDto>(someString);

                var updatedPlan = await _onboardingService.UpdateOnboardingPlan(onboardingPlanDataDto);
                if (updatedPlan != null)
                {
                    return Ok(new BaseResult { Data = updatedPlan, IsSuccessfull = true });
                }
                return NotFound(new BaseResult { IsSuccessfull = false, Message = "Onboarding plan not found." });
            }
            else
            {
                return BadRequest(new BaseResult { IsSuccessfull = false, Message = "Invalid onboarding plan data." });
            }
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
                return BadRequest(new BaseResult { IsSuccessfull = false, Message = "Invalid Data" });
            }
        }
}
