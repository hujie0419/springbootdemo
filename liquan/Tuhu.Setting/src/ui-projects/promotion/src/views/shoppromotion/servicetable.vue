<template>
<div>
  <Table :columns="columns" :data="data" @on-selection-change="selectChange"></Table>
  <div style="margin: 10px;">
        <div style="float: right;">
            <Page :total="page.total" :page-size="page.pageSize" :current="page.current" :page-size-opts="[10 ,20 ,50]" show-elevator show-sizer @on-change="handlePageChange" @on-page-size-change="handlePageSizeChange"></Page>
        </div>
  </div>
</div>
</template>
<script>
import util from "@/framework/libs/util.js"
export default {
  props: ["showSelection", "total"],
  data () {
    let columns = [
        {
          title: "服务PID",
          key: "ProductID",
          width: 160
        },
        {
          title: "服务名称",
          key: "ServersName",
          width: 180
        },
        {
          title: "销售价格",
          key: "DefaultPrice",
          width: 100
        },
        {
          title: "服务说明",
          key: "ServiceRemark"
        }
      ]
    if (this.showSelection) {
      columns.unshift({
          type: "selection",
          width: 60,
          align: "center",
          checked: true
        })
    }
    return {
      columns: columns,
      data: [],
      page: {
        total: 0,
        current: 1,
        pageSize: 10
      },
      selectServices: {},
      filter: null
    }
  },
  created () {
  },
  methods: {
    selectChange (selection) {
      this.$emit("selectChange", selection, this.data)
    },
    loadList (pageIndex, filter, opts) {
      this.selectServices = opts || this.selectServices
      this.filter = filter
      this.page.current = pageIndex || this.page.current
      let params = {
        PageIndex: this.page.current,
        PageSize: this.page.pageSize,
        ServersName: filter.ServersName,
        CatogryIDs: filter.CatogryIDs,
        ProductID: filter.ProductID,
        ProductIDs: filter.ProductIDs,
        StartDefaultPrice: filter.StartDefaultPrice,
        EndDefaultPrice: filter.EndDefaultPrice
      };
      util.ajax.post("/shoppromotion/GetServiceList", params).then((response) => {
          if (response.status === 200) {
            if (this.showSelection) {
              this.data = response.data.data.map((item) => {
                  if (this.selectServices[item.ProductID]) {
                    item._checked = true 
                  }
                  return item
              });
            } else {
              this.data = response.data.data
            }
            if (this.data.length) {
              var first = this.data[0];
              this.page.total = first.TotalCount;
              this.$emit("update:total", this.page.total)
            }
          }
      })
    },
    clearData () {
      this.data = []
      this.page.total = 0
    },
    handlePageChange (pageIndex) {
      this.loadList(pageIndex, this.filter);
    },
    handlePageSizeChange (pageSize) {
      this.page.pageSize = pageSize;
      this.loadList(1, this.filter);
    }
  }
}
</script>
