$(function () {
    if (isSupperUser()) {
        $("input[isTuhuVerify='1'],a[isTuhuVerify='1'],div[isTuhuVerify='1'],table[isTuhuVerify='1']").css("display", "");
        return;
    }
    $("input[isTuhuVerify='1'],a[isTuhuVerify='1'],div[isTuhuVerify='1'],table[isTuhuVerify='1']").css("display", "none");
    var result = getKeyInfo();
    var info = result.Info;
    var sign = result.KEY;
    //if (sign != md5("Info=" + info))
    //{
    //    alert("参数被篡改,请重新登录！");
    //    return;
    //}
    var powerRows = eval("(" + getBtnPower(info, "", sign) + ")");
    $.each(powerRows, function (i, item) {
        if (item.BtnType == "1") {
            if ($("input[isTuhuVerify='1'][btnKey='" + item.BtnKey + "']").length > 0)
                $("input[isTuhuVerify='1'][btnKey='" + item.BtnKey + "']").css("display", "");
        }
        else if (item.BtnType == "2") {
            if ($("a[isTuhuVerify='1'][btnKey='" + item.BtnKey + "']").length > 0)
                $("a[isTuhuVerify='1'][btnKey='" + item.BtnKey + "']").css("display", "");
        }
        else if (item.BtnType == "3") {
            if ($("div[isTuhuVerify='1'][btnKey='" + item.BtnKey + "']").length > 0)
                $("div[isTuhuVerify='1'][btnKey='" + item.BtnKey + "']").css("display", "");
        }
        else if (item.BtnType == "4") {
            if ($("table[isTuhuVerify='1'][btnKey='" + item.BtnKey + "']").length > 0)
                $("table[isTuhuVerify='1'][btnKey='" + item.BtnKey + "']").css("display", "");
        }
    });
});
function getKeyInfo() {
    var urlWin = window.location.pathname;
    var infos = urlWin.toLowerCase().replace("http://", "").split('/');
    var result = {};
    var queryStr = window.location.search;
    var key = "";
    if (queryStr.indexOf("KEY=") >= 0) {
        var strs = queryStr.split('&');
        key = strs[0].split('=')[1];
    }
    if (infos.length == 1) {
        result.Info = "Home_Index";
    }
    else if (infos.length == 2) {
        result.Info = infos[1] + "_Index";
    }
    else if (infos.length >= 3) {
        result.Info = infos[1] + "_" + infos[2];
    }
    if (key == "" && $("#hidTuhuViewKey").length > 0) {
        key = $("#hidTuhuViewKey").val();
    }
    result.KEY = key;
    return result;
}
function getBtnPower(info, btnkey, key) {
    var result = "";
    $.ajax({
        url: "/Home/GetBtnPower",
        type: "POST",
        data: {
            Info: info,
            BtnKey: btnkey,
            Key: key
        },
        dataType: "json",
        async: false,
        success: function (obj) {
            if (obj.Status)
                result = obj.Msg;
        }
    });
    return result;
}
function setPower(btnKey, btnType) {
    if (isSupperUser()) {
        if (btnType == "1")
            $("input[isTuhuVerify='1'][btnKey='" + btnKey + "']").css("display", "");
        else if (btnType == "2")
            $("a[isTuhuVerify='1'][btnKey='" + btnKey + "']").css("display", "");
        else if (btnType == "3")
            $("div[isTuhuVerify='1'][btnKey='" + btnKey + "']").css("display", "");
        else if (btnType == "4")
            $("table[isTuhuVerify='1'][btnKey='" + btnKey + "']").css("display", "");
        return;
    }
    if (btnType == "1")
        $("input[isTuhuVerify='1'][btnKey='" + btnKey + "']").css("display", "none");
    else if (btnType == "2")
        $("a[isTuhuVerify='1'][btnKey='" + btnKey + "']").css("display", "none");
    else if (btnType == "3")
        $("div[isTuhuVerify='1'][btnKey='" + btnKey + "']").css("display", "none");
    else if (btnType == "4")
        $("table[isTuhuVerify='1'][btnKey='" + btnKey + "']").css("display", "none");
    var result = getKeyInfo();
    var info = result.Info;
    var sign = result.KEY;
    //if (sign != md5("Info=" + info)) {
    //    alert("参数被篡改,请重新登录！");
    //    return;
    //}
    var powerRows = eval("(" + getBtnPower(info, btnKey, sign) + ")");
    $.each(powerRows, function (i, item) {
        if (item.BtnType == "1") {
            if ($("input[isTuhuVerify='1'][btnKey='" + item.BtnKey + "']").length > 0)
                $("input[isTuhuVerify='1'][btnKey='" + item.BtnKey + "']").css("display", "");
        }
        else if (item.BtnType == "2") {
            if ($("a[isTuhuVerify='1'][btnKey='" + item.BtnKey + "']").length > 0)
                $("a[isTuhuVerify='1'][btnKey='" + item.BtnKey + "']").css("display", "");
        }
        else if (item.BtnType == "3") {
            if ($("div[isTuhuVerify='1'][btnKey='" + item.BtnKey + "']").length > 0)
                $("div[isTuhuVerify='1'][btnKey='" + item.BtnKey + "']").css("display", "");
        }
        else if (item.BtnType == "4") {
            if ($("table[isTuhuVerify='1'][btnKey='" + item.BtnKey + "']").length > 0)
                $("table[isTuhuVerify='1'][btnKey='" + item.BtnKey + "']").css("display", "");
        }
    });
}
function isSupperUser()
{
    var bol = false;
    $.ajax({
        url: "/Home/IsSupperUser",
        type: "POST",
        dataType: "json",
        async: false,
        success: function (obj) {
            if (obj.Status)
                bol = true;
        }
    });
    return bol;
}
function isOprPower(controller, action, btnKey) {
    var bol = false;
    if (isSupperUser())
        return true;
    $.ajax({
        url: "/Home/IsOprPower",
        type: "POST",
        data: {
            Controller: controller,
            Action: action,
            BtnKey: btnKey
        },
        dataType: "json",
        async: false,
        success: function (obj) {
            if (obj.Status)
                bol = true;
        }
    });
    return bol;
}
function setBtnKey(btnKeys) {
    $("div.ui-dialog-buttonset").eq(0).find("button[role='button']").each(function (i, item) {
        $(item).attr("btnKey", btnKeys[i]);
        $(item).attr("isTuhuVerify", 1);
        $(item).css("display", "none");
    });
}
function setWinPower(btnKeys, idOfWin, info, sign) {
    if (isSupperUser()) {
        return;
    }
    setBtnKey(btnKeys);
    $.each(idOfWin, function (i, val) {
        $("#" + val).find("*[isTuhuVerify='1']").css("display", "none");
    });
    var powerRows = eval("(" + getBtnPower(info, "", sign) + ")");
    $.each(powerRows, function (i, item) {
        if ($("*[isTuhuVerify='1'][btnKey='" + item.BtnKey + "']").length > 0)
            $("*[isTuhuVerify='1'][btnKey='" + item.BtnKey + "']").css("display", "");
    });
}
//扩展函数
$.fn.extend({
    newLoad: function (url, data) {
        var obj = this;
        $.ajax({
            url: url,
            data: data,
            dataType: "html",
            async: false,
            type: "get",
            success: function (html) {
                $(obj).html(html);
            }
        });
    }
});

