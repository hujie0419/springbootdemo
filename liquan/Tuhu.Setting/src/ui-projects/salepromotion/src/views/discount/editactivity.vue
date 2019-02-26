<template>
  <div>
    <!-- <h1 class="title">新增促销活动</h1> -->
    <Row>
      <Col span=16 offset=5>
      <Steps :current="0">
        <Step title="促销活动配置" content=""></Step>
        <Step title="选择促销商品" content=""></Step>
        <Step title="审核" content=""></Step>
      </Steps>
      </Col>
    </Row>
    <br>
    <Form ref="activity" :model="activity" :rules="rules" :label-width="100">
      <Row>
        <Col span="9">
        <FormItem label="活动名称" prop="Name">
          <Input v-model="activity.Name" :maxlength=20 :disabled="IsUpdateFlag" placeholder="请输入活动名称,不超过10个汉字"></Input>
        </FormItem>
        </Col>
        <Col span="9">
        <FormItem label="活动描述" prop="Description">
          <Input v-model="activity.Description" :maxlength=40 :disabled="IsUpdateFlag" placeholder="请输入活动描述,不超过20个汉字" style="width:400px;"></Input>
        </FormItem>
        </Col>
      </Row>
      <Row>
        <Col span="9">
        <FormItem label="促销描述" prop="Banner">
          <Input v-model="activity.Banner" :maxlength=30 :disabled="IsUpdateFlag" placeholder="请输入促销描述,不超过15个汉字"></Input>
        </FormItem>
        </Col>
        <Col span="15">
        <Row>
          <FormItem label="活动时间" prop="StartTime">
            <Date-Picker v-model="activity.StartTime" type="datetime" format="yyyy-MM-dd HH:mm:ss" placeholder="请选择起始时间" style="width: 200px"></Date-Picker>
            <Date-Picker v-model="activity.EndTime" type="datetime" format="yyyy-MM-dd HH:mm:ss" placeholder="请选择终止时间" style="width: 200px"></Date-Picker>
          </FormItem>
        </Row>
        </Col>
      </Row>
      <Row>
        <Col span="9">
        <Row>
          <Col span="14">
          <FormItem label="标签名称">
            <div style="float:left;">
              <RadioGroup v-model="activity.Is_DefaultLabel" @on-change="isLableChange">
                <Radio label="1" :disabled="IsUpdateFlag">
                  <span>默认(折)</span>
                </Radio>
                <Radio label="0" :disabled="IsUpdateFlag">
                  <span>自定义</span>
                </Radio>
              </RadioGroup>
            </div>
          </FormItem>
          </Col>
          <Col span="10" style="left:-100px;">
          <div v-if="activity.Is_DefaultLabel==0">
            <FormItem prop="Label">
              <Input v-model="activity.Label" :disabled="IsUpdateFlag" style="width:120px;" placeholder="请输入标签名称"></Input>
            </FormItem>
          </div>
          </Col>
        </Row>
        </Col>
        <Col span="5">
        <FormItem label="会场限购数">
          <div style="float:left;">
            <RadioGroup v-model="activity.Is_PurchaseLimit" @on-change="isLimitChange">
              <Radio label="0" :disabled="IsUpdateFlag">
                <span>不限购</span>
              </Radio>
              <Radio label="1" :disabled="IsUpdateFlag">
                <span>限购</span>
              </Radio>
            </RadioGroup>
          </div>
        </FormItem>
        </Col>
        <Col span="10" style="left:-30px;">
        <div v-if="activity.Is_PurchaseLimit==1" style="float:left;">
          <Input-Number v-model="activity.LimitQuantity" :precision=0 :max="9999999" :min="1" @on-blur="changeLimitQuantity"
           @on-change="changeLimitQuantity"  :disabled="IsUpdateFlag" style="width:130px;margin:0 10px;"
            placeholder="请输入限购数量" />
          <span style="color:#ed3f14;margin:0 1px;">
            {{vueData.limitQuantityTip}}
          </span>
        </div>
        </Col>
      </Row>
      <Row>
        <Col span="9">
        <FormItem label="支付方式">
          <Checkbox :disabled="IsUpdateFlag" v-model="payMethod.shopPay" @on-change="changePayMethod(1)">到店支付</Checkbox>
          <Checkbox v-model="payMethod.onlinePay" :disabled="IsUpdateFlag" @on-change="changePayMethod(2)">在线支付</Checkbox>
          <span style="color:red;"> (非必选,仅轮胎业务线生效)</span>
        </FormItem>
        </Col>
        <Col span="15">
        <FormItem label="安装方式">
          <Checkbox v-model="installMethod.onShop" :disabled="IsUpdateFlag" @on-change="changeinstallMethod(1)">到店安装</Checkbox>
          <Checkbox v-model="installMethod.onHome" :disabled="IsUpdateFlag" @on-change="changeinstallMethod(2)">上门安装</Checkbox>
          <Checkbox v-model="installMethod.noInstall" :disabled="IsUpdateFlag" @on-change="changeinstallMethod(3)">无需安装</Checkbox>
          <span style="color:red;"> (非必选,仅轮胎业务线生效)</span>
        </FormItem>
        </Col>
      </Row>
      <Row>
        <FormItem label="活动类型">
          <RadioGroup v-model="activity.PromotionType">
            <Radio label="1" :disabled="IsUpdateFlag">
              <span>打折</span>
            </Radio>
          </RadioGroup>
        </FormItem>
      </Row>
      <Row>
        <Col>
        <FormItem>
          <div style="margin-left:40px;">
            <Row>
              <RadioGroup v-model="activity.DiscountMethod">
                <Radio label="1" :disabled="IsUpdateFlag">
                  <span>全场满额折</span>
                </Radio>
                <Radio label="2" :disabled="IsUpdateFlag">
                  <span>全场满件折</span>
                </Radio>
              </RadioGroup>
            </Row>
            <div style="color:red;">
              请按照阶梯幅度（x/2x）依次增加优惠力度
            </div>
          </div>
        </FormItem>
        </Col>
      </Row>
      <div v-if="activity.DiscountMethod==1">
        <Row v-if="vueData.AmountDiscountContentList.length==0">
          <Col offset=4>
          <Button type="success" :disabled="IsUpdateFlag" size="small" style="margin-left: 8px;" @click="addAmountDiscountContent(0)">添加</Button>
          </Col>
        </Row>
        <Row v-for="(item,index) in vueData.AmountDiscountContentList" :key="index">
          <Col>
          <FormItem label="满">
            <Input-Number v-model="item.Condition" :max="9999999" :min="1" style="width:120px;margin:0 10px;" 
           @on-blur="changeAmountCondition(index)" :disabled="IsUpdateFlag" placeholder="请输入满额金额" /> 元 享
            <Input-Number v-model="item.DiscountRate" :max="99" :min="1"  :precision=0
             @on-change="amountDiscountChange(index)" @on-blur="amountDiscountChange(index)"
              style="width:100px;margin:0 10px;" :disabled="IsUpdateFlag" placeholder="请输入折扣百分比" /> 折(%)
            <Button v-if="index==0" type="success" :disabled="IsUpdateFlag" size="small" style="margin-left: 8px;" @click="addAmountDiscountContent(index)">继续添加</Button>
            <Button v-if="CanDelAmountDiscountContent" type="warning" :disabled="IsUpdateFlag" size="small" style="margin-left: 8px;"
              @click="delAmountDiscountContent(index)">删除</Button>
            <span style="color:#ed3f14;margin:0 5px;">
              {{item.Tip}}
            </span>
          </FormItem>
          </Col>
        </Row>
      </div>
      <div v-else-if="activity.DiscountMethod==2">
        <Row v-if="vueData.CountDiscountContentList.length==0">
          <Col offset=4>
          <Button type="success" :disabled="IsUpdateFlag" size="small" style="margin-left: 8px;" @click="addCountDiscountContent(0)">添加</Button>
          </Col>
        </Row>
        <Row v-for="(item,index) in vueData.CountDiscountContentList" :key="index">
          <Col>
          <FormItem label="满">
            <Input-Number v-model="item.Condition" :max="9999999" :min="1" style="width:120px;margin:0 10px;"
             @on-change="countConditionValid(index)"  @on-blur="countConditionValid(index)"  :disabled="IsUpdateFlag" placeholder="请输入满减件数" />
              件 享
              <Input-Number v-model="item.DiscountRate" :max="99" :min="1"  :precision=0
             @on-change="countDiscountValid(index)" @on-blur="countDiscountValid(index)"
              style="width:100px;margin:0 10px;" :disabled="IsUpdateFlag" placeholder="请输入折扣百分比" />
            折(%)
            <Button v-if="index==0" type="success" :disabled="IsUpdateFlag" size="small" style="margin-left: 8px;" @click="addCountDiscountContent(index)">继续添加</Button>
            <Button v-if="CanDelCountDiscountContent" :disabled="IsUpdateFlag" type="warning" size="small" style="margin-left: 8px;"
              @click="delCountDiscountContent(index)">删除</Button>
            <span style="color:#ed3f14;margin:0 5px;">
              {{item.Tip}}
            </span>
          </FormItem>
          </Col>
        </Row>
      </div>
      <FormItem>
        <div style="margin-left:40px;">
          <Row v-if="IsUpdateFlag">
            <Col span=4 offset=5>
            <div v-if="vueData.btnDisabled">
              <Button type="success" disabled style="margin-left: 8px;">保存并下一步</Button>
            </div>
            <div v-else-if="vueData.btnDisabled==false">
              <Button type="success" @click="submitactivity" style="width:100px;height:34px;">保存并下一步</Button>
            </div>
            </Col>
            <Col span=6>
            <div v-if="vueData.btnDisabled">
              <Button type="success" disabled style="margin-left: 8px;">提交审核</Button>
            </div>
            <div v-else-if="vueData.btnDisabled==false">
              <Button style="width:100px;height:34px;" type="success" @click="waitAudit">提交审核</Button>
            </div>
            </Col>
          </Row>
          <Row v-else>
            <Col span=6 offset=10>
            <div v-if="vueData.btnDisabled">
              <Button type="success" disabled style="margin-left: 8px;">保存并下一步</Button>
            </div>
            <div v-else-if="vueData.btnDisabled==false">
              <Button type="success" @click="submitactivity" style="margin-left: 8px;">保存并下一步</Button>
            </div>
            </Col>
          </Row>
        </div>
      </FormItem>
    </Form>

    <Modal width="730" title="已存在的商品列表" v-model="repeatProductModal.visible" :loading="repeatProductModal.loading" footerHide>
      <br>
      <Row>
        <span style="font-size:16px;">当前活动时间内,已经存在其他活动中的商品</span>
        <!-- <a style="margin:18px;display:inline;" href="/SalePromotionActivity/ExportTemplate?type=pidtemplate" target="_blank">导出结果</a> -->
      </Row>
      <Table :data="repeatProductModal.data" :columns="repeatProductModal.columns" stripe></Table>
    </Modal>
  </div>

