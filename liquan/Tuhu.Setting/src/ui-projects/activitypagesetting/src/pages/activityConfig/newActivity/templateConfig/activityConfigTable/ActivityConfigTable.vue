<template>
    <table-box :pagination="false" class="acs_activity-config-table">
        <el-table
            :data="tableData"
            :row-class-name="'table-td-row'"
            :header-row-class-name="'table-th-row'"
            border
            style="width: 100%">
            <el-table-column
                fixed
                prop="GroupNo"
                label="组号" width="180">
                <template slot-scope="scope">
                    <edit-select-input-control
                        @textBlur="textBlur(scope.row)"
                        v-model="scope.row._GroupNo"></edit-select-input-control>
                </template>
            </el-table-column>
            <el-table-column
                prop="ModuleType"
                label="模块类型">
            </el-table-column>
            <el-table-column
                prop="ImgUrl"
                label="图片">
                <template slot-scope="scope" v-if='scope.row.ImgUrl'>
                  <div class='table-img'>
                    <img :src='scope.row.ImgUrl' alt='' />
                  </div>
                </template>
            </el-table-column>
            <el-table-column
                prop="NumberLines"
                label="每行数量" width="50">
            </el-table-column>
            <el-table-column
                prop="NavigationName"
                label="导航">
                <template slot-scope="navscope">
                    <template>
                        <div class="addNav" @click="openAddNav(navscope.$index, navscope.row)">{{navscope.row.NavigationName?navscope.row.NavigationName:'+'}}</div>
                    </template>
                </template>
            </el-table-column>
            <el-table-column
                prop="operation"
                label="操作" width="200">
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
        <div style="padding-top:20px;"></div>
    </table-box>
</template>

<script>
import TableBox from '../../../commons/tableBox/TableBox';
import WindowLogTable from '../../../commons/logTable/WindowLogTable';
import WindowForm from '../../../commons/windowForm/WindowForm';
import { getChildList, addItemData } from '../../config/popUp.config';
import * as newActivityApi from '../../../commons/apis/newActivity/newActivityApi';
import EditSelectInputControl from '../../../commons/editSelectInputControl/EditSelectInputControl';

