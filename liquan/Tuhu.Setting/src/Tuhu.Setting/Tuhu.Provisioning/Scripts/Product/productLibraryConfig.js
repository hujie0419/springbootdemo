var ImageDomain = "https://image.tuhu.cn";
var NeedRemoveCoupon = new Array();
var NeedAddCoupon = new Array();
var PassedProducts = new Array();
var NotPassedProducts = new Array();
var lock = false;
var CookieTools = {
    GetCookie: function (name) {
        var arr = document.cookie.match(new RegExp("(^| )" + name + "=([^;]*)(;|$)"));
        if (arr != null) return unescape(arr[2]); return null;
    },
    SetCookie: function (name, value) {
        var Days = 3600;
        var exp = new Date();
        exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);
        document.cookie = name + "=" + escape(value) + ";path=/;expires=" + exp.toGMTString();
    },
    DelCookie: function (name) {
        var exp = new Date();
        exp.setTime(exp.getTime() - 1);
        var cval = this.GetCookie(name);
        if (cval != null)
            document.cookie = name + "=" + cval + ";expires=" + exp.toGMTString() + "; path=/";
    }
}

//文件类
var FileClass = {
    GetNameFromPath: function (strFilepath) {
        var objRE = new RegExp(/([^\/\\]+)$/);
        var strName = objRE.exec(strFilepath);
        if (strName == null) {
            return null;
        }
        else {
            return strName[0];
        }
    },
    GetFileSize: function (fileId) {
        var fileSize = 0;
        fileSize = $("#" + fileId)[0].files[0].size;
        fileSize = fileSize / 1048576;
        return fileSize;
    },
    //加载中
    Uploading: function () {
        var _PageHeight = $(window).height();
        var _PageWidth = $(window).width();
        var _LoadingTop = _PageHeight > 61 ? (_PageHeight - 61) / 2 : 0;
        var _LoadingLeft = _PageWidth > 215 ? (_PageWidth - 215) / 2 : 0;
        var _LoadingHtml = '<div id="product-loadingDiv" style="margin-top:-20px;border:0;position:absolute;left:0;width:100%;height:100%;top:0;background:#f3f8ff;opacity:0.8;filter:alpha(opacity=80);z-index:10000;"><div style="border:0;position: absolute; cursor1: wait; left: ' + _LoadingLeft + 'px; top:' + _LoadingTop + 'px; width: auto; height: 57px; line-height: 57px; padding-left: 50px; padding-right: 5px; background: #f3f8ff url(\'/Scripts/Product/loading.gif\') no-repeat scroll 5px 10px; color: #696969; font-family:\'Microsoft YaHei\';">执行中，请等待...</div></div>';//border: 2px solid #95B8E7;
        $('#DivLoadingData').html(_LoadingHtml).show();
    },
    //加载完成
    CompleteUpLoading: function () {
        $('#DivLoadingData').html('').hide();
    }
};

//树形菜单加载
var zTreeSetting = {
    setting: {
        data: {
            simpleData: { enable: true }
        },
        callback: {
            onClick: function () {
                var d = zTreeSetting.getItems();
                $("#PublicCategory").val(d.NodeNo);
                ProductLibraryConfig.DefaultLoad();
            }
        }
    },
    zTreeJson: '',
    zTreeInit: function () {
        $.fn.zTree.init($("#CategoryTagTree"), zTreeSetting.setting, zTreeSetting.zTreeJson);
    },
    getItems: function () {
        return $.fn.zTree.getZTreeObj("CategoryTagTree").getSelectedNodes()[0];
    },
    expandAll: function (isExpandAll) {
        $.fn.zTree.getZTreeObj("CategoryTagTree").expandAll(isExpandAll || false);
    }
};

