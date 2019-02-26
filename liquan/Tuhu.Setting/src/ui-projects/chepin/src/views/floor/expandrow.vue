<template>
  <div v-if="config.data.length > 0" style="width:400px;margin-left: 120px;">
    <Table :columns="config.columns" :data="config.data" :show-header="config.header"></Table>
  </div>
</template>
<script>
export default {
  props: {
    row: Object
  },
  data() {
    return {
      config: {
        data: [],
        header: false,
        columns: [
          {
            title: "楼层",
            key: "Name",
            width: 200,
            align: "center"
          },
          {
            title: "商品数",
            key: "PidCount",
            width: 100,
            align: "center"
          },
          {
            title: "操作",
            key: "operation",
            align: "center",
            render: (h, params) =>
              h("div", [
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
                )
              ])
          }
        ]
      }
    };
  },
  created() {
    this.handleGetList();
  },
  inject: ["getList"],
  methods: {
    /**
     * 获取列表
     */
    handleGetList() {
      this.ajax
        .get(`/CarProducts/FloorConfigList?floorId=` + this.row.PKID)
        .then(response => {
          this.config.data = response.data;
        });
    },

    /**
     * 删除数据
     * @param  {object} row 当前行数据
     */
    handleDelete(row) {
      var id = row.PKID;
      if (id != "" || id != null) {
        this.ajax.post(`/CarProducts/DeleteFloorConfig?id=` + id).then(res => {
          if (res.data) {
            this.$Message.success("删除成功");
            this.handleGetList();
          } else {
            this.$Message.error("删除失败");
          }
        });
      }
    }
  }
};
</script>
<style lang="less">
.ivu-table-expanded-cell {
  padding: 1px 30px !important;
}
</style>
