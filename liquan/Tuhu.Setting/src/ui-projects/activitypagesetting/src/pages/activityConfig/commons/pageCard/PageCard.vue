<template>
<div class="page-card">
    <el-card class="box-card">
        <div slot="header" class="list-box-title clearfix">
            <span class="header-title">{{title}}</span>
            <i v-if="showHideArrow" :class="isShow ? 'el-icon-arrow-down' : 'el-icon-arrow-up'" @click="changeShow"></i>
            <el-button v-if="showTopButton" @click="topSubmit"  type="primary" plain class="list-form-right-btn">{{topButton}}</el-button>
        </div>
        <div ref="cardContent" class="card-content">
            <el-row>
                <el-col :span="24">
                    <slot></slot>
                </el-col>
            </el-row>
            <el-row v-if="showBottomButton">
                <div class="grid-content bg-purple-light">
                    <el-button type="primary" @click="submit">{{bottomButton}}</el-button>
                </div>
            </el-row>
        </div>
    </el-card>
</div>
</template>

<script>
    export default {
        props: {
            title: {
                type: String
            },
            topButton: {
                default: '　新建　'
            },
            bottomButton: {
                default: '　保存　'
            },
            showTopButton: {
                type: Boolean,
                default: false
            },
            showBottomButton: {
                type: Boolean,
                default: true
            },
            showHideArrow: { // 是否显示 下拉/收起 按钮
                type: Boolean,
                default: true
            },
            showPageContent: { // page内容默认状态----true->默认显示----false->默认隐藏
                type: Boolean,
                default: true
            },
            keepHidePageContent: { // page内容是否保持隐藏状态
                type: Boolean,
                default: false
            }
        },
        data() {
            return {
                isShow: true
            };
        },
        mounted() {
            this.isShow = this.showPageContent;
            if (this.keepHidePageContent) {
                this.isShow = false;
            }
            if (this.showHideArrow && !this.isShow) {
                this.$refs.cardContent.classList.add('hide');
            }
        },
        watch: {
            showPageContent(newVal) {
                this.isShow = newVal;
                if (this.showHideArrow && newVal) {
                    this.$refs.cardContent.classList.remove('hide');
                }
            }
        },
        methods: {
            topSubmit(evt) {
                this.$emit('topSubmit', evt);
            },
            submit(evt) {
                this.$emit('submit', evt);
            },
            changeShow(evt) {
                this.$emit('changeShow', evt);
                if (this.keepHidePageContent) {
                    return false;
                }
                this.isShow = !this.isShow;
                this.$refs.cardContent.classList.toggle('hide');
            }
        }
    };
</script>

<style lang='scss'>
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";

.page-card {
    margin-bottom: 20px;
    .grid-content.bg-purple-light{
        text-align: center;
    }
    .box-card{
        .header-title{
            font-size: 18px;
        }
    }
    .list-form-right-btn {
        position: absolute;
        right: 0px;
        // left: 0;
    }
    .list-box-title{
        position: relative;
        .list-form-right-btn {
            top: -7px;
            right: 0px;
            // left: 0;
        }
        .el-icon-arrow-down,
        .el-icon-arrow-up {
            font-size: 20px;
            color: $color6;
            margin-left: 10px;
        }
    }
    @keyframes showCard {
        0%{transform: translate(0, 80px)}
        100%{transform: translate(0, 0) }
    }
    .card-content{
        // display: block;
        min-height: 200px;
        height: auto;
        overflow: visible;
        transition: min-height 500ms;
        &.hide{
            // display: none;
            height: 0;
            min-height: 0;
            overflow: hidden;
        }
    }
}
</style>
