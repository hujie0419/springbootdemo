var allVehicleBrands = [];
var allProvince = [];

$(document).ready(function () {
    GetAllVehicleBrands();
    GetAllProvince();
});
//toll
function getFormJson(frm) {
    var o = {};
    var a = $(frm).serializeArray();
    $.each(a, function () {
        if (o[this.name] !== undefined) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });
    return o;
}

function getValuesByData_type(frm, data_type) {
    var items = $(frm).children();
    var res = "";
    $.each(items, function (index, item) {
        if ($(item).attr("data-type") == data_type) {
            var val = $(item).children().first().next().children().first().children('option:selected').val();
            if (val == undefined){
                val = $(item).children().first().children().first().children('option:selected').val();
            }
            if(val){
            res += val + ",";
            }
        }
    })
    return res;

}

function removeRegion(obj) {
    $(obj).parent().parent().remove();
}

//匹配H5活动页标题
function findTitle() {
    var url = $("#UrlForm #targetUrl").val();
    $.ajax({
        url: "/CityActivityDisplay/GetActivityTitleByUrl",
        type: "GET",
        data: { url: url },
        success: function (result) {
            if (!result.Status)
                alert(result.Msg);
            else
                $("#UrlForm #h5UrlName").val(result.Msg || "未能匹配到名称");
        }
    })
}

//匹配小程序活动页标题
function findWxTitle() {
    var url = $("#UrlForm #wxappUrl").val();
    $.ajax({
        url: "/CityActivityDisplay/GetActivityTitleByUrl",
        type: "GET",
        data: { url: url },
        success: function (result) {
            if (!result.Status)
                alert(result.Msg);
            else
                $("#UrlForm #wxUrlName").val(result.Msg || "未能匹配到名称");
        }
    })
}

function search() {
    var frm = $("#searchForm");
    var data = getFormJson(frm);
    window.location.href = "/CityActivityDisplay/Index?ActivityName=" + data["activityName"] + "&StartTime=" + data["startTime"] + "&endTime=" + data["endTime"] + "&createUser=" + data["createUser"]
        + "&updateUser=" + data["updateUser"] + "&status=" + data["status"] + "&pageIndex=1";
}
//region

function GetCitysByProvinceId(obj, cityId) {
    var $this = $(obj);
    var $region = $this.parent().next().children("select");
    $region.html("");
    var provicneId = $this.val();
    if (provicneId == 1 || provicneId == 2 || provicneId == 19 || provicneId == 20)
        $this.parent().next().children().attr("disabled", "disabled");
    else
        $this.parent().next().children().removeAttr("disabled");
    if (!provicneId) {
        $region.html("<option value=''>--请选择--</option>");
        return;
    }
    var template = "<option value='$value$'>$text$</option>";
    var template2 = "<option value='$value$' selected='selected'>$text$</option>";
    cityId = cityId || 0;
    $.ajax({
        url: "/CarInsurance/GetRegionByName",
        type: "POST",
        data: { provinceId: provicneId },
        success: function (regions) {
            var html = "";
            $.each(regions, function (index, region) {
                if (cityId != region.CityId) {
                    html += template.replace("$value$", region.CityId).replace("$text$", region.CityName);
                }
                else {
                    html += template2.replace("$value$", region.CityId).replace("$text$", region.CityName);
                }
            });
            $region.append(html);
        }
    });
}

function addRegion(btn) {
    if ($("#setDefaultPageCheckBox").attr("checked") == "checked") {
        $("#confirmDialog").html("默认页面默认所有地区有效<br>不支持添加地区配置");
        $("#confirmDialog").dialog({
            title: "提示",
            resizable: false,
            height: "auto",
            width: 300,
            modal: true,
            buttons: {
                "确定": function () {
                    $(this).dialog("close");
                },
                "取消": function () {
                    $(this).dialog("close");
                }
            }
        })
    }
    else {
        var $parent = $(btn).parent().parent().parent();
        var template = "<option value='$value$'>$text$</option>"
        var options = "<option value=''>--请选择--</option>";
        $.each(allProvince, function (index, province) {
            options += template.replace("$value$", province.ProvinceId).replace("$text$", province.ProvinceName);
        });
        var html = "<div class='form-group' data-type='region'><div class='col-sm-4'><select onchange='GetCitysByProvinceId(this)'>" + options +
            "</select></div><div class='col-sm-4'><select><option value=''>--请选择--</option></select></div><div class='col-sm-4'><button class='btn btn-default' onclick='removeRegion(this)'>删除</button></div></div>";
        $parent.append(html);
    }
}

