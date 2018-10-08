//点击快递里面的保存按钮
function btnExp_SaveClick() {

    var input = document.getElementsByName(GetRadioValue(document.getElementsByName("DivExpr_RadioSelect")));
    //验证地址的格式是否符合要求
    //alert($(input.item(0)).val());
    var reg = /[\u4e00-\u9fa5_a-zA-Z0-9]+,[\u4e00-\u9fa5_a-zA-Z0-9]+/;
    var result = reg.test($(input.item(0)).val());
    if (!result) {
        $(input.item(0)).css("border", "1px Solid Red");
        alert("选择的地址必须包含省/市或者直辖市/区");

    }
    var DeliveryDatetimeResult = true;
    //获得当天时间
    var datenow = new Date();
    //获得系统当前时间
    var todayTime = datenow.Format("yyyy-MM-dd");
    //获得选中时间
    var DeliveryDatetime = new Date($("#DeliveryDatetime").val()).Format("yyyy-MM-dd");
    if (DeliveryDatetime < todayTime) {
        DeliveryDatetimeResult = false;
        $("#DeliveryDatetime").css("border", "1px Solid Red");
    }
    else {
        $("#DeliveryDatetime").css("border", "");
    }
    var isTrue = true;
    if ($("#DicType").length > 0) {
        isTrue = DeliveryDatetimeResult && result && JudeSelectValue(document.getElementById("DicType"));
    }
    else {
        isTrue = DeliveryDatetimeResult && result;
    }
    if (isTrue) {
        if ($("#DicType").length > 0) {
            //document.getElementById("DicType").style.border = "1px solid";
            $("#DicType").css("border", "1px solid");
        }
        //隐藏div所有的元素
        HideDivAllChildrenDiv(document.getElementById("DivExpress"));

        //显示保存发货时间的div
        //document.getElementById("DivDeliveryDateSave").style.display = "block";
        $("#DivDeliveryDateSave").css("display", "block");
        ShowInputAllChildrenInput(document.getElementById("DivDeliveryDateSave"));
        //保存发货时间
        //document.getElementById("inputExpr_Date").value = "发货日期:" + document.getElementById("DeliveryDatetime").value;
        $("#inputExpr_Date").val("发货日期:" + $("#DeliveryDatetime").val());
        //document.getElementById("BookDatetime").value = document.getElementById("DeliveryDatetime").value;
        $("#BookDatetime").val($("#DeliveryDatetime").val());
        //alert(input.length);
        //显示保存物流和地址的div
        //document.getElementById("DivExpr_AddressAndExpressSave").style.display = "block";
        $("#DivExpr_AddressAndExpressSave").css("display", "block");
        ShowInputAllChildrenInput(document.getElementById("DivExpr_AddressAndExpressSave"));
        //document.getElementById("inputExpr_AddressAndExpress").value = document.getElementById("DicType").options[document.getElementById("DicType").selectedIndex].value + ":" + input.item(3).value + input.item(4).value;
        //判断选中的地址，是否包含省和市
        //OrderChannel = $("#btnOrderChannel").val();

        OrderChannel = $("#OrderChannel1").val();

        DeliveryType = GetRadioValue(document.getElementsByName("DeliveryType"));
        if ($("#DicType").length > 0 && DeliveryType == "快递送货上门") {
            //document.getElementById("inputExpr_AddressAndExpress").value = $("#DicType").find("option:selected").text() + ":" + input.item(0).value + input.item(1).value + input.item(2).value + input.item(3).value;
            //document.getElementById("DicType").options[document.getElementById("DicType").selectedIndex].text
            $("#inputExpr_AddressAndExpress").val($("#DicType").find("option:selected").text() + ":" + input.item(0).value + input.item(1).value + input.item(2).value + input.item(3).value);
        }
        else {
            $("#inputExpr_AddressAndExpress").val(input.item(0).value + input.item(1).value + input.item(2).value + input.item(3).value);
            //document.getElementById("inputExpr_AddressAndExpress").value = input.item(0).value + input.item(1).value + input.item(2).value + input.item(3).value;
        }
        //隐藏渠道radio
        //update by yangpeipei 2014/2/27
        //        var RegionName = input.item(0).value.toString().split(',')[1];
        //        $("#RegionName").val(RegionName);
        //alert($("#RegionName").val());
        //document.getElementById("DisplayOrderChannel").style.display = "none";
        $("#DisplayOrderChannel").css("display", "none");
        //显示渠道button
        //document.getElementById("DivbtnOrderChannel").style.display = "block";
        $("#DivbtnOrderChannel").css("display", "block");
        //隐藏配送状态radio
        //document.getElementById("DisplayDeliveryType").style.display = "none";
        $("#DisplayDeliveryType").css("display", "none");
        //$("#DisplayDeliveryType").css("display", "none");
        //显示配送状态button
        //document.getElementById("DivbtnDeliveryType").style.display = "block";
        $("#DivbtnDeliveryType").css("display", "block");
        //document.getElementById("btnDeliveryType").value = GetRadioValue(document.getElementsByName("DeliveryType"));
        $("#btnDeliveryType").val(GetRadioValue(document.getElementsByName("DeliveryType")))
        //加载付款方式
        //显示DivShowPayMethod
        //document.getElementById("DivShowPayMethod").style.display = "block";
        $("#DivShowPayMethod").css("display", "block");
        //OrderChannel = document.getElementById("btnOrderChannel").value;

        //快递公司存key值
        //alert($("#DicType").length);
        //alert(DeliveryType);
        //alert($("#DicType").val());
        if ($("#DicType").length > 0 && DeliveryType == "快递送货上门") {
            //document.getElementById("DeliveryCompany").value = $("#DicType").val();
            $("#DeliveryCompany").val($("#DicType").val());
        }
        else {
            $("#DeliveryCompany").val("");
        }
        //document.getElementById("DeliveryCompany").value = document.getElementById("DicType").options[document.getElementById("DicType").selectedIndex].text;
        //快递的地址
        var addressID = GetRadioValue(document.getElementsByName("DivExpr_RadioSelect"));
        var newStr = addressID.substring(5);
        //快递地址
        document.getElementById("DeliveryAddressID").value = newStr;
        var orderId = null;
        if ($("#PKID").length > 0) {
            orderId = $("#PKID").val();
        }

        var InstallType = document.getElementById("InstallType").value;
        $.ajax({
            type: "get",
            url: "/Order/DisplayPayMethod/",
            data: { "OrderChannel": OrderChannel, "DeliveryType": DeliveryType, "InstallShopID": null, "orderId": orderId, "InstallType": InstallType },
            async: false,
            success: function (data) {
                //alert(data);
                $("#DivShowPayMethod").html(data);
            }
        });
        //$("#DivShowPayMethod").load('/Order/DisplayPayMethod/', { "OrderChannel": OrderChannel, "DeliveryType": DeliveryType });
    }
}



