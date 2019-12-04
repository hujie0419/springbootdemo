<template>
  <div>
    <h1>{{msg}}</h1>
    <div>
      <input type="checkbox" v-model="isOnlyUser" title="参加活动人员" name="参加活动人员"/>
      <i-select
        v-model="provinceID"
        style="width:100px"
        @on-change="onchangeRegion(1,$event)"
        ref="selectProvince"
      >
        <i-option v-for="item in provinceList" :key="item.PKID" :value="item.PKID">{{ item.Name }}</i-option>
      </i-select>
      <i-select
        v-model="cityID"
        style="width:100px"
        @on-change="onchangeRegion(2,$event)"
        ref="selectCity"
      >
        <i-option v-for="item in cityList" :key="item.PKID" :value="item.PKID">{{ item.Name }}</i-option>
      </i-select>
      <i-select v-model="areaID" style="width:100px" ref="selectArea">
        <i-option v-for="item in areaList" :key="item.PKID" :value="item.PKID">{{ item.Name }}</i-option>
      </i-select>
      <i-button type="primary" @click="SearchUser">查询</i-button>
    </div>
    <div style="padding-top:5px">
      <Table  :columns="userHeaders" :data="userList"></Table>
    </div>
  </div>
</template>

<script>
export default {
  name: "User",
  data() {
    return {
      msg: "用户信息",
      regionList: [], //区域信息
      provinceList: [], //省
      cityList: [], //市
      areaList: [], //区
      provinceID: "",
      cityID: "",
      areaID: "",
      userHeaders: [
        {
          title: "用户名",
          key: "UserName"
        },
        {
          title: "真实姓名",
          key: "RealName"
        },
        {
          title: "手机号",
          key: "Phone"
        },
        {
          title: "省市区",
          key: "Area"
        },
        {
          title: "详细地址",
          key: "Address"
        }
      ],
      userList: [],
      isOnlyUser: false
    };
  },
  created() {
    this.GetRegionList();
  },
  methods: {
    GetRegionList() {
      var that = this;
      this.util
        .ajax({
          async: false,
          method: "POST",
          url: `/api/Promotion/Activity/GetRegionList?IsALL=false`,
          data: {},
          headers: {
            RequestID: "799385b17308425a9d02e2237fc57501",
            RemoteName: "Tuhu.Service.Promotion.Server",
            RemoteEndpoint: "TH201969521"
          }
        })
        .then(function(response) {
          if (response.data && response.data.Success) {
            that.regionList = response.data.Result;
            that.provinceList = that.regionList.filter(function(item) {
              return item.Layer === 1;
            });
          }
        });
    },
    onchangeRegion(layer, parentID) {
      if (layer == 1) {
        this.$refs.selectCity.setQuery(null);
        this.$refs.selectArea.setQuery(null);
        this.cityList = [];
        this.areaList = [];
        this.cityList = this.regionList.filter(function(item) {
          return item.Layer === 2 && item.ParentID === parentID;
        });
      } else if (layer == 2) {
        this.$refs.selectArea.setQuery(null);
        this.areaList = this.regionList.filter(function(item) {
          return item.Layer === 3 && item.ParentID === parentID;
        });
      }
    },
    SearchUser() {
      var that = this;
      let paraData = {
        ProvinceID: that.provinceID,
        CityID: that.cityID,
        AreaID: that.areaID,
        PageSize: 10,
        CurrentPage: 1,
        IsOnlyUser: that.isOnlyUser
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
          if (response.data && response.data.Success) {
            that.userList = response.data.Result.Source;
          }
        });
    }
  }
};
</script>
