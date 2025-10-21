export function sortHolidays(year, holidaysList) {
  const mapFromHolidays = new Map(holidaysList.map((x) => [x.id, x]))
  const sortedHolidays = [...mapFromHolidays.values()].filter(
    (x) => new Date(Date.parse(x.date)).getFullYear() === year
  )
  sortedHolidays.sort((a, b) => Date.parse(a.date) - Date.parse(b.date))
  return sortedHolidays
}
