
import { validCouponPid } from '../../../commons/couponPidValid/couponPidValid';
// import { validCommonIdFunc } from '../../../commons/commonValid/commonValid';
/**
 * 页面编辑的配置
 *
 * @export
 * @param {Object} $http xhr
 *
 * @returns {Array}}
 */
export function getEditPageConfig ($http) {
    // const { $$form } = Vue.prototype;
    const categoryConfig = [{
        nameText: '商品类目',
        controlName: 'ProductCategories',
        type: 'select',
        defaultValue: '',
        // valid: {
        //     required: true
        // },
        list: [{
            nameText: '不限',
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
        controlName: 'ProductBrand',
        type: 'select',
        // valid: {
        //     required: true
        // },
        defaultValue: '',
        list: []
    }];

    let config = [{
        nameText: '商品配置',
        formControl: [{
            showNameText: true,
            nameText: 'PID',
            controlName: 'PID',
            className: 'form-control-small',
            validFlowTime: 1000,
            valid: (con) => {
                // let _formVal = that.formModel && that.formModel.value;
                return validCouponPid(con, $http, con.formGroup, {
                    pidKey: 'PID',
                    PromotionIdKey: 'SalesPromotion',
                    // PromotionType: _formVal && _formVal['PromotionTyple']
                    PromotionTypeKey: 'PromotionTyple' // _formVal && _formVal['PromotionTyple']
                });
                // return validCommonIdFunc({
                //     formModel: con,
                //     pidKey: 'PID'
                // });
            }
        }, {
            showNameText: true,
            nameText: '促销ID',
            controlName: 'PromotionTyple',
            className: 'form-control-small',
            type: 'select',
            validFlowTime: 1000,
            valid: (con) => {
                // let _formVal = that.formModel && that.formModel.value;
                return validCouponPid(con, $http, con.formGroup, {
                    pidKey: 'PID',
                    PromotionIdKey: 'SalesPromotion',
                    // PromotionTypeKey: _formVal && _formVal['PromotionTyple']
                    PromotionTypeKey: 'PromotionTyple' // _formVal && _formVal['PromotionTyple']
                });
            },
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
            controlName: 'SalesPromotion',
            className: 'form-control-small',
            validFlowTime: 1000,
            valid: (con) => {
                // let _formVal = that.formModel && that.formModel.value;
                return validCouponPid(con, $http, con.formGroup, {
                    pidKey: 'PID',
                    PromotionIdKey: 'SalesPromotion',
                    // PromotionTypeKey: _formVal && _formVal['PromotionTyple']
                    PromotionTypeKey: 'PromotionTyple' // _formVal && _formVal['PromotionTyple']
                });
            }
        }]
    }];
    config = categoryConfig.concat(config);

    // if (formModel) {
    //     // 重新为商品类目模块设置初始化formControl
    //     formModel.removeItem('ProductCategories');
    //     formModel.merge($$form.initFormData(categoryConfig));
    // }

    // const brandConfig = [{
    //     nameText: '品牌',
    //     controlName: 'ProductBrand',
    //     type: 'select',
    //     defaultValue: '',
    //     list: []
    // }];
    return config;
}
