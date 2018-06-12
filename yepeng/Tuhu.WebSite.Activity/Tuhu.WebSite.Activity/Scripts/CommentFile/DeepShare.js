!function (a, b) { "function" == typeof define && (define.amd || define.cmd) ? define(function () { return b(a) }) : b(a, !0) }(this, function (a, b) { function c(b, c, d) { a.WeixinJSBridge ? WeixinJSBridge.invoke(b, e(c), function (a) { g(b, a, d) }) : j(b, d) } function d(b, c, d) { a.WeixinJSBridge ? WeixinJSBridge.on(b, function (a) { d && d.trigger && d.trigger(a), g(b, a, c) }) : d ? j(b, d) : j(b, c) } function e(a) { return a = a || {}, a.appId = E.appId, a.verifyAppId = E.appId, a.verifySignType = "sha1", a.verifyTimestamp = E.timestamp + "", a.verifyNonceStr = E.nonceStr, a.verifySignature = E.signature, a } function f(a) { return { timeStamp: a.timestamp + "", nonceStr: a.nonceStr, "package": a.package, paySign: a.paySign, signType: a.signType || "SHA1" } } function g(a, b, c) { var d, e, f; switch (delete b.err_code, delete b.err_desc, delete b.err_detail, d = b.errMsg, d || (d = b.err_msg, delete b.err_msg, d = h(a, d), b.errMsg = d), c = c || {}, c._complete && (c._complete(b), delete c._complete), d = b.errMsg || "", E.debug && !c.isInnerInvoke && alert(JSON.stringify(b)), e = d.indexOf(":"), f = d.substring(e + 1)) { case "ok": c.success && c.success(b); break; case "cancel": c.cancel && c.cancel(b); break; default: c.fail && c.fail(b) } c.complete && c.complete(b) } function h(a, b) { var e, f, c = a, d = p[c]; return d && (c = d), e = "ok", b && (f = b.indexOf(":"), e = b.substring(f + 1), "confirm" == e && (e = "ok"), "failed" == e && (e = "fail"), -1 != e.indexOf("failed_") && (e = e.substring(7)), -1 != e.indexOf("fail_") && (e = e.substring(5)), e = e.replace(/_/g, " "), e = e.toLowerCase(), ("access denied" == e || "no permission to execute" == e) && (e = "permission denied"), "config" == c && "function not exist" == e && (e = "ok"), "" == e && (e = "fail")), b = c + ":" + e } function i(a) { var b, c, d, e; if (a) { for (b = 0, c = a.length; c > b; ++b) d = a[b], e = o[d], e && (a[b] = e); return a } } function j(a, b) { if (!(!E.debug || b && b.isInnerInvoke)) { var c = p[a]; c && (a = c), b && b._complete && delete b._complete, console.log('"' + a + '",', b || "") } } function k() { 0 != D.preVerifyState && (u || v || E.debug || "6.0.2" > z || D.systemType < 0 || A || (A = !0, D.appId = E.appId, D.initTime = C.initEndTime - C.initStartTime, D.preVerifyTime = C.preVerifyEndTime - C.preVerifyStartTime, H.getNetworkType({ isInnerInvoke: !0, success: function (a) { var b, c; D.networkType = a.networkType, b = "http://open.weixin.qq.com/sdk/report?v=" + D.version + "&o=" + D.preVerifyState + "&s=" + D.systemType + "&c=" + D.clientVersion + "&a=" + D.appId + "&n=" + D.networkType + "&i=" + D.initTime + "&p=" + D.preVerifyTime + "&u=" + D.url, c = new Image, c.src = b } }))) } function l() { return (new Date).getTime() } function m(b) { w && (a.WeixinJSBridge ? b() : q.addEventListener && q.addEventListener("WeixinJSBridgeReady", b, !1)) } function n() { H.invoke || (H.invoke = function (b, c, d) { a.WeixinJSBridge && WeixinJSBridge.invoke(b, e(c), d) }, H.on = function (b, c) { a.WeixinJSBridge && WeixinJSBridge.on(b, c) }) } var o, p, q, r, s, t, u, v, w, x, y, z, A, B, C, D, E, F, G, H; if (!a.jWeixin) return o = { config: "preVerifyJSAPI", onMenuShareTimeline: "menu:share:timeline", onMenuShareAppMessage: "menu:share:appmessage", onMenuShareQQ: "menu:share:qq", onMenuShareWeibo: "menu:share:weiboApp", onMenuShareQZone: "menu:share:QZone", previewImage: "imagePreview", getLocation: "geoLocation", openProductSpecificView: "openProductViewWithPid", addCard: "batchAddCard", openCard: "batchViewCard", chooseWXPay: "getBrandWCPayRequest" }, p = function () { var b, a = {}; for (b in o) a[o[b]] = b; return a }(), q = a.document, r = q.title, s = navigator.userAgent.toLowerCase(), t = navigator.platform.toLowerCase(), u = !(!t.match("mac") && !t.match("win")), v = -1 != s.indexOf("wxdebugger"), w = -1 != s.indexOf("micromessenger"), x = -1 != s.indexOf("android"), y = -1 != s.indexOf("iphone") || -1 != s.indexOf("ipad"), z = function () { var a = s.match(/micromessenger\/(\d+\.\d+\.\d+)/) || s.match(/micromessenger\/(\d+\.\d+)/); return a ? a[1] : "" }(), A = !1, B = !1, C = { initStartTime: l(), initEndTime: 0, preVerifyStartTime: 0, preVerifyEndTime: 0 }, D = { version: 1, appId: "", initTime: 0, preVerifyTime: 0, networkType: "", preVerifyState: 1, systemType: y ? 1 : x ? 2 : -1, clientVersion: z, url: encodeURIComponent(location.href) }, E = {}, F = { _completes: [] }, G = { state: 0, data: {} }, m(function () { C.initEndTime = l() }), H = { config: function (a) { E = a, j("config", a); var b = E.check === !1 ? !1 : !0; m(function () { var a, d, e; if (b) c(o.config, { verifyJsApiList: i(E.jsApiList) }, function () { F._complete = function (a) { C.preVerifyEndTime = l(), G.state = 1, G.data = a }, F.success = function () { D.preVerifyState = 0 }, F.fail = function (a) { F._fail ? F._fail(a) : G.state = -1 }; var a = F._completes; return a.push(function () { k() }), F.complete = function () { for (var c = 0, d = a.length; d > c; ++c) a[c](); F._completes = [] }, F }()), C.preVerifyStartTime = l(); else { for (G.state = 1, a = F._completes, d = 0, e = a.length; e > d; ++d) a[d](); F._completes = [] } }), E.beta && n() }, ready: function (a) { 0 != G.state ? a() : (F._completes.push(a), !w && E.debug && a()) }, error: function (a) { "6.0.2" > z || B || (B = !0, -1 == G.state ? a(G.data) : F._fail = a) }, checkJsApi: function (a) { var b = function (a) { var c, d, b = a.checkResult; for (c in b) d = p[c], d && (b[d] = b[c], delete b[c]); return a }; c("checkJsApi", { jsApiList: i(a.jsApiList) }, function () { return a._complete = function (a) { if (x) { var c = a.checkResult; c && (a.checkResult = JSON.parse(c)) } a = b(a) }, a }()) }, onMenuShareTimeline: function (a) { d(o.onMenuShareTimeline, { complete: function () { c("shareTimeline", { title: a.title || r, desc: a.title || r, img_url: a.imgUrl || "", link: a.link || location.href, type: a.type || "link", data_url: a.dataUrl || "" }, a) } }, a) }, onMenuShareAppMessage: function (a) { d(o.onMenuShareAppMessage, { complete: function () { c("sendAppMessage", { title: a.title || r, desc: a.desc || "", link: a.link || location.href, img_url: a.imgUrl || "", type: a.type || "link", data_url: a.dataUrl || "" }, a) } }, a) }, onMenuShareQQ: function (a) { d(o.onMenuShareQQ, { complete: function () { c("shareQQ", { title: a.title || r, desc: a.desc || "", img_url: a.imgUrl || "", link: a.link || location.href }, a) } }, a) }, onMenuShareWeibo: function (a) { d(o.onMenuShareWeibo, { complete: function () { c("shareWeiboApp", { title: a.title || r, desc: a.desc || "", img_url: a.imgUrl || "", link: a.link || location.href }, a) } }, a) }, onMenuShareQZone: function (a) { d(o.onMenuShareQZone, { complete: function () { c("shareQZone", { title: a.title || r, desc: a.desc || "", img_url: a.imgUrl || "", link: a.link || location.href }, a) } }, a) }, startRecord: function (a) { c("startRecord", {}, a) }, stopRecord: function (a) { c("stopRecord", {}, a) }, onVoiceRecordEnd: function (a) { d("onVoiceRecordEnd", a) }, playVoice: function (a) { c("playVoice", { localId: a.localId }, a) }, pauseVoice: function (a) { c("pauseVoice", { localId: a.localId }, a) }, stopVoice: function (a) { c("stopVoice", { localId: a.localId }, a) }, onVoicePlayEnd: function (a) { d("onVoicePlayEnd", a) }, uploadVoice: function (a) { c("uploadVoice", { localId: a.localId, isShowProgressTips: 0 == a.isShowProgressTips ? 0 : 1 }, a) }, downloadVoice: function (a) { c("downloadVoice", { serverId: a.serverId, isShowProgressTips: 0 == a.isShowProgressTips ? 0 : 1 }, a) }, translateVoice: function (a) { c("translateVoice", { localId: a.localId, isShowProgressTips: 0 == a.isShowProgressTips ? 0 : 1 }, a) }, chooseImage: function (a) { c("chooseImage", { scene: "1|2", count: a.count || 9, sizeType: a.sizeType || ["original", "compressed"], sourceType: a.sourceType || ["album", "camera"] }, function () { return a._complete = function (a) { if (x) { var b = a.localIds; b && (a.localIds = JSON.parse(b)) } }, a }()) }, previewImage: function (a) { c(o.previewImage, { current: a.current, urls: a.urls }, a) }, uploadImage: function (a) { c("uploadImage", { localId: a.localId, isShowProgressTips: 0 == a.isShowProgressTips ? 0 : 1 }, a) }, downloadImage: function (a) { c("downloadImage", { serverId: a.serverId, isShowProgressTips: 0 == a.isShowProgressTips ? 0 : 1 }, a) }, getNetworkType: function (a) { var b = function (a) { var c, d, e, b = a.errMsg; if (a.errMsg = "getNetworkType:ok", c = a.subtype, delete a.subtype, c) a.networkType = c; else switch (d = b.indexOf(":"), e = b.substring(d + 1)) { case "wifi": case "edge": case "wwan": a.networkType = e; break; default: a.errMsg = "getNetworkType:fail" } return a }; c("getNetworkType", {}, function () { return a._complete = function (a) { a = b(a) }, a }()) }, openLocation: function (a) { c("openLocation", { latitude: a.latitude, longitude: a.longitude, name: a.name || "", address: a.address || "", scale: a.scale || 28, infoUrl: a.infoUrl || "" }, a) }, getLocation: function (a) { a = a || {}, c(o.getLocation, { type: a.type || "wgs84" }, function () { return a._complete = function (a) { delete a.type }, a }()) }, hideOptionMenu: function (a) { c("hideOptionMenu", {}, a) }, showOptionMenu: function (a) { c("showOptionMenu", {}, a) }, closeWindow: function (a) { a = a || {}, c("closeWindow", {}, a) }, hideMenuItems: function (a) { c("hideMenuItems", { menuList: a.menuList }, a) }, showMenuItems: function (a) { c("showMenuItems", { menuList: a.menuList }, a) }, hideAllNonBaseMenuItem: function (a) { c("hideAllNonBaseMenuItem", {}, a) }, showAllNonBaseMenuItem: function (a) { c("showAllNonBaseMenuItem", {}, a) }, scanQRCode: function (a) { a = a || {}, c("scanQRCode", { needResult: a.needResult || 0, scanType: a.scanType || ["qrCode", "barCode"] }, function () { return a._complete = function (a) { var b, c; y && (b = a.resultStr, b && (c = JSON.parse(b), a.resultStr = c && c.scan_code && c.scan_code.scan_result)) }, a }()) }, openProductSpecificView: function (a) { c(o.openProductSpecificView, { pid: a.productId, view_type: a.viewType || 0, ext_info: a.extInfo }, a) }, addCard: function (a) { var e, f, g, h, b = a.cardList, d = []; for (e = 0, f = b.length; f > e; ++e) g = b[e], h = { card_id: g.cardId, card_ext: g.cardExt }, d.push(h); c(o.addCard, { card_list: d }, function () { return a._complete = function (a) { var c, d, e, b = a.card_list; if (b) { for (b = JSON.parse(b), c = 0, d = b.length; d > c; ++c) e = b[c], e.cardId = e.card_id, e.cardExt = e.card_ext, e.isSuccess = e.is_succ ? !0 : !1, delete e.card_id, delete e.card_ext, delete e.is_succ; a.cardList = b, delete a.card_list } }, a }()) }, chooseCard: function (a) { c("chooseCard", { app_id: E.appId, location_id: a.shopId || "", sign_type: a.signType || "SHA1", card_id: a.cardId || "", card_type: a.cardType || "", card_sign: a.cardSign, time_stamp: a.timestamp + "", nonce_str: a.nonceStr }, function () { return a._complete = function (a) { a.cardList = a.choose_card_info, delete a.choose_card_info }, a }()) }, openCard: function (a) { var e, f, g, h, b = a.cardList, d = []; for (e = 0, f = b.length; f > e; ++e) g = b[e], h = { card_id: g.cardId, code: g.code }, d.push(h); c(o.openCard, { card_list: d }, a) }, chooseWXPay: function (a) { c(o.chooseWXPay, f(a), a) } }, b && (a.wx = a.jWeixin = H), H });

