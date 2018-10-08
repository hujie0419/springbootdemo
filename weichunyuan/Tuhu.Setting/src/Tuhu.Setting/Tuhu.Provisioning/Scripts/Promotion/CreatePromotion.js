var CreatePromotionTask = {
    //添加产品类别
    AddProductCategory: function () {
        var hidProIds = 'ProductCategoryIds';
        var ids = $('#' + hidProIds + '').val();
        ProductCategoryCommon.OpenComonPage(ids,function (checkNodes) {
            
            ids = "";
            var parentIds = "";
            var categoryNames = "";
            var nodeNos = "";
           
            var name = "";
            for (var i = 0; i < checkNodes.length; i++) {
                var m = checkNodes[i];
                name += m.name + ",";
                ids += m.id + ",";
                if (!m.pId) {
                    m.pId = -1;
                }
                parentIds += m.pId + ",";
                categoryNames += m.CategoryName + ",";
                nodeNos += m.NodeNo + ",";
                console.log(m);
            }
            if (name.indexOf(',') > 0) {
                name = name.substring(0, name.length - 1);
            }
            if (ids.indexOf(',') > 0) {
                ids = ids.substring(0, ids.length - 1);
            }
            if (parentIds.indexOf(',') > 0) {
                parentIds = parentIds.substring(0, parentIds.length - 1);
            }
            if (categoryNames.indexOf(',') > 0) {
                categoryNames = categoryNames.substring(0, categoryNames.length - 1);
            }
            if (ids.indexOf(',') > 0) {
                nodeNos = nodeNos.substring(0, nodeNos.length - 1);
            }
         
            $('#' + hidProIds+'').val(ids);
            $('input.ProductCategoryIds').val(name);
            $('#ProductNames').val(name);
            $("#ParentCategoryIds").val(parentIds);
            $("#CategoryNames").val(categoryNames);
            $("#NodeNos").val(nodeNos);
            //赋值默认值
            CreatePromotionTask.PruductCategotyBulidHtml();
        });
    },
    //删除选择的内容
    CloseAddInfoExhibition: function (obj) {
        $(obj).parent('div').remove();
        $('input.ProductCategoryIds').val('');
        $('#ProductCategoryIds').val('');
        $('#ProductNames').val('');
        $("#ParentCategoryIds").val('');
        $("#CategoryNames").val('');
        $("#NodeNos").val('');
       
    },
    PruductCategotyBulidHtml: function () {
        if ($.trim($('#ProductCategoryIds').val()).length > 0) {
            $("#selectedFilterDiv").find('div[name="divTriggerCategory"]').remove();
            var divHtml = '<div class="selectedFilterValue" name="divTriggerCategory">';
            divHtml += '<label style="cursor: pointer; ">产品类别:' + $('#ProductNames').val() + '</label>';
            divHtml += '<label name="filterValue" style="cursor: pointer;display:none ">' + $('#ProductCategoryIds').val() + '</label>';
            divHtml += '<a href="#" onclick="CreatePromotionTask.CloseAddInfoExhibition(this)">X</a>';
            divHtml += '</div>';
            $("#selectedFilterDiv").append(divHtml);
            $('input.ProductCategoryIds').val($('#ProductNames').val());
        }
    },
    EditBindCategory: function () {
        //赋值默认值
        CreatePromotionTask.PruductCategotyBulidHtml();
    },
    RepeatClick: function () {
        $("#divRepeatTask").show();
        $("#UploadFileSpan").hide();
        $("#TaskActivitySpan").hide();
        $("#ConditionSelect").click();
        $("#StartTimeDiv").show();
        $('#div-car-vehicle-info').hide();
        $("#div-once-product-category").hide();
        $("#div-trigger-product-category").show();
    },
    OnceClick: function () {
        $("#divRepeatTask").hide();
        $("#UploadFileSpan").show();
        $("#TaskActivitySpan").show();
        $("#StartTimeDiv").hide();
        $('#div-car-vehicle-info').show();
        $("#div-once-product-category").show();
        $("#div-trigger-product-category").hide();
        $("#selectedFilterDiv").find('div[name="divTriggerCategory"]').remove();
    }
};

