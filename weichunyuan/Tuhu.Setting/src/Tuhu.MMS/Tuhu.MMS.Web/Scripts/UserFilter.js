define('api', function () {
    return {
        SelectFilterRuleJobDetailsAsync: function (jobid) {
            return $.post('/UserFilter/SelectFilterRuleJobDetailsAsync', { jobid }).then(function (data) {
                return data;
            });
        },
        DeleteFilterRuleJobDetailAsync: function (batchid) {
            return $.post('/UserFilter/DeleteFilterRuleJobDetailAsync', { batchid }).then(function (data) {
                return data;
            });
        },
        SaveJobDescriptionAsync: function (jobid, description) {
            return $.post('/UserFilter/SaveJobDescriptionAsync', { jobid, description }).then(function (data) {
                return data;
            });
        },
        SubmitJobDetail: function (FirstCategory, SecondCategory, TableName, JoinType, JobId, FormData) {
            var url = '/UserFilter/SubmitJobDetail?FirstCategory=' +
                FirstCategory +
                '&SecondCategory=' +
                SecondCategory +
                '&TableName=' +
                TableName +
                '&JoinType=' +
                JoinType +
                '&JobId=' +
                JobId;

            return $.post(url, FormData).then(function (data) {
                return data;
            });
        },
        SelectUserFilterValueConfigsAsync: function (type, value) {
            return $.get('/UserFilter/SelectUserFilterValueConfigsAsync', { type, value }).then(function (data) {
                return data;
            });
        },
        SubmitUserFilterResultConfigAsync: function (basecategory, tablename, colname, jobid, ischecked) {
            return $.post('/UserFilter/SubmitUserFilterResultConfigAsync', { basecategory, tablename, colname, jobid, ischecked }).then(function (data) {
                return data;
            });
        },
        SelectUserFilterResultConfigAsync: function (jobid) {
            return $.get('/UserFilter/SelectUserFilterResultConfigAsync', { jobid }).then(function (data) {
                return data;
            });
        },
        SetJobStartRunAsync: function (jobid) {
            return $.get('/UserFilter/SetJobStartRunAsync', { jobid }).then(function (data) {
                return data;
            });
        },

    }
});
define('vue-select-filterrule', ['api'], function (api) {
    return new Vue({
        el: '#selectfilterrule',
        data: function () {
            return {
                jobid: 0,
                FilterRuleJobResults: [],
                ShowStaticProperty: true,//静态属性统计
                ShowVehicle: false,//车辆统计
                ShowViews: false,//浏览统计
                ShowOrder: false,//订单统计
                ShowUser: false,//会员统计,
                BaseCategorySelect: "",
                SecondCategorySelect: "",
                JoinType: 'and',
                SelectItem: ''
            }
        },
        created: function () {
            var vm = this;
            var jobid = $("#jobid").val();
            vm.jobid = jobid;
            vm.SelectUserFilterResultConfig();
        },
        methods: {
            BaseCategoryClick: function (type) {
                var vm = this;
                vm.BaseCategorySelect = type;
                vm.SecondCategorySelect = '';
            },
            SelectUserFilterResultConfig: function () {
                var vm = this;
                api.SelectUserFilterResultConfigAsync(vm.jobid).then(function (data) {
                    if (data.code == "1") {
                        vm.FilterRuleJobResults = data.data;
                    }
                });
            },
            SaveSelectUserFilterResult: function (tablename, colname, tablecolname) {
                var vm = this;
                var checkbox = $('#' + vm.BaseCategorySelect).find("input[name=" + colname + "]");
                var ischecked = $(checkbox[0]).is(':checked');
                console.log(ischecked);
                api.SubmitUserFilterResultConfigAsync(vm.BaseCategorySelect, tablename, colname, vm.jobid, ischecked).then(function (result) {
                    if (result && result.code == 0) {
                        alert(result.msg);
                        $(checkbox[0]).prop('checked', false);
                    }
                    vm.SelectUserFilterResultConfig();
                });
            },
        },
        computed: {

        }
    });
});

