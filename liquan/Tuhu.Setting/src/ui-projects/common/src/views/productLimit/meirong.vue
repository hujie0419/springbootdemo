<template>
    <div class="content-wrapper" ref="main">
        <!-- 左侧树 start-->
        <div class="left">
            <Tree :data="rootList" :render="renderContent"></Tree>
        </div>
        <!-- 左侧树 end-->
        <!-- 右侧主题内容 start -->
        <div class="right" ref="right">
            <Breadcrumb v-if="showBread">
                <BreadcrumbItem>{{breadText}}</BreadcrumbItem>
            </Breadcrumb>
            <div class="search-wrap">
                <!-- 查询表单 start -->
                <Form :model="searchModel" :label-width="80">
                    <div class="form-item">
                        <Select v-model="searchModel.Brand" placeholder="请选择类目品牌" clearable style="width:200px;margin-right:10px">
                            <Option v-for="item in searchModel.BrandList" :value="item" :key="item">{{ item }}</Option>
                        </Select>
                        <RadioGroup v-model="searchModel.OnSale">
                            <Radio label="">
                                <span>全部</span>
                            </Radio>
                            <Radio label="1">
                                <span>上架</span>
                            </Radio>
                            <Radio label="0">
                                <span>下架</span>
                            </Radio>
                        </RadioGroup>
                    </div>
                    <div class="form-item">
                        <Input style="width:200px;margin-right:10px" clearable v-model="searchModel.Pid" placeholder="请输入PID"></Input>
                        <Input style="width:200px" clearable v-model="searchModel.ProductName" placeholder="请输入产品名称"></Input>
                    </div>
                    <div class="form-item">
                        <Button type="primary" @click="searchProductList">查询</Button>
                        <Button type="success" @click="handleBatchSetting" style="margin-left:10px">批量设置</Button>
                        <Button type="success" @click="refreshCache" style="margin-left:10px">刷新缓存</Button>
                    </div>
                </Form>
                <!-- 查询表单 end -->
            </div>
            <!-- 产品列表 start -->
            <div class="grid">
                <Table ref="gridTable" border stripe :loading="table.loading" :columns="table.cols" :data="table.list">
                </Table>
            </div>
            <!-- 产品列表 end -->
            <div class="pager">
                <Page :total="page.total" :page-size="page.pageSize" :current="page.current" :page-size-opts="[10 ,30 ,50]" show-sizer @on-change="handlePageChange" @on-page-size-change="handlePageSizeChange"></Page>
            </div>
        </div>
        <!-- 右侧主题内容 end -->
        <!-- 编辑分类限购数量 start -->
        <Modal v-model="categoryModal.visible" :loading="categoryModal.loading" title="修改限购数量" @on-ok="submitCategory" :z-index="1">
            <Form :model="categoryModal.model" :label-width="100">
                <FormItem label="当前类目">
                    <Label v-model="categoryModal.model.type">{{categoryModal.model.type}}</Label>
                </FormItem>
                <FormItem label="限购数量">
                    <Input v-model="categoryModal.model.CategoryCode" style="display:none"></Input>
                    <Input v-model="categoryModal.model.CategoryName" style="display:none"></Input>
                    <Input v-model="categoryModal.model.CategoryLevel" style="display:none"></Input>
                    <InputNumber v-model="categoryModal.model.LimitCount" :max="9999" :min="0"></InputNumber>
                    <Label @click="handleShowCategoryLog(categoryModal.model.CategoryCode,categoryModal.model.CategoryLevel)" style="margin-left:10px">
                        <a href="javascript:;">操作日志</a>
                    </Label>
                </FormItem>
            </Form>
        </Modal>
        <!-- 编辑分类限购数量 end -->
        <!-- 编辑产品限购数量 start -->
        <Modal v-model="productModal.visible" :loading="productModal.loading" title="修改限购数量" okText="保存" @on-ok="submitProductLimitMR">
            <Form :model="productModal.model" :label-width="100">
                <FormItem label="PID">
                    <Label v-model="productModal.model.PID">{{productModal.model.PID}}</Label>
                </FormItem>
                <FormItem label="产品名称">
                    <Label v-model="productModal.model.ProductName">{{productModal.model.ProductName}}</Label>
                </FormItem>
                <FormItem label="限购数量">
                    <Input v-model="productModal.model.CategoryCode" style="display:none"></Input>
                    <Input v-model="productModal.model.CategoryName" style="display:none"></Input>
                    <Input v-model="productModal.model.CategoryLevel" style="display:none"></Input>
                    <InputNumber v-model="productModal.model.LimitCount" :max="9999" :min="0"></InputNumber>
                </FormItem>
            </Form>
        </Modal>
        <!-- 编辑产品限购数量 end -->
        <!-- 批量编辑产品限购数量 start -->
        <Modal v-model="batchModal.visible" :loading="batchModal.loading" title="修改限购数量" okText="保存" @on-ok="submitBatchProductLimitMR">
            <Form :model="batchModal.model" :label-width="100">
                <FormItem label="限购数量">
                    <InputNumber v-model="batchModal.model.LimitCount" :max="9999" :min="0"></InputNumber>
                </FormItem>
            </Form>
        </Modal>
        <!-- 批量编辑产品限购数量 end -->
        <!-- 操作日志 start -->
        <Modal v-model="logmodal.visible" title="日志" cancelText="取消" :width="logmodal.width" :styles="{'max-height':'800px'}" :z-index="2" @on-visible-change="modalChange">
            <Table :loading="logmodal.loading" :columns="logmodal.columns" :data="logmodal.data" stripe></Table>
        </Modal>
        <!-- 操作日志 end -->
    </div>
