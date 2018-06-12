$(function () {
    document.querySelector(".layer").style.height = document.body.scrollHeight + "px";
    setTimeout(function () {
        document.querySelector(".layer").style.height = document.body.scrollHeight + "px";
    }, 3000)

    $("#rule").click(function () {
        $(".rule").show();
    });

    $("#close").click(function (event) {
        $(".rule").hide();
    });

    var $tip = $(".errmsg > span");

    function validPhone(phone) {
        if (!phone.trim()) {
            return;
        }
        if (phone.length != 11) {
            var msg = "手机号的长度不是11位";
            $tip.text(msg).show();
            return false;
        }
        var reg = /^1\d{10}$/;
        if (!reg.test(phone)) {
            var msg = "手机号必须是以1开头";
            $tip.text(msg).show();
            return false;
        }
        return true;
    }

    var browser = {
        versions: function () {
            var u = navigator.userAgent,
                app = navigator.appVersion;
            return { //移动终端浏览器版本信息
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

    function is_weixn() {
        var ua = navigator.userAgent.toLowerCase();
        if (ua.match(/MicroMessenger/i) == "micromessenger") {
            return true;
        } else {
            return false;
        }
    }

    $("#imgClose").tap(function () {
        $(".downapp").hide();
    });

    var getCodeObj = null;
    var getCodeSecond = 60;

    function getCodeTip() {
        getCodeSecond--;
        if (getCodeSecond < 1) {
            getCodeSecond = 60;
            clearInterval(getCodeObj);
            $("#getCode").removeClass("disabled").text("获取验证码");
        } else {
            $("#getCode").addClass("checkcode disabled").text(getCodeSecond + "秒后重试");
        }
    }
    //获取验证码
    $("#getCode").tap(function () {
        var phone = $("#tel_phone").val();
        if (validPhone(phone) == true) {
            if (getCodeSecond == 60) {
                $("#getCode").addClass("checkcode disabled");
                getCodeObj = setInterval(getCodeTip, 1000);
                $.ajax({
                    url: "http://wx.tuhu.cn/Wxgwbt/GetCodeForMI?callback=?",
                    data: {
                        "phone": phone
                    },
                    dataType: "jsonp",
                    type: "get",
                    success: function (data) {
                        var result = parseInt(data);
                        if (result == -1) {
                            var msg = "不在活动时间范围内";
                            $tip.text(msg).show();
                            alert(msg);
                        } else if (result == -2) {
                            var msg = "手机号码不正确";
                            $tip.text(msg).show();
                        } else if (result == -3) {
                            var msg = "验证码发送失败";
                            $tip.text(msg).show();
                        } else if (result == -4) {
                            var msg = "您已经领取过了，不能领取";
                            $tip.text(msg).show();
                            alert(msg);
                        } else if (result == -5) {
                            var msg = "系统错误";
                            $tip.text(msg).show();
                        } else if (result == 0) {
                            var msg = "验证码发送失败";
                            $tip.text(msg).show();
                        } else if (result == 1) {
                            var msg = "验证码发送成功";
                            $tip.text(msg).show();
                        } else {
                            var msg = "未知错误";
                            $tip.text(msg).show();
                        }
                    }
                });
            }
        }
    });

    var getCouponObj = null;
    var getCouponSecond = 3;

    function getCouponTip() {
        getCouponSecond--;
        if (getCouponSecond < 1) {
            clearInterval(getCouponObj);
            getCouponSecond = 3;
            $("#getCoupon").removeClass("disabled");
        } else {
            $("#getCoupon").addClass("btn disabled");
        }
    }
    //获取优惠券
    $("#getCoupon").tap(function () {
        var phone = $("#tel_phone").val();
        var code = $("#tel_code").val();
        if (validPhone(phone) == true && code.length > 0) {
            if (getCouponSecond == 3) {
                $("#getCoupon").addClass("btn disabled");
                getCouponObj = setInterval(getCouponTip, 1000);
                $.ajax({
                    url: "http://wx.tuhu.cn/Wxgwbt/GetCouponForMI?callback=?",
                    data: {
                        "phone": phone,
                        "code": code
                    },
                    dataType: "jsonp",
                    type: "get",
                    success: function (data) {
                        var result = parseInt(data);
                        if (result == -1) {
                            var msg = "不在活动时间范围内";
                            $tip.text(msg).show();
                            alert(msg);
                        } else if (result == -2) {
                            var msg = "输入的手机号码或验证码不正确";
                            $tip.text(msg).show();
                        } else if (result == -3) {
                            var msg = "验证码不正确";
                            $tip.text(msg).show();
                        } else if (result == -4) {
                            var msg = "您已经领取过了，不能领取";
                            $tip.text(msg).show();
                            alert(msg);
                        } else if (result == -5) {
                            var msg = "系统错误";
                            $tip.text(msg).show();
                        } else if (result == 0) {
                            var msg = "领取失败";
                            $tip.text(msg).show();
                        } else if (result == 1) {
                            var msg = "领取成功";
                            $tip.text(msg).show();
                            $(".jy_layer").show();
                            $(".layer").show();
                        } else {
                            var msg = "未知错误";
                            $tip.text(msg).show();
                        }
                    }
                });
            }
        } else {
            var msg = "手机号或验证码输入有误";
            $tip.text(msg).show();
        }
    });

    //中奖名单
    function InvokeRandomData() {
        $.ajax({
            url: "http://wx.tuhu.cn/Wxgwbt/GetRandomForMI?callback=?",
            data: {},
            dataType: "jsonp",
            type: "get",
            success: function (data) {
                var showHtml = "";
                var templateHtml = "<li><span>#phone</span><span>#Content</span></li>";
                $.each(JSON.parse(data), function (i, obj) {
                    showHtml += templateHtml.replace(/#phone/ig, obj.PhoneRandom).replace(/#Content/ig, obj.CouponsRandom);
                    if (i == 1) {
                        $("#list_count").html(obj.WinningCount);
                    }
                });
                $("#list_lh > ul").html(showHtml);
                scrollNews("#list_lh");
            }
        });
    }
    //中奖累加
    function InvokeTimerData() {
        var _winningObj = $("#list_count");
        var _winningCount = parseInt($.trim($(_winningObj).html())) + 10;
        $(_winningObj).html(_winningCount)
    }
    //滚动
    function scrollNews(obj) {
        var $self = $(obj).find("ul");
        var lineHeight = $self.find("li").height();

        setInterval(function () {
            $self.animate({
                "margin-top": -lineHeight + "px"
            }, 800, 'ease-out', function () {
                $self.css({
                    "margin-top": 0
                }).find("li").eq(0).appendTo($self);
            })
        }, 2000)
    }
    
    InvokeRandomData();
    setInterval(InvokeTimerData, 1000 * 60); //每分钟调用一次
});