function fillRegion(provinceId, cityId) {
    var $form = $("#UrlForm");
    var template = "<option value='$value$'>$text$</option>";
    var template1 = "<option value='$value$' selected='selected'>$text$</option>";

    var options = "<option value=''>--请选择--</option>";
    $.each(allProvince, function (index, province) {
        if (province.ProvinceId != provinceId) {
            options += template.replace("$value$", province.ProvinceId).replace("$text$", province.ProvinceName);
        }
        else {
            options += template1.replace("$value$", province.ProvinceId).replace("$text$", province.ProvinceName);
        }
    });
    var $div = $("<div class='form-group' data-type='region'><div class='col-sm-4'><select onchange='GetCitysByProvinceId(this)'>" + options +
        "</select></div><div class='col-sm-4'><select><option value=''>--请选择--</option></select></div><div class='col-sm-4'><button class='btn btn-default' onclick='removeRegion(this)'>删除</button></div></div>");
    $form.append($div);
    GetCitysByProvinceId($div.find('select')[0], cityId);
}


//vehicle
function GetVehicleByBrand(obj, productId) {
    var $this = $(obj);
    var $vehicle = $this.parent().next().children("select");
    $vehicle.html("");
    var brand = $this.val();
    if (!brand) {
        $vehicle.html("<option value=''>--请选择--</option>");
        return;
    }
    var template = "<option value='$value$'>$text$</option>";
    var template2 = "<option value='$value$' selected='selected'>$text$</option>";
    productId = productId || 0;
    $.ajax({
        url: "/CityActivityDisplay/GetVehicleByBrand",
        type: "GET",
        data: { brand: brand },
        success: function (brands) {
            var html = "";
            $.each(brands, function (index, brand) {
                if (productId != brand.ProductID) {
                    html += template.replace("$value$", brand.ProductID).replace("$text$", brand.Vehicle);
                }
                else {
                    html += template2.replace("$value$", brand.ProductID).replace("$text$", brand.Vehicle);
                }
            });
            $vehicle.append(html);
        }
    });
}

function fillVehicle(brand, productId) {
    var $form = $("#UrlForm");
    var template = "<option value='$value$'>$text$</option>";
    var template1 = "<option value='$value$' selected='selected'>$text$</option>";
    var options = "<option value=''>--请选择--</option>";
    $.each(allVehicleBrands, function (index, vehicle) {
        if (vehicle.Brand != brand) {
            options += template.replace("$value$", vehicle.Brand).replace("$text$", vehicle.Brand);
        }
        else {
            options += template1.replace("$value$", vehicle.Brand).replace("$text$", vehicle.Brand);
        }
    });
    var $div = $("<div class='form-group' data-type='vehicle'><div class='col-sm-4'><select onchange='GetVehicleByBrand(this)'>" + options +
        "</select></div><div class='col-sm-4'><select><option value=''>--请选择--</option></select></div><div class='col-sm-4'><button class='btn btn-default' onclick='removeRegion(this)'>删除</button></div></div>");
    $form.append($div);
    GetVehicleByBrand($div.find('select')[0], productId);
}

function addVehicle(btn) {
    if ($("#setDefaultPageCheckBox").attr("checked") == "checked") {
        $("#confirmDialog").html("默认页面默认对所有车型有效<br>不支持添加车型配置");
        $("#confirmDialog").dialog({
            title: "提示",
            resizable: false,
            height: "auto",
            width: 300,
            modal: true,
            buttons: {
                "确定": function () {
                    $(this).dialog("close");
                },
                "取消": function () {
                    $(this).dialog("close");
                }
            }
        })
    }
    else {
        var $parent = $(btn).parent().parent().parent();
        var template = "<option value='$value$'>$text$</option>"
        var options = "<option value=''>--请选择--</option>";
        $.each(allVehicleBrands, function (index, brand) {
            options += template.replace("$value$", brand.Brand).replace("$text$", brand.Brand);
        });
        var html = "<div class='form-group' data-type='vehicle'><div class='col-sm-4'><select onchange='GetVehicleByBrand(this)'>" + options +
            "</select></div><div class='col-sm-4'><select><option value=''>--请选择--</option></select></div><div class='col-sm-4'><button class='btn btn-default' onclick='removeRegion(this)'>删除</button></div></div>";
        $parent.append(html);
    }
}


