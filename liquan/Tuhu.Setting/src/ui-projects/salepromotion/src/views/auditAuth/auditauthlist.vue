<template>
  <div>
    <h1 class="title">促销活动审核权限配置</h1>
    <Form :model="search_data" :label-width="100">
      <Row>
        <Col span="6">
        <FormItem label="促销类型">
          <Select v-model="search_data.PromotionType" style="width:130px">
            <Option v-for="item in PromotionTypeList" :value="item.value" :key="item.value">{{ item.label
              }}</Option>
          </Select>
        </FormItem>
        </Col>
        <Col span="11">
        <Button type="primary" icon="search" @click="page.current=1;loadData()" style="margin-left:28px;">查询</Button>
        <Button type="success" icon="plus" style="margin-left: 128px;" @click="addModal.visible=true">新增</Button>
        </Col>
      </Row>
      <Row>

      </Row>
    </Form>

    <Table stripe border :loading="table.loading" :columns="table.columns" :data="table.data"></Table>
    <div style="margin-top:15px;float:right">
      <Page :total="page.total" show-total :page-size="page.pageSize" :current="page.current" :page-size-opts="[10,20 ,50 ,100]"
        show-elevator show-sizer @on-change="handlePageChange" @on-page-size-change="handlePageSizeChange"></Page>
    </div>
    <Modal v-model="addModal.visible" title="新增审核权限" :loading="addModal.loading" width="30px" @on-ok="addAuditAuth"
      @on-cancel="cancel">
      <Form ref="authModel" :rules="addModal.rules" :model="addModal.authModel" :label-width="100">
        <Row>
          <Col span="6">
          <FormItem label="促销类型">
            <Select v-model="addModal.authModel.PromotionType" style="width:130px">
              <Option v-for="item in addModal.promotionTypeList" :value="item.value" :key="item.value">{{ item.label
                }}</Option>
            </Select>
          </FormItem>
          </Col>
        </Row>
        <Row>
          <Col>
          <FormItem label="选择角色">
            <Select v-model="addModal.authModel.RoleType" style="width:130px">
              <Option v-for="item in addModal.roleTypeList" :value="item.value" :key="item.value">{{ item.label
                }}</Option>
            </Select>
          </FormItem>
          </Col>
        </Row>
        <Row>
          <Col>
          <FormItem label="用户邮箱" prop="UserName" style="width:320px">
            <Input v-model="addModal.authModel.UserName" :maxlength=30 placeholder="请输入用户邮箱"></Input>
          </FormItem>
          </Col>
        </Row>
      </Form>
    </Modal>

    <Modal v-model="removeModal.visible" title="删除审核权限" :loading="removeModal.loading" width="20px" @on-ok="removeAuditAuth">
      <h3>确认删除该条审核权限?</h3>
    </Modal>
    <Modal v-model="logModal.visible" title="操作日志" :loading="logModal.loading" width="800px" footerHide>
      <div>
        <div>
          <Table stripe border :loading="logModal.table.loading" :columns="logModal.table.columns" :data="logModal.table.data"></Table>
        </div>
        <!-- <Row>
          <div style="margin-top:15px;float:right;">
            <Page show-total :total="logModal.page.total" :page-size="logModal.page.pageSize" :current="logModal.page.current"
              :page-size-opts="[10,20 ,50 ,100]" show-elevator show-sizer @on-change="handleLogPageChange"
              @on-page-size-change="handleLogPageSizeChange">
            </Page>
          </div>
        </Row> -->
      </div>
    </Modal>

  </div>
