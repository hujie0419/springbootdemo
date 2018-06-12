var app = angular.module('NodeApp', ['tm.pagination']);
var pageIndex = 1;
var pageSize = 15;
app.controller('ProductVehicleTypeController', ['$scope', 'BusinessService', function ($scope, businessService) {
    $scope.apiInfoList = [];
    $scope.showTable = true;
    $scope.number = pageIndex;
    $scope.msg = false;
    //$scope.getAllApiInfo = function () {
    //    var postData = {
    //        pageIndex: pageIndex,
    //        pageSize: pageSize
    //    };
    //    $("body").showLoading();
    //    businessService.list(postData).success(function (response) {
    //        $("body").hideLoading();
    //        //$scope.paginationConf.totalItems = response.count;
    //        //$scope.apiInfoList = response.items;
    //    }); 

    //};

    $scope.showEdit = true;
    $scope.master = {};

    //配置分页基本参数
    //$scope.paginationConf = {
    //    currentPage: 1,
    //    itemsPerPage: 15
    //};

    /***************************************************************
    当页码和页面记录数发生变化时监控后台查询
    如果把currentPage和itemsPerPage分开监控的话则会触发两次后台事件。
    ***************************************************************/
    $scope.$watch('paginationConf.currentPage + paginationConf.itemsPerPage ', $scope.getAllApiInfo);

    $("#btnSearch").click(function() {
        $("body").showLoading();
        var postData = {
            pageIndex: 1,
            pageSize: pageSize
        };
        
        businessService.list(postData).success(function(response) {
            $("body").hideLoading();
            //$scope.paginationConf.totalItems = response.count;
            $scope.apiInfoList = response.items;
            //if ($scope.apiInfoList.length >= 1) { //|| $scope.apiInfoList[0].Id == null
            //    $scope.paginationConf.currentPage = 1;
            //}
        });
    });

    $(".nextPage").click(function () {
        pageIndex = $(".pageIndex").val()
        if (pageIndex == "") {
            pageIndex = 1;
        } else {
            pageIndex = parseInt(pageIndex) + 1;
        }
        $("body").showLoading();
        var postData = {
            pageIndex: pageIndex,
            pageSize: pageSize
        };
        $scope.number = pageIndex;
        console.log(pageIndex);
        businessService.list(postData).success(function (response) {
            $("body").hideLoading();
            //$scope.paginationConf.totalItems = response.count;
            $scope.apiInfoList = response.items;
            if (response.items.length < 1) {
                $scope.msg = true;
                $(".nextPage").attr("disabled", "disabled");
            }
            //if ($scope.apiInfoList.length >= 1) { //|| $scope.apiInfoList[0].Id == null
            //    $scope.paginationConf.currentPage = 1;
            //}
        });
    });
}]);

//自定义服务
app.service('Convert', function () {
    this.myFunc = function (x) {
        var result = "";
        if (x == 1) {
            result = "是";
        } else {
            result = "否"
        }
        return result;
    }
});

///创建过滤器
app.filter('myFormat', ['Convert', function (Convert) {
    return function (x) {
        return Convert.myFunc(x);
    };
}]);

//业务类
app.factory('BusinessService', ['$http', function ($http) {
    var list = function (postData) {
        return $http.post('/ProductVehicleType/GetAllNoImportProduct', postData);
    };

    return {
        list: function (postData) {
            return list(postData);
        }
    };
}]);

$(function () {
    $("#btnSearch").click();
});





