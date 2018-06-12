//<script type="text/javascript">
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
//--------------------------------跳转IOS老协议
function sendCommand(cmd, param) {
    var url = "#testapp#" + cmd + "#" + param;
    document.location = url;
}
//--------------------------------跳转内部产品    javascript:GoTo(1,2)
function GoTo(ProductID, VariantID) {
    if (ProductID != "") {
        //跳转页面地址
        var currentAddress = document.location.toString();
        skipAddress(currentAddress, ProductID, 1);
    }

    if (userAgentInfo.indexOf("Android") >= 0) {
        window.WebViewJavascriptBridge.callHandler(
        'toChePinBridge'
            , {
                'ProductID': ProductID,
                'VariantID': VariantID
            }
        , function (responseData) {
        })
    }
    else if (!!userAgentInfo.match(/\(i[^;]+;( U;)? CPU.+Mac OS X/)) {
        var dt = {
            'ProductID': ProductID,
            'VariantID': VariantID
        }
        sendCommand("gotoMonovularCarAccessoriesDetail", JSON.stringify(dt));
    }
    else {
        window.location.href = "http://wx.tuhu.cn/Products/Details?pid=" + (ProductID || 0) + "&vid=" + (VariantID || 0);
    }
}
//--------------------------------跳转淘宝商品    javascript:GoToTaobao(1,2,3)
function GoToTaobao(ProductUrl, ProductID, ProductType) {
    if (userAgentInfo.indexOf("Android") >= 0) {
        if (isAndroidVersion) {
            window.WebViewJavascriptBridge.callHandler(
            'toTaobaoKe'
                , {
                    'itemId': ProductID,        //商品真实id
                    'itemType': ProductType,    //商品类型 1：淘宝商品;2：天猫商品
                    'pid': 'mm_100829024_0_0',  //pid
                }
            , function (responseData) {
            });
        }
        else {
            document.location = ProductUrl;
        }
    } else {
        if (isIosVersion) {
            var replaceUrl = "";
            if (ProductUrl.toLocaleLowerCase().indexOf("https") >= 0) {
                replaceUrl = ProductUrl.ReplaceAll("https:", "taobao:", "gi");
            }
            else {
                replaceUrl = ProductUrl.ReplaceAll("http:", "taobao:", "gi");
            }
            document.location = replaceUrl;
        } else {
            document.location = ProductUrl;
        }
    }

    //跳转页面地址
    var currentAddress = document.location.toString();
    if (ProductID != "") {
        skipAddress(currentAddress, ProductID, 2);
    }
}
//--------------------------------记录跳转数据
function skipAddress(currentAddress, productId, type) {
    try {
        var articleId = currentAddress.substring((currentAddress.lastIndexOf("_") + 1));
        articleId = articleId.substring(0, articleId.lastIndexOf("."));
        $.ajax({
            type: "POST",
            url: "http://huodong.tuhu.cn/ArticleNewList/AddArticleNewList?",
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
//--------------------------------取消点赞
function uvcmp(da) {
    if (da == null) {
        return false;
    }
    $.ajax({
        url: "http://huodong.tuhu.cn/Advertise/UnVote.html",
        data: { pkid: pakid, userId: da.userid },
        type: 'post',
        datatype: 'json',
        success: function (data) {
            var jsonData = JSON.parse(data);
            var voteModel = new Object();
            voteModel.resultState = 0;//未点赞状态
            voteModel.AllVoteCount = jsonData.AllVoteCount;
            SetCookie(phone + pakid, JSON.stringify(voteModel));//获取是否点赞的cookie
        },
    });
}
//--------------------------------点赞
function vcmp(da) {
    if (da == null) {
        return false;
    }
    $.ajax({
        url: "http://huodong.tuhu.cn/Advertise/Vote.html",
        data: { pkid: pakid, userId: da.userid },
        type: 'post',
        datatype: 'json',
        success: function (data) {
            var jsonData = JSON.parse(data);
            var voteModel = new Object();
            voteModel.resultState = 1;//点赞状态
            voteModel.AllVoteCount = jsonData.AllVoteCount;
            SetCookie(phone + pakid, JSON.stringify(voteModel));//获取是否点赞的cookie
        },
    });
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
var Uuid = "";
//--------------------------图片查看器
var PhotoView = {
    appendView: function () {
        //遍历所有文章产品内容图片
        $(".section > .desc").each(function (i) {
            $(this).find("img").each(function (i) {
                var src = $(this).attr("src") || "";
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
//----------------------------Androdi,IOS 文章评论
var toDiscoveryApi = {
    Android: function (e) {
        window.WebViewJavascriptBridge.callHandler(
         'toDiscovery'
        , { 'keyboard': e }
        , function (responseData) {
        })
    },
    IOS: function (e) {
        e = (e == 0 ? 1 : 0);
        var _iosCommentUrl = "http://huodong.tuhu.cn/home/articlecomment.html?PKID=" + _PKID + "&UserID=" + _UserId;
        var prams = {
            "commentUrl": _iosCommentUrl,
            "mytitle": _BigTitle,
            "categoryName": _CategoryName,
            "pkid": _PKID,
            "showKeyboard": e
        };
        var url = "tuhuDiscover://segue#THDiscoverCommentVC#" + encodeURIComponent(JSON.stringify(prams));
        window.location.href = url;
    },
    Invoke: function (e) {
        if (e == 0) {
            Countloading(_PKID + ",发表评论");
        } else if (e == 1) {
            Countloading(_PKID + ",评论");
        }
        if (userAgentInfo.indexOf("Android") >= 0) {
            if (AndroidVersionNumber) {
                toDiscoveryApi.Android(e);
            }
        }
        else {
            if (IosVersionNumber) {
                toDiscoveryApi.IOS(e);
            }
        }
    }
}
//---------------------------Androdi,IOS 相关文章
var toDiscoveryDetailActivityApi = {
    Android: function (model) {

        var str = {
            "Article": {
                "TitleColor": model.TitleColor,
                "CategoryName": model.CategoryName,
                "ContentUrl": model.ContentUrl,
                "SmallImage": (model.SmallImage || "http://image.tuhu.cn/activity/image/ShareImage.jpg"),
                "CommentNum": model.CommentNum,
                "PKID": model.PKID,
                "AnnotationTime": "",
                "SmallTitle": model.SmallTitle,
                "Brief": model.Brief,
                "Source": "",
                "PublishDateTime": "",
                "PublishNewDateTime": "",
                "Heat": model.Heat,
                "ClickCount": model.ClickCount,
                "BigTitle": model.BigTitle,
                "VoteState": true,
                "RedirectUrl": "",
                "Vote": model.Vote,
                "Image": model.Image,
                "Catalog": "0"
            }
        };
        window.WebViewJavascriptBridge.callHandler(
           'toDiscoveryDetailActivity', encodeURIComponent(encodeURIComponent(JSON.stringify(str)))
           , function (responseData) {
           })
    },
    IOS: function (model) {
        var prams = {
            "discoverModel<THDiscoverModel>":
             {
                 "PKID": model.PKID,
                 "targetUrl": model.ContentUrl,
                 "title": model.BigTitle,
                 "category": model.Category,
                 "commentCount": model.CommentNum,
                 "shareTitle": model.SmallTitle,
                 "shareImage": (model.SmallImage || "http://image.tuhu.cn/activity/image/ShareImage.jpg"),
                 "favorCount": model.Vote,
                 "Brief": model.Brief
             }
        };
        var str = "tuhuDiscover://segue#THDiscoverWebView#" + encodeURIComponent(JSON.stringify(prams));
        window.location.href = str
    },
    Invoke: function (obj) {
        var _model = JSON.parse($(obj).find(".span_data").html());
        if (_model != null) {
            Countloading(_PKID + "," + _model.PKID);
            if (InitNewsApi.IsWeiXin()) {
                window.location.href = _model.ContentUrl;
            }
            else {
                if (userAgentInfo.indexOf("Android") >= 0) {
                    if (AndroidVersionNumber) {
                        toDiscoveryDetailActivityApi.Android(_model);
                    }
                }
                else {
                    if (IosVersionNumber) {
                        toDiscoveryDetailActivityApi.IOS(_model);
                    }
                }
            }
        }
    }
}
//---------------------------Androdi,IOS 喜欢文章
var toDiscoverylikeApi = {
    Android: function (e) {
        NewsDataApi.PostAddVote();
        window.WebViewJavascriptBridge.callHandler(
       'Discoverylike', {}
       , function (responseData) {
       })
    },
    IOS: function (e) {
        NewsDataApi.PostAddVote();
        var url = (e == 1 ? "tuhuDiscover://vote#1" : "tuhuDiscover://vote#0"); //1:点赞,0:取消点赞
        window.location.href = url;
    },
    Invoke: function () {
        Countloading(_PKID + ",喜欢");
        _newsLikeState = $("#like").hasClass("active") ? 0 : 1;
        var vote = parseInt($(".like-n > span").html());
        $(".like-n > span").html(_newsLikeState == 1 ? vote + 1 : (vote <= 0 ? 0 : vote - 1));

        $("#like").hasClass('active') ? $("#like").removeClass('active') : $("#like").addClass('active');
        if (userAgentInfo.indexOf("Android") >= 0) {
            if (AndroidVersionNumber) {
                toDiscoverylikeApi.Android(_newsLikeState);
            }
        }
        else {
            if (IosVersionNumber) {
                toDiscoverylikeApi.IOS(_newsLikeState);
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
            url: "http://huodong.tuhu.cn/NewsApi/ShareWxOrPyq?callback=?",
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
//--------------------------数据接口
var NewsDataApi = {
    PostAddVote: function () {
        if ((_UserId != null && _UserId.length > 0) && (_PKID != null && _PKID.length > 0)) {
            $.ajax({
                url: "http://huodong.tuhu.cn/Advertise/AddVoteNewsJsonp?callback=?",
                data: {
                    "UserId": _UserId,
                    "PKID": _PKID,
                    "Vote": _newsLikeState
                },
                dataType: "jsonp",
                type: "get",
                success: function (data) {
                    //alert(_UserId + "," + _PKID + "," + _newsLikeState + "," + data);
                }
            });
        }
    },
    SelectNewsInfo: function () {
        $.ajax({
            url: "http://huodong.tuhu.cn/Advertise/SelectNewsInfo?callback=?",
            data: {
                "PKID": _PKID,
                "Category": _CategoryId
            },
            dataType: "jsonp",
            type: "get",
            success: function (data) {
                try {
                    DeepShare.GetNewsInfo(data);
                } catch (e) {
                }
                var jsonData = eval("(" + data + ")");
                var CommentItems = jsonData.CommentItems;
                var ArticleItems = jsonData.ArticleItems;
                var CTemplateHtml = "", ATemplateHtml = "";
                var CommentItemsHtml = "", ArticleItemsHtml = "";

                //评论
                if (CommentItems != null && CommentItems.length > 0) {
                    $(".no-data").hide();
                    CTemplateHtml = "<li><div class=\"img\">";
                    CTemplateHtml += "<img src=\"#t_src#\" width=\"30\" height=\"30\" /></div>";
                    CTemplateHtml += "<div class=\"other\"><span class=\"name\">#t_name#</span>";
                    CTemplateHtml += "<span class=\"date\">#t_date#</span><p class=\"desc\">#t_desc#</p></div></li>";

                    $(".comment > .title > a > span").html("评论(" + CommentItems[0].CommentCount + ")");
                    for (var i in CommentItems) {
                        var time = (CommentItems[i].CommentTime.substring(0, CommentItems[i].CommentTime.lastIndexOf("."))).replace(/\T/ig, " ");
                        CommentItemsHtml += CTemplateHtml.
                            replace(/#t_src#/ig, CommentItems[i].UserHead).
                            replace(/#t_name#/ig, CommentItems[i].UserName).
                            replace(/#t_date#/ig, time).
                            replace(/#t_desc#/ig, CommentItems[i].CommentContent);
                    }
                }
                else {
                    $(".no-data").show();
                }

                //相关文章
                if (ArticleItems != null && ArticleItems.length > 1) {
                    //原展示方式
                    //ATemplateHtml = "<li class=\"swiper-slide\" onclick=\"toDiscoveryDetailActivityApi.Invoke(this)\">";
                    //ATemplateHtml += "<img class=\"cover\" src=\"#t_src#\" width=\"180\" height=\"120\">";
                    //ATemplateHtml += "<span class=\"name\">#t_content#</span>";
                    //ATemplateHtml += "<span style=\"display:none;\" class=\"span_data\">#t_data#</span></li>";
                    //新展示方式
                    ATemplateHtml = "<li class=\"swiper-slide\" onclick=\"toDiscoveryDetailActivityApi.Invoke(this)\">";
                    ATemplateHtml += "<i class=\"i-arrow\"></i>";
                    ATemplateHtml += "<span class=\"name\">#t_content#</span>";
                    ATemplateHtml += "<p class=\"date\">#t_datetime#</p>";
                    ATemplateHtml += "<span style=\"display:none;\" class=\"span_data\">#t_data#</span></li>";

                    for (var i = (ArticleItems.length - 1) ; i >= 0; i--) {
                        if (ArticleItems[i].PKID == _PKID) {
                            var vote = parseInt(ArticleItems[i].Vote) > 0 ? ArticleItems[i].Vote : 0;
                            $(".like-n > span").html(vote);
                        }
                        else {
                            var _data = new THDiscoverModel();
                            _data.TitleColor = ArticleItems[i].TitleColor;
                            _data.Category = ArticleItems[i].Category;
                            _data.CategoryName = ArticleItems[i].CategoryName;
                            _data.ContentUrl = ArticleItems[i].ContentUrl;
                            _data.SmallImage = ArticleItems[i].SmallImage;
                            _data.CommentNum = ArticleItems[i].CommentCount;
                            _data.PKID = ArticleItems[i].PKID;
                            _data.SmallTitle = ArticleItems[i].SmallTitle;
                            _data.Heat = ArticleItems[i].Heat;
                            _data.ClickCount = ArticleItems[i].ClickCount;
                            _data.BigTitle = ArticleItems[i].BigTitle;
                            _data.Vote = ArticleItems[i].Vote;
                            _data.Image = ArticleItems[i].Image;
                            _data.PublishDateTime = ArticleItems[i].PublishDateTime;
                            _data.Brief = ArticleItems[i].Brief;

                            var _PublishDateTime = ArticleItems[i].PublishDateTime;
                            _PublishDateTime = _PublishDateTime.substring(0, _PublishDateTime.indexOf("T"));

                            ArticleItemsHtml += ATemplateHtml.
                                replace(/#t_datetime#/ig, _PublishDateTime).
                                replace(/#t_content#/ig, ArticleItems[i].BigTitle).
                                replace(/#t_data#/ig, JSON.stringify(_data));
                        }
                    }
                    $(".about").show();
                    //for (var i in ArticleItems) {}
                }
                else { $(".about").hide(); }
                $(".comment > .list").html(CommentItemsHtml);
                $(".about > .list > ul").html(ArticleItemsHtml);
                swiperApi.invoke();              //添加相关文章滑动效果
                InitNewsApi.InitNewsJoinStyle(); //微信客户端更换显示方式
            }
        });
    },
    IsNewsForLikeNews: function () {
        var getUserId = getCookie("UserId");
        if (getUserId != null && getUserId.length > 0) {
            $.ajax({
                url: "http://huodong.tuhu.cn/Advertise/IsNewsForLikeNews?callback=?",
                data: {
                    "PKID": _PKID,
                    "UserId": getUserId
                },
                dataType: "jsonp",
                type: "get",
                success: function (data) {
                    var resultData = parseInt(data);
                    resultData == 1 ? $("#like").addClass("active") : $("#like").removeClass("active");
                }
            });
        }
    }
}
//-------------------------文章初始操作
var InitNewsApi = {
    InitHideForVersion: function () {
        if (InitNewsApi.IsWeiXin()) {
            $("#wrap > .about").show();
        }
        else {
            setTimeout(function () {
                //此处延时执行，为解决app端延迟传递版本号问题
                if (IosVersionNumber == false && AndroidVersionNumber == false) {
                    InitNewsApi.ModuleHide();
                }
                else {
                    InitNewsApi.ModuleShow();
                }
            }, 3000);
        }
    },
    InitHideForImages: function () {
        $(".section").each(function (i) {
            //遍历，隐藏无效产品图片
            var obj = $(this).find(".img > .inner").find("img");
            var url = $(obj).attr("src") || "";
            if (url.length <= 0 || url.toLocaleLowerCase().indexOf("http") < 0) {
                $(obj).hide().removeClass("cover");
            }

            //遍历，将只有图片的产品，改变其padding
            if ($(this).find(".img > .inner").find(".other").is(":hidden")) {
                $(this).find(".img").css("padding", 0);
            }
        });
    },
    InitHideDownApp: function () {
        if (InitNewsApi.IsWeiXin()) {
            //$("#ActivitiesDownAppSe").css("display", "block")
            //$("#ActivitiesDownApp").click(function () {
            //    window.location.href = "http://a.app.qq.com/o/simple.jsp?pkgname=cn.TuHu.android";
            //});
            //$("#ActivitiesDownAppClose").click(function () {
            //    $("#ActivitiesDownAppSe").hide();
            //});
            $(".WxAppDown").css("display", "block");
            $(".WxAppDown").click(function () {
                CountForWeixin(_PKID + ",打开app");
                window.location.href = "http://a.app.qq.com/o/simple.jsp?pkgname=cn.TuHu.android";
            });
        }
    },
    InitBannerStyle: function () {
        var src = $("#banner").find("img").attr("src") || "";
        if (src.length < 0 || src.toLowerCase().indexOf("http") < 0) {
            $("#banner").hide();
            $("#wrap > #content > .mtitle").attr("class", "mtitle no-bg");
        }
    },
    InitNewsJoinStyle: function () {
        if (InitNewsApi.IsWeiXin()) {
            $(".about").attr("class", "about about-artical");
        }
    },
    InitWeiXinForNews: function () {
        //if (InitNewsApi.IsWeiXin()){ NewsDataApi.SelectNewsInfo();}
        NewsDataApi.SelectNewsInfo();
    },
    ModuleShow: function () {
        $("#content > .like").show();
        $("#wrap > .comment > #commentSubmit").show();  //发表评论按钮
        $("#wrap > .comment").show();
        $("#wrap > .about").show();
    },
    ModuleHide: function () {
        $("#content > .like").hide();
        $("#wrap > .comment > #commentSubmit").hide(); //发表评论按钮
        //$("#wrap > .comment").hide();
        $("#wrap > .about").hide();
    },
    IsWeiXin: function () {
        var ua = window.navigator.userAgent.toLowerCase();
        if (ua.match(/MicroMessenger/i) == 'micromessenger') { return true; }
        return false;
    },
    AddArticleClickNew: function () {
        try {
            if (_PKID.length > 0) {
                $.ajax({
                    url: "http://huodong.tuhu.cn/Advertise/AddArticleClickNew?callback=?",
                    data: {
                        "PKID": _PKID,
                    },
                    dataType: "jsonp",
                    type: "get",
                    success: function (data) {
                        console.log(data);
                    }
                });
            }
        } catch (e) { };
    }
}

InitNewsApi.ModuleHide();
InitNewsApi.InitHideForVersion();
InitNewsApi.InitHideForImages();
InitNewsApi.InitHideDownApp();
InitNewsApi.InitBannerStyle();
InitNewsApi.InitWeiXinForNews();
InitNewsApi.AddArticleClickNew();
PhotoView.Invoke();

//=================================移动交互相关 End==================================//
function Countloading(pkid) {
    var uuidstr = "";
    var cookieId = "";
    var pids = "";

    if (!getCookie("ArticleloadCookie")) {
        SetCookie("ArticleloadCookie", GetDateTime1() + CreateNum())
    }
    cookieId = getCookie("ArticleloadCookie");

    var uid = Uuid;
    if (!Uuid) {
        uid = "";
    }
    var userAgent = navigator.userAgent;
    if (userAgent.indexOf("Android") >= 0) {
        uuidstr = "Android|" + uid;
    } else {
        uuidstr = "IOS|" + uid;
    }
    console.log(GetDateTime());
    console.log(window.location.href);
    console.log(uuidstr);
    console.log(cookieId);
    console.log(pkid);
    try {

        var e = new Image;
        e.id = "Deseno";
        e.src = "http://t.tuhu.cn/act?tm=" + encodeURIComponent(GetDateTime()) +
                                        "&cp=" + encodeURIComponent(window.location.href) +
                                        "&uid=" + encodeURIComponent(uuidstr) +
                                        "&pids=" + encodeURIComponent(pkid) +
                                        "&c_id=" + cookieId;

    } catch (e) {
    }
}
function CountForWeixin(pkid) {
    var uuidstr = "WeiXin|";
    var cookieId = "";
    var pids = "";
    if (!getCookie("ArticleloadCookie")) {
        SetCookie("ArticleloadCookie", GetDateTime1() + CreateNum())
    }
    cookieId = getCookie("ArticleloadCookie");
    try {
        var e = new Image;
        e.id = "Deseno";
        e.src = "http://t.tuhu.cn/act?tm=" + encodeURIComponent(GetDateTime()) +
                                        "&cp=" + encodeURIComponent(window.location.href) +
                                        "&uid=" + encodeURIComponent(uuidstr) +
                                        "&pids=" + encodeURIComponent(pkid) +
                                        "&c_id=" + cookieId;
    } catch (e) {

    }
}
function GetDateTime() {
    var now = new Date();

    var year = now.getFullYear();       //年
    var month = now.getMonth() + 1;     //月
    var day = now.getDate();           //日
    var hh = now.getHours();            //时
    var mm = now.getMinutes();          //分
    var ss = now.getSeconds();          //秒

    var clock = year + "-";

    if (month < 10) {
        clock += "0";
    }

    clock += month + "-";

    if (day < 10) {
        clock += "0";
    }

    clock += day + " ";

    if (hh < 10) {
        clock += "0";
    }

    clock += hh + ":";
    if (mm < 10) {
        clock += '0';
    }
    clock += mm + ":";

    if (ss < 10) {
        clock += '0';
    }
    clock += ss;
    return clock;
}
function GetDateTime1() {
    var now = new Date();

    var year = now.getFullYear();       //年
    var month = now.getMonth() + 1;     //月
    var day = now.getDate();           //日
    var hh = now.getHours();            //时
    var mm = now.getMinutes();          //分
    var ss = now.getSeconds();          //秒

    var clock = year.toString();

    if (month < 10) {
        clock += "0";
    }

    clock += month;

    if (day < 10) {
        clock += "0";
    }

    clock += day;

    if (hh < 10) {
        clock += "0";
    }

    clock += hh;
    if (mm < 10) {
        clock += '0';
    }
    clock += mm;

    if (ss < 10) {
        clock += '0';
    }
    clock += ss;
    return clock;
}
function CreateNum() {
    var Num = "";
    for (var i = 0; i < 8; i++) {
        Num += Math.floor(Math.random() * 10);
    }
    return Num;
}
$(function () {
    Countloading(_PKID);
})
var _hmt = _hmt || []; (function () { var t = document.createElement("script"), n; t.async = !0; t.src = "//hm.baidu.com/hm.js?16afd1d01373c6f2bf619ff824868a5e"; n = document.getElementsByTagName("script")[0]; n.parentNode.insertBefore(t, n) })(), function (n, t, i, r, u, f, e) { n.GoogleAnalyticsObject = u; n[u] = n[u] || function () { (n[u].q = n[u].q || []).push(arguments) }; n[u].l = 1 * new Date; f = t.createElement(i); e = t.getElementsByTagName(i)[0]; f.async = 1; f.src = r; e.parentNode.insertBefore(f, e) }(window, document, "script", "//www.google-analytics.com/analytics.js", "ga"); ga("create", "UA-53906036-1", "auto"); ga("send", "pageview");
//</script> 