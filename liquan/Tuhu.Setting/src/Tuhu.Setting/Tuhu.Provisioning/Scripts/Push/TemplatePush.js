
function SetExpireTime(type) {
    var dialog = $('#expirediaglog');
    dialog.dialog({
        buttons: {
            "取消": function () {
                $(this).dialog("close");
            },
            "保存": function () {

                var day = $("#txtday").val();
                var hour = $("#txthour").val();
                var min = $("#txtmin").val();
                if (!day) {
                    day = "0";
                }
                if (!hour) {
                    hour = "0";
                }
                if (!min) {
                    min = "0";
                }
                $(this).dialog("close");
                var t = day + '/' + hour + '/' + min;
                if (type == 'ios') {
                    $("#IOSForm #ExpireTime").val(t);
                }
                else if (type == 'android') {
                    $("#AndroidForm #ExpireTime").val(t);
                }
                else if (type == 'box') {
                    $("#BoxForm #ExpireTime").val(t);
                }
            },

        }
    });
}
function addedit(type) {
    if (type == 'ios') {
        $("#androidinfo").hide();
        $("#iosinfo").show();
        $("#boxinfo").hide();
    }
    else if (type == 'android') {
        $("#androidinfo").show();
        $("#iosinfo").hide();
        $("#boxinfo").hide();
    }
    else if (type == "box") {
        $("#androidinfo").hide();
        $("#iosinfo").hide();
        $("#boxinfo").show();
    }
}
function SubmitIOS(devicetype) {

    var form = "#IOSForm";
    var type = "#IOSForm #SubmitType";
    if (devicetype == "android") {
        form = "#AndroidForm";
        type = "#AndroidForm #SubmitType";
    }
    else if (devicetype == 'box') {
        form = "#BoxForm";
        type = "#BoxForm #SubmitType";
    }
    $.ajax({
        type: "POST",
        url: "/pushmanager/IOSSubmitTemplate",
        data: $(form).serialize(),
        error: function (data) {
            $(type).val(1);
            alert(data);
        },
        success: function (data) {

            if (data.code == 1) {
                alert(data.msg);
                if (data.iscreate) {
                    location.href = '/PushManager/TemplateEdit?batchid=' + data.iscreate;
                } else {
                    if ($(type).val() == "1") {
                        location.reload();
                    }
                    $(type).val(1);
                }
            }
            else {
                alert(data.msg);
            }
        }
    });

}


$(function () {
    //$("[id=IOSExpireTime],[id=IOSPushTime],[id=AndroidPushTime],[id=AndroidExpireTime]").datetimepicker({
    $("[id=IOSPushTime],[id=AndroidPushTime],[id=BoxPushTime]").datetimepicker({
        //minDate: new Date(),
        controlType: 'select',
        dateFormat: "yy-mm-dd",
        timeFormat: 'HH:mm'
    });
    BdageChange();
});

function ResetIndex(type) {
    var length = 0;
    //var title = $("#androidextratitle" + index);
    //var value = $("#androidextravalue" + index);
    if (type == 'ios') {

        $(".iosextrakey").each(function (index, txt) {
            length++;
            console.log(index);
            console.log($(txt).val());
            $(txt).attr('name', 'ExtraKey[' + index + '].Key');
            $("#iosindex").val(index);
            console.log($(txt).parent().parent().attr("id"));
            $(txt).parent().parent().attr("id", "iosextravalue" + index);
        });
        $(".iosextravalue").each(function (index, txt) {
            $(txt).attr('name', 'ExtraKey[' + index + '].Value');
            console.log($(txt).parent().parent().prev().attr("id"));
            $(txt).parent().parent().prev().attr("id", "iosextratitle" + index);
        });

    }
    if (type == 'android') {
        $(".androidextrakey").each(function (index, txt) {
            length++;
            console.log(index);
            console.log($(txt).val());
            $(txt).attr('name', 'ExtraKey[' + index + '].Key');
            $("#androidindex").val(index);
            console.log($(txt).parent().parent().attr("id"));
            $(txt).parent().parent().attr("id", "androidextravalue" + index);
        });
        $(".androidextravalue").each(function (index, txt) {
            $(txt).attr('name', 'ExtraKey[' + index + '].Value');
            console.log($(txt).parent().parent().prev().attr("id"));
            $(txt).parent().parent().prev().attr("id", "androidextratitle" + index);
        });

    }
}

