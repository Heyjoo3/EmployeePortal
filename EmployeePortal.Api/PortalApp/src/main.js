import './assets/main.css'
import { createApp } from 'vue'
import App from './App.vue'

import 'vuetify/styles'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
// import * as labs from 'vuetify/directives'
import '@mdi/font/css/materialdesignicons.css'
import * as VueI18n from 'vue-i18n'
import { createI18n } from 'vue-i18n'
import { createPinia } from 'pinia'
import piniaPluginPersistedstate from 'pinia-plugin-persistedstate'
import { VDateInput } from 'vuetify/lib/labs/components.mjs'
// import { VDateInput, VNumberInput } from 'vuetify/lib/labs/components.mjs'
import router from './router'

const pinia = createPinia()
pinia.use(piniaPluginPersistedstate)
const app = createApp(App)
const vuetify = createVuetify({
  components: {
    ...components,
    VDateInput,
    // VNumberInput
  },
  directives,
  // labs,
  locale: {
    locale: 'de',
    fallback: 'en'
  }
})

app.use(VueI18n)
app.use(pinia)
app.use(router)
app.use(vuetify)
app.mount('#app')

export const i18n = createI18n({
  locale: 'de',
  fallbackLocale: 'de'
})
