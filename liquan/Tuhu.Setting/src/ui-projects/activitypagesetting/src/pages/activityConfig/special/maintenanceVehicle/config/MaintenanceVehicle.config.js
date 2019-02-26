import { validCouponPid } from '../../../commons/couponPidValid/couponPidValid';

/** 获取保养分车型的配置信息
 *  @param {object} $http xhr
 *  @param {String | Number} index 表单的index
 *  @returns {array} 表单的配置信息
 */
export function getPageConfig ($http, index) {
    return [{
        nameText: '保养ID' + index,
        controlName: 'MaintenanceID',
        defaultValue: '',
        validFlowTime: 1000,
        valid: [{
            required: true,
            requiredErr: '请输入保养ID'
        }, (con) => {
            return validCouponPid(con, $http, '', {
                PromotionIdKey: 'MaintenanceID',
                PromotionType: 'Maintenance',
                errText: '保养ID不存在'
            });
        }]
    }];
}

/** 获取保养分车型的配置信息
 *  @returns {array} 表单的配置信息
 */
export function getCheckBox () {
    return [{
        nameText: '保养项目',
        controlName: 'MaintenanceType',
        defaultValue: 'MaintenancePricing',
        type: 'radio',
        valid: {
            required: true
        },
        list: [{
            nameText: '保养定价',
            value: 'MaintenancePricing'
        }]
    }];
}
