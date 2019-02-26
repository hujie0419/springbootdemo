<template>
  <div v-cloak>
    <Row :gutter="2">
      <i-col span="4">
        <i-select transfer v-model="searchModel.Pid"
          placeholder="-请选择Pid-">
          <i-option v-for="pid in pids" :value="pid" :key="pid">{{pid}}</i-option>
        </i-select>
      </i-col>
      <i-col span="14">
        <RegionSelect @on-district-changed="regionChanged"></RegionSelect>
      </i-col>
      <i-col span="2">
        <Button type="primary" v-on:click="Search(1)">查询</Button>
        <Button v-if="searchModel.Pid" type="error"
          v-on:click="OpenAddDialog()">添加</Button>
      </i-col>
    </Row>

    <Table style="margin-top:50px;" :loading="loading"
      :data="table.Data" :columns="table.Columns" stripe></Table>
    <Page style="margin-top:10px;" :total="pager.TotalCount"
      :current.sync="pager.PageIndex" :page-size="pager.PageSize"
      show-total></Page>

    <Modal v-model="addOrEditDialog.show" :mask-closable="false"
      v-on:on-cancel="CloseAddOrEditDialog" width="500px">
      <p slot="header" style="color:#f60;text-align:center">
        <span v-if="addOrEditDialog.type==='Add'">添加</span>
        <span v-else-if="addOrEditDialog.type==='Edit'">编辑</span>
      </p>
      <div style="text-align:center">
        <table class="ivu-table" style="width:100%;">
          <tr>
            <th>城市:</th>
            <td v-if="addOrEditDialog.type==='Add'">
              <RegionSelect @on-district-changed="AddDialogRegionChanged"></RegionSelect>
            </td>
            <td v-else>
              <label>{{model.ProvinceName}}</label>
              <label> {{model.CityName}} </label>
              <label>{{model.DistrictName}}</label>
            </td>
          </tr>
          <tr>
            <th>备注</th>
            <td>
              <i-input v-model="model.Remark">
              </i-input>
            </td>
          </tr>
          <tr>
            <th>是否启用</th>
            <td>
              <i-switch size="large" v-model="model.IsEnabled">
                <span slot="open">开启</span>
                <span slot="close">禁用</span>
              </i-switch>
            </td>
          </tr>
        </table>
      </div>
      <div slot="footer">
        <Button v-if="addOrEditDialog.type==='Add'"
          v-on:click="Add()">保存</Button>
        <Button v-else-if="addOrEditDialog.type==='Edit'"
          v-on:click="Edit()">保存</Button>
        <Button v-on:click="CloseAddOrEditDialog()">取消</Button>
      </div>
    </Modal>
  </div>
</template>

