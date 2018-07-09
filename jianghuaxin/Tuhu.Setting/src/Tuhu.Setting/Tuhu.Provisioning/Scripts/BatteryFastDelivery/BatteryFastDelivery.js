var pageSize = 20;
var staticProducts = new Array();
var operate = '<a href="javascript:void(0)" style="width:60px"  onclick="Edit(this)">编辑</a><a href="javascript:void(0)" style="width:60px;margin-left:5px" onclick="Delete(this)">删除</a><input type="checkbox" onclick=changeEnabled(this) {{checked}} />是否启用';
$(function () {
    $("#dialog-addoredit").dialog({
        modal: true,
        width: 600,
        autoOpen: false
    });
    $("#dialog-products").dialog({
        title: "选择适配产品",
        modal: true,
        width: 900,
        height: 600,
        autoOpen: false
    });
    $('.date-picker').timepicker({
        hourGrid: 4,
        minuteGrid: 10
    });
    GetAllProvince();
    GetAllBatteryServiceType();
});


function GetAllBatteryServiceType() {
    $.get("GetAllBatteryServiceType", function (result) {
        $("#serviceTypePid").empty();
        $("#serviceTypePidDialog").empty();
        var html = "";
        if (result.Status && result.Data.length > 0) {
            for (var i = 0; i < result.Data.length; i++) {
                html += '<option value="' +
                    result.Data[i].PID +
                    '">' +
                    result.Data[i].DisplayName +
                    '</option>';
            }
            $("#serviceTypePid").append(html);
            $("#serviceTypePidDialog").append(html);
            Search(1);
        }
        else {
            html = '<option value="">-无极速达服务-</option>';
            $("#serviceTypePid").append(html);
            $("#serviceTypePidDialog").append(html);
        }
    });
}

function GetAllProvince() {
    $.get("GetAllProvince", function (result) {
        $("#province").empty();
        if (result.Status && result.Data.length > 0) {
            var html = '<option value=-1>-请选择省份-</option>';
            for (var i = 0; i < result.Data.length; i++) {
                html += '<option value="' +
                    result.Data[i].RegionId +
                    '">' +
                    result.Data[i].RegionName +
                    '</option>';
            }
            $("#province").append(html);
        }
        else {
            var html = '<option value=-1>-无省份数据-</option>';
            $("#province").append(html);
        }
    });
}
//获取省份后二级联动获取市
function CityList(obj) {
    $.get("GetRegionByRegionId", { regionId: obj.value }, function (result) {
        $("#city").empty();
        $("#district").empty();
        $(district).append('<option value="' + (-1) + '">' + "选择区" + '</option>');
        $("#city").append('<option value="' + (-1) + '">' + "选择市" + '</option>');
        if (!(result && result.Data && result.Data.ChildRegions)) {
            return null;
        }
        for (var i = 0; i < result.Data.ChildRegions.length; i++) {
            if (isExistOption('city', result.Data.ChildRegions[i].CityId) === false) {
                $("#city").append('<option value="' +
                    result.Data.ChildRegions[i].CityId +
                    '">' +
                    result.Data.ChildRegions[i].CityName +
                    '</option>');
            }
        }
    });
}
//获取省市后三级联动获取区
function DistrictList(obj) {
    $.get("GetRegionByRegionId", { "regionId": obj.value }, function (result) {
        $("#district").empty();
        $("#district").append('<option value="' + (-1) + '">' + "选择区" + '</option>');
        if (!(result && result.Data && result.Data.ChildRegions)) {
            return null;
        }
        for (var i = 0; i < result.Data.ChildRegions.length; i++) {
            if (isExistOption('district', result.Data.ChildRegions[i].DistrictId) === false) {
                $("#district").append('<option value="' +
                    result.Data.ChildRegions[i].DistrictId +
                    '">' +
                    result.Data.ChildRegions[i].DistrictName +
                    '</option>');
            }
        }
    });
}

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

function Search(pageIndex) {
    var serviceTypePid = $("#serviceTypePid").val();
    if (!serviceTypePid) {
        alert("请选择蓄电池极速达服务类目");
        return;
    }
    $("#loadingData").show(100);
    $("#noData").hide(1);
    $.post("SelectBatteryFastDelivery", { serviceTypePid: serviceTypePid, pageIndex: pageIndex, pageSize: pageSize }, function (data) {
        if (data && data.Status) {
            currentPage = pageIndex;
            BindData(data);
        }
    });
}

