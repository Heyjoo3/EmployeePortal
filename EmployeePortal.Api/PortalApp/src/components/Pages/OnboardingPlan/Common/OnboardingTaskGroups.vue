<template>
  <div class="task-groups">
    <v-btn color="primary" width="100%" class="mb-3" @click="openEditTaskGroupDialog(true, null)">+ Aufgabengruppe
      hinzufügen</v-btn>

  <v-expansion-panels v-if="taskGroups != null && taskGroups.length > 0">
      <v-expansion-panel v-for="(group, idx) in taskGroups" :key="group.id" bg-color="var(--very-light-blue)"
        :style="{ marginBottom: idx < taskGroups.length - 1 ? '20px' : '0' }">
        <v-expansion-panel-title>
          <div class="header-taskgroup">
            <div>
              <h2>{{ group.title }}</h2>
              <br />
              <p><strong>Ansprechpartner:</strong> {{ group.contactPersonName }}</p>
            </div>
            <div class="buttons-taskgroup">
              <v-btn :color="'var(--status-inactive-update)'" density="compact" icon rounded="true"
                @click.stop="openEditTaskGroupDialog(true, group)">
                <v-icon size="18" color="white">mdi-pencil</v-icon>
              </v-btn>
              <v-btn color="error" density="compact" icon="mdi-delete" rounded="true"
                @click.stop="console.log('Delete clicked', group)"></v-btn>
            </div>
          </div>
        </v-expansion-panel-title>
        <v-expansion-panel-text>
          <OnboardingTasks :groupId="group.id" :tasks="group.tasks" />
        </v-expansion-panel-text>
      </v-expansion-panel>
    </v-expansion-panels>
  </div>
  <EditOnboardingTaskGroup v-if="showEditTaskGroupDialog" @close="openEditTaskGroupDialog(false, null)"
    @submit="handleSaveTaskGroup" @save-task="handleTaskSaved" :selectedGroup="selectedGroup" />
</template>

<script setup>
import { useOnboardingStore } from '@/stores/onboarding-store'
import OnboardingTasks from './OnboardingTasks.vue'
import EditOnboardingTaskGroup from '@/components/Pages/OnboardingPlan/AddEditDialogs/EditOnboardingTaskGroup.vue'
import { onMounted, ref, toRef } from 'vue'

const onboardingStore = useOnboardingStore()

const showEditTaskGroupDialog = ref(false)

const taskGroups = toRef(onboardingStore.state.currentGroups)


const openEditTaskGroupDialog = (show, group) => {
  showEditTaskGroupDialog.value = show
  console.log('Open Edit Task Group Dialog:', show, group)
}

// const handleSaveTaskGroup = (payload) => {
//   taskGroups.value.push({
//     id: payload.id || Date.now(),
//     name: payload.name,
//     contactPerson: payload.contactPerson,
//     contactPersonName: payload.contactPersonName ? payload.contactPersonName : '',
//   })

//   showEditTaskGroupDialog.value = false

// }

// const handleTaskSaved = ({ groupId, task }) => {
//   const group = taskGroups.value.find((g) => g.id === groupId)
//   if (!group) {
//     console.warn('Group not found for saved task', groupId, task)
//     return
//   }
//   if (!Array.isArray(group.tasks)) group.tasks = []
//   group.tasks.push(task)
//   console.log('Task added to group', groupId, task)
// }

onMounted(() => {
 console.log('Onboarding Task Groups mounted, current task groups:', taskGroups)
})
</script>

<style scoped>
.task-groups {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.header-taskgroup {
  width: 100%;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-right: 28px;
}

.buttons-taskgroup {
  display: flex;
  gap: 10px;
  width: 72px;
}
</style>
