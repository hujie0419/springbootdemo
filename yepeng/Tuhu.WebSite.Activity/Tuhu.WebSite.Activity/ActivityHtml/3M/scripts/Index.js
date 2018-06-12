$(function () {
    //选择门店
    $(".footer").on("click", function (e) {
        console.log(JSON.stringify(window.name))
        if (window.name) {
            window.location.href = "/ActivityHtml/3M/shop.html";
        }
        e.stopPropagation();
    })

    //选中
    $(".pack-list").on("click", ".info", function (e) {
        $(this).addClass("selected");
        $(".pack-list .info").not(this).removeClass("selected");
        var dataProduct = $(this).find(".data").text();
        window.name = dataProduct;
        //SetCookie("Selected3MPackage", dataProduct);
        e.stopPropagation();
    });

    //详情
    $(".pack-list").on("click", ".viewInfo", function (e) {
        var dataProduct = JSON.parse($(this).parent().parent().find(".data").text());
        var pruductname = $(this).attr("name-data");
        var htmlLi = "";
        for (var item in dataProduct.Items) {
            htmlLi += '<li>' +
                        '<img src="' + dataProduct.Items[item].Image + '" width="100%">' +
                        '<div class="cont">' + dataProduct.Items[item].DisplayName + '</div>' +
                    '</li>';
        }
        var htmlBody = '  <div class="title">' +
                        '<p>' + pruductname + '</p>' +
                        '<span class="close"></span>' +
                    '</div>' +
                      '<div class="showList">' +
                                '<ul>' +
                                htmlLi +
                                '</ul>' +
                            '</div>';
        $(".showBox .showInfo").html(htmlBody);
        $(".showBox").show();
        e.stopPropagation();
    });
    //关闭
    $(".showBox").on("click", ".close", function (e) {
        $(".showBox").hide();
        e.stopPropagation();
    });

    $(document).on({
        "click": function (e) {
            var src = e.target;
            if ($(src).attr("class") && $(src).attr("class") == "showInfo") {
                return false;
            } else {
                $(".showBox").hide();
            }
        }
    });

    //下拉选择效果
    $(".select").on("click", ".value", function () {

        $(".select").not($(this).parents(".select")).removeClass("active");
        $(this).parents(".select").toggleClass("active");
    }).on("click", ".list li", function (e) {
        e.stopPropagation();
        var str = $(this).text();
        $(this).parents(".list").prev().children("span").text(str);
        $(".select").removeClass("active");

        SelectByCategroy(str)
    });

    Login(); //TODO
    //Get3MDataList("VE-GM-S07BT", "1.5L", "2015", "");
    //var userlogin = {
    //    "City": "上海市",
    //    "Province": "上海市",
    //    "District": "",
    //    "UserId": "",
    //    "Uuid": "",
    //    "Phone": "18521709141",
    //    "latitude": 31.738919,
    //    "longitude": 121.409745
    //}
    //SetCookie("UserloginObject3M", JSON.stringify(userlogin))
})

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
        iosend("CallBackAppLoginResponse");
    }
};

function CallBackAppLoginResponse(responseData) {
    var da = null;
    if (userAgentInfo.indexOf("Android") >= 0) {       
        da = JSON.parse(responseData);
        ProvinceName = da.Province;
        UserProvince = da.userProvince;
        UserCity = da.userCity;
        latitude = da.lat;
        longitude = da.lng;
     
    }
    else {
        da = responseData;
        ProvinceName = da.province;
        latitude = da.latitude
        longitude = da.longitude;
    }

    CityName = da.city;
    DistrictName = da.district;
    UserId = da.userid;
    Phone = da.phone;
  
    var userlogin = {
        "City": UserCity ? UserCity : CityName,
        "Province": UserProvince ? UserProvince : ProvinceName,
        "District": DistrictName,
        "UserId": UserId,
        "Uuid": da.uuid,
        "Phone": Phone,
        "latitude": latitude,
        "longitude": longitude
    }
    SetCookie("UserloginObject3M", JSON.stringify(userlogin));
    SelectCarObject(da.userid);
};