function getGenerallNamedMenus(generalName) {
    var TUHUGeneralNameAttribute = "TUHUGeneralName";

    var generalNamedMenus = new Array();
    var menus = $('a:[' + TUHUGeneralNameAttribute + ']');
    menus.each(function () {
        if ($(this).attr(TUHUGeneralNameAttribute) == generalName) {
            var menuName = $(this).html();
            var initialValue = $(this).attr('TUHUInitialValue');
            if (initialValue == null) {
                initialValue = '';
            }

            // JSON
            var generalNamedMenu = { menuName: menuName, initialValue: initialValue };
            // 对象
            //var generalNamedMenu = new Object();
            //generalNamedMenu.menuName = menuName;
            //generalNamedMenu.initialValue = initialValue;
            generalNamedMenus.push(generalNamedMenu);
        }

        //测试
        //generalNamedMenus.forEach(function(e) {
        //    alert(e);
        //});
    });
    return generalNamedMenus;
}

function handNoPower(obj, type) {
    try {
        var tmp = {};
        if (type.toLowerCase() == "json") {
            tmp = obj;
        }
        else
            tmp = eval("(" + obj + ")");
        if (typeof (tmp.IsPower) != undefined && tmp.IsPower != null) {
            if (!tmp.Status)
                window.location.href = "/Home/ErrorFunc?error=" + tmp.Msg;
        }
    }
    catch (ex) { }
}
