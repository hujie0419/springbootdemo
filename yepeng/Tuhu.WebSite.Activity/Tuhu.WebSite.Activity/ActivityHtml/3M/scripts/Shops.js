

$(function () {
    //alert(JSON.stringify(UserloginObject3M));
    //$(".shop-select .select .value span").eq(0).html(UserloginObject3M.City)
    for (var item in Selected3MPackage.InstallServiceses) {
        serviceList += Selected3MPackage.InstallServiceses[item].PID + ";";
    }

    //下拉选择效果
    $(".select").on("tap", ".value", function (e) {
        $(".select").not($(this).parents(".select")).removeClass("active");
        $(this).parents(".select").toggleClass("active");
        e.stopPropagation();
    }).on("tap", ".list li", function (e) {
        var str = $(this).text();
        $(this).parents(".list").prev().children("span").text(str);
        $(".select").removeClass("active");
        order = $(this).attr("data-type");
        console.log(order);
        pageIndex = 1;
        dataEnd = 0;
        $(".shop-list").html("");
        //$(".shop-list").css({ "transition-timing-function": "cubic-bezier(0.1, 0.57, 0.1, 1)", "transition-duration": "0ms", "transform": "translate(0px, 0px) translateZ(0px)" })
        GetShopsFor3M(Province, City, order, serviceList, UserloginObject3M.latitude, UserloginObject3M.longitude, "", pageIndex);
        e.stopPropagation();
    });

    //城市选择
    $("#select_city").on("tap", ".province", function (e) {
        var $this = $(this);
        !$this.hasClass("active") ? $(this).addClass("active").next("ul").show() : $(this).removeClass("active").next("ul").hide();
        e.stopPropagation();
    });


    //筛选店铺 by类型，b快修店|a4S店|c修理厂
    $("#filter").on("tap", ".list a", function () {
        $(this).hasClass("active") ? $(this).removeClass("active") : $(this).addClass("active");
        if ($(this).hasClass("active") && $(this).text() != "完成") {
            $(this).attr("data-type", $(this).text())
        }
    }).on("tap", ".btn a", function () {
        shopBusinessType = "";
        $("#filter a").each(function (index) {
            var dataType = $(this).attr("data-type")
            if (dataType) {
                if (dataType == "快修店") {
                    shopBusinessType += "b快修店" + "|";
                } else if (dataType == "4S店") {
                    shopBusinessType += "a4S店" + "|";
                } else if (dataType == "维修厂") {
                    shopBusinessType += "c修理厂" + "|";
                }
                $(this).attr("data-type", "");
            }
        })
        order = "";
        $("#filter").removeClass("active");
        $("#filter a").removeClass("active");
        console.log(shopBusinessType.RightSubLast());
        dataEnd = 0;
        pageIndex = 1;
        $(".shop-list").html("");
        //$(".shop-list").css({ "transition-timing-function": "cubic-bezier(0.1, 0.57, 0.1, 1)", "transition-duration": "0ms", "transform": "translate(0px, 0px) translateZ(0px)" })
        GetShopsFor3M(Province, City, "", serviceList, UserloginObject3M.latitude, UserloginObject3M.longitude, shopBusinessType.RightSubLast(), pageIndex);

    });

    $(".location p").html("当前位置：" + Province + City)

    console.log(JSON.stringify(Selected3MPackage));

    GetAreaFor3M(serviceList);

    pageIndex = 1;
    GetShopsFor3M(Province, City, "", serviceList, UserloginObject3M.latitude, UserloginObject3M.longitude, "b快修店|a4S店|c修理厂", pageIndex);

    $(".shop-list").on("tap", ".item", function () {
        var dataShop = $(this).find(".data-type").text();
        var price = $(this).find(".price").attr("data-price");
        var km = $(this).find(".km").text();
        var id = $(this).find(".price").attr("id");
        var name = $(this).find(".price").attr("name");
        //console.log(dataShop);
        SetCookie("SelectedShopBy3M", dataShop);
        console.log(JSON.parse(GetCookie("SelectedShopBy3M")));
        window.location.href = "/ActivityHtml/3M/SubmitOrder.html?km=" + km + "&name=" + name + "&price=" + price + "&id=" + id;
        //window.location.href = "/ActivityHtml/3M/product.html?v=6&price=" + price + "&km=" + km;
    })

    ScrollBind.Init();
    //var myScroll = new IScroll('#wrapper', {
    //    mouseWheel: true
    //});
    //myScroll.on('scrollEnd', function () {
    //    console.log("加载更多！");
    //    var index = parseInt(countSize) / 6;
    //    if (index < 1) {
    //        index = 1
    //    }
    //    if (dataEnd == 0) {
    //        if (myScroll.y === myScroll.maxScrollY) {//滚动到底部且有数据加载数据            
    //            ++pageIndex;
    //            GetShopsFor3M(Province, City, order, serviceList, UserloginObject3M.latitude, UserloginObject3M.longitude, shopBusinessType.RightSubLast(), pageIndex);
    //            if (pageIndex - 1 <= index) {
    //                myScroll.refresh();
    //            }
    //        }
    //    }
    //});

})

