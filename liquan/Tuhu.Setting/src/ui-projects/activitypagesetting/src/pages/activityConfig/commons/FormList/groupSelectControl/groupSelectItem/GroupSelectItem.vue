<template>
    <div
        class="group-select-item"
        v-if="controlConfig">
            <radio-select-control
                v-if='controlConfig.type === "radioSelect"'
                :form-model="formModel"
                :control-config="controlConfig"
                v-model="myValue"
                @extendClick="extendClick">
            </radio-select-control>
            <radio-text-control
                v-else-if='controlConfig.type === "radioText"'
                :form-model="formModel"
                :control-config="controlConfig"
                v-model="myValue"
                @extendClick="extendClick">
            </radio-text-control>
            <radio-empty-control
                v-else-if='controlConfig.type === "radioEmpty"'
                :form-model="formModel"
                :control-config="controlConfig"
                v-model="myValue"
                @extendClick="extendClick">
            </radio-empty-control>
            <check-box-text-control
                v-else-if='controlConfig.type === "checkBoxText"'
                :form-model="formModel"
                :control-config="controlConfig"
                v-model="myValue"
                @extendClick="extendClick">
            </check-box-text-control>
            <check-box-select-control
                v-else-if='controlConfig.type === "checkBoxSelect"'
                :form-model="formModel"
                :control-config="controlConfig"
                v-model="myValue"
                @extendClick="extendClick">
            </check-box-select-control>
            <check-box-empty-control
                v-else-if='controlConfig.type === "checkBoxEmpty"'
                :form-model="formModel"
                :control-config="controlConfig"
                v-model="myValue"
                @extendClick="extendClick">
            </check-box-empty-control>
    </div>
</template>

<script>
import FormControlWrap from '../../common/formControlWrap/FormControlWrap';
import ControlExtend from '../../common/controlExtend/ControlExtend';
import RadioTextControl from '../../radioTextControl/RadioTextControl';
import RadioSelectControl from '../../radioSelectControl/RadioSelectControl';
import RadioEmptyControl from '../../radioEmptyControl/RadioEmptyControl';
import CheckBoxTextControl from '../../checkBoxTextControl/CheckBoxTextControl';
import CheckBoxSelectControl from '../../checkBoxSelectControl/CheckBoxSelectControl';
import CheckBoxEmptyControl from '../../checkBoxEmptyControl/CheckBoxEmptyControl';

export default {
    extends: ControlExtend,
    components: {
        FormControlWrap,
        RadioTextControl,
        RadioSelectControl,
        RadioEmptyControl,
        CheckBoxTextControl,
        CheckBoxSelectControl,
        CheckBoxEmptyControl
    }

};
</script>

<style lang="scss">
.group-select-item {
    display: inline-block;
    .form-extend-filters {
        .form-control-leftPadding{
            margin-left: 0;
        }
    }
    .form-control-small {
        width: auto;
        .el-input{
            width: 150px;
        }
    }
}
.group-select-item + .group-select-item{
    margin-left: 15px;
}
</style>
