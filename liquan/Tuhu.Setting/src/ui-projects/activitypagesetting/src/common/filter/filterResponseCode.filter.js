/**
 * 给数字加前缀
 *
 * @export
 * @param {any} data 返回的数据
 * @returns {boolean}
 */
export default function filterResponseCode(data) {
    let res = false;
    let code = data && data.ResponseCode;
    if (!data || !code || code === '0000' || code + '' === '0' || code === 'undefined' || code === '') {
        res = true;
    }
    return res;
}
