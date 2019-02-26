<template>
    <div>
        <h1 class="title">{{ $route.params.id }} 轮胎拼团配置</h1>
        <Card style="margin: 0px auto; width: 1024px;">
            <Form ref="tireGroupConfig" :model="tireGroupConfig" :label-width="90" style="margin: 0px auto; width: 80%;">
                <FormItem label="GroupId" style="margin-top: 24px;">
                    <Row>
                        <Col>
                            <Input v-model="tireGroupConfig.ProductGroupId" :disabled="true" placeholder="自动生成" />
                        </Col>
                    </Row>
                </FormItem>
                <FormItem label="上下架时间">
                    <Row>
                        <Col span="8">
                            <Date-Picker v-model="tireGroupConfig.BeginTime" type="datetime" format="yyyy-MM-dd HH:mm:ss" transfer placeholder="上架时间"></Date-Picker>
                        </Col>
                        <Col span="8">
                            <Date-Picker v-model="tireGroupConfig.EndTime" type="datetime" format="yyyy-MM-dd HH:mm:ss" transfer placeholder="下架时间"></Date-Picker>
                        </Col>
                    </Row>
                </FormItem>
                <FormItem label="列表页展示">
                    <i-switch v-model="tireGroupConfig.IsShow" size="large">
                        <span slot="open">On</span>
                        <span slot="close">Off</span>
                    </i-switch>
                </FormItem>
                <FormItem label="是否自动拼团">
                    <i-switch v-model="tireGroupConfig.IsAutoFinish" size="large" :true-value="1" :false-value="0" >
                        <span slot="open">On</span>
                        <span slot="close">Off</span>
                    </i-switch>
                </FormItem>
                <FormItem label="拼团人数">
                    <Input-Number v-model="tireGroupConfig.MemberCount" :disabled="!isAdd" :max="8" :min="2" placeholder="拼团人数" />
                    <span>&nbsp;填写2~8之间的整数</span>
                </FormItem>
                <FormItem label="拼团类型">
                    <Row>
                        <Col>
                            <Select v-model="tireGroupConfig.GroupType" :disabled="!isAdd" placeholder="拼团类型" transfer>
                                <Option v-for="item in groupTypeList" :value="item.value" :key="item.value">{{ item.label }}</Option>
                            </Select>
                        </Col>
                    </Row>
                </FormItem>
                <FormItem label="拼团种类">
                    <Row>
                        <Col>
                            <Select v-model="tireGroupConfig.GroupCategory" :disabled="true" placeholder="拼团种类" transfer>
                                <Option v-for="item in groupCategoryList" :value="item.value" :key="item.value">{{ item.label }}</Option>
                            </Select>
                        </Col>
                    </Row>
                </FormItem>
                <FormItem label="运营标签">
                    <Row>
                        <Col>
                            <Select v-model="tireGroupConfig.Label" placeholder="运营标签" transfer>
                                <Option v-for="item in labelList" :value="item" :key="item.value">{{ item }}</Option>
                            </Select>
                        </Col>
                    </Row>
                </FormItem>
                <FormItem label="显示顺序">
                    <Input-Number v-model="tireGroupConfig.Sequence" :max="10000000000" :min="0" placeholder="首页显示顺序" />
                </FormItem>
                <FormItem label="显示渠道">
                    <Checkbox v-model="tireGroupConfig.H5Channel" name="Channel" data-key="kH5" @on-change="channelChange">H5</Checkbox>
                    <Checkbox v-model="tireGroupConfig.WXChannel" name="Channel" data-key="WXAPP" @on-change="channelChange">微信小程序</Checkbox>
                </FormItem>
                <FormItem label="宣传图片">
                    <Col span="4" v-show="tireGroupConfig.Image">
                        <a :href="tireGroupConfig.Image" target="_blank"><img :src="tireGroupConfig.Image" style='width:50px;height:50px'></a>
                    </Col>
                    <Col span="6">
                    <Upload action="/GroupBuyingV2/UploadImage?type=image" :format="['jpg','jpeg','png']" :on-format-error="handleFormatError" :max-size="10000" :on-exceeded-size="handleMaxSize" :on-success="handleSuccess" :show-upload-list="false">
                        <Button type="ghost" icon="ios-cloud-upload-outline">Upload files</Button>
                    </Upload>
                    </Col>
                    <Col span="5" v-show="tireGroupConfig.Image">
                    <Button type="warning" icon="refresh" @click="tireGroupConfig.Image=''">清除</Button>
                    </Col>
                </FormItem>
                <FormItem label="分享图片">
                    <Col span="4" v-show="tireGroupConfig.ShareImage">
                    <a :href="tireGroupConfig.ShareImage" target="_blank"><img :src="tireGroupConfig.ShareImage" style='width:50px;height:50px'></a>
                    </Col>
                    <Col span="6">
                    <Upload action="/GroupBuyingV2/UploadImage?type=shareImage" :format="['jpg','jpeg','png']" :on-format-error="handleFormatError" :max-size="10000" :on-exceeded-size="handleMaxSize" :on-success="handleSuccess" :show-upload-list="false">
                        <Button type="ghost" icon="ios-cloud-upload-outline">Upload files</Button>
                    </Upload>
                    </Col>
                    <Col span="5" v-show="tireGroupConfig.ShareImage">
                    <Button type="warning" icon="refresh" @click="tireGroupConfig.ShareImage=''">清除</Button>
                    </Col>
                </FormItem>
                <FormItem label="分享文案">
                    <Row>
                        <Col>
                            <Input v-model="tireGroupConfig.ShareId" placeholder="分享文案" />
                        </Col>
                    </Row>
                </FormItem>
                <FormItem label="抽奖规则描述">
                    <Row>
                        <Col>                
                            <Input type="textarea" :rows="4" v-model="tireGroupConfig.GroupDescription" placeholder="抽奖规则描述" />
                        </Col>
                    </Row>
                </FormItem>
                <FormItem style="text-align: center;">
                    <!-- <Button @click="goBack">返回列表</Button> -->
                    <Button type="primary" @click="nextStep" :disabled="noSave">下一步</Button>
                </FormItem>
            </Form>
        </Card>
    </div>
