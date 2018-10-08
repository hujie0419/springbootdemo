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
        if (result && result.Data) {
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
    $.get("SelectPeccancyProvinceConfig", { provinceId: provinceId, pageIndex: pageIndex, pageSize: pageSize }, function (data) {
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

function Add() {
    var div = '<div class="dialog"><div class="region"><div><label>输入省份信息:</label><input type="number" name="provinceId" placeholder="省份ID" /><input name="provinceName" placeholder="省份名称" /><input name="provinceSimpleName" placeholder="省份简称" /></div></div></div>';
    $('body').append(div);
    $(".dialog").dialog({
        title: "添加省份",
        modal: true,
        width: 500,
        close: function (event, ui) {
            $(this).remove();
        },
    });
    var $dialog = $(".dialog");
    var button1 = {
        text: "新增",
        click: function () {
            if (confirm("确认添加省份?")) {
                var $dialog = $(".dialog");
                var obj = new Object();
                obj.provinceId = $dialog.find("[name = provinceId]").val();
                obj.provinceName = $dialog.find("[name = provinceName]").val();
                obj.provinceSimpleName = $dialog.find("[name = provinceSimpleName]").val();
                if (!Number(obj.provinceId) > 0) {
                    alert("请输入省份Id");
                    return;
                }
                if (!obj.provinceName) {
                    alert("请填写省份名称");
                    return;
                }
                if (!obj.provinceSimpleName) {
                    alert("请输入省份简称");
                    return;
                }
                if (obj.provinceId > 0) {
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
        $.post("AddPeccancyProvinceConfig", obj, function (result) {
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
    var provinceId = $self.parent().prevAll().eq(2).html();
    var provinceName = $self.parent().prevAll().eq(1).html();
    if (!(provinceId && provinceName)) {
        return;
    }
    $.post("GetCityConfigCountByProvinceId", { provinceId: provinceId }, function (result) {
        if (result.Status) {
            if (confirm(provinceName + "下有" + result.Data + "个城市信息，确认删除？")) {
                $.post("DeletePeccancyProvinceConfig", { provinceId: provinceId }, function (result) {
                    if (result.Status) {
                        $(".dialog").dialog("close");
                        alert("删除成功\n若数据未更新,为同步延迟,请刷新页面或重新查询。");
                        setTimeout(function () { Search(currentPage) }, 2000);
                    } else {
                        var msg = result.Msg || "";
                        alert("删除失败！" + msg);
                    }
                });
            }
        }
        else {
            alert("获取" + provinceName + "下的城市信息失败，请重试");
        }
    });
}

function Edit(self) {
    var div = '<div class="dialog"><div class="region"><div><label>更新省份信息:</label><input readonly="readonly" class="disable" type="number" name="provinceId" placeholder="省份ID" /><input name="provinceName" placeholder="省份名称" /><input name="provinceSimpleName" placeholder="省份简称" /></div></div></div>';
    $('body').append(div);
    var $dialog = $(".dialog");
    $dialog.dialog({
        title: "更新省份",
        modal: true,
        width: 500,
        close: function (event, ui) {
            $(this).remove();
        },
    });
    var $self = $(self);
    var provinceId = $self.parent().prevAll().eq(2).html();
    var provinceName = $self.parent().prevAll().eq(1).html();
    var provinceSimpleName = $self.parent().prevAll().eq(0).html();
    if (!(provinceId && provinceName && provinceSimpleName)) {
        return;
    }
    $dialog.find("[name = provinceId]").val(provinceId);
    $dialog.find("[name = provinceName]").val(provinceName);
    $dialog.find("[name = provinceSimpleName]").val(provinceSimpleName);
    var button1 = {
        text: "更新",
        click: function () {
            if (confirm("确认更新省份信息?")) {
                var $dialog = $(".dialog");
                var obj = new Object();
                obj.provinceId = $dialog.find("[name = provinceId]").val();
                obj.provinceName = $dialog.find("[name = provinceName]").val();
                obj.provinceSimpleName = $dialog.find("[name = provinceSimpleName]").val();
                if (!obj.provinceName) {
                    alert("请填写省份名称");
                    return;
                }
                if (!obj.provinceSimpleName) {
                    alert("请输入省份简称");
                    return;
                }
                if (obj.provinceId > 0) {
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
        $.post("UpdatePeccancyQueryProvinceConfig", obj, function (result) {
            if (result.Status) {
                $(".dialog").dialog("close");
                alert("更新成功!\n若数据未更新,为同步延迟,请刷新页面或重新查询。");
                setTimeout(function () { Search(currentPage)}, 2000);
            }
            else {
                var msg = result.Msg || "";
                alert("更新失败!" + msg);
            }
        });
    }
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
    var identityID = $(self).parent().prevAll().eq(3).html() || "";//省份Id
    $.post("SelectPeccancyConfigOprLog", { logType: 'PeccancyProvinceConfig', identityID: identityID }, function (result) {
        console.log(result);
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
        console.log(item);
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