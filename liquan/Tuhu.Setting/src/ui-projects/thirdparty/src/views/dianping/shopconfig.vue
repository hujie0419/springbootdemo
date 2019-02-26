<template>
  <div>
    <h1 class="title">门店维护</h1>
    <Form :model="search_data" :label-width="100">
        <Row>
            <Col span="6">
                <FormItem label="点评门店ID:">
                    <Input v-model="search_data.DianpingId" placeholder="点评门店ID"></Input>
                </FormItem>
            </Col>
            <Col span="6">
                <FormItem label="点评商户名:">
                    <Input v-model="search_data.DianpingName" placeholder="点评商户名"></Input>
                </FormItem></Col>
            <Col span="6">
                <FormItem label="点评分店名:">
                    <Input v-model="search_data.DianpingShopName" placeholder="点评分店名"></Input>
                </FormItem>
            </Col>
        </Row>
        <Row>
            <Col span="6">
                <FormItem label="途虎门店ID:">
                    <Input v-model="search_data.TuhuShopId" placeholder="途虎门店ID"></Input>
                </FormItem>
            </Col>
            <Col span="6">
                <FormItem label="点评团购:">
                    <Select v-model="search_data.GroupStatus">
                        <Option value="-1">全部</Option>
                        <Option value="1">已配置</Option>
                        <Option value="0">未配置</Option>
                    </Select>
                </FormItem>
            </Col>
            <Col span="6">
                <FormItem label="关联点评管家:">
                    <Select v-model="search_data.LinkStatus">
                        <Option value="-1">全部</Option>
                        <Option value="1">已关联</Option>
                        <Option value="0">未关联</Option>
                        <Option value="2">即将过期</Option>
                    </Select>
                </FormItem>
            </Col>
        </Row>
        <Row style="margin-bottom:15px;">
            <Button type="primary" @click="page.current=1;loadData()" style="margin-left: 8px;float:left">查询</Button>
            <Button type="ghost" style="margin-left: 8px;float:left" @click="handleResetForm">重置</Button>
            <Button type="success" style="margin-left: 8px;float:left" @click="modal.edit=false;modal.visible = true;modal.title = '新增大众点评门店';">新增</Button>
            <Upload action="/dianping/upload?type=shopconfig" 
                :format="['xlsx','xls']" :on-format-error="handleFormatError" :max-size="200"
                :on-success="uploadSuccess"
                style="margin-left: 8px;display: inline;float:left">
                <Button type="warning" icon="ios-cloud-upload-outline">批量导入</Button>
            </Upload>
            <a style="margin-left: 8px;margin-top:7px;display: inline;float:left" href="/Dianping/ExportSample?type=shopconfig" target="_blank">导出模板文件</a>
        </Row>
    </Form>
    <Table border ref="shoptable" :loading="table.loading" :columns="table.columns" :data="table.data" @on-selection-change="selectChange"></Table>
    <div style="margin-top:15px;">
      <Button type="success" style="margin-left: 8px;float:left" :disabled="selectedTableData == null || selectedTableData.length == 0" @click="link">关联点评管家</Button>
        <Page style="float:right"  :total="page.total" :page-size="page.pageSize" :current="page.current" :page-size-opts="[20 ,50 ,100]" show-elevator show-sizer @on-change="handlePageChange" @on-page-size-change="handlePageSizeChange"></Page>
    </div>
    <Modal
        v-model="modal.visible"
        :loading="modal.loading"
        :title="modal.title"
        okText="提交"
        cancelText="取消"
        @on-ok="ok"
        @on-cancel="cancel">
        <Form ref="modal.shopConfig" :model="modal.shopConfig" :rules="modal.rules" label-position="left" :label-width="150">
            <FormItem label="点评门店ID" prop="DianpingId">
                <Input v-model="modal.shopConfig.DianpingId" :disabled="modal.edit" placeholder=""/>
            </FormItem>
            <FormItem label="点评商户名" prop="DianpingName">
                <Input v-model="modal.shopConfig.DianpingName" placeholder=""/>
            </FormItem>
            <FormItem label="点评分店名" prop="DianpingShopName">
                <Input v-model="modal.shopConfig.DianpingShopName" placeholder=""/>
            </FormItem>
            <FormItem label="途虎门店ID" prop="TuhuShopId">
                <Input v-model="modal.shopConfig.TuhuShopId" placeholder="" @on-change="setShopName"/>
            </FormItem>
            <FormItem label="途虎门店名称" prop="TuhuShopName">
                <Input v-model="modal.shopConfig.TuhuShopName" :disabled="true" placeholder=""/>
            </FormItem>
            <FormItem label="点评团购" prop="GroupStatus">
                <Select v-model="modal.shopConfig.GroupStatus">
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
    DianpingId: "",
    DianpingName: "",
    DianpingShopName: "",
    TuhuShopId: "",
    GroupStatus: "-1",
    LinkStatus: "-1"
};
const defaultShopConfig = {
    PKID: 0,
    DianpingId: "",
    DianpingName: "",
    DianpingShopName: "",
    TuhuShopId: "",
    TuhuShopName: "",
    GroupStatus: 0
};
export default {
  data () {
      return {
        select_data: [
            {"value": 1, "text": "已配置"},
            {"value": 0, "text": "未配置"}
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
                  type: 'selection',
                  width: 60,
                  align: 'center'
                },
                {
                    title: '点评门店ID',
                    key: 'DianpingId'
                },
                {
                    title: '点评商户名',
                    key: 'DianpingName'
                },
                {
                    title: '点评分店名',
                    key: 'DianpingShopName'
                },
                {
                    title: '途虎门店ID',
                    key: 'TuhuShopId'
                },
                {
                    title: '途虎门店名称',
                    key: 'TuhuShopName'
                },
                {
                    title: '点评团购',
                    key: 'GroupStatus',
                    render: (h, params) => {
                        return h('span', {}, params.row.GroupStatus === 1 ? "已配置" : "未配置");
                    }
                },
                {
                    title: '关联点评管家',
                    key: 'LinkStatus',
                    render: (h, params) => {
                        return h('span', {}, params.row.Session !== null ? params.row.Session.RefreshToken ? "已关联" : "即将过期" : "未关联");
                    }
                },
                {
                    title: '操作',
                    key: 'action',
                    width: 200,
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
                                        this.modal.shopConfig = this.util.deepCopy(this.table.data[params.index]);
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
                                        this.deleteModal.dianpingId = this.table.data[params.index].DianpingId;
                                        this.deleteModal.visible = true;
                                    }
                                }
                            }, '删除'),
                            h('Button', {
                                props: {
                                    type: 'warning',
                                    size: 'small',
                                    disabled: params.row.Session == null || (params.row.Session.RefreshToken !== "" && params.row.Session.RefreshToken != null)
                                },
                                style: {
                                    marginLeft: '5px'
                                },
                                on: {
                                    click: () => {
                                        this.reAuth(this.table.data[params.index]);
                                    }
                                }
                            }, '重新授权')
                        ]);
                    }
                }
            ]
        },
        selectedTableData: [],
        deleteModal: {
            visible: false,
            loading: true,
            dianpingId: ""
        },
        modal: {
            visible: false,
            loading: true,
            edit: true,
            title: "",
            shopConfig: {
                PKID: 0,
                DianpingId: "",
                DianpingName: "",
                DianpingShopName: "",
                TuhuShopId: "",
                TuhuShopName: "",
                GroupStatus: 0
            },
            rules: {
                DianpingId: [
                    {
                        required: true,
                        message: "请输入点评门店ID",
                        trigger: "blur"
                    }
                ],
                DianpingName: [
                    {
                        required: true,
                        message: "请输入点评点评商户名",
                        trigger: "blur"
                    }
                ],
                DianpingShopName: [
                    { required: true, message: '请输入点评分店名', trigger: 'blur' }
                ],
                TuhuShopId: [
                    {
                      validator: (rule, value, callback) => {
                          if (value === '' || isNaN(value)) {
                              callback(new Error('请输入途虎门店数字ID'));
                          } else {
                              callback();
                          }
                      },
                      trigger: "blur"
                    }
                ],
                TuhuShopName: [
                    {
                    required: true,
                    message: "请输入正确的途虎门店ID",
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
      this.ajax.get('/dianping/getshopconfigs', { 
          params: {
              ...this.search_data, 
              pageIndex: this.page.current, 
              pageSize: this.page.pageSize
            } 
        })
      .then((response) => {
        var data = response.data;
        // data.data.forEach(config => {
        //  if (config != null && config.Session != null && config.Session.RefreshToken) {
        //    config._disabled = true;
        //  }
        // });
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
    setShopName () {
        if (this.modal.shopConfig.TuhuShopId && !isNaN(this.modal.shopConfig.TuhuShopId)) {
            this.ajax.get("/Dianping/GetTuhuShopName?tuhuShopId=" + this.modal.shopConfig.TuhuShopId)
            .then((response) => {
                if (response.data.name) {
                    this.modal.shopConfig.TuhuShopName = response.data.name;
                } else {
                    this.modal.shopConfig.TuhuShopName = "";
                }
            });
        }
    },
    ok () {
        this.modal.loading = true;
        this.$refs['modal.shopConfig'].validate((valid) => {
          console.log(valid);
            if (valid) {
              console.log(this.$http);
              var isUpdate = false;
              if (this.modal.shopConfig.PKID > 0) {
                isUpdate = true;
              }
              this.ajax.post(isUpdate ? '/Dianping/UpdateShopConfig' : "/Dianping/InsertShopConfig", this.modal.shopConfig
              ).then((response) => {
                  console.log(response);
                  if (response.data.success) {
                    setTimeout(() => {
                        this.$Message.success(isUpdate ? '更新成功' : '添加成功');
                        this.loadData();
                        this.modal.shopConfig = Object.assign({}, defaultShopConfig);
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
      this.modal.shopConfig = Object.assign({}, defaultShopConfig);
      this.$refs['modal.shopConfig'].resetFields();
    },
    deleteok () {
        this.deleteModal.loading = true;
        this.ajax.post('/Dianping/DeleteShopConfig', {"dianpingId": this.deleteModal.dianpingId}
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
    },
    selectChange (selected) {
      this.selectedTableData = selected;
    },
    get () {
      var list = [];
      this.selectedTableData.forEach(item => {
        list.push({
          "shopAddress": item.TuhuShopAddress,
          "shopName": item.TuhuShopName,
          "shopId": item.TuhuShopId
        });
      });
      var appKey = "075c10a2cb7be033";
      var requestModules = [5];
      var shopList = [
      {        
        "shopAddress": "莘松路1260号(亚繁生活时尚广场隔壁)",
        "shopId": "117271",
        "shopName": "途虎养车工场店 莘松路店"
      }];
      var url = 'https://e.dianping.com/open/merchant/auth?appKey=' + appKey + '&requestModules=' + 
        encodeURIComponent(JSON.stringify(requestModules)) + '&shopList=' + 
        encodeURIComponent(JSON.stringify(shopList));
      window.open(url);
    },
    link () {
      var tempform = document.createElement("form");  
      tempform.action = "https://e.dianping.com/open/merchant/auth";  
      tempform.target = "_blank";  
      tempform.method = "post";  
      tempform.style.display = "none";
      var list = [];
      this.selectedTableData.forEach(item => {
        list.push({
          "shopAddress": item.TuhuShopAddress,
          "shopId": item.TuhuShopId,
          "shopName": item.TuhuShopName
        });
      });

      var data = [
        {key: "appKey", value: "075c10a2cb7be033"},
        {key: "requestModules", value: JSON.stringify([1, 3, 5, 6, 8])},
        {key: "shopList", value: JSON.stringify(list)}
      ];
      
      data.forEach(item => {
          var opt = document.createElement("textarea");  
          opt.name = item.key;  
          // opt.value = encodeURIComponent(item.value);  
          opt.value = item.value; 
          tempform.appendChild(opt); 
      })
          
      document.body.appendChild(tempform);  
      console.log(tempform);
      tempform.submit();
    },
    reAuth (shopConfig) {
      var session = shopConfig.Session.Session;
      var shopId = shopConfig.TuhuShopId;
      var url = "https://e.dianping.com/open/merchant/reauthorize?app_key=075c10a2cb7be033&app_shopid=" + 
        shopId + "&session=" + session;
      window.open(url);
    }
  }
}
</script>

<style lang="less">
</style>
