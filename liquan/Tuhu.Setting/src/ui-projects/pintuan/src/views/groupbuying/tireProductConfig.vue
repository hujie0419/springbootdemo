<template>
    <div>
        <h1 class="title">{{ $route.params.id }} 轮胎拼团配置</h1>
        <Card>
            <Form>
                <FormItem>
                    <Row>
                        <Col>
                            <Button type="success" icon="plus" @click="modal.visible=true">添加商品</Button>
                            <Button type="warning" icon="close" @click="deleteTireProducts(table.selected)">批量删除</Button>
                            <Button type="info" icon="edit" @click="batchUpdateBuyLimitCount">批量修改每人限购单数</Button>
                            <Button type="info" icon="edit" @click="batchUpdateUpperLimitPerOrder">批量修改每单限购数量</Button>
                            <Button type="info" icon="edit" @click="batchUpdateTotalStockCount">批量修改总限购</Button>
                            <Button type="info" icon="edit" @click="batchUpdateUseCoupon">批量修改是否可用券</Button>
                            <span style="float: right;">
                                <Input v-model="query" placeholder="请输入PID或者商品名称" style="width: 250px"></Input>
                                <Button type="success" icon="search" @click="getTireProducts">搜索</Button>
                            </span>
                        </Col>
                    </Row>
                </FormItem>
                <FormItem style="margin-bottom: 0;">
                    <Row>
                        <Col>
                            <Table :loading="table.loading" :data="table.data" :columns="table.columns" border @on-selection-change="handleSelectionChange"></Table>
                        </Col>
                        <Col>
                            共 {{ table.data.length }} 条
                        </Col>
                    </Row>
                </FormItem>
                <FormItem style="text-align: center; margin-bottom: 12px;">
                    <Row>
                        <Col>
                            <Button @click="goBack">上一步</Button>
                            <Button type="primary" @click="saveTireProducts">保存</Button>
                        </Col>
                    </Row>
                </FormItem>
            </Form>
            <Modal v-model="modal.visible" :loading="modal.loading" title="添加轮胎" okText="添加" cancelText="取消" :mask-closable="false" :transfer="false" @on-ok="addTireProducts" @on-cancel="resetSearchParms(true)" width="80%">
                <Form ref="modal.query" :model="modal.query" :label-width="90">
                    <FormItem>
                        <Row>
                            <Col span="6" style="margin-right: 12px;">
                                商品PID：
                                <Input v-model="modal.query.pid" placeholder="请输入PID" style="width: 210px;" />
                            </Col>
                            <Col span="4">
                                <Button type="info" @click="searchTireProductsByPid(1)">搜索</Button>
                                <Button type="info" @click="modal.query.pid=''">重置</Button>
                            </Col>
                        </Row>
                    </FormItem>
                    <FormItem>
                        <Row>
                            <Col span="6" style="margin-right: 12px;">
                                轮胎品牌：
                                <Select v-model="modal.query.brands" @on-change="getTireSizesWithPatterns" multiple transfer style="width: 210px;">
                                    <Option v-for="item in tireBrandList" :value="item" :key="item">{{ item }}</Option>
                                </Select>
                            </Col>
                            <Col span="6">
                                轮胎花纹：
                                <Select v-model="modal.query.pattern" transfer style="width: 200px;">
                                    <Option v-for="item in tirePatternList" :value="item" :key="item">{{ item }}</Option>
                                </Select>
                            </Col>
                            <Col span="7">
                                轮胎规格：
                                <Select v-model="modal.query.width" placeholder="胎面宽" transfer style="width: 80px;">
                                    <Option v-for="item in tireSizeList.widths" :value="item" :key="item">{{ item }}</Option>
                                </Select>
                                <Select v-model="modal.query.aspectRatio" placeholder="扁平比" transfer style="width: 80px;">
                                    <Option v-for="item in tireSizeList.aspectRatios" :value="item" :key="item">{{ item }}</Option>
                                </Select>
                                <Select v-model="modal.query.rim" placeholder="直径" transfer style="width: 80px;">
                                    <Option v-for="item in tireSizeList.rims" :value="item" :key="item">{{ item }}</Option>
                                </Select>
                            </Col>
                            <Col span="4">
                                <Button type="info" @click="searchTireProductsByQuery(1)">搜索</Button>
                                <Button type="info" @click="resetSearchParms(false, true)" style="margin-left: 5px;">重置</Button>
                            </Col>
                        </Row>
                    </FormItem>
                    <FormItem>
                        <Row>
                            <Col>
                                <Table :loading="modal.table.loading" :data="modal.table.data" :columns="modal.table.columns" border @on-selection-change="handleModalSelectionChange"></Table>
                            </Col>
                        </Row>
                    </FormItem>
                    <FormItem>
                        <Row>
                            <Col>
                                <Page :total="modal.page.total" :page-size="modal.page.pageSize" :current="modal.page.current" show-elevator @on-change="handleModalPageChange" style="float: left;" />
                                <span style="float: right;">共 {{ modal.page.total }} 条，每页 10 条</span>
                            </Col>
                        </Row>
                    </FormItem>
              </Form>
            </Modal>
        </Card>
    </div>
