<template>
  <div>
    <h2 class="title">选车攻略</h2>
     <Tabs @on-click="ChangeVehicleLevel">
        <TabPane label="排量" name="2"></TabPane>
        <TabPane label="年份" name="3"></TabPane>
        <TabPane label="款型" name="4"></TabPane>
    </Tabs>
    <vehiclelevelselect 
        :vehicleLevel="vehicleLevel"
        @Search="HandleSearch">
    </vehiclelevelselect>
    <Row style="margin-top:50px;margin-left: 90%;">
    <i-button @click="OpenMultEditDialog">批量编辑</i-button>
    <i-button @click="MultDelete">批量删除</i-button>
    </Row>
    <Table 
    :loading="loading" 
    :data="searchResult" 
    :columns="tableColumns" 
    stripe 
    @on-selection-change="HandleSelectedRows">
    </Table>
    <Page v-if="vehicleLevel<4" style="margin:10px;" 
    :total="pager.TotalCount" 
    :current.sync="pager.PageIndex" 
    :page-size="pager.PageSize" 
    show-total>
    </Page>

    <Modal v-model="addOrEditDialog.show" 
    :mask-closable="false" 
    @on-cancel="CloseAddOrEditDialog" width="500px">
      <p slot="header" style="color:#f60;text-align:center">
        <span v-if="addOrEditDialog.type==='MultEdit'">批量编辑</span>
        <span v-else-if="addOrEditDialog.type==='Edit'">编辑</span>
      </p>
      <div style="text-align:center">
        <table class="ivu-table" style="width:100%;" v-if="addOrEditDialog.type==='MultEdit'">
          <tr>
            <th>文章链接:</th>
            <td style="text-align:left;">
              <p>
                <textarea style="width:95%;" v-model="model.ArticleUrl"></textarea>
              </p>
            </td>
          </tr>
        </table>
        <table class="ivu-table" style="width:100%;" v-else>
          <tr>
            <th>VID:</th>
            <td style="text-align:left;">
              {{model.VehicleId}}
            </td>
          </tr>
          <tr v-if="vehicleLevel>=3">
            <th>排量:</th>
            <td style="text-align:left;">
              {{model.PaiLiang}}
            </td>
          </tr>
          <tr v-if="vehicleLevel>=4">
            <th>年份:</th>
            <td style="text-align:left;">
              {{model.Nian}}
            </td>
          </tr>
          <tr>
            <th>文章链接:</th>
            <td style="text-align:left;">
              <p>
                <textarea style="width:95%;" v-model="model.ArticleUrl"></textarea>
              </p>
            </td>
          </tr>
        </table>
      </div>
      <div slot="footer">
        <Button v-if="addOrEditDialog.type==='Add'" v-on:click="Add()">保存</Button>
        <Button v-else-if="addOrEditDialog.type==='Edit'" v-on:click="Edit()">保存</Button>
        <Button v-else-if="addOrEditDialog.type==='MultEdit'" v-on:click="MultEdit()">保存</Button>
        <Button v-on:click="CloseAddOrEditDialog()">取消</Button>
      </div>
    </Modal>
  </div>
</template>

