<script>
import FormList from '../formList/FormList';

export default {
    // inject: ['$$form'],
    props: {
        defaultData: {
            type: [Object, Array]
        },
        formType: {
            type: String,
            default: ''
        },
        formConfig: {
            type: Array
        },
        addData: { // 每次添加项的时候追加数据
            type: Object
        }
    },
    data() {
        let _that = this;
        return {
            formModel: null
        };
    },
    components: {
        FormList
    },
    watch: {
        defaultData(nowVal) {
            nowVal && this.formModel && this.formModel.setValue(this.defaultData);
        },
        formConfig() {
            if (!this.formModel) {
                this.initFormModel();
            }
        }
    },
    created() {
        this.initFormModel();
    },
    methods: {
        initFormModel() { // 初始化formModule
            if (this.formConfig && this.formConfig.length > 0) {
                let form = this.$$form;
                let formModel;
                if (this.formType === 'array') {
                    formModel = form.initFormArray(this.formConfig, this.defaultData);
                } else {
                    formModel = form.initFormData(this.formConfig, this.defaultData);
                }
                this.formModel = formModel;
                this.$emit('formInit', formModel);
            }
        },
        /**
         * 发布事件给父组件
         * @param {any} val 发送的内容
         * @param {any} index 发送的内容
         */
        emitValueChange(val, index) { // 设置
            if (val instanceof Object && typeof index !== 'undefined') {
                val.groupIndex = index;
            }
            this.$emit('valueChange', val);
        },
        /**
         * 发布事件给父组件
         * @param {any} val 发送的内容
         * @param {any} index 发送的内容
         */
        updataExtendClick(val, index) {
            if (val instanceof Object && typeof index !== 'undefined') {
                val.groupIndex = index;
            }
            this.$emit('extendClick', val);
        },
        /**
         * 发布事件给父组件
         * @param {any} val 发送的内容
         * @param {any} index 发送的内容
         */
        emitSelectFocus(val, index) {
            if (val instanceof Object && typeof index !== 'undefined') {
                val.groupIndex = index;
            }
            this.$emit('selectFocus', val);
        }
    }
};
</script>
