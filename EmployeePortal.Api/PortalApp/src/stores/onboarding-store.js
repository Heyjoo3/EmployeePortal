import onboardingService from '@/services/onboarding-service'
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import router from '@/router'

export const useOnboardingStore = defineStore(
  'onboarding',
  () => {
    return {
      state,
      getters,
      actions
    }
  },
  { persist: true }
)

const state = ref({
  currentTemplate: {
    id: null,
    name: '',
    groups: [] // each group: { id, name, contactPerson, contactPersonName, tasks: [] }
  },
  onboardingPlans: [],
  currentTask: null,
  currentGroup: null,
  currentTasks: [],
  currentGroups: [],
  currentDetails: null
})

const getters = computed({
  currentTemplate: () => state.value.currentTemplate,
  onboardingPlans: () => state.value.onboardingPlans,
  currentTask: () => state.value.currentTask,
  currentGroup: () => state.value.currentGroup,
  currentGroups: () => state.value.currentGroups,
  currentDetails: () => state.value.currentDetails
})

const actions = {
  //Saves task to state to make changes and additions persistent across components
  saveTask(taskGroupId, task) {
    const group = state.value.currentGroups.find((g) => g.id === taskGroupId)
    if (!group) {
      throw new Error('Group not found')
    }

    if (group.tasks.length === 0) {
      task.id = Date.now() // simple unique ID for new tasks before saved to backend
      group.tasks.push(task)
      return
    }

    const existingTaskIndex = group.tasks.findIndex((t) => t.id === task.id)
    if (existingTaskIndex !== -1) {
      group.tasks.splice(existingTaskIndex, 1, task)
    }
  },
//Saves task group to state to make changes and additions persistent across components
  saveTaskGroup(group) {
    console.log('Saving task group:', group)
    if (group.id == null) {
      group.id = Date.now() // simple unique ID for new groups before saved to backend
    }
    const existingGroupIndex = state.value.currentGroups.findIndex((g) => g.id === group.id)
    if (existingGroupIndex !== -1) {
      state.value.currentGroups.splice(existingGroupIndex, 1, group)
    } else {
      state.value.currentGroups.push(group)
    }
  },
//Creates onboarding plan by calling onboarding service and then resets state and navigates to created plan
  async createOnboardingPlan(onboardingPlan) {
    try {
      console.log('store data:', onboardingPlan)
      onboardingPlan.taskGroups = state.value.currentGroups
      console.log('store data with groups:', onboardingPlan)

      const response = await onboardingService.createOnboardingPlan(onboardingPlan)
      resetState()
      router.push({ name: 'OnboardingPlans/' + response.id })
      return response
    } catch (error) {
      console.error('Error creating onboarding plan:', error)
      throw error
    }
  },
}

const resetState = () => {
  state.value = {
    currentTemplate: {
      id: null,
      name: '',
      groups: []
    },
    onboardingPlans: [],
    currentTask: null,
    currentGroup: null,
    currentTasks: [],
    currentGroups: [],
    currentDetails: null
  }
}

// import { defineStore } from 'pinia'
// import { onboardingService } from '@/services/onboarding-service'

// export const useOnboardingStore = defineStore('onboarding', {
//   state: () => ({
//     // the template currently being edited
//     currentTemplate: {
//       id: null,
//       name: '',
//       groups: [], // each group: { id, name, contactPerson, contactPersonName, tasks: [] }
//     },
//   }),

//   actions: {
//     initTemplate(template = null) {
//       if (template) {
//         // shallow copy to avoid direct external mutation
//         this.currentTemplate = JSON.parse(JSON.stringify(template))
//         if (!Array.isArray(this.currentTemplate.groups)) this.currentTemplate.groups = []
//       } else {
//         this.currentTemplate = { id: null, name: '', groups: [] }
//       }
//     },

//     setTemplateName(name) {
//       this.currentTemplate.name = name
//     },

//     addOrUpdateGroup(payload) {
//       const id = payload.id ?? Date.now()
//       const existing = this.currentTemplate.groups.find((g) => g.id === id)
//       if (existing) {
//         existing.name = payload.name
//         existing.contactPerson = payload.contactPerson
//         existing.contactPersonName = payload.contactPersonName ?? existing.contactPersonName
//         if (!Array.isArray(existing.tasks)) existing.tasks = existing.tasks ?? []
//         return existing
//       }

//       const group = {
//         id,
//         name: payload.name,
//         contactPerson: payload.contactPerson ?? null,
//         contactPersonName: payload.contactPersonName ?? '',
//         tasks: payload.tasks ? [...payload.tasks] : [],
//       }
//       this.currentTemplate.groups.push(group)
//       return group
//     },

//     removeGroup(groupId) {
//       this.currentTemplate.groups = this.currentTemplate.groups.filter((g) => g.id !== groupId)
//     },

//     addOrUpdateTask(groupId, taskPayload) {
//       const group = this.currentTemplate.groups.find((g) => g.id === groupId)
//       if (!group) throw new Error('Group not found')
//       if (!Array.isArray(group.tasks)) group.tasks = []

//       const id = taskPayload.id ?? Date.now()
//       const existingIndex = group.tasks.findIndex((t) => t.id === id)
//       const task = { id, ...taskPayload }
//       if (existingIndex === -1) {
//         group.tasks.push(task)
//       } else {
//         group.tasks.splice(existingIndex, 1, task)
//       }
//       return task
//     },

//     removeTask(groupId, taskId) {
//       const group = this.currentTemplate.groups.find((g) => g.id === groupId)
//       if (!group || !Array.isArray(group.tasks)) return
//       group.tasks = group.tasks.filter((t) => t.id !== taskId)
//     },

//     async submitTemplate(apiUrl = '/api/onboarding-templates') {
//       // POST currentTemplate to backend; returns parsed response or throws
//       const payload = JSON.parse(JSON.stringify(this.currentTemplate))
//       const res = await fetch(apiUrl, {
//         method: payload.id ? 'PUT' : 'POST',
//         headers: { 'Content-Type': 'application/json' },
//         body: JSON.stringify(payload),
//       })
//       if (!res.ok) {
//         const text = await res.text()
//         throw new Error(`Failed to submit template: ${res.status} ${text}`)
//       }
//       const data = await res.json()
//       // optionally re-init store with server response
//       this.initTemplate(data)
//       return data
//     },

//     async createOnboardingPlan (onboardingPlan) {
//       try {
//         const response = await onboardingService.createOnboardingPlan(onboardingPlan)
//         return response
//       } catch (error) {
//         console.error('Error creating onboarding plan:', error)
//         throw error
//       }
//     },
//   },
// })
