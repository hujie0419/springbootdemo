<template>
  <div>
    <h1 class="title">门店同步</h1>
    <Button v-for="item in companies"
            style="margin-bottom:20px;margin-left:20px"
            :key="item.name"
            :type="item.isSelected ? 'success' : 'default'"
            @click="switchCompany(item.name)">{{item.name}}
    </Button>
    <Form :label-width="100">
      <Row>
        <Col span="4">
        <FormItem label="门店ID:" >
          <Input v-model="searchCondition.shopId"
                 placeholder="门店ID"></Input>
        </FormItem>
        </Col>
        <Col span="4">
        <FormItem label="门店简单名:">
          <Input v-model="searchCondition.shopSimpleName"
                 placeholder="门店简单名"></Input>
        </FormItem>
        </Col>
        <Col span="4">
        <FormItem label="门店名称:">
          <Input v-model="searchCondition.shopFullName"
                 placeholder="门店名称"></Input>
        </FormItem>
        </Col>
        <Col span="4">
        <FormItem label="状态:">
          <Select v-model="searchCondition.syncStatus">
            <Option value="">全部</Option>
            <Option value="Success">同步成功</Option>
            <Option value="Fail">同步失败</Option>
          </Select>
        </FormItem>
        </Col>
        <Button @click="loadData" type="primary">查询</Button>
      </Row>
    </Form>
    <Table border
           :loading="table.loading"
           :columns="table.columns"
           :data="table.data"></Table>
    <div style="margin-top:15px;float:right">
      <Page :total="page.total"
            :page-size="page.pageSize"
            :current="page.current"
            :page-size-opts="[10, 20 ,50 ,100]"
            show-elevator
            show-sizer
            @on-change="handlePageChange"
            @on-page-size-change="handlePageSizeChange"></Page>
    </div>
    <Modal v-model="showSyncResult.visible"
           title="手动同步"
           :loading="showSyncResult.loading"
           @on-ok="closeSyncModal"
           @on-cancel="closeSyncModal">
      <p>{{showSyncResult.msg}}</p>
    </Modal>
  </div>
</template>
<script>
export default {
    data () {
        return {
            companies: [],
            choosedCompany: "平安",
            searchCondition: {
                shopId: "",
                shopSimpleName: "",
                shopFullName: "",
                syncStatus: ""
            },
            page: {
                total: 0,
                current: 1,
                pageSize: 10
            },
            table: {
                loading: true,
                data: [],
                columns: [
                    {
                        title: "门店ID",
                        key: "TuhuShopId"
                    },
                    {
                        title: "门店简单名",
                        key: "ShopSimpleName"
                    },
                    {
                        title: "门店名称",
                        key: "ShopFullName"
                    },
                    {
                        title: "营业时间",
                        key: "WorkTime"
                    },
                    {
                        title: "门店地址",
                        key: "Address"
                    },
                    {
                        title: "同步状态",
                        key: "SyncStatus",
                        render: (h, params) => {
                            return h(
                                "span",
                                {},
                                params.row.SyncStatus === "Success" ? "成功" : "失败"
                            );
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
                                                console.log(params);
                                                this.syncShop(params.row.TuhuShopId);
                                            }
                                        }
                                    },
                                    "手动同步"
                                )
                            ]);
                        }
                    }
                ]
            },
            showSyncResult: {
                visible: false,
                loading: true,
                msg: ""
            }
        };
    },
    created () {
        this.loadCompanies();
        this.loadData();
    },
    methods: {
        loadCompanies () {
            this.ajax
                .get("/ShopSync/GetShopSyncThirdParties", {})
                .then(response => {
                    var data = response.data;
                    data = data && data.length > 0 ? data : ["平安"];
                    this.companies = (data).map(x => {
                        var obj = {};
                        obj.name = x;
                        obj.isSelected = x === this.choosedCompany;
                        return obj;
                    });
                });
        },
        switchCompany (company) {
            this.companies.forEach(x => {
                x.isSelected = x.name === company
            });
            this.choosedCompany = company;
            this.loadData();
        },
        loadData () {
            this.table.loading = true;
            var self = this;
            setTimeout(function () {
                self.ajax
                    .get("/ShopSync/GetShopSyncRecord", {
                        params: {
                            company: self.choosedCompany,
                            pageIndex: self.page.current,
                            pageSize: self.page.pageSize,
                            syncStatus: self.searchCondition.syncStatus,
                            shopId: self.searchCondition.shopId || 0,
                            simpleName: self.searchCondition.shopSimpleName,
                            fullName: self.searchCondition.shopFullName
                        }
                    })
                    .then(response => {
                        var data = response.data;
                        self.page.total = data.totalCount;
                        self.page.current = data.pageIndex;
                        self.table.data = data.data;
                        self.table.loading = false;
                    })
            }, 1000)
        },
        handlePageChange (pageIndex) {
            this.page.current = pageIndex;
            this.loadData();
        },
        handlePageSizeChange (pageSize) {
            this.page.pageSize = pageSize;
            this.loadData();
        },
        syncShop: function (id) {
            var choosedCompany = this.choosedCompany;
            console.log(id)
            this.ajax
                .post("/ShopSync/syncShop", {
                    shopId: id,
                    thirdParty: choosedCompany
                })
                .then(response => {
                    var result = response.data;
                    this.showSyncResult.visible = true;
                    this.showSyncResult.loading = true;
                    this.showSyncResult.msg = result.msg;
                });
        },
        closeSyncModal: function () {
            this.showSyncResult.visible = false;
        }
    }
};
</script>
