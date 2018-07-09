<style>
.ivu-table .table-module-row td {
  background-color: #fcfcfd;
  /* color: #fff; */
}
/* .ivu-table .table-module-content-row td{
    background-color: #e5f2fa;
    color: #fff;
} */
</style>
<template>
  <div>
    
    <Input v-model="userArea" placeholder="请输入区域信息"  style="width: 200px"></Input>
    <Button type="info" @click="searchUserInfo">查询</Button>
    <Button type="success" @click="activityModalShow">添加报名信息</Button>
    <div style="margin-top:5px;">
      <Table :row-class-name="rowClassName" border strip :loading="tableModal.loading" :columns="tableModal.columns" :data="tableModal.list" size="small" no-data-text="暂无数据"></Table>
      <Page :total="dataCount" :page-size="pageSize" :current="pageIndex" show-total show-sizer  @on-change="changepageIndex" @on-page-size-change="changePageSize" ></Page>
    </div>
    <Modal v-model="activityModal.visible" title="编辑报名信息" :transfer="false" width="40%">
      <div slot="footer">
        <Button type="text" size="large" @click="activityModalCancel('activityModal.ActivityModule')">取消</Button>
        <Button type="primary" size="large" :loading="activityModal.loading" @click="activityModalOk('activityModal.ActivityModule')">提交</Button>
      </div>
      <Form ref="activityModal.ActivityModule" :model="activityModal.ActivityModule" :rules="activityModal.rules" :label-width="80">
        <FormItem label="姓名" prop="UserName" >
          <Input v-model="activityModal.ActivityModule.UserName"  />
        </FormItem>
        <FormItem label="手机号" prop="Tel">
          <Input v-model="activityModal.ActivityModule.Tel" placeholder="手机号" />
        </FormItem>
        <FormItem label="区域信息" prop="Area">
          <Input v-model="activityModal.ActivityModule.Area" placeholder="区域信息" />
        </FormItem>
        <FormItem label="报名状态" prop="EnrollStatus">
         <Select v-model="activityModal.ActivityModule.EnrollStatus" style="width:30%">
            <Option v-for="item in moduleTypeList" :value="item.key" :key="item.key" >{{item.value}}</Option>
          </Select>
        </FormItem>       
      </Form>
    </Modal>  
  </div>
