<template>
    <page-box
        :title='title'
        :form-config='formConfig'
        @formInit="formInit"
        @submit='submit'
        @refresh="refreshData">
            <form-group :defaultData="defaultData"
            @formInit="formInit"
            @valueChange="valueChange"
            :form-config='formConfig'></form-group>
        </page-box>
</template>

<script>
import PageBox from '../commons/pageBox/PageBox';
import FormGroup from '../commons/formGroup/FormGroup';
import TabPage from '../tabPage/TabPage';
import {getDrawLotteryConfig} from './config/drawLottery.config';
import apis from '../commons/apis/drawLottery/drawLotteryApi';

export default {
    extends: TabPage,
    data () {
        let _that = this;
        let tagOption = _that.tagOption && _that.tagOption.data;
        let lotteryType = tagOption && (tagOption.SecondaryModuleTypeCode || tagOption.LotteryType);
        return {
            formModel: null,
            title: [
                '模块类型:',
                '抽奖活动'
            ],
            formConfig: getDrawLotteryConfig(this.$http, lotteryType),
            _temconfig: null,
            defaultData: null,
            lotteryType: lotteryType,
            // moduleData: null,
            formUpdata: null,
            formDataCache: this.$$form.initFormCache() // 缓存表单数据
        };
    },
    components: {
        PageBox,
        FormGroup
    },
    mounted() {
        this.getDrawLottery();
    },
    methods: {
        /**
         * 刷新数据
         */
        refreshData() {
            this.getDrawLottery();
        },
        getDrawLottery() {
            this.getModuleData().subscribe(data => {
            // this.moduleData = data.LuckyDrawModule || null;
                let res = data && data.data;
                if (res) {
                    this.formDataCache.setCache([res.LuckyDrawModule || {
                        LotteryType: (res.LuckyDrawModule && res.LuckyDrawModule.LotteryType) || this.lotteryType
                    }], 'LotteryType');
                }
                this.valueChange({
                    controlConfig: {
                        controlName: 'LotteryType'
                    },
                    value: (res && res.LuckyDrawModule && res.LuckyDrawModule.LotteryType) || this.lotteryType
                });
            // if (this.lotteryType === (this.moduleData && this.moduleData.LotteryType)) {
            //     this.defaultData = this.moduleData || null;
            // }
            });
        },
        formInit(formModel) {
            this.formModel = formModel;
            let res = formModel.value;
            this.valueChange({
                controlConfig: {
                    controlName: 'LotteryType'
                },
                value: res.LotteryType
            });
        },
        valueChange(con) {
            let _that = this;
            let formModel = _that.formModel;
            let formDataCache = _that.formDataCache;
            if (!con) {
                return;
            }
            if (con.controlConfig && con.controlConfig.controlName === 'LotteryType') {
                if (!_that._temconfig) {
                    _that._temconfig = _that.formConfig[1];
                }
                if (con.value === 'ENVELOPEDRAW') {
                    if (_that.formConfig.length == 2) {
                        _that.formConfig.pop();
                        formModel.removeItem('SweepstakesId');
                    }
                } else {
                    if (_that.formConfig.length == 1) {
                        _that.formConfig.push(_that._temconfig);
                        _that.formUpdata = _that.$$form.initFormControl(_that._temconfig);
                        formModel.setItem(_that.formUpdata, 'SweepstakesId');
                    }
                    formModel.setValue({ // 清除上次的数据
                        SweepstakesId: ''
                    });
                }
                let cacheData = formDataCache.getCache([formModel.value], 'LotteryType');
                cacheData && formModel.setValue(cacheData[0]); // 设置默认值
            } else {
                formDataCache && formDataCache.setCache([formModel.value], 'LotteryType');
            }
        },
        submit() {
            this.formModel.isSave = true;
            let tagOption = this.tagOption && this.tagOption.data;
            if (!tagOption || !tagOption.ActivityId) {
                return;
            }

            if (!this.$$validMsg(this.formModel)) {
                let _value = this.formModel.value;

                _value=Object.assign(_value, {
                    ActivityId: tagOption.ActivityId,
                    ModuleId: tagOption.ModuleId,
                    ModuleTypeCode: tagOption.SecondaryModuleTypeCode
                });
                this.formModel.isSend = true;
                this.$http.post(apis.SaveSweepstakes, {
                    data: _value
                }).subscribe((res) => {
                    let _res = res&&res.data;
                    let _rMessage = _res&& _res.ResponseMessage;
                    if (this.$filterResponseCode(_res)) {
                        this.$$saveMsg(_rMessage, {type: 'success'});
                    }
                    // else {
                    //     this.$$saveMsg(res.ResponseMessage, {type: 'error'});
                    // }
                }, () => {}, () => {
                    setTimeout(() => {
                        this.formModel.isSend = false;
                    }, 300);
                });
            }
        }
    }
};
</script>
