<template>
  <div>
    <v-list-subheader>Administration</v-list-subheader>
    <v-list-item
      prepend-icon="mdi-account-plus"
      title="Mitarbeiter erstellen"
      value="create"
      @click="goToRoute('CreateUser')"
    ></v-list-item>
    <v-list-item
      prepend-icon="mdi-label-outline"
      title="Feiertage hinzufügen"
      @click="goToRoute('PublicHolidays')"
      value="holiday"
    ></v-list-item>
    <v-list-item
      prepend-icon="mdi-office-building-remove-outline"
      title="Betriebsurlaub hinzufügen"
      @click="goToRoute('CompanyVacationDays')"
      value="companyVacation"
    ></v-list-item>
  </div>
    <div>
   <v-list-subheader>Onboarding</v-list-subheader>
   <v-list-group v-for="item in onboardingObject" :key="item.title">
    <template v-if="item.title === 'Vorlagen'">
      <v-list-item
        title="+ Neue Vorlage"
        @click="goToRoute('CreateOnboardingTemplate', null, 'Admin')"
        value="create-template"
        base-color="green"
         style="border: 2pt solid green; font-weight: bold;"
      ></v-list-item>
    </template>
        <template v-if="item.title === 'Onboardings'">
      <v-list-item
        title="+ Neues Onboarding"
        @click="goToRoute('CreateOnboardingPlan', null, 'Admin')"
        value="create-onboarding"
        base-color="green"
         style="border: 2pt solid green; font-weight: bold;"
      ></v-list-item>
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
        @click="goToRoute(subItem.routeName, subItem.title.toLowerCase(), 'Admin')"
        :key="subItem.title"
      ></v-list-item>
    </v-list-group>
    </div>
  <div>
    <v-list-subheader>Teams</v-list-subheader>
    <v-list-group v-for="item in props.allTeams" :key="item.department">
      <template v-slot:activator="{ props }">
        <v-list-item
          v-bind="props"
          prepend-icon="mdi-account-group"
          :title="item.department"
          :value="item.department"
        ></v-list-item>
      </template>
      <v-list-item
        title="Übersicht"
        :value="item.department"
        @click="goToRoute('OverviewPage', item.department, 'admin')"
      ></v-list-item>
      <v-list-item
        v-for="employee in item.employees"
        :title="employee.name"
        @click="goToRoute('EmployeeOverview', employee.employeeInternalId, 'admin', employee.id)"
        :key="employee.employeeInternalId"
      ></v-list-item>
    </v-list-group>
  </div>
</template>

<script setup>
import { goToRoute } from '@/Composables/goToRoute'

const props = defineProps({
  allTeams: Array
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
      { title: 'Max Mustermann', routeName: 'OnboardingPlan' },
      { title: 'Peter Müller', routeName: 'OnboardingPlan' }
    ]
  }
]
</script>
