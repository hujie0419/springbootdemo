function DialogCustomerSelection() {
    var dlgResult = window.showModalDialog("/Customer/Selection", "", "dialogWidth:740px; dialogHeight:600px; status:0;");
    if (dlgResult != null) {
        //alert(dlgResult); 
        var uid = document.getElementById("UserID");
        uid.value = dlgResult.split("|")[0];
        var uname = document.getElementById("UserName");
        uname.value = dlgResult.split("|")[1];

    }
}
//得到选中radio的文本
function GetRadioText(input, radio) {
    //alert(1);

    for (var i; i < radio.length; i++) {
        if (radio[i].checked) {
            //alert(radio.item(i).nextSibling.nodeValue);
            input.value = radio.item(i).nextSibling.nodeValue;
            //alert(input.value);
            break;
        }
    }

}

//车牌radio事件
function DisplayOrderChannel(obj, u_carno, u_car_id) {
    document.getElementById("CarID").value = u_car_id;
    document.getElementById("CarPlate").value = u_carno;
    //alert(document.getElementById("CarID").value);
    //alert(document.getElementById("CarPlate").value);
    //点击渠道radio事件的时候，需要把
    //点击车牌radio的时候，如果渠道信息已经加载了，不需要再加载渠道信息，如果渠道button已经加载了，不需要加载渠道信息
    if (!$("#DisplayOrderChannel:visible").length) {
        if (!$("#DivbtnOrderChannel:visible").length) {
            document.getElementById("DisplayOrderChannel").style.display = "block";
        }
    }
}

//渠道radio的事件
//obj是渠道的dicKey，value是渠道的值dicValue
function DisplayDeliveryType(DicKey, DicValue) {
    //    switch (DicValue) {
    //        case "网站":
    //        case "平安":
    //        case "微信":
    //        case "手机":
    //        case "8857":
    //            document.getElementById("DivRef").style.display = "none";
    //            document.getElementById("RefNo").value = "";
    //            break;
    //        default:
    document.getElementById("DivRef").style.display = "block";
    //ShowInputAllChildrenInput(document.getElementById("DivRef"));
    //            break;
    //    }
    //把选中的值给隐藏hiddenOrderChannel
    document.getElementById("OrderChannel1").value = DicKey;
    //alert(document.getElementById("OrderChannel1").value);
    document.getElementById("btnOrderChannel").value = DicValue;
    //如果配送状态button不存在
    if (!$("#DivbtnDeliveryType:visible").length) {
        //1.配送状态不存在，并且门店信息也没有显示
        if (!$("#DisplayInstallShop:visible").length) {
            document.getElementById("DisplayDeliveryType").style.display = "block";
            //如果选中的无需配送
            if (GetRadioValue(document.getElementsByName("DeliveryType")) == "无需配送") {
                document.getElementById("DisplayOrderChannel").style.display = "none";
                if (document.getElementById("DivbtnOrderChannel") != null) {
                    document.getElementById("DivbtnOrderChannel").style.display = "block";
                }
                //var objtype = document.getElementsByName("OrderChannel");
                //document.getElementById("btnOrderChannel").value = DicValue;
                //alert(document.getElementById("btnOrderChannel").value);
                //GiveButtonValue(document.getElementById("btnOrderChannel"), objtype, document.getElementById("hiddenOrderChannel"));
            }
        }
            //2.如果配送状态按钮不存在，但是门店信息存在
        else {
            document.getElementById("DisplayOrderChannel").style.display = "none";
            if (document.getElementById("DivbtnOrderChannel") != null) {
                document.getElementById("DivbtnOrderChannel").style.display = "block";
            }
            var objtype = document.getElementsByName("OrderChannel");
            //给button赋值
            //document.getElementById("btnOrderChannel").value = DicValue;
            //alert(document.getElementById("btnOrderChannel").value);
            //GiveButtonValue(document.getElementById("btnOrderChannel"), objtype, document.getElementById("hiddenOrderChannel"));
        }

    }
        //3.配送状态按钮存在
    else {
        //隐藏渠道radio，转换成button
        // $("#DisplayOrderChannel").hide();
        document.getElementById("DisplayOrderChannel").style.display = "none";
        //$("#DivbtnOrderChannel").show();
        if (document.getElementById("DivbtnOrderChannel") != null) {
            document.getElementById("DivbtnOrderChannel").style.display = "block";
        }
        var objtype = document.getElementsByName("OrderChannel");
        //GiveButtonValue(document.getElementById("btnOrderChannel"), document.getElementsByName("OrderChannel"), document.getElementById("hiddenOrderChannel"));
        //document.getElementById("btnOrderChannel").value = DicValue;
        //alert(document.getElementById("btnOrderChannel").value);
    }
    //如果渠道信息已经存在
    if (document.getElementById("DivShowPayMethod").style.display == "block" || document.getElementById("btnPayMethod").value != "") {
        //得到此时的渠道信息
        //1.如果是付款button
        if (document.getElementById("btnPayMethod").value != "") {
            //隐藏付款button，显示付款选项
            document.getElementById("DivbtnPayMethod").style.display = "none";
            document.getElementById("DivShowPayMethod").style.display = "block";
            var OrderChannel = document.getElementById("btnOrderChannel").value;
            var DeliveryType = document.getElementById("btnDeliveryType").value;
            //alert(1);
            LoadPayMethod();
            //$("#DivShowPayMethod").load('/Order/DisplayPayMethod/', { "OrderChannel": OrderChannel, "DeliveryType": DeliveryType });
        }
            //2.如果不是付款button
        else {
            var OrderChannel = GetRadioValue(document.getElementsByName("OrderChannel"))
            var DeliveryType = GetRadioValue(document.getElementsByName("DeliveryType"));
            document.getElementById("DivShowPayMethod").style.display = "block";
            //alert(2);
            LoadPayMethod();
            //$("#DivShowPayMethod").load('/Order/DisplayPayMethod/', { "OrderChannel": OrderChannel, "DeliveryType": DeliveryType });

        }
    }
}

//渠道button事件
function ClickOrderChannelButton() {
    //$("#DisplayOrderChannel").show();
    document.getElementById("DisplayOrderChannel").style.display = "block";
    //隐藏渠道button所在的div
    //$("#DivbtnOrderChannel").hide();
    if (document.getElementById("DivbtnOrderChannel") != null) {
        document.getElementById("DivbtnOrderChannel").style.display = "none";
    }
}

