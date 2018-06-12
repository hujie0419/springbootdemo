

$.fn.tmpl = function (a) {
    var b = $(this[0]).html().trim();
    if (a) {
        for (k in a) {
            b = b.replace(new RegExp("\\${" + k + "}", "g"), a[k])
        }
    }
    return $(b)
};

Date.prototype.Format = function () {
    return [this.getFullYear(), this.getMonth() + 1, this.getDate()].join('.');
};

//评论内容加载更多
$(".comment").on("tap", ".content .arrowmore", function () {
    $(this).prev(".com").css({ overflow: "visible", "-webkit-line-clamp": "initial", "max-height": "none" });
    $(this).hide();
});

var thisPraise = "";
//点赞
$(".comment").on("tap", ".goodnum", function (e) {
    CommentId = $(this).attr("pkid");
    thisPraise = $(this);

    if (userAgentInfo.indexOf("Android") >= 0) {
        if (isAndroidVersion) {
            Login()
            if (UserId) {
                PraiseClick();
            }
        } else if (userId != undefined && userId != "undefined" && userId != "" && userId != null && userId != "null") {
            PraiseClick();
        }
    } else {
        if (isIosVersion) {

            Login()
            if (UserId) {
                PraiseClick();
            }
        } else if (userId != undefined && userId != "undefined" && userId != "" && userId != null && userId != "null" && userId != "(null)") {
            PraiseClick();
        }
    }


    e.stopPropagation();
});

//name评论
$(".comment").on("tap", ".name", function (e) {

    if (userAgentInfo.indexOf("Android") >= 0) {
        if (isAndroidVersion) {
            Commented(this);
        }
    } else {

        if (isIosVersion) {
            Commented(this);
        }
    }


});
//date评论
$(".comment").on("tap", ".date", function (e) {
    if (userAgentInfo.indexOf("Android") >= 0) {
        if (isAndroidVersion) {
            Commented(this);
        }
    } else {

        if (isIosVersion) {
            Commented(this);
        }
    }


});

//评论
$(".comment").on("tap", ".replaynum", function (e) {

    if (userAgentInfo.indexOf("Android") >= 0) {
        if (isAndroidVersion) {
            Commented(this);
        }
    } else {

        if (isIosVersion) {
            Commented(this);
        }
    }

    e.stopPropagation();
});

//内容评论
$(".comment").on("tap", ".com", function (e) {

    if (userAgentInfo.indexOf("Android") >= 0) {
        if (isAndroidVersion) {
            Commented(this);
        }
    } else {

        if (isIosVersion) {
            Commented(this);
        }
    }
    e.stopPropagation();
});

function Commented(e) {
    Login()
    if (UserId) {

        var num = parseInt($(e).text());
        if (num >= 1000) {
            $(e).html(999 + "<sup>+</sup>");
        } else {
            //$(this).html(num + 1);
        }

        CommentClick(e);
    }
}
var CommentId = 0;
function PraiseClick() {

    $.ajax({

        url: '/Home/InsertCommentPraise',

        type: 'POST',

        data: {
            "CommentId": CommentId,
            "userId": userId,
            "UserHead": UserObjectJson.HeadImage,
            "UserName": UserObjectJson.UserName,
            "RealName": UserObjectJson.RealName,
            "Sex": UserObjectJson.Sex,
            "PhoneNum": Phone
        },

        dataType: 'json',

        async: true,

        success: function (data) {

            if (data != 200) {
                var num = parseInt(thisPraise.text());
                if (num >= 1000) {
                    thisPraise.html(999 + "<sup>+</sup>");
                } else {
                    thisPraise.html(num + 1);
                }

                thisPraise.addClass("selected");
            }
        }
    })
}


function CommentClick(e) {

    var androidPrams = { "id": $(e).attr("pkid"), "name": $(e).attr("username") }

    if (userAgentInfo.indexOf("Android") >= 0) {
        if (isAndroidVersion) {
            window.WebViewJavascriptBridge.callHandler('toDiscoveryReplay',
                encodeURIComponent(encodeURIComponent(JSON.stringify(androidPrams))));
        }
    } else {
        if (isIosVersion) {
            window.location.href = "tuhuDiscover://reply#" + $(e).attr("pkid") + "#" + $(e).attr("username");
        }
    }
}

