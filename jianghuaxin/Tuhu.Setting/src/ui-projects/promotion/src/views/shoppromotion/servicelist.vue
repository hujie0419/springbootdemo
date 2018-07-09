<template>
  <div style="padding-bottom:30px;">
    <ServiceFilter @search="search" @checkAllGroupChange="checkAllGroupChange" :filter.sync="filter"></ServiceFilter>
    <div>共{{total}}个服务产品</div>
    <ServiceTable ref="table" @selectChange="selectChange" :total.sync="total" :showSelection="true"></ServiceTable>
  </div>
</template>
<script>
import ServiceFilter from '@/views/shoppromotion/servicefilter'
import ServiceTable from '@/views/shoppromotion/servicetable'
export default {
  props: ["opts"],
  data () {
    return {
      filter: {
        CatogryIDs: [],
        StartDefaultPrice: null,
        EndDefaultPrice: null,
        ServersName: null
      },
      total: 0,
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
    loadList (opts) {
      this.selectServices = opts
      this.$refs.table.loadList(1, this.filter, this.selectServices)
    },
    checkAllGroupChange () {
      this.$refs.table.loadList(1, this.filter, this.selectServices);
    },
    search () {
      this.$refs.table.loadList(1, this.filter, this.selectServices);
    },
    selectChange (selection, data) {
      for (let i = 0; i < data.length; i++) {
        if (this.selectServices[data[i].ProductID]) {
          delete this.selectServices[data[i].ProductID]
        }
      }
      for (let j = 0; j < selection.length; j++) { 
        this.selectServices[selection[j].ProductID] = selection[j];
      }
      this.$emit("update:opts", this.selectServices)
    }
  }
}
</script>
