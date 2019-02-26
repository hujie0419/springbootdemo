
import { validCouponPid } from '../../commons/couponPidValid/couponPidValid';
/**
 * 校验函数
 *
 * @param {*} con 表单元素
 * @param {*} $http xhr
 * @returns {Promise}}
 */
export function validPromotionProduct(con, $http) {
    // let _formVal = (formModel && formModel.value) || {};
    return validCouponPid(con, $http, con.formGroup, {
        PromotionIdKey: 'LinkPromotionId',
        PromotionTypeKey: 'LinkPromotionType', // _formVal && _formVal['LinkPromotionType'],
        pidKey: 'LinkPid',
        priceKey: 'LinkCommodityPrice'});
}
