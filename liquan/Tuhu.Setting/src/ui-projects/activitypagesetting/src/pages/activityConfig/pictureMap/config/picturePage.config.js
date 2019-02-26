/* eslint-disable max-lines */
import {validPromotionProduct} from '../valid/pictureMapValid';
import {couponIdValid} from '../../commons/couponIdValid/couponIdValid';
import { validCouponPid } from '../../commons/couponPidValid/couponPidValid';
import { picturePageGetGroupId } from '../../commons/picturePageGetGroupId/picturePageGetGroupId';
// import { dateValid } from '../../commons/dateValid/dateValid';
// import { validCommonIdFunc } from '../../commons/commonValid/commonValid';
/**
 * 获取子列表
 * @param {string|number} id 子列表的id
 * @param {HttpClient} $http $http
 * @param {*} context context
 * @param {Number} index 序号
 * @returns {Array}
 */
export function getChildList(id, $http, context, index) {
    let data = {
        'NoLink': [getLinkImgUrlConfig(), {
            nameText: '浮层标题',
            controlName: 'LinkTitle',
            type: 'text',
            placeholder: '例如"活动规则“、”中奖名单“',
            valid: {
                maxLength: 20
            },
            descList: [{
                nameText: '（限20个字）'
            }]
        }, {
            nameText: '浮层文案',
            className: 'form-control-large',
            type: 'textarea',
            controlName: 'LinkCopywriting'
        }],
        'ActivityPage': [getLinkImgUrlConfig(), {
            nameText: '活动页ID',
            controlName: 'LinkActivityId',
            validFlowTime: 1000,
            // type: 'fileText',
            valid: [{
                required: true
            }, (con) => {
                return validCouponPid(con, $http, con.formGroup, {
                    PromotionIdKey: 'LinkActivityId',
                    PromotionType: 'Activity',
                    errText: '活动页ID不正确'
                });
                // return validCommonIdFunc({
                //     formModel: that.formModel && that.formModel.get(index),
                //     promotionKey: 'LinkActivityId',
                //     type: 'Activity'
                // });
            }]
        }, {
            nameText: '其他选项',
            controlName: 'LinkIsTitle',
            type: 'checkBox',
            list: [{
                nameText: '隐藏标题栏',
                value: 'yc'
            }]
        }],
        'ChannelPage': [{
            nameText: '跳转页面',
            controlName: 'TheJumpType',
            type: 'radio',
            valid: {
                required: true
            },
            defaultValue: 'HomePage',
            list: [{
                nameText: '首页',
                value: 'HomePage'
            }, {
                nameText: '车品商城',
                value: 'CarProductsMall'
            }, {
                nameText: '保养适配列表',
                value: 'Maintenance'
            }, {
                nameText: '蓄电池',
                value: 'Battery'
            }, {
                nameText: '会员商城',
                value: 'MemberMall'
            }, {
                nameText: '优惠券列表',
                value: 'CouponList'
            }, {
                nameText: '查违章',
                value: 'CheckIllegal'
            }]
        }, getLinkImgUrlConfig()],
        'Coupon': [{
            nameText: '优惠券ID',
            controlName: 'CouponId',
            validFlowTime: 1000,
            valid: [{
                required: true
            }, (con) => {
                return couponIdValid(con, $http);
            }]
        }, getLinkImgUrlConfig(), {
            nameText: '开始领券时间',
            type: 'datetime',
            controlName: 'ReceiveCouponStartTime',
            formatDate: 'yyyy-MM-dd HH:mm:ss'
            // valid: [{
            //     required: false
            // }, (con) => {
            //     return dateValid(con);
            // }]
        }],
        'VoucherPackage': [getLinkImgUrlConfig(), {
            nameText: '券礼包ID',
            controlName: 'VoucherPackageId',
            validFlowTime: 1000,
            valid: [{
                required: true
            }, (con) => {
                return validCouponPid(con, $http, con.formGroup, {
                    PromotionIdKey: 'VoucherPackageId',
                    PromotionType: 'CouponPackage',
                    errText: '券礼包ID不正确'
                });
                // return validCommonIdFunc({
                //     formModel: that.formModel && that.formModel.get(index),
                //     promotionKey: 'VoucherPackageId',
                //     type: 'CouponPackage'
                // });
            }]
        }, {
            nameText: '车型认证',
            controlName: 'Certification',
            type: 'select',
            defaultValue: '不限',
            popperClass: 'windwoPop-select',
            valid: {
                isCheckd: true
            },
            list: [{
                nameText: '------不限------',
                value: '不限'
            }, {
                nameText: '车型认证',
                value: '车型认证'
            }, {
                nameText: '6周年车型认证',
                value: '6周年车型认证'
            }]
        }],
        'Maintenance': [getLinkImgUrlConfig(), {
            nameText: '保养服务',
            controlName: 'LinkMaintenanceServiceId',
            type: 'select',
            defaultValue: [],
            multiple: true,
            popperClass: 'windwoPop-select',
            valid: {
                isChecked: true
            }
        }, {
            nameText: '保养活动ID',
            controlName: 'LinkMaintenanceId',
            validFlowTime: 1000,
            valid: [{
                required: false
            }, (con) => {
                return validCouponPid(con, $http, con.formGroup, {
                    PromotionIdKey: 'LinkMaintenanceId',
                    PromotionType: 'Maintenance',
                    errText: '保养活动ID不正确'
                });
                // return validCommonIdFunc({
                //     formModel: that.formModel && that.formModel.get(index),
                //     promotionKey: 'LinkMaintenanceId',
                //     type: 'Maintenance'
                // });
            }]
        }],
        'Product': [getLinkImgUrlConfig(), {
            nameText: '商品',
            formControl: [{
                showNameText: true,
                nameText: 'PID',
                controlName: 'LinkPid',
                valid: [{
                    required: true
                }, (con) => {
                    return validPromotionProduct(con, $http);
                }],
                className: 'form-control-small'
            }, {
                showNameText: true,
                nameText: '促销ID',
                controlName: 'LinkPromotionType',
                className: 'form-control-small',
                type: 'select',
                popperClass: 'windwoPop-select',
                defaultValue: 'Whole',
                valid: [(con) => {
                    return validPromotionProduct(con, $http);
                }],
                list: [{
                    nameText: '全网活动ID',
                    value: 'Whole'
                }, {
                    nameText: '抢购ID',
                    value: 'Limit'
                }, {
                    nameText: '打折ID',
                    value: 'Discount'
                }]
            }, {
                nameText: '',
                controlName: 'LinkPromotionId',
                className: 'form-control-small',
                valid: [(con) => {
                    return validPromotionProduct(con, $http);
                }]
            }, {
                showNameText: true,
                nameText: '价格',
                disabled: true,
                controlName: 'LinkCommodityPrice',
                className: 'form-control-small'
            }]
        }],
        'GroupBuying': [getLinkImgUrlConfig(), {
            nameText: '拼团',
            formControl: [{
                showNameText: true,
                nameText: 'PID',
                controlName: 'LinkPid',
                validFlowTime: 1000,
                valid: [{
                    required: true
                }, (con) => {
                    return picturePageGetGroupId(con.formGroup, $http, data['GroupBuying'][1], context, index);
                }],
                className: 'form-control-small'
            }, {
                nameText: '',
                controlName: 'LinkProductGroupId',
                className: 'form-control-small',
                type: 'select',
                list: []
            }, {
                showNameText: true,
                nameText: '价格',
                disabled: true,
                controlName: 'LinkCommodityPrice',
                className: 'form-control-small'
            }]
        }],
        'Customize': [getLinkImgUrlConfig(), {
            nameText: '移动端链接',
            controlName: 'MobileLink',
            valid: {
                required: true
            }
        }, {
            nameText: '小程序链接',
            controlName: 'AppletsLink',
            valid: {
                required: true
            }
        }, {
            nameText: 'PC链接',
            controlName: 'PcLink',
            valid: {
                required: true
            }
        }, {
            nameText: 'APP链接',
            controlName: 'AppLink'
        }, {
            nameText: '快应用链接',
            controlName: 'QuickAppLink'
        }, {
            nameText: '其他选项',
            controlName: 'LinkIsTitle',
            type: 'checkBox',
            list: [{
                nameText: '隐藏标题栏',
                value: 'yc'
            }]
        }]
    };

    return (typeof id!=='undefined' && data[id]) || [];
}
/**
 * 选项列表
 *
 * @export
 * @returns {Array}}
 */
