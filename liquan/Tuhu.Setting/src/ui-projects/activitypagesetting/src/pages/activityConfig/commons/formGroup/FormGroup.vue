<template>
  <div v-if="formModel">
    <template v-if="formType === 'array'">
      <template v-for='(item, index) in formConfig'>
        <form-list
            :formModel="formModel.get(index)"
            :form-config='item'
            :key='index'
            @valueChange="emitValueChange($event, index)"
            @extendClick="updataExtendClick($event, index)">
        </form-list>
      </template>
    </template>
    <template v-else>
      <form-list
        :formModel="formModel"
        :form-config='formConfig'
        @valueChange="emitValueChange($event)"
        @extendClick="updataExtendClick($event)">
      </form-list>
    </template>
  </div>
</template>

<script>
import FormList from '../formList/FormList';
import FormGroupExtend from '../formGroupExtend/FormGroupExtend';

export default {
    extends: FormGroupExtend
    // // inject: ['$$form'],
    // props: {
    //     defaultData: {
    //         type: [Object, Array]
    //     },
    //     formType: {
    //         type: String,
    //         default: ''
    //     },
    //     formConfig: {
    //         type: Array
    //     }
    // },
    // data() {
    //     let _that = this;
    //     return {
    //         formModel: null
    //     };
    // },
    // components: {
    //     FormList
    // },
    // watch: {
    //     defaultData() {
    //         this.formModel && this.formModel.setValue(this.defaultData);
    //     }
    // },
    // created() {
    //     if (this.formConfig && this.formConfig.length > 0) {
    //         let form = this.$$form;
    //         let formModel;
    //         if (this.formType === 'array') {
    //             formModel = form.initFormArray(this.formConfig, this.defaultData);
    //         } else {
    //             formModel = form.initFormData(this.formConfig, this.defaultData);
    //         }
    //         this.formModel = formModel;
    //         this.$emit('formInit', formModel);
    //     }
    // },
    // methods: {
    //     /**
    //      * 发布事件给父组件
    //      * @param {any} val 发送的内容
    //      * @param {any} index 发送的内容
    //      */
    //     emitValueChange(val, index) { // 设置
    //         if (val instanceof Object && typeof index !== 'undefined') {
    //             val.groupIndex = index;
    //         }
    //         this.$emit('valueChange', val);
    //     }
    // }
};
</script>
