import { groupSelectRowValid } from '../../../commons/valid/groupSelectValid/groupSelectRowValid';
/**
 * 秒杀配置项
 *
 * @export
 * @returns {Array}
 */
export function seckillFormList() {
    return [{
        nameText: '每行数量',
        controlName: 'SpikeColumnNumber',
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
        }]
    }, {
        nameText: '展示行数',
        controlName: 'SpikeRowNumber',
        type: 'groupSelect',
        defaultValue: {
            select: '0',
            value: ''
        },
        valid: [{
            isChecked: true
        }, groupSelectRowValid],
        list: [{
            nameText: '全部',
            type: 'radioEmpty',
            value: '0'
        }, {
            nameText: '最多',
            type: 'radioText',
            value: '1',
            descList: [{
                nameText: '行'
            }]
        }],
        descList: [{
            nameText: '（超过将通过滚动栏展示）'
        }]
    }, {
        nameText: '秒杀场次',
        controlName: 'SpikeSessionList',
        type: 'radio',
        defaultValue: []
        // valid: [{
        //     isChecked: true
        // }, (con) => {
        //     if (!con || !con.value || con.value.length <= 0) {
        //         return {
        //             errInfo: {
        //                 desc: '请选择秒杀场次'
        //             }
        //         };
        //     }
        // }]
    }];
}
