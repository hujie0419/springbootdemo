
import { dateValid } from '../../commons/dateValid/dateValid';
/**
 * 表单配置
 *
 * @export
 * @returns {Array}
 */
export function getFormConfig() {
    return [{
        nameText: '活动页标题',
        controlName: 'ActivityTitle',
        valid: {
            required: true,
            maxLength: 20,
            maxLengthErr: '活动标题最多20字符!'
        }
    }, {
        nameText: '活动时间',
        type: 'datetimerange',
        controlName: 'date',
        formatDate: 'yyyy-MM-dd HH:mm:ss',
        defaultValue: '',
        valid: [{
            required: true
        }, (con) => {
            return dateValid(con);
        }]
    }, {
        nameText: '活动类型',
        controlName: 'ActivityTypeCode',
        type: 'select',
        valid: {
            isChecked: true
        }
    }, {
        nameText: '负责人',
        controlName: 'responsePerson',
        valid: {
            required: true
        }
    }];
}
