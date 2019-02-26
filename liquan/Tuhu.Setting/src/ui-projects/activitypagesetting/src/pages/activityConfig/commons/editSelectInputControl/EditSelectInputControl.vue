<template>
    <div class='edit-select-input-container'>
        <template v-if='editable'>
            <template v-if="controlConfig && controlConfig.type === 'select'">
                <el-select
                    class='select-wrapper'
                    filterable
                    :multiple='multiple || controlConfig.multiple' v-model="myValue"
                    ref='inputRef'
                    @visible-change="stoptimer"
                    @blur='toggleStatus'>
                    <el-option
                        v-for="item in controlConfig.list"
                        :key="item.value"
                        :label="item.nameText"
                        :value="item.value">
                    </el-option>
                </el-select>
            </template>
            <template v-else>
                <el-input
                    class='input-wrapper'
                    ref='inputRef'
                    size='medium'
                    v-model="myValue"
                    @blur='toggleStatus'>
                </el-input>
            </template>
        </template>
        <template v-else>
            <div @click='toggleStatus'>{{ myValue }}</div>
        </template>
    </div>
</template>

<script>
import ControlExtend from '../formList/common/controlExtend/ControlExtend';

export default {
    extends: ControlExtend,
    props: {
        isEditable: {
            type: Boolean,
            default: false
        }
    },
    data () {
        return {
            editable: this.isEditable,
            multiple: false,
            timer: null
        };
    },
    watch: {
        isEditable(nowVal) {
            if (nowVal) {
                this.editable = nowVal;
            }
        }
    },
    methods: {
        stoptimer(evt) {
            if (evt && this.timer !== null) {
                clearTimeout(this.timer);
                this.timer = null;
            }
        },
        toggleStatus (e) {
            let t = !this.editable? 0 : 200;
            this.timer = setTimeout(() => {
                if (this.timer === null) {
                    return;
                }
                if (!this.myValue) {
                    return;
                }
                this.editable = !this.editable;
                if (this.editable) {
                    setTimeout(() => {
                        this.$refs.inputRef && this.$refs.inputRef.focus();
                    }, 0);
                    if (this.controlConfig && this.controlConfig.type === 'select') {
                        this.$emit('chooseOption');
                    }
                } else {
                    this.$emit('textBlur');
                }
            }, t);
            // let this = this;
            // let _valid = false;
            // if (this.valid instanceof Function) {
            //     _valid = this.valid(this.myValue);
            // }
            // if (_valid) {
            //     if (_valid instanceof Promise) {

            //     } else {
            //         errMsg('PID不正确');
            //     }
            // } else {
            //     setStatus();
            // }

            /**
             * 设置状态
             */
            function setStatus() {

            }
            /**
             * 提示错误信息
             *
             * @param {string} errInfo -错误信息
             */
            function errMsg(errInfo) {
                this.$$errMsg(errInfo);
            }
        }
    }
};
</script>

<style lang='scss'>
.edit-select-input-container {
    .input-wrapper {
        .el-input__inner {
            padding: 0 2px;
        }
    }
    .select-wrapper {
        .el-input__suffix {
            right: 2px;
        }
        .el-input__inner{
            padding:0 15px 0 2px;
        }
        .el-input__icon{
            width: 14px;
        }
    }
}
</style>
