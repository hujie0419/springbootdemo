<template>
  <div>
    <h1 class="title">团购配置</h1>
    <Form :model="search_data" :label-width="100">
        <Row>
            <Col span="6">
                <FormItem label="点评团购ID:">
                    <Input v-model="search_data.dianpingid" placeholder="点评团购ID"></Input>
                </FormItem>
            </Col>
            <Col span="6">
                <FormItem label="点评品牌名称:">
                    <Input v-model="search_data.dianpingbrand" placeholder="点评品牌名称"></Input>
                </FormItem></Col>
            <Col span="6">
                <FormItem label="点评团购名称:">
                    <Input v-model="search_data.dianpingname" placeholder="点评团购名称"></Input>
                </FormItem>
            </Col>
        </Row>
        <Row>
            <Col span="6">
                <FormItem label="途虎服务产品ID:">
                    <Input v-model="search_data.tuhupid" placeholder="途虎服务产品ID"></Input>
                </FormItem>
            </Col>
            <Col span="6">
                <FormItem label="途虎服务状态:">
                    <Select v-model="search_data.tuhustatus">
                        <Option value="-1">全部</Option>
                        <Option value="1">已上架</Option>
                        <Option value="0">未上架</Option>
                    </Select>
                </FormItem>
            </Col>
        </Row>
        <Row style="margin-bottom:15px;">
            <Button type="primary" @click="page.current=1;loadData()" style="margin-left: 8px;float:left">查询</Button>
            <Button type="ghost" style="margin-left: 8px;float:left" @click="handleResetForm">重置</Button>
            <Button type="success" style="margin-left: 8px;float:left" @click="modal.edit=false;modal.visible = true;modal.title = '新增大众点评美容团购';">新增</Button>
            <Upload action="/dianping/upload?type=groupconfig" 
                :format="['xlsx','xls']" :on-format-error="handleFormatError" :max-size="200"
                :on-success="uploadSuccess"
                style="margin-left: 8px;display: inline;float:left">
                <Button type="warning" icon="ios-cloud-upload-outline">批量导入</Button>
            </Upload>
            <a style="margin-left: 8px;margin-top:7px;display: inline;float:left" href="/Dianping/ExportSample?type=groupconfig" target="_blank">导出模板文件</a>
        </Row>
    </Form>
    <Table border :loading="table.loading" :columns="table.columns" :data="table.data"></Table>
    <div style="margin-top:15px;float:right">
        <Page  :total="page.total" :page-size="page.pageSize" :current="page.current" :page-size-opts="[20 ,50 ,100]" show-elevator show-sizer @on-change="handlePageChange" @on-page-size-change="handlePageSizeChange"></Page>
    </div>
    <Modal
        v-model="modal.visible"
        :loading="modal.loading"
        :title="modal.title"
        okText="提交"
        cancelText="取消"
        @on-ok="ok"
        @on-cancel="cancel">
        <Form ref="modal.gpConfig" :model="modal.gpConfig" :rules="modal.rules" label-position="left" :label-width="150">
            <FormItem label="点评团购ID" prop="DianpingGroupId">
                <Input v-model="modal.gpConfig.DianpingGroupId" :disabled="modal.edit" placeholder=""/>
            </FormItem>
            <FormItem label="点评品牌名称" prop="DianpingBrand">
                <Input v-model="modal.gpConfig.DianpingBrand" placeholder=""/>
            </FormItem>
            <FormItem label="点评团购名称" prop="DianpingTuanName">
                <Input v-model="modal.gpConfig.DianpingTuanName" placeholder=""/>
            </FormItem>
            <FormItem label="途虎服务产品ID" prop="TuhuProductId">
                <Input v-model="modal.gpConfig.TuhuProductId" placeholder="" @on-change="setProductName"/>
            </FormItem>
            <FormItem label="途虎服务产品名称" prop="TuhuProductName">
                <Input v-model="modal.gpConfig.TuhuProductName" :disabled="true" placeholder=""/>
            </FormItem>
            <FormItem label="途虎服务状态">
                <Select v-model="modal.gpConfig.TuhuProductStatus">
                    <Option v-for="item in select_data" :value="item.value" :key="item.value">{{ item.text }}</Option>
                </Select>
            </FormItem>
        </Form>
    </Modal>
    <Modal
        v-model="deleteModal.visible"
        title="删除"
        :loading="deleteModal.loading"
        @on-ok="deleteok">
        <p>确认删除？</p>
    </Modal>
  </div>
</template>

