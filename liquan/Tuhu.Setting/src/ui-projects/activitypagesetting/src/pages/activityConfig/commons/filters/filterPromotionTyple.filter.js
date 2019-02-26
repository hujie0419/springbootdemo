/**
 * 促销类型
 *
 * @export
 * @param {String} typeCode 类型码
 * @returns {String} 对应名称
 */
export default function filterPromotionTyple (typeCode) {
    let res = '';
    switch (typeCode) {
        case 'Whole':
            res = '全网';
            break;
        case 'Limit':
            res = '限时';
            break;
        case 'Discount':
            res = '打折';
            break;

        default:
            break;
    }
    return res;
}
