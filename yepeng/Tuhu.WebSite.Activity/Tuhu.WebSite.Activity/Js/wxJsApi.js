﻿function getAppId() {
    return "wx21f3300bbe61dc90";
}
var timestamp;
var noce;
var timestamp = new Date().getTime();
var timestampstring = timestamp.toString().substring(0, timestamp.toString().length - 3);
var error = false;
var $chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
var maxPos = $chars.length;
var noceStr = "";
for (i = 0; i < 32; i++) {
    noceStr += $chars.charAt(Math.floor(Math.random() * maxPos));
}
noce = noceStr;

var sign = "";
//window.onload = function () {
//    alert("进方法getSign了");
//    var sign = "";
//    $.ajax({
//        url: '/home/GetTicket',
//        type: "post",
//        dataType: "json",
//        data: {},
//        async: false,
//        success: function (data) {
//            if (data["result"] == "1") {
//                alert("ticket:" + data["ticket"])
//                var url = location.href;
//                var nonce_str = getNonceStr().toString();
//                var time_stamp = getTimeStamp().toString();
//                var keyvaluestring = "jsapi_ticket=" + data["ticket"] + "&noncestr=" + nonce_str + "&timestamp=" + time_stamp + "&url=" + url;
//                sign = hex_sha1(keyvaluestring).toString();
//            }
//        }
//    });
//}
'use strict'; (function (D) {
    function p(b, e, c) {
        var a = 0, d = [0], f = "", g = null, f = c || "UTF8"; if ("UTF8" !== f && "UTF16" !== f) throw "encoding must be UTF8 or UTF16"; if ("HEX" === e) { if (0 !== b.length % 2) throw "srcString of HEX type must be in byte increments"; g = v(b); a = g.binLen; d = g.value } else if ("TEXT" === e) g = w(b, f), a = g.binLen, d = g.value; else if ("B64" === e) g = x(b), a = g.binLen, d = g.value; else if ("BYTES" === e) g = y(b), a = g.binLen, d = g.value; else throw "inputFormat must be HEX, TEXT, B64, or BYTES"; this.getHash = function (b, f, c, e) {
            var g = null,
			h = d.slice(), l = a, n; 3 === arguments.length ? "number" !== typeof c && (e = c, c = 1) : 2 === arguments.length && (c = 1); if (c !== parseInt(c, 10) || 1 > c) throw "numRounds must a integer >= 1"; switch (f) { case "HEX": g = z; break; case "B64": g = A; break; case "BYTES": g = B; break; default: throw "format must be HEX, B64, or BYTES"; } if ("SHA-1" === b) for (n = 0; n < c; n += 1) h = s(h, l), l = 160; else throw "Chosen SHA variant is not supported"; return g(h, C(e))
        }; this.getHMAC = function (c, b, e, g, q) {
            var h, l, n, r, p = [], t = []; h = null; switch (g) {
                case "HEX": g = z; break; case "B64": g =
				A; break; case "BYTES": g = B; break; default: throw "outputFormat must be HEX, B64, or BYTES";
            } if ("SHA-1" === e) l = 64, r = 160; else throw "Chosen SHA variant is not supported"; if ("HEX" === b) h = v(c), n = h.binLen, h = h.value; else if ("TEXT" === b) h = w(c, f), n = h.binLen, h = h.value; else if ("B64" === b) h = x(c), n = h.binLen, h = h.value; else if ("BYTES" === b) h = y(c), n = h.binLen, h = h.value; else throw "inputFormat must be HEX, TEXT, B64, or BYTES"; c = 8 * l; b = l / 4 - 1; if (l < n / 8) {
                if ("SHA-1" === e) h = s(h, n); else throw "Unexpected error in HMAC implementation";
                h[b] &= 4294967040
            } else l > n / 8 && (h[b] &= 4294967040); for (l = 0; l <= b; l += 1) p[l] = h[l] ^ 909522486, t[l] = h[l] ^ 1549556828; if ("SHA-1" === e) e = s(t.concat(s(p.concat(d), c + a)), c + r); else throw "Unexpected error in HMAC implementation"; return g(e, C(q))
        }
    } function w(b, e) {
        var c = [], a, d = [], f = 0, g; if ("UTF8" === e) for (g = 0; g < b.length; g += 1) for (a = b.charCodeAt(g), d = [], 128 > a ? d.push(a) : 2048 > a ? (d.push(192 | a >>> 6), d.push(128 | a & 63)) : 55296 > a || 57344 <= a ? d.push(224 | a >>> 12, 128 | a >>> 6 & 63, 128 | a & 63) : (g += 1, a = 65536 + ((a & 1023) << 10 | b.charCodeAt(g) & 1023),
		d.push(240 | a >>> 18, 128 | a >>> 12 & 63, 128 | a >>> 6 & 63, 128 | a & 63)), a = 0; a < d.length; a += 1) (f >>> 2) + 1 > c.length && c.push(0), c[f >>> 2] |= d[a] << 24 - f % 4 * 8, f += 1; else if ("UTF16" === e) for (g = 0; g < b.length; g += 1) (f >>> 2) + 1 > c.length && c.push(0), c[f >>> 2] |= b.charCodeAt(g) << 16 - f % 4 * 8, f += 2; return { value: c, binLen: 8 * f }
    } function v(b) {
        var e = [], c = b.length, a, d; if (0 !== c % 2) throw "String of HEX type must be in byte increments"; for (a = 0; a < c; a += 2) {
            d = parseInt(b.substr(a, 2), 16); if (isNaN(d)) throw "String of HEX type contains invalid characters"; e[a >>> 3] |=
			d << 24 - a % 8 * 4
        } return { value: e, binLen: 4 * c }
    } function y(b) { var e = [], c, a; for (a = 0; a < b.length; a += 1) c = b.charCodeAt(a), (a >>> 2) + 1 > e.length && e.push(0), e[a >>> 2] |= c << 24 - a % 4 * 8; return { value: e, binLen: 8 * b.length } } function x(b) {
        var e = [], c = 0, a, d, f, g, m; if (-1 === b.search(/^[a-zA-Z0-9=+\/]+$/)) throw "Invalid character in base-64 string"; a = b.indexOf("="); b = b.replace(/\=/g, ""); if (-1 !== a && a < b.length) throw "Invalid '=' found in base-64 string"; for (d = 0; d < b.length; d += 4) {
            m = b.substr(d, 4); for (f = g = 0; f < m.length; f += 1) a = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/".indexOf(m[f]),
			g |= a << 18 - 6 * f; for (f = 0; f < m.length - 1; f += 1) e[c >> 2] |= (g >>> 16 - 8 * f & 255) << 24 - c % 4 * 8, c += 1
        } return { value: e, binLen: 8 * c }
    } function z(b, e) { var c = "", a = 4 * b.length, d, f; for (d = 0; d < a; d += 1) f = b[d >>> 2] >>> 8 * (3 - d % 4), c += "0123456789abcdef".charAt(f >>> 4 & 15) + "0123456789abcdef".charAt(f & 15); return e.outputUpper ? c.toUpperCase() : c } function A(b, e) {
        var c = "", a = 4 * b.length, d, f, g; for (d = 0; d < a; d += 3) for (g = (b[d >>> 2] >>> 8 * (3 - d % 4) & 255) << 16 | (b[d + 1 >>> 2] >>> 8 * (3 - (d + 1) % 4) & 255) << 8 | b[d + 2 >>> 2] >>> 8 * (3 - (d + 2) % 4) & 255, f = 0; 4 > f; f += 1) c = 8 * d + 6 * f <= 32 * b.length ? c +
		"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/".charAt(g >>> 6 * (3 - f) & 63) : c + e.b64Pad; return c
    } function B(b) { var e = "", c = 4 * b.length, a, d; for (a = 0; a < c; a += 1) d = b[a >>> 2] >>> 8 * (3 - a % 4) & 255, e += String.fromCharCode(d); return e } function C(b) {
        var e = { outputUpper: !1, b64Pad: "=" }; try { b.hasOwnProperty("outputUpper") && (e.outputUpper = b.outputUpper), b.hasOwnProperty("b64Pad") && (e.b64Pad = b.b64Pad) } catch (c) { } if ("boolean" !== typeof e.outputUpper) throw "Invalid outputUpper formatting option"; if ("string" !== typeof e.b64Pad) throw "Invalid b64Pad formatting option";
        return e
    } function q(b, e) { return b << e | b >>> 32 - e } function r(b, e) { var c = (b & 65535) + (e & 65535); return ((b >>> 16) + (e >>> 16) + (c >>> 16) & 65535) << 16 | c & 65535 } function t(b, e, c, a, d) { var f = (b & 65535) + (e & 65535) + (c & 65535) + (a & 65535) + (d & 65535); return ((b >>> 16) + (e >>> 16) + (c >>> 16) + (a >>> 16) + (d >>> 16) + (f >>> 16) & 65535) << 16 | f & 65535 } function s(b, e) {
        var c = [], a, d, f, g, m, p, u, k, s, h = [1732584193, 4023233417, 2562383102, 271733878, 3285377520]; b[e >>> 5] |= 128 << 24 - e % 32; b[(e + 65 >>> 9 << 4) + 15] = e; s = b.length; for (u = 0; u < s; u += 16) {
            a = h[0]; d = h[1]; f = h[2]; g = h[3];
            m = h[4]; for (k = 0; 80 > k; k += 1) c[k] = 16 > k ? b[k + u] : q(c[k - 3] ^ c[k - 8] ^ c[k - 14] ^ c[k - 16], 1), p = 20 > k ? t(q(a, 5), d & f ^ ~d & g, m, 1518500249, c[k]) : 40 > k ? t(q(a, 5), d ^ f ^ g, m, 1859775393, c[k]) : 60 > k ? t(q(a, 5), d & f ^ d & g ^ f & g, m, 2400959708, c[k]) : t(q(a, 5), d ^ f ^ g, m, 3395469782, c[k]), m = g, g = f, f = q(d, 30), d = a, a = p; h[0] = r(a, h[0]); h[1] = r(d, h[1]); h[2] = r(f, h[2]); h[3] = r(g, h[3]); h[4] = r(m, h[4])
        } return h
    } "function" === typeof define && define.amd ? define(function () { return p }) : "undefined" !== typeof exports ? "undefined" !== typeof module && module.exports ? module.exports =
	exports = p : exports = p : D.jsSHA = p
})(this);
timename = setInterval(function () {
    if (error) {
        //alert("123");
        window.location.reload();
    }

}, 1000);
function getSign() {
    // alert("进方法getSign了");
    var sign = "";
    $.ajax({
        url: 'http://wx.tuhu.cn/home/gethdticket',
        type: "post",
        dataType: "json",
        data: {},
        async: false,
        success: function (data) {
            try{
                //alert("data:"+data);
                if (data["result"] == "1") {
                    // alert("ticket:"+data["ticket"])
                    var url = location.href.split("#")[0];
                    var nonce_str = noce; //getNonceStr().toString();
                    var time_stamp = timestampstring;//getTimeStamp().toString();
                    var keyvaluestring = "jsapi_ticket=" + data["ticket"] + "&noncestr=" + nonce_str + "&timestamp=" + time_stamp + "&url=" + url;
                    var s = new jsSHA(keyvaluestring, "TEXT"), t = s.getHash("SHA-1", "HEX");
                    sign = t;//hex_sha1(keyvaluestring).toString();
                    console.log(sign);
                }
            } catch (e) {
                console.log(e);
            }
        }
    });

    return sign;
}
wx.config({
    debug: false,
    appId: getAppId().toString(),
    timestamp: timestampstring,
    nonceStr: noce,
    signature: getSign().toString(),
    jsApiList: [
      'checkJsApi',
      'onMenuShareTimeline',
      'onMenuShareAppMessage',
      'onMenuShareQQ',
      'onMenuShareWeibo',
      'hideMenuItems',
      'showMenuItems',
      'hideAllNonBaseMenuItem',
      'showAllNonBaseMenuItem',
      'translateVoice',
      'startRecord',
      'stopRecord',
      'onRecordEnd',
      'playVoice',
      'pauseVoice',
      'stopVoice',
      'uploadVoice',
      'downloadVoice',
      'chooseImage',
      'previewImage',
      'uploadImage',
      'downloadImage',
      'getNetworkType',
      'openLocation',
      'getLocation',
      'hideOptionMenu',
      'showOptionMenu',
      'closeWindow',
      'scanQRCode',
      'chooseWXPay',
      'openProductSpecificView',
      'addCard',
      'chooseCard',
      'openCard'
    ]
});
if (!window.delayready) {
    /*
     * 注意：
     * 1. 所有的JS接口只能在公众号绑定的域名下调用，公众号开发者需要先登录微信公众平台进入“公众号设置”的“功能设置”里填写“JS接口安全域名”。
     * 2. 如果发现在 Android 不能分享自定义内容，请到官网下载最新的包覆盖安装，Android 自定义分享接口需升级至 6.0.2.58 版本及以上。
     * 3. 完整 JS-SDK 文档地址：http://mp.weixin.qq.com/wiki/7/aaa137b55fb2e0456bf8dd9148dd613f.html
     *
     * 如有问题请通过以下渠道反馈：
     * 邮箱地址：weixin-open@qq.com
     * 邮件主题：【微信JS-SDK反馈】具体问题
     * 邮件内容说明：用简明的语言描述问题所在，并交代清楚遇到该问题的场景，可附上截屏图片，微信团队会尽快处理你的反馈。
     */
    wx.ready(function () {
        // 1 判断当前版本是否支持指定 JS 接口，支持批量判断
        //document.querySelector('#checkJsApi').onclick = function () {
        wx.checkJsApi({
            jsApiList: [
              'onMenuShareTimeline',
              'onMenuShareAppMessage'
            ],
            success: function (res) {
                //alert(JSON.stringify(res));
            }
        });
        wx.onMenuShareTimeline({
            title: desc,// 分享标题
            link: link, // 分享链接
            imgUrl: imgurlShare, // 分享图标
            success: function () {
                //alert("asd");// 用户确认分享后执行的回调函数
                if (typeof UpdateGuessNextActive != 'undefined' && UpdateGuessNextActive instanceof Function) {
                    UpdateGuessNextActive();
                }
            },
            cancel: function () {
                //alert("asd2");// 用户取消分享后执行的回调函数
            }
        });
        wx.onMenuShareAppMessage({
            title: title, // 分享标题
            desc: desc, // 分享描述
            link: link, // 分享链接
            imgUrl: imgurl, // 分享图标
            type: '', // 分享类型,music、video或link，不填默认为link
            dataUrl: '', // 如果type是music或video，则要提供数据链接，默认为空
            success: function () {
                // 用户确认分享后执行的回调函数
                if (typeof UpdateGuessNextActive != 'undefined' && UpdateGuessNextActive instanceof Function) {
                    UpdateGuessNextActive();
                }
            },
            cancel: function () {
                // 用户取消分享后执行的回调函数
            }
        });
        // };

    });
}
wx.error(function (res) {
    //error = true;
    //alert("error:出错了" + res.errMsg);
});
//function UpdateGuessNextActive()
//{
//    if ($("#word").length > 0) {
//        var id = $("#word").attr("data-id");
//        if (id == null || id == "" || id < 1) {
//            return;
//        }
//        $.post("/Active/ShareData", { id: id }, function (data) {
//            console.log(data);
//        });
//    }
//}
//$(document).ready(function ()
//{
//	$("#jsapiSignForm").submit(function ()
//	{
//		var e = decodeURIComponent($("#jsapiSignForm").serialize()).split("#")[0], s = new jsSHA(e, "TEXT"), t = s.getHash("SHA-1", "HEX");
//		return $("#string1").text(e), $("#signature").text(t), !1;
//	}), $("#jsapi_ticket").change(function ()
//	{
//		var e = $("#jsapi_ticket").val(), s = $("#jsapi_ticket").parent().parent(), t = $("#jsapi_ticket_msg");
//		$.ajax({
//			url: "/debug/cgi-bin/ticket/check?ticket=" + e,
//			success: function (e)
//			{
//				0 === e.errcode ? (s.removeClass("has-error"), s.addClass("has-success"), t.text("")) : 1 === e.errcode ? (s.removeClass("has-success"),
//				s.addClass("has-error"), t.text("jsapi_ticket 已过期")) : -1 === e.errcode && (s.removeClass("has-success"),
//				s.addClass("has-error"), t.text("不是合法的 jsapi_ticket"));
//			}
//		});
//	});
//});


/*
 A JavaScript implementation of the SHA family of hashes, as
 defined in FIPS PUB 180-2 as well as the corresponding HMAC implementation
 as defined in FIPS PUB 198a

 Copyright Brian Turek 2008-2014
 Distributed under the BSD License
 See http://caligatio.github.com/jsSHA/ for more information

 Several functions taken from Paul Johnston
*/

