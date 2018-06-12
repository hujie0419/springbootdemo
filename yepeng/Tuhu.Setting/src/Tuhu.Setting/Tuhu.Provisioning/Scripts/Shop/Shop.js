//根据不同的列表选项配置
var commonValue = {
    lngBegin: "", latBegin: "", province: "", city: "", district: "",
    serviceType: "", pageIndex: "", serviceId: "", productId: "", variantId: "", pageSize: "10", sort: "ShopRange"
};
var commonList = {
    type: "",
    serviceId: "",
    value: commonValue
};


//预约的排序方式为RecommendRange
var bookValue = ObjectCopy(commonValue);
bookValue.sort = "RecommendRange";
var bookList = ObjectCopy(commonList);
bookList.value = ObjectCopy(bookValue);


//车品需要传serviceId,不传serviceType
var carItemValue = ObjectCopy(bookValue);
carItemValue.serviceId= "FU-TM-QC|1";
var carItemList = ObjectCopy(commonList);
carItemList.value = carItemValue;

//喷漆需要传serviceId,不传serviceType
var paintValue = ObjectCopy(bookValue);
paintValue.serviceId = "FU-TUHU-LUTAI|1";
var paintList = ObjectCopy(commonList);
paintList.value = paintValue;


//初始化配置
var interfaceConfig = new Array();
interfaceConfig["all"] = GetList(commonList, "all", "");
interfaceConfig["tyre"] = GetList(commonList, "tyre", "TR");
interfaceConfig["maintain"] = GetList(commonList, "maintain", "BY");
interfaceConfig["install"] = GetList(commonList, "install", "FW");
interfaceConfig["refit"] = GetList(commonList, "refit", "GZ");
interfaceConfig["cosmetology"] = GetList(commonList, "cosmetology", "MR");
interfaceConfig["tyreBook"] = GetList(bookList, "tyreBook", "TR");
interfaceConfig["maintainBook"] = GetList(bookList, "maintainBook", "BY");
interfaceConfig["productBook"] = GetList(carItemList, "productBook", "");
interfaceConfig["paintBook"] = GetList(paintList, "paintBook", "");

var currentPage = 1;

//页面加载时获取省份
$(document).ready(function() {
    $.ajax({
        type: "GET",
        url: "/Shop/GetAllProvince",
        data: {},
        success: function(result) {
            if (result && result.Data) {
                for (var i = 0; i < result.Data.length; i++) {
                    $(province).append('<option value="' +
                        result.Data[i].RegionId +
                        '">' +
                        result.Data[i].RegionName +
                        '</option>');
                }
            }

        }
    });
});

//获取省份后二级联动获取市
function CityList(obj) {
    $.ajax({
        type: "GET",
        url: "/Shop/GetRegionByRegionId",
        data: { "regionId": obj.value },
        success: function (result) {
            $("#city").empty();
            $("#district").empty();
            $(district).append('<option value="' + (-1) + '">' + "选择区" + '</option>');
            $(city).append('<option value="' + (-1) + '">' + "选择市" + '</option>');
            if (!(result && result.Data && result.Data.ChildRegions)) {
                return null;
            }
            for (var i = 0; i < result.Data.ChildRegions.length; i++) {
                if (isExistOption('city', result.Data.ChildRegions[i].CityId) === false) {
                    $(city).append('<option value="' +
                        result.Data.ChildRegions[i].CityId +
                        '">' +
                        result.Data.ChildRegions[i].CityName +
                        '</option>');
                }
            }

        }
    });
};

//获取省市后三级联动获取区
function DistrictList(obj) {
    $.ajax({
        type: "GET",
        url: "/Shop/GetRegionByRegionId",
        data: { "regionId": obj.value },
        success: function (result) {
            $("#district").empty();
            $(district).append('<option value="' + (-1) + '">' + "选择区" + '</option>');
            if (!(result && result.Data && result.Data.ChildRegions)) {
                return null;
            }
            for (var i = 0; i < result.Data.ChildRegions.length; i++) {
                if (isExistOption('district', result.Data.ChildRegions[i].DistrictId) === false) {
                    $(district).append('<option value="' +
                        result.Data.ChildRegions[i].DistrictId +
                        '">' +
                        result.Data.ChildRegions[i].DistrictName +
                        '</option>');
                }
            }
        }
    });
};

//搜索
function SearchList(page) {
    var lngBegin = "";
    var latBegin = "";
    if ($("#LngLat").val().length > 0) {
        var lngAndLat = $("#LngLat").val().split(",", 2);
        lngBegin = lngAndLat[1];
        latBegin = lngAndLat[0];
        if (typeof (lngBegin) === "undefined" || typeof (latBegin) === "undefined") {
            alert("经纬度错误，请检查");
            return false;
        }
    }
    var matchedConfig = interfaceConfig[$("#serviceType").val()];
    var par = matchedConfig.value;
    par.serviceType = matchedConfig.serviceId;
    par.lngBegin = lngBegin;
    par.latBegin = latBegin;
    par.province = $("#province").val() > 0 ? $("#province").find("option:selected").text() : "";
    par.city = $("#city").val() > 0 ? $("#city").find("option:selected").text() : "";
    par.district = $("#district").val() > 0 ? $("#district").find("option:selected").text() : "";
    par.pageIndex = page;
    $.ajax({
        type: "GET",
        contentType: "application/x-www-form-urlencoded; charset=UTF-8",
        url: "/Shop/SearchShopModel",
        data: par
        ,
        success: function (result) {
            if (result && result.Shops) {
                WriteTable(page, par.pageSize, result);
                currentPage = page;
                ShowPage(page, result.TotalPage);
            }
            else {
                $("#listdata>tbody>tr:has(td)").text("");
                $("#currPage").hide();
                $("#nextPage").hide();
                $("#prevPage").hide();
                $("#jumpPage").hide();

            }

        }
    });
};