//配送状态button事件
function btnDeliveryTypeClick(obj) {
    //当我点击配送状态按钮时，门店信息已经被保存了
    //加载配送状态信息，隐藏button
    //alert(1);
    document.getElementById("DisplayDeliveryType").style.display = "block";
    // ShowInputAllChildrenInput(document.getElementById("DisplayDeliveryType"));
    document.getElementById("DivbtnDeliveryType").style.display = "none";

}
// 配送状态radio事件
function DisplayInstallShop(obi) {
    var result = true;
    //	if (document.getElementById("DivRef").style.display == "block") {
    //		result = JudeSelectValue(document.getElementById("RefNo"));
    //	}
    //var OrderChannel = $("input[name='OrderChannel']").prop("checked").val();
    var OrderChannel = GetRadioValue(document.getElementsByName("OrderChannel"));
    switch (OrderChannel) {
        case "1网站":
        case "8手机":
        case "a易迅":
        case "b平安":
        case "c微信":
        case "f8857":
        case "f大客户":
        case "JIOS":
        case "kH5":
            result = true;
            $("#RefNo").css("border", "");
            $("#ErrorMessage").remove();
            break;
        default:
            if (document.getElementById("DivRef").style.display == "block") {
                result = JudeSelectValue(document.getElementById("RefNo"));
                if ($("#RefNo").next().length <= 0) {
                    $("#RefNo").after("<span id='ErrorMessage' style='color: Red'>必填</span>");
                }
            }
            break;
    }
    //验证外联单号是否必填
    if (result == true) {
        //得到安装类型和配送类型
        switch (obi) {

            case "到店不安装":
                {
                    document.getElementById("InstallType").value = "3NoInstall";
                    document.getElementById("DeliveryType1").value = "1ShopInstall";
                    document.getElementById("DeliveryCompany").value = "";
                    //如果物流点已经保存了，清掉
                    document.getElementById("DeliveryPoint").value = "";
                    //清楚地址ID
                    document.getElementById("DeliveryAddressID").value = "";

                    break;
                }
            case "快递送货上门":

                {
                    document.getElementById("InstallType").value = "3NoInstall";
                    document.getElementById("DeliveryType1").value = "2Express";
                    //如果物流点已经保存了，清掉
                    document.getElementById("DeliveryPoint").value = "";
                    //清掉安装门店信息
                    document.getElementById("InstallShopID").value = "";
                    document.getElementById("InstallShop").value = "";

                    break;

                }
            case "途虎送货上门":
                {
                    document.getElementById("InstallType").value = "3NoInstall";
                    document.getElementById("DeliveryType1").value = "5TuHuSent";
                    //如果物流点已经保存了，清掉
                    document.getElementById("DeliveryPoint").value = "";
                    $("#DeliveryCompany").val("");
                    //清掉安装门店信息
                    document.getElementById("InstallShopID").value = "";
                    document.getElementById("InstallShop").value = "";
                    break;
                }
        	case "上门安装":
        		{
        			document.getElementById("InstallType").value = "4SentInstall";
        			document.getElementById("DeliveryType1").value = "";
        			//如果物流点已经保存了，清掉
        			document.getElementById("DeliveryPoint").value = "";
        			$("#DeliveryCompany").val("");
        			//清掉安装门店信息
        			document.getElementById("InstallShopID").value = "";
        			document.getElementById("InstallShop").value = "";
        			break;

        		}
            case "物流点提货":
                {
                    document.getElementById("InstallType").value = "3NoInstall";
                    document.getElementById("DeliveryType1").value = "3Logistic";
                    //清掉安装门店信息
                    document.getElementById("InstallShopID").value = "";
                    document.getElementById("InstallShop").value = "";
                    break;
                }
            case "无需配送":
                {
                    document.getElementById("InstallType").value = "3NoInstall";
                    document.getElementById("DeliveryType1").value = "4NoDelivery";
                    document.getElementById("DeliveryCompany").value = "";
                    //如果物流点已经保存了，清掉
                    document.getElementById("DeliveryPoint").value = "";
                    //清楚地址ID
                    document.getElementById("DeliveryAddressID").value = "";
                    //清掉安装门店信息
                    document.getElementById("InstallShopID").value = "";
                    document.getElementById("InstallShop").value = "";
                    document.getElementById("BookPeriod1").value = "";
                    document.getElementById("BookDatetime").value = "";
                    //清空StockName的值
                    //$("#StockName").val("");
                    break;
                }
            default:
                {
                    document.getElementById("InstallType").value = "1ShopInstall";
                    document.getElementById("DeliveryType1").value = "1ShopInstall";
                    document.getElementById("DeliveryCompany").value = "";
                    //如果物流点已经保存了，清掉
                    document.getElementById("DeliveryPoint").value = "";
                    //清楚地址ID
                    document.getElementById("DeliveryAddressID").value = "";
                    break;
                }


        }
        document.getElementById("btnDeliveryType").value = obi;
        var datenow = new Date();
        //获得系统当前时间
        var todayTime = datenow.Format("yyyy-MM-dd");
        //获得明天时间
        var tomorrowTime = new Date();
        tomorrowTime.setDate(tomorrowTime.getDate() + 1);
        tomorrowTime = new Date(tomorrowTime).Format("yyyy-MM-dd");
        var flag = "";
        switch (obi) {
            //如果是到店安装和到店不安装，加载门店dropdownlist信息和门店保存按钮                                                                                                                                                        
            case "到店安装":
            case "到店不安装":
                {
                    //隐藏点击快递加载的div
                    if (document.getElementById("DivExpress") != null) {
                        if (document.getElementById("DivExpress").style.display == "block") {
                            //alert(document.getElementById("DivExpress").style.display);
                            HideDiv(document.getElementById("DivExpress"));
                        }
                    }
                    //如果已经点了物流，隐藏该div
                    if (document.getElementById("DivLogistic") != null) {
                        if (document.getElementById("DivLogistic").style.display == "block") {
                            HideDiv(document.getElementById("DivLogistic"));
                        }
                    }
                    //点击无需配送按钮加载的div
                    $("#DivNoDelivery").css("display", "none");

                    //当门店信息已经保存了
                    if (document.getElementById("DivbtnInstallShop") != null) {
                        if (document.getElementById("DivbtnInstallShop").style.display == "block") {
                            // alert(2);
                            //不需要显示门店drop
                            if (document.getElementById("DisplayInstallShop") != null) {
                                document.getElementById("DisplayInstallShop").style.display = "none";
                            }
                            //document.getElementById("DisplayInstallShopProvinceCity").style.display = "none";
                            //把radio转换成button
                            if (document.getElementById("DivbtnOrderChannel") != null) {
                                document.getElementById("DivbtnOrderChannel").style.display = "block";
                            }
                            //隐藏配送状态radio
                            if (document.getElementById("DisplayDeliveryType") != null) {
                                document.getElementById("DisplayDeliveryType").style.display = "none";
                            }
                            //显示配送状态的button
                            if (document.getElementById("DivbtnDeliveryType") != null) {
                                document.getElementById("DivbtnDeliveryType").style.display = "block";
                            }

                        }
                            //如果门店信息没有保存
                        else {

                            //隐藏渠道radio
                            if (document.getElementById("DisplayOrderChannel") != null) {
                                document.getElementById("DisplayOrderChannel").style.display = "none";
                            }
                            //显示渠道button
                            if (document.getElementById("DivbtnOrderChannel") != null) {
                                document.getElementById("DivbtnOrderChannel").style.display = "block";
                            }
                            //显示点击到店安装需要显示的div
                            if (document.getElementById("DivInstallShop") != null) {
                                document.getElementById("DivInstallShop").style.display = "block";

                            }
                            if (document.getElementById("DisplayInstallShop") != null) {

                                document.getElementById("DisplayInstallShop").style.display = "block";

                            }
                            //如果
                            if ($("#Province").val() != "请选择") {
                                var city = $("#Province").find("option:selected").text();
                                $("#DisplayInstallShop1").css("display", "block");
                                $("#DisplayInstallShop1").load('/Order/DisplayInstallShop', { "province": $("#Province").val(), "city": city });
                            }
                            //document.getElementById("DisplayInstallShopProvinceCity").style.display = "block";
                        } 
                    }
                    selectChange(1);
                    //加载付款方式
                    LoadPayMethod();
                    break;
                }


        	//如果是快递送货上门，加载送货日期和快递信息
        	case "上门安装":
            case "快递送货上门":
            case "途虎送货上门":
                {
                    //如果已经点了到店安装或者到店不安装，隐藏div"DivInstallShop",隐藏里面的所有元素
                    if (document.getElementById("DivInstallShop").style.display == "block") {
                        HideDiv(document.getElementById("DivInstallShop"));
                    }
                    //如果已经点了物流，隐藏该div
                    if (document.getElementById("DivLogistic").style.display == "block") {
                        HideDiv(document.getElementById("DivLogistic"));
                    }
                    //如果已经点击了快递里面的保存按钮，隐藏保存的地址信息和日期
                    if (document.getElementById("DivDeliveryDateSave").style.display == "block") {
                        document.getElementById("DivDeliveryDateSave").style.display = "none";
                        document.getElementById("inputExpr_Date").style.display = "none";
                        document.getElementById("DivExpr_AddressAndExpressSave").style.display = "none";
                        document.getElementById("inputExpr_AddressAndExpress").style.display = "none";

                    }
                    //隐藏渠道radio
                    if (document.getElementById("DisplayOrderChannel") != null) {
                        document.getElementById("DisplayOrderChannel").style.display = "none";
                    }
                    //显示渠道button
                    if (document.getElementById("DivbtnOrderChannel") != null) {
                        document.getElementById("DivbtnOrderChannel").style.display = "block";
                    }
                    //隐藏点击无需配送按钮加载的div
                    $("#DivNoDelivery").css("display", "none");
                    //显示点击快递需要加载的div
                    document.getElementById("DivExpress").style.display = "block";


                    //如果是快递，当前时间如果小于下午4点，那么显示今天日期
                    //根据配送状态不一样加载不同的div
                    var needTime = todayTime.toString() + " 16:00:00";
                    needTime = new Date(needTime).Format("yyyy-MM-dd hh:mm:ss");
                    var now = new Date();
                    now = new Date(now).Format("yyyy-MM-dd hh:mm:ss");
                    if (now < needTime) {
                        $("#DeliveryDatetime").val(todayTime);
                    }
                        //否则显示明天日期
                    else {
                        $("#DeliveryDatetime").val(tomorrowTime);
                    }
                    //显示日期//显示保存物流的droplist
                    ShowDivAllChildrenDiv(document.getElementById("DivExpress"));
                    if (obi == "途虎送货上门" || obi == "上门安装") {
                        $("#DivExpressDroplist").css("display", "none");
                    }
                    //隐藏添加地址的div
                    document.getElementById("DivExpr_AddNewAddress").style.display = "none";
                    //加载快递信息
                    var guid = document.getElementById("UserID").value;
                    $("#DivExpres_DisplayAddress").load('/Order/DisplayAddress', { "id": guid }, function () {
                        if (obi != "途虎送货上门") {
                            var value = GetRadioValue(document.getElementsByName("DivExpr_RadioSelect"));
                            var input = document.getElementsByName(value);
                            var region_code = input[4].value;
                            flag = "express";
                            $("#DivExpressDroplist").load('/Order/DisplayExpress', { "flag": flag, "region_code": region_code });
                        }
                        //alert(input[4].value);
                    });

                    //加载付款方式
                    LoadPayMethod();
                    break;
                }

            case "物流点提货":
                {
                    //如果已经点了到店安装和快递按钮，隐藏那些div
                    HideDiv(document.getElementById("DivInstallShop"));
                    //隐藏点击快递加载的div
                    HideDiv(document.getElementById("DivExpress"));
                    //如果已经点击了里面物流的保存按钮，隐藏保存的地址信息和日期
                    if (document.getElementById("DivLogis_PointSave").style.display == "block") {
                        document.getElementById("DivLogis_PointSave").style.display = "none";
                        document.getElementById("InputDivLogis_PointSave").style.display = "none";
                        document.getElementById("DivLogis_AddressAndExpressSave").style.display = "none";
                        document.getElementById("inputLogis_AddressAndExpress").style.display = "none";

                    }
                    //隐藏渠道radio
                    if (document.getElementById("DisplayOrderChannel") != null) {
                        document.getElementById("DisplayOrderChannel").style.display = "none";
                    }
                    //显示渠道button
                    if (document.getElementById("DisplayOrderChannel") != null) {
                        document.getElementById("DivbtnOrderChannel").style.display = "block";
                    }
                    //隐藏点击无需配送按钮加载的div
                    $("#DivNoDelivery").css("display", "none");
                    //显示点击物流需要加载的div
                    document.getElementById("DivLogistic").style.display = "block";
                    //如果是快递，当前时间如果小于下午4点，那么显示今天日期
                    //var time = GetNowTime();
                    var needTime = todayTime.toString() + " 15:00:00";
                    needTime = new Date(needTime).Format("yyyy-MM-dd hh:mm:ss");
                    var now = new Date();
                    now = new Date(now).Format("yyyy-MM-dd hh:mm:ss");
                    if (now < needTime) {
                        $("#Abst").val(todayTime);
                    }
                        //否则显示明天日期
                    else {
                        $("#Abst").val(tomorrowTime);
                    }
                    ShowDivAllChildrenDiv(document.getElementById("DivLogistic"));
                    //隐藏添加地址的div
                    document.getElementById("DivLogis_AddNewAddress").style.display = "none";

                    var guid = document.getElementById("UserID").value;
                    $("#DivLogis_DisplayAddress").load('/Order/DisplayLogisticAddress', { "id": guid }, function () {
                        //获取物流提货的默认省市的DicKey值
                        var value = GetRadioValue(document.getElementsByName("DivLogi_RadioSelect"));
                        var input = document.getElementsByName(value);
                        var region_code = input[4].value;
                        flag = "logistic";
                        $("#DivLogis_Droplist").load('/Order/DisplayLogistic', { "flag": flag, "region_code": region_code });
                        //alert(input[4].value);
                    });
                    //加载付款方式
                    LoadPayMethod();
                    break;
                }
                //无需配送
            case "无需配送":
                {

                    //如果已经点了到店安装或者到店不安装，隐藏div"DivInstallShop",隐藏里面的所有元素
                    HideDiv(document.getElementById("DivInstallShop"));
                    //隐藏点击快递加载的div
                    HideDiv(document.getElementById("DivExpress"));
                    //如果已经点了物流，隐藏该div

                    HideDiv(document.getElementById("DivLogistic"));

                    //                    //如果已经点了途虎送货上门，隐藏加载出来的div
                    //                    //点击无需配送之后，直接加载付款方式
                    //点击无需配送，渠道radio隐藏
                    document.getElementById("DisplayOrderChannel").style.display = "none";
                    //渠道显示button显示
                    if (document.getElementById("DivbtnOrderChannel") != null) {
                        document.getElementById("DivbtnOrderChannel").style.display = "block";
                    }

                    //隐藏配送状态radio
                    $("#DisplayDeliveryType").css("display", "none");
                    //显示配送状态button
                    document.getElementById("DivbtnDeliveryType").style.display = "block";

                    //显示点击无需配送需要加载的div
                    $("#DivNoDelivery").css("display", "block");
                    //加载付款方式
                    LoadPayMethod();
                    break;
                }
        }
    }
}

