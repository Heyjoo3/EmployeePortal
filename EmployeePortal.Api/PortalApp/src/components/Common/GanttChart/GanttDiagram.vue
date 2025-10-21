<template>
  <div class="gantt-diagram">
    <table>
      <thead>
        <tr>
          <th v-for="month in monthInYears" :key="month.key" :colspan="month.days" class="top-row">
            {{ month.label }}
          </th>
        </tr>
        <tr>
          <th v-for="day in daysInYears" :key="day.key" class="bottom-row">
            {{ day.weekday + ' ' + day.label }}
          </th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="employee in props.teamData" :key="employee.userName">
          <td v-for="day in daysInYears" :key="day.key" class="cell">
            <div
              v-if="isVacationDay(employee.vacations, day.date)"
              :class="getVacationClass(employee.vacations, day.date)"
            >
              &nbsp;
            </div>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup>
import { computed, onMounted } from 'vue'
import { useDateConverter } from '@/Composables/useDateConverter'

const props = defineProps({
  teamData: Array
})

const today = new Date()
const startDate = new Date(today.getFullYear() - 1, 0, 1)
const endDate = new Date(today.getFullYear() + 2, 0, 0)

const getDaysInYears = (start, end) => {
  const days = []
  for (let date = new Date(start); date <= end; date.setDate(date.getDate() + 1)) {
    days.push({
      key: date.toISOString().slice(0, 10),
      label: date.getDate(),
      weekday: useDateConverter().getWeekday(date.getDay()).slice(0, 2),
      date: new Date(date)
    })
  }
  return days
}

const getMonthsInYears = (start, end) => {
  const months = []
  for (let date = new Date(start); date <= end; date.setMonth(date.getMonth() + 1)) {
    const year = date.getFullYear()
    const month = date.getMonth() + 1
    const daysInMonth = new Date(year, month, 0).getDate()
    months.push({
      key: `${year}-${month}`,
      label: `${month}.${year}`,
      days: daysInMonth
    })
  }
  return months
}

const daysInYears = computed(() => getDaysInYears(startDate, endDate))
const monthInYears = computed(() => getMonthsInYears(startDate, endDate))

const isVacationDay = (vacations, day) => {
  if (!Array.isArray(vacations) || vacations.length === 0) {
    return false
  }

  return vacations.some((vacation) => {
    const vacationStartDate = new Date(vacation.startDate)
    const vacationEndDate = new Date(vacation.endDate)
    return day >= vacationStartDate && day <= vacationEndDate
  })
}

const getVacationClass = (vacations, day) => {
  const vacation = vacations.find((vacation) => {
    const vacationStartDate = new Date(vacation.startDate)
    const vacationEndDate = new Date(vacation.endDate)
    return day >= vacationStartDate && day <= vacationEndDate
  })

  if (!vacation) return ''

  return vacation.vacationClass
}

const centerToday = () => {
  const daysPassed = Math.floor((today - startDate) / (1000 * 60 * 60 * 24)) - 1
  const diagram = document.querySelector('.gantt-diagram')
  diagram.scrollTo({
    left: (daysPassed / daysInYears.value.length) * diagram.scrollWidth
  })
}

const goLeft = () => {
  var diagram = document.querySelector('.gantt-diagram')
  diagram.scrollTo({
    left: diagram.scrollLeft - diagram.clientWidth / 2,
    behavior: 'smooth'
  })
}

const goRight = () => {
  var diagram = document.querySelector('.gantt-diagram')
  diagram.scrollTo({
    left: diagram.scrollLeft + diagram.clientWidth / 2,
    behavior: 'smooth'
  })
}

onMounted(() => {
  centerToday()
})

defineExpose({
  centerToday,
  goLeft,
  goRight
})
</script>

<style lang="scss" src="./GanttChart.scss" scoped />
