//点击配送物流里面的保存按钮
function btnLogic_SaveClick() {
    //得到所有的input
    var input = document.getElementsByName(GetRadioValue(document.getElementsByName("DivLogi_RadioSelect")));
    var reg = /[\u4e00-\u9fa5_a-zA-Z0-9]+,[\u4e00-\u9fa5_a-zA-Z0-9]+/;
    var regResult = reg.test($(input.item(0)).val());
    if (!regResult) {
        $(input.item(0)).css("border", "1px Solid Red");
        alert("选择的地址必须包含省/市或者直辖市/区");
    }
    var DeliveryDatetimeResult = true;
    //获得当天时间
    var datenow = new Date();
    //获得系统当前时间
    var todayTime = datenow.Format("yyyy-MM-dd");
    //获得选中时间
    var DeliveryDatetime = new Date($("#Abst").val()).Format("yyyy-MM-dd");
    if (DeliveryDatetime < todayTime) {
        DeliveryDatetimeResult = false;
        $("#Abst").css("border", "1px Solid Red");
    }
    else {
        $("#Abst").css("border", "");
    }
    //判断地址
    var result = JudeSelectValue(document.getElementById("DicKey"));
    result = JudeSelectValue(document.getElementById("DivLogis_Point"));
    //判断物流点，物流点是必填的
    if (regResult && result) {
        document.getElementById("DivLogis_Point").style.border = "1px solid";
        //隐藏div所有的元素
        HideDivAllChildrenDiv(document.getElementById("DivLogistic"));
        //显示保存发货时间的div
        document.getElementById("DivLogis_PointSave").style.display = "block";
        ShowInputAllChildrenInput(document.getElementById("DivLogis_PointSave"));
        document.getElementById("DivLogis_DateSave").style.display = "block";
        ShowInputAllChildrenInput(document.getElementById("DivLogis_DateSave"));

        //保存物流点
        document.getElementById("InputDivLogis_DateSave").value = "发货日期:" + document.getElementById("Abst").value;
        document.getElementById("BookDatetime").value = document.getElementById("Abst").value;

        document.getElementById("InputDivLogis_PointSave").value = "物流点:" + document.getElementById("DivLogis_Point").value;
        document.getElementById("DeliveryPoint").value = document.getElementById("DivLogis_Point").value;
        //物流公司
        document.getElementById("DeliveryCompany").value = document.getElementById("DicKey").value;
        //document.getElementById("DeliveryCompany").value = document.getElementById("DicKey").options[document.getElementById("DicKey").selectedIndex].text;
        //快递的地址
        var addressID = GetRadioValue(document.getElementsByName("DivLogi_RadioSelect"));
        var newStr = addressID.substring(5);
        //快递地址
        document.getElementById("DeliveryAddressID").value = newStr;

        //alert(input.length);
        //显示保存物流和地址的div
        document.getElementById("DivLogis_AddressAndExpressSave").style.display = "block";
        ShowInputAllChildrenInput(document.getElementById("DivLogis_AddressAndExpressSave"));
        //alert(document.getElementById("DicKey"));

        document.getElementById("inputLogis_AddressAndExpress").value = document.getElementById("DicKey").options[document.getElementById("DicKey").selectedIndex].text + ":" + input.item(0).value + input.item(1).value + input.item(2).value + input.item(3).value;
        //update by yangpeipei 2014/2/27
        var RegionName = input.item(0).value.toString().split(',')[1];
        $("#RegionName").val(RegionName);
        //alert($("#RegionName").val());
        //隐藏配送状态radio
        document.getElementById("DisplayDeliveryType").style.display = "none";
        //显示配送状态button
        document.getElementById("DivbtnDeliveryType").style.display = "block";

        //加载付款方式
        //显示DivShowPayMethod
        document.getElementById("DivShowPayMethod").style.display = "block";
        //OrderChannel = document.getElementById("btnOrderChannel").value;
        OrderChannel = document.getElementById("OrderChannel1").value;
      
        DeliveryType = GetRadioValue(document.getElementsByName("DeliveryType"));
        $("#DivShowPayMethod").load('/Order/DisplayPayMethod/', { "OrderChannel": OrderChannel, "DeliveryType": DeliveryType });

    }
}