$(function () {
    //ClearOldValue();
    //优惠券数据绑定
    CreatePromotionTask.PruductCategotyBulidHtml();
    //初始化优惠券规则信息
    var taskPromotionListIds = $("#TaskPromotionListIds").val().trim();
    var couponRulesIds = $("#CouponRulesIds").val().trim();
    couponRulesIds = couponRulesIds.substring(1, couponRulesIds.length - 1);
    if (couponRulesIds.length > 0) {
        var elementTitle = "编号:";
        var elementName = "";

        var arr = couponRulesIds.split(',');
        var arrListId = taskPromotionListIds.split(',');
        for (var i = 0; i < arr.length; i++) {
            if (arr[i].length > 0) {
                elementName = "couponDIv" + $("#couponsDiv").children().length;
                var couponsAttributes = [["TaskPromotionListId", arrListId[i]], ["PromotionRuleId", arr[i]], ["CouponsDescription", ''], ["UseStartTime", ''], ["UseEndTime", ''],
                ["UseMoney", ''], ["DiscountMoney", ''], ["FinanceMarkName", '']];
                var couponDiv = createElement(elementTitle, elementName, arr[i], couponsAttributes);
                $("#couponsDiv").append(couponDiv);
            }
        }
    }

    //button 取消
    $("#btnCancel").click(function () {
        location.href = "/promotion/SearchPromotion";
    });

    $("[id=FilterStartTime],[id=FilterEndTime]").datetimepicker({
        //minDate: new Date(),
        controlType: 'select',
        dateFormat: "yy-mm-dd",
        timeFormat: 'HH:mm'
    });
    $("[id=UseStartTime],[id=UseEndTime]").datetimepicker({
        //minDate: new Date(),
        controlType: 'select',
        showTimepicker: false,
        dateFormat: "yy-mm-dd"
    });

    $("[id=TaskStartTime],[id=TaskEndTime]").datetimepicker({
        controlType: 'select',
        showTimepicker: false,
        minDate: +1,
        todayButton: false,   //关闭选择今天按钮
    });
    $("#TaskStartTime").change(function () {
        //if (!(
        //    $("input[name=excuteType]:eq(0)").attr("checked") == "checked"
        //    && !$("input[name=excuteType]:eq(0)").is(':hidden')
        //    ))
        $("#TaskStartTime").val($("#TaskStartTime").val().substr(0, 10) + " 00:01");
    });
    $("#TaskEndTime").change(function () {
        $("#TaskEndTime").val($("#TaskEndTime").val().substr(0, 10) + " 23:59");
    });
    $("#excuteType,#biExcuteType").change(function() {
        $("#IsImmediately").val($(this).prop("checked") ? 1 : 0);
    });
    if ($("#IsImmediately").val() + "" == "1") {
        if (selectUserType == 1) {
            $("#excuteType").prop("checked", true);
        }else if (selectUserType == 3) {
            $("#biExcuteType").prop("checked", true);
        }

    } else {
        if (selectUserType == 1) {
            $("#excuteType").prop("checked", false);
        } else if (selectUserType == 3) {
            $("#biExcuteType").prop("checked", false);
        }
    }

    //单次任务
    $("#Once").click(function () {
        CreatePromotionTask.OnceClick();
    });
    //触发任务
    $("#Repeat").click(function () {
        CreatePromotionTask.RepeatClick();
    });
     
    //add产品信息
    $("#selectProductInfo").change(function () {
        var name = $(this).val();
        if ($.trim(name) == "Brand") {
            $("#selBrandInfo").show();
            $("#productInfo").hide();
            $("#productListInfo").hide();
            $("#lookPid").hide();
        } else if ($.trim(name) == "Pid") {
            $("#selBrandInfo").hide();
            $("#productInfo").show();
            $("#productListInfo").show();
            $("#lookPid").show();
            $("#productInfo").val("");
        } else {
            $("#selBrandInfo").hide();
            $("#productInfo").show();
            $("#productListInfo").hide();
            $("#productListInfo").val("");
            $("#lookPid").hide();
            $("#productInfo").val("");
        }
    });

    $("#btnAddProductInfoFilter").click(function () {
        var filterDiv = $("#selectedFilterDiv");
        var value = $("#selectProductInfo").val();
        var text = $("#selectProductInfo").find("option:selected").text();
        var productInfo = $.trim(text) == "品牌" ? $("#selBrandInfo").find("option:selected").text() : $("#productInfo").val();

        if (value === "Brand") {
            CreateMultiFilter(text + ":" + productInfo, value, productInfo);
            ResetMultiFilterValue(value);
        } else if (value === "Pid") {
            var xxx = "";
            if (productInfo.length > 0) {
                xxx = productInfo + ",";
            }else  if (!document.getElementById("productListInfo").files[0]) {
                alert("请上传pid或者填写pid");
                return;
            }
            if (document.getElementById("productListInfo").files[0])
            {
                var ddd = new FileReader();
                ddd.readAsText(document.getElementById("productListInfo").files[0]);
                ddd.onload = function (aa) {
                    xxx += aa.target.result;
                    if (xxx.length > 0) {
                        var aaa = xxx.split(/[,，]/);
                        $.each(aaa, function (i, n) {
                            if (n.trim().length > 0) {
                                CreateMultiFilter(text + ":" + n.trim(), value, n.trim());
                                ResetMultiFilterValue(value);
                            }
                        });
                    }
                }
            }
            else {
                if (xxx.length > 0) {
                    var aaa = xxx.split(/[,，]/);
                    $.each(aaa, function (i, n) {
                        if (n.trim().length > 0) {
                            CreateMultiFilter(text + ":" + n.trim(), value, n.trim());
                            ResetMultiFilterValue(value);
                        }
                    });
                }
            };

        }
        else {
            if (productInfo.length > 0) {
                var newElement = CreateElementFilter(text + ":" + productInfo, value, productInfo);
                filterDiv.append(newElement);
            }
            $("#" + value).val(productInfo);
        }
    });
    //地区
    $("#btnAddRegionFilter").click(function () {
        var filterDiv = $("#selectedFilterDiv");
        var text = "地区", value = "Area";
        var destValue = "", destText = "", newElement = "", attributes = "";
        destValue = $("#selectCity").val();
        destText = $("#selectCity").find("option:selected").text();
        if (destText.length > 0) {
            CreateMultiFilter(text + ":" + destText, value, destValue);
        }

        //submit data set value
        ResetMultiFilterValue(value);
    });
    //省市联动
    $("#selectProvince").change(function () {
        var pid = $("#selectProvince").val();

        $.ajax({
            type: 'GET',
            url: '/Promotion/SelectRegionInfoById',
            dataType: 'json',
            data: "id=" + pid,
            contentType: "application/json; charset=utf-8",
            success: function (jsonObj) {
                var filterDiv = $("#selectCity");
                filterDiv.empty();
                if (jsonObj.IsSuccess) {
                    $.each(jsonObj.ObjectData, function (i, row) {
                        filterDiv.append($("<option></option>").val(row["PKID"]).text(row["RegionName"]));
                    });
                }
            }
        });
    });

    $("#selectProvince").change();

    //产品分类
    $("#btnAddCategoryFilter").click(function () {
        var filterDiv = $("#selectedFilterDiv");
        var text = "产品类别", value = "Category";
        var destValue = "", destText = "", newElement = "", attributes = "";
        var oneValue = "", oneText = "", twoValue = "", twoText = "", threeValue = "", threeText = "";
        //destValue = $("#selectCity").val();
        //destText = $("#selectCity").find("option:selected").text();
        oneValue = $("#selectOneCatagory").val();
        oneText = $("#selectOneCatagory").find("option:selected").text();
        twoValue = $("#selectTwoCatagory").val();
        twoText = $("#selectTwoCatagory").find("option:selected").text();
        threeValue = $("#selectThreeCatagory").val();
        threeText = $("#selectThreeCatagory").find("option:selected").text();

        if (oneValue != null && oneValue != 0) {
            destValue = $("#selectOneCatagory").val();
            destText = $("#selectOneCatagory").find("option:selected").text();
        }
        var elementName = "CatagoryType";

        if (destText.length > 0) {
            var newElement = CreateElementFilter("产品类别:" + destText, "Category", destValue);
            filterDiv.append(newElement);
        }

        //submit data set value
        $("#" + value).val(destValue);
    });

    //产品类别联动
    $("#selectOneCatagory").change(function () {
        //修改为只能勾选大类
        return;
        var id = $("#selectOneCatagory").val();

        $.ajax({
            type: 'GET',
            url: '/Promotion/GetProductCategories',
            dataType: 'json',
            data: "oid=" + id,
            contentType: "application/json; charset=utf-8",
            success: function (jsonObj) {
                $("#selectThreeCatagory").empty();
                $("#selectThreeCatagory").hide();
                var filterDiv = $("#selectTwoCatagory");
                filterDiv.empty();
                filterDiv.append($("<option></option>").val("0").text("全部"));
                if (jsonObj.IsSuccess) {
                    filterDiv.show();
                    $.each(jsonObj.ObjectData, function (i, row) {
                        filterDiv.append($("<option></option>").val(row["Oid"]).text(row["DisplayName"]));
                    });
                } else {
                    filterDiv.hide();
                }
            }
        });
    });
    $("#selectTwoCatagory").change(function () {
        var id = $("#selectTwoCatagory").val();

        $.ajax({
            type: 'GET',
            url: '/Promotion/GetProductCategories',
            dataType: 'json',
            data: "oid=" + id,
            contentType: "application/json; charset=utf-8",
            success: function (jsonObj) {
                var filterDiv = $("#selectThreeCatagory");
                filterDiv.empty();

                if (jsonObj.IsSuccess) {
                    filterDiv.append($("<option></option>").val("0").text("全部"));
                    filterDiv.show();
                    $.each(jsonObj.ObjectData, function (i, row) {
                        filterDiv.append($("<option></option>").val(row["Oid"]).text(row["DisplayName"]));
                    });
                } else {
                    filterDiv.hide();
                }
            }
        });
    });


    //渠道
    $("#btnAddOrderrChannelFilter").click(function () {
        var filterDiv = $("#selectedFilterDiv");

        var value = $("#selectOrderChannel").val();
        var text = $("#selectOrderChannel").find("option:selected").text();

        if (text.length > 0) {
            CreateMultiFilter("渠道:" + text, "Channel", value);
        }
        //submit data set value
        ResetMultiFilterValue("Channel");

    });
    //安装类型
    $("#btnAddInstallTypeFilter").click(function () {
        var filterDiv = $("#selectedFilterDiv");

        var value = $("#selectInstallType").val();
        var text = $("#selectInstallType").find("option:selected").text();

        if (text.length > 0) {
            var newElement = CreateElementFilter("安装类型:" + text, "InstallType", value);
            filterDiv.append(newElement);
        }
        //submit data set value
        $("#InstallType").val(value);

    });
    //订单状态
    $("#btnAddOrderStateFilter").click(function () {
        var filterDiv = $("#selectedFilterDiv");

        var value = $("#selectOrderState").val();
        var text = $("#selectOrderState").find("option:selected").text();

        if (text.length > 0) {
            var newElement = CreateElementFilter("订单状态:" + text, "OrderStatus", value);
            filterDiv.append(newElement);
        }
        //submit data set value
        $("#OrderStatus").val(value);

    });
    //添加订单时间过滤
    $("#btnAddOrdersDateTimeFilter").click(function () {
        var filterDiv = $("#selectedFilterDiv");
        var filterStartTime = $("#FilterStartTime").val();
        var filterEndTime = $("#FilterEndTime").val();
        var filterTime = (filterStartTime + "," + filterEndTime);
        var newElement = CreateElementFilter(("订单时间:" + filterTime), "ordersTime", filterTime);
        filterDiv.append(newElement);
    });


    //加载车型
    LoadBrand();
    //车型信息
    $("#btnAddVehicleInfoFilter").click(function () {
        var filterDiv = $("#selectedFilterDiv");
        var elementName = "Vehicle";

        var value = $("#selectSeableInfo").val();
        var text = $("#selectSeableInfo").find("option:selected").text();

        var destValue = $("#selectVehicleInfo").val();
        var destText = $("#selectVehicleInfo").find("option:selected").text();


        //豆腐块
        if (destText.length > 0 && destValue != '') {
            CreateMultiFilter("车系:" + destText, elementName, destValue);
        }
        //submit data set value
        ResetMultiFilterValue(elementName);

    });
    //订单的产品类型
    $("#btnAddProductTypeFilter").click(function () {
        var filterDiv = $("#selectedFilterDiv");
        var productTypeValue = $('input[type="radio"][name="ProductType"]:checked').val();
        var productTypeText = $("input[name='ProductType']:checked")[0].nextSibling.nodeValue;  
        var newElement = CreateElementFilter(("订单类型:" + productTypeText), "productType", productTypeValue);
        filterDiv.append(newElement);
    });

    function ClearOldValue() {
        $("#FilterStartTime").val('');
        $("#FilterEndTime").val('');
        $("#Brand").val('');
        $("#Category").val('');
        $("#Pid").val('');
        $("#SpendMoney").val('');
        $("#PurchaseNum").val('');
        $("#Area").val('');
        $("#Channel").val('');
        $("#InstallType").val('');
        $("#OrderStatus").val('');
        $("#Vehicle").val('');
    }
    //规则创建多选过滤框
    function CreateMultiFilter(title, elementNameKey, elementIdCode) {
        var filterDiv = $("#selectedFilterDiv");
        var idCode = elementIdCode;
        var newElement = CreateElementFilter(title, elementNameKey + "_" + idCode, idCode);//"Vehicle_" + value.replace(/\ +/g, "") + "_" + 
        filterDiv.append(newElement);
    }
    //submit MultiData set value
    function ResetMultiFilterValue(elementName) {
        $("#" + elementName).val(GetValueStrById(elementName + "_", elementName + "Id"));
    }


    /*
    修改时创建已有过滤条件
TODO:提交修改值
    */
    $.each(filterJson, function (i, row) {
        //var str = '';
        //str += "|| Title:" + row["Title"] + "-ElementName:" + row["ElementName"];
        var filterDiv = $("#selectedFilterDiv");
        var newElement = CreateElementFilter(row["Title"], row["ElementName"], row["ElementValue"]);
        filterDiv.append(newElement);
    });

    /*
创建过滤条件控件
title;过滤条件显示内容
elementName:元素Key
elementValue 元素值
    <div class="selectedFilterValue" id="【elementName】" name="Channel_OwnChannel" >
        <label style="cursor: pointer; ">【title】</label>
        <label id="filterValue" style="cursor: pointer;display:none ">【elementValue】</label>
        <a href="#">X</a>
    </div>
    */
    function CreateElementFilter(title, elementName, elementValue) {

        //已经添加过此类型的过滤条件，则进行内容替换
        if ($("#selectedFilterDiv #" + elementName.replace(/(:|\.|\/|\\|\|)/g, '\\$1')).length > 0) {
            $("#selectedFilterDiv #" + elementName.replace(/(:|\.|\/|\\|\|)/g, '\\$1')).find("[name='filterValue']").html(elementValue);
            $("#selectedFilterDiv #" + elementName.replace(/(:|\.|\/|\\|\|)/g, '\\$1')).find("[name='filterValue']").prev().html(title);
            return;
        }

        var divHtml = '<div id="' + elementName + '" name="' + elementName + '"';
        divHtml += '/>';

        var divElement = $(divHtml);
        var labelTitle = $('<label style="cursor: pointer; ">' + title + '</label>');
        var labelElement = $('<label name="filterValue" style="cursor: pointer;display:none ">' + elementValue + '</label>');

        var aElement = $('<a href="#">X</a>');
        aElement.click(function () {
            RemoveFilterItem(elementName);
        });
        divElement.addClass("selectedFilterValue");
        divElement.append(labelTitle);
        divElement.append(labelElement);
        divElement.append(aElement);

        return divElement;
    }


    //filter user button
    $("#btnFilter").click(function () {
        if (!validateUserChoose())
            return;

        var param = {
            FilterStartTime: $("#FilterStartTime").val(),
            FilterEndTime: $("#FilterEndTime").val(),
            Brand: $("#Brand").val(),
            Category: $("#Category").val(),
            Pid: $("#Pid").val(),
            SpendMoney: $("#SpendMoney").val(),
            PurchaseNum: $("#PurchaseNum").val(),
            Area: $("#Area").val(),
            Channel: $("#Channel").val(),
            InstallType: $("#InstallType").val(),
            OrderStatus: $("#OrderStatus").val(),
            Vehicle: $("#Vehicle").val()
        };

        $.ajax({
            type: 'POST',
            url: '/Promotion/FilterUserByCondition',
            dataType: 'json',
            data: JSON.stringify(param),
            contentType: "application/json; charset=utf-8",
            beforeSend: function () {
                $("#filterResult").html("正在查询中....");
                $("#btnFilter").attr('disabled', 'disabled');
            },
            success: function (jsonObj) {
                $("#btnFilter").removeAttr('disabled');
                if (!jsonObj.IsSuccess) {
                    $("#filterResult").html("共筛选出<font color='red'>" + jsonObj.ObjectData + "</font>位用户");
                    return;
                }
                else
                    $("#filterResult").html("查询异常");
            }
        });
    });

    //filter user button
    $("#btnFilterDown").click(function () {
        if (!validateUserChoose())
            return;

        $("#FilterStartTime_Down").val($("#FilterStartTime").val());
        $("#FilterEndTime_Down").val($("#FilterEndTime").val());
        $("#Brand_Down").val($("#Brand").val());
        $("#Category_Down").val($("#Category").val());
        $("#Pid_Down").val($("#Pid").val());
        $("#SpendMoney_Down").val($("#SpendMoney").val());
        $("#PurchaseNum_Down").val($("#PurchaseNum").val());
        $("#Area_Down").val($("#Area").val());
        $("#PurchaseNum_Down").val($("#PurchaseNum").val());
        $("#Channel_Down").val($("#Channel").val());
        $("#InstallType_Down").val($("#InstallType").val());
        $("#OrderStatus_Down").val($("#OrderStatus").val());
        $("#Vehicle_Down").val($("#Vehicle").val());
        document.getElementById('PromotionFormDown').submit();

    });

    
});

