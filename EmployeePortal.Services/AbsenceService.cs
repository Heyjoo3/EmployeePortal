using AutoMapper;
using EmployeePortal.Core.Data;
using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Helpers;
using EmployeePortal.Core.Models;
using EmployeePortal.Core.Repositories;
using EmployeePortal.Core.Services;

namespace EmployeePortal.Services
{
    public class AbsenceService : IAbsenceService
    {
        private readonly IAbsenceRepository _absenceRepository;
        private readonly IMapper _mapper;
        private readonly EmployeePortalContext _employeePlannerContext;

        public AbsenceService(
            IAbsenceRepository absenceRepository,
            IMapper mapper,
            EmployeePortalContext employeePlannerContext)
        {
            _employeePlannerContext = employeePlannerContext;
            _mapper = mapper;
            _absenceRepository = absenceRepository;
        }

        public async Task<Guid> CreateAbsence(AbsenceDto absenceDto)
        {
            var absence = _mapper.Map<Absence>(absenceDto);
            var guid = absenceDto.EmployeeId;

            if (guid == Guid.Empty)
            {
                throw new Exception("Invalid employee id");
            }

            var employeeIdString = guid.ToString();
            var employee = await _employeePlannerContext.Employees.FindAsync(employeeIdString);

            if (employee == null)
            {
                throw new Exception("Employee not found");
            }

            absence.EmployeeId = new Guid(employee.Id);

            absence.Duration = CalculateDays(absence);

            await _absenceRepository.Add(absence);
            await _absenceRepository.Commit();

            return absence.AbsenceId;
        }


        public async Task<BaseResult> DeleteAbsence(Guid absenceId)
        {
            var absence = await _absenceRepository.GetById(absenceId);
            if (absence == null)
            {
                return new BaseResult { IsSuccessfull = false, Message = "Absence not found" };
            }
            _absenceRepository.Remove(absence);
            if (await _absenceRepository.Commit() > 0 )
            {
                return new BaseResult { IsSuccessfull = true };
            }
            return new BaseResult { IsSuccessfull = false, Message = "Failed to delete absence" };
        }

        public async Task<IEnumerable<AbsenceDto>> GetAbsencesByEmployeeId(Guid employeeId)
        {
            var absences = await  _absenceRepository.GetAbsencesByEmployeeId(employeeId);
            return _mapper.Map<IEnumerable<AbsenceDto>>(absences);
        }

        public async Task<IEnumerable<AbsenceDto>> GetAllAbsences()
        {
            var absences = await _absenceRepository.GetAllAbsences();
            return _mapper.Map<IEnumerable<AbsenceDto>>(absences);
            // return absences.Select(x => _mapper.Map<Absence, AbsenceDto>(x));

        }

        public async Task<BaseResult> UpdateAbsence(AbsenceDto absenceDto)
        {
            var absenceData = _mapper.Map<Absence>(absenceDto);

            var absence =await  _absenceRepository.GetById(absenceData.AbsenceId);
            
            if (absence == null)
            {
                throw new Exception("Absence not found");
            }

            absence.StartDate = absenceData.StartDate;
            absence.EndDate = absenceData.EndDate;
            absence.AbsenceType = absenceData.AbsenceType;
            absence.Duration = absenceData.Duration;
            if (absence.HasSickLeave == false && absenceDto.HasSickLeave== true)
            {
                //absence.Remarks = absenceData.Remarks.Replace("AU nicht pünktlich abgegeben.", "");
            }
            else if (absence.HasSickLeave == true && absenceDto.HasSickLeave == false)
            {
                var employee = await _employeePlannerContext.Employees.FindAsync(absenceDto.EmployeeId.ToString());
                int deadline = (int)employee.SickNoteDeadLine;
                DateTime startDate = absence.StartDate;
                DateTime today = DateTime.Now;

                if (startDate.AddDays(deadline) < today)
                {
                    if (!absence.Remarks.Contains("AU nicht pünktlich abgegeben.") && absence.Remarks.Trim() != "")
                    {
                        absence.Remarks = " - AU nicht pünktlich abgegeben.";
                    }
                    else if (!absence.Remarks.Contains("AU nicht pünktlich abgegeben."))
                    {
                        absence.Remarks += "AU nicht pünktlich abgegeben.";
                    }
                    
                }
            }
            else
            {
                absence.Remarks = absenceData.Remarks;

            }

            absence.HasSickLeave = absenceData.HasSickLeave;

            absence.HasStartedShift = absenceData.HasStartedShift;

            absence.Duration = CalculateDays(absence);

            var absences = await _absenceRepository.UpdateAbsence(absence);

            return new BaseResult { IsSuccessfull = true, Message = "Absence updated successfully", Data = absences};
        }

