<template>
  <div class="content-wrapper ivu-layout-content">

    <VTablePage ref="list" :columns="columns" :border="list.border" :loading="list.loading" :data="list.data" :total="list.total" @on-page-change="handleGetList"></VTablePage>
    <!-- VTablePage -->

    <Edit ref="edit" :model="edit" @on-update="handleGetList"></Edit>
    <!-- Edit -->

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
import Edit from "./edit";
import List from "@/views/components/List";
import VTablePage from "@/views/components/v-table/v-table-page.vue";
export default {
  name: "Kill",
  components: {
    List,
    Edit,
    VTablePage
  },
  data() {
    return {
      // 列表属性
      list: {},
      edit: {
        PKID: 0,
        FKHomePageID: 0,
        ModuleName: "",
        ModuleType: 0,
        Sort: 1,
        IsEnabled: false
      },
      create: {
        PKID: 0,
        FKHomePageID: 0,
        ModuleName: "",
        ModuleType: 3,
        Sort: 1,
        IsEnabled: false
      },
      // 表格列的配置描述(用户)
      columns: [
        {
          title: "ID",
          key: "PKID"
        },
        {
          title: "名称",
          key: "ModuleName"
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
          title: "管理",
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
  methods: {
    /**
     * 获取列表
     */
    handleGetList() {
      this.list.loading = true;
      // 获取分页信息
      let page = this.$refs["list"].getPage(3);

      this.util.ajax.get(`/CarProducts/GetModuleList?type=3`).then(res => {
        this.list.loading = false;
        if (res.data) {
          this.list = {
            data: res.data,
            loading: false,
            border: true
          };
        }
      });
    },

    /**
     * 添加
     */
    handleCreate() {
      this.edit = Object.assign({}, this.create);
      this.$refs["edit"].showModal(); // 显示模态框
    },

    /**
     * 编辑
     * @param  {object} row 当前行数据
     */
    handleEdit(row) {
      this.$refs["edit"].showModal("edit");

      let edit = Object.assign({}, row);
      this.edit = edit;
    },

    /**
     * 显示日志
     */
    handleShowLog(pkid){
      this.logmodal.loading = true;
      this.util.ajax.post(`/CommonConfigLog/GetCommonConfigLogs`,{
        objectId: pkid,
        objectType: "CarProductsModule"
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