<template>
  <form-control-wrap
    class="radio-empty-control"
    v-if="controlConfig"
    :bypassedWrap="bypassedWrap"
    :control-config="controlConfig"
    :valueData="myValueData"
    @extendClick="extendClick">
    <el-radio
        :readonly="controlConfig.readonly"
        :disabled="controlConfig.disabled"
        v-model="checkValue"
        :label="controlConfig.value">{{controlConfig.nameText}}</el-radio>
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
.radio-empty-control{
//   margin-left: 15px;
  display: inline-block;
}
</style>