function BindData(data) {
    $("#showArea>tbody").empty();
    if (data.Data && data.Data.length > 0) {
        $(data.Data).each(function (index) {
            var html = $("#Template").html();
            html = html
                .replace("$number$", ((index + 1) + (currentPage - 1) * pageSize) + '<span style="display:none">' + data.Data[index].PKID + '</span>')
                .replace("$province$", data.Data[index].ProvinceName)
                .replace("$city$", data.Data[index].CityName)
                .replace("$district$", '<label>' + data.Data[index].DistrictName + '</label><span style="display:none">' + data.Data[index].RegionId + '</span>')
                .replace("$serviceTime$", '<label>' + DatePrase(data.Data[index].StartTime) + "</label> - <span>" + DatePrase(data.Data[index].EndTime) + "</span>")
                .replace("$products$", ProductPrase(data.Data[index].Products))
                .replace("$remark$", data.Data[index].Remark || "")
                .replace("$operate$", operate.replace("{{checked}}", data.Data[index].IsEnabled ? 'checked="checked"' : ''));
            $("#showArea").append(html);
        });
        $("#loadingData").hide(1);
        $("#showArea").show();
    }
    else if (currentPage > 1) {
        Search(currentPage - 1);
    }
    else {
        $("#loadingData").hide(1);
        $("#noData").show(1);
    }
    kkpager.generPageHtml({
        pno: currentPage,
        mode: 'click',
        total: data.TotalPage,
        totalRecords: data.TotalCount,
        click: function (n) {
            Search(n);
            this.selectPage(n);
        },
        getHref: function (n) {
            return 'javascript:void(0);';
        }
    }, true);
}

function Add() {
    var $dialog = $("#dialog-addoredit");
    $dialog.dialog("option", "title", "添加配置");
    staticProducts = new Array();
    var fastDeliveryId = -1;
    $("#serviceTypePidDialog").val($("#serviceTypePid").val());
    $("#serviceTypePidDialog")[0].disabled = false; $("#serviceTypePidDialog").removeClass("disable");
    $("#province").val(-1);
    $("#province")[0].disabled = false; $("#province").removeClass("disable");
    $("#city").empty().append('<option value="' + (-1) + '">' + "-请选择城市-" + '</option>');
    $("#city")[0].disabled = false; $("#city").removeClass("disable");
    $("#district").empty().append('<option value="' + (-1) + '">' + "-请选择地区-" + '</option>');
    $("#district")[0].disabled = false; $("#district").removeClass("disable");
    $("#remark").val("");
    InitProductList(-1);
    var button1 = {
        text: "新增",
        click: function () {
            if (confirm("确认添加极速达服务信息?")) {
                var obj = new Object();
                obj.serviceTypePid = $("#serviceTypePidDialog").val();
                obj.regionId = $("#district").val();
                obj.startTime = $("#startTime").val();
                obj.endTime = $("#endTime").val();
                obj.remark = $("#remark").val();
                if (!obj.serviceTypePid) {
                    alert("请选择蓄电池极速达服务类目");
                    return;
                }
                if (obj.regionId < 1) {
                    alert("请选择地区");
                    return;
                }
                if (!(obj.endTime && obj.startTime)) {
                    alert("请选择服务时间");
                    return;
                }
                if ("1900-01-01 " + obj.endTime < "1900-01-01 " + obj.startTime) {
                    alert("服务时间开始时间应小于等于结束时间");
                    return;
                }
                if (staticProducts.length < 1) {
                    if (!confirm("选择了0个适配产品，是否确认添加？")) {
                        return;
                    }
                }
                add(obj);
            }
            else {
                return;
            }
        }
    };
    var button2 = {
        text: "取消",
        click: function () {
            $dialog.dialog("close");
        }
    };
    $dialog.dialog("option", "buttons", [button1, button2]);
    $dialog.dialog("open");


    function add(obj) {
        $.post("AddBatteryFastDelivery", { model: obj, productModels: staticProducts }, function (result) {
            if (result.Status) {
                $("#dialog-addoredit").dialog("close");
                if ($("#serviceTypePid").val) {
                    alert(result.Msg || "未知错误,请刷新页面并重试");
                    Search(1);
                }
            }
            else {
                alert(result.Msg || "未知错误,请刷新页面并重试");
            }
        });
    }
}

function Delete(self) {
    var $self = $(self);
    var fastDeliveryId = $self.parent().prevAll().eq(6).find("span").html();
    var regionId = $self.parent().prevAll().eq(3).find("span").html();
    if (fastDeliveryId < 1 || regionId < 1) {
        return;
    }
    if (confirm("确认删除极速达服务信息吗？")) {
        $.post("DeleteBatteryFastDelivery", { fastDeliveryId: fastDeliveryId, regionId: regionId }, function (result) {
            if (result.Status) {
                alert(result.Msg || "未知错误,请刷新页面并重试");
                Search(1);
            }
            else {
                alert(result.Msg || "未知错误,请刷新页面并重试");
            }
        });
    };
}

