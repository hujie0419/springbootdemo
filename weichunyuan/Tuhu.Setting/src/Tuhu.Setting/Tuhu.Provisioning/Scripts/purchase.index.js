var flag = "";
var purchadeFlag = 0;
var action = function ()
{
    setTimeout(action, 1000);

    $(".BookingTime").each(function ()
    {
        var self = $(this);
        var day, hour, minitue, seconds;
        var ticks = new Date(self.attr("data-dealline")) - new Date();
        if (ticks > 0)
        {
            day = Math.floor(ticks / 1000 / 60 / 60 / 24);
            hour = Math.floor(ticks / 1000 / 60 / 60) % 24;
            minitue = Math.floor(ticks / 1000 / 60) % 60;
            seconds = Math.ceil(ticks / 1000) % 60;
            self.css("color", (minitue < 11 ? 'red' : "#696969")).text((day > 0 ? day + "天" : "") + (day > 0 || hour > 0 ? hour + "小时" : "") + (day > 0 || hour > 0 || minitue > 0 ? minitue + "分" : "") + seconds + "秒");
        }
        else
        {
            ticks = -ticks;
            day = Math.floor(ticks / 1000 / 60 / 60 / 24);
            hour = Math.floor(ticks / 1000 / 60 / 60) % 24;
            minitue = Math.floor(ticks / 1000 / 60) % 60;
            seconds = Math.ceil(ticks / 1000) % 60;
            self.css("color", "red").text("已超时" + (day > 0 ? day + "天" : "") + (day > 0 || hour > 0 ? hour + "小时" : "") + (day > 0 || hour > 0 || minitue > 0 ? minitue + "分" : "") + seconds + "秒");
        }
    });
}
action();

function ShowStock(target, orderListId)
{
    var $target = $(target).closest("div").next();
    if (purchadeFlag != orderListId){
        purchadeFlag = orderListId;
        $(".CreatePurchase").empty();
        $("tbody[data-status='OrderList']").each(function ()
        {
            var flag = $(this).closest("div").find("div[data-status='OrderStatus']").text().trim();
            if (flag != "待查货")
            {
                $(this).find("tr").css("background", "#FFFFFF");
            } else
            {
                $(this).find("tr").css("background", "#FBF7C1");
            }
        });
        $(target).closest("tr").css("background", "#C5C9F6");
        $.ajax({
            type: 'get',
            //async: false,
            data: { "id": orderListId },
            url: '/Logistic/CreatePurchase',
            beforeSend: function () {
                $("#loading").dialog("open");
            },
            success: function (html)
            {
                //$('html, body').animate({ scrollTop: $(target).position().top }, 'slow');
                $target.html("<fieldset id='Purchase'><legend>新建采购</legend>" + html + "</fieldset>").hide();
                $($target[0]).slideDown("slow");
                $("#loading").dialog("close");
            },
            error: function () {
                $("#loading").dialog("close");
                alert("操作失败！");
            }
        });
    } else {
        $target.slideToggle();
    }
    
}
//使用可用库存
function AddSoSotck(target, soItemId, batchId, num)
{
    var $target = $(target).closest("div[class='OrderAndPurchase']");
    $.ajax({
        type: 'get',
        //async: false,
        data: { "id": soItemId, "batchId": batchId, "num": num },
        url: '/Logistic/OccupySoStock',
        beforeSend: function () {
            $("#loading").dialog("open");
        },
        success: function (html)
        {
            $target.html(html);
            $("#loading").dialog("close");
        },
        error: function() {
            $("#loading").dialog("close");
            alert("使用库存失败！");
        }
    });
}
//释放库存
function RemoveSoSotck(target, soStockId)
{
    var $target = $(target).closest("div[class='OrderAndPurchase']");
    $.ajax({
        type: 'get',
        //async: false,
        data: { "id": soStockId },
        url: '/Logistic/ReleaseSoStock',
        beforeSend: function () {
            $("#loading").dialog("open");
        },
        success: function (html)
        {
            $target.html(html);
            $("#loading").dialog("close");
        },
        error:function() {
            $("#loading").dialog("close");
            alert("释放库存失败！");
        }
    });
}
//使用可用订单
function AddSoPo(target, soItemId, poId, num)
{
    var $target = $(target).closest("div[class='OrderAndPurchase']");
    $.ajax({
        type: 'get',
        //async: false,
        data: { "soItemId": soItemId, "poId": poId, "num": num },
        url: '/Logistic/UsingAvailablePurchaseOrders',
        beforeSend: function () {
            $("#loading").dialog("open");
        },
        success: function (html)
        {
            $target.html(html);
            $("#loading").dialog("close");
        },
        error:function() {
            $("#loading").dialog("close");
            alert("使用订单失败！");
        }
    });
}
//使用可用移库单
function AddSoTransfer(target, soItemId, taskProductId) {
    var $target = $(target).closest("div[class='OrderAndPurchase']");
    $.ajax({
        type: 'get',
        //async: false,
        data: { "soItemId": soItemId, "taskProductId": taskProductId },
        url: '/Logistic/UsingAvailableTransferOrders',
        beforeSend: function () {
            $("#loading").dialog("open");
        },
        success: function (html) {
            switch (html) {
                case "此移库产品的可用量为0，请刷新页面重新占用！":
                case "占用失败！":
                    alert(html);
                    break;
                default:
                    $target.html(html);
            }
            $("#loading").dialog("close");
        },
        error: function () {
            $("#loading").dialog("close");
            alert("使用移库单失败！");
        }
    });
}
//释放可用订单
function RemoveSoPo(target, sopoId)
{
    if (confirm("确认删除？"))
    {
        var $target = $(target).closest("div[class='OrderAndPurchase']");
        $.ajax({
            type: 'get',
            //async: false,
            data: { "sopoId": sopoId },
            url: '/Logistic/ReleaseAvailablePurchaseOrders',
            beforeSend: function () {
                $("#loading").dialog("open");
            },
            success: function (html)
            {
                $target.html(html);
                $("#loading").dialog("close");
            },
            error:function() {
                $("#loading").dialog("close");
                alert("释放订单失败！");
            }
        });
    }
}

