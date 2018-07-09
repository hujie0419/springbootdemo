$(function () { // zTree configuration information, refer to API documentation (setting details)
    var setting = {
        data: {
            simpleData: {
                enable: true
            }
        },
        async: {//异步加载节点数据
            enable: true,
            url: "/Push/GetAllPushTag/"
        },
        check: {
            enable: true,
            autoCheckTrigger: false,
            chkStyle: "checkbox",//修改了这里
            radioType: "all"//还有这里，ALL标识整个Tree只准选中一个节点
        }

    }
    var zTreeObj = $.fn.zTree.init($("#treeTag"), setting);

    $("[id=SendTime],[id=ExpireTime],[id=APPExpireTime]").datetimepicker({
        //minDate: new Date(),
        controlType: 'select',
        dateFormat: "yy-mm-dd",
        timeFormat: 'HH:mm'
    });

    $("[name=AfterOpen]").on("change", function () {
        if (this.value == 0) {
            $("#AppActivity").hide();
            $("#AppActivity").val("");
        }
		else
		    $("#AppActivity").show();
    });

    $("[name=PushType]").on("change", function () {
        if (this.value == 2) {
            $("#divTag").hide();
            $("#divFile").show();
        } else if (this.value == 3) {
            $("#divTag").show();
            $("#divFile").hide();
            $("#FilePhones").val("");
        }
        else {
            $("#divTag").hide();
            $("#divFile").hide();
            $("#FilePhones").val("");
        }
    });

    $("#CheckBadge").on("change", function() {
        if ($("#CheckBadge").prop("checked")) {
            $("#IOSShowBadge").val("true");
        } else {
            $("#IOSShowBadge").val("false");
        }
    });
});


function upImage(o) {
    var filePath = $(o).val();
    // 获取“.”位置
    var extStart = filePath.lastIndexOf(".");
    // 获取文件格式后缀，并全部大写
    var geshi = filePath.substring(extStart + 1, filePath.length).toUpperCase();
    if (geshi !== "JPG" && geshi !== "JPEG" && geshi !== "GIF" && geshi !== "PNG" && geshi !== "BMP") {
        alert("图片大小不能超过20M，格式支持jpg、jpeg、gif、png、bmp");
    }

    $.ajaxFileUpload({
        url: "/Push/ImageUploadToAli",
        secureuri: false,
        fileElementId: "imgUpload",
        data: { checkModel: 0 },
        async: false,
        dataType: 'text',
        success: function (result) {
            var reusltJson = JSON.parse(result);
            if (reusltJson.Msg === "上传成功") {
                var imgurl = reusltJson.FullImage;
                if (imgurl !== null && imgurl !== "") {
                    var pictureHtml = "<div algin='center' ><img src='" + imgurl + "' ></img></div>";
                    $("#BigImagePath").val(imgurl);
                    $("#imgUploadUrl").attr("src", imgurl);
                }
            } else {
                alert("图片上传失败！" + reusltJson.Msg);
                return;
            }
        },
        error: function (e, t, x) {
        }
    });
}
function upImageR(o) {
    var filePath = $(o).val();
    // 获取“.”位置
    var extStart = filePath.lastIndexOf(".");
    // 获取文件格式后缀，并全部大写
    var geshi = filePath.substring(extStart + 1, filePath.length).toUpperCase();
    if (geshi !== "JPG" && geshi !== "JPEG" && geshi !== "GIF" && geshi !== "PNG" && geshi !== "BMP") {
        alert("图片大小不能超过20M，格式支持jpg、jpeg、gif、png、bmp");
    }

    $.ajaxFileUpload({
        url: "/Push/ImageUploadToAli",
        secureuri: false,
        fileElementId: "RichTextImgUpload",
        data: { checkModel: 1 },
        async: false,
        dataType: 'text',
        success: function (result) {
            var reusltJson = JSON.parse(result);
            if (reusltJson.Msg === "上传成功") {
                var imgurl = reusltJson.FullImage;
                if (imgurl !== null && imgurl !== "") {
                    var pictureHtml = "<div algin='center' ><img src='" + imgurl + "' ></img></div>";
                    $("#RichTextImage").val(imgurl);
                    $("#RichTextImgUploadUrl").attr("src", imgurl);
                }
            } else {
                alert("图片上传失败！" + reusltJson.Msg);
                return;
            }
        },
        error: function (e, t, x) {
        }
    });
}

