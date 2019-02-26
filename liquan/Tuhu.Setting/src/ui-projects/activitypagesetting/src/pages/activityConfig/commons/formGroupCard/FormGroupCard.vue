<template>
  <div v-if="formModel" class="form-group-cards">
    <template v-if="formType === 'array'">
      <template v-for='(item, index) in formConfig'>
          <el-card class="box-card form-group-card-box" :key="index">
            <div slot="header" class="clearfix">
                <span>第{{index + 1}}列</span>
            </div>
            <el-row class="form-group-cards-row" :gutter="22" width="100%">
                <el-col :span="21">
                    <div class="grid-content">
                        <form-list
                            @extendClick="updataExtendClick($event, index)"
                            @valueChange="emitValueChange($event, index)"
                            @selectFocus="emitSelectFocus($event, index)"
                            :formModel="formModel.get(index)"
                            :form-config='item'
                            :key='index'>
                        </form-list>
                    </div>
                </el-col>
                <el-col class="form-group-cards-col form-group-cards-right" :span="3">
                    <div class="grid-content">
                        <add-minus @addControl="editControl({
                            type: 'add',
                            index: index
                        })" @minusControl="editControl({
                            type: 'minus',
                            index: index
                        })"></add-minus>
                    </div>
                </el-col>
            </el-row>
        </el-card>
      </template>
    </template>
  </div>
</template>

<script>
// import FormList from '../formList/FormList';
import AddMinus from '../formList/addMinusControl/AddMinusControl';
import FormGroupExtend from '../formGroupExtend/FormGroupExtend';

export default {
    extends: FormGroupExtend,
    // props: {
    //     defalutData: {},
    //     formType: {
    //         type: String,
    //         default: ''
    //     },
    //     formConfig: {
    //         type: Array
    //     }
    // },
    // data() {
    //     return {
    //         formModel: null
    //     };
    // },
    components: {
        AddMinus
    },
    // mounted() {
    //     if (this.formConfig && this.formConfig.length > 0) {
    //         let form = this.$$form;
    //         let formModel = this.formType === 'array' ? form.initFormArray(this.formConfig) : form.initFormData(this.formConfig);
    //         this.formModel = formModel;
    //         this.$emit('formInit', formModel);
    //     }
    // },
    methods: {
        editControl(data) {
            this.$emit('editControl', data);
        }
        // /**
        //  * 发布事件给父组件
        //  * @param {any} val 发送的内容
        //  * @param {any} index 发送的内容
        //  */
        // emitValueChange(val, index) { // 设置
        //     if (val instanceof Object && typeof index !== 'undefined') {
        //         val.groupIndex = index;
        //     }
        //     this.$emit('valueChange', val);
        // }
    }
};
</script>
<style  lang='scss'>
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";
.form-group-cards {
    .form-group-card-box{
        margin-top: 20px;
        &:first-child {
            margin-top: 0;
        }
    }
    .form-group-cards-row {
        margin-bottom: 20px;
        display: flex;
        &:last-child {
            margin-bottom: 0;
        }
    }
    .form-group-cards-col {
        border-radius: 4px;
        &.form-group-cards-right {
            display: flex;
            align-items: center;
            justify-content: center;
        }
    }
}
</style>
