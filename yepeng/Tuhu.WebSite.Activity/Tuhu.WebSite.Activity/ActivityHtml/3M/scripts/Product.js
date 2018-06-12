$(function () {
    var SelectedCarDataFor3M = JSON.parse(GetCookie("SelectedCarDataFor3M"));
    //alert(SelectedCarDataFor3M);
    $(".car-info .img img").attr("src",SelectedCarDataFor3M.VehicleLogin)
    var plNian = SelectedCarDataFor3M.Pailiang + " " + SelectedCarDataFor3M.Nian;
    $(".car-info .other .name").html(SelectedCarDataFor3M.Vehicle);
    $(".car-info .other .type").html(plNian);

    $(".pack-list").on("tap", ".btn", function () {
        window.location.href = "/ActivityHtml/3M/index.html";
    })

    $(".shop-list").on("tap", ".btn", function () {
        window.location.href = "/ActivityHtml/3M/shop.html";
    })

    $(".footer").on("tap", ".btn", function () {
        var name = $(".shop-list .item .filed .name").text();
        var id = $(".shop-list .item .filed .name").attr("shopid");
        var km = $(".shop-list .item .filed .km").text();
        window.location.href = "/ActivityHtml/3M/SubmitOrder.html?km=" + km + "&name=" + name + "&price=" + GetQueryString("price") + "&id=" + id;
    })

    Selected3MPackage = JSON.parse(GetCookie("Selected3MPackage"));
    CreateSelectPackageHtml(Selected3MPackage);

    console.log(JSON.parse(GetCookie("SelectedShopBy3M")))
    SelectedShopBy3M = JSON.parse(GetCookie("SelectedShopBy3M"));

    CreateSelectedShopHtml(SelectedShopBy3M);

    var totalPrice = parseFloat(Selected3MPackage.Price) + parseFloat(GetQueryString("price"));
    $(".footer .price p").html("¥" + totalPrice)
})

var Selected3MPackage = "";
var SelectedShopBy3M = "";

function CreateSelectPackageHtml(Selected3MPackage) {

    var productHtml = "";

    for (var item in Selected3MPackage.Items) {
        productHtml += '<li>' +
                           '<div class="type">' + Selected3MPackage.Items[item].CatalogName + '</div>' +
                           '<div class="name"><p>' + Selected3MPackage.Items[item].DisplayName + '</p></div>' +
                     '</li>'
    }
    var packageHtml = ' <div class="pack-info clearfix">' +
                    '<div class="filed">' +
                        '<p class="name">' + Selected3MPackage.DisplayName + '</p>' +
                        '<p class="price">途虎价：<span><i>¥</i>' + Selected3MPackage.Price + '</span><del>¥' + Selected3MPackage.MarketPrice + '</del></p>' +
                    '</div>' +
                    '<a href="javascript:void(0);" class="btn">更换此套餐</a>' +
                '</div>' +
                '<div class="pro-list">' +
                    '<ul>' +
                    productHtml
    '</ul>' +
'</div>';
    $(".pack-list .item").html(packageHtml);
}

function CreateSelectedShopHtml(SelectedShopBy3M) {
    console.log(SelectedShopBy3M);
    if (SelectedShopBy3M.ShopClassification === "快修店") {
        ShopClassificationClass = "green";
    } else if (SelectedShopBy3M.ShopClassification === "维修厂") {
        ShopClassificationClass = "xiu";
    }
    else if (SelectedShopBy3M.ShopClassification === "4S店") {
        ShopClassificationClass = "blue";
    }
    var posHtml = "";
    if (SelectedShopBy3M.POS.indexOf("支付宝") > -1) {
        posHtml += '<span class="i-alpay">支付宝</span>';
    }
    if (SelectedShopBy3M.POS.indexOf("微信") > -1) {
        posHtml += '<span class="i-wx">微信</span>';
    }
    if (SelectedShopBy3M.POS.indexOf("刷卡") > -1) {
        posHtml += '<span class="i-bank">网银</span>';
    }
    if (SelectedShopBy3M.POS.indexOf("现金") > -1) {
        posHtml += '<span class="i-cash">现金</span>';
    }
    var hzxHtml = "";
    if (SelectedShopBy3M.ShopType & 8 == 8) {//直
        hzxHtml += '<span class="i-z">直</span>';
    }
    if (SelectedShopBy3M.ShopType & 16 == 16) {//虎
        hzxHtml += '<span class="i-hu">虎</span>';
    }
    if (SelectedShopBy3M.ShopType & 128 == 128) {//星
        hzxHtml += '<span class="i-star">星</span>';
    }
    var shopHtml = '   <div class="item clearfix">' +
                '<div class="img">' +
                    '<img src="' + SelectedShopBy3M.ShopImg + '" alt="" width="90" height="70" />' +
                     '<p class="label ' + ShopClassificationClass + '">' + SelectedShopBy3M.ShopClassification + '</p>' +
                '</div>' +
                '<div class="filed clearfix">' +
                    '<p class="name" shopid="' + SelectedShopBy3M.PKID + '">' + SelectedShopBy3M.CarParName + '</p>' +
                    '<span class="price">¥' + GetQueryString("price") + '</span>' +
                    '<div class="other">' +
                        '<ul class="clearfix">' +
                            '<li>评价<span>' + SelectedShopBy3M.BaoYangCommentRate + '</span></li>' +
                            '<li>技术<span>' + SelectedShopBy3M.TechnicalLevel + '</span></li>' +
                            '<li>' + SelectedShopBy3M.BaoYangInstallQuantity + '单</li>' +
                        '</ul>' +
                    '</div>' +
                    '<p class="address">' + SelectedShopBy3M.Address + '</p>' +
                    '<span class="km">' + GetQueryString("km") + '</span>' +
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

    $(".shop-list .btn").before(shopHtml);
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

