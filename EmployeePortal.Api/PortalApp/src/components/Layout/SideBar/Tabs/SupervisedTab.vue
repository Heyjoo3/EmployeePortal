<template>
  <div>
    <v-list-subheader>Teammitglieder</v-list-subheader>
    <v-list-item
      prepend-icon="mdi-account-supervisor"
      title="Übersicht"
      value="supervised"
      @click="goToRoute('OverviewPage', 'supervised', 'supervisor')"
    >
    </v-list-item>
    <v-list-item
      v-for="employee in props.supervised"
      :key="employee.id"
      :title="employee.name"
      prepend-icon="mdi-account"
      @click="goToRoute('EmployeeOverview', employee.employeeInternalId, 'supervisor', employee.id)"
    ></v-list-item>
  </div>
    <div>
   <v-list-subheader>Onboarding</v-list-subheader>
  <v-list-group v-for="item in onboardingObject" :key="item.title">
    <template v-if="item.title === 'Vorlagen'">
      <v-list-item
        title="+ Neue Vorlage"
        @click="goToRoute('CreateOnboardingTemplate', null, 'Supervisor')"
        value="create-template"
        base-color="green"
        style="border: 2pt solid green; font-weight: bold;"
      ></v-list-item>
    </template>
        <template v-if="item.title === 'Onboardings'">
    </template>
      <template v-slot:activator="{ props }">
        <v-list-item
          v-bind="props"
          :prepend-icon="item.icon"
          :title="item.title"
          :value="item.title"
        ></v-list-item>
      </template>
      <v-list-item
        v-for="subItem in item.subItems"
        :title="subItem.title"
        @click="goToRoute(subItem.routeName, subItem.title.toLowerCase(), 'Supervisor')"
        :key="subItem.title"
      ></v-list-item>
    </v-list-group>
    </div>
</template>

<script setup>
import { goToRoute } from '@/Composables/goToRoute'

const props = defineProps({
  supervised: Array
})


const onboardingObject = [
  {
    title: 'Vorlagen',
    icon: 'mdi-file-document-edit-outline',
    subItems: [
      { title: 'Vorlage1', routeName: 'OnboardingTemplate' },
      { title: 'Vorlage2', routeName: 'OnboardingTemplate' }
    ],
  },
  {
    title: 'Onboardings',
    icon: 'mdi-account-file-text',
    subItems: [
      { title: 'Onboarding1', routeName: 'OnboardingPlan' },
      { title: 'Onboarding2', routeName: 'OnboardingPlan' }
    ]
  }
]
</script>
s
