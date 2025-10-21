import api from './api'
import _ from 'lodash'
import { authHeader } from '@/Composables/authorizationHeader'

export default {
  createEmployee: async (employee) => {
    const formData = new FormData()
    const copyEmployee = _.cloneDeep(employee)
    const data = copyEmployee._value
    formData.append('employeeFormData', JSON.stringify(data))

    try {
      const response = await api.post('/Employee/CreateEmployee', formData, {
        headers: authHeader()
      })
      return response.data
    } catch (error) {
      console.error('Error creating employee:', error)
      throw error
    }
  },

  Login: async (data) => {
    const formData = new FormData()
    const Ldata = data._value
    formData.append('loginData', JSON.stringify(Ldata))

    try {
      const response = await api.post('/Employee/Login', formData)
      return response
    } catch (error) {
      console.error('Error logging in:', error)
      throw error
    }
  },

  updateCredentials: async (data) => {
    const formData = new FormData()
    const copy = _.cloneDeep(data)
    const credentialsData = copy
    formData.append('credentialsForm', JSON.stringify(credentialsData))
    try {
      const response = await api.post('/Employee/UpdateCredentials', formData, {
        headers: authHeader()
      })
      return response
    } catch (error) {
      console.error('Error updating credentials:', error)
      throw error
    }
  },

  getAllEmployees: async () => {
    const formData = new FormData()
    const response = await api.post('/Employee/GetAllEmployees', formData, {
      headers: authHeader()
    })
    return response
  },

  getAllEmployeesReduced: async () => {
    const formData = new FormData()
    const response = await api.post('/Employee/GetAllEmployeesReduced', formData, {
      headers: authHeader()
    })
    return response.data.data
  },

  getEmployeeById: async (id) => {
    try {
      const formData = new FormData()
      formData.append('employeeID', JSON.stringify(id))
      const response = await api.post('/Employee/GetEmployeeById', formData, {
        headers: authHeader()
      })
      return response
    } catch (error) {
      console.error('Error logging in:', error)
      throw error
    }
  },

  getEmployeeRoles: async () => {
    const response = await api.get('/Employee/GetEmployeeRoles', {
      headers: authHeader()
    })
    return response
  },

  updateEmployee: async (employee, forcePasswordUpdate) => {
    const formData = new FormData()
    const copyEmployee = _.cloneDeep(employee)
    const data = copyEmployee._value
    formData.append('employeeFormData', JSON.stringify(data))
    if (forcePasswordUpdate == true) {
      formData.append('forcePasswordUpdate', JSON.stringify(forcePasswordUpdate))
    }
    try {
      const response = await api.post('/Employee/UpdateEmployee', formData, {
        headers: authHeader()
      })
      return response.data
    } catch (error) {
      console.error('Error updating employee:', error)
      throw error
    }
  },

  renewJwtToken: async (username) => {
    const token = localStorage.getItem('jwtToken')
    const formData = new FormData()
    formData.append('Username', username)
    formData.append('Token', token)
    const response = await api.post('/Employee/RenewJwtToken', formData, {
      headers: authHeader()
    })
    localStorage.setItem('jwtToken', response.data.token)
    return response
  }
}
