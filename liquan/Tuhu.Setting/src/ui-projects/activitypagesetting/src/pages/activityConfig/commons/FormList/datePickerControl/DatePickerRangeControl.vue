<template>
    <form-control-wrap
        class="date-picker-container"
        v-if="controlConfig"
        :form-model="formModel"
        :control-config="controlConfig"
        :valueData="myValueData"
        @extendClick="extendClick">
        <div class='data-picker-range-container'>
            <el-date-picker
                @blur="setIsBlur"
                :class="[hasBlur && 'isBlur']"
                v-model="startDate"
                :type="((controlConfig.type || '') + '').replace(/user|range/g, '')"
                :placeholder="controlConfig.startPlaceholder || controlConfig.placeholder || '开始日期'"
                :readonly="controlConfig.readonly"
                :disabled="controlConfig.disabled"
                :value-format="controlConfig.formatDate"
                :picker-options="pickerBeginDateBefore">
            </el-date-picker>
            <span>到</span>
            <el-date-picker
                @blur="setIsBlur"
                :class="[hasBlur && 'isBlur']"
                v-model="endDate"
                :type="((controlConfig.type || '') + '').replace(/user|range/g, '')"
                :placeholder="controlConfig.endPlaceholder || controlConfig.placeholder || '结束日期'"
                :readonly="controlConfig.readonly"
                :value-format="controlConfig.formatDate"
                :disabled="controlConfig.disabled"
                :picker-options="pickerBeginDateAfter">
            </el-date-picker>
        </div>
    </form-control-wrap>
</template>

<script>
import FormControlWrap from '../common/formControlWrap/FormControlWrap';
import ControlExtend from '../common/controlExtend/ControlExtend';

/**
 * 时间转换
 * @param {string|number|date} data 时间
 * @returns {number}
 */
function formatDate(data) {
    let res = data;
    if (typeof data === 'string') {
        res = new Date(data.replace('-', '/'));
    }

    if (res instanceof Date) {
        res = res.getTime();
    }
    return res;
}

export default {
    extends: ControlExtend,
    data () {
        return {
            startDate: null, // 开始日期
            endDate: null, // 结束日期
            pickerBeginDateBefore: {
                disabledDate: (time) => {
                    let beginDateVal = this.endDate;
                    if (beginDateVal) {
                        return time.getTime() > formatDate(beginDateVal);
                    }
                }
            },
            pickerBeginDateAfter: {
                disabledDate: (time) => {
                    let beginDateVal = this.startDate;
                    if (beginDateVal) {
                        return time.getTime() < formatDate(beginDateVal);
                    }
                }
            }
        };
    },
    created () {
        this.setDefaultValue();
    },
    methods: {
        /**
         * 设置默认值
         */
        setDefaultValue () {
            const _mValue = this.mValue;
            if (_mValue !== undefined && _mValue !== null) {
                if (_mValue instanceof Array) {
                    if (_mValue[0] !== undefined && this.startDate !== _mValue[0]) {
                        this.startDate = _mValue[0];
                    }
                    if (_mValue[1] !== undefined && this.endDate !== _mValue[1]) {
                        this.endDate = _mValue[1];
                    }
                } else {
                    if (this.startDate !== _mValue) {
                        this.startDate = _mValue;
                    }
                    if (this.endDate !== _mValue) {
                        this.endDate = _mValue;
                    }
                }
            }
        },
        setMyValue({index, value}) {
            this.myValue = this.myValue || new Array(2);
            this.myValue.splice(index, 1, value);
        }
    },
    components: {
        FormControlWrap
    },
    watch: {
        /**
         * 监听传入的值
         */
        mValue() {
            this.setDefaultValue();
        },
        /**
         * 监听开始时间的值
         * @param {string} newVal 改变后的开始时间值
         */
        startDate (newVal) {
            this.setMyValue({
                index: 0,
                value: newVal
            });
        },
        /**
         * 监听结束时间的值
         * @param {string} newVal 改变后的结束时间的值
         */
        endDate (newVal) {
            this.setMyValue({
                index: 1,
                value: newVal
            });
        }
    }
};
</script>
