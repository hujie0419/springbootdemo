<style type="text/css" scoped>
.orderlink table {
  border: 2px solid  #e9eaec;
  border-radius: 3px;
  background-color:  #e9eaec;
  margin: 20px 0 20px 0;
}

.orderlink thead th {
  background-color: #f8f8f9;
  color:#495060;
  cursor: pointer;
  -webkit-user-select: none;
  -moz-user-select: none;
  -user-select: none;
}

.orderlink tbody th {
  background-color: white;
  color:#495060;
}
.orderlink td {
  background-color: #f9f9f9;
}

.orderlink th,
td {
  min-width: 10px;
  padding: 10px 20px;
}

</style>
<template>
  <div>
        <Row align="middle"
             justify="space-around">
        <i-col span="6">
          <label class="label">订单渠道Key：</label>
          <Select filterable
                  placeholder="请选择订单渠道Key"
                  v-model="orderChannel" style="width:200px">
            <Option v-for="item in orderChannels"
                    :value="item.OrderChannel"
                    :key="item.OrderChannel">{{item.OrderChannel}}</Option>
          </Select>
        </i-col>
        <i-col span="5">
            <Button type="primary" icon="plus" @click="AddBusinessType" style="">为该渠道添加其他合作业务</Button>
        </i-col>
        <i-col span="5">
            <Button type="info" @click="back">返回申请列表</Button>
        </i-col>
        </Row>
        <div class='orderlink'>
            <table style="width:100%" >
            <thead>
                <tr>
                <th style="width:10px">#</th>
                <th style="width:100px">合作业务类型</th>
                <th style="width:280px">初始链接</th>
                <th style="width:180px">额外需求</th>
                <th style="width:50px">操作</th>
                </tr>
            </thead>
            <tbody v-for="(item, index) in channelLinks" :label="'渠道链接-'+(index+1)" :key="index" :prop="'executors.' + index">
               
                <tr > 
                    <th><Row align="middle"
           justify="space-around">{{index+1}}</Row></th>
                    <th>
                        <Row align="middle"
           justify="space-around">
                            <Select v-model="item.businessType" @on-change="GetInitialLink(item)">
                                <Option v-for="typeitem in businessTypesLinks"
                                    :value="typeitem.businessType"
                                    :key="typeitem.businessType">{{typeitem.businessType}}</Option>
                            </Select>
                        </Row>
                    </th>
                    <th>
                        <Row align="middle"
           justify="space-around">
                            <Input v-model.trim="item.initialPagelink" placeholder="请输入初始链接" size="large" />
                        </Row>
                    </th>
                   <th>
                       <Row align="middle"
           justify="space-around">
                            <Select multiple
                            v-model="item.additionalRequirement" @on-change="changeRequirement(item)">
                                <Option v-for="requireitem in additionalRequirements"
                                    :value="requireitem.value"
                                    :key="requireitem.value">{{requireitem.label}}</Option>
                            </Select>
                        </Row>
                    </th>
                    <th> <Row align="middle"
           justify="space-around"><Button type="primary"  @click="DeleteBusinessType(index)">删除</Button> </Row></th>
                </Row>
                </tr>
                
            </tbody>
            </table>
        </div>
      <Button type="info" @click="Submit()">生成定制链接</Button>
  </div>
