<template>
  <div>
    <h3> {{blockSystemDictionary[query.blockSystem]}}黑名单配置</h3>
    <div>
      <Form inline>
        <FormItem>
          <Select filterable v-model="query.blockType" style="width:140px" @on-change='loadData(1)'>
            <Option v-for="key in Object.keys(blockTypeDictionary)" :key="key" :value="key">{{blockTypeDictionary[key]}}</Option>
          </Select>
          <Button type="warning" @click="onAdd()">新增黑名单</Button>
          <Button type="info" @click="viewLog()">查看日志</Button>
          <Button type="error" @click="uploadExcel">批量导入</Button>
          <a :href="'/BlockListConfig/DownloadTemplate?blockSystem='+blockSystemDictionary[query.blockSystem]+''" style="padding: 6px 15px;">下载Excel模板</a>
        </FormItem>
        <FormItem style="float: right;margin-right: 0;">
          <Input v-model="query.blockValue" placeholder="IP地址/用户手机号/设备号/用户Id/支付账号" ref="blockValueRef" style="width: 280px"></Input>
          <Button type="success" icon="search" @click="search">搜索</Button>
        </FormItem>
      </Form>
    </div>
    <Table border stripe :loading="table.loading" :columns="table.cols" :data="table.list" style="font-family: 微软雅黑;line-height: 1.4em;">
    </Table>
    <div style="margin: 10px;overflow: hidden">
      <div style="float: right;">
          <Page :total="table.totalSize" :page-size="table.pageSize" :current="table.pageIndex" @on-change="loadData" show-total></Page>
      </div>
    </div>
    <Modal
        :loading="addItem.loading"
        v-model="addItem.showDialog"
        title="新增黑名单"
        @on-ok="addItem.add"
        @on-cancel="addItem.cancel">
        <Form :model="addItem.item" :label-width="82">
          <FormItem label="黑名单类型">
              <Select filterable v-model="addItem.item.blockType" style="width:140px" transfer>
                <Option v-for="key in Object.keys(blockTypeDictionary)" :key="key" :value="key">{{blockTypeDictionary[key]}}</Option>
              </Select>
          </FormItem>
          <FormItem label="黑名单值">
              <Input v-model="addItem.item.blockValue" placeholder="要拉入黑名单的值" required></Input>
          </FormItem>
          <FormItem label="开始时间（不设置则立即生效）">
              <DatePicker type="date" placeholder="Select date" v-model="addItem.item.blockBeginTime.date" transfer></DatePicker>
                  <span> - </span>
                  <TimePicker type="time" placeholder="Select time" v-model="addItem.item.blockBeginTime.time" transfer></TimePicker>
          </FormItem>
          <FormItem label="结束时间（不设置则永久生效）">
                  <DatePicker type="date" placeholder="Select date" v-model="addItem.item.blockEndTime.date" transfer></DatePicker>
                  <span> - </span>
                  <TimePicker type="time" placeholder="Select time" v-model="addItem.item.blockEndTime.time" transfer></TimePicker>
          </FormItem>
          <FormItem label="拉黑原因">
              <Input v-model="addItem.item.reason" placeholder="拉黑原因"></Input>
          </FormItem>
          <FormItem label="备注">
              <Input v-model="addItem.item.remark" placeholder="备注"></Input>
          </FormItem>
      </Form>
    </Modal>
    <Modal :title="uploadModal.title" ok-text="上传" v-model="uploadModal.show" :closable="uploadModal.closable" :mask-closable="false" :loading="uploadModal.loading" @on-cancel="uploadModal.file = null" @on-ok="submitUpload">
      <Upload :before-upload="handleUpload" type="drag" :show-upload-list="false" :on-success="handleSuccess" :on-error="handleError" action="" accept="application/vnd.ms-excel, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" ref="uploadRef">
        <div style="padding: 20px 0">
          <Icon type="ios-cloud-upload" size="52" style="color: #3399ff"></Icon>
          <p>选择文件或者将文件拖拽到此处</p>
        </div>
      </Upload>
      <div v-if="uploadModal.file !== null" style="text-align:center">
        文件: {{ uploadModal.file.name }}
      </div>
    </Modal>
  </div>
