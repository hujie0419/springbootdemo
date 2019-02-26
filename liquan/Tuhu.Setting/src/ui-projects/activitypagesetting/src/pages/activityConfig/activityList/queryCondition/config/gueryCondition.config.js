import dateValid from '../../../commons/dateValid/dateValid.js';
/**
 * 查询条件表单元素数据列表
 *
 * @export
 * @returns {Array}
 */
export function getGueryControlList() {
    let fromItemModule = [{
        nameText: '关键字',
        controlName: 'gjz',
        placeholder: '活动页id 或 标题关键字'
    }, {
        nameText: '创建人',
        controlName: 'cjr',
        type: 'text'
    }, {
        nameText: '活动时间',
        controlName: 'atime',
        type: 'userdate',
        formatDate: 'yyyy-MM-dd'
        // valid: [{
        //     required: true
        // }, function(con) {
        //     let valid = dateValid.dateValid(con, false);
        //     return valid;
        // }]
    }, {
        nameText: '创建时间',
        controlName: 'ctime',
        type: 'userdate'
    }, {
        nameText: '活动类型',
        controlName: 'h3',
        type: 'checkBox',
        defaultValue: [],
        list: []
    }];
    return fromItemModule;
}
