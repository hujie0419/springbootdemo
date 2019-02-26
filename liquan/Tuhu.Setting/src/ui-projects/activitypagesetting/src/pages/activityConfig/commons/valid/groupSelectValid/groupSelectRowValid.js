/**
 * 验证选中输入的行数
 *
 * @export
 * @param {*} con 当前选中项的数据
 * @returns {Object| null}
 */
export function groupSelectRowValid (con) {
    let res = null;
    if (con && con.value && con.value.select + '' === '1') {
        if (con.value.value) {
            let validate = con.validate;
            let validFn = validate && validate.generateValidetor({
                required: true,
                requiredErr: '请输入展示行数',
                format: 'vNumber',
                formatErr: '展示行数只能为大于0的整数'
            });
            if (validFn instanceof Function) {
                return validFn(con.value);
            }
            // if (!(parseInt(con.value.value, 10) > 0)) {
            //     res = {
            //         errInfo: {
            //             desc: '展示行数只能为大于0的整数'
            //         }
            //     };
            // }
        } else {
            res = {
                errInfo: {
                    desc: '请输入展示行数'
                }
            };
        }
    }
    return res;
}