</template>

<script>
export default {
    data () {
        return {
            tireProducts: [],
            tireBrandList: [],
            tirePatternList: [],
            tireSizeList: {
                widths: [],
                aspectRatios: [],
                rims: []
            },
            query: "",
            table: {
                selected: [],
                loading: false,
                data: [],
                columns: [
                    {
                        type: "selection",
                        align: "center",
                        width: 60
                    },
                    {
                        title: "商品PID",
                        key: "PID",
                        align: "center",
                        width: 200
                    },
                    {
                        title: "显示名称",
                        key: "ProductName",
                        align: "center",
                        width: 300,
                        render: (h, params) => {
                            return h("Input", {
                                props: {
                                    value: params.row.ProductName
                                },
                                on: {
                                    'on-change': (event) => {
                                        this.isModifiedTable = true;
                                        this.tireProducts[params.index].ProductName = event.target.value;
                                    }
                                }
                            })
                        }
                    },
                    {
                        title: "品牌",
                        key: "TireBrand",
                        align: "center",
                        width: 150
                    },
                    {
                        title: "花纹",
                        key: "TirePattern",
                        align: "center",
                        width: 100
                    },
                    {
                        title: "规格",
                        key: "TireSize",
                        align: "center",
                        width: 120,
                        render: function (h, params) {
                            let tireSize = `${params.row.TireWidth}/${params.row.TireAspectRatio} R${params.row.TireRim}`;
                            return h("span", tireSize);
                        }
                    },
                    {
                        title: "途虎价",
                        key: "OriginalPrice",
                        align: "center",
                        width: 100
                    },
                    {
                        title: "成本价",
                        key: "CostPrice",
                        align: "center",
                        width: 100
                    },
                    {
                        title: "拼团价",
                        key: "FinalPrice",
                        align: "center",
                        width: 120,
                        render: (h, params) => {
                            return h("Input", {
                                props: {
                                    value: params.row.FinalPrice
                                },
                                on: {
                                    'on-blur': (event) => {
                                        this.isModifiedTable = true;
                                        this.tireProducts[params.index].FinalPrice = event.target.value;
                                        if (isNaN(event.target.value) || event.target.value <= 0) {
                                            this.$Message.warning("请输入正确的拼团价");
                                        } else {
                                            this.tireProducts[params.index].SpecialPrice = event.target.value;
                                            this.table.data = JSON.parse(JSON.stringify(this.tireProducts));
                                        }
                                    }
                                }
                            })
                        }
                    },
                    {
                        title: "团长价",
                        key: "SpecialPrice",
                        align: "center",
                        width: 120,
                        render: (h, params) => {
                            return h("Input", {
                                props: {
                                    value: params.row.SpecialPrice
                                },
                                on: {
                                    'on-blur': (event) => {
                                        this.isModifiedTable = true;
                                        this.tireProducts[params.index].SpecialPrice = event.target.value;
                                        if (isNaN(event.target.value) || event.target.value <= 0) {
                                            this.$Message.warning("请输入正确的团长价");
                                        }
                                    }
                                }
                            })
                        }
                    },
                    {
                        title: "每人限购单数",
                        key: "BuyLimitCount",
                        align: "center",
                        width: 120,
                        render: (h, params) => {
                            return h("Input-Number", {
                                props: {
                                    value: params.row.BuyLimitCount,
                                    min: 0
                                },
                                on: {
                                    'on-change': (value) => {
                                        this.isModifiedTable = true;
                                        this.tireProducts[params.index].BuyLimitCount = value;
                                    }
                                }
                            })
                        }
                    },
                    {
                        title: "每单限购数量",
                        key: "UpperLimitPerOrder",
                        align: "center",
                        width: 120,
                        render: (h, params) => {
                            return h("Input-Number", {
                                props: {
                                    value: params.row.UpperLimitPerOrder,
                                    min: 1
                                },
                                on: {
                                    'on-change': (value) => {
                                        this.isModifiedTable = true;
                                        this.tireProducts[params.index].UpperLimitPerOrder = value;
                                    }
                                }
                            })
                        }
                    },
                    {
                        title: "总限购",
                        key: "TotalStockCount",
                        align: "center",
                        width: 120,
                        render: (h, params) => {
                            return h("Input-Number", {
                                props: {
                                    value: params.row.TotalStockCount,
                                    min: 0
                                },
                                on: {
                                    'on-change': (value) => {
                                        this.isModifiedTable = true;
                                        this.tireProducts[params.index].TotalStockCount = value;
                                    }
                                }
                            })
                        }
                    },
                    {
                        title: "剩余限购",
                        key: "UsableStockCount",
                        align: "center",
                        width: 100
                    },
                    {
                        title: "是否可用券",
                        key: "UseCoupon",
                        align: "center",
                        width: 100,
                        render: (h, params) => {
                            return h("i-switch", {
                                props: {
                                    size: "large",
                                    value: params.row.UseCoupon
                                },
                                on: {
                                    'on-change': (value) => {
                                        this.isModifiedTable = true;
                                        this.tireProducts[params.index].UseCoupon = value;
                                    }
                                }
                            });
                        }
                    },
                    {
                        title: "是否默认",
                        key: "DisPlay",
                        align: "center",
                        width: 100,
                        render: function (h, params) {
                            if (params.row.DisPlay) {
                                return h("span", "默认");
                            } else {
                                return h("span", "-");
                            }
                        }
                    },
                    {
                        title: "上下架",
                        key: "OnSale",
                        align: "center",
                        width: 100,
                        render: function (h, params) {
                            if (params.row.OnSale) {
                                return h("span", "上架");
                            } else {
                                return h("span", "下架");
                            }
                        }
                    },
                    {
                        title: "操作",
                        key: "Action",
                        fixed: "right",
                        width: 150,
                        align: "center",
                        render: (h, params) => {
                            return h("div", [
                                h("Button", {
                                    props: {
                                        type: "error",
                                        size: "small",
                                        disabled: params.row.DisPlay
                                    },
                                    style: {
                                        marginRight: "5px"
                                    },
                                    on: {
                                        click: () => {
                                            this.deleteTireProducts(
                                                params.row.ProductConfigID,
                                                params.row.PID
                                            );
                                        }
                                    }
                                }, "删除"),
                                h("Button", {
                                    props: {
                                        type: "primary",
                                        size: "small",
                                        disabled: params.row.DisPlay
                                    },
                                    on: {
                                        click: () => {
                                            this.setDefaultProduct(
                                                this.$route.params.id,
                                                params.row.PID
                                            );
                                        }
                                    }
                                }, "设为默认")
                            ]);
                        }
                    }
                ]
            },
            modal: {
                visible: false,
                loading: true,
                query: {
                    pid: "",
                    brands: "",
                    pattern: "",
                    width: "",
                    aspectRatio: "",
                    rim: ""
                },
                table: {
                    selected: [],
                    loading: false,
                    data: [],
                    columns: [
                        {
                            type: "selection",
                            align: "center",
                            width: 60
                        },
                        {
                            title: "商品PID",
                            key: "Pid",
                            align: "center",
                            width: 200
                        },
                        {
                            title: "显示名称",
                            key: "DisplayName",
                            align: "center"
                        },
                        {
                            title: "途虎价",
                            key: "Price",
                            align: "center",
                            width: 100
                        },
                        {
                            title: "上下架",
                            key: "Onsale",
                            align: "center",
                            width: 100,
                            render: function (h, params) {
                                if (params.row.Onsale) {
                                    return h("span", "上架");
                                } else {
                                    return h("span", "下架");
                                }
                            }
                        }
                    ]
                },
                page: {
                    total: 0,
                    current: 1,
                    pageSize: 10
                }
            },
            isModifiedTable: false,
            batchUpdateValue: null
        }
    },
    methods: {
        getTireBrands () {
            let that = this;
            this.ajax
                .get("/GroupBuyingV2/GetTireBrands")
                .then(function (response) {
                    if (response.data) {
                        that.tireBrandList = response.data;
                    } else {
                        that.tireBrandList = [];
                        that.$Message.warning("获取轮胎品牌列表失败");
                    }
                });
        },
        getTireSizesWithPatterns (value) {
            let that = this;
            that.clearTireSizesWithPatterns();
            this.ajax
                .post("/GroupBuyingV2/GetTireSizesWithPatterns", {
                    bands: value
                })
                .then(function (response) {
                    if (response.data.tireSizes && response.data.tirePatterns) {
                        that.tireSizeList = response.data.tireSizes;
                        that.tirePatternList = response.data.tirePatterns;
                    } else {
                        that.$Message.warning("获取轮胎规格、花纹失败");
                    }
                });
        },
        clearTireSizesWithPatterns () {
            this.modal.query.pattern = "";
            this.modal.query.width = "";
            this.modal.query.aspectRatio = "";
            this.modal.query.rim = "";

            this.tirePatternList = [];
            this.tireSizeList.widths = [];
            this.tireSizeList.aspectRatios = [];
            this.tireSizeList.rims = [];
        },
        searchTireProducts (query) {
            this.modal.table.selected = [];
            this.modal.table.loading = true;
            this.modal.page.current = query.pageIndex;

            let that = this;
            this.ajax
                .post("/GroupBuyingV2/SearchTireProducts", query)
                .then(function (response) {
                    console.log(response.data);
                    that.modal.page.total = response.data.Total;
                    that.modal.table.data = response.data.Source;
                    that.modal.table.loading = false;
                });
        },
        searchTireProductsByPid (pageIndex) {
            var pid = this.modal.query.pid.trim();
            if (pid === "") {
                this.$Message.warning("请输入商品PID");
            } else {
                this.searchTireProducts({
                    pid: pid,
                    pageIndex: pageIndex
                });
            }
        },
        searchTireProductsByQuery (pageIndex) {
            this.searchTireProducts({
                brands: this.modal.query.brands,
                pattern: this.modal.query.pattern,
                width: this.modal.query.width,
                aspectRatio: this.modal.query.aspectRatio,
                rim: this.modal.query.rim,
                pageIndex: pageIndex
            });
        },
        resetSearchParms (withTable, withoutPid) {
            if (!withoutPid) {
                this.modal.query.pid = "";
            }
            this.modal.query.brands = "";
            this.clearTireSizesWithPatterns();
            if (withTable) {
                this.modal.table.data = [];
                this.modal.table.selected = [];
                this.modal.page.total = 0;
                this.modal.page.current = 1;
            }
        },
        handleModalPageChange (pageIndex) {
            this.searchTireProductsByQuery(pageIndex);
        },
        handleModalSelectionChange (selection) {
            let pids = [];
            selection.forEach(item => {
                pids.push(item.Pid);
            });
            this.modal.table.selected = pids;
        },
        resetModalLoading () {
            this.modal.loading = false;
            this.$nextTick(() => {
                this.modal.loading = true;
            });
        },
        addTireProducts () {
            if (this.modal.table.selected.length > 0) {
                let that = this;
                this.ajax
                    .post("/GroupBuyingV2/AddGroupBuyingTireProducts", {
                        productGroupId: that.$route.params.id,
                        pids: that.modal.table.selected
                    })
                    .then(function (response) {
                        if (response.data.Success) {
                            that.$Message.success("添加成功");
                            that.modal.visible = false;
                            that.resetSearchParms(true);
                            that.getTireProducts();
                        } else {
                            that.$Message.warning(response.data.Message);
                            that.resetModalLoading();
                        }
                    });
                } else {
                    this.$Message.warning("请筛选轮胎商品");
                    this.resetModalLoading();
                }
        },
        getTireProducts () {
            this.table.loading = true;
            this.table.selected = [];

            let that = this;
            this.ajax
                .post("/GroupBuyingV2/GetGroupBuyingTireProducts", {
                    productGroupId: that.$route.params.id,
                    query: that.query.trim()
                })
                .then(function (response) {
                    // 深拷贝轮胎列表（解决Input-Number控件触发onchange事件后失去焦点）
                    that.tireProducts = JSON.parse(JSON.stringify(response.data));

                    that.table.data = response.data;
                    that.table.data.forEach(item => {
                        if (item.DisPlay) {
                            item._disabled = true; 
                        }
                    });
                    that.table.loading = false;
                });
        },
        saveTireProducts () {
            if (this.tireProducts.length <= 0) {
                this.$Message.warning("请添加轮胎商品");
                return;
            }

            let products = [];
            for (var i = 0; i < this.tireProducts.length; i++) {
                if (this.tireProducts[i].ProductName.trim() === "") {
                    this.$Message.warning(`第${i + 1}行显示名称为空`);
                    return;
                }
                if (isNaN(this.tireProducts[i].FinalPrice) ||
                    parseFloat(this.tireProducts[i].FinalPrice) <= 0) {
                    this.$Message.warning(`第${i + 1}行拼团价有误`);
                    return;
                }
                if (isNaN(this.tireProducts[i].SpecialPrice) ||
                    parseFloat(this.tireProducts[i].SpecialPrice) <= 0) {
                    this.$Message.warning(`第${i + 1}行团长价有误`);
                    return;
                }
                if (this.tireProducts[i].BuyLimitCount === null) {
                    this.$Message.warning(`第${i + 1}行每人限购单数为空`);
                    return;
                }
                if (this.tireProducts[i].UpperLimitPerOrder === null) {
                    this.$Message.warning(`第${i + 1}行每单限购数量为空`);
                    return;
                }
                if (this.tireProducts[i].TotalStockCount === null) {
                    this.$Message.warning(`第${i + 1}行总限购为空`);
                    return;
                }

                products.push({
                    PID: this.tireProducts[i].PID,
                    PKID: this.tireProducts[i].ProductConfigID,
                    DisPlay: this.tireProducts[i].DisPlay,
                    ProductName: this.tireProducts[i].ProductName,
                    FinalPrice: this.tireProducts[i].FinalPrice,
                    SpecialPrice: this.tireProducts[i].SpecialPrice,
                    BuyLimitCount: this.tireProducts[i].BuyLimitCount,
                    UpperLimitPerOrder: this.tireProducts[i].UpperLimitPerOrder,
                    TotalStockCount: this.tireProducts[i].TotalStockCount,
                    UseCoupon: this.tireProducts[i].UseCoupon
                });
            }

            let that = this;
            this.ajax
                .post("/GroupBuyingV2/SaveGroupBuyingTireProducts", {
                    productGroupId: that.$route.params.id,
                    configs: products
                })
                .then(function (response) {
                    if (response.data.Success) {
                        that.isModifiedTable = false;
                        that.$Message.success("保存成功");
                        setTimeout(() => {
                            that.$router.push({
                                name: "ProductConfig"
                            });
                        }, 500);
                    } else {
                        that.$Message.warning(response.data.Message);
                    }
                });
        },
        setDefaultProduct (productGroupId, pid) {
            let that = this;
            this.ajax
                .post("/GroupBuyingV2/SetGroupBuyingDefaultProduct", {
                    productGroupId, pid
                })
                .then(function (response) {
                    if (response.data.Success) {
                        that.$Message.success("设置成功");
                        that.getTireProducts();
                    } else {
                        that.$Message.warning("设置默认商品失败");
                    }
                });
        },
        handleSelectionChange (selection) {
            let configIds = [];
            selection.forEach(item => {
                configIds.push(item.ProductConfigID);
            });
            this.table.selected = configIds;
        },
        deleteTireProducts (productConfigIds, pid) {
            if (pid === undefined && productConfigIds.length === 0) {
                this.$Message.warning("请选择轮胎商品");
                return;
            }

            this.$Modal.confirm({
                title: "提示",
                content: `您确实要删除${pid === undefined ? "选中商品" : pid}吗？`,
                onOk: () => {
                    let that = this;
                    this.ajax
                        .post("/GroupBuyingV2/DeleteGroupBuyingTireProducts", {
                            productGroupId: that.$route.params.id,
                            productConfigIds: productConfigIds
                        })
                        .then(function (response) {
                            if (response.data.Success) {
                                that.$Message.success("删除成功");
                                that.getTireProducts();
                            } else {
                                that.$Modal.error({
                                    title: "删除失败",
                                    content: response.data.Message
                                });
                            }
                        });
                }
            });
        },
        batchUpdateBuyLimitCount () {
            this.batchUpdateValue = 0;
            this.$Modal.confirm({
                render: (h) => {
                    return h('Input-Number', {
                        props: {
                            min: 0,
                            value: 0
                        },
                        style: {
                            width: "100%"
                        },
                        on: {
                            'on-change': (value) => {
                                this.batchUpdateValue = value;
                            }
                        }
                    })
                },
                onOk: () => {
                    this.batchUpdateConfig(
                        "BuyLimitCount",
                        this.batchUpdateValue
                    );
                }
            })
        },
        batchUpdateUpperLimitPerOrder () {
            this.batchUpdateValue = 1;
            this.$Modal.confirm({
                render: (h) => {
                    return h('Input-Number', {
                        props: {
                            min: 1,
                            value: 1
                        },
                        style: {
                            width: "100%"
                        },
                        on: {
                            'on-change': (value) => {
                                this.batchUpdateValue = value;
                            }
                        }
                    })
                },
                onOk: () => {
                    this.batchUpdateConfig(
                        "UpperLimitPerOrder",
                        this.batchUpdateValue
                    );
                }
            })
        },
        batchUpdateTotalStockCount () {
            this.batchUpdateValue = 10000;
            this.$Modal.confirm({
                render: (h) => {
                    return h('Input-Number', {
                        props: {
                            min: 0,
                            value: 10000
                        },
                        style: {
                            width: "100%"
                        },
                        on: {
                            'on-change': (value) => {
                                this.batchUpdateValue = value;
                            }
                        }
                    })
                },
                onOk: () => {
                    this.batchUpdateConfig(
                        "TotalStockCount",
                        this.batchUpdateValue
                    );
                }
            })
        },
        batchUpdateUseCoupon () {
            this.batchUpdateValue = true;
            this.$Modal.confirm({
                render: (h) => {
                    return h('Select', {
                        props: {
                            value: 1,
                            transfer: true
                        },
                        style: {
                            width: "100%"
                        },
                        on: {
                            'on-change': (value) => {
                                this.batchUpdateValue = value === 1;
                            }
                        }
                    },
                    [
                        h('Option', {
                            props: {
                                value: 1
                            }
                        }, '可用券'),
                        h('Option', {
                            props: {
                                value: 0
                            }
                        }, '不可用券')
                    ])
                },
                onOk: () => {
                    this.batchUpdateConfig(
                        "UseCoupon",
                        this.batchUpdateValue
                    );
                }
            })
        },
        batchUpdateConfig (columnName, value) {
            let that = this;
            this.ajax
                .post("/GroupBuyingV2/BatchUpdateGroupBuyingProduct", {
                    productGroupId: that.$route.params.id,
                    columnName: columnName,
                    value: value
                })
                .then(function (response) {
                    if (response.data.Success) {
                        that.$Message.success("修改成功");
                        that.getTireProducts();
                    } else {
                        that.$Message.warning("修改失败");
                    }
                });
        },
        goBack () {
            if (this.isModifiedTable) {
                this.$Modal.confirm({
                    title: "提示",
                    content: '返回上一步将导致未保存内容丢失，确实要继续吗？',
                    onOk: () => {
                        this.$router.push({
                            path: `/ProductConfig/TireGroup/${this.$route.params.id}`
                        });
                    }
                });
            } else {
                this.$router.push({
                    path: `/ProductConfig/TireGroup/${this.$route.params.id}`
                });
            }
        }
    },
    created: function () {
        this.getTireBrands();
        this.getTireProducts();
    }
}
</script>

<style lang="less" scoped>
.ivu-form .ivu-form-item:first-child {
    button:not(:first-child) {
        margin-left: 5px;
    }
}

.ivu-modal {
    .ivu-form-item {
        margin-bottom: 12px;
    }
}
</style>
