<template>
  <div>
    <h1 class="title">活动列表</h1>
   
     <Row style="margin-bottom:15px;">
        <Button type="success" style="margin-left: 8px;float:left" @click="add">新增</Button>
    </Row>

    <Table  :loading="table.loading" :columns="table.columns" :data="table.data"></Table>
    <Page style="margin-top: 15px;"
          show-total
          show-sizer
          :page-size-opts="[10, 20, 40, 100, 200]"
          :total="page.total"
          :current.sync="page.index"
          :page-size="page.size"
          @on-page-size-change="page.size=arguments[0]"></Page>
          
    <Modal
        v-model="modal.visible"
        :loading="modal.loading"
        :title="modal.title"
        okText="提交"
        cancelText="取消"
        @on-ok="ok"
        @on-cancel="cancel" :mask-closable="false">
        <Form ref="modal.activitys" :model="modal.activitys" :rules="modal.rules" label-position="left" :label-width="150">
            <FormItem label="活动名称" prop="ActivityTitle">
                <Input v-model.trim="modal.activitys.ActivityTitle" placeholder=""/>
            </FormItem>
            <FormItem label="活动口号" prop="ActivitySlug">
                <Input v-model.trim="modal.activitys.ActivitySlug" placeholder=""/>
            </FormItem>     
            <FormItem label="活动描述" prop="ActivitDes">
                <Input v-model.trim="modal.activitys.ActivityDes" placeholder=""/>
            </FormItem>     
            <FormItem label="开始时间" prop="StarTime">
                <Input v-model.trim="modal.activitys.StarTime" placeholder=""/>
            </FormItem>    
            <FormItem label="结束时间" prop="EndTime">
                <Input v-model.trim="modal.activitys.EndTime" placeholder=""/>
            </FormItem>      
        </Form>
    </Modal>
  </div>
