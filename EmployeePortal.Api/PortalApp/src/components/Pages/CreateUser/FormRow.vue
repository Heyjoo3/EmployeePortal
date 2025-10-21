<template>
  <v-row class="slots-row">
    <div v-if="$slots.firstSlot" class="slot">
      <slot name="firstSlot"></slot>
    </div>
    <div v-if="$slots.secondSlot" class="slot">
      <slot name="secondSlot"></slot>
    </div>
    <div v-if="$slots.thirdSlot" class="slot">
      <slot name="thirdSlot"></slot>
    </div>
    <div v-if="$slots.fourthSlot" class="slot">
      <slot name="fourthSlot"></slot>
    </div>
  </v-row>
</template>

<style scoped>
.slots-row {
  display: flex;
  justify-content: space-between;
}
.slot {
  flex: 1;
  margin: 0 8px;
}
</style>