//activityUrl
function addActivityUrl() {
    $("#setDefaultPageCheckBox").prop('checked', false);
    $("#UrlForm #targetUrl").val("");
    $("#UrlForm #h5UrlName").val("");
    $("#UrlForm #wxappUrl").val("");
    $("#UrlForm #wxUrlName").val("");
    $("#UrlForm [data-type='region']").remove();
    $("#UrlForm [data-type='vehicle']").remove();
    $("#saveUrlBtn").html("保存");
    $("#saveUrlBtn").attr("onclick", "saveUrl()");
    if ($("#typeSelect").val() == "请选择") {
        $("#confirmDialog").html("请先选择活动类型");
        $("#confirmDialog").dialog({
            title: "提示",
            resizable: false,
            height: "auto",
            width: 300,
            modal: true,
            buttons: {
                "确定": function () {
                    $(this).dialog("close");
                },
                "取消": function () {
                    $(this).dialog("close");
                }
            }
        })
    }
    else {
        $("#activityUrlDialog #activityId").val($("#activityBaseInfoForm #activityId").val());
        //如果是要更新活动,弹出地址配置窗口
        if ($("#saveActivityBtn").html() == "更新") {
            if ($("#typeSelect").val() == "Region") {
                $("#addRegionVehicleBtn").html("添加地区");
                $("#addRegionVehicleBtn").attr("onclick", "addRegion(this)");
                $("#activityUrlDialog").dialog({
                    title: "添加活动页面",
                    modal: true,
                    width: 500
                });
            }
            else {
                $("#addRegionVehicleBtn").html("添加车型");
                $("#addRegionVehicleBtn").attr("onclick", "addVehicle(this)");
                $("#activityUrlDialog").dialog({
                    title: "添加活动页面",
                    modal: true,
                    width: 500
                });
            }
        }
        //如果新建活动，先检测字段，保存活动基本信息，然后弹出地址配置窗口
        else {
            $("#confirmDialog").html("保存活动基本信息后才能添加URL<br>确定保存?");
            $("#confirmDialog").dialog({
                title: "提示",
                resizable: false,
                height: "auto",
                width: 300,
                modal: true,
                buttons: {
                    "确定": function () {
                        $(this).dialog("close");
                        var frm = $("#activityBaseInfoForm");
                        var data = getFormJson(frm);
                        if (data['startTime'] == "" || data["endTime"] == "") {
                            alert("起止时间字段不能为空");
                            return;
                        }
                        else {
                            $.ajax({
                                url: "/CityActivityDisplay/CreateActivity",
                                type: "POST",
                                data: data,
                                success: function (result) {
                                    if (!result.Status)
                                        alert(result.Msg);
                                    else {
                                        alert("创建活动成功");
                                        $("#activityBaseInfoForm #activityId").val(result.Data);
                                        $("#saveActivityBtn").html("更新");
                                        $("#saveActivityBtn").attr("onclick", "updateActivity()");
                                        $("#activityBaseInfoForm #activityId").attr("disabled", "disabled");
                                        $("#activityBaseInfoForm #typeSelect").attr("disabled", "disabled");
                                        $("#activityBaseInfoForm #typeSelect").attr("style", "background-color:#CCC");

                                        if ($("#typeSelect").val() == "Region") {
                                            $("#addRegionVehicleBtn").html("添加地区");
                                            $("#addRegionVehicleBtn").attr("onclick", "addRegion(this)");
                                            $("#activityUrlDialog").dialog({
                                                title: "添加活动页面",
                                                modal: true,
                                                width: 500
                                            });
                                        }
                                        else if ($("#typeSelect").val() == "Vehicle") {
                                            $("#addRegionVehicleBtn").html("添加车型");
                                            $("#addRegionVehicleBtn").attr("onclick", "addVehicle(this)");
                                            $("#activityUrlDialog").dialog({
                                                title: "添加活动页面",
                                                modal: true,
                                                width: 500
                                            });

                                        }
                                    }
                                }
                            })
                        }
                    },
                    "取消": function () {
                        $(this).dialog("close");
                    }
                }
            })
        }
    }
}

