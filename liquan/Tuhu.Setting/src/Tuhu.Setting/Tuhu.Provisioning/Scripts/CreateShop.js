$(function () {
    $("form").filtrateSelect();//下拉框选择提示

    //根据省份获取城市
    $("#Provinces").change(function () {
        var self = $(this);
        var parentId = self.val();
        if (parentId != "") {
            $("#Province").val(self.find("option:selected").text());
            var option = GetRegion(parentId);
            $("#Citys").html(option);
            $("#Districts").html("<option>---请选择---</option>");
        } else {
            $("#Citys").html("<option>---请选择---</option>");
            $("#Districts").html("<option>---请选择---</option>");
        }
    });
    //根据城市获取区
    $("#Citys").change(function () {
        var self = $(this);
        var parentId = self.val();
        if (parentId != "") {
            $("#City").val(self.find("option:selected").text());
            $("#RegionID").val(parentId);
            var option = GetRegion(parentId);
            $("#Districts").html(option);
        } else {
            $("#Districts").html("<option>---请选择---</option>");
        }
    });
    $("#Districts").change(function () {
        var self = $(this);
        var parentId = self.val();
        if (parentId != "") {
            $("#District").val(self.find("option:selected").text());
        }
    });
    //根据省份获取城市（覆盖区域）
    $("#Covers").change(function () {
        var self = $(this);
        var parentId = self.val();
        if (parentId != "") {
            var option = GetRegion(parentId);
            $("#Covers2").html(option);
        } else {
            $("#Covers2").html("<option>---请选择---</option>");
        }
    });

    $("#addCover").click(function () {
        var text = $("#Covers2").find("option:selected").text();
        var Cover = $("#Cover").val();
        if (text != "---请选择---") {
            if (Cover != ""){
                if (Cover.indexOf(text)==-1) {
                    $("#Cover").val(Cover + "," + text);
                }
            }
            else
                $("#Cover").val(text);
        }
    });

    $("#delCover").click(function () {
        $("#Cover").val('');
    });
});

//绑定数据
$(document).ready(function () {
    //初始化绑定图片
    var url = $("#hdshopImgUrl").val();
    var Img = $("#Images").val();
    var imgs = Img.split(',');
    for (var i = 0; i < imgs.length; i++) {
        $("#img" + (i + 1)).attr("src", url + imgs[i]);
    }
    //绑定对账周期
    var AccountingPeriod = $("#AccountingPeriod").val();
    if (AccountingPeriod != "" && AccountingPeriod != null) {
        var count = $("#AccountingPeriods option").length;
        for (var i = 0; i < count; i++) {
            var op = $("#AccountingPeriods").get(0).options[i];
            if (op.text == AccountingPeriod) {
                op.selected = true;
                $("#AccountingPeriods").get(0).options[i].selected = true;
                break;
            }
        }
    }

    //初始化绑定省市区
    var province = $("#Province").val();
    if (province != "") {
        //$("#Provinces").find("option[text='" + province + "']").attr("selected", true);//上海市
        var count = $("#Provinces option").length;
        for (var i = 0; i < count; i++) {
            var op = $("#Provinces").get(0).options[i];
            if (op.text == province) {
                //绑定城市
                var option = GetRegion(op.value);
                $("#Citys").html(option);
                $("#Covers2").html(option);
                $("#Districts").html("<option>---请选择---</option>");

                op.selected = true;
                $("#Covers").get(0).options[i].selected = true;
                break;
            }
        }
    }
    var city = $("#RegionID").val();//城市id
    if (city != "") {
        //绑定区
        var option = GetRegion(city);
        $("#Districts").html(option);

        $("#Citys").val(city);
        $("#Covers2").val(city);
    }
    var district = $("#District").val();
    if (district != "") {
        //$("#Districts").find("option[text='" + district + "']").attr("selected", true);
        var count = $("#Districts option").length;
        for (var i = 0; i < count; i++) {
            var op = $("#Districts").get(0).options[i];
            if (op.text == district) {
                op.selected = true;
                break;
            }
        }
    }
});





//获取地区的通用方法
function GetRegion(parentId) {
    var option = "<option>---请选择---</option>";
    $.ajax({
        type: "get",
        url: "/Shop/SelectCity",
        data: { "parentId": parentId },
        async: false,
        success: function (city) {
            $.each(city, function (index, item) {
                option += "<option value=" + item.Pkid + ">" + item.RegionName + "</option>";
            });
        }
    });
    return option;
}


