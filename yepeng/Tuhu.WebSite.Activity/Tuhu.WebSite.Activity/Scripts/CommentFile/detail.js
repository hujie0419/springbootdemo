
var user = null;
var isFirstEnter = true;
var DomainHost = "http://" + location.host;
$(document).ready(function () {
    getSize();
    var relate_list = $("#xgyd").children(".floor1");
    var source = APPShare.GetQueryString("utm_source");
    if (relate_list.length > 1) {
        relate_list.last().addClass("foot-top");                
        if (source != "" && source != undefined) {
            relate_list.last().css("margin-bottom", "3px");
        }
    }
    var userAgentInfo = navigator.userAgent;
    if (userAgentInfo.indexOf("tuhuAndroid") >= 0) {
        getAndroidUserInfo('loginBridge', userInfoForAndroid);
    }
    else if (userAgentInfo.indexOf("tuhuIOS") >= 0) {
        getIOSUserInfo(0, 'userInfoCallback');
    }

    if (Utils.IsAppNew()) {
        $("footer.footer").show();
    }

    //setTimeout(function () {
    //    $('li.swiper-slide img').each(function () {
    //        new RTP.PinchZoom($(this), {});
    //    });
    //}, 4000);
   
    //**************************点击事件******************************//

    //点击相关阅读
    $(".relateds").click(function () {
        var thisObj = $(this);
        var articleId = thisObj.attr("aid");
        var title = thisObj.find(".shenglu").text();
        var href = thisObj.attr("href");
        var img = thisObj.find("img").length <= 0 ? "" : thisObj.find("img").attr("src");
        ga('send', 'event', 'toutiao_correlation', 'click_article', articleId, thisObj.index());
        Ta.Run("home_click_相关文章", "event", "ArticleId", articleId);
        var url = DomainHost + href;
        if (userAgentInfo.indexOf("tuhuAndroid") >= 0) {
            var android = {};
            var defaultImg = "http://" + window.location.host + '/Content/imgs/detail/drawable-hdpi/hu-2x.png';
            android.androidValue = [{
                Url: url,
                shareImage: img || defaultImg,
                shareUrl: url,
                shareDescrip: title,
                shareTitle: title,
                isShowShareIcon: false,
                type: 8
            }];
            android.androidKey = "cn.TuHu.Activity.AutomotiveProducts.AutomotiveProductsWebViewUI";
            console.log(android);
            window.WebViewJavascriptBridge.callHandler('jumpActivityBridge', encodeURIComponent(encodeURIComponent(JSON.stringify(android))));
        }
        else if (userAgentInfo.indexOf("tuhuIOS") > 0) {
            window.location.href = 'tuhuaction://segue#TNHBH5VViewController#{"url": "' + url + '"}';
        } else {
            window.location.href = url;
        }

    });
    //点击标签
    $(".tag-item").each(function () {
        $(this).click(function () {
            var $this = $(this);
            var itemid = $this.attr("data-itemid");
            var title = $(this).text().trim();
            ga('send', 'event', 'toutiao_detail', 'click_tag', itemid, $(this).index() + 1);
            if (itemid != undefined && itemid != "") {     
                if (userAgentInfo.indexOf("tuhuAndroid") >= 0) {
                    var android = {};
                    var defaultImg = "http://" + window.location.host + '/Content/imgs/detail/drawable-hdpi/hu-2x.png';
                    var urlAndroid = DomainHost + "/firstline?ArticleTagId=" + itemid + "&title=" + title;
                    android.androidValue = [{
                        Url: urlAndroid,
                        shareImage: defaultImg,
                        shareUrl: urlAndroid,
                        shareDescrip: title,
                        shareTitle: title,
                        isShowShareIcon: false,
                        type: 8
                    }];
                    android.androidKey = "cn.TuHu.Activity.AutomotiveProducts.AutomotiveProductsWebViewUI";
                    console.log(android);
                    window.WebViewJavascriptBridge.callHandler('jumpActivityBridge', encodeURIComponent(encodeURIComponent(JSON.stringify(android))));
                }
                else if (userAgentInfo.indexOf("tuhuIOS") > 0) {
                    var urlIOS = DomainHost + "/firstline?ArticleTagId=" + itemid + "&title=" + encodeURIComponent(encodeURIComponent(title));
                    window.location.href = 'tuhuaction://segue#TNHBH5VViewController#{"url": "' + urlIOS + '"}';
                }
            }
        });
    });
    //点击分享
    $("#shareImage").click(function () {
        var userAgentInfo = navigator.userAgent;
        var _articleId = $("#ArticleId").val();
        var _articleTitle = $("#ArticleTitle").val();
        CollectShareTimes();
        Ta.Run("home_click_分享", "event");
        ga('send', 'event', 'toutiao_detail', 'click_share');
        var url = "http://" + window.location.host + "/Article/Detail?id=" + _articleId + "&utm_medium=TOUTIAO&utm_campaign=USER_SHARE";
        var content = $("#ArticleDes").val() || _articleTitle;
        var defaultImg = "http://" + window.location.host + '/Content/imgs/detail/drawable-hdpi/hu-2x.png';
        var image = $("#ArticleCoverImage").val();

        if (userAgentInfo.indexOf("tuhuAndroid") >= 0) {
            var android = {};
            android.share_media = ["QQ", "SINA", "QZONE", "WEIXIN", "WEIXIN_CIRCLE"];

            android.Url = url;
            android.Title =_articleTitle;
            android.Description = content;
            android.Picture = image || defaultImg;
            android.type = 2;

            window.WebViewJavascriptBridge.callHandler('toShareMedia', encodeURIComponent(encodeURIComponent(JSON.stringify(android))));
        }
        else if (userAgentInfo.indexOf("tuhuIOS") > 0) {                   
            window.location.href = 'tuhuaction://share#63#{"url": "' + url + '","title":"' + _articleTitle + '","content":"' + content + '","image":"' + (image || defaultImg) + '"}';
        }                
    });
    //点击收藏
    $("#favoriate").click(function () {
        if (userAgentInfo.indexOf("tuhuAndroid") >= 0) {
            getAndroidUserInfo('actityBridge', AddFavorite);
        }
        else if (userAgentInfo.indexOf("tuhuIOS") > 0) {
            getIOSUserInfo(1, 'AddFavorite');
        }
    });
    //点击查看所有评论
    $("#openAllComment").click(function () {
        var articleId = $("#ArticleId").val();
        ga('send', 'event', 'toutiao_detail', 'click_commentnumber');
        Ta.Run("home_click_评论列表", "event");
        if (userAgentInfo.indexOf("tuhuAndroid") >= 0) {
            var android = {};
            android.androidValue = [{
                PKID: articleId,
                Category: "",
                Title: document.title,
                keyboard: 1,
                AddClick: false,
                isNew: false
            }];
            android.androidKey = "cn.TuHu.Activity.Found.DiscoveryH5Activity";
            console.log(android);
            window.WebViewJavascriptBridge.callHandler('jumpActivityBridge', encodeURIComponent(encodeURIComponent(JSON.stringify(android))));
        }
        else if (userAgentInfo.indexOf("tuhuIOS") > 0) {
            window.location.href = 'tuhuaction://segue#THDiscoverCommentVC#{"categoryName": "","mytitle":"' + document.title + '","pkid":"' + articleId + '","showKeyboard":0}';
        }
    });
    //点击跳转到评论列表页
    $('#argueState').click(function (event) {
        var articleId = $("#ArticleId").val();
        ga('send', 'event', 'toutiao_detail', 'click_commentnumber');
        Ta.Run("home_click_评论列表", "event");
        if (userAgentInfo.indexOf("Android") >= 0) {
            var android = {};
            android.androidValue = [{
                PKID: articleId,
                Category: "",
                Title: document.title,
                keyboard: 1,
                AddClick: false,
                isNew: false
            }];
            android.androidKey = "cn.TuHu.Activity.Found.DiscoveryH5Activity";
            console.log(android);
            window.WebViewJavascriptBridge.callHandler('jumpActivityBridge', encodeURIComponent(encodeURIComponent(JSON.stringify(android))));
        }
        else if (userAgentInfo.indexOf("tuhuIOS") > 0) {
            window.location.href = 'tuhuaction://segue#THDiscoverCommentVC#{"categoryName": "","mytitle":"' + document.title + '","pkid":"' + articleId + '","showKeyboard":0}';
        }
    });
    //点击发表评论
    $("#addComment").click(function () {
        var articleId = $("#ArticleId").val();
        ga('send', 'event', 'toutiao_detail', 'click_publish_comment');
        Ta.Run("home_click_发表评论", "event");
        if (userAgentInfo.indexOf("tuhuAndroid") >= 0) {
            var android = {};
            android.androidValue = [{
                PKID: articleId,
                Category: "",
                Title: document.title,
                keyboard: "0",
                AddClick: false,
                isNew: false
            }];
            android.androidKey = "cn.TuHu.Activity.Found.DiscoveryH5Activity";
            console.log(android);
            window.WebViewJavascriptBridge.callHandler('jumpActivityBridge', encodeURIComponent(encodeURIComponent(JSON.stringify(android))));
        }
        else if (userAgentInfo.indexOf("tuhuIOS") > 0) {
            window.location.href = 'tuhuaction://segue#THDiscoverCommentVC#{"categoryName": "","mytitle":"' + document.title + '","pkid":"' + articleId + '","showKeyboard":1}';
        }
    });
    //点击头像&昵称
    $(".toUserCenter").click(function () {
        var thisObj = $(this);
        var userid = thisObj.parents(".commentCon").attr("code");
        var uid = userid != "" ?userid : "";
        if (userAgentInfo.indexOf("tuhuAndroid") >= 0) {
            var android = {};
            android.androidValue = [{ "userId": uid }];
            android.androidKey = "cn.TuHu.Activity.Found.PersonalPage.OnePageActivity";
            console.log(android);
            window.WebViewJavascriptBridge.callHandler('jumpActivityBridge', encodeURIComponent(encodeURIComponent(JSON.stringify(android))));
        }
        else if (userAgentInfo.indexOf("tuhuIOS") > 0) {
            window.location.href = 'tuhuaction://segue#Tuhu.THDiscoverPersonVC#{"userId":"' + uid + '"}';
        }
    });

    if (source == undefined || source == "") {
        //点击作者
        $("#author").click(function () {
            var title = $(this).text().trim();
            ga('send', 'event', 'toutiao_detail', 'click_author', title);
            window.location.href = DomainHost + "/firstline?ArticleAuthor=" + $(this).text().trim() + "&title=" + title;
        });
    } else {
        $("#author").removeClass("aucolor");
    }
    //图片处理事件
    //if (window.location.host.indexOf("tuhu.cn") < 0) {
    //    $(".section  .de-page").each(function (i) {
    //        $(this).find("img").each(function (i) {
    //            var imgObj = $(this);
    //            var src = imgObj.attr("data-original") || "";
    //            imgObj.attr("src", src);
    //        });
    //    });
    //}

});
function androidLoginSuccessFuc() {
    //alert('登录成功回掉方法');            
}
//跳转到评论页（已废弃）
function GoComment() {
    var url = DomainHost + "/Article/Comment?Aid=" + $("#ArticleId").val() + "&UserId=" + (user.userid || "");
    if (userAgentInfo.indexOf("tuhuAndroid") >= 0) {
        var android = {};
        android.androidValue = [{ "Url": url }];
        android.isShowShareIcon = false;
        android.androidKey = "cn.TuHu.Activity.AutomotiveProducts.AutomotiveProductsWebViewUI";
        console.log(android);
        window.WebViewJavascriptBridge.callHandler('jumpActivityBridge', encodeURIComponent(encodeURIComponent(JSON.stringify(android))));
    }
    else if (userAgentInfo.indexOf("iPhone") > 0) {
        window.location.href = 'tuhuaction://segue#TNHBH5VViewController#{"url": "' + url + '"}';
    }
}
function CollectShareTimes() {
    $.ajax({
        type: 'post',
        url: "/Article/CollectShareTimes",
        dataType: 'json',
        data: { Id: $("#ArticleId").val(), userId: (user && user.userid || ""), DeviceId: (user && user.uuid || "") },
        success: function (response) {
        }
    });
}
function AddFavorite(userInfo) {
    if (typeof (userInfo) == 'string') {
        user = JSON.parse(userInfo);
    } else {
        user = userInfo;
    }
    if ($("#favoriate").hasClass("czbg")) {//取消收藏
        var voteCount = $("#favCount").text();
        if (voteCount != "") {
            var count = parseInt(voteCount) - 1;
            $("#favCount").text(count);
        }        
        setTimeout(function () {
            $(".collected").fadeOut("slow");
        }, 1000);//等待1秒执行
        $(".collected").show();
        $(".collected p").text("取消成功");
        $("#favoriate").removeClass("czbg").removeClass("shake");
        ga('send', 'event', 'toutiao_detail', 'click_cancel_favorite');
        Ta.Run("home_click_取消收藏", "event");
    } else {//点击收藏
        var favCount = $("#favCount").text();
        if (favCount == "") {
            $("#favCount").text(1);
        } else {
            var count = parseInt(favCount) + 1;
            $("#favCount").text(count);
        }
        setTimeout(function () {
            $(".collected").fadeOut("slow");
        }, 1000);//等待1秒执行
        $(".collected").show();
        $(".collected p").text("收藏成功");
        $("#favoriate").addClass("czbg").addClass("shake");
        ga('send', 'event', 'toutiao_detail', 'click_favorite');
        Ta.Run("home_click_收藏", "event");
    }
    $.ajax({
        type: 'post',
        url: "/Article/AddFavoriate",
        dataType: 'json',
        data: { Id: $("#ArticleId").val(), userId: user.userid, DeviceId: user.uuid },
        success: function (response) {
        }
    });
    //getIsFavor(function (flag) {
    //    if ($("#favoriate").hasClass("czbg") && flag
    //        || !$("#favoriate").hasClass("czbg") && !flag) {
    //        $.ajax({
    //            type: 'post',
    //            url: "/Article/AddFavoriate",
    //            dataType: 'json',
    //            data: { Id: $("#ArticleId").val(), userId: user.userid, DeviceId: user.uuid },
    //            success: function (response) {
    //                if (response.success == true) {
    //                    if ($("#favoriate").hasClass("czbg")) {//取消收藏
    //                        var count = parseInt($("#favCount").text()) - 1;
    //                        $("#favCount").text(count);
    //                        setTimeout(function () {
    //                            $(".collected").fadeOut("slow");
    //                        }, 1000);//等待1秒执行
    //                        $(".collected").show();
    //                        $(".collected p").text("取消成功");
    //                        $("#favoriate").removeClass("czbg").removeClass("shake");
    //                        ga('send', 'event', 'toutiao_detail', 'click_cancel_favorite');
    //                        Ta.Run("home_click_取消收藏", "event");
    //                    } else {//点击收藏
    //                        var favCount = $("#favCount").text();
    //                        if (favCount == "") {
    //                            $("#favCount").text(1);
    //                        } else {
    //                            var count = parseInt(favCount) + 1;
    //                            $("#favCount").text(count);
    //                        }                              
    //                        setTimeout(function () {
    //                            $(".collected").fadeOut("slow");
    //                        }, 1000);//等待1秒执行
    //                        $(".collected").show();
    //                        $(".collected p").text("收藏成功");
    //                        $("#favoriate").addClass("czbg").addClass("shake");
    //                        ga('send', 'event', 'toutiao_detail', 'click_favorite');
    //                        Ta.Run("home_click_收藏", "event");
    //                    }
    //                }
    //            }
    //        });
    //    } else {
    //        setTimeout(function () {
    //            $(".collected").fadeOut("slow");
    //        }, 1000);//等待1秒执行
    //        $(".collected").show();
    //        $(".collected p").text("收藏成功");
    //        $("#favoriate").addClass("czbg").addClass("shake");
    //    }
    //});             
}
function getIsFavor(callback) {
    $.ajax({
        type: 'get',
        url: "/Article/IsFavoriate",
        dataType: 'json',
        data: { Id: $("#ArticleId").val(), userId: (user && user.userid || ""), DeviceId: (user && user.uuid || "") },
        success: function (response) {
            if (response.success == true) {
                callback(response.IsFavoriate);
            }
        }
    });
}
function getIsFavoriate() {
    var flag = false;
    $.ajax({
        type: 'get',
        url: "/Article/IsFavoriate",
        dataType: 'json',
        data: { Id: $("#ArticleId").val(), userId: (user && user.userid || ""), DeviceId: (user && user.uuid || "") },
        async:false,
        success: function (response) {
            if (response.success == true) {
                if (response.IsFavoriate == true) {
                    $("#favoriate").addClass("czbg");
                    flag = true;
                }
            }
        }
    });
    return flag;
}
function getIOSUserInfo(type, callback) {
    window.location.href = 'tuhuaction://getuserinfo#' + type + '#' + callback;
}
function userInfoCallback(userInfo) {
    user = userInfo;
    if (isFirstEnter) {
        getIsFavoriate();
        isFirstEnter = false;
    }
}
function userInfoForAndroid(responseData) {
    var userInfo = $.parseJSON(responseData);
    user = userInfo;
    if (isFirstEnter) {
        getIsFavoriate();
        isFirstEnter = false;
    }
}
function getAndroidUserInfo(type, callback) {
    var android = {};
    window.WebViewJavascriptBridge.callHandler(type, encodeURIComponent(encodeURIComponent(JSON.stringify(android))), callback);
}

