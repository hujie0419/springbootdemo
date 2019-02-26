<template>
    <page-box
        class="sysrec"
        :title="['模块类型：','商品—系统推荐']"
        @submit='submit'
        @refresh="refreshData">
        <form-group :defaultData="defaultData" @formInit="formInit" :form-config='formList'></form-group>
    </page-box>
</template>

<script>
import TabPage from '../../tabPage/TabPage';
import FormGroup from '../../commons/formGroup/FormGroup';
import PageBox from '../../commons/pageBox/PageBox';
import { getSysRecConfig } from './config/sysRec.config';
import apis from '../../commons/apis/goods/goodsApi';

export default {
    extends: TabPage,
    components: {
        FormGroup,
        PageBox
    },
    data() {
        return {
            formModel: null,
            formList: getSysRecConfig(),
            defaultData: null
        };
    },
    mounted() {
        this.getSysRec();
    },
    methods: {
        /**
         * 刷新数据
         */
        refreshData() {
            this.getSysRec();
        },
        getSysRec() {
            this.getModuleData().subscribe(data => {
                let pushModule = data.data.PushModule;
                this.defaultData = (pushModule && Object.assign({}, pushModule, {
                    PushRowNumber: {
                        select: pushModule.PushRowNumber + '' === '0' ? '0' : '1',
                        value: pushModule.PushRowNumber || ''
                    }
                })) || null;
            });
        },
        formInit(formModel) {
            this.formModel = formModel;
        },
        submit() {
            this.formModel.isSave = true;
            let tagOption = this.tagOption && this.tagOption.data;
            if (!tagOption || !tagOption.ActivityId ||!tagOption.ModuleId || !tagOption.SecondaryModuleTypeCode) {
                return;
            }
            if (!this.$$validMsg(this.formModel)) {
                // this.$emit('submit', this.formModel && this.formModel.value);
                let _value = this.formModel.value;
                // _value['PushRowNumber'] = (_value.PushRowNumber.select == 2 ? _value.PushRowNumber.value : _value.PushRowNumber);
                _value.PushRowNumber = _value.PushRowNumber.value;
                _value=Object.assign(_value, {
                    ActivityId: tagOption.ActivityId,
                    ModuleId: tagOption.ModuleId,
                    ModuleTypeCode: tagOption.SecondaryModuleTypeCode
                });
                this.formModel.isSend = true;
                this.$http.post(apis.SavePushProductSetting, {
                    data: _value
                }).subscribe((res) => {
                    let _res = res&&res.data;
                    let _rMessage = _res&& _res.ResponseMessage;
                    if (this.$filterResponseCode(_res)) {
                        this.$$saveMsg(_rMessage, {type: 'success'});
                        if (this.tagOption && this.tagOption.callBackUpdate instanceof Function) {
                            this.tagOption.callBackUpdate({tabName: this.tagOption.name, activityid: tagOption.ActivityId});
                        }
                    }
                    // else {
                    //     this.$$saveMsg(res.ResponseMessage, {type: 'error'});
                    // }
                }, () => {}, () => {
                    setTimeout(() => {
                        this.formModel.isSend = false;
                    }, 300);
                });
            }
        }
    }
};
</script>

<style lang="scss">
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";
.sysrec{
    .radio-text-control {
        .el-input{
            width: 50px;
            text-align: center;
            .el-input__inner {
                text-align: center;
                padding: 0 2px;
            }
        }
    }
}
</style>
