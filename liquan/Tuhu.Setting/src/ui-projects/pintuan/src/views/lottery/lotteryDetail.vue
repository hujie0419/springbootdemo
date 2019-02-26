<template>
  <div>
    <div style="margin-bottom:20px;">
      <h2>{{ $route.params.id }} 拼团抽奖拼团成功订单</h2>
    </div>
    <div class="page-form-header">
      <Form :model="form" style="width:500px;" label-position="left" inline>
          <FormItem label="订单号" :label-width="50">
              <Input type="text" v-model.trim.number="form.orderId" placeholder="请输入订单号">
              </Input>
          </FormItem>
          <FormItem label="获奖状态" :label-width="60">
            <Select transfer v-model.number="form.lotteryLevel" style="width:100px">
                <Option v-for="item in dropdownList" :value="item.value" :key="item.value">{{ item.text }}</Option>
            </Select>
          </FormItem>
          <FormItem>
              <Button type="success" @click="loadData()">搜索</Button>
          </FormItem>
      </Form>
      <div style="display:inline-block;">
        <Button v-if="countdown > 0" type="info" @click="sendSms()" disabled>发送短信({{ countdown }}s)</Button>
        <Button v-else type="info" @click="sendSms()">发送短信</Button>
        <Button type="info" @click="refund()">给用户退款</Button>
        <Button type="info" @click="issueCoupon()">给用户塞券</Button>
        <Button type="info" @click="viewLog()">查看日志</Button>
      </div>
    </div>
    <div class="page-main">
      <Table border :columns="table.cols" :data="filtedList" no-data-text="暂无数据"></Table>
      <div style="margin: 10px;overflow: hidden">
          <div style="float: right;">
              <Page :total="table.totalSize" :current="1" @on-change="loadData" show-total></Page>
          </div>
      </div>
    </div>

    <Modal
        v-model="hiddenModal.showPushDialog"
        title="发送短信"
        @on-ok="pushDialog.sendSms" :closable="false" :mask-closable="false">
        <table class="popup">
          <tr>
            <th>ProductGroupId</th>
            <td><label style="font-weight:bold">{{groupId}}</label></td>
          </tr>
          <tr>
            <th>用户范围</th>
            <td>
              <Select transfer v-model="pushDialog.pushUserScope" @on-change="pushDialog.onUserScopeChange" style="width: 300px">
                  <Option value="-1">全部用户</Option>
                  <Option value="-99">单个用户</Option>
              </Select>
            </td>
          </tr>
          <tr v-if="pushDialog.userCount == 1">
            <th>订单编号</th>
            <td>
              <Input v-model.trim.number="pushDialog.pushUserOrderId" placeholder="订单号" style="width: 300px"></Input>
            </td>
          </tr>
          <tr>
            <th>短信预览</th>
            <td>
              <span>尊敬的途虎用户，您参加的{{productName}}抽奖活动开奖啦！关注“途虎一元团”公众号，回复“获奖名单”获取中奖名单，一等奖用户客服会尽快与其联系，二等奖用户更能获得百元红包大礼。</span>
            </td>
          </tr>
        </table>
    </Modal>
    <Modal
        v-model="hiddenModal.showRefundDialog"
        title="用户退款"
        @on-ok="refundDialog.refund" :closable="false" :mask-closable="false">
        <table class="popup">
          <tr>
            <th>ProductGroupId</th>
            <td><label style="font-weight:bold">{{groupId}}</label></td>
          </tr>
          <tr>
            <th>订单状态</th>
            <td>
              <Select transfer v-model="refundDialog.orderStatus">
                  <Option value="1" selected="selected">取消订单并退款</Option>
              </Select>
            </td>
          </tr>
          <tr>
            <th>用户范围</th>
            <td>
              <span v-if="refundDialog.userCount > 0">所选用户数量：{{refundDialog.userCount}}</span>
              <Select transfer v-model="refundDialog.userScope" @on-change="refundDialog.onUserScopeChange">
                  <Option value="2">二等奖用户</Option>
                  <Option value="-99">单个用户</Option>
              </Select>
            </td>
          </tr>
          <tr>
            <th>订单Id</th>
            <td>
              <Input v-model.trim.number="refundDialog.refundOrderId" placeholder="订单号" style="width: 300px"></Input>
            </td>
          </tr>
        </table>
    </Modal>
    <Modal
        v-model="hiddenModal.showCouponDialog"
        title="给用户塞券"
        @on-ok="couponDialog.issueCoupon" width="720" :closable="false" :mask-closable="false">
        <table class="popup">
          <tr>
            <th>ProductGroupId</th>
            <td><label style="font-weight:bold">{{groupId}}</label></td>
          </tr>
          <tr>
            <th>用户范围</th>
            <td>
              <span v-if="couponDialog.userCount > 0">所选用户数量：{{couponDialog.userCount}}</span>
              <Select transfer v-model="couponDialog.userScope" @on-change="couponDialog.onUserScopeChange">
                  <Option value="1">一等奖用户</Option>
                  <Option value="2">二等奖用户</Option>
                  <Option value="-1">全部用户</Option>
                  <Option value="-99">单个用户</Option>
              </Select>
            </td>
          </tr>
          <tr>
            <th>订单Id</th>
            <td>
              <Input v-model.trim.number="couponDialog.couponOrderId" placeholder="订单号" style="width: 300px"></Input>
            </td>
          </tr>
        </table>
        <div id="couponList">
          <header style="background-color: #ccc;font-size: 1.2rem;line-height: 2rem;margin-top: 0.2em;">
            请选择优惠券 &nbsp;
            <Button type="info" @click="couponDialog.onAddCoupon()">新增优惠券</Button>
          </header>
          <Table ref="selection" border :columns="couponDialog.cols" :data="couponDialog.list" no-data-text="暂无优惠券"></Table>
        </div>
    </Modal>
    <Modal
        v-model="hiddenModal.showLogDialog"
        title="查看日志" width="800">
      <div>
          <label>日志类型</label>
          <Select transfer v-model="logDialog.logType" @on-change="logDialog.onLogTypeChange()">
            <Option value="PushMessage">推送日志</Option>
            <Option value="CancelOrder">订单操作日志</Option>
            <Option value="SendCoupon">塞券日志</Option>
            <Option value="SetLevel->1">一等奖设置日志</Option>
          </Select> <br/><br/>
          <Table ref="selection" width="740" border :columns="logDialog.cols" :data="logDialog.logList" no-data-text="暂无日志"></Table>
      </div>
      <div slot="footer"></div>
    </Modal>

    <Modal v-model="hiddenModal.showAddCouponDialog"
        title="添加优惠券"
        @on-ok="couponDialog.onAdddCouponSuccess" :closable="false" :mask-closable="false">
         <table class="popup">
          <tr>
            <th>ProductGroupId</th>
            <td><label style="font-weight:bold">{{groupId}}</label></td>
          </tr>
          <tr>
            <th>优惠券GUID</th>
            <td>
              <Input v-model="couponDialog.newCoupon.CouponId" @on-blur="couponDialog.fetchCouponInfo()"></Input>
            </td>
          </tr>
          <tr>
            <th>优惠券说明</th>
            <td>
              <Input v-model="couponDialog.newCoupon.CouponDesc" :readonly="true"></Input>
            </td>
          </tr>
          <tr>
            <th>使用条件</th>
            <td>
              <Input v-model="couponDialog.newCoupon.CouponCondition" :readonly="true"></Input>
            </td>
          </tr>
          <tr>
            <th>有效期</th>
            <td>
              <Input v-model="couponDialog.newCoupon.UsefulLife" :readonly="true"></Input>
            </td>
          </tr>
         </table>
    </Modal>
  </div>
