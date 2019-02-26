<template>
    <table-box
        :page-size='pageSize'
        :total='total'
        v-model="pageNum">
        <el-table
            ref="goodsTable"
            :data="tableData"
            :row-class-name="'table-td-row'"
            :header-row-class-name="'table-th-row'"
            border
            style="width: 100%"
            @select="select"
            @select-all='selectAll'>
            <el-table-column
                type="selection"
                width="55">
            </el-table-column>
            <el-table-column
                fixed
                prop="SortGroupId"
                label="组号"
                width="100">
                <template slot-scope="scope">
                    <edit-select-input-control
                        v-model="scope.row.SortGroupId"
                        @textBlur="textBlur(scope.row)"></edit-select-input-control>
                </template>
            </el-table-column>
            <el-table-column
                prop="PID"
                label="PID">
            </el-table-column>
            <el-table-column
                prop="ProductImgUrl"
                label="图片"
                width="140">
                <template slot-scope="scope">
                    <div class="table-img">
                        <img :src="scope.row.ProductImgUrl" alt="">
                    </div>
                </template>
            </el-table-column>
            <el-table-column
                prop="ProductName"
                label="产品名称">
            </el-table-column>
            <el-table-column
                prop="ProductCategory"
                label="类目"
                width="120">
            </el-table-column>
            <el-table-column
                prop="ProductPrice"
                label="价格"
                width="80">
                <template slot-scope="scope">
                    {{scope.row.ProductPrice|filter_money('2.0-2')}}
                </template>
            </el-table-column>
            <el-table-column
                prop="PromotionId"
                label="促销ID">
            </el-table-column>
            <el-table-column
                prop="PromotionTyple"
                label="促销类型-名称"
                width="180">
                <template slot-scope="scope">
                    {{scope.row.PromotionType|filter_promotionTyple}}{{scope.row.PromotionName && (((scope.row.PromotionType && '-') || '') + scope.row.PromotionName)}}
                </template>
            </el-table-column>
            <el-table-column
                prop="Remarks"
                label="备注">
            </el-table-column>
        </el-table>
    </table-box>
</template>

<script>
import TableBox from '../../../commons/tableBox/TableBox';
import { getProducts, editProductSortGroupId } from '../common/GoodsApi';
import EditSelectInputControl from '../../../commons/editSelectInputControl/EditSelectInputControl';
import { CONFIGURATION_CATEGORY } from '../config/goods.config';

// 操作类型
const operationType = {
    true: 'Add',
    false: 'Delete'
};
// 唯一标识每行数据对象的key
const rowUniqueKey = 'PID';

// 更新组号的更新事件
const UPDATE_PRODUCT_SORT_EVENT = 'updateProductSortId';

// 更新checkbox的事件名称
const UPDATE_CHECKBOX_EVENT = 'updateCheckbox';