function GetValueStrById(keyPrefix, id) {
    var tmpStr = "";
    $("div[id^=" + keyPrefix + "]").each(function () {

        if (tmpStr === "") {
            //tmpStr = $(this).attr(id);
            tmpStr = $(this).find("label[name='filterValue']").html()
        } else {
            //tmpStr = tmpStr + "," + $(this).attr(id);
            tmpStr = tmpStr + "," + $(this).find("label[name='filterValue']").html()
        }
    });
    return tmpStr;
}

function uploadUserCellphoneFile() {
    var oldElement = $('#CellphonesFile', document.forms[0]);
    $.ajaxFileUpload({
        url: '/Promotion/UploadUserFile',
        secureuri: false,
        fileElementId: 'CellphonesFile',
        dataType: 'json',
        data: {},
        type: "post",
        success: function (data) {
            if (data.IsSuccess) {
                $("#UploadUserFileTip").html("共筛选出<font style='color:red;'>" + data.ObjectData + "</font>位用户,已存在<font style='color:red;'>" + data.ExistsData + "</font>位用户,未注册<font style='color:red;'>" + (data.ObjectData - data.ExistsData) + "</font>位用户");
                if (Number(data.ObjectData) <= 100) {
                    $("input[name=excuteType]:eq(0)").attr("checked", 'checked');
                    $("div[name=excuteTypeDiv]:eq(0)").removeAttr('style');
                }
                else {
                    $("input[name=excuteType]:eq(0)").removeAttr("checked");
                    $("div[name=excuteTypeDiv]:eq(0)").attr('style', 'display:none');
                }
                $("#excuteType").change();
            } else {
                $("#UploadUserFileTip").html("<font style='color:red;width: 800px;color:red;display: block;word-break: break-all;word-wrap: break-word;'>" + data.OutMessage + "</font>");
            }
        },
        complete: function (xmlHttpRequest) {
            var pos = oldElement[0].value.lastIndexOf("\\");
            $("[name='fileName']")[0].value = oldElement[0].value.substring(pos + 1);

            var tmpfile = "<input type=\"file\" id=\"tmpfile\" />";
            $("[name='fileA']").html(tmpfile)
            $("#CellphonesFile").remove();
            $("#tmpfile").replaceWith(oldElement);

            oldElement.attr("id", "CellphonesFile");

        }
    });

}