</template>
<style lang="less">
form {
  display: inline;
}
table.popup {
  th {
    text-align: right;
    padding-right: 10px;
  }
  td{
    text-align: left;
  }
}

label {
  font-weight: bold;
  font-size: 1.1rem;
  line-height: 1.4rem;
}
</style>

<script>
import util from "@/framework/libs/util";
var viewModel = {
  groupId: "",
  timer: null,
  countdown: 0,
  productName: "",
  hiddenModal: {
    showPushDialog: false,
    showRefundDialog: false,
    showCouponDialog: false,
    showAddCouponDialog: false,
    showLogDialog: false
  },
  pushDialog: {
    pushUserScope: "-1",
    pushUserOrderId: "",
    userCount: -1,
    onUserScopeChange: function () {
      if (viewModel.pushDialog.pushUserScope === "-99") {
        viewModel.pushDialog.userCount = 1;
        return;
      }
      util.ajax.post(`/GroupBuyingV2/GetLotteryUserCount?productGroupId=${viewModel.groupId}&tag=${viewModel.pushDialog.pushUserScope}`)
      .then(function (response) {
        if (response.data) {
          if (response.data.Code === 1) {
            viewModel.pushDialog.userCount = response.data.Info;
          } else {
            util.message.error({
              top: 50,
              duration: 3,
              content: "获取用户数量失败"
            });
            viewModel.pushDialog.userCount = -1;
          }
        }
      });
    },
    sendLotterySms: function () {
      util.ajax.post("/GroupBuyingV2/SendLotterySms", null, {
        headers: {
          'Content-Type': 'application/x-www-form-urlencoded'
        },
        params: {
          productGroupId: viewModel.groupId,
          tag: viewModel.pushDialog.pushUserScope,
          orderId: viewModel.pushDialog.pushUserOrderId
        }
      })
      .then(function (response) {
        if (response.data.Code === 1) {
          util.message.success({
              top: 50,
              duration: 3,
              content: "操作成功"
            });

          // 禁用“发送短信”按钮60s
          viewModel.countdown = 60;
          viewModel.timer = setInterval(() => {
            if (viewModel.countdown > 0) {
              viewModel.countdown--;
            } else {
              clearInterval(viewModel.timer);
              viewModel.timer = null;
            }
          }, 1000)
        } else {
          util.message.error({
              top: 50,
              duration: 3,
              content: response.data.Info
            });
        }
      });
    },
    sendSms: function () {
      if (viewModel.pushDialog.pushUserScope === "-99" && !viewModel.pushDialog.pushUserOrderId) {
        util.modal.error({
          title: "错误",
          content: "订单号不能为空"
        });
        return;
      }

      if (viewModel.pushDialog.pushUserScope === "-1") {
        util.modal.confirm({
          title: "提示",
          content: `确认给全部一等奖和二等奖用户发送短信？`,
          onOk: () => {
            viewModel.pushDialog.sendLotterySms();
          }
        });
      } else {
        viewModel.pushDialog.sendLotterySms();
      }
    }
  },
  refundDialog: {
    userScope: "-99",
    orderStatus: "1",
    refundOrderId: "",
    userCount: 1,
    onUserScopeChange: function () {
      if (viewModel.refundDialog.userScope === "-99") {
        viewModel.refundDialog.userCount = 1;
        return;
      }
      util.ajax.post(`/GroupBuyingV2/GetLotteryUserCount?productGroupId=${viewModel.groupId}&tag=${viewModel.refundDialog.userScope}`)
      .then(function (response) {
        if (response.data) {
          if (response.data.Code === 1) {
            viewModel.refundDialog.userCount = response.data.Info;
          } else {
            util.message.error({
              top: 50,
              duration: 3,
              content: "获取用户数量失败"
            });
            viewModel.refundDialog.userCount = -1;
          }
        }
      });
    },
    refund: function () {
      if (viewModel.refundDialog.userScope === "-99" && !viewModel.refundDialog.refundOrderId) {
        util.modal.error({
          title: "错误",
          content: "订单号不能为空"
        });
        return;
      }
      util.modal.confirm({
        title: "提示",
        content: `确定要给${(viewModel.refundDialog.userScope === "2" ? "二等奖" : "")}用户取消订单并退款吗？`,
        onOk: () => {
            util.ajax.post("/GroupBuyingV2/CancelLotteryOrder", null, {
            headers: {
              'Content-Type': 'application/x-www-form-urlencoded'
            },
            params: {
              productGroupId: viewModel.groupId,
              tag: viewModel.refundDialog.userScope,
              orderId: viewModel.refundDialog.refundOrderId
            }
          })
          .then(function (response) {
            if (response.data.Code === 1) {
              util.message.success({
                  top: 50,
                  duration: 3,
                  content: "操作成功"
                });
            } else {
              util.message.error({
                  top: 50,
                  duration: 3,
                  content: response.data.Info
                });
            }
          });
        }
      });
    }
  },
  couponDialog: {
    userScope: "-1",
    couponOrderId: "",
    userCount: -1,
    newCoupon: {
      CouponId: "",
      CouponDesc: "",
      CouponCondition: "",
      UsefulLife: ""
    },
    fetchCouponInfo: function () {
      if (viewModel.couponDialog.newCoupon.CouponId) {
         util.ajax.get(`/GroupBuyingV2/GetCouponDetail?couponId=${viewModel.couponDialog.newCoupon.CouponId}`)
          .then(function (response) {
            if (response.data.Code === 1) {
              viewModel.couponDialog.newCoupon.CouponDesc = response.data.CouponDesc;
              viewModel.couponDialog.newCoupon.CouponCondition = response.data.CouponCondition;
              viewModel.couponDialog.newCoupon.UsefulLife = response.data.UsefulLife;
            }
          });
      }
    },
    onUserScopeChange: function () {
      if (viewModel.couponDialog.userScope === "-99") {
        viewModel.couponDialog.userCount = 1;
        return;
      }
      util.ajax.post(`/GroupBuyingV2/GetLotteryUserCount?productGroupId=${viewModel.groupId}&tag=${viewModel.couponDialog.userScope}`)
      .then(function (response) {
        if (response.data) {
          if (response.data.Code === 1) {
            viewModel.couponDialog.userCount = response.data.Info;
          } else {
            util.message.error({
              top: 50,
              duration: 3,
              content: "获取用户数量失败"
            });
            viewModel.couponDialog.userCount = -1;
          }
        }
      });
    },
    cols: [
      {
        type: 'selection'
      },
      {
        title: "优惠券GUID",
        key: "CouponId"
      },
      {
        title: "优惠券说明",
        key: "CouponDesc"
      },
      {
        title: "使用条件",
        key: "CouponCondition"
      },
      {
        title: "有效期",
        key: "UsefulLife"
      },
      {
        title: "创建人",
        key: "Creator"
      },
      {
        title: "操作",
        key: "action",
        render: function (h, params) {
          return h("Button", {
            on: {
              click: function () {
                util.modal.confirm({
                  title: "警告",
                  content: `确定要删除优惠券 ${params.row.CouponId} 吗？`,
                  onOk: function () {
                      var parameter = new URLSearchParams();
                      parameter.append('productGroupId', viewModel.groupId);
                      parameter.append('couponIds', params.row.CouponId);
                      util.ajax.post("/GroupBuyingV2/DeleteLotteryCoupons", parameter, {
                        headers: {
                          'Content-Type': 'application/x-www-form-urlencoded'
                        }
                      })
                      .then(function (response) {
                        if (response.data.Code === 1) {
                          viewModel.couponDialog.loadCoupon();
                          util.message.success({
                              top: 50,
                              duration: 3,
                              content: "操作成功"
                            });
                        } else {
                          util.message.error({
                              top: 50,
                              duration: 3,
                              content: response.data.Info
                            });
                        }
                      });
                  }
                });
              }
            },
            props: {
              type: "error"
            }
          }, "删除")
        }
      }
    ],
    list: [],
    loadCoupon: function () {
      util.ajax.get(`/GroupBuyingV2/GetLotteryCouponList?productGroupId=${viewModel.groupId}`)
      .then(function (response) {
        if (response.data) {
          if (response.data.Code === 1) {
            viewModel.couponDialog.list = response.data.Data;
          }
        }
      });
    },
    onAdddCouponSuccess: function () {
      let isExist = false;
      for (var i = 0; i < viewModel.couponDialog.list.length; i++) {
        if (viewModel.couponDialog.list[i].CouponId === viewModel.couponDialog.newCoupon.CouponId) {
          isExist = true;
          break;
        }
      }
      if (isExist) {
        util.message.error({
            top: 50,
            duration: 3,
            content: "优惠券已存在"
          });
        return;
      }
      util.modal.confirm({
        title: "提示",
        content: `确定要添加这张优惠券吗？`,
        onOk: function () {
            var params = new URLSearchParams();
            params.append('productGroupId', viewModel.groupId);
            params.append('couponId', viewModel.couponDialog.newCoupon.CouponId);
            util.ajax.post("/GroupBuyingV2/AddLotteryCoupon", params, {
              headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
              }
            })
            .then(function (response) {
              if (response.data.Code === 1) {
                viewModel.couponDialog.loadCoupon();
                util.message.success({
                    top: 50,
                    duration: 3,
                    content: "操作成功"
                  });
              } else {
                util.message.error({
                    top: 50,
                    duration: 3,
                    content: response.data.Info
                  });
              }
            });
        }
      });
    },
    onAddCoupon: function () {
      viewModel.hiddenModal.showAddCouponDialog = true;
    },
    issueCoupon: function () {
      var selectionNodes = document.querySelectorAll("#couponList table td .ivu-checkbox-input");
      if (!selectionNodes || selectionNodes.length === 0) {
        util.modal.error({
          title: "错误",
          content: "请添加优惠券"
        });
        return;
      }
      let couponList = [];
      for (var i = 0; i < selectionNodes.length; i++) {
        var node = selectionNodes[i];
        if (node.checked) {
          var couponId = node.parentNode.parentNode.parentNode.parentNode.parentNode.childNodes[1].textContent.trim();
          if (couponId) {
            couponList.push(couponId);
          }
        }
      }
      if (couponList.length === 0) {
        util.modal.error({
          title: "错误",
          content: "请选择优惠券"
        });
        return;
      }

      let postdata = {};
      postdata["productGroupId"] = viewModel.groupId;
      postdata["tag"] = viewModel.couponDialog.userScope;    
      postdata["orderId"] = viewModel.couponDialog.couponOrderId;    
      postdata["couponList"] = couponList;
      let postmessage = JSON.stringify(postdata);

      util.modal.confirm({
        title: "提示",
        content: `确定要给用户塞券吗？`,
        onOk: () => {
            util.ajax.post("/GroupBuyingV2/SendLotteryCoupon", postmessage, {
              headers: {
                'Content-Type': 'application/json'
              }
            })
            .then(function (response) {
              if (response.data.Code === 1) {
                util.message.success({
                    top: 50,
                    duration: 3,
                    content: "操作成功"
                  });
              } else {
                util.message.error({
                    top: 50,
                    duration: 3,
                    content: response.data.Info
                  });
              }
            });
        }
      });
    }
  },
  logDialog: {
    logType: "PushMessage",
    cols: [
      {
        title: "时间",
        key: "CreateDateTime"
      },
      {
        title: "操作类型",
        key: "OperateType"
      },
      {
        title: "操作人",
        key: "Operator"
      },
      {
        title: "备注",
        key: "Remark"
      }
    ],
    logList: [],
    onLogTypeChange: function () {
      util.ajax.get(`/GroupBuyingV2/GetLotteryLog?productGroupId=${viewModel.groupId}&type=${viewModel.logDialog.logType}`)
      .then(function (response) {
        if (response.data) {
          if (response.data.Code === 1) {
            viewModel.logDialog.logList = response.data.Data;
          }
        }
      });
    }
  },
  form: {
    orderId: "",
    lotteryLevel: 0
  },
  dropdownList: [
    {
      value: 0,
      text: "全部"
    },
    {
      value: -1,
      text: "未开奖"
    },
    {
      value: 1,
      text: "一等奖"
    },
    {
      value: 2,
      text: "二等奖"
    }
  ],
  table: {
    loading: true,
    list: [],
    cols: [
      {
        title: "拼团Id",
        key: "ProductGroupId"
      },
      {
        title: "订单号",
        key: "OrderId"
      },
      {
        title: "订单状态",
        key: "OrderStatus"
      },
      {
        title: "账户手机号",
        key: "UserPhone"
      },
      {
        title: "用户Id",
        key: "UserId"
      },
      {
        title: "获奖状态",
        key: "LotteryResult",
        render: function (h, params) {
          switch (params.row.LotteryResult) {
            case 0:
              return h("span", "未开奖");
            case 1:
              return h("span", "一等奖");
            default:
              return h("span", "二等奖");
          }
        }
      },
      {
        title: "操作",
        key: "action",
        width: 320,
        render: (h, params) => {
          let btns = [];
          if (params.row.OrderStatus === "未完成" || params.row.LotteryResult === 0) {
            // 未完成的订单才可以修改
            btns.push(h("Button", {
              props: {
                type: "warning"
              },
              style: {
                margin: "1px 4px"
              },
              on: {
                click: () => {
                  // set lottery result
                  methods.setLotteryResult(params.row.LotteryResult === 1 ? 2 : 1, params.row.OrderId, params.row.UserId);
                }
              }
            }, params.row.LotteryResult === 1 ? "设为二等奖" : "设为一等奖"));
          }
          if (params.row.OrderStatus === "未完成") {
            btns.push(h("Button", {
              props: {
                type: "primary"
              },
              style: {
                margin: "1px 4px"
              },
              on: {
                click: () => {
                  methods.finishOrder(params.row.OrderId, params.row.UserId)
                }
              }
            }, "完成订单"));
            btns.push(h("Button", {
              props: {
                type: "primary"
              },
              style: {
                margin: "1px 4px"
              },
              on: {
                click: () =>
                  // cancel order
                  methods.cancelOrder(params.row.OrderId, params.row.UserId)
              }
            }, "取消订单"));
          }
          return h('div', btns);
        }
      }
    ],
    pageIndex: 0,
    pageSize: 10,
    totalSize: 10
  }
};
var methods = {
    loadData: function (pIndex) {
      if (!pIndex) {
        pIndex = 1;
      }
      util.ajax.get(`/GroupBuyingV2/LotteryDetailList?groupId=${viewModel.groupId}&pageIndex=${pIndex}&pageSize=${viewModel.table.pageSize}&orderId=${viewModel.form.orderId}&lotteryResult=${viewModel.form.lotteryLevel}`)
      .then(function (response) {
        if (response.data) {
          if (response.data.Data) {
            viewModel.table.pageIndex = response.data.PageIndex;
            viewModel.table.totalSize = response.data.TotalSize;
            viewModel.table.list = response.data.Data;
          }
          viewModel.table.loading = false;
        }
      });
    },
    finishOrder: function (orderId, userId) {
      util.modal.confirm({
        title: '确认',
        content: `确定要完成订单 ${orderId} 吗？`,
        onOk: () => {
          util.ajax.post('/GroupBuyingV2/SetOrderStatusForOne', null, {
            headers: {
              'Content-Type': 'application/x-www-form-urlencoded'
            },
            params: {
              productGroupId: viewModel.groupId,
              userId: userId,
              orderId: orderId,
              status: 1
            }
          })
          .then(function (response) {
            if (response.data.Code === 1) {
              util.message.success({
                top: 50,
                duration: 3,
                content: "操作成功"
              });
              methods.loadData();
            } else {
              util.message.error({
                top: 50,
                duration: 3,
                content: response.data.Info
              });
            }
          });
        }
      });
    },
    cancelOrder: function (orderId, userId) {
      util.modal.confirm({
        title: '确认',
        content: `确定要取消订单 ${orderId} 吗？`,
        onOk: () => {
            util.ajax.post('/GroupBuyingV2/SetOrderStatusForOne', null, {
              headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
              },
              params: {
                productGroupId: viewModel.groupId,
                userId: userId,
                orderId: orderId,
                status: -1
              }
            })
            .then(function (response) {
              if (response.data.Code === 1) {
                util.message.success({
                  top: 50,
                  duration: 3,
                  content: "操作成功"
                });
                methods.loadData();
              } else {
                util.message.error({
                  top: 50,
                  duration: 3,
                  content: response.data.Info
                });
              }
            });
        }
      });
    },
    setLotteryResult: function (result, orderId, userId) {
      util.modal.confirm({
        title: '确认',
        content: `确定将用户 ${userId} 设置为 ${(result === 1 ? "一等奖" : "二等奖")} 吗？`,
        onOk: () => {
            util.ajax.post('/GroupBuyingV2/SetLotteryResult', null, {
              headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
              },
              params: {
                productGroupId: viewModel.groupId,
                userId: userId,
                orderId: orderId,
                level: result
              }
            })
            .then(function (response) {
              if (response.data.Code > 0) {
                util.message.success({
                  top: 50,
                  duration: 3,
                  content: "操作成功"
                });
                methods.loadData();
              } else {
                util.message.error({
                  top: 50,
                  duration: 3,
                  content: response.data.Info
                });
              }
            });
        }
      });
    },
    sendSms: function () {
      viewModel.hiddenModal.showPushDialog = true;
    },
    refund: function () {
      viewModel.hiddenModal.showRefundDialog = true;
    },
    issueCoupon: function () {
      viewModel.hiddenModal.showCouponDialog = true;
    },
    viewLog: function () {
      viewModel.hiddenModal.showLogDialog = true;
    }
};
export default {
  name: "Lottery",
  data () {
    return viewModel;
  },
  beforeCreate: function () {
    viewModel.groupId = this.$route.params.id;
    util.ajax.get(`/GroupBuyingV2/LotteryDetailList?groupId=${viewModel.groupId}&pageIndex=1&pageSize=${viewModel.table.pageSize}&orderId=${viewModel.form.orderId}&lotteryResult=${viewModel.form.lotteryLevel}`)
    .then(function (response) {
      if (response.data) {
        if (response.data.Data) {
          viewModel.table.pageIndex = response.data.PageIndex;
          viewModel.table.totalSize = response.data.TotalSize;
          viewModel.table.list = response.data.Data;
        }
        viewModel.table.loading = false;
      }
    });

    // 获取商品名称
    util.ajax.get(`/GroupBuyingV2/GetLotteryProductName?productGroupId=${viewModel.groupId}`)
    .then(function (response) {
      if (response.data) {
        viewModel.productName = response.data;
      }
    });
    viewModel.logDialog.onLogTypeChange();
    viewModel.couponDialog.loadCoupon();
  },
  computed: {
    filtedList: function () {
      if (viewModel.form.orderId || viewModel.form.lotteryLevel || viewModel.form.lotteryLevel !== 0) {
        return viewModel.table.list.filter(function (item) {
          if (viewModel.form.orderId && viewModel.form.lotteryLevel !== 0 && viewModel.form.lotteryLevel !== "") {
            return item.OrderId === viewModel.form.orderId && item.LotteryResult === viewModel.form.lotteryLevel;
          }
          if (viewModel.form.orderId) {
            return item.OrderId === viewModel.form.orderId;
          }
          return item.LotteryResult === viewModel.form.lotteryLevel;
        });
      } else {
        return viewModel.table.list;
      }
    }
  },
  methods: methods
};
</script>
