<template>
  <div class="content-wrapper ivu-layout-content">

    <List :title="list.title" :icon="list.icon">
      <div slot="extra" @click="handleRefresh">
        <Icon type="refresh" size="16" style="margin-top: -2px;"></Icon> 刷新
      </div>
      <!-- extra -->

      <div class="toolbar">
        <Button type="primary" icon="plus" @click="handleCreate">添加</Button>
        <Button type="default" icon="load-c" style="margin-left:20px;" @click="handleRefreshCache">刷新缓存</Button>
      </div>
      <!-- .toolbar -->

      <VTablePage ref="list" :columns="columns" :border="list.border" :loading="list.loading" :data="list.data" :total="list.total" @on-page-change="handleGetList"></VTablePage>
      <!-- VTablePage -->

    </List>
    <!-- List -->

    <Modal
        v-model="logmodal.visible"
        title="日志"
        cancelText="取消"
        :width="logmodal.width"
        :transfer="false">
        <Table
          :loading="logmodal.loading"
          :columns="logmodal.columns" 
          :data="logmodal.data"
          stripe></Table>
    </Modal>
    <!-- Modal -->
  </div>
</template>
<script>
import List from "@/views/components/List";
import expandrow from "@/views/floor/expandrow";
import VTablePage from "@/views/components/v-table/v-table-page.vue";
export default {
  name: "Banner",
  components: {
    List,
    expandrow,
    VTablePage
  },
  data() {
    return {
      // 列表属性
      list: {
        loading: true
      },
      edit: {
        PKID: 0,
        DisplayName: "",
        Name: "",
        ImgUrl: "",
        Code: "",
        Sort: 1,
        PidCount: 0,
        IsEnabled: false
      },
      create: {
        PKID: 0,
        DisplayName: "",
        Name: "",
        ImgUrl: "",
        Code: "",
        Sort: 1,
        PidCount: 0,
        IsEnabled: false
      },
      // 表格列的配置描述(用户)
      columns: [
        {
          type: "expand",
          width: 50,
          render: (h, params) => {
            return h(expandrow, {
              props: {
                row: params.row
              }
            });
          }
        },
        {
          title: "ID",
          key: "PKID",
          width: 100
        },
        {
          title: "楼层",
          width: 200,
          key: "DisplayName"
        },
        {
          title: "商品数",
          width: 100,
          key: "PidCount"
        },
        {
          title: "广告",
          key: "ImgUrl",
          render: (h, params) => {
            return h("div", [
              h(
                "a",
                {
                  style: {
                    marginRight: "16px"
                  },
                  attrs: {
                    target: "_blank",
                    href: params.row.ImgUrl
                  }
                },
                [
                  h("img", {
                    attrs: {
                      src: params.row.ImgUrl
                    },
                    style: {
                      height: "40px",
                      marginTop: "10px"
                    }
                  }),
                  ""
                ]
              )
            ]);
          }
        },
        {
          title: "状态",
          key: "IsEnabled",
          render: (h, params) => {
            return h(
              "Tag",
              {
                props: {
                  color: params.row.IsEnabled === true ? "green" : "red"
                }
              },
              params.row.IsEnabled === true ? "启用" : "禁用"
            );
          }
        },
        {
          title: "排序",
          key: "Sort"
        },
        {
          title: "操作",
          key: "operation",
          align: "center",
          minWidth: 150,
          render: (h, params) =>
            h("div", [
              h(
                "a",
                {
                  style: {
                    marginRight: "16px"
                  },
                  on: {
                    click: () => this.handleEdit(params.row)
                  }
                },
                [
                  h("Icon", {
                    props: {
                      type: "edit",
                      size: 10
                    },
                    style: {
                      marginTop: "-2px",
                      marginRight: "4px"
                    }
                  }),
                  "编辑"
                ]
              ),
              h(
                "Poptip",
                {
                  props: {
                    confirm: true,
                    transfer: true,
                    title: "确定删除?",
                    "ok-text": "确认",
                    "cancel-text": "取消"
                  },
                  on: {
                    "on-ok": () => this.handleDelete(params.row)
                  }
                },
                [
                  h("a", [
                    h("Icon", {
                      props: {
                        type: "trash-b",
                        size: 12
                      },
                      style: {
                        marginTop: "-2px",
                        marginRight: "4px"
                      }
                    }),
                    "删除"
                  ])
                ]
              ),
              h(
                "a",
                {
                  style: {
                    marginLeft: "16px"
                  },
                  on: {
                    click: () => this.handleShowLog(params.row.PKID)
                  }
                },
                [
                  h("Icon", {
                  props: {
                    type: "ios-list-outline",
                    size: 10
                  },
                  style: {
                    marginTop: "-2px",
                    marginRight: "4px"
                  }
                }),
                "日志"
                ]
              )
            ])
        }
      ],
      logmodal:{
        loading: true,
        visible: false,
        width: 635,
        data: [],
        columns:[{
          title:'操作人',
          key:"Creator"
        },{
          title:'操作内容',
          key:"Remark"
        },{
          title:"操作时间",
            key: "CreateDateTime",
            render: (h, params) => {
              return h("span", this.util.formatDate(params.row.CreateDateTime));
            }
        }]
      }
    };
  },
  mounted() {
    // 获取列表数据
    this.handleGetList();
  },
  provide() {
    return {
      getList: this.handleGetList
    };
  },
  methods: {
    /**
     * 获取列表
     */
    handleGetList() {
      // 取消全选
      this.$refs["list"].selectAll(false);
      this.list.loading = true;
      // 获取分页信息
      let page = this.$refs["list"].getPage(1);

      this.util.ajax.get(`/CarProducts/FloorList`).then(res => {
        this.list.loading = false;
        if (res.data) {
          this.list = {
            data: res.data,
            loading: false,
            border: true,
            icon: "navicon",
            title: "楼层列表"
          };
        }
      });
    },

    /**
     * 列表刷新方法
     */
    handleRefresh() {
      this.handleGetList();
    },

    /**
     * 刷新缓存
     */
    handleRefreshCache() {
      this.ajax.post(`/CarProducts/RefreshFloor`).then(res => {
        if (res.data) {
          this.$Message.success("刷新缓存成功");
        } else {
          this.$Message.error("刷新缓存失败");
        }
      });
    },

    /**
     * 添加
     */
    handleCreate() {
      this.$router.push({
        path: "/floor/edit",
        query: {
          FloorId: 0
        }
      });
    },

    /**
     * 编辑
     * @param  {object} row 当前行数据
     */
    handleEdit(row) {
      this.$router.push({
        path: "/floor/edit",
        query: {
          FloorId: row.PKID
        }
      });
    },

    /**
     * 删除数据
     * @param  {object} row 当前行数据
     */
    handleDelete(row) {
      var id = row.PKID;
      if (id != "" || id != null) {
        this.list.loading = true;
        this.ajax
          .post(`/CarProducts/DeleteFloor?id=` + id)
          .then(res => {
            if (res.data) {
              this.$Message.success("删除成功");
              this.handleGetList();
            } else {
              this.$Message.error("删除失败");
            }
          })
          .catch(() => {
            this.list.loading = false;
          });
      }
    },

    /**
     * 显示日志
     */
    handleShowLog(pkid){
      this.logmodal.loading = true;
      this.util.ajax.post(`/CommonConfigLog/GetCommonConfigLogs`,{
        objectId: pkid,
        objectType: "CarProductsFloor"
      }).then(res => {
        if (res.data) {
          this.logmodal.data = res.data
        } else {
            this.logmodal.data = [];
        }
        this.logmodal.visible = true;
        this.logmodal.loading = false;
      });
    }
  }
};
</script>
<style lang="less">
.toolbar {
  padding: 12px 16px;
  border: 1px solid #dcdee2;
  border-bottom-style: none;
}
</style>