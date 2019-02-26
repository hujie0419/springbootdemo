//会员奖励奖励
var UserPromotionCodeRights = (function () {
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
                , id: 'tbl_UserPromotionCodeRights_table'
                , height: 500
                , url: '/UserPromotionCode/GetDataByPage'
                , title: '会员权益奖励'
                , limit: 20
                , page: true //开启分页
                , cols: [[ //表头
                    { type: 'checkbox', fixed: true }
                    , { field: 'Id', width: 65, title: 'Id', sort: true, fixed: true }
                    , { field: 'Name', title: '奖励名称', width: 80 }
                    , { field: 'CouponName', title: '优惠券名称', width: 90 }
                    , { field: 'CouponDescription', title: '优惠券描述', width: 150 }
                    , { field: 'StrRewardType', title: '奖励类型', width: 150 }
                    , { field: 'RewardValue', title: '奖励值', width: 150 }
                    , { field: 'SortIndex', title: '排序序列', width: 80, }
                    , { field: 'LastUpdateBy', title: '最后修改人', width: 100 }
                    , { field: 'StrLastUpdateDateTime', title: '最后修改时间', width: 160 }
                ]],
            });
        },
        //查询
        Search: function (table) {
            var searchName = $('#txtName').val();
            var memberGradeId = $('#sel-memeberShipGradeId').val();
            table.reload('tbl_UserPromotionCodeRights_table', {
                page: {
                    curr: 1 //重新从第 1 页开始
                }
                , where: {
                    key: {
                    },
                    Name: searchName,
                    MembershipsGradeId: memberGradeId
                }
            });
        },
        //获取选择行数据
        GetCheckedData: function (table) {
            var checkStatus = table.checkStatus('tbl_UserPromotionCodeRights_table');
            return checkStatus.data;
        },
        //新增或者编辑
        Modify: function (table, id, callback) {
            if (!id) { id = 0; }
            var title = id && id > 0 ? '编辑会员权益奖励信息' : '添加会员权益奖励信息';
            var memberGradeId = $('#sel-memeberShipGradeId').val();
            var memberGradeName = $('#sel-memeberShipGradeId').find('option[value="' + memberGradeId+'"]').data('value');
            var index = layer.open({
                title: title,
                content: '/UserPromotionCode/Edit?id=' + id + '&membershipsGradeId=' + memberGradeId + '&memberGradeName=' + memberGradeName,
                type: 2,
                area: ['700px', '500px'],
                btn: ['确定', '取消'],
                success: function (layero, index) {
                },
                yes: function (index, layero) {
                    var iframeWin = window[layero.find('iframe')[0]['name']];
                    var validateResult = iframeWin.UserPromotionCodeRightsEdit.Validate();
                    if (!validateResult) {
                        return;
                    }
                    var result = iframeWin.UserPromotionCodeRightsEdit.Save(index);
                    if (result) {

                    } else {
                        setting.base.close(index);
                    }
                },
                cancel: function (index, layero) {
                },

            });
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
                url: '/UserPromotionCode/Delete',
                data: { ids: ids },
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
                        UserPromotionCodeRights.Search(table);
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
            layui.use(['table', 'form'], function () {
                var table = layui.table //表格 
                    ,form = layui.form
                UserPromotionCodeRights.TableDataLoad(table);
                var $ = layui.$;
                form.on('select(memberGrade)', function (data) {
                    UserPromotionCodeRights.Search(table);
                });
                $('#btn_add').on('click', function () {
                    UserPromotionCodeRights.Modify(table, 0);
                    return false;
                });
                //编辑
                $('#btn_modify').on('click', function () {
                    var checkData = UserPromotionCodeRights.GetCheckedData(table);
                    if (!checkData || checkData.length <= 0) {
                        setting.base.msg('请勾选编辑行');
                        return false;
                    }
                    if (checkData.length > 1) {
                        setting.base.msg('一次仅能编辑一行数据');
                        return false;
                    }
                    UserPromotionCodeRights.Modify(table, checkData[0].Id);
                    return false;
                });
                //删除
                $('#btn_delete').on('click', function () {
                    var checkData = UserPromotionCodeRights.GetCheckedData(table);
                    if (!checkData || checkData.length <= 0) {
                        setting.base.msg('请勾选删除行');
                        return false;
                    }
                    layer.confirm('确定删除勾选行数据么？', { icon: 3 }, function (index) {
                        UserPromotionCodeRights.Delete(table, checkData);
                        return false;
                    });
                });
                //查询
                $('#btn_serach').on('click', function () {
                    UserPromotionCodeRights.Search(table);
                });
            });
        },
        init: function () {
            UserPromotionCodeRights.pageLayout();
        }
    }
})();

