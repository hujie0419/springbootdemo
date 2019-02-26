/**
 * 获取产品表单的配置信息
 * @returns {array} 表单的配置信息
 */
export function getCountDownConfig () {
    return [{
        nameText: '倒计时样式',
        controlName: 'CountdownStyle',
        type: 'radio',
        defaultValue: 'Deep',
        valid: {
            isChecked: true
        },
        list: [{
            nameText: '深色背景',
            value: 'Deep'
        }, {
            nameText: '浅色背景',
            value: 'Shallow'
        }]
    }];
}
