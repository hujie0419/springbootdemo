//进入跳转页面
function getUrl(linkId, noLinkId, selectId, linkKind) {
    $(linkId).on("click",
        function() {
            var kind = $(selectId).val();
            var url = "";
            var hight = "500px";
            var routerMainLinkDiscription = $(selectId).val().split("&")[1];
            if (kind === "") {
                // alert("请选择链接种类");//注释空选项的弹出提醒
                return;
            }
            if (routerMainLinkDiscription !== "") {
                if (routerMainLinkDiscription === "美容") {
                    url = "/Router/Beauty";
                    hight = "400px";
                } else if (routerMainLinkDiscription === "保养") {
                    url = "/Router/Maintenance";
                } else if (routerMainLinkDiscription === "发现文章") {
                    url = "/Router/Find";
                    hight = "400px";
                } else if (routerMainLinkDiscription === "商品详情页") {
                    url = "/Router/ProductDetail";
                } else if (routerMainLinkDiscription === "H5页面") {
                    url = "/Router/HFive";
                } else if (routerMainLinkDiscription === "其他") {
                    url = "/Router/Other";
                    hight = "600px";
                }
            } else {
                alert("请先选择");
                return;
            }


            layer.open({
                type: 2,
                title: routerMainLinkDiscription,
                maxmin: true,
                shadeClose: true, //点击遮罩关闭层
                area: ["800px", hight],
                content: url +
                    "?routerMainLinkDiscription=" +
                    encodeURIComponent(routerMainLinkDiscription) +
                    "&linkUrl=" +
                    encodeURIComponent(decodeURIComponent($(linkId).val())) +
                    "&linkId=" +
                    linkId.id +
                    "&noLinkId=" +
                    noLinkId.id +
                    "&linkKind=" +
                    linkKind
            });


        });
}

//生成选择项
function initSelectRouter(selectId,linkKind) {
    if (selectId === "") return;
    $(selectId).empty();
    $.ajax({
        url: "/Router/GetMainLinkList",
        type: "post",
        dataType: "json",
        data:{linkKind:linkKind},
        success: function(result) {
            if (result) {
                var html = '<option value="">请选择</option>';
                for (var i = 0; i < result.length; i++) {
                    html += '<option value="' +
                        result[i].Content +
                        "&" +
                        result[i].Discription +
                        '">' +
                        result[i].Discription +
                        "</option>";
                }
                $(selectId).html(html);
            }
        },
        error: function(xmlHttpRequest, textStatus) {
            //alert("进入error！");
            alert(XMLHttpRequest.status);
            alert(XMLHttpRequest.readyState);
            alert(textStatus);
        }
    });
}

//生成未编码链接
function initDocodeLink(linkId, noLinkId) {
    if ($(linkId).val()) {
        $(noLinkId).val(decodeURIComponent($(linkId).val()));
    }
}