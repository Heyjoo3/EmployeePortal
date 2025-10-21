using EmployeePortal.Core.Data;
using EmployeePortal.Core.Models;
using EmployeePortal.Core.Repositories;
using EmployeePortal.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePortal.Data
{
    public class AbsenceRepository : Repository<Absence>, IAbsenceRepository
    {

        public AbsenceRepository(EmployeePortalContext context) : base(context) { }

        private EmployeePortalContext EmployeePortalContext
        {
            get { return _context as EmployeePortalContext; }
        }

        public async Task<IEnumerable<Absence>> GetAllAbsences()
        {
            List<Absence> absences = await EmployeePortalContext.Absences

                .Select(a => new Absence
                {
                    AbsenceId = a.AbsenceId,
                    AbsenceType = a.AbsenceType,
                    StartDate = a.StartDate,
                    EndDate = a.EndDate,
                    Duration = a.Duration,
                    Remarks = a.Remarks,
                    HasSickLeave = a.HasSickLeave,
                    HasStartedShift = a.HasStartedShift,
                    EmployeeId = a.EmployeeId
                })
                .ToListAsync();
            return absences;
        }

        public new async Task<Absence> GetById(Guid absenceId)
        {
            Absence? absence = await EmployeePortalContext.Absences.Where(x => x.AbsenceId == absenceId).FirstOrDefaultAsync();
            return absence;
        }

        public async Task<IEnumerable<Absence>> GetAbsencesByEmployeeId(Guid EmployeeId)
        {
            var id = EmployeeId.ToString();
            List<Absence> absences = await EmployeePortalContext.Absences.Where(x => x.EmployeeId.ToString() == id).ToListAsync();
            return absences;
        }

        public async new Task Add (Absence absence)
        {
            await EmployeePortalContext.Absences.AddAsync(absence);
        }

        public async Task<IEnumerable<Absence>> UpdateAbsence(Absence absence)
        {
            EmployeePortalContext.Absences.Update(absence);
            await EmployeePortalContext.SaveChangesAsync();
            return await EmployeePortalContext.Absences.ToListAsync();
        }

        public async Task<IEnumerable<Absence>> GetUnexcusedSickLeaves(Guid EmployeeId)
        {
            var id = EmployeeId.ToString();
            var absences = await EmployeePortalContext.Absences.Where(x => 
            x.EmployeeId.ToString() == id && 
            x.HasSickLeave != true && 
            x.AbsenceType == AbsenceType.SickLeave
            ).ToListAsync();
            return absences;
        }

        public async Task<IEnumerable<Absence>> GetSickDaysByEmployeeAndDateRange(String EmployeeId, DateTime startDate, DateTime endDate, int? deadline)
        {
            var absences = await EmployeePortalContext.Absences
                .Where(x => x.EmployeeId.ToString() == EmployeeId && x.AbsenceType == AbsenceType.SickLeave && (x.HasSickLeave == true || x.Duration < deadline))
                .Where(x =>
                  (x.StartDate >= startDate && x.EndDate <= endDate) ||  // v is during company vacation
                  (x.StartDate <= startDate && x.EndDate >= endDate) ||
                  (x.StartDate <= startDate && x.EndDate >= startDate && x.EndDate <= endDate) ||
                  (x.StartDate >= startDate && x.StartDate <= endDate && x.EndDate >= endDate))
                .ToListAsync();
            return absences;
        }
    }
}
