<template>
  <div v-if="formModel && controlConfig" class='form-control-filter'>
    <div
        :form-model='formModel'
        :control-config='controlConfig'
        v-model="myValue"
        @extendClick="extendClick"
        @selectFocus="$emit('selectFocus')"
        @enter="$emit('enter')"
        :is="controlFactory(controlConfig.type)">
    </div>
    <!-- <template v-if='controlConfig.type === "select"'>
      <select-control
        :form-model='formModel'
        :control-config='controlConfig'
        v-model="myValue"
        @extendClick="extendClick"></select-control>
    </template> -->
    <!-- <template v-else-if='controlConfig.type === "fileText"'>
      <file-up-load
        :form-model='formModel'
        :control-config='controlConfig'
        v-model="myValue"
        @extendClick="extendClick"></file-up-load>
    </template> -->
    <!-- <template v-else-if='controlConfig.type === "checkBox"'>
      <check-box-control
        :form-model='formModel'
        :control-config='controlConfig'
        v-model="myValue"
        @extendClick="extendClick">
      </check-box-control>
    </template> -->
    <!-- <template v-else-if='controlConfig.type === "radio"'>
      <radio-control
        :form-model='formModel'
        :control-config='controlConfig'
        v-model="myValue"
        @extendClick="extendClick">
      </radio-control>
    </template> -->
    <!-- <template v-else-if='controlConfig.type === "groupSelect"'>
      <group-select-control
        :form-model='formModel'
        :control-config='controlConfig'
        v-model="myValue"
        @extendClick="extendClick">
      </group-select-control>
    </template> -->
    <!-- <template v-else-if="controlConfig.type === 'radioSelect' ||
        controlConfig.type === 'radioText' ||
        controlConfig.type === 'radioEmpty' ||
        controlConfig.type === 'checkBoxSelect' ||
        controlConfig.type === 'checkBoxText' ||
        controlConfig.type === 'checkBoxEmpty'">
        <group-select-item
            :form-model='formModel'
            :control-config='controlConfig'
            v-model="myValue"
            @extendClick="extendClick">
        </group-select-item>
    </template> -->
    <!-- <template v-else-if='controlConfig.type === "year" ||
        controlConfig.type === "month" ||
        controlConfig.type === "date" ||
        controlConfig.type === "dates" ||
        controlConfig.type === "week" ||
        controlConfig.type === "datetime" ||
        controlConfig.type === "datetimerange" ||
        controlConfig.type === "daterange"'>
        <date-picker-control
            :form-model='formModel'
            :control-config='controlConfig'
            v-model="myValue"
            @extendClick="extendClick">
        </date-picker-control>
    </template> -->
    <!-- <template v-else-if='controlConfig.type === "useryear" ||
        controlConfig.type === "usermonth" ||
        controlConfig.type === "userdate" ||
        controlConfig.type === "userdates" ||
        controlConfig.type === "userweek" ||
        controlConfig.type === "userdatetime" ||
        controlConfig.type === "userdatetimerange" ||
        controlConfig.type === "userdaterange"'>
      <date-picker-range-control
        :form-model='formModel'
        :control-config='controlConfig'
        v-model="myValue"
        @extendClick="extendClick"></date-picker-range-control>
    </template> -->
    <!-- <template v-else>
      <input-control
        :form-model='formModel'
        v-model="myValue"
        @extendClick="extendClick"
        :control-config='controlConfig'></input-control>
    </template> -->
  </div>
</template>

<script>
import ControlExtend from '../controlExtend/ControlExtend';
import SelectControl from '../../selectControl/SelectControl';
import InputControl from '../../inputControl/InputControl';
import CheckBoxControl from '../../checkBoxControl/CheckBoxControl';
import FileUpLoad from '../../fileUpLoad/FileUpLoad';
import RadioControl from '../../radioControl/RadioControl';
import GroupSelectControl from '../../groupSelectControl/GroupSelectControl';
import GroupSelectItem from '../../groupSelectControl/groupSelectItem/GroupSelectItem';
import DatePickerControl from '../../datePickerControl/DatePickerControl';
import DatePickerRangeControl from '../../datePickerControl/DatePickerRangeControl';

export default {
    extends: ControlExtend,
    // props: {
    //   formModel: {},
    //   control: {
    //     type: Object
    //   }
    // },
    components: {
        SelectControl,
        InputControl,
        CheckBoxControl,
        FileUpLoad,
        RadioControl,
        GroupSelectControl,
        GroupSelectItem,
        DatePickerControl,
        DatePickerRangeControl
    },
    methods: {
        controlFactory(typeKey) {
            let res = 'input-control';
            switch (typeKey) {
                case 'select':
                    res = 'select-control';
                    break;
                case 'fileText':
                    res = 'file-up-load';
                    break;
                case 'checkBox':
                    res = 'check-box-control';
                    break;
                case 'radio':
                    res = 'radio-control';
                    break;
                case 'groupSelect':
                    res = 'group-select-control';
                    break;
                case 'radioSelect':
                case 'radioText':
                case 'radioEmpty':
                case 'checkBoxSelect':
                case 'checkBoxText':
                case 'checkBoxEmpty':
                    res = 'group-select-item';
                    break;
                case 'year':
                case 'month':
                case 'date':
                case 'dates':
                case 'week':
                case 'datetime':
                case 'datetimerange':
                case 'daterange':
                    res = 'date-picker-control';
                    break;
                case 'useryear':
                case 'usermonth':
                case 'userdate':
                case 'userdates':
                case 'userweek':
                case 'userdatetime':
                case 'userdatetimerange':
                case 'userdaterange':
                    res = 'date-picker-range-control';
                    break;
                default:
                    break;
            }
            return res;
        }
    }
};
</script>

<style lang='scss'>
.form-control-filter {
  display: inline-block;
  // width: 100%;
}
</style>