var pageIndex = 1;
var ScrollBind = {
    Init: function () {
        var nScrollHight = 0; //滚动距离总长(注意不是滚动条的长度)
        var nScrollTop = 0; //滚动到的当前位置
        var $shopsScroll = $(".shop-list");
        var nDivHight = $shopsScroll.height();
        //滚动事件
        $(".shop-list").on("scroll", function () {

            var totalPage = parseInt(countSize) / 10;
            if (totalPage < 1) {
                totalPage = 1
            }

            nDivHight = document.documentElement.scrollTop == 0 ? document.body.clientHeight : document.documentElement.clientHeight;
            nScrollTop = document.documentElement.scrollTop == 0 ? document.body.scrollTop : document.documentElement.scrollTop;
            nScrollHight = document.documentElement.scrollTop == 0 ? document.body.scrollHeight : document.documentElement.scrollHeight;
            if (nScrollTop + nDivHight + 100 >= nScrollHight) {
                if (pageIndex >= totalPage) {
                    return false;
                } else if (pageIndex < totalPage) {
                    ++pageIndex;
                    GetShopsFor3M(Province, City, order, serviceList, UserloginObject3M.latitude, UserloginObject3M.longitude, shopBusinessType.RightSubLast(), pageIndex);

                }
            }
        });
    }
}

String.prototype.RightSubLast = function () {
    return this.substring(0, this.length - 1);
};

String.prototype.LeftSubFrist = function () {
    return this.substring(0, 1);
};

var shopBusinessType = "";
var order = "";
var ProductID = "";
var VariantID = "";
var serviceList = "";
var Selected3MPackage = JSON.parse(window.name);// JSON.parse(GetCookie("Selected3MPackage"));
var UserloginObject3M = JSON.parse(GetCookie("UserloginObject3M"));
var SelectedCarDataFor3M = JSON.parse(GetCookie("SelectedCarDataFor3M"));

function GetAreaFor3M(serviceList) {
    $.ajax({
        method: "GET",
        url: "http://api.tuhu.cn/shops/GetMaintenanceArea",
        data: {
            "serviceList": serviceList
        },
        dataType: "jsonp"
    }).done(function (data) {
        // console.log(JSON.stringify(data.Area));
        for (var item in data.Area) {
            if (data.Area[item].Belong != "全部地区") {
                if (data.Area[item].Region == data.Area[item].Belong) {
                    var parent = $("<li  data-parentName=" + data.Area[item].Belong + " class='province'><div  class='province'>" + data.Area[item].Belong + "</div><ul></ul></li>")
                    $(".list-multilevel").append(parent);
                }
                var sb = "<li onclick = 'CityClick(this)'  data-region=" + data.Area[item].Region + ">" + data.Area[item].Region + "</li>";
                $(".province[data-parentName='" + data.Area[item].Belong + "'] ul").append(sb);
            }
        }

    }).fail(function (data) {
        // alert("错误");
    })
}
var Province = UserloginObject3M.Province;
var City = UserloginObject3M.City;
function CityClick(e) {
    dataEnd = 0;
    console.log($(e).parent().find("li").eq(0).attr("data-region"));
    console.log($(e).attr("data-region"));

    Province = $(e).parent().find("li").eq(0).attr("data-region");
    City = $(e).attr("data-region");
    $(".location p").html("当前位置：" + Province + City)
    $("#select_city").removeClass("active");
    console.log(Province);
    console.log(City);
    pageindex = 1;
    $(".shop-list").html("");
    GetShopsFor3M(Province, City, "", serviceList, UserloginObject3M.latitude, UserloginObject3M.longitude, "", pageindex);
}