</template>
<script>
export default {
    data () {
        return {
            orderChannels: [],
            orderChannelModel: {},
            orderChannel: "",
            businessTypesLinks: [
                {
                    businessType: "轮胎",
                    initialLink: "https://wx.tuhu.cn/vue/wx/pages/tire/tirelist?_tab=0"
                },
                {
                    businessType: "保养",
                    initialLink: "https://wx.tuhu.cn/vue/wx/pages/maintenance/index?_tab=0"
                },
                {
                    businessType: "洗车（美容）",
                    initialLink: "https://wx.tuhu.cn/vue/wx/pages/shops/beautyindex?_tab=0"
                },
                {
                    businessType: "车品",
                    initialLink: "https://wx.tuhu.cn/vue/wx/pages/chepin/index"
                },
                {
                    businessType: "蓄电池",
                    initialLink: "https://wx.tuhu.cn/vue/wx/pages/chepin/battery"
                },
                {
                    businessType: "钣喷",
                    initialLink: "https://wx.tuhu.cn/vue/wx/pages/paint/index"
                },
                {
                    businessType: "首页",
                    initialLink: "https://wx.tuhu.cn/vue/wx/"
                },
                {
                    businessType: "其他",
                    initialLink: ""
                }
            ],
            additionalRequirements: [
                {
                    value: '无',
                    label: '无'
                },
                {
                    value: 'IsAggregatePage',
                    label: '聚合页'
                },
                {
                    value: 'IsAuthorizedLogin',
                    label: '授权登录'
                },
                {
                    value: 'IsPartnerReceivSilver',
                    label: '合作方收银'
                },
                {
                    value: 'IsOrderBack',
                    label: '订单回传'
                },
                {
                    value: 'IsViewOrders',
                    label: '查看订单（浮层）'
                },
                {
                    value: 'IsViewCoupons',
                    label: '查看优惠券（浮层）'
                },
                {
                    value: 'IsContactUserService',
                    label: '联系客服（浮层）'
                },
                {
                    value: 'IsBackTop',
                    label: '返回顶部（浮层）'
                }
            ],
            channelLinks: [{
                businessType: "其他",
                initialPagelink: "",
                additionalRequirement: ["无"]
            }]
        }
    },
    methods: {
        loadOrderChannel () {
            this.ajax.get('/ThirdPartyOrderChannellink/GetOrderChanneKeylList')
                .then(response => {
                    var res = response.data.data;
                    this.orderChannels = res || [];
                    if (res.length >= 1) {
                       this.orderChannel = res[0].OrderChannel;
                    }
            })
        },
        AddBusinessType () {
            var that = this;
            that.channelLinks.push({
                businessType: "其他",
                initialPagelink: "",
                additionalRequirement: ["无"]
            });
        },
        DeleteBusinessType (index) {
            this.channelLinks.splice(index, 1);
        },
        GetOrderChannelModel () {
            var channels = this.orderChannels;
            var channel = this.orderChannel;
            var channelArr = channels.filter(function (item) {
                return item.OrderChannel === channel;
            });
            return channelArr;
        },
        GetInitialLink (item) {
            var typeArr = [];
            typeArr = this.businessTypesLinks.filter(function (typeitem) {
                return typeitem.businessType === item.businessType;
            });
            item.initialPagelink = typeArr[0].initialLink;
        },
        changeRequirement (item) {
           if (item.additionalRequirement.indexOf("无") < item.additionalRequirement.length - 1 && item.additionalRequirement.indexOf("无") !== -1) {
                let newArr = [];
                let oldArr = item.additionalRequirement;
                for (let index = 0; index < oldArr.length; index++) {
                   if (oldArr[index] !== "无") {
                       newArr.push(oldArr[index]);
                   }
                }
                item.additionalRequirement = newArr;
            }
        },
        judge () {
            var isfalse = true;
            var links = this.channelLinks;
            if (links.length === 0) {
                 this.$Message.info('请为该渠道添加合作业务！！');
                 isfalse = false;
                 return false;
            } else {
                for (var i = 0; i < links.length; i++) {
                    if (links[i].initialPagelink === '') {
                        this.$Message.info('初始链接不能为空！！');
                        isfalse = false;
                        break;
                    }
                    if (links[i].additionalRequirement.join(",").length === 0) {
                        this.$Message.info('额外需求为必选项！！');
                        isfalse = false;
                        break;
                    }
                    if (links[i].additionalRequirement.join(",").indexOf("无") >= 0 && links[i].additionalRequirement.join(",").length > 1) {
                        this.$Message.info('额外需求中的无与其他选项是互斥项！！');
                        isfalse = false;
                        break;
                    }
                }
            }
            return isfalse;
        },
        Submit () {
            if (!this.judge()) {
                return false;
            }
            this.orderChannelModel = this.GetOrderChannelModel()[0];
            var links = (this.channelLinks || []).map(x => {
                        var link = {};
                        link.BusinessType = x.businessType;
                        link.InitialPagelink = x.initialPagelink;
                        link.AdditionalRequirement = x.additionalRequirement.join(",");
                        return link;
                    });
            var validLinks = links || [];
            this.ajax
            .post(
             `/ThirdPartyOrderChannellink/AddOrderChannellink`, {
                 channelModel: this.orderChannelModel,
                 linkList: validLinks
             }
            )
            .then(response => {
              if (response.data) {
                this.$Message.success("保存成功");
                 setTimeout(() => {
                    this.$router.push({'name': 'TPOderChannelLink'});
                }, 1500);
              } else {
                this.$Message.success("保存失败");
              }
            });
        },
        back () {
            this.$router.push({'name': 'TPOderChannelLink'});
        }
    },
    mounted () {
        this.loadOrderChannel();
    }
}
</script>
