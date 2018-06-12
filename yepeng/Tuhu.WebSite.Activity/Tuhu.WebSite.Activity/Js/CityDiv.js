//选择区域
var vscrollTop = 0;
var getArea = false;
function cityshow() {
    vscrollTop = $.mobile.activePage.scrollTop();
    $("#city").show();
    if (!getArea) {
        $("#loadon").show();
        GetRegion();
    }
    $.mobile.activePage.scrollTop(0);
    $.mobile.silentScroll(0);
}
function Cityhide() {
    $("#city").hide();
    $.mobile.activePage.scrollTop(vscrollTop);
    $.mobile.silentScroll(0);
}
function GetRegion() {
    $.ajax({
        url: Domain + "/ZeroActive/Region.html",
        type: "get",
        success: function (region) {
            $("#loadon").hide();
            getArea = true;
            $.each(region, function () {
                if (this.IsInstall && this.ParentID == 0) {
                    var parent = $("<div class='cityname' data-value='" + this.PKID + "' data-parentName='" + this.RegionName + "'><label data-role=\"none\" data-id=\"" + "0" + "\">" + this.RegionName + "<span class=\"arrow-s-r\"></span></label></div><hr class='hr' />");
                    $(".city_div").append(parent);
                    var provinceid = this.PKID, provincename = this.RegionName;
                    $.each(this.ChildrenRegion, function () {
                        if (this.IsInstall) {
                            var sb = "<span><hr class='hr' /><span class='cityname' data-id=\"" + "0" + "\" onclick='cityhide(\"" + provinceid + "\",\"" + provincename + "\",\"" + this.PKID + "\",\"" + this.RegionName + "\")' >" + this.RegionName + "</span></span>";
                            $(".cityname[data-parentName='" + provincename + "']").append(sb);
                        }
                    });
                }
            });
            $(".city_div>.cityname").click(function () {
                $(this).children("label").children("span").toggleClass("arrow-s-b");
                $(this).children("span").toggle();
            });
        },
        error: function () {
        }
    });
}
function cityhide(provinceCode, provinceName, cityCode, cityName) {
    $("#city").slideUp("fast");
    $("#txtcity").val(provinceName + '-' + cityName);
    $("#provinceCode").val(provinceCode);
    $("#cityCode").val(cityCode);
    $.mobile.activePage.scrollTop(vscrollTop);
    $.mobile.silentScroll(0);
}