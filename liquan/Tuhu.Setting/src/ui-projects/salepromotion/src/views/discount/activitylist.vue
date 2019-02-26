<template>
  <div>
    <h1 class="title">促销活动配置</h1>
    <Form :model="search_data" :label-width="100">
      <Row>
        <Col span="6">
        <FormItem label="活动名称">
          <Input v-model="search_data.Name" placeholder="请输入活动名称"></Input>
        </FormItem>
        </Col>
        <Col span="12">
        <FormItem label="活动时间">
          <Date-Picker v-model="search_data.StartTime" type="datetime" @on-change="search_data.StartTime=$event"
            placeholder="请选择起始时间" style="width: 200px"></Date-Picker>
          -
          <Date-Picker v-model="search_data.EndTime" type="datetime" @on-change="search_data.EndTime=$event"
            placeholder="请选择终止时间" style="width: 200px"></Date-Picker>
        </FormItem>
        </Col>
      </Row>
      <Row>
        <Col span="6">
        <FormItem label="商品pid">
          <Input v-model="search_data.Pid" placeholder="请输入商品pid"></Input>
        </FormItem>
        </Col>
        <Button type="primary" icon="search" @click="page.current=1;btnloadData()" style="margin-left:28px;">查询</Button>
        <Button type="ghost" icon="refresh" style="margin-left: 8px;" @click="handleResetForm">重置</Button>
      </Row>
      <Row style="margin-bottom:5px;">
        <Radio-group v-model="search_data.Status" @on-change="selectSattus" type="button">
          <Radio label="0">
            <span>全部
              <i>({{countModel.AllCount}})</i>
            </span>
          </Radio>
          <Radio label="1">
            <span>进行中
              <i>({{countModel.OnlineCount}})</i>
            </span>
          </Radio>
          <Radio label="2">
            <span>待审核
              <i>({{countModel.WaitAuditCount}})</i>
            </span>
          </Radio>
          <Radio label="3">
            <span>待上线
              <i>({{countModel.StayOnlineCount}})</i>
            </span>
          </Radio>
          <Radio label="4">
            <span>已拒绝
              <i>({{countModel.RejectedCount}})</i>
            </span>
          </Radio>
          <Radio label="5">
            <span>已结束
              <i>({{countModel.EndCount}})</i>
            </span>
          </Radio>

          <Button type="success" icon="plus" @click="addactivity" style="margin-left: 28px;">新增</Button>
        </Radio-group>
      </Row>
    </Form>
    <Table stripe border :loading="table.loading" :columns="table.columns" :data="table.data"></Table>
    <div style="margin-top:15px;float:right">
      <Page :total="page.total" show-total :page-size="page.pageSize" :current="page.current" :page-size-opts="[10,20 ,50 ,100]" show-elevator
        show-sizer @on-change="handlePageChange" @on-page-size-change="handlePageSizeChange"></Page>
    </div>

     <Modal v-model="unShelveModal.visible" title="下架活动" :loading="unShelveModal.loading" width="30px"
      @on-ok="unShelveActivity">
      <h3>确认下架该活动</h3>
      <br>
      <div style="color:red;">
          * 下架后该活动将会移至「已结束」状态下  您可重新编辑提交审核上架
      </div>
        <br>
    </Modal>
     <Modal v-model="rejectRemarkModal.visible" :loading="rejectRemarkModal.loading" width="30px" footerHide>
      <h3>拒绝原因</h3>
      <br>
          <Input v-model="rejectRemarkModal.remark" type="textarea" :rows="4" disabled></Input>
      <br> <br>
      <div>
        拒绝人: {{rejectRemarkModal.userName}}
      </div>
        <br>
    </Modal>
  </div>
</template>
<script>

