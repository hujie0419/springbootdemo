<template>
<div style="padding-bottom:200px;">
  <Alert type="success" v-if="showAlert" show-icon closable>
        成功提示
        <span slot="desc">{{this.alertText}}</span>
    </Alert>
    <Table :columns="columns" :data="data"></Table>
    <div style="margin: 10px;">
        <div style="float: right;">
            <Page :total="page.total" :page-size="page.pageSize" :current="page.current" :page-size-opts="[10 ,20 ,50]" show-elevator show-sizer @on-change="handlePageChange" @on-page-size-change="handlePageSizeChange"></Page>
        </div>
    </div>

    <Modal
        v-model="showModal"
        title="查看优惠券的服务产品"
        width="900">
       <ServiceTableList ref="service" :opts="selectServices" :times="reloadSelectServices" :showFilter="true"></ServiceTableList>
        <div slot="footer">
            <Button type="primary" @click="ok">确定</Button>
        </div>
    </Modal>

</div>

</template>
<script>
import util from "@/framework/libs/util.js"
import ServiceTableList from '@/views/shoppromotion/serviceTableList.vue'
export default {
  props: ['status', 'opts', 'total', 'tabIndex'],
  data () {
    return {
      showAlert: false,
      showModal: false,
      selectServices: {},
      reloadSelectServices: null,
      alertText: '',
      columns: [
        {
          title: "RuleId",
          key: "RuleId"
        },
        {
          title: "券类型",
          key: "PromotionType",
          render: (h, params) => {
            let text = '';
            if (params.row.PromotionType === 0) {
              text = "满减券";
            }
            return h('span', {
              props: {

              }
            }, text)
          }
        },
        {
          title: "优惠券规则名称",
          key: "PromotionName",
          render: (h, params) => {
            return h('a', {
              on: {
                click: () => {
                  this.$router.push(
                    {
                      name: 'shoppromotionDetail', 
                      params: {
                        ruleId: params.row.RuleId
                      }
                    })
                }
              }
            }, params.row.PromotionName)
          }
        },
        {
          title: "面值",
          key: "Discount",
          render: (h, params) => {
            let text = params.row.Discount + '元';
            return h('span', {
              props: {

              }
            }, text)
          }
        },
        {
          title: "使用级别",
          key: "MinMoney",
          render: (h, params) => {
            let text = '满' + params.row.MinMoney + '减' + params.row.Discount;
            return h('span', {
              props: {
                
              }
            }, text)
          }
        },
        {
          title: "支持产品",
          key: "ShopProduct",
          render: (h, params) => {
            let texts = [];
            if (params.row.Products && params.row.Products.length) {
              var first = params.row.Products[0];
              if (first.Type === 0) {
                texts.push(h('a', {
                  props: {

                  },
                  on: {
                    click: () => {
                      this.selectServices = {}
                      this.showModal = true
                      this.$refs.service.loadList(this.selectServices)
                    }
                  }
                }, '全部服务'))
              } else {
                texts.push(h('a', {
                  props: {

                  },
                  on: {
                    click: () => {
                      this.selectServices = {}
                      params.row.Products.forEach(p => {
                        this.selectServices[p.ConfigValue] = {ProductID: p.ConfigValue}
                      })
                      this.showModal = true
                      this.$refs.service.loadList(this.selectServices)
                    }
                  }
                }, '指定服务'))
              }
              return h('div', texts);
            } else {
              return h('span', {
                props: {

                }
              }, '没有配置服务')
            }
          }
        },
        {
          title: "创建时间",
          key: "CreateDateTimeString"
        },
        {
          title: "最后编辑时间",
          key: "LastUpdateDateTimeString"
        },
        {
          title: "操作",
          key: "Operate",
          render: (h, params) => {
            let buttons = [];
            if (params.row.Status === 1) {
              buttons.push(h('Button', {
                props: {
                  type: 'text',
                  size: 'small'
                },
                on: {
                  click: () => {
                    this.updateStatus(params.row.RuleId, 2);
                  }
                }
              }, '暂停领取'))
            }
            if (params.row.Status === 2 || params.row.Status === 0) {
              buttons.push(h('Button', {
                props: {
                  type: 'text',
                  size: 'small'
                },
                on: {
                  click: () => {
                    this.updateStatus(params.row.RuleId, 1);
                  }
                }
              }, '上架领取'))
            }
            if (params.row.Status === 3) {
              buttons.push(h('Button', {
                props: {
                  type: 'text',
                  size: 'small'
                },
                on: {
                  click: () => {
                    this.$router.push({
                      name: "shoppromotionCreate",
                        params: {
                          ruleId: params.row.RuleId
                      }})
                  }
                }
              }, '复制新建'))
            } else {
              buttons.push(h('Button', {
                props: {
                  type: 'text',
                  size: 'small'
                },
                on: {
                  click: () => {
                    this.updateStatus(params.row.RuleId, 3);
                  }
                }
              }, '作废'))
            }
            
            return h('div', buttons);
          }
        }
      ],
      data: [],
      page: {
        total: 0,
        current: 1,
        pageSize: 10
      }
    };
  },
  components: {
    ServiceTableList
  },
  created () {
    this.$watch('opts.time', (o) => {
        this.page.current = 1;
        this.loadData();
    });
    this.$watch("showAlert", (n) => {
      if (n) {
        setTimeout(() => {
          this.showAlert = false;
        }, 1000);
      }
    })
    this.loadData();
  },
  methods: {
    loadData () {
          let params = {
            status: this.status,
            pageIndex: this.page.current,
            pageSize: this.page.pageSize
          };
          if (this.opts) {
            params.keywords = this.opts.keywords;
            params.discount = this.opts.discount;
            params.startDate = this.opts.filterStartDate;
            params.endDate = this.opts.filterEndDate;
          }
          util.ajax.get("/shoppromotion/GetCouponList", {
            params: params
          }).then((response) => {
            if (response.status === 200) {
              this.data = response.data.Source;
              this.page.total = response.data.Pager.TotalItem;
              this.$emit('update:total', this.page.total)
            }
          })
    },
    updateStatus (id, status) {
          util.ajax.post("/shoppromotion/UpdateCouponStatus", {
            ruleId: id,
            status: status
          }).then((response) => {
            if (response.status === 200 && response.data.data) {
              this.alertText = "操作成功";
              this.showAlert = true;
              this.$emit("loadListData")
            }
          })
    },
    toEdit (id) {
      this.$router.push({name: 'shoppromotionCreate', params: {ruleId: id}})
    },
    copy (id) {
      util.ajax.post("/shoppromotion/CopyCouponRule", {
            ruleId: id
          }).then((response) => {
            if (response.status === 200 && response.data.data) {
              this.alertText = "操作成功";
              this.showAlert = true;
              this.$emit("loadListData")
            }
          })
    },
    handlePageChange (pageIndex) {
      this.page.current = pageIndex;
      this.loadData();
    },
    handlePageSizeChange (pageSize) {
      this.page.pageSize = pageSize;
      this.loadData();
    },
    ok () {
        this.showModal = false
    }
  }
};
</script>
