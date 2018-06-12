$(function () {
    //隐藏客服
    //$(".footer .price").css("width", "45%");
    //$(".footer .service").css("display", "none");
    //$(".footer .pay").css("width", "50%");

    $(".notice, .notice-bg").show();
    //订单页面
    $(".service-info").click(function () {
        $(".notice, .notice-bg").show();
    });

    $(".btn").click(function () {
        $(".notice, .notice-bg").hide();
    })

    var $quanList = $(".quan-list");
    $(".yhq").click(function (e) {
        $(this).find("div").eq(1).addClass("active");
        $quanList.show();
    })
    $(".yhq").hide();


    //加载地址信息集合
    GetUserAddressDetailList(userId, addressList.ProvinceName, addressList.CityName);
    //GetUserAddressDetailList("{FC6C9AEC-4C0C-4281-9E2B-87E4BB20ABB8}", "山东省", "菏泽市");

    //加载优惠券信息集合
    GetCouponsList('{\"' + productList.ProductID + "|" + productList.VariantID + '\":\"1\"}', userId);
    //GetCouponsList("{\"TR-KH-KH15|6\":\"10\"}", "4F79E798-502C-1891-AFE5-156B452BF4B2");
    GetProduct();

})

//判断手机号是否正确
function IsMobile(val) {
    if (val.match(/^[1][3|4|5|6|7|8]{1}[0-9]{9}$/) != null) {
        return true;
    } else {
        return false;
    }
}

//获取页面参数
function GetQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return decodeURIComponent(r[2]); return null;
}

var isIosVersion = false;
var isAndroidVersion = false;

function VersionForIos(version) {
    isIosVersion = true;//true:新版本 false:老版本
}

function VersionForAndroid(version) {
    isAndroidVersion = true;//true:新版本 false:老版本
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
});

//判断系统&版本
function CreateOrderInfo(serialNumbers, payAmount, subject, body, casgiersData, orderNumber) {

    //统计
    Count(orderNumber);

    if (userAgentInfo.indexOf("Android") >= 0) {
        if (isAndroidVersion) {
            window.WebViewJavascriptBridge.callHandler(
                        'toActityBridge'
                        , {
                            'FunctionID': 'cn.TuHu.Activity.OrderSubmit.PayOrderConfirm',
                            'SerialNumbers': serialNumbers,
                            'PayAmount': payAmount,
                            'subject': subject,
                            'body': body,
                            'OrderNO': orderNumber
                        }
                        , function () {
                        });

        }
        else {
            window.WebViewJavascriptBridge.callHandler(
                        'toActityBridge'
                        , {
                            'FunctionID': 'cn.TuHu.Activity.OrderInfomation',
                            'OrderNO': orderNumber
                        }
                        , function () {
                        });
        }
    }
    else {
        if (isIosVersion) {
            iosend("customSegue#THOrderCashiersVC", '{"casgiersData":' + JSON.stringify(casgiersData) + ',"orderNumber":"' + orderNumber + '"}');
        }
        else {
            iosend("customSegue#TNOrderDetailVC", '{ "orderNumber":"' + orderNumber + '"}');
        }
    }
};

