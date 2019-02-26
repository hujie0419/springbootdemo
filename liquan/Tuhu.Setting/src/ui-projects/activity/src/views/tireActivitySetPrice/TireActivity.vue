<template>
  <div>
    <div>
        <Button type="primary"
            @click="uploadExcel">新建计划</Button>
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
    <Modal :title="uploadModal.title"
           ok-text="上传"
           v-model="uploadModal.show"
           :closable="uploadModal.closable"
           :mask-closable="false"
           :loading="uploadModal.loading"
           @on-cancel="cancelload"
           @on-ok="submitUpload('uploadModal.tireActivityInfo')">
      <Upload :before-upload="handleUpload"
              type="drag"
              :show-upload-list="false"
              :on-success="handleSuccess"
              :on-error="handleError"
              action="123"
              ref="upload"
              >
        <div style="padding: 20px 0">
          <Icon type="ios-cloud-upload"
                size="52"
                style="color: #3399ff"></Icon>
          <p>选择文件或者将文件拖拽到此处</p>
        </div>
      </Upload>
      <Form label-position="right"
            :label-width="100"
            :model="uploadModal.tireActivityInfo"
            ref="uploadModal.tireActivityInfo"
            :rules="ruleValidate">
            <FormItem label="活动开始时间">
                 <Row>
                    <Col span="13">
                        <DatePicker editable=false v-model.trim="uploadModal.tireActivityInfo.beginMonth"  transfer type="date" style="width: 200px"></DatePicker>
                    </Col>
                    <Col span="10">
                        <TimePicker editable=false  confirm type="time" v-model.trim="uploadModal.tireActivityInfo.beginTime" transfer style="width: 168px"></TimePicker>
                    </Col>
                </Row>
            </FormItem>
            <FormItem label="活动结束时间">
                <Row>
                    <Col span="13">
                        <DatePicker editable=false v-model.trim="uploadModal.tireActivityInfo.endMonth"  transfer type="date" style="width: 200px"></DatePicker>
                    </Col>
                    <Col span="10">
                        <TimePicker editable=false confirm type="time" v-model.trim="uploadModal.tireActivityInfo.endTime" transfer style="width: 168px"></TimePicker>
                     </Col>
                </Row>
            </FormItem>
            <FormItem label="计划名称" prop="planName">
                <Input v-model.trim="uploadModal.tireActivityInfo.planName" maxlength=15 size="large" placeholder="计划名称" />
            </FormItem>
            <FormItem label="计划说明" prop="planDesc">
                <Input v-model.trim="uploadModal.tireActivityInfo.planDesc" maxlength=30 type="textarea" :rows="6" placeholder="计划名称" />
            </FormItem>
      </Form>
      <div v-if="uploadModal.file !== null"
           style="text-align:center">文件: {{ uploadModal.file.name }}</div>
    </Modal>
    <Modal v-model="logModal.visible" title="操作日志" cancelText="取消" width="785" >
        <Table :loading="logModal.loading"  :data="logModal.data" :columns="logModal.columns" stripe no-data-text="暂无数据"></Table>
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
                        title: '计划编号',
                        align: 'center',
                        key: 'PlanNumber',
                        width: 130
                    },
                    {
                        title: '计划名称',
                        key: 'PlanName',
                        align: 'center',
                        width: 130
                    },
                    {
                        title: '计划说明',
                        key: 'PlanDesc',
                        align: 'center',
                        width: 200
                    },
                    {
                        title: 'PID个数',
                        key: 'PIDNum',
                        align: 'center',
                        width: 100
                        
                    },
                    {
                        title: '创建时间',
                        key: 'CreateDatetime',
                        align: 'center',
                        width: 130,
                        render: (h, params) => {
                        return h('div', [
                                h('span', this.FormatToDate(params.row.CreateDatetime))
                                        ]);
                        }
                    },
                    {
                        title: '状态',
                        key: 'Status',
                        align: 'center',
                        width: 100,
                        render: function (h, params) {
                        switch (params.row.Status) {
                            case 0:
                            return h("span", 
                             {
                                style: {
                                    color: "#c5c8ce"
                                }
                             },
                            "未开始");
                            case 1:
                            return h("span", 
                            {
                                style: {
                                    color: "#19be6b"
                                }
                             },
                            "运行中");
                            case 2:
                            return h("span",
                            {
                                style: {
                                    color: "red"
                                }
                             },
                            "己过期");
                            default:
                            return h("span",
                             {
                                style: {
                                    color: "red"
                                }
                             },
                            "暂停");
                            }
                        }
                    },
                    {
                        title: '开始时间',
                        key: 'BeginDatetime',
                        align: 'center',
                        width: 100,
                        render: (h, params) => {
                        return h('div', [
                                h('span', this.FormatToDate(params.row.BeginDatetime))
                                        ]);
                        }
                    },
                    {
                        title: '结束时间',
                        key: 'EndDatetime',
                        align: 'center',
                        width: 100,
                        render: (h, params) => {
                        return h('div', [
                                h('span', this.FormatToDate(params.row.EndDatetime))
                                        ]);
                        }
                    },
                    {
                        title: '创建人',
                        key: 'CreateBy',
                        align: 'center',
                        width: 100
                    },
                    {
                        title: '操作',
                        align: 'left',
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
                                        this.exportExcel(p.row.PKID);
                                     }
                                    }
                                },
                                "下载"
                                ),
                                  h('Button', {
                                    props: {
                                        type: 'primary',
                                        size: 'small'
                                    },
                                    style: {
                                        marginRight: '3px'
                                    },
                                    on: {
                                        click: () => {
                                         this.searchLog('TireActivity', p.row.PKID); 
                                        }
                                    }
                                }, '日志')
                            );
                            if (p.row.Status === 0 || p.row.Status === 1) {
                                    buttons.push(
                                    h(
                                    "Button",
                                    {
                                        props: {
                                            type: "error",
                                            size: "small"
                                        },
                                        style: {
                                            marginRight: "3px"
                                        },
                                        on: {
                                        click: () => {
                                            this.stopPlan(p.row.PKID);
                                        }
                                        }
                                    },
                                   "停止"
                                    )
                                );
                            } else {
                                 buttons.push(
                                     h("span", 
                                        {
                                            style: {
                                                color: "#c5c8ce",
                                                marginLeft: "18px"
                                            }
                                        },
                                        "停止")
                                );
                            }
                            return h("div", buttons);
                        }
                    }
                ],
                data: [],
                loading: false
            },
            page: { total: 0, index: 1, size: 20 },
            uploadModal: {
                title: '导入数据',
                show: false,
                loading: true,
                closable: true,
                file: null,
                tireActivityInfo: {
                    planName: '1',
                    planDesc: '',
                    beginMonth: '',
                    beginTime: '00:00:00',
                    endMonth: '',
                    endTime: '00:00:00',
                    beginDateTime: '',
                    endDateTime: ''
                }
            },
            excelDate: {
                repestIndex: 0,
                repest: [],
                tireActivityModel: {},
                notValidCount: 0,
                pidList: []
            },
            ruleValidate: {
                    planName: [
                        { required: true, message: '计划名称不能为空', trigger: 'blur' },
                        { type: "string", max: 15, message: "不能超过15字", trigger: "blur" }
                    ],
                    planDesc: [
                        { required: true, message: '计划说明不能为空', trigger: 'blur' },
                        { type: "string", max: 30, message: "不能超过30字", trigger: "blur" }
                    ]
            },
            logModal: {
                loading: true,
                visible: false,
                data: [],
                columns: [
                    {
                        title: "操作人",
                        width: 150,
                        key: "Creator",
                        align: "left",
                        fixed: "left"
                    },
                    {
                        title: "时间",
                        width: 150,
                        key: "CreateDateTime",
                        align: "left",
                        fixed: "left"
                    },
                    {
                        title: "操作",
                        width: 150,
                        key: "Remark",
                        align: "left",
                        fixed: "left"
                    },
                    {
                        title: "改后数据",
                        width: 300,
                        key: "AfterValue",
                        align: "left",
                        fixed: "left"
                    }
                ]
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
            this.ajax.get(`/TireActivity/GetTireActivityList?pageSize=${this.page.size}&pageIndex=${this.page.index}`).then(response => {
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
        uploadExcel () {
            this.uploadModal.show = true;
            this.cancelUpload();
        },
        handleUpload (file) {
            this.uploadModal.file = file;
            return false;
        },
        cancelload () {
            this.uploadModal.show = false;
            this.cancelUpload();
        },
        submitUpload (name) {
            if (this.uploadModal.tireActivityInfo.beginMonth === ' ' || this.uploadModal.tireActivityInfo.beginMonth === '') {
                this.$Message.info("时间不能为空!");
                this.uploadModal.loading = false;
                return false;
            };
            if (this.uploadModal.tireActivityInfo.beginTime === ' ' || this.uploadModal.tireActivityInfo.beginTime === '') {
                this.$Message.info("时间不能为空!");
                this.uploadModal.loading = false;
                return false;
            }
            if (this.uploadModal.tireActivityInfo.endMonth === ' ' || this.uploadModal.tireActivityInfo.endMonth === '') {
                 this.$Message.info("时间不能为空!");
                this.uploadModal.loading = false;
                return false;
            }
            if (this.uploadModal.tireActivityInfo.endTime === ' ' || this.uploadModal.tireActivityInfo.endTime === '') {
                 this.$Message.info("时间不能为空!");
                this.uploadModal.loading = false;
                return false;
            }
            if (this.uploadModal.tireActivityInfo.planName === ' ' || this.uploadModal.tireActivityInfo.planName === '') {
                this.$Message.info("计划名称不能为空!");
                this.uploadModal.loading = false;
                return false;
            }
            if (this.uploadModal.tireActivityInfo.planDesc === ' ' || this.uploadModal.tireActivityInfo.planDesc === '') {
                this.$Message.info("计划说明不能为空!");
                this.uploadModal.loading = false;
                return false;
            }
            this.uploadModal.tireActivityInfo.beginDateTime = this.FormatToTime(this.uploadModal.tireActivityInfo.beginMonth).split(' ')[0] + ' ' + this.uploadModal.tireActivityInfo.beginTime;
            this.uploadModal.tireActivityInfo.endDateTime = this.FormatToTime(this.uploadModal.tireActivityInfo.endMonth).split(' ')[0] + ' ' + this.uploadModal.tireActivityInfo.endTime;
            if (this.uploadModal.tireActivityInfo.beginDateTime > this.uploadModal.tireActivityInfo.endDateTime) {
                this.$Message.info("活动开始时间不能大于活动结束时间!");
                this.uploadModal.loading = false;
                return false;
            }
            var fd = new FormData();
            fd.append('file', this.uploadModal.file);
            fd.append('planName', this.uploadModal.tireActivityInfo.planName);
            fd.append('beginDateTime', this.uploadModal.tireActivityInfo.beginDateTime);
            fd.append('endDateTime', this.uploadModal.tireActivityInfo.endDateTime);
            fd.append('planDesc', this.uploadModal.tireActivityInfo.planDesc);
            this.ajax
            .post(
            `/TireActivity/ImportTireActivityPid`, fd
            )
            .then(response => {
            if (response.data.code === 1) {
                this.$Message.warning((response.data.msg || ""));
                this.uploadModal.loading = false;
                this.uploadModal.show = true;
            } else {
                this.excelDate.repest = response.data.repeatList;
                this.excelDate.repestIndex = this.excelDate.repest.length - 1;
                this.excelDate.tireActivityModel = response.data.activityModel;
                this.excelDate.notValidCount = response.data.notvalidCount;
                this.excelDate.pidList = response.data.list;
                this.$Modal.confirm({
                title: "温馨提示",
                content: "文件上传成功，共包含" + response.data.validCount + "个轮胎产品，" + response.data.notvalidCount + "个未知，是否确认新建计划？",
                loading: false,
                onOk: () => {
                    if (response.data.repeatList.length === 0) {
                        this.addTireActivityPid();
                    } else {
                        setTimeout(() => {
                        this.confim(response.data.repeatList.length - 1, response.data.repeatList, response.data.activityModel, response.data.notvalidCount, response.data.list);
                        }, 1500);
                    }
                },
                onCancel: () => {
                    this.uploadModal.loading = false;
                    this.uploadModal.show = true;
                }
                });
            }
                });
        },
         confim (count, repest, model, notvalidCount, list) {
               model.BeginDatetime = this.FormatToTime(this.uploadModal.tireActivityInfo.beginMonth).split(' ')[0] + ' ' + this.uploadModal.tireActivityInfo.beginTime;
               model.EndDatetime = this.FormatToTime(this.uploadModal.tireActivityInfo.endMonth).split(' ')[0] + ' ' + this.uploadModal.tireActivityInfo.endTime;
                        this.$Modal.confirm({
                        title: "温馨提示",
                        content: "该计划中有" + repest[count].repeatPidCount + "个PID与" + repest[count].PlanNumber + "重复，是否确定新建？",
                        onOk: () => {
                            if (count === 0) {
                                 this.addTireActivityPid();
                            } else {
                                setTimeout(() => {
                                this.confim(count = count - 1, repest, model, notvalidCount, list);
                            }, 1500);
                            }
                        },
                        onCancel: () => {
                             this.uploadModal.show = true;
                            this.uploadModal.loading = false;
                        }
                             });
                    },
        addTireActivityPid () {
            this.excelDate.tireActivityModel.BeginDatetime = this.uploadModal.tireActivityInfo.beginDateTime;
            this.excelDate.tireActivityModel.EndDatetime = this.uploadModal.tireActivityInfo.endDateTime;
            this.ajax
                    .post(
                    `/TireActivity/AddTireActivityPid`, {
                        model: this.excelDate.tireActivityModel,
                        notvalidCount: this.excelDate.notValidCount,
                        list: this.excelDate.pidList
                            }
                        )
                    .then(response => {
                        if (response.data.status) {
                            this.$Message.info((response.data.msg || ""))
                            this.uploadModal.file = null;
                            this.uploadModal.show = false;
                            this.uploadModal.loading = false;
                            this.lazyLoadData(); 
                        } else {
                            this.$Message.warning((response.data.msg || ""));
                            this.uploadModal.loading = false;
                        }
                    });
        },
        cancelUpload () {
            this.uploadModal.file = null;
            this.uploadModal.tireActivityInfo.planName = " ";
            this.uploadModal.tireActivityInfo.planDesc = " ";
            this.uploadModal.tireActivityInfo.beginMonth = " ";
            this.uploadModal.tireActivityInfo.beginTime = "00:00:00";
            this.uploadModal.tireActivityInfo.endMonth = " ";
            this.uploadModal.tireActivityInfo.endTime = "00:00:00";
            this.uploadModal.tireActivityInfo.beginDateTime = " ";
            this.uploadModal.tireActivityInfo.endDateTime = " ";
        },
        handleError () {
            this.uploadModal.loading = false;
        },
        handleSuccess (response) {
        },
        exportExcel (pkid) {
            this.$Modal.confirm({
                title: "温馨提示",
                content: "是否导出数据?",
                loading: false,
                onOk: () => {
                    window.location.href = "/TireActivity/ExportExcelTireActivity?tireActivityID=" + pkid;
                }
            });
        },
        stopPlan (pkid) {
            this.$Modal.confirm({
                title: "温馨提示",
                content: "确定要暂停该计划？该操作不可恢复",
                loading: true,
                onOk: () => {
                    this.ajax.post(`/TireActivity/UpdateTireActivityStatus?pkid=${pkid}`).then(response => {
                    var code = response.data.code;
                    if (code === 0) {
                        this.$Message.warning(response.data.msg);
                        this.$Modal.remove();
                        this.lazyLoadData();
                    } else {
                        this.$Modal.remove();
                        this.lazyLoadData();
                    }
                })
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
        FormatToTime (timestamp) {
            var time = new Date(timestamp);
            var year = time.getFullYear();
            var month = time.getMonth() + 1 < 10 ? "0" + (time.getMonth() + 1) : time.getMonth() + 1;
            var date = time.getDate() < 10 ? "0" + time.getDate() : time.getDate();
            var hour = time.getHours() < 10 ? "0" + time.getHours() : time.getHours();
            var minute = time.getMinutes() < 10 ? "0" + time.getMinutes() : time.getMinutes();
            var second = time.getSeconds() < 10 ? "0" + time.getSeconds() : time.getSeconds();
            var YmdHis = year + '/' + month + '/' + date + ' ' + hour + ':' + minute + ':' + second;
            return YmdHis;
        },
        searchLog (objectType, objectId) {
            this.logModal.data = [];        
            this.logModal.visible = true;      
            this.logModal.loading = false;
            this.ajax.get(`/CommonConfigLog/GetCommonConfigLogs?objectType=${objectType}&objectId=${objectId}`)
                .then(response => {
                this.logModal.data = response.data;
            });
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
<style type="text/css" scoped>
.pastPlan {
    background-color: #DDDDDD;
}
</style>
