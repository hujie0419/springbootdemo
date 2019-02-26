<template>
  <Row>
    <i-col span="8" >
      <i-select transfer v-model="searchModel.ProvinceId"
        placeholder="请选择省份" @on-change="GetCitiesByProvince">
        <i-option value="" key="">-请选择省份-</i-option>
        <i-option v-for="province in provinces" :value="province.ProvinceId"
          :key="province.ProvinceId">{{province.ProvinceName}}</i-option>
      </i-select>
    </i-col>
    <i-col span="8">
      <i-select transfer v-model="searchModel.CityId"
        placeholder="请选择城市" @on-change="GetDistrictsByCityId">
        <i-option value="" key="">-请选择城市-</i-option>
        <i-option v-for="city in cities" :value="city.CityId"
          :key="city.CityId">{{city.CityName}}</i-option>
      </i-select>
    </i-col>
    <i-col span="8">
      <i-select transfer v-model="searchModel.DistrictId"
        placeholder="请选择地区">
        <i-option value="" key="">-请选择城市-</i-option>
        <i-option v-for="district in districts" :value="district.DistrictId"
          :key="district.DistrictId">{{district.DistrictName}}</i-option>
      </i-select>
    </i-col>
  </Row>
</template>
<script>
export default {
  data () {
    return {
      searchModel: {
        ProvinceId: "",
        CityId: "",
        DistrictId: "",
        RegionId: 0
      },
      provinces: [],
      cities: [],
      districts: []
    };
  },
  mounted: function () {
    window.Vue = this;
    this.$Message.config({
      top: 50,
      duration: 5
    });
    this.GetAllProvince();
  },
  watch: {
    "searchModel.DistrictId": function () {
      this.DistrictChanged();
    }
  },
  methods: {
    // 获取所有省市
    GetAllProvince: function () {
      var vm = this;
      vm.util.ajax.get("/Region/GetAllProvince").then(function (res) {
        var data = (res || []).data || [];
        vm.provinces = data.Data || [];
      });
    },
    // 获取二级城市
    GetCitiesByProvince: function () {
      var vm = this;
      vm.cities = [];
      vm.searchModel.CityId = 0;
      vm.$emit("on-district-changed", vm.searchModel);
      if (vm.searchModel.ProvinceId) {
        vm.util.ajax
          .post("/Region/GetCitiesByProvinceId", {
            provinceId: vm.searchModel.ProvinceId
          })
          .then(function (res) {
            var data = (res || []).data || [];
            vm.cities = data.Data || [];
          });
      }
    },
    // 获取三级地区
    GetDistrictsByCityId: function () {
      var vm = this;
      vm.districts = [];
      vm.searchModel.DistrictId = 0;
      vm.$emit("on-district-changed", vm.searchModel);
      if (vm.searchModel.CityId) {
        vm.util.ajax
          .post("/Region/GetDistrictsByCityId", {
            cityId: vm.searchModel.CityId
          })
          .then(function (res) {
            var data = (res || []).data || [];
            vm.districts = data.Data || [];
          });
      }
    },
    DistrictChanged: function () {
      var vm = this;
      vm.searchModel.RegionId = vm.searchModel.DistrictId;
      vm.$emit("on-district-changed", vm.searchModel);
    }
  }
};
</script>

<style scoped>
[v-cloak] {
  display: none;
}
</style>
