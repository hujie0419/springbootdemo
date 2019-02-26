<template>
    <div>
        <h1 class="title">平安门店同步配置</h1>
        <Form :label-width="100">
            <Row>
                <Col span="5">
                    <FormItem label="机构ID:" >
                         <i-Input v-model="filterCondition.businessId"  placeholder="机构ID"></i-Input>
                    </FormItem>
                </Col>
                <Col span="5">
                    <FormItem label="美容包ID:" >
                         <i-Input v-model="filterCondition.packageId"  placeholder="美容包ID"></i-Input>
                    </FormItem>
                </Col>
                <Col span="5">
                    <FormItem label="保养套餐ID:" >
                         <i-Input v-model="filterCondition.byPackagePID"  placeholder="保养套餐ID"></i-Input>
                    </FormItem>
                </Col>
                <Col span="4">
                     <FormItem >
                        <i-Button @click="loadData" type="primary">查询</i-Button>
                        <i-Button @click="addShopConfig" type="primary">添加</i-Button>
                    </FormItem>
                </Col>
            </Row>
        </Form>
        <Table border :loading="table.loading" :columns="table.columns" :data="table.data"></Table>
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
         <Modal v-model="modal.visible" :mask-closable="false" :loading="modal.loading" title="门店同步配置（编辑）" 
            okText="提交" :transfer="false" cancelText="取消" @on-ok="ok()" scrollable width="30%">
            <Form ref="modal.syncConfig" :model="modal.syncConfig"  :label-width="90">
                <FormItem label="机构ID">
                     <Col span="8">
                         <i-Input v-model="modal.syncConfig.BusinessId" :width="150" :disabled="!modal.isAdd"  placeholder="机构ID" ></i-Input>
                     </Col>
                </FormItem>
                <FormItem label="地区">
                    <Col span="10">
                        <Select v-model="modal.syncConfig.ProvinceId" :disabled="!modal.isAdd" placeholder="" @on-change="provinceChange" style="width:80%" transfer :filterable="true">
                            <Option v-for="item in allProvinceOptions" :value="item.value" :key="item.value">{{ item.text }}</Option>
                        </Select>
                    </Col>
                    <Col span="10">
                        <Select v-model="modal.syncConfig.CityId" :disabled="!modal.isAdd" placeholder="市" style="width:80%" transfer :filterable="true">
                            <Option v-for="item in allCityOptions" :value="item.value" :key="item.value">{{ item.text }}</Option>
                        </Select>
                    </Col>
                </FormItem>
                <FormItem label="业务类型">
                    <RadioGroup v-model="modal.syncConfig.PackageType" >
                        <Radio label="MeiRong" :disabled="!modal.isAdd">
                            <span>美容包ID</span>
                        </Radio>
                        <Radio label="BaoYang" :disabled="!modal.isAdd">
                            <span>保养套餐PID</span>
                        </Radio>
                    </RadioGroup>
                </FormItem>
                <FormItem label="美容包ID" v-if="modal.syncConfig.PackageType=='MeiRong'">
                    <Select v-model="modal.syncConfig.PackageId" :disabled="!modal.isAdd" placeholder="美容包ID" style="width:80%" transfer :filterable="true">
                        <Option v-for="item in packageIdOptions" :value="item.value" :key="item.value">{{ item.text }}</Option>
                    </Select>
                </FormItem>
                <FormItem label="保养套餐PID"  v-if="modal.syncConfig.PackageType=='BaoYang'">
                    <Select v-model="modal.syncConfig.BYPackagePID" :disabled="!modal.isAdd" placeholder="保养套餐PID" style="width:80%" transfer :filterable="true">
                        <Option v-for="item in packagePIDOptions" :value="item.value" :key="item.value">{{ item.text }}</Option>
                    </Select>
                </FormItem>
            </Form>
        </Modal>
    </div>
