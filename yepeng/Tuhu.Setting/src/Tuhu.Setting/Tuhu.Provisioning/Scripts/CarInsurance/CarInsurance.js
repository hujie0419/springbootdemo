var editor = new baidu.editor.ui.Editor();
editor.render('txteditor');

function updateFAQ() {
    var data = {};
    $("#tipText").html("确定更新?");
    $("#opConfirm").dialog({
        resizable: false,
        height: "auto",
        width: 300,
        modal: true,
        buttons: {
            "确定": function () {
                $(this).dialog("close");
                data["FAQ"] = editor.getContent();
                $.ajax({
                    url: "/CarInsurance/UpdateFAQ",
                    type: "POST",
                    data: data,
                    success: function (result) {
                        if (result.status <= 0)
                            alert(result.msg);
                    }
                })
            },
            "取消": function () {
                $(this).dialog("close");
            }
        }
    });
}

function HandlerPicResult(result, elementId) {
    $("#bannerImg").val(result.BImage);
    $("#bannerPic").attr("src", result.BImage);
}

function HandlerPicResult1(result, elementId) {
    $("#insuranceImg").val(result.BImage);
    $("#insurancePic").attr("src", result.BImage);
}

function uploadPic(_fileElementId, _callback, hwProporLimit) {
    $.ajaxFileUpload({
        url: 'ImageUploadToAli',
        secureuri: false,
        fileElementId: _fileElementId || '',
        data: { hwProporLimit: hwProporLimit || 0 },
        dataType: 'json',
        async: false,
        success: function (result) {
            if (result.BImage) {
                _callback(result, _fileElementId);
            }
            else {
                alert("上传图片失败!" + (result.Msg || ""));
            }
        }
    });
}

$(function () {
    $('.sorted_table').sortable({
        containerSelector: 'table',
        itemPath: '> tbody',
        itemSelector: 'tr',
        placeholder: '<tr class="placeholder"/>'
    });
    var oldIndex;
    $('.sorted_head tr').sortable({
        containerSelector: 'tr',
        itemSelector: 'th',
        placeholder: '<th class="placeholder" />',
        vertical: false,
        onDragStart: function ($item, container, _super) {
            oldIndex = $item.index();
            $item.appendTo($item.parent());
            _super($item, container);
        },
        onDrop: function ($item, container, _super) {
            var field,
                newIndex = $item.index();

            if (newIndex != oldIndex) {
                $item.closest('table').find('tbody tr').each(function (i, row) {
                    row = $(row);
                    if (newIndex < oldIndex) {
                        row.children().eq(newIndex).before(row.children()[oldIndex]);
                    } else if (newIndex > oldIndex) {
                        row.children().eq(newIndex).after(row.children()[oldIndex]);
                    }
                });
            }

            _super($item, container);
        }
    });
    $('.sorted_table').sortable("disable");
});

var bannerId = "";
function bannerSave() {
    $("#tipText").html("确定提交?");
    $("#opConfirm").dialog({
        resizable: false,
        height: "auto",
        width: 300,
        modal: true,
        buttons: {
            "确定": function () {
                $(this).dialog("close");
                var frm = $("#bannerForm");
                var data = getFormJson(frm);
                if (data["displayPage"] == "全部")
                    data["displayPage"] = "All";
                else if (data["displayPage"] == "营销页面")
                    data["displayPage"] = "Marketing";
                else if (data["displayPage"] == "常规页面")
                    data["displayPage"] = "Normal";
                $.ajax({
                    url: "/CarInsurance/CreateBanner",
                    type: "POST",
                    data: data,
                    success: function (result) {
                        if (result.status > 0)
                            setTimeout(function () { location.reload(); }, 3000);
                        else
                            alert(result.msg);
                    }
                })
            },
            "取消": function () {
                $(this).dialog("close");
            }
        }
    })

}

function bannerUpdate() {
    $("#tipText").html("确定提交?");
    $("#opConfirm").dialog({
        resizable: false,
        height: "auto",
        width: 300,
        modal: true,
        buttons: {
            "确定": function () {
                var frm = $("#bannerForm");
                var data = getFormJson(frm);
                data["bannerId"] = bannerId;
                if (data["displayPage"] == "全部")
                    data["displayPage"] = "All";
                else if (data["displayPage"] == "营销页面")
                    data["displayPage"] = "Marketing";
                else if (data["displayPage"] == "常规页面")
                    data["displayPage"] = "Normal";
                $(this).dialog("close");
                $.ajax({
                    url: "/CarInsurance/UpdateBanner",
                    type: "POST",
                    data: data,
                    success: function (result) {
                        if (result.status > 0)
                            setTimeout(function () { location.reload(); }, 3000);
                        else
                            alert(result.msg);
                    }
                })
            },
            "取消": function () {
                $(this).dialog("close");
            }
        }
    })


}

