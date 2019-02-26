/**
 * 给数字加前缀
 *
 * @export
 * @param {(number|string)} value 内容
 * @param {number} [leng=1] 长度
 * @param {number} isExt 是否为后缀加0
 * @returns {string}
 */
export function prefixInt(value, leng = 1, isExt) {
    let res = (value || '') + '';
    if (value || value + '' === '0') {
        res = value + '';
    }
    let _res = res;
    if (value.length < leng || (isExt && value.length !== leng && leng > -1)) {
        let fix = (new Array(leng + 1)).join('0');
        res = (((!isExt && fix) || '') + value + ((isExt && fix) || ''));
        if (isExt) {
            _res = res.slice(0, leng);
            if (parseInt(res.slice(leng, leng + 1), 10) >= 5) {
                let _num = (parseInt(_res, 10) + 1) + '';
                _res = _num;
            }
        } else {
            _res = res.slice(-leng);
        }
    }
    return _res;
}

/**
 * 过滤字符串
 * @param {any} data 需要过滤的数据
 * @return {any}
 */
export function filterStr (data) {
    if (data) {
        if (typeof data === 'string') {
            data = data.replace(/^\s*|\s*$/g, '');
        } else if (data instanceof Array) {
            data.map(item => {
                return filterStr(item);
            });
        } else if (data instanceof Object && (!data.constructor || data.constructor === Object)) {
            for (const key in data) {
                if (data.hasOwnProperty(key)) {
                    const item = data[key];
                    data[key] = filterStr(item);
                }
            }
        }
    }
    return data;
}
