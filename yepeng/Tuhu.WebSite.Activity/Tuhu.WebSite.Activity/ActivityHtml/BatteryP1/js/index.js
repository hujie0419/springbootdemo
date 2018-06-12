
$(function () {
    $(".chooseItem").hide();
    $(".view-more").hide();
    //隐藏客服
    //$(".footer .price").css("width", "45%");
    //$(".footer .service").css("display", "none");
    //$(".footer .pay").css("width", "50%");

    GetHuoDongImage();

    var sty = {
        "width": "100%",
        "height": $(document).height(),
        "background-color": "rgba(0, 0, 0, 0.24)",
        "zIndex": 10000,
        "position": "absolute",
        "top": 0,
        "left": 0,
        "display": "none"
    }
    $("<div class='layer'/>").css(sty).appendTo($("body"));
    if ($(".car-info").length) {

        $(".btn").tap(function (e) {
            $(".notice").toggle();
            $(".notice-bg").toggle();
            //$(".action-icon").toggle();
            //$("#banner").addClass("slideDown-animation");
            //$(".scroll").addClass("move-animation")
            e.stopPropagation();
        })

        $(".pay").tap(function (e) {
            PayClick();
            e.stopPropagation();
        })

        $(".action-icon").tap(function (e) {
            //$(this).toggle();
            $(".notice").toggle();
            $(".notice-bg").toggle();
            e.stopPropagation();
        })

        $(".scroll").on("tap", ".t", function () {
            SelectedProduct(this);
        })

        $(".scroll").on("tap", ".m", function () {
            ToDetail(this);
        })

        $(".view-more").on("tap", "p", function () {
            GetMore(this);
        })

        var $provinces = $("#province-ul"),
            $citys = $("#city-ul");
        $(".areaSelect").on("tap", "li", function (e) {
            var index = $(this).index();
            if (index === 0) {
                if ($citys.css("display") == "block") {
                    return false;
                }
                if ($provinces.css("display") == "block") {
                    $provinces.hide();
                }
                else {
                    $provinces.show();
                }
            }
            else {
                if ($citys.css("display") == "block") {
                    $citys.hide();
                }
                else {
                    $citys.show();
                }
            }
        });
    }

    Login();

   //GetBatteryList(13580);//线上
    //GetBatteryList(575);//

});

function GetHuoDongImage() {

    $.ajax({

        url: "http://faxian.tuhu.cn/Activity/GetBatteryBanner",

        type: 'GET',

        data: {},

        dataType: 'json',

        async: false,

        success: function (data) {
            if (data) {
                $("#LodImg").attr("src", data.Image);
                if (data.Display) {
                    $(".notice").show();
                    $(".notice-bg").show();
                } else {
                    $(".notice").hide();
                    $(".notice-bg").hide();
                }
            }
        }
    })
}

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
            , {
                'param': ""
            }
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
        CountyName = da.County;
    }
    else {
        da = responseData;
        ProvinceName = da.province;

    }

    CountyName = da.district;
    UserId = da.userid;

    Phone = da.phone;

    if (arryZhiXiaSHi.indexOf(ProvinceName) > -1) {
        CityName = CountyName;
        //alert(ProvinceName)
        //alert(CityName)
    } else {
        CityName = da.city;
    }
    SetCookie("UserId", UserId);
    SetCookie("Phone", Phone);
    SetCookie("Uuid", da.uuid);
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
var CountyName = "";
var DomainItem = "http://item.tuhu.cn/";
var arryZhiXiaSHi = ["上海市", "北京市", "天津市", "重庆市"]; //直辖市 

function GetTID(carId, vehicleID, paiLiang, nian) {
    //alert("vehicleID=" + vehicleID)
    //alert("paiLiang=" + paiLiang)
    //alert("nian=" + nian)
    $.ajax({

        url: DomainItem + "Car/SelectVehicle",

        type: 'GET',

        data: {
            "VehicleID": vehicleID, "PaiLiang": paiLiang, "Nian": nian, "IsBattery": true
        },

        dataType: 'jsonp',

        async: false,

        success: function (data) {

            if (data.Code == "1") { //需要选择五级数据 data 五级车型的数据  
                $(".progress").hide();
                $(".cx-container").show();
                var html = "";
                console.log(JSON.stringify(data));
                for (var item in data.SalesName) {
                    html += '<div class="cx" tid="' + data.SalesName[item].TID + '">' + data.SalesName[item].Name + '</div>';
                }
                $(".cx-wrapper").append(html);

                $(".cx").on("click", function (e) {
                    TID = $(this).attr("tid");                 
                    GetBatteryList(TID);
                    $(".cx-container").hide();
                    UpdateDefultCar(carId, vehicleID, paiLiang, nian, TID);
                })

            } else if (data.Code == "0") {//无需选择五级数据 ，直接读取五级默认数据，

                GetWuJi(vehicleID, paiLiang, nian);
            }
        }
    })
}

