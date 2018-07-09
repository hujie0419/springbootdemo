define('push-api', function () {
    return {
        QuerySinglePushTemplateAsync: function (query) {
            return $.post('/PushManager/QuerySinglePushTemplateAsync', { query }).then(function (data) {
                return data;
            });
        },
        SelectTemplatesByBatchIDAsync: function (batchid) {
            return $.post('/PushManager/SelectTemplatesByBatchIDAsync', { batchid }).then(function (data) {
                return data;
            });
        },
        SubmitSingleTemplateAsync: function (model, ispreview, targets, planname, TemplateTag) {
            //return $.post('/PushManager/SubmitSingleTemplateAsync', { model, ispreview, targets })
            //    .then(function (data) {
            //        return data;
            //    });
            return $.ajax({
                url: '/PushManager/SubmitSingleTemplateAsync',
                type: 'POST',
                data: JSON.stringify({
                    model, ispreview, targets, planname, TemplateTag
                }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    return data;
                }
            });
        },
        SelectTemplatePlanInfoAsync: function (batchid) {
            return $.get('/PushManager/SelectTemplatePlanInfoAsync', { batchid }).then(function (data) {
                return data;
            });
        },
        SelectAllWxAppConfigAsync: function () {
            return $.get('/PushManager/SelectAllWxAppConfigAsync').then(function (data) {
                return data;
            });
        },
        SelectAllWxAppTemplatesAsync: function (platform) {
            return $.get('/PushManager/SelectAllWxAppTemplatesAsync', { platform }).then(function (data) {
                return data;
            });
        },
        SelectAllMessageTypes: function () {
            return $.get('/PushManager/SelectMessageNavigationTypesAsync').then(function (data) {
                return data;
            });
        },
        SelectImageTextByBatchId: function (batchid) {
            return $.get('/PushManager/SelectImageTextByBatchIdAsync', { batchid }).then(function (data) {
                return data;
            });
        },
        LogImageTextConfigAsync: function (config, showtype) {
            return $.post('/PushManager/LogImageTextConfigAsync', { config, showtype }).then(function (data) {
                return data;
            });
        },
        DeleteImageTextConfigAsync: function (pkid) {
            return $.post('/PushManager/DeleteImageTextConfigAsync', { pkid }).then(function (data) {
                return data;
            });
        },
    }
});

define('vue-push-single', ['push-api'], function (api) {
    return new Vue({
        el: '#SinglePush',
        data: function () {
            return {
                BatchID: "",
                PlanName: "",
                DeviceType: "",
                IsEnable: "",
                status: {
                    loading: false
                },
                Templates: [],
                Pager: {},
                JumpPageIndex: 0,
                CreateStartTime: "",
                CreateEndTime: "",
                CreateUser: ""
            }
        },
        computed: {
            query: function () {
                var vm = this;
                return {
                    BatchID: vm.BatchID,
                    PlanName: vm.PlanName,
                    DeviceType: vm.DeviceType,
                    IsEnable: vm.IsEnable,
                    PageIndex: vm.Pager.PageIndex,
                    CreateStartTime: vm.CreateStartTime,
                    CreateEndTime: vm.CreateEndTime,
                    CreateUser: vm.CreateUser
                }
            },
            //JumpPageIndex:function() {
            //    return this.Pager.PageIndex;
            //}
        },
        methods: {
            NextPage: function () {
                var vm = this;
                if (vm.Pager.PageIndex < vm.Pager.PageCount) {
                    vm.Pager.PageIndex += 1;
                    vm.JumpPageIndex = vm.Pager.PageIndex;
                    vm.Search();
                }
            },
            PreviousPage: function () {
                var vm = this;
                if (vm.Pager.PageIndex > 1) {
                    vm.Pager.PageIndex -= 1;
                    vm.JumpPageIndex = vm.Pager.PageIndex;
                    vm.Search();
                }
            },
            PageJump: function () {
                var vm = this;
                var index = parseInt(vm.JumpPageIndex);
                if (index) {
                    if (index <= vm.Pager.PageCount && index >= 1) {
                        vm.query.PageIndex = index;
                        vm.Search();
                        return;
                    }
                }
                alert("页码非法");
                return;
            },
            DoSearch: function () {
                var vm = this;
                vm.query.PageIndex = 1;
                vm.Search();
            },
            Search: function () {
                var vm = this;
                vm.status.loading = true;
                if (vm.query.IsEnable) {
                    if (vm.query.IsEnable == "0") {
                        vm.query.PushStatus = "4";
                    } else {
                        vm.query.PushStatus = "6";
                    }
                }
                api.QuerySinglePushTemplateAsync(vm.query).then(function (data) {
                    console.log(data);
                    vm.status.loading = false;
                    if (data.code == 1) {
                        vm.JumpPageIndex = data.Pager.PageIndex;
                        vm.Pager = data.Pager;
                        vm.Templates = data.Templates;
                    }
                });
            },
        },
        created: function () {
            var vm = this;
            vm.Search();
        },
    })
});



