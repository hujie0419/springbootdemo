<template>
  <div v-cloak>
        <row>
            <i-col span="5">
                <i-select filterable v-model="childRegion" placeholder="请选择城市" 
                @on-change="GetShopsByRegionId">
                    <i-option :value="0" key="0"
                    :disabled="disableRegions.length>1 && disableRegions.findIndex(s=>s>0)>-1">全国</i-option>
                    <OptionGroup :label="region.RegionName" v-for="region in regions"
                     :value="region.RegionName" :key="region.RegionName"> 
                    <i-option v-if="region.ChildRegions ==null" :value="region.RegionId" 
                    :disabled="disableRegions.findIndex(s=>s===region.RegionId)>-1"
                    :key="region.RegionId">{{region.RegionName}}</i-option>
                    <i-option v-else v-for="child in region.ChildRegions" 
                    :value="child.RegionId" :key="child.RegionId" 
                    :disabled="disableRegions.findIndex(s=>s===child.RegionId)>-1">{{child.RegionName}}</i-option>
                    </OptionGroup>
                </i-select>
            </i-col>
            <i-col span="14" offset="1">
                <i-select multiple filterable v-model="childShops" :loading="loadingShop"
                placeholder="全部门店" @on-change="$emit('on-shop-changed',childShops)">
                   <i-option v-for="shop in shops" 
                   :value="shop.ShopId" :key="shop.ShopId">{{shop.ShopId}}-{{shop.ShopName}}</i-option>
                </i-select>
            </i-col>
            <i-col span="3">
                <i-button type="ghost" v-on:click="$emit('on-remove')">删除</i-button>
            </i-col>
        </row>
    </div>
</template>

<script>
export default {
  name: "cityShop",
  props: ["regions", "selectedRegion", "selectedShops", "disableRegions"],
  data: function () {
    return {
      shops: [],
      loadingShop: false,
      childRegion: "",
      childShops: [],
      filterRegions: []
    };
  },
  created: function () {
    var vm = this;
    window.Vue = this;
    vm.childRegion = vm.selectedRegion;
    vm.GetShopsByRegionId();
    vm.childShops = vm.selectedShops;
  },
  methods: {
    GetShopsByRegionId: function () {
      var vm = this;
      var regionId = vm.childRegion;
      if (vm.childRegion !== vm.selectedRegion) {
        vm.$emit("on-city-changed", vm.childRegion);
      }
      vm.shops = [];
      if (regionId < 1) {
        return;
      }
      vm.loadingShop = true;
      vm.util.ajax
        .post("/PaintDiscountConfig/GetPaintShopsByRegionId", {
          regionId: regionId
        })
        .then(function (result) {
          result = (result || []).data || [];
          if (result.Status) {
            vm.shops = result.Data;
          }
          vm.loadingShop = false;
        });
    }
  }
};
</script>

<style scoped>
[v-cloak] {
  display: none;
}
</style>
