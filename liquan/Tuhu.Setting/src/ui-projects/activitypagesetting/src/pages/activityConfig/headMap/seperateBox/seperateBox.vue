<template>
    <table-box
        :pageNum="pageNum"
        :pageSize="pageSize"
        :total="total"
        class="seperate-table"
        @valueChange="$emit('updatePage', $event)">
            <el-table
              class='tables-wrap'
              :data="defaultData"
              border
              style="width: 100%"
              header-row-class-name='table-th-row'
              row-class-name='table-td-row'
              @select="select"
              @select-all="selectAll">
              <el-table-column
                type="selection"
                width="55">
              </el-table-column>
              <el-table-column
                prop="CarTypeBrandName"
                label="车型"
                width="180">
              </el-table-column>
              <el-table-column
                prop="ImageUrl"
                label="图片"
                width="200">
                <template slot-scope="scope" v-if='scope.row.ImageUrl'>
                  <div class='table-img'>
                    <img :src='scope.row.ImageUrl' alt='' />
                  </div>
                </template>
              </el-table-column>
              <el-table-column
                prop="ImageUrl"
                label="图片">
                <template slot-scope='scope'>
                  <template v-if='!scope.row.isUpdating'>
                    {{scope.row.ImageUrl}}
                  </template>
                  <template v-else>
                    <form-group class="imgsrc-input" @formInit="formInit" :defaultData="scope.row" @valueChange="valueChange(arguments, scope.row)" :form-config='controlConfig'></form-group>
                  </template>
                </template>
              </el-table-column>
              <el-table-column
                label="操作"
                width="100">
                <template slot-scope="scope">
                  <template v-if='!scope.row.isUpdating'>
                    <el-button @click="update(scope.$index, scope.row)" size="small">修改</el-button>
                  </template>
                  <template v-else>
                    <el-button @click='save(scope.$index, scope.row)' size='small'>保存</el-button>
                  </template>
                  <el-button type="text" size="small" @click='deleteData(scope.$index, scope.row)'>删除</el-button>
                </template>
              </el-table-column>
            </el-table>
          </table-box>
</template>
<script>
import TableBox from '../../commons/tableBox/TableBox';
import FormGroup from '../../commons/formGroup/FormGroup';
export default {
    props: {
        defaultData: {
            type: Array
        },
        pageNum: { // 当前页码
            type: Number
        },
        pageSize: { // 每页数量
            type: Number
        },
        total: { // 总数量
            type: Number
        }
    },
    data() {
        return {
            controlConfig: [{
                nameText: '图片',
                controlName: 'ImageUrl',
                type: 'fileText',
                readonly: true,
                descList: [{
                    action: '',
                    type: 'fileButton',
                    validFile: {
                        type: 'Image',
                        limitMinWidth: 1080,
                        limitMaxWidth: 1080
                    },
                    nameText: '上传图片'
                }]
            }]
        };
    },
    components: {
        TableBox, FormGroup
    },
    methods: {
        valueChange(args, row) {
            row.ImageUrl = args[0].value;
        },
        formInit(formModel) {
            this.formModel = formModel;
        },
        update(index, row) {
            // this.$set(this.defaultData, index, Object.assign({}, this.defaultData[index], {isUpdating: true}));
            this.$emit('update', index);
        },
        deleteData(index, row) {
            this.$emit('deleteData', index, row);
        },
        save(index, row) {
            this.$emit('save', index, row);
        },
        select(selection, row) {
            this.$emit('select', selection);
        },
        selectAll(selection) {
            this.$emit('select', selection);
        }
    }
};
</script>
<style lang="scss">
.seperate-table {
    .el-table .el-table-column--selection {
        text-align: center;
    }
}
.imgsrc-input {
    .control-filter-wrap {
        width: 100%;
    }
    .form-control-filter {
        width: 100%;
    }
}
</style>