function Base64() {
    // private property
    _keyStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
    // public method for encoding
    this.encode = function (input) {
        var output = "";
        var chr1, chr2, chr3, enc1, enc2, enc3, enc4;
        var i = 0;
        input = _utf8_encode(input);
        while (i < input.length) {
            chr1 = input.charCodeAt(i++);
            chr2 = input.charCodeAt(i++);
            chr3 = input.charCodeAt(i++);
            enc1 = chr1 >> 2;
            enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
            enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
            enc4 = chr3 & 63;
            if (isNaN(chr2)) {
                enc3 = enc4 = 64;
            } else if (isNaN(chr3)) {
                enc4 = 64;
            }
            output = output +
            _keyStr.charAt(enc1) + _keyStr.charAt(enc2) +
            _keyStr.charAt(enc3) + _keyStr.charAt(enc4);
        }
        return output;
    }

    // public method for decoding
    this.decode = function (input) {
        var output = "";
        var chr1, chr2, chr3;
        var enc1, enc2, enc3, enc4;
        var i = 0;
        input = input.replace(/[^A-Za-z0-9\+\/\=]/g, "");
        while (i < input.length) {
            enc1 = _keyStr.indexOf(input.charAt(i++));
            enc2 = _keyStr.indexOf(input.charAt(i++));
            enc3 = _keyStr.indexOf(input.charAt(i++));
            enc4 = _keyStr.indexOf(input.charAt(i++));
            chr1 = (enc1 << 2) | (enc2 >> 4);
            chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
            chr3 = ((enc3 & 3) << 6) | enc4;
            output = output + String.fromCharCode(chr1);
            if (enc3 != 64) {
                output = output + String.fromCharCode(chr2);
            }
            if (enc4 != 64) {
                output = output + String.fromCharCode(chr3);
            }
        }
        output = _utf8_decode(output);
        return output;
    }

    // private method for UTF-8 encoding
    _utf8_encode = function (string) {
        string = string.replace(/\r\n/g, "\n");
        var utftext = "";
        for (var n = 0; n < string.length; n++) {
            var c = string.charCodeAt(n);
            if (c < 128) {
                utftext += String.fromCharCode(c);
            } else if ((c > 127) && (c < 2048)) {
                utftext += String.fromCharCode((c >> 6) | 192);
                utftext += String.fromCharCode((c & 63) | 128);
            } else {
                utftext += String.fromCharCode((c >> 12) | 224);
                utftext += String.fromCharCode(((c >> 6) & 63) | 128);
                utftext += String.fromCharCode((c & 63) | 128);
            }

        }
        return utftext;
    }
    // private method for UTF-8 decoding
    _utf8_decode = function (utftext) {
        var string = "";
        var i = 0;
        var c = c1 = c2 = 0;
        while (i < utftext.length) {
            c = utftext.charCodeAt(i);
            if (c < 128) {
                string += String.fromCharCode(c);
                i++;
            } else if ((c > 191) && (c < 224)) {
                c2 = utftext.charCodeAt(i + 1);
                string += String.fromCharCode(((c & 31) << 6) | (c2 & 63));
                i += 2;
            } else {
                c2 = utftext.charCodeAt(i + 1);
                c3 = utftext.charCodeAt(i + 2);
                string += String.fromCharCode(((c & 15) << 12) | ((c2 & 63) << 6) | (c3 & 63));
                i += 3;
            }
        }
        return string;
    }
}