//释放可用移库单
function RemoveSoTransfer(target, taskProductId, orderListId) {
    if (confirm("确认删除？")) {
        var $target = $(target).closest("div[class='OrderAndPurchase']");
        $.ajax({
            type: 'get',
            //async: false,
            data: { "taskProductId": taskProductId,"orderListId":orderListId },
            url: '/Logistic/ReleaseAvailableTransferOrders',
            beforeSend: function () {
                $("#loading").dialog("open");
            },
            success: function (html) {
                switch (html) {
                    case "释放失败！":
                        alert(html);
                        break;
                    default:
                        $target.html(html);
                }
                $("#loading").dialog("close");
            },
            error:function() {
                $("#loading").dialog("close");
                alert("释放移库单失败！");
            }
        });
    }
}

//创建采购
function CreatePoItem(target, soItemId, num, wareHouseId)
{
    var $item = $(target).text().trim();
    if (flag != $item){
        $("#CreatePoItemDiv").hide();
        flag = $item;
    }
    $("div[aria-labelledby='ui-dialog-title-SelectProducesDialog']").remove();
    $("#SelectProducesDialog").remove();
        $.ajax({
            type: 'get',
            async: false,
            url: '/Logistic/CreatePoItem',
            data: { "wareHouseId": wareHouseId, "id": soItemId, "num": num },
            success: function (html)
            {
                //$('html, body').animate({ scrollTop: $(target).position().top }, 'slow');
                $("#CreatePoItemDiv").html("<fieldset><legend>建采购</legend>" + html + "</fieldset>");
                $("#CreatePoItemDiv").slideToggle("slow");
            }
        });
}

//货源查询
function SearchGoodsRecord(target, orderListId, orderListPid) {
    var $item = $(target).text().trim();
    if (flag != $item){
        $("#CreatePoItemDiv").hide();
        flag = $item;
    }
    $("div[aria-labelledby='ui-dialog-title-SelectProducesDialog']").remove();
    $("#SelectProducesDialog").remove();
    $.ajax({
        type: 'get',
        async: false,
        data: { "orderListId": orderListId, "pid": orderListPid },
        url: '/Logistic/SearchGoodsRecord',
        success: function (html) {
            $("#CreatePoItemDiv").html(html);
            $("#CreatePoItemDiv ul:first").find("li").siblings().removeClass("resp-tab-active").end().eq(0).addClass("resp-tab-active");
            $(".resp-tabs-container").children(":odd").hide().removeClass("resp-tab-content-active")
                                                                          .eq(0).show().addClass("resp-tab-content-active");
            $("#CreatePoItemDiv").slideToggle("slow");
        }
    });
}

//切换仓库
function SwitchWareHouse(target, orderListId, orderNo, whOrderNo)
{
    if (confirm("确认修改仓库吗？"))
    {
        var $target = $(target).closest("div[class='OrderAndPurchase']");
        //同步--修改订单的仓库
        $.ajax({
            type: "post",
            url: "/Logistic/SwitchWareHouse",
            data: { "orderListId": orderListId, "orderNo": orderNo, "whOrderNo": whOrderNo },
            beforeSend: function () {
                $("#loading").dialog("open");
            },
            success: function (result){
                $("#loading").dialog("close");
                if (result.indexOf("false") === 0) {
                    alert(result.replace("false",""));
                } else {
                    $target.html(result);
                }
            },
            error: function () {
                $("#loading").dialog("close");
                alert("改仓库时出错!");
            }
        });
    }
}

//回复无货
function ReplyNoGoods(target,orderListId)
{
    $("#CreatePoItemDiv").empty();
    $('#ShowReplyNoGoodsDialog').dialog({
        title: "回复无货",
        width: 380,
        height: 230,
        modal: true,
        buttons: {
            "保存": function ()
            {
                var $target = $(target).closest("div[class='OrderAndPurchase']");
                var remark = $("#NoGoodsRemark").val().trim();
                if (remark !== "") {
                    $.ajax({
                        type: 'post',
                        anysc: false,
                        data: { "orderListId": orderListId, "remark": remark },
                        url: '/Logistic/ReplyPurchaseNoGoods',
                        success: function (result){
                            $target.html(result);
                            $('#ShowReplyNoGoodsDialog').dialog("close");
                            alert("保存成功!");
                        },
                        error: function() {
                            alert("保存失败!");
                        }
                    });
                } else {
                    alert("请输入备注!");
                }
            },
            "取消": function ()
            {
                $("#NoGoodsRemark").val("");
                $(this).dialog("close");
            }
        }
    });
}

$(document).ready(function ()
{
    $("#orderStatus").val("@ViewBag.OrderStatus");
    $("body").on("click", "#submit", function (event)
    {
        event.preventDefault();
        $(this).attr("disabled", "disabled");
        var $target = $(this).closest("div[class='OrderAndPurchase']");
        $("#CreatePoItemDiv form").ajaxSubmit({
            url: '/Logistic/CreatePurchaseOrderItem',
            success: function (html)
            {
                //$('html, body').animate({ scrollTop: $target.position().top + 100 }, 'slow');
                $target.html(html);
            }
        });
    });
});