</template>
<script>
import util from "@/framework/libs/util";
export default {
  name: "Activity",
  props: {
    pageCode: {
      type: String,
      default: "activity"
    }
  },
  data () {
    return {
      dataCount: 0,
      pageSize: 20,  
      pageIndex: 1, 
      tableModal: {
        list: [],
        loading: true,
        columns: [
          {
            title: "姓名",
            key: "UserName",
            width: 120
          },
          {
            title: "手机号",
            key: "Tel",
            width: 100
          },
          {
            title: "区域信息",
            key: "Area",
            width: 200
          },
          {
            title: "报名状态",
            key: "EnrollStatus",
            width: 100,
            render: function (h, params) {
              switch (params.row.EnrollStatus) {
                case 0:
                  return h("span", "未审核");
                default:
                  return h("span", "已通过");
              }
            }
          },
          {
            title: "创建时间",
            key: "CreatTime",
            width: 180,
            render: (h, params) => {
              return h('div', [
                    h('span', this.FormatToDate(params.row.CreatTime))
                            ]);
            }
          },
          {
            title: "修改时间",
            key: "UpdateTime",
            width: 180,
            render: (h, params) => {
              return h('div', [
                    h('span', this.FormatToDate(params.row.UpdateTime))
                            ]);
            }
          },
          {
            title: "参加的活动",
            key: "ActivityName",
            width: 188
          },
          {
            title: "操作",
            key: "action",
            width: 200,
            fixed: "right",
            render: (h, params) => {
              let buttons = [];
              buttons.push(
                h(
                  "Button",
                  {
                    props: {
                      type: "primary",
                      size: "small"
                    },
                    style: {
                      marginRight: "3px"
                    },
                    on: {
                      click: () => {
                        this.getActivityModuleInfo(params.row.Tel);
                      }
                    }
                  },
                  "编辑"
                )
              );
              return h("div", buttons);
            }
          }
        ]
      },
      moduleTypeList: [
        {
          key: 0,
          value: "未审核"
        },
        {
          key: 1,
          value: "已通过"
        }
      ],
      activityModal: {
        visible: false,
        loading: false,
        edit: false,
        ActivityModule: {},
        rules: {
          UserName: [
            { required: true, message: "姓名不能为空", trigger: "blur" }
          ],
          Tel: [{ required: true, message: "手机号不能为空", trigger: "blur" }],
          Area: [
            { required: true, message: "区域信息不能为空", trigger: "blur" }
          ]
        }
      },
      userArea: " "
     };
  },
  created () {
    this.init();
  },
  methods: {
    FormatToDate (timestamp) {
            var time = new Date(parseInt(timestamp.replace("/Date(", "").replace(")/", ""), 10));
            var year = time.getFullYear();
            var month = time.getMonth() + 1 < 10 ? "0" + (time.getMonth() + 1) : time.getMonth() + 1;
            var date = time.getDate() < 10 ? "0" + time.getDate() : time.getDate();
            var hour = time.getHours() < 10 ? "0" + time.getHours() : time.getHours();
            var minute = time.getMinutes() < 10 ? "0" + time.getMinutes() : time.getMinutes();
            var second = time.getSeconds() < 10 ? "0" + time.getSeconds() : time.getSeconds();
            var YmdHis = year + '/' + month + '/' + date + ' ' + hour + ':' + minute + ':' + second;
            return YmdHis;
        },
    init () {
      this.tableModal.loading = false;
      util.ajax
        .get(
          `/NewActivity/GetActivityPageList?area=${this.userArea.trim()}&pageIndex=${this.pageIndex}&pageSize=${this.pageSize}`)
        .then(response => {    
          this.tableModal.list = response.data.data; 
          this.dataCount = response.data.totalCount; 
          this.pageSize = response.data.pageSize; 
          this.pageIndex = response.data.currentPage; 
        });
    },
    GetDateFormat (str) {
      return new Date(parseInt(str.substr(6, 13))).toLocaleDateString(); 
    },
    changePageSize (value) {
      this.pageSize = value;
      this.init();
    },
    changepageIndex (value) {
      this.pageIndex = value;
     this.init();
    },
    searchUserInfo () {
      this.tableModal.loading = false;
      util.ajax
        .get(
          `/NewActivity/GetActivityPageList?area=` +
            this.userArea.trim() + ``
        )
        .then(response => {
          this.tableModal.list = response.data.data; 
          this.dataCount = response.data.totalCount; 
          this.pageSize = response.data.pageSize; 
          this.pageIndex = response.data.currentPage; 
        });
    },
    rowClassName (row, index) {
      if (row.ModuleType === 0) {
        return "table-module-content-row";
      } else {
        return "table-module-row";
      }
    },
    getActivityModuleInfo (tel) {
      this.$refs["activityModal.ActivityModule"].resetFields();
      util.ajax.get(`/NewActivity/GetEnrollModel?tel=${tel}`).then(response => {
        if (response.data) {
          this.activityModal.loading = false;
          this.activityModal.ActivityModule = response.data.data;
          this.activityModal.visible = true;
        }
      });
    },
    activityModalShow () {
      this.$refs["activityModal.ActivityModule"].resetFields();
      this.activityModal.ActivityModule = {
        Id: 0,
        UserName: "",
        Tel: "",
        Area: "",
        EnrollStatus: 0
      };
      this.activityModal.visible = true;
    },
    activityModalOk (name) {
      this.activityModal.loading = true;
      this.$refs[name].validate(valid => {
        if (valid) {
          if (this.activityModal.ActivityModule.Id === 0) {
            util.ajax
              .post(
                `/NewActivity/AddEnrollInfo`,
                this.activityModal.ActivityModule
              )
              .then(response => {
                if (response.data) {
                  this.init();
                  this.$Message.success("操作成功");
                }
              });
          } else {
            util.ajax
              .post(
                `/NewActivity/UpdateEnrollInfo`,
                this.activityModal.ActivityModule
              )
              .then(response => {
                if (response.data) {
                  this.init();
                  this.$Message.success("操作成功");
                }
              });
          }
          this.activityModal.loading = false;
          this.activityModal.visible = false;
        } else {
          this.activityModal.loading = false;
        }
      });
    },
    activityModalCancel (name) {
      this.$refs[name].resetFields();
      this.activityModal.visible = false;
    },
    deleteActivityModule (contentId, moduleId, moduleType) {
      var confirmContent = "确认要删除模块及其所有内容吗？";
      if (moduleType === 0) {
        confirmContent = "确定要删除吗？";
      }
      util.modal.confirm({
        title: "警告",
        content: confirmContent,
        onOk: () => {
          util.ajax
            .post(
              `/MemberPage/DeleteActivityModule?contentId=${contentId}&moduleId=${moduleId}&moduleType=${moduleType}`
            )
            .then(response => {
              if (response.data) {
                this.init();
                this.$Message.success("操作成功");
              } else {
                this.$Message.success("操作失败");
              }
            });
        }
      });
    }
  }
};
</script>
