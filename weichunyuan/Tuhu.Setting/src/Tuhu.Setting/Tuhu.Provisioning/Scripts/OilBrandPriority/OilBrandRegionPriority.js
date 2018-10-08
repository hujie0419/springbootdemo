var currentPage = 1;
var pageSize = 20;
var operate = '<button class="btn btn-basic" style="width:60px"  onclick="Edit(this)">编辑</button><button class="btn btn-danger" style="width:60px;margin-left:5px"  onclick="Delete(this)">删除</button>';
var operateHistory = '<button class="btn btn-basic" style="width:60px" onclick="WatchLog(this)">查看</button>';
$(document).ready(function () {
    $("#dialog-log").dialog({
        modal: true,
        autoOpen: false,
        width: 900
    });
    $("#dialog-addoredit").dialog({
        modal: true,
        autoOpen: false,
        width: 900
    });
    $("#dialog-confirm").dialog({
        modal: true,
        autoOpen: false,
        width: 500
    });
    GetAllProvince();
    GetAllBrands();
});

function GetAllProvince() {
    $.get("GetAllProvince", function (result) {
        $("#province").empty();
        $("#dialog-addoredit").find("[name=province]").empty();
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
            $("#dialog-addoredit").find("[name=province]").append(html);
        }
        else {
            var html = '<option value=-1>-无省份数据-</option>';
            $("#province").append(html);
            $("#dialog-addoredit").find("[name=province]").append(html);
        }
    });
}

function ProvinceChange(obj) {
    $.get("GetRegionByRegionId", { "regionId": obj.value }, function (result) {
        $("#city").empty();
        if (!(result && result.Data)) {
            $("#city").append('<option value="' + (-1) + '">' + "-请选择城市-" + '</option>');
            return;
        }
        $("#city").append('<option value="' + (-1) + '">' + "-请选择城市-" + '</option>');
        for (var i = 0; i < result.Data.length; i++) {

            $("#city").append('<option value="' +
                result.Data[i].CityId +
                '">' +
                result.Data[i].CityName +
                '</option>');
        }
    });
};

function ProvinceChangeDialog(obj) {
    $.get("GetRegionByRegionId", { "regionId": obj.value }, function (result) {
        var $dialog = $("#dialog-addoredit");
        $city = $dialog.find("[name=city]");
        $city.empty();
        if (!(result && result.Data)) {
            $city.append('<option value="' + (-1) + '">' + "-请选择城市-" + '</option>');
            return;
        }
        $city.append('<option value="' + (-1) + '">' + "-请选择城市-" + '</option>');
        for (var i = 0; i < result.Data.length; i++) {

            $city.append('<option value="' +
                result.Data[i].CityId +
                '">' +
                result.Data[i].CityName +
                '</option>');
        }
    });
};

function GetAllBrands() {
    $.get("GetAllBrands", function (data) {
        if (data.Status) {
            var html = "";
            html = "<option value=''>-选择品牌-</option>" + html;
            $(data.Data).each(function (index) {
                html += "<option value='" + data.Data[index] + "'>" + data.Data[index] + "</option>";
            });
            $("#brand").empty();
            $("#brand").append(html);
            $("#dialog-addoredit").find("[name=brand]").empty();
            $("#dialog-addoredit").find("[name=brand]").append(html);
        } else {
            html = "<option value=''>无数据</option>";
            $("#brand").empty();
            $("#brand").append(html);
            $("#dialog-addoredit").find("[name=brand]").empty();
            $("#dialog-addoredit").find("[name=brand]").append(html);
        }
    });
}

