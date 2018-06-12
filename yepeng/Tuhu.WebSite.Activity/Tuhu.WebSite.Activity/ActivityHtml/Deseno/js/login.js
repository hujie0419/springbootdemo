
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
        iosend("aboutTuhuUserinfologin", "CallBackAppLoginResponse");
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
    Uuid = da.uuid;
};
//进入iosApp
function iosend(cmd, arg) {
    location.href = '#testapp#' + cmd + '#' + encodeURIComponent(arg);
};

var isIosVersion = false;
//IOS版本判断
function VersionForIos(version) {
    isIosVersion = true;
}