//快递或者物流里面新地址保存button
function btnExpr_SaveNewAddress(inputName, divvalue) {
    //alert(inputName);
    //alert(divvalue);
    var provinceResult = true;
    var cityResult = true;
    switch (inputName) {
        case "Logisaddress":
            var u_city = jQuery("#Logis_city  option:selected").text();
            var u_region_code = jQuery("#Logis_u_region_name  option:selected").val() + "," + jQuery("#Logis_city  option:selected").val();
            //得到u_region_name
            if (jQuery("#Logis_u_region_name  option:selected").val() == "") {
                $("#Logis_u_region_name").css("border", "1px Solid Red");
                provinceResult = false;
            }
            if (jQuery("#Logis_city  option:selected").val() == "") {
                $("#Logis_city").css("border", "1px Solid Red");
                cityResult = false;
            }
            var u_region_name = jQuery("#Logis_u_region_name  option:selected").text() + "," + jQuery("#Logis_city  option:selected").text();
            flag = "logistic";
            $("#DivLogis_Droplist").load('/Order/DisplayLogistic', { "flag": flag, "region_code": u_region_code });

            break;
        case "address":
            var u_city = jQuery("#Expr_city  option:selected").text();
            //得到u_region_name
            var u_region_code = jQuery("#u_region_code  option:selected").val() + "," + jQuery("#Expr_city  option:selected").val();
            //得到u_region_name-
            if (jQuery("#u_region_code  option:selected").val() == "") {
                $("#u_region_code").css("border", "1px Solid Red");
                provinceResult = false;
            }
            if (jQuery("#Expr_city  option:selected").val() == "") {
                $("#Expr_city").css("border", "1px Solid Red");
                cityResult = false;
            }

            var u_region_name = jQuery("#u_region_code  option:selected").text() + "," + jQuery("#Expr_city  option:selected").text();
            flag = "express";
            $("#DivExpressDroplist").load('/Order/DisplayExpress', { "flag": flag, "region_code": u_region_code });
            break;
    }

    //  document.getElementById("DivExpr_AddNewAddress").style.display = "none";
    if (cityResult && provinceResult) {
        document.getElementById("DivLogis_AddNewAddress").style.display = "none";
        document.getElementById("DivExpr_AddNewAddress").style.display = "none";
        var id = document.getElementById("UserID").value;

        var address = document.getElementsByName(inputName);
        //用lastname存姓名
        var u_city = jQuery("#Expr_city  option:selected").text();
        //得到姓名
        var u_last_name = address.item(0).value;
        //得到电话
        var u_tel_number = address.item(1).value;
        //得到详细地址
        var u_address_line1 = address.item(3).value;

        //得到地址类型
        var u_address_name = address.item(2).value;

        $.post('/Order/AddNewAddress', { "id": id, "u_last_name": u_last_name, "u_tel_number": u_tel_number, "u_city": u_city, "u_address_line1": u_address_line1, "u_region_name": u_region_name, "u_address_name": u_address_name, "u_region_code": u_region_code }, function () {

            switch (divvalue) {

                //如果是快递，加载一下div                                                      
                case "Expr":
                    $("#DivExpres_DisplayAddress").load('/Order/DisplayAddress', { "id": id });
                    break;
                //如果是物流，加载一下div                                                      
                case "Logi":
                    $("#DivLogis_DisplayAddress").load('/Order/DisplayLogisticAddress', { "id": id });
                    break;
            }

        });

    }
}