</template>
<script>
export default {
    data () {
        return {
            filterCondition: {
                businessId: "",
                packageId: "",
                byPackagePID: ""
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
                        title: "PKID",
                        key: "PKID",
                        align: "center"
                    },
                    {
                        title: "地区名称",
                        key: "RegionName",
                        align: "center",
                        render: (h, params) => {
                            if (params.row.ProvinceName) {
                                return h("span", params.row.RegionName);
                            } else {
                                return h("span", "/");
                            }
                        }
                    },
                    {
                        title: "美容包ID",
                        key: "PackageId",
                        align: "center",
                        render: (h, params) => {
                            if (params.row.PackageId) {
                                return h("span", params.row.PackageId);
                            } else {
                                return h("span", "/");
                            }
                        }
                    },
                    {
                        title: "保养套餐PID",
                        key: "BYPackagePID",
                        align: "center",
                        render: (h, params) => {
                            if (params.row.BYPackagePID) {
                                return h(
                                    "span",
                                    params.row.BYPackagePID
                                );
                            } else {
                                return h("span", "/");
                            }
                        }
                    },
                    {
                        title: "机构ID",
                        key: "BusinessId",
                        align: "center"
                    },
                    {
                        title: "创建时间",
                        key: "CreatedDateTime",
                        align: "center",
                        render: (h, params) => {
                            return h(
                                "span",
                                this.formatDate(params.row.CreatedDateTime)
                            );
                        }
                    },
                    {
                        title: "更新时间",
                        key: "UpdatedDateTime",
                        align: "center",
                        render: (h, params) => {
                            return h(
                                "span",
                                this.formatDate(params.row.UpdatedDateTime)
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
                                                this.editShopConfig(params.row);
                                            }
                                        }
                                    },
                                    "查看"
                                )
                            ]);
                        }
                    }
                ]
            },
            packageIdOptions: [
                 {"value": "", "text": "请选择"}
            ],
            packagePIDOptions: [
                {"value": "", "text": "请选择"}
            ],
            allProvinceOptions: [
                {"value": 0, "text": "请选择"}
            ],
            allCityOptions: [
                {"value": 0, "text": "请选择"}
            ],
            modal: {
                visible: false,
                loading: true,
                edit: true,
                isAdd: true,
                syncConfig: {
                    PKID: 0,
                    PackageId: "",
                    BYPackagePID: "",
                    BusinessId: "",
                    PackageType: "",
                    ProvinceId: 0,
                    CityId: 0
                }
            }
        }
    },
    created () {
        this.loadData();
        this.GetAllThirdPartyBeautyPackageConfig();
        this.SelectAllVipBaoYangPackage();
        this.GetAllProvince();
    },
    methods: {
        loadData () {
            this.table.loading = true;
            var self = this;
             self.ajax.get("/ShopSync/GetPingAnRegionPackageMapList", {
                        params: {
                            pageIndex: self.page.current,
                            pageSize: self.page.pageSize,
                            packageId: self.filterCondition.packageId,
                            byPackagePID: self.filterCondition.byPackagePID,
                            businessId: self.filterCondition.businessId
                        }
                    })
                    .then(response => {
                        var data = response.data;
                        self.page.total = data.total;
                        self.table.data = data.data;
                        self.table.loading = false;
                    })
        },
        GetAllProvince () {
            var self = this;
            self.ajax.get("/ShopSync/GetAllProvince")
                    .then(response => {
                         (response.data || []).forEach(function (v) {
                            self.allProvinceOptions.push({
                                value: v.RegionId,
                                text: v.RegionName
                            })
                        })
                    })
        },
        provinceChange () {
            var self = this;
            self.ajax
                .post("/ShopSync/GetRegionByRegionId", {regionId: self.modal.syncConfig.ProvinceId})
                    .then(response => {
                         (response.data.ChildRegions || []).forEach(function (v) {
                            self.allCityOptions.push({
                                value: v.RegionId,
                                text: v.RegionName
                            })
                        })
                    })
        },
        GetAllThirdPartyBeautyPackageConfig () {
            var self = this;
            self.ajax.get("/ShopSync/GetAllThirdPartyBeautyPackageConfig")
                    .then(response => {
                         (response.data || []).forEach(function (v) {
                            self.packageIdOptions.push({
                                value: v.PackageId,
                                text: v.PackageName
                            })
                        })
                    })
        },
        SelectAllVipBaoYangPackage () {
            var self = this;
            self.ajax.get("/ShopSync/SelectAllVipBaoYangPackage")
                    .then(response => {
                         (response.data || []).forEach(function (v) {
                            self.packagePIDOptions.push({
                                value: v.PID,
                                text: v.PackageName
                            })
                        })
                    })
        },
        addShopConfig () {
            var self = this;
            self.modal.visible = true;
            self.modal.isAdd = true;
            self.modal.syncConfig.PKID = 0;
            self.modal.syncConfig.PackageId = "";
            self.modal.syncConfig.BYPackagePID = "";
            self.modal.syncConfig.BusinessId = "";
            self.modal.syncConfig.PackageType = "MeiRong";
            self.modal.syncConfig.ProvinceId = 0;
            self.modal.syncConfig.CityId = 0;
        },
        editShopConfig (item) {
            var self = this;
            self.modal.visible = true;
            self.modal.isAdd = false;
            self.modal.syncConfig.PKID = item.PKID;
            self.modal.syncConfig.BusinessId = item.BusinessId;
            self.modal.syncConfig.ProvinceId = item.ProvinceId;
            if (item.BYPackagePID) {
                self.modal.syncConfig.PackageId = "";
                self.modal.syncConfig.BYPackagePID = item.BYPackagePID;
                self.modal.syncConfig.PackageType = "BaoYang";
            } else {
                self.modal.syncConfig.PackageId = item.PackageId;
                self.modal.syncConfig.BYPackagePID = "";
                self.modal.syncConfig.PackageType = "MeiRong";
            }
            self.ajax
                .post("/ShopSync/GetRegionByRegionId", {regionId: self.modal.syncConfig.ProvinceId})
                    .then(response => {
                         (response.data.ChildRegions || []).forEach(function (v) {
                            self.allCityOptions.push({
                                value: v.RegionId,
                                text: v.RegionName
                            })
                        })
                        self.modal.syncConfig.CityId = item.CityId;
                    })
        },
        handlePageChange (pageIndex) {
            this.page.current = pageIndex;
            this.loadData();
        },
        handlePageSizeChange (pageSize) {
            this.page.pageSize = pageSize;
            this.loadData();
        },
        ok () {
            var self = this;
            self.modal.loading = true;
            if (self.modal.syncConfig.ProvinceId > 0 || self.modal.syncConfig.CityId > 0) {
                if (self.modal.isAdd) {
                    self.ajax.post("/ShopSync/UpsertPingAnRegionPackageMap", { data: self.modal.syncConfig })
                        .then(response => {
                            if (response.data.status) {
                                this.$Message.success("操作成功");
                                this.modal.visible = false;
                                this.loadData();
                            } else {
                                this.modal.loading = false;
                                this.$nextTick(() => {
                                    this.modal.loading = true;
                                });
                                this.$Message.error(response.data.msg);
                            }
                        })
                } else {
                    this.modal.loading = false;
                    this.$nextTick(() => {
                                this.modal.loading = true;
                    });
                    this.$Message.error("不允许更改");
                }
            } else {
                this.modal.loading = false;
                this.$nextTick(() => {
                            this.modal.loading = true;
                });
                this.$Message.error("未获取到地区ID");
            }
        },
        formatDate (value) {
            if (value) {
                var type = typeof value;
                if (type === 'string') {
                    if (value.indexOf("Date") > 0) {
                        var time = new Date(
                            parseInt(value.replace("/Date(", "").replace(")/", ""))
                        );
                        var year = time.getFullYear();
                        var day = time.getDate();
                        var month = time.getMonth() + 1;
                        var hours = time.getHours();
                        var minutes = time.getMinutes();
                        var seconds = time.getSeconds();
                        var func = function (value, number) {
                            var str = value.toString();
                            while (str.length < number) {
                                str = "0" + str;
                            }
                            return str;
                        };
                        if (year === 1) {
                            return "";
                        } else {
                            return (
                                func(year, 4) +
                                "-" +
                                func(month, 2) +
                                "-" +
                                func(day, 2) +
                                " " +
                                func(hours, 2) +
                                ":" +
                                func(minutes, 2) +
                                ":" +
                                func(seconds, 2)
                            );
                        }
                    } else {
                        return value;
                    }
                } else {
                    var year1 = value.getFullYear();
                    var day1 = value.getDate();
                    var month1 = value.getMonth() + 1;
                    var hours1 = value.getHours();
                    var minutes1 = value.getMinutes();
                    var seconds1 = value.getSeconds();
                    var func1 = function (value, number) {
                        var str = value.toString();
                        while (str.length < number) {
                            str = "0" + str;
                        }
                        return str;
                    };
                    if (year === 1) {
                        return "";
                    } else {
                        return (
                            func1(year1, 4) +
                            "-" +
                            func1(month1, 2) +
                            "-" +
                            func1(day1, 2) +
                            " " +
                            func1(hours1, 2) +
                            ":" +
                            func1(minutes1, 2) +
                            ":" +
                            func1(seconds1, 2)
                        );
                    }
                }
            }
        }
    }
}
</script>