$(document.forms["PushMessage"]).validate({
    rules: {
        Description: "required",
        Title: "required",
        Content: "required",
      
        FilePhones: {
            accept: ".txt"
        }
    },
    messages: {
        Description: "请输入描述",
        Title: "请输入标题",
        Content: "请输入内容",
        
        FilePhones: {
            required: "请选择一个文本文件",
            accept: "请选择一个文本文件"
        }
    },
    submitHandler: function (form) {
        if ($("input[name='AfterOpen']:checked").val() == 1 && $("#AppActivity").val() == "") {
            alert("请输入指定页面的Activity");
            return;
        }

        if (($("#AndriodKey1").val() != "" && $("#AndriodValue1").val() == "")
            || ($("#AndriodKey2").val() != "" && $("#AndriodValue2").val() == "")
            || ($("#IOSKey1").val() != "" && $("#IOSValue1").val() == "")
            || ($("#IOSKey2").val() != "" && $("#IOSValue2").val() == "")) {
            alert("请输入对应给出自定义key值的Value值");
            return;
        }

        if ($("input[name='PushType']:checked").val() == 2 && $("#FilePhones").val() == "") {
            alert("请上传手机号");
            return;
        }

        var tagDisplay = new Array();
        var tagKeys = new Array();
        if ($("input[name='PushType']:checked").val() == 3) {
            var seletedTags = $.fn.zTree.getZTreeObj("treeTag");
            if (seletedTags) {
                var tags = seletedTags.getCheckedNodes();
                if (tags) {
                    for (var i = 0; i < tags.length; i++) {
                        if (tags[i] && !tags[i].isParent && tags[i].key) {
                            tagDisplay.push(tags[i].name);
                            tagKeys.push(tags[i].key);
                        }
                    }
                }
            }

            if (tagKeys && tagKeys.length > 0) {
                $("#Tags").val(tagKeys.join(";"));
            } else {
                alert("请选择标签！");
                return;
            }
        }
       

        form = $(form);

        var promptText = "确定创建吗？";

        if ($("input[name='PushType']:checked").val() == 1) {
            promptText = "确定创建【广播】吗？";
        }
        else if ($("input[name='PushType']:checked").val() == 2) {
            promptText = "确定创建【单播】吗？";
        }
        else if ($("input[name='PushType']:checked").val() == 3) {
            promptText = "确定创建【组播】吗？\nTag：";
            var x;
            for (x in tagDisplay) {
                if (tagDisplay.hasOwnProperty(x)) {
                    promptText += "\n【" + tagDisplay[x] + "】";
                }
            }
        }

        if (confirm(promptText)) {
            $("#submit").attr("disabled", "disabled").next().css("visibility", "visible");
            var success = function (result) {
                $("#submit").removeAttr("disabled").next().css("visibility", "hidden");
                if (result > 0) {
                    alert("操作成功，已放入推送队列！");
                }
                else
                    switch (result) {
                        case 0:
                            alert("没有有效的手机号！");
                            break;
                        case -1:
                            alert("缺失必填参数错误！");
                            break;
                        case -2:
                            alert("过期时间设置有误！");
                            break;
                        case -3:
                            alert("请输入Activity！");
                            break;
                        case -5:
                            alert("上传文件失败！");
                        case -6:
                            alert("请选择标签！");
                            break;
                        default:
                            alert("未知错误：" + result.Result);
                            break;
                    }
            }
            form.ajaxSubmit({
                error: function (ajax) {
                    $("#submit").removeAttr("disabled").next().css("visibility", "hidden");
                    if (ajax.status == 200)
                        success(JSON.parse(ajax.responseText));
                    else if (ajax.status > 0) {
                        alert("创建失败！");
                    }
                },
                success: success
            });
        }
    }
});