//公共方法用来隐藏div和其子div
function HideDiv(obj) {
    if (obj.style.display == "block") {
        obj.style.display = "none";
        HideDivAllChildrenDiv(obj);
    }
}

//保存门店button事件
function btnSaveShopClick() {
    var result = true;
    result = JudeSelectValue(document.getElementById("Province"));
    result = JudeSelectValue(document.getElementById("City"));
    var SelectText = $('#City').find('option:selected').text();
    if (SelectText.indexOf(' \( 快递 - ') > 0) {
        result = JudeSelectValue(document.getElementById("ExpressCo"));
        $('#DeliveryCompany').val($('#ExpressCo').val());
        document.getElementById('ShowDeliveryCompany').innerHTML = "配送方式:" + $('#ExpressCo').find('option:selected').text();
        document.getElementById('ShowDeliveryCompany').style.display = "block";
    }

    if (result == true) {
        //当点击门店保存按钮，加载需要加载的div，并且将里面的所有元素显示出来
        document.getElementById("DivbtnInstallShop").style.display = "block";
        document.getElementById("ShowShop").style.display = "block";
        document.getElementById("BookDate").style.display = "block";
        document.getElementById("BookTime").style.display = "block";
        document.getElementById("DivSaveTime").style.display = "block";
        //显示保存配送状态的按钮
        document.getElementById("DivbtnDeliveryType").style.display = "block";
        //var objshop = document.getElementById("InstallShopID_Value");
        document.getElementById("ShowShop").innerHTML = "门店：" + $("#Province").val() + $("#Province").find("option:selected").text() + $("#City").find("option:selected").text();
        //得到门店ID
        document.getElementById("InstallShopID").value = $("#City").val();
        //得到区域RegionName
        $("#RegionName").val($("#Province").find("option:selected").text());
        //alert($("#RegionName").val());
        document.getElementById("InstallShop").value = $("#City").find("option:selected").text();
        //alert(objshop.options[objshop.selectedIndex].text)
        //显示配送状态按钮，隐藏radio
        document.getElementById("DivbtnDeliveryType").style.display = "block";
        document.getElementById("DisplayDeliveryType").style.display = "none";
        document.getElementById("DisplayInstallShop").style.display = "none";
        //把值付给新button
        var objtype = document.getElementsByName("DeliveryType");
        for (var i = 0; i < objtype.length; i++) {
            if (objtype[i].checked) {
                document.getElementById("btnDeliveryType").value = objtype[i].value;

            }
        }
        //点击提交按钮时，显示配送状态的值
        //隐藏配送状态的选项
        if (document.getElementById("ShowShop").style.display == "none") {
            document.getElementById("ShowShop").style.display = "block";
        }
        //document.getElementById("BookDate").style.display = "block";
        //判断门店是否是外地门店
        var datenow = new Date();
        //获得系统当前时间
        var todayTime = datenow.Format("yyyy-MM-dd");
        //获得明天时间
        var tomorrowTime = new Date();
        tomorrowTime.setDate(tomorrowTime.getDate() + 1);
        //后天时间
        var tomorrowAfterTime = new Date();
        tomorrowAfterTime = tomorrowAfterTime.setDate(tomorrowAfterTime.getDate() + 2);
        tomorrowTime = new Date(tomorrowTime).Format("yyyy-MM-dd");
        tomorrowAfterTime = new Date(tomorrowAfterTime).Format("yyyy-MM-dd");
        //var InstallShopID = $("#InstallShopID").val().split('|')[0];
        alert($("#InstallShopID").val());
        $.post("/Order/JudgeShopType", { shopID:  $("#InstallShopID").val().split('|')[0] }, function (data) {
            if (data == "2Express") {
                //如果选中的门店需要快递送货，判断创建时间，如果小于下午5点，那么日期显示明天日期，下午五点
                //获得当前时间
                var needTime = todayTime.toString() + " 17:00:00";
                needTime = new Date(needTime).Format("yyyy-MM-dd hh:mm:ss");
                var now = new Date();
                now = new Date(now).Format("yyyy-MM-dd hh:mm:ss");
                if (now < needTime) {
                    $("#BookDatetime___Calendar").val(tomorrowTime);
                    $("#BookPeriod").val("17:00");
                }
                    //如果当前时间大于下午5点，那么日期显示后天日期，下午5点
                else {
                    $("#BookDatetime___Calendar").val(tomorrowAfterTime);
                    var s = document.getElementById("BookPeriod");
                    $("#BookPeriod").val("17:00");
                }
            }
                //如果门店是途虎送货，那么日期显示明天日期，下午三点半
            else {
                //alert(tomorrowTime);
                $("#BookDatetime___Calendar").val(tomorrowTime);

                $("#BookPeriod").val("15:30");
            }
        });

        if ($("#BookPeriod").length > 0) {
            $("#DivSaveTime").css("display", "block");
        }
        else {
            $("#DivSaveTime").css("display", "none");
        }
    }
}



