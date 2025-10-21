export function calculateSickDays(absence) {
  const startDate = new Date(absence.startDate)
  const endDate = new Date(absence.endDate)

  const utcStartDate = Date.UTC(
    startDate.getUTCFullYear(),
    startDate.getUTCMonth(),
    startDate.getUTCDate()
  )
  const utcEndDate = Date.UTC(endDate.getUTCFullYear(), endDate.getUTCMonth(), endDate.getUTCDate())

  const differenceInTime = utcEndDate - utcStartDate
  const differenceInDays = differenceInTime / (1000 * 3600 * 24) + 1

  return differenceInDays
}
