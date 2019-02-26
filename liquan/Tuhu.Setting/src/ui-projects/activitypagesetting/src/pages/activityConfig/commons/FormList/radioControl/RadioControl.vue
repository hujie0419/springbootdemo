<template>
  <form-control-wrap
    v-if="controlConfig"
    :bypassedWrap="bypassedWrap"
    class="radio-control-container"
    :form-model="formModel"
    :control-config="controlConfig"
    :valueData="myValueData"
    @extendClick="extendClick">
    <el-radio-group class="radio-group-wrap"  v-if="controlConfig" v-model="myValue">
      <el-radio
        v-for="item in controlConfig.list"
        :readonly="item.readonly"
        :disabled="item.disabled"
        :key="item.value"
        :label="item.value">
          {{item.nameText}}
      </el-radio>
    </el-radio-group>
  </form-control-wrap>
</template>

<script>
import FormControlWrap from '../common/formControlWrap/FormControlWrap';
import ControlExtend from '../common/controlExtend/ControlExtend';
export default {
    extends: ControlExtend,
    components: {
        FormControlWrap
    }
    // props: {
    //     control: {
    //         type: Object
    //     }
    // },
    // methods: {
    // }
};
</script>

<style lang='scss' scoped>
.radio-control-container {
    .form-control-leftPadding{
        margin-left: 0;
    }
  // flex: 0 0 50%;
  // .el-radio-group {
  //   width: 100%;
  // }
}
</style>
