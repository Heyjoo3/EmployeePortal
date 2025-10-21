import { useEmployeeStore } from '@/stores/employee-store'
import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/login',
      name: 'Login',
      component: () => import('../views/LoginPageView.vue'),
      beforeEnter: (to, from, next) => {
        if (from.name === 'EmployeeOverview') {
          location.reload()
        } else {
          next()
        }
      }
    },
    {
      path: '/loading',
      name: 'Loading',
      component: () => import('../views/LoadingPageView.vue'),
      meta: { requiresAuth: true, permission: '' }
    },
    {
      path: '/',
      name: 'Layout',
      component: () => import('../components/Layout/PortalLayout.vue'),
      meta: { requiresAuth: true, permission: '' },
      children: [
        {
          path: '/userprofile',
          name: 'UserProfile',
          props: true,
          component: () => import('../views/UserProfileView.vue'),
          key: (route) => route.fullPath
        },
        {
          path: '/requests',
          name: 'VacationRequests',
          component: () => import('../views/VacationRequestsView.vue'),
          key: (route) => route.fullPath
        },
        {
          path: '/createuser',
          name: 'CreateUser',
          component: () => import('../views/CreateUserView.vue'),
          meta: { requiresAuth: true, permission: 'Admin' },
          key: (route) => route.fullPath
        },
        {
          path: '/publicholidays',
          name: 'PublicHolidays',
          component: () => import('../views/PublicHolidaysView.vue'),
          meta: { requiresAuth: true, permission: 'Admin' },
          key: (route) => route.fullPath
        },
        {
          path: '/companyVacationDays',
          name: 'CompanyVacationDays',
          component: () => import('../views/CompanyVacationDaysView.vue'),
          meta: { requiresAuth: true, permission: 'Admin' },
          key: (route) => route.fullPath
        },
        {
          path: '/overview/:department',
          name: 'OverviewPage',
          component: () => import('../views/OverviewView.vue'),
          props: true,
          key: (route) => route.fullPath
        },
        {
          path: '/employee/:employeeInternalId/:relationship',
          name: 'EmployeeOverview',
          component: () => import('../views/EmployeeOverviewView.vue'),
          props: true,
          key: (route) => route.fullPath,
          beforeEnter: (to, _, next) => {
            const store = useEmployeeStore()
            const employeeInternalId = to.params.employeeInternalId
            const userExists = store.state.employees.some(
              (employee) => employee.employeeInternalId == employeeInternalId
            )
            if (userExists) {
              next()
            } else {
              next({ name: 'UserProfile' })
            }
          }
        },
        {
          path: '/Onboardingvorlage-Erstellen',
          name: 'CreateOnboardingTemplate',
          component: () => import('../views/CreateOnboardingTemplateView.vue'),
          meta: { requiresAuth: true, permission: ['Admin', 'Supervisor'] },
          key: (route) => route.fullPath
        },
        {
          path: '/Onboarding-Erstellen',
          name: 'CreateOnboardingPlan',
          component: () => import('../views/CreateOnboardingPlanView.vue'),
          meta: { requiresAuth: true, permission: 'Admin' },
          key: (route) => route.fullPath
        },
        {
          path: '/Onboarding/:title',
          name: 'OnboardingPlan',
          component: () => import('../views/OnboardingPlanView.vue'),
          props: true,
          key: (route) => route.fullPath
        },
        {
          path: '/Onboardingvorlage/:title',
          name: 'OnboardingTemplate',
          component: () => import('../views/OnboardingTemplateView.vue'),
          props: true,
          key: (route) => route.fullPath
        }
      ]
    },
    {
      path: '/:pathMatch(.*)*',
      name: 'NotFound',
      beforeEnter: (to, from, next) => {
        const loggedIn = localStorage.getItem('jwtToken')
        if (!loggedIn) {
          next({ name: 'Login' })
        } else {
          next({ name: 'UserProfile' })
        }
      }
    },
    {
      path: '',
      name: 'Redirect',
      beforeEnter: (to, from, next) => {
        const loggedIn = localStorage.getItem('jwtToken')
        if (!loggedIn) {
          next({ name: 'Login' })
        } else {
          next({ name: 'UserProfile' })
        }
      }
    }
  ]
})

router.beforeEach((to, from, next) => {
  const loggedIn = true
  const userRole = 'Admin' // This should be fetched from user state/store

  if (to.matched.some((record) => record.meta.requiresAuth)) {
    if (!loggedIn) {
      next({ name: 'Login' })
    } else if (to.matched.some((record) => record.meta.permission)) {
      const requiredPermission = to.meta.permission
      if (Array.isArray(requiredPermission)) {
        if (requiredPermission.includes(userRole)) {
          next()
        } else {
          next({ name: 'UserProfile' })
        }
      } else if (userRole === requiredPermission) {
        next()
      } else {
        next({ name: 'UserProfile' })
      }
    } else {
      next()
    }
  } else {
    next()
  }
})

export default router