function UpdateDefultCar(carId, pid, paiLiang, nian, tid) {
    $.ajax({
        method: "GET",
        url: DomainItem + "CarHistory/UpdateCarObject.html",
        data: {
            "CarID": carId,
            "ProductID": pid,
            "IsDefaultCar": true,
            "TID": tid,
            "UserID": UserId,
            "Pailiang": paiLiang,
            "Nian": nian,
            "PropertyList": ""
        },
        dataType: "json"
    }).done(function (data) {       
    }).fail(function (data) {        
    })
}
//五级数据
function GetWuJi(vehicleID, paiLiang, nian) {
    //alert("vehicleID=" + vehicleID)
    //alert("paiLiang=" + paiLiang)
    //alert("nian=" + nian)
    $.ajax({

        url: DomainItem + "Car/GetVehicleSalesName",

        type: 'GET',

        data: {
            "VehicleID": vehicleID, "PaiLiang": paiLiang, "Nian": nian
        },

        dataType: 'jsonp',

        async: false,

        success: function (data) {
            //alert(JSON.stringify(data))
            if (data.Code == "1") {
                TID = data.SalesName[0].TID;
                if (TID == null || TID == "null" || TID == "") {
                    NoneFooter();
                    return false;
                }
                else {
                    GetBatteryList(TID);
                }

            } else {
                NoneFooter();

            }

        }
    })

}

