$(function () {

    //日历
    var currYear = (new Date()).getFullYear();
    var opt = {};
    var now = new Date();
    var tomorrow = new Date(now.setDate(now.getDate() + 2));
    opt.date = { preset: 'date' };
    opt.datetime = { preset: 'datetime' };
    opt.time = { preset: 'time' };
    opt.default = {
        preset: 'date', //日期
        theme: 'android-ics light', //皮肤样式
        display: 'modal', //显示方式
        mode: 'scroller', //日期选择模式
        dateFormat: 'yy-mm-dd', // 日期格式
        setText: '确定', //确认按钮名称
        cancelText: '取消', //取消按钮名籍我
        dateOrder: 'yymmdd', //面板中日期排列格式
        dayText: '日',
        monthText: '月',
        yearText: '年', //面板中年月日文字
        startYear: currYear,//开始年份
        endYear: 2050, //结束年份
        minDate: tomorrow//明天
    };
    $("#carUseTime").mobiscroll($.extend(opt['date'], opt['default']));

    UserloginObject3M = JSON.parse(GetCookie("UserloginObject3M"));
    $(".userinfo .tel").val(UserloginObject3M.Phone)
    //到店时间默认显示时间
    var now2 = new Date(now.setDate(now.getDate()));
    $("#carUseTime").val(now2.FormatDate("yyyy-MM-dd"));

    //选择的门店
    var shopname = GetQueryString("name") + '<i>' + GetQueryString("km") + '</i>'
    $(".md .choose").html(shopname)

    $(".products .title").on("click", function () {
        $(this).toggleClass("selected");
        $(this).siblings(".ul_list").toggle();
    });
    $(".payway .title").on("click", function () {
        //$(this).toggleClass("selected");
        //$(this).siblings(".ul_list").toggle();
    });
    $(".coupons .title").on("click", function () {
        $(this).toggleClass("selected");
        $(this).siblings(".ul_list").toggle();
    });

    Selected3MPackage = JSON.parse(window.name);//JSON.parse(GetCookie("Selected3MPackage"));
    console.log(JSON.stringify(Selected3MPackage));

    $(".products .title .choose #products").html(Selected3MPackage.Items.length);
    $(".products .title .choose #services").html(Selected3MPackage.InstallServiceses.length)
    var pruducthtml = "";
    for (var item in Selected3MPackage.Items) {
        pruducthtml += '  <li>' +
                    '<div class="type">' + Selected3MPackage.Items[item].CatalogName + '</div>' +
                    '<div class="name"><p>' + Selected3MPackage.Items[item].DisplayName + '</p></div>' +
                '</li>';
    }

    for (var item in Selected3MPackage.InstallServiceses) {
        pruducthtml += '  <li>' +
                    ' <div class="type">服务</div>' +
                    '<div class="name"> ' + Selected3MPackage.InstallServiceses[item].DisplayName + ' </div>' +
                 '</li>';
    }

    $(".products .ul_list").html(pruducthtml);
    //价格
    $(".payinfo p").eq(0).find(".price").html("¥" + Selected3MPackage.Price);
    $(".payinfo p").eq(1).find(".price").html("¥" + GetQueryString("price"));

    GetFeightFor3M();

    ////优惠券
    //GetCouponsListFor3M('{\"' + Selected3MPackage.PackagePid + '\":\"1\"}', "4F79E798-502C-1891-AFE5-156B452BF4B2");
    //GetCouponsListFor3M('{\"' + Selected3MPackage.PackagePid + '\":\"1\"}', UserloginObject3M.UserId);


    $(".submitorder .subbtn").on("click", function () {
        if (allowSubmit) {
            allowSubmit = false;
            SubmitOrderFor3M();
        }

    })
});
var allowSubmit = true;
var UserloginObject3M = "";
var Selected3MPackage = "";

//进入iosApp
function ToIosFor3M(cmd, arg) {
    location.href = '#testapp#' + cmd + '#' + encodeURIComponent(arg);
}

function SubmitOrderFor3MToApp(serialNumbers, payAmount, subject, body, casgiersData, orderNumber) {

    if (userAgentInfo.indexOf("Android") >= 0) {
        var str = {
            'FunctionID': 'cn.TuHu.Activity.OrderSubmit.PayOrderConfirm',
            'SerialNumbers': serialNumbers,
            'PayAmount': payAmount,
            'subject': subject,
            'body': body
        };
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
        var iosPrams = {
            "casgiersData": casgiersData,
            "orderID": orderNumber
        }

        ToIosFor3M("customSegue#THOrderCashiersVC", JSON.stringify(iosPrams));
    }

}


