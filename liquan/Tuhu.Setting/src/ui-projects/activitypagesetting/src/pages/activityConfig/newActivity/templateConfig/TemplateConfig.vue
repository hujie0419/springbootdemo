<template>
    <page-card class="template-config" @submit="submit" :title="'页面列表'" :keepHidePageContent="!Activityid" @topSubmit="$emit('refresh')" @changeShow="changeShow" :showPageContent="!!Activityid"><!-- :showTopButton="true" topButton="刷新" -->
        <div class="grid-content bg-purple">
            <form-group @valueChange="valueChange" @extendClick="columnModify" :form-config='formConfig' @formInit="formInit"></form-group>
            <div class="list-form">
                <div class="el-col-config el-col el-col-3 el-col-title">
                    模块配置：
                </div>
                <el-button @click="openWindowSet" type="primary" plain>+添加悬浮窗</el-button>
                <el-button @click="addNavSet" type="primary" plain>+添加导航</el-button>
                <el-button @click="addModule" type="primary" plain>+添加模块</el-button>
                <activity-config-table
                    @resetIsRefresh="resetIsRefresh"
                    @updateTable="updateTable"
                    @toTabs="toTabs"
                    :activityInfo="activityInfo"
                    :Activityid="Activityid">
                </activity-config-table>
            </div>
        </div>
    </page-card>
</template>
<script>
import PageCard from '../../commons/pageCard/PageCard';
import FormGroup from '../../commons/formGroup/FormGroup';
import FormControl from '../../commons/formList/formControl/FormControl';
import ActivityConfigTable from './activityConfigTable/ActivityConfigTable';
import * as newActivityApi from '../../commons/apis/newActivity/newActivityApi';
import PopupConfig from './PopupConfig';
import { templateConfigFormconfig } from '../config/templateConfig.config';

export default {
    extends: PopupConfig,
    // model: {
    //     prop: 'misRefresh',
    //     event: 'isRefreshChange'
    // },
    props: {
        // misRefresh: {
        //     type: Boolean
        // },
        activityInfo: {

        }
    },
    components: {
        PageCard,
        FormGroup,
        FormControl,
        ActivityConfigTable
    },
    data() {
        return {
            formModel: null,
            formConfig: templateConfigFormconfig(),
            formList: [],
            // poppic: [],
            title: '',
            moduleData: null,
            ModuleId: ''
            // isRefresh: false
            // activityInfo: null
        };
    },
    watch: {
        // misRefresh(nowVal) {
        //     this.resetIsRefresh(nowVal);
        // },
        activityInfo() {
            this.setData();
        }
    },
    mounted() {
        // this.initData();
    },
    methods: {
        updateTable(evt) {
            this.$emit('updateTable', evt);
        },
        toTabs(evt) {
            this.$emit('toTabs', evt);
        },
        changeShow() {
            if (!this.Activityid) {
                this.$$errorMsg('请优先填写并保存基本信息');
            }
        },
        resetIsRefresh(bl) {
            this.$emit('resetIsRefresh', bl);
            // this.$emit('isRefreshChange', bl);
        },
        // initData() {
        //     this.getActivityInfo(this.Activityid).subscribe(data => {
        //         this.activityInfo = data && data.data;
        //         this.setData();
        //     });
        // },
        formInit(formModel) {
            this.formModel=formModel;
            this.setData();
        },
        /**
         * 设置表单反显数据
         */
        setData() {
            let activityInfo = this.activityInfo;
            if (activityInfo && activityInfo.BasicPageSettingInfo) {
                let data = activityInfo.BasicPageSettingInfo;
                let groupSelect = setColumnMap(data, isCheck => {
                    if (isCheck) {
                        this.lock = isCheck;
                    }
                }) || [];

                this.formModel.setValue(data);
                if (groupSelect && groupSelect.length > 0) {
                    setTimeout(() => {
                        groupSelect.forEach((item, index) => {
                            this.$set(this.formModel.get('groupSelect').value, index, item);
                        });
                    }, 20);
                }
            }
        },
        /**
         * 表单项数据发生改变
         * @param {Object} con 表单项数据
         */
        valueChange(con) {
            let result = false;
            if (con.controlConfig.controlName === 'groupSelect') {
                let temp = con.value && con.value.length > 0;
                if (temp) {
                    // let item = con.value[con.value.length - 1];
                    // if (item.select === 'column') {
                    //     result = true;
                    // }
                    con.value.map(item => {
                        if (item.select === 'column') {
                            result = true;
                        }
                    });
                }
                if (result && !this.upItem && !this.lock) {
                    this.openColumn();
                } else if (!result && this.lock) {
                    this.lock = false;
                }
                this.upItem = result;
            }
        },

        submit() {
            if (this.Activityid === '' && this.Activityid.length===0) {
                this.$$errorMsg('请优先填写并保存基本信息');
                return;
            }
            let result = this.formModel.value;
            if (!this.$$validMsg(this.formModel)) {
                this.$emit('submit', this.formModel && this.formModel.value);
                // 基本信息接口
                let IsAdaptationUps = false;
                let IsAdaptationColumn = false;
                let AdaptationUpsCar = '';
                if (result.groupSelect.length>0) {
                    result.groupSelect.forEach((item) => {
                        if (item.select === 'car') {
                            IsAdaptationUps = true;
                            AdaptationUpsCar = item.value;
                        }
                        if (item.select === 'column') {
                            IsAdaptationColumn = true;
                        }
                    });
                }
                this.formModel.isSend = true;
                this.$http.post(newActivityApi.SAVEACTIVITYCONFIG, {
                    apiServer: 'apiServer',
                    isLoading: true,
                    data: Object.assign({}, result, {
                        ActivityId: this.Activityid,
                        AdaptationUpsCar: AdaptationUpsCar,
                        IsAdaptationUps: IsAdaptationUps,
                        IsAdaptationColumn: IsAdaptationColumn
                    })
                }).subscribe((res) => {
                    let _res = res && res.data;
                    if (this.$filterResponseCode(_res)) {
                        this.$$saveMsg('保存成功!', {type: 'success'});
                    }
                }, () => {}, () => {
                    setTimeout(() => {
                        this.formModel.isSend = false;
                    }, 300);
                });
            }
        }
    }
};
/**
 * 设置修改顶部适配栏
 * @param {Object} data 数据
 * @param {Function} cb 函数
 * @return {Object|undefined}
 */
function setColumnMap(data, cb) {
    let res;
    if (data && data.IsAdaptationUps) {
        res = res || [];
        res.push({
            select: 'car',
            value: data.AdaptationUpsCar
        });
    }
    if (data && data.IsAdaptationColumn) {
        res = res || [];
        res.push({
            select: 'column',
            value: data.IsAdaptationColumn
        });
        if (cb instanceof Function) {
            cb(true);
        }
    }
    return res;
}
</script>
<style lang='scss'>
.template-config {
    .el-col-title{
        display: inline-block;
        margin-right:15px;
        text-align: right;
        line-height: 40px;
    }
    .input-container {
        display: inline-flex;
    }
}
</style>