function Edit(self) {
    var $self = $(self);
    var $dialog = $("#dialog-addoredit");
    $dialog.dialog("option", "title", "编辑配置");
    staticProducts = new Array();
    var fastDeliveryId = $self.parent().prevAll().eq(6).find("span").html();
    var provinceName = $self.parent().prevAll().eq(5).html();
    var cityName = $self.parent().prevAll().eq(4).html();
    var districtName = $self.parent().prevAll().eq(3).find("label").html();
    var regionId = $self.parent().prevAll().eq(3).find("span").html();
    $("#serviceTypePidDialog").val($("#serviceTypePid").val());
    $("#serviceTypePidDialog")[0].disabled = true; $("#serviceTypePidDialog").addClass("disable");
    $("#province").find("option:contains('" + provinceName + "')").attr('selected', true);
    $("#province")[0].disabled = true; $("#province").addClass("disable");
    $("#city").empty().append("<option value='" + cityName + "'>" + cityName + "</option>").val(cityName);
    $("#city")[0].disabled = true; $("#city").addClass("disable");
    $("#district").empty().append("<option value='" + regionId + "'>" + districtName + "</option>").val(districtName);
    $("#district")[0].disabled = true; $("#district").addClass("disable");
    $("#startTime").val($self.parent().prevAll().eq(2).find("label").html());
    $("#endTime").val($self.parent().prevAll().eq(2).find("span").html());
    $("#remark").val($self.parent().prevAll().eq(0).html());
    InitProductList(fastDeliveryId);
    var button1 = {
        text: "更新",
        click: function () {
            if (confirm("确认更新极速达服务配置信息?")) {
                var obj = new Object();
                obj.ServiceTypePid = $("#serviceTypePidDialog").val();
                obj.RegionId = $("#district").val();
                obj.StartTime = $("#startTime").val();
                obj.EndTime = $("#endTime").val();
                obj.Remark = $("#remark").val();
                obj.PKID = fastDeliveryId;
                if (!(obj.EndTime && obj.StartTime)) {
                    alert("请选择服务时间");
                    return;
                }
                if ("1900-01-01 " + obj.EndTime < "1900-01-01 " + obj.StartTime) {
                    alert("服务时间开始时间应小于等于结束时间");
                    return;
                }
                if (staticProducts.length < 1) {
                    if (!confirm("选择了0个适配产品，是否确认更新？")) {
                        return;
                    }
                }
                if (obj.PKID > 0 && obj.RegionId > 0 && obj.ServiceTypePid) {
                    edit(obj);
                }
            }
            else {
                return;
            }
        }
    };
    var button2 = {
        text: "取消",
        click: function () {
            $dialog.dialog("close");
        }
    };
    $dialog.dialog("option", "buttons", [button1, button2]);
    $dialog.dialog("open");

    function edit(obj) {
        $.post("UpdateBatteryFastDelivery", { model: obj, productModels: staticProducts }, function (result) {
            if (result.Status) {
                alert(result.Msg || "未知错误,请刷新页面并重试");
                $(".dialog").dialog("close");
                Search(1);
            }
            else {
                alert(result.Msg || "未知错误,请刷新页面并重试");
            }
        });
    }
}

function changeEnabled(_this) {
    var $this = $(_this);
    var checked = $this.prop('checked');
    var message = checked ? '确认启用极速达服务信息?' : '确认禁用极速达服务信息?';
    if (confirm(message)) {
        var pkid = $this.parent().prevAll().eq(6).find("span").html();
        $.post("ChangeBatteryFastDeliveryStatus", { pkid: pkid, isEnabled: checked }, function (result) {
            if (result.status) {
                alert("操作成功！")
            }
            else {
                alert("操作失败!" + (result.msg || ""));
                $this.prop('checked', !checked);
            }
        });
    } else {
        $this.prop('checked', !checked);
    }
}


function DatePrase(longDate) {
    if (longDate != null) {
        return eval('new ' + eval(longDate).source).Format('hh:mm');
    } else {
        return "";
    }
}

function ProductPrase(products) {
    var productStr = "";
    if (products) {
        for (var key in products) {
            productStr += "<div style='margin-top:10px'>" + key + ": ";
            var battery = new Array();
            if (products.hasOwnProperty(key)) {
                var batteries = products[key];
                $.each(batteries, function (index, value) {
                    /*跳转到官网蓄电池详情页 PID转换*/
                    var productUrl = GetBatteryUrl(value);
                    if (productUrl) { battery.push(productUrl); }
                });
            }
            productStr += battery.join(",&nbsp") + '</div>';
        }
        productStr += '<a herf="javascript:void(0);" onclick="ShowBatteryList(this)" style="cursor:pointer;float:right;margin-bottom:10px">查看全部适配产品</a>';
    }
    return productStr;
}

