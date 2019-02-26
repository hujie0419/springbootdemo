<template>
  <div class="board-table">
      <div class="table-main">
        <div class="table-header">
            <div class="daylist">
                <span class="day" v-for="day in thData" :key="day.date">{{day.day}}</span>
            </div>
            <div class="datelist">
                <span class="date" v-for="date in thData" :key="date.date">{{date.date}}</span>
            </div>
        </div>
        <div class="activity-list" ref="activityList">
            <div class="activity"
                v-for="activity in activityList"
                :key="activity.ActivityId"
                :style="{width: activity.width, backgroundColor: activity.bgColor, marginLeft: activity.marginLeft}">
            </div>
        </div>
      </div>
      <div class="table-text" @scroll="textScroll">
        <div class="name"
            v-for="activity in activityList"
            :key="activity.ActivityId"
            @click="showDetail(activity.ActivityId)">
            {{activity.ActivityId}} {{activity.ActivityName}}
        </div>
      </div>
  </div>
</template>
<script>
import ActivityDetail from '../activityDetail/ActivityDetail';
import apis from '../commons/api';
export default {
    name: '',
    props: {
        startTime: {
            type: Date,
            required: true
        },
        activityList: {
            type: Array
        }
    },
    components: {},

    data () {
        return {
            datArray: ['周日', '周一', '周二', '周三', '周四', '周五', '周六']
        };
    },

    computed: {
        thData() {
            let _startTime = this.startTime || new Date();
            let _thData = [];
            let dateTime = _startTime.getTime();
            for (let i = 0; i < 30; i++) {
                let _dateTime = dateTime + i * 24 * 3600 * 1000;
                let _date = new Date(_dateTime);
                let _day = this.datArray[_date.getDay()];
                _thData.push({
                    day: this.datArray[_date.getDay()],
                    date: (_date.getMonth() + 1) + '月' + _date.getDate() + '日'
                });
            }
            return _thData;
        }
    },

    watch: {},

    created () {},

    mounted () {},

    methods: {
        textScroll(e) {
            let scrollTop = e.target.scrollTop;
            this.$refs.activityList.scrollTop = scrollTop;
        },
        showDetail(ActivityId) {
            this.$http.get(apis.GetActivityBoardDetailInfo, {
                apiServer: 'apiServer',
                isLoading: true,
                params: {
                    activityID: ActivityId
                }
            }).subscribe(res => {
                const activity = res && res.data;
                let popup = this.$tPopup.openPopup(ActivityDetail, {
                    props: {
                        activity: activity
                    },
                    wrapCla: 'acdetail', // 最外层追加的Class名
                    isClickBgClose: true,
                    alignCla: 'centerMiddle', // ''|'bottom'|'top'|'fullScreen'|'centerMiddle',
                    transitionCls: 't_scale' // , // ''|'t_toUp'|'t_scale'|'t_toBottom'|'t_toLeft';
                });
                let com = popup && popup.component;
                if (com) {
                    com.$on('cancelChange', (dataItem) => {
                        popup.closePopup();
                    });
                }
            });
        }
    }

};
</script>
<style lang='scss'>
.board-table {
    position: relative;
    .table-main {
        position: relative;
        overflow-x: scroll;
    }
    .table-header {
        position: absolute;
        border-top: 1px solid #ddd;
        border-left: 1px solid #ddd;
        white-space: nowrap;
        width: fit-content;
        // .daylist,
        // .datelist {
        //     white-space: nowrap;
        // }
        .daylist .day,
        .datelist .date {
            display: inline-block;
            height: 30px;
            line-height: 30px;
            width: 120px;
            text-align: center;
            border-bottom: 1px solid #ddd;
            border-right: 1px solid #ddd;
        }
        .daylist .day {
            font-weight: bold;
            background: #eee;
        }
    }
    .activity-list {
        margin-top: 60px;
        height: calc(100vh - 170px - 60px - 60px);
        width: 3620px;
        overflow-y: scroll;
        .activity {
            border-bottom: 1px solid #fff;
            border-radius: 3px;
            height: 30px;
            // line-height: 30px;
            // position: relative;
        }
    }
    .table-text {
        position: absolute;
        top: 62px;
        left: 0;
        bottom: 0;
        height: calc(100vh - 170px - 60px - 60px);
        width: 100%;
        overflow-y: scroll;
        .name {
            height: 30px;
            line-height: 30px;
            color: #363636;
            padding-left: 5px;
            cursor: pointer;
        }
    }
}

</style>
