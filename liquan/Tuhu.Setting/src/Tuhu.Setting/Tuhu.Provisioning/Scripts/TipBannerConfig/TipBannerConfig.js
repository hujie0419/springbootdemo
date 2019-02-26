var pageSize = 20;
var staticProducts = new Array();
var operate = '<button class="btn btn-basic" style="width:60px"  onclick="Edit(this)">编辑</button><button class="btn btn-danger" style="width:60px;margin-left:5px"  onclick="Delete(this)">删除</button>';
$(function () {
    $("#dialog-addoredit").dialog({
        title: "添加配置",
        modal: true,
        width: 400,
        autoOpen: false
    });
    GetAllTipBannerTypeConfig();
    Search(1);
});

function GetAllTipBannerTypeConfig() {
    $.get("GetAllTipBannerTypeConfig", function (result) {
        var $bannerType = $("#dialog-addoredit").find("[name=tipBannerType]");
        $bannerType.empty();
        $("#tipBannerType").empty();
        var html = '<option value="-1">-请选择提示条类型-</option>';
        if (result.Status && result.Data.length > 0) {
            for (var i = 0; i < result.Data.length; i++) {
                html += '<option value="' +
                    result.Data[i].TypeId +
                    '">' +
                    result.Data[i].TypeName +
                    '</option>';
            }
            $bannerType.append(html);
            $("#tipBannerType").append(html);
        }
        else {
            html = '<option value="">-无提示条类型-</option>';
            $bannerType.append(html);
            $("#tipBannerType").append(html);
        }
    });
}

