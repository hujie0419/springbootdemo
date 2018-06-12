
$(function () {
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
    $(".layer").height($(document).height());
    setTimeout(function () {
        $(".layer").height($(document).height());
    }, 500);

    //60s 倒计时
    var config = {
        times: 60,
        defaultTime: 60,
        state: true,
        dis: "disabled",
        disTxt: "秒后重试",
        initTxt: "获取验证码"
    };

    //fixed placeholder
    $('.input').each(function () {
        var that = $(this),
			prev = that.prev();
        that[0].oninput = function () {
            if (this.value) {
                prev.text("")
            }
            else {
                prev.text(prev.attr("data-pholder"));
                $(".getCode").addClass(config.dis);
            }
        }
    });

    $('.input').eq(0).blur(function () {
        if (this.value) {
            validPhone(this.value);
            $(".getCode").removeClass(config.dis);
        }
        else {
            $(".getCode").addClass(config.dis);
        }
    });

    //$("#getCode").tap(function () {
    //    var phone = $("#tel_phone").val();
    //    if (validPhone(phone) == true) {
    //        var that = $(this);
    //        if (config.times == config.defaultTime) {
    //            that.addClass(config.dis).text(config.times + config.disTxt);
    //            var timer = setInterval(function () {
    //                if (!config.state) {
    //                    clearInterval(timer);
    //                    config.state = true;
    //                    config.times = config.defaultTime;
    //                    that.removeClass(config.dis).text(config.initTxt);
    //                    return;
    //                }
    //                config.times--;
    //                that.text(config.times + config.disTxt);
    //                if (config.times <= 0) {
    //                    config.state = false;
    //                }
    //            }, 1000);

    //            GetCode();
    //        }
    //    }
    //});

    var rd = Math.random();

    $("#getCode").tap(function () {
        var phone = $("#tel_phone").val();
        if (validPhone(phone) == true) {
            if (getCodeSecond == 60) {
                $("#getCode").addClass("checkcode disabled");
                getCodeObj = setInterval(getCodeTip, 1000);
                $.ajax({
                    url: "http://wx.tuhu.cn/Wxgwbt/GetOillingCode?callback=?&v=" + rd,
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
                            MsgBoxErrror();
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
            else { return; }
        }
    });

    $("#GetCoupon").tap(function () {
        $("#tel_code").blur();
        GetCoupon();
    });
})

var $tip = $(".tip");


//领取倒计时
function GetCouponTime() {
    $("#GetCoupon").removeAttr("disabled").attr("class", "mslq");
}


function MsgBoxSuccess() {
    $(".success").show();
    $(".layer").show();
    $(".got").hide();
}

function MsgBoxErrror() {
    $(".got").show();
    $(".layer").show();
    $(".success").hide();
}

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

//获取优惠券
function GetCoupon() {

    var phone = $("#tel_phone").val();
    var code = $("#tel_code").val();
    if (validPhone(phone) == true && code.length > 0) {

        $("#GetCoupon").attr("disabled", "disabled").attr("class", "mslqd");
        $.ajax({
            url: "http://wx.tuhu.cn/Wxgwbt/GetOillingCoupon?callback=?",
            data: { "phone": phone, "code": code },
            dataType: "jsonp",
            type: "get",
            success: function (data) {

                var result = parseInt(data);
                if (result == -1) {
                    var msg = "不在活动时间范围内";
                    $tip.text(msg).show();
                    alert(msg);
                }
                else if (result == -2) {
                    var msg = "输入的手机号码或验证码不正确";
                    $tip.text(msg).show();
                    //alert(msg);
                }
                else if (result == -3) {
                    var msg = "验证码不正确";
                    $tip.text(msg).show();
                    //alert(msg);
                }
                else if (result == -4) {
                    var msg = "您已经领取过，不能再次领取";
                    $tip.text(msg).show();
                    MsgBoxErrror();
                }
                else if (result == -5) {
                    var msg = "系统错误";
                    $tip.text(msg).show();
                    //alert(msg);
                }
                else if (result == 0) {
                    var msg = "领取失败";
                    $tip.text(msg).show();
                    //alert(msg);
                }
                else if (result == 1) {
                    var msg = "领取成功";
                    $tip.text(msg).show();
                    MsgBoxSuccess();
                }
                else {
                    var msg = "未知错误";
                    $tip.text(msg).show();
                    //alert(msg);
                }
            }
        });
        setTimeout(GetCouponTime, 2000);
    }
    else {
        var msg = "手机号或验证码输入有误";
        $tip.text(msg).show();
        setTimeout(GetCouponTime, 2000);
    }
}


//获取验证码
function GetCode() {
    var phone = $("#tel_phone").val();
    if (validPhone(phone) == true) {
        $.ajax({
            url: "http://wx.tuhu.cn/Wxgwbt/GetOillingCode?callback=?",
            data: { "phone": phone },
            dataType: "jsonp",
            type: "get",
            success: function (data) {

                var result = parseInt(data);

                if (result == -1) {
                    var msg = "不在活动时间范围内";
                    $tip.text(msg).show();
                    alert(msg);
                }
                else if (result == -2) {
                    var msg = "手机号码不正确";
                    $tip.text(msg).show();
                    //alert(msg);
                }
                else if (result == -3) {
                    var msg = "验证码发送失败";
                    $tip.text(msg).show();
                    //alert(msg);
                }
                else if (result == -4) {
                    var msg = "您已经领取过，不能再次领取！";
                    $tip.text(msg).show();
                    MsgBoxErrror();
                }
                else if (result == -5) {
                    var msg = "系统错误";
                    $tip.text(msg).show();
                    //alert(msg);
                }
                else if (result == 0) {
                    var msg = "验证码发送失败";
                    $tip.text(msg).show();
                    //alert(msg);
                }
                else if (result == 1) {
                    var msg = "验证码发送成功";
                    $tip.text(msg).show();
                    //alert(msg);
                }
                else {
                    var msg = "未知错误";
                    $tip.text(msg).show();
                    //alert(msg);
                }
            }
        });
    }
}