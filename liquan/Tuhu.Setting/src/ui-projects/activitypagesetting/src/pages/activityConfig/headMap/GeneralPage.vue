<template>
    <div>
      <page-box
        :title='title'
        @submit='submit'
        @refresh="refreshData">
            <form-group @formInit="formInit" :form-config='formConfig' :defaultData="defaultData"
                @extendClick='updataExtendClick'></form-group>
        </page-box>
    </div>
</template>

<script>
import PageBox from '../commons/pageBox/PageBox';
import FormGroup from '../commons/formGroup/FormGroup';
import * as apiConfig from '../commons/apis/headMap/generalPageapi.js';
import { formConfig } from './config/GeneralPage.config.js';
import TabPage from '../tabPage/TabPage';
export default {
    extends: TabPage,
    data() {
        return {
            title: [
                '模块类型:',
                '头图—通用活动页'
            ],
            formConfig: formConfig(),
            defaultData: null
        };
    },
    mounted() {
        this.getModuleData().subscribe(data => {
            this.defaultData = (data && data.data && data.data.GeneralModule) || null;
        });
    },
    methods: {
        /**
         * 刷新数据
         */
        refreshData() {
            this.getModuleData().subscribe(data => {
                this.defaultData = (data && data.data && data.data.GeneralModule) || null;
            });
        },
        updataExtendClick (data) {
        },
        formInit(formModel) {
            this.formModel = formModel;
        },
        submit() {
            this.formModel.isSave = true;
            let tagOption = this.tagOption && this.tagOption.data;
            if (!tagOption || !tagOption.ActivityId || !tagOption.ModuleId) {
                return;
            }
            if (!this.$$validMsg(this.formModel)) {
                // const getData={
                //     ActivityId: tagOption.ActivityId,
                //     ModuleId: tagOption.ModuleId,
                //     ModuleTypeCode: this.formModel.value.ModuleTypeCode,
                //     GeneralImgUrl: this.formModel.value.GeneralImgUrl,
                //     GeneralDynamic: this.formModel.value.GeneralDynamic,
                //     GeneralTitle: this.formModel.value.GeneralTitle,
                //     GeneralCopywriting: this.formModel.value.GeneralCopywriting
                // };
                this.formModel.isSend = true;
                this.$http.post(apiConfig.SaveGeneralFigure, {
                    apiServer: 'apiServer',
                    isLoading: true,
                    data: Object.assign({}, this.formModel.value, {
                        ActivityId: tagOption.ActivityId,
                        ModuleId: tagOption.ModuleId,
                        ModuleTypeCode: tagOption.SecondaryModuleTypeCode
                    })
                }).subscribe(res => {
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