export function cardItemData() {
    return [{
        nameText: '链接类型',
        controlName: 'LinkType',
        type: 'radio',
        defaultValue: 'NoLink',
        valid: {
            required: true
        },
        list: [{
            nameText: '无链接',
            value: 'NoLink'
        }, {
            nameText: '活动页',
            value: 'ActivityPage'
        }, {
            nameText: '频道页面',
            value: 'ChannelPage'
        }, {
            nameText: '优惠券',
            value: 'Coupon'
        }, {
            nameText: '优惠券礼包',
            value: 'VoucherPackage'
        }, {
            nameText: '保养',
            value: 'Maintenance'
        }, {
            nameText: '商品',
            value: 'Product'
        }, {
            nameText: '拼团',
            value: 'GroupBuying'
        }, {
            nameText: '自定义链接',
            value: 'Customize'
        }]
    }];
}

/**
 * 图片项数据
 * @returns {Object}
 */
function getLinkImgUrlConfig() {
    return {
        nameText: '图片',
        controlName: 'LinkImgUrl',
        type: 'fileText',
        prefixList: [{
            nameText: '图片',
            type: 'img'
        }],
        readonly: true,
        valid: {
            required: true,
            requiredErr: '请上传图片'
        },
        descList: [{
            action: '',
            type: 'fileButton',
            validFile: {
                type: 'Image'
            },
            nameText: '上传图片'
        }]
    };
}
