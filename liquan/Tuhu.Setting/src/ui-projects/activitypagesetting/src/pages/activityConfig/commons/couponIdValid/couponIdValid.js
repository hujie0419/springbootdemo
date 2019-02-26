
import apis from '../apis/pictureMap/pictureMapApi.js';
import filterResponseCode from '../../../../common/filter/filterResponseCode.filter';
/**
 * 校验优惠券函数
 *
 * @param {*} con 表单元素
 * @param {*} $http xhr
 * @returns {Promise}}
 */
export function couponIdValid(con, $http) {
    return new Promise((resolve, reject) => {
        $http.post(apis.IsProductId, {
            apiServer: 'apiServer',
            isLoading: true,
            isErrorData: false,
            isErrorMsg: true,
            data: {
                CouponId: con.value
            }
        }).subscribe(data => {
            const _data = data.data;
            if (filterResponseCode(_data)) {
                if (_data.IsCouponId === true || _data.IsProductId === true) {
                    resolve(null);
                } else {
                    resolve({
                        errInfo: {
                            desc: '优惠券ID不正确'
                        }
                    });
                }
            } else {
                reject(_data);
            }
        }, err => {
            reject(err);
        });
    });
}
export default {couponIdValid};
