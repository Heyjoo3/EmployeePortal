using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Helpers;
using EmployeePortal.Core.Services;
using EmployeePortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections;
using System.Diagnostics.Eventing.Reader;
using System.Reflection.Metadata.Ecma335;

namespace EmployeePortal.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class VacationController : ControllerBase
    {
        private readonly IVacationService _vacationService;
        private readonly IUpdateVacationStatusService _updateVacationStatusService;
        private readonly IPublicHolidayService _publicHolidayService;

        public VacationController(IVacationService vacationService, IPublicHolidayService publicHolidayService, IUpdateVacationStatusService updateVacationStatusService)
        {
            _vacationService = vacationService;
            _publicHolidayService = publicHolidayService;
            _updateVacationStatusService = updateVacationStatusService;
        }

        [HttpPost("CreateVacation")]
        public async Task<ActionResult> CreateVacation(IFormCollection VacationFormData)
        {

            if (VacationFormData.TryGetValue("vacationFormData", out var someString))
            {
                var vacationCreateDto = JsonConvert.DeserializeObject<VacationDto>(someString);

                var newPersonnelId = await _vacationService.CreateVacation(vacationCreateDto);
                return Ok(new BaseResult { Data = newPersonnelId, IsSuccessfull = true });
            }
            else
                return BadRequest(ModelState.IsValid);
        }

        [HttpPost("AdminCreateVacation")]
        public async Task<ActionResult> AdminCreateVacation(IFormCollection VacationFormData)
        {

            if (VacationFormData.TryGetValue("vacationFormData", out var someString))
            {
                var vacationCreateDto = JsonConvert.DeserializeObject<VacationDto>(someString);

                var newPersonnelId = await _vacationService.AdminCreateVacation(vacationCreateDto);
                return Ok(new BaseResult { Data = newPersonnelId, IsSuccessfull = true });
            }
            else
                return BadRequest(ModelState.IsValid);
        }

        [HttpPost("DeleteVacation")]
        public async Task<ActionResult> DeleteVacation(IFormCollection vacationData)
        {
            if (vacationData.TryGetValue("vacationId", out var vacationIdString))
            {
                var id = JsonConvert.DeserializeObject<Guid>(vacationIdString);

                var test = await _vacationService.DeleteVacation(id);
                if (test.IsSuccessfull)
                {
                    return Ok(new BaseResult { IsSuccessfull = true });
                }
            }

            return BadRequest(ModelState.IsValid);

        }

        [HttpPost("UpdateVacation")]
        public async Task<ActionResult> UpdateVacation(IFormCollection vacationFormData)
        {
            if (vacationFormData.TryGetValue("vacationFormData", out var someString))
            {
                var vacationUpdateDto = JsonConvert.DeserializeObject<VacationDto>(someString);

                var updatedVacation = await _vacationService.UpdateVacation(vacationUpdateDto);

                return Ok(new BaseResult { Data = updatedVacation.Data, IsSuccessfull = true, Message = updatedVacation.Message });
            }
            else
                return BadRequest(ModelState.IsValid);
        }

        [HttpPost("AcceptVacation")]
        public async Task<ActionResult> AcceptVacation(IFormCollection VacationFormData)
        {
            if (VacationFormData.TryGetValue("vacationFormData", out var someString))
            {
                var vacationUpdateDto = JsonConvert.DeserializeObject<VacationDto>(someString);

                //var updatedVacation = await _vacationService.AcceptVacation(vacationUpdateDto);
                var updatedVacation = await _updateVacationStatusService.AcceptVacation(vacationUpdateDto);
                return Ok(new BaseResult { Data = updatedVacation, IsSuccessfull = true });
            }
            else
                return BadRequest(ModelState.IsValid);
        }


        [HttpPost("CancelVacation")]
        public async Task<ActionResult> CancelVacation(IFormCollection VacationFormData)
        {
            if (VacationFormData.TryGetValue("vacationFormData", out var someString))
            {
                var vacationUpdateDto = JsonConvert.DeserializeObject<VacationDto>(someString);

                //var updatedVacation = await _vacationService.CancelVacation(vacationUpdateDto);
                var updatedVacation = await _updateVacationStatusService.CancelVacation(vacationUpdateDto);
                return Ok(new BaseResult { Data = updatedVacation, IsSuccessfull = true });
            }
            else
                return BadRequest(ModelState.IsValid);
        }


        [HttpGet("GetAllVacations")]
        public async Task<IEnumerable<VacationDto>> GetAllVacations()
        {
            return await _vacationService.GetAllVacations();
        }

        //        [AllowAnonymous]
        [HttpGet("GetVacationsWithEmployee")]
        public async Task<IEnumerable<VacationDto>> GetVacationsWithEmployee()
        {
            var response = await _vacationService.GetVacationsWithEmployee();
            return response;
        }

        [HttpPost("GetVacationsByEmployeeId")]
        public async Task<ActionResult<IEnumerable<VacationDto>>> GetVacationsByEmployeeId(IFormCollection IdForm)
        {
            if (IdForm.TryGetValue("idData", out var someString))
            {
                var id = JsonConvert.DeserializeObject<Guid>(someString);
                var response = await _vacationService.GetVacationsByEmployeeId(id);
                return Ok(new BaseResult { Data = response, IsSuccessfull = true });
            }
            else
            {
                return BadRequest("Invalid ID data.");
            }
        }

        [HttpPost("GetAnnualVacationsByEmployeeId")]
        public async Task<ActionResult<IEnumerable<VacationDto>>> GetAnnualVacationsByEmployeeId(IFormCollection IdForm)
        {
            if (IdForm.TryGetValue("idData", out var someString))
            {
                var id = JsonConvert.DeserializeObject<Guid>(someString);
                var response = await _vacationService.GetAnnualVacationsByEmployeeId(id);
                return Ok(new BaseResult { Data = response, IsSuccessfull = true });
            }
            else
            {
                return BadRequest("Invalid ID data.");
            }
        }


        [HttpPost("GetOpenRequestWithEmployee")]
        public async Task<ActionResult<IEnumerable<RequestDto>>> GetOpenRequestWithEmployee(IFormCollection requestForm)
        {
            if (requestForm.TryGetValue("requestForm", out var someString))
            {
                var request = JsonConvert.DeserializeObject<RequestFormDto>(someString);

                var response = await _vacationService.GetOpenRequests(request);

                return Ok(new BaseResult { IsSuccessfull = true, Data = response });
            }
            else
                return BadRequest(ModelState.IsValid);
        }

        [HttpPost("CreatePublicHoliday")]
        public async Task<ActionResult> CreatePublicHoliday(IFormCollection HolidayFormData)
        {
            if (HolidayFormData.TryGetValue("holidayFormData", out var someString))
            {
                var holidayCreateDto = JsonConvert.DeserializeObject<PublicHolidayDto>(someString);

                await _publicHolidayService.CreateHoliday(holidayCreateDto);

                return Ok(new BaseResult { IsSuccessfull = true });
            }
            else
                return BadRequest(ModelState.IsValid);
        }

        [HttpPost("GetAllPublicHolidays")]
        public async Task<IEnumerable<PublicHolidayDto>> GetAllPublicHolidays(IFormCollection publicHolidayData)
        {
            publicHolidayData.TryGetValue("selectedYear", out var someString);
            var year = JsonConvert.DeserializeObject<int>(someString);

            return await _publicHolidayService.GetAllHolidaysByYear(year);
        }

        [HttpGet("GetAllYears")]
        public async Task<IEnumerable<int>> GetAllYears()
        {
            var allYears = await _publicHolidayService.GetAllYears();

            return allYears.Distinct().OrderBy(x => x);
        }

        [HttpPost("DeleteHoliday")]
        public async Task<ActionResult> DeleteHoliday(IFormCollection publicHolidayData)
        {
            if (publicHolidayData.TryGetValue("holidayId", out var holidayIdString))
            {
                var id = JsonConvert.DeserializeObject<int>(holidayIdString);

                var test = await _publicHolidayService.DeleteHoliday(id);
                if (test.IsSuccessfull)
                {
                    return Ok(new BaseResult { IsSuccessfull = true });
                }
            }

            return BadRequest(ModelState.IsValid);

        }

        // checks if a newly created Company Vacation overlaps with anyones Regular Vacation, which will then be cancelled and gets an remark
        [HttpPost("CheckCompanyVacationOverlaps")]
        public async Task<ActionResult> CheckCompanyVacationOverlaps(IFormCollection vacationFormData)
        {
            if (vacationFormData.TryGetValue("vacationFormData", out var someString))
            {
                var companyVacation = JsonConvert.DeserializeObject<VacationDto>(someString);

                var checkForOverlaps = await _vacationService.CheckForOverlaps(companyVacation);

                if (checkForOverlaps.IsSuccessfull)
                {
                    return Ok(new BaseResult { IsSuccessfull = true, Message = checkForOverlaps.Message, Data = checkForOverlaps.Data });
                }
                else
                {
                    return BadRequest(ModelState.IsValid);
                }

            }
            else
            {
                return BadRequest(ModelState.IsValid);
            }
        }

        [HttpPost("CheckForAbsenceOverlaps")]
        public async Task<ActionResult> CheckForAbsenceOverlaps(IFormCollection absenceData)
        {
            if (absenceData.TryGetValue("absenceData", out var someString))
            {
                try
                {
                    var absence = JsonConvert.DeserializeObject<AbsenceDto>(someString);

                    var checkForAbsenceOverlaps = await _vacationService.CheckForAbsenceOverlaps(absence);

                    return Ok(new BaseResult { IsSuccessfull = true, Message = checkForAbsenceOverlaps.Message, Data = checkForAbsenceOverlaps.Data });
                }
                catch (Exception ex)
                {
                    return BadRequest(new BaseResult
                    {
                        IsSuccessfull = false,
                        Message = "An unexpected error occurred: " + ex.Message
                    });
                }
            }
            else
            {
                return BadRequest("Falsy Input");
            }
        }

        [HttpPost("CalculateVacationDays")]
        public async Task<ActionResult> CalculateVacationDays(IFormCollection vacationFormData)
        {
            if (vacationFormData.TryGetValue("vacationId", out var someString))
            {
                try
                {

                    var vacationId = JsonConvert.DeserializeObject<Guid>(someString);

                    var vacationDays = await _vacationService.CalculateVacationDays(vacationId);

                    return Ok(new BaseResult { Data = vacationDays, IsSuccessfull = true });
                }
                catch (JsonSerializationException ex)
                {
                    return BadRequest("Invalid vacation data format.");
                }
            }
            else
            {
                return BadRequest("Required data is missing.");
            }
        }

        [HttpPost("CloseOpenVacationRequests")]
        public async Task<ActionResult> CloseOpenVacationRequests(IFormCollection employeeIdData)
        {
            if (employeeIdData.TryGetValue("employeeIdData", out var someString))
            {
                try
                {
                    var employeeId = JsonConvert.DeserializeObject<Guid>(someString);
                    var result = await _vacationService.CloseOpenVacationRequests(employeeId);
                    return Ok(new BaseResult { IsSuccessfull = true });
                }
                catch
                {
                    return BadRequest(ModelState.IsValid);
                }
            }
            else
            {
                return BadRequest(ModelState.IsValid);
            }
        }

        [HttpPost("GetCompanyVacations")]
        public async Task<ActionResult> GetCompanyVacations()
        {
            var result = await _vacationService.GetCompanyVacation();
            return Ok(new BaseResult { IsSuccessfull = true, Data = result });
        }

        [HttpPost("UpdateVacationsAfterPublicHolidayDeletion")]
        public async Task<ActionResult> UpdateVacationsAfterPublicHolidayDeletion(IFormCollection vacationFormData)
        {
            if (vacationFormData.TryGetValue("holidayDate", out var someString))
            {
                var holidayDate = JsonConvert.DeserializeObject<DateTime>(someString);

                var result = await _vacationService.UpdateVacationsAfterPublicHolidayDeletion(holidayDate);
                if (result.IsSuccessfull)
                {
                    return Ok(new BaseResult { IsSuccessfull = true });
                }
                else
                {
                    return BadRequest(ModelState.IsValid);
                }
            }
            else
            {
                return BadRequest("Required data is missing.");
            }
        }
    }
}
