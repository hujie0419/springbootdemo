
$(function () {

    $(".tips").hide();

    Login();

    GetBatteryList(1042);

});

var userAgentInfo = navigator.userAgent;

function connectWebViewJavascriptBridge(callback) {
    if (window.WebViewJavascriptBridge) {
        callback(WebViewJavascriptBridge)
    } else {
        document.addEventListener(
            'WebViewJavascriptBridgeReady'
            , function () {
                callback(WebViewJavascriptBridge)
            },
            false
        );
    }
}
connectWebViewJavascriptBridge(function (bridge) {
    bridge.init(function (message, responseCallback) {
        var data = {
            'Javascript Responds': 'Wee!'
        };
        responseCallback(data);
    });
    bridge.registerHandler("functionInJs", function (data, responseCallback) {
        var responseData = "Javascript Says Right back aka!";
        responseCallback(responseData);
    });
})

//判断登录
function Login() {
    if (userAgentInfo.indexOf("Android") >= 0) {
        window.WebViewJavascriptBridge.callHandler(
            'actityBridge'
            , { 'param': "" }
            , function (responseData) {

                if (responseData != null) {
                    CallBackAppLoginResponse(responseData);
                }
            });
    }
    else {
        //iosend("Userinfo", "CallBackAppLoginResponse");
        iosend("aboutTuhuUserinfologin", "CallBackAppLoginResponse");
    }
};

function CallBackAppLoginResponse(responseData) {
    var da = null;
    if (userAgentInfo.indexOf("Android") >= 0) {
        da = JSON.parse(responseData);
        ProvinceName = da.Province;
    }
    else {
        da = responseData;
        ProvinceName = da.province;
    }
    CityName = da.city;
    UserId = da.userid;
    Phone = da.phone;
    SetCookie("UserId", UserId);
    SetCookie("Phone", Phone);
    SelectCarObject(da.userid);
};
//进入iosApp
function iosend(cmd, arg) {
    location.href = '#testapp#' + cmd + '#' + encodeURIComponent(arg);
};

var TID = "";// 6728;
var VehicleLogin = "";
var Vehicle = "";
var UserId = "";
var Phone = "";
var ProvinceName = "";
var CityName = "";
var strurl = "http://item.tuhu.cn/";