//快递或者物流里面的地址删除button
function btnExpr_DeleteClick(inputName, divvalue) {
    if (confirm("确认删除吗?")) {
        var address = document.getElementsByName(inputName);
        var addressID = inputName.substring(5);
        //alert(addressID);
        var id = document.getElementById("UserID").value;
        $.post('/Order/DeleteAddress', { "addressID": addressID, "id": id }, function () {
            switch (divvalue) {
                //如果是快递，加载一下div                                                      
                case "Expr":
                    $("#DivExpres_DisplayAddress").load('/Order/DisplayAddress', { "id": id }, function () {
                        var value = GetRadioValue(document.getElementsByName("DivExpr_RadioSelect"));
                        var input = document.getElementsByName(value);
                        var region_code = input[4].value;
                        flag = "express";
                        $("#DivExpressDroplist").load('/Order/DisplayExpress', { "flag": flag, "region_code": region_code });
                    });
                    break;
                //如果是物流，加载一下div                                                      
                default:
                    $("#DivLogis_DisplayAddress").load('/Order/DisplayLogisticAddress', { "id": id }, function () {
                        var value = GetRadioValue(document.getElementsByName("DivLogi_RadioSelect"));
                        var input = document.getElementsByName(value);
                        var region_code = input[4].value;
                        flag = "logistic";
                        $("#DivLogis_Droplist").load('/Order/DisplayLogistic', { "flag": flag, "region_code": region_code });
                    });
                    break;
            }
        });

    }

}

