<template>
  <div>
    <div>运营 > VIP卡>
      <router-link to="/vipcard/index">VIP卡售卖场次</router-link> >新增售卖场次</div>
    <Input v-model="activityName" placeholder="请填写场次名称" style="width: 300px"></Input>
    <selectclient @on-change="clientId=$event"></selectclient>
    <Button type="primary" @click="search" icon="ios-search">搜索</Button>
    <Button type="success" @click="save">保存</Button>
    <div style="margin-top:18px" ref="divSelect">
      <Table border ref="selection" :columns="columns" :data="data" @on-selection-change="selectChange"></Table>
    </div>
    <br><br/>

  </div>
</template>
<script>
import selectclient from '@/views/vipcard/selectclient.vue'
export default {
  data () {
    return {
      columns: [
        {
          type: 'selection',
          width: 60,
          align: 'center'
        },
        {
          title: "批次ID",
          key: "BatchId"
        },
        {
          title: "VIP卡名称",
          key: "CardName"
        },
        {
          title: "VIP卡面额",
          key: "CardValue"
        },
        {
          title: "销售单价",
          key: "SalePrice"
        },
        {
          title: "库存数量",
          key: "Stock"
        },
        {
          title: "使用范围",
          key: "UseRange"
        },

        {
          title: "开始时间",
          key: "StartDate",
          render: (h, params) => {
            return h("span", this.formatDate(params.row.StartDate));
          }
        },
        {
          title: "结束时间",
          key: "EndDate",
          render: (h, params) => {
            return h("span", this.formatDate(params.row.EndDate));
          }
        }
      ],
      data: [],
      checkDatas: [],
      clientId: -1,
      cleintName: '',
      activityName: ''
    };
  },
  components: {
    selectclient
  },
  methods: {
    search () {
      this.loadData();
    },
    loadData () {
      if (this.clientId === '') {
        this.$Message.error('请先选中客户');
      } else {
        let params = {
          clientId: this.clientId
        };
        var _this = this;
        this.ajax
          .get("/VipCard/GetBatchesByClientId", {
            params: params
          })
          .then(response => {
            _this.data = response.data;
          });
      }
    },
    save () {
      var _this = this;
      var datas = [];
      if (this.checkDatas.length === 0) {
        this.$Message.success('至少选中一条');
      } else {
        if (this.activityName === '') {
          this.$Message.success('活动名称要在前端展示，请编辑下');
        } else {
          this.checkDatas.forEach(function (e, i) {
            var BatchId = e.BatchId;
            var CardName = e.CardName;
            var CardValue = e.CardValue;
            var SalePrice = e.SalePrice;
            var Stock = e.Stock;
            var UseRange = e.UseRange;
            var StartDate = _this.formatDate(e.StartDate);
            var EndDate = _this.formatDate(e.EndDate);
            var onepro = {
              BatchId: BatchId,
              CardName: CardName,
              CardValue: CardValue,
              SalePrice: SalePrice,
              Stock: Stock,
              UseRange: UseRange,
              StartDate: StartDate,
              EndDate: EndDate
            };
            datas.push(onepro);
          });
          this.ajax.post('/vipcard/Save', {
            data: JSON.stringify(datas),
            activityName: this.activityName,
            clientId: this.clientId
          })
            .then(response => {
              if (response.data === true) {
                this.$Message.success('保存成功了');
              } else {
                this.$Message.error('保存失败了');
              }
            })
        } 
}
    },

    selectChange (sections) {
      var _this = this;
      _this.checkDatas = sections;
    },
    formatDate (value) {
      if (value == null) return null;
      var time = new Date(
        parseInt(value.replace("/Date(", "").replace(")/", ""))
      );
      var year = time.getFullYear();
      var day = time.getDate();
      var month = time.getMonth() + 1;
      var hours = time.getHours();
      var minutes = time.getMinutes();
      var seconds = time.getSeconds();
      return (
        year +
        "-" +
        month +
        "-" +
        day +
        " " +
        hours +
        ":" +
        minutes +
        ":" +
        seconds
      );
    }
  }
}

</script>
