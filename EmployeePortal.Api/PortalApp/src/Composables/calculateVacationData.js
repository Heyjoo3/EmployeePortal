import { useVacationStore } from '../stores/vacation-store.js'
import { useEmployeeStore } from '../stores/employee-store.js'
import { useDateConverter } from './useDateConverter.js'

// calculates vacation days for a given employee. returns an object with all the stats
export async function calculateVacationData(employeeId) {
  const employeeStore = useEmployeeStore()
  const vacationStore = useVacationStore()

  const annualVacationDays = await employeeStore.actions
    .getEmployeeById(employeeId)
    .then((employee) => employee.annualVacation)

  const companyVacation = await vacationStore.actions.getCompanyVacations()
  const vacations = await vacationStore.actions.getAnnualVacationsByEmployeeIdDB(employeeId)
  var vacationsWithCompanyVacation = vacations.concat(companyVacation)
  vacationsWithCompanyVacation = vacationsWithCompanyVacation.filter((vacation) => {
    const startDate = new Date(vacation.startDate)
    const endDate = new Date(vacation.endDate)
    const currentYear = new Date().getFullYear()
    return startDate.getFullYear() === currentYear && endDate.getFullYear() === currentYear
  })

  // calculate the vacations before the current day
  const approvedVacationDaysBeforeCurrentDay = await vacationsWithCompanyVacation.reduce(
    async (accPromise, vacation) => {
      const totalDays = await accPromise
      const today = new Date()
      const startDate = new Date(vacation.startDate)
      const endDate = new Date(vacation.endDate)

      if (vacation.status === 1 || vacation.status === 5) {
        let vacationDaysToCount = vacation.vacationdays
        if (endDate < today || (startDate <= today && endDate >= today)) {
          return totalDays + vacationDaysToCount
        }
      }
      return totalDays
    },
    Promise.resolve(0)
  )

  // calculate the vacations after the current day
  const vacationsAfterCurrentDay = await vacationsWithCompanyVacation.reduce(
    async (accPromise, vacation) => {
      const totalDays = await accPromise
      const today = useDateConverter().convertToGermanISO(new Date().setHours(0, 0, 0, 0))
      const startDate = useDateConverter().convertToGermanISO(vacation.startDate)

      // check if the vacation is not cancelled
      if (vacation.status !== 4) {
        if (
          startDate > today &&
          (vacation.status === 1 ||
            vacation.status === 5 ||
            vacation.status === 6 ||
            vacation.status === 3)
        ) {
          return totalDays + vacation.vacationdays
        }
      }
      return totalDays
    },
    Promise.resolve(0)
  )

  const remainingVacationDays =
    annualVacationDays - (approvedVacationDaysBeforeCurrentDay + vacationsAfterCurrentDay)

  // create an object with all the stats
  const counts = {
    AnnualVacation: annualVacationDays,
    ApprovedVacationsBeforeCurrentDay: approvedVacationDaysBeforeCurrentDay,
    VacationsAfterCurrentDay: vacationsAfterCurrentDay,
    RemainingVacationDays: remainingVacationDays,
    NotRelevant: 0,
    VacationApproved: 0,
    VacationRejected: 0,
    VacationPending: 0,
    CancellationApproved: 0,
    CancellationRejected: 0,
    CancellationPending: 0
  }

  // count the vacations by status
  vacations.forEach((vacation) => {
    switch (vacation.status) {
      case 0:
        counts.NotRelevant++
        break
      case 1:
        counts.VacationApproved++
        break
      case 2:
        counts.VacationRejected++
        break
      case 3:
        counts.VacationPending++
        break
      case 4:
        counts.CancellationApproved++
        break
      case 5:
        counts.CancellationRejected++
        break
      case 6:
        counts.CancellationPending++
        break
      default:
        break
    }
  })

  return counts
}