function bannerSwitch(btn) {
    var data = {};
    data["bannerId"] = $(btn).parent().parent().children().first().html();
    if ($(btn).html() == "启用") {
        data["isEnable"] = 1;
        $("#tipText").html("确定启用？");
    }
    else {
        data["isEnable"] = 0;
        $("#tipText").html("确定禁用？");
    }
    $("#opConfirm").dialog({
        resizable: false,
        height: "auto",
        width: 300,
        modal: true,
        buttons: {
            "确定": function () {
                $(this).dialog("close");
                $.ajax({
                    type: 'post',
                    url: "/CarInsurance/UpdateBannerIsEnable",
                    data: data,
                    success: function (result) {
                        if (result.status > 0) {
                            if ($(btn).html() == "禁用") {
                                $(btn).removeClass("btn-danger");
                                $(btn).addClass("btn-success");
                                $(btn).html("启用");
                                $(btn).parent().prev().html("禁用");
                            }
                            else {
                                $(btn).removeClass("btn-success");
                                $(btn).addClass("btn-danger");
                                $(btn).html("禁用");
                                $(btn).parent().prev().html("启用");
                            }
                        }
                        else
                            alert(result.msg);
                    }
                })
            },
            "取消": function () {
                $(this).dialog("close");
            }
        }
    });
}

function bannerDelete(btn) {
    var data = {};
    $("#tipText").html("确定删除？");
    $("#opConfirm").dialog({
        resizable: false,
        height: "auto",
        width: 300,
        modal: true,
        buttons: {
            "删除": function () {
                $(this).dialog("close");
                data["bannerId"] = $(btn).parent().parent().children().first().html();
                $.ajax({
                    url: "/CarInsurance/DeleteBanner",
                    type: "POST",
                    data: data,
                    success: function (result) {
                        if (result.status > 0) {
                            $(btn).parent().parent().remove();
                        }
                        else
                            alert(result.msg);
                    }
                })
            },
            "取消": function () {
                $(this).dialog("close");
            }
        }
    });
}

function bannerEdit(btn) {
    $("#bannerConfigPanel").dialog({
        title: "Banner滚图配置",
        width: 600,
        modal: true
    });
    var tr = $(btn).parent().parent();
    bannerId = $(tr).children().first().html();
    $("#bannerForm [name='name']").val($(tr).children().first().next().html());
    $("#bannerForm [name='img']").val($(tr).children().first().next().next().children().first().attr("src"));
    $("#bannerForm [name='linkUrl']").val($(tr).children().first().next().next().next().children().first().html());
    $("#bannerPic").attr("src", $(tr).children().first().next().next().find("img").attr("src"));
    var displayPage = $(tr).children().first().next().next().next().next().html();
    var btns = $("#bannerForm [name='displayPage']").next().children();
    if ($(btns).first().html() == displayPage)
        $(btns).first().trigger("click");
    else if ($(btns).first().next().html() == displayPage)
        $(btns).first().next().trigger("click");
    else if ($(btns).first().next().next().html() == displayPage)
        $(btns).first().next().next().trigger("click");

    $("#bannerForm").children().last().children().first().attr("onclick", "bannerUpdate()");
    $("#bannerForm").children().last().children().first().html("更新");
}