var UserObjectJson = {};
function GetUserObject(userId) {

    $.ajax({

        url: 'http://api.tuhu.cn/User/SelectUserObjectForH5',

        type: 'GET',

        data: { "userId": userId },

        dataType: 'jsonp',

        async: false,

        success: function (data) {

            if (data.Code == "1") {
                UserObjectJson = data.UserObject[0];
            }
        }
    })
}

var myScroll, index = 1, len = 0, pageSize = 10;
var $pullUpEl = $('#pullUpEl');

//TODO
//var userId = '{93B56317-E0FC-4B58-9F84-467462F4F02A}';
//var pkid = 2171;

//最新评论数据
function RequestData(index) {
    $pullUpEl.addClass('loading').text('加载中...');
    myScroll && (myScroll.refresh(), console.log(myScroll));
    return $.post('/home/ArticleCommentV1.html', { PKID: pkid, PageIndex: index, UserID: userId });
}

//热门评论数据3条数据
function RequestDataTop3() {
    return $.post('/home/ArticleCommentTop3.html', { PKID: pkid, PageIndex: index, UserID: userId });
}

var AppendComment = (function ($tmpl, $comment) {
    return function (d) {

        $tmpl.tmpl(d).appendTo($comment);

    }
})($('#tmplArticleComment'), $('#comment'));

var AppendCommentTop3 = (function ($tmpl, $comment) {
    return function (d) {

        $tmpl.tmpl(d).appendTo($comment);

    }
})($('#tmplArticleComment'), $('#comment1'));


//处理最新评论数据 返回数据长度
function ProcessData(data) {
    var data = typeof data === 'string' ? JSON.parse(data) : data;
    $pullUpEl.removeClass('loading')
    if ($.isArray(data)) {
        data.forEach(function (d, i) {
            for (var o in d) {
                try {
                    d[o] = decodeURIComponent(d[o]);
                }
                catch (e) {
                    d[o] = d[o];
                }

                if (d.hasOwnProperty(o)) {
                    switch (o) {
                        case 'UserGrade':
                            d.UserGrade || (d.UserGrade = '布老虎会员');
                            break;
                        case 'UserHead':
                            d.UserHead || (d.UserHead = 'http://resource.tuhu.cn/Image/Product/bulaohu.png');
                            break;
                        case 'CommentTime':
                            d.CommentTime = d.CommentTime;
                            break;
                        case 'floor':
                            switch (d.floor) {
                                case '1':
                                    d.floor = '沙发';
                                    break;
                                case '2':
                                    d.floor = '板凳';
                                    break;
                                case '3':
                                    d.floor = '地板';
                                    break;
                                default:
                                    d.floor += '楼';
                                    break;
                            }
                            break;
                    }
                }
            }
            AppendComment(d);
        });
        return data.length;
    } else {
        return 0;
    }
}

//处理热门评论数据 返回数据长度
function ProcessDataTop3(data) {
    var data = typeof data === 'string' ? JSON.parse(data) : data;
    $pullUpEl.removeClass('loading')
    if ($.isArray(data)) {
        data.forEach(function (d, i) {
            for (var o in d) {
                try {
                    d[o] = decodeURIComponent(d[o]);
                }
                catch (e) {
                    d[o] = d[o];
                }

                if (d.hasOwnProperty(o)) {
                    switch (o) {
                        case 'UserGrade':
                            d.UserGrade || (d.UserGrade = '布老虎会员');
                            break;
                        case 'UserHead':
                            d.UserHead || (d.UserHead = 'http://resource.tuhu.cn/Image/Product/bulaohu.png');
                            break;
                        case 'CommentTime':
                            d.CommentTime = d.CommentTime;
                            break;
                        case 'floor':
                            switch (d.floor) {
                                case '1':
                                    d.floor = '沙发';
                                    break;
                                case '2':
                                    d.floor = '板凳';
                                    break;
                                case '3':
                                    d.floor = '地板';
                                    break;
                                default:
                                    d.floor += '楼';
                                    break;
                            }
                            break;
                    }
                }
            }
            AppendCommentTop3(d);
        });
        return data.length;
    } else {
        return 0;
    }
}

//根据数据长度 判断是否加载
function EndLoad() {
    if (len >= pageSize) {
        $pullUpEl.text('加载中...');
        myScroll.refresh();
    } else {
        if (index == 1 && len == 0) {

        } else {
            $pullUpEl.removeClass('loadmore').addClass('noneComment').html('无更多评论...');
        }
        myScroll.refresh();
    }
}

