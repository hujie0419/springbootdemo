<template>
  <div>
    <h1 class="title">企业客户专享配置</h1>
   
     <Row style="margin-bottom:15px;">
        <Button type="success" style="margin-left: 8px;float:left" @click="modal.edit=false;modal.visible = true;modal.title = '企业客户专享优惠新增';">新增</Button>
    </Row>

    <Table  ref="shoptable" :loading="table.loading" :columns="table.columns" :data="table.data"></Table>
    <div style="margin-top:15px;">
        <Page style="float:right"  :total="page.total" :page-size="page.pageSize" :current="page.current" :page-size-opts="[10,20 ,50 ,100]" show-elevator show-sizer @on-change="handlePageChange" @on-page-size-change="handlePageSizeChange"></Page>
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
            <FormItem label="活动专享ID">
                <label>{{modal.shopConfig.ActivityExclusiveId}}</label>
            </FormItem>
            <FormItem label="订单渠道" prop="OrderChannel">
              <Select v-model="modal.shopConfig.OrderChannel">
                    <Option v-for="item in oc_select_data" :value="item.value" :key="item.value">{{ item.text }}</Option>
             </Select>     

            </FormItem>
            <FormItem label="大客户名称" prop="LargeCustomersID">
              <Select v-model="modal.shopConfig.LargeCustomersID">
                    <Option v-for="item in lc_select_data" :value="item.value" :key="item.value">{{ item.text }}</Option>
                </Select>     
            </FormItem>
            <FormItem label="活动页链接" prop="EventLink">
                <Input v-model="modal.shopConfig.EventLink" placeholder=""/>
            </FormItem>
            <FormItem label="限时抢购ID" prop="ActivityId">
                <Input v-model="modal.shopConfig.ActivityId"  placeholder=""/>
            </FormItem>
            <FormItem  label="首页广告" >
                <Col span="4" v-show="modal.shopConfig.ImageUrl!=''">
                    <a :href="modal.shopConfig.ImageUrl" target="_blank"><img :src="modal.shopConfig.ImageUrl" style='width:50px;height:50px'></a>
                </Col>
                <Col span="6">
                    <Upload action="/GroupBuyingV2/UploadImage?type=image" :format="['jpg','jpeg','png']" :on-format-error="handleFormatError" :max-size="1000" :on-exceeded-size="handleMaxSize" :on-success="handleSuccess" :show-upload-list="false">
                        <Button type="ghost" icon="ios-cloud-upload-outline">Upload files</Button>
                    </Upload>
                </Col>
                <Col span="5"  style="margin-left:30px;" v-show="modal.shopConfig.ImageUrl!=''">
                    <Button type="warning" icon="refresh" @click="modal.shopConfig.ImageUrl=''">清除</Button>
                </Col>
            </FormItem>
             <FormItem label="企业热线" prop="BusinessHotline">
                <Input v-model="modal.shopConfig.BusinessHotline"  placeholder=""/>
            </FormItem>
             <FormItem label="是否启用" prop="IsEnable">
              <RadioGroup v-model="modal.shopConfig.IsEnable">
                <Radio label="1">启用</Radio>
                <Radio label="0" >禁用</Radio>
            </RadioGroup>
             
            </FormItem>
        </Form>
    </Modal>
    <Modal v-model="logmodal.visible" title="操作日志" cancelText="取消" @on-cancel="cancel" :width="logmodal.width">
            <Table :loading="logmodal.loading" :data="logmodal.data" :columns="logmodal.columns" stripe></Table>
    </Modal>
  </div>
</template>

<script>

const defaultShopConfig = {
        PKID: 0,
        ActivityExclusiveId: "保存后自动生成",
        OrderChannel: "",
        LargeCustomersID: "",
        LargeCustomersName: "",
        EventLink: "",
        ActivityId: "",
        ImageUrl: "",
        BusinessHotline: "",
        IsEnable: "1"
};

