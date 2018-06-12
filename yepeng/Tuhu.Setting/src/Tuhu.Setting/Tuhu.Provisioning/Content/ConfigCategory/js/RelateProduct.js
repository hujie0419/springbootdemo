
var app = angular.module('NodeApp', ['tm.pagination']);

app.controller('ConfigCategoryController', ['$scope', 'BusinessService', function ($scope, businessService) {

    var vm = $scope.vm = {};


    $scope.load = function () {
        //alert('dom ready');
        //$scope.getAllApiInfo();
    }
    $scope.ConfigCategory = [];
    $scope.showTable = false;
    $scope.getAllApiInfo = function () {

        businessService.list().success(function (response) {

            $scope.ConfigCategory = response.items;
            if ($scope.ConfigCategory.length < 1) {//|| $scope.apiInfoList[0].Id == null
                $scope.showTable = false;
            } else {
                $scope.showTable = true;
            }
            vm.vehicles = response.items;
            console.log("apiInfoList长度：" + $scope.ConfigCategory.length + $scope.showTable);
        });

    };

    $scope.itemClicked = function ($item) {
        $scope.selectedItem = $item;
        console.log($item, 'item clicked');
    };

    $scope.itemCheckedChanged = function ($item) {
        
        console.log($item, 'item checked');
    };


    $scope.showEdit = true;
    $scope.master = {};

    //点击展开或者收起信息
    $scope.showNext = function (target) {
        if ($(target).hasClass("triangle-icon-dowm")) {
            //如果当前元素箭头向下，则收起
            $(target).removeClass("triangle-icon-dowm");
            $(target).parent().siblings().removeClass("show");
        } else {
            $(target).addClass("triangle-icon-dowm");
            $(target).parent().siblings().addClass("show");
        }
    }

    $scope.checkNext = function (target) {
        var isChecked = true;
        var siblingItems;
        var level = $(target).attr("id");
        if ($(target).is(":checked")) {
            //如果当前元素处于选中状态，则选中下一级所有的checkbox
            if (level === "level1") {//当前元素level1且选中则下面所有的都选中
                $(target).parent().parent().parent().parent().parent().find("input[type='checkbox']").prop("checked", true);
            }
            if (level === "level2") {
                $(target).parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", true);//当前节点下级所有都选中
                siblingItems = $(target).parent().parent().parent().parent().parent().siblings().find("input[type='checkbox']");//获取所有兄弟节点下的所有子节点
                for (var i = 0; i < siblingItems.length; i++) {
                    if ($(siblingItems[i]).is(":checked") === false) {
                        isChecked = false;
                        break;
                    }
                }
                if (isChecked) {
                    $(target).parent().parent().parent().parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", true);
                } else {
                    $(target).parent().parent().parent().parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", false);
                }
            }
            if (level === "level3") {
                var isCheckLevel3 = true;
                $(target).parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", true);//当前节点下级所有都选中
                siblingItems = $(target).parent().parent().parent().parent().parent().siblings().find("input[type='checkbox']");//三级所有兄弟节点下的子节点
                for (var j = 0; j < siblingItems.length; j++) {
                    if ($(siblingItems[j]).is(":checked") === false) {
                        isChecked = false;
                        break;
                    }
                }
                if (isChecked) {//判断二级节点是否选中
                    $(target).parent().parent().parent().parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", true);
                } else {
                    $(target).parent().parent().parent().parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", false);
                }

                var siblingItemsLevel2 = $(target).parent().parent().parent().parent().parent().parent().parent().parent().parent().find("input[id='level2']");//二级所有
                for (var k = 0; k < siblingItemsLevel2.length; k++) {
                    if ($(siblingItemsLevel2[k]).is(":checked") === false) {
                        isCheckLevel3 = false;
                        break;
                    }
                }
                if (isCheckLevel3) {//判断一级节点是否选中
                    $(target).parent().parent().parent().parent().parent().parent().parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", true);
                } else {
                    $(target).parent().parent().parent().parent().parent().parent().parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", false);
                }

            }
        } else {
            if (level === "level1") {//当前元素level1且选中则下面所有的都不选中
                $(target).parent().parent().parent().parent().parent().find("input[type='checkbox']").prop("checked", false);
            }
            if (level === "level2") {
                $(target).parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", false);//当前节点下级所有都取消选中
                $(target).parent().parent().parent().parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", false);
            }
            if (level === "level3") {
                $(target).parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", false);
                $(target).parent().parent().parent().parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", false);
                $(target).parent().parent().parent().parent().parent().parent().parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", false);
            }
        }
    }
    
    /***************************************************************
    当页码和页面记录数发生变化时监控后台查询
    如果把currentPage和itemsPerPage分开监控的话则会触发两次后台事件。
    ***************************************************************/
    //$scope.$watch('paginationConf.currentPage + paginationConf.itemsPerPage ', $scope.getAllApiInfo);
    //$scope.$watch('paginationConf.currentPage + paginationConf.itemsPerPage ', GetAllProductInfo);



}]);




//业务类
app.factory('BusinessService', ['$http', function ($http) {
    var list = function () {
        return $http.post('/ConfigCategory/GetAllProductCategorys', null);
    };

    return {
        list: function () {
            return list();
        }
    };
}]);





