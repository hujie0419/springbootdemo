<template>
  <div>
    <h1>{{msg}}</h1>
    <div>
      <i-select v-model="chooseUserID" style="width:100px">
        <i-option v-for="item in userList" :key="item.PKID" :value="item.PKID">{{ item.UserName }}</i-option>
      </i-select>
      <i-button type="primary" @click="SearchUserActivity()">查询</i-button>
    </div>
    <div style="padding-top:5px">
      <Table :columns="userActivityHeaders" :data="userActivityData"></Table>
      <Page style="padding-top:5px"
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
      msg: "我的活动",
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
          key: "ApplyTimeStr"
        },
        {
          title: "通过时间",
          key: "PassTimeStr"
        },
        {
          title: "申请状态",
          key: "StatusName"
        }
      ],
      userActivityData: [],
      chooseUserID: "",
      userList: [],
      dataCount: 0,
      // 每页显示多少条
      pageSize: 10,
      // 当前页码
      page: 1
    };
  },
  created() {
    this.getUserList();
    //this.getUserActivityList();
  },
  methods: {
    getUserActivityList() {
      var that = this;
      let paraDate = {
        PageSize: 10,
        CurrentPage: that.page,
        UserId: that.chooseUserID
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
          console.log(response);
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
      var that = this;
      let paraDate = {
        PageSize: 10,
        CurrentPage: that.page,
        UserId: that.chooseUserID
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
          console.log(response);
          if (response.data && response.data.Success) {
            that.userActivityData = response.data.Result.Source;
            that.dataCount = response.data.Result.Pager.Total;
          }
        });
    },
    applyActivity() {
      this.$router.push("/ActivityModify");
    }
  }
};
</script>
