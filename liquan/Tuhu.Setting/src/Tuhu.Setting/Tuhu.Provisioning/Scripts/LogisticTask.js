$(function () {
    //判断数组是否含某一元素
    Array.prototype.in_array = function (e) {
        for (var i = 0; i < this.length && this[i] != e; i++);
        return !(i == this.length);
    }
});
//所搜button事件
function btnSearch(type) {
    var sb = "";
    var SourceWareHouseID = $("#SourceWareHouseID").val();
    var TargetWareHouseID = $("#TargetWareHouseID").val();
    if ($("#SourceWareHouseID").val() == "") {
        //如果是请选择，那么赋值-1，程序里面会判断，如果是-1那么搜全部
        SourceWareHouseID = null;
    }
    if ($("#TargetWareHouseID").val() == "") {
        TargetWareHouseID = null;
    }
    var DeliveryType = $("#DeliveryType").val();
    var LogisticTaskType = $("#LogisticTaskType").find("option:selected").text();
    //var TaskStatus = $("#LogisticTaskStatus").val();
    var TaskStatus = "";
    var tbody = $("#tbody");
    switch (type) {
        case "新建":
            TaskStatus = "0New";
            break;
        case "修改中":
            TaskStatus = "6Modifying";
            break;
        case "已接受":
            TaskStatus = "1Accepted";
            break;
        case "已备货":
            TaskStatus = "2Prepared";
            break;
        case "已发出":
            TaskStatus = "3Sent";
            break;
        case "已收货":
            TaskStatus = "4Received";
            break;
        case "已取消":
            TaskStatus = "5Canceled";
            break;
    }
    //alert(TaskStatus);
    GetLosigticTaskList(SourceWareHouseID, TargetWareHouseID, DeliveryType, LogisticTaskType, TaskStatus, tbody);
}

//得到物流任务
function GetLosigticTaskList(SourceWareHouseID, TargetWareHouseID, DeliveryType, LogisticTaskType, TaskStatus, tbody) {

    $.ajax({
        type: "GET",
        url: "/Logistic/GetLosigticTaskList",
        data: { "SourceWareHouseID": SourceWareHouseID, "TargetWareHouseID": TargetWareHouseID, "DeliveryType": DeliveryType, "LogisticTaskType": LogisticTaskType, "TaskStatus": TaskStatus },
        success: function (result) {
            displayLosigticTaskList(result, tbody, null, null);
        }
    });
}

