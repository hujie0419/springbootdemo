<template>
  <div class="myInput">
    <el-input v-model="selected.minPrice" placeholder="请输入车价"><span slot="suffix">万</span></el-input>
    -
    <el-input v-model="selected.maxPrice" placeholder="请输入车价"><span slot="suffix">万</span></el-input>
    <el-tooltip class="item" effect="dark" content="区间左开右闭" placement="top-start">
      <span class="mr_icon">&#xe60E;</span>
    </el-tooltip>
  </div>
</template>
<script>
export default {
  props: {
    selected: {
      type: Object,
      default: () => {
        return {};
      }
    }
  }
};
</script>
<style lang="scss" scoped>
@import "css/common/_var.scss";
.myInput{
  display: inline-block;
  .mr_icon{
    color: $color9;
  }
  /deep/ .el-input{
    width: 140px;
    .el-input__suffix{
      line-height: 40px;
      color: $color3;
      right: 10px;
    }
  }
}
</style>