//根据输入的优惠券类型编号，查询优惠券的名称
//$("#selPromotionRuleId").on("input", function () {
//    $.post("/Promotion/GetPromotionRuleName", { "promotionRuleId": $("#selPromotionRuleId").val() }, function (data) {
//        $("#promotionType").html(data);
//    }, "HTML");
//});

//查看pid
$("#lookPid").click(function () {
    if (!document.getElementById("productListInfo").files[0]) {
        alert("请选择上传的文件！");
        return;
    }
    var ddd = new FileReader();
    ddd.readAsText(document.getElementById("productListInfo").files[0]);
    ddd.onload = function (aa) {
        var xxx = aa.target.result;
        $("#lookPidsDialog").html(xxx);
        lookPidDialog.dialog("open");
    }
});
var lookPidDialog = $("#lookPidsDialog").dialog({
    autoOpen: false,
    height: 'auto',
    width: 700,
    modal: true
});
//添加优惠券类型Dialog
var dialog = $("#couponTypeDialog").dialog({
    autoOpen: false,
    resizable: false,
    height: 'auto',
    width: 700,
    modal: true,
    buttons: {
        "保存": function () {
            //判断使用开始时间和使用结束时间
            var validateResult = validateAddCouponsType();
            if (validateResult) {
                var elementTitle = "编号:";
                var elementName = "couponDIv" + $("#couponsDiv").children().length;
                var ruleId = $("#selPromotionRuleId").val().trim();
                var couponsDescription = $("#PromotionDescription").val().trim();
                var useStartTime = $("#UseStartTime").val().trim();
                var useEndTime = $("#UseEndTime").val().trim();
                var useMoney = $("#UseMoney").val().trim();
                var discountMoney = $("#DiscountMoney").val().trim();
                var number = $("#Number").val().trim();
                var isRemind = $("#IsRemind:checked").val() || 0;
                var isPush = $("#IsPush:checked").val() || 0;
                var pushSetting = '';
                if (isPush == 1) {
                    pushSetting = $("input[name=PushSetting]").val();
                }
                //var financeMarkId = $("#selFinanceMarkId").val().trim();
                var departMentId = $("#DepartMentId").val().trim();
                var intentionId = $("#IntentionId").val().trim();
                var businessLineId = $("#BusinessLineId").val().trim();
                //添加优惠券类型信息to db 
                var param = {
                    CouponRulesId: ruleId,
                    PromotionDescription: couponsDescription,
                    StartTime: useStartTime,
                    EndTime: useEndTime,
                    MinMoney: useMoney,
                    DiscountMoney: discountMoney,
                    Number: number,
                    IsRemind: isRemind,
                    IsPush: isPush,
                    PushSetting:pushSetting,
                    //FinanceMarkId: financeMarkId,
                    DepartmentId: departMentId,
                    IntentionId: intentionId,
                    BusinessLineId: businessLineId
                };

                $.ajax({
                    type: 'POST',
                    url: '/Promotion/CreateOrUpdatePromotionTaskPromotionList',
                    dataType: 'json',
                    data: JSON.stringify(param),
                    contentType: "application/json; charset=utf-8",
                    success: function (jsonObj) {
                        if (!jsonObj.IsSuccess) {
                            alert(jsonObj.OutMessage);
                            return;
                        }
                        //alert(jsonObj.OutMessage);
                        //location.reload();
                        //创建优惠券控件，添加到优惠券列表中
                        var ids = $("#TaskPromotionListIds").val();
                        if (ids.length == 0) {
                            $("#TaskPromotionListIds").val(jsonObj.ObjectData);
                        } else {
                            $("#TaskPromotionListIds").val(ids + ',' + jsonObj.ObjectData);
                        }

                        var couponsAttributes = [["TaskPromotionListId", jsonObj.ObjectData], ["PromotionRuleId", ruleId], ["CouponsDescription", couponsDescription], ["UseStartTime", useStartTime], ["UseEndTime", useEndTime],
                            ["UseMoney", useMoney], ["DiscountMoney", discountMoney], ["DepartmentId", departMentId], ["IntentionId", intentionId], ["Number",number]];
                        var couponDiv = createElement(elementTitle, elementName, ruleId, couponsAttributes);
                        $("#couponsDiv").append(couponDiv);


                        if ($("#couponsDiv").height() > $("#couponsDiv").prev().height())
                            $("#btnAddCouponsType").css("margin-left", "110px");

                        dialog.dialog("close");
                    }
                });


            }

        },
        "取消": function () {
            dialog.dialog("close");
        }
    },
    close: function () {
        dialog.dialog("close");
    }
});

