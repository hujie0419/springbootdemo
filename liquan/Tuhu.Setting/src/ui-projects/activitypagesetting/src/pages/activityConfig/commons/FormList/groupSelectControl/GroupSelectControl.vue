<template>
<!-- <div class="group-select-control">
    <group-selectitem
        :form-model="formModel"
        v-model="myValue"
        :control-config="item"
        v-for="(item, index) in controlConfig.list" :key="index">
    </group-selectitem>
</div> -->
<form-control-wrap
    class="group-select-control"
    v-if="controlConfig && controlConfig.list.length"
    :form-model="formModel"
    :control-config="controlConfig"
    :valueData="myValueData"
    @extendClick="extendClick">
    <group-selectitem
        :form-model="formModel"
        v-model="myValue"
        :control-config="item"
        @extendClick="extendClick"
        v-for="(item, index) in controlConfig.list" :key="index">
    </group-selectitem>
</form-control-wrap>
</template>

<script>
import FormControlWrap from '../common/formControlWrap/FormControlWrap';
import ControlExtend from '../common/controlExtend/ControlExtend';
import GroupSelectitem from './groupSelectItem/GroupSelectItem';

export default {
    extends: ControlExtend,
    components: {
        FormControlWrap,
        GroupSelectitem
    }
};
</script>
<style lang="scss">
.group-select-control{
//   margin-left: 15px;
    display: inline-block;
    &.form-control-wraps{
        margin-left: 15px;
        .form-control-leftPadding{
            margin-left: 0;
        }
    }
  .radio-input-group{
      .form-control-cont{
          margin: 0;
      }
  }
}
</style>
