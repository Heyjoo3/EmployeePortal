import absenceService from '@/services/absence-service'
import employeeService from '@/services/employee-service'
import { useEmployeeStore } from '@/stores/employee-store'
import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useAbsenceStore = defineStore(
  'absences',
  () => {
    return {
      state,
      actions
    }
  },
  { persist: true }
)

const state = ref({
  absences: []
})

const employeeStore = useEmployeeStore()
const currentEmployee = employeeStore.currentEmployee

const actions = {
  createAbsence: async (absence) => {
    const response = await absenceService.createAbsence(absence)
    employeeService.renewJwtToken(currentEmployee)
    return response
  },

  getAbsencesByEmployeeIdDb: async (id) => {
    const response = await absenceService.getAbsencesByEmployeeId(id)
    state.value.absences = response.data
    employeeService.renewJwtToken(currentEmployee)
    return response.data
  },

  deleteAbsence: async (id) => {
    const response = await absenceService.deleteAbsence(id)
    employeeService.renewJwtToken(currentEmployee)
    return response.data
  },

  updateAbsence: async (absence) => {
    const response = await absenceService.updateAbsence(absence)
    state.value.absences = response.data.data
    employeeService.renewJwtToken(currentEmployee)
    return response.data
  },

  checkExistingAbsence: async (absence) => {
    let absences = await actions.getAbsencesByEmployeeIdDb(absence.employeeId)
    if (absence.absenceId) {
      absences = absences.filter((a) => a.absenceId != absence.absenceId)
    }
    const absenceDays = []
    const absenceStart = new Date(absence.startDate.replace('+02:00', 'Z'))
    const absenceEnd = new Date(absence.endDate.replace('+02:00', 'Z'))
    while (absenceStart <= absenceEnd) {
      const day = absenceStart.toISOString()
      absenceDays.push(day)
      absenceStart.setDate(absenceStart.getDate() + 1)
    }
    let response = false
    for (let i = 0; i < absences.length; i++) {
      const a = absences[i]
      const aEnd = new Date(a.endDate + 'Z')
      const aStart = new Date(a.startDate + 'Z')
      while (aStart <= aEnd) {
        for (let d = 0; d < absenceDays.length; d++) {
          let dDate = new Date(absenceDays[d])
          const aStartDate = new Date(aStart)
          if (dDate.toDateString() === aStartDate.toDateString()) {
            response = true
            employeeService.renewJwtToken(currentEmployee)
            return response
          }
        }
        aStart.setDate(aStart.getDate() + 1)
      }
    }
    employeeService.renewJwtToken(currentEmployee)
    return response
  },

  checkSickNoteDeadline: async (employeeId) => {
    const response = await absenceService.checkSickNoteDeadline(employeeId)
    employeeService.renewJwtToken(currentEmployee)
    return response.data
  },

  getSickDaysFromCurrentYear: async (employeeId) => {
    const response = await absenceService.getAbsencesByEmployeeId(employeeId)
    const currentYear = new Date().getFullYear()
    const sickDays = response.data
      .filter(
        (absence) =>
          absence.absenceType === 2 && new Date(absence.startDate).getFullYear() === currentYear
      )
      .reduce((sum, absence) => sum + (absence.duration || 0), 0)
    employeeService.renewJwtToken(currentEmployee)
    return sickDays
  }
}