//进入iosApp
function iosend(cmd, arg) {
    location.href = '#testapp#' + cmd + '#' + encodeURIComponent(arg);
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

//定义模型类
var DataDetailModel = function () {
    this.BookType = "";//1：到店安装 2:送货上门
    this.Name = "";//收货人姓名
    this.Cellphone = "";//收货人电话
    this.UserId = "";//用户ID
    this.PayMothed = "";//付款方式(1：支付宝)
    this.OrderChannel = "";//订单渠道
    this.PromotionCode = "";//优惠券ID
    this.OrderList = "";//产品列表[数组]
    this.DefaultCar = "";//车型信息集合
    this.DefaultAddress = "";//地址信息集合
}

//定义产品模型类
var OrderDetailModel = function () {
    this.ProductId = ""; //产品编号
    this.VariantId = "";//产品系列编号
    this.Quantity = "";//产品数量
    this.Price = "";//产品价格
}

//定义车型模型类
var CarDetailModel = function () {
    this.CarID = "";//汽车ID
    this.Brand = "";//品牌
    this.ProductID = "";//车型ID
    this.Vehicle = "";//车型名称
    this.LIYANGID = ""; //LYID
    this.Nian = ""; //年份
    this.Pailiang = ""; //排量
    this.TID = "";
    this.VehicleLogin = "";
    this.SalesName = "";
}

//定义地址模型类
var AddressDetailModel = function () {
    this.AddressID = "";// 地址编号
    this.AddressDetail = "";//详细地址
    this.CityID = "";// 市编号
    this.City = "";//市
    this.ProvinceID = "";// 省编号
    this.Province = "";//省
    this.Cellphone = "";//电话
    this.Consignees = "";//收货人
}

//获取用户ID
var userId = GetCookie("UserId");

//产品（电瓶）集合
var productList = JSON.parse(GetCookie("currentSelectProduct"));

//地址集合
var addressList = JSON.parse(GetQueryString("RegionSelect"));

//汽车集合
var carList = JSON.parse(GetCookie("carData"));

//获取产品(电瓶)集合
function GetProduct() {
    var imgUrl = "";

    if (productList.Image) {
        imgUrl = productList.Image.replace("https", "http");
    }

    $(".order-info .prod-name").text(productList.DisplayName);
    $(".order-info .price span").text(productList.Price);
    $(".order-info img").attr("src", imgUrl);
}

//获取地址详细信息集合
function GetUserAddressDetailList(userId, province, city) {
    $("#region").val(province + "" + city);//绑定地区
    $.ajax({
        url: "http://wx.tuhu.cn" + "/Battery/GetUserAddressDetailList",
        type: "GET",
        async: false,
        data: { "userId": userId, "province": province, "city": city },
        dataType: "jsonp",
        success: function (str) {
            var addressModelList = eval('(' + str + ')');
            $(addressModelList).each(function (i) {
                $("#name").val(addressModelList[i].Consignees);//绑定收货人
                $("#mobile").val(addressModelList[i].Cellphone);//绑定联系方式
                $("#address").val(addressModelList[i].AddressDetail);//绑定详细地址
                //$("#region").val(addressModelList[i].Province + "" + addressModelList[i].City);//绑定地区
            });
        }
    });
}

//获取优惠卷集合
function GetCouponsList(products, uerID) {
    //alert("products=" + products)
    //alert("uerID=" + uerID)
    var activityId = "";
    var orderType = "Battery";
    $.ajax({
        url: "http://api.tuhu.cn" + "/User/SelectPromotion",
        type: "POST",
        async: false,
        data: { "Products": products, "UserID": uerID, "ActivityId": activityId, "OrderType": orderType },
        dataType: "jsonp",
        success: function (str) {
            var couponsModelList = JSON.parse(JSON.stringify(str));
            //alert("str=" + JSON.stringify(str));
            if (couponsModelList.Code == "1" && couponsModelList.PromotionCodeList.length > 0) {

                //获取优惠券集合（没有则不显示该模块）            
                var promotionList = couponsModelList.PromotionCodeList;

                if (promotionList.length > 0) {
                    //默认优惠券添加
                    $("#selectQuan").html(promotionList[0].Discount + "元  优惠券");
                    $("#selectQuan").attr("pkid", promotionList[0].PKID);
                    $("#selectQuan").attr("price", promotionList[0].Discount);

                    var templateHTML = "";
                    $(promotionList).each(function (i) {

                        templateHTML += '<div yh="1" pkid="' + promotionList[i].PKID + '" price="' + promotionList[i].Discount + '" class="quan">' +  //active
                                    '<span>' + promotionList[i].Discount + '</span>优惠券 </div>'

                    });
                    templateHTML += '<div yh="0" pkid="" price="0" class="quan"> <span>&nbsp;</span>不使用优惠券</div>';

                    $(".quan-list").append(templateHTML);
                    //$(".quan-list").hide();
                    $(".quan-list div").eq(0).addClass("active")
                    $(".quan").click(function (e) {
                        if ($(this).attr("yh") == "0") {
                            $("#selectQuan").html("不使用优惠券").removeClass("active");
                        } else {
                            $("#selectQuan").html($(this).attr("price") + "元  优惠券").removeClass("active");
                        }

                        $(".quan").not(e).removeClass("active");
                        $(this).toggleClass("active");
                        $("#selectQuan").attr("pkid", $(this).attr("pkid"));
                        $("#selectQuan").attr("price", $(this).attr("price"));
                        $(".quan-list").toggle();

                        Costing();
                        e.stopPropagation();
                    })
                    $(".yhq").show();
                    var myScroll = new IScroll('.container', { mouseWheel: true, click: true });
                    myScroll.refresh();
                }
                else {
                    $(".yhq").hide();
                }
            } else {
                $(".yhq").hide();
            }
            Costing();
        }
    });
    // Costing();
}

//计算实付价格
function Costing() {
    var currentPrice = $("#selectQuan").attr("price");

    if (currentPrice <= 0 || currentPrice == undefined || currentPrice == "undefined") {

        //实付金额
        $(".p-price").html(productList.Price)
    }
    else {

        //实付金额
        var realPayMoney = productList.Price - currentPrice;

        $(".p-price").html(realPayMoney)
    }
}

//表单验证
function SubmitForm() {
    if ($("#name").val().length <= 0) {
        $("#name").attr("placeholder", "请填写完整信息");
        return false;
    }
    if ($.trim($("#mobile").val()).length <= 0) {
        $("#mobile").attr("placeholder", "请填写完整信息");
        return false;
    }
    if (!IsMobile($.trim($("#mobile").val().toString()))) {
        $("#mobile").val("").attr("placeholder", "请输入正确的手机号码");
        return false;
    }
    if ($("#address").val().length <= 0) {
        $("#address").attr("placeholder", "请填写完整信息");
        return false;
    }

    var model = new DataDetailModel();
    var orderArr = new Array();
    orderArr[0] = new OrderDetailModel();//产品模型
    var carModel = new CarDetailModel();//汽车模型
    var addressModel = new AddressDetailModel();//地址模型

    model.BookType = 2;//1：到店安装 2:送货上门
    model.Name = $.trim($("#name").val());//收货人姓名
    model.Cellphone = $.trim($("#mobile").val());//收货人电话
    model.UserId = userId;//用户ID


    //判断Android和IOS
    if (userAgentInfo.indexOf("Android") >= 0) {
        if (isAndroidVersion) {
            model.PayMothed = 4;//(新)线上支付
            model.OrderChannel = "8手机";//订单渠道(安卓)
        }
        else {
            model.PayMothed = 1;//(老)支付宝
            model.OrderChannel = "8手机";//订单渠道(安卓)
        }
    }
    else {
        if (isIosVersion) {
            model.PayMothed = 4;//(新)线上支付
            model.OrderChannel = "JIOS";//订单渠道(ios)
        }
        else {
            model.PayMothed = 1;//(老)支付宝
            model.OrderChannel = "JIOS";//订单渠道(ios)
        }
    }
    model.PromotionCode = $("#selectQuan").attr("pkid");//优惠券ID

    model.BookType = 3;//1：到店安装 2:送货上门3:上门安装
    model.Name = $.trim($("#name").val());//收货人姓名
    model.Cellphone = $.trim($("#mobile").val());//收货人电话
    model.UserId = userId;//用户ID


    //产品
    orderArr[0].ProductId = (productList.ProductID == undefined) ? "" : productList.ProductID;;
    orderArr[0].Quantity = 1;//数量
    orderArr[0].VariantId = (productList.VariantID == undefined) ? "" : productList.VariantID;;
    orderArr[0].Price = (productList.Price == undefined) ? "" : productList.Price;;

    //汽车
    carModel.CarID = carList.CarID;
    carModel.Brand = carList.Brand;
    carModel.ProductID = carList.ProductID;
    carModel.Vehicle = carList.Vehicle;
    carModel.LIYANGID = carList.LIYANGID; //LYID
    carModel.Nian = carList.Nian; //年份
    carModel.Pailiang = carList.Pailiang; //排量
    carModel.TID = carList.TID;
    carModel.VehicleLogin = carList.VehicleLogin;
    carModel.SalesName = carList.SalesName;

    //地址
    addressModel.AddressID = "";
    addressModel.AddressDetail = $.trim($("#address").val());
    addressModel.CityID = addressList.CityId;
    addressModel.City = addressList.CityName;
    addressModel.ProvinceID = addressList.ProvinceId;
    addressModel.Province = addressList.ProvinceName;
    addressModel.Cellphone = $.trim($("#mobile").val());
    addressModel.Consignees = $.trim($("#name").val());

    //产品列表[数组]
    model.OrderList = orderArr;
    //车型信息集合
    model.DefaultCar = carModel;
    //地址信息集合`
    model.DefaultAddress = addressModel;

    var jsonStr = JSON.stringify(model);
    $.ajax({
        url: "http://api.tuhu.cn/Order/CreateOrder?",
        type: "POST",
        dataType: "json",
        data: { "jsonStr": jsonStr },
        success: function (str) {
            var orderModelList = JSON.parse(JSON.stringify(str));
            var serialNumbers = orderModelList.PayInfo.SerialNumbers;
            var payAmount = orderModelList.PayInfo.Price;
            var subject = orderModelList.PayInfo.Title;
            var body = orderModelList.PayInfo.Body;
            var orderNumber = orderModelList.PayInfo.OrderNO;

            //过滤(IOS(老))支付方式（银行卡）
            for (var i = 0; i < orderModelList.Cashier.length; i++) {
                if (orderModelList.Cashier[i].Payment_way_name != "支付宝" && orderModelList.Cashier[i].Payment_way_name != "微信") {
                    delete orderModelList.Cashier[i];
                }
            }
            var casgiersData = orderModelList;

            //alert("返回值（金额）" + casgiersData.PayInfo.Price);
            CreateOrderInfo(serialNumbers, payAmount, subject, body, casgiersData, orderNumber);


            //刷新页面
            window.location.reload();
        }
    });
}

//http://t.tuhu.cn/ord?tm=ww&cp=112&uid=222&o_num=ppdfiodko
function Count(odernum) {
    var uuidstr = "";
    var uid = GetCookie("Uuid");
    if (!GetCookie("Uuid")) {
        uid = "";
    }
    if (userAgentInfo.indexOf("Android") >= 0) {

        uuidstr = "Android|" + uid;
    } else {
        uuidstr = "IOS|" + uid;
    }

    try {
        var e = new Image;
        e.id = "BatteryV1";
        e.src = "http://t.tuhu.cn/ord?tm=" + encodeURIComponent(GetDateTime()) +
                                         "&cp=" + encodeURIComponent(window.location.href) +
                                         "&uid=" + encodeURIComponent(uuidstr) +
                                         "&o_num=" + encodeURIComponent(odernum);

        //$.ajax({
        //    url: "http://t.tuhu.cn/ord",
        //    type: "GET",
        //    dataType: "jsonp",
        //    data: { "tm": myDate.toLocaleString(), "cp": window.location.href, "uid": GetCookie("Uuid"), "o_num": odernum },
        //    success: function (data) {

        //    }
        //});

    } catch (e) {

    }

}


function GetDateTime() {
    var now = new Date();

    var year = now.getFullYear();       //年
    var month = now.getMonth() + 1;     //月
    var day = now.getDate();           //日      
    var hh = now.getHours();            //时
    var mm = now.getMinutes();          //分
    var ss = now.getSeconds();          //秒

    var clock = year + "-";

    if (month < 10) {
        clock += "0";
    }

    clock += month + "-";

    if (day < 10) {
        clock += "0";
    }

    clock += day + " ";

    if (hh < 10) {
        clock += "0";
    }

    clock += hh + ":";
    if (mm < 10) {
        clock += '0';
    }
    clock += mm + ":";

    if (ss < 10) {
        clock += '0';
    }
    clock += ss;
    return clock;
}
