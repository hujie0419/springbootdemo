<template>
    <page-box
        class="maintenanceVehicle-pages"
        @submit="submit"
        :title="['模块类型：','特殊模块—保养分车型']"
        @refresh="refreshData">
        <el-row
            class="maintenanceVehicle-id"
            :gutter="20">
            <el-col :span='20'>
                <form-control :formModel="formModel1" :control-config='formList[0]'></form-control>
            </el-col>
            <el-col :span='4'>
                <div class='grid-content'>

                </div>
            </el-col>
        </el-row>
        <form-group-panel
            form-type='array'
            :addData="addData()"
            :isDefaultAddItem="true"
            :formConfig="formConfig"
            :defaultData="defaultData || [addData()]"
            :get-config='getPageConfig'
            @formInit="formInit"
            :deleteColums="deleteColums"
            :maxPanel='3'
            @formConfigUpdate='formConfigUpdate'></form-group-panel>
    </page-box>
</template>

<script>
import PageBox from '../../commons/pageBox/PageBox';
import FormGroupPanel from '../../commons/formGroupPanel/FormGroupPanel';
import FormControl from '../../commons/formList/formControl/FormControl';
import TabPage from '../../tabPage/TabPage';
import apis from '../../commons/apis/special/specialApi';
import {getPageConfig, getCheckBox} from './config/MaintenanceVehicle.config';

export default {
    extends: TabPage,
    data() {
        let _that = this;
        return {
            defaultData: null,
            formModel: null,
            deleteColums: [],
            formModel1: null,
            formList: getCheckBox(),
            formConfig: [ getPageConfig(this.$http, 1) ],
            getPageConfig: (index) => {
                return getPageConfig(this.$http, index);
            },
            addData() {
                return Object.assign({
                    OperationType: 'Add',
                    PKID: '0'
                }, ((_that.tagOption && _that.tagOption.data[0])||{}));
            }
        };
    },
    created () {
        this.getMaintenanceVehicle();
    },
    mounted() {
        this.formModel1 = this.$$form.initFormData(this.formList);
    },
    methods: {
        /**
         * 刷新数据
         */
        refreshData() {
            this.getMaintenanceVehicle();
        },
        formInit(formModel) {
            this.formModel = formModel;
        },
        formConfigUpdate (formConfig) {
            this.formConfig = formConfig;
        },
        getMaintenanceVehicle() {
            let tagOption = this.tagOption && this.tagOption.data;
            if (!tagOption || !tagOption.ActivityId || !tagOption.ModuleId) {
                return;
            }
            this.$http.post(apis.GetMaintenanceVehicle, {
                apiServer: 'apiServer',
                isLoading: true,
                data: {
                    ActivityId: tagOption.ActivityId,
                    ModuleId: tagOption.ModuleId
                }
            }).subscribe(data => {
                let _data = data&&data.data;
                let _MaintenancePricingList=_data&&_data.MaintenancePricingList ? _data.MaintenancePricingList : [];
                this.defaultData = (_MaintenancePricingList.map(item => { // 设置 可编辑
                    item.OperationType = 'Edit';
                    return item;
                })) || null;
            });
        },
        submit() {
            this.formModel.isSave = true;
            let tagOption = this.tagOption && this.tagOption.data;
            if (!tagOption || !tagOption.ActivityId ||!tagOption.ModuleId) {
                return;
            }
            if (!this.$$validMsg(this.formModel)) {
                let _value = this.formModel.value;
                // 设置提交数据
                const newValue = {
                    ActivityId: tagOption.ActivityId,
                    ModuleId: tagOption.ModuleId,
                    ModuleTypeCode: tagOption.SecondaryModuleTypeCode,
                    MaintenanceType: 'MaintenancePricing',
                    MaintenancePricingList: _value
                };
                // 设置删项的数据
                this.deleteColums.forEach(item => {
                    if (item.PKID + '' !== '0') {
                        item.OperationType = 'Delete';
                        newValue.MaintenancePricingList.push(item);
                    }
                });
                this.formModel.isSend = true;
                this.$http.post(apis.SaveMaintenanceVehicle, {
                    data: newValue
                }).subscribe(res => {
                    let _res = res&&res.data;
                    let _rMessage = _res&& _res.ResponseMessage;
                    if (this.$filterResponseCode(_res)) {
                        this.$$saveMsg(_rMessage, {type: 'success'});
                        this.deleteColums = [];
                        this.getMaintenanceVehicle();
                        if (this.tagOption && this.tagOption.callBackUpdate instanceof Function) {
                            this.tagOption.callBackUpdate({tabName: this.tagOption.name, activityid: tagOption.ActivityId});
                        }
                    }
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
        FormGroupPanel,
        FormControl
    }
};
</script>

<style lang='scss'>
.maintenanceVehicle-pages {
    .maintenanceVehicle-id {
        .form-control-nameText {
            text-align: right;
        }
        .control-filter-wrap {
            padding-left: 20px;
        }
    }
}
</style>
