<template>
  <div>
    <h1 class="title">管理员列表</h1>
   
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
        <Form ref="modal.administrators" :model="modal.administrators" :rules="modal.rules" label-position="left" :label-width="150">
            <FormItem label="微信昵称" prop="WeChatNickName">
                <Input v-model.trim="modal.administrators.WeChatNickName" placeholder=""/>
            </FormItem>
            <FormItem label="微信号" prop="WeChatNumber">
                <Input v-model.trim="modal.administrators.WeChatNumber" placeholder=""/>
            </FormItem>
            <FormItem  label="微信头像" >
                <Col span="4" v-show="modal.administrators.WeChatHeadPortrait!=''">
                    <a :href="modal.administrators.WeChatHeadPortrait" target="_blank"><img :src="modal.administrators.WeChatHeadPortrait" style='width:50px;height:50px'></a>
                </Col>
                <Col span="6">
                    <Upload action="/GroupBuyingV2/UploadImage?type=image" :format="['jpg','jpeg','png']" :on-format-error="handleFormatError"  :on-success="handleSuccess1" :show-upload-list="false">
                        <Button type="ghost" icon="ios-cloud-upload-outline">Upload files</Button>
                    </Upload>
                </Col>
                <Col span="5"  style="margin-left:30px;" v-show="modal.administrators.WeChatHeadPortrait!=''">
                    <Button type="warning" icon="refresh" @click="modal.administrators.WeChatHeadPortrait=''">清除</Button>
                </Col>
            </FormItem>
            <FormItem  label="微信二维码" >
                <Col span="4" v-show="modal.administrators.WeChatQRCode!=''">
                    <a :href="modal.administrators.WeChatQRCode" target="_blank"><img :src="modal.administrators.WeChatQRCode" style='width:50px;height:50px'></a>
                </Col>
                <Col span="6">
                    <Upload action="/GroupBuyingV2/UploadImage?type=image" :format="['jpg','jpeg','png']" :on-format-error="handleFormatError"  :on-success="handleSuccess2" :show-upload-list="false">
                        <Button type="ghost" icon="ios-cloud-upload-outline">Upload files</Button>
                    </Upload>
                </Col>
                <Col span="5"  style="margin-left:30px;" v-show="modal.administrators.WeChatQRCode!=''">
                    <Button type="warning" icon="refresh" @click="modal.administrators.WeChatQRCode=''">清除</Button>
                </Col>
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
                        title: '微信头像',
                        align: 'center',
                        key: 'WeChatHeadPortrait',
                        render: (h, params) => {
                            return h('img', {domProps: { src: params.row.WeChatHeadPortrait, width: "75", height: "80" }}, params.row.WeChatHeadPortrait);
                        }
                     },
                    {
                        title: '微信二维码',
                        align: 'center',
                        key: 'WeChatQRCode',
                        render: (h, params) => {
                            return h('img', {domProps: { src: params.row.WeChatQRCode, width: "75", height: "80" }}, params.row.WeChatQRCode);
                        }
                    },
                    {
                        title: '微信昵称',
                        align: 'left',
                        key: 'WeChatNickName'
                     },
                      {
                        title: '微信号',
                        align: 'left',
                        key: 'WeChatNumber'
                     },
                    {
                        title: "创建时间",
                        key: "CreateDatetime",
                        align: 'center',
                        render: (h, params) => {
                        return h('div', [
                                h('span', this.FormatToDate(params.row.CreateDatetime))
                                        ]);
                        }
                    },
                    {
                        title: "创建人",
                        key: "CreateBy",
                        align: 'left'
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
                title: "编辑途虎管理员",            
                administrators: {
                    pkid: 0,
                    WeChatNickName: "",
                    WeChatNumber: "",
                    WeChatHeadPortrait: "",
                    WeChatQRCode: ""
                },
                rules: {
                    WeChatNickName: [
                    {
                        required: true,
                        message: "请填写微信昵称",
                        trigger: "change"
                    }
                ],
                    WeChatNumber: [
                    {
                        required: true,
                        message: "请填写微信号",
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
            this.ajax.get(`/CarFriendsGroup/GetCarFriendsAdministratorList?pageSize=${this.page.size}&pageIndex=${this.page.index}`).then(response => {
                var res = response.data.data;
                this.page.total = response.data.count;
                this.table.data = res || [];
                this.table.loading = false;
            })
        },
        handleFormatError (file) {
            this.$Message.warning("请选择 .jpg  or .png  or .jpeg图片");
        },
        handleSuccess1 (res, file) {
            if (res.Status) {
                this.modal.administrators.WeChatHeadPortrait = res.ImageUrl
                } else {
                    this.$Message.warning(res.Msg);
            }
        },
        handleSuccess2 (res, file) {
            if (res.Status) {
                this.modal.administrators.WeChatQRCode = res.ImageUrl
                } else {
                    this.$Message.warning(res.Msg);
            }
        },
        update (item) {
            this.modal.administrators.pkid = item.PKID;
            this.modal.administrators.WeChatNickName = item.WeChatNickName
            this.modal.administrators.WeChatNumber = item.WeChatNumber;
            this.modal.administrators.WeChatHeadPortrait = item.WeChatHeadPortrait;
            this.modal.administrators.WeChatQRCode = item.WeChatQRCode;
            this.modal.title = "编辑途虎管理员";
            this.modal.visible = true;
        },
        add () {
            this.$refs['modal.administrators'].resetFields();
            this.modal.title = "新增途虎管理员";
            this.modal.administrators.pkid = 0;
            this.modal.administrators.WeChatHeadPortrait = "";
            this.modal.administrators.WeChatQRCode = "";
            this.modal.visible = true;
        },
        cancel () {
            this.$refs['modal.administrators'].resetFields();
            this.modal.visible = false;
        },
        delete (pkid) {
            this.$Modal.confirm({
                title: "温馨提示",
                content: "确认删除该管理员?",
                loading: true,
                onOk: () => {
                    this.ajax.post("/CarFriendsGroup/DeleteCarFriendsAdministrator", {
                        pkid: pkid
                    }).then(response => {
                        var res = response.data;
                        if (res.status) {
                            this.$Message.info(res.msg);
                        } else {
                            this.$Message.error(res.msg || '')
                        }
                        this.$Modal.remove();
                        if (res.status) {
                            this.loadData(1);
                        }
                    })
                }
            });
        },
        ok () {
            this.modal.loading = true;
            this.$refs['modal.administrators'].validate((valid) => {
                if (valid) {                   
                    if (typeof (this.modal.administrators.WeChatHeadPortrait) === "undefined" || this.modal.administrators.WeChatHeadPortrait === "") {
                        this.$Message.error("请上传微信头像");
                        this.modal.loading = false;
                        this.$nextTick(() => {
                          this.modal.loading = true;
                        });
                        return;
                    } 
                    if (typeof (this.modal.administrators.WeChatQRCode) === "undefined" || this.modal.administrators.WeChatQRCode === "") {
                        this.$Message.error("请上传微信二维码");
                        this.modal.loading = false;
                        this.$nextTick(() => {
                          this.modal.loading = true;
                        });
                        return;
                    }
                    var isUpdate = false;
                    if (this.modal.administrators.pkid > 0) {
                        isUpdate = true;
                    }
                    this.ajax.post(isUpdate ? '/CarFriendsGroup/UpdateCarFriendsAdministrator' : "/CarFriendsGroup/AddCarFriendsAdministrator", this.modal.administrators
                    ).then((response) => {
                        if (response.data.status) {
                            setTimeout(() => {
                            this.$Message.success(response.data.msg);
                            if (isUpdate) {
                                this.loadData(this.page.index);
                            } else {
                                this.loadData(1);
                            }
                            this.$refs['modal.administrators'].resetFields();
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
        }
    }
}
</script>
