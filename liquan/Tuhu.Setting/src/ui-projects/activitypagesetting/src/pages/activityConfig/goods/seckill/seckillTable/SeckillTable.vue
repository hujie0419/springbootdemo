<template>
    <table-box :pagination="false" v-if="formModel" class="seckill-table">
        <el-table
            :data="tableData"
            :row-class-name="'table-td-row'"
            :header-row-class-name="'table-th-row'"
            border
            style="width: 100%">
            <el-table-column
                width="55">
                <template slot-scope="scope">
                    <check-box-empty-control
                        v-model="formModel.get('SpikeSessionList').value"
                        @valueChange="validValue($event)"
                        :bypassedWrap="false"
                        :controlConfig="{type: 'checkBoxEmpty',value: scope.row.SpilkeActivityId,defaultValue: scope.row}">
                    </check-box-empty-control>
                </template>
            </el-table-column>
            <el-table-column
                label="秒杀场次">
                <template slot-scope="scope">
                    {{scope.row.SpikeStarTime + '~' + scope.row.SpikeEndTime}}
                </template>
            </el-table-column>
            <el-table-column
                prop="SpilkeActivityId"
                label="秒杀活动ID">
            </el-table-column>
            <el-table-column
                prop="SpilkeActivityName"
                label="秒杀活动名称">
                <template slot-scope="scope">
                    <a :href="'/QiangGou/Detail?aid='+scope.row.SpilkeActivityId" target="_blank" class="a-link">
                        {{scope.row.SpilkeActivityName}}
                    </a>
                </template>
            </el-table-column>
            <el-table-column
                prop="SpilkeProductCount"
                label="产品数量"
                width="120">
            </el-table-column>
        </el-table>
    </table-box>
</template>

<script>
import TableBox from '../../../commons/tableBox/TableBox';
import CheckBoxEmptyControl from '../../../commons/formList/checkBoxEmptyControl/CheckBoxEmptyControl';
export default {
    components: {
        TableBox,
        CheckBoxEmptyControl
    },
    props: {
        formModel: {
            type: Object
        },
        spikelist: {
            type: Array,
            default: () => {
                return [];
            }
        },
        sectionchange: {
            type: Function
        }
    },
    data() {
        return {
            tableData: []
        };
    },
    watch: {
        spikelist(newVal, oldVal) {
            this.tableData = (newVal||[]).map(item => {
                if (item instanceof Object) {
                    let startTime = (new Date((item.SpikeStarTime + '' || '').replace(/-/g, '/'))).getTime();
                    let endTime = (new Date((item.SpikeEndTime + '' || '').replace(/-/g, '/'))).getTime();
                    item._startTime = Math.min(startTime, endTime);
                    item._endTime = Math.max(startTime, endTime);
                }
                return item;
            });
        }
    },
    created() {

    },
    methods: {

        /**
         * 验证选择的秒杀场次
         * @param {Array} evt 当前选中的数据
         */
        validValue(evt) {
            let selectList = evt || [];
            let isErr = false;
            if (selectList.length > 1) {
                let selectItem = selectList[selectList.length - 1];
                selectList.every((item, index) => {
                    if ((item && item !== selectItem) &&
                        (this.diffTime(selectItem.value, item.value) || this.diffTime(item.value, selectItem.value))) {
                        selectList.splice(selectList.length - 1, 1);
                        isErr = true;
                        return false;
                    }
                    return true;
                });
            }
            if (isErr) {
                this.$$errorMsg('秒杀场次时间不能重叠');
            }
        },

        /**
         * 比较两次的时间是否有重叠
         * @param {Array} nowitem 当前当项
         * @param {Array} item 比较的项
         * @returns {Boolean}
         */
        diffTime(nowitem, item) {
            let res = false;
            if (nowitem instanceof Object && item instanceof Object) {
                if (nowitem._startTime > item._startTime && nowitem._startTime < item._endTime) {
                    res = true;
                } else if (nowitem._endTime > item._startTime && nowitem._endTime < item._endTime) {
                    res = true;
                } else if (nowitem._startTime === item._startTime && nowitem._endTime === item._endTime) {
                    res = true;
                }
            }
            return res;
        }
        // handleSelectionChange(val) {
        //     this.$emit('sectionchange', val);
        // }
    }
};
</script>

<style lang="scss">
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";
.seckill-table{
    margin-top: 0;
}
</style>
