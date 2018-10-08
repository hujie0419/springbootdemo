<template>
  <div>
    <h1 class="title">用户报名配置</h1>
    <label>所在区域：</label>
    <Input v-model="filters.area" style="width: 200px"></Input>
    <Button type="success" icon="search" @click="loadTable(1)">搜索</Button>
    <Button type="success" icon="plus" @click="openInsertModal()" style="float: right">新增报名</Button>
    <div style="clear: both; padding-top: 18px">
      <Table :loading="table.loading" :data="table.data" :columns="table.columns" style="width: 100%" stripe></Table>
      <div style="margin: 10px; overflow: hidden">
        <div style="float: right">
          <Page show-total :total="page.total" :page-size="page.pageSize" :current="page.current" :page-size-opts="[10, 30, 50]" show-sizer @on-change="handlePageChange" @on-page-size-change="handlePageSizeChange"></Page>
        </div>
      </div>
    </div>
    <Modal v-model="insertmodal.visible" :loading="insertmodal.loading" title="新增报名" okText="保存"  cancelText="取消" @on-ok="addUserRegistration"  width="50%" :mask-closable="false">
      <Form ref="insertmodal.userRegistration" :model="insertmodal.userRegistration" :label-width="90">       
        <FormItem label="用户姓名" >
          <Input v-model="insertmodal.userRegistration.Name" />
        </FormItem>
        <FormItem label="手机号" >
          <Input v-model="insertmodal.userRegistration.Phone" />
        </FormItem>
        <FormItem label="所在区域" >
          <Input v-model="insertmodal.userRegistration.Area" />
        </FormItem>
        <FormItem label="活动名称">
          <Select v-model="insertmodal.userRegistration.ActivityId">
            <Option v-for="item in selectList" :key="item.value" :value="item.value">{{ item.label }}</Option>
          </Select>
        </FormItem>
      </Form>
    </Modal>
    <Modal v-model="updatemodal.visible" :loading="updatemodal.loading" title="修改报名" okText="保存"  cancelText="取消" @on-ok="modifyUserRegistration"  width="50%" :mask-closable="false">
      <Form ref="updatemodal.userRegistration" :model="updatemodal.userRegistration" :label-width="90">       
        <FormItem label="用户姓名" >
          <Input v-model="updatemodal.userRegistration.Name" />
        </FormItem>
        <FormItem label="手机号" >
          <Input v-model="updatemodal.userRegistration.Phone" />
        </FormItem>
        <FormItem label="所在区域" >
          <Input v-model="updatemodal.userRegistration.Area" />
        </FormItem>
        <FormItem label="活动名称">
          <Select v-model="updatemodal.userRegistration.ActivityId">
            <Option v-for="item in selectList" :key="item.value" :value="item.value">{{ item.label }}</Option>
          </Select>
        </FormItem>
      </Form>
    </Modal>
  </div>
