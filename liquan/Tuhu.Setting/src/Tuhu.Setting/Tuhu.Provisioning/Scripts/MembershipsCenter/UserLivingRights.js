//会员生活权益脚本
var UserLivingRights = (function () {
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
                , id: 'tbl_userLivingRights_table'
                , height: 500
                , url: '/UserLivingRights/GetDataByPage'
                , title: '会员生活权益'
                , limit: 20
                , page: true //开启分页
                , cols: [[ //表头
                    { type: 'checkbox',fixed:true }
                    , { field: 'PKID', width: 70, title: 'PKID', sort: true, fixed: true}
                    , { field: 'ChannelName', title: '渠道名称', width: 90 }
                    , { field: 'WelfareContent', title: '福利内容', width: 150 }
                    , { field: 'RightsValue', title: '权益值', width: 80 }
                    , { field: 'ReceivingDescription', title: '领取方式说明', width: 150 }
                  //   , { field: 'CouponId', title: '优惠券ID', width: 200 }
                    , { field: 'CouponDescription', title: '优惠券说明', width: 150 }
                    , { field: 'LinkUrl', title: '跳转链接', width: 150 }
                    , { field: 'SortIndex', title: '排序序列', width: 80, }
                    , { field: 'LastUpdateBy', title: '最后修改人', width: 100 }
                    , { field: 'StrLastUpdateDateTime', title: '最后修改时间', width: 160 }
                ]],
            });
        },
        //查询
        Search: function (table) {
            var searchName = $('#txtChannelName').val();
            table.reload('tbl_userLivingRights_table', {
                page: {
                    curr: 1 //重新从第 1 页开始
                }
                , where: {
                    key: {
                        ChannelName: searchName
                    },
                    ChannelName: searchName
                }
            });
        },
        //获取选择行数据
        GetCheckedData: function (table) {
            var checkStatus = table.checkStatus('tbl_userLivingRights_table');
            return checkStatus.data;
        },
        //新增或者编辑
        Modify: function (table, id, callback) {
            if (!id) { id = 0;}
            var title = id && id > 0 ? '编辑会员生活权益信息' : '添加会员生活权益信息';
            var index = layer.open({
                title: title,
                content: '/UserLivingRights/Edit?id='+id,
                type: 2,
                area: ['700px', '480px'],
                btn: ['确定', '取消'],
                success: function (layero, index) {
                    //var iframeWin = window[layero.find('iframe')[0]['name']];
                    //iframeWin.zTreeSetting.SetNodesCheckedValue(ids);
                },
                yes: function (index, layero) {
                    var iframeWin = window[layero.find('iframe')[0]['name']];
                    var validateResult = iframeWin.UserLivingRightsEdit.Validate();
                    if (!validateResult) {
                        return;
                    }
                    var result = iframeWin.UserLivingRightsEdit.Save(index);
                    if (result) {

                    } else {
                        setting.base.close(index);
                    }
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
                ids += model.PKID + ',';
            }
            if (ids.length > 0) {
                ids = ids.substring(0, ids.length - 1);
            }
            $.ajax({
                url: '/UserLivingRights/Delete',
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
                        UserLivingRights.Search(table);
                    } else {
                        setting.base.right('删除失败');
                        setting.base.closeAllLoading();
                    }
                    
                },
                error: function () {
                    setting.base.closeAllLoading();
                }
            });
        },
        DataInit: function () {
            layui.use('table', function () {
                var table = layui.table //表格 
                UserLivingRights.TableDataLoad(table);
                var $ = layui.$;
                $('#btn_add').on('click', function () {
                    UserLivingRights.Modify(table, 0);
                });
                //编辑
                $('#btn_modify').on('click', function () {
                    var checkData = UserLivingRights.GetCheckedData(table);
                    if (!checkData || checkData.length <= 0) {
                        setting.base.msg('请勾选编辑行');
                        return;
                    }
                    if (checkData.length > 1) {
                        setting.base.msg('一次仅能编辑一行数据');
                        return;
                    }
                    UserLivingRights.Modify(table, checkData[0].PKID);
                });
                //删除
                $('#btn_delete').on('click', function () {
                    var checkData = UserLivingRights.GetCheckedData(table);
                    if (!checkData || checkData.length <= 0) {
                        setting.base.msg('请勾选删除行');
                        return;
                    }
                    layer.confirm('确定删除勾选行数据么？', { icon: 3 }, function (index) {
                        UserLivingRights.Delete(table, checkData);
                    });
                });
                //查询
                $('#btn_serach').on('click', function () {
                    UserLivingRights.Search(table);
                });
            });
        },
        init: function () {
            UserLivingRights.pageLayout();
        }
    }
})();

//会员权益编辑
var UserLivingRightsEdit = (function () {
    return {
        //验证数据
        Validate: function () {
            var model = UserLivingRightsEdit.GetData();
            if (!model) {
                parent.setting.base.msg('获取数据对象异常');
                return false;
            }
            if ($.trim(model.ChannelName) == '') {
                parent.setting.base.msg('请输入渠道名称');
                return false;
            }
            if (model.ChannelName.length>25) {
                parent.setting.base.msg('渠道名称超过25个汉字数限制');
                return false;
            }
            if ($.trim(model.WelfareContent) == '') {
                parent.setting.base.msg('请输入福利内容');
                return false;
            }
            if (model.WelfareContent.length > 100) {
                parent.setting.base.msg('福利内容超过100个汉字数限制');
                return false;
            }
            if ($.trim(model.LinkUrl) == '') {
                parent.setting.base.msg('请输入跳转链接');
                return false;
            }
            if (model.LinkUrl.length > 500) {
                parent.setting.base.msg('跳转链超过长度限制');
                return false;
            }
            return true;
        },
       //获取数据
        GetData: function () {
            var $form = $("#UserRightEdit");
            var model = {};
            model.PKID = $('#hid-Id').val();
            model.ChannelName = $form.find('input[name="ChannelName"]').val();
            model.WelfareContent = $form.find('input[name="WelfareContent"]').val();
            model.RightsValue = $form.find('input[name="RightsValue"]').val();
            model.ReceivingDescription = $form.find('input[name="ReceivingDescription"]').val();
            model.CouponDescription = $form.find('input[name="CouponDescription"]').val();
            model.LinkUrl = $form.find('input[name="LinkUrl"]').val();
            model.SortIndex = $form.find('input[name="SortIndex"]').val();
            return model;
        },
        //保存
        Save: function (index) {
            var model = UserLivingRightsEdit.GetData(); 
            var result = 0;
            $.ajax({
                url: '/UserLivingRights/Save', 
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
            });
        },
    }
})();
