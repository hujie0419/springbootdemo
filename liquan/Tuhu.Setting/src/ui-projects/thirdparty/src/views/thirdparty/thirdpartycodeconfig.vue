<template>
    <div>
        <h1 class="title">三方码配置</h1>
        <Form>
            <Row>
                <FormItem >
                    <i-Button @click="addThirdPartyCodeConfig" type="primary">添加</i-Button>
                </FormItem>
            </Row>
        </Form>
        <Table border :loading="table.loading" :columns="table.columns" :data="table.data"></Table>
        <div style="margin-top:15px;float:right">
            <Page :total="page.total"
                :page-size="page.pageSize"
                :current="page.current"
                :page-size-opts="[10, 20 ,50 ,100]"
                show-elevator
                show-sizer
                @on-change="handlePageChange"
                @on-page-size-change="handlePageSizeChange"></Page>
        </div>
         <Modal v-model="modal.visible" :mask-closable="false" :loading="modal.loading" title="三方码配置（编辑）" 
            okText="提交" :transfer="false" cancelText="取消" @on-ok="ok()" scrollable width="30%">
            <Form ref="modal.codeConfig" :model="modal.codeConfig">
                <FormItem label="名称">
                    <i-Input v-model="modal.codeConfig.SourceName" :width="150"  placeholder="名称" ></i-Input>
                </FormItem>
                <FormItem label="正则表达式">
                    <i-Input v-model="modal.codeConfig.SourceRegex" :width="150"  placeholder="正则表达式" ></i-Input>
                </FormItem>
                <FormItem label="码来源">
                    <i-Input v-model="modal.codeConfig.Source" :disabled="!modal.isAdd" :width="150"   placeholder="码来源" ></i-Input>
                </FormItem>
                <FormItem label="备注">
                    <Input type="textarea" :rows="4" v-model="modal.codeConfig.Remarks" placeholder="备注" />
                </FormItem>
            </Form>
        </Modal>
    </div>
</template>
<script>
export default {
    data () {
        return {
            filterCondition: {
                Remarks: "",
                SourceRegex: "",
                Source: ""
            },
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
                        title: "PKID",
                        key: "PKID",
                        align: "center"
                    },
                    {
                        title: "渠道名称",
                        key: "SourceName",
                        align: "center",
                        render: (h, params) => {
                            if (params.row.SourceName) {
                                return h("span", params.row.SourceName);
                            } else {
                                return h("span", "/");
                            }
                        }
                    },
                    {
                        title: "正则表达式",
                        key: "SourceRegex",
                        align: "center",
                        render: (h, params) => {
                            if (params.row.SourceRegex) {
                                return h("span", params.row.SourceRegex);
                            } else {
                                return h("span", "/");
                            }
                        }
                    },
                    {
                        title: "码来源",
                        key: "Source",
                        align: "center",
                        render: (h, params) => {
                            if (params.row.Source) {
                                return h(
                                    "span",
                                    params.row.Source
                                );
                            } else {
                                return h("span", "/");
                            }
                        }
                    },
                    {
                        title: "备注",
                        key: "Remarks",
                        align: "center"
                    },
                    {
                        title: "创建时间",
                        key: "CreatedDateTime",
                        align: "center",
                        render: (h, params) => {
                            return h(
                                "span",
                                this.formatDate(params.row.CreatedTime)
                            );
                        }
                    },
                    {
                        title: "更新时间",
                        key: "UpdatedDateTime",
                        align: "center",
                        render: (h, params) => {
                            return h(
                                "span",
                                this.formatDate(params.row.UpdatedTime)
                            );
                        }
                    },
                    {
                        title: "操作",
                        key: "action",
                        width: 150,
                        align: "center",
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
                                                this.editCodeConfig(params.row);
                                            }
                                        }
                                    },
                                    "编辑"
                                )
                            ]);
                        }
                    }
                ]
            },
            modal: {
                visible: false,
                loading: true,
                edit: true,
                isAdd: true,
                codeConfig: {
                    PKID: 0,
                    SourceRegex: "",
                    Source: "",
                    Remarks: ""
                }
            }
        }
    },
    created () {
        this.loadData();
    },
    methods: {
        loadData () {
            this.table.loading = true;
            var self = this;
             self.ajax.get("/ThirdPartyCode/GetCodeSourceConfig", {
                        params: {
                            pageIndex: self.page.current,
                            pageSize: self.page.pageSize
                        }
                    })
                    .then(response => {
                        var data = response.data;
                        self.page.total = data.total;
                        self.table.data = data.data;
                        self.table.loading = false;
                    })
        },
        addThirdPartyCodeConfig () {
            var self = this;
            self.modal.visible = true;
            self.modal.isAdd = true;
            self.modal.codeConfig.PKID = 0;
            self.modal.codeConfig.SourceRegex = "";
            self.modal.codeConfig.Source = "";
            self.modal.codeConfig.Remarks = "";
            self.modal.codeConfig.SourceName = "";
        },
        editCodeConfig (item) {
            var self = this;
            self.modal.visible = true;
            self.modal.isAdd = false;
            self.modal.codeConfig.PKID = item.PKID;
            self.modal.codeConfig.Remarks = item.Remarks;
            self.modal.codeConfig.SourceRegex = item.SourceRegex;
            self.modal.codeConfig.Source = item.Source;
            self.modal.codeConfig.SourceName = item.SourceName;
        },
        handlePageChange (pageIndex) {
            this.page.current = pageIndex;
            this.loadData();
        },
        handlePageSizeChange (pageSize) {
            this.page.pageSize = pageSize;
            this.loadData();
        },
        ok () {
            var self = this;
            self.modal.loading = true;
            self.ajax.post("/ThirdPartyCode/UpsertCodeSourceConfig", { config: self.modal.codeConfig })
                .then(response => {
                    if (response.data.status) {
                        this.$Message.success("操作成功");
                        this.modal.visible = false;
                        this.loadData();
                    } else {
                        this.modal.loading = false;
                        this.$nextTick(() => {
                            this.modal.loading = true;
                        });
                        this.$Message.error(response.data.msg);
                    }
                })
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
