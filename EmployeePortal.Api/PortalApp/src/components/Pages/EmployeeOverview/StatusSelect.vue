<template>
  <div v-if="props.roleId == currentEmployee.id">
    <select
      v-model="statusModel"
      class="edit-status"
      :disabled="false"
      v-if="
        canEditStatus(
          currentEmployee.id,
          props.relationship,
          item.status,
          props.roleId,
          item.startDate,
          item.employeeStatus,
          item.substituteStatus,
          item.supervisorStatus,
          item.adminStatus,
          props.substituteId
        )
      "
      @change="updateStatus(item)"
    >
      <option
        v-for="option in getOptions(item.status, props.relationship, props.roleStatus)"
        :value="option.value"
        :key="option.value"
      >
        {{ option.text }}
      </option>
    </select>
    <select
      v-model="statusModel"
      class="edit-status"
      :disabled="true"
      v-else-if="
        !canEditStatus(
          currentEmployee.id,
          props.relationship,
          item.status,
          props.roleId,
          item.startDate,
          item.employeeStatus,
          item.substituteStatus,
          item.supervisorStatus,
          item.adminStatus,
          props.substituteId
        )
      "
    >
      <option
        v-for="option in getOptions(item.status, props.relationship, props.roleStatus)"
        :value="option.value"
        :key="option.value"
      >
        {{ option.text }}
      </option>
    </select>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useEmployeeStore } from '@/stores/employee-store'
import { canEditStatus } from '@/Composables/canEditStatus'

const props = defineProps({
  item: Object,
  relationship: String,
  roleStatus: Number,
  roleId: Number,
  disabled: Boolean,
  substituteId: Number
})

const item = ref(props.item)

const { currentEmployee } = useEmployeeStore()

const emit = defineEmits(['updateStatus'])

const statusModel = computed({
  get() {
    if (props.relationship === 'admin' || props.relationship === 'substitute') {
      return item.value.substituteStatus
    } else if (props.relationship === 'supervisor') {
      return item.value.supervisorStatus
    } else if (props.relationship === 'employee') {
      return item.value.employeeStatus
    }
    return null
  },

  set(value) {
    if (props.relationship === 'admin' || props.relationship === 'substitute') {
      item.value.substituteStatus = value
    } else if (props.relationship === 'supervisor') {
      item.value.supervisorStatus = value
    } else if (props.relationship === 'employee') {
      item.value.employeeStatus = value
    }
  }
})

const updateStatus = (item) => {
  emit('updateStatus', item)
}

function getOptions(status, role, roleStatus) {
  const optionsMap = {
    1: [
      { value: 1, text: 'Urlaub Genehmigt' },
      { value: 4, text: 'Stornierung Beantragen' }
    ],
    2:
      roleStatus === 2
        ? [
            { value: 1, text: 'Urlaub Genehmigt' },
            { value: 2, text: 'Urlaub Abgelehnt' }
          ]
        : [
            { value: 1, text: 'Urlaub Genehmigt' },
            { value: 2, text: 'Urlaub Abgelehnt' },
            { value: 3, text: 'Urlaub Offen' },
            { value: 4, text: 'Stornierung Genehmigt' },
            { value: 5, text: 'Stornierung Abgelehnt' },
            { value: 6, text: 'Stornierung Offen' }
          ],
    3: [
      { value: 1, text: 'Urlaub Genehmigt' },
      { value: 2, text: 'Urlaub Abgelehnt' },
      { value: 3, text: 'Urlaub Offen' }
    ],
    4: [{ value: 4, text: 'Stornierung Genehmigt' }],
    5:
      roleStatus === 5
        ? [
            { value: 4, text: 'Stornierung Genehmigt' },
            { value: 5, text: 'Stornierung Abgelehnt' }
          ]
        : [
            { value: 1, text: 'Urlaub Genehmigt' },
            { value: 2, text: 'Urlaub Abgelehnt' },
            { value: 3, text: 'Urlaub Offen' },
            { value: 4, text: 'Stornierung Genehmigt' },
            { value: 5, text: 'Stornierung Abgelehnt' },
            { value: 6, text: 'Stornierung Offen' }
          ],
    6: role
      ? roleStatus === 6
        ? [
            { value: 4, text: 'Stornierung Genehmigt' },
            { value: 5, text: 'Stornierung Abgelehnt' },
            { value: 6, text: 'Stornierung Offen' }
          ]
        : roleStatus === 4 || role !== 'admin'
          ? [
              { value: 4, text: 'Stornierung Genehmigt' },
              { value: 5, text: 'Stornierung Abgelehnt' }
            ]
          : [
              { value: 4, text: 'Stornierung Genehmigt' },
              { value: 5, text: 'Stornierung Abgelehnt' }
            ]
      : [
          { value: 4, text: 'Stornierung Genehmigt' },
          { value: 5, text: 'Stornierung Abgelehnt' }
        ]
  }

  return optionsMap[status] || [{ value: null, text: 'Kein Handlungsbedarf' }]
}
</script>
