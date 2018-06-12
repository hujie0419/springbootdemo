<template>
  <div>
    <div>运营 > 优惠券 > <router-link to="/shoppromotion/index">门店优惠券规则</router-link> > 新建规则</div>
    <div style="padding:20px;">
    <Form ref="formValidate" :model="formValidate" :rules="ruleValidate" :label-width="150">
        <FormItem label="优惠券类型" prop="PromotionType">
          <RadioGroup v-model="formValidate.PromotionType">
            <Radio :label="0">满减券</Radio>
          </RadioGroup>
        </FormItem>
        <FormItem label="优惠券规则名称：" prop="PromotionName">
            <Row>
                <Col span="12">
                    <Input v-model="formValidate.PromotionName" placeholder="用于区分不同的优惠券规则和关键字搜索"></Input> 
                </Col>
                <Col span="4" style="color:red;">
                限15个字
                </Col>
            </Row>
        </FormItem>
        <FormItem label="优惠券面值" prop="Discount">
            <Row>
                <Col span="8">
                    <FormItem>
                        <Input  v-model="formValidate.Discount"></Input>
                    </FormItem>
                </Col>
                <Col span="2">元</Col>
                <Col span="2">/</Col>
                <Col span="1">满</Col>
                <Col span="8">
                    <FormItem>
                        <Input v-model="formValidate.MinMoney"></Input>
                    </FormItem>
                </Col>
                <Col span="2">元可使用</Col>
            </Row>
        </FormItem>
        <FormItem label="用户限定" prop="SupportUserRange">
          <RadioGroup v-model="formValidate.SupportUserRange">
            <Radio :label="0">全部</Radio>
          </RadioGroup>
        </FormItem>
        <FormItem label="可使用产品" prop="ProductType">
            <RadioGroup v-model="formValidate.ProductType" @on-change="onProductTypeChange">
                <Radio :label="0">全部服务产品</Radio>
                <Radio :label="1">自定义可用产品ID</Radio>
            </RadioGroup>
            <div v-show="formValidate.ProductType === 1">
                <div>共{{formValidate.Pids.length}}个自定义产品<a href="javascript:void(0)" @click="showServiceModal()">修改</a></div>
                <ServiceTableList :opts="selectServices" ref="showService"></ServiceTableList>
            </div>
        </FormItem>
        
        <FormItem label="使用说明">
            <Input v-model="formValidate.Description" type="textarea" :autosize="{minRows: 3,maxRows: 5}"></Input>
        </FormItem>
        <FormItem>
            <Button type="primary" @click="handleSubmit('formValidate')">保存</Button>
            <Button type="primary" @click="handleSubmit('formValidate', true)">保存并上架</Button>
            <Button type="ghost" @click="handleReset('formValidate')" style="margin-left: 8px">取消</Button>
        </FormItem>
    </Form>
   </div>
   <Modal
        v-model="showModal"
        title="选择可以使用优惠券的服务产品"
        width="900">
       <ServiceList ref="service" :opts.sync="selectServices" :showModal="showModal"></ServiceList>
        <div slot="footer">
            <Button type="primary" @click="ok">确定</Button>
        </div>
    </Modal>
  </div>
