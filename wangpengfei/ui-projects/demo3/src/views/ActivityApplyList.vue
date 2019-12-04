<template>
  <div>
    <h1>{{msg}}</h1>
    <div>
      申请状态
      <i-select v-model="chooseStatus" style="width:100px" @on-change="chooseChanage">
        <i-option :value="0">--全部--</i-option>
        <i-option :value="1">未通过</i-option>
        <i-option :value="2">已通过</i-option>
      </i-select>
      <i-button type="primary" @click="SearchUserActivity()">查询</i-button>
    </div>
    <div style="padding-top:5px">
      <Table :columns="userActivityHeaders" :data="userActivityData"></Table>
      <Page
        style="padding-top:5px"
        :total="dataCount"
        :page-size="pageSize"
        show-sizer
        class="paging"
        @on-change="changepage"
        @on-page-size-change="pagesize"
      ></Page>
    </div>
  </div>
</template>

<script>
export default {
  name: "UserActivity",
  data() {
    return {
      msg: "活动报名信息管理",
      userActivityHeaders: [
        {
          title: "用户名",
          key: "UserName"
        },
        {
          title: "真实姓名",
          key: "RealName"
        },
        {
          title: "活动名称",
          key: "ActivityName"
        },
        {
          title: "备注",
          key: "Remark"
        },
        {
          title: "申请时间",
          key: "ApplyTimeStr",
          width: 180
        },
        {
          title: "通过时间",
          key: "PassTimeStr",
          width: 180
        },
        {
          title: "申请状态",
          key: "StatusName"
        },
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
                    marginRight: "5px",
                    display: params.row.Status === 1 ? "inline-block" : "none"
                  },
                  on: {
                    click: () => {
                      this.passApply(params.index);
                    }
                  }
                },
                "通过申请"
              ),
              h(
                "Button",
                {
                  props: {
                    type: "primary",
                    size: "small"
                  },
                  style: {
                    marginRight: "5px",
                    display: params.row.Status === 2 ? "inline-block" : "none"
                  },
                  on: {
                    click: () => {
                      this.deleteApply(params.index);
                    }
                  }
                },
                "删除"
              )
            ]);
          }
        }
      ],
      userActivityData: [],
      chooseStatus: 0,
      userList: [],
      dataCount: 0,
      // 每页显示多少条
      pageSize: 10,
      // 当前页码
      page: 1
    };
  },
  created() {
    //this.getUserList();
    this.getUserActivityList();
  },
  methods: {
    getUserActivityList() {
      var that = this;
      let paraDate = {
        PageSize: that.pageSize,
        CurrentPage: that.page,
        UserId: that.chooseUserID,
        Status: that.chooseStatus
      };
      this.util
        .ajax({
          async: false,
          method: "POST",
          url: `/api/Promotion/Activity/GetUserActivityApplyList`,
          data: paraDate,
          headers: {
            RequestID: "799385b17308425a9d02e2237fc57501",
            RemoteName: "Tuhu.Service.Promotion.Server",
            RemoteEndpoint: "TH201969521"
          }
        })
        .then(function(response) {
          if (response.data && response.data.Success) {
            that.userActivityData = response.data.Result.Source;
            that.dataCount = response.data.Result.Pager.Total;
          }
        });
    },
    changepage(index) {
      // 当前页码
      this.page = index;
      this.getUserActivityList();
    },
    pagesize(index) {
      let _start = (this.page - 1) * index;
      let _end = this.page * index;
      // 当前展示条数
      this.pageSize = index;
      this.getUserActivityList();
    },
    getUserList() {
      var that = this;
      let paraData = {
        PageSize: 100,
        CurrentPage: 1,
        IsOnlyUser: true
      };
      this.util
        .ajax({
          method: "POST",
          contentType: "application/json",
          url: `/api/Promotion/Activity/GetUserList`,
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
            that.userList = response.data.Result.Source;
          }
        });
    },
    SearchUserActivity() {
      this.getUserActivityList();
    },
    applyActivity() {
      this.$router.push("/ActivityModify");
    },
    passApply(index) {
      var that = this;
      this.util
        .ajax({
          method: "POST",
          url: `/api/Promotion/Activity/BatchPassUserActivityApplyByPKIDs`,
          data: [that.userActivityData[index].PKID],
          headers: {
            RequestID: "799385b17308425a9d02e2237fc57501",
            RemoteName: "Tuhu.Service.Promotion.Server",
            RemoteEndpoint: "TH201969521"
          }
        })
        .then(function(response) {
          console.log(response)
          if (response.data && response.data.Success) {
            that.getUserActivityList();
            that.$Message.info("申请通过成功");
          } else {
            that.$Message.error(response.data.ErrorMessage);
          }
        });
    },
    chooseChanage(){
      this.getUserActivityList();
    },
    deleteApply(index) {
      var that = this;
      this.util
        .ajax({
          method: "POST",
          url: `/api/Promotion/Activity/DeleteUserActivityApplyByPKID?PKID=${that.userActivityData[index].PKID}`,
          data: { PKIDs: that.userActivityData[index].PKID },
          headers: {
            RequestID: "799385b17308425a9d02e2237fc57501",
            RemoteName: "Tuhu.Service.Promotion.Server",
            RemoteEndpoint: "TH201969521"
          }
        })
        .then(function(response) {
          console.log(response)
          if (response.data && response.data.Success) {
            that.getUserActivityList();
            that.$Message.info("删除成功");
          } else {
            that.$Message.error(response.data.ErrorMessage);
          }
        });
    },
  }
};
</script>
