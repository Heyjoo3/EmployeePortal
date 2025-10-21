import axios from 'axios'
import { useEmployeeStore } from '../stores/employee-store.js' // Make sure to import your logout service

const logout = () => {
  const { logout } = useEmployeeStore().actions;
  logout();
};

axios.defaults.showLoader = true

const api = axios.create({
  baseURL: import.meta.env.VITE_APP_BASE_API,
  headers: {
    'Content-Type': 'multipart/form-data',
    Accept: 'application/json',
    'Cache-Control': 'no-cache',
    'Access-Control-Allow-Origin': '*',
    'Access-Control-Allow-Methods': 'GET, POST, PUT',
    'Access-Control-Allow-Headers': 'Content-Type'
  },
  crossDomain: true
})

api.interceptors.request.use(
  (config) => {
    if (config.showLoader === true) {
      // store.dispatch('loader/PENDING');
    }
    return config
  },
  (error) => {
    // store.dispatch('loader/DONE');
    return Promise.reject(error.response.data.message)
  }
)

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response && error.response.status === 401) {
      logout() // Call the logout service
    }
    return Promise.reject(error)
  }
)

export default api
