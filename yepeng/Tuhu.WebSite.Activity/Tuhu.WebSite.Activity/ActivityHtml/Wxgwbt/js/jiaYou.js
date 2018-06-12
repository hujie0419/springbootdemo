$(function () {
	document.querySelector(".layer").style.height = document.body.scrollHeight  + "px";
	setTimeout(function () {
		document.querySelector(".layer").style.height = document.body.scrollHeight  + "px";
	}, 3000)
	
	var $tip = $(".tip");
	function validPhone (phone) {
		if(!phone.trim()){
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
	
	//60s 倒计时
	var config = {
	    times: 60,
	    defaultTime: 60,
	    state: true,
	    dis: "disabled",
	    disTxt: "秒后重试",
	    initTxt: "获取验证码"
	};

	//领取倒计时
	function GetCouponTime() {
	    $("#GetCoupon").removeAttr("disabled").attr("class", "mslq");
	}

    //获取验证码
	function getCode() {
	    var phone = $("#tel_phone").val();
	    if (validPhone(phone) == true) {
	        $.ajax({
	            url: "http://wx.tuhu.cn/Wxgwbt/GetCode?callback=?",
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
	                    alert(msg);
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

    //获取优惠券
	function GetCoupon() {
	    $("#GetCoupon").attr("disabled", "disabled").attr("class", "mslqd");
	    var phone = $("#tel_phone").val();
	    var code = $("#tel_code").val();
	    if (validPhone(phone) == true && code.length > 0) {
	        $.ajax({
	            url: "http://wx.tuhu.cn/Wxgwbt/GetCoupon?callback=?",
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
	                    alert(msg);
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
	                    $(".jy_layer").show();
	                    $(".layer").show();
	                    //alert(msg);
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

	$("#getCode").tap(function () {
	    var phone = $("#tel_phone").val();
	    if (validPhone(phone) == true) {
	        var that = $(this);
	        if (config.times == config.defaultTime) {
	            that.addClass(config.dis).text(config.times + config.disTxt);
	            var timer = setInterval(function () {
	                if (!config.state) {
	                    clearInterval(timer);
	                    config.state = true;
	                    config.times = config.defaultTime;
	                    that.removeClass(config.dis).text(config.initTxt);
	                    return;
	                }
	                config.times--;
	                that.text(config.times + config.disTxt);
	                if (config.times <= 0) {
	                    config.state = false;
	                }
	            }, 1000);

	            getCode();
	        }
	    }
	});

	$("#GetCoupon").tap(function () {
	    GetCoupon();
	});
})