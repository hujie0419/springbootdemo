<style>
.ivu-table .table-module-row td{
    background-color: #fcfcfd;
    /* color: #fff; */
}
/* .ivu-table .table-module-content-row td{
    background-color: #e5f2fa;
    color: #fff;
} */
</style>
<template>
  <div>
    <Button type="success" @click="moduleModalShow">添加模块</Button>
    <div style="margin-top:5px;">
      <Table :row-class-name="rowClassName" border strip :loading="tableModal.loading" :columns="tableModal.columns" :data="tableModal.list" size="small" no-data-text="暂无数据"></Table>
    </div>
    <Modal v-model="moduleModal.visible" title="模块配置" :transfer="false" width="40%">
      <div slot="footer">
        <Button type="text" size="large" @click="moduleModalCancel('moduleModal.MemberPageModule')">取消</Button>
        <Button type="primary" size="large" :loading="moduleModal.loading" @click="moduleModalOk('moduleModal.MemberPageModule')">提交</Button>
      </div>
      <Form ref="moduleModal.MemberPageModule" :model="moduleModal.MemberPageModule" :rules="moduleModal.rules" :label-width="80">
        <FormItem label="模块标识" prop="MemberPageModuleID" v-if="false">
          <Input v-model="moduleModal.MemberPageModule.PKID" disabled="true" placeholder="自动生成" />
        </FormItem>
        <FormItem label="模块名称" prop="ModuleName">
          <Input v-model="moduleModal.MemberPageModule.ModuleName" placeholder="模块名称" />
        </FormItem>
        <FormItem label="模块类型" prop="ModuleType">
          <Select v-model="moduleModal.MemberPageModule.ModuleType" style="width:30%">
            <Option v-for="item in moduleTypeList" :value="item.key" :key="item.key" >{{item.value}}</Option>
          </Select>
        </FormItem>
        <FormItem label="上边距" prop="MarginTop">
          <InputNumber v-model="moduleModal.MemberPageModule.MarginTop" number></InputNumber>
        </FormItem>
        <FormItem label="显示顺序" prop="DisplayIndex">
          <InputNumber v-model="moduleModal.MemberPageModule.DisplayIndex"></InputNumber>
        </FormItem>
        <FormItem label="状态" prop="Status">
          <i-switch v-model="moduleModal.MemberPageModule.Status" size="large">
            <span slot="open">启用</span>
            <span slot="close">禁用</span>
          </i-switch>
        </FormItem>
      </Form>
    </Modal>  
    <Modal v-model="modal.visible" title="模块内容配置" :transfer="false" width="40%">
      <div slot="footer">
        <Button type="text" size="large" @click="modalCancel('modal.MemberPageModuleContent')">取消</Button>
        <Button type="primary" size="large" :loading="modal.loading" @click="modalOk('modal.MemberPageModuleContent')">提交</Button>
      </div>
      <Form ref="modal.MemberPageModuleContent" :model="modal.MemberPageModuleContent" :rules="modal.rules" :label-width="120">
        <FormItem label="模块标识" prop="MemberPageModuleID" v-if="false">
          <Input v-model="modal.MemberPageModuleContent.MemberPageModuleID" placeholder="模块标识" disabled="true" />
        </FormItem>
        <FormItem label="内容标识" prop="PKID" v-if="false">
          <Input v-model="modal.MemberPageModuleContent.PKID" placeholder="内容标识" disabled="true" />
        </FormItem>
        <FormItem label="状态" prop="Status">
          <i-switch v-model="modal.MemberPageModuleContent.Status" size="large">
            <span slot="open">启用</span>
            <span slot="close">禁用</span>
          </i-switch>
        </FormItem>
        <FormItem label="内容名称" prop="ContentName">
          <Input v-model="modal.MemberPageModuleContent.ContentName" placeholder="内容名称" />
        </FormItem>
        <FormItem label="展示类型" prop="ShowType">
          <Select v-model="modal.MemberPageModuleContent.ShowType">
            <Option v-for="item in showTypeList" :value="item.key" :key="item.key" >{{item.value}}</Option>
          </Select>
        </FormItem>
        <FormItem label="数据类型" prop="DataType">
          <Select v-model="modal.MemberPageModuleContent.DataType">
            <Option v-for="item in dataTypeList" :value="item.key" :key="item.key" >{{item.value}}</Option>
          </Select>
        </FormItem>
        <FormItem label="图片" prop="ImageUrl">
          <Row>
            <Col span="4">
              <img :src="modal.MemberPageModuleContent.ImageUrl" style='width:50px;height:50px'>
            </Col>
            <Col span="4">
              <Upload action="/MemberPage/UploadImage?type=imageUrl" :format="['jpg','jpeg','png']" :on-format-error="uploadFormatError" :max-size="10000" :on-exceeded-size="uploadMaxSize" :on-success="uploadSuccess" :show-upload-list="false">
                <Button type="ghost" icon="ios-cloud-upload-outline">上 传</Button>
              </Upload>
            </Col>
          </Row>
        </FormItem>
        <FormItem label="数字颜色" prop="NumColor">
          <Input v-model="modal.MemberPageModuleContent.NumColor" placeholder="数字颜色" />
        </FormItem>
        <FormItem label="是否启用角标/红点" prop="IsEnableCornerMark">
          <i-switch v-model="modal.MemberPageModuleContent.IsEnableCornerMark" size="large">
            <span slot="open">是</span>
            <span slot="close">否</span>
          </i-switch>
        </FormItem>
        <FormItem label="标题" prop="Title">
          <Input v-model="modal.MemberPageModuleContent.Title" placeholder="标题" />
        </FormItem>
        <FormItem label="标题颜色" prop="TitleColor">
          <Input v-model="modal.MemberPageModuleContent.TitleColor" placeholder="标题颜色" />
        </FormItem>
        <FormItem label="描述" prop="Description">
          <Input v-model="modal.MemberPageModuleContent.Description" placeholder="描述" />
        </FormItem>
        <FormItem label="描述颜色" prop="DescriptionColor">
          <Input v-model="modal.MemberPageModuleContent.DescriptionColor" placeholder="描述颜色" />
        </FormItem>
        <FormItem label="跳转提示文案" prop="ButtonText">
          <Input v-model="modal.MemberPageModuleContent.ButtonText" placeholder="跳转提示文案" />
        </FormItem>
        <FormItem label="跳转提示文案颜色" prop="ButtonTextColor">
          <Input v-model="modal.MemberPageModuleContent.ButtonTextColor" placeholder="跳转提示文案颜色" />
        </FormItem>
        <FormItem label="跳转按钮颜色" prop="ButtonColor">
          <Input v-model="modal.MemberPageModuleContent.ButtonColor" placeholder="跳转按钮颜色" />
        </FormItem>
        <FormItem label="背景颜色" prop="BgColor">
          <Input v-model="modal.MemberPageModuleContent.BgColor" placeholder="背景颜色" />
        </FormItem>
        <FormItem label="APP开始版本" prop="StartVersion">
          <Input v-model="modal.MemberPageModuleContent.StartVersion" placeholder="APP开始版本" />
        </FormItem>
        <FormItem label="APP结束版本" prop="EndVersion">
          <Input v-model="modal.MemberPageModuleContent.EndVersion" placeholder="APP结束版本" />
        </FormItem>
        <FormItem label="展示渠道" prop="SupportedChannels">
         <Checkbox v-for="x in channelModel" v-model="x.checked" :key="x.key">{{x.value}}</Checkbox>
        </FormItem>
        <FormItem v-for="x in channelModel" :label="x.value" :key="x.key" v-if="x.checked">
          <Input v-model="x.link" placeholder="请输入跳转链接" />
        </FormItem>
        <FormItem label="显示顺序" prop="DisplayIndex">
          <InputNumber v-model="modal.MemberPageModuleContent.DisplayIndex"></InputNumber>
        </FormItem>
      </Form>
    </Modal>
    <Modal v-model="logModal.visible" title="操作日志" cancelText="取消" width="785" >
        <Table :loading="logModal.loading"  :data="logModal.data" :columns="logModal.columns" stripe no-data-text="暂无数据"></Table>
    </Modal>
  </div>