//编辑默认页
function editDefaultUrl(btn) {
    //点击编辑时
    if ($(btn).html() == "编辑") {
        var Url = $(btn).parent().prev().prev();
        var h5Url = $(Url).children().first();
        var wxUrl = $(Url).children().last();
        var template1 = "<textarea id = '$h5DefaultUrl$' type='text' data-value='$value$'>$value$</textarea>";
        var h5Href = h5Url.children().first().attr("href") || "";
        var wxHref = wxUrl.children().first().attr("href") || "";
        $(h5Url).html(
            template1.replace("$h5DefaultUrl$", "h5DefaultUrl")
                .replace("$value$", h5Href).replace("$value$", h5Href));
        $(wxUrl).html(
            template1.replace("$h5DefaultUrl$", "wxDefaultUrl")
                .replace("$value$", wxHref).replace("$value$", wxHref));
        $(btn).html("保存");
    }
    //点击保存
    else {
        //更新默认配置
        $("#confirmDialog").html("确定保存默认页面地址");
        $("#confirmDialog").dialog({
            title: "提示",
            resizable: false,
            height: "auto",
            width: 300,
            modal: true,
            buttons: {
                "确定": function () {
                    $(this).dialog("close");
                    var data = {};
                    var activityId = $("#activityBaseInfoForm #activityId").val();
                    var h5OldUrl = $("#h5DefaultUrl").attr("data-value");
                    var wxOldUrl = $("#wxDefaultUrl").attr("data-value");
                    var targetUrl = $("#h5DefaultUrl").val();
                    var wxappUrl = $("#wxDefaultUrl").val();
                    if (!targetUrl && !wxappUrl) {
                        alert("活动页地址不能全为空");
                    }
                    else {
                        data["activityId"] = activityId;
                        data["targetUrl"] = targetUrl;
                        data["wxappUrl"] = wxappUrl;
                        data["isDefault"] = 1;
                        var regionIds = "";
                        var vehicleIds = "";
                        var oldUrl = oldUrl;
                        $.ajax({
                            url: "/CityActivityDisplay/UpdateActivityUrl",
                            type: "POST",
                            data: { model: data, h5OldUrl: h5OldUrl, wxOldUrl: wxOldUrl, vehicleIds: vehicleIds, regionIds: regionIds },
                            success: function (result) {
                                if (result.Status) {
                                    var activityId = $("#activityBaseInfoForm #activityId").val();
                                    setTimeout(function () { GetActivityUrlConfig(activityId) }, 2000);
                                    setTimeout(function () { RefreshCache(activityId) }, 2000);
                                }
                                else {
                                    alert("更新默认页失败" + (result.Msg || ""));
                                    return;
                                }
                            }
                        })
                    }
                },
                "取消": function () {
                    var activityId = $("#activityBaseInfoForm #activityId").val();
                    GetActivityUrlConfig(activityId);
                    $(this).dialog("close");
                }
            }
        })
    }
}

//打开编辑常规活动页对话框
function editUrl(btn) {
    $("#setDefaultPageCheckBox").removeAttr("cheched");
    var activityId = $("#activityBaseInfoForm #activityId").val();
    var targetUrl = $(btn).parent().prev().prev().children().first().text().trim() || "";
    var wxappUrl = $(btn).parent().prev().prev().children().last().text().trim() || "";
    var h5UrlName = $(btn).parent().prev().children().first().html();
    var wxUrlName = $(btn).parent().prev().children().last().html();
    $("#activityUrlDialog #activityId").val(activityId);
    $("#UrlForm #targetUrl").val(targetUrl);
    $("#UrlForm #targetUrl").attr("data-oldUrl", targetUrl);
    $("#UrlForm #wxappUrl").val(wxappUrl);
    $("#UrlForm #wxappUrl").attr("data-oldUrl", wxappUrl);
    $("#UrlForm #h5UrlName").val(h5UrlName);
    $("#UrlForm #wxUrlName").val(wxUrlName);
    $("#UrlForm [data-type='region']").remove();
    $("#UrlForm [data-type='vehicle']").remove();
    $("#saveUrlBtn").html("更新");
    $("#saveUrlBtn").attr("onclick", "updateUrl()");

    //类型为地区
    if ($("#typeSelect").val() == "Region") {
        $("#addRegionVehicleBtn").html("添加地区");
        $("#addRegionVehicleBtn").attr("onclick", "addRegion(this)");
        $("#activityUrlDialog").dialog({
            title: "编辑活动页面",
            modal: true,
            width: 500
        });

        $.ajax({
            url: "/CityActivityDisplay/GetRegionInfoByActivityIdUrl",
            type: "GET",
            data: { activityId: activityId, targetUrl: targetUrl, wxappUrl: wxappUrl },
            success: function (result) {
                if (!result.Status)
                    alert(result.Msg);
                else {
                    regions = result.Data;
                    $.each(regions, function (index, region) {
                        fillRegion(region.ProvinceId, region.CityId);
                    })
                }
            }
        })
    }
    //类型为车型
    else {
        $("#addRegionVehicleBtn").html("添加车型");
        $("#addRegionVehicleBtn").attr("onclick", "addVehicle(this)");
        $("#activityUrlDialog").dialog({
            title: "编辑活动页面",
            modal: true,
            width: 500
        });
        $.ajax({
            url: "/CityActivityDisplay/GetVehicleInfoByActivityIdUrl",
            type: "GET",
            data: { activityId: activityId, targetUrl: targetUrl, wxappUrl: wxappUrl },
            success: function (result) {
                if (!result.Status)
                    alert(result.Msg);
                else {
                    vehicles = result.Data;
                    $.each(vehicles, function (index, vehicle) {
                        fillVehicle(vehicle.Brand, vehicle.ProductID);
                    })
                }
            }
        })
    }
}

