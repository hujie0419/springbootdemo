<template>
  <div class="add-container">
    <el-button class="el-icon-circle-plus"
      type="text"
      @click="addControl"
      v-if='showAddBtn'>
    </el-button>
    <el-button class="el-icon-remove"
      type="text"
      @click="minusControl"
      v-if='showMinusBtn'>
    </el-button>
  </div>
</template>
<script>
export default {
    props: {
        showAddBtn: {
            type: Boolean,
            default: true
        },
        showMinusBtn: {
            type: Boolean,
            default: true
        }
    },
    methods: {
        addControl (data) {
            this.$emit('addControl', data);
        },
        minusControl (data) {
            this.$emit('minusControl', data);
        }
    }
};
</script>
<style lang='scss' scoped>
.add-container {
  display: flex;
  justify-content: center;
  button{
    font-size: 30px;
  }
}
</style>
