<template>
<div>
         <Row>
      <Col span="4">
      <div style="margin:5px;font-size:14px;">日志信息</div>
      </Col>
    </Row>
        <div>
      <Table stripe border :loading="log.table.loading" :columns="log.table.columns" :data="log.table.data"></Table>
    </div>
    <div style="margin-top:15px;float:right">
      <Page show-total :total="log.page.total" :page-size="log.page.pageSize" :current="log.page.current" :page-size-opts="[10,20 ,50 ,100]"
        show-elevator show-sizer @on-change="handleLogPageChange" @on-page-size-change="handleLogPageSizeChange">
      </Page>
    </div>
</div>
</template>
<script>
  import expandrow from '@/views/discount/expandrow'
  // import loglist from '@/views/discount/loglist'
export default {
    name: 'loglist',
    props: {
activityId: String
    },
  data () {
      return {
            log: {
          page: {
            total: 0,
            current: 1,
            pageSize: 20
          },
          table: {
            loading: false,
            data: [],
            columns: [{
                        type: 'expand',
                        width: 50,
                        render: (h, params) => {
                            return h(expandrow, {
                                props: {
                                    row: params.row
                                }
                            });
                    }
                    }, {
                title: "操作",
                key: "OperationLogDescription",
                align: "center"
              },
              {
                title: "操作时间",
                key: "CreateDateTime",
                align: "center"
              },
              {
                title: "操作人",
                key: "CreateUserName",
                align: "center"
              }
            ]
          }
        }
      }
  },
  created () {
this.loadLog();
  },
  methods: {
      loadLog () {
           this.log.table.loading = true;
           this.ajax
          .post("/salepromotionactivity/GetOperationLogList", {
              activityId: this.activityId,
            pageIndex: this.log.page.current,
            pageSize: this.log.page.pageSize
          })
          .then(response => {
            this.log.table.loading = false;
            var data = response.data;
            this.log.table.data = data.List;
            this.log.page.total = data.Total;
          });
      },
            handleLogPageChange (pageIndex) {
        this.log.page.current = pageIndex;
        this.loadLog();
      },
      handleLogPageSizeChange (pageSize) {
        this.log.page.pageSize = pageSize;
        this.loadLog();
      }
  }
}
</script>
