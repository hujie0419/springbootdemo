<template>
  <div>
    <h3> {{blockSystemDictionary[query.blockSystem]}}黑名单配置</h3>
    <div>
      <Form inline>
        <FormItem>
            <Select filterable v-model="query.blockType" style="width:140px" @on-change='loadData(1)'>
              <Option v-for="key in Object.keys(blockTypeDictionary)" :key="key" :value="key">{{blockTypeDictionary[key]}}</Option>
            </Select>
        </FormItem>
        <Button type="warning" @click="onAdd()">新增黑名单</Button>
      </Form>
    </div>
    <Table border strip :loading="table.loading" :columns="table.cols" :data="table.list" style="font-family: 微软雅黑;line-height: 1.4em;">
    </Table>
    <div style="margin: 10px;overflow: hidden">
      <div style="float: right;">
          <Page :total="table.totalSize" :current="table.pageIndex" @on-change="loadData" show-total></Page>
      </div>
    </div>
    <Modal
        v-model="addItem.showDialog"
        title="新增黑名单"
        @on-ok="addItem.add"
        @on-cancel="addItem.cancel">
        <Form :model="addItem.item" :label-width="82">
          <FormItem label="黑名单类型">
              <Select filterable v-model="addItem.item.blockType" style="width:140px">
                <Option v-for="key in Object.keys(blockTypeDictionary)" :key="key" :value="key">{{blockTypeDictionary[key]}}</Option>
              </Select>
          </FormItem>
          <FormItem label="黑名单值">
              <Input v-model="addItem.item.blockValue" placeholder="要拉入黑名单的值" required></Input>
          </FormItem>
          <FormItem label="开始时间（不设置则立即生效）">
              <DatePicker type="date" placeholder="Select date" v-model="addItem.item.blockBeginTime.date"></DatePicker>
                  <span> - </span>
                  <TimePicker type="time" placeholder="Select time" v-model="addItem.item.blockBeginTime.time"></TimePicker>
          </FormItem>
          <FormItem label="结束时间（不设置则永久生效）">
                  <DatePicker type="date" placeholder="Select date" v-model="addItem.item.blockEndTime.date"></DatePicker>
                  <span> - </span>
                  <TimePicker type="time" placeholder="Select time" v-model="addItem.item.blockEndTime.time"></TimePicker>
          </FormItem>
          <FormItem label="拉黑原因">
              <Input v-model="addItem.item.reason" placeholder="拉黑原因"></Input>
          </FormItem>
          <FormItem label="备注">
              <Input v-model="addItem.item.remark" placeholder="备注"></Input>
          </FormItem>
      </Form>
    </Modal>
  </div>
</template>
<script>
export default {
  data () {
    return {
      blockSystemDictionary: {
        "Pintuan": "拼团"
      },
      blockTypeDictionary: {
        "1": "IP地址",
        "2": "用户手机号",
        "4": "设备号",
        "8": "用户Id",
        "16": "支付账号"
      },
      query: {
        blockSystem: "Pintuan",
        blockType: "1"
      },
      table: {
        pageIndex: 1,
        totalSize: 1,
        list: [],
        cols: [
          {
            title: "黑名单类型",
            key: "BlockType",
            render: (h, params) => {
              return h("span", this.blockTypeDictionary[params.row.BlockType]);
            }
          },
          {
            title: "黑名单值",
            key: "BlockValue"
          },
          {
            title: "黑名单开始时间",
            key: "BlockBeginTime"
          },
          {
            title: "黑名单结束时间",
            key: "BlockEndTime"
          },
          {
            title: "拉黑原因",
            key: "Reason"
          },
          {
            title: "备注",
            key: "Remark"
          },
          {
            title: "更新人",
            key: "UpdateBy"
          }
        ],
        loading: true
      },
      addItem: {
        showDialog: false,
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
          if (!this.addItem.item.blockValue) {
            this.util.message.error({
                  top: 100,
                  duration: 3,
                  content: '被拉黑的值不能为空'
                });
            return;
          }
          var that = this;
          //
          this.util.ajax.post('/BlockListConfig/Add', {
                BlockSystem: this.query.blockSystem,
                BlockValue: this.addItem.item.blockValue,
                BlockType: this.addItem.item.blockType,
                Reason: this.addItem.item.reason,
                Remark: this.addItem.item.remark,
                BlockBeginTime: this.addItem.item.blockBeginTime.date + ' ' + this.addItem.item.blockBeginTime.time,
                BlockEndTime: this.addItem.item.blockEndTime.date + ' ' + this.addItem.item.blockEndTime.time
              })
            .then(function (response) {
              if (response.data.Status > 0) {
                console.log('Success');
                that.addItem.item.blockType = "1";
                that.addItem.item.blockValue = "";
                that.addItem.item.reason = "";
                that.addItem.item.remark = "";
                that.addItem.item.blockBeginTime.date = "";
                that.addItem.item.blockBeginTime.time = "";
                that.addItem.item.blockEndTime.date = "";
                that.addItem.item.blockEndTime.time = "";
                that.loadData(1);
              } else {
                that.util.message.error({
                  top: 100,
                  duration: 3,
                  content: response.data.ErrorMsg
                });
              }
            });
        },
        cancel: () => {
          this.addItem.showDialog = false;
        }
      }
    };
  },
  created: function () {
    if (this.$route.params.blockSystem) {
      this.query.blockSystem = this.$route.params.blockSystem;
    }
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
            that.table.list = response.data.Data;
            that.table.totalSize = response.data.TotalSize;
            that.table.pageIndex = pIndex;
          }
        });
    },
    onAdd () {
      this.addItem.showDialog = true;
    }
  }
}
</script>
