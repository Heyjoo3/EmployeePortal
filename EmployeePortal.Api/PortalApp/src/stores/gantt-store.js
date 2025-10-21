import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { calculateVacationData } from '@/Composables/calculateVacationData'
import { useEmployeeStore } from './employee-store'
import { useSideNavStore } from './side-nav-store'
import { useVacationStore } from './vacation-store'
import employeeService from '@/services/employee-service'

export const useGanttStore = defineStore(
  'gantt',
  () => {
    return {
      state,
      actions,
      getTeamData,
      getTeamMembersNeedingNotification
    }
  },
  { persist: true }
)

const teamMemberSet = new Set()

const state = ref({
  teamData: [],
  teamMembersNeedingNotification: teamMemberSet
})

const getTeamData = computed(() => state.value.teamData)
const getTeamMembersNeedingNotification = computed(() => state.value.teamMembersNeedingNotification)

const employeeStore = useEmployeeStore()
const sideNavStore = useSideNavStore()
const vacactionStore = useVacationStore()

const currentEmployee = employeeStore.currentEmployee

const actions = {
  getVacationClass: (vacation) => {
    const relation = employeeStore.state.pickedRelation
    employeeService.renewJwtToken(currentEmployee)
    if (!vacation) return ''

    const {
      type,
      status: vacationStatus,
      adminStatus,
      substituteStatus,
      supervisorStatus
    } = vacation

    if (type === 'Betriebsurlaub') return 'company-vacation'

    const needsApprovalMapping = {
      admin: adminStatus,
      supervisor: supervisorStatus,
      substitute: substituteStatus
    }

    if (
      relation == 'admin' &&
      vacationStatus === 1 &&
      adminStatus === 3 &&
      supervisorStatus === 1
    ) {
      teamMemberSet.add(vacation.employeeId)
      return 'needs-approval-vacation'
    }

    if (
      relation == 'admin' &&
      vacationStatus === 3 &&
      vacation.substitute === employeeStore.state.currentGuid &&
      substituteStatus === 3
    ) {
      teamMemberSet.add(vacation.employeeId)
      return 'needs-approval-vacation'
    }

    if (relation == 'admin' && vacationStatus === 4 && adminStatus === 6) {
      teamMemberSet.add(vacation.employeeId)
      return 'needs-approval-cancellation'
    }

    if (relation == 'supervisor' && vacationStatus === 3 && substituteStatus === 3) {
      return 'vacation-pending'
    }

    if (
      relation &&
      needsApprovalMapping[relation] === 3 &&
      vacationStatus === 3 &&
      relation == 'supervisor' &&
      vacation.supervisor === employeeStore.state.currentGuid &&
      substituteStatus === 1
    ) {
      teamMemberSet.add(vacation.employeeId)
      return 'needs-approval-vacation'
    }

    if (
      relation &&
      needsApprovalMapping[relation] === 3 &&
      vacationStatus === 3 &&
      relation == 'substitute' &&
      vacation.substitute === employeeStore.state.currentGuid
    ) {
      teamMemberSet.add(vacation.employeeId)
      return 'needs-approval-vacation'
    }

    if (
      relation &&
      needsApprovalMapping[relation] === 6 &&
      vacationStatus === 6 &&
      relation != 'admin' &&
      (vacation.substitute === employeeStore.state.currentGuid ||
        vacation.supervisor === employeeStore.state.currentGuid)
    ) {
      teamMemberSet.add(vacation.employeeId)
      return 'needs-approval-cancellation'
    }

    const statusMapping = {
      1: 'vacation-approved',
      2: 'vacation-cancelled',
      3: 'vacation-pending',
      4: 'cancellation-approved',
      5: 'cancellation-cancelled',
      6: 'cancellation-pending'
    }

    return statusMapping[vacationStatus] || ''
  },

  getCompleteTeamData: async () => {
    employeeService.renewJwtToken(currentEmployee)
    if (employeeStore.pickedRelation == 'supervisor') {
      state.value.teamData = sideNavStore.supervised
    } else if (employeeStore.pickedRelation == 'substitute') {
      state.value.teamData = sideNavStore.substituted
    } else if (employeeStore.pickedRelation == 'admin') {
      const allTeams = sideNavStore.employeesByDepartment
      const departmentTeam = allTeams.find(
        (team) => team.department == employeeStore.pickedDepartment
      )
      state.value.teamData = departmentTeam ? departmentTeam.employees : []
    } else {
      return
    }

    for (const employee of state.value.teamData) {
      const vacations = await vacactionStore.actions.getVacationsByEmployeeIdDB(employee.id)
      employee.vacations = vacations

      const vacationStats = await calculateVacationData(employee.id)
      employee.vacationStats = vacationStats

      for (const vacation of vacations) {
        const vacationClass = actions.getVacationClass(vacation)
        vacation.vacationClass = vacationClass
      }

      if (teamMemberSet.has(employee.id)) {
        employee.needsNotification = true
      } else {
        employee.needsNotification = false
      }

      const companyVacations = await vacactionStore.actions.getCompanyVacations()
      employee.vacations.push(
        ...companyVacations.map((companyVacation) => ({
          ...companyVacation,
          vacationClass: 'company-vacation'
        }))
      )
    }

    return state.value.teamData
  }
}
