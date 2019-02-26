<template>
  <div>
    <h1 class="title">查看促销活动</h1>
    <div style="margin:5px;font-size:14px;">活动信息</div>
    <div>
      <Row>
        <Col>
        <table style="font-size:12px;margin:0 25px;">
          <tr>
            <td style="width:80px;">活动名称</td>
            <td>{{activity.Name}}</td>
          </tr>
          <tr>
            <td>活动时间</td>
            <td>{{activity.StartTime}} 至 {{activity.EndTime}}</td>
          </tr>
          <tr>
            <td>打折方式</td>
            <td>
              <div v-if="computeDiscountMethod==1">
                <span v-for="(item,index) in activity.DiscountContentList" :key="index" style="margin-right:20px;">
                  满 {{item.Condition}} 元 享 {{item.DiscountRate/10}} 折</span>
              </div>
              <div v-else-if="computeDiscountMethod==2">
                <span v-for="(item,index) in activity.DiscountContentList" :key="index" style="margin-right:20px;">
                  满 {{item.Condition}} 件 享 {{item.DiscountRate/10}} 折</span>
              </div>
            </td>
          </tr>
          <tr>
            <td>标签名称</td>
            <td>
              <span v-if="activity.Is_DefaultLabel==1">折(默认标签)</span>
              <span v-else>{{activity.Label}}</span>
            </td>
          </tr>
          <tr>
            <td>会场限购数</td>
            <td>
              <span v-if="activity.Is_PurchaseLimit==0">不限购</span>
              <span v-else>{{activity.LimitQuantity}} 件</span>
            </td>
          </tr>
          <tr>
            <td>创建人</td>
            <td>{{activity.CreateUserName}}</td>
          </tr>
          <tr>
            <td>创建时间</td>
            <td>{{activity.CreateDateTime}}</td>
          </tr>
          <tr>
            <td>审核状态</td>
            <td>
              <span v-if="activity.AuditStatus==0">无</span>
              <span v-else-if="activity.AuditStatus==1">待审核</span>
              <span v-else-if="activity.AuditStatus==2">已通过</span>
              <span v-else-if="activity.AuditStatus==3">已拒绝</span>
            </td>
          </tr>
        </table>
        </Col>

      </Row>
    </div>
    <h5 class="title"> </h5>
    <Row>
      <Col span="4">
      <div style="margin:5px;font-size:14px;">商品信息</div>
      </Col>
      <Col>
      <Button style="float:right;margin:5px;" v-if="activity.AuditStatus!=1" type="primary" @click="editProduct">修改活动商品</Button>
      <Button style="float:right;margin:5px;" v-if="activity.AuditStatus!=1" type="primary" @click="editActivity">修改活动信息</Button>
      <Button style="float:right;margin:5px;" icon="loop" type="ghost" @click="refreshProduct">同步商品数据</Button>
       </Col>
    </Row>
    <div>
      <Table stripe border :loading="products.table.loading" :columns="products.table.columns" :data="products.table.data"></Table>
    </div>
    <div style="margin:15px 0;float:right">
      <Page show-total :total="products.page.total" :page-size="products.page.pageSize" :current="products.page.current" :page-size-opts="[10,20 ,50 ,100]"
        show-elevator show-sizer @on-change="handleProductsPageChange" @on-page-size-change="handleProductsPageSizeChange">
      </Page>
    </div>
     <br> <br> <br>
       <h5 class="title"> </h5>
    <Row>
      <Col span="4">
      <div style="margin:5px;font-size:14px;">日志信息</div>
      </Col>
    </Row>
    <div>
      <Table stripe border  @on-expand="expandLogTable" :loading="log.table.loading" :columns="log.table.columns" :data="log.table.data"></Table>
    </div>
    <div style="margin-top:15px;float:right">
      <Page show-total :total="log.page.total" :page-size="log.page.pageSize" :current="log.page.current" :page-size-opts="[10,20 ,50 ,100]"
        show-elevator show-sizer @on-change="handleLogPageChange" @on-page-size-change="handleLogPageSizeChange">
      </Page>
    </div>
  </div>