//将返回的结果显示在tbody里面
function displayLosigticTaskList(result, tbody, tr, tr1) {
    if (result.length != 0) {
        var sb = "";
        var sb_tr = "";
        var sb_tr1 = "";

        for (var i = 0; i < result.length; i++) {
            //alert(result);
            var sb_task = "";
            var item = result[i];
            //var sb_checkbox = "<td><input type='checkbox' /></td>";
            var sb_PKID = "<td><a herf='/Logistic/Details/" + item.PKID + "' style='text-decoration: underline;'>" + item.PKID + "</a></td>";
            var sb_SourceWareHouse = "<td>" + item.SourceWareHouse + "</td>";
            var sb_TargetWareHouse = "<td>" + item.TargetWareHouse + "</td>";
            var sb_LogisticTaskType = "<td>" + item.LogisticTaskType + "</td>";
            var sb_TaskStatus = "<td>" + item.TaskStatus + "</td>";
            var sb_DeliveryType = "<td>" + item.DeliveryType + "</td>";
            var sb_DeliveryCompany = "<td></td>";
            if (item.DeliveryCompany != null) {
                var sb_DeliveryCompany = "<td>" + item.DeliveryCompany + "</td>";
            }
            var sb_DeliveryCode = "<td></td>";
            if (item.DeliveryCode != null) {
                sb_DeliveryCode = "<td>" + item.DeliveryCode + "</td>";
            }
            var sb_DeliveryFee = "<td></td>";

            if (item.DeliveryFee != null) {
                sb_DeliveryFee = "<td>" + item.DeliveryFee + "</td>";
            }
            var date = item.ArrivalDate.toString().match(/\d+/)[0];
            date = Number(date);
            date = new Date(date).Format("MM-dd hh:mm");
            var sb_ArriveTime = "<td>" + date + "</td>";
            if (item.Remark == null) {
                item.Remark = "";
            }
            var sb_Remark = "<td><input type='text' value=" + item.Remark + " ></td>";
            var sb_td = "<td colspan='6'></td>";
            var sb_Accepted = "";
            var sb_Sent = "";
            var sb_Received = "";
            var sb_Cancel = "";
            var sb_Edit = "";
            var sb_action = "";

            if (item.TaskStatus == "新建") {
                sb_Accepted = "<input class='SingleAccepted' type='button' value='接受'>";
            }
            if (item.TaskStatus == "已备货") {
                sb_Sent = "<input class='AllSent' type='button' value='发出'>";
            }
            if (item.TaskStatus == "已发出") {
                sb_Received = "<input class='AllReceived' type='button' value='全部收货'>";
            }
            if (item.TaskStatus != "已取消" && item.TaskStatus != "已发出" && item.TaskStatus != "已收货") {
                sb_Cancel = "<input class='SingleCanceled' type='button' value='取消'>";
            }
            if (item.TaskStatus == "新建" || item.TaskStatus == "修改中") {
                sb_Edit = "<input class='AllEdit' type='button' value='修改'>";
            }
            sb_action = '<td>' + sb_Accepted + " " + sb_Sent + " " + sb_Received + " " + sb_Edit + " " + sb_Cancel + '</td>';
            //sb += '<tr>' + sb_checkbox + sb_PKID + sb_SourceWareHouse + sb_TargetWareHouse + sb_LogisticTaskType + sb_TaskStatus + sb_DeliveryType + sb_ArriveTime + +sb_td + sb_td + sb_td + sb_action + '</tr>';
            sb_task = '<tr id=' + item.PKID + ' style="background-color:#F0F0F0;font-weight:bold;">' + sb_PKID + sb_SourceWareHouse + sb_TargetWareHouse + sb_LogisticTaskType + sb_TaskStatus + sb_DeliveryType + sb_DeliveryCompany + sb_DeliveryCode + sb_DeliveryFee + sb_ArriveTime + sb_td + sb_Remark + sb_action + '</tr>';
            var sb_trclass = "<tr class=" + item.PKID + "><td colspan='18'></td></tr>";
            var sb_pro_td = "<td colspan='9'></td>";
            var sb_pro_sum = "";
            for (var j = 0; j < item.tLTPList.length; j++) {
                var sb_pro = "";
                var proItem = item.tLTPList[j];
                var sb_pro_PKID = '<td> <input type="hidden" value=' + proItem.PKID + ' /><a class="ShowStockHistory" style="text-decoration: underline">' + proItem.PKID + '</a></td>';
                var sb_pro_PID = "<td>" + proItem.PID + "</td>";
                var proItemName = proItem.Name;
                if (proItem.Name.length > 10) {
                    var proItemName = proItem.Name.substring(0, 10) + "...";
                }
                var sb_pro_Name = "<td Title='" + proItem.Name + "'>" + proItemName + "</td>";
                var sb_pro_Num = "<td style='color:Blue'>" + proItem.Num + "</td>";
                var sb_pro_ReceivedNum;
                var sb_pro_NeedNum;
                //var sb_WeekYear = "<td><input type='text' value='周期' style='color:Gray'></td>";
                if (proItem.WeekYear == null) {
                    var sb_WeekYear = "<td><input type='text' name='WeekYear' style='color:Gray;width:70px'></td>";
                }
                else {
                    var sb_WeekYear = "<td><input type='text' name='WeekYear' value=" + proItem.WeekYear + " style='color:Gray;width:70px'></td>";
                }
                if (proItem.ReceivedNum != null) {
                    sb_pro_ReceivedNum = "<td style='color:Green'>" + proItem.ReceivedNum + "</td>";
                    if (item.TaskStatus == "已发出") {

                        sb_pro_NeedNum = "<td><input type='text' value=" + (proItem.Num - proItem.ReceivedNum) + " style='Width:50px;color:Red'></td>";
                    }
                    else {
                        sb_pro_NeedNum = "<td><input type='text' value=" + (proItem.Num - proItem.ReceivedNum) + " style='Width:50px;color:Red' disabled='disabled'></td>"
                    }
                }
                else {
                    sb_pro_ReceivedNum = "<td></td>";
                    if (item.TaskStatus == "已发出") {
                        sb_pro_NeedNum = "<td><input type='text' value=" + proItem.Num + " style='Width:50px;color:Red'></td>"
                    }
                    else {
                        sb_pro_NeedNum = "<td><input type='text' value=" + proItem.Num + " style='Width:50px;color:Red' disabled='disabled'></td>"
                    }

                }
                if (proItem.Remark == null) {
                    proItem.Remark = "";
                }
                var sb_pro_Remark = '<td><input type="text"  style="color:Gray" value=' + proItem.Remark + '></td>';

                //var sb_pro_Remark = "<td><input type='text' value='备注' style='color:Gray'/></td>";
                var sb_pro_Received = "";
                if (item.TaskStatus == "已发出" && (proItem.Num - proItem.ReceivedNum) != 0) {
                    sb_pro_Received = "<td><input class='SeparateReceived' type='button' value='收货'></td>"
                }
                if (item.TaskStatus == "已收货") {
                    sb_pro_Received = "<td><a class='ShowHistory' style=''>收货记录</a></td>"
                }
                else {
                    sb_pro_Received = "<td></td>"
                }
                sb_pro = "<tr class=" + item.PKID + ">" + sb_pro_PKID + sb_pro_td + sb_pro_Name + sb_pro_PID + sb_pro_Num + sb_pro_ReceivedNum + sb_pro_NeedNum + sb_WeekYear + sb_pro_Remark + sb_pro_Received + "</tr>";
                sb_pro_sum += sb_pro;

            }
            sb_tr += sb_task;
            //sb_pro += sb_pro;
            sb += sb_task + sb_pro_sum + sb_trclass;
        }


    }
        //如果没有产品列表，那么清空
    else {
        sb_task = "";
        sb_pro = "";
        sb = "";
    }
    if (tbody != null) {
        tbody.html(sb);
    }
    if (tr != null) {
        alert(sb_tr);
        alert(tr.html());
        var string = $(tr).html();
        $(tr).html(sb_tr);
        $(tr1).html(sb_tr1);
    }
}

