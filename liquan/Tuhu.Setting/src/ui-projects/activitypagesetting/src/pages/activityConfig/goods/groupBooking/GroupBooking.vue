<template>
    <page-box
        :title="['模块类型：','商品—拼团']"
        @submit='submit' class="group-booking-pages"
        @refresh="refreshData">
        <form-group :defaultData="defaultData" :formConfig="formList" @formInit="formInit"></form-group>
        <el-row>
            <el-col :span="1">
                <div style="height: 20px;"></div>
            </el-col>
            <el-col :span="22">
                <group-table :tableData="tableContent" @deleteRow="deleteRow" @updateTableData="updateTableData"></group-table>
            </el-col>
        </el-row>
    </page-box>
</template>

<script>
import TabPage from '../../tabPage/TabPage';
import FormGroup from '../../commons/formGroup/FormGroup';
import GroupTable from './groupTable/GroupTable';
import PageBox from '../../commons/pageBox/PageBox';
import apis from '../../commons/apis/groupBooking/groupBookingApi.js';
import { groupBookingFormList } from './config/groupBooking.config';

export default {
    extends: TabPage,
    components: {
        FormGroup,
        GroupTable,
        PageBox
    },
    data() {
        return {
            formModel: null,
            defaultData: null,
            formList: groupBookingFormList(),
            tableContent: null,
            deleteRows: []
        };
    },
    created() {
        this.getGroupData();
    },
    methods: {
        /**
         * 刷新数据
         */
        refreshData() {
            this.getGroupData();
        },
        formInit(formModel) {
            this.formModel = formModel;
        },
        /**
         * 获取已配置数据
         */
        getGroupData() {
            let tagOption = this.tagOption && this.tagOption.data;
            if (!tagOption || !tagOption.ActivityId || !tagOption.ModuleId) {
                this.$$errorMsg('获取页面信息失败', {type: 'error'});
                return;
            }
            this.$http.post(apis.GetGroupProducts, {
                apiServer: 'apiServer',
                isLoading: true,
                data: {
                    ActivityId: tagOption.ActivityId,
                    ModuleId: tagOption.ModuleId
                }
            }).subscribe(data => {
                let _data = data && data.data;
                let _columNunber = _data && _data.FightColumNumber;
                let _gropData = _data && _data.GroupProducts;
                this.defaultData = {
                    colNum: _columNunber + '' === '0' ? '1': _columNunber + ''
                };
                if (_gropData.length < 1) {
                    _gropData = [{
                        isNew: true
                    }];
                }
                this.tableContent = _gropData;
            });
        },
        /**
         * 删除一行
         *
         * @param {object} row 该行数据
         */
        deleteRow(row) {
            if (!row.isNew) {
                this.deleteRows.push(row);
            }
        },
        /**
         * 表格数据更新
         *
         * @param {object} dataFromTable 表格数据
         */
        updateTableData(dataFromTable) {
            this.dataFromTable = dataFromTable;
        },
        submit() {
            let tagOption = this.tagOption && this.tagOption.data;
            this.formModel.isSave = true;
            if (!tagOption || !tagOption.ActivityId || !tagOption.ModuleId || !tagOption.SecondaryModuleTypeCode) {
                this.$$errorMsg('获取页面信息失败', {type: 'error'});
                return;
            }
            if (!this.$$validMsg(this.formModel)) {
                this.$emit('submit', this.formModel && this.formModel.value);
                let data = {
                    ActivityId: tagOption.ActivityId,
                    ModuleId: tagOption.ModuleId,
                    ModuleTypeCode: tagOption.SecondaryModuleTypeCode,
                    FightColumNumber: parseInt(this.formModel.value.colNum),
                    GroupProducts: []
                };
                if (this.deleteRows) {
                    this.deleteRows.forEach(item => {
                        item.OperationType = 'Delete';
                        data.GroupProducts.push(item);
                    });
                }
                let emptyFlag = false;
                this.dataFromTable.forEach(item => {
                    if (!item.PID || !item.GroupId || (item.isChange && !item.SortGroupId)) {
                        this.$$errorMsg('数据不完整', {type: 'error'});
                        emptyFlag = true;
                    }
                    if (item.isNew === true) {
                        item.OperationType = 'Add';
                        item.PKID = '0';
                        data.GroupProducts.push(item);
                    }
                    if (item.isChange === true) {
                        item.OperationType = 'Edit';
                        data.GroupProducts.push(item);
                    }
                });
                if (emptyFlag) {
                    return false;
                }
                // if (!data.GroupProducts.length) {
                //     this.$$saveMsg('暂无可提交数据', {type: 'warning'});
                //     return false;
                // }
                this.formModel.isSend = true;
                this.$http.post(apis.SaveGroupProducts, {
                    apiServer: 'apiServer',
                    isLoading: true,
                    data: data
                }).subscribe(res => {
                    let _res = res&&res.data;
                    let _rMessage = _res&& _res.ResponseMessage;
                    if (this.$filterResponseCode(_res)) {
                        this.$$saveMsg(_rMessage, {type: 'success'});
                        this.deleteRows = [];
                        this.dataFromTable.forEach(item => {
                            item.isNew && (item.isNew = false);
                            item.isChange && (item.isChange = false);
                        });
                        this.tagOption.callBackUpdate(tagOption.ActivityId);
                    }
                    this.getGroupData();
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

.group-booking-pages {
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
}
</style>
