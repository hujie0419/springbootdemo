<template>
<div style="padding-bottom:100px;">
  <div v-if="showFilter">
    <ServiceFilter @search="search" @checkAllGroupChange="checkAllGroupChange" :filter.sync="filter"></ServiceFilter>
  </div>
  <ServiceTable ref="table"></ServiceTable>
</div>
</template>
<script>
import ServiceFilter from '@/views/shoppromotion/servicefilter'
import ServiceTable from '@/views/shoppromotion/servicetable'
export default {
  props: ["opts", "showFilter", "productType"],
  data () {
    return {
      filter: {
        CatogryIDs: [],
        StartDefaultPrice: null,
        EndDefaultPrice: null,
        ServersName: null,
        ProductIDs: []
      },
      selectServices: {}
    }
  },
  created () {
  },
  components: {
    ServiceFilter,
    ServiceTable
  },
  methods: {
    loadList (_opts) {
      this.selectServices = _opts || this.selectServices
      this.filter = {
        CatogryIDs: [],
        StartDefaultPrice: null,
        EndDefaultPrice: null,
        ServersName: null,
        ProductIDs: []
      }
      let ProductList = [];
      for (let key in this.selectServices) {
          ProductList.push(key);
      }
      this.filter.ProductIDs = ProductList;
      this.total = this.filter.ProductIDs.length
      if (this.filter.ProductIDs.length) {
        this.loadServiceList(1);
      } else if (this.showFilter) {
        this.loadServiceList(1);
      } else {
        this.$refs.table.clearData()
      }
    },
    checkAllGroupChange () {
      this.loadServiceList(1);
    },
    search () {
      this.loadServiceList(1)
    },
    loadServiceList (pageIndex) {
      this.$refs.table.loadList(pageIndex, this.filter)
    }
  }
}
</script>