function bannerSort() {
    if ($("#bannerDragBtn").html() == "拖动排序") {
        $("#bannerTableBody").sortable("enable");
        $("#bannerDragBtn").removeClass("btn-primary");
        $("#bannerDragBtn").addClass("btn-success");
        $("#bannerDragBtn").html("保存排序");
    }
    else {
        $("#tipText").html("确定保存排序?")
        $("#opConfirm").dialog({
            resizable: false,
            height: "auto",
            width: 300,
            modal: true,
            buttons: {
                "保存": function () {
                    $(this).dialog("close");
                    $("#bannerTableBody").sortable("disable");
                    $("#bannerDragBtn").removeClass("btn-success");
                    $("#bannerDragBtn").addClass("btn-primary");
                    $("#bannerDragBtn").html("拖动排序");
                    var trs = $("#bannerTableBody").children();
                    var ids = "";
                    trs.each(function (index, item) {
                        ids += $(item).children().first().html() + ",";
                    })
                    var data = {};
                    data["bannerIds"] = ids;
                    $.ajax({
                        url: "/CarInsurance/UpdateBannerIndex",
                        type: "POST",
                        data: data,
                        success: function (result) {
                            if (result.status < 0)
                                alert(result.msg);

                        }
                    })
                },
                "取消": function () {
                    $(this).dialog("close");
                }
            }
        });
    }

}

//将form中的值转换为键值对。
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

var insurancePartnerId = "";

function insuranceSwitch(btn) {
    var data = {};
    data["insurancePartnerId"] = $(btn).parent().parent().children().first().html();
    if ($(btn).html() == "启用") {
        data["isEnable"] = 1;
        $("#tipText").html("确定启用?");
    }
    else {
        data["isEnable"] = 0;
        $("#tipText").html("确定禁用?");
    }
    $("#opConfirm").dialog({
        resizable: false,
        height: "auto",
        width: 300,
        modal: true,
        buttons: {
            "确定": function () {
                $(this).dialog("close");
                $.ajax({
                    url: "/CarInsurance/UpdateInsuranceIsEnable",
                    type: "POST",
                    data: data,
                    success: function (result) {
                        if (result.status > 0) {
                            if ($(btn).html() == "禁用") {
                                $(btn).removeClass("btn-danger");
                                $(btn).addClass("btn-success");
                                $(btn).html("启用");
                                $(btn).parent().prev().html("禁用");
                            }
                            else {
                                $(btn).removeClass("btn-success");
                                $(btn).addClass("btn-danger");
                                $(btn).html("禁用");
                                $(btn).parent().prev().html("启用");
                            }
                        }
                        else
                            alert(result.msg);
                    }
                })
            },
            "取消": function () {
                $(this).dialog("close");
            }
        }
    });
}

function insuranceEdit(btn) {
    $("#insuranceConfigPanel").dialog({
        title: '车险合作伙伴配置',
        width: 600,
        modal: true
    });
    var tr = $(btn).parent().parent();
    insurancePartnerId = $(tr).children().first().html();

    //获取信息
    var data = {};
    data["insurancePartnerId"] = insurancePartnerId;
    $.ajax({
        url: "/CarInsurance/SelectInsurancePartnerById",
        type: "POST",
        data: data,
        success: function (result) {
            if (result.status <= 0)
                alert(result.msg);
            else {
                var insurance = result.result;
                $("#insuranceForm [name='name']").val(insurance["Name"]);
                $("#insuranceForm [name='img']").val(insurance["Img"]);
                $("#insuranceForm [name='linkUrl']").val(insurance["LinkUrl"]);
                $("#insurancePic").attr("src", insurance["Img"]);
                $("#insuranceForm [name='insuranceId']").val(insurance["InsuranceId"]);
                $("#insuranceForm [name='remarks']").val(insurance["Remarks"]);
                $("#insuranceForm [name='regionCode']").val(insurance["RegionCode"]);
                $("#insuranceForm [name='providerCode']").val(insurance["ProviderCode"]);
                $("#insuranceForm [name='title']").val(insurance["Title"]);
                $("#insuranceForm [name='subTitle']").val(insurance["SubTitle"]);
                $("#insuranceForm [name='tagText']").val(insurance["TagText"]);
                $("#insuranceForm [name='tagColor']").val(insurance["TagColor"]);
                $("#insuranceForm [name='displayIndex']").val(insurance["DisplayIndex"]);
                var status = $("#insuranceForm [name='isEnable']");
                var btns = $(status).next().children();
                if (insurance["IsEnable"] == 1)
                    $(btns).first().trigger("click");
                else if (insurance["IsEnable"] == 0)
                    $(btns).first().next().trigger("click");

                if (insurance["ProviderCode"]) {
                    $("#withparam").attr("checked", "checked");
                    $("#withparam").trigger("change");
                } else {
                    $("#withoutparam").attr("checked", "checked");
                    $("#withoutparam").trigger("change");
                }

                var regionIds = insurance["Regions"].substr(0, insurance["Regions"].length - 1);
                var regionArray = $(regionIds.split(','));
                $("#insuranceForm [data-type='region']").remove();
                $.each(regionArray, function (index, v) {
                    var rs = v.split('-');
                    fill(rs[0], rs[1]);
                })

                $("#insuranceConfigSubmit").attr("onclick", "insuranceUpdate()");
                $("#insuranceConfigSubmit").html("更新");
            }

        }
    })
}

