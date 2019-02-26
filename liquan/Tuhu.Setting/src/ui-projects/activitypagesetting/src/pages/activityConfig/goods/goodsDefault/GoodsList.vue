<template>
    <div>
        <el-row>
            <el-col :span="21">
                <div class="grid-content bg-purple">
                    <div class="tab-page-title">商品列表</div>
                </div>
            </el-col>
            <el-col :span="3">
                <div class="grid-content bg-purple-light" style="text-align: center;">
                    <el-button @click="toEditPage" type="primary" class="goods-form-right-btn" plain>编辑商品</el-button>
                </div>
            </el-col>
        </el-row>
        <goods-default-table
            :myGoodsDefaultTableData='myGoodsDefaultTableData'
            @pageChange="pageChange"
            :tagOption='tagOption'></goods-default-table>
    </div>
</template>

<script>
    import GoodsDefaultTable from './goodsDefaultTable/GoodsDefaultTable';
    export default {
        components: {
            GoodsDefaultTable
        },
        props: {
            goodsDefaultTableData: {
                type: Object,
                default: function () {
                    return {};
                }
            },
            tagOption: {
                type: Object
            }
        },
        data () {
            return {
                myGoodsDefaultTableData: this.goodsDefaultTableData
            };
        },
        methods: {
            toEditPage () {
                this.$emit('toEditPage');
            },
            pageChange(evt) {
                this.$emit('pageChange', evt);
            }
        },
        watch: {
            goodsDefaultTableData (newVal) {
                this.myGoodsDefaultTableData = newVal;
            }
        }
    };
</script>