//iosApp
function iosend(arg) {
    window.location.href = 'tuhuaction://getuserinfo#1#' + encodeURIComponent(arg);
};

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
                if (userAgentInfo.indexOf("Android") >= 0) {//添加车型
                    window.WebViewJavascriptBridge.callHandler(
                        'setUserCarInfo'
                        , function () {
                        });
                }
            } else if (data.Code == "0") {//无服务
                NoneServer();
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
            "userID": userid
        },
        dataType: 'jsonp',
        async: false,
        success: function (data) {
            if (data.Code == "1" && data.CarHistory.length > 0) {
                for (var obj in data.CarHistory) {
                    if (data.CarHistory[obj].IsDefaultCar == "1") {                     
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
                        $(".car-info .img img").attr("src", VehicleLogin);
                        var plNian = Pailiang + " " + Nian;
                        $(".other .name").html(Vehicle);
                        $(".other .type").html(plNian)
                        if (!Pailiang) {
                            //调Pailiang
                            GetPailiang(ProductID);
                        } else {

                            Get3MDataList(ProductID, Pailiang, Nian, TID);

                            for (var item in data.CarHistory[obj]) {
                                if (!data.CarHistory[obj][item]) {
                                    delete data.CarHistory[obj][item];
                                }
                            }

                            SetCookie("SelectedCarDataFor3M", JSON.stringify(data.CarHistory[obj]));                        
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
                        $(".car-info .img img").attr("src", VehicleLogin);
                        var plNian = Pailiang + " " + Nian;
                        $(".car-info .other .name").html(Vehicle);
                        $(".car-info .other .type").html(plNian)

                        if (!Pailiang) {
                            //调pinglian
                            GetPailiang(ProductID);
                        } else {
                            Get3MDataList(ProductID, Pailiang, Nian, TID);

                            for (var item in data.CarHistory[0]) {
                                if (!data.CarHistory[0][item]) {
                                    delete data.CarHistory[0][item];
                                }
                            }
                            SetCookie("SelectedCarDataFor3M", JSON.stringify(data.CarHistory[0]));                   
                            break;
                        }
                    }
                }
            }
            else {
                if (userAgentInfo.indexOf("Android") >= 0) {
                    window.WebViewJavascriptBridge.callHandler(
                        'setUserCarInfo'
                        , function () {
                        });
                }
                NoneServer()
            }
        },
        error: function () {
            //alert("错误");
        }
    });
};

//取消重复数据
function UniqueCategory(arr) {
    var result = [], hash = {};
    for (var i = 0, elem; (elem = arr[i]) != null; i++) {
        if (!hash[elem]) {
            result.push(elem);
            hash[elem] = true;
        }
    }
    return result;
}

function NoneServer() {
    $(".pack-list").html("<div style='font-size:14px;'>您的爱车暂无此服务！</div>");
    $(".pack-list").css({
        "text-align": "center", "margin-top": "45%", "display": "block", "color": "red"
    });
}

function Get3MDataList(vehicleId, paiLiang, nian, tid) {
    $.ajax({
        method: "GET",
        url: "http://by.tuhu.cn/baoyang/Get3MPackages.html",
        data: { "vehicleId": vehicleId, "paiLiang": paiLiang, "nian": nian, "tid": tid },
        dataType: "jsonp"
    }).done(function (data) {
        if (data.length > 0) {
            PackagesDataAll = data;
            //console.log(JSON.stringify(data))
            var PackagesDataLoad = data;
            var resultCategory = [];
            for (var item in PackagesDataLoad) {
                resultCategory.push(PackagesDataLoad[item].Category);
            }
            for (var item in UniqueCategory(resultCategory)) {
                $(".list").append("<li>" + UniqueCategory(resultCategory)[item] + "</li>")
            }

            CreateHtml(PackagesDataLoad);

        } else {
            NoneServer();
        }

    }).fail(function (data) {
        //alert("错误");
    })
}

