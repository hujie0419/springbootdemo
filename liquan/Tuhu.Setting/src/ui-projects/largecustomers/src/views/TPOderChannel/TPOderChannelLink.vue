<template>
  <div>
    <div>
      <Row align="middle"
           justify="space-around">
        <i-col span="7">
          <label class="label">订单渠道Key：</label>
          <Select placeholder="请选择订单渠道Key"
                  v-model="filter.orderChannelEng" style="width:200px">
            <Option value="全部">全部</Option>
            <Option v-for="item in orderChannels"
                    :value="item.OrderChannel"
                    :key="item.OrderChannel">{{item.OrderChannel}}</Option>
          </Select>
        </i-col>
        <i-col span="6">
          <label>页面类型：</label>
          <Select placeholder="请选择页面类型"
                  v-model="filter.businessType" style="width:200px">
            <Option value="全部">全部</Option>
            <Option v-for="item in businessTypes"
                    :value="item.businessType"
                    :key="item.businessType">{{item.businessType}}</Option>
          </Select>
        </i-col>
        <i-col span="6">
          <label>状态：</label>
          <Select placeholder="请选择状态"
                  v-model="filter.statusKey" style="width:200px">
            <Option value="0">全部</Option>
            <Option v-for="item in statusArr"
                    :value="item.statusKey"
                    :key="item.status">{{item.status}}</Option>
          </Select>
        </i-col>
        <i-col span="5">
          <Button type="success"
                  @click="search">查询</Button>
          <Button type="primary"
                  @click="add">添加渠道链接</Button>
        </i-col>
      </Row>
    </div>
    <Table style="margin-top: 15px;"
           border
           stripe
           :columns="table.columns"
           :data="table.data"
           :loading="table.loading"></Table>
    <Page style="margin-top: 15px;"
          show-total
          show-sizer
          :page-size-opts="[10, 20, 40, 100, 200]"
          :total="page.total"
          :current.sync="page.index"
          :page-size="page.size"
          @on-page-size-change="page.size=arguments[0]"></Page>
  </div>
</template>