function GetTID(vehicleID, paiLiang, nian) {
    //alert("vehicleID=" + vehicleID)
    //alert("paiLiang=" + paiLiang)
    //alert("nian=" + nian)
    $.ajax({

        url: strurl + "Car/GetVehicleSalesName",

        type: 'GET',

        data: { "VehicleID": vehicleID, "PaiLiang": paiLiang, "Nian": nian },

        dataType: 'jsonp',

        async: false,

        success: function (data) {
            //alert(JSON.stringify(data))
            if (data.Code == "1") {
                TID = data.SalesName[0].TID;
                if (TID == null || TID == "null" || TID == "") {
                    $(".tips").show();
                    $(".loading").hide();
                    return false;
                }
                else {
                    GetBatteryList(TID);
                }

            } else {
                $(".loading").hide();
                $(".tips").show();

            }

        }
    })

}
function SelectCarObject(userid) {

    $.ajax({

        url: strurl + "CarHistory/SelectCarObject.html",

        type: 'GET',

        data: { userID: userid },//'{fcd72b80-76a7-dbd9-b750-6bcc349a280e}'UserId

        dataType: 'jsonp',

        async: false,

        success: function (data) {

            if (data.Code == "1" && data.CarHistory.length > 0) {

                for (var obj in data.CarHistory) {

                    if (data.CarHistory[obj].IsDefaultCar == "1") {
                        //alert(JSON.stringify(data.CarHistory[obj]));
                        console.log(JSON.stringify(data.CarHistory[obj].TID))
                        var CarId = data.CarHistory[obj].CarID;
                        var LIYANGID = data.CarHistory[obj].LIYANGID;
                        var Nian = data.CarHistory[obj].Nian;
                        var Pailiang = data.CarHistory[obj].Pailiang;
                        TID = data.CarHistory[obj].TID;
                        var ProductID = data.CarHistory[obj].ProductID;
                        Vehicle = data.CarHistory[obj].Vehicle;
                        var Brand = data.CarHistory[obj].Brand;
                        VehicleLogin = 'http://image.tuhu.cn' + data.CarHistory[obj].VehicleLogin;
                        var SalesName = data.CarHistory[obj].SalesName;
                        $("#vehicleName").html(Vehicle);
                        $("#loginImg").attr("src", VehicleLogin);


                        if (userAgentInfo.indexOf("Android") >= 0) {
                            if (ProductID == "" || Nian == "" || Pailiang == "") {
                                if (!isAndroidVersion) {
                                    $(".car-tip").show();
                                } else {
                                    //alert(GetCookie("countSelect"));
                                    if (GetCookie("countSelect") != ProductID) {

                                        SetCookie("countSelect", ProductID)
                                        window.WebViewJavascriptBridge.callHandler(
                                            'setUserCarInfo'
                                            , function () {
                                            });
                                    } else {
                                        $(".loading").hide();
                                        $(".tips").show();
                                    }

                                }
                            }

                        } else {
                            if (ProductID == "" || Nian == "" || Pailiang == "") {
                                $(".car-tip").show();
                                return false;
                            }

                        }

                        if (TID == "null" || TID == null) {

                            GetTID(ProductID, Pailiang, Nian);
                        } else {

                            GetBatteryList(TID);
                        }

                        var chePram = {
                            "CarID": data.CarHistory[obj].CarID,
                            "LIYANGID": data.CarHistory[obj].LIYANGID,
                            "Nian": data.CarHistory[obj].Nian,
                            "Pailiang": data.CarHistory[obj].Pailiang,
                            "TID": data.CarHistory[obj].TID,
                            "ProductID": data.CarHistory[obj].ProductID,
                            "Vehicle": data.CarHistory[obj].Vehicle,
                            "Brand": data.CarHistory[obj].Brand,
                            "VehicleLogin": 'http://image.tuhu.cn' + data.CarHistory[obj].VehicleLogin,
                            "SalesName": data.CarHistory[obj].SalesName
                        };

                        SetCookie("carData", JSON.stringify(chePram));
                        //alert(JSON.stringify(GetCookie("carData")));
                        break;
                    } else {

                        var CarId = data.CarHistory[0].CarID;
                        var LIYANGID = data.CarHistory[0].LIYANGID;
                        var Nian = data.CarHistory[0].Nian;
                        var Pailiang = data.CarHistory[0].Pailiang;
                        TID = data.CarHistory[0].TID;
                        var ProductID = data.CarHistory[0].ProductID;
                        Vehicle = data.CarHistory[0].Vehicle;
                        var Brand = data.CarHistory[0].Brand;
                        VehicleLogin = 'http://image.tuhu.cn' + data.CarHistory[0].VehicleLogin;
                        var SalesName = data.CarHistory[0].SalesName;
                        $("#vehicleName").html(Vehicle);
                        $("#loginImg").attr("src", VehicleLogin);


                        if (userAgentInfo.indexOf("Android") >= 0) {

                            if (ProductID == "" || Nian == "" || Pailiang == "") {
                                if (!isAndroidVersion) {
                                    $(".car-tip").show();
                                    return false;
                                } else {

                                    if (GetCookie("countSelect") != ProductID) {
                                        SetCookie("countSelect", ProductID)
                                        window.WebViewJavascriptBridge.callHandler(
                                            'setUserCarInfo'
                                            , function () {
                                            });
                                    } else {
                                        $(".loading").hide();
                                        $(".tips").show();
                                    }
                                }
                            }

                        } else {
                            if (ProductID == "" || Nian == "" || Pailiang == "") {
                                if (!isIosVersion) {
                                    $(".car-tip").show();
                                    return false;
                                }
                            }

                        }

                        if (TID == "null" && TID == null) {

                            GetTID(ProductID, Pailiang, Nian);
                        } else {
                            GetBatteryList(TID);
                        }

                        var chePram = {
                            "CarID": data.CarHistory[0].CarID,
                            "LIYANGID": data.CarHistory[0].LIYANGID,
                            "Nian": data.CarHistory[0].Nian,
                            "Pailiang": data.CarHistory[0].Pailiang,
                            "TID": data.CarHistory[0].TID,
                            "ProductID": data.CarHistory[0].ProductID,
                            "Vehicle": data.CarHistory[0].Vehicle,
                            "Brand": data.CarHistory[0].Brand,
                            "VehicleLogin": 'http://image.tuhu.cn' + data.CarHistory[0].VehicleLogin,
                            "SalesName": data.CarHistory[0].SalesName
                        };

                        SetCookie("carData", JSON.stringify(chePram));
                        //alert("carData=" + JSON.stringify(GetCookie("carData")));
                    }

                }

            }
            else {

                if (userAgentInfo.indexOf("Android") >= 0) {

                    if (!isAndroidVersion) {
                        $(".car-tip").show();
                        return false;
                    } else {
                        // alert("安卓setUserCarInfo")
                        window.WebViewJavascriptBridge.callHandler(
                            'setUserCarInfo'
                            , function () {
                            });
                    }
                } else {
                    if (!isIosVersion) {
                        $(".car-tip").show();
                        return false;
                    }
                }
            }
        },
        error: function () {
            alert("错误");
        }
    });
};

function GetQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}

var RegionData = null;
var CurrentCityList = null;
var ProductData = null;
var ProvinceList = null;
var CityList = null;

var ProvinceId = 0;
var CityId = 0;

function GetBatteryList(tid) {
    //alert(tid);
    $.ajax({

        url: 'http://wx.tuhu.cn' + "/Battery/GetBatteryList",

        type: 'GET',

        data: { "LiYangID": tid, "TID": tid, "r": Math.random() },

        dataType: 'jsonp',

        async: false,

        success: function (data) {

            if (data.IsSuccess) {
                $(".loading").hide();
                $(".btn").removeClass("disabled");
                $(".no-data").hide();
                var AreaProvinceCode = '';
                ProvinceList = data.ResultProvinceList;//省份List
                CityList = data.ResultCityList;//城市List
                ProductData = data.ResultData;//产品List

                //console.log(JSON.stringify(ProvinceList));
                console.log(JSON.stringify(ProductData));
                $.each(ProvinceList, function (k, v) {//动态迭代省份节点
                    AreaProvinceCode += (k == 0 ? "<span class='closebtn'></span>" : "") + "<li onclick='SelectProvinceItem(\"" + v.Pid + "\",this);'  data-value='" + v.Pid + "' >" + v.PName + "</li>";
                    $(".areaSelect>li:eq(0)").find(".chooseItem").html(AreaProvinceCode);//渲染省份html节点
                });
                ResgistAreaEvent();//动态加载省份节点之后注册区域单击事件

                if (ProvinceName != null && ProvinceName != "") {

                    for (var item in ProvinceList) {
                        if (ProvinceList[item].PName == ProvinceName) {
                            ProvinceId = ProvinceList[item].Pid;
                        }
                    }
                    if (CityName != null && CityName != "") {

                        for (var item in CityList) {
                            if (CityList[item].CName == CityName) {
                                CityId = CityList[item].Cid;
                            }
                        }
                    }
                }

                console.log(ProvinceName);
                console.log(CityName);
                console.log(CityId);
                console.log(ProvinceId);
                AutoFillRegionByGPS(ProvinceId, CityId);
            }
            else {
                $(".tips").show();
                $(".loading").hide();
            }
        },
        error: function () {

        }
    });

};
//选择省份
function SelectProvinceItem(Pid, CurrentElement) {

    $(".provinceid").val(Pid);
    SetCookie("RegionPid", Pid);
    CurrentCityList = CityList.filter(function (k) { if (k.Pid == Pid) return !0; return !1; });//筛选当前省份对应的城市节点
    var AreaCityCode = '';
    $.each(CurrentCityList, function (k, v) {//迭代城市节点
        AreaCityCode += (k == 0 ? "<span class=''></span>" : "") + "<li onclick='SelectCityItem(\"" + v.Cid + "\",this);'  data-value='" + v.Cid + "' >" + v.CName + "</li>";
    });
    //增加当前节点选中样式，同时填充填充当前省份标签文本，之后恢复默认城市文本，最后加载对应城市节点
    $(CurrentElement).addClass("current").siblings().removeClass("current").parent().prev().find(".default").html($(CurrentElement).html()).parent().parent().next().find(".default").html("城市/地区、自治州").parent().next(".chooseItem").html(AreaCityCode);

    $(".areaSelect>li:eq(0)").find(".chooseItem").css("display", "none");
    $(".areaSelect>li:eq(1)").find(".chooseItem").css("display", "block");
}
//选择城市
function SelectCityItem(Cid, CurrentElement) {
    $(".areaSelect").css("border", "");
    $(".cityid").val(Cid);
    SetCookie("RegionCid", Cid);
    //增加当前节点选中样式，同时填充当前城市标签文本
    $(CurrentElement).addClass("current").siblings().removeClass("current").parent().prev().find(".default").html($(CurrentElement).html());
    CurrentCityItem = CurrentCityList.filter(function (v) { if (Cid == v.Cid) return !0; return !1; })[0];//筛选当前城市节点

    SetCookie("CurrentCityItem", encodeURIComponent(JSON.stringify(CurrentCityItem)));
    //alert(JSON.stringify(decodeURIComponent(GetCookie("CurrentCityItem"))));
    console.log(JSON.stringify(CurrentCityItem));
    $(".areaSelect>li:eq(1)").find(".chooseItem").hide();
}

