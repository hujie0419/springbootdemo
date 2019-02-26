<template>
    <div>
        <h1 class="title">拼团产品配置</h1>
        <div>
            <label class="ivu-form-item-label">商品名称：</label>
            <Input v-model="filters.ProductName" placeholder="商品名称" style="width: 200px"></Input>
            <label class="ivu-form-item-label" style="margin-left:15px">产品PID：</label>
            <Input v-model="filters.PID" placeholder="PID" style="width: 200px"></Input>
            <label class="ivu-form-item-label" style="margin-left:15px">创建人：</label>
            <Input v-model="filters.Creator" placeholder="创建人" style="width: 200px"></Input>
            <label class="ivu-form-item-label" style="margin-left:15px">GroupId：</label>
            <Input v-model="filters.ProductGroupId" placeholder="GroupId" style="width: 200px"></Input>
        </div>
        <div style="margin-top:18px">
            <label class="ivu-form-item-label">运营标签：</label>
            <Select v-model="filters.Label" placeholder="运营标签" style="width:200px">
                <Option :value="''" :key="''">全部</Option>
                <Option v-for="item in labelList" :value="item" :key="item.value">{{ item }}</Option>
            </Select>
            <label class="ivu-form-item-label" style="margin-left:13px">活动状态：</label>
            <Select v-model="activityStatus" placeholder="活动状态" style="width:200px">
                <Option v-for="item in activityStatusList" :value="item.value" :key="item.value">{{ item.label }}</Option>
            </Select>
            <label class="ivu-form-item-label" style="margin-left:15px">团类型：</label>
            <Select v-model="filters.GroupType" placeholder="拼团类型" style="width:200px">
                <Option :value="-1" :key="-1">全部</Option>
                <Option v-for="item in groupTypeList" :value="item.value" :key="item.value">{{ item.label }}</Option>
            </Select>
            <label class="ivu-form-item-label" style="margin-left:20px">团种类：</label>
            <Select v-model="filters.GroupCategory" placeholder="拼团种类" style="width:200px">
                <Option :value="-1" :key="-1">全部</Option>
                <Option v-for="item in groupCategoryList" :value="item.value" :key="item.value">{{ item.label }}</Option>
            </Select>
        </div>
        <div style="margin-top:18px">
            <label class="ivu-form-item-label">显示渠道：</label>
            <Select v-model="filters.Channel" placeholder="显示渠道" style="width:200px">
                <Option v-for="item in groupChannelList" :value="item.value" :key="item.value">{{ item.label }}</Option>
            </Select>
            <label class="ivu-form-item-label" style="margin-left:13px">是否显示：</label>
            <Select v-model="filters.IsShowPage" placeholder="是否显示" style="width:200px">
                <Option :value="-1" :key="-1">全部</Option>
                <Option :value="1" :key="1">显示</Option>
                <Option :value="0" :key="0">不显示</Option>
            </Select>
            <label class="ivu-form-item-label" style="margin-left:13px">显示顺序：</label>
            <Select v-model="filters.Sequence" placeholder="是否显示" style="width:200px">
                <Option :value="-1" :key="-1">全部</Option>
                <Option :value="1" :key="1">不为0</Option>
                <Option :value="0" :key="0">为0</Option>
            </Select>
            <label class="ivu-form-item-label" style="margin-left:13px">自动拼团：</label>
            <Select v-model="filters.IsAutoFinish" placeholder="是否自动拼团" style="width:200px">
                <Option :value="-1" :key="-1">全部</Option>
                <Option :value="1" :key="1">是自动拼团</Option>
                <Option :value="0" :key="0">不是自动拼团</Option>
            </Select>
        </div>
        <div style="margin-top:18px">
            <Button type="success" icon="search" @click="loadData(1)">搜索</Button>
            <Button type="success" icon="refresh" style="margin-left:20px" @click="reset">重置</Button>
            <Button type="success" icon="plus" style="margin-left:20px" @click="addProductConfig">创建</Button>
            <Button type="success" icon="plus" style="margin-left:20px" @click="addTireGroupConfig">轮胎拼团</Button>
            <Button type="success" icon="share" style="margin-left:20px" @click="ExportGroupBuyingCofig">导出</Button>
            <Button type="success" icon="load-c" style="margin-left:20px;" @click="RefreshTaskCache">刷新缓存</Button>
        </div>
        <div style="margin-top:18px">
            <Table :height="600" :loading="table.loading" :data="table.data" :columns="table.columns" stripe></Table>
            <div style="margin: 10px;overflow: hidden">
                <div style="float: right;">
                    <Page :total="page.total" :page-size="page.pageSize" :current="page.current" :page-size-opts="[5 ,10 ,20 ,50]" show-elevator show-sizer @on-change="handlePageChange" @on-page-size-change="handlePageSizeChange"></Page>
                </div>
            </div>
        </div>
        <Modal v-model="tablemodal.visible" title="库存列表" cancelText="取消" :height="300" scrollable :width="tablemodal.width">
            <Table :loading="tablemodal.loading" :data="tablemodal.data" :columns="tablemodal.columns" stripe></Table>
        </Modal>
        <Modal v-model="logmodal.visible" title="操作日志" cancelText="取消" scrollable :width="logmodal.width">
            <Table :loading="logmodal.loading" :data="logmodal.data" :columns="logmodal.columns" stripe></Table>
        </Modal>
        <Modal v-model="productmodal.visible" title="商品价格信息确认" cancelText="取消" @on-ok="ok(false)" scrollable :width="productmodal.width">
            <Table :loading="productmodal.loading" :data="productmodal.data" :columns="productmodal.columns" stripe></Table>
        </Modal>
        <Modal v-model="modal.visible" :mask-closable="false" :loading="modal.loading" title="拼团产品配置（编辑）" okText="提交" :transfer="false" cancelText="取消" @on-ok="ok(true)" scrollable width="50%">
            <Form ref="modal.productConfig" :model="modal.productConfig" :rules="modal.rules" :label-width="90">
                <FormItem label="GroupId">
                    <Input v-model="modal.productConfig.ProductGroupId" :disabled="true" placeholder="自动生成" />
                </FormItem>
                <FormItem label="上下架时间">
                    <Row>
                        <Col span="8">
                        <Date-Picker v-model="modal.productConfig.BeginTime" type="datetime" format="yyyy-MM-dd HH:mm:ss" transfer placeholder="上架时间"></Date-Picker>
                        </Col>
                        <Col span="8">
                        <Date-Picker v-model="modal.productConfig.EndTime" type="datetime" format="yyyy-MM-dd HH:mm:ss" transfer placeholder="下架时间"></Date-Picker>
                        </Col>
                    </Row>
                </FormItem>
                <FormItem label="列表页展示">
                    <i-switch v-model="modal.productConfig.IsShow" size="large">
                        <span slot="open">On</span>
                        <span slot="close">Off</span>
                    </i-switch>
                </FormItem>
                <FormItem label="是否自动拼团">
                    <i-switch v-model="modal.productConfig.IsAutoFinish" size="large" :true-value="1" :false-value="0" >
                        <span slot="open">On</span>
                        <span slot="close">Off</span>
                    </i-switch>
                </FormItem>
                <FormItem label="拼团人数">
                    <Input-Number v-model="modal.productConfig.MemberCount" :disabled="!isAdd" :max="8" :min="2" placeholder="拼团人数" />
                </FormItem>
                <FormItem label="拼团类型">
                    <Select v-model="modal.productConfig.GroupType" :disabled="!isAdd" placeholder="拼团类型" style="width:80%" transfer>
                        <Option v-for="item in groupTypeList" :value="item.value" :key="item.value">{{ item.label }}</Option>
                    </Select>
                </FormItem>
                <FormItem label="拼团种类">
                    <Select v-model="modal.productConfig.GroupCategory" :disabled="!isAdd" placeholder="拼团种类" style="width:80%" transfer>
                        <Option :value="0" :key="0">车品拼团</Option>
                        <Option :value="1" :key="1">抽奖拼团</Option>
                        <Option :value="2" :key="2">优惠券拼团</Option>
                    </Select>
                </FormItem>
                <FormItem label="运营标签">
                    <Select v-model="modal.productConfig.Label" placeholder="运营标签" style="width:23%" transfer>
                        <Option v-for="item in labelList" :value="item" :key="item.value">{{ item }}</Option>
                    </Select>
                </FormItem>
                <!-- <FormItem label="团库存上限" prop="TotalGroupCount">
                    <Col span="5">
                    <Input v-model="modal.productConfig.TotalGroupCount" placeholder="团库存上限" />
                    </Col>
                </FormItem>
                <FormItem label="已消耗库存" prop="CurrentGroupCount">
                    <Input v-model="modal.productConfig.CurrentGroupCount" :disabled="true" placeholder="已消耗团库存" />
                </FormItem> -->
                <!-- <FormItem label="特定人群" prop="SpecialUserTag">
                    <Input-Number v-model="modal.productConfig.SpecialUserTag" :max="100" :min="0" placeholder="特定人群" />
                </FormItem> -->
                <FormItem label="显示顺序">
                    <Input-Number v-model="modal.productConfig.Sequence" :max="10000000000" :min="0" placeholder="首页显示顺序" />
                </FormItem>
                <FormItem label="显示渠道">
                    <Checkbox v-model="modal.productConfig.H5Channel" name="Channel" data-key="kH5" @on-change="ChannelChange">H5</Checkbox>
                    <Checkbox v-model="modal.productConfig.WXChannel" name="Channel" data-key="WXAPP" @on-change="ChannelChange">微信小程序</Checkbox>
                </FormItem>
                <FormItem label="宣传图片">
                    <Col span="4" v-show="modal.productConfig.Image">
                    <a :href="modal.productConfig.Image" target="_blank"><img :src="modal.productConfig.Image" style='width:50px;height:50px'></a>
                    </Col>
                    <Col span="6">
                    <Upload action="/GroupBuyingV2/UploadImage?type=image" :format="['jpg','jpeg','png']" :on-format-error="handleFormatError" :max-size="10000" :on-exceeded-size="handleMaxSize" :on-success="handleSuccess" :show-upload-list="false">
                        <Button type="ghost" icon="ios-cloud-upload-outline">Upload files</Button>
                    </Upload>
                    </Col>
                    <Col span="5" v-show="modal.productConfig.Image">
                    <Button type="warning" icon="refresh" @click="modal.productConfig.Image=''">清除</Button>
                    </Col>
                </FormItem>
                <FormItem label="分享图片">
                    <Col span="4" v-show="modal.productConfig.ShareImage">
                    <a :href="modal.productConfig.ShareImage" target="_blank"><img :src="modal.productConfig.ShareImage" style='width:50px;height:50px'></a>
                    </Col>
                    <Col span="6">
                    <Upload action="/GroupBuyingV2/UploadImage?type=shareImage" :format="['jpg','jpeg','png']" :on-format-error="handleFormatError" :max-size="10000" :on-exceeded-size="handleMaxSize" :on-success="handleSuccess" :show-upload-list="false">
                        <Button type="ghost" icon="ios-cloud-upload-outline">Upload files</Button>
                    </Upload>
                    </Col>
                    <Col span="5" v-show="modal.productConfig.ShareImage">
                    <Button type="warning" icon="refresh" @click="modal.productConfig.ShareImage=''">清除</Button>
                    </Col>
                </FormItem>
                <FormItem label="分享文案">
                    <Input v-model="modal.productConfig.ShareId" placeholder="分享文案" />
                </FormItem>
                <FormItem label="抽奖规则描述">
                    <Input type="textarea" :rows="4" v-model="modal.productConfig.GroupDescription" placeholder="抽奖规则描述" />
                </FormItem>
                <FormItem v-if="isAdd">
                    <Button type="success" icon="search" @click="GetAllProducts()">获取产品信息</Button>
                    <Button type="info" icon="search" @click="SearchStock('','PID')">查看库存</Button>
                </FormItem>
                <FormItem  v-if="!isAdd">
                    <Button type="info" icon="search" @click="SearchStock(modal.productConfig.ProductGroupId,'GroupId')">查看库存</Button>
                    <Button type="success" icon="search" @click="RefreshProductConfigByGroupId(modal.productConfig.ProductGroupId)">刷新</Button>
                </FormItem>
                <FormItem v-for="(item, index) in modal.productConfig.GroupProductDetails" :label="'商品配置-'+(++index)" :key="index">
                    <Row>
                        <Col span="10">
                        <Input type="text" :disabled="!isAdd" v-model="item.PID" placeholder="商品PID" />
                        </Col>
                        <Col span="10" offset="1">
                        <Input type="text" v-model="item.ProductName" placeholder="商品名称" />
                        </Col>
                    </Row>
                    <Row>
                        <Col span="11">
                        <Checkbox v-model="item.UseCoupon">是否允许使用优惠券</Checkbox>
                        </Col>
                        <Col span="11">
                        <Checkbox v-model="item.DisPlay">是否用于默认展示</Checkbox>
                        </Col>
                    </Row>
                    <Row>
                        <Col span="11">
                        <Checkbox v-model="item.IsShow">是否显示（至少有一个商品显示)</Checkbox>
                        </Col>
                        <Col span="11">
                           <Checkbox v-model="item.IsShowApp">是否在APP详情页显示</Checkbox>
                        </Col>
                    </Row>
                    <!-- <Row>
                         <Col span="11">
                            <Checkbox v-model="item.IsAutoStock">是否自动库存</Checkbox>
                         </Col>
                    </Row> -->
                    <Row>
                        <Col span="4">
                        <label class="ivu-form-item-label">商品成本价：</label>
                        </Col>
                        <Col span="6">
                        <Input type="text" disabled v-model="item.CostPrice" placeholder="商品成本价：" />
                        </Col>
                        <!-- <Col span="4" offset="1">
                            <label class="ivu-form-item-label">商品售价：</label>
                        </Col>
                        <Col span="6">
                            <Input type="text" v-model="item.OriginalPrice" placeholder="商品售价" />
                        </Col> -->
                    </Row>
                    <Row>
                        <Col span="4">
                        <label class="ivu-form-item-label">商品活动价：</label>
                        </Col>
                        <Col span="6">
                        <Input type="text" v-model="item.FinalPrice" placeholder="商品活动价" />
                        </Col>
                        <Col span="4" offset="1">
                        <label class="ivu-form-item-label">商品团长价：</label>
                        </Col>
                        <Col span="6">
                        <Input type="text" v-model="item.SpecialPrice" placeholder="商品团长价" />
                        </Col>
                    </Row>
                    <Row>
                        <Col span="4">
                        <label class="ivu-form-item-label">每人限购单数：</label>
                        </Col>
                        <Col span="6">
                        <Input-Number v-model="item.BuyLimitCount" :max="100" :min="0" placeholder="每人限购单数：" />
                        </Col>
                        <Col span="4" offset="1">
                        <label class="ivu-form-item-label">每单限购数量：</label>
                        </Col>
                        <Col span="6">
                        <Input-Number v-model="item.UpperLimitPerOrder" :max="100" :min="1" placeholder="每单限购数量" />
                        </Col>
                    </Row>
                    <Row>
                        <Col span="4">
                        <label class="ivu-form-item-label">配置总库存：</label>
                        </Col>
                        <Col span="6">
                        <Input-Number v-model="item.TotalStockCount" :max="1000000" :min="0" placeholder="配置总库存：" />
                        </Col>
                        <Col span="4" offset="1">
                        <label class="ivu-form-item-label">产品已消耗库存：</label>
                        </Col>
                        <Col span="6">
                        <Input-Number :disabled="true"  v-model="item.CurrentSoldCount" :max="1000000" :min="0" placeholder="产品已消耗库存：" />
                        </Col>
                    </Row>
                </FormItem>
            </Form>
        </Modal>
    </div>