//门店距离
function GetShopDistance(lat1, lng1, lat2, lng2) {//计算距离，参数分别为第一点的纬度，经度；第二点的纬度，经度
    var radLat1 = Rad(lat1);
    var radLat2 = Rad(lat2);
    var a = radLat1 - radLat2;
    var b = Rad(lng1) - Rad(lng2);
    var s = 2 * Math.asin(Math.sqrt(Math.pow(Math.sin(a / 2), 2) +
        Math.cos(radLat1) * Math.cos(radLat2) * Math.pow(Math.sin(b / 2), 2)));
    s = s * 6378.137; // EARTH_RADIUS;
    s = Math.round(s * 10000) / 10000; //输出为公里
    s = s.toFixed(2);
    return s;
}

function Rad(d) {
    return d * Math.PI / 180.0; //经纬度转换成三角函数中度分表形式。
}

//门店安装服务费
function GetShopInstallPriceFor3M(vid, pailiang, nian, shopIds) {
    //var servicePIds = ["FU-BY-XBY|27", "FU-TU-Fadongji|", "FU-BY-BYQX|1", "FU-BY-BYQX|3"];
    var servicePIds = [];
    for (var item in Selected3MPackage.InstallServiceses) {
        servicePIds.push(Selected3MPackage.InstallServiceses[item].PID)
    }
    //var shopIds = [];
    //$(".item .price").each(function (index) {
    //    var shopId = $(this).attr("id");
    //    shopIds.push(shopId);
    //})
    console.log(vid);
    console.log(pailiang);
    console.log(nian);
    console.log(shopIds);
    console.log(servicePIds);

    $.ajax({
        method: "GET",
        url: "http://by.tuhu.cn/home/GetBaoYangInstallServices.html",
        data: {
            "vid": vid,
            "paiLiang": pailiang,
            "nian": nian,
            "shopIds": JSON.stringify(shopIds),
            "servicePIds": JSON.stringify(servicePIds)
        },
        dataType: "jsonp"
    }).done(function (data) {
        $.each(data, function (index, value) {
            $("#" + index).html("¥" + value);
            $("#" + index).attr("data-price", value);
            if (!value) {
                $("#" + index).parents(".item").hide();
            }
        })

    }).fail(function (data) {
        //alert("错误");
    })

}
var dataEnd = 0;
var countSize = 0;
//门店列表
function GetShopsFor3M(province, city, order, serviceList, latBegin, lngBegin, shopBusinessType, pageindex) {

    $.ajax({
        method: "GET",
        url: "http://api.tuhu.cn/shops/GetMaintenanceShops",
        data: {
            "Province": province,
            "City": city,
            "Sort": order,//Install,TuhuLevel,Skill,HuShi,Grade
            "serviceList": serviceList,
            "LatBegin": latBegin,
            "LngBegin": lngBegin,
            "ShopBusinessType": shopBusinessType,// "b快修店|a4S店|c修理厂",
            "PageIndex": pageindex,
            "PageSize": 10
        },
        dataType: "jsonp"
    }).done(function (data) {
        countSize = data.Count;
        if (data.Code == "1") {
            console.log(JSON.stringify(data.ShopList[0]));
            if (data.ShopList.length == 0) {
                if (dataEnd == 0) {
                    $(".shop-list").append('<div class="item clearfix" id="end" style="text-align: center;color:red;display:block;">没有数据了...</div>');
                }
                ++dataEnd;
            }
            else {
                var shopIds = [];
                for (var item in data.ShopList) {
                    shopIds.push(data.ShopList[item].PKID);
                    var shopDistance = "";
                    if (UserloginObject3M.latitude) {//服务器返回的经度纬度 （反）

                        shopDistance = GetShopDistance(data.ShopList[item].LngBegin, data.ShopList[item].LatBegin, UserloginObject3M.latitude, UserloginObject3M.longitude) + "km";
                    }

                    var ShopClassificationClass = "";
                    if (data.ShopList[item].ShopClassification === "快修店") {
                        ShopClassificationClass = "green";
                    } else if (data.ShopList[item].ShopClassification === "维修厂") {
                        ShopClassificationClass = "xiu";
                    }
                    else if (data.ShopList[item].ShopClassification === "4S店") {
                        ShopClassificationClass = "blue";
                    }
                    var posHtml = "";
                    if (data.ShopList[item].POS.indexOf("支付宝") > -1) {
                        posHtml += '<span class="i-alpay">支付宝</span>';
                    }
                    if (data.ShopList[item].POS.indexOf("微信") > -1) {
                        posHtml += '<span class="i-wx">微信</span>';
                    }
                    if (data.ShopList[item].POS.indexOf("刷卡") > -1) {
                        posHtml += '<span class="i-bank">网银</span>';
                    }
                    if (data.ShopList[item].POS.indexOf("现金") > -1) {
                        posHtml += '<span class="i-cash">现金</span>';
                    }

                    var hzxHtml = "";

                    if ((data.ShopList[item].ShopType & 8) == 8) {//直
                        hzxHtml += '<span class="i-z">直</span>';
                    }
                    if ((data.ShopList[item].ShopType & 16) == 16) {//虎
                        hzxHtml += '<span class="i-hu">虎</span>';
                    }
                    if ((data.ShopList[item].ShopType & 128) == 128) {//星
                        hzxHtml += '<span class="i-star">星</span>';
                    }
                    var htmlShops = ' <div class="item clearfix">' +
                        '<div class="data-type" style="display:none;">' + JSON.stringify(data.ShopList[item]) + '</div>' +
                    '<div class="img">' +
                        '<img src="' + data.ShopList[item].ShopImg + '" alt="" width="90" height="70" />' +
                        '<p class="label ' + ShopClassificationClass + '">' + data.ShopList[item].ShopClassification + '</p>' +
                    '</div>' +
                    '<div class="filed clearfix">' +
                        '<p class="name">' + data.ShopList[item].CarParName + '</p>' +
                        '<span class="price" id="' + data.ShopList[item].PKID + '" name="' + data.ShopList[item].CarParName + '"></span>' +
                        '<div class="other">' +
                            '<ul class="clearfix">' +
                                '<li>评价<span>' + data.ShopList[item].BaoYangCommentRate + '</span></li>' +
                                //'<li>技术<span>' + data.ShopList[item].TechnicalLevel + '</span></li>' +
                                '<li>' + data.ShopList[item].BaoYangInstallQuantity + '单</li>' +
                            '</ul>' +
                        '</div>' +
                        '<p class="address">' + data.ShopList[item].Address + '</p>' +
                        '<span class="km">' + shopDistance + '</span>' +
                    '</div>' +
                    '<div class="icon clearfix">' +
                        '<div class="left">' +
                          posHtml +
                        '</div>' +
                        '<div class="right">' +
                            hzxHtml +
                        '</div>' +
                    '</div>' +
                '</div>';

                    $(".shop-list").append(htmlShops);

                }
                GetShopInstallPriceFor3M(SelectedCarDataFor3M.ProductID, SelectedCarDataFor3M.Pailiang, SelectedCarDataFor3M.Nian, shopIds)
            }
            // alert(SelectedCarDataFor3M.ProductID); alert(SelectedCarDataFor3M.Pailiang); alert(SelectedCarDataFor3M.Nian)          
            //GetShopInstallPriceFor3M("VE-DDZLYSJDZ","2.0L", "2012")
        }

    }).fail(function (data) {
        // alert("错误");
    })
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
    exp.setTime(exp.getTime() + OneDayTimes * 7);
    document.cookie = name + "=" + escape(value) + ";path=/;expires=" + exp.toGMTString();
}

//获取cookie
function GetCookie(name) {
    var arr = document.cookie.match(new RegExp("(^| )" + name + "=([^;]*)(;|$)"));
    if (arr != null) return unescape(arr[2]); return null;
}

