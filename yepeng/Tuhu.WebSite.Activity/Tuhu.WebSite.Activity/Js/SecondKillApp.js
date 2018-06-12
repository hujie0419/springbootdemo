(function () {
    //  --------------------------根据userid获取默认车型Start
    var domain = 'http://resource.tuhu.cn';
    if (new Date() > new Date("2017-4-30")) {
        window.location.href = "http://resource.tuhu.cn/sp/ActiveNinety.html";
    } else {
        var userAgentInfo = navigator.userAgent;
        var Agents = new Array("iPhone", "iPad", "iPod");
        var flag = true;
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

        function sendCommand(cmd, param) {
            var url = "#testapp#" + cmd + "#" + param;
            document.location = url;
        }

        //-----------------------------------------获取车型
        function GetDefateCartModel(userid) {
            if (userid != null) {
                
                var existCookie = document.cookie.indexOf(userid + "caremodel=");
                if (existCookie != "-1") {
                     // alert("not null");
                      var carmodel = getCookie(userid + "caremodel");//获取这个userid对应的车型
                    var josncarmodel = JSON.parse(carmodel);
                   // alert(josncarmodel.VehicleID+"=>"+josncarmodel.PaiLiang+"=>"+ josncarmodel.Nian);
                    getWiper(josncarmodel.VehicleID, josncarmodel.PaiLiang, josncarmodel.Nian)//获取雨刷，VehicleIDm：车型，PaiLiang：排量，Nian：年
                }
                else {//-------------------------------车型为空
                    // alert("cookie车型为空");
                    $("#nowiper").hide();
                    $("#yeswiper").hide();
                    $.ajax({
                        url: "http://wx.tuhu.cn/By/GetDefCarModelUid?uid=" + userid,
                        type: 'get',
                        datatype: 'json',
                        success: function (data) {
                            var jsondata = JSON.parse(data);
                            if (jsondata != "") {
                                var carmodel = new Object();
                                carmodel.VehicleID = jsondata.VehicleID;
                                carmodel.VehicleName = jsondata.VehicleName;
                                carmodel.PaiLiang = jsondata.PaiLiang;
                                carmodel.Nian = jsondata.Nian;
                                carmodel.liYangID = jsondata.LiYangID;
                                carmodel.Brand = jsondata.Brand;
                                SetCookie(userid +"caremodel", JSON.stringify(carmodel));//设置这个userid所对应的车型
                                getWiper(jsondata.VehicleID, jsondata.PaiLiang, jsondata.Nian)//获取雨刷，VehicleIDm：车型，PaiLiang：排量，Nian：年
                            }
                            else {//没有匹配的车型
                                $(".ljmscx").show();//第一次没有匹配上，显示匹配按钮，匹配上了没有雨刷，显示下面的更改
                            }
                        },
                        error: function (data) {
                            alert("error");
                        }
                    })
                }
            }
        }

        window.callBackSelectDefateCartModel = function (da)//选择车型
        {
            SetCookie("userid", da.userid);
            window.location = "http://faxian.tuhu.cn/selectcar/index.html?UserId=" + da.userid + "&Step=" + 2
        };

        window.callBackChangeCartModel = function ()//更换车型
        {
                var userid = getCookie("userid")
                alert(userid + "caremodel");
                var carmodel = getCookie(userid + "caremodel");//获取这个userid对应的车型
                alert(carmodel);
                var josncarmodel = JSON.parse(carmodel);
                window.location = "http://faxian.tuhu.cn/selectcar/index.html?UserId=" + userid + "&Step=" + 2 + "&VehicleID=" + josncarmodel.VehicleID + "&VehicleName=" + josncarmodel.VehicleName + "&PaiLiang=" + josncarmodel.PaiLiang + "&Nian=" + josncarmodel.Nian + "&liYangID=" + josncarmodel.liYangID;//+ "&Brand=" + josncarmodel.Brand
        };



        function getPhone(cb) {
            if (userAgentInfo.indexOf("Android") >= 0) {
                window.WebViewJavascriptBridge.callHandler(
                    'actityBridge'
                    , { 'param': "" }
                    , function (responseData) {
                        if (responseData != null) {
                            try {
                                var da = JSON.parse(responseData);
                                if (window[cb] != null) {
                                    window[cb](da);
                                }
                            } catch (e) {
                                alert(e);
                            }
                        }
                    });
            } else {
                //openTip("os");
               alert('ios');
                sendCommand("Userinfologin", cb);
            }
        }
    }

    //----------------------------根据userid获取默认车型End
    var timeCountDown;
    GetDefateCartModel(getCookie("userid"));
    $(".ljms").hide();
    $(".ljmshs").hide();//立即秒杀灰色
    $(".kqtz").hide();//开枪通知隐藏
    $("#yeswiper ul").children().last().hide();//匹配车型
    $("#nowiper").hide();//当没有加载到雨刷的时候，
    $("#yeswiper").hide();//当没有加载到雨刷的时候，



    //判断秒杀十分开始，，和开始的逻辑
    var countDone = function () {
        //当十分秒的个位十位都等于=0的时候开枪
        document.getElementById("tenhourid").innerHTML = document.getElementById("thehourid").innerHTML = document.getElementById("tenminuteid").innerHTML = document.getElementById("theminuteid").innerHTML = document.getElementById("tensecondid").innerHTML = document.getElementById("thesecondid").innerHTML = "0";
        $(".ljmshs").hide();//灰色立即秒杀隐藏
        //-----------------------------------------秒杀按钮事件
        $(".ljms").show().click(function () {
            alert("秒杀开始");
            $.ajax({
                url: "http://faxian.tuhu.cn/SecondKillApp/CheckSecondKillByUserID.html",//1元抢购
                type: 'post',
                datatype: 'json',
                data: { userid: getCookie("userid") },
                success: function (data) {
                    alert(data);
                    if (data > 0)//抢购成功
                    {
                        pay(1);
                    }
                    else if (data == -1) {
                        alert("活动还没开始");
                    }
                    else if (data == -2) {
                        alert("没有抢到");
                    }
                    else if (data == -3) {
                        alert("已抢完");
                    }
                }
            });
        });
        setTimeout(function () {
            location.reload();
        }, 600000);
    }

    //十分秒倒计时实现，并显示成"00:04:320"
    var startCountDown = function (hour, day) {
        timeCountDown = Tuhu.Utility.TimeCountDown(day.getFullYear() + "/" + (day.getMonth() + 1) + "/" + day.getDate() + " " + hour + ":00:00").progress(function (remainingTime) {
            var tenhourid = remainingTime.hours.toString().substring(0, 1);
            var thehourid = remainingTime.hours.toString().substring(1, 2);
            var tenminuteid = remainingTime.minutes.toString().substring(0, 1);
            var theminuteid = remainingTime.minutes.toString().substring(1, 2);
            var tensecondid = remainingTime.seconds.toString().substring(0, 1);
            var thesecondid = remainingTime.seconds.toString().substring(1, 2);
            document.getElementById("tenhourid").innerHTML = tenhourid;
            document.getElementById("thehourid").innerHTML = thehourid;
            document.getElementById("tenminuteid").innerHTML = tenminuteid;
            document.getElementById("theminuteid").innerHTML = theminuteid;
            document.getElementById("tensecondid").innerHTML = tensecondid;
            document.getElementById("thesecondid").innerHTML = thesecondid;


            if (remainingTime.tick < +10 * 60 * 1000) {//如果时间小于10分钟
                $(".kqtz").hide();//开枪通知隐藏
                $(".ljmscx").hide();//匹配车型隐藏掉
                $(".ljms").hide();//黄色立即秒杀隐藏
                $(".ljmshs").show();//灰色立即秒杀显示
                $("#yeswiper ul").children().last().show();//匹配车型显示
            }
            if (remainingTime.tick > 10 * 60 * 1000) {//如果时间大于10分钟
                if ($("#yeswiper").is(":hidden") && $("#nowiper").is(":visible"))//当，匹配车型没有雨刷
                {
              
                   // $(".ljmscx").show()
                    $(".ljmscx").hide();//匹配车型隐藏
                    $(".ljmshs").show();//立即秒杀灰色
                   
                }
                else if ($("#yeswiper").is(":hidden") && $("#nowiper").is(":hidden"))//没有匹配到雨刷,需要重新匹配雨刷
                {
                    $(".ljmshs").hide();//立即秒杀灰色
                    $(".ljmscx").show();//匹配车型隐藏
                }
                else {
                    $(".ljmscx").hide();//匹配车型隐藏
                    $(".kqtz").show();//开枪通知显示
                    // $(".ljms").show()
                    $("#yeswiper ul").children().last().show();//匹配车型显示
                }
            }
        }).done(countDone);
    }

    var startNext = function (next) {
        var hoursArr = [10, 11, 13, 16, 20, 21];
        var now = new Date();
        var hour = null, tomorrow = null;
        $.each(hoursArr, function () {
            if (now.getHours() < this || !next && now.getHours() == this && now.getMinutes() < 10) {
                hour = this;
                return false;
            }
        });
        if (!hour) {
            hour = hoursArr[0];
            tomorrow = new Date(now.setDate(now.getDate() + 1));
        }

        if (hour == now.getHours()) {
            $.post("/ActivityPhase/IsFinish.html", { activityID: 'SecondKill' }).complete(function (ajax) {
                if (ajax.status == 200) {
                    if ($.parseJSON(ajax.responseText))
                        startNext(true);
                    else
                        countDone();
                }
                else if (ajax.status > 0)
                    alert("服务器错误！");
            });
        }
        else
            startCountDown(hour, tomorrow || new Date());
    }



    //-------------------------------------------匹配车型单机事件，首次验证
    $(".ljmscx").click(function () {
        getPhone("callBackSelectDefateCartModel");//选择车型
    });

    //--------------------------------------------我要换车型
    $("#yeswiper ul").children().last().click(function () {
        //alert(getCookie(getCookie("userid") + "caremodel"));
        //delCookie(getCookie("userid") + "caremodel");//更改车型的时候把cookie删除
       // alert(getCookie(getCookie("userid") + "caremodel"));
        getPhone("callBackChangeCartModel");//更换车型

    });

    //-------------------------------------------7折购买
    $("#District").click(function () {
        $.ajax({
            url: "http://faxian.tuhu.cn/SecondKillApp/CheckUserSevenDiscount.html",
            type: 'post',
            datatype: 'json',
            data: { userid: getCookie("userid") },
            success: function (data) {
                //  alert(data);
                if (data > 0)//七折购买成功
                {
                    alert("购买成功");
                    pay(46.2);//下单
                }
                else if (data == -1) {
                    alert("秒杀成功过，不能购买");
                }
                else if (data == -2) {
                    alert("7折购买过，不能购买");
                }
            }
        });
    });

    function openTip(msg) {
        var $tips = $("#tipsContent");
        var $popTips = $("#popTips");
        $tips.text(msg);
        $popTips.show();
        setTimeout(function () {
            $popTips.hide();
        }, 1500);
    }

    //--------------------------------------------获取雨刷
    function getWiper(VehicleID, PaiLiang, Nian) {
        $.ajax({
            url: "http://faxian.tuhu.cn/SecondKillApp/getWiper.html",
            data: { vehicleID: VehicleID, paiLiang: PaiLiang, manufactureYear: Nian },
            type: 'post',
            datatype: 'json',
            success: function (data) {
                var jsondata = JSON.parse(data);
                if (jsondata != "") {
                    $("#nowiper").hide();
                    $("#yeswiper").show();
                    var Primaryhtml = "主雨刷:" +jsondata[0].PrimaryDisplayName ;
                    var Secondhtml = "副雨刷: "+ jsondata[0].SecondDisplayName ;
                    var wiperjson = new Object();
                    wiperjson.PrimaryPid = jsondata[0].PrimaryPid;
                    wiperjson.PrimaryDisplayName = jsondata[0].PrimaryDisplayName;
                    wiperjson.SecondPid = jsondata[0].SecondPid;
                    wiperjson.SecondDisplayName = jsondata[0].SecondDisplayName;
                    SetCookie(getCookie("userid") + "wiper", JSON.stringify(wiperjson));
                    $("#Primary").html(Primaryhtml);//加载主雨刷
                    $("#Second").html(Secondhtml);//加载副雨刷
                }
                else  //没有加载到雨刷
                {
                    $("#nowiper").show();
                    $("#yeswiper").hide();
                    $("#nowiper ul li").click(function () {//给a标签绑定，匹配车型的单机事件
                       // delCookie(getCookie("userid") + "caremodel");//更改车型的时候把cook删除
                        getPhone("callBackSelectDefateCartModel");//选择车型
                    });
                }
            },
        });
    };

    //---------------------------------------获取cookie
    function getCookie(name) {
        var arr = document.cookie.match(new RegExp("(^| )" + name + "=([^;]*)(;|$)"));
        if (arr != null) return unescape(arr[2]); return null;
    }

    //--------------------------------------设置cookie
    function SetCookie(name, value) {
        var Days = 3600;
        var exp = new Date();
        exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);
        document.cookie = name + "=" + escape(value) + ";path=/;expires=" + exp.toGMTString();
    }
    //--------------------------------------删除ookie
    function delCookie(name) {
        var exp = new Date();
        exp.setTime(exp.getTime() - 1);
        var cval = getCookie(name);
        if (cval != null)
            document.cookie = name + "=" + cval + ";expires=" + exp.toGMTString()+"; path=/";
    }

    startNext();
    $(".sk_content>div:first-child>img.pointer").click(function () {
        Tuhu.CarHistory.LoadDialog("DBY", function () {
            location.reload();
        });
    });

    //---------------------------------------开枪通知事件
    $(".kqtz").click(function () {
        // var ph = getCookie("phone");
        //  alert(ph);
        //$.post("http://faxian.tuhu.cn/SecondKillApp/Sms.html", { phone: ph, msg: 'SecondKill' }).complete(function (ajax) {
        //    if (ajax.status == 200) {
        //        if ($.parseJSON(ajax.responseText) > -1) {

        //        }
        //    }
        //    else if (ajax.status > 0)
        //        alert("服务器错误！");
        //});

        $.post("http://faxian.tuhu.cn/SecondKillApp/SmsWithThen.html", { userid: getCookie("userid") }).complete(function (ajax) {
            if (ajax.status == 200) {
                if ($.parseJSON(ajax.responseText) > -1) {
                    alert("我们会在下次活动开始前10分钟通知您");
                    //  Tuhu.Dialog.Popup($(".kaiqiang_tips"), { opacity: 0.2, draggable: false, disableScroll: true, closeHandle: ".sk_close" });
                }
            }
            else if (ajax.status > 0)
                alert("服务器错误！");
        });
    });

    //-----------------------------------------支付
    function pay(price) {
        var wiperjson = JSON.parse(getCookie(getCookie("userid") + "wiper"));
        var PrimaryPid = wiperjson.PrimaryPid;
       // alert(wiperjson.PrimaryDisplayName);
        var PrimaryName = encodeURIComponent (wiperjson.PrimaryDisplayName);
        var SecondPid = wiperjson.SecondPid;
        var SecondName = encodeURIComponent(wiperjson.SecondDisplayName);
        var Price = price;
        window.location = "http://resource.tuhu.cn:8080/ActivityHtml/SecondKillApp/OrderInfo.html?UserId=" + getCookie("userid") + "&PrimaryPid=" + PrimaryPid + "&PrimaryName=" + PrimaryName+ "&SecondPid=" + SecondPid + "&SecondName=" + SecondName + "&Price=" + Price + "&Channel=1";
    }
})();