function getSize() {
    var oHtml = document.documentElement;
    var screenWidth = oHtml.clientWidth;
    if (screenWidth > 630) {
        oHtml.style.fontSize = '78.75px';
    } else if (screenWidth <= 320) {
        oHtml.style.fontSize = '40px';
    } else {
        oHtml.style.fontSize = screenWidth / 320 * 40 + 'px';
    }
}
//获取滚动条当前的位置
function getScrollTop() {
    var scrollTop = 0;
    if (document.documentElement && document.documentElement.scrollTop) {
        scrollTop = document.documentElement.scrollTop;
    }
    else if (document.body) {
        scrollTop = document.body.scrollTop;
    }
    return scrollTop;
}
//获取当前可是范围的高度
function getClientHeight() {
    var clientHeight = 0;
    if (document.body.clientHeight && document.documentElement.clientHeight) {
        clientHeight = Math.min(document.body.clientHeight, document.documentElement.clientHeight);
    }
    else {
        clientHeight = Math.max(document.body.clientHeight, document.documentElement.clientHeight);
    }
    return clientHeight;
}
//获取文档完整的高度
function getScrollHeight() {
    return Math.max(document.body.scrollHeight, document.documentElement.scrollHeight);
}

/*滑动一段距离，顶部消失与显示*/
var tipObj = $(".load-tip");
var topHeight = tipObj.height();
var scrolltop = new Array();
var i = 0;
scrolltop[0] = 0;
//滑动事件
window.onscroll = function () {
    i++;
    scrolltop[i] = getScrollTop();
    if (scrolltop[i] > scrolltop[i - 1] && getScrollTop() > topHeight) {
        tipObj.fadeOut(600);
    } else {
        tipObj.fadeIn(800);
    };
    //文章看完（标签上面）
    if (getScrollTop() + getClientHeight() >= $("#bq_list").offset().top + 72) {
        var tagTop = $("#_hdScrollEventTagTop").val();
        if (tagTop == "false") {
            $("#_hdScrollEventTagTop").val(true);
            ga('send', 'event', 'toutiao_detail', 'read_article_complete');
        }
    }
    //看到相关阅读第一篇
    if (getScrollTop() + getClientHeight() >= $("#xgyd").offset().top + 172) {
        var relatedTag = $("#_hdScrollEventRelated").val();
        if (relatedTag == "false") {
            $("#_hdScrollEventRelated").val(true);
            ga('send', 'event', 'toutiao_detail', 'browse_first_correlation');
        }
    }
    //滑到底部
    if (getScrollTop() + getClientHeight() == getScrollHeight()) {
        var bottomTag = $("#_hdScrollEventBottom").val();
        if (bottomTag == "false") {
            $("#_hdScrollEventBottom").val(true);
            ga('send', 'event', 'toutiao_detail', 'browse_page_complete');
        }
    }
}
window.onresize = function () {
    getSize();
}

//var isvalid = true;
//$(".section  .de-page").each(function (i) {
//    $(this).find("img").each(function (i) {
//        if (!(this.src.indexOf("http://img.tuhu.cn") >= 0 || this.src.indexOf("http://image.tuhu.cn") >= 0)) {
//            isvalid = false;
//            return false;
//        }
//    });
//});
//if (!isvalid) {
//    alert('图片必须以http://img.tuhu.cn或http://image.tuhu.cn开始');
//}
 
