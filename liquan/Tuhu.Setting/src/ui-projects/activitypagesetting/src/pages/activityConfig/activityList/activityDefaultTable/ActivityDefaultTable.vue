<template>
    <table-box v-model="pageNum" :pageSize="pageSize" :total="total" @valueChange="changePage">
        <el-table
            :data="tableData"
            :row-class-name="'table-td-row'"
            :header-row-class-name="'table-th-row'"
            border
            style="width: 100%">
            <el-table-column
                fixed
                prop="index"
                label="编号"
                width="60">
            </el-table-column>
            <el-table-column
                prop="ActivityId"
                label="活动页ID"
                width="100">
            </el-table-column>
            <el-table-column
                prop="ActivityTitle"
                label="活动名称">
            </el-table-column>
            <el-table-column
                prop="ActivityTypeName"
                label="类型"
                width="80">
            </el-table-column>
            <el-table-column
                prop="date"
                label="活动时间"
                width="180">
            </el-table-column>
            <el-table-column
                prop="link"
                label="链接">
            </el-table-column>
            <el-table-column
                prop="CreateBy"
                label="创建人"
                width="150">
            </el-table-column>
            <el-table-column
                prop="operation"
                label="操作" width="180">
                <template slot-scope="scope">
                    <!-- <template v-if='!scope.row.isUpdating'> -->
                    <template>
                    <el-button
                        @click.native.prevent="update(scope.$index, scope.row)"
                        type="text"
                        size="small">
                        编辑
                    </el-button>
                    <el-button
                        @click.native.prevent="copyRow(scope.$index, scope.row)"
                        type="text" size="small">
                        复制
                    </el-button>
                    <el-button
                        v-if="activityStatus !== 'Deleted'"
                        @click.native.prevent="deleteData(scope.$index, scope.row)"
                        type="text" size="small">
                        删除
                    </el-button>
                    <el-button
                        @click.native.prevent="showLog(scope.$index, scope.row)"
                        type="text" size="small">
                        日志
                    </el-button>
                  </template>
                  <!-- <template v-else>
                    <el-button @click='save(scope.$index, scope.row)' size='small'>保存</el-button>
                  </template> -->
                </template>
            </el-table-column>
        </el-table>
    </table-box>
</template>

