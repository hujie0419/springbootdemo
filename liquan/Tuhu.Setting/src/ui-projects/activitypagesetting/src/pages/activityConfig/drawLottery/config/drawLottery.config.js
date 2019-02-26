// import { validCommonIdFunc } from '../../commons/commonValid/commonValid';
import { validCouponPid } from '../../commons/couponPidValid/couponPidValid';

/**
 * 获取抽奖活动的配置信息
 * @param {object} $http xhr
 * @param {string} lotteryType 选中的默认值
 * @returns {array} 表单的配置信息
 */
export function getDrawLotteryConfig ($http, lotteryType) {
    return [{
        nameText: '抽奖类型',
        valid: {
            required: true
        },
        formControl: [{
            nameText: '抽奖类型',
            controlName: 'LotteryType',
            type: 'radio',
            valid: {
                isChecked: true
            },
            defaultValue: lotteryType || 'XINDATURN',
            list: [ {
                nameText: '新大翻盘',
                disabled: true,
                value: 'XINDATURN'
            }, {
                nameText: '摇奖机',
                disabled: true,
                value: 'ERNIE'
            }, {
                nameText: '答题抽奖',
                disabled: true,
                value: 'ANSWERLUCKYDRAW'
            }, {
                nameText: '红包抽奖',
                disabled: true,
                value: 'ENVELOPEDRAW'
            }]
        }]
    }, {
        nameText: '活动ID',
        controlName: 'SweepstakesId',
        validFlowTime: 1000,
        valid: [{
            required: true
        }, (con) => {
            return validCouponPid(con, $http, '', {
                PromotionIdKey: 'SweepstakesId',
                PromotionType: 'CouponPackage',
                errText: '活动ID不正确'
            });
        }]
    }];
}
