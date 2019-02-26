<script>
export default {
    model: {
        prop: 'mValue',
        event: 'valueChange'
    },
    // inject: ['$$form'],
    props: {
        mValue: { // 传入的值

        },
        itemFliter: { // 每项数据的过滤器
            type: Function
        },
        formModel: { // 表单数据对象
            type: Object
        },
        controlConfig: { // 表单元素配置参数
            type: Object | Array
        },
        bypassedWrap: { // 是否不处理wrap包装
            default: false
        },
        valueData: { // 当前项的value

        }
    },
    data() {
        let _that = this;
        return {
            isBlur: false, // 是否失去焦点
            isSyncMvalue: true,
            myValue: _that.mValue,
            myValueData: null // // 当前项的value
        };
    },
    computed: {
        /**
         * 适配失去焦点状态
         * @return {Boolean}
         */
        hasBlur() {
            let isBlur = this.isBlur || false;
            if (this.controlConfig) {
                let con = this.formModel && this.formModel.get(this.controlConfig.controlName);
                if (con && con.isBlur) {
                    isBlur = con.isBlur;
                }
            }
            return isBlur;
        },
        /**
         * 判断是否为必填
         * @return {Boolean}
         */
        isRequired() {
            let res = false;
            let valids = this.controlConfig && this.controlConfig.valid;
            let valid = valids;
            if (valids instanceof Array) {
                valid = valids[0];
            }
            if (valid instanceof Object && (valid.required || valid.isChecked)) {
                res = true;
            }
            return res;
        },
        /**
         * 必填标记是否在右边
         * @return {Boolean}
         */
        requiredRight() {
            let res = false;
            let controlConfig = this.controlConfig;
            if (controlConfig instanceof Object && controlConfig.requiredRight) {
                res = true;
            }
            return res;
        }
    },
    watch: {
        mValue(nowVal) { // 传入的值
            if (this.isSyncMvalue) {
                this.myValue = nowVal;
            }
        },
        myValue(nowVal) { // 当前组件内部值
            this.myValueData = nowVal;
            this.emitValueChange(nowVal);
        }
    },
    methods: {
        /**
         * 设置失去焦点的值
         */
        setIsBlur() {
            if (this.formModel && this.controlConfig) {
                let con = this.formModel.get(this.controlConfig.controlName);
                if (con) {
                    con.isBlur = true;
                }
            }
            this.isBlur = true;
        },
        /**
         * 发布事件给父组件
         * @param {any} val 发送的内容
         */
        emitValueChange(val) { // 设置
            this.$emit('valueChange', val);
        },

        /**
         * descList和prefixList项的click
         * @param {Object} nowVal 当前项对应的数据
         */
        extendClick(nowVal) {
            this.$emit('extendClick', nowVal);
        }
    }
};
</script>