function SubmitOrderFor3M() {

    if (!$(".userinfo .username").val()) {
        $(".userinfo .username").css("border", "2px solid red");
        allowSubmit = true;
        return false;
    }
    if (!$(".userinfo .tel").val()) {
        $(".userinfo .tel").css("border", "2px solid red");
        allowSubmit = true;
        return false;
    }

    var regex = /^1\d{10}$/;
    if (!regex.test($(".userinfo .tel").val())) {
        $(".userinfo .tel").css("border", "2px solid red");
        allowSubmit = true;
        return false;
    }
    var products = [];

    for (var item in Selected3MPackage.Items) {
        var product = { "ProductId": "", "VariantId": "", "Quantity": "" };
        if (Selected3MPackage.Items[item].CatalogName == "机油滤清器") {
            product.ProductId = Selected3MPackage.Items[item].ProductPid.split('|')[0];
            product.VariantId = Selected3MPackage.Items[item].ProductPid.split('|')[1];
            product.Quantity = Selected3MPackage.Items[item].Quantity;
            products.push(product);
        }
    }
    console.log(JSON.stringify(products));
    for (var item in Selected3MPackage.InstallServiceses) {
        var product = { "ProductId": "", "VariantId": "", "Quantity": "" };
        product.ProductId = Selected3MPackage.InstallServiceses[item].PID.split('|')[0];
        product.VariantId = Selected3MPackage.InstallServiceses[item].PID.split('|')[1];
        product.Quantity = 1;
        products.push(product);
    }

    console.log(JSON.stringify(products));
    var Package = { "ProductId": "", "VariantId": "", "Quantity": "" };
    Package.ProductId = Selected3MPackage.PackagePid.split('|')[0];
    Package.VariantId = Selected3MPackage.PackagePid.split('|')[1];
    Package.Quantity = 1;
    products.push(Package);
    console.log(JSON.stringify(products));

    var DefaultCar = JSON.parse(GetCookie("SelectedCarDataFor3M"));

    var jsonStr = {
        "BookType": 1,
        "Name": $(".userinfo .username").val(),
        "Cellphone": $(".userinfo .tel").val(),
        "UserId": UserloginObject3M.UserId,
        "ShopId": parseInt(GetQueryString("id")),
        "PayMothed": 1,
        "OrderChannel": "kH5",
        "ShippingMoney": FeightFor3MPrice,
        "BookDatetime": $("#carUseTime").val(),
        "PromotionCode": CouponsId,
        "OrderList": products,
        "DefaultCar": DefaultCar
    }
    console.log(JSON.stringify(jsonStr));

    $.ajax({
        method: "POST",
        url: apiDomain + "/Order/CreateOrder",
        data: { jsonStr: JSON.stringify(jsonStr) },
        dataType: "json"
    }).done(function (str) {
        allowSubmit = false;
        console.log(JSON.stringify(str));
        var serialNumbers = str.PayInfo.SerialNumbers,
            payAmount = str.PayInfo.Price,
            subject = str.PayInfo.Title,
            body = str.PayInfo.Body,
            orderNumber = str.PayInfo.OrderNO;

        //过滤(IOS(老))支付方式（银行卡）
        for (var i = 0; i < str.Cashier.length; i++) {
            if (str.Cashier[i].Payment_way_name != "支付宝" && str.Cashier[i].Payment_way_name != "微信") {
                delete str.Cashier[i];
            }
        }
        var casgiersData = str;
        SubmitOrderFor3MToApp(serialNumbers, payAmount, subject, body, casgiersData, orderNumber)

    }).fail(function (data) {
        //alert("错误");
    })

}

var FeightFor3MPrice = 0;
function GetFeightFor3M() {
    var products = new Array();
    var i = -1;
    for (var item in Selected3MPackage.Items) {
        var product = { "ProductID": "", "ProductNumber": "" };
        product.ProductID = Selected3MPackage.Items[item].ProductPid;
        product.ProductNumber = Selected3MPackage.Items[item].Quantity;
        i++
        products[i] = product;
    }
    console.log(JSON.stringify(products));
    $.ajax({
        method: "GET",
        url: "http://wx.tuhu.cn" + "/Order/GetFeightFor3M",
        data: { "products": JSON.stringify(products), "Province": UserloginObject3M.Province, "City": UserloginObject3M.City, "OrderType": "BaoYang", "isInstall": "true", "callBack": "0" },
        dataType: "jsonp"
    }).done(function (data) {
        console.log(data);
        if (data.Code == "1") {
            FeightFor3MPrice = data.DeliveryFee
            $(".payinfo p").eq(2).find(".price").html("¥" + FeightFor3MPrice);
            //优惠券 Selected3MPackage.InstallServiceses   

            var jsonStr = {};
            jsonStr[Selected3MPackage.PackagePid] = 1;
            for (var item in Selected3MPackage.InstallServiceses) {
                jsonStr[Selected3MPackage.InstallServiceses[item].PID] = 1;
            }
            GetCouponsListFor3M(JSON.stringify(jsonStr), UserloginObject3M.UserId);
            // GetCouponsListFor3M('{\"' + Selected3MPackage.PackagePid + '\":\"1\",\"' + Selected3MPackage.InstallServiceses[0].PID + '\":\"1\"}', UserloginObject3M.UserId);
        }
    }).fail(function (data) {
        //alert("错误");
    })
}
var apiDomain = "http://api.tuhu.cn";

