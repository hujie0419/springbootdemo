<style type="text/css" scoped>
.AddDisabled{
    pointer-events: none;
    opacity: 0.5;
}
</style>
<template>
  <div>
    <h1 class="title">车友群列表</h1>
   
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
        <Form ref="modal.carFriends" :model="modal.carFriends" :rules="modal.rules" label-position="left" :label-width="150">
            <FormItem label="群名称" prop="GroupName">
                <Input v-model.trim="modal.carFriends.GroupName" placeholder=""/>
            </FormItem>
            <FormItem label="群描述" prop="GroupDesc">
                <Input v-model.trim="modal.carFriends.GroupDesc" placeholder=""/>
            </FormItem>
            <FormItem label="绑定车型" >
                 <Row type="flex"
               align="middle"
               justify="space-around">
                    <i-col span="11">
                    <Select filterable placeholder="请选择品牌"
                            v-model="modal.carFriends.BindBrand"
                            @on-change="getModalVehicle" v-bind:class="{AddDisabled:isAddDisabled}" transfer>
                        <Option value="">请选择品牌</Option>
                        <Option v-for="item in modal.brands"
                                :value="item.Name"
                                :key="item.Name">{{item.Name}}</Option>
                    </Select>
                    </i-col>
                    <i-col span="2"></i-col>
                    <i-col span="11">
                    <Select filterable placeholder="请选择车型"
                            v-model="modal.carFriends.BindVehicleTypeID" v-bind:class="{AddDisabled:isAddDisabled}" transfer >
                        <Option value="None" key="None">全部</Option>
                        <Option v-for="item in modal.vehicles"
                                :value="item.VehicleId"
                                :key="item.VehicleId">{{item.Vehicle}}</Option>
                    </Select>
                    </i-col>
                </Row>  
            </FormItem>
            <FormItem  label="群微信头像" >
                <Col span="4" v-show="modal.carFriends.GroupHeadPortrait!=''">
                    <a :href="modal.carFriends.GroupHeadPortrait" target="_blank"><img :src="modal.carFriends.GroupHeadPortrait" style='width:50px;height:50px'></a>
                </Col>
                <Col span="6">
                    <Upload action="/GroupBuyingV2/UploadImage?type=image" :format="['jpg','jpeg','png']" :on-format-error="handleFormatError" :max-size="1000" :on-exceeded-size="handleMaxSize" :on-success="handleSuccess1" :show-upload-list="false">
                        <Button type="ghost" icon="ios-cloud-upload-outline">Upload files</Button>
                    </Upload>
                </Col>
                <Col span="5"  style="margin-left:30px;" v-show="modal.carFriends.GroupHeadPortrait!=''">
                    <Button type="warning" icon="refresh" @click="modal.carFriends.GroupHeadPortrait=''">清除</Button>
                </Col>
            </FormItem>
            <FormItem  label="群微信二维码" >
                <Col span="4" v-show="modal.carFriends.GroupQRCode!=''">
                    <a :href="modal.carFriends.GroupQRCode" target="_blank"><img :src="modal.carFriends.GroupQRCode" style='width:50px;height:50px'></a>
                </Col>
                <Col span="6">
                    <Upload action="/GroupBuyingV2/UploadImage?type=image" :format="['jpg','jpeg','png']" :on-format-error="handleFormatError" :max-size="1000" :on-exceeded-size="handleMaxSize" :on-success="handleSuccess2" :show-upload-list="false">
                        <Button type="ghost" icon="ios-cloud-upload-outline">Upload files</Button>
                    </Upload>
                </Col>
                <Col span="5"  style="margin-left:30px;" v-show="modal.carFriends.GroupQRCode!=''">
                    <Button type="warning" icon="refresh" @click="modal.carFriends.GroupQRCode=''">清除</Button>
                </Col>
            </FormItem>
            <FormItem label="群类别" prop="GroupCategory">
              <RadioGroup v-model="modal.carFriends.GroupCategory" @on-change="GroupCategoryChange">
                <Radio label="0">车型群</Radio>
                <Radio label="1" >工厂店群</Radio>
                <Radio label="2">地区群</Radio>
                <Radio label="3" >福利群</Radio>
                <Radio label="4">拼团群</Radio>
            </RadioGroup>             
            </FormItem>
            <FormItem label="群权重" prop="GroupWeight">
                <Input v-model.trim="modal.carFriends.GroupWeight" placeholder=""/>
            </FormItem>
             <FormItem label="是否推荐" prop="IsRecommendString">
              <RadioGroup v-model="modal.carFriends.IsRecommendString">
                <Radio label="1">推荐</Radio>
                <Radio label="0">不推荐</Radio>
            </RadioGroup>             
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
                        title: '群名称',
                        align: 'left',
                        key: 'GroupName'
                     },
                    {
                        title: '群二维码',
                        align: 'center',
                        key: 'GroupQRCode',
                        render: (h, params) => {
                            return h('img', {domProps: { src: params.row.GroupQRCode, width: "75", height: "80" }}, params.row.ImageUrl);
                        }
                    },
                    {
                        title: '绑定车型',
                        align: 'left',
                        key: 'BindVehicleType'
                     },
                    {
                        title: '群类别',
                        align: 'left',
                        key: 'GroupCategory',
                        render: function (h, params) {
                        switch (params.row.GroupCategory) {
                            case 0:
                            return h("span", "车型群");
                            case 1:
                            return h("span", "工厂店群");
                            case 2:
                            return h("span", "地区群");
                            case 3:
                            return h("span", "福利群");
                            default:
                            return h("span", "拼团群");
                            }
                        }
                    },
                    {
                        title: '权重',
                        align: 'center',
                        key: 'GroupWeight'
                     },
                     {
                        title: '是否推荐',
                        align: 'center',
                        key: 'IsRecommend',
                        render: function (h, params) {
                        switch (params.row.IsRecommend) {
                            case false:
                            return h("span", "不推荐");
                            default:
                            return h("span", "推荐");
                            }
                        }
                    },
                    {
                        title: "群创建时间",
                        key: "GroupCreateTime",
                        align: 'center',
                        render: (h, params) => {
                        return h('div', [
                                h('span', this.FormatToDate(params.row.GroupCreateTime))
                                        ]);
                        }
                    },
                    {
                        title: "过期时间",
                        key: "GroupOverdueTime",
                        align: 'center',
                        render: (h, params) => {
                        return h('div', [
                                h('span', this.FormatToDate(params.row.GroupOverdueTime))
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
            vehicles: [],
            modal: {
                brands: [],
                vehicles: [],
                visible: false,
                loading: true,           
                title: "编辑车友群",            
                carFriends: {
                    pkid: 0,
                    GroupName: "",
                    GroupDesc: "",
                    BindBrand: "",
                    BindVehicleType: "",
                    BindVehicleTypeID: "",
                    GroupHeadPortrait: "",
                    GroupQRCode: "",
                    GroupCategory: "0",
                    GroupWeight: 0,
                    IsRecommendString: "1",
                    IsRecommend: 1,
                    callback: null,
                    defaultBindBrand: ""
                },
                rules: {
                    GroupName: [
                    {
                        required: true,
                        message: "请填写群名称",
                        trigger: "change"
                    }
                ],
                    GroupDesc: [
                    {
                        required: true,
                        message: "请填写群描述",
                        trigger: "change"
                    }
                ],
                    GroupCategory: [
                    {
                        required: true,
                        message: "请选择群类别",
                        trigger: "change"
                    }
                ],
                    GroupWeight: [
                        {
                            type: 'number',
                            message: '请输入数字格式', 
                            trigger: 'blur', 
                            transform (value) {
                             return Number(value);
                          }
                        }
                ]
                }
            },
            isAddDisabled: false
        }
    },
    created () {
        this.loadData();
        this.loadBrands();
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
        search () {
            if (this.page.index === 1) {
                this.loadData();
            } else {
                this.page.index = 1;
            }
        },
        loadData (pageIndex) {
            this.page.index = pageIndex;
            this.table.data = [];
            this.table.loading = true;
            this.ajax.get(`/CarFriendsGroup/GetCarFriendsGroupList?pageSize=${this.page.size}&pageIndex=${this.page.index}`).then(response => {
                var res = response.data.data;
                this.page.total = response.data.count;
                this.table.data = res || [];
                this.table.loading = false;
            })
        },
        loadBrands () {
            this.modal.brands = [];
            this.ajax.get('/CarFriendsGroup/GetAllBrand')
                .then(response => {
                    var data = response.data.data;
                    var brands = (data || []).map(x => {
                        var brand = {};
                        brand.Name = x.Brand;
                        return brand;
                    });
                    this.modal.brands = brands || [];
                })
        },
        lazyLoadData () {
            this.table.loading = true;
            setTimeout(() => {
                this.search();
            }, 1000);
        },
        handleFormatError (file) {
            this.$Message.warning("请选择 .jpg  or .png  or .jpeg图片");
        },
        handleMaxSize (file) {
            this.$Message.warning("请选择不超过100KB的图片");
        },
        handleSuccess1 (res, file) {
            if (res.Status) {
                this.modal.carFriends.GroupHeadPortrait = res.ImageUrl
                } else {
                    this.$Message.warning(res.Msg);
            }
        },
        handleSuccess2 (res, file) {
            if (res.Status) {
                this.modal.carFriends.GroupQRCode = res.ImageUrl
                } else {
                    this.$Message.warning(res.Msg);
            }
        },
        GroupCategoryChange () {
            if (this.modal.carFriends.GroupCategory !== "0") {
                this.isAddDisabled = true;
            } else {
                this.isAddDisabled = false;
            }
        },
        update (item) {
            this.modal.vehicles = [];
            this.ajax.get(`/CarFriendsGroup/GetAllVehicleByBrandName?brand=${item.BindBrand}`)
                .then(response => {
                    if (response.data.msg) {
                        this.$Message.error(response.data.msg);
                    } else {
                        if (response.data.data.length > 0) {
                            console.log(item.BindVehicleTypeID);
                            this.modal.vehicles = response.data.data || [];
                            this.modal.carFriends.BindVehicleTypeID = (item.BindVehicleTypeID === undefined || item.BindVehicleTypeID === null || item.BindVehicleTypeID === "" || item.BindVehicleTypeID === "undefined") ? "None" : item.BindVehicleTypeID;
                            this.modal.carFriends.pkid = item.PKID;
                            this.modal.carFriends.GroupName = item.GroupName
                            this.modal.carFriends.GroupDesc = item.GroupDesc;
                            this.modal.carFriends.BindBrand = item.BindBrand;
                            this.modal.carFriends.GroupHeadPortrait = item.GroupHeadPortrait;
                            this.modal.carFriends.GroupQRCode = item.GroupQRCode;
                            this.modal.carFriends.GroupCategory = item.GroupCategory + "";
                            this.modal.carFriends.GroupWeight = item.GroupWeight;
                            this.modal.carFriends.IsRecommendString = item.IsRecommend === true ? "1" : "0";
                            this.modal.carFriends.BindVehicleType = item.BindVehicleType;
                            this.modal.visible = true;
                            this.modal.title = "修改车友群";
                            this.GroupCategoryChange();
                        } else {
                            this.$Message.error("无数据");
                        }
                    }
                })
        },
        getModalVehicle (bindBrand, defaultVehicleTypeID) {
            this.modal.vehicles = [];
            if (!bindBrand) {
                return;
            }
            this.ajax.get(`/CarFriendsGroup/GetAllVehicleByBrandName?brand=${bindBrand}`)
                .then(response => {
                    if (response.data.msg) {
                        this.$Message.error(response.data.msg);
                    } else {
                        if (response.data.data.length > 0) {
                            console.log(defaultVehicleTypeID);
                            this.modal.vehicles = response.data.data || [];
                            this.modal.carFriends.BindVehicleTypeID = (defaultVehicleTypeID === undefined || defaultVehicleTypeID === null || defaultVehicleTypeID === "" || defaultVehicleTypeID === "undefined") ? "None" : defaultVehicleTypeID;
                        } else {
                            this.$Message.error("无数据");
                        }
                    }
                })
        },
        add () {
            this.GroupCategoryChange();
            this.modal.vehicles = [];
            this.modal.carFriends.pkid = 0;
            this.modal.carFriends.GroupName = "";
            this.modal.carFriends.GroupDesc = "";
            this.modal.carFriends.BindBrand = "";
            this.modal.carFriends.BindVehicleType = "";
            this.modal.carFriends.BindVehicleTypeID = "";
            this.modal.carFriends.GroupHeadPortrait = "";
            this.modal.carFriends.GroupQRCode = "";
            this.modal.carFriends.GroupCategory = "0";
            this.modal.carFriends.GroupWeight = 0;
            this.modal.carFriends.IsRecommendString = "1";
            this.modal.title = "新建车友群";
            this.modal.visible = true;
        },
        cancel () {
            this.$refs['modal.carFriends'].resetFields();
            this.modal.visible = false;
        },
        delete (pkid) {
            this.$Modal.confirm({
                title: "温馨提示",
                content: "确认删除该车友群?",
                loading: true,
                onOk: () => {
                    this.ajax.post("/CarFriendsGroup/DeleteCarFriendsGroup", {
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
            this.modal.visible = true;
            this.modal.loading = true;
            var containFlag = 0;
            this.$refs['modal.carFriends'].validate((valid) => {
                if (valid) {
                    if (this.modal.carFriends.GroupWeight === "") {
                         this.$Message.error("请输入群权重！");
                            this.modal.loading = false;
                            this.$nextTick(() => {
                                this.modal.loading = true;
                            });
                            return;
                    }
                    if (this.modal.carFriends.GroupCategory === "0") {
                        if (this.modal.carFriends.BindBrand === "" || this.modal.carFriends.BindBrand === "undefined" || this.modal.carFriends.BindBrand === null || this.modal.carFriends.BindBrand === undefined) {
                            this.$Message.error("请选择品牌或车型！");
                            this.modal.loading = false;
                            this.$nextTick(() => {
                                this.modal.loading = true;
                            });
                            return;
                        }
                    }
                    if (typeof (this.modal.carFriends.GroupHeadPortrait) === "undefined" || this.modal.carFriends.GroupHeadPortrait === "") {
                        this.$Message.error("请上传群微信头像");
                        this.modal.loading = false;
                        this.$nextTick(() => {
                          this.modal.loading = true;
                        });
                        return;
                    } 
                    if (typeof (this.modal.carFriends.GroupQRCode) === "undefined" || this.modal.carFriends.GroupQRCode === "") {
                        this.$Message.error("请上传群微信二维码");
                        this.modal.loading = false;
                        this.$nextTick(() => {
                          this.modal.loading = true;
                        });
                        return;
                    }
                    if (this.modal.carFriends.GroupCategory !== "0") {
                        this.modal.carFriends.BindBrand = null;
                        this.modal.carFriends.BindVehicleType = null;
                        this.modal.carFriends.BindVehicleTypeID = null;
                        containFlag = 1;
                    } else {
                        if (this.modal.carFriends.BindVehicleTypeID === "" || this.modal.carFriends.BindVehicleTypeID === "undefined" || this.modal.carFriends.BindVehicleTypeID === null || this.modal.carFriends.BindVehicleTypeID === undefined || this.modal.carFriends.BindVehicleTypeID === "None") {
                            this.modal.carFriends.BindVehicleType = this.modal.carFriends.BindBrand.split(" - ")[1];
                            this.modal.carFriends.BindVehicleTypeID = null;
                            containFlag = 1;
                        } else {
                            if (this.modal.vehicles.length > 0) {
                                this.modal.vehicles.forEach(element => {
                                    if (element.VehicleId === this.modal.carFriends.BindVehicleTypeID) {
                                        this.modal.carFriends.BindVehicleType = this.modal.carFriends.BindBrand.split(" - ")[1] + "-" + element.Vehicle;
                                        containFlag = 1;
                                    }
                                });
                            }
                        }
                    }
                    if (containFlag === 0) {
                        this.$Message.error("该车型与品牌不匹配！");
                        this.modal.loading = false;
                        this.$nextTick(() => {
                          this.modal.loading = true;
                        });
                        return;
                    }
                    var isUpdate = false;
                    if (this.modal.carFriends.pkid > 0) {
                        isUpdate = true;
                    }
                    var carFriendsModel = this.modal.carFriends;
                    if (this.modal.carFriends.IsRecommendString === "1") {
                        carFriendsModel.IsRecommend = 1;
                    } else {
                        carFriendsModel.IsRecommend = 0;
                    }
                    this.ajax.post(isUpdate ? '/CarFriendsGroup/UpdateCarFriendsGroup' : "/CarFriendsGroup/AddCarFriendsGroup", carFriendsModel
                    ).then((response) => {
                        if (response.data.status) {
                            setTimeout(() => {
                            this.$Message.success(response.data.msg);
                            if (isUpdate) {
                                this.loadData(this.page.index);
                            } else {
                                this.loadData(1);
                            }
                            this.$refs['modal.carFriends'].resetFields();
                            this.modal.vehicles = [];
                            this.modal.carFriends.pkid = 0;
                            this.modal.carFriends.GroupName = "";
                            this.modal.carFriends.GroupDesc = "";
                            this.modal.carFriends.BindBrand = "";
                            this.modal.carFriends.BindVehicleType = "";
                            this.modal.carFriends.BindVehicleTypeID = "";
                            this.modal.carFriends.GroupHeadPortrait = "";
                            this.modal.carFriends.GroupQRCode = "";
                            this.modal.carFriends.GroupCategory = "0";
                            this.modal.carFriends.GroupWeight = 0;
                            this.modal.carFriends.IsRecommendString = "1";
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