function insuranceRegion(btn) {
    $("#locationConfigPanel").dialog({
        title: '地区配置',
        width: 600,
        modal: true
    });
    var tr = $(btn).parent().parent();
    insurancePartnerId = $(tr).children().first().html();
    $.ajax({
        url: "/CarInsurance/SelectRegionByInsuranceId",
        type: "POST",
        data: { insurancePartnerId: insurancePartnerId },
        success: function (result) {
            if (result.status < 0) {
                alert(result.msg);
            }
            else {
                var regionIds = result.result + "";
                $("#regionForm [data-type='region']").remove();
                regionIds = regionIds.substr(0, regionIds.length - 1);
                var regionArray = $(regionIds.split(','));
                $.each(regionArray, function (index, v) {
                    var rs = v.split('-');
                    fill1(rs[0], rs[1]);
                })
            }
        }
    })
}

function GetRegions(frm) {
    var items = $(frm).children();
    var regions = "";
    $.each(items, function (index, item) {
        if ($(item).attr("data-type") == "region") {
            var regionID = $(item).children().first().next().next().children().first().children('option:selected').val();
            if (regionID == undefined)
                regionID = $(item).children().first().next().children().first().children('option:selected').val();
            regions += regionID + ",";
        }
    })
    return regions;
}

function insuranceSave() {

    $("#tipText").html("确定保存?")
    $("#opConfirm").dialog({
        resizable: false,
        height: "auto",
        width: 300,
        modal: true,
        buttons: {
            "确定": function () {
                $(this).dialog("close");
                var frm = $("#insuranceForm");
                var data = getFormJson(frm);
                data["regions"] = GetRegions(frm);
                if (data.regions) {
                    var regions = data.regions.split(",").filter(function (x) { return Boolean(x) });
                    for (var i = 0; i < regions.length; i++) {
                        if (regions.findIndex(function (item, index) { return index != i && item == regions[i]; }) != -1) {
                            alert("地区有重复，请确认");
                            return;
                        }
                    }
                }
                $.ajax({
                    url: "/CarInsurance/CreateInsurance",
                    type: "POST",
                    data: data,
                    success: function (result) {
                        if (result.status > 0)
                            setTimeout(function () { location.reload(); }, 3000);
                        else
                            alert(result.msg);
                    }
                })
            },
            "取消": function () {
                $(this).dialog("close");
            }
        }
    });
}

function insuranceDelete(btn) {
    var data = {};
    $("#tipText").html("确定删除?");
    $("#opConfirm").dialog({
        resizable: false,
        height: "auto",
        width: 300,
        modal: true,
        buttons: {
            "删除": function () {
                $(this).dialog("close");
                data["insurancePartnerId"] = $(btn).parent().parent().children().first().html();
                $.ajax({
                    url: "/CarInsurance/DeleteInsurance",
                    type: "POST",
                    data: data,
                    success: function (result) {
                        if (result.status > 0) {
                            $(btn).parent().parent().remove();
                        }
                        else {
                            alert(result.msg);
                        }
                    }
                })
            },
            "取消": function () {
                $(this).dialog("close");
            }
        }
    });

}

function insuranceRegionUpdate() {
    $("#tipText").html("确定更新?");
    $("#opConfirm").dialog({
        resizable: false,
        height: "auto",
        width: 300,
        modal: true,
        buttons: {
            "确定": function () {
                $(this).dialog("close");
                var frm = $("#regionForm");
                var selects = frm.children();
                var data = {};
                data["regions"] = GetRegions(frm);
                if (data.regions) {
                    var regions = data.regions.split(",").filter(function (x) { return Boolean(x) });
                    for (var i = 0; i < regions.length; i++) {
                        if (regions.findIndex(function (item, index) { return index != i && item == regions[i]; }) != -1) {
                            alert("地区有重复，请确认");
                            return;
                        }
                    }
                }
                data["insurancePartnerId"] = insurancePartnerId;
                $.ajax({
                    url: "/CarInsurance/UpdateInsuranceRegions",
                    type: "POST",
                    data: data,
                    success: function (result) {
                        if (result.status)
                            setTimeout(function () { location.reload(); }, 3000);
                        else
                            alert(result.msg);
                    }
                })
            },
            "取消": function () {
                $(this).dialog("close");
            }
        }
    })
}

