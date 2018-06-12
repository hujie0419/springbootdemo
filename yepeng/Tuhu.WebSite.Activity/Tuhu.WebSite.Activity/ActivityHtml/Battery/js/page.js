var pid = "";
var vid = "";
var regionSelect = "";
$(function () {
    regionSelect = GetQueryString("RegionSelect");

    var name = GetCookie("vehicleName");
    var imge = GetCookie("vehicleLogin");

    $("#vehicleName").html(name);
    $("#loginImg").attr("src", imge);


    $("#more").on("click", function () {
        $(this).toggleClass("show");
        $("#more_list").toggleClass("show");
    });

    $(".checkbox").on("click", function () {
        $(".checkbox").toggleClass("select");
    })

    var selectedRegion = JSON.parse(decodeURIComponent(GetCookie("SelectedRegion")));
    // var productData = JSON.parse(GetCookie("ProductData"));
    var currentCityItem = JSON.parse(decodeURIComponent(GetCookie("CurrentCityItem")));

    var productData = JSON.parse(window.name);
    //console.log(selectedRegion);
    //console.log(JSON.stringify(productData));
    console.log(JSON.stringify(currentCityItem.BrandCoverList));
    console.log(JSON.stringify(productData));
    var dataArray = new Array();
    var i = -1;
    for (var one in productData) {

        for (var key in currentCityItem.BrandCoverList) {

            if (productData[one].Brand == currentCityItem.BrandCoverList[key].Brand) {

                var jsonData = { "ProductID": "", "VariantID": "", "Image": "", "DisplayName": "", "SalesQuantity": "", "CommentTimes": "", "Price": "", "ServiceRemark": "" };
                jsonData.ProductID = productData[one].ProductID;
                jsonData.Image = productData[one].Image;
                jsonData.DisplayName = productData[one].DisplayName;
                jsonData.SalesQuantity = productData[one].SalesQuantity;
                jsonData.CommentTimes = productData[one].CommentTimes;
                jsonData.Price = productData[one].Price;
                jsonData.VariantID = productData[one].VariantID;
                jsonData.ServiceRemark = currentCityItem.BrandCoverList[key].ServiceRemark;
                i++;
                dataArray[i] = jsonData;
            }
        }

    }
   
    var newary = dataArray.slice(0, 5);
    //alert(JSON.stringify(dataArray));
    //console.log(JSON.stringify(dataArray));
    SetCookie("currentProductItem", JSON.stringify(newary));
    //console.log(GetCookie("currentProductItem"));
    //console.log(JSON.stringify(dataArray[0].Image));

    var productid = GetQueryString("productid");

    if (productid != null && productid != "") {

        for (var item in dataArray) {
            if (dataArray[item].ProductID == productid) {

                $("#imagev").attr("src", dataArray[item].Image);
                $(".prices").html(dataArray[item].Price);
                $(".name").html(dataArray[item].DisplayName);
                $(".remark").html(dataArray[item].ServiceRemark);
                pid = dataArray[item].ProductID;
                vid = dataArray[item].VariantID;

                SetCookie("currentSelecrProduct", JSON.stringify(dataArray[item]));
                console.log(JSON.stringify(GetCookie("currentSelecrProduct")));
                //alert("currentSelecrProduct=" + JSON.stringify(GetCookie("currentSelecrProduct")));
            }
        }

    }
    else {

        $("#imagev").attr("src", dataArray[0].Image);
        $(".prices").html(dataArray[0].Price);
        $(".name").html(dataArray[0].DisplayName);
        $(".remark").html(dataArray[0].ServiceRemark);
        pid = dataArray[0].ProductID;
        vid = dataArray[0].VariantID;
        SetCookie("currentSelecrProduct", JSON.stringify(dataArray[0]));
        console.log(JSON.stringify(GetCookie("currentSelecrProduct")));
        //alert("currentSelecrProduct=" + JSON.stringify(GetCookie("currentSelecrProduct")));

    }


})

function GetQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return decodeURIComponent(r[2]); return null;
}

function GengHuan() {

    window.location.href = "/ActivityHtml/Battery/productlist.html?RegionSelect=" + encodeURIComponent(regionSelect);
}

function ToDetail() {
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

    var userAgentInfo = navigator.userAgent;

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

function ToPay() {

    var select = $(".checkbox").attr("class");

    if (select == "checkbox select") {
        window.location.href = "/ActivityHtml/Battery/order.html?RegionSelect=" + decodeURIComponent(regionSelect);
    } else {
        alert("请同意条款！");
    }

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



