<template>
  <div class="specialCarConfig">
    <div class="title">
      <span>
        {{$route.query.partName}}
        {{$route.query.areaId?"地区模板(" + $route.query.areaId + ')':'全国模板'}}
        特殊车型配置</span>
      <span  @click="checkLog">修改日志</span>
    </div>
    <ul class="selectWrap">
      <li>
        <my-select :selectList="carBrandList" :selected.sync="selectData.brand" title="选择汽车品牌" @change="setSeriesList"></my-select>
        <my-select :selectList="carSeriesList" :selected.sync="selectData.vehicleId" title="选择车系"></my-select>
        <my-input :selected.sync="selectData"></my-input>
      </li>
      <li>
        <my-select :selectList="SeqList" :selected.sync="selectData.Seq" title="选择优先级"></my-select>
        <my-select :selectList="oilGradeList" v-if="isOil" :selected.sync="selectData.ProductPriorityGrade" title="选择推荐机油等级"></my-select>
        <my-select :selectList="productBrandList" :selected.sync="selectData.ProductBrand"  title="选择商品品牌"  @change="setProductSeriesList"></my-select>
        <my-select :selectList="productSeriesList" :selected.sync="selectData.ProductSeries" title="选择商品系列"></my-select>
        <my-select :selectList="VehicleBodyTypeList" :selected.sync="selectData.VehicleBodyType" title="选择车身类别"></my-select>
      </li>
      <li>
        <my-select :selectList="Viscositylist" v-if="isOil" :selected.sync="selectData.Viscosity" title="选择原厂粘度"></my-select>
        <my-select :selectList="NewViscositylist" v-if="isOil" :selected.sync="selectData.NewViscosity" title="选择新粘度"></my-select>
        <my-select :selectList="oilGradeList" v-if="isOil" :selected.sync="selectData.Grade" title="选择适配机油等级"></my-select>
        <my-select :selectList="configType" :selected.sync="selectData.Status" title="选择配置状态"></my-select>
      </li>
    </ul>
    <ul class="operationalBtn">
      <li @click="setDataBySearch">查询</li>
      <li @click="editMore">批量修改</li>
      <li @click="deleteMore">批量删除</li>
    </ul>
    <my-table 
      ref="myTable"
      :tableData="tableData" 
      :tableDataTotal="tableDataTotal" 
      :oilGradeList="oilGradeList"
      :Viscositylist="Viscositylist"
      :productBrandList="productBrandList"
      :pageSize.sync="selectData.pageSize"
      :pageIndex.sync="selectData.pageIndex"
      :brandAndSeries="brandAndSeries"
      :isOil="isOil"
      @click="setData"></my-table>
  </div>
