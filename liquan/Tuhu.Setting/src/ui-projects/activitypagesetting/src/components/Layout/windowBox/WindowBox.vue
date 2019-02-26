<template>
    <div class="window-boxs">
        <header class="window-header">
            <div class="window-title">{{title}}</div>
            <i @click="cancel" class="el-icon-close"></i>
        </header>
        <div class="window-cont">
            <slot></slot>
        </div>
        <div class="window-buttons">
            <el-button v-if="showCancel" class="cancel-btn" @click="cancel">{{cancelText}}</el-button>
            <el-button v-if="showsubmit" class="save-btn" @click="submit">{{submitText}}</el-button>
        </div>
    </div>
</template>

<script>
    export default {
        props: {
            cancelText: {
                default: '取消'
            },
            showCancel: {
                default: true
            },
            submitText: {
                default: '保存'
            },
            showsubmit: {
                default: true
            },
            title: {
                type: String,
                default: ''
            }
        },
        methods: {
            cancel(evt) {
                this.$emit('cancel', evt);
            },
            submit(evt) {
                this.$emit('submit', evt);
            }
        }
    };
</script>

<style lang="scss" scoped>
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";
.window-boxs{
    border:1px solid $brColorE4e;
    background:$colorf;
    .window-header{
        height:48px;
        background:$bgColorF5;
        border-bottom:1px solid $brColorE4e;
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 0 15px;
    }
    .window-title{
        color:$color3;
        font-weight: 700;
        font-size: 16px;

    }
    .window-cont{
        // width: 423px;
        background:$colorf;
        padding: 0 20px;
        // border-top: 0;
    }
    .window-buttons{
        display:flex;
        justify-content: center;
        padding-bottom: 20px;
        .save-btn{
            padding: 8px 30px;
            background: $bgColorBtn;
            color: $colorf;
            margin-left: 30px;
        }
        .cancel-btn{
            padding: 8px 30px;
            background: $bgColorD9;
            color: $colorf;
        }
    }
}

</style>
