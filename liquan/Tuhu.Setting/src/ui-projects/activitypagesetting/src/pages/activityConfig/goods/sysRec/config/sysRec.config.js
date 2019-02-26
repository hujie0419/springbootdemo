import { groupSelectRowValid } from '../../../commons/valid/groupSelectValid/groupSelectRowValid';
/**
 * 获取系统图推荐的配置信息
 * @returns {array} 表单的配置信息
 */
export function getSysRecConfig () {
    return [{
        nameText: '每行数量',
        controlName: 'PushColumNumber',
        type: 'radio',
        defaultValue: 1,
        valid: {
            required: true
        },
        list: [{
            nameText: '1列',
            value: 1
        }, {
            nameText: '2列',
            value: 2
        }, {
            nameText: '3列',
            value: 3
        }]
    }, {
        nameText: '商品类目',
        controlName: 'PushCategory',
        type: 'radio',
        defaultValue: 'CarProducts',
        // requiredRight: true,
        valid: {
            // required: true
            isChecked: true
        },
        list: [{
            nameText: '车品',
            value: 'CarProducts'
        // },
        // {
        //     nameText: '轮胎',
        //     value: 'Tire'
        }]
    }, {
        nameText: '展示行数',
        controlName: 'PushRowNumber',
        type: 'groupSelect',
        defaultValue: {
            select: '0',
            value: 0
        },
        valid: [{
            isChecked: true
        }, groupSelectRowValid],
        list: [{
            nameText: '不限',
            type: 'radioEmpty',
            value: '0'
        }, {
            nameText: '最多',
            type: 'radioText',
            value: '1'
        }],
        descList: [{
            nameText: '行（超过将通过滚动栏展示）'
        }]
    }];
}
