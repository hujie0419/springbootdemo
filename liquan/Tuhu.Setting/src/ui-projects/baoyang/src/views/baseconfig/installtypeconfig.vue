<template>
  <div>
      <div v-if="mode === 'list'">
          <h1 class="title">切换服务配置</h1>
          <p style="padding:8px;background-color:#F3F3F4;line-height:21px;">
              使用前必读：<br/>
              1. 切换安装方式文案中的 {{tips}} (<b>大小写敏感</b>) 会被自动替换成价格 <br/>
              2. 如需修改前减振器, 后减振器的默认显示项目，请找研发 <br/>
              3. 对于自动变速箱油，6年或12W公里以上的车型默认出含养护包
          </p>
          <Card v-for="config in configs" v-bind:key="config.PackageType" style="width:880px; margin-top:18px;">
                <p slot="title">{{ config.PackageName }}</p>
                <div v-for="installType in config.InstallTypes" v-bind:key="installType.Type">
                    <label style="font-weight:bold;display:inline-block;width:200px;">{{ installType.Name }}:</label>
                    <label style="display:inline-block;width:380px;">{{ installType.BaoYangTypeNames.replace(/,/g, ' + ') }}</label>
                    <Radio v-model="installType.IsDefault" @on-change="handleDefaultChange(config.PackageType, installType.Type)">默认显示此项目</Radio>
                    <a v-if="!installType.IsDefault" type="primary" @click="mode='edit';editdata.packageType=config.PackageType;editdata.installType=installType.Type">配置特殊车型的默认显示</a>
                    <div v-for="(textFormat, index) in installType.TextFormats" v-bind:key="index" style="margin-bottom:18px;margin-top:18px">
                        <Input v-model="textFormat.Text" placeholder="文字" style="width: 150px"></Input>
                        <Input v-model="textFormat.Color" placeholder="色值" style="width: 80px;margin-left:8px;"></Input>
                        <ColorPicker v-model="textFormat.Color" style="margin-left:8px;"/>
                        <Button style="margin-left:8px;" @click="installType.TextFormats.splice(index, 1)">删除</Button>
                    </div>                    
                    <div style="margin-bottom:18px;margin-top:18px">
                        <Button type="dashed" @click="installType.TextFormats.push({'Text': '', 'Color': ''})">添加文字</Button>
                    </div>
                    <div style="margin-bottom:18px;margin-top:18px">
                        <span>文字效果展示：</span>
                        <span v-for="(textFormat, index) in installType.TextFormats" v-bind:key="index" :style="{ color: textFormat.Color }">{{textFormat.Text}}</span>
                    </div>
                </div>
                <div style="margin-bottom:18px">
                    <div>
                    <img v-if="config.ImageUrl" :src="'//img1.tuhu.org' + config.ImageUrl" style="height:50px">
                    </div>
                    <Upload 
                        style="display:inline; width:150px;"
                        action="/utils/UploadImage"
                        :data="{'key':config.PackageType, 'path': '/baoyang/'}"
                        :default-file-list="config.files"
                        :show-upload-list="false"
                        :format="['jpg','jpeg','png']"
                        :max-size="200"
                        :on-format-error="handleFormatError"
                        :on-exceeded-size="handleMaxSize"
                        :on-success="handleSuccess">
                        <Button type="ghost" icon="ios-cloud-upload-outline">上传品牌图片</Button>
                    </Upload>
                </div>
                    <Button :loading="config.loading" type="success" @click="save(config)">保存</Button>
            </Card>
      </div>
      <div v-else>
        <h1 class="title">特殊车型默认显示<Button type="primary" @click="resetVehicleData" style='margin-left:18px'>返回</Button></h1>
        <selectvehicle v-bind:searchdata="searchinitdata" @search="handleSearch"/>
        <div style="height:45px">
            <Button type="error" :disabled="this.table.selecteddata.length == 0" style="float:right;margin-right:8px;" @click="handleBulkDisable">批量禁用</Button>
            <Button type="primary" :disabled="this.table.selecteddata.length == 0" style="float:right;margin-right:8px;" @click="handleBulkEnable">批量启用</Button>
        </div>
        <Table border :loading="table.loading" :columns="table.columns" :data="table.data" @on-selection-change="selectChange"></Table>
        <div style="margin-top:15px;float:right">
            <Page  :total="page.total" :page-size="page.pageSize" :current="page.current" :page-size-opts="[20 ,50 ,100]" show-elevator show-sizer @on-change="handlePageChange" @on-page-size-change="handlePageSizeChange"></Page>
        </div>
      </div>
      <Modal
        v-model="operateModal.visible"
        :title="operateModal.title"
        :loading="operateModal.loading"
        @on-ok="operateVehicleConfig">
        <p>{{operateModal.content}}</p>
    </Modal>
  </div>