<script>
import TableBox from '../../commons/tableBox/TableBox';
import WindowLogTable from '../../commons/logTable/WindowLogTable';
import apis from '../../commons/apis/activityList/activityListApi';
let activity = 'https://wx.tuhu.cn/vue/NaActivity/pages/home/index?id=';
let url = location.href;
if (url.indexOf('.tuhu.work') > -1 || url.indexOf('172.') > -1 || url.indexOf('localhost') > -1) {
    activity = 'https://wxdev.tuhu.work/vue/vueTest/pages/home/index?_project=NaActivity&id=';
}
export default {
    inject: ['$$Popup'],
    components: {
        TableBox
    },
    data() {
        return {
            pageNum: 1, // 当前页码
            pageSize: 20, // 每页数量
            total: 0, // 总数量
            tableData: [],
            activityStatus: null
        };
    },
    props: {
        tableContent: {
            type: Object
        }
    },
    watch: {
        tableContent(nowVal) {
            this.setData(nowVal);
            // const tableContent = nowVal || {};
            // const tableData = tableContent.tableData;
            // this.total = tableContent.count;
            // if (tableContent.tableData) {
            //     tableData.forEach((element, index) => {
            //         element.index = index + 1;
            //         // element.startTime = new Date(parseInt(element.ActivityStartTime.replace(/\D/g, '')));
            //         // element.endTime = new Date(parseInt(element.ActivityEndTime.replace(/\D/g, '')));
            //         // element.date = element.startTime.toLocaleString() + '至' + element.endTime.toLocaleString();
            //         element.startTime = element.ActivityStartTime;
            //         element.endTime = element.ActivityEndTime;
            //         element.date = element.startTime + '至' + element.endTime;
            //         // 活动前台面链接地址
            //         element.link = activity + element.ActivityId;
            //     });
            //     this.total = tableContent.count;
            //     this.tableData = tableData;
            //     this.activityStatus = tableContent.activityStatus;
            // }
        },
        tableData() {
            this.tableData.forEach((item, index) => {
                item.index = index + 1;
            });
        }
    },
    mounted() {
        this.setData(this.tableContent);
    },
    methods: {
        setData (nowVal) {
            const tableContent = nowVal || {};
            const tableData = tableContent.tableData;
            this.total = tableContent.count;
            if (tableContent.tableData) {
                tableData.forEach((element, index) => {
                    element.index = index + 1;
                    // element.startTime = new Date(parseInt(element.ActivityStartTime.replace(/\D/g, '')));
                    // element.endTime = new Date(parseInt(element.ActivityEndTime.replace(/\D/g, '')));
                    // element.date = element.startTime.toLocaleString() + '至' + element.endTime.toLocaleString();
                    element.startTime = element.ActivityStartTime;
                    element.endTime = element.ActivityEndTime;
                    element.date = element.startTime + '至' + element.endTime;
                    // 活动前台面链接地址
                    element.link = activity + element.ActivityId;
                });
                this.total = tableContent.count;
                this.tableData = tableData;
                this.activityStatus = tableContent.activityStatus;
            }
        },
        update (index, row) {
            // row.isUpdating = true;
            this.toTab('newActivity', row);
        },
        /**
         * 页码变化时调用
         *
         * @param {number} page 页号
         */
        changePage(page) {
            this.$emit('refreshPage', page);
        },
        /**
         * 复制一行
         *
         * @param {number} index 行号
         * @param {object} row 该行数据
         */
        copyRow(index, row) {
            // row.isUpdating = true;
            // let newRow = JSON.parse(JSON.stringify(row));
            // newRow.ActivityId = '';
            // this.tableData.unshift(newRow);
            // this.toTab('newActivity', newRow);
            this.$http.post(apis.ActivityCopy, {
                apiServer: 'apiServer',
                isLoading: true,
                data: {
                    ActivityId: row.ActivityId
                }
            }).subscribe(res => {
                let _res = res && res.data;
                let newRow = JSON.parse(JSON.stringify(row));
                newRow.ActivityId = _res.newActivityId;
                // this.tableData.unshift(newRow);
                this.$emit('refreshPage');
                this.toTab('newActivity', newRow);
            });
        },
        /**
         * 跳转tab
         *
         * @param {string} tabName 行号
         * @param {object} row 行数据
         */
        toTab(tabName, row) {
            this.$$tabs.addTab(tabName, {
                title: row.ActivityTitle,
                pageId: row.ActivityId,
                data: Object.assign({}, row, {
                    activityTitle: row.ActivityTitle,
                    date: [row.startTime, row.endTime],
                    activityType: row.ActivityTypeCode,
                    responsePerson: row.CreateBy
                }),
                callBackUpdate: () => {
                    this.$emit('refreshPage');
                }
            });
        },
        // save (index, row) {
        //     setTimeout(() => {
        //         row.isUpdating = false;
        //     }, 2000);
        // },
        /**
         * 删除一行
         *
         * @param {number} index 行号
         * @param {object} row 该行数据
         */
        deleteData (index, row) {
            this.$$confirm('确定【删除】此活动吗？', {}).then(() => {
                // this.tableData.splice(index, 1);
                this.$http.post(apis.DeleteActivity, {
                    apiServer: 'apiServer',
                    isLoading: true,
                    data: {
                        ActivityId: row.ActivityId
                    }
                }).subscribe(res => {
                    let _res = res && res.data;
                    this.$$saveMsg(_res.ResponseMessage, {
                        type: 'success'
                    });
                    this.$emit('refreshPage', this.pageNum);
                });
            }).catch(() => {
                this.$$saveMsg('已取消删除', {
                    type: 'info'
                });
            });
        },
        showLog(index, row) {
            this.$$Popup.open(WindowLogTable, {
                props: {
                    referId: row.ActivityId
                },
                wrapCla: 'log_popup' // 最外层追加的Class名
                // isShowCloseBtn: true,
                // isClickBgClose: true,
                // alignCla: 'centerMiddle', // ''|'bottom'|'top'|'fullScreen'|'centerMiddle',
                // transitionCls: 't_toBottom' // , // ''|'t_toUp'|'t_scale'|'t_toBottom'|'t_toLeft';
            // showFlag: showFlag
            }).then(data => {
                // console.log(data);
            });
        }
    }
};
</script>

<style lang="scss">
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";

.alignCla{
    .th_content-wrap{
        width:60%;
        .tables-wrap{
            margin-top:0;
        }
    }
}
.has_padding .th_content-wrap{
    width: 80%;
}
</style>
