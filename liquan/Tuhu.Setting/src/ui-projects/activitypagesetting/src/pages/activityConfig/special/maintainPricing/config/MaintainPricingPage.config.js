// import { validCommonIdFunc } from '../../../commons/commonValid/commonValid';
import { validCouponPid } from '../../../commons/couponPidValid/couponPidValid';

/**
 * 获取保养定价的配置信息
 * @param {object} $http xhr
 * @returns {array} 表单的配置信息
 */
export function getMaintainPricingConfig ($http) {
    return [{
        nameText: '保养ID',
        controlName: 'MainPricingID',
        defaultValue: '',
        validFlowTime: 1000,
        valid: [{
            required: true
        }, (con) => {
            return validCouponPid(con, $http, '', {
                PromotionIdKey: 'MainPricingID',
                // PromotionTypeKey: 'Maintenance',
                PromotionType: 'Maintenance',
                errText: '保养ID不存在'
            });
            // return validCommonIdFunc({
            //     formModel: that.formModel,
            //     promotionKey: 'MainPricingID',
            //     type: 'Maintenance'
            // });
        }]
    }];
}