//轮胎保养
$(function () {
    $.ajax({
        cache: false,
        type: "get",
        url: "/Shop/GetTree",
        contenttype: "application/json;charset=utf-8",
        datatype: "json",
        success: function (data) {
            var dt = eval('(' + data + ')');
            $("#ServiceTree").dynatree({
                checkbox: true,
                selectMode: 3,
                children: dt
            });
        }
    });
});
function Open() {
    $("#ServiceList").css("display", "");
}
function Close() {
    $("#ServiceList").css("display", "none");
}
function TreeOK() {
    var nodes = $("#ServiceTree").dynatree("getSelectedNodes");
    var txt = "";
    for (var i = 0; i < nodes.length; i++) {
        if (nodes[i].data.id == "0") {
            txt += "," + nodes[i].data.title;
        }
        var flag = 0;
        if (nodes[i].data.id != "0") {
            var all = txt.split(',');
            for (var j = 0; j < all.length; j++) {
                if (all[j] == nodes[i].data.name) {
                    flag = 1;
                }
            }
            if (flag == 0) {
                txt += "," + nodes[i].data.name;
            }
            var father = $("#ServiceTree").dynatree("getTree").getNodeByKey(nodes[i].data.id);
            var fatitle = father.data.title;
            var allChildren = father.getChildren();
            var countChildren = father.countChildren();
            var f = 1;
            var str = nodes[i].data.name;
            for (var m = 0; m < countChildren; m++) {
                if (allChildren[m].isSelected() == false) {
                    if (f == 1) {
                        str += "（不包含：";
                    }
                    str += allChildren[m].data.title + ",";
                    f++;
                }
            }
            if (f > 1) {
                str = str.substring(0, str.length - 1);
                str += ")";
            }
            txt = txt.replace(nodes[i].data.name, str);
            i += countChildren - f;
        }
    }
    for (var i = 0; i < nodes.length; i++) {
        if (nodes[i].data.title.toString() == "PM2.5滤芯") {
            if (txt[3] == "（") {
                txt = "，保养（包含：PM2.5滤芯；" + txt.substring(4);
            }
            else {
                txt = "，保养(包含：PM2.5滤芯)" + txt.substring(3);
            }
            break;
        }
    }
    $("#Service").val(txt.substring(1));
    $("#ServiceList").css("display", "none");
}


//收款方式
function clicka(i) {
    var str = $("#hdShopPayWay").val().toString().split(",");
    var txt = $("#POS").val();
    if (txt.trim() == "") {
        txt = str[i];
    } else {
        var flag = 0;
        var all = txt.split(",");
        for (var j = 0; j < all.length; j++) {
            if (all[j] == str[i] && all[j] != "支付宝") {
                flag = 1;
                var l = "," + str[i];
                var f = str[i] + ",";
                if (j == 0 && all.length > 1) {
                    txt = txt.replace(f, "");
                } else if (j > 0) {
                    txt = txt.replace(l, "");
                } else {
                    txt = txt.replace(str[i], "");
                }
                break;
            }
        }
        if (flag == 0 && str[i] != "支付宝") {
            txt += "," + str[i];
        }
    }
    $("#POS").val(txt);
}


//验证数据
//请填入简单名
//简单名不能重复
//请填入对外店名
//请填入营业时间
//请填入店全称
//请填入店公司名称
//门店必须是合作门店

//请填入覆盖区域
//省市区
//请填入地址
//请填入短地址
//请填入经纬度--经纬度为数字，且必须以英文逗号隔开

//请填入收款方式

//请上传图片

//密码不能为空至少为8位