<script>
import vehiclelevelselect from "@/components/vehiclelevelselect";
export default {
  data () {
    return {
      loading: false,
      searchData: {},
      searchResult: [],
      multEditModel: [],
      vehicleLevel: 2,
      addOrEditDialog: {
        type: "",
        show: false
      },
      model: {},
      pager: {
        PageIndex: 1,
        PageSize: 20,
        TotalCount: 0
      },
      cols: [],
      tableColBegin: [
        {
          type: "selection",
          width: 30,
          align: "center"
        },
        {
          type: "index",
          width: 50,
          align: "center"
        },
        {
          title: "VID",
          key: "VehicleId",
          align: "center"
        },
        {
          title: "品牌",
          key: "Brand",
          align: "center"
        },
        {
          title: "车系",
          key: "VehicleSeries",
          align: "center"
        }
      ],
      tableColEnd: [
        {
          title: "链接",
          key: "ArticleUrl",
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
    };
  },
  components: {
    vehiclelevelselect
  },
  created () {
    this.Search();
  },
  computed: {
    tableColumns: function () {
      return this.tableColBegin.concat(this.cols).concat(this.tableColEnd);
    }
  },
  watch: {
    "pager.PageIndex": function () {
      this.Search();
    }
  },
  methods: {
    Search: function () {
      var vue = this;
      vue.searchResult = [];
      vue.multEditModel = [];
      if (
        vue.vehicleLevel >= 4 &&
        (!vue.searchData.VehicleId || !vue.searchData.PaiLiang)
      ) {
        vue.$Message.warning("至少选到排量!");
        return;
      }
      vue.loading = true;
      this.searchData.PageIndex = vue.pager.PageIndex || 1;
      vue.searchData.PageSize = vue.pager.PageSize || 20;
      vue.ajax
        .post("/VehicleArticle/SelectVehicleArticleModel", {
          request: vue.searchData,
          vehicleLevel: vue.vehicleLevel
        })
        .then(response => {
          var res = response.data;
          if (!res.Status) {
            vue.$Message.error("查询失败!" + (res.Msg || ""));
          }
          vue.searchResult = res.Data || [];
          vue.pager.TotalCount = res.TotalCount || 0;
          vue.loading = false;
        });
    },
    // 编辑配置
    Edit: function () {
      var vm = this;
      var model = vm.model;
      model.ArticleUrl = model.ArticleUrl.trim() || "";
      if (!model.ArticleUrl) {
        vm.$Message.warning("文章链接不能为空!");
        return;
      }
      if (!confirm("是否确认更新 " + model.VehicleId + "的配置 ? ")) {
        return;
      }
      this.ajax
        .post("/VehicleArticle/UpSertVehicleArticle", {
          model: model
        })
        .then(function (result) {
          result = (result || []).data || [];
          if (result && result.Status) {
            vm.$Message.success("编辑成功!");
            vm.CloseAddOrEditDialog();
            setTimeout(function () {
              vm.RemoveCache([model]);
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
    // 批量编辑
    MultEdit: function () {
      var vm = this;
      vm.model.ArticleUrl = vm.model.ArticleUrl.trim() || "";
      if (!vm.model.ArticleUrl) {
        vm.$Message.warning("文章链接不能为空!");
        return;
      }
      if (vm.multEditModel && vm.multEditModel.length > 0) {
        var models = vm.multEditModel
          .filter(function (x) {
            return Boolean(x.VehicleId);
          })
          .map(function (x) {
            return x;
          });
        this.ajax
          .post("/VehicleArticle/MultUpsertVehicleArticle", {
            models: models,
            articleUrl: vm.model.ArticleUrl
          })
          .then(function (result) {
            result = (result || []).data || [];
            if (result && result.Status) {
              vm.$Message.success("批量编辑成功!");
              setTimeout(function () {
                vm.RemoveCache(models);
                vm.Search();
              }, 2000);
              vm.CloseAddOrEditDialog();
            } else {
              vm.$Message.error({
                content: "批量编辑失败!" + (result.Msg || ""),
                duration: 10,
                closable: true
              });
            }
          });
      }
    },
    // 删除配置
    Delete: function (item) {
      var vm = this;
      if (!item || !item.VehicleId) {
        return;
      }
      if (!confirm("确认删除VID： " + item.VehicleId + "的配置?")) {
        return;
      }
      this.ajax
        .post("/VehicleArticle/DeleteVehicleArticle", {
          model: item
        })
        .then(function (result) {
          result = (result || []).data || [];
          if (result && result.Status) {
            vm.$Message.success("删除成功!");
            setTimeout(function () {
              vm.RemoveCache([item]);
              vm.Search();
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
    // 批量删除
    MultDelete: function () {
      var vm = this;
      if (!(vm.multEditModel && vm.multEditModel.length > 0)) {
        vm.$Message.warning("未勾选任何记录");
        return;
      }
      var models = vm.multEditModel
        .filter(function (x) {
          return Boolean(x.VehicleId);
        })
        .map(function (x) {
          return x;
        });
      if (!confirm("是否确认批量删除?")) {
        return;
      }
      this.ajax
        .post("/VehicleArticle/MultDeleteVehicleArticle", {
          models: models
        })
        .then(function (result) {
          result = (result || []).data || [];
          if (result && result.Status) {
            vm.$Message.success("批量删除成功!");
            setTimeout(function () {
              vm.RemoveCache(models);
              vm.Search();
            }, 2000);
          } else {
            vm.$Message.error({
              content: "批量删除失败!" + (result.Msg || ""),
              duration: 10,
              closable: true
            });
          }
        });
    },
    OpenEditDialog: function (item) {
      var vm = this;
      vm.imgFile = null;
      vm.model = JSON.parse(JSON.stringify(item));
      vm.addOrEditDialog.type = "Edit";
      vm.addOrEditDialog.show = true;
    },
    OpenMultEditDialog: function () {
      var vm = this;
      vm.imgFile = null;
      vm.model = {};
      if (vm.multEditModel && vm.multEditModel.length > 0) {
        vm.addOrEditDialog.type = "MultEdit";
        vm.addOrEditDialog.show = true;
      } else {
        vm.$Message.warning("未勾选任何记录");
      }
    },
    // 关闭对话框
    CloseAddOrEditDialog: function () {
      var vm = this;
      vm.addOrEditDialog.show = false;
    },
    // 表格展示列根据等级变化
    ChangeVehicleLevel: function (level) {
      this.searchResult = [];
      this.vehicleLevel = parseInt(level);
      if (this.vehicleLevel === 2) {
        this.cols = [];
      } else if (this.vehicleLevel === 3) {
        this.cols = [
          {
            title: "排量",
            key: "PaiLiang",
            align: "center"
          }
        ];
      } else if (this.vehicleLevel === 4) {
        this.cols = [
          {
            title: "排量",
            key: "PaiLiang",
            align: "center"
          },
          {
            title: "年份",
            key: "Nian",
            align: "center"
          }
        ];
      }
      this.Search();
    },
    // 多选
    HandleSelectedRows: function (selections) {
      this.multEditModel = selections;
    },
    // 车型组件查询
    HandleSearch: function (searchInfo) {
      this.searchData = searchInfo;
      this.Search();
    },
    // 清除缓存
    RemoveCache: function (models) {
      var vm = this;
      if (models && models.length > 0) {
        this.ajax
          .post("/VehicleArticle/RemoveCache", {
            models: models
          })
          .then(function (result) {
            result = (result || []).data || [];
            if (result && result.Status) {
              vm.$Message.success("清除缓存成功!");
            } else {
              vm.$Message.error({
                content: "清除缓存失败!" + (result.Msg || ""),
                duration: 10,
                closable: true
              });
            }
          });
      }
    }
  }
};
</script>
