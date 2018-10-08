$(function () {
    //添加优惠券类型
    $(".youhuiquan").click(function () {
        $(".showbox").show();
    });

    $(".close").click(function () {
        $(".showbox").hide();
    });

    $(".chaxun").click(function () {
        var promotionTaskId = $("#PromotionTaskId").val();
        var taskName = $("#TaskName").val();
        var createTime = $("#CreateTime").val();
        var taskStatus = $("#TaskStatus").val();
        var taskType = $("#TaskType").val();
        var couponRulesId = $("#CouponRulesId").val();
        //var pageNo = $("#PageNo").val();

        location.href = "/Promotion/SearchPromotion?pageno=" + 1
            + "&promotionTaskId=" + promotionTaskId
            + "&taskName=" + taskName
            + "&createTime=" + createTime
            + "&taskStatus=" + taskStatus
            + "&taskType=" + taskType
            + "&couponRulesId=" + couponRulesId;
    });

    $(".shenhe").click(function () {
        var param = {
            id: $(this)[0].id,
            opType: "shenhe"
        };
        if (!confirm("是否确定操作："
            + $(this).html()+"-"
            + $(this).parent().parent().children()[1].innerHTML
            ))
            return;
        $.ajax({
            type: 'POST',
            url: '/Promotion/PromotionTaskShenhe',
            dataType: 'json',
            data: JSON.stringify(param),
            contentType: "application/json; charset=utf-8",
            success: function (jsonObj) {
                if (!jsonObj.IsSuccess) {
                    alert(jsonObj.OutMessage);
                    return;
                }
                alert(jsonObj.OutMessage);
                location.reload();
            }
        });

    });
    $(".guanbi").click(function () {
        var param = {
            id: $(this)[0].id,
            opType: "guanbi"
        };
        if (!confirm("是否确定操作："
            + $(this).html() + "-"
            + $(this).parent().parent().children()[1].innerHTML
            ))
            return;

        $.ajax({
            type: 'POST',
            url: '/Promotion/PromotionTaskShenhe',
            dataType: 'json',
            data: JSON.stringify(param),
            contentType: "application/json; charset=utf-8",
            success: function (jsonObj) {
                if (!jsonObj.IsSuccess) {
                    alert(jsonObj.OutMessage);
                    return;
                }
                alert(jsonObj.OutMessage);
                location.reload();
            }
        });
    });


    $("#CreateTime").datepicker({
        dateFormat: "yy-mm-dd"
    });


});