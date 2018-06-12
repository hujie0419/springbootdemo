//=================================万能跳转 Begin==================================//
//使用此跳转需要添加一下插件
// <script src="http://resource.tuhu.cn/Js/WebViewJavascriptBridge.js" type="text/javascript"></script>
//
var domain = 'http://resource.tuhu.cn';
var userAgentInfo = navigator.userAgent;
var Agents = new Array("iPhone", "iPad", "iPod");
var flag = true;
var isIosVersion = false;
var IosVersionNumber = false;
var isAndroidVersion = false;
var AndroidVersionNumber = false;
var GetAndroidVersion, GetIosVersion; //获取app端版本号
var IsWxForIosApp = false;  //客户端是否安装微信客户端,0否，1是
var isAndroidOpen = true;
for (var v = 0; v < Agents.length; v++) {
    if (userAgentInfo.indexOf(Agents[v]) > 0) { flag = false; break; }
}
function connectWebViewJavascriptBridge(callback) {
    if (window.WebViewJavascriptBridge) {
        callback(WebViewJavascriptBridge)
    } else {
        document.addEventListener(
            'WebViewJavascriptBridgeReady'
            , function () {
                callback(WebViewJavascriptBridge)
            },
            false
        );
    }
}
connectWebViewJavascriptBridge(function (bridge) {
    bridge.init(function (message, responseCallback) {
        var data = {
            'Javascript Responds': 'Wee!'
        };
        responseCallback(data);
    });
    bridge.registerHandler("functionInJs", function (data, responseCallback) {
        var responseData = "Javascript Says Right back aka!";
        responseCallback(responseData);
    });
})
//--------------------------------IOS，Android 版本判断
function VersionForIos(version) {
    isIosVersion = true;
    version = parseInt(version.replace(/IOS/ig, "").replace(/\./ig, ""));//2.9.0
    GetIosVersion = version;
    if (version >= 290) {
        IosVersionNumber = true;
        NewsDataApi.SelectNewsInfo();
        NewsDataApi.IsNewsForLikeNews();
    }
    else {
        IosVersionNumber = false;
    }
    //隐藏分享功能
    if (version < 300) {
        $("#haoyou").hide();
        $("#penyouquan").hide();
    }
}
function VersionForAndroid(version) {
    isAndroidVersion = true;
    version = parseInt(version.replace(/\./ig, ""));//4.5.2
    GetAndroidVersion = version;
    if (version >= 450) {
        AndroidVersionNumber = true;
        NewsDataApi.SelectNewsInfo();
        NewsDataApi.IsNewsForLikeNews();
    }
    else {
        AndroidVersionNumber = false;
    }
    //隐藏分享功能
    if (version < 460) {
        $("#haoyou").hide();
        $("#penyouquan").hide();
    }
}
//--------------------------------IOS 判断是否有微信客户端 0 否,1 是
function IsWxForIos(n) {
    IsWxForIosApp = ((n == 1 || 0) ? true : false);
    WeiXinShareApi.WxShareShowOrHide(IsWxForIosApp);
}
//--------------------------------全局替换
String.prototype.ReplaceAll = function (reallyDo, replaceWith, ignoreCase) {
    if (!RegExp.prototype.isPrototypeOf(reallyDo)) {
        return this.replace(new RegExp(reallyDo, (ignoreCase ? "gi" : "g")), replaceWith);
    } else {
        return this.replace(reallyDo, replaceWith);
    }
}
//--------------------------------记录跳转数据
function skipAddress(currentAddress, productId, type) {
    try {
        var articleId = currentAddress.substring((currentAddress.lastIndexOf("_") + 1));
        articleId = articleId.substring(0, articleId.lastIndexOf("."));
        $.ajax({
            type: "POST",
            url: "http://faxian.tuhu.cn/ArticleNewList/AddArticleNewList?",
            data: {
                "ArticleId": articleId, "ArticleUrl": currentAddress, "ProductId": productId, "Type": type
            },
            success: function (result) {
            }
        });
    }
    catch (e) {
    }
}
//--------------------------------时间格式转换
function getNewDay(dateTemp, days) {
    var dateTemp = dateTemp.split("-");
    var nDate = new Date(dateTemp[1] + '-' + dateTemp[2] + '-' + dateTemp[0]); //转换为MM-DD-YYYY格式
    var millSeconds = Math.abs(nDate) + (days * 24 * 60 * 60 * 1000);
    var rDate = new Date(millSeconds);
    var year = rDate.getFullYear();
    var month = rDate.getMonth() + 1;
    if (month < 10) month = "0" + month;
    var date = rDate.getDate();
    if (date < 10) date = "0" + date;
    return (year + "-" + month + "-" + date);
}
function getPhone(cb, state) {
    if (userAgentInfo.indexOf("Android") >= 0) {
        window.WebViewJavascriptBridge.callHandler(
            'actityBridge'
            , { 'state': state }
            , function (responseData) {
                if (responseData != null) {
                    try {
                        var da = JSON.parse(responseData);
                        if (window[cb] != null) {
                            window[cb](da);
                        }
                    } catch (e) {
                        // alert(e);
                    }
                }
            });
    } else {
        sendCommand("Userinfologin", cb);
    }
}
function getPhoneNotToLogin(cb) {
    if (userAgentInfo.indexOf("Android") >= 0) {
        window.WebViewJavascriptBridge.callHandler(
            'toLoginBridge'
            , { 'param': "" }
            , function (responseData) {
                if (responseData != null) {
                    try {
                        var da = JSON.parse(responseData);
                        if (window[cb] != null) {
                            window[cb](da);
                        }
                    } catch (e) {
                        //alert(e);
                    }
                }
            });
    } else {
        sendCommand("UserinfologinNotToLogin", cb);
    }
}
function setPhone(da) {
    phone = da.phone;
    //alert(phone);
    // alert(getCookie(phone + pakid));
    if (getCookie(phone + pakid) != null) {
        var jsonVoteMode = JSON.parse(getCookie(phone + pakid));//获取是否点赞的cookie
        if (jsonVoteMode.resultState == 1) {//说明是已点赞状态
            $("#evaluate").attr("class", "evaluateaa");//红色
        }
        else if (jsonVoteMode.resultState == 0) {//未点赞状态
            $("#evaluate").attr("class", "evaluate");//百色
        }
    }
    else {//cookie为null
        $("#evaluate").attr("class", "evaluate");//百色
    }
}
//--------------------------------获取cookie
function getCookie(name) {
    var arr = document.cookie.match(new RegExp("(^| )" + name + "=([^;]*)(;|$)"));
    if (arr != null) return unescape(arr[2]); return null;
}
//--------------------------------设置cookie
function SetCookie(name, value) {
    var Days = 3600;
    var exp = new Date();
    //  domain = "tuhu.cn";
    exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);
    document.cookie = name + "=" + escape(value) + ";path=/;expires=" + exp.toGMTString();
}
//--------------------------------删除ookie
function delCookie(name) {
    var exp = new Date();
    exp.setTime(exp.getTime() - 1);
    var cval = getCookie(name);
    if (cval != null)
        document.cookie = name + "=" + cval + ";expires=" + exp.toGMTString() + "; path=/";
}
//--------------------------------滑动效果
//window.onload = function () {
//    $("#wrap").css("top", $("#banner").height());
//    $("#banner").css('position', 'fixed');
//}
//--------------------------------图片滚动效果
var swiperApi = {
    invoke: function () {
        if (InitNewsApi.IsWeiXin() == false) {
            new Swiper('.swiper-container', {
                slidesPerView: 'auto',
                spaceBetween: 10
            });
        }
    }
}
//=================================移动交互相关 Begin==================================//
function InitContentUrlForVal() {
    var _protocol = window.location.protocol + "//";
    var _host = window.location.host;
    var _pathname = window.location.pathname;
    var url = _protocol + _host + _pathname;
    return url;
}
var _PKID = $("#PKID").val();
var _BigTitle = $("#BigTitle").val();
var _CategoryId = $("#Category").val();
var _ContentUrl = $("#ContentUrl").val(InitContentUrlForVal()).val();
var _CategoryName = $("#CategoryName").val();
var _CategoryTags = $("#CategoryTags").val();
var _UserId, _Phone, _ProvinceName;
var _callbackForLoginApi, _newsLikeState;
var Uuid = "";
//----------------------------相关文章模型
var THDiscoverModel = function () {
    this.TitleColor,
    this.Category,
    this.CategoryName,
    this.ContentUrl,
    this.SmallImage,
    this.CommentNum,
    this.PKID,
    this.SmallTitle,
    this.Heat,
    this.ClickCount,
    this.BigTitle,
    this.Vote,
    this.Image,
    this.PublishDateTime,
    this.Brief
}
//----------------------------android，ios 登陆获取用户信息
var LoginApi = {
    Login: function (callback) {
        _callbackForLoginApi = callback;
        if (userAgentInfo.indexOf("Android") >= 0) {
            LoginApi.Android();
        }
        else {
            LoginApi.IOS("Userinfologin", "LoginApi.CallBackAppResponse");
        }
    },
    Android: function () {
        window.WebViewJavascriptBridge.callHandler(
        'actityBridge'
        , { 'param': "" }
        , function (responseData) {
            if (responseData != null) {
                LoginApi.CallBackAppResponse(responseData);
            }
        });
    },
    IOS: function (cmd, arg) {
        location.href = '#testapp#' + cmd + '#' + encodeURIComponent(arg);
    },
    CallBackAppResponse: function (responseData) {
        var da = null;
        if (userAgentInfo.indexOf("Android") >= 0) {
            da = JSON.parse(responseData);
            _ProvinceName = da.Province;
        }
        else {
            da = responseData;
            _ProvinceName = da.province;
        }
        _UserId = da.userid;
        _Phone = da.phone;
        Uuid = da.uuid;
        SetCookie("UserId", _UserId);
        SetCookie("Phone", _Phone);
        _callbackForLoginApi();
    }
}
//--------------------------图片查看器
var PhotoView = {
    appendView: function () {
        //遍历所有文章产品内容图片
        $(".section  .de-page").each(function (i) {
            $(this).find("img").each(function (i) {
                var imgObj=$(this);
                var src = imgObj.attr("src") || "";
                if (src.length > 0 || src.toLocaleLowerCase().indexOf("http") >= 0) {
                    $(this).attr("class", "cover");
                }
            });
        });

        //遍历所有文章产品上传图片
        $(".cover").each(function (index, el) {
            var src = $(el).attr("src") || "";
            if (src.length > 0 && src.toLowerCase().indexOf("http") >= 0) {
                console.log(index + "--mm");
                $("#imgList ul").append("<li class='swiper-slide'><img src='" + src + "' style='width:" + $(document).width() + "px;'></li>")
            }
            else {
                $(el).remove();
            }
        });
    },
    ShowView: function () {
        $(".cover").on("click", function (i) {
            console.log(i);
            console.log($(".cover").index(this));
            $("#dialog").show();
            var swiper = new Swiper('#imgList', {
                initialSlide: $(".cover").index(this)
            });
        });
    },
    HideView: function () {
        $("#dialog").on("click", function () {
            if ($(this).css("display") == "block") {
                $(this).hide();
                $('#imgList ul').css('transform', 'translate3d(0px, 0px, 0px)');
            }
        });
    },
    Invoke: function () {
        setTimeout(function () {
            PhotoView.appendView();
            PhotoView.ShowView();
            PhotoView.HideView();
        }, 3000);
    }
}
//----------------------------Androdi,IOS 跳转文章类目标签
var toDiscoveryListApi = {
    Android: function (CategoryId) {
        window.WebViewJavascriptBridge.callHandler(
       'toDiscoveryList'
           , {
               'CategoryId': CategoryId
           }
       , function (responseData) { })
    },
    IOS: function (CategoryId) {
        var prams = {
            "category": CategoryId
        };
        window.location.href = "tuhuDiscover://segue#THDiscoverTableVC#" + encodeURIComponent(JSON.stringify(prams));
    },
    Invoke: function (CategoryTagKey, CategoryTagName) {
        if (IosVersionNumber == true || AndroidVersionNumber == true) {
            //仅当为指定版本范围内方可调用
            if (userAgentInfo.indexOf("Android") >= 0) {
                if (AndroidVersionNumber) {
                    toDiscoveryListApi.Android(_CategoryId);
                }
            }
            else {
                if (IosVersionNumber) {
                    toDiscoveryListApi.IOS(_CategoryId);
                }
            }
        }
    },
    IOS_320: function (key, name) {
        var prams = {
            "category": key,
            "title": name
        };
        window.location.href = "tuhuDiscover://segue#THDiscoverHomeVC#" + encodeURIComponent(JSON.stringify(prams));
    },
    Android_475: function (key, name) {
        window.WebViewJavascriptBridge.callHandler(
        'toDiscoveryListLabel'
           , {
               'CategoryId': key
           }
        , function (responseData) { })
    },
    Invoke_v1: function (key, name) {
        if (userAgentInfo.indexOf("Android") >= 0) {
            if (GetAndroidVersion >= 470) {
                toDiscoveryListApi.Android_475(key, name);
            }
        }
        else {
            if (GetIosVersion >= 320) {
                toDiscoveryListApi.IOS_320(key, name);
            }
        }
    }
}

