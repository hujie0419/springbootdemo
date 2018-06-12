<template>
  <div>
      <h1 class="title">拼团优惠券配置</h1>
      <div>
        <label>商品PID</label>
        <Input type="text" placeholder="商品名称" style="width: 200px" v-model="filters.PID" />
      </div>
      <div style="margin-top:18px">
          <Button type="success" icon="search" @click="loadData(1)">搜索</Button>
          <Button type="success" icon="plus" style="margin-left:20px" @click="search(0)">创建</Button>
          <Button type="success" icon="load-c" style="margin-left:20px;" @click="SearchLog(1)">日志</Button>
      </div>
      <div style="margin-top:18px">
        <Table  :loading="table.loading" :data="table.data" :columns="table.columns" style="width:100%" stripe></Table>
        <div style="margin: 10px;overflow: hidden">
            <div style="float: right;">
                <Page :total="page.total" :page-size="page.pageSize" :current="page.current" :page-size-opts="[5 ,10 ,20 ,50]"  show-sizer @on-change="handlePageChange" @on-page-size-change="handlePageSizeChange"></Page>
            </div>
        </div>
      </div>
      <Modal v-model="modal.visible" :loading="modal.loading" title="拼团优惠券配置（编辑）" okText="提交" :transfer="false" cancelText="取消" @on-ok="SaveProductCouponConfig"   width="50%">
        <Form ref="modal.productConfig" :model="modal.productConfig" :rules="modal.rules" :label-width="90">
            <FormItem label="PID" prop="PID">
                <input v-model="modal.productConfig.PID" :disabled="!isAdd" placeholder="自动生成"  @blur="FetchProductInfo(modal.productConfig.PID)" />
            </FormItem>
            <FormItem label="商品名称">
                <Input v-model="modal.productConfig.ProductName" :disabled="true" />
            </FormItem>
            <FormItem label="商品价格">
                <Input v-model="modal.productConfig.ProductPrice" :disabled="true"  />
            </FormItem>
            <FormItem v-for="(item, index) in modal.productConfig.Coupons" :label="'优惠券-'+(index+1)" :key="index" :prop="'executors.' + index">
                <Row>
                    <!-- <Col span="3">
                     <button @click="deleteCoupon(index)">删除</button>
                    </col> -->
                    <input style="display:inline-block;width:200px;"  type="text" :disabled="!item.AllowEdit" :key="index" @blur="FetchCouponInfo(item.CouponId,index)" v-model="item.CouponId" placeholder="优惠券PID" />
                    <Input style="display:inline-block;width:200px;"  type="text" v-model="item.CouponName" :disabled="true" placeholder="优惠券名称" />
                    <label style="display:inline-block;width:50px;font-size: 10px;" >面额(元)</label>
                    <Input style="display:inline-block;width:100px" type="text" v-model="item.CouponPrice" :disabled="true"/>
                </Row>
                <Row>
                    <label style="display:inline-block;width:100px;font-size: 10px;" >虚拟成本比例(%)</label>
                    <Input style="display:inline-block;width:200px" type="text"  v-model="item.CouponRate" placeholder="1-100" />
                    <label style="display:inline-block;width:100px;font-size: 10px;">使用条件(元)</label>
                    <Input style="display:inline-block;width:200px" type="text" v-model="item.CouponLeastPrice" :disabled="true"/>
                </Row>
                <Row>
                    <label style="display:inline-block;width:100px;font-size: 10px;">优惠券描述</label>
                    <Input style="display:inline-block;width:200px" type="text"  v-model="item.CouponDescription"  :disabled="true"/>
                    <label style="display:inline-block;width:100px;font-size: 10px;">优惠券有效期</label>
                    <Input style="display:inline-block;width:200px"  type="text" v-model="item.AvailablePeriod" :disabled="true"/>
                </Row>
            </FormItem>
            <formItem> 
               <button @click="AddCoupon">添加优惠券 </button>
            </formItem>
        </Form>
      </Modal>
      <Modal v-model="logmodal.visible" title="操作日志" cancelText="取消"   :width="logmodal.width">
        <Table :loading="logmodal.loading" :data="logmodal.data" :columns="logmodal.columns" stripe></Table>
         <div style="margin: 10px;overflow: hidden">
            <div style="float: right;">
                <Page :total="logpage.total" :page-size="logpage.pageSize" :current="logpage.current" :page-size-opts="[5 ,10]"  show-sizer @on-change="handleLogPageChange" @on-page-size-change="handleLogPageSizeChange"></Page>
            </div>
        </div>
      </Modal>  
  </div>
