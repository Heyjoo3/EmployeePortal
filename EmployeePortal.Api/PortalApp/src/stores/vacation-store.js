import vacationService from '@/services/vacation-service'
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { useEmployeeStore } from './employee-store'
import employeeService from '@/services/employee-service'

export const useVacationStore = defineStore(
  'vacations',
  () => {
    return {
      state,
      vacationDayData,
      allHolidayYears,
      allPublicHolidays,
      allVacations,
      allCurrentEmployeeVacations,
      allVacationsBySupervised,
      allVacationsBySubstituted,
      vacationTypes,
      actions
    }
  },
  { persist: true }
)

const employeeStore = useEmployeeStore()
const currentEmployee = employeeStore.currentEmployee

const state = ref({
  vacations: [],
  companyVacations: [],
  currentEmployeeVacationData: [],
  hoildayYears: null,
  holidayObjects: [],
  vacationsBySupervised: [],
  vacationsBySubstituted: [],
  vacationTypes: ['Jahresurlaub', 'Sonderurlaub', 'Betriebsurlaub']
})

const vacationDayData = computed(() => state.value.currentEmployeeVacationData)
const allVacations = computed(() => state.value.vacations)
const allCurrentEmployeeVacations = computed(() => state.value.currentEmployeeVacationData)
const allHolidayYears = computed(() => state.value.hoildayYears)
const allPublicHolidays = computed(() => state.value.holidayObjects)
const allVacationsBySupervised = computed(() => state.value.vacationsBySupervised)
const allVacationsBySubstituted = computed(() => state.value.vacationsBySubstituted)
const vacationTypes = computed(() => state.value.vacationTypes)

