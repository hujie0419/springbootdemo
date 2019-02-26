<template>
  <div v-cloak>
    <Select style="width:10%" v-model="searchData.packageId" filterable>
      <Option key="0" value="0">-请选择活动名称-</Option>
      <Option v-for="packageconfig in packages" 
        :value="packageconfig.ServicePid" 
        :key="packageconfig.ServicePid">
        {{packageconfig.ServiceName}}
      </Option>
    </Select>
    <Select style="width:10%" v-model="searchData.userType">
      <Option key="0" value="0">-请选择用户类型-</Option>
      <Option key="1" value="1">新用户</Option>
      <Option key="2" value="2">老用户</Option>
      <Option key="3" value="3">全部用户</Option>
    </Select>
    <Button type="primary" @click="Search(1)">查询</Button>
    <router-link to="/packageDetail">
      <Button type="error">新增</Button>
    </router-link>
    <Table :loading="loading" style="margin-top:10px;" 
      :data="searchResult" 
      :columns="tableColumns"
      stripe border>
    </Table>
    <Page style="margin:10px;" 
    :total="pager.totalCount" 
    :current.sync="pager.pageIndex" 
    :page-size="pager.pageSize"
    show-total>
    </Page>
  </div>
</template>

<script>
  import util from "@/framework/libs/util";
  import shopViewForTable from "@/views/paintDiscount/shopViewForTable";
  export default {
    name: "packageConfig",
    data () {
      return {
        loading: false,
        searchResult: [],
        packages: [],
        searchData: {
          packageId: "",
          userType: ""
        },
        pager: {
          pageIndex: 1,
          pageSize: 20,
          totalCount: 0
        },
        tableColumns: [{
            type: "index",
            width: 60,
            align: "center"
          },
          {
            title: "活动名称",
            key: "PackageName",
            width: 250,
            align: "center"
          },
          {
            title: "是否限制新用户",
            key: "UserType",
            width: 150,
            align: "center",
            render: (h, params) => {
              if (params.row.UserType === 1) {
                return h("span", "新用户");
              } else if (params.row.UserType === 2) {
                return h("span", "老用户");
              } else if (params.row.UserType === 3) {
                return h("span", "全部用户");
              }
            }
          },
          {
            title: "城市",
            key: "City",
            width: 150,
            align: "center",
            render: (h, params) => {
              return h('div', (params.row.RegionShops || []).map(x => {
                return h('div', x.ProvinceName + "-" + x.CityName);
              }));
            }
          },
          {
            title: "门店",
            key: "Shop",
            align: "center",
            render: (h, params) => {
              return h(shopViewForTable, {
                props: {
                  regionShops: params.row.RegionShops
                }
              });
            }
          },
          {
            title: "操作",
            key: "operate",
            align: "center",
            render: (h, params) => {
              return h("div", [
                h(
                  "Button", {
                    props: {
                      type: "primary"
                    },
                    style: {
                      marginRight: "5px"
                    },
                    on: {
                      click: () => {
                        this.Edit(params.row);
                      }
                    }
                  },
                  "详情"
                ),
                h(
                  "Button", {
                    props: {
                      type: "error"
                    },
                    on: {
                      click: () => {
                        this.Delete(params.row);
                      }
                    }
                  },
                  "删除"
                )
              ]);
            }
          }
        ],
        model: {}
      };
    },
    created: function () {
      this.$Message.config({
        top: 50,
        duration: 5
      });
      this.GetAllPackageConfig();
      this.Search(1);
    },
    methods: {
      GetAllPackageConfig: function () {
        var vm = this;
        util.ajax
          .post("/PaintDiscountConfig/GetAllPaintDiscountPackage")
          .then(function (result) {
            result = (result || []).data || [];
            if (result.Status) {
              vm.packages = result.Data;
            }
          });
      },
      Search: function (pageIndex) {
        var vm = this;
        vm.pager.PageIndex = pageIndex;
        vm.loading = true;
        util.ajax
          .post("/PaintDiscountConfig/SelectPackageConfigForView", {
            packageId: vm.searchData.packageId || -1,
            userType: vm.searchData.userType || 0,
            pageIndex: vm.pager.pageIndex,
            pageSize: vm.pager.pageSize
          })
          .then(function (result) {
            result = (result || []).data || [];
            if (result.Status) {
              vm.searchResult = result.Data;
              vm.pager.totalCount = result.TotalCount;
            }
            vm.loading = false;
          });
      },
      // 跳转详情页
      Edit: function (para) {
        this.$router.push('/packageDetail?packageId=' + para.PKID);
      },
      // 删除价格体系配置
      Delete: function (para) {
        var vm = this;
        if (!confirm("确认删除:" + para.PackageName + "?")) {
          return;
        }
        util.ajax.post("/PaintDiscountConfig/DeletePackageConfig", {
          model: para
        }).then(function (result) {
          result = (result || []).data || [];
          if (result.Status) {
            vm.$Message.success("删除成功");
            setTimeout(() => {
              vm.Search(1);
            }, 2000);
          } else {
            vm.$Message.error("删除失败!" + (result.Msg || ""));
          }
        });
      }
    }
  };

</script>

<style scoped>
[v-cloak] {
  display: none;
}
</style>
