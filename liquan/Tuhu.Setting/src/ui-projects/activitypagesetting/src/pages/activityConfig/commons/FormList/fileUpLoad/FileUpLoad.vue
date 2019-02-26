<template>
  <form-control-wrap
    v-if="controlConfig"
    v-model="myValue"
    :bypassedWrap="bypassedWrap"
    :form-model="formModel"
    :control-config="controlConfig"
    :itemFliter="descFliter"
    :valueData="myValueData"
    @extendClick="extendClick">
      <el-input
        class='control-items'
        @blur="setIsBlur"
        :class="[hasBlur && 'isBlur']"
        :placeholder='controlConfig.placeholder'
        :clearable="!controlConfig.readonly && !controlConfig.disabled"
        :readonly="controlConfig.readonly"
        :disabled="controlConfig.disabled"
        v-model="myValue"></el-input>
  </form-control-wrap>
</template>

<script>
import ControlExtend from '../common/controlExtend/ControlExtend';
import FormControlWrap from '../common/formControlWrap/FormControlWrap';

export default {
    extends: ControlExtend,
    components: {
        FormControlWrap
    },
    methods: {
        descFliter(item) {
            return item;
        }
        // fileChange(evt) { // 设置路径值
        //     this.myValue = evt;
        //     // this.formModel.get(controlConfig.controlName).setValue(evt);
        // }
    }
};
</script>

<style scoped>

</style>
