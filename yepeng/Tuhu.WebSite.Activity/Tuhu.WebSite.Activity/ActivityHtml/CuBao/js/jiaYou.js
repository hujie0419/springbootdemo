$(function () {
    document.querySelector(".layer").style.height = document.body.scrollHeight + "px";
    setTimeout(function () {
        document.querySelector(".layer").style.height = document.body.scrollHeight + "px";
    }, 3000)

    var $tip = $(".tip");
    var $GetCoupon = $("#GetCoupon");

    //60s 倒计时
    var config = {
        times: 3,
        defaultTime: 3,
        state: true,
        dis: "disabled",
        initTxt: "马上领取",
        loadingTxt: "领取中..."
    };

    var browser = {
        versions: function () {
            var u = navigator.userAgent, app = navigator.appVersion;
            return {         //移动终端浏览器版本信息
                trident: u.indexOf('Trident') > -1, //IE内核
                presto: u.indexOf('Presto') > -1, //opera内核
                webKit: u.indexOf('AppleWebKit') > -1, //苹果、谷歌内核
                gecko: u.indexOf('Gecko') > -1 && u.indexOf('KHTML') == -1, //火狐内核
                mobile: !!u.match(/AppleWebKit.*Mobile.*/), //是否为移动终端
                ios: !!u.match(/\(i[^;]+;( U;)? CPU.+Mac OS X/), //ios终端
                android: u.indexOf('Android') > -1 || u.indexOf('Linux') > -1, //android终端或uc浏览器
                iPhone: u.indexOf('iPhone') > -1, //是否为iPhone或者QQHD浏览器
                iPad: u.indexOf('iPad') > -1, //是否iPad
                webApp: u.indexOf('Safari') == -1 //是否web应该程序，没有头部与底部
            };
        }(),
        language: (navigator.browserLanguage || navigator.language).toLowerCase()
    }

    var tipMsg = {
        show: function (msg) {
            $tip.css("visibility", "visible").html(msg);
        },
        hide: function () {
            $tip.css("visibility", "hidden").html("&nbsp;");
        }
    }

    var couponMsg = {
        value: function (msg) {
            $GetCoupon.val(msg);
        },
        disabled: function () {
            $GetCoupon.attr("class", "mslqd");
        },
        enabled: function () {
            $GetCoupon.attr("class", "mslq");
        }
    }

    function GetQueryString(name) {
        /// <summary>获取页面参数</summary>
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
        var r = window.location.search.substr(1).match(reg);
        if (r != null) return unescape(r[2]); return null;
    }

    function validPhone(phone) {
        if (!phone) { return false;}

        if (phone.length != 11) {
            var msg = "手机号的长度不是11位";
            tipMsg.show(msg);
            return false;
        }

        var reg = /^1\d{10}$/;
        if (!reg.test(phone)) {
            var msg = "手机号必须是以1开头";
            tipMsg.show(msg);
            return false;
        }

        tipMsg.hide(msg);
        return true;
    }

    function is_weixn() {
        var ua = navigator.userAgent.toLowerCase();
        if (ua.match(/MicroMessenger/i) == "micromessenger") {
            return true;
        } else {
            return false;
        }
    }

    function SetInitPhone() {
        $("#userPhone").html(GetQueryString("phone") || "");
    }

    $("#GetCoupon").tap(function () {
        var phone = GetQueryString("phone");
        var typePage = GetQueryString("type") || 0;
        if (validPhone(phone) == true) {
            if (config.times == config.defaultTime) {
           
                couponMsg.value(config.loadingTxt);
                couponMsg.disabled();

                var times = setInterval(function () {
                    if (!config.state) {
                        clearInterval(times);
                        config.state = true;
                        config.times = config.defaultTime;

                        var ajaxUrl = (typePage == 1 ? "http://wx.tuhu.cn/Wxgwbt/GetCouponForCuBaoPhone?callback=?" : "http://wx.tuhu.cn/Wxgwbt/GetCouponForCuBao?callback=?");

                        $.ajax({
                            url: ajaxUrl,
                            data: { "phone": phone },
                            dataType: "jsonp",
                            type: "get",
                            success: function (data) {
                                couponMsg.enabled();
                                var result = parseInt(data);
                                if (result == -1) {
                                    var msg = "本次活动已结束";
                                    couponMsg.disabled();
                                    couponMsg.value(msg);
                                }
                                else if (result == -2) {
                                    var msg = "输入的手机号码或验证码不正确";
                                    tipMsg.show(msg);
                                    couponMsg.enabled();
                                    couponMsg.value(config.initTxt);
                                }
                                else if (result == -4) {
                                    var msg = "抱歉，您已领取过";
                                    couponMsg.enabled();
                                    couponMsg.value(msg);
                                }
                                else if (result == -5) {
                                    var msg = "系统错误";
                                    tipMsg.show(msg);
                                    couponMsg.value(config.initTxt);
                                }
                                else if (result == 0) {
                                    var msg = "领取失败";
                                    tipMsg.show(msg);
                                    couponMsg.value(config.initTxt);
                                }
                                else if (result == 1) {
                                    var msg = "领取成功";
                                    couponMsg.value(msg);
                                }
                                else {
                                    var msg = "网络异常请重试";
                                    tipMsg.show(msg);
                                    couponMsg.value(config.initTxt);
                                }
                            }
                        });
                    }

                    config.times--;

                    if (config.times <= 0) {
                        config.state = false;
                    }
                }, 1000);
            }
        }
        else {
            var msg = "手机号输入有误";
            tipMsg.show(msg);
        }
    });

    $("#tipClose").tap(function () {
        $(".downApp").hide();
    });

    $("#tipDown").tap(function () {
        var androidUrl = "http://dwz.cn/1HYW8X";
        var iosUrl = "http://dwz.cn/1HYWUl"

        if (is_weixn()) {
            window.location.href = androidUrl;
        }
        else {
            if (browser.versions.ios) {
                window.location.href = iosUrl;
            }
            else if (browser.versions.android) {
                window.location.href = androidUrl;
            }
            else {
                window.location.href = androidUrl;
            }
        }
    });

    SetInitPhone();
});