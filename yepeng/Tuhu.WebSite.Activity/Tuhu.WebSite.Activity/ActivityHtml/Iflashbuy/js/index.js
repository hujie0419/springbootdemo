!function () {
    alert("alert(activityId);" + activityId);
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
    //域名
    var domainConfig = {};
    domainConfig.TopDomain = ".tuhu.cn";
    if (/\.tuhu\.(\w+)$/.test(location.host))
        domainConfig.TopDomain = ".tuhu." + RegExp.$1;
    domainConfig.ApiSite = "//api" + domainConfig.TopDomain;
    var appUserID, appUserPhone;

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
                        p.SaleOutPercentage = parseInt((p.SaleOutQuantity / p.TotalQuantity) * 100);
                        p.surplusQuantity = p.TotalQuantity - p.SaleOutQuantity;
                        p.Quantity = 0;
                        //是否允许减
                        p.enSub = false;
                        // 是否允许加
                        p.enAdd = true;
                        p.InstallAndPay;
                        p.IsUsePcode;
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
            $.when($resource.getProducts(), $resource.getBuyRecords(appUserID, appUserPhone))
				.then(function (data, records) {
				    callbackInit(data, records || []);
				})
        }
    });
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
                $('.progress').hide();
            }
        });

    }
}();


