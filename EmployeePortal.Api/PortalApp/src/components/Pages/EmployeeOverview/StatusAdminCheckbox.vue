<template>
  <v-checkbox
    class="checkbox"
    v-model="item.adminStatus"
    :disabled="
      !canEditStatus(
        currentEmployee.id,
        props.relationship,
        item.status,
        item.admin,
        item.startDate,
        item.employeeStatus,
        item.substituteStatus,
        item.supervisorStatus,
        item.adminStatus,
        0
      )
    "
    :true-value="props.trueValue"
    @change="updateStatus(item)"
  ></v-checkbox>
</template>

<script setup>
import { ref } from 'vue'
import { canEditStatus } from '@/Composables/canEditStatus'
import { useEmployeeStore } from '@/stores/employee-store'

const props = defineProps({
  item: Object,
  relationship: String,
  trueValue: Number
})

const emit = defineEmits(['updateStatus'])

const { currentEmployee } = useEmployeeStore()

const updateStatus = (item) => {
  emit('updateStatus', item)
}

const item = ref(props.item)
</script>
