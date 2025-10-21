import sideNavService from '@/services/side-nav-service'
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { useEmployeeStore } from './employee-store'
import employeeService from '@/services/employee-service'

export const useSideNavStore = defineStore(
  'sideNav',
  () => {
    return {
      state,
      actions,
      employeesByDepartment,
      supervisedAndSubstitutedEmployees,
      supervised,
      substituted,
      teams
    }
  },
  { persist: true }
)

const employeeStore = useEmployeeStore()
const currentEmployee = employeeStore.currentEmployee

const state = ref({
  employeesByDepartment: [],
  supervisedAndSubstitutedEmployees: [],
  supervisors: [],
  substitutes: [],
  teams: []
})

const employeesByDepartment = computed(() => state.value.employeesByDepartment)
const supervisedAndSubstitutedEmployees = computed(
  () => state.value.supervisedAndSubstitutedEmployees
)
const supervised = computed(() => state.value.supervisors)
const substituted = computed(() => state.value.substitutes)
const teams = computed(() => state.value.teams)

const actions = {
  getEmployeesByDepartment: async () => {
    const response = await sideNavService.GetAllEmployeesSortedByDepartment()
    state.value.employeesByDepartment = response.data.data
    employeeService.renewJwtToken(currentEmployee)
    return response
  },

  getSupervisedAndSubstitutedEmployees: async (id) => {
    const response = await sideNavService.GetSupervisedAndSubstitutedEmployees(id)
    state.value.supervisedAndSubstitutedEmployees = response.data.data
    state.value.supervisors = response.data.data[0].employees
    state.value.substitutes = response.data.data[1].employees
    employeeService.renewJwtToken(currentEmployee)
    return response
  },

  getTeams: async (id) => {
    const response = await sideNavService.getTeams(id)
    state.value.teams = response.data.data
    employeeService.renewJwtToken(currentEmployee)
    return response
  }
}
