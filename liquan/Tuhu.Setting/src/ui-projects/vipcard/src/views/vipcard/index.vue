<template>
  <div>
    <h1 class="title">VIP卡售卖场次</h1>
    <div>
      <selectclient @on-change="changeSelect($event)"></selectclient>
      <Button type="primary" @click="search" icon="ios-search">搜索</Button>
    </div>
    <div align="right">
      <Button type="primary" shape="circle" @click="toCreate" icon="plus-round">售卖场次</Button>
    </div>
    <div style="margin-top:18px">
      <Table :height="600" :data="data" :columns="columns" stripe></Table>
      <div style="margin: 10px;overflow: hidden">
        <div style="float: right;">
          <Page :total="page.total" show-total :page-size="page.pageSize" :current="page.current" :page-size-opts="[5 ,10 ,20 ,50]" show-elevator show-sizer @on-change="handlePageChange" @on-page-size-change="handlePageSizeChange"></Page>
        </div>
      </div>
    </div>
    <Modal v-model="logmodal.visible" title="操作日志" :width="logmodal.width">
      <Table :loading="logmodal.loading" :data="logmodal.data" :columns="logmodal.columns" stripe></Table>
    </Modal>
  </div>
</template>
<script>
import selectclient from '@/views/vipcard/selectclient.vue'
export default {
  data () {
    return {
      columns: [
        {
          title: "场次ID",
          key: "ActivityId"
        },
        {
          title: "场次名",
          key: "ActivityName"
        },
        {
          title: "企业客户",
          key: "ClientName"
        },
        {
          title: "场内VIP卡",
          key: "VipCards"
        },
        {
          title: "自动生成的url地址",
          key: "Url",
          render: (h, params) => {
            return h('a', {
              domProps: {
                href: params.row.Url,
                target: "_blank"
              }
            }, params.row.Url);
          }
        },
        {
          title: "创建时间",
          key: "CreateDateTime",
          render: (h, params) => {
            return h("span", this.formatDate(params.row.CreateDateTime));
          }
        },
        {
          title: "更新时间",
          key: "LastUpdateDateTime",
          render: (h, params) => {
            return h("span", this.formatDate(params.row.LastUpdateDateTime));
          }
        },
        {
          title: "操作",
          key: "action",
          width: 150,
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
                      this.toedit(params.index);
                    }
                  }
                },
                "编辑"
              ),
              h(
                "Button",
                {
                  props: {
                    type: "primary",
                    size: "small"
                  },
                  on: {
                    click: () => {
                      this.getlog(params.row.ActivityId);
                    }
                  }
                },
                "查看日志"
              ),
              h(
                "Button",
                {
                  props: {
                    type: "error",
                    size: "small"
                  },
                  on: {
                    click: () => {
                      this.refreshcache(params.row.ActivityId);
                    }
                  }
                }, "刷新缓存"
              )
            ]);
          }
        }
      ],
      data: [],
      page: {
        total: 5,
        current: 1,
        pageSize: 5
      },
      logmodal: {
        loading: true,
        visible: false,
        width: 800,
        data: [],
        columns: [
          {
            title: "操作",
            width: 150,
            key: "Title",
            align: "center",
            fixed: "left"
          },
          {
            title: "操作人",
            width: 100,
            key: "Name",
            align: "center"
          },
          {
            title: "时间",
            width: 200,
            key: "CreateDateTime",
            align: "center",
            fixed: "left",
            render: (h, params) => {
              return h("span", this.formatDate(params.row.CreateDateTime));
            }
          },
          {
            title: "明细",
            width: 300,
            key: "Remark",
            align: "center",
            fixed: "left"
          }
        ]
      },
      clientId: 0
    };
  },
  created () {
    this.loadData(1);
  },
  computed: {},
  components: {
    selectclient
  },
  methods: {
    search () {
      this.loadData(1);
    },
    changeSelect (data) {
      this.clientId = data;
    },
    loadData (pageIndex) {
      this.page.current = pageIndex;
      let params = {
        clientId: this.clientId === '' ? 0 : this.clientId,
        pageIndex: this.page.current,
        pageSize: this.page.pageSize
      };
      this.ajax
        .get("/VipCard/GetVipCardList", {
          params: params
        })
        .then(response => {
          this.page.total = response.data.Pager.TotalItem;
          this.data = response.data.Source;
        });
    },
    handlePageChange (pageIndex) {
      this.loadData(pageIndex);
    },
    handlePageSizeChange (pageSize) {
      this.page.pageSize = pageSize;
      this.loadData(this.page.current);
    },
    toCreate () {
      this.$router.push({ 'name': 'vipcardCreate' });
    },
    toedit (a) {
      this.$router.push({ 'name': 'vipcardEdit', query: { activityId: this.data[a].ActivityId, clientId: this.data[a].ClientId, activityName: this.data[a].ActivityName } });
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
    },
    getlog (activityId) {
      this.logmodal.loading = true;
      this.ajax
        .post("/VipCard/GetLogList", {
          activityId: activityId
        })
        .then(response => {
          this.logmodal.data = response.data;
          this.logmodal.visible = true;
          this.logmodal.loading = false;
        });
    },
    refreshcache (activityId) {
      console.log(activityId);
      this.ajax.post("/VipCard/RefreshCache", {
        activityId: activityId
      })
        .then(response => {
          if (response.data === true) {
            this.$Message.success('刷新成功了');
          } else {
            this.$Message.error('刷新失败了');
          }
        })
    }
  }
};
</script>
