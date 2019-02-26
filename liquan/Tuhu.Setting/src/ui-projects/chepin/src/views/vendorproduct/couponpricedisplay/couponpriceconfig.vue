 <template>
  <div>
    <h2 class="title">券后价查询</h2>
    <div>
      <Row :gutter="24">
        <i-col span="4">
          <label class="label">品牌:</label>
          <Select style="width:80%;" filterable placeholder="-请选择品牌-"
            v-model="searchData.Brand">
            <Option v-for="item in brands" :value="item"
              :key="item">{{item}}</Option>
          </Select>
        </i-col>
        <i-col span="1">
          <Button type="primary" @click="Search(1)">查询</Button>
        </i-col>
        <i-col span="12" v-if="pager.TotalCount>0">
          <Button @click="ExportExcel">导出</Button>
          <Button @click="RemoveCacheDialog(tableShowData.map(s=>s.Pid))">清除缓存</Button>
          <Button type="success" @click="MultUpsertConfig(true)">当页全部展示</Button>
          <Button type="warning" @click="MultUpsertConfig(false)">当页全部隐藏</Button>
          <Tooltip style="margin-left:15px;">
            <Icon type="alert"></Icon>
            <div style="width:300px;" slot="content">
              只显示有券后价的商品
            </div>
          </Tooltip>
        </i-col>
      </Row>

    </div>
    <Table style="margin-top: 15px;" border stripe :columns="table.Columns"
      :data="tableShowData" :loading="table.Loading"
      @on-selection-change="table.Selection = arguments[0]">
    </Table>
    <Page v-if="pager.TotalCount" :total="pager.TotalCount"
      :current.sync="pager.PageIndex" :page-size="pager.PageSize"
      show-total show-elevator></Page>
  </div>
