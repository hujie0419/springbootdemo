<template>
  <div>
    <div>
      <Table stripe border :loading="table.loading" :columns="table.columns" :data="table.data"></Table>
      <div style="margin:15px 0;float:right">
        <Page show-total :total="page.total" :page-size="page.pageSize" :current="page.current" :page-size-opts="[10,20 ,50 ,100]" show-elevator
          show-sizer @on-change="handlePageChange" @on-page-size-change="handlePageSizeChange"></Page>
      </div>
    </div>
  </div>
</template>
<script>
// import producttable from '@/views/discount/producttable'
export default {
  name: "producttable",
  props: {
    activityId: String,
    dataType: Number, // 1表示正常，表示先获取负毛利，2获取新增数据，3获取修改数据，4获取删除数据
    auditDateTime: String
  },
  data () {
    return {
      page: {
        total: 0,
        current: 1,
        pageSize: 20
      },
      table: {
        loading: false,
        data: [],
        columns: [
          {
            title: "商品PID",
            key: "Pid",
            align: "center"
          },
          {
            title: "商品名称",
            key: "ProductName",
            align: "center"
          },
          {
            title: "商品总库存",
            key: "TotalStock",
            align: "center",
            width: 80,
             render: (h, params) => {
                  if (params.row.TotalStock === -999) {
                  return h(
                    "div", "不限库存"
                  );
                } else {
                  return h("div", params.row.TotalStock);
                }
              }
          },
          {
            title: "限购库存",
            key: "LimitQuantity",
            align: "center",
            width: 100
          }, {
                title: "已售数量",
                key: "SoldQuantity",
                align: "center",
                width: 100
              },
              {
                title: "剩余库存",
                align: "center",
                width: 100,
                render: (h, params) => {
                    var num = params.row.LimitQuantity - params.row.SoldQuantity;
                    if (num < 0) {
 return h('div', 0);
                    } else {
                        if (num < 10) {
return h('div', {style: {
                        color: 'red'
                      }}, num);
                        } else {
return h('div', num);
                        }
                    }
                }
              },
          {
            title: "成本价",
            key: "CostPrice",
            align: "center",
            width: 80,
            render: (h, params) => {
              return h("div", Number(params.row.CostPrice).toFixed(2));
            }
          },
          {
            title: "售价",
            key: "SalePrice",
            align: "center",
            width: 80,
            render: (h, params) => {
              return h("div", Number(params.row.SalePrice).toFixed(2));
            }
          },
          {
            title: "折后毛利",
            key: "DiscountMargin",
            align: "center",
            render: (h, params) => {
              if (params.row.IsMinusProfile) {
                return h(
                  "div",
                  {
                    style: {
                      color: "red"
                    }
                  },
                  params.row.DiscountMargin
                );
              } else {
                return h("div", params.row.DiscountMargin);
              }
            }
          },
          {
            title: "折后毛利率",
            key: "DiscountMarginRate",
            align: "center",
            render: (h, params) => {
              if (params.row.IsMinusProfile) {
                return h(
                  "div",
                  {
                    style: {
                      color: "red"
                    }
                  },
                  params.row.DiscountMarginRate
                );
              } else {
                return h("div", params.row.DiscountMarginRate);
              }
            }
          },
          {
            title: "备注",
            key: "Remark",
            align: "center"
          }
        ]
      }
    };
  },
  mounted () {
    this.loadData();
  },

  methods: {
    loadData () {
      if (this.dataType === 1) {
        var condition = {
          ActivityId: this.activityId
        };
        this.loadDataByCondition(condition);
      } else { // 加载上次审核后改动数据
          if (
        this.auditDateTime == null ||
        this.auditDateTime === undefined ||
        this.auditDateTime === ""
      ) {
        return false;
      }
        if (this.dataType === 2) {
          this.table.columns = [
            {
              title: "商品PID",
              key: "Pid",
              align: "center"
            },
            {
              title: "商品名称",
              key: "ProductName",
              align: "center"
            },
            {
              title: "商品总库存",
              key: "TotalStock",
              align: "center",
              width: 80,
               render: (h, params) => {
                  if (params.row.TotalStock === -999) {
                  return h(
                    "div", "不限库存"
                  );
                } else {
                  return h("div", params.row.TotalStock);
                }
              }
            },
            {
              title: "限购库存",
              key: "LimitQuantity",
              align: "center",
              width: 100
            },
            {
              title: "成本价",
              key: "CostPrice",
              align: "center",
              width: 80,
              render: (h, params) => {
                return h("div", Number(params.row.CostPrice).toFixed(2));
              }
            },
            {
              title: "售价",
              key: "SalePrice",
              align: "center",
              width: 80,
              render: (h, params) => {
                return h("div", Number(params.row.SalePrice).toFixed(2));
              }
            },
            {
              title: "折后毛利",
              key: "DiscountMargin",
              align: "center",
              render: (h, params) => {
                if (params.row.IsMinusProfile) {
                  return h(
                    "div",
                    {
                      style: {
                        color: "red"
                      }
                    },
                    params.row.DiscountMargin
                  );
                } else {
                  return h("div", params.row.DiscountMargin);
                }
              }
            },
            {
              title: "折后毛利率",
              key: "DiscountMarginRate",
              align: "center",
              render: (h, params) => {
                if (params.row.IsMinusProfile) {
                  return h(
                    "div",
                    {
                      style: {
                        color: "red"
                      }
                    },
                    params.row.DiscountMarginRate
                  );
                } else {
                  return h("div", params.row.DiscountMarginRate);
                }
              }
            },
            {
              title: "备注",
              key: "Remark",
              align: "center"
            },
            {
              title: "新增时间",
              key: "CreateDateTime",
              align: "center"
            }
          ];
          var conditionAdd = {
            ActivityId: this.activityId,
            AfterCreateDateTime: this.auditDateTime
          };
          this.loadDataByCondition(conditionAdd);
        } else if (this.dataType === 3) {
          this.table.columns = [
            {
              title: "商品PID",
              key: "Pid",
              align: "center"
            },
            {
              title: "商品名称",
              key: "ProductName",
              align: "center"
            },
            {
              title: "商品总库存",
              key: "TotalStock",
              align: "center",
              width: 80,
               render: (h, params) => {
                  if (params.row.TotalStock === -999) {
                  return h(
                    "div", "不限库存"
                  );
                } else {
                  return h("div", params.row.TotalStock);
                }
              }
            },
            {
              title: "限购库存",
              key: "LimitQuantity",
              align: "center",
              width: 100
            }, {
                title: "已售数量",
                key: "SoldQuantity",
                align: "center",
                width: 100
              },
              {
                title: "剩余库存",
                align: "center",
                width: 100,
                render: (h, params) => {
                    var num = params.row.LimitQuantity - params.row.SoldQuantity;
                    if (num < 0) {
 return h('div', 0);
                    } else {
                        if (num < 10) {
return h('div', {style: {
                        color: 'red'
                      }}, num);
                        } else {
return h('div', num);
                        }
                    }
                }
              },
            {
              title: "成本价",
              key: "CostPrice",
              align: "center",
              width: 80,
              render: (h, params) => {
                return h("div", Number(params.row.CostPrice).toFixed(2));
              }
            },
            {
              title: "售价",
              key: "SalePrice",
              align: "center",
              width: 80,
              render: (h, params) => {
                return h("div", Number(params.row.SalePrice).toFixed(2));
              }
            },
            {
              title: "折后毛利",
              key: "DiscountMargin",
              align: "center",
              render: (h, params) => {
                if (params.row.IsMinusProfile) {
                  return h(
                    "div",
                    {
                      style: {
                        color: "red"
                      }
                    },
                    params.row.DiscountMargin
                  );
                } else {
                  return h("div", params.row.DiscountMargin);
                }
              }
            },
            {
              title: "折后毛利率",
              key: "DiscountMarginRate",
              align: "center",
              render: (h, params) => {
                if (params.row.IsMinusProfile) {
                  return h(
                    "div",
                    {
                      style: {
                        color: "red"
                      }
                    },
                    params.row.DiscountMarginRate
                  );
                } else {
                  return h("div", params.row.DiscountMarginRate);
                }
              }
            },
            {
              title: "备注",
              key: "Remark",
              align: "center"
            },
            {
              title: "修改时间",
              key: "LastUpdateDateTime",
              align: "center"
            }
          ];
          var conditionUpdate = {
            ActivityId: this.activityId,
            AfterLastUpdateDateTime: this.auditDateTime
          };
          this.loadDataByCondition(conditionUpdate);
        } else if (this.dataType === 4) {
          this.table.columns = [
            {
              title: "商品PID",
              key: "Pid",
              align: "center"
            },
            {
              title: "商品名称",
              key: "ProductName",
              align: "center"
            },
            {
              title: "商品总库存",
              key: "TotalStock",
              align: "center",
              width: 80,
               render: (h, params) => {
                  if (params.row.TotalStock === -999) {
                  return h(
                    "div", "不限库存"
                  );
                } else {
                  return h("div", params.row.TotalStock);
                }
              }
            },
            {
              title: "限购库存",
              key: "LimitQuantity",
              align: "center",
              width: 100
            }, {
                title: "已售数量",
                key: "SoldQuantity",
                align: "center",
                width: 100
              },
              {
                title: "剩余库存",
                align: "center",
                width: 100,
                render: (h, params) => {
                    var num = params.row.LimitQuantity - params.row.SoldQuantity;
                    if (num < 0) {
 return h('div', 0);
                    } else {
                        if (num < 10) {
return h('div', {style: {
                        color: 'red'
                      }}, num);
                        } else {
return h('div', num);
                        }
                    }
                }
              },
            {
              title: "成本价",
              key: "CostPrice",
              align: "center",
              width: 80,
              render: (h, params) => {
                return h("div", Number(params.row.CostPrice).toFixed(2));
              }
            },
            {
              title: "售价",
              key: "SalePrice",
              align: "center",
              width: 80,
              render: (h, params) => {
                return h("div", Number(params.row.SalePrice).toFixed(2));
              }
            },
            {
              title: "折后毛利",
              key: "DiscountMargin",
              align: "center",
              render: (h, params) => {
                if (params.row.IsMinusProfile) {
                  return h(
                    "div",
                    {
                      style: {
                        color: "red"
                      }
                    },
                    params.row.DiscountMargin
                  );
                } else {
                  return h("div", params.row.DiscountMargin);
                }
              }
            },
            {
              title: "折后毛利率",
              key: "DiscountMarginRate",
              align: "center",
              render: (h, params) => {
                if (params.row.IsMinusProfile) {
                  return h(
                    "div",
                    {
                      style: {
                        color: "red"
                      }
                    },
                    params.row.DiscountMarginRate
                  );
                } else {
                  return h("div", params.row.DiscountMarginRate);
                }
              }
            },
            {
              title: "备注",
              key: "Remark",
              align: "center"
            },
            {
              title: "删除时间",
              key: "LastUpdateDateTime",
              align: "center"
            }
          ];
          var conditionDelete = {
            ActivityId: this.activityId,
            AfterLastUpdateDateTime: this.auditDateTime,
            IsDeleted: 1
          };
          this.loadDataByCondition(conditionDelete);
        }
      }
    },
    loadDataByCondition (condition) {
      this.table.loading = true;
      this.ajax
        .post("/salepromotionactivity/SelectProductList", {
          condition: condition,
          pageIndex: this.page.current,
          pageSize: this.page.pageSize
        })
        .then(response => {
          this.table.loading = false;
          var data = response.data;
          if (data.List != null) { 
              this.table.data = data.List; 
}
          this.page.total = data.Count;
        });
    },
    handlePageChange (pageIndex) {
      this.page.current = pageIndex;
      this.loadData();
    },
    handlePageSizeChange (pageSize) {
      this.page.pageSize = pageSize;
      this.loadData();
    }
  }
};
</script>
