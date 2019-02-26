<template>
    <table-box
        :page-size='pageSize'
        :total='total'
        v-model="pageNum">
        <el-table
            :data="tableData"
            :row-class-name="'table-td-row'"
            :header-row-class-name="'table-th-row'"
            border
            style="width: 100%">
            <el-table-column
                fixed
                prop="SortGroupId"
                label="组号"
                width="100">
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
                prop="PromotionName"
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
import { getProductAssociations } from '../common/GoodsApi';

export default {
    components: {
        TableBox
    },
    props: {
        myGoodsDefaultTableData: {
            type: Object,
            default: function () {
                return {};
            }
        },
        tagOption: {
            type: Object
        }
    },
    data() {
        return {
            pageNum: 1, // 当前页码
            pageSize: 20, // 每页数量
            total: this.myGoodsDefaultTableData.TotalCount || 0, // 总数量
            tableData: this.myGoodsDefaultTableData.GeneralProductAssociations || []
        };
    },
    methods: {
        // getGeneralProductAssociations () {
        //     const _that = this;
        //     let tagOption = _that.tagOption && _that.tagOption.data;
        //     if (!tagOption || !tagOption.ActivityId || !tagOption.ModuleId) {
        //         return;
        //     }
        //     const params = {
        //         ActivityId: tagOption.ActivityId,
        //         ModuleId: tagOption.ModuleId,
        //         PageIndex: _that.pageNum,
        //         PageSize: _that.pageSize
        //     };
        //     getProductAssociations(params)
        //         .subscribe(res => {
        //             const _res = res || {};
        //             _that.total = _res.TotalCount || 0;
        //             _that.tableData = _res.GeneralProductAssociations || [];
        //         });
        // }
    },
    watch: {
        pageNum (newVal) {
            // this.getGeneralProductAssociations();
            this.$emit('pageChange', newVal);
        },
        myGoodsDefaultTableData (newVal) {
            this.tableData = newVal.GeneralProductAssociations;
            this.total = newVal.TotalCount;
        }
    }
};
</script>

<style lang="scss">
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";

</style>
