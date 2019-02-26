 <template>
  <div>
    <h2 class="title">蓄电池券后价查询</h2> 
    <div>
      <Row :gutter="24">
        <i-col span="4">
          <label class="label">品牌:</label>
          <Select style="width:80%;" filterable
                  placeholder="请选择蓄电池品牌"
                  v-model="searchData.Brand">
            <Option value="All">全部</Option>
            <Option v-for="item in brands"
                    :value="item"
                    :key="item">{{item}}</Option>
          </Select>
        </i-col>
        <i-col span="1" >
          <Button type="primary" @click="Search">查询</Button>
        </i-col>
         <i-col span="12" v-if="table.Data.length>0" >
            <Button @click="ExportExcel">导出</Button>
           <Button @click="RemoveCacheDialog(table.Data.map(s=>s.Pid))">清除缓存</Button>
          <Button type="success" @click="MultUpsertConfig(true)">全部展示</Button>
        <Button type="warning" @click="MultUpsertConfig(false)">全部隐藏</Button>
        <Tooltip style="margin-left:15px;" >
            <Icon type="alert"></Icon>
            <div style="width:300px;" slot="content">
                只显示有券后价的商品
            </div>
        </Tooltip>
        </i-col>
      </Row>

    </div>
    <Table style="margin-top: 15px;"
           border
           stripe
           :columns="table.Columns"
           :data="table.Data"
           :loading="table.Loading"
           @on-selection-change="table.Selection = arguments[0]">
    </Table>
  </div>
</template>
<script>
export default {
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
        Loading: false,
        Selection: []
      }
    };
  },
  created () {
    this.GetAllBrands();
  },
  methods: {
    // 获取所有蓄电池品牌
    GetAllBrands () {
      this.ajax.get("/BaoYangBattery/GetBatteryBrands").then(response => {
        var res = response.data;
        this.brands = res || [];
      });
    },
    // 搜索
    Search () {
      var vue = this;
      if (!vue.searchData.Brand) {
        vue.$Message.warning("请选择蓄电池品牌");
        return;
      }
      vue.table.Loading = true;
      var brand = vue.searchData.Brand === "All" ? "" : vue.searchData.Brand;
      this.ajax
        .post("/BatteryCouponPriceDisplay/SelectConfig", { brand: brand })
        .then(response => {
          var res = response.data;
          vue.table.Loading = false;
          vue.table.Data = res.Data || [];
          if (!res.Status) {
            this.$Message.error("查询失败!");
          }
        });
    },
    // 更改是否展示配置
    UpsertConfig (col) {
      var vue = this;
      var model = this.util.deepCopy(col);
      model.IsShow = !model.IsShow;
      this.ajax
        .post("/BatteryCouponPriceDisplay/UpsertConfig", {
          model: model
        })
        .then(response => {
          var res = response.data || [];
          if (!res.Status) {
            vue.$Message.error("更改失败!" + (res.Msg || ""));
            col.IsShow = !col.IsShow;
            vue.$nextTick(() => {
              col.IsShow = !col.IsShow;
            });
          } else {
            vue.$Message.success("更改成功!" + (res.Msg || ""));
            setTimeout(() => {
              vue.RemoveCache([model.Pid]);
              vue.Search();
            }, 2000);
          }
        });
    },
    // 批量更改
    MultUpsertConfig (value) {
      this.$Modal.confirm({
        title: "全部" + (value ? "展示" : "隐藏"),
        content:
          "确认 全部" + (value ? "展示" : "隐藏") + " 蓄电池券后价数据？",
        loading: true,
        onOk: () => {
          var vue = this;
          var models = vue.table.Data || [];
          var length = models.length || 0;
          if (!length) {
            vue.$Message.warning("至少选择一条记录");
            return;
          }
          vue.ajax
            .post("/BatteryCouponPriceDisplay/MultUpsertConfig", {
              models: models,
              isShow: value
            })
            .then(response => {
              var res = response.data || [];
              if (!res.Status) {
                vue.$Message.error("更改失败!" + (res.Msg || ""));
              } else {
                vue.$Message.success("更改成功!" + (res.Msg || ""));
                setTimeout(() => {
                  vue.RemoveCache(models.map(s => s.Pid));
                  vue.Search();
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
        content: "确认导出 " + this.searchData.Brand + "蓄电池券后价数据？",
        loading: true,
        onOk: () => {
          this.$Modal.remove();
          window.location.href =
            "/BatteryCouponPriceDisplay/ExportExcel?brand=" +
            this.searchData.Brand;
        }
      });
    },
    // 移除缓存
    RemoveCache (pids) {
      this.ajax
        .post("/BatteryCouponPriceDisplay/RemoveCache", { pids: pids })
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
          content: "确定清除缓存？",
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