function GetCouponsListFor3M(products, uerID) {

    $.ajax({
        method: "POST",
        url: "http://api.tuhu.cn" + "/User/SelectPromotion",
        async: false,
        data: { "Products": products, "UserID": uerID, "ActivityId": "", "OrderType": "BaoYang" },
        dataType: "jsonp"
    }).done(function (data) {
        var couponsModelList = JSON.parse(JSON.stringify(data));
        //alert(JSON.stringify(data));
        console.log(JSON.stringify(data))
        if (couponsModelList.Code == "1" && couponsModelList.PromotionCodeList.length > 0) {

            //获取优惠券集合（没有则不显示该模块）            
            var promotionList = couponsModelList.PromotionCodeList;

            if (promotionList.length > 0) {

                var templateHTML = "";
                $(promotionList).each(function (i) {
                    templateHTML += ' <li class="" yh="1" pkid="' + promotionList[i].PKID + '" price="' + promotionList[i].Discount + '">' +
                                        '<span class="red">¥' + promotionList[i].Discount + '</span>养车优惠券 ' +
                                    '</li>';
                });

                templateHTML += ' <li yh="0" pkid="" price="0" class="">不使用优惠券</li>';
                $(".coupons .ul_list").append(templateHTML);

                $(".coupons .ul_list li").click(function (e) {

                    if ($(this).attr("yh") == "0") {
                        $(".coupons .title").find(".choose").html("不使用优惠券").removeClass("active");
                    } else {
                        $(".coupons .title").find(".choose").html("¥" + $(this).attr("price")).removeClass("active");
                    }

                    $(".coupons .ul_list li").not(e).removeClass("selected");
                    $(this).toggleClass("selected");
                    $(".coupons").attr("pkid", $(this).attr("pkid"));
                    $(".coupons").attr("price", $(this).attr("price"));
                    $(".coupons .ul_list").toggle();
                    $(".payinfo p").eq(3).find(".price").html("-¥" + $(this).attr("price"));
                    CostingFor3M();
                    e.stopPropagation();
                })
                $(".coupons").show();
            }
            else {
                $(".coupons").hide();
            }
        } else {
            //$(".coupons .ul_list li").click(function (e) {
            //    if ($(this).attr("yh") == "0") {
            //        $(".coupons .title").find(".choose").html("不使用优惠券").removeClass("active");
            //    } else {
            //        $(".coupons .title").find(".choose").html($(this).attr("price") + "¥").removeClass("active");
            //    }

            //    $(".coupons .ul_list li").not(e).removeClass("selected");
            //    $(this).toggleClass("selected");
            //    $(".coupons").attr("pkid", $(this).attr("pkid"));
            //    $(".coupons").attr("price", $(this).attr("price"));
            //    $(".coupons .ul_list").toggle();
            //    $(".payinfo p").eq(3).find(".price").html("-¥" + $(this).attr("price"));
            //    CostingFor3M();
            //    e.stopPropagation();
            //})

            $(".coupons").hide();
        }

        CostingFor3M();

    }).fail(function (data) {
        //alert("错误");
    })
}

var CouponsId = "";
//计算实付价格
function CostingFor3M() {

    var currentPrice = $(".coupons").attr("price");
    CouponsId = $(".coupons").attr("pkid");
    if (currentPrice <= 0 || currentPrice == undefined || currentPrice == "undefined") {

        //实付金额
        $(".submitorder .price").html(Selected3MPackage.Price + parseFloat(GetQueryString("price")) + FeightFor3MPrice);
    }
    else {

        //实付金额
        var realPayMoney = Selected3MPackage.Price - currentPrice;

        $(".submitorder .price").html(realPayMoney + parseFloat(GetQueryString("price")) + FeightFor3MPrice);
    }
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


Date.prototype.FormatDate = function (fmt) { //fmt 时间格式 
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


function GetQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return decodeURIComponent(r[2]); return null;
}

//写cookie
function SetCookie(name, value) {
    var OneDayTimes = 60 * 60 * 24 * 1000;
    var exp = new Date();
    exp.setTime(exp.getTime() + OneDayTimes * 7);
    document.cookie = name + "=" + escape(value) + ";path=/;expires=" + exp.toGMTString();
}

//获取cookie
function GetCookie(name) {
    var arr = document.cookie.match(new RegExp("(^| )" + name + "=([^;]*)(;|$)"));
    if (arr != null) return unescape(arr[2]); return null;
}

