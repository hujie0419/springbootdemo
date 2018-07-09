<template>
  <div>
    <div>
      <Row type="flex"
           align="middle"
           justify="space-around">
        <i-col span="5">
          <label class="label">品牌:</label>
          <Select class="filter-element"
                  filterable
                  placeholder="请选择蓄电池品牌"
                  v-model="filter.brand">
            <Option value="">请选择蓄电池品牌</Option>
            <Option v-for="item in brands"
                    :value="item"
                    :key="item">{{item}}</Option>
          </Select>
        </i-col>
        <i-col span="5">
          <label>省份:</label>
          <Select class="filter-element"
                  filterable
                  placeholder="请选择省份"
                  v-model="filter.province"
                  @on-change="getCities">
            <Option value="">请选择省份</Option>
            <Option v-for="item in provinces"
                    :value="item.Id"
                    :key="item.Name">{{item.Name}}</Option>
          </Select>
        </i-col>
        <i-col span="5">
          <label>城市:</label>
          <Select class="filter-element"
                  filterable
                  placeholder="请选择城市"
                  v-model="filter.city">
            <Option value="">请选择城市</Option>
            <Option v-for="item in cities"
                    :value="item.CityId"
                    :key="item.CityName">{{item.CityName}}</Option>
          </Select>
        </i-col>
        <i-col span="9">
          <Button type="primary"
                  @click="search">查询</Button>
          <Button type="success"
                  @click="add">添加</Button>
          <Button type="default"
                  @click="downloadTmpl">下载模板</Button>
          <Button type="default"
                  @click="uploadExcel">导入</Button>
          <Button type="default"
                  @click="exportExcel">导出</Button>
        </i-col>
      </Row>
    </div>
    <Table style="margin-top: 15px;"
           border
           stripe
           :columns="table.columns"
           :data="table.data"
           :loading="table.loading"
           @on-selection-change="table.selection = arguments[0]"></Table>
    <Page style="margin-top: 15px;"
          show-elevator
          show-total
          show-sizer
          :page-size-opts="[10, 20, 40, 100, 200]"
          :total="page.total"
          :current.sync="page.index"
          :page-size="page.size"
          @on-page-size-change="page.size=arguments[0]"></Page>
    <Modal :title="modal.title"
           ok-text="提交"
           v-model="modal.show"
           :closable="modal.closable"
           :mask-closable="false"
           :loading="modal.loading"
           @on-ok="submit">
      <Form label-position="right"
            :label-width="80"
            :model="modal.formItem">
        <FormItem label="品牌">
          <Select filterable
                  placeholder="请选择蓄电池品牌"
                  v-model="modal.formItems.brand">
            <Option value="">请选择蓄电池品牌</Option>
            <Option v-for="item in brands"
                    :value="item"
                    :key="item">{{item}}</Option>
          </Select>
        </FormItem>
        <FormItem label="地区">
          <Row type="flex"
               align="middle"
               justify="space-around">
            <i-col span="11">
              <Select filterable
                      placeholder="请选择省份"
                      v-model="modal.formItems.provinceId"
                      @on-change="getModalCities">
                <Option value="">请选择省份</Option>
                <Option v-for="item in provinces"
                        :value="item.Id"
                        :key="item.Name">{{item.Name}}</Option>
              </Select>
            </i-col>
            <i-col span="2"></i-col>
            <i-col span="11">
              <Select filterable
                      placeholder="请选择城市"
                      v-model="modal.formItems.cityId">
                <Option value="">请选择城市</Option>
                <Option v-for="item in modal.cities"
                        :value="item.CityId"
                        :key="item.CityName">{{item.CityName}}</Option>
              </Select>
            </i-col>
          </Row>
        </FormItem>
        <FormItem label="渠道">
          <CheckboxGroup v-model="modal.formItems.channels">
            <Checkbox label="online">
              <span>线上展示</span>
            </Checkbox>
            <Checkbox label="u门店">
              <span>线下展示</span>
            </Checkbox>
          </CheckboxGroup>
        </FormItem>
        <FormItem label="是否隐藏">
          <Checkbox v-model="modal.formItems.disabled"></Checkbox>
        </FormItem>
      </Form>
    </Modal>
    <Modal :title="uploadModal.title"
           ok-text="上传"
           v-model="uploadModal.show"
           :closable="uploadModal.closable"
           :mask-closable="false"
           :loading="uploadModal.loading"
           @on-cancel="uploadModal.file = null"
           @on-ok="submitUpload">
      <Upload :before-upload="handleUpload"
              type="drag"
              :show-upload-list="false"
              :on-success="handleSuccess"
              :on-error="handleError"
              action="/BaoYangBattery/ImportBaoYangBatteryCoverArea"
              ref="upload">
        <div style="padding: 20px 0">
          <Icon type="ios-cloud-upload"
                size="52"
                style="color: #3399ff"></Icon>
          <p>选择文件或者将文件拖拽到此处</p>
        </div>
      </Upload>
      <div v-if="uploadModal.file !== null"
           style="text-align:center">文件: {{ uploadModal.file.name }}</div>
    </Modal>
  </div>