<script>
export default {
    data () {
        return {
            table: {
                columns: [
                    {
                        title: 'PKID',
                        align: 'center',
                        width: 100,
                        key: "PKID"
                    },
                    {
                        title: '订单渠道Key',
                        align: 'center',
                        key: 'OrderChannel'
                    },
                    {
                        title: '页面类型',
                        align: 'left',
                        key: 'BusinessType'
                    },
                    {
                        title: '状态',
                        align: 'left',
                        key: 'Status',
                        render: function (h, params) {
                        switch (params.row.Status) {
                            case 1:
                            return h("span", "启用");
                            default:
                            return h("span", "禁用");
                            }
                        }
                    },
                    {
                        title: '初始页面链接',
                        align: 'left',
                        key: 'InitialPagelink'
                    },
                    {
                        title: '最终页面链接',
                        align: 'left',
                        key: 'FinalPagelink'
                    },
                    {
                        title: '统计UV链接',
                        align: 'left',
                        key: 'redirectLink'
                    },
                    {
                        title: '额外需求',
                        align: 'left',
                        key: 'AdditionalRequirement'
                    },
                    {
                        title: "修改日期",
                        key: "LastUpdateDateTime",
                        align: 'left',
                        render: (h, params) => {
                        return h('div', [
                                h('span', this.FormatToDate(params.row.LastUpdateDateTime))
                                        ]);
                        }
                    },
                     {
                        title: '修改人',
                        align: 'left',
                        key: 'LastUpdateBy'
                    },
                    {
                        title: '状态操作',
                        width: 100,
                        render: (h, p) => {
                            var statusAction = "";
                            if (p.row.Status === 1) {
                                statusAction = "禁用"
                            } else if (p.row.Status === -1) {
                                statusAction = "启用"
                            }
                            let buttons = [];
                            buttons.push(
                                h(
                                "Button",
                                {
                                    props: {
                                        type: "primary",
                                        size: "small"
                                    },
                                    style: {
                                        marginRight: "3px"
                                    },
                                    on: {
                                    click: () => {
                                        this.updateStatus(p.row.Status, p.row.PKID);
                                     }
                                    }
                                },
                                statusAction
                                )
                            );
                            return h("div", buttons);
                        }
                    }
                ],
                data: [],
                loading: false
            },
            page: { total: 0, index: 1, size: 10 },
            orderChannels: [],
            businessTypes: [
                {
                    businessType: "轮胎"
                },
                {
                    businessType: "保养"
                },
                {
                    businessType: "洗车（美容）"
                },
                {
                    businessType: "车品"
                },
                {
                    businessType: "蓄电池"
                },
                {
                    businessType: "钣喷"
                },
                {
                    businessType: "首页"
                },
                {
                    businessType: "其他"
                }
            ],
            statusArr: [
                {
                    statusKey: 1,
                    status: "启用"
                },
                {
                    statusKey: -1,
                    status: "禁用"
                }
            ],
            filter: {
                orderChannelEng: '全部',
                businessType: '全部',
                statusKey: "0"
            },
            status: 0
        }
    },
    created () {
        this.loadOrderChanel();
        this.loadData();
        this.$Message.config({
            duration: 5
        });
    },
    watch: {
        "page.index" () {
            this.loadData();
        },
        "page.size" () {
            this.search();
        }
    },
    methods: {
        FormatToDate (timestamp) {
            var time = new Date(parseInt(timestamp.replace("/Date(", "").replace(")/", ""), 10));
            var year = time.getFullYear();
            var month = time.getMonth() + 1 < 10 ? "0" + (time.getMonth() + 1) : time.getMonth() + 1;
            var date = time.getDate() < 10 ? "0" + time.getDate() : time.getDate();
            var hour = time.getHours() < 10 ? "0" + time.getHours() : time.getHours();
            var minute = time.getMinutes() < 10 ? "0" + time.getMinutes() : time.getMinutes();
            var second = time.getSeconds() < 10 ? "0" + time.getSeconds() : time.getSeconds();
            var YmdHis = year + '/' + month + '/' + date + ' ' + hour + ':' + minute + ':' + second;
            return YmdHis;
        },
        search () {
            if (this.page.index === 1) {
                this.loadData();
            } else {
                this.page.index = 1;
            }
        },
        loadData () {
            this.table.data = [];
            this.table.loading = true;
            this.ajax.get(`/ThirdPartyOrderChannellink/GetTPOrderChannellinkList?orderChannel=${this.filter.orderChannelEng}&businessType=${this.filter.businessType}&status=${this.filter.statusKey}&pageSize=${this.page.size}&pageIndex=${this.page.index}`).then(response => {
                var res = response.data.data;
                res.forEach(element => {
                    element.redirectLink = decodeURIComponent(element.FinalPagelink.split('url=')[1]);
                });
                this.page.total = response.data.dataCount;
                this.table.data = res || [];
                this.table.loading = false;
            })
        },
        updateStatus (status, pkid) {
            this.ajax.post(`/ThirdPartyOrderChannellink/UpdateTPOrderChannellinkStatus?status=${status}&PKID=${pkid}`).then(response => {
                var res = response.data.data;
               if (res > 0) {
                this.lazyLoadData();
               }
            })
        },
        lazyLoadData () {
            this.table.loading = true;
            setTimeout(() => {
                this.search();
            }, 1000);
        },
        loadOrderChanel () {
            this.ajax.get('/ThirdPartyOrderChannellink/GetThirdPartyOrderChannelList')
                .then(response => {
                    var res = response.data.data;
                    this.orderChannels = res || [];
                })
        },
        add () {
            this.$router.push({'name': 'AddTPOderChannelLink'});
        }
    }
}
</script>