//添加优惠券类型
$("#btnAddCouponsType").click(function () {

    //重置新增表单数据
    //$("#couponTypeDialog input[type='text']").each(function () {
    //    $(this).val("");
    //});
    $("#selectRuleId").val("");
    $("#selPromotionRuleId").val("");
    $("#PromotionDescription").val("");
    $("#UseStartTime").val("");
    $("#UseEndTime").val("");
    $("#UseMoney").val("");
    $("#DiscountMoney").val("");
    $("#DepartMentId").val("");
    $("#IntentionId").val("");
    $("#BusinessLineId").val("");
    $("#promotionType").html("");
    $("#Number").val("");
    $("#IsRemind").prop("checked", false);
    $("#IsPush").prop("checked", false);
    $("#PushSetting").hide();
    $("input[name=PushSetting]").val("");
    dialog.dialog("open");
});

var orderChannel = $("#orderChannelDetailDialog").dialog({
    autoOpen: false,
    height: 'auto',
    width: 700,
    modal: true
});

$("#btnViewOrderChanne").click(function () {
    var text = $("#selectOrderChannel").find("option:selected").text();
    $.ajax({
        url: "/promotion/OrderChannelDetail?channel=" + text,
        type: "GET",
        success: function(data) {
            $("#orderChannelDetailDialog_Body").html(data);
            orderChannel.dialog("open");
        }
    });
});
//验证添加优惠券表单
function validateAddCouponsType() {
    var ruleId = $("#selPromotionRuleId").val().trim();
    if (ruleId == "" || ruleId == null) {
        alert("请输入优惠券RuleID");
        return false;
    }

    var couponsDescription = $("#PromotionDescription").val().trim();
    if (couponsDescription == "" || couponsDescription == null) {
        alert("请输入优惠券描述");
        return false;
    }

    var useStartTime = $("#UseStartTime").val().trim();
    if (useStartTime == "" || useStartTime == null) {
        alert("请输入使用开始时间");
        return false;
    }

    var useEndTime = $("#UseEndTime").val().trim();
    if (useEndTime == "" || useEndTime == null) {
        alert("请输入使用结束时间");
        return false;
    }

    if (Date.parse(useStartTime) > Date.parse(useEndTime)) {
        alert("使用开始时间不能大于使用结束时间");
        return false;
    }

    var useMoney = $("#UseMoney").val().trim();
    if (useMoney == "" || useMoney == null) {
        alert("使用金额不能为空");
        return false;
    }
    else if (isNaN(useMoney)) {
        alert("使用金额格式错误，必须为数字");
        return false;
    }

    var discountMoney = $("#DiscountMoney").val().trim();
    if (discountMoney == "" || discountMoney == null) {
        alert("优惠金额不能为空");
        return false;
    }
    else if (isNaN(discountMoney)) {
        alert("优惠金额格式错误，必须为数字");
        return false;
    }

    if (parseFloat(discountMoney) > parseFloat(useMoney)) {
        alert("使用金额不能大于优惠金额");
        return false;
    }

    var isPush = $("#IsPush:checked").val() || 0;
    if (isPush == 1) {
        var pushSetting = $("input[name=PushSetting]").val().trim();
        if (pushSetting == "") {
            alert("提前提醒的天数必须填写");
            return false;
        }
    }

    var DepartMentId = $("#DepartMentId").val().trim();
    if (DepartMentId == "" || DepartMentId == null) {
        alert("请选择部门");
        return false;
    }
    var IntentionId = $("#IntentionId").val()==null? "" : $("#IntentionId").val() .trim();
    if (IntentionId == "" || IntentionId == null) {
        alert("请选择用途");
        return false;
    }
    var Number = $("#Number").val() == null ? "" : $("#Number").val().trim();
    if (Number == "" || Number == null || Number > 99 || Number<=0)
    {
        alert("请输入正确的数量");
        return false;
    }
    return true;
}

