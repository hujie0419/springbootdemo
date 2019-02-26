new Vue({
    el: '#typeconfig',
    data: function () {
        return {
            NavigationConfigs: [],
            EditConfig: {},
            DisableAlias: false
        }
    },
    created: function () {
        var vm = this;
        vm.ReSearchConfigs();
    },
    methods: {
        SelectAllConfigs: function () {
            var vm = this;
            return $.get('/PushManager/SelectMessageNavigationTypesAsync').then(function (data) {
                return data;
            });
        },
        ReSearchConfigs: function () {
            var vm = this;
            vm.SelectAllConfigs().then(function (data) {
                if (data && data.length) {
                    vm.NavigationConfigs = data;
                } else {
                    vm.NavigationConfigs = [];
                }
            });
        },
        SaveTypeConfig: function (config) {
            var vm = this;
            $.post("/PushManager/SaveNavigationTypeConfigAsync", { config }).then(function (data) {
                console.log(data);

                if (data.code == 1) {
                    vm.ReSearchConfigs();
                    var dialog = $('#EditConfig');
                    dialog.dialog("close");
                }
                alert(data.msg);
            });
        },
        DoEditItem: function (item) {
            var vm = this;
            var temp = JSON.parse(JSON.stringify(item));
            vm.DisableAlias = temp.PushAlias && temp.PushAlias != '';
            vm.EditConfig = temp;
            vm.ShowDialog();
        },
        DoDeleteItem: function (item) {
            var vm = this;
            $.post('/PushManager/DeleteMessageNavigationTypeAsync', { pkid: item.PkId }).then(function (data) {
                console.log(data);
                if (data == 1) {
                    alert("删除成功");
                    vm.ReSearchConfigs();
                } else {
                    alert("删除失败");
                }
            });
        },
        ShowDialog: function () {
            var vm = this;

            var dialog = $('#EditConfig');
            dialog.dialog({
                buttons: {
                    "取消": function () {
                        $(this).dialog("close");
                    },
                    "保存": function () {
                        vm.SaveTypeConfig(vm.EditConfig);
                    },
                },
                title: "消息盒子导航分类",
                width: "400px",
            });
        },
        AddTypeConfig: function () {
            var vm = this;
            vm.EditConfig = {};
            vm.DisableAlias = false;
            vm.ShowDialog();
        },
    },
});