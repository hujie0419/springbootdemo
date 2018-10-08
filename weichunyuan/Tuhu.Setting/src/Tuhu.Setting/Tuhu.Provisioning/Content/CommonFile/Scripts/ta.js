var _Utils = {
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
var _Config = {
    SetCookie(name, value) {
        var Days = 3600;
        var exp = new Date();
        //  domain = "tuhu.cn";
        exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);
        document.cookie = name + "=" + escape(value) + ";path=/;expires=" + exp.toGMTString();
    },
    GetCookie(name) {
        var arr = document.cookie.match(new RegExp("(^| )" + name + "=([^;]*)(;|$)"));
        if (arr != null) return unescape(arr[2]); return null;
    },
    DelCookie(name) {
        var exp = new Date();
        exp.setTime(exp.getTime() - 1);
        var cval = getCookie(name);
        if (cval != null)
            document.cookie = name + "=" + cval + ";expires=" + exp.toGMTString() + "; path=/";
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
    BASE_URL:"http://faxian.tuhu.dev",
    STATIC_URL_SPV: "https://hi.tuhu.com/collect/spv",
    STATIC_URL_SEVENT: "https://hi.tuhu.com/collect/sevent",
    eventAction: null,
    eventType: null,
    metadata:null,
    isFirstInTa() {
        return document.cookie.indexOf('TA') < 0
    },
    handleTaData(data) {
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

        data.sid = _Config.GetCookie('WXSid') || '';

        data.uuid = _Config.GetCookie('WXUid') || '';

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
    appVersion() {
        if (_Utils.IsAppNew()) {
            if (_Utils.IsAndroid()) {
                return 'Android' + window.isVersion;
            }
            return window.isVersion;
        }
        return ''
    },
    appName() {
        if (_Utils.IsAppNew()) {
            if (_Utils.IsAndroid()) {
                return this.appNames[0];
            }
            return this.appNames[1];
        }
        return this.appNames[2];
    },
    lang() {
        return navigator.language.split('-')[0]
    },
    country() {
        return navigator.language.split('-')[1]
    },
    screenResolution() {
        return window.innerWidth + 'x' + window.innerHeight;
    },
    osName() {
        if (_Utils.IsAndroid()) {
            return this.osNames[0]
        }

        return this.osNames[1]
    },
    osVersion() {
        if (_Utils.IsAndroid()) {
            return navigator.userAgent.match(/Android\s\d\.\d/)[0].split(' ')[1]
        }

        return "";//navigator.userAgent.match(/\s\d\_\d/)[0].replace('_', '.');
    },
    deviceModel() {
        if (_Utils.IsIOS()) {
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
    childurl() {        
        return location.href.substr(location.host.length + 1);
    },
    GoAjax: function (data) {
        var apiUrl = (data.event_type == 'pv') ? this.STATIC_URL_SPV : this.STATIC_URL_SEVENT;
        $.ajax({
            url: this.BASE_URL + "/Article/SendBiRequest",
            type: "get",
            dataType: 'jsonp',
            crossDomain: true,
            data: { 'Api': apiUrl, 'Data': JSON.stringify(this.handleTaData(data)),'OriginUrl':this.BASE_URL},
            success: function (res) {
                if (res.Index) { 
                    _Config.SetCookie(Ta.WXSid, res.SID);
                    _Config.SetCookie(Ta.WXUid, res.UUID);
                }                
            }
        });
    },
    Run: function (eventAction, eventType,K,V) {
        this.eventAction = eventAction;
        this.eventType = eventType;
        this.userid = "";
        if (K != "" && K != undefined && V != "" && V != undefined) {
            this.metadata = '{"' + K + '":"' + V + '"}';
        }
        var data = {
            app_id: _Utils.IsWx() ? 'weixin' : 'tuhu_wap',
            event_action: this.isFirstInTa() ? 'get_info' : this.eventAction,
            event_category: this.childurl(),
            user_id: this.userid,
            level: 'info',
            event_type: this.isFirstInTa() ? 'get_info' : this.eventType,
            metadata:this.metadata
        }
        this.GoAjax(data);
    }

}