export default {
    inject: ['$$Popup'],
    components: {
        TableBox,
        EditSelectInputControl
    },
    props: {
        Activityid: {
            type: String,
            default: ''
        },
        // goNewTab: {
        //     type: Function
        // },
        // isRefresh: {
        //     type: Boolean
        // },
        resetIsRefresh: {
            type: Function
        },
        activityInfo: {
            default: null
        }
    },
    data() {
        return {
            tableData: [],
            formList: [],
            title: '',
            ModuleId: ''
        };
    },
    watch: {
        // isRefresh(newVal, oldVal) {
        //     if (newVal) {
        //         this.initData();
        //         this.$emit('resetIsRefresh', false);
        //     }
        // },
        activityInfo() {
            this.setData();
        }
    },
    created() {
        if (this.Activityid) {
            this.setData();
            // this.initData();
        }
    },
    methods: {
        /**
         * 设置默认数据
         */
        setData() {
            let activityInfo = this.activityInfo;
            if (activityInfo && activityInfo.ActivityModulesList) {
                let tableData = activityInfo.ActivityModulesList;
                this.tableData = tableData.map((item) => {
                    item._GroupNo = item.GroupNo;
                    item.PrimaryModuleTypeName = item.PrimaryModuleTypeName?item.PrimaryModuleTypeName : '';
                    item.SecondaryModuleTypeName = item.SecondaryModuleTypeName ? item.SecondaryModuleTypeName :'';
                    item.ModuleType = item.PrimaryModuleTypeName + '-' + item.SecondaryModuleTypeName;
                    if (item.SecondaryModuleTypeCode === 'IMAGELINKCOLUMNS') { // 一行1-4列，显示第一张图片以及连接类型信息
                        item.ImageLinkColumnType && (item.ModuleType = item.ModuleType + '-' + item.ImageLinkColumnType);
                    }
                    return Object.assign({}, item);
                });
            }
        },
        // initData() {
        //     this.getActivityInfo(this.Activityid).subscribe((res) => {
        //         // this.activityInfo = res;
        //         if (res && res.data && res.data.ActivityModulesList) {
        //             this.tableData = res.data.ActivityModulesList;
        //             this.tableData.map((item) => {
        //                 item.PrimaryModuleTypeName = item.PrimaryModuleTypeName?item.PrimaryModuleTypeName : '';
        //                 item.SecondaryModuleTypeName = item.SecondaryModuleTypeName ? item.SecondaryModuleTypeName :'';
        //                 item.ModuleType = item.PrimaryModuleTypeName + '-' + item.SecondaryModuleTypeName;
        //             });
        //         }
        //     });
        // },
        textBlur(row) {
            if (row && row._GroupNo !== row.GroupNo) {
                this.$http.post(newActivityApi.EDITMODULESORT, {
                    apiServer: 'apiServer',
                    isLoading: true,
                    isErrorData: false,
                    isErrorMsg: true,
                    data: {
                        ActivityId: this.Activityid || '',
                        ModuleId: row.ModuleId,
                        GroupNo: row._GroupNo
                    }
                }).subscribe((res) => {
                    if (this.$filterResponseCode(res && res.data)) {
                        this.$emit('updateTable', res);
                    } else {
                        this.setData();
                    }
                });
            }
        },
        getActivityInfo(activityid) {
            return this.$http.post(newActivityApi.GETACTIVITYINFO, {
                apiServer: 'apiServer',
                isLoading: true,
                data: {
                    ActivityId: activityid || ''
                }
            });
        },
        update (index, row) {
            // row.isUpdating = true;
            this.$emit('toTabs', row);
        },
        // save (index, row) {
        //     setTimeout(() => {
        //         row.isUpdating = false;
        //     }, 2000);
        // },
        openPop(callback) {
            let _that = this;
            this.$$Popup.open(WindowForm, {
                props: {
                    // onsetChange: () => {},
                    onFormInit: callback
                },
                data: {
                    title: _that.title,
                    formList: _that.formList
                },
                wrapCla: (this.bigWin && 'modifyFit form_popup') || 'alignCla form_popup', // 最外层追加的Class名
                // isShowCloseBtn: alignCla ==='fullScreen' ? true : false,
                alignCla: 'centerMiddle', // ''|'bottom'|'top'|'fullScreen'|'centerMiddle',
                transitionCls: 't_scale' // , // ''|'t_toUp'|'t_scale'|'t_toBottom'|'t_toLeft';
                // showFlag: showFlag
            }).then((d) => {
                if (d.type === 'cancel') return;
                this.$http.post(newActivityApi.EDITNAVIGATIONINFO, {
                    apiServer: 'apiServer',
                    isLoading: true,
                    data: {
                        ActivityId: this.Activityid || '',
                        ModuleId: this.ModuleId || '',
                        NavigationName: d.NavigationName || '',
                        NavigationDescription: d.NavigationDescription || ''
                    }
                }).subscribe((res) => {
                    let _res = res && res.data;
                    if (this.$filterResponseCode(_res)) {
                        this.$emit('resetIsRefresh', true);
                        this.$message({
                            type: 'success',
                            message: '保存成功!'
                        });
                    } else {
                        this.$message({
                            type: 'info',
                            message: '保存失败!'
                        });
                    }
                });
            });
        },
        openAddNav (index, row) {
            this.ModuleId = row.ModuleId;
            this.getActivityInfo(this.Activityid).subscribe(res => {
                this.formList = getChildList(2);
                this.title = '添加导航';
                this.openPop((formModel) => {
                    let _res = res&&res.data;
                    let _modulelist =_res&&_res.ActivityModulesList;
                    _modulelist.forEach((item) => {
                        if (item.ModuleId === row.ModuleId) {
                            formModel.setValue({
                                NavigationName: item.NavigationName,
                                NavigationDescription: item.NavigationDescription
                            });
                        }
                    });
                });
            });
        },
        deleteData (index, row) {
            this.$confirm('确定【删除】此模块吗？', '提示', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'warning'
            }).then(() => {
                this.tableData.splice(index, 1);
                this.$http.post(newActivityApi.DELETEMODULE, {
                    apiServer: 'apiServer',
                    isLoading: true,
                    isErrorData: false,
                    data: {
                        ActivityId: this.Activityid,
                        ModuleId: row.ModuleId
                    }
                }).subscribe((res) => {
                    let _res = res&&res.data;
                    if (this.$filterResponseCode(_res)) {
                        this.$message({
                            type: 'success',
                            message: '删除成功!'
                        });
                    } else {
                        this.$message({
                            type: 'info',
                            message: '删除失败!'
                        });
                    }
                });
            }).catch(() => {
                this.$message({
                    type: 'info',
                    message: '已取消删除'
                });
            });
        },
        showLog(index, row) {
            console.log(row);
            this.$$Popup.open(WindowLogTable, {
                props: {
                    referId: row.ModuleId
                },
                wrapCla: 'log_popup' // 最外层追加的Class名
                // isShowCloseBtn: true,
                // isClickBgClose: true,
                // alignCla: 'centerMiddle', // ''|'bottom'|'top'|'fullScreen'|'centerMiddle',
                // transitionCls: 't_scale' // , // ''|'t_toUp'|'t_scale'|'t_toBottom'|'t_toLeft';
            // showFlag: showFlag
            }).then(data => {
            });
        }
        // handleClick(index, flag) {
        //     switch (flag) {
        //         case 'look': // 查看

        //             break;
        //         case 'edit': // 编辑

        //             break;

        //         default:
        //             break;
        //     }
        // }
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
.addNav {
    // min-width: 20px;
    min-height: 30px;
}
.acs_activity-config-table {
    .table-img {
        text-align: center;
        img {
            width: auto;
            max-height: 80px;
            max-width: 100%
        }
    }
}
</style>
