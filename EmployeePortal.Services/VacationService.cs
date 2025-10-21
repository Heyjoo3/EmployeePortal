using AutoMapper;
using EmployeePortal.Core.Data;
using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Helpers;
using EmployeePortal.Core.Models;
using EmployeePortal.Core.Repositories;
using EmployeePortal.Core.Services;
using EmployeePortal.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace EmployeePortal.Services
{
    public class VacationService : IVacationService
    {
        private readonly IVacationRepository _vacationRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAbsenceRepository _absenceRepository;
        private readonly IPublicHolidayRepository _publicHolidayRepository;
        private readonly IMapper _mapper;
        private readonly EmployeePortalContext _employeePlannerContext;
        private readonly IVacationStatusEmailService _vacationStatusEmailService;

        public VacationService(
            IVacationRepository vacationRepository,
            IEmployeeRepository employeeRepository,
            IAbsenceRepository absenceRepository,
            IPublicHolidayRepository publicHolidayRepository,
            IMapper mapper,
            EmployeePortalContext employeePlannerContext,
            IVacationStatusEmailService vacationStatusEmailService
            )
        {
            _employeePlannerContext = employeePlannerContext;
            _mapper = mapper;
            _vacationRepository = vacationRepository;
            _employeeRepository = employeeRepository;
            _absenceRepository = absenceRepository;
            _publicHolidayRepository = publicHolidayRepository;
            _vacationStatusEmailService = vacationStatusEmailService;
        }

        public async Task<IEnumerable<VacationDto>> GetAllVacations()
        {
            var vacationData = await _vacationRepository.GetAllVacations();
            return vacationData.Select(x => _mapper.Map<Vacation, VacationDto>(x));
        }

        public async Task<IEnumerable<VacationDto>> GetVacationsWithEmployee()
        {
            var vacationData = await _vacationRepository.GetVacationsWithEmployee();
            var mappedVacations = vacationData.Select(x => _mapper.Map<Vacation, VacationDto>(x));
            return mappedVacations;
        }
        public async Task<IEnumerable<VacationDto>> GetVacationsByEmployeeId(Guid id)
        {
            var vacationData = await _vacationRepository.GetByEmployeeId(id);
            var mappedVacations = vacationData.Select(x => _mapper.Map<Vacation, VacationDto>(x));
            return mappedVacations;
        }

        public async Task<IEnumerable<VacationDto>> GetAnnualVacationsByEmployeeId(Guid id)
        {
            var vacationData = await _vacationRepository.GetAnnualByEmployeeId(id);
            var mappedVacations = vacationData.Select(x => _mapper.Map<Vacation, VacationDto>(x));
            return mappedVacations;
        }

        public async Task<IEnumerable<RequestDto>> GetOpenRequests(RequestFormDto requestForm)
        {
            var employeeId = requestForm.EmployeeId;
            var role = requestForm.Role;
            var result = new List<RequestDto>();

            async Task AddVacationsToResult(IEnumerable<Vacation> vacations, string relation)
            {
                foreach (var vacation in vacations)
                {
                    var employee = await _employeeRepository.FindByIdAsync(vacation.EmployeeId.ToString());
                    result.Add(new RequestDto
                    {
                        VacationId = vacation.VacationId,
                        VacationType = vacation.Type,
                        StartDate = vacation.StartDate,
                        VacationStatus = vacation.Status,
                        EndDate = vacation.EndDate,
                        EmployeeId = employee.Id,
                        EmployeeFirstName = employee.FirstName,
                        EmployeeLastName = employee.LastName,
                        EmployeeInternalId = (long)employee.EmployeeInternalId,
                        Relation = relation
                    });
                }
            }

            if (role == "Admin")
            {
                var substitutedAdminVacations = await _vacationRepository.GetRequestsSubstituted(employeeId);
                var adminVacations = await _vacationRepository.GetRequestsAdmin(employeeId);

                await AddVacationsToResult(substitutedAdminVacations, "admin");
                await AddVacationsToResult(adminVacations, "admin");
            }
            else
            {
                var supervisedVacations = await _vacationRepository.GetRequestsSupervised(employeeId);
                var substitutedVacations = await _vacationRepository.GetRequestsSubstituted(employeeId);

                await AddVacationsToResult(supervisedVacations, "supervisor");
                await AddVacationsToResult(substitutedVacations, "substitute");
            }

            var employeeVacations = await _vacationRepository.GetRequestsEmployee(employeeId);
            await AddVacationsToResult(employeeVacations, "ownRequest");

            return result;
        }

        public async Task<Guid> CreateVacation(VacationDto vacationDto)
        {
            Vacation vacationData = _mapper.Map<VacationDto, Vacation>(vacationDto);

            var dateArray = await FilterAffectedVacations(vacationData, _publicHolidayRepository, _absenceRepository);
            vacationData.Vacationdays = CalculateDays(dateArray, vacationData.StartDate, vacationData.EndDate, vacationData.IsHalfStartDay, vacationData.IsHalfEndDay);

            if (vacationDto.Type != "Betriebsurlaub")
            {
                var existingEmployee = await _employeeRepository.FindByIdAsync(vacationDto.EmployeeId.ToString());
                if (existingEmployee == null)
                {
                    throw new Exception("Employee not found");
                }


                vacationData.EmployeeId = new Guid(existingEmployee.Id);
                vacationData.Status = VacationStatus.VacationPending;
                vacationData.SupervisorStatus = VacationStatus.VacationPending;

                if (vacationData.Substitute == null)
                {
                    vacationData.SubstituteStatus = VacationStatus.VacationApproved;
                }
                else
                {
                    vacationData.SubstituteStatus = VacationStatus.VacationPending;
                }

                vacationData.AdminStatus = VacationStatus.NotRelevant;
            }
            else
            {
                vacationData.EmployeeId = null;
            }

            await _vacationRepository.Add(vacationData);

            await _vacationRepository.Commit();

            await _vacationStatusEmailService.SendEmailNotification(vacationDto, "createVacation");
            return vacationData.VacationId;
        }


        public async Task<Guid> AdminCreateVacation(VacationDto vacationDto)
        {
            Vacation vacationData = _mapper.Map<VacationDto, Vacation>(vacationDto);

            var dateArray = await FilterAffectedVacations(vacationData, _publicHolidayRepository, _absenceRepository);
            vacationData.Vacationdays = CalculateDays(dateArray, vacationData.StartDate, vacationData.EndDate, vacationData.IsHalfStartDay, vacationData.IsHalfEndDay);


            var existingEmployee = await _employeeRepository.FindByIdAsync(vacationDto.EmployeeId.ToString());
            if (existingEmployee == null)
            {
                throw new Exception("Employee not found");
            }


            vacationData.EmployeeId = new Guid(existingEmployee.Id);


            await _vacationRepository.Add(vacationData);

            await _vacationRepository.Commit();
            return vacationData.VacationId;
        }

        public async Task<BaseResult> DeleteVacation(Guid vacationId)
        {
            var vacation = await _vacationRepository.GetById(vacationId);
            _vacationRepository.Remove(vacation);
            if (await _vacationRepository.Commit() > 0)
            {
                return new BaseResult { IsSuccessfull = true };
            }

            return new BaseResult { IsSuccessfull = false };
        }

        public async Task<BaseResult> UpdateVacation(VacationDto vacationDto)
        {
            var vacationData = _mapper.Map<VacationDto, Vacation>(vacationDto);


            var vacation = await _vacationRepository.GetById(vacationDto.VacationId);
            if (vacation == null)
            {
                return new BaseResult { IsSuccessfull = false, Message = "Vacation not found" };
            }

            UpdateVacationData(vacation, vacationData);

            vacation.EmployeeId = vacationDto.EmployeeId;

            //vacationData.Vacationdays = await CalculateVacationDays(vacationData.VacationId);
            var dateArray = await FilterAffectedVacations(vacation, _publicHolidayRepository, _absenceRepository);

            var vacationDays = CalculateDays(dateArray, vacation.StartDate, vacation.EndDate, vacation.IsHalfStartDay, vacation.IsHalfEndDay);
            vacation.Vacationdays = vacationDays;



            //_vacationRepository.Update(vacation);

            if (await _vacationRepository.Commit() > 0)
            {
                return new BaseResult { IsSuccessfull = true, Data = vacation, Message = "ok" };
            }
            return new BaseResult { IsSuccessfull = false, Message = "Unable to Save Vacation Update" };
        }

        public async Task<float> CalculateVacationDays(Guid vacationId)
        {
            var vacation = await _vacationRepository.GetById(vacationId);

            if (vacation == null)
            {
                throw new Exception("Vacation not found");
            }

            var dateArray = await FilterAffectedVacations(vacation, _publicHolidayRepository, _absenceRepository);

            var vacationDays = CalculateDays(dateArray, vacation.StartDate, vacation.EndDate, vacation.IsHalfStartDay, vacation.IsHalfEndDay);

            vacation.Vacationdays = vacationDays;

            return vacationDays;

            //if (await _vacationRepository.Commit() > 0)
            //{
            //    return vacationDays;
            //}
            //else
            //{
            //    throw new Exception("Unable to Save Vacation Update");
            //}
        }

        public async Task<BaseResult> CheckForOverlaps(VacationDto companyVacationDataDto)
        {
            var companyVacationData = _mapper.Map<VacationDto, Vacation>(companyVacationDataDto);
            var vacations = await _vacationRepository.GetOverlappedVacations(companyVacationData.StartDate, companyVacationData.EndDate);

            if (vacations == null || !vacations.Any())
            {

                if (vacations == null || !vacations.Any())
                {
                    await _vacationRepository.Add(companyVacationData);
                    return new BaseResult { IsSuccessfull = true };
                }
            }

            bool changesMade = false;
            foreach (var vacationOriginal in vacations)
            {
                await HandleOverlappingVacation(vacationOriginal, companyVacationData);
                changesMade = true;
            }

            return new BaseResult { IsSuccessfull = true };
        }

        public async Task<BaseResult> CheckForAbsenceOverlaps(AbsenceDto absenceDto)
        {
            var absence = _mapper.Map<AbsenceDto, Absence>(absenceDto);
            var vacations = await _vacationRepository.GetOverlappedByAbsence((Guid)absence.EmployeeId, absence.StartDate, absence.EndDate);

            if (vacations == null || !vacations.Any())
            {
                return new BaseResult { IsSuccessfull = true, Data = Array.Empty<object>(), Message = "No overlaps" };
            }

            return new BaseResult { Message = "Overlaps found", IsSuccessfull = true, Data = vacations };
        }

        public async Task<BaseResult> CloseOpenVacationRequests(Guid employeeId)
        {
            var vacations = await _vacationRepository.GetByEmployeeId(employeeId);

            foreach (var vacation in vacations)
            {
                if (vacation.Status == VacationStatus.VacationPending && vacation.StartDate < DateTime.Today)
                {
                    vacation.Status = VacationStatus.VacationRejected;
                }
                else if (vacation.Status == VacationStatus.CancellationPending && vacation.StartDate < DateTime.Today)
                {
                    vacation.Status = VacationStatus.CancellationRejected;
                }

                await _vacationRepository.UpdateVacation(vacation);
            }

            return new BaseResult { IsSuccessfull = true };
        }

        public async Task<IEnumerable<VacationDto>> GetCompanyVacation()
        {
            var vacationData = await _vacationRepository.GetCompanyVacations();
            return vacationData.Select(x => _mapper.Map<Vacation, VacationDto>(x));
        }

        public async Task<BaseResult> UpdateVacationsAfterPublicHolidayDeletion(DateTime publicHolidayDate)
        {
            try
            {
                var vacations = await _vacationRepository.GetOverlappedVacations(publicHolidayDate, publicHolidayDate);

                foreach (var vacation in vacations)
                {
                    float vacationDays = await CalculateVacationDays(vacation.VacationId);
                    vacation.Vacationdays = vacationDays;
                    await _vacationRepository.UpdateVacation(vacation);
                }
                return new BaseResult
                {
                    IsSuccessfull = true,
                    Message = "Vacations updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new BaseResult
                {
                    IsSuccessfull = false,
                    Message = $"An error occurred while updating vacations: {ex.Message}"
                };
            }
        }


        // PRIVATE HELPER METHODS

        // Used Overllapping Vacations
        private async Task<bool> HandleOverlappingVacation(Vacation vacationOriginal, Vacation companyVacationData)
        {
            // Retrieve affected vacations for calculating vacation days
            var dateArray = await FilterAffectedVacations(vacationOriginal, _publicHolidayRepository, _absenceRepository);
            bool changesMade = false;

            // Case 1: Vacation lies entirely within the company vacation period
            if (vacationOriginal.StartDate >= companyVacationData.StartDate && vacationOriginal.EndDate <= companyVacationData.EndDate)
            {
                _vacationRepository.Remove(vacationOriginal);
                changesMade = true;
            }
            // Case 2: Vacation spans both before and after the company vacation
            else if (vacationOriginal.StartDate < companyVacationData.StartDate && vacationOriginal.EndDate > companyVacationData.EndDate)
            {
                // Split vacation into two parts: before and after the company vacation
                var firstPart = new Vacation
                {
                    StartDate = vacationOriginal.StartDate,
                    EndDate = companyVacationData.StartDate.AddDays(-1),
                    IsHalfStartDay = vacationOriginal.IsHalfStartDay,
                    IsHalfEndDay = false, // End before company vacation
                    Substitute = vacationOriginal.Substitute,
                    Type = vacationOriginal.Type,
                    Vacationdays = CalculateDays(dateArray, vacationOriginal.StartDate, companyVacationData.StartDate.AddDays(-1)),
                    Description = vacationOriginal.Description,
                    Status = vacationOriginal.Status,
                    EmployeeStatus = vacationOriginal.EmployeeStatus,
                    SupervisorStatus = vacationOriginal.SupervisorStatus,
                    SubstituteStatus = vacationOriginal.SubstituteStatus,
                    AdminStatus = vacationOriginal.AdminStatus,
                    EmployeeId = vacationOriginal.EmployeeId
                };

                var secondPart = new Vacation
                {
                    StartDate = companyVacationData.EndDate.AddDays(1),
                    EndDate = vacationOriginal.EndDate,
                    IsHalfStartDay = false,  // Start after company vacation
                    IsHalfEndDay = vacationOriginal.IsHalfEndDay,
                    Substitute = vacationOriginal.Substitute,
                    Type = vacationOriginal.Type,
                    Vacationdays = CalculateDays(dateArray, companyVacationData.EndDate.AddDays(1), vacationOriginal.EndDate),
                    Description = vacationOriginal.Description,
                    Status = vacationOriginal.Status,
                    EmployeeStatus = vacationOriginal.EmployeeStatus,
                    SupervisorStatus = vacationOriginal.SupervisorStatus,
                    SubstituteStatus = vacationOriginal.SubstituteStatus,
                    AdminStatus = vacationOriginal.AdminStatus,
                    EmployeeId = vacationOriginal.EmployeeId
                };

                await _vacationRepository.Add(firstPart);
                await _vacationRepository.Add(secondPart);
                _vacationRepository.Remove(vacationOriginal);
                await _vacationRepository.Commit();
                changesMade = true;
            }
            // Case 3: Vacation overlaps at the start of the company vacation
            else if (vacationOriginal.StartDate < companyVacationData.StartDate && vacationOriginal.EndDate >= companyVacationData.StartDate && vacationOriginal.EndDate <= companyVacationData.EndDate)
            {
                vacationOriginal.EndDate = companyVacationData.StartDate.AddDays(-1);
                vacationOriginal.IsHalfEndDay = false;
                vacationOriginal.Vacationdays = CalculateDays(dateArray, vacationOriginal.StartDate, vacationOriginal.EndDate);
                vacationOriginal.Description += " Geändert aufgrund von Überschneidung mit Betriebsurlaub";
                _vacationRepository.Update(vacationOriginal);
                await _vacationRepository.Commit();
                changesMade = true;
            }
            // Case 4: Vacation overlaps at the end of the company vacation
            else if (vacationOriginal.StartDate <= companyVacationData.EndDate && vacationOriginal.StartDate >= companyVacationData.StartDate && vacationOriginal.EndDate > companyVacationData.EndDate)
            {
                vacationOriginal.StartDate = companyVacationData.EndDate.AddDays(1);
                vacationOriginal.IsHalfStartDay = false;
                vacationOriginal.Vacationdays = CalculateDays(dateArray, vacationOriginal.StartDate, vacationOriginal.EndDate);
                vacationOriginal.Description += " Geändert aufgrund von Überschneidung mit Betriebsurlaub";
                _vacationRepository.Update(vacationOriginal);
                await _vacationRepository.Commit();
                changesMade = true;
            }

            return changesMade;
        }

        // Used for Caclulate Vacation Days

        // calculates the number of vacation days between the start and end date and removes half days if necessary
        private float CalculateDays(List<DateTime> dateArray, DateTime startDate, DateTime endDate, bool? isHalfStartDay = null, bool? isHalfEndDay = null)
        {
            if (dateArray == null || dateArray.Count == 0)
            {
                return 0;
            }

            dateArray = dateArray.Where(date => date >= startDate && date <= endDate).ToList();

            dateArray.Sort();

            var vacationDays = (float)dateArray.Count;

            if (startDate == endDate && (isHalfEndDay == true || isHalfStartDay == true))
            {
                return 0.5f;
            }

            if (isHalfStartDay == true && dateArray[0] == startDate)
            {
                vacationDays -= 0.5f;
            }

            if (isHalfEndDay == true && dateArray[^1] == endDate)
            {
                vacationDays -= 0.5f;
            }

            var formattedDateArray = new List<DateTime>();
            foreach (var date in dateArray)
            {
                formattedDateArray.Add(date);
            }
            var publicHolidays = _publicHolidayRepository.GetPublicHolidaysByDateRange(startDate, endDate).Result;

            foreach (var publicHoliday in publicHolidays)
            {
                if (formattedDateArray.Contains(publicHoliday.Date))
                {
                    vacationDays -= 1;
                }
            }
            return vacationDays;
        }

        // removes all public holidays and sick days and weekends from the vacation
        private async Task<List<DateTime>> FilterAffectedVacations(Vacation vacationOriginal, IPublicHolidayRepository _publicHolidayRepository, IAbsenceRepository _absenceRepository)
        {
            var employeeId = vacationOriginal.EmployeeId;
            var start = vacationOriginal.StartDate;
            var end = vacationOriginal.EndDate;

            var absencesDates = new List<Absence>();

            if (employeeId != null)
            {
                var employee = await _employeeRepository.FindByIdAsync(employeeId.ToString());
                var absences = await _absenceRepository.GetSickDaysByEmployeeAndDateRange(employeeId.ToString(), start, end, employee.SickNoteDeadLine);
                absencesDates = absences.ToList();
            }

            // get all public holidays during the vacation
            var publicHolidaysDates = await _publicHolidayRepository.GetPublicHolidaysByDateRange(vacationOriginal.StartDate, vacationOriginal.EndDate);

            // get all dates between the start and end date of the vacation, including start and end date
            var dateArray = DateRange(vacationOriginal.StartDate, vacationOriginal.EndDate);

            dateArray = RemoveWeekends(dateArray);

            if (publicHolidaysDates != null)
            {
                dateArray = RemovePublicHolidays(dateArray, (List<PublicHoliday>)publicHolidaysDates);
            }

            if (absencesDates != null && absencesDates.Count != 0)
            {
                int originalArrayLength = dateArray.Count;
                dateArray = RemoveSickDays(dateArray, (List<Absence>)absencesDates);

                if (dateArray.Count < originalArrayLength)
                {
                    if (
                        vacationOriginal.Description == null ||
                        !vacationOriginal.Description.Contains("Krankheit verkürzt Urlaubsdauer")
                        )
                    {
                        vacationOriginal.Description = vacationOriginal.Description == null
                            ? "Krankheit verkürzt Urlaubsdauer"
                            : vacationOriginal.Description + " Krankheit verkürzt Urlaubsdauer";
                    }

                    //var vacation = await _vacationRepository.GetById(vacationOriginal.VacationId);
                    //vacation.Description = vacationOriginal.Description;
                    //await _vacationRepository.Commit();
                }
            }
            else if (vacationOriginal.Description != null && vacationOriginal.Description.Contains("Krankheit verkürzt Urlaubsdauer"))
            {
                vacationOriginal.Description = vacationOriginal.Description.Replace("Krankheit verkürzt Urlaubsdauer", "");
            }

            return dateArray;
        }

        private List<DateTime> RemoveSickDays(List<DateTime> dateArray, List<Absence> absences)
        {
            HashSet<DateTime> sickDays = new HashSet<DateTime>();

            foreach (var absence in absences)
            {
                for (var date = absence.StartDate; date <= absence.EndDate; date = date.AddDays(1))
                {
                    sickDays.Add(date);
                }
            }

            return dateArray.Where(date => !sickDays.Contains(date)).ToList();
        }

        private List<DateTime> RemovePublicHolidays(List<DateTime> dateArray, List<PublicHoliday> publicHolidays)
        {
            // Create a HashSet of all public holidays
            // HashSet is used for faster lookup and to avoid duplicates
            HashSet<DateTime> holidayDates = new HashSet<DateTime>(publicHolidays.Select(h => h.Date));

            return dateArray.Where(date => !holidayDates.Contains(date)).ToList();
        }

        private List<DateTime> RemoveWeekends(List<DateTime> dateArray)
        {
            List<DateTime> result = new List<DateTime>();

            foreach (var date in dateArray)
            {
                if (!IsWeekend(date))
                {
                    result.Add(date);
                }
            }

            return result;
        }

        private bool IsWeekend(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }

        // creates a list of all dates between the start and end date of the vacation
        private List<DateTime> DateRange(DateTime startDate, DateTime endDate)
        {
            var dateArray = new List<DateTime>();
            var currentDate = startDate;
            while (currentDate <= endDate)
            {
                dateArray.Add(currentDate);
                currentDate = currentDate.AddDays(1);
            }
            return dateArray;
        }

        // Used to Update Vacation
        private void UpdateVacationData(Vacation vacation, Vacation vacationData)
        {
            vacation.StartDate = vacationData.StartDate;
            vacation.EndDate = vacationData.EndDate;
            vacation.IsHalfStartDay = vacationData.IsHalfStartDay;
            vacation.IsHalfEndDay = vacationData.IsHalfEndDay;
            vacation.Supervisor = vacationData.Supervisor;
            vacation.Substitute = vacationData.Substitute;
            vacation.Type = vacationData.Type;
            vacation.Vacationdays = vacationData.Vacationdays;
            vacation.Description = vacationData.Description;
            vacation.Status = vacationData.Status;
            vacation.EmployeeStatus = vacationData.EmployeeStatus;
            vacation.SupervisorStatus = vacationData.SupervisorStatus;
            vacation.SubstituteStatus = vacationData.SubstituteStatus;
            vacation.AdminStatus = vacationData.AdminStatus;
        }
    }
}
