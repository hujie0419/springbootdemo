<template>
  <form-control-wrap
    class="input-container"
    v-if="controlConfig"
    v-model="myValue"
    :form-model="formModel"
    :bypassedWrap="bypassedWrap"
    :control-config="controlConfig"
    :valueData="myValueData"
    @extendClick="extendClick"
    @valueChange="emitValueChange">
      <el-input
        class="control-items"
        @blur="setIsBlur"
        @keyup.enter.native="$emit('enter')"
        :class="[hasBlur && 'isBlur']"
        :type='controlConfig.type'
        :placeholder='controlConfig.placeholder'
        :clearable="(!controlConfig.readonly && !controlConfig.disabled)"
        :readonly="controlConfig.readonly"
        :disabled="controlConfig.disabled"
        v-model="myValue"></el-input>
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
    methods: {}
};
</script>

<style lang='scss' scoped>
.input-container {
  display: inline-block;
}
</style>
