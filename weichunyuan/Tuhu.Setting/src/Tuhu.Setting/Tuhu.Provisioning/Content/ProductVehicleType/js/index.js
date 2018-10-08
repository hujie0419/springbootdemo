var app = angular.module('NodeApp', ['tm.pagination']);

app.controller('ProductVehicleTypeController', ['$scope', 'BusinessService', function ($scope, businessService) {
    $scope.apiInfoList = [];
    $scope.showTable = false;
    $scope.getAllApiInfo = function () {
        var condition = $.trim($("#searchTxt").val());
        var configStatus = $("#selectConfigStatus").find("option:selected").val();
        console.log(condition);
        var postData = {
            pageIndex: $scope.paginationConf.currentPage,
            pageSize: $scope.paginationConf.itemsPerPage
        };

        if (condition !== "" || condition != null) {
            $("body").showLoading();
            postData.condition = condition;
        }
        if (configStatus != "" || configStatus != null) {
            postData.status = configStatus;
        }
        
        
        businessService.list(postData).success(function (response) {
            $("body").hideLoading();
            $scope.paginationConf.totalItems = response.count;
            $scope.apiInfoList = response.items;
            if ($scope.apiInfoList.length < 1) {//|| $scope.apiInfoList[0].Id == null
                $scope.showTable = false;
                if (condition !== "") {
                    $("#tipMsg").attr("style", "display:block");
                }
            } else {
                $scope.showTable = true;
                $("#tipMsg").attr("style", "display:none");
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
        itemsPerPage: 10

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

    $("#btnSearch").click(function() {
        //$scope.getAllApiInfo($("#searchTxt").val());
        $("body").showLoading();
        var searchTxt = $("#searchTxt").val();
        $("#selectConfigStatus").find("option[value='配置状态']").attr("selected", "selected");

        var postData = {
            pageIndex: 1,
            pageSize: $scope.paginationConf.itemsPerPage,
            condition: searchTxt,
            status:"配置状态"
        };
        
        businessService.list(postData).success(function(response) {
            $("body").hideLoading();
            $scope.paginationConf.totalItems = response.count;
            $scope.apiInfoList = response.items;
            if ($scope.apiInfoList.length < 1) { //|| $scope.apiInfoList[0].Id == null
                $scope.showTable = false;
                $("#tipMsg").attr("style", "display:block");
            } else {
                $scope.showTable = true;
                $("#tipMsg").attr("style", "display:none");
                //$scope.$watch('paginationConf.currentPage + paginationConf.itemsPerPage ', $scope.getAllApiInfo);
                $scope.paginationConf.currentPage = 1;
            }
        });
    });

    $("#selectConfigStatus").on('change', function () {
        $("body").showLoading();
        var searchTxt = $("#searchTxt").val();
        var configStatus = $(this).find("option:selected").val();
        var postData = {
            pageIndex: $scope.paginationConf.currentPage,
            pageSize: $scope.paginationConf.itemsPerPage,
            condition: searchTxt,
            status: configStatus
        };

        businessService.list(postData).success(function (response) {
            $("body").hideLoading();
            $scope.paginationConf.totalItems = response.count;
            $scope.apiInfoList = response.items;
            if ($scope.apiInfoList.length < 1) {//|| $scope.apiInfoList[0].Id == null
                $scope.showTable = false;
                $("#tipMsg").attr("style", "display:block");
            } else {
                $scope.showTable = true;
                $("#tipMsg").attr("style", "display:none");
                //$scope.$watch('paginationConf.currentPage + paginationConf.itemsPerPage ', $scope.getAllApiInfo);
            }
        });

    });

    $("#btnSave").click(function () {
        var domainName = $("#domain_name").val();
        var interfaceName = $("#interface_name").val();
        var interfaceAddr = $("#interface_addr").val();

        if (domainName == "" || interfaceName == "" || interfaceAddr == "") {
            alert("不允许空值！");
            return false;
        }

        var param = {
            Host: domainName,
            ServiceName: interfaceName,
            Path: interfaceAddr
        };

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
        return $http.post('/ProductVehicleType/GetAllProductInfoByParams', postData);
    };

    return {
        list: function (postData) {
            return list(postData);
        }
    };
}]);

$(function () {
    document.onkeydown = function(e) {//搜索响应回车键
        var ev = document.all ? window.event : e;
        if (ev.keyCode === 13) {
            $("#btnSearch").click();
        }
    }
});