function CreateHtml(PackagesDataLoad) {
    console.log(JSON.stringify(PackagesDataLoad[0]))
    //SetCookie("Selected3MPackage", JSON.stringify(PackagesDataLoad[0]));
    window.name = JSON.stringify(PackagesDataLoad[0]);
    $(".pack-list").html("");
    var htmlbody = "";

    for (var item in PackagesDataLoad) {
        var imgIcon = "";
        for (var itemServer in PackagesDataLoad[item].InstallServiceses) {
            if (PackagesDataLoad[item].InstallServiceses[itemServer].PID === "FU-BY-BYQX|1") {
                imgIcon = "jqxt";
                break;
            } else if (PackagesDataLoad[item].InstallServiceses[itemServer].PID === "FU-BY-BYQX|3") {
                imgIcon = "pyz";
                break;
            }
            else if (PackagesDataLoad[item].InstallServiceses[itemServer].PID === "FU-BY-BYQX|2") {
                imgIcon = "sanyuan";
                break;
            }
            else if (PackagesDataLoad[item].InstallServiceses[itemServer].PID === "FU-TU-FadongjiQXJ|1") {
                imgIcon = "fdj";
                break;
            }
            else {//PackagesDataLoad[item].InstallServiceses[itemServer].PID === "FU-BY-XBY|"
                imgIcon = "xby";
                break;
            }
        }

        var tips = new Array;
        var tipsshow = "";
        var tip = PackagesDataLoad[item].Tips || "";
        tips = tip.split("。");
        for (i = 0; i < tips.length - 1; i++) {
            tipsshow += tips[i] + "。<br>";
        }
        var htmlImage = "";
        for (var itemImg in PackagesDataLoad[item].Items) {
            htmlImage += '<div class="swiper-slide"><img src="' + PackagesDataLoad[item].Items[itemImg].Image + '" /></div>'
        }
        htmlbody = '  <li>' +
                    '<a>' +
                        '<div class="info">' +
                          '<div class = "data"  style="display:none">' + JSON.stringify(PackagesDataLoad[item]) + '</div> ' +
                           ' <div class="img ' + imgIcon + '"></div>' +
                           ' <div class="name">' + PackagesDataLoad[item].DisplayName
                               + '<span class="checkm"></span>' +
                            '</div>' +
                            '<div class="price">' +
                                '<span class="red">¥' + PackagesDataLoad[item].Price + '</span>' +
                               ' <span class="grey">¥' + PackagesDataLoad[item].MarketPrice + '</span>' +
                            '</div>' +
                            '   <div class="productimg">' +
                                '<div class="swiper-container">' +
                                    '<div class="swiper-wrapper">' +
                                        htmlImage +
                                    '</div>' +
                                '</div>' +

                                '<span class="viewInfo" name-data="' + PackagesDataLoad[item].DisplayName + '">产品详情<i class="i-arrow"></i></span>' +
                           ' </div>' +
                        '</div>' +
                        '<div class="decs">' +
                            '<ul>' +
                                '<li>' + tipsshow + '</li>' +
                                ' </ul>' +
                        '</div>' +
                    '</a>' +
                '</li>';

        $(".pack-list").append(htmlbody);

        var swiper = new Swiper('.swiper-container', {
            pagination: '.swiper-pagination',
            slidesPerView: 4.5,
            paginationClickable: true,
            spaceBetween: 4,
            freeMode: true
        });
    }
    $(".pack-list .info").eq(0).addClass("selected");
}

function SelectByCategroy(str) {
    var selectData = [];
    if (str == "全部") {
        selectData = PackagesDataAll;
    } else {
        for (var item in PackagesDataAll) {
            if (PackagesDataAll[item].Category == str) {
                selectData.push(PackagesDataAll[item]);
            }
        }
    }
    CreateHtml(selectData);
}

function GetQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}

//写cookie
function SetCookie(name, value) {
    var OneDayTimes = 60 * 60 * 24 * 1000;
    var exp = new Date();
    exp.setTime(exp.getTime() + OneDayTimes * 1);
    document.cookie = name + "=" + escape(value) + ";path=/;expires=" + exp.toGMTString();
}

//获取cookie
function GetCookie(name) {
    var arr = document.cookie.match(new RegExp("(^| )" + name + "=([^;]*)(;|$)"));
    if (arr != null) return unescape(arr[2]); return null;
}


var TID = "";// 6728;
var VehicleLogin = "";
var Vehicle = "";
var UserId = "";
var Phone = "";
var ProvinceName = "";
var CityName = "";
var UserProvince = "";
var UserCity = "";
var DistrictName = "";
var latitude = "";
var longitude = "";
var DomainItem = "http://item.tuhu.cn/";
var arryZhiXiaSHi = ["上海市", "北京市", "天津市", "重庆市"]; //直辖市 
var PackagesDataAll = "";
