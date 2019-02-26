<template>
  <form-control-wrap
    class="radio-select-control"
    v-if="controlConfig"
    :bypassedWrap="bypassedWrap"
    :form-model="formModel"
    :control-config="controlConfig"
    :valueData="myValueData"
    @extendClick="extendClick">
        <el-radio
            :readonly="controlConfig.readonly"
            :disabled="controlConfig.disabled"
            v-model="checkValue"
            :label="controlConfig.value">{{controlConfig.nameText}}</el-radio>
        <select-control
            v-model="textValue"
            :readonly="controlConfig.readonly"
            :disabled="controlConfig.disabled"
            :control-config='controlConfig'></select-control>
  </form-control-wrap>
</template>

<script>
import FormControlWrap from '../common/formControlWrap/FormControlWrap';
import GroupSelectExtend from '../common/groupSelectExtend/GroupSelectExtend';
import SelectControl from '../selectControl/SelectControl';

export default {
    extends: GroupSelectExtend,
    components: {
        FormControlWrap,
        SelectControl
    }
};
</script>
<style lang="scss">
.radio-select-control{
//   margin-left: 15px;
  display: inline-block;
//   .el-input{
//       margin-left: 15px;
//   }
}
</style>
