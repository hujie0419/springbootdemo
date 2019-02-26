$(function () {
    //alert('dom ready' + $("#hiddenRemark").val());
    
})
function onChange(value) {
    var vehicleType = value == 0 ? '无需车型' : value == 2 ? "二级车型" : "五级车型";
    alert("确定要选择"+vehicleType+"吗？这样会清空之前选择的车型数据");
}
$("#btnSave").click(function () {
    var pid = $("#hiddenPid").val();
    var cpRemark = $("select").find("option:selected").first().text();
    var vehicleLevel = $("select").find("option:selected").last().text();
    var isAuto = $("#IsAuto").prop("checked");
    //alert(pid + ":" + cpRemark);

    $.ajax({
        type: "POST",
        url: "/ProductVehicleType/SaveProductInfo",
        data: { "pid": pid, "cpremark": cpRemark, "IsAuto": isAuto, "vehicleLevel": vehicleLevel },
        success: function (data) {
            console.log(data.msg);
            if (data.msg == "success") {
                alert('保存成功！');
            } else {
                alert("保存失败！");
            }
        }
    })
})

$("#select1").click(function() {
    if ($("#select1").val() === "0") {
        $("#IsAuto").removeAttr('checked');
    }
});

$("#IsAuto").click(function () {
    if ($("#select1").val() === "0") {
        alert("请选择车型！");
        $("#IsAuto").removeAttr('checked');
    }
});

//$(".triangle-icon-dowm").click(function () {
//    console.log(this);
//    $(this).attr("class", "triangle-icon");
//    $(this).parent().siblings().attr("class", "car-item-sub")
//})

//$(".triangle-icon-dowm").bind("click", function (event) {//点击收起
//    $(this).attr("class", "triangle-icon");
//    $(this).parent().siblings().attr("class", "car-item-sub")
//})


$(".triangle-icon").bind("click", function (event) {//点击展开
    if ($(this).hasClass("triangle-icon-dowm")) {
        $(this).removeClass("triangle-icon-dowm");
        $(this).parent().siblings().attr("class", "car-item-sub");
    } else {
        $(this).addClass("triangle-icon-dowm");
        $(this).parent().siblings().attr("class", "car-item-sub show")
    }
})

//$(".triangle-icon").click(function () {
//    $(this).attr("class", "triangle-icon triangle-icon-dowm");
//    $(this).parent().siblings().attr("class", "car-item-sub show")
//    alert('test');
//})