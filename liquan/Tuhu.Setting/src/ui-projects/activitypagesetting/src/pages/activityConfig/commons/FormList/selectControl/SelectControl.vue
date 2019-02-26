<template>
  <form-control-wrap
    v-if="controlConfig"
    :form-model="formModel"
    :bypassedWrap="bypassedWrap"
    :control-config="controlConfig"
    :valueData="myValueData"
    @extendClick="extendClick"
    @valueChange="emitValueChange">
    <el-select class='control-items' filterable
        :filter-method="filterMethod"
        :popper-class="controlConfig.popperClass"
        :placeholder='controlConfig.placeholder'
        :multiple='controlConfig.multiple'
        :readonly="controlConfig.readonly"
        :disabled="controlConfig.disabled"
        @blur="setIsBlur"
        @focus="$emit('selectFocus', formModel)"
        :class="[hasBlur && 'isBlur']"
        v-model="myValue">
        <el-option
            v-for="item in filterOptions"
            :key="item.value"
            :label="item.nameText"
            :readonly="controlConfig.readonly"
            :disabled="controlConfig.disabled"
            :value="item.value">
        </el-option>
    </el-select>
  </form-control-wrap>
</template>

<script>
import FormControlWrap from '../common/formControlWrap/FormControlWrap';
import ControlExtend from '../common/controlExtend/ControlExtend';
const pinyin = require('pinyin');

export default {
    extends: ControlExtend,
    components: {
        FormControlWrap
    },
    data() {
        return {
            options: null,
            filterOptions: null
        };
    },
    created() {
        this.resetMyValue();
        this.getFilterOptions();
        this.$watch('controlConfig.list', function (newVal, oldVal) {
            this.getFilterOptions();
        });
    },
    watch: {
        myValue() {
            this.resetMyValue();
        },
        controlConfig() {
            this.getFilterOptions();
        }
    },
    methods: {
        resetMyValue() {
            if (this.controlConfig.multiple && (this.myValue === null || this.myValue === undefined)) {
                this.myValue = [];
            }
        },
        getFilterOptions() {
            this.options = this.controlConfig && this.controlConfig.list;
            this.options && this.options.forEach(item => {
                item.pinyin = pinyin(item.nameText, {
                    style: pinyin.STYLE_NORMAL
                }).join('').toLocaleLowerCase();
            });
            this.filterOptions = Object.assign({}, this.options);
        },
        filterMethod(query) {
            query = query.toLocaleLowerCase();
            this.filterOptions = this.options && this.options.filter(option => {
                return option.pinyin.indexOf(query) > -1 || option.nameText.indexOf(query) > -1;
            });
            // this.controlConfig.list = options;
        }
    }
};
</script>
