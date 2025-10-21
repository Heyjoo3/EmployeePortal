import api from './api'
import _ from 'lodash'
import { authHeader } from '@/Composables/authorizationHeader'

export default {
  createOnboardingPlan: async (plan) => {
    console.log('onboarding-service plan data:', plan)
    const formData = new FormData()
    const copyPlan = _.cloneDeep(plan)
    console.log('Cloned plan data:', copyPlan)
    const data = copyPlan
    console.log('Creating onboarding plan with data:', data)
    formData.append('onboardingPlanData', JSON.stringify(data))

    try {
      const response = await api.post('/Onboarding/CreateOnboardingPlan', formData, {
        headers: authHeader()
      })
      return response.data
    } catch (error) {
      console.error('Error creating onboarding plan:', error)
      throw error
    }
  },
}