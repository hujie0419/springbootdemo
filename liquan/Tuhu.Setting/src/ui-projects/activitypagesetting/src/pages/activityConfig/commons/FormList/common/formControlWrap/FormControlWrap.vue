<template>
  <div class="form-control-wraps" v-if="!bypassedWrap && controlConfig">
    <div
        class="form-control-usbNameText form-control-leftPadding"
        :class="[(!requiredRight && isRequired && 'is-required') || '']"
        v-if="controlConfig.showNameText">
      {{controlConfig.nameText && controlConfig.nameText + '：' || ''}}
    </div>
    <form-extend-filter
        v-model="myValue"
        :form-model="formModel"
        :itemFliter="itemFliter"
        :config='controlConfig'
        :control-config="controlConfig.prefixList"
        :valueData="valueData"
        @extendClick="updataExtendClick">
    </form-extend-filter>
    <div class="form-control-cont" :class="[!controlConfig.showNameText && 'form-control-leftPadding', controlConfig.className || '']">
        <slot></slot>
    </div>
    <form-extend-filter
        v-model="myValue"
        :form-model="formModel"
        :itemFliter="itemFliter"
        :config='controlConfig'
        :control-config="controlConfig.descList"
        :valueData="valueData"
        @extendClick="updataExtendClick">
    </form-extend-filter>
    <div
        class="ext-fix"
        :class="[requiredRight && isRequired && 'is-required-ext' || '']"
        v-if="controlConfig.showNameText && requiredRight">
    </div>
  </div>
  <div class="form-control-container" v-else-if="bypassedWrap">
      <slot></slot>
  </div>
</template>

<script>
import FormExtendFilter from '../formExtendFilter/FormExtendFilter';
import ControlExtend from '../controlExtend/ControlExtend';

export default {
    extends: ControlExtend,
    components: {
        FormExtendFilter
    },

    methods: {
        /**
         * 更新extendClick事件数据
         * @param {Object} evt 当前点击项的数据
         */
        updataExtendClick(evt) {
            this.extendClick({
                controlConfig: this.controlConfig, // 当前项的controlConfig
                extendConfig: evt // 当前点击的extend项(descList,prifixList里item)的配置
            });
        }
    }
};
</script>

<style lang="scss">
.form-control-wraps {
    white-space: nowrap;
    // display: inline-block;
    display: inline-flex;
    .form-control-usbNameText {
        display: inline-block;
        text-align: right;
    }
    .form-control-cont {
        display: inline-block;
    }
    .form-control-small { // 小的文本框
        width: 150px;
        .control-items {
            width: 100%;
        }
    }
    .form-control-large { // 大的文本框
        width: 480px;
        .control-items {
            width: 100%;
        }
    }
}
.form-control-container {
    display: inline-block;
}
</style>
