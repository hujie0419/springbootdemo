<template>
  <form-control-wrap
    class="check-box-text-control"
    v-if="controlConfig"
    :bypassedWrap="bypassedWrap"
    :control-config="controlConfig"
    :valueData="myValueData"
    @extendClick="extendClick">
        <el-checkbox
            v-model="isCheckValue"
            :disabled="controlConfig.disabled"
            :readonly="controlConfig.readonly"
            :label="controlConfig.value">
            {{controlConfig.nameText}}
        </el-checkbox>
        <select-control
            v-model="textValue"
            :bypassedWrap="true"
            :control-config='controlConfig'></select-control>
  </form-control-wrap>
</template>


<script>
import FormControlWrap from '../common/formControlWrap/FormControlWrap';
import GroupSelectCheckBoxExtend from '../common/groupSelectExtend/GroupSelectCheckBoxExtend';
import SelectControl from '../selectControl/SelectControl';

export default {
    extends: GroupSelectCheckBoxExtend,
    components: {
        FormControlWrap,
        SelectControl
    }
};
</script>
<style lang="scss">
.check-box-text-control{
//   margin-left: 15px;
  display: inline-block;
  .control-items{
      margin-right: 0;
  }
}
</style>
