<template>
  <div class="tables-wrap">
        <slot></slot>
        <div class="table-pagination-wrap" v-if="pagination">
            <el-pagination
                class="table-pagination"
                :current-page.sync="myPageNum"
                :page-size="pageSize"
                layout="total, prev, pager, next, jumper"
                :total="total">
            </el-pagination>
        </div>
  </div>
</template>

<script>
export default {
    model: {
        prop: 'pageNum',
        event: 'valueChange'
    },
    props: {
        pagination: { // 是否显示分页
            type: Boolean,
            default: true
        },
        pageNum: { // 当前页码
            type: Number,
            default: 1
        },
        pageSize: { // 每页数量
            type: Number,
            default: 20
        },
        total: { // 总数量
            type: Number,
            default: 0
        }
    },
    data() {
        let _that = this;
        return {
            myPageNum: _that.pageNum // 当前页码
        };
    },
    watch: {
        pageNum(nowVal) {
            this.myPageNum = nowVal || 1;
        },
        myPageNum(newVal) {
            this.$emit('valueChange', newVal);
        }
    },
    methods: {
    }
};
</script>

<style lang="scss">
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";
.tables-wrap {
    margin-top: 20px;
    .table-th-row {
        th{
            background: $grayBg;
            color:$tableColor;
            text-align: center;
            font-weight: bold;
        }
    }
    .table-td-row {
        text-align: center;
    }
    .table-pagination-wrap {
        margin-top: 20px;
        .table-pagination{
            text-align: right;
        }
    }
    .table-img {
        width: 100%;
        img {
            width: 100%;
            display: inline-block;
            line-height: 0;
        }
    }
}
</style>