//直辖市重复处理
function isExistOption(optionId, optionValue) {

    var isExist = false;
    var count = $('#' + optionId).find('option').length;
    for (var i = 0; i < count; i++) {
        if ($('#' + optionId).get(0).options[i].value == optionValue)
        { isExist = true; break; }
    }
    return isExist;
};

function WriteTable(page, pageSize, result) {
    $("#listdata>tbody>tr:has(td)").text("");
    for (var i = 0; i < result.Shops.length; i++) {
        var html = $("#Template").html();
        html = html.replace('$id$', ((i + 1) + (page - 1) * pageSize)).replace("$name$", result.Shops[i].CarparName)
            .replace("$serviceType$", $("#serviceType").find("option:selected").text())
            .replace("$distance$",  GetTopRecRanging(result.Shops[i]))
            .replace("$BIWeight$", result.Shops[i].DistanceWeight)
            .replace("$BIGrade$", result.Shops[i].Grade)
            .replace("$comprehensiveScore$",result.Shops[i].ComprehensiveScore)
            .replace("$coorDistance$", result.Shops[i].Distance.toFixed(3) + "  km")
            .replace("$tyreLevel$", result.Shops[i].ShopLevel)
            .replace("$comments$", GetComments(result.Shops[i].CommentRate, result.Shops[i].CommentTimes, result.Shops[i].InstallQuantity))
            .replace("$coverRegion$", GetCoverRegion(result.Shops[i].CoverRegion))
            .replace("$district$", GetRegion(result.Shops[i]))
            .replace("$address$", result.Shops[i].Address)
            .replace("$suspendStatus$", result.Shops[i].IsSuspend ? "暂停营业" : "营业中");       
        $("#listdata").append(html);
    }
};

function ShowPage(currentPage, totalPage) {
    $("#currPage").show();
    $("#currPage").empty();
    var currPageString = "当前第" + currentPage + "页" + " / 共" + totalPage + "页";
    $("#currPage").append(currPageString);
    $("#nextPage").show();
    $("#prevPage").show();
    if (currentPage >= totalPage) {
        $("#nextPage").hide();
    }
    if (currentPage < 2) {
        $("#prevPage").hide();
    }
    $("#jumpPage").show();
    $("#jumpPage").empty();
    $("#jumpPage").append('<option value="-1">跳转到</option>');
    for (var pageI = 1; pageI <= totalPage; pageI++) {
        $("#jumpPage").append('<option value="' + pageI + '">' + "第" + pageI + "页" + '</option>');
    }
};

function RightPage() {
    currentPage = currentPage + 1;
    SearchList(currentPage);
};

function LeftPage() {
    currentPage = currentPage - 1;
    SearchList(currentPage);
};

function JumpPage(jumpPage) {
    var tempPage = parseInt(jumpPage.value);
    if (tempPage > 0) {
        currentPage = tempPage;
        SearchList(currentPage);
    }
};

function GetTopRecRanging(obj) {
    var topRecRang = "";
    if ($("#serviceType").find("option:selected").hasClass("book")) {
        if (obj.ShopRecommendRange > 0) {
            topRecRang = "推荐:" + obj.ShopRecommendRange.toFixed(3) + "km";
        }
    }
    else {
        if (obj.ShopRang && obj.ShopRang > 0 && obj.IsTop) {
            topRecRang = "置顶:" + obj.ShopRang.toFixed(3) + "km";
        }
    }
    return topRecRang;
};

function GetComments(commentRate, commentTimes, installQuantity) {
    var comment = "";
    if (commentRate !== null && commentRate !== "undefined" && commentRate !== 0) {
        comment += "评价等级：" + commentRate + '<br>';
    }
    if (commentTimes !== null && commentTimes !== "undefined" && commentTimes !== 0) {
        comment += "评价数量:" + commentTimes + '<br>';
    }
    if (installQuantity !== null && installQuantity !== "undefined" && installQuantity !== 0) {
        comment += "订单数量:" + installQuantity;
    }
    return comment;
};

function GetRegion(shop) {
    var region = new Array();
    if (shop.Province) {
        region.push(shop.Province);
    }
    if (shop.City) {
        region.push(shop.City);
    }
    if (shop.District) {
        region.push(shop.District);
    }
    return region.join('/')    
}

function GetCoverRegion(coverRegion) {
    var coverRegionString = "";
    if (coverRegion.CoverProvince && coverRegion.CoverCity && coverRegion.CoverDistrict) {
        var coverProvinceString = coverRegion.CoverProvince.join(',');
        var coverCityString = coverRegion.CoverCity.join(',');
        var coverDistrictString = coverRegion.CoverDistrict.join(',');
        if (coverDistrictString != "" && coverCityString != "" && coverProvinceString != "") {
            coverRegionString = "省:" +
                coverProvinceString +
                "<br>" +
                "市:" +
                coverCityString +
                "<br>" +
                "区:" +
                coverDistrictString +
                "<br>";
        }
    }
    return coverRegionString;
};

function GetList(list, type, serviceId) {
    var tempList = ObjectCopy(list);
    tempList.type = type;
    tempList.serviceId = serviceId;
    return tempList;
};

function ObjectCopy(obj) {
    var newObj = {};
    if (obj instanceof Array) {
        return null;
    }
    for (var key in obj) {
        var keyobj = obj[key];
        newObj[key] = typeof keyobj === "object" ? ObjectCopy(keyobj) : keyobj;
    }
    return newObj;
};