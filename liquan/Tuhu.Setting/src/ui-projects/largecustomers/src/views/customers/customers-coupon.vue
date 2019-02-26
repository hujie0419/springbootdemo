<template>
  <div>
    <h1 class="title">企业客户专享券码</h1>
    <Form :model="search_data" :label-width="100">
        <Row style="margin-bottom:15px;">
           <Col span="5">
               <Input v-model="search_data.queryString" placeholder="请填写券码或手机号"></Input>
            </Col>
            <Button type="primary" @click="page.current=1;loadData(1)" style="margin-left: 8px;float:left">查询</Button>
            <Button type="ghost" style="margin-left: 8px;float:left" @click="handleResetForm">重置</Button>
            <Button type="success" style="margin-left: 8px;float:left" @click="modal.edit=false;modal.visible = true;modal.title = '单个添加券码';">新增</Button>
             <Button type="success"  icon="share" @click="ExportCustomerCoupon"  style="margin-left: 8px;float:left">导出</Button>
            <Upload 
                :action = "this.actionURL"
                :format="['xlsx','xls']" :on-format-error="handleFormatError" :max-size="200"
                :on-success="uploadSuccess"
                style="margin-left: 8px;display: inline;float:left">
                <Button type="warning" icon="ios-cloud-upload-outline">批量导入</Button>
            </Upload>
            <a style="margin-left: 8px;margin-top:7px;display: inline;float:left" href="/customersactivity/ExportSample" target="_blank">导出模板文件</a>
        </Row>
    </Form>
    <Table border :loading="table.loading" :columns="table.columns" :data="table.data"></Table>
    <div style="margin-top:15px;float:right">
        <Page  :total="page.total" :page-size="page.pageSize" :current="page.current" :page-size-opts="[10,20 ,50 ,100]" show-elevator show-sizer @on-change="handlePageChange" @on-page-size-change="handlePageSizeChange"></Page>
    </div>
    <Modal
        v-model="modal.visible"
        :loading="modal.loading"
        :title="modal.title"
        okText="提交"
        cancelText="取消"
        @on-ok="ok"
        @on-cancel="cancel">
        <Form ref="modal.gpConfig" :model="modal.gpConfig" :rules="modal.rules" label-position="left" :label-width="80">
          <FormItem  label="*券码" prop="CouponCode">
               <Input  v-model="modal.gpConfig.CouponCode" :disabled="modal.edit" placeholder=""/>
           </FormItem>
        </Form>
    </Modal>
    <Modal
        v-model="deleteModal.visible"
        title="删除"
        :loading="deleteModal.loading"
        width = "10px"
        @on-ok="deleteok">
        <p>确认删除？</p>
    </Modal>
     <Modal v-model="logmodal.visible" title="操作日志" cancelText="取消" @on-cancel="cancel" :width="logmodal.width">
            <Table :loading="logmodal.loading" :data="logmodal.data" :columns="logmodal.columns" stripe></Table>
    </Modal>
  </div>
</template>