define('vue-edit-filterrule', ['api'], function (api) {
    return new Vue({
        el: '#edituserfilterrule',
        data: function () {
            return {
                jobid: 0,
                FilterRuleJobDetails: [],
                ShowStaticProperty: true,//静态属性统计
                ShowVehicle: false,//车辆统计
                ShowViews: false,//浏览统计
                ShowOrder: false,//订单统计
                ShowUser: false,//会员统计,
                BaseCategorySelect: "",
                SecondCategorySelect: "",
                JoinType: 'and',
                SelectItem: '',
                JobDescription: ""
            }
        },
        created: function () {
            var vm = this;
            var jobid = $("#jobid").val();
            vm.jobid = jobid;
            vm.SelectFilterRuleJobDetailsAsync();
        },
        methods: {
            SelectFilterRuleJobDetailsAsync: function () {
                var vm = this;
                api.SelectFilterRuleJobDetailsAsync(vm.jobid).then(function (data) {
                    if (data && data.code == "1") {
                        vm.FilterRuleJobDetails = data.datas;
                    }
                    else if (data && data.code == "-1") {
                        vm.FilterRuleJobDetails = data.datas;
                    }
                });
            },
            BaseCategoryClick: function (type) {
                var vm = this;
                vm.BaseCategorySelect = type;
                vm.SecondCategorySelect = '';
            },
            SaveJobDescription: function () {
                var vm = this;
                api.SaveJobDescriptionAsync(vm.jobid, vm.JobDescription).then(function (data) {
                    alert(data.msg);
                });
            },
            selectchange: function (colname) {
                console.log(colname);
                var vm = this;
                var type = vm.SecondCategorySelect;
                function setvalue(selectvalue, childcolname) {
                    api.SelectUserFilterValueConfigsAsync("parentvalue", selectvalue).then(function (data) {
                        var dropdownlist = $('#' + type).find("select[name=" + childcolname + "]");
                        dropdownlist.empty();
                        $.each(data, function (key, value) {
                            console.log(key);
                            console.log(value);
                            dropdownlist.append($("<option></option>")
                                .attr("value", value.Value).text(value.Name));
                        });

                    });
                };
                var dropdownlist = $('#' + type).find("select[name=" + colname + "]");
                var selectvalue = dropdownlist.val();
                if (!selectvalue || selectvalue == '') {
                    return;
                }
                if (colname == 'province') {
                    setvalue(selectvalue, "city");

                }
                else if (colname == 'vehiclebrand') {
                    setvalue(selectvalue, "vehiclename");
                    vm.selectchange("vehiclename");
                }
                else if (colname == 'vehiclename') {
                    setvalue(selectvalue, "vehiclepailiang");
                    vm.selectchange("vehiclepailiang");
                }
                else if (colname == 'vehiclepailiang') {
                    setvalue(selectvalue, "vehiclenian");
                    vm.selectchange("vehiclenian");
                }
                else if (colname == 'vehiclenian') {
                    setvalue(selectvalue, "vehiclesalesname");
                    vm.selectchange("vehiclesalesname");
                }
                else if (colname == 'businesslines') {
                    setvalue(selectvalue, "category1");
                    vm.selectchange("category1");

                    var temp = "category";
                    var templist = $('#' + type).find("select[name=" + temp + "]");
                    templist.val("");
                }
                else if (colname == 'category1') {
                    setvalue(selectvalue, "category2");
                    vm.selectchange("category2");
                }
                else if (colname == 'category2') {
                    setvalue(selectvalue, "category3");
                }

                else if (colname == 'category') {
                    setvalue(selectvalue, "category1");
                    vm.selectchange("category1");
                   
                    var temp = "businesslines";
                    var templist = $('#' + type).find("select[name=" + temp + "]");
                    templist.val("");
                } 

            },
            SecondCategoryClick: function (type) {
                var vm = this;

                function setvalue(colname) {
                    api.SelectUserFilterValueConfigsAsync("colname", colname).then(function (data) {
                        var dropdownlist = $('#' + type).find("select[name=" + colname + "]");
                        dropdownlist.empty();
                        $.each(data, function (key, value) {
                            console.log(key);
                            console.log(value);
                            dropdownlist.append($("<option></option>")
                                .attr("value", value.Value).text(value.Name));
                        });

                    });
                };

                vm.SecondCategorySelect = type;

                //if (type == 'Sex') {
                //    setvalue("gender");
                //}
                //else if (type == 'Area') {
                //    setvalue("province");
                //    setvalue("city");
                //}
                //else if (type == 'DefaultVehicle') {
                //    setvalue('vehiclebrand');
                //    setvalue('vehiclename');
                //    setvalue('vehiclepailiang');
                //    setvalue('vehiclenian');
                //    setvalue('vehiclesalesname');
                //}
            },
            DeleteFilterRuleJobDetail: function (batchid) {
                var vm = this;
                api.DeleteFilterRuleJobDetailAsync(batchid).then(function (data) {
                    alert(data.msg);
                    vm.SelectFilterRuleJobDetailsAsync();
                });
            },
            SubmitForm: function () {
                var vm = this;
                if (vm.SecondCategorySelect == '') {
                    alert("未选择数据");
                    return;
                }
                var frm = $('#' + vm.SecondCategorySelect);
                var formdata = frm.serialize();
                console.log(formdata);
                api.SubmitJobDetail(vm.BaseCategorySelect, vm.SecondCategorySelect, vm.SelectdTableName, vm.JoinType, vm.jobid, formdata).then(
                    function (data) {
                        console.log(data);
                        alert(data.msg);
                        vm.SelectFilterRuleJobDetailsAsync();
                    });
                //frm.submit(function (e) {
                //    e.preventDefault();
                //    $.ajax({
                //        type: 'POST',
                //        url: '/',
                //        data: frm.serialize(),
                //        success: function (data) {
                //            console.log(data);
                //        }
                //    });
                //});
            },
            SetJobStartRun: function () {
                var vm = this;
                api.SetJobStartRunAsync(vm.jobid).then(function (data) {
                    alert(data.msg);
                    if (data.code == 1) {
                        location.reload();
                    }
                });
            },
        },
        computed: {
            SelectdTableName: function () {
                var vm = this;
                if (vm.BaseCategorySelect == 'ShowStaticProperty') {
                    return 'dm_userdetail';
                }
                else if (vm.BaseCategorySelect == 'ShowVehicle') {
                    return 'dm_userdetail';
                }
                else if (vm.BaseCategorySelect == 'ShowViews') {
                    return "dm_userlog";
                }
                else if (vm.BaseCategorySelect == 'ShowOrders') {
                    return "dm_userorderdetail";
                }
            },
        }
    });
});