</template>

<script>
import moment from "moment"

export default {
    data () {
        return {
            isAdd: true,
            noSave: this.$route.params.id !== undefined,
            labelList: [
                "轮胎团",
                "优惠券团",
                "抽奖团",
                "1分团",
                "低价团",
                "精品团",
                "团长免单",
                "团长特价",
                "优惠券团+团长特价团"
            ],
            groupTypeList: [
                {
                    label: "普通团",
                    value: 0
                },
                {
                    label: "新人团",
                    value: 1
                }
            ],
            groupCategoryList: [
                {
                    label: "轮胎拼团",
                    value: 3
                }
            ],
            tireGroupConfig: {
                ProductGroupId: "",
                BeginTime: new Date(),
                EndTime: "2099-01-01 00:00:00",
                IsShow: true,
                MemberCount: 2,
                GroupType: 0,
                TotalGroupCount: "",
                CurrentGroupCount: 0,
                SpecialUserTag: 0,
                Sequence: 0,
                Channel: "",
                H5Channel: false,
                WXChannel: false,
                Image: "",
                ShareImage: "",
                ShareId: "",
                GroupCategory: 3,
                GroupDescription: "",
                Label: "轮胎团"
            }
        }
    },
    methods: {
        channelChange () {
            this.tireGroupConfig.Channel = "";
            if (this.tireGroupConfig.H5Channel) {
                this.tireGroupConfig.Channel += "kH5;";
            }
            if (this.tireGroupConfig.WXChannel) {
                this.tireGroupConfig.Channel += "WXAPP";
            }
        },
        handleFormatError (file) {
            this.$Message.warning("请选择 .jpg  or .png  or .jpeg图片");
        },
        handleMaxSize (file) {
            this.$Message.warning("请选择不超过100KB的图片");
        },
        handleSuccess (res, file) {
            if (res.Status) {
                if (res.Type === "shareImage") {
                    this.tireGroupConfig.ShareImage = res.ImageUrl;
                } else {
                    this.tireGroupConfig.Image = res.ImageUrl;
                }
            } else {
                this.$Message.warning(res.Msg);
            }
        },
        formatDate (date) {
            return moment(date).format('YYYY-MM-DD HH:mm:ss');
        },
        goBack () {
            this.$router.push({
                name: 'ProductConfig'
            });
        },
        nextStep () {
            this.ajax
                .post("/GroupBuyingV2/UpsertGroupBuyingTireGroupConfig", {
                    config: this.tireGroupConfig,
                    copyFrom: this.$route.query.copyFrom
                })
                .then(response => {
                    if (response.data.Success) {
                        this.$router.push({
                            path: `/ProductConfig/TireProduct/${response.data.Message}`
                        });
                    } else {
                        this.$Message.error(response.data.Message);
                    }
                });
        },
        fetchData () {
            if (this.$route.params.id || this.$route.query.copyFrom) {
                this.isAdd = false;
                this.ajax
                    .post("/GroupBuyingV2/SelectGroupBuyingV2ConfigByGroupId", {
                        groupId: this.$route.params.id ? this.$route.params.id : this.$route.query.copyFrom,
                        isTireGroup: true
                    })
                    .then(response => {
                        if (response.data) {
                            this.tireGroupConfig = response.data;
                            this.tireGroupConfig.ProductGroupId = this.$route.params.id ? response.data.ProductGroupId : "";
                            this.tireGroupConfig.BeginTime = this.formatDate(response.data.BeginTime);
                            this.tireGroupConfig.EndTime = this.formatDate(response.data.EndTime);
                            let data = this.tireGroupConfig.Channel.split(";");
                            for (var i in data) {
                                if (data[i] === "kH5") {
                                    this.tireGroupConfig.H5Channel = true;
                                } else if (data[i] === "WXAPP") {
                                    this.tireGroupConfig.WXChannel = true;
                                }
                            }                           
                            this.noSave = false;
                        } else {
                            this.$Message.error("未查询到拼团配置信息");
                            this.noSave = true;
                        }
                    });
            }
        }
    },
    watch: {
        '$route' (to, from) {
            this.fetchData();
        }
    },
    created: function () {
        this.fetchData();
    }
}
</script>
