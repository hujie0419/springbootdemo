<template>
  <div>
    <Form>
      <FormItem>
        <Row>
          <Col>
            <Select v-model="inputProp.keyWordSearchType" style="width: 100px;">
              <Option v-for="item in keyWordSearchTypeSelect" :value="item.value" :key="item.value">{{ item.label }}</Option>
            </Select>
            <Input v-model="inputProp.keyWord" placeholder="请输入关键字" style="width: 300px;"></Input>
            <Button type="primary" v-on:click="loadData(1)" icon="ios-search" style="margin-left: 20px;">搜索</Button>
            <Button v-on:click="ResetInputProp()" style="margin-left: 20px;">重置</Button>
            <Button type="primary" v-on:click="showLayer_CouponsQuery()" style="margin-left: 20px;">添加优惠券</Button>
          </Col>
        </Row>
      </FormItem>
      <FormItem>
        <Row>
          <Col span="6">
            <span>是否缺货：</span>
            <Select v-model="inputProp.isOutOfStock">
              <Option v-for="item in isOutOfStockSelect" :value="item.value" :key="item.value">{{ item.label }}</Option>
            </Select>
          </Col>
          <!-- <Col span="6">
          <span>采购价格：</span>
          <InputNumber :min="0" :step="0.01" v-model="inputProp.purchaseMinPrice"></InputNumber>
          &nbsp;-&nbsp;
          <InputNumber :min="0" :step="0.01" v-model="inputProp.purchaseMaxPrice"></InputNumber></Col> -->
          <Col span="6">
            <span>是否代发：</span>
            <Select v-model="inputProp.isDaifa">
              <Option v-for="item in isDaifaSelect" :value="item.value" :key="item.value">{{ item.label }}</Option>
            </Select>
          </Col>
          <Col span="6">
            <span>上下架：</span>
            <Select v-model="inputProp.onSale">
              <Option v-for="item in upStateSelect" :value="item.value" :key="item.value">{{ item.label }}</Option>
            </Select>
          </Col>
        </Row>
      </FormItem>
      <FormItem>
        <Row>
          <Col span="6">
            <span>是否有拼团价：</span>
            <Select v-model="inputProp.isHavePintuanPrice">
              <Option v-for="item in isHavePintuanPriceSelect" :value="item.value" :key="item.value">{{ item.label }}</Option>
            </Select>
          </Col>
          <Col span="6">
            <span>商品类目：</span>
            <Cascader :data="categoryData" v-model="inputProp.categorys"  change-on-select style="width: 200px;display: inline-block;" @on-change="GetMerchandiseBrand"></Cascader>
          </Col>
          <Col span="6">
            <span>商品品牌：</span>
            <Select v-model="inputProp.merchandiseBrand">
              <Option v-for="item in merchandiseList" :value="item.Cp_Brand" :key="item.Cp_Brand">{{ item.Cp_Brand }}</Option>
            </Select>
          </Col>
        </Row>
      </FormItem>
      <FormItem>      
        <Row>
          <Col span='6'>
            <span>共<em style='color: #2b85e4;'> {{ totalRecord }} </em>条数据</span>&nbsp; &nbsp;
            <Button icon='arrow-down-a' v-on:click='ExportExcel()'>导出数据</Button>
          </Col>
        </Row>      
      </FormItem>
    </Form>
    <Table border :loading='tbLoading' :columns="columns" :data="filtedList" no-data-text="暂无数据"></Table>
    <br/>
    <Page :total="totalRecord" :current='inputProp.curPage' :page-size="inputProp.pageSize" @on-change='changePage'></Page>
    <Modal v-model="OpenModal" width="80%">
      <p slot="header" >历史记录</p>
      <Table border :columns="hcolumns" :data="hTbData" no-data-text="暂无数据"></Table>
      <div slot="footer"></div>
    </Modal>
    <Modal class='modal-table' v-model="isShowLayer_CouponsQuery"  title="添加优惠券" okText="提交" :transfer="false" cancelText="取消" @on-ok="loadData(1)"  width="80%">
        <Button class="blue" icon="plus" @click="AddCoupon" style="margin-bottom: 10px;">添加优惠券 </Button>
        <table style="width: 100%;">
          <thead>
            <tr>
              <th style="width: 10px;">#</th>
              <th style="width: 280px;">优惠券规则Guid</th>
              <th>优惠券说明</th>
              <th style="width: 20px;">额度</th>
              <th style="width: 20px;">条件</th>
              <th style="width: 80px;">有效期</th>
              <th style="width: 100px;">起始时间</th>
              <th style="width: 50px;">操作</th>
            </tr>
          </thead>
          <tbody v-for="(item, index) in inputProp.coupons" :label="'优惠券-'+(index+1)" :key="index" :prop="'executors.' + index">
            <tr> 
              <th>{{index+1}}</th>
              <th><input v-model="item.GetRuleGUID" placeholder="请输入优惠券规则Guid" style="width: 240px;"  @blur="FetchCouponInfo(item.GetRuleGUID,index)" ></input></th>
              <th>{{item.Description}}</th>
              <th>{{item.Discount | KeepToNum}}</th>
              <th>{{item.Minmoney| KeepToNum}}</th>
              <th>{{item.CouponDuration}}</th>
              <th>{{item.CouponStartTime}}</th>
              <th><Button class="blue"  @click="DeleteCoupon(index)">删除</Button></th>
            </tr>
          </tbody>
      </table>
    </Modal>
    <Modal class='modal-table' v-model="isShowLayer_Coupons" title="优惠券使用详情"  width="80%">
      <table style="width: 100%;">
        <thead>
          <tr>
            <th style="width: 10px;">#</th>
            <th style="width: 100px;">是否可用</th>
            <th style="width: 100px;">全网活动价</th>
            <th style="width: 100px;">原价</th>
            <th style="width: 100px;">券后价格</th>
            <th style="width: 100px;">券后毛利</th>
            <th style="width: 250px;">优惠券规则编号</th>
            <th>优惠券说明</th>
            <th style="width: 20px;">额度</th>
            <th style="width: 20px;">条件</th>
            <th style="width: 80px;">有效期</th>
            <th style="width: 100px;">起始时间</th>
          </tr>
        </thead>
         <tbody v-for="(item, index) in couponsDetail" :label="'优惠券-'+(index+1)" :key="index" :prop="'executors.' + index">
          <tr>
            <th>{{index+1}}</th>
            <th>{{item.IsUseful?'是':'否'}}</th>
            <th>{{row.FullNetActivityPrice | KeepToNum}}</th>
            <th>{{row.OriginalPrice | KeepToNum}}</th>
            <th>{{item.UsedCouponPrice | KeepToNum}}</th>
            <th>{{item.UsedCouponProfit| KeepToNum}}</th>
            <th style="width: 180px;">{{item.GetRuleGUID}}</th>
            <th>{{item.Description}}</th>
            <th>{{item.Discount | KeepToNum}}</th>
            <th>{{item.Minmoney | KeepToNum}}</th>
            <th>{{item.CouponDuration}}</th>
            <th>{{item.CouponStartTime}}</th>
          </tr>
        </tbody>
      </table>
    </Modal>
    <Modal class='modal-table' v-model="isShowLayer_AvailableStock" title="可用库存详情"  width="80%">
      <table style="width: 100%;">
        <thead>
          <tr>
            <th style="width: 250px;">义乌仓可用库存</th>
            <th style="width: 250px;">上海仓可用库存</th>
            <th style="width: 250px;">武汉仓可用库存</th>
            <th style="width: 250px;">北京仓可用库存</th>
            <th style="width: 250px;">广州仓可用库存</th>
          </tr>
        </thead>
         <tbody>
          <tr> 
            <th>{{availableStockDetail.YW_AvailableStockQuantity}}</th>
            <th>{{availableStockDetail.SH_AvailableStockQuantity}}</th>
            <th>{{availableStockDetail.WH_AvailableStockQuantity}}</th>
            <th>{{availableStockDetail.BJ_AvailableStockQuantity}}</th>
            <th>{{availableStockDetail.GZ_AvailableStockQuantity}}</th>
          </tr>
        </tbody>
      </table>
    </Modal>
    <Modal class='modal-table' v-model="isShowLayer_ZaituStock" title="在途库存详情"  width="80%">
      <table style="width: 100%;">
        <thead>
          <tr>
            <th style="width: 250px;">义乌仓在途库存</th>
            <th style="width: 250px;">上海仓在途库存</th>
            <th style="width: 250px;">武汉仓在途库存</th>
            <th style="width: 250px;">北京仓在途库存</th>
            <th style="width: 250px;">广州仓在途库存</th>
          </tr>
        </thead>
         <tbody>
          <tr> 
            <th>{{zaituStockDetail.YW_ZaituStockQuantity}}</th>
            <th>{{zaituStockDetail.SH_ZaituStockQuantity}}</th>
            <th>{{zaituStockDetail.WH_ZaituStockQuantity}}</th>
            <th>{{zaituStockDetail.BJ_ZaituStockQuantity}}</th>
            <th>{{zaituStockDetail.GZ_ZaituStockQuantity}}</th>
          </tr>
        </tbody>
      </table>
    </Modal>
  </div>
