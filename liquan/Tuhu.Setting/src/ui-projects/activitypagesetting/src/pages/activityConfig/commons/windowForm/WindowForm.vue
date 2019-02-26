<template>
    <window-box :title="title" @cancel="cancel" @submit="submit">
        <div class="window-forms">
            <template>
                <form-group v-if="$$form" @valueChange="valueChange" :formConfig="formList" @formInit="formInit"></form-group>
            </template>
        </div>
    </window-box>
</template>

<script>
import FormGroup from '../formGroup/FormGroup';
import WindowBox from '../../../../components/Layout/windowBox//WindowBox';
export default {
    props: {
        onsetChange: {
            type: Function
        },
        onFormInit: {
            type: Function
        },
        valueCheck: {
            type: Function
        }
    },
    components: {
        FormGroup,
        WindowBox
    },
    data() {
        return {
            formList: [],
            title: '',
            formModel: null
        };
    },
    methods: {
        valueChange(con) {
            let _that = this;
            if (this.onsetChange instanceof Function) {
                let list = this.onsetChange(con, _that.formModel, _that.formList);
                if (list instanceof Array) {
                    this.formList = list;
                }
            }
        },
        formInit(formModel) {
            let _that = this;
            _that.formModel = formModel;
            if (_that.onFormInit instanceof Function) {
                let list = _that.onFormInit(formModel, _that.formList);
                if (list instanceof Array) {
                    _that.formList = list;
                }
            }

            // if (formModel.value.FloatingUpsType) {
            //     let con = {};
            //     con.controlConfig = _that.formList[2];
            //     con.value = formModel.value.FloatingUpsType;
            //     _that.valueChange(con);
            // }
        },
        submit() {
            if (!this.$$validMsg(this.formModel)) {
                if (this.valueCheck instanceof Function) {
                    let valueChecked = this.valueCheck(this.formModel.value);
                    if (valueChecked) {
                        this.$emit('confirmChange', this.formModel.value);
                    }
                } else if (!this.valueCheck) {
                    this.$emit('confirmChange', this.formModel.value);
                }
            }
        },
        cancel() {
            this.$emit('cancelChange');
        }
    }
};
</script>

<style lang="scss" scoped>
.window-forms {
    position: relative;
    /deep/ {
        .el-radio__label{
            padding-left:0;
        }
        .el-radio+.el-radio{
            margin-left:20px;
        }
        .el-col-3{
            width: 25%;
        }
        .form{
            .form-control-leftPadding{
                margin-left: 5px;
            }
            .control-items{
                width: auto;
                margin: 0 5px 0 2px;
            }
            .control-items{
                width:288px;
            }
        }
        .input-large .control-items{
            width: 270px;
        }
        .pztype .control-items{
            width: 135px;
        }
    }
}
</style>
<style lang="scss">
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";

.windwoPop-select{
    &.el-select-dropdown{
        z-index: 1002;
        min-width: 135px;
    }
    &.el-color-dropdown{
        z-index: 1002;
    }
}
.ths_popup {
    .th_content {
        font-size: $defaultFontSize + 1;
    }
    &.modifyFit {
        .th_content-wrap {
            width: 640px;
        }
    }
    &.alignCla {
        .th_content-wrap {
            width: 520px;
        }
    }
    &.form_popup {
        z-index: 999;
        .th_popup-bg {
            background: rgba(0,0,0,0.3);
        }
    }
}
.colorFit{
    .el-input{
        width:150px;
    }
}
</style>