const actions = {
  checkExistingVacation: (vacation) => {
    const vacations = state.value.currentEmployeeVacationData.filter(
      (vacation) => vacation.status !== 2 && vacation.status !== 4
    )
    const daysInVacation = []
    const vacationEnd = new Date(vacation.endDate)
    const vacationDay = new Date(vacation.startDate)

    employeeService.renewJwtToken(currentEmployee)

    while (vacationDay <= vacationEnd) {
      daysInVacation.push(new Date(vacationDay))
      vacationDay.setDate(vacationDay.getDate() + 1)
    }
    let existingVacation = false
    for (const v of vacations) {
      if (v.type !== 2 || v.type !== 4) {
        const vEnd = new Date(v.endDate)
        const vDay = new Date(v.startDate)
        while (vDay <= vEnd) {
          for (const day of daysInVacation) {
            if (day.toDateString() === vDay.toDateString()) {
              existingVacation = true
              return existingVacation
            }
          }
          vDay.setDate(vDay.getDate() + 1)
        }
      }
    }
    return existingVacation
  },

  checkExistingCompanyVacation: (vacation) => {
    const vacations = state.value.companyVacations
    const daysInVacation = []
    const vacationEnd = new Date(vacation.endDate)
    const vacationDay = new Date(vacation.startDate)

    employeeService.renewJwtToken(currentEmployee)

    while (vacationDay <= vacationEnd) {
      daysInVacation.push(new Date(vacationDay))
      vacationDay.setDate(vacationDay.getDate() + 1)
    }

    let existingVacation = false
    for (const v of vacations) {
      const vEnd = new Date(v.endDate)
      const vDay = new Date(v.startDate)
      while (vDay <= vEnd) {
        for (const day of daysInVacation) {
          if (day.toDateString() === vDay.toDateString()) {
            existingVacation = true
            return existingVacation
          }
        }
        vDay.setDate(vDay.getDate() + 1)
      }
    }
    return existingVacation
  },

  checkForAbsenceOverlaps: async (absence) => {
    const response = await vacationService.checkForAbsenceOverlaps(absence)
    employeeService.renewJwtToken(currentEmployee)
    if (response.isSuccessfull && response.data.length > 0) {
      return response.data
    }
    return []
  },

  createVacation: async (vacation) => {
    const response = await vacationService.createVacation(vacation)
    employeeService.renewJwtToken(currentEmployee)
    return response
  },

  adminCreateVacation: async (vacation) => {
    const response = await vacationService.adminCreateVacation(vacation)
    employeeService.renewJwtToken(currentEmployee)
    return response
  },

  getVacationsByEmployeeIdDB: async (employeeId) => {
    const response = await vacationService.getVacationsByEmployeeId(employeeId)
    state.value.currentEmployeeVacationData = response.data
    employeeService.renewJwtToken(currentEmployee)
    return response.data
  },

  getAnnualVacationsByEmployeeIdDB: async (employeeId) => {
    const response = await vacationService.getAnnualVacationsByEmployeeId(employeeId)
    state.value.currentEmployeeVacationData = response.data
    employeeService.renewJwtToken(currentEmployee)
    return response.data
  },

  getOpenRequestWithEmployee: async (id, role) => {
    const requestInput = { employeeId: id, role: role }
    const response = await vacationService.getOpenRequestWithEmployee(requestInput)
    employeeService.renewJwtToken(currentEmployee)
    return response
  },

  editVacation: async (vacation) => {
    const response = await vacationService.updateVacation(vacation)
    employeeService.renewJwtToken(currentEmployee)
    return response
  },

  createPublicHoliday: async (holiday) => {
    await vacationService.createPublicHoliday(holiday).then(async (response) => {
      const newHolidayDate = new Date(holiday.date)
      const weekday = newHolidayDate.getDay()

      if (weekday >= 1 && weekday <= 5) {
        state.value.vacations.forEach((vacation) => {
          const vacationStart = new Date(vacation.startDate)
          const vacationEnd = new Date(vacation.endDate)

          if (newHolidayDate >= vacationStart && newHolidayDate <= vacationEnd) {
            actions.editVacation(vacation)
          }
        })
      }
      employeeService.renewJwtToken(currentEmployee)
      return response
    })
  },

  getPublicHolidays: async (years) => {
    state.value.holidayObjects = []
    employeeService.renewJwtToken(currentEmployee)
    for (const year in years.value) {
      await vacationService.getPublicHolidaysByYear(years.value[year]).then((response) => {
        state.value.holidayObjects.push(...response)
        return response
      })
    }
  },

  getAllPublicHolidayYears: async () => {
    await vacationService.getAllPublicHolidayYears().then((response) => {
      state.value.hoildayYears = response
      employeeService.renewJwtToken(currentEmployee)
      return response
    })
  },

  cancelVacation: async (vacationId) => {
    await vacationService.cancelVacation(vacationId).then((response) => {
      employeeService.renewJwtToken(currentEmployee)
      return response
    })
  },

  acceptVacation: async (vacationId) => {
    await vacationService.acceptVacation(vacationId).then((response) => {
      employeeService.renewJwtToken(currentEmployee)
      return response
    })
  },

  deleteVacation: async (vacationId) => {
    const response = await vacationService.deleteVacation(vacationId)
    employeeService.renewJwtToken(currentEmployee)
    return response
  },

  deletePublicHoliday: async (holidayId) => {
    await vacationService.deletePublicHoliday(holidayId).then((response) => {
      employeeService.renewJwtToken(currentEmployee)
      return response
    })
  },

  getCompanyVacations: async () => {
    state.value.companyVacations = await vacationService.getCompanyVacations()
    employeeService.renewJwtToken(currentEmployee)
    return state.value.companyVacations
  },

  deleteCompanyVacation: async (vacationId) => {
    await vacationService.deleteVacation(vacationId).then((response) => {
      employeeService.renewJwtToken(currentEmployee)
      return response
    })
  },

  checkCompanyVacationOverlaps: async (vacation) => {
    await vacationService.checkCompanyVacationOverlaps(vacation).then((response) => {
      employeeService.renewJwtToken(currentEmployee)
      return response
    })
  },

  closeOpenVacationRequests: async (employeeId) => {
    await vacationService.closeOpenVacationRequests(employeeId).then((response) => {
      employeeService.renewJwtToken(currentEmployee)
      return response
    })
  },

  updateVacationsAfterPublicHolidayDeletion: async (holidayDate) => {
    await vacationService
      .updateVacationsAfterPublicHolidayDeletion(holidayDate)
      .then((response) => {
        employeeService.renewJwtToken(currentEmployee)
        return response
      })
  }
}
