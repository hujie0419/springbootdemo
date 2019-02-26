<template>
    <page-box
        class="seckill-pages"
        :title="['模块类型：','商品—秒杀']"
        @submit='submit'
        @refresh="refreshData">
        <form-group :defaultData="defData" :formConfig="formList" @formInit="formInit"></form-group>
        <el-row>
            <el-col :span="3">
                <div style="height: 20px;"></div>
            </el-col>
            <el-col :span="18">
                <seckill-table :formModel="formModel" :spikelist="SpikeSessionList"></seckill-table>
            </el-col>
        </el-row>
    </page-box>
</template>

<script>
import TabPage from '../../tabPage/TabPage';
import FormGroup from '../../commons/formGroup/FormGroup';
import SeckillTable from './seckillTable/SeckillTable';
import PageBox from '../../commons/pageBox/PageBox';
import goodsApi from '../../commons/apis/goods/goodsApi';
import {seckillFormList} from './config/seckill.config';

export default {
    extends: TabPage,
    components: {
        FormGroup,
        SeckillTable,
        PageBox
    },
    data() {
        return {
            formList: seckillFormList(),
            SpikeSessionList: [],
            formModel: null,
            defData: null,
            ActivityId: '',
            ModuleId: ''
        };
    },
    mounted() {
        let data = this.tagOption && this.tagOption.data;
        this.ActivityId = data.ActivityId;
        this.ModuleId = data.ModuleId;
        this.GetSpikeSessions();
    },
    methods: {
        /**
         * 刷新数据
         */
        refreshData() {
            this.GetSpikeSessions();
        },
        // handleSelectionChange(val) {
        //     let result = val.length>0?val:'';
        //     this.formModel.get('SpikeSessionList').setValue(result);
        // },
        GetSpikeSessions() {
            this.$http.post(goodsApi.GetSpikeSessions, {
                apiServer: 'apiServer',
                isLoading: true,
                data: {
                    ActivityId: this.ActivityId,
                    ModuleId: this.ModuleId
                }
            }).subscribe((res) => {
                let selectList = [];
                let _data = res && res.data;
                let _sessionList = _data && _data.SpikeSessionList;
                if (_data) {
                    if (_sessionList) {
                        this.SpikeSessionList =_sessionList || [];
                        this.SpikeSessionList.forEach(item => {
                            if (item && item.IsThere) {
                                selectList.push({
                                    select: item.SpilkeActivityId,
                                    value: item
                                });
                            }
                        });
                    }

                    this.defData = {
                        SpikeColumnNumber: res.data.SpikeColumnNumber+'' === '2' ? '2' : '1',
                        SpikeRowNumber: {
                            select: res.data.SpikeRowNumber+'' !== '0' ? '1' : '0',
                            value: res.data.SpikeRowNumber
                        },
                        SpikeSessionList: selectList
                    };
                }
            });
        },
        formInit(formModel) {
            this.formModel = formModel;
        },
        submit() {
            this.formModel.isSave = true;
            let result = this.formModel.value;
            let tagOption = this.tagOption && this.tagOption.data;
            if (!tagOption || !tagOption.ActivityId) {
                return;
            }
            if (!this.$$validMsg(this.formModel)) {
                // this.$emit('submit', this.formModel && this.formModel.value);
                result.SpikeSessionList = result.SpikeSessionList.map(item => {
                    if (item && item.value) {
                        item.value.PKID = item.value.PKID || '0';
                    }
                    return item.value;
                });
                if (result.SpikeSessionList.length < 1) {
                    this.$$errorMsg('未选择数据');
                    return false;
                }
                result.SpikeRowNumber = parseInt(result.SpikeRowNumber.value, 10) || 0;
                result.SpikeColumnNumber = parseInt(result.SpikeColumnNumber);
                this.formModel.isSend = true;
                this.$http.post(goodsApi.SaveSpikeSessions, {
                    apiServer: 'apiServer',
                    isLoading: true,
                    data: Object.assign({}, result, {
                        ActivityId: tagOption.ActivityId,
                        ModuleId: tagOption.ModuleId,
                        ModuleTypeCode: tagOption.SecondaryModuleTypeCode
                    })
                }).subscribe((res) => {
                    let _res = res&&res.data;
                    let _rMessage = _res&& _res.ResponseMessage;
                    if (this.$filterResponseCode(_res)) {
                        if (this.tagOption && this.tagOption.callBackUpdate instanceof Function) {
                            this.tagOption && this.tagOption.callBackUpdate({tabName: 'goodsSeckill', activityid: this.Activityid});
                        }
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
    }
};
</script>

<style lang="scss">
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";

.seckill-pages {
    .form {
        padding-bottom: 0;
    }
    .goods-form {
        padding-top: 20px;
    }
    .goods-form-right-btn {
        position: absolute;
        right: 0px;
        // left: 0;
    }
    .goods-form-right {
        .goods-form-right-btn {
            bottom: 30px;
        }
    }
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
