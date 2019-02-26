<template>
  <div class="extend-control-items" v-if="controlConfig">
    <template v-if='controlConfig.type === "button"'>
      <el-button class="extend-control-item-cont" v-text='controlConfig.nameText'
        @click="extendClick(controlConfig)"></el-button>
    </template>
    <template v-else-if='controlConfig.type === "fileButton"'>
      <upload-control
        :form-model="formModel"
        :config='config'
        :control-config="controlConfig"
        v-model="myValue"
        @extendClick="extendClick">
      </upload-control>
    </template>
    <template v-else-if='controlConfig.type === "img"'>
      <div class='img-container'
            @click="extendClick(controlConfig)">
            <template v-if="controlConfig.src || formModel && formModel.get(config.controlName) && formModel.get(config.controlName).value">
                <img :src="controlConfig.src || formModel && formModel.get(config.controlName) && formModel.get(config.controlName).value" width='100%' height="100%" />
            </template>
      </div>
    </template>
    <template v-else-if='controlConfig.type === "color"'>
        <color-control
            :control-config="controlConfig"
            :form-model='formModel'
            v-model="myValue"
            :config='config'
            @extendClick="extendClick"></color-control>
    </template>
    <template v-else-if="controlConfig.type === 'link'">
        <link-control
            :control-config='controlConfig'
            :form-model='formModel'
            :config='config'
            :valueData="valueData"
            @extendClick="extendClick"></link-control>
    </template>
    <template v-else>
        <span
            @click="extendClick(controlConfig)">{{controlConfig.nameText}}</span>
    </template>
  </div>
</template>

<script>
import ControlFixExtend from '../../controlFixExtend/ControlFixExtend';
import UploadControl from '../../../uploadControl/UploadControl';
import ColorControl from '../../../colorControl/ColorControl';
import LinkControl from '../../../linkControl/LinkControl';

export default {
    extends: ControlFixExtend,
    components: {
        UploadControl,
        ColorControl,
        LinkControl
    }
};
</script>

<style lang="scss">
  .extend-control-items {
    display: inline-block;
    .extend-control-item-cont{
      display: inline-block;
    }
    .img-container {
      border: 1px solid #e4e7ed;
      width: 40px;
      height: 40px;
      display: inline-block;
    }
  }
</style>
