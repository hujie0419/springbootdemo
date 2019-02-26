<template>
  <div class="board-page">
      <div class="board-header">
        <div class="checkbox">
            <el-checkbox-group v-model="checkList">
                <el-checkbox v-for="option in category" :key="option.value" :label="option.value" :style="{background: option.bgColor}">{{option.nameText}}</el-checkbox>
            </el-checkbox-group>
        </div>
        <div class="search-bar">
            <div class="condition">
                起始日期:
                <el-date-picker
                    :clearable="false"
                    :editable="false"
                    v-model="condition.startTime"
                    type="date"
                    placeholder="选择日期">
                </el-date-picker>
                <el-input
                    clearable
                    placeholder="活动名称"
                    v-model="condition.activityName">
                </el-input>
                <el-input
                    clearable
                    placeholder="创建人"
                    v-model="condition.creator">
                </el-input>
                <el-input
                    clearable
                    placeholder="负责人"
                    v-model="condition.manager">
                </el-input>
            </div>
            <div class="btn-group">
                <el-button type="primary" plain icon="el-icon-search" @click="search('ToStartAndGoing')">搜索</el-button>
                <el-button type="primary" plain @click="search('ToStart')">即将开始</el-button>
                <el-button type="primary" plain @click="search('ToEnd')">即将结束</el-button>
            </div>
        </div>
      </div>
      <board-table :startTime="condition.startTime" :activityList="activityList"></board-table>
  </div>
</template>

<script>
import BoardTable from './boardTable/BoardTable';
import apis from './commons/api';
export default {
    name: 'activity-board',
    props: {},
    components: {BoardTable},

    data () {
        const tdWidth = 120; // 表格每格的宽度
        return {
            category: {
                COMPREHENSIVE: {
                    nameText: '综合',
                    value: 'COMPREHENSIVE',
                    bgColor: '#33FF66'
                },
                TIRE: {
                    nameText: '轮胎',
                    value: 'TIRE',
                    bgColor: '#00CCFF'
                },
                MAINTENANCE: {
                    nameText: '保养',
                    value: 'MAINTENANCE',
                    bgColor: '#FF9933'
                },
                CARPRODUCTS: {
                    nameText: '车品',
                    value: 'CARPRODUCTS',
                    bgColor: '#FFFF99'
                },
                BEAUTIFY: {
                    nameText: '美容',
                    value: 'BEAUTIFY',
                    bgColor: '#FF33FF'
                },
                MODIFICATION: {
                    nameText: '改装',
                    value: 'MODIFICATION',
                    bgColor: '#A1A1A1'
                },
                EXTERNALDELIVERY: {
                    nameText: '外部投放',
                    value: 'EXTERNALDELIVERY',
                    bgColor: '#CCCCFF'
                },
                OTHER: {
                    nameText: '其他',
                    value: 'OTHER',
                    bgColor: '#FF9999'
                }},
            checkList: ['COMPREHENSIVE', 'TIRE', 'MAINTENANCE', 'CARPRODUCTS', 'BEAUTIFY', 'MODIFICATION', 'EXTERNALDELIVERY'],
            condition: {
                startTime: new Date(),
                activityName: '',
                creator: '',
                manager: ''
            },
            activityList: null,
            tdWidth: tdWidth, // 表格每格的宽度
            maxWidth: 30 * tdWidth // 整个表格的宽度
        };
    },

    computed: {},

    watch: {
        checkList() {
            this.search();
        }
    },

    created () {},

    mounted () {
        this.search();
    },

    methods: {
        search(timeType) {
            if (!this.condition.startTime) {
                this.$$errorMsg('请选择起始日期');
                return;
            }
            let param = {
                ActivityType: '\'' + this.checkList.join('\',\'') + '\'', // 服务要求类型格式："'COMPREHENSIVE','MODIFICATION','CARPRODUCTS'"
                StartTime: this.condition.startTime.toLocaleDateString(),
                ActivityName: this.condition.activityName,
                CreateBy: this.condition.creator,
                Head: this.condition.manager,
                TimeType: timeType || 'ToStartAndGoing'
            };
            this.$http.get(apis.GetActivityBoardInfoList, {
                apiServer: 'apiServer',
                isLoading: true,
                params: param
            }).subscribe(res => {
                let _res = res && res.data;
                let activityBoardInfoList = _res && _res.ActivityBoardInfoList;
                const searchStartTime = new Date(param.StartTime).getTime();
                const searchEndTime = searchStartTime + 24 * 3600 * 1000 * 29;
                activityBoardInfoList && activityBoardInfoList.forEach(item => {
                    const acStartTime = new Date(item.StartTimeString).getTime();
                    const moreStartTime = Math.max(acStartTime, searchStartTime);
                    const acEndTime = new Date(item.EndTimeString).getTime();
                    const lessEndTime = Math.min(acEndTime, searchEndTime);
                    const toEndDays = Math.ceil((lessEndTime - moreStartTime) / (24 * 3600 * 1000)) + 1;
                    item.width = toEndDays * this.tdWidth + 'px';
                    if (acStartTime > searchStartTime) {
                        const toStartDays = Math.ceil((acStartTime - searchStartTime) / (24 * 3600 * 1000));
                        item.marginLeft = toStartDays * this.tdWidth + 'px';
                    }
                    let _activityType = this.category[item.ActivityType] ? item.ActivityType : 'OTHER';
                    item.bgColor = this.category[_activityType].bgColor;
                });
                this.activityList = activityBoardInfoList;
            });
        }
    }

};
</script>
<style lang='scss'>
.board-page {
    background: #fff;
    padding: 20px;
    height: calc(100vh - 78px);
    margin-top: -40px;
    .board-header {
        height: 110px;
    }
    .checkbox {
        .el-checkbox__input {
            background: #fff;
            padding: 5px;
        }
        .el-checkbox__label {
            padding-right: 10px;
            color: #363636;
        }
    }
    .search-bar {
        margin: 20px auto;
        display: flex;
        justify-content: space-between;
        .condition {
            .el-input {
                width: 150px;
            }
        }
        .btn-group {
            .el-button {
                margin-left: 0;
            }
        }
    }
}

</style>
