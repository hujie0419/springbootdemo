<template>
  <form-control-wrap
    class="check-box-group"
    v-if="controlConfig"
    :bypassedWrap="bypassedWrap"
    :form-model="formModel"
    :control-config="controlConfig"
    :valueData="myValueData"
    @extendClick="extendClick">
      <el-checkbox-group class="checkbox-group-wrap" v-if="controlConfig" v-model="myValue">
        <el-checkbox
            v-for="item in controlConfig.list"
            :readonly="item.readonly"
            :disabled="item.disabled"
            :key="item.nameText"
            :label="item.value">{{item.nameText}}</el-checkbox>
      </el-checkbox-group>
  </form-control-wrap>
</template>

<script>
import FormControlWrap from '../common/formControlWrap/FormControlWrap';
import ControlExtend from '../common/controlExtend/ControlExtend';

export default {
    extends: ControlExtend,
    components: {
        FormControlWrap
    },
    data() {
        return {};
    }
};
</script>
<style lang="scss">
.check-box-group{
//   margin-left: 15px;
  display: inline-block;
}
</style>