function insuranceUpdate() {

    $("#tipText").html("确定更新?");
    $("#opConfirm").dialog({
        resizable: false,
        height: "auto",
        width: 300,
        modal: true,
        buttons: {
            "确定": function () {
                $(this).dialog("close");
                var frm = $("#insuranceForm")
                var data = getFormJson(frm);
                data["insurancePartnerId"] = insurancePartnerId;
                data["regions"] = GetRegions(frm);
                if (data.regions) {
                    var regions = data.regions.split(",").filter(function (x) { return Boolean(x) });
                    for (var i = 0; i < regions.length; i++) {
                        if (regions.findIndex(function (item, index) { return index != i && item == regions[i]; }) != -1) {
                            alert("地区有重复，请确认");
                            return;
                        }
                    }
                }
                $.ajax({
                    url: "/CarInsurance/UpdateInsurance",
                    type: "POST",
                    data: data,
                    success: function (result) {
                        if (result.status > 0)
                            setTimeout(function () { location.reload(); }, 3000);
                        else
                            alert(result.msg);
                    }
                })
            },
            "取消": function () {
                $(this).dialog("close");
            }
        }
    })
}

function insuranceSort() {
    if ($("#insuranceDragBtn").html() == "拖动排序") {
        $("#insuranceTableBody").sortable("enable");
        $("#insuranceDragBtn").removeClass("btn-primary");
        $("#insuranceDragBtn").addClass("btn-success");
        $("#insuranceDragBtn").html("保存排序");
    }
    else {
        $("#tipText").html("确定保存排序?");
        $("#opConfirm").dialog({
            resizable: false,
            height: "auto",
            width: 300,
            modal: true,
            buttons: {
                "确定": function () {
                    $(this).dialog("close");
                    $("#insuranceTableBody").sortable("disable");
                    $("#insuranceDragBtn").removeClass("btn-success");
                    $("#insuranceDragBtn").addClass("btn-primary");
                    $("#insuranceDragBtn").html("拖动排序");
                    var trs = $("#insuranceTableBody").children();
                    var ids = "";
                    trs.each(function (index, item) {
                        ids += $(item).children().first().html() + ",";
                    })
                    var data = {};
                    data["insurancePartnerIds"] = ids;
                    $.ajax({
                        url: "/CarInsurance/UpdateInsuranceIndex",
                        type: "POST",
                        data: data,
                        success: function (result) {
                            if (result.status <= 0)
                                alert(result.msg);
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

//添加地区
function addregion(btn) {
    var $parent = $(btn).parent().parent().parent();
    var template = "<option value='$value$'>$text$</option>"
    $.ajax({
        url: "/CarInsurance/GetAllProvince",
        type: "POST",
        success: function (provinces) {
            var options = "<option value=''>--请选择--</option>";
            $.each(provinces, function (index, province) {
                options += template.replace("$value$", province.ProvinceId).replace("$text$", province.ProvinceName);
            });
            var html = "<div class='form-group' data-type='region'><label class='col-sm-3 control-label'>地区</label><div class='col-sm-3'><select onchange='GetCitysByProvinceId(this)'>" + options +
                "</select></div><div class='col-sm-4'><select><option value=''>--请选择--</option></select></div><div class='col-sm-2'><button class='btn btn-default' type='button' onclick='removeRegion(this)'>删除</button></div></div>";
            $parent.append(html);
        }
    });
}

function fill(provinceId, cityId) {
    var $form = $("#insuranceForm");
    var template = "<option value='$value$'>$text$</option>";
    var template2 = "<option value='$value$' selected='selected'>$text$</option>";
    $.ajax({
        url: "/CarInsurance/GetAllProvince",
        type: "POST",
        success: function (provinces) {
            var options = "<option value=''>--请选择--</option>";
            $.each(provinces, function (index, province) {
                if (province.ProvinceId != provinceId) {
                    options += template.replace("$value$", province.ProvinceId).replace("$text$", province.ProvinceName);
                }
                else {
                    options += template2.replace("$value$", province.ProvinceId).replace("$text$", province.ProvinceName);
                }
            });
            var $div = $("<div class='form-group' data-type='region'><label class='col-sm-3 control-label'>地区</label><div class='col-sm-3'><select onchange='GetCitysByProvinceId(this)'>" + options +
                "</select></div><div class='col-sm-4'><select><option value=''>--请选择--</option></select></div><div class='col-sm-2'><button class='btn btn-default' type='button' onclick='removeRegion(this)'>删除</button></div></div>");
            $form.append($div);
            GetCitysByProvinceId($div.find('select')[0], cityId);
        }
    });
}

function fill1(provinceId, cityId) {
    var $form = $("#regionForm");
    var template = "<option value='$value$'>$text$</option>";
    var template2 = "<option value='$value$' selected='selected'>$text$</option>";
    $.ajax({
        url: "/CarInsurance/GetAllProvince",
        type: "POST",
        success: function (provinces) {
            var options = "<option value=''>--请选择--</option>";
            $.each(provinces, function (index, province) {
                if (province.ProvinceId != provinceId) {
                    options += template.replace("$value$", province.ProvinceId).replace("$text$", province.ProvinceName);
                }
                else {
                    options += template2.replace("$value$", province.ProvinceId).replace("$text$", province.ProvinceName);
                }
            });
            var $div = $("<div class='form-group' data-type='region'><label class='col-sm-3 control-label'>地区</label><div class='col-sm-3'><select onchange='GetCitysByProvinceId(this)'>" + options +
                "</select></div><div class='col-sm-4'><select><option value=''>--请选择--</option></select></div><div class='col-sm-2'><button class='btn btn-default' type='button' onclick='removeRegion(this)'>删除</button></div></div>");
            $form.append($div);
            GetCitysByProvinceId($div.find('select')[0], cityId);
        }
    });
}

function removeRegion(obj) {
    $(obj).parent().parent().remove();
}

function AddBanner() {
    bannerId = "";
    $("#bannerForm").get(0).reset();
    $("#bannerPic").attr("src", " ");
    $("#bannerForm").children().last().children().first().attr("onclick", "bannerSave()");
    $("#bannerForm").children().last().children().first().html("保存");
    $('#bannerConfigPanel').dialog({
        width: 600,
        title: 'Banner滚图配置',
        modal: true
    });
}

function AddPartner() {
    insurancePartnerId = "";
    $("#insuranceForm").get(0).reset();
    $("#insurancePic").attr("src", " ");
    $("div[data-type='region']").remove();
    $("#insuranceConfigSubmit").attr("onclick", "insuranceSave()");
    $("#insuranceConfigSubmit").html("保存");
    $('#insuranceConfigPanel').dialog({
        title: '车险合作伙伴配置',
        width: 600,
        modal: true
    });

    $("#withparam").attr("checked", "checked");
    $("#withparam").trigger("change");
}

$("[name='paramtype']").change(function () {
    if ($("#withparam").attr("checked") == "checked") {
        $("[name='providerCode']").removeAttr("disabled");
        $("[name='insuranceId']").removeAttr("disabled");
        $("[name='regionCode']").removeAttr("disabled");
        $("[name='linkUrl']").attr("disabled", "disabled");
    } else {
        $("[name='providerCode']").attr("disabled", "disabled");
        $("[name='insuranceId']").attr("disabled", "disabled");
        $("[name='regionCode']").attr("disabled", "disabled");
        $("[name='linkUrl']").removeAttr("disabled");
    }
});

function UpdateBannerCache() {
    $.post("UpdateBannerCache", function (rsp) {
        if (rsp.Status) {
            alert("清除Banner缓存成功");
        }
        else {
            alert("清除Banner缓存失败")
        }
    });
}

function UpdateFooterCache() {
    $.post("UpdateFooterCache", function (rsp) {
        if (rsp.Status) {
            alert("清除Footer缓存成功");
        }
        else {
            alert("清除Footer缓存失败")
        }
    });
}

function UpdatePartnerCache() {
    $.post("UpdatePartnerCache", function (rsp) {
        if (rsp.Status) {
            alert("清除Partner缓存成功");
        }
        else {
            alert("清除Partner缓存失败")
        }
    });
}
