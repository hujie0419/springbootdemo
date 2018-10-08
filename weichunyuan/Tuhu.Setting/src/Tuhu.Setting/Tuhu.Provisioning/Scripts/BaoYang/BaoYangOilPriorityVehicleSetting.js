

(function () {
    "use strict"
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
        if (array && array.length > 0) {
            if (!callback) {
                return true;
            }
            for (var i = 0; i < array.length; i++) {
                var value = array[i];
                if (callback(value, i)) {
                    return true;
                }
            }
        }
        return false;
    }
})();

var init = function (viscosities) {

    Vue.component('oil-select-list', {
        template: '#oil-select-list',
        props: {
            value: {
                type: Array,
                default: function () { return [] },
            }
        },
        data: function () {
            return {
                list: [],
                listJSON: '[]',
                valueJSON: '',
                id: 0,
                brand_series: {},
            };
        },
        mounted: function () {
            var vm = this;
            vm.getOilBrandAndSeries();
        },
        watch: {
            list: {
                handler: function (value) {
                    var vm = this;
                    var list = (value || []).map(function (item) {
                        return { grade: item.grade, brand: item.brand, series: item.series };
                    });
                    vm.listJSON = JSON.stringify(list);
                },
                deep: true,
            },
            listJSON: function (value) {
                var vm = this;
                var list = JSON.parse(value) || [];
                var settings = list.map(function (item, index) {
                    var obj = {};
                    obj.Priority = index + 1;
                    obj.PriorityType = item.grade;
                    obj.Brand = item.brand;
                    obj.Series = item.series;
                    return obj;
                });
                vm.$emit('input', settings);
            },
            value: function (value) {
                var vm = this;
                var settings = (value || []).map(function (setting) {
                    return setting;
                }).sort(function (x, y) {
                    return x.Priority - y.Priority;
                });
                var list = [];
                settings.forEach(function (setting) {
                    var obj = {};
                    obj.grade = setting.PriorityType;
                    obj.brand = setting.Brand;
                    obj.series = setting.Series;
                    list.push(obj);
                });
                vm.valueJSON = JSON.stringify(list);
            },
            valueJSON: function (value) {
                var vm = this;
                if (value != vm.listJSON) {
                    var list = [];
                    var temp = JSON.parse(value) || [];
                    temp.forEach(function (item) {
                        vm.id += 1;
                        item.key = vm.id;
                        list.push(item);
                    });
                    vm.list = list;
                }
                if ((vm.list || []).length <= 0) {
                    vm.id += 1;
                    vm.list = [{ grade: '', brand: '', series: '', key: vm.id }];
                }
            }
        },
        methods: {
            append: function () {
                var vm = this;
                vm.id += 1;
                var priority = vm.list.length + 1;
                vm.list.push({ grade: '', brand: '', series: '', key: vm.id });
            },
            remove: function (index) {
                var vm = this;
                vm.list.splice(index, 1);
            },
            getOilBrandAndSeries: function () {
                var vm = this;
                $.get("GetOilBrandAndSeries").done(function (res) {
                    vm.brand_series = res.data || {};
                });
            }
        },
        components: {
            'oil-select': {
                template: '#oil-select',
                props: {
                    grade: {
                        type: String,
                        default: '',
                    },
                    brand: {
                        type: String,
                        default: '',
                    },
                    series: {
                        type: String,
                        default: '',
                    },
                    brand_series: {
                        type: Object,
                        default: function () {
                            return {}
                        },
                    },
                    settings: {
                        type: Array,
                        default: function () {
                            return []
                        }
                    },
                    priority: {
                        type: Number,
                        default: 1,
                    }
                },
                data: function () {
                    return {
                        v_grade: '',
                        v_brand: '',
                        v_series: '',
                    };
                },
                mounted: function () {
                    var vm = this;
                    vm.v_grade = vm.grade || '';
                    vm.v_brand = vm.brand || '';
                    vm.v_series = vm.series || '';
                    //初始化之后再watch
                    vm.$watch('v_grade', function (val) {
                        var vm = this;
                        console.log(val);
                        vm.v_brand = '';
                        vm.$emit('update:grade', val);
                    });
                    vm.$watch('v_brand', function (val) {
                        var vm = this;
                        console.log(val);
                        vm.v_series = '';
                        vm.$emit('update:brand', val);
                    });
                    vm.$watch('v_series', function (val) {
                        var vm = this;
                        console.log(val);
                        vm.$emit('update:series', val);
                    });
                },
                watch: {
                    grade: function (value) {
                        var vm = this;
                        vm.v_grade = value;
                    },
                    brand: function (value) {
                        var vm = this;
                        vm.v_brand = value;
                    },
                    series: function (value) {
                        var vm = this;
                        vm.v_series = value;
                    }
                },
                computed: {
                    seriesList: function () {
                        var vm = this;
                        var brand = vm.v_brand;
                        return vm.brand_series[brand] || [];
                    },
                    brandList: function () {
                        var vm = this;
                        return Object.keys(vm.brand_series);
                    }
                }
            }
        },
    });

    Vue.component('modiffy-priority-modal', {
        template: '#modiffy-priority-modal',
        props: {
            vehicleid: {
                type: String,
                default: '',
            },
            partname: {
                type: String,
                default: '',
            },
            value: {
                type: Boolean,
                default: false,
            }
        },
        data: function () {
            return {
                settings: [],
                modal: false,
                loading: true,
                isEnabled: true,
            };
        },
        mounted: function () {
            var vm = this;
            vm.modal = vm.show;
        },
        watch: {
            value: function (val) {
                var vm = this;
                if (val && vm.vehicleid) {
                    vm.getSettings(vm.vehicleid, vm.partname);
                } else {
                    vm.settings = [];
                    vm.modal = val;
                }
            },
            modal: function (val) {
                var vm = this;
                if (val !== vm.value) {
                    vm.$emit('input', val);
                }
            },
        },
        methods: {
            getSettings: function (vehicleId, partName) {
                var vm = this;
                $.get('GetPriorityVehicleSettings', {
                    vehicleId: vehicleId,
                    partName: partName
                }).done(function (res) {
                    var settings = res.data;
                    if (settings && settings.length > 0) {
                        vm.isEnabled = settings[0].IsEnabled;
                    } else {
                        vm.isEnabled = true;
                    }
                    vm.settings = settings || [];
                    vm.modal = true;
                });
            },
            submit: function () {
                var vm = this;
                var settings = (vm.settings || []).map(function (setting) {
                    var obj = {};
                    obj.PriorityType = setting.PriorityType;
                    obj.Brand = setting.Brand;
                    obj.Series = setting.Series;
                    obj.Priority = setting.Priority;
                    obj.PartName = vm.partname;
                    obj.VehicleId = vm.vehicleid;
                    obj.IsEnabled = vm.isEnabled;
                    return obj;
                });
                vm.$Modal.confirm({
                    title: "确认操作?",
                    content: "确认添修改优先级?",
                    loading: true,
                    onOk: function () {
                        $.post("AddOrEditPriorityVehicleSettingNew", { settings: settings }).done(function (res) {
                            if (res.status) {
                                vm.modal = false;
                                vm.$Message.info('操作成功！');
                                vm.$emit('on-submit', true);
                                //清除缓存
                                var vehicleids=   settings.map(function (i) {
                                    return i.VehicleId;
                                })
                                removeCache(vehicleids); 
                            } else {
                                vm.$Message.warning('操作失败！' + (res.msg || ""));
                                vm.$emit('on-submit', false);
                            }
                        }).fail(function () {
                            vm.$Message.warning('操作失败！');
                            vm.$emit('on-submit', false);
                        }).always(function () {
                            vm.$Modal.remove();
                            vm.loading = false;
                            vm.$nextTick(function () {
                                vm.loading = true;
                            });
                        });
                    },
                    onCancel: function () {
                        vm.loading = false;
                        vm.$nextTick(function () {
                            vm.loading = true;
                        });
                    }
                });
            },
        },
    });

    Vue.component('batch-modiffy-priority-modal', {
        template: '#batch-modiffy-priority-modal',
        props: {
            value: {
                type: Boolean,
                default: false,
            },
            vehicleids: {
                type: Array,
                default: function () { return []},
            },
            partname: {
                type: String,
                default: '',
            },
        },
        data: function () {
            return {
                settings: [],
                loading: true,
                isEnabled: true,
            };
        },
        watch: {
            'value': function (value) {
                var vm = this;
                if (!value) {
                    vm.settings = [];
                }
            }
        },
        computed: {
            showModal: {
                get: function () {
                    return this.value;
                },
                set: function (value) {
                    this.$emit('input', value);
                }
            }
        },
        methods: {
            submit: function () {
                var vm = this;
                var vehicleids = vm.vehicleids;
                var temp = (vehicleids || []).map(function (vid) {
                    return (vm.settings || []).map(function (setting) {
                        var obj = {};
                        obj.PriorityType = setting.PriorityType;
                        obj.Brand = setting.Brand;
                        obj.Series = setting.Series;
                        obj.Priority = setting.Priority;
                        obj.PartName = vm.partname;
                        obj.VehicleId = vid;
                        obj.IsEnabled = vm.isEnabled;
                        return obj;
                    });
                });
                var settings = temp.length <= 0 ? [] : temp.reduce(function (a, b) {
                    return a.concat(b);
                });
                vm.$Modal.confirm({
                    title: "确认操作?",
                    content: "确认添修改优先级?",
                    loading: true,
                    onOk: function () {
                        $.post("AddOrEditPriorityVehicleSettingNew", { settings: settings }).done(function (res) {
                            if (res.status) {
                                vm.showModal = false;
                                vm.$Message.info('操作成功！');
                                vm.$emit('on-submit', true);
                                //清除缓存
                                var vehicleids = settings.map(function (i) {
                                    return i.VehicleId;
                                })
                                removeCache(vehicleids); 
                            } else {
                                vm.$Message.warning('操作失败！' + (res.msg || ""));
                                vm.$emit('on-submit', false);
                            }
                        }).fail(function () {
                            vm.$Message.warning('操作失败！');
                            vm.$emit('on-submit', false);
                        }).always(function () {
                            vm.$Modal.remove();
                            vm.loading = false;
                            vm.$nextTick(function () {
                                vm.loading = true;
                            });
                        });
                    },
                    onCancel: function () {
                        vm.loading = false;
                        vm.$nextTick(function () {
                            vm.loading = true;
                        });
                    }
                });
            },
        },
        mounted: function () {
            var vm = this;
            vm.settings = [];
        },
    });

    var vm = new Vue({
        el: '#app',
        data: {
            viscosities: viscosities,
            selection: [],
            vehicleBrands: [],
            vehicleSeries: [],
            filterCondition: {
                brand: '',
                vehicleId: '',
                minPrice: '',
                maxPrice: '',
                isConfig: false,
                viscosity: '',
                pageIndex: 1,
                pageSize: 10,
                isEnabled: false,
                oilBrand: '',
                oilSeries: '',
                oilGrade: '',
                oilPriority: '',
                grade: '',
            },
            columns: [],
            list: [],
            total: 0,
            loading: false,
            vehicleId: '',
            showModal: false,
            showBatchModal: false,
            vehicleids: [],
            brand_series: {},
        },
        created: function () {
            var vm = this;
            vm.columns = [
                { type: 'selection', width: 60, align: 'center' },
                { title: '汽车品牌', key: 'Brand', align: 'center', width: 100, },
                { title: '车系', key: 'Vehicle', align: 'center' },
                { title: '车价', key: 'Price', align: 'center', width: 60, },
                {
                    title: '粘度级别', key: 'Viscosities', align: 'center', width: 100,
                    render: function (h, p) {
                        return h('div', (p.row.Viscosities || []).join(','));
                    }
                },
                {
                    title: '机油等级', key: 'Grades', align: 'center', width: 130,
                    render: function (h, p) {
                        return h('div', (p.row.Grades || []).join(','));
                    }
                },
                {
                    title: '优先级',
                    align: 'center',
                    width: 300,
                    render: function (h, p) {
                        var settings = p.row.Settings.map(function (setting) {
                            return setting;
                        }).sort(function (x, y) {
                            return x.Priority - y.Priority
                        });
                        var elements = settings.map(function (setting, index) {
                            var cols = [
                                h('Col', { props: { span: 6 } }, '优先级' + (index + 1).toString()),
                                h('Col', { props: { span: 16 } }, setting.PriorityType + '-' + setting.Brand + '-' + setting.Series)
                            ];
                            var element = h('Row', {
                                props: {
                                    type: "flex", justify: "space-around", align: "middle"
                                }
                            }, cols);
                            return element;
                        });
                        return elements;
                    },
                },
                {
                    title: '操作',
                    align: 'center',
                    render: function (h, p) {
                        var editButton = h('Button', {

                            on: {
                                click: function () {
                                    vm.showModal = false;
                                    vm.vehicleId = '';
                                    vm.$nextTick(function () {
                                        vm.vehicleId = p.row.VehicleId;
                                        vm.showModal = true;
                                    })
                                },
                            }
                        }, '编辑');
                        var delButton = h('Button', {
                            on: {
                                click: function () {
                                    vm.delete(p.row.VehicleId);
                                },
                            }
                        }, '删除');

                        var disabled = (p.row.Settings || []).length <= 0;
                        var status = disabled || p.row.Settings.all(function (x) { return x.IsEnabled });
                        var text = disabled ? '暂无' : (status ? '禁用' : '启用');
                        var type = status ? 'warning' : 'success';
                        var edButton = h('Button', {
                            props: {
                                disabled: disabled,
                            },
                            on: {
                                click: function () {
                                    vm.$Modal.confirm({
                                        title: "操作确认",
                                        content: "是否" + text + "优先级配置",
                                        loading: true,
                                        onOk: function () {
                                            enableOrDisableVehicleSettingNew([p.row.VehicleId], '机油', !status).done(function (res) {
                                                if (res.status) {
                                                    vm.$Message.info('操作成功');
                                                    (p.row.Settings || []).forEach(function (setting) {
                                                        setting.IsEnabled = !status;
                                                    });
                                                } else {
                                                    vm.$Message.error('操作失败' + (res.msg || ''));
                                                }
                                            }).fail(function () {
                                                vm.$Message.error('操作失败');
                                            }).always(function () {
                                                vm.$Modal.remove();
                                            });
                                        },
                                    });
                                },
                            }
                        }, text);
                        return [editButton, edButton, delButton];
                    }
                },
            ];
        },
        watch: {
            'filterCondition.brand': function (value) {
                var vm = this;
                vm.vehicleSeries = [];
                vm.filterCondition.vehicleId = '';
                vm.getVehicleSeries(value);
            },
            'filterCondition.pageIndex': function (value) {
                var vm = this;
                this.getVehicleSettingNew();
            },
            'filterCondition.pageSize': function (value) {
                var vm = this;
                if (vm.filterCondition.pageIndex === 1) {
                    vm.getVehicleSettingNew();
                }
                else {
                    vm.filterCondition.pageIndex = 1;
                }
            },
        },
        mounted: function () {
            var vm = this;
            vm.getVehicleBrands();
            vm.getVehicleSettingNew();
            vm.getOilBrandAndSeries();
        },
        methods: {
            search: function () {
                var vm = this;
                if (vm.filterCondition.pageIndex === 1) {
                    vm.getVehicleSettingNew();
                }
                else {
                    vm.filterCondition.pageIndex = 1;
                }
            },
            getVehicleSettingNew: function () {
                var vm = this;
                var filterCondition = vm.filterCondition;
                vm.selection = [];
                vm.loading = true;
                $.get('GetBaoYangPriorityVehicleSettingNew', {
                    partName: '机油',
                    brand: filterCondition.brand,
                    vehicleId: filterCondition.vehicleId,
                    minPrice: filterCondition.minPrice,
                    maxPrice: filterCondition.maxPrice,
                    isConfig: filterCondition.isConfig,
                    viscosity: filterCondition.viscosity,
                    pageIndex: filterCondition.pageIndex,
                    pageSize: filterCondition.pageSize,
                    isEnabled: filterCondition.isEnabled,
                    priorityBrand: filterCondition.oilBrand,
                    prioritySeries: filterCondition.oilSeries,
                    priority: filterCondition.oilPriority,
                    priorityType: filterCondition.oilGrade,
                    grade:filterCondition.grade
                }).done(function (data) {
                    vm.list = data.data;
                    vm.total = data.total;
                }).fail(function (xhr) {
                    vm.list = [];
                    vm.total = 0;
                }).always(function () {
                    setTimeout(function () {
                        vm.loading = false;
                    }, 100);
                });
            },
            getVehicleBrands: function () {
                var vm = this;
                $.get('GetVehicleBrands').done(function (data) {
                    vm.vehicleBrands = data.data || [];
                }).fail(function (xhr) {
                    vm.getVehicleBrands = [];
                });
            },
            getVehicleSeries: function (brand) {
                var vm = this;
                if (!brand) {
                    return;
                };
                $.get('GetVehicleSeries', { brand: brand }).done(function (data) {
                    var obj = data.data || {};
                    var keys = Object.keys(obj);
                    var series = keys.map(function (key) {
                        return { VehicleId: key, Vehicle: obj[key] };
                    });
                    vm.vehicleSeries = series;
                }).fail(function (xhr) {
                    vm.vehicleSeries = [];
                });
            },
            onSubmit: function (success, isDelete) {
                var vm = this;
                if (success) {
                    vm.loading = true;
                    setTimeout(function () {
                        if (isDelete) {
                            vm.search();
                        } else {
                            vm.getVehicleSettingNew();
                        }
                    }, 2000);
                }
            },
            delete: function (vehicleId) {
                var vm = this;
                vm.$Modal.confirm({
                    title: "删除确认?",
                    content: "确认删除配置的优先级?",
                    loading: true,
                    onOk: function () {
                        delPriorityVehicleSettingNew([vehicleId], '机油').done(function (res) {
                            if (res.status) {
                                vm.$Message.info('删除成功');
                                vm.onSubmit(true, true);
                                //清除缓存  
                                removeCache([vehicleId]); 
                            } else {
                                vm.$Message.error('删除失败！' + (res.msg || ''));
                            }
                        }).fail(function () {
                            vm.$Message.error('删除失败！');
                        }).always(function () {
                            vm.$Modal.remove();
                        });
                    },
                });
            },
            batchEdit: function () {
                var vm = this;
                var vehicleids = (vm.selection || []).map(function (s) {
                    return s.VehicleId;
                });
                vm.showBatchModal = false;
                vm.vehicleids = [];
                if (vehicleids.length <= 0) {
                    vm.$Message.warning('请至少选择一个车型进行操作');
                    return;
                }
                vm.$nextTick(function () {
                    vm.vehicleids = vehicleids;
                    vm.showBatchModal = true;
                });
            },
            batchDel: function () {
                var vm = this;
                var vehicleids = (vm.selection || []).map(function (s) {
                    return s.VehicleId;
                });
                if (vehicleids.length <= 0) {
                    vm.$Message.warning('请至少选择一个车型进行操作');
                    return;
                }
                vm.$Modal.confirm({
                    title: "删除确认?",
                    content: "确认删除选中的车型配置的优先级?",
                    loading: true,
                    onOk: function () {
                        delPriorityVehicleSettingNew(vehicleids, '机油').done(function (res) {
                            if (res.status) {
                                vm.$Message.info('删除成功');
                                vm.onSubmit(true, true); 
                                removeCache(vehicleids); 
                            } else {
                                vm.$Message.error('删除失败！' + (res.msg || ''));
                            }
                        }).fail(function () {
                            vm.$Message.error('删除失败！');
                        }).always(function () {
                            vm.$Modal.remove();
                        });
                    },
                });
            },
            getOilBrandAndSeries: function () {
                var vm = this;
                $.get("GetOilBrandAndSeries").done(function (res) {
                    vm.brand_series = res.data || {};
                });
            }
        },
        computed: {
            seriesList: function () {
                var vm = this;
                var brand = vm.filterCondition.oilBrand;
                return vm.brand_series[brand] || [];
            },
            brandList: function () {
                var vm = this;
                return Object.keys(vm.brand_series);
            }
        }
    });

    function delPriorityVehicleSettingNew(vehicleIds, partName) {
        return $.post("DelPriorityVehicleSettingNew", {
            vehicleIds: vehicleIds,
            partName: partName
        });
    }
    function enableOrDisableVehicleSettingNew(vehicleIds, partName, isEnabled) {
        return $.post('EnableOrDisableVehicleSettingNew', {
            vehicleIds: vehicleIds,
            partName: partName, isEnabled: isEnabled
        });
    }
    function removeCache(vehicleids) {
        $.post("PriorityVehicleSettingRemoveCache", { vehicleids: vehicleids }, function (data) {
            if (!data) {
                vm.$Message.error('清除缓存失败');
            }
           
        })
    }
    return vm;
}