var isBatch = 0;
var ProductLibraryConfig = {
    DefaultLoad: function () {
        ProductLibraryConfig.ResetSearch();
        var category = $.trim($("#PublicCategory").val());
        ProductLibraryConfig.LoadCondition(category);
        ProductLibraryConfig.LoadProductCouponList(1);
    },
    //分页加载条件
    PageEvent: function (el, pageIndex) {
        $(el).css("background", "rgb(0, 136, 204)").siblings().css("background", "#ccc");

        
        var _SearchType = CookieTools.GetCookie("SearchType") || 0;
        ProductLibraryConfig.LoadProductCouponList(pageIndex);
        CookieTools.SetCookie("data-page", pageIndex);
    },
    //加载条件列表
    LoadCondition: function (category) {
        if (!category) {
            category = '1';
        }
        $.ajax({
            url: '/ProductLibraryConfig/CatetoryCondition?category=' + category,
           // async: true,
            type: 'get',
            beforeSend: function () {
                FileClass.Uploading();
            },
            complete: function () {
               // FileClass.CompleteUpLoading();
            },
            success: function (data) {
                $('#div-condition').html(data);
                $('#tb-condition2').width($('#div-condition').width());
            },
            error: function () {
            }
        });
    },
    //获取多项checkbox选择内容
    GetCheckBoxValue: function ($id) {
        var valueLst = '';
        $id.each(function () {
            valueLst += $(this).val() + ','
        });
        if (valueLst.length > 0) {
            valueLst = valueLst.substring(0, valueLst.length - 1);
        }
        return valueLst;
    },    
    //获取查询条件
    GetProductCouponParm: function (pageIndex) {
        if (!pageIndex) {
            pageIndex = 1;
        }
        var category = $.trim($("#PublicCategory").val());
        if (!category) {
            category = 1;
        }
        var parm = {};
        parm.PageIndex = pageIndex;
        parm.PageSize = $('#pageSize').val();
        parm.Category = category;
        parm.Brand = ProductLibraryConfig.GetCheckBoxValue($('#tbody-condition input[type="checkbox"][name="CP_Brand"]:checked'));
        parm.Tab = ProductLibraryConfig.GetCheckBoxValue($('#tbody-condition input[type="checkbox"][name="CP_Tab"]:checked'));
        parm.Rim = ProductLibraryConfig.GetCheckBoxValue($('#tbody-condition input[type="checkbox"][name="CP_Tire_Rim"]:checked'));
        parm.IsShow = $('#tb-condition2 select[name="S_IsShow"]').val();
        parm.OnSale = $('#tb-condition2 select[name="onsale"]').val();
        parm.FiltrateType = $('#tb-condition2 select[name="filtrateType"]').val();
        parm.Maoli = $('#tb-condition2 select[name="maoli"]').val();
        parm.BeginPrice = $('#tb-condition2 input[name="S_Price_Begin"]').val();
        parm.EndPrice = $('#tb-condition2 input[name="S_Price_End"]').val();
        parm.CouponIds = $('#tb-condition2 input[name="S_Coupon"]').val();
        parm.Pattern = $('#tb-condition2 select[name="S_Figure"]').val(); //花纹
        parm.MaoliSort = $('#tb-condition2 select[name="maoliSort"]').val();

        if ($.trim($('#imput_pid_Ids').val()) == '') {
            parm.PId = $('#tb-condition2 input[name="S_PID"]').val();
        } else {
            parm.PId = $.trim($('#imput_pid_Ids').val());
        }
        return parm;
    },
    //加载产品优惠券配置列表
    LoadProductCouponList: function (pageIndex) {
        var search = ProductLibraryConfig.GetProductCouponParm(pageIndex);
        $.ajax({
            url: '/ProductLibraryConfig/ProductCouponList',
            data: search,
            // async: true,
            type: 'post',
            beforeSend: function () {
                FileClass.Uploading();
            },
            complete: function () {
                FileClass.CompleteUpLoading();
            },
            success: function (data) {
                $('#div-product-list').html(data);
                $('#ProductList').width($('#div-condition').width());
                var errorPID = $('#hid_product_error_PID').val();
                if (errorPID && errorPID.length > 0) {
                    $('#tb_error_imputInfo td.td-product-pid').html(errorPID);
                    $('#tb_error_imputInfo').show();
                }
            },
            error: function () {
            }
        });
    },
    //重置搜索内容
    ResetSearch: function () {
        $("#tbody-condition input[data-brand]").attr("checked", false);
        $("#tbody-condition input[data-tab]").attr("checked", false);
        $("#tbody-condition input[data-rim]").attr("checked", false);
        $('#tb-condition2 select[name="S_IsShow"]').val('-1');
        $('#tb-condition2 select[name="onsale"]').val('');
        $('#tb-condition2 select[name="filtrateType"]').val('GrossMarginAfter');
        $('#tb-condition2 select[name="maoli"]').val('');
        $('#tb-condition2 input[name="S_Price_Begin"]').val('');
        $('#tb-condition2 input[name="S_Price_End"]').val('');
        $('#tb-condition2 input[name="S_PID"]').val('');
        $('#tb-condition2 input[name="S_Coupon"]').val('');
        $('#tb-condition2 select[name="S_Figure"]').val('');
        $('#tb-condition2 select[name="pageSize"]').val('0');
        ProductLibraryConfig.ClearImputExcel();
    },
    Search: function () {
        ProductLibraryConfig.LoadProductCouponList(1);
    },
    ClearImputExcel: function () {
        $('#imput_pid_Ids').val('');
        $('#tb_error_imputInfo td.td-product-pid').html('');
        $('#tb_error_imputInfo').hide();
        $('#btn_clear_imput_Excel').hide();
        $('#btn_imput_Excel').show();
        $('#tb-condition2 select[name="pageSize"]').val('0');
        ProductLibraryConfig.Search();
    },
    //导入产品Id集合
    ImputPidsSearch: function (obj) {
        var file = FileClass.GetNameFromPath($(obj).val());
        if (!obj || $(obj).length <= 0) {
            alert('请选择上传文件');
            return false;
        }
        var extension = file.substr((file.lastIndexOf('.') + 1));
        var fileTypeBool = false;
        switch (extension) {
            case 'xls':
            case 'xlsx':
                fileTypeBool = true;
                break;
        }
        if (!fileTypeBool) {
            alert('只能上传Excel文件！');
            return false;
        }
        var size = FileClass.GetFileSize('fileInputPIds');
        if (size > 5) {
            alert('上传文件已经超过5兆！');
            return false;
        }
        $('#btn_excel_submit').click();
    },
    CheckALLOrOut: function (obj) {
        var IsCheck = $(obj).is(":checked");
        $("input[name='BodyCheckBox']").attr("checked", IsCheck);
    },
    GetCheckList: function () {
        var $BodyCheckBox = $("input[name='BodyCheckBox']"), checkArr = [];
        $BodyCheckBox.each(function (i) {
            if ($(this).is(":checked")) {
                checkArr.push($(this).val());
            }
        });

        return checkArr;
    },
    BatchAddCouponOpen: function () {
        isBatch = 0;
        var _listCount = this.GetCheckList().length;
        if (_listCount > 0) {
            var dialogConfig = { title: "批量添加优惠券", width: 700, height: 400, modal: true };
            $("#BatchAddCouponView").html("")
                .load('/ProductLibraryConfig/BatchAddCoupon')
                .dialog(dialogConfig);
        }
        else {
            alert("您未勾选任何项！");
        }
    },
    BatchDeleteCouponOpen: function () {
        isBatch = 0;
        var _listCount = this.GetCheckList().length;
        if (_listCount > 0) {
            var dialogConfig = { title: "批量删除优惠券", width: 700, height: 400, modal: true };
            $("#BatchAddCouponView").html("")
                .load('/ProductLibraryConfig/BatchDeleteCoupon')
                .dialog(dialogConfig);
        }
        else {
            alert("您未勾选任何项！");
        }
    },
    ShowCouponUseMoneyConfigDialog: function () {
        var _listCount = this.GetCheckList().length;
        if (_listCount > 0) {
            var dialogConfig = { title: "验证优惠券使用规则", width: 900, height: 600, modal: true };
            $("#CouponUseMoneyConfig").dialog(dialogConfig);
            $("#CouponUseMoneyConfig #CouponUseMoneyConfigTable").find("tbody:not(:first)").empty();
            $("#CouponUseMoneyConfig #profitLevel").val("");
            $("#NotAvaiableProduct").hide();
            $("#NotAvaiableProductTips").hide();
        } else {
            alert("您未勾选任何项！");
        }
    },
    VerfyProductCoupon: function (callback) {

        var oids = this.ArrayJoin(this.GetCheckList(), ",");
        var profitLevel = $("#CouponUseMoneyConfigTable #profitLevel").val();
        var $ruleRows = $("#CouponUseMoneyConfigTable").find("tr");
        var rules = new Array();
        for (var i = 1; i < $ruleRows.length; i++) {
            var miniMoney = $ruleRows.eq(i).find(".miniMoney").val();
            var discount = $ruleRows.eq(i).find(".discount").val();
            rules.push({
                Minmoney: miniMoney,
                Discount: discount
            });
        }
        if (oids.length > 0 && rules.length > 0) {
            //$("#CouponUseMoneyConfig .nextStep").attr("disabled", true);
            //$("#CouponUseMoneyConfig .nextStep").css("background", "#999999");

            $(".loadingImg").show();
            $("#NotAvaiableProduct").hide();
            $("#NotAvaiableProductTips").hide();
            $.post("VerfyProductCoupon", { oids: oids, rules: JSON.stringify(rules), grossProfit: profitLevel },
                function (result) {
                    $(".loadingImg").hide();
                    var unavaiableProducts = result.unavaiableProducts;
                    PassedProducts = result.avaiableProducts;
                    NotPassedProducts = unavaiableProducts;
                    if (unavaiableProducts && unavaiableProducts.length > 0) {
                        var simpleProductRow = $("#SimpleProductTemplate").html();
                        $("#NotAvaiableProduct").find("tr:not(:first)").remove();
                        for (var i = 0; i < unavaiableProducts.length; i++) {
                            var rowHtml = simpleProductRow.replace("$index$", i + 1).replace("$productId$", unavaiableProducts[i].PID)
                                .replace("$productName$", unavaiableProducts[i].DisplayName).replace("$price$", unavaiableProducts[i].cy_list_price).replace("$cost$", unavaiableProducts[i].cy_cost);
                            $("#NotAvaiableProduct").append(rowHtml);
                        }
                        $("#NotAvaiableProductTips").show();
                        $("#NotAvaiableProduct").show();
                    } else {
                        //$("#CouponUseMoneyConfig .nextStep").css("background", "#0088cc");
                        //$("#CouponUseMoneyConfig .nextStep").attr("disabled", false);
                        $("#NotAvaiableProduct").hide();
                        $("#NotAvaiableProductTips").hide();
                    }
                    return callback;
                });
        }
    },
    ProfitChange: function () {
        this.ClearTableContent();
        this.VerfyProductCoupon();
    },
    CouponRuleRowChange: function (current) {
        $(current).parent().parent().parent().nextAll().remove();
        var oids = this.ArrayJoin(this.GetCheckList(), ",");
        var miniMoney = $(current).val();
        if (miniMoney) {
            $.post("VerifyPrice", { oids: oids, price: miniMoney }, function (result) {
                if (!result) {
                    alert("没有满足条件的产品");
                    $(current).val("0");
                }
                ProductLibraryConfig.VerfyProductCoupon();
            });
        }
    },
    ShowAddParentCouponDialog: function () {
        var dialogConfig = { title: "创建母券规则", width: 800, height: 400, modal: true };
        $("#CreateParentCoupon").dialog(dialogConfig);
    },
    ShowNotPassedCoupon: function (model) {

        var dialogConfig = { title: "低于最低毛利的优惠券", width: 800, height: 400, modal: true };
        $("#notPassedCoupon").html("").dialog();
    },
    ShowPassedProducts: function (current) {
        var $current = $(current);
        var miniMoney = $current.parent().parent().parent().find(".miniMoney").val();
        var discount = $current.parent().parent().parent().find(".discount").val();
        var currentPassedProducts = PassedProducts[miniMoney + "/" + discount];
        if (currentPassedProducts && currentPassedProducts.length > 0) {
            var simpleProductRow = $("#SimpleProductTemplate").html();
            $("#AvaiableProductsTable").find("tr:not(:first)").remove();
            for (var i = 0; i < currentPassedProducts.length; i++) {
                var rowHtml = simpleProductRow.replace("$index$", i + 1).replace("$productId$", currentPassedProducts[i].PID)
                    .replace("$productName$", currentPassedProducts[i].DisplayName).replace("$price$", currentPassedProducts[i].cy_list_price)
                    .replace("$cost$", currentPassedProducts[i].cy_cost);
                $("#AvaiableProductsTable").append(rowHtml);
            }
            var dialogConfig = { title: "高于最低毛利的产品", width: 700, height: 400, modal: true };
            $("#AvaiableProducts").dialog(dialogConfig);
        } else {
            alert("没有可用产品");
        }
    },
    StepChange: function (current) {
        var $this = $(current);
        var id = $this.parent().parent().parent().parent().parent().attr("id");
        switch (id) {
            case "CouponUseMoneyConfig":
                if (NotPassedProducts && NotPassedProducts.length > 0) {
                    var msg = "有部分商品未被配券，是否确认？";
                    if (confirm(msg) == true) {
                    } else {
                        return false;
                    }
                }
                var moneyRule = $("#CouponUseMoneyConfigTable").find("tr:not(:first)");
                if (moneyRule && moneyRule.length > 0 && $("#CouponUseMoneyConfigTable .miniMoney").val() >= 0
                    && $("#CouponUseMoneyConfigTable .discount").val() >= 0 && $("#CouponUseMoneyConfigTable .newRule").text() != "") {
                    var partVal = $("#partList").val();
                    if (!partVal || partVal <= 0) {
                        $.get("GetDepartmentUseSetting", function (setting) {
                            var partList = setting.filter(function (t) { return t.Type == 0 });
                            var useList = setting.filter(function (t) { return t.Type == 1 });
                            var partOptionHtml = "<option value='-1'>请选择部门</option>";
                            for (var i = 0; i < partList.length; i++) {
                                partOptionHtml += "<option value=" + partList[i].SettingId + " >" + partList[i].DisplayName + "</option>";
                            }
                            $("#publicChildCouponInfo #partList").empty().append(partOptionHtml);
                            var userOptionHtml = "<option>请选择用途</option>";
                            $("#publicChildCouponInfo #useList").empty().append(userOptionHtml);
                        });
                    }
                    var individualHtml = "";
                    for (var i = 0; i < moneyRule.length; i++) {
                        if ($(moneyRule[i]).find(".newRule").text() != "") {
                            var miniMoney = $(moneyRule[i]).find(".miniMoney").val();
                            var discount = $(moneyRule[i]).find(".discount").val();
                            if (miniMoney && miniMoney > 0 && discount && discount > 0)
                                individualHtml += $("#IndividualInfoTemplate").html().replace("$miniMoney$", miniMoney).replace("$discount$", discount);
                        }
                    }
                    $("#IndividualInfo").find("tr:not(:first)").remove();
                    $("#IndividualInfo").append(individualHtml);
                    $("#CouponUseMoneyConfig").dialog('close');
                    var dialogConfig = { title: "插优惠券", width: 900, height: 800, modal: true };
                    $("#CreateCoupon").dialog(dialogConfig);
                    $("#publicParentCouponInfo .name").val("");
                    //$("#publicParentCouponInfo #AppJumpPage").val("");
                    //$("#publicParentCouponInfo #H5JumpPage").val("");
                    $("#publicChildCouponInfo .dayAfterReceive").val("");
                    $("#publicChildCouponInfo .finalValidity").val("");
                    $("#publicChildCouponInfo .startDate").val("");
                    $("#publicChildCouponInfo .endDate").val("");
                    $("#publicChildCouponInfo .limitCount").val("");
                    $("#publicChildCouponInfo .Circulation").val("");
                    $("#publicChildCouponInfo .ShowStartTime").val("");
                    $("#publicChildCouponInfo .ShowEndTime").val("");
                    $("#publicChildCouponInfo .couponName").val("");
                    $("#publicChildCouponInfo .description").val("");
                } else {
                    var grossProfit = $("#CouponUseMoneyConfigTable #profitLevel").val();
                    var hasExistRulePKIDs = ProductLibraryConfig.GetHasExistsRulePkid();
                    if (hasExistRulePKIDs && hasExistRulePKIDs.length > 0) {
                        var rulePKIDs = ProductLibraryConfig.ArrayJoin(hasExistRulePKIDs, ",");
                        ProductLibraryConfig.SaveProductCouponRules(rulePKIDs, grossProfit, function (result) {
                            if (result.result && result.result.State == 1) {
                                alert("插券成功");
                                ProductLibraryConfig.Search();
                                ProductLibraryConfig.ClearTableContent();
                                $("#CouponUseMoneyConfig").dialog('close');
                            } else {
                                var msg = "插券失败";
                                if (result.result && result.result.Message) {
                                    msg = result.result.Message;
                                }
                                alert(msg);
                            }
                        });
                    } else {
                        alert("输入有效数据");
                    }
                }
                break;
            case "CreateCoupon":
                var stepType = $this.attr("class");
                if (stepType == "lastStep") {
                    $("#CreateCoupon").dialog('close');
                    var dialogConfig = { title: "验证优惠券使用规则", width: 900, height: 600, modal: true };
                    $("#CouponUseMoneyConfig").dialog(dialogConfig);
                } else {
                    this.SaveCoupon();
                }
                break;
        }
    },
    SaveCoupon: function () {
        var parentCouponName = $("#publicParentCouponInfo .name").val();
        var orderType = $("#publicParentCouponInfo [name=OrderType]:checked").val();
        var promotionType = $("#publicParentCouponInfo [name=CouponType]:checked").val();
        var jumpType = $("#publicParentCouponInfo [name=JumpType]:checked").val();
        //var appJumpPage = $("#publicParentCouponInfo #AppJumpPage").val();
        //var h5jumpPage = $("#publicParentCouponInfo #H5JumpPage").val();
        var pidType = true;//true表示增加PID,false表示排除PID
        if (parentCouponName == "" || orderType == "" || promotionType == "" || jumpType == "" || pidType == "") {
            alert("母券信息不完整！");
            return;
        }
        var parentCoupon = {
            Name: parentCouponName,
            InstallType: orderType,
            HrefType: jumpType,
            PromotionType: promotionType,
            CustomSkipPage: "",
            WxSkipPage: "",
            H5SkipPage: "",
            PIDType: pidType,
            CouponType: 0
        };
        var dayAfterReceive = $("#publicChildCouponInfo .dayAfterReceive").val();
        var finalValidity = $("#publicChildCouponInfo .finalValidity").val();
        var startDate = $("#publicChildCouponInfo .startDate").val();
        var endDate = $("#publicChildCouponInfo .endDate").val();
        var limitCount = $("#publicChildCouponInfo .limitCount").val();
        var Circulation = $("#publicChildCouponInfo .Circulation").val();
        var channel = $("#publicChildCouponInfo #channel").val();
        var isSalerCanSend = $("#publicChildCouponInfo [name=isSalerCanSend]:checked").val();
        var supportUserType = $("#publicChildCouponInfo [name=supportUserType]:checked").val();
        var ShowStartTime = $("#publicChildCouponInfo .ShowStartTime").val();
        var ShowEndTime = $("#publicChildCouponInfo .ShowEndTime").val();
        var departmentId = $("#publicChildCouponInfo #partList").val();
        var departmentName = $("#publicChildCouponInfo #partList option:selected").text();;
        var intentionId = $("#publicChildCouponInfo #useList").val();
        var intentionName = $("#publicChildCouponInfo #useList option:selected").text();
        if ((!ProductLibraryConfig.VerifyParameter(dayAfterReceive) && (!ProductLibraryConfig.VerifyParameter(startDate) || !ProductLibraryConfig.VerifyParameter(endDate))) ||
            !ProductLibraryConfig.VerifyParameter(limitCount) || !ProductLibraryConfig.VerifyParameter(channel) ||
            !ProductLibraryConfig.VerifyParameter(isSalerCanSend) || !ProductLibraryConfig.VerifyParameter(supportUserType) || !ProductLibraryConfig.VerifyParameter(ShowStartTime)
            || !ProductLibraryConfig.VerifyParameter(ShowEndTime) || !ProductLibraryConfig.VerifyParameter(departmentId) || !ProductLibraryConfig.VerifyParameter(intentionId)
            || !ProductLibraryConfig.VerifyParameter(departmentName) || !ProductLibraryConfig.VerifyParameter(intentionName)) {
            alert("子券信息不完整");
            return;
        }

        var childCoupon = {
            SingleQuantity: limitCount,
            Quantity: Circulation,
            Channel: channel,
            AllowChanel: isSalerCanSend,
            SupportUserRange: supportUserType,
            DetailShowStartDate: ShowStartTime,
            DetailShowEndDate: ShowEndTime,
            DepartmentId: departmentId,
            DepartmentName: departmentName,
            IntentionId: intentionId,
            IntentionName: intentionName,
            CouponType: 0,
            DeadLineDate: finalValidity
        };
        if ($(".fixedDateType").attr("checked") && $(".fixedDateType").attr("checked") == "checked") {
            childCoupon.ValiStartDate = startDate;
            childCoupon.ValiEndDate = endDate;
        } else if ($(".afterReceiveType").attr("checked") == "checked") {
            childCoupon.Term = dayAfterReceive;
        }
        console.log(childCoupon);
        var rules = new Array();
        var $individualRows = $("#IndividualInfo").find("tr");
        for (var i = 1; i < $individualRows.length; i++) {
            var promotionName = $($individualRows).eq(i).find(".couponName").val();
            var descriptName = $($individualRows).eq(i).find(".description").val();
            if (!ProductLibraryConfig.VerifyParameter(promotionName) || !ProductLibraryConfig.VerifyParameter(descriptName)) {
                alert("优惠券信息不完整");
                return;
            }
            if (promotionName.length > 10) {
                alert("优惠券名字不能超过10");
                return;
            }
            rules.push({
                PromtionName: promotionName,
                Description: descriptName,
                Discount: $($individualRows).eq(i).find(".discount").text(),
                Minmoney: $($individualRows).eq(i).find(".miniMoney").text(),
            });
        }
        var rulePidsDic = {};
        for (var item in PassedProducts) {
            rulePidsDic[item] = new Array();
            for (var p in PassedProducts[item]) {
                rulePidsDic[item].push(PassedProducts[item][p].PID);
            }
        }
        if (!lock) {
            lock = true;
            $.post("CreateCoupon", { products: rulePidsDic, parentCoupon: parentCoupon, childCoupon: childCoupon, rules: rules },
                function (rulePkids) {
                    console.log(rulePkids);
                    if (rulePkids) {
                        alert("创建优惠券成功");
                        var moneyRule = $("#CouponUseMoneyConfigTable").find("tr:not(:first)");
                        var unionPkids = new Array();
                        for (var i = 0; i < moneyRule.length; i++) {
                            var rulePKID = moneyRule.eq(i).find(".rulePKID").val();
                            if (rulePKID && rulePKID > 0) {
                                unionPkids.push(rulePKID);
                            } else {
                                var miniMoney = moneyRule.eq(i).find(".miniMoney").val();
                                var discount = moneyRule.eq(i).find(".discount").val();
                                var rulePkid = rulePkids[miniMoney + "/" + discount];
                                if (rulePkid) {
                                    unionPkids.push(rulePkid);
                                }
                            }
                        }
                        var rulePKIDs = ProductLibraryConfig.ArrayJoin(unionPkids, ",");
                        var grossProfit = $("#CouponUseMoneyConfigTable #profitLevel").val();
                        ProductLibraryConfig.SaveProductCouponRules(rulePKIDs, grossProfit, function (result) {
                            if (result.result && result.result.State == 1) {
                                alert("插券成功");
                                $("#CreateCoupon").dialog('close');
                                ProductLibraryConfig.Search();
                                ProductLibraryConfig.ClearTableContent();
                            } else {
                                alert("插券失败");
                            }
                            lock = false;
                        });
                    }
                });
        }
    },
    GetHasExistsRulePkid: function () {
        var rows = $("#CouponUseMoneyConfigTable .rulePKID");
        var result = new Array();
        for (var i = 0; i < rows.length; i++) {
            if (rows.eq(i).val() > 0)
                result.push(rows.eq(i).val());
        }

        return result;
    },
    EditProductDetails: function (el, oid, couponIds) {
        isBatch = 1;
        if (parseInt(oid) > 0) {
            $("#ProductListCheck").attr("checked", false);
            $("input[name='BodyCheckBox']").attr("checked", false);
            $(el).parent().parent().find("td:first-child > input[type='checkbox']").attr("checked", true);

            var dialogConfig = { title: "编辑优惠券", width: 1100, height: 600, modal: true };
            $("#BatchAddCouponView").html("")
                .load('/ProductLibraryConfig/EditCoupon')
                .dialog(dialogConfig);

            //等待dialog渲染完毕,在执行
            setTimeout(function () {
                // ProductLibraryConfig.FillProductsEvent(oid);
                var _couponIds = couponIds.split(',');
                if (_couponIds.length > 0) {
                    $.ajax({
                        type: "POST",
                        dataType: 'json',
                        url: "/ProductLibraryConfig/CouponValidateForPKIDs",
                        data: { "pkids": _couponIds, "oid": oid },
                        success: function (result) {
                            var templateHtml = ""
                            // ProductLibraryConfig.OnblurAddCouponEvent(o, result);
                            if (result != null && result.length > 0) {
                                for (var i = 0; i < result.length; i++) {
                                    templateHtml += "<tr>"
                                    var model = JSON.parse(result[i]);
                                    if (model.RuleID != "") {
                                        templateHtml += " <td><input attribute='CouponNum' class='couponBlur' type='number' value='" + model.RuleID + "' data-Oid='" + oid + "' data-statusType='" + model.Status + "' onblur='ProductLibraryConfig.MatchingCoupon(this);' /></td>";
                                        templateHtml += " <td attribute='CouponName'>" + model.Description + "</td>";
                                        templateHtml += " <td attribute='CouponMinmoney'>" + model.Minmoney + "</td>"
                                        templateHtml += " <td attribute='CouponDiscount'>" + model.Discount + "</td>"
                                        templateHtml += " <td attribute='CouponSaleAmount'>" + model.PriceAfterCoupon + "</td>"
                                        templateHtml += " <td attribute='CouponSalePercent'>" + model.GrossProfit + "</td>"
                                        if (model.CouponDuration.length > 0 && model.CouponStartTime.length == 0 && model.CouponEndTime.length == 0) {
                                            templateHtml += " <td attribute='CouponExplain'>自领取后" + model.CouponDuration + "天</td>"
                                        } else {
                                            templateHtml += " <td attribute='CouponExplain'>固定日期" + model.CouponStartTime + "至" + model.CouponEndTime + "</td>"
                                        }
                                        templateHtml += " <td><input type='button' value='删除' onclick='ProductLibraryConfig.RemoveCoupon(this);' /> </td>"
                                    }
                                    templateHtml += "</tr>";
                                }
                            }
                            $("#BatchAddCoupon tbody").append(templateHtml);
                            $("#BatchAddCoupon .couponBlur").each(function (index) {
                                var statusType = $(this).attr("data-statustype");
                                if (statusType == "OnGoing") {
                                    $(this).parents("tr:first").show();
                                } else {
                                    $(this).parents("tr:first").hide();
                                }
                            });
                        }
                    });
                    //for (var i = 0; i < _couponIds.length; i++) {
                    //    if (parseInt(_couponIds[i]) > 0) {
                    //        var model = new ProductLibraryConfig.CouponModel();
                    //        model.CouponNum = _couponIds[i];
                    //        model.Oid = oid;
                    //        //ProductLibraryConfig.AddCoupon(model);

                    //    }
                    //}
                    //ProductLibraryConfig.AutoOnblurEvent();
                }

                $(".filterCoupon").on("click", function () {
                    var fitlerType = $(this).val();
                    var isChecked = $(this).prop("checked");
                    $("#BatchAddCoupon .couponBlur").each(function (index) {
                        var statusType = $(this).attr("data-statustype");
                        if (statusType == fitlerType) {
                            if (isChecked) {
                                $(this).parents("tr:first").show();
                            } else {
                                $(this).parents("tr:first").hide();
                            }
                        }
                    });
                });
            }, 1200);
            //setTimeout(function () {
            //    $(".couponBlur").each(function (index) {
            //        var statusType = $(this).attr("data-statustype");
            //        if (statusType == "OnGoing") {
            //            $(this).parent().parent().parent().find("tr").eq(index).show();
            //        } else {
            //            $(this).parent().parent().parent().find("tr").eq(index).hide();
            //        }
            //    });
            //},3000);
        }
        else {
            alert("操作失败，请刷新重试！");
        }
    },
    CouponModel: function () {
        this.CouponNum = "",
            this.RuleID = "",
            this.CouponName = "",
            this.CouponExplain = "",
            this.Condition = "",
            this.Description = "",
            this.SupportUserRange = "",
            this.CouponMinmoney = "",
            this.CouponDiscount = "",
            this.CouponSaleAmount = "",
            this.CouponSalePercent = "",
            this.Oid = "",
            this.Status = ""
    },
    AddCouponUseMoneyRow: function () {
        var inputs = $("#CouponUseMoneyConfigTable").find('input');
        for (var i = 0; i < inputs.length; i++) {
            if (inputs[i].type == "button") continue;
            if (inputs[i].type == "text" && !inputs[i].value) {
                alert("请完善优惠券信息");
                return;
            }
        }
        var couponMoneyConfigRow = $("#CouponUseMoneyConfigTemplate").html();
        $("#CouponUseMoneyConfigTable").append(couponMoneyConfigRow);

    },
    AddExistRuleRow: function () {
        var inputs = $("#CouponUseMoneyConfigTable").find('input');
        for (var i = 0; i < inputs.length; i++) {
            if (inputs[i].type == "button") continue;
            if (inputs[i].type == "text" && !inputs[i].value) {
                alert("请完善优惠券信息");
                return;
            }
        }
        var existRuleTemplateRow = $("#ExistRuleTemplate").html();
        $("#CouponUseMoneyConfigTable").append(existRuleTemplateRow);
    },
    FillRuleInfo: function (current) {
        var rulePkid = $(current).val();
        if (rulePkid && rulePkid > 0) {
            this.VerifyCouponRulePKID(rulePkid, function (rule) {
                if (rule) {
                    $(current).parent().parent().find(".miniMoney").val(rule.Minmoney);
                    $(current).parent().parent().find(".discount").val(rule.Discount);
                    ProductLibraryConfig.VerfyProductCoupon();
                } else {
                    alert("不存在的规则");
                    $(current).val("");
                }
            });
        }
    },
    ProductDetailsModel: function () {
        this.Iamges = "",
            this.DisplayName = "",
            this.CP_Brand = "",
            this.CP_ShuXing5 = "",
            this.OnSale = "",
            this.PID = "",
            this.CP_Tire_Pattern = "",
            this.CP_Tire_Rim = "",
            this.cy_list_price = "",
            this.cy_marketing_price = ""
    },
    SecondSearchProudsModel: function () {
        this.S_Price_Begin = "",
            this.S_Price_End = "",
            this.S_PID = "",
            this.S_Coupon = "",
            this.S_Figure = "",
            this.S_Params = "",
            this.maoli = "",
            this.maoliSort = "",
            this.pageSize = "",
            this.S_price = "",
            this.S_IsShow
    },
    ReplaceTemplate: function (tmplhtml, obj) {
        for (var key in obj) {
            if (obj.hasOwnProperty(key)) {
                if (obj[key] != null && obj[key] != "null") {
                    tmplhtml = tmplhtml.replace(new RegExp("\{\{" + key + "\}\}"), obj[key]);
                } else {
                    tmplhtml = tmplhtml.replace(new RegExp("\{\{" + key + "\}\}"), "");
                }
            }
        }
        return tmplhtml;
    },
    AddCoupon: function (model, type) {
        var $html = "";
        if (type == "BatchDelete") {
            $html = $("#BatchDeleteCoupon").html();
        } else {
            $html = $("#AssignCouponView").html();
        }
        model = model || new ProductLibraryConfig.CouponModel();
        $("#BatchAddCoupon tbody").append(ProductLibraryConfig.ReplaceTemplate($html, model));
        if (type != "BatchDelete") {
            $("#BatchAddCoupon .couponBlur").blur();
        }
    },
    RemoveCoupon: function (o) {
        $(o).parent("td").parent("tr").remove();
        //if (confirm("删除不可恢复，确认？")) {
        //}
    },
    RemoveCurrentRow: function (o) {
        $(o).parent().parent().remove()
    },
    MatchingCoupon: function (o, type) {
        var id = $(o).val() || 0;
        if (!isNaN(id)) {
            if (type == "BatchDelete") {
                $.ajax({
                    type: "POST",
                    dataType: 'json',
                    url: "/ActivityV1/CouponVlidateForPKID",
                    data: { "couponRulePKID": id, "oid": $(o).attr("data-Oid") },
                    success: function (result) {
                        ProductLibraryConfig.OnblurAddCouponEvent(o, result, "BatchDelete");
                    }
                });
            } else {
                $.ajax({
                    type: "POST",
                    dataType: 'json',
                    url: "/ProductLibraryConfig/CouponVlidateForPKID",
                    data: { "couponRulePKID": id, "oid": $(o).attr("data-Oid") },
                    success: function (result) {
                        ProductLibraryConfig.OnblurAddCouponEvent(o, result);
                    }
                });
            }
        }
    },
    VerifyCouponRulePKID: function (id, callback) {
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: "/ActivityV1/CouponVlidateForPKID",
            data: { "couponRulePKID": id },
            success: function (result) {
                if (result && result.RuleID && result.RuleID != "") {
                    callback(result);
                } else {
                    callback(null);
                }
            }
        });
    },
    OnblurAddCouponEvent: function (o, data, type) {
        if (parseInt(data.RuleID || 0) <= 0) {
            alert("优惠券: " + ($(o).val() || "") + " 不匹配！请填写正确的【优惠券规则编号】");
            $(o).val("");
            return false;
        }


        if (type == "BatchDelete") {
            var $this = $(o).parent("td");
            $this.next().html(data.Description);
            if (data.CouponDuration.length > 0
                && data.CouponStartTime.length == 0
                && data.CouponEndTime.length == 0) {
                $this.next().next().html(["自领取后", data.CouponDuration, "天"].join(" "));
            }
            else {
                $this.next().next().html(["固定日期", data.CouponStartTime, "至", data.CouponEndTime].join(" "));
            }
            $this.next().next().next().html(parseInt(data.SupportUserRange) == 0 ? "全部" : parseInt(data.SupportUserRange) == 1 ? "新用户" : "老用户");
        } else {
            var $this = $(o).parent().parent();
            $this.find("td").eq(0).find("input").attr("data-statustype", data.Status);
            $this.find("td").eq(1).html(data.Description);
            $this.find("td").eq(2).html(data.Minmoney);
            $this.find("td").eq(3).html(data.Discount);
            $this.find("td").eq(4).html(data.PriceAfterCoupon);
            $this.find("td").eq(5).html(data.GrossProfit);

            if (data.CouponDuration.length > 0
                && data.CouponStartTime.length == 0
                && data.CouponEndTime.length == 0) {
                $this.find("td").eq(6).html(["自领取后", data.CouponDuration, "天"].join(" "));
            }
            else {
                $this.find("td").eq(6).html(["固定日期", data.CouponStartTime, "至", data.CouponEndTime].join(" "));
            }
        }
        //$this.next().next().next().html(parseInt(data.SupportUserRange) == 0 ? "全部" : parseInt(data.SupportUserRange) == 1 ? "新用户" :"老用户");
    },
    AutoOnblurEvent: function () {
        var $BatchAddCoupon = $("#BatchAddCoupon tbody tr");
        $BatchAddCoupon.each(function (i, el) {
            $(el).find("td:first-child > input[type='text']").blur();
        });
    },
    FillProductsEvent: function (oid) {
        $.ajax({
            type: 'POST',
            dataType: 'json',
            url: "/ProductLibraryConfig/GetProduct",
            data: { "oid": oid },
            success: function (data) {
                if (data != null || data != "null") {
                    var $html = $("#BatchAddCouponForProductDetailsView").html();
                    data.Image = ImageDomain + data.Image;
                    data.OnSale = data.OnSale == "True" ? "上架" : "下架";
                    $("#BatchAddCouponForProductDetails").append(ProductLibraryConfig.ReplaceTemplate($html, data));
                }
            }
        });
    },
    FilSearchWhereEvent: function (datas) {
        datas = datas || ["data-brand", "data-tab", "data-rim"];
        for (var i = 0; i < datas.length; i++) {
            var $datas = $("input[" + datas[i] + "]"), datasArr = [];

            $datas.each(function (j, el) {
                if ($(el).is(":checked")) {
                    datasArr.push($(el).attr("" + datas[i] + ""));
                }
            });

            CookieTools.SetCookie(datas[i], datasArr.join(","));
        }
    },
  
    ArrayJoin: function (data, decollator) {
        return Array.prototype.join.call(data, decollator);
    },
    Save: function (op) {
        var _olds = this.ArrayJoin(this.GetCheckList(), ",")
            , _coupons = []
            , _couponVaule
            , $BatchAddCoupon = $("#BatchAddCoupon > tbody > tr");

        $BatchAddCoupon.each(function (i, e) {
            _couponVaule = $(e).first().find("input[type='number']").val();
            if (parseInt(_couponVaule) > 0) {
                _coupons.push(_couponVaule);
            }
        });

        if (isBatch == 0 && _coupons.length <= 0) {
            alert("优惠券不能为空！");
            return false;
        }
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: "/ProductLibraryConfig/BatchAddCoupon",
            data: { "oids": _olds, "coupons": this.ArrayJoin(_coupons, ","), "isBatch": isBatch, "opration": op },
            success: function (result) {
                if (parseInt(result.State) == 1) {
                    alert("保存成功!");
                    $('#BatchAddCouponView').dialog('close');
                    ProductLibraryConfig.Search();
                }
                else {
                    alert("保存失败!");
                }

            }
        });
    },
    SaveProductCouponRules: function (rulePKID, profitLevel, callback) {
        var _olds = this.ArrayJoin(this.GetCheckList(), ",")
        $.post("/ProductLibraryConfig/BatchAddCouponNew",
            { "oids": _olds, "rulePKIDs": rulePKID, "grossProfit": profitLevel },
            function (data) {
                return callback(data);
            });
    },
    InitStateElement: function () {
        //等待DOM[#ParamsList]元素加载完毕在执行
        setTimeout(function () {

            //初始化分页状态
            var _dataPage = CookieTools.GetCookie("data-page") || 1;
            $(".pageStyle[data-page='" + _dataPage + "']").css("background", "#0088cc").siblings().css("background", "#cccccc");
            CookieTools.SetCookie("data-page", _dataPage);
            console.log("brand=" + CookieTools.GetCookie("data-brand"));
            //初始化 品牌，标签，尺寸 状态
            var _searchCookies = ["data-brand", "data-tab", "data-rim"];
            for (var i = 0; i < _searchCookies.length; i++) {
                var _dataCookie = CookieTools.GetCookie(_searchCookies[i]);

                if (_dataCookie != null) {
                    _dataCookie.split(",").forEach(function (e) {
                        $("input[" + _searchCookies[i] + "='" + e + "']").attr("checked", true);
                    });
                }
            }

            //初始化二级搜索状态
            var _SecondSearchProudsModel = JSON.parse(CookieTools.GetCookie("SecondSearchProuds") || "");
            console.log("初始化二级搜索状态=" + JSON.stringify(_SecondSearchProudsModel));
            if (_SecondSearchProudsModel != "") {
                $("#ProductList input[data-id='S_Price_Begin']").val(_SecondSearchProudsModel.S_Price_Begin);
                $("#ProductList input[data-id='S_Price_End']").val(_SecondSearchProudsModel.S_Price_End);
                $("#ProductList input[data-id='S_PID']").val(_SecondSearchProudsModel.S_PID);
                $("#ProductList input[data-id='S_Coupon']").val(_SecondSearchProudsModel.S_Coupon);
                //$("#ProductList input[data-id='filtrateType']").val(_SecondSearchProudsModel.FiltrateType);
                $("#ProductList input[data-id='S_ML_Begin']").val(_SecondSearchProudsModel.S_ML_Begin);
                $("#ProductList input[data-id='S_ML_End']").val(_SecondSearchProudsModel.S_ML_End);
                $("#S_Figure").val(_SecondSearchProudsModel.S_Figure);
                $("#pageSize").val(_SecondSearchProudsModel.pageSize);
                $("#onsale").val(_SecondSearchProudsModel.onsale);
                $("#maoli").val(_SecondSearchProudsModel.maoliSort);
                //$("#S_price").val(_SecondSearchProudsModel.S_price);
                $("#S_IsShow").val(_SecondSearchProudsModel.S_IsShow)
                $("#filtrateType").val(_SecondSearchProudsModel.FiltrateType)
            }

            //初始化搜索类型
            if (CookieTools.GetCookie("SearchType") || 0 > 0)
                CookieTools.SetCookie("SearchType", CookieTools.GetCookie("SearchType"));
            else
                CookieTools.SetCookie("SearchType", 1);

        }, 3000)
    },
    CheckBoxManage: function (obj,id,name) {
        var s = arguments;
        var ischeck = $(obj).is(":checked");
        $("#" + id + " input[" + name + "]").attr("checked", ischeck);
    },
    DownOrUp: function (obj) {
        var _value = $(obj).attr("data-value") || 1;
        if (_value == 1)
            $("#tbody-condition").hide();
        else
            $("#tbody-condition").show();
        $(obj).attr("data-value", _value == 1 ? 0 : 1);
    },
    ReloadUseList: function () {
        $.get("GetDepartmentUseSetting", function (setting) {
            var partVal = $("#publicChildCouponInfo #partList").val();
            var useList = setting.filter(function (t) { return t.Type == 1 && t.ParentSettingId == partVal });
            var useOptionHtml = "<option value='-1'>请选择部门</option>";
            for (var i = 0; i < useList.length; i++) {
                useOptionHtml += "<option value=" + useList[i].SettingId + " >" + useList[i].DisplayName + "</option>";
            }
            $("#publicChildCouponInfo #useList").empty().append(useOptionHtml);
        });
    },
    ClearTableContent: function () {
        $("#CouponUseMoneyConfigTable").find("tr:not(:first)").remove();
        $("#NotAvaiableProduct").find("tr:not(:first)").remove();
        $("#NotAvaiableProductTips").hide();
        $("#NotAvaiableProduct").hide();
    },
   
    VerifyParameter: function (parameter) {
        if (parameter && parameter != '' && parameter != 'undefined' && parameter != '-1') {
            return true;
        } else {
            return false;
        }
    }
};