function btnSaveShopClickNew() {
    var result = true;
    result = JudeSelectValue(document.getElementById("City"));
    result = JudeSelectValue(document.getElementById("Shop"));
    var SelectText = $('#Shop').find('option:selected').text();
    if (SelectText.indexOf(' \( 快递 - ') > 0) {
        result = JudeSelectValue(document.getElementById("ExpressCo"));
        $('#DeliveryCompany').val($('#ExpressCo').val());
        document.getElementById('ShowDeliveryCompany').innerHTML = "配送方式:" + $('#ExpressCo').find('option:selected').text();
        document.getElementById('ShowDeliveryCompany').style.display = "block";
    }

    if (result == true) {
        //当点击门店保存按钮，加载需要加载的div，并且将里面的所有元素显示出来
        document.getElementById("DivbtnInstallShop").style.display = "block";
        document.getElementById("ShowShop").style.display = "block";
        document.getElementById("BookDate").style.display = "block";
        document.getElementById("BookTime").style.display = "block";
        document.getElementById("DivSaveTime").style.display = "block";
        //显示保存配送状态的按钮
        document.getElementById("DivbtnDeliveryType").style.display = "block";
        //var objshop = document.getElementById("InstallShopID_Value");
        document.getElementById("ShowShop").innerHTML = "门店：" + $("#Shop").val() +"|"+ $("#Shop").find("option:selected").text();
        //得到门店ID
        document.getElementById("InstallShopID").value = $("#Shop").val();
        //得到区域RegionName和区域RegionID
        $("#RegionName").val($("#City").find("option:selected").text());
        $("#RegionID").val($("#City").val());
        //得到门店ID
        //var shopId = $("#Shop").val().split('|')[0];
        //$("#InstallShopID.Value").val(shopId);
        //alert($("#RegionName").val());
        document.getElementById("InstallShop").value = $("#Shop").find("option:selected").text();
        //alert(objshop.options[objshop.selectedIndex].text)
        //显示配送状态按钮，隐藏radio
        document.getElementById("DivbtnDeliveryType").style.display = "block";
        document.getElementById("DisplayDeliveryType").style.display = "none";
        document.getElementById("DisplayInstallShop").style.display = "none";
        //把值付给新button
        var objtype = document.getElementsByName("DeliveryType");
        for (var i = 0; i < objtype.length; i++) {
            if (objtype[i].checked) {
                document.getElementById("btnDeliveryType").value = objtype[i].value;

            }
        }
        //点击提交按钮时，显示配送状态的值
        //隐藏配送状态的选项
        if (document.getElementById("ShowShop").style.display == "none") {
            document.getElementById("ShowShop").style.display = "block";
        }
        //document.getElementById("BookDate").style.display = "block";
        //判断门店是否是外地门店
        var datenow = new Date();
        //获得系统当前时间
        var todayTime = datenow.Format("yyyy-MM-dd");
        //获得明天时间
        var tomorrowTime = new Date();
        tomorrowTime.setDate(tomorrowTime.getDate() + 1);
        //后天时间
        var tomorrowAfterTime = new Date();
        tomorrowAfterTime = tomorrowAfterTime.setDate(tomorrowAfterTime.getDate() + 2);
        tomorrowTime = new Date(tomorrowTime).Format("yyyy-MM-dd");
        tomorrowAfterTime = new Date(tomorrowAfterTime).Format("yyyy-MM-dd");
       
        //获得门店的配送类型
        var shopType = GetShopType($("#InstallShopID").val().split('|')[0]);
       
        if (shopType == "2Express") {
            //如果选中的门店需要快递送货，判断创建时间，如果小于下午5点，那么日期显示明天日期，下午五点
            //获得当前时间
            var needTime = todayTime.toString() + " 17:00:00";
            needTime = new Date(needTime).Format("yyyy-MM-dd hh:mm:ss");
            var now = new Date();
            now = new Date(now).Format("yyyy-MM-dd hh:mm:ss");
            if (now < needTime) {
                $("#BookDatetime___Calendar").val(tomorrowTime);
                $("#BookPeriod").val("17:00");
            }
            //如果当前时间大于下午5点，那么日期显示后天日期，下午5点
            else {
                $("#BookDatetime___Calendar").val(tomorrowAfterTime);
                var s = document.getElementById("BookPeriod");
                $("#BookPeriod").val("17:00");
            }
        }
        //如果门店是途虎送货，那么日期显示明天日期，下午三点半
        else {
            //alert(tomorrowTime);
            $("#BookDatetime___Calendar").val(tomorrowTime);
            $("#BookPeriod").val("15:30");
        }

        if ($("#BookPeriod").length > 0) {
            $("#DivSaveTime").css("display", "block");
        }
        else {
            $("#DivSaveTime").css("display", "none");
        }
    }
}

