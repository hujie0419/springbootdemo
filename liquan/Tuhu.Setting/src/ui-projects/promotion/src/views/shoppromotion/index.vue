<template>
<div>
    <div>运营 > 优惠券 > 门店优惠券规则</div>
    <Search @filter="filter"></Search>
    <div align="right"><Button type="primary" shape="circle" @click="toCreate" icon="plus-round">新建规则</Button></div>
    <Tabs type="card" @on-click="tabChanged">
        <TabPane :label="'可领取 ('+tabTotals.total1+')'"  name="tab1">
          <List @loadListData="loadListData" :status="1" ref='list1' :total.sync="tabTotals.total1" :opts="searchParams" :tabIndex="tabIndex"></List>
        </TabPane>
        <TabPane :label="'待发布 ('+tabTotals.total0+')'" name="tab0">
          <List @loadListData="loadListData" :status="0" ref='list0' :total.sync="tabTotals.total0" :opts="searchParams" :tabIndex="tabIndex"></List>
        </TabPane>
        <TabPane :label="'暂停领取('+tabTotals.total2+')'" name="tab2">
          <List @loadListData="loadListData" :status="2" ref='list2' :total.sync="tabTotals.total2" :opts="searchParams" :tabIndex="tabIndex"></List>
        </TabPane>
        <TabPane :label="'已作废('+tabTotals.total3+')'" name="tab3">
          <List @loadListData="loadListData" :status="3" ref='list3' :total.sync="tabTotals.total3" :opts="searchParams" :tabIndex="tabIndex"></List>
        </TabPane>
    </Tabs>
</div>
</template>
<script>
import Search from '@/views/shoppromotion/search.vue'
import List from '@/views/shoppromotion/list.vue'
export default {
  data () {
    return {
      searchParams: {time: null},
      tabIndex: 1,
      tabTotals: {
        total0: 0,
        total1: 0,
        total2: 0,
        total3: 0
      }
    }
  },
  created () {
     
  },
  computed: {
    
  },
  components: {
    Search,
    List
  },
  methods: {
    filter (f) {
      this.searchParams = f;
      // this.$refs['list' + this.tabIndex].loadData(f);
    },
    tabChanged (name) {
      this.tabIndex = Number(name.substr(3))
    },
    loadListData () {
      console.log(this.searchParams)
      this.searchParams.time = new Date().getTime()
    },
    toCreate () {
      this.$router.push({'name': 'shoppromotionCreate'});
    }
  }
}
</script>