function InitProductList(fastDeliveryId) {
    var div = '<table><tr style="white-space: nowrap";><th><input type="checkbox" id="checkAllBattery" onchange=CheckAllBattery(this) />全选</th><th>品牌</th><th>PID</th><th>名称</th></tr>';
    $.post("GetAllBatteryForView", { fastDeliveryId: fastDeliveryId }, function (result) {
        if (result.Status && result.Data.length > 0) {
            $.each(result.Data, function (index, value) {
                div += '<tr><td><input type="checkbox" name="product"' + (value.IsChecked ? "checked" : "") + '></td><td>' + value.Brand + '</td><td>' + value.PID + '</td><td>' + value.DisplayName + '<span style="display:none">' + fastDeliveryId + '</span></td></tr>';
            });
            div += '</table>';
            $("#dialog-products").empty().append(div);
            staticProducts = new Array();
            $("#dialog-products input[name='product']:checked").each(function () {
                var obj = new Object();
                obj.Brand = $(this).parent().nextAll().eq(0).html();
                obj.ProductPid = $(this).parent().nextAll().eq(1).html();
                obj.FastDeliveryId = $(this).parent().nextAll().eq(2).find('span').html();
                if (obj.ProductPid && obj.Brand) {
                    staticProducts.push(obj);
                }
            });
            $("#selectedProducts").empty().append("(已选" + staticProducts.length + "个)");
            $("#selectedProducts").show();
        }
        else {
            div += '<tr><td colspan="8">无适配产品</td></tr></table>';
            $("#dialog-products").empty().append(div);
            $("#selectedProducts").empty();
        }
    });
}

function ChooseProduct() {
    var $dialog = $("#dialog-products");
    var button1 = {
        text: "确定",
        click: function () {
            staticProducts = new Array();
            $("#dialog-products input[name='product']:checked").each(function () {
                var obj = new Object();
                obj.Brand = $(this).parent().nextAll().eq(0).html();
                obj.ProductPid = $(this).parent().nextAll().eq(1).html();
                obj.FastDeliveryId = $(this).parent().nextAll().eq(2).find('span').html();
                staticProducts.push(obj);
            });
            $("#selectedProducts").empty().append("(已选" + staticProducts.length + "个)");
            $("#selectedProducts").show();
            $dialog.dialog("close");
        }
    };
    var button2 = {
        text: "取消",
        click: function () {
            $dialog.dialog("close");
        }
    };
    $dialog.dialog("option", "buttons", [button1, button2]);
    $dialog.dialog("open");
}

function CheckAllBattery(self) {
    var $self = $(self);
    if ($self.is(":checked")) {
        $self.parent().parent().parent().find("input[type='checkbox']").prop("checked", true);//全选
    } else {
        $self.parent().parent().parent().find("input[type='checkbox']").prop("checked", false);//反选
    }
}
// 查看全部适配产品
function ShowBatteryList(self) {
    var $self = $(self);
    var fastDeliveryId = $self.parent().prevAll().eq(4).find("span").html();
    if (fastDeliveryId > 0) {
        $.post("GetBatteryFastDeliveryProductsByFastDeliveryId", { fastDeliveryId: fastDeliveryId }, function (result) {
            var div = '<div class="dialog" id="dialog-showAllProducts"><table><tr><th>品牌</th><th>产品PID</th></tr>';
            if (result.Status && result.Data.length > 0) {
                $.each(result.Data, function (index, value) {
                    div += '<tr><td>' + value.Brand + '</td><td>' + GetBatteryUrl(value.ProductPid) + '</td></tr>';
                });
            }
            else {
                div += '<tr><td colspan="8">无适配产品</td></tr>';
            }
            div += '</table></div>';
            $('body').append(div);
            var $dialog = $("#dialog-showAllProducts");
            $dialog.dialog({
                title: "查看适配产品",
                modal: true,
                width: 400,
                height: 500,
                close: function (event, ui) {
                    $(this).remove();
                },
            });
            $dialog.dialog("open");
        });
    }
}

function GetBatteryUrl(productPid) {
    if (!productPid) {
        return;
    }
    var pidArray = productPid.split("|");
    var pidResult = "";
    if (pidArray.length < 2 || !pidArray[1])//不含|或|后内容为空 则去除|
    {
        pidResult = pidArray[0];
    }
    else {
        pidResult = pidArray[0] + "/" + pidArray[1];
    }
    return '<a target="_blank" href="http://item.tuhu.cn/Products/' + pidResult + '.html">' + productPid + '</a>';
}