//两级车型，继续添加车型，
function GetPailiang(vehicleID) {
    $.ajax({

        url: DomainItem + "Car/SelectVehicle",

        type: 'GET',

        data: {
            "VehicleID": vehicleID, "PaiLiang": "", "Nian": ""
        },

        dataType: 'jsonp',

        async: false,

        success: function (data) {

            if (data.Code == "1") {

                if (userAgentInfo.indexOf("Android") >= 0) {

                    if (!isAndroidVersion) {
                        NoneFooterForOld();
                        return false;
                    } else {

                        window.WebViewJavascriptBridge.callHandler(
                            'setUserCarInfo'
                            , function () {
                            });
                    }

                } else {
                    if (isIosVersion) {
                        NoneFooter();
                    } else {
                        NoneFooterForOld();
                    }
                }
                NoneFooterForOld();
            } else if (data.Code == "0") {//无服务
                NoneFooter();
            }
            return false;
        }
    })

}
function SelectCarObject(userid) {

    $.ajax({

        url: DomainItem + "CarHistory/SelectCarObject.html",

        type: 'GET',

        data: {
            userID: userid
        },//'{fcd72b80-76a7-dbd9-b750-6bcc349a280e}'UserId

        dataType: 'jsonp',

        async: false,

        success: function (data) {

            if (data.Code == "1" && data.CarHistory.length > 0) {

                for (var obj in data.CarHistory) {

                    if (data.CarHistory[obj].IsDefaultCar == "1") {
                        //alert(JSON.stringify(data.CarHistory[obj]));
                        //console.log(JSON.stringify(data.CarHistory[obj].TID))
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
                        $("#loginImg").css({
                            "background-image": "url(" + VehicleLogin + ")", "background-repeat": " no-repeat", "background-size": "contain"
                        });

                        var plNian = Pailiang + " " + Nian;
                        $("#plNian").html(plNian);

                        if (ProductID != "" && Pailiang == "") {
                            //调pinglian
                            GetPailiang(ProductID);
                        } else {

                            if (TID == "null" || TID == null || TID == "") {

                                GetTID(CarId, ProductID, Pailiang, Nian);
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
                        }


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
                        $("#loginImg").css({
                            "background-image": "url(" + VehicleLogin + ")", "background-repeat": " no-repeat", "background-size": "contain"
                        });
                        var plNian = Pailiang + " " + Nian;
                        $("#plNian").html(plNian);

                        if (ProductID != "" && Pailiang == "") {
                            //调pinglian
                            GetPailiang(ProductID);
                        } else {

                            if (TID == "null" || TID == null || TID == "") {

                                GetTID(CarId, ProductID, Pailiang, Nian);
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
                            //alert(JSON.stringify(GetCookie("carData")));
                            break;
                        }
                    }

                }

            }
            else {

                if (userAgentInfo.indexOf("Android") >= 0) {

                    if (!isAndroidVersion) {
                        NoneFooterForOld();
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
                        NoneFooterForOld();
                        return false;
                    }
                }
            }
        },
        error: function () {
            //alert("错误");
        }
    });
};

function GetQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}

var RegionData = null;
var CurrentCityList = null;//当前的所有城市
var ProductData = null;//全部电瓶信息
var ProvinceList = null;//可配送的省份
var CityList = null;//可配送的市
var CurrentCityItem = null;//当前城市
var SelectedProductItem = null;//选中的产品信息
var ProvinceId = 0;
var CityId = 0;

function GetBatteryList(tid) {
    //alert(tid);
    $.ajax({

        url: 'http://wx.tuhu.cn' + "/Battery/GetBatteryListV1",

        type: 'GET',

        data: {
            "LiYangID": tid, "TID": tid, "r": Math.random()
        },

        dataType: 'jsonp',

        async: false,

        success: function (data) {
            if (data.IsSuccess) {

                $(".btn").removeClass("disabled");
                $(".no-data").hide();
                var AreaProvinceCode = '';
                ProvinceList = data.ResultProvinceList;//省份List
                CityList = data.ResultCityList;//城市List
                ProductData = data.ResultData;//产品List
                //console.log(JSON.stringify(ProductData));

                $.each(ProvinceList, function (k, v) {//动态迭代省份节点

                    AreaProvinceCode += (k == 0 ? "<span class='closebtn'></span>" : "") + "<li onclick='SelectProvinceItem(\"" + v.Pid + "\",this);'  data-value='" + v.Pid + "' data-name='" + v.PName + "' >" + v.PName.substr(0, 5) + "<br/>" + v.PName.substr(5) + "</li>";

                    $("#province-ul").html(AreaProvinceCode);//渲染省份html节点
                });

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
                } else {
                    $("#province-ul").show();
                }

                AutoFillRegionByGPS(ProvinceId, CityId);

                $(".progress").hide();
                //$(".notice").show();
            }
            else {
                NoneFooter();
                return false;
            }
        },
        error: function () {

        }
    });

};

//您的车型无此服务
function NoneFooter() {

    $(".view-more").html("<span style='color:red;font-size:24px;'>您的车型暂无此服务！</span>");
    $(".foot").css("display", "none");
    $(".view-more").css({
        "text-align": "center", "margin-top": "40%", "display": "block"
    });
    $("#province-ul").css("display", "none");
    $(".progress").hide();
    $(".notice-bg").hide();
}

//老版本，无法判断车型是否全；
//提醒完善车型信息或者已经完善了确实没有服务
function NoneFooterForOld() {

    $(".view-more").html("<div style='font-size:24px;'>您的车型信息不完整 <br/>1、请到爱车档案里完善车型参数 <br/>2、有些车型无参数，暂无电瓶服务</div>");
    $(".foot").css("display", "none");
    $(".view-more").css({
        "text-align": "center", "margin-top": "40%", "display": "block", "color": "red"
    });
    $("#province-ul").css("display", "none");
    $(".progress").hide();
    $(".notice-bg").hide();
}

var SelectedPid = null;
//选择省份
function SelectProvinceItem(Pid, CurrentElement) {
    SelectedPid = Pid;
    console.log(Pid);
    $(".provinceid").val(Pid);
    SetCookie("RegionPid", Pid);
    CurrentCityList = CityList.filter(function (k) {
        if (k.Pid == Pid) return !0; return !1;
    });//筛选当前省份对应的城市节点
    var AreaCityCode = '';
    $.each(CurrentCityList, function (k, v) {//迭代城市节点
        AreaCityCode += (k == 0 ? "<span class=''></span>" : "") + "<li onclick='SelectCityItem(\"" + v.Cid + "\",this);'  data-value='" + v.Cid + "' data-name='" + v.CName + "' >" + v.CName.substr(0, 5) + "<br/>" + v.CName.substr(5) + "</li>";
    });
    //增加当前节点选中样式，同时填充填充当前省份标签文本，之后恢复默认城市文本，最后加载对应城市节点
    $("#province").find(".default").html($(CurrentElement).attr("data-name"));
    $("#city").find(".default").html("城市/地区、自治州");
    $("#city-ul").html(AreaCityCode);

    $("#province-ul").css("display", "none");
    $("#city-ul").css("display", "block");
}

