<template>
    <div class="query-condition">
        <page-card :title="'查询条件'" @submit="submit" :showBottomButton="false" v-if="formModel">
            <el-row>
                <el-col :span="21">
                    <div class="grid-content bg-purple">
                        <el-row :gutter="10" class="query-condition-row">
                            <el-col :span="12">
                                <div class="list-form">
                                    <form-control :formModel="formModel" :control-config='formConfig[0]' @enter="submit"></form-control>
                                </div>
                            </el-col>
                            <el-col :span="12">
                                <div class="list-form">
                                    <form-control :formModel="formModel" :control-config='formConfig[1]' @enter="submit"></form-control>
                                </div>
                            </el-col>
                        </el-row>
                        <el-row :gutter="10" class="query-condition-row">
                            <el-col :span="12">
                                <div class="list-form">
                                    <form-control :formModel="formModel" :control-config='formConfig[2]'></form-control>
                                </div>
                            </el-col>
                            <el-col :span="12">
                                <div class="list-form">
                                    <form-control :formModel="formModel" :control-config='formConfig[3]'></form-control>
                                </div>
                            </el-col>
                        </el-row>
                        <el-row :gutter="10" class="query-condition-row">
                            <el-col :span="24">
                                <div class="list-form">
                                    <form-control :formModel="formModel" :control-config='formConfig[4]'></form-control>
                                </div>
                            </el-col>
                        </el-row>
                    </div>
                </el-col>
                <el-col :span="3">
                    <div class="grid-content bg-purple-light list-form-right">
                        <el-button class="list-form-right-btn" type="primary" @click="submit">　查询　</el-button>
                    </div>
                </el-col>
            </el-row>
        </page-card>
    </div>
</template>

<script>
import PageCard from '../../commons/pageCard/PageCard';
import FormControl from '../../commons/formList/formControl/FormControl';
import {getGueryControlList} from './config/gueryCondition.config.js';
import apis from '../../commons/apis/activityList/activityListApi.js';

export default {
    components: {
        PageCard,
        FormControl
    },
    data() {
        return {
            formModel: null,
            formConfig: getGueryControlList()
        };
    },
    created() {
        this.formModel = this.$$form.initFormData(this.formConfig);
        this.getActivityType();
    },
    methods: {
        /**
         * 获取活动类型
         */
        getActivityType() {
            this.$http.post(apis.GetDictlist, {
                apiServer: 'apiServer',
                isLoading: true,
                data: {
                    DictType: 'ActivityType'
                }
            }).subscribe(res => {
                let _res = res && res.data;
                let activityTypeList = _res && _res.ActivityTypeList;
                let option = [];
                activityTypeList && activityTypeList.forEach((item) => {
                    option.push({
                        nameText: item.DictName,
                        value: item.DictCode
                    });
                });
                this.$set(this.formConfig[4], 'list', option);
            });
        },
        submit() {
            if (!this.$$validMsg(this.formModel)) {
                this.$emit('submit', this.formModel && this.formModel.value);
            }
        }
    }
};
</script>

<style lang="scss">
.query-condition {
    .form-control-nameText {
        width: 80px;
        text-align: right;
    }
    .query-condition-row {
        margin-top: 15px;
    }
    .el-date-editor.el-input{
        width: 150px;
    }
}
</style>
