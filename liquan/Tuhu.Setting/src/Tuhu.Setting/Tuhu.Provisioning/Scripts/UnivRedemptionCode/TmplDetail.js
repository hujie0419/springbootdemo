(function () {
    Array.prototype.all = function (callback) {
        var array = this;
        for (var i = 0; i < array.length; i++) {
            var value = array[i];
            if (!callback(value, i)) {
                return false;
            }
        }
        return true;
    }
    Array.prototype.any = function (callback) {
        var array = this;
        for (var i = 0; i < array.length; i++) {
            var value = array[i];
            if (callback(value, i)) {
                return true;
            }
        }
        return false;
    }
    Array.prototype.firstOrDefault = function (callback) {
        var array = this;
        for (var i = 0; i < array.length; i++) {
            var value = array[i];
            if (callback(value, i)) {
                return value;
            }
        }
        return undefined;
    }
    Array.prototype.removeAll = function (callback) {
        var array = this;
        var count = array.length;
        var index = 0;
        while (index < count) {
            var value = array[index];
            if (callback(value, index)) {
                array.splice(index, 1);
                count--;
            }
            else {
                index++;
            }
        }
    }
    Date.prototype.formatDate = function () {
        var time = this;
        var year = time.getFullYear();
        var month = time.getMonth() + 1;
        var days = time.getDate();
        var hours = time.getHours();
        var minutes = time.getMinutes();
        var seconds = time.getSeconds();
        var milliseconds = time.getMilliseconds();
        var func = function (input, n) {
            var text = (input || "").toString();
            while (text.length < n) {
                text = '0' + text;
            }
            return text;
        }
        return func(year, 4) + '-' +
            func(month, 2) + '-' +
            func(days, 2) + ' ' +
            func(hours, 2) + ':' +
            func(minutes, 2) + ':' +
            func(seconds, 2) + '.' + func(milliseconds, 3);
    }

    Vue.component('city-list', {
        props: {
            value: {
                type: Object,
                default: [],
            },
            province_id: {
                type: Number,
                default: 1,
            },
            provinces: {
                type: Array,
                default: [],
            }
        },
        template: "#city-list",
        data: function () {
            return {
                cities: [],
            }
        },
        mounted: function () {
            var vm = this;
            if (vm.province_id > 0) {
                this.getCities();
            }
        },
        watch: {
            "province_id": function (value) {
                var vm = this;
                vm.value.removeAll(function (cityId) {
                    return vm.cities.any(function (city) {
                        return cityId === city.RegionId;
                    })
                });
                if (value > 0) {
                    vm.getCities();
                }
                else {
                    vm.cities = [];
                }
                vm.$emit('update:province_id', value);
            },
            "value": function (value) {
                var vm = this;
                vm.$emit('input', value);
            }
        },
        methods: {
            remove: function () {
                var vm = this;
                vm.value.removeAll(function (cityId) {
                    return vm.cities.any(function (city) {
                        return cityId === city.RegionId;
                    })
                });
                vm.$emit('on-remove', vm.province_id);
            },
            getCities: function () {
                var vm = this;
                $.get("/BankMRActivity/GetRegionByRegionId", { regionId: vm.province_id }, function (region) {
                    vm.cities = region.ChildRegions.map(function (c) {
                        return {
                            RegionId: c.RegionId,
                            RegionName: c.RegionName
                        }
                    }) || [];
                });
            },
        }

    });
    Vue.component('region-list', {
        props: {
            province_ids: {
                type: Array,
                default: [],
            },
            city_ids: {
                type: Array,
                default: [],
            }
        },
        template: "#region-list",
        data: function () {
            return {
                provinces: [],
            }
        },
        watch: {
            province_ids: function (value) {
                var vm = this;
                vm.$emit('update:province_ids', value);
            },
            city_ids: function (value) {
                var vm = this;
                vm.$emit('update:city_ids', value);
            }
        },
        mounted: function () {
            this.getAllProvince();
        },
        methods: {
            clickAppend: function () {
                var vm = this;
                if (vm.provinces && vm.provinces.length > 0) {
                    var first = vm.provinces.firstOrDefault(function (province) {
                        return vm.province_ids.all(function (provinceId) {
                            return provinceId != province.RegionId;
                        });
                    });
                    if (first) {
                        vm.province_ids.push(first.RegionId);
                    }
                    else {
                        vm.$Message.warning("所有的都已经加入到列表中");
                    }
                }
            },
            clickRemove: function (index) {
                var vm = this;
                vm.province_ids.splice(index, 1);
            },
            getAllProvince: function () {
                var vm = this;
                $.get("/BankMRActivity/GetAllProvince", function (region) {
                    vm.provinces = region;
                })
            }
        }
    });
    Vue.component('limit-modal', {
        props: {
            value: {
                type: Boolean,
                default: false,
            },
            id: {
                type: Number,
                default: 0,
            }
        },
        template: "#limit-modal",
        data: function () {
            return {
                limit: {
                    PKID: 0,
                    MrCodeConfigId: 0,
                    CycleType: "Year",
                    CycleLimit: 0,
                    ProvinceIds: [],
                    CityIds: [],
                },
                showModal: false,
                loading: true,
            }
        },
        created: function () {
            var vm = this;
        },
        watch: {
            value: function (value) {
                var vm = this;
                if (value) {
                    vm.getRedeemMrCodeLimitConfig();
                }
                else {
                    vm.showModal = false;
                }
                vm.$emit('input', value);
            },
            showModal: function (value) {
                var vm = this;
                if (!value) {
                    vm.value = false;
                }
            }
        },
        methods: {
            getRedeemMrCodeLimitConfig: function () {
                var vm = this;
                $.get("GetRedeemMrCodeLimitConfig", { mrCodeConfigId: vm.id }, function (rsp) {
                    var limt = rsp.data;
                    if (limt) {
                        vm.limit = {
                            PKID: limt.PKID,
                            MrCodeConfigId: limt.MrCodeConfigId,
                            CycleType: limt.CycleType,
                            CityIds: (limt.CityIds || "").split(',')
                                .map(function (x) { return parseInt(x); })
                                .filter(function (x) { return !isNaN(x) && x > 0 }),
                            ProvinceIds: (limt.ProvinceIds || "").split(',')
                                .map(function (x) { return parseInt(x); })
                                .filter(function (x) { return !isNaN(x) && x > 0 }),
                            CycleLimit: limt.CycleLimit,
                        };
                    }
                    else {
                        vm.limit = {
                            PKID: 0,
                            MrCodeConfigId: vm.id,
                            CycleType: "Year",
                            CycleLimit: 0,
                            ProvinceIds: [],
                            CityIds: [],
                        }
                    }
                    vm.showModal = true;
                });
            },
            submit: function () {
                var vm = this;
                var data = {
                    PKID: vm.limit.PKID,
                    MrCodeConfigId: vm.limit.MrCodeConfigId,
                    CycleType: vm.limit.CycleType,
                    CycleLimit: vm.limit.CycleLimit,
                    ProvinceIds: vm.limit.ProvinceIds.join(','),
                    CityIds: vm.limit.CityIds.join(','),
                }
                vm.$Modal.confirm({
                    title: "确认修改限购配置?",
                    content: "确认修改限购配置?",
                    loading: true,
                    onOk: function () {
                        $.post("UpdateRedeemMrCodeLimitConfig", data, function (result) {
                            if (result.status) {
                                vm.$Message.info('操作成功！');
                                vm.value = false;
                            }
                            else {
                                vm.$Message.warning('操作失败！' + (result.msg || ''));
                            }
                        }).error(function () {
                            vm.$Message.warning('操作失败！');
                        }).complete(function () {
                            vm.$Modal.remove();
                            vm.loading = false;
                            vm.$nextTick(function () {
                                vm.loading = true;
                            })
                        });
                    },
                    onCancel: function () {
                        vm.loading = false;
                        vm.$nextTick(function () {
                            vm.loading = true;
                        })
                    }
                });
            },
        }
    });
    Vue.component('coupon-select', {
        props: {
            business_type: {
                type: String,
                default: "None",
            },
            get_rule_guid: {
                type: String,
                default: "00000000-0000-0000-0000-000000000000",
            },
            package_id: {
                type: Number,
                default: 0,
            },
            settlement_price: {
                type: Number,
                default: 0.0
            },    
        },
        template: "#coupon-select",
        data: function () {
            return {
                coupons: [],
                packages: [],
                labelText: "",
                isGeneral: true,
                packageId: 0,
                businessType: 'None',
                getRuleGuid: '00000000-0000-0000-0000-000000000000',
                settlementPrice: 0.0,
            };
        },
        created: function () {
            var vm = this;
            vm.businessType = vm.business_type;
            vm.getRuleGuid = vm.get_rule_guid;
            vm.settlementPrice = vm.settlement_price;
            vm.packageId = vm.package_id;
            vm.convertText(vm.businessType);
            vm.getCoupons();
            vm.$watch('businessType', function (value) {
                var vm = this;
                vm.getCoupons();
                vm.packages = [];
                vm.packageId = 0;
                vm.getRuleGuid = "00000000-0000-0000-0000-000000000000";
                vm.convertText(value);
                console.log(value);
                vm.$emit('update:business_type', value);
            });
            vm.$watch('packageId', function (value) {
                var vm = this;
                var package = vm.packages.firstOrDefault(function (x) { return x.PackageId === value });
                if (package) {
                    vm.settlementPrice = package.Price;
                }
                else {
                    vm.settlementPrice = 0.0;
                }
                vm.$emit('update:settlement_price', vm.settlementPrice);
                console.log(value);
                vm.$emit('update:package_id', value);
            });
        },
        watch: {
            business_type: function (value) {
                var vm = this;
                vm.businessType = value;
            },
            get_rule_guid: function (value) {
                var vm = this;
                vm.getRuleGuid = value;
            },
            package_id: function (value) {
                var vm = this;
                vm.packageId = value;
            },
            settlement_price: function (value) {
                var vm = this;
                vm.settlementPrice = value;
            },
            getRuleGuid: function (value) {
                var vm = this;
                console.log(value);
                vm.$emit('update:get_rule_guid', value);
            },
            packages: function () {
                var vm = this;
                var value = vm.packageId;
                var package = vm.packages.firstOrDefault(function (x) { return x.PackageId === value });
                if (package) {
                    vm.settlementPrice = package.Price;
                }
                else {
                    vm.settlementPrice = 0.0;
                }
                vm.$emit('update:settlement_price', vm.settlementPrice);
            },
            settlementPrice: function (value) {
                var vm = this;
                vm.$emit('update:settlement_price', value);
            },
        },
        methods: {
            getCoupons: function () {
                var vm = this;
                if (vm.businessType == 'None' || !vm.businessType) {
                    vm.coupons = [];
                    vm.getRuleGuid = vm.get_rule_guid;
                    return;
                }
                $.get('GetCouponBusinessConfigs', { businessType: vm.businessType }, function (rsp) {
                    var data = rsp.data || [];
                    var coupons = data.map(function (x) {
                        return { GetRuleGUID: (x.GetRuleGuid || "").toLowerCase(), Description: x.Description };
                    });
                    vm.coupons = coupons;
                }).error(function () {
                    vm.coupons = [];
                }).complete(function () {
                    vm.getRuleGuid = vm.get_rule_guid;
                });
            },
            getBaoYangPackages: function () {
                var vm = this;
                $.get('GetBaoYangPackages', { configId: GlobalConstantConfig.ConfigId }, function (rsp) {
                    var data = rsp.data || [];
                    var packages = data.map(function (x) {
                        return { PackageId: x.PackageId, PackageName: x.PackageName, Price: x.Price };
                    });
                    vm.packages = packages;
                }).error(function () {
                    vm.packages = [];
                }).complete(function () {
                    vm.packageId = vm.package_id;
                });
            },
            getPaintPackages: function () {
                var vm = this;
                $.get('GetPaintPackages', { configId: GlobalConstantConfig.ConfigId }, function (rsp) {
                    var data = rsp.data || [];
                    var packages = data.map(function (x) {
                        return { PackageId: x.PackageId, PackageName: x.PackageName, Price: x.Price };
                    });
                    vm.packages = packages;
                }).error(function () {
                    vm.packages = [];
                }).complete(function () {
                    vm.packageId = vm.package_id;
                });
            },
            convertText: function (value) {
                var vm = this;
                var text = '', isGeneral = true;
                switch (value) {
                    case "GeneralBaoYang":
                        isGeneral = true;
                        break;
                    case "BaoYangPackage":
                        text = "保养";
                        isGeneral = false;
                        this.getBaoYangPackages();
                        break;
                    case "GeneralPaint":
                        isGeneral = true;
                        break;
                    case "PaintPackage":
                        text = "喷漆";
                        isGeneral = false;
                        this.getPaintPackages();
                        break;
                    case "GeneralAnnualInspection":
                        isGeneral = true;
                        break;
                    case "AnnualInspectionPackage":
                        text = "年检";
                        isGeneral = false;
                        break;
                }
                vm.labelText = text;
                vm.isGeneral = isGeneral;
            }
        }
    });
    Vue.component('tab-content-coupon', {
        template: '#tab-content-coupon',
        data: function () {
            return {
                showModal: false,
                submitLoading: true,
                loading: false,
                columns: [],
                list: [],
                disabledSettlementMethod: false,
                data: {
                    PKID: 0,
                    RedemptionConfigId: GlobalConstantConfig.ConfigId,
                    Name: "",
                    GetRuleGuid: "00000000-0000-0000-0000-000000000000",
                    BusinessType: "",
                    PackageId: 0,
                    Num: 0,
                    SettlementMethod: "None",
                    SettlementPrice: 0.0,
                    IsRequired: true,
                    IsActive: true,
                    Description: ""
                },
            };
        },
        created: function () {
            var vm = this;
            vm.disabledSettlementMethod = GlobalConstantConfig.SettlementMethod === "BatchPreSettled";
            vm.settlement_price = GlobalConstantConfig.SettlementMethod;
            vm.columns = [
                { title: 'ID', key: 'PKID', width: 60, align: 'center' },
                { title: '优惠券名称', key: 'Name', align: 'center' },
                {
                    title: '优惠券类型', key: 'BusinessType', align: 'center',
                    render: function (h, p) {
                        var type = p.row.BusinessType;
                        var text = '';
                        switch (type) {
                            case "GeneralBaoYang":
                                text = "通用保养流程";
                                break;
                            case "BaoYangPackage":
                                text = "保养套餐流程";
                                break;
                            case "GeneralPaint":
                                text = "通用喷漆流程";
                                break;
                            case "PaintPackage":
                                text = "喷漆套餐流程";
                                break;
                            case "GeneralAnnualInspection":
                                text = "通用年检流程";
                                break;
                            case "AnnualInspectionPackage":
                                text = "年检套餐流程";
                                break;
                        }
                        return h('span', text);
                    }
                },
                { title: '结算价', key: 'SettlementPrice', align: 'center' },
                {
                    title: '结算类型', key: 'SettlementMethod', align: 'center',
                    render: function (h, p) {
                        var type = (p.row.SettlementMethod || '');
                        var text = '';
                        switch (type) {
                            case "BatchPreSettled":
                                text = "大买断结算";
                                break;
                            case "SinglePreSettled":
                                text = "小买断结算";
                                break;
                            case "ByPeriod":
                                text = "据实结算";
                                break;
                        }
                        return h('span', text);
                    }
                },
                {
                    title: '操作', align: 'center', width: 200,
                    render: function (h, p) {
                        return h('div', [
                            h('Button', {
                                on: {
                                    click: function () {
                                        vm.edit(p.row.PKID);
                                    }
                                }
                            }, '编辑'),
                            h('Button', {
                                on: {
                                    click: function () {
                                        vm.delete(p.row.PKID);
                                    }
                                }
                            }, '删除'),
                        ]);
                    }
                }
            ];
            vm.getRedeemPromotionConfigs();
        },
        watch: {

        },
        methods: {
            getRedeemPromotionConfigs: function () {
                var vm = this;
                $.get("GetRedeemPromotionConfigs", { configId: GlobalConstantConfig.ConfigId }, function (rsp) {
                    var list = rsp.data || [];
                    vm.list = list;
                });
            },
            submit: function () {
                var vm = this;
                vm.$Modal.confirm({
                    title: "确认添加该优惠券配置?",
                    content: "确认添加该优惠券配置?",
                    loading: true,
                    onOk: function () {
                        $.post("AddOrUpdateRedeemPromotionConfig", vm.data, function (data) {
                            if (data.status) {
                                vm.$Message.info('操作成功！');
                                vm.getRedeemPromotionConfigs();
                                vm.showModal = false;
                            } else {
                                vm.$Message.warning('操作失败！' + (data.msg || ""));
                            }
                        }).error(function () {
                            vm.$Message.warning('操作失败！');
                        }).complete(function () {
                            vm.$Modal.remove();
                            vm.submitLoading = false;
                            vm.$nextTick(function () {
                                vm.submitLoading = true;
                            });
                        });
                    }
                });
            },
            append: function () {
                var vm = this;
                vm.data = {
                    PKID: 0,
                    RedemptionConfigId: GlobalConstantConfig.ConfigId,
                    Name: "",
                    GetRuleGuid: "00000000-0000-0000-0000-000000000000",
                    BusinessType: "None",
                    PackageId: 0,
                    Num: 0,
                    SettlementMethod: "None",
                    SettlementPrice: 0.0,
                    IsRequired: true,
                    IsActive: true,
                    Description: ""
                };
                vm.showModal = true;
            },
            edit: function (id) {
                var vm = this;
                $.get("GetRedeemPromotionConfig", { id: id }, function (rsp) {
                    var config = rsp.data;
                    if (config) {
                        vm.data.PKID = config.PKID;
                        vm.data.RedemptionConfigId = config.RedemptionConfigId;
                        vm.data.Name = config.Name;
                        vm.data.GetRuleGuid = (config.GetRuleGuid || "00000000-0000-0000-0000-000000000000").toLowerCase();
                        vm.data.BusinessType = config.BusinessType;
                        vm.data.PackageId = config.PackageId;
                        vm.data.Num = config.Num;
                        vm.data.SettlementMethod = config.SettlementMethod;
                        vm.data.SettlementPrice = config.SettlementPrice;
                        vm.data.IsRequired = config.IsRequired;
                        vm.data.IsActive = config.IsActive;
                        vm.data.Description = config.Description;
                        console.log(vm.data.GetRuleGuid)
                        vm.showModal = true;
                    }
                    else {
                        vm.$Message.warning("该配置不存在或者已经被删除！");
                        vm.getRedeemPromotionConfigs();
                    }
                });
            },
            delete: function (id) {
                var vm = this;
                vm.$Modal.confirm({
                    title: "确认删除该优惠券配置?",
                    content: "确认删除该优惠券配置?",
                    loading: true,
                    onOk: function () {
                        $.post("DeleteRedeemPromotionConfig", { id: id }, function (data) {
                            if (data.status) {
                                vm.$Message.info('操作成功！');
                                vm.getRedeemPromotionConfigs();
                            } else {
                                vm.$Message.warning('操作失败！' + (data.msg || ""));
                            }
                        }).error(function () {
                            vm.$Message.warning('操作失败！');
                        }).complete(function () {
                            vm.$Modal.remove();
                        });
                    }
                });
            },
        }
    });
    Vue.component('tab-content-service', {
        props: {
            services: {
                type: Array,
                default: [],
            },
        },
        template: '#tab-content-service',
        data: function () {
            return {
                loading: false,
                columns: [],
                data: [],
                showModal: false,
                submitLoading: true,
                serviceConfig: {
                    PKID: 0,
                    RedemptionConfigId: GlobalConstantConfig.ConfigId,
                    Name: "",
                    SettlementMethod: "",
                    SettlementPrice: 0.0,
                    ShopCommission: 0.00,
                    Num: 1,
                    DateRange: [],
                    EffectiveDay: "",
                    CodeTypeConfigId: 0,
                    IsRequired: true,
                    IsActive: true,
                    Description: "",
                },
                showLimitModal: false,
                mrCodeConfigId: 0,
            };
        },
        created: function () {
            var vm = this;
            vm.columns = [
                { title: 'ID', key: 'PKID', width: 60, align: 'center' },
                { title: '服务码名', key: 'Name', align: 'center' },
                { title: '结算价', key: 'SettlementPrice', align: 'center' },
                { title: '佣金', key: 'ShopCommission', align: 'center' },
                { title: '数量', key: 'Num', align: 'center' },
                {
                    title: '有效期／兑换后有效期（天)', align: 'center',
                    render: function (h, p) {
                        if (p.row.StartTime && p.row.EndTime) {
                            return h('div', p.row.StartTime + '-' + p.row.EndTime);
                        }
                        return h('div', p.row.EffectiveDay);
                    }
                },
                { title: '服务', key: 'ServiceName', align: 'center' },
                {
                    title: '结算方式', align: 'center',
                    render: function (h, p) {
                        var method = p.row.SettlementMethod;
                        var text = '';
                        switch (method) {
                            case 'SinglePreSettled':
                                text = '小买断结算';
                                break;
                            case 'ByPeriod':
                                text = '据实';
                                break;
                            case 'BatchPreSettled':
                                text = '大买断结算';
                                break;
                        }
                        return h('div', text);
                    }
                },
                {
                    title: '操作',
                    align: 'left',
                    width: 200,
                    render: function (createElement, params) {
                        return createElement('div', [
                            createElement('Button', {
                                on: {
                                    click: function () {
                                        vm.edit(params.row.PKID);
                                    }
                                }
                            }, '编辑'),
                            createElement('Button', {
                                on: {
                                    click: function () {
                                        vm.delete(params.row.PKID);
                                    }
                                }
                            }, '删除'),
                            createElement('Button', {
                                on: {
                                    click: function () {
                                        vm.mrCodeConfigId = params.row.PKID;
                                        vm.showLimitModal = true;
                                    }
                                }
                            }, '限购'),
                        ]);
                    }
                },
            ]
            vm.getRedeemMrCodeConfigs();
            vm.$Message.config({
                top: 50,
                duration: 3
            });
        },
        methods: {
            append: function () {
                var vm = this;
                vm.serviceConfig = {
                    PKID: 0,
                    RedemptionConfigId: GlobalConstantConfig.ConfigId,
                    Name: "",
                    SettlementMethod: "",
                    SettlementPrice: 0.0,
                    ShopCommission: 0.00,
                    Num: 1,
                    DateRange: [],
                    EffectiveDay: "",
                    CodeTypeConfigId: 0,
                    IsRequired: true,
                    IsActive: true,
                    Description: "",
                }
                vm.showModal = true;
            },
            edit: function (id) {
                var vm = this;
                $.get("GetRedeemMrCodeConfig", { redeemMrCodeConfigId: id }, function (result) {
                    var data = result.data;
                    if (data) {
                        vm.serviceConfig = {
                            PKID: data.PKID,
                            RedemptionConfigId: data.ConfigId,
                            Name: data.Name,
                            SettlementMethod: data.SettlementMethod,
                            SettlementPrice: data.SettlementPrice,
                            ShopCommission: data.ShopCommission,
                            Num: data.Num,
                            DateRange: [data.StartTime, data.EndTime],
                            //StartTime: data.StartTime,
                            //EndTime: data.EndTime,
                            EffectiveDay: data.EffectiveDay,
                            CodeTypeConfigId: data.CodeTypeConfigId,
                            IsRequired: data.IsRequired,
                            IsActive: data.IsActive,
                            Description: data.Description,
                        };
                    }
                    else {
                        vm.serviceConfig = {
                            PKID: 0,
                            RedemptionConfigId: GlobalConstantConfig.ConfigId,
                            Name: "",
                            SettlementMethod: "",
                            SettlementPrice: 0.0,
                            ShopCommission: 0.00,
                            Num: 1,
                            DateRange: [],
                            EffectiveDay: "",
                            CodeTypeConfigId: 0,
                            IsRequired: true,
                            IsActive: true,
                            Description: "",
                        }
                    }
                    vm.showModal = true;
                });
            },
            submit: function () {
                var vm = this;
                var data = {
                    PKID: vm.serviceConfig.PKID,
                    RedemptionConfigId: GlobalConstantConfig.ConfigId,
                    Name: vm.serviceConfig.Name,
                    SettlementMethod: vm.serviceConfig.SettlementMethod,
                    SettlementPrice: vm.serviceConfig.SettlementPrice,
                    ShopCommission: vm.serviceConfig.ShopCommission,
                    Num: vm.serviceConfig.Num,
                    CodeTypeConfigId: vm.serviceConfig.CodeTypeConfigId,
                    IsRequired: vm.serviceConfig.IsRequired,
                    IsActive: vm.serviceConfig.IsActive,
                    Description: vm.serviceConfig.Description,
                };
                if (vm.serviceConfig.DateRange && vm.serviceConfig.DateRange.length === 2
                    && vm.serviceConfig.DateRange[0] && vm.serviceConfig.DateRange[1]) {
                    data.StartTime = vm.serviceConfig.DateRange[0].formatDate();
                    data.EndTime = vm.serviceConfig.DateRange[1].formatDate();
                }
                if (vm.serviceConfig.EffectiveDay) {
                    data.EffectiveDay = vm.serviceConfig.EffectiveDay;
                }
                console.log(data);
                //Validated
                vm.$Modal.confirm({
                    title: "确认此操作?",
                    content: "确认添加此配置模板?",
                    onOk: function () {
                        $.post("AddOrUpdateRedeemMrCodeConfig", data, function (data) {
                            if (data.status) {
                                vm.showModal = false;
                                vm.$Message.info('操作成功！');
                                vm.getRedeemMrCodeConfigs();
                            } else {
                                vm.$Message.warning('操作失败！' + (data.msg || ""));
                            }
                        }).error(function () {
                            vm.$Message.info('操作失败！');
                        }).complete(function () {
                            vm.submitLoading = false;
                            vm.$nextTick(function () {
                                vm.submitLoading = true;
                            });
                        });
                    },
                    onCancel: function () {
                        vm.submitLoading = false;
                        vm.$nextTick(function () {
                            vm.submitLoading = true;
                        });
                    }
                });
            },
            delete: function (id) {
                var vm = this;
                vm.$Modal.confirm({
                    title: '确认删除？',
                    content: '<p>确认删除该配置？</p>',
                    loading: true,
                    onOk: function () {
                        $.post("DeleteRedeemMrCodeConfig", { id: id }, function (result) {
                            if (result.status) {
                                vm.$Message.info('删除成功！')
                                vm.getRedeemMrCodeConfigs();
                            }
                            else {
                                vm.$Message.warning("删除失败!" + (result.msg || ""));
                            }
                        }).complete(function () {
                            vm.$Modal.remove();
                        });
                    }
                });
            },
            limit: function (id) {
                var vm = this;
                $.get("GetRedeemMrCodeLimitConfig", { mrCodeConfigId: id }, function (rsp) {
                    var limitConfig = vm.data || {};
                    vm.limitConfig = limitConfig;
                });
            },
            getRedeemMrCodeConfigs: function () {
                var vm = this;
                var configId = GlobalConstantConfig.ConfigId;
                $.get("GetRedeemMrCodeConfigs", { configId: configId }, function (result) {
                    var data = result.data || [];
                    vm.data = data;
                });
            }
        }
    });
})();
