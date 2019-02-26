$(function () {
    $("#tbList").on('change', "input[type='checkbox']", function () {
        if ($(this).attr("id") == "checkAll") {
            if ($(this).is(":checked")) { //如果当前全选按钮为选中状态
                $("input[type='checkbox']").prop("checked", true); //全选
            } else {
                $("input[type='checkbox']").prop("checked", false);
            }
        } else {
            if ($(this).is(":checked")) {

            } else {
                $("#checkAll").prop("checked", false);
            }
        }
    });
});

var app = angular.module('NodeApp', ['tm.pagination']);

app.controller('ProductVehicleTypeController', ['$scope', 'BusinessService', function ($scope, businessService) {
    $scope.apiInfoList = [];
    $scope.showTable = false;
    $scope.getAllApiInfo = function () {
        var condition = $.trim($("#searchTxt").val());

        console.log(condition);
        var postData = {
            pageIndex: $scope.paginationConf.currentPage,
            pageSize: $scope.paginationConf.itemsPerPage
        };

        if (condition != "" || condition != null) {
            postData.pids = condition;
        }


        businessService.list(postData).success(function (response) {
            $scope.paginationConf.totalItems = response.count;
            $scope.apiInfoList = response.items;
            if ($scope.apiInfoList.length < 1) {//|| $scope.apiInfoList[0].Id == null
                $scope.showTable = false;
            } else {
                $scope.showTable = true;
            }

            //console.log("apiInfoList长度：" + $scope.apiInfoList.length + $scope.showTable);
        });

    };

    //var GetAllProductInfo = function () {
    //    var postData = {
    //        pageIndex: $scope.paginationConf.currentPage,
    //        pageSize: $scope.paginationConf.itemsPerPage
    //    };

    //    businessService.list(postData).success(function (response) {
    //        $scope.paginationConf.totalItems = response.count;
    //        $scope.apiInfoList = response.items;
    //        if ($scope.apiInfoList.length < 1) {//|| $scope.apiInfoList[0].Id == null) {
    //            $scope.showTable = false;
    //        } else {
    //            $scope.showTable = true;
    //        }

    //    });
    //}

    $scope.showEdit = true;
    $scope.master = {};

    //配置分页基本参数
    $scope.paginationConf = {
        currentPage: 1,
        itemsPerPage: 10,

        //totalItems: 0//$scope.apiInfoList.length
    };

    /***************************************************************
    当页码和页面记录数发生变化时监控后台查询
    如果把currentPage和itemsPerPage分开监控的话则会触发两次后台事件。
    ***************************************************************/
    $scope.$watch('paginationConf.currentPage + paginationConf.itemsPerPage ', $scope.getAllApiInfo);
    //$scope.$watch('paginationConf.currentPage + paginationConf.itemsPerPage ', function (value) {
    //    console.log(value);
    //    $scope.getAllApiInfo();
    //});

    $("#btnFindProduct").click(function () {
        //$scope.getAllApiInfo($("#searchTxt").val());
        var searchTxt = $("#searchTxt").val();//"BR-BO-Import|21,BR-BO-Import|22,BR-BO-Import|23";

        if (searchTxt == null || searchTxt === "") {
            alert("查询的PID不能为空,并请注意输入PID格式！");
            $("#searchTxt").attr("style", "border-color:red").focus();
            return false;
        }

        var postData = {
            pageIndex: $scope.paginationConf.currentPage,
            pageSize: $scope.paginationConf.itemsPerPage,
            pids: searchTxt
        };

        businessService.list(postData).success(function (response) {
            $scope.paginationConf.totalItems = response.count;
            $scope.apiInfoList = response.items;
            if ($scope.apiInfoList.length < 1) {//|| $scope.apiInfoList[0].Id == null
                $scope.showTable = false;
                alert("没有查询到相关数据！");
            } else {
                $scope.showTable = true;
                $("#searchTxt").attr("style", "");
                //$scope.$watch('paginationConf.currentPage + paginationConf.itemsPerPage ', $scope.getAllApiInfo);
            }
        });
    })

    $("#btnCopySelected").click(function () {
        var checkItems = $("input[type='checkbox']:checked"); //所有选中的checkbox
        var pidStr = "";
        for (var i = 0; i < checkItems.length; i++) {
            if ($(checkItems[i]).attr("id") !== "checkAll") {
                var pid = $($(checkItems[i]).parent().siblings()[1]).text();
                pidStr += pid + ",";
            }
        }
        var destPids = pidStr.slice(0, pidStr.length - 1);
        var currentPid = $("#hiddenPid").val();
        if (destPids === "") {
            alert("请先选择要复制的产品！");
            return false;
        }
        var isCheckAll = "0";
        var searchTxt = $("#searchTxt").val();
        if ($("#checkAll").is(":checked")) {//如果全选按钮选中
            isCheckAll = "1";
            destPids = searchTxt;
        }

        $.ajax({
            url: '/ProductVehicleType/CopyProductVehicleConfig',
            type: 'POST',
            dataType: 'JSON',
            data: { sourcePid: currentPid, desPids: destPids, isCheckAll: isCheckAll },
            success: function (result) {
                //$("#exampleModal").modal('hide');
                if (result.msg == "success") {
                    alert("复制成功！");
                    //scope.getAllApiInfo();
                    $("input[type='checkbox']").prop("checked", false);
                }
                //window.location.reload();
            }
        });


    });

    $("#btnAdd").click(function () {
        $("#domain_name").val("");
        $("#interface_name").val("");
        $("#interface_addr").val("");
    });
}]);




//业务类
app.factory('BusinessService', ['$http', function ($http) {
    var list = function (postData) {
        return $http.post('/ProductVehicleType/GetAllProductInfoByPids', postData);
    };

    return {
        list: function (postData) {
            return list(postData);
        }
    };
}]);

app.directive("delete", function ($document) {
    return {
        restrict: 'AE',
        require: 'ngModel',
        link: function (scope, element, attrs, ngModel) {
            element.bind("click", function () {
                var pid = ngModel.$modelValue.Pid;
                var hiddenPid = $("#hiddenPid").val();
                //return false;
                //alert("delete item where pid:=" + pid);
                //scope.$apply(function () {
                //    for (var i = 0; i < scope.apiInfoList.length; i++) {
                //        if (scope.apiInfoList[i].Pid == pid) {
                //            console.log(scope.apiInfoList[i]);
                //            scope.apiInfoList.splice(i, 1);
                //        }
                //    }
                //    console.log(scope.apiInfoList);
                //});

                //scope.$watch('paginationConf.currentPage + paginationConf.itemsPerPage + paginationConf.totalItems',);
                $.ajax({
                    url: '/ProductVehicleType/CopyProductVehicleConfig',
                    type: 'POST',
                    dataType: 'JSON',
                    data: { sourcePid: hiddenPid, desPids: pid },
                    success: function (result) {
                        //$("#exampleModal").modal('hide');
                        if (result.msg == "success") {
                            alert("复制成功！");
                            //scope.getAllApiInfo();
                        }
                        //window.location.reload();
                    }
                });
            });
        }
    };
});