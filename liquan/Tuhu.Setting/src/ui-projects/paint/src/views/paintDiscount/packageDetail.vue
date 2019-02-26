<template>
  <div v-cloak>
    <Form :model="packageModel" :rules="formRule" label-position="left" :label-width="100">
      <FormItem label="活动名称:" prop="PackageName">
        <i-input v-model="packageModel.PackageName" :disabled="operateType==='Edit'" style="width:20%"></i-input>
      </FormItem>
      <FormItem label="限制用户:" prop="UserType">
        <RadioGroup v-model="packageModel.UserType">
          <Radio :label=1 :value=1 :disabled="operateType==='Edit'">
            <span>新用户</span>
          </Radio>
          <Radio :label=2 :value=2 :disabled="operateType==='Edit'">
            <span>老用户</span>
          </Radio>
          <Radio :label=3 :value=3 :disabled="operateType==='Edit'">
            <span>全部用户</span>
          </Radio>
        </RadioGroup>
      </FormItem>
      <FormItem label="是否启用:" prop="IsEnabled">
        <i-switch size="large" v-model="packageModel.IsEnabled">
          <span :value="true" slot="open">启用</span>
          <span :value="false" slot="close">禁用</span>
        </i-switch> 
        <Tooltip style="margin-left:15px;" placement="right">
          <Icon type="alert"></Icon>
          <div style="width:300px;" slot="content">
          <p>建议全部配置完成后再启用</p>
          </div>
        </Tooltip>
      </FormItem>
      <FormItem>
        <Button v-if="operateType==='Add'" type="primary" @click="AddPackageConfig">保存</Button>
        <Button v-else-if="operateType==='Edit'" type="primary" @click="EditPackageConfig">保存</Button>
      </FormItem>
    </Form>

    <div>
      <packageRegion ref="packageRegion" 
        v-if="operateType=='Edit' && packageModel.PKID > 0"
        :packageId="packageModel.PKID">
      </packageRegion>
    </div>

    <div>
      <discountDetail ref="discountDetail" 
        v-if="operateType=='Edit' && packageModel.PKID>0" 
        :packageId="packageModel.PKID" 
        :packageName="packageModel.PackageName">
      </discountDetail>
    </div>
  </div>
</template>

<script>
  import util from "@/framework/libs/util";
  import discountDetail from "@/views/paintDiscount/discountDetail";
  import packageRegion from "@/views/paintDiscount/packageRegion";
  export default {
    name: "packageConfig",
    data () {
      return {
        packageModel: {
          PKID: "",
          UserType: "",
          PackageName: "",
          IsEnabled: false
        },
        operateType: "",
        // form表单验证规则
        formRule: {
          PackageName: [{
            required: true,
            validator: (rule, value, callback) => {
              if (!value || !value.trim()) {
                callback(new Error('请输入活动名称'));
              } else {
                callback();
              }
            },
            trigger: 'blur'
          }],
          UserType: [{
            required: true,
            type: "number",
            message: "请选择限制用户",
            trigger: 'change'
          }],
          IsEnabled: [{
            required: true,
            type: "boolean",
            message: "请选择是否启用",
            trigger: "change"
          }]
        }
      };
    },
    components: {
      packageRegion,
      discountDetail
    },
    created: function () {
      this.$Message.config({
        top: 50,
        duration: 5
      });
      this.GetParaByUrl();
    },
    watch: {
      $route (to, form) {
        this.GetParaByUrl();
      }
    },
    methods: {
      GetParaByUrl: function () {
        var vm = this;
        var packageId = this.$route.query.packageId;
        vm.Init(packageId);
      },
      // 查询
      Init: function (packageId) {
        var vm = this;
        if (!parseInt(packageId) ||
          packageId < 1) {
          vm.packageModel = {};
          vm.operateType = "Add";
          return;
        }
        util.ajax
          .post("/PaintDiscountConfig/GetPackageConfig", {
            packageId: packageId
          })
          .then(function (result) {
            result = (result || []).data || [];
            if (result.Status && result.Data) {
              vm.packageModel = result.Data;
              vm.operateType = "Edit";
            } else {
              vm.packageModel = {};
            }
          });
      },
      // 新建喷漆打折价格体系
      AddPackageConfig: function () {
        var vm = this;
        var model = vm.packageModel;
        if (!model.PackageName) {
          vm.$Message.warning("请输入喷漆价格体系名称");
          return;
        }
        if (!confirm("是否确认添加：" + model.PackageName + " 的价格体系配置 ? ")) {
          return;
        }
        util.ajax
          .post("/PaintDiscountConfig/AddPackageConfig", {
            model: model
          })
          .then(function (result) {
            result = (result || []).data || [];
            if (result && result.Status) {
              vm.$Message.success("添加成功!");
              setTimeout(function () {
                vm.$router.push(
                  "/packageDetail?packageId=" +
                  result.PackageId
                );
              }, 2000);
            } else {
              vm.$Message.error({
                content: "添加失败!" + (result.Msg || ""),
                duration: 10,
                closable: true
              });
            }
          });
      },
      // 编辑配置
      EditPackageConfig: function () {
        var vm = this;
        var model = vm.packageModel;
        if (!model.PackageName) {
          return;
        }
        if (!confirm("是否确认更新 " + model.PackageName + "的配置 ? ")) {
          return;
        }
        util.ajax
          .post("/PaintDiscountConfig/UpdatePackageConfig", {
            model: model
          })
          .then(function (result) {
            result = (result || []).data || [];
            if (result && result.Status) {
              vm.$Message.success("编辑成功!" + (result.Msg || ""));
              setTimeout(function () {
                vm.Init(vm.packageModel.PKID);
                vm.RemoveCache();
              }, 2000);
            } else {
              vm.$Message.error({
                content: "编辑失败!" + (result.Msg || ""),
                duration: 0
              });
            }
          });
      },
      RemoveCache: function () {
        var vm = this;
        vm.$refs.packageRegion.RemoveCache();
        vm.$refs.discountDetail.RemoveCache('');
      }
    }
  };

</script>

<style scoped>
[v-cloak] {
  display: none;
}
</style>
