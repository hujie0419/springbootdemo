<template>
  <div>
    <div>
          <Button type="primary"
                  @click="uploadExcel">导入Excel</Button>
           <Button type="primary"
                  @click="updateRecord">更新记录</Button>
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
    <Modal :title="model.title"
           v-model="model.show"
           :closable="model.closable"
           :mask-closable="false"
           :loading="model.loading"
           cancelText="取消"
           width="785">
        <Table style="margin-top: 15px;"
            border
            stripe
            :columns="logModal.columns"
            :data="logModal.data"
            :loading="logModal.loading"
            no-data-text="暂无数据"></Table>
        <Page style="margin-top: 15px;"
            show-total
            show-sizer
            :page-size-opts="[10, 20, 40, 100, 200]"
            :total="recordpage.total"
            :current.sync="recordpage.index"
            :page-size="recordpage.size"
            @on-page-size-change="recordpage.size=arguments[0]"></Page>
    </Modal>
    <Modal :title="uploadModal.title"
           ok-text="上传"
           v-model="uploadModal.show"
           :closable="uploadModal.closable"
           :mask-closable="false"
           :loading="uploadModal.loading"
           @on-cancel="uploadModal.file = null"
           @on-ok="submitUpload">
      <Upload :before-upload="handleUpload"
              type="drag"
              :show-upload-list="false"
              :on-success="handleSuccess"
              :on-error="handleError"
              action="/TireActivity/ImportMaintenancePackage"
              ref="upload">
        <div style="padding: 20px 0">
          <Icon type="ios-cloud-upload"
                size="52"
                style="color: #3399ff"></Icon>
          <p>选择文件或者将文件拖拽到此处</p>
        </div>
      </Upload>
      <div v-if="uploadModal.file !== null"
           style="text-align:center">文件: {{ uploadModal.file.name }}</div>
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
                        title: "序号",
                        align: "center",
                        type: "index",
                        key: "PKID"
                    },
                    {
                        title: '小保养套餐PID',
                        align: 'center',
                        key: 'PID'
                    },
                    {
                        title: '原价',
                        align: 'center',
                        key: 'Price'
                    },
                    {
                        title: '一条轮胎优惠价',
                        align: 'center',
                        key: 'OnetirePrice'
                    },
                    {
                        title: '二条轮胎优惠价',
                        align: 'center',
                        key: 'TwotirePrice'
                    },
                    {
                        title: '三条轮胎优惠价',
                        align: 'center',
                        key: 'ThreetirePrice'
                    },
                    {
                        title: '四条轮胎优惠价',
                        align: 'center',
                        key: 'FourtirePrice'
                    }
                ],
                data: [],
                loading: false
            },
            dataAll: [],
            logModal: {
                columns: [
                    {
                        title: '更新时间',
                        key: 'CreateDateTime',
                        align: "center",
                        fixed: "center"
                    },
                    {
                        title: '操作人',
                        key: 'Creator',
                        align: 'center',
                        fixed: "center"
                    },
                    {
                        title: '操作',
                        align: 'center',
                        fixed: "center",
                        width: 230,
                        render: (h, p) => {
                            let buttons = [];
                            buttons.push(
                                h(
                                "Button",
                                {
                                    props: {
                                        type: "success",
                                        size: "small"
                                    },
                                    style: {
                                        marginRight: "3px",
                                        marginLeft: "10px"
                                    },
                                    on: {
                                    click: () => {
                                        this.exportExcel(p.row.ObjectId);
                                     }
                                    }
                                },
                                "下载"
                                )
                            );
                            return h("div", buttons);
                        }
                    }
                ],
                data: [],
                loading: false
            },
            model: {
                title: '更新记录',
                show: false,
                loading: false,
                closable: true,
                updateInfo: {
                    updateDateTime: "",
                    createBy: ""
                }
            },
            page: { total: 0, index: 1, size: 20 },
            recordpage: { total: 0, index: 1, size: 20 },
            uploadModal: {
                title: '导入数据',
                show: false,
                loading: true,
                closable: true,
                file: null
            }
        }
    },
    watch: {
        "page.index" () {
            this.loadData();
        },
        "page.size" () {
            this.search();
        },
        "recordpage.index" () {
            this.loadLogData();
        },
        "recordpage.size" () {
            this.searchLog();
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
        searchLog () {
            if (this.recordpage.index === 1) {
                this.loadLogData();
            } else {
                this.recordpage.index = 1;
            }
        },
        loadData () {
            this.table.data = [];
            this.table.loading = true;
            this.ajax.get(`/TireActivity/GetMaintenancePackageOnSaleList?pageSize=${this.page.size}&pageIndex=${this.page.index}`).then(response => {
                var res = response.data.data;
                this.page.total = response.data.dataCount;
                this.table.data = res || [];
                this.table.loading = false;
            })
        },
        lazyLoadData () {
            this.table.loading = true;
            setTimeout(() => {
                this.search();
            }, 1500);
        },
        updateRecord () {
            this.recordpage.index = 1;
            this.recordpage.size = 10;
            this.loadLogData();
            this.model.show = true;
        },
        loadLogData () {
            var objectType = "MaintenancePackageOnSale";
            var objectId = "";
            this.logModal.data = [];        
            this.logModal.loading = false;
            this.ajax.get(`/CommonConfigLog/GetCommonConfigLogs?objectType=${objectType}&objectId=${objectId}`)
                .then(response => {
                this.dataAll = response.data || [];
                this.recordpage.total = this.dataAll.length;
                if (this.recordpage.total > 0) {
                    this.getPageData();
                }
            });
        },
        getPageData () {
            var newPageInfo = [];
            for (var i = 0; i < this.recordpage.size; i++) {
                var index = i + (this.recordpage.index - 1) * this.recordpage.size;
                if (index > this.recordpage.total - 1) break;
                    newPageInfo[newPageInfo.length] = this.dataAll[index];
            }
            this.logModal.data = newPageInfo; 
        },
        uploadExcel () {
            this.uploadModal.show = true;
            this.uploadModal.file = null;
        },
        handleUpload (file) {
            this.uploadModal.file = file;
            return false;
        },
        submitUpload () {
            this.$Modal.confirm({
                title: "温馨提示",
                content: "确认上传该文件?",
                loading: true,
                onOk: () => {
                    if (this.uploadModal.file === null) {
                        this.$Message.info("请先选择文件上传");
                        this.uploadModal.loading = false;
                        this.$Modal.remove();
                        this.uploadModal.show = true;
                    } else {
                        this.$Modal.remove();
                        this.uploadModal.show = true;
                        this.uploadModal.loading = true;
                        this.$refs.upload.post(this.uploadModal.file);
                    }
                },
                 onCancel: () => {
                    this.uploadModal.loading = false;
                    this.uploadModal.show = true;
                }
            });
        },
        handleSuccess (response) {
            this.uploadModal.loading = true;
            if (response.code === 1) {
                if (response.status) {
                this.$Message.info((response.msg || "上传成功!"))
                this.uploadModal.file = null;
                this.uploadModal.show = false;
                 this.uploadModal.loading = false;
                this.lazyLoadData();
                } else {
                    this.$Message.warning((response.msg || ""));
                    this.uploadModal.loading = false;
                }
            } else {
                if (response.invalidCount > 0) {
                    setTimeout(() => {
                        this.$Modal.confirm({
                        title: "温馨提示",
                        content: "文件中存在" + response.invalidCount + "个无法识别PID,是否确定上传?",
                        loading: false,
                        onOk: () => {
                        this.priceConfirm(response.invalidPriceCount, response.data);
                        },
                        onCancel: () => {
                            this.uploadModal.loading = false;
                            this.uploadModal.show = true;
                        }
                        });
                    }, 1500);
                } else {
                    this.priceConfirm(response.invalidPriceCount, response.data);
                }
            }
            this.uploadModal.loading = false;
        },
        priceConfirm (invalidPriceCount, packageData) {
            if (invalidPriceCount > 0) {
                 setTimeout(() => {
                    this.$Modal.confirm({
                    title: "温馨提示",
                    content: "文件中存在" + invalidPriceCount + "条数据价格不是数字类型或为负数,是否确定上传?",
                    loading: false,
                    onOk: () => {
                        this.ajax
                        .post(
                        `/TireActivity/AddMaintenancePackageOnSaleList`, {list: packageData}
                        )
                        .then(response => {
                            if (response.data.status) {
                                this.$Message.info((response.data.msg || "上传成功!"))
                                this.uploadModal.file = null;
                                this.uploadModal.show = false;
                                this.uploadModal.loading = false;
                                this.lazyLoadData(); 
                            } else {
                                this.$Message.warning((response.data.msg || ""));
                            }
                        });
                    },
                    onCancel: () => {
                        this.uploadModal.loading = false;
                        this.uploadModal.show = true;
                    }
                });
                }, 1500);
            } else {
                 this.ajax
                .post(
                `/TireActivity/AddMaintenancePackageOnSaleList`, {list: packageData}
                )
                .then(response => {
                    if (response.data.status) {
                        this.$Message.info((response.data.msg || "上传成功!"))
                        this.uploadModal.file = null;
                        this.uploadModal.show = false;
                        this.uploadModal.loading = false;
                        this.lazyLoadData(); 
                    } else {
                        this.$Message.warning((response.data.msg || ""));
                    }
                });
            }
        },
        handleError () {
            this.uploadModal.loading = false;
        },
        exportExcel (objectID) {
            this.$Modal.confirm({
                title: "温馨提示",
                content: "是否导出数据?",
                loading: false,
                onOk: () => {
                    window.location.href = "/TireActivity/ExportEachExcel?updateID=" + objectID;
                }
            });
        },
        modelCancel () {
            this.model.show = false;
        },
         FormatToTime (timestamp) {
            var time = new Date(parseInt(timestamp.replace("/Date(", "").replace(")/", ""), 10));
            var year = time.getFullYear();
            var month = time.getMonth() + 1 < 10 ? "0" + (time.getMonth() + 1) : time.getMonth() + 1;
            var date = time.getDate() < 10 ? "0" + time.getDate() : time.getDate();
            var hour = time.getHours() < 10 ? "0" + time.getHours() : time.getHours();
            var minute = time.getMinutes() < 10 ? "0" + time.getMinutes() : time.getMinutes();
            var second = time.getSeconds() < 10 ? "0" + time.getSeconds() : time.getSeconds();
            var YmdHis = year + '/' + month + '/' + date + ' ' + hour + ':' + minute + ':' + second;
            return YmdHis;
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