//注册单击区域事件
function ResgistAreaEvent() {

    $(".selectItem").on("click", function () {
        if ($(this).siblings(".chooseItem").is(":visible")) {
            $(this).removeClass('active').siblings(".chooseItem").hide();
        }
        else {
            $(".selectItem").removeClass('active').not($(this)).next("ul").hide();
            $(this).addClass('active').siblings(".chooseItem").show();
        }
    });
    $(document).bind("click", function (e) {
        var target = $(e.target);
        if (target.closest(".selectItem").length == 0) {
            $(".selectItem").removeClass('active');

        }
    });
    $(".chooseItem li").on("click", function () {
        var str = $(this).text();
        $(this).parents(".chooseItem").prev(".selectItem").find(".default").text(str);
        $(this).parents(".chooseItem").hide();
    })
}

//搜索电瓶
function BtnClick() {

    SetCookie("vehicleName", Vehicle);
    SetCookie("vehicleLogin", VehicleLogin);
    if (!$(this).hasClass("disabled") && !($(".areaSelect>li:eq(1)").find(".selectItem").children(".default").text() == "城市/地区、自治州")) {
        var ProvinceItem = $(".areaSelect>li:eq(0)").find(".selectItem").children(".default").text();
        var CityItem = $(".areaSelect>li:eq(1)").find(".selectItem").children(".default").text();

        var ProvinceId = $(".provinceid").val();
        var cityId = $(".cityid").val();

        var RegionSelect = { "ProvinceName": ProvinceItem, "CityName": CityItem, "ProvinceId": ProvinceId, "CityId": cityId };

        //SetCookie("SelectedRegion", JSON.stringify(RegionSelect));
        var prm = encodeURIComponent(JSON.stringify((RegionSelect)));
        window.name = JSON.stringify(ProductData);

        window.location.href = "/ActivityHtml/Battery/page.html?RegionSelect=" + prm + "&v=11";//+ escape(JSON.stringify((RegionSelect)));

        //调用方法 如         
        //Post('http://faxian.tuhu.cn/ActivityHtml/Battery/page.html', { "ProductData": ProductData });
    } else {
        $(".areaSelect").css("border", "2px solid red");
        //alert("请选择地区信息！");
    }
}

function Post(URL, PARAMS) {
    var temp = document.createElement("form");
    temp.action = URL;
    temp.method = "post";
    temp.style.display = "none";
    for (var x in PARAMS) {
        var opt = document.createElement("textarea");
        opt.name = x;
        opt.value = PARAMS[x];
        //alert(opt.value)
        temp.appendChild(opt);
    }
    document.body.appendChild(temp);
    temp.submit();
    return temp;
}

//根据cookie自动填充省市
function AutoFillRegion() {
    var RegionPid = GetCookie("RegionPid"), RegionCid = GetCookie("RegionCid");
    if (RegionPid != null && $("[data-value='" + RegionPid + "']").length > 0) SelectProvinceItem(RegionPid, $("[data-value='" + RegionPid + "']"));
    if (RegionCid != null && $("[data-value='" + RegionCid + "']").length > 0) SelectCityItem(RegionCid, $("[data-value='" + RegionCid + "']"));
}
//写cookie
function SetCookie(name, value) {
    var OneDayTimes = 3600 * 24;
    var exp = new Date();
    exp.setTime(exp.getTime() + OneDayTimes * 7 * 1000);
    document.cookie = name + "=" + escape(value) + ";path=/;expires=" + exp.toGMTString();
}
//获取cookie
function GetCookie(name) {
    var arr = document.cookie.match(new RegExp("(^| )" + name + "=([^;]*)(;|$)"));
    if (arr != null) return unescape(arr[2]); return null;
}

//根据定位自动填充省市
function AutoFillRegionByGPS(RegionPid, RegionCid) {
    if (RegionPid != null && $("[data-value='" + RegionPid + "']").length > 0) SelectProvinceItem(RegionPid, $("[data-value='" + RegionPid + "']"));
    if (RegionCid != null && $("[data-value='" + RegionCid + "']").length > 0) SelectCityItem(RegionCid, $("[data-value='" + RegionCid + "']"));
}


var isIosVersion = false;
var isAndroidVersion = false;
//IOS版本判断
function VersionForIos(version) {
    isIosVersion = true;
}

//安卓版本判断
function VersionForAndroid(version) {
    isAndroidVersion = true;

}

