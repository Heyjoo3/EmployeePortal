import api from './api'
import _ from 'lodash'
import { authHeader } from '@/Composables/authorizationHeader'

export default {
  createAbsence: async (absence) => {
    const formData = new FormData()
    const copyAbsence = _.cloneDeep(absence)
    const data = copyAbsence
    formData.append('absenceFormData', JSON.stringify(data))
    formData.append('employeeId', JSON.stringify(data.employeeId))

    try {
      const response = await api.post('/Absence/CreateAbsence', formData, {
        headers: authHeader()
      })
      return response
    } catch (error) {
      console.error('Error creating absence:', error)
      throw error
    }
  },

  // getAllAbsences: async () => {
  //   const formData = new FormData()
  //   const response = await api.get('/Absence/GetAllAbsences', formData, {
  //     headers: authHeader()
  //   })
  //   return response
  // },

  getAbsencesByEmployeeId: async (id) => {
    try {
      const formData = new FormData()
      formData.append('employeeId', JSON.stringify(id))
      const response = await api.post('/Absence/GetAbsencesByEmployeeId', formData, {
        headers: authHeader()
      })
      return response.data
    } catch (error) {
      console.error('Error getting abesences:', error)
      throw error
    }
  },

  deleteAbsence: async (id) => {
    try {
      const formData = new FormData()
      formData.append('absenceID', JSON.stringify(id))
      const response = await api.post('/Absence/DeleteAbsence', formData, {
        headers: authHeader()
      })
      return response
    } catch (error) {
      console.error('Error deleting absence:', error)
      throw error
    }
  },

  updateAbsence: async (absence) => {
    const formData = new FormData()
    const copyAbsence = _.cloneDeep(absence)
    const data = copyAbsence
    formData.append('absenceFormData', JSON.stringify(data))

    try {
      const response = await api.post('/Absence/UpdateAbsence', formData, {
        headers: authHeader()
      })
      return response
    } catch (error) {
      console.error('Error updating absence:', error)
      throw error
    }
  },

  checkSickNoteDeadline: async (employeeId) => {
    try {
      const formData = new FormData()
      const copyEmployeeId = _.cloneDeep(employeeId)
      const data = copyEmployeeId
      formData.append('employeeId', JSON.stringify(data))
      const response = await api.post('/Absence/CheckSickLeaveDeadline', formData, {
        headers: authHeader()
      })
      return response
    } catch (error) {
      console.error('Error checking sick note deadline:', error)
      throw error
    }
  }
}
