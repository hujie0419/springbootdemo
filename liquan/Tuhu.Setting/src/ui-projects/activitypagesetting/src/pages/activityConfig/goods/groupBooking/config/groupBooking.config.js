
/**
 * 拼团表单数据
 *
 * @export
 * @returns {Array}
 */
export function groupBookingFormList () {
    return [{
        nameText: '每行数量',
        controlName: 'colNum',
        type: 'radio',
        defaultValue: '1',
        valid: {
            required: true
        },
        list: [{
            nameText: '1列',
            value: '1'
        }, {
            nameText: '2列',
            value: '2'
        }, {
            nameText: '3列',
            value: '3'
        }]
    }, {
        nameText: '拼团配置',
        controlName: 'config',
        type: 'radio',
        defaultValue: '1',
        valid: {
            isChecked: true
        }
    }];
}
