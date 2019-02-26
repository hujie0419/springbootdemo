define('api-template-modifylog', function () {
    return {
        QueryTemplateModifyLogsAsync: function (query) {
            return $.post('/PushManager/QueryTemplateModifyLogsAsync', { query }).then(function (data) {
                return data;
            });
        },
        ExportTemplateModifyLogsFileAsync: function (query) {
            window.location.href = "/PushManager/ExportTemplateModifyLogsFileAsync"
                +"?TemplateId=" + query.TemplateId
                + "&PushPlanId=" +query.PushPlanId 
                +"&PushPlanTitle=" + query.PushPlanTitle 
                +"&DeviceType=" + query.DeviceType 
                +"&IsEnable=" +query.IsEnable 
                +"&ModifyUser=" +query.ModifyUser 
                + "&ModifyStartTime=" +query.ModifyStartTime 
                + "&ModifyEndTime=" +query.ModifyEndTime;
        },
    }
});

define('vue-template-modifylog', ['api-template-modifylog'], function (api) {
    return new Vue({
        el: '#AllTemplateModifyLog',
        data: function() {
            return {
                TemplateId: "",
                PushPlanId:"",
                PushPlanTitle: "",
                DeviceType: "",
                IsEnable: "",
                TemplateModifyLogs: [],
                ModifyStartTime: "", 
                ModifyEndTime: "",
                ModifyUser:"",
                status: {
                    loading: false
                },
                current_page: 1, //当前页 
                pages: 50, //总页数 
                changePage: '',//跳转页 
                nowIndex: 0,
              
            }
        },
        computed: {
            query: function() {
                var vm = this;
                return {
                    TemplateId: vm.TemplateId,
                    PushPlanId: vm.PushPlanId,
                    PushPlanTitle: vm.PushPlanTitle,
                    DeviceType: vm.DeviceType,
                    IsEnable: vm.IsEnable,
                    ModifyUser: vm.ModifyUser, 
                    ModifyStartTime: vm.ModifyStartTime,
                    ModifyEndTime: vm.ModifyEndTime, 
                    PageSize: 50
                }
            },
            show: function () {
                return this.pages && this.pages != 1;
            },
            efont: function () {
                if (this.pages <= 7) return false;
                return this.current_page > 5;
            },
            indexs: function () {

                var left = 1,
                    right = this.pages,
                    ar = [];
                if (this.pages >= 7) {
                    if (this.current_page > 5 && this.current_page < this.pages - 4) {
                        left = Number(this.current_page) - 3;
                        right = Number(this.current_page) + 3;
                    } else {
                        if (this.current_page <= 5) {
                            left = 1;
                            right = 7;
                        } else {
                            right = this.pages;

                            left = this.pages - 6;
                        }
                    }
                }
                while (left <= right) {
                    ar.push(left);
                    left++;
                }
                return ar;
            }, 
        },
        methods: {
            DoSearch: function() {
                var vm = this;
                vm.query.PageIndex = 1;
                vm.Search();
            },
            Search: function() {
                var vm = this;
                vm.status.loading = true;
                api.QueryTemplateModifyLogsAsync(vm.query).then(function(data) {
                    vm.status.loading = false;
                    if (data.code == 1) {
                        vm.pages = data.Pager.PageCount;
                        vm.current_page = data.Pager.PageIndex;
                        vm.TemplateModifyLogs = data.TemplateModifyLogs;
                        vm.changePage = "";
                    }
                });
            },
            JumpPage: function (page) {
                var vm = this;  

                if (page > vm.pages || page <= 0) {
                    return;
                }

                vm.current_page = page;
                vm.query.PageIndex = page;
                vm.Search();
            },
            PreviousPage: function () {
                var vm = this;
                if (vm.current_page -1<0) {
                    return;
                }
                vm.current_page--;
                vm.query.PageIndex = vm.current_page;
                vm.Search();
            },
            NextPage: function () {
                var vm = this;
                if (vm.current_page + 1 > vm.pages) {
                    return;
                }
                vm.current_page++;
                vm.query.PageIndex = vm.current_page;
                vm.Search();
            },
            DoExport: function () {
                var vm = this;
                api.ExportTemplateModifyLogsFileAsync(vm.query);
            }

        },
        created: function() {
            var vm = this;
            vm.Search();
        }
    });
});
  