<template>
  <div class="myTable">
    <el-table
      ref="multipleTable"
      :data="tableData"
      tooltip-effect="dark"
      border
      @selection-change="handleSelectionChange">
      <el-table-column
        type="selection">
      </el-table-column>
      <el-table-column
        prop="Brand"
        label="汽车品牌">
      </el-table-column>
      <el-table-column
        prop="Vehicle"
        label="车系">
      </el-table-column>
      <el-table-column
        prop="VehicleBodyType"
        label="车身类别">
      </el-table-column>
      <el-table-column
        prop="AvgPrice"
        label="车价">
      </el-table-column>
      <el-table-column
        v-if="isOil"
        label="原厂粘度">
        <template slot-scope="scope">{{ scope.row.Viscosity}}</template>
      </el-table-column>
      <el-table-column
        v-if="isOil"
        label="新粘度">
        <template slot-scope="scope">{{ scope.row.NewViscosity}}</template>
      </el-table-column>
      <el-table-column
        v-if="isOil"
        label="适配机油等级">
        <template slot-scope="scope">{{ scope.row.Grade}}</template>
      </el-table-column>
      <el-table-column
        label="优先级"
        min-width="200">
        <template slot-scope="scope">
          <table class="smallTable" v-if="scope.row.Details && scope.row.Details.length">
            <tr v-for="(item,index) in scope.row.Details" :key="index">
              <td>优先级{{item.Seq}}</td>
              <td v-if="isOil">{{item.Grade}}</td>
              <td>{{item.Brand}}</td>
              <td>{{item.Series}}</td>
            </tr>
          </table>
          <table  class="smallTable" v-else><tr><td></td></tr></table>
        </template>
      </el-table-column>
      <el-table-column
        label="操作"
        width="200"
        fixed="right">
        <template slot-scope="scope">
          <ul class="tableBtn">
            <li @click="editSelect([scope.row],scope.$index)">编辑</li>
            <li class="disabled" v-if="!scope.row.Details ||!scope.row.Details.length">暂无</li>
            <li v-else @click="setDisabled(scope.row)">{{scope.row.IsEnabled?"禁用":"启用"}}</li>
            <li :class="{disabled:!scope.row.Details || !scope.row.Details.length}" @click="deleteRow([scope.row])">删除</li>
          </ul>
        </template>
      </el-table-column>
    </el-table>
    <div class="pagination">
      <ul class="paginationBtn">
        <li @click="handleCurrentChange(1)">首页</li>
        <li @click="handleCurrentChange(pageIndex - 1)">上一页</li>
        <li @click="handleCurrentChange(pageIndex + 1)">下一页</li>
        <li @click="handleCurrentChange(Math.ceil(tableDataTotal/pageSize))">尾页</li>
      </ul>
      <el-pagination
        @size-change="handleSizeChange"
        @current-change="handleCurrentChange"
        :current-page="pageIndex"
        :page-sizes="[100, 500, 1000]"
        :page-size="pageSize"
        layout="jumper,sizes, prev, pager, next"
        :total="tableDataTotal">
      </el-pagination>
    </div>
    <div>当前第{{pageIndex}}页 共{{Math.ceil(tableDataTotal/pageSize)}}页 共{{tableDataTotal}}条记录</div>
    <el-dialog width="800px" title="特殊车型配置" :visible.sync="outerVisible"  :close-on-click-modal="false">
      <edit-select 
        :oilGradeList='oilGradeList'
        :productBrandList='productBrandList'
        :oilSeriesList='oilSeriesList'
        :Viscosity="NewViscosity"
        :Viscositylist="Viscositylist"
        :selected='selected'
        :brandAndSeries="brandAndSeries"
        :isOil="isOil"
        ref="editSelectCom">
        </edit-select>
      <div slot="footer" class="dialog-footer">
        <el-switch v-model="selectedDisabled" :width="50" active-color="#DF3348" inactive-color="#fff"></el-switch>
        {{selectedDisabled?"启用":"禁用"}}
        <el-button @click="cancel">取 消</el-button>
        <el-button type="primary" @click="submit">提交</el-button>
      </div>
    </el-dialog>
  </div>
