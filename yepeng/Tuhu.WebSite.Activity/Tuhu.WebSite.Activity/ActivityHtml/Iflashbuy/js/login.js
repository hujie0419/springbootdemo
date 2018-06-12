
var activityId = "";
var UserId = "";

var urlStr = "http://faxian.tuhu.cn";
var urlWX = 'http://wx.tuhu.cn';
$(function () {

    //域名
    var domainConfig = {};
    domainConfig.TopDomain = ".tuhu.cn";
    if (/\.tuhu\.(\w+)$/.test(location.host))
        domainConfig.TopDomain = ".tuhu." + RegExp.$1;
    domainConfig.ApiSite = "//api" + domainConfig.TopDomain;
    var statepage = false;
    var activeHtml = "";
    var isFirstLoad = true;
    $.ajax({
        type: 'GET',
        url: urlWX + "/TuHuGas/SelectShanGouProducts",
        data: {},
        async: false,
        dataType: 'jsonp',
        success: function (data) {
            console.log(data);
            if ((data.list || "").length > 0) {
                var html = "";
                var activeOrder = -1;
                var activeIndex = 0;
                for (var obj in data.list) {
                    activeOrder++;
                    //alert(data[obj].StartDateTime.substr(11, 5));
                    if (data.list[obj].Products[0].PID.substr(0, 2) == "TR") {
                        if (data.list[obj].EndDateTime < data.date) {
                            html += '<div class="swiper-slide" type="Tire.html?actid=' + data.list[obj].ActivityID + '"><span>' + data.list[obj].StartDateTime.substr(11, 5) + '</span><span>已结束</span></div>';
                        } else if (data.date >= data.list[obj].StartDateTime && data.date <= data.list[obj].EndDateTime) {
                            html += '<div class="swiper-slide" type="Tire.html?actid=' + data.list[obj].ActivityID + '"><span>' + data.list[obj].StartDateTime.substr(11, 5) + '</span><span>进行中</span></div>';
                            activeHtml = urlStr + "/ActivityHtml/Iflashbuy/Tire.html?actid=" + data.list[obj].ActivityID;
                        } else if (data.list[obj].StartDateTime > data.date) {
                            html += '<div class="swiper-slide" type="Tire.html?actid=' + data.list[obj].ActivityID + '"><span>' + data.list[obj].StartDateTime.substr(11, 5) + '</span><span>即将开始</span></div>';
                        }

                    } else {

                        if (data.list[obj].EndDateTime < data.date) {
                            html += '<div class="swiper-slide" type="Index.html?actid=' + data.list[obj].ActivityID + '"><span>' + data.list[obj].StartDateTime.substr(11, 5) + '</span><span>已结束</span></div>';
                        } else if (data.date >= data.list[obj].StartDateTime && data.date <= data.list[obj].EndDateTime) {
                            html += '<div class="swiper-slide" type="Index.html?actid=' + data.list[obj].ActivityID + '"><span>' + data.list[obj].StartDateTime.substr(11, 5) + '</span><span>进行中</span></div>';
                            activeHtml = urlStr + "/ActivityHtml/Iflashbuy/Index.html?actid=" + data.list[obj].ActivityID;
                        } else if (data.list[obj].StartDateTime > data.date) {
                            html += '<div class="swiper-slide" type="Index.html?actid=' + data.list[obj].ActivityID + '"><span>' + data.list[obj].StartDateTime.substr(11, 5) + '</span><span>即将开始</span></div>';

                        }
                    }

                    var array = data.list.sort(getSortFun('asc', 'StartDateTime'));


                    if (data.date >= data.list[obj].StartDateTime && data.date <= data.list[obj].EndDateTime) {
                        activityId = data.list[obj].ActivityID;
                        activeIndex = activeOrder + 2;

                    } else {
                        for (var i = 0; i < array.length; i++) {
                            if (array[i].StartDateTime.substr(11, 2) >= data.date.substr(11, 2)) {
                                activeIndex = i + 2;
                                activityId = array[i].ActivityID;
                                break;
                            }
                        }
                    }

                    for (var i = 0; i < array.length; i++) {
                        if (data.date >= array[i].StartDateTime && data.date <= array[i].EndDateTime) {
                            activeIndex = i + 2;
                            activityId = array[i].ActivityID;
                            break;
                        }
                    }

                }

                $("#ii").after(html);
                //alert(activeIndex);
                var emptySlide = 2;

                if (getQueryString("change") != null && getQueryString("change") != "") {

                    activeIndex = getQueryString("change");
                    // alert(activeIndex);
                }

                if (getQueryString("actid") != null && getQueryString("actid") != "") {
                    activityId = getQueryString("actid");
                } else {
                    isFirstLoad = false;
                    window.location.href = activeHtml;

                }

                $slides = $(".swiper-slide");
                $slides.eq(activeIndex).addClass("active");
                //alert(activeIndex)
                var swiper = new Swiper('.swiper-container', {
                    initialSlide: activeIndex - emptySlide,
                    slidesPerView: 5,
                    onInit: function (swiper) {
                        statepage = true;

                    },
                    onTransitionEnd: function (swiper) { //拖动的回调                       
                        if (statepage) {
                            var midIndex = swiper.activeIndex + emptySlide;
                            $slides.removeClass("active").eq(midIndex).addClass("active");

                            //alert(midIndex);
                            //alert($slides.eq(midIndex).attr("type"))
                            Tiaozhuan($slides.eq(midIndex).attr("type"), midIndex);

                        }
                    }
                }).on("click", function (swiper) { //点击的回调               
                    var slideCount = $slides.length;
                    if (swiper.clickedIndex < emptySlide || swiper.clickedIndex > slideCount - emptySlide - 1) {
                        return false;
                    }
                    var midIndex = swiper.clickedIndex - emptySlide;
                    swiper.slideTo(midIndex, 500, false);
                    setTimeout(function () {
                        $slides.removeClass("active").eq(swiper.clickedIndex).addClass("active");
                    }, 500)

                    Tiaozhuan($slides.eq(swiper.clickedIndex).attr("type"), swiper.clickedIndex);
                });


                //begin  js

                var appUserID, appUserPhone;
                var doc = document, win = window,
                    tmpl_TimeCountDown = '<div class="header" ><label class="txt" v-text="tips[status]">活动已结束</label>\
		<b v-show="showTime"><span v-text="hour">10</span> : <span v-text="minute">13</span> : <span v-text="second">36</span></b></div>';
                var urlParams = (function () {
                    var search = window.location.search,
                        params = {};
                    search = search.substring(1, search.length);
                    search.split('&').forEach(function (sp) {
                        var kvp = sp.split('=');
                        if (kvp.length > 0) {
                            params[kvp[0]] = kvp.length > 1 ? kvp[1] : '';
                        }
                    });
                    return params;
                })();
                //弹出提示
                var openTip = (function () {
                    var $tips = $("#tipsContent");
                    var $popTips = $("#popTips");
                    return function (msg) {
                        $tips.text(msg);
                        $popTips.show();
                        setTimeout(function () {
                            $popTips.hide();
                        }, 3000);
                    }
                }());

                var $resource = {
                    getProducts: function () {
                        return $.ajax({
                            type: 'get',
                            url: domainConfig.ApiSite + '/action/GetFlashSaleProduct',
                            data: { actid: activityId },
                            dataType: 'jsonp'
                        }).then(function (data) {
                            if (!data) { openTip('活动不存在'); }
                            else {

                                !data.Products && (data.Products = []);
                                data.Products = data.Products.map(function (p) {
                                    //剩余数量
                                    // p.TotalQuantity 库存数量
                                    //	=0 库存无限
                                    //p.MaxQuantity //个人限购数量
                                    //  =0 个人限购数不限
                                    // p.SaleOutQuantity 已售出
                                    p.SaleOutPercentage = parseInt((p.SaleOutQuantity / p.TotalQuantity) * 100) > 100 ? 100 : parseInt((p.SaleOutQuantity / p.TotalQuantity) * 100);
                                    p.surplusQuantity = p.TotalQuantity - p.SaleOutQuantity;
                                    p.Quantity = 0;
                                    //是否允许减
                                    p.enSub = false;
                                    // 是否允许加
                                    p.enAdd = true;
                                    p.InstallAndPay;
                                    p.IsUsePcode;
                                    p.PID;
                                    p.IsEnd = (+new Date(data.EndDateTime)) <= (+new Date(data.ServiceTime));
                                    p.IsStart = (+new Date(data.StartTime)) >= (+new Date(data.ServiceTime));
                                    return p;
                                });
                                data.IsEnd = (+new Date(data.EndDateTime)) <= (+new Date(data.ServiceTime));
                                data.IsStart = (+new Date(data.StartTime)) >= (+new Date(data.ServiceTime));
                                IsEnd = (+new Date(data.EndDateTime)) <= (+new Date(data.ServiceTime));
                                IsStart = (+new Date(data.StartTime)) >= (+new Date(data.ServiceTime));
                                data.TotalPrice = 0;
                                data.TotalQuantity = 0;
                                return data;
                            }
                        }, function () {
                            openTip('加载出错');
                        })
                    }, getBuyRecords: function (userID, phone) {
                        return $.ajax({
                            type: 'get',
                            url: domainConfig.ApiSite + '/action/GetFlashSaleBuyRecord',
                            data: { userID: userID, phone: phone, actid: activityId },
                            dataType: 'jsonp'
                        }).then(function (data) { return data || []; }, function () {
                            openTip('加载出错');
                        })
                    }
                };

                //TEST 正式取消
                //TuhuWebViewBridge.login = function (cb) {
                //    var UserId = 'dbb4a4a8-5bd7-04ff-97bf-e1452dc70884', Phone = '15221306857';
                //    cb({ userid: UserId, phone: Phone })
                //};

                if (isFirstLoad) {
                    //获取登录用户的信息
                    TuhuWebViewBridge.login(function _login(da) {
                        function isNull(d) {
                            return d === "(null)" || d === 'null' || !d;
                        }
                        if (isNull(da.userid) || isNull(da.phone)) {
                            //未登录
                            $resource.getProducts().then(function (data) { callbackInit(data, []) })
                        } else {
                            appUserID = da.userid;
                            appUserPhone = da.phone;
                            Uuid = da.uuid;
                            $.when($resource.getProducts(), $resource.getBuyRecords(appUserID, appUserPhone))
                                .then(function (data, records) {
                                    callbackInit(data, records || []);
                                })
                        }
                    });
                }

                function isAndroid() {
                    var ua = navigator.userAgent.toLowerCase();
                    var isA = ua.indexOf("android") > -1;
                    if (isA) {
                        return true;
                    }
                    return false;
                }
                function isIphone() {
                    var ua = navigator.userAgent.toLowerCase();
                    var isIph = ua.indexOf("iphone") > -1;
                    if (isIph) {
                        return true;
                    }
                    return false;
                }
                function getBannerImg(d) {
                    return d.IsBannerAndroid == 1 ? d.BannerUrlAndroid : "";
                }
                function getEndImg(d) {

                    return d.IsEndImage ? d.EndImage : "";
                }
                //倒计时
                function getSurplusTime(timespan, ret) {
                    var allSeconds = timespan / 1000;//总共剩余秒数
                    var surplusHour = Math.floor(allSeconds / 3600);//剩余小时
                    var surplusMinute = Math.floor((allSeconds - surplusHour * 3600) / 60);//剩余分钟
                    var surplusSecond = Math.floor((allSeconds - surplusHour * 3600 - surplusMinute * 60))//剩余秒数
                    ret.hour = surplusHour;
                    ret.minute = surplusMinute >= 10 ? surplusMinute : '0' + surplusMinute;
                    ret.second = surplusSecond >= 10 ? surplusSecond : '0' + surplusSecond;
                };

                var _now = new Date();
                function invokeUrl() {
                    try {
                        //var hash = window.location.hash;
                        var id = getQueryString("PID");
                        if (id) {
                            var t = $("div[id='" + id + "']").offset().top;
                            $(window).scrollTop(t);
                        }

                    } catch (e) { }

                }

                function callbackInit(data, records) {

                    var bannerImg = getBannerImg(data);
                    var endImg = getEndImg(data);

                    if (endImg.toLowerCase().indexOf("http") >= 0) {
                        $('#endImg').attr('src', endImg);
                    } else {
                        $('#endImg').css("display", "none");
                    }

                    if (bannerImg.toLowerCase().indexOf("http") >= 0) {
                        $('#bannerImg').attr('src', bannerImg);
                    } else {
                        $('#bannerImg').css("display", "none");
                    }

                    $('body').css("backgroundColor", data.BackgoundColor);
                    //data.BackgoundColor && $(document.body).attr('backgroundColor', data.BackgoundColor);
                    data.bannerImg = bannerImg;
                    data.ActivityName && (document.title = data.ActivityName);
                    //var activityId = urlParams['actid'];
                    //限制购买数量
                    function restrict(product, Quantity) {

                        var getErrorMsg = function (code, buyCount) {
                            var errorMsg = {
                                //超出库存
                                ouSurplusMAX: ['您购买的', product.PName, '数量超出该商品的剩余数量！该商品剩余', product.surplusQuantity, '件'].join(''),
                                //已购买已经超出购买限制
                                hasBUYMAX: ['该产品限购买', product.MaxQuantity, '件，您已不能再次购买！'].join(''),
                                //已购买加上这次超出购买限制
                                hasBUY: ['该产品限购买', product.MaxQuantity, '件，您已购买', buyCount, '件！'].join(''),
                                //超出购买限制
                                outLimitMAX: ['该产品限购', product.MaxQuantity, '件哦！'].join('')
                            };
                            openTip(errorMsg[code] || '');
                            return false;
                        };
                        if (product.TotalQuantity > 0 && Quantity > product.surplusQuantity) {
                            return getErrorMsg('ouSurplusMAX');
                        } else {
                            var productRecord = records.filter(function (p) { return p.PID == product.PID });//历史购买数据
                            //var productRecord = [];
                            if (productRecord.length) {
                                var buyCount = productRecord[0].Quantity;
                                if (product.MaxQuantity !== 0) {
                                    if (buyCount >= product.MaxQuantity) { return getErrorMsg('hasBUYMAX') }
                                    else if (buyCount + Quantity > product.MaxQuantity) { return getErrorMsg('hasBUY', buyCount) }
                                }
                            } else {
                                if (Quantity > product.MaxQuantity && product.MaxQuantity > 0) {
                                    return getErrorMsg('outLimitMAX');
                                }
                            }
                        }

                        return true;
                    };
                    //fix js计算浮点数bug
                    var fixMath = {
                        add: function add(a, b) {
                            var c, d, e;
                            try {
                                c = a.toString().split(".")[1].length;
                            } catch (f) {
                                c = 0;
                            }
                            try {
                                d = b.toString().split(".")[1].length;
                            } catch (f) {
                                d = 0;
                            }
                            return e = Math.pow(10, Math.max(c, d)), (this.mul(a, e) + this.mul(b, e)) / e;
                        }, mul: function mul(a, b) {
                            var c = 0,
                                d = a.toString(),
                                e = b.toString();
                            try {
                                c += d.split(".")[1].length;
                            } catch (f) { }
                            try {
                                c += e.split(".")[1].length;
                            } catch (f) { }
                            return Number(d.replace(".", "")) * Number(e.replace(".", "")) / Math.pow(10, c);
                        }
                    };
                    var commonMethods = {

                        add: function (p) {

                            if (IsEnd) {
                                openTip("活动已结束");
                            } else if (IsStart) {
                                openTip("活动未开始");
                            } else {
                                var toQuantity = Number(p.Quantity + 1) || 0;
                                if (restrict(p, toQuantity)) {
                                    p.Quantity = toQuantity;
                                    this.update();
                                }
                                p.Quantity > 0 && !p.enSub && (p.enSub = true);
                            }

                        },
                        sub: function (p) {
                            if (IsEnd) {
                                openTip("活动已结束");
                            } else if (IsStart) {
                                openTip("活动未开始");
                            } else if (p.enSub) {
                                p.Quantity = Math.max(p.Quantity - 1, 0);
                                p.Quantity === 0 && (p.enSub = false);
                                this.update()
                            }
                        },
                        input: function (p) {
                            p.Quantity = Number(p.Quantity) || 0;
                            if (!restrict(p, p.Quantity)) {
                                p.Quantity = 0;
                            }
                            this.update()
                        },
                        toDetail: function (p) {
                            var _status = _vue._children[0].status;
                            var spPID = p.PID.split('|'), isTire = false;
                            if (p.PID.indexOf('TR') == 0) isTire = true;
                            var toDetailActivityId = activityId;
                            switch (_status) {
                                case "DNS":
                                case "END":
                                    toDetailActivityId = "";
                                    break;
                            }
                            if (TuhuWebViewBridge.os === 'Android') {
                                TuhuWebViewBridge.toDetail({ ProductID: spPID[0], VariantID: spPID[1], "FunctionID": !isTire ? "cn.TuHu.Activity.AutomotiveProducts.AutomotiveProductsDetialUI" : "cn.TuHu.Activity.TireInfoUI", activityId: toDetailActivityId, flashSaleNum: p.Quantity }, p.PID.indexOf('TR-') >= 0 ? 'Tire' : 'ChePing');
                            } else {
                                if (isTire) { TuhuWebViewBridge.send('TNTireInfoVC', JSON.stringify({ ProductID: spPID[0], VariantID: spPID[1], activityId: toDetailActivityId }), null, 1); }
                                else TuhuWebViewBridge.send('TNGoodsListDetailViewController', JSON.stringify({ productID: spPID[0], VariantID: spPID[1], activityId: toDetailActivityId }), null, 1)
                            }

                        },
                        update: function () {
                            var totalprice = 0, totalquantity = 0;
                            [].concat(this.Products).filter(function (p) { return p.Quantity > 0 })
                                .forEach(function (p) {
                                    totalprice = fixMath.add(totalprice, fixMath.mul(p.Quantity, p.Price));
                                    totalquantity += p.Quantity;
                                });
                            this.TotalPrice = totalprice;
                            this.TotalQuantity = totalquantity;
                        },
                        submit: function () {
                            Login();
                            var _status = _vue._children[0].status;
                            //{ 'DNS': '活动开始倒计时', 'BEG': '距离本场结束还剩', 'END': '活动已结束' },
                            switch (_status) {
                                case "DNS":
                                    openTip("活动还未开始");
                                    break;
                                case "END":
                                    openTip("活动已结束");
                                    break;
                                case "BEG":
                                    var Goods = [].concat(this.Products).filter(function (p) {
                                        return p.Quantity > 0
                                    });
                                    if (Goods.length === 0) { openTip("还未选择商品"); return; }
                                    var _tires = [], _chepin = [];

                                    var _orderType = 3;  // //1 ：轮胎 2：保养 3 车品***********
                                    //if (_chepin.length === 0) _orderType = 1;

                                    if (TuhuWebViewBridge.os === 'IOS') {
                                        Login();
                                        if (UserId != null && UserId != "") {
                                            var totalPrice = 0;
                                            Goods = Goods.map(function (p) {
                                                if (p.PID.indexOf('TR') == 0 || p.PID.indexOf('LG') == 0) { _tires.push(p.PID) }
                                                else { _chepin.push(p.PID) }
                                                totalPrice += p.Quantity * p.Price;
                                                return {
                                                    name: p.DisplayName,
                                                    count: p.Quantity,
                                                    price: p.Price.toFixed(2),
                                                    productID: p.PID.split('|')[0],
                                                    variantID: p.PID.split('|')[1],
                                                    image: p.Image
                                                }
                                            });
                                            //IOS新版 submit 数据
                                            var Parameters = {
                                                "goods<THGoods>": Goods,
                                                "totalPrice": totalPrice,
                                                "orderType": _orderType,
                                                "orderAddressType": data.ShippType,
                                                "couponType": 8,
                                                "activityId": activityId
                                            };
                                            console.log(Parameters);
                                            TuhuWebViewBridge.send('THCreatOrderVC', JSON.stringify(Parameters), null, 1);
                                        }
                                    } else {
                                        Goods = Goods.map(function (p) {
                                            if (p.PID.indexOf('TR') == 0 || p.PID.indexOf('LG') == 0) { _tires.push(p.PID) }
                                            else { _chepin.push(p.PID) }
                                            return {
                                                orderTitle: p.DisplayName,
                                                orderNum: p.Quantity,
                                                orderPrice: p.Price.toFixed(2),
                                                ProductID: p.PID.split('|')[0],
                                                VariantID: p.PID.split('|')[1],
                                                produteImg: p.Image
                                            }
                                        });
                                        //Android submit 数据
                                        var Parameters = {
                                            "activityId": activityId,
                                            "OrderChannel": "1",  //默认1，，，2：集采订单 3，，，，，
                                            "CouponType": "8",//不用优惠劵 ：不传这个值  或者 值为-1   ;用什么类型劵传什么值 如：车品劵 值为8，
                                            "orderType": _orderType,                  // //1 ：轮胎 2：保养 3 车品***********
                                            "carTypeSize": "",                 //轮胎专用 轮胎型号？？？必填？
                                            "ChePingTyPe": "6",           //1:新限时抢购 2:旧限时抢购 3:(0.99包邮活动)  4:一般车品 5:购物车   6:特价
                                            "typeService": {
                                                "Type": "",                        //填什么？？？
                                                "Name": "",               //填什么？？？
                                                "Description": "",        //填什么？？？
                                                "Count": ""                       //	填什么？？？
                                            },							//
                                            "quanType": "tire",
                                            "Goods": Goods,// 产品list
                                            "Car": {
                                                "VehicleID": "",
                                                "VehicleName": "",
                                                "Brand": "",
                                                "Nian": "",
                                                "PaiLiang": "",
                                                "LiYangID": ""			//数据库里没有可不填写？？
                                            }
                                        };
                                        TuhuWebViewBridge.send('toOrder', JSON.stringify(Parameters));
                                    }

                                    break;
                            }

                        }
                    };
                    var _vue = new Vue({
                        el: doc.body,
                        data: {
                            currentView: data.ShowType === 2 ? 'singleView' : 'listView',
                            Products: data.Products,
                            ShowCountDown: data.IsNoActiveTime,
                            isEnd: (+new Date(data.EndDateTime)) <= (+new Date(data.ServiceTime))
                        },
                        components: {
                            countDown: {
                                template: tmpl_TimeCountDown,
                                replace: true,
                                data: function () {
                                    //var now = new Date();
                                    var currentServiceTime = +new Date(data.ServiceTime);//服务器时间
                                    var currentNow = +new Date;//本地当前时间
                                    var startTime = +new Date(data.StartTime), endTime = +new Date(data.EndDateTime);

                                    var ret = {
                                        hour: 0,
                                        minute: 0,
                                        second: 0,
                                        tips: { 'DNS': '活动开始倒计时', 'BEG': '距离本场结束还剩', 'END': '活动已结束' },
                                        status: 'DNS',
                                        showTime: true
                                    };

                                    var _interval = setInterval(calc, 1000);
                                    function calc() {
                                        var now = (+new Date) - currentNow + currentServiceTime;//服务器时间
                                        //var now = +new Date;
                                        //console.log([(+new Date) - currentNow, currentServiceTime].join(' '));
                                        if (now > endTime || endTime <= startTime) {
                                            //3. now>endTime 活动结束
                                            getSurplusTime(0, ret);
                                            ret.status === 'END' || (ret.status = 'END', _vue && (_vue.isEnd = true));

                                            ret.showTime = false;
                                            clearInterval(_interval);
                                        } else if (now < startTime) {
                                            //1. now<startTime 活动还未开始		
                                            getSurplusTime(startTime - now, ret);
                                            ret.status === 'DNS' || (ret.status = 'DNS');
                                        } else {
                                            //2. starTime<=now<=endTime 活动进行中
                                            getSurplusTime(endTime - now, ret);
                                            ret.status === 'BEG' || (ret.status = 'BEG');
                                        }
                                    };
                                    calc();
                                    return ret;
                                }
                            }, listView: {
                                template: '#tmpl_list',
                                methods: commonMethods,
                                data: function () {
                                    return data
                                }
                            }, singleView: {
                                template: '#tmpl_single',
                                methods: commonMethods,
                                data: function () {
                                    data.Products = data.Products[0];
                                    return data
                                }
                            }
                        },
                        compiled: function () {

                            invokeUrl();
                            $('.progress').hide();
                            var footer = $(".footer").height();
                            $("body").css("padding-bottom", footer)
                            Countloading();
                        }
                    });

                }
                //end
            }
        }
    })

    //滚动条兼容性
    setTimeout(function () {
        var $sc = $(".swiper-container"),
        $middle = $(".middle"),
        $arrow = $(".arrow"),
        $logoBg = $("#bannerImg");

        $timeBarHei = $sc.height();
        $middle.height($timeBarHei);
        $logoBg.css("margin-top", $timeBarHei);
        $arrow.css({ "margin-bottom": -$arrow.height(), "left": ($middle.width() - $arrow.width()) / 2 });
        if ($logoBg.css("display") == "none" || $logoBg.css("display") == "inline") {
            $(".top-header ").css("padding-top", $timeBarHei);
        }
    }, 200)

    var data = [{}, {}, {}, {}, {}, {}, {}, {}], //产品数据
        listTpl = $('#tmpl_list').html(); //获取模板

    laytpl(listTpl).render(data, function (html) {
        $('#view').find(".lists").hide().end().html(html);
    });
})