<script>
import regionSelect from "./regionSelect";
export default {
  props: {
    productType: {
      type: String,
      required: true
    }
  },
  data () {
    return {
      searchModel: {
        CoverType: "Pid",
        Brand: "",
        Pid: "",
        Region: {}
      },
      pids: [],
      excelFile: null,
      uploadExcelStatus: false,
      loading: false,
      table: {
        Data: [],
        Columns: [
          {
            type: "index",
            width: 50,
            align: "center"
          },
          {
            title: "省份",
            key: "ProvinceName",
            align: "center"
          },
          {
            title: "城市",
            key: "CityName",
            align: "center"
          },
          {
            title: "区",
            key: "DistrictName",
            align: "center"
          },
          {
            title: "是否启用",
            key: "IsEnabled",
            align: "center",
            render: (h, p) => {
              var labelhtml = "";
              if (p.row.IsEnabled) {
                labelhtml =
                  '<label style="background-color:#0052cc;color:white; padding:3px 4px">开启</label>';
              } else {
                labelhtml =
                  '<label style="background-color:#ee0701;color:white;padding:3px 4px">禁用</label>';
              }
              return h("span", {
                domProps: {
                  innerHTML: labelhtml
                }
              });
            }
          },
          {
            title: "备注",
            key: "Remark",
            align: "center"
          },
          {
            title: "操作",
            key: "operate",
            width: 180,
            align: "center",
            render: (h, params) => {
              return h("div", [
                h(
                  "Button",
                  {
                    props: {
                      type: "primary"
                    },
                    style: {
                      marginRight: "5px"
                    },
                    on: {
                      click: () => {
                        this.OpenEditDialog(params.row);
                      }
                    }
                  },
                  "编辑"
                ),
                h(
                  "Button",
                  {
                    props: {
                      type: "error"
                    },
                    on: {
                      click: () => {
                        this.Delete(params.row);
                      }
                    }
                  },
                  "删除"
                )
              ]);
            }
          }
        ]
      },
      pager: {
        PageIndex: 1,
        PageSize: 20,
        TotalCount: 0
      },
      packageModel: {
        PKID: "",
        UserType: "",
        PackageName: "",
        IsEnabled: false
      },
      operateType: "",
      model: {},
      searchResult: [],
      addOrEditDialog: {
        show: false,
        type: ""
      }
    };
  },
  components: {
    RegionSelect: regionSelect
  },
  created () {
    window.PidCoverAreaVue = this;
    this.$Message.config({
      top: 50,
      duration: 5
    });
    this.GetAllPids();
  },
   watch: {
    "pager.PageIndex": function () {
        var vm = this;
        vm.Search(vm.pager.PageIndex);
      }
  },
  methods: {
    // 地区选择组件
    regionChanged: function (region) {
      var vm = this;
      vm.searchModel.Region = region || {};
    },
    GetAllPids: function () {
      var vm = this;
      vm.util.ajax
        .post("/VPCoverAreaConfig/GetAllPids", {
          productType: vm.productType
        })
        .then(function (res) {
          var data = (res || []).data || [];
          vm.pids = data.Data || [];
        });
    },
    // 查询
    Search: function (pageIndex) {
      var vm = this;
      vm.pager.PageIndex = pageIndex;
      if (!vm.searchModel.Pid) {
        vm.$Message.warning("请选择Pid");
        return;
      }
      vm.util.ajax
        .post("/VPCoverAreaConfig/SelectCoverArea", {
          productType: vm.productType,
          coverType: vm.searchModel.CoverType,
          brand: vm.searchModel.Brand,
          pid: vm.searchModel.Pid,
          provinceId: vm.searchModel.Region.ProvinceId || 0,
          cityId: vm.searchModel.Region.CityId || 0,
          districtId: vm.searchModel.Region.DistrictId || 0,
          pageIndex: vm.pager.PageIndex || 1,
          pageSize: vm.pager.PageSize || 20
        })
        .then(function (result) {
          result = (result || []).data || [];
          vm.pager.TotalCount = result.TotalCount || 0;
          if (result.Status) {
            console.log(result.Data);
            vm.table.Data = result.Data;
          } else {
            vm.$Message.error({
              content: "查询失败!" + (result.Msg || ""),
              duration: 10,
              closable: true
            });
          }
        });
    },
    OpenAddDialog: function (item) {
      var vm = this;
      vm.model = {};
      vm.model.ProductType = vm.productType;
      vm.model.CoverType = vm.searchModel.CoverType;
      vm.model.Pid = vm.searchModel.Pid;
      vm.addOrEditDialog.type = "Add";
      vm.addOrEditDialog.show = true;
    },
    OpenEditDialog: function (item) {
      var vm = this;
      vm.model = JSON.parse(JSON.stringify(item));
      vm.addOrEditDialog.type = "Edit";
      vm.addOrEditDialog.show = true;
    },
    // 关闭对话框
    CloseAddOrEditDialog: function () {
      var vm = this;
      vm.addOrEditDialog.show = false;
    },
    AddDialogRegionChanged (addRegion) {
      var vm = this;
      vm.model.CoverRegionId = addRegion.DistrictId;
    },
    Add: function () {
      var vm = this;
      var model = vm.model;
      if (!vm.CheckConfig(model)) {
        return;
      }
      if (!confirm("是否确认添加配置 ? ")) {
        return;
      }
      vm.util.ajax
        .post("/VPCoverAreaConfig/AddCoverArea", {
          model: model
        })
        .then(function (result) {
          result = (result || []).data || [];
          if (result && result.Status) {
            vm.$Message.success("添加成功!");
            vm.CloseAddOrEditDialog();
            setTimeout(function () {
              vm.RemoveCache(model);
              vm.Search(vm.pager.pageIndex);
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
    Delete: function (model) {
      var vm = this;
      if (!vm.CheckConfig(model)) {
        return;
      }
      if (
        !confirm(
          "是否确认删除" +
            model.ProvinceName +
            "-" +
            model.CityName +
            "-" +
            model.DistrictName +
            "的配置 ? "
        )
      ) {
        return;
      }
      vm.util.ajax
        .post("/VPCoverAreaConfig/DeleteCoverArea", {
          model: model
        })
        .then(function (result) {
          result = (result || []).data || [];
          if (result && result.Status) {
            vm.$Message.success("删除成功!");
            setTimeout(function () {
              vm.RemoveCache(model);
              vm.Search(vm.pager.pageIndex);
            }, 2000);
          } else {
            vm.$Message.error({
              content: "删除失败!" + (result.Msg || ""),
              duration: 10,
              closable: true
            });
          }
        });
    },
    // 编辑配置
    Edit: function () {
      var vm = this;
      var model = vm.model;
      if (!vm.CheckConfig(model)) {
        return;
      }
      if (
        !confirm(
          "是否确认更新 " +
            model.ProvinceName +
            "-" +
            model.CityName +
            "-" +
            model.DistrictName +
            "的配置 ? "
        )
      ) {
        return;
      }
      model.ProductType = vm.productType;
      model.CoverType = vm.searchModel.CoverType;
      vm.util.ajax
        .post("/VPCoverAreaConfig/EditCoverArea", {
          model: model
        })
        .then(function (result) {
          result = (result || []).data || [];
          if (result && result.Status) {
            vm.$Message.success("编辑成功!");
            vm.CloseAddOrEditDialog();
            setTimeout(function () {
              vm.RemoveCache(model);
              vm.Search(vm.pager.pageIndex);
            }, 2000);
          } else {
            vm.$Message.error({
              content: "编辑失败!" + (result.Msg || ""),
              duration: 10,
              closable: true
            });
          }
        });
    },
     // 移除缓存
    RemoveCache (model) {
      this.ajax
        .post("/VPCoverAreaConfig/RemoveCache", {
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
    CheckConfig (data) {
      var vm = this;
      var isValiad = false;
      if (data) {
        if (!data.ProductType) {
          vm.$Message.warning("请刷新页面重试");
          return isValiad;
        } else if (!data.CoverType) {
          vm.$Message.warning("请刷新页面重试");
          return isValiad;
        } else if (!data.Pid) {
          vm.$Message.warning("请选择Pid");
          return isValiad;
        } else if (!data.CoverRegionId) {
          vm.$Message.warning("请选择省市区");
          return isValiad;
        } else {
          isValiad = true;
        }
      }
      return isValiad;
    }
  }
};
</script>

<style scoped>
[v-cloak] {
  display: none;
}
</style>