        public async Task<BaseResult> CheckSickLeaveDeadline(Guid EmployeeId)
        {
            int deadline = 0;
            var employee = await _employeePlannerContext.Employees.FindAsync(EmployeeId.ToString());
            if (employee == null)
            {
                return new BaseResult { IsSuccessfull = false, Message = "Employee not found" };
            }
            else if (employee.SickNoteDeadLine == null)
            {
                return new BaseResult { IsSuccessfull = false, Message = "No deadline found" };
            }
            else
            {

                 deadline = (int)employee.SickNoteDeadLine;
            }

            var unexcusedSickLeaves = await _absenceRepository.GetUnexcusedSickLeaves(EmployeeId);
            var unexcusedSickLeavesList = unexcusedSickLeaves.ToList();
            unexcusedSickLeavesList = unexcusedSickLeaves.Where(x => x.Duration >= deadline || x.Remarks.Contains("AU nicht pünktlich abgegeben.")).ToList();

            if (unexcusedSickLeaves == null)
            {
                return new BaseResult { IsSuccessfull = true, Message = "No unexcused sick leaves found" };
            }

            for (int i = 0; i < unexcusedSickLeavesList.Count; i++)
            {
                DateTime startDate = unexcusedSickLeavesList[i].StartDate;
                DateTime today = DateTime.Now;

                if (unexcusedSickLeavesList[i].Duration < deadline)
                {
                    unexcusedSickLeavesList[i].Remarks = unexcusedSickLeavesList[i].Remarks.Replace("AU nicht pünktlich abgegeben.", "");
                }
                if (startDate.AddDays(deadline + 2) < today)
                {
                    if (!unexcusedSickLeavesList[i].Remarks.Contains("AU nicht pünktlich abgegeben."))
                    {
                        unexcusedSickLeavesList[i].Remarks += " - AU nicht pünktlich abgegeben.";
                    }
                    unexcusedSickLeavesList[i].HasSickLeave = false;
                    await _absenceRepository.UpdateAbsence(unexcusedSickLeavesList[i]);
                }
                else if (startDate.AddDays(deadline + 2) > today && unexcusedSickLeavesList[i].Remarks.Contains("AU nicht pünktlich abgegeben."))
                {
                    unexcusedSickLeavesList[i].Remarks = unexcusedSickLeavesList[i].Remarks.Replace("AU nicht pünktlich abgegeben.", "");
                    unexcusedSickLeavesList[i].HasSickLeave = false;
                    await _absenceRepository.UpdateAbsence(unexcusedSickLeavesList[i]);
                }
            }

            return new BaseResult { IsSuccessfull = true, Message = "Abgebefristen geprüft" };
        }
        private float CalculateDays(Absence absence)
        {
            DateTime start = absence.StartDate.Date;
            DateTime end = absence.EndDate.Date;

            if (end < start)
                throw new ArgumentException("End date cannot be before start date.");

            var publicHolidays = _employeePlannerContext.PublicHolidays
                .Where(ph => ph.Date >= start && ph.Date <= end)
                .Select(ph => ph.Date)
                .ToList();

            float totalDays = 0;
            for (DateTime date = start; date <= end; date = date.AddDays(1))
            {
                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                if (publicHolidays.Contains(date))
                    continue;

                totalDays += 1;
            }

            return totalDays;
        }
    }
}