//获取url参数
function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}


function getSortFun(order, sortBy) {
    var ordAlpah = (order == 'asc') ? '>' : '<';
    var sortFun = new Function('a', 'b', 'return a.' + sortBy + ordAlpah + 'b.' + sortBy + '?1:-1');
    return sortFun;
}

function Tiaozhuan(e, index) {

    window.location.href = urlStr + "/ActivityHtml/Iflashbuy/" + e + "&change=" + index;
}

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


//统计页面被访问
var Uuid = "";
function Countloading() {

    var myDate = new Date();
    var uuidstr = "";
    var cookieId = "";
    var pids = "";

    if (!GetCookie("loadCookie")) {
        SetCookie("loadCookie", GetDateTime1() + CreateNum())
    }
    cookieId = GetCookie("loadCookie");

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
   
    $(".lists .list").each(function (i) {
        pids += $(this).attr("id") + ",";
    })
    console.log(GetDateTime())
    console.log(window.location.href);
    console.log(uuidstr);
    console.log(pids);
    console.log(cookieId);
    try {
        var e = new Image;
        e.id = "Iflashbuy";
        e.src = "http://t.tuhu.cn/act?tm=" + encodeURIComponent(GetDateTime()) +
                                        "&cp=" + encodeURIComponent(window.location.href) +
                                        "&uid=" + encodeURIComponent(uuidstr) +
                                        "&pids=" + encodeURIComponent(pids) +
                                        "&c_id=" + cookieId;
        //$.ajax({
        //    url: "http://t.tuhu.cn/act",
        //    type: "GET",
        //    dataType: "jsonp",
        //    data: { "tm": myDate.toLocaleString(), "cp": window.location.href, "uid": uuidstr, "pids": pids, "c_id": cookieId },
        //    success: function (data) {

        //    }
        //});
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



var startX, startY, endX, endY;
document.getElementById("divADBox").addEventListener("touchstart", touchStart, false);
document.getElementById("divADBox").addEventListener("touchmove", touchMove, false);
document.getElementById("divADBox").addEventListener("touchend", touchEnd, false);
function touchStart(event) {
    var touch = event.touches[0];
    startY = touch.pageY;
    startX = touch.pageX;
}
function touchMove(event) {
    var touch = event.touches[0];
    //endY = (startY - touch.pageY);
    endX = touch.pageX;
}
function touchEnd(event) {

    //alert("X轴移动大小：" + (startX - endX));
}