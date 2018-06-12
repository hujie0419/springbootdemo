$(function () {
    //选择产品
    $(".details ul").on("click", ".prdetail", function () {
        ClickCount++;
        if ($(this).attr("kv") == "ikv") {
            if ($(this).hasClass("selected")) {
                $(this).removeClass("selected");
            } else {
                $(".prdetail").each(function (index, data) {
                    if ($(this).attr("kv") == "ikv") {
                        $(this).removeClass("selected");
                    }
                });
                $(this).addClass("selected");
            }

        } else if ($(this).attr("kv") == "okv") {
            if ($(this).hasClass("selected")) {
                $(this).removeClass("selected");
            } else {
                $(".prdetail").each(function (index, data) {
                    if ($(this).attr("kv") == "okv") {
                        $(this).removeClass("selected");
                    }
                });
                $(this).addClass("selected");
            }
        }
        SelectedAllProduct = new Array();
        $(".prdetail").each(function (index, data) {
            if ($(this).hasClass("selected")) {
                SelectedAllProduct.push(JSON.parse($(this).find(".data").text()));
            }
        });
        TotalPrice = 0.00;
        for (var items in SelectedAllProduct) {
            TotalPrice += parseFloat(SelectedAllProduct[items].Price);
        }
        $(".settle .leftbtn .total").find("em").html("¥" + TotalPrice.toFixed(2));
    });

    //点击选择品牌
    $(".selectbrand").on("click", function (e) {
        e.preventDefault();
        setTimeout(function () {
            $(".searchPage").show().addClass("slidein").removeClass("slideout");
        }, 350);
    })
    //关闭
    $(".searchPage").on("click", ".search .close", function () {
        setTimeout(function () {
            $(".searchPage").removeClass("slidein").addClass("slideout");
        }, 350);
    });
    //筛选确定
    $(".searchPage").on("click", ".search .queding", function () {
        var brandArr = new Array();
        $(".listItem a").each(function (index, data) {
            if ($(this).hasClass("selected")) {
                brandArr.push($(data).find("em").text())
            }
        })
        var SearchProduct = new Array();
        if (brandArr.length > 0) {
            if (brandArr.indexOf("全部") >= 0) {
                SearchProduct = ProductListAll;
            } else {
                for (var item in ProductListAll) {
                    if (brandArr.indexOf(ProductListAll[item].Brand) >= 0) {
                        SearchProduct.push(ProductListAll[item])
                    }
                }
            }
        } else {
            SearchProduct = ProductListAll;
        }

        AirConditionFilter.CreateHtml(SearchProduct);
        $(".searchResult a").removeClass("selected");
        setTimeout(function () {
            $(".searchPage").removeClass("slidein").addClass("slideout");
        }, 350);

    });
    //选中品牌
    $(".searchPage").on("click", ".searchResult a", function () {
        if ($(this).hasClass("totalbrand")) {
            $(this).toggleClass("selected").siblings().removeClass("selected");
        } else {
            $(".searchResult a.totalbrand").removeClass("selected");
            $(this).toggleClass("selected");
        }
    });

    //结算
    $(".settle .rightbtn").on("click", function () {
        if (ClickCount == 0 && SelectedAllProduct.length <= 0) {
            SelectedAllProduct.push(SelectedProduct);
            TotalPrice = SelectedProduct.Price;
        } else if (ClickCount != 0 && SelectedAllProduct.length <= 0) {
            return false;
        }
        console.log(JSON.stringify(SelectedAllProduct));
        if (userAgentInfo.indexOf("Android") >= 0) {

            if (AndroidVersion.localeCompare("4.9.0") >= 0) {
              
                var goodArray = new Array();
                var servicesArray = new Array();
                var typeServiceArray = new Array();
                for (var item in SelectedAllProduct) {
                    var goodsModel = {
                        "orderTitle": "",
                        "orderNum": 0,
                        "orderPrice": "",
                        "BeiJingPrice": "",
                        "ProductID": "",
                        "VariantID": "",
                        "produteImg": "",
                        "type": "",
                        "CP_UnitP": "",
                        "ActivityId": ""
                    };
                    var typeService = {
                        "Item": "",
                        "count": ""
                    };

                    var typeServiceModel = {
                        "BaoYangType": "",
                        "Products": ""
                    }
                    goodsModel.orderTitle = SelectedAllProduct[item].DisplayName;
                    goodsModel.orderNum = SelectedAllProduct[item].Quantity;
                    goodsModel.orderPrice = SelectedAllProduct[item].Price;
                    goodsModel.BeiJingPrice = "";
                    goodsModel.ProductID = SelectedAllProduct[item].ProductID;
                    goodsModel.VariantID = SelectedAllProduct[item].VariantID;
                    goodsModel.produteImg = SelectedAllProduct[item].Image;
                    goodsModel.CP_UnitP = SelectedAllProduct[item].CP_UnitP;
                    goodsModel.ActivityId = "";

                    typeService.Item = SelectedAllProduct[item].ProductID + "|" + SelectedAllProduct[item].VariantID;
                    typeService.count = SelectedAllProduct[item].Quantity;
                    goodArray.push(goodsModel);
                    servicesArray.push(typeService);
                    typeServiceModel.BaoYangType = SelectedAllProduct[item].Type;
                    typeServiceModel.Products = servicesArray;
                    typeServiceArray.push(typeServiceModel);
                }
             
                var androidParameters = {
                    'typeService': JSON.stringify(typeServiceArray),
                    'FunctionID': 'cn.TuHu.Activity.OrderSubmit.OrderConfirmUI',
                    'Goods': goodArray,
                    'quanType': 2,
                    'orderType': 2,
                    'Car': DefaultCar
                };
           
                window.WebViewJavascriptBridge.callHandler(
                                  'toOrder'
                                  , encodeURIComponent(encodeURIComponent(JSON.stringify(androidParameters))));
            } else {

                var goodArray = new Array();
                var servicesArray = new Array();
                for (var item in SelectedAllProduct) {
                    var goodsModel = {
                        "orderTitle": "",
                        "orderNum": 0,
                        "orderPrice": "",
                        "BeiJingPrice": "",
                        "ProductID": "",
                        "VariantID": "",
                        "produteImg": "",
                        "type": "",
                        "CP_UnitP": "",
                        "ActivityId": ""
                    };
                    var typeService = {
                        "Type": "",
                        "Count": ""
                    };
                    goodsModel.orderTitle = SelectedAllProduct[item].DisplayName;
                    goodsModel.orderNum = SelectedAllProduct[item].Quantity;
                    goodsModel.orderPrice = SelectedAllProduct[item].Price;
                    goodsModel.BeiJingPrice = "";
                    goodsModel.ProductID = SelectedAllProduct[item].ProductID;
                    goodsModel.VariantID = SelectedAllProduct[item].VariantID;
                    goodsModel.produteImg = SelectedAllProduct[item].Image;
                    goodsModel.CP_UnitP = SelectedAllProduct[item].CP_UnitP;
                    goodsModel.ActivityId = "";

                    typeService.Type = SelectedAllProduct[item].Type;
                    typeService.Count = SelectedAllProduct[item].Quantity;
                    goodArray.push(goodsModel);
                    servicesArray.push(typeService);
                }
                var androidParameters = {
                    'FunctionID': 'cn.TuHu.Activity.OrderSubmit.OrderConfirmUI',
                    'Goods': goodArray,
                    'typeService': servicesArray,
                    'quanType': 2,
                    'orderType': 2,
                    'Car': DefaultCar
                };

                window.WebViewJavascriptBridge.callHandler(
                                  'toOrder'
                                  , encodeURIComponent(encodeURIComponent(JSON.stringify(androidParameters))));

            }
        }
        else {
            if (IosVersion.localeCompare("IOS3.3.5") >= 0) {

                var goodsArray = new Array();
                var maintainArray = new Array();
                var mainArray = new Array();
                for (var item in SelectedAllProduct) {
                    var goodsModel = {
                        "productID": "",
                        "variantID": "",
                        "name": "",
                        "image": "",
                        "price": "",
                        "count": 0
                    };


                    var maintainModel = {
                        "Item": "",
                        "Count": 0
                    };

                    var main = {
                        "BaoYangType": "",
                        "Products": ""
                    }
                    goodsModel.productID = SelectedAllProduct[item].ProductID;
                    goodsModel.variantID = SelectedAllProduct[item].VariantID;
                    goodsModel.name = SelectedAllProduct[item].DisplayName;
                    goodsModel.image = SelectedAllProduct[item].Image;
                    goodsModel.price = SelectedAllProduct[item].Price;
                    goodsModel.count = SelectedAllProduct[item].Quantity;

                    maintainModel.Item = SelectedAllProduct[item].ProductID + "|" + SelectedAllProduct[item].VariantID;
                    maintainModel.Count = SelectedAllProduct[item].Quantity;

                    goodsArray.push(goodsModel);
                    maintainArray.push(maintainModel);
                    main.BaoYangType = SelectedAllProduct[item].Type;
                    main.Products = maintainArray;
                    mainArray.push(main);
                }
                var iosParameters = {
                    "goods<THGoods>": goodsArray,
                    "totalPrice": TotalPrice,
                    "orderType": 2,
                    "orderAddressType": 1,
                    "maintainTypes": mainArray,
                    "activityId": ""
                };

                AirConditionFilter.iosend("tuhuaction://segue#THCreatOrderVC#", JSON.stringify(iosParameters));
            } else {
                var goodsArray = new Array();
                var maintainArray = new Array();
                for (var item in SelectedAllProduct) {
                    var goodsModel = {
                        "productID": "",
                        "variantID": "",
                        "name": "",
                        "image": "",
                        "price": "",
                        "count": 0
                    };
                    var maintainModel = {
                        "type": "",
                        "count": 0
                    };
                    goodsModel.productID = SelectedAllProduct[item].ProductID;
                    goodsModel.variantID = SelectedAllProduct[item].VariantID;
                    goodsModel.name = SelectedAllProduct[item].DisplayName;
                    goodsModel.image = SelectedAllProduct[item].Image;
                    goodsModel.price = SelectedAllProduct[item].Price;
                    goodsModel.count = SelectedAllProduct[item].Quantity;
                    maintainModel.type = SelectedAllProduct[item].Type;
                    maintainModel.count = SelectedAllProduct[item].Quantity;

                    goodsArray.push(goodsModel);
                    maintainArray.push(maintainModel);

                }
                var iosParameters = {
                    "goods<THGoods>": goodsArray,
                    "totalPrice": TotalPrice,
                    "orderType": 2,
                    "orderAddressType": 1,
                    "maintainTypes": maintainArray,
                    "activityId": ""
                };
                console.log(JSON.stringify(iosParameters))
                AirConditionFilter.iosend("tuhuaction://segue#THCreatOrderVC#", JSON.stringify(iosParameters));
            }
        }
    });

    //AirConditionFilter.SelectCarObject("{acc71c21-28cd-5ead-da9d-d94af0d69f44}");
    AirConditionFilter.Login();
});

