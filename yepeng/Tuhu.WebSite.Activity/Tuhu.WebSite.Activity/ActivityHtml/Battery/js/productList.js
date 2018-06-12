
var regionSelect = "";
$(function () {
    regionSelect = GetQueryString("RegionSelect");
    var htmlProduct = "";
    var currentProductItem = JSON.parse(GetCookie("currentProductItem"));
    //console.log(currentProductItem);
    for (var item in currentProductItem) {

        htmlProduct = '<tr class="bb1" data-href="" onclick="btnClick(this)">' +
                '<td class="img">  <input type="hidden" class="jsonData" value="' + currentProductItem[item].ProductID + '" />' +
                    '<img style="width: 100px;height: 100px;" src="' + currentProductItem[item].Image + '" class="commodityimg">' +
                '</td>' +
                '<td class="info">' +
                    '<div class="adapt_title">' +
                        '<a style="color:black;" href="javascript:;">' + currentProductItem[item].DisplayName +
                       '</a>' +
                    '</div>' +
                    '<div class="adapt">' +
                        '<span class="adapt_num">已售：' + currentProductItem[item].SalesQuantity + ' 件　</span>' +
                        '<span class="adapt_price r"><span style="color:red;">¥ ' + currentProductItem[item].Price + '</span> x1</span>' +
                    '</div>' +
                '</td>' +
           ' </tr>'

        $("table tbody").append(htmlProduct);
    }

})

function btnClick(e) {
    var productid = $(e).children("td:eq(0)").find("input[type='hidden']").val();

    window.location.href = "/ActivityHtml/Battery/page.html?productid=" + productid + "&RegionSelect=" + decodeURIComponent(regionSelect);
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

function GetQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return decodeURIComponent(r[2]); return null;
}