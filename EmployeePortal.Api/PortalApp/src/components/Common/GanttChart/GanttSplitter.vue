<template>
  <div class="splitter" @mousedown="onMouseDown"><p>&nbsp;</p></div>
</template>

<script>
export default {
  methods: {
    onMouseDown(e) {
      this.$emit('drag-start', e)
    }
  }
}
</script>

<style lang="scss" src="./GanttChart.scss" scoped />