var AirConditionFilter = {
    "GetAirConditionFilterData": function (vehicleId, paiLiang, nian, tid, brand, liyangId, salesName, vehicle) {
        $.ajax({
            method: "GET",
            url: DomainBy + "api/ChangeMaintenanceProduct.html",
            data: {
                "type": "ikv",
                "PaiLiang": paiLiang,
                "Nian": nian,
                "TID": tid,
                "LiYangID": liyangId,
                "VehicleID": vehicleId
            },
            dataType: "jsonp"
        }).done(function (dataikv) {
            if (dataikv.Code == "1") {
                ProductListAll = dataikv.ProductList;
                $.ajax({
                    method: "GET",
                    url: DomainBy + "api/ChangeMaintenanceProduct.html",
                    data: {
                        "type": "okv",
                        "PaiLiang": paiLiang,
                        "Nian": nian,
                        "TID": tid,
                        "LiYangID": liyangId,
                        "VehicleID": vehicleId
                    },
                    dataType: "jsonp"
                }).done(function (dataokv) {
                    if (dataokv.Code == "1") {
                        if (dataokv.ProductList.length > 0) {
                            for (var item in dataokv.ProductList) {
                                ProductListAll.push(dataokv.ProductList[item])
                            }
                        }
                    }

                    if (ProductListAll.length > 0) {

                        var brandArr = new Array();
                        var brandName = new Array();
                        for (var item in ProductListAll) {
                            var brandJson = { "Brand": "", "BrandImage": "" };
                            if (brandName.indexOf(ProductListAll[item].Brand) < 0) {
                                brandName.push(ProductListAll[item].Brand);
                                brandJson.Brand = ProductListAll[item].Brand;
                                brandJson.BrandImage = ProductListAll[item].BrandImage;
                                brandArr.push(brandJson);
                            }
                        }

                        var brandHtml = '<a class="totalbrand selected">' +
                                         '<span></span>' +
                                         '<em>全部</em>' +
                                        '</a>';

                        for (var item in brandArr) {
                            brandHtml += ' <a class="">' +
                                         '<img src="' + brandArr[item].BrandImage + '" />' +
                                         '<em>' + brandArr[item].Brand + '</em>' +
                                         '</a>  ';
                        }
                        $(".listItem").html(brandHtml);

                        AirConditionFilter.CreateHtml(ProductListAll);
                    } else {
                        AirConditionFilter.NoneProduct();
                    }
                })
            } else {
                AirConditionFilter.NoneProduct();
            }
        }).fail(function (data) {
            //alert("错误");
        })
    },
    "Login": function () {
        if (userAgentInfo.indexOf("Android") >= 0) {
            window.WebViewJavascriptBridge.callHandler(
                'actityBridge'
                , {
                    'param': ""
                }
                , function (responseData) {
                    if (responseData != null) {
                        AirConditionFilter.CallBackAppLoginResponse(responseData);
                    }
                });
        }
        else {
            AirConditionFilter.iosend("tuhuaction://getuserinfo#1#", "AirConditionFilter.CallBackAppLoginResponse");
        }
    },
    "CallBackAppLoginResponse": function (responseData) {
        var da = null;
        if (userAgentInfo.indexOf("Android") >= 0) {
            da = JSON.parse(responseData);
        }
        else {
            da = responseData;
        }

        UserId = da.userid;
        Phone = da.phone;
        AirConditionFilter.SelectCarObject(da.userid);
    },
    "SelectCarObject": function (userid) {
        $.ajax({
            method: 'GET',
            url: DomainItem + "CarHistory/SelectCarObject.html",
            data: {
                "userID": userid
            },
            dataType: 'jsonp',
            async: false
        }).done(function (data) {
            if (data.Code == "1" && data.CarHistory.length > 0) {
                for (var obj in data.CarHistory) {
                    if (data.CarHistory[obj].IsDefaultCar == "1") {
                        DefaultCar = data.CarHistory[obj];
                        CarId = data.CarHistory[obj].CarID;
                        LIYANGID = data.CarHistory[obj].LIYANGID;
                        Nian = data.CarHistory[obj].Nian;
                        Pailiang = data.CarHistory[obj].Pailiang;
                        TID = data.CarHistory[obj].TID;
                        ProductID = data.CarHistory[obj].ProductID;
                        Vehicle = data.CarHistory[obj].Vehicle;
                        Brand = data.CarHistory[obj].Brand;
                        VehicleLogin = 'http://image.tuhu.cn' + data.CarHistory[obj].VehicleLogin;
                        SalesName = data.CarHistory[obj].SalesName;

                        var plNian = Pailiang + " " + Nian;
                        $("#plNian").html(plNian);
                        $(".car-info img").attr("src", VehicleLogin)
                        $(".car-info .other .type").html(plNian);
                        $(".car-info .other .name").html(Vehicle);

                        if (ProductID != "" && Pailiang == "") {
                            //调pinglian
                            AirConditionFilter.GetPailiang(ProductID);
                        } else {

                            if (TID == "null" || TID == null || TID == "") {

                                AirConditionFilter.GetTID(CarId, ProductID, Pailiang, Nian);
                            } else {

                                AirConditionFilter.GetAirConditionFilterData(ProductID, Pailiang, Nian, TID, Brand, LIYANGID, SalesName, Vehicle);
                            }

                            break;
                        }
                    } else {
                        DefaultCar = data.CarHistory[0];
                        CarId = data.CarHistory[0].CarID;
                        LIYANGID = data.CarHistory[0].LIYANGID;
                        Nian = data.CarHistory[0].Nian;
                        Pailiang = data.CarHistory[0].Pailiang;
                        TID = data.CarHistory[0].TID;
                        ProductID = data.CarHistory[0].ProductID;
                        Vehicle = data.CarHistory[0].Vehicle;
                        Brand = data.CarHistory[0].Brand;
                        VehicleLogin = 'http://image.tuhu.cn' + data.CarHistory[0].VehicleLogin;
                        SalesName = data.CarHistory[0].SalesName;

                        var plNian = Pailiang + " " + Nian;
                        $("#plNian").html(plNian);
                        $(".car-info img").attr("src", VehicleLogin)
                        $(".car-info .other .type").html(plNian);
                        $(".car-info .other .name").html(Vehicle);
                        if (ProductID != "" && Pailiang == "") {
                            //调pinglian
                            AirConditionFilter.GetPailiang(ProductID);
                        } else {

                            if (TID == "null" || TID == null || TID == "") {

                                AirConditionFilter.GetTID(CarId, ProductID, Pailiang, Nian);
                            } else {

                                AirConditionFilter.GetAirConditionFilterData(ProductID, Pailiang, Nian, TID, Brand, LIYANGID, SalesName, Vehicle);
                            }
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
                } else {
                    AirConditionFilter.iosend("tuhuaction://getcar#5#0#", "AirConditionFilter.carCallback");
                }
            }
        }).fail(function () {

        })

    },
    "carCallback": function () {
        window.location.reload(true);
    },
    "iosend": function (act, arg) {
        window.location.href = act + encodeURIComponent(arg);
    },
    "GetTID": function (carId, vehicleID, paiLiang, nian) {
        $.ajax({
            method: 'GET',
            url: DomainItem + "Car/SelectVehicle",
            data: {
                "VehicleID": vehicleID, "PaiLiang": paiLiang, "Nian": nian
            },
            dataType: 'jsonp',
            async: false
        }).done(function (data) {
            if (data.Code == "1") { //需要选择五级数据 data 五级车型的数据  
                AirConditionFilter.NoneProduct();
            } else if (data.Code == "0") {//无需选择五级数据 ，直接读取五级默认数据，
                AirConditionFilter.GetWuJi(vehicleID, paiLiang, nian);
            }
        }).fail(function (data) {
        })
    },
    "UpdateDefultCar": function (carId, pid, paiLiang, nian, tid) {
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
    },
    "GetWuJi": function (vehicleID, paiLiang, nian) {//五级数据
        $.ajax({
            method: 'GET',
            url: DomainItem + "Car/GetVehicleSalesName",
            data: {
                "VehicleID": vehicleID, "PaiLiang": paiLiang, "Nian": nian
            },
            dataType: 'jsonp',
            async: false
        }).done(function (data) {
            if (data.Code == "1") {
                TID = data.SalesName[0].TID;
                if (TID == null || TID == "null" || TID == "") {
                    AirConditionFilter.NoneProduct();
                    return false;
                }
                else {
                    AirConditionFilter.GetAirConditionFilterData(ProductID, Pailiang, Nian, TID, Brand, LIYANGID, SalesName, Vehicle);
                }
            } else {
                NoneProduct();
            }
        })
    },
    "GetPailiang": function (vehicleID) {//两级车型，继续添加车型，
        $.ajax({
            url: DomainItem + "Car/SelectVehicle",
            type: 'GET',
            data: {
                "VehicleID": vehicleID, "PaiLiang": "", "Nian": ""
            },
            dataType: 'jsonp',
            async: false
        }).done(function (data) {
            if (data.Code == "1") {
                if (userAgentInfo.indexOf("Android") >= 0) {
                    window.WebViewJavascriptBridge.callHandler(
                        'setUserCarInfo'
                        , function () {
                        });

                } else {
                    AirConditionFilter.iosend("tuhuaction://getcar#5#0#", "AirConditionFilter.carCallback");
                }

            } else if (data.Code == "0") {//无服务
                AirConditionFilter.NoneProduct();
            }
            return false;
        })
    },
    "UniqueCategory": function (arr) {//取消重复数据
        var result = [], hash = {};
        for (var i = 0, elem; (elem = arr[i]) != null; i++) {
            if (!hash[elem]) {
                result.push(elem);
                hash[elem] = true;
            }
        }
        return result;
    },
    "GetQueryString": function (name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
        var r = window.location.search.substr(1).match(reg);
        if (r != null) return unescape(r[2]); return null;
    },
    "SetCookie": function (name, value) {
        var OneDayTimes = 60 * 60 * 24 * 1000;
        var exp = new Date();
        exp.setTime(exp.getTime() + OneDayTimes * 1);
        document.cookie = name + "=" + escape(value) + ";path=/;expires=" + exp.toGMTString();
    },
    "GetCookie": function (name) {
        var arr = document.cookie.match(new RegExp("(^| )" + name + "=([^;]*)(;|$)"));
        if (arr != null) return unescape(arr[2]); return null;
    },
    "CreateHtml": function (data) {
        var productHtml = "";
        $(".details ul").html(productHtml);
        for (var item in data) {
            if (data[item].HasStock) {
                var kvname = "";
                if (data[item].Type == "ikv") {
                    kvname = ' <em>内置</em>';
                } else if (data[item].Type == "okv") {
                    kvname = ' <em>外置</em>';
                }
                productHtml += '<li>' +
                               '<div class="prdetail" kv="' + data[item].Type + '">' +
                                   '<p class="data" style="display:none">' + JSON.stringify(data[item]) + '</p>' +
                                   '<div class="product clearfix">' +
                                       '<span class="list-img">' +
                                           '<img src="' + data[item].Image + '">' +
                                       '</span>' +
                                       '<h3>' +
                                           '<span class="name">' + data[item].DisplayName + kvname + '</span>' +
                                       '</h3>' +
                                       '<div class="price">¥' + data[item].Price +
                                       '</div>' +
                                   '</div>' +
                                   '<span class="xuanzhong"></span>' +
                              '</div>' +
                           '</li>';
            }
        }
        $(".details ul").html(productHtml);
        if (productHtml) {
            $(".details ul li .prdetail").eq(0).addClass("selected");
            SelectedProduct = JSON.parse($(".details ul li .prdetail").eq(0).find(".data").text());
            $(".settle .leftbtn .total").find("em").html("¥" + SelectedProduct.Price);
        }
    },
    "NoneProduct": function () {
        $(".details").html('<img src="images/no-product.png"><p>暂无适配产品</p>');
        $(".details").css({
            "text-align": "center",
            "margin-top": "25%",
            "display": "block",
            "color": "red"
        });
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

var TID = "",
    VehicleLogin = "",
    Vehicle = "",
    UserId = "",
    Phone = "",
    CarId = "",
    LIYANGID = "",
    Nian = "",
    Pailiang = "",
    ProductID = "",
    Brand = "",
    SalesName = "",
    SelectedProduct = "",
    SelectedAllProduct = new Array(),
    TotalPrice = 0.00,
    ClickCount = 0,
    ProductListAll = "",
    DefaultCar = "",
    DomainItem = "http://item.tuhu.cn/",
    DomainBy = "http://by.tuhu.cn/";


var IosVersion = "";
var AndroidVersion = "";
//IOS版本判断
function VersionForIos(version) {
    IosVersion = version;
}

//安卓版本判断
function VersionForAndroid(version) { 
    AndroidVersion = version;
}


