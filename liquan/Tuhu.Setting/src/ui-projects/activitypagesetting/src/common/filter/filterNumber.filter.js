import Vue from 'vue';

/**
 * 格式化数字
 *
 * @export
 * @param {number} value 需要格式化的数字
 * @param {(string|number)} [args] 格式(如：1.2-3)
 * @returns {string}
 */
export default function filterNumber(value, args) {
    let _res = '';
    let isReset = false;
    value = value + '' === '0' ? '0' : (value || '') + '';
    if (value || value + '' === '0') {
        if (args) {
            let valueArray = value.split('.') || [];
            let _decimal = decimal(value, args);
            let _numArr = [];
            let _prefixInt = Vue.filter('filter_prefixInt');

            if (typeof _decimal.prefix !== 'undefined') {
                _numArr.push(_prefixInt(valueArray[0] || 0, _decimal.prefix || 0) || 0);
            } else {
                _numArr.push(valueArray[0] || 0);
            }
            if (typeof _decimal.ext !== 'undefined') {
                let ext = _prefixInt(valueArray[1] || 0, _decimal.ext || 0, true) || 0;
                let str = (ext || 0) + '';
                let _ext = (str).substr(0, 1);
                let _oldExt = (valueArray[1] + '').substr(0, 1);

                let _ext1 = (str).substr(-1);
                let _oldExt1 = (valueArray[1] + '').substr(-1);
                if (_ext === '1' && _oldExt === '9') { // 修正小数进位
                    isReset = true;
                    _numArr[0] = _prefixInt((parseInt(valueArray[0] || 0, 10) || 0) + 1, _decimal.prefix || 0);
                    ext = str.substring(1).replace(/0*$/g, '');
                } else if (_ext1 === '0' && _oldExt1 !=='0') { // 修正进位后多余的0
                    isReset = true;
                    ext = str.replace(/0*$/g, '');
                }
                if (ext || ext + '' === '0') {
                    _numArr[1]=ext;
                }
            } else {
                if (typeof valueArray[1] !== 'undefined') {
                    _numArr.push(valueArray[1]);
                }
            }
            _res = _numArr.join('.');
        } else {
            _res = value;
        }
    }

    return isReset ? filterNumber(_res, args) : _res;
}

/**
 * 计算小数位数
 *
 * @param {(number|string)} num 需要格式化的数字
 * @param {(number|string)} [count] 格式(如：1.2-3)
 * @returns {object}
 */
function decimal(num, count) {
    let _res = {};
    num = (num || '') + '';
    count = (count || '') + '';
    let numArr = num.split('.') || [];
    let countArr = count.split('.') || [];
    let decimalArr = (countArr.length > 1 && countArr[1].split('-')) || [];
    if (count) {
        if (countArr.length === 1) {
            _res.ext = parseInt(countArr[0], 10);
        } else {
            if (((parseInt(countArr[0], 10) || 0) > 0 && !numArr[0]) || (parseInt(countArr[0], 10) || 0) > numArr[0].length) {
                _res.prefix = parseInt(countArr[0], 10);
            }

            if (decimalArr && decimalArr.length > 0) {
                if ((parseInt(decimalArr[0], 10) || 0) > ((!numArr[1] || 0) || ((numArr[1] && numArr[1].length) || 0))) {
                    _res.ext = parseInt(decimalArr[0], 10);
                } else if ((numArr[1] && decimalArr[1] && (parseInt(decimalArr[1], 10) || 0) < numArr[1].length)) {
                    _res.ext = parseInt(decimalArr[1], 10);
                }
            }
        }
    }

    return _res;
}