</template>
<style>
body .ivu-modal .ivu-select-dropdown {
    position: fixed !important;
}
</style>
<script>
  export default {
    data () {
        return {
          index: 0,
          isAdd: 1,
          page: {
            total: 0,
            current: 1,
            pageSize: 5
          },
          logpage: {
            total: 0,
            current: 1,
            pageSize: 5
          },
          filters: {
            PID: ""
          },
          tablemodel: {
            loading: true,
            visible: false,
            width: 1355,
            data: []
          },
          logmodal: {
              loading: true,
              visible: false,
              width: 635,
              data: [],
              columns: [
                  {
                      title: "操作人",
                      width: 150,
                      key: "Creator",
                      align: "center",
                      fixed: "left"
                  },
                  {
                      title: "时间",
                      width: 150,
                      key: "CreateDateTime",
                      render: (h, params) => {
                          return h(
                              "span",
                              this.formatDate(params.row.CreateDateTime)
                          );
                      }
                  },
                  {
                      title: "操作内容",
                      width: 300,
                      key: "Remark",
                      align: "center",
                      fixed: "left"
                  }
              ]
          },
          table: {
            loading: true,
            data: [],
            columns: [
              {
                title: "#",
                width: 50,
                align: "center",
                fixed: "left",
                type: "index"
              },
              {
                title: "商品PID",
                key: "PID",
                width: 180
              },
              {
                title: "商品名称",
                key: "ProductName"
              },
              {
                title: "上下架状态",
                width: 100,
                key: "IsActiveStr"
              },
              {
                title: "商品价格",
                width: 100,
                key: "ProductPrice"
              },
              {
                title: "优惠券数量",
                width: 100,
                key: "CouponCount"
              },
              {
                title: "创建时间",
                width: 150,
                key: "CreateTimeStr"
              },
              {
                title: "更新时间",
                width: 150,
                key: "UpdateTimeStr"
              },
              {
                title: "操作",
                width: 100,
                align: "center",
                render: (h, params) => {
                  return h("div", [
                      h(
                          "Button",
                          {
                              props: {
                                  type: "primary",
                                  size: "small"
                              },
                              style: {
                                  marginRight: "5px"
                              },
                              on: {
                                  click: () => {
                                      this.search(
                                          params.row.PID,
                                          "Search"
                                      )
                                  }
                              }
                          },
                          "修改/查询"
                      )
                  ])
                }
              }
              
            ]
          },
          modal: {
            visible: false,
            loading: true,
            edit: true,
            productConfig: {
                PID: "",
                ProductName: "",
                ProductPrice: 0,
                IsActive: true,
                CouponCount: 0,
                CreateTime: "",
                UpdateTime: "",
                Coupons: [],
                CreateTimeStr: "",
                UpdateTimeStr: "",
                IsActiveStr: ""
            }
          },
          coupon: {
            CouponId: "",
            CouponName: "",
            CouponPrice: "",
            CouponRate: "",
            CouponLeastPrice: "",
            CouponDescription: "",
            AvailablePeriod: "",
            AllowEdit: true
          }
        }
    },
    created: function () {
      this.loadData(1);
    },
    methods: {
      loadData (pageIndex) {
        this.page.current = pageIndex;
        this.table.loading = true;
        var requestData = "?PID=" + this.filters.PID;
        requestData += "&pageIndex=" + this.page.current;
        requestData += "&pageSize=" + this.page.pageSize;
        this.ajax
            .get("/GroupBuyingV2/PT_GetCouponList" + requestData)
            .then(response => {
              var data = response.data;
              this.page.total = data.totalCount;
              this.table.data = data.data;
              this.table.loading = false;
            });
      },
      search (pid) {
        if (pid) {
            this.ajax
          .get("/GroupBuyingV2/VirtualProductCouponConfigDetail?pid=" + pid)
          .then(response => {
              if (response.data.status === 2) {
                  this.$Message.warning(response.data.errMsg)
              } else if (response.data.status === 0) {
                this.modal.productConfig = response.data.data;
                this.modal.visible = true;
                this.isAdd = 2;
              }  
          });
        } else {
          this.modal.productConfig = {
              PID: "",
              ProductName: "",
              ProductPrice: 0,
              IsActive: true,
              CouponCount: 0,
              CreateTime: "",
              UpdateTime: "",
              Coupons: [],
              CreateTimeStr: "",
              UpdateTimeStr: "",
              IsActiveStr: ""
            }
            this.modal.visible = true;
            this.isAdd = 1;
        }      
      },
      SearchLog (pageIndex) {
        var that = this;
        that.logmodal.loading = true;
        that.logpage.current = pageIndex;
        that.ajax
            .post("/CommonConfigLog/GetCommonConfigLogList", {
                objectType: "VirtualProductCouponConfig",
                page: that.logpage.current,
                rows: that.logpage.pageSize,
                sord: "asc"
            })
            .then(response => {
                var data = response.data;
                this.logmodal.data = data.rows;
                this.logpage.total = data.records;
                this.logmodal.visible = true;
                this.logmodal.loading = false;
            });
      },
      AddCoupon () {
        var that = this;
        if (!that.modal.productConfig.ProductName) {
          that.$Message.warning("请先输入有效的商品信息");
          return;
        }
        that.modal.productConfig.Coupons.push({
            CouponId: "",
            CouponName: "",
            CouponPrice: "",
            CouponRate: "",
            CouponLeastPrice: "",
            CouponDescription: "",
            AvailablePeriod: "",
            AllowEdit: true
          })
      },
      FetchCouponInfo (PID, index) {
        var that = this;
        if (PID) {
          that.ajax
          .post("/GroupBuyingV2/FetchCouponInfo", {
              couponId: PID
          })
          .then(response => {
            if (response.data) {
              that.modal.productConfig.Coupons[index].AvailablePeriod = response.data.AvailablePeriod;
              that.modal.productConfig.Coupons[index].CouponDescription = response.data.CouponDescription;
              that.modal.productConfig.Coupons[index].CouponId = response.data.CouponId;
              that.modal.productConfig.Coupons[index].CouponLeastPrice = response.data.CouponLeastPrice;
              that.modal.productConfig.Coupons[index].CouponName = response.data.CouponName;
              that.modal.productConfig.Coupons[index].CouponPrice = response.data.CouponPrice;
            } else {
              that.$Message.warning("优惠券 PID 无效");
            } 
          });
        }    
      },
      SaveProductCouponConfig () {
        var that = this;
        var submitData = [];
        var sumCouponRate = 0;
        if (!that.modal.productConfig.PID || !that.modal.productConfig.ProductName || !that.modal.productConfig.ProductPrice) {
          that.$Message.warning("商品信息不能为空或者不正确");
          that.$nextTick(
            () => {
                    that.modal.loading = true;
                  }
          );
          that.modal.loading = false;
          return;
        }
        for (var i = 0; i < that.modal.productConfig.Coupons.length; i++) {
            if (that.modal.productConfig.Coupons[i].CouponId) {
                if (that.modal.productConfig.Coupons[i].CouponRate > 0) {
                    sumCouponRate += parseInt(that.modal.productConfig.Coupons[i].CouponRate)
                } else {
                  that.$Message.warning("优惠券的成本比例只能是不能为0的整数");
                  that.$nextTick(
                            () => {
                                    that.modal.loading = true;
                                  }
                          );
                  that.modal.loading = false;
                  return;
                }   
                submitData.push({
                    PKID: that.modal.productConfig.Coupons[i].PKID,
                    PID: that.modal.productConfig.PID,
                    CouponId: that.modal.productConfig.Coupons[i].CouponId,
                    CouponRate: that.modal.productConfig.Coupons[i].CouponRate
                });
            }
        }
        if (sumCouponRate !== 100) {
          that.$Message.warning("优惠券的百分百总和不为100%");
                  that.$nextTick(
                            () => {
                                    that.modal.loading = true;
                                  }
                          );
                  that.modal.loading = false;
                  return;
        }
        if (submitData.length === 0) {
          that.$Message.warning("商品优惠券不能为空");
          that.$nextTick(
            () => {
                    this.modal.loading = true;
                  }
          );
          that.modal.loading = false;
          return;
        }
        var configJson = JSON.stringify(submitData)
        that.modal.loading = true;
        that.ajax
            .post("/GroupBuyingV2/SaveProductCouponConfig", {
                configJson: configJson,
                operType: that.isAdd 
            })
            .then(response => {
                if (!response.data) {
                    that.$Message.success("操作成功");
                    that.modal.visible = false;
                    that.loadData(that.page.current);
                } else {
                    that.$Message.error(response.data);
                    that.$nextTick(() => {
                        that.modal.loading = true;
                    });
                }
                that.modal.loading = false;
            });
      },
      FetchProductInfo (PID) {
        var that = this;
        if (PID) {
          that.ajax
          .post("/GroupBuyingV2/FetchProductInfo", {
              pid: PID
          })
          .then(response => {
            if (response.data) {
              that.modal.productConfig.PID = response.data.PID;
              that.modal.productConfig.ProductName = response.data.ProductName;
              that.modal.productConfig.ProductPrice = response.data.ProductPrice
            } else {
              that.$Message.warning("商品 PID 无效");
            } 
          });
        }    
      },
      handlePageChange (pageIndex) {
          this.loadData(pageIndex);
      },
      handlePageSizeChange (pageSize) {
          this.page.pageSize = pageSize;
          this.loadData(this.page.current);
      },
      deleteCoupon (index) {
          this.modal.productConfig.Coupons.splice(index, 1);
      },
      handleLogPageChange (pageIndex) {
          var that = this;
          that.SearchLog(pageIndex);
      },
      handleLogPageSizeChange (pageSize) {
          var that = this;
          that.logpage.pageSize = pageSize;
          that.loadData(this.logpage.current);
      },
      formatDate (value) {
        if (value) {
            var time = new Date(
                value
            );
            var year = time.getFullYear();
            var day = time.getDate();
            var month = time.getMonth() + 1;
            var hours = time.getHours();
            var minutes = time.getMinutes();
            var seconds = time.getSeconds();
            var func = function (value, number) {
                var str = value.toString();
                while (str.length < number) {
                    str = "0" + str;
                }
                return str;
            };
            if (year === 1) {
                return "";
            } else {
                return (
                    func(year, 4) +
                    "-" +
                    func(month, 2) +
                    "-" +
                    func(day, 2) +
                    " " +
                    func(hours, 2) +
                    ":" +
                    func(minutes, 2) +
                    ":" +
                    func(seconds, 2)
                );
            }
        }
      }
    }
  }
</script>
