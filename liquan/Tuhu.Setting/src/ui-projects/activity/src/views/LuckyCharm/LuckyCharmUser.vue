<template>
  <div>
    <h1 class="title">报名用户列表</h1>
   
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
        <Form ref="modal.users" :model="modal.users" :rules="modal.rules" label-position="left" :label-width="150">
            <FormItem label="用户名" prop="UserName">
                <Input v-model.trim="modal.users.UserName" placeholder=""/>
            </FormItem>
            <FormItem label="手机号" prop="Phone">
                <Input v-model.trim="modal.users.Phone" placeholder=""/>
            </FormItem>     
            <FormItem label="区域名称" prop="AreaName">
                <Input v-model.trim="modal.users.AreaName" placeholder=""/>
            </FormItem>     
            <FormItem label="活动ID" prop="ActivityId">
                <Input v-model.trim="modal.users.ActivityId" placeholder=""/>
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
                        title: '用户名',
                        align: 'center',
                        key: 'UserName'
                     },
                    {
                        title: '手机号',
                        align: 'center',
                        key: 'Phone'
                    },
                    {
                        title: '活动ID',
                        align: 'left',
                        key: 'ActivityId'
                     },
                      {
                        title: '区域Id',
                        align: 'left',
                        key: 'AreaId'
                     },  
                     {
                        title: '区域名称',
                        align: 'left',
                        key: 'AreaName'
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
                                        type: "primary",
                                        size: "small"
                                    },
                                    style: {
                                        marginRight: "3px",
                                        marginLeft: "10px"
                                    },
                                    on: {
                                    click: () => {
                                        this.update(p.row);
                                     }
                                    }
                                },
                                "编辑"
                                ),
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
                title: "编辑活动报名人员",            
                users: {
                    Pkid: 0,
                    ActivityId: 0,
                    UserId: 0,
                    UserName: "",
                    Phone: "",
                    AreaId: 0,
                    AreaName: ""
                },
                rules: {
                    UserName: [
                    {
                        required: true,
                        message: "请填写用户名称",
                        trigger: "change"
                    }
                ],
                    Phone: [
                    {
                        required: true,
                        message: "请填写手机号",
                        trigger: "change"
                    }
                ],
                    AreaName: [
                    {
                        required: true,
                        message: "请填写地区",
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
        loadData (pageIndex) {
            this.page.index = pageIndex;
            this.table.data = [];
            this.table.loading = true;
            this.ajax.get(`/LuckyCharm/PageUser?pageSize=${this.page.size}&pageIndex=${this.page.index}`).then(response => {
               var res = response.data.Users;
                this.page.total = response.data.Total;
                this.table.data = res || [];
                this.table.loading = false;
            })
        },
        update (item) {
            this.modal.users.Pkid = item.PKID;
            this.modal.users.UserName = item.UserName;
            this.modal.users.UserId = item.UserId;
            this.modal.users.AreaName = item.AreaName;
            this.modal.users.AreaId = item.AreaId;
            this.modal.users.Phone = item.Phone;
            this.modal.users.ActivityId = item.ActivityId;
            this.modal.title = "修改活动参加用户";
            this.modal.visible = true;
        },
        add () {
            this.$refs['modal.users'].resetFields();
            this.modal.title = "新增活动报名用户";
            this.modal.users.UserName = "";
            this.modal.users.UserId = 0;
            this.modal.users.AreaName = "";
            this.modal.users.AreaId = 0;
            this.modal.users.Phone = "";
            this.modal.users.ActivityId = 0;
            this.modal.users.Pkid = 0;
            this.modal.visible = true;
        },
        cancel () {
            this.$refs['modal.users'].resetFields();
            this.modal.visible = false;
        },
        delete (pkid) {
            this.$Modal.confirm({
                title: "温馨提示",
                content: "确认删除该报名人员?",
                loading: true,
                onOk: () => {
                    this.ajax.post("/LuckyCharm/DelUser", {
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
            this.$refs['modal.users'].validate((valid) => {
                if (valid) {                   
                    if (typeof (this.modal.users.Phone) === "undefined" || this.modal.users.Phone === "") {
                        this.$Message.error("请输入手机号");
                        this.modal.loading = false;
                        this.$nextTick(() => {
                          this.modal.loading = true;
                        });
                        return;
                    } 
                    if (typeof (this.modal.users.AreaName) === "undefined" || this.modal.users.AreaName === "") {
                        this.$Message.error("请输入用户名");
                        this.modal.loading = false;
                        this.$nextTick(() => {
                          this.modal.loading = true;
                        });
                        return;
                    } 
                    if (typeof (this.modal.users.ActivityId) === "undefined" || this.modal.users.ActivityId === "") {
                        this.$Message.error("活动ID");
                        this.modal.loading = false;
                        this.$nextTick(() => {
                          this.modal.loading = true;
                        });
                        return;
                    } 
                    if (typeof (this.modal.users.UserName) === "undefined" || this.modal.users.UserName === "") {
                        this.$Message.error("请输入用户名称");
                        this.modal.loading = false;
                        this.$nextTick(() => {
                          this.modal.loading = true;
                        });
                        return;
                    }
  
                    var isUpdate = false;
                    if (this.modal.users.Pkid > 0) {
                        isUpdate = true;
                    }
                    this.ajax.post(isUpdate ? '/LuckyCharm/EditUser' : "/LuckyCharm/AddUser", this.modal.users
                    ).then((response) => {
                        if (response.data) {
                            setTimeout(() => {
                            this.$Message.success("操作成功");
                            if (isUpdate) {
                                this.loadData(this.page.index);
                            } else {
                                this.loadData(1);
                            }
                            this.$refs['modal.users'].resetFields();
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
