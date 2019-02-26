<template>
    <table-box :pagination="false" class="groupTable">
        <el-table
            class="group-table"
            :data="tableData"
            :row-class-name="'table-td-row'"
            :header-row-class-name="'table-th-row'"
            border
            style="width: 100%">
            <el-table-column
                fixed
                prop="SortGroupId"
                label="组号">
                <template slot-scope="scope">
                    <edit-select-input-control
                        :isEditable="!scope.row.SortGroupId"
                        v-model="scope.row.SortGroupId"
                        @textBlur="textBlur(scope.$index, scope.row, 'SortGroupId')"
                        @chooseOption="chooseOption(scope.$index)"></edit-select-input-control>
                </template>
            </el-table-column>
            <el-table-column
                prop="PID"
                label="PID"
                width="140">
                <template slot-scope='scope'>
                    <edit-select-input-control
                        :isEditable="!scope.row.PID"
                        @textBlur="textBlur(scope.$index, scope.row, 'PID')"
                        @chooseOption="chooseOption(scope.$index)"
                        v-model="scope.row.PID"></edit-select-input-control>
                </template>
            </el-table-column>
            <el-table-column
                prop="GroupId"
                label="GroupId"
                width="140">
                <template slot-scope='scope'>
                    <edit-select-input-control
                        :isEditable="!scope.row.GroupId"
                        @textBlur="textBlur(scope.$index, scope.row, 'GroupId')"
                        @chooseOption="chooseOption(scope.$index)"
                        v-model="scope.row.GroupId"
                        :control-config="groupIdControlConfig"></edit-select-input-control>
                </template>
            </el-table-column>
            <el-table-column
                prop="GroupName"
                label="拼团名称">
            </el-table-column>
            <el-table-column
                prop="GroupType"
                label="拼团类型"
                width="80">
            </el-table-column>
            <el-table-column
                prop="Price"
                label="拼团价"
                width="80">
                <template slot-scope="scope">
                    {{scope.row.Price|filter_money('1.0-2')}}
                </template>
            </el-table-column>
            <el-table-column
                label="操作"
                width="80">
                <template slot-scope="scope">
                    <el-button
                        @click.native.prevent="deleteRow(scope.$index, scope.row)"
                        type="text"
                        size="small">
                        删除
                    </el-button>
                    <el-button
                        v-if="scope.row.isLast"
                        @click.native.prevent="addRow(scope.$index, scope.row)"
                        size="small">
                        添加
                    </el-button>
                </template>
            </el-table-column>
        </el-table>
    </table-box>
</template>

<script>
import TableBox from '../../../commons/tableBox/TableBox';
import EditSelectInputControl from '../../../commons/editSelectInputControl/EditSelectInputControl';
import { getGroupId } from '../../../commons/groupBookingId/getGroupId';

export default {
    components: {
        TableBox,
        EditSelectInputControl
    },
    data() {
        return {
            groupIdControlConfig: {
                type: 'select',
                multiple: false,
                list: []
            },
            editable: false
        };
    },
    props: {
        tableData: {
            type: Array
        }
    },
    watch: {
        tableData(nowValue) {
            const length = nowValue.length;
            nowValue.forEach((item, index) => {
                item.isLast = (index === length-1);
            });
            this.$emit('updateTableData', nowValue);
        }
    },
    methods: {
        /**
         * 删除一行
         *
         * @param {number} index 行号
         * @param {object} row 该行数据
         */
        deleteRow(index, row) {
            this.$$confirm('确定【删除】此商品？').then(() => {
                this.tableData.splice(index, 1);
                this.$emit('deleteRow', row);
                this.$$saveMsg('删除成功!', {
                    type: 'success'
                });
            }).catch(() => {
                this.$$saveMsg('已取消删除', {
                    type: 'info'
                });
            });
        },
        /**
         * 新增一行
         *
         * @param {number} index 行号
         * @param {object} row 该行数据
         */
        addRow(index, row) {
            const newRow = {
                isNew: true
            };
            // this.editable = true;
            this.tableData.push(newRow);
        },
        /**
         * 获取groupid的option
         *
         * @param {number} index 行号
         */
        chooseOption(index) {
            getGroupId(this.$http, this.tableData[index].PID).then(res => {
                this.tempRowData = res.tempRowData;
                this.groupIdControlConfig.list = res.listArray;
            }).catch(e => {
                this.$$errorMsg(e.message);
                this.tempRowData = [];
                this.groupIdControlConfig.list = [];
            });
        },
        /**
         * 表格内容失去焦点时触发
         *
         * @param {number} rowIndex 行号
         * @param {object} row 该行数据
         * @param {string} type 修改数据的类型SortGroupId/PID/GroupId
         */
        textBlur(rowIndex, row, type) {
            setTimeout(() => {
                let tableData = this.tableData;
                let tableItem = tableData && tableData[rowIndex];
                if (tableItem) {
                    (tableItem.isNew !== true) && (tableItem.isChange = true);
                    switch (type) {
                        case 'GroupId':
                            this.tempRowData && this.tempRowData.forEach(newItem => {
                                if (newItem.GroupId === row.GroupId) {
                                    this.$set(tableData, rowIndex, Object.assign(tableItem, newItem));
                                }
                            });
                            break;
                        case 'PID':
                            getGroupId(this.$http, row.PID).then(res => {
                                this.tempRowData = res.tempRowData;
                                this.groupIdControlConfig.list = res.listArray;
                                const newRow = this.tempRowData[0];
                                this.$set(tableData, rowIndex, Object.assign(tableItem, newRow));
                            }).catch(e => {
                                this.$$errorMsg(e.message);
                                this.tempRowData = [];
                                this.groupIdControlConfig.list = [];
                                this.$set(tableData, rowIndex, Object.assign(tableItem, {
                                    GroupId: null,
                                    GroupName: null,
                                    GroupType: null,
                                    Price: null
                                }));
                            });
                            break;
                        case 'SortGroupId':
                            break;
                    }
                }
            }, 0);
        }
    }
};
</script>

<style lang="scss">
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";
.groupTable .el-button+.el-button {
    margin: 0;
}

</style>
