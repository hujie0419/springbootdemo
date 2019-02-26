import Vue from 'vue';
/**
 * 格式化货币
 *
 * @export
 * @param {number} value 货币数量
 * @param {string|number} args 保留小数位数
 * @returns {string}
 */
export default function filterMoney(value, args) {
    let _res = '';

    if (value || value + '' === '0') {
        if (args) {
            let _number = Vue.filter('filter_number');
            _res = '￥' + _number(value, args); // '￥' + _number.transform(value, args || '2.2-2');
        } else {
            _res = '￥' + value;
        }
    }

    return _res;
}
