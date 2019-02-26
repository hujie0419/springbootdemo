<template>
  <div>
    <div>
          <Button type="primary"
                  @click="generationModalShow">新增生成</Button>
           <Button type="primary"
                  @click="downloadModalShow">新增下载</Button>
           <h3 style="margin-left:30px;display:inline-block">已生成：{{totalRecordModel.generatedNum}}</h3>       
           <h3 style="margin-left:30px;display:inline-block">已下载：{{totalRecordModel.DownloadedNum}}</h3>       
           <h3 style="margin-left:30px;display:inline-block">可下载：{{totalRecordModel.DownloadableNum}}</h3>       
    </div>
    <Table style="margin-top: 15px;"
           border
           stripe
           :columns="table.columns"
           :data="table.data"
           :loading="table.loading"></Table>
    <Page style="margin-top: 15px;"
          show-elevator
          show-total
          show-sizer
          :page-size-opts="[10, 20, 40, 100, 200]"
          :total="page.total"
          :current.sync="page.index"
          :page-size="page.size"
          @on-page-size-change="page.size=arguments[0]"></Page>
    <Modal v-model="generationModal.visible" title="新增生成(生成完成后即可下载)" :transfer="false" width="40%">
        <div slot="footer">
            <Button type="text" size="large" @click="generationModalCancel('generationModal.moveCargeneration')">取消</Button>
            <Button type="primary" size="large" :loading="generationModal.loading" @click="generationModalOk('generationModal.moveCargeneration')">提交</Button>
        </div>
        <Form ref="generationModal.moveCargeneration" :model="generationModal.moveCargeneration" :rules="generationModal.rules" :label-width="80">
            <FormItem label="生成数量" prop="GeneratedNum">
                <Input v-model="generationModal.moveCargeneration.GeneratedNum"  placeholder="每次上限10000" />
            </FormItem>
        </Form>
    </Modal> 
    <Modal v-model="downloadModal.visible" title="新增下载" :transfer="false" width="40%">
        <div slot="footer">
            <Button type="text" size="large" @click="downloadModalCancel('downloadModal.moveCarDownlaod')">取消</Button>
            <Button type="primary" size="large" :loading="downloadModal.loading" @click="downloadModalOk('downloadModal.moveCarDownlaod')">提交</Button>
        </div>
        <Form ref="downloadModal.moveCarDownlaod" :model="downloadModal.moveCarDownlaod" :rules="downloadModal.rules" :label-width="80">
            <FormItem label="下载数量" prop="DownloadNum">
                <Input v-model="downloadModal.moveCarDownlaod.DownloadNum" :placeholder="DownloadableNumString" />
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
                        title: "PKID",
                        align: "center",
                        key: "PKID"
                    },
                    {
                        title: '时间',
                        align: 'center',
                        key: 'CreateDatetime',
                        render: (h, params) => {
                            return h('div', [
                                    h('span', this.FormatToDate(params.row.CreateDatetime))
                                            ]);
                            }
                    },
                    {
                        title: '生成数量',
                        align: 'center',
                        key: 'GeneratedNum'
                    },
                    {
                        title: '生成状态',
                        align: 'center',
                        key: 'GeneratingStatus',
                        render: function (h, params) {
                            switch (params.row.GeneratingStatus) {
                            case 0:
                            return h("span", "待生成");
                            case 1:
                            return h("span", "生成中");
                            default:
                            return h("span", "已生成");
                        }
                      }
                    }
                ],
                data: [],
                loading: false
            },
            page: { total: 0, index: 1, size: 20 },
            totalRecordModel: {
                generatedNum: 0,
                DownloadedNum: 0,
                DownloadableNum: 0
            },
            DownloadableNumString: "",
            generationModal: {
                visible: false,
                loading: false,
                moveCargeneration: {
                    GeneratedNum: ""
                },
                rules: {
                    GeneratedNum: [
                        { required: true, message: "生成数量不能为空", trigger: 'blur' },
                        { required: true,
                            validator: (rule, value, callback) => {
                                if (!/^[1-9][0-9]{0,3}$|10000$/.test(value)) {
                                    callback(new Error("请输入正整数,且不能大于10000"));                      
                                } else {
                                    callback();
                                }
                            },
                            trigger: 'change'
                        }
                    ]
                }
            },
            downloadModal: {
                visible: false,
                loading: false,
                moveCarDownlaod: {
                    DownloadNum: ""
                },
                rules: {
                    DownloadNum: [
                        { required: true, message: "下载数量不能为空", trigger: 'blur' },
                        { required: true,
                            validator: (rule, value, callback) => {
                                if (this.totalRecordModel.DownloadableNum === 0) {
                                    callback(new Error("当前可下载量为0，无法下载")); 
                                } else {
                                    if (!/^[1-9][0-9]*$/.test(value)) {
                                        callback(new Error("请输入正整数"));                      
                                    } else if (value > this.totalRecordModel.DownloadableNum) {
                                        callback(new Error("下载量不能大于" + this.totalRecordModel.DownloadableNum));   
                                    } else {
                                        callback();
                                    }
                                }
                            },
                            trigger: 'change'
                        }
                    ]
                }
            }
        }
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
        loadData () {
            this.table.data = [];
            this.table.loading = true;
            this.ajax.get(`/MoveCarQRCode/GetMoveCarGenerationRecordsList?pageSize=${this.page.size}&pageIndex=${this.page.index}`).then(response => {
                var res = response.data.data;
                this.page.total = response.data.count;
                this.table.data = res || [];
                this.table.loading = false;
            });
            this.ajax.get(`/MoveCarQRCode/GetMoveCarTotalRecordsModel`).then(response => {
                var res = response.data.data;
                if (res) {
                     this.totalRecordModel.generatedNum = res.GeneratedNum;
                    this.totalRecordModel.DownloadedNum = res.DownloadedNum;
                    this.totalRecordModel.DownloadableNum = res.DownloadableNum;
                    this.DownloadableNumString = "当前可下载量为" + this.totalRecordModel.DownloadableNum;
                }
            });
        },
        FormatToDate (timestamp) {
            var time = new Date(parseInt(timestamp.replace("/Date(", "").replace(")/", ""), 10));
            var year = time.getFullYear();
            var month = time.getMonth() + 1 < 10 ? "0" + (time.getMonth() + 1) : time.getMonth() + 1;
            var date = time.getDate() < 10 ? "0" + time.getDate() : time.getDate();
            var hour = time.getHours() < 10 ? "0" + time.getHours() : time.getHours();
            var minute = time.getMinutes() < 10 ? "0" + time.getMinutes() : time.getMinutes();
            var second = time.getSeconds() < 10 ? "0" + time.getSeconds() : time.getSeconds();
            var YmdHis = year + '/' + month + '/' + date + ' ' + hour + ':' + minute + ':' + second;
            return YmdHis;
        },
        generationModalShow () {
            this.$refs['generationModal.moveCargeneration'].resetFields();
            this.generationModal.moveCargeneration.GeneratedNum = "";
            this.generationModal.visible = true;
            this.generationModal.loading = false; 
        },
        generationModalCancel (name) {
            this.$refs[name].resetFields();
            this.generationModal.visible = false;
        },
        generationModalOk (name) {
            this.generationModal.loading = true;
            this.$refs[name].validate((valid) => {
                if (valid) {
                    if (parseInt(this.generationModal.moveCargeneration.GeneratedNum) > 10000) {
                        this.$Message.warning("生成数量不能大于10000");
                        this.generationModal.loading = false; 
                        return false;
                    }
                    if (parseInt(this.generationModal.moveCargeneration.GeneratedNum) < 1) {
                        this.$Message.warning("生成数量必须大于1");
                        this.generationModal.loading = false; 
                        return false;
                    }
                    this.ajax.post(`/MoveCarQRCode/GenerationMoveCarQRCode`, {generationNum: parseInt(this.generationModal.moveCargeneration.GeneratedNum)})
                    .then(response => {
                        if (response.data.status) {
                            this.$Message.success((response.data.msg || "操作成功!"))
                            this.lazyLoadData();
                            this.generationModal.loading = false;          
                            this.generationModal.visible = false;
                        } else {
                            this.$Message.warning((response.data.msg || "操作失败"));
                            this.generationModal.loading = false; 
                        }
                    });
                } else {
                    this.generationModal.loading = false;          
                } 
            });
        },
        downloadModalShow () {
            this.$refs['downloadModal.moveCarDownlaod'].resetFields();
            this.downloadModal.moveCarDownlaod.DownloadNum = "";
            this.downloadModal.visible = true;
            this.generationModal.loading = false; 
        },
        downloadModalCancel (name) {
            this.$refs[name].resetFields();
            this.downloadModal.visible = false;
        },
        downloadModalOk (name) {
            this.downloadModal.loading = true;
            this.$refs[name].validate((valid) => {
                if (valid) {
                    if (this.totalRecordModel.DownloadableNum > 0) {
                        if (parseInt(this.downloadModal.moveCarDownlaod.DownloadNum) > this.totalRecordModel.DownloadableNum) {
                            this.$Message.warning("下载数量不能大于" + this.totalRecordModel.DownloadableNum);
                            this.downloadModal.loading = false;   
                            return false;
                        }
                    } else {
                        this.$Message.warning("当前可下载量为0，无法下载！");
                        this.downloadModal.loading = false;   
                        return false;
                    }
                    window.location.href = "/MoveCarQRCode/DownloadMoveCarQRCode?downloadNum=" + parseInt(this.downloadModal.moveCarDownlaod.DownloadNum);
                    setTimeout(() => {
                        this.lazyLoadData();
                    }, 1500);
                    this.downloadModal.visible = false;   
                    this.downloadModal.loading = false;   
                } else {
                    this.downloadModal.loading = false;          
                } 
            });
        },
        lazyLoadData () {
            this.table.loading = true;
            setTimeout(() => {
                this.search();
            }, 1500);
        }
    },
    mounted () {
        this.loadData();
        this.$Message.config({
            duration: 5
        });
    }
}
</script>