</template>

<script>

 const validNameFormat = (rule, value, callback) => {
    // 验证标签格式
    let len = 0;
    // 检验标签长度
    for (var i = 0; i < value.length; i++) {
      if (value.charCodeAt(i) > 127 || value.charCodeAt(i) === 94) {
        len += 2;
      } else {
        len++;
      }
    }
    if (len > 20) {
      return callback(new Error("请输入不超过 10 个汉字或 20 个字符"));
    } else {
      callback();
    }
  };
   const validDescriptionFormat = (rule, value, callback) => {
    // 验证活动描述格式
    let len = 0;
    for (var i = 0; i < value.length; i++) {
      if (value.charCodeAt(i) > 127 || value.charCodeAt(i) === 94) {
        len += 2;
      } else {
        len++;
      }
    }
    if (len > 40) {
      return callback(new Error("请输入不超过 20 个汉字或 40 个字符"));
    } else {
      callback();
    }
  };
 const validBannerFormat = (rule, value, callback) => {
    // 验证促销描述格式
    let len = 0;
    // 检验标签长度
    for (var i = 0; i < value.length; i++) {
      if (value.charCodeAt(i) > 127 || value.charCodeAt(i) === 94) {
        len += 2;
      } else {
        len++;
      }
    }
    if (len > 30) {
      return callback(new Error("请输入不超过 15 个汉字或 30 个字符"));
    } else {
      callback();
    }
  };
  const validLabelFormat = (rule, value, callback) => {
    // 验证标签格式
    let len = 0;
    // 检验标签长度
    for (var i = 0; i < value.length; i++) {
      if (value.charCodeAt(i) > 127 || value.charCodeAt(i) === 94) {
        len += 2;
      } else {
        len++;
      }
    }
    if (len > 10) {
      return callback(new Error("请输入不超过 5 个汉字或 10 个字符"));
    } else {
      // 检验是否是字符和汉字
      const regex = /^[0-9A-Za-z\u4e00-\u9fa5]{1,10}$/;
      const rsCheck = regex.test(value);
      if (!rsCheck) {
        return callback(new Error("请输入汉字、数字或字母"));
      } else {
        callback();
      }
    }
  };

  // 折扣正则验证
  const validDiscount = function (value) {
    const regex = /^[1-9][0-9]{0,1}$/;
    const rsCheck = regex.test(value);
    if (!rsCheck) {
      return false;
    } else {
      return true;
    }
  };
  export default {
    data () {
      return {
        IsUpdateFlag: false,
        IsToWaitAudit: false,
        payMethod: {
          shopPay: false,
          onlinePay: false
        },
        installMethod: {
          onHome: false,
          onShop: false,
          noInstall: false
        },
        activity: {
          ActivityId: "",
          Name: "",
          Description: "",
          Banner: "",
          StartTime: "",
          EndTime: "",
          Is_DefaultLabel: "1", // 默认标签
          Is_PurchaseLimit: "0", // 默认不限购
          LimitQuantity: null,
          PaymentMethod: "0",
          InstallMethod: "0",
          PromotionType: "1", // 活动类型：默认打折活动
          DiscountMethod: "1",
          Label: ""
        },
        vueData: {
          AmountDiscountContentList: [{
            Condition: null,
            DiscountRate: null,
            Tip: ""
          }],
          CountDiscountContentList: [{
            Condition: null,
            DiscountRate: null,
            Tip: ""
          }],
          btnDisabled: false,
          limitQuantityTip: ""
        },
        rules: {
          Name: [{
              required: true,
              message: "请填写活动名称",
              trigger: "blur"
            },
          {
              validator: validNameFormat,
              trigger: "blur"
            }
          ],
          Description: [{
            required: true,
            message: "请填写活动描述",
            trigger: "blur"
          },
          {
              validator: validDescriptionFormat,
              trigger: "blur"
            }],
          Banner: [{
            required: true,
            message: "请填写促销描述",
            trigger: "blur"
          },
          {
              validator: validBannerFormat,
              trigger: "blur"
            }],
          Label: [{
              required: true,
              message: "请输入自定义标签",
              trigger: "blur"
            },
            {
              validator: validLabelFormat,
              trigger: "blur"
            }
          ]
        },
        repeatProductModal: {
          loading: true,
          visible: false,
          data: [],
          columns: [{
              title: "商品PID",
              key: "Pid",
              align: "center"
            }, {
              title: "商品名称",
              key: "ProductName",
              align: "center"
            },
            {
              title: "存在的活动",
              key: "ActivityName",
              align: "center",
              width: 380
            }
          ]
        }
      };
    },
    mounted () {
      this.IsUpdateFlag = false;
      let activityId = this.$route.query.activityId;
      if (activityId !== undefined && activityId !== "" && activityId.length > 0) {
        this.IsUpdateFlag = true;
        this.ajax
          .post("/SalePromotionActivity/GetActivityModel", {
            activityId: activityId
          })
          .then(response => {
            if (!response.data.Status) {
              this.messageInfo("数据获取失败,请重试");
            } else {
              var data = response.data.Data;
              this.activity = data;
              if (data.AuditDateTime === '' || data.AuditDateTime === undefined || data.AuditDateTime == null) {
                this.IsUpdateFlag = false;
              } else {
                this.IsUpdateFlag = true;
              }
              this.activity.Is_DefaultLabel = data.Is_DefaultLabel.toString();
              this.activity.Is_PurchaseLimit = data.Is_PurchaseLimit.toString();
              this.activity.PromotionType = data.PromotionType.toString();
              if (data.DiscountContentList.length > 0) {
                this.activity.DiscountMethod = data.DiscountContentList[0].DiscountMethod.toString();
              }
              if (this.activity.DiscountMethod === "1") {
                this.vueData.AmountDiscountContentList = this.activity.DiscountContentList;
              } else {
                this.vueData.CountDiscountContentList = this.activity.DiscountContentList;
              }
              // 限购数量
              if (data.LimitQuantity === 0) {
                this.activity.LimitQuantity = null;
              }
              // 支付方式勾选
              if (data.PaymentMethod === 1) {
                this.payMethod.shopPay = true;
              } else if (data.PaymentMethod === 2) {
                this.payMethod.onlinePay = true;
              }
              // 安装方式勾选
              if (data.InstallMethod === 1) {
                this.installMethod.onShop = true;
              } else if (data.InstallMethod === 2) {
                this.installMethod.onHome = true;
              } else if (data.InstallMethod === 3) {
                this.installMethod.noInstall = true;
              }
            }
          });
      }
    },
    computed: {
      CanDelAmountDiscountContent: function () {
        if (
          this.vueData.AmountDiscountContentList == null ||
          this.vueData.AmountDiscountContentList === undefined
        ) {
          return false;
        } else {
          if (this.vueData.AmountDiscountContentList.length > 1) {
            return true;
          } else {
            return false;
          }
        }
      },
      CanDelCountDiscountContent: function () {
        if (this.vueData.CountDiscountContentList.length > 1) {
          return true;
        }
        return false;
      }
    },
    methods: {
      changeLimitQuantity () {
        this.vueData.limitQuantityTip = '';
        var value = this.activity.LimitQuantity;
        if (value != null) {
          this.activity.LimitQuantity = parseInt(value);
        } else {
          this.vueData.limitQuantityTip = "请输入限购数量";
        }
      },
      isLableChange () {
        if (this.activity.Is_DefaultLabel === "1") {
          this.activity.Label = "";
        }
      },
      isLimitChange () {
        if (this.activity.Is_PurchaseLimit === "0") {
          this.activity.LimitQuantity = null;
          this.vueData.limitQuantityTip = '';
        }
      },
      changePayMethod (type) {
        if (type === 1) {
          if (this.payMethod.shopPay) {
            this.payMethod.onlinePay = false;
          }
        } else if (type === 2) {
          if (this.payMethod.onlinePay) {
            this.payMethod.shopPay = false;
          }
        }
      },
      changeinstallMethod (type) {
        if (type === 1) {
          if (this.installMethod.onShop || this.installMethod.onShop === "true") {
            this.installMethod.onHome = false;
            this.installMethod.noInstall = false;
          }
        } else if (type === 2) {
          if (this.installMethod.onHome || this.installMethod.onHome === "true") {
            this.installMethod.onShop = false;
            this.installMethod.noInstall = false;
          }
        } else if (type === 3) {
          if (
            this.installMethod.noInstall ||
            this.installMethod.noInstall === "true"
          ) {
            this.installMethod.onHome = false;
            this.installMethod.onShop = false;
          }
        }
      },
      // 验证满额折金额输入
      changeAmountCondition: function (index) {
        // 金额最多2位小数
        if (this.vueData.AmountDiscountContentList[index].Condition != null) {
          var value = Number(this.vueData.AmountDiscountContentList[index].Condition).toFixed(2);
          this.vueData.AmountDiscountContentList[index].Condition = parseFloat(value);
        }
      },
      // 验证满件折的件数输入
      countConditionValid: function (index) {
        this.vueData.CountDiscountContentList[index].Tip = '';
        let value = this.vueData.CountDiscountContentList[index].Condition;
        if (value == null || value === '') {
          this.vueData.CountDiscountContentList[index].Tip = "请输入满减件数";
        } else {
          this.vueData.CountDiscountContentList[index].Condition = parseInt(value);
        }
      },
      // 验证满额折折扣输入
      amountDiscountChange: function (index) {
        this.vueData.AmountDiscountContentList[index].Tip = '';
        let value = this.vueData.AmountDiscountContentList[index].DiscountRate;
        if (value != null) {
          this.vueData.AmountDiscountContentList[index].DiscountRate = parseInt(value);
        } else {
          this.vueData.AmountDiscountContentList[index].Tip = "请输入折扣百分比";
        }
      },
      // 验证满件折折扣输入
      countDiscountValid: function (index) {
        let value = this.vueData.CountDiscountContentList[index].DiscountRate;
        this.vueData.CountDiscountContentList[index].Tip = "";
        if (value === "" || value == null) {
           this.vueData.CountDiscountContentList[index].Tip = "请输入折扣百分比";
        } else {
           this.vueData.CountDiscountContentList[index].DiscountRate = parseInt(value);
        }
      },
      checkDiscountContentList () { // 提交前验证折扣内容输入是否合法
        var result = true;
        if (this.activity.DiscountMethod === "1") {
          if (!(this.vueData.AmountDiscountContentList.length > 0)) {
            return false;
          }
          for (var i = 0; i < this.vueData.AmountDiscountContentList.length; i++) {
            result = true;
            const countRegex = /^([1-9][0-9]*)(\.[0-9]{0,2})?$|^(0\.[0-9]{0,2})$/;
            const rsCheckCount = countRegex.test(
              this.vueData.AmountDiscountContentList[i].Condition
            );
            if (!rsCheckCount) {
              result = false;
            }
            const rsCheckDiscount = validDiscount(
              this.vueData.AmountDiscountContentList[i].DiscountRate
            );
            if (!rsCheckDiscount) {
              result = false;
            }
          }
        } else {
          if (!(this.vueData.CountDiscountContentList.length > 0)) {
            return false;
          }
          for (var j = 0; j < this.vueData.CountDiscountContentList.length; j++) {
            result = true;
            const countRegex = /^[1-9][0-9]*$/;
            const rsCheckCount = countRegex.test(
              this.vueData.CountDiscountContentList[j].Condition
            );
            if (!rsCheckCount) {
              result = false;
            }
            const rsCheckDiscount = validDiscount(
              this.vueData.CountDiscountContentList[j].DiscountRate
            );
            if (!rsCheckDiscount) {
              result = false;
            }
          }
        }
        return result;
      },
      waitAudit () {
        this.IsToWaitAudit = true;
        this.submitactivity();
      },
      submitactivity: function () { // 保存活动提交
        // 获取支付方式
        if (this.payMethod.shopPay) {
          this.activity.PaymentMethod = 1;
        } else if (this.payMethod.onlinePay) {
          this.activity.PaymentMethod = 2;
        } else if (!this.payMethod.shopPay && !this.payMethod.onlinePay) {
          this.activity.PaymentMethod = 0;
        }
        // 获取安装方式
        if (this.installMethod.onShop) {
          this.activity.InstallMethod = 1;
        } else if (this.installMethod.onHome) {
          this.activity.InstallMethod = 2;
        } else if (this.installMethod.noInstall) {
          this.activity.InstallMethod = 3;
        } else if (!this.installMethod.onShop && !this.installMethod.onHome && !this.installMethod.noInstall) {
          this.activity.InstallMethod = 0;
        }
        // 验证
        this.$refs["activity"].validate(valid => {
          if (valid) {
            if (
              this.activity.StartTime == null ||
              this.activity.StartTime === ''
            ) {
              this.messageInfo("请选择活动开始时间");
              return false;
            }
            if (
              this.activity.EndTime == null ||
              this.activity.EndTime === ''
            ) {
              this.messageInfo("请选择活动终止时间");
              return false;
            }
            if (this.IsUpdateFlag === true) {
              // 已审核后再编辑
              var auditStatus = 0;
              if (this.IsToWaitAudit) {
                auditStatus = 1
              }
              this.vueData.btnDisabled = true;
              this.ajax
                .post("/SalePromotionActivity/UpdateActivityAfterAudit", {
                  model: this.activity,
                  auditStatus: auditStatus
                })
                .then(response => {
                  this.vueData.btnDisabled = false;
                  if (response.data.Status) {
                    if (this.IsToWaitAudit) {
                      this.$router.push({
                        path: "/discount/waitaudit",
                        query: {
                          activityId: this.activity.ActivityId
                        }
                      });
                    } else {
                      this.$router.push({
                        path: "/discount/activityproduct",
                        query: {
                          activityId: this.activity.ActivityId
                        }
                      });
                    }
                  } else {
                    this.$Message.info(response.data.Msg);
                    if (response.data.FailList != null && response.data.FailList !== undefined && response.data
                      .FailList.length > 0) {
                      this.repeatProductModal.visible = true;
                      this.repeatProductModal.data = response.data.FailList;
                    }
                  }
                });
            } else {
              // 未提交审核流程
              var reslt = this.checkDiscountContentList();
              if (!reslt) {
                this.messageInfo("请输入正确的打折信息");
                return false;
              }
              // 验证打折内容
              var contentList;
              if (this.activity.DiscountMethod === "1") {
                contentList = this.vueData.AmountDiscountContentList;
              } else {
                contentList = this.vueData.CountDiscountContentList;
              }
              if (!(contentList.length > 0)) {
                this.messageInfo("请输入打折信息");
              }
              var limitCheck = this.submitCheckoutUserLimit();
              if (!limitCheck) { 
                return false;
              }
              this.vueData.btnDisabled = true;
              this.ajax
                .post("/SalePromotionActivity/InsertOrUpdateActivity", {
                  model: this.activity,
                  VueDiscountContentList: contentList
                })
                .then(response => {
                  this.vueData.btnDisabled = false;
                  if (response.data.Status) {
                    this.$router.push({
                      path: "/discount/activityproduct",
                      query: {
                        activityId: response.data.ActivityId
                      }
                    });
                  } else {
                    this.$Message.info(response.data.Msg);
                    if (response.data.FailList != null && response.data.FailList !== undefined && response.data
                      .FailList.length > 0) {
                      this.repeatProductModal.visible = true;
                      this.repeatProductModal.data = response.data.FailList;
                    }
                  }
                });
            }
          } else {
            this.messageInfo("请输入必填项");
          }
        });
      },
      submitCheckDiscountContent () { // 验证打折信息是否有输入
        var result = false;
        if (this.activity.DiscountMethod === "1") {
          if (this.vueData.AmountDiscountContentList.length > 0) {
            result = true;
          }
        } else if (this.activity.DiscountMethod === "2") {
          if (this.vueData.CountDiscountContentList.length > 0) {
            result = true;
          }
        }
        return result;
      },
      // 检查用户限购数
      submitCheckoutUserLimit () {
          if (this.activity.Is_PurchaseLimit === "1" && this.activity.LimitQuantity <= 0) {
              this.vueData.limitQuantityTip = "请输入限购数量";
              this.messageInfo("请输入限购数量");
            return false;
          }
        if (this.activity.DiscountMethod === "1" || this.activity.Is_PurchaseLimit === "0") {
          return true;
        } else {
          var maxValue = 0;
          this.vueData.CountDiscountContentList.forEach(element => {
            if (element.Condition > maxValue) {
              maxValue = element.Condition;
            }
          });
          if (maxValue > this.activity.LimitQuantity) {
               this.messageInfo("会场限购数量不可小于打折条件");
            return false;
          } else {
            return true;
          }
        }
      },
      messageInfo (value) {
        this.$Message.info({
          content: value,
          duration: 3,
          closable: true
        });
      },
      addAmountDiscountContent (index) {
        // 添加满额折内容
        this.vueData.AmountDiscountContentList.push({
          Condition: null,
          DiscountRate: null,
          Tip: "" // 错误提示
        });
      },
      delAmountDiscountContent (index) { // 删除打折内容
        this.vueData.AmountDiscountContentList.splice(index, 1);
      },
      addCountDiscountContent (index) {
        // 添加满件折内容
        this.vueData.CountDiscountContentList.push({
          Condition: null,
          DiscountRate: null,
          Tip: ""
        });
      },
      delCountDiscountContent (index) {
        this.vueData.CountDiscountContentList.splice(index, 1);
      }
    }
  };

</script>