</template>
<script>
import util from "@/framework/libs/util";
let data = {
  OpenModal: false,
  filtedList: [],
  categoryData: [],
  merchandiseList: [],
  tbLoading: false,
  totalRecord: 0,
  isShowLayer_CouponsQuery: false,
  isShowLayer_Coupons: false,
  isShowLayer_AvailableStock: false,
  isShowLayer_ZaituStock: false,
  couponsDetail: [],
  row: {},
  availableStockDetail: {},
  zaituStockDetail: {},
  inputProp: {
    pageSize: 10,
    curPage: 1,
    keyWordSearchType: "2",
    keyWord: "",
    categorys: [],
    categoryId: 0,
    purchaseMinPrice: 0,
    purchaseMaxPrice: 0,
    onSale: "-1",
    isDaifa: "-1",
    isOutOfStock: "-1",
    isHavePintuanPrice: "-1",
    merchandiseBrand: "",
    coupons: [],
    couponsString: ""
  },
  keyWordSearchTypeSelect: [
    {
      label: "全部",
      value: "-1"
    },
    {
      label: "商品PID",
      value: "2"
    },
    {
      label: "商品名称",
      value: "1"
    }
  ],
  isDaifaSelect: [
    {
      label: "全部",
      value: "-1"
    },
    {
      label: "非代发",
      value: "0"
    },
    {
      label: "代发",
      value: "1"
    }
  ],
  upStateSelect: [
    {
      label: "全部",
      value: "-1"
    },
    {
      label: "下架",
      value: "0"
    },
    {
      label: "上架",
      value: "1"
    }
  ],
  isOutOfStockSelect: [
    {
      label: "全部",
      value: "-1"
    },
    {
      label: "不缺货",
      value: "0"
    },
    {
      label: "缺货",
      value: "1"
    }
  ],
  isHavePintuanPriceSelect: [
    {
      label: "全部",
      value: "-1"
    },
    {
      label: "无",
      value: "0"
    },
    {
      label: "有",
      value: "1"
    }
  ],
  columns: [
    {
      key: "PID",
      title: "PID",
      align: "center",
      width: 150
    },
    {
      key: "ProductName",
      title: "商品名称",
      align: "center",
      width: 380,
      render: (h, params) => {
        return h(
          "div",
          {
            style: {
              margin: "10px 0px"
            }
          },
          params.row.ProductName
        );
      }
    },
    {
      key: "DaydaySeckillPrice",
      title: "天天秒杀价",
      align: "center",
      width: 100,
      render: (h, params) => {
        if (params.row.DaydaySeckillPrice > 0) {
          return h("div", [
            h("div", params.row.DaydaySeckillPrice),
            h(
              "a",
              {
                props: {
                  type: "text",
                  size: "small"
                },
                on: {
                  click: () => {
                    data.OpenModal = true;
                    data.hTbData = params.row.DaydaySeckillPriceList;
                  }
                }
              },
              "更多"
            )
          ]);
        } else {
          return h("span", 0);
        }
      }
    },
    {
      key: "PintuanPrice",
      title: "拼团价",
      align: "center",
      width: 100,
      render: (h, params) => {
        if (params.row.PintuanPrice > 0) {
          return h("div", [
            h("div", params.row.PintuanPrice),
            h(
              "a",
              {
                props: {
                  type: "text",
                  size: "small"
                },
                style: {
                  display: "inline-block"
                },
                on: {
                  click: () => {
                    data.OpenModal = true;
                    data.hTbData = params.row.PintuanPriceList;
                  }
                }
              },
              "更多"
            )
          ]);
        } else {
          return h("span", 0);
        }
      }
    },
    {
      key: "FullNetActivityPrice",
      title: "全网活动价",
      align: "center",
      width: 100,
      render: (h, params) => {
        if (params.row.FullNetActivityPrice > 0) {
          return h("div", [
            h("div", params.row.FullNetActivityPrice),
            h(
              "a",
              {
                props: {
                  type: "text",
                  size: "small"
                },
                on: {
                  click: () => {
                    data.OpenModal = true;
                    data.hTbData = params.row.FullNetActivityPriceList;
                  }
                }
              },
              "更多"
            )
          ]);
        } else {
          return h("span", 0);
        }
      }
    },
    //  {
    //   key: "FlashSalePrice",
    //   title: "限时抢购价",
    //   width: 100,
    //   render: (h, params) => {
    //     if (params.row.FlashSalePrice > 0) {
    //       return h("div", [
    //         h("div", params.row.FlashSalePrice),
    //         h(
    //           "a",
    //           {
    //             props: {
    //               type: "text",
    //               size: "small"
    //             },
    //             on: {
    //               click: () => {
    //                 data.OpenModal = true;
    //                 data.hTbData = params.row.FlashSalePriceList;
    //               }
    //             }
    //           },
    //           "更多"
    //         )
    //       ]);
    //     } else {
    //       return h("span", 0);
    //     }
    //   }
    // },
    {
      key: "UsedCouponPrice",
      title: "劵后价格",
      align: "center",
      width: 120,
      render: (h, params) => {
        if (params.row.Coupons && params.row.Coupons.length > 0) {
          return h("div", [
            h("div", params.row.UsedCouponPrice),
            h(
              "a",
              {
                props: {
                  type: "text",
                  size: "small"
                },
                style: {
                  display: "inline-block"
                },
                on: {
                  click: () => {
                    debugger;
                    data.isShowLayer_Coupons = true;
                    data.couponsDetail = params.row.Coupons;
                    data.row = params.row
                  }
                }
              },
              "更多"
            )
          ]);
        } else {
          return h("span", 0);
        }
      }
    },
    {
      key: "UsedCouponProfit",
      title: "劵后毛利",
      align: "center",
      width: 120,
      render: (h, params) => {
        if (params.row.Coupons && params.row.Coupons.length > 0) {
          return h("div", [
            h("div", params.row.UsedCouponProfit),
            h(
              "a",
              {
                props: {
                  type: "text",
                  size: "small"
                },
                style: {
                  display: "inline-block"
                },
                on: {
                  click: () => {
                    debugger;
                    data.isShowLayer_Coupons = true;
                    data.couponsDetail = params.row.Coupons;
                    data.row = params.row
                  }
                }
              },
              "更多"
            )
          ]);
        } else {
          return h("span", 0);
        }
      }
    },
    {
      key: "OriginalPrice",
      title: "官网原价",
      align: "center",
      width: 100
    },
    {
      key: "PurchasePrice",
      title: "采购价格",
      align: "center",
      width: 100
    },
    {
      key: "ContractPrice",
      title: "代发价格",
      align: "center",
      width: 100
    },
    // {
    //   key: "OfferPurchasePrice",
    //   title: "采购优惠价",
    //   width: 100
    // },
    // {
    //   key: "OfferContractPrice",
    //   title: "代发优惠价",
    //   width: 100
    // },
    {
      key: "IsDaifa",
      title: "是否代发",
      align: "center",
      width: 100,
      render: (h, params) => {
        if (params.row.IsDaifa) {
          return h("div", "是");
        } else {
          return h("div", "否");
        }
      }
    },
    {
      key: "OnSale",
      title: "上下架",
      align: "center",
      width: 80,
      render: (h, params) => {
        if (params.row.OnSale) {
          return h("div", "是");
        } else {
          return h("div", "否");
        }
      }
    },
    {
      key: "StockOut",
      title: "是否缺货",
      align: "center",
      width: 100,
      render: (h, params) => {
        if (params.row.StockOut) {
          return h("div", "是");
        } else {
          return h("div", "否");
        }
      }
    },
    {
      key: "TotalAvailableStockQuantity",
      title: "全部可用库存",
      align: "center",
      width: 126,
      render: (h, params) => {
        if (params.row.TotalAvailableStockQuantity) {
          return h("div", [
            h("div", params.row.TotalAvailableStockQuantity),
            h(
              "a",
              {
                props: {
                  type: "text",
                  size: "small"
                },
                style: {
                  display: "inline-block"
                },
                on: {
                  click: () => {
                    debugger;
                    data.isShowLayer_AvailableStock = true;
                    data.availableStockDetail = params.row.AvailableStock;
                  }
                }
              },
              "更多"
            )
          ]);
        } else {
          return h("span", 0);
        }
      }
    },
    {
      key: "TotalZaituStockQuantity",
      title: "全部在途库存",
      align: "center",
      width: 126,
      render: (h, params) => {
        if (params.row.TotalZaituStockQuantity) {
          return h("div", [
            h("div", params.row.TotalZaituStockQuantity),
            h(
              "a",
              {
                props: {
                  type: "text",
                  size: "small"
                },
                style: {
                  display: "inline-block"
                },
                on: {
                  click: () => {
                    debugger;
                    data.isShowLayer_ZaituStock = true;
                    data.zaituStockDetail = params.row.ZaituStock;
                  }
                }
              },
              "更多"
            )
          ]);
        } else {
          return h("span", 0);
        }
      }
    }
    // {
    //   key: "YW_AvailableStockQuantity",
    //   title: "义乌仓可用库存",
    //   width: 126
    // },
    // {
    //   key: "YW_ZaituStockQuantity",
    //   title: "义乌仓在途库存",
    //   width: 126
    // },
    // {
    //   key: "SH_AvailableStockQuantity",
    //   title: "上海仓可用库存",
    //   width: 126
    // },
    // {
    //   key: "SH_ZaituStockQuantity",
    //   title: "上海仓在途库存",
    //   width: 126
    // },
    // {
    //   key: "WH_AvailableStockQuantity",
    //   title: "武汉仓可用库存",
    //   width: 126
    // },
    // {
    //   key: "WH_ZaituStockQuantity",
    //   title: "武汉仓在途库存",
    //   width: 126
    // },
    // {
    //   key: "BJ_AvailableStockQuantity",
    //   title: "北京仓可用库存",
    //   width: 126
    // },
    // {
    //   key: "BJ_ZaituStockQuantity",
    //   title: "北京仓在途库存",
    //   width: 126
    // },
    // {
    //   key: "GZ_AvailableStockQuantity",
    //   title: "广州仓可用库存",
    //   width: 126
    // },
    // {
    //   key: "GZ_ZaituStockQuantity",
    //   title: "广州仓在途库存",
    //   width: 126
    // }
  ],
  hcolumns: [
    {
      key: "Price",
      title: "价格",
      align: "center",
      width: 150
    },
    {
      key: "BeginTime",
      title: "开始时间",
      align: "center",
      width: 260
    },
    {
      key: "EndTime",
      title: "结束时间",
      align: "center",
      width: 260
    },
    {
      key: "ActivityId",
      align: "center",
      title: "活动ID"
    }
  ],
  hTbData: []
};
export default {
  data () {
    return data;
  },
  mounted () {
    this.initCategorys();
    this.loadData();
  },
  methods: {
    ExportExcel () {
      var myhref =
        "/ProductPrice/ExportExcel4CarProductMutliPrice?" +
        "categoryId=" +
        data.inputProp.categoryId +
        "&merchandiseBrand=" +
        data.inputProp.merchandiseBrand +
        "&onSale=" +
        data.inputProp.onSale +
        "&isDaifa=" +
        data.inputProp.isDaifa +
        "&isOutOfStock=" +
        data.inputProp.isOutOfStock +
        "&isHavePintuanPrice=" +
        data.inputProp.isHavePintuanPrice +
        "&keyWord=" +
        data.inputProp.keyWord +
        "&keyWordSearchType=" +
        data.inputProp.keyWordSearchType +
        "&couponsString=" + JSON.stringify(this.inputProp.coupons);
      window.location.href = myhref;
    },
    ResetInputProp () {
      data.inputProp.onSale = "-1";
      data.inputProp.keyWord = '';
      data.inputProp.isDaifa = "-1";
      data.inputProp.isOutOfStock = "-1";
      data.inputProp.isHavePintuanPrice = "-1";
      data.inputProp.keyWordSearchType = "2";
      data.inputProp.merchandiseBrand = "";
      data.inputProp.categorys = [];
      data.inputProp.categoryId = 0;
    },
    initCategorys () {
      util.ajax.post("/ProductPrice/GetProductCatalog", {}).then(function (response) {
        if (response.data.Success) {
          data.categoryData = response.data.Data;
        } else {
          data.categoryData = [];
          this.$Message.warning("商品类目初始化失败");
        }
      });
    },
    GetMerchandiseBrand (value, selectedData) {
      this.inputProp.merchandiseBrand = "";
      if (value.length > 0) {
        this.inputProp.categoryId = parseInt(value[value.length - 1]);
      } else {
        this.inputProp.categoryId = 0;
        this.$Message.warning("商品类目未初始化，请再次点击");
        return;
      }
      
      var that = this;
      util.ajax
        .post("/ProductPrice/GetMerchandiseBrands", {
          CategoryID: this.inputProp.categoryId
        })
        .then(function (response) {
          if (response.data.Result) {
            that.merchandiseList = response.data.Data;
          } else {
            that.merchandiseList = [];
            that.$Message.warning(response.data.errMsg);
          }
        });
    },
    changePage (page) {
      this.loadData(page);
    },
    loadData (curPage) {
      var that = this;
      if (curPage) {
        this.inputProp.curPage = curPage;
      }
      this.inputProp.couponsString = JSON.stringify(this.inputProp.coupons);
      this.tbLoading = true;
      util.ajax
        .post("/ProductPrice/GetCarProductMutliPriceByCatalog", this.inputProp)
        .then(function (response) {
          that.tbLoading = false;
          if (response.data) {
            that.filtedList = response.data.Item1;
            that.totalRecord = response.data.Item2;
          } else {
            that.filtedList = [];
          }
        });
    },
    showLayer_Coupons () {
      // this.$Message.warning('打开选择优惠券层')
      this.isShowLayer_Coupons = true;
    },
    showLayer_CouponsQuery () {
      this.isShowLayer_CouponsQuery = true;
    },
    AddCoupon (couponRulePKID) {
      var that = this;
      that.inputProp.coupons.push({
        GetRuleGUID: "",
        RuleID: "",
        Name: "",
        Description: "",
        Minmoney: "",
        Discount: "",
        CouponStartTime: "",
        CouponEndTime: "",
        CouponDuration: "",
        Quantity: "",
        PKID: ""
      });
    },
    FetchCouponInfo (couponRulePKID, index) {
      var that = this;
      if (couponRulePKID) {
        util.ajax
          .post("/ProductPrice/CouponVlidate", {
            couponRulePKID: couponRulePKID
          })
          .then(function (response) {
            if (response.data.Result) {
              that.inputProp.coupons[index].GetRuleGUID =
                response.data.Data.GetRuleGUID;
              that.inputProp.coupons[index].Description =
                response.data.Data.Description;
              that.inputProp.coupons[index].Discount =
                response.data.Data.Discount;
              that.inputProp.coupons[index].Minmoney =
                response.data.Data.Minmoney;
              that.inputProp.coupons[index].CouponStartTime =
                response.data.Data.CouponStartTime;
              that.inputProp.coupons[index].CouponDuration =
                response.data.Data.CouponDuration;
              that.inputProp.coupons[index].RuleID = response.data.Data.RuleID;
              that.inputProp.coupons[index].PKID = response.data.Data.PKID;
              that.$Message.warning("添加优惠券成功");
            } else {
              that.$Message.warning("添加优惠券失败");
              // that.inputProp.coupons = []
            }
          });
      }
    },
    DeleteCoupon (index) {
      this.inputProp.coupons.splice(index, 1);
    },
    NumberFormate (value) {
      value = Number(value);
      return value.toFixed(2);
    }
  },
  filters: {
    KeepToNum: function (value) {
      value = Number(value);
      return value.toFixed(2);
    }
  }
};
</script>

<style lang="less" scoped>
.ivu-form {
  .ivu-select-single {
    width: 200px;
  }
}

.modal-table {
  table {
    border: 1px solid #e9eaec;
    border-collapse: collapse;
    background-color:  #e9eaec;
    th {
      border: 1px solid #e9eaec;
      padding: 10px 20px;
      min-width: 10px;
      color:#495060;
    }
    thead th {
      background-color: #f8f8f9;
    }
    tbody th {
      background-color: white;
    }
  }
}
</style>
