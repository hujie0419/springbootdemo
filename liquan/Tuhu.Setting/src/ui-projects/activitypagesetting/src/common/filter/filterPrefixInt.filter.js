import * as utils from '../utils/utils';

/**
 * 给数字加前缀
 *
 * @export
 * @param {(number|string)} value 内容
 * @param {number} [leng=1] 长度
 * @param {number} isExt 是否为后缀加0
 * @returns {string}
 */
export default function filterPrefixInt(value, leng= 1, isExt) {
    return utils.prefixInt(value, leng, isExt);
}
