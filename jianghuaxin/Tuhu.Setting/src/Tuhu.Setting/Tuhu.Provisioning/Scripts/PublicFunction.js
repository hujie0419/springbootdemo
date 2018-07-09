//将radio选中的值给button
/*function GiveButtonValue(btn, radio, hiddenInput) {
//alert(1);
for (var i = 0; i < radio.length; i++) {
if (radio[i].checked) {
//alert(radio[i].nextSibling.nodeValue);
// document.getElementsByName('radio1').item(0).nextSibling.nodeValue
btn.value = radio.item(i).nextSibling.nodeValue;
//alert(btn.value);
//btn.value=radio[i].value;
//alert(btn.value);
//将选中的值给隐藏的渠道文本
hiddenInput.value = radio[i].value;
//alert(hiddenInput.value);
//alert(document.getElementById("hiddenOrderChannel").value);
break;

}
}
}*/

//将radio选中的值给button
function GiveButtonValue(btn, radio) {
    //alert(1);
    for (var i = 0; i < radio.length; i++) {
        if (radio[i].checked) {
            btn.value = radio.item(i).nextSibling.nodeValue;
            break;

        }
    }
}



//得到radio选中的值
function GetRadioValue(obj) {
    for (var i = 0; i < obj.length; i++) {
        if (obj[i].checked) {
            return obj[i].value;
            break;
        }
    }
}
//点击添加新地址的button
function AddNewAdressClick(obj) {
    switch (obj) {
        //如果点击物流的里面的添加新地址          
        case 2:
            document.getElementById("DivLogis_AddNewAddress").style.display = "block";
            break;
            //如果点击快递的里面的添加新地址           
        case 1:
            document.getElementById("DivExpr_AddNewAddress").style.display = "block";
            break;
    }
}
//点击地址编辑button
//obj是a标签本身，inputName是这个a标签里面的所有Input
//function btnExpr_Modify(obj) {
//alert(1);
//}

//得到一个div下面所有的子div，并将其子元素隐藏，obj是父div
function HideDivAllChildrenDiv(obj) {
    var div = obj.getElementsByTagName("DIV");
    for (var i = 0; i < div.length; i++) {
        div[i].style.display = "none";
    }
}
//得到一个div里面所有的div，并将其显示
function ShowDivAllChildrenDiv(obj) {
    var div = obj.getElementsByTagName("DIV");
    for (var i = 0; i < div.length; i++) {
        div[i].style.display = "block";
    }
}
//得到一个div里面所有的div，并将其显示
function ShowInputAllChildrenInput(obj) {
    var div = obj.getElementsByTagName("INPUT");
    for (var i = 0; i < div.length; i++) {
        div[i].style.display = "block";
    }
}

//公共方法
//判断radio有没有选中值,obj是radio对象
function JudgeRadioSelectStatus(obj) {
    var result = false;
    for (var i = 0; i < obj.length; i++) {
        if (obj.item(i).checked) {
            result = true;
        }
    }
    return result;
}
//判断下拉框的值，如果是0的时候，提示
function JudeSelectValue(obj) {
    var result = true;
    if (obj.value == "0" || obj.value == "" || obj.value == "请选择") {
        obj.style.border = "1px solid Red";
        result = false;
    }
    else {
        obj.style.border = "";
    }
    return result;
}

////得到省
//function GetProvince(obj) {
//    jQuery.getJSON('@Url.Action("DisplayRegion", "Order")/' + this.value, function (regions) {
//        var sb = '<option value="">--请选择--</option>';
//        for (var index = 0; index < regions.length; index++) {
//            var region = regions[index];
//            sb += '<option value="' + region.PKID + '">' + region.RegionName + '</option>';
//        }
//        obj.html(sb);
//    });
//}

////得到市
//function GetCity(pro, city) {
//    pro.change(function () {
//        jQuery.getJSON('@Url.Action("DisplayCity", "Order")/' + this.value, function (citys) {
//            var sb = '<option value="">--请选择--</option>';
//            if (citys != false) {
//                for (var index = 0; index < citys.length; index++) {
//                    var city = citys[index];
//                    sb += '<option value="' + city.PKID + '">' + city.RegionName + '</option>';
//                }
//            }
//            city.html(sb);
//        });
//    });
//}

