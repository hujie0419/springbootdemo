<template>
  <div v-cloak>
    <h2 class="title" style="margin-top:100px;">服务价格配置</h2>
    <label style="margin-left:5px;">选择产品:</label>
    <Select style="width:8%" v-model="searchData.servicePid" filterable>
      <Option value="" key="">-请选择-</Option>
      <Option v-for="service in services" :value="service.ServicePid" :key="service.ServicePid">{{service.ServicePid}}</Option>
    </Select>
    <Button type="primary" v-on:click="Search(1)">查询</Button>
    <Button type="error" v-on:click="OpenAddDialog()">添加</Button>
    <Button v-on:click="RemoveCache('')">刷新服务缓存</Button>
    <Tooltip style="margin-left:5px;" placement="right">
      <Icon type="alert"></Icon>
      <div style="width:300px;" slot="content">
        <p>1.同Pid、同面数 只允许存在一条记录</p>
      </div>
    </Tooltip>
    <div style="display:flex;float:right;">
      <Upload :before-upload="HandleExcelUpload" action="UploadExcel" accept='application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'>
        <Button>导入Excel文件</Button>
      </Upload>
      <div v-if="excelFile !== null">
        上传文件: {{ excelFile.name }}
        <Button v-on:click="ExcelUpload" :loading="uploadExcelStatus">
          {{ uploadExcelStatus ? '上传中' : '点击上传' }}
        </Button>
      </div>
      <Button style="margin-left:5px;height:35px;" @click="ExportExcel"> 导出数据 </Button>
      <Button style="height:35px;margin-left:5px;" v-on:click="OpenMultEditDialog()">批量编辑活动图片</Button>
    </div>
    <Table :loading="loading" style="margin-top:50px;" :data="searchResult" :columns="tableColumns" stripe @on-selection-change="HandleSelectedRows"></Table>
    <Page style="margin:10px;" :total="pager.totalCount" :current.sync="pager.pageIndex" :page-size="pager.pageSize" show-total></Page>

    <Modal v-model="addOrEditDialog.show" :mask-closable="false" v-on:on-cancel="CloseAddOrEditDialog" width="500px">
      <p slot="header" style="color:#f60;text-align:center">
        <span v-if="addOrEditDialog.type==='MultEdit'">批量编辑图片</span>
        <span v-else-if="addOrEditDialog.type==='Add'">添加</span>
        <span v-else-if="addOrEditDialog.type==='Edit'">编辑</span>
      </p>
      <div style="text-align:center">
        <table class="ivu-table" style="width:100%;" v-if="addOrEditDialog.type==='MultEdit'">
          <tr>
            <th>活动图片:</th>
            <td style="text-align:left;">
              <img v-bind:src="model.ActivityImage" style="width:100px;height:100px;" />
              <Upload :before-upload="ImgUpload" action="/Article/AddArticleImg2" accept='image/*' :format="['jpg','jpeg','png']">
                <Button :loading="uploadImgStatus">
                  {{ uploadImgStatus ? '上传中' : '选择图片' }}
                </Button>
                <span style="color:red;"> (540*710)</span>
              </Upload>
              <div v-if="imgFile !== null">
                已上传图片: {{ imgFile.name }}
              </div>
              <p>
                <textarea style="width:95%;" v-model="model.ActivityImage"></textarea>
              </p>
            </td>
          </tr>
        </table>
        <table class="ivu-table" style="width:100%;" v-else>
          <tr>
            <th>活动名称:</th>
            <td style="text-align:left;">
              {{packageName}}
            </td>
          </tr>
          <tr>
            <th>选择产品:</th>
            <td style="text-align:left;" v-if="addOrEditDialog.type==='Add'">
              <i-select style="width:60%" v-model="model.ServicePid" filterable>
                <i-option v-for="service in services" :value="service.ServicePid" :key="service.ServicePid">{{service.ServicePid}}</i-option>
              </i-select>
            </td>
            <td style="text-align:left;" v-else-if="addOrEditDialog.type==='Edit'">
              {{model.ServicePid}}
            </td>
          </tr>
          <tr>
            <th>面数:</th>
            <td style="text-align:left;" v-if="addOrEditDialog.type==='Add'">
              <input type="number" v-model.number="model.SurfaceCount" />
            </td>
            <td style="text-align:left;" v-else-if="addOrEditDialog.type==='Edit'">
              {{model.SurfaceCount}}
            </td>
          </tr>
          <tr>
            <th>活动价:</th>
            <td style="text-align:left;">
              <input type="number" v-model.number="model.ActivityPrice" />
            </td>
          </tr>
          <tr>
            <th>权益名称:</th>
            <td style="text-align:left;">
              <input v-model="model.ActivityName" />
            </td>
          </tr>
          <tr>
            <th>
              活动说明:
            </th>
            <td style="text-align:left;">
              <input v-model="model.ActivityExplain" />
            </td>
          </tr>
          <tr>
            <th>活动图片:</th>
            <td style="text-align:left;">
              <img v-bind:src="model.ActivityImage" style="width:100px;height:100px;" />
              <Upload :before-upload="ImgUpload" action="/Article/AddArticleImg2" accept='image/*' :format="['jpg','jpeg','png']">
                <Button :loading="uploadImgStatus">
                  {{ uploadImgStatus ? '上传中' : '选择图片' }}
                </Button>
                <span style="color:red;"> (540*710)</span>
              </Upload>
              <div v-if="imgFile !== null">
                已上传图片: {{ imgFile.name }}
              </div>
              <p>
                <textarea style="width:95%;" v-model="model.ActivityImage"></textarea>
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

    <Modal v-model="showLog.show" :mask-closable="false" width="50%">
      <p slot="header">
        <span>操作历史</span>
      </p>
      <div style="text-align:center">
        <Table :loading="loading" style="margin-top:10px;" :data="showLog.logs" :columns="logTableCol" stripe @on-selection-change="HandleSelectedRows">
        </Table>
      </div>
      <div slot="footer">
        <Button v-on:click="showLog.show=false">关闭</Button>
      </div>
    </Modal>

    <Modal v-model="showLog.detail.show" :mask-closable="false" width="50%">
      <p slot="header">
        <span>日志详情</span>
      </p>
      <div>
        <table class="ivu-table" style="width:100%;text-align:center;">
          <thead>
            <tr>
              <th>列名</th>
              <th>操作前值</th>
              <th>操作后值</th>
            </tr>
          </thead>
          <tbody>
            <tr v-show="showLog.detail.logProperty.length>0" v-for="property in showLog.detail.logProperty" :key="property">
              <td>{{property}}</td>
              <td v-if="showLog.detail.oldValue!=null">{{showLog.detail.oldValue[property]}}</td>
              <td v-else></td>
              <td v-if="showLog.detail.newValue!=null">{{showLog.detail.newValue[property]}}</td>
              <td v-else></td>
            </tr>
          </tbody>
        </table>
      </div>
      <div slot="footer">
        <Button v-on:click="showLog.detail.show=false">关闭</Button>
      </div>
    </Modal>
  </div>