</template>
<script>
import util from "@/framework/libs/util";
export default {
  name: "MemberPage",
  props: {
    pageCode: {
      type: String,
      default: "member"
    }
  },
  data () {
    return {
      tableModal: {
          list: [],
          loading: true,
          columns: [
            {
              title: "模块顺序",
              key: "ModuleDisplayIndex",
              width: 90,
              render: function (h, params) {
                switch (params.row.ModuleDisplayIndex) {
                  case 0:
                  return h("span", "-");
                  default:
                  return h("span", params.row.ModuleDisplayIndex);
                }
              }
            },
            {
              title: "模块名称",
              key: "ModuleName",
              width: 200,
              render: function (h, params) {
                switch (params.row.ModuleType) {
                  case 0:
                  return h("span", "-");
                  default:
                  return h("span", params.row.ModuleName);
                }
              }
            },
            {
              title: "模块类型",
              key: "ModuleType",
              width: 110,
              render: function (h, params) {
                switch (params.row.ModuleType) {
                  case 1:
                  return h("span", "个人信息");
                  case 2:
                  return h("span", "订单");
                  case 3:
                  return h("span", "猜你喜欢");
                  case 4:
                  return h("span", "车型信息");
                  case 5:
                  return h("span", "会员推荐任务");
                  case 6:
                  return h("span", "标题栏");
                  case 7:
                  return h("span", "底部栏");
                  case 8:
                  return h("span", "一行一列");
                  case 9:
                  return h("span", "一行两列");
                  case 10:
                  return h("span", "一行四列");
                  default:
                  return h("span", "-");
                }
              }
            },
            {
              title: "内容顺序",
              key: "DisplayIndex",
              width: 90,
              render: function (h, params) {
                switch (params.row.ModuleType) {
                  case 0:
                  return h("span", params.row.DisplayIndex);
                  default:
                  return h("span", "-");          
                }
              }
            },
            {
              title: "内容名称",
              key: "ContentName",
              width: 200,
              render: function (h, params) {
                switch (params.row.ModuleType) {
                  case 0:
                  return h("span", params.row.ContentName);
                  default:
                  return h("span", "-");          
                }
              }
            },
            {
              title: "展示类型",
              key: "ShowType",
              width: 100,
              render: function (h, params) {
                switch (params.row.ShowType) {
                  case 1:
                  return h("span", "图文导航");
                  case 2:
                  return h("span", "数字导航");
                  case 3:
                  return h("span", "图片广告");
                  case 4:
                  return h("span", "标题栏");
                  case 5:
                  return h("span", "底部栏");
                  default:
                  return h("span", "-");
                }
              }
            },
            {
              title: "数据类型",
              key: "DataType",
              width: 100,
              render: function (h, params) {
                switch (params.row.DataType) {
                  case 1:
                  return h("span", "普通");
                  case 2:
                  return h("span", "购物车");
                  case 3:
                  return h("span", "优惠券");
                  case 4:
                  return h("span", "收藏夹");
                  case 5:
                  return h("span", "浏览记录");
                  case 6:
                  return h("span", "积分");
                  case 7:
                  return h("span", "爱车档案");
                  default:
                  return h("span", "-");
                }
              }
            },
            {
              title: "状态",
              key: "Status",
              width: 70,
              render: function (h, params) {
                switch (params.row.Status) {
                  case true:
                  return h("span", "启用");
                  case false:
                  return h("span", "禁用");
                }
              }
            },
            {
              title: "展示渠道",
              key: "SupportedChannels",
              width: 240,
              render: function (h, params) {
                switch (params.row.ModuleType) {
                  case 0:
                  return h("span", params.row.SupportedChannels);
                  default:
                  return h("span", "-");          
                }
              }
            },
            {
              title: "APP开始版本",
              key: "StartVersion",
              width: 110,
              render: function (h, params) {
                switch (params.row.ModuleType) {
                  case 0:
                  return h("span", params.row.StartVersion);
                  default:
                  return h("span", "-");          
                }
              }
            },
            {
              title: "APP结束版本",
              key: "EndVersion",
              width: 112,
              render: function (h, params) {
                switch (params.row.ModuleType) {
                  case 0:
                  return h("span", params.row.EndVersion);
                  default:
                  return h("span", "-");          
                }
              }
            },
            {
                title: '操作',
                key: 'action',
                width: 230,
                fixed: 'right',
                render: (h, params) => {
                    let buttons = [];
                    buttons.push(h('Button', {
                            props: {
                                type: 'primary',
                                size: 'small'
                            },
                            style: {
                                marginRight: '3px'
                            },
                            on: {
                                click: () => {
                                    this.getMemberPageModuleInfo(params.row.PKID, params.row.MemberPageModuleID, params.row.ModuleType);
                                }
                            }
                        }, '编辑'),
                        h('Button', {
                            props: {
                                type: 'primary',
                                size: 'small'
                            },
                            style: {
                                marginRight: '3px'
                            },
                            on: {
                                click: () => {
                                  if (params.row.ModuleType === 0) {
                                    this.searchLog('MemberPageModuleContent', params.row.PKID); 
                                  } else {
                                    this.searchLog('MemberPageModule', params.row.MemberPageModuleID);
                                  }
                                }
                            }
                        }, '日志'));
                        if (params.row.ModuleType !== 1 && params.row.ModuleType !== 2 && params.row.ModuleType !== 3) {
                          buttons.push(h('Button', {
                            props: {
                                type: 'error',
                                size: 'small'
                            },
                            style: {
                                marginRight: '3px'
                            },
                            on: {
                                click: () => {
                                  this.deleteMemberPageModule(params.row.PKID, params.row.MemberPageModuleID, params.row.ModuleType);
                                }
                            }
                          }, '删除'));
                        }
                        // ModuleType>5为可动态添加模块内容项
                        if (params.row.ModuleType > 5) {
                          buttons.push(h('Button', {
                            props: {
                                type: 'success',
                                size: 'small'
                            },
                            on: {
                                click: () => {
                                    this.modalShow(params.row.MemberPageModuleID);
                                }
                            }
                          }, '添加内容'));
                        }
                    return h('div', buttons);
                }
            }
          ]
        },
      moduleTypeList: [
          {
            "key": 1,
            "value": "个人信息"
          }, {
            "key": 2,
            "value": "订单"
          }, {
            "key": 3,
            "value": "猜你喜欢"
          }, {
            "key": 4,
            "value": "车型信息"
          }, {
            "key": 5,
            "value": "会员推荐任务"
          }, {
            "key": 6,
            "value": "标题栏"
          }, {
            "key": 7,
            "value": "底部栏"
          }, {
            "key": 8,
            "value": "一行一列"
          }, {
            "key": 9,
            "value": "一行两列"
          }, {
            "key": 10,
            "value": "一行四列"
          }
        ],
      moduleTypeListForMore: [
          {
            "key": 6,
            "value": "标题栏"
          }, {
            "key": 7,
            "value": "底部栏"
          }, {
            "key": 8,
            "value": "一行一列"
          }, {
            "key": 9,
            "value": "一行两列"
          }, {
            "key": 10,
            "value": "一行四列"
          }
        ],  
      showTypeList: [
          {
            "key": 1,
            "value": "图文导航"
          }, {
            "key": 2,
            "value": "数字导航"
          }, {
            "key": 3,
            "value": "图片广告"
          }, {
            "key": 4,
            "value": "标题栏"
          }, {
            "key": 5,
            "value": "底部栏"
          }
        ],
      dataTypeList: [
          {
            "key": 1,
            "value": "普通"
          }, {
            "key": 2,
            "value": "购物车"
          }, {
            "key": 3,
            "value": "优惠券"
          }, {
            "key": 4,
            "value": "收藏夹"
          }, {
            "key": 5,
            "value": "浏览记录"
          }, {
            "key": 6,
            "value": "积分"
          }, {
            "key": 7,
            "value": "爱车档案"
          }
        ],
      modal: {
          visible: false,
          loading: false,
          edit: true,
          MemberPageModuleContent: {
          },
          rules: {
            ContentName: [
              {required: true, message: "内容名称不能为空", trigger: "blur"},
              {type: "string", max: 200, message: "不能超过200字", trigger: "blur"}
            ],
            ShowType: [
              {
                required: true,
                validator: (rule, value, callback) => {
                 if (!value) {
                   callback(new Error('请选择显示类型'));
                 } else {
                   callback();
                 }
                }
              }
            ],
            DataType: [
              {
                required: true
              }
            ],
            DisplayIndex: [
              {
                required: true,
                validator: (rule, value, callback) => {
                  if (!/^[a-z0-9]+$/.test(value)) {
                    callback(new Error("请输入0或正整数"));                      
                  } else {
                    callback();
                  }
                },
                trigger: 'change'                
              }
            ]
          }
        },
      moduleModal: {
          visible: false,
          loading: false,
          edit: false,
          MemberPageModule: {
          },
          rules: {
            ModuleName: [
              { required: true, message: "模块名称不能为空", trigger: 'blur' },
              { type: "string", max: 200, message: "不能超过200字", trigger: "blur" }
            ],
            ModuleType: [
              { required: true, 
                validator: (rule, value, callback) => {
                  if (!value) {
                      callback(new Error('请选择模块类型'));
                  } else {
                      callback();
                  }
                },
                trigger: 'change' 
              }
            ],
            MarginTop: [
              { required: true,
                validator: (rule, value, callback) => {
                  if (!/^[a-z0-9]+$/.test(value)) {
                    callback(new Error("请输入0或正整数"));                      
                  } else {
                    callback();
                  }
                },
                trigger: 'change'
              }
            ],
            DisplayIndex: [
              {
                required: true,
                validator: (rule, value, callback) => {
                  if (!/^[a-z0-9]+$/.test(value)) {
                    callback(new Error("请输入0或正整数"));                      
                  } else {
                    callback();
                  }
                },
                trigger: 'change'                
              }
            ]
          }
        },
      channelModel: [
          {
            "key": "IOS",
            "value": "IOS",
            "checked": false,
            "link": null
          },
          {
            "key": "Android",
            "value": "Andriod",
            "checked": false,
            "link": null
          },
          {
            "key": "TuhuMiniProgram",
            "value": "途虎养车小程序",
            "checked": false,
            "link": null
          },
          {
            "key": "GroupBuyMiniProgram",
            "value": "途虎拼团小程序",
            "checked": false,
            "link": null
          },
          {
            "key": "H5",
            "value": "H5移动站",
            "checked": false,
            "link": null
          },
          {
            "key": "XiaoMiQuickApp",
            "value": "小米快应用",
            "checked": false,
            "link": null
          },
          {
            "key": "HuaweiQuickApp",
            "value": "华为快应用",
            "checked": false,
            "link": null
          }
        ],
      logModal: {
          loading: true,
          visible: false,
          data: [],
          columns: [
              {
                  title: "操作人",
                  width: 150,
                  key: "Creator",
                  align: "left",
                  fixed: "left"
              },
              {
                  title: "时间",
                  width: 150,
                  key: "CreateDateTime",
                  align: "left",
                  fixed: "left"
              },
              {
                  title: "操作",
                  width: 150,
                  key: "Remark",
                  align: "left",
                  fixed: "left"
              },
              {
                  title: "改后数据",
                  width: 300,
                  key: "AfterValue",
                  align: "left",
                  fixed: "left"
              }
          ]
        }
    }
  },
  created () {
      this.init();
    },
  methods: {
    init () {
        this.tableModal.loading = false;
        util.ajax.get(`/MemberPage/GetMemberPageList?pageCode=` + this.pageCode)
            .then(response => {
                this.tableModal.list = response.data;
            });
      },
    rowClassName (row, index) {
        if (row.ModuleType === 0) {
            return 'table-module-content-row';
        } else {
            return 'table-module-row';
        }
      },
    getMemberPageModuleInfo (contentId, moduleId, moduleType) {
        this.channelModel.forEach(element => {
          element.checked = false;
        });
        this.$refs['modal.MemberPageModuleContent'].resetFields();
        this.$refs['moduleModal.MemberPageModule'].resetFields();
        util.ajax.get(`/MemberPage/GetMemberPageModuleInfo?contentId=${contentId}&moduleId=${moduleId}&moduleType=${moduleType}`)
        .then(response => {
          if (response.data) {
            if (moduleType === 0) {
              if (response.data.ImageUrl === null) {
                response.data.ImageUrl = "static/default.jpg";
              }
              this.modal.loading = false;
              this.modal.MemberPageModuleContent = response.data;
              this.channelModel = this.channelModel.map(c => {
                var list = this.modal.MemberPageModuleContent.ChannelList.filter(f => f.Channel === c.key)
                if (list.length) {
                  c.checked = true;
                  c.link = list[0].Link;
                }
                return c;
              });
              this.modal.visible = true;
            } else {
              this.moduleModal.loading = false;
              this.moduleModal.MemberPageModule = response.data;
              this.moduleModal.visible = true;
            }
          }
        });
      },
    modalShow (memberPageModuleID) {
        this.$refs['modal.MemberPageModuleContent'].resetFields();
        this.modal.MemberPageModuleContent = {
          ContentName: "",
          MemberPageModuleID: memberPageModuleID,
          ShowType: 1,
          DataType: 1,
          ImageUrl: "static/default.jpg",          
          DisplayIndex: 1,
          Status: true,
          PageCode: this.pageCode
        };
        this.channelModel.forEach(element => {
          element.checked = false;
          element.link = null;
        });
        this.modal.visible = true;
      },
    modalOk (name) {
      this.modal.loading = true;          
      this.$refs[name].validate((valid) => {
          if (valid) {
            var channelList = this.channelModel.filter(f => f.checked === true);
            var supportedChannels = channelList.map(c => c.value);
            this.modal.MemberPageModuleContent.SupportedChannels = supportedChannels.join(',');
            this.modal.MemberPageModuleContent.ChannelList = [];
            channelList.forEach(element => {
              var obj = {"MemberPageModuleID": this.modal.MemberPageModuleContent.MemberPageModuleID, "MemberPageModuleContentID": this.modal.MemberPageModuleContent.PKID, "Channel": element.key, "Link": element.link};
              this.modal.MemberPageModuleContent.ChannelList.push(obj);
            });
            util.ajax.post(`/MemberPage/AddMemberPageModuleContent`, this.modal.MemberPageModuleContent)
            .then(response => {
              if (response.data.status) {
                this.init();
                this.$Message.success("操作成功");
              } else {
                this.$Message.success(response.data.msg);
              }
              this.modal.loading = false;    
              this.modal.visible = false;                                                  
            });
          } else {
            this.modal.loading = false;          
          }
        });
      },
    modalCancel (name) {
      this.$refs[name].resetFields();
      this.modal.visible = false;
      },
    moduleModalShow () {
        var pageCode = this.pageCode;
        this.$refs['moduleModal.MemberPageModule'].resetFields();
        this.moduleModal.MemberPageModule = {
          ModuleName: "",
          MarginTop: 0,
          DisplayIndex: 1,
          Status: true,
          PageCode: pageCode
        };
        if (pageCode === "more") {
          this.moduleTypeList = this.moduleTypeListForMore;
        }
        this.moduleModal.visible = true;
      },
    moduleModalOk (name) {
        this.moduleModal.loading = true;          
        this.$refs[name].validate((valid) => {
          if (valid) {
            util.ajax.post(`/MemberPage/AddMemberPageModule`, this.moduleModal.MemberPageModule)
            .then(response => {
              if (response.data) {
                this.init();
                this.$Message.success("操作成功");
              }
            });
            this.moduleModal.loading = false;          
            this.moduleModal.visible = false;
          } else {
            this.moduleModal.loading = false;          
          } 
        });
      },
    moduleModalCancel (name) {
      this.$refs[name].resetFields();
      this.moduleModal.visible = false;
      },
    deleteMemberPageModule (contentId, moduleId, moduleType) {
        var confirmContent = "确认要删除模块及其所有内容吗？";
        if (moduleType === 0) {
          confirmContent = "确定要删除吗？";
        } 
        util.modal.confirm({
          title: "警告",
          content: confirmContent,
          onOk: () => {
              util.ajax.post(`/MemberPage/DeleteMemberPageModule?contentId=${contentId}&moduleId=${moduleId}&moduleType=${moduleType}`)
              .then(response => {
                if (response.data) {
                  this.init();
                  this.$Message.success("操作成功");
                } else {
                  this.$Message.success("操作失败");
                }
              });
          }
        });
      },
    uploadFormatError (file) {
        this.$Message.warning("请选择 .jpg  or .png  or .jpeg图片");
      },
    uploadMaxSize (file) {
        this.$Message.warning("请选择不超过100KB的图片");
      },
    uploadSuccess (res, file) {
          if (res.Status) {
              if (res.Type === "imageUrl") {
                  this.modal.MemberPageModuleContent.ImageUrl = res.ImageUrl;
              } 
          } else {
              this.$Message.warning(res.Msg);
          }
        },
    searchLog (objectType, objectId) {
        this.logModal.data = [];        
        this.logModal.visible = true;      
        this.logModal.loading = false;
        util.ajax.get(`/CommonConfigLog/GetCommonConfigLogs?objectType=${objectType}&objectId=${objectId}`)
            .then(response => {
              this.logModal.data = response.data;
            });
      }
  }
};
</script>
