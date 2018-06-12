$(function () {
    $(".chaxunbox").click(function () {
        var userCellPhone = $("#UserCellPhone").val();
        var promotionCodeStatus = $("#PromotionCodeStatus").val();
        var pageNo = $("#PageNo").val();

        location.href = "/Promotion/UserPromotionCheck?pageno=" + pageNo
            + "&userCellPhone=" + userCellPhone
            + "&promotionCodeStatus=" + promotionCodeStatus;
    });
});