//--------------------------Android,IOS 喜欢文章回调函数
function Marked_as_like(e) {
    _newsLikeState = e;
    $("#like").hasClass('active') ? $("#like").removeClass('active') : $("#like").addClass('active');

    var vote = parseInt($(".like-n > span").html());
    $(".like-n > span").html(e == 1 ? vote + 1 : vote - 1);

    //LoginApi.Login(toDiscoverylikeApi.Invoke);
}
//--------------------------Android,IOS 微信分享
var WeiXinShareApi = {
    Android: function (n) {
        //1微信2朋友圈
        window.WebViewJavascriptBridge.callHandler(
        'share', {
            'key': n || 1,
        }
        , function (responseData) { })
    },
    IOS: function (n) {
        //tuhudiscover://share#0或1
        var url = (((n - 1) || 0) == 0 ? "tuhuDiscover://share#0" : "tuhuDiscover://share#1");
        window.location.href = url;
    },
    Invoke: function (n) {
        if (n == 1) {
            Countloading(_PKID + ",分享微信好友");
        } else if (n == 2) {
            Countloading(_PKID + ",分享朋友圈");
        }
        if (userAgentInfo.indexOf("Android") >= 0) {
            if (AndroidVersionNumber) {
                WeiXinShareApi.Android(n);
            }
        }
        else {
            if (IosVersionNumber) {
                WeiXinShareApi.IOS(n);
            }
        }

        n == 1 ? WeiXinShareApi.AddSharePyqOrWx(0) : WeiXinShareApi.AddSharePyqOrWx(1);
    },
    WxShareShowOrHide: function (b) {
        if (b == true) {
            $("#haoyou").show();
            $("#penyouquan").show();
        }
        else {
            $("#haoyou").hide();
            $("#penyouquan").hide();
        }
    },
    AddSharePyqOrWx: function (n) {
        $.ajax({
            url: "http://faxian.tuhu.cn/NewsApi/ShareWxOrPyq?callback=?",
            data: {
                "shareType": n || 0,
                "pkid": _PKID,
            },
            dataType: "jsonp",
            type: "get",
            success: function (data) {
                console.log(data);
            }
        });
    }
}
PhotoView.Invoke();
//=================================移动交互相关 End==================================//
//=================================万能跳转 Begin==================================//
var browser = {
    versions: function () {
        var u = navigator.userAgent, app = navigator.appVersion;
        return {
            trident: u.indexOf('Trident') > -1,                             //IE内核
            presto: u.indexOf('Presto') > -1,                               //opera内核
            webKit: u.indexOf('AppleWebKit') > -1,                          //苹果、谷歌内核
            gecko: u.indexOf('Gecko') > -1 && u.indexOf('KHTML') == -1,     //火狐内核
            mobile: !!u.match(/AppleWebKit.*Mobile.*/),                     //是否为移动终端
            ios: !!u.match(/\(i[^;]+;( U;)? CPU.+Mac OS X/),                //ios终端
            android: u.indexOf('Android') > -1 || u.indexOf('Adr') > -1,    //android终端
            iPhone: u.indexOf('iPhone') > -1,                               //是否为iPhone或者QQHD浏览器
            iPad: u.indexOf('iPad') > -1,                                   //是否iPad
            webApp: u.indexOf('Safari') > -1,                               //是否web应该程序，没有头部与底部
            weixin: u.indexOf('MicroMessenger') > -1,                       //是否微信
            qq: u.match(/\sQQ/i) == " qq",                                  //是否QQ
            isBrowser: function () {
                var UA = u.toLowerCase(), _isBrowser = false;

                if (u.indexOf('MicroMessenger') > -1) { return _isBrowser }; //排除微信浏览器

                if (!!u.match(/AppleWebKit.*Mobile.*/) != true) { return _isBrowser }  //排除PC--非移动终端

                if ((UA.indexOf('360ee') > -1) ||            //360极速浏览器
                    (UA.indexOf('360se') > -1) ||            //360安全浏览器
                    (UA.indexOf('se') > -1) ||               //搜狗浏览器
                    (UA.indexOf('aoyou') > -1) ||            //遨游浏览器
                    (UA.indexOf('theworld') > -1) ||         //世界之窗浏览器
                    (UA.indexOf('worldchrome') > -1) ||      //世界之窗极速浏览器
                    (UA.indexOf('greenbrowser') > -1) ||     //绿色浏览器
                    (UA.indexOf('qqbrowser') > -1) ||        //QQ浏览器
                    (UA.indexOf('ucbrowser') > -1) ||        //UC浏览器
                    (UA.indexOf('firefox') > -1) ||          //firefox浏览器
                    (UA.indexOf('baidu') > -1))              //百度浏览器
                {
                    _isBrowser = true;
                }

                if (_isBrowser == false) {
                    if (UA.indexOf("android") > -1 && UA.indexOf("mobile safari") > -1)                 //若跳过上面判断,同时成立表示在app中
                        _isBrowser = false;
                    else if (!!u.match(/\(i[^;]+;( U;)? CPU.+Mac OS X/) && UA.indexOf('safari') > -1)   //若跳过上面判断,同时成立表示在safari中
                        _isBrowser = true;
                }
                return _isBrowser
            }
        };
    }(),
    language: (navigator.browserLanguage || navigator.language).toLowerCase()
}

//javascript:AllGoTo.InvokeApp('TR-PR-P7C','15','tire'); 
//javascript: AllGoTo.InvokeH5('http://www.sina.com.cn/');
//javascript: AllGoTo.InvokeBY('xby;dby;pm;rx', 'upkeep');

//--------------------------------跳转内部产品    javascript:GoTo(1,2)
function GoTo_Old(ProductID, VariantID) {
    if (ProductID != "") {
        //跳转页面地址
        var currentAddress = document.location.toString();
        skipAddress(currentAddress, ProductID, 1);
    }
    if (userAgentInfo.indexOf("Android") >= 0) {
        //window.WebViewJavascriptBridge.callHandler(
        //'toChePinBridge'
        //    , {
        //        'ProductID': ProductID,
        //        'VariantID': VariantID
        //    }
        //, function (responseData) {
        //})
        var android = {};
        android.androidValue = [{ "ProductID": ProductID, "VariantID": VariantID }];
        android.androidKey = "cn.TuHu.Activity.AutomotiveProducts.AutomotiveProductsDetialUI";
        console.log(android);
        window.WebViewJavascriptBridge.callHandler('jumpActivityBridge', encodeURIComponent(encodeURIComponent(JSON.stringify(android))));
    }
    else if (!!userAgentInfo.match(/\(i[^;]+;( U;)? CPU.+Mac OS X/)) {
        var dt = {
            'ProductID': ProductID,
            'VariantID': VariantID
        }
        sendCommand("THGoodDetailVC", JSON.stringify(dt));
    }
    else {
        window.location.href = "http://wx.tuhu.cn/Products/Details?pid=" + (ProductID || 0) + "&vid=" + (VariantID || 0);
    }
}

//车品跳转（现已不用这个）
function GoTo(ProductID, VariantID) {
    AllGoTo.InvokeApp(ProductID, VariantID, "goods");
}

var AllGoTo = {
    Format: function () {
        /// <summary>用法: Format("Hi {0}, you are {1}!", "Foo", 100) </summary>
        var s = arguments[0];
        for (var i = 0; i < arguments.length - 1; i++) {
            s = s.replace("{" + i + "}", arguments[i + 1]);
        }
        return s;
    },
    Type: {
        App: {
            Tire: {
                Android: "cn.TuHu.Activity.TireInfoUI",
                Ios: "tuhuaction://segue#TNTireInfoVC#",
                Client: "app"
            },
            Goods: {
                Android: "cn.TuHu.Activity.AutomotiveProducts.AutomotiveProductsDetialUI",
                Ios: "tuhuaction://segue#TNGoodsListDetailViewController#",
                Client: "app"
            },
            UpKeep: {
                Android: "toBaoYang",
                Ios: "tuhuaction://segue#TNMaintainVC#",
                Client: "app"
            },
            Hub: {
                Android: "cn.TuHu.Activity.Hub.HubDetailsActivity",
                Ios: "tuhuaction://segue#TNTireInfoVC#",
                Client: "app"
            }
        },
        WeiXin: {
            Tire: {
                Url: "http://wx.tuhu.cn/Products/Tires/{0}/{1}",
                Client: "weixin"
            },
            Goods: {
                Url: "http://wx.tuhu.cn/Products/Details?pid={0}&vid={1}",
                Client: "weixin"
            },
            UpKeep: {
                Url: "http://wx.tuhu.cn/BaoYang/BaoYangMenus#!{0}",
                Client: "weixin"
            },
            Hub: {
                Url: "http://wx.tuhu.cn/Products/Details/{0}/{1}",
                Client: "weixin"
            }
        },
        H5: {
            Android: "cn.TuHu.Activity.AutomotiveProducts.AutomotiveProductsWebViewUI",
            Ios: "tuhuaction://segue#TNHBH5VViewController#",
            H5: "http://wx.tuhu.cn/BaoYang/BaoYangMenus#!{0}",
            Client: "h5"
        }
    },
    KeyToType: function (key) {
        var _type;
        switch (key.toLowerCase()) {
            case "tire":
                if (browser.versions.weixin || browser.versions.isBrowser())
                    _type = AllGoTo.Type.WeiXin.Tire;
                else if (browser.versions.android || browser.versions.ios)
                    _type = AllGoTo.Type.App.Tire;
                break;
            case "goods":
                if (browser.versions.weixin || browser.versions.isBrowser())
                    _type = AllGoTo.Type.WeiXin.Goods;
                else if (browser.versions.android || browser.versions.ios)
                    _type = AllGoTo.Type.App.Goods;
                break;
            case "upkeep":
                if (browser.versions.weixin || browser.versions.isBrowser())
                    _type = AllGoTo.Type.WeiXin.UpKeep;
                else if (browser.versions.android || browser.versions.ios)
                    _type = AllGoTo.Type.App.UpKeep;
                break;
            case "hub":
                if (browser.versions.weixin || browser.versions.isBrowser())
                    _type = AllGoTo.Type.WeiXin.Hub;
                else if (browser.versions.android || browser.versions.ios)
                    _type = AllGoTo.Type.App.Hub;
                break;
            default:
        }
        return _type;
    },
    AppModel: function () {
        this.ProductID,
        this.VariantID,
        this.FunctionID
    },
    H5UrlModel: function () {
        this.Url,
        this.FunctionID
    },
    H5Model: function () {
        this.url,
        this.shareImage,
        this.shareTitle,
        this.shareDescrip,
        this.shareUrl
    },
    InvokeApp: function (p, v, f) {
        /// <summary>轮胎&车品跳转</summary>
        //  p:pid, v:vid ,f:产品类型
        var _model, _type = AllGoTo.KeyToType(f);
        _model = new AllGoTo.AppModel();
        _model.ProductID = p;
        _model.VariantID = v;
        //ga埋点
        ga('send', 'event', 'toutiao_detail', 'jump_product',p+"|"+v);
        if (browser.versions.weixin || browser.versions.isBrowser()) {
            _model.FunctionID = _type.Url;
            var wxurl = AllGoTo.Format(_model.FunctionID, _model.ProductID, _model.VariantID);
            location.href = wxurl;
        }
        else if (browser.versions.android) {
            _model.FunctionID = _type.Android;

            if (f == 'hub') {
                var androidParames = {
                    "productId": p,
                    "variantId": v,
                    "FunctionID": _type.Android
                }
                window.WebViewJavascriptBridge.callHandler('toActityBridge', encodeURIComponent(encodeURIComponent(JSON.stringify(androidParames))));
            } else {
                window.WebViewJavascriptBridge.callHandler('toActityBridge',encodeURIComponent(encodeURIComponent(JSON.stringify(_model))));
            }            
        }
        else if (browser.versions.ios) {
            location.href = _type.Ios + encodeURIComponent(JSON.stringify(_model));
            //location.href = "tuhuaction://segue#THTireInfoVC#" + JSON.stringify(_model);
        } else if (!browser.versions.mobile) {//非移动端（PC）
            var targetUrl = "http://item.tuhu.cn/Products/" + (v == "" ? p + ".html" : p + "/" + v + ".html");
            window.open(targetUrl);
        }
    },
    InvokeBY: function (params, f) {
        /// <summary>保养类型跳转</summary>
        var _type = AllGoTo.KeyToType(f);
        if (browser.versions.weixin || browser.versions.isBrowser()) {
            var wxurl = AllGoTo.Format(_type.Url, params);
            location.href = wxurl;
        }
        else if (browser.versions.android) {
            window.WebViewJavascriptBridge.callHandler(
            '' + _type.Android + '', encodeURIComponent(encodeURIComponent("{'baoyangType':'" + params + "'}"))
            , function (responseData) {
            });
        }
        else if (browser.versions.ios) {
            location.href = _type.Ios + encodeURIComponent("{'types':'" + params + "'}");
        }
    },
    InvokeH5: function (_url) {
        //ga埋点
        ga('send', 'event', 'toutiao_detail', 'jump_product', _url);
        /// <summary>只适用自定义URL跳转</summary>
        if (browser.versions.weixin || browser.versions.isBrowser()) {
            window.open(_url, "_blank");    //location.href = _url;
        }
        else if (browser.versions.android) {
            var _model = new AllGoTo.H5UrlModel();
            _model.Url = _url;
            _model.FunctionID = AllGoTo.Type.H5.Android;

            window.WebViewJavascriptBridge.callHandler('toActityBridge',
                encodeURIComponent(encodeURIComponent(JSON.stringify(_model))));
        }
        else if (browser.versions.ios) {
            var _model = new AllGoTo.H5Model();
            _model.url = _url;

            location.href = AllGoTo.Type.H5.Ios + encodeURIComponent(JSON.stringify(_model));
        } else if (!browser.versions.mobile) {//非移动端（PC）
            window.open(_url);
        }

    }
}
//=================================万能跳转 End====================================//

var Utils = {
    GetQueryString: function (name) {
        ///<summary>获取页面参数</summary>
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
        var r = window.location.search.substr(1).match(reg);
        if (r != null) return unescape(r[2]); return null;
    },
    IsPc: function () {
        return Math.min(window.screen.availWidth, window.screen.availHeight) > 630;
    },
    IsAndroid: function () {
        if (!this.checkAndroid) {
            this.checkAndroid = /android/i.test(navigator.userAgent);
        }
        return this.checkAndroid;
    },
    IsIOS: function () {
        if (!this.checkIOS) {
            this.checkIOS = /iphone|ipad|ipod/i.test(navigator.userAgent);
        }
        return this.checkIOS;
    },
    IsApp: function () {
        return !!window.isVersion;
    },
    IsAppNew: function () {
        if (this.IsAndroid()) {
            return (navigator.userAgent.indexOf('tuhuAndroid') >= 0);
        } else {
            return (navigator.userAgent.indexOf('tuhuIOS') >= 0);
        }
    },
    IsWx: function () {
        return navigator.userAgent.toLowerCase().match(/MicroMessenger/i) == "micromessenger";

    }
}

function whatDevice() {
    if (Utils.IsPc()) {
        return 'pc';
    } else {
        return 'phone';
    }
}

function whatChannel() {
    if (Utils.IsAppNew()) {
        return 'app';
    } else if (Utils.IsWx()) {
        return 'wx';
    } else {
        return 'others';
    }
}

function whatPlatform() {
    if (Utils.IsIOS()) {
        return 'ios';
    } else if (Utils.IsAndroid()) {
        return 'android';
    } else {
        return 'others';
    }
}

document.write('<script>var tracker = {platform: "' + whatDevice() + '_' + whatChannel() + '_' + whatPlatform() + '"};</script>');




//=================================域名配置====================================//

//遍历所有文章产品内容图片
$(".section  .de-page").each(function (i) {
    //$(this).find("img").each(function (i) {
    //    var imgObj = $(this);
    //    var src = imgObj.attr("src") || "";
    //    if (src.length > 0 || src.toLocaleLowerCase().indexOf("http") >= 0) {
    //        //$(this).attr("class", "cover");
    //        imgObj.removeAttr("src");
    //        imgObj.attr({
    //            "style": "background:url(http://resource.tuhu.cn/image/public/loading-712.gif) no-repeat center center",
    //            "data-original": src
    //        });
    //    }
    //});
    $(this).find("a").each(function (n) {
        var aObj = $(this);
        var href = aObj.attr("href") || "";
        if ($.trim(href).substring(0, 10) != "javascript") {
            aObj.attr("href", "javascript: AllGoTo.InvokeH5('" + href + "');");
        }
    });
});
