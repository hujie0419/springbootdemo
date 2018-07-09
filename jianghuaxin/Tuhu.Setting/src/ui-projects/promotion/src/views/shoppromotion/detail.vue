<template>
  <div>
    <div>运营 > 优惠券 > <router-link to="/shoppromotion/index">门店优惠券规则</router-link> > 查看规则 > ID:{{formValidate.RuleId}} 
      (
        <span v-if="ruleStatus === 0">待发布</span>
        <span v-if="ruleStatus === 1">可领取</span>
        <span v-if="ruleStatus === 2">暂停领取</span>
        <span v-if="ruleStatus === 3">已作废</span>
      )
    </div>
    <div style="padding:20px;">
    <Form ref="formValidate" :model="formValidate" :label-width="150">
        <FormItem label="优惠券规则名称：">
            <b>{{formValidate.PromotionName}}</b>
        </FormItem>
        <FormItem label="优惠券类型：">
          {{(formValidate.PromotionType === 0 ? "满减券" : "")}}
        </FormItem>
        
        <FormItem label="优惠券面额：">
            <b>{{formValidate.Discount}}</b>元 / 满<b>{{formValidate.MinMoney}}</b>元可用
        </FormItem>
        <FormItem label="用户限定：">
            全部
        </FormItem>
        <FormItem label="可使用产品" prop="ProductType">
            <RadioGroup v-model="formValidate.ProductType" @on-change="onProductTypeChange">
                <Radio :disabled="ruleStatus === 3" :label="0">全部服务产品</Radio>
                <Radio :disabled="ruleStatus === 3" :label="1">自定义可用产品ID</Radio>
            </RadioGroup>
            <div v-show="formValidate.ProductType === 1">
            <div>共{{formValidate.Pids.length}}个自定义产品<a v-if="ruleStatus !== 3" href="javascript:void(0)" @click="showServiceModal()">修改</a></div>
            <ServiceTableList ref="showService" :opts="selectServices" ></ServiceTableList>
        </div>
        </FormItem>
        
        <FormItem label="使用说明：">
            {{formValidate.Description}}
        </FormItem>
        <FormItem>
            <Button v-if="ruleStatus === 0 || ruleStatus === 2" type="primary" @click="updateStatus(1)">上架领取</Button>
            <Button v-if="ruleStatus === 1" type="primary" @click="updateStatus(2)">暂停领取</Button>
            <Button v-if="ruleStatus !== 3" type="primary" @click="updateStatus(3)">作废</Button>
            <Button v-if="ruleStatus !== 3" type="primary" @click="showConfirmModal()">保存</Button>
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
    <Modal
        v-model="showConfirm"
        title="确认更改"
        width="500"
        @on-ok="confirmOk"
        @on-cancel="confirmCancel">
        <div>修改将影响到{{promotionCount}}张未使用的优惠券，确认修改?</div>
    </Modal>
  </div>
</template>
<script>
import ServiceList from '@/views/shoppromotion/servicelist.vue'
import ServiceTableList from '@/views/shoppromotion/serviceTableList.vue'
import util from "@/framework/libs/util.js"
export default {
  data () {
    return {
      formValidate: {
          RuleId: 0,
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
      ruleStatus: -1,
      showModal: false,
      showConfirm: false,
      promotionCount: 0,
      selectServices: {}
    }
  },
  created () {
      let ruleId = this.$route.params.ruleId || 0
      this.formValidate.RuleId = ruleId
      // ruleId 大于0 代表是编辑
      if (this.formValidate.RuleId > 0) {
          this.loadDetail().then(() => {
            this.$refs.showService.loadList(this.selectServices)
          })
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
            // this.$refs.showService.loadList(this.selectServices)
        }
    },
    showServiceModal () {
        this.showModal = true
        this.$refs.service.loadList(this.selectServices)
    },
    saveData () {
        if (this.formValidate.ProductType === 0) {
            this.formValidate.Pids = [];
        }
        return util.ajax.post('/shoppromotion/SaveCouponRules', this.formValidate);
    },
    loadDetail () {
        return util.ajax.get('/shoppromotion/GetCouponRuleDetail',
        {
            params: {
            ruleId: this.formValidate.RuleId
        }}).then((response) => {
            if (response.status === 200) {
                let data = response.data.data;
                if (data) {
                  this.formValidate.PromotionType = data.PromotionType
                  this.formValidate.PromotionName = data.PromotionName
                  this.formValidate.Description = data.Description
                  this.formValidate.Discount = data.Discount.toString()
                  this.formValidate.MinMoney = data.MinMoney.toString()
                  if (data.Pids.length) {
                    this.formValidate.Pids = data.Pids
                    this.formValidate.ProductType = 1
                    this.selectServices = {};
                    this.formValidate.Pids.forEach(p => {
                        this.selectServices[p] = {ProductID: p};
                    })
                  }
                  this.ruleStatus = data.Status
                  return data
              }
            }
            return null;
        })
    },
    updateStatus (status) {
          return util.ajax.post("/shoppromotion/UpdateCouponStatus", {
            ruleId: this.formValidate.RuleId,
            status: status
          }).then((response) => {
            if (response.status === 200) {
                this.$Message.success('操作成功');
                this.loadDetail();
            }
          })
    },
    ok () {
        this.showModal = false;
        var pids = [];
        for (var key in this.selectServices) {
            pids.push(key);
        }
        this.formValidate.Pids = pids;
        this.$refs.showService.loadList(this.selectServices)
    },
    showConfirmModal () {
        this.loadPromotionCount().then((data) => {
            this.showConfirm = true
        })
    },
    loadPromotionCount () {
        return util.ajax.get('/shoppromotion/GetNotUserdPromotionCount', 
        {
            params: {
                ruleId: this.formValidate.RuleId
            }
        }).then((response) => {
            this.promotionCount = response.data.data
            return response
        })
    },
    confirmOk () {
        this.saveData()
    },
    confirmCancel () {
        
    }
  }
}
</script>
<style>
.leftCol {text-align: right; }
.rightCol {text-align: left; padding-left: 20px;}
</style>
