<template>
    <table-box class="myTable" v-model="pageNum" :pageSize="pageSize" :total="total" @valueChange="changePage">
        <div style="max-height:500px;overflow:auto">
            <el-table
                :data="tableData"
                :row-class-name="'table-td-row'"
                :header-row-class-name="'table-th-row'"
                border
                row-key="PKID"
                :expand-row-keys="expands"
                @row-click="rowClick"
                @expand-change="open"
                >
                <el-table-column
                    width="1"
                    label="箭头"
                    type="expand">
                    <template slot-scope="scope">
                        <el-table
                            :data="scope.row.LogDetailList"
                            :row-class-name="'table-td-row'"
                            :header-row-class-name="'table-th-row'"
                            border
                            style="width: 100%">
                            <el-table-column
                                prop="OperationLogType"
                                label="操作内容">
                            </el-table-column>
                            <el-table-column
                                prop="OldValue"
                                label="操作前">
                            </el-table-column>
                            <el-table-column
                                prop="NewValue"
                                label="操作后">
                            </el-table-column>
                        </el-table>
                    </template>
                </el-table-column>
                <el-table-column
                    label="#"
                    width="50">
                    <template slot-scope="scope">
                        <!-- <span>{{scope.row.PKID}}</span> -->
                        <div class="el-table__expand-icon" v-if="scope.row.DetailCount" :class="{active:expands.indexOf(scope.row.PKID) > -1}"><i class="el-icon el-icon-arrow-right"></i></div>
                    </template>
                </el-table-column>
                <el-table-column
                    prop="OperationLogDescription"
                    label="操作类型">
                </el-table-column>
                <el-table-column
                    prop="CreateUserName"
                    label="操作人">
                </el-table-column>
                <el-table-column
                    prop="CreateDateTime"
                    label="操作时间">
                </el-table-column>
            </el-table>
        </div>
    </table-box>
</template>

<script>
import TableBox from '../../commons/tableBox/TableBox';
import apis from '../../commons/apis/commons/commonApi';
export default {
    props: {
        referId: {
            type: String
        }
    },
    components: {
        TableBox
    },
    data() {
        return {
            pageNum: 1, // 当前页码
            pageSize: 10, // 每页数量
            total: 1, // 总数量
            tableData: [],
            // 要展开的行，数值的元素是row的key值
            expands: []
        };
    },
    created() {
        // let data = {
        //     objectId: '',
        //     source: ''
        // };
        // this.$http.post(apis.GetActivitySettingLog, {
        //     apiServer: 'apiServer',
        //     isLoading: true,
        //     data: data
        // }).subscribe(data => {
        // });
    },
    mounted() {
        this.getTableData(this.referId);
    },
    methods: {
        getTableData(id) {
            this.$http.get(apis.GetOperationLogList, {
                apiServer: 'apiServer',
                isLoading: true,
                params: {
                    referId: id,
                    pageIndex: this.pageNum,
                    pageSize: this.pageSize
                }
            }).subscribe(res => {
                let _res = res && res.data;
                this.tableData = _res && _res.LogPageModel;
                this.total = _res && _res.TotalCount;
            });
        },
        open(row) {
            if (!row.LogDetailList.length) {
                this.$http.get(apis.GetOperationLogDetailList, {
                    apiServer: 'apiServer',
                    isLoading: true,
                    params: {
                        FPKID: row.PKID
                    }
                }).subscribe(res => {
                    let _res = res && res.data;
                    row.LogDetailList = _res && _res.LogPageModel;
                });
            }
        },
        // 在<table>里，我们已经设置row的key值设置为每行数据id：row-key="id"
        rowClick(row) {
            if (row.DetailCount) {
                if (this.expands.indexOf(row.PKID) < 0) {
                    this.expands = [];
                    this.open(row);
                    this.expands.push(row.PKID);
                } else {
                    this.expands.splice(this.expands.indexOf(row.PKID), 1);
                }
            }
        },
        /**
         * 页码变化时调用
         *
         * @param {number} page 页号
         */
        changePage(page) {
            this.pageNum = page;
            this.getTableData(this.referId); // '6DC14F47'
            this.$emit('refreshPage', page);
        }
    }
};
</script>

<style lang="scss">
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";
.myTable{
    .table-td-row{
        td {
            .el-table__expand-icon.active {
                transform: rotate(90deg);
                transition: all 0.5s;
            }
            &:first-child{
                .el-table__expand-icon {
                    display: none; 
                }
            }
        } 
    }
}
</style>
