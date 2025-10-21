<template>
  <div class="add-company-vacation">
    <h2>Betriebsurlaub hinzufügen</h2>
    <p>
      <i
        >Heiligabend und Silvester zählen als halbe Tage. Die Urlaubslänge muss entsprechend
        angepasst werden.</i
      >
    </p>
    <br />
    <company-vacation-form @updateCompanyVacations="updateCompanyVacations()" />
  </div>

  <div class="vacation-days-edit" v-if="companyVacationLoaded">
    <h2>Betriebsurlaube</h2>
    <company-vacation-table
      :key="tableKey"
      :companyVacations="allCompanyVacations"
      @deleteCompanyVacation="deleteVacation($event)"
    />
  </div>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import { useVacationStore } from '@/stores/vacation-store'
// eslint-disable-next-line no-unused-vars
import CompanyVacationForm from '@/components/Pages/CompanyVacationDays/CompanyVacationForm.vue'
import CompanyVacationTable from '@/components/Pages/CompanyVacationDays/CompanyVacationTable.vue'

const tableKey = ref(0)

const vacationStore = useVacationStore()
const { actions } = vacationStore

const companyVacationForm = ref(null)

let allCompanyVacations = ref([])

let companyVacationLoaded = ref(false)

const deleteVacation = async (vacationId) => {
  await actions.deleteCompanyVacation(vacationId)
  await updateCompanyVacations()
}

async function updateCompanyVacations() {
  allCompanyVacations.value = await vacationStore.actions.getCompanyVacations()
  tableKey.value++
}

onMounted(async () => {
  await updateCompanyVacations()
  companyVacationLoaded.value = true
})
</script>

<style lang="scss" src="./CompanyVacation.scss" scoped />
