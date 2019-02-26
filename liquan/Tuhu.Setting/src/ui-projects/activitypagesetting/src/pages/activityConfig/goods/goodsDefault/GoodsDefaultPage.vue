<template>
    <page-box
        class="goodsDefault-pages"
        :showBtn="false"
        :title="['模块类型：','商品—普通商品']"
        @refresh="refreshData">
        <el-row>
            <el-col :span="21">
                <div class="grid-content bg-purple">
                    <div class="tab-page-title">展示配置</div>
                </div>
            </el-col>
            <el-col :span="3">
                <div class="grid-content bg-purple-light">
                </div>
            </el-col>
        </el-row>
        <el-row>
            <div class="grid-content bg-purple">
                <div class="goods-form">
                    <form-group @valueChange="valueChange" :formConfig="formList" @formInit="formInit"></form-group>
                </div>
            </div>
            <div class='save-btn'>
                <el-button type="primary" @click="submit()">保存</el-button>
            </div>
        </el-row>
        <el-row>
            <template v-if="configMethod === CONFIGURATION_MODE.CUSTOM_GOODS">
                <goods-list
                    :goods-default-table-data="goodsDefaultTableData"
                    :tagOption='tagOption'
                    @pageChange="getFormModelDefaultValue"
                    @toEditPage='toEditPage'></goods-list>
            </template>
        </el-row>
    </page-box>
</template>

<script>
/*
eslint-disable max-lines
*/
import GoodsExtend from './common/GoodsExtend';
import FormGroup from '../../commons/formGroup/FormGroup';
import PageBox from '../../commons/pageBox/PageBox';
import GoodsEditPage from './GoodsEditPage';
import GoodsList from './GoodsList';
import { getBrandCat, getActivityConfig, defaultConfig, CONFIGURATION_MODE, CONFIGURATION_CATEGORY } from './config/goods.config';
import apis from '../../commons/apis/goods/goodsApi';
import { getBrandsByCategory, getProductAssociations } from './common/GoodsApi';