//选择人工筛选，则禁用上传控件
$("#ConditionSelect").click(function () {
    //隐藏并禁用上传控件
    $("[name=CellphonesFile]").prop("disabled", true);
    $("[name=CellphonesFile]").hide();
    $("#uploadFileDiv").hide();


    //显示并启用过滤条件
    $("#UploadFile").removeAttr('checked');
    $("#ConditionSelect").attr('checked', 'checked');
    //$("#filterDiv").removeProp("disabled");
    $("#filterDiv").show();
    if ($("#TaskEndTime").val() != "")
        $("#TaskEndTime").change();
    if ($("#TaskStartTime").val() != "")
        $("#TaskStartTime").change();
    //控制页面元素显示状态
    isShow();
});
//选择上传文件，则启用上传控件
$("#UploadFile").click(function () {
    //显示并启用上传控件
    $("[name=CellphonesFile]").removeProp("disabled");
    $("[name=CellphonesFile]").show();
    $("#uploadFileDiv").show();

    //隐藏并禁用过滤条件
    $("#ConditionSelect").removeAttr('checked');
    $("#UploadFile").attr('checked', 'checked');
    //$("#filterDiv").prop("disabled", true);
    $("#filterDiv").hide();
    if ($("#TaskEndTime").val() != "")
        $("#TaskEndTime").change();
    if ($("#TaskStartTime").val() != "")
        $("#TaskStartTime").change();
});