//表单提交时需要验证的方法
function formSubmit() {
    var result = true;
    var DeliveryTypeResult = true;
    var SourceWareHouseIDResult = true;
    var TargetWareHouseIDResult = true;
    var inputResult1 = true;
    var productResult = true;
    var DeliveryCodeResult = true;
    //判断必填项
    if ($("#SourceWareHouseID").val() == "") {
        $("#SourceWareHouseID").next().next().html("//源仓库必填");
        return false;
    }
    if ($("#TargetWareHouseID").val() == "") {
        $("#TargetWareHouseID").next().next().html("//目标仓库必填");
        return false;
    }

    if ($("#SourceWareHouseID").val() == $("#TargetWareHouseID").val() && $("#ShopId").val() == "") {
        $("#ShopId").append("请选择门店！");
        return false;
    }

    // 判断快递费用是否为空
    if ($("#DeliveryFee").val() == "") {
        $("#DeliveryFee").css("border", "1px Solid Red");
        $("#DeliveryFee").next().html("快递费用不能为空");
    } else {
        $("#DeliveryFee").css("border", "");
        $("#DeliveryFee").next().html("*");
    }

    //判断快递单号格式是否正确
    var DeliveryCodeResult = ValidateDeliveryCode($("#DeliveryCode"));
    if (!DeliveryCodeResult) {
        $("#DeliveryCode").css("border", "1px Solid Red");
        $("#DeliveryCode").next().html("快递单号格式不正确：每个快递单号长度为5-15位数字或字母，多个单号用英文逗号(,)连接");
    }
    else {
        $("#DeliveryCode").css("border", "");
        $("#DeliveryCode").next().html("例如：11111,11111");
    }
    //    if ($("#DeliveryCode").val() != "") {
    //        var reg = /^[0-9A-Za-z\-]{5,15}(,[0-9A-Za-z\-]{5,15})*$/;
    //        if (!reg.test($("#DeliveryCode").val())) {
    //            DeliveryCodeResult = false;
    //            $("#DeliveryCode").css("border", "1px Solid Red");
    //            $("#DeliveryCode").next().html("快递单号格式不正确：每个快递单号长度为5-15位数字或字母，多个单号用英文逗号(,)连接");
    //        }
    //        else {
    //            $("#DeliveryCode").css("border", "");
    //            $("#DeliveryCode").next().html("例如：11111,11111");

    //        }

    //    }
    //判断是否添加产品
    if ($("#tbody").find("tr").length == 0) {
        alert("添加产品");
        productResult = false;
    }
    //验证数量的输入框，是否正常输入数字
    for (var i = 0; i < $("#tbody [name='RemoveNum']").length; i++) {
        if ($($("#tbody [name='RemoveNum']")[i]).val() == "") {
            if (inputResult1 = true) {
                inputResult1 = false;
                $($("#tbody [name='RemoveNum']")[i]).css("border", "1px Solid Red");
            }
        }
        else {
            var reg = /^(0|[1-9][0-9]*)$/;
            var inputResult = reg.test($($("#tbody [name='RemoveNum']")[i]).val());
            if (!inputResult) {
                $($("#tbody [name='RemoveNum']")[i]).css("border", "1px Solid Red");
                if (inputResult1 == true) {
                    inputResult1 = false;
                }
            }
        }
    }
    //判断选中的日期
    var dateResult = true;
    var selectDate = $("#ArrivalDate___Calendar").val();
    selectDate = new Date(selectDate).Format("yyyy-MM-dd");
    //alert(selectDate);

    //获得后天的日期
    var tomorrowAfterTime = new Date();
    tomorrowAfterTime.setDate(tomorrowAfterTime.getDate() + 2);
    tomorrowAfterTime = new Date(tomorrowAfterTime).Format("yyyy-MM-dd");
    if (selectDate < tomorrowAfterTime) {
        dateResult = false;
        $("#ArrivalDate___Calendar").css("border", "1px Solid Red");
        $("#DateError").css("display", "inline");
        $("#DateError").html("请选择大于" + tomorrowAfterTime + "的时间");
    }
    else {
        $("#ArrivalDate___Calendar").css("border", "");
        $("#DateError").css("display", "none");
    }

    result = DeliveryTypeResult && SourceWareHouseIDResult && TargetWareHouseIDResult && inputResult1 && productResult && DeliveryCodeResult && dateResult;
    return result;
}

