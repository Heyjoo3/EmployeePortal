import api from './api'
import _ from 'lodash'

export default {
  createVacation: async (vacation) => {
    const formData = new FormData()
    const copyvacationData = _.cloneDeep(vacation)
    const data = copyvacationData._value
    formData.append('vacationFormData', JSON.stringify(data))
    try {
      const response = await api.post('/Vacation/CreateVacation', formData)
      return response.data
    } catch (error) {
      console.error('Error creating vacation:', error)
      throw error
    }
  },

  adminCreateVacation: async (vacation) => {
    const formData = new FormData()
    const copyvacationData = _.cloneDeep(vacation)
    const data = copyvacationData._value
    formData.append('vacationFormData', JSON.stringify(data))
    try {
      const response = await api.post('/Vacation/AdminCreateVacation', formData)
      return response.data
    } catch (error) {
      console.error('Error creating vacation:', error)
      throw error
    }
  },

  // getAllVacations: async () => {
  //   const response = await api.get('/Vacation/GetAllVacations')
  //   return response.data
  // },

  // getVacationsWithEmployee: async () => {
  //   const response = await api.get('/Vacation/GetVacationsWithEmployee')
  //   return response
  // },

  getVacationsByEmployeeId: async (id) => {
    const formData = new FormData()
    const copyVacationData = _.cloneDeep(id)
    const data = copyVacationData
    formData.append('idData', JSON.stringify(data))
    try {
      const response = await api.post('/Vacation/GetVacationsByEmployeeId', formData)
      return response.data
    } catch (error) {
      console.error('Error getting vacation:', error)
      throw error
    }
  },

  getAnnualVacationsByEmployeeId: async (id) => {
    const formData = new FormData()
    const copyVacationData = _.cloneDeep(id)
    const data = copyVacationData
    formData.append('idData', JSON.stringify(data))
    try {
      const response = await api.post('/Vacation/GetAnnualVacationsByEmployeeId', formData)
      return response.data
    } catch (error) {
      console.error('Error getting vacation:', error)
      throw error
    }
  },

  getOpenRequestWithEmployee: async (requestInput) => {
    const formData = new FormData()
    const copyVacationData = _.cloneDeep(requestInput)
    const data = copyVacationData
    formData.append('requestForm', JSON.stringify(data))
    try {
      const response = await api.post('/Vacation/GetOpenRequestWithEmployee', formData)
      return response.data
    } catch (error) {
      console.error('Error getting open requests:', error)
      throw error
    }
  },

  updateVacation: async (vacation) => {
    const formData = new FormData()
    const copyVacationData = _.cloneDeep(vacation)
    const data = copyVacationData
    formData.append('vacationFormData', JSON.stringify(data))
    try {
      const response = await api.post('/Vacation/UpdateVacation', formData)
      return response.data
    } catch (error) {
      console.error('Error updating vacation:', error)
      throw error
    }
  },

  cancelVacation: async (vacation) => {
    const formData = new FormData()
    const copyVacationData = _.cloneDeep(vacation)
    const data = copyVacationData
    formData.append('vacationFormData', JSON.stringify(data))
    try {
      const response = await api.post('/Vacation/CancelVacation', formData)
      return response.data
    } catch (error) {
      console.error('Error canceling vacation:', error)
      throw error
    }
  },

  acceptVacation: async (vacation) => {
    const formData = new FormData()
    const copyVacationData = _.cloneDeep(vacation)
    const data = copyVacationData
    formData.append('vacationFormData', JSON.stringify(data))
    try {
      const response = await api.post('/Vacation/AcceptVacation', formData)
      return response.data
    } catch (error) {
      console.error('Error accepting vacation:', error)
      throw error
    }
  },

  createPublicHoliday: async (holiday) => {
    const formData = new FormData()
    const copyHolidayData = _.cloneDeep(holiday)
    formData.append('holidayFormData', JSON.stringify(copyHolidayData))
    try {
      const response = await api.post('/Vacation/CreatePublicHoliday', formData)
      return response.data
    } catch (error) {
      console.error('Error creating Holiday:', error)
      throw error
    }
  },

  getPublicHolidaysByYear: async (year) => {
    const formData = new FormData()
    formData.append('selectedYear', JSON.stringify(year))
    try {
      const response = await api.post('/Vacation/GetAllPublicHolidays/', formData)
      return response.data
    } catch (error) {
      console.error('Error creating Holiday:', error)
      throw error
    }
  },

  getAllPublicHolidayYears: async () => {
    try {
      const response = await api.get('/Vacation/GetAllYears')
      return response.data
    } catch (error) {
      console.error('Error creating Holiday:', error)
      throw error
    }
  },

  deletePublicHoliday: async (holiday) => {
    const formData = new FormData()
    const copyHolidayData = _.cloneDeep(holiday)
    const data = copyHolidayData
    formData.append('holidayId', JSON.stringify(data))
    try {
      const response = await api.post('/Vacation/DeleteHoliday', formData)
      return response.data
    } catch (error) {
      console.error('Error deleting Holiday:', error)
      throw error
    }
  },

  deleteVacation: async (vacation) => {
    const formData = new FormData()
    const copyVacationData = _.cloneDeep(vacation)
    const data = copyVacationData
    formData.append('vacationId', JSON.stringify(data))
    try {
      const response = await api.post('/Vacation/DeleteVacation', formData)
      return response
    } catch (error) {
      console.error('Error deleting Vacation:', error)
      throw error
    }
  },

  // getEmployeeRoles: async () => {
  //   const response = await api.post('/Employee/GetEmployeeRoles')
  //   return response
  // },

  closeOpenVacationRequests: async (employeeId) => {
    const formData = new FormData()
    const copyEmployeeId = _.cloneDeep(employeeId)
    const data = copyEmployeeId
    formData.append('employeeIdData', JSON.stringify(data))

    try {
      const response = await api.post('/Vacation/CloseOpenVacationRequests', formData)
      return response
    } catch (error) {
      console.error('Error closing open vacation requests:', error)
      throw error
    }
  },

  checkCompanyVacationOverlaps: async (vacation) => {
    const formData = new FormData()
    const copyVacationData = _.cloneDeep(vacation)
    const data = copyVacationData
    formData.append('vacationFormData', JSON.stringify(data))
    try {
      const response = await api.post('/Vacation/CheckCompanyVacationOverlaps', formData)
      return response.data
    } catch (error) {
      console.error('Error checking company vacation overlaps:', error)
      throw error
    }
  },

  updateVacationsAfterPublicHolidayDeletion: async (holidayDate) => {
    const formData = new FormData()
    const copyHolidayDate = _.cloneDeep(holidayDate)
    const data = copyHolidayDate
    formData.append('holidayDate', JSON.stringify(data))
    try {
      const response = await api.post(
        '/Vacation/UpdateVacationsAfterPublicHolidayDeletion',
        formData
      )
      return response
    } catch (error) {
      console.error('Error updating vacations after public holiday deletion:', error)
      throw error
    }
  },

  checkForAbsenceOverlaps: async (absence) => {
    const formData = new FormData()
    const copyAbsenceData = _.cloneDeep(absence)
    const data = copyAbsenceData
    formData.append('absenceData', JSON.stringify(data))
    try {
      const response = await api.post('/Vacation/CheckForAbsenceOverlaps', formData)
      return response.data
    } catch (error) {
      console.error('Error checking absence overlaps:', error)
      throw error
    }
  },

  getCompanyVacations: async () => {
    const response = await api.post('/Vacation/GetCompanyVacations')
    return response.data.data
  }
}
