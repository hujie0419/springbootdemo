var currentPage = 1;
var pageSize = 20;
var operate = '<button class="btn btn-basic" style="width:60px"  onclick="Edit(this)">编辑</button><button class="btn btn-danger" style="width:60px;margin-left:5px"  onclick="Delete(this)">删除</button>';
var operateHistory = '<button class="btn btn-basic" style="width:60px" onclick="WatchLog(this)">查看</button>';
$(document).ready(function () {
    GetAllProvince();
    Search(1);
});

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

function Search(pageIndex) {
    $("#loadingData").show(100);
    $("#noData").hide(1);
    currentPage = pageIndex;
    var provinceId = $("#province").val();
    var cityId = $("#city").val();
    $.get("SelectPeccancyCityConfig", { provinceId: provinceId, cityId: cityId, pageIndex: pageIndex, pageSize: pageSize }, function (data) {
        if (data && data.Status) {
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
                .replace("$number$", ((index + 1) + (currentPage - 1) * pageSize))
                .replace("$provinceId$", data.Data[index].ProvinceId)
                .replace("$provinceName$", data.Data[index].ProvinceName)
                .replace("$provinceSimpleName", data.Data[index].ProvinceSimpleName)
                .replace("$cityId$", data.Data[index].CityId)
                .replace("$cityCode$", data.Data[index].CityCode)
                .replace("$cityName$", data.Data[index].CityName)
                .replace("$needEngine$", (data.Data[index].NeedEngine ? "是" : "否") + '<span style="display:none">' + (data.Data[index].NeedEngine || false) + '</span>')
                .replace("$needFrame$", (data.Data[index].NeedFrame ? "是" : "否") + '<span style="display:none">' + (data.Data[index].NeedFrame || false) + '</span>')
                .replace("$engineLen$", data.Data[index].EngineLen)
                .replace("$frameLen$", data.Data[index].FrameLen)
                .replace("$isEnabled$", (data.Data[index].IsEnabled ? "是" : "否") + '<span style="display:none">' + (data.Data[index].IsEnabled || false) + '</span>')
                .replace("$operate$", operate)
                .replace("$history$", operateHistory);
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

function ProvinceChange(provinceId) {
    if (provinceId < 0) {
        $("#city").empty();
        var html = '<option value=-1>-请选择城市-</option>';
        $("#city").append(html);
        Search(1);
        return;
    }
    $.get("GetPeccancyCitiesByProvinceId", { provinceId: provinceId }, function (result) {
        $("#city").empty();
        if (result.Status && result.Data.length > 0) {
            var html = '<option value=-1>-请选择城市-</option>';
            for (var i = 0; i < result.Data.length; i++) {
                html += '<option value="' +
                    result.Data[i].RegionId +
                    '">' +
                    result.Data[i].RegionName +
                    '</option>';
            }
            $("#city").append(html);
        }
        else {
            var html = '<option value=-1>-无城市数据-</option>';
            $("#city").append(html);
        }
    });
};

function Add() {
    var div = '<div class="dialog"><table><tr><th>输入省份信息:</th><td><input onchange=provinceInfo(this.value) type="number" name="provinceId" placeholder="省份ID" />'
        + '<label name="provinceInfo" style="display:none"></label></td></tr>'
        + '<tr><th>输入城市信息:</th><td><input type="number" name="cityId" placeholder="城市ID" /><input type="number" name="cityCode" placeholder="城市代码" /><input name="cityName" placeholder="城市名称" /></td></tr>'
        + '<tr><th>发动机号是否必须:</th><td><label><input type="radio" name="needEngine" value=true />是<input type="number" name="engineLen" placeholder="发动机长度" /></label><label><input type="radio" name="needEngine" value=false />否</label></td></tr>'
        + '<tr><th>车架号是否必须:</th><td><label><input type="radio" name="needFrame" value=true />是<input type="number" name="frameLen" placeholder="车架号长度" /></label><label><input type="radio" name="needFrame" value=false />否</label></td></tr>'
        + '<tr><th>是否启用: </th><td><label><input type="radio" name="isEnabled" value=true />启用</label><label><input type="radio" name="isEnabled" value=false />禁用</label></td></tr></table></div>';
    $('body').append(div);
    $(".dialog").dialog({
        title: "添加城市",
        modal: true,
        width: 800,
        close: function (event, ui) {
            $(this).remove();
        },
    });
    var $dialog = $(".dialog");
    var button1 = {
        text: "新增",
        click: function () {
            if (confirm("确认添加城市信息?")) {
                var $dialog = $(".dialog");
                var obj = new Object();
                obj.provinceId = $dialog.find("[name = provinceId]").val();
                obj.cityId = $dialog.find("[name = cityId]").val();
                obj.cityCode = $dialog.find("[name = cityCode]").val();
                obj.cityName = $dialog.find("[name = cityName]").val();
                obj.needEngine = $dialog.find('[name="needEngine"]:checked').val() || false;
                obj.engineLen = $dialog.find("[name = engineLen]").val();
                obj.needFrame = $dialog.find('[name="needFrame"]:checked').val() || false;
                obj.frameLen = $dialog.find("[name = frameLen]").val();
                obj.IsEnabled = $dialog.find("[name = isEnabled]:checked").val() || false;
                if (!(Number(obj.cityId) > 0)) {
                    alert("请输入城市Id");
                    return;
                }
                if (!(Number(obj.cityCode) > 0)) {
                    alert("请输入城市代码");
                    return;
                }
                if (!obj.cityName) {
                    alert("请输入城市名称");
                    return;
                }
                if (obj.needEngine == "true" && obj.engineLen < 1) {
                    alert("请输入发动机号长度");
                    return;
                }
                if (obj.needFrame == "true" && obj.frameLen < 1) {
                    alert("请输入车架号长度");
                    return;
                }
                if (obj.provinceId > 0 && obj.cityId && obj.cityCode && obj.cityName) {
                    add(obj);
                }
                else {
                    return;
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


    function add(obj) {
        $.post("AddPeccancyQueryCityConfig", obj, function (result) {
            if (result.Status) {
                $(".dialog").dialog("close");
                alert("添加成功!\n若数据未更新,为同步延迟,请刷新页面或重新查询。");
                setTimeout(function () { Search(1) }, 2000);
            }
            else {
                var msg = result.Msg || "";
                alert("添加失败!" + msg);
            }
        });
    }
}

function Delete(self) {
    var $self = $(self);
    var cityId = $self.parent().prevAll().eq(7).html();
    var cityName = $self.parent().prevAll().eq(5).html();
    if (!(cityId && cityName)) {
        return;
    }

    if (confirm("确认删除" + cityName + "的城市信息吗？")) {
        $.post("DeletePeccancyQueryCityConfig", { cityId: cityId }, function (result) {
            if (result.Status) {
                $(".dialog").dialog("close");
                alert("删除成功!\n若数据未更新,为同步延迟,请刷新页面或重新查询。");
                setTimeout(function () { Search(currentPage) }, 2000);
            } else {
                var msg = result.Msg || "";
                alert("删除失败！" + msg);
            }
        });
    };
}

function Edit(self) {
    var div = '<div class="dialog"><table><tr><th>输入省份信息:</th><td><input class="disable" readonly="readonly" onchange=provinceInfo(this.value) type="number" style="margin-left:20px;" name="provinceId" placeholder="省份ID" />'
        + '<label name="provinceInfo" style="display:none"></label></td></tr>'
        + '<tr><th>输入城市信息:</th><td><input class="disable" type="number" readonly="readonly" name="cityId" placeholder="城市ID" /><input type="number" name="cityCode" placeholder="城市代码" /><input name="cityName" placeholder="城市名称" /></td></tr>'
        + '<tr><th>发动机号是否必须:</th><td><label><input type="radio" name="needEngine" value=true />是<input type="number" name="engineLen" placeholder="发动机长度" /></label><label><input type="radio" name="needEngine" value=false />否</label></td></tr>'
        + '<tr><th>车架号是否必须:</th><td><label><input type="radio" name="needFrame" value=true />是<input type="number" name="frameLen" placeholder="车架号长度" /></label><label><input type="radio" name="needFrame" value=false />否</label></td></tr>'
        + '<tr><th>是否启用: </th><td><label><input type="radio" name="isEnabled" value=true />启用</label><label><input type="radio" name="isEnabled" value=false />禁用</label></td></tr></table></div>';
    $('body').append(div);
    var $dialog = $(".dialog");
    $dialog.dialog({
        title: "更新城市",
        modal: true,
        width: 800,
        close: function (event, ui) {
            $(this).remove();
        },
    });
    var $self = $(self);
    var provinceId = $self.parent().prevAll().eq(10).html();
    var cityId = $self.parent().prevAll().eq(7).html();
    var cityCode = $self.parent().prevAll().eq(6).html();
    var cityName = $self.parent().prevAll().eq(5).html();
    var needEngine = $self.parent().prevAll().eq(4).find("span").html() || false;
    var needFrame = $self.parent().prevAll().eq(3).find("span").html() || false;
    var isEnabled = $self.parent().prevAll().eq(0).find("span").html() || false;
    var engineLen = $self.parent().prevAll().eq(2).html();
    var frameLen = $self.parent().prevAll().eq(1).html();
    $dialog.find("[name = provinceId]").val(provinceId);
    $dialog.find("[name = cityId]").val(cityId);
    $dialog.find("[name = cityCode]").val(cityCode);
    $dialog.find("[name = cityName]").val(cityName);
    $dialog.find("[name = engineLen]").val(engineLen);
    $dialog.find("[name = frameLen]").val(frameLen);
    $dialog.find('[name= needEngine][value=' + needEngine + ']').attr('checked', 'true');
    $dialog.find('[name= needFrame][value=' + needFrame + ']').attr('checked', 'true');
    $dialog.find('[name= isEnabled][value=' + isEnabled + ']').attr('checked', 'true');
    var button1 = {
        text: "更新",
        click: function () {
            if (confirm("确认更新城市信息?")) {
                var $dialog = $(".dialog");
                var obj = new Object();
                obj.provinceId = $dialog.find("[name = provinceId]").val();
                obj.cityId = $dialog.find("[name = cityId]").val();
                obj.cityCode = $dialog.find("[name = cityCode]").val();
                obj.cityName = $dialog.find("[name = cityName]").val();
                obj.needEngine = $dialog.find('[name="needEngine"]:checked').val() || false;
                obj.engineLen = $dialog.find("[name = engineLen]").val();
                obj.needFrame = $dialog.find('[name="needFrame"]:checked').val() || false;
                obj.frameLen = $dialog.find("[name = frameLen]").val();
                obj.IsEnabled = $dialog.find("[name = isEnabled]:checked").val() || false;
                if (!(Number(obj.cityCode) > 0)) {
                    alert("请输入城市代码");
                    return;
                }
                if (!obj.cityName) {
                    alert("请输入城市名称");
                    return;
                }
                if (obj.needEngine == "true" && obj.engineLen < 1) {
                    alert("请输入发动机号长度");
                    return;
                }
                if (obj.needFrame == "true" && obj.frameLen < 1) {
                    alert("请输入车架号长度");
                    return;
                }
                if (obj.provinceId > 0 && obj.cityId > 0 && obj.cityCode > 0 && obj.cityName) {
                    edit(obj);
                }
                else {
                    return;
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
        $.post("UpdatePeccancyQueryCityConfig", obj, function (result) {
            if (result.Status) {
                $(".dialog").dialog("close");
                alert("更新成功!\n若数据未更新,为同步延迟,请刷新页面或重新查询。");
                setTimeout(function () { Search(currentPage) }, 2000);
            }
            else {
                var msg = result.Msg || "";
                alert("更新失败!" + msg);
            }
        });
    }
}

function provinceInfo(provinceId) {
    if (provinceId < 1) {
        return;
    }
    $.post("GetPeccancyProvinceConfigByProvinceId", { provinceId: provinceId }, function (result) {
        var $provinceInfo = $(".dialog").find("[name = provinceInfo]");
        if (result.Status && result.Data) {
            $provinceInfo.empty();
            $provinceInfo.append("<span>" + result.Data.ProvinceName + "</span><span>" + result.Data.ProvinceSimpleName + "</span>");
            $provinceInfo.show();
        }
        else {
            $provinceInfo.hide();
        }
    });
}

function RefreshCache() {
    if (confirm("确认刷新缓存?")) {
        $.post("CleanPeccancyCitysCache", function (result) {
            if (result.Status) {
                alert("清除缓存成功");
            }
            else {
                alert("清除缓存失败!" + result.Msg);
            }
        });
    }
}

//查看操作记录
function WatchLog(self) {
    var identityID = $(self).parent().prevAll().eq(8).html() || "";//省份Id
    $.post("SelectPeccancyConfigOprLog", { logType: 'PeccancyCityConfig', identityID: identityID }, function (result) {
        if (result.Status) {
            ViewLog(result.Data);
        }
    });
}

function ViewLog(items) {
    var tbody = "";
    $.each(items, function (index) {
        tbody += "<tr><td>" + items[index].Remarks + "<span style='display:none'>" + items[index].PKID + "</span></td><td>"
            + items[index].Operator + "</td> <td>" + DatePrase(items[index].CreateDateTime)
            + "</td> <td><button onclick='ViewLogDetail(this)'>详情</button></td ></tr > ";
    });
    var div = "<div class='dialog-showLog'><div><table class='tableContainer'><thead><tr>" +
        "<th>操作</th><th>操作人</th><th>时间</th><th>查看前后数据</th></tr></thead><tbody></tbody></table></div></div>";
    $('body').append(div);
    $('.dialog-showLog table>tbody').append(tbody);
    $(".dialog-showLog").dialog({
        title: "日志详情",
        modal: true,
        width: 900,
        height: 600,
        close: function (event, ui) {
            $(this).remove();
        },
    });
}

//查看操作记录详情
function ViewLogDetail(self) {
    var pkid = $(self).parent().prevAll().eq(2).find('span').html();
    $.post("GetPeccancyConfigOprLog", { pkid: pkid }, function (item) {
        if (item.Status) {
            var CreateDateTime = DatePrase(item.Data.CreateDateTime);
            var operateUser = item.Data.Operator;
            var remarks = item.Data.Remarks;
            var oldval = item.Data.OldValue;
            var newval = item.Data.NewValue;
            var oldObj = /^\s*$/.test(oldval) ? null : JSON.parse(oldval);
            var newObj = /^\s*$/.test(newval) ? null : JSON.parse(newval);
            var obj = oldObj || newObj;
            var keys = [];
            for (var property in obj) {
                if (property != "PriorityList") {
                    if (obj.hasOwnProperty(property)) {
                        keys[keys.length] = property;
                    }
                }

            }
            var tbody = keys.map(function (x) {
                var before = '', after = '';
                if (oldObj != null && newObj != null && oldObj[x] != newObj[x]) {
                    before = "<td style='color:red;'>" + oldObj[x] + "</td>";
                    after = "<td style='color:green;'>" + newObj[x] + "</td>";
                } else if (oldObj != null && newObj != null) {
                    before = "<td>" + oldObj[x] + "</td>";
                    after = "<td>" + newObj[x] + "</td>";
                } else if (oldObj == null && newObj != null) {
                    before = "<td></td>";
                    after = "<td style='color:green;'>" + newObj[x] + "</td>";
                }
                else if (oldObj != null && newObj == null) {
                    before = "<td style='color:red;'>" + oldObj[x] + "</td>";
                    after = "<td></td>";
                }
                else {
                    before = "<td></td>";
                    after = "<td></td>";
                }
                var tr = "<tr><td>" + x + "</td>" + before + after + "</tr>";
                return tr;
            }).join('');
            var div = "<div class='dialog-showDetailLog'><div><table class='tableContainer'><thead><tr>" +
                "<th>列名</th><th>操作前值</th><th>操作后值</th></tr></thead><tbody></tbody></table></div></div>";
            $('body').append(div);
            $('.dialog-showDetailLog table>tbody').append(tbody);
            $(".dialog-showDetailLog").dialog({
                title: "日志详情",
                modal: true,
                width: 600,
                close: function (event, ui) {
                    $(this).remove();
                },
            });
        }
    });
}

//时间戳转换
function DatePrase(longDate) {
    if (longDate != null) {
        return eval('new ' + eval(longDate).source).Format('yyyy-MM-dd hh:mm:ss');
    } else {
        return "";
    }
}