export default {
    props: {
        formModel: {
            type: Object,
            default: function () {
                return {};
            }
        },
        tagOption: {
            type: Object
        },
        changeProductData: {
            type: Array
        }
    },
    data() {
        return {
            pageNum: 1, // 当前页码
            pageSize: 20, // 每页数量
            total: 0, // 总数量
            tableData: [],
            myFormModel: this.formModel
            // changeProductData: []
        };
    },
    methods: {
        /**
         * 组装查询商品列表的请求参数
         * @returns {Object} 返回拼接好的请求参数
         */
        generateParams () {
            let tagOption = this.tagOption && this.tagOption.data;
            if (!tagOption || !tagOption.ActivityId || !tagOption.ModuleId) {
                return;
            }
            const _value = this.myFormModel.value;
            return {
                ActivityId: tagOption.ActivityId,
                ModuleId: tagOption.ModuleId,
                ProductBrand: _value.ProductBrand === -1 ? '' : _value.ProductBrand,
                ProductCategoriesId: _value.ProductCategories,
                ProductCategoriesName: CONFIGURATION_CATEGORY[_value.ProductCategories],
                // ProductBrand: _value.ProductBrand,
                PID: _value.PID,
                SalesPromotion: _value.SalesPromotion,
                PromotionTyple: _value.PromotionTyple,
                PageIndex: this.pageNum,
                PageSize: this.pageSize
            };
        },
        /**
         * 根据条件查询商品列表
         * @param {object} formModel 查询条件的值得表单对象
         */
        search (formModel) {
            const _that = this;
            _that.myFormModel = formModel || _that.formModel;
            const params = this.generateParams();
            if (params) {
                // 参数校验逻辑：已选商品类目，则品牌必选；如果商品类目和品牌都不选，则PID或促销ID至少填一个
                if (params.ProductCategoriesName && !params.ProductBrand) {
                    this.$$errorMsg('当选中类目时品牌必填');
                    return;
                }
                if (!params.ProductCategoriesName && !params.PID && !params.SalesPromotion) {
                    this.$$errorMsg('查询条件不可为空');
                    return;
                }
                getProducts(params)
                    .subscribe(res => {
                        const _res = res && res.data;
                        _that.total = _res.TotalCount || 0;
                        _that.tableData = _res.GeneralProducts || [];
                    });
            }
        },
        /**
         * 进入页面时查询已保存的数据
         * @param {object} formModel 查询条件的值得表单对象
         */
        searchSavedData (formModel) {
            const _that = this;
            _that.myFormModel = formModel || _that.formModel;
            const params = this.generateParams();
            if (params) {
                getProducts(params)
                    .subscribe(res => {
                        const _res = res && res.data;
                        _that.total = _res.TotalCount || 0;
                        _that.tableData = _res.GeneralProducts || [];
                    });
            }
        },
        /**
         * 编辑关联商品组号
         * @param {object} row 当前编辑行的数据对象
         */
        textBlur (row) {
            let tagOption = this.tagOption && this.tagOption.data;
            if (!tagOption || !tagOption.ActivityId || !tagOption.ModuleId) {
                return;
            }
            const params = {
                ActivityId: tagOption.ActivityId,
                ModuleId: tagOption.ModuleId,
                PKID: row.PKID,
                SortGroupId: row.SortGroupId
            };
            editProductSortGroupId(params)
                .subscribe(back => {
                    let res = back && back.data;
                    this.$$saveMsg(res && res.ResponseMessage, {
                        type: 'success'
                    });
                    this.search();
                    this.$emit(UPDATE_PRODUCT_SORT_EVENT);
                });
        },
        findIndex(arr, obj, key) {
            let index = -1;
            if (arr && arr.length > 0) {
                index = arr.findIndex((item, index) => {
                    return item[key] === obj[key];
                });
            }
            return index;
        },
        /**
         * 改变某行checkbox，激活该回调方法
         * @param {array} selection checkbox勾选的row数据数组
         * @param {object} row 触发checkbox的row数据对象
         */
        select (selection, row) {
            let checked = this.findIndex(selection, row, rowUniqueKey) >= 0;
            const index = this.findIndex(this.changeProductData, row, rowUniqueKey);
            if (index > -1) {
                this.changeProductData.splice(index, 1);
            }
            if (row.checked !== checked) {
                row.OperationType = operationType[checked];
                this.changeProductData.push(row);
            }
        },
        /**
         * 选中或取消全部checkbox，激活该回调方法
         * @param {array} selection checkbox勾选的row数据数组
         */
        selectAll(selection) {
            const _that = this;
            let checked = selection && selection.length > 0;
            _that.tableData.forEach(item => {
                const index = _that.findIndex(_that.changeProductData, item, rowUniqueKey);
                if (index > -1) {
                    _that.changeProductData.splice(index, 1);
                }

                if (item.checked !== checked) {
                    item.OperationType = operationType[checked];
                    item.PKID = item.PKID || '0';
                    _that.changeProductData.push(item);
                }
            });
        }
    },
    watch: {
        formModel (newVal, oldVal) {
            // this.search(newVal);
        },
        pageNum (newVal) {
            this.search();
        },
        tableData (newVal) {
            this.$nextTick(() => {
                newVal && newVal.forEach(item => {
                    item.checked = !!item.SortGroupId;
                    this.$refs.goodsTable.toggleRowSelection(item, !!item.SortGroupId);
                });
            });
        },
        changeProductData (newVal) {
            this.$emit(UPDATE_CHECKBOX_EVENT, newVal);
        }
    },
    components: {
        EditSelectInputControl,
        TableBox
    }
};
</script>

<style lang="scss">
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";

</style>
