<template>
  <div v-cloak>
      <p ></p>
      <p v-if="regionShops!=null && regionShops.length>0" 
       v-for="regionShop in regionShops" 
       :key="regionShop.ProvinceName +'-'+regionShop.CityName" 
       :value="regionShop.ProvinceName"
       style="text-align:left;" >
          {{regionShop.ProvinceName}}-{{regionShop.CityName}}: <br>
          {{regionShop.Shops | joinList }}
      </p>
  </div>
</template>

<script>
export default {
 name: 'shopViewForTable',
  props: ['regionShops'],
  filters: {
      joinList: function (list) {
          var shopNames = (list || []).map(x => x.ShopName);
          return shopNames.join(" , ") || "全部门店";
      }
  }
}
</script>

<style scoped>
[v-cloak] {
  display: none;
}
</style>