Date.prototype.Format = function (fmt) { //author: meizz 
    var o = {
        "M+": this.getMonth() + 1, //月份 
        "d+": this.getDate(), //日 
        "h+": this.getHours(), //小时 
        "m+": this.getMinutes(), //分 
        "s+": this.getSeconds(), //秒 
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
        "S": this.getMilliseconds() //毫秒 
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}

function ValidateDeliveryCode(obj) {
    var reg = /^[0-9A-Za-z\-]{5,15}(,[0-9A-Za-z\-]{5,15})*$/;
    if ($(obj).val() != "") {
        if (reg.test($(obj).val())) {
            return true;
        }
        else {
            return false;
        }

    }
}

//编辑后弹出层回调函数
function editbackFunDialog(msg, flag, dalogId) {
    if (flag == "0") {
        art.dialog.list[dalogId].close();
        art.dialog({
            icon: 'succeed', lock: true, title: '提示', cancel: false, opacity: .5, content: msg, time: 1,
            close: function () {
                window.location.reload();
            }
        });
    } else if (flag == "1") {
        art.dialog({ icon: 'error', lock: true, title: '提示', cancel: false, opacity: .5, content: msg, time: 1 });
    }
}

//关闭弹出层
function closeDialog(dalogId) {
    art.dialog.list[dalogId].close();
}

/* 请求Ajax 带返回值，并弹出提示框提醒*/
function getAjax(url, parm, callBack) {
    $.ajax({
        type: 'post',
        dataType: "text",
        url: url,
        data: parm,
        cache: false,
        async: false,
        success: function (msg) {
            callBack(msg);
        }
    });
}

//确认提示框
function artConfirm(rtContent, func) {
    if (rtContent == "") {
        rtContent = "确认该操作？";
    }
    art.dialog.confirm(rtContent, func, function () { });
}

//弹窗提醒
function alertDialog(content, flag) {
    var icons = "succeed";
    if (flag == true) {
        icons = "succeed";
    } else {
        icons = "warning";
    }
    art.dialog({ icon: icons, lock: true, title: '提示', cancel: false, opacity: .5, content: content, time: 1.5 });
}

//弹窗提醒
function alertDialogTime(content, flag, timespace) {
    var icons = "succeed";
    if (flag == true) {
        icons = "succeed";
    } else {
        icons = "warning";
    }
    art.dialog({ icon: icons, lock: true, title: '提示', cancel: false, opacity: .5, content: content, time: timespace });
}

/**
   * 进度条 基于artDialog
   * cuibaoxiong 2015.01.08
   * 调用方式（可参考页面“百世物流-同步商品信息到百世”）  开启进度条：miniAjaxLoading().setTitle('正在处理中……')    关闭进度条：miniAjaxLoading().close();
 */
function miniAjaxLoading() {
    var __exports;
    var jsLoadingDialog = art.dialog({
        id: "jsLoadingDialog",
        title: false,
        cancel: false,
        lock: true,
        fixed: true,
        content: "<div style=\"text-align:center; margin:0px; padding:0px;\"><img src=\"/content/images/load.gif\" style=\" margin:0px; padding:0px;\"/><span id=\"ajaxLoading-inner\"></span><div>"
    });

    function __setInfo(_info) {
        var __loadingInner = document.getElementById('ajaxLoading-inner');
        __loadingInner.innerHTML = _info;
    }

    function __close() {
        art.dialog.get("jsLoadingDialog") && art.dialog.get("jsLoadingDialog").close();
    }

    __exports = {
        setInfo: __setInfo,
        close: __close
    }
    return __exports;
}

//将/Date(.....)/ 格式转换为日期格式
function ChangeDateFormat(time) {
    if (time != null) {
        var date = new Date(parseInt(time.replace("/Date(", "").replace(")/", ""), 10));
        var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
        var currentDate = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
        return date.getFullYear() + "/" + month + "/" + currentDate;
    }
    return "";
}

$(document).ready(function() {
    $("#loading").dialog({
        autoOpen: false,
        modal: true,
        closeOnEscape: false,
        resizable: false,
        draggable: false,
        width: 250,
        height: 40
    }).parent().parent().find(".ui-widget-header").hide();
});