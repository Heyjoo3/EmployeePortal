<template>
  <div>
    <v-btn color="primary" width="100%" @click="openEditTaskDialog(true, null)"
      >+ Aufgabe hinzufügen</v-btn
    >
    <v-list v-for="task in tasks" :key="task.id" bg-color="var(--very-light-blue)">
      <div class="list-item">
        <div class="item-title">{{ task.name }}</div>
        <div class="buttons">
          <v-btn
            :color="'primary'"
            density="compact"
            icon
            rounded="true"
            @click="openViewTaskDialog(true, task)"
          >
            <v-icon size="18" color="white">mdi-eye</v-icon>
          </v-btn>
          <v-btn
            :color="'var(--status-inactive-update)'"
            density="compact"
            icon
            rounded="true"
            @click="openEditTaskDialog(true, task)"
          >
            <v-icon size="18" color="white">mdi-pencil</v-icon>
          </v-btn>
          <v-btn
            color="error"
            density="compact"
            icon="mdi-delete"
            rounded="true"
            @click="console.log('Delete clicked')"
          ></v-btn>
        </div>
      </div>
    </v-list>
  </div>
  <EditOnboardingTask
    v-if="showEditTaskDialog"
    @close="openEditTaskDialog(false, null)"
    @submit="handleSaveTask"
    :selectedTask="selectedTask"
  />
  <ViewOnboardingTask
    v-if="showViewTaskDialog"
    @close="openViewTaskDialog(false, null)"
    :selectedTask="selectedTask"
  />
</template>

<script setup>
import EditOnboardingTask from '@/components/Pages/OnboardingPlan/EditOnboardingTask.vue'
import ViewOnboardingTask from '@/components/Pages/OnboardingPlan/ViewOnboardingTask.vue'
import { onMounted, ref } from 'vue'

const props = defineProps({
  groupId: {
    type: Number,
    required: true,
  },
  tasks: {
    type: Array,
    default: () => [],
  },
})

const emit = defineEmits(['save-task'])

const showEditTaskDialog = ref(false)
const showViewTaskDialog = ref(false)
const selectedTask = ref(null)
const clonedTasks = ref([])

const openEditTaskDialog = (show, task) => {
  showEditTaskDialog.value = show
  console.log('Open Edit Task Dialog:', show, task)
}

const openViewTaskDialog = (show, task) => {
  showViewTaskDialog.value = show
  console.log('Open View Task Dialog:', show, task)
}

const handleSaveTask = (payload) => {
   const newTask = {
    id: payload.id || Date.now(),
    ...payload,
  }

  // update local copy
  clonedTasks.value.push(newTask)
    emit('save-task', { groupId: props.groupId, task: newTask })

  console.log('Saved Task:', clonedTasks.value[clonedTasks.value.length - 1])
  showEditTaskDialog.value = false
}

onMounted(() => {
       clonedTasks.value = Array.isArray(props.tasks) ? [...props.tasks] : []

})
</script>

<style scoped>
.list-item {
  padding: 8px 16px;
  border: 1px solid rgb(205, 205, 218);
  border-radius: 8px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  background-color: white;
}

.item-title {
  font-size: 16px;
  width: 80%;
}

.buttons {
  display: flex;
  gap: 10px;
  width: 104px;
}
</style>