/*
验证、提交创建优惠券任务Form表单
*/
$(document.forms["PromotionForm"]).validate({
    rules: {
        TaskPromotionListIds: "required",
        TaskName:
        {
            required: true
        },
        TaskStartTime: {
            required: true,
            date: true
        },
        TaskEndTime: {
            required: true,
            date: true
        },
        ExecPeriod: {
            required: true,
            number: true
        },
        CellphonesFile: {
            required: true
        }
    },
    ignore: ":hidden",
    messages: {
        TaskPromotionListIds: "请添加优惠券类型信息",
        TaskName: "请输入任务名称",
        TaskStartTime: {
            required: "请选择开始日期",
            date: "格式不正确"
        },
        TaskEndTime: {
            required: "请选择结束日期",
            date: "格式不正确"
        },
        ExecPeriod: "请输入执行周期"
    },
    submitHandler: function (form) {

        //判断优惠券是否设置
        var ids = $("#TaskPromotionListIds").val();
        if (ids.length <= 0) {
            alert("请添加优惠券类型信息！");
            return;
        }
        if ($("input[name=SendSMSType]:checked")[0].value == 1) {
            if ($("#smsId").val().trim() == '') {
                alert("请输入短信模板id");
                return;
            }
        } else {
            $("#smsId").val('');
            $("#smsParam").val('');
        }
        //优惠金额和最少使用金额的判断
        if (parseFloat($("[id=DiscountMoney]").val()) > parseFloat($("[id=UseMoney]").val(), 2)) {
            alert("优惠金额不能大于最少使用金额！");
            return;
        }

        //任务开始时间和任务结束时间的判断
        if (Date.parse($("#TaskStartTime").val()) > Date.parse($("#TaskEndTime").val())) {
            alert("任务开始时间不能大于任务结束时间！");
            return;
        }

        if ($("input[name='SelectUserType']:checked")[0].value == 2 &&
            !validateUserChoose())
            return;
        if ($("input[name='SelectUserType']:checked")[0].value == 3 && !validateTaskActivityChoose())
            return;
        form = $(form);

        if (confirm("确定执行该动作？")) {
            $("#btnSave").attr("disabled", "disabled").next().css("visibility", "visible");
            var success = function (result) {
                $("#btnSave").removeAttr("disabled").next().css("visibility", "hidden");

                if (result.Result >= 0) {
                    alert("操作成功！");
                    location.reload();
                } else
                    switch (result.Result) {
                        case -1:
                            alert("创建券失败！");
                            break;
                        case -2:
                            alert("开始日期不能大于结束日期！");
                            break;
                        case -5:
                            alert("没有有效的手机号！");
                            break;
                        case -6:
                            alert("含有非法手机号！");
                            break;
                        case -7:
                            alert("短信模板参数格式错误");
                            break;
                        default:
                            alert("网络异常，请联系管理员！");
                            break;
                    }
            }
            form.ajaxSubmit({
                error: function (ajax) {
                    $("#btnSave").removeAttr("disabled").next().css("visibility", "hidden");
                    if (ajax.status == 200)
                        success(JSON.parse(ajax.responseText));
                    else if (ajax.status > 0) {
                        alert("创建券失败！");
                    }
                },
                success: success
            });
        }
    }
});

function validateTaskActivityChoose() {
    if ($("#sel_biActivity").val() <= 0) {
        alert("请选择一个活动数据");
        return false;
    }
    return true;
}

/*
验证用户筛选内容
*/
function validateUserChoose() {
    if ($("#selectedFilterDiv #ordersTime").length == 0) {
        alert("必须输入订单时间范围！");
        return false;
    }

    if ($("#selectedFilterDiv #OrderStatus").length == 0
    ) {
        alert("必选订单状态！");
        return false;
    }

    if ($("#selectedFilterDiv #SpendMoney").length == 0 && $("#selectedFilterDiv #PurchaseNum").length == 0) {
        alert("产品购买金额和产品购买数量至必须至少选择一个");
    }


    return true;
}

/*
创建过滤条件控件
*/
function createElement(title, elementName, elementValue, attributes) {

    //已经添加过此类型的过滤条件，则进行内容替换
    if ($("#" + elementName).length > 0) {

        $("#" + elementName).find("#filterValue").html(elementValue);
        return;
    }

    var divHtml = '<div id="' + elementName + '" name="' + elementName + '"';
    if (attributes == null || attributes.length == 0) {
        divHtml += '/>';
    }
    else {

        for (var i = 0; i < attributes.length; i++) {
            var attribute = attributes[i];
            var attributeName = attributes[i][0];
            var attributeValue = attributes[i][1];
            divHtml += ' ' + attributeName + '="' + attributeValue + '"';
        }
        divHtml += ' />';
    }

    var divElement = $(divHtml);
    var labelTitle = $('<label style="cursor: pointer; " >' + title + '</label>');
    var labelElement = $('<label id="filterValue" style="cursor: pointer; ">' + elementValue + '</label>');

    var aElement = $('<a href="#">X</a>');
    aElement.click(function () {
        removeElementFromParent(elementName);
    });

    labelTitle.click(function () {
        dialogShowRuleInfo(elementName);
    });
    labelElement.click(function () {
        dialogShowRuleInfo(elementName);
    });

    divElement.addClass("selectedFilterValue");
    divElement.append(labelTitle);
    divElement.append(labelElement);
    divElement.append(aElement);

    return divElement;
}

function dialogShowRuleInfo(elementName) {
    var element = $("#" + elementName);
    //alert(element.attr("taskpromotionlistid"));
    $("#couponTypeDialog_ReadOnly").dialog({
        //autoOpen: false,
        resizable: false,
        height: 'auto',
        width: 700,
        modal: true
    });

    var param = {
        taskPromotionListId: element.attr("taskpromotionlistid"),
        couponRulesId: element.attr("promotionruleid")
    };
    $.ajax({
        type: 'POST',
        url: '/Promotion/SelectPromotionTaskPromotionListById',
        dataType: 'json',
        data: JSON.stringify(param),
        contentType: "application/json; charset=utf-8",
        beforeSend: function () {
            $("#couponTypeDialog_ReadOnly_title").show();
            $("#couponTypeDialog_ReadOnly_detail").hide();
        },
        success: function (jsonObj) {
            if (jsonObj.IsSuccess) {
                $("#lblNumber").text(jsonObj.ObjectData.Number);
                $("#lblDepartMentId").text(jsonObj.ObjectData.DepartmentName);
                $("#lblIntentionId").text(jsonObj.ObjectData.IntentionName);
                $("#lblFinanceMarkType").text(jsonObj.ObjectData.FinanceMarkName);
                $("#lblSelPromotionRuleId").text(jsonObj.ObjectData.CouponRulesId);
                $("#lblPromotionType").text(jsonObj.ObjectData.RuleName);
                $("#lblPromotionDescription").text(jsonObj.ObjectData.PromotionDescription);
                $("#lblUseStartTime").text(jsonObj.ObjectData.StartTime);
                $("#lblUseEndTime").text(jsonObj.ObjectData.EndTime);
                $("#lblUseMoney").text(jsonObj.ObjectData.MinMoney);
                $("#lblDiscountMoney").text(jsonObj.ObjectData.DiscountMoney);
                $("#lblBusinessLineId").text(jsonObj.ObjectData.BusinessLineName||"");
                $("#lblIsRemind").text(jsonObj.ObjectData.IsRemind === 1 ? "是" : "否");
                $("#lblIsPush").text(jsonObj.ObjectData.IsPush === 1 ? "是，提前("+jsonObj.ObjectData.PushSetting+")天" : "否");
                $("#couponTypeDialog_ReadOnly_detail").show();
                $("#couponTypeDialog_ReadOnly_title").hide();
                return;
            }
        }
    });
}

