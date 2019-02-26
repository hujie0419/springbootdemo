
import {couponIdValid} from '../../commons/couponIdValid/couponIdValid';
/**
 * 获取新的优惠券表单配置对象
 * @param {String | Number} index 表单的index
 * @param {XHR} $http xhr
 * @returns {Array}
 */
export function formItemConfig (index, $http) {
    return [{
        nameText: '优惠券' + index,
        validFlowTime: 1000,
        valid: [{
            required: true,
            requiredErr: '请输入优惠券'
        }, (con) => {
            return couponIdValid(con, $http);
        }],
        requiredRight: true,
        controlName: 'CouponId',
        placeholder: '请输入优惠券ID',
        list: []
    }, {
        nameText: '',
        controlName: 'ImageUrl',
        type: 'fileText',
        prefixList: [{
            nameText: '图片',
            type: 'img'
        }],
        readonly: true,
        valid: {
            required: true,
            requiredErr: '请上传图片'
        },
        requiredRight: true,
        descList: [{
            action: '',
            type: 'fileButton',
            validFile: {
                type: 'Image'
            },
            nameText: '上传图片'
        }]
    }];
}
