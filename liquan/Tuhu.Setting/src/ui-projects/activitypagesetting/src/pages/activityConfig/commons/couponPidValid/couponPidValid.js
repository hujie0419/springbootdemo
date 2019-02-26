import apis from '../../commons/apis/commons/commonApi';
import filterResponseCode from '../../../../common/filter/filterResponseCode.filter';

/**
 * 校验函数
 *
 * @param {*} con 表单元素
 * @param {*} $http xhr
 * @param {*} formModel 表单模型
 * @param {*} options 验证项的key
 * @returns {Promise}}
 */
export function validCouponPid(con, $http, formModel, {PromotionIdKey, PromotionType, PromotionTypeKey, pidKey, priceKey, errText}) {
    return new Promise((resolve, reject) => {
        let _formMd = formModel;
        if (!_formMd && !con) {
            resolve(null);
            return;
        }
        let _formVal = _formMd && _formMd.value;
        let PromotionId = PromotionIdKey && _formVal && _formVal[PromotionIdKey];
        let pid = _formVal && pidKey && _formVal[pidKey];
        /**
         * 获优惠类型
         */
        PromotionType = PromotionType || (PromotionTypeKey && _formVal && _formVal[PromotionTypeKey]);

        if (!_formMd) { // 如果没有传formModel从con里取值
            PromotionId = PromotionIdKey && con.value;
            pid = pidKey && con.value;
        }
        if (!pid && !PromotionId) {
            resolve(null);
            return;
        }
        let _data = {
            PID: pid || ''
        };
        let _get = null;
        if (PromotionId) { // 有活动ID的时候
            _data = Object.assign(_data, {
                PromotionType: PromotionType,
                PromotionId: PromotionId
            });
            _get = $http.post(apis.GetPromotionProductPrice, {
                apiServer: 'apiServer',
                isErrorData: false,
                isErrorMsg: true,
                data: _data
            });
        } else { // 只有商品ID的时候
            _get = $http.post(apis.GetProductPrice, {
                apiServer: 'apiServer',
                isErrorData: false,
                isErrorMsg: true,
                data: _data
            });
        }
        // 清除上次的校验结果
        if (_formMd) {
            let _proId = PromotionIdKey && _formMd.get(PromotionIdKey);
            let _proType = PromotionTypeKey && _formMd.get(PromotionTypeKey);
            let _pid = pidKey && _formMd.get(pidKey);
            if (_proId) {
                _proId.sub && _proId.sub.unsubscribe();
                _proId.sub = null;
                _proId.valid = null;
            }
            if (_proType) {
                _proType.sub && _proType.sub.unsubscribe();
                _proType.sub = null;
                _proType.valid = null;
            }
            if (_pid) {
                _pid.sub && _pid.sub.unsubscribe();
                _pid.sub = null;
                _pid.valid = null;
            }
        }
        con = con || {};
        con.sub = _get.subscribe((res) => {
            let _res= res && res.data;
            // let _result =_res&& _res.PromotionIdResult;
            let errDesc = '';
            if (filterResponseCode(_res)) {
                if (_res.IsProductId === false) {
                    errDesc = 'PID不正确';
                }
                if (!errDesc) {
                    if (_res.PromotionIdResult === '0001') {
                        errDesc = '无效促销ID';
                    } else if (_res.PromotionIdResult === '0002') {
                        errDesc = '促销ID下无对应商品';
                    } else if (_res.PromotionIdResult === '0009') {
                        errDesc = '系统异常';
                    }
                }
                if (errDesc) {
                    let err = {
                        errInfo: {
                            desc: errText || errDesc
                        }
                    };
                    resolve(err);
                } else {
                    if (PromotionId) {
                        if (_res.PromotionPrice !== _formVal.PromotionPrice) {
                            _formMd && priceKey && _formMd.get(priceKey).setValue(_res.PromotionPrice);
                        }
                    } else if (_res.ProductPrice !== _formVal.ProductPrice) {
                    // 设置单价
                        _formMd && priceKey && _formMd.get(priceKey).setValue(_res.ProductPrice);
                    }

                    resolve(null);
                }
            } else {
                reject(res);
            }
        }, (err) => {
            reject(err);
        });
    });
}
