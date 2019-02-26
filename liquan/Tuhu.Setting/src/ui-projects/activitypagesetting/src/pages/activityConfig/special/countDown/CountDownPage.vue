<template>
    <page-box
        @submit="submit"
        class="goodsDefault-pages"
        :title="['模块类型：','特殊模块—倒计时']"
        @refresh="refreshData">
        <form-group :defaultData="defaultData" :formConfig="formList" @formInit="formInit"></form-group>
    </page-box>
</template>

<script>
import PageBox from '../../commons/pageBox/PageBox';
import FormGroup from '../../commons/formGroup/FormGroup';
import TabPage from '../../tabPage/TabPage';
import {getCountDownConfig} from './config/countDownPage.config';
import apis from '../../commons/apis/special/specialApi';

export default {
    extends: TabPage,
    data() {
        return {
            formList: getCountDownConfig(),
            defaultData: null
        };
    },
    mounted() {
        this.getModuleData().subscribe(data => {
            this.defaultData = data.data.CountdownModule || null;
        });
    },
    methods: {
        /**
         * 刷新数据
         */
        refreshData() {
            this.getModuleData().subscribe(data => {
                this.defaultData = data.data.CountdownModule || null;
            });
        },
        formInit(formModel) {
            this.formModel = formModel;
        },
        submit() {
            this.formModel.isSave = true;
            let tagOption = this.tagOption && this.tagOption.data;
            if (!tagOption || !tagOption.ActivityId||!tagOption.ModuleId) {
                return;
            }
            if (!this.$$validMsg(this.formModel)) {
                // this.$emit('submit', this.formModel && this.formModel.value);
                let _value = this.formModel.value;
                _value=Object.assign(_value, {
                    ActivityId: tagOption.ActivityId,
                    ModuleId: tagOption.ModuleId,
                    ModuleTypeCode: tagOption.SecondaryModuleTypeCode
                });
                this.formModel.isSend = true;
                this.$http.post(apis.SaveCountdown, {
                    data: _value
                }).subscribe((res) => {
                    let _res = res&&res.data;
                    let _rMessage = _res&& _res.ResponseMessage;
                    if (this.$filterResponseCode(_res)) {
                        this.$$saveMsg(_rMessage, {type: 'success'});
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
    },
    components: {
        PageBox,
        FormGroup
    }
};
</script>

<style scoped>

</style>