</template>
<script>
   import expandrow from '@/views/discount/expandrow'
  export default {
       components: { expandrow },
    data () {
      return {
        activity: {
        },
        // 商品列表数据
        products: {
          page: {
            total: 0,
            current: 1,
            pageSize: 20
          },
          table: {
            loading: false,
            data: [],
            columns: [{
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
                width: 100,
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
                  return h('div', Number(params.row.CostPrice).toFixed(2));
                }
              },
              {
                title: "售价",
                key: "SalePrice",
                align: "center",
                width: 80,
                render: (h, params) => {
                  return h('div', Number(params.row.SalePrice).toFixed(2));
                }
              },
              {
                title: "折后毛利",
                key: "DiscountMargin",
                align: "center",
                render: (h, params) => {
                  if (params.row.IsMinusProfile) {
                    return h('div', {
                      style: {
                        color: 'red'
                      }
                    }, params.row.DiscountMargin)
                  } else {
                    return h('div', params.row.DiscountMargin)
                  }
                }
              },
              {
                title: "折后毛利率",
                key: "DiscountMarginRate",
                align: "center",
                render: (h, params) => {
                  if (params.row.IsMinusProfile) {
                    return h('div', {
                      style: {
                        color: 'red'
                      }
                    }, params.row.DiscountMarginRate)
                  } else {
                    return h('div', params.row.DiscountMarginRate)
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
        }, 
log: {
          page: {
            total: 0,
            current: 1,
            pageSize: 20
          },
          table: {
            loading: false,
            data: [],
            columns: [{
                        type: 'expand',
                        width: 50,
                        render: (h, params) => {
                            return h(expandrow, {
                                props: {
                                    row: params.row
                                }
                            });
                    }
                    }, {
                title: "操作",
                key: "OperationLogDescription",
                align: "center"
              },
              {
                title: "操作时间",
                key: "CreateDateTime",
                align: "center"
              },
              {
                title: "操作人",
                key: "CreateUserName",
                align: "center"
              }
            ]
          }
        }
      }
    },
    mounted () {
      this.ajax.post("/salepromotionactivity/GetActivityModel", {
        activityId: this.$route.query.activityId
      }).then(res => {
        if (res.data.Status) {
          this.activity = res.data.Data;
        } else {
        }
      });
      this.loadData();
      this.loadLog();
    },
    computed: {
      computeDiscountMethod: function () {
        if (this.activity.DiscountContentList == null || this.activity.DiscountContentList === undefined ||
          !(this.activity.DiscountContentList.length > 0)) {
          return 0;
        }
        return this.activity.DiscountContentList[0].DiscountMethod;
      }
    },
    methods: {
      loadData (pageIndex) {
        if (pageIndex > 0) {
          this.products.page.current = pageIndex;
        }
        this.activity.ActivityId = this.$route.query.activityId;
        this.products.table.loading = true;
        this.ajax
          .post("/salepromotionactivity/SelectProductList", {
            condition: {
              ActivityId: this.activity.ActivityId
            },
            pageIndex: this.products.page.current,
            pageSize: this.products.page.pageSize
          })
          .then(response => {
            this.products.table.loading = false;
            var data = response.data;
            if (data.List != null && data.List !== undefined) {
 this.products.table.data = data.List;
            }
            this.products.page.total = data.Count;
          });
      },
loadLog () {
     this.log.table.loading = true;
        this.activity.ActivityId = this.$route.query.activityId;
        this.ajax
          .post("/salepromotionactivity/GetOperationLogList", {
              activityId: this.activity.ActivityId,
            pageIndex: this.log.page.current,
            pageSize: this.log.page.pageSize
          })
          .then(response => {
            this.log.table.loading = false;
            var data = response.data;
            this.log.table.data = data.List;
            this.log.page.total = data.Total;
          });
},
 // 同步商品信息
      refreshProduct () {
          if (this.products.table.data == null || this.products.table.data === undefined || !(this.products.table.data.length > 0)) {
              this.messageInfo("活动暂无商品");
             return false;
          }
        this.products.table.loading = true;
        this.ajax
          .post("/salepromotionactivity/RefreshProductInfo", {
            activityId: this.$route.query.activityId
          })
          .then(res => {
            this.products.table.loading = false;
            if (res.data.Status) {
              this.messageInfo("同步成功");
              this.loadData();
            } else {
              this.messageInfo(res.data.Msg);
            }
          });
      },
expandLogTable (row) {
},
      editActivity () {
        let activityId = this.$route.query.activityId;
        if (activityId == null || activityId === undefined) {
          // 获取活动id失败
        } else {
          this.$router.push({
            path: "/discount/editactivity",
            query: {
              activityId: activityId
            }
          });
        }
      },
      editProduct () {
        let activityId = this.$route.query.activityId;
        if (activityId == null || activityId === undefined) {
          // 获取活动id失败
        } else {
          this.$router.push({
            path: "/discount/activityproduct",
            query: {
              activityId: activityId
            }
          });
        }
      },
      handleProductsPageChange (pageIndex) {
        this.products.page.current = pageIndex;
        this.loadData();
      },
      handleProductsPageSizeChange (pageSize) {
        this.products.page.pageSize = pageSize;
        this.loadData();
      },
         handleLogPageChange (pageIndex) {
        this.log.page.current = pageIndex;
        this.loadLog();
      },
      handleLogPageSizeChange (pageSize) {
        this.log.page.pageSize = pageSize;
        this.loadLog();
      },
       messageInfo (value) {
        this.$Message.info({
          content: value,
          duration: 3,
          closable: true
        });
      }
    }
  };

</script>

<style scoped>

</style>
