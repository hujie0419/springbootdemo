var currentPage = 1;
var pageSize = 20;
var phoneReg = /^1[0-9]{10}$/;
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
    GetAllBrands();
});

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
    ShowTableInit();
    var phoneNumber = parseInt($("#phone").val()) || "";
    var brand = $("#brand").val();
    $.get("SelectOilBrandPhonePriority", { phoneNumber: phoneNumber, brand: brand, pageIndex: pageIndex, pageSize: pageSize }, function (data) {
        console.log(data);
        if (data && data.Status) {
            currentPage = pageIndex;
            BindData(data);
        }
    });
}

function ShowTableInit()
{
    $("#noData").hide(1);
    $("#showArea").hide();
    $("#loadingData").show(100);
}

function BindData(data) {
    $("#showArea>tbody").empty();
    if (data.Data && data.Data.length > 0) {
        $(data.Data).each(function (index) {
            var html = $("#Template").html();
            html = html
                .replace("$number$", ((index + 1) + (currentPage - 1) * pageSize) + '<span style="display:none">' + data.Data[index].PKID + '</span>')
                .replace("$phoneNumber$", data.Data[index].PhoneNumber)
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
    $dialog.find("[name=phoneNumber]").val($("#phone").val());
    $dialog.find("[name=brand]").val(-1);
    $dialog.find("[name=phoneNumber]")[0].disabled = false; $dialog.find("[name=phoneNumber]").removeClass("disable");
    OpenAddDialog();
    function OpenAddDialog() {
        var $dialog = $("#dialog-addoredit");
        $dialog.dialog("option", "title", "添加排序规则");
        var button1 = {
            text: "保存", click: function () {
                var obj = new Object();
                var phoneStr = $dialog.find("[name=phoneNumber]").val();
                obj.brand = $dialog.find("[name=brand]").val();
                if (phoneStr === "" || obj.brand === "") {
                    alert("手机号或品牌不能为空");
                    return;
                }
                var phoneStrEn = phoneStr.replace("，", ",");
                if (phoneStrEn.indexOf(",") == -1) {
                    var phoneNr = phoneStrEn.trim();
                    if (!(phoneNr && phoneReg.test(phoneNr))) {
                        alert("手机号格式不正确");
                        return;
                    }
                    obj.phoneNumber = phoneNr;
                    MessageConfirm({
                        title: "确认添加", content: "是否确认添加?", confirm: function () {
                            add(obj);
                        }
                    });
                }
                else {
                    var phoneNrs = phoneStrEn.split(",").filter(x => x);
                    var phoneList = new Array();
                    for (var index = 0; index < phoneNrs.length; index++) {
                        var phoneNr = phoneNrs[index].trim();
                        if (!(phoneNr && phoneReg.test(phoneNr))) {
                            alert("手机号格式不正确");
                            return;
                        }
                        phoneList.push(phoneNr);
                    }
                    obj.phoneList = phoneList;


                    MessageConfirm({
                        title: "确认添加", content: "是否确认添加?", confirm: function () {
                            multAdd(obj);
                        }
                    });
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
            $.post("AddOrEditOilBrandPhonePriority", { phoneNumber: obj.phoneNumber, brand: obj.brand }, function (result) {
                if (result.Status) {
                    alert("添加成功");
                    $("#dialog-addoredit").dialog("close");
                    CleanCache();
                    Search(currentPage);
                }
                else {
                    var msg = result.Msg || "";
                    alert("添加失败!" + msg);
                    var repeatPhone = (/1[0-9]{10}/.exec(msg) || new Array())[0];                    
                    if (repeatPhone)
                    {
                        $("#phone").val(repeatPhone);
                        $("#brand").val("");
                        Search(1);
                    }
                }
            });
        }
        function multAdd(obj) {
            $.post("MultAddOilBrandPhonePriority", { phones: obj.phoneList, brand: obj.brand }, function (result) {
                if (result.Status) {
                    alert("批量添加成功");
                    $("#dialog-addoredit").dialog("close");
                    CleanCache();
                    Search(currentPage);
                }
                else {
                    var msg = result.Msg || "";
                    alert("批量添加失败!" + msg);
                    var repeatPhone = (/1[0-9]{10}/.exec(msg) || new Array())[0];
                    if (repeatPhone) {
                        $("#phone").val(repeatPhone);
                        $("#brand").val("");
                        Search(1);
                    }
                }
            });
        }
    }
}

function Edit(self) {
    var $self = $(self), $dialog = $("#dialog-addoredit"), obj = {};
    var PKID = $self.parent().prevAll().eq(2).find('span').html();
    obj.phoneNumber = $self.parent().prevAll().eq(1).html();
    obj.brand = $self.parent().prevAll().eq(0).find('label').html();
    $dialog.find("[name=phoneNumber]").val(obj.phoneNumber);
    $dialog.find("[name=phoneNumber]")[0].disabled = true; $dialog.find("[name=phoneNumber]").addClass("disable");
    if ($dialog.find("[name=brand]")[0].length > 1) {
        $dialog.find("[name=brand]").val(obj.brand);
    }
    else {
        var html = "<option value='" + obj.brand + "'>" + obj.brand + "</option>";
        $dialog.find("[name=brand]").append(html);
        $dialog.find("[name=brand]").val(obj.brand);
    }
    OpenEditDialog(PKID);

    function OpenEditDialog(PKID) {
        var $dialog = $("#dialog-addoredit");
        $dialog.dialog("option", "title", "修改机油品牌推荐优先级");
        var button1 = {
            text: "修改", click: function () {
                var obj = new Object();
                obj.PKID = PKID;
                obj.phoneNumber = $dialog.find("[name=phoneNumber]").val();
                obj.brand = $dialog.find("[name=brand]").val();
                if (obj.brand === "" || obj.phoneNumber === "") {
                    alert("手机号或品牌不能为空");
                    return false;
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
            $.post("AddOrEditOilBrandPhonePriority", obj, function (result) {
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
    var phoneNumber = $self.parent().prevAll().eq(1).html();
    var callback = function () {
        var $self = $(self);
        var pkid = $self.parent().prevAll().eq(2).find('span').html();
        $.post("DeleteOilBrandPriorityByPKID", { type: "phone", pkid: pkid }, function (result) {
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
    MessageConfirm({ title: "温馨提示", content: "确认删除" + phoneNumber + "的推荐吗?", confirm: callback });
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
    var identityID = $(self).parent().prevAll().eq(4).html() || "";
    $.post("GetVehicleOprLog", { logType: 'OilBrandPhonePriority', identityID: identityID }, function (result) {
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
    $.post("CleanOilBrandPriorityCache", { oilBrandPriorityType: 'UserInfo' }, function (result) {
        if (!result.Status) {
            alert(result.Msg);
        }
    });
}


