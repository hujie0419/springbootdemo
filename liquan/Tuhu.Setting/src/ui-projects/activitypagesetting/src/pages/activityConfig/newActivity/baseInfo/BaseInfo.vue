<template>
    <div class="base-info">
        <page-card :title="'基本信息'" @submit="submit" v-if="formModel" @topSubmit="$emit('refresh')"><!-- :showTopButton="true" topButton="刷新" -->
            <div class="grid-content bg-purple base-info-box">
                <el-row class="base-info-row" :gutter="20">
                    <el-col :span="9">
                        <form-control
                            :form-model="formModel"
                            :control-config="formConfig[0]">
                        </form-control>
                    </el-col>
                    <el-col :span="15">
                        <el-row class="time-control">
                            <form-control
                                :form-model="formModel"
                                :control-config="formConfig[1]">
                            </form-control>
                        </el-row>
                    </el-col>
                </el-row>
                <el-row class="base-info-row" :gutter="20">
                    <el-col :span="9">
                        <form-control
                            :form-model="formModel"
                            :control-config="formConfig[2]">
                        </form-control>
                    </el-col>
                    <el-col :span="15">
                        <form-control
                            :form-model="formModel"
                            :control-config="formConfig[3]">
                        </form-control>
                    </el-col>
                </el-row>
            </div>
        </page-card>
    </div>
</template>

<script>
import PageCard from '../../commons/pageCard/PageCard';
import FormControl from '../../commons/formList/formControl/FormControl';
import * as newActivityApi from '../../commons/apis/newActivity/newActivityApi';
import { getFormConfig } from '../config/baseInfo.config';
import { mapGetters } from 'vuex';
export default {
    props: {
        tagOption: {
            type: Object
        },
        activityInfo: {
            default: null
        },
        Activityid: {
            default: null
        }
    },
    data() {
        return {
            formModel: null,
            // defaultData: { //接口返回默认值
            //     activityTitle: '活动页标题',
            //     activityType: '1'
            // },
            formConfig: getFormConfig()
        };
    },
    watch: {
        activityInfo() {
            this.setData();
        }
    },
    components: {
        PageCard,
        FormControl
    },
    computed: {
        // ...Vuex.mapState({
        //     userInfo: 'userInfo'
        // })
        ...mapGetters([
            'userInfo'
            // 'sidebar',
            // 'name',
            // 'avatar',
            // 'introduction'
        ])
    },
    created() {
        this.formInit();
    },
    mounted() {
        // this.setResponsePerson();
        this.getActivityType();
    },
    methods: {
        /**
         * 设置负责人
         */
        setResponsePerson() {
            let formModel = this.formModel;
            let _responsePerson = formModel.value['responsePerson'];
            if (!_responsePerson && this.userInfo) {
                formModel && formModel.setValue({
                    responsePerson: this.userInfo.name
                });
            }
        },
        /**
         * 获取活动类型
         */
        getActivityType() {
            this.$http.post(newActivityApi.GETDICLIST, {
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
                this.$set(this.formConfig[2], 'list', option);
            });
        },
        formInit() {
            if (this.formConfig && this.formConfig.length > 0) {
                let form = this.$$form;
                // 查询列表传过来的数据
                // let defaultTime = this.defaultData.datetimerange;
                // defaultTime = [new Date(2000, 10, 10, 10, 10), new Date(2000, 10, 11, 10, 10)]
                this.formModel = form.initFormData(this.formConfig, this.tagOption && this.tagOption.data);
                this.setData();
            }
        },
        setData() {
            let activityInfo = this.activityInfo;
            let infoData = activityInfo && activityInfo.BasicActivityInfo;
            if (infoData && this.formModel) {
                if (infoData.Head) {
                    infoData.responsePerson = infoData.Head;
                    delete infoData.Head;
                }
                this.$set(infoData, 'date', [infoData.ActivityStartTime, infoData.ActivityEndTime]);
                this.formModel.setValue(infoData);
            }
            this.setResponsePerson();
        },
        submit() {
            this.formModel.isSave = true;
            let result = this.formModel.value;
            if (!this.$$validMsg(this.formModel)) {
                this.$emit('submit', this.formModel && this.formModel.value);
                this.formModel.isSend = true;
                // 基本信息接口
                this.$http.post(newActivityApi.SAVEACTIVYTINFO, {
                    apiServer: 'apiServer',
                    isLoading: true,
                    data: {
                        ActivityId: this.Activityid || (this.tagOption && this.tagOption.pageId) || '',
                        ActivityTitle: result.ActivityTitle,
                        ActivityStartTime: result.date[0],
                        ActivityEndTime: result.date[1],
                        ActivityTypeCode: result.ActivityTypeCode,
                        Head: result.responsePerson
                    }
                }).subscribe((res) => {
                    let _res = res&&res.data;
                    let _ractivityid = _res&& _res.ActivityId;
                    if (this.$filterResponseCode(_res)) {
                        this.$$saveMsg('保存成功!', {type: 'success'});
                        this.$emit('setActivityId', _ractivityid);
                        if (this.tagOption.callBackUpdate instanceof Function) {
                            this.tagOption.callBackUpdate();
                        }
                    }
                    // else {
                    //     this.$$saveMsg('保存失败', {type: 'error'});
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
.base-info {
    .base-info-box {
        padding-bottom: 50px;
    }
    .form-control-nameText{
        display: inline-block;
        width: 100px;
        margin: 0 5px;
        text-align: right;
        line-height: 40px;
    }
    .base-info-row {
        margin-top: 15px;
    }
    .time-control{
        .is-required{
            &::before{
                content: "*";
                color: $stressColor;
                margin-right: 4px;
            }
        }
    }
}
</style>