//快递里面的编辑按钮
/*function btnExpr_Modify(obj, inputName, divvalue)
{
//alert(inputName);
var address = document.getElementsByName(inputName);
//alert(address.length);
if (obj.innerText == "编辑")
{

for (var i = 0; i < address.length; i++)
{
address[i].removeAttribute("readonly");
address[i]
address[i].style.border = "1px solid";

}
obj.innerText = "保存";
}
//当点击保存按钮的时候，要写到数据库里面
else
{
//将显示地址的input隐藏

var addressID = inputName.substring(5);
//  document.getElementById("DivExpr_AddNewAddress").style.display = "none";
var id = document.getElementById("UserID").value;
//获得所有input的值
var address = document.getElementsByName(inputName);
//用lastname存姓名
//得到省或者直辖市
//得到省或者直辖市
var u_region_name = address.item(0).value;
//得到城市
var u_city = address.item(1).value;
//得到详细地址
var u_address_line1 = address.item(2).value;
//得到姓名
var u_last_name = address.item(3).value;
//得到电话
var u_tel_number = address.item(4).value;

$.post('/Order/EditAddress', { "addressID": addressID, "u_last_name": u_last_name, "u_tel_number": u_tel_number, "u_city": u_city, "u_address_line1": u_address_line1, "u_region_name": u_region_name }, function ()
{
switch (divvalue)
{
//如果是快递，加载一下div    
case "Expr":
$("#DivExpres_DisplayAddress").load('/Order/DisplayAddress', { "id": id });
break;
//如果是物流，加载一下div    
case "Logi":
$("#DivLogis_DisplayAddress").load('/Order/DisplayAddress', { "id": id });
break;
}

});

for (var i = 0; i < address.length; i++)
{
address[i].readonly = true;
address[i].style.border = "0px";
}
obj.innerText = "编辑";
}
}
*/

//快递里面的选择地址radio事件
function DivExpr_RadioSelectClick(obj) {
    //obj.checked = checked;
    $(obj).attr("checked", "checked");
    //获取快递提货的默认省市的DicKey值
    var value = GetRadioValue(document.getElementsByName("DivExpr_RadioSelect"));
    var input = document.getElementsByName(value);
    var region_code = input[4].value;
    flag = "express";
    $("#DivExpressDroplist").load('/Order/DisplayExpress', { "flag": flag, "region_code": region_code });

}

//物流里面的选择地址radio事件
function DivLogi_RadioSelectClick(obj) {
    //obj.checked = checked;
    $(obj).attr("checked", "checked");
    //获取物流提货的默认省市的DicKey值
    var value = GetRadioValue(document.getElementsByName("DivLogi_RadioSelect"));
    var input = document.getElementsByName(value);
    var region_code = input[4].value;
    flag = "logistic";
    $("#DivLogis_Droplist").load('/Order/DisplayLogistic', { "flag": flag, "region_code": region_code });

}

function btnExpr_Modify(obj, inputName, divvalue) {
    var address = document.getElementsByName(inputName);
    //已经省和市所在的div
    var addessID = inputName.substring(5);
    var provinceSelectID
    var parentID;
    var CitySelectID;
    var divID;
    switch (divvalue) {
        //如果是快递，加载一下div                                                          
        case "Expr":
            //省的下拉列表
            provinceSelectID = "Prov+" + addessID;
            CitySelectID = "City+" + addessID;
            divID = "Expr1+" + addessID;
            break;
        //如果是物流，加载一下div                                                          
        case "Logi":
            provinceSelectID = "LogiProv+" + addessID;
            CitySelectID = "LogiCity+" + addessID;
            divID = "Logi1+" + addessID;
            break;
    }

    for (var i = 0; i < address.length - 1; i++) {
        address[i].removeAttribute("readonly");
        address[i].style.border = "1px solid";
    }
    //显示button
    address[5].style.display = "block";
    var strs = new Array(); //定义一数组
    strs = address.item(4).value.toString().split(',');
    document.getElementById(divID).style.display = "none";
    //需要显示的省的下拉列表
    document.getElementById(inputName).style.display = "block";
    $(document.getElementById(provinceSelectID)).load('/Order/DisplayProvince', function () {
        //得到省的下拉列表
        var s = document.getElementById(provinceSelectID);
        //得到省的code

        for (i = 0; i < s.length; i++) {
            if (s.options[i].value == strs[0].toString()) {
                s.options[i].selected = true;
                break;
            }
        }

    });

    $(document.getElementById(CitySelectID)).load('/Order/DisplayCity1', { "id": strs[0] }, function () {
        //得到城市的下拉列表
        var s = document.getElementById(CitySelectID);

        for (i = 0; i < s.length; i++) {
            if (s.options[i].value == strs[1].toString()) {
                s.options[i].selected = true;
                break;
            }
        }

    });
}
//省变动的时候，一起城市变动
function ProvinceChange(obj, CitySelectID) {
    var parentID = obj.value;

    $(document.getElementById(CitySelectID)).load('/Order/DisplayCity1', { "id": parentID });
}

