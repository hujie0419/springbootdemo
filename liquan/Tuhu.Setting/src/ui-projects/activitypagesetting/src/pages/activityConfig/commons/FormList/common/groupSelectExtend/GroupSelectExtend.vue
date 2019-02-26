<script>
import ControlExtend from '../../common/controlExtend/ControlExtend';

export default {
    extends: ControlExtend,
    data() {
        return {
            checkValue: null, // 选中内容
            textValue: null, // 输入的内容
            isVmArray: false // 绑定的是否为数组
        };
    },
    watch: {
        /**
         * 监听传入的值
         */
        mValue() {
            this.setDefaultValue();
        },

        /**
         * 监听当前项选中内容
         * @param {boolean} nowVal 当前值
         */
        checkValue(nowVal) {
            let nowValue = this.controlConfig && this.controlConfig.value;
            let myValue = this.getMyValue();
            if (this.mValue && this.mValue.select !== nowValue) {
                this.resetTextValue(''); // 切换状态的时候清空内容
            }
            this.setMyValue();
        },
        /**
         * 监听输入内容发生改变
         */
        textValue() {
            this.setMyValue();
        }
    },
    created() {
        this.setDefaultValue();
    },
    methods: {
        /**
         * 同步默认值
         */
        setDefaultValue() {
            let _mValue = this.mValue;
            if (_mValue instanceof Array) {
                this.isVmArray = true;
            }
            let defValue = this.getMValue();
            let defSelectValue = defValue && defValue.select;
            let nowValue = this.controlConfig && this.controlConfig.value;

            if (defValue && defSelectValue === nowValue) {
                if (this.textValue !== defValue.value) {
                    this.textValue = defValue.value;
                }
                if (this.checkValue !== nowValue) {
                    this.checkValue = nowValue;
                }
            } else {
                this.checkValue = null;
            }
            if (this.textValue === undefined || this.textValue === null) {
                this.resetTextValue(this.textValue); // 切换状态的时候清空内容
            }
        },
        /**
         * 重置textValue
         * @param {any} def 默认值
         */
        resetTextValue(def) {
            this.textValue = (this.controlConfig && this.controlConfig.defaultValue) || def; // 切换状态的时候清空内容
        },
        /**
         * 设置myValue
         */
        setMyValue() {
            let res = this.getMyValue();
            let nowValue = this.controlConfig && this.controlConfig.value;
            if (this.checkValue === nowValue) {
                res = {
                    select: this.controlConfig.value,
                    value: this.textValue
                };
            } else {
                res = null;
                this.resetTextValue(null);
            }
            if (res && res.select === nowValue) {
                // this.myValue = res;
                this.setItemValue(res);
            }
        },

        /**
         * 获取当前项的mValue
         * @returns {Object}
         */
        getMValue() {
            let mValue = this.mValue;
            if (this.isVmArray) {
                return this.getItem(mValue);
            } else {
                return mValue;
            }
        },
        /**
         * 获取当前项的myValue
         * @returns {Object}
         */
        getMyValue() {
            if (this.isVmArray) {
                return this.getItem(this.myValue);
            } else {
                return this.myValue;
            }
        },

        /**
         * 设置myValue的值
         * @param {Object} val 更新的值
         */
        setItemValue(val) {
            let myValue = this.myValue;
            let nowValue = this.controlConfig && this.controlConfig.value;
            if (this.isVmArray) {
                myValue = myValue || [];
                if (myValue.length > 0) {
                    let isSet = myValue.every((item, key, _myValue) => {
                        if ((item && item.select) === nowValue) {
                            if (val instanceof Object) { // 除了是对象的其它全部移除
                                _myValue[key] = val;
                            } else {
                                _myValue.splice(key, 1);
                            }
                            return false;
                        }
                        return true;
                    });
                    if (isSet) {
                        myValue.push(val);
                    }
                } else {
                    myValue.push(val);
                }
                this.myValue = myValue;
            } else {
                this.myValue = val;
            }
        },
        /**
         * 获取数组中配置的项
         * @param {Array} list 要配置的数组
         * @returns {Object}
         */
        getItem(list) {
            let nowValue = this.controlConfig && this.controlConfig.value;
            if (list instanceof Array) {
                return list.find(item => {
                    if (item && item.select === nowValue) {
                        return true;
                    }
                });
            }
        }
    }
};
</script>
