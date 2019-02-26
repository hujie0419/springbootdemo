<template>
  <div class="form-extend-filters" v-if="controlConfig && controlConfig.length">
    <extend-item
        class='form-control-leftPadding'
        :class="item.className || ''"
        v-for="(item, index) in controlConfig"
        :form-model="formModel"
        :itemFliter="itemFliter"
        :control-config="myItemFliter(item)"
        :config="config"
         v-model="myValue"
        :key="index"
        :valueData="valueData"
        @extendClick="extendClick">
    </extend-item>
  </div>
</template>

<script>
import ExtendItem from './extendItem/ExtendItem';
import ControlExtend from '../controlExtend/ControlExtend';

export default {
    extends: ControlExtend,
    props: {
        // itemFliter: {
        //     type: Function
        // },
        // control: {
        //     type: Object
        // }
        config: {
            type: Object
        }
    },
    components: {
        ExtendItem
    },
    methods: {
        /**
         * 执行过滤
         * @param {*} item 当前表单控件配置项
         * @returns {*} 返回过滤后的数据
         */
        myItemFliter(item) {
            let res = item;
            if (this.itemFliter instanceof Function) {
                let _res = this.itemFliter(item);
                if (typeof _res !== 'undefined') {
                    res = _res;
                }
            }
            return res;
        }
    }
};
</script>
<style lang="scss">
.form-extend-filters {
  display: inline-block;
}
</style>