</template>
<script>
export default {
    name: "ProductLimitMR",
    data() {
        return {
            // 面包屑
            showBread: false,
            breadText: "",
            // 类目树
            rootList: [],
            // 查询字段
            searchModel: {
                Pid: "",
                Brand: "",
                OnSale: "",
                Category: "",
                PageIndex: 1,
                BrandList: [],
                ProductName: ""
            },
            // 产品列表
            table: {
                pageSize: 20,
                pageIndex: 1,
                totalSize: 1,
                loading: false,
                list: [],
                cols: [
                    {
                        type: "selection",
                        width: 60,
                        align: "center"
                    },
                    {
                        title: "PID",
                        align: "left",
                        width: 200,
                        key: "Pid"
                    },
                    {
                        title: "产品名称",
                        align: "left",
                        key: "ProductName"
                    },
                    {
                        title: "限购数量",
                        align: "right",
                        width: 100,
                        key: "LimitCount"
                    },
                    {
                        title: "操作",
                        key: "action",
                        width: 100,
                        align: "center",
                        render: (h, params) => {
                            return h(
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
                                            this.editProduct(params.row);
                                        }
                                    }
                                },
                                "编辑"
                            );
                        }
                    },
                    {
                        title: "操作历史",
                        key: "action",
                        width: 100,
                        align: "center",
                        render: (h, params) => {
                            return h(
                                "Button",
                                {
                                    props: {
                                        type: "error",
                                        size: "small"
                                    },
                                    on: {
                                        click: () => {
                                            this.handleShowProductLog(
                                                params.row.PKID
                                            );
                                        }
                                    }
                                },
                                "查看"
                            );
                        }
                    }
                ]
            },
            page: {
                total: 0,
                current: 1,
                pageSize: 10
            },
            // 产品列表选中数据
            selectModel: [],
            // 类目属性
            categoryModal: {
                visible: false,
                loading: true,
                model: {
                    type: "",
                    LimitCount: 0,
                    CategoryName: "",
                    CategoryCode: "",
                    CategoryLevel: ""
                }
            },
            // 产品属性
            productModal: {
                visible: false,
                loading: true,
                model: {
                    PID: "",
                    ProductName: "",
                    LimitCount: 0,
                    CategoryName: "",
                    CategoryCode: "",
                    CategoryLevel: ""
                }
            },
            // 批量编辑产品
            batchModal: {
                visible: false,
                loading: true,
                model: {
                    LimitCount: 0
                }
            },
            // 日志属性
            logmodal: {
                loading: true,
                visible: false,
                width: 800,
                data: [],
                columns: [
                    {
                        title: "操作人",
                        key: "Creator"
                    },
                    {
                        title: "操作内容",
                        key: "Remark"
                    },
                    {
                        title: "操作前",
                        key: "BeforeValue"
                    },
                    {
                        title: "操作后",
                        key: "AfterValue"
                    },
                    {
                        title: "操作时间",
                        key: "CreateDateTime",
                        render: (h, params) => {
                            return h(
                                "span",
                                this.util.formatDate(params.row.CreateDateTime)
                            );
                        }
                    }
                ]
            }
        };
    },
    mounted() {
        this.$refs.main.style.height =
            ((window.outerHeight - 72 - 35) / window.outerHeight) * 100 + "%";
        this.$refs.main.style.top = 72 + "px";
        this.$refs.main.style.bottom = 35 + "px";
        this.$refs.right.style.maxHeight =
            (window.outerHeight / window.outerHeight) * 100 + "%";
        this.$refs.right.style.minHeight =
            (window.outerHeight / window.outerHeight) * 100 + "%";
        this.getRootTree();
    },
    methods: {
        /**
         * 加载类目树结构
         */
        getRootTree() {
            this.ajax
                .get("/ProductLimitMR/GetCatrgoryTree?category=MR1")
                .then(res => {
                    if (res.data.Success) {
                        this.rootList = res.data.Data;
                    } else {
                        this.$Message.warning("类目数据初始化失败");
                    }
                })
                .catch(res => {
                    this.$Message.warning("类目数据初始化失败");
                });
        },
        /**
         * 渲染自定义树控件
         */
        renderContent(h, { root, node, data }) {
            return h("span", [
                h("span", data.title),
                h(
                    "span",
                    {
                        style: {
                            float: "right",
                            marginRight: "20px"
                        }
                    },
                    [
                        h(
                            "span",
                            {
                                style: {
                                    marginRight: "5px"
                                }
                            },
                            "限购数 : " + data.limitCount
                        ),
                        h(
                            "a",
                            {
                                style: {
                                    marginRight: "3px"
                                },
                                on: {
                                    click: () => {
                                        this.editCategory(data);
                                    }
                                }
                            },
                            "[修改]"
                        ),
                        h(
                            "a",
                            {
                                on: {
                                    click: () => {
                                        this.setProductLimitMRCnt(data);
                                    }
                                }
                            },
                            "[设置产品]"
                        )
                    ]
                )
            ]);
        },
        /**
         * 修改类目限购数量
         */
        editCategory(data) {
            this.categoryModal.visible = true;
            this.categoryModal.model.type = data.levelName;
            this.categoryModal.model.CategoryCode = data.value;
            this.categoryModal.model.CategoryName = data.title;
            this.categoryModal.model.CategoryLevel = data.level;
            this.categoryModal.model.LimitCount = data.limitCount;
        },
        /**
         * 提交修改的类目限购数量
         */
        submitCategory() {
            let param = this.categoryModal.model;
            this.categoryModal.loading = true;

            this.ajax
                .post("/ProductLimitMR/SaveCategoryLimitCount", {
                    ...param
                })
                .then(res => {
                    if (res.data.success) {
                        this.$Message.success("设置成功");
                        this.categoryModal.loading = false;
                        this.categoryModal.visible = false;
                        this.$nextTick(() => {
                            this.categoryModal.loading = true;
                        });
                        this.getRootTree();
                    } else {
                        this.$Message.warning("设置失败");
                        this.categoryModal.loading = false;
                        this.$nextTick(() => {
                            this.categoryModal.loading = true;
                        });
                    }
                })
                .catch(res => {
                    this.$Message.warning("设置失败");
                    this.categoryModal.visible = false;
                    this.categoryModal.loading = false;
                });
        },
        /**
         * 编辑产品
         */
        editProduct(data) {
            let category = this.categoryModal.model;
            this.productModal.model.PID = data.Pid;
            this.productModal.model.LimitCount = data.LimitCount;
            this.productModal.model.ProductName = data.ProductName;
            this.productModal.model.CategoryCode = category.CategoryCode;
            this.productModal.model.CategoryName = category.CategoryName;
            this.productModal.model.CategoryLevel = category.CategoryLevel;
            this.productModal.visible = true;
        },
        /**
         * 修改产品限购
         */
        submitProductLimitMR() {
            var that = this;
            let param = that.productModal.model;
            param.CategoryCode="MR1";
            
            that.ajax
                .post("/ProductLimitMR/SaveProductLimitCount", {
                    ...param
                })
                .then(res => {
                    if (res.data.success) {
                        that.$Message.success("设置成功");
                        that.productModal.loading = false;
                        that.productModal.visible = false;
                        this.$nextTick(() => {
                            that.productModal.loading = true;
                        });
                        this.loadData();
                    } else {
                        that.$Message.warning("设置失败");
                        that.productModal.loading = false;
                        that.productModal.visible = false;
                        this.$nextTick(() => {
                            that.productModal.loading = true;
                        });
                    }
                })
                .catch(res => {
                    that.$Message.warning("设置失败");
                    that.productModal.loading = false;
                    that.productModal.visible = false;
                    this.$nextTick(() => {
                        that.productModal.loading = true;
                    });
                });
        },
        /**
         * 搜索产品列表数据
         */
        loadData() {
            this.table.loading = true;
            this.ajax
                .post("/ProductLimitMR/GetProductListByCagegoryCode", {
                    pid: this.searchModel.Pid,
                    pageIndex: this.page.current,
                    pageSize: this.page.pageSize,
                    onSale: this.searchModel.OnSale,
                    brandName: this.searchModel.Brand,
                    category: this.searchModel.Category,
                    productName: this.searchModel.ProductName
                })
                .then(res => {
                    if (res.data.Success) {
                        this.page.total = res.data.totalCount;
                        this.table.list = res.data.Data;
                        this.table.loading = false;
                    }
                })
                .catch(res => {
                    this.table.loading = false;
                    this.$Message.warning("数据初始化失败");
                });
        },
        /**
         * 加载类目下的品牌选项
         */
        loadBrandList(val) {
            this.ajax
                .post("/ProductLimitMR/GetBrandsByCategory", {
                    category: val
                })
                .then(res => {
                    if (res.data.Success) {
                        this.searchModel.BrandList = res.data.Data;
                    }
                })
                .catch(res => {
                    this.table.loading = false;
                    this.$Message.warning("品牌数据初始化失败");
                });
        },
        /**
         *  根据条件查询类目下产品数据
         */
        setProductLimitMRCnt(data) {
            this.categoryModal.model.CategoryLevel = data.level;
            this.categoryModal.model.CategoryCode = data.value;
            this.categoryModal.model.CategoryName = data.title;
            this.searchModel.Category = data.value;
            this.showBread = true;
            this.breadText = data.levelName;
            this.loadData();
            this.loadBrandList(data.value);
        },
        /**
         * 根据条件查询产品列表
         */
        searchProductList() {
            this.page.current = 1;
            this.loadData();
        },
        /**
         * 表格分页
         */
        handlePageChange(pageIndex) {
            this.page.current = pageIndex;
            this.loadData();
        },
        /**
         * 切换表格每页显示数量
         */
        handlePageSizeChange(pageSize) {
            this.page.current = 1;
            this.page.pageSize = pageSize;
            this.loadData();
        },
        /**
         * 显示日志
         */
        handleShowProductLog(pkid) {
            this.logmodal.loading = true;
            this.util.ajax
                .post(`/CommonConfigLog/GetCommonConfigLogs`, {
                    objectId: pkid,
                    objectType: "ProductLimitCountLog"
                })
                .then(res => {
                    if (typeof res.data == "object" && res.data) {
                        this.logmodal.data = res.data;
                    } else {
                        this.logmodal.data = [];
                    }
                    this.logmodal.visible = true;
                    this.logmodal.loading = false;
                });
        },
        /**
         * 显示日志记录
         */
        handleShowCategoryLog(category, level) {
            this.logmodal.loading = true;
            this.util.ajax
                .post(`/ProductLimitMR/GetCommonConfigLogs`, {
                    category,
                    level
                })
                .then(res => {
                    if (typeof res.data == "object" && res.data) {
                        this.logmodal.data = res.data;
                    } else {
                        this.logmodal.data = [];
                    }
                    this.logmodal.visible = true;
                    this.logmodal.loading = false;
                });
        },
        /**
         * 批量设置
         */
        handleBatchSetting() {
            var chkList = this.$refs.gridTable.getSelection();
            if (chkList != null && chkList.length > 0) {
                chkList.forEach(item => {
                    item.CategoryName = this.categoryModal.model.CategoryName;
                    item.CategoryCode = this.categoryModal.model.CategoryCode;
                    item.CategoryLevel = this.categoryModal.model.CategoryLevel;
                });
                this.selectModel = chkList;
                this.batchModal.visible = true;
            } else {
                this.$Message.warning("请选择要设置的产品");
            }
        },
        /**
         * 提交批量设置
         */
        submitBatchProductLimitMR() {
            if (this.selectModel.length <= 0) {
                this.$Message.warning("请选择要设置的产品");
                return;
            }
            this.selectModel.forEach(item => {
                (item.LimitCount = this.batchModal.model.LimitCount),
                    (item.CategoryName = this.categoryModal.model.CategoryName);
                item.CategoryCode = 'MR1';
                item.CategoryLevel = this.categoryModal.model.CategoryLevel;
            });

            this.ajax
                .post("/ProductLimitMR/BatchSaveProductLimitCount", {
                    model: JSON.stringify(this.selectModel)
                })
                .then(res => {
                    if (res.data.success) {
                        this.$Message.success("设置成功");
                        this.batchModal.visible = false;
                        this.batchModal.loading = false;
                        this.$nextTick(() => {
                            this.batchModal.loading = true;
                        });
                        this.loadData();
                    } else {
                        this.$Message.warning("设置失败", res.data.pidList);
                        this.batchModal.visible = false;
                        this.batchModal.loading = false;
                        this.$nextTick(() => {
                            this.batchModal.loading = true;
                        });
                    }
                })
                .catch(res => {
                    this.$Message.warning("设置失败");
                    this.batchModal.visible = false;
                    this.batchModal.loading = false;
                    this.$nextTick(() => {
                        this.batchModal.loading = true;
                    });
                });
        },
        /**
         * 刷新缓存
         */
        refreshCache() {
            this.util.ajax.post("/ProductLimitMR/RefreshCache").then(res => {
                if (res.data.success) {
                    this.$Message.success("刷新成功");
                } else {
                    this.$Message.warning("刷新失败");
                }
            });
        },
        /**
         * 日志弹框超过800像素出现滚动条
         */
        modalChange() {
            this.$nextTick(() => {
                if (this.$refs["logModal"] != undefined) {
                    var height = this.$refs["logModal"].$el.children[1]
                        .scrollHeight;
                    if (height > 800) {
                        this.$refs[
                            "logModal"
                        ].$el.children[1].children[0].style.overflowY =
                            "scroll";
                    }
                }
            });
        }
    }
};
</script>
<style lang="less" scoped>
.left {
    height: 800px;
    width: 24%;
    float: left;
    overflow-y: scroll;
}
.right {
    float: right;
    height: 800px;
    width: 75%;
    margin-left: 1%;
    overflow-x: scroll;
}
.ivu-form-item {
    margin-bottom: 10px;
}
.form-item {
    padding: 5px 0;
}
.grid {
    margin-top: 10px;
}
.pager {
    margin: 10px 0;
    float: right;
}
</style>