export default {
  data () {
      return {
        oc_select_data: [
            {value: "j锦湖员工胎", text: " j锦湖员工胎"},
            {value: "r马牌员工胎", text: "r马牌员工胎"}
        ],
        lc_select_data: [],
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
                  title: " ",
                  width: 50,
                  align: "center",
                  fixed: "left",
                  type: "index"
                },
                {
                    title: '活动专享ID',
                    key: 'ActivityExclusiveId',
                    align: 'center'
                },
                {
                    title: '订单渠道',
                    key: 'OrderChannel',
                    align: 'center'
                },
                {
                    title: '大客户名称',
                    key: 'LargeCustomersName',
                     align: 'center'
                },
                {
                    title: '活动链接',
                    key: 'EventLink',
                     align: 'center',
                     render: (h, params) => {
                        return h('a', {domProps: { href: params.row.EventLink, target: "_blank" }}, params.row.EventLink);
                    }
                },
                {
                    title: '限时抢购ID',
                    key: 'ActivityId',
                    align: 'center'
                },
                {
                    title: '首页广告',
                    key: 'ImageUrl',
                    align: 'center',
                    render: (h, params) => {
                        return h('img', {domProps: { src: params.row.ImageUrl, width: "75", height: "80" }}, params.row.ImageUrl);
                    }
                },
                {
                    title: '企业热线',
                    key: 'BusinessHotline',
                    align: 'center'
                },
                 {
                    title: '登录页链接',
                    align: 'center',
                     render: (h, params) => {
                        var hrefLike = "https://wx.tuhu.cn/enterpriseCustomer/index?CorporateID=" + params.row.ActivityExclusiveId;
                        return h('a', {domProps: { href: hrefLike, target: "_blank" }}, hrefLike);
                    }
                },
                 {
                    title: '是否启用',
                    key: 'IsEnable',
                    align: 'center',
                    render: (h, params) => {
                        return h('span', {}, params.row.IsEnable === "True" ? "启用" : "禁用");
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
                                        this.modal.shopConfig.IsEnable = this.modal.shopConfig.IsEnable === "True" ? "1" : "0";
                                        this.modal.shopConfig.LargeCustomersID = this.modal.shopConfig.LargeCustomersID + "";
                                        this.modal.title = "企业客户专享优惠编辑";
                                    }
                                }
                            }, '编辑'),
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
                                },
                                    "日志"
                                ),
                            h(
                                "router-link",
                                {
                                    attrs: {
                                    to: "customers-coupon/" + params.row.PKID + "/" + params.row.ActivityExclusiveId
                                    }
                                },
                                "  查看详情"
                            )
                        ]);
                    }
                }
            ]
        },
        deleteModal: {
            visible: false,
            loading: true,
            dianpingId: ""
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
            homeImgLab: "*首页广告",
            shopConfig: {
                PKID: 0,
                ActivityExclusiveId: "保存后自动生成",
                OrderChannel: "",
                LargeCustomersID: "",
                LargeCustomersName: "",
                EventLink: "",
                ActivityId: "",
                ImageUrl: "",
                BusinessHotline: "",
                IsEnable: "1"
            },
            rules: {
                OrderChannel: [
                    {
                        required: true,
                        message: "请选择订单渠道",
                        trigger: "change"
                    }
                ],
                LargeCustomersID: [
                    {
                        required: true,
                        message: "请选择大客户",
                        trigger: "change"
                    }
                ],
                EventLink: [
                    {
                    required: true,
                    message: "请输入活动链接地址",
                    trigger: "blur"
                    }
                ],
                 ActivityId: [
                    {
                    required: true,
                    message: "请输入限时抢购ID",
                    trigger: "blur"
                    }
                ],
                 BusinessHotline: [
                    {
                    required: true,
                    message: "请输入企业热线",
                    trigger: "blur"
                    },
                     {
                          type: 'number',
                          message: '请输入数字格式', 
                          trigger: 'blur', 
                          transform (value) {
                           return Number(value);
                          }
                    }
                ],
                 IsEnable: [
                    {
                    required: true,
                    message: "请选择是否启用",
                    trigger: "change"
                    }
                ]
            }
        }
    }
  },
  created () {
    this.loadData(1);
    this.loadSource();
  },
  methods: {
    loadData (pageIndex) {
      this.table.loading = true;
      this.page.current = pageIndex;
      this.ajax.get('/customersactivity/getcustomerexclusivesettings', { 
          params: {
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
    loadSource () {
      this.ajax.get('/customersactivity/getcompanyinfodict')
      .then((response) => {
        var data = response.data;
        this.lc_select_data = data.data;
      });
    },
    ok () {
        this.modal.loading = true;
        this.$refs['modal.shopConfig'].validate((valid) => {
            if (valid) {
              if (typeof (this.modal.shopConfig.ImageUrl) === "undefined" || this.modal.shopConfig.ImageUrl === "") {
                   this.$Message.error("请上传首页广告图片");
                    this.modal.loading = false;
                    this.$nextTick(() => {
                        this.modal.loading = true;
                    });
                   return;
              }  
              
              var isUpdate = false;
              if (this.modal.shopConfig.PKID > 0) {
                isUpdate = true;
              }

              var customersName = this.getLargeCustomersName(this.lc_select_data, this.modal.shopConfig.LargeCustomersID);
              this.modal.shopConfig.LargeCustomersName = customersName;

              this.ajax.post(isUpdate ? '/customersactivity/UpdateCustomerExclusiveSetting' : "/customersactivity/InsertCustomerExclusiveSetting", this.modal.shopConfig
              ).then((response) => {
                  console.log(response);
                  if (response.data.success) {
                    setTimeout(() => {
                        this.$Message.success(isUpdate ? '更新成功' : '添加成功');
                        if (isUpdate) {
                            this.loadData(this.page.current);
                        } else {
                             this.loadData(1);
                        }

                        this.modal.shopConfig = Object.assign({}, defaultShopConfig);
                        this.$refs['modal.shopConfig'].resetFields();
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
     handleFormatError (file) {
        this.$Message.warning("请选择 .jpg  or .png  or .jpeg图片");
     },
     handleMaxSize (file) {
        this.$Message.warning("请选择不超过100KB的图片");
     },
     handleSuccess (res, file) {
         if (res.Status) {
              this.modal.shopConfig.ImageUrl = res.ImageUrl
            } else {
                this.$Message.warning(res.Msg);
          }
     },
     SearchLog (pkid) {
            this.logmodal.loading = true;
            this.ajax
                .post("/customersactivity/getCustomerExclusiveSettingLogs", {
                    objeId: pkid,
                    source: "CustomerExclusiveSetting"
                })
                .then(response => {
                    this.logmodal.data = response.data;
                    this.logmodal.visible = true;
                    this.logmodal.loading = false;
                });
     },
     getLargeCustomersName (array, id) {
         var largeCustomersName = "";
           for (var i = 0; i < this.lc_select_data.length; i++) {
               if (this.lc_select_data[i].value === id) {
                   largeCustomersName = this.lc_select_data[i].text;
                   break;
               }
           }
        return largeCustomersName;
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
