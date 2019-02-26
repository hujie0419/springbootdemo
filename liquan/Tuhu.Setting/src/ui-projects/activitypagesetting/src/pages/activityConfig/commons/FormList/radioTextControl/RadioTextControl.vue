<template>
  <form-control-wrap
    class="radio-text-control"
    v-if="controlConfig"
    :bypassedWrap="bypassedWrap"
    :control-config="controlConfig"
    :valueData="myValueData"
    @extendClick="extendClick">
      <el-radio
        v-model="checkValue"
        :readonly="controlConfig.readonly"
        :disabled="controlConfig.disabled"
        :label="controlConfig.value">
            {{controlConfig.nameText}}
        </el-radio>
      <el-input
        class="form-control-leftPadding"
        v-model="textValue"
        :readonly="controlConfig.readonly"
        :disabled="controlConfig.disabled"
        :placeholder="controlConfig.placeholder">
      </el-input>
  </form-control-wrap>
</template>

<script>
import FormControlWrap from '../common/formControlWrap/FormControlWrap';

import GroupSelectExtend from '../common/groupSelectExtend/GroupSelectExtend';

export default {
    extends: GroupSelectExtend,
    components: {
        FormControlWrap
    }
};
</script>
<style lang="scss">
.radio-text-control{
//   margin-left: 15px;
  display: inline-block;
}
</style>
