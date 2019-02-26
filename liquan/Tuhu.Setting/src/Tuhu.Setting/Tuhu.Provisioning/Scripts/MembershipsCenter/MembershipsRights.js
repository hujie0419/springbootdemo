//会员权益（特权）脚本
var UserRights = (function () {
    return {
        pageLayout: function () {
            var tableHeight = $(window).height() - $('#header').height() - 100;
            var tableWidth = $(window).width() - $('fieldset').width() - 200;
            if (tableWidth < 600) {
                tableWidth = 600;
            }
            $('#right-content').css('width', tableWidth);
            $('#right-content').css('height', tableHeight);
        },
        TableDataLoad: function (table) {
            table.render({
                elem: '#tblData'
                , id: 'tbl_userPermission_table'
                , height: 500
                , url: '/UserPermission/GetUserPermissionByPage'
                , title: '会员权益信息'
                , limit: 20
                , page: true //开启分页
                , cols: [[ //表头
                    { type: 'checkbox',fixed:true }
                    , { field: 'Id', width: 70, title: 'ID', sort: true, fixed: true}
                    , { field: 'Name', title: '名称', width: 90 }
                    , { field: 'FootTile', title: '副标题', width: 150 }
                    , { field: 'MembershipsGradeName', title: '会员等级', width: 80 }
                    , { field: 'StrIsEnable', title: '状态', width: 70 }
                    , { field: 'DescriptionTitle', title: '说明标题', width: 200 }
                    , { field: 'LightText', title: '可领取文案', width: 150 }
                    , { field: 'DarkText', title: '不可领取文案', width: 150 }
                    , { field: 'Position', title: '显示顺序', width: 80, }
                    , { field: 'EnabledVersion', title: '启用版本', width: 90 }
                    , { field: 'LastUpdateBy', title: '最后修改人', width: 100 }
                    , { field: 'CreateDatetime', title: '创建时间', width: 160 }
                    , { field: 'LastUpdateDateTime', title: '最后修改时间', width: 160 }
                ]],
            });
        },
        //查询
        Search: function (table) {
            var searchName = $('#txtSearchName').val();
            var memeberGradeId = $('#sel-memeberShipGradeId').val();
            table.reload('tbl_userPermission_table', {
                page: {
                    curr: 1 //重新从第 1 页开始
                }
                , where: {
                    key: {
                        PermissionName: searchName
                    },
                    PermissionName: searchName,
                    MembershipsGradeId: memeberGradeId
                }
            });
        },
        //获取选择行数据
        GetCheckedData: function (table) {
            var checkStatus = table.checkStatus('tbl_userPermission_table');
            return checkStatus.data;
        },
        //新增或者编辑
        Modify: function (table, id, callback) {
            if (!id) { id = 0; }
            var memeberGradeId = $('#sel-memeberShipGradeId').val();
            var title = id && id > 0 ? '编辑用户权益信息' : '添加用户权益信息';
            var index = layer.open({
                title: title,
                content: '/UserPermission/Edit?id=' + id + '&memberGradeId=' + memeberGradeId,
                type: 2,
                area: ['700px', '700px'],
                btn: ['确定', '取消'],
                success: function (layero, index) {
                    //var iframeWin = window[layero.find('iframe')[0]['name']];
                    //iframeWin.zTreeSetting.SetNodesCheckedValue(ids);
                },
                yes: function (index, layero) {
                    var iframeWin = window[layero.find('iframe')[0]['name']];
                    var validateResult = iframeWin.UserRightsEdit.Validate();
                    if (!validateResult) {
                        return;
                    }
                    var result = iframeWin.UserRightsEdit.Save(index);
                    if (result) {

                    } else {
                        setting.base.close(index);
                    }
                    //if (callback && typeof callback === "function") {
                    //    callback(checkNodes);
                    //}
                },
                cancel: function (index, layero) {
                },

            });
            //操作正确提示
            //  layer.alert('请勾选删除行', { icon: 1 });
        },
        //删除
        Delete: function (table, checkData) {
            var ids = '';
            for (var i = 0; i < checkData.length; i++) {
                var model = checkData[i];
                ids += model.Id + ',';
            }
            if (ids.length > 0) {
                ids = ids.substring(0, ids.length - 1);
            }
            $.ajax({
                url: '/UserPermission/DeleteById',
                data: {ids:ids},
                // async: true,
                type: 'post',
                beforeSend: function () {
                    setting.base.load();
                },
                complete: function () {
                    setting.base.closeAllLoading();
                },
                success: function (m) {
                    if (m.data > 0) {
                        setting.base.right('删除成功');
                        UserRights.Search(table);
                    } else {
                        setting.base.error(e.msg);
                    }
                    
                },
                error: function () {
                    setting.base.closeAllLoading();
                }
            });
        },
        DataInit: function () {
            layui.use(['table', 'form'], function () {
                var table = layui.table //表格 
                    , form = layui.form
                UserRights.TableDataLoad(table);
                var $ = layui.$;

                form.on('select(memberGrade)', function (data) {
                    UserRights.Search(table);
                    return false;
                });

                //添加
                $('#btn_add').on('click', function () {
                    UserRights.Modify(table, 0);
                    return false;
                });
                //编辑
                $('#btn_modify').on('click', function () {
                    var checkData = UserRights.GetCheckedData(table);
                    if (!checkData || checkData.length <= 0) {
                        setting.base.msg('请勾选编辑行');
                        return false;
                    }
                    if (checkData.length > 1) {
                        setting.base.msg('一次仅能编辑一行数据');
                        return false;
                    }
                    UserRights.Modify(table, checkData[0].Id);
                    return false;
                });
                //删除
                $('#btn_delete').on('click', function () {
                    var checkData = UserRights.GetCheckedData(table);
                    if (!checkData || checkData.length <= 0) {
                        setting.base.msg('请勾选删除行');
                        return false;
                    }
                    layer.confirm('确定删除勾选行数据么？', { icon: 3 }, function (index) {
                        UserRights.Delete(table, checkData);
                        return false;
                    });
                });
                //查询
                $('#btn_serach').on('click', function () {
                    UserRights.Search(table);
                    return false;
                });
            });
        },
        init: function () {
            UserRights.pageLayout();
        }
    }
})();

