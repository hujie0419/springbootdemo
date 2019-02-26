function RetSelecteds() {
    var result = "";
    $("#filter a[class='seled']").each(function () {
        result += $(this).html() + "\n";
    });
    return result;
}

var app = angular.module('NodeApp', ['tm.pagination']);

app.controller('ConfigCategoryController', ['$scope', 'BusinessService', function ($scope, businessService) {

    var vm = $scope.vm = {};


    $scope.load = function () {
        //alert('dom ready');
        //$scope.getAllApiInfo();
    }
    $scope.apiInfoList = [];
    $scope.showTable = false;
    $scope.getAllApiInfo = function () {
        var condition = "";
        console.log(condition);
        var articleId = $("#PKID").val();
        var postData = {
            Condition: "B",//默认传 B
            ArticleID: articleId
        };

        if (condition != "" || condition != null) {
            postData.Condition = condition;
        }
        console.log(JSON.stringify(postData));

        businessService.list(postData).success(function (response) {

            $scope.apiInfoList = response.items;
            if ($scope.apiInfoList.length < 1) {//|| $scope.apiInfoList[0].Id == null
                $scope.showTable = false;
            } else {
                $scope.showTable = true;
            }

            console.log("apiInfoList长度：" + $scope.apiInfoList.length + $scope.showTable);
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

    //点击展开或者收起车型信息
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
            if (level === "level4") {
                $(target).parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", true);//当前节点下级所有都选中
                siblingItems = $(target).parent().parent().parent().parent().parent().parent().find("input[id='level4']");//四级所有兄弟节点
                for (var j1 = 0; j1 < siblingItems.length; j1++) {
                    if ($(siblingItems[j1]).is(":checked") === false) {
                        isChecked = false;
                        break;
                    }
                }
                if (isChecked) {//判断三级节点是否选中
                    $(target).parent().parent().parent().parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", true);
                } else {
                    $(target).parent().parent().parent().parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", false);
                }
                var isCheckedLevel2 = true;
                var siblingItems3 = $(target).parent().parent().parent().parent().parent().parent().parent().parent().parent().find("input[id='level3']");//获取对应的三级节点
                for (var j2 = 0; j2 < siblingItems3.length; j2++) {
                    if ($(siblingItems3[j2]).is(":checked") === false) {
                        isCheckedLevel2 = false;
                        break;
                    }
                }
                if (isCheckedLevel2) {//判断二级节点是否选中
                    $(target).parent().parent().parent().parent().parent().parent().parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", true);
                } else {
                    $(target).parent().parent().parent().parent().parent().parent().parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", false);
                }

                var isCheckedLevel1 = true;
                var siblingItems2 = $(target).parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().find("input[id='level2']");//获取对应的二级节点
                for (var j3 = 0; j3 < siblingItems2.length; j3++) {
                    if ($(siblingItems2[j3]).is(":checked") === false) {
                        isCheckedLevel1 = false;
                        break;
                    }
                }
                if (isCheckedLevel1) {//判断一级节点是否选中
                    $(target).parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", true);
                } else {
                    $(target).parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", false);
                }
            }
            if (level === "level5") {
                var siblingItems5 = $(target).parent().parent().parent().parent().parent().find("input[type='checkbox']");//获取所有五级节点
                var isCheck4 = true;
                for (var t1 = 0; t1 < siblingItems5.length; t1++) {
                    if ($(siblingItems5[t1]).is(":checked") === false) {
                        isCheck4 = false;
                        break;
                    }
                }
                if (isCheck4) {//判断四级节点是否选中
                    $(target).parent().parent().parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", true);
                } else {
                    $(target).parent().parent().parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", false);
                }

                var siblingItems4 = $(target).parent().parent().parent().parent().parent().parent().parent().parent().find("input[id='level4']");//获取所有四级节点
                var isCheck3 = true;
                for (var t2 = 0; t2 < siblingItems4.length; t2++) {
                    if ($(siblingItems4[t2]).is(":checked") === false) {
                        isCheck3 = false;
                        break;
                    }
                }
                if (isCheck3) {//判断三级节点是否选中
                    $(target).parent().parent().parent().parent().parent().parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", true);
                } else {
                    $(target).parent().parent().parent().parent().parent().parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", false);
                }

                var sibling3 = $(target).parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().find("input[id='level3']");//获取所有三级节点
                var isCheck2 = true;
                for (var t3 = 0; t3 < sibling3.length; t3++) {
                    if ($(sibling3[t3]).is(":checked") === false) {
                        isCheck2 = false;
                        break;
                    }
                }
                if (isCheck2) {//判断二级节点是否选中
                    $(target).parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", true);
                } else {
                    $(target).parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", false);
                }

                var sibling2 = $(target).parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().find("input[id='level2']"); //获取所有二级节点
                var isCheck1 = true;
                for (var t4 = 0; t4 < sibling2.length; t4++) {
                    if ($(sibling2[t4]).is(":checked") === false) {
                        isCheck1 = false;
                        break;
                    }
                }
                if (isCheck1) {//判断一级节点是否选中
                    $(target).parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", true);
                } else {
                    $(target).parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", false);
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
            if (level === "level4") {
                $(target).parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", false);
                $(target).parent().parent().parent().parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", false);
                $(target).parent().parent().parent().parent().parent().parent().parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", false);
                $(target).parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", false);
            }
            if (level === "level5") {
                $(target).parent().parent().parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", false);
                $(target).parent().parent().parent().parent().parent().parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", false);
                $(target).parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", false);
                $(target).parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().parent().siblings().find("input[type='checkbox']").prop("checked", false);
            }
        }
    }



    /***************************************************************
    当页码和页面记录数发生变化时监控后台查询
    如果把currentPage和itemsPerPage分开监控的话则会触发两次后台事件。
    ***************************************************************/
    //$scope.$watch('paginationConf.currentPage + paginationConf.itemsPerPage ', $scope.getAllApiInfo);
    //$scope.$watch('paginationConf.currentPage + paginationConf.itemsPerPage ', GetAllProductInfo);

    $("#btnSave").click(function () {
       // $("body").showLoading();
        var paramItems = [];
        var checkItems = [];
        var checkBoxArray = $(".search-list").find("input[type='checkbox']:checked");

        for (var i = 0; i < checkBoxArray.length; i++) {
            var item = {};
            var level = $(checkBoxArray[i]).attr("id");
            var name = $(checkBoxArray[i]).attr("name");
           
            item.Level = level;
            item.Name = name;
            checkItems.push(item);
        }


        console.log(JSON.stringify(checkItems));

        var level1Items = _.filter(checkItems, function (dataItem) { return dataItem.Level == 'level1'; });
        var level2Items = _.filter(checkItems, function (dataItem) { return dataItem.Level == 'level2'; });
        var level3Items = _.filter(checkItems, function (dataItem) { return dataItem.Level == 'level3'; });
        var level4Items = _.filter(checkItems, function (dataItem) { return dataItem.Level == 'level4'; });
        var level5Items = _.filter(checkItems, function (dataItem) { return dataItem.Level == 'level5'; });

        fillLevelItems(paramItems, level1Items, "level1");
        fillLevelItems(paramItems, level2Items, "level2");
        fillLevelItems(paramItems, level3Items, "level3");
        fillLevelItems(paramItems, level4Items, "level4");
        fillLevelItems(paramItems, level5Items, "level5");

        var selectParams = getPostDataParams();
        console.log(JSON.stringify(selectParams));

        var articleId = $("#PKID").val(); 
        var articleName = $("#ArticleName").val();
        var articleLink = $("#ArticleLink").val();
        
        if (articleName == null || articleLink == null || articleName.length === 0 || articleLink.length === 0 || articleName.length === "" || articleLink.length === "") {
            alert("文章名字和链接不能为空");
            return;
        }
        if (articleId != null && articleId > 0) {
            $.ajax({
                url: '/ConfigCategory/UpdateArticleItem',
                type: 'POST',
                dataType: 'JSON',
                data: { param: JSON.stringify(paramItems), selectItems: JSON.stringify(selectParams),pkid:$("#PKID").val(), categoryId: $("#CategoryID").val(), articleName: $("#ArticleName").val(), articleLink: $("#ArticleLink").val() },
                success: function (result) {
                    $("body").hideLoading();
                    if (result.msg == "success") {
                        alert("保存成功！");
                        window.location.href = "/ConfigCategory/ArticleCategoryList?categoryid=" + $("#CategoryID").val();
                    }

                }
            });
        } else {
            $.ajax({
                url: '/ConfigCategory/InsertArticleItem',
                type: 'POST',
                dataType: 'JSON',
                data: { param: JSON.stringify(paramItems), selectItems: JSON.stringify(selectParams), categoryId: $("#CategoryID").val(), articleName: $("#ArticleName").val(), articleLink: $("#ArticleLink").val() },
                success: function(result) {
                    $("body").hideLoading();
                    if (result.msg == "success") {
                        alert("保存成功！");
                        window.location.href = "/ConfigCategory/ArticleCategoryList?categoryid=" + $("#CategoryID").val();
                    }

                }
            });
        }
    });

    $("#filter a").hover(
           function () {
               $(this).addClass("seling");
           },
           function () {
               $(this).removeClass("seling");
           }
       );

    //为filter下的所有a标签添加单击事件  
    $("#filter a").click(function () {
        if ($scope.showTable) {
            var r = confirm("切换编辑车型前，请确定先保存当前修改的数据");
            if (r === true) {
            } else {
                return false;
            }
        }

        $("body").showLoading();
        //$("#loadModal").modal({
        //    keyboard: false,
        //    show: true
        //});
        $(this).parents("dl").children("dd").each(function () {
            //下面三种方式效果相同（第三种写法的内部就是调用了find()函数，所以，第二、三种方法是等价的。）  
            //$(this).children("div").children("a").removeClass("seled");  
            //$(this).find("a").removeClass("seled");  
            $('a', this).removeClass("seled");
        });

        $(this).attr("class", "seled");
        //当筛选条件中的 字母 选中状态变化时，向后台重新请求车型数据
        var brandChar = $(this).html();

        var articleId = $("#PKID").val();
        var postData = {
            Condition: brandChar,//默认传 A
            ArticleID:articleId
        };

        businessService.list(postData).success(function (response) {
            $("body").hideLoading();
            if (response.count == -1) {
                $("#divNoResult").removeClass("hidden");
                return false;
            } else {
                $("#divNoResult").addClass("hidden");
            }

            $scope.apiInfoList = response.items;
            vm.vehicles = response.items;
            if ($scope.apiInfoList.length < 1) {//|| $scope.apiInfoList[0].Id == null
                $scope.showTable = false;
                //$("#divNoResult").removeClass("hidden");
                $("#divNoSearchResult").removeClass("hidden");
            } else {
                $scope.showTable = true;
                $("#divNoResult").addClass("hidden");
                $("#divNoSearchResult").addClass("hidden");
                //$("#btnList").attr("style", "display:block");
            }

            //$("#loadModal").modal('hide');
            console.log("apiInfoList长度：" + $scope.apiInfoList.length + $scope.showTable);
        });

        //alert(RetSelecteds()); //返回选中结果  
    });


    $("#filter input[type='checkbox']").on('change', function () {


        $("body").showLoading();
        //$("#loadModal").modal({
        //    keyboard: false,
        //    show: true
        //});

        var brandChar = $("#filter a.seled").html();
        if (brandChar === null || brandChar == undefined) {
            brandChar = "";
            $("#filter a:first").attr("class", "seled");//默认选中A
        }
        console.log(brandChar);
        //$scope.getAllApiInfo();
        var articleId = $("#PKID").val();
        var postData = {
            Condition: brandChar,
            ArticleID: articleId
        };

        businessService.list(postData).success(function (response) {
            $("body").hideLoading();
            if (response.count == -1) {
                $("#divNoResult").removeClass("hidden");
                return false;
            } else {
                $("#divNoResult").addClass("hidden");
            }

            $scope.apiInfoList = response.items;
            vm.vehicles = response.items;

            if ($scope.apiInfoList.length < 1) { //|| $scope.apiInfoList[0].Id == null
                $scope.showTable = false;
                //$("#divNoResult").removeClass("hidden");
                $("#divNoSearchResult").removeClass("hidden");
            } else {
                $scope.showTable = true;
                $("#divNoResult").addClass("hidden");
                $("#divNoSearchResult").addClass("hidden");
                //$("#btnList").attr("style", "display:block");
            }
            //$("#loadModal").modal('hide');

            console.log("apiInfoList长度：" + $scope.apiInfoList.length + $scope.showTable);
        });

    });

    $("#btnCancel").click(function () {
        var r = confirm("取消修改所选车型将全部丢失，确认取消修改吗？");
        if (r === true) {
            var checkBoxArray = $(".search-list").find("input[type='checkbox']:checked");
            for (var i = 0; i < checkBoxArray.length; i++) {
                $(checkBoxArray[i]).prop("checked", false);
            }
            window.location.href = "/ConfigCategory/ArticleCategoryList?categoryid=" + $("#CategoryID").val();
        } else {
            return false;
        }

    });
}]);




//业务类
app.factory('BusinessService', ['$http', function ($http) {
    var list = function (postData) {
        return $http.post('/ConfigCategory/GetAllFourVehicleTypeInfoByParams', postData);
    };

    return {
        list: function (postData) {
            return list(postData);
        }
    };
}]);

function getPostDataParams() {
    var brandChar = $("#filter a.seled").html();
    if (brandChar == undefined) {
        brandChar = "";
    }

    var articleId = $("#PKID").val();

    var postData = {
        Condition: brandChar,//默认传 B
        ArticleID:articleId
    };

    return postData;
}

function fillLevelItems(rootLevels, levelItems, level) {
    _.each(levelItems, function (item) {
        var strNames = item.Name.split('$');
        var brand = strNames[0];
        var vehicle = strNames.length > 1 ? strNames[1] : "";
        var paiLiang = strNames.length > 2 ? strNames[2] : "";
        var nian = strNames.length > 3 ? strNames[3] : "";
        var salesName = strNames.length > 4 ? strNames[4] : "";
        var level1Item = null, level2Item = null, level3Item = null, level4Item = null, level5Item = null;
        if (level == "level1") {
            var existsItem1 = getExistsItem(rootLevels, brand, vehicle, paiLiang, nian, salesName, item.Level);

            if (!existsItem1 || existsItem1.length == 0) {
                rootLevels.push({ "Brand": brand, "Vehicle": vehicle, "PaiLiang": paiLiang, "Nian": nian, "SalesName": salesName, "Level": level });
            }
        }

        if (level == "level2") {
            level1Item = getExistsItem(rootLevels, brand, vehicle, paiLiang, nian, salesName, "level1");
            if (level1Item && level1Item.length > 0) {
                return;
            }

            var existsItem2 = getExistsItem(rootLevels, brand, vehicle, paiLiang, nian, salesName, item.Level);

            if (!existsItem2 || existsItem2.length == 0) {
                rootLevels.push({ "Brand": brand, "Vehicle": vehicle, "PaiLiang": paiLiang, "Nian": nian, "SalesName": salesName, "Level": level });
            }
        }

        if (level == "level3") {
            level1Item = getExistsItem(rootLevels, brand, vehicle, paiLiang, nian, salesName, "level1");
            if (level1Item && level1Item.length > 0) {
                return;
            }
            level2Item = getExistsItem(rootLevels, brand, vehicle, paiLiang, nian, salesName, "level2");
            if (level2Item && level2Item.length > 0) {
                return;
            }
            var existsItem3 = getExistsItem(rootLevels, brand, vehicle, paiLiang, nian, salesName, item.Level);

            if (!existsItem3 || existsItem3.length == 0) {
                rootLevels.push({ "Brand": brand, "Vehicle": vehicle, "PaiLiang": paiLiang, "Nian": nian, "SalesName": salesName, "Level": level });
            }
        }

        if (level == "level4") {
            level1Item = getExistsItem(rootLevels, brand, vehicle, paiLiang, nian, salesName, "level1");
            if (level1Item && level1Item.length > 0) {
                return;
            }
            level2Item = getExistsItem(rootLevels, brand, vehicle, paiLiang, nian, salesName, "level2");
            if (level2Item && level2Item.length > 0) {
                return;
            }
            level3Item = getExistsItem(rootLevels, brand, vehicle, paiLiang, nian, salesName, "level3");
            if (level3Item && level3Item.length > 0) {
                return;
            }
            var existsItem4 = getExistsItem(rootLevels, brand, vehicle, paiLiang, nian, salesName, item.Level);

            if (!existsItem4 || existsItem4.length == 0) {
                rootLevels.push({ "Brand": brand, "Vehicle": vehicle, "PaiLiang": paiLiang, "Nian": nian, "SalesName": salesName, "Level": level });
            }
        }

        if (level == "level5") {
            level1Item = getExistsItem(rootLevels, brand, vehicle, paiLiang, nian, salesName, "level1");
            if (level1Item && level1Item.length > 0) {
                return;
            }
            level2Item = getExistsItem(rootLevels, brand, vehicle, paiLiang, nian, salesName, "level2");
            if (level2Item && level2Item.length > 0) {
                return;
            }
            level3Item = getExistsItem(rootLevels, brand, vehicle, paiLiang, nian, salesName, "level3");
            if (level3Item && level3Item.length > 0) {
                return;
            }
            level4Item = getExistsItem(rootLevels, brand, vehicle, paiLiang, nian, salesName, "level4");
            if (level4Item && level4Item.length > 0) {
                return;
            }
            var existsItem5 = getExistsItem(rootLevels, brand, vehicle, paiLiang, nian, salesName, item.Level);

            if (!existsItem5 || existsItem5.length == 0) {
                rootLevels.push({ "Brand": brand, "Vehicle": vehicle, "PaiLiang": paiLiang, "Nian": nian, "SalesName": salesName, "Level": level });
            }
        }
    });
}

function getExistsItem(rootLevels, brand, vehicle, paiLiang, nian, salesName, level) {
    var existsItem = null;
    switch (level) {
        case 'level1':
            existsItem = _.filter(rootLevels, function (rootItem) {
                return rootItem.Brand == brand && rootItem.Level == level;
            });
            break;
        case 'level2':
            existsItem = _.filter(rootLevels, function (rootItem) {
                return rootItem.Brand == brand && rootItem.Vehicle == vehicle && rootItem.Level == level;
            });
            break;
        case 'level3':
            existsItem = _.filter(rootLevels, function (rootItem) {
                return rootItem.Brand == brand && rootItem.Vehicle == vehicle
                    && rootItem.PaiLiang == paiLiang && rootItem.Level == level;
            });
            break;
        case 'level4':
            existsItem = _.filter(rootLevels, function (rootItem) {
                return rootItem.Brand == brand && rootItem.Vehicle == vehicle
                    && rootItem.PaiLiang == paiLiang && rootItem.Nian == nian && rootItem.Level == level;
            });
            break;
        case 'level5':
            existsItem = _.filter(rootLevels, function (rootItem) {
                return rootItem.Brand == brand && rootItem.Vehicle == vehicle
                    && rootItem.PaiLiang == paiLiang && rootItem.Nian == nian
                    && rootItem.SalesName == salesName && rootItem.Level == level;
            });
            break;
    }
    return existsItem;
}