//保存编辑的地址
function btnExpr_SaveModifyAddress(obj, inputName, divvalue) {
    var self = $(obj);
    var address = document.getElementsByName(inputName);
    //已经省和市所在的div
    var addessID = inputName.substring(5);
    var provinceSelectID
    var parentID;
    var CitySelectID;
    var divID;
    switch (divvalue) {
        //如果是快递，加载一下div                                                             
        case "Expr":
            //省的下拉列表
            provinceSelectID = "Prov+" + addessID;
            CitySelectID = "City+" + addessID;
            divID = "Expr1+" + addessID;
            break;
        //如果是物流，加载一下div                                                             
        case "Logi":
            provinceSelectID = "LogiProv+" + addessID;
            CitySelectID = "LogiCity+" + addessID;
            divID = "Logi1+" + addessID;
            break;
    }
    //    for (var i = 0; i < address.length; i++) {
    //将显示地址的input隐藏

    //var addressID = inputName.substring(5);
    //  document.getElementById("DivExpr_AddNewAddress").style.display = "none";
    var id = document.getElementById("UserID").value;
    //获得所有input的值
    //var address = document.getElementsByName(inputName);
    //用lastname存姓名
    //得到省或者直辖市
    //得到省或者直辖市
    var provinceResult = true;
    var cityResult = true;
    //如果省是请选择，提示错误信息
    if ($(document.getElementById(provinceSelectID)).find("option:selected").text() == "请选择") {
        $(document.getElementById(provinceSelectID)).css("border", "1px Solid Red");
        provinceResult = false;

    }
    //如果城市是请选择，提示错误信息
    if ($(document.getElementById(CitySelectID)).find("option:selected").text() == "请选择") {
        $(document.getElementById(CitySelectID)).css("border", "1px Solid Red");
        cityResult = false;
    }
    //当城市和省都不是请选择的时候，那么更改地址
    if (provinceResult && cityResult) {
        var u_region_code = $(document.getElementById(provinceSelectID)).val() + "," + $(document.getElementById(CitySelectID)).val();
        var u_region_name = $(document.getElementById(provinceSelectID)).find("option:selected").text() + "," + $(document.getElementById(CitySelectID)).find("option:selected").text();
        //得到城市
        var u_city = $(document.getElementById(CitySelectID)).find("option:selected").text();
        //得到详细地址
        var u_address_line1 = address.item(1).value;
        //得到姓名
        var u_last_name = address.item(2).value;
        //得到电话
        var u_tel_number = address.item(3).value;

        $.post('/Order/EditAddress', { "addressID": addessID, "u_last_name": u_last_name, "u_tel_number": u_tel_number, "u_city": u_city, "u_address_line1": u_address_line1, "u_region_name": u_region_name, "u_region_code": u_region_code }, function () {
            switch (divvalue) {
                //如果是快递，加载一下div                                                                              
                case "Expr":
                    $("#DivExpres_DisplayAddress").load('/Order/DisplayAddress', { "id": id }, function () {
                        var $selectValue = $(document.getElementById(self.parent().parent().attr("id")));
                        $("#DivLogis_DisplayAddress :radio:checked").removeProp("checked");
                        $selectValue.find(":radio").prop("checked", true);
                        var region_code = $($selectValue.find("input"))[5].value;
                        flag = "express";
                        $("#DivExpressDroplist").load('/Order/DisplayExpress', { "flag": flag, "region_code": region_code });
                    });
                    break;
                //如果是物流，加载一下div                                                                              
                default:
                    $("#DivLogis_DisplayAddress").load('/Order/DisplayLogisticAddress', { "id": id }, function () {
                        var $selectValue = $(document.getElementById(self.parent().parent().attr("id")));
                        $("#DivLogis_DisplayAddress :radio:checked").removeProp("checked");
                        $selectValue.find(":radio").prop("checked", true);
                        var region_code = $($selectValue.find("input"))[5].value;
                        flag = "logistic";
                        $("#DivLogis_Droplist").load('/Order/DisplayLogistic', { "flag": flag, "region_code": region_code });

                    });
                    break;
            }

        });

        for (var i = 0; i < address.length; i++) {
            address[i].readonly = true;
            address[i].style.border = "0px";
        }
    }
}