var loading = '<div style="width:100%; display:grid;"><span style="margin-left:auto; margin-right:auto;"><img src="/Images/loading.gif" />玩命加载中，请稍候...</span></div>';
var nodata = '<div style="width:100%;display:grid;"><span style="margin-left:auto; margin-right:auto;">暂无数据</span></div>';

var Func = {
    LookSaleNum: function (o) {
        var item = $(o);
        var pid = item.parent().parent().find("td").eq(1).text();
        var name = item.parent().parent().find("td").eq(4).text();
        $.get("GetStock", { pid: pid }, function (data) {
            if (data.Status && data.data.length) {
                $("#SaleNumItem .pid").text(pid);
                $("#SaleNumItem .name").text(name);
                var html = '<tr><th>仓库</th><th>库存数量</th><th>近90天销量</th><th>近30天销量</th><th>近7天销量</th>/tr>';
                html += '';
                $.each(data.data, function (i, v) {
                    html += '<tr>';
                    html += '<td>' + v.WareHouseName + '</td>';
                    html += '<td>' + (v.TotalStock == null ? '-' : v.TotalStock) + '</td>';
                    html += '<td>' + (v.Num_ThreeMonth == null ? '-' : v.Num_ThreeMonth) + '</td>';
                    html += '<td>' + (v.Num_Month == null ? '-' : v.Num_Month) + '</td>';
                    html += '<td>' + (v.Num_Week == null ? '-' : v.Num_Week) + '</td>';
                    html += '</tr>';
                });
                $("#SaleNumItem table").html(html);
                $("#SaleNumItem").dialog({
                    title: "销量明细",
                    width: 450,
                    height: 500,
                    modal: true,
                });
            }
            else
                alert("无分仓信息！");
        });
    },
    LookShopNum: function (o) {
        var item = $(o);
        var pid = item.parent().parent().find("td").eq(1).text();
        var name = item.parent().parent().find("td").eq(4).text();
        $.get("GetBaoYangShopStockByPid", { pid: pid }, function (data) {
            if (data.data.length) {
                $("#ShopNumItem .pid").text(pid);
                $("#ShopNumItem .name").text(name);
                var html = '<tr><th>工场店</th><th>库存数量</th><th>近90天销量</th><th>近30天销量</th><th>近7天销量</th>/tr>';
                html += '';
                $.each(data.data, function (i, v) {
                    html += '<tr>';
                    html += '<td>' + v.ShopName + '</td>';
                    html += '<td>' + (v.StockQuantity == null ? '-' : v.StockQuantity) + '</td>';
                    html += '<td>-</td>';
                    html += '<td>' + (v.SaleNum == null ? '-' : v.SaleNum) + '</td>';
                    html += '<td>-</td>';
                    html += '</tr>';
                });
                $("#ShopNumItem table").html(html);
                $("#ShopNumItem").dialog({
                    title: "销量明细",
                    width: 450,
                    height: 500,
                    modal: true,
                });
            }
            else
                alert("无分工场店信息！");
        });
    },
    ShowZXTPrice: function (pid) {
        $("#ZXTPrice").load('/BaoYangPriceGuide/ZXTPrice?pid=' + pid);
        $("#ZXTPrice").dialog({
            title: "保养品(" + pid + ")价格变动",
            width: 1200,
            height: 700,
            modal: true
        });
    },
    ShowZXTCost: function (pid) {
        $("#ZXTCost").load('/BaoYangPriceGuide/ZXTCost?pid=' + pid);
        $("#ZXTCost").dialog({
            title: "保养品(" + pid + ")近一年采购历史",
            width: 1200,
            height: 700,
            modal: true
        });
    },

    UpdatePrice: function (pid, guidePrice, price, upperLimit, lowerLimit, marketingPrice) {
        $("#UpdatePrice .guidePrice").text(guidePrice == '-' ? '-' : guidePrice + '元');
        $("#UpdatePrice .price").val(price);
        $("#UpdatePrice .xiangbi").text(guidePrice == '-' ? '-' : Func.formatCurrency(price - guidePrice));
        $("#UpdatePrice .jyprice").text(guidePrice == '-' ? '-' : Math.round((guidePrice - parseFloat(lowerLimit)) * 100) / 100 + "-" + Math.round((parseFloat(guidePrice) + parseFloat(upperLimit)) * 100) / 100);
        var guide_temp = guidePrice;
        $("#UpdatePrice .price").keyup(function () {
            var $parent = $(this).parent().parent();
            var price_1 = $parent.find(".price").val();

            if (guide_temp != '-' && !isNaN(parseFloat(guide_temp))) {
                if (!isNaN(price_1)) {
                    $("#UpdatePrice .xiangbi").text(Func.formatCurrency(price_1 - guide_temp));
                } else
                    $("#UpdatePrice .xiangbi").text('-');
            } else {
                $("#UpdatePrice .xiangbi").text('-');
            }
        });
        $("#UpdatePrice").dialog({
            title: "修改官网价格",
            width: 300,
            height: 300,
            modal: true,
            buttons: {
                "保存": function () {
                    var dialog = $(this);
                    var price_t = dialog.find(".price").val();
                    if (isNaN(price_t)) {
                        alert("价格格式有误");
                        return false;
                    }
                    else {
                        price_t = parseFloat(price_t);
                        if (price_t < 0) {
                            alert("价格格式有误");
                            return false;
                        }
                        //$.post("IsLowerThanActivityPrice", { pid: pid, price: price_t }, function (result) { 
                        if (price_t >= parseFloat(marketingPrice)) {
                            alert("该价格大于或等于市场价,请先修改市场价");
                            return false;
                        }  
                        //if (result > 0) {
                        if (price > 0) {
                            if (guidePrice == '-') {
                                Func.ApplyUpdatePrice(dialog, pid, price_t);
                            }
                            else {
                                if ((parseFloat(guidePrice) + parseFloat(upperLimit) < price_t) || (guidePrice - parseFloat(lowerLimit) > price_t)) {
                                    Func.ApplyUpdatePrice(dialog, pid, price_t);
                                }
                                else
                                    Func.UpdateListPrice(dialog, pid, guidePrice, price_t);
                            }
                        }
                        else
                            Func.UpdateListPrice(dialog, pid, price, price_t);
                        //}
                        //else
                        // alert("价格低于或等于活动价格，不允许保存！");
                        //});

                    }
                },
                "关闭": function () {
                    $(this).dialog("close"); //当点击取消按钮时，关闭窗口
                }
            }
        });
    },
    ApplyUpdatePrice: function (dialogold, pid, price) {
        $("#ApplyUpdatePrice .reason").val("");
        $("#ApplyUpdatePrice").dialog({
            title: "申请修改价格",
            width: 300,
            height: 300,
            modal: true,
            buttons: {
                "保存": function () {
                    var dialog = $(this);
                    var reason = dialog.find(".reason").val().trim();
                    if (reason == null || reason == "") {
                        alert("请填写申请理由");
                        return;
                    }
                    $.post("ApplyUpdatePrice", { PID: pid, ApplyPrice: price, ApplyReason: reason }, function (result) {
                        if (result > 0) {
                            alert("申请成功");
                            dialog.dialog("close");
                            dialogold.dialog("close");
                        }
                        else
                            alert("申请失败");
                    });
                },
                "关闭": function () {
                    $(this).dialog("close"); //当点击取消按钮时，关闭窗口
                }
            }
        });
    },
    UpdateListPrice: function (dialog, pid, originPrice, price) {
        $.post("UpdateListPrice", { pid: pid, originPrice: originPrice, price: price }, function (result) {
            if (result > 0) {
                alert("修改成功");
                dialog.dialog("close");
                Func.LoadList($(".list .pager>a.current").text());
            }
            else if (result == -2) {
                alert("PID不存在");
            }
            else if (result == -3) {
                alert("价格未变无需修改");
            }
            else if (result == -4) {
                alert("修改价格超出范围");
            }
            else if (result == -5) {
                alert("原价不在预警线价格范围内");
            }
            else {
                alert("修改失败");
            }
        });
    },
    formatCurrency: function (num) {
        //num = num.toString().replace(/\$|\,/g, '');
        //if (isNaN(num))
        //    num = "0";
        //sign = (num == (num = Math.abs(num)));
        //num = Math.floor(num * 100 + 0.50000000001);
        //cents = num % 100;
        //num = Math.floor(num / 100).toString();
        //if (cents < 10)
        //    cents = "0" + cents;
        //for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3) ; i++)
        //    num = num.substring(0, num.length - (4 * i + 3)) + ',' +
        //    num.substring(num.length - (4 * i + 3));
        //return (((sign) ? '' : '-') + num + '.' + cents);
        if (isNaN(num) || num == 0) {
            num = "0";
        } else {
            if (num < 0) {
                var f = parseFloat(-num);
                num = "少" + Math.round(f * 100) / 100 + "元";
            } else {
                var x = parseFloat(num);
                num = "多" + Math.round(x * 100) / 100 + "元";
            }
        }
        return num;
    },
    LookFlashSalePrice: function (pid) {
        $.get("/BaoYangPriceGuide/GetFlashSalePriceByPid", { pid: pid }, function (data) {
            if (data && data.length) {
                var html = '<table>';
                html += '<tr><th>PID</th><th>活动价格</th><th>活动名称</th><th>活动ID</th><th>操作</th></tr>';
                $.each(data, function (index, value) {
                    html += '<tr><td>' + value.PID + '</td><td>' + value.Price + '</td><td>' + value.DisplayName + '</td><td>' + value.ActivityID + '</td><td><a target="_blank" href="http://setting.tuhu.cn/QiangGou/Detail?aid=' + value.ActivityID + '">去修改活动价</a></td></tr>';
                });
                html += '</table>';
                $("#FlashSaleShow").html(html);
                $("#FlashSaleShow").dialog({
                    title: pid + "的活动价格",
                    width: 600,
                    height: 500,
                    modal: true,
                });

            }
            else
                alert("该PID暂未参与限时抢购！");
        });
    }
}