//=================================DeepShare 打开App End====================================//
var APPShare = {
    Data: "",
    SenderID: "",
    Title: "途虎养车-1分钱洗车",
    Msg: "一分钱洗车，换轮胎、做保养、查违章、汽车美容",
    GetQueryString: function (name) {
        ///<summary>获取页面参数</summary>
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
        var r = window.location.search.substr(1).match(reg);
        if (r != null)
            return unescape(r[2]).toLocaleLowerCase() != "tuhu" ? unescape(r[2]) : null;
        return null;
    },
    base64encode: function (input) {
        var b = new Base64();
        return b.encode(input);
    },
    base64decode: function (input) {
        var b = new Base64();
        return b.decode(input);
    },
    close: function (obj) {
        $(obj).parent().animate({
            'top': '-' + $('.load-tip').height() + 'px'
        }, 800);
        $('.banner').animate({
            'margin-top': '0px'
        }, 400);
        $(window).scroll(function () {
            $(obj).parent().hide();
        });
        return false;
    },
    Init: function () {
        var source=APPShare.GetQueryString("utm_source");
        //判断参数
        if (source != "" && source != undefined) {
            if (source != "Sync") {
                var downHtml = "<div class='load-tip'> <span class='tuhu-logo'><img src='/Content/imgs/detail/drawable-hdpi/hu.png'></span><span class='tuhu-title'>途虎养车</span><span class='load-dele' onclick='APPShare.close(this)'><img src='/Content/imgs/detail/drawable-hdpi/close.png'></span><span class='load-tap' onclick='APPShare.DownApp()'>立即打开</span></div>";
                $("section.boxContent").prepend(downHtml);

                $(".pageCategories").empty();//不显示标签
            }

            //if ($(".boxContent").children().hasClass("imgBox")) {
            //    $(".imgBox").addClass("banner");
            //}
            if ($(".boxContent").children(".imgBox").hasClass("banner")) {
                $(".boxContent").children(".imgBox").addClass("martop44");
            } else if ($(".boxContent").children(".imgBox").hasClass("theme")) {
                $(".boxContent").children(".imgBox").addClass("martop48");
            }
            //$("footer.footer").hide();//非app端不显示收藏和分享
            $(".attend-argu").show();//显示评论
            //相关阅读链接加source
            $("#xgyd").find(".relateds").each(function () {
                var thisObj = $(this);
                var link = thisObj.attr("href") + "&utm_source=" + source;
                thisObj.attr("href", link);
            });


            APPShare.SenderID = APPShare.GetQueryString("senderid");
            if (APPShare.SenderID == undefined || APPShare.SenderID == "") {
                APPShare.SenderID = "SenderID";
            }
            APPShare.Title = "下载途虎APP领百元红包";
            APPShare.Msg = "新用户还有一分钱洗车哟";

            var type = APPShare.GetQueryString("type");//andorid  2  IOS没有type

            var articleId = APPShare.GetQueryString("id");
            var articleUrl = location.href.split("?")[0] + "?id=" + articleId + "&bgColor=%2523ffffff&textColor=%2523333333";
            var data = {};
            data.Type = 2;
            data.URL = articleUrl;
            data.shareUrl = location.href;
            data.shareTitle = document.title;
            data.shareDescription = "";
            data.sharePicture = "";
            data.isShowShareIcon = false;
            APPShare.Data = '{"data":"' + APPShare.base64encode(JSON.stringify(data)) + '"}';

            APPShare.GetToUrl();
        }
    },
    DownApp: function () {
        if (window.GoToUrl) {
            //ga埋点
            ga('send', 'event', 'toutiao_detail', 'click_open', 'open');
            Ta.Run("home_click_打开app", "event");
            location.href = window.GoToUrl;
        }
    },
    OpenAppComment:function(){
        if (window.GoToUrl) {
            ga('send', 'event', 'toutiao_detail', 'click_joincomment');
            Ta.Run("home_click_参与头条评论", "event");
            location.href = window.GoToUrl;
        }
    },
    GetToUrl: function () {
        APPShare.Url = location.href;
        var params = {
            inapp_data: APPShare.Data,
            sender_id: APPShare.SenderID,
            download_title: APPShare.Title,
            download_msg: APPShare.Msg
        };
        var paramsString = JSON.stringify(params);
        $.ajax({
            url: 'https://fds.so/v2/url/b432168cf0b44c7e',
            type: 'POST',
            data: paramsString,
            xhrFields: { withCredentials: true, },
            success: function (result) {
                if (result) {
                    window.GoToUrl = result.url;
                }
            },
            error: function (result) {
                //出错情况下可以继续用户正常逻辑，可设置一个默认url.
            }
        });
    }
}

//初始化
APPShare.Init();
