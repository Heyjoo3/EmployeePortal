export function sortAbsencesByDate() {
  function sortAbsencesByStartDate(absences) {
    absences.sort((a, b) => new Date(a.startDate) - new Date(b.startDate))
    return absences
  }

  function sortAbsencesByEndDate(absences) {
    absences.sort((a, b) => new Date(a.endDate) - new Date(b.endDate))
    return absences
  }

  return {
    sortAbsencesByStartDate,
    sortAbsencesByEndDate
  }
}
