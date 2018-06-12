<template>
<div id="divMain">
  <h2 class="title">拼团抽奖</h2>
    <Form role="form" inline>
      <FormItem label="拼团Id" :label-width="50">
        <Input placeholder="请输入ProductGroupId" v-model="groupId" />
      </FormItem>
    </Form>
    <Table border strip :loading="loading" :columns="columns" :data="filtedList" no-data-text="暂无拼团抽奖产品"></Table>
</div>
</template>
<style scoped>
div.ivu-form-item-content {
  display: inline !important;
}
</style>

<script>
import util from "@/framework/libs/util";
var data = {
  groupId: "",
  list: [],
  loading: true,
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
    util.ajax.get(`/GroupBuyingV2/LotteryList?groupId=${data.groupId}`)
    .then(function (response) {
      data.loading = false;
      if (response.data) {
        data.list = response.data;
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
  }
};
</script>
