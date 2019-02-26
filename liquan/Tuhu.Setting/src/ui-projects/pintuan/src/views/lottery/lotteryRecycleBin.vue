<template>
<div id="divMain">
    <Form role="form" inline>
      <FormItem label="拼团Id" :label-width="50">
        <Input placeholder="请输入 ProductGroupId" v-model="groupId" />
      </FormItem>
      <FormItem :label-width="2">
        <Button type="success" v-on:click="loadData()">搜索</button>
      </FormItem>
    </Form>
    <Table border :columns="columns" :data="filtedList" no-data-text="暂无数据"></Table>
    <div style="margin: 10px;overflow: hidden">
        <div style="float: right;">
            <Page :total="totalSize" :current="1" @on-change="loadData" show-total></Page>
        </div>
    </div>
</div>
</template>
<script>
import util from "@/framework/libs/util";
var data = {
  groupId: "",
  pageIndex: 1,
  pageSize: 10,
  loading: true,
  totalSize: 20,
  list: [],
  columns: [
    {
      title: "拼团Id",
      key: "ProductGroupId"
    },
    {
      title: "团类型",
      key: "GroupType",
      render: function (h, params) {
        switch (params.row.GroupType) {
          case 1:
          return h("span", "新人团");
          case 2:
          return h("span", "团长特价");
          default:
          return h("span", "普通团");
        }
      }
    },
    {
      title: "团类别",
      key: "GroupCategory",
      render: function (h, params) {
        if (params.row.GroupCategory === 0) {
          return h("span", "一般拼团");
        } else {
          return h("span", "抽奖拼团");
        }
      }
    },
    {
      title: "商品名称",
      key: "ProductName"
    },
    {
      title: "活动价（元）",
      key: "FinalPrice"
    },
    {
      title: "开始时间",
      key: "BeginTime"
    },
    {
      title: "结束时间",
      key: "EndTime"
    },
    {
      title: "抽奖规则",
      key: "GroupDescription",
      type: "html"
    },
    {
      title: "操作",
      key: "action",
      render: function (h, params) {
        return h(
          "router-link",
          {
            attrs: {
              to: "lottery/" + params.row.ProductGroupId
            }
          },
          "查看详情"
        );
      }
    }
  ]
};

export default {
  name: "Lottery",
  data () {
    return data;
  },
  beforeCreate: function () {
    util.ajax.get(`/GroupBuyingV2/LotteryRecycleBinList?groupId=${data.groupId}&pageIndex=${data.pageIndex}&pageSize=${data.pageSize}`)
    .then(function (response) {
      if (response.data) {
        if (response.data.Data) {
          data.pageIndex = response.data.PageIndex;
          data.totalSize = response.data.TotalSize;
          data.list = response.data.Data;
        }
        data.loading = false;
      }
    });
  },
  computed: {
    filtedList () {
      if (data.groupId) {
        return data.list.filter(function (item) {
          return item.ProductGroupId === data.groupId;
        });
      } else {
        return data.list;
      }
    }
  },
  methods: {
    loadData (pIndex) {
      if (!pIndex) {
        pIndex = 1;
      }
      data.loading = true;
      util.ajax
        .get(`/GroupBuyingV2/LotteryRecycleBinList?groupId=${data.groupId}&pageIndex=${pIndex}&pageSize=${data.pageSize}`)
        .then(function (response) {
          if (response.data) {
              if (response.data.Data) {
                data.pageIndex = response.data.PageIndex;
                data.totalSize = response.data.TotalSize;
                data.list = response.data.Data;
              }
              data.loading = false;
          }
        });
    }
  }
};
</script>