export default {
    extends: GoodsExtend,
    components: {
        FormGroup,
        PageBox,
        GoodsEditPage,
        GoodsList
    },
    data() {
        let formList = defaultConfig();
        return {
            formList: formList,
            defaultLen: formList.length,
            configMethod: CONFIGURATION_MODE.CATEGORY_BRAND,
            CONFIGURATION_MODE,
            goodsDefaultTableData: {},
            defaultData: null,
            updataItem: null,
            lock: false
        };
    },
    // watch: {
    //     tagOption() {
    //         this.getFormModelDefaultValue(1, (res) => {
    //             this.setFormData();
    //         });
    //     }
    // },
    mounted() {
        this.lock = true;
        this.getFormModelDefaultValue(1, (res) => {
            this.setFormData();
            setTimeout(() => {
                this.lock=false;
            }, 200);
        });
    },
    methods: {
        /**
         * 刷新数据
         */
        refreshData() {
            this.lock = true;
            this.getFormModelDefaultValue(1, (res) => {
                this.setFormData();
                setTimeout(() => {
                    this.lock=false;
                }, 200);
            });
        },
        /**
         * 设置表单默认数据
         */
        setFormData() {
            let _that = this;
            let res = _that.defaultData;
            let rowNum = res && res.OrdinaryRowNumber + '';
            if (!_that.formModel || !res) {
                return;
            }
            if (res && res.IsBrandName) {
                res.MoreType = res.MoreType || [];
                res.MoreType[0] = '1';
            }
            if (res && res.IsProgressBar) {
                res.MoreType = res.MoreType || [];
                res.MoreType[1] = '2';
            }

            let defaultData = res && Object.assign({}, res, {
                OrdinaryColumnNumber: (res.OrdinaryColumnNumber || 2) + '',
                OrdinaryRowNumber: {
                    select: rowNum === '0' ? '0' : '1',
                    value: rowNum
                },
                Template: res.Template || 'Minimalist',
                ConfigurationMode: res.ConfigurationMode || CONFIGURATION_MODE.CATEGORY_BRAND,
                MoreType: (res && res.MoreType) || []
            }); // 初始化的时候的表单默认值
            _that.formModel && _that.formModel.setValue(defaultData);
            let val = _that.formModel && _that.formModel.value;
            _that.brandConfig && _that.$set(_that.brandConfig, 'list', [{
                nameText: '请选择',
                value: ''
            }]);
            _that.setOrdinaryColumnNumber();
            _that.setTemplate();
            // 默认值为极简模式，更多-》进度条disabled
            if (res && res.ConfigurationBrand && _that.formModel) {
                _that.getBrandList('ConfigurationCategoryId', 'ConfigurationBrand', function() {
                    setTimeout(() => {
                        _that.formModel.setValue({
                            ConfigurationBrand: res.ConfigurationBrand
                        });
                    }, 200);
                });
            } else {
                _that.setConfigurationMode(() => {
                    setTimeout(() => {
                        if (res && res.ConfigurationActivityId && _that.formModel) {
                            _that.formModel && _that.formModel.setValue({
                                ConfigurationActivityId: res.ConfigurationActivityId
                            });
                        }
                    }, 200);
                });
            }
        },

        /**
         * 默认普通商品模块关联商品查询接口
         * @param {Number} pageNum 页码
         * @param {Function} cb 完成后的回调
         */
        getFormModelDefaultValue (pageNum, cb) {
            let _that = this;
            let tagOption = _that.tagOption && _that.tagOption.data;
            if (!tagOption || !tagOption.ActivityId || !tagOption.ModuleId) {
                return;
            }
            const params = {
                ActivityId: tagOption.ActivityId,
                ModuleId: tagOption.ModuleId,
                PageIndex: pageNum || 1,
                PageSize: 20
            };
            getProductAssociations(params).subscribe(res => {
                let _res = res&&res.data;
                let _rMessage = _res&& _res.ResponseMessage;
                if (_that.$filterResponseCode(_res)) {
                    let _resd = _res || {};
                    if (typeof _resd.ConfigurationCategoryId !== 'undefined' && !_resd.ConfigurationCategoryId) {
                        delete _resd.ConfigurationCategoryId;
                    }
                    if (typeof _resd.ConfigurationBrand !== 'undefined' && !_resd.ConfigurationBrand) {
                        delete _resd.ConfigurationBrand;
                    }
                    _that.defaultData = _resd;
                    if (cb instanceof Function) {
                        cb(_resd);
                    }
                    _that.goodsDefaultTableData = {
                        TotalCount: _resd.TotalCount || 0,
                        GeneralProductAssociations: _resd.generalProductAssList || []
                    };
                }
            });
        },
        /**
         * 表单项数据发生改变
         * @param {Object} con 当前发生改变项的数据
         */
        valueChange(con) {
            let _that = this;
            let controlConfig = con && con.controlConfig;
            let controlName = controlConfig && controlConfig.controlName;
            let formModel = _that.formModel;
            switch (controlName) {
                case 'ConfigurationMode': // 配置方式改变
                    _that.setConfigurationMode();
                    break;
                case 'ConfigurationCategoryId': // 商品类目改变
                    if (!this.lock) {
                        formModel.setValue({ // 清除上次选中的品牌
                            ConfigurationBrand: ''
                        });
                        _that.getBrandList('ConfigurationCategoryId', 'ConfigurationBrand');
                    }
                    break;
                case 'Template': // 模板样式改变
                    _that.setTemplate();
                    break;
                case 'OrdinaryColumnNumber': // 每行数量修改
                    _that.setOrdinaryColumnNumber();
                    break;
            }
        },
        /**
         * 根据模板样式设置对应的选项
         * @returns {boolean}
         */
        setTemplate() {
            let _that = this;
            let con = this.formModel.get('Template');
            if (!con || !con.value) {
                return false;
            }
            let moreType = this.formModel.get('MoreType');
            let columnNumber = this.formModel.get('OrdinaryColumnNumber');
            if (con.value === 'Minimalist') {
                this.$set(this.formList[2].list[1], 'disabled', true);
                moreType.setValue([moreType.value[0]]);
            } else {
                if (columnNumber.value !== '3') { // 非极简版且列数不为3，才能选择进度条
                    this.$set(this.formList[2].list[1], 'disabled', false);
                }
            }
        },
        /**
         * 根据每列数量设置对应的选项
         *
         * @returns {boolean}
         */
        setOrdinaryColumnNumber() {
            let _that = this;
            let con = this.formModel.get('OrdinaryColumnNumber');
            let template = this.formModel.get('Template');
            let moreType = this.formModel.get('MoreType');
            if (!con || !con.value) {
                return false;
            }

            // 一列时，可选无促销语版
            if (con.value + '' === '1') {
                this.$set(this.formList[1].list[2], 'disabled', false);
            } else {
                this.$set(this.formList[1].list[2], 'disabled', true);
                if (template.value === 'NoPromotion') {
                    template.setValue('Minimalist'); // 每行列数改变，模版样式置为默认极简版
                }
            }
            // 三列时，完整版不可选，进度条不可选
            if (con.value + '' === '3') {
                this.$set(this.formList[1].list[3], 'disabled', true);
                this.$set(this.formList[2].list[1], 'disabled', true);
                if (template.value === 'Complete') {
                    template.setValue('Minimalist'); // 每行列数改变，模版样式置为默认极简版
                }
                let index = moreType.value.indexOf('2');
                if (index > -1) {
                    moreType.value.splice(index, 1); // 列数改变，去掉进度条
                }
            } else {
                this.$set(this.formList[1].list[3], 'disabled', false);
                if (template.value !== 'Minimalist') { // 非极简版且列数不为3，才能选择进度条
                    this.$set(this.formList[2].list[1], 'disabled', false);
                }
            }
        },
        /**
         * 根据配置方式设置对应的选项
         * @param {Function} cb 回调
         */
        setConfigurationMode(cb) {
            let _that = this;
            let con = this.formModel.get('ConfigurationMode');
            if (con && con.value) {
                let deletItem = _that.formList.splice(_that.defaultLen);
                let list;
                deletItem && deletItem.forEach(item => { // 移除formModel里的项
                    this.formModel.removeItem(item.controlName);
                });
                _that.configMethod = con.value;
                switch (con.value) {
                    case CONFIGURATION_MODE.CATEGORY_BRAND: // 类目品牌
                        list = getBrandCat(); // 商品品牌选择列表
                        _that.brandConfig = list[1];
                        _that.brandConfig && _that.$set(_that.brandConfig, 'list', [{
                            nameText: '请选择',
                            value: ''
                        }]);
                        break;
                    case CONFIGURATION_MODE.ACTIVE_CONFIGURATION: // 活动配置
                        list = getActivityConfig(_that.$http); // 活动ID项
                        break;
                }
                if (list) { // 需要增加的列表
                    _that.updataItem = list;
                    _that.formList = _that.formList.concat(list);
                    _that.updataItem = _that.$$form.initFormData(list);
                    _that.formModel.merge(_that.updataItem);
                    if (cb instanceof Function) {
                        cb();
                    }
                }
            }
        },

        /**
         * 跳过到编辑商品页
         */
        toEditPage() {
            const _that = this;
            let tagOption = this.tagOption && this.tagOption.data;
            if (!tagOption || !tagOption.ModuleId) {
                return;
            }
            this.$$tabs.addTab('goodsEdit', {
                title: '编辑商品',
                pageId: tagOption.ModuleId,
                data: Object.assign({}, tagOption),
                callBackUpdate: () => {
                    _that.getFormModelDefaultValue();
                }
            });
        },
        formInit(formModel) {
            const _that = this;
            _that.formModel = formModel;
            _that.setConfigurationMode();
            _that.setFormData();
        },

        submit() {
            this.formModel.isSave = true;
            if (!this.$$validMsg(this.formModel)) {
                let _value = this.formModel.value;
                let tagOption = this.tagOption && this.tagOption.data;
                if (!tagOption || !tagOption.ActivityId || !tagOption.ModuleId) {
                    return;
                }
                let OrRowNum = _value.OrdinaryRowNumber;
                let MoreType = _value.MoreType;
                let params = Object.assign({}, _value, {
                    ConfigurationBrand: _value.ConfigurationBrand === -1 ? '' : _value.ConfigurationBrand,
                    ConfigurationCategoryId: _value.ConfigurationCategoryId === -1 ? '' : _value.ConfigurationCategoryId,
                    ActivityId: tagOption.ActivityId,
                    ModuleId: tagOption.ModuleId,
                    ModuleTypeCode: tagOption.SecondaryModuleTypeCode,
                    IsBrandName: MoreType && MoreType.includes('1'),
                    IsProgressBar: MoreType && MoreType.includes('2'),
                    OrdinaryRowNumber: OrRowNum.value,
                    ConfigurationCategory: CONFIGURATION_CATEGORY[_value.ConfigurationCategoryId],
                    ConfigurationActivityId: _value.ConfigurationMode === CONFIGURATION_MODE.ACTIVE_CONFIGURATION
                        ? _value.ConfigurationActivityId : ''
                });
                delete params.MoreType;
                this.formModel.isSend = true;
                this.$http.post(apis.SaveGeneralProductSetting, {
                    data: params
                }).subscribe((res) => {
                    let _res = res&&res.data;
                    let _rMessage = _res&& _res.ResponseMessage;
                    if (this.$filterResponseCode(_res)) {
                        this.$$saveMsg(_rMessage, {type: 'success'});
                        if (this.tagOption && this.tagOption.callBackUpdate instanceof Function) {
                            this.tagOption.callBackUpdate({tabName: this.tagOption.name, activityid: this.tagOption.ActivityId});
                        }
                    }
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

<style lang="scss">
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";

.goodsDefault-pages {
    .goods-form {
        padding-top: 20px;
    }
    .goods-form-right-btn {
        position: absolute;
        right: 0px;
        bottom: 10px;
        // left: 0;
    }
    .goods-form-right {
        .goods-form-right-btn {
            bottom: 30px;
        }
    }
    .radio-text-control {
        .el-input{
            width: 50px;
            .el-input__inner {
                text-align: center;
                padding: 0 2px;
            }
        }
    }
}
</style>