<script>
const defaultformdata = {
    dianpingid: "",
    dianpingbrand: "",
    dianpingname: "",
    tuhupid: "",
    tuhustatus: "-1"
};
const defaultGroupConfig = {
    PKID: 0,
    DianpingGroupId: "",
    DianpingBrand: "",
    DianpingTuanName: "",
    TuhuProductId: "",
    TuhuProductName: "",
    TuhuProductStatus: 0
};
export default {
  data () {
      return {
        select_data: [
            {"value": 1, "text": "已上架"},
            {"value": 0, "text": "已下架"}
        ],
        search_data: {},
        page: {
            total: 0,
            current: 1,
            pageSize: 20
        },
        table: {
            loading: true,
            data: [],
            columns: [
                {
                    title: '点评团购ID',
                    key: 'DianpingGroupId'
                },
                {
                    title: '点评品牌名称',
                    key: 'DianpingBrand'
                },
                {
                    title: '点评团购名称',
                    key: 'DianpingTuanName'
                },
                {
                    title: '途虎服务产品ID',
                    key: 'TuhuProductId'
                },
                {
                    title: '途虎服务产品名称',
                    key: 'TuhuProductName'
                },
                {
                    title: '途虎服务状态',
                    key: 'Status',
                    render: (h, params) => {
                        return h('span', {}, params.row.TuhuProductStatus === 1 ? "已上架" : "未上架");
                    }
                },
                {
                    title: '操作',
                    key: 'action',
                    width: 150,
                    align: 'center',
                    render: (h, params) => {
                        return h('div', [
                            h('Button', {
                                props: {
                                    type: 'primary',
                                    size: 'small'
                                },
                                style: {
                                    marginRight: '5px'
                                },
                                on: {
                                    click: () => {
                                        this.modal.visible = true;
                                        this.modal.edit = true;
                                        this.modal.gpConfig = this.util.deepCopy(this.table.data[params.index]);
                                        this.modal.title = "编辑大众点评美容团购";
                                    }
                                }
                            }, '编辑'),
                            h('Button', {
                                props: {
                                    type: 'error',
                                    size: 'small'
                                },
                                on: {
                                    click: () => {
                                        this.deleteModal.dianpingGroupId = this.table.data[params.index].DianpingGroupId;
                                        this.deleteModal.visible = true;
                                    }
                                }
                            }, '删除')
                        ]);
                    }
                }
            ]
        },
        deleteModal: {
            visible: false,
            loading: true,
            dianpingGroupId: ""
        },
        modal: {
            visible: false,
            loading: true,
            edit: true,
            title: "",
            gpConfig: {
                PKID: 0,
                DianpingGroupId: "",
                DianpingBrand: "",
                DianpingTuanName: "",
                TuhuProductId: "",
                TuhuProductName: "",
                TuhuProductStatus: 0
            },
            rules: {
                DianpingGroupId: [
                    {
                        required: true,
                        message: "请输入点评团购ID",
                        trigger: "blur"
                    }
                ],
                DianpingBrand: [
                    {
                        required: true,
                        message: "点评品牌名称",
                        trigger: "blur"
                    }
                ],
                DianpingTuanName: [
                    { required: true, message: '点评团购名称', trigger: 'blur' }
                ],
                TuhuProductId: [
                    {
                    required: true,
                    message: "请输入途虎服务产品ID",
                    trigger: "blur"
                    }
                ],
                TuhuProductName: [
                    {
                    required: true,
                    message: "请输入正确的服务Id",
                    trigger: "blur"
                    }
                ]
            }
        }
    }
  },
  created () {
    this.search_data = Object.assign({}, defaultformdata);
    this.loadData();
  },
  methods: {
    loadData () {
      this.table.loading = true;
      this.ajax.get('/dianping/getgroupconfigs', { 
          params: {
              ...this.search_data, 
              pageIndex: this.page.current, 
              pageSize: this.page.pageSize
            } 
        })
      .then((response) => {
        var data = response.data;
        this.page.total = data.totalCount;
        this.page.current = data.pageIndex;
        this.table.data = data.data;
        this.table.loading = false;
      });
    },
    handlePageChange (pageIndex) {
        this.page.current = pageIndex;
        this.loadData();
    },
    handlePageSizeChange (pageSize) {
        this.page.pageSize = pageSize;
        this.loadData();
    },
    handleFormatError (file) {
        console.log(123);
        this.util.message.warning({
            content: file.name + ' 格式错误，必须是xls或xlsx文件',
            duration: 3,
            closable: true
        });
    },
    handleResetForm () {
        this.search_data = Object.assign({}, defaultformdata);
    },
    setProductName () {
        if (this.modal.gpConfig.TuhuProductId) {
            this.ajax.get("/Dianping/GetTuhuProductName?tuhuProductId=" + this.modal.gpConfig.TuhuProductId)
            .then((response) => {
                if (response.data.name) {
                    this.modal.gpConfig.TuhuProductName = response.data.name;
                } else {
                    this.modal.gpConfig.TuhuProductName = "";
                }
            });
        }
    },
    ok () {
        this.modal.loading = true;
        this.$refs['modal.gpConfig'].validate((valid) => {
            if (valid) {
              console.log(this.$http);
              var isUpdate = false;
              if (this.modal.gpConfig.PKID > 0) {
                isUpdate = true;
              }
              this.ajax.post(isUpdate ? '/Dianping/UpdateGroupConfig' : "/Dianping/InsertGroupConfig", this.modal.gpConfig
              ).then((response) => {
                  console.log(response);
                  if (response.data.success) {
                    setTimeout(() => {
                        this.$Message.success(isUpdate ? '更新成功' : '添加成功');
                        this.loadData();
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
        this.ajax.post('/Dianping/DeleteGroupConfig', {"dianpingGroupId": this.deleteModal.dianpingGroupId}
              ).then((response) => {
                  console.log(response);
                  if (response.data.success) {
                    setTimeout(() => {
                        this.$Message.success('删除成功');
                        this.loadData();
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
                        this.$Message.success('上传成功');
                        this.loadData();
                    }, 2000);
        } else {
            this.$Message.error({"content": res.msg, "duration": 0, "closable": true});
        }
    }
  }
}
</script>

<style lang="less">
</style>