</template>

<script>
export default {
    data () {
        return {
            index: 0,
            tablemodal: {
                loading: true,
                visible: false,
                width: 1350,
                data: [],
                columns: [
                    {
                        title: "PID",
                        width: 100,
                        key: "PID",
                        align: "center",
                        fixed: "left"
                    },
                    {
                        title: "义乌可用库存",
                        width: 100,
                        key: "YiWuAvailableStockQuantity",
                        align: "center",
                        fixed: "left"
                    },
                    {
                        title: "义乌在途库存",
                        width: 100,
                        key: "YiWuZaituStockQuantity",
                        align: "center",
                        fixed: "left"
                    },
                    {
                        title: "上海可用库存",
                        width: 100,
                        key: "SHAvailableStockQuantity",
                        align: "center",
                        fixed: "left"
                    },
                    {
                        title: "上海在途库存",
                        width: 100,
                        key: "SHZaituStockQuantity",
                        align: "center",
                        fixed: "left"
                    },
                    {
                        title: "武汉可用库存",
                        width: 100,
                        key: "WHAvailableStockQuantity",
                        align: "center",
                        fixed: "left"
                    },
                    {
                        title: "武汉在途库存",
                        width: 100,
                        key: "WHZaituStockQuantity",
                        align: "center",
                        fixed: "left"
                    },
                    {
                        title: "天津可用库存",
                        width: 100,
                        key: "TianJinAvailableStockQuantity",
                        align: "center",
                        fixed: "left"
                    },
                    {
                        title: "天津在途库存",
                        width: 100,
                        key: "TianJinZaituStockQuantity",
                        align: "center",
                        fixed: "left"
                    },
                    {
                        title: "广州可用库存",
                        width: 100,
                        key: "GuangZhouAvailableStockQuantity",
                        align: "center",
                        fixed: "left"
                    },
                    {
                        title: "广州在途库存",
                        width: 100,
                        key: "GuangZhouZaituStockQuantity",
                        align: "center",
                        fixed: "left"
                    },
                    {
                        title: "总可用库存",
                        width: 100,
                        key: "TotalAvailableStockQuantity",
                        align: "center",
                        fixed: "left"
                    },
                    {
                        title: "总在途库存",
                        width: 100,
                        key: "TotalZaituStockQuantity",
                        align: "center",
                        fixed: "left"
                    }
                ]
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
                        key: "MethodType",
                        align: "center",
                        fixed: "left"
                    },
                    {
                        title: "消息",
                        width: 300,
                        key: "Msg",
                        align: "center",
                        fixed: "left"
                    },
                    {
                        title: "操作人",
                        width: 150,
                        key: "OperateUser",
                        align: "center",
                        fixed: "left"
                    },
                    {
                        title: "时间",
                        width: 200,
                        key: "CreateTime",
                        align: "center",
                        fixed: "left",
                        render: (h, params) => {
                            return h(
                                "span",
                                this.formatDate(params.row.CreateTime)
                            );
                        }
                    }
                ]
            },
            productmodal: {
                loading: false,
                visible: false,
                width: 980,
                data: [],
                columns: [
                    {
                        title: "PID",
                        width: 200,
                        key: "PID",
                        align: "center",
                        fixed: "left"
                    },
                    {
                        title: "活动价",
                        width: 150,
                        key: "FinalPrice",
                        align: "center",
                        fixed: "left"
                    },
                    {
                        title: "采购价格",
                        width: 150,
                        key: "PurchasePrice",
                        align: "center",
                        fixed: "left"
                    },
                    {
                        title: "代发价格",
                        width: 150,
                        key: "ContractPrice",
                        align: "center",
                        fixed: "left"
                    },
                    {
                        title: "采购优惠价",
                        width: 150,
                        key: "OfferPurchasePrice",
                        align: "center",
                        fixed: "left"
                    },
                    {
                        title: "代发优惠价",
                        width: 150,
                        key: "OfferContractPrice",
                        align: "center",
                        fixed: "left"
                    }
                ]
            },
            table: {
                loading: true,
                data: [],
                columns: [
                    // {
                    //     title: "#",
                    //     align: "center",
                    //     fixed: "left",
                    //     type: 'index'
                    // },
                    {
                        title: "GroupId",
                        key: "ProductGroupId"
                    },
                    {
                        title: "是否展示",
                        key: "IsShow",
                        render: (h, params) => {
                            return h("i-switch", {
                                props: {
                                    type: "primary",
                                    size: "small",
                                    value: params.row.IsShow
                                },
                                on: {
                                    "on-change": value => {
                                        this.UpdateProductConfigIsShow(
                                            params.row.ProductGroupId,
                                            value
                                        );
                                    }
                                }
                            });
                        }
                    },
                    {
                        title: "团类型",
                        key: "GroupType",
                        render: function (h, params) {
                            switch (params.row.GroupType) {
                                case 0:
                                    return h("span", "普通团");
                                case 1:
                                    return h("span", "新人团");
                                case 2:
                                    return h("span", "团长特价");
                                case 3:
                                    return h("span", "团长免单");
                                default:
                                    return h("span", "普通团");
                            }
                        }
                    },
                    {
                        title: "团种类",
                        key: "GroupCategory",
                        render: function (h, params) {
                            switch (params.row.GroupCategory) {
                                case 0:
                                    return h("span", "车品拼团");
                                case 1:
                                    return h("span", "抽奖拼团");
                                case 2:
                                    return h("span", "优惠券拼团");
                                case 3:
                                    return h("span", "轮胎拼团");
                                default:
                                    return h("span", "车品拼团");
                            }
                        }
                    },
                    {
                        title: "运营标签",
                        key: "Label"
                    },
                    // {
                    //     title: "显示渠道",
                    //     key: "Channel",
                    //     width: 50,
                    //     render: function (h, params) {
                    //         var result = "";
                    //         var data = params.row.Channel.split(";");
                    //         for (var i in data) {
                    //             if (data[i] === "kH5") {
                    //                 result += "H5";
                    //             } else if (data[i] === "WXAPP") {
                    //                 result += "  微信小程序";
                    //             }
                    //         }
                    //         return h("span", result);
                    //     }
                    // },
                    // {
                    //     title: "展示顺序",
                    //     key: "Sequence",
                    //     width: 60
                    // },
                    {
                        title: "商品PID    |   商品名称   |   活动价",
                        width: 450,
                        align: "center",
                        render: (h, params) => {
                            var dataArray = [];
                            if (params.row.GroupProductDetails.length > 0) {
                                for (
                                    var i = 0;
                                    i < params.row.GroupProductDetails.length;
                                    i++
                                ) {
                                    if (params.row.GroupProductDetails[i].DisPlay) {
                                    dataArray[i] = h("tr", [
                                        h(
                                            "td",
                                            {
                                                style: {
                                                    width: "200px"
                                                }
                                            },
                                            params.row.GroupProductDetails[i]
                                                .PID
                                        ),
                                        h(
                                            "td",
                                            {
                                                style: {
                                                    width: "200px"
                                                }
                                            },
                                            params.row.GroupProductDetails[i]
                                                .ProductName
                                        ),
                                        h(
                                            "td",
                                            {
                                                style: {
                                                    width: "50px"
                                                }
                                            },
                                            params.row.GroupProductDetails[i]
                                                .FinalPrice
                                        )
                                        // h(
                                        //     "td",
                                        //     {
                                        //         style: {
                                        //             width: "50px"
                                        //         }
                                        //     },
                                        //     params.row.GroupProductDetails[i]
                                        //         .IsShow ? "显示" : "不显示"
                                        // ),
                                        // h(
                                        //     "td",
                                        //     {
                                        //         style: {
                                        //             width: "50px"
                                        //         }
                                        //     },
                                        //     params.row.GroupProductDetails[i]
                                        //         .DisPlay ? "默认" : "不默认"
                                        // )
                                    ]);
                                    }
                                }
                            }
                            return h("table", [dataArray]);
                        }
                    },
                    // {
                    //     title: "已消耗库存",
                    //     key: "CurrentGroupCount",
                    //     width: 100
                    // },
                    // {
                    //     title: "库存上限",
                    //     key: "TotalGroupCount",
                    //     width: 100
                    // },
                    // {
                    //     title: "上架时间",
                    //     key: "BeginTime",
                    //     width: 100,
                    //     render: (h, params) => {
                    //         return h(
                    //             "span",
                    //             this.formatDate(params.row.BeginTime)
                    //         );
                    //     }
                    // },
                    // {
                    //     title: "下架时间",
                    //     key: "EndTime",
                    //     width: 100,
                    //     render: (h, params) => {
                    //         return h(
                    //             "span",
                    //             this.formatDate(params.row.EndTime)
                    //         );
                    //     }
                    // },
                    {
                        title: "操作",
                        key: "action",
                        align: "center",
                        // fixed: "right",
                        width: 200,
                        render: (h, params) => {
                            return h("div", [
                                h(
                                    "Button",
                                    {
                                        props: {
                                            type: "primary",
                                            size: "small"
                                        },
                                        style: {
                                            marginRight: "5px"
                                        },
                                        on: {
                                            click: () => {
                                                let operation = params.row.GroupCategory === 3 ? "TireSearch" : "Search";
                                                this.search(
                                                    params.row.ProductGroupId,
                                                    operation
                                                );
                                            }
                                        }
                                    },
                                    "修改"
                                ),
                                h(
                                    "Button",
                                    {
                                        props: {
                                            type: "primary",
                                            size: "small"
                                        },
                                        style: {
                                            marginRight: "5px"
                                        },
                                        on: {
                                            click: () => {
                                                let operation = params.row.GroupCategory === 3 ? "TireCopy" : "Copy";
                                                this.search(
                                                    params.row.ProductGroupId,
                                                    operation
                                                );
                                            }
                                        }
                                    },
                                    "复制"
                                ),
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
                                                    params.row.ProductGroupId
                                                );
                                            }
                                        }
                                    },
                                    "日志"
                                )
                            ]);
                        }
                    }
                ]
            },
            filters: {
                Label: "",
                ProductGroupId: "",
                ProductName: "",
                PID: "",
                Creator: "",
                GroupType: -1,
                GroupCategory: -1,
                Channel: "None",
                IsShowPage: 1,
                Sequence: -1,
                IsAutoFinish: -1
            },
            activityStatus: "Ongoing",
            page: {
                total: 10,
                current: 1,
                pageSize: 10
            },
            isAdd: false,
            labelList: [
                "优惠券团",
                "抽奖团",
                "1分团",
                "低价团",
                "精品团",
                "团长免单",
                "团长特价",
                "优惠券团+团长特价团"
            ],
            activityStatusList: [
                {
                    label: "全部",
                    value: "All"
                },
                {
                    label: "进行中",
                    value: "Ongoing"
                },
                {
                    label: "已结束",
                    value: "Ending"
                }
            ],
            groupTypeList: [
                {
                    label: "普通团",
                    value: 0
                },
                {
                    label: "新人团",
                    value: 1
                },
                {
                    label: "团长特价",
                    value: 2
                },
                {
                    label: "团长免单",
                    value: 3
                }
            ],
            groupCategoryList: [
                {
                    label: "车品拼团",
                    value: 0
                },
                {
                    label: "轮胎拼团",
                    value: 3
                },
                {
                    label: "抽奖拼团",
                    value: 1
                },
                {
                    label: "优惠券拼团",
                    value: 2
                }
            ],
            groupChannelList: [
                {
                    label: "全部",
                    value: "None"
                },
                {
                    label: "H5",
                    value: "H5"
                },
                {
                    label: "微信小程序",
                    value: "WXAPP"
                }
            ],
            modal: {
                visible: false,
                loading: true,
                edit: true,
                productConfig: {
                    ProductGroupId: "",
                    BeginTime: "",
                    EndTime: "2999-01-01",
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
                    GroupCategory: 0,
                    GroupDescription: "",
                    Label: "优惠券团",
                    GroupProductDetails: [
                        {
                            PID: "",
                            ProductName: "",
                            UpperLimitPerOrder: 1,
                            BuyLimitCount: 0,
                            DisPlay: false,
                            UseCoupon: false,
                            IsShow: true,
                            OriginalPrice: "",
                            CostPrice: "",
                            FinalPrice: "",
                            SpecialPrice: "",
                            TotalStockCount: 1000000,
                            CurrentSoldCount: 0,
                            IsAutoStock: false,
                            IsShowApp: false
                        }
                    ]
                },
                rules: {}
            }
        };
    },
    created: function () {
        this.loadData(1);
    },
    methods: {
        loadData (pageIndex) {
            this.page.current = pageIndex;
            this.table.loading = true;
            this.ajax
                .get(
                "/GroupBuyingV2/SelectGroupBuyingV2Config?filterStr=" +
                JSON.stringify(this.filters) +
                "&pageIndex=" +
                this.page.current +
                "&pageSize=" +
                this.page.pageSize +
                "&activityStatus=" +
                this.activityStatus
                )
                .then(response => {
                    var data = response.data;
                    this.page.total = data.totalCount;
                    this.table.data = data.data;
                    this.table.loading = false;
                });
        },
        reset () {
            this.filters.Label = "";
            this.filters.ProductGroupId = "";
            this.filters.PID = "";
            this.filters.ProductName = "";
            this.activityStatus = "Ongoing";
            this.filters.Creator = "";
            this.filters.GroupType = -1;
            this.filters.GroupCategory = -1;
            this.filters.Channel = "None";
            this.filters.IsShowPage = 1;
            this.filters.Sequence = -1;
            this.filters.IsAutoFinish = -1;
        },
        formatDateForfmt (date) {
            var seperator1 = "-";
            var year = date.getFullYear();
            var month = date.getMonth() + 1;
            var strDate = date.getDate();
            if (month >= 1 && month <= 9) {
                month = "0" + month;
            }
            if (strDate >= 0 && strDate <= 9) {
                strDate = "0" + strDate;
            }
            var currentdate = year + seperator1 + month + seperator1 + strDate;
            return currentdate;
        },
        addProductConfig () {
            this.isAdd = true;
            this.modal.productConfig = {
                ProductGroupId: "",
                BeginTime: "",
                EndTime: "",
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
                GroupCategory: 0,
                GroupDescription: "",
                Label: "优惠券团",
                GroupProductDetails: [
                    {
                        PID: "",
                        ProductName: "",
                        UpperLimitPerOrder: 1,
                        BuyLimitCount: 0,
                        DisPlay: true,
                        UseCoupon: false,
                        IsShow: true,
                        OriginalPrice: "0",
                        CostPrice: "0",
                        FinalPrice: "0",
                        SpecialPrice: "0",
                        TotalStockCount: 1000000,
                        CurrentSoldCount: 0,
                        IsAutoStock: false,
                        isShowSearch: true,
                        IsShowApp: false
                    }
                ]
            };
            this.modal.visible = true;
            this.modal.productConfig.BeginTime = this.formatDate(new Date());
            this.modal.productConfig.EndTime = this.formatDate(new Date(2099, 0, 1, 0, 0, 0));
        },
        addTireGroupConfig () {
            this.$router.push({
                name: 'CreateTireGroupConfig'
            });
        },
        handlePageChange (pageIndex) {
            this.loadData(pageIndex);
        },
        handlePageSizeChange (pageSize) {
            this.page.pageSize = pageSize;
            this.loadData(this.page.current);
        },
        ExportGroupBuyingCofig () {
            window.open(
                "/GroupBuyingV2/ExportGroupBuyingCofig?filterStr=" +
                JSON.stringify(this.filters) +
                "&activityStatus=" +
                this.activityStatus
            );
        },
        ok (isJudge) {
            this.modal.loading = true;
            this.modal.productConfig.BeginTime = this.formatDate(this.modal.productConfig.BeginTime);
            this.modal.productConfig.EndTime = this.formatDate(this.modal.productConfig.EndTime);
            this.ajax
                .post("/GroupBuyingV2/UpsertGroupBuyingConfig", {
                    productConfigJson: JSON.stringify(this.modal.productConfig),
                    isJudge: isJudge
                })
                .then(response => {
                    if (response.data.status) {
                        this.$Message.success("操作成功");
                        this.modal.visible = false;
                        this.loadData(this.page.current);
                    } else if (response.data.priceData) {
                        this.modal.loading = false;
                        this.$nextTick(() => {
                            this.modal.loading = true;
                        });
                        this.$Message.error(response.data.msg);
                        this.productmodal.data = response.data.priceData;
                        this.productmodal.visible = true;
                    } else {
                        this.modal.loading = false;
                        this.$nextTick(() => {
                            this.modal.loading = true;
                        });
                        this.$Message.error(response.data.msg);
                    }
                });
        },
        UpdateProductConfigIsShow (groupId, isShow) {
            this.table.loading = true;
            this.ajax
                .post("/GroupBuyingV2/UpdateProductConfigIsShow", {
                    groupId: groupId,
                    isShow: isShow
                })
                .then(response => {
                    if (response.data) {
                        this.$Message.success("操作成功");
                        this.loadData(this.page.current);
                    } else {
                        this.$Message.error("操作失败");
                    }
                });
        },
        ChannelChange (channel) {
            this.modal.productConfig.channel = "";
            if (this.modal.productConfig.H5Channel) {
                this.modal.productConfig.channel += "kH5;";
            }
            if (this.modal.productConfig.WXChannel) {
                this.modal.productConfig.channel += "WXAPP";
            }
        },
        search (groupId, type) {
            if (type === 'TireSearch') {
                this.$router.push({
                    path: `/ProductConfig/TireGroup/${groupId}`
                });
            } else if (type === 'TireCopy') {
                this.$router.push({
                    path: `/ProductConfig/TireGroup?copyFrom=${groupId}`
                });
            } else {
                this.ajax
                    .post("/GroupBuyingV2/SelectGroupBuyingV2ConfigByGroupId", {
                        groupId: groupId
                    })
                    .then(response => {
                        this.modal.productConfig = response.data;
                        this.modal.visible = true;
                        this.isAdd = false;
                        this.modal.productConfig.BeginTime = this.formatDate(this.modal.productConfig.BeginTime);
                        this.modal.productConfig.EndTime = this.formatDate(this.modal.productConfig.EndTime);
                        var data = this.modal.productConfig.Channel.split(";");
                        for (var i in data) {
                            if (data[i] === "kH5") {
                                this.modal.productConfig.H5Channel = true;
                            } else if (data[i] === "WXAPP") {
                                this.modal.productConfig.WXChannel = true;
                            }
                        }
                        if (type === "Copy") {
                            this.isAdd = true;
                            this.modal.productConfig.ProductGroupId = "";
                        }
                    });
            }
        },
        RefreshProductConfigByGroupId (groupId) {
            this.ajax
                .post("/GroupBuyingV2/RefreshProductConfigByGroupId", {
                    groupId: groupId,
                    isLottery:
                    (this.modal.productConfig.GroupCategory === 1)
                })
                .then(response => {
                    this.modal.productConfig.GroupProductDetails = response.data;
                });
        },
        GetAllProducts () {
            if (this.modal.productConfig.GroupProductDetails[0].PID) {
                this.ajax
                    .post(
                    "/GroupBuyingV2/GetProductsByPIDAndIsLottery?pid=" +
                    this.modal.productConfig.GroupProductDetails[0]
                        .PID +
                    "&isLottery=" +
                    (this.modal.productConfig.GroupCategory === 1)
                    )
                    .then(response => {
                        this.modal.productConfig.GroupProductDetails = [];
                        if (response.data.length > 0) {
                            for (var i = 0; i < response.data.length; i++) {
                                this.modal.productConfig.GroupProductDetails.push(
                                    {
                                        PID: response.data[i].PID,
                                        ProductName:
                                        response.data[i].DisplayName,
                                        UpperLimitPerOrder: 1,
                                        BuyLimitCount: 0,
                                        CostPrice: response.data[i].CostPrice,
                                        OriginalPrice:
                                        response.data[i].CY_List_Price,
                                        FinalPrice: response.data[i].CY_List_Price,
                                        SpecialPrice: response.data[i].CY_List_Price,
                                        isShowSearch: i === 0,
                                        UseCoupon: true,
                                        IsShow: true,
                                        DisPlay: i === 0,
                                        TotalStockCount: 1000000,
                                        CurrentSoldCount: 0,
                                        IsAutoStock: false,
                                        IsShowApp: false
                                    }
                                );
                            }
                        } else {
                            this.modal.productConfig.GroupProductDetails = [
                                {
                                    PID: "",
                                    ProductName: "",
                                    UpperLimitPerOrder: 1,
                                    BuyLimitCount: 0,
                                    DisPlay: true,
                                    UseCoupon: true,
                                    IsShow: true,
                                    OriginalPrice: "0",
                                    CostPrice: "0",
                                    FinalPrice: "0",
                                    SpecialPrice: "0",
                                    isShowSearch: true,
                                    TotalStockCount: 1000000,
                                    CurrentSoldCount: 0,
                                    IsAutoStock: false,
                                    IsShowApp: false
                                }
                            ]
                        }
                    });
            } else {
                this.$Message.error("请输入产品PID");
            }
        },
        RefreshTaskCache () {
            this.ajax.post("/GroupBuyingV2/RefreshTaskCache").then(response => {
                this.$Message.success(response.data);
            });
        },
        SearchStock (groupId, type) {
            this.tablemodal.loading = true;
            this.ajax
                .post("/GroupBuyingV2/GetStockInfoByPIDs", {
                    groupId: groupId,
                    pid: this.modal.productConfig.GroupProductDetails[0].PID,
                    type: type,
                    isLottery: this.modal.productConfig.GroupCategory === 1
                })
                .then(response => {
                    this.tablemodal.data = response.data;
                    this.tablemodal.visible = true;
                    this.tablemodal.loading = false;
                });
        },
        SearchLog (groupId) {
            this.logmodal.loading = true;
            this.ajax
                .post("/GroupBuyingV2/GetLogByGroupId", {
                    groupId: groupId
                })
                .then(response => {
                    this.logmodal.data = response.data;
                    this.logmodal.visible = true;
                    this.logmodal.loading = false;
                });
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
                    this.modal.productConfig.ShareImage = res.ImageUrl;
                } else {
                    this.modal.productConfig.Image = res.ImageUrl;
                }
            } else {
                this.$Message.warning(res.Msg);
            }
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
};
</script>

<style>
body .ivu-modal .ivu-select-dropdown {
    position: fixed !important;
}
</style>

<style scoped>
.ivu-form-item-label {
    font-size: unset;
}
</style>