//得到结果
//function GetAjax(PKID, oldPKID) {
//    var sb = "";
//    var pkid = PKID.split(',');
//    var locationId = document.getElementById("input_stockID").value;
//    var hiddenro = parent.document.getElementById("Hidden_ProductList");
//    $.ajax({
//        type: "GET",
//        data: { "PKID": PKID, "locationId": locationId, "oldPKID": oldPKID },
//        url: '/LogisticTask/LoadProductList',
//        success: function (result) {
//            var prouctListValue = hiddenro.value;
//            for (var i = 0; i < result.length; i++) {
//                var stockLocation = result[i];
//                if (hiddenro.value.indexOf(stockLocation.BatchId) >= 0) {
//                    alert("您已选过:" + stockLocation.PID + "批次为:" + stockLocation.BatchId + "的产品");
//                    continue;
//                } else {
//                    var sb_Name = '<td>' + stockLocation.Name + '</td>';
//                    var sb_PID = '<td>' + stockLocation.PID + '</td>';
//                    var sb_AvailableNum = '<td>' + stockLocation.Num + '</td>';
//                    var sb_input = '<td><input type="hidden"  name="batchIds" value="' + stockLocation.BatchId + '"/>' +
//                        '<input type="hidden" id="PID' + pkid[i] + '" name="PID" value="' + stockLocation.PID + '"/>' +
//                        '<input type="hidden" name="Name" value=' + stockLocation.Name + '/>' +
//                        '<input type="hidden" name="CostPrice" value="' + stockLocation.CostPrice + '"/>' +
//                        '<input type="text" style="width:80px;" name="RemoveNum" onblur="blurClick(this,' + pkid[i] + ');"/>' +
//                        '<input type="hidden" name="proPKID" value="0" /></td>';
//                    var sb_Delete = '<td><input class="deleteProList" type="button" value="删除" onclick="btnDelete(this)" /></td>';
//                    sb += '<tr>' + '' + sb_Name + '' + sb_PID + '' + sb_AvailableNum + '' + sb_input + '' + sb_Delete + '</tr>';
//                    prouctListValue += stockLocation.BatchId + ",";
//                    hiddenro.value = prouctListValue;
//                }
//            }
//            $("#tbody").append(sb);
//        }
//    });
//}

