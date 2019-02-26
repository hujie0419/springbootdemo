/**
 * 获取子列表
 * @param {Object} con 时间控件对象
 *  @param {Boolean} mustEnd 结束日期是否必填
 * @returns {Boolean}
 */
export function dateValid(con, mustEnd = true) {
    let val = con.value;
    if (val.length === 1) { // 只设置一个开始时间
        if (!val[0]) {
            return {
                errInfo: {
                    desc: '请输入开始时间'
                }
            };
        }
    } else if (val.length === 2) { // 有两个时间控件
        if (!val[0]) {
            return {
                errInfo: {
                    desc: '请输入开始时间'
                }
            };
        }
        if (mustEnd) {
            if (!val[1]) {
                return {
                    errInfo: {
                        desc: '请输入结束时间'
                    }
                };
            }
            if (getTime(val[0]) >= getTime(val[1])) {
                return {
                    errInfo: {
                        desc: '结束时间必须大于开始时间'
                    }
                };
            }
        }
    }
    return false;
};

/**
 * 时间转换
 * @param {string|number|date} data 时间
 * @returns {number}
 */
function getTime(data) {
    let res = data;
    if (typeof data === 'string') {
        res = new Date(data.replace('-', '/'));
    }

    if (res instanceof Date) {
        res = res.getTime();
    }
    return res;
}
export default {dateValid};