//删除活动页
function delateUrl(btn) {
    $("#confirmDialog").html("确认删除？");
    $("#confirmDialog").dialog({
        title: "提示",
        resizable: false,
        height: "auto",
        width: 300,
        modal: true,
        buttons: {
            "确定": function () {
                $(this).dialog("close");
                var activityId = $("#activityBaseInfoForm #activityId").val();
                var targetUrl = $(btn).parent().prev().prev().children().first().text().trim();
                var wxappUrl = $(btn).parent().prev().prev().children().last().text().trim();
                $.ajax({
                    url: "/CityActivityDisplay/DeleteActivityUrl",
                    type: "POST",
                    data: { activityId: activityId, targetUrl: targetUrl, wxappUrl: wxappUrl },
                    success: function (result) {

                        if (!result.Status)
                            alert(result.Msg);
                        else {
                            alert("删除成功");
                            $(btn).parent().parent().remove();
                            RefreshCache(activityId);
                        }
                    }
                })
            },
            "取消": function () {
                $(this).dialog("close");
            }
        }
    })
}

//更新常规活动页
function updateUrl() {
    $("#confirmDialog").html("确定更新?");
    $("#confirmDialog").dialog({
        title: "提示",
        resizable: false,
        height: "auto",
        width: 300,
        modal: true,
        buttons: {
            "确定": function () {
                $(this).dialog("close");
                var data = {};
                var frm = $("#UrlForm");
                var isDefault = 0;
                if ($("#setDefaultPageCheckBox").attr("checked") == "checked")
                    isDefault = 1;
                data["activityId"] = $("#UrlForm #activityId").val();
                data["targetUrl"] = $("#UrlForm #targetUrl").val();
                data["wxappUrl"] = $("#UrlForm #wxappUrl").val();
                data["isDefault"] = isDefault;
                var h5OldUrl = $("#UrlForm #targetUrl").attr("data-oldUrl").trim();
                var wxOldUrl = $("#UrlForm #wxappUrl").attr("data-oldUrl").trim();
                var vehicleIds = getValuesByData_type(frm, "vehicle");
                var regionIds = getValuesByData_type(frm, "region");
                if (!data["targetUrl"] && !data["wxappUrl"]) {
                    alert("活动页地址不能全为空");
                    return;
                }
                if (data["isDefault"] !=1 && !(vehicleIds || regionIds)) {
                    alert("车型或地区不能为空");
                    return;
                }
                data["targetUrl"] = data["targetUrl"].trim();
                data["wxappUrl"] = data["wxappUrl"].trim();
                $.ajax({
                    url: "/CityActivityDisplay/UpdateActivityUrl",
                    type: "POST",
                    data: { model: data, h5OldUrl: h5OldUrl, wxOldUrl: wxOldUrl, vehicleIds: vehicleIds, regionIds: regionIds },
                    success: function (result) {
                        if (!result.Status)
                            alert(result.Msg);
                        else {
                            alert("更新成功");
                            $("#activityUrlDialog").dialog("close");
                            var activityId = $("#activityBaseInfoForm #activityId").val();
                            setTimeout(function () { GetActivityUrlConfig(activityId) }, 2000);
                            setTimeout(function () { RefreshCache(activityId) }, 2000);
                        }
                    }
                });
            },
            "取消": function () {
                $(this).dialog("close");
            }
        }
    })
}

function saveUrl() {
    $("#confirmDialog").html("确定保存?");
    $("#confirmDialog").dialog({
        title: "提示",
        resizable: false,
        height: "auto",
        width: 300,
        modal: true,
        buttons: {
            "确定": function () {
                $(this).dialog("close");
                var data = {};
                var frm = $("#UrlForm");
                var isDefault = 0;
                if ($("#setDefaultPageCheckBox").attr("checked") == "checked")
                    isDefault = 1;
                data["activityId"] = $("#activityBaseInfoForm #activityId").val();
                data["targetUrl"] = $("#UrlForm #targetUrl").val();
                data["wxappUrl"] = $("#UrlForm #wxappUrl").val();
                data["isDefault"] = isDefault;
                var vehicleIds = getValuesByData_type(frm, "vehicle");
                var regionIds = getValuesByData_type(frm, "region");
                if (data["isDefault"] !=1 && !(vehicleIds || regionIds)) {
                    alert("车型或地区不能为空");
                    return;
                }
                if (!data["targetUrl"] && !data["wxappUrl"]) {
                    alert("活动地址不能全为空");
                    return;
                }
                data["targetUrl"] = data["targetUrl"].trim();
                data["wxappUrl"] = data["wxappUrl"].trim();
                $.ajax({
                    url: "/CityActivityDisplay/CreateActivityUrl",
                    type: "POST",
                    data: { model: data, vehicleIds: vehicleIds, regionIds: regionIds },
                    success: function (result) {
                        if (!result.Status)
                            alert(result.Msg);
                        else {
                            var activityId = $("#activityBaseInfoForm #activityId").val();
                            setTimeout(function () { GetActivityUrlConfig(activityId) }, 2000);
                            $("#activityUrlDialog").dialog("close");
                            setTimeout(function () { RefreshCache(activityId) }, 2000);
                        }
                    }
                });
            },
            "取消": function () {
                $(this).dialog("close");
            }
        }
    })
}

