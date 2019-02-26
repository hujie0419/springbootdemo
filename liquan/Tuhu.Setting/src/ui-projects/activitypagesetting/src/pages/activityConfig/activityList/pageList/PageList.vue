<template>
    <page-card @topSubmit="toEditPage" :title="'页面列表'" :showTopButton="false" :showBottomButton="false">
        <el-row>
            <el-col :span="24">
                <div class="grid-content bg-purple">
                    <div class="list-form">
                        <el-radio-group v-model="status">
                            <el-radio-button label="1">未开始（{{pageCount.NotAtTheCount || 0}}）</el-radio-button>
                            <el-radio-button label="2">进行中（{{pageCount.OngoingCount || 0}}）</el-radio-button>
                            <el-radio-button label="3">已结束（{{pageCount.HasEndedCount || 0}}）</el-radio-button>
                            <el-radio-button label="4">已删除（{{pageCount.DeletedCount || 0}}）</el-radio-button>
                        </el-radio-group>
                        <el-button type="primary" plain class="right-btn" @click="toEditPage">　新建　</el-button>
                        <activity-default-table :tableContent.sync="tableContent" @refreshPage="getActivityList"></activity-default-table>
                    </div>
                </div>
            </el-col>
        </el-row>
    </page-card>
</template>
<script>
import ActivityDefaultTable from '../activityDefaultTable/ActivityDefaultTable';
import PageCard from '../../commons/pageCard/PageCard';
import apis from '../../commons/apis/activityList/activityListApi.js';

export default {
    data() {
        let pageCount = {
            NotAtTheCount: 0,
            OngoingCount: 0,
            HasEndedCount: 0,
            DeletedCount: 0
        };
        return {
            status: '1',
            statusList: ['NotAtThe', 'Ongoing', 'HasEnded', 'Deleted'],
            pageCount: pageCount,
            tableContent: null
        };
    },
    props: {
        formModelValue: {
            type: Object,
            default: null
        }
    },
    watch: {
        status(nowval) {
            this.getActivityList();
        },
        formModelValue(nowval) {
            this.getActivityList();
        }
    },
    components: {
        ActivityDefaultTable,
        PageCard
    },
    created() {
        this.getActivityList();
    },
    methods: {
        /**
         * 跳转至编辑页
         */
        toEditPage() {
            this.$$tabs.addTab('newActivity', {
                title: '新建活动',
                callBackUpdate: () => {
                    this.getActivityList();
                }
            });
        },
        /**
         * 获取页面列表
         *
         * @param {number} page 页数
         * @returns {Boolean}
         */
        getActivityList(page) {
            page = page || 1;
            const formValue = this.formModelValue;
            const atime = (formValue && formValue.atime) || '';
            const ctime = (formValue && formValue.ctime) || '';
            if (atime && !atime[0] && atime[1]) {
                this.$$errorMsg('请填写开始日期', {type: 'error'});
                return false;
            }
            if (ctime && !ctime[0] && ctime[1]) {
                this.$$errorMsg('请填写开始日期', {type: 'error'});
                return false;
            }
            const body = {
                keyWord: (formValue && formValue.gjz) || '',
                ActivityStartTime: (atime && atime[0]) || '',
                ActivityEndTime: (atime && atime[1]) || '',
                CreateStartTime: (ctime && ctime[0]) || '',
                CreateEndTime: (ctime && ctime[1]) || '',
                CreateBy: (formValue && formValue.cjr) || '',
                ActivityType: (formValue && formValue.h3 && formValue.h3.join(',')) || '',
                ActivityStatus: this.statusList[parseInt(this.status)-1],
                PageIndex: page,
                PageSize: 20
            };
            this.getActivityListApi && this.getActivityListApi.unsubscribe();
            this.xxx = this.$http.post(apis.GetActivityList, {
                apiServer: 'apiServer',
                isLoading: true,
                data: body
            }).subscribe(data => {
                this.getActivityListApi = null;
                // tab上的个数
                let _data = data&& data.data;
                if (!_data) {
                    return;
                }
                _data.DeletedCount = _data.DeletedCunt;
                this.pageCount = {
                    NotAtTheCount: _data.NotAtTheCount,
                    OngoingCount: _data.OngoingCount,
                    HasEndedCount: _data.HasEndedCount,
                    DeletedCount: _data.DeletedCount
                };
                const countVar = this.statusList[parseInt(this.status)-1]+'Count';
                // 配置表格数据
                this.tableContent = {
                    count: _data[countVar],
                    tableData: _data.ActivityInfoList,
                    activityStatus: body.ActivityStatus
                };
            });
        }
    }
};
</script>

<style lang='scss'>
    @import "css/common/_var.scss";
    @import "css/common/_mixin.scss";
    @import "css/common/_iconFont.scss";

   .list-pages {
        .list-form {
            // padding-top: 20px;
            .right-btn {
                float: right;
            }
        }
        .list-form-right-btn {
            position: absolute;
            right: 0px;
            // left: 0;
        }
        .list-box-title{
            position: relative;
            .list-form-right-btn {
                top:-10px;
                right: 0px;
                // left: 0;
            }
        }
        .list-form-right {
            .list-form-right-btn {
                bottom: 20px;
            }
        }
    }
</style>