function Search(pageIndex) {
    $("#loadingData").show(100);
    $("#noData").hide(1);
    var provinceId = $("#province").val();
    var regionId = $("#city").val();
    var brand = $("#brand").val();
    $.get("SelectOilBrandRegionPriority", { provinceId: provinceId, regionId: regionId, brand: brand, pageIndex: pageIndex, pageSize: pageSize }, function (data) {
        console.log(data);
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
                .replace("$province$", '<label style="display:none">' + data.Data[index].ProvinceName + '</label><span style="display:none">' + data.Data[index].ProvinceId + '</span>')
                .replace("$city$", '<label style="display:none">' + data.Data[index].CityName + '</label><span style="display:none">' + data.Data[index].RegionId + '</span>')
                .replace("$region$", data.Data[index].ProvinceName + ',' + data.Data[index].CityName)
                .replace("$brand$", '<label>' + data.Data[index].Brand + '</label>')
                .replace("$operate$", operate)
                .replace("$createDateTime$", DatePrase(data.Data[index].CreateDateTime))
                .replace("$lastUpdateDateTime$", DatePrase(data.Data[index].LastUpdateDateTime))
                .replace("$operateHistory$", operateHistory);
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

function DatePrase(longDate) {
    if (longDate != null) {
        return eval('new ' + eval(longDate).source).Format('yyyy-MM-dd hh:mm:ss');
    } else {
        return "";
    }
}

function Add() {
    var $dialog = $("#dialog-addoredit");
    $dialog.find("[name=province]").val(-1);
    $dialog.find("[name=city]").empty();
    $dialog.find("[name=city]").append('<option value="' + (-1) + '">' + "-请选择城市-" + '</option>');
    $dialog.find("[name=city]").val(-1);
    $dialog.find("[name=brand]").val('');
    $dialog.find("[name=province]")[0].disabled = false; $dialog.find("[name=province]").removeClass("disable");
    $dialog.find("[name=city]")[0].disabled = false; $dialog.find("[name=city]").removeClass("disable");
    OpenAddDialog();
    function OpenAddDialog() {
        var $dialog = $("#dialog-addoredit");
        $dialog.dialog("option", "title", "添加排序规则");
        var button1 = {
            text: "保存", click: function () {
                var obj = new Object();
                obj.provinceId = $dialog.find("[name=province]").val();
                obj.provinceName = $dialog.find("[name=province] option:selected").text();
                obj.regionId = $dialog.find("[name=city]").val();
                obj.cityName = $dialog.find("[name=city] option:selected").text();
                obj.brand = $dialog.find("[name=brand]").val();
                if (obj.provinceId < 0 || obj.regionId < 0) {
                    alert("省份或城市不能为空");
                    return;
                }
                if (!obj.brand) {
                    alert("品牌不能为空");
                    return;
                }
                MessageConfirm({
                    title: "确认添加", content: "是否确认添加?", confirm: function () {
                        add(obj);
                    }
                });
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
            $.post("AddOrEditOilBrandRegionPriority", obj, function (result) {
                if (result.Status) {
                    alert("添加成功");
                    $("#dialog-addoredit").dialog("close");
                    CleanCache();
                    Search(currentPage);
                }
                else {
                    var msg = result.Msg || "";
                    alert("添加失败!" + msg);
                }
            });
        }
    }
}

function Edit(self) {
    var $self = $(self), $dialog = $("#dialog-addoredit"), obj = {};
    var PKID = $self.parent().prevAll().eq(4).find('span').html();
    obj.provinceId = $self.parent().prevAll().eq(3).find('span').html();
    obj.cityName = $self.parent().prevAll().eq(2).find('label').html();
    obj.regionId = $self.parent().prevAll().eq(2).find('span').html();
    obj.brand = $self.parent().prevAll().eq(0).find('label').html();
    $dialog.find("[name=brand]").val(obj.brand);
    $dialog.find("[name=province]").val(obj.provinceId);
    $dialog.find("[name=province]")[0].disabled = true; $dialog.find("[name=province]").addClass("disable");
    $dialog.find("[name=city]").empty();
    $dialog.find("[name=city]").append("<option value='" + obj.regionId + "'>" + obj.cityName + "</option>");
    $dialog.find("[name=city]").val(obj.regionId);
    $dialog.find("[name=city]")[0].disabled = true; $dialog.find("[name=city]").addClass("disable");
    OpenEditDialog(PKID);
    function OpenEditDialog(PKID) {
        var $dialog = $("#dialog-addoredit");
        $dialog.dialog("option", "title", "修改机油品牌推荐优先级");
        var button1 = {
            text: "修改", click: function () {
                var obj = new Object();
                obj.PKID = PKID;
                obj.provinceId = $dialog.find("[name=province]").val();
                obj.provinceName = $dialog.find("[name=province] option:selected").text();
                obj.regionId = $dialog.find("[name=city]").val();
                obj.cityName = $dialog.find("[name=city] option:selected").text();
                obj.brand = $dialog.find("[name=brand]").val();
                if (obj.provinceId < 0 || obj.regionId < 0) {
                    alert("城市省份不能为空");
                    return false;
                }
                if (!obj.brand) {
                    alert("品牌不能为空");
                    return;
                }
                MessageConfirm({
                    title: "确认修改", content: "是否确认修改", confirm: function () {
                        edit(obj);
                    }
                })
            }
        };
        var button2 = {
            text: "取消", click: function () {
                $dialog.dialog("close");
            }
        };
        $dialog.dialog("option", "buttons", [button1, button2]);
        $dialog.dialog('open');

        function edit(obj) {
            $.post("AddOrEditOilBrandRegionPriority", obj, function (result) {
                if (result.Status) {
                    alert("修改成功");
                    $dialog.dialog("close");
                    CleanCache();
                    Search(currentPage);
                } else {
                    var msg = result.Msg || "";
                    alert("修改失败!" + msg);
                    $("#dialog-addoredit").dialog("close");
                }
            });
        }
    }


}

function Delete(self) {
    var $self = $(self);
    var cityName = $self.parent().prevAll().eq(2).find('label').html();
    var callback = function () {
        var pkid = $self.parent().prevAll().eq(4).find('span').html();
        $.post("DeleteOilBrandPriorityByPKID", { type: "region", pkid: pkid }, function (result) {
            if (result.Status) {
                alert("删除成功");
                CleanCache();
                Search(currentPage);
            } else {
                var msg = result.Msg || "";
                alert("删除失败！" + msg);
            }
        });
    }
    MessageConfirm({ title: "温馨提示", content: "确认删除" + cityName + "的推荐吗？", confirm: callback });
}


function MessageConfirm(options) {
    var options = $.extend({}, {
        title: "",
        content: "",
        confirm: function () { return; }
    }, options || {});
    var $dialog = $("#dialog-confirm");
    $dialog.dialog("option", "title", options.title);
    $dialog.find(".dialog-content").text(options.content);
    var button1 = {
        text: "确认",
        click: function () {
            $dialog.dialog("close");
            options.confirm();
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

function WatchLog(self) {
    var identityID = $(self).parent().prevAll().eq(5).find('span').html() || "";
    $.post("GetVehicleOprLog", { logType: 'OilBrandRegionPriority', identityID: identityID }, function (result) {
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
            + items[index].OperateUser + "</td> <td>" + DatePrase(items[index].CreateTime)
            + "</td> <td><button onclick='ViewLogDetail(this)'>详情</button></td ></tr > ";
    });
    var div = "<div class='dialog-showLog'><div><table class='tableContainer'><thead><tr>" +
        "<th>操作类型</th><th>操作人</th><th>时间</th><th>查看前后数据</th></tr></thead><tbody></tbody></table></div></div>";
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

function ViewLogDetail(self) {
    var pkid = $(self).parent().prevAll().eq(2).find('span').html();
    $.post("GetBaoYangOprLogByPKID", { pkid: pkid }, function (item) {
        console.log(item);
        if (item.Status) {
            var CreateDateTime = DatePrase(item.Data.CreateDateTime);
            var operateUser = item.Data.OperateUser;
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

function CleanCache() {
    $.post("CleanOilBrandPriorityCache", { oilBrandPriorityType: 'Region' }, function (result) {
        if (!result.Status) {
            alert(result.Msg);
        }
    });
}