//activity

function saveActivity() {
    var frm = $("#activityBaseInfoForm");
    var data = getFormJson(frm);
    if ($("#typeSelect").val() == "请选择") {
        $("#confirmDialog").html("请先选择活动类型");
        $("#confirmDialog").dialog({
            title: "提示",
            resizable: false,
            height: "auto",
            width: 300,
            modal: true,
            buttons: {
                "确定": function () {
                    $(this).dialog("close");
                },
                "取消": function () {
                    $(this).dialog("close");
                }
            }
        })
    }
    else if (data['startTime'] == "" || data["endTime"] == "") {
        alert("起止时间字段不能为空");
        return;
    }
    else {
        $("#confirmDialog").html("确定保存?");
        $("#confirmDialog").dialog({
            title: "提示",
            resizable: false,
            height: "auto",
            width: 300,
            modal: true,
            buttons: {
                "确定": function () {
                    $(this).dialog("close");
                    $.ajax({
                        url: "/CityActivityDisplay/CreateActivity",
                        type: "POST",
                        data: data,
                        success: function (result) {
                            if (!result.Status)
                                alert(result.Msg);
                            else {
                                alert("活动创建成功");
                                $("#activityBaseInfoForm #activityId").val(result.Data);
                                $("#activityBaseInfoForm #activityId").attr("disabled", "disabled");
                                $("#activityBaseInfoForm #typeSelect").attr("disabled", "disabled");
                                $("#activityBaseInfoForm #typeSelect").attr("style", "background-color:#CCC")
                                $("#saveActivityBtn").html("更新");
                                var activityId = $("#activityBaseInfoForm #activityId").val();
                                $.ajax({
                                    url: "/CityActivityDisplay/RefreshRegionVehicleIdActivityUrlCache",
                                    type: "GET",
                                    data: { activityId: activityId },
                                    success: function (result) {
                                        if (!result.Status)
                                            alert("更新缓存失败:" + result.Msg);
                                        window.location.href = window.location.href;
                                    }
                                });
                            }
                        }
                    })
                },
                "取消": function () {
                    $(this).dialog("close");
                }
            }
        })
    }
}

function deleteActivity(btn) {
    $("#confirmDialog").html("确认删除？");
    $("#confirmDialog").dialog({
        title: "提示",
        resizable: false,
        height: "auto",
        width: 300,
        modal: true,
        buttons: {
            "确定": function () {
                var activityId = $(btn).parent().parent().children().first().next().next().attr("data-activityId");
                $.ajax({
                    url: "/CityActivityDisplay/DeleteActivity",
                    type: "POST",
                    data: { activityId, activityId },
                    success: function (result) {
                        if (!result.Status)
                            alert(result.Msg);
                        else {
                            alert("活动已删除");
                            $(btn).parent().parent().remove();
                            $.ajax({
                                url: "/CityActivityDisplay/RefreshRegionVehicleIdActivityUrlCache",
                                type: "GET",
                                data: { activityId: activityId },
                                success: function (result) {
                                    if (!result.Status)
                                        alert("更新缓存失败:" + result.Msg);
                                }
                            });
                        }
                    }
                })
                $(this).dialog("close");
            },
            "取消": function () {
                $(this).dialog("close");
            }
        }
    })
}

function updateActivity() {
    var activityId = $("#activityBaseInfoForm #activityId").val();
    var activityName = $("#activityBaseInfoForm #activityName").val();
    var startTime = $("#activityBaseInfoForm #startTime").val();
    var endTime = $("#activityBaseInfoForm #endTime").val();
    var isEnabled = $("#activityBaseInfoForm #isEnabled").val();
    if (activityName == "")
        alert("活动名不为空");
    else if (startTime == "")
        alert("活动开始时间不能为空");
    else if (endTime == "")
        alert("活动结束时间不能为空");
    else {
        $("#confirmDialog").html("确定保存活动内容?");
        $("#confirmDialog").dialog({
            title: "提示",
            resizable: false,
            height: "auto",
            width: 300,
            modal: true,
            buttons: {
                "确定": function () {
                    $(this).dialog("close");
                    var data = {};
                    data["activityId"] = activityId;
                    data["activityName"] = activityName;
                    data["startTime"] = startTime;
                    data["endTime"] = endTime;
                    data["isEnabled"] = isEnabled;
                    $.ajax({
                        url: "/CityActivityDisplay/UpdateActivity",
                        type: "POST",
                        data: data,
                        success: function (result) {
                            if (!result.Status)
                                alert(result.Msg || result.msg);
                            else {
                                alert("更新成功")
                                $.ajax({
                                    url: "/CityActivityDisplay/RefreshRegionVehicleIdActivityUrlCache",
                                    type: "GET",
                                    data: { activityId: activityId },
                                    success: function (result) {
                                        if (!result.Status)
                                            alert("更新缓存失败:" + result.Msg);
                                        window.location.href = window.location.href;
                                    }
                                });
                            }

                        }
                    })
                },
                "取消": function () {
                    $(this).dialog("close");
                }
            }
        })
    }


}

