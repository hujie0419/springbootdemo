<template>
  <div>
    <div>
      <label>品牌排序:</label>
      <span>{{searchResult.Priority}}</span>
      <Button style="margin-left:10px;" type="ghost" @click="OpenEditDialog">编辑</Button>
    </div>
    <Modal v-model="addOrEditDialog.show" :mask-closable="false"
      v-on:on-cancel="CloseAddOrEditDialog" width="500px">
      <p slot="header" style="color:#f60;text-align:center">
        <span>编辑</span>
      </p>
      <div style="text-align:center">
        <Row style="margin-top:5px;" v-for="index in chooseSize"
          :key="index">
          <i-col span=18>
            <i-select transfer v-model="brandChoose[index-1]"
              placeholder="-请选择品牌-">
              <i-option value="" key="">-请选择品牌-</i-option>
              <i-option v-for="brand in brands" :value="brand"
                :key="brand" :disabled="brandChoose.length>1 && brandChoose.findIndex(s=>s===brand)>-1">{{brand}}</i-option>
            </i-select>
          </i-col>
          <i-col span=2>
            <label>{{index}}</label>
          </i-col>
          <i-col span=1>
            <a v-if="index>0&& index==chooseSize && index<6 && brandChoose[index-1]"
              @click="brandChoose.push(undefined)">+</a>
          </i-col>
          <i-col span=1>
            <a v-if="index>1" @click="brandChoose.pop()">-</a>
          </i-col>
        </Row>
      </div>
      <div slot="footer">
        <Button v-on:click="Edit()">保存</Button>
        <Button v-on:click="CloseAddOrEditDialog()">取消</Button>
      </div>
    </Modal>
  </div>
</template>
<script>
export default {
  props: {
    productType: {
      type: String,
      required: true
    },
    configType: {
      type: String,
      required: true
    }
  },
  data () {
    return {
      searchResult: [],
      addOrEditDialog: {
        show: false,
        type: ""
      },
      brands: [],
      loading: false,
      brandChoose: [],
      model: {}
    };
  },
  created: function () {
    window.PriorityConfigVue = this;
    this.$Message.config({
      top: 50,
      duration: 5
    });
    this.Search();
    this.GetAllBrands();
  },
  watch: {},
  computed: {
       chooseSize () {
           return (this.brandChoose || []).length;
       }
  },
  methods: {
    GetAllBrands: function () {
      var vm = this;
      vm.util.ajax
        .post("/VPCoverAreaConfig/GetAllBrands", {
          productType: vm.productType
        })
        .then(function (res) {
          var data = (res || []).data || [];
          vm.brands = data.Data || [];
        });
    },
    Search () {
      var vm = this;
      vm.util.ajax
        .post("/VPPriorityConfig/SelectPriority", {
          productType: vm.productType,
          configType: vm.configType,
          provinceId: 0,
          cityId: 0,
          pageIndex: 1
        })
        .then(function (res) {
          var data = (res || []).data || [];
          vm.searchResult = (data.Data || [])[0] || {};
        });
    },
    OpenEditDialog: function () {
      var vm = this;
      vm.model = JSON.parse(JSON.stringify(vm.searchResult)) || {};
      vm.model.PKID = vm.model.PKID || 0;
      vm.model.ProductType = vm.model.ProductType || vm.productType;
      vm.model.ConfigType = vm.model.ConfigType || vm.configType;
      vm.model.ProvinceId = vm.model.ProvinceId || 0;
      vm.model.CityId = vm.model.CityId || 0;
      vm.model.IsEnabled = vm.model.IsEnabled || 1;
      vm.brandChoose = (vm.model.Priority || "").split(",");
      vm.addOrEditDialog.type = "Edit";
      vm.addOrEditDialog.show = true;
    },
    Edit () {
      var vm = this;
      var choosedBrands = vm.brandChoose.filter(s => s);
      vm.model.Priority = choosedBrands.join(",");
      if (!confirm("确认修改品牌优先级排序为： " + (vm.model.Priority || "无优先级排序") + " ?")) {
        return;
      }
      vm.util.ajax
        .post("/VPPriorityConfig/EditPriority", { model: vm.model })
        .then(function (res) {
          res = (res || []).data || [];
          if (res && res.Status) {
            vm.$Message.success("编辑成功!");
            vm.CloseAddOrEditDialog();
            setTimeout(function () {
              vm.RemoveCache(vm.model);
              vm.Search();
            }, 2000);
          } else {
            vm.$Message.error({
              content: "编辑失败!" + (res.Msg || ""),
              duration: 10,
              closable: true
            });
          }
        });
    },
    // 移除缓存
    RemoveCache (model) {
      this.ajax
        .post("/VPPriorityConfig/RemoveCache", {
          model: model
        })
        .then(response => {
          var res = response.data;
          if (res.Status) {
            this.$Message.info("清除缓存成功");
          } else {
            this.$Message.error("清除缓存失败!" + (res.Msg || ""));
          }
        });
    },
    // 关闭对话框
    CloseAddOrEditDialog: function () {
      var vm = this;
      vm.addOrEditDialog.show = false;
    }
  }
};
</script>

<style scoped>
[v-cloak] {
  display: none;
}
</style>
