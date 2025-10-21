using EmployeePortal.Core.Data;
using EmployeePortal.Core.Dto;
using EmployeePortal.Core.Models;
using EmployeePortal.Core.Repositories;
using EmployeePortal.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EmployeePortal.Data
{
    public class VacationRepository: Repository<Vacation>, IVacationRepository
    {

        public VacationRepository(EmployeePortalContext context) : base(context) { }

        private EmployeePortalContext EmployeePortalContext
        {
            get { return _context as EmployeePortalContext; }
        }

        public async new Task Add(Vacation vacationData)
        {
            await EmployeePortalContext.Vacations.AddAsync(vacationData);
        }

      
        public async Task<IEnumerable<Vacation>> GetAllVacations()
        {
            var vacations = await EmployeePortalContext.Vacations.ToListAsync();
            return vacations ?? Enumerable.Empty<Vacation>();
        }

        public async Task<IEnumerable<Vacation>> GetVacationsWithEmployee()
        {
            List<Vacation> vacations = await EmployeePortalContext.Vacations
                .Select(v => new Vacation
                   {
                       VacationId = v.VacationId,
                       StartDate = v.StartDate,
                       EndDate = v.EndDate,
                       IsHalfStartDay = v.IsHalfStartDay,
                       IsHalfEndDay = v.IsHalfEndDay,
                       Supervisor = v.Supervisor,
                       Substitute = v.Substitute,
                       Type = v.Type,
                       Status = v.Status,
                       EmployeeStatus = v.EmployeeStatus,
                       SupervisorStatus = v.SupervisorStatus,
                       SubstituteStatus = v.SubstituteStatus,
                       AdminStatus = v.AdminStatus,
                       Vacationdays = v.Vacationdays,
                       Description = v.Description,
                       EmployeeId = v.EmployeeId,
                })
                .ToListAsync();
            return vacations;
        }

        public async Task<IEnumerable<Vacation>> GetByEmployeeId(Guid Id)
        {
            List<Vacation> vacations = await EmployeePortalContext.Vacations.Where(v => v.EmployeeId == Id).ToListAsync();
            return vacations;
        }

        public async Task<IEnumerable<Vacation>> GetAnnualByEmployeeId(Guid Id)
        {
            List<Vacation> vacations = await EmployeePortalContext.Vacations.Where(v => v.EmployeeId == Id && v.Type == "Jahresurlaub").ToListAsync();
            return vacations;
        }

        public async new Task<Vacation> GetById(Guid vacationId)
        {
            var vacation = await EmployeePortalContext.Vacations.Where(x => x.VacationId == vacationId).FirstOrDefaultAsync();
            return vacation;
        }

        public async Task<IEnumerable<Vacation>> UpdateVacation(Vacation vacation)
        {
            EmployeePortalContext.Vacations.Update(vacation);
            await EmployeePortalContext.SaveChangesAsync();
            return await EmployeePortalContext.Vacations.ToListAsync();
        }

        public async Task<IEnumerable<Vacation>> GetOverlappedVacations(DateTime vacationStart, DateTime vacationEnd)
        {
            var startDate = vacationStart.Date;
            var endDate = vacationEnd.Date;
            var vacations = await EmployeePortalContext.Vacations.ToListAsync();

            var filteredVacations = vacations.Where(v => v.EmployeeId != null)
                .Where(v => v.Status != VacationStatus.CancellationApproved)
                .Where(v =>
                (v.StartDate.Date >= startDate && v.EndDate.Date <= endDate) ||
                (v.StartDate.Date <= startDate && v.EndDate.Date >= endDate) ||
                (v.StartDate.Date <= startDate && v.EndDate.Date >= startDate && v.EndDate.Date <= endDate) ||
                (v.StartDate.Date >= startDate && v.StartDate.Date <= startDate && v.EndDate.Date >= endDate)).ToList();

            return filteredVacations;
        }

        public async Task<IEnumerable<Vacation>> GetOverlappedByAbsence(Guid employeeId, DateTime absenceStart, DateTime absenceEnd)
        {
            var vacations = await EmployeePortalContext.Vacations
                .Where(v => v.EmployeeId == employeeId)
                .ToListAsync();

            var overlaps = vacations.Where(v =>
                (v.StartDate.Date >= absenceStart && v.EndDate.Date <= absenceEnd) ||
                (v.StartDate.Date <= absenceStart && v.EndDate.Date >= absenceEnd) ||
                (v.StartDate.Date <= absenceStart && v.EndDate.Date >= absenceStart && v.EndDate.Date <= absenceEnd) ||
                (v.StartDate.Date >= absenceStart && v.StartDate.Date <= absenceStart && v.EndDate.Date >= absenceEnd))
            .ToList();

            return overlaps; 

        }

        public async Task<IEnumerable<Employee>> GetSupervisedEmployees(Guid employeeId)
        {
            var vacations = await EmployeePortalContext.Vacations
                .Where(v => v.Supervisor == employeeId.ToString())
                .ToListAsync();

            var supervisedEmployees = await EmployeePortalContext.Employees
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToListAsync();

            var result = supervisedEmployees
                .Where(e => vacations.Any(v => v.EmployeeId.ToString() == e.Id))
                .ToList();
            return result;
        }

        public async Task<IEnumerable<Employee>> GetSubstitutedEmployees(Guid employeeId)
        {
            var vacations = await EmployeePortalContext.Vacations
                .Where(v => v.Substitute == employeeId.ToString())
                .ToListAsync();

            var substitutedEmployeeIds = vacations.Select(v => v.EmployeeId.ToString()).ToList();

            var substitutedEmployees = await EmployeePortalContext.Employees
                .Where(e => substitutedEmployeeIds.Contains(e.Id))
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToListAsync();

            return substitutedEmployees;
        }

        public async Task<IEnumerable<Vacation>> GetCompanyVacations()
        {
            var vacations = await EmployeePortalContext.Vacations
                .Where(v => v.EmployeeId == null)
                .ToListAsync();
            return vacations;
        }

        public async Task<IEnumerable<Vacation>> GetRequestsAdmin(Guid employeeId)
        {
            var vacations = await EmployeePortalContext.Vacations
                .Where(v => (v.Status == VacationStatus.VacationApproved && v.AdminStatus == VacationStatus.VacationPending && v.Status == VacationStatus.VacationApproved) ||
                            (v.Status == VacationStatus.CancellationApproved && v.AdminStatus == VacationStatus.CancellationPending))
                .ToListAsync();
            return vacations;
        }

        public async Task<IEnumerable<Vacation>> GetRequestsSupervised(Guid employeeId)
        {
            var vacations = await EmployeePortalContext.Vacations
                .Where(v => v.Supervisor == employeeId.ToString())
                .Where(v => (v.SupervisorStatus == VacationStatus.VacationPending && v.SubstituteStatus == VacationStatus.VacationApproved && v.Status == VacationStatus.VacationPending) ||
                      (v.SupervisorStatus == VacationStatus.CancellationPending && v.Status == VacationStatus.CancellationPending))
                .ToListAsync();
            return vacations;
        }

        public async Task<IEnumerable<Vacation>> GetRequestsSubstituted(Guid employeeId)
        {
            var vacations = await EmployeePortalContext.Vacations
                .Where(v => v.Substitute == employeeId.ToString())
                .Where(v => (v.SubstituteStatus == VacationStatus.VacationPending && v.Status == VacationStatus.VacationPending) ||
                      (v.SubstituteStatus == VacationStatus.CancellationPending && v.Status == VacationStatus.CancellationPending))
                .ToListAsync();
            return vacations;
        }

        public async Task<IEnumerable<Vacation>> GetRequestsEmployee(Guid employeeId)
        {
            var vacations = await EmployeePortalContext.Vacations
               .Where(v => v.EmployeeId == employeeId)
               .Where(v => (v.EmployeeStatus == VacationStatus.CancellationPending && v.Status == VacationStatus.CancellationPending))
               .ToListAsync();
            return vacations;
        }
    }
}
