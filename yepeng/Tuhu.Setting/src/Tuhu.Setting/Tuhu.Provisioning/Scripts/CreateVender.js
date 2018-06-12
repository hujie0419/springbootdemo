$(function () {
    $.ajax({
        cache: false,
        type: "get",
        url: "/PurchaseOrder/GetTreeForCooperativeBrand",
        contenttype: "application/json;charset=utf-8",
        datatype: "json",
        success: function (data) {
            var dt = eval('(' + data + ')');
            $("#CooperativeBrandTree").dynatree({
                checkbox: true,
                selectMode: 3,
                children: dt
            });
        }
    });

    $.ajax({
        cache: false,
        type: "get",
        url: "/PurchaseOrder/GetTreeForVenderShipableWarehouse",
        dataType: "json",
        success: function(data) {
            var dt = eval('(' + data + ')');
            $("#ShipableWareHouseTree").dynatree({
                checkbox: true,
                selectMode: 3,
                children: dt
            });
        }
    });
});

function OpenCooperativeBrand() {
    $("#CooperativeBrandList").css("display", "block");
}

function OpenShipableWareHouse() {
    $("#ShipableWareHouseList").css("display", "block");
}
function CloseCooperativeBrand() {
    $("#CooperativeBrandList").css("display", "none");
}

function CloseShipableWareHouse() {
    $("#ShipableWareHouseList").css("display", "none");
}

function TreeOKForCooperativeBrand() {
    var nodes = $("#CooperativeBrandTree").dynatree("getSelectedNodes");
    var txt = "";
    for (var i = 0; i < nodes.length; i++) {
        if (nodes[i].data.id == "0") {
            txt += "," + nodes[i].data.title;
        }
        var flag = 0;
        if (nodes[i].data.id != "0") {
            var all = txt.split(',');
            for (var j = 0; j < all.length; j++) {
                if (all[j] == nodes[i].data.name) {
                    flag = 1;
                }
            }
            if (flag == 0) {
                txt += "," + nodes[i].data.name;
            }
            var father = $("#CooperativeBrandTree").dynatree("getTree").getNodeByKey(nodes[i].data.id);
            var fatitle = father.data.title;
            var allChildren = father.getChildren();
            var countChildren = father.countChildren();
            var f = 1;
            var str = nodes[i].data.name;
            for (var m = 0; m < countChildren; m++) {
                if (allChildren[m].isSelected() == false) {
                    if (f == 1) {
                        str += "（不包含：";
                    }
                    str += allChildren[m].data.title + ",";
                    f++;
                }
            }
            if (f > 1) {
                str = str.substring(0, str.length - 1);
                str += ")";
            }
            txt = txt.replace(nodes[i].data.name, str);
            i += countChildren - f;
        }
    }

    $("#CooperativeBrand").val(txt.substring(1));
    $("#CooperativeBrandList").css("display", "none");
}

function TreeOKForShipableWareHouse() {
    var nodes = $("#ShipableWareHouseTree").dynatree("getSelectedNodes");

    var txtForWareHouse = "";
    var txtForShipableWareHouse = "";

    if (nodes != null) {
        for (var i = 0; i < nodes.length; i++) {
            txtForWareHouse += "," + nodes[i].data.title;
            txtForShipableWareHouse += "{\"WarehouseId\":\"" + nodes[i].data.key + "\",\"Warehouse\":\"" + nodes[i].data.title + "\"},";
        }

        if (txtForShipableWareHouse.length > 0) {
            txtForShipableWareHouse = txtForShipableWareHouse.substring(0, txtForShipableWareHouse.length - 1);
        }
    }

    $("#ShipableWareHouseData").val("[" + txtForShipableWareHouse + "]");
    $("#ShipableWareHouse").val(txtForWareHouse.length> 1 ? txtForWareHouse.substring(1) : "");
    $("#ShipableWareHouseList").css("display", "none");
}