</template>
<script>
import EditSelect from './EditSelect';
export default {
  props: {
    tableData: {
      type: Array,
      default: () => {
        return [];
      }
    },
    tableDataTotal: {
      type: Number,
      default: 0
    },
    oilGradeList: {
      type: Array,
      default: () => {
        return [];
      }
    },
    productBrandList: {
      type: Array,
      default: () => {
        return [];
      }
    },
    oilSeriesList: {
      type: Array,
      default: () => {
        return [];
      }
    },
    Viscositylist: {
      type: Array,
      default: () => {
        return [];
      }
    },
    isOil: {
      type: Boolean,
      default: false // 0 不是机油，1是机油
    },
    pageSize: {
      type: Number,
      default: 0
    },
    pageIndex: {
      type: Number,
      default: 0
    },
    brandAndSeries: {
      type: Object,
      default: {}
    }
  },
  data() {
    return {
      multipleSelection: [], // 多选数据
      NewViscosity: '', // 新粘度
      myPageSize: this.pageSize,
      myPageIndex: this.myPageIndex,
      selected: [],
      outerVisible: false,
      selectedDisabled: false
    };
  },
  watch: {
    pageSize(val) {
      this.myPageSize = val;
    },
    pageIndex(val) {
      this.myPageSize = val;
    }
  },
  methods: {
    /**
     * 编辑
     * @param {Array} rowList - 当前item组成的数组，为了方便加入多选
     * @param {Number} i - 当前index
     */
    editSelect(rowList, i) {
      this.$refs.multipleTable.clearSelection();
      this.index = i;
      this.openEditSelect(rowList, rowList[0].Details.length ? rowList[0].Details : [{}]);
      this.selectedDisabled = rowList[0].Details.length ? rowList[0].IsEnabled : true;
    },
    /**
     * 显示编辑面板
     * @param {Array} rowList - 当前item组成的数组，为了方便加入多选
     * @param {Object} selected -当前item 的 编辑数据details
     */
    openEditSelect(rowList, selected) {
      if (!selected) { // 多选
        this.selected=[{}];
        rowList = this.multipleSelection;
        this.selectedDisabled = true;
        this.NewViscosity = '';
      } else { // 单行编辑
        this.selected=selected;
        this.NewViscosity = rowList[0].NewViscosity;
      }
      if (!rowList.length) {
        this.$message({type: 'info', message: '请至少选择一个车型进行操作'});
        return;
      }
      this.outerVisible = true;
      this.$refs.editSelectCom && this.$refs.editSelectCom.updateSelect();
    },
    /**
     * 删除
     * @param {Array} rowList - 当前item组成的数组，为了方便加入多选
     */
    deleteRow(rowList) {
      if (!rowList) rowList = this.multipleSelection;
      if (!rowList.length) {
        this.$message({type: 'info', message: '请至少选择一个车型进行操作'});
        return;
      }
      this.$confirm('确认删除配置的优先级?', '删除确认?', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning',
        closeOnClickModal: false
      }).then(() => {
        let areaOilIds = [];
        let VehicleIds=[];
        rowList.forEach(item => {
          if (item.Details) {
            areaOilIds.push(item.AreaOilId);
            VehicleIds.push(item.VehicleId);
          }
        });
        let data = {
          areaOilIds: areaOilIds,
          VehicleIds: VehicleIds
        };
        const loading = this.$loading({text: 'Loading', spinner: 'el-icon-loading', background: 'rgba(0, 0, 0, 0.7)'});
        let url = '/BaoYangRecommend/DeleteProductPriorityAreaDetail';
        if (this.isOil) url = '/BaoYangRecommend/DeleOilProductPriorityAreaOil';
        this.$http.post(url, {
          apiServer: 'apiServer',
          data: data
        }).subscribe((res) => {
          loading.close();
          if (res.Status) {
            rowList.forEach(item => {
              item.NewViscosity = '';
              item.Details=[];
            });
            this.$message({type: 'success', message: '删除成功!'});
          } else {
            this.$message({message: res.Msg || '删除失败', type: 'error', dangerouslyUseHTMLString: true});
          }
        });
      }).catch(() => {});
    },
    /**
     * 启用&禁用
     * @param {Array} row - 当前item
     */
    setDisabled(row) {
      let flagText = row.IsEnabled?'禁用':'启用';
      this.$confirm('是否'+flagText+'优先级配置', '操作确认', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'info',
        closeOnClickModal: false
      }).then(() => {
        let data = {
          'AreaOilId': row.AreaOilId,
          'VehicleId': row.VehicleId,
          'IsEnabled': !row.IsEnabled
        };
        const loading = this.$loading({text: 'Loading', spinner: 'el-icon-loading', background: 'rgba(0, 0, 0, 0.7)'});
        let url = '/BaoYangRecommend/UpdateAreaDetailEnabled';
        if (this.isOil) url = '/BaoYangRecommend/UpdateOilAreaEnabled';
        this.$http.post(url, {
          apiServer: 'apiServer',
          data: data
        }).subscribe((res) => {
          loading.close();
          if (res.Status) {
            row.IsEnabled = !row.IsEnabled;
            this.$message({type: 'success', message: flagText + '成功!'});
          } else {
            this.$message({message: res.Msg || flagText + '失败', type: 'error', dangerouslyUseHTMLString: true});
          }
        });
      }).catch(() => {});
    },
    /**
     * ele table 多选 回调
     * @param {Array} val - 多选数据
     */
    handleSelectionChange(val) {
      this.multipleSelection = val;
    },
    /**
     * pageSize更新
     * @param {Number} val - 当前
     */
    handleSizeChange(val) {
      this.$emit('update:pageSize', val);
      this.$emit('update:pageIndex', 1);
      this.$emit('click');
    },
    /**
     * pageIndex更新
     * @param {Number} val - 当前
     */
    handleCurrentChange(val) {
      if (val && this.pageIndex !== val && val < Math.ceil(this.tableDataTotal/this.pageSize) + 1) {
        this.$emit('update:pageIndex', val);
        this.$emit('click');
      }
    },
    cancel() {
      this.outerVisible = false;
      this.$refs.editSelectCom.clearNewViscosity();
    },
    /**
     * 编辑面板 点击 提交
     */
    submit() {
      this.$confirm('确认编辑优先级?', '确认操作?', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'info',
        closeOnClickModal: false
      }).then(() => {
        this.save();
      }).catch(() => {});
    },
    /**
     * 更新 表格信息
     */
    save() {
      let selected = this.$refs.editSelectCom.submit();
      if (!selected) {
        return;
      }
      selected.forEach((item, index) => {
        item.Seq = index + 1;
      });
      let tableDataList = [];
      let submitTableDataList=[];
      if (this.multipleSelection.length) {
        tableDataList = this.multipleSelection;
      } else {
        tableDataList.push(this.tableData[this.index]);
      }
      submitTableDataList = JSON.parse(JSON.stringify(tableDataList));
      submitTableDataList.forEach((item, index) => {
        item.NewViscosity = this.$refs.editSelectCom.getNewViscosity();
        item.Details = [];
        item.IsEnabled = this.selectedDisabled;
      });
      let data = {
        Views: submitTableDataList,
        Details: selected,
        AreaId: this.$route.query.areaId,
        PartName: this.$route.query.partName
      };
      const loading = this.$loading({text: 'Loading', spinner: 'el-icon-loading', background: 'rgba(0, 0, 0, 0.7)'});
      let url = '/BaoYangRecommend/SaveVehicleProductPriority';
      if (this.isOil) url = '/BaoYangRecommend/SaveVehicleOilProductPriority';
      this.$http.post(url, {
        apiServer: 'apiServer',
        data: data
      }).subscribe((res) => {
        if (res.Status === true) {
          this.outerVisible = false;
          // this.productSeriesList = res.data;
          if (tableDataList.length === 1) {
            this.$set(tableDataList[0], 'NewViscosity', this.$refs.editSelectCom.getNewViscosity());
            this.$set(tableDataList[0], 'Details', selected);
            this.$set(tableDataList[0], 'IsEnabled', this.selectedDisabled);
            this.$set(tableDataList[0], 'AreaOilId', res.Data?res.Data[0]:0);
            loading.close();
          } else {
            // 多选编辑保存，由于读写库问题，延迟2s请求查询
            setTimeout(() => {
              this.$emit('click');
            }, 2000);
          }
          this.$refs.editSelectCom.clearNewViscosity();
          this.$message({type: 'success', message: '编辑成功!'});
        } else {
          loading.close();
          this.$message({message: res.Msg || '删除失败', type: 'error', dangerouslyUseHTMLString: true});
        }
      });
    }
  },
  components: {
    'edit-select': EditSelect
  }
};
</script>
<style lang="scss" scoped>
@import "css/common/_var.scss";
.myTable{
  margin: 0 20px;
  color: $color3;
  /deep/ .el-table{
    thead{
      color: $color3;
      font-weight: bold;
    }
    .smallTable{
      width: calc(100% + 20px);
      margin: 0 -10px;
      tr{
        background: rgba(0,0,0,0) !important;
        td{
          width: 25%;
          height: 44px;
          padding: 10px;
          text-align: center;
          &:last-child{
            border-right: 0 !important;
          }
        }
        &:hover>td{
          background: rgba(0,0,0,0) !important;
        }
        &:last-child td{
          border-bottom: 0 !important;
        }
      }
    }
    .tableBtn{
      display: inline-block;
      overflow: hidden;
      border: 1px solid $colore;
      background-color: $colorfa;
      vertical-align: middle;
      li{
        float: left;
        padding: 3px 10px;
        border-left: 1px solid $colore;
        cursor: pointer;
        &:first-child{
          border: 0;
        }
        &.disabled{
          color: $color9;
        }
      }
    }
    td{
      padding: 0;
    }
    .cell{
      text-align: center;
    }
  }
  .pagination{
    height: 36px;
    line-height: 32px;
    padding: 2px 5px 2px 0;
    display: flex;
    margin-top:2rem;
    .paginationBtn{
      line-height: 12px;
      overflow: hidden;
      border: 1px solid $colore;
      background-color: $colorfa;
      li{
        float: left;
        padding: 10px 20px;
        border-left: 1px solid $colore;
        cursor: pointer;
        &:first-child{
          border: 0;
        }
      }
    }
    /deep/ .el-pagination{
      flex: 1;
      text-align: right;
      display: inline-block;
      .el-pagination__jump{
        float: left;
      }
    }
  }
}
</style>