</template>
<script>
export default {
  props: {
    productType: {
      type: String,
      required: true
    }
  },
  data () {
    return {
      searchData: {
        Brand: ""
      },
      brands: [],
      table: {
        Columns: [
          {
            title: "编号",
            align: "center",
            type: "index",
            width: 60
          },
          {
            title: "商品PID",
            align: "center",
            key: "Pid"
          },
          {
            title: "商品名称",
            align: "center",
            key: "DisplayName"
          },
          {
            title: "券后价",
            align: "center",
            key: "Price"
          },
          {
            title: "可用券",
            align: "center",
            key: "Coupons",
            render: (h, p) => {
              var labelhtml = "";
              var coupons = p.row.Coupons;
              for (var index in coupons) {
                labelhtml += "<lable>" + coupons[index] + "</lable>";
                labelhtml += "<br></br>";
              }
              return h("span", {
                domProps: {
                  innerHTML: labelhtml
                }
              });
            }
          },
          {
            title: "是否展示券后价",
            align: "center",
            key: "IsShow",
            render: (h, p) => {
              return h(
                "Checkbox",
                {
                  props: {
                    value: p.row.IsShow
                  },
                  on: {
                    "on-change": () => {
                      this.UpsertConfig(p.row);
                    }
                  }
                },
                "显示"
              );
            }
          }
        ],
        Data: [],
        TotalData: [],
        Loading: false,
        Selection: []
      },
      pager: {
        TotalCount: 0,
        PageIndex: 1,
        PageSize: 50
      }
    };
  },
  created () {
    window.CouponPirceConfigVue = this;
    this.GetAllBrands();
  },
  computed: {
    tableShowData () {
      var vm = this;
      var startDataIndex = (vm.pager.PageIndex - 1) * vm.pager.PageSize;
      var endDataIndex = vm.pager.PageIndex * vm.pager.PageSize;
      return vm.table.TotalData.slice(startDataIndex, endDataIndex);
    }
  },
  methods: {
    // 获取所有品牌
    GetAllBrands () {
      var vm = this;
      vm.ajax
        .post("/VPCouponPrice/GetAllBrands", {
          productType: vm.productType
        })
        .then(response => {
          var res = response.data || {};
          this.brands = res.Data || [];
          if (!res.Status) {
            vm.$Message.error("查询失败!" + (res.Msg || ""));
          }
        });
    },
    // 搜索
    Search (index) {
      var vm = this;
      vm.pager.PageIndex = index;
      if (!vm.searchData.Brand) {
        vm.$Message.warning("请选择品牌");
        return;
      }
      vm.table.Loading = true;
      vm.pager.TotalCount = 0;
      if (!vm.searchData.Brand) {
        vm.$Message.warning("请选择品牌");
        return;
      }
      this.ajax
        .post("/VPCouponPrice/SelectConfig", {
          productType: vm.productType,
          brand: vm.searchData.Brand
        })
        .then(response => {
          var res = response.data;
          vm.table.Loading = false;
          vm.table.TotalData = res.Data || [];
          vm.pager.TotalCount = vm.table.TotalData.length;
          if (!res.Status) {
            this.$Message.error("查询失败!");
          }
        });
    },
    // 更改是否展示配置
    UpsertConfig (col) {
      var vm = this;
      vm.table.Loading = true;
      var model = JSON.parse(JSON.stringify(col));
      model.IsShow = !model.IsShow;
      this.ajax
        .post("/VPCouponPrice/UpsertConfig", {
          model: model
        })
        .then(response => {
          var res = response.data || [];
          if (!res.Status) {
            vm.$Message.error("更改失败!" + (res.Msg || ""));
            col.IsShow = !col.IsShow;
            vm.$nextTick(() => {
              col.IsShow = !col.IsShow;
            });
            vm.table.Loading = false;
          } else {
            vm.$Message.success("更改成功!" + (res.Msg || ""));
            setTimeout(() => {
              vm.RemoveCache([model.Pid]);
              vm.Search(vm.pager.PageIndex);
            }, 2000);
          }
        });
    },
    // 批量更改
    MultUpsertConfig (value) {
      this.$Modal.confirm({
        title: "全部" + (value ? "展示" : "隐藏"),
        content: "确认 全部" + (value ? "展示" : "隐藏") + " 券后价数据？",
        loading: true,
        onOk: () => {
          var vm = this;
          var models = vm.tableShowData || [];
          var length = models.length || 0;
          if (!length) {
            vm.$Message.warning("至少选择一条记录");
            return;
          }
          vm.ajax
            .post("/VPCouponPrice/MultUpsertConfig", {
              models: models,
              isShow: value
            })
            .then(response => {
              var res = response.data || [];
              if (!res.Status) {
                vm.$Message.error("更改失败!" + (res.Msg || ""));
              } else {
                vm.$Message.success("更改成功!" + (res.Msg || ""));
                setTimeout(() => {
                  vm.RemoveCache(models.map(s => s.Pid));
                  vm.Search(vm.pager.PageIndex);
                }, 2000);
              }
              this.$Modal.remove();
            });
        }
      });
    },
    // 导出数据
    ExportExcel () {
      
      this.$Modal.confirm({
        title: "导出数据",
        content: "确认导出 " + this.searchData.Brand + "券后价数据？",
        loading: true,
        onOk: () => {
          this.$Modal.remove();
          window.location.href =
            "/VPCouponPrice/ExportExcel?productType=" +
            this.productType +
            "&brand=" +
            this.searchData.Brand;
        }
      });
    },
    // 移除缓存
    RemoveCache (pids) {
      this.ajax
        .post("/VPCouponPrice/RemoveCache", {
          productType: this.productType,
          pids: pids
        })
        .then(response => {
          var res = response.data;
          if (res.Status) {
            this.$Message.info("清除缓存成功");
          } else {
            this.$Message.error("清除缓存失败!" + (res.Msg || ""));
          }
        });
    },
    // 清除缓存对话框
    RemoveCacheDialog (pids) {
      if (pids && pids.length > 0) {
        this.$Modal.confirm({
          title: "清除缓存",
          content: "确定清除当页数据的服务缓存？",
          loading: true,
          onOk: () => {
            this.RemoveCache(pids);
            this.$Modal.remove();
          }
        });
      }
    }
  }
};
</script>