</template>
<script>
export default {
    data () {
        return {
            table: {
               columns: [
                    {
                        title: '序号',
                        align: 'center',
                        width: 100,
                        type: "index",
                        key: "PKID"
                    },
                    {
                        title: '活动名称',
                        align: 'center',
                        key: 'ActivityTitle'
                     },
                    {
                        title: '活动口号',
                        align: 'center',
                        key: 'ActivitySlug'
                    },
                    {
                        title: '活动描述',
                        align: 'left',
                        key: 'ActivityDes'
                     },
                      {
                        title: '开始时间',
                        key: 'StarTime',
                        align: 'center',
                        render: (h, params) => {
                        return h('div', [
                                h('span', this.FormatToDate(params.row.StarTime))
                                        ]);
                        }
                     },  
                     {
                        title: '结束时间',
                        key: 'EndTime',
                        align: 'center',
                        render: (h, params) => {
                        return h('div', [
                                h('span', this.FormatToDate(params.row.EndTime))
                                        ]);
                        }
                     },
                    {
                        title: '操作',
                        width: 200,
                        render: (h, p) => {
                             let buttons = [];
                            buttons.push(
                                 h(
                                "Button",
                                {
                                    props: {
                                        type: "error",
                                        size: "small"
                                    },
                                    style: {
                                        marginRight: "3px",
                                        marginLeft: "10px"
                                    },
                                    on: {
                                    click: () => {
                                        this.delete(p.row.PKID);
                                     }
                                    }
                                },
                                "删除"
                                )
                            );
                            return h("div", buttons);
                        }
                    }
                ],
                data: [],
                loading: false
            },
            page: { total: 0, index: 1, size: 10 },
            modal: {
                visible: false,
                loading: true,           
                title: "编辑活动信息",            
                activitys: {
                    Pkid: 0,
                    ActivityTitle: 0,
                    ActivitySlug: 0,
                    ActivityDes: "",
                    StarTime: "",
                    EndTime: 0
                },
                rules: {
                    ActivityTitle: [
                    {
                        required: true,
                        message: "请填写活动名称",
                        trigger: "change"
                    }
                ],
                    StarTime: [
                    {
                        required: true,
                        message: "请填写开始时间",
                        trigger: "change"
                    }
                ],
                    EndTime: [
                    {
                        required: true,
                        message: "请填写结束时间",
                        trigger: "change"
                    }
                ]
                }
            }
        }
    },
    created () {
        this.loadData();
        this.$Message.config({
            duration: 5
        });
    },
    watch: {
        "page.index" () {
            this.loadData(this.page.index);
        },
        "page.size" () {
            this.loadData(1);
        }
    },
    methods: {
        FormatToDate (timestamp) {
            var time = new Date(parseInt(timestamp.replace("/Date(", "").replace(")/", ""), 10));
            var year = time.getFullYear();
            var month = time.getMonth() + 1 < 10 ? "0" + (time.getMonth() + 1) : time.getMonth() + 1;
            var date = time.getDate() < 10 ? "0" + time.getDate() : time.getDate();
            var YmdHis = year + '-' + month + '-' + date;
            return YmdHis;
        },
        loadData (pageIndex) {
            this.page.index = pageIndex;
            this.table.data = [];
            this.table.loading = true;
            this.ajax.get(`/LuckyCharm/PageActivity?pageSize=${this.page.size}&pageIndex=${this.page.index}`).then(response => {
               var res = response.data.Activitys;
                this.page.total = response.data.Total;
                this.table.data = res || [];
                this.table.loading = false;
            })
        },
        update (item) {
            this.modal.activitys.Pkid = item.PKID;
            this.modal.activitys.ActivityTitle = item.ActivityTitle;
            this.modal.activitys.ActivitySlug = item.ActivitySlug;
            this.modal.activitys.ActivityDes = item.ActivityDes;
            this.modal.activitys.StarTime = item.StarTime;
            this.modal.activitys.EndTime = item.EndTime;
            this.modal.title = "修改活动信息";
            this.modal.visible = true;
        },
        add () {
            this.$refs['modal.activitys'].resetFields();
            this.modal.title = "新增活动信息";
            this.modal.activitys.ActivityTitle = "";
            this.modal.activitys.ActivitySlug = "";
            this.modal.activitys.ActivityDes = "";
            this.modal.activitys.StarTime = "";
            this.modal.activitys.EndTime = "";
            this.modal.visible = true;
        },
        cancel () {
            this.$refs['modal.activitys'].resetFields();
            this.modal.visible = false;
        },
        delete (pkid) {
            this.$Modal.confirm({
                title: "温馨提示",
                content: "确认删除该报名人员?",
                loading: true,
                onOk: () => {
                    this.ajax.post("/LuckyCharm/DelActivity", {
                        pkid: pkid
                    }).then(response => {
                        var res = response.data;
                        if (res) {
                            this.$Message.info("删除成功");
                        } else {
                            this.$Message.error("删除失败")
                        }
                        this.$Modal.remove();
                        if (res) {
                            this.loadData(1);
                        }
                    })
                }
            });
        },
        ok () {
            this.modal.loading = true;
            this.$refs['modal.activitys'].validate((valid) => {
                if (valid) {                   
                    if (typeof (this.modal.activitys.ActivityTitle) === "undefined" || this.modal.activitys.ActivityTitle === "") {
                        this.$Message.error("请输入手机号");
                        this.modal.loading = false;
                        this.$nextTick(() => {
                          this.modal.loading = true;
                        });
                        return;
                    } 
                    if (typeof (this.modal.activitys.StarTime) === "undefined" || this.modal.activitys.StarTime === "") {
                        this.$Message.error("请输入用户名");
                        this.modal.loading = false;
                        this.$nextTick(() => {
                          this.modal.loading = true;
                        });
                        return;
                    } 
                    if (typeof (this.modal.activitys.EndTime) === "undefined" || this.modal.activitys.EndTime === "") {
                        this.$Message.error("活动ID");
                        this.modal.loading = false;
                        this.$nextTick(() => {
                          this.modal.loading = true;
                        });
                        return;
                    } 
                   
                    var isUpdate = false;
                    if (this.modal.activitys.Pkid > 0) {
                        isUpdate = true;
                    }
                    this.ajax.post(isUpdate ? '/LuckyCharm/EditActivity' : "/LuckyCharm/AddActivity", this.modal.activitys
                    ).then((response) => {
                        if (response.data) {
                            setTimeout(() => {
                            this.$Message.success("操作成功");
                            if (isUpdate) {
                                this.loadData(this.page.index);
                            } else {
                                this.loadData(1);
                            }
                            this.$refs['modal.activitys'].resetFields();
                            this.modal.visible = false;
                            this.modal.loading = false;
                            this.$nextTick(() => {
                                this.modal.loading = true;
                        });
                        }, 2000);
                        } else {
                            this.$Message.error("操作失败");
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
        }
    }
}
</script>
