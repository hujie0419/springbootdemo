<template>
  <div style="margin-top:15px;">
    <Row type="flex" :gutter="16" style="margin-top:5px;">
      <i-col span="3" v-if="vehicleLevel>=1">
          <label>品牌: </label>
          <i-select filterable style="width:80%" v-model="searchData.Brand" @on-change="BrandChange">
              <i-option value="" key="">-请选择品牌-</i-option>
              <i-option v-for="brand in brands" :value="brand" :key="brand">{{brand}}</i-option>
          </i-select>
      </i-col>
      <i-col span="4" v-if="vehicleLevel>=2">
          <label>车系: </label>
          <i-select style="width:80%" v-model="searchData.VehicleId" @on-change="SeriesChange">
              <i-option value="" key="">-请选择车系-</i-option>
              <i-option v-for="(vehicleName,vehicleId) in vehicles" :value="vehicleId" :key="vehicleId">{{vehicleName}}</i-option>
          </i-select>
      </i-col>
      <i-col span="4" v-if="vehicleLevel>=3">
          <label>排量: </label>
          <i-select style="width:80%" v-model="searchData.PaiLiang" @on-change="PaiLiangChange">
              <i-option value="" key="">-请选择排量-</i-option>
              <i-option v-for="paliang in pailiangs" :value="paliang" :key="paliang">{{paliang}}</i-option>
          </i-select>
      </i-col>
      <i-col span="4" v-if="vehicleLevel>=4">
          <label>年份: </label>
          <i-select style="width:80%" v-model="searchData.Nian" @on-change="NianChange">
              <i-option value="" key="">-请选择年份-</i-option>
              <i-option v-for="nian in nians" :value="nian" :key="nian">{{nian}}</i-option>
          </i-select>
      </i-col>
       <i-col span="4" v-if="vehicleLevel>=5">
          <label>款型: </label>
          <i-select style="width:80%" v-model="searchData.Tid">
              <i-option value="" key="">-请选择款型-</i-option>
              <i-option v-for="(salesName,tid) in salesNames" :value="tid" :key="tid">{{salesName}}</i-option>
          </i-select>
      </i-col>
    </Row>
    <Row type="flex" :gutter="16" style="margin-top:10px;">
        <i-col span="4">
          <label>输入Vid: </label>
          <i-input style="width:80%" v-model.trim="vehicleIdInput">
          </i-input>
      </i-col>
    </Row>
    <Row style="margin-top:10px;">
      <i-col span="4">
          <Checkbox v-model="searchData.IsOnlyConfiged">只显示配置数据的车型</Checkbox>      
      </i-col>
    </Row>
    <Row style="margin-top:10px;">
     <i-button type="success" size="large" @click="Search">查询</i-button>
    </Row>
  </div>
</template>

<script>
export default {
  name: "vehiclelevelselect",
  props: ["vehicleLevel"],
  data () {
    return {
      searchData: {
        Brand: "",
        VehicleId: "",
        PaiLiang: "",
        Nian: "",
        SalesName: "",
        Tid: "",
        IsOnlyConfiged: false
      },
      brands: [],
      vehicles: [],
      pailiangs: [],
      nians: [],
      salesNames: [],
      vehicleIdInput: ""
    };
  },
  created () {
    window.Vue = this;
    this.GetAllBrands();
  },
  watch: {
    vehicleLevel: function () {
      if (this.vehicleLevel < 5) {
        this.searchData.Tid = "";
      }
      if (this.vehicleLevel < 4) {
        this.searchData.Nian = "";
      }
      if (this.vehicleLevel < 3) {
        this.searchData.PaiLiang = "";
      }
      if (this.vehicleLevel < 2) {
        this.searchData.VehicleId = "";
      }
    },
    vehicleIdInput: function () {
      var vue = this;
      if (vue.vehicleIdInput) {
        if (!vue.vehicles.hasOwnProperty(vue.vehicleIdInput)) {
          vue.searchData.Brand = "";
          vue.vehicles = [];
        }
        if (vue.searchData.VehicleId !== vue.vehicleIdInput) {
          vue.searchData.VehicleId = vue.vehicleIdInput;
          vue.SeriesChange();
        }
      }
    }
  },
  methods: {
    GetAllBrands: function () {
      var vue = this;
      this.ajax.post("/Vehicle/GetAllVehicleBrands").then(response => {
        var res = response.data || [];
        if (!res.Success) {
          this.$Message.error("获取车型品牌失败!" + (res.Msg || ""));
        }
        vue.brands = res.Data || [];
      });
    },
    BrandChange: function () {
      var vue = this;
      var brand = vue.searchData.Brand;
      vue.vehicles = [];
      vue.searchData.VehicleId = "";
      if (!brand) {
        return;
      }
      vue.vehicleIdInput = "";
      this.ajax
        .post("/vehicle/GetVehicleSeriesByBrand", { brand: brand })
        .then(response => {
          var res = response.data;
          if (!res.Success) {
            this.$Message.error("获取车型系列失败!" + (res.Msg || ""));
          }
          vue.vehicles = res.Data || [];
        });
    },
    SeriesChange: function () {
      var vue = this;
      var vid = vue.searchData.VehicleId;
      vue.searchData.PaiLiang = "";
      vue.pailiangs = [];
      if (!vid) {
        return;
      }
      if (vid !== vue.vehicleIdInput) {
        vue.vehicleIdInput = vid;
      }
      this.ajax
        .post("/vehicle/GetVehiclePaiLiang", { vid: vid })
        .then(response => {
          var res = response.data;
          if (!res.Success) {
            this.$Message.error("获取车型排量失败!" + (res.Msg || ""));
          }
          vue.pailiangs = res.Data || [];
        });
    },
    PaiLiangChange: function () {
      var vue = this;
      var vid = vue.searchData.VehicleId;
      var paiLiang = vue.searchData.PaiLiang;
      vue.nians = [];
      vue.searchData.Nian = "";
      if (!(vid && paiLiang)) {
        return;
      }
      this.ajax
        .post("/vehicle/GetVehicleNian", { vid: vid, paiLiang: paiLiang })
        .then(response => {
          var res = response.data;
          if (!res.Success) {
            this.$Message.error("获取车型年产失败!" + (res.Msg || ""));
          }
          vue.nians = res.Data || [];
        });
    },
    NianChange: function () {
      var vue = this;
      var vid = vue.searchData.VehicleId;
      var paiLiang = vue.searchData.PaiLiang;
      var nian = vue.searchData.Nian;
      vue.salesNames = [];
      vue.searchData.Tid = "";
      if (!(vid && paiLiang && nian)) {
        return;
      }
      vue.ajax
        .post("/vehicle/GetVehicleSalesName", {
          vid: vid,
          paiLiang: paiLiang,
          nian: nian
        })
        .then(response => {
          var res = response.data;
          if (!res.Success) {
            this.$Message.error("获取车型款型失败!" + (res.Msg || ""));
          }
          vue.salesNames = res.Data || [];
        });
    },
    Search () {
      this.$emit("Search", this.searchData);
    }
  }
};
</script>

<style lang="less">
ivu-row {
  margin-top: 15px;
}
</style>
