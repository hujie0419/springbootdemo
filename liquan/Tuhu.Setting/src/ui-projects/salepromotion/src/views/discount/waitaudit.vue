<template>
  <div>
    <h1 class="title">促销活动审核</h1>

    <div style="margin:5px;font-size:14px;">活动信息</div>
    <div>
      <Row>
        <Col span=16>
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
            <td>活动限购数</td>
            <td>
              <span v-if="activity.Is_PurchaseLimit==0">不限购</span>
              <span v-else>{{activity.LimitQuantity}}</span>
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
        <Col span=6>
        <Button v-if="activity.AuditStatus==1" type="primary" style="width:80px;font-size:14px;" @click="passAudit">审 核</Button>
        <Button v-else-if="activity.AuditStatus==2" type="primary" disabled style="width:80px;font-size:14px;">已通过</Button>
        <Button v-else-if="activity.AuditStatus==3" type="error" disabled style="width:80px;font-size:14px;">已拒绝</Button>
        </col>
      </Row>
    </div>
    <h5 class="title"> </h5>
    <div style="margin:5px;font-size:14px;">商品信息</div>
    <!-- <Table stripe border :loading="products.table.loading" :columns="products.table.columns" :data="products.table.data"></Table>
    <div style="margin:15px 0;float:right">
      <Page :total="products.page.total" :page-size="products.page.pageSize" :current="products.page.current" :page-size-opts="[10,20 ,50 ,100]"
        show-elevator show-sizer @on-change="handleProductsPageChange" @on-page-size-change="handleProductsPageSizeChange"></Page>
    </div> -->
    <producttable ref="producttable" :activityId="$route.query.activityId" :dataType="1">
</producttable>
 <br> <br> <br>
 <div v-if="addAfterAudit.IsEditAfterAuidt">
      <div style="margin:5px;font-size:14px;">新增商品</div>
   <producttable ref="addAfterAudit" :activityId="$route.query.activityId" :auditDateTime="addAfterAudit.AuditDateTime" :dataType="2">
</producttable>
 <br> <br> <br>
 <div style="margin:5px;font-size:14px;">修改商品</div>
   <producttable ref="addAfterAudit" :activityId="$route.query.activityId" :auditDateTime="addAfterAudit.AuditDateTime" :dataType="3">
</producttable>
 <br> <br> <br>
 <div style="margin:5px;font-size:14px;">删除商品</div>
   <producttable ref="addAfterAudit" :activityId="$route.query.activityId" :auditDateTime="addAfterAudit.AuditDateTime" :dataType="4">
</producttable>
 <br> <br> <br>
 </div>
<loglist ref="loglist" :activityId="$route.query.activityId">
</loglist>
    <Modal v-model="auditModal.visible" title="审核活动" :loading="auditModal.loading" width="30px">
          <Form :model="auditModal" :label-width="100">
            <FormItem label="是否通过">
             <RadioGroup v-model="auditModal.auditStatus">
                <Radio label="2" >
                  <span>通过</span>
                </Radio>
                <Radio label="3" >
                  <span>拒绝</span>
                </Radio>
              </RadioGroup>
               <div style="color:red;">
         <span v-if="auditModal.auditStatus==3">* 拒绝后该活动将不能上线生效</span>
      </div>
            </FormItem>
            <FormItem v-if="auditModal.auditStatus==3" label="拒绝原因">
              <Input v-model="auditModal.auditRemark" type="textarea" :maxlength="200" placeholder="请输入拒绝原因，最多100汉字"
              style="width:260px;"></Input>
            </FormItem>
        </Form>
        <div slot="footer">
            <Button type="primary"  @click="setActivityAuditStatus">确认</Button>
            <Button type="ghost" @click="auditModal.visible=false">取消</Button>
        </div>
        </Modal>
  </div>
</template>
<script>
   import loglist from '@/views/discount/loglist'
    import producttable from '@/views/discount/producttable'
  export default {
      components: {
      loglist, producttable
    },
    data () {
      return {
          addAfterAudit: {
              IsEditAfterAuidt: false
          },
        activity: {
        },
        auditModal: {
            activityId: '',
          visible: false,
          loading: false,
          auditStatus: '2',
          auditRemark: ''
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
        }
      }
    },
    mounted () {
          this.ajax.post("/salepromotionactivity/GetActivityModel", {
        activityId: this.$route.query.activityId
      }).then(res => {
        if (res.data.Status) {
          this.activity = res.data.Data;
          if (this.activity.AuditStatus === 1 && this.activity.AuditDateTime != null && this.activity.AuditDateTime !== undefined) {
              this.addAfterAudit.IsEditAfterAuidt = true;
              this.addAfterAudit.AuditDateTime = this.activity.AuditDateTime;
          } else {
              this.addAfterAudit.IsEditAfterAuidt = false;
          }
        } else {
        }
      });
    //  this.loadData();
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
              activityId: this.activity.ActivityId
            },
            pageIndex: this.products.page.current,
            pageSize: this.products.page.pageSize
          })
          .then(response => {
            this.products.table.loading = false;
            var data = response.data;
            this.products.table.data = data.List;
            this.products.page.total = data.Count;
          });
      },
      setActivityAuditStatus () {
          let auditStatus = this.auditModal.auditStatus;
          var auditRemark = this.auditModal.auditRemark.trim();
          if (auditStatus === '3' && (auditRemark === '' || auditRemark == null)) {
this.messageInfo("请输入拒绝原因");
return false;
          }
this.ajax
          .post("/salepromotionactivity/setActivityAuditStatus", {
            activityId: this.$route.query.activityId,
            auditStatus: this.auditModal.auditStatus,
            auditRemark: this.auditModal.auditRemark
          })
          .then(response => {
            if (response.data.Status) {
this.auditStatus = auditStatus;
this.activity.AuditStatus = auditStatus;
this.auditModal.visible = false;
this.$refs.loglist.loadLog();
this.messageInfo("审核成功"); 
} else {
this.messageInfo(response.data.Msg); 
return false;
            }
          });
      },
      passAudit () {
        this.auditModal.visible = true;
      }, // 工具函数
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
