<template>
    <div class='form' v-if="formModel">
        <div class='form-list' v-for="(controlItem, index) in formConfig" :key='index'>
          <form-control
            :form-model="formModel"
            :itemFliter="itemFliter"
            :control-config="controlItem"
            @valueChange="emitValueChange"
            @selectFocus="$emit('selectFocus', $event)"
            @extendClick="extendClick">
          </form-control>
        </div>
    </div>
</template>

<script>

import ControlExtend from './common/controlExtend/ControlExtend';
import FormControl from './formControl/FormControl';

export default {
    extends: ControlExtend,
    props: {
        formConfig: {
            type: Array
        }
    },
    components: {
        FormControl
    }
};
</script>

<style lang='scss'>
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";

.form {
    background: $colorf;
    margin-top: 20px;
    &:first-child {
      margin-top: 0;
    }

    .form-list + .form-list {
        margin-top: 15px;
    }
    .control-items{
      width: 288px;
    }

    .form-control-nameText {
        text-align: right;
        min-height: 20px;
    }

    .form-control-leftPadding {
        margin-left: 15px;
        .form-control-leftPadding {
            margin-left:0;
        }
        .form-control-wraps.radio-group {
            &:first-child{
                margin-left:0;
            }
        }
    }

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
}
</style>