function GetShopType(installShopId) {
    var result = "";
    $.ajax({
        url: "/Order/JudgeShopType/",
        data: { "shopID": installShopId },
        type: "POST",
        async: false,
        success: function(shopType) {
            result = shopType;
        }
    });
    return result;
}
//保存时间button事件
function ClickSaveTimeBtn(obj, orderChannel, payStatus, orderDatetime) {
    var myDate = new Date();
    //得到系统当前时间
    var systemTime = myDate.Format("yyyy-MM-dd");
    //得到选中的时间
    var selectTime = new Date($("#BookDatetime___Calendar").val()).Format("yyyy-MM-dd");

    //如果是天猫已付款订单，就只能预约 付款时间三天内的日期(当前选择的时间-付款时间<=3)
    if (orderChannel == "4天猫" && payStatus == "2Paid") {
        var maxDate = new Date(orderDatetime).Format("yyyy-MM-dd");
        var s1 = selectTime.replace(/-/g, "/");
        var s2 = maxDate.replace(/-/g, "/");
        s1 = new Date(s1);
        s2 = new Date(s2);
        var time = s1.getTime() - s2.getTime();
        var days = parseInt(time / (1000 * 60 * 60 * 24));
        if (days > 3) {
            document.getElementById("ErrorBookDate").style.display = "block";
            document.getElementById("ErrorBookDate").innerHTML = "天猫已付款订单，就只能预约 付款时间后三天内的日期";
            return;
        }
    }

    //比较两个值，如果选中的时间小于今天的日期，提示错误信息
    var shopType = GetShopType($("#InstallShopID").val().split('|')[0]);

    //到店安装，预约安装时间，如果发快递，那么只能选择明天时间，如果是途虎物流，那么可以选择今天日期 2014-12-3 yangpeipei修改
    var shopTypeResult = true;
    switch (shopType) {
        //如果发快递，那么不能选择今天日期
        case "2Express":
            var tomorrowTime = new Date();
            tomorrowTime.setDate(tomorrowTime.getDate() + 1);
            tomorrowTime = new Date(tomorrowTime).Format("yyyy-MM-dd");
            if (selectTime < tomorrowTime) {
                document.getElementById("ErrorBookDate").style.display = "block";
                document.getElementById("ErrorBookDate").innerHTML = "请选择大于" + tomorrowTime + "的日期";
                shopTypeResult = false;
            }
            //如果到点安装，是发途虎物流，那么可以选择当前日期
        default:
            if (selectTime < systemTime) {
                document.getElementById("ErrorBookDate").style.display = "block";
                document.getElementById("ErrorBookDate").innerHTML = "请选择大于" + systemTime + "的日期";
                shopTypeResult = false;
            }
    }
    if (shopTypeResult) {
        document.getElementById("ErrorBookDate").style.display = "none";
        document.getElementById("BookDate").innerHTML = "预约日期:" + selectTime;
        var objtime = document.getElementById("BookPeriod");
        var timevalue = objtime.options[objtime.selectedIndex].text;
        document.getElementById("BookTime").innerHTML = "预约时间:" + timevalue;
        document.getElementById("BookPeriod1").value = timevalue;
        //alert(document.getElementById("BookPeriod1").value);
        //隐藏保存时间按钮
        document.getElementById("DivSaveTime").style.display = "none";
        document.getElementById("BookDatetime").value = selectTime;

        OrderChannel = document.getElementById("btnOrderChannel").value;
        DeliveryType = document.getElementById("btnDeliveryType").value;
        //alert(OrderChannel);
        //alert(DeliveryType);
        //显示DivShowPayMethod
        //显示button
        //document.getElementById("DivbtnPayMethod").style.display = "block";
        document.getElementById("DivShowPayMethod").style.display = "block";
        //获得门店信息
        var InstallShopID = $("#InstallShopID").val().split('|')[0];
        //alert(InstallShopID);
        var orderId = null;
        if ($("#PKID").length > 0) {
            orderId = $("#PKID").val();
        }
        
        LoadPayMethod();
        //$("#DivShowPayMethod").load('/Order/DisplayPayMethod/', { "OrderChannel": OrderChannel, "DeliveryType": DeliveryType, "InstallShopID": InstallShopID, "orderId": orderId });
    }
}

////保存时间button事件
//function ClickSaveTimeBtn(obj) {

//    //alert(1);
//    //隐藏自己本身，保存选中的日期和时间
//    var year = document.getElementById("BookDatetime___Year").value;
//    var mouth = document.getElementById("BookDatetime___Month").value;
//    var day = document.getElementById("BookDatetime___Day").value;

//    //获取当前的日期
//    var datenow = new Date();
//    var timeNow;
//    if (datenow.getMonth().toString().length == 1) {
//        if ((datenow.getDate() + 1).toString().length == 1) {
//            timeNow = datenow.getFullYear().toString() + "0" + (datenow.getMonth() + 1).toString() + "0" + (datenow.getDate() + 1).toString();
//        }
//        else {
//            timeNow = datenow.getFullYear().toString() + "0" + (datenow.getMonth() + 1).toString() + (datenow.getDate() + 1).toString();
//        }
//    }
//    else {
//        if ((datenow.getDate() + 1).toString().length == 1) {
//            timeNow = datenow.getFullYear().toString() + "0" + (datenow.getMonth() + 1).toString() + "0" + (datenow.getDate() + 1).toString();
//        }
//        else {
//            timeNow = datenow.getFullYear().toString() + "0" + (datenow.getMonth() + 1).toString() + (datenow.getDate() + 1).toString();
//        }
//    }