//表单提交最后验证
function OrderSubmit() {

    var pkid = $("#PKID").val();
    //简单名不能重复
    var simpleName = $("#SimpleName").val();
    if (simpleName == null || simpleName == "" || simpleName == "请输入") {
        $("#msgSimpleName").html("必填！");
        return false;
    } else {
        var bl = true;
        if (pkid == null || pkid == "" || pkid == "0" || pkid == 0) {
            $.ajax({
                type: "GET",
                url: "/Shop/IsExistSimpleNameHTML",
                data: {
                    "simpleName": simpleName
                },
                async: false,
                success: function (result) {
                    if (result == false) {
                        $("#msgSimpleName").html("重复了");
                        bl = false;
                    }
                }
            });
        }
        if (!bl) {
            return false;
        }
    }
    $("#msgSimpleName").html("*");

    var hdReset = $("#hdReset").val();
    if (hdReset == "False") {
        var types = $("#types").val();
        if (types == null || types[0] != "1") {
            $("#msgTypes").html("门店必须是合作门店！");
            return false;
        } else {
            $("#msgTypes").html("");
        }
    }

    var suspendStartDateTime = $("#SuspendStartDateTime").val();
    var suspendEndDateTime = $("#SuspendEndDateTime").val();
    if ((suspendStartDateTime == "" || suspendStartDateTime == null) && (suspendEndDateTime == null || suspendEndDateTime == "")) {
        $("#lbSuspendTime").html("");
    } else {
        if ((suspendStartDateTime != null && suspendStartDateTime != "") && (suspendEndDateTime != "" && suspendEndDateTime != null)) {
            var start = new Date(suspendStartDateTime);
            var end = new Date(suspendEndDateTime);
            if (end < start) {
                $("#lbSuspendTime").html("开始时间不能小于结束时间");
                return false;
            } else {
                $("#lbSuspendTime").html("");
            }
        } else {
            $("#lbSuspendTime").html("开始时间和结束时间不能只有一个");
            return false;
        }
    }

    var citySelected = $("#Citys").val();
    if (citySelected == null || citySelected == "" || citySelected == "---请选择---") {
        $("#msgCity").html("请选择门店所在城市！");
        return false;
    } else {
        $("#msgCity").html("*");
    }

    var reg = /^((\d+)|(\d+\.\d+))\,((\d+)|(\d+\.\d+))$/;
    if (!reg.test($("#Position").val())) {
        $("#msgPosition").html("经纬度为数字，且必须以英文逗号隔开！");
        return false;
    } else {
        $("#msgPosition").html("*");
    }

    var regMobile = /^1[3|4|5|7|8][0-9]\d{8}$/;
    var Mobile = $("#Mobile").val().trim();
    if (Mobile != "") {
        if (!regMobile.test(Mobile)) {
            $("#msgPhone").html("手机格式不正确！");
            alert("手机格式不正确！");
            return false;
        } else {
            $("#msgPhone").html("");
        }
    }

    $("#msgPassword").html("");
    if (pkid == null || pkid == "" || pkid == "0" || pkid == 0) {
        if ($('#Password').val().length < 8) {
            $("#msgPassword").html("密码不能为空至少为8位");
            return false;
        }
    }

    var images = $("#Images").val();
    if (images == null || images == "") {
        alert("请上传图片");
        return false;
    }
    //var externalPhone = $("#ExternalPhone").val();
    //if (externalPhone == "") {
    //    $("#msgExternalPhone").html("请勾选电话");
    //    $("#ExternalPhone").css("border-color", "red");
    //    return false;
    //} else {
    //    $("#msgExternalPhone").html("*");
    //}
    //编辑时候验证是否可用状态合理性
    //if (pkid > 0) {
    //    var ddlShopIsActiveType = $("#ddlShopIsActiveType").val();
    //    //有权限的编辑的时候判断选择的 门店合作类型
    //    if (ddlShopIsActiveType == undefined) {
    //        var ck = $("#IsActive").attr("checked");
    //        var shopIsActiveType = $("#shopIsActiveType").val();
    //        $("#msgIsActive").html("");
    //        if (ck == "checked" || ck == true) {
    //            //门店可用的时候 只能 选择正常营业/暂停营业 
    //            if (shopIsActiveType == "3PauseCooperation" || shopIsActiveType == "4StopCooperation") {
    //                $("#msgIsActive").html("门店可用（上架）的时候 只能 选择正常营业/暂停营业 ");
    //                return false;
    //            }
    //        } else {
    //            //门店可用的时候 只能 选择暂停合作/终止合作 
    //            if (shopIsActiveType == "1StartBusiness" || shopIsActiveType == "2PauseBusiness") {
    //                $("#msgIsActive").html("门店不可用（下架）的时候 只能 选择暂停合作/终止合作");
    //                return false;
    //            }
    //        }
    //    }
    //}
    return true;
}

function IsPhone() {
    $("#msgPhone").html("");
    var Mobile = $("#Mobile").val().trim();
    var reg = /^1[3|4|5|7|8][0-9]\d{8}$/;
    if (Mobile != "") {
        if (!reg.test(Mobile)) {
            $("#msgPhone").html("手机格式不正确!");
        } else {
            $("#msgPhone").html("");
        }
    }

}

//该经纬度已经存在门店
function IsExistPosition() {
    $("#msgPosition").html("*");
    var position = $("#Position").val();
    var reg = /^((\d+)|(\d+\.\d+))\,((\d+)|(\d+\.\d+))$/;
    if (!reg.test(position)) {
        $("#msgPosition").html("经纬度为数字，且必须以英文逗号隔开！");
    } else {
        $.ajax({
            type: "GET",
            url: "/Shop/IsExistPosition",
            data: { "position": position },
            success: function (result) {
                if (result == false) {
                    $("#msgPosition").html("注意：该经纬度已经存在门店！");
                }
            }
        });
    }
}


