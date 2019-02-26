<template>
    <div class='general-pages'>
        <div class="general-page-refresh" @click="refresh" v-if="showRefresh"><el-button type="primary">刷新</el-button></div>
        <div class='general-page-title'>
            <div class='general-page-title-left' v-text='title[0]'></div>
            <div class='general-page-title-right' v-text='title[1]'></div>
        </div>
        <!-- <form-group @formInit="formInit" :form-config='formConfig'></form-group> -->
        <slot></slot>
        <div class='save-btn' v-if="showBtn">
          <el-button type="primary" @click="submit()" v-text='btnTxt'></el-button>
        </div>
    </div>
</template>

<script>
// import FormGroup from '../commons/formGroup/FormGroup';

export default {
    props: {
        showBtn: { // 显示按钮
            type: Boolean,
            default: true
        },
        title: { // 标题
            type: Array,
            default: function () {
                return [];
            }
        },
        btnTxt: {
            type: String,
            default: '保存'
        },
        showRefresh: { // 显示刷新按钮
            type: Boolean,
            default: false
        }
        // formConfig: {
        //     type: Array,
        //     default: function () {
        //         return [];
        //     }
        // }
    },
    components: {
        // FormGroup
    },
    methods: {
        // formInit(formModel) {
        //     this.$emit('formInit', formModel);
        // },
        submit() {
            this.$emit('submit');
        },
        refresh() {
            this.$emit('refresh');
        }
    }
};
</script>

<style lang='scss'>
    @import "css/common/_var.scss";
    @import "css/common/_mixin.scss";
    @import "css/common/_iconFont.scss";

    .general-pages {
        // border: 1px solid rgba(0, 0, 0, 0.5);
        padding: 30px;
        background: $colorf;

        .general-page-refresh {
            position: absolute;
            right: 50px;
            top: 30px;
        }
        .general-page-title {
            display: flex;
            align-items: center;
            padding: 0 0 20px 20px;
            font-weight: bold;
        }
        .general-page-title-left {
            flex: 0 0 75px;
            margin-right: 15px;
            text-align: right;
        }
        .general-page-title-right {
            display: flex;
            flex: 1;
            align-items: center;
        }

        .save-btn {
          text-align: center;
          margin-top: 30px;
          .el-button {
            padding: 12px 40px;
            // background: rgba(22, 155, 213, 1);
            color: $colorf;
          }
        }
    }
</style>