//    var timeNow1 = datenow.getFullYear().toString() + "/" + (datenow.getMonth() + 1).toString() + "/" + (datenow.getDate() + 1).toString();
//    if (mouth.length == 1) {
//        mouth = "0" + mouth;
//    }
//    if (day.length == 1) {
//        day = "0" + day;
//    }
//    var choseTime = year.toString() + mouth.toString() + day.toString();
//    var choseTime1 = year.toString() + "/" + mouth.toString() + "/" + day.toString();
//    document.getElementById("BookDatetime").value = choseTime1;

//    //如果选择的日期比今天时间前，提示错误信息
//    if ((choseTime - timeNow) < 0) {
//        document.getElementById("ErrorBookDate").style.display = "block";
//        document.getElementById("ErrorBookDate").innerHTML = "请选择大于" + timeNow1 + "的日期";

//    }

//    //否则显示选中的日期
//    else {
//        //如果错误提示信息已经存在，隐藏该div
//        document.getElementById("ErrorBookDate").style.display = "none";
//        if (document.getElementById("ErrorBookDate").innerHTML != "") {
//            document.getElementById("ErrorBookDate").innerHTML = "";
//        }
//        //显示选中的正确的日期
//        document.getElementById("BookDate").innerHTML = "预约日期:" + choseTime1;
//        //alert(1);
//        //得到日期

//        //alert(document.getElementById("BookDatetime").value);
//        //获取选中的时间
//        var objtime = document.getElementById("BookPeriod");
//        var timevalue = objtime.options[objtime.selectedIndex].text;
//        document.getElementById("BookTime").innerHTML = "预约时间:" + timevalue;
//        document.getElementById("BookPeriod1").value = timevalue;
//        //alert(document.getElementById("BookPeriod1").value);
//        //隐藏保存时间按钮
//        document.getElementById("DivSaveTime").style.display = "none";

//        OrderChannel = document.getElementById("btnOrderChannel").value;
//        DeliveryType = document.getElementById("btnDeliveryType").value;
//        //alert(OrderChannel);
//        //alert(DeliveryType);
//        //显示DivShowPayMethod
//        //显示button
//        //document.getElementById("DivbtnPayMethod").style.display = "block";
//        document.getElementById("DivShowPayMethod").style.display = "block";
//        $("#DivShowPayMethod").load('/Order/DisplayPayMethod/', { "OrderChannel": OrderChannel, "DeliveryType": DeliveryType });
//        //
//        //$(obj).parent().css("display", "none");
//    }
//}

//付款方式radio事件
function radioPayMethodClick(DicKey, DicValue) {
    //判断如果选择支付宝，那么判断外联单号是否为空
    $("#RefNo").css("border", "");
    $("#ErrorMessage").remove();
    if (DicKey == "5Alipay" && $("#RefNo").val() == "") {
        $("#RefNo").css("border", "1px solid red");
        if ($("#RefNo").next().length <= 0) {
            $("#RefNo").after("<span id='ErrorMessage' style='color: Red'>必填</span>");
        }
        $("#DivShowPayMethod").nextAll().css("display", "none");
    }
    else {
        if (DicKey != "5Alipay") {
            $("#RefNo").val("");
        }
        document.getElementById("PayMothed").value = DicKey;
        //alert(document.getElementById("PayMothed").value);
        document.getElementById("btnPayMethod").value = DicValue;
        //1.当发票按钮存在
        if ($("#DivbtnInvoiceType:visible").length) {
            document.getElementById("DivShowPayMethod").style.display = "none";
            document.getElementById("DivbtnPayMethod").style.display = "block";
        }

        else {
            //2.当发票按钮不存在时，如果发票类型已经加载了
            if ($("#DivShowInvoiceType:visible").length) {
                document.getElementById("DivShowPayMethod").style.display = "none";
                document.getElementById("DivbtnPayMethod").style.display = "block";
            }
                //当发票按钮不存在，但是发票类型已经加载了
            else {
                //alert(1);
                //显示发票类型的div
                document.getElementById("DivShowInvoiceType").style.display = "block";

                //ShowInputAllChildrenInput(document.getElementById("DivShowInvoiceType"));
                $("#DivShowInvoiceType").load('/Order/DisplayInvoiceType');
            }
        }
    }
}

//付款方式button事件
function btnPayMethodClick() {
    document.getElementById("DivbtnPayMethod").style.display = "none";
    document.getElementById("DivShowPayMethod").style.display = "block";
}