</template>
<script>
  const defaultAuth = {
    PromotionType: "1",
    RoleType: "Manager",
    UserName: ""
  };
 const validEmail = (rule, value, callback) => {
   const regex = /^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$/;
      const rsCheck = regex.test(value);
      if (!rsCheck) {
        return callback(new Error("请输入正确的邮箱格式"));
      } else {
        callback();
      }
  };
  export default {
    data () {
      return {
        logModal: {
          visible: false,
          loading: true,
          AuthId: '',
          page: {
            total: 0,
            current: 1,
            pageSize: 20
          },
          table: {
            loading: false,
            data: [],
            columns: [{
                title: "操作",
                key: "OperationLogDescription",
                align: "center"
              },
              {
                title: "操作时间",
                key: "CreateDateTime",
                align: "center"
              },
              {
                title: "操作人",
                key: "CreateUserName",
                align: "center"
              }
            ]
          }
        },
        PromotionTypeList: [{
            value: "0",
            label: "请选择"
          },
          {
            value: "1",
            label: "打折"
          }
        ],
        addModal: {
          authModel: {
            PromotionType: "1",
            RoleType: "Manager",
            UserName: ""
          },
          visible: false,
          loading: true,
          rules: {
            UserName: [{
              required: true,
              message: "请输入用户邮箱",
              trigger: "blur"
            },
            {
              validator: validEmail,
              trigger: "blur"
            }
            ]
          },
          promotionTypeList: [{
            value: "1",
            label: "打折"
          }],
          roleTypeList: [{
              value: "Manager",
              label: "管理员"
            },
            {
              value: "SuperManager",
              label: "超级管理员"
            }
          ]
        },
        removeModal: {
          PKID: 0,
          visible: false,
          loading: false
        },
        search_data: {
          PromotionType: "0"
        },
        page: {
          total: 0,
          current: 1,
          pageSize: 20
        },

        table: {
          loading: false,
          data: [],
          columns: [{
              title: "促销类型",
              key: "PromotionType",
              align: "center",
              render: (h, params) => {
                if (params.row.PromotionType === 1) {
                  return h("div", "打折");
                }
              }
            },
            {
              title: "用户名",
              key: "UserName",
              align: "center"
            },
            {
              title: "角色",
              key: "RoleType",
              align: "center",
              render: (h, params) => {
                if (params.row.RoleType === 'Manager') {
                  return h("div", "管理员");
                } else if (params.row.RoleType === 'SuperManager') {
                  return h("div", "超级管理员");
                }
              }
            },
            {
              title: "创建时间",
              key: "CreateDateTime",
              align: "center"
            },
            {
              title: "创建用户",
              key: "CreateUserName",
              align: "center"
            },
            {
              title: '操作',
              key: 'action',
              width: 150,
              align: 'center',
              render: (h, params) => {
                return h('div', [
                  h(
                    "Button", {
                      props: {
                        type: "error",
                        size: "small"
                      },
                      style: {
                        marginRight: "7px"
                      },
                      on: {
                        click: () => {
                          this.removeModal.PKID = params.row.PKID;
                          this.removeModal.visible = true;
                        }
                      }
                    }, "删除")
                //   h(
                //     "Button", {
                //       props: {
                //         type: "info",
                //         size: "small"
                //       },
                //       on: {
                //         click: () => {
                //         this.logModal.AuthId = params.row.AuthId;
                //         this.logModal.visible = true;
                //         this.loadLog();
                //         }
                //       }
                //     }, "日志")
                ]);
              }
            }
          ]
        }
      };
    },
    mounted () {
      this.loadData();
    },
    methods: {
      loadData () {
        // 查询列表
        this.table.loading = true;
        this.ajax
          .post("/salepromotionactivity/SelectAuditAuthList", {
            searchModel: this.search_data,
            pageIndex: this.page.current,
            pageSize: this.page.pageSize
          })
          .then(response => {
            var data = response.data;
            if (data.Status) {
              this.page.total = data.Total;
              this.table.data = data.List;
              this.table.loading = false;
            } else {
              this.messageInfo(data.Msg);
            }
          });
      },
      cancel () {
        this.addModal.authModel = Object.assign({}, defaultAuth);
        this.$refs['authModel'].resetFields();
      },
      // 新增
      addAuditAuth () {
        // 验证
        this.addModal.loading = true;
        this.$refs['authModel'].validate((valid) => {
          if (valid) {
            this.ajax
              .post("/salepromotionactivity/InsertAuditAuth", {
                model: this.addModal.authModel
              })
              .then(res => {
                var data = res.data;
                if (data.Status) {
                  setTimeout(() => {
                    this.addModal.visible = false;
                    this.messageInfo("新增成功");
                    this.addModal.authModel = Object.assign({}, defaultAuth);
                    this.$refs['authModel'].resetFields();
                    this.loadData();
                    this.addModal.visible = false;
                    this.addModal.loading = false;
                    this.$nextTick(() => {
                      this.addModal.loading = true;
                    });
                  }, 2000);
                } else {
                  this.messageInfo(data.Msg);
                  this.addModal.loading = false;
                  this.$nextTick(() => {
                    this.addModal.loading = true;
                  });
                }
              });
          } else {
            this.addModal.loading = false;
            this.$nextTick(() => {
              this.addModal.loading = true;
            });
          }
        });
      },

      // 删除
      removeAuditAuth () {
        this.ajax
          .post("/salepromotionactivity/DeleteAuditAuth", {
            PKID: this.removeModal.PKID
          })
          .then(res => {
            this.removeModal.PKID = 0;
            var data = res.data;
            if (data.Status) {
              this.messageInfo("删除成功");
              this.loadData();
            } else {
              this.messageInfo(res.data.Msg);
            }
          });
      },
      handleResetForm () {
        this.search_data = {};
      },
      handlePageChange (pageIndex) {
        this.page.current = pageIndex;
        this.loadData();
      },
      handlePageSizeChange (pageSize) {
        this.page.pageSize = pageSize;
        this.loadData();
      },

      // log
      loadLog () {
        this.logModal.table.loading = true;
        this.ajax
          .post("/salepromotionactivity/GetOperationLogList", {
            activityId: this.logModal.AuthId,
            pageIndex: 1,
            pageSize: 30
          })
          .then(response => {
            this.logModal.table.loading = false;
            var data = response.data;
            this.logModal.table.data = data.List;
            this.logModal.page.total = data.Total;
          });
      },
      // 工具函数
      messageInfo (value) {
        this.$Message.info({
          content: value,
          duration: 3,
          closable: true
        });
      }
    }
  };

</script>
