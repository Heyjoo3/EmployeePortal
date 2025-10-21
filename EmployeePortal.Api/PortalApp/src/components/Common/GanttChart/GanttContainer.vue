<template>
  <div class="component">
    <div v-if="isloading" class="loading-info">
      <h2>Einen Moment bitte...</h2>
      <v-progress-circular
        color="var(--primary-blue)"
        indeterminate
        :size="128"
        :width="10"
      ></v-progress-circular>
    </div>

    <div v-else-if="teamData == null || teamData.length <= 0" class="no-data-info">
      <h2>Es gibt hier nichts zu sehen.</h2>
    </div>

    <div class="chart-container" v-else>
      <div class="gantt-header">
        <div class="color-explaination">
          <div class="color-label-set">
            <div class="color-box" id="approved"></div>
            <p>Genehmigt</p>
          </div>
          <div class="color-label-set">
            <div class="color-box" id="cancelled"></div>
            <p>Abgelehnt</p>
          </div>
          <div class="color-label-set">
            <div class="color-box" id="needs-approval-vacation"></div>
            <p>Braucht meine Begutachtung</p>
          </div>
          <div class="color-label-set">
            <div class="color-box" id="pending"></div>
            <p>In Bearbeitung</p>
          </div>
          <div class="color-label-set">
            <div class="color-box" id="company-vacation"></div>
            <p>Betriebsurlaub</p>
          </div>
          <div class="color-label-set">
            <div class="color-box" id="cancellation-approved"></div>
            <p>Stornierung genehmigt</p>
          </div>
          <div class="color-label-set">
            <div class="color-box" id="cancellation-cancelled"></div>
            <p>Stornierung abgelehnt</p>
          </div>
          <div class="color-label-set">
            <div class="color-box" id="needs-approval-cancellation"></div>
            <p>Stornierung braucht meine Begutachtung</p>
          </div>
          <div class="color-label-set">
            <div class="color-box" id="cancellation-pending"></div>
            <p>Stornierung in Bearbeitung</p>
          </div>
        </div>
        <div class="buttons-container">
          <button @click="goLeft">Zurück</button>
          <button @click="centerToday">Heute</button>
          <button @click="goRight">Vor</button>
        </div>
      </div>
      <div class="gantt-chart" @mousemove="onMouseMove" @mouseup="onMouseUp">
        <div class="gantt-table-container" :style="{ flexBasis: tableWidth + 'px' }">
          <GanttTable :teamData="teamData" />
        </div>
        <Splitter @drag-start="onDragStart" />
        <div class="gantt-diagram-container">
          <GanttDiagram :teamData="teamData" ref="ganttDiagram" />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted, ref, watch } from 'vue'
import GanttTable from './GanttTable.vue'
import GanttDiagram from './GanttDiagram.vue'
import Splitter from './GanttSplitter.vue'
import { useGanttStore } from '@/stores/gantt-store'

const props = defineProps({
  refresh: Boolean
})

watch(
  () => props.refresh,
  async (newValue, oldValue) => {
    if (newValue !== oldValue) {
      isloading.value = true
      teamData.value = await ganttStore.actions.getCompleteTeamData()
      isloading.value = false
    }
  }
)

const ganttStore = useGanttStore()

const teamData = ref(null)
const isloading = ref(true)

const ganttDiagram = ref(null)
const isDragging = ref(false)
const tableWidth = ref(380)

const centerToday = () => {
  ganttDiagram.value.centerToday()
}

const goLeft = () => {
  ganttDiagram.value.goLeft()
}

const goRight = () => {
  ganttDiagram.value.goRight()
}

const onDragStart = () => {
  isDragging.value = true
}

const onMouseMove = (e) => {
  if (isDragging.value) {
    const newWidth = e.clientX - 250 // 250px is the width of the side nav
    if (newWidth >= 100) {
      tableWidth.value = newWidth
    }
  }
}

const onMouseUp = () => {
  isDragging.value = false
}

onMounted(async () => {
  isloading.value = true

  teamData.value = await ganttStore.actions.getCompleteTeamData()

  isloading.value = false
})
</script>

<style lang="scss" src="./GanttChart.scss" scoped />