//发票类型radio事件
function radioInvoiceTypeClick(Dickey, DicValue) {
    document.getElementById("btnInvoiceType").value = DicValue;
    document.getElementById("InvoiceType").value = Dickey;
    //alert(document.getElementById("InvoiceType").value);
    //如果付款方式没有选中值
    var paymethod = document.getElementsByName("radioPayMethod");
    //如果付款方式没有选中值，提示错误信息
    if (JudgeRadioSelectStatus(paymethod) == false) {
        alert("请选择付款方式");
    }
        //如果付款方式已经选中
    else {

        //隐藏付款方式radio
        document.getElementById("DivShowPayMethod").style.display = "none";
        //显示付款方式的button 所在的div
        document.getElementById("DivbtnPayMethod").style.display = "block";
        //alert(radioInvoiceType);
        var radioInvoiceType = GetRadioValue(document.getElementsByName("radioInvoiceType"));
        //alert(radioInvoiceType);
        switch (radioInvoiceType) {
            //如果选中的是不开发票，隐藏发票radio，显示发票button                                                                                                                                                  
            case "1NoInvoice":
                //如果已经点击了普通发票或者增值税发票，隐藏点击普通发票加载出来的div"DivComIT",隐藏点击增值税发票加载出来的div"DivValueAddIT"
                if (document.getElementById("DivComIT").style.display == "block") {
                    document.getElementById("DivComIT").style.display = "none";
                }
                if (document.getElementById("DivValueAddIT").style.display == "block") {
                    document.getElementById("DivValueAddIT").style.display = "none";
                }
                //隐藏发票radio所在的div"DivShowInvoiceType"
                document.getElementById("DivShowInvoiceType").style.display = "none";
                //显示保存发票类型的按钮所在的div"DivbtnInvoiceType"
                document.getElementById("DivbtnInvoiceType").style.display = "block";

                //显示div DivComITBtnLoad
                document.getElementById("DivComITBtnLoad").style.display = "block";
                //显示创建订单按钮
                document.getElementById("DivbtnCreateOrder").style.display = "block";
                //如果点击了普通发票或者增值税发票
                //清空点击增值税保存的值
                //得到抬头的值
                document.getElementById("InvoiceTitle").value = "";
                //如果已经点击了增值税，那么清空那些值
                document.getElementById("InvTaxNum").value = "";
                //得到抬银行的值
                document.getElementById("InvBank").value = "";
                //得到银行账号的值
                document.getElementById("InvBankAccount").value = "";
                //得到金额的值
                document.getElementById("InvAmont").value = "";
                //得到地址电话的值
                document.getElementById("InvAddress").value = "";
                document.getElementById("InvoiceStatus").value = "1NoInvoice";
                break;
                //如果选中的是普通发票                                                                                                                                                    
            case "2NormalInvoice":
                //如果已经点击了无需发票或者增值税发发票，隐藏无需发票加载出来的div"DivbtnCreateOrder"
                if (document.getElementById("DivbtnCreateOrder").style.display == "block") {
                    document.getElementById("DivbtnCreateOrder").style.display = "none";
                }
                //隐藏增值税加载出来的div"DivValueAddIT"
                if (document.getElementById("DivValueAddIT").style.display == "block") {
                    document.getElementById("DivValueAddIT").style.display = "none";
                }

                //如果公司文本值为空
                //if (document.getElementById("ComIT_Title").value == "" || document.getElementById("ComIT_Title").value != "某某公司")
                //{
                //document.getElementById("ComIT_Title").value = "某某公司";
                //}
                //显示普通发票需要加载的div"DivComIT"
                document.getElementById("DivComIT").style.display = "block";
                document.getElementById("DivComIT_Title").style.display = "block";
                document.getElementById("DivComIT_Title").style.display = "block";
                document.getElementById("DivbtnComIT_Save").style.display = "block";
                document.getElementById("InvoiceStatus").value = "2NotIssue";
                break;
                //如果选中的是增值税发票                                                                                               
            case "3TaxInvoice":
                //如果已经点击无需发票，隐藏无需发票加载出来的div"DivCreateOrder"
                if (document.getElementById("DivbtnCreateOrder").style.display == "block") {
                    document.getElementById("DivbtnCreateOrder").style.display = "none";
                }
                //隐藏因点击普通发票加载的div"DivComIT"
                if (document.getElementById("DivComIT").style.display == "block") {
                    document.getElementById("DivComIT").style.display = "none";
                }
                //显示点击增值税需要加载的div"DivValueAddIT"
                document.getElementById("DivValueAddIT").style.display = "block";
                //如果已经点击了增值税发票的保存button
                document.getElementById("DivbtnValueAddIT_Save").style.display = "block";
                document.getElementById("DivbtnValueAddIT_Skip").style.display = "block";
                if (document.getElementById("ValueAddIT_Title").readonly) {
                    //隐藏保存按钮所在的div"DivbtnValueAddIT_Save"
                    document.getElementById("DivbtnValueAddIT_Save").style.display = "none";
                    //隐藏跳过按钮所在的div"DivbtnValueAddIT_Skip"
                    document.getElementById("DivbtnValueAddIT_Skip").style.display = "none";
                    //显示创建订单的按钮
                    document.getElementById("DivbtnCreateOrder").style.display = "block";
                }
                document.getElementById("InvoiceStatus").value = "2NotIssue";
                //alert(document.getElementById("InvoiceStatus").value);
                break;
        }

    }

}
//点击普通发票里面的保存button事件
function btnComIT_SaveClick() {
    /*//当普通发票公司填写错误，提示错误信息
	if (document.getElementById("ComIT_Title").value == "" || document.getElementById("ComIT_Title").value == "某某公司")
	{
	document.getElementById("ComIT_Error").style.display = "block";
	document.getElementById("ComIT_Error").innerHTML = "请输入公司名称";
	}
	else
	{*/
    //当普通发票里面填写正确的公司名称，隐藏普通发票radio div"DivShowInvoiceType"
    document.getElementById("DivShowInvoiceType").style.display = "none";
    //显示保存发票类型的button所在的div
    document.getElementById("DivbtnInvoiceType").style.display = "block";

    //隐藏因点击普通发票加载的div"DivComIT"，
    document.getElementById("DivComIT").style.display = "none";
    //填写正确的公司名称显示在div"DivComIT_SaveTitle"
    document.getElementById("DivComIT_SaveTitle").style.display = "block";
    document.getElementById("DivComIT_SaveTitle").innerHTML = "抬头:" + document.getElementById("ComIT_Title").value;
    //得到抬头的值
    document.getElementById("InvoiceTitle").value = document.getElementById("ComIT_Title").value;
    //得到抬头的值

    //如果已经点击了增值税，那么清空那些值
    document.getElementById("InvTaxNum").value = "";
    //得到抬银行的值
    document.getElementById("InvBank").value = "";
    //得到银行账号的值
    document.getElementById("InvBankAccount").value = "";
    //得到金额的值
    document.getElementById("InvAmont").value = "";
    //得到地址电话的值
    document.getElementById("InvAddress").value = "";
    //alert(document.getElementById("InvoiceTitle").value);
    //加载点击保存按钮之后需要加载的div"DivComITBtnLoad"
    document.getElementById("DivComITBtnLoad").style.display = "block";
    document.getElementById("DivbtnValueAddIT_Save").style.display = "block";
    document.getElementById("DivbtnValueAddIT_Skip").style.display = "block";

    document.getElementById("DivbtnCreateOrder").style.display = "block";
    //显示创建订单的按钮


}

//点击发票类型的button事件
function btnInvoiceTypeClick() {
    //显示发票类型radio所在的div
    document.getElementById("DivShowInvoiceType").style.display = "block";
    //隐藏该button所在的div
    document.getElementById("DivbtnInvoiceType").style.display = "none";
    //如果已经点击了无需发票，隐藏无需发票加载的div"DivbtnCreateOrder"
    if (document.getElementById("DivbtnCreateOrder").style.display == "block") {
        document.getElementById("DivbtnCreateOrder").style.display = "none";
    }
    //如果已经点了普通发票里面的按钮，隐藏点击普通发票按钮需要加载的div
    if (document.getElementById("DivComITBtnLoad").style.display == "block") {
        document.getElementById("DivComITBtnLoad").style.display = "none";
    }
    //隐藏保存公司名称的div
    if (document.getElementById("DivComIT_SaveTitle").innerHTML != "") {
        document.getElementById("DivComIT_SaveTitle").innerHTML = "";
    }
    //如果已经点击了增值税发发票，隐藏增值税加载出来的div"DivValueAddIT"
    if (document.getElementById("DivComIT_SaveTitle").style.display == "block") {
        document.getElementById("DivValueAddIT").style.display = "none";
    }
    //隐藏因点击普通发票加载的div"DivComIT"
    if (document.getElementById("DivComIT").style.display == "block") {
        document.getElementById("DivComIT").style.display = "none";
    }

}

