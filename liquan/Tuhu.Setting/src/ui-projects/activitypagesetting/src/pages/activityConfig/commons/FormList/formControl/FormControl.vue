<template>
    <el-row
        class="form-control"
        v-if="formModel && filterList && controlConfig">
        <el-col
            class="form-control-nameText"
            :class="[(!requiredRight && isRequired && 'is-required') || '']"
            :span="3">
            {{controlConfig.nameText && controlConfig.nameText + '：' || ''}}
        </el-col>
        <div class="control-filter-wrap">
            <template v-for="(item, index) in filterList">
                <form-control-filter
                v-if="formModel.get(item.controlName)"
                :class="[formModel.get(item.controlName).isValid && 'is-valid-err', formModel.isSave && 'is-save']"
                :form-model="formModel"
                :itemFliter="itemFliter"
                :control-config="item"
                v-model="formModel.get(item.controlName).value"
                @valueChange="emitValueChange({
                    controlConfig: item,
                    value: $event
                })"
                @selectFocus="$emit('selectFocus', {formConfig: controlConfig, formModel: formModel})"
                @enter="$emit('enter')"
                @extendClick="updataExtendClick($event, formModel.get(item.controlName).value)"
                :key="index">
                </form-control-filter>
            </template>
        </div>
        <div
            class="ext-fix"
            :class="[requiredRight && isRequired && 'is-required-ext' || '']"
            v-if="requiredRight">
        </div>
    </el-row>
</template>

<script>
import ControlExtend from '../common/controlExtend/ControlExtend';
import FormControlFilter from '../common/formControlFilter/FormControlFilter';

export default {
    extends: ControlExtend,
    data() {
        return {
            isFormControl: false
        };
    },
    components: {
        FormControlFilter
    },
    methods: {
        /**
         * 更新extendClick
         * @param {Object} evt 事件对象
         * @param {any} value 选中的值
         */
        updataExtendClick(evt, value) {
            if (evt instanceof Object) {
                evt.value = value;
            }
            this.extendClick(evt);
        }
    },
    computed: {
        filterList() {
            let list = this.controlConfig;
            if (list) {
                if (typeof list.formControl !== 'undefined') {
                    list = list.formControl;
                    this.isFormControl = true;
                } else {
                    list = [this.controlConfig];
                    this.isFormControl = false;
                }
            }
            return list;
        }
    }
};
</script>

<style lang='scss'>
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";
// .el-row {
//     display: flex;
//     align-items: center;
// }
.form-control{
    line-height: 40px;
    .is-required {
        &:before {
            content: "*";
            color: $stressColor;
            margin-right: 4px;
        }
    }
    .is-required-ext {
        &:after {
            content: "*";
            color: $stressColor;
            margin-left: 4px;
        }
    }
    .ext-fix {
        display: inline-block;
    }
    .control-filter-wrap{
        display: inline-block;
    }
    .is-valid-err {
        &.is-save,.isBlur{
            .el-input__inner {
                border-color: $colorRed;
            }
            .radio-group-wrap,.checkbox-group-wrap {
                border: solid 1px $colorRed;
                padding: 5px;
            }
        }
    }
}
</style>