//选择城市
function SelectCityItem(Cid, CurrentElement) {
    $(".areaSelect").css("border", "");
    $(".cityid").val(Cid);
    SetCookie("RegionCid", Cid);
    //增加当前节点选中样式，同时填充当前城市标签文本
    $("#city").find(".default").html($(CurrentElement).attr("data-name"));
    CurrentCityItem = CurrentCityList.filter(function (v) { if (Cid == v.Cid) return !0; return !1; })[0];//筛选当前城市节点
    $("#city-ul").hide();
    AppendProudts();

}

function AppendProudts() {
    var dataArrayAll = new Array();
    var i = -1;
    for (var one in ProductData) {

        for (var key in CurrentCityItem.BrandCoverList) {

            if (ProductData[one].Brand == CurrentCityItem.BrandCoverList[key].Brand && CurrentCityItem.BrandCoverList[key].Cid == ProductData[one].CityId) {
                var jsonData = {
                    "ProductID": "",
                    "VariantID": "",
                    "Image": "",
                    "DisplayName": "",
                    "SalesQuantity": "",
                    "CommentTimes": "",
                    "Price": "",
                    "ServiceRemark": "",
                    "CP_ShuXing5": "",
                    "CP_Tab": ""
                };
                jsonData.ProductID = ProductData[one].ProductID;
                jsonData.Image = ProductData[one].Image;
                jsonData.DisplayName = ProductData[one].DisplayName;
                jsonData.SalesQuantity = ProductData[one].SalesQuantity;
                jsonData.CommentTimes = ProductData[one].CommentTimes;
                jsonData.Price = ProductData[one].Price;
                jsonData.VariantID = ProductData[one].VariantID;
                jsonData.CP_Tab = ProductData[one].CP_Tab;
                jsonData.CP_ShuXing5 = ProductData[one].CP_ShuXing5;
                jsonData.ServiceRemark = CurrentCityItem.BrandCoverList[key].ServiceRemark;
                i++;
                dataArrayAll[i] = jsonData;
            }
        }

    }
    var htmlProdiuct = "";
    var currentProductItem = dataArrayAll;
    console.log(JSON.stringify(currentProductItem));
    //dataArraySlicelast = dataArrayAll.slice(4, dataArrayAll.length)
    $(".scroll .product").remove();

    for (var item in currentProductItem) {
        if (currentProductItem[item].ProductID) {
            var _shuxing5 = currentProductItem[item].CP_ShuXing5 || "";
            var shuxing5 = _shuxing5.split("#"),
                tipHtml = "";
            if (!shuxing5[1]) {
                tipHtml = '<span style="display:none" class="tip red">赠小米电源</span><span class="tip green" style="display:none">' + shuxing5[1] + '</span>';
            }
            else {
                tipHtml = '<span style="display:none" class="tip red">赠小米电源</span><span class="tip green">' + shuxing5[1] + '</span>';
            }
            var imgUrl = "";

            if (currentProductItem[item].Image) {
                imgUrl = currentProductItem[item].Image.replace("https", "http");
            }

            htmlProdiuct += ' <div class="product" style="display:none;">' +
                '<div class="t clear">' +
                    '<div class="checkbox">选择</div>' +
                    '<img style="display:none" src="img/icon.png" alt="" class="right" />' +
                    '<span>&yen; <span>' + currentProductItem[item].Price + '</span></span>' +
                    '<p style="display:none;" class="datajson">' + JSON.stringify(currentProductItem[item]) + '</p>' +
            '</div>' +
            '<div class="m clear">' +
                '<img src="' + imgUrl + '" alt="" />' +
                '<div>' +
                    '<div>' + currentProductItem[item].DisplayName + '</div>' +
                     tipHtml +
                    '<div>配送范围：' + currentProductItem[item].ServiceRemark + '</div>' +
                '</div>' +
            '</div>' +
        '</div>';
        } else {

        }
    }
    if (currentProductItem.length > 2) {
        $(".view-more").css("display", "block");
        $(".view-more>p").css("display", "block");
    } else {
        $(".view-more>p").css("display", "none");
    }
    if (htmlProdiuct) {
        $(".view-more").before(htmlProdiuct);
    } else {
        NoneFooter();
    }

    var myScroll = new IScroll('.scroll', {
        mouseWheel: true
    });
    myScroll.refresh();
    $(".product").eq(0).css("display", "block");
    $(".product").eq(0).find("span").addClass("recommend");
    $(".product").eq(0).find(".right").css("display", "block");
    $(".product").eq(1).css("display", "block");

    //默认选择推荐电瓶
    $(".product").eq(0).find(".checkbox").toggleClass("checked");
    SelectedProductItem = JSON.parse($(".product").eq(0).find(".datajson").text());
    SetCookie("currentSelectProduct", JSON.stringify(SelectedProductItem));
    if (SelectedProductItem) {
        $(".p-price").html(SelectedProductItem.Price);
    }
    GetMore();
}