$(function () {
    //文件上传
    $("#form-product-condition").ajaxForm(function (data) {
        if (data && data.result) {
            $('#imput_pid_Ids').val('');
            alert('上传成功！');
            $('#tb-condition2 input[name="S_PID"]').val('');
            $('#imput_pid_Ids').val(data.Pids);
            $('#tb-condition2 select[name="pageSize"]').val('500');
            ProductLibraryConfig.Search();
            $('#btn_clear_imput_Excel').show();
            $('#btn_imput_Excel').hide();
        } else if (data && !data.result) {
            alert(data.errorMessage);
        } else {
            alert('上传失败，请检查excel格式和状态！');
        }
        $('#fileInputPIds').val('');
    });
    setTimeout(function () {
        ProductLibraryConfig.DefaultLoad();
    }, 1000);
    $("#publicChildCouponInfo .startDate").datetimepicker({
        timeFormat: "HH:mm:ss",
        dateFormat: "yy-mm-dd"
    });
    $("#publicChildCouponInfo .endDate").datetimepicker({
        timeFormat: "HH:mm:ss",
        dateFormat: "yy-mm-dd"
    });
    $("#publicChildCouponInfo .ShowStartTime").datetimepicker({
        timeFormat: "HH:mm:ss",
        dateFormat: "yy-mm-dd"
    });
    $("#publicChildCouponInfo .ShowEndTime").datetimepicker({
        timeFormat: "HH:mm:ss",
        dateFormat: "yy-mm-dd"
    });
    $("#publicChildCouponInfo .finalValidity").datetimepicker({
        timeFormat: "HH:mm:ss",
        dateFormat: "yy-mm-dd"
    });
})