</template>

<script>
  import util from "@/framework/libs/util";
  export default {
    name: "packageDetail",
    props: {
      'packageId': {
        type: Number,
        required: true
      },
      'packageName': {
        type: String,
        required: true
      }
    },
    data () {
      return {
        tableColumns: [{
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
            title: "PID",
            key: "ServicePid",
            align: "center"
          },
          {
            title: "面数",
            key: "SurfaceCount",
            align: "center"
          },
          {
            title: "活动价",
            key: "ActivityPrice",
            align: "center"
          },
          {
            title: "权益名称",
            key: "ActivityName",
            align: "center"
          },
          {
            title: "活动说明",
            key: "ActivityExplain",
            align: "center"
          },
          {
            title: "活动图片",
            key: "ActivityImage",
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
                  "Button", {
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
                  "Button", {
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
          },
          {
            title: "操作日志",
            key: "OperateLog",
            align: "center",
            render: (h, params) => {
              return h("div", [
                h(
                  "Button", {
                    on: {
                      click: () => {
                        this.WatchHistroy(params.row);
                      }
                    }
                  },
                  "查看"
                )
              ]);
            }
          }
        ],
        logTableCol: [{
            type: "index",
            width: 40,
            align: "center"
          },
          {
            title: "操作人",
            key: "Operator",
            align: "center"
          },
          {
            title: "操作类型",
            key: "OperationType",
            align: "center"
          },
          {
            title: "操作时间",
            key: "CreateDateTime",
            align: "center",
            render: (h, params) => {
              return h('div', this.formateDataTime(params.row.CreateDateTime));
            }
          },
          {
            title: "操作",
            key: "Remarks",
            align: "center"
          },
          {
            title: "操作日志",
            key: "OperateLog",
            align: "center",
            width: 90,
            render: (h, params) => {
              return h(
                "Button", {
                  on: {
                    click: () => {
                      this.WatchLogDetail(params.row.OldValue, params.row.NewValue)
                    }
                  }
                },
                "查看"
              )
            }
          }
        ],
        selectedRows: [],
        services: [],
        packages: [],
        searchData: {
          servicePid: ""
        },
        pager: {
          pageIndex: 1,
          pageSize: 20,
          totalPage: 0,
          totalCount: 0
        },
        model: {},
        loading: false,
        searchResult: [],
        addOrEditDialog: {
          show: false,
          type: ""
        },
        showLog: {
          show: false,
          logs: [],
          detail: {
            show: false,
            oldValue: "",
            newValue: "",
            logProperty: []
          }
        },
        multEditModel: [],
        excelFile: null,
        imgFile: null,
        uploadExcelStatus: false,
        uploadImgStatus: false
      };
    },
    created: function () {
      this.$Message.config({
        top: 50,
        duration: 5
      });
      this.GetAllPaintDiscountService();
      this.GetAllPackageConfig();
      this.Search(1);
    },
    watch: {
      "pager.pageIndex": function () {
        var vm = this;
        vm.Search(vm.pager.pageIndex);
      }
    },
    methods: {
      GetAllPaintDiscountService: function () {
        var vm = this;
        util.ajax
          .post("/PaintDiscountConfig/GetAllPaintDiscountService")
          .then(function (result) {
            result = result.data;
            if (result.Status) {
              vm.services = result.Data;
            }
          });
      },
      GetAllPackageConfig: function () {
        var vm = this;
        util.ajax
          .post("/PaintDiscountConfig/GetAllPaintDiscountPackage")
          .then(function (result) {
            result = result.data;
            vm.loading = false;
            if (result.Status) {
              vm.packages = result.Data;
            }
          });
      },
      Search: function (pageindex) {
        var vm = this;
        vm.loading = true;
        vm.pager.pageIndex = pageindex;
        vm.multEditModel = [];
        util.ajax
          .post("/PaintDiscountConfig/SelectPaintConfig", {
            packageId: vm.packageId,
            servicePid: vm.searchData.servicePid,
            pageIndex: vm.pager.pageIndex,
            pageSize: vm.pager.pageSize
          })
          .then(function (result) {
            if (result.data.Status) {
              vm.searchResult = result.data.Data;
              vm.pager.totalCount = result.data.TotalCount;
            } else {
              vm.$Message.error({
                content: "查询失败!" + (result.data.Msg || ""),
                duration: 10,
                closable: true
              });
              vm.searchResult = [];
            }
            vm.loading = false;
          });
      },
      OpenAddDialog: function (item) {
        var vm = this;
        vm.imgFile = null;
        vm.model = {
          PackageId: vm.packageId,
          PKID: 0,
          ServicePid: "",
          ServiceName: "",
          SurfaceCount: 0,
          ActivityPrice: 0,
          ActivityName: "",
          ActivityExplain: "",
          ActivityImage: ""
        };
        vm.addOrEditDialog.type = "Add";
        vm.addOrEditDialog.show = true;
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
        vm.model = {
          PackageId: vm.packageId,
          PKID: 0,
          ServicePid: "",
          ServiceName: "",
          SurfaceCount: 0,
          ActivityPrice: 0,
          ActivityName: "",
          ActivityExplain: "",
          ActivityImage: ""
        };
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
      // 刷新服务缓存
      RemoveCache: function (servicePid) {
        var vm = this;
        if (vm.packageId < 1) {
          vm.$Message.warning("请选择价格体系");
          return;
        }
        util.ajax
          .post("/PaintDiscountConfig/RemovePaintDiscountDetailCache", {
            packageId: vm.packageId,
            servicePid: servicePid
          })
          .then(function (result) {
            result = (result || []).data || [];
            if (result && result.Status) {
              vm.$Message.success("刷新服务价格缓存成功!");
            } else {
              vm.$Message.error({
                content: "刷新服务价格缓存失败",
                duration: 10,
                closable: true
              });
            }
          });
      },
      // 查看操作记录
      WatchHistroy: function (item) {
        var vm = this;
        if (item && item.ServicePid && item.SurfaceCount) {
          util.ajax
            .post("/PaintDiscountConfig/GetPaintDiscountOprLog", {
              logType: "PaintDiscountConfig",
              packageId: vm.packageId,
              servicePid: item.ServicePid,
              surfaceCount: item.SurfaceCount
            })
            .then(function (result) {
              result = (result || []).data || [];
              if (result && result.Status) {
                vm.showLog.logs = result.Data;
                vm.showLog.show = true;
              }
            });
        }
      },
      // 查看单条日志详情
      WatchLogDetail: function (oldValue, newValue) {
        var vm = this;
        var oldObj = /^\s*$/.test(oldValue) ? null : JSON.parse(oldValue);
        var newObj = /^\s*$/.test(newValue) ? null : JSON.parse(newValue);
        var obj = oldObj || newObj;
        var keys = [];
        for (var property in obj) {
          if (obj.hasOwnProperty(property)) {
            keys[keys.length] = property;
          }
        }
        vm.showLog.detail.oldValue = oldObj;
        vm.showLog.detail.newValue = newObj;
        vm.showLog.detail.logProperty = keys;
        vm.showLog.detail.show = true;
      },
      // 添加或编辑是数据校验
      CheckPaintConfig: function (model) {
        var vm = this;
        var errorMessage = "";
        if (!model) {
          errorMessage = "未知的对象";
        } else if (!parseInt(model.PackageId) || model.PackageId < 1) {
          errorMessage = "请选择价格体系";
        } else if (!model.ServicePid) {
          errorMessage = "请选择产品";
        } else if (!parseInt(model.SurfaceCount) || model.SurfaceCount < 0) {
          errorMessage = "请选择面数";
        } else if (!parseFloat(model.ActivityPrice) || model.ActivityPrice < 0) {
          errorMessage = "活动价格须大于0";
        } else if (!model.ActivityName) {
          errorMessage = "权益名称不能为空";
        } else if (!model.ActivityImage) {
          errorMessage = "活动图片不能为空";
        }
        if (errorMessage) {
          vm.$Message.warning(errorMessage);
          return false;
        } else {
          return true;
        }
      },
      // 添加配置
      Add: function () {
        var vm = this;
        var model = vm.model;
        if (!model.ServicePid) {
          vm.$Message.warning("请选择产品");
          return;
        }
        if (!(model.SurfaceCount > 0)) {
          vm.$Message.warning("请选择面数");
          return;
        }
        if (!vm.CheckPaintConfig(model)) {
          return;
        }
        if (!confirm(
            "是否确认添加 " +
            model.ServicePid +
            "下" +
            model.SurfaceCount +
            " 的配置 ? "
          )) {
          return;
        }
        util.ajax.post("/PaintDiscountConfig/AddPaintConfig", {
          model: model
        }).then(function (result) {
          result = (result || []).data || [];
          if (result && result.Status) {
            vm.$Message.success("添加成功!" + (result.Msg || ""));
            vm.CloseAddOrEditDialog();
            setTimeout(function () {
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
      // 编辑配置
      Edit: function () {
        var vm = this;
        var model = vm.model;
        if (!confirm(
            "是否确认更新 " +
            model.ServicePid +
            "的" +
            model.SurfaceCount +
            " 配置 ? "
          )) {
          return;
        }
        if (!vm.CheckPaintConfig(model)) {
          return;
        }
        util.ajax
          .post("/PaintDiscountConfig/UpdatePaintConfig", {
            model: model
          })
          .then(function (result) {
            result = (result || []).data || [];
            if (result && result.Status) {
              vm.$Message.success("编辑成功!");
              vm.CloseAddOrEditDialog();
              setTimeout(function () {
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
      // 批量编辑图片
      MultEdit: function () {
        var vm = this;
        if (vm.multEditModel && vm.multEditModel.length > 0) {
          var models = vm.multEditModel
            .filter(function (x) {
              return Boolean(x.ServicePid) && x.SurfaceCount > 0;
            })
            .map(function (x) {
              return x;
            }); // 确保servicePid和surfaceCount有效
          util.ajax
            .post("/PaintDiscountConfig/MultUpdatePaintConfig", {
              models: models,
              activityImage: vm.model.ActivityImage
            })
            .then(function (result) {
              result = (result || []).data || [];
              if (result && result.Status) {
                vm.$Message.success("批量编辑成功!");
                setTimeout(function () {
                  vm.Search(vm.pager.pageIndex);
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
        if (!item || !item.PackageId || !item.ServicePid || item.SurfaceCount < 1) {
          return;
        }
        if (!confirm(
            "确认删除PID： " +
            item.ServicePid +
            ",面数:" +
            item.SurfaceCount +
            " 的配置?"
          )) {
          return;
        }
        util.ajax
          .post("/PaintDiscountConfig/DeletePaintConfig", {
            packageId: item.PackageId,
            servicePid: item.ServicePid,
            surfaceCount: item.SurfaceCount
          })
          .then(function (result) {
            result = (result || []).data || [];
            if (result && result.Status) {
              vm.$Message.success("删除成功!");
              setTimeout(function () {
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
      // 手动上传模式
      HandleExcelUpload (excelFile) {
        var vm = this;
        vm.excelFile = excelFile;
        return false;
      },
      // 上传Excel文件
      ExcelUpload () {
        var vm = this;
        vm.uploadExcelStatus = true;
        var formData = new FormData();
        formData.append("file", vm.excelFile);
        formData.append("packageId", vm.packageId);
        util.ajax({
            url: "/PaintDiscountConfig/UploadExcel",
            method: "post",
            data: formData
          })
          .then(function (result) {
            result = (result || []).data || [];
            vm.uploadExcelStatus = false;
            vm.excelFile = null;
            if (result.Status) {
              setTimeout(function () {
                vm.Search(vm.pager.pageIndex);
              }, 2000);
              vm.$Message.success("上传成功!");
            } else {
              vm.$Message.error({
                content: "上传失败!" + (result.Msg || ""),
                duration: 10,
                closable: true
              });
            }
          })
          .catch(function (result) {
            result = (result || []).data || [];
            vm.uploadExcelStatus = false;
            vm.$Message.error({
              content: "上传失败!" + (result.Msg || ""),
              duration: 10,
              closable: true
            });
          });
      },
      // 上传模式图片
      ImgUpload: function (imgFile) {
        var vm = this;
        vm.uploadImgStatus = true;
        vm.imgFile = imgFile;
        var formData = new FormData();
        formData.append("imgFile", vm.imgFile);
        util
          .ajax({
            url: "/Article/AddArticleImg2",
            method: "post",
            data: formData
          })
          .then(function (result) {
            result = (result || []).data || [];
            vm.uploadImgStatus = false;
            vm.model.ActivityImage = result.SImage;
          })
          .catch(function (result) {
            result = (result || []).data || [];
            vm.uploadImgStatus = false;
            vm.$Message.error({
              content: "上传图片失败!" + (result.Msg || ""),
              duration: 10,
              closable: true
            });
          });
        return false; // 阻止自动上传以避免重复上传
      },
      ExportExcel: function () {
        var vm = this;
        if (!parseInt(vm.packageId) || vm.packageId < 1) {
          vm.$Message.warning("请先选择价格体系");
          return;
        }
        if (!confirm("确认导出" + vm.packageName + "的服务价格数据?")) {
          return;
        }
        window.location.href = "/PaintDiscountConfig/ExportExcel?packageId=" + vm.packageId;
      },
      HandleSelectedRows: function (selections) {
        this.multEditModel = selections;
      },
      formateDataTime: function (longtime) {
        if (longtime) {
          var time = new Date(
            parseInt(longtime.replace("/Date(", "").replace(")/", ""))
          );
          var year = time.getFullYear();
          var day = time.getDate();
          var month = time.getMonth() + 1;
          var hours = time.getHours();
          var minutes = time.getMinutes();
          var seconds = time.getSeconds();
          return (
            year +
            "-" +
            month +
            "-" +
            day +
            " " +
            hours +
            ":" +
            minutes +
            ":" +
            seconds
          );
        }
      }
    }
  };

</script>

<style scoped>
[v-cloak] {
  display: none;
}
</style>