</template>
<script>
export default {
  data () {
    return {
      blockSystemDictionary: {
      },
      blockTypeDictionary: {
        "1": "IP地址",
        "2": "用户手机号",
        "4": "设备号",
        "8": "用户Id",
        "16": "支付账号"
      },
      query: {
        blockSystem: "",
        blockValue: "",
        blockType: "1"
      },
      table: {
        pageSize: 20,
        pageIndex: 1,
        totalSize: 1,
        list: [],
        cols: [
          {
            title: "黑名单类型",
            align: "center",
            key: "BlockType",
            render: (h, params) => {
              return h("span", this.blockTypeDictionary[params.row.BlockType]);
            }
          },
          {
            title: "黑名单值",
            align: "center",
            key: "BlockValue"
          },
          {
            title: "黑名单开始时间",
            align: "center",
            key: "BlockBeginTime"
          },
          {
            title: "黑名单结束时间",
            align: "center",
            key: "BlockEndTime"
          },
          {
            title: "拉黑原因",
            align: "center",
            key: "Reason"
          },
          {
            title: "备注",
            align: "center",
            key: "Remark"
          },
          {
            title: "更新人",
            align: "center",
            key: "UpdateBy"
          },
          {
            title: "操作",
            align: "center",
            width: 100,
            key: "action",
            render: (h, params) => {
              return h("Button", {
                on: {
                  click: () => {
                    this.$Modal.confirm({
                      title: "警告",
                      content: '确定要删除吗？',
                      onOk: () => {
                          var parameter = new URLSearchParams();
                          parameter.append('BlockSystem', this.query.blockSystem);
                          parameter.append('BlockType', this.query.blockType);
                          parameter.append('BlockValue', params.row.BlockValue);
                          parameter.append('BlockBeginTime', params.row.BlockBeginTime);
                          parameter.append('PKID', params.row.PKID);

                          this.util.ajax.post("/BlockListConfig/DeleteById", parameter, {
                            headers: {
                              'Content-Type': 'application/x-www-form-urlencoded'
                            }
                          })
                          .then((response) => {
                            if (response.data.Status > 0) {
                              this.$Message.success('删除成功');
                              this.loadData(1);
                            } else {
                              this.$Message.error({
                                  top: 50,
                                  duration: 3,
                                  content: response.data.ErrorMsg
                                });
                            }
                          });
                      }
                    });
                  }
                },
                props: {
                  type: "error"
                }
              }, "删除")
            }
          }
        ],
        loading: true
      },
      addItem: {
        showDialog: false,
        loading: true,
        item: {
          blockType: '1',
          blockValue: '',
          blockBeginTime: {
            date: '',
            time: ''
          },
          blockEndTime: {
            date: '',
            time: ''
          },
          reason: '',
          remark: ''
        },
        add: () => {
          this.addItem.showDialog = true;
          this.addItem.loading = true;
          if (!this.addItem.item.blockValue) {
            this.util.message.error({
                  top: 100,
                  duration: 3,
                  content: '被拉黑的值不能为空'
                });
            this.addItem.loading = false;
            this.$nextTick(() => {
                this.addItem.loading = true;
            });
            return;
          }
          var that = this;
          this.util.ajax.post('/BlockListConfig/Add', {
                BlockSystem: this.query.blockSystem,
                BlockValue: this.addItem.item.blockValue,
                BlockType: this.addItem.item.blockType,
                Reason: this.addItem.item.reason,
                Remark: this.addItem.item.remark,
                BlockBeginTime: this.FormatToTime(this.addItem.item.blockBeginTime.date) + ' ' + this.addItem.item.blockBeginTime.time,
                BlockEndTime: this.addItem.item.blockEndTime.date + ' ' + this.addItem.item.blockEndTime.time
              })
            .then(function (response) {
              if (response.data.Status > 0) {
                that.addItem.item.blockType = "1";
                that.addItem.item.blockValue = "";
                that.addItem.item.reason = "";
                that.addItem.item.remark = "";
                that.addItem.item.blockBeginTime.date = "";
                that.addItem.item.blockBeginTime.time = "";
                that.addItem.item.blockEndTime.date = "";
                that.addItem.item.blockEndTime.time = "";
                that.addItem.showDialog = false;
                that.addItem.loading = false;
                that.$nextTick(() => {
                  that.addItem.loading = true;
                });
                that.loadData(1);
              } else {
                that.util.message.error({
                  top: 100,
                  duration: 3,
                  content: response.data.ErrorMsg
                });
                that.addItem.loading = false;
                that.$nextTick(() => {
                  that.addItem.loading = true;
            });
              }
            });
        },
        cancel: () => {
          this.addItem.showDialog = false;
        }
      },
      uploadModal: {
          title: '导入数据',
          show: false,
          loading: true,
          closable: true,
          file: null
      }
    };
  }, 
  props: ['blockSystemModel'],  
  created: function () {
    this.query.blockSystem = this.blockSystemModel.blockSystem;
    this.blockSystemDictionary = {};
    this.blockSystemDictionary[this.blockSystemModel.blockSystem] = this.blockSystemModel.blockSystemName;
    this.loadData(1);
  },
  methods: {
    loadData (pIndex) {
      if (!pIndex) {
        pIndex = 1;
      }
      this.table.loading = true;
      var that = this;
      this.util.ajax
        .get(`/BlockListConfig/List?blockSystem=${that.query.blockSystem}&blockType=${that.query.blockType}&pageIndex=${pIndex}&pageSize=20`)
        .then(function (response) {
          that.table.loading = false;
          if (response.data && response.data.Status > 0) {
            that.table.list = response.data.Data || [];
            that.table.totalSize = response.data.TotalSize;
            that.table.pageIndex = pIndex;
          }
        });
    },
    FormatToTime (timestamp) {
            var time = new Date(timestamp);
            var year = time.getFullYear();
            var month = time.getMonth() + 1 < 10 ? "0" + (time.getMonth() + 1) : time.getMonth() + 1;
            var date = time.getDate() < 10 ? "0" + time.getDate() : time.getDate();
            var YmdHis = year + '-' + month + '-' + date;
            return YmdHis;
    },
    onAdd () {
      this.addItem.showDialog = true;
    },
    viewLog () {
      this.$Modal.info({
        title: '操作日志',
        width: 1024,
        render: h => {
          return h('iframe', {
            attrs: {
              src: `/CommonConfigLog/ListLoger?objectType=${this.query.blockSystem}BlockListLog`,
              width: 1000,
              height: 500,
              frameborder: 0
            }
          });
        }
      });
    },
    search () {
      if (this.query.blockValue.trim() !== '') {
        this.table.loading = true;
        var that = this;
        this.util.ajax
          .get(`/BlockListConfig/SearchBlockList?blockSystem=${that.query.blockSystem}&blockValue=${that.query.blockValue}`)
          .then(function (response) {
            that.table.loading = false;
            if (response.data && response.data.Status > 0) {
              that.table.list = response.data.Data == null ? [] : response.data.Data;
              that.table.totalSize = response.data.TotalSize;
              that.table.pageIndex = response.data.PageIndex;
            }
          });
      } else {
        this.$refs['blockValueRef'].focus();
      }
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
      if (this.uploadModal.file !== null) {
        this.$Modal.confirm({
          title: "温馨提示",
          content: "确认上传该文件?",
          loading: true,
          onOk: () => {
            this.$Modal.remove();
            this.uploadModal.show = true;
            this.uploadModal.loading = true;

            var fd = new FormData();
            fd.append('file', this.uploadModal.file);
            fd.append('blockSystem', this.query.blockSystem);

             this.ajax
            .post(
            `/BlockListConfig/ImportExcel`, fd
            )
            .then(response => {
              if (response.data.code === 1) {
                if (response.data.status) {
                  this.$Message.info((response.data.msg || "上传成功!"))
                  this.uploadModal.file = null;
                  this.uploadModal.show = false;
                  this.uploadModal.loading = false;
                  this.loadData(1);
                } else {
                  this.$Message.warning((response.data.msg || ""));
                  this.uploadModal.loading = false;
                }
              }
              this.uploadModal.loading = false;
            })
          },
          onCancel: () => {
            this.uploadModal.loading = false;
            this.uploadModal.show = true;
          }
        });
      } else {
        this.$Message.info("请先选择文件上传");
        this.uploadModal.loading = false;
        this.$nextTick(() => {
            this.uploadModal.loading = true;
        });
      }
    },
    handleSuccess (response) {
      this.uploadModal.loading = true;
      if (response.code === 1) {
        if (response.status) {
          this.$Message.info((response.msg || "上传成功!"))
          this.uploadModal.file = null;
          this.uploadModal.show = false;
          this.uploadModal.loading = false;
          this.loadData(1);
        } else {
          this.$Message.warning((response.msg || ""));
          this.uploadModal.loading = false;
        }
      }
      this.uploadModal.loading = false;
    },
    handleError () {
      this.uploadModal.loading = false;
    }
  }
}
</script>