//数字验证
function blurClick(input, id) {
    //验证该输入是否是数字
    var reg = /^(0|[1-9][0-9]*)$/;
    var result = reg.test($(input).val());
    $(input).css("border", "");
    if (!result && $(input).val() != "") {
        $(input).css("border", "1px Solid Red");
        alert("请输入非零开头的整数");
        $(input).val("");

        return false;
    }
    var num = $($(input).parent().parent().find("td")[2]).html();
    if (($(input).val() - num) > 0) {

        $(input).css("border", "1px Solid Red");
        alert("该产品的可用数量是：" + num + ", 请输入少于该数量的数字");
        $(input).val("");
        return false;
    }
    if ($(input).val() == 0 && $(input).val() != "") {
        $(input).css("border", "1px Solid Red");
        alert("请输入大于0的数字");
        $(input).val("");
        return false;
    }

    var category;
    var sourceWarehouse = $("#SourceWareHouseID").val();
    var targetWarehouse = $("#TargetWareHouseID").val();
    var pid = $("#PID" + id).val();
    if (pid.substring(0, 3) == "TR-") {
        category = "TR-";
    }
    else if (pid.substring(0, 3) == "LG-") {
        category = "LG-";
    }
    var transferNum = $(input).val();

    $.ajax({
        url: "/LogisticTask/FetchTransferPrice",
        data: { "sourceWarehouse": sourceWarehouse, "targetWarehouse": targetWarehouse, "category": category, "num": transferNum },
        type: "Get",
        success: function (data) {
            var price = parseFloat(transferNum * data).toFixed(2);
            $("#transferPrice" + id).text(price);

        }
    });

}

//批量添加产品
//点击添加产品button事件
function btnBatchAdd() {
    var sb = "";
    //获得所有选中的checkbox
    var goodToAdd = document.getElementsByName("GoodToAdd");
    $(".disabled").removeAttr("disabled");
    $("#light").css("display", "none");
    var hiddenro = parent.document.getElementById("Hidden_ProductList");
    for (var i = 0; i < goodToAdd.length; i++) {
        if ($(goodToAdd[i]).attr("checked") == "checked") {
            var pkid = $(goodToAdd[i]).val().split('&')[0];
            var name = $(goodToAdd[i]).val().split('&')[1];
            var pid = $(goodToAdd[i]).val().split('&')[2];
            var num = $(goodToAdd[i]).val().split('&')[3];
            var str = pkid.split(";");
            for (var j = 0; j < str.length - 1; j++) {
                if (hiddenro.value.indexOf(str[j].split(":")[0]) >= 0) {
                    break;
                } else {
                    var sbName = '<td>' + name + '</td>';
                    var sbPid = '<td>' + pid + '</td>';
                    var sbAvailableNum = '<td>' + num + '</td>';
                    var sbInput = '<td><input type="hidden"  name="batchIds" value="' + pkid + '"/>' +
                        '<input type="hidden" id="PID' + pkid + '" name="PID" value="' + pid + '"/>' +
                        '<input type="hidden" name="Name" value="&quot;' + name + '&quot;"/>' +
                        '<input type="text" style="width:80px;" name="RemoveNum" onblur="blurClick(this,&quot;' + pkid + '&quot;);"/>' +
                        '<input type="hidden" name="proPKID" value="0" /></td>';
                    var sbDelete = '<td><input class="deleteProList" type="button" value="删除" onclick="btnDelete(this)" /></td>';
                    sb += '<tr>' + '' + sbName + '' + sbPid + '' + sbAvailableNum + '' + sbInput + '' + sbDelete + '</tr>';
                    hiddenro.value += str[j].split(":")[0] + ",";
                    break;
                }
            }
            
        }
    }
    $("#tbody").append(sb);
}

//单独添加产品
function btnAdd(pkid, name, pid, num) {
    var sb = "";
    var hiddenro = parent.document.getElementById("Hidden_ProductList");
    var str = pkid.split(";");
    for (var i = 0; i < str.length-1 ; i++) {
        if (hiddenro.value.indexOf(str[i].split(":")[0]) >= 0) {
            alert("您已选过该产品");
            break;
        } else {
            $(".disabled").removeAttr("disabled");
            $("#light").css("display", "none");
            var sbName = '<td>' + name + '</td>';
            var sbPid = '<td>' + pid + '</td>';
            var sbAvailableNum = '<td>' + num + '</td>';
            var sbInput = '<td><input type="hidden"  name="batchIds" value="' + pkid + '"/>' +
                '<input type="hidden" id="PID' + pkid + '" name="PID" value="' + pid + '"/>' +
                '<input type="hidden" name="Name" value="&quot;' + name + '&quot;"/>' +
                '<input type="text" style="width:80px;" name="RemoveNum" onblur="blurClick(this,&quot;' + pkid + '&quot;);"/>' +
                '<input type="hidden" name="proPKID" value="0" /></td>';
            var sbDelete = '<td><input class="deleteProList" type="button" value="删除" onclick="btnDelete(this)" /></td>';
            sb += '<tr>' + '' + sbName + '' + sbPid + '' + sbAvailableNum + '' + sbInput + '' + sbDelete + '</tr>';
            $("#tbody").append(sb);
            hiddenro.value += str[i].split(":")[0] + ",";
            break;
        }
    }
   
    
}

