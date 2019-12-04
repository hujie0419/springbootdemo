<template>
  <div>
    <h1>{{msg}}</h1>
    <div>
      <Table border :content="self" :columns="activityHeaders" :data="activityTbList"></Table>
    </div>
  </div>
</template>

<script>
export default {
  name: "ActivityDetailModify",
  data() {
    return {
      msg: "活动列表",
      self: this,
      activityHeaders: [
        { title: "活动名称", key: "Name" },
        { title: "描述", key: "Description" },
        { title: "开始时间", key: "StartTimeStr" },
        { title: "结束时间", key: "EndTimeStr" },
        { title: "状态", key: "StatusName" },
        {
          title: "操作",
          key: "action",
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
                      this.show(params.index);
                    }
                  }
                },
                "报名"
              )
            ]);
          }
        }
      ],
      activityTbList: []
    };
  },
  created() {
    this.getAllActivityList();
  },
  methods: {
    getAllActivityList() {
      var that = this;
      this.util
        .ajax({
          method: "POST",
          url: `/api/Promotion/Activity/GetAllActivityList`,
          data: {},
          headers: {
            RequestID: "799385b17308425a9d02e2237fc57501",
            RemoteName: "Tuhu.Service.Promotion.Server",
            RemoteEndpoint: "TH201969521"
          }
        })
        .then(function(response) {
          console.log(response);
          if (response.data && response.data.Success) {
            that.activityTbList = response.data.Result;
          }
        });
    },
    show(index) {
      console.log(`名称：${this.activityTbList[index].Name}`);
      var that = this;
      let paraData={
        UserId:1,
        ActivityId:that.activityTbList[index].ID,
        Remark:"test",
      }
      this.util
        .ajax({
          method: "POST",
          url: `/api/Promotion/Activity/CreateUserActivityApply`,
          data: paraData,
          headers: {
            RequestID: "799385b17308425a9d02e2237fc57501",
            RemoteName: "Tuhu.Service.Promotion.Server",
            RemoteEndpoint: "TH201969521"
          }
        })
        .then(function(response) {
          console.log(response);
          if (response.data && response.data.Success) {
          }
        });
    }
  }
};
</script>
