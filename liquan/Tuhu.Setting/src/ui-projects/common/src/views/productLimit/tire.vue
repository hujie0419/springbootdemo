<template>
    <div class="content-wrapper ivu-layout-content">
        <Form>
            <slot name="head"></slot>
            <FormItem label="限购数量">
                <InputNumber v-model="model.limitCount" :max="9999" :min="0"></InputNumber>
                <!-- 数字输入框 -->
                <Label @click="handleShowCategoryLog()"><a href="javascript:;" style="margin-left:10px">操作日志</a></Label>
            </FormItem>
            <FormItem>
                <Button type="primary" :loading="btnLoading" @click="handleSubmit">保存</Button>
                <Button type="success" @click="refreshCache" style="margin-left:10px">刷新缓存</Button>
            </FormItem>
        </Form>
        <!-- 操作日志 start -->
        <Modal ref="logModal" v-model="logmodal.visible" title="日志" cancelText="取消" :width="logmodal.width" :styles="{'max-height':'800px'}" :z-index="2" @on-visible-change="modalChange">
            <Table :loading="logmodal.loading" :columns="logmodal.columns" :data="logmodal.data" stripe></Table>
        </Modal>
        <!-- 操作日志 end -->
    </div>
</template>
<script>
export default {
    name: "productLimit",
    data() {
        return {
            model: {
                limitCount: 0
            },
            btnLoading: false,
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
    created() {
        this.getLimitCount();
    },
    methods: {
        /**
         *
         */
        getLimitCount() {
            var param = "name=轮胎&code=Tires&level=1";
            this.ajax
                .get("/ProductLimitTire/FetchCategoryLimitCount?" + param)
                .then(res => {
                    if (res.data.success) {
                        this.model.limitCount = res.data.data.LimitCount;
                    } else {
                        this.$Message.warning("数据初始化失败");
                    }
                })
                .catch(res => {
                    this.$Message.warning("数据初始化失败");
                });
        },
        /**
         *
         */
        handleSubmit() {
            this.btnLoading = true;
            this.ajax
                .post("/ProductLimitTire/SaveCategoryLimitCount", {
                    CategoryCode: "Tires",
                    CategoryName: "轮胎",
                    CategoryLevel: 1,
                    LimitCount: this.model.limitCount
                })
                .then(res => {
                    if (res.data.success) {
                        this.$Message.success("设置成功");
                    } else {
                        this.$Message.warning("设置失败");
                    }
                    this.btnLoading = false;
                })
                .catch(res => {
                    this.$Message.warning("设置失败");
                    this.btnLoading = false;
                });
        },
        /**
         * 显示日志记录
         */
        handleShowCategoryLog() {
            this.logmodal.loading = true;
            this.util.ajax
                .post(`/ProductLimitTire/GetCommonConfigLogs`, {
                    category: "Tires",
                    level: 1
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
         * 刷新缓存
         */
        refreshCache() {
            this.util.ajax.post("/ProductLimitTire/RefreshCache").then(res => {
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