</template>

<script>
export default {
    data () {
        return {
            table: {
                columns: [
                    {
                        title: '品牌',
                        align: 'center',
                        key: 'Brand'
                    },
                    {
                        title: '省份',
                        align: 'center',
                        key: 'ProvinceName'
                    },
                    {
                        title: '城市',
                        align: 'center',
                        key: 'CityName'
                    },
                    {
                        title: '线上展示',
                        align: 'center',
                        render: (h, p) => {
                            var channels = (p.row.Channels || '').split(',');
                            var value = channels.indexOf('online') > -1;
                            console.log(value);
                            return h('Checkbox', {
                                props: {
                                    value: value
                                },
                                on: {
                                    "on-change": (value) => {
                                        var channel = 'online';
                                        var channels = (p.row.Channels || '').split(',');
                                        if (channels.indexOf(channel) > -1) {
                                            channels = channels.filter(x => x !== channel);
                                        } else {
                                            channels[channels.length] = channel;
                                        }
                                        channels = channels.filter(x => !!x);
                                        this.changeChannels(p.row, channels);
                                    }
                                }
                            });
                        }
                    },
                    {
                        title: '线下展示',
                        align: 'center',
                        // width: 150,
                        render: (h, p) => {
                            var channels = (p.row.Channels || '').split(',');
                            var value = channels.indexOf('u门店') > -1;
                            console.log(value);
                            return h('Checkbox', {
                                props: {
                                    value: value
                                },
                                on: {
                                    "on-change": () => {
                                        var channel = 'u门店';
                                        var channels = (p.row.Channels || '').split(',');
                                        if (channels.indexOf(channel) > -1) {
                                            channels = channels.filter(x => x !== channel);
                                        } else {
                                            channels[channels.length] = channel;
                                        }
                                        channels = channels.filter(x => !!x);
                                        this.changeChannels(p.row, channels);
                                    }
                                }
                            });
                        }
                    },
                    {
                        title: '操作',
                        align: 'center',
                        // width: 150,
                        render: (h, p) => {
                            var disabled = !p.row.IsEnabled;
                            return [
                                h('a', {
                                    style: {
                                        "margin-right": "9px"
                                    },
                                    domProps: {
                                        href: 'javascript:void(0)',
                                        innerHTML: '修改'
                                    },
                                    on: {
                                        click: () => {
                                            this.update(p.row);
                                        }
                                    }
                                }),
                                h('a', {
                                    style: {
                                        "margin-right": "9px"
                                    },
                                    domProps: {
                                        href: 'javascript:void(0)',
                                        innerHTML: '删除'
                                    },
                                    on: {
                                        click: () => {
                                            this.delete(p.row.PKID);
                                        }
                                    }
                                }),
                                h('Checkbox', {
                                    props: {
                                        value: disabled
                                    },
                                    on: {
                                        "on-change": () => {
                                            this.changeStatus(p.row);
                                        }
                                    }
                                }, "隐藏")
                            ]
                        }
                    }
                ],
                data: [],
                loading: false,
                selection: []
            },
            modal: {
                title: '添加区域',
                show: false,
                loading: true,
                closable: true,
                formItems: {
                    channels: [],
                    brand: '',
                    cityId: '',
                    provinceId: '',
                    isEnabled: true,
                    callback: null,
                    befaultProvinceId: ''
                },
                cities: []
            },
            page: { total: 0, index: 1, size: 20 },
            brands: [],
            provinces: [],
            cities: [],
            filter: {
                brand: '',
                province: '',
                city: ''
            },
            uploadModal: {
                title: '导入数据',
                show: false,
                loading: true,
                closable: true,
                file: null
            }
        }
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
            var params = {};
            params.Brand = this.filter.brand;
            params.Province = this.filter.province;
            params.City = this.filter.city;
            params.PageIndex = this.page.index;
            params.PageSize = this.page.size;
            this.ajax.get("/BaoYangBattery/GetBaoYangBatteryCoverAreas", {
                params: params
            }).then(response => {
                var res = response.data;
                this.page.total = res.total || 0;
                this.table.data = res.data || [];
                this.table.loading = false;
            })
        },
        lazyLoadData () {
            this.table.loading = true;
            setTimeout(() => {
                this.search();
            }, 1500);
        },
        loadBrands () {
            this.ajax.get('/BaoYangBattery/GetBatteryBrands')
                .then(response => {
                    var res = response.data;
                    this.brands = res || [];
                })
        },
        loadProvince () {
            this.provinces = [];
            this.ajax.get('/BatteryFastDelivery/GetAllProvince')
                .then(response => {
                    var res = response.data;
                    var data = res.Data;
                    var provinces = (data || []).map(x => {
                        var province = {};
                        province.Id = x.ProvinceId;
                        province.Name = x.ProvinceName;
                        return province;
                    });
                    this.provinces = provinces || [];
                })
        },
        getCities (province) {
            this.cities = [];
            if (!province) {
                return;
            }
            this.ajax.get('/BatteryFastDelivery/GetRegionByRegionId', {
                params: {
                    regionId: province
                }
            }).then(response => {
                var res = response.data;
                var region = res.Data || {};
                var cities = [];
                if (region.IsMunicipality) {
                    cities = [{ CityId: region.ProvinceId, CityName: region.ProvinceName }];
                } else {
                    cities = (region.ChildRegions || []).map(x => {
                        var city = {};
                        city.CityId = x.CityId;
                        city.CityName = x.CityName;
                        return city;
                    });
                }
                this.cities = cities;
            })
        },
        getModalCities (province) {
            this.modal.cities = [];
            if (!province) {
                if (this.modal.formItems.callback != null &&
                    this.modal.formItems.callback instanceof Function) {
                    this.modal.formItems.callback();
                    this.modal.formItems.callback = null;
                }
                return;
            }
            this.ajax.get('/BatteryFastDelivery/GetRegionByRegionId', {
                params: {
                    regionId: province
                }
            }).then(response => {
                var res = response.data;
                var region = res.Data || {};
                var cities = [];
                if (region.IsMunicipality) {
                    cities = [{ CityId: region.ProvinceId, CityName: region.ProvinceName }];
                } else {
                    cities = (region.ChildRegions || []).map(x => {
                        var city = {};
                        city.CityId = x.CityId;
                        city.CityName = x.CityName;
                        return city;
                    });
                }
                this.modal.cities = cities;
                if (this.modal.formItems.callback != null &&
                    this.modal.formItems.callback instanceof Function) {
                    this.$nextTick(() => {
                        this.modal.formItems.callback();
                        this.modal.formItems.callback = null;
                    });
                }
            })
        },
        add () {
            this.modal.formItems.pkid = 0;
            this.modal.formItems.cityId = '';
            this.modal.formItems.provinceId = '';
            this.modal.formItems.brand = '';
            this.modal.formItems.disabled = false;
            this.modal.formItems.channels = [];
            this.modal.title = "添加区域";
            this.modal.show = true;
        },
        update (item) {
            this.modal.formItems.pkid = item.PKID;
            this.modal.formItems.provinceId = item.ProvinceId;
            // 异步加载的bugfix
            if (this.modal.formItems.provinceId === this.modal.formItems.befaultProvinceId) {
                this.modal.formItems.cityId = item.CityId;
            } else {
                this.modal.formItems.befaultProvinceId = this.modal.formItems.provinceId;
                this.modal.formItems.callback = () => {
                    this.modal.formItems.cityId = item.CityId;
                }
            }
            this.modal.formItems.brand = item.Brand;
            this.modal.formItems.disabled = !item.IsEnabled;
            this.modal.formItems.channels = (item.Channels || '').split(',');
            this.modal.title = "修改区域";
            this.modal.show = true;
        },
        submit () {
            var item = {};
            item.PKID = this.modal.formItems.pkid || 0;
            item.CityId = this.modal.formItems.cityId || 0;
            item.ProvinceId = this.modal.formItems.provinceId || 0;
            item.Brand = this.modal.formItems.brand;
            item.IsEnabled = !this.modal.formItems.disabled;
            item.Channels = (this.modal.formItems.channels || []).join(',');
            if (item.CityId <= 0 || item.ProvinceId <= 0 || !item.Brand) {
                this.$Message.warning("品牌地区不能为空");
                this.modal.loading = false;
                this.$nextTick(() => {
                    this.modal.loading = true;
                })
                return;
            }
            var content = item.PKID > 0 ? "确认修改配置?" : "确认添加配置?";
            this.$Modal.confirm({
                title: "温馨提示",
                content: content,
                loading: true,
                onOk: () => {
                    this.ajax.post("/BaoYangBattery/AddOrUpdateBaoYangBatteryCoverArea", {
                        ...item
                    }).then(response => {
                        var res = response.data;
                        if (res.status) {
                            this.$Message.info("操作成功");
                        } else {
                            this.$Message.error("操作失败!" + (res.msg || ''))
                        }
                        this.$Modal.remove();
                        this.modal.loading = false;
                        this.$nextTick(function () {
                            this.modal.loading = true;
                        })
                        if (res.status) {
                            this.modal.show = false;
                            this.lazyLoadData();
                        }
                    })
                },
                onCancel: () => {
                    this.$Modal.remove();
                    this.modal.loading = false;
                    this.$nextTick(function () {
                        this.modal.loading = true;
                    })
                }
            });
        },
        delete (id) {
            this.$Modal.confirm({
                title: "温馨提示",
                content: "确认删除覆盖区域配置吗?",
                loading: true,
                onOk: () => {
                    this.ajax.post("/BaoYangBattery/DeleteBaoYangBatteryCoverArea", {
                        id: id
                    }).then(response => {
                        var res = response.data;
                        if (res.status) {
                            this.$Message.info("操作成功");
                        } else {
                            this.$Message.error("操作失败!" + (res.msg || ''))
                        }
                        this.$Modal.remove();
                        if (res.status) {
                            this.lazyLoadData();
                        }
                    })
                }
            });
        },
        changeStatus (input) {
            var item = {};
            item.PKID = input.PKID;
            item.CityId = input.CityId;
            item.ProvinceId = input.ProvinceId;
            item.Brand = input.Brand;
            item.IsEnabled = !input.IsEnabled;
            item.Channels = input.Channels;
            this.ajax.post("/BaoYangBattery/AddOrUpdateBaoYangBatteryCoverArea", {
                ...item
            }).then(response => {
                var res = response.data;
                input.IsEnabled = item.IsEnabled;
                if (!res.status) {
                    this.$Message.error("操作失败!" + (res.msg || ''))
                    this.$nextTick(() => {
                        input.IsEnabled = !input.IsEnabled;
                    })
                }
            })
        },
        changeChannels (input, channels) {
            var item = {};
            item.PKID = input.PKID;
            item.CityId = input.CityId;
            item.ProvinceId = input.ProvinceId;
            item.Brand = input.Brand;
            item.IsEnabled = input.IsEnabled;
            item.Channels = channels.join(',');
            this.ajax.post("/BaoYangBattery/AddOrUpdateBaoYangBatteryCoverArea", {
                ...item
            }).then(response => {
                var res = response.data;
                var temp = input.Channels;
                input.Channels = item.Channels;
                if (!res.status) {
                    this.$Message.error("操作失败!" + (res.msg || ''))
                    this.$nextTick(() => {
                        input.Channels = temp;
                    })
                }
            })
        },
        downloadTmpl () {
            window.location.href = '/BaoYangBattery/DownloadBaoYangBatteryTmpl';
        },
        uploadExcel () {
            this.uploadModal.show = true;
        },
        handleUpload (file) {
            this.uploadModal.file = file;
            return false;
        },
        exportExcel () {
            this.$Modal.confirm({
                title: "温馨提示",
                content: "是否导出数据?",
                loading: true,
                onOk: () => {
                    window.location.href = "/BaoYangBattery/ExportBaoYangBatteryCoverArea";
                    this.$Modal.remove();
                }
            });
        },
        handleError () {

        },
        handleSuccess (response) {
            this.uploadModal.loading = false;
            this.$nextTick(() => {
                this.uploadModal.loading = true;
            });
            if (response.status) {
                this.$Message.info("上传成功!")
                this.uploadModal.file = null;
                this.uploadModal.show = false;
                this.lazyLoadData();
            } else {
                this.$Message.warning("上传失败!" + (response.msg || ""));
            }
        },
        submitUpload () {
            this.$Modal.confirm({
                title: "温馨提示",
                content: "确认上传该文件?",
                loading: true,
                onOk: () => {
                    this.$refs.upload.post(this.uploadModal.file)
                    this.$Modal.remove();
                }
            });
        }
    },
    mounted () {
        this.loadBrands();
        this.loadProvince();
        this.loadData();
        this.$Message.config({
            duration: 5
        });
    }
}
</script>

<style>
.filter-element {
  width: 70%;
}
</style>