//关闭页面button事件
function CloseButton() {
    $('#light').css('display', 'none');
    $(".disabled").removeAttr("disabled");
}

//加产品事件
function AddProduct() {
    //仓库ID
    var SourceWareHouseID = $("#SourceWareHouseID").val();
    if (SourceWareHouseID == "") {
        alert("请选择源库存");
    }
    else {
        //先disabled在父窗口所有控件的值
        $(".disabled").attr("disabled", "disabled");
        $("#light").css("display", "block");
        $("#div_StockProductList").load('/LogisticTask/LoadStockProductList', { "SourceWareHouseID": SourceWareHouseID, "SearchString": "", "Type": "aClick" });
    }
}

//源仓库change事件
function SourceWareHouseChange() {
    var oldSourceWareHouseID = $("#SourceWareHouseID").val();
    //如果有产品列表，那么提示用户是否需要修改源仓库
    if ($("#tbody").find("tr").length != 0) {
        if (confirm("修改源库存会清空产品列表，确定修改吗?")) {
            $("#tbody").html("");
        }
        else {
            $("#SourceWareHouseID").val(oldSourceWareHouseID);
            //break;
        }
    }
    if ($("#SourceWareHouseID").val() == "") {
        $("#SourceWareHouseID").next().next().html("//源仓库必填");
    }
    else {
        //重新加载目标仓库的值
        $("#ShopSpan").empty();
        $("#toStore").hide();
        var option = "<option>请选择</option>";
        $.get("/LogisticTask/GetStockList", { "SelectWareHouseID": $("#SourceWareHouseID").val() }, function (result) {
            for (var i = 0; i < result.length; i++) {
                //alert(result);
                option += "<option value=" + result[i].PKID + ">" + result[i].SimpleName + "</option>";
            }
            $("#TargetWareHouseID").html(option);
            $("#SourceWareHouse").val($("#SourceWareHouseID").find("option:selected").text());
            $("#SourceWareHouseID").next().next().html("*");
        });
    }
}

//目标仓库change事件
function TargetWareHouseChange() {
    $("#ShopSpan").empty();
    $("#toStore").hide();
    if ($("#TargetWareHouseID").val() == "请选择") {
        $("#TargetWareHouseID").next().next().html("//目标仓库必填");
    }
    else {
        var targetName = $("#TargetWareHouseID").val();
        var sourceName = $("#SourceWareHouseID").val();
        $("#TargetWareHouse").val(targetName);
        $("#TargetWareHouseID").next().next().html("*");
        var ss = "";
        ss = "移到门店(请谨慎选择):<select id='ShopId' name='ShopId'><option>请选择</option>";
        var efectNum = 0;
        $.get("/LogisticTask/LoadStoreShop?sourceName=" + sourceName + "&targetName=" + targetName, function (result) {
            $.each(result, function (index, dom) {
                ss = ss + "<option value=" + index + ">" + dom + "</option>";
                efectNum++;
            });
            ss = ss + "</select>";
            if (efectNum>0) {
                $("#ShopSpan").html(ss);
                $("#toStore").show();
            }
           
        });
    }
    //获得源仓库的值
    var SourceName = $("#SourceWareHouseID").val();
    var TargetName = $("#TargetWareHouseID").val();
    $.ajax({
        url: "/LogisticTask/FetchTransferExpectSentDate",
        data: { "sourceWarehouse": SourceName, "targetWarehouse": TargetName },
        type: "Get",
        success: function (data) {
            var ExpectSentDate = $("#ExpectSentDate___Calendar").val();
            var time = new Date(ExpectSentDate);
            time.setHours(time.getHours() + data * 24);
            $("#ArrivalDate___Calendar").val(time.Format("yyyy-MM-dd"));
            $("#ArrivalDate___Year").val(time.getFullYear());
            $("#ArrivalDate___Month").val(time.getMonth() + 1);
            $("#ArrivalDate___Day").val(time.getDate());
        }
    });
}

function CheckArrivalDateCalendar() {
    if ($("#ArrivalDate___Calendar").val() == "NaN-aN-aN") {
        var date = new Date();
        date.setDate(date.getDate() + 1);
        $("#ArrivalDate___Calendar").val(date.Format("MM/dd/yyyy"));
        $("#ArrivalDate___Year").val(date.getFullYear());
        $("#ArrivalDate___Month").val(date.getMonth() + 1);
        $("#ArrivalDate___Day").val(date.getDate());
    }
}

