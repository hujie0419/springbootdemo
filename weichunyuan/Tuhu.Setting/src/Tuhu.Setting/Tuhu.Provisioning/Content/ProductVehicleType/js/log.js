var app = angular.module('NodeApp', ['tm.pagination']);

app.controller('ProductVehicleTypeController', ['$scope', 'BusinessService', function ($scope, businessService) {
    //$scope.load = function () {
    //    //alert('dom ready');
    //    $scope.getAllApiInfo();
    //}
    $scope.apiInfoList = [];
    $scope.showTable = false;
    $scope.getAllApiInfo = function () {
        var startTime = $("#datetimepicker1").val();
        var endTime = $("#datetimepicker2").val();

        var postData = {
            pageIndex: $scope.paginationConf.currentPage,
            pageSize: $scope.paginationConf.itemsPerPage,
            TimeStart: startTime,
            TimeEnd: endTime
        };


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

    $("#btnFindProduct").click(function () {
        //$scope.getAllApiInfo($("#searchTxt").val());
        var startTime = $("#datetimepicker1").val();
        var endTime = $("#datetimepicker2").val();

        if (startTime == "" && endTime == "") {
            alert("请先选择查询时间范围！");
            //$("#searchTxt").attr("style", "border-color:red").focus();
            return false;
        }

        var postData = {
            pageIndex: $scope.paginationConf.currentPage,
            pageSize: $scope.paginationConf.itemsPerPage,
            TimeStart: startTime,
            TimeEnd: endTime
        };

        businessService.list(postData).success(function (response) {
            $scope.paginationConf.totalItems = response.count;
            $scope.apiInfoList = response.items;
            if ($scope.apiInfoList.length < 1) {//|| $scope.apiInfoList[0].Id == null
                $scope.showTable = false;
                alert("没有查询到相关数据！");
            } else {
                $scope.showTable = true;
            }
        });
    })
}]);




//业务类
app.factory('BusinessService', ['$http', function ($http) {
    var list = function (postData) {
        return $http.post('/ProductVehicleType/GetAllLogByTime', postData);
    };

    return {
        list: function (postData) {
            return list(postData);
        }
    };
}]);