function SelectedProduct(e) {
    $(".product .checkbox").not(e).removeClass("checked");
    $(e).find(".checkbox").toggleClass("checked");
    SelectedProductItem = JSON.parse($(e).parents(".product").find(".datajson").text());
    SetCookie("currentSelectProduct", JSON.stringify(SelectedProductItem));
    $(".p-price").html(SelectedProductItem.Price);
}

function GetMore(e) {
    $(".product").css("display", "block");
    $(".view-more").css("display", "none");
    var myScroll = new IScroll('.scroll', {
        mouseWheel: true
    });
    myScroll.refresh();
}

//结算
function PayClick() {

    SetCookie("vehicleName", Vehicle);
    SetCookie("vehicleLogin", VehicleLogin);
    if (!$(this).hasClass("disabled") && !($(".areaSelect>li").eq(1).find(".selectItem").children(".default").text() == "城市/地区、自治州")) {
        if ($(".p-price").text() != "" && $(".p-price").text() != null) {
            var ProvinceItem = $(".areaSelect>li").eq(0).find(".selectItem").children(".default").text();
            var CityItem = $(".areaSelect>li").eq(1).find(".selectItem").children(".default").text();

            var ProvinceId = $(".provinceid").val();
            var cityId = $(".cityid").val();

            var RegionSelect = {
                "ProvinceName": ProvinceItem, "CityName": CityItem, "ProvinceId": ProvinceId, "CityId": cityId
            };

            //SetCookie("SelectedRegion", JSON.stringify(RegionSelect));
            var selectRegions = encodeURIComponent(JSON.stringify((RegionSelect)));
            //window.name = JSON.stringify(ProductData);

            window.location.href = "http://faxian.tuhu.cn/ActivityHtml/BatteryV1/Order.html?RegionSelect=" + selectRegions + "&v=7";

        } else {
            alert("请选择产品！")
        }

    } else {
        $(".areaSelect").css("border", "2px solid red");
        //alert("请选择地区信息！");
    }
}

function ToDetail(e) {
    var data = JSON.parse($(e).parent().children().find(".datajson").text());
    var pid = data.ProductID;
    var vid = data.VariantID;
    var Parameters = {
        "ProductID": pid,
        "VariantID": vid,
        "ChePingTyPe": "4",
        "FunctionID": "cn.TuHu.Activity.AutomotiveProducts.AutomotiveProductsDetialUI",
        "isPAy": 0
    };
    var Parameters_ios = {
        'productID': pid,
        'VariantID': vid,
        'goodsListDetailType': 2
    };

    if (userAgentInfo.indexOf("Android") >= 0) {
        //window.WebViewJavascriptBridge.callHandler('toActityBridge',
        //    encodeURIComponent(encodeURIComponent(JSON.stringify(Parameters))));
        //  alert(encodeURIComponent(encodeURIComponent(JSON.stringify(Parameters))));
        var hr = "http://item.tuhu.cn/MobileClient/Introduce/" + pid;
        if (vid > 0) {
            hr = hr + "/" + vid + ".html";
        } else {
            hr = hr + ".html";
        }

        location.href = hr;
    }
    else {
        location.href = '#testapp#customSegue#TNGoodsListDetailViewController#' + encodeURIComponent(JSON.stringify(Parameters_ios));

    }
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

