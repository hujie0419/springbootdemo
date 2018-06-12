function Base64() {
    // private property
    var _keyStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

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
    var _utf8_encode = function (string) {
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
    var _utf8_decode = function (utftext) {
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
    Data: null,
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
    Init: function (ids) {
        var source = APPShare.GetQueryString("utm_source");
        var topicId = APPShare.GetQueryString("id");
        var appUrl = 'tuhu://tuhu.cn/bbs/topic?id=' + topicId;
        var _data = {
            url: appUrl
        }

        APPShare.Data = this.base64encode(JSON.stringify(_data));

        var results = [];

        if (ids.length > 0) {
            for (var i = 0; i < ids.length; i++) {
                results.push({
                    mlink: 'https://a.mlinks.cc/AakR',
                    button: document.querySelector(ids[i]),
                    params: { data: APPShare.Data }
                })
            }

            new Mlink(results);
        }
    },
    OpenApp: function () {
        var appUrl = APPShare.GetToUrl();
        if (appUrl) {
            Ta.Run("home_click_打开论坛app", "event");
            location.href = appUrl;
        }
    },
    GetToUrl: function () {
        var topicId = APPShare.GetQueryString("id")
        var title = document.title;
        var defaultImg = 'https://faxian.tuhu.cn/Content/imgs/detail/drawable-hdpi/hu-2x.png';
        var params = {
            'shareUrl': 'http://faxian.tuhu.test/article/forumshare?id=' + topicId,
            'id': topicId,
            'shareImage': defaultImg,
            'shareTitle': title,
            'shareDescrip': title,
            'URL': 'tuhu://tuhu.cn/bbs/topic?id=' + topicId
        }
        return params.URL;
    }
}

//初始化
APPShare.Init(['a#backApp1', 'a#backApp2']);
