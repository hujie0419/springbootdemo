<template>
    <div
        class='form-group-panels'
        v-if='formModel && formType === "array" && formConfig && formConfig.length'>
        <template v-for='(formList, index) in formConfig'>
            <el-row
                class="form-group-panel-row"
                :gutter="20"
                :key='index'>
                <el-col :span='20'>
                    <form-list
                        :deleteColums='deleteColums'
                        :form-config='formList'
                        :form-model='formModel.get(index)'
                        @extendClick="updataExtendClick($event, index)"
                        @valueChange="emitValueChange($event, index)"></form-list>
                </el-col>
                <el-col :span='4'>
                    <div class='grid-content'>
                        <add-minus
                            :show-add-btn='(index + 1) >= minPanel'
                            :show-minus-btn='(index) >= minPanel'
                            @addControl="addOrDeleteForm({
                                type: 'add',
                                index: index
                            })"
                            @minusControl="addOrDeleteForm({
                                type: 'minus',
                                index: index
                            })">
                        </add-minus>
                    </div>
                </el-col>
            </el-row>
        </template>
    </div>
</template>

<script>
// import FormList from '../formList/FormList';
import AddMinus from '../formList/addMinusControl/AddMinusControl';
import FormGroupExtend from '../formGroupExtend/FormGroupExtend';

export default {
    extends: FormGroupExtend,
    data() {
        return {
            formUpdata: null
        };
    },
    props: {
        // formType: {
        //     type: String,
        //     default: ''
        // },
        // formConfig: {
        //     type: Array
        // },
        isDefaultAddItem: {
            type: Boolean
        },
        minPanel: {
            type: Number,
            default: 1
        },
        maxPanel: {
            type: Number,
            default: 5
        },
        // 获取新的表单配置对象
        getConfig: {
            type: Function
        },
        delFormItem: { // 移除一项group的回调
            type: Function
        },
        // 增加或减少表单项的方法
        addOrDelete: {
            type: Function
        },
        deleteColums: {
            type: Array
        }
    },

    watch: {
        defaultData(nowval) { // 默认值发生改变时，根据默认值添加formgroup
            if (!this.isDefaultAddItem) {
                return;
            }
            let val = this.formModel && this.formModel.value;
            if (nowval && val && (nowval.length > val.length)) {
                let len = nowval.length - val.length;
                for (let i = 0; i < len; i++) {
                    this.addOrDeleteForm({
                        type: 'add',
                        index: val.length + i,
                        data: nowval[i]
                    });
                }
                this.formModel.setValue(nowval);
            }
        }
    },
    // data () {
    //     return {
    //         formModel: null
    //     };
    // },
    methods: {
        // emitValueChange(val, index) { // 设置
        //     if (val instanceof Object && typeof index !== 'undefined') {
        //         val.groupIndex = index;
        //     }
        //     this.$emit('valueChange', val);
        // },
        filterControlConfig ({ groupIndex }) {
            const _that = this;
            if (_that.formConfig && _that.formConfig.length > 0) {
                for (let i = 0; i < _that.formConfig.length; i++) {
                    let config = _that.formConfig[i];
                    config[0].nameText = config[0].nameText.replace(/\d+/, i + 1);
                }
            }
        },
        addOrDeleteForm ({ type, index, data }) {
            let _that = this;
            if (this.addOrDelete && typeof this.addOrDelete === 'function') {
                this.addOrDelete({type, index});
            } else {
                this.addOrDeleteDefault({ type, index, data });
                // for (let i = 0; i < _that.formConfig.length; i++) {
                //     let config = _that.formConfig[i];
                //     this.$set(config[0], 'nameText', config[0].nameText.replace(/\d+/, i + 1));
                // }
            }
        },
        /**
         * 默认的方法(增加或减少一项表单)
         * @param {object} options 当前参数
         * @param {string} options.type 增加或减少一项表单
         * @param {number} options.index 触发事件新增或减少的表单索引
         */
        addOrDeleteDefault ({ type, index, data }) {
            let _that = this;
            let _formConfig = this.formConfig || [];
            switch (type) {
                case 'add':
                    if (_formConfig.length < this.maxPanel) {
                        const newIndex = index + 1;
                        let item;
                        if (_that.getConfig instanceof Function) {
                            item = _that.getConfig(newIndex + 1, _that);
                        }
                        _that.formConfig.splice(newIndex, 0, item);
                        _that.filterControlConfig({
                            groupIndex: newIndex
                        });
                        _that.formUpdata = _that.$$form.initFormData(item, data || this.addData);
                        _that.formModel.setItem(_that.formUpdata, newIndex);
                    }
                    break;
                case 'minus':
                    if (_formConfig.length > 1) {
                        if (_that.delFormItem instanceof Function) {
                            _that.delFormItem(index);
                        }
                        _that.formConfig.splice(index, 1);
                        _that.deleteColums.push(_that.formModel.get(index).value);
                        this.filterControlConfig({ groupIndex: index });
                        _that.formModel.removeItem(index);
                    }
                    break;
            }
            this.$emit('formInit', this.formModel);
            this.$emit('formConfigUpdate', _that.formConfig);
        }
    },
    // mounted () {
    //     if (this.formConfig && this.formConfig.length > 0) {
    //         let form = this.$$form;
    //         this.formModel = this.formType === 'array' ? form.initFormArray(this.formConfig) : form.initFormData(this.formConfig);
    //         this.$emit('formInit', this.formModel);
    //     }
    // },
    components: {
        // FormList,
        AddMinus
    }
};
</script>

<style lang='scss'>
@import "css/common/_var.scss";
.form-group-panels {
  .form-group-panel-row {
    display: flex;
    align-items: center;
    border-bottom: 1px dashed $color9;
    padding: 10px 0;
    &:last-child {
      border-bottom: 0;
    }
  }
}
</style>