</template>
<script>
import MySelect from './components/MySelect';
import MyInput from './components/MyInput';
import MyTable from './components/MyTable';
import { Observable } from 'rxjs';
import 'rxjs/add/observable/forkJoin';
export default {
  data() {
    return {
      carBrandList: [], // 汽车品牌
      carSeries: {},
      carSeriesList: [], // 汽车品牌
      SeqList: ['1', '2', '3', '4', '5', '6', '7', '8', '9', '10'], // 优先级列表
      oilGradeList: ['全合成', '半合成', '矿物油'],
      productBrandList: [],
      productSeriesList: [],
      VehicleBodyTypeList: ['中型SUV'],
      Viscositylist: [],
      NewViscositylist: [],
      configType: ['全部|0', '已启用|1', '已禁用|2', '暂无|3'],
      value: '',
      selectData: {
        brand: '', // 车型品牌
        vehicleId: '', // 车系
        minPrice: '', // 车价1
        maxPrice: '', // 车价2
        Viscosity: '', // 粘度
        NewViscosity: '',
        Grade: '', // 选择适配机油等级
        Seq: '', // 机油优先级
        ProductBrand: '', // 机油品牌
        ProductSeries: '', // 机油系列
        ProductPriorityGrade: '', // 选择推荐机油等级
        VehicleBodyType: '', // 车身类别
        Status: '0',
        pageIndex: 1, // 页码
        pageSize: 100 // 一页多少条
      },
      tableData: null,
      tableDataTotal: 0,
      isOil: this.$route.query.category === 'Oil',
      brandAndSeries: {}
    };
  },
  created() {
    this.setData();
    this.setSelectData();
  },
  components: {
    MySelect,
    MyInput,
    MyTable
  },
  methods: {
    /**
     * 查看日志
     */
    checkLog() {
      window.open('https://parts.tuhu.cn/Log/baoyangoprlog');
    },
    /**
     * 获取特殊车型配置 表格内容
     */
    setData() {
      let url = '/BaoYangRecommend/GetVehicleProductPriority';
      if (this.isOil) url = '/BaoYangRecommend/GetVehicleOilProductPriority';
      const loading = this.$loading({text: '数据加载中,请稍后...', spinner: 'el-icon-loading', background: 'rgba(0, 0, 0, 0.7)'});
      let selectData = JSON.parse(JSON.stringify(this.selectData));
      if (selectData.NewViscosity === '新粘度为空') {
        selectData.NewViscosity = '';
        selectData.IsNewViscosity = true;
      } else {
        selectData.IsNewViscosity = false;
      }
      this.$http.get(url, {
        apiServer: 'apiServer',
        params: Object.assign(this.$route.query, selectData)
      }).subscribe((res) => {
        loading.close();
        if (res.Status) {
          // debugger;
          this.tableData = res.Data;
          this.tableDataTotal = res.Total;
        }
      });
    },
    /**
     * 获取特殊车型配置 下拉框内容
     */
    setSelectData() {
      const GetVehicleBrands = this.$http.get('/BaoYang/GetVehicleBrands', {
        apiServer: 'apiServer',
        cacheData: false
      });
      const GetProductBrand = this.$http.get('/Product/GetAllBrandAndSeries', {
        apiServer: 'apiServer',
        cacheData: false,
        params: {category: this.$route.query.category, partName: this.$route.query.partName}
      });
      const GetAllOilViscosity = this.$http.get('/OilViscosityConfig/GetAllOilViscosity', {
        apiServer: 'apiServer',
        cacheData: false
      });
      const GetVehicleBodyType = this.$http.get('/BaoYangRecommend/GetVehicleBodyType', {
        apiServer: 'apiServer',
        cacheData: false
      });
      // const loading = this.$loading({text: 'Loading', spinner: 'el-icon-loading', background: 'rgba(0, 0, 0, 0.7)'});
      Observable.forkJoin([GetVehicleBrands, GetProductBrand, GetAllOilViscosity, GetVehicleBodyType]).subscribe(res => {
        // loading.close();
        // 汽车品牌
        if (res[0].status === 'success') {
          this.carBrandList = res[0].data;
        }
        // 商品品牌
        if (res[1].status === true) {
          this.brandAndSeries = res[1].Data;
          this.productBrandList = Object.keys(this.brandAndSeries);
        }
        // 获取原厂粘度|新粘度
        if (res[2].Status === true) {
          this.Viscositylist = JSON.parse(JSON.stringify(res[2].Data));
          this.NewViscositylist =JSON.parse(JSON.stringify(res[2].Data));
          this.NewViscositylist.splice(0, 0, '新粘度为空');
        }
        // 车身类别
        if (res[3].Status === true) {
          this.VehicleBodyTypeList = res[3].Data;
        }
      });
    },
    /**
     * 通过品牌获得系列-汽车
     */
    setSeriesList() {
      this.$set(this.selectData, 'vehicleId', '');
      this.carSeriesList = [];
      if (!this.selectData.brand) {
        return;
      }
      const loading = this.$loading({text: 'Loading', spinner: 'el-icon-loading', background: 'rgba(0, 0, 0, 0.7)'});
      this.$http.get('/Baoyang/GetVehicleSeries', {
        apiServer: 'apiServer',
        params: {brand: this.selectData.brand}
      }).subscribe((res) => {
        loading.close();
        if (res.status === 'success') {
          let arr = [];
          for (let key in res.data) {
            arr.push(res.data[key]+'|'+key);
          }
          this.carSeriesList = arr;
          this.carSeries = res.data;
        }
      });
    },
    /**
     * 通过品牌获得系列-商品
     */
    setProductSeriesList() {
      this.$set(this.selectData, 'ProductSeries', '');
      if (!this.selectData.ProductBrand) {
        this.productSeriesList = [];
        return;
      }
      this.productSeriesList = JSON.parse(JSON.stringify(this.brandAndSeries[this.selectData.ProductBrand]));
      // const loading = this.$loading({text: 'Loading', spinner: 'el-icon-loading', background: 'rgba(0, 0, 0, 0.7)'});
      // this.$http.get('/Product/GetProductSeries', {
      //   apiServer: 'apiServer',
      //   params: {category: this.$route.query.category, brand: this.selectData.ProductBrand}
      // }).subscribe((res) => {
      //   loading.close();
      //   if (res.status === true) {
      //     this.productSeriesList = res.data;
      //   }
      // });
    },
    /**
     * 查询
     */
    setDataBySearch() {
      if (this.selectData.minPrice || this.selectData.maxPrice) {
        if (Number(this.selectData.minPrice) > Number(this.selectData.maxPrice) || this.selectData.minPrice === this.selectData.maxPrice) {
          this.$message({message: '车价区间错误，起始价应小于结束价格', type: 'error', dangerouslyUseHTMLString: true});
          return;
        }
      }
      this.$set(this.selectData, 'pageIndex', 1);
      this.setData();
    },
    /**
     * 删除
     */
    deleteMore() {
      this.$refs.myTable.deleteRow();
    },
    /**
     * 修改
     */
    editMore() {
      this.$refs.myTable.openEditSelect();
    }
  }
};
</script>
<style lang="scss" scoped>
@import "css/common/_var.scss";
@import "css/common/_mixin.scss";
@import "css/common/_iconFont.scss";
$titleHeight: 54px;
.specialCarConfig{
  background-color: $colorf;
  margin: 12px;
  padding-bottom: 1rem;
  .title{
    background-color: $colorfa;
    height: $titleHeight;
    line-height: $titleHeight;
    font-size: $largeFontSize;
    color: $color3;
    padding: 0 20px;
    font-weight: bold;
    span{
      &:last-child{
        font-weight: normal;
        font-size: 12px;
        color: #DF3348;
        text-align: right;
        cursor: pointer;
        float: right;
      }
    }
  }
  .selectWrap{
    padding: 0 20px;
    li{
      margin: 1rem 0;
    }
  }
  .operationalBtn{
      line-height: 12px;
      overflow: hidden;
      border: 1px solid $colore;
      background-color: $colorfa;
      float: right;
      margin-right: 20px;
      display: flex;
      border-bottom: 0;
      li{
        width: 120px;
        height: 40px;
        line-height: 40px;
        flex: 1;
        text-align: center;
        border-left: 1px solid $colore;
        cursor: pointer;
        &:first-child{
          border: 0;
        }
      }
    }
}
</style>