document.addEventListener('touchmove', function (e) { e.preventDefault(); }, false);


function ChangeCss() {

    $(".comment .item").each(function (i) {

        if ($(this).find(".goodnum").attr("ispraise") == "1") {

            $(this).find(".goodnum").addClass("selected")
        }

        if ($(this).find(".com").height() >= 85) {

            $(this).find(".arrowmore").css("display", "block")
        }
    })

    ////TODO
    //if (userAgentInfo.indexOf("Android") >= 0) {
    //    if (!isAndroidVersion) {
    //        $(".replay").hide();
    //    }
    //} else {
    //    if (!isIosVersion) {
    //        $(".replay").hide();
    //    }
    //}
}

function HideHot() {

    if ($(".comment .item").eq(0).find(".goodnum").text() >= 1 && CountZuixin > 10) {

        $("#newcomment1").show();
        $("#comment1").show();

    }

    if ($("#noComment").css("display") == "block") {

        $("#newcomment").hide();
    }
}

function GetCount() {
    $.ajax({

        url: '/Home/GetCountCommentBYId',

        type: 'Post',

        data: { "UserId": userId, "PKID": pkid },

        dataType: 'json',

        async: false,

        success: function (data) {

            CountZuixin = data;
        }
    })
}

var CountZuixin = 0;

$(function () {

    GetUserObject(userId);

    GetCount();

    RequestDataTop3().then(function (data3) {

        ProcessDataTop3(data3);
        ChangeCss();
        HideHot();
    })
    //请求首屏数据
    RequestData(index).then(function (data) {

        len = ProcessData(data);
        if (len == 0) {
            $pullUpEl.parent().remove();

            $('#noComment').show();
            return;
        }
        myScroll = new IScroll('#wrapper', {
            mouseWheel: true,
            //eventPassthrough: true,
            //scrollX: true,
            //scrollY: false,
            //preventDefault: false
            //hScroll: true,
            //vScroll: true

        });

        myScroll.on('scrollEnd', function () {
            if (myScroll.y === myScroll.maxScrollY && !$pullUpEl.hasClass('noneComment')) {//滚动到底部且有数据加载数据
                RequestData(++index).then(function (data) {
                    len = ProcessData(data);
                    EndLoad();
                    ChangeCss();
                })
            }
        });
        ChangeCss();
        EndLoad();

    });

})

var userAgentInfo = navigator.userAgent;

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

//判断登录
function Login() {
    if (userAgentInfo.indexOf("Android") >= 0) {
        window.WebViewJavascriptBridge.callHandler(
            'actityBridge'
            , { 'param': "" }
            , function (responseData) {

                if (responseData != null) {
                    CallBackAppLoginResponse(responseData);
                }
            });
    }
    else {

        iosend("Userinfo", "CallBackAppLoginResponse");
        //iosend("aboutTuhuUserinfologin", "CallBackAppLoginResponse");
    }
};

function CallBackAppLoginResponse(responseData) {
    var da = null;
    if (userAgentInfo.indexOf("Android") >= 0) {
        da = JSON.parse(responseData);
    }
    else {
        da = responseData;
    }
    UserId = da.userid;
    Phone = da.phone;

};
//进入iosApp
function iosend(cmd, arg) {
    location.href = '#testapp#' + cmd + '#' + encodeURIComponent(arg);
};

var UserId = "";
var Phone = "";
var isIosVersion = false;
var isAndroidVersion = false;

//IOS版本判断
function VersionForIos(version) {
    isIosVersion = true;
}

//安卓版本判断
function VersionForAndroid(version) {
    isAndroidVersion = true;
}

//写cookie
function SetCookie(name, value) {
    var OneDayTimes = 60 * 60 * 24 * 1000;
    var exp = new Date();
    exp.setTime(exp.getTime() + OneDayTimes * 30);
    document.cookie = name + "=" + escape(value) + ";path=/;expires=" + exp.toGMTString();
}
//获取cookie
function GetCookie(name) {
    var arr = document.cookie.match(new RegExp("(^| )" + name + "=([^;]*)(;|$)"));
    if (arr != null) return unescape(arr[2]); return null;
}