</template>
<script>
import ServiceList from '@/views/shoppromotion/servicelist.vue'
import ServiceTableList from '@/views/shoppromotion/serviceTableList.vue'
import util from "@/framework/libs/util.js"
export default {
  data () {
    let _this = this
    return {
      formValidate: {
          PromotionName: '',
          Discount: 0,
          MinMoney: 0,
          PromotionType: 0,
          DisplayName: '',
          SupportUserRange: 0,
          Description: '',
          ProductType: 0,
          Pids: []
      },
      ruleValidate: {
          PromotionName: [
              { 
                trigger: 'blur',
                validator: (rule, value, callback) => {
                    if (value === '') {
                        callback(new Error('优惠券名称不能为空'))
                    } else if (value.length > 15) {
                        callback(new Error('字数不能超过15个字'))
                    } else {
                        callback()
                    }
                } 
              }
          ],
          Discount: [
            { 
                trigger: 'blur',
                validator: (rule, value, callback) => {
                    if (value === '') {
                        callback(new Error('折扣不能为空'))
                    } else if (isNaN(Number(value))) {
                        callback(new Error("折扣请输入数字"))
                    } else if (Number(value) <= 0) {
                        callback(new Error("折扣必须大于0"))
                    } else if (_this.formValidate.MinMoney === '') {
                        callback(new Error('最低金额不能为空'))
                    } else if (isNaN(Number(_this.formValidate.MinMoney))) {
                        callback(new Error("最低金额请输入数字"))
                    } else if (Number(_this.formValidate.Discount) > Number(_this.formValidate.MinMoney)) {
                        callback(new Error("不能高于优惠券最低价"))
                    } else {
                        callback()
                    }
                }
            }
          ],
          ProductType: [
              { 
                trigger: 'blur',
                validator: (rule, value, callback) => {
                    if (value === 1) {
                        if (_this.formValidate.Pids == null || _this.formValidate.Pids.length === 0) {
                            callback(new Error('请至少配置一个产品'))
                        } else {
                            callback()
                        }
                    } else {
                        callback()
                    }
                }
            }
          ]
      },
      showModal: false,
      selectServices: {}
    }
  },
  created () {
      let ruleId = this.$route.params.ruleId || 0
      // ruleId 大于0 代表是编辑
      if (ruleId > 0) {
          this.loadDetail(ruleId).then((data) => {
              if (data) {
                  this.formValidate.PromotionType = data.PromotionType
                  this.formValidate.PromotionName = data.PromotionName
                  this.formValidate.Description = data.Description
                  this.formValidate.Discount = data.Discount.toString()
                  this.formValidate.MinMoney = data.MinMoney.toString()
                  if (data.Pids.length) {
                      this.formValidate.Pids = data.Pids
                      this.formValidate.ProductType = 1
                  }
                  return this.formValidate.Pids
              }
              return []
          }).then((pids) => {
              pids.forEach(p => {
                  this.selectServices[p] = {ProductID: p}
              })
              if (this.formValidate.Pids.length) {
                  this.$refs.showService.loadList(this.selectServices)
              }
          });
      }
  },
  components: {
    ServiceList,
    ServiceTableList
  },
  methods: {
    onProductTypeChange () {
        if (this.formValidate.ProductType === 1) {
            if (!this.formValidate.Pids.length) {
                this.showServiceModal()
            }
            this.$refs.showService.loadList(this.selectServices)
        }
    },
    showServiceModal () {
        this.showModal = true
        this.$refs.service.loadList(this.selectServices)
    },
    handleSubmit (name, up) {
        this.$refs[name].validate((valid) => {
            if (valid) {
                this.saveData().then((data) => {
                    if (data.data && up) {
                        return this.updateStatus(data.ruleId, 1);
                    }
                }).then(() => {
                    this.$router.push({'name': 'shoppromotionIndex'});
                });
            }
        })
    },
    loadDetail (ruleId) {
        return util.ajax.get('/shoppromotion/GetCouponRuleDetail',
        {
            params: {
            ruleId: ruleId
        }}).then((response) => {
            if (response.status === 200) {
                return response.data.data;
            }
            return null;
        })
    },
    saveData () {
        if (this.formValidate.ProductType === 0) {
            this.formValidate.Pids = [];
        }
        return util.ajax.post('/shoppromotion/SaveCouponRules', this.formValidate).then((response) => {
            if (response.status === 200) {
                this.$Message.success('保存成功');
                return response.data;
            }
        })
    },
    updateStatus (id, status) {
          return util.ajax.post("/shoppromotion/UpdateCouponStatus", {
            ruleId: id,
            status: status
          })
    },
    handleReset (name) {
        this.$refs[name].resetFields();
    },
    ok () {
        this.showModal = false;
        var pids = [];
        for (var key in this.selectServices) {
            pids.push(key);
        }
        this.formValidate.Pids = pids;
        this.$refs.showService.loadList(this.selectServices)
    }
  }
}
</script>
<style>
.leftCol {text-align: right; }
.rightCol {text-align: left; padding-left: 20px;}
</style>
