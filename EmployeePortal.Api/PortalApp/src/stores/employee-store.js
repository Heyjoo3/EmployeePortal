import employeeService from '@/services/employee-service'
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import router from '@/router'

export const useEmployeeStore = defineStore(
  'employeeAuthentication',
  () => {
    return {
      state,
      actions,
      getEmployees,
      getEmployeesExcludingId,
      currentEmployee,
      currentId,
      currentPage,
      allRoles,
      allDepartments,
      allSupervisors,
      allSubstitutes,
      availableSubstitutes,
      pickedDepartment,
      pickedEmployeeId,
      pickedRelation
    }
  },
  { persist: true }
)

const state = ref({
  employees: [],
  selectedEmployee: null,
  currentEmployeesCount: 0,
  currentGuid: null,
  currentView: null,
  roles: [],
  loggedIn: false,
  departments: [
    'Administration',
    'Ausbildung',
    'Geschäftsführung',
    'Inside Sales',
    'Marketing',
    'PreSales-Consulting',
    'Product Management',
    'Sales',
    'Suite',
    'Suite +'
  ],
  supervisors: [],
  substitutes: [],
  supervisedEmployees: [],
  substitutedEmployees: [],
  pickedDepartment: null,
  pickedEmployeeId: null,
  pickedRelation: null
})

// getters
const getEmployees = computed(() => state.value.employees)
const getEmployeesExcludingId = (id) =>
  computed(() => state.value.employees.filter((employee) => employee.id !== id))
const currentEmployee = computed(() => state.value.selectedEmployee)
const currentId = computed(() => state.value.currentGuid)
const currentPage = computed(() => state.value.currentView)
const allRoles = computed(() => state.value.roles)
const allDepartments = computed(() => state.value.departments)
const allSupervisors = computed(() =>
  state.value.supervisors.filter((supervisor) => supervisor.id !== state.value.currentGuid)
)
const allSubstitutes = computed(() => state.value.substitutes)
const availableSubstitutes = computed(() =>
  state.value.substitutes.filter((substitute) => substitute.id !== state.value.currentGuid)
)
const pickedDepartment = computed(() => state.value.pickedDepartment)
const pickedEmployeeId = computed(() => state.value.pickedEmployeeId)
const pickedRelation = computed(() => state.value.pickedRelation)

//actions
const actions = {
  getEmployeesReduced: async (id) => {
    const response = await employeeService.getAllEmployeesReduced()
    state.value.employees = response
    employeeService.renewJwtToken(currentEmployee)
    if (id == null) {
      return response
    } else {
      const employees = response
      return employees.filter((employee) => employee.id !== id)
    }
  },

  getSupervisors: () => {
    const employees = state.value.employees
    const supervisors = employees.filter((employee) => employee.role === 'Supervisor')
    state.value.supervisors = supervisors
    return supervisors
  },

  getSubstitutes: () => {
    const substitutes = state.value.employees
    state.value.substitutes = substitutes
    return substitutes
  },

  getNameById: (id) => {
    const employees = state.value.employees
    const employee = employees.find((e) => e.id === id)
    if (employee == null) {
      return '-'
    }
    return employee.name
  },

  create: async (employee) => {
    const response = await employeeService.createEmployee(employee)
    employeeService.renewJwtToken(currentEmployee)
    return response
  },

  loginEmployee: async (data) => {
    const response = await employeeService.Login(data)
    state.value.currentGuid = response.data.data.id
    state.value.loggedIn = true
    return response
  },

  logout: () => {
    localStorage.removeItem('jwtToken')
    localStorage.removeItem('role')
    localStorage.removeItem('vacations')
    localStorage.removeItem('absences')
    actions.resetState()
    state.value.loggedIn = false
    if (state.value.currentGuid == null && localStorage.getItem('jwtToken') == null) {
      router.push({ name: 'Login' })
    }
  },

  updateCredentials: async (data) => {
    const response = await employeeService.updateCredentials(data)

    if (response.data.isSuccessfull) {
      localStorage.setItem('jwtToken', response.data.token)
    }

    return response.data
  },

  resetState: () => {
    state.value.employees = []
    // Cannot reset selectedEmployee. It will cause an error in the view
    state.value.selectedEmployee = null
    state.value.currentEmployeesCount = 0
    state.value.currentGuid = null
    state.value.currentView = null
    state.value.roles = []
    state.value.loggedIn = false
    state.value.departments = [
      'Administration',
      'Ausbildung',
      'Geschäftsführung',
      'Inside Sales',
      'Marketing',
      'PreSales-Consulting',
      'Product Management',
      'Sales',
      'Suite',
      'Suite +'
    ]
    state.value.supervisors = []
    state.value.substitutes = []
    state.value.supervisedEmployees = []
    state.value.substitutedEmployees = []
    state.value.pickedDepartment = null
    state.value.pickedEmployeeId = null
    state.value.pickedRelation = null
  },

  getLoggedEmployee: async () => {
    const Guid = state.value.currentGuid
    const response = await employeeService.getEmployeeById(Guid)
    state.value.selectedEmployee = response.data.data
    employeeService.renewJwtToken(currentEmployee)
    return response.data
  },

  getEmployeeById: async (Guid) => {
    const response = await employeeService.getEmployeeById(Guid)
    if (state.value.currentGuid === Guid) {
      state.value.selectedEmployee = response.data.data
    }
    employeeService.renewJwtToken(currentEmployee)
    return response.data.data
  },

  getRoles: async () => {
    await employeeService.getEmployeeRoles().then((response) => {
      state.value.roles = response.data
      employeeService.renewJwtToken(currentEmployee)
      return response.data
    })
  },

  setPickedDepartment: (department) => {
    state.value.pickedDepartment = department
  },

  setPickedEmployeeId: (employee) => {
    state.value.pickedEmployeeId = employee
  },

  setPickedRelation: (relation) => {
    state.value.pickedRelation = relation
  },

  updateEmployee: async (employee, forcePasswordUpdate) => {
    const response = await employeeService.updateEmployee(employee, forcePasswordUpdate)
    state.value.employees = await employeeService.getAllEmployeesReduced()
    const currentEmployee = await actions.getLoggedEmployee()
    employeeService.renewJwtToken(currentEmployee)
    return response
  }
}
