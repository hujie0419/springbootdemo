<template>
  <form-control-wrap
    class="date-picker-container"
    v-if="controlConfig"
    :form-model="formModel"
    :bypassedWrap="bypassedWrap"
    :control-config="controlConfig"
    :valueData="myValueData"
    @extendClick="extendClick">
        <!-- type="daterange" -->
    <el-date-picker
        v-model="myValue"
        @blur="setIsBlur"
        :class="[hasBlur && 'isBlur']"
        :type="controlConfig.type"
        :value-format="controlConfig.formatDate"
        :readonly="controlConfig.readonly"
        :disabled="controlConfig.disabled"
        :range-separator="controlConfig.rangeSeparator || ' 到 '"
        :start-placeholder="controlConfig.startPlaceholder || controlConfig.placeholder || '开始日期'"
        :end-placeholder="controlConfig.endPlaceholder || controlConfig.placeholder || '结束日期'"
        :default-time="controlConfig.defaultValue">
    </el-date-picker>
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
    computed: {
        // dateType() {
        //     let res = 'date';
        //     // year/month/date/dates/ week/datetime/datetimerange/daterange
        //     switch (this.controlConfig && this.controlConfig.type + '') {
        //         case 'datetime':
        //             res = 'datetime';
        //             break;

        //         default:
        //             break;
        //     }
        // }
    }
};
</script>

<style lang='scss'>

.date-picker-container {
  display: inline-block;
  .el-date-editor{
      .el-range-separator{
          width: 24px;
      }
  }
}
</style>
