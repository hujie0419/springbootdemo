var pageSize = 20;

$(function () {
    GetAllLiveWorkShopConfigType();
    Search(1);
});


function GetAllLiveWorkShopConfigType() {
    $.get("GetAllLiveWorkShopConfigType", function (result) {
        $("#typeName").empty();
        var html = '<option value="">-请选择类型-</option>';
        if (result.Status && result.Data.length > 0) {
            for (var i = 0; i < result.Data.length; i++) {
                html += '<option value="' +
                    result.Data[i] +
                    '">' +
                    result.Data[i] +
                    '</option>';
            }
            $("#typeName").append(html);
        }
        else {
            html = '<option value="">-无类型-</option>';
            $("#typeName").append(html);
        }
    });
}

function Search(pageIndex) {
    var typeName = $("#typeName").val() || "";
    $("#loadingData").show(100);
    $("#loadingUpdate").hide(1);
    $("#noData").hide(1);
    $.post("SelectLiveWorkShopConfig", { typeName: typeName, pageIndex: pageIndex, pageSize: pageSize }, function (data) {
        if (data && data.Status) {
            currentPage = pageIndex;
            BindData(data);
        }
        else {
            alert("查询失败");
        }
    });
}

function BindData(data) {
    $("#showArea>tbody").empty();
    if (data.Data && data.Data.length > 0) {
        $(data.Data).each(function (index) {
            var html = $("#Template").html();
            html = html
                .replace("$typeName$", data.Data[index].TypeName)
                .replace("$sortNumber$", data.Data[index].SortNumber)
                .replace("$content$", data.Data[index].Content || "")
                .replace("$picture$", '<img style="width:70px;height:70px;"' + (data.Data[index].Picture ? 'src="' + data.Data[index].Picture + '"' : "") + 'alt="" /><span style=display:none>' + (data.Data[index].Picture || "") + '</span>')
                .replace("$gif$", '<img style="width:70px;height:70px;"' + (data.Data[index].Gif ? 'src="' + data.Data[index].Gif + '"' : "") + 'alt="" /><span style=display:none>' + (data.Data[index].Gif || "") + '</span>')
                .replace("$h5Url$", '<a href="' + (data.Data[index].H5Url || "") + '" target="_blank">' + (data.Data[index].H5Url || "") + '</a>')
                .replace("$pcUrl$", '<a href="' + (data.Data[index].PcUrl || "") + '" target="_blank">' + (data.Data[index].PcUrl || "") + '</a>')
                .replace("$shopId$", data.Data[index].ShopId)
                .replace("$channelName$", data.Data[index].ChannelName || "")
                .replace("$createDateTime$", DatePrase(data.Data[index].CreateDateTime))
                .replace("$lastUpdateDateTime$", DatePrase(data.Data[index].LastUpdateDateTime))
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

function UploadExcelFile(e) {
    var $e = $(e);
    var filePath = $(e).val();
    if (!filePath) {
        return;
    }
    if (!confirm("确认上传文件:" + filePath)) {
        return;
    }
    $("#loadingUpdate").show(100);
    $.ajaxFileUpload({
        url: 'ImportLiveWorkShopConfig',
        secureuri: false,
        fileElementId: $e.attr("id"),
        dataType: 'json',
        data: {},
        type: "post",
        success: function (result) {
            if (result.Status) {
                alert("导入成功");
                Search(1);
            } else {
                alert("上传失败！" + result.Msg);
            }
            $("#LiveWorkShopConfigExcel").val("");
            $("#loadingUpdate").hide(1);
        }
    });
}

function RefreshCache() {
    if (confirm("确认刷新缓存?")) {
        $.post("RefreshLiveWorkShopConfigCache", function (result) {
            if (result.Status) {
                alert("清除缓存成功");
            }
            else {
                alert("清除缓存失败!");
            }
        });
    }
}