//会员权益编辑
var UserRightsEdit = (function () {
    return {
        //验证数据
        Validate: function () {
            var model = UserRightsEdit.GetData();
            if (!model) {
                parent.setting.base.msg('获取数据对象异常');
                return false;
            }
            if ($.trim(model.Name) == '') {
                parent.setting.base.msg('请输入权益名称');
                return false;
            }
            if ($.trim(model.FootTile) == '') {
                parent.setting.base.msg('请输入副标题');
                return false;
            }
            if ($.trim(model.EnabledVersion) == '') {
                parent.setting.base.msg('请输入启用版本');
                return false;
            }
            if ($.trim(model.EndVersion) == '') {
                parent.setting.base.msg('请输入结束版本');
                return false;
            }
            return true;
        },
       //获取数据
        GetData: function () {
            var $form = $("#UserRightEdit");
            var model = {};
            model.Id = $('#hid-Id').val();
            model.Name = $form.find('input[name="Name"]').val();
            model.FootTile = $form.find('input[name="FootTile"]').val();
            model.MembershipsGradeId = $form.find('select[name="MembershipsGradeId"]').val();
            model.PermissionType = $form.find('select[name="PermissionType"]').val();
            model.CheckCycle = $form.find('select[name="CheckCycle"]').val();
            model.Position = $form.find('input[name="Position"]').val();
            model.DescriptionTitle = $form.find('input[name="DescriptionTitle"]').val();
            model.IsEnable = $form.find('input[name="IsEnable"][ type="checkbox"]').data('value');
            model.EnabledVersion = $form.find('input[name="EnabledVersion"]').val();
            model.EndVersion = $form.find('input[name="EndVersion"]').val();
            model.IsLinkUrl = $form.find('input[name="IsLinkUrl"][ type="checkbox"]').data('value');
            model.IsLight = $form.find('input[name="IsLight"][ type="checkbox"]').data('value');
            model.LightText = $form.find('input[name="LightText"]').val();
            model.DarkText = $form.find('input[name="DarkText"]').val();
            model.LightUrl = $form.find('input[name="LightUrl"]').val();
            model.LightButtonUrl = $form.find('input[name="LightButtonUrl"]').val();
            model.Description = $form.find('textarea[name="Description"]').val();
            model.DescriptionDetail = $form.find('textarea[name="DescriptionDetail"]').val();
            model.LightImage = $form.find('img[name="LightImage"]').attr('src');
            model.DarkImage = $form.find('img[name="DarkImage"]').attr('src');
            model.CardImage = $form.find('img[name="CardImage"]').attr('src');
            model.AndroidUrl = $form.find('input[name="AndroidUrl"]').val();
            model.IOSUrl = $form.find('input[name="IOSUrl"]').val();
            model.PromptTag = $form.find('input[name="PromptTag"]').val();
            model.IsClickReceive = $form.find('input[name="IsClickReceive"][ type="checkbox"]').data('value');
            return model;
        },
        //保存
        Save: function (index) {
            var model = UserRightsEdit.GetData(); 
            var result = 0;
            $.ajax({
                url: '/UserPermission/Edit', 
                data: model,
                type: 'post',
                async: true,
                beforeSend: function () {
                    setting.base.load();
                },
                complete: function () {
                    parent.setting.base.closeAllLoading();
                },
                success: function (data) {
                    if (data.result) {
                        result = 1;
                        parent.setting.base.right('操作成功');
                        parent.$('#btn_serach').click();
                        parent.setting.base.closeAllLoading();
                    } else {
                        parent.setting.base.alert('操作失败');
                    }
                },
                error: function () {
                    parent.setting.base.error('保存异常');
                    parent.setting.base.closeAllLoading();
                }
            });
            return result;
        },
        //文本框初始化
        EditDataInit: function () {
            layui.use(['form', 'layedit', 'laydate', 'upload'], function () {
                var form = layui.form
                    , layer = layui.layer
                    , layedit = layui.layedit
                    , laydate = layui.laydate
                    , upload = layui.upload;
                var uploadLightImage = upload.render({
                    elem: '#btn-upload-light-imge',
                    url: '/Article/AddArticleImg2',
                    accept: 'images',
                    size: 5120,
                    before: function (obj) {
                        parent.setting.base.load();
                    },
                    done: function (result) {
                        parent.setting.base.closeAllLoading();
                        //上传成功
                        if (result.BImage != "" && result.SImage != "") {
                            $("#UserRightEdit").find('img[name="LightImage"]').attr('src', result.BImage);
                        } else {
                            parent.setting.base.error('上传失败');
                        }
                    },
                    error: function () {
                        parent.setting.base.closeAllLoading();
                        parent.setting.base.error('上传失败');
                    }
                });
                var uploadDarkImage = upload.render({
                    elem: '#btn-upload-Dark-imge',
                    url: '/Article/AddArticleImg2',
                    accept: 'images',
                    size: 5120,
                    before: function (obj) {
                        parent.setting.base.load();
                    },
                    done: function (result) {
                        //上传成功
                        if (result.BImage != "" && result.SImage != "") {
                            $("#UserRightEdit").find('img[name="DarkImage"]').attr('src', result.BImage);
                        } else {
                            parent.setting.base.error('上传失败');
                        }
                        parent.setting.base.closeAllLoading();
                    },
                    error: function () {
                        parent.setting.base.error('上传失败');
                        parent.setting.base.closeAllLoading();
                    }
                });
                var uploadCardImage = upload.render({
                    elem: '#btn-upload-card-imge',
                    url: '/Article/AddArticleImg2',
                    accept: 'images',
                    size: 5120,
                    before: function (obj) {
                        parent.setting.base.load();
                    },
                    done: function (result) {
                        //上传成功
                        if (result.BImage != "" && result.SImage != "") {
                            $("#UserRightEdit").find('img[name="CardImage"]').attr('src', result.BImage);
                        } else {
                            parent.setting.base.error('上传失败');
                        }
                        parent.setting.base.closeAllLoading();
                    },
                    error: function () {
                        parent.setting.base.error('上传失败');
                        parent.setting.base.closeAllLoading();
                    }
                });
                var $editFormId = $("#UserRightEdit");
                //监听指定开关
                form.on('switch(IsEnable)', function (data) {
                    $editFormId.find('input[name="IsEnable"][ type="checkbox"]').data('value', this.checked);
                });
                form.on('switch(IsLinkUrl)', function (data) {
                    $editFormId.find('input[name="IsLinkUrl"][ type="checkbox"]').data('value', this.checked);
                });
                form.on('switch(IsLight)', function (data) {
                    $editFormId.find('input[name="IsLight"][ type="checkbox"]').data('value', this.checked ? '1' : '0');
                });
                form.on('switch(IsClickReceive)', function (data) {
                    $editFormId.find('input[name="IsClickReceive"][ type="checkbox"]').data('value', this.checked);
                });
            });
        },
    }
})();
