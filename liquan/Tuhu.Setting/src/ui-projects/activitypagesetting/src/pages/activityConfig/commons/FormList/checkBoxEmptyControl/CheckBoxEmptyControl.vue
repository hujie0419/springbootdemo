<template>
  <form-control-wrap
    class="check-box-text-control"
    v-if="controlConfig"
    :bypassedWrap="bypassedWrap"
    :form-model="formModel"
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
  </form-control-wrap>
</template>

<script>
import FormControlWrap from '../common/formControlWrap/FormControlWrap';
import GroupSelectCheckBoxExtend from '../common/groupSelectExtend/GroupSelectCheckBoxExtend';

export default {
    extends: GroupSelectCheckBoxExtend,
    components: {
        FormControlWrap
    }
};
</script>
<style lang="scss">
.check-box-text-control{
//   margin-left: 15px;
  display: inline-block;
}
</style>
