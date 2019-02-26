$(function () {
    //重新分配按钮事件
    $("#divOrderList").on("click", ".assignSubmitor", function () {
        var self = $(this);
        //alert(self.prev().val());
        var submitor = self.parent().prev().find('span').text();
        if (submitor != "") {
            if (!confirm("该订单已有制单人,确认重新分配？")) {
                return false;
            }
        } else {
            if (!confirm("确认重新分配？")) {
                return false;
            }
        }
        var pkid = self.prev().val();
        $.ajax({
            type: 'post',
            async: false,
            data: { "OrderID": pkid },
            url: '/Order/ForcedAssign',
            success: function (result) {
                if (result === true) {
                    self.closest("tr").remove();
                } else {
                    alert("重分配失败！");
                }
            }
        });
    });

 
    //分配保存事件
    $("#divOrderList").on("click", ".submitorSave", function() {
        var self = $(this);
        var pkid = self.closest('tr').attr('id');
        var submitor = self.parent().find("select").eq(0).find("option:selected").val();
        $.ajax({
            type: 'post',
            url: '/Order/UpdateOrderSubmitor',
            data: { "PKID": pkid, "submitor": submitor },
            async: false,
            success: function(result) {
                var sb = "<input class='assignSubmitor' type='button' value='分配' />";
                self.parent().prev().text(result.replace('@tuhu.cn',''));
                self.parent().html(sb);
            },
            error: function() {
                alert("分配制单人出错");
            }
        });
    });

    //分配按钮事件
    $("#divOrderList").on("click", ".forcedAssignSubmitor", function () {
    var self = $(this);
    var submitor = self.parent().prev().text().trim();
        if (submitor != "") {
            if (!confirm("该订单已有制单人,确认强制分配？")) {
                return false;
            }
        }
        self.css("display", "none");
        $.ajax({
            type: "get",
            url: "/Order/GetSystemUser",
            async: false,
            success: function(result) {
                var sb = "";
                var sbButton = "";
                var option = "";
                for (var i = 0; i < result.length; i++) {
                    option += "<option value=" + result[i].u_email_address + ">" + result[i].u_last_name + "</option>"
                }
                sb = "<select>" + option + "</select>";
                sbButton = "<input class='submitorSave' value='保存' type='button'></input>";
                self.before(sb + sbButton);
            },
            error: function() {
                alert(Error);
            }
        });
    });
})