function editActivity(btn) {
    $(CreateActivityDialog).dialog({ title: '编辑活动', width: 1000, modal: true, height: "auto" });
    var tr = $(btn).parent().parent();
    var id = $(tr).children().first();
    var name = $(id).next();
    var activityId = $(name).next();
    var activityType = $(activityId).next();
    var startTime = $(activityType).next();
    var endTime = $(startTime).next();
    var isEnabled = $(endTime).next();
    $("#activityBaseInfoForm #activityId").attr("disabled", "disabled");
    $("#activityBaseInfoForm #typeSelect").attr("disabled", "disabled");
    $("#activityBaseInfoForm #typeSelect").attr("style", "background-color:#CCC")
    $("#saveActivityBtn").html("更新");
    $("#saveActivityBtn").attr("onclick", "updateActivity()")


    $("#activityBaseInfoForm #pkid").val(id.html());
    $("#activityBaseInfoForm #activityName").val(name.html());
    $("#activityBaseInfoForm #activityId").val(activityId.attr("data-activityId"));
    $("#activityBaseInfoForm #startTime").val(startTime.html());
    $("#activityBaseInfoForm #endTime").val(endTime.html());
    if (activityType.html() == "地区") {
        $("#activityBaseInfoForm #typeSelect").val("Region");
    }
    else {
        $("#activityBaseInfoForm #typeSelect").val("Vehicle");
    }

    if ($(isEnabled).html().substr(0, 2) == "启用") {
        $("#btnEnable").trigger("click");
    }
    else {
        $("#btnDisEnable").trigger("click");
    }

    GetActivityUrlConfig(activityId.attr("data-activityId"));

}

//打开新建活动对话框
function createActivity() {
    $("#urlTableBody").html("");
    $("#activityBaseInfoForm #activityName").val("");
    $("#activityBaseInfoForm #startTime").val("");
    $("#activityBaseInfoForm #endTime").val("");
    $("#activityBaseInfoForm #typeSelect").val("");
    $("#activityBaseInfoForm #activityId").removeAttr("disabled");
    $("#activityBaseInfoForm #typeSelect").removeAttr("disabled");
    $("#activityBaseInfoForm #typeSelect").removeAttr("style");
    $("#CreateActivityDialog").dialog({ title: '新建活动', width: 1000, modal: true, height: "auto" });
    $("#saveActivityBtn").html("保存");
    $("#saveActivityBtn").attr("onclick", "saveActivity()");
}

//指定为默认页面
function setDefaultPage(btn) {
    //如果复选框被选中，检查该活动有没有默认页面，如果有则报错
    if ($(btn).prop("checked") == true) {
        var activityId = $("#activityBaseInfoForm #activityId").val();
        $.ajax({
            url: "/CityActivityDisplay/GetIsExistDefaultUrl",
            type: "POST",
            data: { activityId: activityId},
            success: function (result) {
                if (!result.Status)
                    alert(result.Msg);
                else {
                    if (result.Data == true) {
                        $(btn).prop("checked", false);
                        $("#confirmDialog").html("该活动已存在默认页面<br>无法将该页面指定为默认页面");
                        $("#confirmDialog").dialog({
                            title: "提示",
                            resizable: false,
                            height: "auto",
                            width: 300,
                            modal: true,
                            buttons: {
                                "确定": function () {
                                    $(this).dialog("close");
                                },
                                "取消": function () {
                                    $(this).dialog("close");
                                }
                            }
                        })
                    }
                    //将该页面设置为默认页面
                    else {
                        $("#UrlForm [data-type='region']").remove();
                        $("#UrlForm [data-type='vehicle']").remove();
                    }
                }
            }

        })
    }
}

//查看日志
function searchOpLog() {
    var pkid = $("#opLogPkid").val();
    var startTime = $("#opLogStartTime").val();
    var endTime = $("#opLogEndTime").val();
    $.ajax({
        url: "/CityActivityDisplay/GetOpRecords",
        type: "GET",
        data: { pkid: pkid, startTime: startTime, endTime: endTime },
        success: function (result) {
            if (!result.Status)
                alert(result.Msg);
            else {
                var temp = "<tr><td>$opUser$</td><td>$opTime$</td><td>$opContent$</td></tr>";
                var html = "";
                $.each(result.Data, function (index, v) {
                    html += temp.replace("$opUser$", v.Author).replace("$opTime$", v.ChangeDatetime).replace("$opContent$", v.Operation);
                });
                $("#opLogTbody").html(html);
            }
        }
    })
}