define('vue-edit-single-push', ['push-api'], function (api) {
    return new Vue({
        el: '#EditSinglePushTemplate',
        data: function () {
            return {
                Templates: [],
                //iostemplate: {},
                //androidtemplate: {},
                //messageboxtemplate: {},
                //wechattemplate: {},
                ShowDeviceType: 1,
                WeiXinTemplates: [],
                batchid: 0,
                planstatus: '',
                planname: '',
                SelectWxTemplateID: '',
                SelectWxAppTemplateID: '',
                WxAppConfigs: [],
                WxAppTemplates: [],
                WxAppPlatform: 5,
                ShowWxTemplates: false,
                WxTemplateTitle: "",
                WxTemplateContent: "",
                WxAppPlatform: 5,
                WxAppPlatformForWxOpen: 5,
                WxTemplateColors: [],
                WxAPPTemplateColors: [],
                WxAppEmKeyWord: "",
                CanSave: true,
                MessageTypes: [],
                MessageBoxShowType: "Text",
                ImageTextInfos: [],
                AllTemplateTags: [],
                TemplateTag: "",
                EditImageText: {
                    ImageUrl: "",
                    Title: "",
                    Desctiption: "",
                    JumpUrl: "",
                    Order: 0
                }
            };
        },
        computed: {
            SelectWxTemplate: function () {

                var vm = this;
                var ismodify = false;
                var wxpushtemplate = vm.Templates.filter(function (x) { return x.DeviceType == 4 });
                if (wxpushtemplate && wxpushtemplate.length) {

                    var id = wxpushtemplate[0].WxTemplateID;
                    if (vm.SelectWxTemplateID != id) {
                        ismodify = true;
                    }
                    if (!wxpushtemplate[0].ContentDesc || !wxpushtemplate[0].ContentDesc.length) {
                        ismodify = true;
                    }
                    vm.SelectWxTemplateID = wxpushtemplate[0].WxTemplateID;

                    if (vm.WeiXinTemplates) {
                        var temp = vm.WeiXinTemplates.filter(function (x) { return x.template_id == id });

                        if (temp && temp.length) {
                            if (ismodify) {

                                wxpushtemplate[0].ContentDesc = [];
                                temp[0].keys.forEach(function (k) {
                                    if (k != "{{first.DATA}}" && k != "{{remark.DATA}}") {
                                        wxpushtemplate[0].ContentDesc.push({
                                            Key: k,
                                            Value: ''
                                        });
                                    }
                                });
                            }
                            wxpushtemplate[0].Title = temp[0].title;
                            wxpushtemplate[0].Content = temp[0].content;
                            var keys = temp[0].keys;
                            vm.WxTemplateColors = keys.map(function (x) {
                                var color = '#173177';
                                var colorinfo = wxpushtemplate[0].WxTemplateColorDict.filter(function (s) { return s.Key == x });
                                if (colorinfo && colorinfo.length) {
                                    color = colorinfo[0].Value;
                                }
                                return {
                                    Key: x,
                                    Value: color
                                };
                            });

                            return temp[0];
                        }
                    }
                }
            },
            SelectWxAPPTemplate: function () {
                var vm = this;
                var ismodify = false;

                var wxpushtemplate = vm.Templates.filter(function (x) { return x.DeviceType == 6 });
                if (wxpushtemplate && wxpushtemplate.length) {

                    var id = wxpushtemplate[0].WxTemplateID;
                    if (vm.SelectWxAppTemplateID != id) {
                        ismodify = true;
                    }
                    if (!wxpushtemplate[0].ContentDesc || !wxpushtemplate[0].ContentDesc.length) {
                        ismodify = true;
                    }
                    vm.SelectWxAppTemplateID = wxpushtemplate[0].WxTemplateID;

                    if (vm.WxAppTemplates) {
                        var temp = vm.WxAppTemplates.filter(function (x) { return x.template_id == id });

                        if (temp && temp.length) {
                            if (ismodify) {

                                wxpushtemplate[0].ContentDesc = [];
                                temp[0].keys.forEach(function (k) {
                                    if (k != "{{first.DATA}}" && k != "{{remark.DATA}}") {
                                        wxpushtemplate[0].ContentDesc.push({
                                            Key: k,
                                            Value: ''
                                        });
                                    }
                                });
                            }
                            wxpushtemplate[0].Title = temp[0].title;
                            wxpushtemplate[0].Content = temp[0].content;
                            var keys = temp[0].keys;
                            vm.WxAPPTemplateColors = keys.map(function (x) {
                                var color = '#173177';
                                var colorinfo = wxpushtemplate[0].WxTemplateColorDict.filter(function (s) { return s.Key == x });
                                if (colorinfo && colorinfo.length) {
                                    color = colorinfo[0].Value;
                                }
                                return {
                                    Key: x,
                                    Value: color
                                };
                            });

                            return temp[0];
                        }
                    }
                }
                return {};
            },
            NotSelectedWxAppTemplates: function () {
                var vm = this;

                var selecttemplate = vm.SelectWxAPPTemplate;
                if (selecttemplate) {
                    var id = selecttemplate.template_id;
                    return vm.WxAppTemplates.filter(function (x) {
                        return x.template_id != id;
                    })
                } else {
                    return vm.WxAppTemplates;
                }
            },
            NotSelectedWxTemplates: function () {
                var vm = this;
                var selecttemplate = vm.SelectWxTemplate;
                if (selecttemplate) {
                    var id = selecttemplate.template_id;
                    return vm.WeiXinTemplates.filter(function (x) {
                        return x.template_id != id;
                    })
                }
            },

            WxTemplateContentDesc: function () {
                var vm = this;
                var wxpushtemplate = vm.Templates.filter(function (x) { return x.DeviceType == 4 });
                if (wxpushtemplate && wxpushtemplate.length) {
                    var template = wxpushtemplate[0];
                    return template.ContentDesc.filter(function (x) {
                        return x.Key != "{{first.DATA}}" && x.Key != "{{remark.DATA}}";
                    });
                }
            },
            MessageBoxIsImageTemplate: function () {
                var vm = this;
                return vm.MessageBoxShowType == 'BigImageText' || vm.MessageBoxShowType == 'SmallImageText';
            },
        },
        created: function () {
            var vm = this;
            var batchid = $("#batchid").val();
            batchid = batchid ? batchid : "";
            console.log(batchid);

            api.SelectTemplatesByBatchIDAsync(batchid).then(function (data) {
                if (data.code == 1) {
                    var templates = data.datas;
                    var weixintemplates = data.WeiXinTemplates;
                    vm.WeiXinTemplates = weixintemplates;
                    templates = templates.map(function (item) {
                        //item.IsEnable = item.PushStatus == 4 ? "0" : "1";
                        switch (item.DeviceType) {
                            case 1:
                                //vm.iostemplate = item;
                                item.PushTypeDesc = 'iOS推送';
                                break;
                            case 2:
                                //vm.androidtemplate = item;
                                item.PushTypeDesc = '安卓推送';
                                break;
                            case 3:
                                //vm.messageboxtemplate = item;
                                item.PushTypeDesc = 'APP消息';
                                vm.MessageBoxShowType = item.MessageBoxShowType;
                                break;
                            case 4:
                                //vm.wechattemplate = item;
                                if (item.BoxID) {
                                    vm.WxAppPlatformForWxOpen = item.BoxID;
                                }
                                vm.SelectWxTemplateID = item.WxTemplateID;
                                item.PushTypeDesc = '微信公众号模版消息';

                                var finds1 = item.ContentDesc.filter(function (s) { return s.Key == "{{first.DATA}}" });
                                if (finds1 && finds1.length) {
                                    vm.WxTemplateTitle = finds1[0].Value;
                                } else {
                                    vm.WxTemplateTitle = "";
                                }
                                var finds2 = item.ContentDesc.filter(function (s) { return s.Key == "{{remark.DATA}}" });
                                if (finds2 && finds2.length) {
                                    vm.WxTemplateContent = finds2[0].Value;
                                } else {
                                    vm.WxTemplateContent = "";
                                }
                                break;
                            case 6:
                                vm.SelectWxAppTemplateID = item.WxTemplateID;
                                if (item.WxAppEmKeyWord) {
                                    vm.WxAppEmKeyWord = item.WxAppEmKeyWord;
                                }
                                if (item.BoxID) {
                                    vm.WxAppPlatform = item.BoxID;
                                }
                                item.PushTypeDesc = '微信小程序模版消息';
                                break;

                        }

                        return item;
                    });
                    //  templates[0].ContentDesc.push({ Keyss: "fasdfasdfasdf", Value: 'dfsdfsdf' });
                    vm.Templates = templates;
                    vm.AllTemplateTags = data.AllTemplateTags;
                    if (data.TemplateTag) {
                        vm.TemplateTag = data.TemplateTag;
                    }
                    api.SelectAllWxAppConfigAsync().then(function (data) {
                        vm.WxAppConfigs = data;
                        vm.WxAPPPlatformSelectChange();
                    });
                }
            });

            if (batchid) {
                vm.batchid = batchid;
                vm.SelectTemplatePlanInfo();
                vm.SelectTemplateImageTextInfo();
            }
            api.SelectAllMessageTypes().then(function (data) {
                vm.MessageTypes = data;
            });
        },
        methods: {
            EditMessageImageTextConfig: function (item) {
                var vm = this;
                $("#textimageupload").val('');
                vm.EditImageText = item;
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
                api.DeleteImageTextConfigAsync(pkid).then(function (data) {
                    alert(data.msg);
                    vm.SelectTemplateImageTextInfo();
                });
            },
            ResetMessageBoxTemplate: function () {
                var vm = this;
                var batchid = $("#batchid").val();

                batchid = batchid ? batchid : vm.batchid;
                api.SelectTemplatesByBatchIDAsync(batchid).then(function (data) {
                    if (data.code == 1) {
                        var templates = data.datas;
                        templates = templates.map(function (item) {
                            switch (item.DeviceType) {
                                case 3:
                                    //vm.messageboxtemplate = item;
                                    item.PushTypeDesc = 'APP消息';
                                    vm.MessageBoxShowType = item.MessageBoxShowType;
                                    break;


                            }
                            return item;
                        });
                        var messagetemplates = templates.filter(function (item) { return item.DeviceType == 3 });
                        if (messagetemplates && messagetemplates.length) {
                            var temp = messagetemplates[0];
                            var origintemplate = vm.Templates.filter(function (item) { return item.DeviceType == 3 });
                            if (origintemplate && origintemplate.length) {
                                origintemplate[0].Title = temp.Title;
                                origintemplate[0].Content = temp.Content;
                                origintemplate[0].AppActivity = temp.AppActivity;
                            }
                        }
                    }
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
                var dialog = $('#EditConfig');
                vm.EditImageText.BatchID = vm.batchid;
                api.LogImageTextConfigAsync(vm.EditImageText, vm.MessageBoxShowType).then(function (data) {
                    if (data.code == 1) {
                        vm.SelectTemplateImageTextInfo();
                        dialog.dialog("close");
                        setTimeout(function () {
                            vm.ResetMessageBoxTemplate();
                        }, 300);
                    }
                    alert(data.msg);
                });
            },
            SelectTemplateImageTextInfo: function () {
                var vm = this;
                api.SelectImageTextByBatchId(vm.batchid).then(function (data) {
                    vm.ImageTextInfos = data;
                });
            },
            ColorChange: function (color) {
                var vm = this;
                console.log(color);
                var pattern = /^#[0-9a-fA-F]{6}$/
                //var obj = eval("document.all['" + object + "'].value");
                if (color.match(pattern) == null) {
                    vm.CanSave = false;
                    alert("请输入正确的颜色值");
                } else {
                    vm.CanSave = true;
                }
            },
            WxAPPPlatformSelectChange: function () {
                var vm = this;
                api.SelectAllWxAppTemplatesAsync(vm.WxAppPlatform).then(function (data) {
                    vm.WxAppTemplates = data;
                    if (data && data.length) {
                        var wxpushtemplate = vm.Templates.filter(function (x) { return x.DeviceType == 6 });
                        if (wxpushtemplate && wxpushtemplate.length) {
                            var id = wxpushtemplate[0].WxTemplateID;
                            if (!id) {
                                wxpushtemplate[0].WxTemplateID = data[0].template_id;
                            }
                            wxpushtemplate[0].BoxID = vm.WxAppPlatform;
                        }
                    }
                });
            },
            WxAPPPlatformForWxOpenSelectChange: function () {
                var vm = this;
                var wxpushtemplate = vm.Templates.filter(function (x) { return x.DeviceType == 4 });
                if (wxpushtemplate && wxpushtemplate.length) {
                    wxpushtemplate[0].BoxID = vm.WxAppPlatformForWxOpen;
                }
            },
            AddContentDesc: function (template) {
                template.ContentDesc.push({});
            },
            AddKeyValue: function (template) {
                template.ExtraDict.push({});
            },
            AddKeyValueDesc: function (template) {
                template.KeyValueDesc.push({});
            },
            DeleteContentDesc: function (template, desc) {
                var index = template.ContentDesc.indexOf(desc);
                template.ContentDesc.splice(index, 1);
            },
            DeleteKeyValue: function (template, extra) {

                var index = template.ExtraDict.indexOf(extra);
                template.ExtraDict.splice(index, 1);
            },
            DeleteKeyValueDesc: function (template, keyvaluedesc) {

                var index = template.KeyValueDesc.indexOf(keyvaluedesc);
                template.KeyValueDesc.splice(index, 1);
            },
            PreviewTemplate: function (template) {
                var vm = this;
                var dialog = $('#diaglog');
                if (template.DeviceType == '4' || template.DeviceType == '6') {
                    $("#diaglogspan").text("填写预览OPENID,用英文逗号隔开");
                } else {
                    $("#diaglogspan").text("填写预览手机号,用英文逗号隔开");
                }
                $("#previewtargets").val("");
                dialog.dialog({
                    buttons: {
                        "取消": function () {
                            $(this).dialog("close");
                        },
                        "发送预览": function () {
                            var targets = $("#previewtargets").val();
                            if (!targets) {
                                alert("输入数据有误.");
                                return;
                            }
                            template = vm.PreModifyTemplate(template);
                            api.SubmitSingleTemplateAsync(template, "1", targets, "").then(function (result) {
                                console.log(result);
                                alert(result.msg);
                            });
                        }
                    },
                    title: "推送预览"
                });
            },

            PreModifyTemplate: function (template) {
                var vm = this;
                if (template.DeviceType == 6) {
                    template.BoxID = vm.WxAppPlatform;
                    template.WxTemplateColors = vm.WxAPPTemplateColors;
                    template.WxAppEmKeyWord = vm.WxAppEmKeyWord;
                }
                if (template.DeviceType == 4) {
                    template.BoxID = vm.WxAppPlatformForWxOpen;
                }
                if (template.DeviceType == 4) {
                    template.WxTemplateColors = vm.WxTemplateColors;
                    var finds1 = template.ContentDesc.filter(function (s) { return s.Key == "{{first.DATA}}" });
                    if (finds1 && finds1.length) {
                        finds1[0].Value = vm.WxTemplateTitle;
                    } else {
                        template.ContentDesc.push({
                            Key: "{{first.DATA}}",
                            Value: vm.WxTemplateTitle
                        });
                    }
                    var finds2 = template.ContentDesc.filter(function (s) { return s.Key == "{{remark.DATA}}" });
                    if (finds2 && finds2.length) {
                        finds2[0].Value = vm.WxTemplateContent;
                    } else {
                        template.ContentDesc.push({
                            Key: "{{remark.DATA}}",
                            Value: vm.WxTemplateContent
                        });
                    }
                    //template.ContentDesc["{{first.DATA}}"] = vm.WxTemplateTitle;
                    //template.ContentDesc["{{remark.DATA}}"] = vm.WxTemplateContent;
                }
                if (template.DeviceType == 3) {
                    template.MessageBoxShowType = vm.MessageBoxShowType;
                }
                return template;
            },
            SaveTemplate: function (template) {
                var vm = this;
                console.log(template);
                template = vm.PreModifyTemplate(template);
                if (!vm.CanSave) {
                    alert("请查看是否有错误信息,无法保存");
                    return;
                }
                if (template.DeviceType == 3 && vm.MessageBoxIsImageTemplate) {
                    if (vm.MessageBoxShowType == 'BigImageText') {
                        if (vm.ImageTextInfos && vm.ImageTextInfos.length > 5) {
                            alert("大图文最多5条");
                            return;
                        }
                    }
                    else if (vm.MessageBoxShowType == 'SmallImageText') {
                        if (vm.ImageTextInfos && vm.ImageTextInfos.length > 1) {
                            alert("小图文最多1条");
                            return;
                        }
                    }
                }
                api.SubmitSingleTemplateAsync(template, "0", "", vm.planname, vm.TemplateTag).then(function (result) {
                    console.log(result);
                    alert(result.msg);
                    if (result.batchid) {
                        vm.Templates.forEach(function (t) {
                            t.BatchID = result.batchid;
                        });
                        vm.batchid = result.batchid;
                    }
                    if (result.pkid) {
                        template.PKID = result.pkid;
                    }
                    vm.SelectTemplatePlanInfo();
                });
            },
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
            upImage: function (template, devicetype) {

                var elemid = "uploadimage" + devicetype;
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
                                template.ImageUrl = imgurl;
                            }
                        } else {
                            alert("图片上传失败！" + reusltJson.Msg);
                            return;
                        }
                    }
                });
                //$.ajaxFileUpload({
                //    url: "/PushManager/ImageUploadToAli",
                //    secureuri: false,
                //    fileElementId: elemid,
                //    data: { checkModel: 0 },
                //    async: false,
                //    dataType: 'text',
                //    success: function (result) {
                //        console.log(result);
                //        var reusltJson = JSON.parse(result);
                //        if (reusltJson.Msg === "上传成功") {
                //            var imgurl = reusltJson.FullImage;
                //            //template.ImageUrl = "https://resource.lylinux.net/image/2017/07/16/cv.png"; 
                //            if (imgurl !== null && imgurl !== "") {
                //                template.ImageUrl = imgurl;
                //            }
                //        } else {
                //            alert("图片上传失败！" + reusltJson.Msg);
                //            return;
                //        }
                //    },
                //    error: function (e, t, x) {
                //    }
                //});
            },
            SelectTemplatePlanInfo: function () {
                var vm = this;
                if (vm.batchid) {
                    api.SelectTemplatePlanInfoAsync(vm.batchid).then(function (data) {
                        if (data.code == 1) {
                            if (data.planname) {
                                vm.planname = data.planname;
                            }
                            if (data.planstatus) {
                                vm.planstatus = data.planstatus;
                            }
                        }
                    });
                }
            }
        }
    });
})