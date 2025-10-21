import api from './api'
import { authHeader } from '@/Composables/authorizationHeader'

export default {
  GetAllEmployeesSortedByDepartment: async () => {
    try {
      const response = await api.post('/SideNav/EmployeesSortedByDepartment', '', {
        headers: authHeader()
      })
      return response
    } catch (error) {
      console.error('Error getting Admin Sidenav', error)
      throw error
    }
  },

  GetSupervisedAndSubstitutedEmployees: async (id) => {
    try {
      const formData = new FormData()
      formData.append('employeeId', JSON.stringify(id))
      const response = await api.post('/SideNav/SupervisedAndSubsitutedEmployees', formData, {
        headers: authHeader()
      })
      return response
    } catch (error) {
      console.error('Error getting Sidenav:', error)
      throw error
    }
  },

  getTeams: async (id) => {
    try {
      const formData = new FormData()
      formData.append('employeeId', JSON.stringify(id))
      const response = await api.post('/SideNav/getTeams', formData, {
        headers: authHeader()
      })
      return response
    } catch (error) {
      console.error('Error getting Sidenav:', error)
      throw error
    }
  }
}
