<template>
    <div class="color-container" v-if="controlConfig">
        <el-color-picker
            @change="extendClick(controlConfig)"
            @active-change="setParentIsBlur()"
            :popper-class="controlConfig.popperClass"
            :readonly="controlConfig.readonly"
            :disabled="controlConfig.disabled"
            class="el-color-picker"
            v-model="myValue">
        </el-color-picker>
    </div>
</template>

<script>
import ControlFixExtend from '../common/controlFixExtend/ControlFixExtend';

export default {
    extends: ControlFixExtend,
    methods: {
    }
};
</script>

<style lang="scss">
.color-container {
    vertical-align: middle;
    line-height: 40px;
    .el-color-picker{
        margin-top: -3px;
        vertical-align: middle;
        line-height: 40px;
    }
}
</style>