</template>
<script>
  import moment from "moment"

  export default {
    data () {
      return {
        page: {
          total: 0,
          current: 1,
          pageSize: 10
        },
        filters: {
          area: ""
        },
        table: {
          loading: true,
          data: [],
          columns: [
            {
              title: "#",
              width: 50,
              align: "center",
              fixed: "left",
              type: "index",
              key: "PKID"
            },
            {
              title: "用户姓名",
              align: "center",
              key: "Name"             
            },
            {
              title: "手机号",
              align: "center",
              key: "Phone"
            },
            {
              title: "所在区域",
              align: "center",
              key: "Area"             
            },
            {
              title: "活动名称",
              align: "center",
              key: "ActivityName"             
            },
            {
              title: "报名状态",
              align: "center",
              key: "Status",
              render: (h, params) => {
                return h(
                  "span",
                  this.convertStatus(params.row.Status)
                );
              }
            },
            {
              title: "报名时间",
              align: "center",
              key: "CreateDateTime",
              render: (h, params) => {
                return h(
                  "span",
                  this.formatDate(params.row.CreateDateTime)
                );
              }
            },
            {
              title: "操作",
              key: "action",
              align: "center",
              render: (h, params) => {
                return h("div", [
                  h(
                    "Button",
                    {
                      props: {
                        type: "primary",
                        size: "small"
                      },
                      on: {
                        click: () => {
                          this.openUpdateModal(
                            params.row.PKID
                          );
                        }
                      }
                    },
                    "修改"
                  )
                ]);
              }
            }
          ]
        },
        selectList: [],
        insertmodal: {
          visible: false,
          loading: true,
          userRegistration: {
            Name: "",
            Phone: "",
            Area: "",
            ActivityId: 0
          }
        },
        updatemodal: {
          visible: false,
          loading: true,
          userRegistration: {
            PKID: 0,
            Name: "",
            Phone: "",
            Area: "",
            ActivityId: 0
          }
        }
      }
    },
    created: function () {
      this.$Message.config({
        top: 50,
        duration: 3
      });
      this.loadTable(1);
      this.loadSelectList();
    },
    methods: {
      loadTable (pageIndex) {
        this.page.current = pageIndex;
        this.table.loading = true;
        var requestData = "?area=" + this.filters.area;      
        requestData += "&pageIndex=" + this.page.current;
        requestData += "&pageSize=" + this.page.pageSize;
        this.ajax
          .get("/UserActivity/GetPagedUserRegistration" + requestData)
          .then(response => {
            var data = response.data;
            this.page.total = data.totalCount;
            this.table.data = data.data;
            this.table.loading = false;
          });
      },
      loadSelectList () {
        this.ajax
          .get("/UserActivity/GetUserActivities")
          .then(response => {
            this.selectList = response.data;
          });
      },
      openInsertModal () {
        this.insertmodal.visible = true;
        this.insertmodal.userRegistration = {
            Name: "",
            Phone: "",
            Area: "",
            ActivityId: 0
        }
      },
      openUpdateModal (pkid) {
        this.ajax
        .get("/UserActivity/GetUserRegistration?pkid=" + pkid)
        .then(response => {
          this.updatemodal.userRegistration = response.data;
          this.updatemodal.visible = true;
        });
      },
      modalValidation (modal) {
        modal.loading = true;

        if (!modal.userRegistration.Name) {
          this.$Message.warning("请输入用户姓名");
          this.$nextTick(() => {
            modal.loading = true;
          });
          modal.loading = false;
          return false;
        }

        var phone = modal.userRegistration.Phone;
        if (!phone) {
          this.$Message.warning("请输入手机号");
          this.$nextTick(() => {
            modal.loading = true;
          });
          modal.loading = false;
          return false;
        } else if (!(/^1[34578]\d{9}$/.test(phone))) {
          this.$Message.warning("手机号格式有误");
          this.$nextTick(() => {
            modal.loading = true;
          });
          modal.loading = false;
          return false;
        }

        if (!modal.userRegistration.Area) {
          this.$Message.warning("请输入所在区域");
          this.$nextTick(() => {
            modal.loading = true;
          });
          modal.loading = false;
          return false;
        }

        if (modal.userRegistration.ActivityId === 0) {
          this.$Message.warning("请选择活动名称");
          this.$nextTick(() => {
            modal.loading = true;
          });
          modal.loading = false;
          return false;
        }
        return true;
      },
      addUserRegistration () {
        var modal = this.insertmodal;
        if (this.modalValidation(modal)) {
          this.ajax
            .post("/UserActivity/AddUserRegistration", {
                request: modal.userRegistration
            })
            .then(response => {
                if (!response.data) {
                  this.$Message.success("保存成功！");
                  modal.visible = false;
                  this.loadTable(this.page.current);
                } else {
                  this.$Message.error(response.data);
                  this.$nextTick(() => {
                    modal.loading = true;
                  });
                }
                modal.loading = false;
            });          
        }
      },
      modifyUserRegistration () {
        var modal = this.updatemodal;
        if (this.modalValidation(modal)) {
          this.ajax
            .post("/UserActivity/ModifyUserRegistration", {
                request: modal.userRegistration
            })
            .then(response => {
                if (!response.data) {
                  this.$Message.success("保存成功！");
                  modal.visible = false;
                  this.loadTable(this.page.current);
                } else {
                  this.$Message.error(response.data);
                  this.$nextTick(() => {
                    modal.loading = true;
                  });
                }
                modal.loading = false;
            });          
        }
      },
      handlePageChange (pageIndex) {
        this.loadTable(pageIndex);
      },
      handlePageSizeChange (pageSize) {
        this.page.pageSize = pageSize;
        this.loadTable(this.page.current);
      },
      formatDate (date) {
        return moment(date).format('YYYY-MM-DD HH:mm')
      },
      convertStatus (status) {
        if (status === 1) {
          return '通过';
        } else if (status === 2) {
          return '失败';
        } else {
          return '等待';
        }
      }
    }
  }
</script>
