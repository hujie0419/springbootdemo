<template>
    <page-box
        @submit="submit"
        class="goodsEdit-pages"
        :title="['模块类型：','商品—普通商品']"
        :showRefresh="false">
        <el-row>
            <el-col :span="21">
                <div class="grid-content bg-purple">
                    <div class="tab-page-title">自定义商品</div>
                </div>
            </el-col>
            <el-col :span="3">
                <div class="grid-content bg-purple-light">
                </div>
            </el-col>
        </el-row>
        <el-row>
            <el-col :span="21">
                <div class="grid-content bg-purple">
                    <form-group :formConfig="formList" @formInit="formInit" @valueChange="valueChange"></form-group>
                </div>
            </el-col>
            <el-col :span="3" class="goods-form-right-btn">
                <div class="goods-form-right">
                    <div class='goods-form-right-btn'>
                        <el-button
                            type="primary"
                            plain
                            @click="search">
                            　查询　
                        </el-button>
                    </div>
                </div>
            </el-col>
        </el-row>

        <goods-edit-table ref='goodsEditTable'
            :form-model='formModel'
            :tagOption='tagOption'
            :changeProductData="innerChangeProductData"
            @updateCheckbox='updateCheckbox'
            @updateProductSortId='updateProductSortId'></goods-edit-table>
    </page-box>
</template>

<script>
import FormGroup from '../../commons/formGroup/FormGroup';
import GoodsEditTable from './goodsEditTable/GoodsEditTable';
import PageBox from '../../commons/pageBox/PageBox';
import { getEditPageConfig } from './config/getEdit.config';
import { getBrandsByCategory, saveProductAssociations } from './common/GoodsApi';
// import TabPage from '../../tabPage/TabPage';
import GoodsExtend from './common/GoodsExtend';

export default {
    // extends: TabPage,
    extends: GoodsExtend,
    components: {
        FormGroup,
        GoodsEditTable,
        PageBox
    },
    data() {
        let _that = this;
        const formList = getEditPageConfig(this.$http);
        return {
            formModel: null,
            formList: formList,
            changeProductData: [],
            innerChangeProductData: []
        };
    },
    mounted() {
        // 进入页面时查询已保存的数据
        // this.$refs.goodsEditTable.searchSavedData(this.formModel);
    },
    methods: {
        // updateEditPageConfig () {
        //     const defaultCategoryValue = this.formModel.get('ProductCategories') ? this.formModel.get('ProductCategories').value : '';
        //     const defaultBrandValue = this.formModel.get('ProductBrand') ? this.formModel.get('ProductBrand').value : '';

        //     const cb = getEditPageConfig({
        //         formModel: this.formModel,
        //         defaultCategoryValue,
        //         defaultBrandValue
        //     });
        //     if (cb instanceof Promise) {
        //         cb.then(config => {
        //             this.formList = config;
        //         });
        //     } else {
        //         this.formList = cb;
        //     }
        // },
        formInit(formModel) {
            this.formModel = formModel;
            // this.updateEditPageConfig();
            this.getBrandList('ProductCategories');
        },
        updateProductSortId () {
            if (this.tagOption.callBackUpdate instanceof Function) {
                this.tagOption.callBackUpdate();
            }
        },
        search() {
            if (!this.$$validMsg(this.formModel)) {
                this.$refs.goodsEditTable.search(this.formModel);
                this.$emit('submit', this.formModel && this.formModel.value);
            }
        },
        submit() {
            this.formModel.isSave = true;
            if (!this.$$validMsg(this.formModel)) {
                // this.$emit('submit', this.formModel && this.formModel.value);
                let tagOption = this.tagOption && this.tagOption.data;
                if (!tagOption || !tagOption.ActivityId || !tagOption.ModuleId) {
                    return;
                }
                const params = {
                    ActivityId: tagOption.ActivityId,
                    ModuleId: tagOption.ModuleId,
                    GeneralProductAssociations: this.changeProductData
                };
                if (!this.changeProductData || this.changeProductData.length < 1) {
                    this.$$errorMsg('未选择数据');
                    return false;
                }
                this.formModel.isSend = true;
                saveProductAssociations(params).subscribe(res => {
                    let _res = res&&res.data;
                    let _rMessage = _res&& _res.ResponseMessage;
                    if (this.$filterResponseCode(_res)) {
                        this.$$saveMsg(_rMessage, {type: 'success'});
                        this.changeProductData = [];
                        this.innerChangeProductData = [];
                        // 保存成功后重新调用查询接口查询商品数据
                        this.$refs.goodsEditTable.search(this.formModel);
                        if (this.tagOption.callBackUpdate instanceof Function) {
                            this.tagOption.callBackUpdate();
                        }
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
        },
        valueChange (con) {
            if (con.controlConfig.controlName === 'ProductCategories') {
                // 根据商品类目更新品牌信息
                this.getBrandList('ProductCategories', 'ProductBrand', null, true);
            }
        },
        updateCheckbox (changeProductData) {
            this.changeProductData = changeProductData;
        }
    },
    created () {
        this.brandConfig = this.formList[1];
    }
};
</script>

<style lang="scss">
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";

.goodsEdit-pages {
    .goods-form {
        padding-top: 20px;
    }
    .goods-form-right-btn {
        position: absolute;
        right: 0px;
        bottom: 0px;
        // left: 0;
    }
    .goods-form-right {
        .goods-form-right-btn {
            bottom: 30px;
        }
    }
}
</style>
