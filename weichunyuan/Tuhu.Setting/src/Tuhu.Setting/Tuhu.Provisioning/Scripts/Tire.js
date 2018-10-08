window.TuhuTire = window.TuhuTire || (function (window, document, $) {
    var TuhuTire = {};
    var $win = $(window);
    var $doc = $(document);

    TuhuTire.Domain = (function () {
        var domainConfig = {};

        domainConfig.TopDomain = ".tuhu.cn";
        if (/\.tuhu\.(\w+)$/.test(location.host))
            domainConfig.TopDomain = ".tuhu." + RegExp.$1;


        domainConfig.Image = location.protocol + "//image" + domainConfig.TopDomain;
        domainConfig.Product = location.protocol + "//item" + domainConfig.TopDomain;
        if (domainConfig.TopDomain.indexOf(".tuhu.cn") == -1)
            domainConfig.Image = location.protocol + "//image.tuhu.test";
        return domainConfig;
    })();

    TuhuTire.Cookie = {
        Get: function (name) {
            /// <signature>
            ///		<summary>获得cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<returns type="String" />
            /// </signature>
            if (!name) {
                throw new Error("cookie名称是必须的");
            }
            var cookieName = encodeURIComponent(name) + "=";
            var cookieStart = document.cookie.indexOf(cookieName);
            var cookieValue = null;
            if (cookieStart > -1) {
                var cookieEnd = document.cookie.indexOf(";", cookieStart);
                if (cookieEnd === -1) {
                    cookieEnd = document.cookie.length;
                }
                cookieValue = decodeURIComponent(document.cookie.substring(cookieStart + cookieName.length, cookieEnd).replace(/\+/g, ' '));
            }
            return cookieValue;
        },
        Set: function (name, value, expires, domain, path, secure) {
            /// <signature>
            ///		<summary>设置cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="value" type="String">cookie值</param>
            /// </signature>
            /// <signature>
            ///		<summary>设置cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="value" type="String">cookie值</param>
            ///		<param name="expires" type="Date">cookie有效斯</param>
            /// </signature>
            /// <signature>
            ///		<summary>设置cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="value" type="String">cookie值</param>
            ///		<param name="expires" type="Date">cookie有效斯</param>
            ///		<param name="domain" type="String">cookie域</param>
            /// </signature>
            /// <signature>
            ///		<summary>设置cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="value" type="String">cookie值</param>
            ///		<param name="expires" type="Date">cookie有效斯</param>
            ///		<param name="domain" type="String">cookie域</param>
            ///		<param name="path" type="String">cookie路径</param>
            /// </signature>
            /// <signature>
            ///		<summary>设置cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="value" type="String">cookie值</param>
            ///		<param name="expires" type="Date">cookie有效斯</param>
            ///		<param name="domain" type="String">cookie域</param>
            ///		<param name="path" type="String">cookie路径</param>
            ///		<param name="secure" type="Boolean">true或false</param>
            /// </signature>
            if (!name || arguments.length < 2) {
                throw new Error("cookie名称和值是必须的");
            }
            var cookieText = encodeURIComponent(name) + "=" + encodeURIComponent(value);
            if (expires instanceof Date)
                cookieText += "; expires=" + expires.toGMTString();
            if (domain)
                cookieText += "; domain=" + domain;
            cookieText += "; path=" + (path || "/");
            if (secure)
                cookieText += "; secure";
            document.cookie = cookieText;
        },
        Unset: function (name, domain, path, secure) {
            /// <signature>
            ///		<summary>删除cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            /// </signature>
            /// <signature>
            ///		<summary>删除cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="domain" type="String">cookie域</param>
            /// </signature>
            /// <signature>
            ///		<summary>删除cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="domain" type="String">cookie域</param>
            ///		<param name="path" type="String">cookie路径</param>
            /// </signature>
            /// <signature>
            ///		<summary>删除cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="domain" type="String">cookie域</param>
            ///		<param name="path" type="String">cookie路径</param>
            ///		<param name="secure" type="Boolean">true或false</param>
            /// </signature>
            this.Set(name, "", new Date(0), domain, path, secure);
        },
        GetSub: function (name, subName) {
            /// <signature>
            ///		<summary>获得子cookie</summary>
            ///		<param name="name" type="String">cookie父名称</param>
            ///		<param name="subName" type="String">子cookie名称</param>
            ///		<returns type="String" />
            /// </signature>
            if (!name || !subName) {
                throw new Error("cookie父名称和子名称是必须的");
            }
            var subCookies = this.getAll(name);
            if (!!subCookies)
                return subCookies[subName];
            return null;
        },
        GetAll: function (name) {
            /// <signature>
            ///		<summary>获得所有子cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<returns type="Array" />
            /// </signature>
            if (!name) {
                throw new Error("cookie名称是必须的");
            }
            var cookieValue = this.get(name);
            if (!!cookieValue) {
                var subCookies = cookieValue.split('&');
                var result = {};
                for (var index = 0; index < subCookies.length; index++) {
                    var parts = subCookies[index].split("=");
                    result[decodeURIComponent(parts[0].replace(/\+/g, ' '))] = decodeURIComponent(parts[1].replace(/\+/g, ' '));
                }
                return result;
            }
            return null;
        },
        SetSub: function (name, subName, value, expires, domain, path, secure) {
            /// <signature>
            ///		<summary>设置子cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="subName" type="String">子cookie名称</param>
            ///		<param name="value" type="String">子cookie值</param>
            /// </signature>
            /// <signature>
            ///		<summary>设置子cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="subName" type="String">子cookie名称</param>
            ///		<param name="value" type="String">子cookie值</param>
            ///		<param name="expires" type="Date">cookie有效斯</param>
            /// </signature>
            /// <signature>
            ///		<summary>设置子cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="subName" type="String">子cookie名称</param>
            ///		<param name="value" type="String">子cookie值</param>
            ///		<param name="expires" type="Date">cookie有效斯</param>
            ///		<param name="domain" type="String">cookie域</param>
            /// </signature>
            /// <signature>
            ///		<summary>设置子cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="subName" type="String">子cookie名称</param>
            ///		<param name="value" type="String">子cookie值</param>
            ///		<param name="expires" type="Date">cookie有效斯</param>
            ///		<param name="domain" type="String">cookie域</param>
            ///		<param name="path" type="String">cookie路径</param>
            /// </signature>
            /// <signature>
            ///		<summary>设置子cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="subName" type="String">子cookie名称</param>
            ///		<param name="value" type="String">子cookie值</param>
            ///		<param name="expires" type="Date">cookie有效斯</param>
            ///		<param name="domain" type="String">cookie域</param>
            ///		<param name="path" type="String">cookie路径</param>
            ///		<param name="secure" type="Boolean">true或false</param>
            /// </signature>
            if (!name || !subName || !!value) {
                throw new Error("cookie父名称、子名称和子值是必须的");
            }
            var subCookies = this.getAll(name) || {};
            subCookies[subName] = value;
            this.SetAll(name, subCookies, expires, domain, path, secure);
        },
        SetAll: function (name, subCookies, expires, domain, path, secure) {
            /// <signature>
            ///		<summary>设置cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="subCookies" type="Object">cookie值对象</param>
            /// </signature>
            /// <signature>
            ///		<summary>设置cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="subCookies" type="Object">cookie值对象</param>
            ///		<param name="expires" type="Date">cookie有效斯</param>
            /// </signature>
            /// <signature>
            ///		<summary>设置cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="subCookies" type="Object">cookie值对象</param>
            ///		<param name="expires" type="Date">cookie有效斯</param>
            ///		<param name="domain" type="String">cookie域</param>
            /// </signature>
            /// <signature>
            ///		<summary>设置cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="subCookies" type="Object">cookie值对象</param>
            ///		<param name="expires" type="Date">cookie有效斯</param>
            ///		<param name="domain" type="String">cookie域</param>
            ///		<param name="path" type="String">cookie路径</param>
            /// </signature>
            /// <signature>
            ///		<summary>设置cookie</summary>
            ///		<param name="name" type="String">cookie名称</param>
            ///		<param name="subCookies" type="Object">cookie值对象</param>
            ///		<param name="expires" type="Date">cookie有效斯</param>
            ///		<param name="domain" type="String">cookie域</param>
            ///		<param name="path" type="String">cookie路径</param>
            ///		<param name="secure" type="Boolean">true或false</param>
            /// </signature>
            if (!name || arguments.length < 2 || typeof (subCookies) !== "object") {
                throw new Error("cookie名称和值对象是必须的");
            }
            var cookieText = encodeURIComponent(name) + "=";
            var subCookieParts = [];
            for (var subName in subCookies) {
                if (subName.length > 0 && subCookies.hasOwnProperty(subName)) {
                    subCookieParts.push(encodeURIComponent(subName) + "=" + encodeURIComponent(subCookies[subName]));
                }
            }
            this.Set(name, cookieText + subCookieParts.join("&"), expires, domain, path, secure);
        }
    };
    //格式化
    TuhuTire.Format = {
        FormatCurrency: function (num) {
            //四舍五入(保留两位小数)
            num = num.toString().replace(/\$|\,/g, '');
            if (isNaN(num))
                num = "0";
            sign = (num == (num = Math.abs(num)));
            num = Math.floor(num * 100 + 0.50000000001);
            cents = num % 100;
            num = Math.floor(num / 100).toString();
            if (cents < 10)
                cents = "0" + cents;
            for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3); i++)
                num = num.substring(0, num.length - (4 * i + 3)) + ',' +
                    num.substring(num.length - (4 * i + 3));
            return (((sign) ? '' : '-') + num + '.' + cents);
        },
        FormatCurrencyTenThou: function (num) {
            //四舍五入(保留一位小数)
            num = num.toString().replace(/\$|\,/g, '');
            if (isNaN(num))
                num = "0";
            sign = (num == (num = Math.abs(num)));
            num = Math.floor(num * 10 + 0.50000000001);
            cents = num % 10;
            num = Math.floor(num / 10).toString();
            for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3); i++)
                num = num.substring(0, num.length - (4 * i + 3)) + ',' +
                    num.substring(num.length - (4 * i + 3));
            return (((sign) ? '' : '-') + num + '.' + cents);
        }
    };
    TuhuTire.Dialog = (function () {
        var dialogList = []; //弹出框列表
        var isDialogShow = false;
        var WindowsList = {}; //窗体列表
        return {
            Popup: function (div, option) {
                /// <signature>
                ///		<summary>创建模态窗口</summary>
                ///		<param name="div" type="Element">需要被创建为模态窗口的引用</param>
                ///		<param name="option" type="Object">选项，默认非全屏</param>
                ///		<returns type="Element" />
                /// </signature>
                option = $.extend({
                    fullScreen: true,
                    opacity: 0.1,
                    clone: true,
                    bgColor: "black",
                    zIndex: 9999,
                    draggable: false,
                    disableScroll: false
                }, option);
                var jqDialog = $(div);
                //jqDialog.find("#noSelectCar").click(function () {
                //    jqDialog = $('<form><label for="myModel">您的车型</label><input type="text" id="myModel" /><br /><label for="myTel"><span class="required">*</span>手机号码</label><input type="text" id="myTel" /></form>');
                //    console.log(jqDialog);
                //});
                var background = undefined;
                if (option.clone)
                    jqDialog = jqDialog.clone(true, true);
                jqDialog.appendTo(document.body).show();
                var dialog = jqDialog[0];
                dialog.close = function (event) {
                    $win.off("resize scroll", dialog.adjustPosition);
                    if (background)
                        document.body.removeChild(background);
                    document.body.removeChild(dialog);
                    dialog.adjustPosition = undefined;
                    event && event.preventDefault();
                    jqDialog.triggerHandler("close");
                    jqDialog.off("close");
                    if (option.disableScroll)
                        $doc.off("mousewheel DOMMouseScroll", false);
                };
                document.body.appendChild(dialog);
                dialog.style.zIndex = option.zIndex;
                jqDialog.close = function () {
                    dialog.close();
                };
                jqDialog.showDialog = function () {
                    background.style.display = "block";
                    jqDialog.show();
                };
                jqDialog.hideDialog = function () {
                    background.style.display = "none";
                    jqDialog.hide();
                };

                var documentElement = document.documentElement;

                if (option.fullScreen) {
                    background = document.createElement("div");
                    background.style.position = "fixed";
                    background.style.zIndex = option.zIndex - 1;
                    background.style.backgroundColor = option.bgColor;
                    background.style.filter = 'alpha(opacity=' + option.opacity * 100 + ')';
                    background.style.opacity = option.opacity;
                    document.body.appendChild(background);

                    background.style.top = background.style.left = background.style.bottom = background.style.right = 0;
                }
                dialog.style.position = "fixed";
                dialog.adjustPosition = function () {
                    if (navigator.appName.toLowerCase().indexOf("Microsoft".toLowerCase()) == -1) {
                        dialog.style.top = (documentElement.clientHeight - dialog.clientHeight) / 2 + "px";
                        dialog.style.left = (documentElement.clientWidth - dialog.clientWidth) / 2 + "px";
                    } else {
                        var zoom = document.body.style.zoom;
                        if (zoom.endsWith("%"))
                            zoom = parseFloat(zoom) / 100;
                        else
                            zoom = parseFloat(zoom);

                        if (isNaN(zoom))
                            zoom = 1.0;
                        dialog.style.top = (documentElement.clientHeight / zoom - dialog.clientHeight) / 2 + "px";
                        dialog.style.left = (documentElement.clientWidth / zoom - dialog.clientWidth) / 2 + "px";
                    }
                };

                if (typeof (option.onclose) === "function")
                    jqDialog.on("close", option.onclose);
                option.closedelay = option.closedelay ? option.closedelay : 0;
                if (typeof (option.closebefore) === "function") {
                    (option.closeHandle ? $(option.closeHandle, jqDialog) : jqDialog).on("click", function () {
                        option.closebefore();
                        setTimeout(function () {
                            dialog.close();
                        }, option.closedelay);
                    });
                } else {
                    (option.closeHandle ? $(option.closeHandle, jqDialog) : jqDialog).click(dialog.close);
                }
                if (option.disableScroll)
                    $doc.on("mousewheel DOMMouseScroll", false);

                $win.on("resize", dialog.adjustPosition);
                dialog.adjustPosition();

                if (option.draggable || option.dragHandle || dialog.getAttribute("draggable") === "true")
                    (option.fullScreen ? jqDialog : jqDialog.add(background)).drag(option.dragHandle);

                return jqDialog;
            },
            PopupDialog: function (panle, option) {
                /// <summary>
                /// 将指定的元素显示在外面（含外框）
                /// </summary>
                option = $.extend({
                    closeButton: true,
                    title: "提示",
                    fullScreen: true,
                    opacity: 0.1
                }, option);

                var Popup = this.Popup;
                var dialog = $(panle);

                dialog.on("close", function () {
                    var obj = dialogList.shift();
                    if (obj)
                        Popup(obj.dialog, obj.option);
                    else
                        isDialogShow = false;
                });
                if (isDialogShow)
                    dialogList.push({
                        dialog: dialog,
                        option: option
                    });
                else {
                    isDialogShow = true;
                    Popup(dialog, option);
                }
            },
            //console.log: function (message, option)
            //{
            //	option = $.extend({ title: "友情提示", closeButton: true, closor: ".confirm", width: (message || "").byteLength() * 10 + 100, height: 160 }, option);
            //	var panle = $("<div class='message'></div><div class='confirm'><div class='border'><div>确定</div></div></div>");
            //	panle.filter(".message").text(message);
            //	this.PopupDialog(panle, option);
            //},
            //error: function (message, option)
            //{
            //	option = $.extend({ title: "提示", closeButton: false, closor: ".confirm", width: (message || "").byteLength() * 10 + 100, height: 160 }, option);
            //	var panle = $("<div class='message'></div><div class='confirm'><div class='border'><div>确定</div></div></div>");
            //	panle.filter(".message").text(message);
            //	this.PopupDialog(panle, option);
            //},
            //confirm: function (message, callback, option)
            //{
            //	option = $.extend({ title: "提示", closeButton: true, closor: ".confirm, .cancel", width: (message || "").byteLength() * 10 + 100, height: 160 }, option);
            //	var panle = $("<div class='message'></div><div class='buttons'><div class='confirm'><div class='border'><div>确定</div></div></div><div class='cancel'><div>取消</div></div></div>");
            //	panle.filter(".message").text(message);
            //	if (typeof (callback) === "function")
            //	{
            //		panle.find(".confirm").click(function ()
            //		{
            //			callback(true);
            //		});
            //		panle.find(".cancel").click(function ()
            //		{
            //			callback(false);
            //		});
            //	}
            //	this.PopupDialog(panle, option);
            //},
            openCenterWindow: function (url, name, width, height) {
                /// <signature>
                ///		<summary>打开居中窗口</summary>
                ///		<param name="url" type="String">Url</param>
                ///		<param name="name" type="String">窗口名称，默认为url的MD5值</param>
                ///		<param name="width" type="Number">窗口宽，默认400px</param>
                ///		<param name="height" type="Number">窗口高，默认300px</param>
                /// </signature>
                name = name || Ebdoor.Security.ComputeMD5(url);
                if (WindowsList[name] && !WindowsList[name].closed) {
                    var winHandle = WindowsList[name];
                    winHandle.focus();
                    return;
                }
                width = width || 400;
                height = height || 300;
                var sw = screen.availWidth || screen.width;
                var sh = screen.availHeight || screen.height;
                var l = width < sw ? (sw - width) / 2 : 0;
                var t = height < sh ? (sh - height) / 2 : 0;
                var features = ' width=' + width + ',height=' + height + ',left=' + l + ',top=' + t;
                var winHandle = window.open(url, name, features);
                WindowsList[name] = winHandle;
                winHandle.focus();
            }
        };
    })();
    TuhuTire.Utility = {
        //选项卡(hover切换)tags是hover的位置  tagContents是对应的展示内容
        SwitchTags: function (tags, tagContents) {
            $(document).on("mouseover", tags, function () {
                var _this = $(this);
                $(tags).removeClass("current");
                _this.addClass("current");
                $(tagContents).removeClass("current").eq($(tags).index(_this)).addClass("current");
            });
        },
        switchTagsClick: function (tags, tagContents) {
            $(document).on("click", tags, function () {
                var _this = $(this);
                $(tags).removeClass("current");
                _this.addClass("current");
                $(tagContents).removeClass("current").eq($(tags).index(_this)).addClass("current");
            });
        },
        //获取GUID格式随机字符串
        GetUUID: function () {
            var s = [];
            var hexDigits = "0123456789abcdef";
            for (var i = 0; i < 36; i++) {
                s[i] = hexDigits.substr(Math.floor(Math.random() * 0x10), 1);
            }
            s[14] = "4";
            s[19] = hexDigits.substr((s[19] & 0x3) | 0x8, 1);
            s[8] = s[13] = s[18] = s[23] = "-";

            var uuid = s.join("");
            return uuid;
        },
    }
    Array.prototype.in_array = function (e) {
        for (i = 0; i < this.length && this[i] != e; i++);
        return !(i == this.length);
    }
    TuhuTire.Logic = {
        ListActivity: {
            ShowMoreTireSize: function () {
                $(".tiresize-append .hide").removeClass("hide");
                $(".tiresize-append .more").hide();
            },
            HasSelected: function () {
                return $("[name=selectSingle]:checked").length;
            },
            BatchAdd: function () {
                if (!TuhuTire.Logic.ListActivity.HasSelected()) {
                    swal("未选中", "当前无任何选中,无法添加", "error");
                    return false;
                }
                var VehicleTireSize = [];
                var VehicleTireSizeTemp = [];
                $("[name=selectSingle]:checked").each(function () {
                    var obj = {
                        VehicleID: $(this).data("vehicleid"),
                        TireSize: $(this).data("tiresize")
                    }

                    if (!VehicleTireSizeTemp.in_array(obj.VehicleID + "|" + obj.TireSize)) {//同车型同规格多条只添加一条
                        VehicleTireSizeTemp.push(obj.VehicleID + "|" + obj.TireSize);
                        VehicleTireSize.push(obj);
                    }
                });
                if (VehicleTireSize.length > 0) {
                    var tempTireSize = VehicleTireSize[0].TireSize;
                    var flag = false;
                    $.each(VehicleTireSize, function (index, value) {
                        if (value.TireSize != tempTireSize)
                            flag = true;
                    });
                    if (flag)
                        swal("不同规格", "当前选中存在不同轮胎规格,无法添加", "error");
                    else {
                        var $dialog = $("#BatchAddItem");
                        $dialog.find(".status select").val('1');
                        $dialog.find(".img").attr("src", "");
                        $dialog.find(".buttontype select").val('0');
                        $dialog.find(".getruleid").hide();
                        $dialog.find("input[type=text]").val("");
                        $dialog.find("span.error").remove();
                        $dialog.find(".pid input").next().text("").removeClass("no").removeClass("yes");
                        $dialog.find(".pid").attr("data-tiresize", tempTireSize);
                        $("#BatchAddItem").dialog({
                            title: "批量添加活动",
                            width: 500,
                            height: 800,
                            modal: true,
                            buttons: {
                                "保存": function () {
                                    var dialog = $(this);
                                    var ActivityName = dialog.find(".activityname input").val(),
                                        Status = dialog.find(".status select>option:selected").val(),
                                        Sort = dialog.find(".Sort select>option:selected").val(),
                                        StartTime = dialog.find("#BitchAddStartTime").val(),
                                        EndTime = dialog.find("#BitchAddEndTime").val(),
                                        Image = dialog.find(".image .img").attr("src"),
                                        Image2 = dialog.find(".image2 .img").attr("src"),
                                        Icon = dialog.find(".icon .img").attr("src"),
                                        ButtonType = dialog.find(".buttontype select>option:selected").val(),
                                        GetRuleGUID = undefined,
                                        ButtonText = dialog.find(".buttontext input").val(),
                                        PID1 = dialog.find("[name=PID1]").val().trim(),
                                        PID2 = dialog.find("[name=PID2]").val().trim(),
                                        PID3 = dialog.find("[name=PID3]").val().trim();
                                    var canSave = true;
                                    if (!ActivityName.length) {
                                        canSave = false;
                                        TuhuTire.Logic.ListActivity.ShowErrorTit(dialog.find(".activityname"), "请填写活动名称");
                                    }
                                    if (!StartTime.length || !EndTime.length) {
                                        canSave = false;
                                        TuhuTire.Logic.ListActivity.ShowErrorTit(dialog.find(".activitytime"), "请填写活动时间");
                                    }
                                    else if (new Date(StartTime) >= new Date(EndTime)) {
                                        canSave = false;
                                        TuhuTire.Logic.ListActivity.ShowErrorTit(dialog.find(".activitytime"), "请填写正确活动时间");
                                    }
                                    if (!Image.length) {
                                        canSave = false;
                                        TuhuTire.Logic.ListActivity.ShowErrorTit(dialog.find(".image"), "请上传浮层图片");
                                    }
                                    if (!Image2.length) {
                                        canSave = false;
                                        TuhuTire.Logic.ListActivity.ShowErrorTit(dialog.find(".image2"), "请上传新浮层图片");
                                    }
                                    if (!Icon.length) {
                                        canSave = false;
                                        TuhuTire.Logic.ListActivity.ShowErrorTit(dialog.find(".icon"), "请上传活动图标");
                                    }
                                    if (ButtonType == 1) {
                                        GetRuleGUID = dialog.find(".getruleguid input").val();
                                        if (!GetRuleGUID.length) {
                                            canSave = false;
                                            TuhuTire.Logic.ListActivity.ShowErrorTit(dialog.find(".getruleguid"), "请填写优惠券GUID");
                                        }
                                        else {
                                            $.ajax({
                                                type: 'get',
                                                async: false,
                                                url: "/Tire/CheckGetRuleGUID",
                                                data: { guid: GetRuleGUID },
                                                dataType: "json",
                                                success: function (result) {
                                                    if (result <= 0) {
                                                        canSave = false;
                                                        TuhuTire.Logic.ListActivity.ShowErrorTit(dialog.find(".getruleguid"), "请填写正确的优惠券GUID");
                                                    }
                                                }
                                            });
                                        }
                                    }

                                    if (!ButtonText.length) {
                                        canSave = false;
                                        TuhuTire.Logic.ListActivity.ShowErrorTit(dialog.find(".buttontext"), "请填写按钮文字");
                                    }


                                    var Products = [];
                                    var repeat = false;
                                    if (PID1.length) {
                                        if (dialog.find("[name=PID1]").next().hasClass("yes")) {
                                            if (Products.length) {
                                                $.each(Products, function (index, value) {
                                                    if (value.PID == PID1)
                                                        repeat = true;
                                                });
                                            }
                                            Products.push({
                                                PID: PID1,
                                                Postion: 1
                                            });
                                        }
                                        else
                                            canSave = false;
                                    }
                                    if (PID2.length) {
                                        if (dialog.find("[name=PID2]").next().hasClass("yes")) {
                                            if (Products.length) {
                                                $.each(Products, function (index, value) {
                                                    if (value.PID == PID2)
                                                        repeat = true;
                                                });
                                            }
                                            Products.push({
                                                PID: PID2,
                                                Postion: 2
                                            });
                                        }
                                        else
                                            canSave = false;
                                    }
                                    if (PID3.length) {
                                        if (dialog.find("[name=PID3]").next().hasClass("yes")) {
                                            if (Products.length) {
                                                $.each(Products, function (index, value) {
                                                    if (value.PID == PID3)
                                                        repeat = true;
                                                });
                                            }
                                            Products.push({
                                                PID: PID3,
                                                Postion: 3
                                            });
                                        }
                                        else
                                            canSave = false;
                                    }
                                    if (repeat) {
                                        swal("重复", "关联PID不允许出现相同PID", "error");
                                        return false;
                                    }
                                    if (canSave) {
                                        $.post("/Tire/SaveBitchAdd", {
                                            ActivityName: ActivityName,
                                            Status: Status,
                                            Sort: Sort,
                                            StartTime: StartTime,
                                            EndTime: EndTime,
                                            Image: Image,
                                            Image2: Image2,
                                            Icon: Icon,
                                            ButtonType: ButtonType,
                                            GetRuleGUID: GetRuleGUID,
                                            ButtonText: ButtonText,
                                            Products: Products.length ? JSON.stringify(Products) : undefined,
                                            VehicleTireSize: JSON.stringify(VehicleTireSize)
                                        }, function (result) {
                                            if (result == -98)
                                                swal("异常", "车型和轮胎尺寸未捕捉到,请刷新页面重试", "error");
                                            else if (result == -97)
                                                swal("错误", "不允许同一车型规格同一优先级同时开启多个活动", "error");
                                            else if (result < 0)
                                                swal("失败", "保存失败", "error");
                                            else if (result > 0) {
                                                swal("成功", "保存成功", "success");
                                                dialog.dialog("close");
                                                $("#PartRefresh").click();
                                            }
                                        });
                                    }
                                },
                                "关闭": function () {
                                    $(this).dialog("close"); //当点击取消按钮时，关闭窗口
                                }
                            }
                        });
                    }
                }

            },
            BatchOn: function () {
                if (!TuhuTire.Logic.ListActivity.HasSelected()) {
                    swal("未选中", "当前无任何选中,无法启用", "error");
                    return false;
                }
                var ActivityIDs = [];

                $("[name=selectSingle]:checked").each(function () {
                    var aid = $(this).data("aid");
                    if (aid.length)
                        ActivityIDs.push(aid);
                });
                if (!ActivityIDs.length) {
                    swal("未合法选中", "当前未选中活动,无法启用", "error");
                    return false;
                }
                swal({
                    title: "确认批量启用?",
                    type: "warning",//图标
                    showCancelButton: true,//是否展示取消按钮
                    confirmButtonColor: "#DD6B55",//确认按钮颜色
                    confirmButtonText: "确认!",//确认按钮的文本
                    cancelButtonText: "取消",//取消按钮的文本
                    closeOnConfirm: false,//点击确认按钮后是否关闭窗口
                },
                    function (isConfirm) {
                        if (isConfirm) {
                            //点击确认按钮后执行
                            $.post("/Tire/BitchOn", { ActivityIDs: ActivityIDs.join(',') }, function (result) {
                                if (result > 0) {
                                    swal("成功", "启用成功", "success");
                                    $("#PartRefresh").click();
                                }
                                else if (result == -99)
                                    swal("错误", "不允许同一车型规格同一优先级同时启用多个活动", "error");
                                else
                                    swal("失败", "启用失败", "error");
                            });
                        }
                    });
            },
            BatchOff: function () {
                if (!TuhuTire.Logic.ListActivity.HasSelected()) {
                    swal("未选中", "当前无任何选中,无法禁用", "error");
                    return false;
                }
                var ActivityIDs = [];

                $("[name=selectSingle]:checked").each(function () {
                    var aid = $(this).data("aid");
                    if (aid.length)
                        ActivityIDs.push(aid);
                });
                if (!ActivityIDs.length) {
                    swal("未合法选中", "当前未选中活动,无法禁用", "error");
                    return false;
                }
                swal({
                    title: "确认批量禁用?",
                    type: "warning",//图标
                    showCancelButton: true,//是否展示取消按钮
                    confirmButtonColor: "#DD6B55",//确认按钮颜色
                    confirmButtonText: "确认!",//确认按钮的文本
                    cancelButtonText: "取消",//取消按钮的文本
                    closeOnConfirm: false,//点击确认按钮后是否关闭窗口
                },
                    function (isConfirm) {
                        if (isConfirm) {
                            //点击确认按钮后执行
                            $.post("/Tire/BitchOff", { ActivityIDs: ActivityIDs.join(',') }, function (result) {
                                if (result > 0) {
                                    swal("成功", "禁用成功", "success");
                                    $("#PartRefresh").click();
                                }
                                else
                                    swal("失败", "禁用失败", "error");
                            });
                        }
                    });
            },
            ReplaceItem: function () {
                if (!TuhuTire.Logic.ListActivity.HasSelected()) {
                    swal("未选中", "当前无任何选中,无法替换", "error");
                    return false;
                }
                var ActivityIDs = [];

                $("[name=selectSingle]:checked").each(function () {
                    var aid = $(this).data("aid");
                    if (aid.length)
                        ActivityIDs.push(aid);
                });
                if (!ActivityIDs.length) {
                    swal("未合法选中", "当前未选中活动,无法替换", "error");
                    return false;
                }


                var $dialog = $("#ReplaceItem");
                $dialog.find(".img").attr("src", "");
                $dialog.find("input[type=text]").val("");
                $dialog.find("span.error").remove();
                $("#ReplaceItem").dialog({
                    title: "批量替换",
                    width: 500,
                    height: 800,
                    modal: true,
                    buttons: {
                        "保存": function () {
                            var dialog = $(this);
                            var ActivityName = dialog.find(".activityname input").val().trim(),
                                Image = dialog.find(".image .img").attr("src").trim(),
                                Image2 = dialog.find(".image2 .img").attr("src").trim(),
                                Icon = dialog.find(".icon .img").attr("src").trim(),
                                ButtonText = dialog.find(".buttontext input").val().trim();
                            $.post("/Tire/ReplaceListActivityItem", {
                                ActivityName: ActivityName,
                                Image: Image,
                                Icon: Icon,
                                Image2: Image2,
                                ButtonText: ButtonText,
                                ActivityIDs: ActivityIDs.join(';')
                            }, function (result) {
                                if (result > 0) {
                                    swal("成功", "保存成功", "success");
                                    dialog.dialog("close");
                                    $("#PartRefresh").click();
                                }
                                else
                                    swal("失败", "保存失败", "error");
                            });
                        },
                        "关闭": function () {
                            $(this).dialog("close"); //当点击取消按钮时，关闭窗口
                        }
                    }
                });

            },
            UploadImage: function (target) {
                var $target = $(target);
                var $parent = $target.parent();
                var $img = $target.prev();
                var now = new Date().getTime();
                $target.attr("id", now);
                $target.attr("name", now);
                if ($target.val().split('.')[1].toUpperCase() != "JPG"
                    && $target.val().split('.')[1].toUpperCase() != "JPEG"
                    && $target.val().split('.')[1].toUpperCase() != "GIF"
                    && $target.val().split('.')[1].toUpperCase() != "PNG"
                    && $target.val().split('.')[1].toUpperCase() != "BMP") {
                    swal("格式错误", "支持jpg、jpeg、gif、png、bmp", "error");
                    return false;
                }

                $.ajaxFileUpload(
                    {
                        url: '/Tire/ImageUploadToAli',
                        secureuri: false,
                        fileElementId: now,
                        async: false,
                        dataType: 'JSON',
                        type: 'post',
                        success: function (src) {
                            if (src != null) {
                                $img.attr("src", src);
                                TuhuTire.Logic.ListActivity.ClearErrorTit($parent);
                                $("#" + now).val("");

                            } else
                                swal("上传失败！");
                        },
                        //complete: function (xmlHttpRequest) {
                        //    //$target.replaceWith('<input type="file"  style="display:none;" />');
                        //    //$target.attr("id", now);
                        //    //$target.attr("name", now);
                        //    //$target.on("change", function (ev) {
                        //    //    var filename = $(this).val();
                        //    //    alert(filename);
                        //    //    TuhuTire.Logic.ListActivity.UploadImage(ev);
                        //    //});
                        //    $target.on("click", function (ev) {
                        //        $(ev).value = '';
                        //        console.log($(ev).value);
                        //    });
                        //    $target.on("change", function (ev) {
                        //        TuhuTire.Logic.ListActivity.UploadImage(this);
                        //        console.log($(ev).value);
                        //    });
                        //   },
                    });

            },
            ShowErrorTit: function ($target, errorMsg) {
                $target.find("span.error").remove();
                $target.append('<span class="error">' + errorMsg + '</span>');
            },
            ClearErrorTit: function ($target) {
                $target.find("span.error").remove();
            },
            CheckPID: function (target) {
                var $target = $(target);
                if ($target.val().length) {
                    $.ajax({
                        type: 'post',
                        async: false,
                        url: "/Tire/CheckPID",
                        data: {
                            PID: $target.val(),
                            TireSize: $target.parent().attr("data-tiresize")
                        },
                        dataType: "json",
                        success: function (result) {
                            $target.next().text("").removeClass("no").removeClass("yes");
                            if (result == 1)
                                $target.next().text("PID不存在").addClass("no");
                            else if (result == 3)
                                $target.next().text("PID尺寸不匹配").addClass("no");
                            else
                                $target.next().text(result).addClass("yes");

                        }
                    });
                }
            },
            ShowRelationPIDs: function (ActivityID) {
                $("#RelationPIDItem").empty();
                $.get("/Tire/SelectRelationPIDs", { ActivityID: ActivityID }, function (data) {
                    if (data && data.length) {
                        var html = '';
                        $.each(data, function (index, value) {
                            html += '<div style="margin-bottom:30px;"><span>关联PID' + value.Postion + '：</span><a target="_blank" href="' + TuhuTire.Domain.Product + '/Products/' + value.PID.replace('|', '/') + '.html">' + value.PID + '</a></div>';
                        });
                        $("#RelationPIDItem").html(html);
                    }
                    else
                        $("#RelationPIDItem").html("<span>无</span>")
                });
                $("#RelationPIDItem").dialog({
                    title: "展示关联PID",
                    width: 300,
                    height: 200,
                    modal: true
                });
            },
            Delete: function (activityID) {
                swal({
                    title: "确认删除?",
                    type: "warning",//图标
                    showCancelButton: true,//是否展示取消按钮
                    confirmButtonColor: "#DD6B55",//确认按钮颜色
                    confirmButtonText: "确认!",//确认按钮的文本
                    cancelButtonText: "取消",//取消按钮的文本
                    closeOnConfirm: false,//点击确认按钮后是否关闭窗口
                },
                    function (isConfirm) {
                        if (isConfirm) {
                            //点击确认按钮后执行
                            $.post("/Tire/DeleteListActivity", { ActivityID: activityID }, function (result) {
                                if (result > 0) {
                                    swal("成功", "删除成功", "success");
                                    $("#PartRefresh").click();
                                }
                                else
                                    swal("失败", "删除失败", "error");
                            });
                        }
                    });

            },
            EditActivity: function (activityID) {
                $.get("/Tire/GetListActivityByID", { ActivityID: activityID }, function (data) {
                    var $dialog = $("#BatchAddItem");
                    $dialog.find(".activityname input").val(data.ActivityName);
                    $dialog.find(".Sort select").val(data.Sort);
                    $dialog.find(".status select").val(data.Status);
                    $dialog.find("#BitchAddStartTime").val(data.StartTimeStr);
                    $dialog.find("#BitchAddEndTime").val(data.EndTimeStr);
                    $dialog.find(".image .img").attr("src", data.Image);
                    $dialog.find(".image2 .img").attr("src", data.Image2);
                    $dialog.find(".icon .img").attr("src", data.Icon);
                    $dialog.find(".buttontype select").val(data.ButtonType);
                    $dialog.find(".getruleguid input").val(data.GetRuleGUID);
                    if (data.ButtonType == 1)
                        $dialog.find(".getruleguid").show();
                    else
                        $dialog.find(".getruleguid").hide();
                    $dialog.find(".buttontext input").val(data.ButtonText);

                    $dialog.find("span.error").remove();
                    $dialog.find(".pid input").next().text("").removeClass("no").removeClass("yes");
                    $dialog.find(".pid").attr("data-tiresize", data.TireSize);
                    $dialog.find("[name=PID1]").val("");
                    $dialog.find("[name=PID2]").val("");
                    $dialog.find("[name=PID3]").val("");
                    if (data.Products && data.Products.length) {
                        $.each(data.Products, function (index, value) {
                            if (value.Postion == 1)
                                $dialog.find("[name=PID1]").val(value.PID).next().addClass("yes").text(value.DisplayName);
                            else if (value.Postion == 2)
                                $dialog.find("[name=PID2]").val(value.PID).next().addClass("yes").text(value.DisplayName);
                            else if (value.Postion == 3)
                                $dialog.find("[name=PID3]").val(value.PID).next().addClass("yes").text(value.DisplayName);
                        });
                    }

                    $("#BatchAddItem").dialog({
                        title: "编辑",
                        width: 500,
                        height: 800,
                        modal: true,
                        buttons: {
                            "保存": function () {
                                var dialog = $(this);
                                var ActivityName = dialog.find(".activityname input").val(),
                                    Status = dialog.find(".status select>option:selected").val(),
                                    Sort = dialog.find(".Sort select>option:selected").val(),
                                    StartTime = dialog.find("#BitchAddStartTime").val(),
                                    EndTime = dialog.find("#BitchAddEndTime").val(),
                                    Image = dialog.find(".image .img").attr("src"),
                                    Image2 = dialog.find(".image2 .img").attr("src"),
                                    Icon = dialog.find(".icon .img").attr("src"),
                                    ButtonType = dialog.find(".buttontype select>option:selected").val(),
                                    GetRuleGUID = undefined,
                                    ButtonText = dialog.find(".buttontext input").val(),
                                    PID1 = dialog.find("[name=PID1]").val().trim(),
                                    PID2 = dialog.find("[name=PID2]").val().trim(),
                                    PID3 = dialog.find("[name=PID3]").val().trim();
                                var canSave = true;
                                if (!ActivityName.length) {
                                    canSave = false;
                                    TuhuTire.Logic.ListActivity.ShowErrorTit(dialog.find(".activityname"), "请填写活动名称");
                                }
                                if (!StartTime.length || !EndTime.length) {
                                    canSave = false;
                                    TuhuTire.Logic.ListActivity.ShowErrorTit(dialog.find(".activitytime"), "请填写活动时间");
                                }
                                else if (new Date(StartTime) >= new Date(EndTime)) {
                                    canSave = false;
                                    TuhuTire.Logic.ListActivity.ShowErrorTit(dialog.find(".activitytime"), "请填写正确活动时间");
                                }
                                if (!Image.length) {
                                    canSave = false;
                                    TuhuTire.Logic.ListActivity.ShowErrorTit(dialog.find(".image"), "请上传浮层图片");
                                }
                                if (!Image2.length) {
                                    canSave = false;
                                    TuhuTire.Logic.ListActivity.ShowErrorTit(dialog.find(".image2"), "请上传新浮层图片");
                                }
                                if (!Icon.length) {
                                    canSave = false;
                                    TuhuTire.Logic.ListActivity.ShowErrorTit(dialog.find(".icon"), "请上传活动图标");
                                }
                                if (ButtonType == 1) {
                                    GetRuleGUID = dialog.find(".getruleguid input").val();
                                    if (!GetRuleGUID.length) {
                                        canSave = false;
                                        TuhuTire.Logic.ListActivity.ShowErrorTit(dialog.find(".getruleguid"), "请填写优惠券GUID");
                                    }
                                    else {
                                        $.ajax({
                                            type: 'get',
                                            async: false,
                                            url: "/Tire/CheckGetRuleGUID",
                                            data: { guid: GetRuleGUID },
                                            dataType: "json",
                                            success: function (result) {
                                                if (result <= 0) {
                                                    canSave = false;
                                                    TuhuTire.Logic.ListActivity.ShowErrorTit(dialog.find(".getruleguid"), "请填写正确的优惠券GUID");
                                                }
                                            }
                                        });
                                    }
                                }

                                if (!ButtonText.length) {
                                    canSave = false;
                                    TuhuTire.Logic.ListActivity.ShowErrorTit(dialog.find(".buttontext"), "请填写按钮文字");
                                }


                                var Products = [];
                                var repeat = false;
                                if (PID1.length) {
                                    if (dialog.find("[name=PID1]").next().hasClass("yes")) {
                                        if (Products.length) {
                                            $.each(Products, function (index, value) {
                                                if (value.PID == PID1)
                                                    repeat = true;
                                            });
                                        }
                                        Products.push({
                                            PID: PID1,
                                            Postion: 1
                                        });
                                    }
                                    else
                                        canSave = false;
                                }
                                if (PID2.length) {
                                    if (dialog.find("[name=PID2]").next().hasClass("yes")) {
                                        if (Products.length) {
                                            $.each(Products, function (index, value) {
                                                if (value.PID == PID2)
                                                    repeat = true;
                                            });
                                        }
                                        Products.push({
                                            PID: PID2,
                                            Postion: 2
                                        });
                                    }
                                    else
                                        canSave = false;
                                }
                                if (PID3.length) {
                                    if (dialog.find("[name=PID3]").next().hasClass("yes")) {
                                        if (Products.length) {
                                            $.each(Products, function (index, value) {
                                                if (value.PID == PID3)
                                                    repeat = true;
                                            });
                                        }
                                        Products.push({
                                            PID: PID3,
                                            Postion: 3
                                        });
                                    }
                                    else
                                        canSave = false;
                                }
                                if (repeat) {
                                    swal("重复", "关联PID不允许出现相同PID", "error");
                                    return false;
                                }
                                if (canSave) {

                                    $.post("/Tire/EditActivity", {
                                        ActivityName: ActivityName,
                                        Status: Status,
                                        Sort: Sort,
                                        StartTime: StartTime,
                                        EndTime: EndTime,
                                        Image: Image,
                                        Image2: Image2,
                                        Icon: Icon,
                                        ButtonType: ButtonType,
                                        GetRuleGUID: GetRuleGUID,
                                        ButtonText: ButtonText,
                                        Products: Products.length ? JSON.stringify(Products) : undefined,
                                        ActivityID: data.ActivityID,
                                        OldStatus: data.Status,
                                        TireSize: data.TireSize,
                                        VehicleId: data.VehicleId
                                    }, function (result) {
                                        if (result == -97)
                                            swal("错误", "不允许同一车型规格同一优先级同时开启多个活动", "error");
                                        else if (result < 0)
                                            swal("失败", "保存失败", "error");
                                        else if (result > 0) {
                                            swal("成功", "保存成功", "success");
                                            dialog.dialog("close");
                                            $("#PartRefresh").click();
                                        }
                                    });
                                }
                            },
                            "关闭": function () {
                                $(this).dialog("close"); //当点击取消按钮时，关闭窗口
                            }
                        }
                    });
                });
            },
        },
        InstallNow: {
            LoadList: function (pageIndex) {
                $(".loadimg").show();

                var PID = $("[name=PID]").val();
                var Status = $(".status option:selected").val();
                var CityIds = [];
                var $checkbox = $(".condition .region").find("[type=checkbox]");
                if ($checkbox.length) {
                    $checkbox.each(function () {
                        if ($(this).prop("checked") && $(this).val() && !isNaN($(this).val()))
                            CityIds.push($(this).val());
                    })
                }
                console.log(CityIds);
                //规格尺寸
                var TireSize = "";
                var Tire_Width = $("#Tire_Width option:selected").val();
                var Tire_AspectRatio = $("#Tire_AspectRatio option:selected").val();
                var Tire_Rim = $("#Tire_Rim option:selected").val();
                if (Tire_Width.length && Tire_AspectRatio.length && Tire_Rim.length) {
                    TireSize = Tire_Width + '/' + Tire_AspectRatio + 'R' + Tire_Rim;
                }

                $.post("/Tire/InstallNowList", {
                    CityIds: CityIds.join(';'),
                    PID: PID,
                    Status: !Status.length ? undefined : Status == 1,
                    TireSize: TireSize,
                    PageSize: $(".PageSize option:selected").val(),
                    PageIndex: pageIndex,
                }, function (html, status) {
                    if (status === "success") {
                        $(".loadimg").hide();
                        $(".list").html(html);
                        $(".data_count").text('共' + ($(".table").length ? $(".table").attr("data-count") : 0) + '条数据');
                    }
                });
            },
            ShowAddDialog: function () {
                var dialog = TuhuTire.Dialog.Popup("#add", {
                    clone: true, draggable: false, closeHandle: ".close,.closeBtn", disableScroll: false, opacity: 0.3
                });
                dialog.find(".save").on("click", function () {
                    var CityIds = [];
                    var $checkbox = dialog.find("[type=checkbox]");
                    if ($checkbox.length) {
                        $checkbox.each(function () {
                            if ($(this).prop("checked") && $(this).val() && !isNaN($(this).val()))
                                CityIds.push($(this).val());
                        })
                    }
                    if (!CityIds.length) {
                        swal("错误", "请选择地区!", "error");
                        return false;
                    }

                    var $pids = dialog.find(".pids>div");

                    var pids = [];
                    if ($pids.length) {
                        $pids.each(function () {
                            if (!$(this).find(".tit.error").length && $(this).find(".tit").text().length)
                                pids.push({
                                    PID: $(this).find("input").val().trim(),
                                    Status: $(this).find("select option:selected").val() == 1
                                })
                        })
                    }
                    if (!pids.length) {
                        swal("错误", "请正确填写PID!", "error");
                        return false;
                    }
                    $.post("/Tire/SaveInstallNow", { cityIds: CityIds.join(';'), pidObj: JSON.stringify(pids) }, function (result) {
                        if (result.IsSuccess) {
                            swal("成功", "保存成功!", "success");
                            window.location.reload();
                        }
                        else
                            swal("失败", result.ReturnMessage, "error");
                    });
                })
            },
            CheckPid: function (target) {
                var $target = $(target);
                var pid = $target.val().trim();
                var titDom = $target.parent().find(".tit");
                if (pid.length) {
                    $.post("/Tire/FetchDisPlayNameByPID", { pid: pid }, function (result) {
                        if (result)
                            titDom.removeClass("error").text(result);
                        else
                            titDom.addClass("error").text("PID不存在");
                    });
                }
                else
                    titDom.addClass("error").text("PID为空");
            },
            Delete: function (pkid) {
                swal({
                    title: "确认删除?",
                    type: "warning",//图标
                    showCancelButton: true,//是否展示取消按钮
                    confirmButtonColor: "#DD6B55",//确认按钮颜色
                    confirmButtonText: "确认!",//确认按钮的文本
                    cancelButtonText: "取消",//取消按钮的文本
                    closeOnConfirm: false,//点击确认按钮后是否关闭窗口
                },
                    function (isConfirm) {
                        if (isConfirm) {
                            //点击确认按钮后执行
                            $.post("/Tire/DeleteInstallNow", { pkid: pkid }, function (result) {
                                if (result > 0) {
                                    swal("成功", "删除成功", "success");
                                    TuhuTire.Logic.InstallNow.LoadList($(".list .pager>a.current").text());
                                }
                                else
                                    swal("失败", "删除失败", "error");
                            });
                        }
                    });
            },
            BitchOn: function () {
                if (!TuhuTire.Logic.ListActivity.HasSelected()) {
                    swal("未选中", "当前无任何选中,无法启用", "error");
                    return false;
                }
                var pkidArr = [];

                $("[name=selectSingle]:checked").each(function () {
                    var pkid = $(this).data("pkid");
                    if (pkid)
                        pkidArr.push(pkid);
                });
                if (!pkidArr.length) {
                    swal("未合法选中", "当前未选中活动,无法启用", "error");
                    return false;
                }
                swal({
                    title: "确认批量启用?",
                    type: "warning",//图标
                    showCancelButton: true,//是否展示取消按钮
                    confirmButtonColor: "#DD6B55",//确认按钮颜色
                    confirmButtonText: "确认!",//确认按钮的文本
                    cancelButtonText: "取消",//取消按钮的文本
                    closeOnConfirm: false,//点击确认按钮后是否关闭窗口
                },
                    function (isConfirm) {
                        if (isConfirm) {
                            //点击确认按钮后执行
                            $.post("/Tire/InstallNowBitchOn", { PKIDS: pkidArr.join(',') }, function (result) {
                                if (result > 0) {
                                    swal("成功", "启用成功", "success");
                                    TuhuTire.Logic.InstallNow.LoadList($(".list .pager>a.current").text());
                                }
                                else if (result == -99)
                                    swal("错误", "不允许同一二级城市同一轮胎规格同时启用多于5个活动", "error");
                                else if (result == -101)
                                    swal("失败", "启用失败", "error");
                                else if (result == -1)
                                    swal("未合法选中", "当前未选中活动,无法禁用", "error");
                                else if (result == -2)
                                    swal("未合法选中", "当前选中皆为禁用状态,无需禁用", "error");
                            });
                        }
                    });
            },
            BitchOff: function () {
                if (!TuhuTire.Logic.ListActivity.HasSelected()) {
                    swal("未选中", "当前无任何选中,无法禁用", "error");
                    return false;
                }
                var pkidArr = [];

                $("[name=selectSingle]:checked").each(function () {
                    var pkid = $(this).data("pkid");
                    if (pkid)
                        pkidArr.push(pkid);
                });
                if (!pkidArr.length) {
                    swal("未合法选中", "当前未选中活动,无法禁用", "error");
                    return false;
                }
                swal({
                    title: "确认批量禁用?",
                    type: "warning",//图标
                    showCancelButton: true,//是否展示取消按钮
                    confirmButtonColor: "#DD6B55",//确认按钮颜色
                    confirmButtonText: "确认!",//确认按钮的文本
                    cancelButtonText: "取消",//取消按钮的文本
                    closeOnConfirm: false,//点击确认按钮后是否关闭窗口
                },
                    function (isConfirm) {
                        if (isConfirm) {
                            //点击确认按钮后执行
                            $.post("/Tire/InstallNowBitchOff", { PKIDS: pkidArr.join(',') }, function (result) {
                                if (result > 0) {
                                    swal("成功", "禁用成功", "success");
                                    TuhuTire.Logic.InstallNow.LoadList($(".list .pager>a.current").text());
                                }
                                else if (result == -1)
                                    swal("未合法选中", "当前未选中活动,无法禁用", "error");
                                else if (result == -99)
                                    swal("未合法选中", "当前选中皆为禁用状态,无需禁用", "error");
                                else
                                    swal("失败", "禁用失败", "error");

                            });
                        }
                    });
            },
        },
        InsuranceYears: {
            LoadList: function (pageIndex) {
                $(".loadimg").show();
                var brands,
                    patterns,
                    tiresizes,
                    rims,
                    rof = $(".rof option:selected").val(),
                    onSale = $(".onsale option:selected").val(),
                    winter = $(".winter option:selected").val(),
                    isConfig = $(".config option:selected").val(),
                    pid = $("[name=PID]").val();
                if (!$("[name=Brand]:first").prop("checked")) {
                    var tempBrands = [];
                    $("[name=Brand]:checked").each(function () {
                        tempBrands.push($(this).val());
                    });
                    if (tempBrands.length)
                        brands = tempBrands.join(';');
                }
                if (!$("[name=Pattern]:first").prop("checked")) {
                    var tempPatterns = [];
                    $("[name=Pattern]:checked").each(function () {
                        tempPatterns.push($(this).val());
                    });
                    if (tempPatterns.length)
                        patterns = tempPatterns.join(';');
                }
                if ($(".tireSizeList span").length) {
                    var tempTireSizes = [];
                    $(".tireSizeList span").each(function () {
                        tempTireSizes.push($(this).text());
                    });
                    if (tempTireSizes.length)
                        tiresizes = tempTireSizes.join(';');
                }

                if (!$("[name=Rim]:first").prop("checked")) {
                    var tempRims = [];
                    $("[name=Rim]:checked").each(function () {
                        tempRims.push($(this).val());
                    });
                    if (tempRims.length)
                        rims = tempRims.join(';');
                }
                $.get("/Tire/TireInsuranceYearsList", {
                    Brands: brands,
                    Patterns: patterns,
                    TireSizes: tiresizes,
                    Rims: rims,
                    Rof: rof,
                    OnSale: onSale,
                    Winter: winter,
                    IsConfig: isConfig.length ? isConfig == 1 : undefined,
                    PID: pid,
                    PageSize: $(".PageSize option:selected").val(),
                    PageIndex: pageIndex,
                }, function (html, status) {
                    if (status === "success") {
                        $(".loadimg").hide();
                        $(".list").html(html);
                        $(".data_count").text('共' + ($(".table").length ? $(".table").attr("data-count") : 0) + '条数据');
                    }
                });
            },
            Edit: function (pid, year) {
                var $dialog = $("#Edit");
                $dialog.find("ul").html('<li class="pid">' + pid + '</li>');
                $dialog.find("[name=type]").val(year);
                
                $("#Edit").dialog({
                    title: "编辑",
                    width: 400,
                    height: 300,
                    modal: true,
                    buttons: {
                        "保存": function () {
                            var dialog = $(this);
                            var pid = dialog.find(".pid").text(),
                                type = dialog.find("[name=type]").val().trim();
                            var pids = [];
                            pids.push(pid);
                            $.post("/Tire/TireInsuranceYearsEdit", { pids: pids, years: type }, function (result) {
                                if (result.code > 0) {
                                    swal("成功", "保存成功！", "success");
                                    dialog.dialog("close");
                                    TuhuTire.Logic.InsuranceYears.LoadList($(".list .pager>a.current").text());
                                }
                                else
                                    swal("失败", "保存失败！", "error");

                            });
                        },
                        "关闭": function () {
                            $(this).dialog("close"); //当点击取消按钮时，关闭窗口
                        }
                    }
                });
            },
            BitchEdit: function () {
                if (!TuhuTire.Logic.ListActivity.HasSelected()) {
                    swal("未选中", "当前无任何选中,无法启用", "error");
                    return false;
                }
                var objArr = [];

                $("[name=selectSingle]:checked").each(function () {
                    var pid = $(this).data("pid");
                    objArr.push(pid);
                });
                if (!objArr.length) {
                    swal("未合法选中", "当前未选中活动,无法启用", "error");
                    return false;
                }
                var $dialog = $("#BitchEdit");
                var html = '';
                $.each(objArr, function (i, v) {
                    html += '<li class="pid">' + v + '</li>'
                });
                $dialog.find("ul").html(html);
                $dialog.find("[name=type]").val('1');
                $("#BitchEdit").dialog({
                    title: "批量编辑",
                    width: 400,
                    height: 500,
                    modal: true,
                    buttons: {
                        "保存": function () {
                            var dialog = $(this);
                            var type = dialog.find("[name=type]").val();
                             

                            $.post("/Tire/TireInsuranceYearsEdit", { pids: objArr, years: type }, function (result) {
                                if (result.code == 1) {
                                    swal("成功", "保存成功！", "success");
                                    dialog.dialog("close");
                                    TuhuTire.Logic.InsuranceYears.LoadList($(".list .pager>a.current").text());
                                }
                                else {
                                    swal("失败", "保存失败！", "error");
                                }

                            });
                        },
                        "关闭": function () {
                            $(this).dialog("close"); //当点击取消按钮时，关闭窗口
                        }
                    }
                });
            },
        },
        InstallFee: {
            LoadList: function (pageIndex) {
                $(".loadimg").show();
                var brands,
                    patterns,
                    tiresizes,
                    rims,
                    rof = $(".rof option:selected").val(),
                    onSale = $(".onsale option:selected").val(),
                    winter = $(".winter option:selected").val(),
                    isConfig = $(".config option:selected").val(),
                    pid = $("[name=PID]").val();
                if (!$("[name=Brand]:first").prop("checked")) {
                    var tempBrands = [];
                    $("[name=Brand]:checked").each(function () {
                        tempBrands.push($(this).val());
                    });
                    if (tempBrands.length)
                        brands = tempBrands.join(';');
                }
                if (!$("[name=Pattern]:first").prop("checked")) {
                    var tempPatterns = [];
                    $("[name=Pattern]:checked").each(function () {
                        tempPatterns.push($(this).val());
                    });
                    if (tempPatterns.length)
                        patterns = tempPatterns.join(';');
                }
                if ($(".tireSizeList span").length) {
                    var tempTireSizes = [];
                    $(".tireSizeList span").each(function () {
                        tempTireSizes.push($(this).text());
                    });
                    if (tempTireSizes.length)
                        tiresizes = tempTireSizes.join(';');
                }

                if (!$("[name=Rim]:first").prop("checked")) {
                    var tempRims = [];
                    $("[name=Rim]:checked").each(function () {
                        tempRims.push($(this).val());
                    });
                    if (tempRims.length)
                        rims = tempRims.join(';');
                }
                $.get("/Tire/InstallFeeList", {
                    Brands: brands,
                    Patterns: patterns,
                    TireSizes: tiresizes,
                    Rims: rims,
                    Rof: rof,
                    OnSale: onSale,
                    Winter: winter,
                    IsConfig: isConfig.length ? isConfig == 1 : undefined,
                    PID: pid,
                    PageSize: $(".PageSize option:selected").val(),
                    PageIndex: pageIndex,
                }, function (html, status) {
                    if (status === "success") {
                        $(".loadimg").hide();
                        $(".list").html(html);
                        $(".data_count").text('共' + ($(".table").length ? $(".table").attr("data-count") : 0) + '条数据');
                    }
                });
            },
            AddTireSize: function () {
                var TireSize = "";
                var Tire_Width = $("#Tire_Width option:selected").val();
                var Tire_AspectRatio = $("#Tire_AspectRatio option:selected").val();
                var Tire_Rim = $("#Tire_Rim option:selected").val();
                if (Tire_Width.length && Tire_AspectRatio.length && Tire_Rim.length) {
                    TireSize = Tire_Width + '/' + Tire_AspectRatio + 'R' + Tire_Rim;
                }
                if (TireSize) {
                    var isExsit = false;
                    $(".tireSizeList li>span").each(function () {
                        if ($(this).text() == TireSize)
                            isExsit = true;
                    })
                    if (isExsit)
                        swal("错误", "该规则已选择!", "error");
                    else
                        $(".tireSizeList").append('<li><span>' + TireSize + '</span><a style="margin-left: 15px;cursor: pointer;" onclick="$(this).parent().remove();">删除</a></li>')
                }
                else
                    swal("错误", "规则选择不正确!", "error");
            },
            Edit: function (pid, oldprice) {
                var $dialog = $("#Edit");
                $dialog.find("ul").html('<li class="pid">' + pid + '</li>');
                $dialog.find("[name=price]").val(oldprice > 0 ? oldprice : '');
                $("#Edit").dialog({
                    title: "编辑",
                    width: 400,
                    height: 300,
                    modal: true,
                    buttons: {
                        "保存": function () {
                            var dialog = $(this);
                            var pid = dialog.find(".pid").text(),
                                price = dialog.find("[name=price]").val().trim();
                            if (price.length && !isNaN(price)) {
                                price = parseFloat(price);
                                if (price < 0) {
                                    swal("错误", "价格不合法！", "error");
                                    return false;
                                }
                            }
                            else {
                                swal("错误", "价格不合法！", "error");
                                return false;
                            }
                            $.post("/Tire/InstallFeeEdit", { AddPrice: price, Pid: pid, OldPrice: oldprice }, function (result) {
                                if (result > 0) {
                                    swal("成功", "保存成功！", "success");
                                    dialog.dialog("close");
                                    TuhuTire.Logic.InstallFee.LoadList($(".list .pager>a.current").text());
                                }
                                else
                                    swal("失败", "保存失败！", "error");

                            });
                        },
                        "关闭": function () {
                            $(this).dialog("close"); //当点击取消按钮时，关闭窗口
                        }
                    }
                });
            },
            BitchEdit: function () {
                if (!TuhuTire.Logic.ListActivity.HasSelected()) {
                    swal("未选中", "当前无任何选中,无法启用", "error");
                    return false;
                }
                var objArr = [];

                $("[name=selectSingle]:checked").each(function () {
                    var pid = $(this).data("pid");
                    var oldprice = $(this).data("price");
                    objArr.push({
                        PID: pid,
                        OldPrice: oldprice
                    });
                });
                if (!objArr.length) {
                    swal("未合法选中", "当前未选中活动,无法启用", "error");
                    return false;
                }
                var $dialog = $("#BitchEdit");
                var html = '';
                $.each(objArr, function (i, v) {
                    html += '<li class="pid">' + v.PID + '</li>'
                });
                $dialog.find("ul").html(html);
                $dialog.find("[name=price]").val('');
                $("#BitchEdit").dialog({
                    title: "批量编辑",
                    width: 400,
                    height: 500,
                    modal: true,
                    buttons: {
                        "保存": function () {
                            var dialog = $(this);
                            var price = dialog.find("[name=price]").val().trim();
                            if (price.length && !isNaN(price)) {
                                price = parseFloat(price);
                                if (price < 0) {
                                    swal("错误", "价格不合法！", "error");
                                    return false;
                                }
                            }
                            else {
                                swal("错误", "价格不合法！", "error");
                                return false;
                            }
                            $.post("/Tire/InstallFeeBitchEdit", { AddPrice: price, json: JSON.stringify(objArr) }, function (result) {
                                if (result.code == 1) {
                                    swal("成功", "保存成功！", "success");
                                    dialog.dialog("close");
                                    TuhuTire.Logic.InstallFee.LoadList($(".list .pager>a.current").text());
                                }
                                else {
                                    alert("失败PID：" + result.list.join(';'));
                                    dialog.dialog("close");
                                }

                            });
                        },
                        "关闭": function () {
                            $(this).dialog("close"); //当点击取消按钮时，关闭窗口
                        }
                    }
                });
            },
        },
        InstallmentConfig: {
            LoadList: function (pageindex, categoryName) {
                $(".loadimg").show();
                var brands,
                    patterns,
                    tiresizes,
                    rims,
                    rof = $(".rof option:selected").val(),
                    onSale = $(".onsale option:selected").val(),
                    winter = $(".winter option:selected").val(),
                    isConfig = $(".installment option:selected").val(),
                    pid = $("[name=PID]").val();
                if (!$("[name=Brand]:first").prop("checked")) {
                    var tempBrands = [];
                    $("[name=Brand]:checked").each(function () {
                        tempBrands.push($(this).val());
                    });
                    if (tempBrands.length)
                        brands = tempBrands.join(';');
                }
                if (!$("[name=Pattern]:first").prop("checked")) {
                    var tempPatterns = [];
                    $("[name=Pattern]:checked").each(function () {
                        tempPatterns.push($(this).val());
                    });
                    if (tempPatterns.length)
                        patterns = tempPatterns.join(';');
                }
                if ($(".tireSizeList span").length) {
                    var tempTireSizes = [];
                    $(".tireSizeList span").each(function () {
                        tempTireSizes.push($(this).text());
                    });
                    if (tempTireSizes.length)
                        tiresizes = tempTireSizes.join(';');
                }

                if (!$("[name=Rim]:first").prop("checked")) {
                    var tempRims = [];
                    $("[name=Rim]:checked").each(function () {
                        tempRims.push($(this).val());
                    });
                    if (tempRims.length)
                        rims = tempRims.join(';');
                }
                var Tire_Width = $("#Tire_Width").val();
                var Tire_AspectRatio = $("#Tire_AspectRatio").val();
                var Tire_Rim = $("#Tire_Rim").val();
                var TireSize = "";
                if (Tire_Width && Tire_AspectRatio && Tire_Rim) {
                    TireSize = Tire_Width + '/' + Tire_AspectRatio + 'R' + Tire_Rim;
                }
                var ThreeConfig = $(".threeconfig").val();
                var SixConfig = $(".sixconfig").val();
                var TweleveConfig = $(".twelveconfig").val();
                var pid = $("#PID").val();
                var productname = $("#ProductName").val();
                $.get("/InstalmentConfig/InstallmentList",
                    {
                        Brands: brands,
                        Patterns: patterns,
                        TireSize: TireSize,
                        HubRim: Tire_Rim,
                        IsOnSale: onSale,
                        IsInstallmentOpen: isConfig,
                        ThreePeriods: ThreeConfig,
                        SixPeriods: SixConfig,
                        TwelvePeriods: TweleveConfig,
                        PID: pid,
                        CategoryName: categoryName,
                        ProductName: productname,
                        PageSize: $(".PageSize option:selected").val(),
                        PageIndex: pageindex
                    }, function (html, status) {
                        if (status === "success") {

                            $(".loadimg").hide();
                            $(".list").html(html);
                            $(".data_count").text('共' +
                                ($(".table").length ? $(".table").attr("data-count") : 0) +
                                '条数据');
                        }
                        //function (html, status) {
                        //    if (status === "success") {
                        //        $(".loadimg").hide();
                        //        $(".list").html(html);
                        //        $(".data_count").text('共' + ($(".table").length ? $(".table").attr("data-count") : 0) + '条数据');
                        //    }
                        //});
                    });

            },
            BitchEdit: function (categoryName) {
                if (!$("[name=selectSingle]:checked").length) {
                    swal("未选中", "当前无任何选中,无法启用", "error");
                    return false;
                }
                var objArr = [];
                $("[name=selectSingle]:checked").each(function () {
                    var pid = $(this).val();
                    objArr.push(pid);
                });
                if (!objArr.length) {
                    swal("未合法选中", "当前未选中活动,无法启用", "error");
                    return false;
                }
                var html = '';
                $.each(objArr, function (i, v) {
                    html += '<li class="pid">' + v + '</li>'
                });
                var $dialog = $("#editdiv");
                $dialog.find("ul").html(html);
                $("#EditIsInstallmentOpen").val("0");
                $("#EditPeriods").hide();
                $dialog.dialog({
                    title: "编辑",
                    width: 500,
                    height: 400,
                    modal: true,
                    buttons: {
                        "保存": function (evt) {
                            var buttonDomElement = evt.target;
                            var isopen = $("#EditIsInstallmentOpen").val() == "1";
                            var model = {
                                IsInstallmentOpen: isopen,
                                PID: $("#EditPid").text(),
                                ThreePeriods: isopen ? $("#EditThreePeriods").val() : "",
                                SixPeriods: isopen ? $("#EditSixPeriods").val() : "",
                                TwelvePeriods: isopen ? $("#EditTwelvePeriods").val() : "",
                            }
                            $(buttonDomElement).attr('disabled', true);

                            $.post("/InstalmentConfig/BatchSaveTireInstallmentConfigAsync", { model: model, pids: objArr }, function (result) {
                                console.log(result);
                                alert(result.msg);
                                if (result.code == 1) {
                                    $dialog.dialog("close");
                                    $(buttonDomElement).attr('disabled', false);

                                    TuhuTire.Logic.InstallmentConfig.LoadList(1, categoryName);
                                }
                            });
                        },
                        "关闭": function () {
                            $(this).dialog("close"); //当点击取消按钮时，关闭窗口
                        }
                    }
                });
            },
            EditProduct: function (s, categoryName) {
                var product = JSON.parse(s);
                $("#EditPeriods").hide();
                console.log(product);
                $("#EditPid").text(product.PID);
                $("#EditProductName").text(product.ProductName);
                $("#EditIsInstallmentOpen").val(product.IsInstallmentOpen ? "1" : "0");
                if (product.IsInstallmentOpen == "1") {
                    $("#EditPeriods").show();
                } else {
                    $("#EditPeriods").hide();
                }
                if (typeof (product.ThreePeriods) !== 'undefined') {
                    $("#EditThreePeriods").val(product.ThreePeriods)
                }
                if (typeof (product.SixPeriods) !== 'undefined') {
                    $("#EditSixPeriods").val(product.SixPeriods)
                }
                if (typeof (product.TwelvePeriods) != 'undefined') {
                    $("#EditTwelvePeriods").val(product.TwelvePeriods)
                }
                var $dialog = $("#editdiv");
                $dialog.find("ul").html("");
                var html = '';
                html += '<li class="pid">' + product.PID + '</li>';
                var $dialog = $("#editdiv");
                $dialog.find("ul").html(html);

                $("#editdiv").dialog({
                    title: "编辑",
                    width: 500,
                    height: 400,
                    modal: true,
                    buttons: {
                        "保存": function () {
                            var isopen = $("#EditIsInstallmentOpen").val() == "1";
                            var model = {
                                IsInstallmentOpen: isopen,
                                PID: $("#EditPid").text(),
                                ThreePeriods: isopen ? $("#EditThreePeriods").val() : "",
                                SixPeriods: isopen ? $("#EditSixPeriods").val() : "",
                                TwelvePeriods: isopen ? $("#EditTwelvePeriods").val() : "",
                            }
                            $.post("/InstalmentConfig/SaveTireInstallmentConfigAsync", { model }, function (result) {
                                console.log(result);
                                alert(result.msg);
                                if (result.code == 1) {
                                    $dialog.dialog("close");
                                    TuhuTire.Logic.InstallmentConfig.LoadList(1, categoryName);
                                }
                            });
                        },
                        "关闭": function () {
                            $(this).dialog("close"); //当点击取消按钮时，关闭窗口
                        }
                    }
                });
            },
        },
        TireStockoutWhite: {
            LoadList: function (pageIndex) {
                $(".loadimg").show();
                var pid = $("[name=PID]").val();
                var cityId = $("#CityId>option:selected").data("pkid");
                var TireSize = '';
                //规格尺寸的补充
                var Tire_Width = $("#Tire_Width option:selected").val();
                var Tire_AspectRatio = $("#Tire_AspectRatio option:selected").val();
                var Tire_Rim = $("#Tire_Rim option:selected").val();
                if (Tire_Width.length && Tire_AspectRatio.length && Tire_Rim.length) {
                    TireSize = Tire_Width + '/' + Tire_AspectRatio + 'R' + Tire_Rim;
                }

                var regionalStockout = $("#RegionalStockout option:selected").val();
                var OnSale = $("#OnSale option:selected").val();
                var Stuckout = $("#Stuckout option:selected").val();
                var SystemStuckout = $("#SystemStuckout option:selected").val();
                var status = $("#status option:selected").val();
                var productname = $("[name=productname]").val();
                var isShow = $("#isShow option:selected").val();
                var address = $("#province option:selected").val() + ';' + $("#CityId option:selected").text();
                $.post("/Tire/TireStockoutWhiteList", {
                    Status: status,
                    PID: pid,
                    CityId: cityId,
                    OnSale: OnSale,
                    SystemStuckout: SystemStuckout,
                    Stuckout: Stuckout,
                    TireSize: TireSize,
                    DisplayName: productname,
                    RegionalStockout: regionalStockout,
                    IsShow: isShow,
                    Address: address,
                    PageSize: $(".PageSize option:selected").val(),
                    PageIndex: pageIndex,
                }, function (html, status) {
                    if (status === "success") {
                        $(".loadimg").hide();
                        $(".list").html(html);
                        $(".data_count").text('共' + ($(".table").length ? $(".table").attr("data-count") : 0) + '条数据');
                    }
                });
            },
            BitAdd: function () {
                var dialog = TuhuTire.Dialog.Popup("#add", {
                    clone: true, draggable: false, closeHandle: ".close,.closeBtn", disableScroll: false, opacity: 0.3
                });
                dialog.find(".save").on("click", function () {
                    var $pids = dialog.find(".pids>div");

                    var flag = true;
                    var pids = [];
                    if ($pids.length) {
                        $pids.each(function () {
                            if ($(this).find(".tit.error").length)
                                flag = false;
                            if (!$(this).find(".tit.error").length && $(this).find(".tit").text().length)
                                pids.push($(this).find("input").val().trim())
                        })
                    }
                    if (!pids.length || !flag) {
                        swal("错误", "请正确填写PID!", "error");
                        return false;
                    }
                    $.post("/Tire/SaveTireStockWhite", { pids: pids.join(';') }, function (result) {
                        if (result > 0) {
                            swal("成功", "保存成功!", "success");
                            window.location.reload();
                        }
                        else
                            swal("失败", result.ReturnMessage, "error");
                    });
                })
            },
            Delete: function (pid) {
                swal({
                    title: "确认删除?",
                    type: "warning",//图标
                    showCancelButton: true,//是否展示取消按钮
                    confirmButtonColor: "#DD6B55",//确认按钮颜色
                    confirmButtonText: "确认!",//确认按钮的文本
                    cancelButtonText: "取消",//取消按钮的文本
                    closeOnConfirm: false,//点击确认按钮后是否关闭窗口
                },
                    function (isConfirm) {
                        if (isConfirm) {
                            $.post("/Tire/RemoveWhite", { pid: pid }, function (result) {
                                if (result > 0) {
                                    swal("成功", "保存成功!", "success");
                                    TuhuTire.Logic.TireStockoutWhite.LoadList($(".list .pager>a.current").text());
                                }
                                else
                                    swal("失败", result.ReturnMessage, "error");
                            });
                        }
                    });
            },
        },
    };

    return TuhuTire;
})(window, document, jQuery);