//会员奖励奖励
var UserPromotionCodeRightsEdit = (function () {
    return {
        //验证数据
        Validate: function () {
            var model = UserPromotionCodeRightsEdit.GetData();
            if (!model) {
                parent.setting.base.msg('获取数据对象异常');
                return false;
            }
            if ($.trim(model.Name) == '') {
                parent.setting.base.msg('请输入奖励名称');
                return false;
            }
            if (model.Name.length > 25) {
                parent.setting.base.msg('奖励名称字数超长');
                return false;
            }
            if ($.trim(model.RewardId) == '') {
                parent.setting.base.msg('请输入奖励Id');
                return false;
            }
            if (model.RewardId.length > 50) {
                parent.setting.base.msg('奖励Id超长');
                return false;
            }
            if ($.trim(model.RewardValue) == '' && model.RewardType==2) {
                parent.setting.base.msg('请输入奖励值');
                return false;
            }
            if (model.RewardValue.length > 25) {
                parent.setting.base.msg('奖励值超长');
                return false;
            }
            //if ($.trim(model.CouponName) == '') {
            //    parent.setting.base.msg('请输入优惠券名称');
            //    return false;
            //}
            //if (model.CouponName.length > 15) {
            //    parent.setting.base.msg('优惠券名称字数超长');
            //    return false;
            //}
            if ($.trim(model.RuleID) == '' && model.RewardType != 2) {
                parent.setting.base.msg('请输入规则Id');
                return false;
            }
            if (model.RuleID.length > 15) {
                parent.setting.base.msg('规则Id字数超长');
                return false;
            }
            if ($.trim(model.CouponDescription) != '' && model.CouponDescription.length > 25) {
                parent.setting.base.msg('优惠券描述字数超长');
                return false;
            }
            return true;
        },
        //获取数据
        GetData: function () {
            var $form = $("#UserRewardEdit");
            var model = {};
            model.Id = $('#hid-Id').val();
            model.SImage = $form.find('img[name="SImage"]').val();
            model.BImage = $form.find('img[name="BImage"]').val();
            model.RuleID = $form.find('input[name="RuleID"]').val();
            model.CouponName = $form.find('input[name="CouponName"]').val();
            model.CouponDescription = $form.find('textarea[name="CouponDescription"]').val();
            model.MembershipsGradeId = $form.find('input[name="MembershipsGradeId"]').data('value');
            model.Name = $form.find('input[name="Name"]').val();
            model.UserPermissionId = $form.find('select[name="UserPermissionId"]').val();
            model.PermissionType = $form.find('select[name="UserPermissionId"]').find('option[value="' + model.UserPermissionId + '"]').data('value');
            model.RewardType = $form.find('select[name="RewardType"]').val();
            model.RewardId = $form.find('input[name="RewardId"]').val();
            model.RewardValue = $form.find('input[name="RewardValue"]').val();
            model.SortIndex = $form.find('input[name="SortIndex"]').val();
            return model;
        },
        //保存
        Save: function (index) {
            var model = UserPromotionCodeRightsEdit.GetData();
            var result = 0;
            $.ajax({
                url: '/UserPromotionCode/Save',
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
        RewardTypeChange: function () {
            var rewardType = $('#sel-rewardType').val();
            if (rewardType == '2') {
                $('#div-integral').show();
                $('#div-coupon').hide();
                $('#lblRewardId').html('积分规则ID');
            } else {
                $('#div-integral').hide();
                $('#div-coupon').show();
                $('#lblRewardId').html('优惠券ID');
            }
        },
        //文本框初始化
        EditDataInit: function () {
            layui.use(['form', 'layedit', 'laydate', 'upload'], function () {
                var form = layui.form
                    , layer = layui.layer
                    , layedit = layui.layedit
                    , laydate = layui.laydate
                    , upload = layui.upload;
                form.on('select(RewardType)', function () {
                    UserPromotionCodeRightsEdit.RewardTypeChange();
                });
                var sImage = upload.render({
                    elem: '#btn-upload-sm-imge',
                    url: '/Article/AddArticleImg2',
                    accept: 'images',
                    size: 5120,
                    before: function (obj) {
                        parent.setting.base.load();
                    },
                    done: function (result) {
                        //上传成功
                        if (result.BImage != "" && result.SImage != "") {
                            $("#UserRewardEdit").find('img[name="SImage"]').attr('src', result.BImage);
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
                var BImage = upload.render({
                    elem: '#btn-upload-bm-imge',
                    url: '/Article/AddArticleImg2',
                    accept: 'images',
                    size: 5120,
                    before: function (obj) {
                        parent.setting.base.load();
                    },
                    done: function (result) {
                        //上传成功
                        if (result.BImage != "" && result.SImage != "") {
                            $("#UserRewardEdit").find('img[name="BImage"]').attr('src', result.BImage);
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
            });
            
        },
    }
})();
