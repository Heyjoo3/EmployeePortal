export function sortVacationsByDate() {
  function sortVacationsByStartDate(vacations) {
    vacations.sort((a, b) => new Date(a.startDate) - new Date(b.startDate))
    return vacations
  }

  function sortVacationsByEndDate(vacations) {
    vacations.sort((a, b) => new Date(a.endDate) - new Date(b.endDate))
    return vacations
  }

  return {
    sortVacationsByStartDate,
    sortVacationsByEndDate
  }
}