</template>

<script>
import selectvehicle from "@/components/selectvehicle.vue"

export default {
    components: {
        selectvehicle
    },
    data () {
        return {
            mode: "list",
            tips: "{{price}}",
            configs: [],
            images: {},
            table: {
                selecteddata: [],
                loading: false,
                data: [],
                columns: [
                    {
                    type: 'selection',
                    width: 60,
                    align: 'center'
                    },
                    {
                        title: 'VehicleId',
                        key: 'VehicleId'
                    },
                    {
                        title: '品牌',
                        key: 'Brand'
                    },
                    {
                        title: '车系',
                        key: 'Series'
                    },
                    {
                        title: '是否强制推出',
                        key: 'IsRecommend',
                        render: (h, params) => {
                            return h('span', {}, params.row.IsRecommend ? "是" : "\\");
                        }
                    },
                    {
                        title: '操作',
                        key: 'action',
                        width: 200,
                        align: 'center',
                        render: (h, params) => {
                            return h('div', [
                                h('Button', {
                                    props: {
                                        type: 'primary',
                                        size: 'small',
                                        disabled: params.row.IsRecommend
                                    },
                                    style: {
                                        marginRight: '5px'
                                    },
                                    on: {
                                        click: () => {
                                            this.operateModal.visible = true;
                                            this.operateModal.title = "确认";
                                            this.operateModal.content = "确认启用？";
                                            this.operateModal.vehicleIds = this.table.data[params.index].VehicleId;
                                            this.operateModal.operateType = "enable";
                                        }
                                    }
                                }, '启用'),
                                h('Button', {
                                    props: {
                                        type: 'error',
                                        size: 'small',
                                        disabled: !params.row.IsRecommend
                                    },
                                    on: {
                                        click: () => {
                                            this.operateModal.visible = true;
                                            this.operateModal.title = "确认";
                                            this.operateModal.content = "确认禁用？";
                                            this.operateModal.vehicleIds = this.table.data[params.index].VehicleId;
                                            this.operateModal.operateType = "disable";
                                        }
                                    }
                                }, '禁用')
                            ]);
                        }
                    }
                ]
            },
            page: {
                total: 0,
                current: 1,
                pageSize: 20
            },
            editdata: {
                packageType: "",
                installType: ""
            },
            searchdata: {},
            searchinitdata: {
                brand: "",
                series: "",
                brandcategories: [],
                minprice: 0,
                maxprice: 9999,
                brands: [],
                vehicleId: "",
                isconfiged: false
            },
            operateModal: {
                visible: false,
                loading: true,
                vehicleIds: "",
                operateType: "",
                content: ""
            }
        }
    },
    created () {
        this.loaddata();
    },
    methods: {
        loaddata () {
            this.ajax.get("/baoyang/GetInstallTypeConfigs").then((response) => {
                response.data.forEach(config => {
                    config.loading = false;
                    config.InstallTypes.forEach(installType => {
                        if (!installType.TextFormats) {
                            installType.TextFormats = [];
                        }
                    })
                })

                this.configs = response.data;
            });
        },
        loadVehicleConfig (pageIndex, pageSize) {
            this.table.loading = true;
            this.ajax.post("/baoyang/GetInstallTypeVehicleConfigs", {
                packageType: this.editdata.packageType,
                installType: this.editdata.installType,
                brand: this.searchdata.brand,
                series: this.searchdata.series,
                categories: this.searchdata.brandcategories.join(','),
                minprice: this.searchdata.minprice,
                maxprice: this.searchdata.maxprice,
                brands: this.searchdata.brands.join(','),
                vehicleId: this.searchdata.vehicleId,
                isconfig: this.searchdata.isconfiged,
                pageIndex: pageIndex,
                pageSize: pageSize
             }).then(response => {
                this.table.data = response.data.data;
                this.page.total = response.data.totalCount;
                console.log(response.data);
                this.table.loading = false;
                this.table.selecteddata = [];
            })
        },
        handleView (name) {
            this.imgName = name;
            this.visible = true;
        },
        handleRemove (file) {
            const fileList = this.$refs.upload.fileList;
            this.$refs.upload.fileList.splice(fileList.indexOf(file), 1);
        },
        handleFormatError (file) {
            this.util.message.warning('图片格式错误, 请上传jpg, jpeg, png格式的图片');
        },
        handleMaxSize (file) {
            console.log(this.util);
            this.util.message.warning('图片大小不能超过50kb');
        },
        handleSuccess (res, file) {
            file.url = res.Url;
            file.name = res.Name;
            this.configs.forEach(config => {
                if (config.PackageType === res.Key) {
                    config.ImageUrl = res.Url;
                }
            });
        },
        handleSearch (searchdata) {
            this.page.current = 1;
            this.searchdata = searchdata;
            this.loadVehicleConfig(this.page.current, this.page.pageSize);
        },
        handleDefaultChange (packageType, installType) {
            console.log(packageType, installType);
            this.configs.forEach(config => {
                if (config.PackageType === packageType) {
                    config.InstallTypes.forEach(installTypeConfig => {
                        installTypeConfig.IsDefault = installTypeConfig.Type === installType;
                    });
                }
            });
        },
        handlePageChange (pageIndex) {
            this.page.current = pageIndex;
            this.loadVehicleConfig(pageIndex, this.page.pageSize);
        },
        handlePageSizeChange (pageSize) {
            this.page.pageSize = pageSize;
            this.loadVehicleConfig(1, pageSize);
        },
        selectChange (selected) {
            this.table.selecteddata = selected;
        },
        handleBulkEnable () {
            this.operateModal.visible = true;
            this.operateModal.title = "确认";
            this.operateModal.content = "确认启用？";
            this.operateModal.vehicleIds = this.table.selecteddata.map(item => item.VehicleId).join(',');
            this.operateModal.operateType = "enable";
        },
        handleBulkDisable () {
            this.operateModal.visible = true;
            this.operateModal.title = "确认";
            this.operateModal.content = "确认禁用？";
            this.operateModal.vehicleIds = this.table.selecteddata.map(item => item.VehicleId).join(',');
            this.operateModal.operateType = "disable";
        },
        updateCache (type, data) {
            this.ajax.post("/baoyang/UpdateCache", {
                type: type,
                data: data
            }).then(response => {
                if (response.data.success) {
                    this.util.message.success("缓存更新成功");
                } else {
                    this.util.message.success("缓存更新失败");
                }
            });
        },
        operateVehicleConfig () {
            var url = this.operateModal.operateType === 'enable'
                ? "/baoyang/AddInstallTypeVehicleConfig" 
                : "/baoyang/DeleteInstallTypeVehicleConfig";
            this.ajax.post(url, {
                packageType: this.editdata.packageType,
                installType: this.editdata.installType,
                vehicleIds: this.operateModal.vehicleIds
            }).then(response => {
                if (response.data.success) {
                    setTimeout(() => {
                        this.util.message.success("更新成功");
                        this.loadVehicleConfig(this.page.current, this.page.pageSize);
                        this.operateModal.visible = false;
                        this.operateModal.loading = false;
                        this.$nextTick(() => {
                          this.operateModal.loading = true;
                        });
                        this.updateCache("InstallType_VehicleConfig", this.operateModal.vehicleIds.split(','))
                    }, 2000);
                } else {
                    this.message.error("更新失败");
                    this.operateModal.loading = false;
                    this.$nextTick(() => {
                        this.operateModal.loading = true;
                    });
                }
            });
        },
        save (config) {
            if (config) {
                config.loading = true;
                console.log(this.configs)
                this.ajax.post("/baoyang/UpdateInstallType", config).then((response) => {
                    if (response.data.success) {
                        setTimeout(() => {
                            this.util.message.success("更新成功");
                            this.loaddata();
                            config.loading = false;
                            this.updateCache("InstallType_Config", []);
                        }, 2000);
                    } else {
                        this.message.error("更新失败");
                        config.loading = false;
                    }
                });
            }
        },
        resetVehicleData () {
            this.mode = 'list';
            this.table.data = [];
            this.table.selecteddata = [];
            this.page = {
                total: 0,
                current: 1,
                pageSize: 20
            };
            this.searchdata = [];
            this.searchinitdata = {
                brand: "",
                series: "",
                brandcategories: [],
                minprice: 0,
                maxprice: 9999,
                brands: [],
                vehicleId: "",
                isconfiged: false
            };
        }
    }
}
</script>

<style lang="less">

</style>