//删除重复任务过滤条件Pid_PID：TR-DP-PT3|12
function RemoveFilterItem(elementName) {
    var element = $("#selectedFilterDiv #" + elementName.replace(/(:|\.|\/|\\|\|)/g, '\\$1'));
    element.remove();
    //拼接每个过滤条件的id属性
    var pos = elementName.lastIndexOf("_");

    //单选项
    if (pos <= 0) {
        $("#" + elementName).val("");
    }
    else {//多选项
        var name = elementName.substring(0, pos);
        $("#" + name).val(GetValueStrById(name + "_", name + "Id"));
    }
}

/*
移除过滤条件控件
*/
function removeElementFromParent(elementName) {
    var element = $("#" + elementName.replace(/\//g, '\\/'));
    element.remove();

    var ids = resetTaskPromotionListIds();
    $("#TaskPromotionListIds").val(ids);

    var param = {
        taskPromotionListId: element.attr("taskpromotionlistid")
    };

    $.ajax({
        type: 'POST',
        url: '/Promotion/DelPromotionTaskPromotionListById',
        dataType: 'json',
        data: JSON.stringify(param),
        contentType: "application/json; charset=utf-8",
        success: function (jsonObj) {
            if (!jsonObj.IsSuccess) {
                alert(jsonObj.OutMessage);
                return;
            }
        }
    });
}

function resetTaskPromotionListIds() {
    var str = "";
    $("#couponsDiv div").each(function (i) {
        if (str.length == 0) {
            str = $(this).attr("taskpromotionlistid");
        } else {
            str += ',' + $(this).attr("taskpromotionlistid");
        }
    });
    return str;
}


/*
车型信息
*/
function LoadBrand() {
    $.get("GetVehicleBrands", function (data) {
        if (data.status === "success") {
            //var html = "<option value=''>-选择品牌-</option>";
            var html = "";
            $(data.data).each(function (index) {
                html += "<option value='" + data.data[index] + "'>" + data.data[index] + "</option>";
            });
            $("#selectSeableInfo").empty();
            $("#selectSeableInfo").append(html);
            BrandChange(data.data[0]);
        } else {
            //var html = "<option value=''>无数据</option>";
            var html = "";
            $("#selectSeableInfo").empty();
            $("#selectSeableInfo").append(html);
            BrandChange("");
        }
    });
}

function BrandChange(brand) {
    $.get("GetVehicleSeries", { brand: brand }, function (data) {
        if (data.status === "success") {
            var html = "";
            for (var item in data.data) {
                if (data.data.hasOwnProperty(item)) {
                    html += "<option value='" + item + "'>" + data.data[item] + "</option>";
                }
            }
            $("#selectVehicleInfo").empty();
            $("#selectVehicleInfo").append(html);

        } else {
            var html = "";
            if (brand == "") {
                html = "<option value=''>全部</option>";
            } else {
                html = "";
            }
            $("#selectVehicleInfo").empty();
            $("#selectVehicleInfo").append(html);

        }
    });
}

$(function () {

    if (IsLimitOnce == 1) {
        $("#notlimitOnce").removeAttr("checked");
        $("#limitOnce").attr("checked", 'checked');
    }
    else {
        $("#limitOnce").removeAttr("checked");
        $("#notlimitOnce").attr("checked", 'checked');
    }
    //init tasktype
    if (taskType == 2) {
        $("#divRepeatTask").show();
        $("input[name=TaskType]:eq(1)").attr("checked", 'checked');
    }

    $("#filterDiv").hide();
    $("#uploadFileDiv").hide();
    $("#filterBIDev").hide();
    if (selectUserType == 2) {
        $("#filterDiv").show();
        $("input[name=SelectUserType]:eq(1)").attr("checked", 'checked');
    }else if (selectUserType == 3) {
        $("#filterBIDev").show();
        $("input[name=SelectUserType]:eq(2)").attr("checked", 'checked');
    }

    $("input[name=TaskType]").click(function () {
        var selValue = $(this).val();
        if (selValue == 1) {
            $("#divRepeatTask").hide();
        } else {
            $("#divRepeatTask").show();
        }
        //控制页面元素显示状态
        isShow();
    });
    $("input[name=SendSMSType]").click(function() {
        var selValue = $(this).val();
        if (selValue == 1) {
            $("#divSms").show();
        } else {
            $("#divSms").hide();
        }
    });
    $("input[name=SelectUserType]").click(function () {
        var selValue = $(this).val();
        $("#filterDiv").hide();
        $("#uploadFileDiv").hide();
        $("#filterBIDev").hide();
        if (selValue == 1) {
            $("#uploadFileDiv").show();
        } else if(selValue==2){
            $("#filterDiv").show();
        }
        else if (selValue == 3) {
            $("#filterBIDev").show();
        }
    });
    $("#sel_biActivity").change(function() {
        var des = $(this).find("option:selected").data("description");
        $("#biActivityDescription").text(des);
        var val = $(this).val();
        if (val > 0) {
            $.ajax({
                type: "POST",
                url: "/Promotion/GetPromotionTaskActivityUsersCount",
                data: {
                    promotionTaskActivityId:val
                },
                dataType: "JSON",
                success: function(data) {
                    if (data.Code == 1) {
                        $("#biActivityUsersCount").text("总共" + data.Count + "个用户");
                    }
                }
            })
        } else {
            $("#biActivityUsersCount").text("");
        }
    });
    //控制页面元素显示状态
    isShow();
});
//控制页面元素显示状态
function isShow() {
    var taskType = $('input[type="radio"][name="TaskType"]:checked').val();
    //订单的产品类型显示
    if (taskType === "2") {
        $("#divProductType").show();
    }else {
        $("#divProductType").hide();
    }
}