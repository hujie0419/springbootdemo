<template>
    <div class='link-container' @click="extendClick(controlConfig)" v-if='isShowLink'>
        <span class='link-style' :class="[controlConfig.className || '']">{{ controlConfig.nameText }}</span>
    </div>
</template>

<script>
import ControlExtend from '../common/controlExtend/ControlExtend';

export default {
    extends: ControlExtend,
    props: {
        config: {
            type: Object
        }
    },
    computed: {
        isShowLink () {
            let res = false;
            let valueData = this.valueData;
            const status = this.controlConfig.status;
            // let control = this.formModel.get(this.config.controlName);
            // return !status || (status === (control && control.value && control.value.select));
            if (typeof status !== 'undefined') {
                if (valueData instanceof Array) {
                    valueData.find(item => {
                        res = getStatus(item);
                        return res;
                    });
                } else {
                    res = getStatus(valueData);
                }
            } else {
                res = true;
            }
            return res;

            /**
             * 获取状态
             * @param {Object} item 当前的值
             * @return {Boolean}
             */
            function getStatus(item) {
                let _res = false;
                if (item instanceof Object && item.select === status) { // 先择组类型
                    _res = true;
                } else {
                    _res = (item === status); // 值类型
                }
                return _res;
            }
        }
    }
};
</script>

<style lang='scss'>
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";
.link-container {
    .link-style {
        color: $linkColor;
        text-decoration: underline;
        cursor: pointer;
        &:active {
            color: $colorRed;
        }
        &.link-left-margin {
            margin-left: 15px;
        }
    }
}
</style>