<script>
const defaultformdata = {
    queryString: "",
    customersSettingId: "",
    activityExclusiveId: ""
};
const defaultGroupConfig = {
    PKID: 0,
    CouponCode: "",
    CustomerExclusiveSettingPkId: "",
    ActivityExclusiveId: "",
    Status: 0
};
export default {
  data () {
      return {
        actionURL: "/customersactivity/upload?customersSettingId=" + this.$route.params.customersSettingId + "&activityExclusiveId=" + this.$route.params.activityExclusiveId,
        search_data: {},
        page: {
            total: 0,
            current: 1,
            pageSize: 10
        },
        table: {
            loading: true,
            data: [],
            columns: [
                {
                    title: '活动专享ID',
                    key: 'ActivityExclusiveId',
                    align: 'center'
                },
                {
                    title: '活动券码',
                    key: 'CouponCode',
                    align: 'center'
                },
                {
                    title: '姓名',
                    key: 'UserName',
                    align: 'center'
                },
                {
                    title: '手机号',
                    key: 'Phone',
                    align: 'center'
                },
                {
                    title: 'Userid',
                    key: 'UserId',
                    align: 'center',
                    render: (h, params) => {
                        return h('span', {}, params.row.UserId === "00000000-0000-0000-0000-000000000000" ? "" : params.row.UserId);
                    }
                },
                 {
                    title: "创建时间",
                    key: "CreateTime",
                    align: "center",
                    render: (h, params) => {
                        return h(
                             "span",
                                this.formatDate(params.row.CreateTime)
                            );
                    }
                },
                 {
                    title: "修改时间",
                    key: "UpdateDatetime",
                    align: "center",
                    render: (h, params) => {
                        return h(
                             "span",
                                this.formatDate(params.row.UpdateDatetime)
                            );
                    }
                },
                {
                    title: '状态',
                    key: 'Status',
                    align: "center",                   
                    render: (h, params) => {
                        return h('span', {}, params.row.Status === "0" ? "正常" : "删除");
                    }
                },
                {
                    title: '操作',
                    key: 'action',
                    width: 150,
                    align: 'center',
                    render: (h, params) => {
                       if (params.row.Status === "0") {
                          return h('div', [
                            h('Button', {
                                props: {
                                    type: 'error',
                                    size: 'small'
                                },
                                on: {
                                    click: () => {
                                        this.modal.gpConfig.PKID = this.table.data[params.index].PKID;
                                        this.modal.gpConfig.Status = -1;
                                        this.deleteModal.visible = true;
                                    }
                                }
                            }, '删除'),
                            h(
                                "Button",
                                {
                                    props: {
                                         type: "primary",
                                         size: "small"
                                    },
                                    style: {
                                      marginLeft: '5px'
                                    },
                                    on: {
                                        click: () => {
                                            this.SearchLog(
                                                params.row.PKID
                                            );
                                        }
                                        }
                                },
                                    "日志"
                                )
                        ]);
                       } else {
                            return h('div', [ 
                            h(
                                "Button",
                                {
                                    props: {
                                         type: "primary",
                                         size: "small"
                                    },
                                    on: {
                                        click: () => {
                                            this.SearchLog(
                                                params.row.PKID
                                            );
                                        }
                                        }
                                }, "日志")
                        ]);
                       }   
                    }
                }
            ]
        },
        deleteModal: {
            visible: false,
            loading: true
        },
        logmodal: {
            loading: true,
            visible: false,
            width: 885,
            data: [],
            columns: [
                {
                    title: "类型",
                    width: 200,
                    key: "ObjectType",
                    align: "center",
                    fixed: "left"
                },
                    {
                    title: "消息",
                    width: 300,
                    key: "Remark",
                    align: "center",
                    fixed: "left"
                },
                {
                    title: "操作人",
                    width: 150,
                    key: "Creator",
                    align: "center",
                    fixed: "left"
                },
                {
                    title: "时间",
                    width: 200,
                    key: "CreateDateTime",
                    align: "center",
                    fixed: "left",
                    render: (h, params) => {
                        return h(
                             "span",
                                this.formatDate(params.row.CreateDateTime)
                            );
                    }
                    }
                ]
            },
        modal: {
            visible: false,
            loading: true,
            edit: true,
            title: "",
            gpConfig: {
                  PKID: 0,
                  CouponCode: "",
                  CustomerExclusiveSettingPkId: "",
                  ActivityExclusiveId: "",
                  Status: 0
            },
            rules: {
                CouponCode: [
                    {
                        required: true,
                        message: "请填写券码",
                        trigger: "blur"
                    }
                ]
            }
        }
    }
  },
  created () {
    this.search_data = Object.assign({}, defaultformdata);
    this.loadData(1);
  },
  methods: {
    loadData (pageIndex) {
      this.search_data.customersSettingId = this.$route.params.customersSettingId;
      this.search_data.activityExclusiveId = this.$route.params.activityExclusiveId;
      this.table.loading = true;
      this.page.current = pageIndex;
      this.ajax.get('/customersactivity/SelectCustomerCoupons', { 
          params: {
              ...this.search_data, 
              pageIndex: this.page.current, 
              pageSize: this.page.pageSize
            } 
        })
      .then((response) => {
        var data = response.data;
        this.page.total = data.totalCount;
        this.table.data = data.data;
        this.table.loading = false;
      });
    },
    handlePageChange (pageIndex) {
        this.page.current = pageIndex;
        this.loadData(pageIndex);
    },
    handlePageSizeChange (pageSize) {
        this.page.pageSize = pageSize;
        this.loadData(this.page.current);
    },
    handleFormatError (file) {
        this.util.message.warning({
            content: file.name + ' 格式错误，必须是xls或xlsx文件',
            duration: 3,
            closable: true
        });
    },
    handleResetForm () {
        this.search_data = Object.assign({}, defaultformdata);
        this.loadData(this.page.current);
    },
    ExportCustomerCoupon () {
           window.open("/customersactivity/ExportCustomerCoupon?queryString=" + this.search_data.queryString + "&customersSettingId=" + 
                  this.search_data.customersSettingId + "&activityExclusiveId=" + this.search_data.activityExclusiveId
            );
    },
    ok () {
        this.modal.loading = true;
        this.modal.gpConfig.CustomerExclusiveSettingPkId = this.$route.params.customersSettingId;
         this.modal.gpConfig.ActivityExclusiveId = this.$route.params.activityExclusiveId;
        this.$refs['modal.gpConfig'].validate((valid) => {
            if (valid) {
              this.ajax.post("/customersactivity/InsertCustomerCoupon", this.modal.gpConfig
              ).then((response) => {
                  console.log(response);
                  if (response.data.success) {
                    setTimeout(() => {
                        this.$Message.success('添加成功');
                        this.loadData(1);
                        this.modal.gpConfig = Object.assign({}, defaultGroupConfig);
                        this.modal.visible = false;
                        this.modal.loading = false;
                        this.$nextTick(() => {
                            this.modal.loading = true;
                        });
                    }, 2000);
                  } else {
                    this.$Message.error(response.data.msg);
                    this.modal.loading = false;
                    this.$nextTick(() => {
                        this.modal.loading = true;
                    });
                  }
              });
            } else {
                this.modal.loading = false;
                this.$nextTick(() => {
                        this.modal.loading = true;
                    });
            }
        })
    },
    cancel () {
      this.modal.gpConfig = Object.assign({}, defaultGroupConfig);
      this.$refs['modal.gpConfig'].resetFields();
    },
    deleteok () {
        this.deleteModal.loading = true;
        this.ajax.post('/customersactivity/UpdateCustomerCouponStatus', this.modal.gpConfig
              ).then((response) => {
                  console.log(response);
                  if (response.data.success) {
                    setTimeout(() => {
                        this.$Message.success('删除成功');
                        this.loadData(this.page.current);
                        this.deleteModal.loading = false;
                        this.deleteModal.visible = false;
                        this.$nextTick(() => {
                            this.deleteModal.loading = true;
                        });
                    }, 2000);
                  } else {
                      console.log(this.deleteModal);
                    this.$Message.error('删除失败');
                    this.deleteModal.loading = false;
                    this.$nextTick(() => {
                        this.deleteModal.loading = true;
                    });
                  }
              });
    },
    uploadSuccess (res, file) {
        if (res.success) {            
            setTimeout(() => {
                        this.$Message.success({"content": res.msg, "duration": 0, "closable": true});
                        this.loadData(1);
                    }, 2000);
        } else {
            this.$Message.error({"content": res.msg, "duration": 0, "closable": true});
        }
    },
    SearchLog (pkid) {
            this.logmodal.loading = true;
            this.ajax
                .post("/customersactivity/getCustomerExclusiveSettingLogs", {
                    objeId: pkid,
                    source: "CustomerExclusiveCoupon"
                })
                .then(response => {
                    this.logmodal.data = response.data;
                    this.logmodal.visible = true;
                    this.logmodal.loading = false;
                });
     },
    formatDate (value) {
            if (value) {
                var type = typeof value;
                if (type === 'string') {
                    if (value.indexOf("Date") > 0) {
                        var time = new Date(
                            parseInt(value.replace("/Date(", "").replace(")/", ""))
                        );
                        var year = time.getFullYear();
                        var day = time.getDate();
                        var month = time.getMonth() + 1;
                        var hours = time.getHours();
                        var minutes = time.getMinutes();
                        var seconds = time.getSeconds();
                        var func = function (value, number) {
                            var str = value.toString();
                            while (str.length < number) {
                                str = "0" + str;
                            }
                            return str;
                        };
                        if (year === 1) {
                            return "";
                        } else {
                            return (
                                func(year, 4) +
                                "-" +
                                func(month, 2) +
                                "-" +
                                func(day, 2) +
                                " " +
                                func(hours, 2) +
                                ":" +
                                func(minutes, 2) +
                                ":" +
                                func(seconds, 2)
                            );
                        }
                    } else {
                        return value;
                    }
                } else {
                    var year1 = value.getFullYear();
                    var day1 = value.getDate();
                    var month1 = value.getMonth() + 1;
                    var hours1 = value.getHours();
                    var minutes1 = value.getMinutes();
                    var seconds1 = value.getSeconds();
                    var func1 = function (value, number) {
                        var str = value.toString();
                        while (str.length < number) {
                            str = "0" + str;
                        }
                        return str;
                    };
                    if (year === 1) {
                        return "";
                    } else {
                        return (
                            func1(year1, 4) +
                            "-" +
                            func1(month1, 2) +
                            "-" +
                            func1(day1, 2) +
                            " " +
                            func1(hours1, 2) +
                            ":" +
                            func1(minutes1, 2) +
                            ":" +
                            func1(seconds1, 2)
                        );
                    }
                }
            }
        }
  }
}
</script>

<style lang="less">
</style>