export default {
  data () {
    return {
      vueData: {
        StartTimeOptions: {}, // 开始日期设置
        EndTimeOptions: {} // 结束日期设置
      },
      countModel: {
        AllCount: 0,
        OnlineCount: 0,
        WaitAuditCount: 0,
        StayOnlineCount: 0,
        RejectedCount: 0,
        EndCount: 0
      },
      search_data: {
        Name: "",
        Pid: "",
        Status: "0",
        StartTime: "", // 开始日期model
        EndTime: "" // 结束日期model
      },
      page: {
        total: 0,
        current: 1,
        pageSize: 20
      },
      rejectRemarkModal: {
          remark: '',
          userName: '',
          visible: false,
          loading: false
        },
        unShelveModal: {
          activityId: '',
          visible: false,
          loading: false
        },
      table: {
        loading: false,
        data: [],
        columns: [
            {
            title: "促销活动Id",
            key: "ActivityId",
            align: "center",
            width: 125
          },
          {
            title: "活动名称",
            key: "Name",
            align: "center"
          },
          {
            title: "促销方式",
            key: "DiscountMethod",
            align: "center",
            width: 85,
             render: (h, params) => {
              if (params.row.DiscountMethod === 1) {
                return h("div", "满额折");
              } else {
                  if (params.row.DiscountMethod === 2) {
                return h("div", "满件折");
              }
              }
            }
          },
          {
            title: "促销语",
            key: "Banner",
            align: "center"
          },
          {
            title: "活动状态",
            key: "Status",
            align: "center",
            width: 90,
            render: (h, params) => {
              if (params.row.Status === 1) {
                return h("div", "进行中");
              } else if (params.row.Status === 2) {
                return h("div", "待审核");
              } else if (params.row.Status === 3) {
                return h("div", "待上线");
              } else if (params.row.Status === 4) {
                return h("div", "已拒绝");
              } else if (params.row.Status === 5) {
                return h("div", "已结束");
              } else {
                return h("div", "未提交");
              }
            }
          },
          {
            title: "开始时间",
            key: "StartTime",
            align: "center",
            width: 95
          },
          {
            title: "结束时间",
            key: "EndTime",
            align: "center",
            width: 95
          },
          {
            title: "创建人",
            key: "CreateUserName",
            align: "center"
          },
          {
            title: "审核状态",
            key: "AuditStatus",
            align: "center",
             width: 90,
            render: (h, params) => {
                if (params.row.Status === 5 && params.row.AuditStatus !== 3) {
                     return h("div", "无");
                } else {
                     if (params.row.AuditStatus === 0) {
                return h("div", "无");
              } else if (params.row.AuditStatus === 1) {
                return h("div", "待审核");
              } else if (params.row.AuditStatus === 2) {
                return h("div", "已通过");
              } else if (params.row.AuditStatus === 3) {
                return h("div", [h(
                    "div",
                    "已拒绝"
                  ), h(
                    "Button",
                    {
                      props: {
                        type: "error",
                        size: "small"
                      },
                      style: {
                        marginRight: "5px"
                      },
                      on: {
                        click: () => {
                            this.seeRejectRemark(params.row.ActivityId);
                        }
                      }
                    },
                    "查看原因"
                  )]);
              }
                }
            }
          },
          {
            title: "操作",
            key: "action",
            width: 170,
            align: "center",
            render: (h, params) => {
              if (params.row.Status === 1) {
                return h("div", [
                  h(
                    "Button",
                    {
                      props: {
                        type: "error",
                        size: "small"
                      },
                      style: {
                        marginRight: "5px"
                      },
                      on: {
                        click: () => {
                            this.clickUnshelve(params.row.ActivityId);
                        }
                      }
                    },
                    "下架"
                  ), h(
                    "Button",
                    {
                      props: {
                        type: "info",
                        size: "small"
                      },
                      style: {
                        marginRight: "5px"
                      },
                      on: {
                        click: () => {
                            this.clickEidt(params.row.ActivityId);
                        }
                      }
                    },
                    "编辑"
                  ),
                  h(
                    "Button",
                    {
                      props: {
                        type: "info",
                        size: "small"
                      },
                      style: {
                        marginRight: "5px"
                      },
                      on: {
                        click: () => {
                            this.clickLookInfo(params.row.ActivityId);
                        }
                      }
                    },
                    "查看"
                  )
                ]);
              } else if (params.row.Status === 2) {
                return h("div", [
                  h(
                    "Button",
                    {
                      props: {
                        type: "error",
                        size: "small"
                      },
                      style: {
                        marginRight: "5px"
                      },
                      on: {
                        click: () => {
  this.clickPassAudit(params.row.ActivityId);
                        }
                      }
                    },
                    "审核"
                  ),
                  h(
                    "Button",
                    {
                      props: {
                        type: "info",
                        size: "small"
                      },
                      style: {
                        marginRight: "5px"
                      },
                      on: {
                        click: () => {
                            this.clickLookInfo(params.row.ActivityId);
                        }
                      }
                    },
                    "查看"
                  )
                ]);
              } else if (params.row.Status === 6) {
                return h("div", [
                  h(
                    "Button",
                    {
                      props: {
                        type: "error",
                        size: "small"
                      },
                      style: {
                        marginRight: "5px"
                      },
                      on: {
                        click: () => {
                            this.clickWaitAudit(params.row.ActivityId);
                        }
                      }
                    },
                    "提交审核"
                  ), h(
                    "Button",
                    {
                      props: {
                        type: "info",
                        size: "small"
                      },
                      style: {
                        marginRight: "5px"
                      },
                      on: {
                        click: () => {
                            this.clickEidt(params.row.ActivityId);
                        }
                      }
                    },
                    "编辑"
                  ),
                  h(
                    "Button",
                    {
                      props: {
                        type: "info",
                        size: "small"
                      },
                      style: {
                        marginRight: "5px"
                      },
                      on: {
                        click: () => {
                            this.clickLookInfo(params.row.ActivityId);
                        }
                      }
                    },
                    "查看"
                  )
                ]);
              } else {
                return h("div", [
                  h(
                    "Button",
                    {
                      props: {
                        type: "success",
                        size: "small"
                      },
                      style: {
                        marginRight: "5px"
                      },
                      on: {
                        click: () => {
                          this.clickEidt(params.row.ActivityId);
                        }
                      }
                    },
                    "编辑"
                  ),
                  h(
                    "Button",
                    {
                      props: {
                        type: "info",
                        size: "small"
                      },
                      on: {
                        click: () => {
                          this.clickLookInfo(params.row.ActivityId);
                        }
                      }
                    },
                    "查看"
                  )
                ]);
              }
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
    StartTimeChange: function (e) {
      // 设置开始时间
      this.search_data.StartTime = e;
      this.search_data.EndTimeOptions = {
        disabledDate: date => {
          let StartTime = this.search_data.StartTime
            ? new Date(this.search_data.StartTime).valueOf()
            : "";
          return date && date.valueOf() < StartTime;
        }
      };
    },
    EndTimeChange: function (e) {
      // 设置结束时间
      this.search_data.EndTime = e;
      let EndTime = this.search_data.EndTime
        ? new Date(this.search_data.EndTime).valueOf() - 1 * 24 * 60 * 60 * 1000
        : "";
      this.search_data.StartTimeOptions = {
        disabledDate (date) {
          return date && date.valueOf() > EndTime;
        }
      };
    },
    btnloadData () {
        if (this.search_data.EndTime != null && this.search_data.EndTime !== '' && this.search_data.StartTime > this.search_data.EndTime) {
 this.messageInfo("开始时间不能大于结束时间");
            return false;
        }
         this.loadData();
    },
    loadData () {
        // 查询列表
      this.table.loading = true;
      this.ajax
        .post("/salepromotionactivity/SelectActivityList", {
          search_data: this.search_data,
          pageIndex: this.page.current,
          pageSize: this.page.pageSize
        })
        .then(response => {
          var data = response.data;
          if (data.Status) {
            this.page.total = data.Total;
            this.table.data = data.List;
            this.table.loading = false;
            if (data.CountModel !== null) { 
                this.countModel = data.CountModel; 
}
          } else {
this.messageInfo(data.Msg);
          }
        });
    },
    selectSattus () {
this.loadData();
    },
    addactivity () {
      this.$router.push({
        path: "/discount/editactivity",
        query: {
          activityId: ""
        }
      });
    },
    // 操作页面跳转
    clickEidt (activityId) {
      if (activityId == null || activityId === undefined) {
        activityId = "";
      }
      this.$router.push({
        path: "/discount/editactivity",
        query: {
          activityId: activityId
        }
      });
    },
    clickLookInfo (activityId) {
      if (activityId == null || activityId === undefined) {
        activityId = "";
      }
      this.$router.push({
        path: "/discount/activityinfo",
        query: {
          activityId: activityId
        }
      });
    },
    clickLookLog (activityId) {
      if (activityId == null || activityId === undefined) {
        activityId = "";
      }
    },
    clickUnshelve (activityId) {
      if (activityId == null || activityId === undefined) {
      
      } else {
 this.unShelveModal.visible = true;
      this.unShelveModal.activityId = activityId;
      }
    },
     clickPassAudit (activityId) {
      if (activityId == null || activityId === undefined) {
        activityId = "";
      }
      this.$router.push({
        path: "/discount/waitaudit",
        query: {
          activityId: activityId
        }
      });
    },
    seeRejectRemark (activityId) {
        this.ajax.post("/salepromotionactivity/GetActivityModel", {
activityId: activityId
        }).then(res => {
if (res.data.Status) {
this.rejectRemarkModal.visible = true;
this.rejectRemarkModal.remark = res.data.Data.AuditRemark;
this.rejectRemarkModal.userName = res.data.Data.AuditUserName;
} else {
this.messageInfo("获取拒绝原因失败");
}
        })
    },
     clickWaitAudit (activityId) {
      if (activityId == null || activityId === undefined) {
        activityId = "";
      }
    },
    unShelveActivity () {
        this.ajax.post("/salepromotionactivity/UnShelveActivity", {
activityId: this.unShelveModal.activityId
        }).then(res => {
if (res.data.Status) {
this.messageInfo("成功下架该活动");
this.loadData();
} else {
this.messageInfo(res.data.Msg);
}
        })
    },
    handleResetForm () {
      this.search_data = {
        Name: "",
        Pid: "",
        Status: "0",
        StartTime: "", // 开始日期model
        EndTime: "" // 结束日期model
      };
    },
    handlePageChange (pageIndex) {
      this.page.current = pageIndex;
      this.loadData();
    },
    handlePageSizeChange (pageSize) {
      this.page.pageSize = pageSize;
      this.loadData();
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

<style scoped>
.ivu-radio-group-button .ivu-radio-wrapper-checked {
  color: aliceblue;
  background: rgba(12, 183, 198, 1);
}
</style>
