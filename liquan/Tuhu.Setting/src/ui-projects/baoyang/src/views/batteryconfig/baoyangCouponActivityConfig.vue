 <template>
  <div>
    <h2 class="title">蓄电池/加油卡浮动配置</h2>
    <div>
      <Row type="flex"
           align="middle">
        <i-col span="5"
               style="text-align:left">
          <label>活动类型 : </label>
          <Select v-model="parameters.Type"
                  style="width:200px">
            <Option v-for="item in TypeList"
                    :value="item.value"
                    :key="item.value">{{ item.label }}</Option>
          </Select>
        </i-col>
        <i-col span="9">
          <Button type="primary"
                  @click="search">查询</Button>
          <Button type="success"
                  @click="add">添加</Button>
        </i-col>
      </Row>
    </div>
    <Table style="margin-top: 15px;"
           border
           stripe
           :columns="table.columns"
           :data="table.data"
           :loading="table.loading"
           @on-selection-change="table.selection = arguments[0]"></Table>
    <Page style="margin-top: 15px;"
          show-elevator
          show-total
          show-sizer
          :page-size-opts="[10, 20, 40, 100, 200]"
          :total="page.total"
          :current.sync="page.index"
          :page-size="page.size"
          @on-page-size-change="page.size=arguments[0]"></Page>

    <Modal :title="modal.title"
           ok-text="提交"
           v-model="modal.show"
           :closable="modal.closable"
           :mask-closable="false"
           :loading="modal.loading"
           @on-ok="submit">
      <Form label-position="right"
            :label-width="80"
            :model="modal.formItem">
        <FormItem label="活动名称">
          <Input type="text"
                 v-model="modal.formItems.ActivityName"></Input>
        </FormItem>
        <FormItem label="活动类型">
          <Select v-model="modal.formItems.Type"
                  style="width:200px">
            <Option v-for="item in TypeList"
                    :value="item.value"
                    :key="item.value">{{ item.label }}</Option>
          </Select>
        </FormItem>

        <FormItem label="活动开启">
          <Checkbox v-model="modal.formItems.ActivityStatus"></Checkbox>
        </FormItem>
        <FormItem label="浮层展开">
          <Checkbox v-model="modal.formItems.CheckStatus"></Checkbox>
        </FormItem>
        <FormItem label="浮层图片">
          <Upload  ref="upload1" action="/Files/uploadImg/"
                  :on-success="LayerhandleSuccess" >
            <Button icon="ios-cloud-upload-outline">Upload files</Button>
          </Upload>
          <span v-if="modal.formItems.LayerImage !==''">
            <img :src=" modal.formItems.LayerImage"
                 width="50"
                 height="50">
          </span>
        </FormItem>
        <FormItem label="活动图标">
          <Upload   ref="upload"  action="/Files/uploadImg/"
                  :on-success="ActivityhandleSuccess"  >
            <Button icon="ios-cloud-upload-outline">Upload files</Button>
          </Upload>
          <span v-if="modal.formItems.ActivityImage !==''">
            <img :src="modal.formItems.ActivityImage"
                 width="50"
                 height="50">
          </span>
        </FormItem>
        <FormItem label="使用平台">
          <div style="border-bottom: 1px solid #e9e9e9;padding-bottom:6px;margin-bottom:6px;">
            <Checkbox :indeterminate="Channel.indeterminate"
                      :value="Channel.checkAll"
                      @click.prevent.native="handleCheckAll">全选</Checkbox>
          </div>
          <CheckboxGroup v-model="Channel.ChannelName"
                         @on-change="checkAllGroupChange">
            <Checkbox label="APP"></Checkbox>
            <Checkbox label="H5"></Checkbox>
            <Checkbox label="WxApp">
              <label>小程序</label>
            </Checkbox>
          </CheckboxGroup>
        </FormItem>

        <FormItem label="类型(非必填)">
          <RadioGroup v-model="modal.ChannelConfigType">
            <Radio label="Coupon">
              <span>优惠券</span>
            </Radio>
            <Radio label="URL">
              <span>跳转链接</span>
            </Radio>
          </RadioGroup>
        </FormItem>
        <FormItem label="跳转类型"
                  v-if="modal.ChannelConfigType==='URL'">
          <label>APP链接</label>
          <Input type="text"
                 v-model="modal.UrlChannelConfig.App"></Input>
          <label>H5链接</label>
          <Input type="text"
                 v-model="modal.UrlChannelConfig.H5"></Input>
          <label>小程序链接</label>
          <Input type="text"
                 v-model="modal.UrlChannelConfig.WxApp"></Input>
        </FormItem>
        <FormItem label="优惠券GUID"
                  v-if="modal.ChannelConfigType==='Coupon'">

          <Input style="margin-bottom:10px"
                 v-model="modal.CouponChannelConfig[index]"
                 v-for="(item, index) in modal.CouponChannelConfig"
                 :key="index">
          <Button slot="append"
                  icon="minus-round"
                  v-on:click="ReductionGetRuleGUID(index)"></Button>
          </Input>

          <h3>
            <Button type="success"
                    v-on:click="AddGetRuleGUID()">
              <Icon type="plus"></Icon>
            </Button>
           
          </h3> 
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
                        title: "ID",
                        align: "center",
                        key: "Id",
                        width: 60
                    },
                    {
                        title: "活动编号",
                        width: 200,
                        align: "center",
                        key: "ActivityNum"
                    },
                    {
                        title: "活动类型",
                        width: 100,
                        align: "center",
                        key: "Type",
                        render: (h, p) => {
                            var ActivityStatus = p.row.Type === 1 ? '蓄电池' : '加油卡';
                            return h("label", {
                                domProps: {
                                    innerHTML: ActivityStatus
                                }
                            });
                        }
                    },
                    {
                        title: "活动名称",
                        width: 100,
                        align: "center",
                        key: "ActivityName"
                    },
                    {
                        title: "活动状态",
                        align: "center",
                        width: 100,
                        key: "ActivityStatus",
                        render: (h, p) => {
                            var ActivityStatus = p.row.ActivityStatus ? "启用" : "禁用";
                            return h("label", {
                                domProps: {
                                    innerHTML: ActivityStatus
                                }
                            });
                        }
                    },
                    {
                        title: "浮动图片",
                        align: "center",
                        width: 100,
                        key: "LayerImage",
                        render: (h, p) => {
                            var imgsrc = p.row.LayerImage;
                            return h("Img", {
                                domProps: {
                                    height: 50,
                                    src: imgsrc
                                }
                            });
                        }
                    },
                    {
                        title: "浮层状态",
                        align: "center",
                        width: 100,
                        key: "CheckStatus",
                        render: (h, p) => {
                            var CheckStatus = p.row.CheckStatus ? "展开" : "收起";
                            return h("label", {
                                domProps: {
                                    innerHTML: CheckStatus
                                }
                            });
                        }
                    },
                    {
                        title: "活动图标",
                        width: 100,
                        align: "center",

                        key: "ActivityImage",
                        render: (h, p) => {
                            var imgsrc = p.row.ActivityImage;
                            return h("Img", {
                                domProps: {
                                    height: 50,
                                    src: imgsrc
                                }
                            });
                        }
                    },
                    {
                        title: "适用平台",
                        width: 100,
                        align: "center",
                        render: (h, p) => {
                            var channelConfigs = p.row.ChannelConfigs || [];
                            var labelhtml = "";
                            var Channels = [];
                            if (channelConfigs.length === 0) {
                                channelConfigs = p.row.Channels;
                                for (var x in channelConfigs) {
                                    Channels.push(channelConfigs[x])
                                }
                            } else {
                                for (var i in channelConfigs) {
                                    Channels.push(channelConfigs[i].Channel)
                                }
                            }
                            Channels = this.distinct(Channels);
                            for (var index in Channels) {
                                labelhtml += "<lable>" + Channels[index] + "</lable>";
                                labelhtml += "<br></br>";
                            }
                            return h("span", {
                                domProps: {
                                    innerHTML: labelhtml
                                }
                            });
                        }
                    },
                    {
                        title: "优惠券GUID",
                        width: 200,
                        align: "center",
                        render: (h, p) => {
                            var ChannelConfigs = p.row.ChannelConfigs;
                            var labelhtml = "";

                            var Guids = [];
                            for (var x in ChannelConfigs) {
                                if (ChannelConfigs[x].Type === "Coupon") {
                                    Guids.push(ChannelConfigs[x].GetRuleGUID);
                                }
                            }
                            Guids = this.distinct(Guids);
                            for (var index in Guids) {
                                labelhtml += "<lable>" + Guids[index] + "</lable>";
                                labelhtml += "<br></br>";
                            }
                            return h("span", {
                                domProps: {
                                    innerHTML: labelhtml
                                }
                            });
                        }
                    },
                    {
                        title: "跳转链接",
                        width: 100,
                        align: "center",
                        render: (h, p) => {
                            var channelConfigs = p.row.ChannelConfigs;
                            var labelhtml = "";
                            for (var x in channelConfigs) {
                                if (channelConfigs[x].Type === "URL") {
                                    labelhtml += "<lable>" + channelConfigs[x].Url + "</lable>";
                                    labelhtml += "<br></br>";
                                }
                            }
                            return h("span", {
                                domProps: {
                                    innerHTML: labelhtml
                                }
                            });
                        }
                    },
                    {
                        title: "添加时间",
                        width: 100,
                        align: "center",
                        render: (h, p) => {
                            var labelhtml = "" + this.formatDate(p.row.CreateTime);
                            return h('label', {
                                domProps: {
                                    innerHTML: labelhtml
                                }
                            });
                        }
                    },
                    {
                        title: "更新时间",
                        width: 100,
                        align: "center",
                        render: (h, p) => {
                            var labelhtml = "" + this.formatDate(p.row.UpdateTime);
                            return h('label', {
                                domProps: {
                                    innerHTML: labelhtml
                                }
                            });
                        }
                    },
                    {
                        title: "操作",
                        width: 150,
                        align: "center",
                        // width: 150,
                        render: (h, p) => {
                            return [
                                h("a", {
                                    style: {
                                        "margin-right": "9px"
                                    },
                                    domProps: {
                                        href: "javascript:void(0)",
                                        innerHTML: "修改"
                                    },
                                    on: {
                                        click: () => {
                                            this.update(p.row);
                                        }
                                    }
                                }),
                                h("a", {
                                    style: {
                                        "margin-right": "9px"
                                    },
                                    domProps: {
                                        href: "javascript:void(0)",
                                        innerHTML: "删除"
                                    },
                                    on: {
                                        click: () => {
                                            this.delete(p.row.Id);
                                        }
                                    }
                                }),
                                h("a", {
                                    style: {
                                        "margin-right": "9px"
                                    },
                                    domProps: {
                                        href: "javascript:void(0)",
                                        innerHTML: "清除缓存"
                                    },
                                    on: {
                                        click: () => {
                                            this.RemoveCache(p.row.Id);
                                        }
                                    }
                                })
                            ];
                        }
                    }
                ],
                data: [],
                loading: false,
                selection: []
            },
            modal: {
                title: "添加区域",
                show: false,
                loading: true,
                closable: true,
                formItems: {
                    Id: "",
                    ActivityNum: "",
                    ActivityName: "",
                    ActivityStatus: false,
                    CheckStatus: 0,
                    LayerImage: "",
                    ActivityImage: "",
                    Type: 0,
                    Channels: [],
                    ChannelConfigs: []
                },
                UrlChannelConfig: {
                    App: "",
                    H5: "",
                    WxApp: ""
                },
                ChannelConfigType: "",
                CouponChannelConfig: []
            },
            TypeList: [
                {
                    value: 1,
                    label: "蓄电池"
                },
                {
                    value: 2,
                    label: "加油卡"
                }
            ],
            page: { total: 0, index: 1, size: 20 },
            parameters: { Type: '' },
            Channel: {
                indeterminate: true,
                checkAll: false,
                ChannelName: []
            },
            Url: {
                "APP": "",
                "H5": "",
                "WxApp": ""
            },
            Coupons: []
        };
    },
    watch: {
        "page.index" () {
            this.loadData();
        },
        "page.size" () {
            this.search();
        }
    },
    methods: {
        search () {
            if (this.page.index === 1) {
                this.loadData();
            } else {
                this.page.index = 1;
            }
        },
        AddGetRuleGUID () {
            if (this.modal.CouponChannelConfig.length < 5) {
                this.modal.CouponChannelConfig.push("");
            }
        },
        ReductionGetRuleGUID (key) { 
          this.modal.CouponChannelConfig.splice(key, 1);
        },
        formatDate (now) {
            if (now === "" || now === null) return "";
            var date = new Date(parseInt(now.substr(6)));
            var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
            var currentDate = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
            return date.getFullYear() + "-" + month + "-" + currentDate;
        },
        distinct (arr) {
            var result = [];
            arr.forEach(function (v, i, arr) { // 这里利用map，filter方法也可以实现
                var bool = arr.indexOf(v, i + 1); // 从传入参数的下一个索引值开始寻找是否存在重复
                if (bool === -1) {
                    result.push(v);
                }
            })
            return result;
        },
        RemoveCache (id) {
            this.$Modal.confirm({
                title: "温馨提示",
                content: "确定清除缓存？",
                loading: true,
                onOk: () => {
                    this.ajax.get("/CouponActivityConfigV2/RemoveCouponActivityConfigCache",
                        { params: {
                                id: id
                            } }).then(response => {
                            if (response.data.Status) {
                                this.$Message.info("操作成功");
                                this.$Modal.remove();
                            } else {
                                this.$Message.error("操作失败!" + (response.msg || ""));
                                this.$Modal.remove();
                            }
                        });
                },
                onCancel: () => {
                }
            });
        },
        loadData () {
            this.table.data = [];
            this.table.loading = true;
            var params = {};
            params.PageIndex = this.page.index;
            params.PageSize = this.page.size;
            params.Type = this.parameters.Type;
            this.ajax.post("/CouponActivityConfigV2/GetCouponActivityConfigs", {
                ...params
            })
                .then(response => {
                    if (response.data.Status) {
                        var res = response.data;
                        this.page.total = res.Data.Item1 || 0;
                        this.table.data = res.Data.Item2 || [];
                        this.table.loading = false;
                    } else {
                        this.$Message.error("操作失败!" + (response.data.Msg || ""));
                    }
                });
        },
        lazyLoadData () {
            this.table.loading = true;
            setTimeout(() => {
                this.search();
            }, 1500);
        },
        add () {
            this.modal.formItems.Id = 0;
            this.modal.formItems.ActivityNum = "";
            this.modal.formItems.ActivityName = "";
            this.modal.formItems.LayerImage = "";
            this.modal.formItems.ActivityImage = "";
            this.modal.formItems.ActivityStatus = false;
            this.modal.formItems.CheckStatus = false;
            this.modal.formItems.Type = "";
            this.modal.formItems.ChannelConfigs = [];
            this.modal.UrlChannelConfig.App = "";
            this.modal.UrlChannelConfig.H5 = "";
            this.modal.UrlChannelConfig.WxApp = "";
            this.modal.CouponChannelConfig = [];
            this.Channel.ChannelName = [];
            this.$refs.upload.fileList.splice(0, this.$refs.upload.fileList.length); 
            this.$refs.upload1.fileList.splice(0, this.$refs.upload.fileList.length);
            this.modal.ChannelConfigType = "";
            this.modal.title = "添加区域";
            this.modal.show = true;
        },
        update (item) {
            this.modal.formItems.Id = 0;
            this.modal.formItems.ActivityNum = "";
            this.modal.formItems.ActivityName = "";
            this.modal.formItems.LayerImage = "";
            this.modal.formItems.ActivityImage = "";
            this.modal.formItems.ActivityStatus = false;
            this.modal.formItems.CheckStatus = false;
            this.modal.formItems.Type = "";
            this.modal.formItems.ChannelConfigs = [];
            this.modal.UrlChannelConfig.App = "";
            this.modal.UrlChannelConfig.H5 = "";
            this.modal.UrlChannelConfig.WxApp = "";
            this.modal.CouponChannelConfig = [];
            this.Channel.ChannelName = [];
            this.modal.ChannelConfigType = "";
            this.$refs.upload.fileList.splice(0, this.$refs.upload.fileList.length);
            this.$refs.upload1.fileList.splice(0, this.$refs.upload.fileList.length);
            this.modal.formItems.Id = item.Id;
            this.modal.formItems.ActivityNum = item.ActivityNum;
            this.modal.formItems.ActivityName = item.ActivityName;
            this.modal.formItems.LayerImage = item.LayerImage;
            this.modal.formItems.ActivityImage = item.ActivityImage;
            this.modal.formItems.Type = item.Type;
            this.modal.formItems.ActivityStatus = item.ActivityStatus;
            this.modal.formItems.CheckStatus = item.CheckStatus;
            this.modal.formItems.ChannelConfigs = item.ChannelConfigs;
            this.modal.formItems.Channels = item.Channels;
            this.Channel.ChannelName = item.Channels;
            for (var configItem in this.modal.formItems.ChannelConfigs) {
                this.Channel.ChannelName.push(this.modal.formItems.ChannelConfigs[configItem].Channel);
                var itemType = this.modal.formItems.ChannelConfigs[configItem].Type;
                var itemChannel = this.modal.formItems.ChannelConfigs[configItem].Channel;
                this.modal.ChannelConfigType = itemType;
                if (itemType === "URL") {
                    switch (itemChannel) {
                        case "APP":
                            this.modal.UrlChannelConfig.App = this.modal.formItems.ChannelConfigs[configItem].Url;
                            break;
                        case "H5":
                            this.modal.UrlChannelConfig.H5 = this.modal.formItems.ChannelConfigs[configItem].Url;
                            break;
                        case "WxApp":
                            this.modal.UrlChannelConfig.WxApp = this.modal.formItems.ChannelConfigs[configItem].Url;
                            break;
                    }
                } else if (itemType === "Coupon") {
                    this.modal.CouponChannelConfig.push(this.modal.formItems.ChannelConfigs[configItem].GetRuleGUID);
                }
            }
            this.modal.CouponChannelConfig = this.distinct(this.modal.CouponChannelConfig);
            this.Channel.ChannelName = this.distinct(this.Channel.ChannelName);
            this.modal.title = "修改区域";
            this.modal.show = true;
        },
        submit () {
            var item = {};
            item.Id = this.modal.formItems.Id || 0;
            item.ActivityNum = this.modal.formItems.ActivityNum;
            item.ActivityStatus = this.modal.formItems.ActivityStatus;
            item.CheckStatus = this.modal.formItems.CheckStatus;
            item.LayerImage = this.modal.formItems.LayerImage;
            item.ActivityName = this.modal.formItems.ActivityName;
            item.ActivityImage = this.modal.formItems.ActivityImage;
            item.Type = this.modal.formItems.Type;
            item.ChannelConfigType = this.modal.ChannelConfigType;
            item.Channels = this.Channel.ChannelName;
            item.ChannelConfigs = [];
            for (var index in this.Channel.ChannelName) {
                if (item.ChannelConfigType === "URL") {
                    var url = "";
                    if (this.Channel.ChannelName[index] === "APP") {
                        url = this.modal.UrlChannelConfig.App
                    } else if (this.Channel.ChannelName[index] === "H5") {
                        url = this.modal.UrlChannelConfig.H5
                    } else if (this.Channel.ChannelName[index] === "WxApp") {
                        url = this.modal.UrlChannelConfig.WxApp
                    }
                    item.ChannelConfigs.push({
                        "ConfigId": item.Id,
                        "Channel": this.Channel.ChannelName[index],
                        "Type": item.ChannelConfigType,
                        "Url": url
                    });
                } else if (item.ChannelConfigType === "Coupon") {
                    for (var CouponIndex in this.modal.CouponChannelConfig) {
                        item.ChannelConfigs.push({
                            "ConfigId": item.Id,
                            "Channel": this.Channel.ChannelName[index],
                            "Type": item.ChannelConfigType,
                            "GetRuleGUID": this.modal.CouponChannelConfig[CouponIndex]
                        });
                    }
                }
            }
            if (item.LayerImage === "" || item.LayerImage === null) {
                this.$Message.warning("浮层图片必须上传");
                this.modal.loading = false;
                this.$nextTick(() => {
                    this.modal.loading = true;
                });
                return;
            }
            if (item.ActivityImage === "" || item.ActivityImage === null) {
                this.$Message.warning("活动图标必须上传");
                this.modal.loading = false;
                this.$nextTick(() => {
                    this.modal.loading = true;
                });
                return;
            }
            if (item.Type === "") {
                this.$Message.warning("活动类型不能为空");
                this.modal.loading = false;
                this.$nextTick(() => {
                    this.modal.loading = true;
                });
                return;
            }
            if (item.ActivityName === "") {
                this.$Message.warning("活动名称不能为空");
                this.modal.loading = false;
                this.$nextTick(() => {
                    this.modal.loading = true;
                });
                return;
            }
            if (item.Channels.length === 0) {
                this.$Message.warning("平台至少选择一个");
                this.modal.loading = false;
                this.$nextTick(() => {
                    this.modal.loading = true;
                });
                return;
            }
            var content = item.PKID > 0 ? "确认修改配置?" : "确认添加配置?";
            this.$Modal.confirm({
                title: "温馨提示",
                content: content,
                loading: true,
                onOk: () => {
                    this.ajax
                        .post("/CouponActivityConfigV2/SaveCouponActivityConfig", {
                            ...item
                        })
                        .then(response => {
                            var res = response.data;
                            if (res.Status) {
                                this.$Message.info("操作成功");
                            } else {
                                this.$Message.error("操作失败!" + (res.Msg || ""));
                            }
                            this.$Modal.remove();
                            this.modal.loading = false;
                            this.$nextTick(function () {
                                this.modal.loading = true;
                            });
                            if (res.Status) {
                                this.modal.show = false;
                                this.lazyLoadData();
                            }
                        });
                },
                onCancel: () => {
                    this.$Modal.remove();
                    this.modal.loading = false;
                    this.$nextTick(function () {
                        this.modal.loading = true;
                    });
                }
            });
        },
        delete (Id) {
            this.$Modal.confirm({
                title: "温馨提示",
                content: "确认删除蓄电池/加油卡浮动配置吗?",
                loading: true,
                onOk: () => {
                    this.ajax.get("/CouponActivityConfigV2/DeleteChannelConfigsByConfigId", {
                        params: {
                            Id: Id
                        }
                    })
                        .then(response => {
                            var res = response.data;
                            if (res.Status) {
                                this.$Message.info("操作成功");
                            } else {
                                this.$Message.error("操作失败!" + (res.Msg || ""));
                            }
                            this.$Modal.remove();
                            if (res.Status) {
                                this.lazyLoadData();
                            }
                        });
                }
            });
        },
        LayerhandleSuccess (response) {
            if (response.Status) {
                var res = response.Data;
                for (var x in res) {
                    this.modal.formItems.LayerImage = res[x];
                } 
            } else {
                this.$Message.error("操作失败!" + (response.Data.Msg || ""));
            }
        },
        ActivityhandleSuccess (response) {
            if (response.Status) {
                var res = response.Data;
                for (var x in res) {
                    this.modal.formItems.ActivityImage = res[x];
                }
            } else {
                this.$Message.error("操作失败!" + (response.Data.Msg || ""));
            }
        },
        handleCheckAll () {
            if (this.Channel.indeterminate) {
                this.Channel.checkAll = false;
            } else {
                this.Channel.checkAll = !this.Channel.checkAll;
            }
            this.Channel.indeterminate = false;

            if (this.Channel.checkAll) {
                this.Channel.ChannelName = ['APP', 'H5', 'WxApp'];
            } else {
                this.Channel.ChannelName = [];
            }
        },
        checkAllGroupChange (data) {
            if (data.length === 3) {
                this.Channel.indeterminate = false;
                this.Channel.checkAll = true;
            } else if (data.length > 0) {
                this.Channel.indeterminate = true;
                this.Channel.checkAll = false;
            } else {
                this.Channel.indeterminate = false;
                this.Channel.checkAll = false;
            }
        }
    },
    mounted () {
        this.loadData();
        this.$Message.config({
            duration: 5
        });
    }
};
</script>
<style>
.filter-element {
  width: 70%;
}
.demo-upload-list {
  display: inline-block;
  width: 60px;
  height: 60px;
  text-align: center;
  line-height: 60px;
  border: 1px solid transparent;
  border-radius: 4px;
  overflow: hidden;
  background: #fff;
  position: relative;
  box-shadow: 0 1px 1px rgba(0, 0, 0, 0.2);
  margin-right: 4px;
}
.demo-upload-list img {
  width: 100%;
  height: 100%;
}
.demo-upload-list-cover {
  display: none;
  position: absolute;
  top: 0;
  bottom: 0;
  left: 0;
  right: 0;
  background: rgba(0, 0, 0, 0.6);
}
.demo-upload-list:hover .demo-upload-list-cover {
  display: block;
}
.demo-upload-list-cover i {
  color: #fff;
  font-size: 20px;
  cursor: pointer;
  margin: 0 2px;
}
</style>
