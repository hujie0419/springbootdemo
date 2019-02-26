<template>
  <div class="mySelect">
    <el-select v-model="mySelected" clearable filterable  :placeholder="title" @change="change">
      <el-option
        v-for="(item,index) in selectList"
        :key="index"
        :label="item.split('|')[0]"
        :value="item.split('|')[item.split('|').length-1]">
      </el-option>
    </el-select>
  </div>
</template>
<script>
export default {
  props: {
    title: {
      type: String,
      default: ''
    },
    selectList: {
      type: Array,
      default: () => {
        return [];
      }
    },
    selected: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      mySelected: ''
    };
  },
  watch: {
    selected(val) {
      this.mySelected = val;
    }
  },
  methods: {
    change(data) {
      this.$emit('update:selected', data);
      this.$emit('change');
    }
  }
};
</script>
<style lang="less" scoped>
.mySelect{
  display: inline-block;
}
</style>