function AddExtra(type) {

    if (type == 'ios') {
        var index = $("#iosindex").val();

        var indexnum = parseInt(index) + 1;
        var title = $("#iosextratitle" + index);
        var value = $("#iosextravalue" + index);
        var temptitle = title.clone(true);
        var tempvalue = value.clone(true);
        temptitle.attr("id", "iosextratitle" + indexnum);
        tempvalue.attr("id", "iosextravalue" + indexnum);
        $($(tempvalue).children().find('input')[0]).attr('name', 'ExtraKey[' + indexnum + '].Key');
        $($(tempvalue).children().find('input')[1]).attr('name', 'ExtraKey[' + indexnum + '].Value');

        $($(tempvalue).children().find('input')[0]).val("");
        $($(tempvalue).children().find('input')[1]).val("");

        temptitle.insertBefore($("#appendto"));
        tempvalue.insertBefore($("#appendto"));
        $("#iosindex").val(indexnum);
    } else {
        console.log(2345);
        var index = $("#androidindex").val();
        var indexnum = parseInt(index) + 1;
        var title = $("#androidextratitle" + index);
        var value = $("#androidextravalue" + index);
        var temptitle = title.clone(true);
        var tempvalue = value.clone(true);
        $($(tempvalue).children().find('input')[0]).attr('name', 'ExtraKey[' + indexnum + '].Key');
        $($(tempvalue).children().find('input')[1]).attr('name', 'ExtraKey[' + indexnum + '].Value');

        $($(tempvalue).children().find('input')[0]).val("");
        $($(tempvalue).children().find('input')[1]).val("");

        temptitle.attr("id", "androidextratitle" + indexnum);
        tempvalue.attr("id", "androidextravalue" + indexnum);
        temptitle.insertBefore($("#androidappendto"));
        tempvalue.insertBefore($("#androidappendto"));
        $("#androidindex").val(indexnum);
    }
    //ResetIndex(type);
}
function DeleteExtra(btn, type) {
    if (type == 'ios') {
        if ($(".iosextrakey").length == 1) {
            alert('ff');
            return;
        }
    }
    if (type == 'android') {
        if ($(".androidextrakey").length == 1) {
            alert('ff');
            return;
        }
    }
    $(btn).parent().parent().prev().remove();
    $(btn).parent().parent().remove();
    ResetIndex(type);

}
function upImage(o, type) {
    var filePath = $(o).val();
    // 获取“.”位置
    var extStart = filePath.lastIndexOf(".");
    // 获取文件格式后缀，并全部大写
    var geshi = filePath.substring(extStart + 1, filePath.length).toUpperCase();
    if (geshi !== "JPG" && geshi !== "JPEG" && geshi !== "GIF" && geshi !== "PNG" && geshi !== "BMP") {
        alert("图片大小不能超过200k，格式支持jpg、jpeg、gif、png、bmp");
    }
    var elemid = "IOSimgUpload";
    if (type == "box") {
        elemid = "BoximgUpload";
    }
    $.ajaxFileUpload({
        url: "/PushManager/ImageUploadToAli",
        secureuri: false,
        fileElementId: elemid,
        data: { checkModel: 0 },
        async: false,
        dataType: 'text',
        success: function (result) {

            var reusltJson = JSON.parse(result);
            if (reusltJson.Msg === "上传成功") {
                var imgurl = reusltJson.FullImage;
                if (imgurl !== null && imgurl !== "") {
                    var pictureHtml = "<div algin='center' ><img src='" + imgurl + "' ></img></div>";
                    var form = "#IOSForm";
                    if (type == "android") {
                        form = "#AndroidForm";
                    }
                    else if (type == 'box') {
                        form = "#BoxForm";
                    }
                    $(form + " #BigImagePath").val(imgurl);
                    $(form + " #imgUploadUrl").attr("src", imgurl);
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

function IOSPreview(type) {
    var dialog = $('#diaglog');



    var id = "#IOSForm #SubmitType";
    if (type == "android") {
        id = "#AndroidForm #SubmitType";
    }
    dialog.dialog({
        buttons: {
            "取消": function () {
                $(this).dialog("close");
            },
            "发送预览": function () {
                var phones = $("#diaglogphpne").val();
                if (!phones) {
                    alert("手机号不能为空");
                    return;
                }
                alert(phones);
                if (type == 'ios') {
                    $('#IOSForm #SubmitType').val(0);
                    $("#IOSForm #phonepreview").val(phones);
                    SubmitIOS('ios');
                }
                else if (type == 'android') {
                    $('#AndroidForm #SubmitType').val(0);
                    $("#AndroidForm #phonepreview").val(phones);
                    SubmitIOS('android');
                }
                else if (type == 'box') {
                    $('#BoxForm #SubmitType').val(0);
                    $("#BoxForm #phonepreview").val(phones);
                    SubmitIOS('box');
                }
            },

        }
    });
};

function SendPrev() {

}

function BdageChange() {
    var type = $("#Bdagetype").val();
    if (type == "1") {
        $("#Bdage").show();
    } else {
        $("#Bdage").hide();
    }
}

function SetPlanReadyPush() {

    var BatchID = $("#BatchID").val();
    if (BatchID == '0') {
        alert("请先创建计划");
        return;
    }
    if (!confirm("您确定要保存吗？")) {
        return;
    }
    $.ajax({
        type: "POST",
        url: "/pushmanager/SetPlanReadyPush",
        data: { batchid: BatchID },
        error: function (data) {

            alert(data);
        },
        success: function (data) {

            if (data.code == 1) {
                alert("更新成功");
                location.reload();
            }
            else {
                alert(data.msg);
            }
        }
    });
}

function saveplan() {
    if (!confirm("您确定要保存吗？")) {
        return;
    }
    var iosenable = $("#iosenable").val();
    var iosid = $("#iosid").val();

    var androidenable = $("#androidenable").val();
    var androidid = $("#androidid").val();

    var boxenable = $("#boxenable").val();
    var boxid = $("#boxid").val();

    var BatchID = $("#BatchID").val();

    var PlanName = $("#PlanName").val();
    var data = {
        iosenable, iosid, androidenable, androidid, boxenable, boxid, BatchID, PlanName
    }
    $.ajax({
        type: "POST",
        url: "/pushmanager/UpdatePlanInfo",
        data: data,
        error: function (data) {

            alert(data);
        },
        success: function (data) {

            if (data.code == 1) {
                alert("更新成功");
                location.reload();
            }
            else {
                alert(data.msg);
            }
        }
    });
}


function custom_close() {
    if
    (confirm("您确定要关闭本页吗？")) {
        window.opener = null;
        window.open('', '_self');
        window.close();
    }
    else { }
}

function addintentextra() {
    var div = $("#intentkeyvalue").clone(true);
    div.attr("style", '');
    div.appendTo($("#intentapp"));
}
function deleteintentextra(btn) {
    var div = $(btn).parent();
    div.remove();
}

function showintenturi() {
    var dialog = $('#buildintent');


    dialog.dialog({
        maxWidth: 500,

        width: 500,

        buttons: {
            "取消": function () {
                $(this).dialog("close");
            },
            "提交": function () {
                var appactivity = $("#BuildActivity").val();
                var list = [];
                $(".intentkeyvalue").each(function (index, item) {
                    //$($(tempvalue).children().find('input')[0]).attr('name', 'ExtraKey[' + indexnum + '].Key');
                    //$($(tempvalue).children().find('input')[1]).attr('name', 'ExtraKey[' + indexnum + '].Value');
                    var keydev = $(item).find('input')[0];
                    var valuedev = $(item).find('input')[1];
                    var key = $(keydev).val();
                    var value = $(valuedev).val();
                    var data = {
                        key: key,
                        value: value
                    };
                    list.push(data);
                });
                console.log(list);
                $.post("/pushmanager/BuildIntentUri", { appactivity: appactivity, list: list }, function (result) {
                    console.log(result);
                    if (result.code == '1') {
                        alert("生成成功");
                        $("#intentresult").text(result.msg);
                    } else {
                        alert(result.msg);
                    }
                });
            },

        }
    });
}


function MessageBoxShowTypeChange() {
    var value = $('input[name=MessageBoxShowType]:checked').val();
    console.log(value);
}


new Vue({
    el: '#boxinfo',
    data: function () {
        return {
            MessageTypes: [],
            MessageBoxShowType: "Text",
            batchid: 0,
            ImageTextInfos: [],
            EditImageText: {
                ImageUrl: "",
                Title: "",
                Desctiption: "",
                JumpUrl: "",
                Order: 0
            }
        }
    },
    created: function () {
        var vm = this;
        var batchidinfo = $("#BatchID").val();
        if (batchidinfo && batchidinfo != "0") {
            vm.batchid = batchidinfo;
            vm.SelectTemplateImageTextInfo();
        }
        //MessageBoxShowType
        var showtype = $("#MessageBoxShowType").val();
        if (showtype) {
            vm.MessageBoxShowType = showtype;
        }
    },
    computed: {
        IsShowImageText: function () {
            var vm = this;
            return vm.MessageBoxShowType != 'Text' && vm.MessageBoxShowType != 'ActivityText' && vm.MessageBoxShowType != 'OrderText';
        },
    },
    methods: {
        upTextImage: function (EditImageText) {
            var elemid = 'textimageupload';
            var filePath = $("#" + elemid).val();
            console.log(elemid);
            // 获取“.”位置
            var extStart = filePath.lastIndexOf(".");
            // 获取文件格式后缀，并全部大写
            var geshi = filePath.substring(extStart + 1, filePath.length).toUpperCase();
            if (geshi !== "JPG" && geshi !== "JPEG" && geshi !== "GIF" && geshi !== "PNG" && geshi !== "BMP") {
                alert("图片大小不能超过200k，格式支持jpg、jpeg、gif、png、bmp");
            }
            var formData = new FormData();
            var file = $("#" + elemid)[0].files[0];
            formData.append('file', file);
            formData.append('checkModel', 0);
            $.ajax({
                url: '/PushManager/ImageUploadToAli',
                type: 'POST',
                data: formData,
                processData: false,  // tell jQuery not to process the data
                contentType: false,  // tell jQuery not to set contentType
                success: function (result) {
                    var reusltJson = JSON.parse(result);
                    console.log(reusltJson);
                    if (reusltJson.Msg === "上传成功") {
                        var imgurl = reusltJson.FullImage;
                        //template.ImageUrl = "https://resource.lylinux.net/image/2017/07/16/cv.png"; 
                        if (imgurl !== null && imgurl !== "") {
                            EditImageText.ImageUrl = imgurl;
                        }
                    } else {
                        alert("图片上传失败！" + reusltJson.Msg);
                        return;
                    }
                }
            });
        },
        EditMessageImageTextConfig: function (item) {
            var vm = this;

            vm.EditImageText = item;
            $("#textimageupload").val('');
            var dialog = $('#EditConfig');
            dialog.dialog({
                buttons: {
                    "取消": function () {
                        $(this).dialog("close");
                    },
                    "保存": function () {
                        vm.SaveImageTextConfig();
                    },
                },
                title: "图文消息编辑",
                width: "30%",
            });
        },
        DeleteMessageImageTextConfig: function (pkid) {
            var vm = this;
            $.post('/PushManager/DeleteImageTextConfigAsync', { pkid }).then(function (data) {
                alert(data.msg);
                vm.SelectTemplateImageTextInfo();
            });
        },
        AddMessageImageTextConfig: function () {
            var dialog = $('#EditConfig');
            var vm = this;
            if (vm.MessageBoxShowType == "SmallImageText") {
                if (vm.ImageTextInfos.length >= 1) {
                    alert("小图文只能添加一条");
                    return;
                }
            }
            if (vm.MessageBoxShowType == "BigImageText") {
                if (vm.ImageTextInfos.length >= 5) {
                    alert("大图文只能添加五条");
                    return;
                }
            }
            vm.EditImageText = {
                ImageUrl: "",
                Title: "",
                Desctiption: "",
                JumpUrl: "",
                Order: 0
            };
            $("#textimageupload").val('');
            dialog.dialog({
                buttons: {
                    "取消": function () {
                        $(this).dialog("close");
                    },
                    "保存": function () {
                        vm.SaveImageTextConfig();
                    },
                },
                title: "图文消息编辑",
                width: "30%",
            });
        },
        SaveImageTextConfig: function () {
            var vm = this;
            if (!vm.batchid || vm.batchid == 0) {
                alert("请先创建模版");
                return;
            }
            vm.EditImageText.BatchID = vm.batchid;
            $.post('/PushManager/LogImageTextConfigAsync', { config: vm.EditImageText, showtype:vm.MessageBoxShowType }).then(function (data) {
                if (data.code == 1) {
                    vm.SelectTemplateImageTextInfo();
                    var dialog = $('#EditConfig');
                    dialog.dialog('close');
                    location.reload();
                }
                alert(data.msg);
            });
        },
        SelectTemplateImageTextInfo: function () {
            var vm = this;
            $.get('/PushManager/SelectImageTextByBatchIdAsync', { batchid: vm.batchid }).then(function (data) {
                vm.ImageTextInfos = data;
            });
        },
    },
});