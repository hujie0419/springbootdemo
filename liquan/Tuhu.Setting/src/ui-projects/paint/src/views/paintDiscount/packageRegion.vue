<template>
  <div v-cloak>
    <h2 class="title" style="margin-top:100px;">城市门店配置</h2>
    <Row>
      <i-col span="3">选择城市与门店:</i-col>
      <i-col span="3">
        <i-button long type="dashed" icon="plus-round" v-on:click="AppendNewCityShop">新增</i-button>
      </i-col>
    </Row>
    <Row>
      <cityShop v-if="isShowCityShop && list.length>0"
       v-for="(item,index) in list" 
       :regions="regions" 
       :selectedRegion="list[index].RegionId" 
       :selectedShops="list[index].ShopIds"
       :disableRegions="list.map(s=>s.RegionId)" 
       :key="list[index].Key" 
       style="margin-top:10px; margin-bottom:10px;" 
       v-on:on-remove="RemoveCityShop(index)"
       v-on:on-city-changed="CityChanged($event,index)" 
       v-on:on-shop-changed="ShopsChanged($event,index)">
      </cityShop>
    </Row>
    <Row style="margin-top:20px;">
      <i-col span="3">
        <i-button long type="primary" v-on:click="SaveCityShops">保存</i-button>
      </i-col>
    </Row>
  </div>
</template>

<script>
  import util from "@/framework/libs/util";
  import cityShop from "@/views/paintDiscount/cityShop";
  export default {
    name: "packageRegion",
    props: ["packageId"],
    data: function () {
      return {
        regions: [],
        regionShops: [],
        list: [],
        sequence: 0,
        isShowCityShop: false
      };
    },
    components: {
      cityShop
    },
    computed: {
      // 城市与门店 （过滤未选择城市的）
      regionsWithShop: function () {
        return (this.list || []).filter(s => s.RegionId !== "" && s.RegionId > -1);
      },
      // 选择了全部门店的城市名称
      cityWithAllShop: function () {
        var vm = this;
        var cityIdWithAllShop = (vm.regionsWithShop || [])
          .filter(x => x.ShopIds && x.ShopIds.length < 1 && x.RegionId > -1)
          .map(m => m.RegionId) || [];
        if (cityIdWithAllShop && cityIdWithAllShop.length > 0) {
          return (vm.regions || [])
            .map(m => {
              if (m.ChildRegions == null && cityIdWithAllShop.findIndex(a => a === m.RegionId) > -1) {
                return m.RegionName;
              } else if (m.ChildRegions != null) {
                var childRegion = m.ChildRegions.find(f =>
                  cityIdWithAllShop.findIndex(a => a === f.RegionId) > -1);
                if (childRegion) {
                  return childRegion.RegionName;
                }
              }
            }).filter(s => s).join(",");
        } else {
          return "";
        }
      }
    },
    created: function () {
      this.GetAllRegion();
      this.Search();
    },
    methods: {
      // 获取所有二级城市
      GetAllRegion: function () {
        var vm = this;
        util.ajax
          .post("/PaintDiscountConfig/GetAllRegions")
          .then(function (result) {
            result = (result || []).data || [];
            if (result.Status) {
              vm.regions = result.Data;
              vm.isShowCityShop = true;
            }
          });
      },
      Search: function () {
        var vm = this;
        vm.util.ajax.post("/PaintDiscountConfig/GetPackageRegionForView", {
            packageId: vm.packageId
          })
          .then(function (result) {
            result = (result || []).data || [];
            if (result && result.Status) {
              vm.regionShops = result.Data;
            } else {
              vm.regionShops = [];
            }
            vm.list = [];
            if (!vm.regionShops || vm.regionShops.length <= 0) {
              vm.sequence += 1;
              vm.list.push({
                RegionId: "",
                ShopIds: [],
                Key: vm.sequence.toString()
              });
            } else {
              vm.regionShops.forEach(function (item) {
                vm.sequence += 1;
                vm.list.push({
                  RegionId: item.CityId,
                  ShopIds: (item.Shops || []).map(x => x.ShopId),
                  Key: vm.sequence.toString()
                });
              });
            }
          });
      },
      // 删除城市与门店
      RemoveCityShop: function (index) {
        var vm = this;
        vm.list.splice(index, 1);
      },
      // 新增城市与门店
      AppendNewCityShop: function () {
        var vm = this;
        if (vm.list.findIndex(s => s.RegionId === "") > -1) {
          vm.$Message.warning("上一次新增未选择城市门店，请完成后再次新增");
          return;
        }
        if (vm.list.findIndex(s => s.RegionId !== "" && s.RegionId < 1) > -1) {
          vm.$Message.warning("配置了全国，无法再配置其他地区");
          return;
        }
        vm.sequence += 1;
        vm.list.push({
          RegionId: "",
          ShopIds: [],
          Key: vm.sequence.toString()
        });
      },
      // 子组件城市变更触发同步事件
      CityChanged: function (childCity, index) {
        var vm = this;
        if (vm.list.findIndex(s => s.RegionId === childCity) > -1) {
          vm.$Message.warning("无法配置相同的城市");
          vm.list.splice(index, 1);
        } else {
          vm.$set(vm.list[index], "RegionId", childCity);
        }
      },
      // 子组件门店变更触发同步事件
      ShopsChanged: function (childShops, index) {
        var vm = this;
        vm.$set(vm.list[index], "ShopIds", childShops);
      },
      // 保存城市与门店配置
      SaveCityShops: function () {
        var vm = this;
        if (!vm.regionsWithShop || vm.regionsWithShop.length < 1) {
          vm.$Message.warning("城市门店配置不能为空");
          return;
        }
        if (vm.cityWithAllShop) {
          if (!confirm(vm.cityWithAllShop + " 设置为全部门店，是否确认?")) {
            return;
          }
        }
        if (!confirm("确认保存" + "?")) {
          return;
        }
        util.ajax
          .post("/PaintDiscountConfig/UpsertPackageRegion", {
            packageId: vm.packageId,
            regionShops: JSON.stringify(vm.regionsWithShop)
          })
          .then(function (result) {
            result = (result || []).data || [];
            if (result.Status) {
              vm.$Message.success("编辑成功!");
              setTimeout(() => {
                vm.Search();
                vm.RemoveCache(vm.regionsWithShop);
              }, 2000);
            } else {
              vm.$Message.error({
                content: "编辑失败!" + (result.Msg || ""),
                duration: 0,
                closable: true
              });
            }
          });
      },
      // 刷新城市门店配置缓存
      RemoveCache: function () {
        var vm = this;
        var regionIds = vm.regionsWithShop.map(s => s.RegionId);
        util.ajax.post("/PaintDiscountConfig/RemovePackageRegionCache", {
            packageId: vm.packageId,
            regionIds: regionIds
          })
          .then(function (result) {
            result = (result || []).data || [];
            if (result && result.Status) {
              vm.$Message.success("刷新城市门店缓存成功!")
            } else {
              vm.$Message.error("刷新城市门店缓存失败" + (result.Msg || ""));
            }
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