function Search(pageIndex) {
    var typeId = $("#tipBannerType").val() || -1;
    $("#loadingData").show(100);
    $("#noData").hide(1);
    $.post("SelectTipBannerDetailConfig", { typeId: typeId, pageIndex: pageIndex, pageSize: pageSize }, function (data) {
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
                .replace("$number$", ((index + 1) + (currentPage - 1) * pageSize))
                .replace("$pkid$", data.Data[index].PKID)
                .replace("$type$", data.Data[index].TypeName + '<span style="display:none">' + data.Data[index].TypeId + '</span>')
                .replace("$isEnabled$", (data.Data[index].IsEnabled ? '<label style="background-color:#0052cc;color:white; padding:3px 4px">开启</label>' : '<label style="background-color:#ee0701;color:white;padding:3px 4px">禁用</label>') + '<span style="display:none">' + (data.Data[index].IsEnabled || false) + '</span>')
                .replace("$icon$", '<img style="width:70px;height:70px;"' + (data.Data[index].Icon ? 'src="' + data.Data[index].Icon + '"' : "") + 'alt="" /><span style=display:none>' + (data.Data[index].Icon || "") + '</span>')
                .replace("$title$", data.Data[index].Title || "")
                .replace("$content$", data.Data[index].Content)
                .replace("$url$", '<a href="' + (data.Data[index].Url || "") + '" target="_blank">' + (data.Data[index].Url || "") + '</a>')
                .replace("$backgroundColor$", '<input type="color" disabled value="' + data.Data[index].BackgroundColor + '" />')
                .replace("$bgTransparent$", data.Data[index].BgTransparent)
                .replace("$contentColor$", '<input type="color" disabled value="' + data.Data[index].ContentColor + '" />')
                .replace("$createDateTime$", DatePrase(data.Data[index].CreateDateTime))
                .replace("$lastUpdateDateTime$", DatePrase(data.Data[index].LastUpdateDateTime))
                .replace("$operate$", operate);
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
    $dialog.dialog("option", "title", "添加");
    $dialog.find("[name = tipBannerType]").val(-1);
    $dialog.find("[name=tipBannerType]")[0].disabled = false;
    $dialog.find("[name=tipBannerType]").removeClass("disable");
    $dialog.find('[name= isEnabled][value=false]').attr('checked', 'true');
    $dialog.find("[name=icon]").val("");
    $dialog.find("[name=upImgUrl]").val("");
    $dialog.find("img").attr("src", "");
    $dialog.find("[name=title]").val("");
    $dialog.find("[name=content]").val("");
    $dialog.find("[name=url]").val("");
    $dialog.find("[name=backgroundColor]")[0].jscolor.fromString("#ffffff");
    $dialog.find("[name=bgTransparent]").val(0);
    $dialog.find("[name=contentColor]")[0].jscolor.fromString("#000000");

    var button1 = {
        text: "添加",
        click: function () {
            if (confirm("确认添加?")) {
                var obj = new Object();
                obj.TypeId = $dialog.find("[name = tipBannerType]").val();
                obj.TypeName = $dialog.find("[name = tipBannerType]").find("option:selected").text();
                obj.IsEnabled = $dialog.find("[name = isEnabled]:checked").val() || false;
                obj.Icon = $dialog.find("[name = icon]").val();
                obj.Title = $dialog.find("[name = title]").val().trim();
                obj.Content = $dialog.find("[name = content]").val().trim();
                obj.Url = $dialog.find("[name = url]").val().trim();
                obj.BackgroundColor = $dialog.find('[name="backgroundColor"]').val();
                obj.BgTransparent = parseFloat($dialog.find('[name="bgTransparent"]').val()) || 0;
                obj.ContentColor = $dialog.find("[name = contentColor]").val();
                if (obj.TypeId < 1) {
                    alert("请选择类别");
                    return;
                }
                if (!obj.Content) {
                    alert("请输入文字");
                    return;
                }
                if (!(obj.BackgroundColor && obj.ContentColor)) {
                    alert("文字/背景颜色不能为空");
                    return;
                }
                if (obj.BgTransparent < 0 || obj.BgTransparent > 1) {
                    alert("背景透明度值在0~1之间");
                    return;
                }
                if (obj.Url && !CheckURL(obj.Url)) {
                    alert("请输入http(s)://开头的完整链接");
                    return;
                }
                add(obj);
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
        $.post("AddTipBannerDetailConfig", obj, function (result) {
            if (result.Status) {
                alert("添加成功!");
                $(".dialog").dialog("close");
                Search(1);
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
    var pkid = $self.parent().prevAll().eq(11).html();
    if (pkid < 1) {
        return;
    }
    if (confirm("确认删除该提示条配置吗？")) {
        $.post("DeleteTipBannerDetailConfig", { pkid: pkid }, function (result) {
            if (result.Status) {
                alert("删除成功");
                Search(1);
            } else {
                var msg = result.Msg || "";
                alert("删除失败！" + msg);
            }
        });
    };
}

function Edit(self) {
    var $self = $(self);
    var $dialog = $("#dialog-addoredit");
    $dialog.dialog("option", "title", "编辑");
    var pkid = $self.parent().prevAll().eq(11).html() || -1;
    var typeId = $self.parent().prevAll().eq(10).find("span").html() || -1;
    var isEnabled = $self.parent().prevAll().eq(9).find("span").html() || false;
    var icon = $self.parent().prevAll().eq(8).find("span").html() || "";
    var title = $self.parent().prevAll().eq(7).text().trim();
    var content = $self.parent().prevAll().eq(6).html().trim();
    var url = $self.parent().prevAll().eq(5).text().trim();
    var backgroundColor = $self.parent().prevAll().eq(4).find("input")[0].value;
    var bgTransparent = $self.parent().prevAll().eq(3).text().trim();
    var contentColor = $self.parent().prevAll().eq(2).find("input")[0].value;
    $dialog.find("[name=tipBannerType]").val(typeId);
    $dialog.find("[name=tipBannerType]")[0].disabled = true;
    $dialog.find("[name=tipBannerType]").addClass("disable");
    $dialog.find('[name= isEnabled][value=' + isEnabled + ']').attr('checked', 'true');
    $dialog.find("[name=upImgUrl]").val("");
    $dialog.find("img").attr("src", icon);
    $dialog.find("[name=icon]").val(icon);
    $dialog.find("[name=title]").val(title);
    $dialog.find("[name=content]").val(content);
    $dialog.find("[name=url]").val(url);
    $dialog.find("[name=backgroundColor]")[0].jscolor.fromString(backgroundColor);
    $dialog.find("[name=bgTransparent]").val(bgTransparent);
    $dialog.find("[name=contentColor]")[0].jscolor.fromString(contentColor);

    var button1 = {
        text: "更新",
        click: function () {
            if (confirm("确认更新提示条配置信息?")) {
                var obj = new Object();
                obj.PKID = pkid;
                obj.TypeId = $dialog.find("[name = tipBannerType]").val();
                obj.IsEnabled = $dialog.find("[name = isEnabled]:checked").val() || false;
                obj.Icon = $dialog.find("[name = icon]").val().trim();
                obj.Title = $dialog.find("[name = title]").val().trim();
                obj.Content = $dialog.find("[name = content]").val().trim();
                obj.Url = $dialog.find("[name = url]").val().trim();
                obj.BackgroundColor = $dialog.find('[name="backgroundColor"]').val();
                obj.BgTransparent = parseFloat($dialog.find('[name="bgTransparent"]').val()) || 0;
                obj.ContentColor = $dialog.find("[name = contentColor]").val();
                if (obj.TypeId < 1) {
                    alert("请选择类别");
                    return;
                }
                if (!obj.Content) {
                    alert("请输入文字");
                    return;
                }
                if (!(obj.BackgroundColor && obj.ContentColor)) {
                    alert("文字/背景颜色不能为空");
                    return;
                }
                if (obj.BgTransparent < 0 || obj.BgTransparent > 1) {
                    alert("背景透明度值在0~1之间");
                    return;
                }
                if (obj.Url && !CheckURL(obj.Url)) {
                    alert("请输入http(s)://开头的完整链接");
                    return;
                }
                if (obj.PKID > 0) {
                    edit(obj);
                }
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
        $.post("UpdateTipBannerDetailConfig", obj, function (result) {
            if (result.Status) {
                alert("更新成功");
                $(".dialog").dialog("close");
                Search(1);
            }
            else {
                var msg = result.Msg || "";
                alert("更新失败!" + msg);
            }
        });
    }
}

function DatePrase(longDate) {
    if (longDate != null) {
        return eval('new ' + eval(longDate).source).Format('yyyy-MM-dd hh:mm:ss');
    } else {
        return "";
    }
}

//添加新类型
function AddType() {
    var div = '<div class="dialog" id="dialog-addType"><table><tr><th>类型:</th><td><input name="typeName" value="" /></td></tr></table></div>';
    $('body').append(div);
    var $dialog = $("#dialog-addType");
    $dialog.dialog({
        title: "添加新类型",
        modal: true,
        width: 400,
        close: function (event, ui) {
            $(this).remove();
        }
    });
    var button1 = {
        text: "确定",
        click: function () {
            var obj = new Object();
            obj.TypeName = $dialog.find("[name=typeName]").val().trim();
            if (!confirm("是否确认添加新类型:" + obj.TypeName + " ?")) {
                return;
            }
            if (!obj.TypeName) {
                alert("新类型不能为空");
                return;
            }
            $.post("AddTipBannerTypeConfig", obj, function (result) {
                if (result.Status) {
                    alert("添加新类型成功");
                    $dialog.dialog("close");
                    GetAllTipBannerTypeConfig();
                }
                else {
                    alert("添加失败" + result.Msg);
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
}

function UploadImg(e) {
    var $e = $(e);
    var $ee = $e.parents("tr").find("td").eq(0).find("img");
    var $image = $e.parents("tr").find("td").eq(0).find("input[class='image']");
    if (!$e.val()) {
        return;
    }
    $.ajaxFileUpload({
        url: '/Article/AddArticleImg2',
        secureuri: false,
        fileElementId: $e.attr("id"),
        dataType: 'json',
        data: {},
        type: "post",
        success: function (result) {
            if (result.BImage != "" && result.SImage != "") {
                $ee.attr("src", result.SImage);
                $image.val(result.BImage);
            } else {
                alert("上传图片失败！");
            }
        }
    });
}

function RefreshCache() {
    if (confirm("确认刷新缓存?")) {
        $.post("RefreshTipBannerConfigCache", function (result) {
            if (result.Status) {
                alert("清除缓存成功");
            }
            else {
                alert("清除缓存失败!");
            }
        });
    }
}

function CheckURL(url) {
    var reg = /(http:\/\/|https:\/\/)((\w|=|\?|\.|\/|&|-)+)/g;
    return reg.test(url);
}
