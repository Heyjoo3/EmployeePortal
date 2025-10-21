import { useEmployeeStore } from '@/stores/employee-store.js'
import router from '@/router'

const employeeStore = useEmployeeStore()

const { actions } = employeeStore

export function goToRoute(route, param = null, relationship = null, id = null) {
  {
    if (route === 'OverviewPage') {
      actions.setPickedDepartment(param)
      actions.setPickedRelation(relationship)
      router.push({ name: route, params: { department: param } })
    } else if (route === 'EmployeeOverview') {
      actions.setPickedEmployeeId(id)
      actions.setPickedRelation(relationship)
      router.push({
        name: route,
        params: { employeeInternalId: param, relationship: relationship }
      })
    }
     else if (param) {
      console.log('Navigating to route:', route, 'with param:', param)
      router.push({ name: route, params: { title: param }, relationship: relationship })
    } else if (relationship) {
      console.log('Navigating to route:', route, 'with relationship:', relationship, 'and params:', param)
      router.push({ name: route, params: null,  relationship: relationship  })
    }
    else {
      console.log('Navigating to route:', route)
      router.push({ name: route })
    }
  }
}
