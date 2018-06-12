var _Config = {
    SetCookie: function (name, value) {
        var Days = 3600;
        var exp = new Date();
        //  domain = "tuhu.cn";
        exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);
        document.cookie = name + "=" + escape(value) + ";path=/;expires=" + exp.toGMTString();
    },
    GetCookie: function (name) {
        var arr = document.cookie.match(new RegExp("(^| )" + name + "=([^;]*)(;|$)"));
        if (arr != null) return unescape(arr[2]); return null;
    },
    DelCookie: function (name) {
        var exp = new Date();
        exp.setTime(exp.getTime() - 1);
        var cval = getCookie(name);
        if (cval != null)
            document.cookie = name + "=" + cval + ";expires=" + exp.toGMTString() + "; path=/";
    }
};
var _H5Utils = {
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
        return navigator.userAgent.indexOf('tuhuAndroid') >= 0 ? true : false;
    },
    IsIOS: function () {
        return navigator.userAgent.indexOf('tuhuIOS') >= 0 ? true : false;
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
var Ta = {
    devices: [
            'other iphone',
            'Apple iPhone 6 Plus / 6s Plus',
            'Apple iPhone 6 / 6s',
            'Apple iPhone 4, 4s, 5, 5c, 5s',
            'android device'
    ],
    appNames: ['tuhuAndroid', 'tuhuIOS', ''],
    osNames: ['Android', 'iOS'],
    WXSid: 'WXSid',
    WXUid: 'WXUid',
    userid: '',
    BASE_URL: "http://faxian.tuhu.dev",
    STATIC_URL_SPV: "https://hi.tuhu.com/collect/spv",
    STATIC_URL_SEVENT: "https://hi.tuhu.com/collect/sevent",
    eventAction: null,
    eventType: null,
    metadata: null,
    isFirstInTa: function () {
        return document.cookie.indexOf('TA') < 0
    },
    handleTaData: function (data) {
        data.client_time = Math.floor(+new Date() / 1000)

        if (document.referrer) {
            data.refer = document.referrer.substr(0, 512);
        };

        data.url = window.location.href.substr(0, 512);

        if (data.event_action) {
            data.event_action = data.event_action.substr(0, 512);
        }

        if (data.event_category) {
            data.event_category = data.event_category.substr(0, 32);
        }

        data.sid = _Config.GetCookie('sid') || '';

        data.uuid = _Config.GetCookie('uuid') || '';

        if (this.isFirstInTa()) {
            data.app_version = this.appVersion();
            data.app_name = this.appName();
            data.screen_resolution = this.screenResolution();
            data.os_name = this.osName();
            data.os_version = this.osVersion();
            data.device_model = this.deviceModel();
            data.lang = this.lang();
            data.country = this.country();

            _Config.SetCookie('TA', '8868');
        }

        return data;
    },
    appVersion: function () {
        if (_H5Utils.IsAppNew()) {
            if (_H5Utils.IsAndroid()) {
                return 'Android' + window.isVersion;
            }
            return window.isVersion;
        }
        return ''
    },
    appName: function () {
        if (_H5Utils.IsAppNew()) {
            if (_H5Utils.IsAndroid()) {
                return this.appNames[0];
            }
            return this.appNames[1];
        }
        return this.appNames[2];
    },
    lang: function () {
        return navigator.language.split('-')[0]
    },
    country: function () {
        return navigator.language.split('-')[1]
    },
    screenResolution: function () {
        return window.innerWidth + 'x' + window.innerHeight;
    },
    osName: function () {
        if (_H5Utils.IsAndroid()) {
            return this.osNames[0]
        }

        return this.osNames[1]
    },
    osVersion:function() {
        //if (_H5Utils.IsAndroid()) {
        //    return navigator.userAgent.match(/Android\s\d\.\d/)[0].split(' ')[1]
        //}
        //return navigator.userAgent.match(/\s\d\_\d/)[0].replace('_', '.');
        return "";
    },
    deviceModel: function () {
        if (_H5Utils.IsIOS()) {
            switch (window.innerWidth) {
                case 414:
                    return this.devices[1]

                case 375:
                    return this.devices[2]

                case 320:
                    return this.devices[3]

                default:
                    return this.devices[0]
            }
        }
        return this.devices[4]
    },
    childurl: function () {
        return location.href.substr(location.href.indexOf("/", 8), location.href.length - location.href.indexOf("/", 8));
    },
    GoAjax:function(data) {
        var apiUrl = (data.event_type == 'pv') ? this.STATIC_URL_SPV : this.STATIC_URL_SEVENT;

        //var postData = { 'Api': apiUrl, 'Data': JSON.stringify(this.handleTaData(data)), 'OriginUrl': this.BASE_URL };
        var xhr=null;
        if(window.XMLHttpRequest){  
            xhr = new XMLHttpRequest();  
        }  
        else{  
            xhr = new ActiveXObject("Microsoft.XMLHTTP");
        }
        //var reqUrl= "/Article/SendBiRequest?Api="+apiUrl+"&Data="+encodeURIComponent (JSON.stringify(this.handleTaData(data)))+"&OriginUrl="+this.BASE_URL;
        var postData = JSON.stringify(this.handleTaData(data));


        xhr.withCredentials = true;
        xhr.open("POST", apiUrl, true);
        xhr.setRequestHeader("Content-type", "application/json");

        xhr.onreadystatechange =  function(){  
            if(xhr.readyState == 4){//异步请求时的状态码4代表数据接收完毕  
                if (xhr.status == 200) {//HTTP的状态 成功     
                    var res = eval("(" + xhr.responseText + ")");
                    //if (res.Index) {
                    //    var wxSid = _Config.GetCookie('WXSid');
                    //    var wxUid = _Config.GetCookie('WXUid');
                    //    if (wxSid==null && wxUid==null) {
                    //        _Config.SetCookie(Ta.WXSid, res.SID);
                    //        _Config.SetCookie(Ta.WXUid, res.UUID);
                    //    }                        
                    //}
                }  
            }  
        }  
        xhr.send(postData);
    },
    Run:function(eventAction, eventType, K, V) {
        this.eventAction = eventAction;
        this.eventType = eventType;
        this.userid = "";
        if (K != "" && K != undefined && V != "" && V != undefined) {
            this.metadata = '{"' + K + '":"' + V + '"}';
        }
        try {
            var _app_id = "tuhu_wap";
            if (_H5Utils.IsAndroid()) {
                _app_id = "h5_android_app";
            }
            if (_H5Utils.IsIOS()) {
                _app_id = "h5_ios_app";
            }
            if (_H5Utils.IsWx()) {
                _app_id = "weixin";
            }
            var data = {
                app_id: _app_id,
                event_action: this.isFirstInTa() ? 'get_info' : this.eventAction,
                event_category: this.childurl(),
                user_id: this.userid,
                level: 'info',
                event_type: this.isFirstInTa() ? 'get_info' : this.eventType,
                metadata: this.metadata
            }
            this.GoAjax(data);
        } catch (e) {
            console.log(e);
        }
    }

}
