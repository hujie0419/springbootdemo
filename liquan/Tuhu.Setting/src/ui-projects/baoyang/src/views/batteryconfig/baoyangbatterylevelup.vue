 <template>
  <div>
    <h2 class="title">蓄电池升级购配置</h2>
    <div>
      <Row type="flex"
           align="middle">
        <i-col span="5"
               style="text-align:left">
          <label>产品PID : </label>
          <i-input class="filter-element"
                   v-model="parameters.pid"
                   placeholder="请输入产品ID"></i-input>
        </i-col>
        <i-col span="9">
          <Button type="primary"
                  @click="search">查询</Button>
          <Button type="success"
                  @click="add">添加</Button>
          <Button type="primary"
                  @click="RemoveCache">清除缓存</Button>
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
        <FormItem label="原始产品">
          <Input type="text"
                 v-model="modal.formItems.OriginalPID"
                 @on-blur="changeDescribe(modal.formItems.OriginalPID)"></Input>
          <label>{{describe.OriginalDescribe}}</label>
        </FormItem>
        <FormItem label="升级购产品">
          <Input type="text"
                 v-model="modal.formItems.NewPID"
                 @on-blur="changeNewDescribe(modal.formItems.NewPID)"></Input>
          <Label v-text="describe.Newdescribe"> </Label>
        </FormItem>
        <FormItem label="提示语">
          <Input type="text"  :maxlength="16"
                 v-model="modal.formItems.Copywriting"></Input>
          <label>限制输入16个中文字符</label>
        </FormItem>
        <FormItem label="是否启用">
          <Checkbox v-model="modal.formItems.IsEnabled"></Checkbox>
        </FormItem>
      </Form>
    </Modal>
    <Modal :title="history.title"
           v-model="history.show"
           :closable="history.closable"
           :mask-closable="false"
           :loading="history.loading"
           width="1200">
      <i-table :columns="history.columns"
               :data="history.data"></i-table>
      <Page style="margin-top: 15px;"
            show-elevator
            show-total
            show-sizer
            :page-size-opts="[3, 5, 10, 20]"
            :total="history.historyPage.total"
            :current.sync="history.historyPage.index"
            :page-size="history.historyPage.size"
            @on-page-size-change="history.historyPage.size=arguments[0]"></Page>
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
                        title: "编号",
                        align: "center",
                        key: "PKID",
                        width: 60
                    },
                    {
                        title: "原始PID",
                        align: "center",
                        key: "OriginalPID"
                    },
                    {
                        title: "原始产品",
                        align: "center",
                        key: "OriginalDisplayName"
                    },
                    {
                        title: "升级购PID",
                        align: "center",
                        key: "NewPID"
                    },
                    {
                        title: "升级购产品",
                        align: "center",
                        key: "NewDisplayName"
                    },
                    {
                        title: "提示语",
                        align: "center",
                        key: "Copywriting"
                    },
                    {
                        title: "是否启用",
                        align: "center",
                        width: 85,
                        key: "IsEnabled",
                        render: (h, p) => {
                            var IsEnabled = p.row.IsEnabled ? "启用" : "隐藏";
                            return h("label", {
                                domProps: {
                                    innerHTML: IsEnabled
                                }
                            });
                        }
                    },
                    {
                        title: "操作",
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
                                            this.delete(p.row.PKID);
                                        }
                                    }
                                }),
                                h("a", {
                                    style: {
                                        "margin-right": "9px"
                                    },
                                    domProps: {
                                        href: "javascript:void(0)",
                                        innerHTML: "查看历史记录"
                                    },
                                    on: {
                                        click: () => {
                                            this.historyInfo(p.row.OriginalPID);
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
                    PKID: "",
                    OriginalPID: "",
                    NewPID: "",
                    Copywriting: "",
                    IsEnabled: false,
                    OriginalDescribe: "",
                    Newdescribe: ""
                }
            },
            history: {
                columns: [
                    {
                        title: "编号",
                        align: "center",
                        width: 70,
                        key: "PKID"
                    },
                    {
                        title: "原始属性",
                        width: 300,
                        align: "center",
                        render: (h, p) => {
                            var OldValue = JSON.parse(p.row.OldValue);
                            var labelhtml = "";
                            for (var x in OldValue) {
                                labelhtml += "<lable>" + x + " :  </lable>";
                                labelhtml += "<lable>" + OldValue[x] + "</lable>";
                                labelhtml += "<br></br>";
                            }
                            return h('label', {
                                domProps: {
                                    innerHTML: labelhtml
                                }
                            });
                        }
                    },
                    {
                        title: "新属性",
                        width: 300,
                        align: "center",
                        render: (h, p) => {
                            var NewValue = JSON.parse(p.row.NewValue);
                            var labelhtml = "";
                            for (var x in NewValue) {
                                labelhtml += "<lable>" + x + " :  </lable>";
                                labelhtml += "<lable>" + NewValue[x] + "</lable>";
                                labelhtml += "<br></br>";
                            }
                            return h('label', {
                                domProps: {
                                    innerHTML: labelhtml
                                }
                            });
                        }
                    },
                    {
                        title: "操作人",
                        align: "center",
                        key: "OperateUser"
                    },
                    {
                        title: "操作类型",
                        align: "center",
                        key: "Remarks"
                    },
                    {
                        title: "创建时间",
                        align: "center",
                        render: (h, p) => {
                            var labelhtml = "" + this.formatDate(p.row.CreateTime);
                            return h('label', {
                                domProps: {
                                    innerHTML: labelhtml
                                }
                            });
                        }
                    }
                ],
                title: "历史记录",
                show: false,
                loading: true,
                closable: true,
                data: [],
                historyPage: { total: 0, index: 1, size: 20, originalPID: '' }
            },
            page: { total: 0, index: 1, size: 20 },
            parameters: { pid: '' },
            describe: { Newdescribe: '', OriginalDescribe: '' }
        };
    },
    watch: {
        "page.index" () {
            this.loadData();
        },
        "history.historyPage.index" () {
            this.historyInfo(this.history.historyPage.originalPID);
        },
        "history.historyPage.size" () {
            this.historyInfo(this.history.historyPage.originalPID);
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
        formatDate (now) {
            if (now === "" || now === null) return "";
            var date = new Date(parseInt(now.substr(6)));
            var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
            var currentDate = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
            return date.getFullYear() + "-" + month + "-" + currentDate;
        },
        historyInfo (originalPID) {
            this.history.data = [];
            this.history.show = true;
            this.history.loading = true;
            var params = {};
            params.pageIndex = this.history.historyPage.index;
            params.pageSize = this.history.historyPage.size;
            params.originalPID = originalPID;
            this.ajax.post("/BaoYang/GetBtaaertLevelUpSettingLog", {
                ...params
            }).then(response => {
                var res = response.data;
                if (res.Status) {
                    this.history.data = res.Data.Item2;
                    this.history.historyPage.total = res.Data.Item1;
                    this.history.historyPage.originalPID = params.originalPID;
                    this.history.loading = false;
                } else {
                    this.$Message.error("操作失败!" + (res.msg || ""));
                }
            });
        },
        RemoveCache () {
            this.$Modal.confirm({
                title: "温馨提示",
                content: "确定清除缓存？",
                loading: true,
                onOk: () => {
                    this.ajax.get("/BaoYang/RemoveBtaaeryLevelUpCache").then(response => {
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
            params.OriginalPID = this.parameters.pid;
            this.ajax.post("/BaoYang/GetBatteryLeveUpList", {
                ...params
            })
                .then(response => {
                    console.info(response);
                    if (response.data.Status) {
                        var res = response.data;
                        this.page.total = res.Data.Item1 || 0;
                        this.table.data = res.Data.Item2 || [];
                        this.table.loading = false;
                    } else {
                        this.$Message.error("操作失败!" + (response.data.msg || ""));
                    }
                });
        },
        lazyLoadData () {
            this.table.loading = true;
            setTimeout(() => {
                this.search();
            }, 1500);
        },
        changeDescribe (value) {
            if (value === '') return;

            this.ajax.get("/Product/GetProductNameByPid", {
                params: {
                    pid: value
                }
            })
                .then(response => {
                    var res = response.data;
                    if (res.Status) {
                        this.describe.OriginalDescribe = res.Data;
                        console.info(this.describe.OriginalDescribe);
                    } else {
                        this.describe.OriginalDescribe = "";
                        this.$Message.error("操作失败! 获取产品名称失败 " + (res.msg || ""));
                    }
                });
        },
        changeNewDescribe (value) {
            if (value === '') return

            this.ajax.get("/Product/GetProductNameByPid", {
                params: {
                    pid: value
                }
            })
                .then(response => {
                    var res = response.data;
                    if (res.Status) {
                        this.describe.Newdescribe = res.Data;
                        console.info(this.describe.Newdescribe);
                    } else {
                        this.describe.Newdescribe = "";
                        this.$Message.error("操作失败! 获取产品名称失败 " + (res.msg || ""));
                    }
                });
        },
        add () {
            this.modal.formItems.pkid = 0;
            this.modal.formItems.OriginalPID = "";
            this.modal.formItems.NewPID = "";
            this.modal.formItems.Copywriting = "";
            this.modal.formItems.IsEnabled = true;
            this.describe.OriginalDescribe = "";
            this.describe.Newdescribe = "";
            this.modal.title = "添加区域";
            this.modal.show = true;
        },
        update (item) {
            this.modal.formItems.pkid = item.PKID;
            this.modal.formItems.OriginalPID = item.OriginalPID;
            this.modal.formItems.NewPID = item.NewPID;
            this.modal.formItems.Copywriting = item.Copywriting;
            this.modal.formItems.IsEnabled = item.IsEnabled;
            this.describe.OriginalDescribe = item.OriginalDisplayName;
            this.describe.Newdescribe = item.NewDisplayName;
            this.modal.title = "修改区域";
            this.modal.show = true;
        },
        submit () {
            var item = {};
            item.PKID = this.modal.formItems.pkid || 0;
            item.OriginalPID = this.modal.formItems.OriginalPID;
            item.NewPID = this.modal.formItems.NewPID;
            item.Copywriting = this.modal.formItems.Copywriting;
            item.IsEnabled = this.modal.formItems.IsEnabled;
            item.Newdescribe = this.describe.Newdescribe;
            item.OriginalDescribe = this.describe.OriginalDescribe;
            if (item.OriginalPID === "") {
                this.$Message.warning("原始品牌ID不能为空");
                this.modal.loading = false;
                this.$nextTick(() => {
                    this.modal.loading = true;
                });
                return;
            }
            if (item.NewPID === "") {
                this.$Message.warning("升级购品牌ID不能为空");
                this.modal.loading = false;
                this.$nextTick(() => {
                    this.modal.loading = true;
                });
                return;
            }
            if (item.Copywriting === "") {
                this.$Message.warning("提示语不能为空");
                this.modal.loading = false;
                this.$nextTick(() => {
                    this.modal.loading = true;
                });
                return;
            }
            if (item.Copywriting.length > 16) {
                this.$Message.warning("提示语不能超过16个中文字符");
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
                        .post("/BaoYang/InsertOrUpdateBatteryLevelUp", {
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
        delete (id) {
            this.$Modal.confirm({
                title: "温馨提示",
                content: "确认删除升级购配置吗?",
                loading: true,
                onOk: () => {
                    this.ajax.get("/BaoYang/DeleteBatteryLevelUpByPkid", {
                        params: {
                            pkid: id
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
.ivu-table td.demo-table-info-column {
  background-color: #2db7f5;
  color: #fff;
}
.ivu-table td.demo-table-info-column {
  background-color: #2db7f5;
  color: rgb(12, 2, 2);
}
.ivu-table td.demo-table-info-column2 {
  background-color: #7c2bd8;
  color: rgb(12, 2, 2);
}
</style>
