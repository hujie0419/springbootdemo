// import { validCommonIdFunc } from '../../../commons/commonValid/commonValid';
import { validCouponPid } from '../../../commons/couponPidValid/couponPidValid';

/**
 * 获取特殊模块—底部tab配置对象
 * @param {String | Number} index 表单的index
 * @param {Object} $http xhr
 * @returns {Array}
 */
export function getFootTabsPageConfig (index, $http) {
    return [{
        nameText: 'Tab ' + index,
        formControl: [{
            nameText: 'icon图片',
            showNameText: true,
            controlName: 'LinkImgUrl',
            type: 'fileText',
            prefixList: [{
                nameText: '图片',
                className: 'foottab-img',
                type: 'img'
            }],
            defaultValue: '',
            readonly: true,
            valid: {
                required: true,
                requiredErr: '请上传icon图片'
            },
            descList: [{
                action: '',
                type: 'fileButton',
                validFile: {
                    type: 'Image',
                    // extension: ['png', 'jpg', 'jpeg', 'gif']
                    // limitSize: 10245,
                    // limitWidth: 200,
                    // limitHeight: 200,
                    limitMinWidth: 200,
                    limitMinHeight: 200,
                    limitMaxWidth: 200,
                    limitMaxHeight: 200
                    // limitScale: 1/2, // 比例

                    // typeErrorMsg: '请上传正确文件格式',
                    // sizeErrorMsg: '文件大小过大',
                    // widthErrorMsg: '文件宽度过大',
                    // heightErrorMsg: '文件高度过大'
                },
                nameText: '上传图片'
            }, {
                nameText: '限200*200'
            }]
        }]
    }, {
        nameText: '',
        formControl: [{
            nameText: '链接',
            showNameText: true,
            controlName: 'tabLinkType',
            type: 'groupSelect',
            validFlowTime: 1000,
            defaultValue: {
                select: 'ActivityPage',
                value: ''
            },
            valid: [{
                isChecked: true
            }, (con) => {
                let res = null;
                if (con && con.value && con.value.select) {
                    switch (con.value.select) { // 校验活动页ID
                        case 'ActivityPage':
                            if (con.value.value) {
                                res = validCouponPid(con.value, $http, '', {
                                    PromotionIdKey: 'LinkActivityId',
                                    PromotionType: 'Activity',
                                    errText: '活动页ID不正确'
                                });
                            } else { // 没有输入活动页ID
                                res = {
                                    errInfo: {
                                        desc: '请输入活动页ID'
                                    }
                                };
                            }
                            break;
                        case 'ChannelPage':
                            if (!con.value.value) {
                                res = {
                                    errInfo: {
                                        desc: '请选择频道页面'
                                    }
                                };
                            }
                            break;
                        case 'Customize':
                            if (!con.value.value) {
                                res = {
                                    errInfo: {
                                        desc: '请输入自定义链接'
                                    }
                                };
                            }
                            break;
                        default:
                            break;
                    }
                }
                return res;
            }],
            list: [{
                nameText: '活动页',
                controlName: 'LinkActivityId',
                type: 'radioText',
                placeholder: '活动页ID',
                className: 'form-control-small',
                value: 'ActivityPage'
            }, {
                nameText: '频道页面',
                controlName: 'channelPageTab',
                className: 'form-control-small',
                type: 'radioSelect',
                value: 'ChannelPage',
                list: [{
                    nameText: '首页',
                    value: 'HomePage'
                }, {
                    nameText: '车品商城',
                    value: 'CarProductsMall'
                }, {
                    nameText: '保养大全',
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
                }
                ]
            }, {
                nameText: '自定义链接',
                controlName: 'selfLinkTab',
                type: 'radioEmpty',
                value: 'Customize'
            }],
            descList: [{
                type: 'link',
                nameText: '修改',
                status: 'Customize',
                className: 'link-left-margin'
            }]
        }]
    }];
}

/**
 * 自定义链接浮窗配置项
 *
 * @export
 * @returns {Array}
 */
export function getSelfLink () {
    return [{
        nameText: '移动端链接',
        controlName: 'MobileLink',
        defaultValue: '',
        valid: {
            required: true
        }
    }, {
        nameText: 'PC端链接',
        controlName: 'PcLink',
        defaultValue: '',
        valid: {
            required: true
        }
    }, {
        nameText: '小程序链接',
        controlName: 'AppletsLink',
        defaultValue: '',
        valid: {
            required: true
        }
    }, {
        nameText: 'APP链接',
        controlName: 'AppLink'
    }, {
        nameText: '快应用链接',
        controlName: 'QuickAppLink'
    }];
}
/** 背景颜色的配置信息
 *  @returns {array} 表单的配置信息
 */
export function getBgColor () {
    return [{
        nameText: '背景色',
        controlName: 'TabBackGroundColor',
        placeholder: '6位RGB码',
        defaultValue: '#FFFFFF',
        valid: {
            required: true,
            format: 'vColor'
        },
        prefixList: [{
            type: 'color',
            nameText: '颜色'
        }]
    }];
}