//增值税保存button事件
function btnValueAddIT_SaveClick() {
    //判断公司名称 
    var ValueAddIT_Title = document.getElementById("ValueAddIT_Title");
    //税号
    var ValueAddIT_Tariff = document.getElementById("ValueAddIT_Tariff");
    //开户
    var ValueAddIT_OpenAccount = document.getElementById("ValueAddIT_OpenAccount");
    //账号
    var ValueAddIT_Account = document.getElementById("ValueAddIT_Account");
    //金额
    var ValueAddIT_Money = document.getElementById("ValueAddIT_Money");
    //发票地址
    var ValueAddIT_Address = document.getElementById("ValueAddIT_Address");
    //保存增值税里面填写文本
    ValueAddIT_InputChange(ValueAddIT_Title);
    //判断税号
    ValueAddIT_InputChange(ValueAddIT_Tariff);
    //判断开户
    ValueAddIT_InputChange(ValueAddIT_OpenAccount);
    //判断账号
    ValueAddIT_InputChange(ValueAddIT_Account);
    //判断金额
    ValueAddIT_InputChange(ValueAddIT_Money);
    //判断地址
    ValueAddIT_InputChange(ValueAddIT_Address);

    //当所有的值都是不是必填
    /*if ((ValueAddIT_Title.value != "必填") && (ValueAddIT_Tariff.value != "必填") && (ValueAddIT_OpenAccount.value != "必填") && (ValueAddIT_Account.value != "必填") && (ValueAddIT_Money.value != "必填") && (ValueAddIT_Address.value != "必填"))
	{
	*/

    //隐藏保存的button所在的div，
    if (document.getElementById("DivbtnValueAddIT_Save").style.display == "block") {
        document.getElementById("DivbtnValueAddIT_Save").style.display = "none";
    }
    //隐藏跳过的button所在的div
    if (document.getElementById("DivbtnValueAddIT_Skip").style.display == "block") {
        document.getElementById("DivbtnValueAddIT_Skip").style.display = "none";
    }
    document.getElementById("DivShowInvoiceType").style.display = "none";
    document.getElementById("DivbtnInvoiceType").style.display = "block";

    //加载税点，金额，安装费的值
    document.getElementById("DivComITBtnLoad").style.display = "block";
    //加载订单按钮
    document.getElementById("DivbtnCreateOrder").style.display = "block";
    //得到抬头的值
    document.getElementById("InvoiceTitle").value = ValueAddIT_Title.value;

    //得到税号的值
    document.getElementById("InvTaxNum").value = ValueAddIT_Tariff.value;
    //得到抬银行的值
    document.getElementById("InvBank").value = ValueAddIT_OpenAccount.value;
    //得到银行账号的值
    document.getElementById("InvBankAccount").value = ValueAddIT_Account.value;
    //得到金额的值
    document.getElementById("InvAmont").value = ValueAddIT_Money.value;
    //得到地址电话的值
    document.getElementById("InvAddress").value = ValueAddIT_Address.value;

}

//点击保存按钮，引发input文本的变化
function ValueAddIT_InputChange(obj) {

    //判断公司名称如果等于空，某某公司，必填提示用户必填正确的公司
    /*if (obj.value == "" || obj.value == "某某公司" || obj.value == "必填")
	{
	obj.value = "必填";
	obj.style.color = "Red";
	}
	else
	{*/
    obj.style.color = "";
    //ValueAddIT_Account.disabled = "disabled";
    obj.readonly = true;
    obj.style.border = "0px";
}
//如果创建订单的按钮出来，隐藏该button



//增值税跳过button事件
function btnValueAddIT_SkipClick() {
    //隐藏增值税需要填写的内容
    document.getElementById("DivValueAddIT").style.display = "none";
    //加载创建订单button所在的div
    document.getElementById("DivComITBtnLoad").style.display = "block";
    document.getElementById("DivbtnCreateOrder").style.display = "block";

}
//增值税里面的文本Click事件,为了修改
function ValueAddIT_InputClick(obj) {
    //如果创建订单按钮存在，那么隐藏
    if (document.getElementById("DivbtnCreateOrder").style.display == "block") {
        document.getElementById("DivbtnCreateOrder").style.display = "none";
    }
    //当点击文本修改时，如果保存按钮和跳过按钮没有，显示该button所在的div
    document.getElementById("DivbtnValueAddIT_Save").style.display = "block";
    //隐藏跳过按钮所在的div"DivbtnValueAddIT_Skip"
    document.getElementById("DivbtnValueAddIT_Skip").style.display = "block";
    /*//判断公司名称
	var ValueAddIT_Title = document.getElementById("ValueAddIT_Title");
	//税号
	var ValueAddIT_Tariff = document.getElementById("ValueAddIT_Tariff");
	//开户
	var ValueAddIT_OpenAccount = document.getElementById("ValueAddIT_OpenAccount");
	//账号
	var ValueAddIT_Account = document.getElementById("ValueAddIT_Account");
	//金额
	var ValueAddIT_Money = document.getElementById("ValueAddIT_Money");
	//发票地址
	var ValueAddIT_Address = document.getElementById("ValueAddIT_Address");
	/*switch (obj) {
	case 1:
	ValueAddIT_Title.removeAttribute("readonly");
	break;
	case 2:
	ValueAddIT_Tariff.removeAttribute("readonly");
	break;
	case 3:
	ValueAddIT_OpenAccount.removeAttribute("readonly");
	break;
	case 4:
	ValueAddIT_Account.removeAttribute("readonly");
	break;
	case 5:
	ValueAddIT_Money.removeAttribute("readonly");
	break;
	case 6:
	ValueAddIT_Address.removeAttribute("readonly");
	break;
	}*/
}

//function btnCreateOrder() {
//    var result = true;
//    if (document.getElementById("DivRef").style.display == "block") {
//        result = JudeSelectValue(document.getElementById("RefNo"));
//    }
//    //验证外联单号是否必填
//    if (result == true) {
//        if (document.getElementById("btnInvoiceType").value == "无需发票") {
//            document.getElementById("InvoiceStatus").value = "1NoInvoice";
//        }
//        else {
//            document.getElementById("InvoiceStatus").value = "2NotIssue";
//        }
//        document.getElementById("DeliveryStatus").value = "1NotStarted";
//        //alert(document.getElementById("DeliveryStatus").value);

//    }
//}
//
//无需配送里面的保存button事件
function btnNoDeliverySave(obj) {
    //获得这个button的text值
    var text = $(obj).val();
    var result = true;
    switch (text) {
        case "修改":
            $(obj).val("保存");
            $("#NoDeliveryProvince").removeAttr("disabled");
            $("#NoDeliveryCity").removeAttr("disabled");
            break;
            //如果button是保存                                                                                               
        default:
            //判断dropdownlist的值，如果是请选择，那么提示错误信息
            $("#NoDeliveryCity").css("border", "");
            $("#NoDeliveryProvince").css("border", "");
            if ($("#NoDeliveryCity").val() == null || $("#NoDeliveryCity").val() == "") {

                if ($("#NoDeliveryProvince").val() == "") {
                    $("#NoDeliveryProvince").css("border", "1px solid red");
                }
                $("#NoDeliveryCity").css("border", "1px solid red");
                result = false;
            }
            if (result) {
                $(obj).val("修改");
                $("#NoDeliveryProvince").attr("disabled", "disabled");
                //省和市的dropdownlist disabled
                $("#NoDeliveryCity").attr("disabled", "disabled");
                //加载付款方式
                //显示DivShowPayMethod
                document.getElementById("DivShowPayMethod").style.display = "block";
                //加载付款方式
                LoadPayMethod();
                //保存区域RegionID
                $("#RegionID").val($("#NoDeliveryCity").val());
                //alert($("#RegionName").val());
                break;
            }
    }
}

//加载付款方式公共方法综合                                       
function LoadPayMethod() {
    //如果付款方式已经存在，那么重新加载付款方式
    var OrderChannel = document.getElementById("OrderChannel1").value;
    var DeliveryType = document.getElementById("DeliveryType1").value;
    var InstallType = document.getElementById("InstallType").value;
    
    var installShopId = $("#InstallShopID").val().split('|')[0];
    if (document.getElementById("DivbtnPayMethod").style.display == "block") {//隐藏button
        document.getElementById("DivbtnPayMethod").style.display = "none";
        document.getElementById("DivShowPayMethod").style.display = "block";
    }
    var orderId = null;
    if ($("#PKID").length > 0) {
        orderId = $("#PKID").val();
    }
   
    $("#DivShowPayMethod").load('/Order/DisplayPayMethod/', { "OrderChannel": OrderChannel, "DeliveryType": DeliveryType, "InstallShopID": installShopId, "orderId": orderId, "InstallType": InstallType });
}

