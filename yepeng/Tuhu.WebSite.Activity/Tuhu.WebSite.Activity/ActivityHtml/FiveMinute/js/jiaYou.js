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

	function is_weixn() {
	    var ua = navigator.userAgent.toLowerCase();
	    if (ua.match(/MicroMessenger/i) == "micromessenger") {
	        return true;
	    } else {
	        return false;
	    }
	}
	
	//fixed placeholder
	$('.input').each(function(){
		var that = $(this),
			prev = that.prev();
		that[0].oninput = function () {
			if(this.value){
				prev.text("")
			}
			else{
				prev.text(prev.attr("data-pholder"));
				$(".getCode").addClass(config.dis);
			}
		}
	})
	
	$('.input').eq(0).blur(function () {
		if(this.value){
			validPhone(this.value);
			$(".getCode").removeClass(config.dis);
		}
		else{
			$(".getCode").addClass(config.dis);
		}
	})
	
	//60s 倒计时
	var config = {
		times:60,
		defaultTime:60,
		state:true,
		dis:"disabled",
		disTxt:"秒后重试",
		initTxt:"获取验证码"
	}

	//领取倒计时
	function GetCouponTime() {
	    $("#GetCoupon").show();
	    $("#DisCoupon").hide();
	}

    //跳转APP
	function AutoGoToPage() {
	    if (is_weixn) {
	        window.location.href = "http://a.app.qq.com/o/simple.jsp?pkgname=cn.TuHu.android";
	    }
	    else {
	        if (browser.versions.android) {
	            window.location.href = "http://a.app.qq.com/o/simple.jsp?pkgname=cn.TuHu.android";
	        }
	        else if (browser.versions.ios) {
	            window.location.href = "http://lnk8.cn/osY5Yp";
	        }
	        else {
	            window.location.href = "http://a.app.qq.com/o/simple.jsp?pkgname=cn.TuHu.android";
	        }
	    }
	}

    //获取验证码
	function getCode() {
	    var phone = $("#tel_phone").val();
	    if (validPhone(phone) == true) {
	        $.ajax({
	            url: "http://wx.tuhu.cn/Wxgwbt/Get58FamilyCode?callback=?",
	            data: { "phone": phone },
	            dataType: "jsonp",
	            type: "get",
	            success: function (data) {
	                var result = parseInt(data);
	                if (result == -1) {
	                    var msg = "活动时间为 8月28-9月30";
	                    $tip.text(msg).show();
	                    alert(msg);
	                }
	                else if (result == -2) {
	                    var msg = "手机号码不正确";
	                    $tip.text(msg).show();
	                }
	                else if (result == -3) {
	                    var msg = "验证码发送失败";
	                    $tip.text(msg).show();
	                }
	                else if (result == -4) {
	                    var msg = "您不是新用户，不能领取";
	                    $tip.text(msg).show();
	                    alert(msg);
	                }
	                else if (result == -5) {
	                    var msg = "系统错误";
	                    $tip.text(msg).show();
	                }
	                else if (result == 0) {
	                    var msg = "验证码发送失败";
	                    $tip.text(msg).show();
	                }
	                else if (result == 1) {
	                    var msg = "验证码发送成功";
	                    $tip.text(msg).show();
	                }
	                else {
	                    var msg = "未知错误";
	                    $tip.text(msg).show();
	                }
	            }
	        });
	    }
	}

    //获取优惠券
	function GetCoupon() {
	    $("#GetCoupon").hide();
	    $("#DisCoupon").show();
	    var phone = $("#tel_phone").val();
	    var code = $("#tel_code").val();
	    if (validPhone(phone) == true && code.length > 0) {
	        $.ajax({
	            url: "http://wx.tuhu.cn/Wxgwbt/Get58FamilyCoupon?callback=?",
	            data: { "phone": phone, "code": code },
	            dataType: "jsonp",
	            type: "get",
	            success: function (data) {
	                var result = parseInt(data);
	                if (result == -1) {
	                    var msg = "活动时间为 8月28-9月30";
	                    $tip.text(msg).show();
	                    alert(msg);
	                }
	                else if (result == -2) {
	                    var msg = "输入的手机号码或验证码不正确";
	                    $tip.text(msg).show();
	                }
	                else if (result == -3) {
	                    var msg = "验证码不正确";
	                    $tip.text(msg).show();
	                }
	                else if (result == -4) {
	                    var msg = "您不是新用户，不能领取";
	                    $tip.text(msg).show();
	                    alert(msg);
	                }
	                else if (result == -5) {
	                    var msg = "系统错误";
	                    $tip.text(msg).show();
	                }
	                else if (result == 0) {
	                    var msg = "领取失败";
	                    $tip.text(msg).show();
	                }
	                else if (result == 1) {
	                    var msg = "领取成功";
	                    $tip.text(msg).show();
	                    $(".jy_layer").show();
	                    $(".layer").show();
	                    setTimeout(AutoGoToPage, 2000);
	                }
	                else {
	                    var msg = "未知错误";
	                    $tip.text(msg).show();
	                }
	            }
	        });
	        setTimeout(GetCouponTime, 3000);
	    }
	    else {
	        var msg = "手机号或验证码输入有误";
	        $tip.text(msg).show();
	        setTimeout(GetCouponTime, 3000);
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
	})

	$("#GetCoupon").tap(function () {
	    GetCoupon();
	});
})