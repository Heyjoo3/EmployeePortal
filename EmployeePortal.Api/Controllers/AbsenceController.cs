using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Helpers;
using EmployeePortal.Core.Services;
using EmployeePortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace EmployeePortal.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]

    public class AbsenceController : ControllerBase
    {

        private readonly IAbsenceService _absenceService;

        public AbsenceController(IAbsenceService absenceService)
        {
            _absenceService = absenceService;
        }

        [HttpGet("GetAllAbsences")]
        public async Task<ActionResult> GetAllAbsences()
        {
            var absences = await _absenceService.GetAllAbsences();
            return Ok(absences);
        }

        [HttpPost("GetAbsencesByEmployeeId")]
        public async Task<ActionResult> GetAbsencesByEmployeeId(IFormCollection data)
        {
            if (data.TryGetValue("employeeId", out var someString))
            {
                    
                if (StringValues.IsNullOrEmpty(someString) || someString == "undefined")
                {
                    return BadRequest("Invalid employee id");
                }
                var employeeId = JsonConvert.DeserializeObject<Guid>(someString);
                if (employeeId == Guid.Empty)
                {
                    return BadRequest("Invalid employee id");

                }
                var absences = await _absenceService.GetAbsencesByEmployeeId(employeeId);
                return Ok(new BaseResult { IsSuccessfull = true, Data = absences });
            }
            else
            {
                return BadRequest("Invalid employee id");
            }
        }

        [HttpPost("CreateAbsence")]
        public async Task<ActionResult> CreateAbsence(IFormCollection absenceFormData)
        {
            if (absenceFormData.TryGetValue("absenceFormData", out var someString))
            {
                var absenceCreateDto = JsonConvert.DeserializeObject<AbsenceDto>(someString);

                if (absenceFormData.TryGetValue("employeeId", out var idString))
                {
                    var cleanIdString = idString.ToString().Trim('"');
                    var employeeId = new Guid(cleanIdString);

                    absenceCreateDto.EmployeeId = employeeId;
                    var newAbsenceId = await _absenceService.CreateAbsence(absenceCreateDto);

                    // You can now use trimmedGuidString wherever necessary
                    return Ok(new BaseResult { Data = newAbsenceId, IsSuccessfull = true });
                }
                else
                {
                    return BadRequest("The employee Id is unvalid");
                }
            }
            else
            {
                return BadRequest(ModelState.IsValid);
            }
        }


        [HttpPost("DeleteAbsence")]
        public async Task<ActionResult> DeleteAbsence(IFormCollection absenceData)
        {
            if (absenceData.TryGetValue("absenceId", out var absenceIdString))
            {
                var id = JsonConvert.DeserializeObject<Guid>(absenceIdString);

                var test = await _absenceService.DeleteAbsence(id);
                if (test.IsSuccessfull)
                {
                    return Ok(new BaseResult { IsSuccessfull = true });
                }
            }

            return BadRequest(ModelState.IsValid);
        }

        [HttpPost("UpdateAbsence")]
        public async Task<ActionResult> UpdateAbsence(IFormCollection absenceData)
        {
            if (absenceData.TryGetValue("absenceFormData", out var someString))
            {
                var absenceUpdateDto = JsonConvert.DeserializeObject<AbsenceDto>(someString);

                var updatedAbsence = await _absenceService.UpdateAbsence(absenceUpdateDto);
                return Ok(updatedAbsence);
            }
            else
                return BadRequest(ModelState.IsValid);
        }
        [HttpPost("CheckSickLeaveDeadline")]
        public async Task<ActionResult> CheckSickLeaveDeadline(IFormCollection employeeId)
        {
            var id = JsonConvert.DeserializeObject<Guid>(employeeId["employeeId"]);
            var expiredDeadline = await _absenceService.CheckSickLeaveDeadline(id);
            return Ok(expiredDeadline);
        }


    }
}
 