//查看操作日志详情
function lookOpLog(btn) {
    //var activityId = $(btn).parent().parent().children().first().next().next().attr("data-activityId");
    var pkid = $(btn).parent().parent().children().first().attr("data-PKID");
    $("#opLogPkid").val(pkid);
    $("#opLogTbody").html("");
    $("#opLogStartTime").val("");
    $("#opLogEndTime").val("");
    $("#opLogDialog").dialog({
        title: "查看操作日志",
        height: "auto",
        width: 1000,
        modal: true,
        buttons: {
            "关闭": function () {
                $(this).dialog("close");
            }
        }
    });
    $.ajax({
        url: "/CityActivityDisplay/GetOpRecords",
        type: "GET",
        data: { pkid: pkid, startTime: "", endTime: "" },
        success: function (result) {
            if (!result.Status)
                alert(result.Msg);
            else {
                var temp = "<tr><td>$opUser$</td><td>$opTime$</td><td>$opContent$</td></tr>";
                var html = "";
                $.each(result.Data, function (index, v) {
                    html += temp.replace("$opUser$", v.Author).replace("$opTime$", v.ChangeDatetime).replace("$opContent$", v.Operation);
                });
                $("#opLogTbody").html(html);
            }
        }
    })
}

//只获取一次所有车型品牌
function GetAllVehicleBrands() {
    $.ajax({
        url: "/CityActivityDisplay/GetAllBrand",
        type: "GET",
        success: function (vehicles) {
            allVehicleBrands = vehicles;
        }
    });
}

//只获取一次全部省份
function GetAllProvince() {
    $.ajax({
        url: "/CarInsurance/GetAllProvince",
        type: "POST",
        success: function (provinces) {
            allProvince = provinces;
        }
    });
}

//根据活动Id获取活动页
function GetActivityUrlConfig(activityId) {
    if (!activityId) {
        return;
    }
    $.ajax({
        url: "/CityActivityDisplay/GetActivityUrlByActivityId",
        type: "GET",
        data: { activityId: activityId },
        success: function (result) {
            if (!result.Status)
                alert(result.Msg);
            else {
                var templatedefault = "<tr><td>默认页</td><td style='text-align:left;'>" +
                    "H5:<p> <a href='$href$' target='_blank'>$href$</a> </p></br> "+
                    "小程序:<p> <a href='$wxhref$' target='_blank'>$wxhref$</a></p>" +
                    "</td><td>H5:<p> $name$ </p></br>小程序:<p> $wxname$</p></td>"
                    + "<td><a style='cursor: pointer' onclick='editDefaultUrl(this)'>编辑</a>&nbsp;<a style='cursor: pointer' onclick='alert(\"默认页不可删除\")'>删除</a></td></tr>";
                var templategeneral = "<tr><td>常规页</td><td style='text-align:left;'>" +
                    "H5:<p> <a href='$href$' target='_blank'>$href$</a> </p></br>" +
                    "小程序:<p> <a href='$wxhref$' target='_blank'>$wxhref$</a></p>"
                    + "</td><td>H5:<p> $name$ </p></br>小程序:<p> $wxname$ </p></td>"
                    + "<td><a style='cursor: pointer' onclick='editUrl(this)'>编辑</a>&nbsp;<a style='cursor: pointer' onclick='delateUrl(this)'>删除</a></td></tr>"
                $("#urlTableBody").html("");
                $.each(result.Data, function (index, url) {
                    if (url.IsDefault == 1)
                        $("#urlTableBody").append(
                            templatedefault
                            .replace("$href$", url.TargetUrl || "" ).replace("$href$", url.TargetUrl || "")
                                .replace("$wxhref$", url.WxappUrl || "").replace("$wxhref$", url.WxappUrl || "")
                            .replace("$name$", url.UrlTitle || "未能匹配到名称")
                            .replace("$wxname$", url.WxappUrlTitle || "未能匹配到名称"));
                    else
                        $("#urlTableBody").append(
                            templategeneral.replace("$href$", url.TargetUrl || "").replace("$href$", url.TargetUrl || "")
                                .replace("$wxhref$", url.WxappUrl || "").replace("$wxhref$", url.WxappUrl || "")
                                .replace("$name$", url.UrlTitle || "未能匹配到名称")
                                .replace("$wxname$", url.WxappUrlTitle || "未能匹配到名称"));
                })
            }
        }
    });
}

//刷新活动缓存
function RefreshCache(activityId) {
    if (!activityId) {
        return;
    }
    $.ajax({
        url: "/CityActivityDisplay/RefreshRegionVehicleIdActivityUrlCache",
        type: "GET",
        data: { activityId: activityId },
        success: function (result) {
            if (!result.Status)
                alert("更新缓存失败:" + result.Msg);
        }
    });
}

