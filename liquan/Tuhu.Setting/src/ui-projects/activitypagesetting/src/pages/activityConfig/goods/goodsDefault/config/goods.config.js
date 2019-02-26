import { groupSelectRowValid } from '../../../commons/valid/groupSelectValid/groupSelectRowValid';
import { validCouponPid } from '../../../commons/couponPidValid/couponPidValid';
// 申明配置方式的值
export const CONFIGURATION_MODE = {
    CATEGORY_BRAND: 'CategoryBrand',
    ACTIVE_CONFIGURATION: 'ActiveConfiguration',
    CUSTOM_GOODS: 'CustomGoods'
};

// 商品类目
export const CONFIGURATION_CATEGORY = {
    Tires: '轮胎',
    AutoProduct: '车品',
    hub: '轮毂',
    BaoYang: '保养',
    // Gifts: '礼品',
    MR1: '美容',
    QCBL: '汽车玻璃'
};

/**
 * 默认配置选项
 *
 * @export
 * @returns {Array}}
 */
export function defaultConfig () {
    return [{
        nameText: '每行数量',
        controlName: 'OrdinaryColumnNumber',
        type: 'radio',
        defaultValue: '1',
        valid: {
            isChecked: true
        },
        list: [{
            nameText: '1列',
            value: '1'
        }, {
            nameText: '2列',
            value: '2'
        }, {
            nameText: '3列',
            value: '3'
        }]
    }, {
        nameText: '模板样式',
        controlName: 'Template',
        type: 'radio',
        defaultValue: 'Minimalist',
        // requiredRight: true,
        valid: {
            required: true
        },
        list: [{
            nameText: '极简版',
            value: 'Minimalist'
        }, {
            nameText: '无按钮版',
            value: 'NoButtons'
        }, {
            nameText: '无促销语版',
            value: 'NoPromotion'
        }, {
            nameText: '完整版',
            value: 'Complete'
        }]
    }, {
        nameText: '更多',
        controlName: 'MoreType',
        type: 'checkBox',
        defaultValue: [],
        list: [{
            nameText: '品牌标签',
            value: '1'
        }, {
            nameText: '进度条',
            value: '2'
        }]
    }, {
        nameText: '分车型展示',
        controlName: 'OrdinaryCarType',
        type: 'select',
        defaultValue: '',
        placeholder: '-----不限-----',
        // valid: {
        //     isChecked: true
        // },
        list: [{
            nameText: '-----不限-----',
            value: ''
        }, {
            nameText: '二级车型',
            value: 'Two'
        }, {
            nameText: '五级车型',
            value: 'Five'
        }],
        descList: [{
            nameText: '（仅支持轮胎、轮毂和车品的部分商品）'
        }]
    }, {
        nameText: '展示行数',
        controlName: 'OrdinaryRowNumber',
        type: 'groupSelect',
        // defaultValue: '0',
        valid: [{
            isChecked: true
        }, groupSelectRowValid],
        list: [{
            type: 'radioEmpty',
            nameText: '不限',
            value: '0'
        }, {
            type: 'radioText',
            nameText: '最多',
            value: '1'
        }],
        descList: [{
            nameText: '行',
            value: '2'
        }, {
            nameText: '（超过将通过滚动栏展示）'
        }]
    }, {
        nameText: '配置方式',
        controlName: 'ConfigurationMode',
        type: 'radio',
        defaultValue: CONFIGURATION_MODE.CATEGORY_BRAND,
        valid: {
            isChecked: true
        },
        list: [{
            nameText: '类目品牌',
            value: CONFIGURATION_MODE.CATEGORY_BRAND
        }, {
            nameText: '活动配置',
            value: CONFIGURATION_MODE.ACTIVE_CONFIGURATION
        }, {
            nameText: '自定义商品',
            value: CONFIGURATION_MODE.CUSTOM_GOODS
        }],
        descList: [{
            nameText: '（限200以内）'
        }]
    }];
}
/**
 * 品牌类目
 *
 * @export
 * @returns {Array}}
 */
export function getBrandCat () {
    const res = [{
        nameText: '商品类目',
        controlName: 'ConfigurationCategoryId',
        type: 'select',
        defaultValue: '',
        placeholder: '请选择',
        valid: {
            isChecked: true
        },
        list: [{
            nameText: '请选择',
            value: ''
        }, {
            nameText: '轮胎',
            value: 'Tires'
        }, {
            nameText: '车品',
            value: 'AutoProduct'
        }, {
            nameText: '轮毂',
            value: 'hub'
        }, {
            nameText: '保养',
            value: 'BaoYang'
        },
        //  {
        //     nameText: '礼品',
        //     value: 'Gifts'
        // },
        {
            nameText: '美容',
            value: 'MR1'
        }, {
            nameText: '汽车玻璃',
            value: 'QCBL'
        }]
    }, {
        nameText: '品牌',
        controlName: 'ConfigurationBrand',
        placeholder: '请选择',
        type: 'select',
        defaultValue: '',
        valid: {
            isChecked: true
        }
    }];
    return res;
}
/**
 * 获取活动配置
 *
 * @export
 * @param {object} $http xhr
 * @returns {Array}
 */
export function getActivityConfig ($http) {
    const config = [{
        nameText: '抢购ID',
        controlName: 'ConfigurationActivityId',
        validFlowTime: 1000,
        valid: [{
            required: true
        }, (con) => {
            return validCouponPid(con, $http, '', {
                PromotionIdKey: 'SweepstakesId',
                PromotionType: 'Limit',
                errText: '抢购ID不正确'
            });
        }],
        defaultValue: ''
    }];
    return config;
}
