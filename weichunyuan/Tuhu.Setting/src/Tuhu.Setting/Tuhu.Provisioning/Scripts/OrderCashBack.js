function Add(target, id) {
    $("#AddMessage").load('/Order/AddCashBackMess?OrderID=' + id);
    $('#AddMessage').dialog({
        title: "新增返现信息",
        width: 750,
        height: 500,
        async: false,
        modal: true,
        buttons: {
            "保存": function () {
                var flag = $("input[name='AddCashBackMoney']").prop("checked");
                if (flag == false && $("#OtherMoney").val() == "") {
                    alert("请填写其他金额！");
                    return false;
                }
                $("#AddForm").ajaxSubmit({
                    url: this.href,
                    success: function (result) {
                        var pkid = $(target).parent().attr("id");
                        switch (result) {
                            case true:
                                $("#" + pkid).children().remove();
                                $("#AddPaypalAccount").val("");
                                $("#AddWeChatID").val("");
                                alert('保存成功！');
                                $("#AddMessage").dialog("close");
                                break;
                            case false:
                                alert("已保存,不能重复保存！");
                                $("#AddMessage").dialog("close");
                                return false;
                                break;
                            default:
                                alert("保存失败！");
                                break;

                        }
                    }
                });
            },
            "取消": function () {
                $("#AddPaypalAccount").val("");
                $("#AddWeChatID").val("");
                $(this).dialog("close"); //当点击取消按钮时，关闭窗口
            }
        }
    });
}

function Edit(target) {
    $('#EditMessage').dialog({
        title: "修改返现信息",
        width: 750,
        height: 500,
        async: false,
        modal: true,
        open: function () {
            var trChildren = $(target).closest("tr").children();
            $("#Order").text(trChildren.eq(0).text().trim());
            $("#Order_No").val(trChildren.eq(0).text().trim());
            $("#PaypalAccount").val(trChildren.eq(2).text().trim());
            $("#WeChatID").val(trChildren.eq(3).text().trim());
            $("#Image").attr("src", trChildren.eq(10).text().trim());
            switch (trChildren.eq(5).text().trim()) {
                case "需付款":
                    $("#NeedPay").prop("checked", true);
                    $("#NotPayRemark").val("");
                    $("#lblRemark").hide();
                    break;
                case "已付款":
                    $("#Payed").prop("checked", true);
                    $("#NotPayRemark").val("");
                    $("#lblRemark").hide();
                    break;
                default:
                    $("#NotPay").prop("checked", true);
                    $("#NotPayRemark").val(trChildren.find("#Remark").text());
                    $("#lblRemark").show();
                    break;
            }
            if (trChildren.eq(7).text().trim() == "支付宝转账") {
                $("#Paypal").prop("checked", true);
            }
            else {
                $("#WeinXin").prop("checked", true);
            }
            if (trChildren.eq(6).text().trim() == "68.00") {
                $("#Money").prop("checked", true);
            }
            else {
                $("#Other").prop("checked", true);
                $("#OtherMoney").val(trChildren.eq(6).text().trim());
            }
        },
        buttons: {
            "保存": function () {
                var flag = $("input[name='CashBackMoney']").prop("checked");
                if (flag == false && $("#OtherMoney").val() == "") {
                    alert("请填写其他金额！");
                    return false;
                }
                $("#EditForm").ajaxSubmit({
                    url: this.href,
                    success: function (result) {
                        if (result != false) {
                            var trChildren = $(target).closest("tr").children();
                            var paypalAccount = $("#PaypalAccount").val();
                            var weChatID = $("#WeChatID").val();
                            var payStatus = $("input[name='PayStatus']:checked").val();
                            var payMethod = $("input[name='PayMethod']:checked").val();
                            var cashBackMoney = 0;
                            if (flag == false) {
                                cashBackMoney = $("#OtherMoney").val();
                            }
                            else {
                                cashBackMoney = 68.00;
                            }
                            trChildren.eq(2).text(paypalAccount);
                            trChildren.eq(3).text(weChatID);
                            trChildren.eq(5).text(payStatus);
                            trChildren.eq(6).text(cashBackMoney);
                            trChildren.eq(7).text(payMethod);
                            trChildren.eq(10).text(result.UploadImg);
                            alert('保存成功！');
                            $('#EditMessage').dialog("close");
                        } else {
                            alert('保存失败！');
                        }
                    }
                });
            },
            "取消": function () {
                $("#Image").attr("src", "");
                $("#OtherMoney").val("");
                $(this).dialog("close"); //当点击取消按钮时，关闭窗口
            }
        }
    });
}

function Delete(target) {
    if (confirm("确认删除？")) {
        var orderNo = $(target).closest("tr").children().eq(0).text().trim();
        $.post("DeleteCashBaskMess", { "OrderNo": orderNo }, function (result) {
            if (result == true) {
                $(target).closest("tr").remove();
            }
        }, "json");
    }
}
//支付宝帐号或微信ID判重
function IsUnique(target) {
    var value = "";
    if (target == "PaypalAccount") {
        value = $("#AddPaypalAccount").val();
    }
    else {
        value = $("#AddWeChatID").val();
    }
    if (value != "") {
        $.post("IsUnique", { "Value": value, "Type": target }, function (data) {
            if (data > 0) {
                alert("该支付宝帐号已存在！");
                $("#Add" + target).val("");
                $("#Add" + target).focus();
                return false;
            }
        }, "json")
    }
}

$(document).ready(function () {
    $("#PaypalAccount,#WeChatID").change(function () {
        var type = $(this).attr("id");
        var value = $(this).val();
        $.post("IsUnique", { "Value": value, "Type": type }, function (data) {
            if (data > 0) {
                alert("该支付宝帐号已存在！");
                $("#" + type).val("");
                $("#" + type).focus();
                return false;
            }
        }, "json")
    });

    //修改时支付状态的改变事件
    $("input[name='PayStatus']").click(function () {
        var target = $(this).val();
        if (target == "暂不支付") {
            $("#lblRemark").show();
        }
        else {
            $("#lblRemark").hide();
            $("#AddNotPayRemark").val("");
        }
    });
})